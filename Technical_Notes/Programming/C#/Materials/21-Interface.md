using System;
using System.Collections.Generic;

interface ILogger
{
    string LogPath { get; set; }
    int LogCount { get; set; }
    double StorageSizeMb { get; set; }
    decimal OperationalCost { get; set; }
    bool IsActive { get; set; }
    char SystemCode { get; set; }
    DateTime CreatedAt { get; set; }
    List<string> Categories { get; set; }

    void Log(string message);
    bool SetLog();
    string GetLastLog();
    int GetTotalLogsCount();
}

class FileLogger : ILogger
{
    private string _lastMessage;

    public string LogPath { get; set; }
    public int LogCount { get; set; }
    public double StorageSizeMb { get; set; }
    public decimal OperationalCost { get; set; }
    public bool IsActive { get; set; }
    public char SystemCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Categories { get; set; }

    public FileLogger(string logpath, int logCount, double storageSizeMb, decimal operationalCost, bool isActive, char systemCode)
    {
        LogPath = logpath;
        LogCount = logCount;
        StorageSizeMb = storageSizeMb;
        OperationalCost = operationalCost;
        IsActive = isActive;
        SystemCode = systemCode;
        CreatedAt = DateTime.Now;
        Categories = new List<string> { "Error", "Warning", "Info" };
    }

    public bool SetLog()
    {
        return true;
    }

    public void Log(string message)
    {
        _lastMessage = message;
        LogCount++;
    }

    public string GetLastLog()
    {
        return $"Written to file [{LogPath}]: {_lastMessage}";
    }

    public int GetTotalLogsCount()
    {
        return LogCount;
    }
}

class DatabaseLogger : ILogger
{
    private string _lastMessage;

    public string LogPath { get; set; }
    public int LogCount { get; set; }
    public double StorageSizeMb { get; set; }
    public decimal OperationalCost { get; set; }
    public bool IsActive { get; set; }
    public char SystemCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Categories { get; set; }

    public DatabaseLogger(string logpath, int logCount, double storageSizeMb, decimal operationalCost, bool isActive, char systemCode)
    {
        LogPath = logpath;
        LogCount = logCount;
        StorageSizeMb = storageSizeMb;
        OperationalCost = operationalCost;
        IsActive = isActive;
        SystemCode = systemCode;
        CreatedAt = DateTime.Now;
        Categories = new List<string> { "Audit", "Security" };
    }

    public bool SetLog()
    {
        return true;
    }

    public void Log(string message)
    {
        _lastMessage = message;
        LogCount++;
    }

    public string GetLastLog()
    {
        return $"Written to database [{LogPath}]: {_lastMessage}";
    }

    public int GetTotalLogsCount()
    {
        return LogCount;
    }
}

class Program
{
    static void Main(string[] args)
    {
        ILogger fileLogger = new FileLogger("app.log", 10, 25.5, 99.99m, true, 'F');
        ILogger dbLogger = new DatabaseLogger("UsersDatabase", 100, 500.75, 299.50m, true, 'D');

        fileLogger.Log("File error occurred");
        dbLogger.Log("Database connection lost");

        Console.WriteLine(fileLogger.GetLastLog());
        Console.WriteLine(dbLogger.GetLastLog());

        Console.WriteLine($"Total File Logs: {fileLogger.GetTotalLogsCount()}");
        Console.WriteLine($"Total DB Logs: {dbLogger.GetTotalLogsCount()}");
    }
}
