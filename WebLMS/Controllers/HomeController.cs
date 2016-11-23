using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.IO.Compression;
using VClass;
using System.Net.Mime;
using System.Threading;
using System.Diagnostics;
using WebLMS.Models;
using WebLMS.Utils;
using WebLMS.Utils.Sender;

namespace WebLMS.Controllers
{
    enum Statuses
    {
        process,
        error
    };

    public class HomeController : Controller
    {
        private WebLMSContext _db = new WebLMSContext();

        //private void SendMessage(WebLMSForm form)
        //{
        //    MailMessage m = new MailMessage();
        //    SmtpClient sc = new SmtpClient();
        //    m.From = new MailAddress("cabehok@inbox.ru");
        //    m.To.Add("gabdsh@gmail.com");
        //    m.Subject = String.Format("Заявка от {0}", form.Fullname);
        //    m.Body = String.Format("ФИО - {0} \r\n Email - {1} \r\n Телефон - {2} \r\n Срочный заказ  - {3} \r\n Описание - {4}", form.Fullname, form.Email, form.Phone, form.IsQuickly, form.Description);
        //    sc.Host = "smtp.mail.ru";
        //    try
        //    {
        //        sc.Port = 25;
        //        sc.Credentials = new System.Net.NetworkCredential("cabehok@inbox.ru", "1qaz2wsx3edc4RFV");
        //        sc.EnableSsl = false;
        //        sc.Send(m);
        //    }
        //    catch (Exception ex)
        //    {
               
        //    }
        //}

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateForm(WebLMSForm form)
        {
            form.RequestDate = DateTime.Now;
            _db.WebLMSForms.Add(form);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult RequestList()
        {
            return View(_db.WebLMSForms.ToList());
        }

        [HttpGet]
        public void GetVideoFile(string hash)
        {
            if (hash == null)
            {
                return;
            }

            Models.File file = _db.Files.Where(f => f.Md5Hash == hash).FirstOrDefault<Models.File>();
            if (file == null)
            {
                return;
            }
            string filePath = Server.MapPath(file.FilePath);
            string fileName = Path.GetFileName(filePath);
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    Response.BufferOutput = false;   // to prevent buffering
                    Response.ContentType = "video/avi";
                    Response.AddHeader("content-disposition", @"attachment;filename='"+fileName+"'");
                    
                    byte[] buffer = new byte[4096];
                    int bytesRead = 0;
                    while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        Response.OutputStream.Write(buffer, 0, bytesRead);
                    }
                }
                System.IO.File.Delete(filePath);
            }

            catch (Exception ex) { Debug.WriteLine(ex); }
            //return File(fileBytes, "video/avi", fileName);
        }

        [HttpPost]
        public ActionResult ConvertForm(HttpPostedFileBase fileUpload)
        {

            if (fileUpload == null || fileUpload.ContentLength == 0)
            {
                return Json(new { error = "Не выбран или поврежден файл!" });
            }
            using (MD5 md5 = MD5.Create())
            {
                try
                {
                    string hash = Hash.GetMD5Hash(md5, fileUpload.InputStream);
                    string destDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempVideoFiles");
                    string destFullDirectory = Path.Combine(destDirectory, hash);
                    string videoDirectory = Path.Combine(Path.Combine(destFullDirectory, "video"));
                    if (!Directory.Exists(destDirectory))
                    {
                        Directory.CreateDirectory(destDirectory);
                    }
                    Directory.CreateDirectory(destFullDirectory);
                    Directory.CreateDirectory(videoDirectory);
                    Zip.Unzip(fileUpload.InputStream, destFullDirectory);
                    VideoConverter converter = new VideoConverter(destFullDirectory, videoDirectory);

                    WebLMSThread.StartBackgroundThread(() =>
                    {
                        string outFilePath = converter.Start();
                        Models.File file = new Models.File();
                        file.Md5Hash = hash;
                        file.FilePath = "/TempVideoFiles/" + hash + "/video/" + Path.GetFileName(outFilePath);
                        _db.Files.Add(file);
                        _db.SaveChanges();
                        //RedirectToAction("GetVideoFile", new { hash = hash });
                        ISender sender = new FileSender();
                        sender.SendFileLink(Server.MapPath("/TempVideoFiles/" + hash + "/" +hash +".txt"), "/Home/GetVideoFile/?hash=" + hash);
                        //SendFileLink("cabehok@inbox.ru", "/Home/GetVideoFile/?fileName=" + Path.GetFileName(outFilePath) + "&filePath=/TempVideoFiles/" + hash + "/video/" + Path.GetFileName(outFilePath));
                    });
                    return Json(
                        new
                        {
                            status = Statuses.process
                        }
                    );
                }
                catch (Exception e)
                {
                    return Json(new { status = Statuses.error, error = e.Message });
                }
            }
        }
    }
}
