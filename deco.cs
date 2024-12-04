using System;
using System.Collections.Generic;
using System.Text;

// abc2svg - deco.js - decorations
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

// Decoration objects
// dd {			// decoration definition (static)
//	dd_en,			// definition of the ending decoration
//	dd_st,			// definition of the starting decoration
//	func,			// function
//	glyph,			// glyph
//	h,			// height / ascent
//	hd,			// descent
//	inv,			// inverted glyph
//	name,			// name
//	str,			// string
//	wl,			// left width
//	wr,			// right width
// }
// de {			// decoration elements (in an array - one list per music line)
//	dd,			// definition of the decoration
//	defl {			// flags
//		noen,			// no end of this decoration
//		nost,			// no start of this decoration
//	},
//	has_val,		// defined value
//	ix,			// index of the decoration in the 'de' list
//	lden,			// end of a long decoration
//	ldst,			// start of a long decoration if true
//	m,			// note index when note decoration
//	prev,			// previous decoration (hack for 'tr~~~~~')
//	s,			// symbol
//	start,			// start of the decoration (in the ending element)
//	up,			// above the symbol
//	val,			// value
//	x,			// x offset
//	y,			// y offset
// }


namespace autocad_part2
{
    partial class Abc
    {


        public class Symbolxx
        {
            public int type;
            public int nhd;
            public int stem;
            public int head;
            public int x;
            public int y;
            public int ymx;
            public int ymn;
            public bool bar_dotted;
            public bool ottava;
            public bool beam_on;
            public bool trem2;
            public bool beam_end;
            public bool beam_st;
            public bool xstem;
            public bool beam_br1;
            public bool beam_br2;
            public bool rbstop;
            public bool trem1;
            public bool stemless;
            public int rbend;
            public List<NoteItem> notes;
            public List<int> ottava;
            public List<DecorationDef> a_dd;
            public int feathered_beam;
            public int rbstop;
            public int ntrem;
            public int rbend;
            public int[] acc;
            public string color;
            public bool invis;
        }



        public class Decorationxxx
        {
            public string name;
            public int func;
            public string glyph;
            public int h;
            public int hd;
            public double wl;
            public double wr;
            public int dx;
            public int dy;
            public string str;
            public string ty;
            public int x;
            public int y;
            public bool inv;
            public bool has_val;
            public int val;
            public Decoration dd_en;
            public Decoration dd_st;
        }

        public Dictionary<string, DecorationDef> dd_tb = new Dictionary<string, DecorationDef>();   // definition of the decorations
        public List<DecorationElement> a_de = new List<DecorationElement>();              // array of the decoration elements
        public Dictionary<string, NoteItem> cross = new Dictionary<string, NoteItem>();     // cross voice decorations
                                                                                     // decorations - populate with standard decorations
                                                                                     // 裝飾 - 用標準裝飾填充
        public static Dictionary<string, string> decos = new Dictionary<string, string>() {
        { "dot", "0 stc 6 1.5 1" },
        { "tenuto", "0 emb 6 4 3" },
        { "slide", "1 sld 3 7 1" },
        { "arpeggio", "2 arp 12 10 3" },
        { "roll", "3 roll 5,4 5 6" },
        { "lowermordent", "3 lmrd 6,5 4 6" },
        { "uppermordent", "3 umrd 6,5 4 6" },
        { "trill", "3 trl 14 5 8" },
        { "upbow", "3 upb 10,2 3 7" },
        { "downbow", "3 dnb 9 4 6" },
        { "gmark", "3 grm 7 4 6" },
        { "wedge", "0 wedge 8 1.5 1" },		// (staccatissimo or spiccato)
        { "longphrase", "5 lphr 0 1 16" },
        { "mediumphrase", "5 mphr 0 1 16" },
        { "shortphrase", "5 sphr 0 1 16" },
        { "turnx", "3 turnx 7,2.5 5 6" },
        { "invertedturn", "3 turn 7,2 5 6" },
        { "0", "3 fng 5,5 3 3 0" },
        { "1", "3 fng 5,5 3 3 1" },
        { "2", "3 fng 5,5 3 3 2" },
        { "3", "3 fng 5,5 3 3 3" },
        { "4", "3 fng 5,5 3 3 4" },
        { "5", "3 fng 5,5 3 3 5" },
        { "plus", "3 dplus 8,2 2 4" },
        { "+", "3 dplus 8,2 2 4" },
        { ">", "5 accent 3.5,3.5 4 4" },
        { "accent", "5 accent 3.5,3.5 4 4" },
        { "emphasis", "5 accent 3.5,3.5 4 4" },
        { "marcato", "3 marcato 9 5 5" },
        { "^", "3 marcato 9 5 5" },
        { "mordent", "3 lmrd 6,5 4 6" },
        { "open", "3 opend 8 3 3" },
        { "snap", "3 snap 10 3 3" },
        { "thumb", "3 thumb 10 3 3" },
        { "turn", "3 turn 7,2.5 5 6" },
        { "trill(", "5 ltr 8 0 0" },
        { "trill)", "5 ltr 8 0 0" },
        { "8va(", "5 8va 12 6 6" },
        { "8va)", "5 8va 12 6 6" },
        { "8vb(", "4 8vb 10,5 6 6" },
        { "8vb)", "4 8vb 10,5 6 6" },
        { "15ma(", "5 15ma 12 9 9" },
        { "15ma)", "5 15ma 12 9 9" },
        { "15mb(", "4 15mb 12 9 9" },
        { "15mb)", "4 15mb 12 9 9" },
        { "breath", "5 brth 0 1 16" },
        { "caesura", "5 caes 0 1 20" },
        { "short", "5 short 0 1 16" },
        { "tick", "5 tick 0 1 16" },
        { "coda", "5 coda 22,5 10 10" },
        { "dacapo", "5 dacs 16 20 20 Da Capo" },
        { "dacoda", "5 dacs 16 20 20 Da Coda" },
        { "D.C.", "5 dcap 16,3 12 12" },
        { "D.S.", "5 dsgn 16,3 12 12" },
        { "D.C.alcoda", "5 dacs 16 38 38 D.C. al Coda" },
        { "D.S.alcoda", "5 dacs 16 38 38 D.S. al Coda" },
        { "D.C.alfine", "5 dacs 16 38 38 D.C. al Fine" },
        { "D.S.alfine", "5 dacs 16 38 38 D.S. al Fine" },
        { "fermata", "5 hld 12 7.5 7.5" },
        { "fine", "5 dacs 16 14 14 Fine" },
        { "invertedfermata", "7 hld 12 8 8" },
        { "segno", "5 sgno 22,2 5 5" },
        { "f", "6 f 12,5 3 4" },
        { "ff", "6 ff 12,5 8 5" },
        { "fff", "6 fff 12,5 11 9" },
        { "ffff", "6 ffff 12,5 15 12" },
        { "mf", "6 mf 12,5 8 10" },
        { "mp", "6 mp 12,5 9 10" },
        { "p", "6 p 12,5 3 6" },
        { "pp", "6 pp 12,5 8 9" },
        { "ppp", "6 ppp 12,5 14 11" },
        { "pppp", "6 pppp 12,5 14 17" },
        { "pralltriller", "3 umrd 6,5 4 6" },
        { "sfz", "6 sfz 12,5 9 9" },
        { "ped", "7 ped 14 6 10" },
        { "ped-up", "7 pedoff 12 4 4" },
        { "ped(", "7 lped 14 1 1" },
        { "ped)", "7 lped 14 1 1" },
        { "crescendo(", "6 cresc 15,2 0 0" },
        { "crescendo)", "6 cresc 15,2 0 0" },
        { "<(", "6 cresc 15,2 0 0" },
        { "<)", "6 cresc 15,2 0 0" },
        { "diminuendo(", "6 dim 15,2 0 0" },
        { "diminuendo)", "6 dim 15,2 0 0" },
        { ">(", "6 dim 15,2 0 0" },
        { ">)", "6 dim 15,2 0 0" },
        { "-(", "8 gliss 0 0 0" },
        { "-)", "8 gliss 0 0 0" },
        { "~(", "8 glisq 0 0 0" },
        { "~)", "8 glisq 0 0 0" },
// internal
//	color: "10 0 0 0 0",
        { "invisible", "32 0 0 0 0" },
        { "beamon", "33 0 0 0 0" },
        { "trem1", "34 0 0 0 0" },
        { "trem2", "34 0 0 0 0" },
        { "trem3", "34 0 0 0 0" },
        { "trem4", "34 0 0 0 0" },
        { "xstem", "35 0 0 0 0" },
        { "beambr1", "36 0 0 0 0" },
        { "beambr2", "36 0 0 0 0" },
        { "rbstop", "37 0 0 0 0" },
        { "/", "38 0 0 6 6" },
        {"//", "38 0 0 6 6" },
        {"///", "38 0 0 6 6" },
        { "beam-accel", "39 0 0 0 0" },
        { "beam-rall", "39 0 0 0 0" },
        { "stemless", "40 0 0 0 0" },
        { "rbend", "41 0 0 0 0" },
        { "editorial", "42 0 0 0 0" },
        { "sacc-1", "3 sacc-1 6,4 4 4" },
        { "sacc3", "3 sacc3 6,5 4 4" },
        { "sacc1", "3 sacc1 6,4 4 4" },
        { "courtesy", "43 0 0 0 0" },
        { "cacc-1", "3 cacc-1 0 0 0" },
        { "cacc3", "3 cacc3 0 0 0" },
        { "cacc1", "3 cacc1 0 0 0" },
        { "tie(", "44 0 0 0 0" },
        { "tie)", "44 0 0 0 0" }
    };

        // types of decoration per function
        // 每個函數的裝飾類型
        public static Action<DecorationElement>[] f_near = new Action<DecorationElement>[]
    {
        d_near,		// 0 - near the note
		d_slide,	// 1
		d_arp		// 2
    };

        public static Action<DecorationElement>[] f_note = new Action<DecorationElement>[]
    {
        null, null, null,
        d_upstaff,	// 3 - tied to note
		d_upstaff	// 4 (below the staff)
    };

        public static Action<DecorationElement>[] f_staff = new Action<DecorationElement>[]
    {
        null, null, null, null, null,
        d_upstaff,	// 5 (above the staff)
		d_upstaff,	// 6 - tied to staff (dynamic marks)
		d_upstaff	// 7 (below the staff)
    };

        /* -- get the max/min vertical offset -- */
        /* -- 取得最大/最小垂直偏移 -- */
        public double y_get(int st, bool up, double x, double w)
        {
            double y;
            Staff p_staff = staff_tb[st];
            int i = (int)(x / 2);
            int j = (int)((x + w) / 2);

            if (i < 0)
                i = 0;
            if (j >= YSTEP)
            {
                j = (int)YSTEP - 1;
                if (i > j)
                    i = j;
            }
            if (up)
            {
                y = p_staff.top[i++];
                while (i <= j)
                {
                    if (y < p_staff.top[i])
                        y = p_staff.top[i];
                    i++;
                }
            }
            else
            {
                y = p_staff.bot[i++];
                while (i <= j)
                {
                    if (y > p_staff.bot[i])
                        y = p_staff.bot[i];
                    i++;
                }
            }
            return y;
        }

        /* -- adjust the vertical offsets -- */
        /* -- 調整垂直偏移 -- */
        public void y_set(int st, bool up, double x, double w, double y)
        {
            Staff p_staff = staff_tb[st];
            int i = (x / 2);
            int j = ((x + w) / 2);

            /* (may occur when annotation on 'y' at start of an empty staff) */
            if (i < 0)
                i = 0;
            if (j >= YSTEP)
            {
                j = YSTEP - 1;
                if (i > j)
                    i = j;
            }
            if (up)
            {
                while (i <= j)
                {
                    if (p_staff.top[i] < y)
                        p_staff.top[i] = y;
                    i++;
                }
            }
            else
            {
                while (i <= j)
                {
                    if (p_staff.bot[i] > y)
                        p_staff.bot[i] = y;
                    i++;
                }
            }
        }

        // get the staff position
        // - of the ornements
        public bool up3(VoiceItem s, int pos)
        {
            switch (pos & 0x07)
            {
                case C.SL_ABOVE:
                    return true;
                case C.SL_BELOW:
                    return false;
            }
            //if (s.multi)
            //    return s.multi > 0;
            //return true;
            return !s.second;
        }

        // - of the dynamic and volume marks
        // - 動態和音量標記
        public bool up6(VoiceItem s, int pos)
        {
            switch (pos & 0x07)
            {
                case C.SL_ABOVE:
                    return true;
                case C.SL_BELOW:
                    return false;
            }
            if (s.multi)
                return s.multi > 0;
            if (!s.p_v.have_ly)
                return false;

            /* above if the lyrics are below the staff */
            return (s.pos.voc & 0x07) != C.SL_ABOVE;
        }

        /* -- drawing functions -- */
        /* 2: special case for arpeggio */
        /* -- 繪圖函數 -- */
        /* 2: 琶音的特殊情況 */
        public static void d_arp(DecorationElement de)
        {
            int m;
            double h, dx;
            VoiceItem s = de.s;
            DecorationDef dd = de.dd;
            double xc = dd.wr;

            if (s.type == C.NOTE)
            {
                for (m = 0; m <= s.nhd; m++)
                {
                    if (s.notes[m].acc)
                    {
                        dx = s.notes[m].shac;
                    }
                    else
                    {
                        dx = 1 - s.notes[m].shhd;
                        switch (s.head)
                        {
                            case C.SQUARE:
                                dx += 3.5;
                                break;
                            case C.OVALBARS:
                            case C.OVAL:
                                dx += 2;
                                break;
                        }
                    }
                    if (dx > xc)
                        xc = dx;
                }
            }
            h = 3 * (s.notes[s.nhd].pit - s.notes[0].pit) + 4;
            m = dd.h;           /* minimum height */
            if (h < m)
                h = m;

            de.has_val = true;
            de.val = h;
            //de.x = s.x - xc;
            de.x -= xc;
            de.y = 3 * ((s.notes[0].pit + s.notes[s.nhd].pit) / 2 - 18) - h / 2 - 3;
        }

        /* 0: near the note (dot, tenuto) */
        /* 0: 靠近音符 (dot, tenuto) */
        public static void d_near(DecorationElement de)
        {
            int y;
            bool up = de.up;
            VoiceItem s = de.s;
            DecorationDef dd = de.dd;

            y = up ? s.ymx : s.ymn;
            if (y > 0 && y < 24)
            {
                y = (((y + 9) / 6)) * 6 - 6;   // between lines
            }
            if (up)
            {
                y += dd.hd;
                s.ymx = y + dd.h;
            }
            else if (dd.name[0] == 'w')       // wedge (no descent)
            {
                de.inv = true;
                y -= dd.h;
                s.ymn = y;
            }
            else
            {
                y -= dd.h;
                s.ymn = y - dd.hd;
            }
            de.x -= dd.wl;
            de.y = y;
            if (s.type == C.NOTE)
                de.x += s.notes[s.stem >= 0 ? 0 : s.nhd].shhd;
            if (dd.name[0] == 'd')       // if dot (staccato)
            {
                if (!(s.beam_st && s.beam_end))    // if in a beam sequence
                {
                    if (up)
                    {
                        if (s.stem > 0)
                            de.x += 3.5;    // stem_xoff
                    }
                    else
                    {
                        if (s.stem < 0)
                            de.x -= 3.5;
                    }
                }
                else
                {
                    if (up && s.stem > 0)
                    {
                        y = s.y + (y - s.y) * .6;
                        if (y >= 27)
                        {
                            de.y = y;   // put the dot a bit lower
                            s.ymx = de.y + dd.h;
                        }
                    }
                }
            }
        }

        /* 1: special case for slide */
        public static void d_slide(DecorationElement de)
        {
            int m, dx;
            VoiceItem s = de.s;
            int yc = s.notes[0].pit;
            int xc = 5;

            for (m = 0; m <= s.nhd; m++)
            {
                if (s.notes[m].acc)
                {
                    dx = 4 + s.notes[m].shac;
                }
                else
                {
                    dx = 5 - s.notes[m].shhd;
                    switch (s.head)
                    {
                        case C.SQUARE:
                            dx += 3.5;
                            break;
                        case C.OVALBARS:
                        case C.OVAL:
                            dx += 2;
                            break;
                    }
                }
                if (s.notes[m].pit <= yc + 3 && dx > xc)
                    xc = dx;
            }
            //de.x = s.x - xc;
            de.x -= xc;
            de.y = 3 * (yc - 18);
        }

        // special case for long decoration
        /* 1: 幻燈片的特殊情況 */
        public static void d_trill(DecorationElement de)
        {
            if (de.ldst)
                return;
            int y, w, tmp;
            DecorationDef dd = de.dd;
            DecorationElement de2 = de.prev;
            bool up = de.start.up;
            VoiceItem s2 = de.start.s;
            int st = s2.st;
            VoiceItem s = de.start.s;
            double x = s.x;

            // shift the starting point of a long decoration
            // in the cases "T!trill(!" and "!pp!!<(!"
            // (side effect on x)
            void sh_st()
            {
                DecorationElement de3;
                DecorationElement de2 = de.start;         // start of the decoration
                VoiceItem s = de2.s;
                int i = de2.ix;         // index of the current decoration

                while (--i >= 0)
                {
                    de3 = a_de[i];
                    if (de3 == null || de3.s != s)
                        break;
                }
                while (true)         // loop on the decorations of the symbol
                {
                    i++;
                    de3 = a_de[i];
                    if (de3 == null || de3.s != s)
                        break;
                    if (de3 == de2)
                        continue;
                    if (!(up ^ de3.up)
                     && (de3.dd.name == "trill"
                      || de3.dd.func == 6))    // dynamic
                    {
                        x += de3.dd.wr + 2;
                        break;
                    }
                }
            }

            // shift the ending point of a long decoration
            // (side effect on w)
            void sh_en(params dynamic[] arge)
            {
                dynamic de3;
                int i = de.ix;         // index of the current decoration

                while (--i > 0)
                {
                    de3 = a_de[i];
                    if (de3 == null || de3.s != s2)
                        break;
                }
                while (true)         // loop on the decorations of the symbol
                {
                    i++;
                    de3 = a_de[i];
                    if (de3 == null || de3.s != s2)
                        break;
                    //if (de3 == de || de3 == de2)
                    if (de3 == de)
                        continue;
                    if (!(up ^ de3.up)
                     && de3.dd.func == 6)    // if dynamic mark
                    {
                        w -= de3.dd.wl;
                        break;
                    }
                }
            }

            // d_trill()
            if (de2 != null)         // same height
            {
                x = de2.s.x + de.dd.wl + 2;
                de2.val -= de2.dd.wr;
                if (de2.val < 8)
                    de2.val = 8;
            }
            de.st = st;
            de.up = up;

            sh_st();               // shift the starting point?

            if (de.defl.noen)       /* if no decoration end */
            {
                w = de.x - x;
                if (w < 20)
                {
                    x = de.x - 20 - 3;
                    w = 20;
                }
            }
            else
            {
                w = s2.x - x - 4;
                sh_en(de);          // shift the ending point?
                if (w < 20)
                    w = 20;
            }
            y = y_get(st, up, x - dd.wl, w);
            if (up)
            {
                tmp = staff_tb[s.st].topbar + 2;
                if (y < tmp)
                    y = tmp;
            }
            else
            {
                tmp = staff_tb[s.st].botbar - 2;
                if (y > tmp)
                    y = tmp;
                y -= dd.h;
            }
            if (de2 != null)         // if same height
            {
                if (up)
                {
                    if (y < de2.y)
                        y = de2.y;  // (only on one note)
                }
                else
                {
                    if (y >= de2.y)
                    {
                        y = de2.y;
                    }
                    else
                    {
                        do
                        {
                            de2.y = y;
                            de2 = de2.prev; // go backwards
                        } while (de2 != null);
                    }
                }
            }

            de.lden = false;
            de.has_val = true;
            de.val = w;
            de.x = x;
            de.y = y;
            if (up)
                y += dd.h;
            else
                y -= dd.hd;
            y_set(st, up, x, w, y);
            if (up)
            {
                s.ymx = s2.ymx = y;
            }
            else
            {
                s.ymn = s2.ymn = y;
            }
        }

        /* 3, 4, 5, 7: above (or below) the staff */
        /* 3, 4, 5, 7: 員工上方（或下方） */
        public static void d_upstaff(DecorationElement de)
        {
            // don't treat here the long decorations
            if (de.ldst)            // if long deco start
                return;
            if (de.start != null)            // if long decoration
            {
                d_trill(de);
                return;
            }
            double y;
            int inv;
            bool up = de.up;
            VoiceItem s = de.s;
            DecorationDef dd = de.dd;
            double x = de.x;
            double w = dd.wl + dd.wr;

            // glyphs inside the staff
            switch (dd.glyph)
            {
                case "brth":
                case "caes":
                case "lphr":
                case "mphr":
                case "sphr":
                case "short":
                case "tick":
                    y = staff_tb[s.st].topbar + 2 + dd.hd;
                    if (s.type == C.BAR)
                    {
                        s.bar_dotted = true;
                    }
                    else
                    {
                        if (dd.glyph == "brth" && y < s.ymx)
                            y = s.ymx;
                        for (s = s.ts_next; s != null; s = s.ts_next)
                        {
                            if (s.seqst != null)
                                break;
                        }
                        x += ((s != null ? s.x : realwidth) - x) * .45;
                    }
                    de.x = x;
                    de.y = y;
                    return;
            }

            if (s.nhd != null)
                x += s.notes[s.stem >= 0 ? 0 : s.nhd].shhd;

            switch (dd.ty)
            {
                case '@':
                case '<':
                case '>':
                    y = de.y;
                    break;
                default:
                    y = 0;
                    break;
            }
            if (y == 0)
            {
                if (up)
                {
                    y = y_get(s.st, true, x - dd.wl, w)
                            + dd.hd;
                    if (de.y > y)
                        y = de.y;
                    s.ymx = y + dd.h;
                }
                else
                {
                    y = y_get(s.st, false, x - dd.wl, w)
                        - dd.h;
                    if (de.y < y)
                        y = de.y;
                    if (dd.name == "fermata"
                     || dd.glyph == "accent"
                     || dd.glyph == "roll")
                        de.inv = 1;
                    s.ymn = y - dd.hd;
                }
            }

            if (dd.wr > 10 && x > realwidth - dd.wr)
                de.x = x = realwidth - dd.wr - dd.wl;

            //if (dd.func == 6
            // && ((de.pos & C.SL_ALI_MSK) == C.SL_ALIGN
            //  || ((de.pos & C.SL_ALI_MSK) == 0
            //   && de.s.fmt.dynalign > 0)))    // if align
            //;
            //else
            if (up)
                y_set(s.st, 1, x - dd.wl, w, y + dd.h);
            else
                y_set(s.st, 0, x - dd.wl, w, y - dd.hd);

            de.y = y;
        }

        // add a decoration
        // 新增裝飾
        /* syntax:
         *	%%deco <name> <c_func> <glyph> <h> <wl> <wr> [<str>]
         * "<h>" may be followed by ",<hd>" (descent)
         */
        public void deco_add(string param)
        {
            var dv = param.Split(' ', 2);
            decos[dv[0]] = dv[1];
        }

        // define a decoration
        // nm is the name of the decoration
        // nmd is the name of the definition in the table 'decos'
        // 定義一個裝飾
        // nm 是裝飾的名稱
        // nmd 是表 'decos' 中定義的名稱
        public DecorationDef deco_def(string nm, string nmd = null)
        {
            if (nmd == null)
                nmd = nm;
            DecorationDef dd = null, dd2;
            int a, nm2, c, i, elts, hd;
            string str, text = decos[nmd]
            if (!string.IsNullOrEmpty(decos[nmd]))
            {
                text = decos[nmd];

                // check if a long decoration with number
                if (string.IsNullOrEmpty(text) && Regex.IsMatch(nmd, @"\d[()]$"))
                    text = decos[nmd.Replace("\d", "")];

                if (!string.IsNullOrEmpty(text))
                {
                    var a = Regex.Match(text, @"(\d+)\s+(.+?)\s+([0-9.,-]+)\s+([0-9.-]+)\s+([0-9.-]+)");
                    if (a.Success)
                    {
                        int c_func = int.Parse(a.Groups[1].Value);
                        string h = a.Groups[3].Value;
                        double wl = double.Parse(a.Groups[4].Value);
                        double wr = double.Parse(a.Groups[5].Value);

                        if (double.IsNaN(c_func))
                        {
                            Console.WriteLine("%%deco: bad C function value '{0}'", a.Groups[1].Value);
                            return null;
                        }
                        if (c_func > 10 && (c_func < 32 || c_func > 44))
                        {
                            Console.WriteLine("%%deco: bad C function index '{0}'", c_func);
                            return null;
                        }

                        if (h.Contains(","))
                        {
                            var hSplit = h.Split(',');
                            int hd = int.Parse(hSplit[1]);
                            h = hSplit[0];
                            // dd.hd = hd;
                        }
                        else
                        {
                            dd.hd = 0;
                        }
                        if (int.Parse(h) > 50 || wl > 80 || wr > 80)
                        {
                            Console.WriteLine("%%deco: abnormal h/wl/wr value '{0}'", text);
                            return null;
                        }

                        dd = new DecorationDef
                        {
                            name = nm,
                            func = nm.StartsWith("head-") ? 9 : c_func,
                            glyph = a.Groups[2].Value,
                            h = int.Parse(h),
                            wl = wl,
                            wr = wr
                        };
                    }
                    else
                    {
                        Console.WriteLine("%%deco: bad value '{0}'", text);
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine("Unknown decoration '{0}'", nm);
                    return null;
                }
            }
            else
            {
                Console.WriteLine("Invalid decoration '{0}'", nm);
                return null;
            }

            return dd;
        }

        // define a cross-voice tie
        // @nm = decoration name
        // @s = note symbol
        // @nt1 = note
        // 定義跨語音聯繫
        // @nm = 裝飾名稱
        // @s = 註解符號
        // @nt1 = 注意
        public void do_ctie(string nm, VoiceItem s, dynamic nt1)
        {
            dynamic nt2 = cross[nm];
            string nm2 = nm.Substring(0, nm.Length - 1) + (nm[nm.Length - 1] == '(' ? ')' : '(');

            if (nt2 != null)
            {
                Console.WriteLine("Conflict on '!{0}'!", nm);
                return;
            }
            nt1.s = s;
            nt2 = cross[nm2];
            if (nt2 != null)
            {
                if (nm[nm.Length - 1] == '(')
                {
                    nt2 = nt1;
                    nt1 = cross[nm2];
                }
                cross[nm2] = null;
                if (nt1.midi != nt2.midi || nt1.s.time + nt1.s.dur != nt2.s.time)
                {
                    Console.WriteLine("Bad tie");
                }
                else
                {
                    nt1.tie_ty = C.SL_AUTO;
                    nt1.tie_e = nt2;
                    nt2.tie_s = nt1;
                    nt1.s.ti1 = nt2.s.ti2 = true;
                }
            }
            else
            {
                cross[nm] = nt1;     // keep the start/end
            }
        }

        // get/create the definition of a decoration
        // 取得/建立裝飾的定義
        public DecorationDef get_dd(string nm)
        {
            DecorationDef dd = dd_tb[nm];

            if (dd != null)
                return dd;
            if ("<>^_@".IndexOf(nm[0]) >= 0 && !Regex.IsMatch(nm, @"^([>^]|[<>]\d?[()])$"))
            {
                string ty = nm[0].ToString();
                if (ty == "@")
                {
                    Match p = Regex.Match(nm, @"@([-\d]+),([-\d]+)");
                    if (p.Success)
                        ty = p.Value;
                    else
                        ty = "";        // accept decorations starting with '@'
                }
                dd = deco_def(nm, nm.Replace(ty, ""));
            }
            else
            {
                dd = deco_def(nm);
            }
            if (dd == null)
                return null;
            if (!string.IsNullOrEmpty(ty))
            {
                if (ty[0] == '@')       // if with x,y
                {
                    dd.x = int.Parse(ty[1].ToString());
                    dd.y = int.Parse(ty[3].ToString());
                    ty = "@";
                }
                dd.ty = ty;
            }
            return dd;
        }

        /* -- convert the decorations -- */
        /* -- 轉換裝飾 -- */
        public void deco_cnv(VoiceItem s, dynamic prev = null)
        {
            VoiceItem s1;
            DecorationDef dd;
            int i, j, nm, note, court;
            while (true)
            {
                string nm = a_dcn[0];
                if (string.IsNullOrEmpty(nm))
                    break;
                DecorationDef dd = get_dd(nm);
                if (dd == null)
                    continue;

                /* special decorations */
                switch (dd.func)
                {
                    case 0:         // near
                        if (s.type == C.BAR && nm == "dot")
                        {
                            s.bar_dotted = true;
                            continue;
                        }
                    // fall thru
                    case 1:         // slide
                    case 2:         // arp
                                    //if (s.type != C.NOTE && s.type != C.REST)
                        if (s.notes == null)
                        {
                            Console.WriteLine("Must be a note or rest");
                            continue;
                        }
                        break;
                    case 4:         // below the staff
                    case 5:         // above the staff
                        Match i = Regex.Match(nm, @"1?[85]([vm])([ab])([()])");
                        if (i.Success)              // if ottava
                        {
                            int j = i.Groups[1].Value == "v" ? 1 : 2;
                            if (i.Groups[2].Value == "b")
                                j = -j;
                            if (s.ottava == null)
                                s.ottava = new int[2];
                            s.ottava[i.Groups[3].Value == "(" ? 0 : 1] = j;
                            glovar.ottava = true;
                        }
                        break;
                    case 8:         // gliss
                        if (s.type != C.NOTE)
                        {
                            Console.WriteLine("Must be a note");
                            continue;
                        }
                        NoteItem note = s.notes[s.nhd]; // move to the upper note of the chord
                        if (note.a_dd == null)
                            note.a_dd = new List<DecorationDef>();
                        note.a_dd.Add(dd);
                        continue;
                    case 9:         // alternate head
                        if (s.notes == null)
                        {
                            Console.WriteLine("Must be a note or rest");
                            continue;
                        }

                        // move the alternate head of the chord to the notes
                        for (int j = 0; j <= s.nhd; j++)
                        {
                            note = s.notes[j];
                            note.invis = true;
                            if (note.a_dd == null)
                                note.a_dd = new List<DecorationDef>();
                            note.a_dd.Add(dd);
                        }
                        continue;
                    case 10:        /* color */
                        if (s.notes != null)
                        {
                            for (int j = 0; j <= s.nhd; j++)
                                s.notes[j].color = nm;
                        }
                        else
                        {
                            s.color = nm;
                        }
                        break;
                    case 32:        /* invisible */
                        s.invis = true;
                        break;
                    case 33:        /* beamon */
                        if (s.type != C.BAR)
                        {
                            Console.WriteLine("!beamon! must be on a bar");
                            continue;
                        }
                        s.beam_on = true;
                        break;
                    case 34:        /* trem1..trem4 */
                        if (s.type != C.NOTE || prev == null || prev.type != C.NOTE || s.dur != prev.dur)
                        {
                            Console.WriteLine("!{0}! must be on the last of a couple of notes", nm);
                            continue;
                        }
                        s.trem2 = true;
                        s.beam_end = true;
                        s.beam_st = false;
                        prev.beam_st = true;
                        prev.beam_end = false;
                        s.ntrem = prev.ntrem = int.Parse(nm[4].ToString());
                        for (int j = 0; j <= s.nhd; j++)
                            s.notes[j].dur *= 2;
                        for (int j = 0; j <= prev.nhd; j++)
                            prev.notes[j].dur *= 2;
                        break;
                    case 35:        /* xstem */
                        if (s.type != C.NOTE)
                        {
                            Console.WriteLine("Must be a note");
                            continue;
                        }
                        s.xstem = true;
                        break;
                    case 36:        /* beambr1 / beambr2 */
                        if (s.type != C.NOTE)
                        {
                            Console.WriteLine("Must be a note");
                            continue;
                        }
                        if (nm[6] == '1')
                            s.beam_br1 = true;
                        else
                            s.beam_br2 = true;
                        break;
                    case 37:        /* rbstop */
                        s.rbstop = 1;   // open
                        break;
                    case 38:        /* /, // and /// = tremolo */
                        if (s.type != C.NOTE)
                        {
                            Console.WriteLine("Must be a note");
                            continue;
                        }
                        s.trem1 = true;
                        s.ntrem = nm.Length;    /* 1, 2 or 3 */
                        break;
                    case 39:        /* beam-accel/beam-rall */
                        if (s.type != C.NOTE)
                        {
                            Console.WriteLine("Must be a note");
                            continue;
                        }
                        s.feathered_beam = nm[5] == 'a' ? 1 : -1;
                        break;
                    case 40:        /* stemless */
                        s.stemless = true;
                        break;
                    case 41:        /* rbend */
                        s.rbstop = 2;   // with end
                        break;
                    case 42:        // editorial
                        if (!s.notes[0].acc)
                            continue;
                        nm = "sacc" + s.notes[0].acc.ToString(); // small accidental
                        dd = dd_tb[nm];
                        if (dd == null)
                        {
                            dd = deco_def(nm);
                            if (dd == null)
                            {
                                Console.WriteLine("Bad value '!editorial!'");
                                continue;
                            }
                        }
                        s.notes[0].acc = null;
                        curvoice.acc[s.notes[0].pit + 19] = 0;   // ignore the accidental
                        break;
                    case 43:        // courtesy
                        int j = curvoice.acc[s.notes[0].pit + 19];
                        if (s.notes[0].acc != null || j == 0)
                            continue;
                        court = 1;          // defer
                        break;
                    case 44:        // cross-voice ties
                        do_ctie(nm, s, s.notes[0]); // (only one note for now)
                        continue;
                        //default:
                        //    break;
                }

                // add the decoration in the symbol
                if (s.a_dd == null)
                    s.a_dd = new List<DecorationDef>();
                s.a_dd.Add(dd);
            }
            // handle the possible courtesy accidental
            if (court != 0)
            {
                a_dcn.Add("cacc" + j);
                dh_cnv(s, s.notes[0]);
            }
        }

        // -- convert head decorations --
        // The decorations are in the global array a_dcn
        // -- 轉換頭部裝飾 --
        // 裝飾位於全域數組 a_dcn 中
        public void dh_cnv(VoiceItem s, object nt)
        {
            string nm;
            DecorationDef dd;

            while (true)
            {
                nm = a_dcn.Shift();
                if (string.IsNullOrEmpty(nm))
                    break;
                dd = get_dd(nm);
                if (dd == null)
                    continue;

                switch (dd.func)
                {
                    case 0:
                    case 1:
                    case 3:
                    case 4:
                    case 8:         // gliss
                        break;
                    default:
                        //case 2:         // arpeggio
                        //case 5:         // trill
                        //case 7:         // d_cresc
                        error(1, s, "Cannot have !$1! on a head", nm);
                        continue;
                    case 9:         // head replacement
                        nt.invis = true;
                        break;
                    case 32:        // invisible
                        nt.invis = true;
                        continue;
                    case 10:        // color
                        nt.color = nm;
                        continue;
                    case 40:        // stemless chord (abcm2ps behaviour)
                        s.stemless = true;
                        continue;
                    case 44:        // cross-voice ties
                        do_ctie(nm, s, nt);
                        continue;
                }

                // add the decoration in the note
                if (nt.a_dd == null)
                    nt.a_dd = new List<DecorationDef>();
                nt.a_dd.Add(dd);
            }
        }

        /* -- update the x position of a decoration -- */
        // used to center the rests
        /* -- 更新裝飾的 x 位置 -- */
        // 用於將其餘部分居中
        public void deco_update(VoiceItem s, double dx)
        {
            int nd = a_de.Count;

            for (int i = 0; i < nd; i++)
            {
                DecorationElement de = a_de[i];
                if (de.s == s)
                    de.x += dx;
            }
        }

        /* -- adjust the symbol width -- */
        /* -- 調整符號寬度 -- */
        public double deco_width(VoiceItem s, double wlnt)
        {
            DecorationDef dd;
            int i = 0;
            double w = 0;
            double wl = wlnt;
            double wr = s.wr;
            List<DecorationDef> a_dd = s.a_dd;
            int nd = a_dd.Count;

            for (i = 0; i < nd; i++)
            {
                dd = a_dd[i];
                switch (dd.func)
                {
                    case 1:         /* slide */
                    case 2:         /* arpeggio */
                        if (wl < 12)
                            wl = 12;
                        break;
                    case 3:
                        switch (dd.glyph)
                        {
                            case "brth":
                            case "lphr":
                            case "mphr":
                            case "sphr":
                                if (s.wr < 20)
                                    s.wr = 20;
                                break;
                            default:
                                w = dd.wl + 2;
                                if (wl < w)
                                    wl = w;
                                break;
                        }
                    // fall thru
                    default:
                        switch (dd.ty)
                        {
                            case '<':
                                w = wlnt + dd.wl + dd.wr + 6;
                                if (wl < w)
                                    wl = w;
                                break;
                            case '>':
                                w = wr + dd.wl + dd.wr + 6;
                                if (s.wr < w)
                                    s.wr = w;
                                break;
                        }
                        break;
                }
            }
            return wl;
        }

        // compute the width of decorations in chord
        // 計算和弦中裝飾的寬度
        public double deco_wch(object nt)
        {
            int i = 0;
            double w = 0;
            DecorationDef dd ;
            double wl = 0;
            int n = nt.a_dd.Count;

            for (i = 0; i < n; i++)
            {
                dd = nt.a_dd[i];
                if (dd.ty == '<')
                {
                    w = dd.wl + dd.wr + 4;
                    if (w > wl)
                        wl = w;
                }
            }
            return wl;
        }

        /* -- draw the decorations -- */
        /* (the staves are defined) */
        /* -- 繪製裝飾 -- */
        /*（五線譜已定義）*/
        public void draw_all_deco()
        {
            if (a_de.Count == 0)
                return;
            DecorationElement de;
            DecorationDef dd;
            VoiceItem s;
            NoteItem note ;
            var f = "";
            int st = 0;
            double x, y, y2, ym;
            var uf = "";
            int i = 0;
            string str;
            var a = new List<string>();
            List<DecorationElement> new_de = new List<DecorationElement>();
            List<double> ymid = new List<double>();

            st = nstaff;
            y = staff_tb[st].y;
            while (--st >= 0)
            {
                y2 = staff_tb[st].y;
                ymid[st] = (y + 24 + y2) * .5;
                y = y2;
            }

            while (true)
            {
                de = a_de[0];
                if (string.IsNullOrEmpty(de))
                    break;
                dd = de.dd;
                if (string.IsNullOrEmpty(dd))        // deleted
                    continue;

                if (dd.dd_en)           // start of long decoration
                    continue;

                // handle the stem direction
                s = de.s;
                f = dd.glyph;
                i = f.IndexOf('/');
                if (i > 0)
                {
                    if (s.stem >= 0)
                        f = f.Substring(0, i);
                    else
                        f = f.Substring(i + 1);
                }

                // no voice scale if staff decoration
                if (f_staff[dd.func])
                    set_sscale(s.st);
                else
                    set_scale(s);

                st = de.st;
                if (!staff_tb[st].topbar)       // invisible staff
                    continue;
                x = de.x + (dd.dx || 0);
                y = de.y + staff_tb[st].y + (dd.y || 0);

                // update the coordinates if head decoration
                if (de.m != null)
                {
                    note = s.notes[de.m];
                    if (note.shhd)
                        x += note.shhd * stv_g.scale;

                    /* center the dynamic marks between two staves */
                    /*fixme: KO when deco on other voice and same direction*/
                }
                else if (dd.func == 6
                    && ((de.pos & C.SL_ALI_MSK) == C.SL_CENTER
                     || ((de.pos & C.SL_ALI_MSK) == 0
                      && !s.fmt.dynalign))
                    && ((de.up && st > 0)
                     || (!de.up && st < nstaff)))
                {
                    if (de.up)
                        ym = ymid[--st];
                    else
                        ym = ymid[st++];
                    ym -= dd.h * .5;
                    if ((de.up && y < ym)
                     || (!de.up && y > ym))
                    {
                        y2 = y_get(st, !de.up, de.x, de.val)
                            + staff_tb[st].y;
                        if (de.up)
                            y2 -= dd.h;
                        if ((de.up && y2 > ym)
                         || (!de.up && y2 < ym))
                        {
                            y = ym;
                            if (stv_g.scale != 1)
                                y += stv_g.dy / 2;
                        }
                    }
                }

                // check if user JS decoration
                if (user.deco != null)
                {
                    uf = user.deco[f];
                    if (uf != null && uf.GetType() == typeof(Action<object>))
                    {
                        uf(x, y, de);
                        continue;
                    }
                }

                // check if user PS definition
                if (self.psdeco(x, y, de))
                    continue;

                anno_start(s, 'deco');
                if (de.inv)
                {
                    y = y + dd.h - dd.hd;
                    g_open(x, y, 0, 1, -1);
                    x = y = 0;
                }
                if (de.has_val)
                {
                    if (dd.func != 2     // if not !arpeggio!
                     || stv_g.st < 0)   // or not staff scale
                        out_deco_val(x, y, f, de.val / stv_g.scale, de.defl);
                    else
                        out_deco_val(x, y, f, de.val, de.defl);
                    if (de.cont)
                        new_de.Add(de.start);   // to be continued next line
                }
                else if (dd.str != null        // string
                 && !tgls[dd.glyph]
                 && !glyphs[dd.glyph])       // with a class
                {
                    out_deco_str(x, y,        // - dd.h * .2,
                        de);
                }
                else if (de.lden)
                {
                    out_deco_long(x, y, de);
                }
                else
                {
                    xygl(x, y, f);
                }
                if (stv_g.g)
                    g_close();
                anno_stop(s, 'deco');
            }

            // keep the long decorations which continue on the next line
            a_de = new_de;

            // create the decorations of note heads
            for (i = 0; i < nd; i++)
            {
                de = a_de[i];
                dd = de.dd;
                f = dd.func;
                if (f_note[f]
                 && de.m == null)
                    f_note[f](de);
            }
        }

        /* -- create the decorations and define the ones near the notes -- */
        /* (the staves are not yet defined) */
        /* (delayed output) */
        /* this function must be called first as it builds the deco element table */
        /* -- 建立裝飾並定義音符附近的裝飾 -- */
        /*（五線譜尚未定義）*/
        /*（延遲輸出）*/
        /* 這個函數在建立裝飾元素表時必須先呼叫 */
        public void draw_deco_near()
        {
            var s = "";
            var g = "";

            // update starting old decorations
            void ldeco_update(VoiceItem s)
            {
                int i = 0;
                DecorationElement de;
                double x = s.ts_prev.x + s.ts_prev.wr;
                int nd = a_de.Count;

                for (i = 0; i < nd; i++)
                {
                    de = a_de[i];
                    de.ix = i;
                    de.s.x = de.x = x;
                    de.defl.nost = true;
                }
            }

            /* -- create the deco elements, and treat the near ones -- */
            /* -- 建立裝飾元素，並處理附近的元素 -- */
            void create_deco(VoiceItem s)
            {
                DecorationDef dd;
                int k, pos;
                DecorationElement de;
                double x,y;
                var up = 0;
                var nd = s.a_dd.Count;

                if (s.y == null)
                    s.y = 0;            // (no y in measure bars)

                /*fixme:pb with decorations above the staff*/
                /*fixme:pb 在五線譜上方有裝飾*/
                for (k = 0; k < nd; k++)
                {
                    dd = s.a_dd[k];

                    // adjust the position
                    x = s.x;
                    y = s.y;
                    switch (dd.func)
                    {
                        default:
                            if (dd.func >= 10)
                                continue;
                            pos = 0;
                            break;
                        case 3:             /* d_upstaff */
                        case 4:
                        case 5:             // after slurs
                            pos = s.pos.orn;
                            break;
                        case 6:             /* dynamic */
                            pos = s.pos.dyn;
                            break;
                    }

                    switch (dd.ty)
                    {        // explicit position
                        case "^":
                            pos = (pos & ~0x07) | C.SL_ABOVE;
                            break;
                        case "_":
                            pos = (pos & ~0x07) | C.SL_BELOW;
                            break;
                        case "<":
                        case ">":
                            pos = (pos & 0x07) | C.SL_CLOSE;
                            if (dd.ty == "<")
                            {
                                x -= dd.wr + 8;
                                if (s.notes[0].acc)
                                    x -= 8;
                            }
                            else
                            {
                                x += dd.wl + 8;
                            }
                            y = 3 * (s.notes[0].pit - 18)
                                    - (dd.h - dd.hd) / 2;
                            break;
                        case "@":
                            x += dd.x;
                            y += dd.y;
                            break;
                    }

                    if ((pos & 0x07) == C.SL_HIDDEN)
                        continue;

                    de = new DecorationElement()
                {
                     s= s ,
                     dd= dd ,
                    st= s.st ,
                    ix= a_de.Count - 1 ,
                    defl= new Dictionary<string, object>() ,
                    x= x ,
                    y= y 
                };
                    if (pos != 0)
                        de.pos = pos;

                    up = 0; //false
                    if (dd.ty == "^")
                    {
                        up = 1; //true
                    }
                    else if (dd.ty == "_")
                    {
                        ;
                    }
                    else
                    {
                        switch (dd.func)
                        {
                            case 0:
                                if (s.multi)
                                    up = s.multi > 0;
                                else
                                    up = s.stem < 0;
                                break;
                            case 3:
                            case 5:
                                up = up3(s, pos);
                                break;
                            case 6:
                            case 7:
                                up = up6(s, pos);
                                break;
                        }
                    }
                    de.up = up;

                    if (dd.name.IndexOf("inverted") >= 0)
                        de.inv. = 1;
                    if (s.type == C.BAR && dd.ty == null)
                        de.x = s.x - s.wl / 2 - 2;
                    a_de.Add(de);
                    if (dd.dd_en)
                    {
                        de.ldst = true;
                    }
                    else if (dd.dd_st)
                    {
                        de.lden = true;
                        de.defl["nost"] = true;
                    }

                    if (f_near[dd.func])
                        f_near[dd.func](de);
                }
            } // create_deco()

            // create the decorations of note heads
            // 建立音符頭的裝飾
            void create_dh(VoiceItem s, int m)
            {
                var de = "";
                var k = 0;
                DecorationDef dd;
                int nd = s.notes[m].a_dd.Count;
                var x = s.x;

                for (k = 0; k < nd; k++)
                {
                    dd = s.notes[m].a_dd[k];

                    //fixme: check if hidden?
                    de = new Dictionary<string, object>()
                {
                    { "s", s },
                    { "dd", dd },
                    { "st", s.st },
                    { "m", m },
                    { "ix", 0 },
                    { "defl", new Dictionary<string, object>() },
                    { "x", x },
                    { "y", 3 * (s.notes[m].pit - 18) - (dd.h - dd.hd) / 2 }
                };

                    a_de.Add(de);
                    if (dd.dd_en)
                    {
                        de.ldst = true;
                    }
                    else if (dd.dd_st)
                    {
                        de.lden = true;
                        de.defl["nost"] = true;
                    }
                }
            } // create_dh()

            // create all decorations of a note (chord and heads)
            // 創造音符的所有裝飾（和弦和頭）
            void create_all(string s)
            {
                if (s.invis && s.play)  // play sequence: no decoration
                    return;
                if (s.a_dd != null)
                    create_deco(s);
                if (s.notes != null)
                {
                    for (var m = 0; m < s.notes.Count; m++)
                    {
                        if (s.notes[m].a_dd != null)
                            create_dh(s, m);
                    }
                }
            } // create_all()

            // link the long decorations
            // 連結長裝飾
            void ll_deco()
            {
                int i, j;
                DecorationElement de, de2, de3;
                DecorationDef dd, dd2;
                var v = 0;
                VoiceItem s;
                int st = 0;
                int n_de = a_de.Count;

                // add ending decorations
                for (i = 0; i < n_de; i++)
                {
                    de = a_de[i];
                    if (!de.ldst)   // not the start of long decoration
                        continue;
                    dd = de.dd;
                    dd2 = dd.dd_en;
                    s = de.s;
                    v = s.v;            // search later in the voice
                    for (j = i + 1; j < n_de; j++)
                    {
                        de2 = a_de[j];
                        if (!de2.start
                         && de2.dd == dd2 && de2.s.v == v)
                            break;
                    }
                    if (j == n_de)  // no end, search in the staff
                    {
                        st = s.st;
                        for (j = i + 1; j < n_de; j++)
                        {
                            de2 = a_de[j];
                            if (!de2.start
                             && de2.dd == dd2 && de2.s.st == st)
                                break;
                        }
                    }
                    if (j == n_de)  // no end, insert one
                    {
                        de2 = new Dictionary<string, object>()
                    {
                        { "s", s },
                        { "st", de.st },
                        { "dd", dd2 },
                        { "ix", a_de.Count - 1 },
                        { "x", realwidth - 6 },
                        { "y", s.y },
                        { "cont", true },   // keep for next line
                        { "lden", true },
                        { "defl", new Dictionary<string, object>()
                            {
                                { "noen", true }
                            }
                        }
                    };
                        if (de2.x < s.x + 10)
                            de2.x = s.x + 10;
                        if (de.m != null)
                            de2.m = de.m;
                        a_de.Add(de2);
                    }
                    de2.start = de;
                    de2.defl.nost = de.defl.nost;

                    // handle same decoration ending at a same time
                    // 處理同時結束的相同裝飾
                    j = i;
                    while (--j >= 0)
                    {
                        de3 = a_de[j];
                        if (!de3.start)
                            continue;
                        if (de3.s.time < s.time)
                            break;
                        if (de3.dd.name == de2.dd.name)
                        {
                            de2.prev = de3;
                            break;
                        }
                    }
                }

                // add starting decorations
                // 新增起��裝飾
                for (i = 0; i < n_de; i++)
                {
                    de2 = a_de[i];
                    if (!de2.lden    // not the end of long decoration 漫長的裝飾還沒結束
                     || de2.start)  // start already found 開始已經找到
                        continue;
                    s = de2.s;
                    de = new DecorationElement
                {
                    s= prev_scut(s) ,
                    st= de2.st ,
                    dd= de2.dd.dd_st ,
                    ix= a_de.Count - 1 ,
                    y= s.y ,
                    ldst= true 
                };
                    de.x = de.s.x + de.s.wr;
                    if (de2.m != null)
                        de.m = de2.m;
                    a_de.Add(de);
                    de2.start = de;
                }
            } // ll_deco

            // update the long decorations started in the previous line
            // 更新上一行開始的長裝飾
            for (s = tsfirst; s != null; s = s.ts_next)
            {
                switch (s.type)
                {
                    case C.CLEF:
                    case C.KEY:
                    case C.METER:
                        continue;
                }
                break;
            }
            if (a_de.Count > 0)
                ldeco_update(s);

            for (; s != null; s = s.ts_next)
            {
                switch (s.type)
                {
                    case C.BAR:
                    case C.MREST:
                    case C.NOTE:
                    case C.REST:
                    case C.SPACE:
                        break;
                    case C.GRACE:
                        for (var g = s.extra; g != null; g = g.next)
                            create_all(g);
                        break;
                    default:
                        continue;
                }
                create_all(s);
            }
            ll_deco();          // link the long decorations
        }

        /* -- define the decorations tied to a note -- */
        /* (the staves are not yet defined) */
        /* (delayed output) */
        /* -- 定義與註解相關的裝飾 -- */
        /*（五線譜尚未定義）*/
        /*（延遲輸出）*/
        public void draw_deco_note()
        {
            DecorationElement de ;
            DecorationDef dd ;
            int f;
            int nd = a_de.Count;

            for (int i = 0; i < nd; i++)
            {
                de = a_de[i];
                dd = de.dd;
                f = dd.func;
                if (f_note[f]
                 && de.m != null)
                    f_note[f](de);
            }
        }

        // -- define the music elements tied to the staff --
        //	- decoration tied to the staves
        //	- chord symbols
        //	- repeat brackets
        /* (the staves are not yet defined) */
        /* (unscaled delayed output) */
        // -- 定義與五線譜相關的音樂元素 --
        // - 綁在五線譜上的裝飾
        // - 和弦符號
        // - 重複括號
        /*（五線譜尚未定義）*/
        /*（未縮放的延遲輸出）*/
        public void draw_deco_staff()
        {
            VoiceItem s;
            var p_voice = "";
            double y = 0;
            int i, v;
            DecorationElement de;
            DecorationDef dd;
            double w = 0;
            List<MinMaxDeco> minmax = new List<MinMaxDeco>();
            int nd = a_de.Count;

            /* draw the repeat brackets */
            void draw_repbra(object p_voice)
            {
                var s = "";
                var s1 = "";
                var x = 0;
                var y = 0;
                var y2 = 0;
                var i = 0;
                var p = 0;
                var w = 0;
                var wh = 0;
                var first_repeat = "";

                // search the max y offset of the line
                y = staff_tb[p_voice.st].topbar + 15;   // 10 (vert bar) + 5 (room)
                for (s = p_voice.sym; s != null; s = s.next)
                {
                    if (s.type != C.BAR)
                        continue;
                    if (!s.rbstart || s.norepbra)
                        continue;
                    /*fixme: line cut on repeat!*/
                    if (!s.next)
                        break;
                    if (first_repeat == "")
                    {
                        first_repeat = s;
                        set_font("repeat");
                    }
                    s1 = s;
                    while (true)
                    {
                        if (!s.next)
                            break;
                        s = s.next;
                        if (s.rbstop)
                            break;
                    }
                    x = s1.x;
                    if (s1.xsh)         // volta shift
                        x += s1.xsh;
                    y2 = y_get(p_voice.st, true, x, s.x - x) + 2;
                    if (y < y2)
                        y = y2;

                    // have room for the vertical lines and the repeat numbers
                    if (s1.rbstart == 2)
                    {
                        y2 = y_get(p_voice.st, true, x, 3) + 10;
                        if (y < y2)
                            y = y2;
                    }
                    if (s.rbstop == 2)
                    {
                        y2 = y_get(p_voice.st, true, s.x - 3, 3) + 10;
                        if (y < y2)
                            y = y2;
                    }
                    if (s1.text != null)
                    {
                        wh = strwh(s1.text);
                        y2 = y_get(p_voice.st, true, x + 4, wh[0]) +
                                wh[1];
                        if (y < y2)
                            y = y2;
                    }
                    if (s.rbstart)
                        s = s.prev;
                }

                /* draw the repeat indications */
                s = first_repeat;
                if (s == null)
                    return;
                set_dscale(p_voice.st, true);
                y2 = y * staff_tb[p_voice.st].staffscale;
                for (; s != null; s = s.next)
                {
                    if (!s.rbstart || s.norepbra)
                        continue;
                    s1 = s;
                    while (true)
                    {
                        if (!s.next)
                            break;
                        s = s.next;
                        if (s.rbstop)
                            break;
                    }
                    if (s1 == s)
                        break;
                    x = s1.x;
                    if (s1.xsh)         // volta shift
                        x += s1.xsh;
                    if (cfmt.measurenb > 0 & s.bar_num
                     && s.bar_num % cfmt.measurenb)
                        x += 6;
                    if (s.type != C.BAR)
                    {
                        w = s.rbstop ? 0 : s.x - realwidth + 4;
                    }
                    else if ((s.bar_type.Length > 1   // if complex bar
                         && s.bar_type != "[]")
                        || s.bar_type == "]")
                    {
                        //if (s.bar_type == "]")
                        //    s.invis = true;
                        //fixme:%%staves: cur_sy moved?
                        if (s1.st > 0
                         && !(cur_sy.staves[s1.st - 1].flags & STOP_BAR))
                            w = s.wl;
                        else if (s.bar_type[s.bar_type.Length - 1] == ':')
                            w = 12;
                        else if (s.bar_type[0] != ':')
                            //      || s.bar_type == "]")
                            w = 0;      /* explicit repeat end */
                        else
                            w = 8;
                    }
                    else
                    {
                        w = (s.rbstop && !s.rbstart) ? 0 : 8;
                    }
                    w = (s.x - x - w)   // / staff_tb[p_voice.st].staffscale;

                if (!s.next      // 2nd ending at end of line
                 && !s.rbstop
                 && !p_voice.bar_start) // continue on next line
                    {
                        p_voice.bar_start = _bar(s);
                        p_voice.bar_start.bar_type = "";
                        p_voice.bar_start.rbstart = 1;
                    }
                    if (s1.text != null)
                        xy_str(x + 4, y2 - gene.curfont.size,
                            s1.text);
                    xypath(x, y2);
                    if (s1.rbstart == 2)
                        output += 'm0 10v-10';
                    output += 'h' + w.ToString("F1");
                    if (s.rbstop == 2)
                        output += 'v10';
                    output += '"/>\n';
                    y_set(s1.st, true, x, w, y + 2);

                    if (s.rbstart)
                        s = s.prev;
                }
            } // draw_repbra()

            /* create the decorations tied to the staves */
            for (i = 0; i <= nstaff; i++)
                minmax.Add(new MinMaxDeco { ymin = 0, ymax = 0 });
            for (i = 0; i < nd; i++)
            {
                de = a_de[i];
                dd = de.dd;
                if (dd == null)        // if error
                    continue;
                if (!f_staff[dd.func]   /* if not tied to the staff */
                 || de.m != null        // or head decoration
                 || dd.ty == '<' || dd.ty == '>' || dd.ty == '@')
                    continue;

                f_staff[dd.func](de);
                if (dd.func != 6
                 || dd.dd_en)        // if start
                    continue;

                if ((de.pos & C.SL_ALI_MSK) == C.SL_ALIGN
                 || ((de.pos & C.SL_ALI_MSK) == 0
                  && de.s.fmt.dynalign > 0))
                {   // if align
                    if (de.up)
                    {
                        if (de.y > minmax[de.st].ymax)
                            minmax[de.st].ymax = de.y;
                    }
                    else
                    {
                        if (de.y < minmax[de.st].ymin)
                            minmax[de.st].ymin = de.y;
                    }
                }
            }

            // set the same vertical offset of the dynamic marks
            for (i = 0; i < nd; i++)
            {
                de = a_de[i];
                dd = de.dd;
                if (dd == null)             // if error
                    continue;

                // if @x,y offsets, update the top and bottom of the staff
                if (dd.ty == '@')
                {
                    var y2 = 0;

                    y = de.y;
                    if (y > 0)
                    {
                        y2 = y + dd.h + 2;
                        if (y2 > staff_tb[de.st].ann_top)
                            staff_tb[de.st].ann_top = y2;
                    }
                    else
                    {
                        y2 = y - dd.hd - 2;
                        if (y2 < staff_tb[de.st].ann_bot)
                            staff_tb[de.st].ann_bot = y2;
                    }
                    continue;
                }
                if (dd.func != 6
                 || dd.ty == '<' || dd.ty == '>'
                 || dd.dd_en)                // if start
                    continue;

                w = de.val || (dd.wl + dd.wr);
                if ((de.pos & C.SL_ALI_MSK) == C.SL_ALIGN
                  || ((de.pos & C.SL_ALI_MSK) == 0
                   && de.s.fmt.dynalign > 0))
                {        // if align
                    if (de.up)
                        y = minmax[de.st].ymax;
                    else
                        y = minmax[de.st].ymin;
                    de.y = y;
                }
                else
                {
                    y = de.y;
                }
                if (de.up)
                    y += dd.h;
                else
                    y -= dd.hd;
                y_set(de.st, de.up, de.x, w, y);
            }

            // second pass for pedal (under the staff)
            for (i = 0; i < nd; i++)
            {
                de = a_de[i];
                dd = de.dd;
                if (dd == null)             // if error
                    continue;
                if (dd.dd_en            // if start
                 || dd.name.Substring(0, 3) != "ped")
                    continue;
                w = de.val || 10;
                de.y = y_get(de.st, 0, de.x, w)
                    - (dd.dd_st && cfmt.pedline ? 10 : dd.h);
                y_set(de.st, 0, de.x, w, de.y);    // (no descent)
            }

            draw_all_chsy();        // draw all chord symbols

            /* draw the repeat brackets */
            for (v = 0; v < voice_tb.Count; v++)
            {
                p_voice = voice_tb[v];
                if (p_voice.second || p_voice.sym == null || p_voice.ignore)
                    continue;
                draw_repbra(p_voice);
            }
        }

        //// -- define the music elements tied to the staff --
        ////	- decoration tied to the staves
        ////	- chord symbols
        ////	- repeat brackets
        ///* (the staves are not yet defined) */
        ///* (unscaled delayed output) */
        //// -- 定義與五線譜相關的音樂元素 --
        //// - 綁在五線譜上的裝飾
        //// - 和弦符號
        //// - 重複括號
        ///*（五線譜尚未定義）*/
        ///*（未縮放的延遲輸出）*/
        //public void draw_deco_staff()
        //{
        //    var s = 0;
        //    var p_voice = new object();
        //    var y = 0;
        //    var i = 0;
        //    var v = 0;
        //    var de = new object();
        //    var dd = new object();
        //    var w = 0;
        //    var minmax = new List<object>();
        //    var nd = a_de.Length;

        //    // draw the repeat brackets
        //    void draw_repbra(object p_voice)
        //    {
        //        var s = 0;
        //        var s1 = 0;
        //        var x = 0;
        //        var y = 0;
        //        var y2 = 0;
        //        var i = 0;
        //        var p = 0;
        //        var w = 0;
        //        var wh = 0;
        //        var first_repeat = 0;

        //        // search the max y offset of the line
        //        y = staff_tb[p_voice.st].topbar + 15; // 10 (vert bar) + 5 (room)
        //        for (s = p_voice.sym; s != null; s = s.next)
        //        {
        //            if (s.type != C.BAR)
        //                continue;
        //            if (!s.rbstart || s.norepbra)
        //                continue;
        //            /*fixme: line cut on repeat!*/
        //            if (s.next == null)
        //                break;
        //            if (first_repeat == 0)
        //            {
        //                first_repeat = s;
        //                set_font("repeat");
        //            }
        //            s1 = s;
        //            for (; ; )
        //            {
        //                if (s.next == null)
        //                    break;
        //                s = s.next;
        //                if (s.rbstop)
        //                    break;
        //            }
        //            x = s1.x;
        //            if (s1.xsh != 0) // volta shift
        //                x += s1.xsh;
        //            y2 = y_get(p_voice.st, true, x, s.x - x) + 2;
        //            if (y < y2)
        //                y = y2;

        //            // have room for the vertical lines and the repeat numbers
        //            if (s1.rbstart == 2)
        //            {
        //                y2 = y_get(p_voice.st, true, x, 3) + 10;
        //                if (y < y2)
        //                    y = y2;
        //            }
        //            if (s.rbstop == 2)
        //            {
        //                y2 = y_get(p_voice.st, true, s.x - 3, 3) + 10;
        //                if (y < y2)
        //                    y = y2;
        //            }
        //            if (s1.text != null)
        //            {
        //                wh = strwh(s1.text);
        //                y2 = y_get(p_voice.st, true, x + 4, wh[0]) +
        //                        wh[1];
        //                if (y < y2)
        //                    y = y2;
        //            }
        //            if (s.rbstart != 0)
        //                s = s.prev;
        //        }

        //        /* draw the repeat indications */
        //        s = first_repeat;
        //        if (s == null)
        //            return;
        //        set_dscale(p_voice.st, true);
        //        y2 = y * staff_tb[p_voice.st].staffscale;
        //        for (; s != null; s = s.next)
        //        {
        //            if (s.rbstart == 0 || s.norepbra)
        //                continue;
        //            s1 = s;
        //            while (true)
        //            {
        //                if (s.next == null)
        //                    break;
        //                s = s.next;
        //                if (s.rbstop)
        //                    break;
        //            }
        //            if (s1 == s)
        //                break;
        //            x = s1.x;
        //            if (s1.xsh != 0) // volta shift
        //                x += s1.xsh;
        //            if (cfmt.measurenb > 0 & s.bar_num
        //             && s.bar_num % cfmt.measurenb != 0)
        //                x += 6;
        //            if (s.type != C.BAR)
        //            {
        //                w = s.rbstop ? 0 : s.x - realwidth + 4;
        //            }
        //            else if ((s.bar_type.Length > 1 // if complex bar
        //                 && s.bar_type != "[]")
        //                || s.bar_type == "]")
        //            {
        //                //				if (s.bar_type == "]")
        //                //					s.invis = true
        //                //fixme:%%staves: cur_sy moved?
        //                if (s1.st > 0
        //                 && !(cur_sy.staves[s1.st - 1].flags & STOP_BAR))
        //                    w = s.wl;
        //                else if (s.bar_type[s.bar_type.Length - 1] == ':')
        //                    w = 12;
        //                else if (s.bar_type[0] != ':')
        //                    //				      || s.bar_type == "]")
        //                    w = 0;        /* explicit repeat end */
        //                else
        //                    w = 8;
        //            }
        //            else
        //            {
        //                w = (s.rbstop && !s.rbstart) ? 0 : 8;
        //            }
        //            w = (s.x - x - w); // / staff_tb[p_voice.st].staffscale;

        //            if (s.next == null // 2nd ending at end of line
        //             && !s.rbstop
        //             && !p_voice.bar_start) // continue on next line
        //            {
        //                p_voice.bar_start = _bar(s);
        //                p_voice.bar_start.bar_type = "";
        //                p_voice.bar_start.rbstart = 1;
        //            }
        //            if (s1.text != null)
        //                xy_str(x + 4, y2 - gene.curfont.size,
        //                    s1.text);
        //            xypath(x, y2);
        //            if (s1.rbstart == 2)
        //                output += 'm0 10v-10';
        //            output += 'h' + w.toFixed(1);
        //            if (s.rbstop == 2)
        //                output += 'v10';
        //            output += '"/>\n';
        //            y_set(s1.st, true, x, w, y + 2);

        //            if (s.rbstart != 0)
        //                s = s.prev;
        //        }
        //    } // draw_repbra()

        //    /* create the decorations tied to the staves */
        //    for (i = 0; i <= nstaff; i++)
        //        minmax[i] = new
        //        {
        //            ymin = 0,
        //            ymax = 0
        //        };
        //    for (i = 0; i < nd; i++)
        //    {
        //        de = a_de[i];
        //        dd = de.dd;
        //        if (dd == null) // if error
        //            continue;
        //        if (f_staff[dd.func] == null /* if not tied to the staff */
        //         || de.m != null // or head decoration
        //         || dd.ty == '<' || dd.ty == '>' || dd.ty == '@')
        //            continue;

        //        f_staff[dd.func](de);
        //        if (dd.func != 6
        //         || dd.dd_en) // if start
        //            continue;

        //        if ((de.pos & C.SL_ALI_MSK) == C.SL_ALIGN
        //         || ((de.pos & C.SL_ALI_MSK) == 0
        //          && de.s.fmt.dynalign > 0)) // if align
        //        {
        //            if (de.up)
        //            {
        //                if (de.y > minmax[de.st].ymax)
        //                    minmax[de.st].ymax = de.y;
        //            }
        //            else
        //            {
        //                if (de.y < minmax[de.st].ymin)
        //                    minmax[de.st].ymin = de.y;
        //            }
        //        }
        //    }

        //    // set the same vertical offset of the dynamic marks
        //    for (i = 0; i < nd; i++)
        //    {
        //        de = a_de[i];
        //        dd = de.dd;
        //        if (dd == null) // if error
        //            continue;

        //        // if @x,y offsets, update the top and bottom of the staff
        //        if (dd.ty == '@')
        //        {
        //            var y2 = 0;

        //            y = de.y;
        //            if (y > 0)
        //            {
        //                y2 = y + dd.h + 2;
        //                if (y2 > staff_tb[de.st].ann_top)
        //                    staff_tb[de.st].ann_top = y2;
        //            }
        //            else
        //            {
        //                y2 = y - dd.hd - 2;
        //                if (y2 < staff_tb[de.st].ann_bot)
        //                    staff_tb[de.st].ann_bot = y2;

        //            }
        //            continue;
        //        }
        //        if (dd.func != 6
        //         || dd.ty == '<' || dd.ty == '>'
        //         || dd.dd_en) // if start
        //            continue;

        //        w = de.val != null ? de.val : (dd.wl + dd.wr);
        //        if ((de.pos & C.SL_ALI_MSK) == C.SL_ALIGN
        //          || ((de.pos & C.SL_ALI_MSK) == 0
        //           && de.s.fmt.dynalign > 0)) // if align
        //        {
        //            if (de.up)
        //                y = minmax[de.st].ymax;
        //            else
        //                y = minmax[de.st].ymin;
        //            de.y = y;
        //        }
        //        else
        //        {
        //            y = de.y;
        //        }
        //        if (de.up)
        //            y += dd.h;
        //        else
        //            y -= dd.hd;
        //        y_set(de.st, de.up, de.x, w, y);
        //    }

        //    // second pass for pedal (under the staff)
        //    for (i = 0; i < nd; i++)
        //    {
        //        de = a_de[i];
        //        dd = de.dd;
        //        if (dd == null) // if error
        //            continue;
        //        if (dd.dd_en // if start
        //         || dd.name.Substring(0, 3) != "ped")
        //            continue;
        //        w = de.val != null ? de.val : 10;
        //        de.y = y_get(de.st, 0, de.x, w)
        //            - (dd.dd_st && cfmt.pedline ? 10 : dd.h);
        //        y_set(de.st, 0, de.x, w, de.y); // (no descent)
        //    }

        //    draw_all_chsy(); // draw all chord symbols

        //    /* draw the repeat brackets */
        //    for (v = 0; v < voice_tb.Length; v++)
        //    {
        //        p_voice = voice_tb[v];
        //        if (p_voice.second || p_voice.sym == null || p_voice.ignore)
        //            continue;
        //        draw_repbra(p_voice);
        //    }
        //}

        /* -- draw the measure bar numbers -- */
        /* (scaled delayed output) */
        public void draw_measnb()
        {
            var s = 0;
            var st = 0;
            var bar_num = 0;
            var x = 0;
            var y = 0;
            var w = 0;
            var any_nb = false;
            var font_size = 0;
            var w0 = 0;
            var sy = cur_sy;

            /* search the top staff */
            for (st = 0; st <= nstaff; st++)
            {
                if (sy.st_print[st])
                    break;
            }
            if (st > nstaff)
                return; // no visible staff
            set_dscale(st);

            /* leave the measure numbers as unscaled */
            if (staff_tb[st].staffscale != 1)
            {
                font_size = get_font("measure").size;
                param_set_font("measurefont", "* " +
                    (font_size / staff_tb[st].staffscale).ToString());
            }
            set_font("measure");
            w0 = cwidf('0'); // (greatest) width of a number

            s = tsfirst; // clef
            bar_num = gene.nbar;
            if (bar_num > 1)
            {
                if (cfmt.measurenb == 0)
                {
                    any_nb = true;
                    y = y_get(st, true, 0, 20);
                    if (y < staff_tb[st].topbar + 14)
                        y = staff_tb[st].topbar + 14;
                    xy_str(0, y - gene.curfont.size * .2, bar_num.ToString());
                    y_set(st, true, 0, 20, y + gene.curfont.size + 2);
                }
                else if (bar_num % cfmt.measurenb == 0)
                {
                    for (; ; s = s.ts_next)
                    {
                        switch (s.type)
                        {
                            case C.CLEF:
                            case C.KEY:
                            case C.METER:
                            case C.STBRK:
                                continue;
                        }
                        break;
                    }

                    // don't display the number twice
                    if (s.type != C.BAR || s.bar_num == 0)
                    {
                        any_nb = true;
                        w = w0;
                        if (bar_num >= 10)
                            w *= bar_num >= 100 ? 3 : 2;
                        if (gene.curfont.pad != 0)
                            w += gene.curfont.pad * 2;
                        x = (s.prev != null
                            ? s.prev.x + s.prev.wr / 2
                            : s.x - s.wl) - w;
                        y = y_get(st, true, x, w) + 5;
                        if (y < staff_tb[st].topbar + 6)
                            y = staff_tb[st].topbar + 6;
                        y += gene.curfont.pad;
                        xy_str(x, y - gene.curfont.size * .2, bar_num.ToString());
                        y += gene.curfont.size + gene.curfont.pad;
                        y_set(st, true, x, w, y);
                        //			s.ymx = y
                    }
                }
            }

            for (; s != null; s = s.ts_next)
            {
                switch (s.type)
                {
                    case C.STAVES:
                        sy = s.sy;
                        for (st = 0; st < nstaff; st++)
                        {
                            if (sy.st_print[st])
                                break;
                        }
                        set_dscale(st);
                        continue;
                    default:
                        continue;
                    case C.BAR:
                        if (s.bar_num == 0 || s.bar_num <= 1)
                            continue;
                        break;
                }

                bar_num = s.bar_num;
                if (cfmt.measurenb == 0
                 || (bar_num % cfmt.measurenb) != 0
                 || s.next == null
                 || s.bar_mrep)
                    continue;
                if (!any_nb)
                    any_nb = true;
                w = w0;
                if (bar_num >= 10)
                    w *= bar_num >= 100 ? 3 : 2;
                if (gene.curfont.pad != 0)
                    w += gene.curfont.pad * 2;
                x = s.x;
                y = y_get(st, true, x, w);
                if (y < staff_tb[st].topbar + 6)
                    y = staff_tb[st].topbar + 6;
                if (s.next.type == C.NOTE)
                {
                    if (s.next.stem > 0)
                    {
                        if (y < s.next.ys - gene.curfont.size)
                            y = s.next.ys - gene.curfont.size;
                    }
                    else
                    {
                        if (y < s.next.y)
                            y = s.next.y;
                    }
                }
                y += 2 + gene.curfont.pad;
                xy_str(x, y - gene.curfont.size * .2, bar_num.ToString());
                y += gene.curfont.size + gene.curfont.pad;
                y_set(st, true, x, w, y);
                //		s.ymx = y
            }
            gene.nbar = bar_num;

            if (font_size != 0)
                param_set_font("measurefont", "* " + font_size.ToString());
        }

        /* -- draw the parts and the tempo information -- */
        // (unscaled delayed output)
        public void draw_partempo()
        {
            var s = 0;
            var s2 = 0;
            var some_part = 0;
            var some_tempo = 0;
            var h = 0;
            var w = 0;
            var y = 0;
            var st = 0;
            var sy = cur_sy;

            // search the top staff
            for (st = 0; st <= nstaff; st++)
            {
                if (sy.st_print[st])
                    break;
            }
            if (st > nstaff)
                return; // no visible staff
            set_dscale(st, 1); // no scale

            /* get the minimal y offset */
            var ymin = staff_tb[st].topbar + 2;
            var dosh = 0;
            var shift = 1;
            var x = -100; // (must be negative for %%soloffs)
            var yn = 0; // y min when x < 0

            // output the parts
            for (s = tsfirst; s != null; s = s.ts_next)
            {
                s2 = s.part;
                if (s2 == null || s2.invis)
                    continue;
                if (some_part == 0)
                {
                    some_part = s;
                    set_font("parts");
                    h = gene.curfont.size + 2 +
                        gene.curfont.pad * 2;
                }
                if (s2.x == null)
                    s2.x = s.x - 10;
                w = strwh(s2.text)[0];
                y = y_get(st, true, s2.x, w + 3);
                if (ymin < y)
                    ymin = y;
            }
            if (some_part != 0)
            {
                set_sscale(-1);
                ymin *= staff_tb[st].staffscale;
                for (s = some_part; s != null; s = s.ts_next)
                {
                    s2 = s.part;
                    if (s2 == null || s2.invis)
                        continue;
                    w = strwh(s2.text)[0];
                    if (user.anno_start || user.anno_stop)
                    {
                        s2.wl = 0;
                        s2.wr = w;
                        s2.ymn = ymin;
                        s2.ymx = s2.ymn + h;
                        anno_start(s2);
                    }
                    xy_str(s2.x,
                        ymin + 2 + gene.curfont.pad + gene.curfont.size * .22,
                        s2.text);
                    y_set(st, 1, s2.x, w + 3, ymin + 2 + h);
                    if (s2.x < 0)
                        yn = ymin + 2 + h;
                    anno_stop(s2);
                }
            }

            // output the tempos
            ymin = staff_tb[st].topbar + 6;
            for (s = tsfirst; s != null; s = s.ts_next)
            {
                if (s.type != C.TEMPO || s.invis)
                    continue;
                if (some_tempo == 0)
                    some_tempo = s;
                w = s.tempo_wh[0];
                //		if (s.time == 0 && s.x > 40)	// at start of tune and no %%soloffs,
                //			s.x = 40	// shift the tempo over the key signature
                y = y_get(st, true, s.x - 16, w);
                if (s.x - 16 < 0)
                    y = yn;
                if (y > ymin)
                    ymin = y;
                if (x >= s.x - 16 && !(dosh & (shift >> 1)))
                    dosh |= shift;
                shift <<= 1;
                x = s.x - 16 + w;
            }
            if (some_tempo != 0)
            {
                set_sscale(-1);
                set_font("tempo");
                h = gene.curfont.size;
                ymin += 2;
                ymin *= staff_tb[st].staffscale;

                /* draw the tempo indications */
                for (s = some_tempo; s != null; s = s.ts_next)
                {
                    if (s.type != C.TEMPO
                     || s.invis) // (displayed by %%titleformat)
                        continue;
                    w = s.tempo_wh[0];
                    y = ymin;
                    if (dosh & 1)
                        y += h;
                    if (user.anno_start || user.anno_stop)
                    {
                        s.wl = 16;
                        //				s.wr = 30;
                        s.wr = w - 16;
                        s.ymn = y;
                        s.ymx = s.ymn + 14;
                        anno_start(s);
                    }
                    writempo(s, s.x - 16, y);
                    anno_stop(s);
                    y_set(st, 1, s.x - 16, w, y + h + 2);
                    dosh >>= 1;
                }
            }
        }





        //        public static Dictionary<string, Staff> staff_tb = new Dictionary<string, Staff>();




        //        public static int YSTEP = 48;
        //        public static int realwidth = 1000;



        //        public static void Main(string[] args)
        //        {

        //                var a_dcn = new List<string>();
        //            var cfmt = new
        //            {
        //                decoerr = true
        //            };
        //            var glovar = new
        //            {
        //                ottava = false
        //            };
        //            var curvoice = new
        //            {
        //                acc = new int[38]
        //            };
        //            var errs = new
        //            {
        //                must_note_rest = "Decoration '$1' must be on a note or rest",
        //                must_note = "Decoration '$1' must be on a note",
        //                bad_val = "Bad value '$1'"
        //            };

        //            var s = new Symbol();
        //            var prev = new Symbol();
        //            var nt1 = new Note();
        //            var nt2 = new Note();
        //            var nm = "";
        //            var dd = new Decoration();
        //            var note = new Note();
        //            var s1 = new Symbol();
        //            var court = false;

        //            while (true)
        //            {
        //                nm = a_dcn[0];
        //                if (string.IsNullOrEmpty(nm))
        //                {
        //                    break;
        //                }
        //                a_dcn.RemoveAt(0);
        //                dd = get_dd(nm);
        //                if (dd == null)
        //                {
        //                    continue;
        //                }

        //                switch (dd.func)
        //                {
        //                    case 0:
        //                        if (s.type == C.BAR && nm == "dot")
        //                        {
        //                            s.bar_dotted = true;
        //                            continue;
        //                        }
        //                        goto case 1;
        //                    case 1:
        //                    case 2:
        //                        if (s.notes == null)
        //                        {
        //                            Console.WriteLine("Error: Decoration '{0}' must be on a note or rest", nm);
        //                            continue;
        //                        }
        //                        break;
        //                    case 4:
        //                    case 5:
        //                        var i = nm.Match(/ 1?[85]([vm])([ab])([()]) /);
        //                        if (i != null)
        //                        {
        //                            var j = i[1] == 'v' ? 1 : 2;
        //                            if (i[2] == 'b')
        //                            {
        //                                j = -j;
        //                            }
        //                            if (s.ottava == null)
        //                            {
        //                                s.ottava = new List<int>();
        //                            }
        //                            s.ottava[i[3] == '(' ? 0 : 1] = j;
        //                            glovar.ottava = true;
        //                        }
        //                        break;
        //                    case 8:
        //                        if (s.type != C.NOTE)
        //                        {
        //                            Console.WriteLine("Error: Decoration '{0}' must be on a note", nm);
        //                            continue;
        //                        }
        //                        note = s.notes[s.nhd];
        //                        if (note.a_dd == null)
        //                        {
        //                            note.a_dd = new List<Decoration>();
        //                        }
        //                        note.a_dd.Add(dd);
        //                        continue;
        //                    case 9:
        //                        if (s.notes == null)
        //                        {
        //                            Console.WriteLine("Error: Decoration '{0}' must be on a note or rest", nm);
        //                            continue;
        //                        }
        //                        for (var j = 0; j <= s.nhd; j++)
        //                        {
        //                            note = s.notes[j];
        //                            note.invis = true;
        //                            if (note.a_dd == null)
        //                            {
        //                                note.a_dd = new List<Decoration>();
        //                            }
        //                            note.a_dd.Add(dd);
        //                        }
        //                        continue;
        //                    case 10:
        //                        if (s.notes != null)
        //                        {
        //                            for (var j = 0; j <= s.nhd; j++)
        //                            {
        //                                s.notes[j].color = nm;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            s.color = nm;
        //                        }
        //                        break;
        //                    case 32:
        //                        s.invis = true;
        //                        break;
        //                    case 33:
        //                        if (s.type != C.BAR)
        //                        {
        //                            Console.WriteLine("Error: !beamon! must be on a bar");
        //                            continue;
        //                        }
        //                        s.beam_on = true;
        //                        break;
        //                    case 34:
        //                        if (s.type != C.NOTE || prev == null || prev.type != C.NOTE || s.dur != prev.dur)
        //                        {
        //                            Console.WriteLine("Error: !{0}! must be on the last of a couple of notes", nm);
        //                            continue;
        //                        }
        //                        s.trem2 = true;
        //                        s.beam_end = true;
        //                        s.beam_st = false;
        //                        prev.beam_st = true;
        //                        prev.beam_end = false;
        //                        s.ntrem = prev.ntrem = int.Parse(nm[4].ToString());
        //                        for (var j = 0; j <= s.nhd; j++)
        //                        {
        //                            s.notes[j].dur *= 2;
        //                        }
        //                        for (var j = 0; j <= prev.nhd; j++)
        //                        {
        //                            prev.notes[j].dur *= 2;
        //                        }
        //                        break;
        //                    case 35:
        //                        if (s.type != C.NOTE)
        //                        {
        //                            Console.WriteLine("Error: Decoration '{0}' must be on a note", nm);
        //                            continue;
        //                        }
        //                        s.xstem = true;
        //                        break;
        //                    case 36:
        //                        if (s.type != C.NOTE)
        //                        {
        //                            Console.WriteLine("Error: Decoration '{0}' must be on a note", nm);
        //                            continue;
        //                        }
        //                        if (nm[6] == '1')
        //                        {
        //                            s.beam_br1 = true;
        //                        }
        //                        else
        //                        {
        //                            s.beam_br2 = true;
        //                        }
        //                        break;
        //                    case 37:
        //                        s.rbstop = 1;
        //                        break;
        //                    case 38:
        //                        if (s.type != C.NOTE)
        //                        {
        //                            Console.WriteLine("Error: Decoration '{0}' must be on a note", nm);
        //                            continue;
        //                        }
        //                        s.trem1 = true;
        //                        s.ntrem = nm.Length;
        //                        break;
        //                    case 39:
        //                        if (s.type != C.NOTE)
        //                        {
        //                            Console.WriteLine("Error: Decoration '{0}' must be on a note", nm);
        //                            continue;
        //                        }
        //                        s.feathered_beam = nm[5] == 'a' ? 1 : -1;
        //                        break;
        //                    case 40:
        //                        s.stemless = true;
        //                        break;
        //                    case 41:
        //                        s.rbstop = 2;
        //                        break;
        //                    case 42:
        //                        if (!s.notes[0].acc)
        //                        {
        //                            continue;
        //                        }
        //                        nm = "sacc" + s.notes[0].acc.ToString();
        //                        dd = dd_tb[nm];
        //                        if (dd == null)
        //                        {
        //                            dd = deco_def(nm);
        //                            if (dd == null)
        //                            {
        //                                Console.WriteLine("Error: Bad value '!editorial!'");
        //                                continue;
        //                            }
        //                        }
        //                        s.notes[0].acc = null;
        //                        curvoice.acc[s.notes[0].pit + 19] = 0;
        //                        break;
        //                    case 43:
        //                        j = curvoice.acc[s.notes[0].pit + 19];
        //                        if (s.notes[0].acc || j == 0)
        //                        {
        //                            continue;
        //                        }
        //                        court = true;
        //                        break;
        //                    case 44:
        //                        do_ctie(nm, s, s.notes[0]);
        //                        continue;
        //                }

        //                if (s.a_dd == null)
        //                {
        //                    s.a_dd = new List<Decoration>();
        //                }
        //                s.a_dd.Add(dd);
        //            }

        //            if (court)
        //            {
        //                a_dcn.Add("cacc" + j);
        //                dh_cnv(s, s.notes[0]);
        //            }
        //        }


        //    /*********************************************/


        //            static Dictionary<string, dynamic> dd_tb = new Dictionary<string, dynamic>(); // definition of the decorations
        //        static dynamic a_de;                    // array of the decoration elements
        //        static dynamic cross;                   // cross voice decorations

        //        static Dictionary<string, string> decos = new Dictionary<string, string>()
        //        {
        //            { "dot", "0 stc 6 1.5 1" },
        //            { "tenuto", "0 emb 6 4 3" },
        //            { "slide", "1 sld 3 7 1" }
        //        };

        //            static dynamic[] f_near = new dynamic[]
        //            {
        //            d_near,     // 0 - near the note
        //            d_slide,    // 1
        //            d_arp       // 2
        //            };

        //            static dynamic[] f_note = new dynamic[]
        //            {
        //            null, null, null,
        //            d_upstaff,  // 3 - tied to note
        //            d_upstaff   // 4 (below the staff)
        //            };

        //            static dynamic[] f_staff = new dynamic[]
        //            {
        //            null, null, null, null, null,
        //            d_upstaff,  // 5 (above the staff)
        //            d_upstaff,  // 6 - tied to staff (dynamic marks)
        //            d_upstaff   // 7 (below the staff)
        //            };



        ///*******************************************************/

        //        public  Dictionary<string, string> dd_tb = new Dictionary<string, string>();
        //        public  Action<object> a_de;
        //        public  Action<object> cross;

        //        public  Dictionary<string, string> decos = new Dictionary<string, string>()
        //    {
        //        {"dot", "0 stc 6 1.5 1"},
        //        {"tenuto", "0 emb 6 4 3"}
        //    };

        //        public  List<Action<object>> f_near = new List<Action<object>>()
        //    {
        //        d_test1, d_test2, d_test3
        //    };

        //        public  List<Action<object>> f_note = new List<Action<object>>()
        //    {
        //        null, null, null,
        //        d_test4,
        //        d_test4
        //    };

        //        public  List<Action<object>> f_staff = new List<Action<object>>()
        //    {
        //        null, null, null, null, null,
        //        d_test4,
        //        d_test4,
        //        d_test4
        //    };

        //        public  void d_test1(object de)
        //        {
        //            var d = (dynamic)de;
        //            if (d.start) { d_trill(d); return; }
        //        }

        //        public  void d_test2(object de)
        //        {
        //            var d = (dynamic)de;
        //            if (d.start) { d_trill(d); return; }
        //        }

        //        public  void d_test3(object de)
        //        {
        //            var d = (dynamic)de;
        //            if (d.start) { d_trill(d); return; }
        //        }

        //        public  void d_test4(object de)
        //        {
        //            var d = (dynamic)de;
        //            if (d.start) { d_trill(d); return; }
        //        }




        //        static Dictionary<string, string> dd_tb = new Dictionary<string, string>();
        //        static Action<object> a_de;
        //        static Action<object> cross;

        //        static Dictionary<string, string> decos = new Dictionary<string, string>
        //    {
        //        { "dot", "0 stc 6 1.5 1" },
        //        { "tenuto", "0 emb 6 4 3" }
        //    };

        //        static List<Action<object>> f_near = new List<Action<object>> { d_test1, d_test2, d_test3 };
        //        static List<Action<object>> f_note = new List<Action<object>> { null, null, null, d_test4, d_test4 };
        //        static List<Action<object>> f_staff = new List<Action<object>> { null, null, null, null, null, d_test4, d_test4, d_test4 };

        //        static void d_test1(object de)
        //        {
        //            dynamic deDynamic = de;
        //            if (deDynamic.start) { d_trill(de); return; }
        //        }

        //        static void d_test2(object de)
        //        {
        //            dynamic deDynamic = de;
        //            if (deDynamic.start) { d_trill(de); return; }
        //        }

        //        static void d_test3(object de)
        //        {
        //            dynamic deDynamic = de;
        //            if (deDynamic.start) { d_trill(de); return; }
        //        }

        //        static void d_test4(object de)
        //        {
        //            dynamic deDynamic = de;
        //            if (deDynamic.start) { d_trill(de); return; }
        //        }

        //        static void d_trill(object de)
        //        {
        //            // Implementation of d_trill
        //        }

        //        static void draw_measnb()
        //        {
        //            dynamic cur_sy = null; // Placeholder for cur_sy
        //            dynamic tsfirst = null; // Placeholder for tsfirst
        //            dynamic gene = new { nbar = 0, curfont = new { size = 0, pad = 0 } }; // Placeholder for gene
        //            dynamic cfmt = new { measurenb = 0 }; // Placeholder for cfmt
        //            dynamic staff_tb = new List<dynamic>(); // Placeholder for staff_tb
        //            int nstaff = 0; // Placeholder for nstaff

        //            VoiceItem s, st, bar_num, x, y, w, any_nb, font_size, w0;
        //            dynamic sy = cur_sy;

        //            // search the top staff
        //            for (st = 0; st <= nstaff; st++)
        //            {
        //                if (sy.st_print[st])
        //                    break;
        //            }
        //            if (st > nstaff)
        //                return; // no visible staff
        //            set_dscale(st);

        //            // leave the measure numbers as unscaled
        //            if (staff_tb[st].staffscale != 1)
        //            {
        //                font_size = get_font("measure").size;
        //                param_set_font("measurefont", "* " + (font_size / staff_tb[st].staffscale).ToString());
        //            }
        //            set_font("measure");
        //            w0 = cwidf('0'); // (greatest) width of a number

        //            s = tsfirst; // clef
        //            bar_num = gene.nbar;
        //            if (bar_num > 1)
        //            {
        //                if (cfmt.measurenb == 0)
        //                {
        //                    any_nb = true;
        //                    y = y_get(st, true, 0, 20);
        //                    if (y < staff_tb[st].topbar + 14)
        //                        y = staff_tb[st].topbar + 14;
        //                    xy_str(0, y - gene.curfont.size * .2, bar_num.ToString());
        //                    y_set(st, true, 0, 20, y + gene.curfont.size + 2);
        //                }
        //                else if (bar_num % cfmt.measurenb == 0)
        //                {
        //                    for (; ; s = s.ts_next)
        //                    {
        //                        switch (s.type)
        //                        {
        //                            case C.CLEF:
        //                            case C.KEY:
        //                            case C.METER:
        //                            case C.STBRK:
        //                                continue;
        //                        }
        //                        break;
        //                    }

        //                    // don't display the number twice
        //                    if (s.type != C.BAR || !s.bar_num)
        //                    {
        //                        any_nb = true;
        //                        w = w0;
        //                        if (bar_num >= 10)
        //                            w *= bar_num >= 100 ? 3 : 2;
        //                        if (gene.curfont.pad)
        //                            w += gene.curfont.pad * 2;
        //                        x = (s.prev != null
        //                            ? s.prev.x + s.prev.wr / 2
        //                            : s.x - s.wl) - w;
        //                        y = y_get(st, true, x, w) + 5;
        //                        if (y < staff_tb[st].topbar + 6)
        //                            y = staff_tb[st].topbar + 6;
        //                        y += gene.curfont.pad;
        //                        xy_str(x, y - gene.curfont.size * .2, bar_num.ToString());
        //                        y += gene.curfont.size + gene.curfont.pad;
        //                        y_set(st, true, x, w, y);
        //                    }
        //                }
        //            }

        //            for (; s != null; s = s.ts_next)
        //            {
        //                switch (s.type)
        //                {
        //                    case C.STAVES:
        //                        sy = s.sy;
        //                        for (st = 0; st < nstaff; st++)
        //                        {
        //                            if (sy.st_print[st])
        //                                break;
        //                        }
        //                        set_dscale(st);
        //                        continue;
        //                    default:
        //                        continue;
        //                    case C.BAR:
        //                        if (!s.bar_num || s.bar_num <= 1)
        //                            continue;
        //                        break;
        //                }

        //                bar_num = s.bar_num;
        //                if (cfmt.measurenb == 0
        //                 || (bar_num % cfmt.measurenb) != 0
        //                 || s.next == null
        //                 || s.bar_mrep)
        //                    continue;
        //                if (!any_nb)
        //                    any_nb = true;
        //                w = w0;
        //                if (bar_num >= 10)
        //                    w *= bar_num >= 100 ? 3 : 2;
        //                if (gene.curfont.pad)
        //                    w += gene.curfont.pad * 2;
        //                x = s.x;
        //                y = y_get(st, true, x, w);
        //                if (y < staff_tb[st].topbar + 6)
        //                    y = staff_tb[st].topbar + 6;
        //                if (s.next.type == C.NOTE)
        //                {
        //                    if (s.next.stem > 0)
        //                    {
        //                        if (y < s.next.ys - gene.curfont.size)
        //                            y = s.next.ys - gene.curfont.size;
        //                    }
        //                    else
        //                    {
        //                        if (y < s.next.y)
        //                            y = s.next.y;
        //                    }
        //                }
        //                y += 2 + gene.curfont.pad;
        //                xy_str(x, y - gene.curfont.size * .2, bar_num.ToString());
        //                y += gene.curfont.size + gene.curfont.pad;
        //                y_set(st, true, x, w, y);
        //            }
        //            gene.nbar = bar_num;

        //            if (font_size != null)
        //                param_set_font("measurefont", "* " + font_size.ToString());
        //        }

        //        static void set_dscale(dynamic st)
        //        {
        //            // Implementation of set_dscale
        //        }

        //        static dynamic get_font(string fontName)
        //        {
        //            // Implementation of get_font
        //            return new { size = 0 };
        //        }

        //        static void param_set_font(string fontName, string value)
        //        {
        //            // Implementation of param_set_font
        //        }

        //        static void set_font(string fontName)
        //        {
        //            // Implementation of set_font
        //        }

        //        static int cwidf(char c)
        //        {
        //            // Implementation of cwidf
        //            return 0;
        //        }

        //        static int y_get(dynamic st, bool flag, int x, int w)
        //        {
        //            // Implementation of y_get
        //            return 0;
        //        }

        //        static void xy_str(int x, int y, string str)
        //        {
        //            // Implementation of xy_str
        //        }

        //        static void y_set(dynamic st, bool flag, int x, int w, int y)
        //        {
        //            // Implementation of y_set
        //        }



        //    static class C
        //    {
        //        public const int CLEF = 1;
        //        public const int KEY = 2;
        //        public const int METER = 3;
        //        public const int STBRK = 4;
        //        public const int BAR = 5;
        //        public const int NOTE = 6;
        //        public const int STAVES = 7;
        //    }


        ///***********************************************/

        //        static Dictionary<string, string> dd_tb = new Dictionary<string, string>();
        //        static Action<object> a_de;
        //        static object cross;

        //        static Dictionary<string, string> decos = new Dictionary<string, string>()
        //    {
        //        { "dot", "0 stc 6 1.5 1" },
        //        { "tenuto", "0 emb 6 4 3" }
        //    };

        //        static List<Action<object>> f_near = new List<Action<object>>()
        //    {
        //        d_test1, d_test2, d_test3
        //    };

        //        static List<Action<object>> f_note = new List<Action<object>>()
        //    {
        //        null, null, null,
        //        d_test4,
        //        d_test4
        //    };

        //        static List<Action<object>> f_staff = new List<Action<object>>()
        //    {
        //        null, null, null, null, null,
        //        d_test4,
        //        d_test4,
        //        d_test4
        //    };

        //        static void d_test1(object de)
        //        {
        //            if (de.start) { d_trill(de); return; }
        //        }

        //        static void d_test2(object de)
        //        {
        //            if (de.start) { d_trill(de); return; }
        //        }

        //        static void d_test3(object de)
        //        {
        //            if (de.start) { d_trill(de); return; }
        //        }

        //        static void d_test4(object de)
        //        {
        //            if (de.start) { d_trill(de); return; }
        //        }




    }


}