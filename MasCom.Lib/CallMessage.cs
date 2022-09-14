using System;
using System.Collections.Generic;
using System.Text;

namespace MasCom.Lib
{
    public class CallMessage: MessageBase
    {
        public new MessageType MessageType => MessageType.Call;
        public CallResponse CallResponse { get; set; } = CallResponse.Rejected;
    }

    public enum CallResponse
    {
        Accepted,
        Rejected,
        Init
    }
}
