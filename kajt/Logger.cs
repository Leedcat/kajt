using System.Collections.Concurrent;

namespace kajt
{
    public class Logger
    {
        public static Logger.Level globalLevel = Logger.Level.INFO;

        private static BlockingCollection<Logger.Message> collection = new BlockingCollection<Message>();

        // Create a thread that collects all the messages in the queue and displayes them
        public static void Init()
        {
            Task.Factory.StartNew(() =>
            {
                foreach (Logger.Message message in Logger.collection.GetConsumingEnumerable())
                {
                    if (message.type < Logger.globalLevel) { continue; }

                    Console.WriteLine(message);
                }
            });
        }

        // Add a message the to the queue
        public static void Log(string name, Logger.Level level, string message)
        {
            Logger.collection.Add(new Logger.Message(level, message, name));
        }

        public static string LevelString(Logger.Level level)
        {
            switch (level)
            {
                case Logger.Level.DEBUG:
                    return "DEBUG";

                case Logger.Level.INFO:
                    return "INFO";

                case Logger.Level.WARNING:
                    return "WARNING";

                case Logger.Level.ERROR:
                    return "ERROR";

                default:
                    return "UNKNOWN";
            }
        }

        public readonly string name;

        public Logger(string name)
        {
            this.name = name;
        }

        public void Debug(string message)
        {
            Logger.Log(this.name, Logger.Level.DEBUG, message);
        }

        public void Info(string message)
        {
            Logger.Log(this.name, Logger.Level.INFO, message);
        }
        public void Warning(string message)
        {
            Logger.Log(this.name, Logger.Level.WARNING, message);
        }
        public void Error(string message)
        {
            Logger.Log(this.name, Logger.Level.ERROR, message);
        }

        public enum Level
        {
            DEBUG = 0,
            INFO = 1,
            WARNING = 2,
            ERROR = 3,
        }

        private struct Message
        {
            public readonly Logger.Level type;
            public readonly string typeString;
            public readonly string timeString;
            public readonly string content;
            public readonly string caller;

            public Message(Logger.Level type, string content, string caller)
            {
                this.type = type;
                this.typeString = Logger.LevelString(this.type);
                this.content = content;
                this.caller = caller;

                DateTime now = DateTime.Now;
                this.timeString = now.ToString("HH:mm:ss");
            }

            public override string ToString()
            {
                return String.Format("{0} [{1}] {2}: {3}",
                        this.timeString,
                        this.caller,
                        this.typeString,
                        this.content
                        );
            }
        }
    }
}