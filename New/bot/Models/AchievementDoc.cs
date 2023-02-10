using System.Collections.Generic;

#nullable enable

namespace Bot.Models
{
    /// <summary>
    /// Model representing the fixed structure of a doc belonging to the
    /// Achievements collection
    /// </summary>
    internal class AchievementDoc : BaseTemplateDoc
    {
        public Dictionary<int, string> Achievements { get; set; } = null!;
    }
}
