using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Reflection;
using static System.Windows.Forms.AxHost;

namespace SplitGifAnimation
{
    public partial class MainForm : Form
    {
        ReadOnlyDictionary<Guid, ImageFormat> guidToImageFormatMap = new ReadOnlyDictionary<Guid, ImageFormat>(new Dictionary<Guid, ImageFormat>()
                {
                    {ImageFormat.Bmp.Guid,  ImageFormat.Bmp},
                    {ImageFormat.Gif.Guid,  ImageFormat.Png},
                    {ImageFormat.Icon.Guid, ImageFormat.Png},
                    {ImageFormat.Jpeg.Guid, ImageFormat.Jpeg},
                    {ImageFormat.Png.Guid,  ImageFormat.Png}
                });

        public MainForm()
        {
            InitializeComponent();
        }
        private void dropPointLabel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
        private void dropPointLabel_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data is null) return;
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (filePaths.Length != 1) return;

            ProcessStartInfo psi = new ProcessStartInfo(Environment.CurrentDirectory + @"\Vendor\zopflipng.exe");
            psi.CreateNoWindow = true;

            List<byte[]> frames = EnumerateFrames(filePaths[0]);
            if (frames == null || frames.Count() == 0)
            {
                throw new NoNullAllowedException("Unable to obtain frames from " + filePaths[0]);
            }
            for (int i = 0; i < frames.Count(); i++)
            {
                string output = String.Format("frame-{0}.png", i);
                ConvertBytesToImage(frames[i]).Save(output);
                psi.Arguments = String.Format("{0} {0} -y", output);
                Process.Start(psi)?.WaitForExit();
            }
        }
        private List<byte[]> EnumerateFrames(string imagePath)
        {
            try
            {
                if (!File.Exists(imagePath))
                {
                    throw new FileNotFoundException("Unable to locate " + imagePath);
                }

                List<byte[]> tmpFrames = new List<byte[]>() { };

                using (Image img = Image.FromFile(imagePath, true))
                {
                    //Check the image format to determine what
                    //format the image will be saved to the 
                    //memory stream in
                    ImageFormat imageFormat = null;
                    Guid imageGuid = img.RawFormat.Guid;

                    if (guidToImageFormatMap.ContainsKey(imageGuid))
                    {
                        imageFormat = guidToImageFormatMap[imageGuid];
                    }
                    else
                    {
                        throw new NoNullAllowedException("Unable to determine image format");
                    }

                    //Get the frame count
                    FrameDimension dimension = new FrameDimension(img.FrameDimensionsList[0]);
                    int frameCount = img.GetFrameCount(dimension);
                    img.FrameDimensionsList[0].ToByteArray();
                    //Step through each frame
                    for (int i = 0; i < frameCount; i++)
                    {
                        //Set the active frame of the image and then 
                        //write the bytes to the tmpFrames array
                        img.SelectActiveFrame(dimension, i);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            img.Save(ms, imageFormat);
                            tmpFrames.Add(ms.ToArray());
                        }
                    }
                }
                return tmpFrames;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error type: " + ex.GetType().ToString() + "\n" +
                    "Message: " + ex.Message,
                    "Error in " + MethodBase.GetCurrentMethod().Name
                    );
            }

            return null;
        }

        private Bitmap ConvertBytesToImage(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                return null;
            }

            try
            {
                //Read bytes into a MemoryStream
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    //Recreate the frame from the MemoryStream
                    using (Bitmap bmp = new Bitmap(ms))
                    {
                        return (Bitmap)bmp.Clone();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error type: " + ex.GetType().ToString() + "\n" +
                    "Message: " + ex.Message,
                    "Error in " + MethodBase.GetCurrentMethod().Name
                    );
            }

            return null;
        }
    }
}