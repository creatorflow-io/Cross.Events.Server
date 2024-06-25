using Cross.AspNetCore.Authorization;
using Lib.Net.Http.WebPush;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace Cross.AspNetCore.WebPush.Controllers
{
    [Route("push-notifications-api")]
    [Authorize(Policies.Authenticated)]
    [ApiController]
    public class PushNotificationsApiController : ControllerBase
    {
        private readonly IPushNotificationService _notificationService;

        public PushNotificationsApiController(
            IPushNotificationService notificationService
            )
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Return the public key to be used for push notifications
        /// </summary>
        /// <returns></returns>
        [HttpGet("public-key")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ContentResult GetPublicKey()
        {
            return Content(_notificationService.PublicKey, "text/plain");
        }

		/// <summary>
        /// Subscribe a user to push notifications
        /// </summary>
        /// <param name="user"></param>
        /// <param name="subscription"></param>
        /// <param name="_subscriptionStore"></param>
        /// <returns></returns>
		[HttpPost("{user}/subscription")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> StoreSubscription(string user, 
            [FromBody] PushSubscription subscription, [FromServices] IPushSubscriptionStore _subscriptionStore)
        {
            try
            {
                await _subscriptionStore.StoreSubscriptionAsync(subscription, user);

                await _notificationService.SendNotificationAsync(subscription, new PushMessage("Welcome to the push notifications!"));
                return Ok();
            }
			catch(Exception ex)
            {
				return BadRequest(ex.Message);
			}
        }

        /// <summary>
        /// Discard a user subscription
        /// </summary>
        /// <param name="user"></param>
        /// <param name="endpoint"></param>
        /// <param name="_subscriptionStore"></param>
        /// <returns></returns>
        [HttpDelete("{user}/subscription")]
		[ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> DiscardSubscription(string user, string endpoint, [FromServices] IPushSubscriptionStore _subscriptionStore)
        {
            try
            {
                await _subscriptionStore.DiscardSubscriptionAsync(endpoint, user);
				return Ok();
			}
			catch(Exception ex)
            {
				return BadRequest(ex.Message);
			}
        }

        /// <summary>
        /// Check if a user subscription exists
        /// </summary>
        /// <param name="user"></param>
        /// <param name="subscription"></param>
        /// <param name="_subscriptionStore"></param>
        /// <returns></returns>
        [HttpGet("{user}/subscription")]
		[ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> CheckSubscription(string user, [FromBody] PushSubscription subscription, [FromServices] IPushSubscriptionStore _subscriptionStore)
        {
            if (string.IsNullOrEmpty(subscription?.Endpoint) || string.IsNullOrEmpty(user))
            {
                return NotFound("User or endpoint is null");
            }
            var exists = await _subscriptionStore.ExistsAsync(subscription.Endpoint, user);
            if (!exists)
            {
                return NotFound("Subscription not exists");
            }
            return Ok();
        }

    }
}
