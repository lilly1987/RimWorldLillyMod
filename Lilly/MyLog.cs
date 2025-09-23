using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Lilly
{
    public static class MyLog
    {
        //public static void Warning(string text) => Log.Warning($"Lilly - {text}");
        //public static void Error(string text) => Log.Error($"Lilly - {text}");
        //public static void Message(string text) => Log.Message($"Lilly - {text}");

        public static void Warning(string text,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            bool print=true
            )
        {
            if(!print) return;
            string className = System.IO.Path.GetFileNameWithoutExtension(filePath);
            Log.Warning($"Lilly - {className}.{memberName} (Line {lineNumber}) - {text}");
        }

        public static void Error(string text,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            bool print = true)
        {
            if (!print) return;
            string className = System.IO.Path.GetFileNameWithoutExtension(filePath);
            Log.Error($"Lilly - {className}.{memberName} (Line {lineNumber}) - {text}");
        }

        public static void Message(string text,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            bool print = true)
        {
            if (!print) return;
            string className = System.IO.Path.GetFileNameWithoutExtension(filePath);
            Log.Message($"Lilly - {className}.{memberName} (Line {lineNumber}) - {text}");
        }

    }
}
