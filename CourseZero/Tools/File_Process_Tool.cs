using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseZero.Tools
{

    public class File_Process_Tool
    {
        static string[] AllowedFiles_Types = { ".txt", ".doc", ".docx", ".ppt", ".pptx", ".pdf", ".wav", ".mp3", ".3gp", ".mp4", ".avi", ".mkv" };
        public enum FileProcess_Status
        {
            Success,
            NotFound,
            TimeOut,
            FileOpenError,
            ThumbnailError,
            ReadError,
            OtherError,
            UnknownFileType
        }
        static Microsoft.Office.Interop.PowerPoint.Application powerPoint_App = null;
        static Microsoft.Office.Interop.PowerPoint.Presentation powerPoint_Presentation = null;
        static Microsoft.Office.Interop.Word.Document word_Document = null;
        public static void Try_Close_AllHandles()
        {
            if (powerPoint_App != null)
            {
                try
                {
                    powerPoint_App.Quit();
                }
                catch
                {

                }
                powerPoint_App = null;
            }
            if (powerPoint_Presentation != null)
            {
                try
                {
                    powerPoint_Presentation.Close();
                }
                catch
                {

                }
                powerPoint_Presentation = null;
            }
            if (word_Document != null)
            {
                try
                {
                    word_Document.Close(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges, Type.Missing, Type.Missing);
                }
                catch
                {

                }
                word_Document = null;
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        public static (FileProcess_Status status, Bitmap thumbnail, byte[] content) Process(string filename, string filetype)
        {

            if (filetype == ".docx" || filetype == ".doc")
                return Process_DOC(filename);
            else if (filetype == ".pptx" || filetype == ".ppt")
                return Process_PPT(filename);
            else if (filetype == ".txt")
                return Process_TXT(filename);

            return (FileProcess_Status.UnknownFileType, null, null);
        }
        public static (FileProcess_Status status, Bitmap thumbnail, byte[] content) Process_TXT(string filename)
        {
            // Using word to create thumbnail
            try
            {
                word_Document = new Microsoft.Office.Interop.Word.Application().Documents.Open(FileName: filename, ReadOnly: true);
                word_Document.ShowGrammaticalErrors = false;
                word_Document.ShowRevisions = false;
                word_Document.ShowSpellingErrors = false;
            }
            catch
            {
                return (FileProcess_Status.FileOpenError, null, null);
            }
            byte[] bytes = null;
            try
            {
                bytes = (byte[])word_Document.Range().EnhMetaFileBits;
                word_Document.Application.Quit(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges, Type.Missing, Type.Missing);
            }
            catch
            {
                return (FileProcess_Status.ThumbnailError, null, null);
            }
            Bitmap bitmap;
            try
            {
                bitmap = ReplaceTransparency(new Bitmap(Image.FromStream(new MemoryStream(bytes))));
            }
            catch
            {
                return (FileProcess_Status.OtherError, null, null);
            }
            return (FileProcess_Status.Success, bitmap, File.ReadAllBytes(filename));
        }
        public static (FileProcess_Status status, Bitmap thumbnail, byte[] content) Process_PPT(string filename)
        {
            powerPoint_App = new Microsoft.Office.Interop.PowerPoint.Application();
            try
            {
                powerPoint_Presentation = powerPoint_App.Presentations.Open(filename, ReadOnly: Microsoft.Office.Core.MsoTriState.msoCTrue, WithWindow: Microsoft.Office.Core.MsoTriState.msoFalse);
            }
            catch
            {
                return (FileProcess_Status.FileOpenError, null, null);
            }
            string thumbnail_temp = AppDomain.CurrentDomain.BaseDirectory + "/temp/" + Guid.NewGuid().ToString() + ".png";
            try
            {
                powerPoint_Presentation.Slides[1].Export(thumbnail_temp, "png");
                powerPoint_Presentation.Close();
                powerPoint_App.Quit();
            }
            catch
            {
                return (FileProcess_Status.ThumbnailError, null, null);
            }

            Image image;
            try
            {
                using (FileStream fileStream = new FileStream(thumbnail_temp, FileMode.Open, FileAccess.Read))
                {
                    image = Image.FromStream(fileStream);
                }
                File.Delete(thumbnail_temp);
            }
            catch
            {
                return (FileProcess_Status.OtherError, null, null);
            }
            return (FileProcess_Status.Success, new Bitmap(image), File.ReadAllBytes(filename));
        }
        public static (FileProcess_Status status, Bitmap thumbnail, byte[] content) Process_DOC(string filename)
        {
            try
            {
                word_Document = new Microsoft.Office.Interop.Word.Application().Documents.Open(FileName: filename, ReadOnly: true);
                word_Document.ShowGrammaticalErrors = false;
                word_Document.ShowRevisions = false;
                word_Document.ShowSpellingErrors = false;
            }
            catch
            {
                return (FileProcess_Status.FileOpenError, null, null);
            }
            byte[] bytes = null;
            try
            {
                bytes = (byte[])word_Document.Range().EnhMetaFileBits;
                word_Document.Application.Quit(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges, Type.Missing, Type.Missing);
            }
            catch
            {
                return (FileProcess_Status.ThumbnailError, null, null);
            }
           
            Bitmap bitmap;
            try
            {
                
                bitmap = ReplaceTransparency(new Bitmap(Image.FromStream(new MemoryStream(bytes))));
            }
            catch
            {
                return (FileProcess_Status.OtherError, null, null);
            }
             return (FileProcess_Status.Success, bitmap, File.ReadAllBytes(filename));
        }
        private static Bitmap ReplaceTransparency(Bitmap bitmap)
        {
            var result = new Bitmap(bitmap.Size.Width, bitmap.Size.Height, PixelFormat.Format24bppRgb);
            var g = Graphics.FromImage(result);
            g.Clear(Color.White);
            g.CompositingMode = CompositingMode.SourceOver;
            g.DrawImage(bitmap, 0, 0);
            return result;
        }
        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        public static bool File_Allowed(string typename)
        {
            foreach (var s in AllowedFiles_Types)
                if (s == typename)
                    return true;
            return false;
        }
    }
}
