#nullable enable

namespace Bot.Models
{
    /// <summary>
    ///     Model representing the fixed structure of a BSON doc containing
    ///     specific responses as a list.
    /// </summary>
    internal class ResponseDoc : BaseTemplateDoc
    {
        public string[]? Responses { get; set; }
    }
}
