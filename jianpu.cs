using System;
using System.Collections.Generic;
using System.Text;

namespace autocad_part2
{
    partial  class Abc
    {

        public class Jianpu
        {
            public List<int> befAcc { get; set; }
            public List<string> k_tb { get; set; }
            public int[] abc2midiKey = { 0, 2, 4, 5, 7, 9, 11 };
            public int[] midi2jianpu = {-1, 6, 1, 8, 3, 10, 5,
                            0,
                            7, 2, 9, 4, 11, 6, 1 };
            public (int num, int acc)[] midi2jianpuSharp =//升記號用
                    { ( num: 1,acc:0 ), ( num: 1, acc: 1 ),
                    ( num: 2,acc:0 ), ( num: 2, acc: 1 ),
                    ( num: 3,acc:0 ),
                    ( num: 4,acc:0 ), ( num: 4, acc: 1 ),
                    ( num: 5,acc:0 ), ( num: 5, acc: 1 ),
                    ( num: 6,acc:0 ), ( num: 6, acc: 1 ),
                    ( num: 7,acc:0 )};
            public (int num, int acc)[] midi2jianpuFlat =//降記號用
                    {   (num: 1,acc:0 ), (num: 1, acc: -1 ),
                        (num: 2,acc:0 ), (num: 2, acc: -1 ),
                        (num: 3,acc:0 ),
                        (num: 4,acc:0 ), (num: 4, acc: -1 ),
                        (num: 5,acc:0 ), (num: 5, acc: -1 ),
                        (num: 6,acc:0 ), (num: 6, acc: -1 ),
                        (num: 7,acc:0 )};
            public int[] cde2fcg = { 0, 2, 4, -1, 1, 3, 5 };
            public int[] cgd2cde ={0, -4, -1, -5, -2, -6, -3,
        0, -4, -1, -5, -2, -6, -3, 0};
            public int[] acc2 = { -2, -1, 3, 1, 2 };
            public string[] acc_tb = { "\ue264", "\ue260", , "\ue262", "\ue263", "\ue261" };
            // don't calculate the beams
            public void calc_beam(Func<object, object, object> of, object bm, object s1)
            {
                if (!s1.p_v.jianpu)
                    return of(bm, s1);
            }

            // adjust some symbols before the generation
            public void output_music(Func<object> of)
            {
                var p_v = new object();
                var v = new object();
                var C = new object();
                var abc = new object();
                var cur_sy = new object();
                var voice_tb = new object();

                void ov_def(object v)
                {
                    var s1 = new object();
                    var tim = new object();
                    var s = p_v.sym;
                    while (s != null)
                    {
                        s1 = s.ts_prev;
                        if (!s.invis && s.dur && s1.v != v && s1.st == s.st && s1.time == s.time)
                        {
                            while (true)
                            {
                                if (s1.prev == null || s1.prev.bar_type)
                                    break;
                                s1 = s1.prev;
                            }
                            while (!s1.bar_type)
                            {
                                s1.dy = 14;
                                s1.notes[0].pit = 30;
                                if (s1.type == C.REST)
                                    s1.combine = -1;
                                s1 = s1.next;
                            }
                            while (true)
                            {
                                s.dy = -14;
                                s.notes[0].pit = 20;
                                if (s.next == null || s.next.bar_type || s.next.time >= s1.time)
                                    break;
                                s = s.next;
                            }
                        }
                        s = s.next;
                    }
                }

                void SetHead()
                {
                    var v = new object();
                    var p_v = new object();
                    var mt = new object();
                    var s2 = new object();
                    var sk = new object();
                    var s = new object();
                    var tsfirst = abc.get_tsfirst();
                    for (v = 0; v < voice_tb.length; v++)
                    {
                        p_v = voice_tb[v];
                        if (p_v.jianpu)
                            break;
                    }
                    if (v >= voice_tb.length)
                        return;
                    mt = p_v.meter.a_meter[0];
                    sk = p_v.key;
                    s2 = p_v.sym;
                    s = new
                    {
                        type = C.BLOCK,
                        subtype = "text",
                        time = s2.time,
                        dur = 0,
                        v = 0,
                        p_v = p_v,
                        st = 0,
                        fmt = s2.fmt,
                        seqst = true,
                        text = (sk.k_mode + 1) + "=" + (abc2svg.jianpu.k_tb[sk.k_sf + 7 + abc2svg.jianpu.cde2fcg[sk.k_mode]]),
                        font = abc.get_font("text")
                    };
                    if (mt)
                        s.text += ' ' + (mt.bot ? (mt.top + '/' + mt.bot) : mt.top);
                    s2 = tsfirst;
                    s.next = s2.next;
                    if (s.next != null)
                        s.next.prev = s;
                    s.prev = s2;
                    s2.next = s;
                    s.ts_next = s2.ts_next;
                    s.ts_next.ts_prev = s;
                    s.ts_prev = s2;
                    s2.ts_next = s;
                }

                void Slice(object s)
                {
                    var n = new object();
                    var s2 = new object();
                    var s3 = new object();
                    var jn = s.type == C.REST ? 0 : 8;
                    if (s.dur >= C.BLEN)
                        n = 3;
                    else if (s.dur == C.BLEN / 2)
                        n = 1;
                    else
                        n = 2;
                    s.notes[0].dur = s.dur = s.dur_orig = C.BLEN / 4;
                    s.fmr = null;
                    while (--n >= 0)
                    {
                        s2 = new
                        {
                            type = C.REST,
                            v = s.v,
                            p_v = s.p_v,
                            st = s.st,
                            dur = C.BLEN / 4,
                            dur_orig = C.BLEN / 4,
                            fmt = s.fmt,
                            stem = 0,
                            multi = 0,
                            nhd = 0,
                            notes = new[]
                            {
                            new
                            {
                                dur = s.dur,
                                pit = s.notes[0].pit,
                                jn = jn
                            }
                        },
                            xmx = 0,
                            noplay = true,
                            time = s.time + C.BLEN / 4,
                            prev = s,
                            next = s.next
                        };
                        s.next = s2;
                        if (s2.next != null)
                            s2.next.prev = s2;
                        if (s.ts_next == null)
                        {
                            s.ts_next = s2;
                            if (s.soln)
                                s.soln = false;
                            s2.ts_prev = s;
                            s2.seqst = true;
                        }
                        else
                        {
                            for (s3 = s.ts_next; s3 != null; s3 = s3.ts_next)
                            {
                                if (s3.time < s2.time)
                                    continue;
                                if (s3.time > s2.time)
                                {
                                    s2.seqst = true;
                                    s3 = s3.ts_prev;
                                }
                                s2.ts_next = s3.ts_next;
                                s2.ts_prev = s3;
                                if (s2.ts_next != null)
                                    s2.ts_next.ts_prev = s2;
                                s3.ts_next = s2;
                                break;
                            }
                        }
                        s = s2;
                    }
                }

                void SetNote(object s, object sf)
                {
                    var i = new object();
                    var m = new object();
                    var note = new object();
                    var p = new object();
                    var pit = new object();
                    var a = new object();
                    var nn = new object();
                    var delta = abc2svg.jianpu.cgd2cde[sf + 7] - 2;
                    var mn = (note.pit + 77 - 2) % 7;
                    var mo = (((note.pit + 77 - 2) / 7) | 0) - 13;
                    if (abc2svg.jianpu.befAcc[note.pit] && !note.acc)
                    {
                        macc = abc2svg.jianpu.befAcc[note.pit];
                        note.acc = abc2svg.jianpu.befAcc[note.pit];
                    }
                    else if (note.acc == 3)
                    {
                        macc = 0;
                        abc2svg.jianpu.befAcc[note.pit] = null;
                    }
                    else if (note.acc)
                    {
                        macc = note.acc;
                        abc2svg.jianpu.befAcc[note.pit] = macc;
                    }
                    else
                    {
                        macc = note.acc != null ? note.acc : 0;
                    }
                    mp = 60 + 12 * mo + abc2svg.jianpu.abc2midiKey[mn] + macc;
                    mp = mp - m2j;
                    note.pit = 25;
                    if (note.acc == 3)
                    {
                        note.jn = abc2svg.jianpu.midi2jianpuSharp[mp % 12].num;
                        note.acc = 3;
                        note.jo = (mp / 12 | 0) - 5;
                    }
                    else if ((note.acc || sf) >= 0)
                    {
                        note.jn = abc2svg.jianpu.midi2jianpuSharp[mp % 12].num;
                        note.acc = abc2svg.jianpu.midi2jianpuSharp[mp % 12].acc;
                        note.jo = (mp / 12 | 0) - 5;
                    }
                    else
                    {
                        note.jn = abc2svg.jianpu.midi2jianpuFlat[mp % 12].num;
                        note.acc = abc2svg.jianpu.midi2jianpuFlat[mp % 12].acc;
                        note.jo = (mp / 12 | 0) - 5;
                    }
                    if (note.sls != null)
                    {
                        for (i = 0; i < note.sls.length; i++)
                            note.sls[i].ty = C.SL_ABOVE;
                    }
                    if (note.tie_ty != null)
                        note.tie_ty = C.SL_ABOVE;
                }

                void SetSym(object p_v)
                {
                    var s = new object();
                    var g = new object();
                    var sf = p_v.key.k_sf;
                    p_v.key.k_a_acc = null;
                    s = p_v.clef;
                    s.invis = true;
                    s.clef_type = 't';
                    s.clef_line = 2;
                    for (s = p_v.sym; s != null; s = s.next)
                    {
                        s.st = p_v.st;
                        switch (s.type)
                        {
                            case C.BAR:
                                abc2svg.jianpu.befAcc = new List<int>();
                                break;
                            case C.CLEF:
                                s.invis = true;
                                s.clef_type = 't';
                                s.clef_line = 2;
                                break;
                            default:
                                continue;
                            case C.KEY:
                                sf = s.k_sf;
                                s.a_gch = new[]
                                {
                                new
                                {
                                    type = '@',
                                    font = abc.get_font("annotation"),
                                    wh = new[] { 10, 10 },
                                    x = -5,
                                    y = 26,
                                    text = (s.k_mode + 1) + "=" + (abc2svg.jianpu.k_tb[sf + 7 + abc2svg.jianpu.cde2fcg[s.k_mode]])
                                }
                            };
                                break;
                            case C.REST:
                                if (s.notes[0].jn != null)
                                    continue;
                                s.notes[0].jn = 0;
                                if (s.dur >= C.BLEN / 2 && !s.invis)
                                    Slice(s);
                                break;
                            case C.NOTE:
                                SetNote(s, sf);
                                break;
                            case C.GRACE:
                                for (g = s.extra; g != null; g = g.next)
                                    SetNote(g, sf);
                                break;
                        }
                    }
                }

                SetHead();
                for (v = 0; v < voice_tb.length; v++)
                {
                    p_v = voice_tb[v];
                    if (p_v.jianpu)
                    {
                        SetSym(p_v);
                        if (p_v.second)
                            ov_def(v);
                    }
                }
                of();
            }

            public void draw_symbols(Func<object, object> of, object p_voice)
            {
                var s = new object();
                var s2 = new object();
                var nl = new object();
                var y = new object();
                var C = new object();
                var abc = new object();
                var dot = "\ue1e7";
                var staff_tb = abc.get_staff_tb();
                var out_svg = abc.out_svg;
                var out_sxsy = abc.out_sxsy;
                var xypath = abc.xypath;
                if (!p_voice.jianpu)
                {
                    of(p_voice);
                    return;
                }
                for (s = p_voice.sym; s != null; s = s.next)
                {
                    if (s.invis)
                        continue;
                    switch (s.type)
                    {
                        case C.METER:
                            abc.draw_meter(s);
                            break;
                        case C.NOTE:
                        case C.REST:
                            DrawNote(s);
                            break;
                        case C.GRACE:
                            for (var g = s.extra; g != null; g = g.next)
                                DrawNote(g);
                            break;
                    }
                }
                for (s = p_voice.sym; s != null; s = s.next)
                {
                    if (s.invis)
                        continue;
                    switch (s.type)
                    {
                        case C.NOTE:
                        case C.REST:
                            nl = s.nflags;
                            if (nl <= 0)
                                continue;
                            y = staff_tb[s.st].y;
                            s2 = s;
                            while (s.next != null && s.next.nflags > 0)
                            {
                                s = s.next;
                                if (s.nflags > nl)
                                    nl = s.nflags;
                                if (s.beam_end)
                                    break;
                            }
                            if (s.dy != null)
                                y += s.dy;
                            DrawDur(s2, s2.x, y, s, 1, nl);
                            break;
                    }
                }
            }

            // set some parameters
            public void set_fmt(Func<object, object, object> of, object cmd, object param)
            {
                if (cmd == "jianpu")
                {
                    this.set_v_param("jianpu", param);
                    return;
                }
                of(cmd, param);
            }

            // adjust some values
            public void set_pitch(Func<object, object> of, object last_s)
            {
                of(last_s);
                if (last_s == null)
                    return;
                var C = abc2svg.C;
                for (var s = this.get_tsfirst(); s != null; s = s.ts_next)
                {
                    if (!s.p_v.jianpu)
                        continue;
                    switch (s.type)
                    {
                        case C.KEY:
                            if (s.prev.type == C.CLEF || s.v != 0)
                                s.a_gch = null;
                            break;
                        case C.NOTE:
                            s.ymx = 20 * s.nhd + 22;
                            if (s.notes[s.nhd].jo > 2)
                            {
                                s.ymx += 3;
                                if (s.notes[s.nhd].jo > 3)
                                    s.ymx += 2;
                            }
                            s.ymn = 0;
                            break;
                    }
                }
            }

            public void set_vp(System.Action<string[]> of, string[] a)
            {
                int i;
                var p_v = GetCurvoice();
                for (i = 0; i < a.Length; i++)
                {
                    if (a[i] == "jianpu=")
                    {
                        p_v.jianpu = GetBool(a[++i]);
                        if (p_v.jianpu)
                            SetVp(new string[]
                            {
                            "staffsep=", "20",
                            "sysstaffsep=", "14",
                            "stafflines=", "...",
                            "tuplets=", "0 1 0 1"
                            });
                        break;
                    }
                }
                of(a);
            }

            // set the width of some symbols
            public static void set_width(System.Action<Abc2SvgSymbol> of, Abc2SvgSymbol s)
            {
                of(s);
                if (s.p_v == null || !s.p_v.jianpu)
                    return;
                float w, m;
                Abc2SvgNote note;
                var C = Abc2Svg.C;
                switch (s.type)
                {
                    case C.CLEF:
                    case C.KEY:
                        s.wl = s.wr = 0.1f; // (must not be null)
                        break;
                    case C.NOTE:
                        for (m = 0; m <= s.nhd; m++)
                        {
                            note = s.notes[(int)m];
                            if (note.acc && s.wl < 14) // room for the accidental
                                s.wl = 14;
                        }
                        break;
                }
            }

            public static void SetHooks(Abc2Svg abc)
            {
                abc.calculate_beam = Abc2Svg.Jianpu.CalcBeam(abc.calculate_beam);
                abc.DrawSymbols = Abc2Svg.Jianpu.DrawSymbols(abc.DrawSymbols);
                abc.OutputMusic = Abc2Svg.Jianpu.OutputMusic(abc.OutputMusic);
                abc.SetFormat = Abc2Svg.Jianpu.SetFmt(abc.SetFormat);
                abc.SetPitch = Abc2Svg.Jianpu.SetPitch(abc.SetPitch);
                abc.SetVp = Abc2Svg.Jianpu.SetVp(abc.SetVp);
                abc.SetWidth = Abc2Svg.Jianpu.SetWidth(abc.SetWidth);

                // big staccato dot
                abc.GetGlyphs().gstc = "<circle id=\"gstc\" cx=\"0\" cy=\"-3\" r=\"2\"/>";
                abc.GetDecos().gstc = "0 gstc 5 1 1";
                abc.AddStyle("\n.fj{font:15px sans-serif}");

                if (abc.decos)
                {    //新增加 字符號
                    abc.decos['ustenuto'] = "0 emb 6 4 3"
                    abc.decos['uswedge'] = "0 wedge 0 0 0"
                    abc.decos['uswedge1'] = "0 wedge 1,10 -20 1"
                    abc.decos['uswedge2'] = "0 wedge 10,20 -1 1"
                    abc.decos['uswedge3'] = "0 wedge 20,1 -30 1"
        
            abc.decos['usslide1'] = "0 sld -1,10 -10 1"
                    abc.decos['usslide2'] = "0 sld -10,20 -1 1"
                    abc.decos['usslide3'] = "0 sld -20,30 -20 1"
                    abc.decos['shake'] = "0 wedge -10,10 -10 1" //搖指
            //第一個字符 func 說明
            // 0:near the note(dot, tenuto)     音符附近（點、緩音）
            // 1: special case for slide        幻燈片專用案例
            // 2: special case for arpeggio     琶音的特殊情況
            // 3, 4: (below the staff)          （五線譜下方）
            // 5: (above the staff)             （五線譜上方）
            // 6: tied to staff(dynamic marks)  綁法杖（動態標記）
            // 7: (below the staff)             （五線譜下方）
                }
            }
        }
    }
    }




