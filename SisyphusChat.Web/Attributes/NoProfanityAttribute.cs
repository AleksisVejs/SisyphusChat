using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SisyphusChat.Web.Attributes
{
    public class NoProfanityAttribute : ValidationAttribute
    {
        private static readonly HashSet<string> ProfanityWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "anal", "anus", "arse", "ass", "ballsack", "balls", "bastard", "bitch", "biatch",
            "bloody", "blowjob", "bollock", "bollok", "boner", "boob", "bugger", "bum", "butt",
            "buttplug", "cock", "coon", "crap", "cunt", "damn", "dick", "dildo", "dyke", "fag",
            "feck", "fellate", "fellatio", "felching", "fuck", "fucking", "fucked", "fuckhead",
            "fudgepacker", "fudger", "faggot", "flange", "goddamn", "hell", "homo", "jerk",
            "jizz", "knobend", "labia", "muff", "nigger", "nigga", "penis", "piss", "poop",
            "prick", "pube", "pussy", "queer", "scrotum", "sex", "shit", "slut", "smegma",
            "spunk", "tit", "tosser", "turd", "twat", "vagina", "wank", "whore", "wtf"
        };

        private static readonly Regex[] ProfanityPatterns = new[]
        {
            new Regex(@"\bn[i1l|]+g+[e3]+r+s?\b", RegexOptions.IgnoreCase),
            new Regex(@"\bn[i1l|]+g+[a@]+s?\b", RegexOptions.IgnoreCase),
            new Regex(@"\bn[i1l|]+g+[e3]+r+\w*\b", RegexOptions.IgnoreCase),
            new Regex(@"\bn[i1l|]+g+[a@]+s?\w*/i\b", RegexOptions.IgnoreCase),
            new Regex(@"\bn[i1l|]+g+[z@s]+\b", RegexOptions.IgnoreCase),
            new Regex(@"\bf+u+c+k+\w*\b", RegexOptions.IgnoreCase),
            new Regex(@"\bb+i+t+c+h+\w*\b", RegexOptions.IgnoreCase),
            new Regex(@"\bc+u+n+t+\w*\b", RegexOptions.IgnoreCase),
            new Regex(@"\bp+u+s+s+y+\w*\b", RegexOptions.IgnoreCase),
        };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            string text = value.ToString()!;
            if (string.IsNullOrWhiteSpace(text)) return ValidationResult.Success;

            var cleanWord = text.Trim();
            if (ProfanityWords.Contains(cleanWord) || ProfanityPatterns.Any(pattern => pattern.IsMatch(cleanWord)))
            {
                return new ValidationResult(ErrorMessage ?? "Username contains inappropriate content.");
            }

            return ValidationResult.Success;
        }
    }
} 