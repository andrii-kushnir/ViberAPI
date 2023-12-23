using System;

namespace Models
{
    public class ChatMessage
    {
        public Guid MessageId { get; set; }
        public long Token { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateDelivered { get; set; }
        public DateTime DateSeen { get; set; }
        public string Text { get; set; }
        public Guid Owner { get; set; }
        public Guid Receiver { get; set; }
        public string OwnerName { get; set; }
        public ChatMessageTypes ChatMessageType { get; set; }
    }

    public enum ChatMessageTypes
    {
        Unknown = 0,
        MessageToViber = 1,
        MessageFromViber = 2,
        FindOperator = 3,
        MarkOperator = 4,
        Menu = 5,
        OperatorConnect = 6,
        Fix = 7,
        Complaint = 8,
        Offer = 9,
        Advert = 10,
        LinkAsImageToViber = 11,
        ImageToViber = 12,
        ImageFromViber = 13,
        VideoToViber = 14,
        VideoFromViber = 15,
        FileToViber = 16,
        FileFromViber = 17
    }

    public static class ChatMessageTypesExpansion
    {
        public static bool MsgOperatorToViber(this ChatMessageTypes messageTypes)
        {
            switch (messageTypes)
            {
                case ChatMessageTypes.MessageToViber:
                case ChatMessageTypes.LinkAsImageToViber:
                case ChatMessageTypes.ImageToViber:
                case ChatMessageTypes.VideoToViber:
                case ChatMessageTypes.FileToViber:
                    return true;
                default:
                    return false;
            }
        }

        public static bool MsgSendToOperator(this ChatMessageTypes messageTypes)
        {
            switch (messageTypes)
            {
                case ChatMessageTypes.MessageToViber:
                case ChatMessageTypes.MessageFromViber:
                case ChatMessageTypes.Menu:
                case ChatMessageTypes.OperatorConnect:
                case ChatMessageTypes.Fix:
                case ChatMessageTypes.LinkAsImageToViber:
                case ChatMessageTypes.ImageToViber:
                case ChatMessageTypes.ImageFromViber:
                case ChatMessageTypes.VideoToViber:
                case ChatMessageTypes.VideoFromViber:
                case ChatMessageTypes.FileToViber:
                case ChatMessageTypes.FileFromViber:
                    return true;
                case ChatMessageTypes.FindOperator:
                case ChatMessageTypes.MarkOperator:
                case ChatMessageTypes.Complaint:
                case ChatMessageTypes.Offer:
                    return false;
                default:
                    return true;
            }
        }

        public static bool MsgServices(this ChatMessageTypes messageTypes)
        {
            switch (messageTypes)
            {
                case ChatMessageTypes.Menu:
                case ChatMessageTypes.OperatorConnect:
                case ChatMessageTypes.Fix:
                    return true;
                case ChatMessageTypes.FindOperator: // not send to operator
                    return true;
                default:
                    return false;
            }
        }

        public static bool MsgPool(this ChatMessageTypes messageTypes)
        {
            switch (messageTypes)
            {
                case ChatMessageTypes.MarkOperator:
                case ChatMessageTypes.Complaint:
                case ChatMessageTypes.Offer:
                    return true;
                default:
                    return false;
            }
        }
    }
}