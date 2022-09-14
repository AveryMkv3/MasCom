using Microsoft.Extensions.Logging;
using System;
using WebSocketSharper;
using WebSocketSharper.Server;

namespace MasCom.Serverv2.WebsocketServices
{
    public class GroupMessagingBehavior : WebSocketBehavior
    {
        protected override async void OnOpen()
        {
            if (!this.IsConnectionSecured())
            {
                Close(CloseStatusCode.TlsHandshakeFailure, "a secured connection is required");
                return;
            }

            //-- Get current user
            var user = await this.GetCurrentUser();

            if (user == null)
            {
                Close(CloseStatusCode.InvalidData, "Sould have a valid authenticated user");
                return;
            }
        }

        protected override async void OnMessage(MessageEventArgs e)
        {
            string requestedUserId = Context.QueryString["usersid"];

            if (string.IsNullOrWhiteSpace(requestedUserId))
            {
                Close(CloseStatusCode.InvalidData, "recipient userid not specified");
                return;
            }

            try
            {
                int uid = Convert.ToInt32(requestedUserId);
                var mapping = await this.GetSpecifiedUserSessionMappings(uid);

                if (mapping != null)
                {
                    if (e.IsText)
                    {
                        Sessions.SendTo(e.Data, mapping.GroupSid);
                    }
                    else if (e.IsBinary)
                    {
                        Sessions.SendTo(e.RawData, mapping.GroupSid);
                    }
                    else if (e.IsPing)
                    {
                        Sessions.PingTo(e.Data, mapping.GroupSid);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
        }



    }
}
