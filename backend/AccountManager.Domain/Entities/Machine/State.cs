using System;

namespace AccountManager.Domain.Entities
{
    public class State : StateBase
    {
        public bool Desired { get; set; }
    }

    public class HistoricalDesiredState : StateBase
    {
    }
}