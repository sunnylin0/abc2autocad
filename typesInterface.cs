using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;

namespace autocad_part2
{
    public partial class Abc2Svg
    {

    }
    
    public interface IVoiceBase
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
       public List<DecorationDef> a_dd { get; set; }
       public List<LyricsItem> a_ly { get; set; }
       public List<GChordItem> a_gch { get; set; }
       public VoiceItem next { get; set; }
       public VoiceItem prev { get; set; }
       public VoiceItem ts_prev { get; set; }
       public VoiceItem ts_next { get; set; }
       public bool err { get; set; }
       public bool grace { get; set; }
    }

    public interface IVoiceBar : IVoiceBase
    {
        new int type { get; set; }
        string bar_type { get; set; }
        int bar_num { get; set; }
        int bar_mrep { get; set; }
        bool bar_dotted { get; set; }
        string text { get; set; }
        string fname { get; set; }
        int istart { get; set; }
        int multi { get; set; }
        int iend { get; set; }
        bool invis { get; set; }
        object pos { get; set; }
        bool seqst { get; set; }
        List<NoteItem> notes { get; set; }
        int nhd { get; set; }
        double mid { get; set; }
        double ymx { get; set; }
        double ymn { get; set; }
        int shrink { get; set; }
        double space { get; set; }
        int rbstop { get; set; }
        int rbstart { get; set; }
        int xsh { get; set; }
        bool norepbra { get; set; }
    }

    public interface IVoiceClef : IVoiceBase
    {
        new int type { get; set; }
        int clef_line { get; set; }
        string clef_type { get; set; }
        bool clef_auto { get; set; }
        string fname { get; set; }
        int istart { get; set; }
        int iend { get; set; }
        double ymx { get; set; }
        double ymn { get; set; }
        bool seqst { get; set; }
        int shrink { get; set; }
        double space { get; set; }
    }

    public interface IVoiceKey : IVoiceBase
    {
        new int type { get; set; }
        int istart { get; set; }
        int iend { get; set; }
        int k_sf { get; set; }
        sbyte[] k_map { get; set; }
        int k_mode { get; set; }
        int k_b40 { get; set; }
        bool seqst { get; set; }
        int k_old_sf { get; set; }
        int k_y_clef { get; set; }
        double ymx { get; set; }
        double ymn { get; set; }
        int shrink { get; set; }
        double space { get; set; }
        bool k_bagpipe { get; set; }
        bool k_drum { get; set; }
        bool k_none { get; set; }
        bool exp { get; set; }
        List<NoteItem> k_a_acc { get; set; }
    }

    public interface IVoiceMeter : IVoiceBase
    {
        new int type { get; set; }
        List<Meter> a_meter { get; set; }
        string fname { get; set; }
        int istart { get; set; }
        int iend { get; set; }
        int wmeasure { get; set; }
        int[] x_meter { get; set; }
        bool seqst { get; set; }
        double ymx { get; set; }
        double ymn { get; set; }
        int shrink { get; set; }
        double space { get; set; }
        object pos { get; set; }
        List<NoteItem> notes { get; set; }
        int nhd { get; set; }
        double mid { get; set; }
    }

    public interface IVoiceNote : IVoiceBase
    {
        new int type { get; set; }
        string fname { get; set; }
        int stem { get; set; }
        int multi { get; set; }
        int nhd { get; set; }
        int istart { get; set; }
        int iend { get; set; }
        List<NoteItem> notes { get; set; }
        int dur_orig { get; set; }
        object pos { get; set; }
        int head { get; set; }
        int dots { get; set; }
        int nflags { get; set; }
        List<VoiceItem> extra { get; set; }
        int acc { get; set; }
        bool beam_end { get; set; }
        bool beam_st { get; set; }
        double mid { get; set; }
        double xs { get; set; }
        double ys { get; set; }
        double ymn { get; set; }
        double xmx { get; set; }
        double ymx { get; set; }
        bool in_tuplet { get; set; }
        int tpe { get; set; }
        bool seqst { get; set; }
        bool stemless { get; set; }
        int shrink { get; set; }
        double space { get; set; }
        List<SlurGroup> sls { get; set; }
        //{ int ty, voiceitem ss, voiceitem se} sls { get; set; }
        List<int> slurStart { get; set; }
        List<int> slurEnd { get; set; }
        bool soln { get; set; }
        bool nl { get; set; }
        int repeat_n { get; set; }
        int repeat_k { get; set; }
        bool play { get; set; }
        bool invis { get; set; }
    }

    public interface IVoiceRest : IVoiceBase
    {
        new int type { get; set; }
        string fname { get; set; }
        int stem { get; set; }
        int multi { get; set; }
        int nhd { get; set; }
        int istart { get; set; }
        int dur_orig { get; set; }
        int fmr { get; set; }
        List<NoteItem> notes { get; set; }
        object pos { get; set; }
        int iend { get; set; }
        bool beam_end { get; set; }
        int head { get; set; }
        int dots { get; set; }
        int nflags { get; set; }
        bool stemless { get; set; }
        bool beam_st { get; set; }
        double mid { get; set; }
        double xmx { get; set; }
        double ymx { get; set; }
        double ymn { get; set; }
        bool invis { get; set; }
        int nmes { get; set; }
        int repeat_n { get; set; }
        int repeat_k { get; set; }
        int rep_nb { get; set; }
        bool seqst { get; set; }
        int shrink { get; set; }
        double space { get; set; }
        bool soln { get; set; }
    }

    public interface IVoiceStaves : IVoiceBase
    {
        new int type { get; set; }
        string fname { get; set; }
        int st { get; set; }
        VoiceStavesSymbols sy { get; set; }
        bool seqst { get; set; }
        string parts { get; set; }
        NoteItem notes { get; set; }
        int nhd { get; set; }
        double ymx { get; set; }
        double ymn { get; set; }
        int shrink { get; set; }
        double space { get; set; }
    }

    public interface IVoiceTempo : IVoiceBase
    {
        new int type { get; set; }
        string fname { get; set; }
        int istart { get; set; }
        int iend { get; set; }
        string tempo_str1 { get; set; }
        int[] tempo_notes { get; set; }
        int tempo { get; set; }
        List<NoteItem> notes { get; set; }
        int nhd { get; set ; }
        double mid { get; set; }
        double ymx { get; set; }
        double ymn { get; set; }
        string tempo_str { get; set; }
        string tempo_str2 { get; set; }
        string tempo_ca { get; set; }
        int new_beat { get; set; }
        (int, int) tempo_wh { get; set; }
        bool seqst { get; set; }
        int shrink { get; set; }
        double space { get; set; }
        bool invis { get; set; }
    }

    public interface IVoiceBlock : IVoiceBase
    {
        new int type { get; set; }
        string subtype { get; set; }
        string fname { get; set; }
        int istart { get; set; }
        int iend { get; set; }
        object pos { get; set; }
        bool invis { get; set; }
        bool play { get; set; }
        int chn { get; set; }
        List<NoteItem> notes { get; set; }
        int nhd { get; set; }
        double mid { get; set; }
        double ymx { get; set; }
        double ymn { get; set; }
        int instr { get; set; }
    }

    public interface IVoiceItem : IVoiceBar, IVoiceClef, IVoiceKey, IVoiceMeter,
                     IVoiceNote, IVoiceRest, IVoiceTempo, IVoiceBlock
    {

    }
}