using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Utility.Benchmarking
{
    public class PerformanceTester
    {

        private const string TARGET_DIRECTORY = "performance_targets";

        public static readonly PerformanceTester Tester = new PerformanceTester();

        private readonly List<PerformanceTarget> Targets;


        private PerformanceTester()
        {
            Targets = new List<PerformanceTarget>();
            Directory.CreateDirectory(TARGET_DIRECTORY);
            string[] files = Directory.GetFiles(TARGET_DIRECTORY, "*.xml", SearchOption.AllDirectories);
            XmlSerializer xs = new XmlSerializer(typeof(PerformanceTarget));
            for (int i = 0; i < files.Length; i++)
            {
                Stream s = null;
                try
                {
                    s = File.OpenRead(files[i]);
                    Targets.Add((PerformanceTarget) xs.Deserialize(s));
                    s.Close();
                }
                catch (Exception)
                {
                    s?.Close();
                }
            }
        }

        private void WriteTarget(PerformanceTarget target)
        {
            Stream s = null;
            try
            {
                s = File.Create(Path.Combine(TARGET_DIRECTORY, target.TestName + ".xml"));
                XmlSerializer xs = new XmlSerializer(typeof(PerformanceTarget));
                xs.Serialize(s, target);
                s.Close();
            }
            catch (Exception)
            {
                s?.Close();
            }
        }

        private PerformanceTarget GetTarget(string name)
        {
            return Targets.FirstOrDefault(x => x.TestName == name);
        }

        public PerformanceResult RunTest(
            string testName, int testCount, Action<int> beforeTest, Action<int> test,
            Action<int> afterTest)
        {
            PerformanceTarget target = GetTarget(testName);
            Stopwatch sw = new Stopwatch();
            for (int i = 0; i < testCount; i++)
            {
                beforeTest?.Invoke(i);
                sw.Start();
                test(i);
                sw.Stop();
                afterTest?.Invoke(i);
            }

            decimal result = (decimal) sw.Elapsed.TotalMilliseconds;
            if (target == null)
            {
                target = new PerformanceTarget(testName, result, testCount);
                Targets.Add(target);
                WriteTarget(target);
            }

            return new PerformanceResult(target, testCount, result);
        }

        public class PerformanceTarget
        {

            public bool LowerIsBetter;
            public decimal Target;
            public string TestName;
            public decimal Variance;

            public PerformanceTarget()
            {
            }

            public PerformanceTarget(string nameOfTest, decimal target, int n)
            {
                TestName = nameOfTest;
                Target = target;
                Variance = Target / 2;
                LowerIsBetter = true;
            }

        }

        public class PerformanceResult
        {

            public bool LowerIsBetter;
            public int N;
            public decimal Result;
            public decimal Target;
            public string TestName;
            public decimal Variance;

            public PerformanceResult()
            {
            }

            public PerformanceResult(PerformanceTarget target, int n, decimal result)
            {
                LowerIsBetter = target.LowerIsBetter;
                TestName = target.TestName;
                N = n;
                Result = result;
                Target = target.Target;
                Variance = target.Variance;
            }

            public bool Matched =>
                (!LowerIsBetter || TargetAndVariance >= Result) &&
                (LowerIsBetter || TargetAndVariance <= Result);

            public decimal TargetAndVariance => LowerIsBetter ? Target + Variance : Target - Variance;

            public decimal DeltaFromTarget => Target - Result;

            public decimal Percentage => Math.Round(Result / Target * 100, 4);

            public override string ToString()
            {
                return
                    $"[{(Matched ? "Pass" : "Fail")}] {TestName}: {Result}ms ({Percentage}%); Target: {Target}ms; Delta: {DeltaFromTarget}ms";
            }

        }

    }
}