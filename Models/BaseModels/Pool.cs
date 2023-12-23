using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Pool
    {
        public const string markOperator = "Оцінка розмови";
        public const string complaint = "Скарга";
        public const string offer = "Пропозиція";
        public const string unknown = "Невідомо";

        public Guid Id { get; set; }
        public string ViberName { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public string Operator { get; set; }
        public DateTime DateCreate { get; set; }
        public ChatMessageTypes Type { get; set; }
        public string StringType
        {
            get
            {
                switch (Type)
                {
                    case ChatMessageTypes.MarkOperator:
                        return markOperator;
                    case ChatMessageTypes.Complaint:
                        return complaint;
                    case ChatMessageTypes.Offer:
                        return offer;
                    default:
                        return unknown;
                }
            }
        }
        public string Text { get; set; }
    }
}
