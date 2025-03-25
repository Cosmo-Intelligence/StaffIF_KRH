echo off

rem ===================================================
rem 該当APがあるフォルダへ移動するため、パスを設定する
rem ===================================================
cd C:\Users\ichinose\Desktop\work\01_作業\01_案件\02_横河\sa_相模原協同\01_ソース\trunk\UsersLinkage\StaffLinkage\StaffLinkage\bin\x64\Release\Install

rem ===================================================
rem 32bitアプリの場合
rem ===================================================
rem C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u ..\StaffLinkage.exe

rem ===================================================
rem 64bitアプリの場合
rem ===================================================
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe /u ..\StaffLinkage.exe

pause
