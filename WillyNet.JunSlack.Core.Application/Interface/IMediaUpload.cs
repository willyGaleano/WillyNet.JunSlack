using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.DTOs;

namespace WillyNet.JunSlack.Core.Application.Interface
{
    public interface IMediaUpload
    {
        Task<MediaUploadResult> UploadMedia(IFormFile file);
    }
}
