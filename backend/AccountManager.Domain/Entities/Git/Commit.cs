using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Domain.Entities.Git
{
    public class Commit
    {
        public long Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }
        public string Tag { get; set; }
        public string FullHash { get; set; }
        public string ShortHash { get; set; }
        
        public string Repo { get; set; }
        public long BranchId { get; set; }
        public Branch Branch { get; set; }
        
    }

    public class Repo
    {
        private ICollection<Commit> _commits;

        public string Name { get; set; }
        public string Url { get; set; }

        public ICollection<Commit> Commits
        {
            get => _commits ?? (_commits = new List<Commit>());
            set => _commits = value;
        }
    }

    public class Branch
    {
        private ICollection<Commit> _commits;

        public long Id { get; set; }
        public string Name { get; set; }
        public string Repo { get; set; }
        public bool Stable { get; set; }

        public ICollection<Commit> Commits
        {
            get => _commits ?? (_commits = new List<Commit>());
            set => _commits = value;
        }
    }
}
