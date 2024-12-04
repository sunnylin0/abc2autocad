using System;
using System.Collections.Generic;
using System.Text;

namespace autocad_part2
{
    partial class Abc
    {

        static dynamic user = new System.Dynamic.ExpandoObject();

        // constants
        //static readonly abc2svg.C C = new abc2svg.C;

        // mask some unsafe functions
        static Action require = EmptyFunction,
                      system = EmptyFunction,
                      write = EmptyFunction,
                      XMLHttpRequest = EmptyFunction;
        static object std = null,
                      os = null;

        // -- constants --

        // staff system
        const int OPEN_BRACE = 0x01,
                  CLOSE_BRACE = 0x02,
                  OPEN_BRACKET = 0x04,
                  CLOSE_BRACKET = 0x08,
                  OPEN_PARENTH = 0x10,
                  CLOSE_PARENTH = 0x20,
                  STOP_BAR = 0x40,
                  FL_VOICE = 0x80,
                  OPEN_BRACE2 = 0x0100,
                  CLOSE_BRACE2 = 0x0200,
                  OPEN_BRACKET2 = 0x0400,
                  CLOSE_BRACKET2 = 0x0800,
                  MASTER_VOICE = 0x1000;

        const int IN = 96; // resolution 96 PPI
        const double CM = 37.8; // 1 inch = 2.54 centimeter
        double YSTEP; // number of steps for y offsets

        // error texts
        static readonly Dictionary<string, string> errs = new Dictionary<string, string>
    {
        { "bad_char", "Bad character '$1'" },
        { "bad_grace", "Bad character in grace note sequence" },
        { "bad_transp", "Bad transpose value" },
        { "bad_val", "Bad value in $1" },
        { "bar_grace", "Cannot have a bar in grace notes" },
        { "ignored", "$1: inside tune - ignored" },
        { "misplaced", "Misplaced '$1' in %%score" },
        { "must_note", "!$1! must be on a note" },
        { "must_note_rest", "!$1! must be on a note or a rest" },
        { "nonote_vo", "No note in voice overlay" },
        { "not_ascii", "Not an ASCII character" },
        { "not_enough_n", "Not enough notes/rests for %%repeat" },
        { "not_enough_m", "Not enough measures for %%repeat" },
        { "not_enough_p", "Not enough parameters in %%map" },
        { "not_in_tune", "Cannot have '$1' inside a tune" },
        { "notransp", "Cannot transpose with a temperament" }
    };

        static dynamic self = new System.Dynamic.ExpandoObject();
        static GlobalVar glovar = new GlobalVar
        {
            meter = new VoiceMeter
            {
                type = C.METER,
                wmeasure = 1,
                a_meter = new List<Meter>()
            }
        };
        static Information info = new Information();
        static Parse parse = new Parse
        {
            ctx = new Dictionary<string, object>(),
            prefix = "%",
            state = 0,
            ottava = new List<object>(),
            line = new scanBuf()
        };
        static List<object> tunes = new List<object>();
        static object psvg;

        // utilities
        static object clone(object obj, int lvl = 0)
        {
            if (obj == null)
                return obj;

            var tmp = Activator.CreateInstance(obj.GetType());
            foreach (var k in obj.GetType().GetProperties())
            {
                if (/*lvl.HasValue && lvl.Value > 0 &&*/ k.PropertyType.IsClass)
                    k.SetValue(tmp, clone(k.GetValue(obj), lvl - 1));
                else
                    k.SetValue(tmp, k.GetValue(obj));
            }
            return tmp;
        }

        static void errbld(int sev, string txt, string fn = null, int? idx = null)
        {
            int i, j, l=0, c=0;
            string h;
            string outsev;

            if (user.errbld != null)
            {
                switch (sev)
                {
                    case 0: outsev = "warn"; break;
                    case 1: outsev = "error"; break;
                    default: outsev = "fatal"; break;
                }
                user.errbld(outsev, txt, fn, idx);
                return;
            }
            if (idx.HasValue && idx.Value >= 0)
            {
                i = l = 0;
                while (true)
                {
                    j = parse.file.IndexOf('\n', i);
                    if (j < 0 || j > idx.Value)
                        break;
                    l++;
                    i = j + 1;
                }
                c = idx.Value - i;
            }
            h = "";
            if (fn != null)
            {
                h = fn;
                if (l > 0)
                    h += ":" + (l + 1) + ":" + (c + 1);
                h += " ";
            }
            switch (sev)
            {
                case 0: h += "Warning: "; break;
                case 1: h += "Error: "; break;
                default: h += "Internal bug: "; break;
            }
            user.errmsg(h + txt, l, c);
        }

        static void error(int sev, VoiceItem s, string msg, object a1 = null, object a2 = null, object a3 = null, object a4 = null)
        {
            if (sev == 0 && cfmt.quiet)
                return;
            if (s != null)
            {
                if (s.err) // only one error message per symbol
                    return;
                s.err = true;
            }
            if (user.textrans != null)
            {
                var tmp = user.textrans[msg];
                if (tmp != null)
                    msg = tmp;
            }
            if (a1 != null || a2 != null || a3 != null || a4 != null)
            {
                msg = msg.Replace("$1", a1?.ToString())
                         .Replace("$2", a2?.ToString())
                         .Replace("$3", a3?.ToString())
                         .Replace("$4", a4?.ToString());
            }
            if (s != null && s.fname != null)
                errbld(sev, msg, s.fname, s.istart);
            else
                errbld(sev, msg);
        }

        // scanning functions
        public class scanBuf
        {
            public string buffer;
            public int index = 0;

            public char Char()
            {
                return buffer[index];
            }

            public char nextChar()
            {
                return buffer[++index];
            }

            public int getInt()
            {
                int val = 0;
                char c = buffer[index];
                while (c >= '0' && c <= '9')
                {
                    val = val * 10 + (c - '0');
                    c = nextChar();
                }
                return val;
            }
        }

        static void syntax(int sev, string msg, object a1 = null, object a2 = null, object a3 = null, object a4 = null)
        {
            VoiceItem s= new VoiceItem
            {
                fname = parse.fname,
                istart = parse.istart + parse.line.index
            };

            error(sev, s, msg, a1, a2, a3, a4);
        }

        // inject javascript code
        static void js_inject(string js)
        {
            // Note: C# does not support eval like JavaScript. 
            // You might need to use a JavaScript engine like Jint or ClearScript to execute JavaScript code.
            throw new NotImplementedException("JavaScript injection is not supported in C#.");
        }


    }

    //public class GlobalVar
    //{
    //    public Meter meter { get; set; }
    //}

    ////public class Meter
    ////{
    ////    public dynamic type { get; set; }
    ////    public int wmeasure { get; set; }
    ////    public List<object> a_meter { get; set; }
    ////}

    //public class Information { }

    //public class Parse
    //{
    //    public Dictionary<string, object> ctx { get; set; }
    //    public string prefix { get; set; }
    //    public int state { get; set; }
    //    public List<object> ottava { get; set; }
    //    public ScanBuf line { get; set; }
    //}





}
}
