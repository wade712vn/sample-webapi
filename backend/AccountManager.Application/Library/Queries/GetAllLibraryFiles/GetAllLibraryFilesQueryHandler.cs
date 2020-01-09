using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Git.Queries.GetAllRepos;
using AccountManager.Application.Key;
using AccountManager.Application.Models.Dto;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Entities.Git;
using AccountManager.Domain.Entities.Library;
using AccountManager.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AccountManager.Application.Library.Queries.GetAllLibraryFiles
{
    public class GetAllLibraryFilesQueryHandler : IRequestHandler<GetAllLibraryFilesQuery, List<FileDto>>
    {
        private readonly IAccountManagerDbContext _context;
        private readonly IKeysManager _keyManager;

        public GetAllLibraryFilesQueryHandler(IAccountManagerDbContext context, IKeysManager keyManager)
        {
            _context = context;
            _keyManager = keyManager;
        }

        public async Task<List<FileDto>> Handle(GetAllLibraryFilesQuery request, CancellationToken cancellationToken)
        {
            var files = await _context.Set<File>()
                .Include(x => x.Packages)
                .Where(x => x.Id > 1)
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync(cancellationToken);

            var dtos = new List<FileDto>();

            foreach (var file in files)
            {
                var dto = Mapper.Map<FileDto>(file);
                dtos.Add(dto);
            }

            return await Task.FromResult(dtos);
        }

        private bool VerifyLibraryFileSignature(File libraryFile, string lctInterServerPublicKey)
        {
            // Create LctSignatureBlock object
            var signatureBlock = new
            {
                libraryFile.Type,
                libraryFile.Url,
                AccountName = libraryFile.AddedBy,
                Timestamp = libraryFile.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fffK", CultureInfo.InvariantCulture)
            };
            
            var json = JsonConvert.SerializeObject(signatureBlock, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                TypeNameHandling = TypeNameHandling.None,
            });
            var bytes = Encoding.UTF8.GetBytes(json);

            using (var provider = DSA.Create())
            using (var sha = SHA1.Create())
            {
                var rgbHash = sha.ComputeHash(bytes);
                var rgbSignature = Convert.FromBase64String(libraryFile.Signature);

                provider.FromXmlString(lctInterServerPublicKey);

                return provider.VerifySignature(rgbHash, rgbSignature);
            }
        }
    }
}
