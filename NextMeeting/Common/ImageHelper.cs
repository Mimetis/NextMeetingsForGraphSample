using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI;
using System.IO;
using Windows.Graphics.Imaging;

namespace NextMeeting.Common
{
    public class ImageHelper
    {

        private static Uri unknownPersonUri = new Uri("ms-appx:///Assets/UnknownPerson.jpg");
        private static BitmapImage unknownPersonBitmapImage = new BitmapImage(unknownPersonUri);
        private static ImageHelper current;

        public static ImageHelper Current
        {
            get
            {
                if (current == null)
                    current = new ImageHelper();

                return current;
            }
        }


        public static BitmapImage UnknownPersonImage
        {
            get
            {
                return unknownPersonBitmapImage;
            }
        }

     

        public static async Task<BitmapImage> SaveImageToCacheAndGetImage(Byte[] imageArray, string fileName)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            var bitmapImage = new BitmapImage();

            try
            {
                using (MemoryStream memoryStream = new MemoryStream(imageArray))
                {
                    using (var randomAccessStream = memoryStream.AsRandomAccessStream())
                    {
                        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(randomAccessStream);

                        uint width = 80;
                        var height = decoder.OrientedPixelHeight * width / decoder.OrientedPixelWidth;

                        using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(stream, decoder);

                            encoder.BitmapTransform.ScaledHeight = height;
                            encoder.BitmapTransform.ScaledWidth = width;

                            await encoder.FlushAsync();

                            stream.Seek(0);
                            await bitmapImage.SetSourceAsync(stream);
                        }
                    }

                }
                return bitmapImage;

            }
            catch (Exception ex)
            {

            }

            return null;

        }
     

        public static async Task<Tuple<BitmapImage, Uri>> GetImageFromCache(string fileName)
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;

                var file = await folder.TryGetItemAsync(fileName) as StorageFile;

                if (file == null)
                    return null;

                BitmapImage bitmapImage = new BitmapImage();
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    await bitmapImage.SetSourceAsync(stream);
                }

                var imgUri = new Uri("ms-appdata:///local/" + fileName);

                // bitmapImage.UriSource = new Uri(file.Path);
                return new Tuple<BitmapImage, Uri>(bitmapImage, imgUri);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }

        }


    }
}
