using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace wcf_chat
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IServiceChat" в коде и файле конфигурации.
    [ServiceContract(CallbackContract = typeof(IServerChatCallback))]
    public interface IServiceChat
    {
        [OperationContract]
        int Connect(string name);

        [OperationContract]
        void Disconnect(int id);

        [OperationContract(IsOneWay = true)]
        void SendMsg(string msg, int sender, string recipient);

        [OperationContract]
        List<string> GetUsersNames();

        [OperationContract]
        string GetNameById(int id);
        [OperationContract(IsOneWay = true)]
        void InstallChats();

        [OperationContract]
        int GetIdByName(string userName);

        [OperationContract(IsOneWay = true)]
        void SetDialog(int ID, string withUser);
        

    }

    public interface IServerChatCallback
    {
        [OperationContract(IsOneWay = true)]
        void MsgCallback(string msg, string sender, string recipient);

        [OperationContract(IsOneWay = true)]
        void GetChats(string userName);
        [OperationContract(IsOneWay = true)]
        void SetNewDialog(string withUser);

    }
}
