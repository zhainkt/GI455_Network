using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{    public enum UIRootType
    {
        SelectRole,
        CreateServer,
        JoinRoom,
        MainChat
    }

    public GameObject uiRootSelectRole;
    public GameObject uiRootServer;
    public GameObject uiRootClient;
    public GameObject uiRootMain;

    public GameObject uiRootPopUp;

    public Button btnSelectServer, btnSelectClient;

    public Button btnCreateServer;
    public Button btnJoinClient;

    public Button btnPopupOK;

    public InputField inputFieldServerPort;

    public InputField inputFieldClientIP;
    public InputField inputFieldClientPort;

    public Text textPopUpMsg;

    private Dictionary<UIRootType, GameObject> uiRootDict = new Dictionary<UIRootType, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        btnSelectClient.onClick.AddListener(BTN_SelectClient);
        btnSelectServer.onClick.AddListener(BTN_SelectServer);
        btnCreateServer.onClick.AddListener(BTN_CreateServer);
        btnJoinClient.onClick.AddListener(BTN_JoinClient);
        btnPopupOK.onClick.AddListener(BTN_PopupOK);

        Connection.instance.OnCreateServerFail += ShowPopup;
        Connection.instance.OnCreateServerSuccess += OnCreateServerSuccess;
        Connection.instance.OnJoinRoomSuccess += OnJoinRoomSuccess;
        Connection.instance.OnJoinRoomFail += OnJoinRoomFail;

        uiRootDict.Add(UIRootType.SelectRole, uiRootSelectRole);
        uiRootDict.Add(UIRootType.CreateServer, uiRootServer);
        uiRootDict.Add(UIRootType.JoinRoom, uiRootClient);
        uiRootDict.Add(UIRootType.MainChat, uiRootMain);

        OpenUIRoot(UIRootType.SelectRole);
    }

    void BTN_SelectServer()
    {
        OpenUIRoot(UIRootType.CreateServer);
    }

    void BTN_SelectClient()
    {
        OpenUIRoot(UIRootType.JoinRoom);
    }

    void BTN_CreateServer()
    {
        int port = 0;

        try
        {
            port = int.Parse(inputFieldServerPort.text);

            if(Connection.instance.GetConnectionState() == Connection.ConnectionState.Disconnect)
            {
                Connection.instance.StartServer(port);
            }
            else
            {
                ShowPopup("Already running server");
            }
            
        }
        catch(Exception e)
        {
            ShowPopup("Port is not correct");
        }
    }

    void BTN_JoinClient()
    {
        if(string.IsNullOrEmpty(inputFieldClientIP.text) ||
            string.IsNullOrEmpty(inputFieldClientPort.text))
        {
            ShowPopup("Please enter ip and port");
        }

        string ip = inputFieldClientIP.text;
        int port = 0;

        try
        {
            port = int.Parse(inputFieldClientPort.text);
        }
        catch(Exception e)
        {
            ShowPopup("Error : " + e);
            return;
        }


        if (Connection.instance.GetConnectionState() == Connection.ConnectionState.Disconnect)
        {
            Connection.instance.JoinServer(ip, port);
        }
        else
        {
            ShowPopup("Already in room.");
        }
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
        foreach(var uiRoot in uiRootDict)
        {
            uiRoot.Value.SetActive(false);
        }

        uiRootDict[uiRootType].SetActive(true);
    }

    private void OnCreateServerSuccess(string msg)
    {
        OpenUIRoot(UIRootType.MainChat);
    }

    private void OnJoinRoomSuccess(string msg)
    {
        OpenUIRoot(UIRootType.MainChat);
    }

    private void OnJoinRoomFail(string msg)
    {
        ShowPopup(msg);   
    }


}
