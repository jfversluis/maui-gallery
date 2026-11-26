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
#endif
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

#if DEBUG
    /// <summary>The crash log the Windows UI crawl collects.</summary>
    private static string CrashLogPath => Path.Combine(Path.GetTempPath(), "MauiGallery-crash.log");

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
