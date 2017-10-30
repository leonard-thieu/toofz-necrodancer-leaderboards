﻿using System;

namespace toofz.NecroDancer.Leaderboards
{
    public sealed class Run
    {
        private Run() { }

        public Run(int runId, string name, string displayName) : this()
        {
            RunId = runId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }

        public int RunId { get; private set; }
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
    }
}
