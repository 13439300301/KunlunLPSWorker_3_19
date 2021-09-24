SET PATH=C:\Program Files\7-Zip;%PATH%

SET LPS_VERSION=3.19
SET PUBLISH_PATH=D:\publish\3.19\KunlunLPSWorker
SET PACKAGE_PATH=D:\packages\crs_package\3.19\LPSWorker\Nightly\

REM !! DO NOT MODIFY !!
SET VERSION=%LPS_VERSION%-build-%1%
REM !! END DO NOT MODIFY !!

net stop KunlunLPSWorker319 | exit 0
rmdir "%PUBLISH_PATH%" | exit 0

cd ..
dotnet restore KunlunLPSWorker.sln --configfile D:/tools/CI/NuGet.config

cd ./src/Kunlun.LPS.Worker.Console
dotnet publish ./Kunlun.LPS.Worker.Console.csproj --output D:\publish\3.19\KunlunLPSWorker --configuration Release

del KunlunLPSWorker.zip | exit 0
7z a KunlunLPSWorker.zip "%PUBLISH_PATH%"
mkdir "%PACKAGE_PATH%"
move KunlunLPSWorker.zip "%PACKAGE_PATH%KunlunLPSWorker_%VERSION%.zip"

net start KunlunLPSWorker319

@echo ------------------------------
@echo -- Use this info to release --
@echo ------------------------------
@echo ECRS_Version:
@echo %LPS_VERSION%
@echo:
@echo LPSWorker_Version:
@echo %VERSION%
@echo ------------------------------
