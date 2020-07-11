# *Human: Fall Flat* Source Code

The decompiled source code of the Unity game *Human: Fall Flat*.

English | [简体中文](/docs/README_cn.md)

<br />

## Declaraction

***

This project aims to provide assistance to the innovative development of the game *Human: Fall Flat*. It also facilitates developers to conduct research and develop modules. The copyright of this project belongs to [*No Brakes Games*](https://www.nobrakesgames.com/). It is only used for technical research and prohibits any type of commercial use. The officials of *Human: Fall Flat* severely crack down on such acts and reserve the right to pursue legal responsibility.

The code of this project follows the GPLv3 open source agreement. But it should be noted that the scope of the code is limited to the Assembly-CSharp.dll library file. The developer's secondary development can't bypass the own copyright of *Human: Fall Flat*. So it is not allowed to use this code for cracking or other acts that violate the copyright of the game.

<br />

## Introduction

***

This project is a decompilation of the core function code in *Human: Fall Flat* program, namely the Assembly-CSharp.dll. The project will be updated with the update of *Human: Fall Flat*. The project uses [ILSpy](https://github.com/icsharpcode/ILSpy) to decompile and build C sharp project files. Then import into Visual Studio IDE to make solution files. The files of this project include a C Sharp project file and a Visual Studio solution file. If these two files' version are inconsistent, they can be regenerated without affecting the code.

This project contains two branches. The master branch represents the code of Windows. The osx branch represents the code of MacOS. Please use them according to the actual situation.

<br />

## Build

***

We recommend using Visual Studio to build the solution and generate the Assembly-CSharp.dll file:
```
Open Assembly-CSharp.sln file by Visual Studio IDE;

Choose "Assembly-CSharp" from the "Solution Explorer" on the right;

Right click the item, choose "Build" menu;

Then you can see the result of building at the bottom.
```

Build Visual Studio solution through the /src/Assembly-CSharp.csproj file:
```
open /src/Assembly-CSharp.csproj file by Visual Studio IDE;

Choose "References" from the "Solution Explorer" on the right;

Right click the item, choose "Add Reference..." menu;

Choose "Browse" on the left of the pop-up "Reference Manager" dialog, then click "Browse" button at the bottom;

According to the system version used, select different folders under the "reference" folder and import all the dll files;

Click "Add" button, a prompt box indicating the failure to introduce "mscorlib.dll" may pop up, click "OK" directly;

Click "Project"->"Assembly-CSharp Properties" in the menu bar to enter the project property settings;

Set the "Target Framework" to ".NET Framework 4";

Close the page and continue to generate according to the above "Using Visual Studio to build the solution".
```

Build by csc command:
```
Now writing...
```

<br />

## Credit

***

[XiaoHuiHui233](https://github.com/XiaoHuiHui233)