namespace Models
{
    class Score
    {
        public int ResourceCount = 0;
        public int ResourceStack = 0;

        public Score()
        {
            ResourceCount = 0;
            ResourceStack = 0;
        }

        public Score(Score CopyFrom)
        {
            ResourceCount = CopyFrom.ResourceCount;
            ResourceStack = CopyFrom.ResourceStack;
        }
    }
}