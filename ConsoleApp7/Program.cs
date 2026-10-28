using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Console.OutputEncoding = Encoding.UTF8;

        ProcessStartInfo startInfo = new()
        {
            FileName = "cmd.exe",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,

            StandardOutputEncoding = Encoding.GetEncoding(866),
            StandardErrorEncoding = Encoding.GetEncoding(866)
        };

        startInfo.ArgumentList.Add("/c");
        startInfo.ArgumentList.Add("echo Сообщение в stdout & echo Ошибка в stderr 1>&2");

        using Process process = new()
        {
            StartInfo = startInfo
        };

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            process.Start();

            Console.WriteLine($"PID: {process.Id}");

            Task<string> stdoutTask = process.StandardOutput.ReadToEndAsync();
            Task<string> stderrTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            string stdout = await stdoutTask;
            string stderr = await stderrTask;

            stopwatch.Stop();

            Console.WriteLine($"Длительность: {stopwatch.Elapsed}");
            Console.WriteLine($"Exit code: {process.ExitCode}");
            Console.WriteLine();

            Console.WriteLine("=== STDOUT ===");
            Console.WriteLine(stdout.TrimEnd());

            Console.WriteLine();

            Console.WriteLine("=== STDERR ===");
            Console.WriteLine(stderr.TrimEnd());
        }
        catch (Win32Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"Длительность: {stopwatch.Elapsed}");
            Console.WriteLine($"Ошибка запуска: {ex.Message}");
        }
    }
}