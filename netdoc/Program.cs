using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace netdoc
{
    class Program
    {
        static void Main(string[] args)
        {
            String config_file;

            if (args.Length == 0)
            {
                Console.WriteLine("No argument provided. Default config file is being used.");
                config_file = "config.json";
            } else {
                config_file = args[0];
                Console.WriteLine($"Using configuration from {config_file}");
            }

            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(config_file, optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            Console.WriteLine(configuration["Settings:in_folder"].ToString());


        }
    }
}
