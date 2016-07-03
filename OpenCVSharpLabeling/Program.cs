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

            Mat label = new Mat();
            int nLabels = Cv2.ConnectedComponents(binary, label, PixelConnectivity.Connectivity8, MatType.CV_32SC1);

            Scalar[] colors = Enumerable.Range(0, nLabels + 1).Select(_ => Scalar.RandomColor()).ToArray();
            colors[0] = Scalar.Black;

            int rows = binary.Rows;
            int cols = binary.Cols;
            var dst = new Mat(rows, cols, MatType.CV_8UC3);
            var labelIndexer = label.GetGenericIndexer<int>();
            var dstIndexer = dst.GetGenericIndexer<Vec3b>();

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int labelValue = labelIndexer[y, x];
                    dstIndexer[y, x] = colors[labelValue].ToVec3b();
                }
            }

            Cv2.NamedWindow("dst", WindowMode.AutoSize);

            Cv2.ImShow("dst", dst);

            //ConnectedComponents cc = Cv2.ConnectedComponentsEx(binary);

            //#region 전체 이미지에서 픽셀별로 어떤 라벨인지 확인
            //Mat Color = src.EmptyClone();
            //var colorIndexer = Color.GetGenericIndexer<Vec3b>();
            //Scalar[] colors = Enumerable.Range(0, cc.LabelCount).Select(_ => Scalar.RandomColor()).ToArray();

            //for (int y = 0; y < src.Height; y++)
            //{
            //    for (int x = 0; x < src.Width; x++)
            //    {
            //        int labelValue = cc.Labels[y, x];

            //        colorIndexer[y, x] = colors[labelValue].ToVec3b();
            //    }
            //}

            //Cv2.NamedWindow("전체이미지에서 픽셀별 라벨 확인", WindowMode.AutoSize);

            //Cv2.ImShow("전체이미지에서 픽셀별 라벨 확인", Color);
            //#endregion

            //Cv2.NamedWindow("특정 라벨 접근");
            //foreach (var blob in cc.Blobs.Skip(0))
            //{
            //    src.Rectangle(blob.Rect, Scalar.Red);
            //}
            //Cv2.ImShow("특정 라벨 접근", src);

            //ConnectedComponents.Blob maxBlob1 = cc.Blobs.Skip(1).OrderByDescending(b => b.Area).First();

            //ConnectedComponents.Blob maxBlob2 = cc.GetLargestBlob(); // OpenCvSharp.Blob 상당의 편리한 메소드

            //var dst = new Mat();
            //cc.RenderBlobs(dst);

            //var maxBlob = cc.GetLargestBlob();
            //var filtered = new Mat();
            //cc.FilterByBlob(src, filtered, maxBlob);

            int c = Cv2.WaitKey();
        }
    }
}
