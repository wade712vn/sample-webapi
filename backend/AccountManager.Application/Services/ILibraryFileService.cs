using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Application.Services
{
    public interface ILibraryFileService
    {
        Task<bool> FileExists(string fileUrl);
    }
}
