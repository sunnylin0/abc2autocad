using System;
using System.Collections.Generic;
using System.Text;

namespace autocad_part2
{
    class Abc
    {

        // transpose a chord symbol
        string note_names = "CDEFGAB";
        string[] acc_name = { "bb", "b", "", "#", "##" };

        // -- parse a chord symbol / annotation --
        // the result is added in the global variable a_gch
        // 'type' may be a single '"' or a string '"xxx"' created by U:
        void parse_gchord(string type)
        {
            char c;
            string text, gch;
            double x_abs, y_abs;
            int i, j, istart, iend;
            Font ann_font = get_font("annotation");
            var h_ann = ann_font.size;
            var line = parse.line;

            double get_float()
            {
                string txt = "";

                while (true)
                {
                    c = text[i++];
                    if ("1234567890.-".IndexOf(c) < 0)
                        return double.Parse(txt);
                    txt += c;
                }
            }

            istart = parse.bol + line.index;
            if (type.Length > 1)
            {
                text = type.Substring(1, type.Length - 2);
                iend = istart + 1;
            }
            else
            {
                i = ++line.index;
                while (true)
                {
                    j = line.buffer.IndexOf('"', i);
                    if (j < 0)
                    {
                        syntax(1, "No end of chord symbol/annotation");
                        return;
                    }
                    if (line.buffer[j - 1] != '\\' || line.buffer[j - 2] == '\\')
                        break;
                    i = j + 1;
                }
                text = cnv_escape(line.buffer.Substring(line.index, j));
                line.index = j;
                iend = parse.bol + line.index + 1;
            }

            if (ann_font.pad != 0)
                h_ann += ann_font.pad;
            i = 0;
            type = "g";
            while (true)
            {
                c = text[i];
                if (c == '\0')
                    break;
                gch = "";
                switch (c)
                {
                    case '@':
                        type = c.ToString();
                        i++;
                        x_abs = get_float();
                        if (c != ',')
                        {
                            syntax(1, "',' lacking in annotation '@x,y'");
                            y_abs = 0;
                        }
                        else
                        {
                            y_abs = get_float();
                            if (c != ' ')
                                i--;
                        }
                        break;
                    case '^':
                        gch.pos = C.SL_ABOVE;
                        goto case '_';
                    case '_':
                        if (c == '_')
                            gch.pos = C.SL_BELOW;
                        goto case '<';
                    case '<':
                    case '>':
                        i++;
                        type = c.ToString();
                        break;
                    default:
                        switch (type)
                        {
                            case "g":
                                gch.font = get_font("gchord");
                                gch.pos = curvoice.pos.gch ?? C.SL_ABOVE;
                                break;
                            case "^":
                                gch.pos = C.SL_ABOVE;
                                break;
                            case "_":
                                gch.pos = C.SL_BELOW;
                                break;
                            case "@":
                                gch.x = x_abs;
                                y_abs -= h_ann;
                                gch.y = y_abs;
                                break;
                        }
                        break;
                }
                gch.type = type;
                while (true)
                {
                    c = text[i];
                    if (c == '\0')
                        break;
                    switch (c)
                    {
                        case '\\':
                            c = text[++i];
                            if (c == 'n')
                                break;
                            gch.text += '\\';
                            if (c != '\0')
                                break;
                            gch.text += c;
                            continue;
                        default:
                            gch.text += c;
                            i++;
                            continue;
                        case '&':
                            while (true)
                            {
                                gch.text += c;
                                c = text[++i];
                                switch (c)
                                {
                                    default:
                                        continue;
                                    case ';':
                                    case '\0':
                                    case '\\':
                                        break;
                                }
                                break;
                            }
                            if (c == ';')
                            {
                                i++;
                                gch.text += c;
                                continue;
                            }
                            break;
                        case ';':
                            break;
                    }
                    i++;
                    break;
                }
                gch.otext = gch.text;
                if (a_gch == null)
                    a_gch = new List<string>();
                a_gch.Add(gch);
            }
        }

        string gch_tr1(string p, int tr)
        {
            int i, j, o, n, a, ip, b40, b40c;
            var csa = p.Split('/');

            for (i = 0; i < csa.Length; i++)
            {
                p = csa[i];
                o = p.IndexOfAny("ABCDEFG".ToCharArray());
                if (o < 0)
                    continue;
                ip = o + 1;
                a = 0;
                while (p[ip] == '#' || p[ip] == '\u266f')
                {
                    a++;
                    ip++;
                }
                while (p[ip] == 'b' || p[ip] == '\u266d')
                {
                    a--;
                    ip++;
                }
                n = "CDEFGAB".IndexOf(p[o]) + 16;
                b40 = (abc2svg.pab40(n, a) + tr + 200) % 40;
                if (i == 0)
                    b40c = (p[ip] == 'm' && p[ip + 1] != 'a') ? abc2svg.b40mc : abc2svg.b40Mc;
                j = b40c[b40] - b40;
                if (j != 0)
                    b40 += j;
                csa[i] = p.Substring(0, o) +
                    "CDEFGAB"[abc2svg.b40_p[b40]] +
                    new[] { "bb", "b", "", "#", "##" }[abc2svg.b40_a[b40] + 2] +
                    p.Substring(ip);
            }
            return string.Join("/", csa);
        }

        // parser: add the parsed list of chord symbols and annotations
        //	to the symbol (note, rest or bar)
        //	and transpose the chord symbols
        void csan_add(VoiceItem s)
        {
            int i;
            string gch;

            if (s.type == C.BAR)
            {
                for (i = 0; i < a_gch.Count; i++)
                {
                    if (a_gch[i].type == "g")
                    {
                        error(1, s, "There cannot be chord symbols on measure bars");
                        a_gch.RemoveRange(i, a_gch.Count - i);
                    }
                }
            }

            if (curvoice.tr_sco != null)
            {
                for (i = 0; i < a_gch.Count; i++)
                {
                    gch = a_gch[i];
                    if (gch.type == "g")
                        gch.text = gch_tr1(gch.text, curvoice.tr_sco.Value);
                }
            }

            if (s.a_gch != null)
                s.a_gch.AddRange(a_gch);
            else
                s.a_gch = a_gch;
            a_gch = null;
        }

        // generator: build the chord symbols / annotations
        // (possible hook)
        void gch_build(VoiceItem s)
        {
            string gch;
            double wh, xspc, ix;
            double y_left = 0, y_right = 0;
            const double GCHPRE = .4;

            for (ix = 0; ix < s.a_gch.Count; ix++)
            {
                gch = s.a_gch[ix];
                if (gch.type == "g")
                {
                    gch.text = gch.text.Replace("##", "&#x1d12a;")
                        .Replace("#", "\u266f")
                        .Replace("=", "\u266e")
                        .Replace("b", "\u266d");
                }
                else
                {
                    if (gch.type == "@"
                        && !user.anno_start && !user.anno_stop)
                    {
                        set_font(gch.font);
                        gch.text = str2svg(gch.text);
                        continue;
                    }
                }

                set_font(gch.font);
                gch.text = str2svg(gch.text);
                wh = gch.text.wh;
                switch (gch.type)
                {
                    case "@":
                        break;
                    default:
                    case "g":
                    case "^":
                    case "_":
                        xspc = (int)(wh * GCHPRE);
                        if (xspc > 8)
                            xspc = 8;
                        gch.x = -xspc;
                        break;
                    case "<":
                        gch.x = -(wh + 6);
                        y_left -= wh;
                        gch.y = y_left + wh / 2;
                        break;
                    case ">":
                        gch.x = 6;
                        y_right -= wh;
                        gch.y = y_right + wh / 2;
                        break;
                }
            }

            y_left /= 2;
            y_right /= 2;
            for (ix = 0; ix < s.a_gch.Count; ix++)
            {
                gch = s.a_gch[ix];
                switch (gch.type)
                {
                    case "<":
                        gch.y -= y_left;
                        break;
                    case ">":
                        gch.y -= y_right;
                        break;
                }
            }
        }

        // -- draw the chord symbols and annotations
        // (the staves are not yet defined)
        // (unscaled delayed output)
        // (possible hook)
        void draw_gchord(int i, VoiceItem s, double x, double y)
        {
            if (s.invis && s.play)
                return;
            string an;
            int h, pad, w, dy;
            var h_ann = s.font.size;
            var line = parse.line;

            if (s.type == C.BAR)
            {
                for (i = 0; i < a_gch.Count; i++)
                {
                    if (a_gch[i].type == "g")
                    {
                        error(1, s, "There cannot be chord symbols on measure bars");
                        a_gch.RemoveRange(i, a_gch.Count - i);
                    }
                }
            }

            if (curvoice.tr_sco != null)
            {
                for (i = 0; i < a_gch.Count; i++)
                {
                    gch = a_gch[i];
                    if (gch.type == "g")
                        gch.text = gch_tr1(gch.text, curvoice.tr_sco.Value);
                }
            }

            if (s.a_gch != null)
                s.a_gch.AddRange(a_gch);
            else
                s.a_gch = a_gch;
            a_gch = null;
        }

        // draw all chord symbols
        void draw_all_chsy()
        {
            VoiceItem s;
            string word, p, ly;
            int i, j, ly, dfnt, ln, c, cf;

            if (curvoice.ignore)
                return;
            if ((curvoice.pos.voc & 0x07) != C.SL_HIDDEN)
                curvoice.have_ly = true;

            if (cont)
            {
                s = curvoice.lyric_cont;
                if (s == null)
                {
                    syntax(1, "+: lyric without music");
                    return;
                }
                dfnt = get_font("vocal");
                if (gene.deffont != dfnt)
                {
                    if (gene.curfont == gene.deffont)
                        gene.curfont = dfnt;
                    gene.deffont = dfnt;
                }
            }
            else
            {
                set_font("vocal");
                if (curvoice.lyric_restart)
                {
                    curvoice.lyric_start = s = curvoice.lyric_restart;
                    curvoice.lyric_restart = null;
                    curvoice.lyric_line = 0;
                }
                else
                {
                    curvoice.lyric_line++;
                    s = curvoice.lyric_start;
                }
                if (s == null)
                    s = curvoice.sym;
                if (s == null)
                {
                    syntax(1, "w: without music");
                    return;
                }
            }

            p = text;
            i = 0;
            cf = gene.curfont;
            while (true)
            {
                while (p[i] == ' ' || p[i] == '\t')
                    i++;
                if (p[i] == '\0')
                    break;
                ln = 0;
                j = parse.istart + i + 2;
                switch (p[i])
                {
                    case '|':
                        while (s != null && s.type != C.BAR)
                            s = s.next;
                        if (s == null)
                        {
                            syntax(1, "Not enough measure bars for lyric line");
                            return;
                        }
                        s = s.next;
                        i++;
                        continue;
                    case '-':
                    case '_':
                        word = p[i].ToString();
                        ln = p[i] == '-' ? 2 : 3;
                        break;
                    case '*':
                        word = "";
                        break;
                    default:
                        word = "";
                        while (true)
                        {
                            if (p[i] == '\0')
                                break;
                            switch (p[i])
                            {
                                case '_':
                                case '*':
                                case '|':
                                    i--;
                                case ' ':
                                case '\t':
                                    break;
                                case '~':
                                    word += ' ';
                                    i++;
                                    continue;
                                case '-':
                                    ln = 1;
                                    break;
                                case '\\':
                                    if (p[++i] == '\0')
                                        continue;
                                    word += p[i++];
                                    continue;
                                case '$':
                                    word += p[i++];
                                    c = p[i];
                                    if (c == '0')
                                        gene.curfont = gene.deffont;
                                    else if (c >= '1' && c <= '9')
                                        gene.curfont = get_font("u" + c);
                                    break;
                                default:
                                    word += p[i++];
                                    continue;
                            }
                            break;
                        }
                        break;
                }

                while (s != null && s.type != C.NOTE)
                    s = s.next;
                if (s == null)
                {
                    syntax(1, "Too many words in lyric line");
                    return;
                }
                if (!string.IsNullOrEmpty(word) && (s.pos.voc & 0x07) != C.SL_HIDDEN)
                {
                    ly = new
                    {
                        t = word,
                        font = cf,
                        istart = j,
                        iend = j + word.Length
                    };
                    if (ln != 0)
                        ly.ln = ln;
                    if (s.a_ly == null)
                        s.a_ly = new List<string>();
                    s.a_ly[curvoice.lyric_line] = ly;
                    cf = gene.curfont;
                }
                s = s.next;
                i++;
            }
            curvoice.lyric_cont = s;
        }


    }
}
