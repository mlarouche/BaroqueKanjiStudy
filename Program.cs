using NDesk.Options.Fork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BaroqueKanjiStudy
{
    class Program
    {
        static void Main(string[] args)
        {
            int? grade = null;
            int? jlpt = null;
            string pageTitle = null;
            string filePath = null;
            string outputFile = null;

            var optionSet = new OptionSet()
            {
                {"g|grade=", "Which grade do you want to study", v => grade = int.Parse(v) },
                {"j|jlpt=", "Which JLPT do you want to study", v => jlpt = int.Parse(v) },
                {"f|file=", "File with kanjis in it", v => filePath = v },
                {"t|title=", "Custom Page Title", v => pageTitle = v },
                {"o|output=", "Output file name", v => outputFile = v }
            };

            optionSet.Parse(args);

            Dictionary<char, KanjiEntry> entries = new Dictionary<char, KanjiEntry>();

            using (StreamReader kanjiDict = new StreamReader("kanjidic2.xml", Encoding.UTF8))
            {
                var document = XDocument.Load(kanjiDict);

                foreach(var character in document.Descendants("character"))
                {
                    KanjiEntry entry = new KanjiEntry();
                    entry.Kanji = character.Descendants("literal").First().Value[0];

                    var gradeElement = character.Descendants("grade").FirstOrDefault();
                    if (gradeElement != null)
                    {
                        entry.Grade = int.Parse(gradeElement.Value);
                    }

                    var jltpElement = character.Descendants("jlpt").FirstOrDefault();
                    if (jltpElement != null)
                    {
                        entry.JLPT = int.Parse(jltpElement.Value);
                    }

                    foreach(var meaningElement in character.Descendants("meaning"))
                    {
                        if (!meaningElement.HasAttributes)
                        {
                            entry.MeaningList.Add(meaningElement.Value);
                        }
                    }

                    foreach(var readingElement in character.Descendants("reading"))
                    {
                        var rTypeAttribute = readingElement.Attribute("r_type");
                        if (rTypeAttribute != null && rTypeAttribute.Value == "ja_on")
                        {
                            entry.OnyomiList.Add(readingElement.Value);
                        }

                        if (rTypeAttribute != null && rTypeAttribute.Value == "ja_kun")
                        {
                            entry.KunyomiList.Add(readingElement.Value);
                        }
                    }

                    if (!entries.ContainsKey(entry.Kanji))
                    {
                        entries.Add(entry.Kanji, entry);
                    }
                }
            }

            List<KanjiEntry> list = new List<KanjiEntry>();

            string defaultTitle = "Kanji List";
            string defaultFilePath = @"out\kanjilist.html";

            if (!string.IsNullOrEmpty(filePath))
            {
                HashSet<char> fileChars = new HashSet<char>();

                using (var fileReader = new StreamReader(filePath, Encoding.UTF8))
                {
                    while (!fileReader.EndOfStream)
                    {
                        string line = fileReader.ReadLine();

                        char kanji = line[0];

                        if (!fileChars.Contains(kanji))
                        {
                            fileChars.Add(kanji);

                            KanjiEntry result = null;
                            if (entries.TryGetValue(kanji, out result))
                            {
                                list.Add(result);
                            }
                        }
                    }
                }
            }
            else
            {
                if (grade.HasValue)
                {
                    defaultTitle = $"Grade {grade.Value} 漢字";
                    defaultFilePath = $@"out\grade{grade.Value}.html";

                    list.AddRange(entries.Values.Where(x => x.Grade == grade.Value));
                }
                else if (jlpt.HasValue)
                {
                    defaultTitle = $"JLPT N{jlpt.Value} 漢字";
                    defaultFilePath = $@"out\jlpt{jlpt.Value}.html";

                    list.AddRange(entries.Values.Where(x => x.JLPT == jlpt.Value));
                }
            }

            KanjiHTMLPage htmlPage = new KanjiHTMLPage();
            htmlPage.PageTitle = !string.IsNullOrEmpty(pageTitle) ? pageTitle : defaultTitle;
            htmlPage.Kanjis = list;

            using (StreamWriter writer = new StreamWriter(!string.IsNullOrEmpty(outputFile) ? outputFile : defaultFilePath, false, Encoding.UTF8))
            {
                writer.Write(htmlPage.TransformText());
            }
        }
    }
}
