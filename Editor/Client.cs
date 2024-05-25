// This work is licensed under the Creative Commons Attribution-ShareAlike 4.0 International License. 
// To view a copy of this license, visit http://creativecommons.org/licenses/by-sa/4.0/ 
// or send a letter to Creative Commons, PO Box 1866, Mountain View, CA 94042, USA.
using RemoteHierarchy.Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
namespace RemoteHierarchy
{
    public class Client
    {
        #region private members 	
        private TcpClient socketConnection;
        private Thread clientReceiveThread;
        private string m_kHost = "localhost";
        #endregion
        
        public event Action<S2C_GameObjectTree> OnEvtNewGameObjectTree;
        public event Action<S2C_ResponseGameObjectDetail> OnEvtResponseGameObjectDetail;

        // Use this for initialization 	
        public  Client()
        {
            var _ = EditorUnityMainThreadDispatcher.Instance;
            //ConnectToTcpServer();
        }
        /// <summary> 	
        /// Setup socket connection. 	
        /// </summary> 	
        public void ConnectToTcpServer(string host)
        {
            try
            {
                m_kHost = host;
                clientReceiveThread = new Thread(new ThreadStart(ListenForData));
                clientReceiveThread.IsBackground = true;
                clientReceiveThread.Start();
            }
            catch (Exception e)
            {
                Debug.Log("On client connect exception " + e);
            }
        }

        public bool IsConnected()
        {
            if (socketConnection == null)
                return false;
            return socketConnection.Connected;
        }

        public void StopConnect()
        {
            if (socketConnection == null)
                return;
            if(socketConnection.Connected)
                socketConnection.Close();
            socketConnection = null;
        }

        /// <summary> 	
        /// Runs in background clientReceiveThread; Listens for incomming data. 	
        /// </summary>     
        private void ListenForData()
        {
            try
            {
                socketConnection = new TcpClient(m_kHost, 8052);
                MemoryStream memoryStream = new MemoryStream();
                byte[] buff = new byte[1024];
                while (true)
                {
                    // Get a stream object for reading 					
                    using (NetworkStream stream = socketConnection.GetStream())
                    {
                        int length;
                        // Read incomming stream into byte arrary. 	
                        while ((length = stream.Read(buff, 0, buff.Length)) != 0)
                        {
                            memoryStream.Write(buff, 0, length);
                            if (memoryStream.Length > 4)
                            {
                                int messageLength = BitConverter.ToInt32(memoryStream.GetBuffer());
                                if (messageLength <= memoryStream.Length)
                                {
                                    EditorUnityMainThreadDispatcher.WaitExecOnMainThread(() =>
                                    {
                                        OnRevMsg(memoryStream, messageLength);
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                Debug.LogError("Socket exception: " + socketException);
                socketConnection = null;
            }
        }

        private void OnRevMsg(MemoryStream ms, int msgLength)
        {
            byte[] buffer = ms.GetBuffer();
            //todo process
            int msgId = BitConverter.ToInt32(buffer, 4);

            switch (msgId)
            {
                case MessageId.S2C_GameObjectTree:
                    {
                        OnMsgGameObjectTree(buffer, msgLength);
                        break;
                    }
                case MessageId.S2C_ResponseGameObjectDetail:
                {
                    OnMsgResponseGameObjectDetail(buffer, msgLength);
                    break;
                }
            }
            Array.Copy(buffer, msgLength, buffer, 0, ms.Position - msgLength);//可能有隐患
            ms.Position = ms.Position - msgLength;
        }
        private void OnMsgGameObjectTree(byte[] buff, int length)
        {
            var tree = Utility.ResoveMessageBody<Proto.S2C_GameObjectTree>(buff, length);
            if (OnEvtNewGameObjectTree != null)
                OnEvtNewGameObjectTree(tree);
            Debug.Log(JsonUtility.ToJson(tree));
        }
        private void OnMsgResponseGameObjectDetail(byte[] buff, int length)
        {
            var detail = Utility.ResoveMessageBody<Proto.S2C_ResponseGameObjectDetail>(buff, length);
            if (OnEvtResponseGameObjectDetail != null)
                OnEvtResponseGameObjectDetail(detail);
            Debug.Log(JsonUtility.ToJson(detail));
        }

        public void SendGetGameObjectList()
        {
            SendBytes(Utility.BuildMessageBytes(Proto.MessageId.C2S_GetGameObjectTree));
        }

        public void SendSetGameObjectActive(int instanceId,bool active)
        {
            SendBytes(Utility.BuildMessageBytes(Proto.MessageId.C2S_SetGameObjectActiveState, new Proto.GameObjectActive() { InstanceId = instanceId,IsActive = active}));
        }
        public void SendGetGameObjectDetail(int instanceId)
        {
            SendBytes(Utility.BuildMessageBytes(Proto.MessageId.C2S_GetGameObjectDetail, new Proto.C2S_GetGameObjectDetail() { InstanceId = instanceId}));
        }

        /// <summary> 	
        /// Send message to server using socket connection. 	
        /// </summary> 	
        private void SendBytes(byte[] data)
        {
            if (socketConnection == null)
            {
                return;
            }
            try
            {
                NetworkStream stream = socketConnection.GetStream();
                if (stream.CanWrite)
                {
                    stream.Write(data, 0, data.Length);
                    Debug.Log($"Client Bytes[{data.Length}]");
                }
            }
            catch (SocketException socketException)
            {
                Debug.Log("Socket exception: " + socketException);
            }
        }
    }
}