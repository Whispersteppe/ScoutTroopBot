using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutBot.Modules;

public class SubstitutionHelper : Dictionary<string, string>
{
    /// <summary>
    /// Replaces placeholders in the input string with corresponding values from the substitutions dictionary.
    /// </summary>
    /// <remarks>Placeholders in the input string are identified by enclosing keys in curly braces (e.g.,
    /// <c>{key}</c>). The method performs a case-sensitive replacement for each key in the dictionary.</remarks>
    /// <param name="input">The input string containing placeholders in the format <c>{key}</c>.</param>
    /// <param name="substitutions">A dictionary where each key represents a placeholder (without braces) and the value is the replacement string.</param>
    /// <returns>A new string with all placeholders replaced by their corresponding values. If a placeholder does not have a
    /// matching key in the dictionary, it remains unchanged.</returns>
    public string DoSubstitutions(string input)
    {
        var output = input;
        foreach (var kvp in this)
        {
            string key = "{" + kvp.Key + "}";
            output = output.Replace(key, kvp.Value);
        }
        return output;
    }
}
