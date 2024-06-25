namespace Cross.AspNetCore.WebPush.Services
{
    public class PushNotificationServiceOptions
    {
        public string Subject { get; set; }

        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }
        public bool Enabled { get; set; } = true;
    }
}
