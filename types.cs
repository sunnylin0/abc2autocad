using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
namespace autocad_part2
{

    public partial class Abc2Svg
    {
        public Dictionary<string, object> Abc { get; set; }
        public object sym_name { get; set; }
        public object[] font_tb { get; set; }    // fonts - index = font.fid
        public Dictionary<string, object> font_st { get; set; }    // font style => font_tb index for incomplete user fonts

        public List<object> mhooks { get; set; }
        public object sheet { get; set; }
        public Regex ft_re { get; set; }
        public Func<int, int, object> rat { get; set; }
        public Func<object, object, object> pitcmp { get; set; }
        public Func<string, Action, Action> loadjs { get; set; }
        public Action<object> printErr { get; set; }

        public class C
        {
            public const int BLEN = 1536;

            // symbol types
            public const int BAR = 0;
            public const int CLEF = 1;
            public const int CUSTOS = 2;
            public const int SM = 3;        // sequence marker (transient)
            public const int GRACE = 4;    // this is the decoration note class
            public const int KEY = 5;      // this is the key signature K:C, K:Db
            public const int METER = 6;    // this is the time signature M:4/4 3/4
            public const int MREST = 7;
            public const int NOTE = 8;     // this is the note
            public const int PART = 9;
            public const int REST = 10;
            public const int SPACE = 11;
            public const int STAVES = 12;
            public const int STBRK = 13;
            public const int TEMPO = 14;
            public const int BLOCK = 16;
            public const int REMARK = 17;

            // note heads
            public const int FULL = 0;
            public const int EMPTY = 1;
            public const int OVAL = 2;
            public const int OVALBARS = 3;
            public const int SQUARE = 4;

            // position types
            public const int SL_ABOVE = 0x01;        // position (3 bits)
            public const int SL_BELOW = 0x02;
            public const int SL_AUTO = 0x03;
            public const int SL_HIDDEN = 0x04;
            public const int SL_DOTTED = 0x08;    // modifiers
            public const int SL_ALI_MSK = 0x70;    // align
            public const int SL_ALIGN = 0x10;
            public const int SL_CENTER = 0x20;
            public const int SL_CLOSE = 0x40;
        }

        sbyte[][] keys { get; set; }
        sbyte[] p_b40 { get; set; }
        sbyte[] b40_p { get; set; }
        sbyte[] b40_a { get; set; }
        sbyte[] b40_m { get; set; }
        sbyte[] b40Mc { get; set; }
        sbyte[] b40sf { get; set; }
        sbyte[] isb40 { get; set; }

        //Func<sbyte, sbyte, object> pab40 { get; set; }
        //Func<sbyte, object> b40p { get; set; }
        //Func<sbyte, object> b40a { get; set; }
        //Func<sbyte, object> b40m { get; set; }

        public Dictionary<string, string> ch_alias { get; set; }
        public string[,] _ch_alias = {
               {"maj", ""       }  ,
               {"min", "m"      }  ,
               {"-", "m"        }  ,
               {"°", "dim"      }  ,
               {"+", "aug"      }  ,
               {"+5", "aug"     }  ,
               {"maj7", "M7"    }  ,
               {"Δ7", "M7"     } ,
               {"Δ", "M7"      } ,
               {"min7", "m7"    }  ,
               {"-7", "m7"      }  ,
               {"ø7", "m7b5"    }  ,
               {"°7", "dim7"    }  ,
               {"min+7", "m+7"  }  ,
               {"aug7", "+7"    }  ,
               {"7+5", "+7"     }  ,
               {"7#5", "+7"     }  ,
               {"sus", "sus4"   }  ,
                { "7sus", "7sus4" }
            };

        // font weight
        public Dictionary<string, int> ft_w { get; set; }
        public (string key, int val)[] _ft_w ={
                       ("thin", 100 ),
                       ("extralight", 200) ,
                       ("light", 300     ) ,
                       ("regular", 400   ) ,
                       ("medium", 500    ) ,
                       ("semi", 600      ) ,
                       ("demi", 600      ) ,
                       ("semibold", 600  ) ,
                       ("demibold", 600  ) ,
                       ("bold", 700      ) ,
                       ("extrabold", 800 ) ,
                       ("ultrabold", 800 ) ,
                       ("black", 900     ) ,
                       ("heavy", 900 ) };


        Abc2Svg()
        {


        }
    }

    public class ClefDefinition
    {
        public int type { get; set; }
        public int clef_line { get; set; }
        public string clef_type { get; set; }
        public string clef_name { get; set; }
        public bool clef_auto { get; set; }
        public bool clef_none { get; set; }
        public bool clef_oct_transp { get; set; }
        public int clef_octave { get; set; }
        public int v { get; set; }
        public PageVoiceTune p_v { get; set; }
        public int time { get; set; }
        public int dur { get; set; }
        public bool invis { get; set; }
    }

    public static class elem
    {
        public static string[] properties = {
        "type", "fname", "stem", "multi", "nhd", "xmx", "istart", "iend", "notes",
        "dur_orig", "dur", "next", "prev", "v", "p_v", "st", "time", "fmt", "pos", "ts_prev",
        "ts_next", "head", "dots", "nflags", "beam_st", "beam_end", "mid", "ys", "ymn", "y",
        "ymx", "wr", "wl", "seqst", "shrink", "space", "x", "xs"
    };
    }

    public class Stv_G
    {
        public double scale { get; set; }
        public double dy { get; set; }
        public int st { get; set; }
        public int v { get; set; }
        public int g { get; set; }
        public string color { get; set; }
        public bool started { get; set; }
    }

    public class Parse
    {
        public object ctx { get; set; }
        public string prefix { get; set; }
        public int state { get; set; }
        public int[] ottava { get; set; }
        public ScanBuf line { get; set; }
        public string fname { get; set; }
        public int istart { get; set; }
        public int iend { get; set; }
        public string file { get; set; }
        public object ckey { get; set; }
        public int eol { get; set; }
        public Dictionary<string, object> ctrl { get; set; }
        public int repeat_n { get; set; }
        public int repeat_k { get; set; }
        public List<object> scores { get; set; }
        public object tp { get; set; }
        public object tps { get; set; }
        public int tpn { get; set; }
        public int bol { get; set; }
        public bool stemless { get; set; }
        public bool ufmt { get; set; }
        public Dictionary<string, int> tune_v_opts { get; set; }
        public Dictionary<string, int> voice_opts { get; set; }
        public Dictionary<string, int> tune_opts { get; set; }
        public Regex select { get; set; }
    }

    public class SaveGlobalDefinitions
    {
        public object cfmt { get; set; }
        public Dictionary<string, object> char_tb { get; set; }
        public Dictionary<string, object> glovar { get; set; }
        public Dictionary<string, object> info { get; set; }
        public Dictionary<string, object> maci { get; set; }
        public Dictionary<string, object> mac { get; set; }
        public Dictionary<string, object> maps { get; set; }
    }

    public class GlobalVar
    {
        public VoiceMeter meter { get; set; }
        public VoiceTempo tempo { get; set; }
        public bool mrest_p { get; set; }
        public int ulen { get; set; }
        public int new_nbar { get; set; }
        public int[] ottava { get; set; }
    }

    public class ScanBuf
    {
        public string buffer { get; set; }
        public int index { get; set; }
        public void Char() { }
        public void nextChar() { }
        public int getInt() { return 0; }
    }

    public class PageVoiceTune
    {
        public double v { get; set; }
        public string id { get; set; }
        public double time { get; set; }
        public object pos { get; set; }
        public double scale { get; set; }
        public double ulen { get; set; }
        public double dur_fact { get; set; }
        public VoiceMeter meter { get; set; }
        public double wmeasure { get; set; }
        public double staffnonote { get; set; }
        public VoiceClef clef { get; set; }
        public List<object> acc { get; set; }
        public List<SlurGroup> sls { get; set; }
        public double hy_st { get; set; }
        public double cst { get; set; }
        public int st { get; set; }
        public VoiceKey ckey { get; set; }
        public bool init { get; set; }
        public bool jianpu { get; set; }
        public string nm { get; set; }
        public string snm { get; set; }
        public bool ignore { get; set; }
        public VoiceItem last_note { get; set; }
        public VoiceItem sym { get; set; }
        public VoiceItem last_sym { get; set; }
        public VoiceItem lyric_restart { get; set; }
        public VoiceItem sym_restart { get; set; }
        public object tie_s_rep { get; set; }
        public bool have_ly { get; set; }
        public VoiceItem lyric_start { get; set; }
        public int lyric_line { get; set; }
        public VoiceItem lyric_cont { get; set; }
        public object tie_s { get; set; }
        public class Key
        {
            public new int type { get; set; }
            public double dur { get; set; }
            public string fname { get; set; }
            public int istart { get; set; }
            public int iend { get; set; }
            public int k_sf { get; set; }
            public sbyte[] k_map { get; set; }
            public int k_mode { get; set; }
            public int k_b40 { get; set; }
            public double wl { get; set; }
            public double wr { get; set; }
        }
        public Key key { get; set; }
        public VoiceItem osym { get; set; }
        public VoiceItem s_next { get; set; }
        public VoiceItem s_prev { get; set; }
        public object second { get; set; }
        public object bar_start { get; set; }
    }

    public class Font
    {
        public bool box { get; set; }
        public int pad { get; set; }
        public string name { get; set; }
        public string weight { get; set; }
        public int size { get; set; }
        public bool used { get; set; }
        public int fid { get; set; }
        public double swfac { get; set; }
        public string fname { get; set; }
        public string style { get; set; }
        public string src { get; set; }
    }

    public class FormationInfo
    {

        public string abc_version { get; set; }
        public Font annotationfont { get; set; }
        public int aligncomposer { get; set; }
        public double beamslope { get; set; }
        private string _bardef;
        public string bardef
        {
            get { return _bardef; }
            set
            {
                switch (value)
                {
                    case "[":
                    case "[]":
                    case "|:":
                    case "|::":
                    case "|:::":
                    case ":|":
                    case "::|":
                    case ":::|":
                        _bardef = value;
                        break;
                    default:
                        _bardef = "";
                        break;
                }
            }
        }
        public double breaklimit { get; set; }
        public bool breakoneoln { get; set; }
        public bool cancelkey { get; set; }
        public Font composerfont { get; set; }
        public double composerspace { get; set; }
        public bool decoerr { get; set; }
        public bool dynalign { get; set; }
        public Font footerfont { get; set; }
        public string fullsvg { get; set; }
        public Font gchordfont { get; set; }
        public double[] gracespace { get; set; }
        public bool graceslurs { get; set; }
        public Font headerfont { get; set; }
        public Font historyfont { get; set; }
        public bool hyphencont { get; set; }
        public double indent { get; set; }
        public Font infofont { get; set; }
        public string infoname { get; set; }
        public double infospace { get; set; }
        public bool keywarn { get; set; }
        public double leftmargin { get; set; }
        public double lineskipfac { get; set; }
        public bool linewarn { get; set; }
        public double maxshrink { get; set; }
        public double maxstaffsep { get; set; }
        public double maxsysstaffsep { get; set; }
        public int measrepnb { get; set; }
        public Font measurefont { get; set; }
        public int measurenb { get; set; }
        public Font musicfont { get; set; }
        public double musicspace { get; set; }
        public Font partsfont { get; set; }
        public double parskipfac { get; set; }
        public double partsspace { get; set; }
        public double pagewidth { get; set; }
        public string propagate_accidentals { get; set; }
        public double printmargin { get; set; }
        public double rightmargin { get; set; }
        public double rbmax { get; set; }
        public double rbmin { get; set; }
        public Font repeatfont { get; set; }
        public double scale { get; set; }
        public double slurheight { get; set; }
        public double[] spatab { get; set; }
        public double staffsep { get; set; }
        public double stemheight { get; set; }
        public double stretchlast { get; set; }
        public bool stretchstaff { get; set; }
        public Font subtitlefont { get; set; }
        public double subtitlespace { get; set; }
        public double sysstaffsep { get; set; }
        public int systnames { get; set; }
        public Font tempofont { get; set; }
        public Font textfont { get; set; }
        public double textspace { get; set; }
        public double tieheight { get; set; }
        public Font titlefont { get; set; }
        public double titlespace { get; set; }
        public object titletrim { get; set; }
        public double topspace { get; set; }
        public int[] tuplets { get; set; }
        public Font tupletfont { get; set; }
        public Font vocalfont { get; set; }
        public double vocalspace { get; set; }
        public Font voicefont { get; set; }
        public string writefields { get; set; }
        public Font wordsfont { get; set; }
        public double wordsspace { get; set; }
        public string writeout_accidentals { get; set; }
        public double pageheight { get; set; }
        public string dateformat { get; set; }
        public int barsperstaff { get; set; }
        public bool squarebreve { get; set; }
        public bool altchord { get; set; }
        public bool quiet { get; set; }
        public bool infoline { get; set; }
        public bool splittune { get; set; }
        public string fgcolor { get; set; }
        public string bgcolor { get; set; }
        public string graceword { get; set; }
        public int contbarnb { get; set; }
        public string textoption { get; set; }
        public int transp { get; set; }
        public int tp { get; set; }
        public int tps { get; set; }
        public string voice_opts { get; set; }
        public string tune_v_opts { get; set; }
        public bool titlecaps { get; set; }
        public bool titleleft { get; set; }
        public string titleformat { get; set; }
        public string sound { get; set; }
        public bool nedo { get; set; }
        public double[] temper { get; set; }
        public bool checkbars { get; set; }
        public bool custos { get; set; }
        public bool ufmt { get; set; }
        public object select { get; set; }
        public string tune_opts { get; set; }
        public string soundfont { get; set; }
        public bool oldmrest { get; set; }
        public bool singleline { get; set; }
        public bool pedline { get; set; }
    }

    public class Information
    {
        public string X { get; set; }
        public string T { get; set; }
        public string W { get; set; }
        public string K { get; set; }
        public string C { get; set; }
        public string V { get; set; }
        public string M { get; set; }
        public string Q { get; set; }
        public string P { get; set; }
        public string R { get; set; }
        public string A { get; set; }
        public string O { get; set; }
    }

    public class DrawImage
    {
        public int width { get; set; }
        public int lm { get; set; }
        public int rm { get; set; }
        public int lw { get; set; }
        public bool chg { get; set; }
    }

    public class NoteItem
    {
        public int pit { get; set; }
        public int opit { get; set; }
        public double shhd { get; set; }
        public int shac { get; set; }
        public int dur { get; set; }
        public int midi { get; set; }
        public int jn { get; set; }
        public int jo { get; set; }
        public int acc { get; set; }
        public bool invis { get; set; }
        public List<DecorationItem> a_dd { get; set; }
    }

    public class DecorationItem
    {
        public string name { get; set; }
        public int func { get; set; }
        public string glyph { get; set; }
        public int h { get; set; }
        public int hd { get; set; }
        public double wl { get; set; }
        public double wr { get; set; }
        public int dx { get; set; }
        public int dy { get; set; }
        public string str { get; set; }
        public int ty { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public bool inv { get; set; }
        public bool has_val { get; set; }
        public int val { get; set; }
        public DecorationItem dd_st { get; set; }
        public DecorationItem dd_en { get; set; }
    }

    public class LyricsItem
    {
        public string t { get; set; }
        public Font font { get; set; }
        public int istart { get; set; }
        public int iend { get; set; }
        public int ln { get; set; }
        public int shift { get; set; }
    }

    public class GChordItem
    {
        public string type { get; set; }
        public string text { get; set; }
        public double x { get; set; }
        public int pos { get; set; }
        public string otext { get; set; }
        public int istart { get; set; }
        public int iend { get; set; }
        public Font font { get; set; }
    }

    //public class BarDef
    //{
    //    public string  "[" { get; set; }
    //    public string  "[]" { get; set; }
    //    public string  "|:" { get; set; }
    //    public string  "|::" { get; set; }
    //    public string  "|:::" { get; set; }
    //    public string  ":|" { get; set; }
    //    public string  "::|" { get; set; }
    //    public string  ":::|" { get; set; }
    //    public string  "::" { get; set; }
    //}

    public class Meter
    {
        public int top { get; set; }
        public int bot { get; set; }
    }

    public class Staff
    {
        public int flags { get; set; }
        public double[] top { get; set; }
        public double[] bot { get; set; }
        public double topbar { get; set; }
        public double botbar { get; set; }
        public double hll { get; set; }
        public int[] hlmap { get; set; }
        public double[] hlu { get; set; }
        public double[] hld { get; set; }
        public string stafflines { get; set; }
        public string scale_str { get; set; }
        public int botline { get; set; }
        public double y { get; set; }
        public double ann_top { get; set; }
        public double ann_bot { get; set; }
        public double staffscale { get; set; }
        public double staffnonote { get; set; }
    }

    public class BeamItem
    {

        public double a { get; set; }
        public double b { get; set; }
        public VoiceItem s1 { get; set; }
        public VoiceItem s2 { get; set; }
        public int nflags { get; set; }
    }

    public class SlurGroup
    {
        public string ty { get; set; }
        public VoiceItem ss { get; set; }
        public VoiceItem se { get; set; }
        public object nte { get; set; }
        public bool grace { get; set; }
        public string loc { get; set; }
    }

    public class Gener
    {
        public Font curfont { get; set; }
        public Font deffont { get; set; }
        public List<object> a_sl { get; set; }
        public int nbar { get; set; }
        public List<object> st_print { get; set; }
        public int vnt { get; set; }
    }


    public class VoiceBase : IVoiceBase
    {
        public int type { get; set; }
        public int v { get; set; }
        public int dur { get; set; }
        public int time { get; set; }
        public FormationInfo fmt { get; set; }
        public PageVoiceTune p_v { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double wr { get; set; }
        public double wl { get; set; }
        public int st { get; set; }
        public List<DecorationItem> a_dd { get; set; }
        public List<LyricsItem> a_ly { get; set; }
        public List<GChordItem> a_gch { get; set; }
        public VoiceItem next { get; set; }
        public VoiceItem prev { get; set; }
        public VoiceItem ts_prev { get; set; }
        public VoiceItem ts_next { get; set; }
        public bool err { get; set; }
        public bool grace { get; set; }
    }

    public class VoiceBar : VoiceBase, IVoiceBar
    {

        public new int type { get; set; }
        public string bar_type { get; set; }
        public int bar_num { get; set; }
        public int bar_mrep { get; set; }
        public bool bar_dotted { get; set; }
        public string text { get; set; }
        public string fname { get; set; }
        public int istart { get; set; }
        public int multi { get; set; }
        public int iend { get; set; }
        public bool invis { get; set; }
        public object pos { get; set; }
        public bool seqst { get; set; }
        public List<NoteItem> notes { get; set; }
        public int nhd { get; set; }
        public double mid { get; set; }
        public double ymx { get; set; }
        public double ymn { get; set; }
        public int shrink { get; set; }
        public double space { get; set; }
        public int rbstop { get; set; }
        public int rbstart { get; set; }
        public int xsh { get; set; }
        public bool norepbra { get; set; }
    }

    public class VoiceClef : VoiceBase, IVoiceClef
    {

        //public new int type { get; set; }
        public int clef_line { get; set; }
        public string clef_type { get; set; }
        public bool clef_auto { get; set; }
        public string fname { get; set; }
        public int istart { get; set; }
        public int iend { get; set; }
        public double ymx { get; set; }
        public double ymn { get; set; }
        public bool seqst { get; set; }
        public int shrink { get; set; }
        public double space { get; set; }
    }

    public class VoiceKey : VoiceBase, IVoiceKey
    {


        public new int type { get; set; }
        public int istart { get; set; }
        public int iend { get; set; }
        public int k_sf { get; set; }
        public sbyte[] k_map { get; set; }
        public int k_mode { get; set; }
        public int k_b40 { get; set; }
        public bool seqst { get; set; }
        public int k_old_sf { get; set; }
        public int k_y_clef { get; set; }
        public double ymx { get; set; }
        public double ymn { get; set; }
        public int shrink { get; set; }
        public double space { get; set; }
        public bool k_bagpipe { get; set; }
        public bool k_drum { get; set; }
        public bool k_none { get; set; }
        public bool exp { get; set; }
        public List<NoteItem> k_a_acc { get; set; }
    }

    public class VoiceMeter : VoiceBase, IVoiceMeter
    {
        public new int type { get; set; }
        public List<Meter> a_meter { get; set; }
        public string fname { get; set; }
        public int istart { get; set; }
        public int iend { get; set; }
        public int wmeasure { get; set; }
        public int[] x_meter { get; set; }
        public bool seqst { get; set; }
        public double ymx { get; set; }
        public double ymn { get; set; }
        public int shrink { get; set; }
        public double space { get; set; }
        public object pos { get; set; }
        public List<NoteItem> notes { get; set; }
        public int nhd { get; set; }
        public double mid { get; set; }
    }

    public class VoiceNote : VoiceBase, IVoiceNote
    {
        public new int type { get; set; }
        public string fname { get; set; }
        public int stem { get; set; }
        public int multi { get; set; }
        public int nhd { get; set; }
        public int istart { get; set; }
        public int iend { get; set; }
        public List<NoteItem> notes { get; set; }
        public int dur_orig { get; set; }
        public object pos { get; set; }
        public int head { get; set; }
        public int dots { get; set; }
        public int nflags { get; set; }
        public List<VoiceItem> extra { get; set; }
        public int acc { get; set; }
        public bool beam_end { get; set; }
        public bool beam_st { get; set; }
        public double mid { get; set; }
        public double xmx { get; set; }
        public double xs { get; set; }
        public double ys { get; set; }
        public double ymn { get; set; }
        public double ymx { get; set; }
        public bool in_tuplet { get; set; }
        public int tpe { get; set; }
        public bool seqst { get; set; }
        public bool stemless { get; set; }
        public int shrink { get; set; }
        public double space { get; set; }
        public List<SlurGroup> sls { get; set; }
        //public { int ty, voiceitem ss, voiceitem se} sls { get; set; }
        public List<int> slurStart { get; set; }
        public List<int> slurEnd { get; set; }
        public bool soln { get; set; }
        public bool nl { get; set; }
        public int repeat_n { get; set; }
        public int repeat_k { get; set; }
        public bool play { get; set; }
        public bool invis { get; set; }
    }

    public class VoiceRest : VoiceBase, IVoiceRest
    {
        public new int type { get; set; }
        public string fname { get; set; }
        public int stem { get; set; }
        public int multi { get; set; }
        public int nhd { get; set; }
        public double xmx { get; set; }
        public int istart { get; set; }
        public int dur_orig { get; set; }
        public int fmr { get; set; }
        public List<NoteItem> notes { get; set; }
        public object pos { get; set; }
        public int iend { get; set; }
        public bool beam_end { get; set; }
        public int head { get; set; }
        public int dots { get; set; }
        public int nflags { get; set; }
        public bool stemless { get; set; }
        public bool beam_st { get; set; }
        public double mid { get; set; }
        public double ymx { get; set; }
        public double ymn { get; set; }
        public bool invis { get; set; }
        public int nmes { get; set; }
        public int repeat_n { get; set; }
        public int repeat_k { get; set; }
        public int rep_nb { get; set; }
        public bool seqst { get; set; }
        public int shrink { get; set; }
        public double space { get; set; }
        public bool soln { get; set; }
    }
    public class VoiceStaves : VoiceBase, IVoiceStaves
    {
        public new int type { get; set; }
        public string fname { get; set; }
        public int st { get; set; }
        public VoiceStavesSymbols sy { get; set; }
        public bool seqst { get; set; }
        public string parts { get; set; }
        public NoteItem notes { get; set; }
        public int nhd { get; set; }
        public double ymx { get; set; }
        public double ymn { get; set; }
        public int shrink { get; set; }
        public double space { get; set; }
    }

    public class VoiceTempo : VoiceBase, IVoiceTempo
    {
        public new int type { get; set; }
        public string fname { get; set; }
        public int istart { get; set; }
        public int iend { get; set; }
        public string tempo_str1 { get; set; }
        public int[] tempo_notes { get; set; }
        public int tempo { get; set; }
        public List<NoteItem> notes { get; set; }
        public int nhd { get; set; }
        public double mid { get; set; }
        public double ymx { get; set; }
        public double ymn { get; set; }
        public string tempo_str { get; set; }
        public string tempo_str2 { get; set; }
        public string tempo_ca { get; set; }
        public int new_beat { get; set; }
        public (int, int) tempo_wh { get; set; }
        public bool seqst { get; set; }
        public int shrink { get; set; }
        public double space { get; set; }
        public bool invis { get; set; }
    }

    public class VoiceBlock : VoiceBase, IVoiceBlock
    {
        public new int type { get; set; }
        public string subtype { get; set; }
        public string fname { get; set; }
        public int istart { get; set; }
        public int iend { get; set; }
        public object pos { get; set; }
        public bool invis { get; set; }
        public bool play { get; set; }
        public int chn { get; set; }
        public List<NoteItem> notes { get; set; }
        public int nhd { get; set; }
        public double mid { get; set; }
        public double ymx { get; set; }
        public double ymn { get; set; }
        public int instr { get; set; }
    }

    public class VoiceStavesSymbols
    {
        public VoiceStavesVoices[] voices { get; set; }
        public VoiceStavesStaves[] staves { get; set; }
        public int top_voice { get; set; }
        public int nstaff { get; set; }
    }

    public class VoiceStavesVoices
    {
        public int st { get; set; }
        public int range { get; set; }
        public int sep { get; set; }
    }

    public class VoiceStavesStaves
    {
        public string stafflines { get; set; }
        public int staffscale { get; set; }
        public int staffnonote { get; set; }
        public int maxsep { get; set; }
    }

    public class VoiceItem : IVoiceBar, IVoiceClef, IVoiceKey, IVoiceMeter,
                     IVoiceNote, IVoiceRest, IVoiceTempo, IVoiceBlock
    {

        public new int type { get; set; }
        public int v { get; set; }
        public int dur { get; set; }
        public int time { get; set; }
        public FormationInfo fmt { get; set; }
        public PageVoiceTune p_v { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double wr { get; set; }
        public double wl { get; set; }
        public int st { get; set; }
        public List<DecorationItem> a_dd { get; set; }
        public List<LyricsItem> a_ly { get; set; }
        public List<GChordItem> a_gch { get; set; }
        public VoiceItem next { get; set; }
        public VoiceItem prev { get; set; }
        public VoiceItem ts_prev { get; set; }
        public VoiceItem ts_next { get; set; }
        public bool err { get; set; }
        public bool grace { get; set; }


        //public new int type { get; set; }
        public string bar_type { get; set; }
        public int bar_num { get; set; }
        public int bar_mrep { get; set; }
        public bool bar_dotted { get; set; }
        public string text { get; set; }
        public string fname { get; set; }
        public int istart { get; set; }
        public int multi { get; set; }
        public int iend { get; set; }
        public bool invis { get; set; }
        public object pos { get; set; }
        public bool seqst { get; set; }
        public List<NoteItem> notes { get; set; }
        public int nhd { get; set; }
        public double mid { get; set; }
        public double ymx { get; set; }
        public double ymn { get; set; }
        public int shrink { get; set; }
        public double space { get; set; }
        public int rbstop { get; set; }
        public int rbstart { get; set; }
        public int xsh { get; set; }
        public bool norepbra { get; set; }


        //public new int type { get; set; }
        public int clef_line { get; set; }
        public string clef_type { get; set; }
        public bool clef_auto { get; set; }



        public int k_sf { get; set; }
        public sbyte[] k_map { get; set; }
        public int k_mode { get; set; }
        public int k_b40 { get; set; }
        public int k_old_sf { get; set; }
        public int k_y_clef { get; set; }
        public bool k_bagpipe { get; set; }
        public bool k_drum { get; set; }
        public bool k_none { get; set; }
        public bool exp { get; set; }
        public List<NoteItem> k_a_acc { get; set; }

        public List<Meter> a_meter { get; set; }
        public int wmeasure { get; set; }
        public int[] x_meter { get; set; }



        public int dur_orig { get; set; }
        public int head { get; set; }
        public int dots { get; set; }
        public int nflags { get; set; }
        public List<VoiceItem> extra { get; set; }
        public int acc { get; set; }
        public bool beam_end { get; set; }
        public bool beam_st { get; set; }
        public double xmx { get; set; }
        public double ys { get; set; }
        public bool in_tuplet { get; set; }
        public int tpe { get; set; }
        public bool stemless { get; set; }
        public List<SlurGroup> sls { get; set; }
        //public { int ty, voiceitem ss, voiceitem se} sls { get; set; }
        public List<int> slurStart { get; set; }
        public List<int> slurEnd { get; set; }
        public bool soln { get; set; }
        public bool nl { get; set; }
        public double xs { get; set; }
        public int repeat_n { get; set; }
        public int repeat_k { get; set; }
        public int rep_nb { get; set; }
        public bool play { get; set; }

        public int stem { get; set; }
        public int fmr { get; set; }
        public int nmes { get; set; }

        public VoiceStavesSymbols sy { get; set; }
        public string parts { get; set; }

        public string tempo_str1 { get; set; }
        public int[] tempo_notes { get; set; }
        public int tempo { get; set; }
        public string tempo_str { get; set; }
        public string tempo_str2 { get; set; }
        public string tempo_ca { get; set; }
        public int new_beat { get; set; }
        public (int, int) tempo_wh { get; set; }


        public string subtype { get; set; }
        public int chn { get; set; }
        public int instr { get; set; }

    }
}