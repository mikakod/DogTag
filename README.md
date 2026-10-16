# DogTag

A small, very fast, highly customizable desktop utility/widget that shows system info on your Windows computer.

It pins itself to wherever you want on the screen, ignores mouse clicks, and stays out of the way — kind of like a desktop system tag. It can sit on top of everything, blend in and hide behind your desktop shortcuts.

<table>
  <tr>
    <td align="center">
      <bExample</b><br>
      <img src="https://i.imgur.com/GbJuKS7.jpeg" width="400" alt="Preview">
    </td>
    <td align="center">
      <b>Settings</b><br>
      <img src="https://i.imgur.com/Enm940M.jpeg" width="400" alt="Settings">
    </td>
  </tr>
</table>


## What it shows
- Title (can be hidden)
- OS name and bitness (can be hidden)
- CPU model + core count and load
- RAM usage
- Disk usage
- Uptime
- Battery status (if present)
- GPU name, VRAM, usage and temperature (when available)

## Settings

Left-click the tray icon to open Settings. They are grouped into three sections:

**Display** — where the tag lives and how it layers:
- **Display over shortcuts** — render above desktop icons, or yield and move where no icon overlaps
- **Display over windows** — pin on top of everything (implies over shortcuts)
- **Binary units** — show GiB instead of GB for RAM, storage and VRAM
- **Monitor** — which screen to live on, plus a **Show VRAM usage** toggle
- **Anchor** — any screen corner, or **Custom**: unlock to drag it by its handle, nudge it with buttons/arrow keys, type exact coordinates, or center it
- **Mirror layout / Mirror Art-Logo** — flip the text block and the art/logos independently

**Text** — what the tag looks like:
- **Font Color Profile** — twelve color schemes
- **Font** — five inbox Windows fonts, no downloads needed
- **Text spacing** — Tight (values hug their labels) or Aligned columns
- **Text outline**, **Background opacity**, **Text opacity**
- **Art color** (picker) and **Art opacity** for logos
- **Show title(label)**, **Show OS line** — hide either line; the window shrinks to fit

**System** — how it runs:
- **Start with Windows** — autostart via registry
- **Refresh interval** — 500 ms to 10 s, default 2 s
- **Theme** — Follow Windows, Light, or Dark (follows the Windows dark style, easy-on-the-eyes grey text)

Also has a system tray icon: left-click for settings, right-click for the menu, **Enabled** toggle, and **Exit**.

## Release builds
- `DogTag.exe` -- framework-dependent (~0.3 MB). Requires the .NET 10 Desktop Runtime. Windows prompts to download if missing.
- `DogTag_standalone.exe` -- fully self-contained (~116 MB), no runtime needed.


Note: `Windows Forms` doesn't yet support trimming/AOT, so self-contained builds can't get much smaller than ~100 MB. If you ship yourself, expect either the large bundle or the tiny framework-dependent file plus a runtime dependency(.net Desktop 10 Runtime).

## Build & run
Requires [.NET 10 SDK](https://dotnet.microsoft.com/download) with Windows desktop workload.

Build:

```bash
cd final_source
dotnet build
./bin/Debug/net10.0-windows/DogTag.exe
```

## Commission
If you want a custom version tailored for your organization or team, with your own ASCII art/logo or specific sys info, reach out or open an issue on GitHub.

## Privacy
No usernames, hostnames, IPs, MAC addresses, or serial numbers are shown -- only hardware info.
Again, if you want a privately maintained and tailored version containing hostnames/IPs etc. feel free to reach out and I can cook something up.


## Author
Made by [Mikakod](https://github.com/mikakod)
