using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sol.ShieldOfFaith
{
    public static class Program
    {
        static public bool MainWindowStartup;

        static public string ShaderGlassExecutable
        {
            get
            {
                var p = Program.Settings.ShaderGlassPath;
                if (string.IsNullOrWhiteSpace(p))
                    return null;
                // can only be relative to program folder (presumed higher security than user folders),
                // otherwise low-security folders can be used to unexpectedly swap in executable
                return Path.GetFullPath(Path.Combine(GetExecutableContainingFolder(), p));
            }
        }

        const string DefaultAppDataSubfolder =
#if DEBUG
            @"Sol\Shield Of Faith\DEBUG-BUILD";
#else
            @"Sol\Shield Of Faith";
#endif

        static public string DefaultAppDataLocation
        {
            get
            {
                var s = Environment.GetEnvironmentVariable("AppData");
                if (string.IsNullOrEmpty(s))
                    return null;
                return Path.Combine(s, DefaultAppDataSubfolder);
            }
        }

        static string EmergencyCloseTracking;

        static public IEnumerable<KeyValuePair<string, FileAttributes>> GetAvailableFilesFromTree(string root)
        {
            string[] fs;
            try
            {
                fs = Directory.GetFileSystemEntries(root);
            }
            catch (UnauthorizedAccessException) { yield break; }
            catch (IOException) { yield break; }

            foreach (var e in fs)
                if (TryGetFileAttributes(e, out var attr))
                    switch (attr & (FileAttributes.Directory | FileAttributes.Hidden | FileAttributes.System))
                    {
                        case FileAttributes.Directory:
                            foreach (var c in GetAvailableFilesFromTree(e))
                                yield return c;
                            break;
                        case 0:
                            yield return new KeyValuePair<string, FileAttributes>(e, attr);
                            break;
                    }
        }

        static public bool TryGetFileAttributes(string path, out FileAttributes attr)
        {
            try { attr = File.GetAttributes(path); return true; }
            catch (UnauthorizedAccessException) { }
            catch (IOException) { }
            attr = 0;
            return false;
        }

        static public bool FileSystemEntryAvailable(string path)
        {
            try { File.GetAttributes(path); return true; }
            catch (UnauthorizedAccessException) { }
            catch (IOException) { }
            return false;
        }

        static public string FindPathTo(string file, string preferred_relative = null, bool include_directories = false)
        {
            var exists = include_directories ? (Func<string, bool>)FileSystemEntryAvailable : File.Exists;
            if (string.IsNullOrEmpty(file))
                return null;
            string p;
            if (exists(file))
                p = file;
            else if ((!string.IsNullOrEmpty(preferred_relative)) && exists(p = Path.Combine(preferred_relative, file)))
            { }
            else if (!(
                exists(p = Path.Combine(AppDataLocation, file)) ||
                exists(p = Path.Combine(DefaultAppDataLocation, file)) ||
                exists(p = Path.Combine(GetExecutableContainingFolder(), file))
                ))
                return null;
            return Path.GetFullPath(p);
        }

        static public System.IO.Compression.ZipArchive
            EmbedCatalog = new System.IO.Compression.ZipArchive(Assembly.GetExecutingAssembly().GetManifestResourceStream("Sol.ShieldOfFaith.Files.Catalog.zip")
                , System.IO.Compression.ZipArchiveMode.Read);

        static public ZipArchiveEntry GetEmbed(string path)
            => EmbedCatalog.GetEntry(path);

        static public IEnumerable<Tuple<string, ZipArchiveEntry>> GetEmbedsIn(string path, bool includeSublevels = false, bool includeFolders = false, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            string pathb;
            if (path.EndsWith("/"))
                pathb = path.Replace('/', '\\');
            else if (path.EndsWith("\\"))
                pathb = path.Replace('\\', '/');
            else
            {
                if (path.Contains("/"))
                {
                    path += "/";
                    pathb = path.Replace('/', '\\');
                }
                else
                {
                    path += "\\";
                    pathb = path.Replace('\\', '/');
                }
            }
            foreach (var e in EmbedCatalog.Entries)
            {
                var p = e.FullName;
                if (p.Length > path.Length && (p.StartsWith(path, stringComparison) || p.StartsWith(pathb, stringComparison)))
                {
                    if (includeFolders || !(p.EndsWith("/") || p.EndsWith("\\")))
                    {
                        var key = p.Substring(path.Length);
                        if (!includeSublevels)
                            foreach (var c in key)
                                if (c == '/' || c == '\\')
                                    goto next;
                        yield return new Tuple<string, ZipArchiveEntry>(key, e);
                    next:;
                    }                        
                }
            }
        }

        static public Stream OpenEmbed(string path)
            => GetEmbed(path).Open();

        static public String ReadEmbed(string path)
        {
            using (var r = new StreamReader(OpenEmbed(path)))
                return r.ReadToEnd();
        }
        static public String Read(this ZipArchiveEntry entry)
        {
            using (var r = new StreamReader(entry.Open()))
                return r.ReadToEnd();
        }

        static public bool? TraceHighOrLow
#if DEBUG
            = true
#endif
            ;

        static public void RecordEvent(this Composure cWin, string eventText, bool significantEvent = false)
        {
            if (TraceHighOrLow.HasValue && (significantEvent || TraceHighOrLow.Value))
            {
                    Events.Add(new Tuple<string, DateTime>(eventText, DateTime.Now));
                    if (cWin != null && !cWin.IsDisposed)
                    try
                    {
                        cWin.GrabEvents();
                    }
                    catch (InvalidOperationException) { }
            }
        }
        
        static public readonly List<Tuple<string, DateTime>> Events = new List<Tuple<string, DateTime>>();

        static public string AppDataLocation;
        static public Settings Settings;
        static public List<string> Problem = new List<string>();
        //static public Dictionary<string, string> UsedCommandLine = new Dictionary<string, string>();
        static public string[] args;

        static public string GetExecutableContainingFolder() => Path.GetDirectoryName(GetExecutedFilePath());

        static public string GetAssemblyFilePath(this Assembly assembly)
            => new Uri(assembly.CodeBase).LocalPath;
        static public string GetAssemblyFilePath() => GetAssemblyFilePath(Assembly.GetCallingAssembly());
        static public string GetExecutedFilePath() => GetAssemblyFilePath(Assembly.GetEntryAssembly());

        public static bool EmergencyCloseEnable(this Process process)
        {
            if (EmergencyCloseTracking == null)
                return false;
            try
            {
                File.Create(Path.Combine(EmergencyCloseTracking, process.StartTime.Ticks + "." + process.Id)).Dispose();
                return true;
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            catch (ArgumentException) { }
            catch (InvalidOperationException) { }
            return false;
        }
        public static void EmergencyCloseDisable(this Process process)
        {
            if (process != null && EmergencyCloseTracking != null)
            {
                try
                {
                    File.Delete(Path.Combine(EmergencyCloseTracking, process.StartTime.Ticks + "." + process.Id));
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
                catch (ArgumentException) { }
                catch (InvalidOperationException) { }
            }
        }
        public static void EmergencyCloseExecute()
        {
            if (EmergencyCloseTracking == null) return;
            foreach (var ec in Directory.GetFiles(EmergencyCloseTracking))
            {
                var n = Path.GetFileName(ec).Split('.');
                if (n.Length == 2 && long.TryParse(n[0], out var startTicks) && int.TryParse(n[1], out var pid))
                {
                    try { File.Delete(ec); }
                    catch (IOException) { }
                    catch (ArgumentException) { }
                    catch (UnauthorizedAccessException) { }
                    try
                    {
                        using (var p = Process.GetProcessById(pid))
                            if (p.StartTime.Ticks == startTicks)
                            {
                                if (p.WaitForExit(50)) continue;
                                Problem?.Add("Emergency closing process " + p.ProcessName + " (" + p.Id + ") with start time " + p.StartTime);
                                if (p.CloseMainWindow() && p.WaitForExit(800)) continue;
                                p.Kill();
                            }
                    }
                    catch (Win32Exception) { }
                    catch (ArgumentException) { }
                    catch (InvalidOperationException) { }
                }
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Program.args = args;
            bool defaultcfglocation = true, deploy = true;
            {
                void LoadOrCreateSettings(string path)
                {
                    Settings = new Settings();
                    if (Path.IsPathRooted(path) && path.Length > 3 && path[1] == ':' && path[2] == '\\')
                    {
                        if (Settings.Load(path))
                            return;
                    }
                    else if (string.IsNullOrWhiteSpace(AppDataLocation))
                        Problem.Add("Could not validate settings path, either by a simple drive letter root (L:\\) or by appdata location");
                    else if (Settings.Load(path = Path.Combine(AppDataLocation, path)))
                        return;

                    Settings.Autofill();

                    if (!deploy)
                    {
                        Settings.SaveSettingsOnClose = Settings.When.Never;
                        Settings.LoadError.Add("New settings file not written to disk due to command line options about data directories or deployment");
                        return;
                    }

                    if (defaultcfglocation && AppDataLocation != null)
                    {
                        Settings.LoadError.Clear();
                        if (!Settings.SaveAndApplyRedirect(path, "sessionsettings.cfg"))
                            goto unsaved;
                        goto saved;
                    }

                    if (!Settings.Save(path))
                        goto unsaved;
                    saved:
                    Settings.LoadError.Clear();
                    Settings.LoadError.Add("Initialized new configuration file; this message should not appear next time you run the program with the same shortcut/command.");
                    return;

                    unsaved:
                    Problem.Add("Could not create or read settings at " + path);
                }


                try
                {
                    EmergencyCloseTracking = Directory.CreateDirectory(Path.Combine(DefaultAppDataLocation, "ManagedProcesses")).FullName;
                    File.SetAttributes(EmergencyCloseTracking, File.GetAttributes(EmergencyCloseTracking) | FileAttributes.Hidden);
                }
                catch (IOException) { }
                catch (ArgumentException) { }
                catch (UnauthorizedAccessException) { }

                string cfg = "settings.cfg";
                {
                    string key = null;
                    foreach (var a in args)
                    {
                        if (key == null)
                        {
                            switch (a)
                            {
                                case "--close-others":
                                case "-co":
                                case "-c":
                                    EmergencyCloseExecute();
                                    continue;
                                case "--main-window":
                                case "-mw":
                                    MainWindowStartup = true;
                                    continue;

                                case "--no-deploy":
                                case "-nd":
                                    deploy = false;
                                    continue;
                                default:
                                    key = a;
                                    continue;
                            }
                            //UsedCommandLine[a] = null;
                            //continue;
                        }
                        else
                            switch (key)
                            {
                                case "--data":
                                case "-ad":
                                case "-d":
                                    AppDataLocation = a;
                                    defaultcfglocation = false;
                                    deploy = false;
                                    break;
                                case "--config":
                                case "--settings":
                                case "-cfg":
                                case "-c":
                                case "-s":
                                    cfg = a;
                                    defaultcfglocation = false;
                                    break;
                                default:
                                    Problem.Add("Unexpected parameter " + key);
                                    key = a;
                                    continue;
                            }
                        //UsedCommandLine[key] = a;
                        key = null;
                    }
                    if (key != null)
                        Problem.Add("Unexpected or incomplete parameter " + key);
                }

                if (AppDataLocation == null)
                {
                    AppDataLocation = Environment.GetEnvironmentVariable("appdata");

                    if (AppDataLocation == null)
                        Problem.Add("Unable to resolve environment variable %appdata%");
                    
                    AppDataLocation = Path.Combine(AppDataLocation, DefaultAppDataSubfolder);
                    try
                    {
                        Directory.CreateDirectory(AppDataLocation);
#if DEBUG
                        try { File.SetAttributes(AppDataLocation, File.GetAttributes(AppDataLocation) | FileAttributes.Hidden); }
                        catch { }
#endif
                    }
                    catch (IOException)
                    {
                        Problem.Add("Unable to access/create subfolder " + AppDataLocation);
                        AppDataLocation = null;
                    }
                }

                if (defaultcfglocation)
                {
                    if (deploy && AppDataLocation != null)
                    {
                        var l = Path.GetFullPath(AppDataLocation);
                        if (!l.EndsWith(Path.DirectorySeparatorChar.ToString())) l += Path.DirectorySeparatorChar;

                        foreach (var file in GetEmbedsIn("Deployable/AppData/"))
                        {
                            var name = file.Item1.Replace('/', '\\');
                            name = Path.GetFullPath(Path.Combine(l, name));

                            if (File.Exists(name)) continue;

                            if (!name.StartsWith(l, StringComparison.OrdinalIgnoreCase))
                            {
                                Problem.Add("Path " + name + " not match for container " + l);
                                continue;
                            }

                            try
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(name));
                                using (var deployfile = File.Open(name, FileMode.CreateNew))
                                using (var deployfrom = file.Item2.Open())
                                    deployfrom.CopyTo(deployfile);
                            }
                            catch (IOException)
                            {
                                Problem.Add("Unable to deploy embedded file " + name);
                            }
                        }
                    }
                }

                LoadOrCreateSettings(cfg);
            }
            Settings = Settings.RedirectedToSettings ?? Settings;

            if (Settings.EnableVisualStyles) 
                Application.EnableVisualStyles();

            if (Problem.Count == 0) Problem = null;

            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                {
                    Faith in_fundamental_solidarity = new Faith();
                    try
                    {

                        Application.Run(in_fundamental_solidarity);
                    }
                    finally
                    {
                        in_fundamental_solidarity.ClearIPC(false);
                    }
                }

                if (Settings.SaveSettingsOnClose != Settings.When.Never)
                    try
                    {
                        Settings.Save();
                    }
                    catch (InvalidOperationException) { }
            }
            finally
            {
                if (EmergencyCloseTracking != null)
                    using (var p = Process.GetCurrentProcess())
                        p.EmergencyCloseDisable();
            }
        }
    }
}
