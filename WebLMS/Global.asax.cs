using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebLMS.Utils;
using System.IO;
using System.Reflection;

namespace WebLMS
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private void LoadLibraries()
        {
            string destDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FfmpegNativeLibraries");
            Win32NativeMethods.LoadLibrary(Path.Combine(destDirectory, "avutil-51.dll"));
            Win32NativeMethods.LoadLibrary(Path.Combine(destDirectory, "avcodec-53.dll"));
            Win32NativeMethods.LoadLibrary(Path.Combine(destDirectory, "avformat-53.dll"));

            Win32NativeMethods.LoadLibrary(Path.Combine(destDirectory, "swresample-0.dll"));
            Win32NativeMethods.LoadLibrary(Path.Combine(destDirectory, "swscale-2.dll"));
            Win32NativeMethods.LoadLibrary(Path.Combine(destDirectory, "avfilter-2.dll"));
            Win32NativeMethods.LoadLibrary(Path.Combine(destDirectory, "avdevice-53.dll"));
            Win32NativeMethods.LoadLibrary(Path.Combine(destDirectory, "postproc-52.dll"));
            
            

            //a = Win32NativeMethods.LoadLibrary(Path.Combine(destDirectory, "Aforge.Video.dll"));
            //a = Win32NativeMethods.LoadLibrary(Path.Combine(destDirectory, "Aforge.Video.FFMPEG.dll"));

            //Assembly assemblyVideo = Assembly.LoadFrom(Path.Combine(destDirectory, "AForge.Video.dll"));
            /*Assembly assemblyFfmpeg = Assembly.LoadFrom(Path.Combine(destDirectory, "AForge.Video.FFMPEG.dll"));

            Type vw = assemblyFfmpeg.GetType("AForge.Video.FFMPEG.VideoFileWriter");
            Type codec = assemblyFfmpeg.GetType("AForge.Video.FFMPEG.VideoCodec");
            var aa = codec.GetProperties();
            
           
            var cc = codec.GetEnumValues();
            foreach ( var c in cc) {
                var ff = c;
            }

            var ccc = codec.GetField("MPEG4");

            //var c = vw.GetConstructor(new Type[] { }).Invoke(new object[]{});
            object obj = Activator.CreateInstance(vw);
            var methods = vw.GetMethods();
            var bb = codec.GetField("MPEG4").GetValue(obj);
            
            PropertyInfo[] myFields = vw.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var g in myFields)
            {
                if (g.GetIndexParameters().Length == 0)
                {
                    string name = g.Name;
                    var s = g.GetValue(obj, null);
                }
            }*/
        }

        protected void Application_Start()
        {
            this.LoadLibraries();
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }
    }
}