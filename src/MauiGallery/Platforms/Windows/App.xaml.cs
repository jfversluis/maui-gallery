namespace MauiGallery.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        InitializeComponent();
#if DEBUG
        // WinUI ends the process on an unhandled exception with only a stowed-exception code in the event log. Keep
        // the managed exception, so UI test runs (see docs/windows-validation.md) can report what crashed.
        UnhandledException += (_, e) => LogCrash(e.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, e) => LogCrash(e.ExceptionObject as Exception);
        AppDomain.CurrentDomain.FirstChanceException += (_, e) => LogFirstChance(e.Exception);
#endif
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

#if DEBUG
    /// <summary>The crash log the Windows UI crawl collects.</summary>
    private static string CrashLogPath => Path.Combine(Path.GetTempPath(), "MauiGallery-crash.log");

    private static readonly HashSet<string> s_firstChance = [];

    /// <summary>
    /// Handled exceptions too, once per type and message: platform views (such as WebView2) often swallow the
    /// exception that explains why they show nothing.
    /// </summary>
    private static void LogFirstChance(Exception exception)
    {
        var key = $"{exception.GetType().FullName}: {exception.Message}";
        lock (s_firstChance)
        {
            if (s_firstChance.Count > 200 || !s_firstChance.Add(key))
            {
                return;
            }
        }

        try
        {
            File.AppendAllText(Path.Combine(Path.GetTempPath(), "MauiGallery-first-chance.log"), $"[{DateTime.Now:O}] {key}{Environment.NewLine}{exception.StackTrace}{Environment.NewLine}{Environment.NewLine}");
        }
        catch (IOException)
        {
            // Diagnostics only.
        }
    }

    private static void LogCrash(Exception? exception)
    {
        try
        {
            File.AppendAllText(CrashLogPath, $"[{DateTime.Now:O}] {exception}{Environment.NewLine}{Environment.NewLine}");
        }
        catch (IOException)
        {
            // Logging must never hide the original crash.
        }
    }
#endif
}
