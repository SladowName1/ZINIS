using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;


namespace wcf_chat
{
  
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceChat : IServiceChat
    {
        List<ServerUser> users = new List<ServerUser>();
        int nextId = 1;
        Serpent serpent;

        public int Connect(string name,string key)
        {
            serpent = new Serpent();
            ServerUser user = new ServerUser() {
                Key = key,
                ID = nextId,
                Name = name,
                operationContext = OperationContext.Current
            };
            nextId++;

            SendMsg(": "+user.Name+" Connect!",0);
            users.Add(user);
            return user.ID;
        }

        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(i => i.ID == id);
            if (user!=null)
            {
                users.Remove(user);
                SendMsg(": "+user.Name + " disconnect",0);
            }
        }

        public void SendMsg(string msg, int id)
        {
            foreach (var item in users)
            {
                string answer = DateTime.Now.ToShortTimeString();
                string keyForDecrypt;

                var user = users.FirstOrDefault(i => i.ID == id);
                if (user != null)
                {
                    answer += ": " + user.Name+" ";
                    keyForDecrypt = msg.Substring(msg.Length-44);
                    msg = msg.Remove(msg.Length - 44);
                    msg = serpent.Encrypt(serpent.Decrypt(msg, keyForDecrypt),Convert.FromBase64String(item.Key));
                    msg = Converts.BinaryToString(msg);
                    msg += item.Key;
                }
                answer +=msg;
                item.operationContext.GetCallbackChannel<IServerChatCallback>().MsgCallback(answer);
            }
        }
    }
}
