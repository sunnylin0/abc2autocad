using System;
using System.Collections.Generic;
using System.Text;





namespace autocad_part2
{
    partial class Abc
    {


        public static Dictionary<string, double> font_scale_tb = new Dictionary<string, double>()
        {
            {"serif", 1},
            {"serifBold", 1},
            {"sans-serif", 1},
            {"sans-serifBold", 1},
            {"Palatino", 1.1},
            {"monospace", 1}
        };

        public string txt_ff = "text,serif";

        public static Dictionary<string, bool> fmt_lock = new Dictionary<string, bool>();

        public static FormationInfo cfmt = new FormationInfo
        {
            abc_version = "1",        // default: old version
            annotationfont = new Font { name = "text,sans-serif", size = 12 },
            aligncomposer = 1,
            beamslope = 0.4,            // max slope of a beam
                                        //botmargin = 0.7 * IN,     // != 1.8 * CM,
            bardef = new Dictionary<string, string>
    {
        { "[", "" },            // invisible
        { "[]", "" },
        { "|:", "[|:" },
        { "|::", "[|::" },
        { "|:::", "[|:::" },
        { ":|", ":|]" },
        { "::|", "::|]" },
        { ":::|", ":::|]" },
        { "::", ":][:" }
    },
            breaklimit = 0.7,
            breakoneoln = true,
            cancelkey = true,
            composerfont = new Font { name = txt_ff, style = "italic", size = 14 },
            composerspace = 6,
            //contbarnb = false,
            decoerr = true,
            dynalign = true,
            footerfont = new Font { name = txt_ff, size = 16 },
            fullsvg = "",
            gchordfont = new Font { name = "text,sans-serif", size = 12 },
            gracespace = new double[] { 6, 8, 11 },    // left, inside, right
            graceslurs = true,
            headerfont = new Font { name = txt_ff, size = 16 },
            historyfont = new Font { name = txt_ff, size = 16 },
            hyphencont = true,
            indent = 0,
            infofont = new Font { name = txt_ff, style = "italic", size = 14 },
            infoname = "R \"Rhythm: \"\nB \"Book: \"\nS \"Source: \"\nD \"Discography: \"\nN \"Notes: \"\nZ \"Transcription: \"\nH \"History: \"",
            infospace = 0,
            keywarn = true,
            leftmargin = 1.4 * CM,
            lineskipfac = 1.1,
            linewarn = true,
            maxshrink = 0.65,        // nice scores
            maxstaffsep = 2000,
            maxsysstaffsep = 2000,
            measrepnb = 1,
            measurefont = new Font { name = txt_ff, style = "italic", size = 10 },
            measurenb = -1,
            musicfont = new Font { name = "music", src = musicfont, size = 24 },
            musicspace = 6,
            //notespacingfactor = "1.3, 38",
            partsfont = new Font { name = txt_ff, size = 15 },
            parskipfac = 0.4,
            partsspace = 8,
            //pageheight = 29.7 * CM,
            pagewidth = 21 * CM,
            propagate_accidentals = "o",        // octave
            printmargin = 0,
            rightmargin = 1.4 * CM,
            rbmax = 4,
            rbmin = 2,
            repeatfont = new Font { name = txt_ff, size = 9 },
            scale = 1,
            slurheight = 1.0,
            spatab = new double[]        // spacing table (see "notespacingfactor" and set_space())
        {
        10.2, 13.3, 17.3, 22.48, 29.2,
        38,
        49.4, 64.2, 83.5, 108.5
        },
            staffsep = 46,
            stemheight = 21,            // one octave
            stretchlast = 0.25,
            stretchstaff = true,
            subtitlefont = new Font { name = txt_ff, size = 16 },
            subtitlespace = 3,
            sysstaffsep = 34,
            systnames = -1,
            tempofont = new Font { name = txt_ff, weight = "bold", size = 12 },
            textfont = new Font { name = txt_ff, size = 16 },
            //textoption = undefined,
            textspace = 14,
            tieheight = 1.0,
            titlefont = new Font { name = txt_ff, size = 20 },
            //titleleft = false,
            titlespace = 6,
            titletrim = true,
            //transp = 0,            // global transpose
            //topmargin = 0.7 * IN,
            topspace = 22,
            tuplets = new int[] { 0, 0, 0, 0 },
            tupletfont = new Font { name = txt_ff, style = "italic", size = 12 },
            vocalfont = new Font { name = txt_ff, weight = "bold", size = 13 },
            vocalspace = 10,
            voicefont = new Font { name = txt_ff, weight = "bold", size = 13 },
            //voicescale = 1,
            writefields = "CMOPQsTWw",
            wordsfont = new Font { name = txt_ff, size = 16 },
            wordsspace = 5,
            writeout_accidentals = "n"
        };

        // parameters that are used in the symbols
        public static Dictionary<string, bool> sfmt = new Dictionary<string, bool> {
            {"bardef", true},
            {"barsperstaff", true},
            {"beamslope", true},
            {"breaklimit", true},
            {"bstemdown", true},
            {"cancelkey", true},
            {"dynalign", true},
            {"flatbeams", true},
            {"gracespace", true},
            {"hyphencont", true},
            {"keywarn", true},
            {"maxshrink", true},
            {"maxstaffsep", true},
            {"measrepnb", true},
            {"rbmax", true},
            {"rbmin", true},
            {"shiftunison", true},
            {"slurheight", true},
            {"squarebreve", true},
            {"staffsep", true},
            {"systnames", true},
            {"stemheight", true},
            {"stretchlast", true},
            {"stretchstaff", true},
            {"tieheight", true},
            {"timewarn", true},
            {"trimsvg", true},
            {"vocalspace", true }
            };


        // get the text option
        public static Dictionary<string, string> textopt = new Dictionary<string, string>()
        {
            {"align", "j"},
            {"center", "c"},
            {"fill", "f"},
            {"justify", "j"},
            {"obeylines", "l"},
            {"ragged", "f"},
            {"right", "r"},
            {"skip", "s"},
            {"0", "l"},
            {"1", "j"},
            {"2", "f"},
            {"3", "c"},
            {"4", "s"},
            {"5", "r"}
        };

        function get_bool(param)
        {
            return !param || !/ ^(0 | n | f) / i.test(param) // accept void as true !
}
        // %%font <font> [<encoding>] [<scale>]
        public static void get_font_scale(string param)
        {
            var a = param.Split(' ');
            if (a.Length <= 1)
                return;
            double scale = double.Parse(a[a.Length - 1]);
            if (double.IsNaN(scale) || scale <= 0.5)
            {
                syntax(1, "Bad scale value in %%font");
                return;
            }
            font_scale_tb[a[0]] = scale;
        }

        // set the width factor of a font
        public static void set_font_fac(Dictionary<string, object> font)
        {
            double scale = font_scale_tb[font.ContainsKey("fname") ? (string)font["fname"] : (string)font["name"]];
            if (scale == 0)
                scale = 1.1;
            font["swfac"] = (double)font["size"] * scale;
        }

        // %%xxxfont fontname|* [encoding] [size|*]
        public static void param_set_font(string xxxfont, string p)
        {
            Dictionary<string, object> font = new Dictionary<string, object>();
            var a = p.Split(' ');
            if (a.Length <= 1)
                return;
            if (a.Contains("nobox"))
            {
                font["box"] = false;
                font["pad"] = 0;
                p = p.Replace("nobox", "");
            }
            else if (a.Contains("box"))
            {
                font["box"] = true;
                font["pad"] = 2.5;
                p = p.Replace("box", "");
            }
            var padding = a.FirstOrDefault(x => x.StartsWith("padding="));
            if (padding != null)
            {
                font["pad"] = double.Parse(padding.Substring(8));
                p = p.Replace(padding, "");
            }
            var fontClass = a.FirstOrDefault(x => x.StartsWith("class="));
            if (fontClass != null)
            {
                font["class"] = fontClass.Substring(6);
                p = p.Replace(fontClass, "");
            }
            var wadj = a.FirstOrDefault(x => x.StartsWith("wadj="));
            if (wadj != null)
            {
                switch (wadj.Substring(5))
                {
                    case "none":
                        font["wadj"] = "";
                        break;
                    case "space":
                        font["wadj"] = "spacing";
                        break;
                    case "glyph":
                        font["wadj"] = "spacingAndGlyphs";
                        break;
                    default:
                        syntax(1, errs.bad_val, "%%" + xxxfont);
                        break;
                }
                p = p.Replace(wadj, "");
            }
            if (p[0] == 'u' && p.StartsWith("url(") || p[0] == 'l' && p.StartsWith("local("))
            {
                int n = p.IndexOf(')', 1);
                if (n < 0)
                {
                    syntax(1, "No end of url in font family");
                    return;
                }
                font["src"] = p.Substring(0, n + 1);
                font["fid"] = abc2svg.font_tb.Count;
                abc2svg.font_tb.Add(font);
                font["name"] = "ft" + font["fid"];
                p = p.Replace((string)font["src"], "");
            }
            if (p[0] == '"')
            {
                int n = p.IndexOf('"', 1);
                if (n < 0)
                {
                    syntax(1, "No end of string in font family");
                    return;
                }
                p = p.Substring(1, n - 1);
            }
            p = p.Trim();
            switch (p)
            {
                case "":
                case "*":
                    p = "";
                    break;
                case "Times-Roman":
                case "Times":
                    p = "serif";
                    break;
                case "Helvetica":
                    p = "sans-serif";
                    break;
                case "Courier":
                    p = "monospace";
                    break;
                case "music":
                    p = cfmt["musicfont"]["name"];
                    break;
                default:
                    if (p.IndexOf("Fig") > 0)
                        font["figb"] = true;
                    break;
            }
            if (!string.IsNullOrEmpty(p))
                font["name"] = p;
            if (font.ContainsKey("size"))
                set_font_fac(font);
            if (!font.ContainsKey("name") || !font.ContainsKey("size"))
            {
                var ft2 = (Dictionary<string, object>)cfmt[xxxfont];
                foreach (var k in ft2.Keys)
                {
                    if (!font.ContainsKey(k))
                    {
                        switch (k)
                        {
                            case "fid":
                            case "used":
                            case "src":
                                break;
                            case "style":
                            case "weight":
                                if (font.ContainsKey("normal"))
                                    break;
                                goto default;
                            default:
                                font[k] = ft2[k];
                                break;
                        }
                    }
                }
                if (!font.ContainsKey("swfac"))
                    set_font_fac(font);
            }
            if (!font.ContainsKey("pad"))
                font["pad"] = 0;
            font["fname"] = font.ContainsKey("name") ? (string)font["name"] : null;
            if (font.ContainsKey("weight") && (int)font["weight"] >= 700)
                font["fname"] += "Bold";
            cfmt[xxxfont] = font;
        }

        // get a length with a unit - return the number of pixels
        public static double get_unit(string param)
        {
            var v = System.Text.RegularExpressions.Regex.Match(param.ToLower(), @"(-?[\d.]+)(.*)");
            if (!v.Success)
                return double.NaN;
            double.TryParse(v.Groups[1].Value, out double result);
            switch (v.Groups[2].Value)
            {
                case "cm":
                    return result * CM;
                case "in":
                    return result * IN;
                case "pt":
                    return result / .75;
                case "px":
                case "":
                    return result;
            }
            return double.NaN;
        }

        // set the infoname
        public static void set_infoname(string param)
        {
            var tmp = cfmt["infoname"].ToString().Split('\n');
            var letter = param[0];
            for (int i = 0; i < tmp.Length; i++)
            {
                var infoname = tmp[i];
                if (infoname[0] != letter)
                    continue;
                if (param.Length == 1)
                    tmp = tmp.Where((source, index) => index != i).ToArray();
                else
                    tmp[i] = param;
                cfmt["infoname"] = string.Join('\n', tmp);
                return;
            }
            cfmt["infoname"] += "\n" + param;
        }

        public static string get_textopt(string v)
        {
            int i = v.IndexOf(' ');
            if (i > 0)
                v = v.Substring(0, i);
            return textopt[v];
        }


        /*************************************/

        var font_scale_tb = new Dictionary<string, double>()
            {
                {"serif", 1},
                {"serifBold", 1},
                {"sans-serif", 1},
                {"sans-serifBold", 1},
                {"Palatino", 1.1},
                {"monospace", 1}
            };

        var txt_ff = "text,serif";
        var fmt_lock = new Dictionary<string, object>();

        FormationInfo cfmt;
        var sfmt = new Dictionary<string, bool>()
            {
                {"bardef", true},
                {"barsperstaff", true}
            };

        void set_pos(string k, string v)
        {
            k = k.Substring(0, 3);
            if (k == "ste")
                k = "stm";
            set_v_param("pos", '"' + k + ' ' + v + '"');
        }

        void set_writefields(string parm)
        {
            var a = parm.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var cfmt_writefields = cfmt.writefields.ToCharArray();

            if (get_bool(a[1]))
            {
                foreach (var c in a[0])
                {
                    if (!cfmt_writefields.Contains(c))
                        cfmt_writefields += c;
                }
            }
            else
            {
                foreach (var c in a[0])
                {
                    if (cfmt_writefields.Contains(c))
                        cfmt_writefields = cfmt_writefields.Replace(c.ToString(), "");
                }
            }

            cfmt.writefields = new string(cfmt_writefields);
        }

        void set_v_param(string k, string v)
        {
            k = k + "=" + v;
            if (parse.state < 3)
                memo_kv_parm(curvoice != null ? curvoice.id : "*", k);
            else if (curvoice != null)
                set_kv_parm(k);
            else
                memo_kv_parm("*", k);
        }

        void set_page()
        {
            if (!img.chg)
                return;
            img.chg = false;
            img.lm = cfmt.leftmargin - cfmt.printmargin;
            if (img.lm < 0)
                img.lm = 0;
            img.rm = cfmt.rightmargin - cfmt.printmargin;
            if (img.rm < 0)
                img.rm = 0;
            img.width = cfmt.pagewidth - 2 * cfmt.printmargin;

            if (img.width - img.lm - img.rm < 100)
            {
                error(0, null, "Bad staff width");
                img.width = img.lm + img.rm + 150;
            }

            img.lw = (img.width - img.lm - img.rm - 2) / cfmt.scale;

            set_posx();
        }

        void set_format(string cmd, string param)
        {
            double f, f2;
            int v, i;

            if (Regex.IsMatch(cmd, ".+font(-[\\d])?$"))
            {
                if (cmd == "soundfont")
                    cfmt.soundfont = param;
                else
                    param_set_font(cmd, param);
                return;
            }

            if (sfmt.ContainsKey(cmd) && parse.ufmt)
                cfmt = cfmt.Clone();

            switch (cmd)
            {
                case "aligncomposer":
                case "barsperstaff":
                case "infoline":
                case "measurenb":
                case "rbmax":
                case "rbmin":
                case "measrepnb":
                case "shiftunison":
                case "systnames":
                    v = int.Parse(param);
                    if (int.TryParse(param, out v))
                    {
                        cfmt[cmd] = v;
                    }
                    else
                    {
                        syntax(1, "Bad integer value");
                    }
                    break;
                case "abc-version":
                case "bgcolor":
                case "fgcolor":
                case "propagate-accidentals":
                case "titleformat":
                case "writeout-accidentals":
                    cfmt[cmd] = param;
                    break;
                case "beamslope":
                case "breaklimit":
                case "lineskipfac":
                case "maxshrink":
                case "pagescale":
                case "parskipfac":
                case "scale":
                case "slurheight":
                case "stemheight":
                case "tieheight":
                    f = double.Parse(param);
                    if (double.TryParse(param, out f) && param != null && f >= 0)
                    {
                        switch (cmd)
                        {
                            case "scale":
                                f /= .75;
                            case "pagescale":
                                if (f < .1)
                                    f = .1;
                                cmd = "scale";
                                img.chg = true;
                                break;
                        }
                        cfmt[cmd] = f;
                    }
                    else
                    {
                        syntax(1, errs.bad_val, "%%" + cmd);
                    }
                    break;
                case "annotationbox":
                case "gchordbox":
                case "measurebox":
                case "partsbox":
                    param_set_font(cmd.Replace("box", "font"), get_bool(param) ? "box" : "nobox");
                    break;
                case "altchord":
                case "bstemdown":
                case "breakoneoln":
                case "cancelkey":
                case "checkbars":
                case "contbarnb":
                case "custos":
                case "decoerr":
                case "flatbeams":
                case "graceslurs":
                case "graceword":
                case "hyphencont":
                case "keywarn":
                case "linewarn":
                case "quiet":
                case "squarebreve":
                case "splittune":
                case "straightflags":
                case "stretchstaff":
                case "timewarn":
                case "titlecaps":
                case "titleleft":
                case "trimsvg":
                    cfmt[cmd] = get_bool(param);
                    break;
                case "dblrepbar":
                    param = ":: " + param;
                    goto case "bardef";
                case "bardef":
                    var v = param.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (v.Length != 2)
                    {
                        syntax(1, errs.bad_val, "%%bardef");
                    }
                    else
                    {
                        if (parse.ufmt)
                            cfmt.bardef = cfmt.bardef.Clone();
                        cfmt.bardef[v[0]] = v[1];
                    }
                    break;
                case "chordalias":
                    v = param.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (v.Length == 0)
                    {
                        syntax(1, errs.bad_val, "%%chordalias");
                    }
                    else
                    {
                        abc2svg.ch_alias[v[0]] = v.Length > 1 ? v[1] : "";
                    }
                    break;
                case "composerspace":
                case "indent":
                case "infospace":
                case "maxstaffsep":
                case "maxsysstaffsep":
                case "musicspace":
                case "partsspace":
                case "staffsep":
                case "subtitlespace":
                case "sysstaffsep":
                case "textspace":
                case "titlespace":
                case "topspace":
                case "vocalspace":
                case "wordsspace":
                    f = get_unit(param);
                    if (double.TryParse(param, out f))
                    {
                        cfmt[cmd] = f;
                    }
                    else
                    {
                        syntax(1, errs.bad_val, "%%" + cmd);
                    }
                    break;
                case "page-format":
                    user.page_format = get_bool(param);
                    break;
                case "print-leftmargin":
                    syntax(0, "$1 is deprecated - use %%printmargin instead", "%%" + cmd);
                    cmd = "printmargin";
                    goto case "printmargin";
                case "printmargin":
                case "leftmargin":
                case "pagewidth":
                case "rightmargin":
                    f = get_unit(param);
                    if (double.TryParse(param, out f))
                    {
                        cfmt[cmd] = f;
                        img.chg = true;
                    }
                    else
                    {
                        syntax(1, errs.bad_val, "%%" + cmd);
                    }
                    break;
                case "concert-score":
                    if (cfmt.sound != "play")
                        cfmt.sound = "concert";
                    break;
                case "writefields":
                    set_writefields(param);
                    break;
                case "volume":
                    cmd = "dynamic";
                    goto case "dynamic";
                case "dynamic":
                case "gchord":
                case "gstemdir":
                case "ornament":
                case "stemdir":
                case "vocal":
                    set_pos(cmd, param);
                    break;
                case "font":
                    get_font_scale(param);
                    break;
                case "fullsvg":
                    if (parse.state != 0)
                    {
                        syntax(1, errs.not_in_tune, "%%fullsvg");
                        break;
                    }
                    cfmt[cmd] = param;
                    break;
                case "gracespace":
                    v = param.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (i = 0; i < 3; i++)
                    {
                        if (!double.TryParse(v[i], out f))
                        {
                            syntax(1, errs.bad_val, "%%gracespace");
                            break;
                        }
                    }
                    if (parse.ufmt)
                        cfmt[cmd] = new double[3];
                    for (i = 0; i < 3; i++)
                    {
                        cfmt[cmd][i] = double.Parse(v[i]);
                    }
                    break;
                case "tuplets":
                    v = param.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    f = v[3];
                    if (f != null)
                    {
                        f = posval[f];
                    }
                    if (f != null)
                    {
                        v[3] = f;
                    }
                    if (curvoice != null)
                    {
                        curvoice.tup = v;
                    }
                    else
                    {
                        cfmt[cmd] = v;
                    }
                    break;
                case "infoname":
                    set_infoname(param);
                    break;
                case "notespacingfactor":
                    v = Regex.Match(param, "([.\\d]+)[,\\s]*(\\d+)?");
                    if (v != null)
                    {
                        f = double.Parse(v[1]);
                        if (double.TryParse(v[1], out f) && f >= 1 && f <= 2)
                        {
                            if (v[2] != null)
                            {
                                f2 = double.Parse(v[2]);
                                if (double.TryParse(v[2], out f2))
                                {
                                    cfmt[cmd] = param;
                                    cfmt.spatab = new double[10];
                                    i = 5;
                                    do
                                    {
                                        cfmt.spatab[i] = f2;
                                        f2 /= f;
                                    } while (--i >= 0);
                                    i = 5;
                                    f2 = cfmt.spatab[i];
                                    for (; ++i < cfmt.spatab.Length;)
                                    {
                                        f2 *= f;
                                        cfmt.spatab[i] = f2;
                                    }
                                }
                            }
                            else
                            {
                                cfmt[cmd] = param;
                                cfmt.spatab = new double[10];
                                i = 5;
                                do
                                {
                                    cfmt.spatab[i] = f2;
                                    f2 /= f;
                                } while (--i >= 0);
                                i = 5;
                                f2 = cfmt.spatab[i];
                                for (; ++i < cfmt.spatab.Length;)
                                {
                                    f2 *= f;
                                    cfmt.spatab[i] = f2;
                                }
                            }
                        }
                    }
                    break;
                case "play":
                    cfmt.sound = "play";
                    break;
                case "pos":
                    v = Regex.Match(param, "(\\w*)\\s+(.*)");
                    if (v != null && v[2] != null)
                    {
                        if (v[1].Substring(0, 3) == "tup" && curvoice != null)
                        {
                            if (curvoice.tup == null)
                                curvoice.tup = cfmt.tuplets;
                            else
                                curvoice.tup = curvoice.tup.Clone();
                            v = posval[v[2]];
                            switch (v)
                            {
                                case C.SL_ABOVE:
                                    curvoice.tup[3] = 1;
                                    break;
                                case C.SL_BELOW:
                                    curvoice.tup[3] = 2;
                                    break;
                                case C.SL_HIDDEN:
                                    curvoice.tup[2] = 1;
                                    break;
                            }
                            break;
                        }
                        if (v[1].Substring(0, 3) == "vol")
                            v[1] = "dyn";
                        set_pos(v[1], v[2]);
                    }
                    else
                    {
                        syntax(1, "Error in %%pos");
                    }
                    break;
                case "sounding-score":
                    if (cfmt.sound != "play")
                        cfmt.sound = "sounding";
                    break;
                case "staffwidth":
                    v = get_unit(param);
                    if (double.TryParse(param, out v) && v >= 100)
                    {
                        v = cfmt.pagewidth - v - cfmt.leftmargin;
                        if (v >= 2)
                        {
                            cfmt.rightmargin = v;
                            img.chg = true;
                        }
                        else
                        {
                            syntax(1, "%%staffwidth too big");
                        }
                    }
                    else
                    {
                        syntax(1, "%%staffwidth too small");
                    }
                    break;
                case "textoption":
                    cfmt[cmd] = get_textopt(param);
                    break;
                case "dynalign":
                case "singleline":
                case "stretchlast":
                case "titletrim":
                    v = int.Parse(param);
                    if (int.TryParse(param, out v))
                    {
                        if (cmd[1] == 't')
                        {
                            if (v >= 0 && v <= 1)
                            {
                                cfmt[cmd] = v;
                            }
                            else
                            {
                                syntax(1, errs.bad_val, "%%" + cmd);
                            }
                        }
                        else
                        {
                            cfmt[cmd] = v;
                        }
                    }
                    else
                    {
                        cfmt[cmd] = get_bool(param) ? 0 : 1;
                    }
                    break;
                case "combinevoices":
                    syntax(1, "%%combinevoices is deprecated - use %%voicecombine instead");
                    break;
                case "voicemap":
                    set_v_param("map", param);
                    break;
                case "voicescale":
                    set_v_param("scale", param);
                    break;
                case "rbdbstop":
                    v = get_bool(param);
                    if (v && cfmt["abc-version"] >= "2.2")
                        cfmt["abc-version"] = "1";
                    else if (!v && cfmt["abc-version"] < "2.2")
                        cfmt["abc-version"] = "2.2";
                    break;
                default:
                    if (parse.state == 0)
                        cfmt[cmd] = param;
                    break;
            }

            if (sfmt.ContainsKey(cmd) && parse.ufmt)
            {
                parse.ufmt = false;
            }
        }

        string st_font(Font font)
        {
            var n = font.name;
            var r = "";

            if (font.weight != null)
                r += font.weight + " ";
            if (font.style != null)
                r += font.style + " ";
            if (n.IndexOf('"') < 0 && n.IndexOf(' ') > 0)
                n = '"' + n + '"';
            return r + font.size.ToString("F1") + "px " + n;
        }

        string style_font(Font font)
        {
            return "font:" + st_font(font);
        }

        string font_class(Font font)
        {
            var f = "f" + font.fid + cfmt.fullsvg;
            if (font.@class != null)
                f += " " + font.@class;
            if (font.box != null)
                f += " " + "box";
            return f;
        }

        void use_font(Font font)
        {
            if (!font.used)
            {
                font.used = true;
                if (font.fid == null)
                {
                    font.fid = abc2svg.font_tb.Count;
                    abc2svg.font_tb.Add(font);
                    if (font.swfac == null)
                        set_font_fac(font);
                    if (font.pad == null)
                        font.pad = 0;

                    font.cw_tb = !string.IsNullOrEmpty(font.name) ? ssw_tb
                        : font.name.IndexOf("ans") > 0
                            ? ssw_tb
                            : font.name.IndexOf("ono") > 0
                                ? mw_tb
                                : sw_tb;
                }
                add_fstyle(".f" + font.fid +
                    (cfmt.fullsvg ?? "") +
                    "{" + style_font(font) + "}");
                if (font.src != null)
                    add_fstyle("@font-face{\n" +
                        " font-family:" + font.name + ";\n" +
                        " src:" + font.src + "}");
                if (font == cfmt.musicfont)
                    add_fstyle(".f" + font.fid +
                        (cfmt.fullsvg ?? "") +
                        " text,tspan{white-space:pre}");
            }
        }

        Font get_font(string fn)
        {
            Font font, font2;
            int fid;
            string st;

            fn += "font";
            font = cfmt[fn];
            if (font == null)
            {
                syntax(1, "Unknown font $1", fn);
                return gene.curfont;
            }

            if (string.IsNullOrEmpty(font.name) || font.size == null)
            {
                font2 = gene.deffont.Clone();
                if (!string.IsNullOrEmpty(font.name))
                    font2.name = font.name;
                if (font.normal != null)
                {
                    if (font2.weight != null)
                        font2.weight = null;
                    if (font2.style != null)
                        font2.style = null;
                }
                else
                {
                    if (font.weight != null)
                        font2.weight = font.weight;
                    if (font.style != null)
                        font2.style = font.style;
                }
                if (font.src != null)
                    font2.src = font.src;
                if (font.size != null)
                    font2.size = font.size;
                st = st_font(font2);
                if (font.@class != null)
                {
                    font2.@class = font.@class;
                    st += ' ' + font.@class;
                }
                fid = abc2svg.font_st[st];
                if (fid != null)
                    return abc2svg.font_tb[fid];
                abc2svg.font_st[st] = abc2svg.font_tb.Count;
                font2.fid = font2.used = null;
                font = font2;
            }
            use_font(font);
            return font;
        }
    }
}







