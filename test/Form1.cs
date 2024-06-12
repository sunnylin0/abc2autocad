using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    public interface Ifly
    {

    }
    public static class 飞接口的扩展
    {
        public static int indexc;
        public static void 飞<T>(this T 飞实例) where T : Ifly
        {
            Console.WriteLine("准备");
            Console.WriteLine("张开双翅");
            Console.WriteLine("起飞");
            Console.WriteLine("我飞，我飞，我飞飞飞");
        }
    }

    public class 老虎
    {
        public virtual void 自我介绍()
        {
            Console.WriteLine("大家好，我是老虎。");
        }
    }

    public class 苍蝇 : Ifly
    {
        public void 飞一个看看()
        {
            this.飞();
        }
    }

    public class 超级老虎 : 老虎, Ifly
    {
        public override void 自我介绍()
        {
            Console.WriteLine("大家好，我是超级老虎哦！");
        }

        public void 我会飞哟()
        {
            this.飞();
        }
    }



    class mmm
    {
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
    }
    public interface VoiceBase
    {
        int Type { get; set; }
        int V { get; set; }
        int Dur { get; set; }
        int Time { get; set; }
        int X { get; set; }
        int Y { get; set; }
        int Wr { get; set; }
        int Wl { get; set; }
        int St { get; set; }
        bool Err { get; set; }
    }

    public interface VoiceBar : VoiceBase
    {
        int Type { get; set; }
        string Bar_type { get; set; }
        int Bar_num { get; set; }
        int Bar_mrep { get; set; }
        int Rbstart { get; set; }
        int Xsh { get; set; }
        bool Norepbra { get; set; }
    }


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        Dictionary<string, string> ch_alias { get; set; }
        private (string key, string val)[] _ch_alias = {
               ("maj", ""       ),
               ("min", "m"      ),
               ("-", "m"        ),
               ("°", "dim"      ),
               ("+", "aug"      ),
               ("+5", "aug"     ),
               ("maj7", "M7"    ),
               ("Δ7", "M7"     ),
               ("Δ", "M7"      ),
               ("min7", "m7"    ),
               ("-7", "m7"      ),
               ("ø7", "m7b5"    ),
               ("°7", "dim7"    ),
               ("min+7", "m+7"  ),
               ("aug7", "+7"    ),
               ("7+5", "+7"     ),
               ("7#5", "+7"     ),
               ("sus", "sus4"   ),
                ( "7sus", "7sus4" )
            };

        // font weight
        Dictionary<string, int> ft_w { get; set; }
        private (string key, int val)[] _ft_w ={
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


        public void Abc2Svg()
        {
            (string key, string val) tch;
            for (int ii = 0; ii < _ch_alias.Length; ii++)
            {
                //tch = _ch_alias[ii];
                Debug.Print(_ch_alias[ii].key + "  " + _ch_alias[ii].val);
                //Debug.Print(tch.key + "  " +tch.val);
            }
            //foreach ((string key, string val) cch in _ch_alias)
            //{
            //    Debug.Print(cch);
            //}


        }
        private void button1_Click(object sender, EventArgs e)
        {
            Abc2Svg();

        }







    }


}
