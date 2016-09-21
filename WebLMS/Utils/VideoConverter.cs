using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Video;
using System.Drawing;
using System.IO;
using NReco.VideoConverter;
using System.Reflection;

namespace VClass
{
    public class VideoConverter
    {
        Type VideoCodecType;
        Type VideFileWriterType;
        object VideoFileWriter;

        int frameRate;
        string sourceDirectoryPath { get; set; }
        string destDirectoryPath { get; set; }
        string dataDirectoryPath { get; set; }
        string recordXmlPath { get; set; }
        XmlModel model;

        public VideoConverter(string sourceDirectoryPath, string destDirectoryPath, int frameRate = 10)
        {
            this.sourceDirectoryPath = sourceDirectoryPath;
            this.destDirectoryPath = destDirectoryPath;
            this.dataDirectoryPath = Path.Combine(this.sourceDirectoryPath, "data");
            this.recordXmlPath = Path.Combine(this.dataDirectoryPath, "record.xml");
            this.frameRate = frameRate;
        }

        public string Start()
        {
            this.CheckArchiveCorrect();
            this.LoadXml();
            this.CreateMovie();
            return this.AddAudio();
        }

        public Bitmap ReduceBitmap(Bitmap original, Bitmap reduced, int x, int y, bool isVisible, string imageName)
        {
            if (!isVisible)
            {
                return reduced;
            }
            using (var dc = Graphics.FromImage(reduced))
            {
                dc.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                dc.DrawImage(original, x, y, original.Width, original.Height);
                //reduced.Save(@"D:\images\" + i++ + "_" + imageName);
                //dc.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), new Rectangle(0, 0, original.Width, original.Height), GraphicsUnit.Pixel);
            }
            return reduced;
        }

        private void CheckArchiveCorrect()
        {
            if (!Directory.Exists(this.sourceDirectoryPath))
            {
                throw new DirectoryNotFoundException("Не найдена корневая папка!");
            }
            if (!Directory.Exists(this.dataDirectoryPath))
            {
                throw new DirectoryNotFoundException("Не найдена папка \"data\" с изображениями!");
            }
            if (!File.Exists(this.recordXmlPath))
            {
                throw new FileNotFoundException("Не найден файл record.xml!");
            }
            if (!Directory.Exists(this.destDirectoryPath))
            {
                throw new DirectoryNotFoundException("Не найдена папка записи!");
            }
        }

        private void LoadXml()
        {
            this.model = XmlSettings.LoadXml(this.recordXmlPath);
        }

        private string[] GetFiles(string filePath)
        {
            return Directory.GetFiles(filePath, "*.png");
        }

        private string GetNewFilename()
        {
            return Path.Combine(this.destDirectoryPath, this.model.Name + ".avi");
        }

        private void SetVideoFileWriter()
        {
            string destDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FfmpegNativeLibraries");
            Assembly assemblyFfmpeg = Assembly.LoadFrom(Path.Combine(destDirectory, "AForge.Video.FFMPEG.dll"));

            Type vw = assemblyFfmpeg.GetType("AForge.Video.FFMPEG.VideoFileWriter");
            Type codec = assemblyFfmpeg.GetType("AForge.Video.FFMPEG.VideoCodec");
            //var c = vw.GetConstructor(new Type[] { }).Invoke(new object[]{});
            
            this.VideoFileWriter = Activator.CreateInstance(vw);
            this.VideFileWriterType = vw;
            this.VideoCodecType = codec;
        }

        private void InvokeMethod(string methodName, object[] param = null)
        {
            if (methodName == "" || methodName == null)
            {
                return;
            }
            if (param == null)
            {
                this.VideFileWriterType.GetMethod(methodName).Invoke(this.VideoFileWriter, null);
                return;
            }

            Type[] types = new Type[param.Length];
            for (int i = 0; i < param.Length; i++)
            {
                types[i] = param[i].GetType();
            }
            MethodInfo method = this.VideFileWriterType.GetMethod(methodName, types);
            method.Invoke(this.VideoFileWriter, param);
        }

        private object GetVideoCodec(string codecName)
        {
            return this.VideoCodecType.GetField(codecName).GetValue(this.VideoFileWriter);
        }

        private void CreateMovie()
        {
            int width = this.model.Width % 2 == 0 ? this.model.Width : this.model.Width + 1;
            int height = this.model.Height % 2 == 0 ? this.model.Height : this.model.Height + 1;

            // create instance of video writer
            this.SetVideoFileWriter();

            // create new video file
            string destNewFilePath = this.GetNewFilename();

            this.InvokeMethod("Open", new object[] { destNewFilePath, width, height, this.frameRate, this.GetVideoCodec("MPEG4") });

            //vFWriter.Open(destNewFilePath, width, height, this.frameRate, VideoCodec.MPEG4);
            var imagePaths = this.GetFiles(this.dataDirectoryPath);
            List<Frame> frames = this.model.Frames.Where(f => f.Type == "image").ToList<Frame>();
            Bitmap reduced = new Bitmap(width, height);

            foreach (Frame fr in frames)
            {
                string imagePath = imagePaths.Where(img => Path.GetFileName(img) == fr.Name).FirstOrDefault<string>();
                if (imagePath != null)
                {
                    string imageName = Path.GetFileName(imagePath);
                    Bitmap originalBitmap = new Bitmap(imagePath);
                    Bitmap bmpReduced = this.ReduceBitmap(originalBitmap, reduced, (int)fr.X, (int)fr.Y, fr.Visible, imageName);

                    this.InvokeMethod("WriteVideoFrame", new object[] { bmpReduced, TimeSpan.FromMilliseconds(fr.Time) });
                }
            }
            this.InvokeMethod("Close");
        }

        private string AddAudio()
        {
            string flvFilePath = Directory.GetFiles(this.dataDirectoryPath, "*.flv").FirstOrDefault<string>();
            string outFilePath = null;
            if (flvFilePath != null)
            {
                //string spxAudioPath = Program.ExtractSpx(@"D:\test\audio.flv");
                string newWavPath = Path.Combine(this.destDirectoryPath, Path.GetFileNameWithoutExtension(flvFilePath) + ".wav");
                FFMpegConverter converter = new FFMpegConverter();
                converter.ConvertMedia(flvFilePath, newWavPath, "wav");


                string filePathWithoutAudio = this.GetNewFilename();
                string oldFilePath = Path.Combine(this.destDirectoryPath, Path.GetFileNameWithoutExtension(filePathWithoutAudio) + "_old" + ".avi");
                File.Move(filePathWithoutAudio, oldFilePath);
                converter.Invoke(String.Format("-i {0} -i {1} -codec copy -shortest {2}", "\"" + oldFilePath + "\"", newWavPath, "\"" + filePathWithoutAudio +"\""));
                File.Delete(newWavPath);
                File.Delete(oldFilePath);
                outFilePath = filePathWithoutAudio;
            }
            return outFilePath;
        }
    }
}
