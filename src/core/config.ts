namespace abc2svg{
    export var Abc = function (user) { };
    export var modules: any;
    export var mhooks: any[];
    export var sheet: any ;

    // global fonts
    export var font_tb = []	// fonts - index = font.fid
    export var font_st = {}	// font style => font_tb index for incomplete user fonts

    export var loadjs 
    export var printErr 
    export var C = {
        BLEN: 1536,

        // symbol types
        BAR: 0,
        CLEF: 1,
        CUSTOS: 2,
        SM: 3,		// sequence marker (transient)
        GRACE: 4,
        KEY: 5,
        METER: 6,
        MREST: 7,
        NOTE: 8,
        PART: 9,
        REST: 10,
        SPACE: 11,
        STAVES: 12,
        STBRK: 13,
        TEMPO: 14,
        BLOCK: 16,
        REMARK: 17,

        // note heads
        FULL: 0,
        EMPTY: 1,
        OVAL: 2,
        OVALBARS: 3,
        SQUARE: 4,

        // position types
        SL_ABOVE: 0x01,		// position (3 bits)
        SL_BELOW: 0x02,
        SL_AUTO: 0x03,
        SL_HIDDEN: 0x04,
        SL_DOTTED: 0x08,	// modifiers
        SL_ALI_MSK: 0x70,	// align
        SL_ALIGN: 0x10,
        SL_CENTER: 0x20,
        SL_CLOSE: 0x40
    };

    // !! tied to the symbol types in abc2svg.js !!
    export var sym_name = ['bar', 'clef', 'custos', 'smark', 'grace',
        'key', 'meter', 'Zrest', 'note', 'part',
        'rest', 'yspace', 'staves', 'Break', 'tempo',
        '', 'block', 'remark']

    // key table - index = number of accidentals + 7
    export var keys = [
        new Int8Array([-1, -1, -1, -1, -1, -1, -1]),	// 7 flat signs
        new Int8Array([-1, -1, -1, 0, -1, -1, -1]),	// 6 flat signs
        new Int8Array([0, -1, -1, 0, -1, -1, -1]),	// 5 flat signs
        new Int8Array([0, -1, -1, 0, 0, -1, -1]),	// 4 flat signs
        new Int8Array([0, 0, -1, 0, 0, -1, -1]),	// 3 flat signs
        new Int8Array([0, 0, -1, 0, 0, 0, -1]),	// 2 flat signs
        new Int8Array([0, 0, 0, 0, 0, 0, -1]),	// 1 flat signs
        new Int8Array([0, 0, 0, 0, 0, 0, 0]),	// no accidental
        new Int8Array([0, 0, 0, 1, 0, 0, 0]),	// 1 sharp signs
        new Int8Array([1, 0, 0, 1, 0, 0, 0]),	// 2 sharp signs
        new Int8Array([1, 0, 0, 1, 1, 0, 0]),	// 3 sharp signs
        new Int8Array([1, 1, 0, 1, 1, 0, 0]),	// 4 sharp signs
        new Int8Array([1, 1, 0, 1, 1, 1, 0]),	// 5 sharp signs
        new Int8Array([1, 1, 1, 1, 1, 1, 0]),	// 6 sharp signs
        new Int8Array([1, 1, 1, 1, 1, 1, 1])	// 7 sharp signs
    ]

    // base-40 representation of musical pitch
    // (http://www.ccarh.org/publications/reprints/base40/)
    export var p_b40 = new Int8Array(			// staff pitch to base-40
        //		  C  D   E   F   G   A   B
        [2, 8, 14, 19, 25, 31, 37])
    export var b40_p = new Int8Array(			// base-40 to staff pitch
        //		       C		 D
        [0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1,
            //	      E		     F		       G
            2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4,
            //	      A			B
            5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6])
    export var b40_a = new Int8Array(			// base-40 to accidental
        //		         C		      D
        [-2, -1, 0, 1, 2, -3, -2, -1, 0, 1, 2, -3,
        //		E		 F		      G
        -2, -1, 0, 1, 2, -2, -1, 0, 1, 2, -3, -2, -1, 0, 1, 2, -3,
        //		A		     B
        -2, -1, 0, 1, 2, -3, -2, -1, 0, 1, 2])
    export var b40_m = new Int8Array(			// base-40 to midi
        //			 C		   D
        [-2, -1, 0, 1, 2, 0, 0, 1, 2, 3, 4, 0,
            //	      E		     F		       G
            2, 3, 4, 5, 6, 3, 4, 5, 6, 7, 0, 5, 6, 7, 8, 9, 0,
            //	      A			    B
            7, 8, 9, 10, 11, 0, 9, 10, 11, 12, 13])
    export var b40Mc = new Int8Array(		// base-40 major chords
        //		        C		  D
        [36, 1, 2, 3, 8, 2, 2, 7, 8, 13, 14, 2,
            //		    37	   7	       3
            //	       E	      F		        G
            8, 13, 14, 19, 20, 13, 14, 19, 20, 25, 2, 19, 24, 25, 30, 31, 2,
            //				24	    20
            //	       A		 B
            25, 30, 31, 36, 37, 2, 31, 36, 37, 2, 3])
    //				 1
    export var b40mc = new Int8Array(		// base-40 minor chords
        //		        C		  D
        [36, 37, 2, 3, 8, 2, 2, 3, 8, 9, 14, 2,
            //					    13
            //	       E	      F		        G
            8, 13, 14, 19, 20, 13, 14, 19, 20, 25, 2, 19, 20, 25, 26, 31, 2,
            //	    9					  30
            //	       A		 B
            25, 30, 31, 32, 37, 2, 31, 36, 37, 2, 3])
    //	   26    36	     32
    export var b40sf = new Int8Array(		// base-40 interval to key signature
        //		        C		   D
        [-2, -7, 0, 7, 2, 88, 0, -5, 2, -3, 4, 88,
            //	       E	      F		        G
            2, -3, 4, -1, 6, -3, 4, -1, 6, 1, 88, -1, -6, 1, -4, 3, 88,
            //	       A		 B
            1, -4, 3, -2, 5, 88, 3, -2, 5, 0, 7])
    export var isb40 = new Int8Array(		// interval with sharp to base-40 interval
        [0, 1, 6, 7, 12, 17, 18, 23, 24, 29, 30, 35])

    export var pab40 = function (p, a) {
        p += 19				// staff pitch from C-1
        var b40 = ((p / 7) | 0) * 40 + p_b40[p % 7]
        if (a && a != 3)		// if some accidental, but not natural
            b40 += a
        return b40
    } // pit2b40()
    export var b40p = function (b) {
        return ((b / 40) | 0) * 7 + b40_p[b % 40] - 19
    } // b40p()
    export var b40a = function (b) {
        return b40_a[b % 40]
    } // b40a()
    export var b40m = function (b) {
        return ((b / 40) | 0) * 12 + b40_m[b % 40]
    } // b40m()

    // chord table
    // This table is used in various modules
    // to convert the types of chord symbols to a minimum set.
    // More chord types may be added by the command %%chordalias.
    export var ch_alias = {
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
    // reference:
    //	https://developer.mozilla.org/en-US/docs/Web/CSS/font-weight
    export var ft_w = {
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


    export var ft_re = new RegExp('\
-?Thin|-?Extra Light|-?Light|-?Regular|-?Medium|\
-?[DS]emi|-?[DS]emi[ -]?Bold|\
-?Bold|-?Extra[ -]?Bold|-?Ultra[ -]?Bold|-?Black|-?Heavy/',
        "i")

    export var style = '\
\n.stroke{stroke:currentColor;fill:none}\
\n.bW{stroke:currentColor;fill:none;stroke-width:1}\
\n.bthW{stroke:currentColor;fill:none;stroke-width:3}\
\n.slW{stroke:currentColor;fill:none;stroke-width:.7}\
\n.slthW{stroke:currentColor;fill:none;stroke-width:1.5}\
\n.sW{stroke:currentColor;fill:none;stroke-width:.7}\
\n.box{outline:1px solid black;outline-offset:1px}';
    // simplify a rational number n/d
    export var rat = function (n, d) {
        var a, t,
            n0 = 0,
            d1 = 0,
            n1 = 1,
            d0 = 1
        while (1) {
            if (d == 0)
                break
            t = d
            a = (n / d) | 0
            d = n % d
            n = t
            t = n0 + a * n1
            n0 = n1
            n1 = t
            t = d0 + a * d1
            d0 = d1
            d1 = t
        }
        return [n1, d1]
    } // rat()

    // compare pitches
    // This function is used to sort the note pitches
    export var pitcmp = function (n1, n2) { return n1.pit - n2.pit }

    //var abc2svg: any = {}
    //abc2svg = {
    //    C, sym_name, keys,
    //    p_b40,
    //    b40_p,
    //    b40_a,
    //    b40_m,
    //    b40Mc,
    //    b40mc,
    //    b40sf,
    //    isb40,
    //    pab40,
    //    b40p,
    //    b40a,
    //    b40m,
    //    ch_alias,
    //    font_tb,
    //    font_st,
    //    ft_w,
    //    ft_re,
    //    rat,
    //    pitcmp,
    //}
}   //** config **