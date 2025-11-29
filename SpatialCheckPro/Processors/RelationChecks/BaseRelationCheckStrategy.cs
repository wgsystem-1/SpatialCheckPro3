using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OSGeo.OGR;
using SpatialCheckPro.Models;
using SpatialCheckPro.Models.Config;
using SpatialCheckPro.Models.Enums;
using SpatialCheckPro.Utils;

namespace SpatialCheckPro.Processors.RelationChecks
{
    public abstract class BaseRelationCheckStrategy : IRelationCheckStrategy
    {
        protected readonly ILogger _logger;
        private DateTime _lastProgressUpdate = DateTime.MinValue;
        private const int PROGRESS_UPDATE_INTERVAL_MS = 200;

        protected BaseRelationCheckStrategy(ILogger logger)
        {
            _logger = logger;
        }

        public abstract string CaseType { get; }

        public abstract Task ExecuteAsync(
            DataSource ds,
            Func<string, Layer?> getLayer,
            ValidationResult result,
            RelationCheckConfig config,
            Action<RelationValidationProgressEventArgs> onProgress,
            CancellationToken token);

        protected void RaiseProgress(
            Action<RelationValidationProgressEventArgs> onProgress,
            string ruleId, 
            string caseType, 
            long processedLong, 
            long totalLong, 
            bool completed = false, 
            bool successful = true)
        {
            var now = DateTime.Now;
            if (!completed && (now - _lastProgressUpdate).TotalMilliseconds < PROGRESS_UPDATE_INTERVAL_MS)
            {
                return;
            }
            _lastProgressUpdate = now;

            var processed = (int)Math.Min(int.MaxValue, Math.Max(0, processedLong));
            var total = (int)Math.Min(int.MaxValue, Math.Max(0, totalLong));
            var pct = total > 0 ? (int)Math.Min(100, Math.Round(processed * 100.0 / (double)total)) : (completed ? 100 : 0);
            
            var eventArgs = new RelationValidationProgressEventArgs
            {
                CurrentStage = RelationValidationStage.SpatialRelationValidation,
                StageName = string.IsNullOrWhiteSpace(caseType) ? "공간 관계 검수" : caseType,
                OverallProgress = pct,
                StageProgress = completed ? 100 : pct,
                StatusMessage = completed
                    ? $"규칙 {ruleId} 처리 완료 ({processed}/{total})"
                    : $"규칙 {ruleId} 처리 중... {processed}/{total}",
                CurrentRule = ruleId,
                ProcessedRules = processed,
                TotalRules = total,
                IsStageCompleted = completed,
                IsStageSuccessful = successful,
                ErrorCount = 0,
                WarningCount = 0
            };
            
            onProgress?.Invoke(eventArgs);
        }

        protected void AddError(ValidationResult result, string errType, string message, string table = "", string objectId = "", Geometry? geometry = null, string tableDisplayName = "")
        {
            result.IsValid = false;
            result.ErrorCount += 1;
            
            var (x, y) = ExtractCentroid(geometry);
            result.Errors.Add(new ValidationError
            {
                ErrorCode = errType,
                Message = message,
                TableId = string.IsNullOrWhiteSpace(table) ? null : table,
                TableName = !string.IsNullOrWhiteSpace(tableDisplayName) ? tableDisplayName : string.Empty,
                FeatureId = objectId,
                SourceTable = string.IsNullOrWhiteSpace(table) ? null : table,
                SourceObjectId = long.TryParse(objectId, NumberStyles.Any, CultureInfo.InvariantCulture, out var oid) ? oid : null,
                Severity = Models.Enums.ErrorSeverity.Error,
                X = x,
                Y = y,
                GeometryWKT = QcError.CreatePointWKT(x, y)
            });
        }

        protected void AddDetailedError(ValidationResult result, string errType, string message, string table = "", string objectId = "", string additionalInfo = "", Geometry? geometry = null, string tableDisplayName = "")
        {
            result.IsValid = false;
            result.ErrorCount += 1;
            
            var fullMessage = string.IsNullOrWhiteSpace(additionalInfo) ? message : $"{message} ({additionalInfo})";
            var (x, y) = ExtractCentroid(geometry);
            
            result.Errors.Add(new ValidationError
            {
                ErrorCode = errType,
                Message = fullMessage,
                TableId = string.IsNullOrWhiteSpace(table) ? null : table,
                TableName = !string.IsNullOrWhiteSpace(tableDisplayName) ? tableDisplayName : string.Empty,
                FeatureId = objectId,
                SourceTable = string.IsNullOrWhiteSpace(table) ? null : table,
                SourceObjectId = long.TryParse(objectId, NumberStyles.Any, CultureInfo.InvariantCulture, out var oid) ? oid : null,
                Severity = Models.Enums.ErrorSeverity.Error,
                X = x,
                Y = y,
                GeometryWKT = QcError.CreatePointWKT(x, y)
            });
        }

        protected (double X, double Y) ExtractCentroid(Geometry? geometry)
        {
            if (geometry == null)
                return (0, 0);

            try
            {
                var geomType = geometry.GetGeometryType();
                var flatType = (wkbGeometryType)((int)geomType & 0xFF);

                if (flatType == wkbGeometryType.wkbPolygon || flatType == wkbGeometryType.wkbMultiPolygon)
                {
                    return GeometryCoordinateExtractor.GetPolygonInteriorPoint(geometry);
                }

                if (flatType == wkbGeometryType.wkbLineString || flatType == wkbGeometryType.wkbMultiLineString)
                {
                    return GeometryCoordinateExtractor.GetLineStringMidpoint(geometry);
                }

                if (flatType == wkbGeometryType.wkbPoint || flatType == wkbGeometryType.wkbMultiPoint)
                {
                    return GeometryCoordinateExtractor.GetFirstVertex(geometry);
                }

                return GeometryCoordinateExtractor.GetEnvelopeCenter(geometry);
            }
            catch
            {
                return (0, 0);
            }
        }

        protected Geometry? GetGeometryByOID(Layer? layer, long oid)
        {
            if (layer == null)
                return null;

            try
            {
                layer.SetAttributeFilter($"OBJECTID = {oid}");
                layer.ResetReading();
                var feature = layer.GetNextFeature();
                layer.SetAttributeFilter(null);

                if (feature != null)
                {
                    using (feature)
                    {
                        var geometry = feature.GetGeometryRef();
                        return geometry?.Clone();
                    }
                }
            }
            catch
            {
                layer.SetAttributeFilter(null);
            }

            return null;
        }

        protected IDisposable ApplyAttributeFilterIfMatch(Layer layer, string fieldFilter)
        {
            if (string.IsNullOrWhiteSpace(fieldFilter))
            {
                return new ActionOnDispose(() => { });
            }

            try
            {
                layer.SetAttributeFilter(fieldFilter);
                return new ActionOnDispose(() => layer.SetAttributeFilter(null));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "AttributeFilter 적용 실패: {Filter}", fieldFilter);
                layer.SetAttributeFilter(null);
                return new ActionOnDispose(() => { });
            }
        }

        protected bool IsLineWithinPolygonWithTolerance(Geometry line, Geometry polygon, double tolerance)
        {
            if (line == null || polygon == null) return false;
            
            try
            {
                var pointCount = line.GetPointCount();
                
                for (int i = 0; i < pointCount; i++)
                {
                    var x = line.GetX(i);
                    var y = line.GetY(i);

                    using var pt = new Geometry(wkbGeometryType.wkbPoint);
                    pt.AddPoint(x, y, 0);

                    var dist = pt.Distance(polygon);
                    
                    if (dist > tolerance)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "선형 객체 허용오차 검사 중 오류 발생");
                return false;
            }
        }

        protected class ActionOnDispose : IDisposable
        {
            private readonly Action _onDispose;
            public ActionOnDispose(Action onDispose) { _onDispose = onDispose; }
            public void Dispose() { _onDispose?.Invoke(); }
        }
    }
}
