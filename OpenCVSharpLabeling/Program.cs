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
            Mat binary = gray.Threshold(0, 255, ThresholdTypes.Otsu | 
                ThresholdTypes.Binary);

            ConnectedComponents cc = Cv2.ConnectedComponentsEx(binary);

            #region PointLabelIndex

            Mat pixelLabel = src.EmptyClone();
            var pixelLabelIndexer = pixelLabel.GetGenericIndexer<Vec3b>();
            Scalar[] colors = Enumerable.Range(0, cc.LabelCount).Select
                (_ => Scalar.RandomColor()).ToArray();

            for (int y = 0; y < src.Height; y++)
            {
                for (int x = 0; x < src.Width; x++)
                {
                    int labelValue = cc.Labels[y, x];

                    pixelLabelIndexer[y, x] = colors[labelValue].ToVec3b();
                }
            }

            Cv2.NamedWindow("PointLabelIndex", WindowMode.AutoSize);
            Cv2.ImShow("PointLabelIndex", pixelLabel);

            #endregion

            #region Connect Labels

            // Skip(1) mean background skip
            foreach (var blob in cc.Blobs.Skip(1)) 
            {
                src.Rectangle(blob.Rect, Scalar.Red);
            }

            Cv2.NamedWindow("ConnectLabels", WindowMode.AutoSize);
            Cv2.ImShow("ConnectLabels", src);

            #endregion

            #region Get Max Blob And Show

            // Get Max Blob And Skip(1) mean background skip
            ConnectedComponents.Blob maxBlob = cc.Blobs.Skip(1).
                OrderByDescending(b => b.Area).First();
            //maxBlob and maxBlobSimple is same Blob
            ConnectedComponents.Blob maxBlobSimple = cc.GetLargestBlob(); 

            //Filtering Blob
            var filtered = new Mat();
            cc.FilterByBlob(src, filtered, maxBlobSimple);

            Cv2.NamedWindow("FilteringMaxBlobSimple", WindowMode.AutoSize);
            Cv2.ImShow("FilteringMaxBlobSimple", filtered);

            #endregion

            #region RenderBlobs

            var dst = new Mat();
            cc.RenderBlobs(dst);

            Cv2.NamedWindow("RenderBlobs", WindowMode.AutoSize);
            Cv2.ImShow("RenderBlobs", dst);

            #endregion

            int c = Cv2.WaitKey();
        }
    }
}
