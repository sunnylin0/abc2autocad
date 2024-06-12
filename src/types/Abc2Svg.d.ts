declare interface Abc2Svg {
    Abc?: {};
    sym_name?;
    font_tb?: any[];	// fonts - index = font.fid
    font_st?: {};	// font style => font_tb index for incomplete user fonts

    mhooks?: [];
    sheet?:any;
    ft_re?: RegExp;
    rat?(n, d)
    pitcmp?(n1, n2)
    loadjs?(fn, onsuccess, onerror)
    printErr?
    C?: {
        BLEN?: 1536;

        // symbol types
        BAR?: 0;
        CLEF?: 1;
        CUSTOS?: 2;
        SM?: 3;		// sequence marker (transient)
        GRACE?: 4;  //這是裝飾音類別
        KEY?: 5;    //這是調號 K:C ,K:Ｄb
        METER?: 6;  //這是拍號 M:4/4 3/4
        MREST?: 7;
        NOTE?: 8;   //這是音符 
        PART?: 9;
        REST?: 10;
        SPACE?: 11;
        STAVES?: 12;
        STBRK?: 13;
        TEMPO?: 14;
        BLOCK?: 16;
        REMARK?: 17;

        // note heads
        FULL?: 0;
        EMPTY?: 1;
        OVAL?: 2;
        OVALBARS?: 3;
        SQUARE?: 4;

        // position types
        SL_ABOVE?: 0x01;		// position (3 bits)
        SL_BELOW?: 0x02;
        SL_AUTO?: 0x03;
        SL_HIDDEN?: 0x04;
        SL_DOTTED?: 0x08;	// modifiers
        SL_ALI_MSK?: 0x70;	// align
        SL_ALIGN?: 0x10;
        SL_CENTER?: 0x20;
        SL_CLOSE?: 0x40
    };

    keys?: Int8Array[];
    p_b40?: Int8Array;
    b40_p?: Int8Array;
    b40_a?: Int8Array;
    b40_m?: Int8Array;
    b40Mc?: Int8Array;
    b40sf?: Int8Array,
    isb40?: Int8Array,

    pab40?(p, a);
    b40p?(b);
    b40a?(b);
    b40m?(b);
    ch_alias?: {
        "maj": "",
        "min": "m",
        "-": "m",
        "°": "dim",
        "+": "aug",
        "+5": "aug",
        "maj7": "M7",
        "Δ7": "M7",
        "Δ": "M7",
        "min7": "m7",
        "-7": "m7",
        "ø7": "m7b5",
        "°7": "dim7",
        "min+7": "m+7",
        "aug7": "+7",
        "7+5": "+7",
        "7#5": "+7",
        "sus": "sus4",
        "7sus": "7sus4"
    } // ch_alias

    // font weight
    ft_w?: {
        thin: 100,
        extralight: 200,
        light: 300,
        regular: 400,
        medium: 500,
        semi: 600,
        demi: 600,
        semibold: 600,
        demibold: 600,
        bold: 700,
        extrabold: 800,
        ultrabold: 800,
        black: 900,
        heavy: 900
    }
}
declare interface ClefDefinition {
    type?: number;
    clef_line?: number
    clef_type?: string;
    clef_name?: string;
    clef_auto?: boolean;
    clef_none?: boolean;
    clef_oct_transp?: boolean;
    clef_octave?: number
    v?: number;
    p_v?: PageVoiceTune;
    time?: number;
    dur?: number;
    invis?: boolean

}
declare const elem: {
    "type", "fname", "stem", "multi", "nhd", "xmx", "istart", "iend", "notes",
    "dur_orig", "dur", "next", "prev", "v", "p_v", "st", "time", "fmt", "pos", "ts_prev",
    "ts_next", "head", "dots", "nflags", "beam_st", "beam_end", "mid", "ys", "ymn", "y",
    "ymx", "wr", "wl", "seqst", "shrink", "space", "x", "xs"
}
declare interface Stv_G {
    scale?: number;
    dy?: number;
    st?: number;
    v?: number;
    g?: number;
    color?: string;
    started?: boolean;
}
declare interface Parse {
    ctx?
    prefix?: string
    state?: number
    ottava?: number[]
    line?: scanBuf
    fname?: string
    istart?: number;
    iend?: number;
    file?: string
    ckey?
    eol?
    ctrl?: {}
    repeat_n?
    repeat_k?
    scores?: any[]
    tp?
    tps?
    tpn?
    bol?
    stemless?: boolean;
    ufmt?: boolean
    tune_v_opts?: {}
    voice_opts?: {}
    tune_opts?: {}
    select?: RegExp
}
declare interface SaveGlobalDefinitions {
    cfmt?
    char_tb?
    glovar?
    info?
    maci?
    mac?
    maps?
}
declare interface GlobalVar {
    meter?: voiceMeter;
    tempo?: voiceTempo
    mrest_p?: boolean
    ulen?: number
    new_nbar?: number;
    ottava?
}

declare class scanBuf {
    buffer?: string
    index?: number
    char()
    next_char()
    get_int()
}


declare interface PageVoiceTune {
    v?: number;
    id?: string;
    time?: number;
    pos?;
    scale?: number;
    ulen?: number;
    dur_fact?: number;
    meter?: voiceMeter;
    wmeasure?: number;
    staffnonote?: number;
    clef?: voiceClef
    acc?: [];
    sls?: [];
    hy_st?: number;
    cst?: number;
    st?: number;
    ckey?: voiceKey;
    init?: boolean;
    jianpu?: boolean;
    nm?: string;
    snm?: string;
    ignore?: boolean;
    last_note?: voiceItem;
    sym?: voiceItem;
    last_sym?: voiceItem;
    lyric_restart?: voiceItem;
    sym_restart?: voiceItem;
    tie_s_rep?;
    have_ly?: boolean;
    lyric_start?: voiceItem;
    lyric_line?: number;
    lyric_cont?: voiceItem;
    tie_s?;
    key?: {
        type?: number;
        dur?: number;
        fname?: string;
        istart?: number;
        iend?: number;
        k_sf?: number;
        k_map?: Int8Array;
        k_mode?: number;
        k_b40?: number;
        wl?: number;
        wr?: number;
    };
    osym?: voiceItem;
    s_next?: voiceItem;
    s_prev?: voiceItem;
    second?;
    bar_start?;

}

declare type Font = {
    box?: boolean;
    pad?: number;
    name?: string;
    weight?: string;
    size?: number;
    used?: boolean;
    fid?: number;
    swfac?: number;
    fname?: string;
    style?: string;
    src?: string;
};
declare interface FormationInfo {
    'abc-version'?: string;
    annotationfont?: Font;
    aligncomposer?: number;
    beamslope?: number;
    bardef?: {
        '['?: string;
        '[]'?: string;
        '|:'?: string;
        '|::'?: string;
        '|:::'?: string;
        ':|'?: string;
        '::|'?: string;
        ':::|'?: string;
        '::'?: string;
    };
    breaklimit?: number;
    breakoneoln?: boolean;
    cancelkey?: boolean;
    composerfont?: Font;
    composerspace?: number;
    decoerr?: boolean;
    dynalign?: boolean;
    footerfont?: Font;
    fullsvg?: string;
    gchordfont?: Font;
    gracespace?: Float32Array;
    graceslurs?: boolean;
    headerfont?: Font;
    historyfont?: Font;
    hyphencont?: boolean;
    indent?: number;
    infofont?: Font;
    infoname?: string;
    infospace?: number;
    keywarn?: boolean;
    leftmargin?: number;
    lineskipfac?: number;
    linewarn?: boolean;
    maxshrink?: number;
    maxstaffsep?: number;
    maxsysstaffsep?: number;
    measrepnb?: number;
    measurefont?: Font;
    measurenb?: number;
    musicfont?: Font;
    musicspace?: number;
    partsfont?: Font;
    parskipfac?: number;
    partsspace?: number;
    pagewidth?: number;
    'propagate-accidentals'?: string;
    printmargin?: number;
    rightmargin?: number;
    rbmax?: number;
    rbmin?: number;
    repeatfont?: Font;
    scale?: number;
    slurheight?: number;
    spatab?: Float32Array;
    staffsep?: number;
    stemheight?: number;
    stretchlast?: number;
    stretchstaff?: boolean;
    subtitlefont?: Font;
    subtitlespace?: number;
    sysstaffsep?: number;
    systnames?: number;
    tempofont?: Font;
    textfont?: Font;
    textspace?: number;
    tieheight?: number;
    titlefont?: Font;
    titlespace?: number;
    titletrim?: number | boolean;
    topspace?: number;
    tuplets?: number[];
    tupletfont?: Font;
    vocalfont?: Font;
    vocalspace?: number;
    voicefont?: Font;
    writefields?: string;
    wordsfont?: Font;
    wordsspace?: number;
    'writeout-accidentals'?: string;
    pageheight?: number;
    dateformat?: string;
    barsperstaff?: number;
    squarebreve?: boolean;
    altchord?: boolean;
    quiet?
    infoline?
    splittune?
    fgcolor?
    bgcolor?
    graceword?
    contbarnb?
    textoption?
    transp?: number
    tp?
    tps?
    voice_opts?
    tune_v_opts?
    titlecaps?
    titleleft?
    titleformat?
    sound?
    nedo?
    temper?: Float32Array
    checkbars?
    custos?
    ufmt?: boolean
    select?: {}
    tune_opts?
    soundfont?
    oldmrest?
    singleline?
    pedline?
}

declare interface Information {
    // information fields
    X?: string
    T?: string
    W?: string
    K?: string
    C?: string
    V?: {}
    M?: string
    Q?: string
    P?: string
    R?: string
    A?: string
    O?: string
}

declare interface DrawImage {
    width?: number
    lm?: number
    rm?: number
    lw?: number
    chg?: boolean

}

declare interface noteItem {
    pit?: number;
    shhd?: number;
    shac?: number;
    dur?: number;
    midi?: number;
    jn?: number;
    jo?: number;
}
declare interface decorationItem { // 裝飾
    name?: string;
    func?: number;
    glyph?: string;
    h?: number;
    hd?: number;
    wl?: number;
    wr?: number;
    str?: string;
    dd_st?: decorationItem;
    dd_en?: decorationItem;
}
declare interface lyricsItem { // 歌詞
    t?: string;
    font?: Font;
    istart?: number;
    iend?: number;
    ln?: number;
    shift?: number;
}
declare interface gchordItem { // 合弦
    type?: string;
    text?: string;
    x?: number;
    pos?: number;
    otext?: string;
    istart?: number;
    iend?: number;
    font?: Font;
}

declare interface voiceBase {
    type?: number;
    v?: number;
    dur?: number;
    time?: number;
    fmt?: FormationInfo,
    p_v?: PageVoiceTune,
    x?: number;
    y?: number;
    wr?: number;
    wl?: number;
    st?: number;
    a_dd?: decorationItem[];
    a_ly?: lyricsItem[];
    a_gch?: gchordItem[]
    next?: voiceItem
    prev?: voiceItem
    ts_prev?: voiceItem
    ts_next?: voiceItem
    err?: boolean
}


declare interface voiceBar extends voiceBase {
    type?: 0 | number;
    bar_type?: string;
    bar_num?: number;
    bar_mrep?: number;
    bar_dotted?: boolean;
    text?: string;
    fname?: string;
    istart?: number;
    multi?: number;
    iend?: number;
    invis?: boolean;
    pos?: {},
    seqst?: true,
    notes?: noteItem;
    nhd?: number;
    mid?: number;
    ymx?: number;
    ymn?: number;
    shrink?: number;
    space?: number;
    rbstop?: number;
    rbstart?: number;
    xsh?: number;
    norepbra?: boolean
}
declare interface voiceClef extends voiceBase {
    type?: 1;
    clef_line?: number;
    clef_type?: string;
    clef_auto?: boolean;
    fname?: string;
    istart?: number;
    iend?: number;
    ymx?: number;
    ymn: number;
    seqst?: boolean,
    shrink?: number;
    space?: number;
}
declare interface voiceKey extends voiceBase {
    type?: 5 | number;
    istart?: number;
    iend?: number;
    k_sf?: number;
    k_map?: Int8Array
    k_mode?: number;
    k_b40?: number;
    seqst?: true,
    k_old_sf?: number;
    k_y_clef?: number;
    ymx?: number;
    ymn?: number;
    shrink?: number;
    space?: number;

    k_bagpipe?
    k_drum?
    k_none?: boolean;
    exp?: boolean;
    k_a_acc?: noteItem[]
}
declare interface voiceMeter extends voiceBase {
    type?: 6 | number;
    a_meter?: { top?: string, bot?: string }[],
    fname?: string;
    istart?: number;
    iend?: number;
    wmeasure?: number;
    x_meter?: number[];
    seqst?: true,
    ymx?: number;
    ymn?: number;
    shrink?: number;
    space?: number;
    pos?: {},
    notes?: noteItem[];
    nhd?: number;
    mid?: number;
}

declare interface voiceNote extends voiceBase {
    type?: 8;
    fname?: string;
    stem?: number;
    multi?: number;
    nhd?: number;
    istart?: number;        //這是記錄這物件在文字檔的位置 開始
    iend?: number;          //這是記錄這物件在文字檔的位置 結束
    notes?: noteItem[];
    dur_orig?: number;
    pos?: {},
    head?: number;
    dots?: number;      //幾個 負點音符
    nflags?: number;    //這是拍子 有幾條拍線
    extra?: voiceItem[];  //裝飾音
    acc?: number;       // 升降記號
    beam_end?: boolean,
    beam_st?: boolean;
    mid?: number;
    xmx?: number;
    ys?: number;
    ymn?: number;
    ymx?: number;
    in_tuplet?: boolean;
    tpe?: number;
    seqst?: boolean;
    stemless?: boolean; // 音符是否無莖的
    shrink?: number;
    space?: number;
    sls?: {
        ty: number
        ss: voiceItem
        se:voiceItem
    }[]
    slurStart: number[];
    slurEnd: number[];
    soln?: boolean;     //換行 一行結束
    nl?: boolean;       //換行 新的一行開始
    xs?: number;
    repeat_n?: number;
    repeat_k?: number;
    play?: boolean;
    invis?: boolean;
}

declare interface voiceRest extends voiceBase {
    type?: 10;
    fname?: string;
    stem?: number;
    multi?: number;
    nhd?: number;
    xmx?: number;
    istart?: number;
    dur_orig?: number;
    fmr?: number;
    notes?: noteItem[]
    pos?: {},
    iend?: number;
    beam_end?: boolean;
    head?: number;
    dots?: number;
    nflags?: number;
    stemless?: boolean;
    beam_st?: boolean;
    mid?: number;
    ymx?: number;
    ymn?: number;
    invis?: boolean;
    nmes?: number;
    repeat_n?: number;
    repeat_k?: number;
    rep_nb?: number;
    seqst?: boolean;
    shrink?: number;
    space?: number;
    soln?: boolean;
}

declare interface voiceStaves extends voiceBase {
    type?: 12 | number;
    fname?: string;
    st?: number;
    sy?: {
        voices?: {
            st?: number;
            range?: number;
            sep?: number;
        }[];
        staves?: {
            stafflines?: string;
            staffscale?: number;
            staffnonote?: number;
            maxsep?: number;
        }[];
        top_voice?: number;
        nstaff?: number;
    };
    seqst?: boolean;
    parts?: string,
    notes?: noteItem[];
    nhd?: number;
    ymx?: number;
    ymn?: number;
    shrink?: number;
    space?: number;
}


declare interface voiceTempo extends voiceBase {
    type?: 14 | number;
    fname?: string;
    istart?: number;
    iend?: number;
    tempo_str1?: string;
    tempo_notes?: number[];
    tempo?: number;
    notes?: noteItem[];
    nhd?: number;
    mid?: number;
    ymx?: number;
    ymn?: number;
    tempo_str?: string;
    tempo_str2?: string;
    tempo_ca?: string;
    new_beat?: number;
    tempo_wh?: [number, number]
    seqst?: boolean;
    shrink?: number;
    space?: number;
    invis?: boolean;
}

declare interface voiceBlock extends voiceBase {
    type?: 16;
    subtype?: string;
    fname?: string;
    istart?: number;
    iend?: number;
    pos?: {},
    invis?: number;
    play?: number;
    chn?: number;
    notes?: noteItem[];
    nhd?: number;
    mid?: number;
    ymx?: number;
    ymn?: number;
    instr?: number;
}

declare type voiceItem = voiceBar | voiceClef | voiceKey | voiceMeter | voiceNote | voiceRest | voiceTempo | voiceBlock
type asdf =`{
    mtr : " "
    brace: ""
    lphr: ""
    mphr: ""
    sphr: ""
    short: ""
    tick: ""
    rdots: ""
    dsgn: ""
    dcap: ""
    sgno: ""
    coda: ""
    tclef: ""
    cclef: ""
    bclef: ""
    pclef: ""
    spclef: ""
    stclef: ""
    scclef: ""
    sbclef: ""
    oct: ""
    oct2: ""
    mtr0: ""
    mtr1: ""
    mtr2: ""
    mtr3: ""
    mtr4: ""
    mtr5: ""
    mtr6: ""
    mtr7: ""
    mtr8: ""
    mtr9: ""
    mtrC: ""
    mtr+: ""
    mtr(: ""
    mtr): ""
    HDD: ""
    breve: ""
    HD: ""
    Hd: ""
    hd: ""
    ghd: ""
    pshhd: ""
    pfthd: ""
    x: ""
    circle-x: ""
    srep: ""
    dot+: ""
    diamond: ""
    triangle: ""
    dot: ""
    flu1: ""
    fld1: ""
    flu2: ""
    fld2: ""
    flu3: ""
    fld3: ""
    flu4: ""
    fld4: ""
    flu5: ""
    fld5: ""
    acc-1: ""
    cacc-1: ""
    sacc-1: ""
    acc3: ""
    cacc3: ""
    sacc3: ""
    acc1: ""
    cacc1: ""
    sacc1: ""
    acc2: ""
    acc-2: ""
    acc-1_2: ""
    acc-3_2: ""
    acc1_2: ""
    acc3_2: ""
    accent: ""
    stc: ""
    emb: ""
    wedge: ""
    marcato: ""
    hld: ""
    brth: ""
    caes: ""
    r00: ""
    r0: ""
    r1: ""
    r2: ""
    r4: ""
    r8: ""
    r16: ""
    r32: ""
    r64: ""
    r128: ""
    mrep: ""
    mrep2: ""
    p: ""
    f: ""
    pppp: ""
    ppp: ""
    pp: ""
    mp: ""
    mf: ""
    ff: ""
    fff: ""
    ffff: ""
    sfz: ""
    trl: ""
    turn: ""
    turnx: ""
    umrd: ""
    lmrd: ""
    dplus: ""
    sld: ""
    grm: ""
    dnb: ""
    upb: ""
    opend: ""
    roll: ""
    thumb: ""
    snap: ""
    ped: ""
    pedoff: ""
    mtro: ""
    mtrc: ""
    mtr.: ""
    mtr|: ""
    longa: ""
    custos: ""
    ltr: ""
}`




 
