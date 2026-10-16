using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace DogTag
{
    public class DesktopHud : Form
    {
        private const string PawArt =
            @"
⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⠀⠀⠀⠀⠀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⢠⣾⣿⣿⣷⠀⠀⢠⣿⣿⣿⣦⡀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⣰⣿⣿⣿⣿⣿⡇⠀⣸⣿⣿⣿⣿⣿⡄⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⡇⠀⢿⣿⣿⣿⣿⣿⡟⠀⠀⠀⠀⠀
⠀⣠⣶⣦⡀⠹⣿⣿⣿⣿⡿⠁⠀⠘⣿⣿⣿⣿⣿⠇⣠⣴⣶⡄⠀
⢰⣿⣿⣿⣿⣆⠉⠛⠛⠋⠁⣀⣀⣀⠈⠛⠛⠛⠁⣼⣿⣿⣿⣿⡀
⢸⣿⣿⣿⣿⣿⡆⠀⢀⣴⣿⣿⣿⣿⣿⣦⠀⠀⢸⣿⣿⣿⣿⣿⡇
⠈⢿⣿⣿⣿⣿⠃⣠⣿⣿⣿⣿⣿⣿⣿⣿⣷⡀⢸⣿⣿⣿⣿⠟⠀
⠀⠀⠉⠙⠋⣡⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣦⣍⠛⠋⠁⠀⠀
⠀⠀⠀⠀⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣆⠀⠀⠀⠀
⠀⠀⠀⠀⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡟⠀⠀⠀⠀
⠀⠀⠀⠀⠈⠻⢿⣿⣿⣿⣿⡿⠿⢿⣿⣿⣿⣿⡿⠛⠁⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠈⠀⠀⠀⠀⠀⠀⠀⠁⠀⠀⠀⠀⠀⠀⠀⠀
";


        // Fox Mode easter-egg art (unlocked by typing "aethercore").
        private const string FoxArt =
            @"
 ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⡀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣾⠙⠻⢶⣄⡀⠀⠀⠀⢀⣤⠶⠛⠛⡇⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⣇⠀⠀⣙⣿⣦⣤⣴⣿⣁⠀⠀⣸⠇⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⣡⣾⣿⣿⣿⣿⣿⣿⣿⣷⣌⠋⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣴⣿⣷⣄⡈⢻⣿⡟⢁⣠⣾⣿⣦⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⣿⣿⣿⣿⠘⣿⠃⣿⣿⣿⣿⡏⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⠀⠈⠛⣰⠿⣆⠛⠁⠀⡀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣼⣿⣦⠀⠘⠛⠋⠀⣴⣿⠁⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣤⣶⣾⣿⣿⣿⣿⡇⠀⠀⠀⢸⣿⣏⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⣠⣶⣿⣿⣿⣿⣿⣿⣿⣿⠿⠿⠀⠀⠀⠾⢿⣿⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⣠⣿⣿⣿⣿⣿⣿⡿⠟⠋⣁⣠⣤⣤⡶⠶⠶⣤⣄⠈⠀⠀⠀⠀⠀⠀
⠀⠀⠀⢰⣿⣿⣮⣉⣉⣉⣤⣴⣶⣿⣿⣋⡥⠄⠀⠀⠀⠀⠉⢻⣄⠀⠀⠀⠀⠀
⠀⠀⠀⠸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣟⣋⣁⣤⣀⣀⣤⣤⣤⣤⣄⣿⡄⠀⠀⠀⠀
⠀⠀⠀⠀⠙⠿⣿⣿⣿⣿⣿⣿⣿⡿⠿⠛⠋⠉⠁⠀⠀⠀⠀⠈⠛⠃⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠉⠉⠉⠉⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
";



        private readonly Font pawFont = new Font("Consolas", 8f, FontStyle.Regular);
        // Art renders slightly smaller than full size so it sits inside the
        // window with breathing room. Applied uniformly to drawing (both
        // layers) and to window measurement, so nothing clips.
        private const float ArtScale = 0.85f;
        // All choices are inbox Windows fonts, so both the self-contained and
        // framework-dependent exe flavors work with no extra payload.
        private static readonly string[] FontOptions = { "Verdana", "Consolas", "Cascadia Mono", "Lucida Console", "Segoe UI" };
        private int fontChoice = 2;
        private Font textFont;

        private void ApplyFont()
        {
            string name = (fontChoice >= 0 && fontChoice < FontOptions.Length) ? FontOptions[fontChoice] : FontOptions[0];
            FontFamily ff = null;
            try
            {
                foreach (var f in FontFamily.Families)
                    if (string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase)) { ff = f; break; }
            }
            catch { }
            if (ff == null)
            {
                try { ff = new FontFamily("Verdana"); } catch { }
            }
            Font old = textFont;
            try { textFont = ff != null ? new Font(ff, 11f, FontStyle.Bold) : new Font(FontFamily.GenericSansSerif, 11f, FontStyle.Bold); }
            catch { if (old == null) textFont = SystemFonts.DefaultFont; else return; }
            try { old?.Dispose(); } catch { }
        }
        private readonly Brush pawBrush = new SolidBrush(Color.FromArgb(60, 64, 74));
        // Legacy per-line palette (whole line drawn in one color). Kept as
        // "Legacy Old" mode; the other profiles split label/value runs.
        private readonly Color[] colors =
        {
            Color.MediumPurple,
            Color.Cyan,
            Color.Firebrick,
            Color.DarkRed,
            Color.LimeGreen,
            Color.DodgerBlue,
            Color.Violet,
            Color.Orange,
            Color.Teal
        };

        // Text color profiles. 0 = Legacy (muted labels + per-component
        // values + vendor overrides + dynamic warns), 1 = Legacy Old (one
        // color per line), 2 = Slate, 3 = Dracula, 4 = Monochrome,
        // 5 = Cyberpunk/Synthwave, 6 = Catppuccin Mocha, 7 = Gruvbox Dark,
        // 8 = Tokyo Night, 9 = Matrix/Terminal, 10 = Solarized Dark,
        // 11 = Carbon/Stealth. The newer profiles use a small core scheme
        // with shade variations instead of a hue per line; warn colors are
        // kept everywhere so e.g. a 95% disk still reads as hot. No pure
        // white except Monochrome values.
        private sealed class TextPalette
        {
            public readonly Color Label;
            public readonly Color Title;
            public readonly Color Os;
            public readonly Color Cpu;
            public readonly Color Cores;
            public readonly Color Ram;
            public readonly Color Disk;
            public readonly Color Uptime;
            public readonly Color Gpu;
            public readonly Color MetricNormal;
            public readonly Color MetricWarm;
            public readonly Color MetricHot;
            public TextPalette(Color label, Color title, Color os, Color cpu, Color cores, Color ram, Color disk, Color uptime, Color gpu, Color metricNormal, Color metricWarm, Color metricHot)
            {
                Label = label; Title = title; Os = os; Cpu = cpu; Cores = cores; Ram = ram;
                Disk = disk; Uptime = uptime; Gpu = gpu;
                MetricNormal = metricNormal; MetricWarm = metricWarm; MetricHot = metricHot;
            }
        }

        private static readonly TextPalette DefaultPalette = new TextPalette(
            Color.FromArgb(0x9A, 0xA1, 0xAF),
            Color.FromArgb(0xF5, 0xF5, 0xF5),
            Color.FromArgb(0xE0, 0xE0, 0xE0),
            Color.FromArgb(0xFF, 0x9F, 0x1C),
            Color.FromArgb(0x64, 0xB5, 0xF6),
            Color.FromArgb(0x69, 0xF0, 0xAE),
            Color.FromArgb(0xCE, 0x93, 0xD8),
            Color.FromArgb(0x80, 0xCB, 0xC4),
            Color.FromArgb(0x76, 0xB9, 0x00),
            Color.FromArgb(0xE0, 0xE0, 0xE0),
            Color.FromArgb(0xFF, 0xB7, 0x4D),
            Color.FromArgb(0xEF, 0x53, 0x50));

        // Slate: slate labels, frost blue + teal + steel-blue values.
        private static readonly TextPalette SlatePalette = new TextPalette(
            Color.FromArgb(0x7B, 0x84, 0x94),
            Color.FromArgb(0xEC, 0xEF, 0xF4),
            Color.FromArgb(0x88, 0xC0, 0xD0),
            Color.FromArgb(0x88, 0xC0, 0xD0),
            Color.FromArgb(0x8F, 0xBC, 0xBB),
            Color.FromArgb(0x8F, 0xBC, 0xBB),
            Color.FromArgb(0x81, 0xA1, 0xC1),
            Color.FromArgb(0x81, 0xA1, 0xC1),
            Color.FromArgb(0x88, 0xC0, 0xD0),
            Color.FromArgb(0xD8, 0xDE, 0xE9),
            Color.FromArgb(0xEB, 0xCB, 0x8B),
            Color.FromArgb(0xBF, 0x61, 0x6A));

        // Dracula: comment-gray labels, violet + pink + cyan values.
        private static readonly TextPalette DraculaPalette = new TextPalette(
            Color.FromArgb(0x62, 0x72, 0xA4),
            Color.FromArgb(0xF8, 0xF8, 0xF2),
            Color.FromArgb(0xBD, 0x93, 0xF9),
            Color.FromArgb(0xBD, 0x93, 0xF9),
            Color.FromArgb(0xFF, 0x79, 0xC6),
            Color.FromArgb(0xFF, 0x79, 0xC6),
            Color.FromArgb(0x8B, 0xE9, 0xFD),
            Color.FromArgb(0x8B, 0xE9, 0xFD),
            Color.FromArgb(0xBD, 0x93, 0xF9),
            Color.FromArgb(0xF8, 0xF8, 0xF2),
            Color.FromArgb(0xFF, 0xB8, 0x6C),
            Color.FromArgb(0xFF, 0x55, 0x55));

        // Monochrome: dim gray labels, crisp white values.
        private static readonly TextPalette MonoPalette = new TextPalette(
            Color.FromArgb(0x80, 0x80, 0x80),
            Color.FromArgb(0xFF, 0xFF, 0xFF),
            Color.FromArgb(0xFF, 0xFF, 0xFF),
            Color.FromArgb(0xFF, 0xFF, 0xFF),
            Color.FromArgb(0xFF, 0xFF, 0xFF),
            Color.FromArgb(0xFF, 0xFF, 0xFF),
            Color.FromArgb(0xFF, 0xFF, 0xFF),
            Color.FromArgb(0xFF, 0xFF, 0xFF),
            Color.FromArgb(0xFF, 0xFF, 0xFF),
            Color.FromArgb(0xFF, 0xFF, 0xFF),
            Color.FromArgb(0xFF, 0xB7, 0x4D),
            Color.FromArgb(0xEF, 0x53, 0x50));

        // Cyberpunk / Synthwave: muted purple labels, glowing magenta +
        // electric cyan + neon orange values.
        private static readonly TextPalette CyberpunkPalette = new TextPalette(
            Color.FromArgb(0x6B, 0x5B, 0x7A),
            Color.FromArgb(0xFF, 0x2A, 0x6D),
            Color.FromArgb(0x05, 0xD9, 0xE8),
            Color.FromArgb(0xFF, 0x9E, 0x00),
            Color.FromArgb(0x05, 0xD9, 0xE8),
            Color.FromArgb(0xFF, 0x2A, 0x6D),
            Color.FromArgb(0xFF, 0x9E, 0x00),
            Color.FromArgb(0x05, 0xD9, 0xE8),
            Color.FromArgb(0xFF, 0x2A, 0x6D),
            Color.FromArgb(0xC8, 0xC8, 0xD8),
            Color.FromArgb(0xFF, 0x9E, 0x00),
            Color.FromArgb(0xFF, 0x2A, 0x6D));

        // Catppuccin (Mocha): overlay labels, lavender + peach + mint +
        // rosewater pastels.
        private static readonly TextPalette CatppuccinPalette = new TextPalette(
            Color.FromArgb(0x7F, 0x84, 0x9C),
            Color.FromArgb(0xF5, 0xE0, 0xDC),
            Color.FromArgb(0xB4, 0xBE, 0xFE),
            Color.FromArgb(0xFA, 0xB3, 0x87),
            Color.FromArgb(0xCB, 0xA6, 0xF7),
            Color.FromArgb(0xA6, 0xE3, 0xA1),
            Color.FromArgb(0xF5, 0xE0, 0xDC),
            Color.FromArgb(0x94, 0xE2, 0xD5),
            Color.FromArgb(0xB4, 0xBE, 0xFE),
            Color.FromArgb(0xCD, 0xD6, 0xF4),
            Color.FromArgb(0xF9, 0xE2, 0xAF),
            Color.FromArgb(0xF3, 0x8B, 0xA8));

        // Gruvbox Dark: gray labels, cream + burnt orange + green + muted
        // yellow values.
        private static readonly TextPalette GruvboxPalette = new TextPalette(
            Color.FromArgb(0xA8, 0x99, 0x84),
            Color.FromArgb(0xEB, 0xDB, 0xB2),
            Color.FromArgb(0xEB, 0xDB, 0xB2),
            Color.FromArgb(0xFE, 0x80, 0x19),
            Color.FromArgb(0xB8, 0xBB, 0x26),
            Color.FromArgb(0xB8, 0xBB, 0x26),
            Color.FromArgb(0xFA, 0xBD, 0x2F),
            Color.FromArgb(0x8E, 0xC0, 0x7C),
            Color.FromArgb(0xFE, 0x80, 0x19),
            Color.FromArgb(0xEB, 0xDB, 0xB2),
            Color.FromArgb(0xFA, 0xBD, 0x2F),
            Color.FromArgb(0xFB, 0x49, 0x34));

        // Tokyo Night: comment labels, violet + cyan + pink + amber values.
        private static readonly TextPalette TokyoPalette = new TextPalette(
            Color.FromArgb(0x56, 0x5F, 0x89),
            Color.FromArgb(0xC0, 0xCA, 0xF5),
            Color.FromArgb(0xBB, 0x9A, 0xF7),
            Color.FromArgb(0xFF, 0x9E, 0x64),
            Color.FromArgb(0x7D, 0xCF, 0xFF),
            Color.FromArgb(0x7D, 0xCF, 0xFF),
            Color.FromArgb(0xF7, 0x76, 0x8E),
            Color.FromArgb(0x7A, 0xA2, 0xF7),
            Color.FromArgb(0xBB, 0x9A, 0xF7),
            Color.FromArgb(0xC0, 0xCA, 0xF5),
            Color.FromArgb(0xE0, 0xAF, 0x68),
            Color.FromArgb(0xF7, 0x76, 0x8E));

        // Matrix / Terminal: forest-green labels, lime + emerald values.
        private static readonly TextPalette MatrixPalette = new TextPalette(
            Color.FromArgb(0x2F, 0x6B, 0x33),
            Color.FromArgb(0x00, 0xFF, 0x41),
            Color.FromArgb(0x00, 0xFF, 0x41),
            Color.FromArgb(0x00, 0xFF, 0x41),
            Color.FromArgb(0x33, 0xFF, 0x66),
            Color.FromArgb(0x33, 0xFF, 0x66),
            Color.FromArgb(0x00, 0xE6, 0x76),
            Color.FromArgb(0x00, 0xE6, 0x76),
            Color.FromArgb(0x00, 0xFF, 0x41),
            Color.FromArgb(0x00, 0xFF, 0x41),
            Color.FromArgb(0xFF, 0xD6, 0x00),
            Color.FromArgb(0xFF, 0x3D, 0x00));

        // Solarized Dark: muted teal labels, solarized accent values.
        private static readonly TextPalette SolarizedPalette = new TextPalette(
            Color.FromArgb(0x58, 0x6E, 0x75),
            Color.FromArgb(0xEE, 0xE8, 0xD5),
            Color.FromArgb(0x83, 0x94, 0x96),
            Color.FromArgb(0xB5, 0x89, 0x00),
            Color.FromArgb(0x2A, 0xA1, 0x98),
            Color.FromArgb(0x85, 0x99, 0x00),
            Color.FromArgb(0xCB, 0x4B, 0x16),
            Color.FromArgb(0x26, 0x8B, 0xD2),
            Color.FromArgb(0x2A, 0xA1, 0x98),
            Color.FromArgb(0x93, 0xA1, 0xA1),
            Color.FromArgb(0xB5, 0x89, 0x00),
            Color.FromArgb(0xDC, 0x32, 0x2F));

        // Carbon / Stealth: charcoal labels, off-white values, cobalt + red
        // reserved strictly for warn/hot alerts.
        private static readonly TextPalette CarbonPalette = new TextPalette(
            Color.FromArgb(0x5C, 0x5C, 0x5C),
            Color.FromArgb(0xE8, 0xE8, 0xE8),
            Color.FromArgb(0xED, 0xED, 0xED),
            Color.FromArgb(0xED, 0xED, 0xED),
            Color.FromArgb(0xED, 0xED, 0xED),
            Color.FromArgb(0xED, 0xED, 0xED),
            Color.FromArgb(0xED, 0xED, 0xED),
            Color.FromArgb(0xED, 0xED, 0xED),
            Color.FromArgb(0xED, 0xED, 0xED),
            Color.FromArgb(0xED, 0xED, 0xED),
            Color.FromArgb(0x2E, 0x6B, 0xFF),
            Color.FromArgb(0xFF, 0x22, 0x22));

        // Vendor overrides (Legacy profile only).
        private static readonly Color AmdRedColor = Color.FromArgb(0xED, 0x1C, 0x24);
        private static readonly Color IntelCpuColor = Color.FromArgb(0x00, 0x71, 0xC5);
        private static readonly Color IntelArcColor = Color.FromArgb(0x00, 0xB4, 0xFF);
        // 0 Legacy, 1 Legacy Old, 2 Slate, 3 Dracula, 4 Monochrome,
        // 5 Cyberpunk/Synthwave, 6 Catppuccin Mocha, 7 Gruvbox Dark,
        // 8 Tokyo Night, 9 Matrix/Terminal, 10 Solarized Dark, 11 Carbon.
        private int textColorProfile = 10;
        // Text spacing: 0 = Tight (default, values hug their labels),
        // 1 = Aligned columns (labels padded to a uniform column).
        private int textSpacing = 0;

        private static int MaxTextColorProfile = 11;

        private TextPalette ActivePalette()
        {
            switch (textColorProfile)
            {
                case 2: return SlatePalette;
                case 3: return DraculaPalette;
                case 4: return MonoPalette;
                case 5: return CyberpunkPalette;
                case 6: return CatppuccinPalette;
                case 7: return GruvboxPalette;
                case 8: return TokyoPalette;
                case 9: return MatrixPalette;
                case 10: return SolarizedPalette;
                case 11: return CarbonPalette;
                default: return DefaultPalette;
            }
        }

        // Split once: the art is constant, and these run several times per tick.
        private static readonly string[] PawLineCache = PawArt.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);
        private static readonly string[] FoxLineCache = FoxArt.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);

        private static string[] PawLines() => PawLineCache;

        private static string[] FoxLines() => FoxLineCache;

        private string[] ActivePawLines() => foxMode ? FoxLines() : PawLines();

        private string[] info = new string[0];

        // Fox Mode easter egg: unlocked by typing "aethercore" (any case)
        // into the Settings window. Settings-only by design: DogTag never
        // listens to keystrokes outside itself. Unlock persists; so does
        // the toggle.
        private bool foxMode = false;
        private bool foxUnlocked = false;
        private readonly List<char> codeBuffer = new List<char>();

        private static long lastGpuTick = 0; // TickCount64 ms; 0 = never polled

        private static bool cpuSampled;
        private static ulong lastKernel;
        private static ulong lastUser;
        private static ulong lastIdle;
        private static volatile int physCores, logCores;
        private static string gpuName = "";
        private static string gpuTotal = "";
        private static string gpuUsed = "";
        private static string gpuUtil = "";
        private static string gpuTemp = "";
        private static bool gpuHasVramInfo = false;
        // GPU vendor captured from the RAW adapter name before CleanGpuName
        // strips the vendor prefix: "NVIDIA" | "AMD" | "Intel" | "".
        private static string gpuVendor = "";
        private bool displayOverShortcuts;
        private bool displayOverWindows = false;
        private bool hudEnabled = true;
        private bool useBinaryUnits = true;
        private IntPtr placedParent = new IntPtr(-1);
        private IntPtr rawHandle = IntPtr.Zero;
        private IntPtr iconListView = IntPtr.Zero;
        private RawDesktopHudWindow rawSubWindow;
        private Rectangle placedBounds;
        private NotifyIcon tray;
        private ToolStripMenuItem enabledItem;
        private int refreshInterval = 2000;
        private int monitorIndex = 0;
        private bool showVramUsage = true;
        private bool showTextOutline = true;
        private bool showTitle = true;
        private bool showOsLine = true;
        private int anchorIndex = 1;
        // Custom anchor (4): free screen position set by drag or numeric entry.
        // Unset sentinel = fall back to bottom-right until the user moves it.
        private int customX = int.MinValue;
        private int customY = int.MinValue;
        // Locked by default: HUD is click-through and cannot be dragged.
        // Unlocked: HUD becomes hit-testable and reports HTCAPTION so the
        // user can drag it; position persists on drop.
        private bool customLocked = true;
        // True while a system caption-drag is in progress. UpdateInfo skips
        // repositioning during the modal move loop so it never fights the drag.
        private bool customDragActive = false;
        private long customDragStartTick = 0;
        private TopDragWindow rawTopDragWindow;
        private bool syncingBehindSibling = false;
        // Manual-drag fallback state (used when the WM_LBUTTONDOWN path owns the
        // drag instead of the HTCAPTION system move loop). Single mouse => one
        // active manual drag at a time.
        private IntPtr manualDragHwnd = IntPtr.Zero;
        private Point manualDragOffset;
        private bool nchHitLogged = false;
        // Floating drag-handle (a real borderless WinForms form shown while
        // Custom is unlocked). Raw popup HWNDs demonstrably never receive
        // button presses on this machine (hovers arrive, presses vanish), so
        // the handle is a full WinForms window with standard mouse handling,
        // and the HUD follows it. dragHandle mirrors the form's HWND for the
        // shared follow/measure math.
        private IntPtr dragHandle = IntPtr.Zero;
        private DragHandleForm handleForm;
        private const int DragHandleW = 190;
        private const int DragHandleH = 30;
        private const int DragHandleGap = 6;
        private int dragSessionHudW;
        private int dragSessionHudH;
        private bool dragHandleBelowHud;
        private bool mirrorLayout = false;
        // Mirror Art/Logo: flips only the paw/fox art to the opposite side,
        // text alignment always follows Mirror layout. Combined truth table
        // (art side): layout XOR art -> neither = left, either one = right,
        // both = left with right-aligned text.
        private bool mirrorArt = true;
        private bool IsArtMirrored() => mirrorLayout ^ mirrorArt;
        // Settings theme: 0 = Follow Windows, 1 = Light, 2 = Dark (default).
        // Rendered with WinForms native color modes (Application.SetColorMode)
        // so dark follows the Windows dark theme instead of a hand-rolled
        // palette (which left white title bars, combo buttons, sliders...).
        private int settingsTheme = 2;

        private static bool IsWindowsDarkMode()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false))
                {
                    if (key == null) return false;
                    object v = key.GetValue("AppsUseLightTheme");
                    if (v is int i) return i == 0;
                }
            }
            catch { }
            return false;
        }

        private static void ApplyAppColorMode(int theme)
        {
            try
            {
                if (theme == 2)
                    Application.SetColorMode(SystemColorMode.Dark);
                else if (theme == 1)
                    Application.SetColorMode(SystemColorMode.Classic);
                else
                    Application.SetColorMode(SystemColorMode.System);
            }
            catch { }
        }

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        // Dark title bar fallback (SetColorMode already handles this on
        // Win11, but Win10 needs the attribute explicitly). Tries the Win11
        // attribute id first (20), then the pre-20H1 id (19).
        private static bool TryEnableDarkTitleBar(IntPtr hwnd, bool dark)
        {
            if (hwnd == IntPtr.Zero) return false;
            try
            {
                int v = dark ? 1 : 0;
                int hr = DwmSetWindowAttribute(hwnd, 20, ref v, sizeof(int));
                if (hr != 0)
                    hr = DwmSetWindowAttribute(hwnd, 19, ref v, sizeof(int));
                return hr == 0;
            }
            catch { }
            return false;
        }
        private double windowOpacity = 0.5;
        private double textOpacity = 1.0;
        // ASCII-art tint (paw/fox) + art-specific opacity. RGB comes from the
        // color picker, alpha from the Art opacity slider; together = RGBA.
        private Color pawTint = Color.FromArgb(232, 232, 232);
        private double pawOpacity = 1.0;
        private string gpuSource = "";
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Timer zTimer;
        // Robust placement state: cached desktop hosts, visibility, reentrancy.
        // Single-HWND design: one top-level layered popup (proven visible via ULW).
        // Behind-icons is achieved by overlap-avoidance, NOT by parenting into the
        // shell desktop: probes proved no foreign child of Progman/WorkerW/DefView
        // ever composites on this system (plain red + layered ULW both invisible in
        // every slot), and a top-level window can never interleave between Progman
        // background and its icon children. So OFF = HUD yields to shortcuts: it is
        // placed where no icon overlaps (never covering shortcuts), ON = pinned.
        private IntPtr rawTop = IntPtr.Zero;
        private IntPtr rawBehind = IntPtr.Zero;
        private IntPtr rawBehindText = IntPtr.Zero;
        private RawDesktopHudWindow rawTextWindow;
        private int lastBgLwa = -1;
        private int lastTextLwa = -1;
        // Colorkey for the text layer: near-black, unused by HUD palette.
        private const uint TextKeyRef = (uint)(1 | (2 << 8) | (3 << 16));
        private static readonly Color TextKeyColor = Color.FromArgb(1, 2, 3);
        private IntPtr cachedWallpaperWorker = IntPtr.Zero;
        private IntPtr cachedIconHost = IntPtr.Zero;
        private IntPtr desktopDefView = IntPtr.Zero;
        private IntPtr desktopWallpaperWorker = IntPtr.Zero;
        private bool raisedDesktop;
        private long desktopCacheTick = 0;
        private Rectangle lastWallpaperBlit = Rectangle.Empty;
        private IntPtr lastWallpaperWorker = IntPtr.Zero;
        private List<Rectangle> cachedIconRects = new List<Rectangle>();
        private long iconCacheTick = 0;
        private List<string> cachedFixedDrives = new List<string>();
        private long driveCacheTick = 0;
        private static int s_iconDiag = 0;
        private static bool legacyBurnsCleared = false;
        private const int IconPad = 12;
        private const int ScreenMargin = 20;
        private bool rawShown;
        private bool updating;

        private int taskbarCreatedMsg;
        private const int GWL_STYLE = -16;
        private const int WS_CHILD = 0x40000000;
        private const int WS_POPUP_R = unchecked((int)0x80000000);

        public DesktopHud()
        {
            Text = "DogTag";
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            TopMost = false;
            BackColor = Color.FromArgb(20, 24, 32);
            DoubleBuffered = true;
            LoadSettings();
            // Display-over-windows implies over-shortcuts (it is topmost over all).
            if (displayOverWindows && !displayOverShortcuts) displayOverShortcuts = true;
            ApplyFont();
            AllowTransparency = false;
            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;
            Opacity = 1.0;
            HandleCreated += (_, _) =>
            {
                try { SetLayeredWindowAttributes(Handle, 0x00FF00FF, 0, 1); } catch { }
            };
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_LAYERED = 0x00080000;
                const int WS_EX_TRANSPARENT = 0x00000020;
                const int WS_EX_TOOLWINDOW = 0x00000080;
                const int WS_EX_NOACTIVATE = 0x08000000;
                var cp = base.CreateParams;
                cp.ExStyle |= WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE;
                return cp;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try { taskbarCreatedMsg = RegisterWindowMessage("TaskbarCreated"); } catch { taskbarCreatedMsg = 0; }
            EnsureRawHandle();
            var appIcon = MakeTrayIcon();
            Icon = appIcon;
            tray = new NotifyIcon
            {
                Icon = appIcon,
                Visible = true,
                Text = "DogTag"
            };
            var menu = new ContextMenuStrip();
            enabledItem = new ToolStripMenuItem("Enabled") { CheckOnClick = true, Checked = hudEnabled };
            enabledItem.CheckedChanged += (_, _) =>
            {
                hudEnabled = enabledItem.Checked;
                SaveSettings();
                if (hudEnabled) UpdateInfo();
                else
                {
                    try { if (rawTop != IntPtr.Zero && IsWindowVisible(rawTop)) ShowWindow(rawTop, 0); } catch { }
                    try { if (rawBehind != IntPtr.Zero && IsWindowVisible(rawBehind)) ShowWindow(rawBehind, 0); } catch { }
                    try { if (rawBehindText != IntPtr.Zero && IsWindowVisible(rawBehindText)) ShowWindow(rawBehindText, 0); } catch { }
                    rawShown = false;
                }
            };
            menu.Items.Add(enabledItem);
            menu.Items.Add("Settings", null, (_, _) => ShowSettings());
            menu.Items.Add("Exit", null, (_, _) => ExitApp());
            tray.ContextMenuStrip = menu;
            tray.MouseUp += (_, args) =>
            {
                if (args.Button == MouseButtons.Left)
                    ShowSettings();
            };
            Hide();
            // Prime the two blocking first-tick queries (core-count and GPU
            // subprocesses) on a worker so first paint isn't held up. Same
            // methods the tick calls, serialized by the same guards/locks;
            // if the worker loses the race the tick just does the work itself
            // (today's behavior). Touches fields only, never UI handles.
            try
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    try { EnsureCpuCores(); } catch { }
                    try { GetGpu(); } catch { }
                });
            }
            catch { }
            UpdateInfo();
            EnforceHudZOrder();
            timer = new System.Windows.Forms.Timer { Interval = refreshInterval };

            timer.Tick += (_, _) => UpdateInfo();
            timer.Start();

            zTimer = new System.Windows.Forms.Timer { Interval = 1500 };
            zTimer.Tick += (_, _) => EnforceHudZOrder();
            zTimer.Start();

        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            try { Hide(); } catch { }
        }

        protected override void WndProc(ref Message m)
        {
            // Explorer restart / display change: desktop hosts died, re-resolve without flicker.
            if (taskbarCreatedMsg != 0 && m.Msg == taskbarCreatedMsg)
            {
                InvalidateDesktopCache();
                try { cachedIconRects.Clear(); iconCacheTick = 0; } catch { }
                try { lastWallpaperWorker = IntPtr.Zero; lastWallpaperBlit = Rectangle.Empty; } catch { }
                placedParent = new IntPtr(-1);
                placedBounds = Rectangle.Empty;
                rawShown = rawShown && ((rawTop != IntPtr.Zero && IsWindow(rawTop) && IsWindowVisible(rawTop)) || (rawBehind != IntPtr.Zero && IsWindow(rawBehind) && IsWindowVisible(rawBehind)) || (rawBehindText != IntPtr.Zero && IsWindow(rawBehindText) && IsWindowVisible(rawBehindText)));
                try { UpdateInfo(); } catch { }
                try { EnforceHudZOrder(); } catch { }
            }
            else if (m.Msg == 0x007E /* WM_DISPLAYCHANGE */ || m.Msg == 0x001A /* WM_SETTINGCHANGE */)
            {
                // Display/wallpaper change may recreate WorkerW: drop bounds so next
                // UpdateInfo repositions via ULW once, and drop cache so behind-icons
                // mode re-resolves the fresh wallpaper WorkerW instead of a dead one.
                placedBounds = Rectangle.Empty;
                try { cachedIconRects.Clear(); iconCacheTick = 0; } catch { }
                try { lastWallpaperWorker = IntPtr.Zero; lastWallpaperBlit = Rectangle.Empty; } catch { }
                InvalidateDesktopCache();
            }
            else if (m.Msg == 0x0219 /* WM_DEVICECHANGE */)
            {
                // Drives may have arrived/left: re-enumerate on next tick.
                try { driveCacheTick = 0; } catch { }
            }
            base.WndProc(ref m);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
            if (zTimer != null)
            {
                zTimer.Stop();
                zTimer.Dispose();
                zTimer = null;
            }
            try { if (saveDebounceTimer != null) { saveDebounceTimer.Stop(); saveDebounceTimer.Dispose(); } } catch { }
            saveDebounceTimer = null;
            FlushSettings();
            if (tray != null)
            {
                tray.Visible = false;
                tray.Dispose();
            }
            try { rawTopDragWindow?.ReleaseHandle(); } catch { }
            rawTopDragWindow = null;
            try { if (handleForm != null && !handleForm.IsDisposed) handleForm.Close(); } catch { }
            handleForm = null;
            dragHandle = IntPtr.Zero;
            try { rawSubWindow?.ReleaseHandle(); } catch { }
            rawSubWindow = null;
            try { rawTextWindow?.ReleaseHandle(); } catch { }
            rawTextWindow = null;
            if (rawTop != IntPtr.Zero) try { DestroyWindow(rawTop); } catch { } rawTop = IntPtr.Zero;
            if (rawBehind != IntPtr.Zero) try { DestroyWindow(rawBehind); } catch { } rawBehind = IntPtr.Zero;
            if (rawBehindText != IntPtr.Zero) try { DestroyWindow(rawBehindText); } catch { } rawBehindText = IntPtr.Zero;
            try { RestoreWallpaperBlit(); } catch { }
            rawHandle = IntPtr.Zero;
            base.OnFormClosed(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Renderingen sker via UpdateLayeredWindow.
            base.OnPaint(e);
        }

        private void DrawHud(Graphics g, int bgAlpha, double textOpacity)        {
            if (bgAlpha <= 0)
                g.Clear(Color.FromArgb(0, 20, 24, 32));
            else
                g.Clear(Color.FromArgb(bgAlpha, 20, 24, 32));
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            string[] paw = ActivePawLines();

            int pawW = 0;
            foreach (var line in paw)
                if (line.Length > pawW) pawW = line.Length;

            int infoCount = 0;
            for (int i = 0; i < info.Length; i++)
                if (!string.IsNullOrWhiteSpace(info[i])) infoCount++;
            if (infoCount == 0) infoCount = 1;

            int pawLineHeight = pawFont.Height;
            int textLineHeight = Math.Max(16, textFont.Height);
            int pawBlockHeight = paw.Length * pawLineHeight;
            int infoBlockHeight = infoCount * textLineHeight;
            float pawScale = pawBlockHeight > 0 ? (float)infoBlockHeight / pawBlockHeight : 1f;
            if (pawScale > 1.0f) pawScale = 1.0f;
            pawScale *= ArtScale;

            float scaledPawHeight = pawBlockHeight * pawScale;
            int top = Math.Max(0, (infoBlockHeight - (int)Math.Round(scaledPawHeight)) / 2);

            // Mirror mode: paw moves to the right edge, text right-aligns to the
            // box end (glyph order unchanged). Box width from the clip bounds.
            // Mirror Art flips only the art side (text follows Mirror layout).
            float boxW = 0;
            try { boxW = g.VisibleClipBounds.Width; } catch { }
            bool mirror = mirrorLayout && boxW > 0;
            bool artRight = IsArtMirrored() && boxW > 0;
            float pawCharW = 8;
            try { pawCharW = g.MeasureString("M", pawFont).Width; } catch { }
            float pawPxW = pawW * pawCharW * ArtScale;

            // Skala tassen för att passa textblocket bakom etiketterna.
            var state = g.Save();
            g.TranslateTransform(artRight ? Math.Max(0, boxW - pawPxW) : 0, top);
            g.ScaleTransform(ArtScale, pawScale);
            // In child/behind mode brush alpha can be ignored by GDI text rendering;
            // blend the colour toward the panel background (as well as using alpha).
            // Base color is the Art tint picker RGB; alpha combines the global
            // text fade with the Art opacity slider.
            Color pawColor;
            if (bgAlpha >= 255)
                pawColor = BlendTowardPanel(Color.FromArgb(pawTint.R, pawTint.G, pawTint.B), textOpacity);
            else
                pawColor = Color.FromArgb(pawTint.R, pawTint.G, pawTint.B);
            using (var brush = new SolidBrush(Color.FromArgb((int)Math.Clamp(Math.Round(175 * textOpacity * pawOpacity), 0, 255), pawColor)))
                for (int i = 0; i < paw.Length; i++)
                    g.DrawString(paw[i], pawFont, brush, 0, i * pawLineHeight);
            g.Restore(state);

            // SingleBitPerPixelGridFit can ignore brush alpha on a layered child
            // window; AntiAliasGridFit keeps text-opacity alpha visible in behind mode.
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            int idx = 0;
            var drawnHud = BuiltSegments();
            var drawnHudSegs = new List<List<KeyValuePair<string, Color>>>();
            foreach (var kv in drawnHud) drawnHudSegs.Add(kv.segs);
            float maxLabelHud = textSpacing == 1 ? MaxLabelWidth(g, drawnHudSegs) : 0;
            foreach (var kv in drawnHud)
            {
                var segs = kv.segs;
                float pad = LabelPadForLine(g, segs, maxLabelHud);
                int alpha = bgAlpha >= 255
                    ? 255
                    : (int)Math.Round(textOpacity * 255, MidpointRounding.AwayFromZero);
                float tx = 2;
                if (mirror)
                {
                    try { tx = boxW - 2 - SegmentsWidth(g, segs, pad); } catch { }
                    if (tx < 0) tx = 0;
                }
                DrawSegmentsHud(g, segs, tx, idx * textLineHeight, alpha, bgAlpha, textOpacity, showTextOutline, pad);
                idx++;
            }
        }

        // Revised dual-layer behind mode. Both layers avoid alpha channels
        // entirely (GDI drops alpha on an HDC, which caused the black-box /
        // gradient muddiness last attempt):
        // - Panel: painted fully opaque, LWA global alpha = windowOpacity.
        // - Text: painted opaque on a colorkey background, LWA = COLORKEY|ALPHA
        //   with textOpacity. Key-colored pixels vanish, text gets its own
        //   independent opacity.
        private void DrawPanelOpaque(Graphics g)
        {
            g.Clear(Color.FromArgb(255, 20, 24, 32));
        }

        private static readonly Color PanelColor = Color.FromArgb(255, 20, 24, 32);

        private void DrawTextOnKey(Graphics g)
        {
            g.Clear(TextKeyColor);
            DrawTextGlyphs(g, false);
        }

        // Glyphs only (no background clear). smooth=true renders antialiased
        // against a panel-colored backing so edges match ON-mode quality; the
        // caller then keys out untouched panel pixels.
        private void DrawTextGlyphs(Graphics g, bool smooth)
        {
            g.TextRenderingHint = smooth
                ? System.Drawing.Text.TextRenderingHint.AntiAliasGridFit
                : System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            string[] paw = ActivePawLines();

            int pawW = 0;
            foreach (var line in paw)
                if (line.Length > pawW) pawW = line.Length;

            int infoCount = 0;
            for (int i = 0; i < info.Length; i++)
                if (!string.IsNullOrWhiteSpace(info[i])) infoCount++;
            if (infoCount == 0) infoCount = 1;

            int pawLineHeight = pawFont.Height;
            int textLineHeight = Math.Max(16, textFont.Height);
            int pawBlockHeight = paw.Length * pawLineHeight;
            int infoBlockHeight = infoCount * textLineHeight;
            float pawScale = pawBlockHeight > 0 ? (float)infoBlockHeight / pawBlockHeight : 1f;
            if (pawScale > 1.0f) pawScale = 1.0f;
            pawScale *= ArtScale;

            float scaledPawHeight = pawBlockHeight * pawScale;
            int top = Math.Max(0, (infoBlockHeight - (int)Math.Round(scaledPawHeight)) / 2);

            float pawCharW = 8;
            try { pawCharW = g.MeasureString("M", pawFont).Width; } catch { }
            float pawX = 0;
            if (IsArtMirrored())
            {
                try
                {
                    float bw = g.VisibleClipBounds.Width;
                    if (bw > 0) pawX = Math.Max(0, bw - pawW * pawCharW * ArtScale);
                }
                catch { }
            }
            var state = g.Save();
            g.TranslateTransform(pawX, top);
            g.ScaleTransform(ArtScale, pawScale);
            // Behind-icons layer is fully opaque (LWA carries the fade), so the
            // tint is pre-shaded to the old constant's depth (~0.68x).
            Color pawShade = Color.FromArgb(
                (int)Math.Round(Math.Clamp(pawTint.R * 0.68, 0, 255)),
                (int)Math.Round(Math.Clamp(pawTint.G * 0.68, 0, 255)),
                (int)Math.Round(Math.Clamp(pawTint.B * 0.68, 0, 255)));
            using (var brush = new SolidBrush(pawShade))
                for (int i = 0; i < paw.Length; i++)
                    g.DrawString(paw[i], pawFont, brush, 0, i * pawLineHeight);
            g.Restore(state);

            int idx = 0;
            var drawnGte = BuiltSegments();
            var drawnGteSegs = new List<List<KeyValuePair<string, Color>>>();
            foreach (var kv in drawnGte) drawnGteSegs.Add(kv.segs);
            float maxLabelGte = textSpacing == 1 ? MaxLabelWidth(g, drawnGteSegs) : 0;
            foreach (var kv in drawnGte)
            {
                float pad = LabelPadForLine(g, kv.segs, maxLabelGte);
                DrawSegmentsOpaque(g, kv.segs, MirrorTextXForSegments(g, kv.segs, kv.line, pad), idx * textLineHeight, showTextOutline, pad);
                idx++;
            }
        }

        private static Color BlendTowardPanel(Color c, double tOpacity)
        {
            double k = Math.Pow(tOpacity, 2.0);
            return Color.FromArgb(
                (int)Math.Clamp(Math.Round(20 + (c.R - 20) * k), 0, 255),
                (int)Math.Clamp(Math.Round(24 + (c.G - 24) * k), 0, 255),
                (int)Math.Clamp(Math.Round(32 + (c.B - 32) * k), 0, 255));
        }

        private static int ParseLeadingInt(string s)
        {
            if (string.IsNullOrEmpty(s)) return -1;
            var m = System.Text.RegularExpressions.Regex.Match(s, @"(\d+)");
            if (m.Success && int.TryParse(m.Groups[1].Value, out int v)) return v;
            return -1;
        }

        private Color MetricColorForUsage(string valueText)
        {
            var pal = ActivePalette();
            int v = ParseLeadingInt(valueText);
            if (v < 0) return pal.MetricNormal;
            if (v >= 85) return pal.MetricHot;
            if (v >= 60) return pal.MetricWarm;
            return pal.MetricNormal;
        }

        private Color MetricColorForTemp(string valueText)
        {
            var pal = ActivePalette();
            int v = ParseLeadingInt(valueText);
            if (v < 0) return pal.MetricNormal;
            if (v >= 80) return pal.MetricHot;
            if (v >= 65) return pal.MetricWarm;
            return pal.MetricNormal;
        }

        // VRAM "used/total" fraction, e.g. " 1 GiB/16 GiB". Storage-like
        // thresholds: warn only when actually filling up.
        private Color MetricColorForVram(string valueText)
        {
            var pal = ActivePalette();
            if (string.IsNullOrEmpty(valueText)) return pal.MetricNormal;
            var m = System.Text.RegularExpressions.Regex.Match(valueText, @"(\d+)\s*\S*\s*/\s*(\d+)");
            if (m.Success && double.TryParse(m.Groups[1].Value, out double used)
                && double.TryParse(m.Groups[2].Value, out double total) && total > 0)
            {
                double pct = 100.0 * used / total;
                if (pct >= 90) return pal.MetricHot;
                if (pct >= 75) return pal.MetricWarm;
                return pal.MetricNormal;
            }
            return pal.MetricNormal;
        }

        private Color PctWarnColor(int pct, Color baseColor)
        {
            var pal = ActivePalette();
            if (pct < 0) return baseColor;
            if (pct >= 90) return pal.MetricHot;
            if (pct >= 75) return pal.MetricWarm;
            return baseColor;
        }

        // Vendor sniffing for the default profile. Runs on the RAW adapter /
        // CPU string (vendor prefixes are stripped from the displayed text).
        private static string SniffGpuVendor(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "";
            string u = raw.ToUpperInvariant();
            if (u.Contains("NVIDIA") || u.Contains("GEFORCE") || u.Contains("QUADRO") || u.Contains("TESLA")) return "NVIDIA";
            if (u.Contains("AMD") || u.Contains("RADEON")) return "AMD";
            if (u.Contains("INTEL")) return "Intel";
            if (System.Text.RegularExpressions.Regex.IsMatch(raw, @"\bArc\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase)) return "Intel";
            return "";
        }

        private Color CpuColorForText(string cpuText)
        {
            var pal = ActivePalette();
            // Vendor overrides only in the Legacy profile; themed profiles
            // stay within their 2-3 color scheme.
            if (textColorProfile != 0) return pal.Cpu;
            if (string.IsNullOrEmpty(cpuText)) return pal.Cpu;
            if (cpuText.IndexOf("AMD", StringComparison.OrdinalIgnoreCase) >= 0) return AmdRedColor;
            if (cpuText.IndexOf("INTEL", StringComparison.OrdinalIgnoreCase) >= 0) return IntelCpuColor;
            return pal.Cpu;
        }

        private Color GpuNameColor()
        {
            var pal = ActivePalette();
            if (textColorProfile != 0) return pal.Gpu;
            if (gpuVendor == "AMD") return AmdRedColor;
            if (gpuVendor == "Intel") return IntelArcColor;
            if (gpuVendor == "NVIDIA") return pal.Gpu;
            // Stale cache fallback: sniff the (cleaned) displayed name.
            string n = gpuName ?? "";
            string u = n.ToUpperInvariant();
            if (u.Contains("GEFORCE") || u.Contains("QUADRO") || u.Contains("NVIDIA")) return pal.Gpu;
            if (u.Contains("RADEON") || u.Contains("AMD")) return AmdRedColor;
            if (u.Contains("INTEL") || u.Contains("ARC")) return IntelArcColor;
            return pal.Gpu;
        }

        private static int IndexOfWord(string haystack, string word)
        {
            if (string.IsNullOrEmpty(haystack) || string.IsNullOrEmpty(word)) return -1;
            return haystack.IndexOf(word, StringComparison.OrdinalIgnoreCase);
        }

        private List<KeyValuePair<string, Color>> BuildGpuSegments(string line)
        {
            var segs = new List<KeyValuePair<string, Color>>();
            var pal = ActivePalette();
            int colon = line.IndexOf(':');
            if (colon < 0)
            {
                segs.Add(new KeyValuePair<string, Color>(line, GpuNameColor()));
                return segs;
            }
            segs.Add(new KeyValuePair<string, Color>(line.Substring(0, colon + 1), pal.Label));
            string rest = line.Substring(colon + 1);
            // Markers are emitted with two leading spaces ("  VRAM", "  usage",
            // "  temp"); require the spaces so a GPU name containing e.g.
            // "temp" can never mis-split.
            int vramIdx = IndexOfWord(rest, "  VRAM");
            int usageIdx = IndexOfWord(rest, "  usage");
            int tempIdx = IndexOfWord(rest, "  temp");
            if (vramIdx < 0) vramIdx = IndexOfWord(rest, "VRAM");
            if (usageIdx < 0) usageIdx = IndexOfWord(rest, "usage");
            if (tempIdx < 0) tempIdx = IndexOfWord(rest, "temp");
            // Normalize to word starts so leading separator spaces stay with
            // the preceding run and chunks start at the label word.
            int vramWord = vramIdx >= 0 ? (rest.Substring(vramIdx).StartsWith("  ") ? vramIdx + 2 : vramIdx) : -1;
            int usageWord = usageIdx >= 0 ? (rest.Substring(usageIdx).StartsWith("  ") ? usageIdx + 2 : usageIdx) : -1;
            int tempWord = tempIdx >= 0 ? (rest.Substring(tempIdx).StartsWith("  ") ? tempIdx + 2 : tempIdx) : -1;
            int first = -1;
            if (vramWord >= 0) first = vramWord;
            if (usageWord >= 0 && (first < 0 || usageWord < first)) first = usageWord;
            if (tempWord >= 0 && (first < 0 || tempWord < first)) first = tempWord;
            if (first < 0)
            {
                segs.Add(new KeyValuePair<string, Color>(rest, GpuNameColor()));
                return segs;
            }
            segs.Add(new KeyValuePair<string, Color>(rest.Substring(0, first), GpuNameColor()));
            var markers = new List<(int idx, string kind)>();
            if (vramWord >= 0) markers.Add((vramWord, "VRAM"));
            if (usageWord >= 0) markers.Add((usageWord, "usage"));
            if (tempWord >= 0) markers.Add((tempWord, "temp"));
            markers.Sort((a, b) => a.idx.CompareTo(b.idx));
            for (int m = 0; m < markers.Count; m++)
            {
                int start = markers[m].idx;
                int end = (m + 1 < markers.Count) ? markers[m + 1].idx : rest.Length;
                if (start < 0 || end <= start || start >= rest.Length) continue;
                string chunk = rest.Substring(start, Math.Min(end - start, rest.Length - start));
                string kind = markers[m].kind;
                int labelLen = Math.Min(kind.Length, chunk.Length);
                string subLabel = chunk.Substring(0, labelLen);
                string subVal = chunk.Length > labelLen ? chunk.Substring(labelLen) : "";
                segs.Add(new KeyValuePair<string, Color>(subLabel, pal.Label));
                Color vc = pal.MetricNormal;
                if (kind.Equals("usage", StringComparison.OrdinalIgnoreCase))
                    vc = MetricColorForUsage(subVal);
                else if (kind.Equals("temp", StringComparison.OrdinalIgnoreCase))
                    vc = MetricColorForTemp(subVal);
                else if (kind.Equals("VRAM", StringComparison.OrdinalIgnoreCase))
                    vc = MetricColorForVram(subVal);
                segs.Add(new KeyValuePair<string, Color>(subVal, vc));
            }
            return segs;
        }

        private List<KeyValuePair<string, Color>> BuildLineSegments(string line, Color legacyFallback)
        {
            var segs = new List<KeyValuePair<string, Color>>();
            var pal = ActivePalette();
            if (textColorProfile == 1)
            {
                segs.Add(new KeyValuePair<string, Color>(line, legacyFallback));
                return segs;
            }
            if (line == "DogTag" || line == "FoxTag")
            {
                segs.Add(new KeyValuePair<string, Color>(line, pal.Title));
                return segs;
            }
            int colon = line.IndexOf(':');
            if (colon < 0)
            {
                segs.Add(new KeyValuePair<string, Color>(line, pal.Label));
                return segs;
            }
            string label = line.Substring(0, colon + 1);
            string rest = line.Substring(colon + 1);
            string key = label.Trim().TrimEnd(':').ToUpperInvariant();
            if (key == "GPU")
                return BuildGpuSegments(line);
            Color valueColor = pal.MetricNormal;
            switch (key)
            {
                case "OS": valueColor = pal.Os; break;
                case "CPU": valueColor = CpuColorForText(rest); break;
                case "CORES": valueColor = pal.Cores; break;
                case "RAM": valueColor = pal.Ram; break;
                case "DISK": valueColor = pal.Disk; break;
                case "UPTIME": valueColor = pal.Uptime; break;
                case "BAT": valueColor = pal.Uptime; break;
            }
            if (key == "CORES")
            {
                int loadIdx = rest.IndexOf("CPU load:", StringComparison.OrdinalIgnoreCase);
                if (loadIdx >= 0)
                {
                    string coresPart = rest.Substring(0, loadIdx);
                    string loadLabel = rest.Substring(loadIdx, "CPU load:".Length);
                    string loadVal = rest.Substring(loadIdx + "CPU load:".Length);
                    segs.Add(new KeyValuePair<string, Color>(label, pal.Label));
                    segs.Add(new KeyValuePair<string, Color>(coresPart, valueColor));
                    segs.Add(new KeyValuePair<string, Color>(loadLabel, pal.Label));
                    segs.Add(new KeyValuePair<string, Color>(loadVal, valueColor));
                    return segs;
                }
            }
            if (key == "RAM" || key == "DISK")
            {
                // Split trailing "(NN%)" so a nearly-full disk/RAM warns while
                // the main "used / total" stays in the component color.
                int paren = rest.LastIndexOf('(');
                int parenEnd = rest.LastIndexOf(')');
                if (paren >= 0 && parenEnd > paren)
                {
                    string main = rest.Substring(0, paren);
                    string pct = rest.Substring(paren, parenEnd - paren + 1);
                    string tail = (parenEnd + 1 < rest.Length) ? rest.Substring(parenEnd + 1) : "";
                    segs.Add(new KeyValuePair<string, Color>(label, pal.Label));
                    segs.Add(new KeyValuePair<string, Color>(main, valueColor));
                    segs.Add(new KeyValuePair<string, Color>(pct, PctWarnColor(ParseLeadingInt(pct), valueColor)));
                    if (!string.IsNullOrEmpty(tail))
                        segs.Add(new KeyValuePair<string, Color>(tail, valueColor));
                    return segs;
                }
            }
            segs.Add(new KeyValuePair<string, Color>(label, pal.Label));
            segs.Add(new KeyValuePair<string, Color>(rest, valueColor));
            return segs;
        }

        // Fixed tabular layout: value runs reserve their worst-case width
        // (percentages -> "100%", temps -> "100°", "used/total" fractions ->
        // used padded to the total's digits, uptime -> "888d 88h 88m") so a
        // growing number never pushes later runs or the window edge. Sizing
        // and drawing share RunAdvance, so geometry is identical in both and
        // constant across ticks: only glyph shapes change in place.
        // Static text (names, totals, labels) is unaffected by construction:
        // none of the patterns below match it.
        private static readonly Regex FracMaxRegex = new Regex(@"(\d+)([^0-9]*\/[^0-9]*)(\d+)", RegexOptions.Compiled);
        private static readonly Regex PctDegMaxRegex = new Regex(@"(\d+)(\s*)(%|°)", RegexOptions.Compiled);
        private static readonly Regex UptimeMaxRegex = new Regex(@"(\d+)([dhm])(?![A-Za-z])", RegexOptions.Compiled);

        private static string MaxedRun(string run)
        {
            if (string.IsNullOrEmpty(run)) return run;
            try
            {
                string s = FracMaxRegex.Replace(run, m => new string('8', m.Groups[3].Value.Length) + m.Groups[2].Value + m.Groups[3].Value);
                s = PctDegMaxRegex.Replace(s, m => "100" + m.Groups[2].Value + m.Groups[3].Value);
                s = UptimeMaxRegex.Replace(s, m => new string('8', m.Groups[2].Value == "d" ? 3 : 2) + m.Groups[2].Value);
                return s;
            }
            catch { return run; }
        }

        private float RunAdvance(Graphics g, string run)
        {
            if (string.IsNullOrEmpty(run)) return 0;
            try
            {
                float w = g.MeasureString(run, textFont).Width;
                try
                {
                    string mx = MaxedRun(run);
                    if (!string.Equals(mx, run, StringComparison.Ordinal))
                    {
                        float mw = g.MeasureString(mx, textFont).Width;
                        if (mw > w) w = mw;
                    }
                }
                catch { }
                return w;
            }
            catch { return 0; }
        }

        private float SegmentsWidth(Graphics g, List<KeyValuePair<string, Color>> segs, float labelPad = 0)
        {
            float w = labelPad;
            foreach (var s in segs)
                w += RunAdvance(g, s.Key);
            return w;
        }

        // "Aligned columns" text spacing: labels ("OS:", "Cores:",
        // "Uptime:", ...) have different widths, so padding every label to
        // the widest one lines values up in a straight column (normal mode)
        // or mirrors exactly (mirrored mode). "Tight" spacing (default)
        // skips the padding so values hug their labels.
        // Title lines (single run, no colon) are not labels: pad 0.
        private float LabelPadForLine(Graphics g, List<KeyValuePair<string, Color>> segs, float maxLabelW)
        {
            if (segs == null || segs.Count <= 1 || maxLabelW <= 0) return 0;
            try
            {
                float pad = maxLabelW - RunAdvance(g, segs[0].Key);
                return pad > 0 ? pad : 0;
            }
            catch { return 0; }
        }

        private float MaxLabelWidth(Graphics g, List<List<KeyValuePair<string, Color>>> allSegs)
        {
            float m = 0;
            try
            {
                foreach (var segs in allSegs)
                {
                    if (segs == null || segs.Count <= 1) continue;
                    float w = RunAdvance(g, segs[0].Key);
                    if (w > m) m = w;
                }
            }
            catch { }
            return m;
        }

        // Per-tick segment cache: measure + both draw layers previously
        // rebuilt identical segment lists 2-3x per tick. UpdateInfo rebuilds
        // this right after replacing `info`, so any draw in the same tick
        // reuses it; the reference guard keeps stray calls correct.
        private string[] segCacheInfo = null;
        private List<(int idx, string line, List<KeyValuePair<string, Color>> segs)> segCache = null;

        private List<(int idx, string line, List<KeyValuePair<string, Color>> segs)> BuiltSegments()
        {
            try
            {
                if (segCache != null && info != null && ReferenceEquals(segCacheInfo, info))
                    return segCache;
            }
            catch { }
            var list = new List<(int idx, string line, List<KeyValuePair<string, Color>> segs)>();
            try
            {
                var src = info;
                if (src != null)
                {
                    for (int i = 0; i < src.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(src[i])) continue;
                        try { list.Add((i, src[i], BuildLineSegments(src[i], colors[i % colors.Length]))); } catch { }
                    }
                }
            }
            catch { }
            return list;
        }

        private float MirrorTextXForSegments(Graphics g, List<KeyValuePair<string, Color>> segs, string fullLine, float labelPad = 0)
        {
            if (!mirrorLayout) return 2;
            try
            {
                float w = g.VisibleClipBounds.Width;
                if (w > 0)
                {
                    float tw = SegmentsWidth(g, segs, labelPad);
                    if (tw <= 0) tw = g.MeasureString(fullLine, textFont).Width;
                    return Math.Max(0, w - 2 - tw);
                }
            }
            catch { }
            return 2;
        }

        private void DrawSegmentsHud(Graphics g, List<KeyValuePair<string, Color>> segs, float tx, float y, int alpha, int bgAlpha, double tOpacity, bool withOutline, float labelPad = 0)
        {
            // Precompute run origins so outline + foreground share identical
            // positions. Drawing the outline as one whole-line string does NOT
            // align with multi-run foreground (GDI+ kerns them differently),
            // which is what made it look like an offset shadow.
            // labelPad ("Aligned columns" spacing) goes right after the label run.
            var xs = new float[segs.Count];
            float x = tx;
            for (int i = 0; i < segs.Count; i++)
            {
                xs[i] = x;
                x += RunAdvance(g, segs[i].Key);
                if (i == 0) x += labelPad;
            }
            if (withOutline)
            {
                using (var ob = new SolidBrush(Color.FromArgb(alpha, Color.FromArgb(8, 10, 14))))
                {
                    for (int d = 0; d < 8; d++)
                    {
                        float dx = (d % 3) - 1, dy = (d / 3) - 1;
                        if (dx == 0 && dy == 0) continue;
                        for (int i = 0; i < segs.Count; i++)
                        {
                            if (string.IsNullOrEmpty(segs[i].Key)) continue;
                            g.DrawString(segs[i].Key, textFont, ob, xs[i] + dx, y + dy);
                        }
                    }
                }
            }
            for (int i = 0; i < segs.Count; i++)
            {
                if (string.IsNullOrEmpty(segs[i].Key)) continue;
                Color adj = bgAlpha >= 255 ? BlendTowardPanel(segs[i].Value, tOpacity) : segs[i].Value;
                using (var b = new SolidBrush(Color.FromArgb(alpha, adj)))
                    g.DrawString(segs[i].Key, textFont, b, xs[i], y);
            }
        }

        private void DrawSegmentsOpaque(Graphics g, List<KeyValuePair<string, Color>> segs, float x0, float y, bool withOutline, float labelPad = 0)
        {
            var xs = new float[segs.Count];
            float x = x0;
            for (int i = 0; i < segs.Count; i++)
            {
                xs[i] = x;
                x += RunAdvance(g, segs[i].Key);
                if (i == 0) x += labelPad;
            }
            if (withOutline)
            {
                using (var ob = new SolidBrush(Color.FromArgb(255, 8, 10, 14)))
                {
                    for (int d = 0; d < 8; d++)
                    {
                        float dx = (d % 3) - 1, dy = (d / 3) - 1;
                        if (dx == 0 && dy == 0) continue;
                        for (int i = 0; i < segs.Count; i++)
                        {
                            if (string.IsNullOrEmpty(segs[i].Key)) continue;
                            g.DrawString(segs[i].Key, textFont, ob, xs[i] + dx, y + dy);
                        }
                    }
                }
            }
            for (int i = 0; i < segs.Count; i++)
            {
                if (string.IsNullOrEmpty(segs[i].Key)) continue;
                using (var b = new SolidBrush(segs[i].Value))
                    g.DrawString(segs[i].Key, textFont, b, xs[i], y);
            }
        }



        private void UpdateInfo()
        {
            if (updating) return;
            // Never fight an in-progress caption drag: the modal move loop owns
            // the window position until WM_EXITSIZEMOVE clears the flag.
            // Watchdog: a missed EXIT/UP must never freeze refreshes forever.
            if (customDragActive)
            {
                try
                {
                    if ((Environment.TickCount64 - customDragStartTick) > 30000)
                        customDragActive = false;
                    else
                        return;
                }
                catch { return; }
            }
            updating = true;
            try
            {
                EnsureHandles();
                if (!hudEnabled)
                {
                    try { if (rawTop != IntPtr.Zero && IsWindowVisible(rawTop)) ShowWindow(rawTop, 0); } catch { }
                    try { if (rawBehind != IntPtr.Zero && IsWindowVisible(rawBehind)) ShowWindow(rawBehind, 0); } catch { }
                    try { if (rawBehindText != IntPtr.Zero && IsWindowVisible(rawBehindText)) ShowWindow(rawBehindText, 0); } catch { }
                    RestoreWallpaperBlit();
                    rawShown = false;
                    try { HideDragHandle(); } catch { }
                    return;
                }
                var arr = BuildInfo();
                info = arr;
                segCacheInfo = arr;
                try
                {
                    var fresh = new List<(int idx, string line, List<KeyValuePair<string, Color>> segs)>();
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(arr[i])) continue;
                        try { fresh.Add((i, arr[i], BuildLineSegments(arr[i], colors[i % colors.Length]))); } catch { }
                    }
                    segCache = fresh;
                }
                catch { segCache = null; }

                using (var g = CreateGraphics())
                {
                    string[] paw = ActivePawLines();

                    int pawW = 0;
                    foreach (var line in paw) if (line.Length > pawW) pawW = line.Length;

                    // Pinned geometry: measure with worst-case run slots
                    // (RunAdvance), so the width is identical on every tick.
                    // Static text (names, totals, labels) is exact; only
                    // fluctuating numbers carry small fixed slack. The window
                    // therefore only ever re-sizes on structural change: font,
                    // profile, units, monitor, anchor, line visibility toggles,
                    // or lines appearing.
                    int contentW = 12;
                    var measured = BuiltSegments();
                    var measuredSegs = new List<List<KeyValuePair<string, Color>>>();
                    foreach (var kv in measured) measuredSegs.Add(kv.segs);
                    float maxLabelMeas = textSpacing == 1 ? MaxLabelWidth(g, measuredSegs) : 0;
                    foreach (var kv in measured)
                    {
                        try
                        {
                            float sw = SegmentsWidth(g, kv.segs, LabelPadForLine(g, kv.segs, maxLabelMeas));
                            if (sw > 0) contentW = Math.Max(contentW, (int)Math.Ceiling(sw) + 16);
                        }
                        catch { try { contentW = Math.Max(contentW, (int)Math.Ceiling(g.MeasureString(kv.line, textFont).Width) + 16); } catch { } }
                    }

                    int w = Math.Max(contentW, (int)Math.Ceiling(pawW * g.MeasureString("M", pawFont).Width * ArtScale) + 16);
                    int infoCount = 0;
                    foreach (var l in arr) if (!string.IsNullOrWhiteSpace(l)) infoCount++;
                    if (infoCount == 0) infoCount = 1;
                    int textLineHeight = Math.Max(16, textFont.Height);
                    int h = infoCount * textLineHeight;
                    var screens = Screen.AllScreens;
                    int mi = Math.Clamp(monitorIndex, 0, screens.Length - 1);
                    var area = screens[mi].WorkingArea;
                    // Anchor: 0 = bottom-right, 1 = bottom-left (default),
                    // 2 = top-right, 3 = top-left, 4 = Custom (free position).
                    int ai = Math.Clamp(anchorIndex, 0, 4);
                    int x, y;
                    if (ai == 4)
                    {
                        var customPos = ResolveCustomPos(w, h);
                        x = customPos.X; y = customPos.Y;
                    }
                    else
                    {
                        x = (ai == 1 || ai == 3) ? area.Left + 20 : area.Right - w - 20;
                        y = (ai >= 2) ? area.Top + 20 : area.Bottom - h - 20;
                    }
                    if (IconsYieldMode())
                    {
                        try
                        {
                            var spot = FindNonOverlappingSpot(new Rectangle(x, y, w, h), w, h);
                            if (spot.X != x || spot.Y != y)
                            {
                                try
                                {
                                    string lg = Path.Combine(Path.GetTempPath(), "dogtag_spot.log");
                                    string prev = "";
                                    try { if (File.Exists(lg)) prev = File.ReadAllText(lg); } catch { }
                                    string line = DateTime.Now.ToString("HH:mm:ss") + " yield " + x + "," + y + "->" + spot.X + "," + spot.Y + Environment.NewLine;
                                    if (!prev.EndsWith(line)) File.AppendAllText(lg, line);
                                }
                                catch { }
                            }
                            x = spot.X; y = spot.Y;
                        }
                        catch { }
                    }
                    PlaceOnDesktop(x, y, w, h);
                    if (hudEnabled)
                    {
                        if (WantChild())
                        PresentBehindIcons(x, y, w, h);
                        else
                        {
                            string pk = BuildPresentKey(arr, x, y, w, h);
                            if (!ShouldSkipPresent(arr, pk, x, y, w, h))
                                PresentOverShortcuts(x, y, w, h, pk);
                            // Skipped: identical frame already on screen; nothing to do.
                        }
                    }
                    // Grip follows the freshly placed HUD (no-op unless Custom
                    // is unlocked). Skipped implicitly mid-drag: UpdateInfo
                    // returns early while customDragActive is set.
                    try { UpdateDragHandlePlacement(x, y, w, h); } catch { }
                }
            }
            finally { updating = false; }
        }

        private string[] BuildInfo()
        {
            var lines = new List<string>();
            if (showTitle)
                lines.Add(foxMode ? "FoxTag" : "DogTag");

            string cpu = GetCpuName();
            if (showOsLine)
                lines.Add($"OS: {GetOSName()}  {(Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit")}");
            lines.Add($"CPU: {cpu.Substring(0, Math.Min(44, cpu.Length))}");

            EnsureCpuCores();
            double load = GetCpuLoad();
            lines.Add($"Cores: {physCores}c/{logCores}t   CPU load: {load:F0}%");

            var mem = new MEMORYSTATUSEX { dwLength = (uint)MemStatusSize };
            if (GlobalMemoryStatusEx(ref mem))
            {
                ulong used = mem.ullTotalPhys - mem.ullAvailPhys;
                double pct = 100.0 * used / mem.ullTotalPhys;
                lines.Add($"RAM: {FormatBytes(used)} / {FormatBytes(mem.ullTotalPhys)} ({pct:F0}%)");
            }

            try
            {
                // Enumeration is the expensive part; per-tick we only need
                // fresh sizes. Cache the fixed-drive set (60s TTL, dropped
                // on WM_DEVICECHANGE); vanished drives are skipped below.
                long nowDrv = Environment.TickCount64;
                if (driveCacheTick == 0 || nowDrv - driveCacheTick > 60000 || cachedFixedDrives == null || cachedFixedDrives.Count == 0)
                {
                    var fresh = new List<string>();
                    try
                    {
                        foreach (var d in DriveInfo.GetDrives())
                        {
                            try { if (d.DriveType == DriveType.Fixed) fresh.Add(d.Name); } catch { }
                        }
                    }
                    catch { }
                    if (fresh.Count > 0) { cachedFixedDrives = fresh; driveCacheTick = nowDrv; }
                }
                long total = 0, usedDisk = 0;
                var driveNames = cachedFixedDrives;
                if (driveNames != null)
                {
                    foreach (var name in driveNames)
                    {
                        try
                        {
                            var drv = new DriveInfo(name);
                            if (drv.IsReady && drv.DriveType == DriveType.Fixed)
                            {
                                total += drv.TotalSize;
                                usedDisk += drv.TotalSize - drv.AvailableFreeSpace;
                            }
                        }
                        catch { }
                    }
                }
                double pct = total > 0 ? 100.0 * usedDisk / total : 0;
                lines.Add($"Disk: {FormatBytes((ulong)usedDisk)} / {FormatBytes((ulong)total)} ({pct:F0}%)");
            }
            catch { }

            TimeSpan up = TimeSpan.FromMilliseconds(Environment.TickCount64);
            lines.Add($"Uptime: {up.Days}d {up.Hours}h {up.Minutes}m");

            try
            {
                var ps = SystemInformation.PowerStatus;
                if (ps.BatteryChargeStatus != BatteryChargeStatus.NoSystemBattery)
                    lines.Add($"Bat: {(ps.BatteryLifePercent * 100):F0}% {(ps.PowerLineStatus == PowerLineStatus.Online ? "plugged" : "battery")}");
            }
            catch { }

            GetGpu();
            if (!string.IsNullOrWhiteSpace(gpuName))
            {
                var sb = new System.Text.StringBuilder(gpuName);
                if (gpuHasVramInfo)
                {
                    if (showVramUsage)
                        sb.Append($"  VRAM {gpuUsed}/{gpuTotal}");
                    else
                        sb.Append($"  VRAM {gpuTotal}");
                }
                else
                    sb.Append("  VRAM n/a (WMI)");
                if (!string.IsNullOrWhiteSpace(gpuUtil))
                    sb.Append($"  usage {gpuUtil}");
                if (!string.IsNullOrWhiteSpace(gpuTemp))
                    sb.Append($"  temp {gpuTemp}°");
                lines.Add($"GPU: {sb}");
            }

            return lines.ToArray();
        }

        // OS/CPU names never change at runtime: query once, not every tick.
        private static string cachedOsName = null;
        private static string cachedCpuName = null;

        private static string GetOSName()
        {
            if (cachedOsName == null) cachedOsName = QueryOsName();
            return cachedOsName;
        }

        private static string GetCpuName()
        {
            if (cachedCpuName == null) cachedCpuName = QueryCpuName();
            return cachedCpuName;
        }

        private static string QueryOsName()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    var name = key?.GetValue("ProductName") as string;
                    var build = key?.GetValue("CurrentBuild") as string;
                    if (!string.IsNullOrWhiteSpace(name) && int.TryParse(build, out int buildNo) && buildNo >= 22000 && name.IndexOf("Windows 10", StringComparison.OrdinalIgnoreCase) >= 0)
                        return name.Replace("Windows 10", "Windows 11", StringComparison.OrdinalIgnoreCase);
                    return name;
                }
            }
            catch { }
            return Environment.OSVersion.VersionString;
        }

        private static string QueryCpuName()
        {
            string raw = null;
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0"))
                    raw = (key?.GetValue("ProcessorNameString") as string)?.Trim();
            }
            catch { }
            if (string.IsNullOrWhiteSpace(raw))
                raw = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");
            return CleanCpuName(raw);
        }

        // The line already says "CPU:": drop the redundant "Processor" and
        // the core-count suffix ("8-Core", "16 Cores") — the Cores line shows
        // "8c/16t". Only count-form "Core(s)" is stripped, so brand names
        // ("Core i7", "7800X3D") and vendors ("AMD", "Intel") survive.
        private static string CleanCpuName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "";
            string n = name.Trim();
            n = Regex.Replace(n, @"\d+\s*-?\s*Cores?\b", "", RegexOptions.IgnoreCase);
            n = Regex.Replace(n, @"\bProcessors?\b", "", RegexOptions.IgnoreCase);
            n = Regex.Replace(n, @"\s{2,}", " ");
            return n.Trim().Trim('-', ':', ' ');
        }

        private static void EnsureCpuCores()
        {
            if (logCores != 0) return;
            try
            {
                var psi = new ProcessStartInfo("powershell", "-NoProfile -Command \"$cpu=Get-CimInstance Win32_Processor; Write-Output ($cpu | Measure-Object NumberOfCores -Sum).Sum; Write-Output ($cpu | Measure-Object NumberOfLogicalProcessors -Sum).Sum\"")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                using (var p = Process.Start(psi))
                {
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit(3000);
                    var nums = new List<int>();
                    foreach (var line in output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (int.TryParse(line.Trim(), out int value))
                            nums.Add(value);
                    }
                    if (nums.Count >= 2)
                    {
                        physCores = nums[0];
                        logCores = nums[1];
                    }
                }
            }
            catch { }
            if (physCores == 0) physCores = Environment.ProcessorCount;
            if (logCores == 0) logCores = Environment.ProcessorCount;
        }

        private static double GetCpuLoad()
        {
            FILETIME idle, kernel, user;
            if (!GetSystemTimes(out idle, out kernel, out user)) return 0;
            var uIdle = idle.ToUInt64();
            var uKernel = kernel.ToUInt64();
            var uUser = user.ToUInt64();
            if (!cpuSampled)
            {
                lastIdle = uIdle; lastKernel = uKernel; lastUser = uUser;
                cpuSampled = true;
                return 0;
            }
            var sys = (uKernel + uUser) - (lastKernel + lastUser);
            var idleDelta = uIdle - lastIdle;
            double load = 0;
            if (sys > 0)
                load = 100.0 * (sys - idleDelta) / sys;
            lastIdle = uIdle;
            lastKernel = uKernel;
            lastUser = uUser;
            return Math.Clamp(load, 0, 100);
        }

        private static readonly int MemStatusSize = Marshal.SizeOf(typeof(MEMORYSTATUSEX));

        private string UnitLabel() => useBinaryUnits ? "GiB" : "GB";

        private string FormatBytes(ulong bytes)
        {
            double denom = useBinaryUnits ? 1024.0 * 1024 * 1024 : 1000.0 * 1000 * 1000;
            return ((long)Math.Round(bytes / denom, MidpointRounding.AwayFromZero)).ToString() + " " + UnitLabel();
        }

        private bool gpuUsedBinary;
        private static readonly object gpuLock = new object();
        private static long gpuStaticTick = 0; // last static (name/total) resolve
        private static bool nvidiaSmiWorks = true;
        private static long nvidiaRetryTick = 0;

        // Tiered GPU polling: the 4s cadence only needs dynamic values (used
        // VRAM, utilization, temp). Static data (adapter name, total VRAM) is
        // cached long-term, and each subprocess backend is retried with
        // backoff instead of every poll — previously a missing nvidia-smi
        // meant a PowerShell spawn every 4 seconds. Serialized: also called
        // from the startup priming thread (see OnLoad).
        private string GetGpu()
        {
            if (lastGpuTick != 0 && Environment.TickCount64 - lastGpuTick < 4000 && gpuUsedBinary == useBinaryUnits)
                return gpuName;
            lock (gpuLock)
            {
                if (lastGpuTick != 0 && Environment.TickCount64 - lastGpuTick < 4000 && gpuUsedBinary == useBinaryUnits)
                    return gpuName;
                long now = Environment.TickCount64;
                bool unitsChanged = gpuUsedBinary != useBinaryUnits;
                bool dynOk = false;
                // NVIDIA: one call yields static + dynamic. Only skip when a
                // previous attempt proved it missing (retry every 60s).
                if (unitsChanged || nvidiaSmiWorks || nvidiaRetryTick == 0 || now - nvidiaRetryTick > 60000)
                {
                    if (TryNvidiaSmi()) { dynOk = true; nvidiaSmiWorks = true; gpuStaticTick = now; }
                    else
                    {
                        nvidiaSmiWorks = false;
                        nvidiaRetryTick = now;
                        // Never present frozen dynamics as live: drop them,
                        // keep the static name/total.
                        gpuUsed = "";
                        gpuUtil = "";
                        gpuTemp = "";
                    }
                }
                if (!dynOk)
                {
                    // WMI fallback resolves only the static name; the dynamic
                    // slots stay as they were (empty on this path).
                    if (string.IsNullOrEmpty(gpuName) && (gpuStaticTick == 0 || now - gpuStaticTick > 300000))
                    {
                        gpuStaticTick = now;
                        TryWmiGpuName();
                    }
                }
                gpuUsedBinary = useBinaryUnits;
                lastGpuTick = now;
                return gpuName;
            }
        }

        private bool TryNvidiaSmi()
        {
            try
            {
                var psi = new ProcessStartInfo("nvidia-smi", "--query-gpu=name,memory.total,memory.used,utilization.gpu,temperature.gpu --format=csv,noheader")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                using (var p = Process.Start(psi))
                {
                    string output = p.StandardOutput.ReadToEnd();
                    if (!p.WaitForExit(3000)) p.Kill();
                    if (p.ExitCode == 0 && !string.IsNullOrWhiteSpace(output))
                    {
                        var first = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)[0];
                        var parts = first.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        if (parts.Length >= 5)
                        {
                            gpuName = CleanGpuName(parts[0]);
                            gpuTotal = FormatVram(parts[1]);
                            gpuUsed = FormatVram(parts[2]);
                            gpuUtil = parts[3];
                            gpuTemp = parts[4];
                            gpuHasVramInfo = true;
                            gpuSource = "nvidia-smi";
                            gpuVendor = "NVIDIA";
                            return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        private void TryWmiGpuName()
        {
            try
            {
                using (var p = Process.Start(new ProcessStartInfo("powershell", "-NoProfile -Command \"(Get-CimInstance Win32_VideoController | Select-Object -First 1).Name\"") { RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true }))
                {
                    string name = p.StandardOutput.ReadToEnd().Trim();
                    p.WaitForExit(3000);
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        gpuName = CleanGpuName(name);
                        gpuSource = "WMI";
                        gpuVendor = SniffGpuVendor(name);
                    }
                }
            }
            catch { }
        }

        private static string CleanGpuName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return name ?? "";
            string n = name.Trim();
            if (n.StartsWith("NVIDIA", StringComparison.OrdinalIgnoreCase))
                n = n.Substring("NVIDIA".Length).Trim();
            else if (n.StartsWith("AMD", StringComparison.OrdinalIgnoreCase))
                n = n.Substring("AMD".Length).Trim();
            n = n.Replace("©", "").Replace("(R)", "").Replace("(TM)", "").Trim();
            n = n.TrimStart('-', ':', ' ');
            return n;
        }

        private string FormatVram(string mem)
        {
            if (string.IsNullOrWhiteSpace(mem)) return mem ?? "";
            var m = Regex.Match(mem, @"(\d+)\s*(MiB|GiB)", RegexOptions.IgnoreCase);
            if (m.Success && long.TryParse(m.Groups[1].Value, out long value))
            {
                string unit = m.Groups[2].Value.ToLowerInvariant();
                double bytes = unit == "mib" ? value * 1024.0 * 1024 : value * 1024.0 * 1024 * 1024;
                return FormatBytes((ulong)Math.Round(bytes, MidpointRounding.AwayFromZero));
            }
            return mem;
        }

        private SettingsForm settingsForm;

        private void ShowSettings()
        {
            if (settingsForm != null && !settingsForm.IsDisposed)
                settingsForm.Dispose();
            if (true)
            {
                // Native WinForms color mode must be set before the form's
                // handle exists, so the title bar, combos, sliders and
                // buttons all render in the Windows dark style when dark.
                ApplyAppColorMode(settingsTheme);
                settingsForm = new SettingsForm(displayOverShortcuts, displayOverWindows, IsAutostart(), useBinaryUnits, gpuSource, refreshInterval, windowOpacity, textOpacity, monitorIndex, Screen.AllScreens.Length, showVramUsage, showTextOutline, showTitle, showOsLine, textColorProfile, textSpacing, anchorIndex, fontChoice, mirrorLayout, mirrorArt, settingsTheme, customX, customY, customLocked, foxMode, foxUnlocked, Color.FromArgb(pawTint.R, pawTint.G, pawTint.B), pawOpacity);
                settingsForm.OverChanged += on =>
                {
                    displayOverShortcuts = on;
                    // Mode switch: drop placement + icon cache so next UpdateInfo
                    // re-solves the free spot immediately (single move, no flicker).
                    InvalidateDesktopCache();
                    try { cachedIconRects.Clear(); iconCacheTick = 0; } catch { }
                    placedParent = new IntPtr(-1);
                    placedBounds = Rectangle.Empty;
                    SaveSettings();
                    UpdateInfo();
                    EnforceHudZOrder();
                };
                settingsForm.WindowsChanged += on =>
                {
                    displayOverWindows = on;
                    // Over-windows is topmost over everything, so it implies
                    // over-shortcuts (the form already re-checks that box too).
                    if (on) displayOverShortcuts = true;
                    InvalidateDesktopCache();
                    placedParent = new IntPtr(-1);
                    placedBounds = Rectangle.Empty;
                    SaveSettings();
                    UpdateInfo();
                    EnforceHudZOrder();
                };
                settingsForm.AutostartChanged += on =>
                {
                    SetAutostart(on);
                    SaveSettings();
                };
                settingsForm.BinaryChanged += on =>
                {
                    useBinaryUnits = on;
                    lastGpuTick = 0;
                    placedBounds = Rectangle.Empty;
                    SaveSettings();
                    UpdateInfo();
                };
                settingsForm.OpacityChanged += v =>
                {
                    windowOpacity = v;
                    SaveSettingsDebounced();
                    // LWA sync inside UpdateInfo applies this without moving.
                    UpdateInfo();
                };
                settingsForm.TextOpacityChanged += v =>
                {
                    textOpacity = v;
                    SaveSettingsDebounced();
                    UpdateInfo();
                };
                settingsForm.PawColorChanged += c =>
                {
                    pawTint = Color.FromArgb(c.R, c.G, c.B);
                    SaveSettings();
                    UpdateInfo();
                };
                settingsForm.PawOpacityChanged += v =>
                {
                    pawOpacity = Math.Clamp(v, 0.0, 1.0);
                    SaveSettingsDebounced();
                    UpdateInfo();
                };
                settingsForm.MonitorChanged += i =>
                {
                    monitorIndex = Math.Clamp(i, 0, Screen.AllScreens.Length - 1);
                    placedBounds = Rectangle.Empty;
                    SaveSettings();
                    UpdateInfo();
                };
                settingsForm.ShowVramUsageChanged += on =>
                {
                    showVramUsage = on;
                    SaveSettings();
                    UpdateInfo();
                };
                settingsForm.TextOutlineChanged += on =>
                {
                    showTextOutline = on;
                    SaveSettings();
                    UpdateInfo();
                };
                settingsForm.ShowTitleChanged += on =>
                {
                    showTitle = on;
                    placedBounds = Rectangle.Empty;
                    SaveSettings();
                    UpdateInfo();
                };
                settingsForm.ShowOsLineChanged += on =>
                {
                    showOsLine = on;
                    placedBounds = Rectangle.Empty;
                    SaveSettings();
                    UpdateInfo();
                };
                settingsForm.TextColorProfileChanged += i =>
                {
                    textColorProfile = Math.Clamp(i, 0, MaxTextColorProfile);
                    SaveSettings();
                    UpdateInfo();
                };
                settingsForm.TextSpacingChanged += i =>
                {
                    textSpacing = Math.Clamp(i, 0, 1);
                    placedBounds = Rectangle.Empty;
                    SaveSettings();
                    UpdateInfo();
                };
                settingsForm.AnchorChanged += i =>
                {
                    anchorIndex = Math.Clamp(i, 0, 4);
                    placedBounds = Rectangle.Empty;
                    SaveSettings();
                    UpdateCustomDragStyle();
                    UpdateInfo();
                };
                settingsForm.CustomPosChanged += (cx, cy) =>
                {
                    // Manual numeric entry: same clamp + persist path as drag-drop.
                    SetCustomPos(cx, cy, true);
                    placedBounds = Rectangle.Empty;
                    UpdateInfo();
                };
                settingsForm.CustomLockedChanged += locked =>
                {
                    SetCustomLocked(locked);
                    // Re-present once so the click-through flag and HTCAPTION
                    // take effect immediately, without waiting for the tick.
                    // (Unlocking from behind-icons mode also promotes the HUD
                    // to a top-level window so it can receive drag input.)
                    placedBounds = Rectangle.Empty;
                    UpdateInfo();
                };
                settingsForm.CustomCenterRequested += () =>
                {
                    RequestCustomCenter();
                };
                settingsForm.FontChanged += i =>
                {
                    fontChoice = Math.Clamp(i, 0, FontOptions.Length - 1);
                    ApplyFont();
                    placedBounds = Rectangle.Empty;
                    SaveSettings();
                    UpdateInfo();
                };
                settingsForm.MirrorLayoutChanged += on =>
                {
                    mirrorLayout = on;
                    SaveSettings();
                    UpdateInfo();
                };
                settingsForm.MirrorArtChanged += on =>
                {
                    mirrorArt = on;
                    SaveSettings();
                    UpdateInfo();
                };
                settingsForm.SettingsThemeChanged += theme =>
                {
                    int t = Math.Clamp(theme, 0, 2);
                    if (t == settingsTheme) return;
                    settingsTheme = t;
                    SaveSettings();
                    // Re-create the dialog so every control (including
                    // dropdown lists) is built fresh under the new native
                    // color mode. Deferred past this event so the old form
                    // is not disposed mid-dispatch; skipped if the user
                    // already closed Settings in the meantime.
                    var old = settingsForm;
                    try
                    {
                        BeginInvoke(new Action(() =>
                        {
                            try { if (old == null || old.IsDisposed) return; ShowSettings(); }
                            catch { }
                        }));
                    }
                    catch { }
                };
                settingsForm.FoxModeChanged += on =>
                {
                    foxMode = on;
                    if (on) foxUnlocked = true;
                    SaveSettings();
                    UpdateInfo();
                };
                settingsForm.CodeLetterTyped += c =>
                {
                    if (c == '\0') ResetCodeBuffer();
                    else NoteCodeLetter(c);
                };
                settingsForm.RefreshIntervalChanged += on =>
                {
                    refreshInterval = on;
                    SaveSettings();
                    if (timer != null)
                    {
                        timer.Stop();
                        timer.Dispose();
                        timer = new System.Windows.Forms.Timer { Interval = refreshInterval };
                        timer.Tick += (_, _) => UpdateInfo();
                        timer.Start();
                    }
                };
            }
            settingsForm.FormClosed += (_, _) => FlushSettings();
            settingsForm.Show();
            settingsForm.Activate();
        }

        private void ExitApp()
        {
            if (tray != null) tray.Visible = false;
            Application.Exit();
        }

        private static string SettingsFile()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DogTag", "settings.txt");
        }

        private void LoadSettings()
        {
            displayOverShortcuts = true;
            hudEnabled = true;
            refreshInterval = 2000;
            windowOpacity = 0.5;
            textOpacity = 1.0;
            pawTint = Color.FromArgb(232, 232, 232);
            pawOpacity = 1.0;
            monitorIndex = 0;
            showVramUsage = true;
            showTextOutline = true;
            showTitle = true;
            showOsLine = true;
            textColorProfile = 10;
            textSpacing = 0;
            anchorIndex = 1;
            customX = int.MinValue;
            customY = int.MinValue;
            customLocked = true;
            mirrorLayout = false;
            mirrorArt = true;
            settingsTheme = 2;
            foxMode = false;
            foxUnlocked = false;
            try
            {
                string path = SettingsFile();
                if (!File.Exists(path)) return;
                bool textProfileSeen = false;
                bool settingsThemeSeen = false;
                foreach (var line in File.ReadAllLines(path))
                {
                    if (line.StartsWith("DisplayOverShortcuts=", StringComparison.Ordinal))
                        displayOverShortcuts = line.EndsWith("1");
                    else if (line.StartsWith("DisplayOverWindows=", StringComparison.Ordinal))
                        displayOverWindows = line.EndsWith("1");
                    else if (line.StartsWith("Enabled=", StringComparison.Ordinal))
                        hudEnabled = line.EndsWith("1");
                    else if (line.StartsWith("BinaryUnits=", StringComparison.Ordinal))
                        useBinaryUnits = line.EndsWith("1");
                    else if (line.StartsWith("TextOpacity=", StringComparison.Ordinal))
                    {
                        var to = line.Substring("TextOpacity=".Length);
                        if (double.TryParse(to, out double t) && t >= 0.0 && t <= 1.0)
                            textOpacity = t;
                    }
                    else if (line.StartsWith("PawColor=", StringComparison.Ordinal))
                    {
                        var hex = line.Substring("PawColor=".Length).Trim();
                        try
                        {
                            if (hex.Length == 6)
                                pawTint = Color.FromArgb(
                                    Convert.ToInt32(hex.Substring(0, 2), 16),
                                    Convert.ToInt32(hex.Substring(2, 2), 16),
                                    Convert.ToInt32(hex.Substring(4, 2), 16));
                        }
                        catch { }
                    }
                    else if (line.StartsWith("PawOpacity=", StringComparison.Ordinal))
                    {
                        var po = line.Substring("PawOpacity=".Length);
                        if (double.TryParse(po, out double p) && p >= 0.0 && p <= 1.0)
                            pawOpacity = p;
                    }
                    else if (line.StartsWith("Opacity=", StringComparison.Ordinal))
                    {
                        var val = line.Substring("Opacity=".Length);
                        if (double.TryParse(val, out double v) && v >= 0.0 && v <= 1.0)
                            windowOpacity = v;
                    }
                    else if (line.StartsWith("RefreshInterval=", StringComparison.Ordinal))
                    {
                        var val = line.Substring("RefreshInterval=".Length);
                        if (int.TryParse(val, out int v) && v >= 500 && v <= 10000)
                            refreshInterval = v;
                    }
                    else if (line.StartsWith("Monitor=", StringComparison.Ordinal))
                    {
                        var val = line.Substring("Monitor=".Length);
                        if (int.TryParse(val, out int v) && v >= 0)
                            monitorIndex = v;
                    }
                    else if (line.StartsWith("ShowVramUsage=", StringComparison.Ordinal))
                        showVramUsage = line.EndsWith("1");
                    else if (line.StartsWith("ShowTitle=", StringComparison.Ordinal))
                        showTitle = !line.EndsWith("0");
                    else if (line.StartsWith("ShowOsLine=", StringComparison.Ordinal))
                        showOsLine = !line.EndsWith("0");
                    else if (line.StartsWith("TextOutline=", StringComparison.Ordinal))
                        showTextOutline = line.EndsWith("1");
                    else if (line.StartsWith("TextColorProfile=", StringComparison.Ordinal))
                    {
                        var val = line.Substring("TextColorProfile=".Length);
                        if (int.TryParse(val, out int v) && v >= 0 && v <= MaxTextColorProfile)
                        {
                            textColorProfile = v;
                            textProfileSeen = true;
                        }
                    }
                    else if (line.StartsWith("TextSpacing=", StringComparison.Ordinal))
                    {
                        var val = line.Substring("TextSpacing=".Length).Trim();
                        if (int.TryParse(val, out int v) && v >= 0 && v <= 1)
                            textSpacing = v;
                    }
                    else if (line.StartsWith("LegacyTextColors=", StringComparison.Ordinal))
                    {
                        // Pre-profile upgrade path: map the old checkbox.
                        if (!textProfileSeen && line.EndsWith("1"))
                            textColorProfile = 1;
                    }
                    else if (line.StartsWith("Anchor=", StringComparison.Ordinal))
                    {
                        var val = line.Substring("Anchor=".Length);
                        if (int.TryParse(val, out int v) && v >= 0 && v <= 4)
                            anchorIndex = v;
                    }
                    else if (line.StartsWith("CustomX=", StringComparison.Ordinal))
                    {
                        var val = line.Substring("CustomX=".Length);
                        if (int.TryParse(val, out int v) && v >= -10000 && v <= 10000)
                            customX = v;
                    }
                    else if (line.StartsWith("CustomY=", StringComparison.Ordinal))
                    {
                        var val = line.Substring("CustomY=".Length);
                        if (int.TryParse(val, out int v) && v >= -10000 && v <= 10000)
                            customY = v;
                    }
                    else if (line.StartsWith("CustomLocked=", StringComparison.Ordinal))
                        customLocked = !line.EndsWith("0");
                    else if (line.StartsWith("FontChoice=", StringComparison.Ordinal))
                    {
                        var val = line.Substring("FontChoice=".Length);
                        if (int.TryParse(val, out int v) && v >= 0 && v < FontOptions.Length)
                            fontChoice = v;
                    }
                    else if (line.StartsWith("MirrorLayout=", StringComparison.Ordinal))
                        mirrorLayout = line.EndsWith("1");
                    else if (line.StartsWith("MirrorArt=", StringComparison.Ordinal))
                        mirrorArt = line.EndsWith("1");
                    else if (line.StartsWith("SettingsTheme=", StringComparison.Ordinal))
                    {
                        var val = line.Substring("SettingsTheme=".Length).Trim();
                        if (int.TryParse(val, out int v) && v >= 0 && v <= 2)
                        {
                            settingsTheme = v;
                            settingsThemeSeen = true;
                        }
                    }
                    else if (line.StartsWith("SettingsDarkMode=", StringComparison.Ordinal))
                    {
                        // Pre-theme upgrade path: explicit dark stays dark,
                        // explicit light stays light.
                        if (!settingsThemeSeen)
                            settingsTheme = line.EndsWith("1") ? 2 : 1;
                    }
                    else if (line.StartsWith("FoxMode=", StringComparison.Ordinal))
                        foxMode = line.EndsWith("1");
                    else if (line.StartsWith("FoxUnlocked=", StringComparison.Ordinal))
                        foxUnlocked = line.EndsWith("1");
                }
            }
            catch { }
            // FoxMode on without the unlock flag (hand-edited file): the
            // toggle must still be reachable.
            if (foxMode) foxUnlocked = true;
        }

        // Slider drags fire dozens of events per second; persisting to disk
        // on each one is pure I/O churn. Live values still apply instantly
        // (UpdateInfo runs per event); only the file write is deferred and
        // coalesced, and always flushed when Settings closes or the app exits.
        private System.Windows.Forms.Timer saveDebounceTimer = null;

        private void SaveSettingsDebounced()
        {
            try
            {
                if (saveDebounceTimer == null)
                {
                    saveDebounceTimer = new System.Windows.Forms.Timer { Interval = 600 };
                    saveDebounceTimer.Tick += (_, _) =>
                    {
                        try { saveDebounceTimer.Stop(); } catch { }
                        SaveSettings();
                    };
                }
                saveDebounceTimer.Stop();
                saveDebounceTimer.Start();
            }
            catch { try { SaveSettings(); } catch { } }
        }

        private void FlushSettings()
        {
            try { if (saveDebounceTimer != null) saveDebounceTimer.Stop(); } catch { }
            SaveSettings();
        }

        private void SaveSettings()
        {
            try
            {
                string path = SettingsFile();
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path,
                    "DisplayOverShortcuts=" + (displayOverShortcuts ? "1" : "0") + Environment.NewLine +
                    "DisplayOverWindows=" + (displayOverWindows ? "1" : "0") + Environment.NewLine +
                    "Enabled=" + (hudEnabled ? "1" : "0") + Environment.NewLine +
                    "BinaryUnits=" + (useBinaryUnits ? "1" : "0") + Environment.NewLine +
                    "RefreshInterval=" + refreshInterval + Environment.NewLine +
                    "Monitor=" + monitorIndex + Environment.NewLine +
                    "ShowVramUsage=" + (showVramUsage ? "1" : "0") + Environment.NewLine +
                    "ShowTitle=" + (showTitle ? "1" : "0") + Environment.NewLine +
                    "ShowOsLine=" + (showOsLine ? "1" : "0") + Environment.NewLine +
                    "TextOutline=" + (showTextOutline ? "1" : "0") + Environment.NewLine +
                    "TextColorProfile=" + textColorProfile + Environment.NewLine +
                    "TextSpacing=" + textSpacing + Environment.NewLine +
                    "Anchor=" + anchorIndex + Environment.NewLine +
                    "CustomX=" + (customX == int.MinValue ? "" : customX.ToString()) + Environment.NewLine +
                    "CustomY=" + (customY == int.MinValue ? "" : customY.ToString()) + Environment.NewLine +
                    "CustomLocked=" + (customLocked ? "1" : "0") + Environment.NewLine +
                    "FontChoice=" + fontChoice + Environment.NewLine +
                    "MirrorLayout=" + (mirrorLayout ? "1" : "0") + Environment.NewLine +
                    "MirrorArt=" + (mirrorArt ? "1" : "0") + Environment.NewLine +
                    "SettingsTheme=" + settingsTheme + Environment.NewLine +
                    "FoxMode=" + (foxMode ? "1" : "0") + Environment.NewLine +
                    "FoxUnlocked=" + (foxUnlocked ? "1" : "0") + Environment.NewLine +
                    "Opacity=" + windowOpacity.ToString("F2") + Environment.NewLine +
                    "TextOpacity=" + textOpacity.ToString("F2") + Environment.NewLine +
                    "PawColor=" + pawTint.R.ToString("X2") + pawTint.G.ToString("X2") + pawTint.B.ToString("X2") + Environment.NewLine +
                    "PawOpacity=" + pawOpacity.ToString("F2") + Environment.NewLine);
            }
            catch { }
        }

        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string RunValueName = "DogTag";

        private static bool IsAutostart()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, false))
                    return key?.GetValue(RunValueName) != null;
            }
            catch { }
            return false;
        }

        private static void SetAutostart(bool on)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
                {
                    if (key == null) return;
                    if (on)
                        key.SetValue(RunValueName, "\"" + Application.ExecutablePath + "\"");
                    else
                        key.DeleteValue(RunValueName, false);
                }
            }
            catch { }
        }

        private static Icon MakeTrayIcon()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DogTag.png"))
            using (var bmp = new Bitmap(stream))
                return Icon.FromHandle(bmp.GetHicon());
        }

        private void EnforceHudZOrder()
        {
            if (!hudEnabled) return;
            try
            {
                EnsureHandles();
                // Keep rawBehind alive; only using rawTop for top-level modes.
                IntPtr active = rawTop;
                if (active == IntPtr.Zero || !IsWindow(active)) return;
                if (placedBounds.Width <= 0 || placedBounds.Height <= 0) return;
                rawHandle = active;
                if (!displayOverShortcuts && !displayOverWindows)
                {
                    EnsureRawBehind();
                    if (rawBehind == IntPtr.Zero || !IsWindow(rawBehind)) return;
                    IntPtr progman = FindWindow("Progman", "Program Manager");
                    IntPtr defView = IntPtr.Zero;
                    try { if (progman != IntPtr.Zero) defView = FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null); } catch { }
                    if (defView != IntPtr.Zero)
                    {
                        // Skip redundant reorders: churning two windows every
                        // z-tick smeared icon drags last attempt.
                        bool ordered = false;
                        try
                        {
                            ordered = IsWindowVisible(rawBehind)
                                && rawBehindText != IntPtr.Zero && IsWindow(rawBehindText) && IsWindowVisible(rawBehindText)
                                && GetWindow(rawBehindText, GW_HWNDPREV) == defView
                                && GetWindow(rawBehind, GW_HWNDPREV) == rawBehindText;
                        }
                        catch { }
                        if (!ordered)
                        {
                            SetWindowPos(rawBehind, defView, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE | (IsWindowVisible(rawBehind) ? 0u : SWP_SHOWWINDOW));
                            if (rawBehindText != IntPtr.Zero && IsWindow(rawBehindText))
                                SetWindowPos(rawBehindText, defView, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE | (IsWindowVisible(rawBehindText) ? 0u : SWP_SHOWWINDOW));
                        }
                    }
                    SyncBehindAlpha();
                    return;
                }
                bool wantTopmost = displayOverWindows;
                bool visible = IsWindowVisible(active);
                bool isTopmost = false;
                try { isTopmost = (GetWindowLong(active, GWL_EXSTYLE) & 0x00000008) != 0; } catch { }
                if (wantTopmost)
                {
                    if (!isTopmost)
                        SetWindowPos(active, HWND_TOPMOST, 0, 0, 0, 0,
                            SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE | (visible ? 0u : SWP_SHOWWINDOW));
                    else if (!visible && rawShown)
                        SetWindowPos(active, HWND_TOPMOST, 0, 0, 0, 0,
                            SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
                }
                else
                {
                    if (isTopmost)
                        SetWindowPos(active, HWND_NOTOPMOST, 0, 0, 0, 0,
                            SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE | (visible ? 0u : SWP_SHOWWINDOW));
                    else if (!visible && rawShown)
                        SetWindowPos(active, HWND_NOTOPMOST, 0, 0, 0, 0,
                            SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
                }
            }
            catch { }
        }

        private bool EnsureIconBackgroundTransparent(Rectangle hudRect)
        {
            // Keep SysListView32 background transparent so our DefView child shows
            // through with icon glyphs opaque over it. Query-first: steady-state sends
            // zero messages (no icon flicker). Targeted invalidate of our rect only,
            // and only when something actually changed.
            bool changed = false;
            try
            {
                IntPtr list = iconListView;
                if (list == IntPtr.Zero || !IsWindow(list))
                {
                    RefreshDesktopHandles(false);
                    list = iconListView;
                }
                if (list == IntPtr.Zero || !IsWindow(list)) return false;
                const uint LVM_FIRST = 0x1000;
                const uint LVM_GETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 55;
                const uint LVM_SETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 54;
                const uint LVM_SETBKCOLOR = LVM_FIRST + 1;
                const uint LVM_SETTEXTBKCOLOR = LVM_FIRST + 38;
                const uint LVM_GETBKCOLOR = LVM_FIRST + 0;
                const uint LVM_GETTEXTBKCOLOR = LVM_FIRST + 37;
                const uint LVS_EX_TRANSPARENTBKGND = 0x00400000;
                const uint CLR_NONE = 0xFFFFFFFF;
                try
                {
                    IntPtr ex = SendMessage(list, LVM_GETEXTENDEDLISTVIEWSTYLE, IntPtr.Zero, IntPtr.Zero);
                    if ((((uint)ex.ToInt64()) & LVS_EX_TRANSPARENTBKGND) == 0)
                    {
                        SendMessage(list, LVM_SETEXTENDEDLISTVIEWSTYLE, new IntPtr((long)LVS_EX_TRANSPARENTBKGND), new IntPtr((long)LVS_EX_TRANSPARENTBKGND));
                        changed = true;
                    }
                }
                catch { }
                try
                {
                    IntPtr bk = SendMessage(list, LVM_GETBKCOLOR, IntPtr.Zero, IntPtr.Zero);
                    if ((uint)bk.ToInt64() != CLR_NONE)
                    {
                        SendMessage(list, LVM_SETBKCOLOR, IntPtr.Zero, new IntPtr((long)CLR_NONE));
                        changed = true;
                    }
                }
                catch { }
                try
                {
                    IntPtr tbk = SendMessage(list, LVM_GETTEXTBKCOLOR, IntPtr.Zero, IntPtr.Zero);
                    if ((uint)tbk.ToInt64() != CLR_NONE)
                    {
                        SendMessage(list, LVM_SETTEXTBKCOLOR, IntPtr.Zero, new IntPtr((long)CLR_NONE));
                        changed = true;
                    }
                }
                catch { }
                if (changed && hudRect.Width > 0)
                {
                    RECT r = new RECT { Left = hudRect.X, Top = hudRect.Y, Right = hudRect.Right, Bottom = hudRect.Bottom };
                    try { InvalidateRect(list, ref r, true); } catch { }
                }
            }
            catch { }
            return changed;
        }

        private IntPtr GetWallpaperPaintTarget()
        {
            // Paint on Progman itself (the visible desktop surface behind the
            // transparent icon ListView). The WorkerW child is a legacy/transient
            // surface on Win11 - BitBlt there succeeds but is never composited.
            try
            {
                IntPtr progman = FindWindow("Progman", "Program Manager");
                if (progman != IntPtr.Zero && IsWindow(progman)) return progman;
            }
            catch { }
            return IntPtr.Zero;
        }

        private void RestoreWallpaperBlit()
        {
            try
            {
                if (lastWallpaperBlit.Width > 0 && lastWallpaperWorker != IntPtr.Zero && IsWindow(lastWallpaperWorker))
                {
                    RECT r = new RECT { Left = lastWallpaperBlit.X, Top = lastWallpaperBlit.Y, Right = lastWallpaperBlit.Right, Bottom = lastWallpaperBlit.Bottom };
                    // WorkerW/Progman client coords == screen coords (fullscreen at 0,0),
                    // but subtract defensively for multi-monitor offsets.
                    try
                    {
                        RECT pr;
                        if (GetWindowRect(lastWallpaperWorker, out pr))
                        {
                            r.Left -= pr.Left; r.Top -= pr.Top; r.Right -= pr.Left; r.Bottom -= pr.Top;
                        }
                    }
                    catch { }
                    InvalidateRect(lastWallpaperWorker, ref r, true);
                }
            }
            catch { }
            lastWallpaperBlit = Rectangle.Empty;
        }

        private bool PaintBehindOnWallpaper(int x, int y, int w, int h)
        {
            if (w <= 0 || h <= 0) return false;
            IntPtr wall = GetWallpaperPaintTarget();
            if (wall == IntPtr.Zero || !IsWindow(wall))
            {
                try { File.AppendAllText(Path.Combine(Path.GetTempPath(), "dogtag_wall.log"), DateTime.Now.ToString("HH:mm:ss") + " no-wall" + Environment.NewLine); } catch { }
                return false;
            }
            if (wall != lastWallpaperWorker && lastWallpaperBlit.Width > 0)
                RestoreWallpaperBlit();
            lastWallpaperWorker = wall;
            try { EnsureIconBackgroundTransparent(new Rectangle(x, y, w, h)); } catch { }
            int bgAlpha = Math.Clamp((int)Math.Round(windowOpacity * 255, MidpointRounding.AwayFromZero), 0, 255);
            if (bgAlpha <= 0)
            {
                // Transparent background = show wallpaper: erase old burn if shrinking.
                if (lastWallpaperBlit.Width > 0 && (lastWallpaperBlit.Width != w || lastWallpaperBlit.Height != h || lastWallpaperBlit.X != x || lastWallpaperBlit.Y != y))
                    RestoreWallpaperBlit();
                return true;
            }
            int cx = x, cy = y;
            try
            {
                RECT pr;
                if (GetWindowRect(wall, out pr)) { cx = x - pr.Left; cy = y - pr.Top; }
            }
            catch { }
            try
            {
                using (var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb))
                {
                    using (var g = Graphics.FromImage(bmp))
                        DrawHud(g, bgAlpha, textOpacity);
                    IntPtr hdcWall = GetDC(wall);
                    if (hdcWall == IntPtr.Zero)
                    {
                        try { File.AppendAllText(Path.Combine(Path.GetTempPath(), "dogtag_wall.log"), DateTime.Now.ToString("HH:mm:ss") + " GetDC-null wall=" + wall + Environment.NewLine); } catch { }
                        return false;
                    }
                    try
                    {
                        IntPtr mem = CreateCompatibleDC(hdcWall);
                        if (mem == IntPtr.Zero) return false;
                        try
                        {
                            IntPtr hbm = bmp.GetHbitmap();
                            try
                            {
                                IntPtr old = SelectObject(mem, hbm);
                                bool blt = BitBlt(hdcWall, cx, cy, w, h, mem, 0, 0, SRCCOPY);
                                int bltErr = blt ? 0 : Marshal.GetLastWin32Error();
                                try { SelectObject(mem, old); } catch { }
                                try { File.AppendAllText(Path.Combine(Path.GetTempPath(), "dogtag_wall.log"), DateTime.Now.ToString("HH:mm:ss") + " blt=" + blt + " err=" + bltErr + " wall=" + wall + " cx=" + cx + " cy=" + cy + " w=" + w + " h=" + h + Environment.NewLine); } catch { }
                                if (blt)
                                {
                                    // Erase leftover fringe when shrinking/moving.
                                    if (lastWallpaperBlit.Width > 0 && lastWallpaperBlit != new Rectangle(x, y, w, h))
                                    {
                                        RECT oldR = new RECT { Left = lastWallpaperBlit.X, Top = lastWallpaperBlit.Y, Right = lastWallpaperBlit.Right, Bottom = lastWallpaperBlit.Bottom };
                                        try
                                        {
                                            RECT pr2;
                                            if (GetWindowRect(wall, out pr2)) { oldR.Left -= pr2.Left; oldR.Top -= pr2.Top; oldR.Right -= pr2.Left; oldR.Bottom -= pr2.Top; }
                                        }
                                        catch { }
                                        InvalidateRect(wall, ref oldR, true);
                                        // Repaint fresh (invalidate erased our new blit where overlapping).
                                        SelectObject(mem, hbm);
                                        BitBlt(hdcWall, cx, cy, w, h, mem, 0, 0, SRCCOPY);
                                        try { SelectObject(mem, old); } catch { }
                                    }
                                    lastWallpaperBlit = new Rectangle(x, y, w, h);
                                    return true;
                                }
                                return false;
                            }
                            finally { try { DeleteObject(hbm); } catch { } }
                        }
                        finally { try { DeleteDC(mem); } catch { } }
                    }
                    finally { try { ReleaseDC(wall, hdcWall); } catch { } }
                }
            }
            catch { return false; }
        }

        private void ClearLegacyBurnsOnce()
        {
            // One-time cleanup of opaque BitBlt burns left on Progman/WorkerW by
            // previous builds (wallpaper-BitBlt era). Fullscreen repaint flashes once
            // on upgrade only, never per-frame.
            if (legacyBurnsCleared) return;
            legacyBurnsCleared = true;
            try { RestoreWallpaperBlit(); } catch { }
            try
            {
                IntPtr progman = FindWindow("Progman", "Program Manager");
                if (progman != IntPtr.Zero && IsWindow(progman))
                {
                    InvalidateRect(progman, IntPtr.Zero, true);
                    IntPtr wall = FindWindowEx(progman, IntPtr.Zero, "WorkerW", null);
                    if (wall != IntPtr.Zero && IsWindow(wall)) InvalidateRect(wall, IntPtr.Zero, true);
                }
            }
            catch { }
        }

        private bool IsBehindBuried(IntPtr parent, IntPtr self)
        {
            // Buried = above the icon ListView (over icons) - must be below it.
            // Walk DefView children top-to-bottom.
            try
            {
                IntPtr list = FindWindowEx(parent, IntPtr.Zero, "SysListView32", null);
                if (list == IntPtr.Zero || list == self || !IsWindow(list)) return false;
                // Walk DefView children top-to-bottom: want ListView above self.
                IntPtr cur = GetWindow(parent, GW_CHILD);
                int seenSelf = -1, seenList = -1, idx = 0;
                var guard = 0;
                while (cur != IntPtr.Zero && guard++ < 256)
                {
                    if (cur == self) seenSelf = idx;
                    if (cur == list) seenList = idx;
                    if (seenSelf >= 0 && seenList >= 0) break;
                    cur = GetWindow(cur, GW_HWNDNEXT);
                    idx++;
                }
                if (seenSelf >= 0 && seenList >= 0) return seenSelf < seenList;
                return false;
            }
            catch { }
            return false;
        }

        private bool WantChild() => !displayOverWindows && !displayOverShortcuts;

        // OFF-mode semantics: HUD yields to desktop shortcuts (never covers them).
        // True behind-icons interleaving is impossible here (proven: no foreign child
        // of the shell desktop ever composites), so we place the top-level HUD where
        // no icon overlaps. ON-mode pins bottom-right regardless.
        private bool IconsYieldMode() => false;

        // ---- Custom anchor (anchor 4): free drag position ----
        private const int WS_EX_TRANSPARENT_N = 0x00000020;
        private const int WM_NCHITTEST_C = 0x0084;
        private const int WM_ENTERSIZEMOVE_C = 0x0231;
        private const int WM_EXITSIZEMOVE_C = 0x0232;
        private const int WM_MOVE_C = 0x0003;
        private const int WM_LBUTTONDOWN_C = 0x0201;
        private const int WM_LBUTTONUP_C = 0x0202;
        private const int WM_MOUSEMOVE_C = 0x0200;
        private const int WM_CAPTURECHANGED_C = 0x0215;
        private const int HTCAPTION_C = 2;
        private static readonly IntPtr HTTRANSPARENT_C = new IntPtr(-1);

        private bool IsCustomAnchor() => anchorIndex == 4;

        private bool IsCustomDragArmed() => anchorIndex == 4 && !customLocked && hudEnabled;

        private void MarkCustomDragStart()
        {
            customDragActive = true;
            try { customDragStartTick = Environment.TickCount64; } catch { }
        }

        private void NoteCodeLetter(char c)
        {
            try
            {
                codeBuffer.Add(c);
                if (codeBuffer.Count > 10) codeBuffer.RemoveAt(0);
                if (codeBuffer.Count == 10 && new string(codeBuffer.ToArray()) == "aethercore")
                {
                    codeBuffer.Clear();
                    UnlockFoxMode();
                }
            }
            catch { }
        }

        private void ResetCodeBuffer()
        {
            try { codeBuffer.Clear(); } catch { }
        }

        private void UnlockFoxMode()
        {
            foxUnlocked = true;
            foxMode = true;
            SaveSettings();
            UpdateInfo();
            try
            {
                if (settingsForm != null && !settingsForm.IsDisposed)
                    settingsForm.RevealFoxMode();
            }
            catch { }
        }

        private Point ResolveCustomPos(int w, int h)
        {
            try
            {
                var screens = Screen.AllScreens;
                if (screens == null || screens.Length == 0)
                    return new Point(20, 20);
                // No saved position yet: fall back to bottom-right of the
                // selected monitor (same as anchor 0) so first use is sane.
                if (customX == int.MinValue || customY == int.MinValue)
                {
                    int fmi = Math.Clamp(monitorIndex, 0, screens.Length - 1);
                    var fa = screens[fmi].WorkingArea;
                    return new Point(fa.Right - w - 20, fa.Bottom - h - 20);
                }
                // Saved position is in virtual-screen coords and may span any
                // monitor, so clamp to the screen containing the point (not the
                // selected monitor) to survive monitor add/remove.
                Point p = new Point(customX, customY);
                Screen owner = null;
                try { owner = Screen.FromPoint(p); } catch { owner = null; }
                if (owner == null)
                {
                    int fmi2 = Math.Clamp(monitorIndex, 0, screens.Length - 1);
                    owner = screens[fmi2];
                }
                var wa = owner.WorkingArea;
                int cx = customX, cy = customY;
                if (w < wa.Width)
                    cx = Math.Clamp(cx, wa.Left, wa.Right - w);
                else
                    cx = wa.Left;
                if (h < wa.Height)
                    cy = Math.Clamp(cy, wa.Top, wa.Bottom - h);
                else
                    cy = wa.Top;
                return new Point(cx, cy);
            }
            catch { return new Point(20, 20); }
        }

        private void SetCustomPos(int x, int y, bool save)
        {
            try
            {
                // Clamp to the owning screen so the HUD can never be lost
                // off-screen (e.g. after unplugging a monitor).
                int w = placedBounds.Width > 0 ? placedBounds.Width : 300;
                int h = placedBounds.Height > 0 ? placedBounds.Height : 150;
                Point p = new Point(x, y);
                Screen owner = null;
                try { owner = Screen.FromPoint(p); } catch { }
                if (owner == null)
                {
                    try
                    {
                        var ss = Screen.AllScreens;
                        if (ss.Length > 0) owner = ss[Math.Clamp(monitorIndex, 0, ss.Length - 1)];
                    }
                    catch { }
                }
                if (owner != null)
                {
                    var wa = owner.WorkingArea;
                    if (w < wa.Width) x = Math.Clamp(x, wa.Left, wa.Right - w);
                    if (h < wa.Height) y = Math.Clamp(y, wa.Top, wa.Bottom - h);
                }
                customX = x; customY = y;
                if (save)
                {
                    SaveSettings();
                    try { settingsForm?.UpdateCustomPos(x, y); } catch { }
                }
            }
            catch { try { customX = x; customY = y; } catch { } }
        }

        private void SetCustomLocked(bool locked)
        {
            customLocked = locked;
            SaveSettings();
            DragLog("lock=" + (locked ? 1 : 0) + " anchor=" + anchorIndex);
            UpdateCustomDragStyle();
            try { settingsForm?.SetCustomLock(locked); } catch { }
        }

        // True-center: place the HUD's center (not its top-left corner) at the
        // working-area center of the screen it currently lives on.
        private void RequestCustomCenter()
        {
            try
            {
                if (placedBounds.Width <= 0 || placedBounds.Height <= 0)
                {
                    // No measured size yet: present once so placedBounds (w/h)
                    // becomes valid, then center using the real extents.
                    try { UpdateInfo(); } catch { }
                }
                int w = placedBounds.Width, h = placedBounds.Height;
                Screen owner = null;
                int ownerIdx = -1;
                try
                {
                    var ss = Screen.AllScreens;
                    if (ss.Length == 0) return;
                    try
                    {
                        Point centerProbe = (w > 0 && h > 0)
                            ? new Point(placedBounds.X + w / 2, placedBounds.Y + h / 2)
                            : new Point(customX == int.MinValue ? 0 : customX, customY == int.MinValue ? 0 : customY);
                        owner = Screen.FromPoint(centerProbe);
                    }
                    catch { owner = null; }
                    if (owner == null)
                        owner = ss[Math.Clamp(monitorIndex, 0, ss.Length - 1)];
                    for (int i = 0; i < ss.Length; i++)
                        if (ss[i].DeviceName == owner.DeviceName) { ownerIdx = i; break; }
                }
                catch { return; }
                var wa = owner.WorkingArea;
                int cx, cy;
                if (w > 0 && h > 0 && w < wa.Width && h < wa.Height)
                {
                    cx = wa.Left + (wa.Width - w) / 2;
                    cy = wa.Top + (wa.Height - h) / 2;
                }
                else
                {
                    cx = wa.Left + wa.Width / 2 - Math.Max(0, w) / 2;
                    cy = wa.Top + wa.Height / 2 - Math.Max(0, h) / 2;
                }
                if (ownerIdx >= 0 && ownerIdx != monitorIndex)
                {
                    monitorIndex = ownerIdx;
                    try { settingsForm?.UpdateMonitorIndex(ownerIdx); } catch { }
                }
                SetCustomPos(cx, cy, true);
                placedBounds = Rectangle.Empty;
                UpdateInfo();
            }
            catch { }
        }

        private void SetExTransparent(IntPtr hwnd, bool transparent)
        {
            try
            {
                if (hwnd == IntPtr.Zero || !IsWindow(hwnd)) return;
                int ex = GetWindowLong(hwnd, GWL_EXSTYLE);
                bool has = (ex & WS_EX_TRANSPARENT_N) != 0;
                if (has == transparent) return;
                int next = transparent ? (ex | WS_EX_TRANSPARENT_N) : (ex & ~WS_EX_TRANSPARENT_N);
                SetWindowLong(hwnd, GWL_EXSTYLE, next);
                try { SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED); } catch { }
            }
            catch { }
        }

        // Locked Custom = click-through (does not block icons/windows).
        // Unlocked Custom = hit-testable + HTCAPTION so it can be dragged.
        // Anchors 0-3 keep legacy behaviour (blocking) to avoid any change.
        private void UpdateCustomDragStyle()
        {
            try
            {
                if (anchorIndex == 4)
                {
                    bool draggable = IsCustomDragArmed();
                    SetExTransparent(rawTop, !draggable);
                    SetExTransparent(rawBehind, !draggable);
                    SetExTransparent(rawBehindText, !draggable);
                }
                else
                {
                    // Restore legacy blocking behaviour on all layers.
                    SetExTransparent(rawTop, false);
                    SetExTransparent(rawBehind, false);
                    SetExTransparent(rawBehindText, false);
                }
            }
            catch { }
        }

        private bool GetWindowScreenPos(IntPtr hwnd, out int sx, out int sy)
        {
            sx = 0; sy = 0;
            try
            {
                RECT r;
                if (!GetWindowRect(hwnd, out r)) return false;
                sx = r.Left; sy = r.Top;
                return true;
            }
            catch { return false; }
        }

        private void OnCustomDragMove(IntPtr moved)
        {
            try
            {
                if (!IsCustomDragArmed()) return;
                int sx, sy;
                if (!GetWindowScreenPos(moved, out sx, out sy)) return;
                SetCustomPos(sx, sy, false);
                SyncBehindSiblingFrom(moved);
            }
            catch { }
        }

        private void OnCustomDragFinished(IntPtr moved)
        {
            bool wasActive = customDragActive;
            customDragActive = false;
            manualDragHwnd = IntPtr.Zero;
            nchHitLogged = false;
            try
            {
                int sx, sy;
                if (GetWindowScreenPos(moved, out sx, out sy))
                {
                    SetCustomPos(sx, sy, true);
                    // Keep the selected-monitor fallback sane: point the
                    // monitor dropdown at the screen the HUD now lives on.
                    try
                    {
                        var ss = Screen.AllScreens;
                        Screen hit = Screen.FromPoint(new Point(sx, sy));
                        for (int i = 0; i < ss.Length; i++)
                        {
                            if (ss[i].DeviceName == hit.DeviceName)
                            {
                                if (monitorIndex != i)
                                {
                                    monitorIndex = i;
                                    SaveSettings();
                                    try { settingsForm?.UpdateMonitorIndex(i); } catch { }
                                }
                                break;
                            }
                        }
                    }
                    catch { }
                }
                else if (wasActive)
                {
                    SaveSettings();
                }
            }
            catch { }
            try { settingsForm?.UpdateCustomPos(customX, customY); } catch { }
            UpdateCustomDragStyle();
            // Re-present once at the clamped position (also repaints).
            try { UpdateInfo(); } catch { }
        }

        private void SyncBehindSiblingFrom(IntPtr moved)
        {
            try
            {
                if (syncingBehindSibling) return;
                if (rawBehind == IntPtr.Zero || rawBehindText == IntPtr.Zero) return;
                if (!IsWindow(rawBehind) || !IsWindow(rawBehindText)) return;
                IntPtr sibling = IntPtr.Zero;
                if (moved == rawBehind) sibling = rawBehindText;
                else if (moved == rawBehindText) sibling = rawBehind;
                else return;
                RECT r;
                if (!GetWindowRect(moved, out r)) return;
                int w = r.Right - r.Left, h = r.Bottom - r.Top;
                if (w <= 0 || h <= 0) return;
                IntPtr progman = IntPtr.Zero;
                try { progman = FindWindow("Progman", "Program Manager"); } catch { }
                if (progman == IntPtr.Zero || !IsWindow(progman)) return;
                var pt = new WinPoint { X = r.Left, Y = r.Top };
                try { ScreenToClient(progman, ref pt); } catch { return; }
                syncingBehindSibling = true;
                try { SetWindowPos(sibling, IntPtr.Zero, pt.X, pt.Y, w, h, SWP_NOZORDER | SWP_NOACTIVATE); } catch { }
            }
            catch { }
            finally { syncingBehindSibling = false; }
        }

        // Drag diagnostics: append-only, capped, never throws. Tells us whether
        // unlock flips state, whether hit-testing reaches the HUD window, and
        // which drag path (system move vs manual capture) owns the gesture.
        // File: %TEMP%\dogtag_drag.log
        private static void DragLog(string msg)
        {
            try
            {
                string path = Path.Combine(Path.GetTempPath(), "dogtag_drag.log");
                try
                {
                    var fi = new FileInfo(path);
                    if (fi.Exists && fi.Length > 100 * 1024)
                        File.WriteAllText(path, "");
                }
                catch { }
                File.AppendAllText(path,
                    DateTime.Now.ToString("HH:mm:ss.fff") + " " + msg + Environment.NewLine);
            }
            catch { }
        }

        // Manual-drag fallback: moves the window with the mouse while captured.
        // Complements HTCAPTION (which swallows LBUTTONDOWN into a system modal
        // move loop when it works); this path only engages when LBUTTONDOWN
        // actually reaches us, i.e. the system loop did not take the gesture.
        private bool TryBeginManualDrag(IntPtr hwnd)
        {
            try
            {
                if (!IsCustomDragArmed()) return false;
                if (hwnd == IntPtr.Zero || !IsWindow(hwnd)) return false;
                CursorPoint cur;
                if (!GetCursorPos(out cur)) return false;
                RECT r;
                if (!GetWindowRect(hwnd, out r)) return false;
                manualDragOffset = new Point(cur.X - r.Left, cur.Y - r.Top);
                manualDragHwnd = hwnd;
                MarkCustomDragStart();
                try { SetCapture(hwnd); } catch { }
                int sx, sy;
                GetWindowScreenPos(hwnd, out sx, out sy);
                DragLog("lbdown hwnd=" + hwnd + " armed=1 locked=" + customLocked + " anchor=" + anchorIndex + " at=" + sx + "," + sy);
                return true;
            }
            catch { return false; }
        }

        private void UpdateManualDrag(IntPtr hwnd)
        {
            try
            {
                if (manualDragHwnd == IntPtr.Zero || manualDragHwnd != hwnd) return;
                if (!customDragActive) return;
                // Handle moves itself and pulls the HUD along (HUD follow lives
                // inside MoveCustomWindow for the handle branch).
                if (hwnd == dragHandle && hwnd != IntPtr.Zero)
                {
                    CursorPoint curH;
                    if (!GetCursorPos(out curH)) return;
                    MoveCustomWindow(hwnd, curH.X - manualDragOffset.X, curH.Y - manualDragOffset.Y);
                    return;
                }
                CursorPoint cur;
                if (!GetCursorPos(out cur)) return;
                int nx = cur.X - manualDragOffset.X;
                int ny = cur.Y - manualDragOffset.Y;
                MoveCustomWindow(hwnd, nx, ny);
                OnCustomDragMove(hwnd);
            }
            catch { }
        }

        private void EndManualDrag(IntPtr hwnd, string why)
        {
            try
            {
                if (manualDragHwnd == IntPtr.Zero) return;
                if (manualDragHwnd != hwnd) return;
                manualDragHwnd = IntPtr.Zero;
                try { if (GetCapture() == hwnd) ReleaseCapture(); } catch { }
                if (hwnd == dragHandle && hwnd != IntPtr.Zero)
                {
                    // The handle's own position is meaningless: persist the
                    // HUD position it pulled along instead.
                    FinishHandleDrag(why);
                    return;
                }
                int sx, sy;
                GetWindowScreenPos(hwnd, out sx, out sy);
                DragLog("up (" + why + ") hwnd=" + hwnd + " at=" + sx + "," + sy);
                OnCustomDragFinished(hwnd);
            }
            catch { try { manualDragHwnd = IntPtr.Zero; } catch { } }
        }

        private void MoveCustomWindow(IntPtr hwnd, int sx, int sy)
        {
            try
            {
                if (hwnd == IntPtr.Zero || !IsWindow(hwnd)) return;
                if (hwnd == dragHandle && hwnd != IntPtr.Zero)
                {
                    // The handle moves freely; the HUD follows it.
                    SetWindowPos(hwnd, IntPtr.Zero, sx, sy, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
                    FollowHudToHandle();
                    return;
                }
                if (hwnd == rawTop)
                {
                    SetWindowPos(hwnd, IntPtr.Zero, sx, sy, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
                }
                else if (hwnd == rawBehind || hwnd == rawBehindText)
                {
                    IntPtr progman = IntPtr.Zero;
                    try { progman = FindWindow("Progman", "Program Manager"); } catch { }
                    if (progman == IntPtr.Zero || !IsWindow(progman)) return;
                    var pt = new WinPoint { X = sx, Y = sy };
                    try { ScreenToClient(progman, ref pt); } catch { return; }
                    SetWindowPos(hwnd, IntPtr.Zero, pt.X, pt.Y, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
                }
            }
            catch { }
        }

        private IntPtr HudHwndForHandle()
        {
            try
            {
                if (WantChild())
                {
                    if (rawBehind != IntPtr.Zero && IsWindow(rawBehind)) return rawBehind;
                    if (rawBehindText != IntPtr.Zero && IsWindow(rawBehindText)) return rawBehindText;
                    return IntPtr.Zero;
                }
                if (rawTop != IntPtr.Zero && IsWindow(rawTop)) return rawTop;
            }
            catch { }
            return IntPtr.Zero;
        }

        private void EnsureDragHandle()
        {
            try
            {
                if (handleForm == null || handleForm.IsDisposed)
                {
                    handleForm = new DragHandleForm(this);
                    dragHandle = IntPtr.Zero;
                }
                try
                {
                    if (dragHandle == IntPtr.Zero && handleForm != null && !handleForm.IsDisposed)
                        dragHandle = handleForm.Handle;
                }
                catch { }
            }
            catch { }
        }

        private void HideDragHandle()
        {
            try
            {
                if (manualDragHwnd == dragHandle) manualDragHwnd = IntPtr.Zero;
                try
                {
                    if (handleForm != null && !handleForm.IsDisposed && handleForm.Visible)
                        handleForm.Hide();
                }
                catch { }
            }
            catch { }
        }

        // Keeps the floating grip glued above (or below, when crowded) the HUD.
        // Called after every successful HUD present; skipped while a drag owns
        // the geometry. Programmatic moves must never be mistaken for drags:
        // handle WM_MOVE only follows while a handle drag flag is set.
        private void UpdateDragHandlePlacement(int x, int y, int w, int h)
        {
            try
            {
                bool show = anchorIndex == 4 && !customLocked && hudEnabled && w > 0 && h > 0;
                if (!show) { HideDragHandle(); return; }
                EnsureDragHandle();
                if (dragHandle == IntPtr.Zero || !IsWindow(dragHandle)) return;
                Screen owner = null;
                try { owner = Screen.FromPoint(new Point(x + w / 2, y + h / 2)); } catch { owner = null; }
                if (owner == null)
                {
                    try
                    {
                        var ss = Screen.AllScreens;
                        if (ss.Length > 0) owner = ss[Math.Clamp(monitorIndex, 0, ss.Length - 1)];
                    }
                    catch { }
                }
                Rectangle wa = owner != null ? owner.WorkingArea : new Rectangle(x - 500, y - 500, 2000, 2000);
                int hx = x + (w - DragHandleW) / 2;
                int hy = y - DragHandleGap - DragHandleH;
                if (hy < wa.Top) hy = y + h + DragHandleGap; // no room above: dock below
                hx = Math.Clamp(hx, wa.Left, Math.Max(wa.Left, wa.Right - DragHandleW));
                hy = Math.Clamp(hy, wa.Top, Math.Max(wa.Top, wa.Bottom - DragHandleH));
                bool wasVis = false;
                try { wasVis = handleForm != null && !handleForm.IsDisposed && handleForm.Visible; } catch { }
                try { if (handleForm != null && !handleForm.IsDisposed) handleForm.SyncTopMost(displayOverWindows); } catch { }
                Point want = new Point(hx, hy);
                if (!wasVis)
                {
                    try
                    {
                        handleForm.Location = want;
                        if (!displayOverWindows)
                        {
                            // Keep the grip above the HUD without going topmost.
                            handleForm.Show();
                            try { handleForm.BringToFront(); } catch { }
                        }
                        else
                        {
                            handleForm.Show();
                        }
                    }
                    catch { }
                    bool nowVis = false;
                    try { nowVis = handleForm != null && !handleForm.IsDisposed && handleForm.Visible; } catch { }
                    DragLog("handle-shown at=" + hx + "," + hy + " form=1 vis=" + (nowVis ? 1 : 0));
                }
                else
                {
                    try
                    {
                        if (handleForm.Location != want) handleForm.Location = want;
                    }
                    catch { }
                }
            }
            catch { }
        }

        private void SnapshotHandleSession()
        {
            try
            {
                dragSessionHudW = (placedBounds.Width > 0) ? placedBounds.Width : 0;
                dragSessionHudH = (placedBounds.Height > 0) ? placedBounds.Height : 0;
                dragHandleBelowHud = false;
                try
                {
                    RECT hr;
                    if (GetWindowRect(dragHandle, out hr) && placedBounds.Width > 0)
                        dragHandleBelowHud = hr.Top > placedBounds.Y + placedBounds.Height / 2;
                }
                catch { }
            }
            catch { }
        }

        // Repositions the HUD from the handle's current screen rect using the
        // geometry snapshot taken at drag start (HUD content is frozen mid-drag).
        private void FollowHudToHandle()
        {
            try
            {
                if (!IsCustomDragArmed()) return;
                if (dragSessionHudW <= 0 || dragSessionHudH <= 0)
                    SnapshotHandleSession();
                int hw = dragSessionHudW > 0 ? dragSessionHudW : placedBounds.Width;
                int hh = dragSessionHudH > 0 ? dragSessionHudH : placedBounds.Height;
                if (hw <= 0 || hh <= 0) return;
                RECT hr;
                if (!GetWindowRect(dragHandle, out hr)) return;
                int hx = hr.Left + (DragHandleW - hw) / 2;
                int hy = dragHandleBelowHud
                    ? hr.Top - DragHandleGap - hh
                    : hr.Bottom + DragHandleGap;
                IntPtr hud = HudHwndForHandle();
                if (hud == IntPtr.Zero) return;
                MoveCustomWindow(hud, hx, hy);
                OnCustomDragMove(hud);
            }
            catch { }
        }

        private void FinishHandleDrag(string why)
        {
            try
            {
                if (manualDragHwnd == dragHandle) manualDragHwnd = IntPtr.Zero;
                IntPtr hud = HudHwndForHandle();
                if (hud == IntPtr.Zero) { customDragActive = false; return; }
                int sx, sy;
                GetWindowScreenPos(hud, out sx, out sy);
                DragLog("handle-up (" + why + ") hud=" + sx + "," + sy);
                OnCustomDragFinished(hud);
            }
            catch { try { customDragActive = false; } catch { } }
        }

        private IntPtr FindIconListView()
        {
            try
            {
                IntPtr progman = FindWindow("Progman", "Program Manager");
                if (progman == IntPtr.Zero || !IsWindow(progman)) return IntPtr.Zero;
                IntPtr defView = FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (defView == IntPtr.Zero || !IsWindow(defView)) return IntPtr.Zero;
                IntPtr list = FindWindowEx(defView, IntPtr.Zero, "SysListView32", null);
                if (list != IntPtr.Zero && IsWindow(list)) iconListView = list;
                return list;
            }
            catch { }
            return IntPtr.Zero;
        }

        private List<Rectangle> GetIconRects()
        {
            if (iconCacheTick != 0 && Environment.TickCount64 - iconCacheTick < 10000 && cachedIconRects.Count > 0)
                return cachedIconRects;
            var rects = new List<Rectangle>();
            try
            {
                IntPtr list = iconListView;
                if (list == IntPtr.Zero || !IsWindow(list)) list = FindIconListView();
                if (list == IntPtr.Zero || !IsWindow(list)) return rects;
                const uint LVM_FIRST = 0x1000;
                const uint LVM_GETITEMCOUNT = LVM_FIRST + 4;
                const uint LVM_GETITEMPOSITION = LVM_FIRST + 16;
                int count = SendMessage(list, LVM_GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero).ToInt32();
                if (count <= 0 || count > 512) return rects;
                GetWindowThreadProcessId(list, out uint pid);
                IntPtr hProc = OpenProcess(0x0438, false, pid);
                if (hProc == IntPtr.Zero) return rects;
                try
                {
                    IntPtr remote = VirtualAllocEx(hProc, IntPtr.Zero, new IntPtr(16), 0x3000, 0x04);
                    if (remote == IntPtr.Zero) return rects;
                    try
                    {
                        for (int i = 0; i < count; i++)
                        {
                            try
                            {
                                var zero = new WinPoint();
                                if (!WriteProcessMemory(hProc, remote, ref zero, 8, out _)) continue;
                                IntPtr ok = SendMessageTimeout(list, LVM_GETITEMPOSITION, new IntPtr(i), remote, 0, 500, out _);
                                if (ok == IntPtr.Zero) continue;
                                var pt = new WinPoint();
                                if (!ReadProcessMemory(hProc, remote, ref pt, 8, out _)) continue;
                                var mpt = new WinPoint { X = pt.X, Y = pt.Y };
                                MapWindowPoints(list, IntPtr.Zero, ref mpt, 1);
                                rects.Add(new Rectangle(mpt.X - 8, mpt.Y - 8, 96, 112));
                            }
                            catch { }
                        }
                    }
                    finally { try { VirtualFreeEx(hProc, remote, IntPtr.Zero, 0x8000); } catch { } }
                }
                finally { try { CloseHandle(hProc); } catch { } }
                if (rects.Count > 0)
                {
                    cachedIconRects = rects;
                    iconCacheTick = Environment.TickCount64;
                }
                try
                {
                    if (s_iconDiag++ < 3)
                        File.AppendAllText(Path.Combine(Path.GetTempPath(), "dogtag_icons.log"),
                            DateTime.Now.ToString("HH:mm:ss") + " count=" + count + " got=" + rects.Count + Environment.NewLine);
                }
                catch { }
            }
            catch { }
            return rects.Count > 0 ? rects : cachedIconRects;
        }

        private Rectangle FindNonOverlappingSpot(Rectangle preferred, int w, int h)
        {
            Rectangle area;
            try { area = Screen.PrimaryScreen.WorkingArea; }
            catch { return preferred; }
            var candidates = new List<Rectangle>
            {
                preferred,
                new Rectangle(area.Left + ScreenMargin, area.Bottom - h - ScreenMargin, w, h),
                new Rectangle(area.Right - w - ScreenMargin, area.Top + ScreenMargin, w, h),
                new Rectangle(area.Left + ScreenMargin, area.Top + ScreenMargin, w, h),
            };
            List<Rectangle> icons;
            try { icons = GetIconRects(); } catch { return preferred; }
            if (icons == null || icons.Count == 0) return preferred;
            // Fast path: first fully free corner (deterministic order, no moves when stable).
            foreach (var c in candidates)
            {
                if (c.Left < area.Left || c.Top < area.Top || c.Right > area.Right || c.Bottom > area.Bottom)
                    continue;
                var test = Rectangle.Inflate(c, IconPad, IconPad);
                bool hit = false;
                foreach (var ir in icons)
                {
                    try { if (test.IntersectsWith(ir)) { hit = true; break; } } catch { }
                }
                if (!hit) return c;
            }
            try
            {
                if (s_iconDiag++ < 2)
                {
                    var sb = new System.Text.StringBuilder();
                    sb.Append(DateTime.Now.ToString("HH:mm:ss") + " crowded icons=" + icons.Count + " pref=" + preferred.X + "," + preferred.Y + " hits=[");
                    foreach (var ir in icons)
                    {
                        try
                        {
                            var t0 = Rectangle.Inflate(preferred, IconPad, IconPad);
                            if (t0.IntersectsWith(ir)) sb.Append(ir.X + "," + ir.Y + " ");
                        }
                        catch { }
                    }
                    sb.Append("]" + Environment.NewLine);
                    File.AppendAllText(Path.Combine(Path.GetTempPath(), "dogtag_icons.log"), sb.ToString());
                }
            }
            catch { }
            // Crowded desktop (no free corner): pick least-overlapped corner so the
            // fewest shortcuts are touched. Shell always draws icon glyphs over us.
            Rectangle best = preferred;
            long bestScore = long.MaxValue;
            foreach (var c in candidates)
            {
                if (c.Left < area.Left || c.Top < area.Top || c.Right > area.Right || c.Bottom > area.Bottom)
                    continue;
                var test = Rectangle.Inflate(c, IconPad, IconPad);
                long score = 0;
                foreach (var ir in icons)
                {
                    try
                    {
                        if (test.IntersectsWith(ir))
                        {
                            var inter = Rectangle.Intersect(test, ir);
                            score += (long)inter.Width * inter.Height;
                        }
                    }
                    catch { }
                }
                if (score < bestScore) { bestScore = score; best = c; }
            }
            return best;
        }

        private static IntPtr behindWndProcKeeper = IntPtr.Zero;
        private static object behindClassLock = new object();
        private static bool behindClassRegistered = false;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WndClassEx
        {
            public uint cbSize;
            public uint style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            [MarshalAs(UnmanagedType.LPWStr)] public string lpszMenuName;
            [MarshalAs(UnmanagedType.LPWStr)] public string lpszClassName;
            public IntPtr hIconSm;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern ushort RegisterClassExW(ref WndClassEx lpwcx);
        [DllImport("user32.dll")]
        private static extern IntPtr DefWindowProcW(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
        private delegate IntPtr WndProcDel(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        private static WndProcDel behindWndProcRef;

        private static IntPtr BehindWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            return DefWindowProcW(hWnd, msg, wParam, lParam);
        }

        private void RegisterBehindClass()
        {
            if (behindClassRegistered) return;
            lock (behindClassLock)
            {
                if (behindClassRegistered) return;
                try
                {
                    behindWndProcRef = BehindWndProc;
                    var wc = new WndClassEx();
                    wc.cbSize = (uint)Marshal.SizeOf(typeof(WndClassEx));
                    wc.style = 0;
                    wc.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(behindWndProcRef);
                    wc.hInstance = GetModuleHandle(null);
                    wc.hbrBackground = IntPtr.Zero;
                    wc.lpszClassName = "DogTagBehind";
                    ushort atom = RegisterClassExW(ref wc);
                    if (atom != 0) behindClassRegistered = true;
                    else
                    {
                        // Already registered by previous run/instance: fine.
                        behindClassRegistered = true;
                    }
                }
                catch { behindClassRegistered = true; }
            }
        }

        private IntPtr ResolveBehindParent()
        {
            // Parent INSIDE DefView (below SysListView32 icons, above DefView background).
            // Progman/WorkerW slots are fully covered by the opaque fullscreen DefView,
            // proven by red-probe invisibility in both. DefView background + transparent
            // ListView let a DefView child show with icons opaque over it.
            try
            {
                IntPtr progman = FindWindow("Progman", "Program Manager");
                if (progman != IntPtr.Zero && IsWindow(progman))
                {
                    IntPtr defView = FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);
                    if (defView != IntPtr.Zero && IsWindow(defView)) return defView;
                }
            }
            catch { }
            return IntPtr.Zero;
        }

        private void EnsureHandles()
        {
            const uint EX = 0x00080000 | 0x00000080;
            const uint WS_POPUP = 0x80000000;
            if ((rawTop == IntPtr.Zero || !IsWindow(rawTop)))
            {
                try
                {
                    // Detach stale subclass before the HWND is gone.
                    try { rawTopDragWindow?.ReleaseHandle(); } catch { }
                    rawTopDragWindow = null;
                    rawTop = CreateWindowExW(EX, "static", "DogTagRawTop", WS_POPUP, 0, 0, 1, 1,
                        IntPtr.Zero, IntPtr.Zero, GetModuleHandle(null), IntPtr.Zero);
                }
                catch { rawTop = IntPtr.Zero; }
            }
            // Attach (once) the drag subclass so Custom-unlocked can HTCAPTION-drag.
            // Pass-through for everything else keeps ULW presentation untouched.
            try
            {
                if (rawTop != IntPtr.Zero && IsWindow(rawTop))
                {
                    if (rawTopDragWindow == null || rawTopDragWindow.Handle != rawTop)
                    {
                        try { rawTopDragWindow?.ReleaseHandle(); } catch { }
                        rawTopDragWindow = new TopDragWindow(this);
                        rawTopDragWindow.AssignHandle(rawTop);
                    }
                }
            }
            catch { }
            // Dual-window design: rawBehind is persistent and may already be created.
            rawHandle = rawTop;
            try { UpdateCustomDragStyle(); } catch { }
        }

        private void EnsureRawHandle()
        {
            EnsureHandles();
        }

        private void InvalidateDesktopCache()
        {
            cachedWallpaperWorker = IntPtr.Zero;
            cachedIconHost = IntPtr.Zero;
            desktopDefView = IntPtr.Zero;
            desktopWallpaperWorker = IntPtr.Zero;
            desktopCacheTick = 0;
        }

        private const uint WS_EX_NOREDIRECTIONBITMAP = 0x00200000;

        private void EnsureDesktopLayer(bool force)
        {
            if (!force && desktopDefView != IntPtr.Zero && IsWindow(desktopDefView) && desktopCacheTick != 0 && (Environment.TickCount64 - desktopCacheTick) < 30000)
                return;
            try
            {
                IntPtr progman = FindWindow("Progman", "Program Manager");
                if (progman == IntPtr.Zero || !IsWindow(progman)) return;

                int ex = 0;
                try { ex = GetWindowLong(progman, GWL_EXSTYLE); } catch { }
                raisedDesktop = (ex & (int)WS_EX_NOREDIRECTIONBITMAP) != 0;

                bool needSpawn = force || desktopDefView == IntPtr.Zero || !IsWindow(desktopDefView);
                if (needSpawn)
                    SendMessageTimeout(progman, 0x052C, new IntPtr(0xD), new IntPtr(0x1), 0, 1000, out _);

                IntPtr iconHost = IntPtr.Zero;
                EnumWindows((hwnd, lp) =>
                {
                    try
                    {
                        if (FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero)
                            iconHost = hwnd;
                    }
                    catch { }
                    return true;
                }, IntPtr.Zero);
                if (iconHost == IntPtr.Zero || !IsWindow(iconHost))
                    iconHost = progman;

                IntPtr defView = IntPtr.Zero;
                try
                {
                    if (raisedDesktop)
                        defView = FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);
                    else
                        defView = FindWindowEx(iconHost, IntPtr.Zero, "SHELLDLL_DefView", null);
                }
                catch { }
                if (defView == IntPtr.Zero)
                    try { defView = FindWindowEx(iconHost, IntPtr.Zero, "SHELLDLL_DefView", null); } catch { }
                if (defView != IntPtr.Zero && IsWindow(defView)) desktopDefView = defView;

                iconListView = desktopDefView != IntPtr.Zero ? FindWindowEx(desktopDefView, IntPtr.Zero, "SysListView32", null) : iconListView;

                if (!raisedDesktop)
                {
                    IntPtr wall = IntPtr.Zero;
                    try { wall = FindWindowEx(IntPtr.Zero, iconHost, "WorkerW", null); } catch { }
                    if (wall == IntPtr.Zero || !IsWindow(wall))
                        try { wall = FindWindowEx(progman, IntPtr.Zero, "WorkerW", null); } catch { }
                    if (wall != IntPtr.Zero && IsWindow(wall)) desktopWallpaperWorker = wall;
                }
                else
                    desktopWallpaperWorker = IntPtr.Zero;

                desktopCacheTick = Environment.TickCount64;
                cachedIconHost = iconHost;
            }
            catch { }
        }

        private const uint LWA_COLORKEY = 1;

        // Each layer's LWA state applies only on change: no moves/repaints on
        // ticks or slider drags.
        private void SyncBehindAlpha()
        {
            int wantBg = Math.Clamp((int)Math.Round(windowOpacity * 255, MidpointRounding.AwayFromZero), 0, 255);
            int wantText = Math.Clamp((int)Math.Round(textOpacity * 255, MidpointRounding.AwayFromZero), 0, 255);
            try
            {
                if (rawBehind != IntPtr.Zero && IsWindow(rawBehind) && wantBg != lastBgLwa)
                {
                    SetLayeredWindowAttributes(rawBehind, 0, (byte)wantBg, LWA_ALPHA);
                    lastBgLwa = wantBg;
                }
            }
            catch { }
            try
            {
                if (rawBehindText != IntPtr.Zero && IsWindow(rawBehindText) && wantText != lastTextLwa)
                {
                    SetLayeredWindowAttributes(rawBehindText, TextKeyRef, (byte)wantText, LWA_COLORKEY | LWA_ALPHA);
                    lastTextLwa = wantText;
                }
            }
            catch { }
        }

        private IntPtr CreateBehindChild(string name)
        {
            IntPtr hwnd = IntPtr.Zero;
            try
            {
                const uint EX = 0x00080000 | 0x00000080;
                const uint WS_POPUP = 0x80000000;
                hwnd = CreateWindowExW(EX, "static", name, WS_POPUP, 0, 0, 1, 1,
                    IntPtr.Zero, IntPtr.Zero, GetModuleHandle(null), IntPtr.Zero);
            }
            catch { hwnd = IntPtr.Zero; }
            if (hwnd == IntPtr.Zero) return IntPtr.Zero;
            try
            {
                IntPtr progman = FindWindow("Progman", "Program Manager");
                if (progman == IntPtr.Zero) return IntPtr.Zero;
                int st = GetWindowLong(hwnd, GWL_STYLE);
                SetWindowLong(hwnd, GWL_STYLE, (st & ~WS_POPUP_R) | WS_CHILD);
                SetParent(hwnd, progman);
            }
            catch { }
            return hwnd;
        }

        private void EnsureRawBehind()
        {
            if (rawBehind != IntPtr.Zero && IsWindow(rawBehind)
                && rawBehindText != IntPtr.Zero && IsWindow(rawBehindText))
            {
                if (rawSubWindow == null || rawSubWindow.Handle != rawBehind)
                {
                    if (rawSubWindow != null) try { rawSubWindow.ReleaseHandle(); } catch { }
                    rawSubWindow = new RawDesktopHudWindow(this, false);
                    rawSubWindow.AssignHandle(rawBehind);
                }
                if (rawTextWindow == null || rawTextWindow.Handle != rawBehindText)
                {
                    if (rawTextWindow != null) try { rawTextWindow.ReleaseHandle(); } catch { }
                    rawTextWindow = new RawDesktopHudWindow(this, true);
                    rawTextWindow.AssignHandle(rawBehindText);
                }
                SyncBehindAlpha();
                try { UpdateCustomDragStyle(); } catch { }
                return;
            }
            EnsureHandles();
            if (rawTop == IntPtr.Zero || !IsWindow(rawTop)) return;
            IntPtr progman = FindWindow("Progman", "Program Manager");
            if (progman == IntPtr.Zero) return;
            IntPtr defView = IntPtr.Zero;
            try { defView = FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null); } catch { }
            try { if (rawBehind != IntPtr.Zero && IsWindow(rawBehind)) DestroyWindow(rawBehind); } catch { }
            try { if (rawBehindText != IntPtr.Zero && IsWindow(rawBehindText)) DestroyWindow(rawBehindText); } catch { }
            rawBehind = IntPtr.Zero;
            rawBehindText = IntPtr.Zero;
            lastBgLwa = -1;
            lastTextLwa = -1;
            rawBehind = CreateBehindChild("DogTagRawBehind");
            if (rawBehind == IntPtr.Zero) return;
            rawBehindText = CreateBehindChild("DogTagRawText");
            if (rawBehindText == IntPtr.Zero) return;
            if (rawSubWindow != null) try { rawSubWindow.ReleaseHandle(); } catch { }
            rawSubWindow = new RawDesktopHudWindow(this, false);
            rawSubWindow.AssignHandle(rawBehind);
            if (rawTextWindow != null) try { rawTextWindow.ReleaseHandle(); } catch { }
            rawTextWindow = new RawDesktopHudWindow(this, true);
            rawTextWindow.AssignHandle(rawBehindText);
            SyncBehindAlpha();
            try { UpdateCustomDragStyle(); } catch { }
            if (defView != IntPtr.Zero)
            {
                // Stack: DefView (icons) > text > panel.
                try { SetWindowPos(rawBehind, defView, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | 0x0080); } catch { }
                try { SetWindowPos(rawBehindText, defView, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | 0x0080); } catch { }
            }
            else
            {
                try { ShowWindow(rawBehind, 0); } catch { }
                try { ShowWindow(rawBehindText, 0); } catch { }
            }
        }

        private void RefreshDesktopHandles(bool force)
        {
            bool cacheValid = cachedWallpaperWorker != IntPtr.Zero && IsWindow(cachedWallpaperWorker)
                && cachedIconHost != IntPtr.Zero && IsWindow(cachedIconHost)
                && (Environment.TickCount64 - desktopCacheTick) < 30000;
            if (!force && cacheValid) return;
            try
            {
                IntPtr progman = FindWindow("Progman", "Program Manager");
                // Only poke Progman when we have no usable cache; spamming 0x052C every
                // refresh can recreate WorkerW and causes flicker.
                bool needSpawn = force || cachedWallpaperWorker == IntPtr.Zero || !IsWindow(cachedWallpaperWorker);
                if (progman != IntPtr.Zero && needSpawn)
                    SendMessageTimeout(progman, 0x052C, IntPtr.Zero, IntPtr.Zero, 0, 1000, out _);
                IntPtr workerHost = IntPtr.Zero;
                IntPtr progmanHost = IntPtr.Zero;
                EnumWindows((hwnd, lp) =>
                {
                    try
                    {
                        if (FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero)
                        {
                            var sb = new System.Text.StringBuilder(64);
                            GetClassName(hwnd, sb, sb.Capacity);
                            string cls = sb.ToString();
                            if (cls == "WorkerW")
                                workerHost = hwnd;
                            else if (hwnd == progman)
                                progmanHost = hwnd;
                        }
                    }
                    catch { }
                    return true;
                }, IntPtr.Zero);
                IntPtr iconHost = workerHost != IntPtr.Zero ? workerHost : (progmanHost != IntPtr.Zero ? progmanHost : progman);
                cachedIconHost = iconHost;
                IntPtr wallpaper = IntPtr.Zero;
                if (iconHost != IntPtr.Zero)
                    wallpaper = FindWindowEx(IntPtr.Zero, iconHost, "WorkerW", null);
                // wallpaper is the WorkerW spawned *after* the icon host: behind icons,
                // above wallpaper. If missing (older Windows / no slideshow), Progman
                // itself is a safe behind-icons parent.
                cachedWallpaperWorker = wallpaper != IntPtr.Zero ? wallpaper : progman;
                desktopCacheTick = Environment.TickCount64;
                IntPtr defView = iconHost != IntPtr.Zero ? FindWindowEx(iconHost, IntPtr.Zero, "SHELLDLL_DefView", null) : IntPtr.Zero;
                iconListView = defView != IntPtr.Zero ? FindWindowEx(defView, IntPtr.Zero, "SysListView32", null) : IntPtr.Zero;
            }
            catch { }
        }

        private IntPtr ResolveDesiredParent()
        {
            // "Display over windows" wins: must be top-level TOPMOST, above icons too.
            if (displayOverWindows) return IntPtr.Zero;
            if (displayOverShortcuts) return IntPtr.Zero;
            RefreshDesktopHandles(false);
            if (cachedWallpaperWorker != IntPtr.Zero && IsWindow(cachedWallpaperWorker))
                return cachedWallpaperWorker;
            try
            {
                IntPtr progman = FindWindow("Progman", "Program Manager");
                if (progman != IntPtr.Zero) return progman;
            }
            catch { }
            return IntPtr.Zero;
        }

        private void PlaceOnDesktop(int x, int y, int w, int h)
        {
            EnsureHandles();
            IntPtr active = rawTop;
            if (active == IntPtr.Zero || !IsWindow(active)) return;
            rawHandle = active;
            if (!hudEnabled)
            {
                try
                {
                    if (IsWindowVisible(rawTop)) ShowWindow(rawTop, 0);
                }
                catch { }
                try { if (IsWindow(rawBehind) && IsWindowVisible(rawBehind)) ShowWindow(rawBehind, 0); } catch { }
                try { if (IsWindow(rawBehindText) && IsWindowVisible(rawBehindText)) ShowWindow(rawBehindText, 0); } catch { }
                try { ClearLegacyBurnsOnce(); } catch { }
                rawShown = false;
                placedParent = new IntPtr(-1);
                return;
            }

            var next = new Rectangle(x, y, w, h);

            bool behindIcons = !displayOverShortcuts && !displayOverWindows;

            if (behindIcons)
            {
                EnsureHandles();
                EnsureRawBehind();
                if (rawBehind == IntPtr.Zero || !IsWindow(rawBehind)) return;

                IntPtr progman = FindWindow("Progman", "Program Manager");
                if (progman == IntPtr.Zero) return;
                IntPtr defView = IntPtr.Zero;
                try { defView = FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null); } catch { }

                int px = x, py = y;
                try
                {
                    var pt = new WinPoint { X = x, Y = y };
                    ScreenToClient(progman, ref pt);
                    px = pt.X; py = pt.Y;
                }
                catch { }
                IntPtr anchor = defView != IntPtr.Zero ? defView : IntPtr.Zero;
                bool sameBounds = placedBounds == next
                    && placedParent == progman
                    && rawBehind != IntPtr.Zero
                    && IsWindow(rawBehind)
                    && IsWindowVisible(rawBehind)
                    && rawBehindText != IntPtr.Zero
                    && IsWindow(rawBehindText)
                    && IsWindowVisible(rawBehindText);
                if (!sameBounds)
                {
                    // Panel first, then text: both under DefView gives
                    // DefView (icons) > text > panel.
                    SetWindowPos(rawBehind, anchor, px, py, w, h, SWP_NOACTIVATE | SWP_SHOWWINDOW | SWP_FRAMECHANGED);
                    SetWindowPos(rawBehindText, anchor, px, py, w, h, SWP_NOACTIVATE | SWP_SHOWWINDOW | SWP_FRAMECHANGED);
                }
                SyncBehindAlpha();

                // Hide the top-level HUD.
                try { if (rawTop != IntPtr.Zero && IsWindow(rawTop) && IsWindowVisible(rawTop)) ShowWindow(rawTop, 0); } catch { }
                rawHandle = rawBehind;
                rawShown = true;
                placedParent = progman;
                placedBounds = next;
                return;
            }

            // Top-level modes.
            EnsureHandles();
            if (rawTop == IntPtr.Zero || !IsWindow(rawTop)) return;
            if (rawBehind != IntPtr.Zero && IsWindow(rawBehind) && IsWindowVisible(rawBehind))
                try { ShowWindow(rawBehind, 0); } catch { }
            if (rawBehindText != IntPtr.Zero && IsWindow(rawBehindText) && IsWindowVisible(rawBehindText))
                try { ShowWindow(rawBehindText, 0); } catch { }

            // Ensure child subclass isn't attached.
            if (rawSubWindow != null) { try { rawSubWindow.ReleaseHandle(); } catch { } rawSubWindow = null; }

            // Ensure rawTop is top-level.
            if (GetParent(rawTop) != IntPtr.Zero)
            {
                try { SetWindowLong(rawTop, GWL_STYLE, (GetWindowLong(rawTop, GWL_STYLE) & ~WS_CHILD) | WS_POPUP_R); } catch { }
                try { SetParent(rawTop, IntPtr.Zero); } catch { }
            }
            placedParent = IntPtr.Zero;

            bool wantTopmost = displayOverWindows;
            bool visible = false;
            try { visible = IsWindowVisible(rawTop); } catch { }
            if (!visible)
            {
                SetWindowPos(rawTop, wantTopmost ? HWND_TOPMOST : HWND_NOTOPMOST,
                    x, y, w, h, SWP_NOACTIVATE | SWP_SHOWWINDOW);
                rawShown = true;
            }
            else
            {
                // Only touch z-order when the current state differs from the desired
                // one: repeated SetWindowPos on every periodic refresh causes flicker.
                bool isTopmost = false;
                try { isTopmost = (GetWindowLong(rawTop, GWL_EXSTYLE) & 0x00000008) != 0; } catch { }
                if (wantTopmost && !isTopmost)
                    SetWindowPos(rawTop, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE);
                else if (!wantTopmost && isTopmost)
                    SetWindowPos(rawTop, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE);
            }
            rawHandle = rawTop;
            placedBounds = next;
        }


        // OFF-mode icon holes: pixel-diff ground truth. Captured screen pixels that
        // are bright where our own render is dark = foreign icon pixels above us.
        // Hysteresis (2 consecutive confirmations) ignores transient popups.
        private bool[] holeMask = new bool[0];
        private byte[] holeConfirm = new byte[0];
        private int holeW, holeH, holeX, holeY;
        private byte[] lastHudRgb;
        private int presentCount;

        private void ApplyIconHoles(Bitmap bmp, int x, int y, int w, int h)
        {
            presentCount++;
            bool sizeChanged = holeW != w || holeH != h || holeX != x || holeY != y;
            if (sizeChanged)
            {
                holeW = w; holeH = h; holeX = x; holeY = y;
                holeMask = new bool[w * h];
                holeConfirm = new byte[w * h];
                lastHudRgb = null;
            }
            // Refresh mask every 5th present (~10s): capture composited screen and
            // diff against our last unholed render.
            if (lastHudRgb != null && presentCount % 5 == 0)
            {
                try { RefreshHoleMask(x, y, w, h); } catch { }
            }
            // Snapshot current unholed render for the NEXT diff cycle.
            try
            {
                var snap = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                try
                {
                    int sstride = Math.Abs(snap.Stride);
                    var sraw = new byte[sstride * h];
                    Marshal.Copy(snap.Scan0, sraw, 0, sraw.Length);
                    var rgb = new byte[w * h * 3];
                    for (int r = 0; r < h; r++)
                        for (int c = 0; c < w; c++)
                        {
                            rgb[(r * w + c) * 3] = sraw[r * sstride + c * 4 + 2];
                            rgb[(r * w + c) * 3 + 1] = sraw[r * sstride + c * 4 + 1];
                            rgb[(r * w + c) * 3 + 2] = sraw[r * sstride + c * 4];
                        }
                    lastHudRgb = rgb;
                }
                finally { bmp.UnlockBits(snap); }
            }
            catch { lastHudRgb = null; }
            if (holeMask == null || holeMask.Length != w * h) return;
            bool any = false;
            for (int i = 0; i < holeMask.Length; i++) if (holeMask[i]) { any = true; break; }
            if (!any) return;
            var data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            try
            {
                int stride = Math.Abs(data.Stride);
                var row = new byte[stride];
                for (int r = 0; r < h; r++)
                {
                    Marshal.Copy(IntPtr.Add(data.Scan0, r * stride), row, 0, stride);
                    for (int c = 0; c < w; c++)
                    {
                        if (holeMask[r * w + c])
                        {
                            row[c * 4] = 0; row[c * 4 + 1] = 0; row[c * 4 + 2] = 0; row[c * 4 + 3] = 0;
                        }
                    }
                    Marshal.Copy(row, 0, IntPtr.Add(data.Scan0, r * stride), stride);
                }
            }
            finally { bmp.UnlockBits(data); }
        }

        private void RefreshHoleMask(int x, int y, int w, int h)
        {
            if (lastHudRgb == null || lastHudRgb.Length != w * h * 3) return;
            byte[] cap;
            using (var shot = new Bitmap(w, h, PixelFormat.Format32bppArgb))
            {
                using (var g = Graphics.FromImage(shot))
                    g.CopyFromScreen(x, y, 0, 0, new Size(w, h));
                var data = shot.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                try
                {
                    int stride = Math.Abs(data.Stride);
                    cap = new byte[stride * h];
                    Marshal.Copy(data.Scan0, cap, 0, cap.Length);
                    // Pass 1: per-pixel foreign map (screen differs clearly from our
                    // render, either direction). Matching pixels (own text/bg) untouched.
                    var foreign = new bool[w * h];
                    int foreignCount = 0;
                    for (int r = 0; r < h; r++)
                    {
                        for (int c = 0; c < w; c++)
                        {
                            int si = r * stride + c * 4;
                            int hi = (r * w + c) * 3;
                            int cr = cap[si + 2] & 255, cg = cap[si + 1] & 255, cb = cap[si] & 255;
                            int hr = lastHudRgb[hi] & 255, hg = lastHudRgb[hi + 1] & 255, hb = lastHudRgb[hi + 2] & 255;
                            int diff = Math.Abs(cr - hr);
                            int d2 = Math.Abs(cg - hg);
                            if (d2 > diff) diff = d2;
                            d2 = Math.Abs(cb - hb);
                            if (d2 > diff) diff = d2;
                            if (diff > 90) { foreign[r * w + c] = true; foreignCount++; }
                        }
                    }
                    // A normal window parked over us differs almost everywhere: freeze
                    // the mask (don't punch window-shaped holes, don't decay either).
                    if (foreignCount > w * h * 40 / 100) return;
                    // Pass 2: keep only icon-sized blobs (glyphs/labels), ignore large
                    // regions. Dilate 1px so edges stay clean.
                    var seen = new bool[w * h];
                    var stack = new int[w * h];
                    for (int i = 0; i < w * h; i++)
                    {
                        if (!foreign[i] || seen[i]) continue;
                        int top = 0, minx = w, maxx = -1, miny = h, maxy = -1, n = 0;
                        stack[top++] = i;
                        seen[i] = true;
                        while (top > 0 && n < w * h)
                        {
                            int cur = stack[--top];
                            n++;
                            int cc = cur % w, rr = cur / w;
                            if (cc < minx) minx = cc;
                            if (cc > maxx) maxx = cc;
                            if (rr < miny) miny = rr;
                            if (rr > maxy) maxy = rr;
                            if (cc > 0 && foreign[cur - 1] && !seen[cur - 1]) { seen[cur - 1] = true; stack[top++] = cur - 1; }
                            if (cc < w - 1 && foreign[cur + 1] && !seen[cur + 1]) { seen[cur + 1] = true; stack[top++] = cur + 1; }
                            if (rr > 0 && foreign[cur - w] && !seen[cur - w]) { seen[cur - w] = true; stack[top++] = cur - w; }
                            if (rr < h - 1 && foreign[cur + w] && !seen[cur + w]) { seen[cur + w] = true; stack[top++] = cur + w; }
                        }
                        if (n >= 2 && maxx - minx <= 130 && maxy - miny <= 140)
                        {
                            for (int rr = Math.Max(0, miny - 1); rr <= Math.Min(h - 1, maxy + 1); rr++)
                                for (int cc = Math.Max(0, minx - 1); cc <= Math.Min(w - 1, maxx + 1); cc++)
                                {
                                    // Only confirm pixels inside the blob's area.
                                    bool inside = false;
                                    for (int rr2 = Math.Max(0, rr - 1); rr2 <= Math.Min(h - 1, rr + 1) && !inside; rr2++)
                                        for (int cc2 = Math.Max(0, cc - 1); cc2 <= Math.Min(w - 1, cc + 1); cc2++)
                                            if (foreign[rr2 * w + cc2]) { inside = true; break; }
                                    if (!inside) continue;
                                    int ii = rr * w + cc;
                                    if (holeConfirm[ii] < 2) holeConfirm[ii]++;
                                    if (holeConfirm[ii] >= 2) holeMask[ii] = true;
                                }
                        }
                    }
                    // Decay holes with no foreign pixel nearby (icon moved away).
                    for (int idx = 0; idx < w * h; idx++)
                    {
                        if (!holeMask[idx]) { if (holeConfirm[idx] > 0 && !foreign[idx]) holeConfirm[idx]--; continue; }
                        int cc = idx % w, rr = idx / w;
                        bool nearForeign = false;
                        for (int rr2 = Math.Max(0, rr - 2); rr2 <= Math.Min(h - 1, rr + 2) && !nearForeign; rr2++)
                            for (int cc2 = Math.Max(0, cc - 2); cc2 <= Math.Min(w - 1, cc + 2); cc2++)
                                if (foreign[rr2 * w + cc2]) { nearForeign = true; break; }
                        if (!nearForeign)
                        {
                            if (holeConfirm[idx] > 0) holeConfirm[idx]--;
                            if (holeConfirm[idx] == 0) holeMask[idx] = false;
                        }
                        else if (holeConfirm[idx] < 2) holeConfirm[idx] = 2;
                    }
                }
                finally { shot.UnlockBits(data); }
            }
        }

        private void PunchIconHoles(Bitmap bmp, int x, int y, int w, int h)
        {
            List<Rectangle> icons;
            try { icons = GetIconRects(); } catch { return; }
            if (icons == null || icons.Count == 0) return;
            var hud = new Rectangle(x, y, w, h);
            bool any = false;
            foreach (var ir in icons)
            {
                try { if (hud.IntersectsWith(ir)) { any = true; break; } } catch { }
            }
            if (!any) return;
            using (var g = Graphics.FromImage(bmp))
            {
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                using (var clear = new SolidBrush(Color.FromArgb(0, 0, 0, 0)))
                {
                    foreach (var ir in icons)
                    {
                        try
                        {
                            if (!hud.IntersectsWith(ir)) continue;
                            var inter = Rectangle.Intersect(hud, ir);
                            inter.X -= x; inter.Y -= y;
                            // Slight inset so HUD edge doesn't touch glyph pixels.
                            inter.Inflate(-2, -2);
                            if (inter.Width > 0 && inter.Height > 0)
                                g.FillRectangle(clear, inter);
                        }
                        catch { }
                    }
                }
            }
        }

        private void PresentBehindIcons(int x, int y, int w, int h)
        {
            if (w <= 0 || h <= 0) return;
            if (!hudEnabled) return;
            EnsureHandles();
            EnsureRawBehind();
            IntPtr target = rawBehind;
            if (target == IntPtr.Zero || !IsWindow(target)) return;
            rawHandle = target;
            try { EnsureIconBackgroundTransparent(new Rectangle(x, y, w, h)); } catch { }
            SyncBehindAlpha();
            try { InvalidateRect(target, IntPtr.Zero, false); } catch { }
            try { if (rawBehindText != IntPtr.Zero && IsWindow(rawBehindText)) InvalidateRect(rawBehindText, IntPtr.Zero, false); } catch { }
            placedBounds = new Rectangle(x, y, w, h);
            rawShown = true;
        }

        // Identical-frame skip (top-level path only): re-rendering + ULW
        // upload every tick is the biggest per-tick cost when idle. Skip only
        // when every pixel input matches the last SUCCESSFUL present and the
        // geometry already equals the placed bounds (so the skip is a pure
        // no-op). Any content/setting/geometry change, any invalidation
        // (they all reset placedBounds), a dead target, a hidden window, or
        // the 30s heartbeat forces a fresh present. Behind-icons/child mode
        // always presents (wallpaper-blt maintenance lives in that path).
        private string lastPresentKey = null;
        private long lastPresentTick = 0;

        private string BuildPresentKey(string[] arr, int x, int y, int w, int h)
        {
            try
            {
                string lines = arr != null ? string.Join("\n", arr) : "";
                return lines + "\n|x=" + x + ",y=" + y + ",w=" + w + ",h=" + h
                    + "|op=" + windowOpacity.ToString("F3")
                    + "|tx=" + textOpacity.ToString("F3")
                    + "|tint=" + pawTint.ToArgb().ToString()
                    + "|pa=" + pawOpacity.ToString("F3")
                    + "|prof=" + textColorProfile + "|sp=" + textSpacing + "|f=" + fontChoice
                    + "|ml=" + (mirrorLayout ? 1 : 0) + "|ma=" + (mirrorArt ? 1 : 0)
                    + "|ol=" + (showTextOutline ? 1 : 0)
                    + "|os=" + (displayOverShortcuts ? 1 : 0) + "|ow=" + (displayOverWindows ? 1 : 0)
                    + "|a=" + anchorIndex + "|m=" + monitorIndex;
            }
            catch { return null; }
        }

        private bool ShouldSkipPresent(string[] arr, string presentKey, int x, int y, int w, int h)
        {
            try
            {
                if (WantChild()) return false;
                if (placedBounds == Rectangle.Empty) return false;
                if (placedBounds != new Rectangle(x, y, w, h)) return false;
                if (!rawShown) return false;
                if (rawTop == IntPtr.Zero || !IsWindow(rawTop)) return false;
                if (lastPresentKey == null) return false;
                if (lastPresentTick == 0 || Environment.TickCount64 - lastPresentTick > 30000) return false;
                if (presentKey == null) return false;
                return string.Equals(presentKey, lastPresentKey, StringComparison.Ordinal);
            }
            catch { return false; }
        }

        private void PresentOverShortcuts(int x, int y, int w, int h, string presentKey)
        {
            if (w <= 0 || h <= 0) return;
            if (!hudEnabled) return;
            EnsureHandles();
            IntPtr target = rawTop;
            if (target == IntPtr.Zero || !IsWindow(target)) return;
            bool isChild = false;
            IntPtr actualParent = IntPtr.Zero;
            rawHandle = target;

            int bgAlpha = Math.Clamp((int)Math.Round(windowOpacity * 255, MidpointRounding.AwayFromZero), 0, 255);
            using (var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb))
            {
                using (var g = Graphics.FromImage(bmp))
                    DrawHud(g, bgAlpha, textOpacity);
                // Keep at least one fully-opaque-ish pixel so the DWM compositor
                // cannot classify this zero-background HUD window as fully empty.
                if (true)
                {
                    var s = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    try
                    {
                        Marshal.WriteByte(s.Scan0, s.Stride * (h - 1) + (w - 1) * 4 + 3, 1);
                    }
                    finally { bmp.UnlockBits(s); }
                }

                var data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                int stride = Math.Abs(data.Stride);
                var raw = new byte[stride * h];
                Marshal.Copy(data.Scan0, raw, 0, raw.Length);
                bmp.UnlockBits(data);

                var packed = new byte[w * h * 4];
                for (int row = 0; row < h; row++)
                {
                    var srcOff = row * stride;
                    var dstOff = row * w * 4;
                    Buffer.BlockCopy(raw, srcOff, packed, dstOff, w * 4);
                    for (int i = 0; i < w; i++)
                    {
                        int b = packed[dstOff + i * 4] & 255;
                        int g = packed[dstOff + i * 4 + 1] & 255;
                        int r = packed[dstOff + i * 4 + 2] & 255;
                        int a = packed[dstOff + i * 4 + 3] & 255;
                        packed[dstOff + i * 4] = (byte)(b * a / 255);
                        packed[dstOff + i * 4 + 1] = (byte)(g * a / 255);
                        packed[dstOff + i * 4 + 2] = (byte)(r * a / 255);
                    }
                }
                // Keep a faint one-pixel sentinel in the bottom-right corner so the
                // desktop compositor cannot classify this under-band HUD as fully
                // empty when window opacity is reduced (which made it vanish).
                {
                    int idx = (h - 1) * w * 4 + (w - 1) * 4;
                    if (packed[idx + 3] == 0)
                        packed[idx + 3] = 8;
                }
                IntPtr screen = GetDC(IntPtr.Zero);
                IntPtr mem = CreateCompatibleDC(screen);
                var bmi = new BitmapInfo();
                bmi.Header.biSize = 40;
                bmi.Header.biWidth = w;
                bmi.Header.biHeight = -h;
                bmi.Header.biPlanes = 1;
                bmi.Header.biBitCount = 32;
                IntPtr bits;
                IntPtr dib = CreateDIBSection(screen, ref bmi, 0, out bits, IntPtr.Zero, 0);
                bool ok = false;
                IntPtr old = IntPtr.Zero;
                try
                {
                    if (dib == IntPtr.Zero || bits == IntPtr.Zero) return;
                    Marshal.Copy(packed, 0, bits, packed.Length);
                    old = SelectObject(mem, dib);
                    var blend = new BlendFunction { BlendOp = AC_SRC_OVER, BlendFlags = 0, SourceConstantAlpha = 255, AlphaFormat = AC_SRC_ALPHA };
                    {
                        // Top-level (above icons, TOPMOST when DOW): single atomic
                        // move+resize+content. No SetWindowPos for position.
                        var dst = new WinPoint { X = x, Y = y };
                        var src = new WinPoint();
                        var size = new WinSize { cx = w, cy = h };
                        ok = UpdateLayeredWindow(target, screen, ref dst, ref size, mem, ref src, 0, ref blend, ULW_ALPHA);
                    }
                }
                finally
                {
                    try { if (old != IntPtr.Zero) SelectObject(mem, old); } catch { }
                    if (dib != IntPtr.Zero) DeleteObject(dib);
                    DeleteDC(mem);
                    ReleaseDC(IntPtr.Zero, screen);
                    if (ok)
                    {
                        placedBounds = new Rectangle(x, y, w, h);
                        placedParent = isChild ? actualParent : IntPtr.Zero;
                        rawShown = true;
                        lastPresentKey = presentKey;
                        lastPresentTick = Environment.TickCount64;
                    }
                    else
                    {
                        // Retry next tick instead of skipping on a stale key.
                        lastPresentKey = null;
                        try
                        {
                            int err = Marshal.GetLastWin32Error();
                            string log = Path.Combine(Path.GetTempPath(), "dogtag_ulw.log");
                            File.AppendAllText(log, DateTime.Now.ToString("HH:mm:ss") + " ULW fail child=" + isChild + " w=" + w + " h=" + h + " err=" + err + " target=" + target + " x=" + x + " y=" + y + Environment.NewLine);
                        }
                        catch { }
                    }
                }
            }
        }

        private sealed class TopDragWindow : NativeWindow
        {
            private readonly DesktopHud owner;
            public TopDragWindow(DesktopHud owner) { this.owner = owner; }
            protected override void WndProc(ref Message m)
            {
                try
                {
                    if (m.Msg == WM_NCHITTEST_C)
                    {
                        if (owner.IsCustomDragArmed())
                        {
                            if (!owner.nchHitLogged)
                            {
                                owner.nchHitLogged = true;
                                int sx, sy;
                                owner.GetWindowScreenPos(m.HWnd, out sx, out sy);
                                DragLog("nchittest-armed hwnd=" + m.HWnd + " at=" + sx + "," + sy);
                            }
                            m.Result = new IntPtr(HTCAPTION_C);
                            return;
                        }
                        if (owner.anchorIndex == 4 && owner.customLocked)
                        {
                            m.Result = HTTRANSPARENT_C;
                            return;
                        }
                    }
                    else if (m.Msg == WM_LBUTTONDOWN_C)
                    {
                        // Only reached when the HTCAPTION system loop did NOT
                        // take the gesture: own it manually with capture.
                        if (owner.TryBeginManualDrag(m.HWnd))
                        {
                            m.Result = IntPtr.Zero;
                            return;
                        }
                    }
                    else if (m.Msg == WM_MOUSEMOVE_C)
                    {
                        if (owner.manualDragHwnd == m.HWnd && owner.manualDragHwnd != IntPtr.Zero)
                        {
                            try { owner.UpdateManualDrag(m.HWnd); } catch { }
                        }
                    }
                    else if (m.Msg == WM_CAPTURECHANGED_C)
                    {
                        if (owner.manualDragHwnd == m.HWnd && owner.manualDragHwnd != IntPtr.Zero)
                        {
                            try { owner.EndManualDrag(m.HWnd, "capture-lost"); } catch { }
                        }
                    }
                    else if (m.Msg == WM_ENTERSIZEMOVE_C)
                    {
                        if (owner.IsCustomDragArmed())
                        {
                            owner.MarkCustomDragStart();
                            DragLog("entersizemove hwnd=" + m.HWnd);
                        }
                    }
                    else if (m.Msg == WM_EXITSIZEMOVE_C)
                    {
                        if (owner.anchorIndex == 4 && (owner.customDragActive || !owner.customLocked))
                        {
                            int sx, sy;
                            owner.GetWindowScreenPos(m.HWnd, out sx, out sy);
                            DragLog("exitsizemove hwnd=" + m.HWnd + " at=" + sx + "," + sy);
                            try { owner.OnCustomDragFinished(m.HWnd); } catch { owner.customDragActive = false; }
                        }
                    }
                    else if (m.Msg == WM_MOVE_C)
                    {
                        // ENTERSIZEMOVE is the normal start signal; a MOVE only
                        // starts a session while the button is held, so
                        // programmatic repositions can never start a phantom
                        // drag (which would freeze refreshes until the watchdog).
                        if (owner.IsCustomDragArmed())
                        {
                            bool held = false;
                            try { held = (GetAsyncKeyState(VK_LBUTTON_G) & 0x8000) != 0; } catch { }
                            if (owner.customDragActive || held) owner.MarkCustomDragStart();
                            try { owner.OnCustomDragMove(m.HWnd); } catch { }
                        }
                    }
                    else if (m.Msg == WM_LBUTTONUP_C)
                    {
                        if (owner.manualDragHwnd == m.HWnd && owner.manualDragHwnd != IntPtr.Zero)
                        {
                            try { owner.EndManualDrag(m.HWnd, "lbuttonup"); } catch { }
                        }
                        else if (owner.anchorIndex == 4 && owner.customDragActive)
                        {
                            try { owner.OnCustomDragFinished(m.HWnd); } catch { owner.customDragActive = false; }
                        }
                    }
                }
                catch { }
                base.WndProc(ref m);
            }
        }

        // Floating grip shown while the Custom anchor is unlocked. A real
        // borderless WinForms form: raw popup HWNDs on this machine receive
        // hover hit-tests but never button presses, while standard windows
        // (like the Settings form) get full mouse input. The HUD follows the
        // form via the shared FollowHudToHandle/FinishHandleDrag math.
        private sealed class DragHandleForm : Form
        {
            private readonly DesktopHud owner;
            private bool dragging;
            private Point grabOffset;
            private Font gripFont;

            public DragHandleForm(DesktopHud owner)
            {
                this.owner = owner;
                FormBorderStyle = FormBorderStyle.None;
                ShowInTaskbar = false;
                StartPosition = FormStartPosition.Manual;
                AutoScaleMode = AutoScaleMode.Dpi;
                Size = new Size(DragHandleW, DragHandleH);
                BackColor = Color.FromArgb(24, 28, 36);
                ForeColor = Color.White;
                Cursor = Cursors.SizeAll;
                try { gripFont = new Font("Segoe UI", 9f, FontStyle.Bold); } catch { }
                try { TopMost = owner.displayOverWindows; } catch { }
                Visible = false;
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    var cp = base.CreateParams;
                    cp.ExStyle |= 0x00000080; // TOOLWINDOW: no taskbar, no Alt+Tab
                    return cp;
                }
            }

            public void SyncTopMost(bool topmost)
            {
                try { if (TopMost != topmost) TopMost = topmost; } catch { }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                try
                {
                    var g = e.Graphics;
                    g.Clear(Color.FromArgb(255, 24, 28, 36));
                    using (var pen = new Pen(Color.DodgerBlue, 2))
                        g.DrawRectangle(pen, 1, 1, Math.Max(2, Width - 2), Math.Max(2, Height - 2));
                    if (gripFont != null)
                    {
                        TextRenderer.DrawText(g, "⠿  Drag DogTag", gripFont, ClientRectangle, Color.White,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
                    }
                }
                catch { }
                base.OnPaint(e);
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                try
                {
                    if (e.Button == MouseButtons.Left && owner.IsCustomDragArmed() && !dragging)
                    {
                        dragging = true;
                        try
                        {
                            Point cur = Cursor.Position;
                            grabOffset = new Point(cur.X - Left, cur.Y - Top);
                        }
                        catch { grabOffset = new Point(DragHandleW / 2, DragHandleH / 2); }
                        try { Capture = true; } catch { }
                        owner.MarkCustomDragStart();
                        owner.SnapshotHandleSession();
                        DragLog("form-lbdown at=" + Left + "," + Top);
                    }
                }
                catch { }
                base.OnMouseDown(e);
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                try
                {
                    if (dragging && owner.customDragActive)
                    {
                        Point cur = Cursor.Position;
                        int nx = cur.X - grabOffset.X, ny = cur.Y - grabOffset.Y;
                        if (Left != nx || Top != ny)
                        {
                            Left = nx;
                            Top = ny;
                        }
                        try { owner.FollowHudToHandle(); } catch { }
                    }
                }
                catch { }
                base.OnMouseMove(e);
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                bool was = dragging;
                dragging = false;
                try { if (Capture) Capture = false; } catch { }
                if (was)
                {
                    try { owner.FinishHandleDrag("form-up"); } catch { }
                }
                base.OnMouseUp(e);
            }

            protected override void WndProc(ref Message m)
            {
                const int WM_CAPTURECHANGED = 0x0215;
                if (m.Msg == WM_CAPTURECHANGED && dragging)
                {
                    dragging = false;
                    try { owner.FinishHandleDrag("form-capture-lost"); } catch { }
                }
                base.WndProc(ref m);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing) { try { gripFont?.Dispose(); } catch { } gripFont = null; }
                base.Dispose(disposing);
            }
        }

        private sealed class RawDesktopHudWindow : NativeWindow
        {
            private readonly DesktopHud owner;
            private readonly bool isTextLayer;
            public RawDesktopHudWindow(DesktopHud owner, bool isTextLayer = false) { this.owner = owner; this.isTextLayer = isTextLayer; }

            protected override void WndProc(ref Message m)
            {
                // Custom-anchor drag affordance for behind-icons layers. Painting
                // behaviour below is untouched.
                try
                {
                    if (m.Msg == WM_NCHITTEST_C)
                    {
                        if (owner.IsCustomDragArmed())
                        {
                            if (!owner.nchHitLogged)
                            {
                                owner.nchHitLogged = true;
                                DragLog("nchittest-armed-behind hwnd=" + m.HWnd);
                            }
                            m.Result = new IntPtr(HTCAPTION_C);
                            return;
                        }
                        if (owner.anchorIndex == 4 && owner.customLocked)
                        {
                            m.Result = HTTRANSPARENT_C;
                            return;
                        }
                    }
                    else if (m.Msg == WM_LBUTTONDOWN_C)
                    {
                        if (owner.TryBeginManualDrag(m.HWnd))
                        {
                            m.Result = IntPtr.Zero;
                            return;
                        }
                    }
                    else if (m.Msg == WM_MOUSEMOVE_C)
                    {
                        if (owner.manualDragHwnd == m.HWnd && owner.manualDragHwnd != IntPtr.Zero)
                        {
                            try { owner.UpdateManualDrag(m.HWnd); } catch { }
                        }
                    }
                    else if (m.Msg == WM_CAPTURECHANGED_C)
                    {
                        if (owner.manualDragHwnd == m.HWnd && owner.manualDragHwnd != IntPtr.Zero)
                        {
                            try { owner.EndManualDrag(m.HWnd, "capture-lost"); } catch { }
                        }
                    }
                    else if (m.Msg == WM_ENTERSIZEMOVE_C)
                    {
                        if (owner.IsCustomDragArmed()) owner.MarkCustomDragStart();
                    }
                    else if (m.Msg == WM_EXITSIZEMOVE_C)
                    {
                        if (owner.anchorIndex == 4 && (owner.customDragActive || !owner.customLocked))
                        {
                            try { owner.OnCustomDragFinished(m.HWnd); } catch { owner.customDragActive = false; }
                            // Fall through to paint handling below? No: EXITSIZEMOVE
                            // has no paint work; return after base below.
                        }
                    }
                    else if (m.Msg == WM_MOVE_C)
                    {
                        // Same button-held guard as the top-level window: behind
                        // children are repositioned programmatically on every
                        // present, which must not start a phantom drag session.
                        if (owner.IsCustomDragArmed())
                        {
                            bool held = false;
                            try { held = (GetAsyncKeyState(VK_LBUTTON_G) & 0x8000) != 0; } catch { }
                            if (owner.customDragActive || held) owner.MarkCustomDragStart();
                            try { owner.OnCustomDragMove(m.HWnd); } catch { }
                        }
                    }
                    else if (m.Msg == WM_LBUTTONUP_C)
                    {
                        if (owner.manualDragHwnd == m.HWnd && owner.manualDragHwnd != IntPtr.Zero)
                        {
                            try { owner.EndManualDrag(m.HWnd, "lbuttonup"); } catch { }
                        }
                        else if (owner.anchorIndex == 4 && owner.customDragActive)
                        {
                            try { owner.OnCustomDragFinished(m.HWnd); } catch { owner.customDragActive = false; }
                        }
                    }
                }
                catch { }
                const int WM_PAINT = 0x000F;
                const int WM_ERASEBKGND = 0x0014;
                if (m.Msg == WM_PAINT)
                {
                    try
                    {
                        var cr = new RECT();
                        if (GetClientRect(m.HWnd, out cr))
                        {
                            int cw = cr.Right - cr.Left;
                            int ch = cr.Bottom - cr.Top;
                            if (cw > 0 && ch > 0)
                            {
                                using (var bmp = new Bitmap(cw, ch, PixelFormat.Format32bppArgb))
                                {
                                    if (isTextLayer)
                                    {
                                        using (var mg = Graphics.FromImage(bmp))
                                        {
                                            if (owner.windowOpacity >= 0.35)
                                            {
                                                mg.Clear(PanelColor);
                                                owner.DrawTextGlyphs(mg, true);
                                            }
                                            else
                                            {
                                                // Faint panel: panel-blended AA edges would
                                                // float over wallpaper as a halo, so use
                                                // crisp exact-keyed rendering instead.
                                                owner.DrawTextOnKey(mg);
                                            }
                                        }
                                        // Key out only untouched panel pixels: glyph
                                        // bodies and smooth panel-blended AA edges stay.
                                        var lk = bmp.LockBits(new Rectangle(0, 0, cw, ch), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                                        try
                                        {
                                            int lstride = Math.Abs(lk.Stride);
                                            var lrow = new byte[lstride];
                                            for (int r = 0; r < ch; r++)
                                            {
                                                Marshal.Copy(IntPtr.Add(lk.Scan0, r * lstride), lrow, 0, lstride);
                                                for (int c = 0; c < cw; c++)
                                                {
                                                    int o = c * 4;
                                                    if (lrow[o] == 32 && lrow[o + 1] == 24 && lrow[o + 2] == 20)
                                                    { lrow[o] = 3; lrow[o + 1] = 2; lrow[o + 2] = 1; }
                                                }
                                                Marshal.Copy(lrow, 0, IntPtr.Add(lk.Scan0, r * lstride), lstride);
                                            }
                                        }
                                        finally { bmp.UnlockBits(lk); }
                                    }
                                    else
                                    {
                                        using (var mg = Graphics.FromImage(bmp))
                                            owner.DrawPanelOpaque(mg);
                                    }
                                    using (var g = Graphics.FromHwnd(m.HWnd))
                                        g.DrawImageUnscaled(bmp, 0, 0);
                                }
                            }
                            else
                            {
                                using (var g = Graphics.FromHwnd(m.HWnd))
                                {
                                    if (isTextLayer)
                                        owner.DrawTextOnKey(g);
                                    else
                                        owner.DrawPanelOpaque(g);
                                }
                            }
                        }
                        else
                        {
                            using (var g = Graphics.FromHwnd(m.HWnd))
                            {
                                if (isTextLayer)
                                    owner.DrawTextOnKey(g);
                                else
                                    owner.DrawPanelOpaque(g);
                            }
                        }
                    }
                    catch { }
                    try { DesktopHud.ValidateRect(m.HWnd, IntPtr.Zero); } catch { }
                    m.Result = IntPtr.Zero;
                    return;
                }
                if (m.Msg == WM_ERASEBKGND)
                {
                    m.Result = new IntPtr(1);
                    return;
                }
                base.WndProc(ref m);
            }
        }

        private sealed class SettingsForm : Form
        {
            public event Action<bool> OverChanged;
            public event Action<bool> WindowsChanged;
            public event Action<bool> AutostartChanged;
            public event Action<bool> BinaryChanged;
            public event Action<int> RefreshIntervalChanged;
            public event Action<double> OpacityChanged;
            public event Action<double> TextOpacityChanged;
public event Action<int> MonitorChanged;
public event Action<bool> ShowVramUsageChanged;
public event Action<bool> ShowTitleChanged;
public event Action<bool> ShowOsLineChanged;
public event Action<bool> TextOutlineChanged;
public event Action<int> TextColorProfileChanged;
public event Action<int> TextSpacingChanged;
public event Action<int> AnchorChanged;
public event Action<int, int> CustomPosChanged;
public event Action<bool> CustomLockedChanged;
public event Action CustomCenterRequested;
public new event Action<int> FontChanged;
public event Action<bool> MirrorLayoutChanged;
public event Action<bool> MirrorArtChanged;
public event Action<int> SettingsThemeChanged;
public event Action<bool> FoxModeChanged;
public event Action<char> CodeLetterTyped;
public event Action<Color> PawColorChanged;
public event Action<double> PawOpacityChanged;

            private TextBox customXText;
            private TextBox customYText;
            private Button customLockBtn;
            private Label customHint;
            private ComboBox monitorComboRef;
            private bool suppressCustomEvents = false;
            // Nudge (hold-to-slide) state + cached Custom UI state for the
            // arrow-key path. Nudges live in this form because presses here
            // demonstrably work, independent of HUD-window mouse delivery.
            private System.Windows.Forms.Timer nudgeTimer;
            private int nudgeDX, nudgeDY;
            private bool customLockUi = true;
            private int customAnchorUi = 0;
            private Button nudgeL, nudgeR, nudgeU, nudgeD;
            private Label nudgeLabel, nudgeHintLabel;
            private CheckBox foxBox;
            private int lastCustomX = 0;
            private int lastCustomY = 0;
            // 0 = Follow Windows, 1 = Light, 2 = Dark. Rendered with the
            // native WinForms color mode, so dark matches Windows.
            private int themeMode = 0;

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);
                // Windows theme changed while open and we follow it: refresh
                // the immersive dark title bar and re-apply the color mode
                // + text softening (control colors refresh on next open at
                // the latest).
                if (m.Msg == 0x001A /* WM_SETTINGCHANGE */ && themeMode == 0)
                {
                    try
                    {
                        ApplyAppColorMode(themeMode);
                        TryEnableDarkTitleBar(Handle, IsWindowsDarkMode());
                        ApplySoftDarkText();
                        try { Invalidate(true); } catch { }
                    }
                    catch { }
                }
            }

            // Soften bright-white text in dark mode: the native dark style
            // renders labels, checkboxes and group titles near-white, which
            // strains eyes on the dark surface. Muted to a soft grey while
            // dark; restored to the system text color while light. Hint/link
            // labels keep their own muted colors in both modes.
            private void ApplySoftDarkText()
            {
                bool dark = themeMode == 2 || (themeMode == 0 && IsWindowsDarkMode());
                Color soft = Color.FromArgb(0xC6, 0xC6, 0xC6);
                SoftOne(this, dark, soft);
            }

            private void SoftOne(Control c, bool dark, Color soft)
            {
                try
                {
                    if (c is Label || c is CheckBox || c is GroupBox)
                    {
                        if (!(c.ForeColor == SystemColors.GrayText || c.ForeColor == SystemColors.HotTrack))
                            c.ForeColor = dark ? soft : SystemColors.ControlText;
                    }
                    foreach (Control k in c.Controls)
                        SoftOne(k, dark, soft);
                }
                catch { }
            }

            protected override void OnHandleCreated(EventArgs e)
            {
                base.OnHandleCreated(e);
                try
                {
                    bool dark = themeMode == 2 || (themeMode == 0 && IsWindowsDarkMode());
                    TryEnableDarkTitleBar(Handle, dark);
                }
                catch { }
            }

            // Called by the owner when "aethercore" is typed: reveals the
            // hidden toggle and switches it on.
            public void RevealFoxMode()
            {
                try
                {
                    if (IsDisposed || Disposing) return;
                    if (InvokeRequired) { try { BeginInvoke(new Action(RevealFoxMode)); } catch { } return; }
                    if (foxBox == null || foxBox.IsDisposed) return;
                    foxBox.Visible = true;
                    foxBox.Checked = true;
                }
                catch { }
            }

            public void UpdateCustomPos(int x, int y)
            {
                try
                {
                    if (customXText == null || customYText == null) return;
                    if (IsDisposed || Disposing) return;
                    // Plain Text assignment: no commit event fires, so no
                    // suppress flag or echo loop is possible here.
                    if (x != int.MinValue) { x = Math.Clamp(x, -10000, 10000); lastCustomX = x; customXText.Text = x.ToString(); }
                    if (y != int.MinValue) { y = Math.Clamp(y, -10000, 10000); lastCustomY = y; customYText.Text = y.ToString(); }
                }
                catch { }
            }

            public void SetCustomLock(bool locked)
            {
                try
                {
                    if (customLockBtn == null || customHint == null) return;
                    if (IsDisposed || Disposing) return;
                    customLockBtn.Text = locked ? "Unlock to drag" : "Lock";
                    customHint.Text = locked
                        ? "Locked: HUD is click-through."
                        : "Unlocked: drag the blue handle (or nudge below), then Lock.";
                    customLockUi = locked;
                    RefreshNudgeEnabled();
                }
                catch { }
            }

            private void RefreshNudgeEnabled()
            {
                try
                {
                    bool en = customAnchorUi == 4 && !customLockUi;
                    if (nudgeL != null) nudgeL.Enabled = en;
                    if (nudgeR != null) nudgeR.Enabled = en;
                    if (nudgeU != null) nudgeU.Enabled = en;
                    if (nudgeD != null) nudgeD.Enabled = en;
                    if (nudgeLabel != null) nudgeLabel.Enabled = en;
                    if (nudgeHintLabel != null) nudgeHintLabel.Enabled = en;
                }
                catch { }
            }

            private void NudgeStep()
            {
                try
                {
                    if (IsDisposed || Disposing) return;
                    if (customXText == null || customYText == null) return;
                    if (customAnchorUi != 4 || customLockUi) return;
                    int curX = 0, curY = 0;
                    try { int.TryParse(customXText.Text.Trim(), out curX); } catch { }
                    try { int.TryParse(customYText.Text.Trim(), out curY); } catch { }
                    int nx = Math.Clamp(curX + nudgeDX, -10000, 10000);
                    int ny = Math.Clamp(curY + nudgeDY, -10000, 10000);
                    lastCustomX = nx;
                    lastCustomY = ny;
                    customXText.Text = nx.ToString();
                    customYText.Text = ny.ToString();
                    CustomPosChanged?.Invoke(nx, ny);
                }
                catch { }
            }

            public void UpdateMonitorIndex(int idx)
            {
                try
                {
                    if (monitorComboRef == null || IsDisposed || Disposing) return;
                    suppressCustomEvents = true;
                    try { monitorComboRef.SelectedIndex = Math.Clamp(idx, 0, Math.Max(0, monitorComboRef.Items.Count - 1)); }
                    finally { suppressCustomEvents = false; }
                }
                catch { try { suppressCustomEvents = false; } catch { } }
            }

            public SettingsForm(bool displayOverShortcuts, bool displayOverWindows, bool autostart, bool binaryUnits, string gpuSource, int refreshInterval, double opacity, double textOpacity, int monitorIndex, int monitorCount, bool showVramUsage, bool showTextOutline, bool showTitle, bool showOsLine, int textColorProfile, int textSpacing, int anchor, int fontChoice, bool mirrorLayout, bool mirrorArt, int settingsTheme, int customX, int customY, bool customLocked, bool foxMode, bool foxUnlocked, Color pawColor, double pawOpacity)
            {
                Text = "DogTag Settings";
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                ShowInTaskbar = false;
                StartPosition = FormStartPosition.CenterScreen;
                AutoScaleMode = AutoScaleMode.Dpi;
                // Scrolling lives in the inner host panel (below) so the
                // footer with Close stays pinned on small screens.
                AutoScroll = false;
                ClientSize = new Size(480, 700);
                // Small screens (down to 640x480): cap the window to the
                // working area and let AutoScroll handle the rest instead
                // of overflowing off-screen.
                try
                {
                    Rectangle wa = Screen.PrimaryScreen.WorkingArea;
                    try { wa = Screen.FromPoint(Cursor.Position).WorkingArea; } catch { }
                    int maxH = Math.Max(300, wa.Height - 60);
                    int maxW = Math.Max(320, wa.Width - 30);
                    if (ClientSize.Height > maxH || ClientSize.Width > maxW)
                        ClientSize = new Size(Math.Min(ClientSize.Width, maxW), Math.Min(ClientSize.Height, maxH));
                }
                catch { }
                KeyPreview = true;
                var over = new CheckBox
                {
                    Text = "Display over shortcuts",
                    Checked = displayOverShortcuts,
                    AutoSize = true,
                    Location = new Point(14, 14)
                };
                var overWindows = new CheckBox
                {
                    Text = "Display over windows",
                    Checked = displayOverWindows,
                    AutoSize = true,
                    Location = new Point(14, 42)
                };
                var start = new CheckBox
                {
                    Text = "Start with Windows",
                    Checked = autostart,
                    AutoSize = true,
                    Location = new Point(14, 70)
                };
                var binary = new CheckBox
                {
                    Text = "Binary units for RAM, storage, and VRAM (GiB)",
                    Checked = binaryUnits,
                    AutoSize = true,
                    Location = new Point(14, 98)
                };
                var fontLabel = new Label
                {
                    Text = "Font:",
                    AutoSize = true,
                    Location = new Point(14, 170)
                };
                var fontCombo = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new Point(14, 190),
                    Size = new Size(200, 24)
                };
                fontCombo.Items.AddRange(FontOptions);
                fontCombo.SelectedIndex = Math.Clamp(fontChoice, 0, FontOptions.Length - 1);
                fontCombo.SelectedIndexChanged += (_, _) => { if (fontCombo.SelectedIndex >= 0) FontChanged?.Invoke(fontCombo.SelectedIndex); };
                var intervalLabel = new Label
                {
                    Text = "Refresh interval (ms):",
                    AutoSize = true,
                    Location = new Point(14, 258)
                };
                var intervalCombo = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Size = new Size(100, 24),
                    Location = new Point(14, 278)
                };
                for (int iv = 500; iv <= 10000; iv += 500)
                    intervalCombo.Items.Add(iv.ToString());
                intervalCombo.SelectedIndex = Math.Clamp((int)Math.Round((refreshInterval - 500) / 500.0, MidpointRounding.AwayFromZero), 0, intervalCombo.Items.Count - 1);
                var opacityLabel = new Label
                {
                    Text = "Background opacity",
                    AutoSize = true,
                    Location = new Point(14, 312)
                };
                var opacityTrack = new TrackBar
                {
                    Minimum = 0,
                    Maximum = 100,
                    Value = Math.Clamp((int)Math.Round(opacity * 100, MidpointRounding.AwayFromZero), 0, 100),
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Size = new Size(260, 34),
                    Location = new Point(14, 332)
                };
                var opacityValue = new Label
                {
                    Text = $"{(int)Math.Round(opacity * 100, MidpointRounding.AwayFromZero)}%",
                    Location = new Point(284, 338),
                    AutoSize = true
                };
                var gpuLabel = new Label
                {
                    Text = "GPU detection: " + (string.IsNullOrWhiteSpace(gpuSource) ? "not yet detected" : gpuSource),
                    AutoSize = true,
                    Location = new Point(14, 658),
                    ForeColor = SystemColors.GrayText
                };
                opacityTrack.ValueChanged += (_, _) =>
                {
                    opacityValue.Text = opacityTrack.Value + "%";
                    OpacityChanged?.Invoke(opacityTrack.Value / 100.0);
                };
                intervalCombo.SelectedIndexChanged += (_, _) => { if (!suppressCustomEvents && intervalCombo.SelectedIndex >= 0) RefreshIntervalChanged?.Invoke(500 * (intervalCombo.SelectedIndex + 1)); };
                // Display-over-windows is topmost over everything, so it forces
                // over-shortcuts on and blocks turning it off while active.
                over.CheckedChanged += (_, _) =>
                {
                    if (!over.Checked && overWindows.Checked) { over.Checked = true; return; }
                    OverChanged?.Invoke(over.Checked);
                };
                overWindows.CheckedChanged += (_, _) =>
                {
                    if (overWindows.Checked && !over.Checked) over.Checked = true;
                    WindowsChanged?.Invoke(overWindows.Checked);
                };
                start.CheckedChanged += (_, _) => AutostartChanged?.Invoke(start.Checked);
                binary.CheckedChanged += (_, _) => BinaryChanged?.Invoke(binary.Checked);
                var close = new Button
                {
                    Text = "Close",
                    DialogResult = DialogResult.OK,
                    Location = new Point(336, 694),
                    Size = new Size(86, 28)
                };
                close.Click += (_, _) => Close();
                var credits = new Label
                {
                    Text = "Made by Mikakod",
                    AutoSize = true,
                    Location = new Point(14, 698),
                    ForeColor = SystemColors.GrayText,
                    Font = new Font("Consolas", 8f, FontStyle.Regular)
                };
                var creditUrl = new Label
                {
                    Text = "https://github.com/mikakod",
                    AutoSize = true,
                    Location = new Point(14, 680),
                    ForeColor = SystemColors.HotTrack,
                    Font = new Font("Consolas", 8f, FontStyle.Underline)
                };
                creditUrl.Cursor = Cursors.Hand;
                creditUrl.Click += (_, _) =>
                {
                    try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://github.com/mikakod") { UseShellExecute = true }); } catch { }
                };
                // (Assembled into layout groups at the end of the ctor.)

                var textOpacityLabel = new Label
                {
                    Text = "Text opacity",
                    AutoSize = true,
                    Location = new Point(14, 378)
                };
                var textOpacityTrack = new TrackBar
                {
                    Minimum = 20,
                    Maximum = 100,
                    Value = Math.Clamp((int)Math.Round(textOpacity * 100, MidpointRounding.AwayFromZero), 20, 100),
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Size = new Size(260, 34),
                    Location = new Point(14, 398)
                };
                var textOpacityValue = new Label
                {
                    Text = $"{(int)Math.Round(textOpacity * 100, MidpointRounding.AwayFromZero)}%",
                    Location = new Point(284, 404),
                    AutoSize = true
                };
                textOpacityTrack.ValueChanged += (_, _) =>
                {
                    textOpacityValue.Text = textOpacityTrack.Value + "%";
                    TextOpacityChanged?.Invoke(textOpacityTrack.Value / 100.0);
                };

                var artColorLabel = new Label
                {
                    Text = "Art color:",
                    AutoSize = true,
                    Location = new Point(14, 424)
                };
                var artSwatch = new Panel
                {
                    Size = new Size(44, 23),
                    Location = new Point(100, 422),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.FromArgb(255, pawColor),
                    Tag = "artswatch"
                };
                var artPickBtn = new Button
                {
                    Text = "Pick…",
                    Size = new Size(70, 26),
                    Location = new Point(150, 420)
                };
                artPickBtn.Click += (_, _) =>
                {
                    try
                    {
                        using (var dlg = new ColorDialog
                        {
                            FullOpen = true,
                            AnyColor = true,
                            SolidColorOnly = false,
                            Color = Color.FromArgb(artSwatch.BackColor.R, artSwatch.BackColor.G, artSwatch.BackColor.B)
                        })
                        {
                            if (dlg.ShowDialog(this) == DialogResult.OK)
                            {
                                artSwatch.BackColor = Color.FromArgb(dlg.Color.R, dlg.Color.G, dlg.Color.B);
                                PawColorChanged?.Invoke(artSwatch.BackColor);
                            }
                        }
                    }
                    catch { }
                };
                var artOpacityLabel = new Label
                {
                    Text = "Art opacity",
                    AutoSize = true,
                    Location = new Point(14, 450)
                };
                var artOpacityTrack = new TrackBar
                {
                    Minimum = 0,
                    Maximum = 100,
                    Value = Math.Clamp((int)Math.Round(pawOpacity * 100, MidpointRounding.AwayFromZero), 0, 100),
                    TickFrequency = 5,
                    LargeChange = 5,
                    SmallChange = 1,
                    Size = new Size(260, 34),
                    Location = new Point(14, 470)
                };
                var artOpacityValue = new Label
                {
                    Text = $"{(int)Math.Round(pawOpacity * 100, MidpointRounding.AwayFromZero)}%",
                    Location = new Point(284, 476),
                    AutoSize = true
                };
                artOpacityTrack.ValueChanged += (_, _) =>
                {
                    artOpacityValue.Text = artOpacityTrack.Value + "%";
                    PawOpacityChanged?.Invoke(artOpacityTrack.Value / 100.0);
                };

                // (Assembled into layout groups at the end of the ctor.)

                var anchorLabel = new Label
                {
                    Text = "Anchor:",
                    AutoSize = true,
                    Location = new Point(14, 444)
                };
                var anchorCombo = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new Point(100, 440),
                    Size = new Size(160, 24)
                };
                anchorCombo.Items.AddRange(new object[] { "Bottom-right", "Bottom-left", "Top-right", "Top-left", "Custom" });
                anchorCombo.SelectedIndex = Math.Clamp(anchor, 0, 4);
                anchorCombo.SelectedIndexChanged += (_, _) => { if (anchorCombo.SelectedIndex >= 0) AnchorChanged?.Invoke(anchorCombo.SelectedIndex); };
                var monitorLabel = new Label
                {
                    Text = "Monitor:",
                    AutoSize = true,
                    Location = new Point(14, 476)
                };
                var monitorCombo = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Size = new Size(70, 24),
                    Location = new Point(100, 472)
                };
                for (int m = 1; m <= Math.Max(1, monitorCount); m++)
                    monitorCombo.Items.Add(m.ToString());
                monitorCombo.SelectedIndex = Math.Clamp(monitorIndex, 0, monitorCombo.Items.Count - 1);
                monitorCombo.SelectedIndexChanged += (_, _) => { if (!suppressCustomEvents && monitorCombo.SelectedIndex >= 0) MonitorChanged?.Invoke(monitorCombo.SelectedIndex); };
                monitorComboRef = monitorCombo;

                // Custom anchor: free position + lock. Enabled only when
                // Anchor == Custom (index 4); otherwise greyed out.
                var customXLabel = new Label
                {
                    Text = "X:",
                    AutoSize = true,
                    Location = new Point(14, 532)
                };
                customXText = new TextBox
                {
                    Text = (customX == int.MinValue ? 0 : Math.Clamp(customX, -10000, 10000)).ToString(),
                    Size = new Size(70, 24),
                    Location = new Point(34, 528)
                };
                lastCustomX = customX == int.MinValue ? 0 : Math.Clamp(customX, -10000, 10000);
                var customYLabel = new Label
                {
                    Text = "Y:",
                    AutoSize = true,
                    Location = new Point(112, 532)
                };
                customYText = new TextBox
                {
                    Text = (customY == int.MinValue ? 0 : Math.Clamp(customY, -10000, 10000)).ToString(),
                    Size = new Size(70, 24),
                    Location = new Point(132, 528)
                };
                lastCustomY = customY == int.MinValue ? 0 : Math.Clamp(customY, -10000, 10000);
                customLockBtn = new Button
                {
                    Text = customLocked ? "Unlock to drag" : "Lock",
                    Size = new Size(110, 28),
                    Location = new Point(214, 526)
                };
                var customSnapBtn = new Button
                {
                    Text = "Center",
                    Size = new Size(70, 28),
                    Location = new Point(330, 526)
                };
                customHint = new Label
                {
                    Text = customLocked ? "Locked: HUD is click-through." : "Unlocked: drag the blue handle (or nudge below), then Lock.",
                    AutoSize = true,
                    Location = new Point(14, 556),
                    ForeColor = SystemColors.GrayText
                };
                // Manual entry commits on Enter or focus-leave (same persist
                // path as drag). Invalid text reverts to the last good
                // position instead of moving the HUD. While editing, Enter
                // must not trigger the dialog Close button.
                void CommitCustomXY()
                {
                    try
                    {
                        int nx = lastCustomX, ny = lastCustomY;
                        bool okX = customXText != null && int.TryParse(customXText.Text.Trim(), out nx);
                        bool okY = customYText != null && int.TryParse(customYText.Text.Trim(), out ny);
                        if (!okX || !okY)
                        {
                            if (customXText != null) customXText.Text = lastCustomX.ToString();
                            if (customYText != null) customYText.Text = lastCustomY.ToString();
                            return;
                        }
                        nx = Math.Clamp(nx, -10000, 10000);
                        ny = Math.Clamp(ny, -10000, 10000);
                        lastCustomX = nx;
                        lastCustomY = ny;
                        if (customXText != null) customXText.Text = nx.ToString();
                        if (customYText != null) customYText.Text = ny.ToString();
                        CustomPosChanged?.Invoke(nx, ny);
                    }
                    catch { }
                }
                customXText.Leave += (_, _) => { try { AcceptButton = close; } catch { } CommitCustomXY(); };
                customYText.Leave += (_, _) => { try { AcceptButton = close; } catch { } CommitCustomXY(); };
                customXText.Enter += (_, _) => { try { AcceptButton = null; } catch { } };
                customYText.Enter += (_, _) => { try { AcceptButton = null; } catch { } };
                customXText.KeyDown += (_, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        CommitCustomXY();
                    }
                };
                customYText.KeyDown += (_, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        CommitCustomXY();
                    }
                };
                bool uiLocked = customLocked;
                customLockUi = customLocked;
                customAnchorUi = Math.Clamp(anchor, 0, 4);
                customLockBtn.Click += (_, _) =>
                {
                    uiLocked = !uiLocked;
                    customLockBtn.Text = uiLocked ? "Unlock to drag" : "Lock";
                    customHint.Text = uiLocked ? "Locked: HUD is click-through." : "Unlocked: drag the blue handle (or nudge below), then Lock.";
                    customLockUi = uiLocked;
                    RefreshNudgeEnabled();
                    CustomLockedChanged?.Invoke(uiLocked);
                };
                customSnapBtn.Click += (_, _) =>
                {
                    // Owner centers using the real measured HUD extents, so the
                    // HUD's center (not its top-left corner) lands mid-screen.
                    try { CustomCenterRequested?.Invoke(); } catch { }
                };
                void RefreshCustomEnabled()
                {
                    bool isCustom = anchorCombo.SelectedIndex == 4;
                    customAnchorUi = anchorCombo.SelectedIndex;
                    customXText.Enabled = isCustom;
                    customYText.Enabled = isCustom;
                    customLockBtn.Enabled = isCustom;
                    customSnapBtn.Enabled = isCustom;
                    customHint.Enabled = isCustom;
                    customXLabel.Enabled = isCustom;
                    customYLabel.Enabled = isCustom;
                    RefreshNudgeEnabled();
                }
                anchorCombo.SelectedIndexChanged += (_, _) => RefreshCustomEnabled();
                RefreshCustomEnabled();

                // Nudge pad: hold a direction to slide the HUD. Same persist
                // path as drag (CustomPosChanged); arrows on the keyboard do
                // the same when Custom is selected and unlocked.
                nudgeLabel = new Label
                {
                    Text = "Nudge:",
                    AutoSize = true,
                    Location = new Point(14, 584)
                };
                nudgeL = new Button { Text = "◀", Size = new Size(44, 26), Location = new Point(70, 578) };
                nudgeU = new Button { Text = "▲", Size = new Size(44, 26), Location = new Point(120, 578) };
                nudgeD = new Button { Text = "▼", Size = new Size(44, 26), Location = new Point(170, 578) };
                nudgeR = new Button { Text = "▶", Size = new Size(44, 26), Location = new Point(220, 578) };
                nudgeHintLabel = new Label
                {
                    Text = "hold to slide, arrows work too",
                    AutoSize = true,
                    Location = new Point(270, 584),
                    ForeColor = SystemColors.GrayText
                };
                void ArmNudge(Button b, int dx, int dy)
                {
                    b.MouseDown += (_, e) =>
                    {
                        if (e.Button != MouseButtons.Left) return;
                        nudgeDX = dx; nudgeDY = dy;
                        NudgeStep();
                        try { if (nudgeTimer != null) { nudgeTimer.Stop(); nudgeTimer.Dispose(); } } catch { }
                        nudgeTimer = new System.Windows.Forms.Timer { Interval = 70 };
                        nudgeTimer.Tick += (_, _) => NudgeStep();
                        nudgeTimer.Start();
                    };
                    b.MouseUp += (_, _) => { try { if (nudgeTimer != null) nudgeTimer.Stop(); } catch { } };
                }
                ArmNudge(nudgeL, -10, 0);
                ArmNudge(nudgeR, 10, 0);
                ArmNudge(nudgeU, 0, -10);
                ArmNudge(nudgeD, 0, 10);
                KeyDown += (_, e) =>
                {
                    try
                    {
                        if (customAnchorUi != 4 || customLockUi) return;
                        // Native spinner arrows keep their own behaviour.
                        try
                        {
                            if (ActiveControl == customXText || ActiveControl == customYText) return;
                        }
                        catch { }
                        int dx = 0, dy = 0;
                        if (e.KeyCode == Keys.Left) dx = -10;
                        else if (e.KeyCode == Keys.Right) dx = 10;
                        else if (e.KeyCode == Keys.Up) dy = -10;
                        else if (e.KeyCode == Keys.Down) dy = 10;
                        else return;
                        nudgeDX = dx; nudgeDY = dy;
                        NudgeStep();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    catch { }
                };
                FormClosed += (_, _) => { try { if (nudgeTimer != null) { nudgeTimer.Stop(); nudgeTimer.Dispose(); nudgeTimer = null; } } catch { } };
                RefreshNudgeEnabled();

                // Easter-egg entry: plain letter keys would otherwise jump
                // the DropDownList combos (e.g. 'c' -> Custom anchor, 't' ->
                // Terminal profile) while typing the unlock code. Swallow
                // them here and forward them to the owner for the code check
                // ('\0' = sequence-breaking key). Arrows, digits and spinner
                // behaviour are unaffected.
                KeyDown += (_, e) =>
                {
                    try
                    {
                        if (e.Control || e.Alt) return;
                        if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z)
                        {
                            e.Handled = true;
                            e.SuppressKeyPress = true;
                            try { CodeLetterTyped?.Invoke((char)('a' + (e.KeyCode - Keys.A))); } catch { }
                            return;
                        }
                        switch (e.KeyCode)
                        {
                            case Keys.ShiftKey:
                            case Keys.LShiftKey:
                            case Keys.RShiftKey:
                            case Keys.ControlKey:
                            case Keys.LControlKey:
                            case Keys.RControlKey:
                            case Keys.Menu:
                            case Keys.LMenu:
                            case Keys.RMenu:
                            case Keys.CapsLock:
                            case Keys.LWin:
                            case Keys.RWin:
                                break;
                            default:
                                try { CodeLetterTyped?.Invoke('\0'); } catch { }
                                break;
                        }
                    }
                    catch { }
                };

                var vramUsage = new CheckBox
                {
                    Text = "Show VRAM usage",
                    Checked = showVramUsage,
                    AutoSize = true,
                    Location = new Point(172, 476)
                };
                vramUsage.CheckedChanged += (_, _) => ShowVramUsageChanged?.Invoke(vramUsage.Checked);

                var outlineBox = new CheckBox
                {
                    Text = "Text outline",
                    Checked = showTextOutline,
                    AutoSize = true,
                    Location = new Point(14, 220)
                };
                outlineBox.CheckedChanged += (_, _) => TextOutlineChanged?.Invoke(outlineBox.Checked);

                var showTitleBox = new CheckBox
                {
                    Text = "Show title",
                    Checked = showTitle,
                    AutoSize = true,
                    Location = new Point(14, 244)
                };
                showTitleBox.CheckedChanged += (_, _) => ShowTitleChanged?.Invoke(showTitleBox.Checked);

                var showOsBox = new CheckBox
                {
                    Text = "Show OS line",
                    Checked = showOsLine,
                    AutoSize = true,
                    Location = new Point(170, 244)
                };
                showOsBox.CheckedChanged += (_, _) => ShowOsLineChanged?.Invoke(showOsBox.Checked);

                var profileLabel = new Label
                {
                    Text = "Font Color Profile:",
                    AutoSize = true,
                    Location = new Point(14, 120)
                };
                var profileCombo = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new Point(14, 140),
                    Size = new Size(200, 24)
                };
                // Display order puts the default (Solarized Dark) first.
                // Combo index != profile id: ComboProfileId maps between them
                // so stored TextColorProfile values keep their meaning.
                int[] comboToProfile = { 10, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 11 };
                profileCombo.Items.AddRange(new object[] { "Solarized Dark (Default)", "Legacy", "Legacy Old", "Slate", "Dracula", "Monochrome", "Synthwave", "Mocha", "Gruvbox Dark", "Tokyo Night", "Terminal", "Carbon" });
                profileCombo.SelectedIndex = Array.IndexOf(comboToProfile, Math.Clamp(textColorProfile, 0, MaxTextColorProfile));
                profileCombo.SelectedIndexChanged += (_, _) => { if (profileCombo.SelectedIndex >= 0) TextColorProfileChanged?.Invoke(comboToProfile[profileCombo.SelectedIndex]); };

                var spacingLabel = new Label
                {
                    Text = "Text spacing:",
                    AutoSize = true,
                    Location = new Point(14, 632)
                };
                var spacingCombo = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new Point(100, 628),
                    Size = new Size(200, 24)
                };
                spacingCombo.Items.AddRange(new object[] { "Tight", "Aligned columns" });
                spacingCombo.SelectedIndex = Math.Clamp(textSpacing, 0, 1);
                spacingCombo.SelectedIndexChanged += (_, _) => { if (spacingCombo.SelectedIndex >= 0) TextSpacingChanged?.Invoke(spacingCombo.SelectedIndex); };

                themeMode = Math.Clamp(settingsTheme, 0, 2);
                var themeLabel = new Label
                {
                    Text = "Theme:",
                    AutoSize = true,
                    Location = new Point(14, 632)
                };
                var themeCombo = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new Point(100, 628),
                    Size = new Size(160, 24)
                };
                themeCombo.Items.AddRange(new object[] { "Follow Windows", "Light", "Dark" });
                themeCombo.SelectedIndex = themeMode;
                themeCombo.SelectedIndexChanged += (_, _) =>
                {
                    if (themeCombo.SelectedIndex < 0) return;
                    themeMode = themeCombo.SelectedIndex;
                    // The owner re-creates this dialog under the new color
                    // mode (live-switching leaves stale-themed parts such as
                    // dropdown lists behind), so just report the change.
                    SettingsThemeChanged?.Invoke(themeMode);
                };

                var mirrorBox = new CheckBox
                {
                    Text = "Mirror layout",
                    Checked = mirrorLayout,
                    AutoSize = true,
                    Location = new Point(14, 608)
                };
                mirrorBox.CheckedChanged += (_, _) => MirrorLayoutChanged?.Invoke(mirrorBox.Checked);

                var mirrorArtBox = new CheckBox
                {
                    Text = "Mirror Art/Logo",
                    Checked = mirrorArt,
                    AutoSize = true,
                    Location = new Point(170, 608)
                };
                mirrorArtBox.CheckedChanged += (_, _) => MirrorArtChanged?.Invoke(mirrorArtBox.Checked);

                // Fox Mode easter egg: hidden until "aethercore" is typed.
                foxBox = new CheckBox
                {
                    Text = "Fox Mode",
                    Checked = foxMode,
                    AutoSize = true,
                    Location = new Point(170, 632),
                    Visible = foxUnlocked
                };
                foxBox.CheckedChanged += (_, _) => FoxModeChanged?.Invoke(foxBox.Checked);

                // Native WinForms color modes paint combos (including the
                // drop-down buttons) in the Windows dark style, so no
                // owner-draw workaround is needed anymore.

                // ---- Flow layout assembly (resolution/DPI-proof) ----
                // Absolute Locations above are ignored once parented into
                // flow containers: rows size to content, so nothing can
                // overlap at any DPI, and the form scrolls on tiny screens.
                FlowLayoutPanel HRow(params Control[] cs)
                {
                    var f = new FlowLayoutPanel
                    {
                        FlowDirection = FlowDirection.LeftToRight,
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink,
                        WrapContents = true,
                    };
                    f.SuspendLayout();
                    foreach (var c in cs) f.Controls.Add(c);
                    f.ResumeLayout(false);
                    return f;
                }
                FlowLayoutPanel VStack(params Control[] cs)
                {
                    var f = new FlowLayoutPanel
                    {
                        FlowDirection = FlowDirection.TopDown,
                        Dock = DockStyle.Fill,
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink,
                        WrapContents = false,
                    };
                    f.SuspendLayout();
                    foreach (var c in cs) f.Controls.Add(c);
                    f.ResumeLayout(false);
                    return f;
                }
                GroupBox Section(string title, Control content)
                {
                    var g = new GroupBox
                    {
                        Text = title,
                        Dock = DockStyle.Fill,
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    };
                    content.Dock = DockStyle.Fill;
                    g.Controls.Add(content);
                    return g;
                }

                var displaySection = Section("Display", VStack(
                    over,
                    overWindows,
                    binary,
                    HRow(monitorLabel, monitorCombo, vramUsage),
                    HRow(anchorLabel, anchorCombo),
                    HRow(customXLabel, customXText, customYLabel, customYText, customLockBtn, customSnapBtn),
                    customHint,
                    HRow(nudgeLabel, nudgeL, nudgeU, nudgeD, nudgeR, nudgeHintLabel),
                    HRow(mirrorBox, mirrorArtBox)));

                var textSection = Section("Text", VStack(
                    HRow(profileLabel, profileCombo),
                    HRow(fontLabel, fontCombo),
                    HRow(spacingLabel, spacingCombo),
                    outlineBox,
                    HRow(showTitleBox, showOsBox),
                    HRow(opacityLabel, opacityTrack, opacityValue),
                    HRow(textOpacityLabel, textOpacityTrack, textOpacityValue),
                    HRow(artColorLabel, artSwatch, artPickBtn),
                    HRow(artOpacityLabel, artOpacityTrack, artOpacityValue)));

                var systemSection = Section("System", VStack(
                    start,
                    HRow(intervalLabel, intervalCombo),
                    HRow(themeLabel, themeCombo),
                    foxBox,
                    gpuLabel));

                var sections = new TableLayoutPanel
                {
                    ColumnCount = 1,
                    RowCount = 3,
                    Dock = DockStyle.Top,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Padding = new Padding(6, 2, 6, 2),
                };
                sections.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
                sections.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                sections.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                sections.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                sections.SuspendLayout();
                sections.Controls.Add(displaySection, 0, 0);
                sections.Controls.Add(textSection, 0, 1);
                sections.Controls.Add(systemSection, 0, 2);
                sections.ResumeLayout(false);

                // Fixed footer: Close + credits always visible, even when the
                // content scrolls on small screens. No Anchor: the dialog is
                // fixed-size, and anchoring against the pre-dock footer width
                // would preserve a negative overhang (Close sliding away).
                var footer = new Panel { Dock = DockStyle.Bottom, Height = 52, Tag = "footer" };
                close.Location = new Point(ClientSize.Width - close.Width - 14, 12);
                creditUrl.Location = new Point(12, 4);
                credits.Location = new Point(12, 24);
                footer.Controls.Add(close);
                footer.Controls.Add(creditUrl);
                footer.Controls.Add(credits);

                // Scroll host: sections scroll inside it; the footer above is a
                // sibling, so Close never scrolls out of view. (Docking the
                // footer on a scrolling form would park it at the bottom of
                // the scroll extent instead.)
                var scrollHost = new Panel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                    Padding = new Padding(0),
                    Margin = new Padding(0),
                };
                scrollHost.SuspendLayout();
                scrollHost.Controls.Add(sections);
                scrollHost.ResumeLayout(false);

                SuspendLayout();
                Controls.Add(scrollHost);
                Controls.Add(footer);
                ResumeLayout(true);
                AcceptButton = close;
                // Native color mode (set by the owner before construction)
                // paints the form, title bar, combos, sliders and buttons in
                // the Windows light/dark style. No manual recoloring: the old
                // hand-rolled dark palette is what left white title bars,
                // combo buttons and slider tracks. Bright-white text is only
                // muted to a soft grey (see ApplySoftDarkText).
                ApplySoftDarkText();
            }
        }



        private static void RenderRawULW(IntPtr hwnd, int x, int y, int w, int h)
        {
            using (var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb))
            {
                using (var g = Graphics.FromImage(bmp)) g.Clear(Color.FromArgb(255, 0, 255, 0));
                var data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                int stride = Math.Abs(data.Stride);
                var raw = new byte[stride * h];
                Marshal.Copy(data.Scan0, raw, 0, raw.Length);
                bmp.UnlockBits(data);
                var packed = new byte[w * h * 4];
                for (int row = 0; row < h; row++) Buffer.BlockCopy(raw, row * stride, packed, row * w * 4, w * 4);
                IntPtr screen = GetDC(IntPtr.Zero);
                IntPtr mem = CreateCompatibleDC(screen);
                var bmi = new BitmapInfo();
                bmi.Header.biSize = 40;
                bmi.Header.biWidth = w;
                bmi.Header.biHeight = -h;
                bmi.Header.biPlanes = 1;
                bmi.Header.biBitCount = 32;
                IntPtr bits;
                IntPtr dib = CreateDIBSection(screen, ref bmi, 0, out bits, IntPtr.Zero, 0);
                Marshal.Copy(packed, 0, bits, packed.Length);
                IntPtr old = SelectObject(mem, dib);
                var dst = new WinPoint { X = x, Y = y };
                var src = new WinPoint();
                var size = new WinSize { cx = w, cy = h };
                var blend = new BlendFunction { BlendOp = 0, BlendFlags = 0, SourceConstantAlpha = 255, AlphaFormat = 1 };
                bool ok = UpdateLayeredWindow(hwnd, screen, ref dst, ref size, mem, ref src, 0, ref blend, ULW_ALPHA);
                SelectObject(mem, old);
                DeleteObject(dib);
                DeleteDC(mem);
                ReleaseDC(IntPtr.Zero, screen);
            }
        }

        public static void Main()
        {
            System.Threading.Mutex mutex = null;
            bool created = false;
            try
            {
                try { mutex = new System.Threading.Mutex(true, "DogTagSingleInstance", out created); }
                catch (System.Threading.AbandonedMutexException)
                {
                    created = true;
                    try { mutex = System.Threading.Mutex.OpenExisting("DogTagSingleInstance"); } catch { mutex = null; }
                }
                if (!created) return;
                Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                // Follow the Windows theme by default; the Settings window
                // re-applies the saved override (Light/Dark) before it opens.
                try { Application.SetColorMode(SystemColorMode.System); } catch { }
                Application.Run(new DesktopHud());
            }
            finally { try { if (created && mutex != null) mutex.ReleaseMutex(); } catch { } }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FILETIME
        {
            public uint dwLowDateTime, dwHighDateTime;
            public ulong ToUInt64() => ((ulong)dwHighDateTime << 32) | dwLowDateTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORYSTATUSEX
        {
            public uint dwLength, dwMemoryLoad;
            public ulong ullTotalPhys, ullAvailPhys, ullTotalPageFile, ullAvailPageFile, ullTotalVirtual, ullAvailVirtual, ullAvailExtendedVirtual;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        [DllImport("kernel32.dll")]
        private static extern bool GetSystemTimes(out FILETIME lpIdleTime, out FILETIME lpKernelTime, out FILETIME lpUserTime);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [StructLayout(LayoutKind.Sequential)] private struct RECT { public int Left, Top, Right, Bottom; }

        [StructLayout(LayoutKind.Sequential)]
        private struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public RECT rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] rgbReserved;
        }
        private static readonly IntPtr HWND_TOP = IntPtr.Zero;
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        [DllImport("user32.dll")] private static extern bool IsWindow(IntPtr hwnd);
        [DllImport("user32.dll")] private static extern IntPtr GetParent(IntPtr hwnd);
        [DllImport("user32.dll")] private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")] private static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, ref WinPoint lpPoints, uint cPoints);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint dwFreeType);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref WinPoint lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref WinPoint lpBuffer, int nSize, out IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool CloseHandle(IntPtr hObject);
        [DllImport("user32.dll")] private static extern bool InvalidateRect(IntPtr hWnd, ref RECT lpRect, bool bErase);
        [DllImport("user32.dll")] private static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);
        [DllImport("user32.dll")] private static extern bool ValidateRect(IntPtr hWnd, IntPtr lpRect);
     [DllImport("user32.dll")] private static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);
     [DllImport("user32.dll")] private static extern bool EndPaint(IntPtr hwnd, ref PAINTSTRUCT lpPaint);
        [DllImport("user32.dll")] private static extern bool ScreenToClient(IntPtr hWnd, ref WinPoint lpPoint);
        [DllImport("user32.dll")] private static extern bool SetWindowBand(IntPtr hWnd, IntPtr hwndInsertAfter, uint dwBand);
        private const uint GW_CHILD = 5;
        private const uint GW_HWNDPREV = 3;
        private const uint GW_HWNDNEXT = 2;
        [DllImport("user32.dll", CharSet = CharSet.Auto)] private static extern int RegisterWindowMessage(string lpString);
        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        private const int GWL_EXSTYLE = -20;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const uint SWP_FRAMECHANGED = 0x0020;
        private const int ULW_ALPHA = 2;
        private const byte AC_SRC_OVER = 0;
        private const byte AC_SRC_ALPHA = 1;
        private const uint LWA_ALPHA = 2;

        [StructLayout(LayoutKind.Sequential)]
        private struct WinPoint { public int X, Y; }

        [StructLayout(LayoutKind.Sequential)]
        private struct CursorPoint { public int X, Y; }

        [StructLayout(LayoutKind.Sequential)]
        private struct WinSize { public int cx, cy; }

        [StructLayout(LayoutKind.Sequential)]
        private struct BlendFunction
        {
            public byte BlendOp, BlendFlags, SourceConstantAlpha, AlphaFormat;
        }

        
        [DllImport("user32.dll")] private static extern int GetWindowLong(IntPtr hwnd, int nIndex);
        [DllImport("user32.dll")] private static extern int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")] private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        [DllImport("user32.dll")] private static extern bool GetLayeredWindowAttributes(IntPtr hwnd, out uint crKey, out byte bAlpha, out uint dwFlags);
[DllImport("user32.dll", SetLastError=true)]
        private static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref WinPoint pptDst, ref WinSize psize, IntPtr hdcSrc, ref WinPoint pptSrc, int crKey, ref BlendFunction pblend, int dwFlags);


        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr hwnd);
    [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
     [DllImport("user32.dll")] private static extern bool GetClientRect(IntPtr hwnd, out RECT lpRect);
    [DllImport("user32.dll")] private static extern bool GetCursorPos(out CursorPoint lpPoint);
    [DllImport("user32.dll")] private static extern IntPtr SetCapture(IntPtr hwnd);
    [DllImport("user32.dll")] private static extern bool ReleaseCapture();
    [DllImport("user32.dll")] private static extern IntPtr GetCapture();
    [DllImport("user32.dll")] private static extern short GetAsyncKeyState(int vKey);
    private const int VK_LBUTTON_G = 0x01;
    [DllImport("user32.dll")] private static extern int GetClassName(IntPtr hwnd, System.Text.StringBuilder lpClassName, int nMaxCount);
[DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr CreateWindowExW(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
        [DllImport("user32.dll")] private static extern bool DestroyWindow(IntPtr hwnd);
        [DllImport("kernel32.dll")] private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool BitBlt(IntPtr hdcDest, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, uint rop);
        private const uint SRCCOPY = 0x00CC0020;

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateDIBSection(IntPtr hdc, ref BitmapInfo pbmi, uint usage, out IntPtr bits, IntPtr hSection, uint offset);

        [StructLayout(LayoutKind.Sequential)]
        private struct BitmapInfoHeader
        {
            public uint biSize;
            public int biWidth, biHeight;
            public ushort biPlanes, biBitCount;
            public uint biCompression, biSizeImage;
            public int biXPelsPerMeter, biYPelsPerMeter;
            public uint biClrUsed, biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BitmapInfo
        {
            public BitmapInfoHeader Header;
        }

        private delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);
    }
}
