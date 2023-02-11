using System.Collections.Generic;

#nullable enable

namespace Bot.Models
{
    /// <summary>
    ///     Model representing the fixed structure of a doc belonging to the
    ///     Roles collection
    /// </summary>
    internal class RolesDoc : BaseTemplateDoc
    {
        public Dictionary<string, ulong>? Roles { get; set; }
    }
}
