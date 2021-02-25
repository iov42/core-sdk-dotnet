namespace Iov42sdk.Models
{
    public class Credentials
    {
        // ReSharper disable once UnusedMember.Global
        public Credentials()
        {
        }

        public Credentials(string key, string protocolId)
        {
            Key = key;
            ProtocolId = protocolId;
        }

        public string Key { get; set; }
        public string ProtocolId { get; set; }
    }
}