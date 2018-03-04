using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using TravelActive.Data;
using TravelActive.Models.ViewModels;
using ZXing;
using ZXing.QrCode;

namespace TravelActive.Services
{
    public class QrCodeService : Service
    {

        public QrCodeService(TravelActiveContext context) : base(context)
        {
        }


        public PictureViewModel GenerateQrCodeImage(dynamic qrCodeContent)
        {
            var value = JsonConvert.SerializeObject(qrCodeContent);
            var width = 250;
            var height = 250;
            var margin = 0;
            var qrCodeWriter = new BarcodeWriterPixelData()
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions()
                {
                    Height = height,
                    Width = width,
                    Margin = margin
                }
            };
            var pixelData = qrCodeWriter.Write(value);
            var picture = new PictureViewModel();
            picture.Name = $"Qr code for {qrCodeContent.BusName}";
            picture.MediaType = "image/jpg";
            picture.Type = "File";
            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
            {
                using (var ms = new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                        ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                    try
                    {
                        Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                    bitmap.Save(ms, ImageFormat.Png);
                    picture.Value = Convert.ToBase64String(ms.ToArray());
                }
            }

            return picture;
        }
    }
}