using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Security.Cryptography;
using System.Net;

namespace SeniorDesign.Controllers
{
	public class ValidateRequestAttribute : ActionFilterAttribute
	{
		public string AuthToken { get; set; }

		public ValidateRequestAttribute(string authToken)
		{
			AuthToken = authToken;
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var context = filterContext.HttpContext;
			if (context.Request.IsLocal)
			{
				return;
			}

			// validate request
			// http://www.twilio.com/docs/security-reliability/security
			// Take the full URL of the request, from the protocol (http...) through the end of the query string (everything after the ?)
			var value = new StringBuilder();
			// could use Request.RawUrl outside of load-balanced/proxied scenarios
			var fullUrl = string.Format("http://{0}{1}", context.Request.Url.Host, context.Request.Url.PathAndQuery);

			value.Append(fullUrl);

			// If the request is a POST, take all of the POST parameters and sort them alphabetically.
			if (context.Request.HttpMethod == "POST")
			{
				// Iterate through that sorted list of POST parameters, and append the variable name and value (with no delimiters) to the end of the URL string
				var sortedKeys = context.Request.Form.AllKeys.OrderBy(k => k, StringComparer.Ordinal).ToList();
				foreach (var key in sortedKeys)
				{
					value.Append(key);
					value.Append(context.Request.Form[key]);
				}
			}

			// Sign the resulting value with HMAC-SHA1 using your AuthToken as the key (remember, your AuthToken's case matters!).
			var sha1 = new HMACSHA1(Encoding.UTF8.GetBytes(AuthToken));
			var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(value.ToString()));

			// Base64 encode the hash
			var encoded = Convert.ToBase64String(hash);

			// Compare your hash to ours, submitted in the X-Twilio-Signature header. If they match, then you're good to go.
			var sig = context.Request.Headers["X-Twilio-Signature"];

			var invalid = sig != encoded;

			if (invalid)
			{
				filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
				filterContext.HttpContext.Response.SuppressContent = true;
				filterContext.HttpContext.ApplicationInstance.CompleteRequest();
			}

			base.OnActionExecuting(filterContext);
		}
	}
}