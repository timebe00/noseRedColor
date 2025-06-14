using OpenCvSharp;
using System.IO;
using System.Reflection;

namespace noseRedColor.Content
{
    public static class FaceProcessor
    {
        public static Mat AddRudolphNose(Mat image)
        {
            //  실행하는 파일 경로 가져오기
            string locationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //  얼굴 검출용 데이터 가져오기
            string[] locationPaths = locationPath.Split("noseRedColor");
            string rootPath = string.Join("noseRedColor", locationPaths.Take(locationPaths.Length - 1));

            //  훈련데이터 경로 가져오기
            string haarcascadePath = Path.Combine(rootPath, "noseRedColor\\Resources\\xml", "haarcascade_frontalface_default.xml");

            //  얼굴에 중심 부분(코) 추출하기
            CascadeClassifier faceCascade = new CascadeClassifier(haarcascadePath);
            Rect[] faces = faceCascade.DetectMultiScale(image);

            //  빨강색 원 그리기
            foreach (var face in faces)
            {
                Point noseCenter = new Point(face.X + face.Width / 2, face.Y + face.Height / 2);
                Cv2.Circle(image, noseCenter, 50, new Scalar(0, 0, 255), -1);
            }

            return image;
        }
    }
}
