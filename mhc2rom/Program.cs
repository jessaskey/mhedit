using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using mhedit.GameControllers;
using mhedit.Extensions;
using mhedit.Containers;
using System.Reflection;

namespace mhc2rom
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Usage: ");
                sb.Append(Path.GetFileName(Assembly.GetExecutingAssembly().CodeBase));
                sb.Append(" <path to .mhc file> <SourceROMFolder> <DestROMFolder>");
                Console.Out.WriteLine(sb.ToString());
            }
            else
            {
                string mazeCollectionFile = args[0];
                string sourceROMPath = args[1];
                string destROMPath = args[2];

                if (!File.Exists(mazeCollectionFile))
                {
                    Console.Out.WriteLine("Maze Collection file does not exist or could not be found: " + mazeCollectionFile);
                }
                else
                {
                    if (!Directory.Exists(sourceROMPath))
                    {
                        Console.Out.WriteLine("Source ROM Path does not exist : " + sourceROMPath);
                    }
                    else
                    {
                        //Destination ROMPath does *not* have to exist because it will just be created (if possible)
                        MajorHavocPromisedEnd mhpe = new MajorHavocPromisedEnd();
                        bool loadSuccess = mhpe.LoadTemplate(sourceROMPath);
                        if (loadSuccess)
                        {
                            MazeCollection mazeCollection = mazeCollectionFile.ExpandAndDeserialize<MazeCollection>();
                            if (!String.IsNullOrEmpty(SerializationExtensions.LastError))
                            {
                                Console.Out.WriteLine("Error Deserializing Maze Collection File: " + SerializationExtensions.LastError);
                            }
                            else
                            {
                                bool encodeSuccess = mhpe.EncodeObjects(mazeCollection);
                                if (encodeSuccess)
                                {
                                    bool writeSuccess = mhpe.WriteFiles(destROMPath);
                                    if (writeSuccess)
                                    {
                                        Console.Out.WriteLine("Sucessfully merged .mhc file into ROM images");
                                    }
                                    else
                                    {
                                        Console.Out.WriteLine("Error writing the destination ROM images: " + mhpe.LastError);
                                    }
                                }
                                else
                                {
                                    Console.Out.WriteLine("Error Encoding Maze Objects: " + mhpe.LastError);
                                }
                            }
                        }
                        else
                        {
                            Console.Out.WriteLine("Error loading the Source ROM images: " + mhpe.LastError);
                        }
                    }
                    
                }
            }
            Console.Out.WriteLine("Press any key...");
            Console.ReadKey();
        }


    }
}
