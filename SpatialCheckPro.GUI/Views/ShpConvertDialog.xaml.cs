#nullable enable
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SpatialCheckPro.GUI.Services;
using WinForms = System.Windows.Forms;

namespace SpatialCheckPro.GUI.Views
{
    /// <summary>
    /// FileGDB → Shapefile 변환 대화상자
    /// </summary>
    public partial class ShpConvertDialog : Window
    {
        private readonly ShpConvertService _convertService;
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _isConverting;

        /// <summary>
        /// 생성자
        /// </summary>
        public ShpConvertDialog()
        {
            InitializeComponent();
            _convertService = new ShpConvertService();
        }

        /// <summary>
        /// 소스 FileGDB 찾아보기 버튼 클릭
        /// </summary>
        private void BrowseSource_Click(object sender, RoutedEventArgs e)
        {
            using var folderDialog = new WinForms.FolderBrowserDialog
            {
                Description = "File Geodatabase(.gdb) 폴더를 선택하세요",
                ShowNewFolderButton = false
            };

            if (folderDialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                var selectedPath = folderDialog.SelectedPath;

                // .gdb 확장자 확인
                if (!selectedPath.EndsWith(".gdb", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "선택한 폴더가 File Geodatabase(.gdb)가 아닙니다.\n.gdb 확장자를 가진 폴더를 선택해주세요.",
                        "잘못된 선택",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                SourcePathTextBox.Text = selectedPath;
                
                // 자동으로 출력 경로 설정: FileGDB명_out 폴더
                SetDefaultOutputPath(selectedPath);
                
                UpdateLayerInfo(selectedPath);
                UpdateConvertButtonState();
            }
        }

        /// <summary>
        /// 소스 FileGDB 경로를 기반으로 기본 출력 경로를 설정합니다
        /// 출력 경로: FileGDB가 있는 디렉토리에 "FileGDB명_out" 폴더
        /// </summary>
        private void SetDefaultOutputPath(string gdbPath)
        {
            try
            {
                // FileGDB의 상위 디렉토리
                var parentDir = Path.GetDirectoryName(gdbPath);
                if (string.IsNullOrEmpty(parentDir)) return;

                // FileGDB 폴더명에서 .gdb 제거
                var gdbName = Path.GetFileNameWithoutExtension(gdbPath);
                
                // 출력 폴더 경로: "FileGDB명_out"
                var outputDir = Path.Combine(parentDir, $"{gdbName}_out");
                
                OutputPathTextBox.Text = outputDir;
            }
            catch
            {
                // 경로 생성 실패 시 무시 (수동으로 선택하도록)
            }
        }

        /// <summary>
        /// 출력 폴더 찾아보기 버튼 클릭
        /// </summary>
        private void BrowseOutput_Click(object sender, RoutedEventArgs e)
        {
            using var folderDialog = new WinForms.FolderBrowserDialog
            {
                Description = "Shapefile을 저장할 폴더를 선택하세요",
                ShowNewFolderButton = true
            };

            if (folderDialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                OutputPathTextBox.Text = folderDialog.SelectedPath;
                UpdateConvertButtonState();
            }
        }

        /// <summary>
        /// FileGDB의 레이어 정보를 표시합니다
        /// </summary>
        private void UpdateLayerInfo(string gdbPath)
        {
            try
            {
                var layerInfos = _convertService.GetLayerInfos(gdbPath);
                
                if (layerInfos.Count == 0)
                {
                    LayerInfoText.Text = "레이어가 없습니다.";
                    return;
                }

                var infoText = $"총 {layerInfos.Count}개 레이어:\n";
                foreach (var info in layerInfos)
                {
                    infoText += $"  • {info.Name} ({info.GeometryType}, {info.FeatureCount:N0}개 피처)\n";
                }

                LayerInfoText.Text = infoText.TrimEnd('\n');
            }
            catch (Exception ex)
            {
                LayerInfoText.Text = $"레이어 정보 조회 실패: {ex.Message}";
            }
        }

        /// <summary>
        /// 변환 버튼 활성화 상태 업데이트
        /// </summary>
        private void UpdateConvertButtonState()
        {
            ConvertButton.IsEnabled = !string.IsNullOrWhiteSpace(SourcePathTextBox.Text)
                                   && !string.IsNullOrWhiteSpace(OutputPathTextBox.Text)
                                   && !_isConverting;
        }

        /// <summary>
        /// 변환 실행 버튼 클릭
        /// </summary>
        private async void Convert_Click(object sender, RoutedEventArgs e)
        {
            if (_isConverting)
            {
                // 변환 중이면 취소
                _cancellationTokenSource?.Cancel();
                return;
            }

            var sourcePath = SourcePathTextBox.Text;
            var outputPath = OutputPathTextBox.Text;

            // 유효성 검사
            if (!Directory.Exists(sourcePath))
            {
                MessageBox.Show("소스 FileGDB 경로가 존재하지 않습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Directory.Exists(outputPath))
            {
                try
                {
                    Directory.CreateDirectory(outputPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"출력 폴더 생성 실패: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // 변환 시작
            _isConverting = true;
            _cancellationTokenSource = new CancellationTokenSource();
            ConvertButton.Content = "취소";
            ConvertButton.IsEnabled = true;
            StatusText.Text = "변환 준비 중...";
            ConvertProgressBar.Value = 0;

            try
            {
                var progress = new Progress<ShpConvertProgress>(p =>
                {
                    ConvertProgressBar.Value = p.OverallProgress;
                    StatusText.Text = p.StatusMessage;
                });

                var result = await _convertService.ConvertAsync(
                    sourcePath,
                    outputPath,
                    progress,
                    _cancellationTokenSource.Token);

                if (result.Success)
                {
                    ConvertProgressBar.Value = 100;
                    StatusText.Text = $"완료: {result.ConvertedCount}개 레이어 변환됨";
                    
                    var openFolder = MessageBox.Show(
                        $"변환이 완료되었습니다.\n\n성공: {result.ConvertedCount}개\n실패: {result.FailedCount}개\n\n출력 폴더를 여시겠습니까?",
                        "변환 완료",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                    if (openFolder == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start("explorer.exe", outputPath);
                    }
                }
                else
                {
                    StatusText.Text = $"변환 실패: {result.ErrorMessage}";
                    MessageBox.Show(
                        $"변환 중 오류가 발생했습니다.\n\n{result.ErrorMessage}",
                        "변환 오류",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (OperationCanceledException)
            {
                StatusText.Text = "사용자에 의해 취소됨";
                ConvertProgressBar.Value = 0;
            }
            catch (Exception ex)
            {
                StatusText.Text = $"오류: {ex.Message}";
                MessageBox.Show(
                    $"변환 중 예기치 않은 오류가 발생했습니다.\n\n{ex.Message}",
                    "오류",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                _isConverting = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
                ConvertButton.Content = "변환 실행";
                UpdateConvertButtonState();
            }
        }

        /// <summary>
        /// 닫기 버튼 클릭
        /// </summary>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (_isConverting)
            {
                var result = MessageBox.Show(
                    "변환이 진행 중입니다. 취소하고 닫으시겠습니까?",
                    "변환 진행 중",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _cancellationTokenSource?.Cancel();
                    Close();
                }
            }
            else
            {
                Close();
            }
        }
    }
}

