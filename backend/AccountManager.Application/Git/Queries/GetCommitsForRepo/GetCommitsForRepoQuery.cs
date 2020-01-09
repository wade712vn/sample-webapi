using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Git.Queries.GetCommitsForRepo
{
    public class GetCommitsForRepoQuery : IRequest<PagedData<CommitDto>>
    {
        public string Repo { get; set; }
        public int? BranchId { get; set; }
        public bool TagOnly { get; set; }
        public string Search { get; set; }
        public int StartIndex { get; set; }
        public int Limit { get; set; }

        public GetCommitsForRepoQuery()
        {
            StartIndex = 0;
            Limit = 20;
        }
    }
}