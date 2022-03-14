using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Drawing.Imaging;//Для работы графического метафайла
using System.Runtime.InteropServices;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;

namespace Геометрия
{
    
    public partial class Form1 : Form
    {
        
        class InfoForGraphicsPipeToFile
            {
            public PipeDecart pipe1;
            public PipeDecart pipe2;
            public string Pipename1;
            public string Pipename2;
            public double corner;
            public string filename;

            public InfoForGraphicsPipeToFile(PipeDecart pipe1, PipeDecart pipe2, string Pipename1, string Pipename2, double corner, string filename)
            {
            this.pipe1= pipe1;
                this.pipe2= pipe2;
                this.Pipename1= Pipename1;
                this.Pipename2= Pipename2;
                this.corner= corner;
                this.filename= filename;
        }

            }

        class InfoForGraphicsOnePipe//Данные для отрисовки графики в окне программы (для одной кромки)
        {
            public PipeDecart pipe;
            public string name;
            public Point PointMin1;
            public Point PointMin2;
            public Point PointMax1;
            public Point PointMax2;
            public double DiamertMax;
            public double DiametrMin;
            public InfoForGraphicsOnePipe(PipeDecart pipe, string name, Point PointMin1, Point PointMin2, Point PointMax1, Point PointMax2, double DiamertMax, double DiametrMin)
            {
                this.pipe=pipe;
                this.name= name;
                this.PointMin1 = PointMin1;
                this.PointMin2 = PointMin2;
                this.PointMax1 = PointMax1;
                this.PointMax2 = PointMax2;
                this.DiamertMax = DiamertMax;
                this.DiametrMin = DiametrMin;
                
        }

        }
        
        class csScanerDataAbouteObject//хранит данные об объекте из файла овалометра
        {
            public string FileDate;
            public string FileTime;
            public string ObjectName;
            public double PipeDiameter;
            public csScanerDataAbouteObject(string FileDate, string FileTime, string ObjectName, double PipeDiameter)
            {
                this.FileDate = FileDate;
                this.FileTime = FileTime;
                this.ObjectName = ObjectName;
                this.PipeDiameter = PipeDiameter;
            }
        }

        class OvalometrFile //хранит содержимое файла с овалометра
        {
            public OvalometrDataAbouteObject OvalometrData;
            public List<OvalometrMeasurementString> OvalMeasure = new List<OvalometrMeasurementString>();

            public OvalometrFile(OvalometrDataAbouteObject OvalometrData, List<OvalometrMeasurementString> OvalMeasure)
            {
                this.OvalometrData = OvalometrData;
                this.OvalMeasure.Clear();
                this.OvalMeasure.AddRange(OvalMeasure);
            }
        }

        class OvalometrDataAbouteObject//хранит данные об объекте из файла овалометра
        {
            public string FileDate;
            public string FileTime;
            public string ObjectName;
            public double PipeDiameter;
            public OvalometrDataAbouteObject(string FileDate, string FileTime, string ObjectName, double PipeDiameter)
            {
                this.FileDate = FileDate;
                this.FileTime = FileTime;
                this.ObjectName = ObjectName;
                this.PipeDiameter = PipeDiameter;
            }
        }

        class OvalometrMeasurementString//хранит данные из строчки измерений файла с овалометра
        {
            public string TimeMeasure;
            public double OdometrMaesure;
            public double LengthMeasere;
            // public int NumbersOfMeasure; 

            public OvalometrMeasurementString(string TimeMeasure, double OdometrMaesure, double LengthMeasere)
            {
                this.TimeMeasure = TimeMeasure;
                this.OdometrMaesure = OdometrMaesure;
                this.LengthMeasere = LengthMeasere;
                //this.NumbersOfMeasure = NumbersOfMeasure;
            }

        }

        static OvalometrFile read_from_file_Ovalometr(string fn)//метод для чтения файла с овалометра
        {

            List<OvalometrMeasurementString> ovalometrMeasurementString = new List<OvalometrMeasurementString>();

            using (StreamReader file = new StreamReader(fn))
            {
                string FileDate = "";
                string FileTime = "";
                string ObjectName = "";
                double PipeDiameter = 0;

                string TimeMeasure;
                double OdometrMaesure;
                double LengthMeasere;


                string ln;
                int Marker = 0;
                while ((ln = file.ReadLine()) != null)
                {
                    if (Marker == 0)
                    {
                        FileDate = ln.Substring(0, ln.IndexOf('/'));
                        string ln1 = ln.Substring(ln.IndexOf('/') + 1, ln.Length - ln.IndexOf('/') - 1);
                        FileTime = ln1.Substring(0, ln1.IndexOf('/'));

                    }
                    if (Marker == 1)
                    {
                        ObjectName = ln.Substring(0, ln.IndexOf('/'));
                        string ln1 = ln.Substring(ln.IndexOf('/') + 1, ln.Length - ln.IndexOf('/') - 1);
                        //string PipeDiameter1 = ln1.Substring(ln1.IndexOf('/') + 1, ln1.Length - ln1.IndexOf('/') - 1);
                        PipeDiameter = double.Parse(ln1.Substring(ln1.IndexOf(':') + 2, ln1.Length - ln1.IndexOf(':') - 2));




                    }

                    if (Marker > 1)
                    {
                        TimeMeasure = ln.Substring(0, ln.IndexOf(';'));
                        string ln1 = ln.Substring(ln.IndexOf(';') + 1, ln.Length - ln.IndexOf(';') - 1);
                        string OdometrMaesureNN = ln1.Substring(0, ln1.IndexOf(';'));

                        OdometrMaesure = double.Parse(OdometrMaesureNN.Replace(".", ","));
                        ln1 = ln1.Substring(ln1.IndexOf(';') + 1, ln1.Length - ln1.IndexOf(';') - 1);
                        string LengthMeasereNN = ln1.Substring(0, ln1.IndexOf(';'));

                        if (LengthMeasereNN != "nan")
                        {
                            LengthMeasere = double.Parse(LengthMeasereNN.Replace(".", ","));
                        }
                        else
                        {
                            LengthMeasere = 333.3;
                        }

                        OvalometrMeasurementString step = new OvalometrMeasurementString(TimeMeasure, OdometrMaesure, LengthMeasere);
                        if (LengthMeasereNN != "nan")
                        {
                            ovalometrMeasurementString.Add(step);
                        }

                    }
                    Marker++;
                }
                OvalometrDataAbouteObject ovalometrDataAbouteObject = new OvalometrDataAbouteObject(FileDate, FileTime, ObjectName, PipeDiameter);
                OvalometrFile ovalometrFile = new OvalometrFile(ovalometrDataAbouteObject, ovalometrMeasurementString);

                return ovalometrFile;

            }
        }
        private void Print_Ovalometr_file(OvalometrFile ovalometrFile)
        {
            richTextBox2.AppendText(Environment.NewLine + $"Дата: {ovalometrFile.OvalometrData.FileDate}, время: {ovalometrFile.OvalometrData.FileTime}, имя: {ovalometrFile.OvalometrData.ObjectName}, D:{ovalometrFile.OvalometrData.PipeDiameter}.");
            for (int i = 0; i < ovalometrFile.OvalMeasure.Count; i++)
            {
                richTextBox2.AppendText(Environment.NewLine + $"{ovalometrFile.OvalMeasure[i].TimeMeasure}--{ovalometrFile.OvalMeasure[i].OdometrMaesure}--{ovalometrFile.OvalMeasure[i].LengthMeasere}");
            }
        }

        class PointPolar//точка в полярных координатах
        {
            public double alpha;
            public double r;

            public PointPolar(double alpha, double r)//конструктор. Создаёт экземпляр класса
            {
                this.alpha = alpha;
                this.r = r;
            }
        }
        class PipePolar//труба в полярных координатах - содержит имя трубы, дату обследования и множество точек в полярных координатах
        {
            public string name;
            public string date;
            public List<PointPolar> points = new List<PointPolar>();

            public PipePolar(string name, string date, List<PointPolar> points)
            {
                this.name = name;
                this.date = date;
                this.points.Clear();
                this.points.AddRange(points);
            }

        }
        class localOffset//класс для хранения величины смещения кромок в одной точке
        {
            public double PointNumber;
            public double PointOffset;

            public localOffset(double x, double y)//конструктор класса
            {
                this.PointNumber = x;
                this.PointOffset = y;
            }


        }
        class edgeOffset//класс для хранения смещения кромок по всей трубе
        {
            public List<localOffset> Offsets = new List<localOffset>();
        }
        class Point//точка в декартовых координатах
        {

            public double x;
            public double y;
            //public List<Point> points = new List<Point>();
            public Point(double x, double y)
            {
                this.x = x;
                this.y = y;
            }

            public Point add(Point p)
            {
                Point result = new Point(this.x + p.x, this.y + p.y);
                return result;
            }

            public Point minus(Point p)
            {
                Point result = new Point(this.x - p.x, this.y - p.y);
                return result;
            }

        }
        class PipeDecart//труба в декартовых координатах
        {
            public List<Point> points = new List<Point>();
            /*public PipeDecart( List<Point> points)
             {
                 this.points.Clear();
                 this.points.AddRange(points);
             }*/
        }
        class RotateResult//класс для хранения промежуточных результатов оптимизации (угол поворота, точка совмещения, сумма смещений)
        {
            public double corner;
            public int pointnumber;
            public double offsetSum;
            public RotateResult(double corner, int pointnumber, double offsetSum)
            {
                this.corner = corner;
                this.pointnumber = pointnumber;
                this.offsetSum = offsetSum;
            }
        }
        class ERR_RAIT//класс для сортировки вариантов стыковки для врезки катушки
        {
            public double corner;
            public double err1;
            public double err2;
            public int rait1;
            public int rait2;
            public int summ_rait;
            public ERR_RAIT(double corner, double err1, double err2, int rait1, int rait2, int summ_rait)
            {
                this.corner = corner;
                this.err1 = err1;
                this.err2 = err2;
                this.rait1 = rait1;
                this.rait2 = rait2;
                this.summ_rait = summ_rait;
            }
        }
        List<PipePolar> resultFromFile = new List<PipePolar>();//массив для хранения информации по результатам чтения файла
        List<PipePolar> PipesForСalculations = new List<PipePolar>();//массив для хранения информации по результатам чтения файла или сборки в конструкторе
        InfoForGraphicsOnePipe infoForGrahhicsOnePipe;
        InfoForGraphicsPipeToFile infoForGraphicsPipeToFile1;
        InfoForGraphicsPipeToFile infoForGraphicsPipeToFile2;
        InfoForGraphicsPipeToFile infoForGraphicsPipeToFile3;
        edgeOffset OffsetOneWeldedJoint;
        edgeOffset OffsetFirstWeldedJoint;
        edgeOffset OffsetSecondWeldedJoint;
        static List<ERR_RAIT> GetErrReit(List<RotateResult> input1, List<RotateResult> input2)//метод для создания сортированного списка рейтингов
        {
            List<ERR_RAIT> resultErrReit = new List<ERR_RAIT>();
            List<ERR_RAIT> ErrReit = new List<ERR_RAIT>();


            for (int i = 0; i < input1.Count; i++)
            {
                ERR_RAIT stepErr = new ERR_RAIT(input1[i].corner, input1[i].offsetSum, input2[i].offsetSum, 0, 0, 0);
                ErrReit.Add(stepErr);
            }

            resultErrReit = sortErrSumAscending(sortErr2Ascending(sortErr1Ascending(ErrReit)));
            return resultErrReit;
        }
        static List<RotateResult> GetOffsetForReiting(PipeDecart Pipe1, PipeDecart Pipe2)//
        {
            List<RotateResult> resultOfJoint = new List<RotateResult>();


            for (int i = 0; i < 360; i++)
            {
                RotateResult step1 = new RotateResult(i, 0, 100000);
                resultOfJoint.Add(step1);
            }

            for (int angel = 0; angel < 360; angel++)
            {
                for (int point_index = 0; point_index < Pipe1.points.Count; point_index++)
                {

                    Point shift_delta = Pipe1.points[point_index].minus(Pipe2.points[0]);
                    PipeDecart pipe2_shifted = shift_pipe(Pipe2, shift_delta);
                    PipeDecart pipe2_rotated = rotate_pipe(pipe2_shifted, pipe2_shifted.points[0], angel);//Вращаем 3 вокруг 1

                    double diff = Calc_pipe_diffRing(Pipe1, pipe2_rotated, point_index);

                    if (diff < resultOfJoint[angel].offsetSum)
                    {
                        resultOfJoint[angel].offsetSum = diff;
                        resultOfJoint[angel].pointnumber = point_index;
                    }

                }
            }
            return resultOfJoint;
        }
        static List<RotateResult> sortAscending(List<RotateResult> parts)//метод для сортировки массива RotateResult по возрастанию
        {
            List<RotateResult> resultOfSort = new List<RotateResult>();
            List<RotateResult> partsSort = parts;

            for (int j = 0; j < parts.Count; j++)
            {
                double sortF = 0;
                for (int i = 0; i < parts.Count; i++)
                {
                    if (parts[i].offsetSum > sortF)
                    {
                        sortF = parts[i].offsetSum;
                    }
                }
                double sort = sortF;
                int step = 0;

                for (int i = 0; i < parts.Count; i++)
                {
                    if (partsSort[i].offsetSum < sort)
                    {
                        sort = parts[i].offsetSum;
                        step = i;
                    }
                }
                RotateResult stepPart = new RotateResult(parts[step].corner, parts[step].pointnumber, parts[step].offsetSum);
                resultOfSort.Add(stepPart);
                partsSort[step].offsetSum = sortF + 1;
            }

            return resultOfSort;
        }
        static List<ERR_RAIT> sortErr1Ascending(List<ERR_RAIT> parts)//метод для сортировки массива  рейтингов по ошибке 1
        {
            List<ERR_RAIT> resultOfSort = new List<ERR_RAIT>();
            List<ERR_RAIT> partsSort = parts;

            for (int j = 0; j < parts.Count; j++)
            {
                double sortF = 0;
                for (int i = 0; i < parts.Count; i++)
                {
                    if (parts[i].err1 > sortF)
                    {
                        sortF = parts[i].err1;
                    }
                }
                double sort = sortF;
                int step = 0;

                for (int i = 0; i < parts.Count; i++)
                {
                    if (partsSort[i].err1 < sort)
                    {
                        sort = parts[i].err1;
                        step = i;
                    }
                }
                ERR_RAIT stepPart = new ERR_RAIT(parts[step].corner, parts[step].err1, parts[step].err2, step, 0, 0);
                resultOfSort.Add(stepPart);
                partsSort[step].err1 = sortF + 1;
            }

            return resultOfSort;
        }
        static List<ERR_RAIT> sortErr2Ascending(List<ERR_RAIT> parts)//метод для сортировки массива  рейтингов по ошибке 2
        {
            List<ERR_RAIT> resultOfSort = new List<ERR_RAIT>();
            List<ERR_RAIT> partsSort = parts;

            for (int j = 0; j < parts.Count; j++)
            {
                double sortF = 0;
                for (int i = 0; i < parts.Count; i++)
                {
                    if (parts[i].err2 > sortF)
                    {
                        sortF = parts[i].err2;
                    }
                }
                double sort = sortF;
                int step = 0;

                for (int i = 0; i < parts.Count; i++)
                {
                    if (partsSort[i].err2 < sort)
                    {
                        sort = parts[i].err2;
                        step = i;
                    }
                }
                ERR_RAIT stepPart = new ERR_RAIT(parts[step].corner, parts[step].err1, parts[step].err2, parts[step].rait1, step, parts[step].rait1 + step);
                resultOfSort.Add(stepPart);
                partsSort[step].err2 = sortF + 1;
            }

            return resultOfSort;
        }
        static List<ERR_RAIT> sortErrSumAscending(List<ERR_RAIT> parts)//метод для сортировки массива  рейтингов по сумме рейтингов
        {
            List<ERR_RAIT> resultOfSort = new List<ERR_RAIT>();
            List<ERR_RAIT> partsSort = parts;

            for (int j = 0; j < parts.Count; j++)
            {
                int sortF = 0;
                for (int i = 0; i < parts.Count; i++)
                {
                    if (parts[i].summ_rait > sortF)
                    {
                        sortF = parts[i].summ_rait;
                    }
                }
                int sort = sortF;
                int step = 0;

                for (int i = 0; i < parts.Count; i++)
                {
                    if (partsSort[i].summ_rait < sort)
                    {
                        sort = parts[i].summ_rait;
                        step = i;
                    }
                }
                ERR_RAIT stepPart = new ERR_RAIT(parts[step].corner, parts[step].err1, parts[step].err2, parts[step].rait1, parts[step].rait2, parts[step].summ_rait);
                resultOfSort.Add(stepPart);
                partsSort[step].summ_rait = sortF + 1;
            }

            return resultOfSort;
        }
        static edgeOffset calcoffset(PipeDecart pipe1, PipeDecart pipe2, double angel, int point_index)// метод, который выдаёт лист с величинами смещения кромок в каждой точке, для определённых кромок в определённом положении
        {

            PipeDecart pipe2_rotated = rotate_pipe(pipe2, pipe2.points[0], angel);
            Point shift_delta = pipe1.points[point_index].minus(pipe2_rotated.points[0]);
            PipeDecart pipe2_shifted = shift_pipe(pipe2_rotated, shift_delta);

            double HeightOfTheRing(Point n1, Point n2, Point n3, Point n4)//метод для определения высоты треугольника
            {

                Point Cr = new Point(0, 0);
                double A = n2.x - n1.x;
                double B = n2.y - n1.y;
                double C = n3.x - n1.x;
                double D = n3.y - n1.y;
                double E = A * (n1.x + n2.x) + B * (n1.y + n1.y);
                double F = C * (n1.x + n3.x) + D * (n1.y + n3.y);
                double G = 2 * (A * (n3.y - n2.y) - B * (n3.x - n2.x));
                double Cx = (D * E - B * F) / G;
                double Cy = (A * F - C * E) / G;
                Cr = new Point(Cx, Cy);//это центр окрудности образованной тремя точками

                double Rad = Math.Sqrt((Cr.x - n1.x) * (Cr.x - n1.x) + (Cr.y - n1.y) * (Cr.y - n1.y));
                double P_to_Rad = Math.Sqrt((Cr.x - n4.x) * (Cr.x - n4.x) + (Cr.y - n4.y) * (Cr.y - n4.y));
                double result = Rad - P_to_Rad;

                return result;


            }

            //Создадим удлинненный массив
            PipeDecart PipeOneLong = new PipeDecart();
            int OnePipe = pipe1.points.Count;
            int DoublePipe = OnePipe * 2;
            PipeOneLong.points = new List<Point>(DoublePipe);

            for (int i = 0; i < OnePipe; i++)
            {
                PipeOneLong.points.Add(new Point(pipe1.points[i].x, pipe1.points[i].y));
            }
            for (int i = 0; i < OnePipe; i++)
            {
                PipeOneLong.points.Add(new Point(pipe1.points[i].x, pipe1.points[i].y));
            }
            //Создали удлинненный массив

            //Выполним расчет суммы расстояний
            //List<localOffset> resultOffset = new List<localOffset>();
            edgeOffset resultOffset = new edgeOffset();
            double offset;
            for (int j = 1; j < OnePipe; j++)
            {
                offset = HeightOfTheRing(PipeOneLong.points[j + point_index - 1], PipeOneLong.points[j + point_index], PipeOneLong.points[j + point_index + 1], pipe2_shifted.points[j]);
                localOffset ls = new localOffset(j, offset);
                resultOffset.Offsets.Add(ls);
            }

            return resultOffset;

        }
        static PipePolar IncreaseTheNumberOfPoints(PipePolar pipe)
        {
            List<PointPolar> points = new List<PointPolar>();
            PipePolar result = new PipePolar(pipe.name, pipe.date, points);


            double angel1;
            PointPolar point1 = new PointPolar(0, pipe.points[0].r);
            result.points.Add(point1);

            angel1 = pipe.points[1].alpha / 2;
            point1 = new PointPolar(angel1, (pipe.points[1].r + pipe.points[0].r) / 2);
            result.points.Add(point1);

            point1 = new PointPolar(pipe.points[1].alpha, pipe.points[1].r);
            result.points.Add(point1);

            angel1 = pipe.points[1].alpha + pipe.points[1].alpha / 2;
            point1 = new PointPolar(angel1, (pipe.points[1].r + pipe.points[2].r) / 2);
            result.points.Add(point1);


            for (int i = 2; i < pipe.points.Count - 1; i++)
            {


                angel1 = pipe.points[i].alpha;
                point1 = new PointPolar(angel1, pipe.points[i].r);
                result.points.Add(point1);

                angel1 = pipe.points[i].alpha + pipe.points[1].alpha / 2;
                point1 = new PointPolar(angel1, (pipe.points[i].r + pipe.points[i + 1].r) / 2);
                result.points.Add(point1);


                Console.Write(".");

            }
            angel1 = pipe.points[pipe.points.Count - 1].alpha;
            point1 = new PointPolar(angel1, pipe.points[pipe.points.Count - 1].r);
            result.points.Add(point1);

            angel1 = pipe.points[pipe.points.Count - 1].alpha + pipe.points[1].alpha / 2;
            point1 = new PointPolar(angel1, (pipe.points[pipe.points.Count - 1].r + pipe.points[0].r) / 2);
            result.points.Add(point1);



            return result;
        }
        static List<PipePolar> read_from_file(string fn)
        {
            List<PipePolar> result = new List<PipePolar>();
            using (StreamReader file = new StreamReader(fn))
            {
                bool isNewPipe = true;
                string name = "";
                string date = "";
                List<double> rs = new List<double>();
                string ln;
                while ((ln = file.ReadLine()) != null)
                {
                    if (ln == "end")
                    {
                        List<PointPolar> points = new List<PointPolar>();
                        for (int i = 0; i < rs.Count; i++)
                        {
                            double angel = Convert.ToDouble(360) * (Convert.ToDouble(i) / Convert.ToDouble(rs.Count));
                            PointPolar point = new PointPolar(angel, rs[i]);
                            points.Add(point);
                        }
                        rs.Clear();
                        PipePolar pipe = new PipePolar(name, date, points);
                        result.Add(pipe);
                        isNewPipe = true;
                    }
                    else
                    {
                        if (isNewPipe)
                        {
                            date = ln;
                            ln = file.ReadLine();
                            name = ln;
                            isNewPipe = false;
                        }
                        else
                        {
                            double d = double.Parse(ln);
                            rs.Add(d);
                        }
                    }
                }
                file.Close();
            }
            return result;
        }
        static List<PipePolar> read_from_file_csScaner(string fn)
        {
            List<PipePolar> result = new List<PipePolar>();
            using (StreamReader file = new StreamReader(fn))
            {
                bool isNewPipe = true;
                string name = "";
                string date = "";
                List<double> rs = new List<double>();
                string ln;
                while ((ln = file.ReadLine()) != null)
                {
                    if (ln == "end")
                    {
                        List<PointPolar> points = new List<PointPolar>();
                        for (int i = 0; i < rs.Count; i++)
                        {
                            double angel = Convert.ToDouble(360) * (Convert.ToDouble(i) / Convert.ToDouble(rs.Count));
                            PointPolar point = new PointPolar(angel, rs[i]);
                            points.Add(point);
                        }
                        rs.Clear();
                        PipePolar pipe = new PipePolar(name, date, points);
                        result.Add(pipe);
                        isNewPipe = true;
                    }
                    else
                    {
                        if (isNewPipe)
                        {
                            date = ln;
                            ln = file.ReadLine();
                            name = ln;
                            isNewPipe = false;
                        }
                        else
                        {
                            string str = ln;
                            string n1 = str.Substring(0, str.IndexOf(';'));
                            string remainder1 = str.Substring(n1.Length, str.Length - n1.Length);
                           
                            string n2 = remainder1.Substring(1, str.IndexOf(';'));
                            string remainder2 = remainder1.Substring(n2.Length + 2, remainder1.Length - n2.Length - 2);
                           
                            string n3 = remainder2.Substring(0, str.IndexOf(';'));
                            string remainder3 = remainder2.Substring(n3.Length + 1, remainder2.Length - n3.Length - 1);
                           
                            string n4 = remainder3.Substring(0, str.IndexOf(';'));
                            string remainder4 = remainder3.Substring(n4.Length + 1, remainder3.Length - n4.Length - 1);
                           
                            string n5 = remainder4.Substring(0, str.IndexOf(';'));
                            string remainder5 = remainder4.Substring(n5.Length + 1, remainder4.Length - n5.Length - 1);
                           
                            string n6 = remainder5.Substring(0, str.IndexOf(';'));
                            string remainder6 = remainder5.Substring(n6.Length + 1, remainder5.Length - n6.Length - 1);
                           
                            string n7 = remainder6.Substring(0, str.IndexOf(';'));
                            string n8 = remainder6.Substring(n7.Length + 1, remainder6.Length - n7.Length - 1);

                            double d = (Convert.ToDouble(n1) + Convert.ToDouble(n2) + Convert.ToDouble(n3) + Convert.ToDouble(n4) + Convert.ToDouble(n5) + Convert.ToDouble(n6) + Convert.ToDouble(n7) + Convert.ToDouble(n8)) / 8;
                            //разделить ln на значения, найти среднее.

                            //double d = double.Parse(ln);
                            rs.Add(d);
                        }
                    }
                }
                file.Close();
            }
            return result;
        }
        static PipeDecart convertToDecart(PipePolar pipePolar)
        {
            PipeDecart result = new PipeDecart();
            for (int i = 0; i < pipePolar.points.Count; i++)
            {
                double x = pipePolar.points[i].r * Math.Cos((pipePolar.points[i].alpha) * (Math.PI / 180));
                double y = pipePolar.points[i].r * Math.Sin((pipePolar.points[i].alpha) * (Math.PI / 180));
                result.points.Add(new Point(x, y));
                //richTextBox1.AppendText(Environment.NewLine + $"x'={result.points[i].x}, y'={result.points[i].y}");
                // richTextBox1.AppendText(Environment.NewLine + $"x={x}, y={y}");
            }

            //TODO 1.перевести в дек коорд
            return result;
        }
        static PipeDecart shift_pipe(PipeDecart pipe, Point delta)
        {
            PipeDecart result = new PipeDecart();
            //result.points = new List<Point>(pipe.points.Count);
            for (int i = 0; i < pipe.points.Count; i++)
            {
                Point point = pipe.points[i];
                //Point newPoint = point.add(delta);

                double nx = pipe.points[i].x + delta.x;
                double ny = pipe.points[i].y + delta.y;
                //result.points[i] = newPoint;
                result.points.Add(new Point(nx, ny));
            }
            return result;
        }
        static PipeDecart rotate_pipe(PipeDecart pipe, Point center, double angel)
        {
            PipeDecart result = new PipeDecart();
            result.points = new List<Point>(pipe.points.Count);

            for (int i = 0; i < pipe.points.Count; i++)
            {
                double xRotated = (pipe.points[i].x - center.x) * Math.Cos(angel * (Math.PI / 180)) - (pipe.points[i].y - center.y) * Math.Sin(angel * (Math.PI / 180)) + center.x;
                double yRotated = (pipe.points[i].x - center.x) * Math.Sin(angel * (Math.PI / 180)) + (pipe.points[i].y - center.y) * Math.Cos(angel * (Math.PI / 180)) + center.y;

                result.points.Add(new Point(xRotated, yRotated));
                // richTextBox1.AppendText(Environment.NewLine + $"x'={result.points[i].x}, y'={result.points[i].y}");
                // richTextBox1.AppendText(Environment.NewLine + $"x={xRotated}, y={yRotated}");
            }

            //TODO повернуть координаты//повернул
            return result;
        }
        static PipePolar mirror_pipe(PipePolar pipe)//отразить точки трубы в полярных координатах
        {
            List<PointPolar> points = new List<PointPolar>();

            PipePolar result = new PipePolar(pipe.name, pipe.date, points);
            result.name = pipe.name;
            result.date = pipe.date;

            result.points = new List<PointPolar>(pipe.points.Count);

            PointPolar point = new PointPolar(pipe.points[0].alpha, pipe.points[0].r);
            result.points.Add(point);

            for (int i = 1; i < pipe.points.Count; i++)
            {
                point = new PointPolar(pipe.points[i].alpha, pipe.points[pipe.points.Count - i].r);
                result.points.Add(point);
            }


            return result;
        }
        static double calc_pipe_diff(PipeDecart pipe1, PipeDecart pipe2, int point_index)//расчет по точкам
        {

            //Создадим удлинненный массив
            PipeDecart PipeOneLong = new PipeDecart();
            int OnePipe = pipe1.points.Count;
            int DoublePipe = OnePipe * 2;
            PipeOneLong.points = new List<Point>(DoublePipe);

            for (int i = 0; i < OnePipe; i++)
            {
                PipeOneLong.points.Add(new Point(pipe1.points[i].x, pipe1.points[i].y));
            }
            for (int i = 0; i < OnePipe; i++)
            {
                PipeOneLong.points.Add(new Point(pipe1.points[i].x, pipe1.points[i].y));
            }
            //Создали удлинненный массив
            // richTextBox1.AppendText(Environment.NewLine + $"Длина массивав: {PipeOneLong.points.Count}.");
            // Console.ReadLine();
            //Выполним расчет суммы расстояний
            double result = 0;
            for (int j = 0; j < OnePipe; j++)
            {
                result += Math.Sqrt(Math.Pow((pipe2.points[j].x - PipeOneLong.points[j + point_index].x), 2) + Math.Pow((pipe2.points[j].y - PipeOneLong.points[j + point_index].y), 2));
            }

            return result;

        }
        static double Calc_pipe_diffNew(PipeDecart pipe1, PipeDecart pipe2, int point_index)//метод вычисления  локального смещения кромок с использованием высоты треугольника-заменён и не используется
        {
            double HeightOfTheTriangle(Point n1, Point n2, Point n3)//метод для определения высоты треугольника
            {
                double A = (n2.y - n1.y) / (n2.x - n1.x);
                double B = -1;
                double C = n1.y - ((n2.y - n1.y) / (n2.x - n1.x)) * n1.x;
                double result1 = Math.Abs(A * n3.x + B * n3.y + C) / Math.Sqrt(A * A + B * B);
                return result1;
            }

            //Создадим удлинненный массив
            PipeDecart PipeOneLong = new PipeDecart();
            int OnePipe = pipe1.points.Count;
            int DoublePipe = OnePipe * 2;
            PipeOneLong.points = new List<Point>(DoublePipe);

            for (int i = 0; i < OnePipe; i++)
            {
                PipeOneLong.points.Add(new Point(pipe1.points[i].x, pipe1.points[i].y));
            }
            for (int i = 0; i < OnePipe; i++)
            {
                PipeOneLong.points.Add(new Point(pipe1.points[i].x, pipe1.points[i].y));
            }
            //Создали удлинненный массив
            // richTextBox1.AppendText(Environment.NewLine + $"Длина массивав: {PipeOneLong.points.Count}.");
            // Console.ReadLine();
            //Выполним расчет суммы расстояний
            double result = 0;
            for (int j = 2; j < OnePipe - 2; j++)
            {
                //double heit = HeightOfTheTriangle(pipe1.points[j], pipe1.points[j+1], pipe2.points[j]);//вычисление расстояния от точки до прямой, содержащей отрезок
                result += Math.Min(HeightOfTheTriangle(PipeOneLong.points[j + point_index], PipeOneLong.points[j + point_index + 1], pipe2.points[j]), HeightOfTheTriangle(PipeOneLong.points[j + point_index], PipeOneLong.points[j + point_index - 1], pipe2.points[j]));
            }

            return result;

        }
        static double Calc_pipe_diffRing(PipeDecart pipe1, PipeDecart pipe2, int point_index)//расчет локального смещения кромок методом дуги
        {
            double HeightOfTheRing(Point n1, Point n2, Point n3, Point n4)//метод для определения высоты треугольника
            {

                Point Cr = new Point(0, 0);
                double A = n2.x - n1.x;
                double B = n2.y - n1.y;
                double C = n3.x - n1.x;
                double D = n3.y - n1.y;
                double E = A * (n1.x + n2.x) + B * (n1.y + n1.y);
                double F = C * (n1.x + n3.x) + D * (n1.y + n3.y);
                double G = 2 * (A * (n3.y - n2.y) - B * (n3.x - n2.x));
                double Cx = (D * E - B * F) / G;
                double Cy = (A * F - C * E) / G;
                Cr = new Point(Cx, Cy);//это центр окрудности образованной тремя точками

                double Rad = Math.Sqrt((Cr.x - n1.x) * (Cr.x - n1.x) + (Cr.y - n1.y) * (Cr.y - n1.y));
                double P_to_Rad = Math.Sqrt((Cr.x - n4.x) * (Cr.x - n4.x) + (Cr.y - n4.y) * (Cr.y - n4.y));
                double result2 = Math.Abs(Rad - P_to_Rad);

                return result2;


            }



            //Создадим удлинненный массив
            PipeDecart PipeOneLong = new PipeDecart();
            int OnePipe = pipe1.points.Count;
            int DoublePipe = OnePipe * 2;
            PipeOneLong.points = new List<Point>(DoublePipe);

            for (int i = 0; i < OnePipe; i++)
            {
                PipeOneLong.points.Add(new Point(pipe1.points[i].x, pipe1.points[i].y));
            }
            for (int i = 0; i < OnePipe; i++)
            {
                PipeOneLong.points.Add(new Point(pipe1.points[i].x, pipe1.points[i].y));
            }
            //Создали удлинненный массив

            //Выполним расчет суммы расстояний
            double result = 0;
            for (int j = 1; j < OnePipe; j++)
            {
                result += HeightOfTheRing(PipeOneLong.points[j + point_index - 1], PipeOneLong.points[j + point_index], PipeOneLong.points[j + point_index + 1], pipe2.points[j]);
            }

            return result;

        }
        private void printPolarPipe(PipePolar pipe)
        {
            //richTextBox1.AppendText(Environment.NewLine +"" ); 
            richTextBox1.AppendText(Environment.NewLine + pipe.name);
            richTextBox1.AppendText(Environment.NewLine + pipe.date);
            for (int i = 0; i < pipe.points.Count; i++)
            {
                Console.Write($"({Math.Round(pipe.points[i].alpha, 0)}, {pipe.points[i].r})");
            }
            //richTextBox1.AppendText(Environment.NewLine +"" ); 
        }
        private void printPolarPipe2(PipePolar pipe)
        {
            //richTextBox1.AppendText(Environment.NewLine +"" ); 
            richTextBox2.AppendText(Environment.NewLine + pipe.name);
            richTextBox2.AppendText(Environment.NewLine + pipe.date);
            for (int i = 0; i < pipe.points.Count; i++)
            {
                richTextBox2.AppendText(Environment.NewLine + $"({i}*****{Math.Round(pipe.points[i].alpha, 0)}, {pipe.points[i].r})");
            }
            //richTextBox1.AppendText(Environment.NewLine +"" ); 
        }
        private void PipeToFile(PipeDecart pipe1, PipeDecart pipe2, string Pipename1, string Pipename2, double corner, string filename)
        {
            string writePath = textBox2.Text + filename + ".dxf";
            using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
            {
                sw.WriteLine("0");
                sw.WriteLine("SECTION");
                sw.WriteLine("2");
                sw.WriteLine("HEADER");
                sw.WriteLine("9");
                sw.WriteLine("$ACADVER");
                sw.WriteLine("1");
                sw.WriteLine("AC1009");
                sw.WriteLine("9");
                sw.WriteLine("$DWGCODEPAGE");
                sw.WriteLine("3");
                sw.WriteLine("ansi_1251");
                sw.WriteLine("9");
                sw.WriteLine("$INSBASE");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$EXTMIN");
                sw.WriteLine("10");
                sw.WriteLine("1.000000E+20");
                sw.WriteLine("20");
                sw.WriteLine("1.000000E+20");
                sw.WriteLine("30");
                sw.WriteLine("1.000000E+20");
                sw.WriteLine("9");
                sw.WriteLine("$EXTMAX");
                sw.WriteLine("10");
                sw.WriteLine("-1.000000E+20");
                sw.WriteLine("20");
                sw.WriteLine("-1.000000E+20");
                sw.WriteLine("30");
                sw.WriteLine("-1.000000E+20");
                sw.WriteLine("9");
                sw.WriteLine("$LIMMIN");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$LIMMAX");
                sw.WriteLine("10");
                sw.WriteLine("84100.0");
                sw.WriteLine("20");
                sw.WriteLine("59400.0");
                sw.WriteLine("9");
                sw.WriteLine("$ORTHOMODE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$REGENMODE");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$FILLMODE");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$QTEXTMODE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$MIRRTEXT");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DRAGMODE");
                sw.WriteLine("70");
                sw.WriteLine("2");
                sw.WriteLine("9");
                sw.WriteLine("$LTSCALE");
                sw.WriteLine("40");
                sw.WriteLine("1.0");
                sw.WriteLine("9");
                sw.WriteLine("$OSMODE");
                sw.WriteLine("70");
                sw.WriteLine("37");
                sw.WriteLine("9");
                sw.WriteLine("$ATTMODE");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$TEXTSIZE");
                sw.WriteLine("40");
                sw.WriteLine("250.0");
                sw.WriteLine("9");
                sw.WriteLine("$TRACEWID");
                sw.WriteLine("40");
                sw.WriteLine("1.0");
                sw.WriteLine("9");
                sw.WriteLine("$TEXTSTYLE");
                sw.WriteLine("7");
                sw.WriteLine("ГОСТ_2_304");
                sw.WriteLine("9");
                sw.WriteLine("$CLAYER");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$CELTYPE");
                sw.WriteLine("6");
                sw.WriteLine("BYLAYER");
                sw.WriteLine("9");
                sw.WriteLine("$CECOLOR");
                sw.WriteLine("62");
                sw.WriteLine("256");
                sw.WriteLine("9");
                sw.WriteLine("$DIMSCALE");
                sw.WriteLine("40");
                sw.WriteLine("100.0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMASZ");
                sw.WriteLine("40");
                sw.WriteLine("44318");
                sw.WriteLine("9");
                sw.WriteLine("$DIMEXO");
                sw.WriteLine("40");
                sw.WriteLine("0.625");
                sw.WriteLine("9");
                sw.WriteLine("$DIMDLI");
                sw.WriteLine("40");
                sw.WriteLine("27454");
                sw.WriteLine("9");
                sw.WriteLine("$DIMRND");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMDLE");
                sw.WriteLine("40");
                sw.WriteLine("2.0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMEXE");
                sw.WriteLine("40");
                sw.WriteLine("45658");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTP");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTM");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTXT");
                sw.WriteLine("40");
                sw.WriteLine("44318");
                sw.WriteLine("9");
                sw.WriteLine("$DIMCEN");
                sw.WriteLine("40");
                sw.WriteLine("44318");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTSZ");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTOL");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMLIM");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTIH");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTOH");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMSE1");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMSE2");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTAD");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$DIMZIN");
                sw.WriteLine("70");
                sw.WriteLine("8");
                sw.WriteLine("9");
                sw.WriteLine("$DIMBLK");
                sw.WriteLine("1");
                sw.WriteLine("ARCHTICK");
                sw.WriteLine("9");
                sw.WriteLine("$DIMASO");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$DIMSHO");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$DIMPOST");
                sw.WriteLine("1");
                sw.WriteLine("");
                sw.WriteLine("9");
                sw.WriteLine("$DIMAPOST");
                sw.WriteLine("1");
                sw.WriteLine("");
                sw.WriteLine("9");
                sw.WriteLine("$DIMALT");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMALTD");
                sw.WriteLine("70");
                sw.WriteLine("2");
                sw.WriteLine("9");
                sw.WriteLine("$DIMALTF");
                sw.WriteLine("40");
                sw.WriteLine("44311");
                sw.WriteLine("9");
                sw.WriteLine("$DIMLFAC");
                sw.WriteLine("40");
                sw.WriteLine("1.0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTOFL");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTVP");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTIX");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMSOXD");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMSAH");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMBLK1");
                sw.WriteLine("1");
                sw.WriteLine("ARCHTICK");
                sw.WriteLine("9");
                sw.WriteLine("$DIMBLK2");
                sw.WriteLine("1");
                sw.WriteLine("ARCHTICK");
                sw.WriteLine("9");
                sw.WriteLine("$DIMSTYLE");
                sw.WriteLine("2");
                sw.WriteLine("СПДС");
                sw.WriteLine("9");
                sw.WriteLine("$DIMCLRD");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMCLRE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMCLRT");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTFAC");
                sw.WriteLine("40");
                sw.WriteLine("0.75");
                sw.WriteLine("9");
                sw.WriteLine("$DIMGAP");
                sw.WriteLine("40");
                sw.WriteLine("0.625");
                sw.WriteLine("9");
                sw.WriteLine("$LUNITS");
                sw.WriteLine("70");
                sw.WriteLine("2");
                sw.WriteLine("9");
                sw.WriteLine("$LUPREC");
                sw.WriteLine("70");
                sw.WriteLine("4");
                sw.WriteLine("9");
                sw.WriteLine("$SKETCHINC");
                sw.WriteLine("40");
                sw.WriteLine("1.0");
                sw.WriteLine("9");
                sw.WriteLine("$FILLETRAD");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$AUNITS");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$AUPREC");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$MENU");
                sw.WriteLine("1");
                sw.WriteLine(".");
                sw.WriteLine("9");
                sw.WriteLine("$ELEVATION");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$PELEVATION");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$THICKNESS");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$LIMCHECK");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$CHAMFERA");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$CHAMFERB");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$SKPOLY");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$TDCREATE");
                sw.WriteLine("40");
                sw.WriteLine("2454083.075050046");
                sw.WriteLine("9");
                sw.WriteLine("$TDUPDATE");
                sw.WriteLine("40");
                sw.WriteLine("2459240.856584491");
                sw.WriteLine("9");
                sw.WriteLine("$TDINDWG");
                sw.WriteLine("40");
                sw.WriteLine("0.0105753588");
                sw.WriteLine("9");
                sw.WriteLine("$TDUSRTIMER");
                sw.WriteLine("40");
                sw.WriteLine("0.0105751736");
                sw.WriteLine("9");
                sw.WriteLine("$USRTIMER");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$ANGBASE");
                sw.WriteLine("50");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$ANGDIR");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$PDMODE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$PDSIZE");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$PLINEWID");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$COORDS");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$SPLFRAME");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$SPLINETYPE");
                sw.WriteLine("70");
                sw.WriteLine("6");
                sw.WriteLine("9");
                sw.WriteLine("$SPLINESEGS");
                sw.WriteLine("70");
                sw.WriteLine("8");
                sw.WriteLine("9");
                sw.WriteLine("$ATTDIA");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$ATTREQ");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$HANDLING");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$HANDSEED");
                sw.WriteLine("5");
                sw.WriteLine("3BA");
                sw.WriteLine("9");
                sw.WriteLine("$SURFTAB1");
                sw.WriteLine("70");
                sw.WriteLine("6");
                sw.WriteLine("9");
                sw.WriteLine("$SURFTAB2");
                sw.WriteLine("70");
                sw.WriteLine("6");
                sw.WriteLine("9");
                sw.WriteLine("$SURFTYPE");
                sw.WriteLine("70");
                sw.WriteLine("6");
                sw.WriteLine("9");
                sw.WriteLine("$SURFU");
                sw.WriteLine("70");
                sw.WriteLine("6");
                sw.WriteLine("9");
                sw.WriteLine("$SURFV");
                sw.WriteLine("70");
                sw.WriteLine("6");
                sw.WriteLine("9");
                sw.WriteLine("$UCSNAME");
                sw.WriteLine("2");
                sw.WriteLine("");
                sw.WriteLine("9");
                sw.WriteLine("$UCSORG");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$UCSXDIR");
                sw.WriteLine("10");
                sw.WriteLine("1.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$UCSYDIR");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("1.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$PUCSNAME");
                sw.WriteLine("2");
                sw.WriteLine("");
                sw.WriteLine("9");
                sw.WriteLine("$PUCSORG");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$PUCSXDIR");
                sw.WriteLine("10");
                sw.WriteLine("1.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$PUCSYDIR");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("1.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$USERI1");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$USERI2");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$USERI3");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$USERI4");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$USERI5");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$USERR1");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$USERR2");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$USERR3");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$USERR4");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$USERR5");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$WORLDVIEW");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$SHADEDGE");
                sw.WriteLine("70");
                sw.WriteLine("3");
                sw.WriteLine("9");
                sw.WriteLine("$SHADEDIF");
                sw.WriteLine("70");
                sw.WriteLine("70");
                sw.WriteLine("9");
                sw.WriteLine("$TILEMODE");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$MAXACTVP");
                sw.WriteLine("70");
                sw.WriteLine("64");
                sw.WriteLine("9");
                sw.WriteLine("$PLIMCHECK");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$PEXTMIN");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$PEXTMAX");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$PLIMMIN");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$PLIMMAX");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$UNITMODE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$VISRETAIN");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$PLINEGEN");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$PSLTSCALE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("ENDSEC");
                sw.WriteLine("0");
                sw.WriteLine("SECTION");
                sw.WriteLine("2");
                sw.WriteLine("TABLES");
                sw.WriteLine("0");
                sw.WriteLine("TABLE");
                sw.WriteLine("2");
                sw.WriteLine("VPORT");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("0");
                sw.WriteLine("VPORT");
                sw.WriteLine("2");
                sw.WriteLine("*ACTIVE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("11");
                sw.WriteLine("1.0");
                sw.WriteLine("21");
                sw.WriteLine("1.0");
                sw.WriteLine("12");
                sw.WriteLine("7393.85989");
                sw.WriteLine("22");
                sw.WriteLine("12257.763736");
                sw.WriteLine("13");
                sw.WriteLine("0.0");
                sw.WriteLine("23");
                sw.WriteLine("0.0");
                sw.WriteLine("14");
                sw.WriteLine("10.0");
                sw.WriteLine("24");
                sw.WriteLine("10.0");
                sw.WriteLine("15");
                sw.WriteLine("10.0");
                sw.WriteLine("25");
                sw.WriteLine("10.0");
                sw.WriteLine("16");
                sw.WriteLine("0.0");
                sw.WriteLine("26");
                sw.WriteLine("0.0");
                sw.WriteLine("36");
                sw.WriteLine("1.0");
                sw.WriteLine("17");
                sw.WriteLine("210.0");
                sw.WriteLine("27");
                sw.WriteLine("148.5");
                sw.WriteLine("37");
                sw.WriteLine("0.0");
                sw.WriteLine("40");
                sw.WriteLine("47008.928571");
                sw.WriteLine("41");
                sw.WriteLine("2.047436");
                sw.WriteLine("42");
                sw.WriteLine("50.0");
                sw.WriteLine("43");
                sw.WriteLine("0.0");
                sw.WriteLine("44");
                sw.WriteLine("0.0");
                sw.WriteLine("50");
                sw.WriteLine("0.0");
                sw.WriteLine("51");
                sw.WriteLine("0.0");
                sw.WriteLine("71");
                sw.WriteLine("16");
                sw.WriteLine("72");
                sw.WriteLine("1000");
                sw.WriteLine("73");
                sw.WriteLine("1");
                sw.WriteLine("74");
                sw.WriteLine("3");
                sw.WriteLine("75");
                sw.WriteLine("0");
                sw.WriteLine("76");
                sw.WriteLine("0");
                sw.WriteLine("77");
                sw.WriteLine("0");
                sw.WriteLine("78");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("ENDTAB");
                sw.WriteLine("0");
                sw.WriteLine("TABLE");
                sw.WriteLine("2");
                sw.WriteLine("LTYPE");
                sw.WriteLine("70");
                sw.WriteLine("6");
                sw.WriteLine("0");
                sw.WriteLine("LTYPE");
                sw.WriteLine("2");
                sw.WriteLine("CONTINUOUS");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("Solid line");
                sw.WriteLine("72");
                sw.WriteLine("65");
                sw.WriteLine("73");
                sw.WriteLine("0");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("0");
                sw.WriteLine("LTYPE");
                sw.WriteLine("2");
                sw.WriteLine("ГОСТ_2_303_3");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("Сплошная волнистая ~~~~~~~~~~~~~~~~~~~");
                sw.WriteLine("72");
                sw.WriteLine("65");
                sw.WriteLine("73");
                sw.WriteLine("2");
                sw.WriteLine("40");
                sw.WriteLine("26.001");
                sw.WriteLine("49");
                sw.WriteLine("0.001");
                sw.WriteLine("49");
                sw.WriteLine("-26.0");
                sw.WriteLine("0");
                sw.WriteLine("LTYPE");
                sw.WriteLine("2");
                sw.WriteLine("ГОСТ_2_303_5");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("Штрих-пунктирная тонкая ____ _ ____ _ ____ _ __");
                sw.WriteLine("72");
                sw.WriteLine("65");
                sw.WriteLine("73");
                sw.WriteLine("4");
                sw.WriteLine("40");
                sw.WriteLine("24.0");
                sw.WriteLine("49");
                sw.WriteLine("20.0");
                sw.WriteLine("49");
                sw.WriteLine("-1.5");
                sw.WriteLine("49");
                sw.WriteLine("1.0");
                sw.WriteLine("49");
                sw.WriteLine("-1.5");
                sw.WriteLine("0");
                sw.WriteLine("LTYPE");
                sw.WriteLine("2");
                sw.WriteLine("ГОСТ_2_303_4");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("Штриховая __ __ __ __ __ __ __ __ __");
                sw.WriteLine("72");
                sw.WriteLine("65");
                sw.WriteLine("73");
                sw.WriteLine("3");
                sw.WriteLine("40");
                sw.WriteLine("7.0");
                sw.WriteLine("49");
                sw.WriteLine("5.0");
                sw.WriteLine("49");
                sw.WriteLine("-2.0");
                sw.WriteLine("49");
                sw.WriteLine("0.0");
                sw.WriteLine("0");
                sw.WriteLine("LTYPE");
                sw.WriteLine("2");
                sw.WriteLine("ГОСТ_2_303_6");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("Штрих-пунктирная утолщенная ___ _ ___ _ ___ _ _");
                sw.WriteLine("72");
                sw.WriteLine("65");
                sw.WriteLine("73");
                sw.WriteLine("4");
                sw.WriteLine("40");
                sw.WriteLine("12.0");
                sw.WriteLine("49");
                sw.WriteLine("8.0");
                sw.WriteLine("49");
                sw.WriteLine("-1.5");
                sw.WriteLine("49");
                sw.WriteLine("1.0");
                sw.WriteLine("49");
                sw.WriteLine("-1.5");
                sw.WriteLine("0");
                sw.WriteLine("LTYPE");
                sw.WriteLine("2");
                sw.WriteLine("ГОСТ_2_303_8");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("");
                sw.WriteLine("72");
                sw.WriteLine("65");
                sw.WriteLine("73");
                sw.WriteLine("2");
                sw.WriteLine("40");
                sw.WriteLine("48.0");
                sw.WriteLine("49");
                sw.WriteLine("40.0");
                sw.WriteLine("49");
                sw.WriteLine("-8.0");
                sw.WriteLine("0");
                sw.WriteLine("ENDTAB");
                sw.WriteLine("0");
                sw.WriteLine("TABLE");
                sw.WriteLine("2");
                sw.WriteLine("LAYER");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("0");
                sw.WriteLine("LAYER");
                sw.WriteLine("2");
                sw.WriteLine("0");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("62");
                sw.WriteLine("7");
                sw.WriteLine("6");
                sw.WriteLine("CONTINUOUS");
                sw.WriteLine("0");
                sw.WriteLine("ENDTAB");
                sw.WriteLine("0");
                sw.WriteLine("TABLE");
                sw.WriteLine("2");
                sw.WriteLine("STYLE");
                sw.WriteLine("70");
                sw.WriteLine("3");
                sw.WriteLine("0");
                sw.WriteLine("STYLE");
                sw.WriteLine("2");
                sw.WriteLine("STANDARD");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("41");
                sw.WriteLine("1.0");
                sw.WriteLine("50");
                sw.WriteLine("0.0");
                sw.WriteLine("71");
                sw.WriteLine("0");
                sw.WriteLine("42");
                sw.WriteLine("44318");
                sw.WriteLine("3");
                sw.WriteLine("txt.shx");
                sw.WriteLine("4");
                sw.WriteLine("");
                sw.WriteLine("0");
                sw.WriteLine("STYLE");
                sw.WriteLine("2");
                sw.WriteLine("");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("41");
                sw.WriteLine("1.0");
                sw.WriteLine("50");
                sw.WriteLine("0.0");
                sw.WriteLine("71");
                sw.WriteLine("0");
                sw.WriteLine("42");
                sw.WriteLine("0.2");
                sw.WriteLine("3");
                sw.WriteLine("GOST 2.303-68.shx");
                sw.WriteLine("4");
                sw.WriteLine("");
                sw.WriteLine("0");
                sw.WriteLine("STYLE");
                sw.WriteLine("2");
                sw.WriteLine("ГОСТ_2_304");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("41");
                sw.WriteLine("1.0");
                sw.WriteLine("50");
                sw.WriteLine("0.0");
                sw.WriteLine("71");
                sw.WriteLine("0");
                sw.WriteLine("42");
                sw.WriteLine("250.0");
                sw.WriteLine("3");
                sw.WriteLine("CS_Gost2304.shx");
                sw.WriteLine("4");
                sw.WriteLine("");
                sw.WriteLine("0");
                sw.WriteLine("ENDTAB");
                sw.WriteLine("0");
                sw.WriteLine("TABLE");
                sw.WriteLine("2");
                sw.WriteLine("VIEW");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("ENDTAB");
                sw.WriteLine("0");
                sw.WriteLine("TABLE");
                sw.WriteLine("2");
                sw.WriteLine("UCS");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("ENDTAB");
                sw.WriteLine("0");
                sw.WriteLine("TABLE");
                sw.WriteLine("2");
                sw.WriteLine("APPID");
                sw.WriteLine("70");
                sw.WriteLine("14");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("ACAD");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("ACAD_PSEXT");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("ACADANNOPO");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("ACADANNOTATIVE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("ACAD_DSTYLE_DIMJAG");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("ACAD_DSTYLE_DIMTALN");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("ACAD_MLEADERVER");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("MCS_DIM1");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("MCSXDATA5");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("MCS_PARAMS_DATA");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("MCS_PARAMS_HIDDEN_DATA");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("MCS_DOCUMENT_ID");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("MCSUBENTIDENTIFICATION");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("MC_VERSION_DATA");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("ENDTAB");
                sw.WriteLine("0");
                sw.WriteLine("TABLE");
                sw.WriteLine("2");
                sw.WriteLine("DIMSTYLE");
                sw.WriteLine("70");
                sw.WriteLine("6");
                sw.WriteLine("0");
                sw.WriteLine("DIMSTYLE");
                sw.WriteLine("2");
                sw.WriteLine("STANDARD");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("");
                sw.WriteLine("4");
                sw.WriteLine("");
                sw.WriteLine("5");
                sw.WriteLine("");
                sw.WriteLine("6");
                sw.WriteLine("");
                sw.WriteLine("7");
                sw.WriteLine("");
                sw.WriteLine("40");
                sw.WriteLine("100.0");
                sw.WriteLine("41");
                sw.WriteLine("44318");
                sw.WriteLine("42");
                sw.WriteLine("0.5");
                sw.WriteLine("43");
                sw.WriteLine("6.0");
                sw.WriteLine("44");
                sw.WriteLine("0.0");
                sw.WriteLine("45");
                sw.WriteLine("0.0");
                sw.WriteLine("46");
                sw.WriteLine("0.0");
                sw.WriteLine("47");
                sw.WriteLine("0.0");
                sw.WriteLine("48");
                sw.WriteLine("0.0");
                sw.WriteLine("140");
                sw.WriteLine("44318");
                sw.WriteLine("141");
                sw.WriteLine("44318");
                sw.WriteLine("142");
                sw.WriteLine("0.0");
                sw.WriteLine("143");
                sw.WriteLine("44311");
                sw.WriteLine("144");
                sw.WriteLine("1.0");
                sw.WriteLine("145");
                sw.WriteLine("0.0");
                sw.WriteLine("146");
                sw.WriteLine("1.0");
                sw.WriteLine("147");
                sw.WriteLine("1.0");
                sw.WriteLine("71");
                sw.WriteLine("0");
                sw.WriteLine("72");
                sw.WriteLine("0");
                sw.WriteLine("73");
                sw.WriteLine("0");
                sw.WriteLine("74");
                sw.WriteLine("1");
                sw.WriteLine("75");
                sw.WriteLine("0");
                sw.WriteLine("76");
                sw.WriteLine("0");
                sw.WriteLine("77");
                sw.WriteLine("0");
                sw.WriteLine("78");
                sw.WriteLine("0");
                sw.WriteLine("170");
                sw.WriteLine("0");
                sw.WriteLine("171");
                sw.WriteLine("2");
                sw.WriteLine("172");
                sw.WriteLine("0");
                sw.WriteLine("173");
                sw.WriteLine("0");
                sw.WriteLine("174");
                sw.WriteLine("0");
                sw.WriteLine("175");
                sw.WriteLine("0");
                sw.WriteLine("176");
                sw.WriteLine("0");
                sw.WriteLine("177");
                sw.WriteLine("0");
                sw.WriteLine("178");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("DIMSTYLE");
                sw.WriteLine("2");
                sw.WriteLine("СПДС");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("");
                sw.WriteLine("4");
                sw.WriteLine("");
                sw.WriteLine("5");
                sw.WriteLine("ARCHTICK");
                sw.WriteLine("6");
                sw.WriteLine("ARCHTICK");
                sw.WriteLine("7");
                sw.WriteLine("ARCHTICK");
                sw.WriteLine("40");
                sw.WriteLine("100.0");
                sw.WriteLine("41");
                sw.WriteLine("44318");
                sw.WriteLine("42");
                sw.WriteLine("0.625");
                sw.WriteLine("43");
                sw.WriteLine("27454");
                sw.WriteLine("44");
                sw.WriteLine("45658");
                sw.WriteLine("45");
                sw.WriteLine("0.0");
                sw.WriteLine("46");
                sw.WriteLine("2.0");
                sw.WriteLine("47");
                sw.WriteLine("0.0");
                sw.WriteLine("48");
                sw.WriteLine("0.0");
                sw.WriteLine("140");
                sw.WriteLine("44318");
                sw.WriteLine("141");
                sw.WriteLine("44318");
                sw.WriteLine("142");
                sw.WriteLine("0.0");
                sw.WriteLine("143");
                sw.WriteLine("44311");
                sw.WriteLine("144");
                sw.WriteLine("1.0");
                sw.WriteLine("145");
                sw.WriteLine("0.0");
                sw.WriteLine("146");
                sw.WriteLine("0.75");
                sw.WriteLine("147");
                sw.WriteLine("0.625");
                sw.WriteLine("71");
                sw.WriteLine("0");
                sw.WriteLine("72");
                sw.WriteLine("0");
                sw.WriteLine("73");
                sw.WriteLine("0");
                sw.WriteLine("74");
                sw.WriteLine("0");
                sw.WriteLine("75");
                sw.WriteLine("0");
                sw.WriteLine("76");
                sw.WriteLine("0");
                sw.WriteLine("77");
                sw.WriteLine("1");
                sw.WriteLine("78");
                sw.WriteLine("8");
                sw.WriteLine("170");
                sw.WriteLine("0");
                sw.WriteLine("171");
                sw.WriteLine("2");
                sw.WriteLine("172");
                sw.WriteLine("1");
                sw.WriteLine("173");
                sw.WriteLine("0");
                sw.WriteLine("174");
                sw.WriteLine("0");
                sw.WriteLine("175");
                sw.WriteLine("0");
                sw.WriteLine("176");
                sw.WriteLine("0");
                sw.WriteLine("177");
                sw.WriteLine("0");
                sw.WriteLine("178");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("DIMSTYLE");
                sw.WriteLine("2");
                sw.WriteLine("СПДС$2");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("");
                sw.WriteLine("4");
                sw.WriteLine("");
                sw.WriteLine("5");
                sw.WriteLine("OPEN30");
                sw.WriteLine("6");
                sw.WriteLine("OPEN30");
                sw.WriteLine("7");
                sw.WriteLine("OPEN30");
                sw.WriteLine("40");
                sw.WriteLine("1.0");
                sw.WriteLine("41");
                sw.WriteLine("44318");
                sw.WriteLine("42");
                sw.WriteLine("0.625");
                sw.WriteLine("43");
                sw.WriteLine("27454");
                sw.WriteLine("44");
                sw.WriteLine("0.18");
                sw.WriteLine("45");
                sw.WriteLine("0.0");
                sw.WriteLine("46");
                sw.WriteLine("0.0");
                sw.WriteLine("47");
                sw.WriteLine("0.0");
                sw.WriteLine("48");
                sw.WriteLine("0.0");
                sw.WriteLine("140");
                sw.WriteLine("44318");
                sw.WriteLine("141");
                sw.WriteLine("0.09");
                sw.WriteLine("142");
                sw.WriteLine("0.0");
                sw.WriteLine("143");
                sw.WriteLine("44311");
                sw.WriteLine("144");
                sw.WriteLine("1.0");
                sw.WriteLine("145");
                sw.WriteLine("0.0");
                sw.WriteLine("146");
                sw.WriteLine("1.0");
                sw.WriteLine("147");
                sw.WriteLine("0.625");
                sw.WriteLine("71");
                sw.WriteLine("0");
                sw.WriteLine("72");
                sw.WriteLine("0");
                sw.WriteLine("73");
                sw.WriteLine("0");
                sw.WriteLine("74");
                sw.WriteLine("1");
                sw.WriteLine("75");
                sw.WriteLine("0");
                sw.WriteLine("76");
                sw.WriteLine("0");
                sw.WriteLine("77");
                sw.WriteLine("1");
                sw.WriteLine("78");
                sw.WriteLine("0");
                sw.WriteLine("170");
                sw.WriteLine("0");
                sw.WriteLine("171");
                sw.WriteLine("2");
                sw.WriteLine("172");
                sw.WriteLine("0");
                sw.WriteLine("173");
                sw.WriteLine("0");
                sw.WriteLine("174");
                sw.WriteLine("0");
                sw.WriteLine("175");
                sw.WriteLine("0");
                sw.WriteLine("176");
                sw.WriteLine("0");
                sw.WriteLine("177");
                sw.WriteLine("0");
                sw.WriteLine("178");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("DIMSTYLE");
                sw.WriteLine("2");
                sw.WriteLine("СПДС$3");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("");
                sw.WriteLine("4");
                sw.WriteLine("");
                sw.WriteLine("5");
                sw.WriteLine("OPEN30");
                sw.WriteLine("6");
                sw.WriteLine("OPEN30");
                sw.WriteLine("7");
                sw.WriteLine("OPEN30");
                sw.WriteLine("40");
                sw.WriteLine("1.0");
                sw.WriteLine("41");
                sw.WriteLine("44318");
                sw.WriteLine("42");
                sw.WriteLine("0.0625");
                sw.WriteLine("43");
                sw.WriteLine("0.38");
                sw.WriteLine("44");
                sw.WriteLine("0.18");
                sw.WriteLine("45");
                sw.WriteLine("0.0");
                sw.WriteLine("46");
                sw.WriteLine("0.0");
                sw.WriteLine("47");
                sw.WriteLine("0.0");
                sw.WriteLine("48");
                sw.WriteLine("0.0");
                sw.WriteLine("140");
                sw.WriteLine("44318");
                sw.WriteLine("141");
                sw.WriteLine("44318");
                sw.WriteLine("142");
                sw.WriteLine("0.0");
                sw.WriteLine("143");
                sw.WriteLine("44311");
                sw.WriteLine("144");
                sw.WriteLine("1.0");
                sw.WriteLine("145");
                sw.WriteLine("0.0");
                sw.WriteLine("146");
                sw.WriteLine("1.0");
                sw.WriteLine("147");
                sw.WriteLine("0.625");
                sw.WriteLine("71");
                sw.WriteLine("0");
                sw.WriteLine("72");
                sw.WriteLine("0");
                sw.WriteLine("73");
                sw.WriteLine("0");
                sw.WriteLine("74");
                sw.WriteLine("1");
                sw.WriteLine("75");
                sw.WriteLine("0");
                sw.WriteLine("76");
                sw.WriteLine("0");
                sw.WriteLine("77");
                sw.WriteLine("1");
                sw.WriteLine("78");
                sw.WriteLine("0");
                sw.WriteLine("170");
                sw.WriteLine("0");
                sw.WriteLine("171");
                sw.WriteLine("2");
                sw.WriteLine("172");
                sw.WriteLine("1");
                sw.WriteLine("173");
                sw.WriteLine("0");
                sw.WriteLine("174");
                sw.WriteLine("0");
                sw.WriteLine("175");
                sw.WriteLine("0");
                sw.WriteLine("176");
                sw.WriteLine("0");
                sw.WriteLine("177");
                sw.WriteLine("0");
                sw.WriteLine("178");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("DIMSTYLE");
                sw.WriteLine("2");
                sw.WriteLine("СПДС$7");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("");
                sw.WriteLine("4");
                sw.WriteLine("");
                sw.WriteLine("5");
                sw.WriteLine("");
                sw.WriteLine("6");
                sw.WriteLine("");
                sw.WriteLine("7");
                sw.WriteLine("");
                sw.WriteLine("40");
                sw.WriteLine("1.0");
                sw.WriteLine("41");
                sw.WriteLine("1.0");
                sw.WriteLine("42");
                sw.WriteLine("0.0625");
                sw.WriteLine("43");
                sw.WriteLine("0.38");
                sw.WriteLine("44");
                sw.WriteLine("0.18");
                sw.WriteLine("45");
                sw.WriteLine("0.0");
                sw.WriteLine("46");
                sw.WriteLine("0.0");
                sw.WriteLine("47");
                sw.WriteLine("0.0");
                sw.WriteLine("48");
                sw.WriteLine("0.0");
                sw.WriteLine("140");
                sw.WriteLine("44318");
                sw.WriteLine("141");
                sw.WriteLine("0.09");
                sw.WriteLine("142");
                sw.WriteLine("0.0");
                sw.WriteLine("143");
                sw.WriteLine("44311");
                sw.WriteLine("144");
                sw.WriteLine("1.0");
                sw.WriteLine("145");
                sw.WriteLine("0.0");
                sw.WriteLine("146");
                sw.WriteLine("1.0");
                sw.WriteLine("147");
                sw.WriteLine("0.625");
                sw.WriteLine("71");
                sw.WriteLine("0");
                sw.WriteLine("72");
                sw.WriteLine("0");
                sw.WriteLine("73");
                sw.WriteLine("0");
                sw.WriteLine("74");
                sw.WriteLine("1");
                sw.WriteLine("75");
                sw.WriteLine("0");
                sw.WriteLine("76");
                sw.WriteLine("0");
                sw.WriteLine("77");
                sw.WriteLine("1");
                sw.WriteLine("78");
                sw.WriteLine("0");
                sw.WriteLine("170");
                sw.WriteLine("0");
                sw.WriteLine("171");
                sw.WriteLine("2");
                sw.WriteLine("172");
                sw.WriteLine("0");
                sw.WriteLine("173");
                sw.WriteLine("0");
                sw.WriteLine("174");
                sw.WriteLine("0");
                sw.WriteLine("175");
                sw.WriteLine("0");
                sw.WriteLine("176");
                sw.WriteLine("0");
                sw.WriteLine("177");
                sw.WriteLine("0");
                sw.WriteLine("178");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("DIMSTYLE");
                sw.WriteLine("2");
                sw.WriteLine("СПДС$4");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("");
                sw.WriteLine("4");
                sw.WriteLine("");
                sw.WriteLine("5");
                sw.WriteLine("OPEN30");
                sw.WriteLine("6");
                sw.WriteLine("OPEN30");
                sw.WriteLine("7");
                sw.WriteLine("OPEN30");
                sw.WriteLine("40");
                sw.WriteLine("1.0");
                sw.WriteLine("41");
                sw.WriteLine("44318");
                sw.WriteLine("42");
                sw.WriteLine("0.0625");
                sw.WriteLine("43");
                sw.WriteLine("0.38");
                sw.WriteLine("44");
                sw.WriteLine("0.18");
                sw.WriteLine("45");
                sw.WriteLine("0.0");
                sw.WriteLine("46");
                sw.WriteLine("0.0");
                sw.WriteLine("47");
                sw.WriteLine("0.0");
                sw.WriteLine("48");
                sw.WriteLine("0.0");
                sw.WriteLine("140");
                sw.WriteLine("44318");
                sw.WriteLine("141");
                sw.WriteLine("44318");
                sw.WriteLine("142");
                sw.WriteLine("0.0");
                sw.WriteLine("143");
                sw.WriteLine("44311");
                sw.WriteLine("144");
                sw.WriteLine("1.0");
                sw.WriteLine("145");
                sw.WriteLine("0.0");
                sw.WriteLine("146");
                sw.WriteLine("1.0");
                sw.WriteLine("147");
                sw.WriteLine("0.625");
                sw.WriteLine("71");
                sw.WriteLine("0");
                sw.WriteLine("72");
                sw.WriteLine("0");
                sw.WriteLine("73");
                sw.WriteLine("0");
                sw.WriteLine("74");
                sw.WriteLine("1");
                sw.WriteLine("75");
                sw.WriteLine("0");
                sw.WriteLine("76");
                sw.WriteLine("0");
                sw.WriteLine("77");
                sw.WriteLine("1");
                sw.WriteLine("78");
                sw.WriteLine("0");
                sw.WriteLine("170");
                sw.WriteLine("0");
                sw.WriteLine("171");
                sw.WriteLine("2");
                sw.WriteLine("172");
                sw.WriteLine("1");
                sw.WriteLine("173");
                sw.WriteLine("0");
                sw.WriteLine("174");
                sw.WriteLine("0");
                sw.WriteLine("175");
                sw.WriteLine("0");
                sw.WriteLine("176");
                sw.WriteLine("0");
                sw.WriteLine("177");
                sw.WriteLine("0");
                sw.WriteLine("178");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("ENDTAB");
                sw.WriteLine("0");
                sw.WriteLine("ENDSEC");
                sw.WriteLine("0");
                sw.WriteLine("SECTION");
                sw.WriteLine("2");
                sw.WriteLine("BLOCKS");
                sw.WriteLine("0");
                sw.WriteLine("BLOCK");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("2");
                sw.WriteLine("$MODEL_SPACE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("3");
                sw.WriteLine("$MODEL_SPACE");
                sw.WriteLine("1");
                sw.WriteLine("");
                sw.WriteLine("0");
                sw.WriteLine("ENDBLK");
                sw.WriteLine("5");
                sw.WriteLine("21");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("BLOCK");
                sw.WriteLine("67");
                sw.WriteLine("1");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("2");
                sw.WriteLine("$PAPER_SPACE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("3");
                sw.WriteLine("$PAPER_SPACE");
                sw.WriteLine("1");
                sw.WriteLine("");
                sw.WriteLine("0");
                sw.WriteLine("ENDBLK");
                sw.WriteLine("5");
                sw.WriteLine("227");
                sw.WriteLine("67");
                sw.WriteLine("1");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("BLOCK");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("2");
                sw.WriteLine("_ARCHTICK");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("3");
                sw.WriteLine("_ARCHTICK");
                sw.WriteLine("1");
                sw.WriteLine("");
                sw.WriteLine("0");
                sw.WriteLine("POLYLINE");
                sw.WriteLine("5");
                sw.WriteLine("1EF");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("66");
                sw.WriteLine("1");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("40");
                sw.WriteLine("0.15");
                sw.WriteLine("41");
                sw.WriteLine("0.15");
                sw.WriteLine("0");
                sw.WriteLine("VERTEX");
                sw.WriteLine("5");
                sw.WriteLine("1F0");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("-0.5");
                sw.WriteLine("20");
                sw.WriteLine("-0.5");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("0");
                sw.WriteLine("VERTEX");
                sw.WriteLine("5");
                sw.WriteLine("1F1");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.5");
                sw.WriteLine("20");
                sw.WriteLine("0.5");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("0");
                sw.WriteLine("SEQEND");
                sw.WriteLine("5");
                sw.WriteLine("1F2");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("ENDBLK");
                sw.WriteLine("5");
                sw.WriteLine("1EE");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("BLOCK");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("2");
                sw.WriteLine("_OPEN30");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("3");
                sw.WriteLine("_OPEN30");
                sw.WriteLine("1");
                sw.WriteLine("");
                sw.WriteLine("0");
                sw.WriteLine("LINE");
                sw.WriteLine("5");
                sw.WriteLine("1F7");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("-1.0");
                sw.WriteLine("20");
                sw.WriteLine("0.267949");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("11");
                sw.WriteLine("0.0");
                sw.WriteLine("21");
                sw.WriteLine("0.0");
                sw.WriteLine("31");
                sw.WriteLine("0.0");
                sw.WriteLine("0");
                sw.WriteLine("LINE");
                sw.WriteLine("5");
                sw.WriteLine("1F8");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("11");
                sw.WriteLine("-1.0");
                sw.WriteLine("21");
                sw.WriteLine("-0.267949");
                sw.WriteLine("31");
                sw.WriteLine("0.0");
                sw.WriteLine("0");
                sw.WriteLine("LINE");
                sw.WriteLine("5");
                sw.WriteLine("1F9");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("11");
                sw.WriteLine("-1.0");
                sw.WriteLine("21");
                sw.WriteLine("0.0");
                sw.WriteLine("31");
                sw.WriteLine("0.0");
                sw.WriteLine("0");
                sw.WriteLine("ENDBLK");
                sw.WriteLine("5");
                sw.WriteLine("1F6");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("BLOCK");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("2");
                sw.WriteLine("_DOT");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("3");
                sw.WriteLine("_DOT");
                sw.WriteLine("1");
                sw.WriteLine("");
                sw.WriteLine("0");
                sw.WriteLine("POLYLINE");
                sw.WriteLine("5");
                sw.WriteLine("1FF");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("66");
                sw.WriteLine("1");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("40");
                sw.WriteLine("0.5");
                sw.WriteLine("41");
                sw.WriteLine("0.5");
                sw.WriteLine("0");
                sw.WriteLine("VERTEX");
                sw.WriteLine("5");
                sw.WriteLine("201");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("-0.25");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("42");
                sw.WriteLine("1.0");
                sw.WriteLine("0");
                sw.WriteLine("VERTEX");
                sw.WriteLine("5");
                sw.WriteLine("202");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.25");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("42");
                sw.WriteLine("1.0");
                sw.WriteLine("0");
                sw.WriteLine("SEQEND");
                sw.WriteLine("5");
                sw.WriteLine("203");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("LINE");
                sw.WriteLine("5");
                sw.WriteLine("200");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("-0.5");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("11");
                sw.WriteLine("-1.0");
                sw.WriteLine("21");
                sw.WriteLine("0.0");
                sw.WriteLine("31");
                sw.WriteLine("0.0");
                sw.WriteLine("0");
                sw.WriteLine("ENDBLK");
                sw.WriteLine("5");
                sw.WriteLine("1FE");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("ENDSEC");
                sw.WriteLine("0");
                sw.WriteLine("SECTION");
                sw.WriteLine("2");
                sw.WriteLine("ENTITIES");
                sw.WriteLine("0");
            }
            using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
            {

                for (int d = 1; d < pipe1.points.Count; d++)
                {
                    sw.WriteLine("LINE");
                    sw.WriteLine("5");
                    sw.WriteLine("3B0");
                    sw.WriteLine("8");
                    sw.WriteLine("0");
                    sw.WriteLine("62");
                    sw.WriteLine("5");
                    sw.WriteLine("10");
                    sw.WriteLine(pipe1.points[d - 1].x);
                    sw.WriteLine("20");
                    sw.WriteLine(pipe1.points[d - 1].y);
                    sw.WriteLine("30");
                    sw.WriteLine("0.0");
                    sw.WriteLine("11");
                    sw.WriteLine(pipe1.points[d].x);
                    sw.WriteLine("21");
                    sw.WriteLine(pipe1.points[d].y);
                    sw.WriteLine("31");
                    sw.WriteLine("0.0");
                    sw.WriteLine("0");
                    Console.Write(".");//прогресс-индикатор для отслеживания процесса обработки массива
                }

            }
            //Вбиваем в файл информацию об объекте, заключительный отрезок и отрезок, обозначающий место начала построения
            using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
            {
                sw.WriteLine("TEXT");
                sw.WriteLine("5");
                sw.WriteLine("3B1");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("CONTINUOUS");
                sw.WriteLine("10");
                sw.WriteLine("0");//координата X
                sw.WriteLine("20");
                sw.WriteLine("1000");//координата Y
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("40");
                sw.WriteLine("40.0");//размер шрифта
                sw.WriteLine("1");
                sw.WriteLine($"Control object number: {Pipename1}.");
                sw.WriteLine("7");
                sw.WriteLine("ГОСТ_2_304");
                sw.WriteLine("0");
                //Добавляем заключительный отрезок, который соединит начало и конец
                {
                    sw.WriteLine("LINE");
                    sw.WriteLine("5");
                    sw.WriteLine("3B0");
                    sw.WriteLine("8");
                    sw.WriteLine("0");
                    sw.WriteLine("62");
                    sw.WriteLine("5");
                    sw.WriteLine("10");
                    sw.WriteLine(pipe1.points[0].x);
                    sw.WriteLine("20");
                    sw.WriteLine(pipe1.points[0].y);
                    sw.WriteLine("30");
                    sw.WriteLine("0.0");
                    sw.WriteLine("11");
                    sw.WriteLine(pipe1.points[pipe1.points.Count - 1].x);
                    sw.WriteLine("21");
                    sw.WriteLine(pipe1.points[pipe1.points.Count - 1].y);
                    sw.WriteLine("31");
                    sw.WriteLine("0.0");
                    sw.WriteLine("0");
                }
                //Добавляем отрезок, обозначающий место начала построения (предполагается, что здесь продольный шов)
                {
                    sw.WriteLine("LINE");
                    sw.WriteLine("5");
                    sw.WriteLine("3B0");
                    sw.WriteLine("8");
                    sw.WriteLine("0");
                    sw.WriteLine("62");
                    sw.WriteLine("5");
                    sw.WriteLine("10");
                    sw.WriteLine(pipe1.points[0].x);
                    sw.WriteLine("20");
                    sw.WriteLine(pipe1.points[0].y);
                    sw.WriteLine("30");
                    sw.WriteLine("0.0");
                    sw.WriteLine("11");
                    sw.WriteLine(0);
                    sw.WriteLine("21");
                    sw.WriteLine(0);
                    sw.WriteLine("31");
                    sw.WriteLine("0.0");
                    sw.WriteLine("0");
                }
            }

            using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
                //вбиваем данные о второй трубе
                for (int d = 1; d < pipe2.points.Count; d++)
                {

                    sw.WriteLine("LINE");
                    sw.WriteLine("5");
                    sw.WriteLine("3B0");
                    sw.WriteLine("8");
                    sw.WriteLine("0");
                    sw.WriteLine("62");
                    sw.WriteLine("1");//цвет?
                    sw.WriteLine("10");
                    sw.WriteLine(pipe2.points[d - 1].x);
                    sw.WriteLine("20");
                    sw.WriteLine(pipe2.points[d - 1].y);
                    sw.WriteLine("30");
                    sw.WriteLine("0.0");
                    sw.WriteLine("11");
                    sw.WriteLine(pipe2.points[d].x);
                    sw.WriteLine("21");
                    sw.WriteLine(pipe2.points[d].y);
                    sw.WriteLine("31");
                    sw.WriteLine("0.0");
                    sw.WriteLine("0");
                    // Console.Write(".");//прогресс-индикатор для отслеживания процесса обработки массива
                }

            using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
            {
                sw.WriteLine("TEXT");
                sw.WriteLine("5");
                sw.WriteLine("3B1");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("CONTINUOUS");
                sw.WriteLine("10");
                sw.WriteLine("0");//координата X
                sw.WriteLine("20");
                sw.WriteLine("900");//координата Y
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("40");
                sw.WriteLine("40.0");//размер шрифта
                sw.WriteLine("1");
                sw.WriteLine($"Control object number: {Pipename2}.");
                sw.WriteLine("7");
                sw.WriteLine("ГОСТ_2_304");
                sw.WriteLine("0");
                //Добавляем заключительный отрезок, который соединит начало и конец
                {
                    sw.WriteLine("LINE");
                    sw.WriteLine("5");
                    sw.WriteLine("3B0");
                    sw.WriteLine("8");
                    sw.WriteLine("0");
                    sw.WriteLine("62");
                    sw.WriteLine("1");
                    sw.WriteLine("10");
                    sw.WriteLine(pipe2.points[0].x);
                    sw.WriteLine("20");
                    sw.WriteLine(pipe2.points[0].y);
                    sw.WriteLine("30");
                    sw.WriteLine("0.0");
                    sw.WriteLine("11");
                    sw.WriteLine(pipe2.points[pipe2.points.Count - 1].x);
                    sw.WriteLine("21");
                    sw.WriteLine(pipe2.points[pipe2.points.Count - 1].y);
                    sw.WriteLine("31");
                    sw.WriteLine("0.0");
                    sw.WriteLine("0");


                    sw.WriteLine("LINE");
                    sw.WriteLine("5");
                    sw.WriteLine("3B0");
                    sw.WriteLine("8");
                    sw.WriteLine("0");
                    sw.WriteLine("62");
                    sw.WriteLine("5");
                    sw.WriteLine("10");
                    sw.WriteLine(-300);
                    sw.WriteLine("20");
                    sw.WriteLine(1000);
                    sw.WriteLine("30");
                    sw.WriteLine("0.0");
                    sw.WriteLine("11");
                    sw.WriteLine(0);
                    sw.WriteLine("21");
                    sw.WriteLine(1000);
                    sw.WriteLine("31");
                    sw.WriteLine("0.0");
                    sw.WriteLine("0");


                    sw.WriteLine("LINE");
                    sw.WriteLine("5");
                    sw.WriteLine("3B0");
                    sw.WriteLine("8");
                    sw.WriteLine("0");
                    sw.WriteLine("62");
                    sw.WriteLine("1");
                    sw.WriteLine("10");
                    sw.WriteLine(-300);
                    sw.WriteLine("20");
                    sw.WriteLine(900);
                    sw.WriteLine("30");
                    sw.WriteLine("0.0");
                    sw.WriteLine("11");
                    sw.WriteLine(0);
                    sw.WriteLine("21");
                    sw.WriteLine(900);
                    sw.WriteLine("31");
                    sw.WriteLine("0.0");
                    sw.WriteLine("0");

                    sw.WriteLine("TEXT");
                    sw.WriteLine("5");
                    sw.WriteLine("3B1");
                    sw.WriteLine("8");
                    sw.WriteLine("0");
                    sw.WriteLine("6");
                    sw.WriteLine("CONTINUOUS");
                    sw.WriteLine("10");
                    sw.WriteLine("-300");//координата X
                    sw.WriteLine("20");
                    sw.WriteLine("800");//координата Y
                    sw.WriteLine("30");
                    sw.WriteLine("0.0");
                    sw.WriteLine("40");
                    sw.WriteLine("40.0");//размер шрифта
                    sw.WriteLine("1");
                    sw.WriteLine($"Optimal angle between zero points: {corner}, arc distance {Math.Truncate((corner * Math.PI * Convert.ToDouble(textBox1.Text)) / 360)}.");
                    sw.WriteLine("7");
                    sw.WriteLine("ГОСТ_2_304");
                    sw.WriteLine("0");
                }
                //Добавляем отрезок, обозначающий место начала построения (предполагается, что здесь продольный шов)
                {
                    sw.WriteLine("LINE");
                    sw.WriteLine("5");
                    sw.WriteLine("3B0");
                    sw.WriteLine("8");
                    sw.WriteLine("0");
                    sw.WriteLine("62");
                    sw.WriteLine("1");
                    sw.WriteLine("10");
                    sw.WriteLine(pipe2.points[0].x);
                    sw.WriteLine("20");
                    sw.WriteLine(pipe2.points[0].y);
                    sw.WriteLine("30");
                    sw.WriteLine("0.0");
                    sw.WriteLine("11");
                    sw.WriteLine(0);
                    sw.WriteLine("21");
                    sw.WriteLine(0);
                    sw.WriteLine("31");
                    sw.WriteLine("0.0");
                    sw.WriteLine("0");
                }
            }
            //Вбиваем в файл конечную галиматью
            using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
            {
                sw.WriteLine("VIEWPORT");
                sw.WriteLine("5");
                sw.WriteLine("228");
                sw.WriteLine("67");
                sw.WriteLine("1");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("40");
                sw.WriteLine("2172.474887");
                sw.WriteLine("41");
                sw.WriteLine("992.070339");
                sw.WriteLine("68");
                sw.WriteLine("1");
                sw.WriteLine("69");
                sw.WriteLine("1");
                sw.WriteLine("1001");
                sw.WriteLine("ACAD");
                sw.WriteLine("1000");
                sw.WriteLine("MVIEW");
                sw.WriteLine("1002");
                sw.WriteLine("{");
                sw.WriteLine("1070");
                sw.WriteLine("16");
                sw.WriteLine("1010");
                sw.WriteLine("0.0");
                sw.WriteLine("1020");
                sw.WriteLine("0.0");
                sw.WriteLine("1030");
                sw.WriteLine("0.0");
                sw.WriteLine("1020");
                sw.WriteLine("0.0");
                sw.WriteLine("1030");
                sw.WriteLine("0.0");
                sw.WriteLine("1010");
                sw.WriteLine("0.0");
                sw.WriteLine("1020");
                sw.WriteLine("0.0");
                sw.WriteLine("1030");
                sw.WriteLine("1.0");
                sw.WriteLine("1020");
                sw.WriteLine("0.0");
                sw.WriteLine("1030");
                sw.WriteLine("1.0");
                sw.WriteLine("1040");
                sw.WriteLine("0.0");
                sw.WriteLine("1040");
                sw.WriteLine("992.070339");
                sw.WriteLine("1040");
                sw.WriteLine("497.20797");
                sw.WriteLine("1040");
                sw.WriteLine("380.27835");
                sw.WriteLine("1040");
                sw.WriteLine("50.0");
                sw.WriteLine("1040");
                sw.WriteLine("0.0");
                sw.WriteLine("1040");
                sw.WriteLine("0.0");
                sw.WriteLine("1070");
                sw.WriteLine("16");
                sw.WriteLine("1070");
                sw.WriteLine("100");
                sw.WriteLine("1070");
                sw.WriteLine("0");
                sw.WriteLine("1070");
                sw.WriteLine("3");
                sw.WriteLine("1070");
                sw.WriteLine("0");
                sw.WriteLine("1070");
                sw.WriteLine("0");
                sw.WriteLine("1070");
                sw.WriteLine("0");
                sw.WriteLine("1070");
                sw.WriteLine("0");
                sw.WriteLine("1040");
                sw.WriteLine("0.0");
                sw.WriteLine("1040");
                sw.WriteLine("0.0");
                sw.WriteLine("1040");
                sw.WriteLine("0.0");
                sw.WriteLine("1040");
                sw.WriteLine("10.0");
                sw.WriteLine("1040");
                sw.WriteLine("10.0");
                sw.WriteLine("1040");
                sw.WriteLine("10.0");
                sw.WriteLine("1040");
                sw.WriteLine("10.0");
                sw.WriteLine("1070");
                sw.WriteLine("0");
                sw.WriteLine("1002");
                sw.WriteLine("{");
                sw.WriteLine("1002");
                sw.WriteLine("}");
                sw.WriteLine("1002");
                sw.WriteLine("}");
                sw.WriteLine("0");
                sw.WriteLine("ENDSEC");
                sw.WriteLine("0");
                sw.WriteLine("EOF");
            }

            //richTextBox1.AppendText(Environment.NewLine +"" ); 
            //richTextBox1.AppendText(Environment.NewLine + "Формирование файла с результатами вычислений выполнено.");
        }
        private void OnePipeToFile(PipeDecart pipe1, string Pipename1, Point min1, Point min2, Point max1, Point max2, string filename, double DiametrMax, double DiametrMin)
        {
            string writePath = textBox2.Text + filename + ".dxf";
            using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
            {
                sw.WriteLine("0");
                sw.WriteLine("SECTION");
                sw.WriteLine("2");
                sw.WriteLine("HEADER");
                sw.WriteLine("9");
                sw.WriteLine("$ACADVER");
                sw.WriteLine("1");
                sw.WriteLine("AC1009");
                sw.WriteLine("9");
                sw.WriteLine("$DWGCODEPAGE");
                sw.WriteLine("3");
                sw.WriteLine("ansi_1251");
                sw.WriteLine("9");
                sw.WriteLine("$INSBASE");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$EXTMIN");
                sw.WriteLine("10");
                sw.WriteLine("1.000000E+20");
                sw.WriteLine("20");
                sw.WriteLine("1.000000E+20");
                sw.WriteLine("30");
                sw.WriteLine("1.000000E+20");
                sw.WriteLine("9");
                sw.WriteLine("$EXTMAX");
                sw.WriteLine("10");
                sw.WriteLine("-1.000000E+20");
                sw.WriteLine("20");
                sw.WriteLine("-1.000000E+20");
                sw.WriteLine("30");
                sw.WriteLine("-1.000000E+20");
                sw.WriteLine("9");
                sw.WriteLine("$LIMMIN");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$LIMMAX");
                sw.WriteLine("10");
                sw.WriteLine("84100.0");
                sw.WriteLine("20");
                sw.WriteLine("59400.0");
                sw.WriteLine("9");
                sw.WriteLine("$ORTHOMODE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$REGENMODE");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$FILLMODE");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$QTEXTMODE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$MIRRTEXT");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DRAGMODE");
                sw.WriteLine("70");
                sw.WriteLine("2");
                sw.WriteLine("9");
                sw.WriteLine("$LTSCALE");
                sw.WriteLine("40");
                sw.WriteLine("1.0");
                sw.WriteLine("9");
                sw.WriteLine("$OSMODE");
                sw.WriteLine("70");
                sw.WriteLine("37");
                sw.WriteLine("9");
                sw.WriteLine("$ATTMODE");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$TEXTSIZE");
                sw.WriteLine("40");
                sw.WriteLine("250.0");
                sw.WriteLine("9");
                sw.WriteLine("$TRACEWID");
                sw.WriteLine("40");
                sw.WriteLine("1.0");
                sw.WriteLine("9");
                sw.WriteLine("$TEXTSTYLE");
                sw.WriteLine("7");
                sw.WriteLine("ГОСТ_2_304");
                sw.WriteLine("9");
                sw.WriteLine("$CLAYER");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$CELTYPE");
                sw.WriteLine("6");
                sw.WriteLine("BYLAYER");
                sw.WriteLine("9");
                sw.WriteLine("$CECOLOR");
                sw.WriteLine("62");
                sw.WriteLine("256");
                sw.WriteLine("9");
                sw.WriteLine("$DIMSCALE");
                sw.WriteLine("40");
                sw.WriteLine("100.0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMASZ");
                sw.WriteLine("40");
                sw.WriteLine("44318");
                sw.WriteLine("9");
                sw.WriteLine("$DIMEXO");
                sw.WriteLine("40");
                sw.WriteLine("0.625");
                sw.WriteLine("9");
                sw.WriteLine("$DIMDLI");
                sw.WriteLine("40");
                sw.WriteLine("27454");
                sw.WriteLine("9");
                sw.WriteLine("$DIMRND");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMDLE");
                sw.WriteLine("40");
                sw.WriteLine("2.0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMEXE");
                sw.WriteLine("40");
                sw.WriteLine("45658");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTP");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTM");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTXT");
                sw.WriteLine("40");
                sw.WriteLine("44318");
                sw.WriteLine("9");
                sw.WriteLine("$DIMCEN");
                sw.WriteLine("40");
                sw.WriteLine("44318");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTSZ");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTOL");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMLIM");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTIH");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTOH");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMSE1");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMSE2");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTAD");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$DIMZIN");
                sw.WriteLine("70");
                sw.WriteLine("8");
                sw.WriteLine("9");
                sw.WriteLine("$DIMBLK");
                sw.WriteLine("1");
                sw.WriteLine("ARCHTICK");
                sw.WriteLine("9");
                sw.WriteLine("$DIMASO");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$DIMSHO");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$DIMPOST");
                sw.WriteLine("1");
                sw.WriteLine("");
                sw.WriteLine("9");
                sw.WriteLine("$DIMAPOST");
                sw.WriteLine("1");
                sw.WriteLine("");
                sw.WriteLine("9");
                sw.WriteLine("$DIMALT");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMALTD");
                sw.WriteLine("70");
                sw.WriteLine("2");
                sw.WriteLine("9");
                sw.WriteLine("$DIMALTF");
                sw.WriteLine("40");
                sw.WriteLine("44311");
                sw.WriteLine("9");
                sw.WriteLine("$DIMLFAC");
                sw.WriteLine("40");
                sw.WriteLine("1.0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTOFL");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTVP");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTIX");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMSOXD");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMSAH");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMBLK1");
                sw.WriteLine("1");
                sw.WriteLine("ARCHTICK");
                sw.WriteLine("9");
                sw.WriteLine("$DIMBLK2");
                sw.WriteLine("1");
                sw.WriteLine("ARCHTICK");
                sw.WriteLine("9");
                sw.WriteLine("$DIMSTYLE");
                sw.WriteLine("2");
                sw.WriteLine("СПДС");
                sw.WriteLine("9");
                sw.WriteLine("$DIMCLRD");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMCLRE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMCLRT");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$DIMTFAC");
                sw.WriteLine("40");
                sw.WriteLine("0.75");
                sw.WriteLine("9");
                sw.WriteLine("$DIMGAP");
                sw.WriteLine("40");
                sw.WriteLine("0.625");
                sw.WriteLine("9");
                sw.WriteLine("$LUNITS");
                sw.WriteLine("70");
                sw.WriteLine("2");
                sw.WriteLine("9");
                sw.WriteLine("$LUPREC");
                sw.WriteLine("70");
                sw.WriteLine("4");
                sw.WriteLine("9");
                sw.WriteLine("$SKETCHINC");
                sw.WriteLine("40");
                sw.WriteLine("1.0");
                sw.WriteLine("9");
                sw.WriteLine("$FILLETRAD");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$AUNITS");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$AUPREC");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$MENU");
                sw.WriteLine("1");
                sw.WriteLine(".");
                sw.WriteLine("9");
                sw.WriteLine("$ELEVATION");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$PELEVATION");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$THICKNESS");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$LIMCHECK");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$CHAMFERA");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$CHAMFERB");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$SKPOLY");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$TDCREATE");
                sw.WriteLine("40");
                sw.WriteLine("2454083.075050046");
                sw.WriteLine("9");
                sw.WriteLine("$TDUPDATE");
                sw.WriteLine("40");
                sw.WriteLine("2459240.856584491");
                sw.WriteLine("9");
                sw.WriteLine("$TDINDWG");
                sw.WriteLine("40");
                sw.WriteLine("0.0105753588");
                sw.WriteLine("9");
                sw.WriteLine("$TDUSRTIMER");
                sw.WriteLine("40");
                sw.WriteLine("0.0105751736");
                sw.WriteLine("9");
                sw.WriteLine("$USRTIMER");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$ANGBASE");
                sw.WriteLine("50");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$ANGDIR");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$PDMODE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$PDSIZE");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$PLINEWID");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$COORDS");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$SPLFRAME");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$SPLINETYPE");
                sw.WriteLine("70");
                sw.WriteLine("6");
                sw.WriteLine("9");
                sw.WriteLine("$SPLINESEGS");
                sw.WriteLine("70");
                sw.WriteLine("8");
                sw.WriteLine("9");
                sw.WriteLine("$ATTDIA");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$ATTREQ");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$HANDLING");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$HANDSEED");
                sw.WriteLine("5");
                sw.WriteLine("3BA");
                sw.WriteLine("9");
                sw.WriteLine("$SURFTAB1");
                sw.WriteLine("70");
                sw.WriteLine("6");
                sw.WriteLine("9");
                sw.WriteLine("$SURFTAB2");
                sw.WriteLine("70");
                sw.WriteLine("6");
                sw.WriteLine("9");
                sw.WriteLine("$SURFTYPE");
                sw.WriteLine("70");
                sw.WriteLine("6");
                sw.WriteLine("9");
                sw.WriteLine("$SURFU");
                sw.WriteLine("70");
                sw.WriteLine("6");
                sw.WriteLine("9");
                sw.WriteLine("$SURFV");
                sw.WriteLine("70");
                sw.WriteLine("6");
                sw.WriteLine("9");
                sw.WriteLine("$UCSNAME");
                sw.WriteLine("2");
                sw.WriteLine("");
                sw.WriteLine("9");
                sw.WriteLine("$UCSORG");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$UCSXDIR");
                sw.WriteLine("10");
                sw.WriteLine("1.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$UCSYDIR");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("1.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$PUCSNAME");
                sw.WriteLine("2");
                sw.WriteLine("");
                sw.WriteLine("9");
                sw.WriteLine("$PUCSORG");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$PUCSXDIR");
                sw.WriteLine("10");
                sw.WriteLine("1.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$PUCSYDIR");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("1.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$USERI1");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$USERI2");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$USERI3");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$USERI4");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$USERI5");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$USERR1");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$USERR2");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$USERR3");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$USERR4");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$USERR5");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$WORLDVIEW");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$SHADEDGE");
                sw.WriteLine("70");
                sw.WriteLine("3");
                sw.WriteLine("9");
                sw.WriteLine("$SHADEDIF");
                sw.WriteLine("70");
                sw.WriteLine("70");
                sw.WriteLine("9");
                sw.WriteLine("$TILEMODE");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$MAXACTVP");
                sw.WriteLine("70");
                sw.WriteLine("64");
                sw.WriteLine("9");
                sw.WriteLine("$PLIMCHECK");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$PEXTMIN");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$PEXTMAX");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$PLIMMIN");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$PLIMMAX");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("9");
                sw.WriteLine("$UNITMODE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$VISRETAIN");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("9");
                sw.WriteLine("$PLINEGEN");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("9");
                sw.WriteLine("$PSLTSCALE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("ENDSEC");
                sw.WriteLine("0");
                sw.WriteLine("SECTION");
                sw.WriteLine("2");
                sw.WriteLine("TABLES");
                sw.WriteLine("0");
                sw.WriteLine("TABLE");
                sw.WriteLine("2");
                sw.WriteLine("VPORT");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("0");
                sw.WriteLine("VPORT");
                sw.WriteLine("2");
                sw.WriteLine("*ACTIVE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("11");
                sw.WriteLine("1.0");
                sw.WriteLine("21");
                sw.WriteLine("1.0");
                sw.WriteLine("12");
                sw.WriteLine("7393.85989");
                sw.WriteLine("22");
                sw.WriteLine("12257.763736");
                sw.WriteLine("13");
                sw.WriteLine("0.0");
                sw.WriteLine("23");
                sw.WriteLine("0.0");
                sw.WriteLine("14");
                sw.WriteLine("10.0");
                sw.WriteLine("24");
                sw.WriteLine("10.0");
                sw.WriteLine("15");
                sw.WriteLine("10.0");
                sw.WriteLine("25");
                sw.WriteLine("10.0");
                sw.WriteLine("16");
                sw.WriteLine("0.0");
                sw.WriteLine("26");
                sw.WriteLine("0.0");
                sw.WriteLine("36");
                sw.WriteLine("1.0");
                sw.WriteLine("17");
                sw.WriteLine("210.0");
                sw.WriteLine("27");
                sw.WriteLine("148.5");
                sw.WriteLine("37");
                sw.WriteLine("0.0");
                sw.WriteLine("40");
                sw.WriteLine("47008.928571");
                sw.WriteLine("41");
                sw.WriteLine("2.047436");
                sw.WriteLine("42");
                sw.WriteLine("50.0");
                sw.WriteLine("43");
                sw.WriteLine("0.0");
                sw.WriteLine("44");
                sw.WriteLine("0.0");
                sw.WriteLine("50");
                sw.WriteLine("0.0");
                sw.WriteLine("51");
                sw.WriteLine("0.0");
                sw.WriteLine("71");
                sw.WriteLine("16");
                sw.WriteLine("72");
                sw.WriteLine("1000");
                sw.WriteLine("73");
                sw.WriteLine("1");
                sw.WriteLine("74");
                sw.WriteLine("3");
                sw.WriteLine("75");
                sw.WriteLine("0");
                sw.WriteLine("76");
                sw.WriteLine("0");
                sw.WriteLine("77");
                sw.WriteLine("0");
                sw.WriteLine("78");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("ENDTAB");
                sw.WriteLine("0");
                sw.WriteLine("TABLE");
                sw.WriteLine("2");
                sw.WriteLine("LTYPE");
                sw.WriteLine("70");
                sw.WriteLine("6");
                sw.WriteLine("0");
                sw.WriteLine("LTYPE");
                sw.WriteLine("2");
                sw.WriteLine("CONTINUOUS");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("Solid line");
                sw.WriteLine("72");
                sw.WriteLine("65");
                sw.WriteLine("73");
                sw.WriteLine("0");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("0");
                sw.WriteLine("LTYPE");
                sw.WriteLine("2");
                sw.WriteLine("ГОСТ_2_303_3");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("Сплошная волнистая ~~~~~~~~~~~~~~~~~~~");
                sw.WriteLine("72");
                sw.WriteLine("65");
                sw.WriteLine("73");
                sw.WriteLine("2");
                sw.WriteLine("40");
                sw.WriteLine("26.001");
                sw.WriteLine("49");
                sw.WriteLine("0.001");
                sw.WriteLine("49");
                sw.WriteLine("-26.0");
                sw.WriteLine("0");
                sw.WriteLine("LTYPE");
                sw.WriteLine("2");
                sw.WriteLine("ГОСТ_2_303_5");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("Штрих-пунктирная тонкая ____ _ ____ _ ____ _ __");
                sw.WriteLine("72");
                sw.WriteLine("65");
                sw.WriteLine("73");
                sw.WriteLine("4");
                sw.WriteLine("40");
                sw.WriteLine("24.0");
                sw.WriteLine("49");
                sw.WriteLine("20.0");
                sw.WriteLine("49");
                sw.WriteLine("-1.5");
                sw.WriteLine("49");
                sw.WriteLine("1.0");
                sw.WriteLine("49");
                sw.WriteLine("-1.5");
                sw.WriteLine("0");
                sw.WriteLine("LTYPE");
                sw.WriteLine("2");
                sw.WriteLine("ГОСТ_2_303_4");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("Штриховая __ __ __ __ __ __ __ __ __");
                sw.WriteLine("72");
                sw.WriteLine("65");
                sw.WriteLine("73");
                sw.WriteLine("3");
                sw.WriteLine("40");
                sw.WriteLine("7.0");
                sw.WriteLine("49");
                sw.WriteLine("5.0");
                sw.WriteLine("49");
                sw.WriteLine("-2.0");
                sw.WriteLine("49");
                sw.WriteLine("0.0");
                sw.WriteLine("0");
                sw.WriteLine("LTYPE");
                sw.WriteLine("2");
                sw.WriteLine("ГОСТ_2_303_6");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("Штрих-пунктирная утолщенная ___ _ ___ _ ___ _ _");
                sw.WriteLine("72");
                sw.WriteLine("65");
                sw.WriteLine("73");
                sw.WriteLine("4");
                sw.WriteLine("40");
                sw.WriteLine("12.0");
                sw.WriteLine("49");
                sw.WriteLine("8.0");
                sw.WriteLine("49");
                sw.WriteLine("-1.5");
                sw.WriteLine("49");
                sw.WriteLine("1.0");
                sw.WriteLine("49");
                sw.WriteLine("-1.5");
                sw.WriteLine("0");
                sw.WriteLine("LTYPE");
                sw.WriteLine("2");
                sw.WriteLine("ГОСТ_2_303_8");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("");
                sw.WriteLine("72");
                sw.WriteLine("65");
                sw.WriteLine("73");
                sw.WriteLine("2");
                sw.WriteLine("40");
                sw.WriteLine("48.0");
                sw.WriteLine("49");
                sw.WriteLine("40.0");
                sw.WriteLine("49");
                sw.WriteLine("-8.0");
                sw.WriteLine("0");
                sw.WriteLine("ENDTAB");
                sw.WriteLine("0");
                sw.WriteLine("TABLE");
                sw.WriteLine("2");
                sw.WriteLine("LAYER");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("0");
                sw.WriteLine("LAYER");
                sw.WriteLine("2");
                sw.WriteLine("0");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("62");
                sw.WriteLine("7");
                sw.WriteLine("6");
                sw.WriteLine("CONTINUOUS");
                sw.WriteLine("0");
                sw.WriteLine("ENDTAB");
                sw.WriteLine("0");
                sw.WriteLine("TABLE");
                sw.WriteLine("2");
                sw.WriteLine("STYLE");
                sw.WriteLine("70");
                sw.WriteLine("3");
                sw.WriteLine("0");
                sw.WriteLine("STYLE");
                sw.WriteLine("2");
                sw.WriteLine("STANDARD");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("41");
                sw.WriteLine("1.0");
                sw.WriteLine("50");
                sw.WriteLine("0.0");
                sw.WriteLine("71");
                sw.WriteLine("0");
                sw.WriteLine("42");
                sw.WriteLine("44318");
                sw.WriteLine("3");
                sw.WriteLine("txt.shx");
                sw.WriteLine("4");
                sw.WriteLine("");
                sw.WriteLine("0");
                sw.WriteLine("STYLE");
                sw.WriteLine("2");
                sw.WriteLine("");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("41");
                sw.WriteLine("1.0");
                sw.WriteLine("50");
                sw.WriteLine("0.0");
                sw.WriteLine("71");
                sw.WriteLine("0");
                sw.WriteLine("42");
                sw.WriteLine("0.2");
                sw.WriteLine("3");
                sw.WriteLine("GOST 2.303-68.shx");
                sw.WriteLine("4");
                sw.WriteLine("");
                sw.WriteLine("0");
                sw.WriteLine("STYLE");
                sw.WriteLine("2");
                sw.WriteLine("ГОСТ_2_304");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("40");
                sw.WriteLine("0.0");
                sw.WriteLine("41");
                sw.WriteLine("1.0");
                sw.WriteLine("50");
                sw.WriteLine("0.0");
                sw.WriteLine("71");
                sw.WriteLine("0");
                sw.WriteLine("42");
                sw.WriteLine("250.0");
                sw.WriteLine("3");
                sw.WriteLine("CS_Gost2304.shx");
                sw.WriteLine("4");
                sw.WriteLine("");
                sw.WriteLine("0");
                sw.WriteLine("ENDTAB");
                sw.WriteLine("0");
                sw.WriteLine("TABLE");
                sw.WriteLine("2");
                sw.WriteLine("VIEW");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("ENDTAB");
                sw.WriteLine("0");
                sw.WriteLine("TABLE");
                sw.WriteLine("2");
                sw.WriteLine("UCS");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("ENDTAB");
                sw.WriteLine("0");
                sw.WriteLine("TABLE");
                sw.WriteLine("2");
                sw.WriteLine("APPID");
                sw.WriteLine("70");
                sw.WriteLine("14");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("ACAD");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("ACAD_PSEXT");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("ACADANNOPO");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("ACADANNOTATIVE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("ACAD_DSTYLE_DIMJAG");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("ACAD_DSTYLE_DIMTALN");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("ACAD_MLEADERVER");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("MCS_DIM1");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("MCSXDATA5");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("MCS_PARAMS_DATA");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("MCS_PARAMS_HIDDEN_DATA");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("MCS_DOCUMENT_ID");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("MCSUBENTIDENTIFICATION");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("APPID");
                sw.WriteLine("2");
                sw.WriteLine("MC_VERSION_DATA");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("ENDTAB");
                sw.WriteLine("0");
                sw.WriteLine("TABLE");
                sw.WriteLine("2");
                sw.WriteLine("DIMSTYLE");
                sw.WriteLine("70");
                sw.WriteLine("6");
                sw.WriteLine("0");
                sw.WriteLine("DIMSTYLE");
                sw.WriteLine("2");
                sw.WriteLine("STANDARD");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("");
                sw.WriteLine("4");
                sw.WriteLine("");
                sw.WriteLine("5");
                sw.WriteLine("");
                sw.WriteLine("6");
                sw.WriteLine("");
                sw.WriteLine("7");
                sw.WriteLine("");
                sw.WriteLine("40");
                sw.WriteLine("100.0");
                sw.WriteLine("41");
                sw.WriteLine("44318");
                sw.WriteLine("42");
                sw.WriteLine("0.5");
                sw.WriteLine("43");
                sw.WriteLine("6.0");
                sw.WriteLine("44");
                sw.WriteLine("0.0");
                sw.WriteLine("45");
                sw.WriteLine("0.0");
                sw.WriteLine("46");
                sw.WriteLine("0.0");
                sw.WriteLine("47");
                sw.WriteLine("0.0");
                sw.WriteLine("48");
                sw.WriteLine("0.0");
                sw.WriteLine("140");
                sw.WriteLine("44318");
                sw.WriteLine("141");
                sw.WriteLine("44318");
                sw.WriteLine("142");
                sw.WriteLine("0.0");
                sw.WriteLine("143");
                sw.WriteLine("44311");
                sw.WriteLine("144");
                sw.WriteLine("1.0");
                sw.WriteLine("145");
                sw.WriteLine("0.0");
                sw.WriteLine("146");
                sw.WriteLine("1.0");
                sw.WriteLine("147");
                sw.WriteLine("1.0");
                sw.WriteLine("71");
                sw.WriteLine("0");
                sw.WriteLine("72");
                sw.WriteLine("0");
                sw.WriteLine("73");
                sw.WriteLine("0");
                sw.WriteLine("74");
                sw.WriteLine("1");
                sw.WriteLine("75");
                sw.WriteLine("0");
                sw.WriteLine("76");
                sw.WriteLine("0");
                sw.WriteLine("77");
                sw.WriteLine("0");
                sw.WriteLine("78");
                sw.WriteLine("0");
                sw.WriteLine("170");
                sw.WriteLine("0");
                sw.WriteLine("171");
                sw.WriteLine("2");
                sw.WriteLine("172");
                sw.WriteLine("0");
                sw.WriteLine("173");
                sw.WriteLine("0");
                sw.WriteLine("174");
                sw.WriteLine("0");
                sw.WriteLine("175");
                sw.WriteLine("0");
                sw.WriteLine("176");
                sw.WriteLine("0");
                sw.WriteLine("177");
                sw.WriteLine("0");
                sw.WriteLine("178");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("DIMSTYLE");
                sw.WriteLine("2");
                sw.WriteLine("СПДС");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("");
                sw.WriteLine("4");
                sw.WriteLine("");
                sw.WriteLine("5");
                sw.WriteLine("ARCHTICK");
                sw.WriteLine("6");
                sw.WriteLine("ARCHTICK");
                sw.WriteLine("7");
                sw.WriteLine("ARCHTICK");
                sw.WriteLine("40");
                sw.WriteLine("100.0");
                sw.WriteLine("41");
                sw.WriteLine("44318");
                sw.WriteLine("42");
                sw.WriteLine("0.625");
                sw.WriteLine("43");
                sw.WriteLine("27454");
                sw.WriteLine("44");
                sw.WriteLine("45658");
                sw.WriteLine("45");
                sw.WriteLine("0.0");
                sw.WriteLine("46");
                sw.WriteLine("2.0");
                sw.WriteLine("47");
                sw.WriteLine("0.0");
                sw.WriteLine("48");
                sw.WriteLine("0.0");
                sw.WriteLine("140");
                sw.WriteLine("44318");
                sw.WriteLine("141");
                sw.WriteLine("44318");
                sw.WriteLine("142");
                sw.WriteLine("0.0");
                sw.WriteLine("143");
                sw.WriteLine("44311");
                sw.WriteLine("144");
                sw.WriteLine("1.0");
                sw.WriteLine("145");
                sw.WriteLine("0.0");
                sw.WriteLine("146");
                sw.WriteLine("0.75");
                sw.WriteLine("147");
                sw.WriteLine("0.625");
                sw.WriteLine("71");
                sw.WriteLine("0");
                sw.WriteLine("72");
                sw.WriteLine("0");
                sw.WriteLine("73");
                sw.WriteLine("0");
                sw.WriteLine("74");
                sw.WriteLine("0");
                sw.WriteLine("75");
                sw.WriteLine("0");
                sw.WriteLine("76");
                sw.WriteLine("0");
                sw.WriteLine("77");
                sw.WriteLine("1");
                sw.WriteLine("78");
                sw.WriteLine("8");
                sw.WriteLine("170");
                sw.WriteLine("0");
                sw.WriteLine("171");
                sw.WriteLine("2");
                sw.WriteLine("172");
                sw.WriteLine("1");
                sw.WriteLine("173");
                sw.WriteLine("0");
                sw.WriteLine("174");
                sw.WriteLine("0");
                sw.WriteLine("175");
                sw.WriteLine("0");
                sw.WriteLine("176");
                sw.WriteLine("0");
                sw.WriteLine("177");
                sw.WriteLine("0");
                sw.WriteLine("178");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("DIMSTYLE");
                sw.WriteLine("2");
                sw.WriteLine("СПДС$2");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("");
                sw.WriteLine("4");
                sw.WriteLine("");
                sw.WriteLine("5");
                sw.WriteLine("OPEN30");
                sw.WriteLine("6");
                sw.WriteLine("OPEN30");
                sw.WriteLine("7");
                sw.WriteLine("OPEN30");
                sw.WriteLine("40");
                sw.WriteLine("1.0");
                sw.WriteLine("41");
                sw.WriteLine("44318");
                sw.WriteLine("42");
                sw.WriteLine("0.625");
                sw.WriteLine("43");
                sw.WriteLine("27454");
                sw.WriteLine("44");
                sw.WriteLine("0.18");
                sw.WriteLine("45");
                sw.WriteLine("0.0");
                sw.WriteLine("46");
                sw.WriteLine("0.0");
                sw.WriteLine("47");
                sw.WriteLine("0.0");
                sw.WriteLine("48");
                sw.WriteLine("0.0");
                sw.WriteLine("140");
                sw.WriteLine("44318");
                sw.WriteLine("141");
                sw.WriteLine("0.09");
                sw.WriteLine("142");
                sw.WriteLine("0.0");
                sw.WriteLine("143");
                sw.WriteLine("44311");
                sw.WriteLine("144");
                sw.WriteLine("1.0");
                sw.WriteLine("145");
                sw.WriteLine("0.0");
                sw.WriteLine("146");
                sw.WriteLine("1.0");
                sw.WriteLine("147");
                sw.WriteLine("0.625");
                sw.WriteLine("71");
                sw.WriteLine("0");
                sw.WriteLine("72");
                sw.WriteLine("0");
                sw.WriteLine("73");
                sw.WriteLine("0");
                sw.WriteLine("74");
                sw.WriteLine("1");
                sw.WriteLine("75");
                sw.WriteLine("0");
                sw.WriteLine("76");
                sw.WriteLine("0");
                sw.WriteLine("77");
                sw.WriteLine("1");
                sw.WriteLine("78");
                sw.WriteLine("0");
                sw.WriteLine("170");
                sw.WriteLine("0");
                sw.WriteLine("171");
                sw.WriteLine("2");
                sw.WriteLine("172");
                sw.WriteLine("0");
                sw.WriteLine("173");
                sw.WriteLine("0");
                sw.WriteLine("174");
                sw.WriteLine("0");
                sw.WriteLine("175");
                sw.WriteLine("0");
                sw.WriteLine("176");
                sw.WriteLine("0");
                sw.WriteLine("177");
                sw.WriteLine("0");
                sw.WriteLine("178");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("DIMSTYLE");
                sw.WriteLine("2");
                sw.WriteLine("СПДС$3");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("");
                sw.WriteLine("4");
                sw.WriteLine("");
                sw.WriteLine("5");
                sw.WriteLine("OPEN30");
                sw.WriteLine("6");
                sw.WriteLine("OPEN30");
                sw.WriteLine("7");
                sw.WriteLine("OPEN30");
                sw.WriteLine("40");
                sw.WriteLine("1.0");
                sw.WriteLine("41");
                sw.WriteLine("44318");
                sw.WriteLine("42");
                sw.WriteLine("0.0625");
                sw.WriteLine("43");
                sw.WriteLine("0.38");
                sw.WriteLine("44");
                sw.WriteLine("0.18");
                sw.WriteLine("45");
                sw.WriteLine("0.0");
                sw.WriteLine("46");
                sw.WriteLine("0.0");
                sw.WriteLine("47");
                sw.WriteLine("0.0");
                sw.WriteLine("48");
                sw.WriteLine("0.0");
                sw.WriteLine("140");
                sw.WriteLine("44318");
                sw.WriteLine("141");
                sw.WriteLine("44318");
                sw.WriteLine("142");
                sw.WriteLine("0.0");
                sw.WriteLine("143");
                sw.WriteLine("44311");
                sw.WriteLine("144");
                sw.WriteLine("1.0");
                sw.WriteLine("145");
                sw.WriteLine("0.0");
                sw.WriteLine("146");
                sw.WriteLine("1.0");
                sw.WriteLine("147");
                sw.WriteLine("0.625");
                sw.WriteLine("71");
                sw.WriteLine("0");
                sw.WriteLine("72");
                sw.WriteLine("0");
                sw.WriteLine("73");
                sw.WriteLine("0");
                sw.WriteLine("74");
                sw.WriteLine("1");
                sw.WriteLine("75");
                sw.WriteLine("0");
                sw.WriteLine("76");
                sw.WriteLine("0");
                sw.WriteLine("77");
                sw.WriteLine("1");
                sw.WriteLine("78");
                sw.WriteLine("0");
                sw.WriteLine("170");
                sw.WriteLine("0");
                sw.WriteLine("171");
                sw.WriteLine("2");
                sw.WriteLine("172");
                sw.WriteLine("1");
                sw.WriteLine("173");
                sw.WriteLine("0");
                sw.WriteLine("174");
                sw.WriteLine("0");
                sw.WriteLine("175");
                sw.WriteLine("0");
                sw.WriteLine("176");
                sw.WriteLine("0");
                sw.WriteLine("177");
                sw.WriteLine("0");
                sw.WriteLine("178");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("DIMSTYLE");
                sw.WriteLine("2");
                sw.WriteLine("СПДС$7");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("");
                sw.WriteLine("4");
                sw.WriteLine("");
                sw.WriteLine("5");
                sw.WriteLine("");
                sw.WriteLine("6");
                sw.WriteLine("");
                sw.WriteLine("7");
                sw.WriteLine("");
                sw.WriteLine("40");
                sw.WriteLine("1.0");
                sw.WriteLine("41");
                sw.WriteLine("1.0");
                sw.WriteLine("42");
                sw.WriteLine("0.0625");
                sw.WriteLine("43");
                sw.WriteLine("0.38");
                sw.WriteLine("44");
                sw.WriteLine("0.18");
                sw.WriteLine("45");
                sw.WriteLine("0.0");
                sw.WriteLine("46");
                sw.WriteLine("0.0");
                sw.WriteLine("47");
                sw.WriteLine("0.0");
                sw.WriteLine("48");
                sw.WriteLine("0.0");
                sw.WriteLine("140");
                sw.WriteLine("44318");
                sw.WriteLine("141");
                sw.WriteLine("0.09");
                sw.WriteLine("142");
                sw.WriteLine("0.0");
                sw.WriteLine("143");
                sw.WriteLine("44311");
                sw.WriteLine("144");
                sw.WriteLine("1.0");
                sw.WriteLine("145");
                sw.WriteLine("0.0");
                sw.WriteLine("146");
                sw.WriteLine("1.0");
                sw.WriteLine("147");
                sw.WriteLine("0.625");
                sw.WriteLine("71");
                sw.WriteLine("0");
                sw.WriteLine("72");
                sw.WriteLine("0");
                sw.WriteLine("73");
                sw.WriteLine("0");
                sw.WriteLine("74");
                sw.WriteLine("1");
                sw.WriteLine("75");
                sw.WriteLine("0");
                sw.WriteLine("76");
                sw.WriteLine("0");
                sw.WriteLine("77");
                sw.WriteLine("1");
                sw.WriteLine("78");
                sw.WriteLine("0");
                sw.WriteLine("170");
                sw.WriteLine("0");
                sw.WriteLine("171");
                sw.WriteLine("2");
                sw.WriteLine("172");
                sw.WriteLine("0");
                sw.WriteLine("173");
                sw.WriteLine("0");
                sw.WriteLine("174");
                sw.WriteLine("0");
                sw.WriteLine("175");
                sw.WriteLine("0");
                sw.WriteLine("176");
                sw.WriteLine("0");
                sw.WriteLine("177");
                sw.WriteLine("0");
                sw.WriteLine("178");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("DIMSTYLE");
                sw.WriteLine("2");
                sw.WriteLine("СПДС$4");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("3");
                sw.WriteLine("");
                sw.WriteLine("4");
                sw.WriteLine("");
                sw.WriteLine("5");
                sw.WriteLine("OPEN30");
                sw.WriteLine("6");
                sw.WriteLine("OPEN30");
                sw.WriteLine("7");
                sw.WriteLine("OPEN30");
                sw.WriteLine("40");
                sw.WriteLine("1.0");
                sw.WriteLine("41");
                sw.WriteLine("44318");
                sw.WriteLine("42");
                sw.WriteLine("0.0625");
                sw.WriteLine("43");
                sw.WriteLine("0.38");
                sw.WriteLine("44");
                sw.WriteLine("0.18");
                sw.WriteLine("45");
                sw.WriteLine("0.0");
                sw.WriteLine("46");
                sw.WriteLine("0.0");
                sw.WriteLine("47");
                sw.WriteLine("0.0");
                sw.WriteLine("48");
                sw.WriteLine("0.0");
                sw.WriteLine("140");
                sw.WriteLine("44318");
                sw.WriteLine("141");
                sw.WriteLine("44318");
                sw.WriteLine("142");
                sw.WriteLine("0.0");
                sw.WriteLine("143");
                sw.WriteLine("44311");
                sw.WriteLine("144");
                sw.WriteLine("1.0");
                sw.WriteLine("145");
                sw.WriteLine("0.0");
                sw.WriteLine("146");
                sw.WriteLine("1.0");
                sw.WriteLine("147");
                sw.WriteLine("0.625");
                sw.WriteLine("71");
                sw.WriteLine("0");
                sw.WriteLine("72");
                sw.WriteLine("0");
                sw.WriteLine("73");
                sw.WriteLine("0");
                sw.WriteLine("74");
                sw.WriteLine("1");
                sw.WriteLine("75");
                sw.WriteLine("0");
                sw.WriteLine("76");
                sw.WriteLine("0");
                sw.WriteLine("77");
                sw.WriteLine("1");
                sw.WriteLine("78");
                sw.WriteLine("0");
                sw.WriteLine("170");
                sw.WriteLine("0");
                sw.WriteLine("171");
                sw.WriteLine("2");
                sw.WriteLine("172");
                sw.WriteLine("1");
                sw.WriteLine("173");
                sw.WriteLine("0");
                sw.WriteLine("174");
                sw.WriteLine("0");
                sw.WriteLine("175");
                sw.WriteLine("0");
                sw.WriteLine("176");
                sw.WriteLine("0");
                sw.WriteLine("177");
                sw.WriteLine("0");
                sw.WriteLine("178");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("ENDTAB");
                sw.WriteLine("0");
                sw.WriteLine("ENDSEC");
                sw.WriteLine("0");
                sw.WriteLine("SECTION");
                sw.WriteLine("2");
                sw.WriteLine("BLOCKS");
                sw.WriteLine("0");
                sw.WriteLine("BLOCK");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("2");
                sw.WriteLine("$MODEL_SPACE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("3");
                sw.WriteLine("$MODEL_SPACE");
                sw.WriteLine("1");
                sw.WriteLine("");
                sw.WriteLine("0");
                sw.WriteLine("ENDBLK");
                sw.WriteLine("5");
                sw.WriteLine("21");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("BLOCK");
                sw.WriteLine("67");
                sw.WriteLine("1");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("2");
                sw.WriteLine("$PAPER_SPACE");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("3");
                sw.WriteLine("$PAPER_SPACE");
                sw.WriteLine("1");
                sw.WriteLine("");
                sw.WriteLine("0");
                sw.WriteLine("ENDBLK");
                sw.WriteLine("5");
                sw.WriteLine("227");
                sw.WriteLine("67");
                sw.WriteLine("1");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("BLOCK");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("2");
                sw.WriteLine("_ARCHTICK");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("3");
                sw.WriteLine("_ARCHTICK");
                sw.WriteLine("1");
                sw.WriteLine("");
                sw.WriteLine("0");
                sw.WriteLine("POLYLINE");
                sw.WriteLine("5");
                sw.WriteLine("1EF");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("66");
                sw.WriteLine("1");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("40");
                sw.WriteLine("0.15");
                sw.WriteLine("41");
                sw.WriteLine("0.15");
                sw.WriteLine("0");
                sw.WriteLine("VERTEX");
                sw.WriteLine("5");
                sw.WriteLine("1F0");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("-0.5");
                sw.WriteLine("20");
                sw.WriteLine("-0.5");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("0");
                sw.WriteLine("VERTEX");
                sw.WriteLine("5");
                sw.WriteLine("1F1");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.5");
                sw.WriteLine("20");
                sw.WriteLine("0.5");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("0");
                sw.WriteLine("SEQEND");
                sw.WriteLine("5");
                sw.WriteLine("1F2");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("ENDBLK");
                sw.WriteLine("5");
                sw.WriteLine("1EE");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("BLOCK");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("2");
                sw.WriteLine("_OPEN30");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("3");
                sw.WriteLine("_OPEN30");
                sw.WriteLine("1");
                sw.WriteLine("");
                sw.WriteLine("0");
                sw.WriteLine("LINE");
                sw.WriteLine("5");
                sw.WriteLine("1F7");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("-1.0");
                sw.WriteLine("20");
                sw.WriteLine("0.267949");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("11");
                sw.WriteLine("0.0");
                sw.WriteLine("21");
                sw.WriteLine("0.0");
                sw.WriteLine("31");
                sw.WriteLine("0.0");
                sw.WriteLine("0");
                sw.WriteLine("LINE");
                sw.WriteLine("5");
                sw.WriteLine("1F8");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("11");
                sw.WriteLine("-1.0");
                sw.WriteLine("21");
                sw.WriteLine("-0.267949");
                sw.WriteLine("31");
                sw.WriteLine("0.0");
                sw.WriteLine("0");
                sw.WriteLine("LINE");
                sw.WriteLine("5");
                sw.WriteLine("1F9");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("11");
                sw.WriteLine("-1.0");
                sw.WriteLine("21");
                sw.WriteLine("0.0");
                sw.WriteLine("31");
                sw.WriteLine("0.0");
                sw.WriteLine("0");
                sw.WriteLine("ENDBLK");
                sw.WriteLine("5");
                sw.WriteLine("1F6");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("BLOCK");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("2");
                sw.WriteLine("_DOT");
                sw.WriteLine("70");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("3");
                sw.WriteLine("_DOT");
                sw.WriteLine("1");
                sw.WriteLine("");
                sw.WriteLine("0");
                sw.WriteLine("POLYLINE");
                sw.WriteLine("5");
                sw.WriteLine("1FF");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("66");
                sw.WriteLine("1");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("70");
                sw.WriteLine("1");
                sw.WriteLine("40");
                sw.WriteLine("0.5");
                sw.WriteLine("41");
                sw.WriteLine("0.5");
                sw.WriteLine("0");
                sw.WriteLine("VERTEX");
                sw.WriteLine("5");
                sw.WriteLine("201");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("-0.25");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("42");
                sw.WriteLine("1.0");
                sw.WriteLine("0");
                sw.WriteLine("VERTEX");
                sw.WriteLine("5");
                sw.WriteLine("202");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.25");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("42");
                sw.WriteLine("1.0");
                sw.WriteLine("0");
                sw.WriteLine("SEQEND");
                sw.WriteLine("5");
                sw.WriteLine("203");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("LINE");
                sw.WriteLine("5");
                sw.WriteLine("200");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("BYBLOCK");
                sw.WriteLine("62");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("-0.5");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("11");
                sw.WriteLine("-1.0");
                sw.WriteLine("21");
                sw.WriteLine("0.0");
                sw.WriteLine("31");
                sw.WriteLine("0.0");
                sw.WriteLine("0");
                sw.WriteLine("ENDBLK");
                sw.WriteLine("5");
                sw.WriteLine("1FE");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("ENDSEC");
                sw.WriteLine("0");
                sw.WriteLine("SECTION");
                sw.WriteLine("2");
                sw.WriteLine("ENTITIES");
                sw.WriteLine("0");
            }
            using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
            {

                for (int d = 1; d < pipe1.points.Count; d++)
                {
                    sw.WriteLine("LINE");
                    sw.WriteLine("5");
                    sw.WriteLine("3B0");
                    sw.WriteLine("8");
                    sw.WriteLine("0");
                    sw.WriteLine("62");
                    sw.WriteLine("5");
                    sw.WriteLine("10");
                    sw.WriteLine(pipe1.points[d - 1].x);
                    sw.WriteLine("20");
                    sw.WriteLine(pipe1.points[d - 1].y);
                    sw.WriteLine("30");
                    sw.WriteLine("0.0");
                    sw.WriteLine("11");
                    sw.WriteLine(pipe1.points[d].x);
                    sw.WriteLine("21");
                    sw.WriteLine(pipe1.points[d].y);
                    sw.WriteLine("31");
                    sw.WriteLine("0.0");
                    sw.WriteLine("0");
                    Console.Write(".");//прогресс-индикатор для отслеживания процесса обработки массива
                }

            }
            //Вбиваем в файл информацию об объекте, заключительный отрезок и отрезок, обозначающий место начала построения
            using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
            {
                sw.WriteLine("TEXT");
                sw.WriteLine("5");
                sw.WriteLine("3B1");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("CONTINUOUS");
                sw.WriteLine("10");
                sw.WriteLine("0");//координата X
                sw.WriteLine("20");
                sw.WriteLine("1000");//координата Y
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("40");
                sw.WriteLine("40.0");//размер шрифта
                sw.WriteLine("1");
                sw.WriteLine($"Minimum diameter: {Math.Round(DiametrMin, 0)} mm.");
                sw.WriteLine("7");
                sw.WriteLine("ГОСТ_2_304");
                sw.WriteLine("0");
                //Добавляем заключительный отрезок, который соединит начало и конец
                {
                    sw.WriteLine("LINE");
                    sw.WriteLine("5");
                    sw.WriteLine("3B0");
                    sw.WriteLine("8");
                    sw.WriteLine("0");
                    sw.WriteLine("62");
                    sw.WriteLine("5");
                    sw.WriteLine("10");
                    sw.WriteLine(pipe1.points[0].x);
                    sw.WriteLine("20");
                    sw.WriteLine(pipe1.points[0].y);
                    sw.WriteLine("30");
                    sw.WriteLine("0.0");
                    sw.WriteLine("11");
                    sw.WriteLine(pipe1.points[pipe1.points.Count - 1].x);
                    sw.WriteLine("21");
                    sw.WriteLine(pipe1.points[pipe1.points.Count - 1].y);
                    sw.WriteLine("31");
                    sw.WriteLine("0.0");
                    sw.WriteLine("0");
                }
                //Добавляем отрезок, обозначающий место начала построения (предполагается, что здесь продольный шов)
                {
                    sw.WriteLine("LINE");
                    sw.WriteLine("5");
                    sw.WriteLine("3B0");
                    sw.WriteLine("8");
                    sw.WriteLine("0");
                    sw.WriteLine("62");
                    sw.WriteLine("8");
                    sw.WriteLine("10");
                    sw.WriteLine(pipe1.points[0].x);
                    sw.WriteLine("20");
                    sw.WriteLine(pipe1.points[0].y);
                    sw.WriteLine("30");
                    sw.WriteLine("0.0");
                    sw.WriteLine("11");
                    sw.WriteLine(0);
                    sw.WriteLine("21");
                    sw.WriteLine(0);
                    sw.WriteLine("31");
                    sw.WriteLine("0.0");
                    sw.WriteLine("0");
                }
                //Добавляем отрезок, обозначающий максимальный диаметр
                {
                    sw.WriteLine("LINE");
                    sw.WriteLine("5");
                    sw.WriteLine("3B0");
                    sw.WriteLine("8");
                    sw.WriteLine("0");
                    sw.WriteLine("62");
                    sw.WriteLine("1");
                    sw.WriteLine("10");
                    sw.WriteLine(max1.x);
                    sw.WriteLine("20");
                    sw.WriteLine(max1.y);
                    sw.WriteLine("30");
                    sw.WriteLine("0.0");
                    sw.WriteLine("11");
                    sw.WriteLine(max2.x);
                    sw.WriteLine("21");
                    sw.WriteLine(max2.y);
                    sw.WriteLine("31");
                    sw.WriteLine("0.0");
                    sw.WriteLine("0");
                }
                //Добавляем отрезок, обозначающий минимальный диаметр
                {
                    sw.WriteLine("LINE");
                    sw.WriteLine("5");
                    sw.WriteLine("3B0");
                    sw.WriteLine("8");
                    sw.WriteLine("0");
                    sw.WriteLine("62");
                    sw.WriteLine("5");
                    sw.WriteLine("10");
                    sw.WriteLine(min1.x);
                    sw.WriteLine("20");
                    sw.WriteLine(min1.y);
                    sw.WriteLine("30");
                    sw.WriteLine("0.0");
                    sw.WriteLine("11");
                    sw.WriteLine(min2.x);
                    sw.WriteLine("21");
                    sw.WriteLine(min2.y);
                    sw.WriteLine("31");
                    sw.WriteLine("0.0");
                    sw.WriteLine("0");
                }
            }



            using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
            {
                sw.WriteLine("TEXT");
                sw.WriteLine("5");
                sw.WriteLine("3B1");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("CONTINUOUS");
                sw.WriteLine("10");
                sw.WriteLine("0");//координата X
                sw.WriteLine("20");
                sw.WriteLine("900");//координата Y
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("40");
                sw.WriteLine("40.0");//размер шрифта
                sw.WriteLine("1");
                sw.WriteLine($"Maximum diameter: {Math.Round(DiametrMax, 0)} mm.");
                sw.WriteLine("7");
                sw.WriteLine("ГОСТ_2_304");
                sw.WriteLine("0");
                //Добавляем заключительный отрезок, который соединит начало и конец


                sw.WriteLine("LINE");
                sw.WriteLine("5");
                sw.WriteLine("3B0");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("62");
                sw.WriteLine("5");
                sw.WriteLine("10");
                sw.WriteLine(-300);
                sw.WriteLine("20");
                sw.WriteLine(1000);
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("11");
                sw.WriteLine(0);
                sw.WriteLine("21");
                sw.WriteLine(1000);
                sw.WriteLine("31");
                sw.WriteLine("0.0");
                sw.WriteLine("0");


                sw.WriteLine("LINE");
                sw.WriteLine("5");
                sw.WriteLine("3B0");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("62");
                sw.WriteLine("1");
                sw.WriteLine("10");
                sw.WriteLine(-300);
                sw.WriteLine("20");
                sw.WriteLine(900);
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("11");
                sw.WriteLine(0);
                sw.WriteLine("21");
                sw.WriteLine(900);
                sw.WriteLine("31");
                sw.WriteLine("0.0");
                sw.WriteLine("0");

                sw.WriteLine("TEXT");
                sw.WriteLine("5");
                sw.WriteLine("3B1");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("CONTINUOUS");
                sw.WriteLine("10");
                sw.WriteLine("-300");//координата X
                sw.WriteLine("20");
                sw.WriteLine("800");//координата Y
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("40");
                sw.WriteLine("40.0");//размер шрифта
                sw.WriteLine("1");
                sw.WriteLine($"Ovality of the pipe ({Pipename1}): {Math.Round((100 * ((2 * (DiametrMax - DiametrMin)) / (DiametrMax + DiametrMin))), 3)} %.");
                sw.WriteLine("7");
                sw.WriteLine("ГОСТ_2_304");
                sw.WriteLine("0");
            }
            //Добавляем отрезок, обозначающий место начала построения (предполагается, что здесь продольный шов)


            //Вбиваем в файл конечную галиматью
            using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
            {
                sw.WriteLine("VIEWPORT");
                sw.WriteLine("5");
                sw.WriteLine("228");
                sw.WriteLine("67");
                sw.WriteLine("1");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("10");
                sw.WriteLine("0.0");
                sw.WriteLine("20");
                sw.WriteLine("0.0");
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("40");
                sw.WriteLine("2172.474887");
                sw.WriteLine("41");
                sw.WriteLine("992.070339");
                sw.WriteLine("68");
                sw.WriteLine("1");
                sw.WriteLine("69");
                sw.WriteLine("1");
                sw.WriteLine("1001");
                sw.WriteLine("ACAD");
                sw.WriteLine("1000");
                sw.WriteLine("MVIEW");
                sw.WriteLine("1002");
                sw.WriteLine("{");
                sw.WriteLine("1070");
                sw.WriteLine("16");
                sw.WriteLine("1010");
                sw.WriteLine("0.0");
                sw.WriteLine("1020");
                sw.WriteLine("0.0");
                sw.WriteLine("1030");
                sw.WriteLine("0.0");
                sw.WriteLine("1020");
                sw.WriteLine("0.0");
                sw.WriteLine("1030");
                sw.WriteLine("0.0");
                sw.WriteLine("1010");
                sw.WriteLine("0.0");
                sw.WriteLine("1020");
                sw.WriteLine("0.0");
                sw.WriteLine("1030");
                sw.WriteLine("1.0");
                sw.WriteLine("1020");
                sw.WriteLine("0.0");
                sw.WriteLine("1030");
                sw.WriteLine("1.0");
                sw.WriteLine("1040");
                sw.WriteLine("0.0");
                sw.WriteLine("1040");
                sw.WriteLine("992.070339");
                sw.WriteLine("1040");
                sw.WriteLine("497.20797");
                sw.WriteLine("1040");
                sw.WriteLine("380.27835");
                sw.WriteLine("1040");
                sw.WriteLine("50.0");
                sw.WriteLine("1040");
                sw.WriteLine("0.0");
                sw.WriteLine("1040");
                sw.WriteLine("0.0");
                sw.WriteLine("1070");
                sw.WriteLine("16");
                sw.WriteLine("1070");
                sw.WriteLine("100");
                sw.WriteLine("1070");
                sw.WriteLine("0");
                sw.WriteLine("1070");
                sw.WriteLine("3");
                sw.WriteLine("1070");
                sw.WriteLine("0");
                sw.WriteLine("1070");
                sw.WriteLine("0");
                sw.WriteLine("1070");
                sw.WriteLine("0");
                sw.WriteLine("1070");
                sw.WriteLine("0");
                sw.WriteLine("1040");
                sw.WriteLine("0.0");
                sw.WriteLine("1040");
                sw.WriteLine("0.0");
                sw.WriteLine("1040");
                sw.WriteLine("0.0");
                sw.WriteLine("1040");
                sw.WriteLine("10.0");
                sw.WriteLine("1040");
                sw.WriteLine("10.0");
                sw.WriteLine("1040");
                sw.WriteLine("10.0");
                sw.WriteLine("1040");
                sw.WriteLine("10.0");
                sw.WriteLine("1070");
                sw.WriteLine("0");
                sw.WriteLine("1002");
                sw.WriteLine("{");
                sw.WriteLine("1002");
                sw.WriteLine("}");
                sw.WriteLine("1002");
                sw.WriteLine("}");
                sw.WriteLine("0");
                sw.WriteLine("ENDSEC");
                sw.WriteLine("0");
                sw.WriteLine("EOF");
            }

            //richTextBox1.AppendText(Environment.NewLine +"" ); 
            //richTextBox1.AppendText(Environment.NewLine + "Формирование файла с результатами вычислений выполнено.");
        }
        private void PipeToScreen(PipeDecart pipe1, PipeDecart pipe2, string Pipename1, string Pipename2, double corner, string filename)//Вывод на экран одного сварного соединения.
        {
            Graphics g = pictureBox5.CreateGraphics();
            g.Clear(Color.White);
            // Создаем объекты-кисти для закрашивания фигур
            SolidBrush myCorp = new SolidBrush(Color.DarkMagenta);
            SolidBrush myTrum = new SolidBrush(Color.DarkOrchid);
            SolidBrush myTrub = new SolidBrush(Color.DeepPink);
            SolidBrush mySeа = new SolidBrush(Color.Blue);
            //Выбираем перо myPen желтого цвета толщиной в 2 пикселя:
            Pen myWindYellow = new Pen(Color.Yellow, 2);
            //Выбираем перо myPen черного цвета толщиной в 2 пикселя:
            Pen myWindBlack = new Pen(Color.Black, 2);
            //Выбираем перо myPen Голубого цвета толщиной в 2 пикселя:
            Pen myWindBlue = new Pen(Color.Blue, 2);
            //Выбираем перо myPen Красного цвета толщиной в 2 пикселя:
            Pen myWindRed = new Pen(Color.Red, 2);
            //Выбираем перо myPen зелёного цвета толщиной в 2 пикселя:
            Pen myWindGreen = new Pen(Color.Lime, 2);
            //g.DrawLine(myWindRed, 100, 100, 200, 200);
            //запрашиваем ширину и высоту окна с графикой
            float gWidht = (float)pictureBox5.Width;
            float gHeight = (float)pictureBox5.Height;
            float halfgWidht = (float)pictureBox5.Width / 2;
            float halfgHeight = pictureBox5.Height / 2;
            float minim = Math.Min(gWidht, gHeight);
            float xCoeff = 1500 / minim;


            for (int d = 1; d < pipe1.points.Count; d++)//рисуем основную массу отрезков
            {
                //g.DrawLine(myWindBlack, ((float)(800 + pipe1.points[d - 1].x)) / 4, ((float)((-pipe1.points[d - 1].y)-800)) / 4, ((float)((-pipe1.points[d].x)-800)) / 4, ((float)(-800 - pipe1.points[d].y) / 4));
                g.DrawLine(myWindBlack, ((float)(halfgWidht + pipe1.points[d - 1].x / xCoeff)), ((float)(halfgHeight - pipe1.points[d - 1].y / xCoeff)), ((float)(halfgWidht + pipe1.points[d].x / xCoeff)), ((float)(halfgHeight - pipe1.points[d].y / xCoeff)));

            }
             //Добавляем заключительный отрезок, который соединит начало и конец               
            double x1 = pipe1.points[0].x;
            double y1 = pipe1.points[0].y;
            double x2 = pipe1.points[pipe1.points.Count - 1].x;
            double y2 = pipe1.points[pipe1.points.Count - 1].y;
            g.DrawLine(myWindBlack, ((float)(halfgWidht + x1 / xCoeff)), ((float)(halfgHeight - y1 / xCoeff)), ((float)(halfgWidht + x2 / xCoeff)), ((float)(halfgHeight - y2 / xCoeff)));

            //Добавляем отрезок, обозначающий место начала построения (предполагается, что здесь продольный шов)                
            x1 = pipe1.points[0].x;
            y1 = pipe1.points[0].y;
            x2 = 0;
            y2 = 0;
            g.DrawLine(myWindBlack, ((float)(halfgWidht + x1 / xCoeff)), ((float)(halfgHeight - y1 / xCoeff)), ((float)(halfgWidht + x2 / xCoeff)), ((float)(halfgHeight - y2 / xCoeff)));

            //добавляем вторую кромку***********************************************************************
            for (int d = 1; d < pipe2.points.Count; d++)//рисуем основную массу отрезков
            {
                //g.DrawLine(myWindBlack, ((float)(800 + pipe1.points[d - 1].x)) / 4, ((float)((-pipe1.points[d - 1].y)-800)) / 4, ((float)((-pipe1.points[d].x)-800)) / 4, ((float)(-800 - pipe1.points[d].y) / 4));
                g.DrawLine(myWindGreen, ((float)(halfgWidht + pipe2.points[d - 1].x / xCoeff)), ((float)(halfgHeight - pipe2.points[d - 1].y / xCoeff)), ((float)(halfgWidht + pipe2.points[d].x / xCoeff)), ((float)(halfgHeight - pipe2.points[d].y / xCoeff)));

            }
            //Добавляем заключительный отрезок, который соединит начало и конец               
            x1 = pipe2.points[0].x;
            y1 = pipe2.points[0].y;
            x2 = pipe2.points[pipe2.points.Count - 1].x;
            y2 = pipe2.points[pipe2.points.Count - 1].y;
            g.DrawLine(myWindGreen, ((float)(halfgWidht + x1 / xCoeff)), ((float)(halfgHeight - y1 / xCoeff)), ((float)(halfgWidht + x2 / xCoeff)), ((float)(halfgHeight - y2 / xCoeff)));

            //Добавляем отрезок, обозначающий место начала построения (предполагается, что здесь продольный шов)                
            x1 = pipe2.points[0].x;
            y1 = pipe2.points[0].y;
            x2 = 0;
            y2 = 0;
            g.DrawLine(myWindGreen, ((float)(halfgWidht + x1 / xCoeff)), ((float)(halfgHeight - y1 / xCoeff)), ((float)(halfgWidht + x2 / xCoeff)), ((float)(halfgHeight - y2 / xCoeff)));







            //richTextBox1.AppendText(Environment.NewLine +"" ); 
            //richTextBox1.AppendText(Environment.NewLine + "Формирование файла с результатами вычислений выполнено.");
        }
        private void PipeToScreenTwoJoint1(PipeDecart pipe1, PipeDecart pipe2, string Pipename1, string Pipename2, double corner, string filename)//Вывод на экран первого сварного соединения при расчете положения катушки
        {
            Graphics g = pictureBox6.CreateGraphics();
            g.Clear(Color.White);
            // Создаем объекты-кисти для закрашивания фигур
            SolidBrush myCorp = new SolidBrush(Color.DarkMagenta);
            SolidBrush myTrum = new SolidBrush(Color.DarkOrchid);
            SolidBrush myTrub = new SolidBrush(Color.DeepPink);
            SolidBrush mySeа = new SolidBrush(Color.Blue);
            //Выбираем перо myPen желтого цвета толщиной в 2 пикселя:
            Pen myWindYellow = new Pen(Color.Yellow, 2);
            //Выбираем перо myPen черного цвета толщиной в 2 пикселя:
            Pen myWindBlack = new Pen(Color.Black, 2);
            //Выбираем перо myPen Голубого цвета толщиной в 2 пикселя:
            Pen myWindBlue = new Pen(Color.Blue, 2);
            //Выбираем перо myPen Красного цвета толщиной в 2 пикселя:
            Pen myWindRed = new Pen(Color.Red, 2);
            //Выбираем перо myPen зелёного цвета толщиной в 2 пикселя:
            Pen myWindGreen = new Pen(Color.Lime, 2);
            //g.DrawLine(myWindRed, 100, 100, 200, 200);
            //запрашиваем ширину и высоту окна с графикой
            float gWidht = (float)pictureBox6.Width;
            float gHeight = (float)pictureBox6.Height;
            float halfgWidht = (float)pictureBox6.Width / 2;
            float halfgHeight = pictureBox6.Height / 2;
            float minim = Math.Min(gWidht, gHeight);
            float xCoeff = 1500 / minim;


            for (int d = 1; d < pipe1.points.Count; d++)//рисуем основную массу отрезков
            {
                //g.DrawLine(myWindBlack, ((float)(800 + pipe1.points[d - 1].x)) / 4, ((float)((-pipe1.points[d - 1].y)-800)) / 4, ((float)((-pipe1.points[d].x)-800)) / 4, ((float)(-800 - pipe1.points[d].y) / 4));
                g.DrawLine(myWindBlack, ((float)(halfgWidht + pipe1.points[d - 1].x / xCoeff)), ((float)(halfgHeight - pipe1.points[d - 1].y / xCoeff)), ((float)(halfgWidht + pipe1.points[d].x / xCoeff)), ((float)(halfgHeight - pipe1.points[d].y / xCoeff)));

            }
            //Добавляем заключительный отрезок, который соединит начало и конец               
            double x1 = pipe1.points[0].x;
            double y1 = pipe1.points[0].y;
            double x2 = pipe1.points[pipe1.points.Count - 1].x;
            double y2 = pipe1.points[pipe1.points.Count - 1].y;
            g.DrawLine(myWindBlack, ((float)(halfgWidht + x1 / xCoeff)), ((float)(halfgHeight - y1 / xCoeff)), ((float)(halfgWidht + x2 / xCoeff)), ((float)(halfgHeight - y2 / xCoeff)));

            //Добавляем отрезок, обозначающий место начала построения (предполагается, что здесь продольный шов)                
            x1 = pipe1.points[0].x;
            y1 = pipe1.points[0].y;
            x2 = 0;
            y2 = 0;
            g.DrawLine(myWindBlack, ((float)(halfgWidht + x1 / xCoeff)), ((float)(halfgHeight - y1 / xCoeff)), ((float)(halfgWidht + x2 / xCoeff)), ((float)(halfgHeight - y2 / xCoeff)));

            //добавляем вторую кромку***********************************************************************
            for (int d = 1; d < pipe2.points.Count; d++)//рисуем основную массу отрезков
            {
                //g.DrawLine(myWindBlack, ((float)(800 + pipe1.points[d - 1].x)) / 4, ((float)((-pipe1.points[d - 1].y)-800)) / 4, ((float)((-pipe1.points[d].x)-800)) / 4, ((float)(-800 - pipe1.points[d].y) / 4));
                g.DrawLine(myWindGreen, ((float)(halfgWidht + pipe2.points[d - 1].x / xCoeff)), ((float)(halfgHeight - pipe2.points[d - 1].y / xCoeff)), ((float)(halfgWidht + pipe2.points[d].x / xCoeff)), ((float)(halfgHeight - pipe2.points[d].y / xCoeff)));

            }
            //Добавляем заключительный отрезок, который соединит начало и конец               
            x1 = pipe2.points[0].x;
            y1 = pipe2.points[0].y;
            x2 = pipe2.points[pipe2.points.Count - 1].x;
            y2 = pipe2.points[pipe2.points.Count - 1].y;
            g.DrawLine(myWindGreen, ((float)(halfgWidht + x1 / xCoeff)), ((float)(halfgHeight - y1 / xCoeff)), ((float)(halfgWidht + x2 / xCoeff)), ((float)(halfgHeight - y2 / xCoeff)));

            //Добавляем отрезок, обозначающий место начала построения (предполагается, что здесь продольный шов)                
            x1 = pipe2.points[0].x;
            y1 = pipe2.points[0].y;
            x2 = 0;
            y2 = 0;
            g.DrawLine(myWindGreen, ((float)(halfgWidht + x1 / xCoeff)), ((float)(halfgHeight - y1 / xCoeff)), ((float)(halfgWidht + x2 / xCoeff)), ((float)(halfgHeight - y2 / xCoeff)));







            //richTextBox1.AppendText(Environment.NewLine +"" ); 
            //richTextBox1.AppendText(Environment.NewLine + "Формирование файла с результатами вычислений выполнено.");
        }
        private void PipeToScreenTwoJoint2(PipeDecart pipe1, PipeDecart pipe2, string Pipename1, string Pipename2, double corner, string filename)//Вывод на экран второго сварного соединения при расчете положения катушки
        {
            Graphics g = pictureBox7.CreateGraphics();
            g.Clear(Color.White);
            // Создаем объекты-кисти для закрашивания фигур
            SolidBrush myCorp = new SolidBrush(Color.DarkMagenta);
            SolidBrush myTrum = new SolidBrush(Color.DarkOrchid);
            SolidBrush myTrub = new SolidBrush(Color.DeepPink);
            SolidBrush mySeа = new SolidBrush(Color.Blue);
            //Выбираем перо myPen желтого цвета толщиной в 2 пикселя:
            Pen myWindYellow = new Pen(Color.Yellow, 2);
            //Выбираем перо myPen черного цвета толщиной в 2 пикселя:
            Pen myWindBlack = new Pen(Color.Black, 2);
            //Выбираем перо myPen Голубого цвета толщиной в 2 пикселя:
            Pen myWindBlue = new Pen(Color.Blue, 2);
            //Выбираем перо myPen Красного цвета толщиной в 2 пикселя:
            Pen myWindRed = new Pen(Color.Red, 2);
            //Выбираем перо myPen зелёного цвета толщиной в 2 пикселя:
            Pen myWindGreen = new Pen(Color.Lime, 2);
            //g.DrawLine(myWindRed, 100, 100, 200, 200);
            //запрашиваем ширину и высоту окна с графикой
            float gWidht = (float)pictureBox7.Width;
            float gHeight = (float)pictureBox7.Height;
            float halfgWidht = (float)pictureBox7.Width / 2;
            float halfgHeight = pictureBox7.Height / 2;
            float minim = Math.Min(gWidht, gHeight);
            float xCoeff = 1500 / minim;


            for (int d = 1; d < pipe1.points.Count; d++)//рисуем основную массу отрезков
            {
                //g.DrawLine(myWindBlack, ((float)(800 + pipe1.points[d - 1].x)) / 4, ((float)((-pipe1.points[d - 1].y)-800)) / 4, ((float)((-pipe1.points[d].x)-800)) / 4, ((float)(-800 - pipe1.points[d].y) / 4));
                g.DrawLine(myWindBlack, ((float)(halfgWidht + pipe1.points[d - 1].x / xCoeff)), ((float)(halfgHeight - pipe1.points[d - 1].y / xCoeff)), ((float)(halfgWidht + pipe1.points[d].x / xCoeff)), ((float)(halfgHeight - pipe1.points[d].y / xCoeff)));

            }
            //Добавляем заключительный отрезок, который соединит начало и конец               
            double x1 = pipe1.points[0].x;
            double y1 = pipe1.points[0].y;
            double x2 = pipe1.points[pipe1.points.Count - 1].x;
            double y2 = pipe1.points[pipe1.points.Count - 1].y;
            g.DrawLine(myWindBlack, ((float)(halfgWidht + x1 / xCoeff)), ((float)(halfgHeight - y1 / xCoeff)), ((float)(halfgWidht + x2 / xCoeff)), ((float)(halfgHeight - y2 / xCoeff)));

            //Добавляем отрезок, обозначающий место начала построения (предполагается, что здесь продольный шов)                
            x1 = pipe1.points[0].x;
            y1 = pipe1.points[0].y;
            x2 = 0;
            y2 = 0;
            g.DrawLine(myWindBlack, ((float)(halfgWidht + x1 / xCoeff)), ((float)(halfgHeight - y1 / xCoeff)), ((float)(halfgWidht + x2 / xCoeff)), ((float)(halfgHeight - y2 / xCoeff)));

            //добавляем вторую кромку***********************************************************************
            for (int d = 1; d < pipe2.points.Count; d++)//рисуем основную массу отрезков
            {
                //g.DrawLine(myWindBlack, ((float)(800 + pipe1.points[d - 1].x)) / 4, ((float)((-pipe1.points[d - 1].y)-800)) / 4, ((float)((-pipe1.points[d].x)-800)) / 4, ((float)(-800 - pipe1.points[d].y) / 4));
                g.DrawLine(myWindGreen, ((float)(halfgWidht + pipe2.points[d - 1].x / xCoeff)), ((float)(halfgHeight - pipe2.points[d - 1].y / xCoeff)), ((float)(halfgWidht + pipe2.points[d].x / xCoeff)), ((float)(halfgHeight - pipe2.points[d].y / xCoeff)));

            }
            //Добавляем заключительный отрезок, который соединит начало и конец               
            x1 = pipe2.points[0].x;
            y1 = pipe2.points[0].y;
            x2 = pipe2.points[pipe2.points.Count - 1].x;
            y2 = pipe2.points[pipe2.points.Count - 1].y;
            g.DrawLine(myWindGreen, ((float)(halfgWidht + x1 / xCoeff)), ((float)(halfgHeight - y1 / xCoeff)), ((float)(halfgWidht + x2 / xCoeff)), ((float)(halfgHeight - y2 / xCoeff)));

            //Добавляем отрезок, обозначающий место начала построения (предполагается, что здесь продольный шов)                
            x1 = pipe2.points[0].x;
            y1 = pipe2.points[0].y;
            x2 = 0;
            y2 = 0;
            g.DrawLine(myWindGreen, ((float)(halfgWidht + x1 / xCoeff)), ((float)(halfgHeight - y1 / xCoeff)), ((float)(halfgWidht + x2 / xCoeff)), ((float)(halfgHeight - y2 / xCoeff)));







            //richTextBox1.AppendText(Environment.NewLine +"" ); 
            //richTextBox1.AppendText(Environment.NewLine + "Формирование файла с результатами вычислений выполнено.");
        }

        private void PipeToScreenMoov(PipeDecart pipe1, PipeDecart pipe2, string Pipename1, string Pipename2, double corner, string filename)//вывод графики сварного стыка на экран. С движением.
        {
            Graphics g = pictureBox5.CreateGraphics();
            g.Clear(Color.White);
            // Создаем объекты-кисти для закрашивания фигур
            SolidBrush myCorp = new SolidBrush(Color.DarkMagenta);
            SolidBrush myTrum = new SolidBrush(Color.DarkOrchid);
            SolidBrush myTrub = new SolidBrush(Color.DeepPink);
            SolidBrush mySeа = new SolidBrush(Color.Blue);
            //Выбираем перо myPen желтого цвета толщиной в 2 пикселя:
            Pen myWindYellow = new Pen(Color.Yellow, 2);
            //Выбираем перо myPen черного цвета толщиной в 2 пикселя:
            Pen myWindBlack = new Pen(Color.Black, 2);
            //Выбираем перо myPen Голубого цвета толщиной в 2 пикселя:
            Pen myWindBlue = new Pen(Color.Blue, 2);
            //Выбираем перо myPen Красного цвета толщиной в 2 пикселя:
            Pen myWindRed = new Pen(Color.Red, 2);
            //Выбираем перо myPen зелёного цвета толщиной в 2 пикселя:
            Pen myWindGreen = new Pen(Color.Lime, 2);
            //g.DrawLine(myWindRed, 100, 100, 200, 200);
            //запрашиваем ширину и высоту окна с графикой
            float gWidht = (float)pictureBox5.Width;
            float gHeight = (float)pictureBox5.Height;
            float halfgWidht = (float)pictureBox5.Width / 2;
            float halfgHeight = pictureBox5.Height / 2;
            float minim = Math.Min(gWidht, gHeight);
            float xCoeff = 1500 / minim;


           
            //добавляем вторую кромку***********************************************************************
            Point Centre = new Point(0,0);
            pipe2 = rotate_pipe(pipe2, Centre,360-corner);

            for (int i = 0; i < corner; i++)
            {
                g.Clear(Color.White);
                for (int d = 1; d < pipe1.points.Count; d++)//рисуем основную массу отрезков
                {
                    //g.DrawLine(myWindBlack, ((float)(800 + pipe1.points[d - 1].x)) / 4, ((float)((-pipe1.points[d - 1].y)-800)) / 4, ((float)((-pipe1.points[d].x)-800)) / 4, ((float)(-800 - pipe1.points[d].y) / 4));
                    g.DrawLine(myWindBlack, ((float)(halfgWidht + pipe1.points[d - 1].x / xCoeff)), ((float)(halfgHeight - pipe1.points[d - 1].y / xCoeff)), ((float)(halfgWidht + pipe1.points[d].x / xCoeff)), ((float)(halfgHeight - pipe1.points[d].y / xCoeff)));

                }
                //Добавляем заключительный отрезок, который соединит начало и конец               
                double x1 = pipe1.points[0].x;
                double y1 = pipe1.points[0].y;
                double x2 = pipe1.points[pipe1.points.Count - 1].x;
                double y2 = pipe1.points[pipe1.points.Count - 1].y;
                g.DrawLine(myWindBlack, ((float)(halfgWidht + x1 / xCoeff)), ((float)(halfgHeight - y1 / xCoeff)), ((float)(halfgWidht + x2 / xCoeff)), ((float)(halfgHeight - y2 / xCoeff)));

                //Добавляем отрезок, обозначающий место начала построения (предполагается, что здесь продольный шов)                
                x1 = pipe1.points[0].x;
                y1 = pipe1.points[0].y;
                x2 = 0;
                y2 = 0;
                g.DrawLine(myWindBlack, ((float)(halfgWidht + x1 / xCoeff)), ((float)(halfgHeight - y1 / xCoeff)), ((float)(halfgWidht + x2 / xCoeff)), ((float)(halfgHeight - y2 / xCoeff)));

                pipe2 = rotate_pipe(pipe2, Centre, 1);

                for (int d = 1; d < pipe2.points.Count; d++)//рисуем основную массу отрезков
                {
                    //g.DrawLine(myWindBlack, ((float)(800 + pipe1.points[d - 1].x)) / 4, ((float)((-pipe1.points[d - 1].y)-800)) / 4, ((float)((-pipe1.points[d].x)-800)) / 4, ((float)(-800 - pipe1.points[d].y) / 4));
                    g.DrawLine(myWindGreen, ((float)(halfgWidht + pipe2.points[d - 1].x / xCoeff)), ((float)(halfgHeight - pipe2.points[d - 1].y / xCoeff)), ((float)(halfgWidht + pipe2.points[d].x / xCoeff)), ((float)(halfgHeight - pipe2.points[d].y / xCoeff)));

                }
                //Добавляем заключительный отрезок, который соединит начало и конец               
                x1 = pipe2.points[0].x;
                y1 = pipe2.points[0].y;
                x2 = pipe2.points[pipe2.points.Count - 1].x;
                y2 = pipe2.points[pipe2.points.Count - 1].y;
                g.DrawLine(myWindGreen, ((float)(halfgWidht + x1 / xCoeff)), ((float)(halfgHeight - y1 / xCoeff)), ((float)(halfgWidht + x2 / xCoeff)), ((float)(halfgHeight - y2 / xCoeff)));

                //Добавляем отрезок, обозначающий место начала построения (предполагается, что здесь продольный шов)                
                x1 = pipe2.points[0].x;
                y1 = pipe2.points[0].y;
                x2 = 0;
                y2 = 0;
                g.DrawLine(myWindGreen, ((float)(halfgWidht + x1 / xCoeff)), ((float)(halfgHeight - y1 / xCoeff)), ((float)(halfgWidht + x2 / xCoeff)), ((float)(halfgHeight - y2 / xCoeff)));

            }








            //richTextBox1.AppendText(Environment.NewLine +"" ); 
            //richTextBox1.AppendText(Environment.NewLine + "Формирование файла с результатами вычислений выполнено.");
        }

        private void OnePipeToScreen(PipeDecart pipe1, string Pipename1, Point min1, Point min2, Point max1, Point max2, string filename, double DiametrMax, double DiametrMin)//вывод на экран графики при расчете величины овальности.
        {
            
            // Объявляем объект "g" класса Graphics и предоставляем
            // ему возможность рисования на pictureBox1:
            Graphics g = pictureBox4.CreateGraphics();
            g.Clear(Color.White);
            // Создаем объекты-кисти для закрашивания фигур
            SolidBrush myCorp = new SolidBrush(Color.DarkMagenta);
            SolidBrush myTrum = new SolidBrush(Color.DarkOrchid);
            SolidBrush myTrub = new SolidBrush(Color.DeepPink);
            SolidBrush mySeа = new SolidBrush(Color.Blue);
            //Выбираем перо myPen желтого цвета толщиной в 2 пикселя:
            Pen myWindYellow = new Pen(Color.Yellow, 2);
            //Выбираем перо myPen черного цвета толщиной в 2 пикселя:
            Pen myWindBlack = new Pen(Color.Black, 2);
            //Выбираем перо myPen Голубого цвета толщиной в 2 пикселя:
            Pen myWindBlue = new Pen(Color.Blue, 2);
            //Выбираем перо myPen Коасного цвета толщиной в 2 пикселя:
            Pen myWindRed = new Pen(Color.Red, 2);
            //g.DrawLine(myWindRed, 100, 100, 200, 200);
            //запрашиваем ширину и высоту окна с графикой
            float gWidht = (float)pictureBox4.Width;
            float gHeight = (float)pictureBox4.Height;
            float halfgWidht = (float)pictureBox4.Width/2;
            float halfgHeight = pictureBox4.Height/2;
            float minim = Math.Min(gWidht, gHeight);
            float xCoeff = 1500 / minim;

            //Metafile mf = new Metafile("tempfile.wmf", hdc);

            //Metafile metafile = new Metafile("SampleMetafile.emf");
            //g.Graphics.DrawImage(metafile, 60, 10);
            //Metafile mf = new Metafile("tempfile.wmf");
            // Потом наполнить файл действиями
            //.... MetaGraphics.DrawRectangle
            // Потом сохранить
            for (int d = 1; d < pipe1.points.Count; d++)//рисуем основную массу отрезков
            {
                //g.DrawLine(myWindBlack, ((float)(800 + pipe1.points[d - 1].x)) / 4, ((float)((-pipe1.points[d - 1].y)-800)) / 4, ((float)((-pipe1.points[d].x)-800)) / 4, ((float)(-800 - pipe1.points[d].y) / 4));
                g.DrawLine(myWindBlack, ((float)(halfgWidht+pipe1.points[d - 1].x / xCoeff)), ((float)(halfgHeight - pipe1.points[d - 1].y / xCoeff)), ((float)(halfgWidht+pipe1.points[d].x / xCoeff)), ((float)(halfgHeight - pipe1.points[d].y / xCoeff)));
                
            }

           

            {
                //Вбиваем в файл информацию об объекте, заключительный отрезок и отрезок, обозначающий место начала построения
                /*
                {
                    sw.WriteLine("TEXT");
                    sw.WriteLine("5");
                    sw.WriteLine("3B1");
                    sw.WriteLine("8");
                    sw.WriteLine("0");
                    sw.WriteLine("6");
                    sw.WriteLine("CONTINUOUS");
                    sw.WriteLine("10");
                    sw.WriteLine("0");//координата X
                    sw.WriteLine("20");
                    sw.WriteLine("1000");//координата Y
                    sw.WriteLine("30");
                    sw.WriteLine("0.0");
                    sw.WriteLine("40");
                    sw.WriteLine("40.0");//размер шрифта
                    sw.WriteLine("1");
                    sw.WriteLine($"Minimum diameter: {Math.Round(DiametrMin, 0)} mm.");
                    sw.WriteLine("7");
                    sw.WriteLine("ГОСТ_2_304");
                    sw.WriteLine("0");*/
            }
                //Добавляем заключительный отрезок, который соединит начало и конец               
                double x1=pipe1.points[0].x;
                double y1 = pipe1.points[0].y;
                double x2 = pipe1.points[pipe1.points.Count - 1].x;
                double y2 = pipe1.points[pipe1.points.Count - 1].y;
                g.DrawLine(myWindBlack, ((float)(halfgWidht + x1 / xCoeff)), ((float)(halfgHeight - y1 / xCoeff)), ((float)(halfgWidht + x2 / xCoeff)), ((float)(halfgHeight - y2 / xCoeff)));
           
                //Добавляем отрезок, обозначающий место начала построения (предполагается, что здесь продольный шов)                
                x1 = pipe1.points[0].x;
                y1 = pipe1.points[0].y;
                x2 = 0;
                y2 = 0;
                g.DrawLine(myWindYellow, ((float)(halfgWidht + x1 / xCoeff)), ((float)(halfgHeight - y1 / xCoeff)), ((float)(halfgWidht + x2 / xCoeff)), ((float)(halfgHeight - y2 / xCoeff)));
            
                //Добавляем отрезок, обозначающий максимальный диаметр
               
                x1 = max1.x;
                y1 = max1.y;
                x2 = max2.x;
                y2 = max2.y;
                g.DrawLine(myWindRed, (float)(halfgWidht + x1 / xCoeff), (float)(halfgHeight - y1 / xCoeff), (float)(halfgWidht + x2 / xCoeff), (float)(halfgHeight - y2 / xCoeff));
            
            //Добавляем отрезок, обозначающий минимальный диаметр
            x1 = min1.x;
            y1 = min1.y;
            x2 = min2.x;
            y2 = min2.y;
            g.DrawLine(myWindBlue, (float)(halfgWidht + x1 / xCoeff), (float)(halfgHeight - y1 / xCoeff), (float)(halfgWidht + x2 / xCoeff), (float)(halfgHeight - y2 / xCoeff));



             
            //using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
            {
               /* sw.WriteLine("TEXT");
                sw.WriteLine("5");
                sw.WriteLine("3B1");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("CONTINUOUS");
                sw.WriteLine("10");
                sw.WriteLine("0");//координата X
                sw.WriteLine("20");
                sw.WriteLine("900");//координата Y
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("40");
                sw.WriteLine("40.0");//размер шрифта
                sw.WriteLine("1");
                sw.WriteLine($"Maximum diameter: {Math.Round(DiametrMax, 0)} mm.");
                sw.WriteLine("7");
                sw.WriteLine("ГОСТ_2_304");
                sw.WriteLine("0");
                //Добавляем заключительный отрезок, который соединит начало и конец


                sw.WriteLine("LINE");
                sw.WriteLine("5");
                sw.WriteLine("3B0");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("62");
                sw.WriteLine("5");
                sw.WriteLine("10");
                sw.WriteLine(-300);
                sw.WriteLine("20");
                sw.WriteLine(1000);
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("11");
                sw.WriteLine(0);
                sw.WriteLine("21");
                sw.WriteLine(1000);
                sw.WriteLine("31");
                sw.WriteLine("0.0");
                sw.WriteLine("0");


                sw.WriteLine("LINE");
                sw.WriteLine("5");
                sw.WriteLine("3B0");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("62");
                sw.WriteLine("1");
                sw.WriteLine("10");
                sw.WriteLine(-300);
                sw.WriteLine("20");
                sw.WriteLine(900);
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("11");
                sw.WriteLine(0);
                sw.WriteLine("21");
                sw.WriteLine(900);
                sw.WriteLine("31");
                sw.WriteLine("0.0");
                sw.WriteLine("0");

                sw.WriteLine("TEXT");
                sw.WriteLine("5");
                sw.WriteLine("3B1");
                sw.WriteLine("8");
                sw.WriteLine("0");
                sw.WriteLine("6");
                sw.WriteLine("CONTINUOUS");
                sw.WriteLine("10");
                sw.WriteLine("-300");//координата X
                sw.WriteLine("20");
                sw.WriteLine("800");//координата Y
                sw.WriteLine("30");
                sw.WriteLine("0.0");
                sw.WriteLine("40");
                sw.WriteLine("40.0");//размер шрифта
                sw.WriteLine("1");
                sw.WriteLine($"Ovality of the pipe ({Pipename1}): {Math.Round((100 * ((2 * (DiametrMax - DiametrMin)) / (DiametrMax + DiametrMin))), 3)} %.");
                sw.WriteLine("7");
                sw.WriteLine("ГОСТ_2_304");
                sw.WriteLine("0");*/
            }
            //Добавляем отрезок, обозначающий место начала построения (предполагается, что здесь продольный шов)*/

            


        }
        
        private void OneWeldedJoint(List<PipePolar> pipes)//расчет положения для одного сварного соединения
        {
            //tabControl2.TabPages.Remove(tabPage4);//прячем вкладку с графикой для овальности
            //tabControl2.TabPages.Add(tabPage5);//показываем вкладку с графикой для одного стыка
            richTextBox1.AppendText(Environment.NewLine + "Будет выполнен расчет положения трубы для монтажа одного сварного соединения.");
            double perimetr = Convert.ToDouble(textBox1.Text) * Math.PI;//задаём периметр трубы

            PipePolar pipe1;
            PipePolar pipe2;


            /* if (pipes[0].name.Contains("FromOvalometr"))
             {

             }*/

            /*if (pipes[0].points.Count > 50)
            {
                pipe1 = pipes[0];//Записываем в лист интерполяцию исходного листа            
            }
            else
            {
                pipe1 = IncreaseTheNumberOfPoints(pipes[0]);//Записываем в лист интерполяцию исходного листа                    
            }

            if (pipes[1].points.Count > 50)
            {
                pipe2 = mirror_pipe(pipes[1]);//Записываем в лист отраженную интерполяцию исходного листа
            }
            else
            {
                pipe2 = mirror_pipe(IncreaseTheNumberOfPoints(pipes[1]));//Записываем в лист отраженную интерполяцию исходного листа
            }*/

            if (pipes[0].name.Contains("FromOvalometr"))
            {
                pipe1 = pipes[0];//Записываем в лист исходный лист без интерполяции
                richTextBox1.AppendText(Environment.NewLine + "Первая кромка получена с прибора контроля овальности");
            }
            else
            {
                pipe1 = IncreaseTheNumberOfPoints(pipes[0]);//Записываем в лист интерполяцию исходного листа
                if (pipes[0].name.Contains("From_scaner"))
                {
                    richTextBox1.AppendText(Environment.NewLine + "Первая кромка получена получена с автоматического сканера профиля");
                }
                else
                {
                    richTextBox1.AppendText(Environment.NewLine + "Первая кромка получена получена методом ручных измерений");
                }    


            }

            if (pipes[1].name.Contains("FromOvalometr"))
            {
                pipe2 = mirror_pipe(pipes[1]);//Записываем в лист отраженный исходный лист без интерполяции
                richTextBox1.AppendText(Environment.NewLine + "Вторая кромка получена с прибора контроля овальности");
            }
            else
            {
                pipe2 = mirror_pipe(IncreaseTheNumberOfPoints(pipes[1]));//Записываем в лист отраженную интерполяцию исходного листа
                if (pipes[1].name.Contains("From_scaner"))
                {
                    richTextBox1.AppendText(Environment.NewLine + "Вторая кромка получена получена с автоматического сканера профиля");
                }
                else
                {
                    richTextBox1.AppendText(Environment.NewLine + "Вторая кромка получена получена методом ручных измерений");
                }
            }


            richTextBox1.AppendText(Environment.NewLine + "Первая кромка:");
            printPolarPipe(pipe1);
            richTextBox1.AppendText(Environment.NewLine + "Вторая кромка:");
            printPolarPipe(pipe2);
            int points_count = pipe1.points.Count;
            PipeDecart pipe1Decart = convertToDecart(pipe1);
            PipeDecart pipe2Decart = convertToDecart(pipe2);

            double minimum = 1000000;
            double corner = 0;
            int number = 0;

            for (int point_index = 0; point_index <= points_count - 1; point_index++)
            {
                for (int angel = 0; angel < 360; angel++)
                {
                    PipeDecart pipe2_rotated = rotate_pipe(pipe2Decart, pipe2Decart.points[0], angel);
                    Point shift_delta = pipe1Decart.points[point_index].minus(pipe2_rotated.points[0]);
                    PipeDecart pipe2_shifted = shift_pipe(pipe2_rotated, shift_delta);
                    double diff = Calc_pipe_diffRing(pipe1Decart, pipe2_shifted, point_index);

                    if (diff < minimum)
                    {
                        minimum = diff;
                        corner = angel;
                        number = point_index;
                    }

                }

            }
            //richTextBox1.AppendText(Environment.NewLine +"" ); 
            richTextBox1.AppendText(Environment.NewLine + "Расчет оптимального взаимного положения выполнен.");
            richTextBox1.AppendText(Environment.NewLine + $"Минимальное суммарное смещение кромок = {Math.Round(minimum, 0)}, оптимальный угол для совмещения труб  = {corner} (поворот катушки против часовой стрелки по ходу газа),  дуговое расстояние между начальными точками совмещаемых кромок {Math.Truncate((corner * Math.PI * Convert.ToDouble(textBox1.Text)) / 360)} мм вдоль сварного шва против часовой стрелки по ходу газа.");
            edgeOffset edge = calcoffset(pipe1Decart, pipe2Decart, corner, number);//считаем лист значений смещений кромок по точкам
            OffsetOneWeldedJoint = edge;


            string writePath_txt = textBox2.Text + "result.txt";
            using (StreamWriter sw_txt = new StreamWriter(writePath_txt, false, System.Text.Encoding.Default))//для записи текстово файла с результатами

            {
                sw_txt.WriteLine($"Дата контроля: {pipe1.date}.");
                sw_txt.WriteLine($"Выполнен расчет оптимального взаимного расположения для соединения кромок объектов: {pipe1.name}, {pipe2.name}.");
                sw_txt.WriteLine($"Угол поворота трубы для оптимального совмещения кромок: {corner}, дуговое расстояние между нулевыми точками кромок: {Math.Truncate((corner * perimetr) / 360)} мм.");
                sw_txt.WriteLine($"Смещение кромок по периметру сопрягаемых элементов:");
            }
            richTextBox1.AppendText(Environment.NewLine + "Расчетные смещения кромок в оптимальном взаимном расположении труб по координатам:");
            for (int i = 0; i < edge.Offsets.Count; i++)//выдаём на экран величины смещения кромок по точкам
            {
                if (Math.Abs(edge.Offsets[i].PointOffset) > 3)
                {
                    richTextBox1.SelectionColor = Color.Black;
                    richTextBox1.AppendText(Environment.NewLine + $"Координата: ");
                    richTextBox1.SelectionColor = Color.Blue;
                    richTextBox1.AppendText($" {Math.Round((perimetr * edge.Offsets[i].PointNumber) / (edge.Offsets.Count), 0)} мм");
                    richTextBox1.SelectionColor = Color.Black;
                    richTextBox1.AppendText($" - смещение кромок: ");
                    richTextBox1.SelectionColor = Color.Red;
                    richTextBox1.AppendText($" {Math.Round(edge.Offsets[i].PointOffset, 2)} мм.");
                }
                if (Math.Abs(edge.Offsets[i].PointOffset) < 3)
                {
                    richTextBox1.SelectionColor = Color.Black;
                    richTextBox1.AppendText(Environment.NewLine + $"Координата: ");
                    richTextBox1.SelectionColor = Color.Blue;
                    richTextBox1.AppendText($" {Math.Round((perimetr * edge.Offsets[i].PointNumber) / (edge.Offsets.Count), 0)} мм");
                    richTextBox1.SelectionColor = Color.Black;
                    richTextBox1.AppendText($" - смещение кромок: ");
                    richTextBox1.SelectionColor = Color.Green;
                    richTextBox1.AppendText($" {Math.Round(edge.Offsets[i].PointOffset, 2)} мм.");
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    // scroll it automatically
                    richTextBox1.ScrollToCaret();
                }


                using (StreamWriter sw_txt = new StreamWriter(writePath_txt, true, System.Text.Encoding.Default))
                {
                    sw_txt.WriteLine($"Координата: {Math.Round((perimetr * edge.Offsets[i].PointNumber) / (edge.Offsets.Count), 0)} мм - смещение кромок {Math.Round(edge.Offsets[i].PointOffset, 2)}.");

                }

            }
            richTextBox1.SelectionColor = Color.Black;
            double sumoffsets = 0;
            for (int i = 0; i < edge.Offsets.Count; i++)
            {
                sumoffsets += Math.Abs(edge.Offsets[i].PointOffset);
            }

            richTextBox1.AppendText(Environment.NewLine + $"Сумма смещения {sumoffsets}.");


            Point shift_delta2 = pipe1Decart.points[number].minus(pipe2Decart.points[0]);
            PipeDecart pipe2_shifted2 = shift_pipe(pipe2Decart, shift_delta2);
            PipeDecart result = rotate_pipe(pipe2_shifted2, pipe2_shifted2.points[0], corner);

            PipeToFile(pipe1Decart, result, pipe1.name, pipe2.name, corner, "OneWeldedJoint");
            infoForGraphicsPipeToFile1 = new InfoForGraphicsPipeToFile(pipe1Decart, result, pipe1.name, pipe2.name, corner, "OneWeldedJoint");
            PipeToScreen (infoForGraphicsPipeToFile1.pipe1, infoForGraphicsPipeToFile1.pipe2, infoForGraphicsPipeToFile1.Pipename1, infoForGraphicsPipeToFile1.Pipename2, infoForGraphicsPipeToFile1.corner, "OneWeldedJoint");
            richTextBox1.AppendText(Environment.NewLine + "Файлы с отчетом и чертежем сформированы.");
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            // scroll it automatically
            richTextBox1.ScrollToCaret();
            textBox14.Text = infoForGraphicsPipeToFile1.Pipename1;
            textBox15.Text = infoForGraphicsPipeToFile1.Pipename2;
            textBox13.Text = Convert.ToString(infoForGraphicsPipeToFile1.corner); 
            textBox12.Text = Convert.ToString(Math.Truncate((infoForGraphicsPipeToFile1.corner * Math.PI * Convert.ToDouble(textBox1.Text)) / 360));
            textBox11.Text = Convert.ToString(Math.Truncate(sumoffsets));
            //richTextBox1.AppendText(Environment.NewLine + "Нажмите любую клавишу.");
            //Console.ReadKey();
        }
        private void TwoWeldedJoint(List<PipePolar> pipes)//расчет положения для двух сварных соединений
        {
            //tabControl2.TabPages.Remove(tabPage4);
            richTextBox1.AppendText(Environment.NewLine + "Обнаружено четыре или более кромки. Будет выполнен расчет положения катушки для монтажа двух сварных соединений.");
            PipePolar pipe1_increase = IncreaseTheNumberOfPoints(pipes[0]);
            PipePolar pipe2_increase = IncreaseTheNumberOfPoints(pipes[1]);
            PipePolar pipe3_increase = IncreaseTheNumberOfPoints(pipes[2]);
            PipePolar pipe4_increase = IncreaseTheNumberOfPoints(pipes[3]);

            PipePolar pipe1 = pipe1_increase;
            PipePolar pipe2 = mirror_pipe(pipe2_increase);
            PipePolar pipe3 = mirror_pipe(pipe3_increase);
            PipePolar pipe4 = pipe4_increase;



            //richTextBox1.AppendText(Environment.NewLine +"" ); 
            richTextBox1.AppendText(Environment.NewLine + "Первая кромка:");
            printPolarPipe(pipe1);
            richTextBox1.AppendText(Environment.NewLine + "Вторая кромка:");
            printPolarPipe(pipe2);
            richTextBox1.AppendText(Environment.NewLine + "Третья кромка:");
            printPolarPipe(pipe3);
            richTextBox1.AppendText(Environment.NewLine + "Четвёртая кромка:");
            printPolarPipe(pipe4);
            int points_count = pipe1.points.Count;
            PipeDecart pipe1Decart = convertToDecart(pipe1);
            PipeDecart pipe2Decart = convertToDecart(pipe2);
            PipeDecart pipe3Decart = convertToDecart(pipe3);
            PipeDecart pipe4Decart = convertToDecart(pipe4);
            PipeDecart pipe3_1Decart = convertToDecart(pipe3_increase);
            PipeDecart pipe4_1Decart = convertToDecart(mirror_pipe(pipe4_increase));

            double perimetr = Convert.ToDouble(textBox1.Text) * Math.PI;//задаём периметр трубы            
            double minimum = 1000000;
            bool direction = true;
            double corner = 0;
            int number1 = 0;
            int number2 = 0;
            double minimum2 = 1000000;

            double corner2 = 0;
            int number1_2 = 0;
            int number2_2 = 0;
            List<RotateResult> resultOfFirstJoint1 = GetOffsetForReiting(pipe1Decart, pipe3Decart);
            List<RotateResult> resultOfSecondJoint1 = GetOffsetForReiting(pipe2Decart, pipe4Decart);
            List<RotateResult> resultOfFirstJoint2 = GetOffsetForReiting(pipe1Decart, pipe4_1Decart);
            List<RotateResult> resultOfSecondJoint2 = GetOffsetForReiting(pipe2Decart, pipe3_1Decart);

            /* for (int i = 0; i < resultOfFirstJoint1.Count; i++)
             {
                 richTextBox1.AppendText(Environment.NewLine + $"///+*+*+*+*+*+*+*+*+*+*+*+*+*+*+*+*+*+{resultOfFirstJoint1[i].offsetSum}");
             }*/

            List<ERR_RAIT> ErrReit = new List<ERR_RAIT>();


            for (int i = 0; i < resultOfSecondJoint1.Count; i++)
            {
                ERR_RAIT stepErr = new ERR_RAIT(resultOfFirstJoint1[i].corner, resultOfFirstJoint1[i].offsetSum, resultOfSecondJoint1[i].offsetSum, 0, 0, 0);
                ErrReit.Add(stepErr);
            }
            /* richTextBox1.AppendText(Environment.NewLine + $"///+*+*+*+*+*+*+*+*+*+*+*+*+*+*+*+*+*+");
             for (int i = 0; i < ErrReit.Count; i++)
             {
                 richTextBox1.SelectionColor = Color.Red;
                 richTextBox1.AppendText(Environment.NewLine + $"{ErrReit[i].corner};{ErrReit[i].err1};{ErrReit[i].err2};{ErrReit[i].rait1};{ErrReit[i].rait2};{ErrReit[i].summ_rait}");
                 richTextBox1.SelectionColor = Color.Green;
                 richTextBox1.AppendText($"{ErrReit[i].corner};{ErrReit[i].err1};{ErrReit[i].err2};{ErrReit[i].rait1};{ErrReit[i].rait2};{ErrReit[i].summ_rait}");
             }
             richTextBox1.AppendText(Environment.NewLine + $"///+*+*+*+*+*+*+*+*+*+*+*+*+*+*+*+*+*+");*/



            List<ERR_RAIT> ErrReit1 = new List<ERR_RAIT>();
            ErrReit1 = GetErrReit(resultOfFirstJoint1, resultOfSecondJoint1);
            /* richTextBox1.AppendText(Environment.NewLine + $"+*+*+*+*+*+*+*+*+*+*+*+*+*+*+*+*+*+");
             richTextBox1.AppendText(Environment.NewLine + $"Corner={ErrReit1[0].corner}, Summ_err={ErrReit1[0].err1+ ErrReit1[0].err2}");
             richTextBox1.AppendText(Environment.NewLine + $"+*+*+*+*+*+*+*+*+*+*+*+*+*+*+*+*+*+");


              for (int i = 0; i < ErrReit1.Count; i++)
              {
                  richTextBox1.AppendText(Environment.NewLine + $"{ErrReit1[i].corner};{ErrReit1[i].err1};{ErrReit1[i].err2};{ErrReit1[i].rait1};{ErrReit1[i].rait2};{ErrReit1[i].summ_rait}");
              }*/




            /* for (int i = 0; i < resultOfFirstJoint1.Count; i++)
             {
                 richTextBox1.AppendText(Environment.NewLine + $"{resultOfFirstJoint1[i].offsetSum};{resultOfFirstJoint1[i].corner};{resultOfSecondJoint1[i].offsetSum};{resultOfSecondJoint1[i].corner};{resultOfFirstJoint2[i].offsetSum};{resultOfFirstJoint2[i].corner};{resultOfSecondJoint2[i].offsetSum};{resultOfSecondJoint2[i].corner}");
             }*/

            /* richTextBox1.AppendText(Environment.NewLine + $"*********************************");
             foreach (RotateResult aPart in resultOfFirstJoint1)
             {
                 richTextBox1.AppendText(Environment.NewLine + $"Угол={aPart.corner}, Точка= {aPart.pointnumber}, смещение= {aPart.offsetSum}.");
             }
             richTextBox1.AppendText(Environment.NewLine + $"*********************************");*/

            for (int i = 0; i < resultOfFirstJoint1.Count; i++)
            {
                if (resultOfFirstJoint1[i].offsetSum + resultOfSecondJoint1[i].offsetSum < minimum & resultOfFirstJoint1[i].offsetSum / resultOfSecondJoint1[i].offsetSum < 1.25 & resultOfFirstJoint1[i].offsetSum / resultOfSecondJoint1[i].offsetSum > 0.75)
                {
                    minimum = resultOfFirstJoint1[i].offsetSum + resultOfSecondJoint1[i].offsetSum;
                    corner = resultOfFirstJoint1[i].corner;
                    number1 = resultOfFirstJoint1[i].pointnumber;
                    number2 = resultOfSecondJoint1[i].pointnumber;
                }
            }

            /* for (int i = 0; i < resultOfFirstJoint1.Count; i++)
             {
                 if (resultOfFirstJoint2[i].offsetSum + resultOfSecondJoint2[i].offsetSum < minimum & resultOfFirstJoint2[i].offsetSum / resultOfSecondJoint2[i].offsetSum < 1.25 & resultOfFirstJoint2[i].offsetSum / resultOfSecondJoint2[i].offsetSum > 0.75)
                 {
                     minimum2 = resultOfFirstJoint2[i].offsetSum + resultOfSecondJoint1[i].offsetSum;
                     corner2 = resultOfFirstJoint2[i].corner;
                     number1_2 = resultOfFirstJoint2[i].pointnumber;
                     number2_2 = resultOfSecondJoint2[i].pointnumber;

                 }
             }

             if (minimum2<minimum)
             {
                 minimum = minimum2;
                 corner = corner2;
                 number1 = number1_2;
                 number2 = number2_2;
                 direction = false;
             }*/

            //richTextBox1.AppendText(Environment.NewLine +"" ); 
            richTextBox1.AppendText(Environment.NewLine + "Расчет оптимального взаимного положения выполнен.");
            richTextBox1.AppendText(Environment.NewLine + $"Минимальное суммарное смещение кромок = {Math.Round(minimum, 0)}, оптимальный угол для совмещения труб  = {corner} (поворот катушки против часовой стрелки по ходу газа),  дуговое расстояние между начальными точками совмещаемых кромок {Math.Truncate((corner * Math.PI * Convert.ToDouble(textBox1.Text)) / 360)} мм вдоль сварного шва против часовой стрелки по ходу газа.");
            if (direction == true)
            {
                richTextBox1.AppendText(Environment.NewLine + $"Оптимальное положение кромок: 1-3-4-2");
            }
            if (direction == false)
            {
                richTextBox1.AppendText(Environment.NewLine + $"Оптимальное положение кромок: 1-4-3-2");
            }

            string writePath_txt = textBox2.Text + "result.txt";
            using (StreamWriter sw_txt = new StreamWriter(writePath_txt, false, System.Text.Encoding.Default))//для записи текстово файла с результатами

            {
                /* sw_txt.WriteLine($"Дата контроля: {pipe1.date}.");
                 sw_txt.WriteLine($"Выполнен расчет оптимального взаимного расположения. Кромки магистрали:{pipe1.name}, {pipe2.name}, кромки катушки:{pipe3.name}, {pipe4.name}.");
                 if (direction == true)
                  {
                      sw_txt.WriteLine($"Оптимальное положение кромок: 1-3-4-2");
                  }
                  if (direction == false)
                  {
                      sw_txt.WriteLine($"Оптимальное положение кромок: 1-4-3-2");
                  }*/

                sw_txt.WriteLine($"Угол поворота трубы для оптимального совмещения кромок: {corner}, дуговое расстояние между нулевыми точками кромок: {Math.Truncate((corner * perimetr) / 360)} мм.");
                sw_txt.WriteLine($"Смещение кромок по периметру сопрягаемых элементов:");
            }
            edgeOffset edge1;
            edgeOffset edge2;

            edge1 = calcoffset(pipe1Decart, pipe3Decart, corner, number1);//считаем лист значений смещений кромок по точкам
            edge2 = calcoffset(pipe2Decart, pipe4Decart, corner, number2);//считаем лист значений смещений кромок по точкам


            if (direction == false)//расчет смещений кромок для второго варианта расположения катушки
            {
                edge1 = calcoffset(pipe2Decart, pipe3_1Decart, corner, number2);//считаем лист значений смещений кромок по точкам
                edge2 = calcoffset(pipe1Decart, pipe4_1Decart, corner, number1);//считаем лист значений смещений кромок по точкам
            }


            for (int i = 0; i < edge1.Offsets.Count; i++)//выдаём на экран величины смещения кромок по точкам
            {
                richTextBox1.SelectionColor = Color.Black;
                richTextBox1.AppendText(Environment.NewLine + $"Координата: ");
                richTextBox1.SelectionColor = Color.Blue;
                richTextBox1.AppendText($"{Math.Round((perimetr * edge1.Offsets[i].PointNumber) / (edge1.Offsets.Count), 0)} мм ");


                if (Math.Abs(edge1.Offsets[i].PointOffset) > 3)
                {
                    richTextBox1.SelectionColor = Color.Black;
                    richTextBox1.AppendText($"| смещение кромок 1:");
                    richTextBox1.SelectionColor = Color.Red;
                    richTextBox1.AppendText($" {Math.Round(edge1.Offsets[i].PointOffset, 2)}");
                    richTextBox1.AppendText($" мм.");
                }
                if (Math.Abs(edge1.Offsets[i].PointOffset) < 3)
                {
                    richTextBox1.SelectionColor = Color.Black;
                    richTextBox1.AppendText($"| смещение кромок 1:");
                    richTextBox1.SelectionColor = Color.Green;
                    richTextBox1.AppendText($" {Math.Round(edge1.Offsets[i].PointOffset, 2)}");
                    richTextBox1.AppendText($" мм.");
                }

                if (Math.Abs(edge2.Offsets[i].PointOffset) > 3)
                {
                    richTextBox1.SelectionColor = Color.Black;
                    richTextBox1.AppendText($"| смещение кромок 2:");
                    richTextBox1.SelectionColor = Color.Red;
                    richTextBox1.AppendText($" {Math.Round(edge2.Offsets[i].PointOffset, 2)}");
                    richTextBox1.AppendText($" мм.");
                }
                if (Math.Abs(edge2.Offsets[i].PointOffset) < 3)
                {
                    richTextBox1.SelectionColor = Color.Black;
                    richTextBox1.AppendText($"| смещение кромок 2:");
                    richTextBox1.SelectionColor = Color.Green;
                    richTextBox1.AppendText($" {Math.Round(edge2.Offsets[i].PointOffset, 2)}");
                    richTextBox1.AppendText($" мм.");
                }
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                // scroll it automatically
                richTextBox1.ScrollToCaret();

                using (StreamWriter sw_txt = new StreamWriter(writePath_txt, true, System.Text.Encoding.Default))
                {
                    sw_txt.WriteLine($"Координата: {Math.Round((perimetr * edge1.Offsets[i].PointNumber) / (edge1.Offsets.Count), 0)} мм - смещение кромок 1: {Math.Round(edge1.Offsets[i].PointOffset, 2)} мм, смещение кромок 2: {Math.Round(edge2.Offsets[i].PointOffset, 2)} мм .");

                }

            }
            richTextBox1.SelectionColor = Color.Black;
            double sumoffsets = 0;
            for (int i = 0; i < edge1.Offsets.Count; i++)
            {
                sumoffsets += (Math.Abs(edge1.Offsets[i].PointOffset) + Math.Abs(edge2.Offsets[i].PointOffset));
            }

            richTextBox1.AppendText(Environment.NewLine + $"Сумма смещения {sumoffsets}.");


            if (direction == true)//формирование чертежей для прямого направления катушки
            {
                Point shift_delta2 = pipe1Decart.points[number1].minus(pipe3Decart.points[0]);
                PipeDecart pipe3_shifted2 = shift_pipe(pipe3Decart, shift_delta2);
                PipeDecart result = rotate_pipe(pipe3_shifted2, pipe3_shifted2.points[0], corner);

                PipeToFile(pipe1Decart, result, pipe1.name, pipe3.name, corner, "FirstWeldedJoint");
                richTextBox1.AppendText(Environment.NewLine + "Файл с чертежем первого сварного соединения сформирован.");
                //выводим на экран первый стык
                infoForGraphicsPipeToFile2 = new InfoForGraphicsPipeToFile(pipe1Decart, result, pipe1.name, pipe3.name, corner, "FirstWeldedJoint");
                PipeToScreenTwoJoint1(infoForGraphicsPipeToFile2.pipe1, infoForGraphicsPipeToFile2.pipe2, infoForGraphicsPipeToFile2.Pipename1, infoForGraphicsPipeToFile2.Pipename2, infoForGraphicsPipeToFile2.corner, "OneWeldedJoint");



                shift_delta2 = pipe2Decart.points[number2].minus(pipe4Decart.points[0]);
                PipeDecart pipe4_shifted2 = shift_pipe(pipe4Decart, shift_delta2);
                result = rotate_pipe(pipe4_shifted2, pipe4_shifted2.points[0], corner);

                PipeToFile(pipe2Decart, result, pipe2.name, pipe4.name, corner, "SecondWeldedJoint");
                richTextBox1.AppendText(Environment.NewLine + "Файл с чертежем второго сварного соединения сформирован.");


                //выводим на экран второй стык
                infoForGraphicsPipeToFile3 = new InfoForGraphicsPipeToFile(pipe2Decart, result, pipe2.name, pipe4.name, corner, "SecondWeldedJoint");
                PipeToScreenTwoJoint2(infoForGraphicsPipeToFile3.pipe1, infoForGraphicsPipeToFile3.pipe2, infoForGraphicsPipeToFile3.Pipename1, infoForGraphicsPipeToFile3.Pipename2, infoForGraphicsPipeToFile3.corner, "OneWeldedJoint");

                textBox18.Text = infoForGraphicsPipeToFile2.Pipename1;
                textBox20.Text = infoForGraphicsPipeToFile2.Pipename2;
                textBox21.Text = infoForGraphicsPipeToFile3.Pipename1;
                textBox19.Text = infoForGraphicsPipeToFile3.Pipename2;

                textBox16.Text = Convert.ToString(infoForGraphicsPipeToFile2.corner);
                textBox17.Text = Convert.ToString(Math.Truncate((infoForGraphicsPipeToFile2.corner * Math.PI * Convert.ToDouble(textBox1.Text)) / 360));
                textBox22.Text = Convert.ToString(Math.Truncate(sumoffsets));
            }

            if (direction == false)//формирование чертежей для обратного направления катушки
            {
                Point shift_delta2 = pipe2Decart.points[number1].minus(pipe3Decart.points[0]);
                PipeDecart pipe3_shifted2 = shift_pipe(pipe3Decart, shift_delta2);
                PipeDecart result = rotate_pipe(pipe3_shifted2, pipe3_shifted2.points[0], corner);

                PipeToFile(pipe2Decart, result, pipe2.name, pipe3.name, corner, "FirstWeldedJoint");
                richTextBox1.AppendText(Environment.NewLine + "Файл с чертежем первого сварного соединения сформирован.");

                shift_delta2 = pipe1Decart.points[number2].minus(pipe4Decart.points[0]);
                PipeDecart pipe4_shifted2 = shift_pipe(pipe4Decart, shift_delta2);
                result = rotate_pipe(pipe4_shifted2, pipe4_shifted2.points[0], corner);

                PipeToFile(pipe1Decart, result, pipe1.name, pipe4.name, corner, "SecondWeldedJoint");
                richTextBox1.AppendText(Environment.NewLine + "Файл с чертежем второго сварного соединения сформирован.");
            }
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            // scroll it automatically
            richTextBox1.ScrollToCaret();



        }
        private void OnePipe(List<PipePolar> pipes)//расчет положения максимального, минимального диаметра и овальности для одной кромки
        {
            //tabControl2.TabPages.Remove(tabPage4);
            //tabControl2.TabPages.Add(tabPage4);
            PipeDecart pipe = convertToDecart(IncreaseTheNumberOfPoints(pipes[0]));

            Point PointMax1 = new Point(0, 0);
            Point PointMax2 = new Point(0, 0);
            Point PointMin1 = new Point(0, 0);
            Point PointMin2 = new Point(0, 0);
            //Создадим удлинненный массив
            PipeDecart PipeOneLong = new PipeDecart();
            int OnePipe = pipe.points.Count;
            int DoublePipe = OnePipe * 2;
            PipeOneLong.points = new List<Point>(DoublePipe);

            for (int i = 0; i < OnePipe; i++)
            {
                PipeOneLong.points.Add(new Point(pipe.points[i].x, pipe.points[i].y));
            }
            for (int i = 0; i < OnePipe; i++)
            {
                PipeOneLong.points.Add(new Point(pipe.points[i].x, pipe.points[i].y));
            }
            //Создали удлинненный массив
            double DiamertMax = 0;
            double length;
            for (int i = 0; i < pipe.points.Count; i++)//найдём максимальный диаметр контура
            {
                for (int j = i + 1; j < i + pipe.points.Count - 1; j++)
                {
                    length = Math.Sqrt(Math.Pow((PipeOneLong.points[j].x - pipe.points[i].x), 2) + Math.Pow((PipeOneLong.points[j].y - pipe.points[i].y), 2));
                    if (length > DiamertMax)
                    {
                        DiamertMax = length;
                        PointMax1.x = pipe.points[i].x;
                        PointMax1.y = pipe.points[i].y;
                        PointMax2.x = PipeOneLong.points[j].x;
                        PointMax2.y = PipeOneLong.points[j].y;
                    }
                }
            }

            Point CentrOfDiamMax = new Point((PointMax1.x + PointMax2.x) / 2f, (PointMax1.y + PointMax2.y) / 2f);

            double DiametrMin = DiamertMax;

            for (int i = 0; i < pipe.points.Count; i++)//найдём максимальный диаметр контура
            {
                for (int j = i + 1; j < i + pipe.points.Count - 1; j++)
                {
                    double dist = ((Math.Abs((PipeOneLong.points[j].y - pipe.points[i].y) * CentrOfDiamMax.x - (PipeOneLong.points[j].x - pipe.points[i].x) * CentrOfDiamMax.y + PipeOneLong.points[j].x * pipe.points[i].y - PipeOneLong.points[j].y * pipe.points[i].x)) / (Math.Sqrt(Math.Pow((PipeOneLong.points[j].y - pipe.points[i].y), 2) + Math.Pow((PipeOneLong.points[j].x - pipe.points[i].x), 2))));
                    length = Math.Sqrt(Math.Pow((PipeOneLong.points[j].x - pipe.points[i].x), 2) + Math.Pow((PipeOneLong.points[j].y - pipe.points[i].y), 2));
                    if (length < DiametrMin & dist < 1)
                    {
                        DiametrMin = length;
                        PointMin1.x = pipe.points[i].x;
                        PointMin1.y = pipe.points[i].y;
                        PointMin2.x = PipeOneLong.points[j].x;
                        PointMin2.y = PipeOneLong.points[j].y;
                    }
                }
            }

            //richTextBox1.AppendText(Environment.NewLine +"" ); 
            richTextBox1.AppendText(Environment.NewLine + $"Выполнено чтение из файла. Файл содержит информацию об одном сечении трубы.");
            richTextBox1.AppendText(Environment.NewLine + ($"Сведения об обследуемом сечении трубы:"));
            printPolarPipe(pipes[0]);
            //double diametrMin = Math.Sqrt(Math.Pow((PointMin1.x - PointMin2.x), 2) + Math.Pow((PointMin1.y - PointMin2.y), 2));
            //richTextBox1.AppendText(Environment.NewLine +"" ); 
            richTextBox1.AppendText(Environment.NewLine + ($"Количество точек обследуемого сечения трубы: {pipes[0].points.Count}."));
            richTextBox1.AppendText(Environment.NewLine + "Выполнен расчет максимального диаметра, минимального диаметра и величины овальности трубы.");
            richTextBox1.AppendText(Environment.NewLine + $"Максимальный диаметр: {Math.Round(DiamertMax, 0)} мм, минимальный диаметр: {Math.Round(DiametrMin, 0)} мм, овальность трубы {Math.Round((100 * ((2 * (DiamertMax - DiametrMin)) / (DiamertMax + DiametrMin))), 3)} %.");
            ////richTextBox1.AppendText(Environment.NewLine +"" );                        
            OnePipeToFile(pipe, pipes[0].name, PointMin1, PointMin2, PointMax1, PointMax2, "OnePipe", DiamertMax, DiametrMin);
            infoForGrahhicsOnePipe = new InfoForGraphicsOnePipe(pipe, pipes[0].name, PointMin1, PointMin2, PointMax1, PointMax2, DiamertMax, DiametrMin);//заполняем переменную для отрисовки графики
            OnePipeToScreen(infoForGrahhicsOnePipe.pipe, infoForGrahhicsOnePipe.name, infoForGrahhicsOnePipe.PointMin1, 
                infoForGrahhicsOnePipe.PointMin2, infoForGrahhicsOnePipe.PointMax1, infoForGrahhicsOnePipe.PointMax2, "OnePipe", 
                infoForGrahhicsOnePipe.DiamertMax, infoForGrahhicsOnePipe.DiametrMin);//выводим графику на экран

            textBox7.Text = infoForGrahhicsOnePipe.name;
            textBox8.Text = Convert.ToString(Math.Round(DiamertMax,0));
            textBox9.Text = Convert.ToString(Math.Round(DiametrMin,0));
            textBox10.Text = Convert.ToString(Math.Round((100 * ((2 * (DiamertMax - DiametrMin)) / (DiamertMax + DiametrMin))), 3));

            richTextBox1.AppendText(Environment.NewLine + "Файл с результатами вычислений сформирован.");
            //richTextBox1.AppendText(Environment.NewLine + "Нажмите любую клавишу");
            ////Console.ReadKey();
        }
        static PipePolar Ovalometr_to_Pipepolar_Common_Center(OvalometrFile ovalometrFile, int count, double diameter, double Hb, double L, double Rk, double pipeThickness, string name, string date)//конвертация данных с овалометра методом общего центра                                                                                                                                                                                                                                                                                 
        {
            //грубая обработка измерительных выбросов
            {
                double summ = 0;
                for (int i = 0; i < ovalometrFile.OvalMeasure.Count; i++)
                {
                    summ += ovalometrFile.OvalMeasure[i].LengthMeasere;
                }
                double sr_summ = summ / ovalometrFile.OvalMeasure.Count;
                for (int j = 0; j < 20; j++)
                {
                    for (int i = 0; i < ovalometrFile.OvalMeasure.Count - 1; i++)
                    {
                        if (Math.Abs(ovalometrFile.OvalMeasure[i].LengthMeasere - sr_summ) > 30)
                        {
                            ovalometrFile.OvalMeasure[i].LengthMeasere = ovalometrFile.OvalMeasure[i + 1].LengthMeasere;
                        }
                    }
                }
            }

            List<PointPolar> points = new List<PointPolar>();

            PipePolar result = new PipePolar(ovalometrFile.OvalometrData.ObjectName, ovalometrFile.OvalometrData.FileDate, points);


            for (int j = 0; j < count; j++)
            {
                double alpha_j = (360 * j) / count;
                double delta_j = (Math.PI * diameter * j) / count;
                double delta_min = 6000;
                int oval_index = 0;
                for (int i = 0; i < ovalometrFile.OvalMeasure.Count; i++)
                {
                    double delta = Math.Abs(delta_j - ovalometrFile.OvalMeasure[i].OdometrMaesure);
                    if (delta < delta_min)
                    {
                        delta_min = delta;
                        oval_index = i;
                    }

                }


                double hi = ovalometrFile.OvalMeasure[oval_index].LengthMeasere;//показания датчика в точке замера локального радиуса
                double Ri = LocalRadius(Hb, L, Rk, hi, diameter);

                double Rn = Ri + (Math.Sqrt((diameter / 2) * (diameter / 2) - ((L * L) / 4)) - Math.Sqrt(Ri * Ri - ((L * L) / 4)));

                PointPolar point = new PointPolar(alpha_j, Rn- pipeThickness);
                result.points.Add(point);
            }
            result.name = name;
            result.date = date;

            return result;
        }
        static PipeDecart Ovalometr_to_PipeDecart_Common_Tangent(OvalometrFile ovalometrFile, double diameter, double Hb, double L, double Rk, double PipeThickness)//конвертация данных с овалометра методом общего центра                                                                                                                                                                                                                                                                                 
        {
            //грубая обработка измерительных выбросов
            {
                double summ = 0;
                for (int i = 0; i < ovalometrFile.OvalMeasure.Count; i++)
                {
                    summ += ovalometrFile.OvalMeasure[i].LengthMeasere;
                }
                double sr_summ = summ / ovalometrFile.OvalMeasure.Count;
                for (int j = 0; j < 20; j++)
                {
                    for (int i = 0; i < ovalometrFile.OvalMeasure.Count - 1; i++)
                    {
                        if (Math.Abs(ovalometrFile.OvalMeasure[i].LengthMeasere - sr_summ) > 5)
                        {
                            ovalometrFile.OvalMeasure[i].LengthMeasere = ovalometrFile.OvalMeasure[i + 1].LengthMeasere;
                        }
                    }
                }
            }
            int step = 15;
            for (int i = 0; i < ovalometrFile.OvalMeasure.Count - step; i++)//дополнительная защита от шумов
            {
                double localSumm = 0;
                for (int j = 0; j < step; j++)
                {
                    localSumm += ovalometrFile.OvalMeasure[i + j].LengthMeasere;
                }
                double localSrSumm = localSumm / step;
                for (int k = 0; k < step; k++)
                {
                    if (Math.Abs(ovalometrFile.OvalMeasure[i + k].LengthMeasere - localSrSumm) > 0.4)
                    {
                        ovalometrFile.OvalMeasure[i + k].LengthMeasere = localSrSumm;
                    }
                }


            }

            //List<Point> points = new List<Point>();
            PipeDecart result = new PipeDecart();
            PipeDecart result_centrs = new PipeDecart();
            

            //создаём первую точку искомой кривой
            Point point1 = new Point(0, diameter / 2);
            result.points.Add(point1);//добавляем точку в декартову торубу
                                      // Point point_c0 = new Point(0, 0);
                                      // result_centrs.points.Add(point_c0);//добавляем точку первого центра в список центров локальных дуг



            //создаём точку центра первой локальной дуги
            double R1 = LocalRadius(Hb, L, Rk, ovalometrFile.OvalMeasure[0].LengthMeasere, diameter);
            double R2 = LocalRadius(Hb, L, Rk, ovalometrFile.OvalMeasure[1].LengthMeasere, diameter);
            Point point_c1 = new Point(0, diameter / 2 - R1);
            result_centrs.points.Add(point_c1);


            //Строим вторую точку искомой кривой
            double T = ovalometrFile.OvalMeasure[1].OdometrMaesure - ovalometrFile.OvalMeasure[0].OdometrMaesure;//путь одометра от предыдущей точки до текущей
            double x2 = -R1 * Math.Sin(T / R1);
            double y2 = diameter / 2 + R1 * (Math.Cos(T / R1) - 1);
            Point point = new Point(x2, y2);
            result.points.Add(point);
            //ищем центр дуги №2
            double Xc2 = x2 + (R2 / R1) * (result_centrs.points[0].x - x2);
            double Yc2 = y2 + (R2 / R1) * (result_centrs.points[0].y - y2);
            Point point_c2 = new Point(Xc2, Yc2);
            result_centrs.points.Add(point_c2);

            //далее вычисляем остальные чентры и точки кривых в цикле
            double Perimetr = (PipeThickness + (diameter / 2)) * 2 * Math.PI;//расчет периметра внешней поверхности
            int finish = 0;
            for (int i = 0; i < ovalometrFile.OvalMeasure.Count; i++)//поиск последнего замера первого оборота прибора вокруг трубы.
            {
                if (ovalometrFile.OvalMeasure[i].OdometrMaesure< Perimetr)
                {
                    finish = i;
                }
            }
            if (finish< ovalometrFile.OvalMeasure.Count)
            {
                finish++;
            }

            for (int i = 2; i < finish; i++)
            {
                if ((result.points[i - 1].x - result_centrs.points[i - 1].x) > 0)
                {
                    double Ti = ovalometrFile.OvalMeasure[i].OdometrMaesure - ovalometrFile.OvalMeasure[i - 1].OdometrMaesure;//путь одометра от предыдущей точки до текущей
                    double Ri = LocalRadius(Hb, L, Rk, ovalometrFile.OvalMeasure[i].LengthMeasere, diameter);
                    double Ri_1 = LocalRadius(Hb, L, Rk, ovalometrFile.OvalMeasure[i - 1].LengthMeasere, diameter);
                    double Xi = Ri * Math.Cos(Math.Atan((result.points[i - 1].y - result_centrs.points[i - 1].y) / (result.points[i - 1].x - result_centrs.points[i - 1].x)) + Ti / Ri) + result_centrs.points[i - 1].x;
                    double Yi = Ri * Math.Sin(Math.Atan((result.points[i - 1].y - result_centrs.points[i - 1].y) / (result.points[i - 1].x - result_centrs.points[i - 1].x)) + Ti / Ri) + result_centrs.points[i - 1].y;
                    double Xci = Xi + (Ri / Ri_1) * (result_centrs.points[i - 1].x - Xi);
                    double Yci = Yi + (Ri / Ri_1) * (result_centrs.points[i - 1].y - Yi);
                    Point point_i = new Point(Xi, Yi);
                    result.points.Add(point_i);
                    Point point_ci = new Point(Xci, Yci);
                    result_centrs.points.Add(point_ci);
                }
                if ((result.points[i - 1].x - result_centrs.points[i - 1].x) < 0)
                {
                    double Ti = ovalometrFile.OvalMeasure[i].OdometrMaesure - ovalometrFile.OvalMeasure[i - 1].OdometrMaesure;//путь одометра от предыдущей точки до текущей
                    double Ri = LocalRadius(Hb, L, Rk, ovalometrFile.OvalMeasure[i].LengthMeasere, diameter);
                    double Ri_1 = LocalRadius(Hb, L, Rk, ovalometrFile.OvalMeasure[i - 1].LengthMeasere, diameter);
                    double Xi = Ri * Math.Cos(Math.Atan((result.points[i - 1].y - result_centrs.points[i - 1].y) / (result.points[i - 1].x - result_centrs.points[i - 1].x)) + Ti / Ri + Math.PI) + result_centrs.points[i - 1].x;
                    double Yi = Ri * Math.Sin(Math.Atan((result.points[i - 1].y - result_centrs.points[i - 1].y) / (result.points[i - 1].x - result_centrs.points[i - 1].x)) + Ti / Ri + Math.PI) + result_centrs.points[i - 1].y;
                    double Xci = Xi + (Ri / Ri_1) * (result_centrs.points[i - 1].x - Xi);
                    double Yci = Yi + (Ri / Ri_1) * (result_centrs.points[i - 1].y - Yi);
                    Point point_i = new Point(Xi, Yi);
                    result.points.Add(point_i);
                    Point point_ci = new Point(Xci, Yci);
                    result_centrs.points.Add(point_ci);
                }

            }
            int PipeCount = result.points.Count;//добавляем "костыль" для соединения начала с концом
            double dX = result.points[PipeCount - 1].x - result.points[0].x;
            double dY = result.points[PipeCount - 1].y - result.points[0].y;
            for (int i = 0; i < result.points.Count; i++)
            {
                result.points[i].x = result.points[i].x - dX * i / PipeCount;
                result.points[i].y = result.points[i].y - dY * i / PipeCount;
            }

            return result;
        }
        private static double LocalRadius(double Hb, double L, double Rk, double hi, double diameter)//высота датчика на плоской поверхности, длина телеги, радиус колеса, показания датчика, номинальный диаметр
        {
            double Ri = 0;
            double Rnom = diameter / 2;//номинальный радиус внешней поверхности трубы
            double BC = Math.Sqrt((Hb - hi) * (Hb - hi) + L * L);
            double alpha = Math.Acos((BC * BC + (Hb - hi) * (Hb - hi) - L * L) / (2 * BC * (Hb - hi)));
            double beta = Math.PI - 2 * alpha;
            double DC = Math.Sqrt(2 * Rk * Rk * (1 - Math.Cos(beta)));
            double BD = BC - DC;
            Ri = Math.Sqrt((BD * BD) / (2 * (1 - Math.Cos(beta))));//локальный радиус в точке контроля
            return Ri;
        }
        static PipePolar DecartToPolar(PipeDecart pipeDecart, int countOfPipe, string name, string date, double diameter, double PipeThickness)//метод для конвертации декартовой трубы, полученной методом общей касательной из файла с овалометра.
        {
            List<PointPolar> points = new List<PointPolar>();
            PipePolar result0 = new PipePolar(name, date, points);
            List<PointPolar> points2 = new List<PointPolar>();
            PipePolar result = new PipePolar(name, date, points2);
            List<PointPolar> points3 = new List<PointPolar>();
            PipePolar result1 = new PipePolar(name, date, points3);
            for (int i = 0; i < pipeDecart.points.Count; i++)
            {
                double x = pipeDecart.points[i].x;
                double y = pipeDecart.points[i].y;
                if (x >= 0 & y > 0)
                {
                    double Ri = Math.Sqrt(x * x + y * y);
                    double alpha = (360 / (2 * Math.PI)) * (Math.Atan(y / x));
                    PointPolar point = new PointPolar(alpha, Ri);
                    result0.points.Add(point);
                }
                if (x < 0 & y > 0)
                {
                    double Ri = Math.Sqrt(x * x + y * y);
                    double alpha = (360 / (2 * Math.PI)) * (Math.Atan(y / x)) + 180;
                    PointPolar point = new PointPolar(alpha, Ri);
                    result0.points.Add(point);
                }
                if (x < 0 & y < 0)
                {
                    double Ri = Math.Sqrt(x * x + y * y);
                    double alpha = (360 / (2 * Math.PI)) * (Math.Atan(y / x)) + 180;
                    PointPolar point = new PointPolar(alpha, Ri);
                    result0.points.Add(point);
                }
                if (x > 0 & y < 0)
                {
                    double Ri = Math.Sqrt(x * x + y * y);
                    double alpha = (360 / (2 * Math.PI)) * (Math.Atan(y / x)) + 360;
                    PointPolar point = new PointPolar(alpha, Ri);
                    result0.points.Add(point);
                }
            }

            for (int j = 0; j < countOfPipe; j++)//TODO
            {
                double alpha_j = (360 * j) / countOfPipe;//текущий угол
                double delta_j = (Math.PI * diameter * j) / countOfPipe;//текущий путь одометра
                double delta_min = 1000;
                int oval_index = 0;
                for (int i = 0; i < result0.points.Count; i++)
                {
                    double delta = Math.Abs(alpha_j - result0.points[i].alpha);
                    //double ActualAngle = 0;
                    if (delta < delta_min)
                    {
                        delta_min = delta;
                        oval_index = i;
                    }

                }
                double actualAlpha = result0.points[oval_index].alpha;
                double actualRadius = result0.points[oval_index].r;
                PointPolar point = new PointPolar(actualAlpha, actualRadius);
                result.points.Add(point);

            }
            for (int i = 0; i < result.points.Count; i++)//учтем толщину трубы для перехода к внутренней кромке
            {
                result.points[i].r -= PipeThickness;

            }
            /*int index90 = 0;//учтем особенность построения контура по методу общей касательной и повернём контур влево на 90 градусов
            double corner90 = 360;

            for (int i = 0; i < result.points.Count; i++)
            {
                if (Math.Abs(result.points[i].alpha - 90) < corner90)
                {
                    corner90 = result.points[i].alpha;
                    index90 = i;
                }
            }


            for (int i = index90; i < result.points.Count; i++)
            {
                PointPolar point = new PointPolar(result.points[i].alpha - 90, result.points[i].r);
                result1.points.Add(point);
            }


            for (int i = 0; i < index90; i++)
            {
                PointPolar point = new PointPolar(result.points[i].alpha + 270, result.points[i].r);
                result1.points.Add(point);
            }*/



            return result;

        }
        private void OnePipe_exp(PipePolar pipe0) //метод нужен только для тестирования метода конвертации данных файла овалометра в полярную трубу
        {

            PipeDecart pipe = convertToDecart(IncreaseTheNumberOfPoints(pipe0));

            Point PointMax1 = new Point(0, 0);
            Point PointMax2 = new Point(0, 0);
            Point PointMin1 = new Point(0, 0);
            Point PointMin2 = new Point(0, 0);
            //Создадим удлинненный массив
            PipeDecart PipeOneLong = new PipeDecart();
            int OnePipe = pipe.points.Count;
            int DoublePipe = OnePipe * 2;
            PipeOneLong.points = new List<Point>(DoublePipe);

            for (int i = 0; i < OnePipe; i++)
            {
                PipeOneLong.points.Add(new Point(pipe.points[i].x, pipe.points[i].y));
            }
            for (int i = 0; i < OnePipe; i++)
            {
                PipeOneLong.points.Add(new Point(pipe.points[i].x, pipe.points[i].y));
            }
            //Создали удлинненный массив
            double DiamertMax = 0;
            double length;
            for (int i = 0; i < pipe.points.Count; i++)//найдём максимальный диаметр контура
            {
                for (int j = i + 1; j < i + pipe.points.Count - 1; j++)
                {
                    length = Math.Sqrt(Math.Pow((PipeOneLong.points[j].x - pipe.points[i].x), 2) + Math.Pow((PipeOneLong.points[j].y - pipe.points[i].y), 2));
                    if (length > DiamertMax)
                    {
                        DiamertMax = length;
                        PointMax1.x = pipe.points[i].x;
                        PointMax1.y = pipe.points[i].y;
                        PointMax2.x = PipeOneLong.points[j].x;
                        PointMax2.y = PipeOneLong.points[j].y;
                    }
                }
            }

            Point CentrOfDiamMax = new Point((PointMax1.x + PointMax2.x) / 2f, (PointMax1.y + PointMax2.y) / 2f);

            double DiametrMin = DiamertMax;

            for (int i = 0; i < pipe.points.Count; i++)//найдём максимальный диаметр контура
            {
                for (int j = i + 1; j < i + pipe.points.Count - 1; j++)
                {
                    double dist = ((Math.Abs((PipeOneLong.points[j].y - pipe.points[i].y) * CentrOfDiamMax.x - (PipeOneLong.points[j].x - pipe.points[i].x) * CentrOfDiamMax.y + PipeOneLong.points[j].x * pipe.points[i].y - PipeOneLong.points[j].y * pipe.points[i].x)) / (Math.Sqrt(Math.Pow((PipeOneLong.points[j].y - pipe.points[i].y), 2) + Math.Pow((PipeOneLong.points[j].x - pipe.points[i].x), 2))));
                    length = Math.Sqrt(Math.Pow((PipeOneLong.points[j].x - pipe.points[i].x), 2) + Math.Pow((PipeOneLong.points[j].y - pipe.points[i].y), 2));
                    if (length < DiametrMin & dist < 1)
                    {
                        DiametrMin = length;
                        PointMin1.x = pipe.points[i].x;
                        PointMin1.y = pipe.points[i].y;
                        PointMin2.x = PipeOneLong.points[j].x;
                        PointMin2.y = PipeOneLong.points[j].y;
                    }
                }
            }

            //richTextBox1.AppendText(Environment.NewLine +"" ); 
            richTextBox1.AppendText(Environment.NewLine + $"Выполнено чтение из файла. Файл содержит информацию об одном сечении трубы.");
            richTextBox1.AppendText(Environment.NewLine + ($"Сведения об обследуемом сечении трубы:"));
            printPolarPipe(pipe0);
            //double diametrMin = Math.Sqrt(Math.Pow((PointMin1.x - PointMin2.x), 2) + Math.Pow((PointMin1.y - PointMin2.y), 2));
            //richTextBox1.AppendText(Environment.NewLine +"" ); 
            richTextBox1.AppendText(Environment.NewLine + ($"Количество точек обследуемого сечения трубы: {pipe0.points.Count}."));
            richTextBox1.AppendText(Environment.NewLine + "Выполнен расчет максимального диаметра, минимального диаметра и величины овальности трубы.");
            richTextBox1.AppendText(Environment.NewLine + $"Максимальный диаметр: {Math.Round(DiamertMax, 0)} мм, минимальный диаметр: {Math.Round(DiametrMin, 0)} мм, овальность трубы {Math.Round((100 * ((2 * (DiamertMax - DiametrMin)) / (DiamertMax + DiametrMin))), 3)} %.");
            ////richTextBox1.AppendText(Environment.NewLine +"" );                        
            OnePipeToFile(pipe, pipe0.name, PointMin1, PointMin2, PointMax1, PointMax2, "OnePipe", DiamertMax, DiametrMin);
            richTextBox1.AppendText(Environment.NewLine + "Файл с результатами вычислений сформирован.");
            //richTextBox1.AppendText(Environment.NewLine + "Нажмите любую клавишу");
            ////Console.ReadKey();
        }
        private void OnePipe_exp_decart_input(PipeDecart pipe) //метод нужен только для тестирования метода конвертации данных файла овалометра методом общей касательной
        {


            Point PointMax1 = new Point(0, 0);
            Point PointMax2 = new Point(0, 0);
            Point PointMin1 = new Point(0, 0);
            Point PointMin2 = new Point(0, 0);
            //Создадим удлинненный массив
            PipeDecart PipeOneLong = new PipeDecart();
            int OnePipe = pipe.points.Count;
            int DoublePipe = OnePipe * 2;
            PipeOneLong.points = new List<Point>(DoublePipe);

            for (int i = 0; i < OnePipe; i++)
            {
                PipeOneLong.points.Add(new Point(pipe.points[i].x, pipe.points[i].y));
            }
            for (int i = 0; i < OnePipe; i++)
            {
                PipeOneLong.points.Add(new Point(pipe.points[i].x, pipe.points[i].y));
            }
            //Создали удлинненный массив
            double DiamertMax = 0;
            double length;
            for (int i = 0; i < pipe.points.Count; i++)//найдём максимальный диаметр контура
            {
                for (int j = i + 1; j < i + pipe.points.Count - 1; j++)
                {
                    length = Math.Sqrt(Math.Pow((PipeOneLong.points[j].x - pipe.points[i].x), 2) + Math.Pow((PipeOneLong.points[j].y - pipe.points[i].y), 2));
                    if (length > DiamertMax)
                    {
                        DiamertMax = length;
                        PointMax1.x = pipe.points[i].x;
                        PointMax1.y = pipe.points[i].y;
                        PointMax2.x = PipeOneLong.points[j].x;
                        PointMax2.y = PipeOneLong.points[j].y;
                    }
                }
            }

            Point CentrOfDiamMax = new Point((PointMax1.x + PointMax2.x) / 2f, (PointMax1.y + PointMax2.y) / 2f);

            double DiametrMin = DiamertMax;

            for (int i = 0; i < pipe.points.Count; i++)//найдём максимальный диаметр контура
            {
                for (int j = i + 1; j < i + pipe.points.Count - 1; j++)
                {
                    double dist = ((Math.Abs((PipeOneLong.points[j].y - pipe.points[i].y) * CentrOfDiamMax.x - (PipeOneLong.points[j].x - pipe.points[i].x) * CentrOfDiamMax.y + PipeOneLong.points[j].x * pipe.points[i].y - PipeOneLong.points[j].y * pipe.points[i].x)) / (Math.Sqrt(Math.Pow((PipeOneLong.points[j].y - pipe.points[i].y), 2) + Math.Pow((PipeOneLong.points[j].x - pipe.points[i].x), 2))));
                    length = Math.Sqrt(Math.Pow((PipeOneLong.points[j].x - pipe.points[i].x), 2) + Math.Pow((PipeOneLong.points[j].y - pipe.points[i].y), 2));
                    if (length < DiametrMin & dist < 1)
                    {
                        DiametrMin = length;
                        PointMin1.x = pipe.points[i].x;
                        PointMin1.y = pipe.points[i].y;
                        PointMin2.x = PipeOneLong.points[j].x;
                        PointMin2.y = PipeOneLong.points[j].y;
                    }
                }
            }

            //richTextBox1.AppendText(Environment.NewLine +"" ); 
            richTextBox1.AppendText(Environment.NewLine + $"Выполнено чтение из файла. Файл содержит информацию об одном сечении трубы.");
            richTextBox1.AppendText(Environment.NewLine + ($"Сведения об обследуемом сечении трубы:"));
            //printPolarPipe(pipe0);
            //double diametrMin = Math.Sqrt(Math.Pow((PointMin1.x - PointMin2.x), 2) + Math.Pow((PointMin1.y - PointMin2.y), 2));
            //richTextBox1.AppendText(Environment.NewLine +"" ); 
            richTextBox1.AppendText(Environment.NewLine + ($"Количество точек обследуемого сечения трубы: {pipe.points.Count}."));
            richTextBox1.AppendText(Environment.NewLine + "Выполнен расчет максимального диаметра, минимального диаметра и величины овальности трубы.");
            richTextBox1.AppendText(Environment.NewLine + $"Максимальный диаметр: {Math.Round(DiamertMax, 0)} мм, минимальный диаметр: {Math.Round(DiametrMin, 0)} мм, овальность трубы {Math.Round((100 * ((2 * (DiamertMax - DiametrMin)) / (DiamertMax + DiametrMin))), 3)} %.");
            ////richTextBox1.AppendText(Environment.NewLine +"" );                        
            OnePipeToFile(pipe, "name", PointMin1, PointMin2, PointMax1, PointMax2, "OnePipe", DiamertMax, DiametrMin);
            richTextBox1.AppendText(Environment.NewLine + "Файл с результатами вычислений сформирован.");
            //richTextBox1.AppendText(Environment.NewLine + "Нажмите любую клавишу");
            ////Console.ReadKey();
        }
        public Form1()
        {
            InitializeComponent();

        }
        
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void OpenFile_Click(object sender, EventArgs e)//открытие и обработка файлов
        {
            
            double pipeDiameter = Convert.ToDouble(textBox1.Text);
            double wheelDiameter = Convert.ToDouble(textBox4.Text);
            double pipeThickness = Convert.ToDouble(textBox3.Text);
            double HalfBaseLong = 0.5 * Convert.ToDouble(textBox5.Text);
            double baseHeight = Convert.ToDouble(textBox6.Text);

            if (openFileDialog1.ShowDialog() == DialogResult.OK)



                if (openFileDialog1.FileName.Contains(".txt"))
                {
                    List<PipePolar> resultFromFile1 = new List<PipePolar>();
                    resultFromFile1 = read_from_file(openFileDialog1.FileName);

                    for (int i = 0; i < resultFromFile1.Count; i++)//добавляем результаты чтения файла к общему списку труб
                    {
                        resultFromFile.Add(resultFromFile1[i]);
                        PipesForСalculations.Add(resultFromFile1[i]);
                    }
                    
                    richTextBox1.AppendText(Environment.NewLine + "<---------------------------------------------------------------------------------------------->");
                    richTextBox1.AppendText(Environment.NewLine + ($"Файл {openFileDialog1.FileName} загружен и готов к обработке. Количество сечений в файле: {resultFromFile.Count}."));
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                }

            if (openFileDialog1.FileName.Contains(".SCN"))
            {
                List<PipePolar> resultFromFile1 = new List<PipePolar>();
                resultFromFile1 = read_from_file_csScaner(openFileDialog1.FileName);

                for (int i = 0; i < resultFromFile1.Count; i++)
                {
                    resultFromFile.Add(resultFromFile1[i]);
                    PipesForСalculations.Add(resultFromFile1[i]);
                }

                richTextBox1.AppendText(Environment.NewLine + "<---------------------------------------------------------------------------------------------->");
                richTextBox1.AppendText(Environment.NewLine + ($"Файл {openFileDialog1.FileName} загружен и готов к обработке. Количество сечений в файле: {resultFromFile.Count}."));
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }
            if (openFileDialog1.FileName.Contains(".OVL"))
            {
                string fn = openFileDialog1.FileName;
                OvalometrFile ovalFile1 = read_from_file_Ovalometr(fn);
                if (radioButton1.Checked)
                {
                    PipeDecart Ovalpipedecart = Ovalometr_to_PipeDecart_Common_Tangent(ovalFile1, pipeDiameter, baseHeight, HalfBaseLong, wheelDiameter, pipeThickness);
                    PipePolar newpolar = DecartToPolar(Ovalpipedecart, 90, "FromOvalometr_" + ovalFile1.OvalometrData.ObjectName, ovalFile1.OvalometrData.FileDate, ovalFile1.OvalometrData.PipeDiameter, pipeThickness);
                    resultFromFile.Add(newpolar);
                    PipesForСalculations.Add(newpolar);
                    richTextBox1.AppendText(Environment.NewLine + "<---------------------------------------------------------------------------------------------->");
                    richTextBox1.AppendText(Environment.NewLine + $"Файл овалометра [{openFileDialog1.FileName}] обработан.");
                    richTextBox1.AppendText(Environment.NewLine + $"Труба [{ovalFile1.OvalometrData.ObjectName}] обработана по методу общей касательной и добавлена к массиву.");
                }

                if (radioButton2.Checked)
                {
                    PipePolar newpolar = Ovalometr_to_Pipepolar_Common_Center(ovalFile1, 90, pipeDiameter, baseHeight, HalfBaseLong, wheelDiameter, pipeThickness, "FromOvalometr_" + ovalFile1.OvalometrData.ObjectName, ovalFile1.OvalometrData.FileDate);
                    resultFromFile.Add(newpolar);
                    PipesForСalculations.Add(newpolar);
                    richTextBox1.AppendText(Environment.NewLine + "<---------------------------------------------------------------------------------------------->");
                    richTextBox1.AppendText(Environment.NewLine + $"Файл овалометра [{openFileDialog1.FileName}] обработан.");
                    richTextBox1.AppendText(Environment.NewLine + $"Труба [{ovalFile1.OvalometrData.ObjectName}] обработана по методу общего центра и добавлена к массиву.");
                }


            }

         
            comboBox1.Items.Clear();//очищаем списки КомбоБоксов
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
            for (int i = 0; i < resultFromFile.Count; i++)//Заполняем списки КомбоБоксов
            {
                comboBox1.Items.Add(resultFromFile[i].name);
                comboBox2.Items.Add(resultFromFile[i].name);
                comboBox3.Items.Add(resultFromFile[i].name);
                comboBox4.Items.Add(resultFromFile[i].name);
            }


            /*if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                resultFromFile.Clear();
                string fn = openFileDialog1.FileName;
                //string fn = fn1.Replace("'\'","\\");
                //string fn = "C:\\Optimalangle\\Pipeinput.txt";
                //pictureBox1.Load(openFileDialog1.FileName);
                ///List<PipePolar> result = new List<PipePolar>();
                using (StreamReader file = new StreamReader(fn))
                {
                    bool isNewPipe = true;
                    string name = "";
                    string date = "";
                    List<double> rs = new List<double>();
                    string ln;
                    while ((ln = file.ReadLine()) != null)
                    {
                        if (ln == "end")
                        {
                            List<PointPolar> points = new List<PointPolar>();
                            for (int i = 0; i < rs.Count; i++)
                            {
                                double angel = Convert.ToDouble(360) * (Convert.ToDouble(i) / Convert.ToDouble(rs.Count));
                                PointPolar point = new PointPolar(angel, rs[i]);
                                points.Add(point);
                                //richTextBox1.AppendText(Environment.NewLine + "Строка 3");
                            }
                            rs.Clear();
                            PipePolar pipe = new PipePolar(name, date, points);
                            resultFromFile.Add(pipe);
                            isNewPipe = true;
                        }
                        else
                        {
                            if (isNewPipe)
                            {
                                date = ln;
                                ln = file.ReadLine();
                                name = ln;
                                isNewPipe = false;
                            }
                            else
                            {
                                double d = double.Parse(ln);
                                rs.Add(d);
                                //richTextBox1.Text = "vvvv";
                            }
                        }
                    }
                    file.Close();
                    richTextBox1.AppendText(Environment.NewLine + "<---------------------------------------------------------------------------------------------->");
                    richTextBox1.AppendText(Environment.NewLine + ($"Файл {fn} загружен и готов к обработке. Количество сечений в файле: {resultFromFile.Count}."));
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    // scroll it automatically
                    richTextBox1.ScrollToCaret();
                    //richTextBox1.Text = ($"{resultFromFile.Count}")+ System.Environment.NewLine;
                    //richTextBox1.AppendText(Environment.NewLine + "Строка 3");
                }
                //return result;




            }*/
        }

        private void Working_Click(object sender, EventArgs e)//обработка собранного списка труб
        {
            string path = textBox2.Text;//проверяем есть ли такая папка. Если нет - то создаём

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            if (PipesForСalculations.Count == 1)//отработка варианта исходных данных для одной кромки
            {
                OnePipe(PipesForСalculations);
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                // scroll it automatically
                richTextBox1.ScrollToCaret();
            }


            if (PipesForСalculations.Count < 3 & PipesForСalculations.Count > 1)//отработка варианта исходных данных для одного сварного соединения
            {
                OneWeldedJoint(PipesForСalculations);
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                // scroll it automatically
                richTextBox1.ScrollToCaret();
            }

            if (PipesForСalculations.Count > 3)//отработка варианта исходных данных для врезки катушки
            {
                TwoWeldedJoint(PipesForСalculations);
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                // scroll it automatically
                richTextBox1.ScrollToCaret();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void OpenFolder_Click(object sender, EventArgs e)//открытие папки со сформированными отчетными файлами
        {
            Process.Start(textBox2.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)//формирование списка объектов для вычислений
        {
            richTextBox2.AppendText(Environment.NewLine + $"Идёт формирование списка");
            PipesForСalculations.Clear();//очищаем массив от старых значений
            

            if (comboBox1.SelectedIndex > -1)//Заполняем массив профилями в выбранном порядке
            {
                if (checkBox1.Checked == true)
                {
                    PipesForСalculations.Add(mirror_pipe(resultFromFile[comboBox1.SelectedIndex]));
                    richTextBox2.AppendText(Environment.NewLine + $"Сечение {resultFromFile[comboBox1.SelectedIndex].name}  отражено и добавлено к массиву для обработки под номером {PipesForСalculations.Count}.");
                }
                else
                {
                    PipesForСalculations.Add(resultFromFile[comboBox1.SelectedIndex]);
                    richTextBox2.AppendText(Environment.NewLine + $"Сечение {resultFromFile[comboBox1.SelectedIndex].name} добавлено к массиву для обработки под номером {PipesForСalculations.Count}.");
                }

                
            }

            if (comboBox2.SelectedIndex > -1)
            {
                if (checkBox2.Checked == true)
                {                    
                    PipesForСalculations.Add(mirror_pipe(resultFromFile[comboBox2.SelectedIndex]));
                    richTextBox2.AppendText(Environment.NewLine + $"Сечение {resultFromFile[comboBox2.SelectedIndex].name} отражено и добавлено к массиву для обработки под номером {PipesForСalculations.Count}.");
                }
                else
                {
                    PipesForСalculations.Add(resultFromFile[comboBox2.SelectedIndex]);                    
                    richTextBox2.AppendText(Environment.NewLine + $"Сечение {resultFromFile[comboBox2.SelectedIndex].name} добавлено к массиву для обработки под номером {PipesForСalculations.Count}.");
                }

            }
            if (comboBox3.SelectedIndex > -1)
            {
                if (checkBox3.Checked == true)
                {
                    
                    PipesForСalculations.Add(mirror_pipe(resultFromFile[comboBox3.SelectedIndex]));
                    richTextBox2.AppendText(Environment.NewLine + $"Сечение {resultFromFile[comboBox3.SelectedIndex].name} отражено и добавлено к массиву для обработки под номером {PipesForСalculations.Count}.");
                }

                else
                {
                    PipesForСalculations.Add(resultFromFile[comboBox3.SelectedIndex]);                    
                    richTextBox2.AppendText(Environment.NewLine + $"Сечение {resultFromFile[comboBox3.SelectedIndex].name} добавлено к массиву для обработки под номером {PipesForСalculations.Count}.");
                }


            }
            if (comboBox4.SelectedIndex > -1)
            {
                if (checkBox4.Checked == true)
                {
                    PipesForСalculations.Add(mirror_pipe(resultFromFile[comboBox4.SelectedIndex]));
                    richTextBox2.AppendText(Environment.NewLine + $"Сечение {resultFromFile[comboBox4.SelectedIndex].name} отражено и добавлено к массиву для обработки под номером {PipesForСalculations.Count}.");
                }

                else
                {
                    PipesForСalculations.Add(resultFromFile[comboBox4.SelectedIndex]);
                    richTextBox2.AppendText(Environment.NewLine + $"Сечение {resultFromFile[comboBox4.SelectedIndex].name} добавлено к массиву для обработки под номером {PipesForСalculations.Count}.");
                }
            }
            

        }

        private void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)//очистка массива труб и очистка комбобоксов от значений и старых вариантов выбора
        {
            resultFromFile.Clear();
            richTextBox1.Clear();
            PipesForСalculations.Clear();//очищаем массив от старых значений
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            comboBox4.SelectedIndex = -1;
            comboBox1.Items.Clear();//очищаем списки КомбоБоксов
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
            checkBox1.Checked = false;//очистка чекбоксов
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }
        private void pictureBox2_Click(object sender, EventArgs e)//если кликнем по картинке со схемой контроля - картинка откроется в отдельном окне  
        {
            Form2 newForm = new Form2();
            newForm.Show();
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void label10_Click(object sender, EventArgs e)
        {

        }
        private void label10_Click_1(object sender, EventArgs e)
        {

        }
        private void button2_Click_2(object sender, EventArgs e)
        {
            if (infoForGrahhicsOnePipe!= null)
            {
                OnePipeToScreen(infoForGrahhicsOnePipe.pipe, infoForGrahhicsOnePipe.name, infoForGrahhicsOnePipe.PointMin1, infoForGrahhicsOnePipe.PointMin2, infoForGrahhicsOnePipe.PointMax1, infoForGrahhicsOnePipe.PointMax2, "OnePipe", infoForGrahhicsOnePipe.DiamertMax, infoForGrahhicsOnePipe.DiametrMin);
            }
                        
        }
        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (infoForGrahhicsOnePipe != null)
            {
                OnePipeToScreen(infoForGrahhicsOnePipe.pipe, infoForGrahhicsOnePipe.name, infoForGrahhicsOnePipe.PointMin1, infoForGrahhicsOnePipe.PointMin2, infoForGrahhicsOnePipe.PointMax1, infoForGrahhicsOnePipe.PointMax2, "OnePipe", infoForGrahhicsOnePipe.DiamertMax, infoForGrahhicsOnePipe.DiametrMin);
            }
        }
        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (infoForGraphicsPipeToFile1!=null)
            {
                PipeToScreen(infoForGraphicsPipeToFile1.pipe1, infoForGraphicsPipeToFile1.pipe2, infoForGraphicsPipeToFile1.Pipename1, infoForGraphicsPipeToFile1.Pipename2, infoForGraphicsPipeToFile1.corner, "OneWeldedJoint");
            }
            
        }
        private void Mooving_Click(object sender, EventArgs e)
        {
            if (infoForGraphicsPipeToFile1 != null)
            {
                PipeToScreenMoov(infoForGraphicsPipeToFile1.pipe1, infoForGraphicsPipeToFile1.pipe2, infoForGraphicsPipeToFile1.Pipename1, infoForGraphicsPipeToFile1.Pipename2, infoForGraphicsPipeToFile1.corner, "OneWeldedJoint");
            }
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            if (infoForGraphicsPipeToFile1 != null)
            {
                PipeToScreen(infoForGraphicsPipeToFile1.pipe1, infoForGraphicsPipeToFile1.pipe2, infoForGraphicsPipeToFile1.Pipename1, infoForGraphicsPipeToFile1.Pipename2, infoForGraphicsPipeToFile1.corner, "OneWeldedJoint");
            }
        }
        private void button4_Click_1(object sender, EventArgs e)
        {
            if (infoForGraphicsPipeToFile2 != null)
            {
                PipeToScreenTwoJoint1(infoForGraphicsPipeToFile2.pipe1, infoForGraphicsPipeToFile2.pipe2, infoForGraphicsPipeToFile2.Pipename1, infoForGraphicsPipeToFile2.Pipename2, infoForGraphicsPipeToFile2.corner, "OneWeldedJoint");
            }
            if (infoForGraphicsPipeToFile3 != null)
            {
                PipeToScreenTwoJoint2(infoForGraphicsPipeToFile3.pipe1, infoForGraphicsPipeToFile3.pipe2, infoForGraphicsPipeToFile3.Pipename1, infoForGraphicsPipeToFile3.Pipename2, infoForGraphicsPipeToFile3.corner, "OneWeldedJoint");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            object oMissing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */

            
            //Start Word and create a new document.
            Word.Application oWord;
            Word.Document oDoc;
            oWord = new Word.Application();
            oWord.Visible = true;
            oDoc = oWord.Documents.Add(ref oMissing, ref oMissing,
            ref oMissing, ref oMissing);

            //Insert a paragraph at the beginning of the document.
            Word.Paragraph oPara1;
            oPara1 = oDoc.Content.Paragraphs.Add(ref oMissing);
            oPara1.Range.Text = "Heading 1";
            oPara1.Range.Font.Size = 25;
            oPara1.Range.Font.Bold = 1;
            oPara1.Format.SpaceAfter = 24;    //24 pt spacing after paragraph.
            oPara1.Range.InsertParagraphAfter();

            /*oPara1.Range.Font.Size = 12;
            oPara1.Range.Text = "Heading 1___";
            oPara1.Range.Font.Bold = 1;
            //oPara1.Range.Font.Size=55;
            oPara1.Format.SpaceAfter = 24;    //24 pt spacing after paragraph.
            oPara1.Range.InsertParagraphAfter();*/
            oPara1.Range.Font.Size = 12;


            //Insert a paragraph at the end of the document.
            /*Word.Paragraph oPara2;
            object oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara2 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara2.Range.Text = "Heading 2";
            oPara2.Format.SpaceAfter = 6;
            oPara2.Range.InsertParagraphAfter();*/

            //Insert another paragraph.
            object oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            Word.Paragraph oPara3;
            oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara3 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara3.Range.Text = "This is a sentence of normal text. Now here is a table:";
            oPara3.Range.Font.Bold = 0;
            oPara3.Format.SpaceAfter = 24;
            oPara3.Range.InsertParagraphAfter();

            //Insert a 3 x 5 table, fill it with data, and make the first row
            //bold and italic.
            int a = 2;
            int b = 5;
            Word.Table oTable;
            Word.Range wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oTable = oDoc.Tables.Add(wrdRng, a, b, ref oMissing, ref oMissing);
            oTable.Range.ParagraphFormat.SpaceAfter = 6;
            int r, c;
            string strText;
            for (r = 1; r <= a; r++)
                for (c = 1; c <= b; c++)
                {
                    strText = "r" + r + "c" + c;
                    oTable.Cell(r, c).Range.Text = strText;
                }
            oTable.Rows[1].Range.Font.Bold = 1;
            oTable.Rows[1].Range.Font.Italic = 1;

            //Add some text after the table.
            Word.Paragraph oPara4;
            oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara4 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara4.Range.InsertParagraphBefore();
            oPara4.Range.Text = "And here's another table:";
            oPara4.Format.SpaceAfter = 24;
            oPara4.Range.InsertParagraphAfter();

            //Insert a 5 x 2 table, fill it with data, and change the column widths.
            wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oTable = oDoc.Tables.Add(wrdRng, 5, 2, ref oMissing, ref oMissing);
            oTable.Range.ParagraphFormat.SpaceAfter = 6;
            for (r = 1; r <= 5; r++)
                for (c = 1; c <= 2; c++)
                {
                    strText = "r" + r + "c" + c;
                    oTable.Cell(r, c).Range.Text = strText;
                }
            oTable.Columns[1].Width = oWord.InchesToPoints(2); //Change width of columns 1 & 2
            oTable.Columns[2].Width = oWord.InchesToPoints(3);

            //Keep inserting text. When you get to 7 inches from top of the
            //document, insert a hard page break.
            object oPos;
            double dPos = oWord.InchesToPoints(7);
            oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range.InsertParagraphAfter();
            do
            {
                wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                wrdRng.ParagraphFormat.SpaceAfter = 6;
                wrdRng.InsertAfter("A line of text");
                wrdRng.InsertParagraphAfter();
                oPos = wrdRng.get_Information
                                       (Word.WdInformation.wdVerticalPositionRelativeToPage);
            }
            while (dPos >= Convert.ToDouble(oPos));
            object oCollapseEnd = Word.WdCollapseDirection.wdCollapseEnd;
            object oPageBreak = Word.WdBreakType.wdPageBreak;
            wrdRng.Collapse(ref oCollapseEnd);
            wrdRng.InsertBreak(ref oPageBreak);
            wrdRng.Collapse(ref oCollapseEnd);
            wrdRng.InsertAfter("We're now on page 2. Here's my chart:");
            wrdRng.InsertParagraphAfter();

            //Insert a chart.
            Word.InlineShape oShape;
            object oClassType = "MSGraph.Chart.8";
            wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oShape = wrdRng.InlineShapes.AddOLEObject(ref oClassType, ref oMissing,
            ref oMissing, ref oMissing, ref oMissing,
            ref oMissing, ref oMissing, ref oMissing);

            //Demonstrate use of late bound oChart and oChartApp objects to
            //manipulate the chart object with MSGraph.
            object oChart;
            object oChartApp;
            oChart = oShape.OLEFormat.Object;
            oChartApp = oChart.GetType().InvokeMember("Application",
            BindingFlags.GetProperty, null, oChart, null);

            //Change the chart type to Line.
            object[] Parameters = new Object[1];
            Parameters[0] = 4; //xlLine = 4
            oChart.GetType().InvokeMember("ChartType", BindingFlags.SetProperty,
            null, oChart, Parameters);

            //Update the chart image and quit MSGraph.
            oChartApp.GetType().InvokeMember("Update",
            BindingFlags.InvokeMethod, null, oChartApp, null);
            oChartApp.GetType().InvokeMember("Quit",
            BindingFlags.InvokeMethod, null, oChartApp, null);
            //... If desired, you can proceed from here using the Microsoft Graph 
            //Object model on the oChart and oChartApp objects to make additional
            //changes to the chart.

            //Set the width of the chart.
            oShape.Width = oWord.InchesToPoints(6.25f);
            oShape.Height = oWord.InchesToPoints(3.57f);

            //Add text after the chart.
            wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            wrdRng.InsertParagraphAfter();
            wrdRng.InsertAfter("THE END.");

            //Close this form.
            //this.Close();
        }

        private void OneWeldToWord_Click(object sender, EventArgs e)
        {
            object oMissing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */


            //Start Word and create a new document.
            Word.Application oWord;
            Word.Document oDoc;
            oWord = new Word.Application();
            oWord.Visible = true;
            oDoc = oWord.Documents.Add(ref oMissing, ref oMissing,
            ref oMissing, ref oMissing);

            //Insert a paragraph at the beginning of the document.
            
            Word.Paragraph oPara1;            
            oPara1 = oDoc.Content.Paragraphs.Add(ref oMissing);
            oPara1.Range.Font.Size = 16;
            string HeadOne = "Совмещение трубы " + infoForGraphicsPipeToFile1.Pipename1 + " с трубой " + infoForGraphicsPipeToFile1.Pipename2;
            oPara1.Range.Text = HeadOne;            
            oPara1.Range.Font.Bold = 1;
            oPara1.Format.SpaceAfter = 24;    //24 pt spacing after paragraph.
            oPara1.Range.InsertParagraphAfter();
            oPara1.Range.Font.Size = 12;

            //Добавляем таблицу с информацией
            Word.Table oTable1;
            Word.Range wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            int a = 2;//строки
            int b = 4;//столбцы
            oTable1 = oDoc.Tables.Add(wrdRng, a, b, ref oMissing, ref oMissing);
            oTable1.Range.ParagraphFormat.SpaceAfter = 6;

            //oTable1.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
            oTable1.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
            oTable1.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleDoubleWavy;
            oTable1.Cell(1, 1).Range.Text = "Первая труба";
            oTable1.Cell(1, 2).Range.Text = infoForGraphicsPipeToFile1.Pipename1;
            oTable1.Cell(1, 3).Range.Text = "Оптимальный угол";
            oTable1.Cell(1, 4).Range.Text = Convert.ToString(infoForGraphicsPipeToFile1.corner);
            oTable1.Cell(2, 1).Range.Text = "Вторая труба";
            oTable1.Cell(2, 2).Range.Text = infoForGraphicsPipeToFile1.Pipename2;
            oTable1.Cell(2, 3).Range.Text = "Дуговое расстояние";
            oTable1.Cell(2, 4).Range.Text = Convert.ToString(Math.Truncate((infoForGraphicsPipeToFile1.corner * Math.PI * Convert.ToDouble(textBox1.Text)) / 360));

            oTable1.Cell(1, 1).Range.Bold = 2;
            oTable1.Cell(1, 2).Range.Bold = 0;
            oTable1.Cell(1, 3).Range.Bold = 2;
            oTable1.Cell(1, 4).Range.Bold = 0;
            oTable1.Cell(2, 1).Range.Bold = 2;
            oTable1.Cell(2, 2).Range.Bold = 0;
            oTable1.Cell(2, 3).Range.Bold = 2;
            oTable1.Cell(2, 4).Range.Bold = 0;





            oPara1.Range.InsertParagraphAfter();
            //Insert a paragraph at the beginning of the document.
            Word.Paragraph oPara2;
            oPara2 = oDoc.Content.Paragraphs.Add(ref oMissing);
            string HeadTwo = "Смещения кромок";
            oPara2.Range.Text = HeadTwo;
            oPara2.Range.Font.Size = 16;
            oPara2.Range.Font.Bold = 1;
            oPara2.Format.SpaceAfter = 24;    //24 pt spacing after paragraph.
            oPara2.Range.InsertParagraphAfter();
            oPara2.Range.Font.Size = 12;


            //int a = OffsetOneWeldedJoint.Offsets.Count;
            a = OffsetOneWeldedJoint.Offsets.Count;//строки
            b = 2;//столбцы
            Word.Table oTable;
            wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oTable = oDoc.Tables.Add(wrdRng, a, b, ref oMissing, ref oMissing);
            oTable.Range.ParagraphFormat.SpaceAfter = 6;
            
            string strText;

            for (int i = 0; i < a; i++)
            {
                //strText = Convert.ToString(OffsetOneWeldedJoint.Offsets[a].PointOffset);
                strText = Convert.ToString(Math.Round((OffsetOneWeldedJoint.Offsets[i].PointOffset), 1));
                oTable.Cell(i+1, 2).Range.Text = strText;
                oTable.Cell(i+1, 1).Range.Text = Convert.ToString(i);
            }

            /*oTable.Rows[1].Range.Font.Bold = 2;
            oTable.Cell(1,1).Row.Height=3;
            oTable.Rows[1].Range.Font.Italic = 1;*/

            //Clipboard.SetImage(pictureBox5.Image);
            //oWord.ActiveDocument.Paragraphs[1].Range.Paste();

        }
        }
    }

