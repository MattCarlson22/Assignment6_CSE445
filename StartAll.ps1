# StartAll.ps1
# -------------------------------------------------
# Launch three IIS-Express instances in parallel,
# each binding one of the projects to its own port.
# -------------------------------------------------

# 1) Path to iisexpress.exe
$iis = "C:\Program Files\IIS Express\iisexpress.exe"

# 2) Root folder where your projects live
# CHANGE THIS LINE TO WHERE THE PROJECTS ARE vvvvvv
$root = "$env:USERPROFILE\Desktop\School\CSE445\Assignment6"

# 3) Launch WebAPIStuff on 5551
Start-Process $iis -ArgumentList @(
  "/path:`"$root\WebAPIStuff`"",
  "/port:5551",
  "/trace:error"
) -NoNewWindow

# 4) Launch TextToPdfTryIt on 5552
Start-Process $iis -ArgumentList @(
  "/path:`"$root\TextToPdfTryIt`"",
  "/port:5552",
  "/trace:error"
) -NoNewWindow

# 5) Launch TextChunkEditor on 5553
Start-Process $iis -ArgumentList @(
  "/path:`"$root\TextChunkEditor`"",
  "/port:5553",
  "/trace:error"
) -NoNewWindow

Write-Host "All sites started:"
Write-Host " • http://localhost:5551/    (WebAPIStuff)"
Write-Host " • http://localhost:5552/    (TextToPdfTryIt)"
Write-Host " • http://localhost:5553/    (TextChunkEditor)"
