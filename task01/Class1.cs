using System;
namespace task01
{
    public static class StringExtensions
    {
        public static bool IsPalindrome(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;
            string cleanText = "";
            foreach (char c in text)
            {
                if (char.IsLetter(c))
                {
                    cleanText += char.ToLower(c);
                }
            }
            int left = 0;
            int right = cleanText.Length - 1;
            while (left < right)
            {
                if (cleanText[left] != cleanText[right])
                    return false;
                left++;
                right--;
            }
            return true;
        }
    }
}
