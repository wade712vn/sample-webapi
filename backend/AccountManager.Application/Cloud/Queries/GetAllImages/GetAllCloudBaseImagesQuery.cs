using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AccountManager.Application.Models.Dto;

namespace AccountManager.Application.Cloud.Queries.GetAllImages
{
    public class GetAllImagesQuery : IRequest<List<CloudBaseImageDto>>
    {
    }
}
