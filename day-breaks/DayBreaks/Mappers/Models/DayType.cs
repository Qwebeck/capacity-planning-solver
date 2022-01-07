using System;

namespace DayBreaks.Mappers.Models
{
    public enum DayType { Hard, Normal, Easy }
    
    public static class DayTypesConverter
    {
        private const double EasyCoefficient = 0.31;
        private const double NormalCoefficient = 0.51;
        private const double HardCoefficient = 1;
        private static double Coefficient(this DayType dayType) => dayType switch
        {
            DayType.Easy => EasyCoefficient,
            DayType.Normal => NormalCoefficient,
            DayType.Hard => HardCoefficient,
            _ => throw new ArgumentOutOfRangeException(nameof(dayType), dayType, null)
        };
        public static int CalculateVisitsCount(this DayType dayType, int maxCount) 
            => Convert.ToInt32(dayType.Coefficient() * maxCount);

        public static DayType DetermineDayType(int visitsCount, int maxVisitsCount) =>
            ((double) visitsCount / maxVisitsCount) switch
            {
                <= EasyCoefficient   => DayType.Easy,
                <= NormalCoefficient => DayType.Normal,
                <= HardCoefficient   => DayType.Hard,
                var v => throw new ArgumentOutOfRangeException($"{nameof(visitsCount)}/{nameof(maxVisitsCount)}", v,
                    null)
            };
    }
}