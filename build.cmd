@ECHO OFF
SETLOCAL

REM Uncomment to forcibly set the build version.
REM set FORCE_VERSION=0.0.4-alpha4

IF NOT DEFINED DevEnvDir (
	IF DEFINED vs140comntools ( 
		CALL "%vs140comntools%\vsvars32.bat"
	)
)

nuget restore source\SharpFlame.VS2015.sln
if %errorlevel% neq 0 exit /b %errorlevel%

msbuild Source\Builder\Builder.csproj
if %errorlevel% neq 0 exit /b %errorlevel%

ECHO ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
ECHO        RUNNING BAU BUILDER
ECHO ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Source\Builder\bin\Debug\Builder.exe %1
if %errorlevel% neq 0 exit /b %errorlevel%
