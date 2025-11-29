using System;
using System.Threading;
using System.Threading.Tasks;
using OSGeo.OGR;
using SpatialCheckPro.Models;
using SpatialCheckPro.Models.Config;

namespace SpatialCheckPro.Processors.RelationChecks
{
    public interface IRelationCheckStrategy
    {
        /// <summary>
        /// 이 전략이 처리하는 CaseType (예: "PointInsidePolygon")
        /// </summary>
        string CaseType { get; }

        /// <summary>
        /// 관계 검수 실행
        /// </summary>
        Task ExecuteAsync(
            DataSource ds,
            Func<string, Layer?> getLayer,
            ValidationResult result,
            RelationCheckConfig config,
            Action<RelationValidationProgressEventArgs> onProgress,
            CancellationToken token);
    }
}
