using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : MonoBehaviour
{

    //UI1.1要做逻辑的话我们要给按钮绑定事件，首先，我们要找到按钮给按钮做初始化

    GameObject register;
    //注册相关
    Button registerBtn;
    InputField usernameInput;
    InputField emailInput;
    InputField passwordInput;
    Button _registerBtn;
    Button _closeBtn;

    //登录相关
    Button loginBtn;
    InputField usernameInput_Login;
    InputField passwordInput_Login;
    //提示
    GameObject _tipsObj;


    // Start is called before the first frame update
    void Start()
    {
        ///------------UI1.2进行初始化,并为其添加接口，接口可以先命名在生成-----------------------

        register = transform.Find("Register").gameObject;
        //打开注册界面
        registerBtn = transform.Find("RegisterBtn").GetComponent<Button>();
        registerBtn.onClick.AddListener(OpenRegisterView);
        //关闭注册界面
        _closeBtn = transform.Find("Register/View/CloseBtn").GetComponent<Button>();
        _closeBtn.onClick.AddListener(CloseRegisterView);
        //注册界面输入框
        usernameInput = transform.Find("Register/View/UsernameInput").GetComponent<InputField>();
        emailInput = transform.Find("Register/View/EmailInput").GetComponent<InputField>();
        passwordInput = transform.Find("Register/View/PasswordInput").GetComponent<InputField>();
        //注册界面——注册按钮
        _registerBtn = transform.Find("Register/View/RegisterBtn").GetComponent<Button>();
        _registerBtn.onClick.AddListener(RegisterOnClick);

        //登录界面——登录按钮
        loginBtn = transform.Find("LoginBtn").GetComponent<Button>();
        loginBtn.onClick.AddListener(LoginOnClick);

        usernameInput_Login = transform.Find("UsernameInput").GetComponent<InputField>();
        passwordInput_Login = transform.Find("PasswordInput").GetComponent <InputField>();

        //处理登录结果
        MessageHelper.Instance.loginMsgHandle += LoginHandle;
        MessageHelper.Instance.registerHandle += RegisterHandle;
    }


    // Update is called once per frame
    void Update()
    {

    }
    ///--------生成给按钮与输入框的接口-----------------
  

    private void RegisterHandle(RegisterMsgS2C obj)         //注册结果的处理
    {
        if(obj.result == 0)         //看看成功还是失败，成功就提示并将返回的账号和密码输入上，失败就返回错误码并提示
        {
            Debug.Log("注册成功");
            Tips("注册成功");
            register.gameObject.SetActive(false);
            usernameInput_Login.text = obj.account;
            passwordInput_Login.text = obj.password;
        }
        else
        {
            Tips($"注册失败，错误码：{obj.result}");     //通常错误了我们会把这个错误码打印出来，去表里找，把错误加进来提示给用户
        }
    }
    private void LoginHandle(LoginMsgS2C obj)           //服务端返回登录结果，我们对其的处理
    {
        if (obj.result == 0)                 //看看成功还是失败，成功就下面逻辑，失败就返回错误码并提示
        {
            PlayerData.Instance.loginMsgS2C = obj;      //将用户的信息缓存起来
            Debug.Log("登录成功");
            this.gameObject.SetActive(false);           //关闭本界面

            var chatView = Resources.Load<GameObject>("ChatView");    //在Resources里找到聊天界面并实例化出来
            var chatObj =  GameObject.Instantiate<GameObject>(chatView);
            chatObj.AddComponent<ChatView>();           //给聊天界面挂上聊天管理脚本
        }
        else
        {
            Tips($"登录失败，错误码：{obj.result}");   
        }
    }

    private void OpenRegisterView()         //打开注册界面
    {
        register.gameObject.SetActive(true);
    }

    private void CloseRegisterView()        //关闭注册界面
    {
        register.gameObject.SetActive(false);
    }

    private void RegisterOnClick()          //在注册界面点击了确认注册按钮
    {
        //，做判定，看看有没有输入邮箱、账号、密码 如果有没输入的就做相应回应，如果都对了就将其发送给服务端
        if (string.IsNullOrEmpty(usernameInput.text))
        {
            Tips("请输入账号！！！");
            return;
        }
        if (string.IsNullOrEmpty(emailInput.text))
        {
            Tips("请输入邮箱！！！");
            return;
        }
        if (string.IsNullOrEmpty(passwordInput.text))
        {
            Tips("请输入密码！！！");
            return;
        }
        //调用刚刚声明的接口发送数据给服务器的接口
        MessageHelper.Instance.SendRegisterMsg(usernameInput.text, emailInput.text, passwordInput.text);
    }

    private void LoginOnClick()         //在登录界面登录
    {
        //先做判定，是否正确
        if (string.IsNullOrEmpty(usernameInput_Login.text))
        {
            Tips("请输入账号！！！");
            return;
        }
        if (string.IsNullOrEmpty(passwordInput_Login.text))
        {
            Tips("请输入密码！！！");
            return;
        }
        //通过这个接口将数据发送给服务端
        MessageHelper.Instance.SendLoginMsg(usernameInput_Login.text, passwordInput_Login.text);
    }

    //确认注册输入不正确提示
    public void Tips(string str)
    {
        if (_tipsObj != null)                                       //用于反复用这个方法时候的保险
        {
            GameObject.Destroy(_tipsObj);
        }
        var tipItem = transform.Find("TipsItem").gameObject;        //在场景里找到TipsItem文件
        _tipsObj = GameObject.Instantiate<GameObject>(tipItem);     //将其实例化一个，并付给_tipsObj
        _tipsObj.transform.SetParent(this.transform, false);        //让_tipsObj成为本脚本所在对象的子物体
        var content = _tipsObj.transform.Find("Text").GetComponent<Text>(); //找到_tipsObj下的Text文件的Text组件
        content.text = str;                                         //改变一下Text内容
        _tipsObj.gameObject.SetActive(true);                        //将_tipsObj激活
        GameObject.Destroy(_tipsObj,1.5f);                          //激活后1.5秒将其销毁

    }
}
