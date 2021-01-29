using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatDotNet
{
    public class Connection : MonoBehaviour
    {
        public enum ConnectionState
        {
            Disconnect,
            Connect
        }

        public enum ConnectionRole
        {
            None,
            Server,
            Client
        }

        private Socket socket;
        private int byteSize = 32;
        private byte[] buffer;
        AsyncCallback callback;
        EndPoint endPoint;

        public struct ClientData
        {
            public Socket socket;
            public string groupID;
        }

        private List<Socket> clientSocketList = new List<Socket>();

        private ConnectionState connectionState;
        private ConnectionRole connectionRole;

        public static Connection instance;

        public delegate void DelegateHandleCallback(string msg);

        public event DelegateHandleCallback OnCreateServerSuccess;
        public event DelegateHandleCallback OnCreateServerFail;

        public event DelegateHandleCallback OnJoinRoomSuccess;
        public event DelegateHandleCallback OnJoinRoomFail;

        private WaitForSeconds waitConnecting = new WaitForSeconds(1.0f);
        private bool isJoiningRoom = false;

        public ConnectionState GetConnectionState()
        {
            return connectionState;
        }

        public ConnectionRole GetConnectionRole()
        {
            return connectionRole;
        }

        public void Awake()
        {
            instance = this;

            OnCreateServerSuccess += CallbackCreateServerSuccess;
        }

        public void StartServer(int port)
        {
            buffer = new byte[byteSize];
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            endPoint = new IPEndPoint(IPAddress.Any, port);

            try
            {
                socket.Bind(endPoint);

                if (OnCreateServerSuccess != null)
                {
                    OnCreateServerSuccess("create success");
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);

                if (OnCreateServerFail != null)
                {
                    OnCreateServerFail(e.Message);
                }
            }
        }

        public void CallbackCreateServerSuccess(string msg)
        {
            callback = Callback;

            socket.BeginReceiveFrom(buffer, 0, byteSize, SocketFlags.None, ref endPoint, callback, socket);

            connectionState = ConnectionState.Connect;

            connectionRole = ConnectionRole.Server;
        }

        public void JoinServer(string ip, int port)
        {
            if (isJoiningRoom == true)
                return;

            isJoiningRoom = true;

            try
            {
                IPAddress ipAddr = IPAddress.Parse(ip);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                endPoint = new IPEndPoint(ipAddr, port);
                socket.Connect(endPoint);
            }
            catch (Exception e)
            {
                if (OnJoinRoomFail != null)
                    OnJoinRoomFail(e.Message);

                isJoiningRoom = false;
                return;
            }

            socket.BeginReceiveFrom(buffer, 0, byteSize, SocketFlags.None, ref endPoint, callback, socket);

            StartCoroutine(IEJoinRoomWaitCheckingConnnection());
        }

        private IEnumerator IEJoinRoomWaitCheckingConnnection()
        {
            yield return waitConnecting;

            if (socket.Connected)
            {
                connectionState = ConnectionState.Connect;

                connectionRole = ConnectionRole.Client;

                if (OnJoinRoomSuccess != null)
                    OnJoinRoomSuccess("Join room success");
            }
            else
            {
                if (OnJoinRoomFail != null)
                    OnJoinRoomFail("Join room fail");
            }

            isJoiningRoom = false;
        }

        public void SendData(string dataStr)
        {
            if (connectionState == ConnectionState.Connect
                && socket.Connected)
            {
                byte[] byteSend = Encoding.ASCII.GetBytes(dataStr);
                socket.Send(byteSend);
            }
        }

        public void Disconnect()
        {
            if (socket != null)
                socket.Close();

            connectionState = ConnectionState.Disconnect;
            connectionRole = ConnectionRole.None;
        }

        private void OnDestroy()
        {
            Disconnect();
            Debug.Log("Connection Close");
        }

        void Callback(IAsyncResult result)
        {
            if (result == null)
                return;

            Socket recvSocket = (Socket)result.AsyncState;
            EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

            if (connectionRole == ConnectionRole.Server &&
                clientSocketList.Contains(recvSocket) == false)
            {
                clientSocketList.Add(recvSocket);
            }

            int msgLen = recvSocket.EndReceiveFrom(result, ref clientEndPoint);

            byte[] localMsg = new byte[msgLen];
            Array.Copy(buffer, localMsg, msgLen);
            string msgStr = Encoding.ASCII.GetString(localMsg);

            socket.BeginReceiveFrom(buffer, 0, byteSize, SocketFlags.None, ref endPoint, callback, socket);
        }
    }
}


