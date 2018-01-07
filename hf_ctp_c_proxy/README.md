
# hf_ctp_c_proxy
 上期技术CTP封装,可用C#,PY等语言调用.采用最新CTP版本,支持期权交易.支持win和linux 64位系统.
 
##C++封装的目的是什么?
通过封装,将原DLL中无法访问到的函数暴露出来供上层应用调用.

##封装中的要点

* `__declspec(dllexport)`
 <p>  表明函数为导出函数
* `extern "C"`
  <p> 导出的函数名与声明一致,否则导出函数名无法正常使用
  <p> 当然,也可以通过定义`.def`文件实现函数名正常
* `_stdcall`
  <p> 32位不能使用此声明
  <p> linux不能使用此声明
* WIN32 & _WINDOWS
  * WIN32 编译32位C+时使用
  * _WINDOWS 编译64位时使用

## 完整的.h宏定义
  `64位`
```c
#ifdef _WINDOWS  //64位系统没有预定义 WIN32
#ifdef __cplusplus
#define DLL_EXPORT_C_DECL extern "C" __declspec(dllexport)
#else
#define DLL_EXPORT_DECL __declspec(dllexport)
#endif
#else
#ifdef __cplusplus
#define DLL_EXPORT_C_DECL extern "C"
#else
#define DLL_EXPORT_DECL extern
#endif
#endif

#ifdef _WINDOWS
#define WINAPI      __stdcall
#define WIN32_LEAN_AND_MEAN             //从 Windows 头文件中排除极少使用的信息
#include "stddef.h"
#else
#define WINAPI
#endif

```
## 有问题反馈
在使用中有任何问题，欢迎反馈给我，可以用以下联系方式跟我交流

* 邮件(hubert28@qq.com)
* QQ: 24918700
* Q群:65164336
* 代码同步
    * https://git.oschina.net/haifengat
    * https://github.com/haifengat

##代码
### test.h

```c
  
#pragma once
#define TEST_API extern "C" __declspec(dllexport)

#define WINAPI   __stdcall    //32位不能声明为__stdcall,否则函数名乱码
#define WIN32_LEAN_AND_MEAN             //  从 Windows 头文件中排除极少使用的信息
#include "stddef.h"

// 此类是从 Test.dll 导出的
class CTest {
public:
	CTest(void);
	// TODO:  在此添加您的方法。
};

```

### test.cpp

```c

#include "stdafx.h"
#include "Test.h"

// 声明回调函数类型
typedef int (WINAPI *FrontConnected)();

// 回调函数变量
void *_connect;

// 利用set函数将上层的回调函数指针传递到C+层,并赋值给_connect
TEST_API void WINAPI SetConnect(void* conn)
{
    _connect = conn;
}

// C+层通过_connect调用上层的回调函数
TEST_API int WINAPI fnTest(int a, int b)
{
    if (_connect != NULL)
	{
		((FrontConnected)_connect)();
	}
	return a + b;
}


// 这是已导出类的构造函数。
// 有关类定义的信息，请参阅 Test.h
CTest::CTest()
{
	return;
}
```

## C#调用
* 使用`LoadLibrary`动态加载C+的dll
* `GetProcAddress`取得C+中函数的地址,对应C#的IntPtr
* 调用C++函数
    * 定义`fnTestDef`类型
    * 取值fnTestDef fnTest = ... 取到C+中`fnTest`函数
    * fnTest(xx)直接调用
* 回调函数
    * 定义回调函数类型 `ConnectDef`
    * 定义`SetConnectDef(ConnectDef fn)`函数类型
    * 取值`SetConnectDef setConnect = xxx` 取得C+中 `SetConnect` 函数
    * 调用`setConnect`函数,将C#的函数`Connect()`传递给C++中的`_connect`
    * 到此为止,C+中调用`_connect`时,即实现了对C#`Connect`的调用

```c
class Test
{

	[DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
	public static extern IntPtr LoadLibrary(
		[MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

	[DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
	public static extern IntPtr GetProcAddress(IntPtr hModule,
		[MarshalAs(UnmanagedType.LPStr)] string lpProcName);

	[DllImport("kernel32.dll", EntryPoint = "FreeLibrary")]
	public static extern bool FreeLibrary(int hModule);
	
	delegate int fnTestDef(int a, int b);
	delegate int SetConnectDef(ConnectDef fn);//参数必须声明为ConnectDef不能用Delegate
	public delegate int ConnectDef();
	IntPtr h;

	//直播中问题解决方法:上面声明的LoadLibrary由int改为IntPtr
	public Test()
	{
		var file = @".\test.dll";
		if (!File.Exists(file)) return;
		h = LoadLibrary(file);
		IntPtr fn = GetProcAddress(h, "fnTest");
		fnTestDef fnTest = (fnTestDef)Marshal.GetDelegateForFunctionPointer(fn, typeof(fnTestDef));

		IntPtr set = GetProcAddress(h, "SetConnect");
		SetConnectDef setConnect = (SetConnectDef)Marshal.GetDelegateForFunctionPointer(set, typeof(SetConnectDef));
		setConnect(Connect);

		Console.WriteLine(fnTest(3, 4));
	}

	private int Connect()
	{
		Console.WriteLine("connected");
		return 0;
	}
}
```

