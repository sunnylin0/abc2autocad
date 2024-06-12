var a_gch,
    a_dcn = [],
    multicol,
    maps = {}
var qplet_tb = new Int8Array([0, 1,]),
    ntb = "CDEab"
var reg_dur = /aawwd/
var nil = "0",
    char_tb = [nil, " ", "\n", nil]
interface Voice {
    acc_tie: number[];
    acc: number[];
}


var curvoice: Voice;
function parse_staves(p) {
    var v, vid,
        vids = {},
        a_vf = [],
        err = false,
        flags = 0,
        brace = 0,
        bracket = 0,
        parenth = 0,
        flags_st = 0,
        e,
        a = p.match(/[^[\]|{}()*+\s]+|[^\s]/g)

    if (!a) {
        syntax(1, errs.bad_val, "%%score")
        return // null
    }
    while (1) {
        e = a.shift()
        if (!e)
            break
        switch (e) {
            case '[':
                if (parenth || brace + bracket >= 2) {
                    syntax(1, errs.misplaced, '[');
                    err = true
                    break
                }
                flags |= brace + bracket == 0 ? OPEN_BRACKET : OPEN_BRACKET2;
                bracket++;
                flags_st <<= 8;
                flags_st |= OPEN_BRACKET
                break
            case '{':
                if (parenth || brace || bracket >= 2) {
                    syntax(1, errs.misplaced, '{');
                    err = true
                    break
                }
                flags |= !bracket ? OPEN_BRACE : OPEN_BRACE2;
                brace++;
                flags_st <<= 8;
                flags_st |= OPEN_BRACE
                break
            case '(':
                if (parenth) {
                    syntax(1, errs.misplaced, '(');
                    err = true
                    break
                }
                flags |= OPEN_PARENTH;
                parenth++;
                flags_st <<= 8;
                flags_st |= OPEN_PARENTH
                break
            case '*':
                if (brace && !parenth && !(flags & (OPEN_BRACE | OPEN_BRACE2)))
                    flags |= FL_VOICE
                break
            case '+':
                flags |= MASTER_VOICE
                break
            case ']':
            case '}':
            case ')':
                syntax(1, "Bad voice ID in %%score");
                err = true
                break
            default:	// get / create the voice in the voice table
                vid = e
                while (1) {
                    e = a.shift()
                    if (!e)
                        break
                    switch (e) {
                        case ']':
                            if (!(flags_st & OPEN_BRACKET)) {
                                syntax(1, errs.misplaced, ']');
                                err = true
                                break
                            }
                            bracket--;
                            flags |= brace + bracket == 0 ?
                                CLOSE_BRACKET :
                                CLOSE_BRACKET2;
                            flags_st >>= 8
                            continue
                        case '}':
                            if (!(flags_st & OPEN_BRACE)) {
                                syntax(1, errs.misplaced, '}');
                                err = true
                                break
                            }
                            brace--;
                            flags |= !bracket ?
                                CLOSE_BRACE :
                                CLOSE_BRACE2;
                            flags &= ~FL_VOICE;
                            flags_st >>= 8
                            continue
                        case ')':
                            if (!(flags_st & OPEN_PARENTH)) {
                                syntax(1, errs.misplaced, ')');
                                err = true
                                break
                            }
                            parenth--;
                            flags |= CLOSE_PARENTH;
                            flags_st >>= 8
                            continue
                        case '|':
                            flags |= STOP_BAR
                            continue
                    }
                    break
                }
                if (vids[vid]) {
                    syntax(1, "Double voice in %%score")
                    err = true
                } else {
                    vids[vid] = true
                    a_vf.push([vid, flags])
                }
                flags = 0
                if (!e)
                    break
                a.unshift(e)
                break
        }
    }
    if (flags_st != 0) {
        syntax(1, "'}', ')' or ']' missing in %%score");
        err = true
    }
    if (err || !a_vf.length)
        return //null
    return a_vf
}
function new_note (grace, sls) {
    var note, s, in_chord, c, dcn, type, tie_s, acc_tie,
        i, n, s2, nd, res, num, dur, apit, div, ty,
        dpit = 0,
        sl1 = [],
        line = parse.line,
        a_dcn_sav = a_dcn		// save parsed decoration names

    a_dcn = []
    parse.stemless = false;
    s = {
        type: C.NOTE,
        fname: parse.fname,
        stem: 0,
        multi: 0,
        nhd: 0,
        xmx: 0
    }
    s.istart = parse.bol + line.index

    if (curvoice.color)
        s.color = curvoice.color

    if (grace) {
        s.grace = true
    } else {
        if (curvoice.tie_s) {	// if tie from previous note / grace note
            tie_s = curvoice.tie_s
            curvoice.tie_s = null
        }
        if (a_gch)
            csan_add(s)
        if (parse.repeat_n) {
            s.repeat_n = parse.repeat_n;
            s.repeat_k = parse.repeat_k;
            parse.repeat_n = 0
        }
    }
    c = line.char()
    switch (c) {
        case 'X':
            s.invis = true
        case 'Z':
            s.type = C.MREST;
            c = line.next_char()
            s.nmes = (c > '0' && c <= '9') ? line.get_int() : 1;
            if (curvoice.wmeasure == 1) {
                error(1, s, "multi-measure rest, but no measure!")
                return
            }
            s.dur = curvoice.wmeasure * s.nmes

            // ignore if in second voice
            if (curvoice.second) {
                delete curvoice.eoln	// ignore the end of line
                curvoice.time += s.dur
                return //null
            }

            // convert 'Z'/'Z1' to a whole measure rest
            if (s.nmes == 1) {
                s.type = C.REST;
                s.dur_orig = s.dur;
                s.fmr = 1		// full measure rest
                s.notes = [{
                    pit: 18,
                    dur: s.dur
                }]
            } else {
                glovar.mrest_p = true
                if (par_sy.voices.length == 1) {
                    s.tacet = curvoice.tacet
                    delete s.invis	// show the 'H' when 'Xn'
                }
            }
            break
        case 'y':
            s.type = C.SPACE;
            s.invis = true;
            s.dur = 0;
            c = line.next_char()
            if (c >= '0' && c <= '9')
                s.width = line.get_int()
            else
                s.width = 10
            if (tie_s) {
                curvoice.tie_s = tie_s
                tie_s = null
            }
            break
        case 'x':
            s.invis = true
        case 'z':
            s.type = C.REST;
            line.index++;
            nd = parse_dur(line);
            s.dur_orig = ((curvoice.ulen < 0) ?
                C.BLEN :
                curvoice.ulen) * nd[0] / nd[1];
            s.dur = s.dur_orig * curvoice.dur_fact;
            if (s.dur == curvoice.wmeasure)
                s.fmr = 1		// full measure rest
            s.notes = [{
                pit: 18,
                dur: s.dur_orig
            }]
            break
        case '[':			// chord
            in_chord = true;
            c = line.next_char()
        // fall thru
        default:			// accidental, chord, note
            if (curvoice.acc_tie) {
                acc_tie = curvoice.acc_tie
                curvoice.acc_tie = null
            }
            s.notes = []

            // loop on the chord
            while (1) {

                // when in chord, get the slurs and decorations
                if (in_chord) {
                    while (1) {
                        if (!c)
                            break
                        i = c.charCodeAt(0);
                        if (i >= 128) {
                            syntax(1, errs.not_ascii)
                            return //null
                        }
                        type = char_tb[i]
                        switch (type[0]) {
                            case '(':
                                sl1.push(parse_vpos());
                                c = line.char()
                                continue
                            case '!':
                                if (type.length > 1)
                                    a_dcn.push(type.slice(1, -1))
                                else
                                    get_deco()	// line -> a_dcn
                                c = line.next_char()
                                continue
                        }
                        break
                    }
                }
                note = parse_basic_note(line,
                    s.grace ? C.BLEN / 4 :
                        curvoice.ulen < 0 ?
                            C.BLEN :
                            curvoice.ulen)
                if (!note)
                    return //null

                if (curvoice.octave)
                    note.pit += curvoice.octave * 7

                // get the real accidental
                apit = note.pit + 19		// pitch from C-1
                i = note.acc
                if (!i) {
                    if (cfmt["propagate-accidentals"][0] == 'p')
                        i = curvoice.acc[apit % 7]
                    else
                        i = curvoice.acc[apit]
                    if (!i)
                        i = curvoice.ckey.k_map[apit % 7] || 0
                }

                if (i) {
                    if (cfmt["propagate-accidentals"][0] == 'p')
                        curvoice.acc[apit % 7] = i
                    else if (cfmt["propagate-accidentals"][0] != 'n')
                        curvoice.acc[apit] = i
                }

                if (acc_tie && acc_tie[apit])
                    i = acc_tie[apit]	// tied note

                // map
                if (curvoice.map
                    && maps[curvoice.map])
                    set_map(note, i)

                // set the MIDI pitch
                if (!note.midi)		// if not map play
                    note.midi = pit2mid(apit, i)

                // transpose
                if (curvoice.tr_sco
                    && !note.notrp) {
                    i = nt_trans(note, i)
                    if (i == -3) {		// if triple sharp/flat
                        error(1, s, "triple sharp/flat")
                        i = note.acc > 0 ? 1 : -1
                        note.pit += i
                        note.acc = i
                    }
                    dpit = note.pit + 19 - apit
                }
                if (curvoice.tr_snd)
                    note.midi += curvoice.tr_snd

                //fixme: does not work if transposition
                if (i) {
                    switch (cfmt["writeout-accidentals"][1]) {
                        case 'd':			// added
                            s2 = curvoice.ckey
                            if (!s2.k_a_acc)
                                break
                            for (n = 0; n < s2.k_a_acc.length; n++) {
                                if ((s2.k_a_acc[n].pit - note.pit)
                                    % 7 == 0) {
                                    note.acc = i
                                    break
                                }
                            }
                            break
                        case 'l':			// all
                            note.acc = i
                            break
                    }
                }

                // starting slurs
                if (sl1.length) {
                    while (1) {
                        i = sl1.shift()
                        if (!i)
                            break
                        curvoice.sls.push({
                            ty: i,
                            ss: s,
                            nts: note	// starting note
                        })
                    }
                }
                if (a_dcn.length) {
                    s.time = curvoice.time	// (needed for !tie)!
                    dh_cnv(s, note)
                }
                s.notes.push(note)
                if (!in_chord)
                    break

                // in chord: get the ending slurs and the ties
                c = line.char()
                while (1) {
                    switch (c) {
                        case ')':
                            slur_add(s, note)
                            c = line.next_char()
                            continue
                        case '-':
                            note.tie_ty = parse_vpos()
                            note.s = s
                            curvoice.tie_s = s
                            s.ti1 = true
                            if (curvoice.acc[apit]
                                || (acc_tie
                                    && acc_tie[apit])) {
                                if (!curvoice.acc_tie)
                                    curvoice.acc_tie = []
                                i = curvoice.acc[apit]
                                if (acc_tie && acc_tie[apit])
                                    i = acc_tie[apit]
                                curvoice.acc_tie[apit] = i
                            }
                            c = line.char()
                            continue
                        case '.':
                            c = line.next_char()
                            switch (c) {
                                case '-':
                                case '(':
                                    a_dcn.push("dot")
                                    continue
                            }
                            syntax(1, "Misplaced dot")
                            break
                    }
                    break
                }
                if (c == ']') {
                    line.index++;

                    // adjust the chord duration
                    nd = parse_dur(line);
                    s.nhd = s.notes.length - 1
                    for (i = 0; i <= s.nhd; i++) {
                        note = s.notes[i];
                        note.dur = note.dur * nd[0] / nd[1]
                    }
                    break
                }
            }

            // handle the starting slurs
            if (sls.length) {
                while (1) {
                    i = sls.shift()
                    if (!i)
                        break
                    curvoice.sls.push({
                        ty: i,
                        ss: s
                        // no starting note
                    })
                    if (grace)
                        curvoice.sls[curvoice.sls.length - 1].grace =
                            grace
                }
            }

            // the duration of the chord is the duration of the 1st note
            s.dur_orig = s.notes[0].dur;
            s.dur = s.notes[0].dur * curvoice.dur_fact
            break
    }
    if (s.grace && s.type != C.NOTE) {
        syntax(1, errs.bad_grace)
        return //null
    }

    if (s.notes) {				// if note or rest
        if (!grace) {
            switch (curvoice.pos.stm & 0x07) {
                case C.SL_ABOVE: s.stem = 1; break
                case C.SL_BELOW: s.stem = -1; break
                case C.SL_HIDDEN: s.stemless = true; break
            }

            // adjust the symbol duration
            num = curvoice.brk_rhythm
            if (num) {
                curvoice.brk_rhythm = 0;
                s2 = curvoice.last_note
                if (num > 0) {
                    n = num * 2 - 1;
                    s.dur = s.dur * n / num;
                    s.dur_orig = s.dur_orig * n / num
                    for (i = 0; i <= s.nhd; i++)
                        s.notes[i].dur =
                            s.notes[i].dur * n / num;
                    s2.dur /= num;
                    s2.dur_orig /= num
                    for (i = 0; i <= s2.nhd; i++)
                        s2.notes[i].dur /= num
                } else {
                    num = -num;
                    n = num * 2 - 1;
                    s.dur /= num;
                    s.dur_orig /= num
                    for (i = 0; i <= s.nhd; i++)
                        s.notes[i].dur /= num;
                    s2.dur = s2.dur * n / num;
                    s2.dur_orig = s2.dur_orig * n / num
                    for (i = 0; i <= s2.nhd; i++)
                        s2.notes[i].dur =
                            s2.notes[i].dur * n / num
                }
                curvoice.time = s2.time + s2.dur;

                // adjust the time of the grace notes, bars...
                for (s2 = s2.next; s2; s2 = s2.next)
                    s2.time = curvoice.time
            }
        } else {		/* grace note - adjust its duration */
            div = curvoice.ckey.k_bagpipe ? 8 : 4
            for (i = 0; i <= s.nhd; i++)
                s.notes[i].dur /= div;
            s.dur /= div;
            s.dur_orig /= div
            if (grace.stem)
                s.stem = grace.stem
        }

        curvoice.last_note = s

        // get the possible ties and end of slurs
        c = line.char()
        while (1) {
            switch (c) {
                case '.':
                    if (line.buffer[line.index + 1] != '-')
                        break
                    a_dcn.push("dot")
                    line.index++
                // fall thru
                case '-':
                    ty = parse_vpos()
                    for (i = 0; i <= s.nhd; i++) {
                        s.notes[i].tie_ty = ty
                        s.notes[i].s = s
                    }
                    curvoice.tie_s = grace || s
                    curvoice.tie_s.ti1 = true
                    for (i = 0; i <= s.nhd; i++) {
                        note = s.notes[i]
                        apit = note.pit + 19	// pitch from C-1
                            - dpit		// (if transposition)
                        if (curvoice.acc[apit]
                            || (acc_tie
                                && acc_tie[apit])) {
                            if (!curvoice.acc_tie)
                                curvoice.acc_tie = []
                            n = curvoice.acc[apit]
                            if (acc_tie && acc_tie[apit])
                                n = acc_tie[apit]
                            curvoice.acc_tie[apit] = n
                        }
                    }
                    c = line.char()
                    continue
            }
            break
        }

        // handle the ties ending on this chord/note
        if (tie_s)		// if tie from previous note / grace note
            do_ties(s, tie_s)
    }

    sym_link(s)

    if (!grace) {
        if (!curvoice.lyric_restart)
            curvoice.lyric_restart = s
        if (!curvoice.sym_restart)
            curvoice.sym_restart = s
    }

    if (a_dcn_sav.length) {
        a_dcn = a_dcn_sav
        deco_cnv(s, s.prev)
    }
    if (grace && s.ottava)
        grace.ottava = s.ottava
    if (parse.stemless)
        s.stemless = true
    s.iend = parse.bol + line.index
    return s
}