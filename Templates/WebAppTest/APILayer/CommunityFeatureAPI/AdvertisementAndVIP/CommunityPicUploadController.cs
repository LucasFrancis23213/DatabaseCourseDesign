using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebAppTest.APILayer.CommunityFeatureAPI
{
    [Route("api/AdPicUpload/")]
    [ApiController]
    public class CommunityPicUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly string _RemoteHost = "121.36.200.128";
        private readonly string _UserName = "root";
        private readonly string _RemoteBasePath = "/www/wwwroot/picUpload";

        public CommunityPicUploadController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadPic(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                // 返回带有自定义状态的错误信息
                return BadRequest(new { status = "error", message = "No file uploaded" });
            }


            var FolderName = "Advertisements";
            var LocalFolderPath = $"{_RemoteBasePath}/{FolderName}";

            var FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var LocalFilePath = $"{LocalFolderPath}/{FileName}";

            if (!Directory.Exists(LocalFolderPath))
            {
                // 返回带有自定义状态的错误信息
                return BadRequest(new { status = "error", message = "Remote folder does not exist." });
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
