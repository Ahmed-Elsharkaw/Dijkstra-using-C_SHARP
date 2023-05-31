using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Dijkstra
{
    class NodeData
    {
        public NodeData(int n, int x, int y)
        {
            NodeId = n;
            Place = new Point(x, y);
        }

        public int NodeId { get; private set; }
        public Point Place { get; private set; }
    }

    class LinkData
    {
        public LinkData(int s, int e, decimal l, decimal t)
        {
            StartId = s;
            EndId = e;
            Time = t;
            Distance = l;
        }

        public int StartId { get; private set; }
        public int EndId { get; private set; }
        public decimal Time { get; private set; }
        public decimal Distance { get; private set; }
    }

    class CaseData
    {
        public CaseData(int s, int e)
        {
            StartId = s;
            EndId = e;
        }

        public int StartId { get; private set; }
        public int EndId { get; private set; }
    }

    class Program
    {
        static int _numberOfNodes;
        static IList<NodeData> _nodeTable;
        static IList<LinkData> _linkTable;
        static IList<CaseData> _cases;
        static Dijkstra _distanceSolver;
        static Dijkstra _timeSolver;
        
        static void Main(string[] args)
        {
            var fileName = Path.GetFullPath(@"..\..\data\small_input.in");
            var input = new FileStream(fileName, FileMode.Open);
            var output = new FileStream(Path.GetDirectoryName(fileName) + @"\" + Path.GetFileNameWithoutExtension(fileName) + ".out", FileMode.Create);
            var reader = new StreamReader(input);
            using (var writer = new StreamWriter(output))
            {
                _nodeTable = new List<NodeData>();
                _linkTable = new List<LinkData>();
                _cases = new List<CaseData>();
                _numberOfNodes = ReadData(reader);
#if DEBUG
                ShowData();
#endif
                _distanceSolver = new Dijkstra(_numberOfNodes, GetDistance);
                _timeSolver = new Dijkstra(_numberOfNodes, GetTime);
                for (var i = 0; i < _cases.Count; i++)
                {
                    var header = string.Format("Case {0}: ", i + 1);
                    writer.WriteLine(header);
#if DEBUG
                    Console.WriteLine(header);
#endif
                    var start = DateTime.Now;
                    var solution = _distanceSolver.Solve(_cases[i].StartId, _cases[i].EndId);
                    var ellaped = DateTime.Now.Subtract(start);
                    var path = solution.Aggregate("", (s, n) => { return (s == "" ? "" : s + "->") + n; });
                    var segments = solution.Zip(solution.Skip(1), (a, b) => Tuple.Create<int, int>(a, b));
                    var distance = segments.Select(s => GetDistance(s.Item1, s.Item2)).Sum();
                    var time = segments.Select(s => GetTime(s.Item1, s.Item2)).Sum();
                    writer.WriteLine("Shortest: {0}: {1} miles, {2} minutes; Solution Time:{3};", path, distance.ToString("F3"), time.ToString("F1"), ellaped.ToString("G"));
#if DEBUG
                    Console.WriteLine("Shortest: {0}: {1} miles, {2} minutes; Solution Time:{3};", path, distance.ToString("F3"), time.ToString("F1"), ellaped.ToString("G"));
#endif
                    start = DateTime.Now;
                    solution = _timeSolver.Solve(_cases[i].StartId, _cases[i].EndId);
                    ellaped = DateTime.Now.Subtract(start);
                    path = solution.Aggregate("", (s, n) => { return (s == "" ? "" : s + "->") + n; });
                    segments = solution.Zip(solution.Skip(1), (a, b) => Tuple.Create<int, int>(a, b));
                    distance = segments.Select(s => GetDistance(s.Item1, s.Item2)).Sum();
                    time = segments.Select(s => GetTime(s.Item1, s.Item2)).Sum();
                    writer.WriteLine("Fastest : {0}: {1} miles, {2} minutes; Solution Time:{3};", path, distance.ToString("F3"), time.ToString("F1"), ellaped.ToString("G"));
#if DEBUG
                    Console.WriteLine("Fastest : {0}: {1} miles, {2} minutes; Solution Time:{3};", path, distance.ToString("F3"), time.ToString("F1"), ellaped.ToString("G"));
#endif
                }
                writer.Close();
            }
        }

        static int ReadData(StreamReader reader)
        {
            var result = 0;
            var nodeCount = int.Parse(reader.ReadLine());
            for (var n = 0; n < nodeCount; n++)
            {
                result++;
                var nodeData = reader.ReadLine().Split('\t').Select(v => int.Parse(v)).ToArray();
                _nodeTable.Add(new NodeData(n, nodeData[0], nodeData[1]));
            }
            var linkCount = int.Parse(reader.ReadLine());
            for (var l = 0; l < linkCount; l++)
            {
                var linkData = reader.ReadLine().Split('\t').ToArray();
                _linkTable.Add(new LinkData(int.Parse(linkData[0]), int.Parse(linkData[1]), decimal.Parse(linkData[2].Replace(".", ",")), decimal.Parse(linkData[3].Replace(".", ","))));
            }
            var caseCount = int.Parse(reader.ReadLine());
            for (var c = 0; c < caseCount; c++)
            {
                var caseData = reader.ReadLine().Split('\t').Select(v => int.Parse(v)).ToArray();
                _cases.Add(new CaseData(caseData[0], caseData[1]));
            }
            return result;
        }

        static decimal GetTime(int start, int end)
        {
            return (from n in _linkTable
                    where n.StartId == start
                    && n.EndId == end
                    select n.Time).DefaultIfEmpty(-1).Single();
        }

        static decimal GetDistance(int start, int end)
        {
            return (from n in _linkTable
                    where n.StartId == start
                    && n.EndId == end
                    select n.Distance).DefaultIfEmpty(-1).Single();
        }
#if DEBUG
        static void ShowData()
        {
            Console.WriteLine("Link Matrix:");
            for (var sn = 0; sn < _numberOfNodes; sn++)
            {
                for (var en = 0; en < _numberOfNodes; en++)
                {
                    Console.Write(GetDistance(sn, en) > 0 ? "." : " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
#endif
    }
}
