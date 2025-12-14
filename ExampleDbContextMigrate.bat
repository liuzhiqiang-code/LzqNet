@echo off
setlocal enabledelayedexpansion

:: 跳转到项目目录
cd /d "src\BusinessServices\LzqNet.Services.Msm"

:: 生成带时间戳的迁移名称（格式：Migration_yyyyMMdd_HHmmss）
for /f "tokens=1-3 delims=/ " %%a in ('date /t') do (set datepart=%%c%%a%%b)
for /f "tokens=1-3 delims=:." %%a in ('time /t') do (set timepart=%%a%%b%%c)
set "migration_name=Migration_%datepart%_%timepart%"

:: 执行迁移命令
dotnet ef migrations add %migration_name% --context ExampleDbContext

:: 应用迁移
:: dotnet ef database update --context ExampleDbContext

pause
