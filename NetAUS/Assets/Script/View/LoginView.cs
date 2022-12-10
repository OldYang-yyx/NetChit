using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : MonoBehaviour
{

    //UI1.1Ҫ���߼��Ļ�����Ҫ����ť���¼������ȣ�����Ҫ�ҵ���ť����ť����ʼ��

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
    //��ʾ
    GameObject _tipsObj;


    // Start is called before the first frame update
    void Start()
    {
        ///------------UI1.2���г�ʼ��,��Ϊ����ӽӿڣ��ӿڿ���������������-----------------------

        register = transform.Find("Register").gameObject;
        //��ע�����
        registerBtn = transform.Find("RegisterBtn").GetComponent<Button>();
        registerBtn.onClick.AddListener(OpenRegisterView);
        //�ر�ע�����
        _closeBtn = transform.Find("Register/View/CloseBtn").GetComponent<Button>();
        _closeBtn.onClick.AddListener(CloseRegisterView);
        //ע����������
        usernameInput = transform.Find("Register/View/UsernameInput").GetComponent<InputField>();
        emailInput = transform.Find("Register/View/EmailInput").GetComponent<InputField>();
        passwordInput = transform.Find("Register/View/PasswordInput").GetComponent<InputField>();
        //ע����桪��ע�ᰴť
        _registerBtn = transform.Find("Register/View/RegisterBtn").GetComponent<Button>();
        _registerBtn.onClick.AddListener(RegisterOnClick);

        //��¼���桪����¼��ť
        loginBtn = transform.Find("LoginBtn").GetComponent<Button>();
        loginBtn.onClick.AddListener(LoginOnClick);

        usernameInput_Login = transform.Find("UsernameInput").GetComponent<InputField>();
        passwordInput_Login = transform.Find("PasswordInput").GetComponent <InputField>();

        //�����¼���
        MessageHelper.Instance.loginMsgHandle += LoginHandle;
        MessageHelper.Instance.registerHandle += RegisterHandle;
    }


    // Update is called once per frame
    void Update()
    {

    }
    ///--------���ɸ���ť�������Ľӿ�-----------------
  

    private void RegisterHandle(RegisterMsgS2C obj)         //ע�����Ĵ���
    {
        if(obj.result == 0)         //�����ɹ�����ʧ�ܣ��ɹ�����ʾ�������ص��˺ź����������ϣ�ʧ�ܾͷ��ش����벢��ʾ
        {
            Debug.Log("ע��ɹ�");
            Tips("ע��ɹ�");
            register.gameObject.SetActive(false);
            usernameInput_Login.text = obj.account;
            passwordInput_Login.text = obj.password;
        }
        else
        {
            Tips($"ע��ʧ�ܣ������룺{obj.result}");     //ͨ�����������ǻ������������ӡ������ȥ�����ң��Ѵ���ӽ�����ʾ���û�
        }
    }
    private void LoginHandle(LoginMsgS2C obj)           //����˷��ص�¼��������Ƕ���Ĵ���
    {
        if (obj.result == 0)                 //�����ɹ�����ʧ�ܣ��ɹ��������߼���ʧ�ܾͷ��ش����벢��ʾ
        {
            PlayerData.Instance.loginMsgS2C = obj;      //���û�����Ϣ��������
            Debug.Log("��¼�ɹ�");
            this.gameObject.SetActive(false);           //�رձ�����

            var chatView = Resources.Load<GameObject>("ChatView");    //��Resources���ҵ�������沢ʵ��������
            var chatObj =  GameObject.Instantiate<GameObject>(chatView);
            chatObj.AddComponent<ChatView>();           //�������������������ű�
        }
        else
        {
            Tips($"��¼ʧ�ܣ������룺{obj.result}");   
        }
    }

    private void OpenRegisterView()         //��ע�����
    {
        register.gameObject.SetActive(true);
    }

    private void CloseRegisterView()        //�ر�ע�����
    {
        register.gameObject.SetActive(false);
    }

    private void RegisterOnClick()          //��ע���������ȷ��ע�ᰴť
    {
        //�����ж���������û���������䡢�˺š����� �����û����ľ�����Ӧ��Ӧ����������˾ͽ��䷢�͸������
        if (string.IsNullOrEmpty(usernameInput.text))
        {
            Tips("�������˺ţ�����");
            return;
        }
        if (string.IsNullOrEmpty(emailInput.text))
        {
            Tips("���������䣡����");
            return;
        }
        if (string.IsNullOrEmpty(passwordInput.text))
        {
            Tips("���������룡����");
            return;
        }
        //���øո������Ľӿڷ������ݸ��������Ľӿ�
        MessageHelper.Instance.SendRegisterMsg(usernameInput.text, emailInput.text, passwordInput.text);
    }

    private void LoginOnClick()         //�ڵ�¼�����¼
    {
        //�����ж����Ƿ���ȷ
        if (string.IsNullOrEmpty(usernameInput_Login.text))
        {
            Tips("�������˺ţ�����");
            return;
        }
        if (string.IsNullOrEmpty(passwordInput_Login.text))
        {
            Tips("���������룡����");
            return;
        }
        //ͨ������ӿڽ����ݷ��͸������
        MessageHelper.Instance.SendLoginMsg(usernameInput_Login.text, passwordInput_Login.text);
    }

    //ȷ��ע�����벻��ȷ��ʾ
    public void Tips(string str)
    {
        if (_tipsObj != null)                                       //���ڷ������������ʱ��ı���
        {
            GameObject.Destroy(_tipsObj);
        }
        var tipItem = transform.Find("TipsItem").gameObject;        //�ڳ������ҵ�TipsItem�ļ�
        _tipsObj = GameObject.Instantiate<GameObject>(tipItem);     //����ʵ����һ����������_tipsObj
        _tipsObj.transform.SetParent(this.transform, false);        //��_tipsObj��Ϊ���ű����ڶ����������
        var content = _tipsObj.transform.Find("Text").GetComponent<Text>(); //�ҵ�_tipsObj�µ�Text�ļ���Text���
        content.text = str;                                         //�ı�һ��Text����
        _tipsObj.gameObject.SetActive(true);                        //��_tipsObj����
        GameObject.Destroy(_tipsObj,1.5f);                          //�����1.5�뽫������

    }
}
