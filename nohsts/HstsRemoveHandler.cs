using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace nohsts
{
    public static class HstsRemoveHandler
    {
        private class MessageException : Exception
        {
            public MessageException(string message) : base(message)
            {
            }

            public MessageException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected MessageException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }


        public async static Task Delegate(HttpContext context)
        {
            try
            {

                if (context.Request.QueryString.ToString().Length <= 1)
                    throw new MessageException("Please provide URL in query string.");

                var urlText = context.Request.QueryString.ToString().Substring(1);

                var uri = urlText.Contains("://")
                    ? new Uri(urlText)
                    : new Uri($"http://{urlText}");

                var ip = await Dns.GetHostEntryAsync(uri.Host);

                if (!ip.AddressList.Any())
                    throw new MessageException($"Couldn't resolve name {uri.Host}.");

                var baseUri = new Uri($"http://{ip.AddressList.First()}:{(uri.IsDefaultPort ? "80" : uri.Port.ToString())}/");

                var newUri = new Uri(baseUri, uri.PathAndQuery);

                context.Response.Redirect(newUri.ToString());

            }
            catch(MessageException msg)
            {
                await context.Response.WriteAsync(msg.Message);
            }

        }

    }
}
