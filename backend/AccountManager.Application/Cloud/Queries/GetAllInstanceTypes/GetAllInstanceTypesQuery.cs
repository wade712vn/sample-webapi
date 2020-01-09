using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AccountManager.Application.Models.Dto;

namespace AccountManager.Application.Cloud.Queries.GetAllInstanceTypes
{
    public class GetAllInstanceTypesQuery : IRequest<List<CloudInstanceTypeDto>>
    {
    }
}
