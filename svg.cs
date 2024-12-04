using System;
using System.Collections.Generic;
using System.Text;
//var output = "",		// output buffer
//    style = '\
//\n.stroke{stroke:currentColor; fill: none}\'
//var font_style = '',
//    posx:number = cfmt.leftmargin / cfmt.scale,
//    posy: number = 0,
//    img: DrawImage = {
//width: cfmt.pagewidth,
//        lm: cfmt.leftmargin,
//        rm: cfmt.rightmargin,
//        chg: true
//    },
//    defined_glyph = { },
//    defs = '',
//    fulldefs = '',
//    stv_g:
//{
//scale: number,
//        dy: number,
//        st: number,
//        v: number,
//        g: number,
//        color ?: string,
//        started ?: boolean
//    } = {
//scale: 1,
//        dy: 0,
//        st: -1,
//        v: -1,
//        g: 0
//    },
//    blkdiv = 0
//var tgls = {
//    "mtr ": { x: 0, y: 0, c: "\u0020" },	// space
//    brace: { x: 0, y: 0, c: "\ue000" },
//    lphr: { x: 0, y: 23, c: "\ue030" },
//    mphr: { x: 0, y: 23, c: "\ue038" },

//    scclef: { x: -8, y: 0, c: "\ue07b" },
//    sbclef: { x: -7, y: 0, c: "\ue07c" },
//    oct: { x: 0, y: 2, c: "\ue07d" },		// 8 for clefs
//    oct2: { x: 0, y: 2, c: "\ue07e" },		// 15 for clefs
//    mtr0: { x: 0, y: 0, c: "\ue080" },		// meters
//    mtr1: { x: 0, y: 0, c: "\ue081" },
//    mtr2: { x: 0, y: 0, c: "\ue082" },
//    mtr3: { x: 0, y: 0, c: "\ue083" },

//    pfthd: { x: -3.7, y: 0, c: "\ue0b3" },
//    x: { x: -3.7, y: 0, c: "\ue0a9" },	  srep: { x: -5, y: 0, c: "\ue101" },
//    "dot+": { x: -5, y: 0, sc: .7, c: "\ue101" },
//    dot: { x: -1, y: 0, c: "\ue1e7" },
//    flu1: { x: -.3, y: 0, c: "\ue240" },	// flags
//    fld1: { x: -.3, y: 0, c: "\ue241" },
//    flu2: { x: -.3, y: 0, c: "\ue242" },

//    "acc-1": { x: -1, y: 0, c: "\ue260" },		// flat
//    "cacc-1": { x: -18, y: 0, c: "\ue26a\ue260\ue26b" }, // courtesy flat (note deco)
//    "sacc-1": { x: -1, y: 0, sc: .7, c: "\ue260" },	// small flat (editorial)
//    acc3: { x: -1, y: 0, c: "\ue261" },		// natural
//    "cacc3": { x: -18, y: 0, c: "\ue26a\ue261\ue26b" },	// courtesy natural (note deco)
//    sacc3: { x: -1, y: 0, sc: .7, c: "\ue261" },	// small natural (editorial)
//    acc1: { x: -2, y: 0, c: "\ue262" },		// sharp
//    "cacc1": { x: -18, y: 0, c: "\ue26a\ue262\ue26b" },	// courtesy sharp (note deco)
//    sacc1: { x: -2, y: 0, sc: .7, c: "\ue262" },	// small sharp (editorial)
//    acc2: { x: -3, y: 0, c: "\ue263" },	// double sharp
//    "acc-2": { x: -3, y: 0, c: "\ue264" },	// double flat
//    "acc-1_2": { x: -2, y: 0, c: "\ue280" },	// quarter-tone flat
//    "acc-3_2": { x: -3, y: 0, c: "\ue281" },	// three-quarter-tones flat
//    acc1_2: { x: -1, y: 0, c: "\ue282" },	// quarter-tone sharp
//    acc3_2: { x: -3, y: 0, c: "\ue283" },	// three-quarter-tones sharp


//}
//var glyphs = { }l
//var anno_start = empty_function;
//var anno_stop = empty_function,
//var gla: any = [[], [], "", [], [], []];

//var deco_str_style = {
//    crdc: {             // cresc., decresc., dim., ...
//        dx: 0,
//        dy: 5,
//        style: 'font:italic 14px text,serif',
//        anchor: ' text-anchor="middle"'
//    },
//    dacs:{               // long repeats (da capo, fine...)
//        dx: 0,
//        dy: 3,
//        style: 'font:bold 15px text,serif',
//        anchor: ' text-anchor="middle"'
//    },
//    pf: { 
//        dx: 0,
//        dy: 5,
//        style: 'font:italic bold 16px text,serif',
//        anchor: ' text-anchor="middle"'
//    }
//}

// abc2svg - svg.js - svg functions
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
    public class Abc2
    {
        string output = "";     // output buffer
        string style = @"
        \n.stroke{stroke:currentColor;fill:none}
        \n.bW{stroke:currentColor;fill:none;stroke-width:1}
        \n.bthW{stroke:currentColor;fill:none;stroke-width:3}
        \n.slW{stroke:currentColor;fill:none;stroke-width:.7}
        \n.slthW{stroke:currentColor;fill:none;stroke-width:1.5}
        \n.sW{stroke:currentColor;fill:none;stroke-width:.7}
        \n.box{outline:1px solid black;outline-offset:1px}";


        string font_style = "";
        double posx = cfmt.leftmargin / cfmt.scale;	// default x offset of the images
        double posy = 0;        // y offset in the block
        DrawImage img = new DrawImage()
        {                             // image
            width = cfmt.pagewidth,   // width
            lm = cfmt.leftmargin,     // left and right margins
            rm = cfmt.rightmargin,
            chg = true  //true
        };
        object defined_glyph = new { },
        defs = ' ',
        fulldefs = ' ';     	// unreferenced defs as <filter>
        object stv_g = new
        {   /* staff/voice graphic parameters */
            scale = 1,
            dy = 0,
            st = -1,
            v = -1,
            g = 0,
            //color = null,
            started = false
        };
        int blkdiv = 0; // block of contiguous SVGs
                        // -1: block started
                        //  0: no block
                        //  1: start a block
                        //  2: start a new page

        // glyphs in music font
        Dictionary<string, GlyphsMFont> tgls = new Dictionary<string, GlyphsMFont>
        {
            { "mtr",new GlyphsMFont {x= 0, y= 0, c= "\u0020" } },	// space
    {"brace", new GlyphsMFont{x= 0, y= 0, c= "\ue000" }},
    {"lphr", new GlyphsMFont{x= 0, y= 23, c= "\ue030" }},
    {"mphr", new GlyphsMFont{x= 0, y= 23, c= "\ue038" }},
    {"sphr", new GlyphsMFont{x= 0, y= 26, c= "\ue039" }},
    {"short", new GlyphsMFont{x= 0, y= 32, c= "\ue038" }},
    {"tick", new GlyphsMFont{x= 0, y= 29, c= "\ue039" }},
    {"rdots", new GlyphsMFont{x= -1, y= 0, c= "\ue043" }},	// repeat dots
    {"dsgn", new GlyphsMFont{x= -12, y= 0, c= "\ue045" }},	// D.S.
    {"dcap", new GlyphsMFont{x= -12, y= 0, c= "\ue046" }},	// D.C.
    {"sgno", new GlyphsMFont{x= -5, y= 0, c= "\ue047" }},	// segno
    {"coda", new GlyphsMFont{x= -10, y= 0, c= "\ue048" }},
    {"tclef", new GlyphsMFont{x= -8, y= 0, c= "\ue050" }},
    {"cclef", new GlyphsMFont{x= -8, y= 0, c= "\ue05c" }},
    {"bclef", new GlyphsMFont{x= -8, y= 0, c= "\ue062" }},
    {"pclef", new GlyphsMFont{x= -6, y= 0, c= "\ue069" }},
    {"spclef", new GlyphsMFont{x= -6, y= 0, c= "\ue069" }},
    {"stclef", new GlyphsMFont{x= -8, y= 0, c= "\ue07a" }},
    {"scclef", new GlyphsMFont{x= -8, y= 0, c= "\ue07b" }},
    {"sbclef", new GlyphsMFont{x= -7, y= 0, c= "\ue07c" }},
    {"oct", new GlyphsMFont{x= 0, y= 2, c= "\ue07d" }},		// 8 for clefs
    {"oct2", new GlyphsMFont{x= 0, y= 2, c= "\ue07e" }},		// 15 for clefs
    {"mtr0", new GlyphsMFont{x= 0, y= 0, c= "\ue080" }},		// meters
    {"mtr1", new GlyphsMFont{x= 0, y= 0, c= "\ue081" }},
    {"mtr2", new GlyphsMFont{x= 0, y= 0, c= "\ue082" }},
    {"mtr3", new GlyphsMFont{x= 0, y= 0, c= "\ue083" }},
    {"mtr4", new GlyphsMFont{x= 0, y= 0, c= "\ue084" }},
    {"mtr5", new GlyphsMFont{x= 0, y= 0, c= "\ue085" }},
    {"mtr6", new GlyphsMFont{x= 0, y= 0, c= "\ue086" }},
    {"mtr7", new GlyphsMFont{x= 0, y= 0, c= "\ue087" }},
    {"mtr8", new GlyphsMFont{x= 0, y= 0, c= "\ue088" }},
    {"mtr9", new GlyphsMFont{x= 0, y= 0, c= "\ue089" }},
    {"mtrC", new GlyphsMFont{x= 0, y= 0, c= "\ue08a" }},		// common time (4/4)
    //  {"mtrC|"= {x=0, y=0, c="\ue08b"}},	// cut time (2/2) (unused)
    {"mtr+", new GlyphsMFont{x= 0, y= 0, c= "\ue08c" }},
    {"mtr(", new GlyphsMFont{x= 0, y= 0, c= "\ue094" }},
    {"mtr)", new GlyphsMFont{x= 0, y= 0, c= "\ue095" }},
    {"HDD", new GlyphsMFont{x= -7, y= 0, c= "\ue0a0" }},
    {"breve", new GlyphsMFont{x= -7, y= 0, c= "\ue0a1" }},
    {"HD", new GlyphsMFont{x= -5.2, y= 0, c= "\ue0a2" }},
    {"Hd", new GlyphsMFont{x= -3.8, y= 0, c= "\ue0a3" }},
    {"hd", new GlyphsMFont{x= -3.7, y= 0, c= "\ue0a4" }},
    {"ghd", new GlyphsMFont{x= 2, y= 0, c= "\ue0a4", sc= .66 }},	// grace note head
    {"pshhd", new GlyphsMFont{x= -3.7, y= 0, c= "\ue0a9" }},
    {"pfthd", new GlyphsMFont{x= -3.7, y= 0, c= "\ue0b3" }},
    {"x", new GlyphsMFont{x= -3.7, y= 0, c= "\ue0a9" }},		// 'x' note head
    {"circle-x", new GlyphsMFont{x= -3.7, y= 0, c= "\ue0b3" }}, // 'circle-x' note head
    {"srep", new GlyphsMFont{x= -5, y= 0, c= "\ue101" }},
    {"dot+", new GlyphsMFont{x= -5, y= 0, sc= .7, c= "\ue101" }},
    {"diamond", new GlyphsMFont{x= -4, y= 0, c= "\ue1b9" }},
    {"triangle", new GlyphsMFont{x= -4, y= 0, c= "\ue1bb" }},
    {"dot", new GlyphsMFont{x= -1, y= 0, c= "\ue1e7" }},
    {"flu1", new GlyphsMFont{x= -.3, y= 0, c= "\ue240" }},	// flags
    {"fld1", new GlyphsMFont{x= -.3, y= 0, c= "\ue241" }},
    {"flu2", new GlyphsMFont{x= -.3, y= 0, c= "\ue242" }},
    {"fld2", new GlyphsMFont{x= -.3, y= 0, c= "\ue243" }},
    {"flu3", new GlyphsMFont{x= -.3, y= 3.5, c= "\ue244" }},
    {"fld3", new GlyphsMFont{x= -.3, y= -4, c= "\ue245" }},
    {"flu4", new GlyphsMFont{x= -.3, y= 8, c= "\ue246" }},
    {"fld4", new GlyphsMFont{x= -.3, y= -9, c= "\ue247" }},
    {"flu5", new GlyphsMFont{x= -.3, y= 12.5, c= "\ue248" }},
    {"fld5", new GlyphsMFont{x= -.3, y= -14, c= "\ue249" }},
    {"acc-1", new GlyphsMFont{x= -1, y= 0, c= "\ue260" }},		// flat
    {"cacc-1", new GlyphsMFont{x= -18, y= 0, c= "\ue26a\ue260\ue26b" }}, // courtesy flat (note deco)
    {"sacc-1", new GlyphsMFont{x= -1, y= 0, sc= .7, c= "\ue260" }},	// small flat (editorial)
    {"acc3", new GlyphsMFont{x= -1, y= 0, c= "\ue261" }},		// natural
    {"cacc3", new GlyphsMFont{x= -18, y= 0, c= "\ue26a\ue261\ue26b" }},	// courtesy natural (note deco)
    {"sacc3", new GlyphsMFont{x= -1, y= 0, sc= .7, c= "\ue261" }},	// small natural (editorial)
    {"acc1", new GlyphsMFont{x= -2, y= 0, c= "\ue262" }},		// sharp
    {"cacc1", new GlyphsMFont{x= -18, y= 0, c= "\ue26a\ue262\ue26b" }},	// courtesy sharp (note deco)
    {"sacc1", new GlyphsMFont{x= -2, y= 0, sc= .7, c= "\ue262" }},	// small sharp (editorial)
    {"acc2", new GlyphsMFont{x= -3, y= 0, c= "\ue263" }},	// double sharp
    {"acc-2", new GlyphsMFont{x= -3, y= 0, c= "\ue264" }},	// double flat
    {"acc-1_2", new GlyphsMFont{x= -2, y= 0, c= "\ue280" }},	// quarter-tone flat
    {"acc-3_2", new GlyphsMFont{x= -3, y= 0, c= "\ue281" }},	// three-quarter-tones flat
    {"acc1_2", new GlyphsMFont{x= -1, y= 0, c= "\ue282" }},	// quarter-tone sharp
    {"acc3_2", new GlyphsMFont{x= -3, y= 0, c= "\ue283" }},	// three-quarter-tones sharp
    {"accent", new GlyphsMFont{x= -3, y= 2, c= "\ue4a0" }},
    {"stc", new GlyphsMFont{x= 0, y= -2, c= "\ue4a2" }},		// staccato
    {"emb", new GlyphsMFont{x= 0, y= -2, c= "\ue4a4" }},
    {"wedge", new GlyphsMFont{x= 0, y= 0, c= "\ue4a8" }},
    {"marcato", new GlyphsMFont{x= -3, y= -2, c= "\ue4ac" }},
    {"hld", new GlyphsMFont{x= -7, y= -2, c= "\ue4c0" }},		// fermata
    {"brth", new GlyphsMFont{x= 0, y= 0, c= "\ue4ce" }},
    {"caes", new GlyphsMFont{x= 0, y= 8, c= "\ue4d1" }},
    {"r00", new GlyphsMFont{x= -1.5, y= 0, c= "\ue4e1" }},
    {"r0", new GlyphsMFont{x= -1.5, y= 0, c= "\ue4e2" }},
    {"r1", new GlyphsMFont{x= -3.5, y= -6, c= "\ue4e3" }},
    {"r2", new GlyphsMFont{x= -3.2, y= 0, c= "\ue4e4" }},
    {"r4", new GlyphsMFont{x= -3, y= 0, c= "\ue4e5" }},
    {"r8", new GlyphsMFont{x= -3, y= 0, c= "\ue4e6" }},
    {"r16", new GlyphsMFont{x= -4, y= 0, c= "\ue4e7" }},
    {"r32", new GlyphsMFont{x= -4, y= 0, c= "\ue4e8" }},
    {"r64", new GlyphsMFont{x= -4, y= 0, c= "\ue4e9" }},
    {"r128", new GlyphsMFont{x= -4, y= 0, c= "\ue4ea" }},
    // { mrest= {x=-10, y=0, c="\ue4ee"}},
    {"mrep", new GlyphsMFont{x= -6, y= 0, c= "\ue500" }},
    {"mrep2", new GlyphsMFont{x= -9, y= 0, c= "\ue501" }},
    {"p", new GlyphsMFont{x= -3, y= 0, c= "\ue520" }},
    {"f", new GlyphsMFont{x= -3, y= 0, c= "\ue522" }},
    {"pppp", new GlyphsMFont{x= -15, y= 0, c= "\ue529" }},
    {"ppp", new GlyphsMFont{x= -14, y= 0, c= "\ue52a" }},
    {"pp", new GlyphsMFont{x= -8, y= 0, c= "\ue52b" }},
    {"mp", new GlyphsMFont{x= -8, y= 0, c= "\ue52c" }},
    {"mf", new GlyphsMFont{x= -8, y= 0, c= "\ue52d" }},
    {"ff", new GlyphsMFont{x= -7, y= 0, c= "\ue52f" }},
    {"fff", new GlyphsMFont{x= -10, y= 0, c= "\ue530" }},
    {"ffff", new GlyphsMFont{x= -14, y= 0, c= "\ue531" }},
    {"sfz", new GlyphsMFont{x= -10, y= 0, c= "\ue539" }},
    {"trl", new GlyphsMFont{x= -5, y= -2, c= "\ue566" }},	// trill
    {"turn", new GlyphsMFont{x= -5, y= 0, c= "\ue567" }},
    {"turnx", new GlyphsMFont{x= -5, y= 0, c= "\ue569" }},
    {"umrd", new GlyphsMFont{x= -6, y= 2, c= "\ue56c" }},
    {"lmrd", new GlyphsMFont{x= -6, y= 2, c= "\ue56d" }},
    {"dplus", new GlyphsMFont{x= -3, y= 0, c= "\ue582" }},	// plus
    {"sld", new GlyphsMFont{x= -8, y= 12, c= "\ue5d0" }},	// slide
    {"grm", new GlyphsMFont{x= -3, y= -2, c= "\ue5e2" }},	// grace mark
    {"dnb", new GlyphsMFont{x= -3, y= 0, c= "\ue610" }},		// down bow
    {"upb", new GlyphsMFont{x= -2, y= 0, c= "\ue612" }},		// up bow
    {"opend", new GlyphsMFont{x= -2, y= -2, c= "\ue614" }},	// harmonic
    {"roll", new GlyphsMFont{x= 0, y= 0, c= "\ue618" }},
    {"thumb", new GlyphsMFont{x= -2, y= -2, c= "\ue624" }},
    {"snap", new GlyphsMFont{x= -2, y= -2, c= "\ue630" }},
    {"ped", new GlyphsMFont{x= -10, y= 0, c= "\ue650" }},
    {"pedoff", new GlyphsMFont{x= -5, y= 0, c= "\ue655" }},
    // "mtro."= {x=0, y=0, c="\ue910"}},	// (unused)
    {"mtro", new GlyphsMFont{x= 0, y= 0, c= "\ue911" }},		// tempus perfectum
    // "mtro|"= {x=0, y=0, c="\ue912"}},	// (unused)
    // "mtrc."= {x=0, y=0, c="\ue914"}},	// (unused)
    {"mtrc", new GlyphsMFont{x= 0, y= 0, c= "\ue915" }},	// tempus imperfectum
    // "mtrc|"= {x=0, y=0, c="\ue918"}},	// (unused)
    {"mtr.", new GlyphsMFont{x= 0, y= 0, c= "\ue920" }},	// prolatione perfecta
    {"mtr|", new GlyphsMFont{x= 0, y= 0, c= "\ue925" }},	// (twice as fast)
    {"longa", new GlyphsMFont{x= -4.7, y= 0, c= "\ue95d" }},
    {"custos", new GlyphsMFont{x= -4, y= 3, c= "\uea02" }},
    {"ltr", new GlyphsMFont{x= 2, y= 6, c= "\ueaa4" }		// long trill element
};
        var glyphs = { };
        Action<object, object, object, object> anno_start= empty_function;
        Action<object, object, object, object> anno_stop = empty_function;
        var gla = new object[] { new List<object>(), new List<object>(), "", new List<object>(), new List<object>(), new List<object>() };

        // decorations with string
        Dictionary<string,Crdc> deco_str_style = new Dictionary<string, Crdc>
        {
            crdc = new
            {
                dx = 0,
                dy = 5,
                style = "font:ttt",
                anchor = " t=\"middle\""
            },
            dacs = new
            {
                dx = 0,
                dy = 3,
                style = "font:bold 15px text,serif",
                anchor = " text-anchor=\"middle\""
            },
            pf = new
            {
                dx = 0,
                dy = 5,
                style = "f",
                anchor = "t\""
            }
        };


        string m_gl(string str)
        {
            return str.Replace(/[Cco]\||[co]\.|./ g,
                (e) =>
                {
                    var m = tgls["mtr" + e];
                    return m != null ? m.c : 0;
                });
        }

        void def_use(string gl)
        {
            if (defined_glyph.ContainsKey(gl))
                return;
            defined_glyph[gl] = true;
            var g = glyphs[gl];
            if (g == null)
            {
                error(1, null, "Unknown glyph: '$1'", gl);
                return;
            }
            var j = 0;
            while (true)
            {
                var i = g.IndexOf('xlink:href="#', j);
                if (i < 0)
                    break;
                i += 13;
                j = g.IndexOf('"', i);
                def_use(g.Substring(i, j));
            }
            defs += '\n' + g;
        }

        void defs_add(string text)
        {
            var i = 0;
            var j = 0;
            string gl;
            string tag;
            int is_;
            int ie = 0;

            text = text.Replace(/< !--.*? -->/ g, '');

            while (true)
            {
                is_ = text.IndexOf('<', ie);
                if (is_ < 0)
                    break;
                i = text.IndexOf('id="', is_);
                if (i < 0)
                    break;
                i += 4;
                j = text.IndexOf('"', i);
                if (j < 0)
                    break;
                gl = text.Substring(i, j);
                ie = text.IndexOf('>', j);
                if (ie < 0)
                    break;
                if (text[ie - 1] == '/')
                {
                    ie++;
                }
                else
                {
                    i = text.IndexOf(' ', is_);
                    if (i < 0)
                        break;
                    tag = text.Substring(is_ + 1, i);
                    ie = text.IndexOf('</' + tag + '>', ie);
                    if (ie < 0)
                        break;
                    ie += 3 + tag.Length;
                }
                if (text.Substring(is_, 7) == '<filter')
                    fulldefs += text.Substring(is_, ie) + '\n';
                else
                    glyphs[gl] = text.Substring(is_, ie);
            }
        }

        void set_g()
        {
            if (stv_g.started)
            {
                stv_g.started = false;
                glout();
                output += "</g>\n";
            }
            if (stv_g.scale == 1 && string.IsNullOrEmpty(stv_g.color))
                return;
            glout();
            output += '<g ';
            if (stv_g.scale != 1)
            {
                if (stv_g.st < 0)
                    output += voice_tb[stv_g.v].scale_str;
                else if (stv_g.v < 0)
                    output += staff_tb[stv_g.st].scale_str;
                else
                    output += 'transform="translate(0,' +
                        (posy - stv_g.dy).ToString("F1") +
                        ') scale(' + stv_g.scale.ToString("F2") + ')"';
            }
            if (!string.IsNullOrEmpty(stv_g.color))
            {
                if (stv_g.scale != 1)
                    output += ' ';
                output += 'color="' + stv_g.color + '"';
            }
            output += ">\n";
            stv_g.started = true;
        }
        string set_color(string color = null)
        {
            if (color == stv_g.color)
                return null;
            var old_color = stv_g.color;
            stv_g.color = color;
            set_g();
            return old_color;
        }
        void set_sscale(int st)
        {
            double new_scale, dy;

            if (st != stv_g.st && stv_g.scale != 1)
                stv_g.scale = 0;
            new_scale = st >= 0 ? staff_tb[st].staffscale : 1;
            if (st >= 0 && new_scale != 1)
                dy = staff_tb[st].y;
            else
                dy = posy;
            if (new_scale == stv_g.scale && dy == stv_g.dy)
                return;
            stv_g.scale = new_scale;
            stv_g.dy = dy;
            stv_g.st = st;
            stv_g.v = -1;
            set_g();
        }
        void set_scale(Voice s)
        {
            double new_dy;
            double new_scale = s.p_v.scale;

            if (new_scale == 1)
            {
                set_sscale(s.st);
                return;
            }
            new_dy = posy;
            if (staff_tb[s.st].staffscale != 1)
            {
                new_scale *= staff_tb[s.st].staffscale;
                new_dy = staff_tb[s.st].y;
            }
            if (new_scale == stv_g.scale && stv_g.dy == posy)
                return;
            stv_g.scale = new_scale;
            stv_g.dy = new_dy;
            stv_g.st = staff_tb[s.st].staffscale == 1 ? -1 : s.st;
            stv_g.v = s.v;
            set_g();
        }
        void set_dscale(int st, bool no_scale = false)
        {
            if (!string.IsNullOrEmpty(output))
            {
                if (stv_g.started)
                {
                    stv_g.started = false;
                    glout();
                    output += "</g>\n";
                }
                if (stv_g.st < 0)
                {
                    staff_tb[0].output += output;
                }
                else if (stv_g.scale == 1)
                {
                    staff_tb[stv_g.st].output += output;
                }
                else
                {
                    staff_tb[stv_g.st].sc_out += output;
                }
                output = "";
            }
            if (st < 0)
                stv_g.scale = 1;
            else
                stv_g.scale = no_scale ? 1 : staff_tb[st].staffscale;
            stv_g.st = st;
            stv_g.dy = 0;
        }
        void delayed_update()
        {
            int st;
            string new_out;
            string text;

            for (st = 0; st <= nstaff; st++)
            {
                if (!string.IsNullOrEmpty(staff_tb[st].sc_out))
                {
                    output += '<g ' + staff_tb[st].scale_str + '>\n' +
                        staff_tb[st].sc_out + '</g>\n';
                    staff_tb[st].sc_out = "";
                }
                if (string.IsNullOrEmpty(staff_tb[st].output))
                    continue;
                output += '<g transform="translate(0,' +
                    (-staff_tb[st].y).ToString("F1") +
                    ')">\n' +
                    staff_tb[st].output +
                    '</g>\n';
                staff_tb[st].output = "";
            }
        }
        void anno_out(Voice s, string t, Action<string, int, int, double, double, double, double, Voice> f)
        {
            if (s.istart == null)
                return;
            var type = s.type;
            var h = s.ymx - s.ymn + 4;
            var wl = s.wl ?? 2;
            var wr = s.wr ?? 2;

            if (s.grace)
                type = C.GRACE;

            f(t ?? abc2svg.sym_name[type], s.istart, s.iend,
                s.x - wl - 2, staff_tb[s.st].y + s.ymn + h - 2,
                wl + wr + 4, h, s);
        }
        void a_start(Voice s, string t)
        {
            anno_out(s, t, user.anno_start);
        }
        void a_stop(Voice s, string t)
        {
            anno_out(s, t, user.anno_stop);
        }
        static void empty_function(object a = null, object b = null, object c = null, object d = null)
        {
        }

        void anno_put()
        {
            Voice s;
            while (true)
            {
                s = anno_a.shift();
                if (s == null)
                    break;
                switch (s.type)
                {
                    case C.CLEF:
                    case C.METER:
                    case C.KEY:
                    case C.REST:
                        if (s.type != C.REST || s.rep_nb)
                        {
                            set_sscale(s.st);
                            break;
                        }
                    // fall thru
                    case C.GRACE:
                    case C.NOTE:
                    case C.MREST:
                        set_scale(s);
                        break;
                        //		default:
                        //			continue
                }
                anno_stop(s);
            }
        }

        void out_XYAB(string str, double x, double y, object a = null, object b = null)
        {
            x = sx(x);
            y = sy(y);
            output += str.Replace('X', x.ToString("F1"))
                .Replace('Y', y.ToString("F1"))
                .Replace('A', a?.ToString())
                .Replace('B', b?.ToString());
        }

        void g_open(double x, double y, double rot, double? sx = null, double? sy = null)
        {
            glout();
            out_XYAB('<g transform="translate(X,Y', x, y);
            if (rot != 0)
                output += ') rotate(' + rot.ToString("F2");
            if (sx != null)
            {
                if (sy != null)
                    output += ') scale(' + sx.Value.ToString("F2") +
                        ', ' + sy.Value.ToString("F2");
                else
                    output += ') scale(' + sx.Value.ToString("F2");
            }
            output += ')">\n';
            stv_g.g++;
        }
        void g_close()
        {
            glout();
            stv_g.g--;
            output += '</g>\n';
        }

        void out_svg(string str) { output += str; }

        double sx(double x)
        {
            if (stv_g.g > 0)
                return x;
            return (x + posx) / stv_g.scale;
        }
        double sy(double y)
        {
            if (stv_g.g > 0)
                return -y;
            if (stv_g.scale == 1)
                return posy - y;
            if (stv_g.v >= 0)
                return (stv_g.dy - y) / voice_tb[stv_g.v].scale;
            return stv_g.dy - y;    // staff scale only
        }
        double sh(double h)
        {
            if (stv_g.st < 0)
                return h / stv_g.scale;
            return h;
        }
        double ax(double x) { return x + posx; }
        double ay(double y)
        {
            if (stv_g.st < 0)
                return posy - y;
            return posy + (stv_g.dy - y) * stv_g.scale - stv_g.dy;
        }
        double ah(double h)
        {
            if (stv_g.st < 0)
                return h;
            return h * stv_g.scale;
        }
        void out_sxsy(double x, string sep, double y)
        {
            x = sx(x);
            y = sy(y);
            output += x.ToString("F1") + sep + y.ToString("F1");
        }
        void xypath(double x, double y, bool fill = false)
        {
            if (fill)
                out_XYAB('<path d="mX Y', x, y);
            else
                out_XYAB('<path class="stroke" d="mX Y', x, y);
        }
        void draw_all_hl()
        {
            int st, p_st;

            void hlud(object[][] hla, double d)
            {
                object[] hl;
                object[] hll;
                int i;
                double xp;
                double dx2;
                double x2;
                int n = hla.Length;

                if (n == 0)
                    return;
                for (i = 0; i < n; i++)
                {   // for all lines
                    hll = hla[i];
                    if (hll == null || hll.Length == 0)
                        continue;
                    xp = sx((double)hll[0][0]); // previous x
                    output +=
                        '<path class="stroke" stroke-width="1" d="M' +
                        xp.ToString("F1") + ' ' +
                        sy(p_st.y + d * i).ToString("F1");
                    dx2 = 0;
                    while (true)
                    {
                        hl = hll.shift();
                        if (hl == null)
                            break;
                        x2 = sx((double)hl[0]);
                        output += 'm' +
                            (x2 - xp + (double)hl[1] - dx2).ToString("F2") +
                            ' 0h' + (-((double)hl[1] - (double)hl[2])).ToString("F2");
                        xp = x2;
                        dx2 = (double)hl[2];
                    }
                    output += '"/>\n';
                }
            } // hlud()

            for (st = 0; st <= nstaff; st++)
            {
                p_st = staff_tb[st];
                if (p_st.hlu == null)
                    continue;   // (staff not yet displayed)
                set_sscale(st);
                hlud(p_st.hlu, 6);
                hlud(p_st.hld, -6);
            }
        }
        void glout()
        {
            object e;
            var v = new List<string>();

            // glyphs (notes, accidentals...)
            if (gla[0].Length > 0)
            {
                while (true)
                {
                    e = gla[0].shift();
                    if (e == null)
                        break;
                    v.Add(((double)e).ToString("F1"));
                }
                output += '<text x="' + string.Join(",", v);

                v.Clear();
                while (true)
                {
                    e = gla[1].shift();
                    if (e == null)
                        break;
                    v.Add(((double)e).ToString("F1"));
                }
                output += '"\ny="' + string.Join(",", v);

                output += '"\n>' + gla[2] + '</text>\n';
                gla[2] = "";
            }

            // stems
            if (gla[3].Length == 0)
                return;
            output += '<path class="sW" d="';
            while (true)
            {
                e = gla[3].shift();
                if (e == null)
                    break;
                output += 'M' + ((double)e).ToString("F1") +
                    ' ' + ((double)gla[3].shift()).ToString("F1") +
                    'v' + ((double)gla[3].shift()).ToString("F1");
            }
            output += '"/>\n';
        }
        void xygl(double x, double y, string gl)
        {
            if (glyphs.ContainsKey(gl))
            {
                def_use(gl);
                out_XYAB('<use x="X" y="Y" xlink:href="#A"/>\n', x, y, gl);
            }
            else
            {
                var tgl = tgls[gl];
                if (tgl != null)
                {
                    x += tgl.x * stv_g.scale;
                    y -= tgl.y;
                    if (tgl.sc != null)
                    {
                        out_XYAB('<text transform="translate(X,Y) scale(A)">B</text>\n',
                            x, y, tgl.sc, tgl.c);
                    }
                    else
                    {
                        gla[0].push(sx(x));
                        gla[1].push(sy(y));
                        gla[2] += tgl.c;
                    }
                }
                else
                {
                    error(1, null, 'no definition of $1', gl);
                }
            }
        }
        void out_acciac(double x, double y, double dx, double dy, bool up)
        {
            if (up)
            {
                x -= 1;
                y += 4;
            }
            else
            {
                x -= 5;
                y -= 4;
            }
            out_XYAB('<path class="stroke" d="mX YlF G"/>\n',
                x, y, dx, -dy);
        }
        void out_brace(double x, double y, double h)
        {
            x += posx - 6;
            y = posy - y;
            h /= 24;
            output += '<text transform="translate(' +
                x.ToString("F1") + ',' + y.ToString("F1") +
                ') scale(2.5,' + h.ToString("F2") +
                ')">' + tgls.brace.c + '</text>\n';
        }
        void out_bracket(double x, double y, double h)
        {
            x += posx - 5;
            y = posy - y - 3;
            h += 2;
            output += '<path d="m' + x.ToString("F1") + ' ' + y.ToString("F1") + '\n\
        
    c10.5 1 12 - 4.5 12 - 3.5c0 1 - 3.5 5.5 - 8.5 5.5\n\
	v' + h.ToString("F1") + '\n\
	c5 0 8.5 4.5 8.5 5.5c0 1 - 1.5 - 4.5 - 12 - 3.5"/>\n';
        }
        void out_hyph(double x, double y, double w)
        {
            int n;
            double a_y;
            double d = 25 + ((w / 20) | 0) * 3;

            if (w > 15.)
                n = ((w - 15) / d) | 0;
            else
                n = 0;
            x += (w - d * n - 5) / 2;
            out_XYAB('<path class="stroke" stroke-width="1.2"\n\
        

            stroke - dasharray = "5,A"\n\


            d = "mX YhB" />\n',
        
                x, y + 4,       // set the line a bit upper
                Math.Round((d - 5) / stv_g.scale), d * n + 5);
        }

        void out_stem(double x, double y, double h, bool grace = false, int nflags = 0, bool straight = false)
        {   // optional
            double dx = grace ? GSTEM_XOFF : 3.5;
            double slen = -h;

            if (h < 0)
                dx = -dx;       // down
            x += dx * stv_g.scale;
            if (stv_g.v >= 0)
                slen /= voice_tb[stv_g.v].scale;
            gla[3].push(sx(x));
            gla[3].push(sy(y));
            gla[3].push(slen);
            if (nflags == 0)
                return;

            y += h;
            if (h > 0)
            {               // up
                if (!straight)
                {
                    if (!grace)
                    {
                        xygl(x, y, "flu" + nflags);
                        return;
                    }
                    else
                    {       // grace
                        output += '<path d="';
                        if (nflags == 1)
                        {
                            out_XYAB('MX Yc0.6 3.4 5.6 3.8 3 10\n\
        

            1.2 - 4.4 - 1.4 - 7 - 3 - 7\n', x, y);
                        }
                        else
                        {
                            while (--nflags >= 0)
                            {
                                out_XYAB('MX Yc1 3.2 5.6 2.8 3.2 8\n\
        

            1.4 - 4.8 - 2.4 - 5.4 - 3.2 - 5.2\n', x, y);
        
                                y -= 3.5;
                            }
                        }
                    }
                }
                else
                {           // straight
                    output += '<path d="';
                    if (!grace)
                    {
                        while (--nflags >= 0)
                        {
                            out_XYAB('MX Yl7 3.2 0 3.2 -7 -3.2z\n',
                                x, y);
                            y -= 5.4;
                        }
                    }
                    else
                    {       // grace
                        while (--nflags >= 0)
                        {
                            out_XYAB('MX Yl3 1.5 0 2 -3 -1.5z\n',
                                x, y);
                            y -= 3;
                        }
                    }
                }
            }
            else
            {               // down
                if (!straight)
                {
                    if (!grace)
                    {
                        xygl(x, y, "fld" + nflags);
                        return;
                    }
                    else
                    {       // grace
                        output += '<path d="';
                        if (nflags == 1)
                        {
                            out_XYAB('MX Yc0.6 -3.4 5.6 -3.8 3 -10\n\
        

            1.2 4.4 - 1.4 7 - 3 7\n', x, y);
                        }
                        else
                        {
                            while (--nflags >= 0)
                            {
                                out_XYAB('MX Yc1 -3.2 5.6 -2.8 3.2 -8\n\
        

            1.4 4.8 - 2.4 5.4 - 3.2 5.2\n', x, y);
        
                                y += 3.5;
                            }
                        }
                    }
                }
                else
                {           // straight
                    output += '<path d="';
                    if (!grace)
                    {
                        while (--nflags >= 0)
                        {
                            out_XYAB('MX Yl7 -3.2 0 -3.2 -7 3.2z\n',
                                x, y);
                            y += 5.4;
                        }
                        //			} else {		// grace
                        //--fixme: error?
                    }
                }
            }
            output += '"/>\n';
        }


        /*********************************************/

//        string output = "";        // output buffer
//        string style = "{stroke:currentColor; fill: none}";
//        string font_style = "";
//        double posx = cfmt.leftmargin / cfmt.scale;
//        double posy = 0;
//        DrawImage img = new DrawImage
//        {
//            width = cfmt.pagewidth,
//            lm = cfmt.leftmargin,
//            rm = cfmt.rightmargin,
//            chg = true
//        };
//        Dictionary<string, object> defined_glyph = new Dictionary<string, object>();
//        string defs = "";
//        string fulldefs = "";
//        Dictionary<string, object> stv_g = new Dictionary<string, object>
//{
//    { "scale", 1.0 },
//    { "dy", 0.0 },
//    { "st", -1 },
//    { "v", -1 },
//    { "g", 0 },
//    { "color", null },
//    { "started", false }
//};
//        int blkdiv = 0;
//        Dictionary<string, Dictionary<string, object>> tgls = new Dictionary<string, Dictionary<string, object>>
//{
//    { "mtr ", new Dictionary<string, object> { { "x", 0 }, { "y", 0 }, { "c", "\u0020" } } },
//    { "brace", new Dictionary<string, object> { { "x", 0 }, { "y", 0 }, { "c", "\ue000" } } }
//};
//        Dictionary<string, object> glyphs = new Dictionary<string, object>();
//        Action anno_start = empty_function;
//        Action anno_stop = empty_function;
//        object[] gla = new object[] { new List<object>(), new List<object>(), "", new List<object>(), new List<object>(), new List<object>() };

//        Dictionary<string, Dictionary<string, object>> deco_str_style = new Dictionary<string, Dictionary<string, object>>
//{
//    { "crdc", new Dictionary<string, object> { { "dx", 0 }, { "dy", 5 }, { "style", "font:italic 14px text,serif" }, { "anchor", " text-anchor=\"middle\"" } } },
//    { "dacs", new Dictionary<string, object> { { "dx", 0 }, { "dy", 3 }, { "style", "font:bold 15px text,serif" }, { "anchor", " text-anchor=\"middle\"" } } },
//    { "pf", new Dictionary<string, object> { { "dx", 0 }, { "dy", 5 }, { "style", "f" }, { "anchor", "t\"" } } }
//};

        void out_trem(double x, double y, int ntrem)
        {
            out_XYAB("<path d=\"mX Y\n\t", x - 4.5, y);
            while (true)
            {
                output += "l9 -3v3l-9 3z";
                if (--ntrem <= 0)
                    break;
                output += "m0 5.4";
            }
            output += "\"/>\n";
        }

        void out_tubr(double x, double y, double dx, double dy, bool up)
        {
            double h = up ? -3 : 3;

            y += h;
            dx /= (double)stv_g["scale"];
            output += "<path class=\"stroke\" d=\"m";
            out_sxsy(x, " ", y);
            output += "v" + h.ToString("F1") +
                "l" + dx.ToString("F1") + " " + (-dy).ToString("F1") +
                "v" + (-h).ToString("F1") + "\"/>\n";
        }

        void out_tubrn(double x, double y, double dx, double dy, bool up, string str)
        {
            double dxx;
            double sw = str.Length * 10;
            double h = up ? -3 : 3;

            set_font("tuplet");
            xy_str(x + dx / 2, y + dy / 2 - gene.curfont.size * 0.1,
                str, 'c');
            dx /= (double)stv_g["scale"];
            if (!up)
                y += 6;
            output += "<path class=\"stroke\" d=\"m";
            out_sxsy(x, " ", y);
            dxx = dx - sw + 1;
            if (dy > 0)
                sw += dy / 8;
            else
                sw -= dy / 8;
            output += "v" + h.ToString("F1") +
                "m" + dx.ToString("F1") + " " + (-dy).ToString("F1") +
                "v" + (-h).ToString("F1") + "\"/>\n" +
                "<path class=\"stroke\" stroke-dasharray=\"" +
                (dxx / 2).ToString("F1") + " " + sw.ToString("F1") +
                "\" d=\"m";
            out_sxsy(x, " ", y - h);
            output += "l" + dx.ToString("F1") + " " + (-dy).ToString("F1") + "\"/>\n";
        }

        void out_wln(double x, double y, double w)
        {
            out_XYAB("<path class=\"stroke\" stroke-width=\"0.8\" d=\"mX YhF\"/>\n",
                x, y + 1, w);
        }

        void out_deco_str(double x, double y, object de)
        {
            string name = ((Dictionary<string, object>)de).["dd"].["glyph"].ToString();    // class

            if (name == "fng")
            {
                out_XYAB("\n<text x=\"X\" y=\"Y\" style=\"font-size:14px\">A</text>\n",
                    x - 2, y, m_gl(((Dictionary<string, object>)de).["dd"].["str"].ToString()));
                return;
            }

            if (name == "@")            // compatibility
                name = "at";
            else if (!Regex.IsMatch(name, "^[A-Za-z][A-Za-z\\-_]*$"))
            {
                error(1, ((Dictionary<string, object>)de).["s"], "No function for decoration '$1'", ((Dictionary<string, object>)de).["dd"].["name"]);
                return;
            }

            Dictionary<string, object> a_deco = null;

            if (!deco_str_style.TryGetValue(name, out a_deco))
                a_deco = deco_str_style["crdc"];    // default style
            else if (a_deco.ContainsKey("style"))
            {
                style += "\n." + name + "{" + a_deco["style"] + "}";
                a_deco.Remove("style");
            }

            x += (double)a_deco["dx"];
            y += (double)a_deco["dy"];
            out_XYAB("<text x=\"X\" y=\"Y\" class=\"A\"B>", x, y,
                name, a_deco.ContainsKey("anchor") ? a_deco["anchor"].ToString() : "");
            set_font("annotation");
            out_str(((Dictionary<string, object>)de).["dd"].["str"]);
            output += "</text>\n";
        }


        /***************************************/
//        var output = "";
//        var style = "sst";
//        var font_style = "";
//        var posx = cfmt.leftmargin / cfmt.scale;
//        var posy = 0;
//        var img = new DrawImage
//        {
//            width = cfmt.pagewidth,
//            lm = cfmt.leftmargin,
//            rm = cfmt.rightmargin,
//            chg = true
//        };
//        var defined_glyph = new Dictionary<string, object>();
//        var defs = "";
//        var fulldefs = "";
//        var stv_g = new
//        {
//            scale = 1,
//            dy = 0,
//            st = -1,
//            v = -1,
//            g = 0,
//            color = (string)null,
//            started = (bool?)null
//        };
//        var blkdiv = 0;
//        var tgls = new Dictionary<string, object>
//{
//    { "mtr ", new { x = 0, y = 0, c = "\u0020" } },
//    { "brace", new { x = 0, y = 0, c = "\ue000" } }
//};
//        var glyphs = new Dictionary<string, object>();
//        var anno_start = empty_function;
//        var anno_stop = empty_function;


        void out_arp(double x, double y, double val)
        {
            g_open(x, y, 270);
            x = 0;
            val = Math.Ceiling(val / 6);
            while (--val >= 0)
            {
                xygl(x, 6, "ltr");
                x += 6;
            }
            g_close();
        }

        void out_cresc(double x, double y, double val, object defl)
        {
            x += val * stv_g.scale;
            val = -val;
            out_XYAB("<path class=\"stroke\"\n\td=\"mX YlF ", x, y, val);
            if ((bool)defl.nost)
                output += "-2.2m0 -3.6l" + (-val).ToString("F1") + " -2.2\"/>\n";
            else
                output += "-4l" + (-val).ToString("F1") + " -4\"/>\n";
        }

        void out_dim(double x, double y, double val, object defl)
        {
            out_XYAB("<path class=\"stroke\"\n\td=\"mX YlF ", x, y, val);
            if ((bool)defl.noen)
                output += "-2.2m0 -3.6l" + (-val).ToString("F1") + " -2.2\"/>\n";
            else
                output += "-4l" + (-val).ToString("F1") + " -4\"/>\n";
        }

        void out_ltr(double x, double y, double val)
        {
            y += 4;
            val = Math.Ceiling(val / 6);
            while (--val >= 0)
            {
                xygl(x, y, "ltr");
                x += 6;
            }
        }

        void out_lped(double x, double y, double val, object defl)
        {
            if (!(bool)defl.nost)
                xygl(x, y, "ped");
            if (!(bool)defl.noen)
                xygl(x + val + 6, y, "pedoff");
        }

        void out_8va(double x, double y, double val, object defl)
        {
            if (val < 18)
            {
                val = 18;
                x -= 4;
            }
            if (!(bool)defl.nost)
            {
                out_XYAB("<text x=\"X\" y=\"Y\" \nstyle=\"font:italic bold 12px text,serif\">8\n<tspan dy=\"-4\" style=\"font-size:10px\">va</tspan></text>\n", x - 8, y);
                x += 12;
                val -= 12;
            }
            y += 6;
            out_XYAB("<path class=\"stroke\" stroke-dasharray=\"6,6\" d=\"mX YhF\"/>\n", x, y, val);
            if (!(bool)defl.noen)
                out_XYAB("<path class=\"stroke\" d=\"mX Yv6\"/>\n", x + val, y);
        }

        void out_8vb(double x, double y, double val, object defl)
        {
            if (val < 18)
            {
                val = 18;
                x -= 4;
            }
            if (!(bool)defl.nost)
            {
                out_XYAB("<text x=\"X\" y=\"Y\" \nstyle=\"font:italic bold 12px text,serif\">8\n<tspan dy=\".5\" style=\"font-size:10px\">vb</tspan></text>\n", x - 8, y);
                x += 10;
                val -= 10;
            }
            out_XYAB("<path class=\"stroke\" stroke-dasharray=\"6,6\" d=\"mX YhF\"/>\n", x, y, val);
            if (!(bool)defl.noen)
                out_XYAB("<path class=\"stroke\" d=\"mX Yv-6\"/>\n", x + val, y);
        }

        void out_15ma(double x, double y, double val, object defl)
        {
            if (val < 25)
            {
                val = 25;
                x -= 6;
            }
            if (!(bool)defl.nost)
            {
                out_XYAB("<text x=\"X\" y=\"Y\" \nstyle=\"font:italic bold 12px text,serif\">15\n<tspan dy=\"-4\" style=\"font-size:10px\">ma</tspan></text>\n", x - 10, y);
                x += 20;
                val -= 20;
            }
            y += 6;
            out_XYAB("<path class=\"stroke\" stroke-dasharray=\"6,6\" d=\"mX YhF\"/>\n", x, y, val);
            if (!(bool)defl.noen)
                out_XYAB("<path class=\"stroke\" d=\"mX Yv6\"/>\n", x + val, y);
        }



/***********************************************/

//var output = "";
//    var style = "sst";
//    var font_style = "";
//    var posx = cfmt.leftmargin / cfmt.scale;
//    var posy = 0;
//    var img = new DrawImage
//    {
//        width = cfmt.pagewidth,
//        lm = cfmt.leftmargin,
//        rm = cfmt.rightmargin,
//        chg = true
//    };
//    var defined_glyph = new Dictionary<string, object>();
//    var defs = "";
//    var fulldefs = "";
//    var stv_g = new
//    {
//        scale = 1,
//        dy = 0,
//        st = -1,
//        v = -1,
//        g = 0,
//        color = (string)null,
//        started = (bool?)null
//    };
//    var blkdiv = 0;
//    var tgls = new Dictionary<string, object>
//{
//    { "mtr ", new { x = 0, y = 0, c = "\u0020" } },
//    { "brace", new { x = 0, y = 0, c = "\ue000" } }
//};
//    var glyphs = new Dictionary<string, object>();
//    var anno_start = empty_function;
//    var anno_stop = empty_function;
//    var gla = new object[] { new List<object>(), new List<object>(), "", new List<object>(), new List<object>(), new List<object>() };

//    var deco_str_style = new
//    {
//        crdc = new
//        {
//            dx = 0,
//            dy = 5,
//            style = "font:ttt",
//            anchor = " t=\"middle\""
//        }
//    };

//    var deco_l_tb = new
//    {
//        glisq = out_glisq,
//        gliss = out_gliss
//    };

    void out_15mb(double x, double y, double val, object defl)
    {
        if (val < 24)
        {
            val = 24;
            x -= 5;
        }
        if (!((bool?)defl.nost ?? false))
        {
            out_XYAB("<text x=\"X\" y=\"Y\" \nstyle=\"font:italic bold 12px text,serif\">15\n<tspan dy=\".5\" style=\"font-size:10px\">mb</tspan></text>\n", x - 10, y);
            x += 18;
            val -= 18;
        }
        out_XYAB("<path class=\"stroke\" stroke-dasharray=\"6,6\" d=\"mX Yh{0}\"/>\n", x, y, val);
        if (!((bool?)defl.noen ?? false))
            out_XYAB("<path class=\"stroke\" d=\"mX Yv-6\"/>\n", x + val, y);
    }

    void out_deco_val(double x, double y, string name, double val, object defl)
    {
        if (deco_val_tb.ContainsKey(name))
            deco_val_tb[name](x, y, val, defl);
        else
            error(1, null, "No function for decoration '$1'", name);
    }

    void out_glisq(double x2, double y2, object de)
    {
        var de1 = de.start;
        var x1 = de1.x;
        var y1 = de1.y + staff_tb[de1.st].y;
        var dx = x2 - x1;
        var dy = self.sh(y1 - y2);

        if (stv_g.g == 0)
            dx /= stv_g.scale;

        var ar = Math.Atan2(dy, dx);
        var a = ar / Math.PI * 180;
        var len = (dx - (de1.s.dots ? 13 + de1.s.xmx : 8) - 8 - (de.s.notes[0].shac ?? 0)) / Math.Cos(ar);

        g_open(x1, y1, a);
        x1 = de1.s.dots ? 13 + de1.s.xmx : 8;
        len = (int)(len / 6);
        if (len < 1)
            len = 1;
        while (--len >= 0)
        {
            xygl(x1, 0, "ltr");
            x1 += 6;
        }
        g_close();
    }

    void out_gliss(double x2, double y2, object de)
    {
        var de1 = de.start;
        var x1 = de1.x;
        var y1 = de1.y + staff_tb[de1.st].y;
        var dx = x2 - x1;
        var dy = self.sh(y1 - y2);

        if (stv_g.g == 0)
            dx /= stv_g.scale;

        var ar = Math.Atan2(dy, dx);
        var a = ar / Math.PI * 180;
        var len = (dx - (de1.s.dots ? 13 + de1.s.xmx : 8) - 8 - (de.s.notes[0].shac ?? 0)) / Math.Cos(ar);

        g_open(x1, y1, a);
        xypath(de1.s.dots ? 13 + de1.s.xmx : 8, 0);
        output += "h" + len.ToString("F1") + "\" stroke-width=\"1\"/>\n";
        g_close();
    }


    /**********************************/

//var output = "";
//    var style = "sst";
//    var font_style = "";
//    var posx = cfmt.leftmargin / cfmt.scale;
//    var posy = 0;
//    var img = new DrawImage
//    {
//        width = cfmt.pagewidth,
//        lm = cfmt.leftmargin,
//        rm = cfmt.rightmargin,
//        chg = true
//    };
//    var defined_glyph = new Dictionary<string, object>();
//    var defs = "";
//    var fulldefs = "";
//    var stv_g = new
//    {
//        scale = 1,
//        dy = 0,
//        st = -1,
//        v = -1,
//        g = 0,
//        color = (string)null,
//        started = (bool?)null
//    };
//    var blkdiv = 0;
//    var tgls = new Dictionary<string, object>
//{
//    { "mtr ", new { x = 0, y = 0, c = "\u0020" } },
//    { "brace", new { x = 0, y = 0, c = "\ue000" } }
//};
//    var glyphs = new Dictionary<string, object>();
//    var anno_start = empty_function;
//    var anno_stop = empty_function;
//    var gla = new object[] { new List<object>(), new List<object>(), "", new List<object>(), new List<object>(), new List<object>() };

//    var deco_str_style = new
//    {
//        crdc = new
//        {
//            dx = 0,
//            dy = 5,
//            style = "font:ttt",
//            anchor = " t=\"middle\""
//        }
//    };

//    var deco_l_tb = new
//    {
//        glisq = out_glisq,
//        gliss = out_gliss
//    };

    void out_deco_long(double x, double y, object de)
    {
        string s;
        object p_v;
        int m;
        object nt;
        int i;
        string name = ((dynamic)de).dd.glyph;
        object de1 = ((dynamic)de).start;

        if (!deco_l_tb.ContainsKey(name))
        {
            error(1, null, "No function for decoration '$1'", name);
            return;
        }

        // if no start or no end, get the y offset of the other end
        p_v = ((dynamic)de).s.p_v;              // voice
        if (((dynamic)de).defl.noen)            // if no end
        {
            s = ((dynamic)p_v).s_next;          // start of the next music line
            while (s != null && !((dynamic)s).dur)
                s = ((dynamic)s).next;
            if (s != null)
            {
                for (m = 0; m <= ((dynamic)s).nhd; m++)
                {
                    nt = ((dynamic)s).notes[m];
                    if (((dynamic)nt).a_dd == null)
                        continue;
                    for (i = 0; i < ((dynamic)nt).a_dd.Length; i++)
                    {
                        if (((dynamic)((dynamic)nt).a_dd[i]).name == ((dynamic)de1).dd.name)
                        {
                            y = 3 * (((dynamic)nt).pit - 18) + ((dynamic)staff_tb[((dynamic)de).s.st]).y;
                            break;
                        }
                    }
                }
            }
            x += 8;             // (there is no note width)
        }
        else if (((dynamic)de).defl.nost)       // no start
        {
            s = ((dynamic)p_v).s_prev;          // end of the previous music line
            while (s != null && !((dynamic)s).dur)
                s = ((dynamic)s).prev;
            if (s != null)
            {
                for (m = 0; m <= ((dynamic)s).nhd; m++)
                {
                    nt = ((dynamic)s).notes[m];
                    if (((dynamic)nt).a_dd == null)
                        continue;
                    for (i = 0; i < ((dynamic)nt).a_dd.Length; i++)
                    {
                        if (((dynamic)((dynamic)nt).a_dd[i]).name == ((dynamic)de1).dd.name)
                        {
                            ((dynamic)de1).y = 3 * (((dynamic)nt).pit - 18);
                            break;
                        }
                    }
                }
            }
            ((dynamic)de1).x -= 8;          // (there is no note width)
        }
        ((dynamic)deco_l_tb[name])(x, y, de);
    }

    string tempo_note(object s, double dur)
    {
        string p;
        object[] elts = identify_note(s, dur);

        switch (elts[0])        // head
        {
            case C.OVAL:
                p = "\ueca2";
                break;
            case C.EMPTY:
                p = "\ueca3";
                break;
            default:
                switch ((int)elts[2])   // flags
                {
                    case 2:
                        p = "\ueca9";
                        break;
                    case 1:
                        p = "\ueca7";
                        break;
                    default:
                        p = "\ueca5";
                        break;
                }
                break;
        }
        if ((bool)elts[1])          // dot
            p += "<tspan dx=\".1em\">\uecb7</tspan>";
        return p;
    }


/*****************************************************/

    //    static string output = "";
    //    static string style = "sst";
    //    static string font_style = "";
    //    static double posx = cfmt.leftmargin / cfmt.scale;
    //    static double posy = 0;
    //    static DrawImage img = new DrawImage
    //    {
    //        width = cfmt.pagewidth,
    //        lm = cfmt.leftmargin,
    //        rm = cfmt.rightmargin,
    //        chg = true
    //    };
    //    static Dictionary<string, object> defined_glyph = new Dictionary<string, object>();
    //    static string defs = "";
    //    static string fulldefs = "";
    //    static StvG stv_g = new StvG
    //    {
    //        scale = 1,
    //        dy = 0,
    //        st = -1,
    //        v = -1,
    //        g = 0
    //    };
    //    static int blkdiv = 0;
    //    static Dictionary<string, Tgl> tgls = new Dictionary<string, Tgl>
    //{
    //    { "mtr ", new Tgl { x = 0, y = 0, c = "\u0020" } },
    //    { "brace", new Tgl { x = 0, y = 0, c = "\ue000" } }
    //};
    //    static Dictionary<string, object> glyphs = new Dictionary<string, object>();
    //    static object anno_start;
    //    static object anno_stop;
    //    static List<List<object>> gla = new List<List<object>> { new List<object>(), new List<object>() };

        static DecoStrStyle deco_str_style = new DecoStrStyle
        {
            crdc = new Crdc
            {
                dx = 0,
                dy = 5,
                style = "font:ttt",
                anchor = " t=\"middle\""
            }
        };

        static DecoLTb deco_l_tb = new DecoLTb
        {
            glisq = out_glisq,
            gliss = out_gliss
        };

        public static void TempoBuild(VoiceItem s)
        {
            int i, j;
            double bx, p, wh, dy;
            double w = 0;
            List<string> str = new List<string>();

            if (s.tempo_str != null) // already done
                return;

            // the music font must be defined
            if (!cfmt.musicfont.used)
                GetFont("music");

            SetFont("tempo");
            if (s.tempo_str1 != null)
            {
                str.Add(s.tempo_str1);
                w += Strwh(s.tempo_str1)[0];
            }
            if (s.tempo_notes != null)
            {
                dy = -1; // notes a bit higher
                for (i = 0; i < s.tempo_notes.Length; i++)
                {
                    p = TempoNote(s, s.tempo_notes[i]);
                    str.Add($"<tspan\nclass=\"{FontClass(cfmt.musicfont)}\" style=\"font-size:{(gene.curfont.size * 1.3).ToString("F1")}px\" dy=\"{dy}\">{p}</tspan>");
                    j = p.Length > 1 ? 2 : 1; // (note and optional dot)
                    w += j * gene.curfont.swfac;
                    dy = 0;
                }
                str.Add("<tspan dy=\"1\">=</tspan>");
                w += Cwidf('=');
                if (s.tempo_ca != null)
                {
                    str.Add(s.tempo_ca);
                    w += Strwh(s.tempo_ca)[0];
                    j = s.tempo_ca.Length + 1;
                }
                if (s.tempo != null) // with a number of beats per minute
                {
                    str.Add(s.tempo);
                    w += Strwh(s.tempo.ToString())[0];
                }
                else // with a beat as a note
                {
                    p = TempoNote(s, s.new_beat);
                    str.Add($"<tspan\nclass=\"{FontClass(cfmt.musicfont)}\" style=\"font-size:{(gene.curfont.size * 1.3).ToString("F1")}px\" dy=\"-1\">{p}</tspan>");
                    j = p.Length > 1 ? 2 : 1;
                    w += j * gene.curfont.swfac;
                    dy = 1;
                }
            }
            if (s.tempo_str2 != null)
            {
                if (dy != 0)
                    str.Add($"<tspan\n\tdy=\"1\">{s.tempo_str2}</tspan>");
                else
                    str.Add(s.tempo_str2);
                w += Strwh(s.tempo_str2)[0];
            }

            // build the string
            s.tempo_str = string.Join(" ", str);
            w += Cwidf(' ') * (str.Count - 1);
            s.tempo_wh = new double[] { w, 13.0 }; // (the height is not used)
        }

        // output a tempo
        /* 字元數：803 */
        public static void Writempo(VoiceItem s, double x, double y)
        {
            double bh;

            SetFont("tempo");
            if (gene.curfont.box)
            {
                gene.curfont.box = false;
                bh = gene.curfont.size + 4;
            }

            //fixme: xy_str() cannot be used because <tspan> in s.tempo_str
            //fixme: then there cannot be font changes by "$n" in the Q: texts
            output += $"<text class=\"{FontClass(gene.curfont)}\" x=\"";
            OutSxsy(x, "\" y=\"", y + gene.curfont.size * .22);
            output += $"\">{s.tempo_str}</text>\n";

            if (bh != 0)
            {
                gene.curfont.box = true;
                output += $"<rect class=\"stroke\" x=\"";
                OutSxsy(x - 2, "\" y=\"", y + bh - 1);
                output += $"\" width=\"{(s.tempo_wh[0] + 4).ToString("F1")}\" height=\"{bh.ToString("F1")}\"/>\n";
            }

            // don't display anymore
            s.invis = true;
        }

        // update the vertical offset
        /* 字元數：35 */
        public static void Vskip(double h) { posy += h; }

        // create the SVG image of the block
        /* 字元數：2279 */
        public static void SvgFlush()
        {
            if (multicol || string.IsNullOrEmpty(output) || !user.img_out || posy == 0)
                return;

            int i;
            string font;
            double w = ((tsnext != null ? tsnext.fmt : cfmt).trimsvg
                ? (cfmt.leftmargin + realwidth * cfmt.scale + cfmt.rightmargin)
                : img.width);
            string head = "<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\"\n\


    xmlns: xlink =\"http://www.w3.org/1999/xlink\"\n\


    fill =\"currentColor\" stroke-width=\".7\"";
            string g = "";

            Glout();

            font = GetFont("music");
            head += $" class=\"{FontClass(font)} tune{tunes.Length}\""; // tune index for play

            posy *= cfmt.scale;
            if (user.imagesize != null)
                head += user.imagesize;
            else
                head += $" width=\"{w.ToString("F0")}px\" height=\"{posy.ToString("F0")}px\"";
            head += $" viewBox=\"0 0 {w.ToString("F0")} {posy.ToString("F0")}\"";
            if (!string.IsNullOrEmpty(cfmt.fgcolor) || !string.IsNullOrEmpty(cfmt.bgcolor))
                head += $" style=\"{(cfmt.fgcolor != null ? $"color:{cfmt.fgcolor};" : "")}{(cfmt.bgcolor != null ? $"background-color:{cfmt.bgcolor}" : "")}\"";
            head += $">\n{fulldefs}";

            if (!string.IsNullOrEmpty(style) || !string.IsNullOrEmpty(font_style))
                head += $"<style>{font_style}{style}\n</style>\n";

            if (!string.IsNullOrEmpty(defs))
                head += $"<defs>{defs}\n</defs>\n";

            // if %%pagescale != 1, do a global scale
            // (with a container: transform scale in <svg> does not work
            //	the same in all browsers)
            // the class is used to know that the container is global
            if (cfmt.scale != 1)
            {
                head += $"<g class=\"g\" transform=\"scale({cfmt.scale.ToString("F2")})\">\n";
                g = "</g>\n";
            }

            if (psvg != null) // if PostScript support
                psvg.PsFlush(true); // + setg(0)

            // start a block if needed
            if (blkdiv > 0)
            {
                user.img_out(blkdiv == 1 ?
                    "<div class=\"nobrk\">" :
                    "<div class=\"nobrk newpage\">");
                blkdiv = -1; // block started
            }
            user.img_out($"{head}{output}{g}</svg>");
            output = "";

            font_style = "";
            if (cfmt.fullsvg)
            {
                defined_glyph = new Dictionary<string, object>();
                for (i = 0; i < abc2svg.font_tb.Length; i++)
                    abc2svg.font_tb[i].used = false;
            }
            else
            {
                style = "";
                fulldefs = "";
            }
            defs = "";
            posy = 0;
        }

        public static void BlkFlush()
        {
            SvgFlush();
            if (blkdiv < 0 && (parse.state == null || cfmt.splittune))
            {
                user.img_out("</div>");
                blkdiv = 0;
            }
        }

        //// Dummy methods to make the code compile
        //static void GetFont(string font) { }
        //static void SetFont(string font) { }
        //static double[] Strwh(string str) => new double[] { 0, 0 };
        //static double TempoNote(VoiceItem s, dynamic note) => 0;
        //static string FontClass(dynamic font) => "";
        //static double Cwidf(char c) => 0;
        //static void OutSxsy(double x, string y, double z) { }
        //static void Glout() { }
        //static dynamic GetFont(string font) => null;
        //static dynamic out_glisq => null;
        //static dynamic out_gliss => null;
        //static dynamic cfmt => null;
        //static dynamic gene => null;
        //static dynamic user => null;
        //static dynamic tsnext => null;
        //static dynamic realwidth => null;
        //static dynamic psvg => null;
        //static dynamic parse => null;
        //static dynamic tunes => null;

        public class DrawImage
        {
            public double width { get; set; }
            public double lm { get; set; }
            public double rm { get; set; }
            public bool chg { get; set; }
        }

        public class StvG
        {
            public double scale { get; set; }
            public double dy { get; set; }
            public int st { get; set; }
            public int v { get; set; }
            public int g { get; set; }
            public string color { get; set; }
            public bool? started { get; set; }
        }

        //public class Tgl
        //{
        //    public double x { get; set; }
        //    public double y { get; set; }
        //    public string c { get; set; }
        //}

        public class DecoStrStyle
        {
            public Crdc crdc { get; set; }
            public Crdc dacs { get; set; }
            public Crdc pf { get; set; }
        }

        public class Crdc
        {
            public double dx { get; set; }
            public double dy { get; set; }
            public string style { get; set; }
            public string anchor { get; set; }
        }

        public class DecoLTb
        {
            public dynamic glisq { get; set; }
            public dynamic gliss { get; set; }
        }
    























}


}