using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using MediatR;

namespace AccountManager.Application.Saas.Commands.CreateSite
{
    public class SiteCreatedEvent : INotification
    {
        public string Actor { get; set; }
        public Site Site { get; set; }
        public dynamic Command { get; set; }
    }
}
