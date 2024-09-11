namespace TrafficSim.Printer
{
    internal class SingleFilePrinter
    {
        private readonly List<IProcessor> _processors;
        private readonly StreamWriter _writer;

        public SingleFilePrinter(List<IProcessor> processors)
        {
            _processors = processors;
            _writer = new StreamWriter(File.Open("output.log", FileMode.Create, FileAccess.Write));
        }

        public void PrintTile()
        {
            _processors[0].PrintTitle(_writer);
        }

        public void Print()
        {
            for (int i = 0; i < _processors.Count; ++i)
            {
                _processors[i].DumpStats(_writer);
            }
        }
    }
}
