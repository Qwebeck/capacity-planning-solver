using System.Collections.Generic;
using DayBreaks.Mappers;
using DayBreaks.Mappers.Models;

namespace DayBreaks.FleetStructureTesting.Models
{
    public record TestCase(string Instance, IEnumerable<CharacteristicDayDescription> CharacteristicDays);
}