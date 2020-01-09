using AccountManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Application.Mappings
{
    public class MiscMappingProfile : ProfileBase
    {
        public MiscMappingProfile()
        {
            CreateMap<State, HistoricalDesiredState>();
        }
    }
}
