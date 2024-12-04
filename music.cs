// abc2svg - music.js - music generation
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


//var gene, staff_tb, nstaff, tsnext, realwidth, insert_meter, spf_last, smallest_duration;
//var dx_tb =  new Float32Array([1.1, 2.2]);
//var hw_tb = new Float32Array([1.1, 2.2]);
//var w_note = new Float32Array([1.1,2.2]);

//var delta_tb = {t: 0,c: 6,b: 12,p: -2}
//var rest_sp = [[18, 18],[12, 18],[12, 12]]

//    var MAXPIT = 48 * 2
//    var blocks = []

//var slurePos = 0;

using System.Collections.Generic;
using static autocad_part2.abc2svg;

namespace autocad_part2
{
    public partial class Abc
    {
        /* -- decide whether to shift heads to other side of stem on chords -- */
        /* this routine is called only once per tune */
        /* -- 決定是否將琴頭移到和弦上的琴幹的另一側 -- */
        /* 每首曲子只呼叫此例程一次 */

        // distance for no overlap - index: [prev acc][cur acc]
        // 無重疊的距離 - 索引: [prev acc][cur acc]
        //var dt_tb = [
        //	[5, 5, 5, 5],		/* dble sharp 雙升記號*/
        //	[5, 6, 6, 6],		/* sharp 升記號*/
        //	[5, 6, 5, 6],		/* natural 還原記號*/
        //	[5, 5, 5, 5]		/* flat / dble flat 降記號/雙降記號*/
        //]

        // accidental x offset - index = note head type
        // 意外 x 偏移 - 索引 = 音符頭類型
        public static double[] dx_tb = new double[]
        {
        10,     // FULL
        10,     // EMPTY
        11,     // OVAL
        13,     // OVALBARS
        15      // SQUARE     
        };
        // head width  - index = note head type
        // 符頭寬度 - 索引 = 符頭類型
        public static double[] hw_tb = new double[]
        {
        4.7f,   // FULL
        5f,     // EMPTY
        6f,     // OVAL
        7.2f,   // OVALBARS
        7.5f    // SQUARE
        };
        /* head width for voice overlap - index = note head type */
        /* 語音重疊的頭寬度 - 索引 = 音符頭類型 */
        public static double[] w_note = new double[]
        {
        3.5f,   // FULL
        3.7f,   // EMPTY
        5f,     // OVAL
        6f,     // OVALBARS
        7f      // SQUARE
        };



        public Gener gene;
        public  List<Staff> staff_tb;
        public  int nstaff;         // current number of staves
        public  int tsnext;         // next line when cut
        public  double realwidth;    // real staff width while generating
        public  bool insert_meter;  // insert the time signature
        public  double spf_last;     // spacing for last short line
        public  double smallest_duration;

        /******************************************************/

        /**
        * get head type, dots, flags of note/rest for a duration
        * 取得一段時間內的頭類型、點、音符 / 休息標誌
        */
        (int head, int dots, int flags) identify_note(VoiceItem s, int dur_o)
        {
            int head = 0, flags;
            int dots = 0;
            int dur = dur_o;

            if (dur % 12 != 0)
                error(1, s, "Invalid note duration $1", dur);
            dur /= 12;			/* see C.BLEN for values */
            if (dur == 0)
                error(1, s, "Note too short");
            for (flags = 5; dur != 0; dur >>= 1, flags--)
            {
                if ((dur & 1) != 0)//找出奇數
                    break;
            }
            dur >>= 1;
            if ((dur + 1) != 0 && dur != 0)
            {
                if (s.type != C.REST || dur_o != s.p_v.wmeasure)
                    error(0, s, "Non standard note duration $1", dur_o);
            }
            while (dur >> dots > 0)
                dots++;

            flags -= dots;
            if (flags >= 0)
            {
                head = C.FULL;
            }
            else
            {
                switch (flags)
                {
                    case -4:
                        head = C.SQUARE;
                        break;
                    case -3:
                        head = s.fmt.squarebreve ? C.SQUARE : C.OVALBARS;
                        break;
                    case -2:
                        head = C.OVAL;
                        break;
                    case -1:
                        head = C.EMPTY;
                        break;
                    default:
                        error(1, s, "Note too long");
                        break;
                }
            }

            return (head, dots, flags);
        }

        void set_head_shift(VoiceItem s)
        {
            int i, i1, i2, d, ps;
            double dx;
            double dx_head = dx_tb[s.head];
            int dir = s.stem;
            int n = s.nhd;

            if (n == 0)
                return;

            dx = dx_head * 0.74d;
            if (s.grace)
                dx *= 0.6f;
            if (dir >= 0)
            {
                i1 = 1;
                i2 = n + 1;
                ps = s.notes[0].pit;
            }
            else
            {
                dx = -dx;
                i1 = n - 1;
                i2 = -1;
                ps = s.notes[n].pit;
            }
            bool shift = false;
            double dx_max = 0;
            for (i = i1; i != i2; i += dir)
            {
                d = s.notes[i].pit - ps;
                ps = s.notes[i].pit;
                if (d == 0)
                {
                    if (shift)
                    {
                        double new_dx = s.notes[i].shhd = s.notes[i - dir].shhd + dx;
                        if (dx_max < new_dx)
                            dx_max = new_dx;
                        continue;
                    }
                    if (i + dir != i2 && ps + dir == s.notes[i + dir].pit)
                    {
                        s.notes[i].shhd = -dx;
                        if (dx_max < -dx)
                            dx_max = -dx;
                        continue;
                    }
                }
                if (d < 0)
                    d = -d;
                if (d > 3 || (d >= 2 && s.head != C.SQUARE))
                {
                    shift = false;
                }
                else
                {
                    shift = !shift;
                    if (shift)
                    {
                        s.notes[i].shhd = dx;
                        if (dx_max < dx)
                            dx_max = dx;
                    }
                }
            }
            s.xmx = dx_max;
        }

        // set the accidental shifts for a set of chords
        // 設定一組和弦的臨時記號改變
        void acc_shift(dynamic notes, double dx_head)
        {
            int i, i1, i2, dx, dx1, dx2, ps, p1, acc;
            int n = notes.Length;

            for (i = n - 1; --i >= 0;)
            {
                dx = notes[i].shhd;
                if (dx == 0 || dx > 0)
                    continue;
                dx = dx_head - dx;
                ps = notes[i].pit;
                for (i1 = n; --i1 >= 0;)
                {
                    if (notes[i1].acc == 0)
                        continue;
                    p1 = notes[i1].pit;
                    if (p1 < ps - 3)
                        break;
                    if (p1 > ps + 3)
                        continue;
                    if (notes[i1].shac < dx)
                        notes[i1].shac = dx;
                }
            }

            for (i1 = n; --i1 >= 0;)
            {
                if (notes[i1].acc != 0)
                {
                    p1 = notes[i1].pit;
                    dx1 = notes[i1].shac;
                    if (dx1 == 0)
                    {
                        dx1 = notes[i1].shhd;
                        if (dx1 < 0)
                            dx1 = dx_head - dx1;
                        else
                            dx1 = dx_head;
                    }
                    break;
                }
            }
            if (i1 < 0)
                return;
            for (i2 = 0; i2 < i1; i2++)
            {
                if (notes[i2].acc != 0)
                {
                    ps = notes[i2].pit;
                    dx2 = notes[i2].shac;
                    if (dx2 == 0)
                    {
                        dx2 = notes[i2].shhd;
                        if (dx2 < 0)
                            dx2 = dx_head - dx2;
                        else
                            dx2 = dx_head;
                    }
                    break;
                }
            }
            if (i1 == i2)
            {
                notes[i1].shac = dx1;
                return;
            }

            if (p1 > ps + 4)
            {
                if (dx1 > dx2)
                    dx2 = dx1;
                notes[i1].shac = notes[i2].shac = dx2;
            }
            else
            {
                notes[i1].shac = dx1;
                if (notes[i1].pit != notes[i2].pit)
                    dx1 += 7;
                notes[i2].shac = dx2 = dx1;
            }
            dx2 += 7;

            for (i = i1; --i > i2;)
            {
                acc = notes[i].acc;
                if (acc == 0)
                    continue;
                dx = notes[i].shac;
                if (dx < dx2)
                    dx = dx2;
                ps = notes[i].pit;
                for (i1 = n; --i1 > i;)
                {
                    if (notes[i1].acc == 0)
                        continue;
                    p1 = notes[i1].pit;
                    if (p1 >= ps + 4)
                    {
                        if (p1 > ps + 4 || acc < 0 || notes[i1].acc < 0)
                            continue;
                    }
                    if (dx > notes[i1].shac - 6)
                    {
                        dx1 = notes[i1].shac + 7;
                        if (dx1 > dx)
                            dx = dx1;
                    }
                }
                notes[i].shac = dx;
            }
        }

        /* set the horizontal shift of accidentals */
        /* this routine is called only once per tune */
        /* 設定臨時記號的水平移動 */
        /* 每首曲子只呼叫此例程一次 */
        void set_acc_shft()
        {
            VoiceItem s, s2;
            int st, i, acc, t, dx_head;
            List<autocad_part2.NoteItem> notes;

            s = tsfirst;
            while (s != null)
            {
                if (s.type != C.NOTE || s.invis)
                {
                    s = s.ts_next;
                    continue;
                }
                st = s.st;
                t = s.time;
                acc = false;
                for (s2 = s; s2 != null; s2 = s2.ts_next)
                {
                    if (s2.time != t || s2.type != C.NOTE || s2.st != st)
                        break;
                    if (acc)
                        continue;
                    for (i = 0; i <= s2.nhd; i++)
                    {
                        if (s2.notes[i].acc != 0)
                        {
                            acc = true;
                            break;
                        }
                    }
                }
                if (!acc)
                {
                    s = s2;
                    continue;
                }

                dx_head = dx_tb[s.head];

                notes = new dynamic[] { };
                for (; s != s2; s = s.ts_next)
                {
                    if (!s.invis)
                        Array.Resize(ref notes, notes.Length + s.notes.Length);
                    Array.Copy(s.notes, 0, notes, notes.Length - s.notes.Length, s.notes.Length);
                }
                Array.Sort(notes, (a, b) => a.pit.CompareTo(b.pit));
                acc_shift(notes, dx_head);
            }
        }

        // link a symbol before an other one
        // 將一個符號連結到另一個符號之前
        void lkvsym(VoiceItem s, dynamic next)
        {
            s.next = next;
            s.prev = next.prev;
            if (s.prev != null)
                s.prev.next = s;
            else
                s.p_v.sym = s;
            next.prev = s;
        }

        void lktsym(VoiceItem s, dynamic next)
        {
            dynamic old_wl;

            s.ts_next = next;
            if (next != null)
            {
                s.ts_prev = next.ts_prev;
                if (s.ts_prev != null)
                    s.ts_prev.ts_next = s;
                next.ts_prev = s;
            }
            else
            {
                throw new Exception("Bad linkage");
                s.ts_prev = null;
            }
            s.seqst = !s.ts_prev || s.time != s.ts_prev.time || (w_tb[s.ts_prev.type] != w_tb[s.type] && w_tb[s.ts_prev.type]);
            if (!next || next.seqst)
                return;
            next.seqst = next.time != s.time || (w_tb[s.type] != w_tb[next.type] && w_tb[s.type]);
            if (next.seqst)
            {
                old_wl = next.wl;
                set_width(next);
                if (next.a_ly)
                    ly_set(next);
                if (!next.shrink)
                {
                    next.shrink = next.wl;
                    if (next.prev != null)
                        next.shrink += next.prev.wr;
                }
                else
                {
                    next.shrink += next.wl - old_wl;
                }
                next.space = 0;
            }
        }

        /* -- unlink a symbol -- */
        /* -- 取消符號連結 -- */
        void unlksym(VoiceItem s)
        {
            if (s.next != null)
                s.next.prev = s.prev;
            if (s.prev != null)
                s.prev.next = s.next;
            else
                s.p_v.sym = s.next;
            if (s.ts_next != null)
            {
                if (s.seqst)
                {
                    if (s.ts_next.seqst)
                    {
                        s.ts_next.shrink += s.shrink;
                        s.ts_next.space += s.space;
                    }
                    else
                    {
                        s.ts_next.seqst = true;
                        s.ts_next.shrink = s.shrink;
                        s.ts_next.space = s.space;
                    }
                }
                else
                {
                    if (s.ts_next.seqst && s.ts_prev != null && s.ts_prev.seqst && !w_tb[s.ts_prev.type])
                    {
                        s.ts_next.seqst = false;
                        s.shrink = s.ts_next.shrink;
                        s.space = s.ts_next.space;
                    }
                }
                s.ts_next.ts_prev = s.ts_prev;
            }
            if (s.ts_prev != null)
                s.ts_prev.ts_next = s.ts_next;
            if (tsfirst == s)
                tsfirst = s.ts_next;
            if (tsnext == s)
                tsnext = s.ts_next;
        }

        /* -- insert a clef change (treble or bass) before a symbol -- */
        /* -- 在符號前插入譜號變化（高音或低音） -- */
        dynamic insert_clef(VoiceItem s, int clef_type, int clef_line)
        {
            dynamic p_voice = s.p_v;
            dynamic new_s;
            dynamic st = s.st;

            if (s.type == C.BAR && s.prev != null && s.prev.type == C.BAR && s.prev.bar_type[0] != ':')
                s = s.prev;

            p_voice.last_sym = s.prev;
            if (p_voice.last_sym == null)
                p_voice.sym = null;
            p_voice.time = s.time;
            new_s = sym_add(p_voice, C.CLEF);
            new_s.next = s;
            s.prev = new_s;

            new_s.clef_type = clef_type;
            new_s.clef_line = clef_line;
            new_s.st = st;
            new_s.clef_small = true;
            new_s.second = null;
            new_s.notes = new dynamic[] { };
            new_s.notes[0] = new { pit = s.notes[0].pit };
            new_s.nhd = 0;

            lktsym(new_s, s);
            if (s.soln)
            {
                new_s.soln = true;
                s.soln = null;
            }
            return new_s;
        }

        /* -- set the staff of the floating voices -- */
        /* this function is called only once per tune */
        /* -- 設定浮動聲音的五線譜 -- */
        /* 每個曲調只呼叫此函數一次 */
        void set_float()
        {
            VoiceItem s, s1;
            dynamic p_voice, st, staff_chg;
            int v, up, down;
            for (v = 0; v < voice_tb.Length; v++)
            {
                p_voice = voice_tb[v];
                staff_chg = false;
                st = p_voice.st;
                for (s = p_voice.sym; s != null; s = s.next)
                {
                    if (!s.floating)
                    {
                        while (s != null && !s.floating)
                            s = s.next;
                        if (s == null)
                            break;
                        staff_chg = false;
                    }
                    if (!s.dur)
                    {
                        if (staff_chg)
                            s.st++;
                        continue;
                    }
                    if (s.notes[0].pit >= 19)
                    {
                        staff_chg = false;
                        continue;
                    }
                    if (s.notes[s.nhd].pit <= 12)
                    {
                        staff_chg = true;
                        s.st++;
                        continue;
                    }
                    up = 127;
                    for (s1 = s.ts_prev; s1 != null; s1 = s1.ts_prev)
                    {
                        if (s1.st != st || s1.v == s.v)
                            break;
                        if (s1.type == C.NOTE)
                            if (s1.notes[0].pit < up)
                                up = s1.notes[0].pit;
                    }
                    if (up == 127)
                    {
                        if (staff_chg)
                            s.st++;
                        continue;
                    }
                    if (s.notes[s.nhd].pit > up - 3)
                    {
                        staff_chg = false;
                        continue;
                    }
                    down = -127;
                    for (s1 = s.ts_next; s1 != null; s1 = s1.ts_next)
                    {
                        if (s1.st != st + 1 || s1.v == s.v)
                            break;
                        if (s1.type == C.NOTE)
                            if (s1.notes[s1.nhd].pit > down)
                                down = s1.notes[s1.nhd].pit;
                    }
                    if (down == -127)
                    {
                        if (staff_chg)
                            s.st++;
                        continue;
                    }
                    if (s.notes[0].pit < down + 3)
                    {
                        staff_chg = true;
                        s.st++;
                        continue;
                    }
                    up -= s.notes[s.nhd].pit;
                    down = s.notes[0].pit - down;
                    if (!staff_chg)
                    {
                        if (up < down + 3)
                            continue;
                        staff_chg = true;
                    }
                    else
                    {
                        if (up < down - 3)
                        {
                            staff_chg = false;
                            continue;
                        }
                    }
                    s.st++;
                }
            }
        }

        /* -- set the x offset of the grace notes -- */
        /* -- 設定裝飾音的 x 偏移量 -- */
        double set_graceoffs(VoiceItem s)
        {
            dynamic next, m, dx, x;
            double gspleft = s.fmt.gracespace[0];
            double gspinside = s.fmt.gracespace[1];
            double gspright = s.fmt.gracespace[2];
            dynamic g = s.extra;

            if (s.prev != null && s.prev.type == C.BAR)
                gspleft -= 3;
            x = gspleft;

            g.beam_st = true;
            for (; ; g = g.next)
            {
                set_head_shift(g);
                acc_shift(g.notes, 6.5f);
                dx = 0;
                for (m = g.nhd; m >= 0; m--)
                {
                    if (g.notes[m].shac - 2 > dx)
                        dx = g.notes[m].shac - 2;
                }
                x += dx;
                g.x = x;

                if (g.nflags <= 0)
                    g.beam_st = g.beam_end = true;
                next = g.next;
                if (next == null)
                {
                    g.beam_end = true;
                    break;
                }
                if (next.nflags <= 0)
                    g.beam_end = true;
                if (g.beam_end)
                {
                    next.beam_st = true;
                    x += gspinside / 4;
                }
                if (g.nflags <= 0)
                    x += gspinside / 4;
                if (g.y > next.y + 8)
                    x -= 1.5f;
                x += gspinside;
            }

            next = s.next;
            if (next != null && next.type == C.NOTE)
            {
                if (g.y >= 3 * (next.notes[next.nhd].pit - 18))
                    gspright -= 1;
                else if (g.beam_st && g.y < 3 * (next.notes[next.nhd].pit - 18) - 4)
                    gspright += 2;
            }
            x += gspright;

            return x;
        }


        // Compute the smallest spacing between symbols according to chord symbols
        //	so that they stay at the same offset
        // and, also, adjust the spacing due to the lyric words.
        // Constraints:
        // - assume the chord symbols are only in the first staff
        // - treat only the first chord symbol of each symbol
        // - the chord symbols under the staff are ignored
        // 根據和弦符號計算符號之間的最小間距
        // 使它們保持相同的偏移量
        // 並且，也根據歌詞調整間距。
        // 約束：
        // - 假設和弦符號只存在於第一個五線譜中
        // - 僅處理每個符號的第一個和弦符號
        // - 譜表下的和弦符號將被忽略
        /* 字元數：1359 */
        void set_w_chs(VoiceItem s)
        {
            int i;
            char ch;
            double w0, s0, dw;
            double x = 0;
            double n = 0;

            SetFont("vocal");
            while (s != null)
            {
                if (s.Shrink)
                {
                    x += s.Shrink;
                    n++;
                }
                if (s.ALy)          // if some lyric
                    LySet(s);

                if (!s.AGch)
                    continue;
                for (i = 0; i < s.AGch.Length; i++)
                {
                    ch = s.AGch[i];
                    if (ch.Type != 'g' || ch.Y < 0) // upper chord symbol only
                        continue;
                    if (w0 != 0)        // width of the previous chord symbol
                    {
                        if (w0 > x + ch.X)
                        {
                            if (s.Prev // (if not at start of a secondary voice)
                                && s.Prev.SeqSt
                                && s.Prev.Type == C.BAR) // don't move away
                                n--;        // the symbol from a bar
                            dw = (w0 - x - ch.X) / n;
                            while (true)
                            {
                                s0 = s0.TSNext;
                                if (s0.Shrink)
                                    s0.Shrink += dw;
                                if (s0 == s
                                    || s0.Type == C.BAR)
                                    break;
                            }
                        }
                    }
                    s0 = s;
                    w0 = ch.Text.Wh[0];
                    n = 0;
                    //			x = ch.Font.Box ? -2 : 0
                    x = 0;
                    break;
                }
                s = s.TSNext;
            }
        }



        // compute the width needed by the left and right annotations
        // 計算左右註解所需的寬度
        double gchord_width(VoiceItem s, double wlnote, double wlw)
        {
            dynamic gch;
            double w;
            int ix;
            double arspc = 0;

            for (ix = 0; ix < s.a_gch.Length; ix++)
            {
                gch = s.a_gch[ix];
                switch (gch.type)
                {
                    case '<':
                        w = gch.text.wh[0] + wlnote;
                        if (w > wlw)
                            wlw = w;
                        break;
                    case '>':
                        w = gch.text.wh[0] + s.wr;
                        if (w > arspc)
                            arspc = w;
                        break;
                }
            }
            if (s.wr < arspc)
                s.wr = arspc;

            return wlw;
        }


        /* -- set the width of a symbol -- */
        /* This routine sets the minimal left and right widths wl,wr
         * so that successive symbols are still separated when
         * no extra glue is put between them */
        // (possible hook)
        /* -- 設定符號的寬度 -- */
        /* 此例程設定最小左右寬度 wl,wr
          * 這樣連續的符號在以下情況下仍然是分開的
          * 它們之間沒有額外的膠水 */
        //（可能的鉤子）
        /* 字元數：11885 */
        public void set_width(VoiceItem s)
        {
            double s2, i, m, xx, w, wlnote, wlw, acc, nt, bar_type, meter, last_acc, n1, n2, esp, tmp;

            if (s.play)
            {
                s.wl = s.wr = 0;
                return;
            }

            switch (s.type)
            {
                case C.NOTE:
                case C.REST:
                    s.wr = wlnote = s.invis ? 0 : hw_tb[s.head];

                    if (s.xmx > 0)
                        s.wr += s.xmx + 4;

                    for (s2 = s.prev; s2 != null; s2 = s2.prev)
                    {
                        if (w_tb[s2.type])
                            break;
                    }

                    if (s2 != null)
                    {
                        switch (s2.type)
                        {
                            case C.BAR:
                            case C.CLEF:
                            case C.KEY:
                            case C.METER:
                                wlnote += 3;
                                break;
                            case C.STBRK:
                                wlnote += 8;
                                break;
                        }
                    }

                    for (m = 0; m <= s.nhd; m++)
                    {
                        nt = s.notes[m];
                        xx = nt.shhd;

                        if (xx < 0)
                        {
                            if (wlnote < -xx + 5)
                                wlnote = -xx + 5;
                        }

                        acc = nt.acc;

                        if (acc != null)
                        {
                            tmp = nt.shac + (acc.GetType() == typeof(object) ? 5.5f : 3.5f);

                            if (wlnote < tmp)
                                wlnote = tmp;
                        }

                        if (nt.a_dd)
                            wlnote += deco_wch(nt);
                    }

                    if (s2 != null)
                    {
                        switch (s2.type)
                        {
                            case C.BAR:
                            case C.CLEF:
                            case C.KEY:
                            case C.METER:
                                wlnote -= 3;
                                break;
                        }
                    }

                    if (s.a_dd)
                        wlnote = deco_width(s, wlnote);

                    if (s.beam_st && s.beam_end && s.stem > 0 && s.nflags > 0)
                    {
                        if (s.wr < s.xmx + 9)
                            s.wr = s.xmx + 9;
                    }

                    if (s.dots)
                    {
                        if (s.wl == null)
                        {
                            switch (s.head)
                            {
                                case C.SQUARE:
                                case C.OVALBARS:
                                    s.xmx += 3;
                                    break;
                                case C.OVAL:
                                    s.xmx += 1;
                                    break;
                            }
                        }

                        if (s.wr < s.xmx + 8)
                            s.wr = s.xmx + 8;

                        if (s.dots >= 2)
                            s.wr += 3.5f * (s.dots - 1);
                    }

                    if (s.trem2 && s.beam_end && wlnote < 20)
                        wlnote = 20;

                    wlw = wlnote;

                    if (s2 != null)
                    {
                        switch (s2.type)
                        {
                            case C.NOTE:
                                if (s2.stem > 0 && s.stem < 0)
                                {
                                    if (wlw < 7)
                                        wlw = 7;
                                }

                                if ((s.y > 27 && s2.y > 27) || (s.y < -3 && s2.y < -3))
                                {
                                    if (wlw < 6)
                                        wlw = 6;
                                }

                                if (s2.tie)
                                {
                                    if (wlw < 14)
                                        wlw = 14;
                                }
                                break;
                            case C.CLEF:
                                if (s2.second || s2.clef_small)
                                    break;
                                goto case C.KEY;
                            case C.KEY:
                                if (s.a_gch)
                                    wlw += 4;
                                goto case C.METER;
                            case C.METER:
                                wlw += 3;
                                break;
                        }
                    }

                    if (s.a_gch)
                        wlw = gchord_width(s, wlnote, wlw);

                    if (s.prev != null && s.prev.type == C.GRACE)
                    {
                        s.prev.wl += wlnote - 4.5f;
                        s.wl = s.prev.wl;
                    }
                    else
                    {
                        s.wl = wlw;
                    }
                    return;
                case C.SPACE:
                    xx = s.width / 2;
                    s.wr = xx;

                    if (s.a_gch)
                        xx = gchord_width(s, xx, xx);

                    if (s.a_dd)
                        xx = deco_width(s, xx);

                    s.wl = xx;
                    return;
                case C.BAR:
                    bar_type = s.bar_type;

                    switch (bar_type)
                    {
                        case "|":
                            w = 5;
                            break;
                        case "[":
                            w = 0;
                            break;
                        default:
                            w = 2 + 2.8f * bar_type.Length;

                            for (i = 0; i < bar_type.Length; i++)
                            {
                                switch (bar_type[i])
                                {
                                    case "[":
                                    case "]":
                                        w += 1;
                                        goto case ":";
                                    case ":":
                                        w += 2;
                                        break;
                                }
                            }
                            break;
                    }

                    s.wl = w;

                    if (s.next != null && s.next.type != C.METER)
                        s.wr = 7;
                    else
                        s.wr = 5;

                    if (s.invis && s.prev != null && s.prev.bar_type)
                        s.wl = s.wr = 2;

                    s2 = s.prev;

                    if (s2 != null && s2.type == C.GRACE)
                        s.wl -= 6;

                    for (; s2 != null; s2 = s2.prev)
                    {
                        if (w_tb[s2.type])
                        {
                            if (s2.type == C.STBRK)
                                s.wl -= 12;
                            break;
                        }
                    }

                    if (s.a_dd)
                        s.wl = deco_width(s, s.wl);

                    if (s.text != null && s.text.Length < 4 && s.next != null && s.next.a_gch)
                    {
                        s.wr += strwh(s.text)[0] + 2;

                        if (cfmt.measurenb > (0 & s.bar_num) && s.bar_num % cfmt.measurenb)
                            s.wr += 4;
                    }
                    return;
                case C.CLEF:
                    if (s.invis)
                    {
                        s.wl = s.wr = 1;
                        return;
                    }

                    if (s.prev != null && s.prev.type == C.STBRK)
                    {
                        s.wl = 6;
                        s.wr = 13;
                        s.clef_small = null;
                        return;
                    }

                    s.wl = s.clef_small ? 11 : 12;
                    s.wr = s.clef_small ? 8 : 13;
                    return;
                case C.KEY:
                    if (s.invis)
                    {
                        s.wl = s.wr = 0;
                        return;
                    }

                    s.wl = 0;
                    esp = 3;
                    n1 = s.k_sf;
                    if (s.k_old_sf != null && (s.fmt.cancelkey || n1 == 0))
                        n2 = s.k_old_sf;
                    else
                        n2 = 0;

                    if (n1 * n2 >= 0)
                    {
                        if (n1 < 0)
                            n1 = -n1;

                        if (n2 < 0)
                            n2 = -n2;

                        if (n2 > n1)
                            n1 = n2;
                    }
                    else
                    {
                        n1 -= n2;

                        if (n1 < 0)
                            n1 = -n1;

                        esp += 3;
                    }

                    if (s.k_bagpipe == 'p')
                        n1++;

                    if (s.k_a_acc != null)
                    {
                        n2 = s.k_a_acc.Length;

                        if (s.exp)
                            n1 = n2;
                        else
                            n1 += n2;

                        if (n2 != 0)
                            last_acc = s.k_a_acc[0].acc;

                        for (i = 1; i < n2; i++)
                        {
                            acc = s.k_a_acc[i];

                            if (acc.pit > s.k_a_acc[i - 1].pit + 6 || acc.pit < s.k_a_acc[i - 1].pit - 6)
                                n1--;
                            else if (acc.acc != last_acc)
                                esp += 3;

                            last_acc = acc.acc;
                        }
                    }

                    if (n1 == 0)
                        break;

                    s.wr = 5.5f * n1 + esp;

                    if (s.prev != null && !s.prev.bar_type)
                        s.wl += 2;

                    return;
                case C.METER:
                    s.x_meter = new double[s.a_meter.Length];

                    if (s.a_meter.Length == 0)
                        break;

                    wlw = 0;

                    for (i = 0; i < s.a_meter.Length; i++)
                    {
                        meter = s.a_meter[i];

                        switch (meter.top[0])
                        {
                            case 'C':
                            case 'c':
                            case 'o':
                                s.x_meter[i] = wlw + 6;
                                wlw += 12;
                                break;
                            case '.':
                            case '|':
                                s.x_meter[i] = s.x_meter[i - 1];
                                break;
                            default:
                                w = 0;

                                if (meter.bot == null || meter.top.Length > meter.bot.Length)
                                    meter = meter.top;
                                else
                                    meter = meter.bot;

                                for (m = 0; m < meter.Length; m++)
                                {
                                    switch (meter[m])
                                    {
                                        case '(':
                                            wlw += 4;
                                            goto case ')';
                                        case ')':
                                        case '1':
                                            w += 4;
                                            break;
                                        default:
                                            w += 12;
                                            break;
                                    }
                                }

                                s.x_meter[i] = wlw + w / 2;
                                wlw += w;
                                break;
                        }
                    }

                    s.wl = 1;
                    s.wr = wlw + 7;
                    return;
                case C.MREST:
                    s.wl = 6;
                    s.wr = 66;
                    return;
                case C.GRACE:
                    if (s.invis)
                        break;

                    s.wl = set_graceoffs(s);
                    s.wr = 0;

                    if (s.a_ly)
                        ly_set(s);

                    return;
                case C.STBRK:
                    s.wl = s.xmx;
                    s.wr = 8;
                    return;
                case C.CUSTOS:
                    s.wl = s.wr = 4;
                    return;
                case C.TEMPO:
                    tempo_build(s);
                    break;
                case C.BLOCK:
                case C.REMARK:
                case C.STAVES:
                    break;
                default:
                    error(2, s, "set_width - Cannot set width for symbol $1", s.type);
                    break;
            }

            s.wl = s.wr = 0;
        }


        // convert delta time to natural spacing
        // 將增量時間轉換為自然間距
        double time2space(VoiceItem s, int len)
        {
            int i, l;
            double space;

            if (smallest_duration >= C.BLEN / 2)
            {
                if (smallest_duration >= C.BLEN)
                    len /= 4;
                else
                    len /= 2;
            }
            else if (s.next == null && len >= C.BLEN)
            {
                len /= 2;
            }
            if (len >= C.BLEN / 4)
            {
                if (len < C.BLEN / 2)
                    i = 5;
                else if (len < C.BLEN)
                    i = 6;
                else if (len < C.BLEN * 2)
                    i = 7;
                else if (len < C.BLEN * 4)
                    i = 8;
                else
                    i = 9;
            }
            else
            {
                if (len >= C.BLEN / 8)
                    i = 4;
                else if (len >= C.BLEN / 16)
                    i = 3;
                else if (len >= C.BLEN / 32)
                    i = 2;
                else if (len >= C.BLEN / 64)
                    i = 1;
                else
                    i = 0;
            }
            l = len - ((C.BLEN / 16 / 8) << i);
            space = cfmt.spatab[i];
            if (l != 0)
            {
                if (l < 0)
                {
                    space = cfmt.spatab[0] * len / (C.BLEN / 16 / 8);
                }
                else
                {
                    if (i >= 9)
                        i = 8;
                    space += (cfmt.spatab[i + 1] - cfmt.spatab[i]) * l / ((C.BLEN / 16 / 8) << i);
                }
            }
            return space;
        }

        /******************************************************/


        // set the natural space
        // 設定自然空間
        /* 字元數：2530 */
        double set_space(VoiceItem s, double ptime)
        {
            double space, len, s2, stemdir;

            len = s.time - ptime;

            if (len == 0)
            {
                switch (s.type)
                {
                    case C.MREST:
                        return s.wl;
                }
                return 0;
            }

            if (s.ts_prev.type == C.MREST)
                return 71;

            space = time2space(s, len);

            while (!s.dur)
            {
                switch (s.type)
                {
                    case C.BAR:
                        if (!s.next)
                            space *= .9f;
                        return space * .9f - 3;
                    case C.CLEF:
                        return space - s.wl - s.wr;
                    case C.BLOCK:
                    case C.REMARK:
                    case C.STAVES:
                    case C.TEMPO:
                        s = s.ts_next;
                        if (s == null)
                            return space;
                        continue;
                }
                break;
            }

            if (s.dur && len <= C.BLEN / 4)
            {
                s2 = s;

                while (s2 != null)
                {
                    if (!s2.beam_st)
                    {
                        space *= .9f;
                        break;
                    }

                    s2 = s2.ts_next;

                    if (s2 == null || s2.seqst)
                        break;
                }
            }

            if (s.type == C.NOTE && s.nflags >= -1 && s.stem > 0)
            {
                stemdir = true;

                for (s2 = s.ts_prev; s2 != null && s2.time == ptime; s2 = s2.ts_prev)
                {
                    if (s2.type == C.NOTE && (s2.nflags < -1 || s2.stem > 0))
                    {
                        stemdir = false;
                        break;
                    }
                }

                if (stemdir)
                {
                    for (s2 = s.ts_next; s2 != null && s2.time == s.time; s2 = s2.ts_next)
                    {
                        if (s2.type == C.NOTE && (s2.nflags < -1 || s2.stem < 0))
                        {
                            stemdir = false;
                            break;
                        }
                    }

                    if (stemdir)
                        space *= .9f;
                }
            }

            return space;
        }

        // set the spacing inside tuplets or L: factor
        // 設定連音或 L: 因子內的間距
        /* 字元數：682 */
        static void set_sp_tup(object s, object s_et)
        {
            var tim = s.time;
            var ttim = s_et.time - tim;
            var sp = time2space(s, ttim);
            var s2 = s;
            var wsp = 0;
            while (true)
            {
                s2 = s2.ts_next;
                if (s2.seqst)
                {
                    wsp += s2.space;
                    if (s2.bar_type)
                        wsp += 10;
                }
                if (s2 == s_et)
                    break;
            }
            sp = (sp + wsp) / 2 / ttim;
            while (true)
            {
                s = s.ts_next;
                if (s.seqst)
                {
                    s.space = sp * (s.time - tim);
                    tim = s.time;
                }
                if (s == s_et)
                    break;
            }
        }


        // return an empty bar
        // 回傳一個空柱
        /* 字元數：434 */
        VoiceItem _bar(VoiceItem s)
        {
            VoiceItem b = new VoiceItem();

            b.type = C.BAR;
            b.bar_type = "|";
            b.fname = s.fname;
            b.istart = s.istart;
            b.iend = s.iend;
            b.v = s.v;
            b.p_v = s.p_v;
            b.st = s.st;
            b.dur = 0;
            b.time = s.time + (s.dur != null ? s.dur : 0);
            b.nhd = 0;
            b.notes = new double[] { s.notes != null ? s.notes[0] : 22 };
            b.seqst = true;
            b.invis = true;
            b.prev = s;
            b.fmt = s.fmt;

            return b;
        }

        // create an invisible bar for end of music lines
        // 建立一個不可見的音樂線結尾欄
        /* 字元數：447 */
        VoiceItem add_end_bar(VoiceItem s)
        {
            VoiceItem b = _bar(s);
            VoiceItem sn = s.ts_next;

            b.wl = 0;
            b.wr = 0;
            b.ts_prev = s;
            b.next = s.next;
            b.ts_next = s.ts_next;
            b.shrink = s.type == C.STBRK ? 0 : (s.wr + 3);

            if (s.next != null)
                s.next.prev = b;

            s.ts_next.ts_prev = b;
            s.next = s.ts_next = b;
            b.space = sn.space * .9f - 3;

            return b;
        }

        /* -- set the width and space of all symbols -- */
        // this function is called once for the whole tune
        // and once more for each new music line
        /* -- 設定所有符號的寬度和間距 -- */
        // 這個函數在整個曲調中被呼叫一次
        // 對於每個新的音樂線再一次
        /* 字元數：2228 */
        void set_allsymwidth(bool first)
        {
            double val, st, s_chs, stup, itup;
            VoiceItem s = tsfirst;
            VoiceItem s2 = s;
            double xa = 0;
            double[] xl = new double[100];
            double[] wr = new double[100];
            double maxx = xa;
            double tim = s.time;

            while (true)
            {
                itup = 0;

                do
                {
                    if ((s.a_gch != null || s.a_ly != null) && s_chs == null)
                        s_chs = s;

                    set_width(s);
                    st = s.st;

                    if (xl[st] == null)
                        xl[st] = 0;

                    if (wr[st] == null)
                        wr[st] = 0;

                    val = xl[st] + wr[st] + s.wl;

                    if (val > maxx)
                        maxx = val;

                    if (s.dur != null && s.dur != s.notes[0].dur && first)
                        itup = 1;

                    s = s.ts_next;
                } while (s != null && !s.seqst);

                s2.shrink = maxx - xa;
                s2.space = s2.ts_prev != null ? set_space(s2, tim) : 0;

                if (s2.space == 0 && s2.ts_prev != null && s2.ts_prev.type == C.SPACE && s2.ts_prev.seqst)
                    s2.space = s2.ts_prev.space /= 2;

                if (itup != 0)
                {
                    if (stup == null)
                        stup = s2;
                }
                else if (stup != null && stup.v == s2.v)
                {
                    set_sp_tup(stup, s2);
                    stup = null;
                }

                if (s2.shrink == 0)
                {
                    if (s2.type == C.CLEF && !s2.ts_prev.bar_type)
                    {
                        s2.seqst = false;
                        s2.time = tim;
                    }
                    else
                    {
                        s2.shrink = 10;
                    }
                }

                tim = s2.time;

                if (s == null)
                    break;

                s = s2;

                do
                {
                    wr[s.st] = 0;
                    s = s.ts_next;
                } while (!s.seqst);

                xa = maxx;

                do
                {
                    st = s2.st;
                    xl[st] = xa;

                    if (s2.wr > wr[st])
                        wr[st] = s2.wr;

                    s2 = s2.ts_next;
                } while (!s2.seqst);
            }

            if (stup != null)
                set_sp_tup(stup, s2);

            if (first && s_chs != null)
                set_w_chs(s_chs);
        }

        // insert a rest, this one replacing a sequence or a measure
        // 插入一個休止符，這個休止符會取代一個序列或一個小節
        /* 字元數：473 */
        VoiceItem to_rest(VoiceItem so)
        {
            VoiceItem s = (VoiceItem)so.Clone();

            s.prev.next = so.ts_prev = so.prev = s.ts_prev.ts_next = s;
            s.next = s.ts_next = so;
            so.seqst = false;
            so.invis = so.play = true;

            s.type = C.REST;
            s.in_tuplet = null;
            s.tp = null;
            s.a_dd = null;
            s.a_gch = null;
            s.sls = null;

            return s;
        }




        /*************************************/


        //double[] gene, staff_tb, nstaff, tsnext, realwidth, insert_meter, spf_last, smallest_duration;
        //double[] dx_tb = new double[] { 1.1f, 2.2f };
        //double[] hw_tb = new double[] { 1.1f, 2.2f };
        //double[] w_note = new double[] { 1.1f, 2.2f };

        /* -- set the repeat sequences / measures -- */
        /* -- 設定重複序列/小節 -- */
        /* 字元數：5315 */
        void set_repeat(VoiceItem s)
        {   // first note
            VoiceItem s2, s3;
            int i, j;
            double dur;
            int n = s.repeat_n;
            int k = s.repeat_k;
            int st = s.st;
            int v = s.v;

            s.repeat_n = 0;             // treated

            /* treat the sequence repeat */
            if (n < 0)
            {               /* number of notes / measures */
                n = -n;
                i = n;              /* number of notes to repeat */
                for (s3 = s.prev; s3 != null; s3 = s3.prev)
                {
                    if (s3.dur == 0)
                    {
                        if (s3.type == C.BAR)
                        {
                            error(1, s3, "Bar in repeat sequence");
                            return;
                        }
                        continue;
                    }
                    if (--i <= 0)
                        break;
                }
                if (s3 == null)
                {
                    error(1, s, errs.not_enough_n);
                    return;
                }
                dur = s.time - s3.time;

                i = k * n;          /* whole number of notes/rests to repeat */
                for (s2 = s; s2 != null; s2 = s2.next)
                {
                    if (s2.dur == 0)
                    {
                        if (s2.type == C.BAR)
                        {
                            error(1, s2, "Bar in repeat sequence");
                            return;
                        }
                        continue;
                    }
                    if (--i <= 0)
                        break;
                }
                if (s2 == null
                    || s2.next == null)
                {       /* should have some symbol */
                    error(1, s, errs.not_enough_n);
                    return;
                }
                for (s2 = s.prev; s2 != s3; s2 = s2.prev)
                {
                    if (s2.type == C.NOTE)
                    {
                        s2.beam_end = true;
                        break;
                    }
                }
                for (j = k; --j >= 0;)
                {
                    i = n;          /* number of notes/rests */
                    if (s.dur != 0)
                        i--;
                    s2 = s.ts_next;
                    while (i > 0)
                    {
                        if (s2.st == st)
                        {
                            s2.invis = s2.play = true;
                            if (s2.seqst && s2.ts_next.seqst)
                                s2.seqst = false;
                            if (s2.v == v
                                && s2.dur != 0)
                                i--;
                        }
                        s2 = s2.ts_next;
                    }
                    s = to_rest(s);
                    s.dur = s.notes[0].dur = dur;
                    s.rep_nb = -1;      // single repeat
                    s.beam_st = true;
                    self.set_width(s);
                    s.head = C.SQUARE;
                    for (s = s2; s != null; s = s.ts_next)
                    {
                        if (s.st == st
                            && s.v == v
                            && s.dur != 0)
                            break;
                    }
                }
                return;
            }

            /* check the measure repeat */
            i = n;              /* number of measures to repeat */
            for (s2 = s.prev.prev; s2 != null; s2 = s2.prev)
            {
                if (s2.type == C.BAR
                    || s2.time == tsfirst.time)
                {
                    if (--i <= 0)
                        break;
                }
            }
            if (s2 == null)
            {
                error(1, s, errs.not_enough_m);
                return;
            }

            dur = s.time - s2.time;     /* repeat duration */

            if (n == 1)
                i = k;          /* repeat number */
            else
                i = n;          /* check only 2 measures */
            for (s2 = s; s2 != null; s2 = s2.next)
            {
                if (s2.type == C.BAR)
                {
                    if (--i <= 0)
                        break;
                }
            }
            if (s2 == null)
            {
                error(1, s, errs.not_enough_m);
                return;
            }

            /* if many 'repeat 2 measures'
             * insert a new %%repeat after the next bar */
            i = k;              /* repeat number */
            if (n == 2 && i > 1)
            {
                s2 = s2.next;
                if (s2 == null)
                {
                    error(1, s, errs.not_enough_m);
                    return;
                }
                s2.repeat_n = n;
                s2.repeat_k = --i;
            }

            /* replace */
            dur /= n;
            if (n == 2)
            {           /* repeat 2 measures (once) */
                s3 = s;
                for (s2 = s.ts_next; ; s2 = s2.ts_next)
                {
                    if (s2.st != st)
                        continue;
                    if (s2.type == C.BAR)
                    {
                        if (s2.v == v)
                            break;
                        continue;
                    }
                    s2.invis = s2.play = true;
                    if (s2.seqst && s2.ts_next.seqst)
                        s2.seqst = false;
                }
                s3 = to_rest(s3);
                s3.dur = s3.notes[0].dur = dur;
                s3.invis = true;
                s2.bar_mrep = 2;
                s3 = s2.next;
                for (s2 = s3.ts_next; ; s2 = s2.ts_next)
                {
                    if (s2.st != st)
                        continue;
                    if (s2.type == C.BAR)
                    {
                        if (s2.v == v)
                            break;
                        continue;
                    }
                    if (s2.dur == 0)
                        continue;
                    s2.invis = s2.play = true;
                    if (s2.seqst && s2.ts_next.seqst)
                        s2.seqst = false;
                }
                s3 = to_rest(s3);
                s3.dur = s3.notes[0].dur = dur;
                s3.invis = true;
                self.set_width(s3);
                return;
            }

            /* repeat 1 measure */
            s3 = s;
            for (j = k; --j >= 0;)
            {
                for (s2 = s3.ts_next; ; s2 = s2.ts_next)
                {
                    if (s2.st != st)
                        continue;
                    if (s2.type == C.BAR)
                    {
                        if (s2.v == v)
                            break;
                        continue;
                    }
                    if (s2.dur == 0)
                        continue;
                    s2.invis = s2.play = true;
                    if (s2.seqst && s2.ts_next.seqst)
                        s2.seqst = false;
                }
                s3 = to_rest(s3);

                s3.dur = s3.notes[0].dur = dur;
                s3.beam_st = true;
                if (k == 1)
                {
                    s3.rep_nb = 1;
                    break;
                }
                s3.rep_nb = k - j + 1;  // number to print above the repeat rest
                s3 = s2.next;
            }
        }

        /* add a custos before the symbol of the next line */
        /* 在下一行的符號前面加上 custos */
        /* 字元數：909 */
        void custos_add(VoiceItem s)
        {
            VoiceItem p_voice, new_s;
            int i;
            VoiceItem s2 = s;

            while (true)
            {
                if (s2.type == C.NOTE)
                    break;
                s2 = s2.next;
                if (s2 == null)
                    return;
            }

            p_voice = s.p_v;
            p_voice.last_sym = s.prev;
            //	if (!p_voice.last_sym)
            //		p_voice.sym = null;
            p_voice.time = s.time;
            new_s = sym_add(p_voice, C.CUSTOS);
            new_s.next = s;
            s.prev = new_s;
            new_s.wl = 0;           // (needed here for lktsym)
            new_s.wr = 4;
            lktsym(new_s, s);

            new_s.shrink = s.shrink;
            if (new_s.shrink < 8 + 4)
                new_s.shrink = 8 + 4;
            new_s.space = s2.space;

            new_s.head = C.FULL;
            new_s.stem = s2.stem;
            new_s.nhd = s2.nhd;
            new_s.notes = new NoteItem[s2.notes.Length];
            for (i = 0; i < s2.notes.Length; i++)
            {
                new_s.notes[i] = new NoteItem
                {
                    pit = s2.notes[i].pit,
                    shhd = 0,
                    dur = C.BLEN / 4
                };
            }
            new_s.stemless = true;
        }

        /* -- define the beginning of a new music line -- */
        /* -- 定義新音樂線的開始 -- */
        /* 字元數：10756 */
        void set_nl(VoiceItem s)
        {           // s = start of line
            VoiceItem p_voice;
            bool done;
            double tim;
            int ptyp;

            // divide the left repeat (|:) or variant bars (|1)
            // the new bars go in the next line
            VoiceItem bardiv(VoiceItem so)
            {       // start of next line
                VoiceItem s, s1, s2, t1, t2;
                int i;

                void new_type(VoiceItem s)
                {
                    string[] t = s.bar_type.Split(':');
                    // [1] = starting ':'s, [2] = middle, [3] = ending ':'s

                    if (t[3].Length == 0)
                    {       // if start of variant
                            // |1 -> | [1
                            // :|]1 -> :|] [1
                        t1 = t[1] + t[2];
                        t2 = "[";
                    }
                    else if (t[1].Length == 0)
                    {       // if left repeat only
                            // x|: -> || [|:
                        t1 = "||";
                        t2 = "[|" + t[3];
                    }
                    else
                    {
                        // :][: -> :|] [|:
                        i = t[2].Length / 2;
                        t1 = t[1] + "|" + t[2].Substring(0, i);
                        t2 = t[2].Substring(i) + "|" + t[3];
                    }
                } // new_typ()

                // change or add a bar for the voice in the previous line
                void eol_bar(VoiceItem s,       // bar |:
                    VoiceItem so,      // start of new line
                    VoiceItem sst)     // first bar (for seqst)
                {
                    VoiceItem s1, s2, s3;

                    // check if a bar in the previous line
                    for (s1 = so.ts_prev; s1.time == s.time; s1 = s1.ts_prev)
                    {
                        if (s1.v != s.v)
                            continue;
                        if (s1.bar_type != null)
                        {
                            if (s1.bar_type != "|")
                                return;     // don't change
                            s2 = s1;        // last symbol in previous line
                            break;
                        }
                        if (s3 == null)
                            s3 = s1.next;   // possible anchor for the new bar
                    }
                    if (s2 == null)
                    {               // if no symbol in previous line
                        s2 = clone(s);
                        if (s3 == null)
                            s3 = s;
                        s2.next = s3;
                        s2.prev = s3.prev;
                        if (s2.prev != null)
                            s2.prev.next = s2;
                        s3.prev = s2;
                        s2.ts_prev = so.ts_prev; // time linkage
                        s2.ts_prev.ts_next = s2;
                        s2.ts_next = so;
                        so.ts_prev = s2;
                        if (s == sst)       // if first inserted bar
                            s2.seqst = true;
                        if (s2.seqst)
                        {
                            for (s = s2.ts_next; !s.seqst; s = s.ts_next)
                                ;
                            s2.shrink = s.shrink;
                            s.shrink = s2.wr + s.wl;
                            s2.space = s.space;
                            s.space = 0;
                        }
                        s2.part = null;
                    }
                    s2.bar_type = "||";
                } // eol_bar()

                // check if there is a left repeat bar at start of the new line
                s = so;             // start of new music line
                while (s != null && s.time == so.time)
                {
                    if (s.bar_type != null && s.bar_type.EndsWith(":"))
                    {
                        s2 = s;
                        break;
                    }
                    s = s.ts_next;
                }
                if (s2 != null)
                {
                    s = s2;
                    while (true)
                    {           // loop on all voices
                        eol_bar(s2, so, s);
                        s2 = s2.ts_next;
                        if (s2 == null || s2.seqst)
                            break;
                    }
                    return so;
                }

                s = so;
                while (s.ts_prev != null
                    && s.ts_prev.time == so.time)
                {
                    s = s.ts_prev;
                    if (s.bar_type != null)
                        s1 = s;         // first previous bar
                    else if (s1 == null && s.type == C.GRACE && s.seqst)
                        so = s;         // if grace note after a bar
                                        // move the start of line
                }
                if (s1 == null
                    || s1.bar_type == null
                    || (!s1.bar_type.EndsWith(":")
                        && s1.text == null))
                    return so;

                // search the new start of the next line
                for (so = s1; so.time == s1.time; so = so.ts_prev)
                {
                    switch (so.ts_prev.type)
                    {
                        case C.KEY:
                        case C.METER:
                        //			case C.PART:
                        case C.TEMPO:
                        case C.STAVES:
                        case C.STBRK:
                            continue;
                    }
                    break;
                }

                // put the new bar before the end of music line
                s = s1;             // keep first bar
                while (true)
                {
                    new_type(s1);
                    s2 = clone(s1);
                    s2.bar_type = t1;
                    s1.bar_type = t2;
                    s2.ts_prev = so.ts_prev;
                    s2.ts_prev.ts_next = s2;
                    s2.ts_next = so;
                    so.ts_prev = s2;
                    if (s1 == s)
                        s2.seqst = true;
                    s2.next = s1;
                    if (s2.prev != null)
                        s2.prev.next = s2;
                    s1.prev = s2;
                    if (s1.rbstop != null)
                        s2.rbstop = s1.rbstop;
                    if (s1.text != null)
                    {
                        s1.invis = true;
                        s1.xsh = null;
                        s2.text = null;
                        s2.rbstart = null;
                    }
                    s2.part = null;
                    s1.a_dd = null;
                    do
                    {
                        s1 = s1.ts_next;
                    } while (!s1.seqst && s1.bar_type == null);
                    if (s1.seqst)
                        break;
                }
                return so;
            } // bardiv()

            // set the start of line marker
            void set_eol(VoiceItem s)
            {
                if (cfmt.custos && voice_tb.Length == 1)
                    custos_add(s);
                s.nl = true;
                s = s.ts_prev;
                if (s.type != C.BAR)
                    add_end_bar(s);
            } // set_eol()

            // put the warning symbols
            // the new symbols go in the previous line
            // 放置警告符號
            // 新符號位於前一行
            void do_warn(VoiceItem s)
            {       // start of next line
                VoiceItem s1, s2, s3, s4;
                int w;

                // advance in the next line
                for (s2 = s; s2 != null && s2.time == s.time; s2 = s2.ts_next)
                {
                    switch (s2.type)
                    {
                        case C.KEY:
                            if (!s.fmt.keywarn
                                || s2.invis)
                                continue;
                            for (s1 = s.ts_prev; s1 != null; s1 = s1.ts_prev)
                            {
                                if (s1.type != C.METER)
                                    break;
                            }
                        // fall thru
                        case C.METER:
                            if (s2.type == C.METER)
                            {
                                if (!s.fmt.timewarn)
                                    continue;
                                s1 = s.ts_prev;
                            }
                        // fall thru
                        case C.CLEF:
                            if (s2.prev == null)        // start of voice
                                continue;
                            if (s2.type == C.CLEF)
                            {
                                if (s2.invis)   // if 'K: clef=none' after bar
                                    break;
                                for (s1 = s.ts_prev; s1 != null; s1 = s1.ts_prev)
                                {
                                    switch (s1.type)
                                    {
                                        case C.BAR:
                                            if (s1.bar_type[0] == ':')
                                                break;
                                        // fall thru
                                        case C.KEY:
                                        case C.METER:
                                            continue;
                                    }
                                    break;
                                }
                            }

                            // put the warning symbol at end of line
                            s3 = clone(s2);     // duplicate the K:/M:/clef

                            lktsym(s3, s1.ts_next);  // time link

                            s1 = s3;
                            while (true)
                            {
                                s1 = s1.ts_next;
                                if (s1.v == s2.v)
                                    break;
                            }
                            lkvsym(s3, s1);     // voice link

                            // care with spacing
                            if (s3.seqst)
                            {
                                self.set_width(s3);
                                s3.shrink = s3.wl;
                                s4 = s3;
                                w = 0;
                                while (true)
                                {
                                    if (s4.wr > w)
                                        w = s4.wr;
                                    if (s4.seqst)
                                        break;
                                    s4 = s4.ts_prev;
                                }
                                s3.shrink += w;
                                s3.space = 0;
                                s4 = s3;
                                while (true)
                                {
                                    if (s4.ts_next.seqst)
                                        break;
                                    s4 = s4.ts_next;
                                }
                                w = 0;
                                while (true)
                                {
                                    if (s4.wl > w)
                                        w = s4.wl;
                                    s4 = s4.ts_next;
                                    if (s4.seqst)
                                        break;
                                }
                                s4.shrink = s3.wr + w;
                            }
                            s3.part = null;
                            continue;
                    }
                    if (w_tb[s2.type] != null)
                        break;      // symbol with a width
                }
            } // do_warn()

            // divide the left repeat and variant bars
            s = bardiv(s);

            // add the warning symbols at the end of the previous line
            do_warn(s);

            /* if normal symbol, cut here */
            if (s.ts_prev.type != C.STAVES)
            {
                set_eol(s);
                return;
            }

            /* go back to handle the staff breaks at end of line */
            for (s = s.ts_prev; s != null; s = s.ts_prev)
            {
                if (s.seqst && s.type != C.CLEF)
                    break;
            }
            done = false;
            ptyp = s.type;
            for (; ; s = s.ts_next)
            {
                if (s == null)
                    return;
                if (s.type == ptyp)
                    continue;
                ptyp = s.type;
                if (done < 0)
                    break;
                switch (s.type)
                {
                    case C.STAVES:
                        if (s.ts_prev == null)
                            return; // null        // no music yet
                        if (s.ts_prev.type == C.BAR)
                            break;
                        while (s.ts_next != null)
                        {
                            if (w_tb[s.ts_next.type] != null
                                && s.ts_next.type != C.CLEF)
                                break;
                            s = s.ts_next;
                        }
                        if (s.ts_next == null || s.ts_next.type != C.BAR)
                            continue;
                        s = s.ts_next;
                    // fall thru
                    case C.BAR:
                        if (done)
                            break;
                        done = true;
                        continue;
                    case C.STBRK:
                        if (!s.stbrk_forced)
                            unlksym(s); /* remove */
                        else
                            done = -1;  // keep the next symbols on the next line
                        continue;
                    case C.CLEF:
                        if (done)
                            break;
                        continue;
                    default:
                        if (!done || (s.prev != null && s.prev.type == C.GRACE))
                            continue;
                        break;
                }
                break;
            }
            set_eol(s);
        }

        /* get the width of the starting clef and key signature */
        // return
        //  r[0] = width of clef and key signature
        //  r[1] = width of the meter
        /* 取得起始譜號和調號的寬度 */
        // 返回
        // r[0] = 譜號和調號的寬度
        // r[1] = 公尺的寬度
        /* 字元數：308 */
        double[] get_ck_width()
        {
            double r0, r1;
            PageVoiceTune p_voice = voice_tb[0];

            self.set_width(p_voice.clef);
            self.set_width(p_voice.ckey);
            self.set_width(p_voice.meter);
            return new double[] { p_voice.clef.wl + p_voice.clef.wr + p_voice.ckey.wl + p_voice.ckey.wr, p_voice.meter.wl + p_voice.meter.wr };
        }

        // get the width of the symbols up to the next soln or eof
        // also, set a x (nice spacing) to all symbols
        // two returned values: width of nice spacing, width with max shrinking
        // 取得到下一個 soln 或 eof 的符號寬度
        // 另外，為所有符號設定 x（良好間距）
        // 兩個回傳值：nice間距的寬度、最大收縮的寬度
        /* 字元數：558 */
        double[] get_width(VoiceItem s, VoiceItem next)
        {
            double shrink, space;
            double w = 0;
            double wmx = 0;
            double sp_fac = (1 - s.fmt.maxshrink);

            while (s != next)
            {
                if (s.seqst)
                {
                    shrink = s.shrink;
                    wmx += shrink;
                    if ((space = s.space) < shrink)
                        w += shrink;
                    else
                        w += shrink * s.fmt.maxshrink + space * sp_fac;
                    s.x = w;
                }
                s = s.ts_next;
            }
            if (next != null)
                wmx += next.wr;     // big key signatures may be wide enough
            return new double[] { w, wmx };
        }




        /******************************************************************/


        /* -- search where to cut the lines according to the staff width -- */
        /* -- 依照五線譜寬度找出在哪裡剪線 -- */
        static void set_lines(object s, object next, object lwidth, object indent)
        {
            object first, s2, s3, s4, s5, x, xmin, xmid, xmax, wwidth, shrink, space, nlines, last = next ? next.ts_prev : null;
            var ws = get_width(s, next);

            if (s.fmt.keywarn && next && next.type == C.KEY && !last.dur)
            {
                ws[0] += next.wr;
                ws[1] += next.wr;
            }

            if (ws[0] + indent < lwidth)
            {
                if (next)
                    next = set_nl(next);
                return next || last;
            }

            wwidth = ws[0] + indent;
            while (true)
            {
                nlines = Math.Ceiling(wwidth / lwidth);
                if (nlines <= 1)
                {
                    if (next)
                        next = set_nl(next);
                    return next || last;
                }

                s2 = first = s;
                xmin = s.x - s.shrink - indent;
                xmax = xmin + lwidth;
                xmid = xmin + wwidth / nlines;
                xmin += wwidth / nlines * s.fmt.breaklimit;
                for (s = s.ts_next; s != next; s = s.ts_next)
                {
                    if (!s.x)
                        continue;
                    if (s.type == C.BAR)
                        s2 = s;
                    if (s.x >= xmin)
                        break;
                }
                s4 = s;
                if (s == next)
                {
                    if (s)
                        s = set_nl(s);
                    return s;
                }

                s3 = null;
                for (; s != next; s = s.ts_next)
                {
                    x = s.x;
                    if (!x)
                        continue;
                    if (x > xmax)
                        break;
                    if (s.type != C.BAR)
                        continue;

                    if (x < xmid)
                    {
                        s3 = s;
                        continue;
                    }
                    if (!s3 || x - xmid < xmid - s3.x)
                        s3 = s;
                    break;
                }

                if (!s3)
                {
                    s = s4;

                    var beam = 0;
                    var bar_time = s2.time;

                    xmax -= 8;
                    s5 = s;
                    for (; s != next; s = s.ts_next)
                    {
                        if (s.seqst)
                        {
                            x = s.x;
                            if (x + s.wr >= xmax)
                                break;
                            if (!beam && !s.in_tuplet && (xmid - s5.x > x - xmid || (s.time - bar_time) % (C.BLEN / 4) == 0))
                                s3 = s;
                        }
                        if (s.beam_st)
                            beam |= 1 << s.v;
                        if (s.beam_end)
                            beam &= ~(1 << s.v);
                        s5 = s;
                    }
                    if (s3)
                    {
                        do
                        {
                            s3 = s3.ts_prev;
                        } while (!s3.seqst);
                    }
                }

                if (!s3)
                {
                    s3 = s = s4;
                    for (; s != next; s = s.ts_next)
                    {
                        x = s.x;
                        if (!x)
                            continue;
                        if (x + s.wr >= xmax)
                            break;
                        if (s3 && x >= xmid)
                        {
                            if (xmid - s3.x > x - xmid)
                                s3 = s;
                            break;
                        }
                        s3 = s;
                    }
                }
                s = s3;
                while (s.ts_next)
                {
                    s = s.ts_next;
                    if (s.seqst)
                        break;
                }

                if (s.nl)
                {
                    error(0, s, "Line split problem - adjust maxshrink and/or breaklimit");
                    nlines = 2;
                    for (s = s.ts_next; s != next; s = s.ts_next)
                    {
                        if (!s.x)
                            continue;
                        if (--nlines <= 0)
                            break;
                    }
                }
                s = set_nl(s);
                if (!s || (next && s.time >= next.time))
                    break;
                wwidth -= s.x - first.x;
                indent = 0;
            }
            return s;
        }

        /* -- cut the tune into music lines -- */
        /* -- 將曲調剪成音樂列 -- */
        static void cut_tune(object lwidth, object lsh)
        {
            object s2, i, mc, pg_sav = new { leftmargin = cfmt.leftmargin, rightmargin = cfmt.rightmargin, pagewidth = cfmt.pagewidth, scale = cfmt.scale };
            var indent = lsh[0] - lsh[1];
            var ckw = get_ck_width();
            var s = tsfirst;

            lwidth -= lsh[1];
            if (cfmt.indent && cfmt.indent > lsh[0])
                indent += cfmt.indent;

            lwidth -= ckw[0];
            indent += ckw[1];

            if (cfmt.custos && voice_tb.Length == 1)
                lwidth -= 12;

            i = s.fmt.barsperstaff;
            if (i)
            {
                for (s2 = s; s2; s2 = s2.ts_next)
                {
                    if (s2.type != C.BAR || !s2.bar_num || --i > 0)
                        continue;
                    while (s2.ts_next && s2.ts_next.type == C.BAR)
                        s2 = s2.ts_next;
                    if (s2.ts_next)
                        s2.ts_next.soln = true;
                    i = s.fmt.barsperstaff;
                }
            }

            s2 = s;
            for (; s != null; s = s.ts_next)
            {
                if (s.type == C.BLOCK)
                {
                    switch (s.subtype)
                    {
                        case "leftmargin":
                        case "rightmargin":
                        case "pagescale":
                        case "pagewidth":
                        case "scale":
                        case "staffwidth":
                            if (!s.soln)
                                self.set_format(s.subtype, s.param);
                            break;
                        case "mc_start":
                            mc = new { lm = cfmt.leftmargin, rm = cfmt.rightmargin };
                            break;
                        case "mc_new":
                        case "mc_end":
                            if (mc == null)
                                break;
                            cfmt.leftmargin = mc.lm;
                            cfmt.rightmargin = mc.rm;
                            img.chg = true;
                            break;
                    }
                }
                if (s.ts_next == null)
                {
                    s = null;
                }
                else if (s.soln)
                {
                    continue;
                }
                else
                {
                    s.soln = false;
                    if (s.time == s2.time)
                        continue;
                    while (!s.seqst)
                        s = s.ts_prev;
                }
                set_page();
                lwidth = get_lwidth() - lsh[1] - ckw[0];
                s2 = set_lines(s2, s, lwidth, indent);
                if (s2 == null)
                    break;
                s = s2.type == C.BLOCK ? s2.ts_prev : s;
                indent = 0;
            }

            cfmt.leftmargin = pg_sav.leftmargin;
            cfmt.rightmargin = pg_sav.rightmargin;
            cfmt.pagewidth = pg_sav.pagewidth;
            cfmt.scale = pg_sav.scale;
            img.chg = 1;
            set_page();
        }

        /* -- set the y values of some symbols -- */
        /* -- 設定某些符號的 y 值 -- */
        static void set_yval(object s)
        {
            switch (s.type)
            {
                case C.CLEF:
                    if (s.second || s.invis)
                    {
                        s.ymx = s.ymn = 12;
                        break;
                    }
                    s.y = (s.clef_line - 1) * 6;
                    switch (s.clef_type)
                    {
                        default:
                            s.ymx = s.y + 28;
                            s.ymn = s.y - 14;
                            break;
                        case "c":
                            s.ymx = s.y + 13;
                            s.ymn = s.y - 11;
                            break;
                        case "b":
                            s.ymx = s.y + 7;
                            s.ymn = s.y - 12;
                            break;
                    }
                    if (s.clef_small)
                    {
                        s.ymx -= 2;
                        s.ymn += 2;
                    }
                    if (s.ymx < 26)
                        s.ymx = 26;
                    if (s.ymn > -1)
                        s.ymn = -1;
                    if (s.clef_octave)
                    {
                        if (s.clef_octave > 0)
                            s.ymx += 12;
                        else
                            s.ymn -= 12;
                    }
                    break;
                case C.KEY:
                    if (s.k_sf > 2)
                        s.ymx = 24 + 10;
                    else if (s.k_sf > 0)
                        s.ymx = 24 + 6;
                    else
                        s.ymx = 24 + 2;
                    s.ymn = -2;
                    break;
                default:
                    s.ymx = 24;
                    s.ymn = 0;
                    break;
            }
        }

        // set the pitch of the notes under an ottava sequence
        // 設定 ottava 序列下音符的音高
        static void set_ottava()
        {
            VoiceItem s, s1;
            object st;
            int  o, d, m = nstaff + 1;
            var staff_d = new int[m];

            void sym_ott(VoiceItem s, int d)
            {
                VoiceItem g;
                int m;
                NoteItem note;

                switch (s.type)
                {
                    case C.REST:
                        if (voice_tb.Length == 1)
                            break;
                    case C.NOTE:
                        if (!s.p_v.ckey.k_drum)
                        {
                            for (m = s.nhd; m >= 0; m--)
                            {
                                note = s.notes[m];
                                if (note.opit == null)
                                    note.opit = note.pit;
                                note.pit += d;
                            }
                        }
                        break;
                    case C.GRACE:
                        for (g = s.extra; g != null; g = g.next)
                        {
                            if (!s.p_v.ckey.k_drum)
                            {
                                for (m = 0; m <= g.nhd; m++)
                                {
                                    note = g.notes[m];
                                    if (note.opit == 0)
                                        note.opit = note.pit;
                                    note.pit += d;
                                }
                            }
                        }
                        break;
                }
            }

            void deco_rm(VoiceItem s)
            {
                for (int i = s.a_dd.Count; --i >= 0;)
                {
                    if (s.a_dd[i].name.Match(/ 1?[85][vm][ab] /))
                        s.a_dd.splice(i, 1);
                }
            }

            for (s = tsfirst; s != null; s = s.ts_next)
            {
                st = s.st;
                o = s.ottava;
                if (o != null)
                {
                    if (o[0])
                    {
                        if (staff_d[st] != 0 && !o[1])
                        {
                            sym_ott(s, staff_d[st]);
                            deco_rm(s);
                            continue;
                        }
                    }
                    else if (staff_d[st] == 0)
                    {
                        deco_rm(s);
                        continue;
                    }
                    s1 = s;
                    while (!s1.seqst)
                        s1 = s1.ts_prev;
                    if (s1 != null)
                    {
                        while (s1 != s)
                        {
                            if (s1.st == st)
                            {
                                if (o[1])
                                    sym_ott(s1, -staff_d[st]);
                                if (o[0])
                                    sym_ott(s1, -o[0] * 7);
                            }
                            s1 = s1.ts_next;
                        }
                    }
                    if (o[0])
                    {
                        staff_d[st] = -o[0] * 7;
                    }
                    else
                    {
                        staff_d[st] = 0;
                    }
                }
                if (staff_d[st] != 0)
                    sym_ott(s, staff_d[st]);
            }
        }

        // expand the multi-rests as needed
        // 依需要擴展多休息
        static void mrest_expand()
        {
            object s, s2;

            void mexp(object s)
            {
                object bar, s3, s4, tim, nbar, nb = s.nmes, dur = s.dur / nb, s2 = s.next;

                while (!s2.bar_type)
                    s2 = s2.next;
                bar = s2;
                while (!s2.bar_num)
                    s2 = s2.ts_prev;
                nbar = s2.bar_num - s.nmes;

                s.type = C.REST;
                s.notes[0].dur = s.dur = s.dur_orig = dur;
                s.nflags = -2;
                s.head = C.FULL;
                s.fmr = 1;

                tim = s.time + dur;
                s3 = s;
                while (--nb > 0)
                {
                    s2 = clone(bar);
                    delete s2.soln;
                    delete s2.a_gch;
                    delete s2.a_dd;
                    delete s2.text;
                    delete s2.rbstart;
                    delete s2.rbstop;
                    lkvsym(s2, s.next);

                    s2.time = tim;
                    while (s3.time < tim)
                        s3 = s3.ts_next;
                    while (s3 != null && s3.v < s.v && s3.type == C.BAR)
                        s3 = s3.ts_next;
                    if (s3 != null)
                    {
                        if (s3.bar_type)
                            s3.seqst = false;
                        lktsym(s2, s3);
                        if (s3.type == C.BAR)
                            delete s3.bar_num;
                    }
                    else
                    {
                        s3 = s;
                        while (s3.ts_next != null)
                            s3 = s3.ts_next;
                        s3.ts_next = s2;
                        s2.ts_prev = s3;
                        s2.ts_next = null;
                    }
                    nbar++;
                    if (s2.seqst)
                    {
                        s2.bar_num = nbar;
                        s4 = s2.ts_next;
                    }
                    else
                    {
                        delete s2.bar_num;
                        s4 = s2.ts_prev;
                    }
                    s2.bar_type = s4.bar_type || "|";
                    if (s4.bar_num && !s4.seqst)
                        delete s4.bar_num;

                    s4 = clone(s);
                    delete s4.a_dd;
                    delete s4.soln;
                    delete s4.a_gch;
                    delete s4.part;
                    if (s2.next != null)
                    {
                        s4.next = s2.next;
                        s4.next.prev = s4;
                    }
                    else
                    {
                        s4.next = null;
                    }
                    s2.next = s4;
                    s4.prev = s2;
                    s4.time = tim;

                    while (s3 != null && !s3.dur && s3.time == tim)
                        s3 = s3.ts_next;
                    while (s3 != null && s3.v < s.v)
                    {
                        s3 = s3.ts_next;
                        if (s3 != null && s3.seqst)
                            break;
                    }
                    if (s3 != null)
                    {
                        if (s3.dur)
                            s3.seqst = false;
                        lktsym(s4, s3);
                    }
                    else
                    {
                        s3 = s;
                        while (s3.ts_next != null)
                            s3 = s3.ts_next;
                        s3.ts_next = s4;
                        s4.ts_prev = s3;
                        s4.ts_next = null;
                    }

                    tim += dur;
                    s = s3 = s4;
                }
            }

            for (s = tsfirst; s != null; s = s.ts_next)
            {
                if (s.type != C.MREST)
                    continue;
                if (!s.seqst && w_tb[s.ts_prev.type])
                {
                    s2 = s;
                }
                else
                {
                    s2 = s.ts_next;
                    while (!s2.seqst)
                    {
                        if (s2.type != C.MREST || s2.nmes != s.nmes)
                            break;
                        s2 = s2.ts_next;
                    }
                }
                if (!s2.seqst)
                {
                    while (s.type == C.MREST)
                    {
                        mexp(s);
                        s = s.ts_next;
                    }
                }
                else
                {
                    s = s2.ts_prev;
                }
            }
        }

        /**
         * set the clefs (treble or bass) in a 'auto clef' sequence
        *  return the starting clef type
         *  在「自動譜號」序列中設定譜號（高音或低音）
         *  傳回起始譜號類型
         */
        static void set_auto_clef(object st, object s_start, object clef_type_start)
        {
            object s, time, s2, s3, max = 12, min = 20;

            for (s = s_start; s != null; s = s.ts_next)
            {
                if (s.type == C.STAVES && s != s_start)
                    break;
                if (s.st != st)
                    continue;
                if (s.type != C.NOTE)
                {
                    if (s.type == C.CLEF)
                    {
                        if (s.clef_type != 'a')
                            break;
                        unlksym(s);
                    }
                    continue;
                }
                if (s.notes[0].pit < min)
                    min = s.notes[0].pit;
                if (s.notes[s.nhd].pit > max)
                    max = s.notes[s.nhd].pit;
            }

            if (min >= 19 || (min >= 13 && clef_type_start != 'b'))
                return 't';
            if (max <= 13 || (max <= 19 && clef_type_start != 't'))
                return 'b';

            if (clef_type_start == 'a')
            {
                if ((max + min) / 2 >= 16)
                    clef_type_start = 't';
                else
                    clef_type_start = 'b';
            }
            var clef_type = clef_type_start;
            var s_last = s;
            var s_last_chg = null;
            for (s = s_start; s != s_last; s = s.ts_next)
            {
                if (s.type == C.STAVES && s != s_start)
                    break;
                if (s.st != st || s.type != C.NOTE)
                    continue;

                time = s.time;
                if (clef_type == 't')
                {
                    if (s.notes[0].pit > 12 || s.notes[s.nhd].pit > 20)
                    {
                        if (s.notes[0].pit > 20)
                            s_last_chg = s;
                        continue;
                    }
                    s2 = s.ts_prev;
                    if (s2 != null && s2.time == time && s2.st == st && s2.type == C.NOTE && s2.notes[0].pit >= 19)
                        continue;
                    s2 = s.ts_next;
                    if (s2 != null && s2.st == st && s2.time == time && s2.type == C.NOTE && s2.notes[0].pit >= 19)
                        continue;
                }
                else
                {
                    if (s.notes[0].pit <= 12 || s.notes[s.nhd].pit < 20)
                    {
                        if (s.notes[s.nhd].pit <= 12)
                            s_last_chg = s;
                        continue;
                    }
                    s2 = s.ts_prev;
                    if (s2 != null && s2.time == time && s2.st == st && s2.type == C.NOTE && s2.notes[0].pit <= 13)
                        continue;
                    s2 = s.ts_next;
                    if (s2 != null && s2.st == st && s2.time == time && s2.type == C.NOTE && s2.notes[0].pit <= 13)
                        continue;
                }

                if (s_last_chg == null)
                {
                    clef_type = clef_type_start = clef_type == 't' ? 'b' : 't';
                    s_last_chg = s;
                    continue;
                }

                s3 = s;
                for (s2 = s.ts_prev; s2 != s_last_chg; s2 = s2.ts_prev)
                {
                    if (s2.st != st)
                        continue;
                    if (s2.type == C.BAR)
                    {
                        s3 = s2.bar_type[0] != ':' ? s2 : s2.next;
                        break;
                    }
                    if (s2.type != C.NOTE)
                        continue;
                    /* have a 2nd choice on beam start */
                    if (s2.beam_st
                        && !s2.p_v.second)
                        s3 = s2;
                }
                /* no change possible if no insert point */
                if (s3.time == s_last_chg.time)
                {
                    s_last_chg = s;
                    continue;
                }
                s_last_chg = s;

                /* insert a clef change */
                clef_type = clef_type == 't' ? 'b' : 't';
                s2 = insert_clef(s3, clef_type, clef_type == "t" ? 2 : 4);
                s2.clef_auto = true;
                //		s3.prev.st = st    
            }
            return clef_type_start
        }

        /* set the clefs */
        /* this function is called once at start of tune generation */
        /*
         * global variables:
         *	- staff_tb[st].clef = clefs at start of line (here, start of tune)
         *				(created here, updated on clef draw)
         *	- voice_tb[v].clef = clefs at end of generation
         *				(created on voice creation, updated here)
         */
        /* 設定譜號 */
        /* 此函數在曲調生成開始時呼叫一次 */
        /*
          * 全域變數：
          * - Staff_tb[st].clef = 行首的譜號（這裡是樂曲的開始）
          *（在此處創建，在譜號繪製上更新）
          * - voice_tb[v].clef = 產生結束時的譜號
          *（在語音創建時創建，此處更新）
          */
        public void set_clefs()
        {
            VoiceItem s, s2;
            int st, v, p_voice, g, new_type, new_line,  pit;
            Staff p_staff;
            dynamic staff_clef = new dynamic[nstaff + 1];
            dynamic sy = cur_sy;
            dynamic mid = new dynamic[sy.nstaff + 1];

            staff_tb = new dynamic[nstaff + 1];
            for (st = 0; st <= nstaff; st++)
            {
                staff_clef[st] = new { autoclef = true };
                staff_tb[st] = new { output = "", sc_out = "" };
            }

            for (st = 0; st <= sy.nstaff; st++)
                mid[st] = (sy.staves[st].stafflines.Length - 1) * 3;

            for (s = tsfirst; s != null; s = s.ts_next)
            {
                if (s.repeat_n)
                    SetRepeat(s);

                switch (s.type)
                {
                    case C.STAVES:
                        sy = s.sy;
                        for (st = 0; st <= nstaff; st++)
                            staff_clef[st].autoclef = true;
                        for (v = 0; v < voice_tb.Length; v++)
                        {
                            if (sy.voices[v] == null)
                                continue;
                            p_voice = voice_tb[v];
                            st = sy.voices[v].st;
                            if (!sy.voices[v].second)
                            {
                                sy.staves[st].staffnonote = p_voice.staffnonote;
                                if (p_voice.staffscale != null)
                                    sy.staves[st].staffscale = p_voice.staffscale;
                                if (sy.voices[v].sep != null)
                                    sy.staves[st].sep = sy.voices[v].sep;
                                if (sy.voices[v].maxsep != null)
                                    sy.staves[st].maxsep = sy.voices[v].maxsep;
                            }
                            s2 = p_voice.clef;
                            if (!s2.clef_auto)
                                staff_clef[st].autoclef = false;
                        }
                        for (st = 0; st <= sy.nstaff; st++)
                            mid[st] = (sy.staves[st].stafflines.Length - 1) * 3;
                        for (v = 0; v < voice_tb.Length; v++)
                        {
                            if (sy.voices[v] == null || sy.voices[v].second)
                                continue;
                            p_voice = voice_tb[v];
                            st = sy.voices[v].st;
                            s2 = p_voice.clef;
                            if (s2.clef_auto)
                            {
                                new_type = SetAutoClef(st, s, staff_clef[st].clef != null ? staff_clef[st].clef.clef_type : 'a');
                                new_line = new_type == 't' ? 2 : 4;
                            }
                            else
                            {
                                new_type = s2.clef_type;
                                new_line = s2.clef_line;
                            }
                            if (staff_clef[st].clef == null)
                            {
                                if (s2.clef_auto)
                                {
                                    if (s2.clef_type != 'a')
                                        p_voice.clef = Clone(p_voice.clef);
                                    p_voice.clef.clef_type = new_type;
                                    p_voice.clef.clef_line = new_line;
                                }
                                staff_tb[st].clef = staff_clef[st].clef = p_voice.clef;
                                continue;
                            }
                            if (new_type == staff_clef[st].clef.clef_type && new_line == staff_clef[st].clef.clef_line)
                                continue;
                            g = s.ts_prev;
                            while (g != null && g.time == s.time && (g.v != v || g.st != st))
                                g = g.ts_prev;
                            if (g == null || g.time != s.time)
                            {
                                g = s.ts_next;
                                while (g != null && (g.v != v || g.st != st))
                                    g = g.ts_next;
                                if (g == null || g.time != s.time)
                                    g = s;
                            }
                            if (g.type != C.CLEF)
                            {
                                g = InsertClef(g, new_type, new_line);
                                if (s2.clef_auto)
                                    g.clef_auto = true;
                            }
                            staff_clef[st].clef = p_voice.clef = g;
                        }
                        continue;
                    default:
                        s.mid = mid[s.st];
                        continue;
                    case C.CLEF:
                        break;
                }

                if (s.clef_type == 'a')
                {
                    s.clef_type = SetAutoClef(s.st, s.ts_next, staff_clef[s.st].clef != null ? staff_clef[s.st].clef.clef_type : 'a');
                    s.clef_line = s.clef_type == 't' ? 2 : 4;
                }

                p_voice = s.p_v;
                p_voice.clef = s;
                if (s.second)
                {
                    unlksym(s);
                    continue;
                }
                st = s.st;
                if (staff_clef[st].clef != null)
                {
                    if (s.clef_type == staff_clef[st].clef.clef_type && s.clef_line == staff_clef[st].clef.clef_line)
                    {
                        continue;
                    }
                }
                else
                {
                    staff_tb[st].clef = s;
                }
                staff_clef[st].clef = s;
            }

            sy = cur_sy;
            for (v = 0; v < voice_tb.Length; v++)
            {
                if (sy.voices[v] == null)
                    continue;
                s2 = voice_tb[v].sym;
                if (s2 == null || s2.notes[0].pit != 127)
                    continue;
                st = sy.voices[v].st;
                switch (staff_tb[st].clef.clef_type)
                {
                    default:
                        pit = 22;
                        break;
                    case "c":
                        pit = 16;
                        break;
                    case "b":
                        pit = 10;
                        break;
                }
                for (s = s2; s != null; s = s.next)
                    s.notes[0].pit = pit;
            }
        }

        /*****************************************************************/


        /* set the pitch of the notes according to the clefs
         * and set the vertical offset of the symbols */
        /* this function is called at start of tune generation and
         * then, once per music line up to the old sequence */
        /* 根據譜號設定音符的音高
          * 並設定符號的垂直偏移*/
        /* 該函數在曲調生成開始時調用，並且
          * 然後，每個音樂排列一次到舊序列 */
        public (int t,int c,int b,int p) delta_tb =
         (
           t : 0 - 2 * 2,
            c : 6 - 3 * 2,
            b : 12 - 4 * 2,
            p : 0 - 3 * 2
        ) ;

        /* upper and lower space needed by rests */
        /* 休息所需的上下空間 */
        int[,] rest_sp =
        {
    {18, 18},
    {12, 18},
    {12, 12},
    {6, 12},
    {6, 8},
    {10, 10},			/* crotchet */
    {6, 4},
    {10, 0},
    {10, 4},
    {10, 10}
};

        // (possible hook)
        //（可能的鉤子）
        public void set_pitch(dynamic last_s)
        {
            VoiceItem s, s2, g, st, delta, pitch, note;
            dynamic dur = C.BLEN;
            dynamic m = nstaff + 1;
            dynamic staff_delta = new int[m * 2];
            dynamic sy = cur_sy;

            for (st = 0; st <= nstaff; st++)
            {
                s = staff_tb[st].clef;
                staff_delta[st] = delta_tb[s.clef_type] + s.clef_line * 2;
                if (s.clefpit != null)
                    staff_delta[st] += s.clefpit;
                if (cfmt.sound)
                {
                    if (s.clef_octave != null && !s.clef_oct_transp)
                        staff_delta[st] += s.clef_octave;
                }
                else
                {
                    if (s.clef_oct_transp != null)
                        staff_delta[st] -= s.clef_octave;
                }
            }

            for (s = tsfirst; s != last_s; s = s.ts_next)
            {
                st = s.st;
                switch (s.type)
                {
                    case C.CLEF:
                        staff_delta[st] = delta_tb[s.clef_type] + s.clef_line * 2;
                        if (s.clefpit != null)
                            staff_delta[st] += s.clefpit;
                        if (cfmt.sound)
                        {
                            if (s.clef_octave != null && !s.clef_oct_transp)
                                staff_delta[st] += s.clef_octave;
                        }
                        else
                        {
                            if (s.clef_oct_transp != null)
                                staff_delta[st] -= s.clef_octave;
                        }
                        SetYval(s);
                        break;
                    case C.GRACE:
                        foreach (g in s.extra)
                        {
                            delta = staff_delta[g.st];
                            if (delta != null && !s.p_v.ckey.k_drum)
                            {
                                for (m = 0; m <= g.nhd; m++)
                                {
                                    note = g.notes[m];
                                    note.opit = note.pit;
                                    note.pit += delta;
                                }
                            }
                            g.ymn = 3 * (g.notes[0].pit - 18) - 2;
                            g.ymx = 3 * (g.notes[g.nhd].pit - 18) + 2;
                        }
                        SetYval(s);
                        break;
                    case C.KEY:
                        s.k_y_clef = staff_delta[st];
                        SetYval(s);
                        break;
                    default:
                        SetYval(s);
                        break;
                    case C.MREST:
                        if (s.invis)
                            break;
                        s.y = 12;
                        s.ymx = 24 + 15;
                        s.ymn = -2;
                        break;
                    case C.REST:
                        if (s.rep_nb > 1 || s.bar_mrep)
                        {
                            s.y = 12;
                            s.ymx = 38;
                            s.ymn = 0;
                            break;
                        }
                        if (voice_tb.Length == 1)
                        {
                            s.y = 12;
                            s.ymx = 24;
                            s.ymn = 0;
                            break;
                        }
                        goto case C.NOTE;
                    case C.NOTE:
                        delta = staff_delta[st];
                        if (delta != null && !s.p_v.ckey.k_drum)
                        {
                            for (m = s.nhd; m >= 0; m--)
                            {
                                note = s.notes[m];
                                note.opit = note.pit;
                                note.pit += delta;
                            }
                        }
                        if (s.type == C.REST)
                        {
                            s.y = (((s.notes[0].pit - 18) / 2) | 0) * 6;
                            s.ymx = s.y + rest_sp[5 - s.nflags][0];
                            s.ymn = s.y - rest_sp[5 - s.nflags][1];
                        }
                        if (s.dur < dur)
                            dur = s.dur;
                        break;
                }
            }
            if (last_s == null)
                smallest_duration = dur;
        }

        /* -- set the stem direction when multi-voices -- */
        /* this function is called only once per tune */
        // (possible hook)
        /* -- 設定多聲部時的詞幹方向 -- */
        /* 每個曲調只呼叫此函數一次 */
        //（可能的鉤子）
        public void set_stem_dir()
        {
            dynamic t, u, i, st, rvoice, v, v_st, st_v, vobj, v_st_tb, st_v_tb = new dynamic[nst + 1], s = tsfirst, sy = cur_sy, nst = sy.nstaff;

            while (s != null)
            {
                for (st = 0; st <= nst; st++)
                    st_v_tb[st] = new dynamic[0];
                v_st_tb = new dynamic[0];

                for (u = s; u != null; u = u.ts_next)
                {
                    if (u.type == C.BAR)
                        break;
                    if (u.type == C.STAVES)
                    {
                        if (u != s)
                            break;
                        sy = s.sy;
                        for (st = nst; st <= sy.nstaff; st++)
                            st_v_tb[st] = new dynamic[0];
                        nst = sy.nstaff;
                        continue;
                    }
                    if ((u.type != C.NOTE && u.type != C.REST) || u.invis)
                        continue;
                    st = u.st;
                    if (st > nst)
                    {
                        var msg = "*** fatal set_stem_dir(): bad staff number " + st + " max " + nst;
                        error(2, null, msg);
                        throw new Exception(msg);
                    }
                    v = u.v;
                    v_st = v_st_tb[v];
                    if (v_st == null)
                    {
                        v_st = new { st1 = -1, st2 = -1 };
                        v_st_tb[v] = v_st;
                    }
                    if (v_st.st1 < 0)
                    {
                        v_st.st1 = st;
                    }
                    else if (v_st.st1 != st)
                    {
                        if (st > v_st.st1)
                        {
                            if (st > v_st.st2)
                                v_st.st2 = st;
                        }
                        else
                        {
                            if (v_st.st1 > v_st.st2)
                                v_st.st2 = v_st.st1;
                            v_st.st1 = st;
                        }
                    }
                    st_v = st_v_tb[st];
                    rvoice = sy.voices[v].range;
                    for (i = st_v.Length; --i >= 0;)
                    {
                        vobj = st_v[i];
                        if (vobj.v == rvoice)
                            break;
                    }
                    if (i < 0)
                        continue;
                    if (i == st_v.Length - 1)
                    {
                        u.multi = -1;
                    }
                    else
                    {
                        u.multi = 1;
                        if (i != 0 && i + 2 == st_v.Length)
                        {
                            if (st_v[i].ymn - s.fmt.stemheight >= st_v[i + 1].ymx)
                                u.multi = -1;
                            t = s.ts_next;
                            if (s.ts_prev != null && s.ts_prev.time == s.time && s.ts_prev.st == s.st && s.notes[s.nhd].pit == s.ts_prev.notes[0].pit && s.beam_st != null && s.beam_end != null && (t == null || t.st != s.st || t.time != s.time))
                                u.multi = -1;
                        }
                    }
                }

                for (; s != u; s = s.ts_next)
                {
                    if (s.multi != null)
                        continue;
                    switch (s.type)
                    {
                        default:
                            continue;
                        case C.REST:
                            if ((s.combine != null && s.combine < 0) || s.ts_next == null || s.ts_next.type != C.REST || s.ts_next.st != s.st || s.time != s.ts_next.time || s.dur != s.ts_next.dur || (s.a_dd != null && s.ts_next.a_dd != null) || (s.a_gch != null && s.ts_next.a_gch != null) || s.invis)
                                break;
                            if (s.ts_next.a_dd != null)
                                s.a_dd = s.ts_next.a_dd;
                            if (s.ts_next.a_gch != null)
                                s.a_gch = s.ts_next.a_gch;
                            unlksym(s.ts_next);
                            break;
                        case C.NOTE:
                        case C.GRACE:
                            break;
                    }

                    st = s.st;
                    v = s.v;
                    v_st = v_st_tb[v];
                    st_v = st_v_tb[st];
                    if (v_st != null && v_st.st2 >= 0)
                    {
                        if (st == v_st.st1)
                            s.multi = -1;
                        else if (st == v_st.st2)
                            s.multi = 1;
                        continue;
                    }
                    if (st_v.Length <= 1)
                    {
                        if (s.floating)
                            s.multi = st == voice_tb[v].st ? -1 : 1;
                        continue;
                    }
                    rvoice = sy.voices[v].range;
                    for (i = st_v.Length; --i >= 0;)
                    {
                        if (st_v[i].v == rvoice)
                            break;
                    }
                    if (i < 0)
                        continue;
                    if (i == st_v.Length - 1)
                    {
                        s.multi = -1;
                    }
                    else
                    {
                        s.multi = 1;
                        if (i != 0 && i + 2 == st_v.Length)
                        {
                            if (st_v[i].ymn - s.fmt.stemheight >= st_v[i + 1].ymx)
                                s.multi = -1;
                            t = s.ts_next;
                            if (s.ts_prev != null && s.ts_prev.time == s.time && s.ts_prev.st == s.st && s.notes[s.nhd].pit == s.ts_prev.notes[0].pit && s.beam_st != null && s.beam_end != null && (t == null || t.st != s.st || t.time != s.time))
                                s.multi = -1;
                        }
                    }
                }
                while (s != null && s.type == C.BAR)
                    s = s.ts_next;
            }
        }



        /* -- adjust the offset of the rests when many voices -- */
        /* this function is called only once per tune */
        /* -- 多聲部時調整休止符的偏移量 -- */
        /* 每個曲調只呼叫此函數一次 */
        static void set_rest_offset()
        {
            int s, s2, v, end_time, not_alone, v_s, y, ymax, ymin,
                shift, dots, dx;
            int[] v_s_tb = new int[tsfirst];
            int sy = cur_sy;

            for (s = tsfirst; s != null; s = s.ts_next)
            {
                if (s.invis)
                    continue;
                if (s.type == C.STAVES)
                    sy = s.sy;
                if (s.dur == 0)
                    continue;
                v_s = v_s_tb[s.v];
                if (v_s == 0)
                {
                    v_s = new int();
                    v_s_tb[s.v] = v_s;
                }
                v_s.s = s;
                v_s.st = s.st;
                v_s.end_time = s.time + s.dur;
                if (s.type != C.REST)
                    continue;

                ymin = -127;
                ymax = 127;
                not_alone = dots = false;
                for (v = 0; v <= v_s_tb.Length; v++)
                {
                    v_s = v_s_tb[v];
                    if (v_s == 0 || v_s.s == null || v_s.st != s.st || v == s.v)
                        continue;
                    if (v_s.end_time <= s.time)
                        continue;
                    not_alone = true;
                    s2 = v_s.s;
                    if (sy.voices[v].range < sy.voices[s.v].range)
                    {
                        if (s2.time == s.time)
                        {
                            if (s2.ymn < ymax)
                            {
                                ymax = s2.ymn;
                                if (s2.dots)
                                    dots = true;
                            }
                        }
                        else
                        {
                            if (s2.y < ymax)
                                ymax = s2.y;
                        }
                    }
                    else
                    {
                        if (s2.time == s.time)
                        {
                            if (s2.ymx > ymin)
                            {
                                ymin = s2.ymx;
                                if (s2.dots)
                                    dots = true;
                            }
                        }
                        else
                        {
                            if (s2.y > ymin)
                                ymin = s2.y;
                        }
                    }
                }

                end_time = s.time + s.dur;
                for (s2 = s.ts_next; s2 != null; s2 = s2.ts_next)
                {
                    if (s2.time >= end_time)
                        break;
                    if (s2.st != s.st || !s2.dur || s2.invis)
                        continue;
                    not_alone = true;
                    if (sy.voices[s2.v].range < sy.voices[s.v].range)
                    {
                        if (s2.time == s.time)
                        {
                            if (s2.ymn < ymax)
                            {
                                ymax = s2.ymn;
                                if (s2.dots)
                                    dots = true;
                            }
                        }
                        else
                        {
                            if (s2.y < ymax)
                                ymax = s2.y;
                        }
                    }
                    else
                    {
                        if (s2.time == s.time)
                        {
                            if (s2.ymx > ymin)
                            {
                                ymin = s2.ymx;
                                if (s2.dots)
                                    dots = true;
                            }
                        }
                        else
                        {
                            if (s2.y > ymin)
                                ymin = s2.y;
                        }
                    }
                }
                if (!not_alone)
                {
                    s.y = 12;
                    s.ymx = 24;
                    s.ymn = 0;
                    continue;
                }
                if (ymax == 127 && s.y < 12)
                {
                    shift = 12 - s.y;
                    s.y += shift;
                    s.ymx += shift;
                    s.ymn += shift;
                }
                if (ymin == -127 && s.y > 12)
                {
                    shift = s.y - 12;
                    s.y -= shift;
                    s.ymx -= shift;
                    s.ymn -= shift;
                }
                shift = ymax - s.ymx;
                if (shift < 0)
                {
                    shift = (int)Math.Ceiling(-shift / 6) * 6;
                    if (s.ymn - shift >= ymin)
                    {
                        s.y -= shift;
                        s.ymx -= shift;
                        s.ymn -= shift;
                        continue;
                    }
                    dx = dots ? 15 : 10;
                    s.notes[0].shhd = dx;
                    s.xmx = dx;
                    continue;
                }
                shift = ymin - s.ymn;
                if (shift > 0)
                {
                    shift = (int)Math.Ceiling(shift / 6) * 6;
                    if (s.ymx + shift <= ymax)
                    {
                        s.y += shift;
                        s.ymx += shift;
                        s.ymn += shift;
                        continue;
                    }
                    dx = dots ? 15 : 10;
                    s.notes[0].shhd = dx;
                    s.xmx = dx;
                    continue;
                }
            }
        }


        /* -- create a starting symbol -- */
        // last_s = symbol at same time
        /* -- 建立一個起始符號 -- */
        // last_s = 同時符號
        public void NewSym(VoiceItem s, dynamic p_v, dynamic last_s)
        {
            s.p_v = p_v;
            s.v = p_v.v;
            s.st = p_v.st;
            s.time = last_s.time;

            if (p_v.last_sym != null)
            {
                s.next = p_v.last_sym.next;
                if (s.next != null)
                    s.next.prev = s;
                p_v.last_sym.next = s;
                s.prev = p_v.last_sym;
            }
            p_v.last_sym = s;

            lktsym(s, last_s);
        }



        /* -- init the symbols at start of a music line -- */
        /* -- 初始化音樂行開頭的符號 -- */
        static void init_music_line()
        {
            int p_voice, s, s1, s2, s3, last_s, v, st, shr, shrmx, shl,
                shlp, p_st, top,
                nv = voice_tb.Length,
                fmt = tsfirst.fmt;

            for (v = 0; v < nv; v++)
            {
                if (cur_sy.voices[v] == null)
                    continue;
                p_voice = voice_tb[v];
                p_voice.st = cur_sy.voices[v].st;
                p_voice.second = cur_sy.voices[v].second;
                p_voice.last_sym = p_voice.sym;

                for (s = p_voice.sym; s != null; s = s.next)
                {
                    if (s.type == C.CLEF || s.type == C.KEY || s.type == C.METER)
                    {
                        switch (s.type)
                        {
                            case C.CLEF:
                                staff_tb[s.st].clef = s;
                                break;
                            case C.KEY:
                                s.p_v.ckey = s;
                                break;
                            case C.METER:
                                s.p_v.meter = s;
                                insert_meter = cfmt.writefields.IndexOf('M') >= 0 && s.a_meter.Length > 0;
                                break;
                        }
                        if (s.part != null)
                            s.next.part = s.part;
                        unlksym(s);
                    }
                    else if (s.type == C.TEMPO || s.type == C.BLOCK || s.type == C.REMARK)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            last_s = tsfirst;
            for (v = 0; v < nv; v++)
            {
                p_voice = voice_tb[v];
                if (cur_sy.voices[v] == null || (cur_sy.voices[v].second && !p_voice.bar_start))
                    continue;
                st = cur_sy.voices[v].st;
                if (staff_tb[st] == null || staff_tb[st].clef == null)
                    continue;
                s = clone(staff_tb[st].clef);
                s.v = v;
                s.p_v = p_voice;
                s.st = st;
                s.time = tsfirst.time;
                s.prev = null;
                s.next = p_voice.sym;
                if (s.next != null)
                    s.next.prev = s;
                p_voice.sym = p_voice.last_sym = s;
                s.ts_next = last_s;
                if (last_s != null)
                    s.ts_prev = last_s.ts_prev;
                else
                    s.ts_prev = null;
                if (s.ts_prev == null)
                {
                    tsfirst = s;
                }
                else
                {
                    s.ts_prev.ts_next = s;
                    s.seqst = null;
                }
                if (last_s != null)
                    last_s.ts_prev = s;
                s.clef_small = null;
                s.part = null;
                s.second = cur_sy.voices[v].second;
                if (cur_sy.st_print[st] == null)
                    s.invis = true;
                else if (s.clef_none == null)
                    s.invis = null;
                s.fmt = fmt;
            }

            for (v = 0; v < nv; v++)
            {
                if (cur_sy.voices[v] == null || cur_sy.voices[v].second || cur_sy.st_print[cur_sy.voices[v].st] == null)
                    continue;
                p_voice = voice_tb[v];
                s2 = p_voice.ckey;
                if (s2.k_sf != null || s2.k_a_acc != null)
                {
                    s = clone(s2);
                    new_sym(s, p_voice, last_s);
                    s.invis = null;
                    s.part = null;
                    s.k_old_sf = s2.k_sf;
                    s.fmt = fmt;
                }
            }

            if (insert_meter)
            {
                for (v = 0; v < nv; v++)
                {
                    p_voice = voice_tb[v];
                    s2 = p_voice.meter;
                    if (cur_sy.voices[v] == null || cur_sy.voices[v].second || cur_sy.st_print[cur_sy.voices[v].st] == null)
                        continue;
                    s = clone(s2);
                    new_sym(s, p_voice, last_s);
                    s.part = null;
                    s.fmt = fmt;
                }
                insert_meter = false;
            }

            for (v = 0; v < nv; v++)
            {
                p_voice = voice_tb[v];
                if (p_voice.sls.Length > 0)
                {
                    s = new { type = C.BAR, fname = last_s.fname, bar_type = "|", dur = 0, multi = 0, invis = true, sls = p_voice.sls, fmt = fmt };
                    new_sym(s, p_voice, last_s);
                    p_voice.sls = new int[0];
                }
            }

            for (v = 0; v < nv; v++)
            {
                p_voice = voice_tb[v];
                s2 = p_voice.bar_start;
                p_voice.bar_start = null;
                for (s = last_s; s != null && s.time == last_s.time; s = s.ts_next)
                {
                    if (s.rbstop != null)
                    {
                        s2 = null;
                        break;
                    }
                }
                if (s2 != null)
                {
                    if (cur_sy.voices[v] == null || cur_sy.st_print[cur_sy.voices[v].st] == null)
                        continue;
                    if (p_voice.last_sym.type == C.BAR)
                    {
                        if (p_voice.last_sym.rbstop == null)
                            p_voice.last_sym.rbstart = 1;
                    }
                    else
                    {
                        new_sym(s2, p_voice, last_s);
                        s2.fmt = fmt;
                    }
                }
            }

            self.set_pitch(last_s);

            s = tsfirst;
            s.seqst = true;

            for (s = last_s; s.ts_next != null && s.ts_next.seqst == null; s = s.ts_next)
                ;
            if (s.ts_next != null && s.ts_next.type != C.CLEF && s.ts_next.a_ly == null)
                for (s = s.ts_next; s.ts_next != null && s.ts_next.seqst == null; s = s.ts_next)
                    ;
            s2 = s.ts_next;
            s.ts_next = null;
            set_allsymwidth();
            s.ts_next = s2;
        }


        // check if the tune ends on a measure bar
        // 檢查曲調是否在小節欄上結束
        public void check_end_bar()
        {
            dynamic s2, s = tsfirst;
            while (s.ts_next != null)
                s = s.ts_next;
            if (s.type != C.BAR)
            {
                s2 = _bar(s);
                s2.ts_prev = s;

                s.next = s.ts_next = s2;
            }
        }



        /*******************************************************/

        //double[] dx_tb = new double[] { 1.1f, 2.2f };
        //double[] hw_tb = new double[] { 1.1f, 2.2f };
        //double[] w_note = new double[] { 1.1f, 2.2f };

        //var delta_tb = new { t = 0, c = 6, b = 12, p = -2 };
        //var rest_sp = new int[][] { new int[] { 18, 18 }, new int[] { 12, 18 }, new int[] { 12, 12 } };
        //var delpit = new int[] { 0, -7, -14, 0 };




        /* -- set a pitch in all symbols and the start/stop of the beams -- */
        // and sort the pitches in the chords
        // and build the chord symbols / annotations
        // this function is called only once per tune
        /* -- 設定所有符號的間距以及光束的開始/停止 -- */
        //並對和弦中的音高進行排序
        // 並建構和弦符號/註釋
        // 每個曲調只呼叫此函數一次
        static void set_words(int p_voice)
        {
            int s2, n,
                s = p_voice.sym;

            while (s != null)
            {
                if (s.a_gch != null)
                    self.gch_build(s);
                switch (s.type)
                {
                    case C.MREST:
                        break;
                    case C.BAR:
                        var res = s.fmt.bardef[s.bar_type];
                        if (res != null)
                            s.bar_type = res;
                        if (!s.beam_on)
                            break;
                        if (s.next == null && s.prev != null && !s.invis && s.prev.head == C.OVALBARS)
                            s.prev.head = C.SQUARE;
                        break;
                    case C.GRACE:
                        for (s2 = s.extra; s2 != null; s2 = s2.next)
                        {
                            s2.notes.Sort(abc2svg.pitcmp);
                            var res = identify_note(s2, s2.dur_orig);
                            s2.head = res[0];
                            s2.dots = res[1];
                            s2.nflags = res[2];
                            if (s2.trem2 && (s2.next == null || s2.next.trem2))
                                trem_adj(s2);
                        }
                        break;
                    case C.NOTE:
                    case C.REST:
                        var res = identify_note(s, s.dur_orig);
                        s.head = res[0];
                        s.dots = res[1];
                        s.nflags = res[2];
                        if (s.nflags <= -2)
                            s.stemless = true;

                        if (s.xstem)
                            s.nflags = 0;
                        if (s.trem1)
                        {
                            if (s.nflags > 0)
                                s.nflags += s.ntrem;
                            else
                                s.nflags = s.ntrem;
                        }
                        if (s.next != null && s.next.trem2)
                            break;
                        if (s.trem2)
                        {
                            trem_adj(s);
                            break;
                        }

                        n = s.nflags;

                        if (s.ntrem != null)
                            n += s.ntrem;
                        if (s.type == C.REST && s.beam_end && !s.beam_on)
                        {
                            s.beam_end = false;
                            break;
                        }
                        if (start_flag || n <= 0)
                        {
                            if (lastnote != null)
                            {
                                lastnote.beam_end = true;
                                lastnote = null;
                            }
                            if (n <= 0)
                            {
                                s.beam_st = s.beam_end = true;
                            }
                            else if (s.type == C.NOTE || s.beam_on)
                            {
                                s.beam_st = true;
                                start_flag = false;
                            }
                        }
                        if (s.beam_end)
                            start_flag = true;
                        if (s.type == C.NOTE || s.beam_on)
                            lastnote = s;
                        break;
                }
                if (s.type == C.NOTE)
                {
                    if (s.nhd != null)
                        s.notes.Sort(abc2svg.pitcmp);
                    pitch = s.notes[0].pit;
                    for (s2 = s.prev; s2 != null; s2 = s2.prev)
                    {
                        if (s2.type != C.REST)
                            break;
                        s2.notes[0].pit = pitch;
                    }
                }
                else
                {
                    if (s.notes == null)
                    {
                        s.notes = new NoteItem[1];
                        s.notes[0] = new NoteItem();
                        s.nhd = 0;
                    }
                    s.notes[0].pit = pitch;
                }
                s = s.next;
            }
            if (lastnote != null)
                lastnote.beam_end = true;
        }

        /**
         * -- set the end of the repeat sequences --
         *  -- 設定重複序列的結尾 --  
         */
        static void set_rb(int p_voice)
        {
            int s2, n,
                s = p_voice.sym;

            while (s != null)
            {
                if (s.type != C.BAR || s.rbstart == null || s.norepbra != null)
                {
                    s = s.next;
                    continue;
                }
                n = 0;
                s2 = null;
                for (s = s.next; s != null; s = s.next)
                {
                    if (s.type != C.BAR)
                        continue;
                    if (s.rbstop != null)
                        break;
                    if (s.next == null)
                    {
                        s.rbstop = 2;
                        break;
                    }
                    n++;
                    if (n == s.fmt.rbmin)
                        s2 = s;
                    if (n == s.fmt.rbmax)
                    {
                        if (s2 != null)
                            s = s2;
                        s.rbstop = 1;
                        break;
                    }
                }
            }
        }

        /* -- initialize the generator -- */
        // this function is called only once per tune
        /* -- 初始化生成器 -- */
        // 每個曲調只呼叫此函數一次
        int[] delpit = new int[] { 0, -7, -14, 0 };

        static void set_global()
        {
            int p_voice, v,
                nv = voice_tb.Length,
                sy = cur_sy,
                st = sy.nstaff;

            insert_meter = cfmt.writefields.IndexOf('M') >= 0;

            while (true)
            {
                sy = sy.next;
                if (sy == null)
                    break;
                if (sy.nstaff > st)
                    st = sy.nstaff;
            }
            nstaff = st;

            check_end_bar();

            for (v = 0; v < nv; v++)
            {
                p_voice = voice_tb[v];
                set_words(p_voice);
                p_voice.ckey = p_voice.key;
                set_rb(p_voice);
            }

            if (nv > 1)
            {
                set_float();
                if (glovar.mrest_p)
                    mrest_expand();
            }

            if (glovar.ottava && cfmt.sound != "play")
                set_ottava();

            set_clefs();
            self.set_pitch(null);
        }

        // get the left offsets of the first and other staff systems
        // return [lsh1, lsho]
        // 取得第一個和其他五線譜系統的左偏移量
        // 返回 [lsh1, lsho]
        static int get_lshift()
        {
            int st, v, p_v, p1, po, fnt, w,
                sy = cur_sy,
                lsh1 = 0,
                lsho = 0,
                nv = voice_tb.Length;

            int get_wx(string p, int wx)
            {
                int w, j,
                    i = 0;

                p += '\n';
                while (true)
                {
                    j = p.IndexOf("\n", i);
                    if (j < 0)
                        break;
                    w = strwh(p.Substring(i, j - i))[0] + 12;
                    if (w > wx)
                        wx = w;
                    if (j < 0)
                        break;
                    i = j + 1;
                }
                return wx;
            }

            for (v = 0; v < nv; v++)
            {
                p_v = voice_tb[v];
                p1 = p_v.nm;
                po = p_v.snm;
                if ((p1 != null || po != null) && fnt == 0)
                {
                    set_font("voice");
                    fnt = gene.deffont;
                }
                if (p1 != null)
                {
                    w = get_wx(p1, lsh1);
                    if (w > lsh1)
                        lsh1 = w;
                }
                if (po != null)
                {
                    w = get_wx(po, lsho);
                    if (w > lsho)
                        lsho = w;
                }
            }
            w = 0;
            while (sy != null)
            {
                for (st = 0; st <= sy.nstaff; st++)
                {
                    if ((sy.staves[st].flags & (OPEN_BRACE2 | OPEN_BRACKET2)) != 0)
                    {
                        w = 12;
                        break;
                    }
                    if ((sy.staves[st].flags & (OPEN_BRACE | OPEN_BRACKET)) != 0)
                        w = 6;
                }
                if (w == 12)
                    break;
                sy = sy.next;
            }
            lsh1 += w;
            lsho += w;
            return vnt == 2 ? lsh1 : lsho;
        }

        /* -- return the left indentation of the staves -- */
        /* -- 傳回五線譜的左側縮排 -- */
        static int set_indent(int lsh)
        {
            int st, v, w, p_voice, p, i, j, font,
                vnt = 0,
                fmt = tsnext != null ? tsnext.fmt : cfmt;

            if (fmt.systnames)
            {
                for (v = voice_tb.Length - 1; v >= 0; v--)
                {
                    p_voice = voice_tb[v];
                    if (cur_sy.voices[v] == null || !gene.st_print[p_voice.st])
                        continue;
                    if (p_voice.nm != null && (p_voice.new_name || fmt.systnames == 2))
                    {
                        vnt = 2;
                        break;
                    }
                    if (p_voice.snm != null)
                        vnt = 1;
                }
            }
            gene.vnt = vnt;
            return vnt == 2 ? lsh[0] : lsh[1];
        }


        /* -- decide on beams and on stem directions -- */
        /* this routine is called only once per tune */
        /* -- 決定樑和莖方向 -- */
        /* 每首曲子只呼叫此例程一次 */
        public void set_beams(object sym)
        {
            object s, t, g, beam, s_opp, n, m, mid_p, pu, pd,
                laststem = -1;

            for (s = sym; s != null; s = s.next)
            {
                if (s.type != C.NOTE)
                {
                    if (s.type != C.GRACE)
                        continue;
                    g = s.extra;
                    if (g.stem == 2)
                    {
                        s_opp = s;
                        continue;
                    }
                    if (s.stem == null)
                        s.stem = s.multi ?? 1;
                    for (; g != null; g = g.next)
                    {
                        g.stem = s.stem;
                        g.multi = s.multi;
                    }
                    continue;
                }

                if (s.stem == null && s.multi != null)
                    s.stem = s.multi;
                if (s.stem == null)
                {
                    mid_p = s.mid / 3 + 18;

                    if (beam != null)
                    {
                        s.stem = laststem;
                    }
                    else if (s.beam_st != null && !s.beam_end)
                    {
                        beam = true;

                        pu = s.notes[s.nhd].pit;
                        pd = s.notes[0].pit;
                        for (g = s.next; g != null; g = g.next)
                        {
                            if (g.type != C.NOTE)
                                continue;
                            if (g.stem != null || g.multi != null)
                                s.stem = g.stem ?? g.multi;
                            if (g.notes[g.nhd].pit > pu)
                                pu = g.notes[g.nhd].pit;
                            if (g.notes[0].pit < pd)
                                pd = g.notes[0].pit;
                            if (g.beam_end)
                                break;
                        }
                        if (s.stem == null && g.beam_end != null)
                        {
                            if (pu + pd < mid_p * 2)
                            {
                                s.stem = 1;
                            }
                            else if (pu + pd > mid_p * 2)
                            {
                                s.stem = -1;
                            }
                            else
                            {
                                if (s.fmt.bstemdown)
                                    s.stem = -1;
                            }
                        }
                        if (s.stem == null)
                            s.stem = laststem;
                    }
                    else
                    {
                        n = (s.notes[s.nhd].pit + s.notes[0].pit) / 2;
                        if (n == mid_p && s.nhd > 1)
                        {
                            for (m = 0; m < s.nhd; m++)
                            {
                                if (s.notes[m].pit >= mid_p)
                                    break;
                            }
                            n = m * 2 < s.nhd ? mid_p - 1 : mid_p + 1;
                        }
                        if (n < mid_p)
                            s.stem = 1;
                        else if (n > mid_p || s.fmt.bstemdown)
                            s.stem = -1;
                        else
                            s.stem = laststem;
                    }
                }
                else
                {
                    if (s.beam_st != null && !s.beam_end)
                        beam = true;
                }
                if (s.beam_end != null)
                    beam = false;
                laststem = s.stem;

                if (s_opp != null)
                {
                    for (g = s_opp.extra; g != null; g = g.next)
                        g.stem = -laststem;
                    s_opp.stem = -laststem;
                    s_opp = null;
                }
            }
        }

        // check if there may be one head for unison when voice overlap
        // 檢查語音重疊時是否有一個頭可以齊聲
        public bool same_head(object s1, object s2)
        {
            object i1, i2, l1, l2, head, i11, i12, i21, i22, sh1, sh2,
                shu = s1.fmt.shiftunison ?? 0;

            if (shu >= 3)
                return false;
            if ((l1 = s1.dur) >= C.BLEN)
                return false;
            if ((l2 = s2.dur) >= C.BLEN)
                return false;
            if (s1.stemless != null && s2.stemless != null)
                return false;
            if (s1.dots != s2.dots)
            {
                if (shu & 1
                    || s1.dots * s2.dots != 0)
                    return false;
            }
            if (s1.stem * s2.stem > 0)
                return false;

            i1 = i2 = 0;
            if (s1.notes[0].pit > s2.notes[0].pit)
            {
                if (s1.stem < 0)
                    return false;
                while (s2.notes[i2].pit != s1.notes[0].pit)
                {
                    if (++i2 > s2.nhd)
                        return false;
                }
            }
            else if (s1.notes[0].pit < s2.notes[0].pit)
            {
                if (s2.stem < 0)
                    return false;
                while (s2.notes[0].pit != s1.notes[i1].pit)
                {
                    if (++i1 > s1.nhd)
                        return false;
                }
            }
            if (s2.notes[i2].acc != s1.notes[i1].acc)
                return false;
            i11 = i1;
            i21 = i2;
            sh1 = s1.notes[i1].shhd;
            sh2 = s2.notes[i2].shhd;
            do
            {
                i1++;
                i2++;
                if (i1 > s1.nhd)
                {
                    break;
                }
                if (i2 > s2.nhd)
                {
                    break;
                }
                if (s2.notes[i2].acc != s1.notes[i1].acc)
                    return false;
                if (sh1 < s1.notes[i1].shhd)
                    sh1 = s1.notes[i1].shhd;
                if (sh2 < s2.notes[i2].shhd)
                    sh2 = s2.notes[i2].shhd;
            } while (s2.notes[i2].pit == s1.notes[i1].pit);
            if (i1 <= s1.nhd)
            {
                if (i2 <= s2.nhd)
                    return false;
                if (s2.stem > 0)
                    return false;
            }
            else if (i2 <= s2.nhd)
            {
                if (s1.stem > 0)
                    return false;
            }
            i12 = i1;
            i22 = i2;

            head = 0;
            if (l1 != l2)
            {
                if (l1 < l2)
                {
                    l1 = l2;
                    l2 = s1.dur;
                }
                if (l1 < C.BLEN / 2)
                {
                    if (s2.dots != null)
                        head = 2;
                    else if (s1.dots != null)
                        head = 1;
                }
                else if (l2 < C.BLEN / 4)
                {
                    if (shu == 2)
                        return false;
                    head = s2.dur >= C.BLEN / 2 ? 2 : 1;
                }
                else
                {
                    return false;
                }
            }
            if (head == 0)
                head = s1.p_v.scale < s2.p_v.scale ? 2 : 1;
            if (head == 1)
            {
                for (i2 = i21; i2 < i22; i2++)
                {
                    s2.notes[i2].invis = true;
                    s2.notes[i2].acc = null;
                }
                for (i2 = 0; i2 <= s2.nhd; i2++)
                    s2.notes[i2].shhd += sh1;
            }
            else
            {
                for (i1 = i11; i1 < i12; i1++)
                {
                    s1.notes[i1].invis = true;
                    s1.notes[i1].acc = null;
                }
                for (i1 = 0; i1 <= s1.nhd; i1++)
                    s1.notes[i1].shhd += sh2;
            }
            return true;
        }

        /* handle unison with different accidentals */
        /* 處理不同記號的同音 */
        public void unison_acc(object s1, object s2, object i1, object i2)
        {
            object m, d, acc;

            acc = s2.notes[i2].acc;
            if (acc == null)
            {
                d = w_note[s2.head] * 2 + s2.xmx + s1.notes[i1].shac + 2;
                acc = s1.notes[i1].acc;
                if (acc is object)  // microtone
                    d += 2;
                if (s2.dots != null)
                    d += 6;
                for (m = 0; m <= s1.nhd; m++)
                {
                    s1.notes[m].shhd += d;
                    s1.notes[m].shac -= d;
                }
                s1.xmx += d;
            }
            else
            {
                d = w_note[s1.head] * 2 + s1.xmx + s2.notes[i2].shac + 2;
                if (acc is object)  // microtone
                    d += 2;
                if (s1.dots != null)
                    d += 6;
                for (m = 0; m <= s2.nhd; m++)
                {
                    s2.notes[m].shhd += d;
                    s2.notes[m].shac -= d;
                }
                s2.xmx += d;
            }
        }

        const int MAXPIT = 48 * 2;

        /* set the left space of a note/chord */
        /* 設定音符/和弦的左側空間 */
        public void set_left(Symbol s)
        {
            int m;
            int i;
            int j;
            int shift;
            int w_base = w_note[s.head];
            int w = w_base;
            int[] left = new int[MAXPIT];

            for (i = 0; i < MAXPIT; i++)
            {
                left[i] = -100;
            }

            if (s.nflags > -2)
            {
                if (s.stem > 0)
                {
                    w = -w;
                    i = s.notes[0].pit * 2;
                    j = (Math.Ceiling((s.ymx - 2) / 3) + 18) * 2;
                }
                else
                {
                    i = (Math.Ceiling((s.ymn + 2) / 3) + 18) * 2;
                    j = s.notes[s.nhd].pit * 2;
                }
                if (i < 0)
                {
                    i = 0;
                }
                if (j >= MAXPIT)
                {
                    j = MAXPIT - 1;
                }
                while (i <= j)
                {
                    left[i++] = w;
                }
            }

            shift = s.notes[s.stem > 0 ? 0 : s.nhd].shhd;
            for (m = 0; m <= s.nhd; m++)
            {
                w = -s.notes[m].shhd + w_base + shift;
                i = s.notes[m].pit * 2;
                if (i < 0)
                {
                    i = 0;
                }
                else if (i >= MAXPIT - 1)
                {
                    i = MAXPIT - 2;
                }
                if (w > left[i])
                {
                    left[i] = w;
                }
                if (s.head != C.SQUARE)
                {
                    w -= 1;
                }
                if (w > left[i - 1])
                {
                    left[i - 1] = w;
                }
                if (w > left[i + 1])
                {
                    left[i + 1] = w;
                }
            }

            return left;
        }

        /* set the right space of a note/chord */
        /* 設定音符/和弦的右側空間 */
        public void set_right(Symbol s)
        {
            int m;
            int i;
            int j;
            int k;
            int shift;
            int w_base = w_note[s.head];
            int w = w_base;
            bool flags = s.nflags > 0 && s.beam_st && s.beam_end;
            int[] right = new int[MAXPIT];

            for (i = 0; i < MAXPIT; i++)
            {
                right[i] = -100;
            }

            if (s.nflags > -2)
            {
                if (s.stem < 0)
                {
                    w = -w;
                    i = (Math.Ceiling((s.ymn + 2) / 3) + 18) * 2;
                    j = s.notes[s.nhd].pit * 2;
                    k = i + 4;
                }
                else
                {
                    i = s.notes[0].pit * 2;
                    j = (Math.Ceiling((s.ymx - 2) / 3) + 18) * 2;
                }
                if (i < 0)
                {
                    i = 0;
                }
                if (j > MAXPIT)
                {
                    j = MAXPIT;
                }
                while (i < j)
                {
                    right[i++] = w;
                }
            }

            if (flags)
            {
                if (s.stem > 0)
                {
                    if (s.xmx == 0)
                    {
                        i = s.notes[s.nhd].pit * 2;
                    }
                    else
                    {
                        i = s.notes[0].pit * 2;
                    }
                    i += 4;
                    if (i < 0)
                    {
                        i = 0;
                    }
                    for (; i < MAXPIT && i <= j - 4; i++)
                    {
                        right[i] = 11;
                    }
                }
                else
                {
                    i = k;
                    if (i < 0)
                    {
                        i = 0;
                    }
                    for (; i < MAXPIT && i <= s.notes[0].pit * 2 - 4; i++)
                    {
                        right[i] = 3.5;
                    }
                }
            }

            shift = s.notes[s.stem > 0 ? 0 : s.nhd].shhd;
            for (m = 0; m <= s.nhd; m++)
            {
                w = s.notes[m].shhd + w_base - shift;
                i = s.notes[m].pit * 2;
                if (i < 0)
                {
                    i = 0;
                }
                else if (i >= MAXPIT - 1)
                {
                    i = MAXPIT - 2;
                }
                if (w > right[i])
                {
                    right[i] = w;
                }
                if (s.head != C.SQUARE)
                {
                    w -= 1;
                }
                if (w > right[i - 1])
                {
                    right[i - 1] = w;
                }
                if (w > right[i + 1])
                {
                    right[i + 1] = w;
                }
            }

            return right;
        }


        /* -- shift the notes horizontally when voices overlap -- */
        /* this routine is called only once per tune */
        /* -- 當聲音重疊時水平移動音符 -- */
        /* 每首曲子只呼叫此例程一次 */
        private void set_overlap()
        {
            VoiceItem s, s1, s2, s3;
            int i, i1, i2, m, sd, t, dp, d, d2, dr, dr2, left1, right1, left2, right2, right3, pl, pr;
            double dx;
            VoiceStavesSymbols sy = cur_sy;

            // invert the voices
            void v_invert()
            {
                s1 = s2;
                s2 = s;
                d = d2;
                pl = left1;
                pr = right1;
                dr2 = dr;
            }

            for (s = tsfirst; s != null; s = s.ts_next)
            {
                if (s.type != C.NOTE || s.invis)
                {
                    if (s.type == C.STAVES)
                    {
                        sy = s.sy;
                    }
                    continue;
                }

                /* treat the stem on two staves with different directions */
                if (s.xstem && s.ts_prev.stem < 0)
                {
                    for (m = 0; m <= s.nhd; m++)
                    {
                        s.notes[m].shhd -= 7;        // stem_xoff
                        s.notes[m].shac += 16;
                    }
                }

                /* search the next note at the same time on the same staff */
                s2 = s;
                while (true)
                {
                    s2 = s2.ts_next;
                    if (s2 == null)
                    {
                        break;
                    }
                    if (s2.time != s.time)
                    {
                        s2 = null;
                        break;
                    }
                    if (s2.type == C.NOTE && !s2.invis && s2.st == s.st)
                    {
                        break;
                    }
                }
                if (s2 == null)
                {
                    continue;
                }
                s1 = s;

                /* set the dot vertical offset */
                if (sy.voices[s1.v].range < sy.voices[s2.v].range)
                {
                    s2.dot_low = true;
                }
                else
                {
                    s1.dot_low = true;
                }

                /* no shift if no overlap */
                if (s1.ymn > s2.ymx || s1.ymx < s2.ymn)
                {
                    continue;
                }

                if (same_head(s1, s2))
                {
                    continue;
                }

                // special case when only a second and no dots
                if (!s1.dots && !s2.dots)
                {
                    if ((s1.stem > 0 && s2.stem < 0 && s1.notes[0].pit == s2.notes[s2.nhd].pit + 1) || (s1.stem < 0 && s2.stem > 0 && s1.notes[s1.nhd].pit + 1 == s2.notes[0].pit))
                    {
                        if (s1.stem < 0)
                        {
                            s1 = s2;
                            s2 = s;
                        }
                        d = s1.notes[0].shhd + 7;
                        for (m = 0; m <= s2.nhd; m++)    // shift the lower note(s)
                        {
                            s2.notes[m].shhd += d;
                        }
                        s2.xmx += d;
                        s1.xmx = s2.xmx;        // align the dots
                        continue;
                    }
                }

                /* compute the minimum space for 's1 s2' and 's2 s1' */
                right1 = set_right(s1);
                left2 = set_left(s2);

                s3 = s1.ts_prev;
                if (s3 != null && s3.time == s1.time && s3.st == s1.st && s3.type == C.NOTE && !s3.invis)
                {
                    right3 = set_right(s3);
                    for (i = 0; i < MAXPIT; i++)
                    {
                        if (right3[i] > right1[i])
                        {
                            right1[i] = right3[i];
                        }
                    }
                }
                else
                {
                    s3 = null;
                }
                d = -10;
                for (i = 0; i < MAXPIT; i++)
                {
                    if (left2[i] + right1[i] > d)
                    {
                        d = left2[i] + right1[i];
                    }
                }

                if (d < -3 && ((s2.notes[0].pit & 1) || !(s1.dots || s2.dots) || (!(s1.notes[s1.nhd].pit == s2.notes[0].pit + 2 && s1.dot_low) && !(s1.notes[s1.nhd].pit + 2 == s2.notes[0].pit && s2.dot_low))))
                {
                    continue;
                }

                right2 = set_right(s2);
                left1 = set_left(s1);
                if (s3 != null)
                {
                    right3 = set_left(s3);
                    for (i = 0; i < MAXPIT; i++)
                    {
                        if (right3[i] > left1[i])
                        {
                            left1[i] = right3[i];
                        }
                    }
                }
                d2 = dr = dr2 = -100;
                for (i = 0; i < MAXPIT; i++)
                {
                    if (left1[i] + right2[i] > d2)
                    {
                        d2 = left1[i] + right2[i];
                    }
                    if (right2[i] > dr2)
                    {
                        dr2 = right2[i];
                    }
                    if (right1[i] > dr)
                    {
                        dr = right1[i];
                    }
                }

                /* check for unison with different accidentals and clash of dots */
                t = 0;
                i1 = s1.nhd;
                i2 = s2.nhd;
                while (true)
                {
                    dp = s1.notes[i1].pit - s2.notes[i2].pit;
                    switch (dp)
                    {
                        case 2:
                            if (!(s1.notes[i1].pit & 1))
                            {
                                s1.dot_low = false;
                            }
                            break;
                        case 1:
                            if (s1.notes[i1].pit & 1)
                            {
                                s2.dot_low = true;
                            }
                            else
                            {
                                s1.dot_low = false;
                            }
                            break;
                        case 0:
                            if (s1.notes[i1].acc != s2.notes[i2].acc)
                            {
                                t = -1;
                                break;
                            }
                            if (s2.notes[i2].acc)
                            {
                                if (!s1.notes[i1].acc)
                                {
                                    s1.notes[i1].acc = s2.notes[i2].acc;
                                }
                                s2.notes[i2].acc = 0;
                            }
                            if (s1.dots && s2.dots && (s1.notes[i1].pit & 1))
                            {
                                t = 1;
                            }
                            break;
                        case -1:
                            if (s1.notes[i1].pit & 1)
                            {
                                s2.dot_low = false;
                            }
                            else
                            {
                                s1.dot_low = true;
                            }
                            break;
                        case -2:
                            if (!(s1.notes[i1].pit & 1))
                            {
                                s2.dot_low = false;
                            }
                            break;
                    }
                    if (t < 0)
                    {
                        break;
                    }
                    if (dp >= 0)
                    {
                        if (--i1 < 0)
                        {
                            break;
                        }
                    }
                    if (dp <= 0)
                    {
                        if (--i2 < 0)
                        {
                            break;
                        }
                    }
                }

                if (t < 0)
                {   /* unison and different accidentals */
                    unison_acc(s1, s2, i1, i2);
                    continue;
                }

                sd = 0;
                if (s1.dots)
                {
                    if (s2.dots)
                    {
                        if (!t)         /* if no dot clash */
                        {
                            sd = 1;     /* align the dots */
                        }
                    }
                    else
                    {
                        v_invert();     /* shift the first voice */
                    }
                }
                else if (s2.dots)
                {
                    if (d2 + dr < d + dr2)
                    {
                        sd = 1;     /* align the dots */
                    }
                }
                pl = left2;
                pr = right2;
                if (!s3 && d2 + dr < d + dr2)
                {
                    v_invert();
                }
                d += 3;
                if (d < 0)
                {
                    d = 0;         // (not return!)
                }

                /* handle the previous shift */
                m = s1.stem >= 0 ? 0 : s1.nhd;
                d += s1.notes[m].shhd;
                m = s2.stem >= 0 ? 0 : s2.nhd;
                d -= s2.notes[m].shhd;

                /*
                 * room for the dots
                 * - if the dots of v1 don't shift, adjust the shift of v2
                 * - otherwise, align the dots and shift them if clash
                 */
                if (s1.dots)
                {
                    dx = 7.7 + s1.xmx +        // x 1st dot
                        3.5 * s1.dots - 3.5 +    // x last dot
                        3;            // some space
                    if (!sd)
                    {
                        d2 = -100;
                        for (i1 = 0; i1 <= s1.nhd; i1++)
                        {
                            i = s1.notes[i1].pit;
                            if (!(i & 1))
                            {
                                if (!s1.dot_low)
                                {
                                    i++;
                                }
                                else
                                {
                                    i--;
                                }
                            }
                            i *= 2;
                            if (i < 1)
                            {
                                i = 1;
                            }
                            else if (i >= MAXPIT - 1)
                            {
                                i = MAXPIT - 2;
                            }
                            if (pl[i] > d2)
                            {
                                d2 = pl[i];
                            }
                            if (pl[i - 1] + 1 > d2)
                            {
                                d2 = pl[i - 1] + 1;
                            }
                            if (pl[i + 1] + 1 > d2)
                            {
                                d2 = pl[i + 1] + 1;
                            }
                        }
                        if (dx + d2 + 2 > d)
                        {
                            d = dx + d2 + 2;
                        }
                    }
                    else
                    {
                        if (dx < d + dr2 + s2.xmx)
                        {
                            d2 = 0;
                            for (i1 = 0; i1 <= s1.nhd; i1++)
                            {
                                i = s1.notes[i1].pit;
                                if (!(i & 1))
                                {
                                    if (!s1.dot_low)
                                    {
                                        i++;
                                    }
                                    else
                                    {
                                        i--;
                                    }
                                }
                                i *= 2;
                                if (i < 1)
                                {
                                    i = 1;
                                }
                                else if (i >= MAXPIT - 1)
                                {
                                    i = MAXPIT - 2;
                                }
                                if (pr[i] > d2)
                                {
                                    d2 = pr[i];
                                }
                                if (pr[i - 1] + 1 > d2)
                                {
                                    d2 = pr[i - 1] = 1;
                                }
                                if (pr[i + 1] + 1 > d2)
                                {
                                    d2 = pr[i + 1] + 1;
                                }
                            }
                            if (d2 > 4.5 && 7.7 + s1.xmx + 2 < d + d2 + s2.xmx)
                            {
                                s2.xmx = d2 + 3 - 7.7;
                            }
                        }
                    }
                }

                for (m = s2.nhd; m >= 0; m--)
                {
                    s2.notes[m].shhd += d;
                    //          if (s2.notes[m].acc
                    //           && s2.notes[m].pit < s1.notes[0].pit - 4)
                    //              s2.notes[m].shac -= d
                }
                s2.xmx += d;
                if (sd)
                {
                    s1.xmx = s2.xmx;        // align the dots
                }
            }
        }


        /* -- set the stem height -- */
        /* this routine is called only once per tune */
        // (possible hook)
        /* -- 設定莖高度 -- */
        /* 每首曲子只呼叫此例程一次 */
        //（可能的鉤子）
        void set_stems()
        {
            VoiceItem s, s2;
            double g, slen, nflags, ymin, ymax;
            double scale, ymn, ymx;

            for (s = tsfirst; s != null; s = s.ts_next)
            {
                if (s.type != C.NOTE)
                {
                    if (s.type != C.GRACE)
                        continue;
                    ymin = ymax = s.mid;
                    for (g = s.extra; g != null; g = g.next)
                    {
                        slen = GSTEM;
                        if (g.nflags > 1)
                            slen += 1.2 * (g.nflags - 1);
                        ymn = 3 * (g.notes[0].pit - 18);
                        ymx = 3 * (g.notes[g.nhd].pit - 18);
                        if (s.stem >= 0)
                        {
                            g.y = ymn;
                            g.ys = ymx + slen;
                            ymx = Math.Round(g.ys);
                        }
                        else
                        {
                            g.y = ymx;
                            g.ys = ymn - slen;
                            ymn = Math.Round(g.ys);
                        }
                        ymx += 4;
                        ymn -= 4;
                        if (ymn < ymin)
                            ymin = ymn;
                        else if (ymx > ymax)
                            ymax = ymx;
                        g.ymx = ymx;
                        g.ymn = ymn;
                    }
                    s.ymx = ymax;
                    s.ymn = ymin;
                    continue;
                }
                /* shift notes in chords (need stem direction to do this) */
                set_head_shift(s);

                /* if start or end of beam, adjust the number of flags
                 * with the other end */
                nflags = s.nflags;
                if (s.beam_st && !s.beam_end)
                {
                    if (s.feathered_beam)
                        nflags = ++s.nflags;
                    for (s2 = s.next; /*s2*/; s2 = s2.next)
                    {
                        if (s2.type == C.NOTE)
                        {
                            if (s.feathered_beam)
                                s2.nflags++;
                            if (s2.beam_end)
                                break;
                        }
                    }
                    /*			if (s2) */
                    if (s2 != null && s2.nflags > nflags)
                        nflags = s2.nflags;
                }
                else if (!s.beam_st && s.beam_end){
                    //fixme: keep the start of beam ?
                    for (s2 = s.prev; s2 != null; s2 = s2.prev)
                    {
                        if (s2.beam_st)
                            break;
                    }
                    if (s2 != null && s2.nflags > nflags)
                        nflags = s2.nflags;
                }

                /* set height of stem end */
                slen = s.fmt.stemheight;
                switch (nflags)
                {
                    case 2: slen += 0; break;
                    case 3: slen += 4; break;
                    case 4: slen += 8; break;
                    case 5: slen += 12; break;
                }
                if ((scale = s.p_v.scale) != 1)
                    slen *= (scale + 1) * 0.5;
                ymn = 3 * (s.notes[0].pit - 18);
                if (s.nhd > 0)
                {
                    slen -= 2;
                    ymx = 3 * (s.notes[s.nhd].pit - 18);
                }
                else
                {
                    ymx = ymn;
                }
                if (s.ntrem)
                    slen += 2 * s.ntrem; // tremolo
                if (s.stemless)
                {
                    if (s.stem >= 0)
                    {
                        s.y = ymn;
                        s.ys = ymx;
                    }
                    else
                    {
                        s.ys = ymn;
                        s.y = ymx;
                    }
                    s.ymx = ymx + 4;
                    s.ymn = ymn - 4;
                }
                else if (s.stem >= 0)
                {
                    if (s.notes[s.nhd].pit > 26 && (nflags <= 0 || !s.beam_st || !s.beam_end))
                    {
                        slen -= 2;
                        if (s.notes[s.nhd].pit > 28)
                            slen -= 2;
                    }
                    s.y = ymn;
                    if (s.notes[0].tie)
                        ymn -= 3;
                    s.ymn = ymn - 4;
                    s.ys = ymx + slen;
                    if (s.ys < s.mid)
                        s.ys = s.mid;
                    s.ymx = (s.ys + 2.5) | 0;
                }
                else
                {       /* stem down */
                    if (s.notes[0].pit < 18 && (nflags <= 0 || !s.beam_st || !s.beam_end))
                    {
                        slen -= 2;
                        if (s.notes[0].pit < 16)
                            slen -= 2;
                    }
                    s.ys = ymn - slen;
                    if (s.ys > s.mid)
                        s.ys = s.mid;
                    s.ymn = (s.ys - 2.5) | 0;
                    s.y = ymx;
                    /*fixme:the tie may be lower*/
                    if (s.notes[s.nhd].tie)
                        ymx += 3;
                    s.ymx = ymx + 4;
                }
            }
        }


        List<VoiceItem> blocks = new List<VoiceItem>(); // array of delayed block symbols

        /********************************************************/

        //private const int MAXPIT = 48 * 2;

        //private List<Symbol> voice_tb;
        //private Symbol tsfirst;
        //private int posy;
        //private int blkdiv;
        //private int img;
        //private int cfmt;
        //private int posx;


        // (possible hook)
        //（可能的鉤子）
        public void block_gen(VoiceItem s)
        {
            switch (s.subtype)
            {
                case "leftmargin":
                case "rightmargin":
                case "pagescale":
                case "pagewidth":
                case "scale":
                case "staffwidth":
                    SetFormat(s.subtype, s.param);
                    break;
                case "mc_start":
                    if (multicol)
                    {
                        error(1, s, "No end of the previous %%multicol");
                        break;
                    }
                    multicol = new
                    {
                        posy = posy,
                        maxy = posy,
                        lm = cfmt.leftmargin,
                        rm = cfmt.rightmargin,
                        w = cfmt.pagewidth,
                        sc = cfmt.scale
                    };
                    break;
                case "mc_new":
                    if (!multicol)
                    {
                        error(1, s, "%%multicol new without start");
                        break;
                    }
                    if (posy > multicol.maxy)
                    {
                        multicol.maxy = posy;
                    }
                    cfmt.leftmargin = multicol.lm;
                    cfmt.rightmargin = multicol.rm;
                    cfmt.pagewidth = multicol.w;
                    cfmt.scale = multicol.sc;
                    posy = multicol.posy;
                    img.chg = true;
                    break;
                case "mc_end":
                    if (!multicol)
                    {
                        error(1, s, "%%multicol end without start");
                        break;
                    }
                    if (posy < multicol.maxy)
                    {
                        posy = multicol.maxy;
                    }
                    cfmt.leftmargin = multicol.lm;
                    cfmt.rightmargin = multicol.rm;
                    cfmt.pagewidth = multicol.w;
                    cfmt.scale = multicol.sc;
                    multicol = null;
                    blk_flush();
                    img.chg = true;
                    break;
                case "ml":
                    blk_flush();
                    user.img_out(s.text);
                    break;
                case "newpage":
                    if (!user.page_format)
                    {
                        break;
                    }
                    blk_flush();
                    if (blkdiv < 0)
                    {
                        user.img_out("</div>");
                    }
                    blkdiv = 2;
                    break;
                case "sep":
                    SetPage();
                    vskip(s.sk1);
                    output += '<path class="stroke"\n\td="M';
                    out_sxsy((img.width - s.l) / 2 - img.lm, ' ', 0);
                    output += 'h' + s.l.toFixed(1) + '"/>\n';
                    vskip(s.sk2);
                    break;
                case "text":
                    SetFont(s.font);
                    UseFont(s.font);
                    WriteText(s.text, s.opt);
                    break;
                case "title":
                    WriteTitle(s.text, true);
                    break;
                case "vskip":
                    vskip(s.sk);
                    break;
            }
        }

        // -- move some symbols of an empty staff to the next one --
        // -- 將空五線譜的一些符號移到下一個 --
        public void sym_staff_move(int st)
        {
            for (var s = tsfirst; s != null; s = s.ts_next)
            {
                if (s.nl)
                {
                    break;
                }
                if (s.st == st && s.type != C.CLEF)
                {
                    s.st++;
                    if (s.type != C.TEMPO)
                    {
                        s.invis = true;
                    }
                }
            }
        }


        /* -- define the start and end of a piece of tune -- */
        /* tsnext becomes the beginning of the next line */
        /* -- 定義一段曲子的開始與結束 -- */
        /* tsnext 成為下一行的開頭 */
        public void set_piece()
        {
            VoiceItem s, last;
            int p_voice, st, v, nv, tmp, non_empty, non_empty_gl = new List<bool>(), sy = cur_sy;

            void reset_staff(st)
            {
                dynamic p_staff = staff_tb[st], sy_staff = sy.staves[st];

                if (p_staff == null)
                {
                    p_staff = staff_tb[st] = new { y = 0, stafflines = sy_staff.stafflines, staffscale = sy_staff.staffscale, ann_top = 0, ann_bot = 0 };
                }
                p_staff.y = 0;            // staff system not computed yet
                p_staff.stafflines = sy_staff.stafflines;
                p_staff.staffscale = sy_staff.staffscale;
                p_staff.ann_top = p_staff.ann_bot = 0;
            }

            // adjust the empty flag of brace systems
            void set_brace()
            {
                dynamic st, i, empty_fl, n = sy.staves.Length;

                // if a system brace has empty and non empty staves, keep all staves
                for (st = 0; st < n; st++)
                {
                    if (!(sy.staves[st].flags & (OPEN_BRACE | OPEN_BRACE2)))
                    {
                        continue;
                    }
                    empty_fl = 0;
                    i = st;
                    while (st < n)
                    {
                        empty_fl |= non_empty[st] ? 1 : 2;
                        if (sy.staves[st].flags & (CLOSE_BRACE | CLOSE_BRACE2))
                        {
                            break;
                        }
                        st++;
                    }
                    if (empty_fl == 3)
                    {   // if both empty and not empty staves
                        while (i <= st)
                        {
                            non_empty[i] = true;
                            non_empty_gl[i++] = true;
                        }
                    }
                }
            }

            // set the top and bottom of the staves
            void set_top_bot()
            {
                Staff p_staff;
                    int st,i, j, l;

                for (st = 0; st <= nstaff; st++)
                {
                    p_staff = staff_tb[st];

                    // ledger lines
                    // index = line number
                    // values = [x symbol, x start, x stop]
                    p_staff.hlu = new List<int[]>();    // above the staff
                    p_staff.hld = new List<int[]>();    // under the staff

                    l = p_staff.stafflines.Length;
                    p_staff.topbar = 6 * (l - 1);

                    for (i = 0; i < l - 1; i++)
                    {
                        switch (p_staff.stafflines[i])
                        {
                            case '.':
                            case '-':
                                continue;
                        }
                        break;
                    }
                    p_staff.botline = p_staff.botbar = i * 6;
                    if (i >= l - 2)
                    {       // 0, 1 or 2 lines
                        if (p_staff.stafflines[i] != '.')
                        {
                            p_staff.botbar -= 6;
                            p_staff.topbar += 6;
                        }
                        else
                        {       // no line: big bar
                            p_staff.botbar -= 12;
                            p_staff.topbar += 12;
                            continue;   // no helper line
                        }
                    }
                    if (!non_empty_gl[st])
                    {
                        continue;
                    }

                    // define the helper lines
                    p_staff.hll = 17 + i * 2;    // pitch of lowest note
                                                 // without helper line
                                                 // ('D' when standard staff)
                    p_staff.hlmap = new int[(l - i + 1) * 2 + 2];    // (bug android 4.0)
                    for (j = 1; i < l; i++, j += 2)
                    {
                        switch (p_staff.stafflines[i])
                        {
                            case '|':
                            case '[':
                                p_staff.hlmap[j - 1] = 1; // no helper line
                                p_staff.hlmap[j] = 1;
                                p_staff.hlmap[j + 1] = 1;
                                break;
                        }
                    }
                }
            }

            // remove the staff system at start of line
            if (tsfirst.type == C.STAVES)
            {
                s = tsfirst;
                tsfirst = tsfirst.ts_next;
                tsfirst.ts_prev = null;
                if (s.seqst)
                {
                    tsfirst.seqst = true;
                }
                s.p_v.sym = s.next;
                if (s.next)
                {
                    s.next.prev = null;
                }
            }

            /* reset the staves */
            nstaff = sy.nstaff;
            for (st = 0; st <= nstaff; st++)
            {
                reset_staff(st);
            }
            non_empty = new bool[nstaff + 1];

            /*
             * search the next end of line,
             * and mark the empty staves
             */
            for (s = tsfirst; s != null; s = s.ts_next)
            {
                if (s.nl)
                {
                    break;
                }
                switch (s.type)
                {
                    case C.STAVES:
                        set_brace();
                        sy.st_print = non_empty;
                        sy = s.sy;
                        while (nstaff < sy.nstaff)
                        {
                            reset_staff(++nstaff);
                        }
                        non_empty = new bool[nstaff + 1];
                        continue;

                    // the block symbols will be treated after music line generation
                    case C.BLOCK:
                        if (!s.play)
                        {
                            blocks.Add(s);
                            unlksym(s);
                        }
                        else if (s.ts_next && s.ts_next.shrink)
                        {
                            s.ts_next.shrink = 0;
                        }
                        continue;
                }
                st = s.st;
                if (st > nstaff)
                {
                    switch (s.type)
                    {
                        case C.CLEF:
                            staff_tb[st].clef = s;    // clef warning/change for new staff
                            break;
                        case C.KEY:
                            s.p_v.ckey = s;
                            break;
                        //useless ?
                        case C.METER:
                            s.p_v.meter = s;
                            break;
                    }
                    unlksym(s);
                    continue;
                }
                if (non_empty[st])
                {
                    continue;
                }
                switch (s.type)
                {
                    default:
                        continue;
                    case C.BAR:
                        if (s.bar_mrep || sy.staves[st].staffnonote > 1)
                        {
                            break;
                        }
                        continue;
                    case C.GRACE:
                        break;
                    case C.NOTE:
                    case C.REST:
                    case C.SPACE:
                    case C.MREST:
                        if (sy.staves[st].staffnonote > 1)
                        {
                            break;
                        }
                        if (s.invis)
                        {
                            continue;
                        }
                        if (sy.staves[st].staffnonote || s.type == C.NOTE)
                        {
                            break;
                        }
                        continue;
                }
                non_empty_gl[st] = non_empty[st] = true;
            }
            tsnext = s;

            /* set the last empty staves */
            /* 設定最後一個空五線譜 */
            set_brace();
            sy.st_print = non_empty;

            /* define the offsets of the measure bars */
            /* 定義測量條的偏移量 */
            set_top_bot();

            // move the symbols of the empty staves to the next staff
            // 將空五線譜的符號移到下��個五線譜
            for (st = 0; st < nstaff; st++)
            {
                if (!non_empty_gl[st])
                {
                    sym_staff_move(st);
                }
            }

            // set a null height if the last staff is empty
            // 如果最後一個員工為空，則設定一個空高度
            if (!non_empty_gl[nstaff])
            {
                staff_tb[nstaff].topbar = 0;
            }

            // if not the end of the tune, set the end of the music line
            // 如果不是曲子的結尾，則設定音樂線的結尾
            if (tsnext != null)
            {
                s = tsnext;
                s.nl = null;
                last = s.ts_prev;
                last.ts_next = null;

                // and the end of the voices
                nv = voice_tb.Length;
                for (v = 0; v < nv; v++)
                {
                    p_voice = voice_tb[v];
                    if (p_voice.sym != null && p_voice.sym.time <= tsnext.time)
                    {
                        for (s = last; s != null; s = s.ts_prev)
                        {
                            if (s.v == v)
                            {
                                p_voice.s_next = s.next;
                                s.next = null;
                                break;
                            }
                        }
                        if (s != null)
                        {
                            continue;
                        }
                    }
                    p_voice.s_next = p_voice.sym;
                    p_voice.sym = null;
                }
            }

            // initialize the music line
            init_music_line();

            // keep the array of the staves to be printed
            gene.st_print = non_empty_gl;
        }


        /* -- position the symbols along the staff -- */
        // (possible hook)
        /* -- 沿著五線譜放置符號 -- */
        //（可能的鉤子）
        public void set_sym_glue(double width)
        {
            VoiceItem s, g, ll, x, some_grace,
            spf,            // spacing factor
            xmin = 0,       // sigma shrink = minimum spacing 最小間距
            xx = 0,         // sigma natural spacing 自然間距
            xs = 0,         // sigma unexpandable elements with no space 沒有空間的不可擴充元素
            xse = 0;            // sigma unexpandable elements with space 帶空格的不可擴充元素
            /* calculate the whole space of the symbols */
            /* 計算符號的整個空間 */
            for (s = tsfirst; s != null; s = s.ts_next)
            {
                if (s.type == C.GRACE && !some_grace)
                {
                    some_grace = s;
                }
                if (s.seqst)
                {
                    xmin += s.shrink;
                    if (xmin > width)
                    {
                        error(1, s, "Line too much shrunk 線條收縮太多  $1 $2 $3", xmin.toFixed(1), xx.toFixed(1), width.toFixed(1));
                        break;
                    }
                    if (s.space)
                    {
                        if (s.space < s.shrink)
                        {
                            xse += s.shrink;
                            xx += s.shrink;
                        }
                        else
                        {
                            xx += s.space;
                        }
                    }
                    else
                    {
                        xs += s.shrink;
                    }
                }
            }
            // can occur when bar alone in a staff system
            if (!xx)
            {
                realwidth = 0;
                return;
            }
            // last line?
            ll = !tsnext        // yes
                || (tsnext.type == C.BLOCK  // no, but followed by %%command
                && !tsnext.play)
                || blocks.Count;    //	(abcm2ps compatibility)
                                    // strong shrink
            s = tsfirst;
            if (xmin >= width)
            {
                //		if (xmin > width)
                //			error(1, s, "Line too much shrunk $1 $2 $3",
                //				xmin.toFixed(1),
                //				xx.toFixed(1),
                //				width.toFixed(1))
                x = 0;
                for (; s != null; s = s.ts_next)
                {
                    if (s.seqst)
                    {
                        x += s.shrink;
                    }
                    s.x = x;
                }
                //		realwidth = width
                spf_last = 0;
            }
            else if ((ll && xx + xs > width * (1 - s.fmt.stretchlast)) || (!ll && (xx + xs > width || s.fmt.stretchstaff)))
            {
                if (xx == xse)            // if no space
                {
                    xx += 5;
                }
                for (var cnt = 4; --cnt >= 0;)
                {
                    spf = (width - xs - xse) / (xx - xse);
                    xx = 0;
                    xse = 0;
                    x = 0;
                    for (s = tsfirst; s != null; s = s.ts_next)
                    {
                        if (s.seqst)
                        {
                            if (s.space)
                            {
                                if (s.space * spf <= s.shrink)
                                {
                                    xse += s.shrink;
                                    xx += s.shrink;
                                    x += s.shrink;
                                }
                                else
                                {
                                    xx += s.space;
                                    x += s.space * spf;
                                }
                            }
                            else
                            {
                                x += s.shrink;
                            }
                        }
                        s.x = x;
                    }
                    if (Math.Abs(x - width) < 0.1)
                    {
                        break;
                    }
                }
                spf_last = spf;
            }
            else
            {            // shorter line
                spf = 1 - s.fmt.maxshrink;
                if (spf_last && xx * spf_last + xs < width)
                {
                    spf = spf_last;
                }
                x = 0;
                for (; s != null; s = s.ts_next)
                {
                    if (s.seqst)
                    {
                        x += s.space <= s.shrink ? s.shrink : s.shrink * (1 - spf) + s.space * spf;
                    }
                    s.x = x;
                }
            }
            realwidth = x;

            /* set the x offsets of the grace notes */
            for (s = some_grace; s != null; s = s.ts_next)
            {
                if (s.type != C.GRACE)
                {
                    continue;
                }
                if (s.gr_shift)
                {
                    x = s.prev.x + s.prev.wr;
                }
                else
                {
                    x = s.x - s.wl;
                }
                for (g = s.extra; g != null; g = g.next)
                {
                    g.x += x;
                }
            }
        }

        // set the starting symbols of the voices for the new music line
        // 設定新音樂線的聲音起始符號
        public void set_sym_line()
        {
            Symbol p_v;
            Symbol s;
            int v = voice_tb.Count;

            while (--v >= 0)
            {
                p_v = voice_tb[v];
                if (p_v.sym && p_v.s_prev)
                {
                    p_v.sym.prev = p_v.s_prev;
                    p_v.s_prev.next = p_v.sym;
                }
                s = p_v.s_next;
                p_v.s_next = null;
                p_v.sym = s;
                if (s != null)
                {
                    if (s.prev != null)
                    {
                        s.prev.next = s;
                    }
                    p_v.s_prev = s.prev;
                    s.prev = null;
                }
                else
                {
                    p_v.s_prev = null;
                }
            }
        }


        // set the left offset the images
        // 設定影像的左偏移量
        public void set_posx()
        {
            posx = img.lm / cfmt.scale;
        }

        // initialize the start of generation / new music line
        // and output the inter-staff blocks if any
        // 初始化產生開始/新音樂線
        // 並輸出內部人員區塊（如果有）
        public void gen_init()
        {
            Symbol s = tsfirst;
            int tim = s.time;

            for (; s != null; s = s.ts_next)
            {
                if (s.time != tim)
                {
                    SetPage();
                    return;
                }
                switch (s.type)
                {
                    case C.NOTE:
                    case C.REST:
                    case C.MREST:
                    case C.SPACE:
                        SetPage();
                        return;
                    default:
                        continue;
                    case C.STAVES:
                        cur_sy = s.sy;
                        continue;
                    case C.BLOCK:
                        if (s.play)
                        {
                            continue;
                        }
                        block_gen(s);
                        break;
                }
                unlksym(s);
                if (s.p_v.s_next == s)
                {
                    s.p_v.s_next = s.next;
                }
            }
            tsfirst = null;
        }


        /**************************************/

        //    public double[] dx_tb = new double[] { 1.1f, 2.2f };
        //    public double[] hw_tb = new double[] { 1.1f, 2.2f };
        //    public double[] w_note = new double[] { 1.1f, 2.2f };

        //    public Dictionary<string, int> delta_tb = new Dictionary<string, int>()
        //{
        //    { "t", 0 },
        //    { "c", 6 },
        //    { "b", 12 },
        //    { "p", -2 }
        //};

        //    public List<List<int>> rest_sp = new List<List<int>>()
        //{
        //    new List<int> { 18, 18 },
        //    new List<int> { 12, 18 },
        //    new List<int> { 12, 12 }
        //};

        //    public const int MAXPIT = 48 * 2;
        //    public List<object> blocks = new List<object>();

        //    public int slurePos = 0;


        /* -- generate the music -- */
        // (possible hook)
        /* -- 生成音樂 -- */
        //（可能的鉤子）
        public void output_music()
        {
            int v, lwidth, indent, lsh, line_height, ts1st, tslast, p_v;
            int nv = voice_tb.Length;

            SetGlobal();
            if (nv > 1)
                set_stem_dir();

            for (v = 0; v < nv; v++)
                SetBeams(voice_tb[v].sym);

            set_stems();

            SetAccShft();
            if (nv > 1)
            {
                SetRestOffset();
                SetOverlap();
            }
            SetAllSymWidth(1);

            lsh = GetLShift();

            if (cfmt.singleline)
            {
                var v = GetCkWidth();
                lwidth = lsh[0] + v[0] + v[1] + GetWidth(tsfirst, null)[0];
                v = cfmt.singleline == 2 ? GetLWidth() : lwidth;
                if (v > lwidth)
                    lwidth = v;
                else
                    img.width = lwidth * cfmt.scale + img.lm + img.rm + 2;
            }
            else
            {
                lwidth = GetLWidth();
                CutTune(lwidth, lsh);
            }

            gen_init();
            if (tsfirst == null)
                return;

            ts1st = tsfirst;
            v = nv;
            while (--v >= 0)
                voice_tb[v].osym = voice_tb[v].sym;

            spf_last = 0;

            set_all_sls();
            while (true)
            {
                SetPiece();
                indent = SetIndent(lsh);
                if (line_height == 0 && cfmt.indent != null && indent < cfmt.indent)
                    indent = cfmt.indent;
                SetSymGlue(lwidth - indent);
                if (realwidth)
                {
                    if (indent != 0)
                        posx += indent;

                    draw_sym_near();
                    line_height = set_staff();
                    DrawSystems(indent);
                    draw_all_sym();
                    DelayedUpdate();
                    if (output)
                        Vskip(line_height);
                    if (indent != 0)
                        posx -= indent;
                }
                blk_flush();
                while (blocks.Count > 0)
                    BlockGen(blocks.RemoveAt(0));
                if (tslast != null)
                    tslast.ts_next.ts_prev = tslast;
                if (tsnext == null)
                    break;
                tsnext.ts_prev.ts_next = tsfirst = tsnext;

                gen_init();
                if (tsfirst == null)
                    break;
                tslast = tsfirst.ts_prev;
                tsfirst.ts_prev = null;
                set_sym_line();
                lwidth = GetLWidth();
            }

            tsfirst = ts1st;
            v = nv;
            while (--v >= 0)
            {
                p_v = voice_tb[v];
                if (p_v.sym != null && p_v.s_prev != null)
                    p_v.sym.prev = p_v.s_prev;
                p_v.sym = p_v.osym;
            }
        }


        int slurePos = 0;

        public void set_all_sls()
        {
            VoiceItem s;
            int p_voice, v,
                n = voice_tb.Length;
            for (v = 0; v < n; v++)
            {
                p_voice = voice_tb[v];
                if (p_voice.sym != null)
                {
                    for (s = p_voice.sym; s != null; s = s.next)
                    {
                        if (s.sls != null)
                        {
                            foreach (SlurGroup the in s.sls)
                            {
                                VoiceItem sst = the.ss;
                                VoiceItem send = the.se;
                                slurePos++;
                                if (sst.slurStart == null) sst.slurStart = new List<int>();
                                sst.slurStart.Add(slurePos);
                                if (send.slurEnd == null) send.slurEnd = new List<int>();
                                send.slurEnd.Add(slurePos);
                            }
                        }
                    }
                }
            }
        }



    }

}

