namespace Com.ChinaSoft.HistorianService.Models
{
    public class HistorianConnection
    {
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public ushort TcpPort { get; set; }
        public uint MinSFDuration { get; set; }
        public uint FreeDiskSpace { get; set; }
        public string StoreForwardPath { get; set; }
    }
}