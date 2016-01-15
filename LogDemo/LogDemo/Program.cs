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
        private static ILog _log4netKeepFalse;
        private static ILog _log4netKeepTrue;
        private static Logger _nlogKeppFalse;
        private static Logger _nlogKeppTrue;

        static void Main(string[] args)
        {
            InitLog4Net();
            InitNLog();

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 10000; i++)
            {
                _log4netKeepFalse.Debug("log4net测试");
            }

            sw.Stop();
            Console.WriteLine("log4net无锁日志循环10000次，总耗时：" + sw.ElapsedMilliseconds);

            sw.Restart();

            for (int i = 0; i < 10000; i++)
            {
                _nlogKeppFalse.Debug("Nlog测试");
            }

            sw.Stop();
            Console.WriteLine("Nlog无锁日志循环10000次，总耗时：" + sw.ElapsedMilliseconds);

            //初始化300个线程备用
            Parallel.For(0, 200, (i) => Thread.Sleep(1000));

            sw.Restart();

            Parallel.For(0, 100, (i) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _log4netKeepFalse.Debug("log4net测试");
                }
            });

            sw.Stop();
            Console.WriteLine("log4net无锁日志并行循环，总耗时：" + sw.ElapsedMilliseconds);

            sw.Restart();

            Parallel.For(0, 100, (i) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _nlogKeppFalse.Debug("Nlog测试");
                }
            });

            sw.Stop();
            Console.WriteLine("Nlog无锁日志并行循环，总耗时：" + sw.ElapsedMilliseconds);

            sw.Restart();

            for (int i = 0; i < 10000; i++)
            {
                _log4netKeepTrue.Debug("log4net测试");
            }

            sw.Stop();
            Console.WriteLine("log4net独锁日志循环10000次，总耗时：" + sw.ElapsedMilliseconds);

            sw.Restart();

            for (int i = 0; i < 10000; i++)
            {
                _nlogKeppTrue.Debug("Nlog测试");
            }

            sw.Stop();
            Console.WriteLine("Nlog独锁日志循环10000次，总耗时：" + sw.ElapsedMilliseconds);

            //初始化300个线程备用
            Parallel.For(0, 200, (i) => Thread.Sleep(1000));

            sw.Restart();

            Parallel.For(0, 100, (i) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _log4netKeepTrue.Debug("log4net测试");
                }
            });

            sw.Stop();
            Console.WriteLine("log4net独锁日志并行循环，总耗时：" + sw.ElapsedMilliseconds);

            sw.Restart();

            Parallel.For(0, 100, (i) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _nlogKeppTrue.Debug("Nlog测试");
                }
            });

            sw.Stop();
            Console.WriteLine("Nlog独锁日志并行循环，总耗时：" + sw.ElapsedMilliseconds);

            Console.ReadKey();
        }

        private static void InitLog4Net()
        {
            XmlConfigurator.ConfigureAndWatch(
                new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
            _log4netKeepFalse = LogManager.GetLogger("keepfalse");
            _log4netKeepTrue = LogManager.GetLogger("keeptrue");
        }

        private static void InitNLog()
        {
            _nlogKeppFalse = NLog.LogManager.GetLogger("keepfalse");
            _nlogKeppTrue = NLog.LogManager.GetLogger("keeptrue");
        }
    }
}
