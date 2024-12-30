namespace Diplom
{
    public class Position
    {
        public int PositionId { get; set; }
        public string PositionName { get; set; }

        public static Position Of(string name)
        {
            return new Position() {PositionName = name};
        }
    }
}