namespace TrafficSim
{
    internal class Clock : IClock
    {
        public int Now { get; private set; }

        public Clock()
        {
            Now = 0;
        }

        public void Advance(int duration)
        {
            Now += duration;
        }
    }
}
