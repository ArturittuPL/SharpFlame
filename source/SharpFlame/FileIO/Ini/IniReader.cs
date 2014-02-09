using NLog;
using System;
using System.IO;
using Microsoft.VisualBasic;
using SharpFlame.Collections;

namespace SharpFlame.FileIO.Ini
{
    public class IniReader
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public SimpleList<Section> Sections = new SimpleList<Section>();
        public Section RootSection;

        public void CreateSection(string Name)
        {
            Section newSection = new Section();
            newSection.Name = Name;

            Sections.Add(newSection);
        }

        public clsResult ReadFile(StreamReader File)
        {
            clsResult ReturnResult = new clsResult("Reading INI", false);
            logger.Debug ("Reading INI.");

            int InvalidLineCount = 0;
            int CurrentEntryNum = -1;
            string LineText = null;
            int A = 0;
            string SectionName = "";

            RootSection = new Section();

            do
            {
                LineText = File.ReadLine();
                if ( LineText == null )
                {
                    break;
                }
                LineText = LineText.Trim();
                A = LineText.IndexOf('#');
                if ( A >= 0 )
                {
                    LineText = Strings.Left(LineText, A).Trim();
                }
                if ( LineText.Length >= 2 )
                {
                    if ( LineText[0] == '[' )
                    {
                        if ( LineText[LineText.Length - 1] == ']' )
                        {
                            SectionName = LineText.Substring(1, LineText.Length - 2);
                            for ( A = 0; A <= Sections.Count - 1; A++ )
                            {
                                if ( Sections[A].Name == SectionName )
                                {
                                    break;
                                }
                            }
                            CurrentEntryNum = A;
                            if ( CurrentEntryNum == Sections.Count )
                            {
                                CreateSection(SectionName);
                            }
                        }
                        else
                        {
                            InvalidLineCount++;
                        }
                    }
                    else if ( CurrentEntryNum >= 0 )
                    {
                        A = LineText.IndexOf('=');
                        if ( A >= 0 )
                        {
                            Sections[CurrentEntryNum].CreateProperty(LineText.Substring(0, A).ToLower().Trim(),
                                LineText.Substring(A + 1, LineText.Length - A - 1).Trim());
                        }
                        else
                        {
                            InvalidLineCount++;
                        }
                    }
                    else
                    {
                        A = LineText.IndexOf('=');
                        if ( A >= 0 )
                        {
                            RootSection.CreateProperty(LineText.Substring(0, A).ToLower().Trim(), LineText.Substring(A + 1, LineText.Length - A - 1).Trim());
                        }
                        else
                        {
                            InvalidLineCount++;
                        }
                    }
                }
                else if ( LineText.Length > 0 )
                {
                    InvalidLineCount++;
                }
            } while ( true );

            Sections.RemoveBuffer();

            if ( InvalidLineCount > 0 )
            {
                ReturnResult.WarningAdd("There were " + Convert.ToString(InvalidLineCount) + " invalid lines that were ignored.");
            }

            return ReturnResult;
        }

        public clsResult Translate(SectionTranslator Translator)
        {
            clsResult ReturnResult = new clsResult("Translating INI", false);
            logger.Debug ("Translating INI");

            int A = 0;
            ErrorCount ErrorCount = new ErrorCount();

            ErrorCount.NameWarningCountMax = 16;
            ErrorCount.ValueWarningCountMax = 16;

            for ( A = 0; A <= Sections.Count - 1; A++ )
            {
                ReturnResult.Add(Sections[A].Translate(A, Translator, ErrorCount));
            }

            if ( ErrorCount.NameErrorCount > ErrorCount.NameWarningCountMax )
            {
                ReturnResult.WarningAdd("There were " + Convert.ToString(ErrorCount.NameErrorCount) + " unknown property names that were ignored.");
            }
            if ( ErrorCount.ValueErrorCount > ErrorCount.ValueWarningCountMax )
            {
                ReturnResult.WarningAdd("There were " + Convert.ToString(ErrorCount.ValueErrorCount) + " invalid values that were ignored.");
            }

            return ReturnResult;
        }
    }
}