//diff insertselect

let baseProp0 = {
    fname: '多個符號.abc',
    istart: 1159,
    multi: 0,
    iend: 1160,
    bar_type: '|',
    pos: {},
    notes: [{ pit: 16 }],
    nhd: 0,
    mid: 12,
    ymx: 24,
    ymn: 0
}
let baseProp1 = {
    clef_line: 3,
    clef_type: 'c',
    fname: '多個符號.abc',
    istart: 1146,
    iend: 1151,
    ymx: 26,
    ymn: -1,
    seqst: true,
    shrink: 12,
    space: 0
}
let baseProp5 = {
    fname: '多個符號.abc',
    istart: 1136,
    iend: 1144,
    k_sf: 1,
    k_map: {
        '0': 0,
        '1': 0,
        '2': 0,
        '3': 1,
        '4': 0,
        '5': 0,
        '6': 0
    },
    k_mode: 0,
    k_b40: 25,
    seqst: true,
    k_old_sf: 1,
    k_y_clef: 6,
    ymx: 30,
    ymn: -2,
    shrink: 15,
    space: 0
}
let baseProp6 = {
    a_meter: [{ top: 'C' }],
    fname: '多個符號.abc',
    istart: 557,
    iend: 561,
    wmeasure: 1536,
    x_meter: [6],
    seqst: true,
    ymx: 24,
    ymn: 0,
    shrink: 9.5,
    space: 0
}
let baseProp8 = {
    fname: '多個符號.abc',
    stem: -1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1154,
    notes: [{
        pit: 22,
        shhd: 0,
        shac: 0,
        dur: 768,
        midi: 60,
        opit: 16
    }],
    dur_orig: 768,
    pos: {},
    iend: 1156,
    head: 1,
    dots: 0,
    nflags: -1,
    mid: 12,
    ys: -9,
    ymn: -11,
    ymx: 16
}
let baseProp10 = {
    fname: '多個符號.abc',
    stem: 0,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1641,
    dur_orig: 1536,
    notes: [{
        pit: 18,
        dur: 1536,
        opit: 6
    }],
    pos: {},
    iend: 1643,
    head: 2,
    dots: 0,
    nflags: -2,
    beam_st: true,
    ymx: 24,
    ymn: 0
}
let baseProp14 = {
    fname: '多個符號.abc',
    istart: 569,
    iend: 597,
    tempo_str1: 'Andante mosso',
    tempo_notes: [384],
    tempo: 110,
    notes: [{ pit: 16 }],
    nhd: 0,
    mid: 12,
    ymx: 61,
    ymn: 47,
    tempo_str: `Andante mosso <tspan
class="f5" style="font-size:15.6px" dy="-1"></tspan> <tspan dy="1">=</tspan> 110`,
    tempo_wh: [125.54480013847352, 13],
    seqst: true,
    shrink: 27,
    space: 0,
    invis: true
}
let baseProp16 = {
    subtype: 'midiprog',
    fname: '多個符號.abc',
    istart: 1136,
    iend: 1144,
    pos: {},
    invis: 1,
    play: 1,
    chn: 0,
    notes: [{ pit: 2 }],
    nhd: 0,
    mid: 12,
    ymx: 24,
    ymn: 0
}



// diff 簡化後
let mainNumber0 = [{
    fname: '多個符號.abc',
    istart: 1159,
    multi: 0,
    iend: 1160,
    bar_type: '|',
    pos: {},
    seqst: true,
    bar_num: 2,
    notes: [{ pit: 16 }],
    nhd: 0,
    mid: 12,
    ymx: 24,
    ymn: 0,
    shrink: 16.35,
    space: 31.200000000000003
}, {
    a_dd: [{
        name: '<)',
        func: 6,
        glyph: 'cresc',
        h: 15,
        hd: 2,
        wl: 0,
        wr: 0,
        str: null,
        dd_st: {
            name: '<(',
            func: 6,
            glyph: 'cresc',
            h: 15,
            hd: 2,
            wl: 0,
            wr: 0,
            str: null,
            dd_en: '__DD_end'
        }
    }, {
        name: 'dc',
        func: 3,
        glyph: '@',
        h: 20,
        hd: 0,
        wl: 0,
        wr: 0,
        dx: -20,
        dy: 0,
        str: '$5Da Capo'
    }]
}, { rbstop: 1 }, {
    a_dd: [{
        name: 'segno',
        func: 5,
        glyph: 'sgno',
        h: 22,
        hd: 2,
        wl: 5,
        wr: 5
    }],
    rbstop: 1
}, {
    a_dd: [{
        name: 'trill',
        func: 3,
        glyph: 'trl',
        h: 14,
        hd: 0,
        wl: 5,
        wr: 8
    }]
}, {
    err: true,
    a_gch: []
}, {
    rbstop: 1,
    err: true
}, {
    text: '1',
    rbstop: 2,
    rbstart: 2,
    invis: 1,
    xsh: 0
}, {
    rbstop: 2,
    text: '2',
    rbstart: 2
}, { bar_mrep: 2 }]
let mainNumber1 = [{
    clef_line: 3,
    clef_type: 'c',
    fname: '多個符號.abc',
    istart: 1146,
    iend: 1151,
    ymx: 26,
    ymn: -1,
    seqst: true,
    shrink: 12,
    space: 0
}]
let mainNumber5 = [{
    fname: '多個符號.abc',
    istart: 1136,
    iend: 1144,
    k_sf: 1,
    k_map: {
        '0': 0,
        '1': 0,
        '2': 0,
        '3': 1,
        '4': 0,
        '5': 0,
        '6': 0
    },
    k_mode: 0,
    k_b40: 25,
    seqst: true,
    k_old_sf: 1,
    k_y_clef: 6,
    ymx: 30,
    ymn: -2,
    shrink: 15,
    space: 0
}]
let mainNumber6 = [{
    a_meter: [{ top: 'C' }],
    fname: '多個符號.abc',
    istart: 557,
    iend: 561,
    wmeasure: 1536,
    x_meter: [6],
    seqst: true,
    ymx: 24,
    ymn: 0,
    shrink: 9.5,
    space: 0
}, {
    pos: {},
    notes: [{ pit: 12 }],
    nhd: 0,
    mid: 12
}]
let mainNumber8 = [{
    fname: '多個符號.abc',
    stem: -1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1154,
    a_gch: [{
        text: 'C',
        istart: 1151,
        iend: 1154,
        font: {
            name: 'text,sans-serif',
            size: 12,
            used: true,
            fid: 3,
            swfac: 13.200000000000001,
            pad: 0
        },
        pos: 1,
        type: 'g',
        otext: 'C',
        x: -3.6
    }],
    notes: [{
        pit: 22,
        shhd: 0,
        shac: 0,
        dur: 768,
        midi: 60,
        opit: 16
    }],
    dur_orig: 768,
    pos: {},
    iend: 1156,
    a_ly: [{
        t: 'Son',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1395,
        iend: 1398,
        shift: 10.9
    }, {
        t: 'Que',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1429,
        iend: 1432,
        ln: 1,
        shift: 11.3
    }],
    head: 1,
    dots: 0,
    nflags: -1,
    beam_end: true,
    beam_st: true,
    mid: 12,
    ys: -9,
    ymn: -11,
    ymx: 16
}, {
    a_dd: [{
        name: '<(',
        func: 6,
        glyph: 'cresc',
        h: 15,
        hd: 2,
        wl: 0,
        wr: 0,
        str: null,
        dd_en: {
            name: '<)',
            func: 6,
            glyph: 'cresc',
            h: 15,
            hd: 2,
            wl: 0,
            wr: 0,
            str: null,
            dd_st: '__DD_start'
        }
    }]
}, {
    seqst: true,
    shrink: 28.350000190734864,
    space: 38
}, {
    seqst: true,
    shrink: 17.5,
    space: 0
}, {
    seqst: true,
    stemless: true,
    shrink: 13.5,
    space: 0
}, {
    in_tuplet: true,
    seqst: true,
    shrink: 14.400000190734865,
    space: 0
}, {
    in_tuplet: true,
    tpe: 0,
    seqst: true,
    shrink: 10.699999809265137,
    space: 24.3000005086263
}, {
    a_dd: [{
        name: 'arpeggio',
        func: 2,
        glyph: 'arp',
        h: 12,
        hd: 0,
        wl: 10,
        wr: 3
    }],
    seqst: true,
    shrink: 19,
    space: 0
}, {
    sls: null,
    xs: 52.5
}, {
    seqst: true,
    shrink: 19.750000190734866,
    space: 26.280000686645508,
    xs: 72.94194915099649
}, {
    seqst: true,
    shrink: 9.399999618530273,
    space: 26.280000686645508,
    xs: 113.82584745298948
}, { sls: null }, {
    seqst: true,
    shrink: 12.824999904632568,
    space: 26.280000686645508,
    xs: 425.9332776737847
}, {
    a_dd: [{
        name: 'segno',
        func: 5,
        glyph: 'sgno',
        h: 22,
        hd: 2,
        wl: 5,
        wr: 5
    }],
    seqst: true,
    shrink: 15.000000190734864,
    space: 0
}, {
    seqst: true,
    shrink: 22,
    space: 0,
    soln: false
}, {
    seqst: true,
    shrink: 12.100000381469727,
    space: 26.280000686645508,
    xs: 187.18261092045037
}, {
    seqst: true,
    shrink: 16.199999809265137,
    space: 0,
    soln: false,
    xs: 39.69999980926514
}, {
    repeat_n: 0,
    repeat_k: 3,
    seqst: false,
    play: true,
    invis: true
}, {
    seqst: false,
    play: true,
    invis: true
}, {
    repeat_n: 0,
    repeat_k: 2,
    seqst: false,
    play: true,
    invis: true
}]
let mainNumber10 = [{
    fname: '多個符號.abc',
    stem: 0,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1641,
    dur_orig: 1536,
    fmr: 1,
    notes: [{
        pit: 18,
        dur: 1536,
        opit: 6
    }],
    pos: {},
    iend: 1643,
    beam_end: true,
    head: 2,
    dots: 0,
    nflags: -2,
    stemless: true,
    beam_st: true,
    mid: 12,
    ymx: 24,
    ymn: 0
}, {
    invis: true,
    seqst: true,
    shrink: 23.575,
    space: 38
}, {
    invis: true,
    nmes: 1,
    seqst: true,
    shrink: 22,
    space: 0,
    soln: false
}, {
    repeat_n: 0,
    repeat_k: 3,
    seqst: true,
    rep_nb: -1,
    shrink: 12.199999809265137,
    space: 20.231999588012695
}, {
    seqst: true,
    rep_nb: -1,
    shrink: 15,
    space: 27.733333206176763
}, {
    repeat_n: 0,
    repeat_k: 2,
    seqst: true,
    rep_nb: 2,
    shrink: 11.699999809265137,
    space: 0
}, {
    seqst: true,
    rep_nb: 3,
    shrink: 11.699999809265137,
    space: 0
}, {
    repeat_n: 0,
    repeat_k: 1,
    seqst: true,
    invis: true,
    shrink: 7,
    space: 0
}]
let mainNumber14 = [{
    fname: '多個符號.abc',
    istart: 569,
    iend: 597,
    tempo_str1: 'Andante mosso',
    tempo_notes: [384],
    tempo: 110,
    notes: [{ pit: 16 }],
    nhd: 0,
    mid: 12,
    ymx: 61,
    ymn: 47,
    tempo_str: `Andante mosso <tspan
class="f5" style="font-size:15.6px" dy="-1"></tspan> <tspan dy="1">=</tspan> 110`,
    tempo_wh: [125.54480013847352, 13],
    seqst: true,
    shrink: 27,
    space: 0,
    invis: true
}]
let mainNumber16 = [{
    subtype: 'midiprog',
    fname: '多個符號.abc',
    istart: 1136,
    iend: 1144,
    pos: {},
    invis: 1,
    play: 1,
    chn: 0,
    notes: [{ pit: 2 }],
    nhd: 0,
    mid: 12,
    ymx: 24,
    ymn: 0
}, { instr: 75 }]



// 沒簡化

let objNumber0 = [{
    fname: '多個符號.abc',
    istart: 1159,
    multi: 0,
    iend: 1160,
    bar_type: '|',
    pos: {},
    seqst: true,
    bar_num: 2,
    notes: [{ pit: 16 }],
    nhd: 0,
    mid: 12,
    ymx: 24,
    ymn: 0,
    shrink: 16.35,
    space: 31.200000000000003
}, {
    fname: '多個符號.abc',
    istart: 1231,
    multi: 0,
    a_dd: [{
        name: '<)',
        func: 6,
        glyph: 'cresc',
        h: 15,
        hd: 2,
        wl: 0,
        wr: 0,
        str: null,
        dd_st: {
            name: '<(',
            func: 6,
            glyph: 'cresc',
            h: 15,
            hd: 2,
            wl: 0,
            wr: 0,
            str: null,
            dd_en: '__DD_end'
        }
    }, {
        name: 'dc',
        func: 3,
        glyph: '@',
        h: 20,
        hd: 0,
        wl: 0,
        wr: 0,
        dx: -20,
        dy: 0,
        str: '$5Da Capo'
    }],
    iend: 1232,
    bar_type: '|',
    pos: {},
    notes: [{ pit: 23 }],
    nhd: 0,
    mid: 12,
    ymx: 24,
    ymn: 0
}, {
    fname: '多個符號.abc',
    istart: 1343,
    multi: 0,
    iend: 1345,
    rbstop: 1,
    bar_type: '[|:',
    pos: {},
    seqst: true,
    bar_num: 4,
    notes: [{ pit: 20 }],
    nhd: 0,
    mid: 12,
    ymx: 24,
    ymn: 0,
    shrink: 21.400000000000006,
    space: 54.77999725341797
}, {
    fname: '多個符號.abc',
    istart: 1469,
    multi: 0,
    a_dd: [{
        name: 'segno',
        func: 5,
        glyph: 'sgno',
        h: 22,
        hd: 2,
        wl: 5,
        wr: 5
    }],
    iend: 1471,
    rbstop: 1,
    bar_type: '[|:',
    pos: {},
    notes: [{ pit: 20 }],
    nhd: 0,
    mid: 12,
    ymx: 24,
    ymn: 0
}, {
    fname: '多個符號.abc',
    istart: 1359,
    multi: 0,
    a_dd: [{
        name: 'trill',
        func: 3,
        glyph: 'trl',
        h: 14,
        hd: 0,
        wl: 5,
        wr: 8
    }],
    iend: 1360,
    bar_type: '|',
    pos: {},
    seqst: true,
    bar_num: 5,
    notes: [{ pit: 26 }],
    nhd: 0,
    mid: 12,
    ymx: 24,
    ymn: 0,
    shrink: 11.699999809265137,
    space: 31.200000000000003
}, {
    fname: '多個符號.abc',
    istart: 1483,
    multi: 0,
    err: true,
    a_gch: [],
    iend: 1484,
    bar_type: '|',
    pos: {},
    notes: [{ pit: 23 }],
    nhd: 0,
    mid: 12,
    ymx: 24,
    ymn: 0
}, {
    fname: '多個符號.abc',
    istart: 1982,
    multi: 0,
    rbstop: 1,
    iend: 1984,
    bar_type: ':][:',
    pos: {},
    err: true,
    notes: [{ pit: 18 }],
    nhd: 0,
    mid: 12,
    ymx: 24,
    ymn: 0
}, {
    fname: '多個符號.abc',
    istart: 2400,
    multi: 0,
    text: '1',
    iend: 2402,
    rbstop: 2,
    rbstart: 2,
    bar_type: '[',
    pos: {},
    invis: 1,
    xsh: 0,
    seqst: true,
    notes: [{ pit: 18 }],
    nhd: 0,
    mid: 12,
    ymx: 24,
    ymn: 0,
    shrink: 9,
    space: 0
}, {
    fname: '多個符號.abc',
    istart: 2284,
    multi: 0,
    rbstop: 2,
    text: '2',
    iend: 2287,
    rbstart: 2,
    bar_type: ':|]',
    pos: {},
    seqst: true,
    bar_num: 30,
    notes: [{ pit: 19 }],
    nhd: 0,
    mid: 12,
    ymx: 24,
    ymn: 0,
    shrink: 20.40000000000009,
    space: 41.460001373291014
}, {
    fname: '多個符號.abc',
    istart: 2720,
    multi: 0,
    iend: 2721,
    bar_type: '|',
    pos: {},
    seqst: true,
    bar_num: 48,
    notes: [{ pit: 24 }],
    nhd: 0,
    bar_mrep: 2,
    mid: 12,
    ymx: 24,
    ymn: 0,
    shrink: 5,
    space: 64.48999710083008
}]
let objNumber1 = [{
    clef_line: 3,
    clef_type: 'c',
    fname: '多個符號.abc',
    istart: 1146,
    iend: 1151,
    ymx: 26,
    ymn: -1,
    seqst: true,
    shrink: 12,
    space: 0
}]
let objNumber5 = [{
    fname: '多個符號.abc',
    istart: 1136,
    iend: 1144,
    k_sf: 1,
    k_map: {
        '0': 0,
        '1': 0,
        '2': 0,
        '3': 1,
        '4': 0,
        '5': 0,
        '6': 0
    },
    k_mode: 0,
    k_b40: 25,
    seqst: true,
    k_old_sf: 1,
    k_y_clef: 6,
    ymx: 30,
    ymn: -2,
    shrink: 15,
    space: 0
}]
let objNumber6 = [{
    a_meter: [{ top: 'C' }],
    fname: '多個符號.abc',
    istart: 557,
    iend: 561,
    wmeasure: 1536,
    x_meter: [6],
    seqst: true,
    ymx: 24,
    ymn: 0,
    shrink: 9.5,
    space: 0
}, {
    a_meter: [{
        top: '2',
        bot: '4'
    }],
    fname: '多個符號.abc',
    istart: 1789,
    iend: 1794,
    wmeasure: 768,
    pos: {},
    seqst: true,
    notes: [{ pit: 12 }],
    nhd: 0,
    mid: 12,
    ymx: 24,
    ymn: 0,
    x_meter: [6],
    shrink: 6,
    space: 0
}]
let objNumber8 = [{
    fname: '多個符號.abc',
    stem: -1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1154,
    a_gch: [{
        text: 'C',
        istart: 1151,
        iend: 1154,
        font: {
            name: 'text,sans-serif',
            size: 12,
            used: true,
            fid: 3,
            swfac: 13.200000000000001,
            pad: 0
        },
        pos: 1,
        type: 'g',
        otext: 'C',
        x: -3.6
    }],
    notes: [{
        pit: 22,
        shhd: 0,
        shac: 0,
        dur: 768,
        midi: 60,
        opit: 16
    }],
    dur_orig: 768,
    pos: {},
    iend: 1156,
    a_ly: [{
        t: 'Son',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1395,
        iend: 1398,
        shift: 10.9
    }, {
        t: 'Que',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1429,
        iend: 1432,
        ln: 1,
        shift: 11.3
    }],
    head: 1,
    dots: 0,
    nflags: -1,
    beam_end: true,
    beam_st: true,
    mid: 12,
    ys: -9,
    ymn: -11,
    ymx: 16
}, {
    fname: '多個符號.abc',
    stem: -1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1210,
    a_gch: [{
        text: 'C',
        istart: 1206,
        iend: 1209,
        font: {
            name: 'text,sans-serif',
            size: 12,
            used: true,
            fid: 3,
            swfac: 13.200000000000001,
            pad: 0
        },
        pos: 1,
        type: 'g',
        otext: 'C',
        x: -3.6
    }],
    notes: [{
        pit: 23,
        shhd: 0,
        shac: 0,
        dur: 384,
        midi: 72,
        invis: true,
        a_dd: [{
            name: 'head-shd',
            func: 9,
            glyph: 'shd',
            h: 0,
            hd: 0,
            wl: 0,
            wr: 0
        }]
    }],
    dur_orig: 384,
    pos: {},
    a_dd: [{
        name: '<(',
        func: 6,
        glyph: 'cresc',
        h: 15,
        hd: 2,
        wl: 0,
        wr: 0,
        str: null,
        dd_en: {
            name: '<)',
            func: 6,
            glyph: 'cresc',
            h: 15,
            hd: 2,
            wl: 0,
            wr: 0,
            str: null,
            dd_st: '__DD_start'
        }
    }],
    iend: 1211,
    a_ly: [{
        t: 'Son',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1515,
        iend: 1518,
        shift: 10.9
    }, {
        t: 'Que',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1576,
        iend: 1579,
        ln: 1,
        shift: 11.3
    }],
    head: 0,
    dots: 0,
    nflags: 0,
    beam_end: true,
    beam_st: true,
    mid: 12,
    ys: -6,
    ymn: -8,
    ymx: 41
}, {
    fname: '多個符號.abc',
    stem: -1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1212,
    notes: [{
        pit: 23,
        shhd: 0,
        shac: 0,
        dur: 384,
        midi: 72,
        invis: true,
        a_dd: [{
            name: 'head-shd',
            func: 9,
            glyph: 'shd',
            h: 0,
            hd: 0,
            wl: 0,
            wr: 0
        }]
    }],
    dur_orig: 384,
    pos: {},
    iend: 1213,
    a_ly: [{
        t: 'que',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1519,
        iend: 1522,
        ln: 1,
        shift: 10.100000000000001
    }, {
        t: 'sti',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1580,
        iend: 1583,
        shift: 7.300000000000001
    }],
    seqst: true,
    head: 0,
    dots: 0,
    nflags: 0,
    beam_end: true,
    beam_st: true,
    mid: 12,
    ys: -6,
    ymn: -8,
    ymx: 19,
    shrink: 28.350000190734864,
    space: 38
}, {
    fname: '多個符號.abc',
    stem: -1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1178,
    a_gch: [{
        text: '[F♯m7♭5]',
        istart: 1161,
        iend: 1178,
        font: {
            name: 'text,sans-serif',
            size: 12,
            used: true,
            fid: 3,
            swfac: 13.200000000000001,
            pad: 0
        },
        pos: 1,
        type: 'g',
        otext: '[F#m7b5]',
        x: -8
    }, {
        text: 'F♯dim7',
        istart: 1161,
        iend: 1178,
        font: {
            name: 'text,sans-serif',
            size: 12,
            used: true,
            fid: 3,
            swfac: 13.200000000000001,
            pad: 0
        },
        pos: 1,
        type: 'g',
        otext: 'F#dim7',
        x: -8
    }],
    notes: [{
        pit: 25,
        shhd: 0,
        shac: 10,
        acc: 1,
        dur: 768,
        midi: 66,
        opit: 19
    }],
    dur_orig: 768,
    pos: {},
    iend: 1181,
    a_ly: [{
        t: 'sti i',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1403,
        iend: 1408,
        shift: 10.5
    }, {
        t: 'son',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1437,
        iend: 1440,
        shift: 9.700000000000001
    }],
    seqst: true,
    head: 1,
    dots: 0,
    nflags: -1,
    beam_end: true,
    beam_st: true,
    mid: 12,
    ys: 0,
    ymn: -2,
    ymx: 25,
    shrink: 17.5,
    space: 0
}, {
    fname: '多個符號.abc',
    stem: -1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1193,
    a_gch: [{
        text: 'G',
        istart: 1186,
        iend: 1189,
        font: {
            name: 'text,sans-serif',
            size: 12,
            used: true,
            fid: 3,
            swfac: 13.200000000000001,
            pad: 0
        },
        pos: 1,
        type: 'g',
        otext: 'G',
        x: -3.6
    }, {
        text: 'Em',
        istart: 1189,
        iend: 1193,
        font: {
            name: 'text,sans-serif',
            size: 12,
            used: true,
            fid: 3,
            swfac: 13.200000000000001,
            pad: 0
        },
        pos: 1,
        type: 'g',
        otext: 'Em',
        x: -7.2
    }],
    notes: [{
        pit: 26,
        shhd: 0,
        shac: 0,
        dur: 1536,
        midi: 67,
        opit: 20
    }],
    dur_orig: 1536,
    pos: {},
    iend: 1195,
    beam_end: true,
    a_ly: [{
        t: 'spi',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1413,
        iend: 1416,
        shift: 8.5
    }, {
        t: 'chi',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1448,
        iend: 1451,
        shift: 8.9
    }],
    seqst: true,
    head: 2,
    dots: 0,
    nflags: -2,
    stemless: true,
    beam_st: true,
    mid: 12,
    ys: 24,
    ymx: 28,
    ymn: 20,
    shrink: 13.5,
    space: 0
}, {
    fname: '多個符號.abc',
    stem: -1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1347,
    notes: [{
        pit: 34,
        shhd: 0,
        shac: 0,
        dur: 384,
        midi: 81,
        opit: 28
    }],
    dur_orig: 384,
    pos: {},
    iend: 1348,
    in_tuplet: true,
    a_ly: [{
        t: 'cri',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1417,
        iend: 1420,
        ln: 1,
        shift: 8.1
    }, {
        t: 'che',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1452,
        iend: 1455,
        shift: 9.700000000000001
    }],
    seqst: true,
    head: 0,
    dots: 0,
    nflags: 0,
    beam_end: true,
    beam_st: true,
    mid: 12,
    ys: 12,
    ymn: -1,
    ymx: 52,
    shrink: 14.400000190734865,
    space: 0
}, {
    fname: '多個符號.abc',
    stem: -1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1349,
    notes: [{
        pit: 29,
        shhd: 0,
        shac: 0,
        dur: 384,
        midi: 72,
        opit: 23
    }],
    dur_orig: 384,
    pos: {},
    iend: 1350,
    in_tuplet: true,
    tpe: 0,
    seqst: true,
    head: 0,
    dots: 0,
    nflags: 0,
    beam_end: true,
    beam_st: true,
    mid: 12,
    ys: 12,
    ymn: -1,
    ymx: 37,
    shrink: 10.699999809265137,
    space: 24.3000005086263
}, {
    fname: '多個符號.abc',
    stem: -1,
    multi: 0,
    nhd: 3,
    xmx: 0,
    istart: 1370,
    notes: [{
        pit: 22,
        shhd: -7.4,
        shac: 0,
        dur: 384,
        midi: 60,
        opit: 16
    }, {
        pit: 23,
        shhd: 0,
        shac: 0,
        dur: 384,
        midi: 62,
        opit: 17
    }, {
        pit: 27,
        shhd: -7.4,
        shac: 0,
        dur: 384,
        midi: 69,
        opit: 21
    }, {
        pit: 28,
        shhd: 0,
        shac: 0,
        dur: 384,
        midi: 71,
        opit: 22
    }],
    dur_orig: 384,
    pos: {},
    a_dd: [{
        name: 'arpeggio',
        func: 2,
        glyph: 'arp',
        h: 12,
        hd: 0,
        wl: 10,
        wr: 3
    }],
    iend: 1376,
    seqst: true,
    head: 0,
    dots: 0,
    nflags: 0,
    beam_end: true,
    beam_st: true,
    mid: 12,
    ys: -7,
    ymn: -9,
    ymx: 34,
    shrink: 19,
    space: 0
}, {
    fname: '多個符號.abc',
    stem: 1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1485,
    notes: [{
        pit: 19,
        shhd: 0,
        shac: 0,
        dur: 192,
        midi: 66
    }],
    dur_orig: 192,
    pos: {},
    iend: 1487,
    sls: null,
    a_ly: [{
        t: 'so',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1568,
        iend: 1570,
        shift: 6.9
    }, {
        t: 'so',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1624,
        iend: 1626,
        shift: 6.9
    }],
    head: 0,
    dots: 0,
    nflags: 1,
    beam_st: true,
    mid: 12,
    ymn: -17.35092379052839,
    ys: 25.20786133485596,
    ymx: 27.70786133485596,
    xs: 52.5
}, {
    fname: '多個符號.abc',
    stem: 1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1487,
    notes: [{
        pit: 20,
        shhd: 0,
        shac: 0,
        dur: 192,
        midi: 67
    }],
    dur_orig: 192,
    pos: {},
    iend: 1489,
    a_ly: [{
        t: 'e',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1571,
        iend: 1572,
        shift: 4.5
    }, {
        t: 'mi',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1627,
        iend: 1629,
        ln: 1,
        shift: 8.1
    }],
    seqst: true,
    head: 0,
    dots: 0,
    nflags: 1,
    mid: 12,
    ymn: -14.997415375197654,
    ys: 27.402620444951985,
    ymx: 29.902620444951985,
    shrink: 19.750000190734866,
    space: 26.280000686645508,
    xs: 72.94194915099649
}, {
    fname: '多個符號.abc',
    stem: 1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1491,
    notes: [{
        pit: 22,
        shhd: 0,
        shac: 0,
        dur: 192,
        midi: 71
    }],
    dur_orig: 192,
    pos: {},
    iend: 1493,
    seqst: true,
    head: 0,
    dots: 0,
    nflags: 1,
    beam_end: true,
    mid: 12,
    ymn: 8,
    ys: 31.792138665144037,
    ymx: 34.29213866514404,
    shrink: 9.399999618530273,
    space: 26.280000686645508,
    xs: 113.82584745298948
}, {
    fname: '多個符號.abc',
    stem: -1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 2092,
    notes: [{
        pit: 26,
        shhd: 0,
        shac: 0,
        dur: 384,
        midi: 57,
        opit: 14
    }],
    dur_orig: 384,
    pos: {},
    iend: 2093,
    sls: null,
    a_ly: [{
        t: 'vi',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 2133,
        iend: 2135,
        shift: 6.5
    }, {
        t: 'fi',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 2181,
        iend: 2183,
        shift: 5.7
    }],
    head: 0,
    dots: 0,
    nflags: 0,
    beam_end: true,
    beam_st: true,
    mid: 12,
    ys: 3,
    ymn: 0,
    ymx: 44.71601166180421
}, {
    fname: '多個符號.abc',
    stem: -1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 2095,
    notes: [{
        pit: 24,
        shhd: 0,
        shac: 0,
        dur: 192,
        midi: 54,
        opit: 12
    }],
    dur_orig: 192,
    pos: {},
    iend: 2097,
    a_ly: [{
        t: '-',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 2138,
        iend: 2139,
        ln: 2,
        shift: 0
    }, {
        t: '-',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 2186,
        iend: 2187,
        ln: 2,
        shift: 0
    }],
    seqst: true,
    head: 0,
    dots: 0,
    nflags: 1,
    beam_end: true,
    mid: 12,
    ys: -2.5973795550480148,
    ymn: -5.097379555048015,
    ymx: 39.91343791495954,
    shrink: 12.824999904632568,
    space: 26.280000686645508,
    xs: 425.9332776737847
}, {
    fname: '多個符號.abc',
    stem: -1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1813,
    notes: [{
        pit: 29,
        shhd: 0,
        shac: 0,
        dur: 384,
        midi: 72,
        opit: 23
    }],
    dur_orig: 384,
    pos: {},
    a_dd: [{
        name: 'segno',
        func: 5,
        glyph: 'sgno',
        h: 22,
        hd: 2,
        wl: 5,
        wr: 5
    }],
    iend: 1814,
    a_ly: [{
        t: 'so',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1860,
        iend: 1862,
        shift: 6.9
    }, {
        t: 'so,',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1909,
        iend: 1912,
        shift: 8.5
    }],
    seqst: true,
    head: 0,
    dots: 0,
    nflags: 0,
    beam_end: true,
    beam_st: true,
    mid: 12,
    ys: 12,
    ymn: 9,
    ymx: 61,
    shrink: 15.000000190734864,
    space: 0
}, {
    fname: '多個符號.abc',
    stem: -1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1820,
    notes: [{
        pit: 29,
        shhd: 0,
        shac: 0,
        dur: 1152,
        midi: 72,
        opit: 23
    }],
    dur_orig: 1152,
    pos: {},
    iend: 1822,
    a_ly: [{
        t: 'man',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1874,
        iend: 1877,
        ln: 1,
        shift: 11.700000000000001
    }, {
        t: 'stai',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 1923,
        iend: 1927,
        shift: 9.700000000000001
    }],
    seqst: true,
    head: 1,
    dots: 1,
    nflags: -1,
    beam_end: true,
    beam_st: true,
    mid: 12,
    ys: 12,
    ymn: 9,
    ymx: 37,
    shrink: 22,
    space: 0,
    soln: false
}, {
    fname: '多個符號.abc',
    stem: 1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 2391,
    notes: [{
        pit: 18,
        shhd: 0,
        shac: 10,
        acc: 3,
        dur: 96,
        midi: 64
    }],
    dur_orig: 96,
    pos: {},
    iend: 2395,
    a_ly: [{
        t: '-',
        font: {
            size: 14,
            name: 'serif',
            swfac: 14,
            pad: 0,
            fname: 'serif',
            used: true,
            fid: 4
        },
        istart: 2460,
        iend: 2461,
        ln: 2,
        shift: 0
    }],
    seqst: true,
    head: 0,
    dots: 0,
    nflags: 2,
    beam_st: true,
    mid: 12,
    ymn: -25.529981672074072,
    ys: 22.199999999999996,
    ymx: 24.699999999999996,
    shrink: 12.100000381469727,
    space: 26.280000686645508,
    xs: 187.18261092045037
}, {
    fname: '多個符號.abc',
    stem: -1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 2581,
    notes: [{
        pit: 22,
        shhd: 0,
        shac: 0,
        dur: 96,
        midi: 60,
        opit: 16
    }],
    dur_orig: 96,
    pos: {},
    iend: 2584,
    seqst: true,
    head: 0,
    dots: 0,
    nflags: 2,
    beam_st: true,
    mid: 12,
    ys: -10.200000000000001,
    ymn: -12.700000000000001,
    ymx: 16,
    shrink: 16.199999809265137,
    space: 0,
    soln: false,
    xs: 39.69999980926514
}, {
    fname: '多個符號.abc',
    stem: -1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 2609,
    repeat_n: 0,
    repeat_k: 3,
    notes: [{
        pit: 28,
        shhd: 0,
        shac: 0,
        dur: 384,
        midi: 60,
        opit: 22
    }],
    dur_orig: 96,
    pos: {},
    iend: 2612,
    seqst: false,
    head: 0,
    dots: 0,
    nflags: 2,
    beam_st: true,
    play: true,
    invis: true,
    mid: 12,
    ys: 9,
    ymn: 6,
    ymx: 34
}, {
    fname: '多個符號.abc',
    stem: -1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 2618,
    notes: [{
        pit: 24,
        shhd: 0,
        shac: 0,
        dur: 96,
        midi: 64,
        opit: 18
    }],
    dur_orig: 96,
    pos: {},
    iend: 2621,
    beam_end: true,
    seqst: false,
    head: 0,
    dots: 0,
    nflags: 2,
    play: true,
    invis: true,
    mid: 12,
    ys: -3,
    ymn: -5,
    ymx: 22
}, {
    fname: '多個符號.abc',
    stem: -1,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 2677,
    repeat_n: 0,
    repeat_k: 2,
    notes: [{
        pit: 26,
        shhd: 0,
        shac: 0,
        dur: 1536,
        midi: 67,
        opit: 20
    }],
    dur_orig: 384,
    pos: {},
    iend: 2678,
    seqst: false,
    head: 0,
    dots: 0,
    nflags: 0,
    beam_end: true,
    beam_st: true,
    play: true,
    invis: true,
    mid: 12,
    ys: 3,
    ymn: 0,
    ymx: 28
}]
let objNumber10 = [{
    fname: '多個符號.abc',
    stem: 0,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1641,
    dur_orig: 1536,
    fmr: 1,
    notes: [{
        pit: 18,
        dur: 1536,
        opit: 6
    }],
    pos: {},
    iend: 1643,
    beam_end: true,
    head: 2,
    dots: 0,
    nflags: -2,
    stemless: true,
    beam_st: true,
    mid: 12,
    ymx: 24,
    ymn: 0
}, {
    fname: '多個符號.abc',
    stem: 0,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1377,
    invis: true,
    dur_orig: 384,
    notes: [{
        pit: 24,
        dur: 384,
        opit: 18
    }],
    pos: {},
    iend: 1378,
    seqst: true,
    head: 0,
    dots: 0,
    nflags: 0,
    beam_end: true,
    beam_st: true,
    mid: 12,
    ymx: 28,
    ymn: 8,
    shrink: 23.575,
    space: 38
}, {
    fname: '多個符號.abc',
    stem: 0,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 1380,
    invis: true,
    nmes: 1,
    dur_orig: 1536,
    fmr: 1,
    notes: [{
        pit: 24,
        dur: 1536,
        opit: 18
    }],
    pos: {},
    iend: 1381,
    beam_end: true,
    seqst: true,
    head: 2,
    dots: 0,
    nflags: -2,
    stemless: true,
    beam_st: true,
    mid: 12,
    ymx: 28,
    ymn: 18,
    shrink: 22,
    space: 0,
    soln: false
}, {
    fname: '多個符號.abc',
    stem: 0,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 2609,
    repeat_n: 0,
    repeat_k: 3,
    notes: [{
        pit: 28,
        shhd: 0,
        shac: 0,
        dur: 384,
        midi: 60,
        opit: 22
    }],
    dur_orig: 96,
    pos: {},
    iend: 2612,
    seqst: true,
    head: 4,
    dots: 0,
    nflags: 2,
    beam_st: true,
    rep_nb: -1,
    ymx: 24,
    ymn: 0,
    shrink: 12.199999809265137,
    space: 20.231999588012695
}, {
    fname: '多個符號.abc',
    stem: 0,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 2624,
    notes: [{
        pit: 28,
        shhd: 0,
        shac: 0,
        dur: 384,
        midi: 60,
        opit: 22
    }],
    dur_orig: 96,
    pos: {},
    iend: 2627,
    seqst: true,
    head: 4,
    dots: 0,
    nflags: 2,
    beam_st: true,
    rep_nb: -1,
    mid: 12,
    ymx: 24,
    ymn: 0,
    shrink: 15,
    space: 27.733333206176763
}, {
    fname: '多個符號.abc',
    stem: 0,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 2677,
    repeat_n: 0,
    repeat_k: 2,
    notes: [{
        pit: 26,
        shhd: 0,
        shac: 0,
        dur: 1536,
        midi: 67,
        opit: 20
    }],
    dur_orig: 384,
    pos: {},
    iend: 2678,
    seqst: true,
    head: 0,
    dots: 0,
    nflags: -2,
    beam_end: true,
    beam_st: true,
    rep_nb: 2,
    ymx: 24,
    ymn: 0,
    shrink: 11.699999809265137,
    space: 0
}, {
    fname: '多個符號.abc',
    stem: 0,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 2684,
    notes: [{
        pit: 26,
        shhd: 0,
        shac: 0,
        dur: 1536,
        midi: 67,
        opit: 20
    }],
    dur_orig: 384,
    pos: {},
    iend: 2685,
    seqst: true,
    head: 0,
    dots: 0,
    nflags: -2,
    beam_end: true,
    beam_st: true,
    rep_nb: 3,
    mid: 12,
    ymx: 24,
    ymn: 0,
    shrink: 11.699999809265137,
    space: 0
}, {
    fname: '多個符號.abc',
    stem: 0,
    multi: 0,
    nhd: 0,
    xmx: 0,
    istart: 2716,
    repeat_n: 0,
    repeat_k: 1,
    notes: [{
        pit: 32,
        shhd: 0,
        shac: 0,
        dur: 1536,
        midi: 67,
        opit: 26
    }],
    dur_orig: 768,
    pos: {},
    iend: 2718,
    seqst: true,
    head: 1,
    dots: 0,
    nflags: -1,
    beam_end: true,
    beam_st: true,
    invis: true,
    ymx: 30,
    ymn: 20,
    shrink: 7,
    space: 0
}]
let objNumber14 = [{
    fname: '多個符號.abc',
    istart: 569,
    iend: 597,
    tempo_str1: 'Andante mosso',
    tempo_notes: [384],
    tempo: 110,
    notes: [{ pit: 16 }],
    nhd: 0,
    mid: 12,
    ymx: 61,
    ymn: 47,
    tempo_str: `Andante mosso <tspan
class="f5" style="font-size:15.6px" dy="-1"></tspan> <tspan dy="1">=</tspan> 110`,
    tempo_wh: [125.54480013847352, 13],
    seqst: true,
    shrink: 27,
    space: 0,
    invis: true
}]
let objNumber16 = [{
    subtype: 'midiprog',
    fname: '多個符號.abc',
    istart: 1136,
    iend: 1144,
    pos: {},
    invis: 1,
    play: 1,
    chn: 0,
    notes: [{ pit: 2 }],
    nhd: 0,
    mid: 12,
    ymx: 24,
    ymn: 0
}, {
    subtype: 'midiprog',
    fname: '多個符號.abc',
    istart: 1136,
    iend: 1144,
    pos: {},
    invis: 1,
    play: 1,
    instr: 75,
    chn: 0,
    notes: [{ pit: 2 }],
    nhd: 0,
    mid: 12,
    ymx: 24,
    ymn: 0
}]




let P_V_voidTune = {
    "v": 0,
    "id": "1",
    "time": 44544,
    "pos": {},
    "scale": 1,
    "ulen": 384,
    "dur_fact": 1,
    "meter": {
        "type": 6,
        "dur": 0,
        "a_meter": [
            {
                "top": "C"
            }
        ],
        "fname": "多個符號.abc",
        "istart": 557,
        "iend": 561,
        "wmeasure": 1536,
        "x_meter": [
            6
        ],
        "wl": 1,
        "wr": 19,
        "p_v": "__Tunes",
        "v": 0,
        "st": 0,
        "time": 0,
        "next": "__voiceItem",
        "prev": "__voiceItem",
        "ts_next": "__voiceItem",
        "ts_prev": "__voiceItem",
        "seqst": true,
        "fmt": "__Formation",
        "ymx": 24,
        "ymn": 0,
        "shrink": 9.5,
        "space": 0,
        "x": 36.5
    },
    "wmeasure": 1536,
    "staffnonote": 1,
    "clef": {
        "type": 1,
        "clef_line": 3,
        "clef_type": "c",
        "v": 0,
        "p_v": "__Tunes",
        "time": 0,
        "dur": 0,
        "fname": "多個符號.abc",
        "istart": 1146,
        "iend": 1151,
        "fmt": "__Formation",
        "wl": 12,
        "wr": 13
    },
    "acc": [],
    "sls": [],
    "hy_st": 0,
    "cst": 0,
    "st": 0,
    "ckey": {
        "type": 5,
        "dur": 0,
        "fname": "多個符號.abc",
        "istart": 1136,
        "iend": 1144,
        "k_sf": 1,
        "k_map": {
            "0": 0,
            "1": 0,
            "2": 0,
            "3": 1,
            "4": 0,
            "5": 0,
            "6": 0
        },
        "k_mode": 0,
        "k_b40": 25,
        "wl": 2,
        "wr": 8.5,
        "p_v": "__Tunes",
        "v": 0,
        "st": 0,
        "time": 38400,
        "next": "__voiceItem",
        "prev": "__voiceItem",
        "ts_next": "__voiceItem",
        "ts_prev": "__voiceItem",
        "seqst": true,
        "k_old_sf": 1,
        "fmt": "__Formation",
        "k_y_clef": 6,
        "ymx": 30,
        "ymn": -2,
        "shrink": 15,
        "space": 0,
        "x": 27
    },
    "init": true,
    "jianpu": false,
    "nm": "Soprano",
    "snm": "A",
    "ignore": false,
    "last_note": {
        "type": 8,
        "fname": "多個符號.abc",
        "stem": -1,
        "multi": 0,
        "nhd": 0,
        "xmx": 0,
        "istart": 2726,
        "notes": [
            {
                "pit": 29,
                "shhd": 0,
                "shac": 0,
                "dur": 1536,
                "midi": 72,
                "opit": 23
            }
        ],
        "dur_orig": 1536,
        "dur": 1536,
        "prev": "__voiceItem",
        "v": 0,
        "p_v": "__Tunes",
        "st": 0,
        "time": 43008,
        "fmt": "__Formation",
        "pos": {},
        "iend": 2728,
        "next": "__voiceItem",
        "seqst": true,
        "ts_prev": "__voiceItem",
        "ts_next": "__voiceItem",
        "head": 2,
        "dots": 0,
        "nflags": -2,
        "stemless": true,
        "beam_end": true,
        "beam_st": true,
        "mid": 12,
        "ys": 33,
        "y": 33,
        "ymx": 37,
        "ymn": 29,
        "wr": 6,
        "wl": 6,
        "shrink": 13,
        "space": 0,
        "x": 333.4807514058446
    },
    "sym": {
        "type": 12,
        "fname": "多個符號.abc",
        "dur": 0,
        "v": 0,
        p_v: "__Tunes",
        "time": 0,
        "st": 0,
        "sy": {
            "voices": [
                {
                    "range": 0,
                    "st": 0
                },
                {
                    "range": 1,
                    "st": 1
                },
                {
                    "range": 2,
                    "st": 2
                }
            ],
            "staves": [
                {
                    "stafflines": "|||||",
                    "staffscale": 1,
                    "flags": 68,
                    "staffnonote": 1
                },
                {
                    "stafflines": "|||||",
                    "staffscale": 1,
                    "flags": 64,
                    "staffnonote": 1
                },
                {
                    "stafflines": "|||||",
                    "staffscale": 1,
                    "flags": 8,
                    "staffnonote": 1
                }
            ],
            "top_voice": 0,
            "nstaff": 2,
            "st_print": {
                "0": 1,
                "1": 0,
                "2": 0
            }
        },
        "next": "__voiceItem",
        "fmt": "__Formation",
        "seqst": true,
        "ts_next": "__voiceItem",
        "notes": [
            {
                "pit": 16
            }
        ],
        "nhd": 0,
        "ymx": 24,
        "ymn": 0,
        "wr": 13.550000381469726,
        "wl": 11.3,
        "shrink": 16.3,
        "space": 0,
        "x": 16.3
    },
    "last_sym": {
        "type": 5,
        "dur": 0,
        "fname": "多個符號.abc",
        "istart": 1136,
        "iend": 1144,
        "k_sf": 1,
        "k_map": {
            "0": 0,
            "1": 0,
            "2": 0,
            "3": 1,
            "4": 0,
            "5": 0,
            "6": 0
        },
        "k_mode": 0,
        "k_b40": 25,
        "wl": 2,
        "wr": 8.5,
        "p_v": "__Tunes",
        "v": 0,
        "st": 0,
        "time": 38400,
        "next": "__voiceItem",
        "prev": "__voiceItem",
        "ts_next": "__voiceItem",
        "ts_prev": "__voiceItem",
        "seqst": true,
        "k_old_sf": 1,
        "fmt": "__Formation",
        "k_y_clef": 6,
        "ymx": 30,
        "ymn": -2,
        "shrink": 15,
        "space": 0,
        "x": 27
    },
    "lyric_restart": {
        "type": 8,
        "fname": "多個符號.abc",
        "stem": -1,
        "multi": 0,
        "nhd": 0,
        "xmx": 0,
        "istart": 2581,
        "notes": [
            {
                "pit": 22,
                "shhd": 0,
                "shac": 0,
                "dur": 96,
                "midi": 60,
                "opit": 16
            }
        ],
        "dur_orig": 96,
        "dur": 96,
        "prev": "__voiceItem",
        "v": 0,
        "p_v": "__Tunes",
        "st": 0,
        "time": 30720,
        "fmt": "__Formation",
        "pos": {},
        "iend": 2584,
        "next": "__voiceItem",
        "seqst": true,
        "ts_prev": "__voiceItem",
        "ts_next": "__voiceItem",
        "head": 0,
        "dots": 0,
        "nflags": 2,
        "beam_st": true,
        "mid": 12,
        "ys": -10.200000000000001,
        "ymn": -12.700000000000001,
        "y": 12,
        "ymx": 16,
        "wr": 4.699999809265137,
        "wl": 7.699999809265137,
        "shrink": 16.199999809265137,
        "space": 0,
        "soln": false,
        "x": 43.19999980926514,
        "xs": 39.69999980926514
    },
    "sym_restart": {
        "type": 8,
        "fname": "多個符號.abc",
        "stem": -1,
        "multi": 0,
        "nhd": 0,
        "xmx": 0,
        "istart": 1154,
        "a_gch": [
            {
                "text": "C",
                "istart": 1151,
                "iend": 1154,
                "font": {
                    "name": "text,sans-serif",
                    "size": 12,
                    "used": true,
                    "fid": 3,
                    "swfac": 13.200000000000001,
                    "pad": 0
                },
                "pos": 1,
                "type": "g",
                "otext": "C",
                "x": -3.6
            }
        ],
        "notes": [
            {
                "pit": 22,
                "shhd": 0,
                "shac": 0,
                "dur": 768,
                "midi": 60,
                "opit": 16
            }
        ],
        "dur_orig": 768,
        "dur": 768,
        "prev": "__voiceItem",
        "v": 0,
        "p_v": "__Tunes",
        "st": 0,
        "time": 0,
        "fmt": "__Formation",
        "pos": {},
        "iend": 1156,
        "next": "__voiceItem",
        "a_ly": [
            {
                "t": "Son",
                "font": {
                    "size": 14,
                    "name": "serif",
                    "swfac": 14,
                    "pad": 0,
                    "fname": "serif",
                    "used": true,
                    "fid": 4
                },
                "istart": 1395,
                "iend": 1398,
                "shift": 10.9
            },
            {
                "t": "Que",
                "font": {
                    "size": 14,
                    "name": "serif",
                    "swfac": 14,
                    "pad": 0,
                    "fname": "serif",
                    "used": true,
                    "fid": 4
                },
                "istart": 1429,
                "iend": 1432,
                "ln": 1,
                "shift": 11.3
            }
        ],
        "ts_prev": "__voiceItem",
        "ts_next": "__voiceItem",
        "head": 1,
        "dots": 0,
        "nflags": -1,
        "beam_end": true,
        "beam_st": true,
        "mid": 12,
        "ys": -9,
        "ymn": -11,
        "y": 12,
        "ymx": 16,
        "wr": 5,
        "wl": 8,
        "x": 63.5
    },
    "have_ly": true,
    "lyric_start": {
        "type": 8,
        "fname": "多個符號.abc",
        "stem": -1,
        "multi": 0,
        "nhd": 0,
        "xmx": 0,
        "istart": 2244,
        "notes": [
            {
                "pit": 32,
                "shhd": 0,
                "shac": 0,
                "dur": 384,
                "midi": 78,
                "opit": 26
            }
        ],
        "dur_orig": 384,
        "dur": 384,
        "prev": "__voiceItem",
        "v": 0,
        "p_v": "__Tunes",
        "st": 0,
        "time": 19968,
        "fmt": "__Formation",
        "pos": {},
        "iend": 2245,
        "next": "__voiceItem",
        "a_ly": [
            {
                "t": "dim",
                "font": {
                    "size": 14,
                    "name": "serif",
                    "swfac": 14,
                    "pad": 0,
                    "fname": "serif",
                    "used": true,
                    "fid": 4
                },
                "istart": 2295,
                "iend": 2298,
                "ln": 1,
                "shift": 10.9
            },
            {
                "t": "sol",
                "font": {
                    "size": 14,
                    "name": "serif",
                    "swfac": 14,
                    "pad": 0,
                    "fname": "serif",
                    "used": true,
                    "fid": 4
                },
                "istart": 2330,
                "iend": 2333,
                "shift": 8.5
            }
        ],
        "seqst": true,
        "ts_prev": "__voiceItem",
        "ts_next": "__voiceItem",
        "head": 0,
        "dots": 0,
        "nflags": 0,
        "beam_end": true,
        "beam_st": true,
        "mid": 12,
        "ys": 12,
        "ymn": 9,
        "y": 42,
        "ymx": 46,
        "wr": 4.699999809265137,
        "wl": 6.9,
        "shrink": 14.200000190734864,
        "space": 0,
        "x": 471.6271193512125
    },
    "lyric_line": 1,
    "lyric_cont": {
        "type": 0,
        "fname": "多個符號.abc",
        "istart": 2255,
        "dur": 0,
        "multi": 0,
        "iend": 2256,
        "bar_type": "|",
        "prev": "__voiceItem",
        "v": 0,
        "p_v": "__Tunes",
        "st": 0,
        "time": 23040,
        "fmt": "__Formation",
        "pos": {},
        "next": "__voiceItem",
        "seqst": true,
        "ts_prev": "__voiceItem",
        "ts_next": "__voiceItem",
        "bar_num": 24,
        "notes": [
            {
                "pit": 23
            }
        ],
        "nhd": 0,
        "mid": 12,
        "ymx": 24,
        "ymn": 0,
        "wl": 5,
        "wr": 7,
        "shrink": 13.95,
        "space": 31.200000000000003,
        "x": 740.357142857143
    },
    "key": {
        "type": 5,
        "dur": 0,
        "fname": "多個符號.abc",
        "istart": 1136,
        "iend": 1144,
        "k_sf": 1,
        "k_map": {
            "0": 0,
            "1": 0,
            "2": 0,
            "3": 1,
            "4": 0,
            "5": 0,
            "6": 0
        },
        "k_mode": 0,
        "k_b40": 25,
        "wl": 0,
        "wr": 8.5
    },
    "osym": {
        "type": 12,
        "fname": "多個符號.abc",
        "dur": 0,
        "v": 0,
        "p_v": "__Tunes",
        "time": 0,
        "st": 0,
        "sy": {
            "voices": [
                {
                    "range": 0,
                    "st": 0
                },
                {
                    "range": 1,
                    "st": 1
                },
                {
                    "range": 2,
                    "st": 2
                }
            ],
            "staves": [
                {
                    "stafflines": "|||||",
                    "staffscale": 1,
                    "flags": 68,
                    "staffnonote": 1
                },
                {
                    "stafflines": "|||||",
                    "staffscale": 1,
                    "flags": 64,
                    "staffnonote": 1
                },
                {
                    "stafflines": "|||||",
                    "staffscale": 1,
                    "flags": 8,
                    "staffnonote": 1
                }
            ],
            "top_voice": 0,
            "nstaff": 2,
            "st_print": {
                "0": 1,
                "1": 0,
                "2": 0
            }
        },
        "next": "__voiceItem",
        "fmt": "__Formation",
        "seqst": true,
        "ts_next": "__voiceItem",
        "notes": [
            {
                "pit": 16
            }
        ],
        "nhd": 0,
        "ymx": 24,
        "ymn": 0,
        "wr": 13.550000381469726,
        "wl": 11.3,
        "shrink": 16.3,
        "space": 0,
        "x": 16.3
    },
    "s_next": null,
    "bar_start": null,
    "s_prev": {
        "type": 0,
        "fname": "多個符號.abc",
        "istart": 2697,
        "dur": 0,
        "multi": 0,
        "iend": 2698,
        "bar_type": "|",
        "prev": "__voiceItem",
        "v": 0,
        "p_v": "__Tunes",
        "st": 0,
        "time": 38400,
        "fmt": "__Formation",
        "pos": {},
        "next": "__voiceItem",
        "seqst": true,
        "ts_prev": "__voiceItem",
        "ts_next": "__voiceItem",
        "bar_num": 44,
        "notes": [
            {
                "pit": 24
            }
        ],
        "nhd": 0,
        "mid": 12,
        "ymx": 24,
        "ymn": 0,
        "wl": 5,
        "wr": 7,
        "shrink": 10,
        "space": 41.460001373291014,
        "x": 740.3571428571431
    }
}




function asdf() {
    
}