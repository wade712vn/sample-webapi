using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Git;
using AccountManager.Domain.Entities.Library;
using AccountManager.Application.Common.Interfaces;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Queries.GetSoftwareInfoForMachine
{
    public class GetSoftwareInfoForMachineQuery : IRequest<MachineStateInfoDto>
    {
        public long Id { get; set; }
    }

    public class GetSoftwareInfoForMachineQueryHandler : IRequestHandler<GetSoftwareInfoForMachineQuery, MachineStateInfoDto>
    {
        private readonly IAccountManagerDbContext _context;

        public GetSoftwareInfoForMachineQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<MachineStateInfoDto> Handle(GetSoftwareInfoForMachineQuery request, CancellationToken cancellationToken)
        {
            var states = new List<State>();
            var latestState = await _context.Set<State>().OrderByDescending(x => x.Timestamp)
                .FirstOrDefaultAsync(x => x.MachineId == request.Id && !x.Desired, cancellationToken);

            if (latestState != null)
                states.Add(latestState);

            var desiredState = await _context.Set<State>()
                .FirstOrDefaultAsync(x => x.MachineId == request.Id && (x.Desired), cancellationToken);

            if (desiredState != null)
                states.Add(desiredState);

            var stateDtos = Mapper.Map<List<StateDto>>(states);

            var hashes = states.SelectMany(x =>
            {
                var stateHashes = new List<string>
                {
                    x.Launcher,
                    x.Reporting,
                    x.PdfExport,
                    x.SiteMaster,
                    x.Client,
                    x.RelExport,
                    x.Deployer,
                    x.Populate
                };
                return stateHashes;
            }).Distinct().Where(x => !x.IsNullOrWhiteSpace() && x != Versions.None);

            var commits = await _context.Set<Commit>().Include(x => x.Branch).Where(x => hashes.Contains(x.ShortHash)).ToListAsync(cancellationToken);
            var commitDtos = Mapper.Map<List<CommitDto>>(commits);
            var commitGroupByBranch = commitDtos.GroupBy(x => x.BranchId).OrderByDescending(x => x.Count());

            foreach (var stateDto in stateDtos) { 
                foreach (var group in commitGroupByBranch)
                {
                    stateDto.LauncherCommit = stateDto.LauncherCommit ?? group.FirstOrDefault(x => x.ShortHash == stateDto.Launcher);
                    stateDto.ReportingCommit = stateDto.ReportingCommit ?? group.FirstOrDefault(x => x.ShortHash == stateDto.Reporting);
                    stateDto.PdfExportCommit = stateDto.PdfExportCommit ?? group.FirstOrDefault(x => x.ShortHash == stateDto.PdfExport);
                    stateDto.SiteMasterCommit = stateDto.SiteMasterCommit ?? group.FirstOrDefault(x => x.ShortHash == stateDto.SiteMaster);
                    stateDto.ClientCommit = stateDto.ClientCommit ?? group.FirstOrDefault(x => x.ShortHash == stateDto.Client);
                    stateDto.RelExportCommit = stateDto.RelExportCommit ?? group.FirstOrDefault(x => x.ShortHash == stateDto.RelExport);
                    stateDto.DeployerCommit = stateDto.DeployerCommit ?? group.FirstOrDefault(x => x.ShortHash == stateDto.Deployer);
                    stateDto.PopulateCommit = stateDto.PopulateCommit ?? group.FirstOrDefault(x => x.ShortHash == stateDto.Populate);
                }

                stateDto.LibraryFiles = stateDto.LibraryFileIds.Any() ? Mapper.Map<IEnumerable<FileDto>>(await _context.Set<File>()
                    .Where(x => x.Id != 0 && stateDto.LibraryFileIds.Contains(x.Id)).ToListAsync(cancellationToken)).ToArray() : new FileDto[0];

                stateDto.AccountLibraryFile = stateDto.AccountLibraryFileId.HasValue ? Mapper.Map<FileDto>(await _context.Set<File>()
                    .FirstOrDefaultAsync(x => x.Id != 0 && stateDto.AccountLibraryFileId.Value == x.Id, cancellationToken)) : null;
            }

            return new MachineStateInfoDto()
            {
                States = stateDtos
            };
        }
    }
}
