# SSIS Wait Task
A SSIS task which suspends execution for a period or until a specific time. Additionally, a sql statement can be defined that can also delay execution.

# Features
Use it as a simple "Sleep" task - e,g. just delay execution of all following tasks for a couple of minutes
Use it as a scheduler - e.g. define a time when your package (or a part of it) is executed
Use it to wait for an event in your database - e.g. just execute something in your package when a particular value in your database was set

# Screenshots
![Task](http://www.andreaslennartz.de/_/rsrc/1374846531155/dotnet/waittask/ALEWaitTask_RunningTask.JPG)

![Task Properties](http://www.andreaslennartz.de/_/rsrc/1376918387502/dotnet/waittask/ALEWaitTask_PropertiesScreen.JPG)

![Executing the task](http://www.andreaslennartz.de/_/rsrc/1374849110380/dotnet/waittask/ALEWaitTask_ProgressScreen.JPG)

# Prerequisites
To use this SSIS Task, you must have either Visual Studio 2008 or Visual Studio 2012 and SQL Server Integration Tools (comes with Microsoft BI) installed

# Installation
You can either use the executable installer (method 1) or download and register the dll (method 2).

## Installation method 1: Using the installer
Just download the installer somewhere on your local harddrive. Start the installer and follow the on-screen instructions. 

## Installation method 2: Download and register the dll
First, download the dll. Copy it into your SQL Server Task Directory. This should be something like
VS 2012: "C:\Program Files (x86)\Microsoft SQL Server\110\DTS\Tasks"
VS 2008: "C:\Program Files (x86)\Microsoft SQL Server\110\DTS\Tasks"

Then locate your local gacutil installion. If you can't find it, check http://blogs.iis.net/davcox/archive/2009/07/14/where-is-gacutil-exe.aspxhere for more help.

Now use gacutil to register the dll with your Global Assembly Cache (GAC). Open a command prompt and enter something like gacutil /i ALE.WaitTask.DLL. Make sure to set the proper path. 

# Post-Installation
For Visual Studio 2012, just restart your Visual Studio. 

For Visual Studio 2008, right click somewhere in your SSIS Toolbox, select Choose Items... - a new window will pop up. Select the tab SSIS Control Flow Items, and make a check at A.LE Wait Task. Press OK. 

In the SSIS Toolbox for Control Flows, you should now find a toolbox item named A.LE. Wait Task

# Usage

In the SSIS Toolbox for Control Flow items, drag and drop the Task A.LE Wait Task somewhere into your Integration Service Package. Double click will pop up a property dialog:

![Property dialog](http://www.andreaslennartz.de/_/rsrc/1376918387502/dotnet/waittask/ALEWaitTask_PropertiesScreen.JPG)

## General
Name : Specifies the name for this task
Description : Describes the task

## Task state
DoNothingAndContinue : If set to true, the execution will continue without any waiting.

## Wait Time
WaitUntilTime : Time until the wait task sleeps (hh:MM) - 24h format. Subsequently the evaluation of the sql check statement is started.
SleepTimesInMinutes : Amount of minutes this task will sleep before execution is continued. Subsequently the evaluation of the sql check statement is started.
Note: If both times are set, the task will wait until the given WaitUntilTime and then sleep for the given SleepTimesInMinutes

## SQL
SQLCheckStatement : An SQL statement that either returns 1 (true) or 0. Wait task delays execution until the statement returns 1.
SQLConnection : OLEDB or ADO.NET Connection on which the SQL check statement is executed. 
MaximumSQLWaitTime : Time period in minutes during the SQL will be evaluated frequently and execution is delayed. If within this period the statement returns 1 the execution is continued, otherwise the execution will be delayed. 
SQLRepetitionFrequencyInMinutes : Pause in minutes between the execution of the SQL check statements. If left empty, the execution frequency will be 1 second. 

## Expressions
You can use expression here as you are used to in SSIS. See the Microsoft documentation (http://technet.microsoft.com/en-us/library/ms137547.aspx) for more details.
