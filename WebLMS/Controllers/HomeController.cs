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
using System.Threading.Tasks;
using WebLMS.ConvertService;
//using Hangfire;

namespace WebLMS.Controllers
{
    public class HomeController : Controller
    {
        private WebLMSContext _db = new WebLMSContext();

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
        public void GetVideoFile(string hash, Int64 id = -1)
        {
            Stream fileStream = null;
            ConverterClient client = null;
            try
            {
                if (hash == null || id == -1)
                {
                    throw new FileNotFoundException("Не найден файл!");
                }

                Models.File file = _db.Files.Where(f => f.Id == id && f.Md5Hash == hash).FirstOrDefault<Models.File>();
                if (file == null)
                {
                    throw new FileNotFoundException("Не найден файл!");
                }
            
                client = new ConverterClient("BasicHttpBinding_IConverter");
                Int64 fileLength = client.DownloadFile(file.FilePath, out fileStream);

                if (fileStream == null)
                {
                    throw new FileNotFoundException("Не найден файл!");
                }

                Response.BufferOutput = true;   // to prevent buffering 
                Response.Buffer = true;
                byte[] buffer = new byte[6500];
                int bytesRead = 0;
                
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.ClearHeaders();
                Response.BufferOutput = false;   // to prevent buffering
                Response.ContentType = "video/mp4";
                Response.AddHeader("Content-Length", fileLength.ToString());
                Response.AddHeader("content-disposition", "attachment;filename=\"" + Path.GetFileName(file.FilePath) + "\"");

                bytesRead = fileStream.Read(buffer, 0, buffer.Length);

                while (bytesRead > 0)
                {
                    // Verify that the client is connected.
                    if (Response.IsClientConnected)
                    {
                        Response.OutputStream.Write(buffer, 0, bytesRead);
                        // Flush the data to the HTML output.
                        Response.Flush();

                        buffer = new byte[6500];
                        bytesRead = fileStream.Read(buffer, 0, buffer.Length);

                    }
                    else
                    {
                        bytesRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                // Trap the error, if any.
                System.Web.HttpContext.Current.Response.Write("Error : " + ex.Message);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
                if (client != null)
                {
                    client.Close();
                }
                Response.Flush();
                Response.Close();
                //Response.End();
                //System.Web.HttpContext.Current.Response.Close();
            }
        }

        //[HttpGet]
        //public void GetVideoFile(string hash, Int64 id)
        //{
        //    if (hash == null)
        //    {
        //        Response.Write("Указаны неверные параметры!");
        //        return;
        //    }

        //    Models.File file = _db.Files.Where(f => f.Id == id && f.Md5Hash == hash).FirstOrDefault<Models.File>();
        //    if (file == null)
        //    {
        //        Response.Write("Указаны неверные параметры!");
        //        return;
        //    }

        //    TransferClient client = new TransferClient("BasicHttpBinding_ITransfer");
        //    RemoteFileInfo fileInfo = client.DownloadFile(new DownloadRequest() { Path = file.FilePath });
        //    //client.DownloadFile()

            
        //    string filePath = Server.MapPath(file.FilePath);
        //    string ext = Path.GetExtension(filePath);
        //    string fileName = Path.GetFileNameWithoutExtension(filePath);
        //    if (!System.IO.File.Exists(filePath))
        //    {
        //        Response.Write("Файл не найден, т.к. уже удален с сервера!");
        //        return;
        //    }
        //    try
        //    {
        //        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        //        {
        //            System.Web.HttpContext.Current.Response.Clear();
        //            System.Web.HttpContext.Current.Response.ClearHeaders();
        //            Response.BufferOutput = false;   // to prevent buffering
        //            Response.ContentType = "video/mp4";
        //            Response.AddHeader("Content-Length", fs.Length.ToString());
        //            Response.AddHeader("content-disposition", "attachment;filename=\"" + fileName + ext + "\"");
                    
        //            byte[] buffer = new byte[6500];
        //            int bytesRead = 0;
        //            while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
        //            {
        //                if (Response.IsClientConnected)
        //                {
        //                    Response.OutputStream.Write(buffer, 0, bytesRead);
        //                }
        //                else
        //                {
        //                    bytesRead = -1;
        //                }
        //            }
        //        }
        //        file.IsDownloaded = true;
        //        _db.SaveChanges();
        //        System.IO.File.Delete(filePath);
        //    }

        //    catch (Exception ex) {
        //        System.Web.HttpContext.Current.Response.Write("Error : " + ex.Message);
        //        Debug.WriteLine(ex); 
        //    }
        //    finally
        //    {
        //        Response.Flush();
        //        Response.Close();
        //        Response.End();
        //        System.Web.HttpContext.Current.Response.Close();
        //    }
        //}

        [HttpPost]
        public ActionResult ConvertForm(HttpPostedFileBase fileUpload, string email = "")
        {
            if (fileUpload == null || fileUpload.ContentLength == 0)
            {
                return Json(new { error = "Не выбран или поврежден файл!" });
            }
            if (!Email.IsValidEmail(email))
            {
                return Json(new { error = "Неправильный email адрес!" });
            }

            string rootHost = Request.Url.Scheme + "://" + Request.Url.Authority;
            
            byte[] streamBytes = StreamUtils.ReadToEnd(fileUpload.InputStream);
            string result = String.Empty;
            ConverterClient client = new ConverterClient("BasicHttpBinding_IConverter");

            Task newTask = client.ConvertFileAsync(new UploadFileInfo() { ByteArray = streamBytes, Email = email }).ContinueWith(t =>
            {
                ResponseFileInfo fileInfo = t.Result;
                Models.File file = new Models.File();
                file.Md5Hash = fileInfo.Hash;
                file.FilePath = fileInfo.Path;
                file.EmailWhoConverted = email;
                file.Datetime = DateTime.Now;
                _db.Files.Add(file);
                _db.SaveChanges();

                string emailMessage = String.Format("Ссылка на скачивание видеофайла: {0}/Home/GetVideoFile/?hash={1}&id={2}\r\nС уважением, компания WebLMS.\r\nhttp://weblms.ru", rootHost, fileInfo.Hash, file.Id);
                ISender emailSender = new FileSender();
                emailSender.SendFileLink(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempVideoFiles", fileUpload.FileName + "_done.txt"), emailMessage);

                //ISender emailSender = new EmailSender();
                //emailSender.SendFileLink(email, emailMessage);
            });

            //Task newTask = Task.Factory.StartNew(() =>
            //{
            //    string result = String.Empty;
            //    try
            //    {
            //        ConverterClient client = new ConverterClient("BasicHttpBinding_IConverter");
            //        ResponseFileInfo fileInfo = client.ConvertFile(new UploadFileInfo() { ByteArray = streamBytes, Email = email});
            //        client.Close();

            //        Models.File file = new Models.File();
            //        file.Md5Hash = fileInfo.Hash;
            //        file.FilePath = fileInfo.Path;
            //        file.EmailWhoConverted = email;
            //        file.Datetime = DateTime.Now;
            //        _db.Files.Add(file);
            //        _db.SaveChanges();

            //        string emailMessage = String.Format("Ссылка на скачивание видеофайла: {0}/Home/GetVideoFile/?hash={1}&id={2}\r\nС уважением, компания WebLMS.\r\nhttp://weblms.ru", rootHost, fileInfo.Hash, file.Id);
            //        ISender emailSender = new FileSender();
            //        emailSender.SendFileLink(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempVideoFiles", fileUpload.FileName + "_done.txt"), emailMessage);

            //        //ISender emailSender = new EmailSender();
            //        //emailSender.SendFileLink(email, emailMessage);
            //    }
            //    catch (Exception e)
            //    {
            //        string emailError = String.Format("Произошла ошибка при конвертации файла, попробуйте повторить операцию позже. Детали ошибки: \r\n{0}.\r\nС уважением, компания WebLMS.\r\nhttp://weblms.ru", "Message: " + e.Message + "\r\nStack: " + e.StackTrace);
            //        //ISender emailSender = new EmailSender();
            //        //emailSender.SendFileLink(email, emailError);

            //        ISender emailSender = new FileSender();
            //        emailSender.SendFileLink(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempVideoFiles", fileUpload.FileName + "_catch.txt"), emailError);
            //    }
                
            //});
            //bool isAdded = WebLMSTasks.AddTask(newTask);
            //if (!isAdded)
            //{
            //    newTask.Dispose();
            //}

            //string messageToClient =
            //    WebLMSTasks.IsInPerformQueue(newTask.Id) ?
            //    "Файл проходит обработку. Ссылку на скачивание Вы получите по почте!" :
            //    WebLMSTasks.IsInLazyQueue(newTask.Id) ?
            //    "Файл поставлен в очередь на обработку. Ссылку на скачивание Вы получите по почте!" :
            //    "Сервер сейчас перегружен. Попробуйте повторить операцию позже.";


            return Json(
                new
                {
                    status = "process",
                    message = "Файл проходит обработку. Ссылку на скачивание Вы получите по почте!"
                }
            );
        }

        public void createFile(string destFullDirectory, string videoDirectory, string hash, string email, string rootHost)
        {
            VideoConverter converter = new VideoConverter(destFullDirectory, videoDirectory);
            Exception ex = null;
            Models.File file = null;
            try
            {
                string outFilePath = converter.Start();
                converter.Dispose();
                file = new Models.File();
                file.Md5Hash = hash;
                file.FilePath = "/TempVideoFiles/" + email + "/" + hash + "/video/" + Path.GetFileName(outFilePath); // Path.Combine(videoDirectory, Path.GetFileName(outFilePath));
                file.EmailWhoConverted = email;
                file.Datetime = DateTime.Now;
                _db.Files.Add(file);
                _db.SaveChanges();
                //System.IO.File.WriteAllText(Path.Combine(destFullDirectory, "saved.txt"), "Saved!");
            }
            catch (Exception e)
            {
                //System.IO.File.WriteAllText(Path.Combine(destFullDirectory, "catch.txt"), "Catch!");
                ex = e;
            }
            finally
            {
                //System.IO.File.WriteAllText(Path.Combine(destFullDirectory, "finally.txt"), "Finally!");

                //string messageFor = "Ссылка на скачивание видеофайла: " + Request.Url.Authority + "/Home/GetVideoFile/?hash=" + hash + "&id=" + file.Id + " \r\nПосле разового скачивания файл удалится с сервера.\r\nС уважением, компания WebLMS.\r\nhttp://weblms.ru";
                //string messageFor = "Ссылка на скачивание видеофайла: /Home/GetVideoFile/?hash=" + hash + "&id=" + file.Id + " После разового скачивания файл удалится с сервера.С уважением, компания WebLMS";
                string messageFor = ex == null && file != null ?
                String.Format("Ссылка на скачивание видеофайла: {0}/Home/GetVideoFile/?hash={1}&id={2} \r\nПосле разового скачивания файл удалится с сервера.\r\nС уважением, компания WebLMS.\r\nhttp://weblms.ru", rootHost, hash, file.Id) :
                String.Format("Произошла ошибка при конвертации файла, попробуйте повторить операцию позже. Детали ошибки: \r\n{0}.\r\nС уважением, компания WebLMS.\r\nhttp://weblms.ru", ex.Message);

                //System.IO.File.WriteAllText(Path.Combine(destFullDirectory, "finally1.txt"), "Finally1!");
                try
                {
                    //System.IO.File.WriteAllText(Path.Combine(destFullDirectory, "finallyTry.txt"), "finallyTry!");
                    //ISender fileSender1 = new FileSender();
                    //fileSender1.SendFileLink(Path.Combine(destFullDirectory, "input.txt1"), "Отправляется!");

                    //ISender emailSender = new FileSender();
                    //emailSender.SendFileLink(Path.Combine(destFullDirectory, "output.txt"), messageFor);

                    ISender emailSender = new EmailSender();
                    emailSender.SendFileLink(email, messageFor);

                    //System.IO.File.WriteAllText(Path.Combine(destFullDirectory, "email.txt"), messageFor);
                    //fileSender1.SendFileLink(Path.Combine(destFullDirectory, "input.txt2"), "Отправлено!");
                }
                catch (Exception e)
                {
                    //System.IO.File.WriteAllText(Path.Combine(destFullDirectory, "input3.txt"), e.Message);

                    //ISender fileSender = new FileSender();
                    //fileSender.SendFileLink(Path.Combine(destFullDirectory, "input3.txt"), e.Message);
                }
            }
        }
    }
}
