using System;
using System.Collections.Generic;
using System.Text;

// abc2svg - lyrics.js - lyrics
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

            //var curvoice = new
            //{
            //    ignore = false,
            //    sym_cont = new Voice(),
            //    sym_restart = new Voice(),
            //    sym_start = new Voice(),
            //    sym = new Voice()
            //};
            //var a_dcn = new List<string>();
            //var a_gch = new List<string>();
            //var char_tb = new Dictionary<int, string>();
            ////var errs = new
            ////{
            ////    bad_char = "Bad character"
            ////};


            ////void syntax(int a, string b) { }
            ////void deco_cnv(Voice a, Voice b) { }
            ////void parse_gchord(string a) { }
            ////void csan_add(Voice a) { }

            //var gene = new
            //{
            //    deffont = "vocal",
            //    curfont = "vocal"
            //};

            //var curvoice = new
            //{
            //    pos = new
            //    {
            //        voc = 0x07
            //    },
            //    have_ly = false,
            //    lyric_cont = new Voice(),
            //    lyric_restart = new Voice(),
            //    lyric_start = new Voice(),
            //    lyric_line = 0
            //};

            //void set_font(string a) { }
            //string get_font(string a) { return ""; }
            //void anno_start(Voice a, string b) { }
            //void anno_stop(Voice a, string b) { }
            //void xy_str(int a, int b, string c) { }
            //void out_wln(int a, int b, int c) { }
            //void out_hyph(int a, int b, int c) { }
            //int cwid(char a) { return 0; }
            //int realwidth = 0;
            //int y_get(int a, int b, int c, int d) { return 0; }
            //void set_dscale(int a, bool b) { }
            //void y_set(int a, int b, int c, int d, int e) { }
            //void draw_lyric_line(Voice p_voice, int j, int y) { }
            //int draw_lyrics(Voice p_voice, int nly, List<int> a_h, int y, int incr) { return 0; }
       





        // parse a symbol line (s:)
        public void get_sym(string p, bool cont)
            {
                VoiceItem s;
                int c, i, j, d;

                if (curvoice.ignore)
                    return;

                // get the starting symbol of the lyrics
                if (cont)
                {                    // +:
                    s = curvoice.sym_cont;
                    if (s == null)
                    {
                        syntax(1, "+: symbol line without music");
                        return;
                    }
                }
                else
                {
                    if (curvoice.sym_restart != null)
                    {        // new music
                        curvoice.sym_start = curvoice.sym_restart;
                        curvoice.sym_restart = null;
                    }
                    s = curvoice.sym_start;
                    if (s == null)
                        s = curvoice.sym;
                    if (s == null)
                    {
                        syntax(1, "s: without music");
                        return;
                    }
                }

                /* scan the symbol line */
                i = 0;
                while (true)
                {
                    while (p[i] == ' ' || p[i] == '\t')
                        i++;
                    c = p[i];
                    if (c == 0)
                        break;
                    switch (c)
                    {
                        case '|':
                            while (s != null && s.type != C.BAR)
                                s = s.next;
                            if (s == null)
                            {
                                syntax(1, "Not enough measure bars for symbol line");
                                return;
                            }
                            s = s.next;
                            i++;
                            continue;
                        case '!':
                        case '"':
                            j = ++i;
                            i = p.IndexOf(c, j);
                            if (i < 0)
                            {
                                syntax(1, c == '!' ?
                                    "No end of decoration" :
                                    "No end of chord symbol/annotation");
                                i = p.Length;
                                continue;
                            }
                            d = p.Substring(j - 1, i + 1 - j);
                            break;
                        case '*':
                            break;
                        default:
                            d = (int)p[i];
                            if (d < 128)
                            {
                                d = char_tb[d];
                                if (d.Length > 1
                                    && (d[0] == '!' || d[0] == '"'))
                                {
                                    c = d[0];
                                    break;
                                }
                            }
                            syntax(1, errs.bad_char, c);
                            break;
                    }

                    /* store the element in the next note */
                    while (s != null && s.type != C.NOTE)
                        s = s.next;
                    if (s == null)
                    {
                        syntax(1, "Too many elements in symbol line");
                        return;
                    }
                    switch (c)
                    {
                        default:
                            //        case '*':
                            break;
                        case '!':
                            a_dcn.Add(d.Substring(1, d.Length - 2));
                            deco_cnv(s, s.prev);
                            break;
                        case '"':
                            parse_gchord(d);
                            if (a_gch != null)            // if no error
                                csan_add(s);
                            break;
                    }
                    s = s.next;
                    i++;
                }
                curvoice.sym_cont = s;
            }

        /* -- parse a lyric (vocal) line (w:) -- */
        public void get_lyrics(string text, bool cont)
        {
            VoiceItem s;
            string word, p;
            int i, j, ly, ln, c;
            Font  cf;
            Font dfnt;

            if (curvoice.ignore)
                return;
            if ((curvoice.pos.voc & 0x07) != C.SL_HIDDEN)
                curvoice.have_ly = true;

            // get the starting symbol of the lyrics
            if (cont)
            {                    // +:
                s = curvoice.lyric_cont;
                if (s == null)
                {
                    syntax(1, "+: lyric without music");
                    return;
                }
                dfnt = get_font("vocal");
                if (gene.deffont != dfnt)
                {    // if vocalfont change
                    if (gene.curfont == gene.deffont)
                        gene.curfont = dfnt;
                    gene.deffont = dfnt;
                }
            }
            else
            {
                set_font("vocal");
                if (curvoice.lyric_restart != null)
                {        // new music
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

            /* scan the lyric line */
            p = text;
            i = 0;
            cf = gene.curfont;
            while (true)
            {
                while (p[i] == ' ' || p[i] == '\t')
                    i++;
                if (p[i] == 0)
                    break;
                ln = 0;
                j = parse.istart + i + 2;    // start index
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
                        ln = p[i] == '-' ? 2 : 3;    // line continuation
                        break;
                    case '*':
                        word = "";
                        break;
                    default:
                        word = "";
                        while (true)
                        {
                            if (p[i] == 0)
                                break;
                            switch (p[i])
                            {
                                case '_':
                                case '*':
                                case '|':
                                    i--;
                                    break;
                                case ' ':
                                case '\t':
                                    break;
                                case '~':
                                    word += ' ';
                                    i++;
                                    continue;
                                case '-':
                                    ln = 1;        // start of line
                                    break;
                                case '\\':
                                    if (p[++i] == 0)
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
                                    // fall thru
                                    break;
                                default:
                                    word += p[i++];
                                    continue;
                            }
                            break;
                        }
                        break;
                }

                /* store the word in the next note */
                while (s != null && s.type != C.NOTE)
                    s = s.next;
                if (s == null)
                {
                    syntax(1, "Too many words in lyric line");
                    return;
                }
                if (!string.IsNullOrEmpty(word)
                    && (s.pos.voc & 0x07) != C.SL_HIDDEN)
                {
                    ly = new Lyric
                    {
                        t = word,
                        font = cf,
                        istart = j,
                        iend = j + word.Length
                    };
                    if (ln != 0)
                        ly.ln = ln;
                    if (s.a_ly == null)
                        s.a_ly = new int[100];
                    s.a_ly[curvoice.lyric_line] = ly;
                    cf = gene.curfont;
                }
                s = s.next;
                i++;
            }
            curvoice.lyric_cont = s;
        }

        // install the words under a note
        // (this function is called during the generation)
        public void ly_set(VoiceItem s)
        {
            int i, j, ly, d, p, w, ly, ln, hyflag, lflag, x0, shift;
            Voice s1, s2, s3 = s;
            var wx = 0;
            var wl = 0;
            var n = 0;
            var dx = 0;
            var a_ly = s.a_ly;
            var align = 0;

            for (s2 = s.ts_next; s2 != null; s2 = s2.ts_next)
            {
                if (s2.shrink)
                {
                    dx += s2.shrink;
                    n++;
                }
                if (s2.bar_type)
                {
                    dx += 3;
                    break;
                }
                if (s2.a_ly == null)
                    continue;
                i = s2.a_ly.Length;
                while (--i >= 0)
                {
                    ly = s2.a_ly[i];
                    if (ly == null)
                        continue;
                    if (!ly.ln || ly.ln < 2)
                        break;
                }
                if (i >= 0)
                    break;
            }

            for (i = 0; i < a_ly.Length; i++)
            {
                ly = a_ly[i];
                if (ly == null)
                    continue;
                gene.curfont = ly.font;
                ly.t = str2svg(ly.t);
                p = ly.t.Replace(/<[^>] *>/ g, '');
                if (ly.ln >= 2)
                {
                    ly.shift = 0;
                    continue;
                }
                spw = cwid(' ') * ly.font.swfac;
                w = ly.t.wh[0];
                if (s.type == C.GRACE)
                {
                    shift = s.wl;
                }
                else if ((p[0] >= '0' && p[0] <= '9' && p.Length > 2)
                    || p[1] == ':'
                    || p[0] == '(' || p[0] == ')')
                {
                    if (p[0] == '(')
                    {
                        sz = spw;
                    }
                    else
                    {
                        j = p.IndexOf(' ');
                        set_font(ly.font);
                        if (j > 0)
                            sz = strwh(p.Substring(0, j))[0];
                        else
                            sz = w * .2;
                    }
                    shift = (w - sz) * .4;
                    if (shift > 14)
                        shift = 14;
                    shift += sz;
                    if (p[0] >= '0' && p[0] <= '9')
                    {
                        if (shift > align)
                            align = shift;
                    }
                }
                else
                {
                    shift = w * .4;
                    if (shift > 14)
                        shift = 14;
                }
                ly.shift = shift;
                if (shift > wl)
                    wl = shift;
                w += spw * 1.5;
                w -= shift;
                if (w > wx)
                    wx = w;
            }

            while (!s3.seqst)
                s3 = s3.ts_prev;
            if (s3.ts_prev != null && s3.ts_prev.bar_type)
                wl -= 4;
            if (s3.wl < wl)
            {
                s3.shrink += wl - s3.wl;
                s3.wl = wl;
            }
            dx -= 6;
            if (dx < wx)
            {
                dx = (wx - dx) / n;
                s1 = s.ts_next;
                while (true)
                {
                    if (s1.shrink)
                    {
                        s1.shrink += dx;
                        s3.wr += dx;
                        s3 = s1;
                    }
                    if (s1 == s2)
                        break;
                    s1 = s1.ts_next;
                }
            }
            if (align > 0)
            {
                for (i = 0; i < a_ly.Length; i++)
                {
                    ly = a_ly[i];
                    if (ly != null && ly.t[0] >= '0' && ly.t[0] <= '9')
                        ly.shift = align;
                }
            }
        }

        /* -- draw the lyrics under (or above) notes -- */
        /* (the staves are not yet defined) */
        public void draw_lyric_line(PageVoiceTune p_voice, int j, double y)
        {
            int s2;
            int p, lastx, w, ly, lyl, ln, hyflag, lflag, x0, shift;

            if ((p_voice.hy_st & (1 << j)) != 0)
            {
                hyflag = true;
                p_voice.hy_st &= ~(1 << j);
            }
            for (s = p_voice.sym; ; s = s.next)
                if (s.type != C.CLEF
                    && s.type != C.KEY && s.type != C.METER)
                    break;
            lastx = s.prev != null ? s.prev.x : tsfirst.x;
            x0 = 0;
            for (; s != null; s = s.next)
            {
                if (s.a_ly != null)
                    ly = s.a_ly[j];
                else
                    ly = null;
                if (ly == null)
                {
                    switch (s.type)
                    {
                        case C.REST:
                        case C.MREST:
                            if (lflag)
                            {
                                out_wln(lastx + 3, y, x0 - lastx);
                                lflag = false;
                                lastx = s.x + s.wr;
                            }
                            break;
                    }
                    continue;
                }
                if (ly.font != gene.curfont)
                    gene.curfont = ly.font;
                p = ly.t;
                ln = ly.ln != null ? ly.ln : 0;
                w = p.wh[0];
                shift = ly.shift;
                if (hyflag)
                {
                    if (ln == 3)
                    {
                        ln = 2;
                    }
                    else if (ln < 2)
                    {
                        out_hyph(lastx, y, s.x - shift - lastx);
                        hyflag = false;
                        lastx = s.x + s.wr;
                    }
                }
                if (lflag
                    && ln != 3)
                {
                    out_wln(lastx + 3, y, x0 - lastx + 3);
                    lflag = false;
                    lastx = s.x + s.wr;
                }
                if (ln >= 2)
                {
                    if (x0 == 0 && lastx > s.x - 18)
                        lastx = s.x - 18;
                    if (ln == 2)
                        hyflag = true;
                    else
                        lflag = true;
                    x0 = s.x - shift;
                    continue;
                }
                x0 = s.x - shift;
                if (ln != 0)
                    hyflag = true;
                if (user.anno_start || user.anno_stop)
                {
                    s2 = new
                    {
                        p_v = s.p_v,
                        st = s.st,
                        istart = ly.istart,
                        iend = ly.iend,
                        ts_prev = s,
                        ts_next = s.ts_next,
                        x = x0,
                        y = y,
                        ymn = y,
                        ymx = y + gene.curfont.size,
                        wl = 0,
                        wr = w
                    };
                    anno_start(s2, "lyrics");
                }
                xy_str(x0, y, p);
                anno_stop(s2, "lyrics");
                lastx = x0 + w;
            }
            if (hyflag)
            {
                hyflag = false;
                x0 = realwidth - 10;
                if (x0 < lastx + 10)
                    x0 = lastx + 10;
                out_hyph(lastx, y, x0 - lastx);
                if (p_voice.s_next != null && p_voice.s_next.fmt.hyphencont)
                    p_voice.hy_st |= (1 << j);
            }
            for (p_voice.s_next; s != null; s = s.next)
            {
                if (s.type == C.NOTE)
                {
                    if (s.a_ly == null)
                        break;
                    ly = s.a_ly[j];
                    if (ly != null && ly.ln == 3)
                    {
                        lflag = true;
                        x0 = realwidth - 15;
                        if (x0 < lastx + 12)
                            x0 = lastx + 12;
                    }
                    break;
                }
            }
            if (lflag)
            {
                out_wln(lastx + 3, y, x0 - lastx + 3);
                lflag = false;
            }
        }

        public double draw_lyrics(PageVoiceTune p_voice, int nly, List<int> a_h, double y, int incr)
        {
            int j
            double top;
            double sc = staff_tb[p_voice.st].staffscale;

            set_font("vocal");
            if (incr > 0)
            {
                if (y > -tsfirst.fmt.vocalspace)
                    y = -tsfirst.fmt.vocalspace;
                y *= sc;
                for (j = 0; j < nly; j++)
                {
                    y -= a_h[j] * 1.1;
                    draw_lyric_line(p_voice, j,
                        y + a_h[j] * .22);
                }
                return y / sc;
            }

            top = staff_tb[p_voice.st].topbar + tsfirst.fmt.vocalspace;
            if (y < top)
                y = top;
            y *= sc;
            for (j = nly; --j >= 0;)
            {
                draw_lyric_line(p_voice, j, y + a_h[j] * .22);
                y += a_h[j] * 1.1;
            }
            return y / sc;
        }

        // -- draw all the lyrics --
        /* (the staves are not yet defined) */
        public void draw_all_lyrics()
        {
            VoiceItem s;
            Voice p_voice;
            double x, y, w, a_ly, ly;
            int v, nly, i ;
            var lyst_tb = new Dictionary<int, Dictionary<string, int>>();
            int nv = voice_tb.Count;
            var h_tb = new Dictionary<int, List<int>>();
            var nly_tb = new Dictionary<int, int>();
            var above_tb = new Dictionary<int, bool>();
            var rv_tb = new Dictionary<int, int>();
            double top = 0;
            double bot = 0;
            int st = -1;

            for (v = 0; v < nv; v++)
            {
                p_voice = voice_tb[v];
                if (p_voice.sym == null)
                    continue;
                if (p_voice.st != st)
                {
                    top = 0;
                    bot = 0;
                    st = p_voice.st;
                }
                nly = 0;
                if (p_voice.have_ly)
                {
                    if (!h_tb.ContainsKey(v))
                        h_tb[v] = new List<int>();
                    for (s = p_voice.sym; s != null; s = s.next)
                    {
                        a_ly = s.a_ly;
                        if (a_ly == null)
                            continue;
                        x = s.x;
                        w = 10;
                        for (i = 0; i < a_ly.Length; i++)
                        {
                            ly = a_ly[i];
                            if (ly != null)
                            {
                                x -= ly.shift;
                                w = ly.t.wh[0];
                                break;
                            }
                        }
                        y = y_get(p_voice.st, 1, x, w);
                        if (top < y)
                            top = y;
                        y = y_get(p_voice.st, 0, x, w);
                        if (bot > y)
                            bot = y;
                        while (nly < a_ly.Length)
                            h_tb[v][nly++] = 0;
                        for (i = 0; i < a_ly.Length; i++)
                        {
                            ly = a_ly[i];
                            if (ly == null)
                                continue;
                            if (!h_tb[v][i]
                                || ly.t.wh[1] > h_tb[v][i])
                                h_tb[v][i] = ly.t.wh[1];
                        }
                    }
                }
                else
                {
                    y = y_get(p_voice.st, 1, 0, realwidth);
                    if (top < y)
                        top = y;
                    y = y_get(p_voice.st, 0, 0, realwidth);
                    if (bot > y)
                        bot = y;
                }
                if (!lyst_tb.ContainsKey(st))
                    lyst_tb[st] = new Dictionary<string, int>();
                lyst_tb[st]["top"] = top;
                lyst_tb[st]["bot"] = bot;
                nly_tb[v] = nly;
                if (nly == 0)
                    continue;
                if (p_voice.pos.voc != null)
                    above_tb[v] = (p_voice.pos.voc & 0x07) == C.SL_ABOVE;
                else if (voice_tb[v + 1] != null
                    && voice_tb[v + 1].st == st
                    && voice_tb[v + 1].have_ly)
                    above_tb[v] = true;
                else
                    above_tb[v] = false;
                if (above_tb[v])
                    lyst_tb[st]["a"] = true;
                else
                    lyst_tb[st]["b"] = true;
            }

            i = 0;
            for (v = 0; v < nv; v++)
            {
                p_voice = voice_tb[v];
                if (p_voice.sym == null)
                    continue;
                if (!p_voice.have_ly)
                    continue;
                if (above_tb[v])
                {
                    rv_tb[i++] = v;
                    continue;
                }
                st = p_voice.st;
                set_dscale(st, true);
                if (nly_tb[v] > 0)
                    lyst_tb[st]["bot"] = draw_lyrics(p_voice, nly_tb[v],
                        h_tb[v],
                        lyst_tb[st]["bot"], 1);
            }

            while (--i >= 0)
            {
                v = rv_tb[i];
                p_voice = voice_tb[v];
                st = p_voice.st;
                set_dscale(st, true);
                lyst_tb[st]["top"] = draw_lyrics(p_voice, nly_tb[v],
                    h_tb[v],
                    lyst_tb[st]["top"], -1);
            }

            for (v = 0; v < nv; v++)
            {
                p_voice = voice_tb[v];
                if (p_voice.sym == null)
                    continue;
                st = p_voice.st;
                if (lyst_tb[st]["a"])
                {
                    top = lyst_tb[st]["top"] + 2;
                    for (s = p_voice.sym; s != null; s = s.next)
                    {
                        if (s.a_ly != null)
                        {
                            y_set(st, 1, s.x - 2, 10, top);
                        }
                    }
                }
                if (lyst_tb[st]["b"])
                {
                    bot = lyst_tb[st]["bot"] - 2;
                    if (nly_tb[p_voice.v] > 0)
                    {
                        for (s = p_voice.sym; s != null; s = s.next)
                        {
                            if (s.a_ly != null)
                            {
                                y_set(st, 0, s.x - 2, 10, bot);
                            }
                        }
                    }
                    else
                    {
                        y_set(st, 0, 0, realwidth, bot);
                    }
                }
            }
        }
  



}
}
