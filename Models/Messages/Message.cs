using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages
{
    public interface IMessage
    {
        MessageTypes MessageType { get; set; }
    }

    public class Message : IMessage
    {
        public MessageTypes MessageType { get; set; }

        public Message()
        {
            MessageType = MessageTypes.Unknown;
        }

        public Message(MessageTypes messageType)
        {
            MessageType = messageType;
        }
    }

    public enum MessageTypes
    {
        Unknown = 0,
        PingRequest = 1,
        PingResponse = 2,

        LoginRequest = 10,
        LoginResponse = 11,
        ReConnectRequest = 12,
        LogoutRequest = 13,

        AwayRequest = 20,
        ReturnAwayRequest = 21,

        UserListRequest = 30,
        UserListResponse = 31,

        UserDetailsRequest = 50,
        UserDetailsResponse = 51,
        ChangeTypeRequest = 52,
        AttachOperatorRequest = 53,

        NewUserRequest = 60,
        NewConversationRequest = 61,

        FindUserRequest = 70,
        FindUserResponse = 71,

        MessageViberPhone = 80,

        FindOperatorRequest = 90,
        FindOperatorResponse = 91,
        ClientBusyRequest = 92,
        ChangeOperatorRequest = 93,

        MessageToViberRequest = 100,
        MessageToViberResponse = 101,
        MessageDeliveredRequest = 102,
        MessageSeenRequest = 103,
        FileToViberRequest = 104,
        FileToViberResponse = 105,
        ImageToViberRequest = 106,
        ImageToViberResponse = 107,

        MessageFromViberRequest = 110,
        FileFromViberRequest = 111,

        FixMessageRequest = 120,

        ArseniumOnlineRequest = 130,
        ArseniumOfflineRequest = 131,

        PoolsListRequest = 140,
        PoolsListResponse = 141,

        UserDetailsBuhnetRequest = 150,
        UserDetailsBuhnetResponse = 151,

        NewMessageRozetkaRequest = 160,
        MessageToRozetkaRequest = 161,

        NewMessagePromRequest = 170,
        MessageToPromRequest = 171,

        ReadHotRequest = 180
    }
}
