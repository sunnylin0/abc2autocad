using System;
using System.Collections.Generic;
using System.Text;

namespace autocad_part2
{
    public class Program
    {
        // Constants
        const int STEM_MIN = 16;  // min stem height under beams
        const int STEM_MIN2 = 14; // ... for notes with two beams
        const int STEM_MIN3 = 12; // ... for notes with three beams
        const int STEM_MIN4 = 10; // ... for notes with four beams
        const int STEM_CH_MIN = 14; // min stem height for chords under beams
        const int STEM_CH_MIN2 = 10; // ... for notes with two beams
        const int STEM_CH_MIN3 = 9; // ... for notes with three beams
        const int STEM_CH_MIN4 = 9; // ... for notes with four beams
        const double BEAM_DEPTH = 3.2; // width of a beam stroke
        const double BEAM_OFFSET = 0.25; // pos of flat beam relative to staff line
        const int BEAM_SHIFT = 5; // shift of second and third beams
        const int BEAM_STUB = 7; // length of stub for flag under beam
        const double SLUR_SLOPE = 0.5; // max slope of a slur
        const int GSTEM = 15; // grace note stem length
        const double GSTEM_XOFF = 2.3; // x offset for grace note stem

        // Variable declarations
        static Dictionary<string, object> cache;
        static List<object> anno_a = new List<object>(); // symbols with annotations

        // Function to compute the best vertical offset for the beams
        static int b_pos(bool grace, int stem, int nflags, int b)
        {
            int top, bot, d1, d2;
            double shift = !grace ? BEAM_SHIFT : 3.5;
            double depth = !grace ? BEAM_DEPTH : 1.8;

            int rnd6(int y)
            {
                int iy = (int)Math.Round((y + 12) / 6.0) * 6 - 12;
                return iy - y;
            }

            if (stem > 0)
            {
                bot = b - (nflags - 1) * (int)shift - (int)depth;
                if (bot > 26)
                    return 0;
                top = b;
            }
            else
            {
                top = b + (nflags - 1) * (int)shift + (int)depth;
                if (top < -2)
                    return 0;
                bot = b;
            }

            d1 = rnd6(top - (int)BEAM_OFFSET);
            d2 = rnd6(bot + (int)BEAM_OFFSET);
            return Math.Abs(d1) > Math.Abs(d2) ? d2 : d1;
        }

        // Function to duplicate a note for beaming continuation
        static Dictionary<string, object> sym_dup(Dictionary<string, object> s)
        {
            Dictionary<string, object> m;
            Dictionary<string, object> note;

            s = Clone(s);
            s["invis"] = true;
            s.Remove("extra");
            s.Remove("text");
            s.Remove("a_gch");
            s.Remove("a_ly");
            s.Remove("a_dd");
            s.Remove("tp");
            s["notes"] = Clone((Dictionary<string, object>)s["notes"]);
            for (int m = 0; m <= (int)s["nhd"]; m++)
            {
                note = (Dictionary<string, object>)s["notes"][m] = Clone((Dictionary<string, object>)s["notes"][m]);
                note.Remove("a_dd");
            }
            return s;
        }

        // Clone function to create a deep copy of a dictionary
        static Dictionary<string, object> Clone(Dictionary<string, object> original)
        {
            var clone = new Dictionary<string, object>();
            foreach (var kvp in original)
            {
                if (kvp.Value is Dictionary<string, object>)
                {
                    clone[kvp.Key] = Clone((Dictionary<string, object>)kvp.Value);
                }
                else
                {
                    clone[kvp.Key] = kvp.Value;
                }
            }
            return clone;
        }

        public static void Main()
        {
            // Example usage of b_pos function
            int result = b_pos(false, 10, 2, 20);
            Console.WriteLine(result);
        }
    }
    public class Abc
    {
        public bool CalculateBeam(dynamic bm, dynamic s1)
        {
            dynamic s, s2, g, notes, nflags, st, v, two_staves, two_dir;
            double x, y, ys, a, b, stem_err, max_stem_err;
            double p_min, p_max, s_closest;
            double stem_xoff, scale;
            bool visible = false;
            double dy;

            if (s1.beam_st == null)
            {
                s = SymDup(s1);
                Lkvsym(s, s1);
                Lktsym(s, s1);
                s.x -= 12;
                if (s.x > s1.prev.x + 12)
                    s.x = s1.prev.x + 12;
                s.beam_st = true;
                s.beam_end = null;
                s.tmp = true;
                s.sls = null;
                s1 = s;
            }

            notes = 0;
            nflags = 0;
            two_staves = false;
            two_dir = false;
            st = s1.st;
            v = s1.v;
            stem_xoff = s1.grace ? GSTEM_XOFF : 3.5;
            for (s2 = s1; ; s2 = s2.next)
            {
                if (s2.type == C.NOTE)
                {
                    if (s2.nflags > nflags)
                        nflags = s2.nflags;
                    notes++;
                    if (s2.st != st)
                        two_staves = true;
                    if (s2.stem != s1.stem)
                        two_dir = true;
                    if (!visible && !s2.invis && (!s2.stemless || s2.trem2))
                        visible = true;
                    if (s2.beam_end != null)
                        break;
                }
                if (s2.next == null)
                {
                    for (; ; s2 = s2.prev)
                    {
                        if (s2.type == C.NOTE)
                            break;
                    }
                    s = SymDup(s2);
                    s.next = s2.next;
                    if (s.next != null)
                        s.next.prev = s;
                    s2.next = s;
                    s.prev = s2;
                    s.ts_next = s2.ts_next;
                    if (s.ts_next != null)
                        s.ts_next.ts_prev = s;
                    s2.ts_next = s;
                    s.ts_prev = s2;
                    s.beam_st = null;
                    s.beam_end = true;
                    s.tmp = true;
                    s.sls = null;
                    s.x += 12;
                    if (s.x < realwidth - 12)
                        s.x = realwidth - 12;
                    s2 = s;
                    notes++;
                    break;
                }
            }

            if (!visible)
                return false;

            bm.s2 = s2;

            if (staff_tb[st].y == 0)
            {
                if (two_staves)
                    return false;
            }
            else
            {
                if (!two_staves)
                {
                    bm.s1 = s1;
                    bm.a = (s1.ys - s2.ys) / (s1.xs - s2.xs);
                    bm.b = s1.ys - s1.xs * bm.a + staff_tb[st].y;
                    bm.nflags = nflags;
                    return true;
                }
            }

            s_closest = s1;
            p_min = 100;
            p_max = 0;
            for (s = s1; ; s = s.next)
            {
                if (s.type != C.NOTE)
                    continue;
                if ((scale = s.p_v.scale) == 1)
                    scale = staff_tb[s.st].staffscale;
                if (s.stem >= 0)
                {
                    x = stem_xoff + s.notes[0].shhd;
                    if (s.notes[s.nhd].pit > p_max)
                    {
                        p_max = s.notes[s.nhd].pit;
                        s_closest = s;
                    }
                }
                else
                {
                    x = -stem_xoff + s.notes[s.nhd].shhd;
                    if (s.notes[0].pit < p_min)
                    {
                        p_min = s.notes[0].pit;
                        s_closest = s;
                    }
                }
                s.xs = s.x + x * scale;
                if (s == s2)
                    break;
            }

            if (s.grace && s1.fmt.flatbeams)
                a = 0;
            else if (!two_dir && notes >= 3 && s_closest != s1 && s_closest != s2)
                a = 0;
            else
            {
                y = s1.ys + staff_tb[st].y;
                a = (s2.ys + staff_tb[s2.st].y - y) / (s2.xs - s1.xs);
                if (a != 0)
                {
                    a = s1.fmt.beamslope * a / (s1.fmt.beamslope + Math.Abs(a));
                    if (a > -.04 && a < .04)
                        a = 0;
                }
            }

            b = (y + s2.ys + staff_tb[s2.st].y) / 2 - a * (s2.xs + s1.xs) / 2;

            max_stem_err = 0;
            s = s1;
            if (two_dir)
            {
                ys = ((s1.grace ? 3.5 : BEAM_SHIFT) * (nflags - 1) + BEAM_DEPTH) * .5;
                if (s1.nflags == s2.nflags) { }
                else if (s1.stem != s2.stem && s1.nflags < s2.nflags)
                    b += ys * s2.stem;
                else
                    b += ys * s1.stem;
            }
            else if (!s1.grace)
            {
                double beam_h = BEAM_DEPTH + BEAM_SHIFT * (nflags - 1);
                while (s.ts_prev != null && s.ts_prev.type == C.NOTE && s.ts_prev.time == s.time && s.ts_prev.x > s1.xs)
                    s = s.ts_prev;
                for (; s != null && s.time <= s2.time; s = s.ts_next)
                {
                    if (s.type != C.NOTE || s.invis || (s.st != st && s.v != v))
                        continue;
                    x = s.v == v ? s.xs : s.x;
                    ys = a * x + b - staff_tb[s.st].y;
                    if (s.v == v)
                    {
                        stem_err = min_tb[s.nhd == 0 ? 0 : 1][s.nflags];
                        if (s.stem > 0)
                        {
                            if (s.notes[s.nhd].pit > 26)
                            {
                                stem_err -= 2;
                                if (s.notes[s.nhd].pit > 28)
                                    stem_err -= 2;
                            }
                            stem_err -= ys - 3 * (s.notes[s.nhd].pit - 18);
                        }
                        else
                        {
                            if (s.notes[0].pit < 18)
                            {
                                stem_err -= 2;
                                if (s.notes[0].pit < 16)
                                    stem_err -= 2;
                            }
                            stem_err -= 3 * (s.notes[0].pit - 18) - ys;
                        }
                        stem_err += BEAM_DEPTH + BEAM_SHIFT * (s.nflags - 1);
                    }
                    else
                    {
                        if (s1.stem > 0)
                        {
                            if (s.stem > 0)
                            {
                                if (s.ymn > ys + 4 || s.ymx < ys - beam_h - 2)
                                    continue;
                                if (s.v > v)
                                    stem_err = s.ymx - ys;
                                else
                                    stem_err = s.ymn + 8 - ys;
                            }
                            else
                            {
                                stem_err = s.ymx - ys;
                            }
                        }
                        else
                        {
                            if (s.stem < 0)
                            {
                                if (s.ymx < ys - 4 || s.ymn > ys - beam_h - 2)
                                    continue;
                                if (s.v < v)
                                    stem_err = ys - s.ymn;
                                else
                                    stem_err = ys - s.ymx + 8;
                            }
                            else
                            {
                                stem_err = ys - s.ymn;
                            }
                        }
                        stem_err += 2 + beam_h;
                    }
                    if (stem_err > max_stem_err)
                        max_stem_err = stem_err;
                }
            }
            else
            {
                for (; ; s = s.next)
                {
                    ys = a * s.xs + b - staff_tb[s.st].y;
                    stem_err = GSTEM - 2;
                    if (s.stem > 0)
                        stem_err -= ys - (3 * (s.notes[s.nhd].pit - 18));
                    else
                        stem_err += ys - (3 * (s.notes[0].pit - 18));
                    stem_err += 3 * (s.nflags - 1);
                    if (stem_err > max_stem_err)
                        max_stem_err = stem_err;
                    if (s == s2)
                        break;
                }
            }

            if (max_stem_err > 0)
                b += s1.stem * max_stem_err;

            if (!two_staves && !two_dir)
                for (s = s1.next; ; s = s.next)
                {
                    switch (s.type)
                    {
                        case C.REST:
                            if (!s.multi)
                                break;
                            g = s.ts_next;
                            if (g == null || g.st != st || (g.type != C.NOTE && g.type != C.REST))
                                break;
                        case C.BAR:
                            if (s.invis)
                                break;
                        case C.CLEF:
                            y = a * s.x + b;
                            if (s1.stem > 0)
                            {
                                y = s.ymx - y + BEAM_DEPTH + BEAM_SHIFT * (nflags - 1) + 2;
                                if (y > 0)
                                    b += y;
                            }
                            else
                            {
                                y = s.ymn - y - BEAM_DEPTH + BEAM_SHIFT * (nflags - 1) - 2;
                                if (y < 0)
                                    b += y;
                            }
                            break;
                        case C.GRACE:
                            for (g = s.extra; g != null; g = g.next)
                            {
                                y = a * g.x + b;
                                if (s1.stem > 0)
                                {
                                    y = g.ymx - y + BEAM_DEPTH + BEAM_SHIFT * (nflags - 1) + 2;
                                    if (y > 0)
                                        b += y;
                                }
                                else
                                {
                                    y = g.ymn - y - BEAM_DEPTH + BEAM_SHIFT * (nflags - 1) - 2;
                                    if (y < 0)
                                        b += y;
                                }
                            }
                            break;
                    }
                    if (s == s2)
                        break;
                }

            if (a == 0)
                b += b_pos(s1.grace, s1.stem, nflags, b - staff_tb[st].y);

            for (s = s1; ; s = s.next)
            {
                switch (s.type)
                {
                    case C.NOTE:
                        s.ys = a * s.xs + b - staff_tb[s.st].y;
                        if (s.stem > 0)
                        {
                            s.ymx = s.ys + 2.5;
                            if (s.ts_prev != null && s.ts_prev.stem > 0 && s.ts_prev.st == s.st && s.ts_prev.ymn < s.ymx && s.ts_prev.x == s.x && s.notes[0].shhd == 0)
                            {
                                s.ts_prev.x -= 3;
                                s.ts_prev.xs -= 3;
                            }
                        }
                        else
                        {
                            s.ymn = s.ys - 2.5;
                        }
                        break;
                    case C.REST:
                        y = a * s.x + b - staff_tb[s.st].y;
                        dy = BEAM_DEPTH + BEAM_SHIFT * (nflags - 1) + (s.head != C.FULL ? 4 : 9);
                        if (s1.stem > 0)
                        {
                            y -= dy;
                            if (s1.multi == 0 && y > 12)
                                y = 12;
                            if (s.y <= y)
                                break;
                        }
                        else
                        {
                            y += dy;
                            if (s1.multi == 0 && y < 12)
                                y = 12;
                            if (s.y >= y)
                                break;
                        }
                        if (s.head != C.FULL)
                            y = (((y + 3 + 12) / 6) | 0) * 6 - 12;
                        s.y = y;
                        break;
                }
                if (s == s2)
                    break;
            }

            if (staff_tb[st].y == 0)
                return false;
            bm.s1 = s1;
            bm.a = a;
            bm.b = b;
            bm.nflags = nflags;
            return true;
        }

        // Helper methods to clone objects and set properties
        private dynamic SymDup(dynamic s)
        {
            dynamic m, note;
            s = Clone(s);
            s.invis = true;
            s.extra = null;
            s.text = null;
            s.a_gch = null;
            s.a_ly = null;
            s.a_dd = null;
            s.tp = null;
            s.notes = Clone(s.notes);
            for (m = 0; m <= s.nhd; m++)
            {
                note = s.notes[m] = Clone(s.notes[m]);
                note.a_dd = null;
            }
            return s;
        }

        private void Lkvsym(dynamic s, dynamic s1) { /* Implementation of lkvsym */ }
        private void Lktsym(dynamic s, dynamic s1) { /* Implementation of lktsym */ }

        private dynamic Clone(dynamic original)
        {
            if (original is Dictionary<string, object>)
            {
                var clone = new Dictionary<string, object>();
                foreach (var kvp in original)
                {
                    if (kvp.Value is Dictionary<string, object>)
                        clone[kvp.Key] = Clone(kvp.Value);
                    else
                        clone[kvp.Key] = kvp.Value;
                }
                return clone;
            }
            return original;
        }
    }
    //****************************************************************************************

    public void DrawBeams(dynamic bm)
    {
        dynamic s, i, beam_dir, shift, bshift, bstub, bh, da, bd;
        dynamic k, k1, k2, x1;
        dynamic s1 = bm.s1;
        dynamic s2 = bm.s2;

        void DrawBeam(double x1, double x2, double dy, double h, dynamic bm, int n)
        {
            double y1, dy2;
            dynamic s = bm.s1;
            int nflags = s.nflags;

            if (s.ntrem != null)
                nflags -= s.ntrem;
            if (s.trem2 != null && n > nflags)
            {
                if (s.dur >= C.BLEN / 2)
                {
                    x1 = s.x + 6;
                    x2 = bm.s2.x - 6;
                }
                else if (s.dur < C.BLEN / 4)
                {
                    double dx = x2 - x1;
                    if (dx < 16)
                    {
                        x1 += dx / 4;
                        x2 -= dx / 4;
                    }
                    else
                    {
                        x1 += 5;
                        x2 -= 6;
                    }
                }
            }

            y1 = bm.a * x1 + bm.b - dy;
            x2 -= x1;
            x2 /= stv_g.scale;
            dy2 = bm.a * x2 * stv_g.scale;
            Xypath(x1, y1, true);
            output += "l" + x2.ToString("0.0") + " " + (-dy2).ToString("0.0") +
                "v" + h.ToString("0.0") +
                "l" + (-x2).ToString("0.0") + " " + dy2.ToString("0.0") +
                "z\"/>\n";
        }

        AnnoStart(s1, "beam");

        if (!s1.grace)
        {
            bshift = BEAM_SHIFT;
            bstub = BEAM_STUB;
            shift = 0.34; // (half width of the stem)
            bh = BEAM_DEPTH;
        }
        else
        {
            bshift = 3.5;
            bstub = 3.2;
            shift = 0.29;
            bh = 1.8;
        }
        bh /= stv_g.scale;

        beam_dir = s1.stem;
        if (s1.stem != s2.stem && s1.nflags < s2.nflags)
            beam_dir = s2.stem;
        if (beam_dir < 0)
            bh = -bh;

        DrawBeam(s1.xs - shift, s2.xs + shift, 0, bh, bm, 1);
        da = 0;
        for (s = s1; ; s = s.next)
        {
            if (s.type == C.NOTE && s.stem != beam_dir)
            {
                s.ys = bm.a * s.xs + bm.b - staff_tb[s.st].y +
                       bshift * (s.nflags - 1) * s.stem - bh;
            }
            if (s == s2)
                break;
        }

        if (s1.feathered_beam)
        {
            da = bshift / (s2.xs - s1.xs);
            if (s1.feathered_beam > 0)
            {
                da = -da;
                bshift = da * s1.xs;
            }
            else
            {
                bshift = da * s2.xs;
            }
            da *= beam_dir;
        }

        shift = 0;
        for (i = 2; i <= bm.nflags; i++)
        {
            shift += bshift;
            if (da != 0)
                bm.a += da;
            for (s = s1; ; s = s.next)
            {
                if (s.type != C.NOTE || s.nflags < i)
                {
                    if (s == s2)
                        break;
                    continue;
                }
                if (s.trem1 != null && i > s.nflags - s.ntrem)
                {
                    x1 = (s.dur >= C.BLEN / 2) ? s.x : s.xs;
                    DrawBeam(x1 - 5, x1 + 5, (shift + 2.5) * beam_dir, bh, bm, i);
                    if (s == s2)
                        break;
                    continue;
                }
                k1 = s;
                while (true)
                {
                    if (s == s2)
                        break;
                    k = s.next;
                    if (k.type == C.NOTE || k.type == C.REST)
                    {
                        if (k.trem1 != null)
                        {
                            if (k.nflags - k.ntrem < i)
                                break;
                        }
                        else if (k.nflags < i)
                        {
                            break;
                        }
                    }
                    if (k.beam_br1 != null || (k.beam_br2 != null && i > 2))
                        break;
                    s = k;
                }
                k2 = s;
                while (k2.type != C.NOTE)
                    k2 = k2.prev;
                x1 = k1.xs;
                bd = beam_dir;
                if (k1 == k2)
                {
                    if (k1 == s1)
                    {
                        x1 += bstub;
                    }
                    else if (k1 == s2)
                    {
                        x1 -= bstub;
                    }
                    else if (k1.beam_br1 != null || (k1.beam_br2 != null && i > 2))
                    {
                        x1 += bstub;
                    }
                    else
                    {
                        k = k1.next;
                        while (k.type != C.NOTE)
                            k = k.next;
                        if (k.beam_br1 != null || (k.beam_br2 != null && i > 2))
                        {
                            x1 -= bstub;
                        }
                        else
                        {
                            k1 = k1.prev;
                            while (k1.type != C.NOTE)
                                k1 = k1.prev;
                            if (k1.nflags < k.nflags || (k1.nflags == k.nflags && k1.dots < k.dots))
                                x1 += bstub;
                            else
                                x1 -= bstub;
                        }
                    }
                    if (k1.stem != beam_dir)
                    {
                        bd = k1.stem;
                        k1.ys = bm.a * k1.xs + bm.b - staff_tb[k1.st].y - bh;
                    }
                }
                DrawBeam(x1, k2.xs, shift * bd, bh, bm, i);
                if (s == s2)
                    break;
            }
        }
        if (s1.tmp)
            Unlksym(s1);
        else if (s2.tmp)
            Unlksym(s2);
        AnnoStop(s1, "beam");
    }


    /* -- draw the left side of the staves -- */
    public void DrawLStaff(double x)
    {
        int i, j;
        double yb, h;
        int fl;
        int nst = cur_sy.nstaff;
        int l = 0;

        /* -- draw a system brace or bracket -- */
        void DrawSysBra(double x, int st, int flag)
        {
            int i, st_end;
            double yt, yb;

            while (!cur_sy.st_print[st])
            {
                if ((cur_sy.staves[st].flags & flag) != 0)
                    return;
                st++;
            }
            i = st_end = st;
            while (true)
            {
                if (cur_sy.st_print[i])
                    st_end = i;
                if ((cur_sy.staves[i].flags & flag) != 0)
                    break;
                i++;
            }
            yt = staff_tb[st].y + staff_tb[st].topbar * staff_tb[st].staffscale;
            yb = staff_tb[st_end].y + staff_tb[st_end].botbar * staff_tb[st_end].staffscale;
            if ((flag & (CLOSE_BRACE | CLOSE_BRACE2)) != 0)
                OutBrace(x, yb, yt - yb);
            else
                OutBracket(x, yt, yt - yb);
        }

        for (i = 0; ; i++)
        {
            fl = cur_sy.staves[i].flags;
            if ((fl & (OPEN_BRACE | OPEN_BRACKET)) != 0)
                l++;
            if (cur_sy.st_print[i])
                break;
            if ((fl & (CLOSE_BRACE | CLOSE_BRACKET)) != 0)
                l--;
            if (i == nst)
                break;
        }
        for (j = nst; j > i; j--)
        {
            if (cur_sy.st_print[j])
                break;
        }
        if (i == j && l == 0)
            return;
        yb = staff_tb[j].y + staff_tb[j].botbar * staff_tb[j].staffscale;
        h = staff_tb[i].y + staff_tb[i].topbar * staff_tb[i].staffscale - yb;
        Xypath(x, yb);
        output += "v" + (-h).ToString("0.0") + "\"/>\n";
        for (i = 0; i <= nst; i++)
        {
            fl = cur_sy.staves[i].flags;
            if ((fl & OPEN_BRACE) != 0)
                DrawSysBra(x, i, CLOSE_BRACE);
            if ((fl & OPEN_BRACKET) != 0)
                DrawSysBra(x, i, CLOSE_BRACKET);
            if ((fl & OPEN_BRACE2) != 0)
                DrawSysBra(x - 6, i, CLOSE_BRACE2);
            if ((fl & OPEN_BRACKET2) != 0)
                DrawSysBra(x - 6, i, CLOSE_BRACKET2);
        }
    }

    public void DrawGraceSlur(dynamic grc, dynamic e, double dx)
    {
        double x1 = grc.x + 3.5;
        double y1 = grc.ys + staff_tb[grc.st].y;
        double x2 = e.x + dx;
        double y2 = e.ys + staff_tb[e.st].y;
        double dy = x2 - x1;

        dy = (Math.Abs(y2 - y1) < 3) ? 3 : dy / 2;
        if (y2 < y1)
            dy = -dy;
        y1 -= dy;
        y2 += dy;
        dy = Math.Max(3, Math.Abs(x2 - x1) / 3);
        Xypath(x1, y1);
        output += "c";
        string dxStr = ((x2 - x1) / 2).ToString("0.0");
        string dyStr = ((y2 - y1) / 2).ToString("0.0");
        output += dxStr + " " + (-dyStr) + " " +
                  dxStr + " " + dyStr + " " +
                  (x2 - x1).ToString("0.0") + " 0\"/>\n";
    }

    public void DrawSlur(dynamic s1, dynamic s2, int dir)
    {
        double x1, y1, x2, y2, yy1, yy2, dx;
        double xt, yt, dx2;
        double slope;
        int i;
        dynamic s = s1;
        int s1_st = s1.st;
        int s2_st = s2.st;

        if (s1.stemless && s1.beam_end == null && s2.beam_st == null)
        {
            DrawGraceSlur(s1, s2, 3.5);
            return;
        }

        x1 = s1.x + 4.5;
        y1 = s1.ys + staff_tb[s1_st].y;
        x2 = s2.x - 3.5;
        y2 = s2.ys + staff_tb[s2_st].y;
        yy1 = yy2 = 0;
        if (s1_st != s2_st)
        {
            if (staff_tb[s1_st].y > staff_tb[s2_st].y)
            {
                yy1 = staff_tb[s1_st].y;
                yy2 = staff_tb[s2_st].y;
            }
            else
            {
                yy1 = staff_tb[s2_st].y;
                yy2 = staff_tb[s1_st].y;
            }
        }

        dx = Math.Abs(x2 - x1);
        if (dx < 7)
        {
            yy1 += (y1 + y2) / 2;
            yy2 += yy1;
        }
        dx2 = Math.Min(12, dx / 3);
        dx /= 2;
        if (y2 > y1)
        {
            yy1 += 4;
            yy2 -= 2;
            slope = -0.5;
        }
        else
        {
            yy1 -= 4;
            yy2 += 2;
            slope = 0.5;
        }

        for (i = 0; s != null && i < 10; s = s.next)
        {
            if (s == s2)
                break;
            i++;
        }

        if (i == 10)
        {
            i = yy1;
            yy1 = yy2;
            yy2 = i;
            slope = -slope;
        }

        if (dir < 0)
        {
            slope = -slope;
            i = yy1;
            yy1 = yy2;
            yy2 = i;
        }

        xt = x1 + dx;
        yt = (y1 + y2) / 2 + (yy1 - y1) / 1.7;
        yy1 = (y1 + yt) / 2 + slope * dx2;
        yy2 = (y2 + yt) / 2 + slope * dx2;

        Xypath(x1, y1);
        output += "c" +
                  (xt - dx2).ToString("0.0") + " " + yy1.ToString("0.0") + " " +
                  (xt + dx2).ToString("0.0") + " " + yy2.ToString("0.0") + " " +
                  x2.ToString("0.0") + " " + y2.ToString("0.0") + "\"/>\n";
    }


    /* -- draw the stems for one note -- */
    public void DrawStem(dynamic s)
    {
        double x1, x2, y1, y2;

        x1 = s.x + s.wl;
        x2 = s.x - s.wr;
        y1 = s.ys + staff_tb[s.st].y;
        y2 = y1 + s.stem;

        if (s.multi < 0)
        {
            if (s.stem > 0)
            {
                Xypath(x2, y1);
                x2 += 6.3;
            }
            else
            {
                Xypath(x1, y1);
                x1 -= 6.3;
            }
        }
        else
        {
            Xypath(x1, y1);
        }

        output += "l" + (x2 - x1).ToString("0.0") + " " +
                  s.stem.ToString("0.0") + "\"/>\n";
    }

    /* -- draw the beams and the stems of one voice -- */
    public void DrawBeatVoice(dynamic s1, dynamic s2, int voice)
    {
        dynamic bm = null;
        dynamic s, s3;
        int st1, st2, i;

        for (st1 = s1.st; st1 <= s2.st; st1++)
        {
            for (st2 = s1.st; st2 <= s2.st; st2++)
            {
                if (staff_tb[st2].y == 0)
                    continue;
                for (s = s1; ; s = s.next)
                {
                    if (s.st == st2)
                    {
                        if (voice == 0 && s1.x == s2.x && s.next == null)
                            s2 = s;
                        if (!s.invis && !s.stemless)
                            DrawStem(s);
                        if (s.beam_st != null)
                            bm = CalculateBeam(bm, s);
                        if (bm != null && bm.s2 == s)
                            DrawBeams(bm);
                    }
                    if (s == s2)
                        break;
                }
                for (s3 = s1; s3 != s2; s3 = s3.next)
                {
                    if (s3.sl1 != null)
                    {
                        for (i = 0; i < s3.sl1.length; i++)
                        {
                            DrawSlur(s3, s3.sl1[i], 1);
                        }
                    }
                    if (s3.sl2 != null)
                    {
                        for (i = 0; i < s3.sl2.length; i++)
                        {
                            DrawSlur(s3, s3.sl2[i], -1);
                        }
                    }
                }
            }
        }
    }

    /* -- draw the beams and the stems -- */
    public void DrawBeat(dynamic s1, dynamic s2)
    {
        int voice, i;

        for (voice = 0; voice < cur_sy.nvoice; voice++)
        {
            for (i = 0; i < 2; i++)
            {
                if (voice == 0 && i == 1)
                    break;
                DrawBeatVoice(s1, s2, voice);
            }
        }
    }

    /* -- draw the whole staff (from s1 to s2) -- */
    public void DrawStaff(dynamic s1, dynamic s2)
    {
        dynamic s;
        double x;

        for (s = s1; ; s = s.next)
        {
            if (!s.invis)
                break;
            if (s == s2)
                return;
        }
        x = s1.x - 15;
        DrawLStaff(x);
        DrawBeat(s1, s2);
    }

    /* -- draw the time signature -- */
    public void DrawTime(dynamic s)
    {
        double x = s.x + 7.5;
        double y = s.ys + staff_tb[s.st].y;
        int n = s.n;

        while (n-- > 0)
        {
            y -= 8;
            Xypath(x, y);
            output += "m-3 0v12h6v-12h-6\"/>\n";
        }
    }


    /* -- draw a clef -- */
    public void DrawClef(dynamic s)
    {
        double x = s.x + 7;
        double y = s.ys + staff_tb[s.st].y;

        Xypath(x, y);
        switch ((string)s.clef)
        {
            case "C":
                output += "m-6-12a12,12 0 0,1 0,24a12,12 0 0,1 0-24\"/>\n";
                break;
            case "F":
                output += "m-3-12v24m-3-6h6v-12h-6\"/>\n";
                break;
            case "G":
                output += "m0-12v24m0-12h-12\"/>\n";
                break;
        }
    }



    /* -- draw the accidentals -- */
    public void DrawAcc(dynamic s)
    {
        double x = s.x;
        double y = s.ys + staff_tb[s.st].y;
        string acc = s.acc;

        Xypath(x, y);
        switch (acc)
        {
            case "#":
                output += "m-3-8h6m-3-2v12m3-6h6m-3-2v12\"/>\n";
                break;
            case "b":
                output += "m-3-8a6,6 0 1,0 0,16a6,6 0 0,0 0-16\"/>\n";
                break;
            case "=":
                output += "m-6-2h12m-12 4h12\"/>\n";
                break;
            case "x":
                output += "m-6-6l12,12m-12,0l12,-12\"/>\n";
                break;
            case "bb":
                output += "m-3-8a6,6 0 1,0 0,16a6,6 0 0,0 0-16m6,0a6,6 0 1,0 0,16a6,6 0 0,0 0-16\"/>\n";
                break;
            case "##":
                output += "m-3-8h6m-3-2v12m3-6h6m-3-2v12m6,0h6m-3-2v12m-3-6h-6m3-2v12\"/>\n";
                break;
        }
    }


    /* -- draw the heads of one note -- */
    public void DrawNoteheads(dynamic s)
    {
        double x = s.x;
        double y = s.ys + staff_tb[s.st].y;
        string head = s.head;

        Xypath(x, y);
        switch (head)
        {
            case "square":
                output += "m-6-6h12v12h-12v-12\"/>\n";
                break;
            case "diamond":
                output += "m0-6l6,6l-6,6l-6-6l6-6\"/>\n";
                break;
            case "triangle":
                output += "m0-6l6,12h-12l6-12\"/>\n";
                break;
            case "x":
                output += "m-6-6l12,12m-12,0l12,-12\"/>\n";
                break;
            default:
                output += "m-6-6h12v12h-12v-12\"/>\n";
                break;
        }
    }


    /* -- draw the dots of one note -- */
    public void DrawDots(dynamic s)
    {
        double x = s.x + s.wr + 3;
        double y = s.ys + staff_tb[s.st].y;

        while (s.dots-- > 0)
        {
            Xypath(x, y);
            output += "m-1-1h2v2h-2v-2\"/>\n";
            x += 5;
        }
    }


    /* -- draw the whole note -- */
    public void DrawNoteS1(dynamic s1, dynamic s2)
    {
        dynamic s;

        for (s = s1; ; s = s.next)
        {
            if (s.type == C.NOTE && !s.invis)
            {
                DrawNoteheads(s);
                if (s.acc != null)
                    DrawAcc(s);
                DrawDots(s);
            }
            if (s == s2)
                break;
        }
    }

    /* -- draw a rest -- */
    public void DrawRest(dynamic s)
    {
        double x = s.x;
        double y = s.ys + staff_tb[s.st].y;

        Xypath(x, y);
        switch (s.dur)
        {
            case C.BREVE:
                output += "m-5-5h10v10h-10v-10\"/>\n";
                break;
            case C.WHOLE:
                output += "m-5 0h10v5h-10v-5\"/>\n";
                break;
            case C.HALF:
                output += "m-5-5h10v5h-10v-5\"/>\n";
                break;
            case C.QUARTER:
                output += "m-2.5-7.5h5v15h-5v-15\"/>\n";
                break;
            case C.EIGHTH:
                output += "m-5-5h10v10h-10v-10\"/>\n";
                break;
            case C.SIXTEENTH:
                output += "m-5-5h10v10h-10v-10\"/>\n";
                break;
        }
    }


    /* -- draw all elements of the staff -- */
    public void DrawElements(dynamic s1, dynamic s2)
    {
        dynamic s;

        for (s = s1; ; s = s.next)
        {
            switch (s.type)
            {
                case C.NOTE:
                    DrawNoteS1(s, s);
                    break;
                case C.REST:
                    DrawRest(s);
                    break;
                case C.CLEF:
                    DrawClef(s);
                    break;
                case C.TIME:
                    DrawTime(s);
                    break;
            }
            if (s == s2)
                break;
        }
    }





}