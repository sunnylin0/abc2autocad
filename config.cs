using System;
using System.Collections.Generic;
using System.Text;

namespace autocad_part2
{
 
        public class abc2svg
        {
            public static Abc Abc;
            public static List<object> modules;
            public static List<object> mhooks;
            public static object sheet;

            // global fonts
            public static List<object> font_tb = new List<object>();    // fonts - index = font.fid
            public static Dictionary<string, int> font_st = new Dictionary<string, int>();    // font style => font_tb index for incomplete user fonts

            public static object loadjs;
            public static object printErr;
            public static class C
            {
                public const int BLEN = 1536;

                // symbol types
                public const int BAR = 0;
                public const int CLEF = 1;
                public const int CUSTOS = 2;
                public const int SM = 3;        // sequence marker (transient)
                public const int GRACE = 4;
                public const int KEY = 5;
                public const int METER = 6;
                public const int MREST = 7;
                public const int NOTE = 8;
                public const int PART = 9;
                public const int REST = 10;
                public const int SPACE = 11;
                public const int STAVES = 12;
                public const int STBRK = 13;
                public const int TEMPO = 14;
                public const int BLOCK = 16;
                public const int REMARK = 17;

                // note heads
                public const int FULL = 0;
                public const int EMPTY = 1;
                public const int OVAL = 2;
                public const int OVALBARS = 3;
                public const int SQUARE = 4;

                // position types
                public const int SL_ABOVE = 0x01;        // position (3 bits)
                public const int SL_BELOW = 0x02;
                public const int SL_AUTO = 0x03;
                public const int SL_HIDDEN = 0x04;
                public const int SL_DOTTED = 0x08;    // modifiers
                public const int SL_ALI_MSK = 0x70;    // align
                public const int SL_ALIGN = 0x10;
                public const int SL_CENTER = 0x20;
                public const int SL_CLOSE = 0x40;
            }

            // !! tied to the symbol types in abc2svg.js !!
            public static List<string> sym_name = new List<string> { "bar", "clef", "custos", "smark", "grace",
            "key", "meter", "Zrest", "note", "part",
            "rest", "yspace", "staves", "Break", "tempo",
            "", "block", "remark" };

            // key table - index = number of accidentals + 7
            public static List<int[]> keys = new List<int[]> {
            new int[] {-1, -1, -1, -1, -1, -1, -1},    // 7 flat signs
            new int[] {-1, -1, -1, 0, -1, -1, -1},    // 6 flat signs
            new int[] {0, -1, -1, 0, -1, -1, -1},    // 5 flat signs
            new int[] {0, -1, -1, 0, 0, -1, -1},    // 4 flat signs
            new int[] {0, 0, -1, 0, 0, -1, -1},    // 3 flat signs
            new int[] {0, 0, -1, 0, 0, 0, -1},    // 2 flat signs
            new int[] {0, 0, 0, 0, 0, 0, -1},    // 1 flat signs
            new int[] {0, 0, 0, 0, 0, 0, 0},    // no accidental
            new int[] {0, 0, 0, 1, 0, 0, 0},    // 1 sharp signs
            new int[] {1, 0, 0, 1, 0, 0, 0},    // 2 sharp signs
            new int[] {1, 0, 0, 1, 1, 0, 0},    // 3 sharp signs
            new int[] {1, 1, 0, 1, 1, 0, 0},    // 4 sharp signs
            new int[] {1, 1, 0, 1, 1, 1, 0},    // 5 sharp signs
            new int[] {1, 1, 1, 1, 1, 1, 0},    // 6 sharp signs
            new int[] {1, 1, 1, 1, 1, 1, 1}    // 7 sharp signs
        };

            // base-40 representation of musical pitch
            // (http://www.ccarh.org/publications/reprints/base40/)
            public static int[] p_b40 = new int[] {    // staff pitch to base-40
            //          C  D   E   F   G   A   B
            2, 8, 14, 19, 25, 31, 37 };
            public static int[] b40_p = new int[] {    // base-40 to staff pitch
            //               C         D
            0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1,
            //          E              F                G
            2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4,
            //          A         B
            5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6 };
            public static int[] b40_a = new int[] {    // base-40 to accidental
            //                 C              D
            -2, -1, 0, 1, 2, -3, -2, -1, 0, 1, 2, -3,
            //          E              F                G
            -2, -1, 0, 1, 2, -2, -1, 0, 1, 2, -3, -2, -1, 0, 1, 2, -3,
            //          A              B
            -2, -1, 0, 1, 2, -3, -2, -1, 0, 1, 2 };
            public static int[] b40_m = new int[] {    // base-40 to midi
            //                 C              D
            -2, -1, 0, 1, 2, 0, 0, 1, 2, 3, 4, 0,
            //          E              F                G
            2, 3, 4, 5, 6, 3, 4, 5, 6, 7, 0, 5, 6, 7, 8, 9, 0,
            //          A              B
            7, 8, 9, 10, 11, 0, 9, 10, 11, 12, 13 };
            public static int[] b40Mc = new int[] {    // base-40 major chords
            //                C           D
            36, 1, 2, 3, 8, 2, 2, 7, 8, 13, 14, 2,
            //            37       7           3
            //          E              F                G
            8, 13, 14, 19, 20, 13, 14, 19, 20, 25, 2, 19, 24, 25, 30, 31, 2,
            //                  24            20
            //          A              B
            25, 30, 31, 36, 37, 2, 31, 36, 37, 2, 3 };
            //                   1
            public static int[] b40mc = new int[] {    // base-40 minor chords
            //                C           D
            36, 37, 2, 3, 8, 2, 2, 3, 8, 9, 14, 2,
            //                            13
            //          E              F                G
            8, 13, 14, 19, 20, 13, 14, 19, 20, 25, 2, 19, 20, 25, 26, 31, 2,
            //        9                          30
            //          A              B
            25, 30, 31, 32, 37, 2, 31, 36, 37, 2, 3 };
            //       26    36           32
            public static int[] b40sf = new int[] {    // base-40 interval to key signature
            //                C              D
            -2, -7, 0, 7, 2, 88, 0, -5, 2, -3, 4, 88,
            //          E              F                G
            2, -3, 4, -1, 6, -3, 4, -1, 6, 1, 88, -1, -6, 1, -4, 3, 88,
            //          A              B
            1, -4, 3, -2, 5, 88, 3, -2, 5, 0, 7 };
            public static int[] isb40 = new int[] {    // interval with sharp to base-40 interval
            0, 1, 6, 7, 12, 17, 18, 23, 24, 29, 30, 35 };

            public static int pab40(int p, int a)
            {
                p += 19;                // staff pitch from C-1
                int b40 = ((p / 7) * 40) + p_b40[p % 7];
                if (a != 0 && a != 3)        // if some accidental, but not natural
                    b40 += a;
                return b40;
            }

            public static int b40p(int b)
            {
                return ((b / 40) * 7) + b40_p[b % 40] - 19;
            }

            public static int b40a(int b)
            {
                return b40_a[b % 40];
            }

            public static int b40m(int b)
            {
                return ((b / 40) * 12) + b40_m[b % 40];
            }

            // chord table
            // This table is used in various modules
            // to convert the types of chord symbols to a minimum set.
            // More chord types may be added by the command %%chordalias.
            public static Dictionary<string, string> ch_alias = new Dictionary<string, string> {
            { "maj", "" },
            { "min", "m" },
            { "-", "m" },
            { "°", "dim" },
            { "+", "aug" },
            { "+5", "aug" },
            { "maj7", "M7" },
            { "Δ7", "M7" },
            { "Δ", "M7" },
            { "min7", "m7" },
            { "-7", "m7" },
            { "ø7", "m7b5" },
            { "°7", "dim7" },
            { "min+7", "m+7" },
            { "aug7", "+7" },
            { "7+5", "+7" },
            { "7#5", "+7" },
            { "sus", "sus4" },
            { "7sus", "7sus4" }
        };

            // font weight
            // reference:
            //  https://developer.mozilla.org/en-US/docs/Web/CSS/font-weight
            public static Dictionary<string, int> ft_w = new Dictionary<string, int> {
            { "thin", 100 },
            { "extralight", 200 },
            { "light", 300 },
            { "regular", 400 },
            { "medium", 500 },
            { "semi", 600 },
            { "demi", 600 },
            { "semibold", 600 },
            { "demibold", 600 },
            { "bold", 700 },
            { "extrabold", 800 },
            { "ultrabold", 800 },
            { "black", 900 },
            { "heavy", 900 }
        };

            public static System.Text.RegularExpressions.Regex ft_re = new System.Text.RegularExpressions.Regex(@"-?Thin|-?Extra Light|-?Light|-?Regular|-?Medium|-?[DS]emi|-?[DS]emi[ -]?Bold|-?Bold|-?Extra[ -]?Bold|-?Ultra[ -]?Bold|-?Black|-?Heavy", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            public static string style = @"
.stroke{stroke:currentColor;fill:none}
.bW{stroke:currentColor;fill:none;stroke-width:1}
.bthW{stroke:currentColor;fill:none;stroke-width:3}
.slW{stroke:currentColor;fill:none;stroke-width:.7}
.slthW{stroke:currentColor;fill:none;stroke-width:1.5}
.sW{stroke:currentColor;fill:none;stroke-width:.7}
.box{outline:1px solid black;outline-offset:1px}";

            public static int[] rat(int n, int d)
            {
                int a, t;
                int n0 = 0, d1 = 0, n1 = 1, d0 = 1;
                while (true)
                {
                    if (d == 0)
                        break;
                    t = d;
                    a = n / d;
                    d = n % d;
                    n = t;
                    t = n0 + a * n1;
                    n0 = n1;
                    n1 = t;
                    t = d0 + a * d1;
                    d0 = d1;
                    d1 = t;
                }
                return new int[] { n1, d1 };
            }

            public static int pitcmp(NoteItem n1, NoteItem n2) { return n1.pit - n2.pit; }


        }
    }

