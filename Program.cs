using System.Diagnostics;

namespace ParallelHomework;

class Program
{
    static async Task Main(string[] args)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        var spaces = await CalculateSpacesInDirectory("/Users/Nikita/RiderProjects/ParallelHomework");
        
        stopwatch.Stop();
        Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Spaces: {spaces}");
    }
    
    private static async Task<int> CalculateSpacesInDirectory(string path)
    {
        Task[] tasks = new Task[Directory.EnumerateFiles(path, "*.txt").Count()];
        var spaces = 0;
        int c = 0;
        foreach (string fileName in Directory.EnumerateFiles(path, "*.txt"))
        {
            var task = new Task(async () =>
            {
                var spacesInFile = await CalculateSpacesInFile(fileName);
                spaces += spacesInFile;
            });
            task.Start();
            tasks[c++] = task;
        }
        
        Task.WaitAll(tasks);
        
        return spaces;
    }
    
    private static async Task<int> CalculateSpacesInFile(string fileName)
    {
        var spaces = 0;
        await using FileStream sourceStream = File.Open(fileName, FileMode.Open);
        byte[] buffer = new byte[1024];
        int bytesRead = 0;
        while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) != 0)
        {
            for (int i = 0; i < bytesRead; i++)
            {
                if (buffer[i] == ' ')
                {
                    spaces++;
                }
            }
        }

        return spaces;
    }
}