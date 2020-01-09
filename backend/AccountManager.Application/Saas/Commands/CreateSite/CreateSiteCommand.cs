using System.Linq;
using System.Text;
using MediatR;

namespace AccountManager.Application.Saas.Commands.CreateSite
{
    public class CreateSiteCommand : CommandBase
    {
        public string Name { get; set; }
        public string UrlFriendlyName { get; set; }
        public string CloudInstanceType { get; set; }

        public string AccountUrlFriendlyName { get; set; }
    }
}
