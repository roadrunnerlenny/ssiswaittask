# These three must be integers
!define VERSIONMAJOR 1
!define VERSIONMINOR 1
#2008
!define VERSIONBUILD 2008
!define SQLDIR "Microsoft SQL Server\100\DTS\Tasks"
#2012
#!define VERSIONBUILD 2012
#!define SQLDIR "Microsoft SQL Server\110\DTS\Tasks"


!define APPNAME "SSIS Wait Task"
!define COMPANYNAME "ALE"
!define DESCRIPTION "Wait Task for Visual Studio ${VERSIONBUILD}"

# These will be displayed by the "Click here for support information" link in "Add/Remove Programs"
# It is possible to use "mailto:" links in here to open the email client
!define HELPURL "http://ssis.andreaslennartz.de" # "Support Information" link
!define UPDATEURL "http://ssis.andreaslennartz.de" # "Product Updates" link
!define ABOUTURL "http://ssis.andreaslennartz.de" # "Publisher" link
# This is the size (in kB) of all the files copied into "Program Files"
!define INSTALLSIZE 256
 
RequestExecutionLevel admin ;Require admin rights on NT6+ (When UAC is turned on)
 
InstallDir "$PROGRAMFILES\${COMPANYNAME}\${APPNAME}"
 
# rtf or txt file - remember if it is txt, it must be in the DOS text format (\r\n)
LicenseData "license.txt"
# This will be in the installer/uninstaller's title bar
Name "${COMPANYNAME} - ${APPNAME}"
Icon "clock.ico"
outFile "SSISWaitTask${VERSIONBUILD}.exe"
 
!include LogicLib.nsh
 
# Just three pages - license agreement, install location, and installation
Page components
Page license
#page directory
Page instfiles
 
!macro VerifyUserIsAdmin
UserInfo::GetAccountType
pop $0
${If} $0 != "admin" ;Require admin rights on NT4+
        messageBox mb_iconstop "Administrator rights required!"
        setErrorLevel 740 ;ERROR_ELEVATION_REQUIRED
        quit
${EndIf}
!macroend
 
!macro CheckSQLServerDir
${If} ${FileExists} "$PROGRAMFILES\${SQLDIR}\*.*"
	;
${Else}
	messageBox mb_iconstop "SSIS Task Directory does not exists - did you install Microsoft BI for VS ${VERSIONBUILD}?"
	setErrorLevel 740 ;ERROR_ELEVATION_REQUIRED
	quit
${EndIf}
!macroend
 
Function .onInit
	SetShellVarContext all
	!insertmacro VerifyUserIsAdmin
	!insertmacro CheckSQLServerDir
FunctionEnd
 
ShowInstDetails nevershow

Section "SSIS Wait Task"
	SectionIn RO
	# Files for the install directory - to build the installer, these should be in the same directory as the install script (this file)
	# Files added here should be removed by the uninstaller (see section "uninstall")
	SetOutPath $INSTDIR		
	File "gacutil.exe"
	File "gacutil.exe.config"	
	File "gacutlrc.dll"	
 
	# Uninstaller - See function un.onInit and section "uninstall" for configuration
	WriteUninstaller "$INSTDIR\uninstall.exe"
 
	#Now write the dll into the SQL Server directory
    SetOutPath "$PROGRAMFILES\${SQLDIR}"	
	File "DLL${VERSIONBUILD}\ALE.WaitTask.dll" 
	
	#Register the components	
	ExecWait '"$INSTDIR\gacutil.exe" /f /i "$PROGRAMFILES\${SQLDIR}\ALE.WaitTask.dll"'
	
	# Registry information for add/remove programs
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "DisplayName" "${COMPANYNAME} - ${APPNAME} - ${DESCRIPTION}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "QuietUninstallString" "$\"$INSTDIR\uninstall.exe$\" /S"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "InstallLocation" "$\"$INSTDIR$\""	
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "Publisher" "$\"${COMPANYNAME}$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "HelpLink" "$\"${HELPURL}$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "URLUpdateInfo" "$\"${UPDATEURL}$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "URLInfoAbout" "$\"${ABOUTURL}$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "DisplayVersion" "$\"${VERSIONMAJOR}.${VERSIONMINOR}.${VERSIONBUILD}$\""
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "VersionMajor" ${VERSIONMAJOR}
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "VersionMinor" ${VERSIONMINOR}
	# There is no option for modifying or repairing the install
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "NoRepair" 1
	# Set the INSTALLSIZE constant (!defined at the top of this script) so Add/Remove Programs can accurately report the size
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "EstimatedSize" ${INSTALLSIZE}
SectionEnd
 
# Uninstaller
 
Function un.onInit
	SetShellVarContext all
 
	#Verify the uninstaller - last chance to back out
	MessageBox MB_OKCANCEL "Permanantly remove ${APPNAME}?" IDOK next
		Abort
	next:
	!insertmacro VerifyUserIsAdmin
FunctionEnd
 
ShowUninstDetails nevershow 

Section "uninstall"
 
	#Unregister Component
	ExecWait '"$INSTDIR\gacutil.exe" -u ALE.WaitTask'
	
	# Remove files
	Delete $INSTDIR\gacutlrc.dll		
	Delete $INSTDIR\gacutil.exe.config
	Delete $INSTDIR\gacutil.exe
	
	Delete "$PROGRAMFILES\${SQLDIR}\ALE.WaitTask.dll"
 
	# Always delete uninstaller as the last action
	Delete $INSTDIR\uninstall.exe
 
	# Try to remove the install directory - this will only happen if it is empty
	rmDir $INSTDIR
 
	# Remove uninstaller information from the registry
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}"
SectionEnd