REM "COPY THIS TO PlayerLauncher/bin/Debug"

FOR /L %%A IN (1, 1, 10) DO (
	start cmd /k .\PlayerLauncher.exe False 0 0 True 127.0.0.1 6666
	sleep 1
)

FOR /L %%A IN (1, 1, 10) DO (
	start cmd /k .\PlayerLauncher.exe False 1 0 True 127.0.0.1 6666
	sleep 1
)

REM "USAGE: .\PlayerLauncher.exe GUI team strategy isLoggingEnabled [server IP] [server port]"

REM Strategies: Normal, Random, Discoverer, Exchange, Superior