using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.StaticFiles;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

namespace Lab5.Controllers
{   
    [Route("storage")]
    [ApiController]
    public class Controller : ControllerBase
    {
        [HttpGet("{*path}")]
        public ActionResult GetFile(string path)
        {
            string fullPath = Model.GetFullPath(path);
            if (Directory.Exists(fullPath))
            {
                return new JsonResult(Model.FindAll(fullPath));
            }
            else
            {
                if (System.IO.File.Exists(fullPath))
                {
                    FileInfo info = new FileInfo(fullPath);
                    string fileType = "application/octet-stream";
                    return PhysicalFile(fullPath, fileType, info.Name);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpDelete("{*path}")]
        public ActionResult DeleteFile(string path)
        {
            string fullPath;

            if (path == null)
            {
                return BadRequest();
            }
            else
            {
                fullPath = Model.GetFullPath(path);
            }
            if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath);
                return Ok();
            }
            else
            {
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpHead("{*path}")]
        public ActionResult GetFileHeader(string path)
        {
            string fullPath = Model.GetFullPath(path);
            if (Directory.Exists(fullPath))
            {
                var info = Model.GetDirInfo(fullPath);
                foreach (var el in info)
                {
                    Response.Headers.Add(el.Key, el.Value);
                }
                return Ok();
            }
            else
            {
                if (System.IO.File.Exists(fullPath))
                {
                    var info = Model.GetFileInfo(fullPath);
                    foreach (var el in info)
                    {
                        Response.Headers.Add(el.Key, el.Value);
                    }
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
        }
        
        [HttpPut("{*path}")]
        public ActionResult PutFile(IFormFile file, string path)
        {
            if (file != null)
            {
                string fullPath = Model.GetFullPath(path);
                if (Directory.Exists(fullPath))
                {
                    try
                    {
                        var fileStream = new FileStream(fullPath + "/" + file.FileName, FileMode.Create);
                        file.CopyTo(fileStream);
                        fileStream.Close();
                        return Ok();
                    }
                    catch (Exception e)
                    {
                        return BadRequest();
                    }
                }
            }
            else
            {
                return BadRequest();
            }
            return BadRequest();
        }
    }
}
