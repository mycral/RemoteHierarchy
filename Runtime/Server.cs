// This work is licensed under the Creative Commons Attribution-ShareAlike 4.0 International License. 
// To view a copy of this license, visit http://creativecommons.org/licenses/by-sa/4.0/ 
// or send a letter to Creative Commons, PO Box 1866, Mountain View, CA 94042, USA.
using RemoteHierarchy.Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RemoteHierarchy
{
    public class Server : MonoBehaviour
    {
        #region private members 	
        /// <summary> 	
        /// TCPListener to listen for incomming TCP connection 	
        /// requests. 	
        /// </summary> 	
        private TcpListener tcpListener;
        /// <summary> 
        /// Background thread for TcpServer workload. 	
        /// </summary> 	
        private Thread tcpListenerThread;
        /// <summary> 	
        /// Create handle to connected tcp client. 	
        /// </summary> 	
        private TcpClient connectedTcpClient;
        #endregion

        private void Awake()
        {
            UnityMainThreadDispatcher _ = UnityMainThreadDispatcher.Instance;
        }

        void Start()
        {
            // Start TcpServer background thread 		
            tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
            tcpListenerThread.IsBackground = true;
            tcpListenerThread.Start();
        }

        private void OnDestroy()
        {
            if (tcpListener != null)
            {
                tcpListener.Stop();
            }
        }

        /// <summary> 	
        /// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
        /// </summary> 	
        private void ListenForIncommingRequests()
        {
            try
            {
                // Create listener on localhost port 8052. 			
                tcpListener = new TcpListener(IPAddress.Parse("0.0.0.0"), 8052);
                tcpListener.Start();
                Debug.Log("Server is listening");
                MemoryStream memoryStream = new MemoryStream();
                byte[] buff = new byte[1024];
                while (true)
                {
                    using (connectedTcpClient = tcpListener.AcceptTcpClient())
                    {
                        Utility.WaitExecOnMainThread(() =>
                        {
                            SendGameObjectTree();
                        });
                        // Get a stream object for reading 					
                        using (NetworkStream stream = connectedTcpClient.GetStream())
                        {
                            int length;
                            // Read incomming stream into byte arrary. 	
                            while ((length = stream.Read(buff, 0, buff.Length)) != 0)
                            {
                                memoryStream.Write(buff, 0, length);
                                if(memoryStream.Length > 4)
                                {
                                   int messageLength = BitConverter.ToInt32(memoryStream.GetBuffer());
                                    if(messageLength <= memoryStream.Length)
                                    {
                                        Utility.WaitExecOnMainThread(() =>
                                        {
                                            OnRevMsg(memoryStream, messageLength);
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                Debug.Log("SocketException " + socketException.ToString());
            }
        }


        private void OnRevMsg(MemoryStream ms,int msgLength)
        {
            byte[] buffer = ms.GetBuffer();
            //todo process
            int msgId = BitConverter.ToInt32(buffer, 4);

            try
            {
                switch(msgId)
                {
                    case MessageId.C2S_SetGameObjectActiveState:
                    {
                        OnMsgGameObjectActive(buffer,msgLength);
                        break;
                    }
                    case MessageId.C2S_GetGameObjectTree:
                    {
                        SendGameObjectTree();
                        break;
                    }
                    case MessageId.C2S_GetGameObjectDetail:
                    {
                        SendGameObjectDetail(buffer,msgLength);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            Array.Copy(buffer, msgLength, buffer, 0, ms.Position - msgLength);//����������
            ms.Position = ms.Position - msgLength;
        }

        private void OnMsgGameObjectActive(byte[] buff,int length)
        {
            var setGoActive = Utility.ResoveMessageBody<Proto.GameObjectActive>(buff, length);
            var gameObject = Utility.GetGameObjectFromInstanceId(setGoActive.InstanceId);
            if (gameObject != null)
            {
                gameObject.SetActive(setGoActive.IsActive);
            }
            else
            {
                Debug.LogError(($"没有找到实例{setGoActive.InstanceId}"));
            }
            SendGameObjectTree();
        }

        private void SendGameObjectTree()
        {
            var tree = Utility.BuildGameObjectTree();
            SendBytes(Utility.BuildMessageBytes(Proto.MessageId.S2C_GameObjectTree, tree));
        }
        
        private void SendGameObjectDetail(byte[] buff,int length)
        {
            var req = Utility.ResoveMessageBody<Proto.C2S_GetGameObjectDetail>(buff, length);
            var gameObject = Utility.GetGameObjectFromInstanceId(req.InstanceId);
            var detail = new Proto.S2C_ResponseGameObjectDetail();
            if (gameObject != null)
            {
                detail.JsonData = SerilizeJsonUtility.SerializeGameObject(gameObject).ToString(true);
            }
            else
            {
                Debug.LogError(($"没有找到实例{req.InstanceId}"));
            }
            SendBytes(Utility.BuildMessageBytes(Proto.MessageId.S2C_ResponseGameObjectDetail,detail));
        }
        

        /// <summary> 	
        /// Send message to client using socket connection. 	
        /// </summary> 	
        private void SendBytes(byte[] data)
        {
            if (connectedTcpClient == null)
            {
                return;
            }
            try
            {
                NetworkStream stream = connectedTcpClient.GetStream();
                if (stream.CanWrite)
                {
                    stream.Write(data, 0, data.Length);
                    Debug.Log($"Server Bytes[{data.Length}]");
                }
            }
            catch (SocketException socketException)
            {
                Debug.Log("Socket exception: " + socketException);
            }
        }
    }
}
