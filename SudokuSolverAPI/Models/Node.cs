namespace SudokuSolverAPI.Models
{
    public class Node
    {
        public int? CurrentValue { get; set; }
        public Dictionary<int,bool>? Possibles { get; set; }
        public int KnownNumberCount { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Zone { get; set; }
    }
}
