namespace DayBreaks.Solver
{
    public interface IDimensionMatrix
    {
        public long CalculateDistance(int fromNode, int toNode);
        public long CalculateTime(int fromNode, int toNode);
        public long CalculateDemand(int atNode);
        public long CalculateDuration(int fromNode, int toNode);
    }
}