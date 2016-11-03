using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Linq;

namespace StrategyIncubator
{
    class Log
    {
        private List<string> _logQueue = new List<string>();
        private string _logPath, _logName;
        private int _logQueueSize;

        public enum LogLevel
        {
            Debug,
            Info,
            Success,
            Warn,
            Text,
            Error
        }
        
        public Log(string logName, string logPath, int queueSize)
        {
            _logName = logName;
            _logPath = Path.Combine("Logs", logPath);
            _logQueueSize = queueSize;

            Directory.CreateDirectory("Logs");
            if (!File.Exists(_logPath))
            {
                File.Create(_logPath).Close();
                Console.WriteLine($"Log file '{_logName}' created for path '{_logPath}");
                Thread.Sleep(250);
            }
        }
        
        private string GetTimestamp()
        {
            return DateTime.Now.ToString("d/M/yyyy HH:mm:ss");
        }
        
        public void Write(LogLevel level, string str, bool logToFile = true, bool writeToConsole = true, bool rawMessage = false)
        {
            switch (level)
            {
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogLevel.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogLevel.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Text:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
            }

            string formattedMessage = $"[{GetTimestamp()}] {level}: {str}";

            if (writeToConsole)
                Console.WriteLine($"{_logName} {formattedMessage}");

            if (logToFile)
            {
                if (rawMessage)
                    _logQueue.Add(str);
                else
                    _logQueue.Add(formattedMessage);
            }


            Console.ForegroundColor = ConsoleColor.White;
            FlushLog();
        }
        
        private void FlushLog()
        {
            if (_logQueue.Count < _logQueueSize)
                return;

            if (!File.Exists(_logPath))
                File.Create(_logPath);

            try
            {
                var queueList = _logQueue.ToList();
                using (FileStream fs = File.Open(_logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        foreach (var str in queueList)
                            sw.WriteLine(str);

                        _logQueue.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Write(LogLevel.Error, $"Unable to flush log - {ex.Message}");
            }
        }
    }
}