using System;
using System.Collections.Generic;
using System.Text;
// abc2svg - draw.js - draw functions
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
        // Constants
        const double STEM_MIN = 16;  // min stem height under beams
        const double STEM_MIN2 = 14; // ... for notes with two beams
        const double STEM_MIN3 = 12; // ... for notes with three beams
        const double STEM_MIN4 = 10; // ... for notes with four beams
        const double STEM_CH_MIN = 14; // min stem height for chords under beams
        const double STEM_CH_MIN2 = 10; // ... for notes with two beams
        const double STEM_CH_MIN3 = 9; // ... for notes with three beams
        const double STEM_CH_MIN4 = 9; // ... for notes with four beams
        const double BEAM_DEPTH = 3.2; // width of a beam stroke
        const double BEAM_OFFSET = 0.25; // pos of flat beam relative to staff line
        const double BEAM_SHIFT = 5; // shift of second and third beams
        const double BEAM_STUB = 7; // length of stub for flag under beam
        const double SLUR_SLOPE = 0.5; // max slope of a slur
        const double GSTEM = 15; // grace note stem length
        const double GSTEM_XOFF = 2.3; // x offset for grace note stem

        // Variable declarations
        static Dictionary<string, object> cache;
        static List<object> anno_a = new List<object>(); // symbols with annotations

        /* -- calculate a beam -- */
        /* (the staves may be defined or not) */
        double[,] min_tb = {
            { STEM_MIN, STEM_MIN,
                STEM_MIN2, STEM_MIN3, STEM_MIN4, STEM_MIN4 },
            { STEM_CH_MIN, STEM_CH_MIN,
                STEM_CH_MIN2, STEM_CH_MIN3, STEM_CH_MIN4, STEM_CH_MIN4 } };



        // Function to compute the best vertical offset for the beams
        public double b_pos(bool grace, int stem, int nflags, int b)
        {
            double top, bot, d1, d2;
            double shift = !grace ? BEAM_SHIFT : 3.5;
            double depth = !grace ? BEAM_DEPTH : 1.8;

            double rnd6(double y)
            {
                double iy = Math.Round((y + 12) / 6.0) * 6 - 12;
                return iy - y;
            }

            if (stem > 0)
            {
                bot = b - (nflags - 1) * shift - depth;
                if (bot > 26)
                    return 0;
                top = b;
            }
            else
            {
                top = b + (nflags - 1) * shift + depth;
                if (top < -2)
                    return 0;
                bot = b;
            }

            d1 = rnd6(top - BEAM_OFFSET);
            d2 = rnd6(bot + BEAM_OFFSET);
            return (d1 * d1 > d2 * d2) ? d2 : d1;
        }

        /* duplicate a note for beaming continuation */
        public VoiceItem sym_dup(VoiceItem s)
        {
            VoiceItem m, note;

            s = clone(s);
            s.invis = true;
            s.extra = null;
            s.text = null;
            s.a_gch = null;
            s.a_ly = null;
            s.a_dd = null;
            s.tp = null;
            s.notes = clone(s.notes);
            for (m = 0; m <= s.nhd; m++)
            {
                note = s.notes[m] = clone(s.notes[m]);
                note.a_dd = null;
            }
            return s;
        }

        // (possible hook)
        public void calculate_beam(BeamItem bm, VoiceItem s1)
        {
            VoiceItem s, s2;
            VoiceItem[] notes;
            int g, nflags, st, v, two_staves, two_dir;
            int x, y, ys, a, b, stem_err, max_stem_err,
                p_min, p_max, s_closest,
                stem_xoff, scale,
                visible, dy;

            if (!s1.beam_st)
            {    /* beam from previous music line */
                s = sym_dup(s1);
                lkvsym(s, s1);
                lktsym(s, s1);
                s.x -= 12;
                if (s.x > s1.prev.x + 12)
                    s.x = s1.prev.x + 12;
                s.beam_st = true;
                s.beam_end = null;
                s.tmp = true;
                s.sls = null;
                s1 = s;
            }

            /* search last note in beam */
            notes = nflags = 0;    /* set x positions, count notes and flags */
            two_staves = two_dir = false;
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
                    if (!visible && !s2.invis
                        && (!s2.stemless || s2.trem2))
                        visible = true;
                    if (s2.beam_end)
                        break;
                }
                if (!s2.next)
                {        /* beam towards next music line */
                    for (; ; s2 = s2.prev)
                    {
                        if (s2.type == C.NOTE)
                            break;
                    }
                    s = sym_dup(s2);
                    s.next = s2.next;
                    if (s.next)
                        s.next.prev = s;
                    s2.next = s;
                    s.prev = s2;
                    s.ts_next = s2.ts_next;
                    if (s.ts_next)
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

            // at least, must have a visible note with a stem
            if (!visible)
                return false;

            bm.s2 = s2;            /* (don't display the flags) */

            if (staff_tb[st].y == 0)
            {    /* staves not defined */
                if (two_staves)
                    return false;
            }
            else
            {            /* staves defined */
                //        if (!two_staves && !s1.grace) {
                if (!two_staves)
                {
                    bm.s1 = s1;    /* beam already calculated */
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

            // have flat beams on grace notes when asked
            if (s.grace && s1.fmt.flatbeams)
                a = 0;
            // if a note inside the beam is the closest to the beam, the beam is flat
            else if (!two_dir
                && notes >= 3
                && s_closest != s1 && s_closest != s2)
                a = 0;
            y = s1.ys + staff_tb[st].y;
            if (a == null)
                a = (s2.ys + staff_tb[s2.st].y - y) / (s2.xs - s1.xs);
            if (a != 0)
            {
                a = s1.fmt.beamslope * a /
                    (s1.fmt.beamslope + Math.abs(a)); // max steepness for beam
                if (a > -.04 && a < .04)
                    a = 0;                // slope too low
            }
            // pivot around the middle of the beam
            b = (y + s2.ys + staff_tb[s2.st].y) / 2 - a * (s2.xs + s1.xs) / 2;
            /*fixme: have a look again*/
            /* have room for the symbols in the staff */
            max_stem_err = 0;        /* check stem lengths */
            s = s1;
            if (two_dir)
            {                /* 2 directions */
                /*fixme: more to do*/
                ys = ((s1.grace ? 3.5 : BEAM_SHIFT) * (nflags - 1) +
                    BEAM_DEPTH) * .5;
                if (s1.nflags == s2.nflags) { }
                else if (s1.stem != s2.stem && s1.nflags < s2.nflags)
                    b += ys * s2.stem;
                else
                    b += ys * s1.stem;
            }
            else if (!s1.grace)
            {            /* normal notes */
                var beam_h = BEAM_DEPTH + BEAM_SHIFT * (nflags - 1);
                //--fixme: added for abc2svg
                while (s.ts_prev
                    && s.ts_prev.type == C.NOTE
                    && s.ts_prev.time == s.time
                    && s.ts_prev.x > s1.xs)
                    s = s.ts_prev;
                for (; s && s.time <= s2.time; s = s.ts_next)
                {
                    if (s.type != C.NOTE
                        || s.invis
                        || (s.st != st
                            && s.v != v))
                    {
                        continue;
                    }
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
                        /*fixme: KO when two_staves*/
                        if (s1.stem > 0)
                        {
                            if (s.stem > 0)
                            {
                                /*fixme: KO when the voice numbers are inverted*/
                                if (s.ymn > ys + 4
                                    || s.ymx < ys - beam_h - 2)
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
                                if (s.ymx < ys - 4
                                    || s.ymn > ys - beam_h - 2)
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
            {                /* grace notes */
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
            if (max_stem_err > 0)        /* shift beam if stems too short */
                b += s1.stem * max_stem_err;
            /* have room for the gracenotes, bars and clefs */
            /*fixme: test*/
            if (!two_staves && !two_dir)
                for (s = s1.next; ; s = s.next)
                {
                    switch (s.type)
                    {
                        case C.REST:        /* cannot move rests in multi-voices */
                            if (!s.multi)
                                break;
                            g = s.ts_next;
                            if (!g || g.st != st
                                || (g.type != C.NOTE && g.type != C.REST))
                                break;
                        //fixme:too much vertical shift if some space above the note
                        //fixme:this does not fix rest under beam in second voice (ts_prev)
                        /*fall thru*/
                        case C.BAR:
                            if (s.invis)
                                break;
                        /*fall thru*/
                        case C.CLEF:
                            y = a * s.x + b;
                            if (s1.stem > 0)
                            {
                                y = s.ymx - y
                                    + BEAM_DEPTH + BEAM_SHIFT * (nflags - 1)
                                    + 2;
                                if (y > 0)
                                    b += y;
                            }
                            else
                            {
                                y = s.ymn - y
                                    - BEAM_DEPTH - BEAM_SHIFT * (nflags - 1)
                                    - 2;
                                if (y < 0)
                                    b += y;
                            }
                            break;
                        case C.GRACE:
                            for (g = s.extra; g; g = g.next)
                            {
                                y = a * g.x + b;
                                if (s1.stem > 0)
                                {
                                    y = g.ymx - y
                                        + BEAM_DEPTH + BEAM_SHIFT * (nflags - 1)
                                        + 2;
                                    if (y > 0)
                                        b += y;
                                }
                                else
                                {
                                    y = g.ymn - y
                                        - BEAM_DEPTH - BEAM_SHIFT * (nflags - 1)
                                        - 2;
                                    if (y < 0)
                                        b += y;
                                }
                            }
                            break;
                    }
                    if (s == s2)
                        break;
                }
            if (a == 0)        /* shift flat beams onto staff lines */
                b += b_pos(s1.grace, s1.stem, nflags, b - staff_tb[st].y);
            /* adjust final stems and rests under beam */
            for (s = s1; ; s = s.next)
            {
                switch (s.type)
                {
                    case C.NOTE:
                        s.ys = a * s.xs + b - staff_tb[s.st].y;
                        if (s.stem > 0)
                        {
                            s.ymx = s.ys + 2.5;
                            //fixme: hack
                            if (s.ts_prev
                                && s.ts_prev.stem > 0
                                && s.ts_prev.st == s.st
                                && s.ts_prev.ymn < s.ymx
                                && s.ts_prev.x == s.x
                                && s.notes[0].shhd == 0)
                            {
                                s.ts_prev.x -= 3;    /* fix stem clash */
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
                        dy = BEAM_DEPTH + BEAM_SHIFT * (nflags - 1)
                            + (s.head != C.FULL ? 4 : 9);
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
            /* save beam parameters */
            if (staff_tb[st].y == 0)    /* if staves not defined */
                return false;
            bm.s1 = s1;
            bm.a = a;
            bm.b = b;
            bm.nflags = nflags;
            return true;
        }

        /* -- draw the beams for one word -- */
        /* (the staves are defined) */
        public void draw_beams(BeamItem bm)
        {
            VoiceItem s;
            VoiceItem s1 = bm.s1;
            VoiceItem s2 = bm.s2;
            int i, beam_dir, shift, bshift, bstub, bh, da, bd,
                k, k1, k2, x1;

            /* -- draw a single beam -- */
            void draw_beam(int x1, int x2, int dy, int h, BeamItem bm,
                int n)    /* beam number (1..n) */
            {
                int y1, dy2;
                VoiceItem s = bm.s1;
                int nflags = s.nflags;

                if (s.ntrem)
                    nflags -= s.ntrem;
                if (s.trem2 && n > nflags)
                {
                    if (s.dur >= C.BLEN / 2)
                    {
                        x1 = s.x + 6;
                        x2 = bm.s2.x - 6;
                    }
                    else if (s.dur < C.BLEN / 4)
                    {
                        int dx = x2 - x1;
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
                xypath(x1, y1, true);
                output += 'l' + x2.toFixed(1) + ' ' + (-dy2).toFixed(1) +
                    'v' + h.toFixed(1) +
                    'l' + (-x2).toFixed(1) + ' ' + dy2.toFixed(1) +
                    'z"/>\n';
            } // draw_beam()

            anno_start(s1, 'beam');
            /*fixme: KO if many staves with different scales*/
            //    set_scale(s1);
            if (!s1.grace)
            {
                bshift = BEAM_SHIFT;
                bstub = BEAM_STUB;
                shift = .34;        /* (half width of the stem) */
                bh = BEAM_DEPTH;
            }
            else
            {
                bshift = 3.5;
                bstub = 3.2;
                shift = .29;
                bh = 1.8;
            }
            bh /= stv_g.scale;

            /*fixme: quick hack for stubs at end of beam and different stem directions*/
            beam_dir = s1.stem;
            if (s1.stem != s2.stem
                && s1.nflags < s2.nflags)
                beam_dir = s2.stem;
            if (beam_dir < 0)
                bh = -bh;

            /* make first beam over whole word and adjust the stem lengths */
            draw_beam(s1.xs - shift, s2.xs + shift, 0, bh, bm, 1);
            da = 0;
            for (s = s1; ; s = s.next)
            {
                if (s.type == C.NOTE
                    && s.stem != beam_dir)
                    s.ys = bm.a * s.xs + bm.b
                        - staff_tb[s.st].y
                        + bshift * (s.nflags - 1) * s.stem
                        - bh;
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
                da = da * beam_dir;
            }

            /* other beams with two or more flags */
            shift = 0;
            for (i = 2; i <= bm.nflags; i++)
            {
                shift += bshift;
                if (da != 0)
                    bm.a += da;
                for (s = s1; ; s = s.next)
                {
                    if (s.type != C.NOTE
                        || s.nflags < i)
                    {
                        if (s == s2)
                            break;
                        continue;
                    }
                    if (s.trem1
                        && i > s.nflags - s.ntrem)
                    {
                        x1 = (s.dur >= C.BLEN / 2) ? s.x : s.xs;
                        draw_beam(x1 - 5, x1 + 5,
                            (shift + 2.5) * beam_dir,
                            bh, bm, i);
                        if (s == s2)
                            break;
                        continue;
                    }
                    k1 = s;
                    while (1)
                    {
                        if (s == s2)
                            break;
                        k = s.next;
                        if (k.type == C.NOTE || k.type == C.REST)
                        {
                            if (k.trem1)
                            {
                                if (k.nflags - k.ntrem < i)
                                    break;
                            }
                            else if (k.nflags < i)
                            {
                                break;
                            }
                        }
                        if (k.beam_br1
                            || (k.beam_br2 && i > 2))
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
                        else if (k1.beam_br1
                            || (k1.beam_br2
                                && i > 2))
                        {
                            x1 += bstub;
                        }
                        else
                        {
                            k = k1.next;
                            while (k.type != C.NOTE)
                                k = k.next;
                            if (k.beam_br1
                                || (k.beam_br2 && i > 2))
                            {
                                x1 -= bstub;
                            }
                            else
                            {
                                k1 = k1.prev;
                                while (k1.type != C.NOTE)
                                    k1 = k1.prev;
                                if (k1.nflags < k.nflags
                                    || (k1.nflags == k.nflags
                                        && k1.dots < k.dots))
                                    x1 += bstub;
                                else
                                    x1 -= bstub;
                            }
                        }
                        if (k1.stem != beam_dir)
                        {
                            bd = k1.stem;
                            k1.ys = bm.a * k1.xs + bm.b
                                - staff_tb[k1.st].y - bh;
                        }
                    }
                    draw_beam(x1, k2.xs,
                        shift * bd,
                        bh, bm, i);
                    if (s == s2)
                        break;
                }
            }
            if (s1.tmp)
                unlksym(s1);
            else if (s2.tmp)
                unlksym(s2);
            anno_stop(s1, 'beam');
        }

        /* -- draw the left side of the staves -- */
        public void draw_lstaff(double x)
        {
            int i, j, yb, h, fl,
                nst = cur_sy.nstaff,
                l = 0;

            /* -- draw a system brace or bracket -- */
            void draw_sysbra(int x, int st, int flag)
            {
                int i, st_end, yt, yb;

                while (!cur_sy.st_print[st])
                {
                    if (cur_sy.staves[st].flags & flag)
                        return;
                    st++;
                }
                i = st_end = st;
                while (1)
                {
                    if (cur_sy.st_print[i])
                        st_end = i;
                    if (cur_sy.staves[i].flags & flag)
                        break;
                    i++;
                }
                yt = staff_tb[st].y + staff_tb[st].topbar
                    * staff_tb[st].staffscale;
                yb = staff_tb[st_end].y + staff_tb[st_end].botbar
                    * staff_tb[st_end].staffscale;
                if (flag & (CLOSE_BRACE | CLOSE_BRACE2))
                    out_brace(x, yb, yt - yb);
                else
                    out_bracket(x, yt, yt - yb);
            }

            for (i = 0; ; i++)
            {
                fl = cur_sy.staves[i].flags;
                if (fl & (OPEN_BRACE | OPEN_BRACKET))
                    l++;
                if (cur_sy.st_print[i])
                    break;
                if (fl & (CLOSE_BRACE | CLOSE_BRACKET))
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
            xypath(x, yb);
            output += "v" + (-h).toFixed(1) + '"/>\n';
            for (i = 0; i <= nst; i++)
            {
                fl = cur_sy.staves[i].flags;
                if (fl & OPEN_BRACE)
                    draw_sysbra(x, i, CLOSE_BRACE);
                if (fl & OPEN_BRACKET)
                    draw_sysbra(x, i, CLOSE_BRACKET);
                if (fl & OPEN_BRACE2)
                    draw_sysbra(x - 6, i, CLOSE_BRACE2);
                if (fl & OPEN_BRACKET2)
                    draw_sysbra(x - 6, i, CLOSE_BRACKET2);
            }
        }

        /* -- draw the time signature -- */
        public void draw_meter(VoiceItem s)
        {
            if (s.a_meter == null)
                return;
            int dx, i, j, meter, x,
                st = s.st,
                p_staff = staff_tb[st],
                y = p_staff.y;

            // adjust the vertical offset according to the staff definition
            if (p_staff.stafflines != "|||||")
                y += (p_staff.topbar + p_staff.botbar) / 2 - 12;    // bottom

            for (i = 0; i < s.a_meter.Length; i++)
            {
                meter = s.a_meter[i];
                x = s.x + s.x_meter[i];

                if (meter.bot)
                {
                    out_XYAB("\
        < g transform =\"translate(X,Y)\" text-anchor=\"middle\">\n\
            < text y =\"-12\">A</text>\n\
            < text > B </ text >\n\
</ g >\n", x, y + 6, m_gl(meter.top), m_gl(meter.bot));
                }
                else
                {
                    out_XYAB("\
        < text x =\"X\" y=\"Y\" text-anchor=\"middle\">A</text>\n",
                        x, y + 12, m_gl(meter.top));
                }
            }
        }

        /* -- draw an accidental -- */
        public void draw_acc(double x, double y, int a)
        {
            if (a.GetType() == typeof(int[]))
            {        // if microtone
                int c;
                int n = a[0];
                int d = a[1];

                c = n + "_" + d;
                a = acc_nd[c];
                if (a == null)
                {
                    a = abc2svg.rat(Math.Abs(n), d);
                    d = a[1];
                    a = (n < 0 ? -a[0] : a[0]).ToString();
                    if (d != 1)
                        a += "_" + d;
                    acc_nd[c] = a;
                }
            }
            xygl(x, y, "acc" + a);
        }

        // memorize the helper/ledger lines
        public void set_hl(Staff p_st, int n, double x, double dx1, double dx2)
        {
            List<int[]> hl;

            if (n >= 0)
            {
                hl = p_st.hlu[n];
                if (hl == null)
                    hl = p_st.hlu[n] = new List<int[]>();
            }
            else
            {
                hl = p_st.hld[-n];
                if (hl == null)
                    hl = p_st.hld[-n] = new List<int[]>();
            }

            for (int i = 0; i < hl.Count; i++)
            {
                if (x >= hl[i][0])
                    break;
            }
            if (i == hl.Count)
            {
                hl.Add(new int[] { x, dx1, dx2 });
            }
            else if (x > hl[i][0])
            {
                hl.Insert(++i, new int[] { x, dx1, dx2 });
            }
            else
            {
                if (dx1 < hl[i][1])
                    hl[i][1] = dx1;
                if (dx2 > hl[i][2])
                    hl[i][2] = dx2;
            }
        }

        // draw helper lines
        // (possible hook)
        public void draw_hl(VoiceItem s)
        {
            int i, j, n, note;
            List<int[]> hla = new List<int[]>();
            int st = s.st;
            Staff p_staff = staff_tb[st];

            // check if any helper line
            if (!p_staff.hll
                || s.invis)
                return;            // no helper line (no line)
            for (i = 0; i <= s.nhd; i++)
            {
                note = s.notes[i];
                if (!p_staff.hlmap[note.pit - p_staff.hll])
                    hla.Add(new int[] { note.pit - 18,
            note.shhd * stv_g.scale });
            }
            n = hla.Count;
            if (n == 0)
                return;            // no

            // handle the helper lines out of the staff
            int dx1, dx2, hl, shhd, hlp;
            string stafflines = p_staff.stafflines;
            int top = stafflines.Length - 1;
            int yu = top;
            int bot = p_staff.botline / 6;
            int yl = bot;
            int dx = s.grace ? 4 : hw_tb[s.head] * 1.3;

            // get the x start and x stop of the intermediate helper lines
            note = s.notes[s.stem < 0 ? s.nhd : 0];
            shhd = note.shhd;

            for (i = 0; i < hla.Count; i++)
            {
                hlp = hla[i][0];
                dx1 = (hla[i][1] < shhd ? hla[i][1] : shhd) - dx;
                dx2 = (hla[i][1] > shhd ? hla[i][1] : shhd) + dx;
                if (hlp < bot * 2)
                {
                    if (hlp < yl * 2)
                        yl = ++hlp >> 1;
                    n--;
                }
                else if (hlp > top * 2)
                {
                    yu = hlp >> 1;
                    n--;
                }
                set_hl(p_staff, hlp >> 1, s.x, dx1, dx2);
            }

            dx1 = shhd - dx;
            dx2 = shhd + dx;
            while (++yl < bot)
                set_hl(p_staff, yl,
                    s.x, dx1, dx2);
            while (--yu > top)
                set_hl(p_staff, yu,
                    s.x, dx1, dx2);
            if (n == 0)
                return;            // no more helper lines

            // draw the helper lines inside the staff
            i = yl;
            j = yu;
            while (i > bot && stafflines[i] == '-')
                i--;
            while (j < top && stafflines[j] == '-')
                j++;
            for (; i < j; i++)
            {
                if (stafflines[i] == '-')
                    set_hl(p_staff, i, s.x, dx1, dx2);
            }
        }

        /* -- draw a key signature -- */
        // (possible hook)
        int[] sharp_cl = new int[] { 24, 9, 15, 21, 6, 12, 18 },
              flat_cl = new int[] { 12, 18, 24, 9, 15, 21, 6 },
              sharp1 = new int[] { -9, 12, -9, -9, 12, -9 },
              sharp2 = new int[] { 12, -9, 12, -9, 12, -9 },
              flat1 = new int[] { 9, -12, 9, -12, 9, -12 },
              flat2 = new int[] { -12, 9, -12, 9, -12, 9 };

        void draw_keysig(double x, VoiceItem s)
        {
            var old_sf = s.k_old_sf;
            var st = s.st;
            var staffb = staff_tb[st].y;
            int i, shift, p_seq;
            var clef_ix = s.k_y_clef;
            var a_acc = s.k_a_acc;

            void set_k_acc(int[] a_acc, int sf)
            {
                int i, j, n, nacc, p_acc;
                int[] accs = new int[7];
                int[] pits = new int[7];

                if (sf > 0)
                {
                    for (nacc = 0; nacc < sf; nacc++)
                    {
                        accs[nacc] = 1;
                        pits[nacc] = new int[] { 26, 23, 27, 24, 21, 25, 22 }[nacc];
                    }
                }
                else
                {
                    for (nacc = 0; nacc < -sf; nacc++)
                    {
                        accs[nacc] = -1;
                        pits[nacc] = new int[] { 22, 25, 21, 24, 20, 23, 26 }[nacc];
                    }
                }
                n = a_acc.Length;
                for (i = 0; i < n; i++)
                {
                    p_acc = a_acc[i];
                    for (j = 0; j < nacc; j++)
                    {
                        if (pits[j] == p_acc.pit)
                        {
                            accs[j] = p_acc.acc;
                            break;
                        }
                    }
                    if (j == nacc)
                    {
                        accs[j] = p_acc.acc;
                        pits[j] = p_acc.pit;
                        nacc++;
                    }
                }
                for (i = 0; i < nacc; i++)
                {
                    p_acc = (int)a_acc[i];
                    if (p_acc == null)
                        p_acc = (int)a_acc[i] = new object();
                    p_acc.acc = accs[i];
                    p_acc.pit = pits[i];
                }
            }

            if (clef_ix.Value % 2 == 1)
                clef_ix += 7;
            clef_ix /= 2;
            while (clef_ix < 0)
                clef_ix += 7;
            clef_ix %= 7;

            if (a_acc != null && !s.exp)
                set_k_acc(a_acc, s.k_sf);

            if (a_acc == null)
            {
                if (s.fmt.cancelkey || s.k_sf == 0)
                {
                    if (s.k_sf == 0 || old_sf * s.k_sf < 0)
                    {
                        shift = sharp_cl[clef_ix];
                        p_seq = shift > 9 ? sharp1 : sharp2;
                        for (i = 0; i < old_sf; i++)
                        {
                            xygl(x, staffb + shift, "acc3");
                            shift += p_seq[i];
                            x += 5;
                        }

                        shift = flat_cl[clef_ix];
                        p_seq = shift < 18 ? flat1 : flat2;
                        for (i = 0; i > old_sf; i--)
                        {
                            xygl(x, staffb + shift, "acc3");
                            shift += p_seq[-i];
                            x += 5;
                        }
                        if (s.k_sf != 0)
                            x += 3;
                    }
                }

                if (s.k_sf > 0)
                {
                    shift = sharp_cl[clef_ix];
                    p_seq = shift > 9 ? sharp1 : sharp2;
                    for (i = 0; i < s.k_sf; i++)
                    {
                        xygl(x, staffb + shift, "acc1");
                        shift += p_seq[i];
                        x += 5;
                    }
                    if (s.fmt.cancelkey && i < old_sf)
                    {
                        x += 2;
                        for (; i < old_sf; i++)
                        {
                            xygl(x, staffb + shift, "acc3");
                            shift += p_seq[i];
                            x += 5;
                        }
                    }
                    if (s.k_bagpipe == 'p')
                    {
                        xygl(x, staffb + 27, "acc3");
                        x += 5;
                    }
                }

                if (s.k_sf < 0)
                {
                    shift = flat_cl[clef_ix];
                    p_seq = shift < 18 ? flat1 : flat2;
                    for (i = 0; i > s.k_sf; i--)
                    {
                        xygl(x, staffb + shift, "acc-1");
                        shift += p_seq[-i];
                        x += 5;
                    }
                    if (s.fmt.cancelkey && i > old_sf)
                    {
                        x += 2;
                        for (; i > old_sf; i--)
                        {
                            xygl(x, staffb + shift, "acc3");
                            shift += p_seq[-i];
                            x += 5;
                        }
                    }
                }
            }
            else if (a_acc.Length > 0)
            {
                var acc = default(object);
                var last_acc = a_acc[0].acc;
                var last_shift = 100;
                var s2 = new VoiceItem
                {
                    st = st,
                    nhd = 0,
                    notes = new noteItem[] { new noteItem() }
                };

                for (i = 0; i < a_acc.Length; i++)
                {
                    acc = a_acc[i];
                    shift = (s.k_y_clef + acc.pit - 18) * 3;
                    while (shift < -3)
                        shift += 21;
                    while (shift > 27)
                        shift -= 21;
                    if (i != 0 && (shift > last_shift + 18 || shift < last_shift - 18))
                        x -= 5;
                    else if (acc.acc != last_acc)
                        x += 3;
                    last_acc = acc.acc;
                    s2.x = x;
                    s2.notes[0].pit = shift / 3 + 18;
                    last_shift = shift;
                    draw_acc(x, staffb + shift, acc.acc);
                    x += 5;
                }
            }
        }

        // output the measure repeat number
        public void nrep_out(double x, double y, int n)
        {
            y -= 3;
            if (n < 10)
            {
                xygl(x - 4, y, "mtr" + n);
            }
            else
            {
                xygl(x - 10, y, "mtr" + ((n / 10) | 0));
                xygl(x - 2, y, "mtr" + (n % 10));
            }
        }

        // if rest alone in the measure or measure repeat,
        // change the head and center
        public void center_rest(VoiceItem s)
        {
            VoiceItem s2, x;

            if (s.dur < C.BLEN * 2)
                s.nflags = -2;        // semibreve / whole
            else if (s.dur < C.BLEN * 4)
                s.nflags = -3;
            else
                s.nflags = -4;
            s.dots = 0;

            /* don't use next/prev: there is no bar in voice overlay */
            s2 = s.ts_next;
            while (s2.time != s.time + s.dur
                && s2.ts_next)
                s2 = s2.ts_next;
            x = s2.x - s2.wl;
            s2 = s;
            while (!s2.seqst)
                s2 = s2.ts_prev;
            s2 = s2.ts_prev;
            x = (x + s2.x + s2.wr) / 2;

            /* center the associated decorations */
            if (s.a_dd != null)
                deco_update(s, x - s.x);
            s.x = x;
        }


        /* -- draw a rest -- */
        /* (the staves are defined) */
        public  string[] rest_tb = {
            "r128", "r64", "r32", "r16", "r8",
            "r4",
            "r2", "r1", "r0", "r00"};

        void draw_rest(VoiceItem s)
        {
            VoiceItem s2;
            int i, j, y, bx;
            var p_staff = staff_tb[s.st];
            var yb = p_staff.y;
            var x = s.x;

            if (s.notes[0].shhd != null)
                x += s.notes[0].shhd.Value * stv_g.scale;

            if (s.rep_nb != null)
            {
                set_sscale(s.st);
                anno_start(s);
                if (p_staff.stafflines == "|||||")
                    yb += 12;
                else
                    yb += (p_staff.topbar + p_staff.botbar) / 2;
                if (s.rep_nb < 0)
                {
                    xygl(x, yb, "srep");
                }
                else
                {
                    xygl(x, yb, "mrep");
                    if (s.rep_nb > 2 && s.v == cur_sy.top_voice && s.fmt.measrepnb > 0 && s.rep_nb % s.fmt.measrepnb == 0)
                        nrep_out(x, yb + p_staff.topbar, s.rep_nb);
                }
                anno_a.Add(s);
                return;
            }

            set_scale(s);
            anno_start(s);

            if (s.notes[0].color != null)
                set_color(s.notes[0].color.Value);

            y = s.y;

            i = 5 - s.nflags;
            if (i == 7 && y == 12 && p_staff.stafflines.Length <= 2)
                y -= 6;

            if (!s.notes[0].invis)
                xygl(x, y + yb, rest_tb[i]);

            if (s.dots)
            {
                x += 8;
                y += yb + 3;
                j = s.dots.Value;
                i = (s.dur.Value / 12) >> ((5 - s.nflags) - j);
                while (j-- > 0)
                {
                    xygl(x, y, (i & (1 << j)) != 0 ? "dot" : "dot+");
                    x += 3.5;
                }
            }
            set_color(null);
            anno_a.Add(s);
        }

        // -- draw a multi-measure rest --
        // (the staves are defined)
        void draw_mrest(VoiceItem s)
        {
            int x1, x2, prev;
            var p_st = staff_tb[s.st];
            var y = p_st.y + (p_st.topbar + p_st.botbar) / 2;
            var p = s.nmes.ToString();

            int omrest()
            {
                int x = s.x;
                int y = p_st.y + 12;
                int n = s.nmes;
                int k = n >> 2;

                if ((n & 3) != 0)
                {
                    k++;
                    if ((n & 3) == 3)
                        k++;
                }
                x -= 3 * (k - 1);
                while (n >= 4)
                {
                    xygl(x, y, "r00");
                    n -= 4;
                    x += 6;
                }
                if (n >= 2)
                {
                    xygl(x, y, "r0");
                    n -= 2;
                    x += 6;
                }
                if (n != 0)
                    xygl(x + 2, y, "r1");
                return 0;
            }

            if (!s.next)
            {
                error(1, s, "Lack of bar after multi-measure rest");
                return;
            }
            set_scale(s);
            anno_start(s);
            if (!cfmt.oldmrest || s.nmes > cfmt.oldmrest)
            {
                out_XYAB("<path d=\"mX Y", x1 + .6, y - 2.7);
                output += "v2.7h-1.4v-10.8h1.4v2.7h" + ((x2 - x1 - 2.8) / stv_g.scale).ToString("F1") + "v-2.7h1.4v10.8h-1.4v-2.7z\"/>\n";
            }
            else
            {
                omrest();
            }
            if (s.tacet)
                out_XYAB("<text x =\"X\" y=\"Y\" style=\"font-size:12px;font-weight:700\" text-anchor=\"middle\">A</text>\n", s.x, y + 18, s.tacet);
            else
                out_XYAB("<text x =\"X\" y=\"Y\" text-anchor=\"middle\">A</text>\n", s.x, y + 22, m_gl(p));
            anno_a.Add(s);
        }

        void grace_slur(VoiceItem s)
        {
            int yy, x0, y0, x3, y3, bet1, bet2, dy1, dy2, last, below;
            var so = s;
            var g = s.extra;

            while (true)
            {
                if (g.next == null)
                    break;
                g = g.next;
            }
            last = g;

            below = ((g.stem >= 0 || s.multi < 0) && g.notes[0].pit <= 28) || g.notes[0].pit < 16;
            if (below)
            {
                yy = 127;
                foreach (var g_ in s.extra)
                {
                    if (g_.y < yy)
                    {
                        yy = g_.y;
                        last = g_;
                    }
                }
                x0 = last.x;
                y0 = last.y - 5;
                if (s.extra != last)
                {
                    x0 -= 4;
                    y0 += 1;
                }
                s = s.next;
                x3 = s.x - 1;
                if (s.stem < 0 && s.nflags > -2)
                    x3 -= 4;
                y3 = 3 * (s.notes[0].pit - 18) - 5;
                dy1 = (x3 - x0) * 0.4;
                if (dy1 > 3)
                    dy1 = 3;
                dy2 = dy1;
                bet1 = 0.2;
                bet2 = 0.8;
                if (y0 > y3 + 7)
                {
                    x0 = last.x - 1;
                    y0 += 0.5;
                    y3 += 6.5;
                    x3 = s.x - 5.5;
                    dy1 = (y0 - y3) * 0.8;
                    dy2 = (y0 - y3) * 0.2;
                    bet1 = 0;
                }
                else if (y3 > y0 + 4)
                {
                    y3 = y0 + 4;
                    x0 = last.x + 2;
                    y0 = last.y - 4;
                }
            }
            else
            {
                yy = -127;
                foreach (var g_ in s.extra)
                {
                    if (g_.y > yy)
                    {
                        yy = g_.y;
                        last = g_;
                    }
                }
                x0 = last.x;
                y0 = last.y + 5;
                if (s.extra != last)
                {
                    x0 -= 4;
                    y0 -= 1;
                }
                s = s.next;
                x3 = s.x - 1;
                if (s.stem >= 0 && s.nflags > -2)
                    x3 -= 2;
                y3 = 3 * (s.notes[s.nhd].pit - 18) + 5;
                dy1 = (x0 - x3) * 0.4;
                if (dy1 < -3)
                    dy1 = -3;
                dy2 = dy1;
                bet1 = 0.2;
                bet2 = 0.8;
                if (y0 < y3 - 7)
                {
                    x0 = last.x - 1;
                    y0 -= 0.5;
                    y3 -= 6.5;
                    x3 = s.x - 5.5;
                    dy1 = (y0 - y3) * 0.8;
                    dy2 = (y0 - y3) * 0.2;
                    bet1 = 0;
                }
                else if (y3 < y0 - 4)
                {
                    y3 = y0 - 4;
                    x0 = last.x + 2;
                    y0 = last.y + 4;
                }
            }

            so.slur = new
            {
                x0 = x0,
                y0 = y0,
                x1 = bet1 * x3 + (1 - bet1) * x0 - x0,
                y1 = y0 - bet1 * y3 - (1 - bet1) * y0 + dy1,
                x2 = bet2 * x3 + (1 - bet2) * x0 - x0,
                y2 = y0 - bet2 * y3 - (1 - bet2) * y0 + dy2,
                x3 = x3 - x0,
                y3 = y0 - y3
            };
            y0 -= so.slur.y1;
            g = so.extra;
            if (below)
            {
                if (y0 < g.ymn)
                    g.ymn = y0;
            }
            else
            {
                if (y0 > g.ymx)
                    g.ymx = y0;
            }
        }

        /* -- draw grace notes -- */
        /* (the staves are defined) */
        void draw_gracenotes(VoiceItem s)
        {
            int x1, y1;
            VoiceItem last, note;
            var bm = new { s2 = default(VoiceItem) };
            var g = s.extra;

            while (true)
            {
                if (g.beam_st && !g.beam_end)
                {
                    if (calculate_beam(bm, g))
                        draw_beams(bm);
                }
                anno_start(g);
                draw_note(g, bm.s2 == null);
                if (g == bm.s2)
                    bm.s2 = null;
                anno_a.Add(s);
                if (g.next == null)
                    break;
                g = g.next;
            }
            last = g;

            if (s.sappo != null)
            {
                g = s.extra;
                if (g.next == null)
                {
                    x1 = 9;
                    y1 = g.stem > 0 ? 5 : -5;
                }
                else
                {
                    x1 = (g.next.x - g.x) * 0.5 + 4;
                    y1 = (g.ys + g.next.ys) * 0.5 - g.y;
                    if (g.stem > 0)
                        y1 -= 1;
                    else
                        y1 += 1;
                }
                note = g.notes[g.stem < 0 ? 0 : g.nhd];
                out_acciac(x_head(g, note), y_head(g, note), x1, y1, g.stem > 0);
            }

            g = s.slur;
            if (g != null)
            {
                anno_start(s, "slur");
                xypath(g.x0, g.y0 + staff_tb[s.st].y);
                output += "c" + g.x1.ToString("F1") + " " + g.y1.ToString("F1") + " " + g.x2.ToString("F1") + " " + g.y2.ToString("F1") + " " + g.x3.ToString("F1") + " " + g.y3.ToString("F1") + "\"/>\n";
                anno_stop(s, "slur");
            }
        }

        /* -- set the y offset of the dots -- */
        public static void setdoty(VoiceItem s, object[] y_tb)
        {
            int? m, m1, y;

            for (m = 0; m <= s.nhd; m++)
            {
                y = 3 * (s.notes[m].pit - 18);
                if ((y % 6) == 0)
                {
                    if (s.dot_low != null)
                        y -= 3;
                    else
                        y += 3;
                }
                y_tb[m] = y;
            }
            for (m = 0; m < s.nhd; m++)
            {
                if (y_tb[m + 1] > y_tb[m])
                    continue;
                m1 = m;
                while (m1 > 0)
                {
                    if (y_tb[m1] > y_tb[m1 - 1] + 6)
                        break;
                    m1--;
                }
                if (3 * (s.notes[m1].pit - 18) - y_tb[m1] < y_tb[m + 1] - 3 * (s.notes[m + 1].pit - 18))
                {
                    while (m1 <= m)
                        y_tb[m1++] -= 6;
                }
                else
                {
                    y_tb[m + 1] = y_tb[m] + 6;
                }
            }
        }

        // get the x and y position of a note head
        // (when the staves are defined)
        public double x_head(VoiceItem s, autocad_part2.NoteItem note)
        {
            return s.x + note.shhd * stv_g.scale;
        }
        public double y_head(VoiceItem s, autocad_part2.NoteItem note)
        {
            return staff_tb[s.st].y + 3 * (note.pit - 18);
        }

        /* -- draw m-th head with accidentals and dots -- */
        /* (the staves are defined) */
        // sets {x,y}_note
        public static void draw_basic_note(VoiceItem s, int? m, object[] y_tb)
        {
            int? i, p, yy, dotx, doty, inv;
            object old_color = false;
            noteItem note = s.notes[m];
            int? staffb = staff_tb[s.st].y;
            int? x = s.x;
            int? y = 3 * (note.pit - 18);
            int? shhd = note.shhd * stv_g.scale;
            int? x_note = x + shhd;
            int? y_note = y + staffb;

            var elts = identify_note(s, note.dur);
            int? head = elts[0];
            int? dots = elts[1];
            int? nflags = elts[2];

            if (note.invis != null)
            {
                ;
            }
            else if (s.grace != null)
            {
                p = "ghd";
                x_note -= 4.5 * stv_g.scale;
            }
            else if (note.map != null && note.map[0] != null && note.map[0][0] != null)
            {
                i = s.head;
                p = note.map[0][i];
                if (p == null)
                    p = note.map[0][note.map[0].Length - 1];
                i = p.IndexOf('/');
                if (i >= 0)
                {
                    if (s.stem >= 0)
                        p = p.Substring(0, i);
                    else
                        p = p.Substring(i + 1);
                }
            }
            else if (s.type == C.CUSTOS)
            {
                p = "custos";
            }
            else
            {
                switch (head)
                {
                    case C.OVAL:
                        p = "HD";
                        break;
                    case C.OVALBARS:
                        if (s.head != C.SQUARE)
                        {
                            p = "HDD";
                            break;
                        }
                        goto case C.SQUARE;
                    case C.SQUARE:
                        if (nflags > -4)
                        {
                            p = "breve";
                        }
                        else
                        {
                            p = "longa";
                            inv = s.stem > 0;
                        }
                        if (!tsnext && s.next != null && s.next.type == C.BAR && s.next.next == null)
                            dots = 0;
                        x_note += 1;
                        break;
                    case C.EMPTY:
                        p = "Hd";
                        break;
                    default:
                        p = "hd";
                        break;
                }
            }
            if (note.color != null)
                old_color = set_color(note.color);
            if (p != null)
            {
                if (inv != null)
                {
                    g_open(x_note, y_note, 0, 1, -1);
                    x_note = y_note = 0;
                }
                if (!self.psxygl(x_note, y_note, p))
                    xygl(x_note, y_note, p);
                if (inv != null)
                    g_close();
            }
            if (dots != null)
            {
                dotx = x + (7.7 + s.xmx) * stv_g.scale;
                if (y_tb[m] == null)
                {
                    y_tb[m] = 3 * (s.notes[m].pit - 18);
                    if ((s.notes[m].pit & 1) == 0)
                        y_tb[m] += 3;
                }
                doty = y_tb[m] + staffb;
                i = (note.dur / 12) >> ((5 - nflags) - dots);
                while (dots-- > 0)
                {
                    xygl(dotx, doty, (i & (1 << dots)) != 0 ? "dot" : "dot+");
                    dotx += 3.5;
                }
            }
            if (note.acc != null)
            {
                x -= note.shac * stv_g.scale;
                if (s.grace != null)
                {
                    draw_acc(x, y + staffb, note.acc);
                }
                else
                {
                    g_open(x, y + staffb, 0, .75);
                    draw_acc(0, 0, note.acc);
                    g_close();
                }
            }
            if (old_color != false)
                set_color(old_color);
        }

        /* -- draw a note or a chord -- */
        /* (the staves are defined) */
        public static void draw_note(VoiceItem s, bool? fl)
        {
            VoiceItem s2;
            int? i, m, y, slen, c, nflags;
            object[] y_tb = new object[s.nhd + 1];
            noteItem note = s.notes[s.stem < 0 ? s.nhd : 0];
            int? x = x_head(s, note);
            int? y = y_head(s, note);
            int? staffb = staff_tb[s.st].y;

            if (s.dots != null)
                setdoty(s, y_tb);

            if (!s.stemless)
            {
                slen = s.ys - s.y;
                nflags = s.nflags;
                if (s.ntrem != null)
                    nflags -= s.ntrem;
                if (!fl || nflags <= 0)
                {
                    if (s.nflags > 0)
                    {
                        if (s.stem >= 0)
                            slen -= 1;
                        else
                            slen += 1;
                    }
                    out_stem(x, y, slen, s.grace);
                }
                else
                {
                    out_stem(x, y, slen, s.grace, nflags, s.fmt.straightflags);
                }
            }
            else if (s.xstem != null)
            {
                s2 = s.ts_prev;
                slen = (s2.stem > 0 ? s2.y : s2.ys) - s.y;
                slen += staff_tb[s2.st].y - staffb;
                out_stem(x, y, slen);
            }

            if (fl && s.trem1 != null)
            {
                int? ntrem = s.ntrem ?? 0;
                int? x1 = x;
                slen = 3 * (s.notes[s.stem > 0 ? s.nhd : 0].pit - 18);
                if (s.head == C.FULL || s.head == C.EMPTY)
                {
                    x1 += (s.grace != null ? GSTEM_XOFF : 3.5) * s.stem;
                    if (s.stem > 0)
                        slen += 6 + 5.4 * ntrem;
                    else
                        slen -= 6 + 5.4;
                }
                else
                {
                    if (s.stem > 0)
                        slen += 5 + 5.4 * ntrem;
                    else
                        slen -= 5 + 5.4;
                }
                slen /= s.p_v.scale;
                out_trem(x1, staffb + slen, ntrem);
            }

            for (m = 0; m <= s.nhd; m++)
                draw_basic_note(s, m, y_tb);
        }

        // find where to start a long decoration
        int prev_scut(VoiceItem s)
        {
            while (s.prev != null)
            {
                s = s.prev;
                if (s.rbstart != null)
                    return s;
            }

            s = s.p_v.sym;
            while (s.type != C.CLEF)
                s = s.ts_prev;
            if (s.next != null && s.next.type == C.KEY)
                s = s.next;
            if (s.next != null && s.next.type == C.METER)
                return s.next;
            return s;
        }

        /* -- decide whether a slur goes up or down (same voice) -- */
        int slur_direction(VoiceItem k1, VoiceItem k2)
        {
            VoiceItem s;
            bool some_upstem, low;
            int dir;

            bool slur_multi(VoiceItem s1, VoiceItem s2)
            {
                if (s1.multi != null)
                    return s1.multi.Value;
                if (s2.multi != null)
                    return s2.multi.Value;
                return false;
            }

            if (k1.grace != null && k1.stem > 0)
                return -1;

            dir = slur_multi(k1, k2);
            if (dir != 0)
                return dir;

            for (s = k1; ; s = s.next)
            {
                if (s.type == C.NOTE)
                {
                    if (s.stemless != null)
                    {
                        if (s.stem < 0)
                            return 1;
                        some_upstem = true;
                    }
                    if (s.notes[0].pit < 22)
                        low = true;
                }
                if (s.time == k2.time)
                    break;
            }
            if (!some_upstem && !low)
                return 1;
            return -1;
        }

        /* -- output a slur / tie -- */
        void slur_out(int x1, int y1, int x2, int y2, int dir, int height, bool dotted)
        {
            int dx, dy, dz;
            double alfa = .3;
            double beta = .45;

            /* for wide flat slurs, make shape more square */
            dy = y2 - y1;
            if (dy < 0)
                dy = -dy;
            dx = x2 - x1;
            if (dx > 40.0 && dy / dx < .7)
            {
                alfa = .3 + .002 * (dx - 40.0);
                if (alfa > .7)
                    alfa = .7;
            }

            int mx = (int)(.5 * (x1 + x2));
            int my = (int)(.5 * (y1 + y2));
            int xx1 = (int)(mx + alfa * (x1 - mx));
            int yy1 = (int)(my + alfa * (y1 - my) + height);
            xx1 = x1 + (int)(beta * (xx1 - x1));
            yy1 = y1 + (int)(beta * (yy1 - y1));

            int xx2 = (int)(mx + alfa * (x2 - mx));
            int yy2 = (int)(my + alfa * (y2 - my) + height);
            xx2 = x2 + (int)(beta * (xx2 - x2));
            yy2 = y2 + (int)(beta * (yy2 - y2));

            dy = 2 * dir;
            dz = (int)(.2 + .001 * dx);
            if (dz > .6)
                dz = .6;
            dz *= dir;
            dx *= (int).03;

            int scale_y = 1;
            if (!dotted)
                output += "<path d=\"M";
            else
                output += "<path class=\"stroke\" stroke-dasharray=\"5,5\" d=\"";
            out_sxsy(x1, " ", y1);
            output += "c" +
                ((xx1 - x1) / stv_g.scale).ToString("F1") + " " +
                ((y1 - yy1) / scale_y).ToString("F1") + " " +
                ((xx2 - x1) / stv_g.scale).ToString("F1") + " " +
                ((y1 - yy2) / scale_y).ToString("F1") + " " +
                ((x2 - x1) / stv_g.scale).ToString("F1") + " " +
                ((y1 - y2) / scale_y).ToString("F1");

            if (!dotted)
                output += "\n\tv" +
                    (-dz).ToString("F1") + "c" +
                    ((xx2 - dx - x2) / stv_g.scale).ToString("F1") + " " +
                    ((y2 + dz - yy2 - dy) / scale_y).ToString("F1") + " " +
                    ((xx1 + dx - x2) / stv_g.scale).ToString("F1") + " " +
                    ((y2 + dz - yy1 - dy) / scale_y).ToString("F1") + " " +
                    ((x1 - x2) / stv_g.scale).ToString("F1") + " " +
                    ((y2 - y1) / scale_y).ToString("F1");
            output += "\"/>\n";
        }

        // draw a slur between two chords / notes
        /* (the staves are not yet defined) */
        /* (delayed output) */
        /* (not a pretty routine, this) */
        /* 字元數：12213 */
        void draw_slur(List<VoiceItem> path, // list of symbols under the slur
            VoiceItem sl, // slur variables: type, end symbol, note
            bool recurr)    // recurrent call when slur on two staves
        {
            int i, k, g;
                double x1, y1, x2, y2, height, y, z, h, dx, dy;
            int addy, s_st2, a;
            string ty = sl.type.Value;
            int dir = (ty & 0x07) == C.SL_ABOVE ? 1 : -1;
            int n = path.Count, i1 = 0, i2 = n - 1, not1 = sl.nhd.Value,  nn = 1;
            VoiceItem k1 = path[0], k2 = path[i2];
            set_dscale(k1.st.Value);

            for (i = 1; i < n; i++)
            {
                k = (int)path[i];
                if (((VoiceItem)k).type == C.NOTE || ((VoiceItem)k).type == C.REST)
                {
                    nn++;
                    if (((VoiceItem)k).st != k1.st && s_st2 == 0)
                        s_st2 = k;
                }
            }

            if (s_st2 != 0 && !recurr)
            {
                if (gene.a_sl == null)
                    gene.a_sl = new List<object[]>();

                h = 24 + k1.fmt.sysstaffsep;
                if (s_st2.st > k1.st)
                    h = -h;
                for (i = 0; i < n; i++)
                {
                    k = (int)path[i];
                    if (((VoiceItem)k).st == k1.st)
                    {
                        if (((VoiceItem)k).dur != null)
                            a = k;
                        continue;
                    }
                    k = clone(k);
                    if (path[i] == s_st2)
                        s_st2 = k;
                    path[i] = k;
                    if (((VoiceItem)k).dur != null)
                    {
                        k.notes = clone(k.notes);
                        k.notes[0] = clone(k.notes[0]);
                        if ((sl.type & C.SL_CENTER) != 0)
                        {
                            if (((VoiceItem)k).st != a.st)
                            {
                                sl.type = (sl.type & ~0x07) | (a.st < ((VoiceItem)k).st ? C.SL_BELOW : C.SL_ABOVE);
                                z = k1.ymn;
                                h = k2.ymx;
                                if (((VoiceItem)k).st < a.st)
                                {
                                    for (i1 = 1; i1 < i; i1++)
                                    {
                                        a = (int)path[i1];
                                        if (((VoiceItem)a).ymn < z)
                                            z = ((VoiceItem)a).ymn;
                                    }
                                    for (i1 = i; i1 < i2; i1++)
                                    {
                                        a = (int)path[i1];
                                        if (((VoiceItem)a).ymx > h)
                                            h = ((VoiceItem)a).ymx;
                                    }
                                }
                                else
                                {
                                    for (i1 = 1; i1 < i; i1++)
                                    {
                                        a = (int)path[i1];
                                        if (((VoiceItem)a).ymx > h)
                                            h = ((VoiceItem)a).ymx;
                                    }
                                    for (i1 = i; i1 < i2; i1++)
                                    {
                                        a = (int)path[i1];
                                        if (((VoiceItem)a).ymn < z)
                                            z = ((VoiceItem)a).ymn;
                                    }
                                }
                                h += z;
                                a = k;
                            }
                            k.y = h - k.y;
                            k.notes[0].pit = (k.y / 3 | 0) + 18;
                            k.ys = h - k.ys;
                            y = k.ymx;
                            k.ymx = h - k.ymn;
                            k.ymn = h - y;
                            k.stem = -k.stem;
                        }
                        else
                        {
                            k.notes[0].pit += h / 3 | 0;
                            k.ys += h;
                            k.y += h;
                            k.ymx += h;
                            k.ymn += h;
                        }
                    }
                }

                ty = k1.st > s_st2.st ? '/' : '\\';
                if ((sl.type & C.SL_CENTER) != 0)
                    ty = ty + ty;
                else if (k1.st == k2.st)
                    ty = ty == '/' ? '/\\' : '\\/';
                else
                    ty += dir > 0 ? '+' : '-';
                string savout = output;
                output = "";
                draw_slur(path, sl, true);
                gene.a_sl.Add(new object[] { k1, s_st2, ty, output });
                output = savout;
                return;
            }

            x1 = k1.x.Value;
            if (((VoiceItem)k1).notes != null && ((VoiceItem)k1).notes[0].shhd != null)
                x1 += ((VoiceItem)k1).notes[0].shhd.Value;
            x2 = k2.x.Value;
            if (((VoiceItem)k2).notes != null)
                x2 += ((VoiceItem)k2).notes[0].shhd.Value;

            if (not1 != 0)
            {
                y1 = 3 * (not1 - 18) + 2 * dir;
                x1 += 3;
            }
            else
            {
                y1 = dir > 0 ? ((VoiceItem)k1).ymx.Value + 2 : ((VoiceItem)k1).ymn.Value - 2;
                if (((VoiceItem)k1).type == C.NOTE)
                {
                    if (dir > 0)
                    {
                        if (((VoiceItem)k1).stem > 0)
                        {
                            x1 += 5;
                            if (((VoiceItem)k1).beam_end != null && ((VoiceItem)k1).nflags >= -1 && !((VoiceItem)k1).in_tuplet)
                            {
                                if (((VoiceItem)k1).nflags > 0)
                                {
                                    x1 += 2;
                                    y1 = ((VoiceItem)k1).ys.Value - 3;
                                }
                                else
                                {
                                    y1 = ((VoiceItem)k1).ys.Value - 6;
                                }
                            }
                            else
                            {
                                y1 = ((VoiceItem)k1).ys.Value + 3;
                            }
                        }
                        else
                        {
                            y1 = ((VoiceItem)k1).y.Value + 8;
                        }
                    }
                    else
                    {
                        if (((VoiceItem)k1).stem < 0)
                        {
                            x1 -= 1;
                            if (((VoiceItem)k2).grace != null)
                            {
                                y1 = ((VoiceItem)k1).y.Value - 8;
                            }
                            else if (((VoiceItem)k1).beam_end != null && ((VoiceItem)k1).nflags >= -1 && (!((VoiceItem)k1).in_tuplet || ((VoiceItem)k1).ys < y1 + 3))
                            {
                                if (((VoiceItem)k1).nflags > 0)
                                {
                                    x1 += 2;
                                    y1 = ((VoiceItem)k1).ys.Value + 3;
                                }
                                else
                                {
                                    y1 = ((VoiceItem)k1).ys.Value + 6;
                                }
                            }
                            else
                            {
                                y1 = ((VoiceItem)k1).ys.Value - 3;
                            }
                        }
                        else
                        {
                            y1 = ((VoiceItem)k1).y.Value - 8;
                        }
                    }
                }
            }

            if (not1 != 0)
            {
                y2 = 3 * (not1 - 18) + 2 * dir;
                x2 -= 3;
            }
            else
            {
                y2 = dir > 0 ? ((VoiceItem)k2).ymx.Value + 2 : ((VoiceItem)k2).ymn.Value - 2;
                if (((VoiceItem)k2).type == C.NOTE)
                {
                    if (dir > 0)
                    {
                        if (((VoiceItem)k2).stem > 0)
                        {
                            x2 += 1;
                            if (((VoiceItem)k2).beam_st != null && ((VoiceItem)k2).nflags >= -1 && !((VoiceItem)k2).in_tuplet)
                                y2 = ((VoiceItem)k2).ys.Value - 6;
                            else
                                y2 = ((VoiceItem)k2).ys.Value + 3;
                        }
                        else
                        {
                            y2 = ((VoiceItem)k2).y.Value + 8;
                        }
                    }
                    else
                    {
                        if (((VoiceItem)k2).stem < 0)
                        {
                            x2 -= 5;
                            if (((VoiceItem)k2).beam_st != null && ((VoiceItem)k2).nflags >= -1 && !((VoiceItem)k2).in_tuplet)
                                y2 = ((VoiceItem)k2).ys.Value + 6;
                            else
                                y2 = ((VoiceItem)k2).ys.Value - 3;
                        }
                        else
                        {
                            y2 = ((VoiceItem)k2).y.Value - 8;
                        }
                    }
                }
            }

            if (((VoiceItem)k1).type != C.NOTE)
            {
                y1 = y2 + (int)(1.2 * dir);
                x1 = ((VoiceItem)k1).x.Value + ((VoiceItem)k1).wr.Value * 5 / 10;
                if (x1 > x2 - 12)
                    x1 = x2 - 12;
            }

            if (((VoiceItem)k2).type != C.NOTE)
            {
                if (((VoiceItem)k1).type == C.NOTE)
                    y2 = y1 + (int)(1.2 * dir);
                else
                    y2 = y1;
                if (k1 != k2)
                    x2 = ((VoiceItem)k2).x.Value - ((VoiceItem)k2).wl.Value * 3 / 10;
            }

            if (nn >= 3)
                height = (int)((.08 * (x2 - x1) + 12) * dir);
            else
                height = (int)((.03 * (x2 - x1) + 8) * dir);
            if (dir > 0)
            {
                if (height < 3 * h)
                    height = 3 * h;
                if (height > 40)
                    height = 40;
            }
            else
            {
                if (height > 3 * h)
                    height = 3 * h;
                if (height < -40)
                    height = -40;
            }

            y = y2 - y1;
            if (y < 0)
                y = -y;
            if (dir > 0)
            {
                if (height < .8 * y)
                    height = (int)(.8 * y);
            }
            else
            {
                if (height > -.8 * y)
                    height = (int)(-.8 * y);
            }
            height *= k1.fmt.Value.slurheight;

            slur_out(x1, y1, x2, y2, dir, height, (ty & C.SL_DOTTED) != 0);

            dx = x2 - x1;
            a = (y2 - y1) / dx;
            addy = y1 - a * x1;
            if (height > 0)
                addy += 4 * Math.Sqrt(height) - 2;
            else
                addy -= 4 * Math.Sqrt(-height) - 2;
            for (i = 0; i < i2; i++)
            {
                k = (int)path[i];
                if (((VoiceItem)k).st != k1.st || ((VoiceItem)k).type == C.BAR)
                    continue;
                y = a * ((VoiceItem)k).x.Value + addy;
                if (((VoiceItem)k).ymx.Value < y)
                    ((VoiceItem)k).ymx = y;
                else if (((VoiceItem)k).ymn.Value > y)
                    ((VoiceItem)k).ymn = y;
                if (recurr)
                    continue;
                if (i == i2 - 1)
                {
                    dx = x2;
                    if (sl.nte != null)
                        dx -= 5;
                }
                else
                {
                    dx = ((VoiceItem)k).x.Value + ((VoiceItem)k).wr.Value;
                }
                if (i != 0)
                    x1 = ((VoiceItem)k).x.Value;
                if (i == 0 || i == i2)
                    y -= height / 3;
                dx -= x1 - ((VoiceItem)k).wl.Value;
                y_set(k1.st.Value, dir > 0, x1 - ((VoiceItem)k).wl.Value, dx, y);
            }
        }

        /* -- draw the slurs between 2 symbols --*/
        /* 字元數：5677 */
        void draw_slurs(VoiceItem s, VoiceItem last)
        {
            int gr1, i, m, note;
            List<SlurGroup> sls,   nsls;
            /* -- draw the slurs between 2 symbols --*/

            void draw_sls(VoiceItem s, SlurGroup sl)
            {
                int k, v, i, dir;
                List<SlurGroup> path = new List<SlurGroup>();
                VoiceItem s2 = sl.se,s3;

                if (last != null && s2.time > last.time)
                    return;

                switch (sl.loc)
                {
                    case 'i':
                        s = prev_scut(s);
                        break;
                    case 'o':
                        for (s3 = s; s3.ts_next != null; s3 = s3.ts_next) ;
                        s2 = s3;
                        for (; s3 != null; s3 = s3.ts_prev)
                        {
                            if (s3.v == s.v)
                            {
                                s2 = s3;
                                break;
                            }
                            if (s3.st == s.st)
                                s2 = s3;
                            if (s3.ts_prev.time != s2.time)
                                break;
                        }
                        break;
                }

                if (s.p_v.s_next != null && s2.time >= tsnext.time)
                {
                    if (s2.time == tsnext.time)
                    {
                        if (s2.grace)
                        {
                            for (s3 = tsnext; s3 != null && s3.time == s2.time; s3 = s3.ts_next)
                            {
                                if (s3.type == C.GRACE)
                                {
                                    s3 = null;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (s3 = tsnext; s3.time == s2.time; s3 = s3.ts_next)
                            {
                                if (s3 == s2)
                                {
                                    s3 = null;
                                    break;
                                }
                            }
                        }
                    }
                    else
                        s3 = null;
                    if (s3 == null)
                    {
                        s.p_v.sls.Add(sl);
                        s2 = s.p_v.s_next.prev;
                        while (s2.next != null)
                            s2 = s2.next;
                        sl = Clone(sl);
                    }
                }

                switch (sl.ty & 0x07)
                {
                    case C.SL_ABOVE:
                        dir = 1;
                        break;
                    case C.SL_BELOW:
                        dir = -1;
                        break;
                    default:
                        dir = s.v != s2.v ? 1 : SlurDirection(s, s2);
                        sl.ty &= ~0x07;
                        sl.ty |= dir > 0 ? C.SL_ABOVE : C.SL_BELOW;
                        break;
                }

                if (s.v == s2.v)
                    v = s.v;
                else if (cur_sy.voices[s.v] == null || cur_sy.voices[s2.v] == null)
                    v = s.v > s2.v ? s.v : s2.v;
                else if (dir * (cur_sy.voices[s.v].range <= cur_sy.voices[s2.v].range ? 1 : -1) > 0)
                    v = s.v;
                else
                    v = s2.v;

                if (gr1 != null)
                {
                    do
                    {
                        path.Add(s);
                        s = s.next;
                    } while (s != null);
                    s = gr1.next;
                }
                else
                {
                    path.Add(s);
                    if (s.grace)
                        s = s.next;
                    else
                        s = s.ts_next;
                }

                if (!s2.grace)
                {
                    while (s != null)
                    {
                        if (s.v == v)
                            path.Add(s);
                        if (s == s2)
                            break;
                        s = s.ts_next;
                    }
                }
                else if (s.grace)
                {
                    while (true)
                    {
                        path.Add(s);
                        if (s == s2)
                            break;
                        s = s.next;
                    }
                }
                else
                {
                    k = s2;
                    while (k.prev != null)
                        k = k.prev;
                    while (true)
                    {
                        if (s.v == v)
                            path.Add(s);
                        if (s.extra == k)
                            break;
                        s = s.ts_next;
                    }
                    s = k;
                    while (true)
                    {
                        path.Add(s);
                        if (s == s2)
                            break;
                        s = s.next;
                    }
                }

                for (i = 1; i < path.Count - 1; i++)
                {
                    s = path[i];
                    if (s.sls != null)
                        draw_slurs(s, last);
                    if (s.tp != null)
                        DrawTuplet(s);
                }
                DrawSlur(path, sl);
                return 1;
            }

            while (true)
            {
                if (s == null || s == last)
                {
                    if (gr1 == null || !(s = gr1.next) || s == last)
                        break;
                    gr1 = null;
                }
                if (s.type == C.GRACE)
                {
                    gr1 = s;
                    s = s.extra;
                    continue;
                }
                if (s.sls != null)
                {
                    sls = s.sls;
                    s.sls = null;
                    nsls = new List<SlurGroup>();
                    for (i = 0; i < sls.Count; i++)
                    {
                        if (!DrawSls(s, sls[i]))
                            nsls.Add(sls[i]);
                    }
                    if (nsls.Count > 0)
                        s.sls = nsls;
                }
                s = s.next;
            }
        }

        /* -- draw a tuplet -- */
        /* (the staves are not yet defined) */
        /* (delayed output) */
        /* See http://moinejf.free.fr/abcm2ps-doc/tuplets.html
         * for the value of 'tp.f' */
        public static void draw_tuplet(VoiceItem s1)
        {
            VoiceItem s2, s3;
            object g, upstaff, nb_only;
            double x1, x2, y1, y2, xm, ym, yy, yx, dy;
            int    a, s0,  dir, r;
            var tp = s1.tp.Shift();

            if (tp.f[0] == 1)
                return;

            if (s1.tp.Length == 0)
                s1.tp = null;

            upstaff = s1.st;
            set_dscale(s1.st);
            for (s2 = s1; s2 != null; s2 = s2.next)
            {
                switch (s2.type)
                {
                    case C.GRACE:
                        if (s2.sl1 == null)
                            continue;
                        foreach (g in s2.extra)
                        {
                            if (g.sls != null)
                                draw_slurs(g);
                        }
                        goto default;
                    default:
                        continue;
                    case C.NOTE:
                    case C.REST:
                        break;
                }
                if (s2.sls != null)
                    draw_slurs(s2);
                if (s2.st < upstaff)
                    upstaff = s2.st;
                if (s2.tp != null)
                    draw_tuplet(s2);
                if (s2.tpe != null)
                    break;
            }

            if (s2 != null)
                s2.tpe--;

            if (tp.f[0] != 2)
            {
                if (s1 == s2 || tp.f[1] == 2)
                    nb_only = true;
                else if (tp.f[1] == 1)
                {
                    nb_only = true;
                    draw_slur(new[] { s1, s2 }, new { ty = dir });
                }
                else
                {
                    nb_only = true;
                    for (s3 = s1; ; s3 = s3.next)
                    {
                        if (s3.type != C.NOTE && s3.type != C.REST)
                        {
                            if (s3.type == C.GRACE || s3.type == C.SPACE)
                                continue;
                            nb_only = false;
                            break;
                        }
                        if (s3 == s2)
                            break;
                        if (s3.beam_end != null)
                        {
                            nb_only = false;
                            break;
                        }
                    }
                    if (nb_only && s1.beam_st == null && s1.beam_br1 == null && s1.beam_br2 == null)
                    {
                        for (s3 = s1.prev; s3 != null; s3 = s3.prev)
                        {
                            if (s3.type == C.NOTE || s3.type == C.REST)
                            {
                                if (s3.nflags >= s1.nflags)
                                    nb_only = false;
                                break;
                            }
                        }
                    }
                    if (nb_only && s2.beam_end == null)
                    {
                        for (s3 = s2.next; s3 != null; s3 = s3.next)
                        {
                            if (s3.type == C.NOTE || s3.type == C.REST)
                            {
                                if (s3.beam_br1 == null && s3.beam_br2 == null && s3.nflags >= s2.nflags)
                                    nb_only = false;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                nb_only = false;
            }

            if (nb_only)
            {
                if (tp.f[2] == 1)
                    return;

                set_font("tuplet");
                xm = (s2.x + s1.x) / 2;
                if (dir == C.SL_ABOVE)
                    ym = y_get(upstaff, 1, xm - 4, 8);
                else
                    ym = y_get(upstaff, 0, xm - 4, 8) - gene.curfont.size;

                if (s1.stem * s2.stem > 0)
                {
                    if (s1.stem > 0)
                        xm += 1.5;
                    else
                        xm -= 1.5;
                }

                if (tp.f[2] == 0)
                    xy_str(xm, ym, tp.p.ToString(), 'c');
                else
                    xy_str(xm, ym, tp.p + ':' + tp.q, 'c');

                for (s3 = s1; ; s3 = s3.next)
                {
                    if (s3.x >= xm)
                        break;
                }
                if (dir == C.SL_ABOVE)
                {
                    ym += gene.curfont.size;
                    if (s3.ymx < ym)
                        s3.ymx = ym;
                    y_set(upstaff, true, xm - 3, 6, ym);
                }
                else
                {
                    if (s3.ymn > ym)
                        s3.ymn = ym;
                    y_set(upstaff, false, xm - 3, 6, ym);
                }
                return;
            }

             x1 = s1.x - 4;

            if (s2.dur > s2.prev.dur)
            {
                s3 = s2.next;
                if (s3 == null || s3.time != s2.time + s2.dur)
                {
                    for (s3 = s2.ts_next; s3 != null; s3 = s3.ts_next)
                    {
                        if (s3.seqst && s3.time >= s2.time + s2.dur)
                            break;
                    }
                }
                x2 = s3 != null ? s3.x - s3.wl - 5 : realwidth - 6;
            }
            else
            {
                x2 = s2.x + 4;
                r = s2.stem >= 0 ? 0 : s2.nhd;
                if (s2.notes[r].shhd > 0)
                    x2 += s2.notes[r].shhd;
                if (s2.st == upstaff && s2.stem > 0)
                    x2 += 3.5;
            }

            if (dir == C.SL_ABOVE)
            {
                int? y1, y2, xm, ym, a, s0, yy, yx, dy, dir, r;
                if (s1.st == s2.st)
                {
                    y1 = y2 = staff_tb[upstaff].topbar + 2;
                }
                else
                {
                    y1 = s1.ymx;
                    y2 = s2.ymx;
                }

                if (s1.st == upstaff)
                {
                    for (s3 = s1; !s3.dur; s3 = s3.next)
                        ;
                    ym = y_get(upstaff, 1, s3.x - 4, 8);
                    if (ym > y1)
                        y1 = ym;
                    if (s1.stem > 0)
                        x1 += 3;
                }

                if (s2.st == upstaff)
                {
                    for (s3 = s2; !s3.dur; s3 = s3.prev)
                        ;
                    ym = y_get(upstaff, 1, s3.x - 4, 8);
                    if (ym > y2)
                        y2 = ym;
                }

                xm = .5 * (x1 + x2);
                ym = .5 * (y1 + y2);

                a = (y2 - y1) / (x2 - x1);
                s0 = 3 * (s2.notes[s2.nhd].pit - s1.notes[s1.nhd].pit) / (x2 - x1);
                if (s0 > 0)
                {
                    if (a < 0)
                        a = 0;
                    else if (a > s0)
                        a = s0;
                }
                else
                {
                    if (a > 0)
                        a = 0;
                    else if (a < s0)
                        a = s0;
                }
                a = s1.fmt.beamslope * a / (s1.fmt.beamslope + Math.Abs(a));
                if (a * a < .1 * .1)
                    a = 0;

                dy = 0;
                for (s3 = s1; ; s3 = s3.next)
                {
                    if (!s3.dur || s3.st != upstaff)
                    {
                        if (s3 == s2)
                            break;
                        continue;
                    }
                    yy = ym + (s3.x - xm) * a;
                    yx = y_get(upstaff, 1, s3.x - 4, 8) + 2;
                    if (yx - yy > dy)
                        dy = yx - yy;
                    if (s3 == s2)
                        break;
                }

                ym += dy;
                y1 = ym + a * (x1 - xm);
                y2 = ym + a * (x2 - xm);

                ym += 6;
                for (s3 = s1; ; s3 = s3.next)
                {
                    if (s3.st == upstaff)
                    {
                        yy = ym + (s3.x - xm) * a;
                        if (s3.ymx < yy)
                            s3.ymx = yy;
                        y_set(upstaff, true, s3.x - 3, 6, yy);
                    }
                    if (s3 == s2)
                        break;
                }
            }
            else
            {
                int? y1, y2, xm, ym, a, s0, yy, yx, dy, dir, r;
                if (s1.stem < 0)
                    x1 -= 2;

                if (s1.st == upstaff)
                {
                    for (s3 = s1; !s3.dur; s3 = s3.next)
                        ;
                    y1 = y_get(upstaff, 0, s3.x - 4, 8);
                }
                else
                {
                    y1 = 0;
                }
                if (s2.st == upstaff)
                {
                    for (s3 = s2; !s3.dur; s3 = s3.prev)
                        ;
                    y2 = y_get(upstaff, 0, s3.x - 4, 8);
                }
                else
                {
                    y2 = 0;
                }

                xm = .5 * (x1 + x2);
                ym = .5 * (y1 + y2);

                a = (y2 - y1) / (x2 - x1);
                s0 = 3 * (s2.notes[0].pit - s1.notes[0].pit) / (x2 - x1);
                if (s0 > 0)
                {
                    if (a < 0)
                        a = 0;
                    else if (a > s0)
                        a = s0;
                    if (a > .35)
                        a = .35;
                }
                else
                {
                    if (a > 0)
                        a = 0;
                    else if (a < s0)
                        a = s0;
                    if (a < -.35)
                        a = -.35;
                }
                if (a * a < .1 * .1)
                    a = 0;

                dy = 0;
                for (s3 = s1; ; s3 = s3.next)
                {
                    if (!s3.dur || s3.st != upstaff)
                    {
                        if (s3 == s2)
                            break;
                        continue;
                    }
                    yy = ym + (s3.x - xm) * a;
                    yx = y_get(upstaff, 0, s3.x - 4, 8);
                    if (yx - yy < dy)
                        dy = yx - yy;
                    if (s3 == s2)
                        break;
                }

                ym += dy - 8;
                y1 = ym + a * (x1 - xm);
                y2 = ym + a * (x2 - xm);

                ym -= 2;
                for (s3 = s1; ; s3 = s3.next)
                {
                    if (s3.st == upstaff)
                    {
                        yy = ym + (s3.x - xm) * a;
                        if (s3.ymn > yy)
                            s3.ymn = yy;
                        y_set(upstaff, false, s3.x - 3, 6, yy);
                    }
                    if (s3 == s2)
                        break;
                }
            }

            if (tp.f[2] == 1)
            {
                out_tubr(x1, y1 + 4, x2 - x1, y2 - y1, dir == C.SL_ABOVE);
                return;
            }
            out_tubrn(x1, y1, x2 - x1, y2 - y1, dir == C.SL_ABOVE, tp.f[2] == 0 ? tp.p.ToString() : tp.p + ':' + tp.q);

            if (dir == C.SL_ABOVE)
                y_set(upstaff, true, xm - 3, 6, yy + 2);
            else
                y_set(upstaff, false, xm - 3, 6, yy);
        }

        // -- draw a ties --
        void draw_tie(int not1, int not2, int job)
        {
            int m, x1, y, h, time, p = job == 2 ? not1 : not2, dir = (not1 & 0x07) == C.SL_ABOVE ? 1 : -1, s1 = not1.s, st = s1.st, s2 = not2.s, x2 = s2.x, sh = not1.shhd;

            for (m = 0; m < s1.nhd; m++)
                if (s1.notes[m] == not1)
                    break;
            if (dir > 0)
            {
                if (m < s1.nhd && p + 1 == s1.notes[m + 1].pit)
                    if (s1.notes[m + 1].shhd > sh)
                        sh = s1.notes[m + 1].shhd;
            }
            else
            {
                if (m > 0 && p == s1.notes[m - 1].pit + 1)
                    if (s1.notes[m - 1].shhd > sh)
                        sh = s1.notes[m - 1].shhd;
            }
            x1 = s1.x + sh;

            if (job != 2)
            {
                for (m = 0; m < s2.nhd; m++)
                    if (s2.notes[m] == not2)
                        break;
                sh = s2.notes[m].shhd;
                if (dir > 0)
                {
                    if (m < s2.nhd && p + 1 == s2.notes[m + 1].pit)
                        if (s2.notes[m + 1].shhd < sh)
                            sh = s2.notes[m + 1].shhd;
                }
                else
                {
                    if (m > 0 && p == s2.notes[m - 1].pit + 1)
                        if (s2.notes[m - 1].shhd < sh)
                            sh = s2.notes[m - 1].shhd;
                }
                x2 += sh;
            }

            switch (job)
            {
                default:
                    if (p < not2 || dir < 0)
                        p = not1;
                    break;
                case 3:
                    dir = -dir;
                    goto case 1;
                case 1:
                    x1 = s2.prev != null ? (s2.prev.x + s2.wr) : s1.x;
                    if (s1.st != s2.st)
                        st = s2.st;
                    x1 += (x2 - x1) * 4 / 10;
                    if (x1 > x2 - 20)
                        x1 = x2 - 20;
                    break;
                case 2:
                    x2 = s1.next != null ? s1.next.x : realwidth;
                    if (x2 != realwidth)
                        x2 -= (x2 - x1) * 4 / 10;
                    if (x2 < x1 + 16)
                        x2 = x1 + 16;
                    break;
            }
            if (x2 - x1 > 20)
            {
                x1 += 3;
                x2 -= 3;
            }
            else
            {
                x1 += 1;
                x2 -= 1;
            }

            if (s1.dots != null && (p & 1) == 0 && ((dir > 0 && s1.dot_low == null) || (dir < 0 && s1.dot_low != null)))
                x1 += 5;

            y = staff_tb[st].y + 3 * (p - 18) + dir;

            h = (int)((.03 * (x2 - x1) + 16) * dir * s1.fmt.Value.tieheight);
            slur_out(x1, y, x2, y, dir, h, (not1 & C.SL_DOTTED) != 0);
        }

        /* -- draw all ties between neighboring notes -- */
        /* -- 繪製相鄰音符之間的所有聯繫 -- */
        void draw_all_ties(VoiceItem p_voice)
        {
            VoiceItem s, s1, s2;
            bool clef_chg;
            int x, dx, m;
            noteItem not1, not2;
            int tim2 = 0;

            s1 = p_voice.sym;
            set_color(s1.color);
            for (; s1 != null; s1 = s1.next)
            {
                if (s1.ti2 != null && s1.time != tim2)
                {
                    for (m = 0; m <= s1.nhd; m++)
                    {
                        not2 = s1.notes[m];
                        not1 = not2.tie_s;
                        if (not1 == null || not1.s.v != s1.v)
                            continue;
                        draw_tie(not1, not2, 1);
                    }
                }
                if (!s1.ti1)
                    continue;
                if (s1.type == C.GRACE)
                {
                    for (s = s1.extra; s != null; s = s.next)
                    {
                        for (m = 0; m <= s1.nhd; m++)
                        {
                            not1 = s.notes[m];
                            not2 = not1.tie_e;
                            if (not2 == null)
                                continue;
                            draw_tie(not1, not2);
                            tim2 = not2.s.time;
                        }
                    }
                    continue;
                }
                for (m = 0; m <= s1.nhd; m++)
                {
                    not1 = s1.notes[m];
                    not2 = not1.tie_e;
                    if (not2 == null)
                    {
                        if (not1.tie_ty != null)
                            draw_tie(not1, not1, 2);
                        continue;
                    }
                    s2 = not2.s;
                    if (tsnext != null && s2.time >= tsnext.time)
                    {
                        draw_tie(not1, not2, 2);
                        continue;
                    }
                    tim2 = s2.time;
                    for (s = s1.ts_next; s != s2; s = s.ts_next)
                    {
                        if (s.st != s1.st)
                            continue;
                        if (s.type == C.CLEF)
                        {
                            clef_chg = true;
                            break;
                        }
                    }
                    if (clef_chg || s1.st != s2.st)
                    {
                        draw_tie(not1, not2, 2);
                        draw_tie(not1, not2, 3);
                        clef_chg = false;
                    }
                    else
                    {
                        draw_tie(not1, not2);
                    }
                }
            }
        }

        /* -- draw the symbols near the notes -- */
        /* (the staves are not yet defined) */
        /* order:
         * - scaled
         *   - beams
         *   - decorations near the notes
         *   - decorations tied to the notes
         *   - tuplets and slurs
         * - not scaled
         *   - measure numbers
         *   - lyrics
         *   - staff decorations
         *   - chord symbols
         *   - repeat brackets
         *   - parts and tempos
         * The buffer output is delayed until the definition of the staff system
         */
        void draw_sym_near()
        {
            VoiceItem s;
            VoiceItem s1;
            VoiceItem s2;
            VoiceItem p_voice;
            VoiceItem p_st;
            int v;
            int st;
            int y;
            int g;
            int w;
            int i;
            int dx;
            int top;
            int bot;
            int ymn;
            string output_sav = output;

            void set_yab(VoiceItem s1, VoiceItem s2)
            {
                int y;
                int k = (int)(realwidth / YSTEP);
                int i = (int)(s1.x / k);
                int j = (int)(s2.x / k);
                double a = (s1.ys - s2.ys) / (s1.xs - s2.xs);
                double b = s1.ys - s1.xs * a;
                p_st = staff_tb[s1.st];

                k *= a;
                if (s1.stem > 0)
                {
                    while (i <= j)
                    {
                        y = (int)(k * i + b);
                        if (p_st.top[i] < y)
                            p_st.top[i] = y;
                        i++;
                    }
                }
                else
                {
                    while (i <= j)
                    {
                        y = (int)(k * i + b);
                        if (p_st.bot[i] > y)
                            p_st.bot[i] = y;
                        i++;
                    }
                }
            }

            output = "";
            YSTEP = (int)Math.Ceiling(realwidth / 2);

            for (st = stl.Length; --st >= 0;)
            {
                if (stl[st])
                    break;
            }
            if (st < 0)
                return;

            for (v = 0; v < voice_tb.Length; v++)
            {
                var bm = new { s2 = (VoiceItem)null };
                bool first_note = true;

                p_voice = voice_tb[v];
                for (s = p_voice.sym; s != null; s = s.next)
                {
                    switch (s.type)
                    {
                        case C.GRACE:
                            for (g = s.extra; g != null; g = g.next)
                            {
                                if (g.beam_st != null && !g.beam_end)
                                {
                                    calculate_beam(bm, g);
                                    if (bm.s2 != null)
                                        set_yab(g, bm.s2);
                                }
                            }
                            if (!s.p_v.ckey.k_bagpipe && s.fmt.graceslurs && !s.gr_shift && !s.sl1 && !s.ti1 && s.next != null && s.next.type == C.NOTE)
                                grace_slur(s);
                            break;
                    }
                }
                for (s = p_voice.sym; s != null; s = s.next)
                {
                    switch (s.type)
                    {
                        case C.NOTE:
                            if ((s.beam_st != null && !s.beam_end) || (first_note && !s.beam_st))
                            {
                                first_note = false;
                                calculate_beam(bm, s);
                                if (bm.s2 != null)
                                    set_yab(s, bm.s2);
                            }
                            break;
                    }
                }
            }

            set_tie_room();
            draw_deco_near();

            for (s = tsfirst; s != null; s = s.ts_next)
            {
                if (s.invis)
                    continue;
                switch (s.type)
                {
                    case C.GRACE:
                        for (g = s.extra; g != null; g = g.next)
                        {
                            y_set(s.st, true, g.x - 2, 4, g.ymx + 1);
                            y_set(s.st, false, g.x - 2, 4, g.ymn - 5);
                        }
                        continue;
                    case C.MREST:
                        y_set(s.st, true, s.x + 16, 32, s.ymx + 2);
                        continue;
                    default:
                        y_set(s.st, true, s.x - s.wl, s.wl + s.wr, s.ymx + 2);
                        y_set(s.st, false, s.x - s.wl, s.wl + s.wr, s.ymn - 2);
                        continue;
                    case C.NOTE:
                        break;
                }
                if (s.stem > 0)
                {
                    if (s.stemless)
                    {
                        dx = -5;
                        w = 10;
                    }
                    else if (s.beam_st != null)
                    {
                        dx = 3;
                        w = s.beam_end != null ? 4 : 10;
                    }
                    else
                    {
                        dx = -8;
                        w = s.beam_end != null ? 11 : 16;
                    }
                    y_set(s.st, true, s.x + dx, w, s.ymx);
                    ymn = s.ymn;
                    if (s.notes[0].acc != null && ymn > 3 * (s.notes[0].pit - 18) - 9)
                        ymn = 3 * (s.notes[0].pit - 18) - 9;
                    y_set(s.st, false, s.x - s.wl, s.wl + s.wr, ymn);
                }
                else
                {
                    y_set(s.st, true, s.x - s.wl, s.wl + s.wr, s.ymx);
                    if (s.stemless)
                    {
                        dx = -5;
                        w = 10;
                    }
                    else if (s.beam_st != null)
                    {
                        dx = -6;
                        w = s.beam_end != null ? 4 : 10;
                    }
                    else
                    {
                        dx = -8;
                        w = s.beam_end != null ? 5 : 16;
                    }
                    dx += s.notes[0].shhd;
                    y_set(s.st, false, s.x + dx, w, s.ymn);
                }
                if (s.notes[s.nhd].acc != null)
                {
                    y = 3 * (s.notes[s.nhd].pit - 18) + (s.notes[s.nhd].acc == -1 ? 11 : 10);
                    y_set(s.st, true, s.x - 10, 10, y);
                }
                if (s.notes[0].acc != null)
                {
                    y = 3 * (s.notes[0].pit - 18) - (s.notes[0].acc == -1 ? 5 : 10);
                    y_set(s.st, false, s.x - 10, 10, y);
                }
            }

            draw_deco_note();

            for (v = 0; v < voice_tb.Length; v++)
            {
                p_voice = voice_tb[v];
                s = p_voice.sym;
                if (s == null)
                    continue;
                set_color(s.color);
                st = p_voice.st;
                for (; s != null; s = s.next)
                {
                    if (s.play)
                        continue;
                    if (s.tp != null)
                        draw_tuplet(s);
                    if (s.sls != null || s.sl1 != null)
                        draw_slurs(s);
                }
            }
            set_color();

            for (st = 0; st < staff_tb.Length; st++)
            {
                p_st = staff_tb[st];
                top = p_st.topbar + 2;
                bot = p_st.botbar - 2;
                for (i = 0; i < YSTEP; i++)
                {
                    if (top > p_st.top[i])
                        p_st.top[i] = top;
                    if (bot < p_st.bot[i])
                        p_st.bot[i] = bot;
                }
            }

            if (cfmt.measurenb >= 0)
                draw_measnb();

            set_dscale(-1);
            draw_all_lyrics();

            draw_deco_staff();

            draw_partempo();

            set_dscale(-1);
            output = output_sav;
        }

        /* -- draw the name/subname of the voices -- */
        void draw_vname(int indent, bool[] stl)
        {
            VoiceItem p_voice;
            int n;
            int st;
            int v;
            string a_p;
            int p;
            int y;
            int h;
            int h2;
            List<string> staff_d = new List<string>();

            for (st = stl.Length; --st >= 0;)
            {
                if (stl[st])
                    break;
            }
            if (st < 0)
                return;

            for (v = 0; v < voice_tb.Length; v++)
            {
                p_voice = voice_tb[v];
                if (!cur_sy.voices[v])
                    continue;
                st = cur_sy.voices[v].st;
                if (!stl[st])
                    continue;
                if (!gene.vnt)
                    continue;
                p = gene.vnt == 2 ? p_voice.nm : p_voice.snm;
                if (p == null)
                    continue;
                p_voice.new_name = null;
                if (!staff_d[st])
                    staff_d[st] = p;
                else
                    staff_d[st] += "\n" + p;
            }
            if (!staff_d.Count)
                return;
            set_font("voice");
            h = gene.curfont.size;
            h2 = h / 2;
            indent = -indent / 2;
            for (st = 0; st < staff_d.Count; st++)
            {
                if (!staff_d[st])
                    continue;
                a_p = staff_d[st].Split("\n");
                y = staff_tb[st].y + staff_tb[st].topbar * staff_tb[st].staffscale + h2 * (a_p.Length - 2);
                if ((cur_sy.staves[st].flags & OPEN_BRACE) && st + 1 < staff_tb.Length && (cur_sy.staves[st + 1].flags & CLOSE_BRACE) && !staff_d[st + 1])
                    y -= (staff_tb[st].y - staff_tb[st + 1].y) / 2;
                for (n = 0; n < a_p.Length; n++)
                {
                    p = a_p[n];
                    xy_str(indent, y, p, "c");
                    y -= h;
                }
            }
        }

        // -- set the y offset of the staves and return the height of the whole system --
        void set_staff()
        {
            int i;
            int st;
            int prev_staff;
            int v;
            int fmt;
            int y;
            int staffsep;
            int dy;
            int maxsep;
            int mbot;
            int val;
            VoiceItem p_voice;
            VoiceItem p_staff;
            var sy = cur_sy;

            for (st = 0; st <= nstaff; st++)
            {
                if (gene.st_print[st])
                    break;
            }
            y = 0;
            if (st > nstaff)
            {
                st--;
                p_staff = staff_tb[st];
            }
            else
            {
                p_staff = staff_tb[st];
                for (i = 0; i < YSTEP; i++)
                {
                    val = p_staff.top[i];
                    if (y < val)
                        y = val;
                }
            }

            y *= p_staff.staffscale;
            staffsep = tsfirst.fmt.staffsep / 2 + p_staff.topbar * p_staff.staffscale;
            if (y < staffsep)
                y = staffsep;
            if (y < p_staff.ann_top)
                y = p_staff.ann_top;
            p_staff.y = -y;

            for (prev_staff = 0; prev_staff < st; prev_staff++)
                staff_tb[prev_staff].y = -y;
            if (!gene.st_print[st])
                return y;

            var sy_staff_prev = sy.staves[prev_staff];
            for (st++; st <= nstaff; st++)
            {
                if (!gene.st_print[st])
                    continue;
                p_staff = staff_tb[st];
                staffsep = sy_staff_prev.sep ?? fmt.sysstaffsep;
                maxsep = sy_staff_prev.maxsep ?? fmt.maxsysstaffsep;

                dy = 0;
                if (p_staff.staffscale == staff_tb[prev_staff].staffscale)
                {
                    for (i = 0; i < YSTEP; i++)
                    {
                        val = p_staff.top[i] - staff_tb[prev_staff].bot[i];
                        if (dy < val)
                            dy = val;
                    }
                    dy *= p_staff.staffscale;
                }
                else
                {
                    for (i = 0; i < YSTEP; i++)
                    {
                        val = p_staff.top[i] * p_staff.staffscale - staff_tb[prev_staff].bot[i] * staff_tb[prev_staff].staffscale;
                        if (dy < val)
                            dy = val;
                    }
                }
                staffsep += p_staff.topbar * p_staff.staffscale;
                if (dy < staffsep)
                    dy = staffsep;
                maxsep += p_staff.topbar * p_staff.staffscale;
                if (dy > maxsep)
                    dy = maxsep;
                y += dy;
                p_staff.y = -y;

                while (!gene.st_print[++prev_staff])
                    staff_tb[prev_staff].y = -y;
                while (true)
                {
                    sy_staff_prev = sy.staves[prev_staff];
                    if (sy_staff_prev != null)
                        break;
                    sy = sy.next;
                }
            }
            mbot = 0;
            for (i = 0; i < YSTEP; i++)
            {
                val = staff_tb[prev_staff].bot[i];
                if (mbot > val)
                    mbot = val;
            }
            if (mbot > p_staff.ann_bot)
                mbot = p_staff.ann_bot;
            mbot *= staff_tb[prev_staff].staffscale;

            for (st = 0; st <= nstaff; st++)
            {
                p_staff = staff_tb[st];
                dy = p_staff.y;
                if (p_staff.staffscale != 1)
                {
                    p_staff.scale_str = $"transform=\"translate(0,{(posy - dy).ToString("F1")}) scale({p_staff.staffscale.ToString("F2")})\"";
                }
            }

            if (mbot == 0)
            {
                for (st = nstaff; st >= 0; st--)
                {
                    if (gene.st_print[st])
                        break;
                }
                if (st < 0)
                    return y;
            }
            dy = -mbot;
            staffsep = fmt.staffsep / 2;
            if (dy < staffsep)
                dy = staffsep;
            maxsep = fmt.maxstaffsep / 2;
            if (dy > maxsep)
                dy = maxsep;

            return y + dy;
        }

        void draw_systems(int indent)
        {
            VoiceItem s, s2;
            int st, x, x2, res, sy;
            List<int> xstaff = new List<int>();
            List<int> stl = new List<int>(); // all staves in the line
            List<int> bar_bot = new List<int>();
            List<int> bar_height = new List<int>();
            List<object> ba = new List<object>(); // bars [symbol, bottom, height]
            string sb = "";
            string thb = "";

            /* -- set the bottom and height of the measure bars -- */
            void bar_set()
            {
                int st, staffscale, top, bot, dy = 0;

                for (st = 0; st <= cur_sy.nstaff; st++)
                {
                    if (xstaff[st] < 0)
                    {
                        bar_bot[st] = bar_height[st] = 0;
                        continue;
                    }
                    staffscale = staff_tb[st].staffscale;
                    top = staff_tb[st].topbar * staffscale;
                    bot = staff_tb[st].botbar * staffscale;
                    if (dy == 0)
                        dy = staff_tb[st].y + top;
                    bar_bot[st] = staff_tb[st].y + bot;
                    bar_height[st] = dy - bar_bot[st];
                    dy = (cur_sy.staves[st].flags & STOP_BAR) != 0 ? 0 : bar_bot[st];
                }
            } // bar_set()

            /* -- draw a staff -- */
            void draw_staff(int st, int x1, int x2)
            {
                int i;
                double w, dy, y=0;
                string ty,ln = "";
                string stafflines = staff_tb[st].stafflines;
                int len = stafflines.Length;
                int  iline = 6 * (int)staff_tb[st].staffscale; // interline

                if (!stafflines.Contains("[") && !stafflines.Contains("|"))
                    return; // no line
                w = x2 - x1;
                set_sscale(-1);

                // check if default staff
                if (cache != null && cache.st_l == stafflines && staff_tb[st].staffscale == 1 && cache.st_w == (int)w)
                {
                    xygl(x1, staff_tb[st].y, "stdef" + cfmt.fullsvg);
                    return;
                }
                for (i = 0; i < len; i++, y -= iline)
                {
                    if (stafflines[i] == '.')
                        continue;
                    dy = 0;
                    for (; i < len; i++, y -= iline, dy -= iline)
                    {
                        switch (stafflines[i])
                        {
                            case '.':
                            case '-':
                                continue;
                            case ty:
                                ln += "m-" + w.ToString("0.0") + " " + dy + "h" + w.ToString("0.0");
                                dy = 0;
                                continue;
                        }
                        if (ty != null)
                            ln += "\"/>\n";
                        ty = stafflines[i];
                        ln += "<path class=\"" + (ty == "[" ? "slthW" : "slW") + "\" d=\"m0 " + y + "h" + w.ToString("0.0");
                        dy = 0;
                    }
                    ln += "\"/>";
                }
                y = staff_tb[st].y;
                if (cache == null && w > get_lwidth() - 10 && staff_tb[st].staffscale == 1)
                {
                    cache = new { st_l = stafflines, st_w = (int)w };
                    string strdef = "stdef" + cfmt.fullsvg;
                    if (ln.IndexOf("<path", 1) < 0)
                        glyphs[strdef] = ln.Replace("path", "path id=\"" + strdef + "\"");
                    else
                        glyphs[strdef] = "<g id=\"" + strdef + "\">\n" + ln + "\n</g>";
                    xygl(x1, y, strdef);
                    return;
                }
                out_XYAB("<g transform=\"translate(X, Y)\">\n" + ln + "\n</g>\n", x1, y);
            } // draw_staff()

            // draw a measure bar
            void draw_bar(VoiceItem s, int bot, int h)
            {
                int i, s2, yb, w;
                string bar_type = s.bar_type;
                int st = s.st;
                PageStaff p_staff = staff_tb[st];
                int x = s.x;

                // don't put a line between the staves if there is no bar above
                if (st != 0 && s.ts_prev != null && s.ts_prev.type != C.BAR && s.ts_prev.st != st - 1)
                    h = p_staff.topbar * p_staff.staffscale;

                s.ymx = s.ymn + h;

                set_sscale(-1);
                anno_start(s);
                if (s.color != null)
                    set_color(s.color);

                // compute the middle vertical offset of the staff
                yb = p_staff.y + 12;
                if (p_staff.stafflines != "|||||")
                    yb += (p_staff.topbar + p_staff.botbar) / 2 - 12; // bottom

                // if measure repeat, draw the "%" like glyphs
                if (s.bar_mrep != null)
                {
                    set_sscale(st);
                    if (s.bar_mrep == 1)
                    {
                        for (s2 = s.prev; s2.type != C.REST; s2 = s2.prev)
                            ;
                        xygl(s2.x, yb, "mrep");
                    }
                    else
                    {
                        xygl(x, yb, "mrep2");
                        if (s.v == cur_sy.top_voice)
                            nrep_out(x, yb + p_staff.topbar, s.bar_mrep);
                    }
                    set_sscale(-1);
                }

                if (bar_type == "||:")
                    bar_type = "[|:";

                for (i = bar_type.Length - 1; i >= 0; i--)
                {
                    switch (bar_type[i])
                    {
                        case '|':
                            if (s.bar_dotted != null)
                            {
                                w = (5 * p_staff.staffscale).ToString("0.0");
                                out_XYAB("<path class=\"bW\" stroke-dasharray=\"A,A\" d=\"MX Yv-G\"/>\n", x, bot, w, h);
                            }
                            else if (s.color != null)
                            {
                                out_XYAB("<path class=\"bW\" d=\"MX Yv-F\"/>\n", x, bot, h);
                            }
                            else
                            {
                                sb += "M" + sx(x).ToString("0.0") + " " + self.sy(bot).ToString("0.0") + "v-" + h.ToString("0.0");
                            }
                            break;
                        default:
                            //			case "[":
                            //			case "]":
                            x -= 3;
                            if (s.color != null)
                                out_XYAB("<path class=\"bthW\" d=\"MX Yv-F\"/>\n", x + 1.5, bot, h);
                            else
                                thb += "M" + sx(x + 1.5).ToString("0.0") + " " + self.sy(bot).ToString("0.0") + "v-" + h.ToString("0.0");
                            break;
                        case ':':
                            x -= 2;
                            set_sscale(st);
                            xygl(x + 1, yb - 12, "rdots");
                            set_sscale(-1);
                            break;
                    }
                    x -= 3;
                }
                set_color();
                anno_stop(s);
            } // draw_bar()

            // output all the bars
            void out_bars()
            {
                int i, b, bx;
                int l = ba.Count;

                set_font("annotation");
                bx = gene.curfont.box;
                if (bx != 0)
                    gene.curfont.box = 0;
                for (i = 0; i < l; i++)
                {
                    b = (int)ba[i]; // symbol, bottom, height
                    draw_bar((VoiceItem)b[0], (int)b[1], (int)b[2]);
                }
                if (bx != 0)
                    gene.curfont.box = bx;

                set_sscale(-1);
                if (sb != "") // single bars
                    output += "<path class=\"bW\" d=\"" + sb + "\"/>\n";

                if (thb != "") // thick bars [x, y, h]
                    output += "<path class=\"bthW\" d=\"" + thb + "\"/>\n";
            } // out_bars()

            // set the helper lines of rests
            void hl_rest(VoiceItem s)
            {
                int j;
                PageStaff p_st = staff_tb[s.st];
                int i = 5 - s.nflags; // rest_tb index (5 = C_XFLAGS)
                int x = s.x;
                int y = s.y;

                if (i < 6) // no ledger line if rest smaller than minim
                    return;

                if (i == 7 && y == 12 && p_st.stafflines.Length <= 2)
                {
                    y -= 6; // semibreve a bit lower
                }

                j = y / 6;
                switch (i)
                {
                    default:
                        switch (p_st.stafflines[j + 1])
                        {
                            case '|':
                            case '[':
                                break;
                            default:
                                set_hl(p_st, j + 1, x, -7, 7);
                                break;
                        }
                        if (i == 9) // longa
                        {
                            y -= 6;
                            j--;
                        }
                        break;
                    case 7: // semibreve
                        y += 6;
                        j++;
                        break;
                    case 6: // minim
                        break;
                }
                switch (p_st.stafflines[j])
                {
                    case '|':
                    case '[':
                        break;
                    default:
                        set_hl(p_st, j, x, -7, 7);
                        break;
                }
            } // hl_rest()

            // return the left x offset of a new staff
            // s is the %%staves
            int st1(int st, VoiceItem s)
            {
                int tim = (int)s.time;

                do // search a voice of this staff
                {
                    s = s.ts_next;
                } while (s.st != st);
                while (s.prev != null && s.prev.time >= tim) // search the first symbol of this voice
                    s = s.prev;
                if (s.bar_type != null)
                    return (int)s.x;
                return (int)(s.x - s.wl);
            } // st1()

            // ---- draw_systems() ----

            /* draw the staff, skipping the staff breaks */
            for (st = 0; st <= nstaff; st++)
            {
                stl[st] = cur_sy.st_print[st] ? 1 : 0; // staff at start of line
                xstaff[st] = stl[st] != 0 ? 0 : -1;
            }
            bar_set();
            draw_lstaff(0);
            for (s = tsfirst; s != null; s = s.ts_next)
            {
                switch (s.type)
                {
                    case C.STAVES:
                        sy = s.sy;
                        for (st = 0; st <= nstaff; st++)
                        {
                            x = xstaff[st];
                            if (x < 0) // no staff yet
                            {
                                if (sy.st_print[st])
                                {
                                    xstaff[st] = st1(st, s);
                                    stl[st] = 1;
                                }
                                continue;
                            }
                            if (sy.st_print[st] // if not staff stop
                                && cur_sy.staves[st] != null
                                && sy.staves[st].stafflines == cur_sy.staves[st].stafflines)
                                continue;
                            if (s.ts_prev.bar_type != null)
                            {
                                x2 = (int)s.ts_prev.x;
                            }
                            else
                            {
                                x2 = (int)((s.ts_prev.x + s.x) / 2);
                                xstaff[st] = -1;
                            }
                            draw_staff(st, x, x2);
                            xstaff[st] = sy.st_print[st] ? x2 : -1;
                        }
                        cur_sy = sy;
                        bar_set();
                        continue;
                    case C.BAR: // display the bars after the staves
                        if (s.invis || s.bar_type == null || !cur_sy.st_print[s.st])
                            break;
                        if (s.second && (s.ts_prev == null || (s.ts_prev.type == C.BAR && s.ts_prev.st == s.st)))
                            break;
                        ba.Add(new object[] { s, bar_bot[s.st], bar_height[s.st] });
                        break;
                    case C.STBRK:
                        if (cur_sy.voices[s.v] != null && cur_sy.voices[s.v].range == 0)
                        {
                            if (s.xmx > 14)
                            {
                                /* draw the left system if stbrk in all voices */
                                int nv = 0;
                                for (int i = 0; i < voice_tb.Length; i++)
                                {
                                    if (cur_sy.voices[i] != null && cur_sy.voices[i].range > 0)
                                        nv++;
                                }
                                for (s2 = s.ts_next; s2 != null; s2 = s2.ts_next)
                                {
                                    if (s2.type != C.STBRK)
                                        break;
                                    nv--;
                                }
                                if (nv == 0)
                                    draw_lstaff(s.x);
                            }
                        }
                        st = s.st;
                        x = xstaff[st];
                        if (x >= 0)
                        {
                            s2 = s.prev;
                            if (s2 == null)
                                break;
                            x2 = s2.type == C.BAR ? (int)s2.x : (int)(s.x - s.xmx);
                            if (x >= x2)
                                break;
                            draw_staff(st, x, x2);
                            xstaff[st] = (int)s.x;
                        }
                        break;
                    case C.GRACE:
                        for (s2 = s.extra; s2 != null; s2 = s2.next)
                            self.draw_hl(s2);
                        break;
                    case C.NOTE:
                        if (!s.invis)
                            self.draw_hl(s);
                        break;
                    case C.REST:
                        if (s.fmr != null || (s.rep_nb != null && s.rep_nb >= 0))
                            center_rest(s);
                        if (!s.invis)
                            hl_rest(s);
                        break;
                        //		default:
                        //fixme:does not work for "%%staves K: M: $" */
                        //removed for K:/M: in empty staves
                        //			if (!cur_sy.st_print[st])
                        //				s.invis = true
                        //			break
                }
            }

            // draw the end of the staves
            for (st = 0; st <= nstaff; st++)
            {
                x = xstaff[st];
                if (x < 0 || x >= realwidth)
                    continue;
                draw_staff(st, x, realwidth);
            }

            // the ledger lines
            draw_all_hl();

            // and the bars
            out_bars();

            draw_vname(indent, stl);

            //	set_sscale(-1)
        }

        /* -- draw remaining symbols when the staves are defined -- */
        // (possible hook)
        void  draw_symbols(object p_voice)
        {
            BeamItem bm = new BeamItem();
            VoiceItem s;
            double x, y, st;

            //	bm.s2 = undefined
            for (s = p_voice.sym; s != null; s = s.next)
            {
                if (s.invis != null)
                {
                    switch (s.type)
                    {
                        case C.CLEF:
                            if (s.time >= staff_tb[s.st].clef.time)
                                staff_tb[s.st].clef = s;
                            continue;
                        case C.KEY:
                            p_voice.ckey = s;
                        default:
                            continue;
                        case C.NOTE: // (beams may start on invisible notes)
                            break;
                    }
                }
                st = s.st;
                x = (int)s.x;
                set_color(s.color);
                switch (s.type)
                {
                    case C.NOTE:
                        //--fixme: recall set_scale if different staff
                        set_scale(s);
                        if (s.beam_st != null && !s.beam_end)
                        {
                            if (self.calculate_beam(bm, s))
                                draw_beams(bm);
                        }
                        if (!s.invis)
                        {
                            anno_start(s);
                            draw_note(s, !bm.s2);
                            anno_a.Add(s);
                        }
                        if (s == bm.s2)
                            bm.s2 = null;
                        break;
                    case C.REST:
                        if (!gene.st_print[st])
                            break;
                        draw_rest(s);
                        break;
                    case C.BAR:
                        break; // drawn in draw_systems
                    case C.CLEF:
                        if (s.time >= staff_tb[st].clef.time)
                            staff_tb[st].clef = s;
                        if (s.second != null || staff_tb[st].topbar == 0 || !gene.st_print[st])
                            break;
                        set_color();
                        set_sscale(st);
                        anno_start(s);
                        y = staff_tb[st].y;
                        if (s.clef_name != null)
                            xygl(x, y + (int)s.y, s.clef_name);
                        else if (!s.clef_small)
                            xygl(x, y + (int)s.y, s.clef_type + "clef");
                        else
                            xygl(x, y + (int)s.y, "s" + s.clef_type + "clef");
                        if (s.clef_octave != null)
                        {
                            /*fixme:break the compatibility and avoid strange numbers*/
                            if (s.clef_octave > 0)
                            {
                                y += s.ymx - 10;
                                if (s.clef_small != null)
                                    y -= 1;
                            }
                            else
                            {
                                y += s.ymn + 6;
                                if (s.clef_small != null)
                                    y += 1;
                            }
                            xygl(x - 2, y, (s.clef_octave == 7 || s.clef_octave == -7) ? "oct" : "oct2");
                        }
                        anno_a.Add(s);
                        break;
                    case C.METER:
                        p_voice.meter = s;
                        if (s.second != null || staff_tb[s.st].topbar == 0)
                            break;
                        set_color();
                        set_sscale(s.st);
                        anno_start(s);
                        draw_meter(s);
                        anno_a.Add(s);
                        break;
                    case C.KEY:
                        p_voice.ckey = s;
                        if (s.second != null || staff_tb[s.st].topbar == 0)
                            break;
                        set_color();
                        set_sscale(s.st);
                        anno_start(s);
                        self.draw_keysig(x, s);
                        anno_a.Add(s);
                        break;
                    case C.MREST:
                        draw_mrest(s);
                        break;
                    case C.GRACE:
                        set_scale(s);
                        draw_gracenotes(s);
                        break;
                    case C.SPACE:
                    case C.STBRK:
                        break; // nothing
                    case C.CUSTOS:
                        set_scale(s);
                        draw_note(s, 0);
                        break;
                    case C.BLOCK: // no width
                    case C.REMARK:
                    case C.STAVES:
                    case C.TEMPO:
                        break;
                    default:
                        error(2, s, "draw_symbols - Cannot draw symbol " + s.type);
                        break;
                }
            }
            set_scale(p_voice.sym);
        }

        /* -- draw all symbols -- */
        void draw_all_sym()
        {
            VoiceItem p_voice;
            int n = voice_tb.Length;

            void draw_sl2()
            {
                int i;
                float[] a;
                float d, dy, dy2, dy2o, dz;
                VoiceItem sl;

                while (true)
                {
                    sl = gene.a_sl.Shift();
                    if (sl == null)
                        break;

                    i = sl[3].IndexOf('d="M') + 4;
                    output += sl[3].Substring(0, i);

                    a = sl[3].Substring(i).Match(/[\d.-] +/ g).Select(float.Parse).ToArray();

                    a[1] -= staff_tb[sl[0].st].y;

                    dy2o = sl[0].fmt.sysstaffsep + 24;
                    dy2 = staff_tb[sl[1].st].y - staff_tb[sl[0].st].y;

                    switch (sl[2])
                    {
                        case "//":
                        case "\\\\":
                            d = -(sl[1].prev.prev.y + staff_tb[sl[0].st].y + sl[1].prev.next.y + staff_tb[sl[1].st].y) - 2 * (a[1] - posy);
                            a[5] = d - a[5];
                            a[7] = d - a[7];
                            if (a.Length > 8)
                            {
                                d = sl[2][0] == '/' ? 3 : -3;
                                a[8] = -a[8];
                                a[10] = -a[3] + d;
                                a[12] = -a[5] + d;
                                a[14] = -a[7];
                            }
                            break;
                        case "/\\":
                        case "\\/":
                            d = sl[2][0] == '/' ? dy2 - dy2o - 10 : dy2 + dy2o + 10;
                            a[3] += d;
                            a[5] += d;
                            if (a.Length > 8)
                            {
                                a[10] += d;
                                a[12] += d;
                            }
                            break;
                        default:
                            d = sl[2][0] == '/' ? dy2 - dy2o : -dy2 - dy2o;
                            a[5] += d;
                            a[7] += d;
                            if (a.Length > 8)
                            {
                                a[12] -= d;
                                a[14] -= d;
                            }
                            break;
                    }

                    output += $"{a[0]:F1} {a[1]:F1}c{a[2]:F1} {a[3]:F1} {a[4]:F1} {a[5]:F1} {a[6]:F1} {a[7]:F1}";
                    if (a.Length > 8)
                        output += $"v{a[8]:F1}c{a[9]:F1} {a[10]:F1} {a[11]:F1} {a[12]:F1} {a[13]:F1} {a[14]:F1}";
                    output += "\"/>\n";
                }
            }

            for (int v = 0; v < n; v++)
            {
                p_voice = voice_tb[v];
                if (p_voice.sym != null && p_voice.sym.x != null)
                {
                    self.draw_symbols(p_voice);
                    draw_all_ties(p_voice);
                    set_color();
                }
            }

            self.draw_all_deco();
            glout();
            anno_put();
            set_sscale(-1);

            if (gene.a_sl != null)
                draw_sl2();
        }

        /* -- set the tie directions for one voice -- */
        void set_tie_dir(VoiceItem s)
        {
            VoiceItem s2;
            int i, ntie, dir, sec, pit;
            int ty;

            for (; s != null; s = s.next)
            {
                if (!s.ti1)
                    continue;

                sec = ntie = 0;
                pit = 128;
                for (i = 0; i <= s.nhd; i++)
                {
                    if (s.notes[i].tie_ty != null)
                    {
                        ntie++;
                        if (pit < 128 && s.notes[i].pit <= pit + 1)
                            sec++;
                        pit = s.notes[i].pit;
                        s2 = s.notes[i].tie_e;
                    }
                }

                if (s2 != null && s.stem * s2.stem < 0)
                    dir = pit >= 22 ? C.SL_ABOVE : C.SL_BELOW;
                else if (s.multi != null)
                    dir = s.multi > 0 ? C.SL_ABOVE : C.SL_BELOW;
                else
                    dir = s.stem < 0 ? C.SL_ABOVE : C.SL_BELOW;

                if (s.multi != null)
                {
                    for (i = 0; i <= s.nhd; i++)
                    {
                        ty = s.notes[i].tie_ty;
                        if (!((ty & 0x07) == C.SL_AUTO))
                            continue;
                        s.notes[i].tie_ty = (ty & C.SL_DOTTED) | dir;
                    }
                    continue;
                }

                if (ntie <= 1)
                {
                    for (i = 0; i <= s.nhd; i++)
                    {
                        ty = s.notes[i].tie_ty;
                        if (ty != null)
                        {
                            if ((ty & 0x07) == C.SL_AUTO)
                                s.notes[i].tie_ty = (ty & C.SL_DOTTED) | dir;
                            break;
                        }
                    }
                    continue;
                }

                if (sec == 0)
                {
                    if (ntie % 2 != 0)
                    {
                        ntie = (ntie - 1) / 2;
                        dir = C.SL_BELOW;
                        for (i = 0; i <= s.nhd; i++)
                        {
                            ty = s.notes[i].tie_ty;
                            if (ty == null)
                                continue;
                            if (ntie == 0)
                            {
                                if (s.notes[i].pit >= 22)
                                    dir = C.SL_ABOVE;
                            }
                            if ((ty & 0x07) == C.SL_AUTO)
                                s.notes[i].tie_ty = (ty & C.SL_DOTTED) | dir;
                            if (ntie-- == 0)
                                dir = C.SL_ABOVE;
                        }
                        continue;
                    }

                    ntie /= 2;
                    dir = C.SL_BELOW;
                    for (i = 0; i <= s.nhd; i++)
                    {
                        ty = s.notes[i].tie_ty;
                        if (ty == null)
                            continue;
                        if ((ty & 0x07) == C.SL_AUTO)
                            s.notes[i].tie_ty = (ty & C.SL_DOTTED) | dir;
                        if (--ntie == 0)
                            dir = C.SL_ABOVE;
                    }
                    continue;
                }

                pit = 128;
                for (i = 0; i <= s.nhd; i++)
                {
                    if (s.notes[i].tie_ty != null)
                    {
                        if (pit < 128 && s.notes[i].pit <= pit + 1)
                        {
                            ntie = i;
                            break;
                        }
                        pit = s.notes[i].pit;
                    }
                }

                dir = C.SL_BELOW;
                for (i = 0; i <= s.nhd; i++)
                {
                    ty = s.notes[i].tie_ty;
                    if (ty == null)
                        continue;
                    if (ntie == i)
                        dir = C.SL_ABOVE;
                    if ((ty & 0x07) == C.SL_AUTO)
                        s.notes[i].tie_ty = (ty & C.SL_DOTTED) | dir;
                }
            }
        }

        /* -- have room for the ties out of the staves -- */
        void set_tie_room()
        {
            VoiceItem s;
            VoiceItem s2;
            VoiceItem p_voice;
            int v;
            int dx;
            int y;
            int dy;

            for (v = 0; v < voice_tb.Length; v++)
            {
                p_voice = voice_tb[v];
                s = p_voice.sym;
                if (s == null)
                    continue;
                s = s.next;
                if (s == null)
                    continue;
                set_tie_dir(s);
                for (; s != null; s = s.next)
                {
                    if (!s.ti1)
                        continue;
                    if (s.notes[0].pit < 20 && s.notes[0].tie_ty != null && (s.notes[0].tie_ty & 0x07) == C.SL_BELOW)
                        ;
                    else if (s.notes[s.nhd].pit > 24 && s.notes[s.nhd].tie_ty != null && (s.notes[s.nhd].tie_ty & 0x07) == C.SL_ABOVE)
                        ;
                    else
                        continue;
                    s2 = s.next;
                    while (s2 != null && s2.type != C.NOTE)
                        s2 = s2.next;
                    if (s2 != null)
                    {
                        if (s2.st != s.st)
                            continue;
                        dx = s2.x - s.x - 10;
                    }
                    else
                    {
                        dx = realwidth - s.x - 10;
                    }
                    if (dx < 100)
                        dy = 9;
                    else if (dx < 300)
                        dy = 12;
                    else
                        dy = 16;
                    if (s.notes[s.nhd].pit > 24)
                    {
                        y = 3 * (s.notes[s.nhd].pit - 18) + dy;
                        if (s.ymx < y)
                            s.ymx = y;
                        if (s2 != null && s2.ymx < y)
                            s2.ymx = y;
                        y_set(s.st, true, s.x + 5, dx, y);
                    }
                    if (s.notes[0].pit < 20)
                    {
                        y = 3 * (s.notes[0].pit - 18) - dy;
                        if (s.ymn > y)
                            s.ymn = y;
                        if (s2 != null && s2.ymn > y)
                            s2.ymn = y;
                        y_set(s.st, false, s.x + 5, dx, y);
                    }
                }
            }
        }


        public interface noteItem
        {
            int? pit { get; set; }
            int? shhd { get; set; }
            int? shac { get; set; }
            int? dur { get; set; }
            int? midi { get; set; }
            int? jn { get; set; }
            int? jo { get; set; }
        }

        //public interface VoiceItem
        //{
        //    int? type { get; set; }
        //    int? v { get; set; }
        //    int? dur { get; set; }
        //    int? time { get; set; }
        //    FormationInfo fmt { get; set; }
        //    PageVoiceTune p_v { get; set; }
        //    noteItem[] notes { get; set; }
        //    int? x { get; set; }
        //    int? y { get; set; }
        //    int? wr { get; set; }
        //    int? wl { get; set; }
        //    int? st { get; set; }
        //    VoiceItem next { get; set; }
        //    VoiceItem prev { get; set; }
        //    VoiceItem ts_prev { get; set; }
        //    VoiceItem ts_next { get; set; }
        //    bool? err { get; set; }
        //    int? nhd { get; set; }
        //}

        public class FormationInfo
        {
            public bool? cancelkey { get; set; }
            public int? measrepnb { get; set; }
        }

        public class PageVoiceTune
        {
            public string stafflines { get; set; }
            public int? topbar { get; set; }
            public int? botbar { get; set; }
            public int? scale { get; set; }
        }

        public class Program
        {
            public static void Main(string[] args)
            {
                int STEM_MIN = 16;
                int STEM_MIN2 = 16;
                int STEM_MIN3 = 16;
                int STEM_MIN4 = 16;
                int STEM_CH_MIN = 16;
                int STEM_CH_MIN2 = 16;
                int STEM_CH_MIN3 = 16;
                int STEM_CH_MIN4 = 16;
                int BEAM_DEPTH = 3;
                double BEAM_OFFSET = 0.25;
                int BEAM_SHIFT = 16;
                int BEAM_STUB = 16;
                double SLUR_SLOPE = 0.5;
                int GSTEM = 16;
                double GSTEM_XOFF = 2.3;
                var cache = default(object);
                var anno_a = new System.Collections.Generic.List<object>();
                var min_tb = new int[][] {
            new int[] { STEM_MIN, STEM_MIN, STEM_MIN2, STEM_MIN3, STEM_MIN4, STEM_MIN4 },
            new int[] { STEM_CH_MIN, STEM_CH_MIN, STEM_CH_MIN2, STEM_CH_MIN3, STEM_CH_MIN4, STEM_CH_MIN4 }
        };
                var sharp_cl = new sbyte[] { 2422, 9, 15 };
                var flat_cl = new sbyte[] { 1233, 18, 24 };
                var sharp1 = new sbyte[] { -922, 12, -9 };
                var sharp2 = new sbyte[] { 1231, -9, 12 };
                var flat1 = new sbyte[] { 923, -12, 9 };
                var flat2 = new sbyte[] { -1223, 9, -12 };


            }
        }










    }


//public interface noteItem
//    {
//        int? pit { get; set; }
//        int? shhd { get; set; }
//        int? shac { get; set; }
//        int? dur { get; set; }
//        int? midi { get; set; }
//        int? jn { get; set; }
//        int? jo { get; set; }
//    }

//    public interface VoiceItem
//    {
//        int? type { get; set; }
//        int? v { get; set; }
//        int? dur { get; set; }
//        int? time { get; set; }
//        FormationInfo fmt { get; set; }
//        PageVoiceTune p_v { get; set; }
//        noteItem[] notes { get; set; }
//        int? x { get; set; }
//        int? y { get; set; }
//        int? wr { get; set; }
//        int? wl { get; set; }
//        int? st { get; set; }
//        VoiceItem next { get; set; }
//        VoiceItem prev { get; set; }
//        VoiceItem ts_prev { get; set; }
//        VoiceItem ts_next { get; set; }
//        bool? err { get; set; }
//        int? nhd { get; set; }
//    }

//    public class FormationInfo
//    {
//        public int? beamslope { get; set; }
//        public bool? straightflags { get; set; }
//    }

//    public class PageVoiceTune
//    {
//        public int? scale { get; set; }
//    }

//    public class Program
//    {
//        public static double STEM_MIN = 16.3;
//        public static double STEM_MIN2 = 16.3;
//        public static double STEM_MIN3 = 16.3;
//        public static double STEM_MIN4 = 16.3;
//        public static double STEM_CH_MIN = 16.3;
//        public static double STEM_CH_MIN2 = 16.3;
//        public static double STEM_CH_MIN3 = 16.3;
//        public static double STEM_CH_MIN4 = 16.3;
//        public static double BEAM_DEPTH = 3.2;
//        public static double BEAM_OFFSET = .25;
//        public static double BEAM_SHIFT = 16.3;
//        public static double BEAM_STUB = 16.3;
//        public static double SLUR_SLOPE = .5;
//        public static double GSTEM = 16.3;
//        public static double GSTEM_XOFF = 2.3;
//        public static object cache;
//        public static object[] anno_a = new object[] { };
//        public static object[][] min_tb = new object[][] {
//        new object[] { STEM_MIN, STEM_MIN, STEM_MIN2, STEM_MIN3, STEM_MIN4, STEM_MIN4 },
//        new object[] { STEM_CH_MIN, STEM_CH_MIN, STEM_CH_MIN2, STEM_CH_MIN3, STEM_CH_MIN4, STEM_CH_MIN4 }
//    };
//        public static sbyte[] sharp_cl = new sbyte[] { 2422, 9, 15 };
//        public static sbyte[] flat_cl = new sbyte[] { 1233, 18, 24 };
//        public static sbyte[] sharp1 = new sbyte[] { -922, 12, -9 };
//        public static sbyte[] sharp2 = new sbyte[] { 1231, -9, 12 };
//        public static sbyte[] flat1 = new sbyte[] { 923, -12, 9 };
//        public static sbyte[] flat2 = new sbyte[] { -1223, 9, -12 };

//        public static void draw_gracenotes(VoiceItem s)
//        {
//            int? x1, y1;
//            VoiceItem last, note;
//            object bm = new { s2 = (object)null };
//            object g = s.extra;

//            while (true)
//            {
//                if (g.beam_st != null && !g.beam_end)
//                {
//                    if (calculate_beam(bm, g))
//                        draw_beams(bm);
//                }
//                anno_start(g);
//                draw_note(g, bm.s2 == null);
//                if (g == bm.s2)
//                    bm.s2 = null;
//                anno_a.Add(s);
//                if (g.next == null)
//                    break;
//                g = g.next;
//            }
//            last = g;

//            if (s.sappo != null)
//            {
//                g = s.extra;
//                if (g.next == null)
//                {
//                    x1 = 9;
//                    y1 = g.stem > 0 ? 5 : -5;
//                }
//                else
//                {
//                    x1 = (g.next.x - g.x) * .5 + 4;
//                    y1 = (g.ys + g.next.ys) * .5 - g.y;
//                    if (g.stem > 0)
//                        y1 -= 1;
//                    else
//                        y1 += 1;
//                }
//                note = g.notes[g.stem < 0 ? 0 : g.nhd];
//                out_acciac(x_head(g, note), y_head(g, note), x1, y1, g.stem > 0);
//            }

//            g = s.slur;
//            if (g != null)
//            {
//                anno_start(s, "slur");
//                xypath(g.x0, g.y0 + staff_tb[s.st].y);
//                output += 'c' + g.x1.toFixed(1) + ' ' + g.y1.toFixed(1) + ' ' + g.x2.toFixed(1) + ' ' + g.y2.toFixed(1) + ' ' + g.x3.toFixed(1) + ' ' + g.y3.toFixed(1) + '"/>\n';
//                anno_stop(s, "slur");
//            }
//        }



//        public interface noteItem
//        {
//            int? pit { get; set; }
//            int? shhd { get; set; }
//            int? shac { get; set; }
//            int? dur { get; set; }
//            int? midi { get; set; }
//            int? jn { get; set; }
//            int? jo { get; set; }
//        }

//        public interface VoiceItem
//        {
//            int? type { get; set; }
//            int? v { get; set; }
//            int? dur { get; set; }
//            int? time { get; set; }
//            FormationInfo fmt { get; set; }
//            PageVoiceTune p_v { get; set; }
//            noteItem[] notes { get; set; }
//            int? x { get; set; }
//            int? y { get; set; }
//            int? wr { get; set; }
//            int? wl { get; set; }
//            int? st { get; set; }
//            VoiceItem next { get; set; }
//            VoiceItem prev { get; set; }
//            VoiceItem ts_prev { get; set; }
//            VoiceItem ts_next { get; set; }
//            bool? err { get; set; }
//            int? nhd { get; set; }
//        }

//        var STEM_MIN = 16.3;
//        var STEM_MIN2 = 16.3;
//        var STEM_MIN3 = 16.3;
//        var STEM_MIN4 = 16.3;
//        var STEM_CH_MIN = 16.3;
//        var cache;
//        var anno_a = new List<object>();
//        var min_tb = new List<List<double>>()
//{
//    new List<double> { STEM_MIN, STEM_MIN, STEM_MIN2, STEM_MIN3, STEM_MIN4, STEM_MIN4 },
//    new List<double> { STEM_CH_MIN, STEM_CH_MIN, STEM_CH_MIN2, STEM_CH_MIN3, STEM_CH_MIN4, STEM_CH_MIN4 }
//};
//        var sharp_cl = new sbyte[] { 2422, 9, 15 };
//        var flat_cl = new sbyte[] { 1233, 18, 24 };
//        var sharp1 = new sbyte[] { -922, 12, -9 };
//        var sharp2 = new sbyte[] { 1231, -9, 12 };
//        var flat1 = new sbyte[] { 923, -12, 9 };
//        var flat2 = new sbyte[] { -1223, 9, -12 };



//        public interface noteItem
//        {
//            int? pit { get; set; }
//            int? shhd { get; set; }
//            int? shac { get; set; }
//            int? dur { get; set; }
//            int? midi { get; set; }
//            int? jn { get; set; }
//            int? jo { get; set; }
//        }

//        public interface VoiceItem
//        {
//            int? type { get; set; }
//            int? v { get; set; }
//            int? dur { get; set; }
//            int? time { get; set; }
//            FormationInfo fmt { get; set; }
//            PageVoiceTune p_v { get; set; }
//            noteItem[] notes { get; set; }
//            int? x { get; set; }
//            int? y { get; set; }
//            int? wr { get; set; }
//            int? wl { get; set; }
//            int? st { get; set; }
//            VoiceItem next { get; set; }
//            VoiceItem prev { get; set; }
//            VoiceItem ts_prev { get; set; }
//            VoiceItem ts_next { get; set; }
//            bool? err { get; set; }
//            int? nhd { get; set; }
//        }

//        var STEM_MIN = 16.3;
//        var STEM_MIN2 = 16.3;
//        var STEM_MIN3 = 16.3;
//        var STEM_MIN4 = 16.3;
//        var STEM_CH_MIN = 16.3;
//        var cache;
//        var anno_a = new List<object>();
//        var min_tb = new List<List<double>>()
//{
//    new List<double> { STEM_MIN, STEM_MIN, STEM_MIN2, STEM_MIN3, STEM_MIN4, STEM_MIN4 },
//    new List<double> { STEM_CH_MIN, STEM_CH_MIN, STEM_CH_MIN2, STEM_CH_MIN3, STEM_CH_MIN4, STEM_CH_MIN4 }
//};
//        var sharp_cl = new sbyte[] { 2422, 9, 15 };
//        var flat_cl = new sbyte[] { 1233, 18, 24 };
//        var sharp1 = new sbyte[] { -922, 12, -9 };
//        var sharp2 = new sbyte[] { 1231, -9, 12 };
//        var flat1 = new sbyte[] { 923, -12, 9 };
//        var flat2 = new sbyte[] { -1223, 9, -12 };



//        public interface noteItem
//        {
//            int? pit { get; set; }
//            int? shhd { get; set; }
//            int? shac { get; set; }
//            int? dur { get; set; }
//            int? midi { get; set; }
//            int? jn { get; set; }
//            int? jo { get; set; }
//        }

//        public interface VoiceItem
//        {
//            int? type { get; set; }
//            int? v { get; set; }
//            int? dur { get; set; }
//            int? time { get; set; }
//            FormationInfo fmt { get; set; }
//            PageVoiceTune p_v { get; set; }
//            noteItem[] notes { get; set; }
//            int? x { get; set; }
//            int? y { get; set; }
//            int? wr { get; set; }
//            int? wl { get; set; }
//            int? st { get; set; }
//            VoiceItem next { get; set; }
//            VoiceItem prev { get; set; }
//            VoiceItem ts_prev { get; set; }
//            VoiceItem ts_next { get; set; }
//            bool? err { get; set; }
//            int? nhd { get; set; }
//        }

//        var STEM_MIN = 16.3;
//        var STEM_MIN2 = 16.3;
//        var STEM_MIN3 = 16.3;
//        var STEM_MIN4 = 16.3;
//        var STEM_CH_MIN = 16.3;
//        var cache, anno_a = new List<object>();
//        var min_tb = new int?[][]
//        {
//    new int?[] { STEM_MIN, STEM_MIN, STEM_MIN2, STEM_MIN3, STEM_MIN4, STEM_MIN4 },
//    new int?[] { STEM_CH_MIN, STEM_CH_MIN, STEM_CH_MIN2, STEM_CH_MIN3, STEM_CH_MIN4, STEM_CH_MIN4 }
//        };
//        var sharp_cl = new sbyte[] { 2422, 9, 15 };
//        var flat_cl = new sbyte[] { 1233, 18, 24 };
//        var sharp1 = new sbyte[] { -922, 12, -9 };
//        var sharp2 = new sbyte[] { 1231, -9, 12 };
//        var flat1 = new sbyte[] { 923, -12, 9 };
//        var flat2 = new sbyte[] { -1223, 9, -12 };





//        public interface noteItem
//        {
//            int? pit { get; set; }
//            int? shhd { get; set; }
//            int? shac { get; set; }
//            int? dur { get; set; }
//            int? midi { get; set; }
//            int? jn { get; set; }
//            int? jo { get; set; }
//        }

//        public interface VoiceItem
//        {
//            int? type { get; set; }
//            int? v { get; set; }
//            int? dur { get; set; }
//            int? time { get; set; }
//            FormationInfo fmt { get; set; }
//            PageVoiceTune p_v { get; set; }
//            noteItem[] notes { get; set; }
//            int? x { get; set; }
//            int? y { get; set; }
//            int? wr { get; set; }
//            int? wl { get; set; }
//            int? st { get; set; }
//            VoiceItem next { get; set; }
//            VoiceItem prev { get; set; }
//            VoiceItem ts_prev { get; set; }
//            VoiceItem ts_next { get; set; }
//            bool? err { get; set; }
//            int? nhd { get; set; }
//        }

//        var STEM_MIN = 16.3;
//        var STEM_MIN2 = 16.3;
//        var STEM_MIN3 = 16.3;
//        var STEM_MIN4 = 16.3;
//        var STEM_CH_MIN = 16.3;
//        var cache;
//        var anno_a = new List<object>();

//        var min_tb = new List<List<double>>()
//{
//    new List<double> { STEM_MIN, STEM_MIN, STEM_MIN2, STEM_MIN3, STEM_MIN4, STEM_MIN4 },
//    new List<double> { STEM_CH_MIN, STEM_CH_MIN, STEM_CH_MIN2, STEM_CH_MIN3, STEM_CH_MIN4, STEM_CH_MIN4 }
//};

//        var sharp_cl = new sbyte[] { 2422, 9, 15 };
//        var flat_cl = new sbyte[] { 1233, 18, 24 };
//        var sharp1 = new sbyte[] { -922, 12, -9 };
//        var sharp2 = new sbyte[] { 1231, -9, 12 };
//        var flat1 = new sbyte[] { 923, -12, 9 };
//        var flat2 = new sbyte[] { -1223, 9, -12 };










//    }




}











