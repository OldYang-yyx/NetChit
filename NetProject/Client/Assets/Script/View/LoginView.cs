using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : MonoBehaviour
{
    GameObject register;

    //ע�����
    Button registerBtn;
    InputField usernameInput;
    InputField emailInput;
    InputField passwordInput;
    Button _registerBtn;
    Button _closeBtn;

    //��¼���
    Button loginBtn;
    InputField usernameInput_Login;
    InputField passwordInput_Login;

    // Start is called before the first frame update
    void Start()
    {
        register = transform.Find("Register").gameObject;

        //��ע�����
        registerBtn = transform.Find("RegisterBtn").GetComponent<Button>();
        registerBtn.onClick.AddListener(OpenRegisterView);

        //�ر�ע�����
        _closeBtn = transform.Find("Register/View/CloseBtn").GetComponent<Button>();
        _closeBtn.onClick.AddListener(CloseRegisterView);//alt+enter

        //ע����������
        usernameInput = transform.Find("Register/View/UsernameInput").GetComponent<InputField>();
        emailInput = transform.Find("Register/View/EmailInput").GetComponent<InputField>();
        passwordInput = transform.Find("Register/View/PasswordInput").GetComponent<InputField>();

        //ע�����-ע�ᰴť
        _registerBtn = transform.Find("Register/View/RegisterBtn").GetComponent<Button>();
        _registerBtn.onClick.AddListener(RegisterOnClick);



        //��¼����-��¼��ť
        loginBtn = transform.Find("LoginBtn").GetComponent<Button>();
        loginBtn.onClick.AddListener(LoginOnClick);

        usernameInput_Login = transform.Find("UsernameInput").GetComponent<InputField>();
        passwordInput_Login = transform.Find("PasswordInput").GetComponent<InputField>();

        //�����¼���
        MessageHelper.Instance.loginHandle += LoginHandle;
        MessageHelper.Instance.rigisterHandle += RigisterHandle;
    }

    private void RigisterHandle(RegisterMsgS2C obj)
    {
        if (obj.result == 0)
        {
            Debug.Log("ע��ɹ�");
            Tips("ע��ɹ�");
            register.gameObject.SetActive(false);
            usernameInput_Login.text = obj.account;
            passwordInput_Login.text = obj.password;
        }
        else
        {
            Tips($"ע��ʧ��,������:{obj.result}");
        }
    }

    private void LoginHandle(LoginMsgS2C obj)
    {
        if (obj.result == 0)
        {
            PlayerData.Instance.loginMsgS2C = obj;
            Debug.Log("��¼�ɹ�");
            this.gameObject.SetActive(false);

            var chatView = Resources.Load<GameObject>("ChatView");
            var chatObj = GameObject.Instantiate<GameObject>(chatView);
            chatObj.AddComponent<ChatView>();
        }
        else
        {
            Tips($"��¼ʧ��,������:{obj.result}");
        }
    }

    private void LoginOnClick()
    {
        if (string.IsNullOrEmpty(usernameInput_Login.text))
        {
            Tips("�������ʺ�!!!");
            return;
        }

        if (string.IsNullOrEmpty(passwordInput_Login.text))
        {
            Tips("����������!!!");
            return;
        }

        //�������ݸ�������
        MessageHelper.Instance.SendLoginMsg(usernameInput_Login.text, passwordInput.text);
    }

    private void RegisterOnClick()
    {
        if (string.IsNullOrEmpty(usernameInput.text))
        {
            Tips("�������ʺ�!!!");
            return;
        }

        if (string.IsNullOrEmpty(emailInput.text))
        {
            Tips("����������!!!");
            return;
        }

        if (string.IsNullOrEmpty(passwordInput.text))
        {
            Tips("����������!!!");
            return;
        }
        //�������ݸ�������
        MessageHelper.Instance.SendRegisterMsg(usernameInput.text, emailInput.text, passwordInput.text);
    }

    private void CloseRegisterView()
    {
        register.gameObject.SetActive(false);
    }

    private void OpenRegisterView()
    {
        register.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    GameObject _tipsObj;
    //��ʾ
    public void Tips(string str)
    {
        if (_tipsObj != null)
        {
            GameObject.Destroy(_tipsObj);
        }
        var tipsItem = transform.Find("TipsItem").gameObject;
        _tipsObj = GameObject.Instantiate<GameObject>(tipsItem);
        _tipsObj.transform.SetParent(this.transform, false);
        var content = _tipsObj.transform.Find("Text").GetComponent<Text>();
        content.text = str;
        _tipsObj.gameObject.SetActive(true);
        GameObject.Destroy(_tipsObj, 1.5f);
    }

}
