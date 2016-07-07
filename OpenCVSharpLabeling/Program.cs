using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace OpenCVSharpLabeling
{
    class Program
    {
        static void Main(string[] args)
        {
            Mat src = new Mat("0.png", ImreadModes.Color);
            Mat gray = src.CvtColor(ColorConversionCodes.BGR2GRAY);
            Mat binary = gray.Threshold(0, 255, ThresholdTypes.Otsu | ThresholdTypes.Binary);

            ConnectedComponents cc = Cv2.ConnectedComponentsEx(binary);

            #region 전체 이미지에서 픽셀별로 어떤 라벨인지 확인
            Mat Color = src.EmptyClone();
            var colorIndexer = Color.GetGenericIndexer<Vec3b>();
            Scalar[] colors = Enumerable.Range(0, cc.LabelCount).Select(_ => Scalar.RandomColor()).ToArray();

            for (int y = 0; y < src.Height; y++)
            {
                for (int x = 0; x < src.Width; x++)
                {
                    int labelValue = cc.Labels[y, x];

                    colorIndexer[y, x] = colors[labelValue].ToVec3b();
                }
            }

            Cv2.NamedWindow("전체이미지에서 픽셀별 라벨 인덱스 확인", WindowMode.AutoSize);

            Cv2.ImShow("전체이미지에서 픽셀별 라벨 인덱스 확인", Color);
            #endregion


            #region 특정 라벨에 접근
            Cv2.NamedWindow("특정 라벨 접근", WindowMode.AutoSize);
            foreach (var blob in cc.Blobs.Skip(0))
            {
                src.Rectangle(blob.Rect, Scalar.Red);
            }
            Cv2.ImShow("특정 라벨 접근", src);
            #endregion

            #region 가장 큰 blob 찾기
            //ConnectedComponents.Blob maxBlob1 = cc.Blobs.OrderByDescending(b => b.Area).First(); // 이렇게 하면 거의 배경이 나옴
            ConnectedComponents.Blob maxBlob1 = cc.Blobs.Skip(1).OrderByDescending(b => b.Area).First(); //그래서 이렇게 하나를 스킵함
            //위와 동일한 코드. 간결함
            ConnectedComponents.Blob maxBlob2 = cc.GetLargestBlob();

            var filtered = new Mat();
            //가장 큰 Blob 만 필터링하여 빈 이미지에 그리기
            cc.FilterByBlob(src, filtered, maxBlob1);

            Cv2.NamedWindow("maxBlob2", WindowMode.AutoSize);
            Cv2.ImShow("maxBlob2", filtered);
            #endregion

            #region 
            var dst = new Mat();
            cc.RenderBlobs(dst);

            Cv2.NamedWindow("label 을 빈 Mat에 그리기", WindowMode.AutoSize);
            Cv2.ImShow("label 을 빈 Mat에 그리기", dst);
            #endregion

            int c = Cv2.WaitKey();
        }
    }
}
