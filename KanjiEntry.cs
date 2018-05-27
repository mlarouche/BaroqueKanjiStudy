using System.Collections.Generic;

namespace BaroqueKanjiStudy
{
    public class KanjiEntry
    {
        private List<string> _meanings = new List<string>();
        private List<string> _onyomis = new List<string>();
        private List<string> _kunyomis = new List<string>();

        public char Kanji { get; set; }
        public int Grade { get; set; }
        public int JLPT { get; set; }

        public int Ranking { get; set; }

        public string SVGFileName
        {
            get
            {
                return $"{char.ConvertToUtf32(Kanji.ToString(), 0):x5}.svg";
            }
        }

        public List<string> MeaningList
        {
            get
            {
                return _meanings;
            }
        }

        public List<string> OnyomiList
        {
            get
            {
                return _onyomis;
            }
        }

        public List<string> KunyomiList
        {
            get
            {
                return _kunyomis;
            }
        }

        public string Meanings
        {
            get
            {
                return string.Join(", ", MeaningList);
            }
        }

        public string Onyomis
        {
            get
            {
                return string.Join(", ", OnyomiList);
            }
        }

        public string Kunyomis
        {
            get
            {
                return string.Join(", ", KunyomiList);
            }
        }
    }
}
