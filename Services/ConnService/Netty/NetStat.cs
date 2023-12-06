namespace ConnService.Netty;

public static class NetStat
    {
        private static long _receiveIncBytes;
        private static long _receiveTotalBytes;
        private static long _receiveIncCount;
        private static long _receiveTotalCount;
        private static long _sendIncBytes;
        private static long _sendTotalBytes;
        private static long _sendIncCount;
        private static long _sendTotalCount;

        public static void Start()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var time = DateTime.Now;
                    Thread.Sleep(1000);
                    var span = DateTime.Now - time;

                    ShowStat(span);
                }
            }, TaskCreationOptions.LongRunning);
        }

        public static void IncReceiveBytes(int bytes)
        {
            Interlocked.Add(ref _receiveIncBytes, bytes);
            Interlocked.Add(ref _receiveTotalBytes, bytes);
        }

        public static void IncReceiveCount(int count)
        {
            Interlocked.Add(ref _receiveIncCount, count);
            Interlocked.Add(ref _receiveTotalCount, count);
        }

        public static void IncSendBytes(int bytes)
        {
            Interlocked.Add(ref _sendIncBytes, bytes);
            Interlocked.Add(ref _sendTotalBytes, bytes);
        }

        public static void IncSendCount(int count)
        {
            Interlocked.Add(ref _sendIncCount, count);
            Interlocked.Add(ref _sendTotalCount, count);
        }

        private static void ShowStat(TimeSpan span)
        {
            var seconds = span.TotalSeconds;
            var recvBytesPerSec = Math.Floor(Interlocked.Exchange(ref _receiveIncBytes, 0) / seconds);
            var recvTotalBytes = _receiveTotalBytes;

            var recvCountPerSec = Math.Floor(Interlocked.Exchange(ref _receiveIncCount, 0) / seconds);
            var recvTotalCount = _receiveTotalCount;

            var sendBytesPerSec = Math.Floor(Interlocked.Exchange(ref _sendIncBytes, 0) / seconds);
            var sendTotalBytes = _sendTotalBytes;

            var sendCountPerSec = Math.Floor(Interlocked.Exchange(ref _sendIncCount, 0) / seconds);
            var sendTotalCount = _sendTotalCount;

            Console.WriteLine($"{DateTime.Now} - " +
                              $"R Count: {recvCountPerSec} /S | " +
                              $"S Count: {sendCountPerSec} /S | " +
                              $"R bytes: {GetFileSize(recvBytesPerSec)}/S | " +
                              $"S bytes: {GetFileSize(sendBytesPerSec)}/S | " +
                              $"R Total Count: {recvTotalCount} | " +
                              $"R Total Bytes: {GetFileSize(recvTotalBytes)}  | " +
                              $"S Total Count {sendTotalCount} | " +
                              $"S Total Bytes: {GetFileSize(sendTotalBytes)} | " +
                              $"ThreadPool: {ThreadPool.ThreadCount}");
        }

        private static string GetFileSize(double filesize)
        {
            if (filesize < 0)
            {
                return "0";
            }

            if (filesize >= 1024 * 1024 * 1024) //文件大小大于或等于1024MB
            {
                return $"{filesize / (1024 * 1024 * 1024):0.00} GB";
            }

            if (filesize >= 1024 * 1024) //文件大小大于或等于1024KB
            {
                return $"{filesize / (1024 * 1024):0.00} MB";
            }

            if (filesize >= 1024) //文件大小大于等于1024bytes
            {
                return $"{filesize / 1024:0.00} KB";
            }

            return $"{filesize:0.00} bytes";
        }
    }