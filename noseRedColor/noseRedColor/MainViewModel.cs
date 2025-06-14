using noseRedColor.Content;
using OpenCvSharp;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace noseRedColor
{
    public class MainViewModel : INotifyPropertyChanged
    {
        //  웹캠 캡쳐 변수
        private VideoCapture _capture;
        //  카메라 실행 여부
        private bool _isCameraRunning;
        //  프레임 변수
        private Mat _frame;
        //  이미지 변수
        private BitmapSource _displayImage;

        public BitmapSource DisplayImage
        {
            get => _displayImage;
            set { _displayImage = value; OnPropertyChanged(); }
        }

        //  Start Camera 클릭 시
        public ICommand StartCameraCommand { get; }
        //  Stop Camera 클릭 시
        public ICommand StopCameraCommand { get; }
        //  Load Image 클릭 시
        public ICommand LoadImageCommand { get; }

        //  커멘드 초기화 및 이벤트 연결
        public MainViewModel()
        {
            StartCameraCommand = new RelayCommand(_ => StartCamera());
            StopCameraCommand = new RelayCommand(_ => StopCamera());
            LoadImageCommand = new RelayCommand(_ => LoadImage());
        }

        //  카메라 시작
        private async void StartCamera()
        {
            //  카메라 연결
            _capture = new VideoCapture(0);
            //  프레임 가져오기
            _frame = new Mat();
            //  카메라 실행 변수 활성환
            _isCameraRunning = true;

            //  카메라 실행
            while (_isCameraRunning)
            {
                //  카메라에서 프레임 가져오기
                _capture.Read(_frame);
                //  프레임이 있다면
                if (!_frame.Empty())
                {
                    //  코 위치에 빨강색 원 그리기
                    Mat result = await Task.Run(() => FaceProcessor.AddRudolphNose(_frame.Clone()));
                    //  화면에 송출
                    DisplayImage = ConvertMatToBitmapSource(result);
                }
            }
        }

        //  카메라 멈추기
        private void StopCamera()
        {
            //  카메라 송출 끄기
            _isCameraRunning = false;
            //  카메라 연결 종료
            _capture?.Release();
        }

        //  이미지 가져오기
        private void LoadImage()
        {
            if(_isCameraRunning)
            {
                StopCamera();
            }

            //  이미지 가져오기 창 열기
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //  가져온 이미지가 있다면
            if (dlg.ShowDialog() == true)
            {
                //  이미지 로드
                Mat img = Cv2.ImRead(dlg.FileName);

                //  이미지에서 코 찾아서 빨강색 원 그리기
                var result = FaceProcessor.AddRudolphNose(img);
                //  화면 송출
                DisplayImage = ConvertMatToBitmapSource(result);
            }
        }

        //  이미지 화면 송출
        private BitmapSource ConvertMatToBitmapSource(Mat mat)
        {
            //  사진 크기 계산하기
            int stride = mat.Cols * mat.ElemSize();
            int size = stride * mat.Rows;
            byte[] data = new byte[size];
            Marshal.Copy(mat.Data, data, 0, size);

            //  사진 크기에 따라 픽셀 크기 결정
            PixelFormat format = mat.Channels() switch
            {
                1 => PixelFormats.Gray8,
                3 => PixelFormats.Bgr24,
                4 => PixelFormats.Bgra32,
                _ => throw new NotSupportedException("Unsupported channel count")
            };

            //  BitmapSource 생성
            return BitmapSource.Create(mat.Cols, mat.Rows, 96, 96,
                format, null, data, stride);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
