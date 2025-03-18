using Newtonsoft.Json;
using PromAPI.Models;
using PromAPI.ModelsProm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromAPI
{
    public class ApiManager
    {
        public const string apiPath = "https://my.prom.ua/api/v1/";

        private string _token = "38f001f7fe7a9916dec88bddb3f1b3dac7db1e96";

        public static ApiManager Current { get; private set; }

        public ApiManager()
        {
            if (Current == null)
                Current = this;
        }

        public List<Message> GetChatMessages(string room_ident, out string error)
        {
#warning Бажано переробити цей метод на асинхрониий, тому що Пром інколи довго не відповідає і ложить весь сервер.
            List<Message> result = new List<Message>();
            string response;
            try
            {
                //response = RequestData.SendGet($"{apiPath}chat/messages_history?status=new&date_from=2023-10-14T00:00:00&limit=100", _token, out error);
                if (room_ident == null)
                    response = RequestData.SendGet($"{apiPath}chat/messages_history?status=new&sort=desc", _token, out error);
                else
                    response = RequestData.SendGet($"{apiPath}chat/messages_history?room_ident={room_ident}&limit=100&sort=desc", _token, out error);
            }
            catch (Exception ex)
            {
                error = ex.Message + " ";
                return result;
            }
            var resultModel = response.ConvertJson<MessagesHistory>(ref error);
            if (resultModel != null && resultModel.status == "ok")
            {
                result = resultModel.data.messages;
            }
            else
            {
                var resultError = response.ConvertJson<ErrorMessage>(ref error);
                if (resultError != null && resultError.status == "error")
                    error = resultError.message;
            }
            return result;
        }

        public int SendMessage(string room_ident, string user_ident, string text, out string error)
        {
            int result = 0;
            string keysBody;

            var body = new SendOurMessage()
            {
                room_ident = room_ident,
                user_id = user_ident,
                body = text
            };
            keysBody = JsonConvert.SerializeObject(body, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            //if (user_ident == null)
            //    keysBody = "{\"room_ident\":\"" + room_ident + "\", \"body\":\"" + text + "\"}";
            //else
            //    keysBody = "{\"room_ident\":\"" + room_ident + "\", \"user_id\":" + user_ident + ", \"body\":\"" + text + "\"}";

            string response;
            try
            {
                response = RequestData.SendPost(apiPath + "chat/send_message", _token, keysBody, out error);
            }
            catch (Exception ex)
            {
                error = ex.Message + " ";
                return result;
            }
            var resultModel = response.ConvertJson<SendMessage>(ref error);
            if (resultModel != null && resultModel.status == "ok")
            {
                result = resultModel.data.message_id;
            }
            else
            {
                var resultError = response.ConvertJson<ErrorMessage>(ref error);
                if (resultError != null && resultError.status == "error")
                    error += " " + resultError.message;
            }
            return result;
        }

        public bool MarkMessageRead(int messageId, string room_id, out string error)
        {
            var keysBody = "{\"message_id\":" + messageId + ", \"room_id\":\"" + room_id + "\"}";

            string response;
            try
            {
                response = RequestData.SendPost(apiPath + "chat/mark_message_read", _token, keysBody, out error);
            }
            catch (Exception ex)
            {
                error = ex.Message + " ";
                return false;
            }
            var resultModel = response.ConvertJson<MarkMessageRead>(ref error);
            if (resultModel != null && resultModel.status == "ok")
                return true;
            var resultError = response.ConvertJson<ErrorMessage>(ref error);
            if (resultError != null && resultError.status == "error")
                error += " " + resultError.message;
            return false;
        }

        public Product GetProduct(long id, out string error)
        {
            Product result = null;
            string response;
            try
            {
                response = RequestData.SendGet(apiPath + $"products/{id}", _token, out error);
            }
            catch (Exception ex)
            {
                error = ex.Message + " ";
                return result;
            }

            var resultModel = response.ConvertJson<GetProduct>(ref error);
            if (resultModel != null)
            {
                result = resultModel.product;
            }
            else
            {
                var resultError = response.ConvertJson<ErrorMessage>(ref error);
                if (resultError != null && resultError.status == "error")
                    error += " " + resultError.message;
            }
            return result;
        }
    }
}
