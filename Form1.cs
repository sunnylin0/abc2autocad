using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoCAD;

namespace autocad_part2
{
    public partial class Form1 : Form
    {
        //private AcadApplication a;//聲明AutoCAD對象
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AcadExample.AutoCADConnector acadapp = new AcadExample.AutoCADConnector();
            AcadDocument a = acadapp.Application.ActiveDocument;
            
            
            
            double[] startPoint = new double[3]; //聲明直線起點座標
            double[] endPoint = new double[3];//聲明直線終點座標
            //string[] str = textBox1.Text.Split(',');//取出直線起點座標輸入文字框的值，文字框的輸入模式為＂x,y,z＂
            //for (int i = 0; i < 3; i++)
            //    startPoint[i] = Convert.ToDouble(str[i]);//將str數組轉為double型 
            //str = textBox2.Text.Split(',');//取出直線終點座標輸入文字框的值
            //for (int i = 0; i < 3; i++)
            //    endPoint[i] = Convert.ToDouble(str[i]);
            startPoint[0] = 44;
            startPoint[1] = 44;
            startPoint[2] = 0;
            endPoint[0] = 55;
            endPoint[1] = 155;
            endPoint[2] = 0;
            a.ModelSpace.AddLine(startPoint, endPoint);//在AutoCAD中畫直線
            a.Application.Update();//更新顯示
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    double[] startPoint = new double[3]; //聲明直線起點座標
        //    double[] endPoint = new double[3];//聲明直線終點座標
        //    string[] str = textBox1.Text.Split(',');//取出直線起點座標輸入文字框的值，文字框的輸入模式為＂x,y,z＂
        //    for (int i = 0; i < 3; i++)
        //        startPoint[i] = Convert.ToDouble(str[i]);//將str數組轉為double型 
        //    str = textBox2.Text.Split(',');//取出直線終點座標輸入文字框的值
        //    for (int i = 0; i < 3; i++)
        //        endPoint[i] = Convert.ToDouble(str[i]);
        //    a.ActiveDocument.ModelSpace.AddLine(startPoint, endPoint);//在AutoCAD中畫直線
        //    a.Application.Update();//更新顯示
        //}


    }
}
