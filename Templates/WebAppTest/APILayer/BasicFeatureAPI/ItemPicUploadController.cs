using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.IO;
using System.Threading.Tasks;

namespace WebAppTest.APILayer.BasicFeatureAPI
{
    [Route("api/ItemPicUpload")]
    [ApiController]
    public class ItemPicUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly string _RemoteHost = "121.36.200.128";
        private readonly string _UserName = "root";
        private readonly string _RemoteBasePath = "/www/wwwroot/picUpload";

        public ItemPicUploadController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadPic([FromQuery] string type, IFormFile file)
        {
            if (file == null|| file.Length == 0)
            {
                return BadRequest("No file uploaded");
            } 

            if (string.IsNullOrEmpty(type) || (type != "Found" && type != "Lost"))
            {
                return BadRequest("Invalid type. Allowed values are 'Found' or 'Lost'.");
            }

            var FolderName = type == "Found" ? "FoundItemPics" : "LostItemPics";
            var RemoteFolderPath = $"{_RemoteBasePath}/{FolderName}";

            var FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var RemoteFilePath = $"{RemoteFolderPath}/{FileName}";

            var PrivateKeyPath = Path.Combine(_hostingEnvironment.ContentRootPath, "sshkey", "id_rsa");

            if (!System.IO.File.Exists(PrivateKeyPath))
            {
                return BadRequest("Private key file not found.");
            } 

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var FileBytes = memoryStream.ToArray();

                using (var sftp = new SftpClient(_RemoteHost, _UserName, new PrivateKeyFile(PrivateKeyPath)))
                {
                    try
                    {
                        sftp.Connect();
                        if (!sftp.Exists(RemoteFolderPath))
                        {
                            return BadRequest("Remote folder does not exist.");
                        }

                        using (var FileStream = new MemoryStream(FileBytes))
                        {
                            sftp.UploadFile(FileStream, RemoteFilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                    finally
                    {
                        sftp.Disconnect();
                    }
                }
            }

            var FileUrl = $"/DB_data/{FolderName}/{FileName}";

            return Ok(new { url = FileUrl });
        }

        [HttpPost("uploadLocal")]
        public async Task<IActionResult> UploadPicLocal([FromQuery] string type, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            } 

            if (string.IsNullOrEmpty(type) || (type != "Found" && type != "Lost"))
            {
                return BadRequest("Invalid type. Allowed values are 'Found' or 'Lost'.");
            }

            var FolderName = type == "Found" ? "FoundItemPics" : "LostItemPics";
            var LocalFolderPath = $"{_RemoteBasePath}/{FolderName}";

            var FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var LocalFilePath = $"{LocalFolderPath}/{FileName}";

            if (!Directory.Exists(LocalFolderPath))
            {
                return BadRequest("Remote folder does not exist.");
            }

            using (var fileStream = new FileStream(LocalFilePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var FileUrl = $"http://121.36.200.128:5600/{FolderName}/{FileName}";

            return Ok(new { url = FileUrl });
        }
    }
}
