using System.Text.RegularExpressions;

namespace SV20T1080048.Web.AddCodes
{
    public static class ProfanityFilter
    {
        private static readonly List<string> Profanities = new List<string>
        {
            "óc chó", "súc vật", "Địt mẹ mày", "Chó", "cặt", "lồn", "lừa đảo"  // thêm các từ cần lọc tại đây
        };

        public static string FilterProfanity(string input)
        {
            foreach (var profanity in Profanities)
            {
                var regex = new Regex(profanity, RegexOptions.IgnoreCase);
                input = regex.Replace(input, new string('*', profanity.Length));
            }

            return input;
        }
    }
}
