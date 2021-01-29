using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace ChatWebSocket
{
    public class UIManager : MonoBehaviour
    {
        public enum UIRootType
        {
            Connection,
            Chat
        }

        public GameObject uiRootConnection;
        public GameObject uiRootChat;
        public GameObject uiRootPopUp;

        public Button btnConnectToServer;
        public Button btnPopupOK;
        public Button btnSendMessage;

        public InputField inputFieldIP;
        public InputField inputFieldPort;
        public InputField inputMessage;

        public Text textPopUpMsg;
        public Text textReceiveMsg;

        private Dictionary<UIRootType, GameObject> uiRootDict = new Dictionary<UIRootType, GameObject>();

        private WebSocketConnection webSocket;

        private string receiveStr;

        // Start is called before the first frame update
        void Start()
        {
            webSocket = GetComponent<WebSocketConnection>();
            btnConnectToServer.onClick.AddListener(BTN_ConnectToServer);
            btnSendMessage.onClick.AddListener(BTN_SendMessage);
            btnPopupOK.onClick.AddListener(BTN_PopupOK);

            webSocket.OnConnectionSuccess += OnConnectionSuccess;
            webSocket.OnConnectionFail += OnConnectionFail;
            webSocket.OnReceive += OnReceiveMessage;

            uiRootDict.Add(UIRootType.Connection, uiRootConnection);
            uiRootDict.Add(UIRootType.Chat, uiRootChat);

            OpenUIRoot(UIRootType.Connection);
        }

        private void Update()
        {
            if(receiveStr != textReceiveMsg.text)
            {
                textReceiveMsg.text = receiveStr;
            }
        }

        void BTN_ConnectToServer()
        {
            webSocket.Connect(inputFieldIP.text, int.Parse(inputFieldPort.text));
        }

        void BTN_SendMessage()
        {
            if(!string.IsNullOrEmpty(inputMessage.text))
                webSocket.Send(inputMessage.text);
        }

        void BTN_PopupOK()
        {
            uiRootPopUp.SetActive(false);
        }

        public void ShowPopup(string msg)
        {
            uiRootPopUp.SetActive(true);
            textPopUpMsg.text = msg;
        }

        private void OpenUIRoot(UIRootType uiRootType)
        {
            foreach (var uiRoot in uiRootDict)
            {
                uiRoot.Value.SetActive(false);
            }

            uiRootDict[uiRootType].SetActive(true);
        }

        private void OnConnectionSuccess(string msg)
        {
            OpenUIRoot(UIRootType.Chat);
        }

        private void OnConnectionFail(string msg)
        {
            ShowPopup(msg);
        }

        private void OnReceiveMessage(string msg)
        {
            Debug.Log("OnReceive : " + msg);
            receiveStr += msg + "\n";
        }


    }

}
