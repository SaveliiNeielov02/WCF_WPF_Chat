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

        public int Connect(string name)
        {

            ServerUser user = new ServerUser() {
                ID = nextId,
                Name = name,
                operationContext = OperationContext.Current
            };
            nextId++;

            users.Add(user);
            return user.ID;
        }

        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(i => i.ID == id);
            if (user != null)
            {
                users.Remove(user);
                nextId--;
            }
        }

        public void SendMsg(string msg, int sender, string recipient)
        {
            
                string answer = DateTime.Now.ToShortTimeString();
                var user = users.FirstOrDefault(_ => _.ID == sender);
                if (user != null)
                {
                    answer += ": [" + user.Name+"] ";
                }
                answer += msg;
           
                for (int i = 0; i < users.Count(); i++)
                {
                    if (users[i].ID == sender)
                    {
                        users[i].operationContext.GetCallbackChannel<IServerChatCallback>().MsgCallback(answer, users[i].Name, recipient);
                    }
                    else if (users[i].Name == recipient)
                    {
                        users[i].operationContext.GetCallbackChannel<IServerChatCallback>().MsgCallback(answer, user.Name, recipient);
                    }
                }
            }
            
        public List<string> GetUsersNames()
        {
            return users.Select(_ =>_.Name).ToList();
        }
        public string GetNameById(int id) 
        {
            foreach (var user in users) 
            {
                if (user.ID == id) return user.Name;
            }
            return null;
        }
        public void InstallChats() 
        {
            for (int i = 0; i < users.Count(); i++) 
            {
                for (int k = 0; k < users.Count(); k++)
                {
                    if (i != k)
                    {
                        users[i].operationContext.GetCallbackChannel<IServerChatCallback>().GetChats(users[k].Name);
                    }
                }
            }
             
            
        }
        
        public int GetIdByName(string userName) 
        {
            return users[users.Select(_ => _.Name).ToList().IndexOf(userName)].ID;
        }
        public void SetDialog(int ID, string withUser) 
        {
          users[ID - 1].operationContext.GetCallbackChannel<IServerChatCallback>().SetNewDialog(withUser);
        }
    }
}
