﻿using OpenTracker.Models.Accessibility;
using OpenTracker.Models.Dungeons.Mutable;
using OpenTracker.Models.Requirements;
using ReactiveUI;

namespace OpenTracker.Models.KeyDoors
{
    /// <summary>
    /// This interface contains key door data.
    /// </summary>
    public interface IKeyDoor : IReactiveObject
    {
        AccessibilityLevel Accessibility { get; }
        bool Unlocked { get; set; }
        IRequirement Requirement { get; }

        delegate IKeyDoor Factory(IMutableDungeon dungeonData);
    }
}