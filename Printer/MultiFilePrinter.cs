namespace TrafficSim.Printer
{
    internal class MultiFilePrinter
    {
        private readonly List<IProcessor> _processors;
        private readonly List<StreamWriter> _writers;

        public MultiFilePrinter(List<IProcessor> processors)
        {
            _processors = processors;
            _writers = new List<StreamWriter>(processors.Count);

            foreach (var p in processors)
            {
                _writers.Add(new StreamWriter(File.Open(p.Name + ".log", FileMode.Create, FileAccess.Write)));
            }
        }

        public void PrintTile()
        {
            for (int i = 0; i < _processors.Count; ++i)
            {
                _processors[i].PrintTitle(_writers[i]);
            }
        }

        public void Print()
        {
            for (int i = 0; i < _processors.Count; ++i)
            {
                _processors[i].DumpStats(_writers[i]);
            }
        }
    }
}
