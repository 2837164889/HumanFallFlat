# 《人类：一败涂地》源代码

Unity游戏《人类：一败涂地》的反编译源代码。

[English](/README.md) | 简体中文

<br />

## 声明


本项目旨在对《人类：一败涂地》的创新性设计提供帮助，方便开发者进行mod功能研究和开发。本项目版权归[*No Brakes Games*工作室](https://www.nobrakesgames.com/)所有，仅用于技术研究，禁止任何类型的商业用途。《人类：一败涂地》官方严厉打击此类行为，并保留追究法律责任的权利。

本项目代码遵循GPLv3开源协议。但须注意的是，本项目代码的作用范围仅限于Assembly-CSharp.dll库文件。开发者进行二次开发并不能绕过《人类：一败涂地》本身所具有的版权，因此不得使用此代码进行《人类：一败涂地》破解等违反《人类：一败涂地》本身所具有版权的行为。

<br />

## 介绍


本项目是对《人类：一败涂地》程序中功能核心代码，即Assembly-CSharp.dll库文件的反编译。项目会随着《人类：一败涂地》的更新而更新。项目使用[ILSpy](https://github.com/icsharpcode/ILSpy)反编译工具进行反编译，生成C Sharp项目文件，之后导入Visual Studio IDE中生成解决方案。项目文件中包含了C Sharp项目文件和Visual Studio解决方案文件。如果版本不一致，可以重新生成两个文件，不会影响代码本身。

本项目包含两个分支，master分支表示Windows版本的代码，osx分支表示MacOS版本的代码，请根据实际情况使用。

<br />

## 构建


建议使用Visual Studio进行解决方案构建，生成Assembly-CSharp.dll文件：
```
使用Visual Studio IDE打开Assembly-CSharp.sln文件；

在右侧“解决方案资源管理器”中选择“Assembly-CSharp”项；

右键该项，选择“生成”；

在下方输出栏即可看到生成结果。
```

通过/src/Assembly-CSharp.csproj文件进行Visual Studio解决方案构建：
```
使用Visual Studio IDE打开/src/Assembly-CSharp.csproj文件；

在右侧“解决方案资源管理器”中选择“引用”；

右键该项，选择“添加引用”；

在弹出的“引用管理器”对话框中，选择左边的“浏览”，再点击下面的“浏览”按钮；

根据所使用的系统版本，选择reference文件夹下的不同文件夹，引入其中的所有dll文件；

点击“添加”按钮，可能会弹出“mscorlib.dll”引入失败的提示框，直接点击“确定”；

点选菜单栏中的“项目”->“Assembly-CSharp属性”，进入项目属性设置；

将“目标框架”选择为“.NET Framework 4”；

关闭该页面，按照上方“使用Visual Studio进行解决方案构建”的方式继续生成即可。
```

使用csc命令进行生成：
```
正在开发中...
```

<br />

## 贡献


[XiaoHuiHui233](https://github.com/XiaoHuiHui233)