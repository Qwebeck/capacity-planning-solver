using System.Collections.Generic;

namespace DayBreaks.CLI
{
    public record MultipleChoiceOptions<T>(string Name, Dictionary<string, T> Map)
    {
        public T Value => Map[Name];
    }
}