@echo off 
cls

set scripts  = .\
set user     = user
set password = ...
set server   = host name or ip-address
set catalog  = database name
set timeout  = 120

echo ======== Settings ========
echo Script folder : %scripts%
echo User          : %user%
echo Password      : %password%
echo Server        : %server%
echo Catalog       : %catalog%
echo Timeout       : %timeout%
echo ==========================

set/p answer=Continue (y/n)?
if "%answer%" == "y" goto Yes
if "%answer%" == "Y" goto Yes
goto No

:Yes
echo do it
DatabaseVersioner /scripts:%scripts% /user:%user% /password:%password% /server:%server% /catalog:%catalog% /timeout:%timeout%

:No
exit /b 1

