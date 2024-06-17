using System;
using System.Collections.Generic;
using System.Text;


//var par_sy, cur_sy, voice_tb, curvoice, staves_found, vover, tsfirst
//var w_tb = new Uint8Array([0, 0, 0, 0])

namespace autocad_part2
{
    partial class Abc
    {
        VoiceStavesSymbols par_sy = new VoiceStavesSymbols();
        VoiceStavesSymbols cur_sy = new VoiceStavesSymbols();
        List< PageVoiceTune> voice_tb = new List<PageVoiceTune>();
        PageVoiceTune curvoice = new PageVoiceTune();
        var staves_found = new object();
        var vover = new object();
        VoiceItem tsfirst = new VoiceItem();

        /* -- sort all symbols by time and vertical sequence -- */
        // weight of the symbols !! depends on the symbol type !!
        byte[] w_tb = new byte[] {
                6,  // bar
                2,  // clef
                8,  // custos
                6,  // sm (sequence marker, after bar)
                0,  // grace (must be null)
                3,  // key
                4,  // meter
                9,  // mrest
                9,  // note
                0,  // part
                9,  // rest
                5,  // space (before bar)
                0,  // staves
                1,  // stbrk
                0,  // tempo
                0,  // (free)
                0,  // block
                0   // remark
            };

        /* apply the %%voice options of the current voice */
        void voice_filter()
        {
            object opt;

            void vfilt(object[] opts, object opt)
            {
                object i;
                var sel = new System.Text.RegularExpressions.Regex(opt.ToString());

                if (sel.IsMatch(curvoice.id.ToString())
                    || sel.IsMatch(curvoice.nm.ToString()))
                {
                    for (i = 0; i < opts.Length; i++)
                        self.do_pscom(opts[i]);
                }
            }

            // global
            if (parse.voice_opts != null)
                foreach (var kvp in parse.voice_opts)
                {
                    opt = kvp.Key;
                    if (parse.voice_opts.ContainsKey(opt))
                        vfilt(parse.voice_opts[opt], opt);
                }

            // tune
            if (parse.tune_v_opts != null)
                foreach (var kvp in parse.tune_v_opts)
                {
                    opt = kvp.Key;
                    if (parse.tune_v_opts.ContainsKey(opt))
                        vfilt(parse.tune_v_opts[opt], opt);
                }
        }

        /* -- link a ABC symbol into the current voice -- */
        // if a voice is ignored (not in %%staves) don't link the symbol
        //	but update the time for P: and Q:
        void sym_link(object s)
        {
            var tim = curvoice.time;

            if (s.fname == null)
                set_ref(s);
            if (curvoice.ignore == null)
            {
                s.prev = curvoice.last_sym;
                if (curvoice.last_sym != null)
                    curvoice.last_sym.next = s;
                else
                    curvoice.sym = s;
            }
            curvoice.last_sym = s;
            s.v = curvoice.v;
            s.p_v = curvoice;
            s.st = curvoice.cst;
            s.time = tim;
            if (s.dur != null && s.grace == null)
                curvoice.time += s.dur;
            parse.ufmt = true;
            s.fmt = cfmt;                // global parameters
            s.pos = curvoice.pos;
            if (curvoice.second != null)
                s.second = true;
            if (curvoice.floating != null)
                s.floating = true;
            if (curvoice.eoln != null)
            {
                s.soln = true;
                curvoice.eoln = false;
            }
        }

        /* -- add a new symbol in a voice -- */
        object sym_add(object p_voice, object type)
        {
            var s = new Dictionary<string, object>
                {
                    { "type", type },
                    { "dur", 0 }
                };
            object s2;
            object p_voice2 = curvoice;

            curvoice = p_voice;
            sym_link(s);
            curvoice = p_voice2;
            s2 = s.prev;
            if (s2 == null)
                s2 = s.next;
            if (s2 != null)
            {
                s["fname"] = s2["fname"];
                s["istart"] = s2["istart"];
                s["iend"] = s2["iend"];
            }
            return s;
        }

        void sort_all()
        {
            VoiceItem s, s2, prev;
            List<VoiceItem> vtb = new List<VoiceItem>();
            PageVoiceTune p_voice;
            int time, w, wmin, ir, fmt, v, 
                fl, 
                nv = voice_tb.Length,
                vn = new object[nv];            // voice indexed by range
            VoiceStavesSymbols sy = cur_sy;            // first staff system
            VoiceStavesSymbols new_sy;

            // check if different bars at the same time
            void b_chk()
            {
                VoiceItem s, s2;
                int v,ir=0;
                object bt, t;
                    

                while (true)
                {
                    v = vn[ir++];
                    if (v == null)
                        break;
                    s = vtb[v];
                    if (s == null || s.bar_type == null || s.invis != null
                        || s.time != time)
                        continue;
                    if (bt == null)
                    {
                        bt = s.bar_type;
                        if (s.text != null && bt.ToString() == "|")
                            t = s.text;
                        continue;
                    }
                    if (s.bar_type.ToString() != bt.ToString())
                        break;
                    if (s.text != null && t == null && bt.ToString() == "|")
                    {
                        t = s.text;
                        break;
                    }
                }

                // if the previous symbol is a grace note at the same offset as the bar
                // remove the grace notes from the previous time sequence
                if (fl == null)
                {
                    while (prev.type == C.GRACE
                        && vtb[prev.v] != null && vtb[prev.v].bar_type == null)
                    {
                        vtb[prev.v] = prev;
                        prev = prev.ts_prev;
                        fl = true;
                    }
                }

                if (v == null)
                    return;            // no problem

                // change "::" to ":| |:"
                // and    "|1" to "| [1"
                if (bt.ToString() == "::" || bt.ToString() == ":|"
                    || t != null)
                {
                    ir = 0;
                    bt = t != null ? '|' : "::";
                    while (true)
                    {
                        v = vn[ir++];
                        if (v == null)
                            break;
                        s = vtb[v];
                        if (s == null || s.invis != null
                            || s.bar_type.ToString() != bt.ToString()
                            || (bt.ToString() == "|" && s.text == null))
                            continue;
                        s2 = clone(s);
                        if (bt.ToString() == "::")
                        {
                            s.bar_type = ":|";
                            s2.bar_type = "|:";
                        }
                        else
                        {
                            //                    s.bar_type = '|';
                            s.Remove("text");
                            s.Remove("rbstart");
                            s2.bar_type = "[";
                            s2.invis = true;
                            s2.xsh = 0;
                        }
                        s2.next = s.next;
                        if (s2.next != null)
                            s2.next.prev = s2;
                        s2.prev = s;
                        s.next = s2;
                    }
                }
                else
                {
                    error(1, s, "Different bars $1 and $2",
                        (bt.ToString() + (t != null ? t.ToString() : "")), (s.bar_type.ToString() + (s.text != null ? s.text.ToString() : "")));
                }
            } // b_chk()

            // set the first symbol of each voice
            for (v = 0; v < nv; v++)
            {
                s = voice_tb[v].sym;
                vtb[v] = s;
                if (sy.voices[v] != null)
                {
                    vn[sy.voices[v].range] = v;
                    if (prev == null && s != null)
                    {
                        fmt = s.fmt;
                        p_voice = voice_tb[v];
                        prev = new Dictionary<string, object>            // symbol defining the first staff system
                            {
                                { "type", C.STAVES },
                                { "fname", parse.fname },
                                { "dur", 0 },
                                { "v", v },
                                { "p_v", p_voice },
                                { "time", 0 },
                                { "st", 0 },
                                { "sy", sy },
                                { "next", s },
                                { "fmt", fmt },
                                { "seqst", true }
                            };
                    }
                }
            }

            if (prev == null)
                return;                    // no symbol yet

            // insert the first staff system in the first voice
            p_voice.sym = tsfirst = s = prev;
            if (s.next != null)
                s.next.prev = s;
            else
                p_voice.last_sym = s;

            // if Q: from tune header, put it at start of the music
            // (after the staff system)
            s = glovar.tempo;
            if (s != null)
            {
                s.v = v = p_voice.v;
                s.p_v = p_voice;
                s.st = 0;
                s.time = 0;
                s.prev = prev;
                s.next = prev.next;
                if (s.next != null)
                    s.next.prev = s;
                else
                    p_voice.last_sym = s;
                s.prev.next = s;
                s.fmt = fmt;
                glovar.tempo = null;
                vtb[v] = s;
            }

            // if only one voice, quickly create the time links
            if (nv == 1)
            {
                s = tsfirst;
                s.ts_next = s.next;
                while (true)
                {
                    s = s.next;
                    if (s == null)
                        return;
                    if (s.time != s.prev.time
                        || w_tb[s.prev.type] != null
                        || s.type == C.GRACE && s.prev.type == C.GRACE)
                        s.seqst = true;
                    if (s.type == C.PART)
                    {        // move the part
                        s.prev.next =
                            s.prev.ts_next = s.next;
                        if (s.next != null)
                        {
                            s.next.part = s;    // to the next symbol
                            s.next.prev = s.prev;
                            if (s.soln != null)
                                s.next.soln = true;
                            if (s.seqst != null)
                                s.next.seqst = true;
                        }
                        continue;
                    }
                    s.ts_prev = s.prev;
                    s.ts_next = s.next;
                }
                // not reached
            }

            // loop on the symbols of all voices
            while (true)
            {
                if (new_sy != null)
                {
                    sy = new_sy;
                    new_sy = null;
                    vn = new object[nv];
                    for (v = 0; v < nv; v++)
                    {
                        if (sy.voices[v] == null)
                            continue;
                        vn[sy.voices[v].range] = v;
                    }
                }

                /* search the min time and symbol weight */
                wmin = time = 10000000;        // big int
                ir = 0;
                while (true)
                {
                    v = vn[ir++];
                    if (v == null)
                        break;
                    s = vtb[v];
                    if (s == null || s.time > time)
                        continue;
                    w = w_tb[s.type];
                    if (s.type == C.GRACE && s.next != null && s.next.type == C.GRACE)
                        w--;
                    if (s.time < time)
                    {
                        time = s.time;
                        wmin = w;
                    }
                    else if (w < wmin)
                    {
                        wmin = w;
                    }
                }

                if (wmin > 127)
                    break;            // done

                // check the type of the measure bars
                if (wmin == 6)            // !! weight of bars
                    b_chk();

                /* link the vertical sequence */
                ir = 0;
                while (true)
                {
                    v = vn[ir++];
                    if (v == null)
                        break;
                    s = vtb[v];
                    if (s == null
                        || s.time != time)
                        continue;
                    w = w_tb[s.type];
                    if (w == null
                        && s.type == C.GRACE && s.next != null && s.next.type == C.GRACE)
                        w--;
                    if (w != wmin)
                        continue;
                    if (w == null
                        && s.type == C.PART)
                    {        // move the part
                        if (s.prev != null)
                            s.prev.next = s.next;
                        else
                            s.p_v.sym = s.next;
                        vtb[v] = s.next;
                        if (s.next != null)
                        {
                            s.next.part = s;    // to the next symbol
                            s.next.prev = s.prev;
                            if (s.soln != null)
                                s.next.soln = true;
                            //                } else {
                            // ignored
                        }
                        continue;
                    }
                    if (s.type == C.STAVES)
                        new_sy = s.sy;
                    if (fl != null)
                    {
                        fl = null;
                        s.seqst = true;
                    }
                    s.ts_prev = prev;
                    prev.ts_next = s;
                    prev = s;

                    vtb[v] = s.next;
                }
                if (wmin != null)            // if some width
                    fl = true;            // start a new sequence
            }
        }

        // adjust some voice elements
        void voice_adj(object sys_chg = null)
        {
            object p_voice, v, sl;
            VoiceItem s, s2;

            // set the duration of the notes under a feathered beam
            void set_feathered_beam(VoiceItem s1)
            {
                VoiceItem s, s2;
                int t, d, b, i, a,
                    d = s1.dur,
                    n = 1;

                /* search the end of the beam */
                for (s = s1; s != null; s = s.next)
                {
                    if (s.beam_end != null || s.next == null)
                        break;
                    n++;
                }
                if (n <= 1)
                {
                    s1.Remove("feathered_beam");
                    return;
                }
                s2 = s;
                b = d / 2;        /* smallest note duration */
                a = d / (n - 1);        /* delta duration */
                t = s1.time;
                if (s1.feathered_beam > 0)
                {        /* !beam-accel! */
                    for (s = s1, i = n - 1;
                        s != s2;
                        s = s.next, i--)
                    {
                        d = ((a * i) | 0) + b;
                        s.dur = d;
                        s.time = t;
                        t += d;
                    }
                }
                else
                {                /* !beam-rall! */
                    for (s = s1, i = 0;
                        s != s2;
                        s = s.next, i++)
                    {
                        d = ((a * i) | 0) + b;
                        s.dur = d;
                        s.time = t;
                        t += d;
                    }
                }
                s.dur = s.time + s.dur - t;
                s.time = t;
            } // end set_feathered_beam()

            // terminate voice cloning
            if (curvoice != null && curvoice.clone != null)
            {
                parse.istart = parse.eol;
                do_cloning();
            }

            // if only one voice and a time skip,
            // fill the voice with the sequence "Z |" (multi-rest and bar)
            if (par_sy.one_v != null)            // if one voice
                fill_mr_ba(voice_tb[par_sy.top_voice]);

            for (v = 0; v < voice_tb.Length; v++)
            {
                p_voice = voice_tb[v];
                if (sys_chg == null)
                {            // if not %%score
                    p_voice.eoln = null;
                    while (true)
                    {        // set the end of slurs
                        sl = p_voice.sls.shift();
                        if (sl == null)
                            break;
                        s = sl.ss;
                        //                    error(1, s, "Lack of ending slur(s)")
                        if (s.sls == null)
                            s.sls = new object[] { };
                        sl.loc = 'o';        // no slur end
                        s.sls.push(sl);
                    }
                } // not %%score
                for (s = p_voice.sym; s != null; s = s.next)
                {
                    if (s.time >= staves_found)
                        break;
                }
                for (; s != null; s = s.next)
                {

                    // if the symbol has a sequence weight smaller than the bar one
                    // and if there a time skip,
                    // add an invisible bar before it
                    if (w_tb[s.type] < 5
                        && s.type != C.STAVES
                        && s.type != C.CLEF
                        && s.time != null            // not at start of tune
                        && (s.prev == null || s.time > s.prev.time + s.prev.dur))
                    {
                        s2 = new VoiceItem
                            {
                                type= C.BAR ,
                                bar_type= "[]" ,
                                v= s.v ,
                                p_v= s.p_v ,
                                st= s.st ,
                                time= s.time ,
                                dur= 0 ,
                                next= s ,
                                prev= s.prev ,
                                fmt= s.fmt ,
                                invis= true 
                            };
                        if (s.prev != null)
                            s.prev.next = s2;
                        else
                            voice_tb[s.v].sym = s2;
                        s.prev = s2;
                    }

                    switch (s.type)
                    {
                        case C.GRACE:
                            if (cfmt.graceword == null)
                                continue;
                            for (s2 = s.next; s2 != null; s2 = s2.next)
                            {
                                switch (s2.type)
                                {
                                    case C.SPACE:
                                        continue;
                                    case C.NOTE:
                                        if (s2.a_ly == null)
                                            break;
                                        s.a_ly = s2.a_ly;
                                        s2.a_ly = null;
                                        break;
                                }
                                break;
                            }
                            continue;
                    }

                    if (s.feathered_beam != null)
                        set_feathered_beam(s);
                }
            }
        }

        /* -- create a new staff system -- */
        void new_syst(object init = null)
        {
            object st, v, sy_staff, p_voice,
                sy_new = new Dictionary<string, object>
                {
                        { "voices", new object[] { } },
                        { "staves", new object[] { } },
                        { "top_voice", 0 }
                };

            if (init != null)
            {                /* first staff system */
                cur_sy = par_sy = sy_new;
                return;
            }

            // update the previous system
            for (v = 0; v < voice_tb.Length; v++)
            {
                if (par_sy.voices[v] != null)
                {
                    st = par_sy.voices[v].st;
                    sy_staff = par_sy.staves[st];
                    p_voice = voice_tb[v];

                    sy_staff.staffnonote = p_voice.staffnonote;
                    if (p_voice.staffscale != null)
                        sy_staff.staffscale = p_voice.staffscale;
                }
            }
            for (st = 0; st < par_sy.staves.Length; st++)
            {
                sy_new.staves[st] = clone(par_sy.staves[st]);
                sy_new.staves[st].flags = 0;
            }
            par_sy.next = sy_new;
            par_sy = sy_new;
        }

        /* -- set the bar numbers -- */
        // (possible hook)
        void set_bar_num()
        {
            object s, s2, rep_tim, k, n, nu, txt,
                tim = 0,            // time of the previous bar
                bar_num = gene.nbar,
                bar_tim = 0,            // time of previous repeat variant
                ptim = 0,            // time of previous bar
                wmeasure = voice_tb[cur_sy.top_voice].meter.wmeasure;

            // check the measure duration
            object check_meas()
            {
                object s3;

                if (tim > ptim + wmeasure
                    && s.prev.type != C.MREST)
                    return true;

                // the measure is too short,
                // check if there is a bar a bit further
                for (s3 = s.next; s3 != null && s3.time == s.time; s3 = s3.next)
                    ;
                for (; s3 != null && s3.bar_type == null; s3 = s3.next)
                    ;
                return s3 != null && (s3.time - bar_tim) % wmeasure;
            }

            // don't count a bar at start of tune
            for (s = tsfirst; ; s = s.ts_next)
            {
                if (s == null)
                    return;
                switch (s.type)
                {
                    case C.METER:
                        wmeasure = s.wmeasure;
                        // fall thru
                        goto case C.CLEF;
                    case C.CLEF:
                    case C.KEY:
                    case C.STBRK:
                        continue;
                    case C.BAR:
                        if (s.bar_num != null)
                            bar_num = s.bar_num;    // %%setbarnb)
                        break;
                }
                break;
            }

            // at start of tune, check for an anacrusis
            for (s2 = s.ts_next; s2 != null; s2 = s2.ts_next)
            {
                if (s2.type == C.BAR && s2.time != null)
                {
                    if (s2.time < wmeasure)
                    {    // if anacrusis
                        s = s2;
                        bar_tim = s.time;
                    }
                    break;
                }
            }

            // set the measure number on the top bars
            for (; s != null; s = s.ts_next)
            {
                switch (s.type)
                {
                    case C.METER:
                        if (s.time == bar_tim)
                            break;        // already seen
                        if (wmeasure != 1)        // if not M:none
                            bar_num += (s.time - bar_tim) / wmeasure;
                        bar_tim = s.time;
                        wmeasure = s.wmeasure;
                        break;
                    case C.BAR:
                        if (s.time <= tim)
                            break;            // already seen
                        tim = s.time;

                        nu = true;            // no num update
                        txt = "";
                        for (s2 = s; s2 != null; s2 = s2.next)
                        {
                            if (s2.time > tim || s2.dur != null)
                                break;
                            if (s2.bar_type == null)
                                continue;
                            if (s2.bar_type.ToString() != "[")
                                nu = false;            // do update
                            if (s2.text != null)
                                txt = s2.text.ToString();
                        }
                        if (s.bar_num != null)
                        {
                            bar_num = s.bar_num;    // (%%setbarnb)
                            ptim = bar_tim = tim;
                            continue;
                        }
                        if (wmeasure == 1)
                        {        // if M:none
                            if (s.bar_dotted != null)
                                continue;
                            if (txt != null)
                            {
                                if (!cfmt.contbarnb)
                                {
                                    if (txt[0] == '1')
                                        rep_tim = bar_num;
                                    else
                                        bar_num = rep_tim;
                                }
                            }
                            if (!nu)
                                s.bar_num = ++bar_num;
                            continue;
                        }

                        n = bar_num + (tim - bar_tim) / wmeasure;
                        k = n - (int)n;
                        if (cfmt.checkbars != null
                            && k != null
                            && check_meas())
                            error(0, s, "Bad measure duration");
                        if (tim > ptim + wmeasure)
                        {    // if more than one measure
                            n = (int)n;
                            k = 0;
                            bar_tim = tim;        // re-synchronize
                            bar_num = n;
                        }

                        if (txt != null)
                        {
                            if (txt[0] == '1')
                            {
                                if (!cfmt.contbarnb)
                                    rep_tim = tim - bar_tim;
                                if (!nu)
                                    s.bar_num = n;
                            }
                            else
                            {
                                if (!cfmt.contbarnb)
                                    bar_tim = tim - rep_tim;
                                n = bar_num + (tim - bar_tim) / wmeasure;
                                if ((int)n == n)
                                    s.bar_num = n;
                            }
                        }
                        else
                        {
                            n = (int)n;
                            s.bar_num = n;
                        }
                        if (k == null)
                            ptim = tim;
                        break;
                }
            }
        }

        // convert a note to ABC
        void not2abc(object pit, object acc)
        {
            object i,
                nn = "";

            if (acc != null && acc != 3)
            {
                if (acc.GetType() != typeof(object[]))
                {
                    nn = new string[] { "__", "_", "", "^", "^^" }[(int)acc + 2];
                }
                else
                {
                    i = ((object[])acc)[0];
                    if ((int)i > 0)
                    {
                        nn += "^";
                    }
                    else
                    {
                        nn += "_";
                        i = -(int)i;
                    }
                    nn += i + "/" + ((object[])acc)[1];
                }
            }
            nn += ntb[((int)pit + 75) % 7];
            for (i = (int)pit; i >= 23; i -= 7)
                nn += "'";
            for (i = (int)pit; i < 16; i += 7)
                nn += ",";
            return nn;
        }

        // note mapping
        // %%map map_name note [print [note_head]] [param]*
        void get_map(object text)
        {
            if (text == null)
                return;

            object i, note, notes, map, tmp, ns,
                ty = "",
                a = text.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (a.Length < 3)
            {
                syntax(1, errs.not_enough_p);
                return;
            }
            ns = a[1];
            if (ns[0] == '*' || ns.IndexOf("all") == 0)
            {
                ns = "all";
            }
            else
            {
                if (ns.IndexOf("octave,") == 0        // remove the octave part
                    || ns.IndexOf("key,") == 0)
                {
                    ty = ns[0].ToString();
                    ns = ns.Split(',')[1];
                    ns = ns.Replace(new string[] { ",", "'" }, "").ToUpper();
                    if (ns.IndexOf("key,") == 0)
                        ns = ns.Replace(new string[] { "=", "^", "_" }, "");
                }
                tmp = new scanBuf();
                tmp.buffer = ns;
                note = parse_acc_pit(tmp);
                if (note == null)
                {
                    syntax(1, "Bad note in %%map");
                    return;
                }
                ns = ty + not2abc(note.pit, note.acc);
            }

            notes = maps[a[0]];
            if (notes == null)
                maps[a[0]] = notes = new Dictionary<object, object>();
            map = notes[ns];
            if (map == null)
                notes[ns] = map = new List<object>();

            // try the optional 'print' and 'heads' parameters
            a = a.Skip(2).ToArray();
            i = 0;
            if (a[0].IndexOf('=') < 0)
            {
                if (a[0][0] != '*')
                {
                    tmp = new scanBuf();        // print
                    tmp.buffer = a[0];
                    map[1] = parse_acc_pit(tmp);
                }
                if (a.Length < 2)
                    return;
                i++;
                if (a[1].IndexOf('=') < 0)
                {
                    map[0] = a[1].Split(',');    // heads
                    i++;
                }
            }

            for (; i < a.Length; i++)
            {
                switch (a[i])
                {
                    case "heads=":
                        if (a.Length < i + 2)
                        {
                            syntax(1, errs.not_enough_p);
                            break;
                        }
                        map[0] = a[i + 1].Split(',');
                        break;
                    case "print=":
                    case "play=":
                    case "print_notrp=":
                        if (a.Length < i + 2)
                        {
                            syntax(1, errs.not_enough_p);
                            break;
                        }
                        tmp = new scanBuf();
                        tmp.buffer = a[i + 1];
                        note = parse_acc_pit(tmp);
                        if (a[i][5] == '_')        // if print no transpose
                            note.notrp = true;
                        if (a[i][1] == 'r')
                            map[1] = note;
                        else
                            map[3] = note;
                        break;
                    case "color=":
                        if (a.Length < i + 2)
                        {
                            syntax(1, errs.not_enough_p);
                            break;
                        }
                        map[2] = a[i + 1];
                        break;
                }
            }
        }

        // check if a common symbol is already registered
        void new_ctrl(object s)
        {
            object a,
                ty = (s.type + curvoice.time).ToString();

            if (parse.ctrl == null)
                parse.ctrl = new Dictionary<object, object>();
            a = parse.ctrl[ty];
            if (a != null)
            {                // symbol already declared
                if (((int)a & (1 << (int)curvoice.v)) != 0)    // in this voice?
                    return;            // yes, keep it (case second Q:)
                parse.ctrl[ty] = (int)a | (1 << (int)curvoice.v);
                return;            // no, ignore
            }
            parse.ctrl[ty] = 1 << (int)curvoice.v;
            return;
        }

        // get a abcm2ps/abcMIDI compatible transposition value as a base-40 interval
        // The value may be
        // - [+|-]<number of semitones>[s|f]
        // - <note1>[<note2>]  % <note2> default is 'c'
        int  get_transp(object param)
        {
            if (param.ToString()[0] == '0')
                return 0;
            if ("123456789-+".IndexOf(param.ToString()[0]) >= 0)
            {    // by semi-tone
                var val = int.Parse(param.ToString());
                if (val < -36 || val > 36)
                {
                    //fixme: no source reference...
                    syntax(1, errs.bad_transp);
                    return;
                }
                val += 36;
                val = (((val / 12) | 0) - 3) * 40 + abc2svg.isb40[val % 12];
                if (param.ToString().Substring(param.ToString().Length - 1) == "b")
                    val += 4;
                return val;
            }
            // return undefined
        }

        /* -- process a pseudo-comment (%% or I:) -- */
        // (possible hook)
        void do_pscom(string text)
        {
            string cmd, param;
            var h1 = 0;
            var val = 0.0;
            var s = new List<object>();
            var n = 0;
            var k = 0;
            var b = false;

            cmd = text.Split(' ')[0];

            if (string.IsNullOrEmpty(cmd))
                return;

            // ignore the command if the voice is ignored,
            // but not if %%score/%%staves!
            if (curvoice != null && curvoice.ignore)
            {
                switch (cmd)
                {
                    case "staves":
                    case "score":
                        break;
                    default:
                        return;
                }
            }

            param = text.Replace(cmd, "").Trim();

            if (param.EndsWith(" lock"))
            {
                fmt_lock[cmd] = true;
                param = param.Substring(0, param.Length - 5).Trim();
            }
            else if (fmt_lock[cmd])
            {
                return;
            }

            switch (cmd)
            {
                case "clef":
                    if (parse.state >= 2)
                    {
                        s = new_clef(param);
                        if (s != null)
                            get_clef(s);
                    }
                    return;
                case "deco":
                    deco_add(param);
                    return;
                case "linebreak":
                    set_linebreak(param);
                    return;
                case "map":
                    get_map(param);
                    return;
                case "maxsysstaffsep":
                case "sysstaffsep":
                    if (parse.state == 3)
                    {
                        val = get_unit(param);
                        if (double.IsNaN(val))
                        {
                            syntax(1, errs.bad_val, "%%" + cmd);
                            return;
                        }
                        par_sy.voices[curvoice.v][cmd[0] == 'm' ? "maxsep" : "sep"] = val;
                        return;
                    }
                    break;
                case "multicol":
                    switch (param)
                    {
                        case "start":
                        case "new":
                        case "end":
                            break;
                        default:
                            syntax(1, "Unknown keyword '$1' in %%multicol", param);
                            return;
                    }
                    s = new List<object> { C.BLOCK, "mc_" + param, 0 };
                    if (parse.state >= 2)
                    {
                        curvoice = voice_tb[0];
                        curvoice.eoln = true;
                        sym_link(s);
                        return;
                    }
                    set_ref(s);
                    self.block_gen(s);
                    return;
                case "ottava":
                    if (parse.state != 3)
                        return;
                    n = int.Parse(param);
                    if (double.IsNaN(n) || n < -2 || n > 2 || (n == 0 && curvoice.ottava == null))
                    {
                        syntax(1, errs.bad_val, "%%ottava");
                        return;
                    }
                    k = n;
                    if (n != 0)
                    {
                        curvoice.ottava = n;
                    }
                    else
                    {
                        n = curvoice.ottava;
                        curvoice.ottava = 0;
                    }
                    a_dcn.Add(new List<string> { "15mb", "8vb", "", "8va", "15ma" }[n + 2] + (k != 0 ? "(" : ")"));
                    return;
                case "repbra":
                    if (curvoice != null)
                        curvoice.norepbra = !get_bool(param);
                    return;
                case "repeat":
                    if (parse.state != 3)
                        return;
                    if (curvoice.last_sym == null)
                    {
                        syntax(1, "%%repeat cannot start a tune");
                        return;
                    }
                    if (string.IsNullOrEmpty(param))
                    {
                        n = 1;
                        k = 1;
                    }
                    else
                    {
                        b = param.Split(' ').Length > 1;
                        n = int.Parse(param.Split(' ')[0]);
                        k = b ? int.Parse(param.Split(' ')[1]) : 1;
                        if (double.IsNaN(n) || n < 1 || (curvoice.last_sym.type == C.BAR && n > 2))
                        {
                            syntax(1, "Incorrect 1st value in %%repeat");
                            return;
                        }
                        if (double.IsNaN(k))
                        {
                            k = 1;
                        }
                        else
                        {
                            if (k < 1)
                            {
                                syntax(1, "Incorrect 2nd value in %%repeat");
                                return;
                            }
                        }
                    }
                    parse.repeat_n = curvoice.last_sym.type == C.BAR ? n : -n;
                    parse.repeat_k = k;
                    return;
                case "sep":
                    double h2, len;
                    var values = new List<double>();
                    var lwidth = img.width - img.lm - img.rm;

                    set_page();
                    h1 = h2 = len = 0;
                    if (!string.IsNullOrEmpty(param))
                    {
                        values = param.Split(' ').Select(double.Parse).ToList();
                        h1 = get_unit(values[0]);
                        if (values.Count > 1)
                        {
                            h2 = get_unit(values[1]);
                            if (values.Count > 2)
                                len = get_unit(values[2]);
                        }
                        if (double.IsNaN(h1) || double.IsNaN(h2) || double.IsNaN(len))
                        {
                            syntax(1, errs.bad_val, "%%sep");
                            return;
                        }
                    }
                    if (h1 < 1)
                        h1 = 14;
                    if (h2 < 1)
                        h2 = h1;
                    if (len < 1)
                        len = 90;
                    if (parse.state >= 2)
                    {
                        s = new_block(cmd);
                        s.x = (lwidth - len) / 2 / cfmt.scale;
                        s.l = len / cfmt.scale;
                        s.sk1 = h1;
                        s.sk2 = h2;
                        return;
                    }
                    vskip(h1);
                    output += "<path class=\"stroke\"\n\td=\"M";
                    out_sxsy((lwidth - len) / 2 / cfmt.scale, ' ', 0);
                    output += "h" + (len / cfmt.scale).ToString("F1") + "\"/>\n";
                    vskip(h2);
                    blk_flush();
                    return;
                case "setbarnb":
                    val = int.Parse(param);
                    if (double.IsNaN(val) || val < 1)
                    {
                        syntax(1, "Bad %%setbarnb value");
                        break;
                    }
                    glovar.new_nbar = val;
                    return;
                case "staff":
                    if (parse.state != 3)
                        return;
                    val = int.Parse(param);
                    if (double.IsNaN(val))
                    {
                        syntax(1, "Bad %%staff value '$1'", param);
                        return;
                    }
                    var st = 0;
                    if (param[0] == '+' || param[0] == '-')
                        st = curvoice.cst + val;
                    else
                        st = val - 1;
                    if (st < 0 || st > nstaff)
                    {
                        syntax(1, "Bad %%staff number $1 (cur $2, max $3)", st, curvoice.cst, nstaff);
                        return;
                    }
                    curvoice.floating = null;
                    curvoice.cst = st;
                    return;
                case "staffbreak":
                    if (parse.state != 3)
                        return;
                    s = new List<object> { C.STBRK, 0 };
                    if (param.EndsWith("f"))
                    {
                        s.stbrk_forced = true;
                        param = param.Replace(" f", "");
                    }
                    if (!string.IsNullOrEmpty(param))
                    {
                        val = get_unit(param);
                        if (double.IsNaN(val))
                        {
                            syntax(1, errs.bad_val, "%%staffbreak");
                            return;
                        }
                        s.xmx = val;
                    }
                    else
                    {
                        s.xmx = 14;
                    }
                    sym_link(s);
                    return;
                case "tacet":
                    if (param[0] == '"')
                        param = param.Substring(1, param.Length - 2);
                    // fall thru
                    goto case "stafflines";
                case "stafflines":
                case "staffscale":
                case "staffnonote":
                    set_v_param(cmd, param);
                    return;
                case "staves":
                case "score":
                    if (parse.state == 0)
                        return;
                    if (parse.scores != null && parse.scores.Count > 0)
                    {
                        text = parse.scores[0];
                        cmd = text.Split(' ')[0];
                        param = text.Replace(cmd, "").Trim();
                    }
                    get_staves(cmd, param);
                    return;
                case "center":
                case "text":
                    k = cmd[0] == 'c' ? 'c' : cfmt.textoption;
                    set_font("text");
                    if (parse.state >= 2)
                    {
                        s = new_block("text");
                        s.text = param;
                        s.opt = k;
                        s.font = cfmt.textfont;
                        return;
                    }
                    write_text(param, k);
                    return;
                case "transpose":        // (abcm2ps compatibility)
                    if (cfmt.sound)
                        return;
                    val = get_transp(param);
                    if (val == null)        // accept note interval
                    {
                        val = get_interval(param);
                        if (val == null)
                            return;
                    }
                    switch (parse.state)
                    {
                        case 0:
                            cfmt.transp = 0;
                            goto case 1;
                        case 1:
                            cfmt.transp = (cfmt.transp ?? 0) + val;
                            return;
                    }
                    curvoice.shift = val;
                    key_trans();
                    return;
                case "tune":
                    //fixme: to do
                    return;
                case "user":
                    set_user(param);
                    return;
                case "voicecolor":
                    if (curvoice != null)
                        curvoice.color = param;
                    return;
                case "vskip":
                    val = get_unit(param);
                    if (double.IsNaN(val))
                    {
                        syntax(1, errs.bad_val, "%%vskip");
                        return;
                    }
                    if (val < 0)
                    {
                        syntax(1, "%%vskip cannot be negative");
                        return;
                    }
                    if (parse.state >= 2)
                    {
                        s = new_block(cmd);
                        s.sk = val;
                        return;
                    }
                    vskip(val);
                    return;
                case "newpage":
                case "leftmargin":
                case "rightmargin":
                case "pagescale":
                case "pagewidth":
                case "printmargin":
                case "scale":
                case "staffwidth":
                    if (parse.state >= 2)
                    {
                        s = new_block(cmd);
                        s.param = param;
                        return;
                    }
                    if (cmd == "newpage")
                    {
                        blk_flush();
                        if (user.page_format)
                            blkdiv = 2;    // start the next SVG in a new page
                        return;
                    }
                    break;
            }
            self.set_format(cmd, param);
        }

        // treat the %%beginxxx / %%endxxx sequences
        // (possible hook)
        void do_begin_end(object type,
            object opt,
            object text)
        {
            object i, j, action, s;

            switch (type.ToString())
            {
                case "js":
                    js_inject(text.ToString());
                    break;
                case "ml":
                    if (cfmt.pageheight != null)
                    {
                        syntax(1, "Cannot have %%beginml with %%pageheight");
                        break;
                    }
                    if (parse.state >= 2)
                    {
                        s = new_block(type.ToString());
                        s.text = text.ToString();
                    }
                    else
                    {
                        blk_flush();
                        if (user.img_out != null)
                            user.img_out(text.ToString());
                    }
                    break;
                case "svg":
                    j = 0;
                    while (true)
                    {
                        i = text.ToString().IndexOf("<style", j);
                        if (i < 0)
                            break;
                        i = text.ToString().IndexOf(">", i);
                        j = text.ToString().IndexOf("</style>", i);
                        if (j < 0)
                        {
                            syntax(1, "No </style> in %%beginsvg sequence");
                            break;
                        }
                        style += text.ToString().Substring(i + 1, j - i).TrimEnd();
                    }
                    j = 0;
                    while (true)
                    {
                        i = text.ToString().IndexOf("<defs>\n", j);
                        if (i < 0)
                            break;
                        j = text.ToString().IndexOf("</defs>", i);
                        if (j < 0)
                        {
                            syntax(1, "No </defs> in %%beginsvg sequence");
                            break;
                        }
                        defs_add(text.ToString().Substring(i + 6, j - i - 6));
                    }
                    break;
                case "text":
                    action = get_textopt(opt);
                    if (action == null)
                        action = cfmt.textoption;
                    set_font("text");
                    if (text.ToString().IndexOf('\\') >= 0)
                        text = cnv_escape(text.ToString());
                    if (parse.state > 1)
                    {
                        s = new_block(type.ToString());
                        s.text = text.ToString();
                        s.opt = action;
                        s.font = cfmt.textfont;
                        break;
                    }
                    write_text(text.ToString(), action);
                    break;
            }
        }



        /* -- generate a piece of tune -- */
        void generate()
        {
            List<object> s, v, p_voice;

            if (a_dcn.Count > 0)
            {
                syntax(1, "Decoration without symbol");
                a_dcn = new List<object>();
            }

            if (parse.tp != null)
            {
                syntax(1, "No end of tuplet");
                s = parse.tps;
                if (s != null)
                    s.tp = null;
                parse.tp = null;
            }

            if (vover != null)
            {
                syntax(1, "No end of voice overlay");
                get_vover(vover.bar != null ? '|' : ')');
            }

            voice_adj();
            sort_all();            /* define the time / vertical sequences */

            if (tsfirst != null)
            {
                for (v = 0; v < voice_tb.Count; v++)
                {
                    if (voice_tb[v].key == null)
                        voice_tb[v].key = parse.ckey;    // set the starting key
                }
                if (user.anno_start != null)
                    anno_start = a_start;
                if (user.anno_stop != null)
                    anno_stop = a_stop;
                self.set_bar_num();

                if (info.P != null)
                    tsfirst.parts = info.P;    // for play

                // give the parser result to the application
                if (user.get_abcmodel != null)
                    user.get_abcmodel(tsfirst, voice_tb, abc2svg.sym_name, info);

                if (user.img_out != null)    // if SVG generation
                    self.output_music();
            } // (tsfirst)

            // finish the generation
            set_page();            // the page layout may have changed
            if (info.W != null)
                put_words(info.W);
            put_history();
            parse.state = 0;            // file header
            blk_flush();            // (force end of block)

            if (tsfirst != null)
            {        // if non void, keep tune data for upper layers
                tunes.Add(new List<object> { tsfirst, voice_tb, info, cfmt });
                tsfirst = null;
            }
        }

        // transpose the current key of the voice (called on K: or V:)
        void key_trans()
        {
            int i, n;
            List<object> a_acc;
            int b40, b40c;
            var s = curvoice.ckey;            // current key
            var ti = s.time ?? 0;

            if (s.k_bagpipe != null || s.k_drum != null)
                return;

            // set the score transposition
            n = (curvoice.score ?? 0)        // new transposition
                + (curvoice.shift ?? 0)
                + (cfmt.transp ?? 0);
            if ((curvoice.tr_sco ?? 0) == n)
                return;                // same transposition

            // get the current key or create a new one
            if (is_voice_sig())            // if no symbol yet
            {
                curvoice.key = s;        // new root key of the voice
            }
            else if (curvoice.time != ti)    // if no K: at this time
            {
                s = clone(s.orig ?? s);        // new key
                if (curvoice.new_name == null)
                    s.k_old_sf = curvoice.ckey.k_sf;
                sym_link(s);
            }
            curvoice.ckey = s;            // current key

            if (cfmt.transp != null && curvoice.shift != null)    // if %%transpose and shift=
                syntax(0, "Mix of old and new transposition syntaxes");

            // define the new key
            curvoice.tr_sco = n;            // b40 interval

            b40 = (s.k_b40 + 200 + n) % 40;    // (s.k_40 is the original K:)
            b40c = s.k_mode != null ? abc2svg.b40mc : abc2svg.b40Mc;    // minor - major
            i = b40c[b40] - b40;
            if (i != 0)                // no chord here
            {
                curvoice.tr_sco += i;        // set an enharmonic one
                b40 += i;
            }

            s.orig = clone(s);            // keep the original K: definition
            s.k_b40 = b40;
            if (s.k_none == null)                // if some key
                s.k_sf = abc2svg.b40sf[b40];

            // transpose the accidental list
            if (s.k_a_acc == null)
                return;
            a_acc = new List<object>();
            foreach (var acc in s.k_a_acc)
            {
                b40 = abc2svg.pab40(acc.pit, acc.acc) + d;
                a_acc.Add(new List<object> { abc2svg.b40p(b40), abc2svg.b40a(b40) ?? 3 });
            }
            s.k_a_acc = a_acc;
        }

        // fill a voice with a multi-rest and a bar
        void fill_mr_ba(List<object> p_v)
        {
            int v;
            List<object> p_v2;
            var mxt = 0;

            for (v = 0; v < voice_tb.Count; v++)
            {
                if (voice_tb[v].time > mxt)
                {
                    p_v2 = voice_tb[v];
                    mxt = p_v2.time;
                }
            }
            if (p_v.time >= mxt)
                return;

            List<object> p_v_sav = curvoice;
            var dur = mxt - p_v.time;
            var s = new List<object> { C.MREST, 0, 0, dur, dur, dur / p_v.wmeasure, new List<object> { new List<object> { 18, dur } }, p_v.tacet };

            var s2 = new List<object> { C.BAR, '|', 0, 0 };

            if (p_v2.last_sym.bar_type != null)
                s2[1] = p_v2.last_sym.bar_type;
            //    s2.soln = p_v2.last_sym.soln

            glovar.mrest_p = true;

            curvoice = p_v;
            sym_link(s);
            sym_link(s2);

            curvoice = p_v_sav;
        }

        /* -- get staves definition (%%staves / %%score) -- */
        void get_staves(string cmd, string parm)
        {
            int s, p_voice, p_voice2, i, flags, v, vid, a_vf, st, range, nv = voice_tb.Count, maxtime = 0;

            if (curvoice.Count > 0 && curvoice.Contains("clone"))
            {
                i = parse.eol;
                parse.eol = parse.bol;
                do_cloning();
                parse.eol = i;
            }

            if (parm != null)
            {
                a_vf = parse_staves(parm);
                if (a_vf == null)
                {
                    return;
                }
            }
            else if (staves_found < 0)
            {
                syntax(1, errs.bad_val, "%%" + cmd);
                return;
            }

            for (v = 0; v < nv; v++)
            {
                p_voice = voice_tb[v];
                if (p_voice.time > maxtime)
                {
                    maxtime = p_voice.time;
                }
            }
            if (maxtime == 0)
            {
                par_sy.staves = new List<byte>();
                par_sy.voices = new List<byte>();
            }
            else
            {
                voice_adj(true);
                for (v = 0; v < par_sy.voices.Count; v++)
                {
                    if (par_sy.voices[v] != null)
                    {
                        curvoice = voice_tb[v];
                        break;
                    }
                }
                curvoice.time = maxtime;
                s = new List<byte> { type = C.STAVES, dur = 0 };
                sym_link(s);
                par_sy.nstaff = nstaff;
                if (parm == null)
                {
                    s.sy = clone(par_sy, 1);
                    par_sy.next = s.sy;
                    par_sy = s.sy;
                    staves_found = maxtime;
                    for (v = 0; v < nv; v++)
                    {
                        voice_tb[v].time = maxtime;
                    }
                    curvoice = voice_tb[par_sy.top_voice];
                    return;
                }
                new_syst();
                s.sy = par_sy;
            }
            staves_found = maxtime;
            for (v = 0; v < nv; v++)
            {
                p_voice = voice_tb[v];
                delete p_voice.second;
                delete p_voice.floating;
                if (p_voice.ignore)
                {
                    p_voice.ignore = 0;
                    s = p_voice.sym;
                    if (s != null)
                    {
                        while (s.next)
                        {
                            s = s.next;
                        }
                    }
                    p_voice.last_sym = s;
                }
            }
            range = 0;
            for (i = 0; i < a_vf.Count; i++)
            {
                vid = a_vf[i][0];
                p_voice = new_voice(vid);
                p_voice.time = maxtime;
                v = p_voice.v;
                a_vf[i][0] = p_voice;
                while (true)
                {
                    par_sy.voices[v] = { range = range++ };
                    p_voice = p_voice.voice_down;
                    if (p_voice == null)
                    {
                        break;
                    }
                    v = p_voice.v;
                }
            }
            par_sy.top_voice = a_vf[0][0].v;
            if (a_vf.Count == 1)
            {
                par_sy.one_v = 1;
            }
            if (cmd[1] == 't')
            {
                for (i = 0; i < a_vf.Count; i++)
                {
                    flags = a_vf[i][1];
                    if (!(flags & (OPEN_BRACE | OPEN_BRACE2)))
                    {
                        continue;
                    }
                    if ((flags & (OPEN_BRACE | CLOSE_BRACE)) == (OPEN_BRACE | CLOSE_BRACE) || (flags & (OPEN_BRACE2 | CLOSE_BRACE2)) == (OPEN_BRACE2 | CLOSE_BRACE2))
                    {
                        continue;
                    }
                    if (a_vf[i + 1][1] != 0)
                    {
                        continue;
                    }
                    if ((flags & OPEN_PARENTH) || (a_vf[i + 2][1] & OPEN_PARENTH))
                    {
                        continue;
                    }
                    if (a_vf[i + 2][1] & (CLOSE_BRACE | CLOSE_BRACE2))
                    {
                        a_vf[i + 1][1] |= FL_VOICE;
                    }
                    else if (a_vf[i + 2][1] == 0 && (a_vf[i + 3][1] & (CLOSE_BRACE | CLOSE_BRACE2)))
                    {
                        a_vf[i][1] |= OPEN_PARENTH;
                        a_vf[i + 1][1] |= CLOSE_PARENTH;
                        a_vf[i + 2][1] |= OPEN_PARENTH;
                        a_vf[i + 3][1] |= CLOSE_PARENTH;
                    }
                }
            }
            st = -1;
            for (i = 0; i < a_vf.Count; i++)
            {
                flags = a_vf[i][1];
                if ((flags & (OPEN_PARENTH | CLOSE_PARENTH)) == (OPEN_PARENTH | CLOSE_PARENTH))
                {
                    flags &= ~(OPEN_PARENTH | CLOSE_PARENTH);
                    a_vf[i][1] = flags;
                }
                p_voice = a_vf[i][0];
                if (flags & FL_VOICE)
                {
                    p_voice.floating = true;
                    p_voice.second = true;
                }
                else
                {
                    st++;
                    if (par_sy.staves[st] == null)
                    {
                        par_sy.staves[st] = { stafflines = p_voice.stafflines || "|||||", staffscale = 1 };
                    }
                    par_sy.staves[st].flags = 0;
                }
                v = p_voice.v;
                p_voice.st = p_voice.cst = par_sy.voices[v].st = st;
                par_sy.staves[st].flags |= flags;
                if (flags & OPEN_PARENTH)
                {
                    p_voice2 = p_voice;
                    while (i < a_vf.Count - 1)
                    {
                        p_voice = a_vf[++i][0];
                        v = p_voice.v;
                        if (a_vf[i][1] & MASTER_VOICE)
                        {
                            p_voice2.second = true;
                            p_voice2 = p_voice;
                        }
                        else
                        {
                            p_voice.second = true;
                        }
                        p_voice.st = p_voice.cst = par_sy.voices[v].st = st;
                        if (a_vf[i][1] & CLOSE_PARENTH)
                        {
                            break;
                        }
                    }
                    par_sy.staves[st].flags |= a_vf[i][1];
                }
            }
            if (st < 0)
            {
                st = 0;
            }
            par_sy.nstaff = nstaff = st;
            if (cmd[1] == 'c')
            {
                for (st = 0; st < nstaff; st++)
                {
                    par_sy.staves[st].flags ^= STOP_BAR;
                }
            }
            nv = voice_tb.Count;
            st = 0;
            for (v = 0; v < nv; v++)
            {
                p_voice = voice_tb[v];
                if (par_sy.voices[v] != null)
                {
                    st = p_voice.st;
                }
                else
                {
                    p_voice.st = st;
                }
                if (p_voice.time == 0)
                {
                    for (s = p_voice.sym; s != null; s = s.next)
                    {
                        s.st = st;
                    }
                }
                if (par_sy.voices[v] != null)
                {
                    p_voice2 = p_voice.voice_down;
                    while (p_voice2 != null)
                    {
                        i = p_voice2.v;
                        p_voice2.st = p_voice2.cst = par_sy.voices[i].st = st;
                        p_voice2 = p_voice2.voice_down;
                    }
                    par_sy.voices[v].second = p_voice.second;
                    st = p_voice.st;
                    if (st > 0 && p_voice.norepbra == null && !(par_sy.staves[st - 1].flags & STOP_BAR))
                    {
                        p_voice.norepbra = true;
                    }
                }
            }
            curvoice = parse.state >= 2 ? voice_tb[par_sy.top_voice] : null;
        }

        // get a voice or create a clone of the current voice
        void clone_voice(string id)
        {
            int v, p_voice;

            for (v = 0; v < voice_tb.Count; v++)
            {
                p_voice = voice_tb[v];
                if (p_voice.id == id)
                {
                    return p_voice;
                }
            }
            p_voice = clone(curvoice);
            p_voice.v = voice_tb.Count;
            p_voice.id = id;
            p_voice.sym = p_voice.last_sym = null;
            p_voice.key = clone(curvoice.key);
            p_voice.sls = [];
            delete p_voice.nm;
            delete p_voice.snm;
            delete p_voice.new_name;
            delete p_voice.lyric_restart;
            delete p_voice.lyric_cont;
            delete p_voice.sym_restart;
            delete p_voice.sym_cont;
            delete p_voice.have_ly;
            delete p_voice.tie_s;
            voice_tb.Add(p_voice);
            return p_voice;
        }

        /* -- get a voice overlay -- */
        void get_vover(string type)
        {
            int p_voice2, p_voice3, range, s, time, v, v2, v3, s2;

            if (type == "|" || type == ")")
            {
                if (curvoice.last_note == null)
                {
                    syntax(1, errs.nonote_vo);
                    if (vover != null)
                    {
                        curvoice = vover.p_voice;
                        vover = null;
                    }
                    return;
                }
                curvoice.last_note.beam_end = true;
                if (vover == null)
                {
                    syntax(1, "Erroneous end of voice overlay");
                    return;
                }
                if (curvoice.time != vover.p_voice.time)
                {
                    if (!curvoice.ignore)
                    {
                        syntax(1, "Wrong duration in voice overlay");
                    }
                    if (curvoice.time > vover.p_voice.time)
                    {
                        vover.p_voice.time = curvoice.time;
                    }
                }
                curvoice.acc = [];
                p_voice2 = vover.p_voice;
                s = curvoice.last_sym;
                if (s.type == C.SPACE && p_voice2.last_sym.type != C.SPACE)
                {
                    s.p_v = p_voice2;
                    s.v = s.p_v.v;
                    while (s.prev.type == C.SPACE)
                    {
                        s = s.prev;
                        s.p_v = p_voice2;
                        s.v = s.p_v.v;
                    }
                    s2 = s.prev;
                    s2.next = null;
                    s.prev = p_voice2.last_sym;
                    s.prev.next = s;
                    p_voice2.last_sym = curvoice.last_sym;
                    curvoice.last_sym = s2;
                }
                curvoice = p_voice2;
                vover = null;
                return;
            }
            if (type == "(")
            {
                if (vover != null)
                {
                    syntax(1, "Voice overlay already started");
                    return;
                }
                vover = { p_voice = curvoice, time = curvoice.time };
                return;
            }
            if (curvoice.last_note == null)
            {
                syntax(1, errs.nonote_vo);
                return;
            }
            curvoice.last_note.beam_end = true;
            p_voice2 = curvoice.voice_down;
            if (p_voice2 == null)
            {
                p_voice2 = clone_voice(curvoice.id + 'o');
                curvoice.voice_down = p_voice2;
                p_voice2.time = 0;
                p_voice2.second = true;
                p_voice2.last_note = null;
                v2 = p_voice2.v;
                if (par_sy.voices[curvoice.v])
                {
                    par_sy.voices[v2] = { st = curvoice.st, second = true };
                    range = par_sy.voices[curvoice.v].range;
                    for (v = 0; v < par_sy.voices.Count; v++)
                    {
                        if (par_sy.voices[v] != null && par_sy.voices[v].range > range)
                        {
                            par_sy.voices[v].range++;
                        }
                    }
                    par_sy.voices[v2].range = range + 1;
                }
            }
            p_voice2.ulen = curvoice.ulen;
            p_voice2.dur_fact = curvoice.dur_fact;
            p_voice2.acc = [];
            if (vover == null)
            {
                s = curvoice.last_sym;
                if (s != null && s.time == curvoice.time)
                {
                    s3 = s;
                    s = s.prev;
                }
                if (s != null && s.time == curvoice.time && s.bar_type && s.bar_type[0] != ':')
                {
                    s3 = s;
                }
                if (s3 != null)
                {
                    s2 = curvoice.last_sym;
                    curvoice.last_sym = s3.prev;
                    sym_link(s);
                    s.next = s3;
                    s3.prev = s;
                    curvoice.last_sym = s2;
                    if (s.soln)
                    {
                        delete s.soln;
                        curvoice.eoln = true;
                    }
                }
                else
                {
                    sym_link(s);
                }
                if (s.prev)
                {
                    s.clef_small = 1;
                }
            }
            else
            {
                if (curvoice != vover.p_voice && curvoice.time != vover.p_voice.time)
                {
                    syntax(1, "Wrong duration in voice overlay");
                    if (curvoice.time > vover.p_voice.time)
                    {
                        vover.p_voice.time = curvoice.time;
                    }
                }
            }
            p_voice2.time = vover.time;
            curvoice = p_voice2;
        }




        // check if a clef, key or time signature may go at start of the current voice
        bool is_voice_sig()
        {
            List<object> s;

            if (curvoice.time != null)
                return false;
            if (curvoice.last_sym == null)
                return true;
            for (s = curvoice.last_sym; s != null; s = s.prev)
                if (w_tb[s.type] != 0)
                    return false;
            return true;
        }


        // treat a clef found in the tune body
        void get_clef(string s)
        {
            if (s.clef_type == 'p')
            {
                s2 = curvoice.ckey;
                s2.k_drum = 1;
                s2.k_sf = 0;
                s2.k_b40 = 2;
                s2.k_map = abc2svg.keys[7];
                if (curvoice.key == null)
                {
                    curvoice.key = s2;
                }
            }

            if (curvoice.time == 0 && is_voice_sig())
            {
                curvoice.clef = s;
                s.fmt = cfmt;
                return;
            }

            for (s2 = curvoice.last_sym; s2 != null && s2.time == curvoice.time; s2 = s2.prev)
            {
                if (w_tb[s2.type])
                {
                    break;
                }
            }
            if (s2 != null && s2.time == curvoice.time && s2.k_sf != null)
            {
                s3 = s2;
                s2 = s2.prev;
            }
            if (s2 != null && s2.time == curvoice.time && s2.bar_type && s2.bar_type[0] != ':')
            {
                s3 = s2;
            }
            if (s3 != null)
            {
                s2 = curvoice.last_sym;
                curvoice.last_sym = s3.prev;
                sym_link(s);
                s.next = s3;
                s3.prev = s;
                curvoice.last_sym = s2;
                if (s.soln)
                {
                    delete s.soln;
                    curvoice.eoln = true;
                }
            }
            else
            {
                sym_link(s);
            }

            if (s.prev)
            {
                s.clef_small = 1;
            }
        }

        // treat K: (kp = key signature + parameters)
        void get_key(string parm)
        {
            int v, p_voice, transp, sndtran, nt;
            List<byte> a = new_key(parm);
            byte[] s_key = a[0];
            List<byte> s = s_key;
            bool empty = s.k_sf == null && !s.k_a_acc;

            if (empty)
            {
                s.invis = 1;
            }

            if (parse.state == 1)
            {
                parse.ckey = s;
                if (empty)
                {
                    s_key.k_sf = 0;
                    s_key.k_none = true;
                    s_key.k_map = abc2svg.keys[7];
                }
                for (v = 0; v < voice_tb.Count; v++)
                {
                    p_voice = voice_tb[v];
                    p_voice.ckey = clone(s_key);
                }
                if (a.Count > 0)
                {
                    memo_kv_parm('*', a);
                    a = new List<byte>();
                }
                if (glovar.ulen == null)
                {
                    glovar.ulen = C.BLEN / 8;
                }
                goto_tune();
            }
            else if (!empty)
            {
                s.k_old_sf = curvoice.ckey.k_sf;
                curvoice.ckey = s;
                sym_link(s);
                if (curvoice.tr_sco)
                {
                    curvoice.tr_sco = 0;
                }
            }

            if (!curvoice)
            {
                if (voice_tb.Count == 0)
                {
                    curvoice = new_voice("1");
                    var def = true;
                }
                else
                {
                    curvoice = voice_tb[staves_found < 0 ? 0 : par_sy.top_voice];
                }
            }

            p_voice = curvoice.clone;
            if (p_voice)
            {
                curvoice.clone = null;
            }
            get_voice(curvoice.id + ' ' + a.join(' '));
            if (p_voice)
            {
                curvoice.clone = p_voice;
            }

            if (def)
            {
                curvoice.default = 1;
            }
        }

        // get / create a new voice
        void new_voice(string id)
        {
            int v, p_v_sav, p_voice = voice_tb[0], n = voice_tb.Count;

            if (n == 1 && p_voice.default_)
            {
                delete p_voice.default_;
                if (p_voice.time == 0)
                {
                    p_voice.id = id;
                    return p_voice;
                }
            }
            for (v = 0; v < n; v++)
            {
                p_voice = voice_tb[v];
                if (p_voice.id == id)
                {
                    return p_voice;
                }
            }

            p_voice = {
                v = v,
                id = id,
                time = 0,
                new = true,
                pos = {
                    //			dyn: 0,
                    //			gch: 0,
                    //			gst: 0,
                    //			orn: 0,
                    //			stm: 0,
                    //			tup: 0,
                    //			voc: 0,
                    //			vol: 0
                },
                scale = 1,
                //		st: 0,
                //		cst: 0,
                ulen = glovar.ulen,
                dur_fact = 1,
                //		key: clone(parse.ckey),		// key at start of tune (parse / gene)
                //		ckey: clone(parse.ckey),	// current key (parse / gene)
                meter = clone(glovar.meter),
                wmeasure = glovar.meter.wmeasure,
                staffnonote = 1,
                clef = {
                    type = C.CLEF,
                    clef_auto = true,
                    clef_type = "a",		// auto
                    time = 0
                },
                acc = [],		// accidentals of the measure (parse)
                sls = [],		// slurs - used in parsing and in generation
                hy_st = 0
            };

            voice_tb.Add(p_voice);

            if (parse.state == 3)
            {
                p_voice.ckey = clone(parse.ckey);
                if (p_voice.ckey.k_bagpipe && !p_voice.pos.stm)
                {
                    p_voice.pos = clone(p_voice.pos);
                    p_voice.pos.stm &= ~0x07;
                    p_voice.pos.stm |= C.SL_BELOW;
                }
            }

            return p_voice;
        }

        // this function is called at program start and on end of tune
        void init_tune()
        {
            nstaff = -1;
            voice_tb = new List<byte>();
            curvoice = null;
            new_syst(true);
            staves_found = -1;
            gene = { };
            a_de = [];
            cross = { };
        }

        // treat V: with many voices
        void do_cloning()
        {
            int i;
            clone = curvoice.clone;
            vs = clone.vs;
            a = clone.a;
            bol = clone.bol;
            eol = parse.eol;
            parse_sav = parse;
            file = parse.file;
            delete curvoice.clone;

            if (file[eol - 1] == '[')
            {
                eol--;
            }

            include++;
            for (i = 0; i < vs.Count; i++)
            {
                parse = Object.create(parse_sav);
                parse.line = Object.create(parse_sav.line);
                get_voice(vs[i] + ' ' + a.join(' '));
                tosvg(parse.fname, file, bol, eol);
            }
            include--;
            parse = parse_sav;
        }

        // treat a 'V:' info
        void get_voice(string parm)
        {
            int v, vs;
            List<byte> a = info_split(parm);
            string vid = a.shift();

            if (vid == null)
            {
                return;
            }

            if (curvoice.Count > 0 && curvoice.Contains("clone"))
            {
                do_cloning();
            }

            if (vid.IndexOf(',') > 0)
            {
                vs = vid.split(',');
            }
            else
            {
                vs = [vid];
            }

            if (parse.state < 2)
            {
                while (true)
                {
                    vid = vs.shift();
                    if (vid == null)
                    {
                        break;
                    }
                    if (a.Count > 0)
                    {
                        memo_kv_parm(vid, a);
                    }
                    if (vid != '*' && parse.state == 1)
                    {
                        curvoice = new_voice(vid);
                    }
                }
                return;
            }

            if (vid == '*')
            {
                syntax(1, "Cannot have V:* in tune body");
                return;
            }

            curvoice = new_voice(vs[0]);

            if (vs.Count > 1)
            {
                vs.shift();
                curvoice.clone = { vs = vs, a = a, bol = parse.iend };
                if (parse.file[curvoice.clone.bol - 1] != ']')
                {
                    curvoice.clone.bol++;
                }
            }

            set_kv_parm(a);

            key_trans();

            v = curvoice.v;
            if (curvoice.new)
            {
                delete curvoice.new;
                if (staves_found < 0)
                {
                    curvoice.st = curvoice.cst = ++nstaff;
                    par_sy.nstaff = nstaff;
                    par_sy.voices[v] = { st = nstaff, range = v };
                    par_sy.staves[nstaff] = { stafflines = curvoice.stafflines || "|||||", staffscale = 1 };
                }
                else if (par_sy.voices[v] == null)
                {
                    curvoice.ignore = 1;
                    return;
                }
            }

            if (!curvoice.filtered && par_sy.voices[v] != null && (parse.voice_opts || parse.tune_v_opts))
            {
                curvoice.filtered = true;
                voice_filter();
            }
        }

        // change state from 'tune header' to 'in tune body'
        // curvoice is defined when called from get_voice()
        void goto_tune()
        {
            int v, p_voice;

            set_page();
            write_heading();
            if (glovar.new_nbar)
            {
                gene.nbar = glovar.new_nbar;
                glovar.new_nbar = 0;
            }
            else
            {
                gene.nbar = 1;
            }

            parse.state = 3;

            for (v = 0; v < voice_tb.Count; v++)
            {
                p_voice = voice_tb[v];
                p_voice.ulen = glovar.ulen;
                if (parse.ckey.k_bagpipe && !p_voice.pos.stm)
                {
                    p_voice.pos = clone(p_voice.pos);
                    p_voice.pos.stm &= ~0x07;
                    p_voice.pos.stm |= C.SL_BELOW;
                }
            }

            if (staves_found < 0)
            {
                v = voice_tb.Count;
                par_sy.nstaff = nstaff = v - 1;
                while (--v >= 0)
                {
                    p_voice = voice_tb[v];
                    delete p_voice.new;
                    p_voice.st = p_voice.cst = v;
                    par_sy.voices[v] = { st = v, range = v };
                    par_sy.staves[v] = { stafflines = p_voice.stafflines || "|||||", staffscale = 1 };
                }
            }
        }
    }
}





class asdfasdf
{

    using System;
using System.Collections.Generic;

public class Program
{
    // constants
    private static readonly int OPEN_BRACE = 0x01;
    private static readonly int CLOSE_BRACE = 0x02;
    private static readonly int OPEN_BRACKET = 0x04;
    private static readonly int CLOSE_BRACKET = 0x08;
    private static readonly int OPEN_PARENTH = 0x10;
    private static readonly int CLOSE_PARENTH = 0x20;
    private static readonly int STOP_BAR = 0x40;
    private static readonly int FL_VOICE = 0x80;
    private static readonly int OPEN_BRACE2 = 0x0100;
    private static readonly int CLOSE_BRACE2 = 0x0200;
    private static readonly int OPEN_BRACKET2 = 0x0400;
    private static readonly int CLOSE_BRACKET2 = 0x0800;
    private static readonly int MASTER_VOICE = 0x1000;

    private static readonly double IN = 96; // resolution 96 PPI
    private static readonly double CM = 37.8; // 1 inch = 2.54 centimeter
    private static double YSTEP; // number of steps for y offsets

    private static readonly Dictionary<string, string> errs = new Dictionary<string, string>
    {
        { "bad_char", "Bad character '$1'" },
        { "bad_grace", "Bad character in grace note sequence" },
        { "bad_transp", "Bad transpose value" },
        { "bad_val", "Bad value in $1" },
        { "bar_grace", "Cannot have a bar in grace notes" },
        { "ignored", "$1: inside tune - ignored" },
        { "misplaced", "Misplaced '$1' in %%score" },
        { "must_note", "!$1! must be on a note" },
        { "must_note_rest", "!$1! must be on a note or a rest" },
        { "nonote_vo", "No note in voice overlay" },
        { "not_ascii", "Not an ASCII character" },
        { "not_enough_n", "Not enough notes/rests for %%repeat" },
        { "not_enough_m", "Not enough measures for %%repeat" },
        { "not_enough_p", "Not enough parameters in %%map" },
        { "not_in_tune", "Cannot have '$1' inside a tune" },
        { "notransp", "Cannot transpose with a temperament" }
    };

    private static dynamic self = new System.Dynamic.ExpandoObject();
    GlobalVar glovar = new
    {
        meter = new VoiceMeter
        {
            type = C.METER, // meter in tune header
            wmeasure = 1, // no M:
            a_meter = new List<object>() // default: none
        }
    };
    private static dynamic info = new System.Dynamic.ExpandoObject();
    private static Parse parse = new Parse
    {
        ctx = new System.Dynamic.ExpandoObject(),
        prefix = "%",
        state = 0,
        ottava = new List<object>(),
        line = new ScanBuf()
    };
    private static List<object> tunes = new List<object>();
    private static object psvg;

    public static object Clone(object obj, int? lvl = null)
    {
        if (obj == null)
            return obj;

        var tmp = Activator.CreateInstance(obj.GetType());
        foreach (var prop in obj.GetType().GetProperties())
        {
            if (lvl.HasValue && lvl.Value > 0 && prop.PropertyType.IsClass)
                prop.SetValue(tmp, Clone(prop.GetValue(obj), lvl - 1));
            else
                prop.SetValue(tmp, prop.GetValue(obj));
        }
        return tmp;
    }

    public static void ErrBld(int sev, string txt, string fn = null, int? idx = null)
    {
        if (user.errbld != null)
        {
            string severity = sev switch
            {
                0 => "warn",
                1 => "error",
                _ => "fatal"
            };
            user.errbld(severity, txt, fn, idx);
            return;
        }

        int i = 0, l = 0, c = 0;
        if (idx.HasValue && idx.Value >= 0)
        {
            while (true)
            {
                int j = parse.file.IndexOf('\n', i);
                if (j < 0 || j > idx.Value)
                    break;
                l++;
                i = j + 1;
            }
            c = idx.Value - i;
        }

        string h = "";
        if (fn != null)
        {
            h = fn;
            if (l > 0)
                h += ":" + (l + 1) + ":" + (c + 1);
            h += " ";
        }

        h += sev switch
        {
            0 => "Warning: ",
            1 => "Error: ",
            _ => "Internal bug: "
        };

        user.errmsg(h + txt, l, c);
    }

    public static void Error(int sev, VoiceItem s, string msg, object a1 = null, object a2 = null, object a3 = null, object a4 = null)
    {
        if (sev == 0 && cfmt.quiet)
            return;

        if (s != null && s.err)
            return;

        s.err = true;

        if (user.textrans != null)
        {
            var tmp = user.textrans[msg];
            if (tmp != null)
                msg = tmp;
        }

        if (a1 != null || a2 != null || a3 != null || a4 != null)
        {
            msg = System.Text.RegularExpressions.Regex.Replace(msg, @"\$\d", match =>
            {
                return match.Value switch
                {
                    "$1" => a1?.ToString(),
                    "$2" => a2?.ToString(),
                    "$3" => a3?.ToString(),
                    _ => a4?.ToString()
                };
            });
        }

        if (s != null && s.fname != null)
            ErrBld(sev, msg, s.fname, s.istart);
        else
            ErrBld(sev, msg);
    }

    public class ScanBuf
    {
        public string buffer;
        public int index;

        public char Char()
        {
            return buffer[index];
        }

        public char NextChar()
        {
            return buffer[++index];
        }

        public int GetInt()
        {
            int val = 0;
            char c = buffer[index];
            while (c >= '0' && c <= '9')
            {
                val = val * 10 + (c - '0');
                c = NextChar();
            }
            return val;
        }
    }

    public static void Syntax(int sev, string msg, object a1 = null, object a2 = null, object a3 = null, object a4 = null)
    {
        var s = new
        {
            fname = parse.fname,
            istart = parse.istart + parse.line.index
        };

        Error(sev, s, msg, a1, a2, a3, a4);
    }

    public static void JsInject(string js)
    {
        // Note: Evaluating JavaScript code in C# is not straightforward and generally not recommended.
        // This is a placeholder to indicate where the JavaScript code would be executed.
        // In a real-world scenario, you might use a JavaScript engine like Jint or ClearScript.
        throw new NotImplementedException("JavaScript evaluation is not implemented.");
    }
}




}


