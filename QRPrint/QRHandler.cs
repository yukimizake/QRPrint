using System;
using System.Drawing;
using System.Text;
using MessagingToolkit.QRCode.Codec;

namespace QRPrint
{
	public class QRHandler
	{
		public string Contents;
		public int Version = 10;
		
		public QRHandler (string contents)
		{
			if (contents == null) {
				throw(new Exception("Contents string cannot be null."));
			}
			
			if (contents.Length > 213) {
				contents = contents.Substring(0,213);
			}
			
			Contents = contents;
			
			int length = contents.Length;
			
			if (length <= 180) {
				Version = 9;
			}
			if (length <= 152) {
				Version = 8;
			}
			if (length <= 122) {
				Version = 7;
			}
			if (length <= 106) {
				Version = 6;
			}
			if (length <= 84) {
				Version = 5;
			}
			if (length <= 62) {
				Version = 4;
			}
			if (length <= 42) {
				Version = 3;
			}
			if (length <= 26) {
				Version = 2;
			}
			if (length <= 14) {
				Version = 1;
			}
		}
		
		public Bitmap GetImage()
		{
			QRCodeEncoder qrEnc = new QRCodeEncoder();
			qrEnc.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
			qrEnc.QRCodeScale = 1;
			qrEnc.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
			qrEnc.QRCodeVersion = Version;
			Bitmap image = qrEnc.Encode(Contents,Encoding.UTF8);
			
			double ratio = ((double)image.Height/(double)image.Width);
			int newHeight = Convert.ToInt32(384*ratio);
			
			image = ResizeImage(image,384,newHeight);
			
			return image;
		}
		
		/// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            //a holder for the result
            Bitmap result = new Bitmap(width, height);
			
            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                //draw the image into the target bitmap
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
			}

            //return the resulting bitmap
            return result;
        }
	}

}

