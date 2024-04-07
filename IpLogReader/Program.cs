﻿
using IpLogParser.Reader;
using IpLogParser.Options;
using IpLogParser.Writer;
using System.ComponentModel.DataAnnotations;

namespace IpLogParser;

internal class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            var parser = new ConfigurationArgsParser(args, "config.json");
            var log_reader = new IpLogReader();
            var writer = new FileIpLogWriter();

            var options = parser.Parse();
            var result = log_reader.Read(options);
            await writer.WriteAsync(options.FileOutput!, result);

            if (result.Errors?.Count() > 0)
            {
                Console.WriteLine($"While reading '{options.FileLog}' file there was {result.Errors.Count()} non critical errors occured.");
                foreach (var err in result.Errors)
                {
                    Console.WriteLine(err.Message);
                }
            }

            Console.WriteLine($"Result successfully exported to '{options.FileOutput}' ({result.AddressToRequestCount!.Count} lines total).");
        }
        catch (ValidationException e)
        {
            Console.WriteLine($"Input error: {e.Message}");
            Console.Write("Usage: IpLogParser.exe --file-log=<path> [REQUIRED] --file-output=<path> [REQUIRED]");
            Console.Write(" --address-start=<IP-Address> --address-mask=<CIDR mask>");
            Console.WriteLine(" --time-start=<dd.MM.yyyy> --time-end=<dd.MM.yyyy>");
            Console.WriteLine("Note: These parameters can also be set through JSON configuration or ENV variables.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Critical error occured: {e.Message}\nApplication cannot continue to work.\nPlease review your input arguments or log file.");
        }
    }
}
