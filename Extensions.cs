namespace OrderApi
{
    public static class Extensions
    {
        public const int LineLength = 151;

        public static void LogLine(this Serilog.ILogger logger)
            => logger.Information(new string('-', LineLength));

        public static void LogLineSpace(this Serilog.ILogger logger)
            => logger.Information(new string('-', LineLength) + "\n\n\n\n\n\n");

        public static void LogLine(this Serilog.ILogger logger, string title)
        {
            var titleLengthHalf = (title.Length + 2) / 2;
            logger.Information($"{new string('-', (LineLength/2)-titleLengthHalf)} {title} {new string('-', (LineLength/2)-titleLengthHalf)}");
        }
        public static void LogTitle(this Serilog.ILogger logger, string title)
        {
            var titleLengthHalf = (title.Length + 2) / 2;
            logger.Information($"{new string(' ', (LineLength / 2) - titleLengthHalf)} {title}");
        }
    }
}
