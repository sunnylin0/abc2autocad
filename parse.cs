using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

//var a_gch,
//    a_dcn = [],
//    multicol,
//    maps = { }	
//var qplet_tb = new Int8Array([0, 1,]),
//    ntb = "CDEab"
//  var reg_dur = /aawwd/
//    var nil = "0",
//    char_tb = [nil, " ", "\n", nil]

//interface Voice
//{
//    acc_tie?: number[];
//    acc?: number[];
//};

//var curvoice: Voice;



namespace autocad_part2
{


    var a_gch = new List<object>();     // array of parsed guitar chords
    var a_dcn = new List<object>();     // array of parsed decoration names
    object multicol;        // multi column object
    var maps = new Dictionary<object, object>();        // maps object - see set_map()
    var qplet_tb = new sbyte[] { 0, 1, 3, 2, 3, 0, 2, 0, 3, 0 };
    var ntb = "CDEFGABcdefgab";

    // parse a duration and return [numerator, denominator]
    // 'line' is not always 'parse.line'
    var reg_dur = new System.Text.RegularExpressions.Regex(@" (\d*)(\/*)(\d*)");        /* (stop comment) */

    var nil = "0";
    var char_tb = new string[]
    {
                nil, nil, nil, nil,		/* 00 - .. */
                nil, nil, nil, nil,
                nil, " ", "\n", nil,		/* . \t \n . */
                nil, nil, nil, nil,
                nil, nil, nil, nil,
                nil, nil, nil, nil,
                nil, nil, nil, nil,
                nil, nil, nil, nil,		/* .. - 1f */
                " ", "!", "\"", "i",		/* (sp) ! " # */
                "\n", nil, "&", nil,		/* $ % & ' */
                "(", ")", "i", nil,		/* ( ) * + */
                nil, "-", "!dot!", nil,		/* , - . / */
                nil, nil, nil, nil, 		/* 0 1 2 3 */
                nil, nil, nil, nil, 		/* 4 5 6 7 */
                nil, nil, "|", "i",		/* 8 9 : ; */
                "<", "n", "<", "i",		/* < = > ? */
                "i", "n", "n", "n",		/* @ A B C */
                "n", "n", "n", "n", 		/* D E F G */
                "!fermata!", "d", "d", "d",	/* H I J K */
                "!emphasis!", "!lowermordent!",
                "d", "!coda!",		/* L M N O */
                "!uppermordent!", "d",
                "d", "!segno!",		/* P Q R S */
                "!trill!", "d", "d", "d",	/* T U V W */
                "n", "d", "n", "[",		/* X Y Z [ */
                "\\", "|", "n", "n",		/* \ ] ^ _ */
                "i", "n", "n", "n",	 	/* ` a b c */
                "n", "n", "n", "n",	 	/* d e f g */
                "d", "d", "d", "d",		/* h i j k */
                "d", "d", "d", "d",		/* l m n o */
                "d", "d", "d", "d",		/* p q r s */
                "d", "!upbow!",
                "!downbow!", "d",	/* t u v w */
                "n", "n", "n", "{",		/* x y z { */
                "|", "}", "!gmark!", nil,   /* | } ~ (del) */
    }; // char_tb[]

    void set_ref(object s)
    {
        s.fname = parse.fname;
        s.istart = parse.istart;
        s.iend = parse.iend;
    }

    object new_clef(string clef_def)
    {
        var s = new Dictionary<string, object>
                {
                    { "type", C.CLEF },
                    { "clef_line", 2 },
                    { "clef_type", "t" },
                    { "v", curvoice.v },
                    { "p_v", curvoice },
                    { "time", curvoice.time },
                    { "dur", 0 }
                };
        var i = 1;

        set_ref(s);

        switch (clef_def[0])
        {
            case '"':
                i = clef_def.IndexOf('"', 1);
                s["clef_name"] = clef_def.Substring(1, i - 1);
                i++;
                break;
            case 'a':
                if (clef_def[1] == 'u')    // auto
                {
                    s["clef_type"] = "a";
                    s["clef_auto"] = true;
                    i = 4;
                    break;
                }
                i = 4;                // alto
                goto case 'C';
            case 'C':
                s["clef_type"] = "c";
                s["clef_line"] = 3;
                break;
            case 'b':                // bass
                i = 4;
                goto case 'F';
            case 'F':
                s["clef_type"] = "b";
                s["clef_line"] = 4;
                break;
            case 'n':                // none
                i = 4;
                s["invis"] = true;
                s["clef_none"] = true; //true
                break;
            case 't':
                if (clef_def[1] == 'e')    // tenor
                {
                    s["clef_type"] = "c";
                    s["clef_line"] = 4;
                    break;
                }
                i = 6;
                goto case 'G';
            case 'G':
                //        s.clef_type = "t"        // treble
                break;
            case 'p':
                i = 4;
                goto case 'P';                // perc
            case 'P':
                s["clef_type"] = "p";
                s["clef_line"] = 3;
                break;
            default:
                syntax(1, "Unknown clef '$1'", clef_def);
                return null;
        }
        if (clef_def[i] >= '1' && clef_def[i] <= '9')
        {
            s["clef_line"] = int.Parse(clef_def[i].ToString());
            i++;
        }

        // handle the octave (+/-8 - ^/_8)
        curvoice.snd_oct = null;
        if (clef_def[i + 1] != '8'
            && clef_def[i + 1] != '1')
            return s;
        switch (clef_def[i])            // octave
        {
            case '^':
                s["clef_oct_transp"] = true;
                goto case '+';
            case '+':
                s["clef_octave"] = clef_def[i + 1] == '8' ? 7 : 14;
                if (!(bool)s["clef_oct_transp"])        // MIDI higher octave
                    curvoice.snd_oct = clef_def[i + 1] == '8' ? 12 : 24;
                break;
            case '_':
                s["clef_oct_transp"] = true;
                goto case '-';
            case '-':
                s["clef_octave"] = clef_def[i + 1] == '8' ? -7 : -14;
                if (!(bool)s["clef_oct_transp"])        // MIDI lower octave
                    curvoice.snd_oct = clef_def[i + 1] == '8' ? -12 : -24;
                break;
        }
        return s;
    }
    object get_interval(object param, object score = null)
    {
        int i;
        object val, tmp, note, pit;

        tmp = new scanBuf();
        tmp.buffer = param;
        pit = new object[2];
        for (i = 0; i < 2; i++)
        {
            note = tmp.buffer[tmp.index] != null ? parse_acc_pit(tmp) : null;
            if (note == null)
            {
                if (i != 1 || score == null)
                {
                    syntax(1, errs.bad_transp);
                    return null;
                }
                pit[i] = 242;            // 'c' (C5)
            }
            else
            {
                if (note.acc.GetType() == typeof(object[]))
                {
                    syntax(1, errs.bad_transp);
                    return null;
                }
                pit[i] = abc2svg.pab40(note.pit, note.acc);
            }
        }
        return pit[1] - pit[0];
    }
    object nt_trans(object nt,
        object a)
    {            // real accidental
        int ak, an, d, b40, n;

        if (a.GetType() == typeof(object[]))
        {        // if microtonal accidental
            n = (int)a[0];            // numerator
            d = (int)a[1];            // denominator
            a = n > 0 ? 1 : -1;        // base accidental for transpose
        }

        b40 = abc2svg.pab40(nt.pit, (int)a)
            + curvoice.tr_sco;        // base-40 transposition

        nt.pit = abc2svg.b40p(b40);        // new pitch
        an = abc2svg.b40a(b40);            // new accidental

        if (d == 0)
        {                // if not a microtonal accidental
            if (an == -3)            // if triple sharp/flat
                return an;
            a = an;
            if (nt.acc != null)
            {            // if old accidental
                if (a == null)
                    a = 3;        // required natural
            }
            else
            {
                if (!curvoice.ckey.k_none) // if normal key
                    a = 0;        // no accidental
            }
            nt.acc = a;
            return an;
        }

        // set the microtonal accidental after transposition
        switch (an)
        {
            case -2:
                if (n > 0)
                    n -= d * 2;
                else
                    n -= d;
                break;
            case -1:
                if (n > 0)
                    n -= d;
                break;
            case 0:
            case 3:
                if (n > 0)
                    n -= d;
                else
                    n += d;
                break;
            case 1:
                if (n < 0)
                    n += d;
                break;
            case 2:
                if (n < 0)
                    n += d * 2;
                else
                    n += d;
                break;
        }
        nt.acc = new object[] { n, d };
        return an;
    }
    void set_linebreak(object param)
    {
        int i;
        object item;

        for (i = 0; i < 128; i++)
        {
            if (char_tb[i] == "\n")
                char_tb[i] = nil;    // remove old definition
        }
        param = param.ToString();
        var paramArr = param.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        for (i = 0; i < paramArr.Length; i++)
        {
            item = paramArr[i];
            switch (item.ToString())
            {
                case "!":
                case "$":
                case "*":
                case ";":
                case "?":
                case "@":
                    break;
                case "<none>":
                    continue;
                case "<EOL>":
                    item = '\n';
                    break;
                default:
                    syntax(1, "Bad value '$1' in %%linebreak - ignored",
                        item);
                    continue;
            }
            char_tb[(int)item.ToString()[0]] = '\n';
        }
    }
    void set_user(object parm)
    {
        int k;
        char c;
        object v;
        var a = System.Text.RegularExpressions.Regex.Match(parm.ToString(), @"(.)[=\s]*(\[I:.+\]|"".+""|!.+!)");

        if (!a.Success)
        {
            syntax(1, 'Lack of starting [, ! or " in U: / %%user');
            return;
        }
        c = a.Groups[1].Value[0];
        v = a.Groups[2].Value;
        if (c == '\\')
        {
            if (c == 't')
                c = '\t';
            else if (c == null)
                c = ' ';
        }

        k = (int)c;
        if (k >= 128)
        {
            syntax(1, errs.not_ascii);
            return;
        }
        switch (char_tb[k][0])
        {
            case '0':            // nil
            case 'd':
            case 'i':
            case ' ':
                break;
            case '"':
            case '!':
            case '[':
                if (char_tb[k].Length > 1)
                    break;
            // fall thru
            default:
                syntax(1, "Bad user character '$1'", c);
                return;
        }
        switch (v.ToString())
        {
            case "!beambreak!":
                v = " ";
                break;
            case "!ignore!":
                v = "i";
                break;
            case "!nil!":
            case "!none!":
                v = "d";
                break;
        }
        char_tb[k] = v.ToString();
    }
    object get_st_lines(object param)
    {
        if (param == null)
            return null;
        if (System.Text.RegularExpressions.Regex.IsMatch(param.ToString(), @"^[\]\[|.-]+$"))
            return System.Text.RegularExpressions.Regex.Replace(param.ToString(), @"\]", "[");

        var n = int.Parse(param.ToString());
        switch (n)
        {
            case 0: return "...";
            case 1: return "..|";
            case 2: return ".||";
            case 3: return ".|||";
        }
        if (int.TryParse(param.ToString(), out n) || n < 0 || n > 16)
            return null;
        return "||||||||||||||||".Substring(0, n);
    }
    object new_block(object subtype)
    {
        object c_v;
        var s = new Dictionary<string, object>
                {
                    { "type", C.BLOCK },
                    { "subtype", subtype },
                    { "dur", 0 }
                };

        c_v = curvoice;
        if (subtype.ToString().Substring(0, 4) != "midi")    // if not a play command
            curvoice = voice_tb[0];    // set the block in the first voice
        sym_link(s);
        if (c_v != null)
            curvoice = c_v;
        return s;
    }

    void set_vp(List<object> a)
    {
        object s, item, pos, val, clefpit;
        int tr_p = 0;

        while (true)
        {
            item = a[0];
            a.RemoveAt(0);
            if (item == null)
                break;
            if (item.ToString().Substring(item.ToString().Length - 1) == "=" && a.Count == 0)
            {
                syntax(1, errs.bad_val, item);
                break;
            }
            switch (item.ToString())
            {
                case "clef=":
                    s = a[0];
                    a.RemoveAt(0);
                    break;
                case "clefpitch=":
                    item = a[0];
                    a.RemoveAt(0);
                    if (item != null)
                    {
                        val = ntb.IndexOf(item.ToString()[0]);
                        if (val >= 0)
                        {
                            switch (item.ToString()[1])
                            {
                                case "'":
                                    val += 7;
                                    break;
                                case ',':
                                    val -= 7;
                                    if (item.ToString()[2] == ',')
                                        val -= 7;
                                    break;
                            }
                            clefpit = 4 - val;
                            break;
                        }
                    }
                    syntax(1, errs.bad_val, item);
                    break;
                case "octave=":
                    val = Convert.ToInt32(a[0]);
                    a.RemoveAt(0);
                    if (double.IsNaN(val))
                        syntax(1, errs.bad_val, item);
                    else
                        curvoice.octave = val;
                    break;
                case "cue=":
                    curvoice.scale = a[0].ToString() == "on" ? 0.7 : 1;
                    a.RemoveAt(0);
                    break;
                case "instrument=":
                    item = a[0];
                    a.RemoveAt(0);
                    val = item.ToString().IndexOf('/');
                    if (val < 0)
                    {
                        val = get_interval('c' + item.ToString());
                        if (val == null)
                            break;
                        curvoice.sound = val;
                        tr_p |= 2;
                        val = 0;
                    }
                    else
                    {
                        val = get_interval('c' + item.ToString().Substring(val + 1));
                        if (val == null)
                            break;
                        curvoice.sound = val;
                        tr_p |= 2;
                        val = get_interval(item.ToString().Replace('/', ''));
                        if (val == null)
                            break;
                    }
                    curvoice.score = cfmt.sound ? curvoice.sound : val;
                    tr_p |= 1;
                    break;
                case "map=":
                    curvoice.map = a[0];
                    a.RemoveAt(0);
                    break;
                case "name=":
                case "nm=":
                    curvoice.nm = a[0];
                    if (curvoice.nm.ToString()[0] == '"')
                        curvoice.nm = cnv_escape(curvoice.nm.ToString().Substring(1, curvoice.nm.ToString().Length - 2));
                    curvoice.new_name = true;
                    a.RemoveAt(0);
                    break;
                case "stem=":
                case "pos=":
                    if (item.ToString() == "pos=")
                        item = a[0].ToString().Substring(1, a[0].ToString().Length - 2).Split(' ');
                    else
                        item = new object[] { "stm", a[0] };
                    val = posval[item.ToString()[1]];
                    if (val == null)
                    {
                        syntax(1, errs.bad_val, "%%pos");
                        break;
                    }
                    switch (item.ToString()[2])
                    {
                        case "align":
                            val |= C.SL_ALIGN;
                            break;
                        case "center":
                            val |= C.SL_CENTER;
                            break;
                        case "close":
                            val |= C.SL_CLOSE;
                            break;
                    }
                    if (pos == null)
                        pos = new Dictionary<object, object>();
                    pos[item.ToString()[0]] = val;
                    break;
                case "scale=":
                    val = Convert.ToDouble(a[0]);
                    a.RemoveAt(0);
                    if (double.IsNaN(val) || val < 0.5 || val > 2)
                        syntax(1, errs.bad_val, "%%voicescale");
                    else
                        curvoice.scale = val;
                    break;
                case "score=":
                    if (cfmt.nedo)
                    {
                        syntax(1, errs.notransp);
                        break;
                    }
                    item = a[0];
                    if (cfmt.sound)
                        break;
                    val = get_interval(item.ToString(), true);
                    if (val != null)
                    {
                        curvoice.score = val;
                        tr_p |= 1;
                    }
                    break;
                case "shift=":
                    if (cfmt.nedo)
                    {
                        syntax(1, errs.notransp);
                        break;
                    }
                    val = get_interval(a[0].ToString());
                    if (val != null)
                    {
                        curvoice.shift = val;
                        tr_p = 3;
                    }
                    break;
                case "sound=":
                    if (cfmt.nedo)
                    {
                        syntax(1, errs.notransp);
                        break;
                    }
                    val = get_interval(a[0].ToString());
                    if (val == null)
                        break;
                    curvoice.sound = val;
                    if (cfmt.sound)
                        curvoice.score = val;
                    tr_p |= 2;
                    break;
                case "subname=":
                case "sname=":
                case "snm=":
                    curvoice.snm = a[0];
                    if (curvoice.snm.ToString()[0] == '"')
                        curvoice.snm = curvoice.snm.ToString().Substring(1, curvoice.snm.ToString().Length - 2);
                    a.RemoveAt(0);
                    break;
                case "stafflines=":
                    val = get_st_lines(a[0]);
                    if (val == null)
                        syntax(1, "Bad %%stafflines value");
                    else if (curvoice.st != null)
                        par_sy.staves[curvoice.st].stafflines = val;
                    else
                        curvoice.stafflines = val;
                    a.RemoveAt(0);
                    break;
                case "staffnonote=":
                    val = Convert.ToInt32(a[0]);
                    if (double.IsNaN(val))
                        syntax(1, "Bad %%staffnonote value");
                    else
                        curvoice.staffnonote = val;
                    a.RemoveAt(0);
                    break;
                case "staffscale=":
                    val = Convert.ToDouble(a[0]);
                    if (double.IsNaN(val) || val < 0.3 || val > 2)
                        syntax(1, "Bad %%staffscale value");
                    else
                        curvoice.staffscale = val;
                    a.RemoveAt(0);
                    break;
                case "tacet=":
                    val = a[0];
                    curvoice.tacet = val != null ? val.ToString() : null;
                    a.RemoveAt(0);
                    break;
                case "transpose=":
                    if (cfmt.nedo)
                    {
                        syntax(1, errs.notransp);
                        break;
                    }
                    val = get_transp(a[0].ToString());
                    if (val == null)
                    {
                        syntax(1, errs.bad_transp);
                    }
                    else
                    {
                        curvoice.sound = val;
                        if (cfmt.sound)
                            curvoice.score = val;
                        tr_p = 2;
                    }
                    break;
                default:
                    switch (item.ToString().Substring(0, 4))
                    {
                        case "treb":
                        case "bass":
                        case "alto":
                        case "teno":
                        case "perc":
                            s = item;
                            break;
                        default:
                            if ("GFC".IndexOf(item.ToString()[0]) >= 0)
                                s = item;
                            else if (item.ToString().Substring(item.ToString().Length - 1) == "=")
                                a.RemoveAt(0);
                            break;
                    }
                    break;
            }
        }
        if (pos != null)
        {
            curvoice.pos = clone(curvoice.pos);
            foreach (var item in pos)
            {
                curvoice.pos[item.Key] = item.Value;
            }
        }

        if (s != null)
        {
            s = new_clef(s);
            if (s != null)
            {
                if (clefpit != null)
                    s.clefpit = clefpit;
                get_clef(s);
            }
        }

        if ((tr_p & 2) != 0)
        {
            tr_p = (int)curvoice.sound + (int)curvoice.shift;
            if (tr_p != 0)
                curvoice.tr_snd = abc2svg.b40m(tr_p + 122) - 36;
            else if (curvoice.tr_snd != null)
                curvoice.tr_snd = 0;
        }
    }
    void set_kv_parm(object a)    // array of items
    {
        if (curvoice.init == null)
        {    // add the global parameters if not done yet
            curvoice.init = true;
            if (info.V != null)
            {
                if (info.V[curvoice.id] != null)
                    a = info.V[curvoice.id].Concat(a);
                if (info.V['*'] != null)
                    a = info.V['*'].Concat(a);
            }
        }
        if (a.Length > 0)
            self.set_vp(a);
    }
    void memo_kv_parm(object vid,    // voice ID (V:) / '*' (K:/V:*)
        object a)    // array of items
    {
        if (a.Length == 0)
            return;
        if (info.V == null)
            info.V = new Dictionary<object, object>();
        if (info.V[vid] != null)
            info.V[vid].AddRange(a);
        else
            info.V[vid] = a;
    }


    /*************************************************************/

    var a_gch = new List<object>();
    var a_dcn = new List<object>();
    object multicol;
    var maps = new Dictionary<object, object>();
    var qplet_tb = new sbyte[] { 0, 1 };
    var ntb = "CDEab";
    var reg_dur = new System.Text.RegularExpressions.Regex("aawwd");
    var nil = "0";
    var char_tb = new object[] { nil, " ", "\n", nil };



    void new_key(object param)
    {
        int i, key_end;
        object c, tmp, note;
        int sf = "FCGDAEB".IndexOf(param.ToString()[0]) - 1;
        int mode = 0;
        var s = new Dictionary<object, object>
            {
                { "type", C.KEY },
                { "dur", 0 }
            };

        set_ref(s);

        i = 1;
        if (sf < -1)
        {
            switch (param.ToString()[0])
            {
                case 'H':
                    key_end = 1;
                    if (param.ToString()[1].ToString().ToLower() != "p")
                    {
                        syntax(1, "Unknown bagpipe-like key");
                        break;
                    }
                    s["k_bagpipe"] = param.ToString()[1];
                    sf = param.ToString()[1] == 'P' ? 0 : 2;
                    i++;

                    if (cfmt.temper == null)
                        cfmt.temper = new double[] { 11.62f, 12.55f, 1.66f, 2.37f, 3.49f, 0, 1.66f, 2.37f, 3.49f, 4.41f, 5.53f, 0, 3.49f, 4.41f, 5.53f, 6.63f, 7.35f, 4.41f, 5.53f, 6.63f, 7.35f, 8.19f, 0, 6.63f, 7.35f, 8.19f, 9.39f, 10.51f, 0, 8.19f, 9.39f, 10.51f, 11.62f, 12.55f, 0, 10.51f, 11.62f, 12.55f, 1.66f, 1.66f };
                    break;
                case 'P':
                    syntax(1, "K:P is deprecated");
                    sf = 0;
                    s["k_drum"] = true;
                    key_end = 1;
                    break;
                case 'n':
                    if (param.ToString().IndexOf("none") == 0)
                    {
                        sf = 0;
                        s["k_none"] = true;
                        i = 4;
                        break;
                    }
                    goto default;
                default:
                    s["k_map"] = new sbyte[0];
                    s["k_mode"] = 0;
                    return;
            }
        }

        if (!key_end)
        {
            switch (param.ToString().Substring(0, 3).ToLower())
            {
                default:
                    if (param.ToString()[0] != 'm' || (param.ToString()[1] != ' ' && param.ToString()[1] != '\t' && param.ToString()[1] != '\n'))
                    {
                        key_end = 1;
                        break;
                    }
                    goto case "aeo";
                case "aeo":
                case "m":
                case "min":
                    sf -= 3;
                    mode = 5;
                    break;
                case "dor":
                    sf -= 2;
                    mode = 1;
                    break;
                case "ion":
                case "maj":
                    break;
                case "loc":
                    sf -= 5;
                    mode = 6;
                    break;
                case "lyd":
                    sf += 1;
                    mode = 3;
                    break;
                case "mix":
                    sf -= 1;
                    mode = 4;
                    break;
                case "phr":
                    sf -= 4;
                    mode = 2;
                    break;
            }
            if (key_end == 0)
                param = param.ToString().Replace(new System.Text.RegularExpressions.Regex(@"\w+\s*"), "");

            if (param.ToString().IndexOf("exp ") == 0)
            {
                param = param.ToString().Replace(new System.Text.RegularExpressions.Regex(@"\w+\s*"), "");
                if (param.ToString() == "")
                    syntax(1, "No accidental after 'exp'");
                s["exp"] = true;
            }
            c = param.ToString()[0];
            if (c.ToString() == "^" || c.ToString() == "_" || c.ToString() == "=")
            {
                s["k_a_acc"] = new List<object>();
                tmp = new scanBuf();
                tmp.buffer = param.ToString();
                do
                {
                    note = parse_acc_pit(tmp);
                    if (note == null)
                        break;
                    ((List<object>)s["k_a_acc"]).Add(note);
                    c = param.ToString()[tmp.index];
                    while (c.ToString() == " ")
                        c = param.ToString()[++tmp.index];
                } while (c.ToString() == "^" || c.ToString() == "_" || c.ToString() == "=");
                param = param.ToString().Substring(tmp.index);
            }
            else if (s.ContainsKey("exp") && param.ToString().IndexOf("none") == 0)
            {
                sf = 0;
                param = param.ToString().Replace(new System.Text.RegularExpressions.Regex(@"\w+\s*"), "");
            }
        }

        if (sf < -7 || sf > 7)
        {
            syntax(1, "Key with double sharps/flats");
            if (sf > 7)
                sf -= 12;
            else
                sf += 12;
        }
        s["k_sf"] = sf;

        s["k_map"] = s.ContainsKey("k_bagpipe") && sf == 0 ? abc2svg.keys[9] : abc2svg.keys[sf + 7];
        if (s.ContainsKey("k_a_acc"))
        {
            s["k_map"] = ((sbyte[])s["k_map"]).Clone();
            i = ((List<object>)s["k_a_acc"]).Count;
            while (--i >= 0)
            {
                note = ((List<object>)s["k_a_acc"])[i];
                ((sbyte[])s["k_map"])[(int)(note["pit"]) + 19 % 7] = (sbyte)note["acc"];
            }
        }
        s["k_mode"] = mode;

        s["k_b40"] = new int[] { 1, 24, 7, 30, 13, 36, 19, 2, 25, 8, 31, 14, 37, 20, 3 }[sf + 7];

        return [s, info_split(param)];
    }

    void new_meter(object p)
    {
        object p_v;
        var s = new Dictionary<object, object>
            {
                { "type", C.METER },
                { "dur", 0 },
                { "a_meter", new List<object>() }
            };
        var meter = new Dictionary<object, object>();
        object val, v;
        int m1 = 0, m2;
        int i = 0, j;
        int wmeasure;
        bool in_parenth;

        set_ref(s);

        if (p.ToString().IndexOf("none") == 0)
        {
            i = 4;
            wmeasure = 1;
        }
        else
        {
            wmeasure = 0;
            while (i < p.ToString().Length)
            {
                if (p.ToString()[i] == '=')
                    break;
                switch (p.ToString()[i])
                {
                    case 'C':
                        meter["top"] = p.ToString()[i++];
                        if (m1 == 0)
                        {
                            m1 = 4;
                            m2 = 4;
                        }
                        break;
                    case 'c':
                    case 'o':
                        meter["top"] = p.ToString()[i++];
                        if (m1 == 0)
                        {
                            if (p.ToString()[i - 1] == 'c')
                            {
                                m1 = 2;
                                m2 = 4;
                            }
                            else
                            {
                                m1 = 3;
                                m2 = 4;
                            }
                            switch (p.ToString()[i])
                            {
                                case '|':
                                    m2 /= 2;
                                    break;
                                case '.':
                                    m1 *= 3;
                                    m2 *= 2;
                                    break;
                            }
                        }
                        break;
                    case '.':
                    case '|':
                        m1 = 0;
                        meter["top"] = p.ToString()[i++];
                        break;
                    case '(':
                        if (p.ToString()[i + 1] == '(')
                        {
                            in_parenth = true;
                            meter["top"] = p.ToString()[i++];
                            ((List<object>)s["a_meter"]).Add(meter);
                            meter = new Dictionary<object, object>();
                        }
                        j = i + 1;
                        while (j < p.ToString().Length)
                        {
                            if (p.ToString()[j] == ')' || p.ToString()[j] == '/')
                                break;
                            j++;
                        }
                        if (p.ToString()[j] == ')' && p.ToString()[j + 1] == '/')
                        {
                            i++;
                            continue;
                        }
                        if (p.ToString()[j] == '/')
                        {
                            i++;
                            if (p.ToString()[i] <= '0' || p.ToString()[i] > '9')
                            {
                                syntax(1, "Bad char '$1' in M:", p.ToString()[i]);
                                return;
                            }
                            meter["bot"] = p.ToString()[i++];
                            while (p.ToString()[i] >= '0' && p.ToString()[i] <= '9')
                                meter["bot"] += p.ToString()[i++];
                            break;
                        }
                        if (p.ToString()[i] != ' ' && p.ToString()[i] != '+')
                            break;
                        if (i >= p.ToString().Length || p.ToString()[i + 1] == '(')
                            break;
                        meter["top"] += p.ToString()[i++];
                        break;
                    case ')':
                        in_parenth = p.ToString()[i] == '(';
                        meter["top"] = p.ToString()[i++];
                        ((List<object>)s["a_meter"]).Add(meter);
                        meter = new Dictionary<object, object>();
                        continue;
                    default:
                        if (p.ToString()[i] <= '0' || p.ToString()[i] > '9')
                        {
                            syntax(1, "Bad char '$1' in M:", p.ToString()[i]);
                            return;
                        }
                        m2 = 2;
                        meter["top"] = p.ToString()[i++];
                        for (; ; )
                        {
                            while (p.ToString()[i] >= '0' && p.ToString()[i] <= '9')
                                meter["top"] += p.ToString()[i++];
                            if (p.ToString()[i] == ')')
                            {
                                if (p.ToString()[i + 1] != '/')
                                    break;
                                i++;
                            }
                            if (p.ToString()[i] == '/')
                            {
                                i++;
                                if (p.ToString()[i] <= '0' || p.ToString()[i] > '9')
                                {
                                    syntax(1, "Bad char '$1' in M:", p.ToString()[i]);
                                    return;
                                }
                                meter["bot"] = p.ToString()[i++];
                                while (p.ToString()[i] >= '0' && p.ToString()[i] <= '9')
                                    meter["bot"] += p.ToString()[i++];
                                break;
                            }
                            if (p.ToString()[i] != ' ' && p.ToString()[i] != '+')
                                break;
                            if (i >= p.ToString().Length || p.ToString()[i + 1] == '(')
                                break;
                            meter["top"] += p.ToString()[i++];
                        }
                        m1 = Convert.ToInt32(meter["top"]);
                        break;
                }
                if (!in_parenth)
                {
                    if (meter.ContainsKey("bot"))
                        m2 = Convert.ToInt32(meter["bot"]);
                    wmeasure += m1 * C.BLEN / m2;
                }
                ((List<object>)s["a_meter"]).Add(meter);
                meter = new Dictionary<object, object>();
                while (p.ToString()[i] == ' ')
                    i++;
                if (p.ToString()[i] == '+')
                {
                    meter["top"] = p.ToString()[i++];
                    ((List<object>)s["a_meter"]).Add(meter);
                    meter = new Dictionary<object, object>();
                }
            }
        }
        if (p.ToString()[i] == '=')
        {
            val = System.Text.RegularExpressions.Regex.Match(p.ToString().Substring(++i), @"^(\d+)\/(\d+)$");
            if (val == null)
            {
                syntax(1, "Bad duration '$1' in M:", p.ToString().Substring(i));
                return;
            }
            wmeasure = C.BLEN * Convert.ToInt32(val.Groups[1].Value) / Convert.ToInt32(val.Groups[2].Value);
        }
        if (wmeasure == 0)
        {
            syntax(1, errs.bad_val, "M:");
            return;
        }
        s["wmeasure"] = wmeasure;

        if (!cfmt.writefields.Contains("M"))
            ((List<object>)s["a_meter"]).Clear();

        if (parse.state != 3)
        {
            info.M = p.ToString();
            glovar.meter = s;
            if (parse.state != 0)
            {
                if (glovar.ulen == null)
                {
                    if (wmeasure <= 1 || wmeasure >= C.BLEN * 3 / 4)
                        glovar.ulen = C.BLEN / 8;
                    else
                        glovar.ulen = C.BLEN / 16;
                }
                for (v = 0; v < voice_tb.Count; v++)
                {
                    voice_tb[v].meter = s;
                    voice_tb[v].wmeasure = wmeasure;
                }
            }
        }
        else
        {
            curvoice.wmeasure = wmeasure;
            if (is_voice_sig())
                curvoice.meter = s;
            else
                sym_link(s);

            for (p_v = curvoice.voice_down; p_v != null; p_v = p_v.voice_down)
                p_v.wmeasure = wmeasure;
        }
    }


    /**************************************************************/


    public class VoiceTempo
    {
        public string type { get; set; }
        public int dur { get; set; }
    }

    public class VoiceBar
    {
        public string type { get; set; }
        public string fname { get; set; }
        public int istart { get; set; }
        public int dur { get; set; }
        public int multi { get; set; }
        public string text { get; set; }
        public int iend { get; set; }
        public bool bar_dotted { get; set; }
        public int rbstop { get; set; }
        public int bar_num { get; set; }
        public bool invis { get; set; }
        public bool norepbra { get; set; }
        public int rbstart { get; set; }
        public int st { get; set; }
        public int xsh { get; set; }
    }

    public class VoicePart
    {
        public string type { get; set; }
        public string text { get; set; }
        public int time { get; set; }
        public bool invis { get; set; }
    }

    public class VoiceRemark
    {
        public string type { get; set; }
        public string text { get; set; }
        public int dur { get; set; }
    }

    public class VoiceNote
    {
        public int pit { get; set; }
        public int shhd { get; set; }
        public int shac { get; set; }
        public int[] acc { get; set; }
    }

    public class Voice
    {
        public List<VoiceTempo> tempo_notes { get; set; }
        public string tempo_ca { get; set; }
        public int new_beat { get; set; }
        public int tempo { get; set; }
        public bool invis { get; set; }
        public string tempo_str1 { get; set; }
        public string tempo_str2 { get; set; }
        public string text { get; set; }
        public int time { get; set; }
        public bool ignore { get; set; }
        public bool second { get; set; }
        public bool norepbra { get; set; }
        public int v { get; set; }
        public int ulen { get; set; }
        public int dur_fact { get; set; }
        public List<int> acc_tie { get; set; }
        public List<int> acc_tie_rep { get; set; }
        public VoiceNote tie_s_rep { get; set; }
        public VoiceNote tie_s { get; set; }
        public List<int> acc { get; set; }
        public VoiceBar last_sym { get; set; }
        public VoiceBar lyric_restart { get; set; }
        public VoiceBar sym_restart { get; set; }
        public int st { get; set; }
        public int eoln { get; set; }
        public bool norepbra { get; set; }
        public string bar_type { get; set; }
        public int rbstop { get; set; }
        public int rbstart { get; set; }
        public bool bar_dotted { get; set; }
        public bool invis { get; set; }
        public bool norepbra { get; set; }
        public int xsh { get; set; }
        public int[] acc { get; set; }
        public int pit { get; set; }
        public int shhd { get; set; }
        public int shac { get; set; }
    }

    public class Parse
    {
        public int state { get; set; }
        public int bol { get; set; }
        public string fname { get; set; }
        public int line { get; set; }
    }

    public class ParSy
    {
        public List<Voice> voices { get; set; }
        public List<Staff> staves { get; set; }
    }

    public class Staff
    {
        public int flags { get; set; }
    }

    public class Info
    {
        public string Q { get; set; }
        public string P { get; set; }
        public string N { get; set; }
        public string R { get; set; }
    }

    public class Glovar
    {
        public Voice tempo { get; set; }
        public int new_nbar { get; set; }
        public int ulen { get; set; }
    }

    public class Line
    {
        public string buffer { get; set; }
        public int index { get; set; }

        public char char1()
        {
            return buffer[index];
        }

        public char next_char()
        {
            return buffer[++index];
        }
    }

    public class CodeTranslationAssistant
    {
        private static Regex reg_dur = new Regex(@"(\d+)\/(\d+)");
        private static string ntb = "CDEab";
        private static Info info = new Info();
        private static Glovar glovar = new Glovar();
        private static ParSy par_sy = new ParSy();
        private static Parse parse = new Parse();
        private static Voice curvoice = new Voice();
        private static int C_BLEN = 0;

        int a_gch = 0;
        int[] a_dcn = new List<int>();
        int multicol = 0;
        Dictionary<string, int> maps = new Dictionary<string, int>();
        int[] qplet_tb = new int[] { 0, 1 };
        string ntb = "CDEab";
        var reg_dur = new Regex(@"aawwd");

        List<string> char_tb = new List<string> { null, " ", "\n", null };

        void new_tempo(string text)
        {
            int i, c, d, nd;
            string txt = text;
            VoiceItem s = new VoiceItem
            {
                type = "C.TEMPO",
                dur = 0
            };

            int[] get_nd(string p)
            {
                int n, d;
                var nd = reg_dur.Match(p);

                if (nd.Success)
                {
                    d = int.Parse(nd.Groups[2].Value);
                    if (d != 0 && !d.Equals(d & (d - 1)))
                    {
                        n = int.Parse(nd.Groups[1].Value);
                        if (!double.IsNaN(n))
                            return new int[] { C_BLEN * n / d };
                    }
                }
                syntax(1, "Invalid note duration $1", c);
                return null;
            }

            set_ref(s);

            if (cfmt.writefields.IndexOf('Q') < 0)
                s.invis = true;

            if (text[0] == '"')
            {
                c = text.IndexOf('"');
                if (c == -1)
                {
                    syntax(1, "Unterminated string in Q:");
                    return;
                }
                s.tempo_str1 = text.Substring(1, c - 1);
                text = text.Substring(c + 1).TrimStart();
            }

            if (text[^1] == '"')
            {
                i = text.IndexOf('"');
                s.tempo_str2 = text.Substring(i + 1, text.Length - i - 2);
                text = text.Substring(0, i).TrimEnd();
            }

            i = text.IndexOf('=');
            if (i > 0)
            {
                var d = text.Substring(0, i).Split(' ');
                text = text.Substring(i + 1).TrimStart();
                while (true)
                {
                    c = d[0];
                    if (c == null)
                        break;
                    nd = get_nd(c);
                    if (nd == null)
                        return;
                    if (s.tempo_notes == null)
                        s.tempo_notes = new List<int>();
                    s.tempo_notes.Add(nd);
                }

                if (text.Substring(0, 4) == "ca. ")
                {
                    s.tempo_ca = "ca. ";
                    text = text.Substring(4);
                }
                i = text.IndexOf('/');
                if (i > 0)
                {
                    nd = get_nd(text);
                    if (nd == null)
                        return;
                    s.new_beat = nd;
                }
                else
                {
                    s.tempo = int.Parse(text);
                    if (s.tempo == 0 || double.IsNaN(s.tempo))
                    {
                        syntax(1, "Bad tempo value");
                        return;
                    }
                }
            }

            if (parse.state < 2 || (curvoice.time == 0 && glovar.tempo == 0))
            {
                info.Q = txt;
                glovar.tempo = s;
                return;
            }

            if (glovar.tempo == 0)
                syntax(0, "No previous tempo");

            if (new_ctrl(s))
                sym_link(s);
        }

        void do_info(string info_type, string text)
        {
            Voice s;
            int d1, d2, a, vid, tim, v, p_v;

            if (curvoice != null && curvoice.ignore)
            {
                switch (info_type)
                {
                    default:
                        return;
                    case "P":
                    case "Q":
                    case "V":
                        break;
                }
            }
            reg a;
            switch (info_type)
            {
                case "I":
                    self.do_pscom(text);
                    break;
                case "L":
                    a = text.Match("/ ^1\/ (\d +)(= (\d +)\/ (\d +))?$/");
                    if (a != null)
                    {
                        d1 = int.Parse(a[1]);
                        if (d1 == 0 || (d1 & (d1 - 1)) != 0)
                            break;
                        d1 = C_BLEN / d1;
                        if (a[2] != null)
                        {
                            d2 = int.Parse(a[4]);
                            d2 = d2 != 0 ? int.Parse(a[3]) / d2 * C_BLEN : 0;
                        }
                        else
                        {
                            d2 = d1;
                        }
                    }
                    else if (text == "auto")
                    {
                        d1 = d2 = -1;
                    }
                    if (d2 == 0)
                    {
                        syntax(1, "Bad L: value");
                        break;
                    }
                    if (parse.state <= 1)
                    {
                        glovar.ulen = d1;
                    }
                    else
                    {
                        curvoice.ulen = d1;
                        curvoice.dur_fact = d2 / d1;
                    }
                    break;
                case "M":
                    new_meter(text);
                    break;
                case "U":
                    set_user(text);
                    break;
                case "P":
                    if (parse.state == 0)
                        break;
                    if (parse.state == 1)
                    {
                        info.P = text;
                        break;
                    }

                    s = new VoicePart
                    {
                        type = "C.PART",
                        text = text,
                        time = tim
                    };
                    if (!new_ctrl(s))
                        break;
                    sym_link(s);
                    if (cfmt.writefields.IndexOf(info_type) < 0)
                        s.invis = true;
                    break;
                case "Q":
                    if (parse.state == 0)
                        break;
                    new_tempo(text);
                    break;
                case "V":
                    get_voice(text);
                    if (parse.state == 3)
                        curvoice.ignore = !par_sy.voices[curvoice.v];
                    break;
                case "K":
                    if (parse.state == 0)
                        break;
                    get_key(text);
                    break;
                case "N":
                case "R":
                    if (info[info_type] == null)
                        info[info_type] = text;
                    else
                        info[info_type] += '\n' + text;
                    break;
                case "r":
                    if (!user.keep_remark || parse.state != 3)
                        break;
                    s = new VoiceRemark
                    {
                        type = "C.REMARK",
                        text = text,
                        dur = 0
                    };
                    sym_link(s);
                    break;
                default:
                    syntax(0, "'$1:' line ignored", info_type);
                    break;
            }
        }

        void adjust_dur(Voice s)
        {
            VoiceBar s2;
            int time, auto_time, i, fac;

            s2 = curvoice.last_sym;
            if (s2 == null)
                return;

            if (s2.type == "C.MREST" || s2.type == "C.BAR")
                return;
            while (s2.type != "C.BAR" && s2.prev != null)
                s2 = s2.prev;
            time = s2.time;
            auto_time = curvoice.time - time;
            fac = curvoice.wmeasure / auto_time;

            if (fac == 1)
                return;

            for (; s2 != null; s2 = s2.next)
            {
                s2.time = time;
                if (s2.dur == 0 || s2.grace)
                    continue;
                s2.dur *= fac;
                s2.dur_orig *= fac;
                time += s2.dur;
                if (s2.type != "C.NOTE" && s2.type != "C.REST")
                    continue;
                for (i = 0; i <= s2.nhd; i++)
                    s2.notes[i].dur *= fac;
            }
            curvoice.time = s.time = time;
        }

        void new_bar()
        {
            VoiceBar s2;
            int c, bar_type;
            var line = parse.line;
            var s = new VoiceBar
            {
                type = "C.BAR",
                fname = parse.fname,
                istart = parse.bol + line.index,
                dur = 0,
                multi = 0
            };

            if (vover != null && vover.bar != null)
                get_vover('|');
            if (glovar.new_nbar != 0)
            {
                s.bar_num = glovar.new_nbar;
                glovar.new_nbar = 0;
            }
            bar_type = line.char();
            while (true)
            {
                c = line.next_char();
                switch (c)
                {
                    case '|':
                    case '[':
                    case ']':
                    case ':':
                        bar_type += c;
                        continue;
                }
                break;
            }
            if (bar_type[0] == ':')
            {
                if (bar_type == ":")
                {
                    bar_type = "|";
                    s.bar_dotted = true;
                }
                else
                {
                    s.rbstop = 2;
                }
            }

            if (a_gch != 0)
                csan_add(s);
            if (a_dcn.Count != 0)
                deco_cnv(s);

            if (bar_type.Slice(-1) == "[" && !Regex.IsMatch(c.ToString(), "[0-9\" ]"))
            {
                bar_type = bar_type.Slice(0, -1);
                line.index--;
                c = '[';
            }

            if (c > '0' && c <= '9')
            {
                s.text = c.ToString();
                while (true)
                {
                    c = line.next_char();
                    if (!Regex.IsMatch(c.ToString(), "[0-9,.-]"))
                        break;
                    s.text += c.ToString();
                }
            }
            else if (c == '"' && bar_type.Slice(-1) == "[")
            {
                s.text = "";
                while (true)
                {
                    c = line.next_char();
                    if (c == null)
                    {
                        syntax(1, "No end of repeat string");
                        return;
                    }
                    if (c == '"')
                    {
                        line.index++;
                        break;
                    }
                    s.text += c.ToString();
                }
            }

            if (bar_type.Slice(-1) == "]" && bar_type.Length != 1)
            {
                bar_type = bar_type.Slice(1);
            }
            else
            {
                s.invis = true;
            }

            s.iend = parse.bol + line.index;

            if (s.text != null && bar_type.Slice(-1) == "[" && bar_type != "[")
            {
                bar_type = bar_type.Slice(0, -1);
            }

            if (bar_type.Slice(-1) == ":" && !s.invis)
            {
                s.rbstart = 2;
            }

            if (s.text != null)
            {
                s.rbstart = 2;
                if (s.text[0] == '1')
                {
                    curvoice.tie_s_rep = curvoice.tie_s;
                    if (curvoice.acc_tie != null)
                        curvoice.acc_tie_rep = curvoice.acc_tie;
                    else if (curvoice.acc_tie_rep != null)
                        curvoice.acc_tie_rep = null;
                }
                else
                {
                    curvoice.tie_s = curvoice.tie_s_rep;
                    if (curvoice.acc_tie_rep != null)
                        curvoice.acc_tie = curvoice.acc_tie_rep;
                }
                if (curvoice.norepbra && !curvoice.second)
                    s.norepbra = true;
            }

            if (curvoice.ulen < 0)
                adjust_dur(s);

            if ((bar_type == "[" || bar_type == "|:") && !curvoice.eoln && a_gch == 0 && !s.invis)
            {
                s2 = curvoice.last_sym;
                if (s2 != null && s2.type == "C.BAR")
                {
                    if ((bar_type == "[" && s2.text == null) || s.norepbra)
                    {
                        if (s.text != null)
                        {
                            s2.text = s.text;
                            if (curvoice.st != 0 && !s.norepbra && (par_sy.staves[curvoice.st - 1].flags & STOP_BAR) == 0)
                                s2.xsh = 4;
                        }
                        if (s.norepbra)
                            s2.norepbra = true;
                        if (s.rbstart != 0)
                            s2.rbstart = s.rbstart;
                        if (s.rbstop != 0)
                            s2.rbstop = s.rbstop;
                        return;
                    }
                    if (bar_type == "|:")
                    {
                        switch (s2.bar_type)
                        {
                            case ":|":
                                s2.bar_type = "::";
                                s2.rbstop = 2;
                                return;
                        }
                    }
                }
            }

            if (bar_type == "||" && cfmt["abc-version"] >= "2.2")
            {
                if (bar_type == "[|" || bar_type == "|]")
                    s.rbstop = 2;
            }

            s.bar_type = bar_type;
            if (curvoice.lyric_restart == null)
                curvoice.lyric_restart = s;
            if (curvoice.sym_restart == null)
                curvoice.sym_restart = s;

            sym_link(s);

            s.st = curvoice.st;

            if (s.text != null && s.st > 0 && !s.norepbra && (par_sy.staves[s.st - 1].flags & STOP_BAR) == 0 && bar_type != "[")
                s.xsh = 4;
            if (!s.bar_dotted && !s.invis)
                curvoice.acc = new List<int>();
        }

        List<int[]> parse_staves(int p)
        {
            int v, vid;
            Dictionary<int, bool> vids = new Dictionary<int, bool>();
            List<int[]> a_vf = new List<int[]>();
            bool err = false;
            int flags = 0;
            int brace = 0;
            int bracket = 0;
            int parenth = 0;
            int flags_st = 0;
            int e;
            List<int> a = new List<int>();

            MatchCollection matches = Regex.Matches(p, @"[^[\]|{}()*+\s]+|[^\s]");
            if (matches.Count == 0)
            {
                syntax(1, errs.bad_val, "%%score");
                return null;
            }
            foreach (Match match in matches)
            {
                e = match.Value;
                if (e == null)
                    break;
                switch (e)
                {
                    case '[':
                        if (parenth || brace + bracket >= 2)
                        {
                            syntax(1, errs.misplaced, '[');
                            err = true;
                            break;
                        }
                        flags |= brace + bracket == 0 ? OPEN_BRACKET : OPEN_BRACKET2;
                        bracket++;
                        flags_st <<= 8;
                        flags_st |= OPEN_BRACKET;
                        break;
                    case '{':
                        if (parenth || brace || bracket >= 2)
                        {
                            syntax(1, errs.misplaced, '{');
                            err = true;
                            break;
                        }
                        flags |= !bracket ? OPEN_BRACE : OPEN_BRACE2;
                        brace++;
                        flags_st <<= 8;
                        flags_st |= OPEN_BRACE;
                        break;
                    case '(':
                        if (parenth)
                        {
                            syntax(1, errs.misplaced, '(');
                            err = true;
                            break;
                        }
                        flags |= OPEN_PARENTH;
                        parenth++;
                        flags_st <<= 8;
                        flags_st |= OPEN_PARENTH;
                        break;
                    case '*':
                        if (brace && !parenth && !(flags & (OPEN_BRACE | OPEN_BRACE2)))
                            flags |= FL_VOICE;
                        break;
                    case '+':
                        flags |= MASTER_VOICE;
                        break;
                    case ']':
                    case '}':
                    case ')':
                        syntax(1, "Bad voice ID in %%score");
                        err = true;
                        break;
                    default:    // get / create the voice in the voice table
                        vid = e;
                        while (true)
                        {
                            e = a.shift();
                            if (e == null)
                                break;
                            switch (e)
                            {
                                case ']':
                                    if (!(flags_st & OPEN_BRACKET))
                                    {
                                        syntax(1, errs.misplaced, ']');
                                        err = true;
                                        break;
                                    }
                                    bracket--;
                                    flags |= brace + bracket == 0 ?
                                        CLOSE_BRACKET :
                                        CLOSE_BRACKET2;
                                    flags_st >>= 8;
                                    continue;
                                case '}':
                                    if (!(flags_st & OPEN_BRACE))
                                    {
                                        syntax(1, errs.misplaced, '}');
                                        err = true;
                                        break;
                                    }
                                    brace--;
                                    flags |= !bracket ?
                                        CLOSE_BRACE :
                                        CLOSE_BRACE2;
                                    flags &= ~FL_VOICE;
                                    flags_st >>= 8;
                                    continue;
                                case ')':
                                    if (!(flags_st & OPEN_PARENTH))
                                    {
                                        syntax(1, errs.misplaced, ')');
                                        err = true;
                                        break;
                                    }
                                    parenth--;
                                    flags |= CLOSE_PARENTH;
                                    flags_st >>= 8;
                                    continue;
                                case '|':
                                    flags |= STOP_BAR;
                                    continue;
                            }
                            break;
                        }
                        if (vids[vid])
                        {
                            syntax(1, "Double voice in %%score");
                            err = true;
                        }
                        else
                        {
                            vids[vid] = true;
                            a_vf.Add(new int[] { vid, flags });
                        }
                        flags = 0;
                        if (e == null)
                            break;
                        a.unshift(e);
                        break;
                }
            }
            if (flags_st != 0)
            {
                syntax(1, "'}', ')' or ']' missing in %%score");
                err = true;
            }
            if (err || a_vf.Count == 0)
                return null;
            return a_vf;
        }

        List<string> info_split(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new List<string>();
            var a = Regex.Matches(text, @"[^\s""=]+=?|""[^""]*""")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToList();
            if (a.Count == 0)
            {
                //fixme: bad error text
                syntax(1, "Unterminated string");
                return new List<string>();
            }
            return a;
        }

        int[] parse_dur(Line line)
        {
            var res = reg_dur.Match(line.buffer, line.index);
            if (!res.Success)
                return new int[] { 1, 1 };
            var num = int.Parse(res.Groups[1].Value);
            var den = int.Parse(res.Groups[3].Value);
            if (res.Groups[3].Value == null)
                den *= 1 << res.Groups[2].Value.Length;
            line.index = res.Index + res.Length;
            return new int[] { num, den };
        }

        int[] parse_acc_pit(Line line)
        {
            Voice note;
            int acc, pit, d, nd;
            var c = line.char();

            switch (c)
            {
                case '^':
                    c = line.next_char();
                    if (c == '^')
                    {
                        acc = 2;
                        c = line.next_char();
                    }
                    else
                    {
                        acc = 1;
                    }
                    break;
                case '=':
                    acc = 3;
                    c = line.next_char();
                    break;
                case '_':
                    c = line.next_char();
                    if (c == '_')
                    {
                        acc = -2;
                        c = line.next_char();
                    }
                    else
                    {
                        acc = -1;
                    }
                    break;
            }

            if (acc == 1 || acc == -1)
            {
                if (Regex.IsMatch(c.ToString(), "[1-9/]"))
                {
                    var nd = parse_dur(line);
                    if (acc < 0)
                        nd[0] = -nd[0];
                    if (cfmt.nedo != 0 && nd[1] == 1)
                    {
                        nd[0] *= 12;
                        nd[1] *= cfmt.nedo;
                    }
                    acc = nd;
                    c = line.char();
                }
            }

            pit = ntb.IndexOf(c) + 16;
            c = line.next_char();
            if (pit < 16)
            {
                syntax(1, "'$1' is not a note", line.buffer[line.index - 1]);
                return null;
            }

            while (c == "'")
            {
                pit += 7;
                c = line.next_char();
            }
            while (c == ",")
            {
                pit -= 7;
                c = line.next_char();
            }
            note = {
            pit: pit,
        shhd: 0,
        shac: 0
                }
            if (acc)
                note.acc = acc;
            return new int[] { note, acc, pit };
        }
        void set_map(Note note, int acc)
        {
            var pit = note.pit;
            var nn = not2abc(pit, acc);
            var map = maps[curvoice.map];    // never null

            if (!map.ContainsKey(nn))
            {
                nn = 'o' + nn.Replace(/[',]+/, '');    // ' octave
                    if (!map.ContainsKey(nn))
                {
                    nn = 'k' + ntb[(pit + 75 -
                        curvoice.ckey.k_sf * 11) % 7];
                    if (!map.ContainsKey(nn))
                    {
                        nn = 'all';        // 'all'
                        if (!map.ContainsKey(nn))
                            return;
                    }
                }
            }
            note.map = map = map[nn];
            if (map[1] != null)                // if print map
            {
                note.pit = pit = map[1].pit;
                note.acc = map[1].acc;
                if (map[1].notrp != null)
                {
                    note.notrp = 1; //true    // no transpose
                    note.noplay = 1; //true    // no play
                }
            }
            if (map[2] != null)                // if color
                note.color = map[2];
            var n3 = map[3];
            if (n3 != null)                    // if play map
                note.midi = pit2mid(n3.pit + 19, n3.acc);
        }


        Note parse_basic_note(string line, int ulen)
        {
            Note note;
            var nd = parse_dur(line);

            if (line[0] == '0')
            {        // compatibility
                parse.stemless = true;
                line = line.Substring(1);
            }
            note = parse_acc_pit(line);
            if (note == null)
                return null;

            note.dur = ulen * nd[0] / nd[1];
            return note;
        }

        int parse_vpos()
        {
            var line = parse.line;
            var ty = 0;

            if (a_dcn.Count > 0 && a_dcn[a_dcn.Count - 1] == "dot")
            {
                ty = C.SL_DOTTED;
                a_dcn.RemoveAt(a_dcn.Count - 1);
            }
            switch (line[0])
            {
                case '\'':
                    line = line.Substring(1);
                    return ty + C.SL_ABOVE;
                case ',':
                    line = line.Substring(1);
                    return ty + C.SL_BELOW;
                case '?':                // slur between staves (like ~)
                    line = line.Substring(1);
                    return ty + C.SL_CENTER;
            }
            return ty + C.SL_AUTO;
        }







    }
}

interface Voice
{
    int[] acc_tie;
    int[] acc;
}

static void Main(string[] args)
{
    int[] a_gch;
    List<int> a_dcn = new List<int>();
    int multicol;
    Dictionary<string, int> maps = new Dictionary<string, int>();
    int[] qplet_tb = new int[] { 0, 1 };
    string ntb = "CDEab";
    Regex reg_dur = new Regex("aawwd");
    string nil = "0";
    string[] char_tb = new string[] { null, " ", "\n", null };



    Voice curvoice;



    int[] new_note(int grace, int sls)
    {
        int note, s, in_chord, c, dcn, type, tie_s, acc_tie;
        int i, n, s2, nd, res, num, dur, apit, div, ty;
        int dpit = 0;
        List<int> sl1 = new List<int>();
        int line = parse.line;
        List<int> a_dcn_sav = a_dcn;        // save parsed decoration names

        a_dcn = new List<int>();
        parse.stemless = false;
        s = new
        {
            type = C.NOTE,
            fname = parse.fname,
            stem = 0,
            multi = 0,
            nhd = 0,
            xmx = 0
        };
        s.istart = parse.bol + line.index;

        if (curvoice.color)
            s.color = curvoice.color;

        if (grace)
        {
            s.grace = true;
        }
        else
        {
            if (curvoice.tie_s)
            {    // if tie from previous note / grace note
                tie_s = curvoice.tie_s;
                curvoice.tie_s = null;
            }
            if (a_gch)
                csan_add(s);
            if (parse.repeat_n)
            {
                s.repeat_n = parse.repeat_n;
                s.repeat_k = parse.repeat_k;
                parse.repeat_n = 0;
            }
        }
        c = line.char1();
        switch (c)
        {
            case 'X':
                s.invis = true;
            case 'Z':
                s.type = C.MREST;
                c = line.next_char();
                s.nmes = (c > '0' && c <= '9') ? line.get_int() : 1;
                if (curvoice.wmeasure == 1)
                {
                    error(1, s, "multi-measure rest, but no measure!");
                    return null;
                }
                s.dur = curvoice.wmeasure * s.nmes;

                // ignore if in second voice
                if (curvoice.second)
                {
                    delete curvoice.eoln;    // ignore the end of line
                    curvoice.time += s.dur;
                    return null;
                }

                // convert 'Z'/'Z1' to a whole measure rest
                if (s.nmes == 1)
                {
                    s.type = C.REST;
                    s.dur_orig = s.dur;
                    s.fmr = 1;        // full measure rest
                    s.notes = new int[] {
                                pit = 18,
                                dur = s.dur
                            };
                }
                else
                {
                    glovar.mrest_p = true;
                    if (par_sy.voices.length == 1)
                    {
                        s.tacet = curvoice.tacet;
                        delete s.invis;    // show the 'H' when 'Xn'
                    }
                }
                break;
            case 'y':
                s.type = C.SPACE;
                s.invis = true;
                s.dur = 0;
                c = line.next_char();
                if (c >= '0' && c <= '9')
                    s.width = line.get_int();
                else
                    s.width = 10;
                if (tie_s)
                {
                    curvoice.tie_s = tie_s;
                    tie_s = null;
                }
                break;
            case 'x':
                s.invis = true;
            case 'z':
                s.type = C.REST;
                line.index++;
                nd = parse_dur(line);
                s.dur_orig = ((curvoice.ulen < 0) ?
                    C.BLEN :
                    curvoice.ulen) * nd[0] / nd[1];
                s.dur = s.dur_orig * curvoice.dur_fact;
                if (s.dur == curvoice.wmeasure)
                    s.fmr = 1;        // full measure rest
                s.notes = new int[] {
                            pit = 18,
                            dur = s.dur_orig
                        };
                break;
            case '[':            // chord
                in_chord = true;
                c = line.next_char();
            // fall thru
            default:            // accidental, chord, note
                if (curvoice.acc_tie)
                {
                    acc_tie = curvoice.acc_tie;
                    curvoice.acc_tie = null;
                }
                s.notes = new List<int>();

                // loop on the chord
                while (true)
                {

                    // when in chord, get the slurs and decorations
                    if (in_chord)
                    {
                        while (true)
                        {
                            if (!c)
                                break;
                            i = c.charCodeAt(0);
                            if (i >= 128)
                            {
                                syntax(1, errs.not_ascii);
                                return null;
                            }
                            type = char_tb[i];
                            switch (type[0])
                            {
                                case '(':
                                    sl1.push(parse_vpos());
                                    c = line.char();
                                    continue;
                                case '!':
                                    if (type.length > 1)
                                        a_dcn.push(type.slice(1, -1));
                                    else
                                        get_deco();    // line -> a_dcn
                                    c = line.next_char();
                                    continue;
                            }
                            break;
                        }
                    }
                    note = parse_basic_note(line,
                        s.grace ? C.BLEN / 4 :
                            curvoice.ulen < 0 ?
                                C.BLEN :
                                curvoice.ulen);
                    if (!note)
                        return null;

                    if (curvoice.octave)
                        note.pit += curvoice.octave * 7;

                    // get the real accidental
                    apit = note.pit + 19        // pitch from C-1
                            i = note.acc;
                    if (!i)
                    {
                        if (cfmt["propagate-accidentals"][0] == 'p')
                            i = curvoice.acc[apit % 7];
                        else
                            i = curvoice.acc[apit];
                        if (!i)
                            i = curvoice.ckey.k_map[apit % 7] || 0;
                    }

                    if (i)
                    {
                        if (cfmt["propagate-accidentals"][0] == 'p')
                            curvoice.acc[apit % 7] = i;
                        else if (cfmt["propagate-accidentals"][0] != 'n')
                            curvoice.acc[apit] = i;
                    }

                    if (acc_tie && acc_tie[apit])
                        i = acc_tie[apit];    // tied note

                    // map
                    if (curvoice.map
                        && maps[curvoice.map])
                        set_map(note, i);

                    // set the MIDI pitch
                    if (!note.midi)        // if not map play
                        note.midi = pit2mid(apit, i);

                    // transpose
                    if (curvoice.tr_sco
                        && !note.notrp)
                    {
                        i = nt_trans(note, i);
                        if (i == -3)
                        {        // if triple sharp/flat
                            error(1, s, "triple sharp/flat");
                            i = note.acc > 0 ? 1 : -1;
                            note.pit += i;
                            note.acc = i;
                        }
                        dpit = note.pit + 19 - apit;
                    }
                    if (curvoice.tr_snd)
                        note.midi += curvoice.tr_snd;

                    //fixme: does not work if transposition
                    if (i)
                    {
                        switch (cfmt["writeout-accidentals"][1])
                        {
                            case 'd':            // added
                                s2 = curvoice.ckey;
                                if (!s2.k_a_acc)
                                    break;
                                for (n = 0; n < s2.k_a_acc.length; n++)
                                {
                                    if ((s2.k_a_acc[n].pit - note.pit)
                                        % 7 == 0)
                                    {
                                        note.acc = i;
                                        break;
                                    }
                                }
                                break;
                            case 'l':            // all
                                note.acc = i;
                                break;
                        }
                    }

                    // starting slurs
                    if (sl1.length)
                    {
                        while (true)
                        {
                            i = sl1.shift();
                            if (!i)
                                break;
                            var tu1 = (
                                ty: i,
                                    ss: s,
                                    nts: note    // starting note
                                );
                            curvoice.sls.push(tu1);
                        }
                    }
                    if (a_dcn.length)
                    {
                        s.time = curvoice.time    // (needed for !tie)!
                                dh_cnv(s, note);
                    }
                    s.notes.push(note);
                    if (!in_chord)
                        break;

                    // in chord: get the ending slurs and the ties
                    c = line.char();
                    while (true)
                    {
                        switch (c)
                        {
                            case ')':
                                slur_add(s, note);
                                c = line.next_char();
                                continue;
                            case '-':
                                note.tie_ty = parse_vpos();
                                note.s = s;
                                curvoice.tie_s = s;
                                s.ti1 = true;
                                if (curvoice.acc[apit]
                                    || (acc_tie
                                        && acc_tie[apit]))
                                {
                                    if (!curvoice.acc_tie)
                                        curvoice.acc_tie = [];
                                    i = curvoice.acc[apit];
                                    if (acc_tie && acc_tie[apit])
                                        i = acc_tie[apit];
                                    curvoice.acc_tie[apit] = i;
                                }
                                c = line.char();
                                continue;
                            case '.':
                                c = line.next_char();
                                switch (c)
                                {
                                    case '-':
                                    case '(':
                                        a_dcn.push("dot");
                                        continue;
                                }
                                syntax(1, "Misplaced dot");
                                break;
                        }
                        break;
                    }
                    if (c == ']')
                    {
                        line.index++;

                        // adjust the chord duration
                        nd = parse_dur(line);
                        s.nhd = s.notes.length - 1;
                        for (i = 0; i <= s.nhd; i++)
                        {
                            note = s.notes[i];
                            note.dur = note.dur * nd[0] / nd[1];
                        }
                        break;
                    }
                }

                // handle the starting slurs
                if (sls.length)
                {
                    while (true)
                    {
                        i = sls.shift();
                        if (!i)
                            break;
                        var tu2 = (
                            ty = i,
                                ss = s
                                // no starting note
                                )
                            curvoice.sls.push(tu2);
                        if (grace)
                            curvoice.sls[curvoice.sls.length - 1].grace =
                                grace;
                    }
                }

                // the duration of the chord is the duration of the 1st note
                s.dur_orig = s.notes[0].dur;
                s.dur = s.notes[0].dur * curvoice.dur_fact;
                break;
        }
        if (s.grace && s.type != C.NOTE)
        {
            syntax(1, errs.bad_grace);
            return null;
        }

        if (s.notes)                // if note or rest
        {
            if (!grace)
            {
                switch (curvoice.pos.stm & 0x07)
                {
                    case C.SL_ABOVE: s.stem = 1; break;
                    case C.SL_BELOW: s.stem = -1; break;
                    case C.SL_HIDDEN: s.stemless = true; break;
                }

                // adjust the symbol duration
                num = curvoice.brk_rhythm;
                if (num)
                {
                    curvoice.brk_rhythm = 0;
                    s2 = curvoice.last_note;
                    if (num > 0)
                    {
                        n = num * 2 - 1;
                        s.dur = s.dur * n / num;
                        s.dur_orig = s.dur_orig * n / num;
                        for (i = 0; i <= s.nhd; i++)
                            s.notes[i].dur =
                                s.notes[i].dur * n / num;
                        s2.dur /= num;
                        s2.dur_orig /= num;
                        for (i = 0; i <= s2.nhd; i++)
                            s2.notes[i].dur /= num;
                    }
                    else
                    {
                        num = -num;
                        n = num * 2 - 1;
                        s.dur /= num;
                        s.dur_orig /= num;
                        for (i = 0; i <= s.nhd; i++)
                            s.notes[i].dur /= num;
                        s2.dur = s2.dur * n / num;
                        s2.dur_orig = s2.dur_orig * n / num;
                        for (i = 0; i <= s2.nhd; i++)
                            s2.notes[i].dur =
                                s2.notes[i].dur * n / num;
                    }
                    curvoice.time = s2.time + s2.dur;

                    // adjust the time of the grace notes, bars...
                    for (s2 = s2.next; s2; s2 = s2.next)
                        s2.time = curvoice.time;
                }
            }
            else
            {        /* grace note - adjust its duration */
                div = curvoice.ckey.k_bagpipe ? 8 : 4;
                for (i = 0; i <= s.nhd; i++)
                    s.notes[i].dur /= div;
                s.dur /= div;
                s.dur_orig /= div;
                if (grace.stem)
                    s.stem = grace.stem;
            }

            curvoice.last_note = s;

            // get the possible ties and end of slurs
            c = line.char1();
            while (true)
            {
                switch (c)
                {
                    case '.':
                        if (line.buffer[line.index + 1] != '-')
                            break;
                        a_dcn.push("dot");
                        line.index++;
                    // fall thru
                    case '-':
                        ty = parse_vpos();
                        for (i = 0; i <= s.nhd; i++)
                        {
                            s.notes[i].tie_ty = ty;
                            s.notes[i].s = s;
                        }
                        curvoice.tie_s = grace || s;
                        curvoice.tie_s.ti1 = true;
                        for (i = 0; i <= s.nhd; i++)
                        {
                            note = s.notes[i];
                            apit = note.pit + 19    // pitch from C-1
                                - dpit        // (if transposition)
                                    if (curvoice.acc[apit]
                                        || (acc_tie
                                            && acc_tie[apit]))
                            {
                                if (!curvoice.acc_tie)
                                    curvoice.acc_tie = [];
                                n = curvoice.acc[apit];
                                if (acc_tie && acc_tie[apit])
                                    n = acc_tie[apit];
                                curvoice.acc_tie[apit] = n;
                            }
                        }
                        c = line.char();
                        continue;
                }
                break;
            }

            // handle the ties ending on this chord/note
            if (tie_s)        // if tie from previous note / grace note
                do_ties(s, tie_s);
        }

        sym_link(s);

        if (!grace)
        {
            if (!curvoice.lyric_restart)
                curvoice.lyric_restart = s;
            if (!curvoice.sym_restart)
                curvoice.sym_restart = s;
        }

        if (a_dcn_sav.length)
        {
            a_dcn = a_dcn_sav;
            deco_cnv(s, s.prev);
        }
        if (grace && s.ottava)
            grace.ottava = s.ottava;
        if (parse.stemless)
            s.stemless = true;
        s.iend = parse.bol + line.index;
        return s;
    }
}



using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CodeTranslationAssistant
{
    class Program
    {
        static void Main(string[] args)
        {
            var a_gch = 0;
            var a_dcn = new List<string>();
            int multicol;
            var maps = new Dictionary<string, object>();

            var qplet_tb = new sbyte[] { 0, 1 };
            var ntb = "CDEab";
            var reg_dur = new Regex("aawwd");
            var nil = "0";
            var char_tb = new string[] { nil, " ", "\n", nil };

            interface Voice
        {
            int[] acc_tie { get; set; }
            int[] acc { get; set; }
        }

        Voice curvoice;







        void parse_music_line()
        {
            Note grace, last_note_sav;
            List<string> a_dcn_sav;
            bool no_eol;
            string s, tps;
            var tp = new List<object>();
            var tpn = -1;
            var sls = new List<int>();
            var line = parse.line;

            // check if a transposing macro matches a source sequence
            // if yes return the base note
            int check_mac(string m)
            {
                int i, j, b;

                for (i = 1, j = line.index + 1; i < m.Length; i++, j++)
                {
                    if (m[i] == line.buffer[j])
                        continue;
                    if (m[i] != 'n')        // search the base note
                        return null;
                    b = ntb.IndexOf(line.buffer[j]);
                    if (b < 0)
                        return null;
                    while (line.buffer[j + 1] == '\'')
                    {
                        b += 7;
                        j++;
                    }
                    while (line.buffer[j + 1] == ',')
                    {
                        b -= 7;
                        j++;
                    }
                }
                line.index = j;
                return b;
            } // check_mac()

            // convert a note as a number into a note as a ABC string
            string n2n(int n)
            {
                var c = "";

                while (n < 0)
                {
                    n += 7;
                    c += ",";
                }
                while (n >= 14)
                {
                    n -= 7;
                    c += "'";
                }
                return ntb[n] + c;
            } // n2n()

            // expand a transposing macro
            string expand(string m, int b)
            {
                if (b == null)        // if static macro
                    return m;
                string c;
                int i;
                var r = "";                // result
                var n = m.Length;

                for (i = 0; i < n; i++)
                {
                    c = m[i].ToString();
                    if (c.CompareTo("h") >= 0 && c.CompareTo("z") <= 0)
                    {
                        r += n2n(b + c[0] - 'n');
                    }
                    else
                    {
                        r += c;
                    }
                }
                return r;
            } // expand()

            // parse a macro
            void parse_mac(string k, string m, int b)
            {
                int te, ti;
                Voice curv;
                string s;
                var line_sav = line;
                var istart_sav = parse.istart;

                parse.line = line = new scanBuf();
                parse.istart += line_sav.index;

                // if the macro is not displayed
                if (cfmt.writefields.IndexOf('m') < 0)
                {

                    // build the display sequence from the original sequence
                    line.buffer = k.Replace('n', n2n(b));
                    s = curvoice.last_sym;
                    ti = curvoice.time;        // start time
                    parse_seq(true);
                    if (s == null)
                        s = curvoice.sym;
                    for (s = s.next; s != null; s = s.next)
                        s.noplay = true;
                    te = curvoice.time;        // end time
                    curv = curvoice;

                    // and put the macro sequence in a play specific voice
                    curvoice = clone_voice(curv.id + '-p');
                    if (!par_sy.voices.ContainsKey(curvoice.v))
                    {
                        curvoice.second = true;
                        par_sy.voices[curvoice.v] = new
                        {
                            st = curv.st,
                            second = true,
                            range = curvoice.v
                        };
                    }
                    curvoice.time = ti;
                    s = curvoice.last_sym;
                    parse.line = line = new scanBuf();
                    parse.istart += line_sav.index;
                    line.buffer = expand(m, b);
                    parse_seq(true);
                    if (curvoice.time != te)
                        syntax(1, "Bad length of the macro sequence");
                    if (s == null)
                        s = curvoice.sym;
                    for (; s != null; s = s.next)
                        s.invis = s.play = true;
                    curvoice = curv;
                }
                else
                {
                    line.buffer = expand(m, b);
                    parse_seq(true);
                }

                parse.line = line = line_sav;
                parse.istart = istart_sav;
            } // parse_mac()

            // parse a music sequence
            void parse_seq(bool in_mac = false)
            {
                string c;
                int idx, type, k, s, dcn, i, n;
                string text;
                Note note;

                while (true)
                {
                    c = line[0];
                    if (string.IsNullOrEmpty(c))
                        break;

                    // check if start of a macro
                    if (!in_mac && maci.ContainsKey(c))
                    {
                        n = null;
                        foreach (var kvp in mac)
                        {
                            var k = kvp.Key;
                            var m = kvp.Value;
                            if (k[0] != c || !m.StartsWith(k))
                                continue;
                            if (k.IndexOf('n') < 0)
                            {
                                if (!line.buffer.Substring(line.index).StartsWith(k))
                                    continue;
                                line.index += k.Length;
                            }
                            else
                            {
                                n = check_mac(k);
                                if (n == null)
                                    continue;
                            }
                            parse_mac(k, m, n);
                            n = 1;
                            break;
                        }
                        if (n != null)
                            continue;
                    }

                    idx = c[0];
                    if (idx >= 128)
                    {
                        syntax(1, errs.not_ascii);
                        line = line.Substring(1);
                        break;
                    }

                    type = char_tb[idx][0];
                    switch (type)
                    {
                        case ' ':            // beam break
                            s = curvoice.last_note;
                            if (s != null)
                            {
                                s.beam_end = true;
                                if (grace != null)
                                    grace.gr_shift = true;
                            }
                            break;
                        case '\n':            // line break
                            if (cfmt.barsperstaff)
                                break;
                            curvoice.eoln = true;
                            break;
                        case '&':            // voice overlay
                            if (grace != null)
                            {
                                syntax(1, errs.bad_grace);
                                break;
                            }
                            c = line[1];
                            if (c == ')')
                            {
                                get_vover(c);    // full overlay stop
                                break;
                            }
                            get_vover('&');
                            continue;
                        case '(':            // slur start - tuplet - vover
                            c = line[1];
                            if (c >= '0' && c <= '9')    // tuplet
                            {
                                if (grace != null)
                                {
                                    syntax(1, errs.bad_grace);
                                    break;
                                }
                                var pplet = line.get_int();
                                var qplet = qplet_tb[pplet];
                                var rplet = pplet;

                                c = line[0];
                                if (c == ':')
                                {
                                    c = line[1];
                                    if (c >= '0' && c <= '9')
                                    {
                                        qplet = line.get_int();
                                        c = line[0];
                                    }
                                    if (c == ':')
                                    {
                                        c = line[1];
                                        if (c >= '0' && c <= '9')
                                        {
                                            rplet = line.get_int();
                                            c = line[0];
                                        }
                                        else
                                        {
                                            syntax(1, "Invalid 'r' in tuplet");
                                            continue;
                                        }
                                    }
                                }
                                if (qplet == 0 || qplet == null)
                                    qplet = (curvoice.wmeasure % 9) == 0 ?
                                        3 : 2;
                                if (tpn < 0)
                                    tpn = tp.Count;    // new tuplet
                                tp.Add(new
                                {
                                    p = pplet,
                                    q = qplet,
                                    r = rplet,
                                    ro = rplet,
                                    f = curvoice.tup || cfmt.tuplets
                                });
                                continue;
                            }
                            if (c == '&')
                            {        // voice overlay start
                                if (grace != null)
                                {
                                    syntax(1, errs.bad_grace);
                                    break;
                                }
                                get_vover('(');
                                break;
                            }
                            line.index--;
                            sls.Add(parse_vpos());
                            continue;
                        case ')':            // slur end
                            s = curvoice.last_sym;
                            if (s != null)
                            {
                                switch (s.type)
                                {
                                    case C.SPACE:
                                        if (s.notes == null)
                                        {
                                            s.notes = new List<object>();
                                            s.notes[0] = new { };
                                        }
                                        break;
                                    case C.NOTE:
                                    case C.REST:
                                        break;
                                    case C.GRACE:

                                        // stop the slur on the last grace note
                                        for (s = s.extra; s.next != null; s = s.next)
                                            ;
                                        break;
                                    default:
                                        s = null;
                                        break;
                                }
                            }
                            if (s == null)
                            {
                                syntax(1, errs.bad_char, c);
                                break;
                            }
                            slur_add(s);
                            break;
                        case '!':            // start of decoration
                            if (char_tb[idx].Length > 1)    // decoration letter
                                a_dcn.Add(char_tb[idx].Substring(1, -1));
                            else
                                get_deco();    // (line -> a_dcn)
                            break;
                        case '"':
                            if (grace != null)
                            {
                                syntax(1, errs.bad_grace);
                                break;
                            }
                            parse_gchord(char_tb[idx]);
                            break;
                        case '[':
                            if (char_tb[idx].Length > 1)
                            {    // U: [I:xxx]
                                self.do_pscom(char_tb[idx].Substring(3, -1));
                                break;
                            }
                            var c_next = line.buffer[line.index + 1];

                            if ("|[]: \"".IndexOf(c_next) >= 0
                                || (c_next >= '1' && c_next <= '9'))
                            {
                                if (grace != null)
                                {
                                    syntax(1, errs.bar_grace);
                                    break;
                                }
                                new_bar();
                                continue;
                            }
                            if (line.buffer[line.index + 2] == ':')
                            {
                                if (grace != null)
                                {
                                    syntax(1, errs.bad_grace);
                                    break;
                                }
                                i = line.buffer.IndexOf(']', line.index + 1);
                                if (i < 0)
                                {
                                    syntax(1, "Lack of ']'");
                                    break;
                                }
                                text = line.buffer.Substring(line.index + 3, i).Trim();

                                parse.istart = parse.bol + line.index;
                                parse.iend = parse.bol + ++i;
                                line.index = 0;
                                do_info(c_next, text);
                                line.index = i;
                                continue;
                            }
                        // fall thru ('[' is start of chord)
                        case 'n':                // note/rest
                            s = self.new_note(grace, sls);
                            if (s == null)
                                continue;

                            // handle the tuplets
                            if (grace != null || s.notes == null)
                                continue;

                            if (tpn >= 0)
                            {        // new tuplet
                                s.tp = tp.GetRange(tpn, tp.Count - tpn);
                                tpn = -1;
                                if (tps != null)
                                    s.tp[0].s = tps;    // if nested
                                tps = s;
                            }
                            else if (tps == null)
                            {
                                continue;    // no tuplet active
                            }

                            k = tp[tp.Count - 1];
                            if (--k.r > 0)
                                continue;    // not end of tuplet yet

                            while (true)
                            {
                                tp_adj(tps, k.q / k.p);
                                i = k.ro;    // number of notes of this tuplet
                                if (k.s != null)
                                    tps = k.s;  // start of upper tuplet

                                tp.RemoveAt(tp.Count - 1);    // previous level
                                if (tp.Count == 0)
                                {
                                    tps = null;    // done
                                    break;
                                }
                                k = tp[tp.Count - 1];
                                k.r -= i;
                                if (k.r > 0)
                                    break;
                            }
                            continue;
                        case '<':                /* '<' and '>' */
                            if (curvoice.last_note == null)
                            {
                                syntax(1, "No note before '<'");
                                break;
                            }
                            if (grace != null)
                            {
                                syntax(1, "Cannot have a broken rhythm in grace notes");
                                break;
                            }
                            n = c == '<' ? 1 : -1;
                            while (c == '<' || c == '>')
                            {
                                n *= 2;
                                c = line[1];
                            }
                            curvoice.brk_rhythm = n;
                            continue;
                        case 'i':                // ignore
                            break;
                        case '{':
                            if (grace != null)
                            {
                                syntax(1, "'{' in grace note");
                                break;
                            }
                            last_note_sav = curvoice.last_note;
                            curvoice.last_note = null;
                            a_dcn_sav = a_dcn;
                            a_dcn = new List<string>();
                            grace = new Note
                            {
                                type = C.GRACE,
                                fname = parse.fname,
                                istart = parse.bol + line.index,
                                dur = 0,
                                multi = 0
                            };
                            if (curvoice.color != null)
                                grace.color = curvoice.color;
                            switch (curvoice.pos.gst & 0x07)
                            {
                                case C.SL_ABOVE: grace.stem = 1; break;
                                case C.SL_BELOW: grace.stem = -1; break;
                                case C.SL_HIDDEN: grace.stem = 2; break    /* opposite */;
                            }
                            sym_link(grace);
                            c = line[1];
                            if (c == '/')
                            {
                                grace.sappo = true;    // acciaccatura
                                break;
                            }
                            continue;
                        case '|':
                            if (grace != null)
                            {
                                syntax(1, errs.bar_grace);
                                break;
                            }
                            new_bar();
                            continue;
                        case '}':
                            if (curvoice.ignore)
                            {
                                grace = null;
                                break;
                            }
                            s = curvoice.last_note;
                            if (grace == null || s == null)
                            {
                                syntax(1, errs.bad_char, c);
                                break;
                            }
                            if (a_dcn.Count > 0)
                                syntax(1, "Decoration ignored");
                            grace.extra = grace.next;
                            grace.extra.prev = null;
                            grace.next = null;
                            curvoice.last_sym = grace;
                            grace = null;
                            if (s.prev == null            // if one grace note
                                && curvoice.ckey.k_bagpipe == 0)
                            {
                                for (i = 0; i <= s.nhd; i++)
                                    s.notes[i].dur *= 2;
                                s.dur *= 2;
                                s.dur_orig *= 2;
                            }
                            curvoice.last_note = last_note_sav;
                            a_dcn = a_dcn_sav;
                            break;
                        case "\\":
                            if (line.buffer[line.index + 1] == null)
                            {
                                no_eol = true;
                                break;
                            }
                        // fall thru
                        default:
                            syntax(1, errs.bad_char, c);
                            break;
                    }
                    line = line.Substring(1);
                }
            } // parse_seq()

            if (parse.state != 3)        // if not in tune body
                return;

            if (parse.tp != null)
            {
                tp = parse.tp;
                tpn = parse.tpn;
                tps = parse.tps;
                parse.tp = null;
            }

            parse_seq();

            if (tp.Count > 0)
            {
                parse.tp = tp;
                parse.tps = tps;
                parse.tpn = tpn;
            }
            if (sls.Count > 0)
                syntax(1, "Start of slur without note");
            if (grace != null)
            {
                syntax(1, "No end of grace note sequence");
                curvoice.last_sym = grace.prev;
                curvoice.last_note = last_note_sav;
                if (grace.prev != null)
                    grace.prev.next = null;
            }
            if (!no_eol && !cfmt.barsperstaff && vover == null
                && char_tb['\n'.charCodeAt(0)] == '\n')
                curvoice.eoln = true;
            if (curvoice.eoln && cfmt.breakoneoln && curvoice.last_note != null)
                curvoice.last_note.beam_end = true;
        }
    }
}
}



using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class Program
{
    static List<object> a_dcn = new List<object>();
    static bool multicol;
    static Dictionary<string, object> maps = new Dictionary<string, object>();
    static sbyte[] qplet_tb = new sbyte[] { 0, 1 };
    static string ntb = "CDEab";
    static Regex reg_dur = new Regex("aawwd");
    static string nil = "0";
    static string[] char_tb = new string[] { nil, " ", "\n", nil };

    interface IVoice
    {
        int[] acc_tie { get; set; }
        int[] acc { get; set; }
    }

    class Voice : IVoice
    {
        public int[] acc_tie { get; set; }
        public int[] acc { get; set; }
        public List<Slur> sls { get; set; } = new List<Slur>();
        public int time { get; set; }
        public int snd_oct { get; set; }
    }

    class Slur
    {
        public object ss { get; set; }
        public object se { get; set; }
        public object nte { get; set; }
        public bool grace { get; set; }
        public string ty { get; set; }
        public string loc { get; set; }
    }

    static Voice curvoice = new Voice();

    static void SlurAdd(object s, object nt = null)
    {
        int i;
        object s2;
        Slur sl;

        for (i = curvoice.sls.Count - 1; i >= 0; i--)
        {
            sl = curvoice.sls[i];

            if (sl.ss == s)
                continue;

            curvoice.sls.RemoveAt(i);
            sl.se = s;
            if (nt != null)
                sl.nte = nt;
            s2 = sl.ss;
            if (s2.GetType().GetProperty("sls") == null)
                s2.GetType().GetProperty("sls").SetValue(s2, new List<Slur>());
            ((List<Slur>)s2.GetType().GetProperty("sls").GetValue(s2)).Add(sl);

            if (sl.grace)
                sl.grace.GetType().GetProperty("sl1").SetValue(sl.grace, true);
            return;
        }

        for (s2 = s.GetType().GetProperty("prev").GetValue(s); s2 != null; s2 = s2.GetType().GetProperty("prev").GetValue(s2))
        {
            if (s2.GetType().GetProperty("type").GetValue(s2).Equals("BAR") &&
                s2.GetType().GetProperty("bar_type").GetValue(s2).ToString()[0] == ':' &&
                s2.GetType().GetProperty("text").GetValue(s2) != null)
            {
                if (s2.GetType().GetProperty("sls") == null)
                    s2.GetType().GetProperty("sls").SetValue(s2, new List<Slur>());
                ((List<Slur>)s2.GetType().GetProperty("sls").GetValue(s2)).Add(new Slur
                {
                    ty = "SL_AUTO",
                    ss = s2,
                    se = s
                });
                if (nt != null)
                    ((List<Slur>)s2.GetType().GetProperty("sls").GetValue(s2)).Last().nte = nt;
                return;
            }
        }

        if (s.GetType().GetProperty("sls") == null)
            s.GetType().GetProperty("sls").SetValue(s, new List<Slur>());
        ((List<Slur>)s.GetType().GetProperty("sls").GetValue(s)).Add(new Slur
        {
            ty = "SL_AUTO",
            se = s,
            loc = "i"
        });
        if (nt != null)
            ((List<Slur>)s.GetType().GetProperty("sls").GetValue(s)).Last().nte = nt;
    }

    static double Pit2Mid(int pit, object acc)
    {
        int[] pArray = { 0, 2, 4, 5, 7, 9, 11 };
        int p = pArray[pit % 7];
        int o = (pit / 7) * 12;
        double s = 0;
        int b40;

        if (curvoice.snd_oct != 0)
            o += curvoice.snd_oct;
        if (acc.Equals(3))
            acc = 0;
        if (acc != null)
        {
            if (acc is int[])
            {
                int[] accArray = (int[])acc;
                s = (double)accArray[0] / accArray[1];
                if (accArray[1] == 100)
                    return p + o + s;
            }
            else
            {
                s = Convert.ToDouble(acc);
            }
        }
        else
        {
            if (cfmt.temper != null)
                return cfmt.temper[abc2svg.p_b40[pit % 7]] + o;
            return p + o;
        }
        if (!cfmt.nedo)
        {
            if (cfmt.temper == null)
            {
                p += o + s;
                return p;
            }
        }
        else
        {
            if (!(acc is int[]))
            {
                b40 = abc2svg.p_b40[pit % 7] + Convert.ToInt32(acc);
                return cfmt.temper[b40] + o;
            }

            int[] accArray = (int[])acc;
            if (accArray[1] == cfmt.nedo)
            {
                b40 = abc2svg.p_b40[pit % 7];
                return cfmt.temper[b40] + o + s;
            }
        }

        double p0 = cfmt.temper[abc2svg.p_b40[pit % 7]];
        double p1;
        if (s > 0)
        {
            p1 = cfmt.temper[(abc2svg.p_b40[pit % 7] + 1) % 40];
            if (p1 < p0)
                p1 += 12;
        }
        else
        {
            p1 = cfmt.temper[(abc2svg.p_b40[pit % 7] + 39) % 40];
            if (p1 > p0)
                p1 -= 12;
            s = -s;
        }
        return p0 + o + (p1 - p0) * s;
    }

    static void DoTies(object s, object tie_s)
    {
        int i, m, nt = 0;
        object not1, not2, mid, g;
        bool se = (tie_s.GetType().GetProperty("time").GetValue(tie_s) + tie_s.GetType().GetProperty("dur").GetValue(tie_s)).Equals(curvoice.time);

        for (m = 0; m <= (int)s.GetType().GetProperty("nhd").GetValue(s); m++)
        {
            not2 = ((List<object>)s.GetType().GetProperty("notes").GetValue(s))[m];
            mid = not2.GetType().GetProperty("midi").GetValue(not2);
            if (!tie_s.GetType().GetProperty("type").GetValue(tie_s).Equals("GRACE"))
            {
                for (i = 0; i <= (int)tie_s.GetType().GetProperty("nhd").GetValue(tie_s); i++)
                {
                    not1 = ((List<object>)tie_s.GetType().GetProperty("notes").GetValue(tie_s))[i];
                    if (not1.GetType().GetProperty("tie_ty").GetValue(not1) == null)
                        continue;
                    if (not1.GetType().GetProperty("midi").GetValue(not1).Equals(mid) &&
                        (!se || not1.GetType().GetProperty("tie_e").GetValue(not1) == null))
                    {
                        not2.GetType().GetProperty("tie_s").SetValue(not2, not1);
                        not2.GetType().GetProperty("s").SetValue(not2, s);
                        if (se)
                        {
                            not1.GetType().GetProperty("tie_e").SetValue(not1, not2);
                            not1.GetType().GetProperty("s").SetValue(not1, tie_s);
                        }
                        nt++;
                        break;
                    }
                }
            }
            else
            {
                for (g = tie_s.GetType().GetProperty("extra").GetValue(tie_s); g != null; g = g.GetType().GetProperty("next").GetValue(g))
                {
                    not1 = ((List<object>)g.GetType().GetProperty("notes").GetValue(g))[0];
                    if (not1.GetType().GetProperty("tie_ty").GetValue(not1) == null)
                        continue;
                    if (not1.GetType().GetProperty("midi").GetValue(not1).Equals(mid))
                    {
                        g.GetType().GetProperty("ti1").SetValue(g, true);
                        not2.GetType().GetProperty("tie_s").SetValue(not2, not1);
                        not2.GetType().GetProperty("s").SetValue(not2, s);
                        not1.GetType().GetProperty("tie_e").SetValue(not1, not2);
                        not1.GetType().GetProperty("s").SetValue(not1, g);
                        nt++;
                        break;
                    }
                }
            }
        }

        if (nt == 0)
            throw new Exception("Bad tie");
        else
            s.GetType().GetProperty("ti2").SetValue(s, true);
    }

    static void TpAdj(object s, double fact)
    {
        double d;
        int tim = (int)s.GetType().GetProperty("time").GetValue(s);
        double to = curvoice.time - tim;
        double tt = to * fact;

        curvoice.time = tim + tt;
        while (true)
        {
            s.GetType().GetProperty("in_tuplet").SetValue(s, true);
            if (s.GetType().GetProperty("grace").GetValue(s) == null)
            {
                s.GetType().GetProperty("time").SetValue(s, tim);
                if (s.GetType().GetProperty("dur").GetValue(s) != null)
                {
                    d = Math.Round((double)s.GetType().GetProperty("dur").GetValue(s) * tt / to);
                    to -= (double)s.GetType().GetProperty("dur").GetValue(s);
                    s.GetType().GetProperty("dur").SetValue(s, d);
                    tt -= (double)s.GetType().GetProperty("dur").GetValue(s);
                    tim += (int)s.GetType().GetProperty("dur").GetValue(s);
                }
            }
            if (s.GetType().GetProperty("next").GetValue(s) == null)
            {
                if (s.GetType().GetProperty("tpe").GetValue(s) != null)
                    s.GetType().GetProperty("tpe").SetValue(s, (int)s.GetType().GetProperty("tpe").GetValue(s) + 1);
                else
                    s.GetType().GetProperty("tpe").SetValue(s, 1);
                break;
            }
            s = s.GetType().GetProperty("next").GetValue(s);
        }
    }

    static void GetDeco()
    {
        char c;
        string line = parse.line;
        int i = line.IndexOf('!'); // in case no deco end
        string dcn = "";

        while (true)
        {
            c = line[i];
            if (c == '\0')
            {
                line = line.Substring(0, i);
                throw new Exception("No end of decoration");
            }
            if (c == '!')
                break;
            dcn += c;
        }
        a_dcn.Add(dcn);
    }
}




















}