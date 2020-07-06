using Plugin.NetStandardStorage.Abstractions.Interfaces;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using TUtils.Collections.Enumeration;
using TUtils.Error;

namespace TUtils.IO
{
    /// <summary>
    /// Used to log data to streams.
    /// </summary>
    public sealed class Log : IDisposable
    {
        public static Log LogFile;

        /// <summary>
        /// The file object that corrosponds with the log.
        /// </summary>
        private readonly IFile LogFileInstance;

        /// <summary>
        /// The writer used to write to the logfile.
        /// </summary>
        private readonly TextWriter Writer;

        /// <summary>
        /// Used to synchronous output to the console in order to maintain proper coloring for output in order.
        /// </summary>
        private readonly object ConsoleSyncObject = new object();

        /// <summary>
        /// The prefix to prepend to messages of error level.
        /// </summary>
        private const string ErrorPrefix = "ERR";

        /// <summary>
        /// The prefix to prepend to messages of warning level.
        /// </summary>
        private const string WarnPrefix = "WARN";

        /// <summary>
        /// The prefix to prepend to messages of information level.
        /// </summary>
        private const string InfoPrefix = "INFO";

        /// <summary>
        /// The prefix to prepend to messages of debug level.
        /// </summary>
        private const string DebugPrefix = "DBG";

        /// <summary>
        /// The prefix to prepend to messages of no level.
        /// </summary>
        private const string NonePrefix = "";

        /// <param name="logPath">The path to where this log is logging to.</param>
        public Log(IFolder instanceRoot)
        {
            this.LogFileInstance = instanceRoot.CreateFile("Masterlog.txt", Plugin.NetStandardStorage.Abstractions.Types.CreationCollisionOption.ReplaceExisting);
            this.Writer = new StreamWriter(this.LogFileInstance.Open(FileAccess.ReadWrite));
        }

        /// <summary>
        /// Handles all the signals that require this log to quicklly flush itself.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleClosingSignal(object sender, EventArgs e)
        {
            this.Writer.Flush();
        }

        /// <summary>
        /// Writes the specified data to the log file. If there is more than one object passed in, each piece will get its own line.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task WriteAsync<DataType>(LogLevel level, params DataType[] data)
        {
            string levelRepresentation = this.GetLogLevelPrefix(level);
            await this.WriteAsync(levelRepresentation, data);
        }

        /// <summary>
        /// Writes the specified data to the log file. If there is more than one object passed in, each piece will get its own line.
        /// </summary>
        /// <typeparam name="DataType"></typeparam>
        /// <param name="level"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task WriteAsync<DataType>(string level, params DataType[] data)
        {
            data.Iterate(async (dataItem, dataIndex) =>
            {
                await this.WriteLineAsync(level, dataItem);
            });
        }

        /// <summary>
        /// Writes the specified data to the log file. If there is more than one object passed in, each piece will get its own line.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public void Write<DataType>(LogLevel level, params DataType[] data)
        {
            string levelRepresentation = this.GetLogLevelPrefix(level);
            this.Write(levelRepresentation, data);
        }

        /// <summary>
        /// Writes the specified data to the log file. If there is more than one object passed in, each piece will get its own line.
        /// </summary>
        /// <typeparam name="DataType"></typeparam>
        /// <param name="level"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private void Write<DataType>(string level, params DataType[] data)
        {
            data.Iterate((dataItem, dataIndex) =>
            {
                this.WriteLine(level, dataItem);
            });
        }

        /// <summary>
        /// Outputs the header in its own line, then writes the specified data to the log file.
        /// If there is more than one object passed in, each piece will get its own line.
        /// </summary>
        /// <typeparam name="DataType"></typeparam>
        /// <typeparam name="HeaderType"></typeparam>
        /// <param name="level"></param>
        /// <param name="header"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task WriteAsync<DataType, HeaderType>(LogLevel level, HeaderType header, params DataType[] data)
        {
            string levelRepresentation = this.GetLogLevelPrefix(level);
            await this.WriteLineAsync(levelRepresentation, header);
            await this.WriteAsync(levelRepresentation, data);
        }

        /// <summary>
        /// Outputs the header in its own line, then writes the specified data to the log file.
        /// If there is more than one object passed in, each piece will get its own line.
        /// </summary>
        /// <typeparam name="DataType"></typeparam>
        /// <typeparam name="HeaderType"></typeparam>
        /// <param name="level"></param>
        /// <param name="header"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public void Write<DataType, HeaderType>(LogLevel level, HeaderType header, params DataType[] data)
        {
            string levelRepresentation = this.GetLogLevelPrefix(level);
            this.WriteLine(levelRepresentation, header);
            this.Write(levelRepresentation, data);
        }

        /// <summary>
        /// Writes a single piece of data to the log.
        /// </summary>
        /// <typeparam name="DataType">The type of the data that is being logged.</typeparam>
        /// <param name="level">The severity level of the logged data.</param>
        /// <param name="data">The piece of data to log.</param>
        /// <returns></returns>
        private async Task WriteLineAsync<DataType>(LogLevel level, DataType data)
        {
            string time = DateTime.UtcNow.ToString("[yyyy-MM-dd tt:HH:mm:ss.fff]");
            string logLevelPrefix = this.GetLogLevelPrefix(level);
            await this.Writer.WriteLineAsync(time + " [" + logLevelPrefix + "]: " + data);
            this.ConsoleWriteLine<DataType>(level, data);
        }

        /// <summary>
        /// Writes a single piece of data to the log.
        /// </summary>
        /// <typeparam name="DataType">The type of the data that is being logged.</typeparam>
        /// <param name="level">The severity level of the logged data.</param>
        /// <param name="data">The piece of data to log.</param>
        /// <returns></returns>
        private void WriteLine<DataType>(LogLevel level, DataType data)
        {
            string time = DateTime.UtcNow.ToString("[yyyy-MM-dd tt:HH:mm:ss.fff]");
            string logLevelPrefix = this.GetLogLevelPrefix(level);
            this.Writer.WriteLine(time + " [" + logLevelPrefix + "]: " + data);
            this.ConsoleWriteLine<DataType>(level, data);
        }

        /// <summary>
        /// Writes a line to the console with appropriate coloring.
        /// </summary>
        /// <typeparam name="DataType"></typeparam>
        /// <param name="level"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private void ConsoleWriteLine<DataType>(LogLevel level, DataType data)
        {
            lock (this.ConsoleSyncObject)
            {
                switch (level)
                {
                    case LogLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;

                    case LogLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;

                    case LogLevel.Information:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;

                    case LogLevel.Debug:
                    case LogLevel.None:
                        break;

                    default:
                        throw new UnexpectedMemberException("An unexpected enum member was found");
                }
                Console.WriteLine(data);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Writes a line to the console with appropriate coloring.
        /// </summary>
        /// <typeparam name="DataType"></typeparam>
        /// <param name="level"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private void ConsoleWriteLine<DataType>(string level, DataType data)
        {
            lock (this.ConsoleSyncObject)
            {
                switch (level)
                {
                    case ErrorPrefix:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;

                    case WarnPrefix:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;

                    case InfoPrefix:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;

                    case DebugPrefix:
                        break;

                    case NonePrefix:
                        break;

                    default:
                        throw new UnexpectedMemberException("An unexpected enum member was found");
                }

                Console.WriteLine(data);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Writes a single piece of data to the log.
        /// </summary>
        /// <typeparam name="DataType">The type of the data that is being logged.</typeparam>
        /// <param name="level">The severity level of the logged data in precomputed string form.</param>
        /// <param name="data">The piece of data to log.</param>
        /// <returns></returns>
        private async Task WriteLineAsync<DataType>(string level, DataType data)
        {
            if (data is IEnumerable enumerable && !(data is string))
            {
                foreach (object item in enumerable)
                {
                    await this.WriteLineAsync(level, item);
                }
            }
            else
            {
                string time = DateTime.UtcNow.ToString("[yyyy-MM-dd tt:HH:mm:ss.fff]");
                await this.Writer.WriteLineAsync(time + " [" + level + "]: " + data);
                await this.Writer.FlushAsync();
                this.ConsoleWriteLine<DataType>(level, data);
            }
        }

        /// <summary>
        /// Writes a single piece of data to the log.
        /// </summary>
        /// <typeparam name="DataType">The type of the data that is being logged.</typeparam>
        /// <param name="level">The severity level of the logged data in precomputed string form.</param>
        /// <param name="data">The piece of data to log.</param>
        /// <returns></returns>
        private void WriteLine<DataType>(string level, DataType data)
        {
            if (data is IEnumerable enumerable && !(data is string))
            {
                foreach (object item in enumerable)
                {
                    this.WriteLine(level, item);
                }
            }
            else
            {
                string time = DateTime.UtcNow.ToString("[yyyy-MM-dd tt:HH:mm:ss.fff]");
                this.Writer.WriteLine(time + " [" + level + "]: " + data);
                this.Writer.Flush();
                this.ConsoleWriteLine<DataType>(level, data);
            }
        }

        /// <summary>
        /// Writes information about a bunch of parameters stored within an anonymous type to the log.
        /// </summary>
        public async void DebugParams(object parameters)
        {
            PropertyInfo[] propertyInformations = parameters.GetType().GetProperties();
            await this.WriteAsync(LogLevel.Debug, propertyInformations);
        }

        /// <summary>
        /// Writes an exception
        /// </summary>
        /// <param name="e">The exception to write.</param>

        public async Task WriteExceptionAsync(Exception e, string msg)
        {
            string levelRepresentation = this.GetLogLevelPrefix(LogLevel.Error);
            await this.WriteAsync(levelRepresentation, msg);
            await this.WriteAsync(levelRepresentation, e.GetType().AssemblyQualifiedName + ":");
            await this.WriteAsync(levelRepresentation, "Help link: " + e.HelpLink);
            await this.WriteAsync(levelRepresentation, "Error code: " + e.HResult.ToString());
            await this.WriteAsync(levelRepresentation, "Message: " + e.Message);
            await this.WriteAsync(levelRepresentation, "Source: " + e.Source);
            await this.WriteAsync(levelRepresentation, "Stack trace: \r\n" + e.StackTrace);

            if (e.InnerException != null)
            {
                await this.WriteExceptionAsync(e.InnerException, "Inner exception: ");
            }

            await this.Writer.FlushAsync();
        }

        /// <summary>
        /// Writes an exception
        /// </summary>
        /// <param name="e">The exception to write.</param>
        public void WriteException(Exception e, string msg)
        {
            string levelRepresentation = this.GetLogLevelPrefix(LogLevel.Error);
            this.Write(levelRepresentation, msg);
            this.Write(levelRepresentation, e.GetType().AssemblyQualifiedName + ":");
            this.Write(levelRepresentation, "Help link: " + e.HelpLink);
            this.Write(levelRepresentation, "Error code: " + e.HResult.ToString());
            this.Write(levelRepresentation, "Message: " + e.Message);
            this.Write(levelRepresentation, "Source: " + e.Source);
            this.Write(levelRepresentation, "Stack trace: \r\n" + e.StackTrace);

            if (e.InnerException != null)
            {
                this.WriteException(e.InnerException, "Inner exception: ");
            }

            this.Writer.Flush();
        }

        /// <summary>
        /// Returns the log level prefix to prefix a message with.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private string GetLogLevelPrefix(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Error:
                    return ErrorPrefix;

                case LogLevel.Warning:
                    return WarnPrefix;

                case LogLevel.Information:
                    return InfoPrefix;

                case LogLevel.Debug:
                    return DebugPrefix;

                case LogLevel.None:
                    return NonePrefix;

                default:
                    throw new UnexpectedMemberException("An unexpected enum member was found");
            }
        }

        /// <summary>
        /// Closes this log gracefully, finishes writing data.
        /// </summary>
        /// <returns></returns>
        public async Task CloseAsync()
        {
            await this.Writer.FlushAsync();
            this.Writer.Close();
            this.Writer.Dispose();
        }

        public void Dispose()
        {
            this.Writer.Flush();
            this.Writer.Close();
            this.Writer.Dispose();
        }
    }
}