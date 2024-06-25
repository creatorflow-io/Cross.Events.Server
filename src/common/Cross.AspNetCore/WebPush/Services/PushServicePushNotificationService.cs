using Lib.Net.Http.WebPush.Authentication;
using Lib.Net.Http.WebPush;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cross.AspNetCore.WebPush.Services
{
    internal class PushServicePushNotificationService : IPushNotificationService
    {
        private readonly PushNotificationServiceOptions _options;
        private readonly PushServiceClient? _pushClient;

        private readonly ILogger _logger;

        public string PublicKey => _options.PublicKey;

        private bool _enabled;


        public PushServicePushNotificationService(IOptions<PushNotificationServiceOptions> optionsAccessor,
            IVapidTokenCache vapidTokenCache,
            PushServiceClient pushClient,
            ILogger<PushServicePushNotificationService> logger)
        {
            _options = optionsAccessor.Value;

            _enabled = _options.Enabled;

            if (_enabled)
            {
                try
                {
                    _pushClient = pushClient;
                    _pushClient.DefaultAuthentication = new VapidAuthentication(_options.PublicKey, _options.PrivateKey)
                    {
                        Subject = _options.Subject,
                        TokenCache = vapidTokenCache
                    };
                }
                catch (Exception ex)
                {
					_enabled = false;
                    logger.LogWarning("[PushNotificationService] failed to init. {0}", ex.Message);
                }
            }
            _logger = logger;
        }

        public async Task SendNotificationAsync(PushSubscription subscription, PushMessage message, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_enabled && _pushClient != null)
                    await _pushClient.RequestPushMessageDeliveryAsync(subscription, message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning("Failed requesting push message delivery to {0}. {1}", subscription.Endpoint, ex.Message + " " + (ex.InnerException?.Message ?? ""));
            }
        }
    }
}
