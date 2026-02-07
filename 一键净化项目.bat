@echo OFF
setlocal enabledelayedexpansion

echo 正在清理项目文件...

REM 删除前端node_modules（更高效的方式）
if exist ".\Web\node_modules" (
    echo 正在删除 Web node_modules...
    rd /s /q ".\Web\node_modules" 2>nul
)

REM 清理Admin.NET项目的bin、obj和public文件夹
for /d /r ".\" %%b in (bin obj public) do (
    if exist "%%b" (
        echo 正在删除 %%~b...
        rd /s /q "%%b" 2>nul
    )
)

echo 【处理完毕，按任意键退出】
pause >nul
exit /b 0