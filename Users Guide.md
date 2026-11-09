# Metaballs ScreenSaver: User's Guide



## Downloads & Runtimes

Download the latest release from the [Releases page](https://github.com/RemusRigo/MetaballsScreenSaver-VB.NET/releases).

Each release contains one folder per framework and architecture:

|Folder|Target Framework|Architecture|Required Runtime|
|-|-|-|-|
|`net48 x64`|.NET Framework 4.8|64-bit|[.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48) (built into Windows 10 1903+ and Windows 11)|
|`net48 x86`|.NET Framework 4.8|32-bit|[.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48) (built into Windows 10 1903+ and Windows 11)|
|`net8.0 x64`|.NET 8.0|64-bit|[.NET 8.0 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/8.0)|
|`net8.0 x86`|.NET 8.0|32-bit|[.NET 8.0 Desktop Runtime (x86)](https://dotnet.microsoft.com/download/dotnet/8.0)|
|`net10.0 x64`|.NET 10.0|64-bit|[.NET 10.0 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0)|
|`net10.0 x86`|.NET 10.0|32-bit|[.NET 10.0 Desktop Runtime (x86)](https://dotnet.microsoft.com/download/dotnet/10.0)|

> **Which one do I need?** If you are not sure, use **`net48 x64`** on 64-bit Windows 10/11:
> .NET Framework 4.8 is already part of Windows, so nothing extra has to be installed.
> For the `net8.0` / `net10.0` builds you need the **Desktop Runtime** of that .NET version;
> the plain ".NET Runtime" is not enough.

To check which .NET (8.0 / 10.0) runtimes are installed, open a command prompt and run:

```
dotnet --list-runtimes
```

Look for a `Microsoft.WindowsDesktop.App 8.x` or `10.x` line.



## Install

1. Download and extract the folder that matches your system (see the table above), e.g. `net48 x64`.
2. Copy the **whole folder** to a permanent location, for example:
   `D:\My Apps\Metaballs ScreenSaver\`
   The folder must contain all of its files; the screensaver won't start if any of them is missing:
   * `net48` builds: `MetaballsSS.scr`, `MetaballsSS.scr.config`
   * `net8.0` / `net10.0` builds: `MetaballsSS.scr`, `MetaballsSS.dll`, `MetaballsSS.deps.json`, `MetaballsSS.runtimeconfig.json`
3. Right-click `MetaballsSS.scr` and choose **Install**.
   The Windows **Screen Saver Settings** dialog opens with *MetaballsSS* selected.
4. Set the **Wait** time and click **OK**.

> If Windows blocks the file ("Windows protected your PC"), right-click `MetaballsSS.scr` →
> **Properties** → check **Unblock** → **OK**, then try again.



## Test it

* In **Screen Saver Settings**, click **Preview**, or
* double-click `MetaballsSS.scr`.

Glowing blobs drift slowly across a dark screen, like wax in a lava lamp.
When two blobs come close they melt together, then stretch apart again as they move away.
Press any key, click, or move the mouse to exit.

The small monitor in **Screen Saver Settings** shows a live, scaled-down preview.



## Settings

Open **Screen Saver Settings** (Start → type *screen saver*), select *MetaballsSS*, and click **Settings...**

|Setting|Range|Default|Description|
|-|-|-|-|
|**Color scheme**|Blue / Red|Blue|Color of the glowing blobs|
|**Number of blobs**|2 - 3|3|How many blobs float on the screen|
|**Speed**|0.25x - 3.00x|1.00x|How fast the blobs move|
|**Blob radius**|0.50x - 2.00x|1.00x|How big the blobs are|
|**Glow intensity**|0.25x - 3.00x|1.00x|How bright the blobs glow|

The preview at the bottom of the dialog shows your changes right away.
Click **OK** (or press **Enter**) to save your changes.
Click **Cancel**, press **Esc**, or close the window to discard them.
To see version and author information, right-click the title bar of the **Settings** window (or press **Alt+Space**) and choose **About...**

Settings are stored per user in the registry under `HKEY_CURRENT_USER\Software\Remus RIGO\MetaballsSS.VB-NET`.



## Multi-monitor

The screensaver runs on all monitors at once; each monitor shows its own blobs.
Any key, click, or mouse movement closes it on every monitor.



## Uninstall

1. In **Screen Saver Settings**, choose another screensaver (or *None*) and click **OK**.
2. Delete the folder you copied in step 2 of *Install*.
3. Optional: delete the registry key `HKEY_CURRENT_USER\Software\Remus RIGO\MetaballsSS.VB-NET`.
