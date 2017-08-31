using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using log4net.Core;
using NLog;
using NLog.Fluent;
using LogManager = log4net.LogManager;

namespace LogDemo
{
    class Program
    {
        private static ILog _log4NetKeepFalse;
        private static ILog _log4NetKeepTrue;
        private static Logger _nlogKeppFalse;
        private static Logger _nlogKeppTrue;

        static void Main(string[] args)
        {
            var log4NetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net");
            var nlogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nlog");

            if (Directory.Exists(log4NetPath))
            {
                Directory.Delete(log4NetPath, true);
            }

            if (Directory.Exists(nlogPath))
            {
                Directory.Delete(nlogPath ,true);
            }

            InitLog4Net();
            InitNLog();

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 10000; i++)
            {
                _log4NetKeepFalse.Debug("log测试");
            }

            sw.Stop();
            Console.WriteLine("log4net无锁日志循环10000次，总耗时：" + sw.ElapsedMilliseconds);

            sw.Restart();

            for (int i = 0; i < 10000; i++)
            {
                _nlogKeppFalse.Debug("log测试");
            }

            sw.Stop();
            Console.WriteLine("Nlog无锁日志循环10000次，总耗时：" + sw.ElapsedMilliseconds);

            //初始化200个线程备用(线程池热身)
            Parallel.For(0, 200, i => Thread.Sleep(100));

            sw.Restart();

            Parallel.For(0, 100, i =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _log4NetKeepFalse.Debug("log测试");
                }
            });

            sw.Stop();
            Console.WriteLine("log4net无锁日志并行循环，总耗时：" + sw.ElapsedMilliseconds);

            sw.Restart();

            Parallel.For(0, 100, i =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _nlogKeppFalse.Debug("log测试");
                }
            });

            sw.Stop();
            Console.WriteLine("Nlog无锁日志并行循环，总耗时：" + sw.ElapsedMilliseconds);

            sw.Restart();

            for (int i = 0; i < 10000; i++)
            {
                _log4NetKeepTrue.Debug("log测试");
            }

            sw.Stop();
            Console.WriteLine("log4net独锁日志循环10000次，总耗时：" + sw.ElapsedMilliseconds);

            sw.Restart();

            for (int i = 0; i < 10000; i++)
            {
                _nlogKeppTrue.Debug("log测试");
            }

            sw.Stop();
            Console.WriteLine("Nlog独锁日志循环10000次，总耗时：" + sw.ElapsedMilliseconds);

            //初始化200个线程备用(线程池热身)
            Parallel.For(0, 200, i => Thread.Sleep(1000));

            sw.Restart();

            Parallel.For(0, 100, i =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _log4NetKeepTrue.Debug("log测试");
                }
            });

            sw.Stop();
            Console.WriteLine("log4net独锁日志并行循环，总耗时：" + sw.ElapsedMilliseconds);

            sw.Restart();

            Parallel.For(0, 100, i =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _nlogKeppTrue.Debug("log测试");
                }
            });

            sw.Stop();
            Console.WriteLine("Nlog独锁日志并行循环，总耗时：" + sw.ElapsedMilliseconds);
            Console.WriteLine("统计输出日志文件大小：");

            var log4NetFiles = Directory.GetFiles(log4NetPath);
            var log4NetSum = 0L;
            foreach (var log4NetFile in log4NetFiles)
            {
                var f = new FileInfo(log4NetFile);
                log4NetSum += f.Length;
            }

            Console.WriteLine($"log4Net共{log4NetFiles.Length}个文件，共{log4NetSum / 1024}Kb");

            var nlogFiles = Directory.GetFiles(nlogPath);
            var nlogSum = 0L;
            foreach (var nlogFile in nlogFiles)
            {
                var f = new FileInfo(nlogFile);
                nlogSum += f.Length;
            }

            Console.WriteLine($"nlog共{nlogFiles.Length}个文件，共{nlogSum / 1024}Kb");

            Console.WriteLine("按任意键结束");

            Console.ReadKey();
        }

        private static void InitLog4Net()
        {
            XmlConfigurator.ConfigureAndWatch(
                new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
            _log4NetKeepFalse = LogManager.GetLogger("keepfalse");
            _log4NetKeepTrue = LogManager.GetLogger("keeptrue");
        }

        private static void InitNLog()
        {
            _nlogKeppFalse = NLog.LogManager.GetLogger("keepfalse");
            _nlogKeppTrue = NLog.LogManager.GetLogger("keeptrue");
        }
    }
}
