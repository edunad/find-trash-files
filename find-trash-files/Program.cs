
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

/*
 *  
 * ___ _           _ _____               _       ___ _ _           
  / __(_)_ __   __| /__   \_ __ __ _ ___| |__   / __(_) | ___  ___ 
 / _\ | | '_ \ / _` | / /\/ '__/ _` / __| '_ \ / _\ | | |/ _ \/ __|
/ /   | | | | | (_| |/ /  | | | (_| \__ \ | | / /   | | |  __/\__ \
\/    |_|_| |_|\__,_|\/   |_|  \__,_|___/_| |_\/    |_|_|\___||___/
Author : Eduardo de Sousa Fernandes (AKA FailCake - edunad)
 * 
 * 
 * From stackoverflow.com :
 * public static bool FilesAreEqual_OneByte(FileInfo first, FileInfo second)
 * public static bool FilesAreEqual_Hash(FileInfo first, FileInfo second)
 * public static bool CompareImages(Bitmap bmp1, Bitmap bmp2)
*/

namespace FindTrashFiles
{

    public class DupedFile
    {
        public Color color;
        public int CH = 0;
        public List<string> path;
    }

    public struct FilePos
    {
        public int X;
        public int Y;
    }

    public class Program
    {
        public static Dictionary<string, FilePos> filePos = new Dictionary<string, FilePos>();
        public static int[,] ConsoleBuffer;
        public static int endYPos;
        public static bool disableUI = false;
        public static bool serverMode = false;

        public static bool FilesAreEqual_OneByte(FileInfo first, FileInfo second)
        {
            if (first.Length != second.Length)
                return false;

            using (FileStream fs1 = first.OpenRead())
            using (FileStream fs2 = second.OpenRead())
            {
                for (int i = 0; i < first.Length; i++)
                {
                    if (fs1.ReadByte() != fs2.ReadByte())
                        return false;
                }
            }

            return true;
        }

        public static bool FilesAreEqual_Hash(FileInfo first, FileInfo second)
        {
            byte[] firstHash = MD5.Create().ComputeHash(first.OpenRead());
            byte[] secondHash = MD5.Create().ComputeHash(second.OpenRead());

            for (int i = 0; i < firstHash.Length; i++)
            {
                if (firstHash[i] != secondHash[i])
                    return false;
            }
            return true;
        }

        public static bool CompareImages(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1.Size != bmp2.Size)
            {
                return false;
            }
            else
            {
                for (int x = 0; x < bmp1.Width; x++)
                {
                    for (int y = 0; y < bmp1.Height; y++)
                    {
                        if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                            return false;
                    }
                }

            }
            return true;
        }

        public static void UpdateFileSquare(string filePath, string color, int CH)
        {
            if (disableUI) return;

            if (filePos.ContainsKey(filePath))
            {
                FilePos pos = filePos[filePath];
                Console.CursorLeft = pos.X;
                Console.CursorTop = pos.Y;

                WriteLine(color, false);

                if (CH < 0 && ConsoleBuffer[pos.X, pos.Y] != 0)
                    CH = ConsoleBuffer[pos.X, pos.Y];

                ConsoleBuffer[pos.X, pos.Y] = CH;
                Console.Write((char)CH);
            }

        }

        public static bool isPathImage(string path)
        {
            return Regex.Match(path, "([^\\s]+(\\.(?i)(jpg|png|gif|bmp|tif))$)", RegexOptions.IgnoreCase).Success;
        }

        static void Main(string[] args)
        {

            /* Console Settings */
            Console.Title = ".: FindTrashFiles :. - I'm too lazy to manually find the duped / empty files version";
            Console.CursorVisible = false;
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
            Console.WindowWidth = 150;
            Console.WindowHeight = 64;

            ConsoleBuffer = new int[Console.BufferWidth, Console.BufferHeight];

            /* ================ */

            if (args.Length < 2)
            {
                WriteLine("#1Missing params {#5folder location#1} {#5output location#1}", true);
                Thread.Sleep(1000);
                return;
            }

            string location = args[0];
            string output = args[1];

            Console.WriteLine("   ___ _           _ _____               _       ___ _ _           "); // If you are looking for the error because vsstudio says its around here. Ignore it.
            Console.WriteLine("  / __(_)_ __   __| /__   \\_ __ __ _ ___| |__   / __(_) | ___  ___ "); // Its just some weird bug.
            Console.WriteLine(" / _\\ | | '_ \\ / _` | / /\\/ '__/ _` / __| '_ \\ / _\\ | | |/ _ \\/ __|");
            Console.WriteLine("/ /   | | | | | (_| |/ /  | | | (_| \\__ \\ | | / /   | | |  __/\\__ \\");
            Console.WriteLine("\\/    |_|_| |_|\\__,_|\\/   |_|  \\__,_|___/_| |_\\/    |_|_|\\___||___/");
            Console.WriteLine("By Eduardo de Sousa Fernandes :)");

            Console.WriteLine("\n");
            WriteLine("#1Scanning project folder: " + location, true);
            WriteLine("#1Output folder: " + output + "\n", true);

            List<string> ignore = new List<string>(){
                "bin",
                ".git"
            };

            // Check extensions
            string waitingParams = "";
            bool errorInParams = false;

            if (args.Length > 2)
                foreach (string arg in args)
                    if (waitingParams == "")
                    {
                        if (arg == "-filter")
                        {
                            waitingParams = arg;
                        }
                        else if (arg == "-disableUI")
                        {
                            disableUI = true;
                            WriteLine("#1UI Disabled!\n", true);
                        }
                        else if (arg == "-servermode")
                        {
                            serverMode = true;
                            WriteLine("#1Running in server mode\n", true);
                        }
                    }
                    else
                    {
                        if (waitingParams == "-filter" && arg != "")
                        {
                            string[] extra = arg.Split(',');

                            if (extra.Length > 0)
                                ignore.AddRange(extra.ToList<string>());
                            else
                            {
                                WriteLine("#5Incorrect extra filters format! #1Make sure you split them with ,", true);
                                errorInParams = true;
                                break;
                            }

                            waitingParams = "";
                        }
                        else if (waitingParams != "")
                            errorInParams = true;

                    }



            if (errorInParams)
            {
                WriteLine("#5Incorrect extra params!", true);
                Thread.Sleep(1000);
                return;
            }


            Random rnd = new Random();
            byte[] colorBytes = { 100, 255 };

            WriteLine("#5===#1 Folder Ignore List#5 ===#1", true);
            foreach (string ign in ignore)
                Console.WriteLine(ign);
            WriteLine("#5=== =========== ===\n#1", true);

            int StatusPos = Console.CursorTop;
            WriteLine("#1Status : #5Scanning Folders...", true);

            Dictionary<string, List<string>> folderData = new Dictionary<string, List<string>>();

            if (Directory.Exists(location))
            {
                foreach (string subDir in Directory.GetDirectories(location, "*", SearchOption.AllDirectories))
                {

                    bool skip = false;

                    foreach (string ign in ignore)
                        if (subDir.Contains(ign))
                        {
                            skip = true;
                            break;
                        }

                    if (skip)
                        continue;

                    foreach (string file in Directory.GetFiles(subDir))
                    {
                        string fileName = Path.GetFileName(file).Replace(".min", "");

                        if (!folderData.ContainsKey(fileName))
                        {
                            folderData.Add(fileName, new List<string>() { Path.GetFullPath(file) });
                        }
                        else
                        {
                            List<string> tmp = folderData[fileName];
                            tmp.Add(Path.GetFullPath(file));
                            folderData[fileName] = tmp;
                        }

                        filePos.Add(Path.GetFullPath(file), new FilePos() { X = Console.CursorLeft, Y = Console.CursorTop });

                        if (disableUI) continue;
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write((char)22);
                    }
                }
            }

            endYPos = Console.CursorTop;

            /* Start Filtering the files */
            Console.SetCursorPosition(0, StatusPos);
            WriteLine("#1Status : #5Filtering files                                            ", true);

            List<string> emptyFiles = new List<string>();
            List<List<string>> generatedFiltered = new List<List<string>>();
            int MAX = 0;
            int TotalTrash = 0;

            foreach (List<string> dfile in folderData.Values)
            {
                // Detect empty files
                foreach (string file in dfile)
                    if (new FileInfo(file).Length < 5)
                        emptyFiles.Add(file);

                if (dfile.Count > 1)
                {
                    generatedFiltered.Add(dfile);
                    TotalTrash += dfile.Count;

                    foreach (string path in dfile)
                    {
                        string filename = Path.GetFileName(path);

                        if (isPathImage(filename))
                            UpdateFileSquare(path, "#6", 35);
                        else
                            UpdateFileSquare(path, "#5", 35);
                    }

                    if (MAX < dfile.Count)
                        MAX = dfile.Count;
                }
            }

            Console.SetCursorPosition(0, StatusPos);
            WriteLine("#1Status : #5Comparing files..                                                ", true);

            List<List<string>> finalFiles = new List<List<string>>();

            foreach (List<string> dfile in generatedFiltered)
            {
                List<string> passCheck = new List<string>();

                foreach (string paths in dfile)
                {
                    FileInfo mainAgainst = new FileInfo(paths);
                    List<string> temp = Extension.Clone<string>(dfile);
                    temp.Remove(paths);

                    foreach (string file in temp)
                    {
                        bool isNotValid = true;

                        if (isPathImage(file))
                        {
                            Bitmap originalImage = new Bitmap(paths);
                            Bitmap compareImage = new Bitmap(file);

                            if (CompareImages(originalImage, compareImage))
                            {
                                UpdateFileSquare(file, "#4", 35);
                                isNotValid = false;
                            }

                            originalImage.Dispose();
                            compareImage.Dispose();
                        }
                        else
                        {
                            FileInfo compareAgainst = new FileInfo(file);

                            if (FilesAreEqual_Hash(mainAgainst, compareAgainst))
                            {
                                UpdateFileSquare(file, "#4", 35);
                                isNotValid = false;
                            }

                        }

                        if (isNotValid)
                        {
                            UpdateFileSquare(file, "#2", 22);
                            continue;
                        }
                        else
                            if (!passCheck.Contains(file))
                            {
                                passCheck.Add(file);
                                passCheck.Add(paths);
                            }

                    }

                    if (passCheck.Count > 0 && passCheck[passCheck.Count - 1] != "END")
                        passCheck.Add("END");

                }

                if (passCheck.Count > 0)
                    finalFiles.Add(passCheck);
            }

            /* Cleanup the compared data */
            Console.SetCursorPosition(0, StatusPos);
            WriteLine("#1Status : #5Cleaning up data...                                        ", true);

            List<DupedFile> finalData = new List<DupedFile>();

            foreach (List<string> pachk in finalFiles)
            {
                DupedFile tmpFile = null;

                foreach (string fpat in pachk)
                {
                    if (fpat != "END")
                    {
                        if (tmpFile == null)
                        {
                            tmpFile = new DupedFile();
                            tmpFile.color = Color.FromArgb((byte)rnd.Next(colorBytes[0], colorBytes[1]),
                                    (byte)rnd.Next(colorBytes[0], colorBytes[1]),
                                    (byte)rnd.Next(colorBytes[0], colorBytes[1]));
                            tmpFile.path = new List<string>();
                        }

                        UpdateFileSquare(fpat, "#0", 35);
                        tmpFile.path.Add(fpat);
                    }
                    else if (tmpFile != null)
                    {
                        finalData.Add(tmpFile);
                        tmpFile = null;
                    }
                }
            }

            /* Prepare the empty file report */
            DupedFile emptyFileDp = new DupedFile()
            {
                color = Color.FromArgb(255, 0, 0),
                path = new List<string>(),
                CH = 19
            };

            emptyFileDp.path.Add("===== EMPTY FILES =====");
            foreach (string file in emptyFiles)
                emptyFileDp.path.Add(file);
            emptyFileDp.path.Add("===== GENERATED BY FindTrashFiles =====");

            // Add it to the report at the final position
            finalData.Add(emptyFileDp);

            if (serverMode)
            {
                Console.SetCursorPosition(0, StatusPos);
                WriteLine("#1Status : #5Server Mode enabled                                                ", true);
                // TODO : return jenkis format

                StreamWriter writer = File.CreateText(output + "\\duped_output.txt");
                foreach (DupedFile dfile in finalData)
                    foreach (string file in dfile.path)
                        writer.WriteLine(file);

                writer.Flush();
                writer.Close();

                return;
            }


            Console.SetCursorPosition(0, StatusPos);
            WriteLine("#1Status : #5Generating Excel report..                                                ", true);

            FileInfo newFile = new FileInfo(output + "\\duped_output.xlsx");

            try
            {
                if (File.Exists(output + "\\duped_output.xlsx"))
                    File.Delete(output + "\\duped_output.xlsx");
            }
            catch
            {
                Console.WriteLine("Output file is open, close it before running the program");
                Thread.Sleep(1000);
                return;
            }

            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Trash Files");

            Dictionary<string, int> formatCounter = new Dictionary<string, int>();
            // Store found formats

            int I = 1;
            foreach (DupedFile dfile in finalData)
            {

                ws.Cell(I, 1).Style.Border.TopBorder = XLBorderStyleValues.Medium;
                ws.Cell(I, 2).Style.Border.TopBorder = XLBorderStyleValues.Medium;

                foreach (string file in dfile.path)
                {

                    string format = Path.GetExtension(file).ToUpper();

                    if (format != "")
                        if (formatCounter.ContainsKey(format))
                            formatCounter[format] += 1;
                        else
                            formatCounter.Add(format, 1);

                    ws.Cell(I, 1).Value = file;
                    ws.Cell(I, 2).Value = format;

                    if (!file.Contains("==")) // 2lazy
                        ws.Cell(I, 1).Hyperlink = new XLHyperlink("file://" + file);

                    ws.Cell(I, 1).Style.Fill.PatternType = XLFillPatternValues.Solid;
                    ws.Cell(I, 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(dfile.color.A,dfile.color.R, dfile.color.G, dfile.color.B));

                    ws.Cell(I, 2).Style.Border.RightBorder = XLBorderStyleValues.Medium;
                    ws.Cell(I, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                    if (dfile.CH == 0)
                        UpdateFileSquare(file, "#7", -1);
                    else if (dfile.CH == 19)
                        UpdateFileSquare(file, "#5", dfile.CH);

                    I++;
                }

                ws.Cell(dfile.path.Count, 1).Style.Border.TopBorder = XLBorderStyleValues.Medium;
                ws.Cell(dfile.path.Count, 2).Style.Border.TopBorder = XLBorderStyleValues.Medium;

            }

            ws.Columns().AdjustToContents();
            wb.SaveAs(output + "\\duped_output.xlsx");
            wb.Dispose();

            Console.SetCursorPosition(0, endYPos + 3);

            WriteLine("#5===#1 Filter Report#5 ===#1", true);
            WriteLine("#1Total duplicated trash : #0" + TotalTrash, true);
            WriteLine("#1Total empty Files : #5" + emptyFiles.Count, true);
            WriteLine("#1Max duplicated file : #5" + MAX, true);
            WriteLine("#5=== ================= ===\n#1", true);

            WriteLine("#5=== BAD CODE ===\n#1", true);

            WriteLine("#5===#1 Files Format Count#5 ===#1", true);
            foreach (var form in formatCounter)
                WriteLine("#1" + form.Key + " : #5" + form.Value, true);
            WriteLine("#5=== ================== ===\n#1", true);

            WriteLine("#5===#1 Final Report#5 ===#1", true);
            WriteLine("#1Final Trash : #0" + I, true);
            WriteLine("#5=== ================== ===#1", true);

            WriteLine("#1\nDone! Press any key to continue.", true);

            Console.SetCursorPosition(0, StatusPos);
            WriteLine("#1Status : #7Done!                                                ", true);

            Console.ReadKey();

        }

        public static void WriteLine(string Msg, bool newLine)
        {
            Console.ForegroundColor = ConsoleColor.White;
            string[] SplitMsg = Msg.Split('#');

            foreach (string str in SplitMsg)
            {
                if (str.StartsWith("1"))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(str.Remove(0, 1));
                }
                else if (str.StartsWith("2"))
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(str.Remove(0, 1));
                }
                else if (str.StartsWith("3"))
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(str.Remove(0, 1));
                }
                else if (str.StartsWith("4"))
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.Write(str.Remove(0, 1));
                }
                else if (str.StartsWith("5"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(str.Remove(0, 1));
                }
                else if (str.StartsWith("6"))
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(str.Remove(0, 1));
                }
                else if (str.StartsWith("7"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(str.Remove(0, 1));
                }
                else if (str.StartsWith("8"))
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(str.Remove(0, 1));
                }
                else if (str.StartsWith("9"))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(str.Remove(0, 1));
                }
                else if (str.StartsWith("0"))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(str.Remove(0, 1));
                }
                else if (str.StartsWith("-1"))
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.Write(str.Remove(0, 2));
                }
                else if (str.StartsWith("-2"))
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(str.Remove(0, 2));
                }


            }

            if (newLine)
                Console.Write('\n');
        }
    }

    public static class Extension
    {
        public static List<T> Clone<T>(this List<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }
}