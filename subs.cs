using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


var sw_tb = new Float32Array([
        .000, .000, .250, .333
        ]),
    // sans-serif
    ssw_tb = new Float32Array([
        .000, .000, 1.015, .667, .667, .722, .722, .667, .611, .778
    ]),
    // monospace
    mw_tb = new Float32Array([
        .0, .0,
        .52, .52, .52
   ])
var info_font_init = {
    A: "info",
    C: "composer",
    O: "composer",
    P: "parts",
    Q: "tempo",
    R: "info",
    T: "title",
    X: "title"
}

    var sheet
var add_fstyle = typeof document != "undefined" ?
	function (s) {
    var e

        if (cfmt.fullsvg)
        font_style += "\n" + s

        if (!sheet)
    {
        if (abc2svg.sheet)
        {   // if styles from a previous generation
            sheet = abc2svg.sheet

                e = sheet.cssRules.length

                while (--e >= 0)
                sheet.deleteRule(e)

            }
        else
        {
            e = document.createElement('style')

                document.head.appendChild(e)

                abc2svg.sheet = sheet = e.sheet

            }
    }
    s = s.match(/[^{]+{[^}]+}/ g)	// insert each style
		while (1)
    {
        e = s.shift()

            if (!e)
            break

            sheet.insertRule(e, sheet.cssRules.length)

        }
} // add_fstyle()
	: function(s) { font_style += "\n" + s }







var sw_tb = new Float32Array([
        .000, .000, .250, .333
        ]),
    // sans-serif
    ssw_tb = new Float32Array([
        .000, .000, 1.015, .667, .667, .722, .722, .667, .611, .778
    ]),
    // monospace
    mw_tb = new Float32Array([
        .0, .0,
        .52, .52, .52
   ])
var info_font_init = {
    A: "info",
	C: "composer",
	O: "composer",
	P: "parts",
	Q: "tempo",
	R: "info",
	T: "title",
	X: "title"
}
var strwh;
function xy_str(x:number, y: number,str: string, action?, w?, wh?)
{
    if (!wh) wh = "__"

}

namespace autocad_part2
{
    public class Abc
    {


        static void Main(string[] args)
        {
            double[] sheet;
            Func<string, string> add_fstyle = (s) =>
            {
                string e;
                if (cfmt.fullsvg)
                    font_style += "\n" + s;
                if (sheet == null)
                {
                    if (abc2svg.sheet != null)
                    {
                        sheet = abc2svg.sheet;
                        e = sheet.cssRules.Length.ToString();
                        while (--e >= 0)
                            sheet.deleteRule(e);
                    }
                    else
                    {
                        e = document.createElement('style');
                        document.head.appendChild(e);
                        abc2svg.sheet = sheet = e.sheet;
                    }
                }
                s = s.match(/[^{]+{[^}]+}/ g);
            while (true)
            {
                e = s.shift();
                if (!e)
                    break;
                sheet.insertRule(e, sheet.cssRules.Length);
            }
        };

        double[] sw_tb = new double[] { .000, .000, .250, .333 };
        double[] ssw_tb = new double[] { .000, .000, 1.015, .667, .667, .722, .722, .667, .611, .778 };
        double[] mw_tb = new double[] { .0, .0, .52, .52, .52 };
        Dictionary<string, string> info_font_init = new Dictionary<string, string>()
            {
                { "A", "info" },
                { "C", "composer" },
                { "O", "composer" },
                { "P", "parts" },
                { "Q", "tempo" },
                { "R", "info" },
                { "T", "title" },
                { "X", "title" }
            };

        Func<char, string, double> cwid = (c, font) =>
        {
            int i = c.charCodeAt(0);

            if (i >= 0x80)
            {
                if (i >= 0x300 && i < 0x370)
                    return 0;
                i = 0x61;
            }
            return (font || gene.curfont).cw_tb[i];
        };

        Func<char, double> cwidf = (c) =>
        {
            return cwid(c) * gene.curfont.swfac;
        };

        double[] strwh;

        Action strwhFunc = () =>
        {
            if (typeof document != "undefined")
            {
                var el;

                strwh = (str) =>
                {
                    if (str.wh)
                        return str.wh;
                    if (el == null)
                    {
                        el = document.createElement('text');
                        el.style.position = 'absolute';
                        el.style.top = '-1000px';
                        el.style.padding = '0';
                        el.style.visibility = "hidden";
                        document.body.appendChild(el);
                    }

                    var c,
                        font = gene.curfont,
                        h = font.size,
                        w = 0,
                        n = str.length,
                        i0 = 0,
                        i = 0;

                    el.className = font_class(font);
                    el.style.lineHeight = 1;

                    if (typeof str == "object")
                    {
                        el.innerHTML = str;
                        str.wh = [el.clientWidth, el.clientHeight];
                        return str.wh;
                    }

                    str = str.replace(/<|>| &[^&\s] *?;| &/ g, (c) =>
                    {
                        switch (c)
                        {
                            case '<': return "&lt;";
                            case '>': return "&gt;";
                            case '&': return "&amp;";
                        }
                        return c;
                    });

                    while (true)
                    {
                        i = str.indexOf('$', i);
                        if (i >= 0)
                        {
                            c = str[i + 1];
                            if (c == '0')
                            {
                                font = gene.deffont;
                            }
                            else if (c >= '1' && c <= '9')
                            {
                                font = get_font("u" + c);
                            }
                            else
                            {
                                i++;
                                continue;
                            }
                        }

                        el.innerHTML = str.slice(i0, i >= 0 ? i : undefined);
                        w += el.clientWidth;
                        if (el.clientHeight > h)
                            h = el.clientHeight;
                        if (i < 0)
                            break;
                        el.style.font = style_font(font).slice(5);
                        i += 2;
                        i0 = i;
                    }
                    return [w, h];
                };
            }
            else
            {
                strwh = (str) =>
                {
                    var font = gene.curfont,
                        swfac = font.swfac,
                        h = font.size,
                        w = 0,
                        i, j, c,
                        n = str.length;

                    for (i = 0; i < n; i++)
                    {
                        c = str[i];
                        switch (c)
                        {
                            case '$':
                                c = str[i + 1];
                                if (c == '0')
                                {
                                    font = gene.deffont;
                                }
                                else if (c >= '1' && c <= '9')
                                {
                                    font = get_font("u" + c);
                                }
                                else
                                {
                                    c = '$';
                                    break;
                                }
                                i++;
                                swfac = font.swfac;
                                if (font.size > h)
                                    h = font.size;
                                continue;
                            case '&':
                                if (str[i + 1] == ' ')
                                    break;
                                j = str.indexOf(';', i);
                                if (j > 0 && j - i < 10)
                                {
                                    i = j;
                                    c = 'a';
                                }
                                break;
                        }
                        w += cwid(c, font) * swfac;
                    }
                    return [w, h];
                };
            }
        };

        strwhFunc();

        Func<string, object> str2svg = (str) =>
        {
            if (typeof str == "object")
                return str;

            int n_font;
            double[] wh;
            var o_font = gene.deffont;
            var c_font = gene.curfont;
            var o = "";

            string tspan(int nf, int of)
            {
                string cl;

                if (nf.class1
                                && nf.name == of.name
                            && nf.size == of.size
                            && nf.weight == of.weight
                            && nf.style == of.style)
                    cl = nf.class1;
                else
                    cl = font_class(nf);

                return "<tspan\n\tclass=\"" + cl + "\">";
            }

            if (c_font != o_font)
                o = tspan(c_font, o_font);
            o += str.replace("/<|>|&[^&\s]*?;|&|\$./g", (c) =>
                        {
                switch (c)
                {
                    case '<': return "&lt;";
                    case '>': return "&gt;";
                    case '&': return "&amp;";
                    default:
                        if (c[0] != '$')
                            break;
                        if (c[1] == '0')
                            n_font = gene.deffont;
                        else if (c[1] >= '1' && c[1] <= '9')
                            n_font = get_font("u" + c[1]);
                        else
                            break;
                        c = '';
                        if (n_font == c_font)
                            return c;
                        if (c_font != o_font)
                            c = "</tspan>";
                        c_font = n_font;
                        if (c_font == o_font)
                            return c;
                        return c + tspan(c_font, o_font);
                }
                return c;
            });
            if (c_font != o_font)
                o += "</tspan>";

            o = new String(o);
            if (typeof document != "undefined")
                strwh(o);
            else
                o.wh = strwh(str);

            gene.curfont = c_font;

            return o;
        };

        Action<string> set_font = (xxx) =>
        {
            if (typeof xxx == "string")
                xxx = get_font(xxx);
            gene.curfont = gene.deffont = xxx;
        };

        Action<string> out_str = (str) =>
        {
            output += str2svg(str);
        };

        Action<double, double, string, string, double, double> xy_str = (x, y, str, action, w, wh) =>
        {
            if (wh == null)
                wh = str.wh || strwh(str);

            output += '<text class="' + font_class(gene.deffont);
            if (action != 'j' && str.length > 5 && gene.deffont.wadj)
                output += '" lengthAdjust="' + gene.deffont.wadj +
                    '" textLength="' + wh[0].toFixed(1);
            output += '" x="';
            out_sxsy(x, '" y="', y);
            switch (action)
            {
                case 'c':
                    output += '" text-anchor="middle">';
                    break;
                case 'j':
                    output += '" textLength="' + w.toFixed(1) + '">';
                    break;
                case 'r':
                    output += '" text-anchor="end">';
                    break;
                default:
                    output += '">';
                    break;
            }
            out_str(str);
            output += "</text>\n";
        };

        Action<string, bool> trim_title = (title, is_subtitle) =>
        {
            int i;

            if (cfmt.titletrim)
            {
                i = title.lastIndexOf(", ");
                if (i < 0 || title[i + 2] < 'A' || title[i + 2] > 'Z')
                {
                    i = 0;
                }
                else if (cfmt.titletrim == 1)
                {
                    if (i < title.length - 7 || title.indexOf(' ', i + 3) >= 0)
                        i = 0;
                }
                else
                {
                    if (i < title.length - cfmt.titletrim - 2)
                        i = 0;
                }
                if (i != 0)
                    title = title.slice(i + 2).trim() + ' ' + title.slice(0, i);
            }
            if (!is_subtitle && cfmt.writefields.indexOf('X') >= 0)
                title = info.X + '.  ' + title;
            if (cfmt.titlecaps)
                return title.ToUpper();
            return title;
        };

        Func<double> get_lwidth = () =>
        {
            if (img.chg)
                set_page();
            return img.lw;
        };

        Action<string, bool> write_title = (title, is_subtitle) =>
        {
            double h, wh;

            if (string.IsNullOrEmpty(title))
                return;
            set_page();
            title = trim_title(title, is_subtitle);
            if (is_subtitle)
            {
                set_font("subtitle");
                h = cfmt.subtitlespace;
            }
            else
            {
                set_font("title");
                h = cfmt.titlespace;
            }
            wh = strwh(title);
            wh[1] += gene.curfont.pad * 2;
            vskip(wh[1] + h + gene.curfont.pad);
            h = gene.curfont.pad + wh[1] * .22;
            if (cfmt.titleleft)
                xy_str(0, h, title, null, null, wh);
            else
                xy_str(get_lwidth() / 2, h, title, "c", null, wh);
        };

        Action<string> put_inf2r = (x, y, str1, str2, action) =>
        {
            if (string.IsNullOrEmpty(str1))
            {
                if (string.IsNullOrEmpty(str2))
                    return;
                str1 = str2;
                str2 = null;
            }
            if (string.IsNullOrEmpty(str2))
                xy_str(x, y, str1, action);
            else
                xy_str(x, y, str1 + ' (' + str2 + ')', action);
        };

        Action<string, char> write_text = (text, action) =>
        {
            if (action == 's')
                return;
            set_page();

            double[] wh;
            var font = gene.curfont;
            double strlw = get_lwidth();
            double sz = gene.curfont.size;
            double lineskip = sz * cfmt.lineskipfac;
            double parskip = sz * cfmt.parskipfac;
            int i, j, x, words, w, k, ww, str;

            switch (action)
            {
                default:
                    font = gene.curfont;
                    switch (action)
                    {
                        case 'c': x = strlw / 2; break;
                        case 'r': x = strlw - font.pad; break;
                        default: x = font.pad; break;
                    }
                    j = 0;
                    while (true)
                    {
                        i = text.indexOf('\n', j);
                        if (i == j)
                        {
                            vskip(parskip);
                            blk_flush();
                            use_font(gene.curfont);
                            while (text[i + 1] == '\n')
                            {
                                vskip(lineskip);
                                i++;
                            }
                            if (i == text.length)
                                break;
                        }
                        else
                        {
                            if (i < 0)
                                str = text.slice(j);
                            else
                                str = text.slice(j, i);
                            ww = strwh(str);
                            vskip(ww[1] * cfmt.lineskipfac + font.pad * 2);
                            xy_str(x, font.pad + ww[1] * .2, str, action);
                            if (i < 0)
                                break;
                        }
                        j = i + 1;
                    }
                    vskip(parskip);
                    blk_flush();
                    break;
                case 'f':
                case 'j':
                    j = 0;
                    while (true)
                    {
                        i = text.indexOf('\n\n', j);
                        if (i < 0)
                            words = text.slice(j);
                        else
                            words = text.slice(j, i);
                        words = words.split(/\s +/);
                        w = k = wh = 0;
                        for (j = 0; j < words.length; j++)
                        {
                            ww = strwh(words[j] + ' ');
                            w += ww[0];
                            if (w >= strlw)
                            {
                                vskip(wh * cfmt.lineskipfac);
                                xy_str(0, ww[1] * .2, words.slice(k, j).join(' '), action, strlw);
                                k = j;
                                w = ww[0];
                                wh = 0;
                            }
                            if (ww[1] > wh)
                                wh = ww[1];
                        }
                        if (w != 0)
                        {
                            vskip(wh * cfmt.lineskipfac);
                            xy_str(0, ww[1] * .2, words.slice(k).join(' '));
                        }
                        vskip(parskip);
                        blk_flush();
                        if (i < 0)
                            break;
                        while (text[i + 2] == '\n')
                        {
                            vskip(lineskip);
                            i++;
                        }
                        if (i == text.length)
                            break;
                        use_font(gene.curfont);
                        j = i + 2;
                    }
                    break;
            }
        };

        Action put_history = () =>
        {
            int i, j, c, str, font, h, w, wh, head;
            var names = cfmt.infoname.split("\n");
            int n = names.length;

            for (i = 0; i < n; i++)
            {
                c = names[i][0];
                if (cfmt.writefields.indexOf(c) < 0)
                    continue;
                str = info[c];
                if (!str)
                    continue;
                if (!font)
                {
                    font = true;
                    set_font("history");
                    vskip(cfmt.textspace);
                    h = gene.curfont.size * cfmt.lineskipfac;
                }
                head = names[i].slice(2);
                if (head[0] == '"')
                    head = head.slice(1, -1);
                vskip(h);
                wh = strwh(head);
                xy_str(0, wh[1] * .22, head, null, null, wh);
                w = wh[0];
                str = str.split('\n');
                xy_str(w, wh[1] * .22, str[0]);
                for (j = 1; j < str.length; j++)
                {
                    if (!str[j])
                    {
                        vskip(gene.curfont.size * cfmt.parskipfac);
                        continue;
                    }
                    vskip(h);
                    xy_str(w, wh[1] * .22, str[j]);
                }
                vskip(h * cfmt.parskipfac);
                use_font(gene.curfont);
            }
        };

        /***************************************/


        double[] sw_tb = new double[] { .000f, .000f, .250f, .333f };
        double[] ssw_tb = new double[] { .000f, .000f, 1.015f, .667f, .667f, .722f, .722f, .667f, .611f, .778f };
        double[] mw_tb = new double[] { .0f, .0f, .52f, .52f, .52f };
        var info_font_init = new Dictionary<char, string>()
        {
            { 'A', "info" },
            { 'C', "composer" },
            { 'O', "composer" },
            { 'P', "parts" },
            { 'Q', "tempo" },
            { 'R', "info" },
            { 'T', "title" },
            { 'X', "title" }
        };
        string strwh;

        void xy_str(double x, double y, string str, object action = null, object w = null, object wh = null)
        {
            if (wh == null) wh = "__";
        }

        void put_words(string words)
        {
            int p, i, j, nw, w, lw, x1, x2, i1, i2, do_flush;
            int maxn = 0; // max number of characters per line
            int n = 1; // number of verses

            void put_wline(int p, int x)
            {
                int i = 0;
                int k = 0;

                if (p[0] == '$' && p[1] >= '0' && p[1] <= '9')
                {
                    gene.curfont = p[1] == '0' ? gene.deffont : get_font("u" + p[1]);
                    p = p.Slice(2);
                }

                if ((p[i] >= '0' && p[i] <= '9') || p[i + 1] == '.')
                {
                    while (i < p.Length)
                    {
                        i++;
                        if (p[i] == ' ' || p[i - 1] == ':' || p[i - 1] == '.')
                            break;
                    }
                    k = i;
                    while (p[i] == ' ')
                        i++;
                }

                double y = gene.curfont.size * .22f; // descent
                if (k != 0)
                    xy_str(x, y, p.Slice(0, k), 'r');
                if (i < p.Length)
                    xy_str(x + 5, y, p.Slice(i), 'l');
            }

            set_font("words");
            vskip(cfmt.wordsspace);
            svg_flush();

            words = words.Split('\n');
            nw = words.Length;
            for (i = 0; i < nw; i++)
            {
                p = words[i];
                if (p == null)
                {
                    while (i + 1 < nw && !words[i + 1])
                        i++;
                    n++;
                }
                else if (p.Length > maxn)
                {
                    maxn = p.Length;
                    i1 = i;
                }
            }

            w = get_lwidth() / 2; // half line width
            lw = strwh(words[i1])[0];
            i1 = i2 = 0;
            if (lw < w)
            {
                j = n >> 1;
                for (i = 0; i < nw; i++)
                {
                    p = words[i];
                    if (p == null)
                    {
                        if (--j <= 0)
                            i1 = i;
                        while (i + 1 < nw && !words[i + 1])
                            i++;
                        if (j <= 0)
                        {
                            i2 = i + 1;
                            break;
                        }
                    }
                }
                n >>= 1;
            }
            if (i2 != 0)
            {
                x1 = (w - lw) / 2 + 10;
                x2 = x1 + w;
            }
            else
            {
                x2 = w - lw / 2 + 10;
            }

            do_flush = 1;
            for (i = 0; i < i1 || i2 < nw; i++, i2++)
            {
                vskip(cfmt.lineskipfac * gene.curfont.size);
                if (i < i1)
                {
                    p = words[i];
                    if (p != null)
                        put_wline(p, x1);
                    else
                        use_font(gene.curfont);
                }
                if (i2 < nw)
                {
                    p = words[i2];
                    if (p != null)
                    {
                        put_wline(p, x2);
                    }
                    else
                    {
                        if (--n == 0)
                        {
                            if (i < i1)
                            {
                                n++;
                            }
                            else if (i2 < nw - 1)
                            {
                                x2 = w - lw / 2 + 10;
                                svg_flush();
                            }
                        }
                    }
                }

                if (!words[i + 1] && !words[i2 + 1])
                {
                    if (do_flush != 0)
                    {
                        svg_flush();
                        do_flush = 0;
                    }
                }
                else
                {
                    do_flush = 1;
                }
            }
        }

        void put_history()
        {
            int i, j, c, str, font, h, w, wh, head;
            string[] names = cfmt.infoname.Split("\n");
            int n = names.Length;

            for (i = 0; i < n; i++)
            {
                c = names[i][0];
                if (cfmt.writefields.IndexOf(c) < 0)
                    continue;
                str = info[c];
                if (str == null)
                    continue;
                if (font == null)
                {
                    font = true;
                    set_font("history");
                    vskip(cfmt.textspace);
                    h = gene.curfont.size * cfmt.lineskipfac;
                }
                head = names[i].Slice(2);
                if (head[0] == '"')
                    head = head.Slice(1, -1);
                vskip(h);
                wh = strwh(head);
                xy_str(0, wh[1] * .22, head, null, null, wh);
                w = wh[0];
                str = str.Split('\n');
                xy_str(w, wh[1] * .22, str[0]);
                for (j = 1; j < str.Length; j++)
                {
                    if (str[j] == null)
                    {
                        vskip(gene.curfont.size * cfmt.parskipfac);
                        continue;
                    }
                    vskip(h);
                    xy_str(w, wh[1] * .22, str[j]);
                }
                vskip(h * cfmt.parskipfac);
                use_font(gene.curfont);
            }
        }

        var info_font_init = new Dictionary<char, string>()
        {
            { 'A', "info" },
            { 'C', "composer" },
            { 'O', "composer" },
            { 'P', "parts" },
            { 'Q', "tempo" },
            { 'R', "info" },
            { 'T', "title" },
            { 'X', "title" }
        };

        void write_headform(int lwidth)
        {
            int c, font, font_name, align, x, y, sz, w;
            var info_val = new Dictionary<char, string>();
            var info_font = info_font_init.ToDictionary(entry => entry.Key, entry => entry.Value);
            var info_sz = new Dictionary<char, int>()
            {
                { 'A', cfmt.infospace },
                { 'C', cfmt.composerspace },
                { 'O', cfmt.composerspace },
                { 'R', cfmt.infospace },
                { 'T', null }
            };
            var info_nb = new Dictionary<char, int>();

            string fmt = "";
            string p = cfmt.titleformat;
            int j = 0;
            int i = 0;

            while (true)
            {
                while (p[i] == ' ')
                    i++;
                c = p[i++];
                if (c == null)
                    break;
                if (c < 'A' || c > 'Z')
                {
                    switch (c)
                    {
                        case '+':
                            align = '+';
                            c = p[i++];
                            break;
                        case ',':
                            fmt += '\n';
                            goto case default;
                        default:
                            continue;
                        case '<':
                            align = 'l';
                            c = p[i++];
                            break;
                        case '>':
                            align = 'r';
                            c = p[i++];
                            break;
                    }
                }
                else
                {
                    switch (p[i])
                    {
                        case '-':
                            align = 'l';
                            i++;
                            break;
                        case '1':
                            align = 'r';
                            i++;
                            break;
                        case '0':
                            i++;
                            goto case default;
                        default:
                            align = 'c';
                            break;
                    }
                }
                if (!info_val.ContainsKey(c))
                {
                    if (!info.ContainsKey(c))
                        continue;
                    info_val[c] = info[c].Split('\n');
                    info_nb[c] = 1;
                }
                else
                {
                    info_nb[c]++;
                }
                fmt += align + c;
            }
            fmt += '\n';

            int ya = cfmt.titlespace;
            int xa = lwidth * .5;
            int yb = new { l = 0, c = 0, r = 0 };
            string str;
            p = fmt;
            i = 0;
            while (true)
            {
                yb.l = yb.c = yb.r = y = 0;
                j = i;
                while (true)
                {
                    align = p[j++];
                    if (align == '\n')
                        break;
                    c = p[j++];
                    if (align == '+' || yb[align] != 0)
                        continue;
                    str = info_val[c];
                    if (str == null)
                        continue;
                    font_name = info_font[c];
                    if (font_name == null)
                        font_name = "history";
                    font = get_font(font_name);
                    sz = font.size * 1.1;
                    if (info_sz.ContainsKey(c))
                        sz += info_sz[c];
                    if (y < sz)
                        y = sz;
                    yb[align] = sz;
                }
                ya.l += y - yb.l;
                ya.c += y - yb.c;
                ya.r += y - yb.r;
                while (true)
                {
                    align = p[i++];
                    if (align == '\n')
                        break;
                    c = p[i++];
                    if (info_val[c].Length == 0)
                        continue;
                    str = info_val[c].Shift();
                    if (p[i] == '+')
                    {
                        info_nb[c]--;
                        i++;
                        c = p[i++];
                        if (info_val[c].Length != 0)
                        {
                            if (str != null)
                                str += ' ' + info_val[c].Shift();
                            else
                                str = ' ' + info_val[c].Shift();
                        }
                    }
                    font_name = info_font[c];
                    if (font_name == null)
                        font_name = "history";
                    font = get_font(font_name);
                    sz = font.size * 1.1;
                    if (info_sz.ContainsKey(c))
                        sz += info_sz[c];
                    set_font(font);
                    x = xa[align];
                    y = ya[align] + sz;

                    if (c == 'Q')
                    {
                        self.set_width(glovar.tempo);
                        if (!glovar.tempo.invis)
                        {
                            if (align != 'l')
                            {
                                tempo_build(glovar.tempo);
                                w = glovar.tempo.tempo_wh[0];

                                if (align == 'c')
                                    w *= .5;
                                x -= w;
                            }
                            writempo(glovar.tempo, x, -y);
                        }
                    }
                    else if (str != null)
                    {
                        if (c == 'T')
                            str = trim_title(str, info_font.T[0] == 's');
                        xy_str(x, -y, str, align);
                    }

                    if (c == 'T')
                    {
                        font_name = info_font.T = "subtitle";
                        info_sz.T = cfmt.subtitlespace;
                    }
                    if (info_nb[c] <= 1)
                    {
                        if (c == 'T')
                        {
                            font = get_font(font_name);
                            sz = font.size * 1.1;
                            if (info_sz.ContainsKey(c))
                                sz += info_sz[c];
                            set_font(font);
                        }
                        while (info_val[c].Length > 0)
                        {
                            y += sz;
                            str = info_val[c].Shift();
                            xy_str(x, -y, str, align);
                        }
                    }
                    info_nb[c]--;
                    ya[align] = y;
                }
                if (ya.c > ya.l)
                    ya.l = ya.c;
                if (ya.r > ya.l)
                    ya.l = ya.r;
                if (i >= p.Length)
                    break;
                ya.c = ya.r = ya.l;
            }
            vskip(ya.l);
        }

        void write_heading()
        {
            int i, j, area, composer, origin, rhythm, down1, down2;
            int lwidth = get_lwidth();

            vskip(cfmt.topspace);

            if (cfmt.titleformat != null)
            {
                write_headform(lwidth);
                vskip(cfmt.musicspace);
                return;
            }

            if (info.T != null && cfmt.writefields.IndexOf('T') >= 0)
            {
                i = 0;
                while (true)
                {
                    j = info.T.IndexOf("\n", i);
                    if (j < 0)
                    {
                        write_title(info.T.Substring(i), i != 0);
                        break;
                    }
                    write_title(info.T.Slice(i, j), i != 0);
                    i = j + 1;
                }
            }

            down1 = down2 = 0;
            if (parse.ckey.k_bagpipe && !cfmt.infoline && cfmt.writefields.IndexOf('R') >= 0)
                rhythm = info.R;
            if (rhythm != null)
            {
                set_font("composer");
                down1 = cfmt.composerspace + gene.curfont.size + 2;
                xy_str(0, -down1 + gene.curfont.size * .22, rhythm);
            }
            area = info.A;
            if (cfmt.writefields.IndexOf('C') >= 0)
                composer = info.C;
            if (cfmt.writefields.IndexOf('O') >= 0)
                origin = info.O;
            if (composer != null || origin != null || cfmt.infoline)
            {
                int xcomp, align;

                set_font("composer");
                if (cfmt.aligncomposer < 0)
                {
                    xcomp = 0;
                    align = ' ';
                }
                else if (cfmt.aligncomposer == 0)
                {
                    xcomp = lwidth * .5;
                    align = 'c';
                }
                else
                {
                    xcomp = lwidth;
                    align = 'r';
                }
                if (composer != null || origin != null)
                {
                    down2 = cfmt.composerspace + 2;
                    i = 0;
                    while (true)
                    {
                        down2 += gene.curfont.size;
                        if (composer != null)
                            j = composer.IndexOf("\n", i);
                        else
                            j = -1;
                        if (j < 0)
                        {
                            put_inf2r(xcomp, -down2 + gene.curfont.size * .22, composer != null ? composer.Substring(i) : null, origin, align);
                            break;
                        }
                        xy_str(xcomp, -down2 + gene.curfont.size * .22, composer.Slice(i, j), align);
                        i = j + 1;
                    }
                }

                rhythm = rhythm != null ? null : info.R;
                if ((rhythm != null || area != null) && cfmt.infoline)
                {
                    set_font("info");
                    down2 += cfmt.infospace + gene.curfont.size;
                    put_inf2r(lwidth, -down2 + gene.curfont.size * .22, rhythm, area, 'r');
                }
            }

            if (info.P != null && cfmt.writefields.IndexOf('P') >= 0)
            {
                set_font("parts");
                i = cfmt.partsspace + gene.curfont.size + gene.curfont.pad;
                if (down1 + i > down2)
                    down2 = down1 + i;
                else
                    down2 += i;
                xy_str(0, -down2 + gene.curfont.size * .22, info.P);
                down2 += gene.curfont.pad;
            }
            else if (down1 > down2)
            {
                down2 = down1;
            }
            vskip(down2 + cfmt.musicspace);
        }
    }
}








