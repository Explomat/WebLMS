using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebLMS.Models;
using System.Net.Mail;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.IO.Compression;
using VClass;
using System.Net.Mime;

/*
 public Int64 Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool? IsQuickly { get; set; }
        public string Description { get; set; }
 */

namespace WebLMS.Controllers
{
    public class HomeController : AsyncController
    {
        private WebLMSContext _db = new WebLMSContext();

        private void SendMessage(WebLMSForm form)
        {
            MailMessage m = new MailMessage();
            SmtpClient sc = new SmtpClient();
            m.From = new MailAddress("cabehok@inbox.ru");
            m.To.Add("gabdsh@gmail.com");
            m.Subject = String.Format("Заявка от {0}", form.Fullname);
            m.Body = String.Format("ФИО - {0} \r\n Email - {1} \r\n Телефон - {2} \r\n Срочный заказ  - {3} \r\n Описание - {4}", form.Fullname, form.Email, form.Phone, form.IsQuickly, form.Description);
            sc.Host = "smtp.mail.ru";
            try
            {
                sc.Port = 25;
                sc.Credentials = new System.Net.NetworkCredential("cabehok@inbox.ru", "1qaz2wsx3edc4RFV");
                sc.EnableSsl = false;
                sc.Send(m);
            }
            catch (Exception ex)
            {
               
            }
        }

        private string GetMD5Hash(MD5 md5, Stream stream)
        {
            byte[] data = md5.ComputeHash(stream);
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        private void Unzip(Stream inputStream, string destDirectory)
        {
            using (ZipStorer zip = ZipStorer.Open(inputStream, FileAccess.Read))
            {
                // Read the central directory collection
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();
                
                foreach (ZipStorer.ZipFileEntry entry in dir)
                {
                    zip.ExtractFile(entry, Path.Combine(destDirectory, entry.FilenameInZip));
                }
            }
        }

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
            //this.SendMessage(form);
            return RedirectToAction("Index");
        }

        public ActionResult RequestList()
        {
            return View(_db.WebLMSForms.ToList());
        }

        [HttpGet]
        public FileContentResult GetVideoFile(string fileName, string filePath)
        {
            string pathToFile = Server.MapPath(filePath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(pathToFile);
            try
            {
                System.IO.File.Delete(pathToFile);
            }catch(Exception ex){}
            return File(fileBytes, "video/avi", fileName);
        }

        [HttpPost]
        public ActionResult ConvertForm(HttpPostedFileBase fileUpload)
        {
            if (fileUpload == null || fileUpload.ContentLength == 0)
            {
                return Json(new { error = "Не выбран или поврежден файл!" } );
            }
            System.Net.WebRequest req = System.Net.WebRequest.Create("http://www.example.com");
            RegisterAsyncTask(cb => req.BeginGetResponse(cb, null), delegate(IAsyncResult result)
            {
                System.Net.WebResponse response = req.EndGetResponse(result);
                // Do something with the response here if you want
                //RenderView("Index");
            });
            using (MD5 md5 = MD5.Create())
            {
                try
                {
                    string hash = this.GetMD5Hash(md5, fileUpload.InputStream);
                    string destDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempVideoFiles");
                    string destFullDirectory = Path.Combine(destDirectory, hash);
                    string videoDirectory = Path.Combine(Path.Combine(destFullDirectory, "video"));
                    if (!Directory.Exists(destDirectory))
                    {
                        Directory.CreateDirectory(destDirectory);
                    }
                    Directory.CreateDirectory(destFullDirectory);
                    Directory.CreateDirectory(videoDirectory);
                    this.Unzip(fileUpload.InputStream, destFullDirectory);
                    VideoConverter converter = new VideoConverter(destFullDirectory, videoDirectory);

                    string outFilePath = converter.Start();
                    return Json(
                        new
                        {
                            filePath = "/Home/GetVideoFile/?fileName=" + Path.GetFileName(outFilePath) + "&filePath=/TempVideoFiles/" + hash + "/video/" + Path.GetFileName(outFilePath)
                        }
                    );
                }
                catch (Exception e)
                {
                    return Json(new { error = e.Message });
                }
                

                /*var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = HttpUtility.UrlPathEncode(Path.GetFileName(outFilePath)),
                    Inline = true
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());
                //MemoryStream ms = new MemoryStream(System.IO.File.ReadAllBytes(outFilePath));
                //Directory.Delete(destFullDirectory, true);
                return File(outFilePath, "video/avi");*/
            }
        }
    }
}
