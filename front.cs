using System;
using System.Collections.Generic;
using System.Text;


// abc2svg - front.js - ABC parsing front-end
//
// Copyright (C) 2014-2023 Jean-Francois Moine
//
// This file is part of abc2svg-core.
//
// abc2svg-core is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// abc2svg-core is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with abc2svg-core.  If not, see <http://www.gnu.org/licenses/>.

namespace autocad_part2
{
    partial class Abc
    {
        public SaveGlobalDefinitions sav = new SaveGlobalDefinitions(); // save global (between tunes) definitions
        public Dictionary<string, string> mac = new Dictionary<string, string>(); // macros (m:)
        public Dictionary<string, string> maci = new Dictionary<string, string>(); // first letters of macros
        public Dictionary<string, string> modone = new Dictionary<string, string>(); // hooks done by module

        // translation table from the ABC draft version 2.2
        public  Dictionary<string, string> abc_utf = new Dictionary<string, string>()
        {
            {"=D", "Đ"},
            {"=H", "Ħ"},
            {"=T", "Ŧ"},
            {"=d", "đ"},
            {"=h", "ħ"},
            {"=t", "ŧ"},
            {"/O", "Ø"},
            {"/o", "ø"},
    //	"/D": "Đ",
    //	"/d": "đ",
            {"/L", "Ł"},
            {"/l", "ł"},
            {"vL", "Ľ"},
            {"vl", "ľ"},
            {"vd", "ď"},
            {".i", "ı"},
            {"AA", "Å"},
            {"aa", "å"},
            {"AE", "Æ"},
            {"ae", "æ"},
            {"DH", "Ð"},
            {"dh", "ð"},
    //	"ng": "ŋ",
            {"OE", "Œ"},
            {"oe", "œ"},
            {"ss", "ß"},
            {"TH", "Þ"},
            {"th", "þ"}
        };

        // accidentals as octal values (abcm2ps compatibility)
        public  Dictionary<string, string> oct_acc = new Dictionary<string, string>()
        {
            {"1", "\u266f"},
            {"2", "\u266d"},
            {"3", "\u266e"},
            {"4", "&#x1d12a;"},
            {"5", "&#x1d12b;"}
        };

        // ABC include
        public int include = 0;

        // convert the escape sequences to utf-8
        public static string cnv_escape(string src, string flag = null)
        {
            string dst = "";
            int i, j = 0;
            while (true)
            {
                i = src.IndexOf('\\', j);
                if (i < 0)
                    break;
                dst += src.Substring(j, i - j);
                char c = src[++i];
                if (c == '\0')
                    return dst + '\\';
                switch (c)
                {
                    case '0':
                    case '2':
                        if (src[i + 1] != '0')
                            break;
                        string c2 = oct_acc[src.Substring(i + 2, 1)];
                        if (c2 != null)
                        {
                            dst += c2;
                            j = i + 3;
                            continue;
                        }
                        break;
                    case 'u':
                        j = int.Parse("0x" + src.Substring(i + 1, 4));
                        if (j < 0x20)
                        {
                            dst += src[++i] + "\u0306";
                            j = i + 1;
                            continue;
                        }
                        c = (char)j;
                        if (c == '\\')
                        {
                            i += 4;
                            break;
                        }
                        dst += c;
                        j = i + 5;
                        continue;
                    case 't':
                        dst += '\t';
                        j = i + 1;
                        continue;
                    case 'n':
                        dst += '\n';
                        j = i + 1;
                        continue;
                    default:
                        c2 = abc_utf[src.Substring(i, 2)];
                        if (c2 != null)
                        {
                            dst += c2;
                            j = i + 2;
                            continue;
                        }
                        c2 = src.Substring(i + 1, 1);
                        if (c2 == null)
                            break;
                        if (!char.IsLetter(c2[0]))
                            break;
                        switch (c)
                        {
                            case '`':
                                dst += c2 + "\u0300";
                                j = i + 2;
                                continue;
                            case '\'':
                                dst += c2 + "\u0301";
                                j = i + 2;
                                continue;
                            case '^':
                                dst += c2 + "\u0302";
                                j = i + 2;
                                continue;
                            case '~':
                                dst += c2 + "\u0303";
                                j = i + 2;
                                continue;
                            case '=':
                                dst += c2 + "\u0304";
                                j = i + 2;
                                continue;
                            case '_':
                                dst += c2 + "\u0305";
                                j = i + 2;
                                continue;
                            case '.':
                                dst += c2 + "\u0307";
                                j = i + 2;
                                continue;
                            case '"':
                                dst += c2 + "\u0308";
                                j = i + 2;
                                continue;
                            case 'o':
                                dst += c2 + "\u030a";
                                j = i + 2;
                                continue;
                            case 'H':
                                dst += c2 + "\u030b";
                                j = i + 2;
                                continue;
                            case 'v':
                                dst += c2 + "\u030c";
                                j = i + 2;
                                continue;
                            case 'c':
                                dst += c2 + "\u0327";
                                j = i + 2;
                                continue;
                            case ';':
                                dst += c2 + "\u0328";
                                j = i + 2;
                                continue;
                        }
                        break;
                }
                if (flag == "w")
                    dst += '\\';
                dst += c;
                j = i + 1;
            }
            return dst + src.Substring(j);
        }

        public static void do_include(string fn)
        {
            string file;
            if (user.read_file == null)
            {
                syntax(1, "No read_file support");
                return;
            }
            if (include > 2)
            {
                syntax(1, "Too many include levels");
                return;
            }
            file = user.read_file(fn);
            if (file == null)
            {
                syntax(1, "Cannot read file '$1'", fn);
                return;
            }
            include++;
            var parse_sav = parse.Clone();
            tosvg(fn, file);
            parse_sav.state = parse.state;
            parse = parse_sav;
            include--;
        }

        // parse ABC code
        public void tosvg(string in_fname, string file, int bol = 0, int eof = 0)
        {
            int i, c, eol, end;
            string select, line0, line1, last_info, opt, text, a, b, s, pscom, txt_add = "\n";

            // check if a tune is selected
            bool tune_selected()
            {
                int re, res, i = file.IndexOf("K:", bol);

                if (i < 0)
                {
                    // syntax(1, "No K: in tune")
                    return false;
                }
                i = file.IndexOf("\n", i);
                if (parse.select.IsMatch(file.Substring(parse.bol, i)))
                    return true;
                re = file.IndexOf("\n\\w*\\n", i);
                res = re;
                if (res > 0)
                    eol = re;
                else
                    eol = eof;
                return false;
            }

            // remove the comment at end of text
            // if flag, handle the escape sequences
            // if flag is 'w' (lyrics), keep the '\'s
            string uncomment(string src, string flag = null)
            {
                if (string.IsNullOrEmpty(src))
                    return src;
                int i = src.IndexOf("%");
                if (i == 0)
                    return "";
                if (i > 0)
                    src = src.Replace("([^\\\\])%.*", "$1").Replace("\\%", "%");
                src = src.Replace("\\s+$", "");
                if (!string.IsNullOrEmpty(flag) && src.IndexOf("\\") >= 0)
                    return cnv_escape(src, flag);
                return src;
            }

            void end_tune()
            {
                generate();
                cfmt = sav.cfmt;
                info = sav.info;
                char_tb = sav.char_tb;
                glovar = sav.glovar;
                maps = sav.maps;
                mac = sav.mac;
                maci = sav.maci;
                parse.tune_v_opts = null;
                parse.scores = null;
                parse.ufmt = false;
                parse.ctrl = null;
                init_tune();
                img.chg = true;
                set_page();
            }

            // get %%voice
            void do_voice(string select, bool in_tune = false)
            {
                string opt;
                if (select == "end")
                    return; // end of previous %%voice

                // get the options
                if (in_tune)
                {
                    if (parse.tune_v_opts == null)
                        parse.tune_v_opts = new Dictionary<string, List<string>>();
                    opt = parse.tune_v_opts;
                }
                else
                {
                    if (parse.voice_opts == null)
                        parse.voice_opts = new Dictionary<string, List<string>>();
                    opt = parse.voice_opts;
                }
                opt[select] = new List<string>();
                while (true)
                {
                    bol = ++eol;
                    if (file[bol] != '%')
                        break;
                    eol = file.IndexOf("\n", eol);
                    if (file[bol + 1] != line1)
                        continue;
                    bol += 2;
                    if (eol < 0)
                        text = file.Substring(bol);
                    else
                        text = file.Substring(bol, eol);
                    a = text.Match("\\S+");
                    switch (a[0])
                    {
                        default:
                            opt[select].Add(uncomment(text, true));
                            continue;
                        case "score":
                        case "staves":
                        case "tune":
                        case "voice":
                            bol -= 2;
                            break;
                    }
                    break;
                }
                eol = parse.eol = bol - 1;
            }

            // apply the options to the current tune
            void tune_filter()
            {
                string o, opts, j, pc, h;
                int i = file.IndexOf("K:", bol.Value);

                i = file.IndexOf("\n", i);
                h = file.Substring(parse.bol, i); // tune header

                foreach (var item in parse.tune_opts)
                {
                    if (!parse.tune_opts.ContainsKey(item.Key))
                        continue;
                    if (!new Regex(item.Key).IsMatch(h))
                        continue;
                    opts = parse.tune_opts[item.Key];
                    foreach (var item2 in opts.t_opts)
                    {
                        pc = item2;
                        switch (pc.Match("\\S+")[0])
                        {
                            case "score":
                            case "staves":
                                if (parse.scores == null)
                                    parse.scores = new List<string>();
                                parse.scores.Add(pc);
                                break;
                            default:
                                self.do_pscom(pc);
                                break;
                        }
                    }
                    opts = opts.v_opts;
                    if (opts == null)
                        continue;
                    foreach (var item3 in opts)
                    {
                        if (!parse.tune_v_opts.ContainsKey(item3.Key))
                            parse.tune_v_opts[item3.Key] = opts[item3.Key];
                        else
                            parse.tune_v_opts[item3.Key] = parse.tune_v_opts[item3.Key].Concat(opts[item3.Key]).ToList();
                    }
                }
            }

            // export functions and/or set module hooks
            if (abc2svg.mhooks != null)
            {
                foreach (var item in abc2svg.mhooks)
                {
                    if (!modone.ContainsKey(item.Key))
                    {
                        modone[item.Key] = "1"; // true
                        abc2svg.mhooks[item.Key](self);
                    }
                }
            }

            // initialize
            parse.file = file; // used for errors
            parse.fname = in_fname;

            // scan the file
            if (bol == null)
                bol = 0;
            if (eof == null)
                eof = file.Length;
            if (file.Substring(bol.Value, 5) == "%abc-")
                cfmt["abc-version"] = new Regex("[1-9.]+").Match(file.Substring(bol.Value + 5, bol.Value + 10)).ToString();
            for (; bol < eof; bol = parse.eol + 1)
            {
                eol = file.IndexOf("\n", bol.Value); // get a line
                if (eol < 0 || eol > eof)
                    eol = eof.Value;
                parse.eol = eol;

                // remove the ending white spaces
                while (true)
                {
                    eol--;
                    switch (file[eol])
                    {
                        case ' ':
                        case '\t':
                            continue;
                    }
                    break;
                }
                eol++;
                if (eol == bol) // empty line
                {
                    if (parse.state == 1)
                    {
                        parse.istart = bol.Value;
                        syntax(1, "Empty line in tune header - ignored");
                    }
                    else if (parse.state >= 2)
                    {
                        end_tune();
                        parse.state = 0;
                        if (parse.select != null) // skip to next tune
                        {
                            eol = file.IndexOf("\nX:", parse.eol);
                            if (eol < 0)
                                eol = eof.Value;
                            parse.eol = eol;
                        }
                    }
                    continue;
                }
                parse.istart = parse.bol = bol.Value;
                parse.iend = eol;
                parse.line.index = 0;

                // check if the line is a pseudo-comment or I:
                line0 = file[bol.Value].ToString();
                line1 = file[bol.Value + 1].ToString();
                if ((line0 == "I" && line1 == ":") || line0 == "%")
                {
                    if (line0 == "%" && parse.prefix.IndexOf(line1) < 0)
                        continue; // comment

                    // change "%%abc xxxx" to "xxxx"
                    if (file.Substring(bol.Value + 2, 5) == "abc ")
                    {
                        bol += 6;
                        line0 = file[bol.Value].ToString();
                        line1 = file[bol.Value + 1].ToString();
                    }
                    else
                    {
                        pscom = "true";
                    }
                }

                // pseudo-comments
                if (pscom != null)
                {
                    pscom = null;
                    bol += 2; // skip %%/I:
                    text = file.Substring(bol.Value, eol - bol.Value);
                    a = text.Match("([^\s]+)\\s*(.*)");
                    if (a == null || a[1][0] == "%")
                        continue;
                    switch (a[1])
                    {
                        case "abcm2ps":
                        case "ss-pref":
                            parse.prefix = a[2]; // may contain a '%'
                            continue;
                        case "abc-include":
                            do_include(uncomment(a[2]));
                            continue;
                    }

                    // beginxxx/endxxx
                    if (a[1].Substring(0, 5) == "begin")
                    {
                        b = a[1].Substring(5);
                        end = "\n" + line0 + line1 + "end" + b;
                        i = file.IndexOf(end, eol);
                        if (i < 0)
                        {
                            syntax(1, "No $1 after %%$2", end.Substring(1), a[1]);
                            parse.eol = eof.Value;
                            continue;
                        }
                        self.do_begin_end(b, uncomment(a[2]), file.Substring(eol + 1, i - eol).Replace("\n%[^%].*$", "").Replace("^%%", ""));
                        parse.eol = file.IndexOf("\n", i + 6);
                        if (parse.eol < 0)
                            parse.eol = eof.Value;
                        continue;
                    }
                    switch (a[1])
                    {
                        case "select":
                            if (parse.state != 0)
                            {
                                syntax(1, errs.not_in_tune, "%%select");
                                continue;
                            }
                            select = uncomment(text.Substring(7));
                            if (select[0] == "\"")
                                select = select.Substring(1, select.Length - 2);
                            if (string.IsNullOrEmpty(select))
                            {
                                parse.select = null;
                                continue;
                            }
                            select = select.Replace("(", "\\(");
                            select = select.Replace(")", "\\)");
                            //				select = select.replace(/\|/g, '\\|');
                            parse.select = new Regex(select, RegexOptions.Multiline);
                            continue;
                        case "tune":
                            if (parse.state != 0)
                            {
                                syntax(1, errs.not_in_tune, "%%tune");
                                continue;
                            }
                            select = uncomment(a[2]);

                            // if void %%tune, free all tune options
                            if (string.IsNullOrEmpty(select))
                            {
                                parse.tune_opts = new Dictionary<string, object>();
                                continue;
                            }

                            if (select == "end")
                                continue; // end of previous %%tune

                            if (parse.tune_opts == null)
                                parse.tune_opts = new Dictionary<string, object>();
                            parse.tune_opts[select] = opt = new Dictionary<string, object>()
                        {
                            { "t_opts", new List<string>() }
                            //						v_opts: {}
                        };
                            while (true)
                            {
                                bol = ++eol;
                                if (file[bol] != '%')
                                    break;
                                eol = file.IndexOf("\n", eol);
                                if (file[bol + 1] != line1)
                                    continue;
                                bol += 2;
                                if (eol < 0)
                                    text = file.Substring(bol);
                                else
                                    text = file.Substring(bol, eol);
                                a = text.Match("([^\s]+)\\s*(.*)");
                                switch (a[1])
                                {
                                    case "tune":
                                        break;
                                    case "voice":
                                        do_voice(uncomment(a[2], true), true);
                                        continue;
                                    default:
                                        opt.t_opts.Add(uncomment(text, true));
                                        continue;
                                }
                                break;
                            }
                            if (parse.tune_v_opts != null)
                            {
                                opt.v_opts = parse.tune_v_opts;
                                parse.tune_v_opts = null;
                            }
                            parse.eol = bol - 1;
                            continue;
                        case "voice":
                            if (parse.state != 0)
                            {
                                syntax(1, errs.not_in_tune, "%%voice");
                                continue;
                            }
                            select = uncomment(a[2]);

                            /* if void %%voice, free all voice options */
                            if (string.IsNullOrEmpty(select))
                            {
                                parse.voice_opts = null;
                                continue;
                            }

                            do_voice(select);
                            continue;
                    }
                    self.do_pscom(uncomment(text, true));
                    continue;
                }

                // music line (or free text)
                if (line1 != ":" || !new Regex("[A-Za-z+]").IsMatch(line0))
                {
                    last_info = null;
                    if (parse.state < 2)
                        continue;
                    parse.line.buffer = uncomment(file.Substring(bol.Value, eol - bol.Value));
                    if (!string.IsNullOrEmpty(parse.line.buffer))
                        parse_music_line();
                    continue;
                }

                // information fields
                bol += 2;
                while (true)
                {
                    switch (file[bol.Value])
                    {
                        case ' ':
                        case '\t':
                            bol++;
                            continue;
                    }
                    break;
                }
                if (line0 == "+")
                {
                    if (string.IsNullOrEmpty(last_info))
                    {
                        syntax(1, "+: without previous info field");
                        continue;
                    }
                    txt_add = " "; // concatenate
                    line0 = last_info;
                }
                text = uncomment(file.Substring(bol.Value, eol - bol.Value), line0);

                switch (line0)
                {
                    case "X": // start of tune
                        if (parse.state != 0)
                        {
                            syntax(1, errs.ignored, line0);
                            continue;
                        }
                        if (parse.select != null && !tune_selected())
                        { // skip to the next tune
                            eol = file.IndexOf("\nX:", parse.eol);
                            if (eol < 0)
                                eol = eof.Value;
                            parse.eol = eol;
                            continue;
                        }

                        sav.cfmt = cfmt.Clone();
                        sav.info = info.Clone(2); // (level 2 for info.V[])
                        sav.char_tb = char_tb.Clone();
                        sav.glovar = glovar.Clone();
                        sav.maps = maps.Clone(1);
                        sav.mac = mac.Clone();
                        sav.maci = maci.Clone();
                        info.X = text;
                        parse.state = 1; // tune header
                        if (user.page_format && blkdiv < 1)
                            blkdiv = 1; // the tune starts by the next SVG
                        if (parse.tune_opts != null)
                            tune_filter();
                        continue;
                    case "T":
                        switch (parse.state)
                        {
                            case 0:
                                continue;
                            case 1: // tune header
                            case 2:
                                if (info.T == null) // (keep empty T:)
                                    info.T = text;
                                else
                                    info.T += "\n" + text;
                                continue;
                        }
                        s = new_block("title");
                        s.text = text;
                        continue;
                    case "K":
                        switch (parse.state)
                        {
                            case 0:
                                continue;
                            case 1: // tune header
                                info.K = text;
                                break;
                        }
                        do_info(line0, text);
                        continue;
                    case "W":
                        if (parse.state == 0 || cfmt.writefields.IndexOf(line0) < 0)
                            break;
                        if (info.W == null)
                            info.W = text;
                        else
                            info.W += txt_add + text;
                        break;

                    case "m":
                        if (parse.state >= 2)
                        {
                            syntax(1, errs.ignored, line0);
                            continue;
                        }
                        a = text.Match("(.*?)[= ]+(.*)");
                        if (a == null || string.IsNullOrEmpty(a[2]))
                        {
                            syntax(1, errs.bad_val, "m:");
                            continue;
                        }
                        mac[a[1]] = a[2];
                        maci[a[1][0]] = "true"; // first letter
                        break;

                    // info fields in tune body only
                    case "s":
                        if (parse.state != 3 || cfmt.writefields.IndexOf(line0) < 0)
                            break;
                        get_sym(text, txt_add == " ");
                        break;
                    case "w":
                        if (parse.state != 3 || cfmt.writefields.IndexOf(line0) < 0)
                            break;
                        get_lyrics(text, txt_add == " ");
                        break;
                    case "|": // "|:" starts a music line
                        if (parse.state < 2)
                            continue;
                        parse.line.buffer = text;
                        parse_music_line();
                        continue;
                    default:
                        if ("ABCDFGHNOSZ".IndexOf(line0) >= 0)
                        {
                            if (parse.state >= 2)
                            {
                                syntax(1, errs.ignored, line0);
                                continue;
                            }
                            if (info[line0] == null)
                                info[line0] = text;
                            else
                                info[line0] += txt_add + text;
                            break;
                        }

                        // info field which may be embedded
                        do_info(line0, text);
                        continue;
                }
                txt_add = "\n";
                last_info = line0;
            }
            if (include)
                return;
            if (parse.state >= 2)
                end_tune();
            parse.state = 0;
        }
    }




}














