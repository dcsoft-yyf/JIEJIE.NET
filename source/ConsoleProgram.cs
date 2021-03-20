using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DCNETProtector
{
    static class ConsoleProgram
    {
        static void Main(string[] args)
        {
            string inputAssmblyFileName = null;
            string outputAssemblyFileName = null;
            string snkFileName = null;
            bool pause = false;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"
*******************************************************************************
  _____   _____   _   _ ______ _______   _____           _            _             
 |  __ \ / ____| | \ | |  ____|__   __| |  __ \         | |          | |            
 | |  | | |      |  \| | |__     | |    | |__) | __ ___ | |_ ___  ___| |_ ___  _ __ 
 | |  | | |      | . ` |  __|    | |    |  ___/ '__/ _ \| __/ _ \/ __| __/ _ \| '__|
 | |__| | |____ _| |\  | |____   | |    | |   | | | (_) | ||  __/ (__| || (_) | |   
 |_____/ \_____(_)_| \_|______|  |_|    |_|   |_|  \___/ \__\___|\___|\__\___/|_|   
                                                                                    

     DC.NET Protector ,protect your .NET software copyright powerfull.
     last update 2021-3-19
     Author:yuan yong fu from CHINA. mail: 28348092@qq.com

     Support command line argument :
        input =[required,Full path of input .NET assembly file , can be .exe or .dll, currenttly only support .NET framework 2.0 or later]
        output=[optional,Full path of output .NET assmebly file , if it is empty , then use input argument value]
        snk   =[optional,Full path of snk file. It use to add strong name to output assembly file.]
        pause =[optional,pause the console after finish process.]

     Example 1, protect d:\a.dll ,this will modify dll file.
        DCNETProtector.exe input=d:\a.dll  
     Exmaple 2, anlyse d:\a.dll , and write result to another dll file with strong name.
        DCNETProtector.exe input=d:\a.dll output=d:\publish\a.dll snk=d:\source\company.snk
　　　　　　　　　　　　　　　　　　　　
*******************************************************************************");
            Console.ResetColor();
            if (args != null)
            {
                foreach (var arg in args)
                {
                    int index = arg.IndexOf('=');
                    if (index > 0)
                    {
                        string argName = arg.Substring(0, index).Trim().ToLower();
                        string argValue = arg.Substring(index + 1).Trim();
                        switch (argName)
                        {
                            case "input":
                                inputAssmblyFileName = argValue;
                                if (File.Exists(inputAssmblyFileName) == false)
                                {
                                    return;
                                }
                                break;
                            case "output":
                                outputAssemblyFileName = argValue;
                                break;
                            case "snk":
                                snkFileName = argValue;
                                break;
                            case "pause":
                                pause = true;
                                break;
                        }
                    }
                    else if( string.Compare( arg , "pause" , StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        pause = true;
                    }
                }
            }
            try
            {
                if (snkFileName != null
                   && snkFileName.Length > 0
                   && File.Exists(snkFileName) == false)
                {
                    ConsoleWriteError("Can not find file : " + snkFileName);
                    return;
                }
                if (inputAssmblyFileName != null && inputAssmblyFileName.Length > 0 )
                {
                    if (File.Exists(inputAssmblyFileName))
                    {
                        DCProtectEngine.ExecuteAssemblyFile(inputAssmblyFileName, snkFileName, outputAssemblyFileName);
                    }
                    else
                    {
                        ConsoleWriteError("Can not find file : " + inputAssmblyFileName);
                    }
                }
            }
            catch( System.Exception ext )
            {
                ConsoleWriteError(ext.ToString());
            }
            finally
            {
                if( pause )
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        private static void ConsoleWriteError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
    }
}
