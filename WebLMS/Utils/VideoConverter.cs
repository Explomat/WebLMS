using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Video;
using System.Drawing;
using System.IO;
using NReco.VideoConverter;
using System.Reflection;
using System.Diagnostics;

namespace VClass
{
    public class VideoConverter : IDisposable
    {
        class DoubleFrames
        {
            public string Key { get; private set; }
            public Frame Play { get; private set; }
            public Frame Stop { get; private set; }

            public DoubleFrames(string key, Frame play, Frame stop)
            {
                this.Key = key;
                this.Play = play;
                this.Stop = stop;
            }
        }

        Type VideoCodecType;
        Type VideFileWriterType;
        object VideoFileWriter;

        int frameRate;
        int sampleRate;
        string sourceDirectoryPath { get; set; }
        string destDirectoryPath { get; set; }
        string dataDirectoryPath { get; set; }
        string recordXmlPath { get; set; }
        string ext { get; set; }
        XmlModel model;
        FFMpegConverter converter = null;

        public VideoConverter(string sourceDirectoryPath, string destDirectoryPath, int frameRate = 25, int sampleRate = 22050)
        {
            this.sourceDirectoryPath = sourceDirectoryPath;
            this.destDirectoryPath = destDirectoryPath;
            this.dataDirectoryPath = Path.Combine(this.sourceDirectoryPath, "data");
            this.recordXmlPath = Path.Combine(this.dataDirectoryPath, "record.xml");
            this.frameRate = frameRate;
            this.sampleRate = sampleRate;
            this.ext = ".mp4";
        }

        public string Start()
        {
            this.converter = new FFMpegConverter();
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

        public string CheckArchiveCorrect()
        {
            if (!Directory.Exists(this.sourceDirectoryPath))
            {
                return "Не найдена корневая папка!";
            }
            if (!Directory.Exists(this.dataDirectoryPath))
            {
                return "Не найдена папка \"data\" с изображениями!";
            }
            if (!File.Exists(this.recordXmlPath))
            {
                return "Не найден файл record.xml!";
            }
            if (!Directory.Exists(this.destDirectoryPath))
            {
                return "Не найдена папка записи!";
            }
            return null;
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
            return Path.Combine(this.destDirectoryPath, this.model.Name + this.ext);
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
            //this.SetVideoFileWriter();

            // create new video file
            string destNewFilePath = this.GetNewFilename();

            //this.InvokeMethod("Open", new object[] { destNewFilePath, width, height, this.frameRate, this.GetVideoCodec("MPEG4") });

            //vFWriter.Open(destNewFilePath, width, height, this.frameRate, VideoCodec.MPEG4);
            var imagePaths = this.GetFiles(this.dataDirectoryPath);
            List<Frame> frames = this.model.Frames.Where(f => f.Type == "image").ToList<Frame>();
            Bitmap reduced = new Bitmap(width, height);

            string inputFileDurations = Path.Combine(this.destDirectoryPath, "input.txt");
            FileStream fs = File.Open(inputFileDurations, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            Frame previousFrame = null;
            Frame curFrame = null;
            foreach (Frame fr in frames)
            {
                string imagePath = imagePaths.Where(img => Path.GetFileName(img) == fr.Name).FirstOrDefault<string>();
                if (imagePath != null)
                {
                    string imageName = Path.GetFileName(imagePath);
                    string newFilePath = Path.Combine(this.destDirectoryPath, Path.GetFileName(imagePath));
                    Bitmap originalBitmap = new Bitmap(imagePath);
                    Bitmap bmpReduced = this.ReduceBitmap(originalBitmap, reduced, (int)fr.X, (int)fr.Y, fr.Visible, imageName);
                    bmpReduced.Save(newFilePath);

                    if (previousFrame != null)
                    {
                        TimeSpan diff = TimeSpan.FromMilliseconds(fr.Time) - TimeSpan.FromMilliseconds(previousFrame.Time);
                        sw.WriteLine("duration {0}", diff.TotalSeconds.ToString().Replace(',', '.'));
                    }
                    curFrame = previousFrame;
                    sw.WriteLine("file '{0}'", newFilePath);
                    previousFrame = fr;
                    //this.InvokeMethod("WriteVideoFrame", new object[] { bmpReduced, TimeSpan.FromMilliseconds(fr.Time) });
                }
            }
            if (curFrame != null && previousFrame != null)
            {
                TimeSpan diff = TimeSpan.FromMilliseconds(previousFrame.Time) - TimeSpan.FromMilliseconds(curFrame.Time);
                sw.WriteLine("duration {0}", diff.TotalSeconds.ToString().Replace(',', '.'));
            }
            reduced.Dispose();
            sw.Close();
            fs.Close();
            this.converter.Invoke(String.Format("-safe 0 -f concat -i \"{0}\" -c:v libx264 -preset ultrafast -crf 23 -pix_fmt yuv420p \"{1}\"", inputFileDurations, destNewFilePath));
            //this.converter.Invoke(String.Format("-safe 0 -f concat -i {0} -c:v libx264 -preset ultrafast -crf 220 -pix_fmt yuv420p {1}", inputFileDurations, destNewFilePath));
            
            //converter.ConvertProgress += (object obj, NReco.VideoConverter.ConvertProgressEventArgs obj2) => {  };
            //this.InvokeMethod("Close");
        }

        private List<DoubleFrames> GetDoubleFrames()
        {
            List<Frame> playAudioFrames = this.model.Frames.Where(fr => (fr.Type == "action" & fr.Stype == "audio" & fr.Action == "play")).ToList<Frame>();
            List<Frame> stopAudioFrames = this.model.Frames.Where(fr => (fr.Type == "action" & fr.Stype == "audio" & fr.Action == "stop")).ToList<Frame>();

            List<DoubleFrames> dFrames = new List<DoubleFrames>();
            foreach (Frame pFrame in playAudioFrames)
            {
                Frame stopFrame = stopAudioFrames.Where(fr => fr.Name == pFrame.Name).FirstOrDefault<Frame>();
                if (stopFrame != null)
                {
                    dFrames.Add(new DoubleFrames(stopFrame.Name, pFrame, stopFrame));
                    stopAudioFrames.Remove(stopFrame);
                }
            }
            return dFrames;
        }

        private int GetAudioDuration(List<DoubleFrames> frames)
        {
            int duration = 0;
            foreach (DoubleFrames df in frames)
            {
                duration += df.Stop.Time - df.Play.Time;
            }
            return duration;
        }

        private List<string> GetFFMPEGCommandsForInvokeAudio(string destDirectory, string ext)
        {
            List<string> commands = new List<string>();
            List<DoubleFrames> frames = this.GetDoubleFrames();
            var groupFrames = frames.GroupBy(df => df.Key);
            int fullTime = model.Frames.Last().Time;
            foreach (IGrouping<string, DoubleFrames> fr in groupFrames)
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder sbAevals = new StringBuilder();
                DoubleFrames firstPlayFrame = fr.FirstOrDefault<DoubleFrames>();
                DoubleFrames lastStopFrame = fr.LastOrDefault<DoubleFrames>();

                if (firstPlayFrame != null & lastStopFrame != null)
                {
                    List<DoubleFrames> sortedFrames = fr.OrderBy(t => t.Play.Time).ToList<DoubleFrames>();
                    int duration = this.GetAudioDuration(sortedFrames);
                    double leftAevalsrc = firstPlayFrame.Play.Time / 1000.0;
                    double lastAevalsrc = (fullTime - lastStopFrame.Stop.Time) / 1000.0;

                    List<string> concats = new List<string>();
                    sbAevals.Append("aevalsrc=0:s=" + this.sampleRate +":d=" + leftAevalsrc.ToString().Replace(',', '.') + "[aevalsrc0]; ");
                    concats.Add("[aevalsrc0]");

                    int countFrames = sortedFrames.Count();

                    double prevTime = 0;
                    int i = 0;
                    for (; i < countFrames - 1; i++)
                    {
                        DoubleFrames firstFrame = sortedFrames[i];
                        DoubleFrames secondFrame = sortedFrames[i + 1];
                        double durationFrame = firstFrame.Stop.Time - firstFrame.Play.Time;
                        double middleAevalsrc = (secondFrame.Play.Time - firstFrame.Stop.Time) / 1000.0;
                        sb.Append(" -ss " + TimeSpan.FromMilliseconds(prevTime) + " -t " + TimeSpan.FromMilliseconds(durationFrame) + " -i " + Path.Combine(destDirectory, fr.Key + "_old." + ext));
                        sbAevals.Append("aevalsrc=0:s=" + this.sampleRate + ":d=" + middleAevalsrc.ToString().Replace(',', '.') + "[aevalsrc" + (i + 1) + "]; ");
                        concats.AddRange(new string[] { "[" + i + "]", "[aevalsrc" + (i + 1) + "]" });
                        prevTime = durationFrame;
                    }
                    sb.Append(" -ss " + TimeSpan.FromMilliseconds(prevTime) + " -t " + TimeSpan.FromMilliseconds(duration - prevTime) + " -i " + Path.Combine(destDirectory, fr.Key + "_old." + ext));
                    concats.AddRange(new string[] { "[" + i + "]", "[aevalsrc" + (i + 1) + "]" });
                    sbAevals.Append("aevalsrc=0:s=" + this.sampleRate + ":d=" + lastAevalsrc.ToString().Replace(',', '.') + "[aevalsrc" + (i + 1) + "]; ");
                    commands.Add(sb.ToString() + " -filter_complex \"" + sbAevals.ToString() + " " + String.Join("", concats) + " concat=n=" + concats.Count + ":v=0:a=1[out]\" -map \"[out]\" -c:v copy " + Path.Combine(destDirectory, fr.Key + "." + ext));
                }
            }
            return commands;
        }

        private void ConvertFlvToAudio(string ext, FFMpegConverter converter)
        {
            string[] flvFilePaths = Directory.GetFiles(this.dataDirectoryPath, "*.flv");
            foreach (string flvPath in flvFilePaths)
            {
                string newWavPath = Path.Combine(this.destDirectoryPath, Path.GetFileNameWithoutExtension(flvPath) + "_old." + ext);
                converter.ConvertMedia(flvPath, newWavPath, ext);
            }
        }

        private string AddAudio()
        {
            string ext = "wav";
            this.ConvertFlvToAudio(ext, this.converter);

            List<string> commands = this.GetFFMPEGCommandsForInvokeAudio(this.destDirectoryPath, ext);
            
            foreach (string command in commands)
            {
                this.converter.Invoke(command);
            }

            foreach (string file in Directory.GetFiles(this.destDirectoryPath, "*_old." + ext))
            {
                File.Delete(file);
            }

            string[] finalAudioPaths = Directory.GetFiles(this.destDirectoryPath, "*." + ext);
            string outFilePath = null;
            if (finalAudioPaths.Length > 0)
            {
                string destAudioPath = finalAudioPaths.Length > 1 ? Path.Combine(this.destDirectoryPath, "audio." + ext) : Path.Combine(this.destDirectoryPath, finalAudioPaths[0]);
                if (finalAudioPaths.Length > 1)
                {
                    string strCommand = "-i " + String.Join(" -i ", finalAudioPaths) + " -filter_complex \"amix=inputs=" + finalAudioPaths.Length + ":duration=first\" " + destAudioPath;
                    this.converter.Invoke(strCommand);
                }

                string filePathWithoutAudio = this.GetNewFilename();
                string oldFilePath = Path.Combine(this.destDirectoryPath, Path.GetFileNameWithoutExtension(filePathWithoutAudio) + "_old" + this.ext);
                File.Move(filePathWithoutAudio, oldFilePath);
                this.converter.Invoke(String.Format("-i {0} -i {1} -c:v copy -c:a aac -strict experimental {2}", "\"" + oldFilePath + "\"", destAudioPath, "\"" + filePathWithoutAudio + "\""));
                //this.converter.Invoke(String.Format("-i {0} -i {1} -codec copy -shortest {2}", "\"" + oldFilePath + "\"", destAudioPath, "\"" + filePathWithoutAudio + "\""));
                File.Delete(destAudioPath);
                File.Delete(oldFilePath);
                outFilePath = filePathWithoutAudio;
            }
            return outFilePath;
            /*string flvFilePath = Directory.GetFiles(this.dataDirectoryPath, "*.flv").FirstOrDefault<string>();
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
            return outFilePath;*/
        }

        public void Dispose()
        {
            this.converter = null;
        }
    }
}
