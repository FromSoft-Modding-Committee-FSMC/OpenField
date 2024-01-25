using Mixolydian.ConsoleHelper;
using Mixolydian.IO;
using Mixolydian.IO.Asset;
using Mixolydian.IO.Asset.Image;
using Mixolydian.IO.Format;
using Mixolydian.IO.Format.ForFolks;
using Mixolydian.String;
using RPCTool;

internal class Program
{
    private static ImageFactory imageFormatFactory = new();

    private static void Main(string[] args)
    {
        //Parse Command Line
        CommandLineParser parser = new CommandLineParser();
        parser.AddPossibleMode("convert");
        parser.AddPossibleMode("peekmeta");
        parser.AddPossibleMode("stripmeta");
        parser.AddPossibleMode("help");
        parser.AddPossibleMode("?");
        parser.AddPossibleFlag(new CommandLineFlag("i", typeof(string)));
        parser.AddPossibleFlag(new CommandLineFlag("ifmt", "*"));
        parser.AddPossibleFlag(new CommandLineFlag("o", typeof(string)));
        parser.AddPossibleFlag(new CommandLineFlag("ofmt", "rpc"));
        parser.Parse(args);

        //Print Program Title AND disclaimer
        ConsoleTitle();
        ConsoleDisclaimer();

        switch (parser.Mode)
        {
            case "convert":
                goto Mode_Convert;

            case "peekmeta":
                goto Mode_PeekMeta;

            case "stripmeta":
                goto Mode_StripMeta;

            case "help":
            case "?":
                goto Mode_Help;

            default:
                Console.WriteLine($"Unknown Mode '{parser.Mode}'!".Colourize(0xA00000));
                ConsoleHelp();
                goto ProgramCloseError;
        }

    //
    // Convert between image formats (X -> RPC, RPC -> X)
    //
    Mode_Convert:
        Console.WriteLine("~-~-~-~-~-~-~-~-~-~-~- Conversion ~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-".Colourize(0x00FF80));
        string inputFile, outputFile;
        IFormat<ImageAsset>? inputFormat, outputFormat;

        //Get Input File
        if (!parser["i"].Set)
        {
            Console.WriteLine("No Input File Set!".Colourize(0xA00000));
            goto ProgramCloseError;
        }
        inputFile = parser["i"].GetValue<string>();
        Console.Write("Input File: ".Colourize(0xFF00FF)); Console.WriteLine($"{inputFile}".Colourize(0xFFFFFF));

        //Get Output File
        if (!parser["o"].Set)
        {
            outputFile = inputFile.Replace(Path.GetExtension(inputFile), "");
        }
        else
        {
            outputFile = parser["o"].GetValue<string>();
        }
        outputFile += $".{parser["ofmt"].GetValue<string>()}";

        Console.Write("Output File: ".Colourize(0xFF00FF)); Console.WriteLine($"{outputFile}".Colourize(0xFFFFFF));

        //Get Input Format
        if (!parser["ifmt"].Set || parser["ifmt"].GetValue<string>() == "*")
        {
            inputFormat = GetFormatAuto(inputFile);
        }
        else
        {
            inputFormat = GetFormatAuto(inputFile.Replace(Path.GetExtension(inputFile), $".{parser["ifmt"].GetValue<string>()}"));     
        }
        
        if(inputFormat == null)
        {
            Console.WriteLine("Could not get input format!".Colourize(0xA00000));
            goto ProgramCloseError;
        }

        Console.Write("Input Format: ".Colourize(0xFF00FF));
        Console.Write("{\n".Colourize(0x808080));
        Console.WriteLine($"\tName ~~~~~~~ {inputFormat.Parameters.metadata.name}");
        Console.WriteLine($"\tDesc ~~~~~~~ {inputFormat.Parameters.metadata.description}");
        Console.WriteLine($"\tVersion ~~~~ {inputFormat.Parameters.metadata.version}");
        Console.WriteLine($"\tDate ~~~~~~~ {inputFormat.Parameters.metadata.specRevisionDate}");
        Console.WriteLine($"\tAuthors ~~~~ {string.Join(", ", inputFormat.Parameters.metadata.authors)}");
        Console.WriteLine($"\tCategories ~ {string.Join(", ", inputFormat.Parameters.metadata.categories)}");
        Console.WriteLine($"\tExtensions ~ {string.Join(", ", inputFormat.Parameters.metadata.extensions)}");
        Console.WriteLine("}".Colourize(0x808080));

        if (!inputFormat.Parameters.allowImport)
        {
            Console.WriteLine("Input format is not an importable format!".Colourize(0xA00000));
            goto ProgramCloseError;
        }

        //Get Output Format
        if (!parser["ofmt"].Set || parser["ofmt"].GetValue<string>().Equals("rpc", StringComparison.InvariantCultureIgnoreCase))
        {
            outputFormat = imageFormatFactory.GetFormat("rpc")[0];
        }
        else
        {
            List<IFormat<ImageAsset>> outputFormats = imageFormatFactory.GetFormat(parser["ofmt"].GetValue<string>());
            if(outputFormats.Count == 1)
            {
                outputFormat = outputFormats[0];
            }
            else
            {
                Console.WriteLine("Output format is ambiguous!".Colourize(0xA00000));
                goto ProgramCloseError;
            }
        }

        if(outputFormat == null)
        {
            Console.WriteLine("Could not get output format!".Colourize(0xA00000));
            goto ProgramCloseError;
        }

        Console.Write("Output Format: ".Colourize(0xFF00FF));
        Console.Write("{\n".Colourize(0x808080));
        Console.WriteLine($"\tName ~~~~~~~ {outputFormat.Parameters.metadata.name}");
        Console.WriteLine($"\tDesc ~~~~~~~ {outputFormat.Parameters.metadata.description}");
        Console.WriteLine($"\tVersion ~~~~ {outputFormat.Parameters.metadata.version}");
        Console.WriteLine($"\tDate ~~~~~~~ {outputFormat.Parameters.metadata.specRevisionDate}");
        Console.WriteLine($"\tAuthors ~~~~ {string.Join(", ", outputFormat.Parameters.metadata.authors)}");
        Console.WriteLine($"\tCategories ~ {string.Join(", ", outputFormat.Parameters.metadata.categories)}");
        Console.WriteLine($"\tExtensions ~ {string.Join(", ", outputFormat.Parameters.metadata.extensions)}");
        Console.WriteLine("}".Colourize(0x808080));

        if (!outputFormat.Parameters.allowExport)
        {
            Console.WriteLine("Output format is not an exportable format!".Colourize(0xA00000));
            goto ProgramCloseError;
        }

        //
        // Start Conversion Process
        //
        ImageAsset? asset;
        if(!inputFormat.Load(inputFile, out asset))
        {
            Console.WriteLine($"Could not import file '{inputFile}'!".Colourize(0xA00000));
            goto ProgramCloseError;
        }

        if(asset == null)
        {
            Console.WriteLine($"Could not import file '{inputFile}'!".Colourize(0xA00000));
            goto ProgramCloseError;
        }

        if(!outputFormat.Save(outputFile, asset))
        {
            Console.WriteLine($"Could not export file '{outputFile}'!".Colourize(0xA00000));
            goto ProgramCloseError;
        }

        goto ProgramClose;

    //
    // Peek RPC Meta Data
    //
    Mode_PeekMeta:
        goto ProgramClose;

    //
    // Strip RPC Meta Data
    //
    Mode_StripMeta:
        goto ProgramClose;

    //
    // Show Program Help
    //
    Mode_Help:
        ConsoleHelp();
        goto ProgramClose;

    ProgramClose:
        Console.WriteLine("Finished! No Errors.".Colourize(0x00FF00));
        Environment.Exit(0);
        return;

    ProgramCloseError:
        Console.WriteLine("Finished With Errors!".Colourize(0xFF0000));
        Environment.Exit(1);
        return;
    }

    private static IFormat<ImageAsset>? GetFormatAuto(string filePath)
    {
        //Auto Detect Format - Easy Mode
        List<IFormat<ImageAsset>> formats = imageFormatFactory.GetFormat(Path.GetExtension(filePath)[1..]);

        //One format
        if(formats.Count == 1)
            return formats[0];

        //Multiple Formats
        if(formats.Count > 1)
        {
            using (InputStream tempins = new InputStream(filePath))
            {
                return imageFormatFactory.GetFormat(tempins.ReadBytes((int)tempins.Size));
            }
        }

        //Zero formats
        return null;
    }

    private static void ConsoleTitle()
    {
        Console.WriteLine("~-~-~-~-~-~ RPC Command Line Tool Version 1.0d -~-~-~-~-~-~-~-~-".Colourize(0x00FF80));
        Console.WriteLine($"RPC Implementation Info: ".Colourize(0xFF8000));
        Console.Write($"\tFormat Version: ".Colourize(0xFF00FF));
        Console.Write($"{RPCFormat.RPC_VERSION_SPEC}\n".Colourize(0xFFFFFF));
        Console.Write($"\tAvailable Extensions: ".Colourize(0xFF00FF));
        Console.Write("{\n".Colourize(0x808080));
        Console.WriteLine("\t}".Colourize(0x808080));

        //Display Importable Formats
        Console.WriteLine($"Supported Import Formats: ".Colourize(0xFF8000));
        Console.Write("\t");

        List<SFormatMetadata> importableFormats = imageFormatFactory.EnumerateFormats(EFormatFilter.Importable);
        for(int i = 0; i < importableFormats.Count; ++i)
        {
            if (i > 0)
                Console.Write(", ".Colourize(0x808080));
            Console.Write(importableFormats[i].extensions[0].Colourize(0xF0F0F0));
        }
        Console.WriteLine();

        //Display Exportable Formats
        Console.WriteLine($"Supported Export Formats: ".Colourize(0xFF8000));
        Console.Write("\t");

        List<SFormatMetadata> exportableFormats = imageFormatFactory.EnumerateFormats(EFormatFilter.Exportable);
        for (int i = 0; i < exportableFormats.Count; ++i)
        {
            if (i > 0)
                Console.Write(", ".Colourize(0x808080));
            Console.Write(exportableFormats[i].extensions[0].Colourize(0xF0F0F0));
        }
        Console.WriteLine();
    }
    private static void ConsoleDisclaimer()
    {
        Console.WriteLine("~-~-~-~-~-~-~-~-~-~ HEY, YOU! EXCUSE ME, BUT -~-~-~-~-~-~-~-~-~-".Colourize(0x00FF80));
        Console.WriteLine("This tool is intended for development of the format only, and should not be");
        Console.WriteLine("taken as an example of:");
        Console.WriteLine("\tA) Production quality code");
        Console.WriteLine("\tB) Good practice");
        Console.WriteLine("\tC) Good taste in programming techniques");
        Console.WriteLine("\tD) A guide to implement the ARI format to a high quality");
        Console.WriteLine("Thank you, ");
        Console.WriteLine("StolenBattenberg");
        Console.WriteLine();
    }
    private static void ConsoleHelp()
    {
        Console.WriteLine("~-~-~-~-~-~-~-~-~-~-~-~ Help Menu ~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-".Colourize(0x00FF80));
        Console.WriteLine("Modes: ".Colourize(0xFF8000));
        Console.Write("\thelp or ?        ".Colourize(0xFF00FF));
        Console.Write("Display this screen\n".Colourize(0xFFFFFF));
        Console.Write("\tconvert          ".Colourize(0xFF00FF));
        Console.Write("Convert between image formats\n".Colourize(0xFFFFFF));
        Console.Write("\tpeekmeta         ".Colourize(0xFF00FF));
        Console.Write("Displays meta stored inside a RPC file\n".Colourize(0xFFFFFF));
        Console.Write("\tstripmeta        ".Colourize(0xFF00FF));
        Console.Write("Removes all meta from an RPC file\n".Colourize(0xFFFFFF));
        Console.WriteLine();

        Console.WriteLine("Flags ('--', '/'): ".Colourize(0xFF8000));
        Console.Write("\ti ".Colourize(0xFF00FF));                      //I
        Console.Write("string[]       ".Colourize(0x0040FF));
        Console.Write("Specify Input File. Required.\n".Colourize(0xFFFFFF));
        Console.Write("\to ".Colourize(0xFF00FF));                      //O
        Console.Write("string[]       ".Colourize(0x0040FF));
        Console.Write("Specify Output File. Required.\n".Colourize(0xFFFFFF));
        Console.Write("\tifmt ".Colourize(0xFF00FF));                   //IFMT
        Console.Write("string      ".Colourize(0x0040FF));
        Console.Write("Specify Input Format. Optional (Auto Detect).\n".Colourize(0xFFFFFF));
        Console.Write("\tofmt ".Colourize(0xFF00FF));                   //OFMT
        Console.Write("string      ".Colourize(0x0040FF));
        Console.Write("Specify Output Format. Optional (RPC).\n".Colourize(0xFFFFFF));
    }
}