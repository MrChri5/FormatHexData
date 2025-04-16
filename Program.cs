//This program reads a .txt file containing hexadecimal data in the following format
// and outputs it as a single line of just the hexadecimal data.
/*
Raw Dump : 29000 bytes
0000: 74C874C8D9E340E4 F1F8F1F0F1404040 4040404040404040 4040404040404040 - ÃHÃHRT.U18101...................
0020: 40404040404040E4 F1F8F1F0F160D140 0000004000000000 0000000000000000 - .......U18101-J.................
0040: ... Line(s) containing only [0x00] ...
00A0: 0000000000F1F0F1 F7F9F84040404040 4040404040404040 4040404040404040 - .....101798.....................
00C0: 4040404040F3F7F3 F5F3404040404040 4040404040404040 4040404040404040 - .....37353......................
00E0: 4040404040F5D7F2 F0F2F560F0F460F1 F4E3F1F07AF5F17A F0F04EF0F17AF0F0 - .....5P2025-04-14T10:51:00+01:00
0100: F2F0F2F560F0F460 F1F4E3F1F57AF0F4 7AF0F04EF0F17AF0 F040404040404040 - 2025-04-14T15:04:00+01:00.......
0120: ... Line(s) containing only [0x40] ...
0340: 4040404040404040 4040404040404040 4040404040404040 40F0F0F0F0F0F0F0 - .........................0000000
0360: F0F0F0C0F0F0F140 4040404040404040 4040404040404040 4040404040404040 - 000{001......................... 
*/

using System.Globalization;

namespace FormatHexData
{
    internal static class Program
    {
        static void Main(string[] args)
        {

            StreamReader objReader;
            StreamWriter objWriter;
            string inputFileName;
            string outputFileName;

            //Check the input file exists.
            if (args.Length == 0
                || !(inputFileName = args[0]).Contains(".txt")
                || !File.Exists(inputFileName))
            {
                Console.WriteLine("Drag a .txt file onto this .exe file.");
                Console.ReadLine();
                return;
            }
            //Build the output file name.
            outputFileName = inputFileName.Substring(0, inputFileName.IndexOf(".txt")) + "_edit.txt";

            //Open the input and output files.
            objReader = new StreamReader(inputFileName);
            objWriter = new StreamWriter(outputFileName);

            //Variables for processing the file.
            string? readLine;
            int prevDataAddress = 0;
            string? prevDataPad = null;

            //Read and process each line of the file.
            while ((readLine = objReader.ReadLine()) != null)
            {
                int dataAddress;
                //Data lines should begin with a 4 digit hexadecimal address. Skip any that do not.
                if (readLine.Length < 4
                    || !int.TryParse(readLine.Substring(0, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out dataAddress))
                {
                    Console.WriteLine($"Line skipped:{readLine}");
                    continue;
                }
                //If the previously read line was "Line(s) containing only [0x**]", pad up to the current data address with that character.
                if (prevDataPad != null)
                {
                    for (int i = prevDataAddress; i < dataAddress; i++)
                    {
                        objWriter.Write(prevDataPad);
                    }
                    prevDataPad = null;
                }
                prevDataAddress = dataAddress;

                //If the current line is "Line(s) containing only [0x**]". Save the padding character.
                if (readLine.Contains("Line(s) containing only [0x"))
                {
                    prevDataPad = readLine.Substring(37, 2);
                }
                //Otherwise output the data portion of the line.
                else if (readLine.Length > 73)
                {
                    foreach (char c in readLine.Substring(6, 67))
                    {
                        if (c != ' ')
                            objWriter.Write(c);
                    }
                }
            }

            //Close files.
            objReader.Close();
            objWriter.Close();

            Console.WriteLine("File edited successfully.");
            Console.ReadLine();
            return;
        }
    }
}