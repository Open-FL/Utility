using System.IO;
using System.Xml.Serialization;

namespace Utility.Benchmarking
{
    public class BenchmarkHelper
    {

        public static int RunNumber = 0;

        protected readonly string PerformanceFolder;


        protected readonly string testName;

        public BenchmarkHelper(string testName, string performance = "performance")
        {
            this.testName = testName;
            PerformanceFolder = performance;
        }

        protected string RunResultPath => Path.Combine(PerformanceFolder, RunNumber.ToString());

        protected string PerformanceOutputFile => Path.Combine(PerformanceFolder, $"{testName}.log");

        protected string DataOutputDirectory => Path.Combine(RunResultPath, "data");

        public void WriteLog(string log)
        {
            File.AppendAllText(PerformanceOutputFile, log);
        }

        public Stream GetDataFileStream(string filename)
        {
            Directory.CreateDirectory(Path.Combine(DataOutputDirectory, Path.GetDirectoryName(filename)));
            return File.Create(Path.Combine(DataOutputDirectory, filename));
        }

        public void WriteResult(PerformanceTester.PerformanceResult result)
        {
            XmlSerializer xs = new XmlSerializer(typeof(PerformanceTester.PerformanceResult));
            Directory.CreateDirectory(RunResultPath);
            Stream s = File.Create(Path.Combine(RunResultPath, $"{result.TestName}.xml"));
            xs.Serialize(s, result);
            s.Close();
        }

    }
}