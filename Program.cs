using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace Hemmings_NN
{
    class HNN
    {
        double eM = 0.1; //Порог принятия решения
        int M = new int(); //Размерность эталонных векторов вектора
        int N = new int(); //Число эталонных векторов

        double[,] w1; //Матрица весов между первым и вторым слоями нейронки                
        double[,] w2; //Матрица весов для обратных связей третьего слоя

        double[] befres; //Вектор для сохранения рельтатов предыдущей итерации третьего слоя
        double[] res; //Вектор для результатов текущей итерации третьего слоя

        double[] vec; //Вектор, который будем классифицировать

        int k = 0;//Счетчик числа итерации, за которые нейросетка закончила работу. Ввел от балды, поидее не нужен



        public HNN()
        {
            LoadVectors();
            Random rnd = new Random();
            double e = (rnd.NextDouble()+0.0000000000000000001)/(-N);/*Число, необходимое для таблицы весов w2. */
            //Заполняем матирицу весов w2
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    if (i == j)
                        w2[i, j] = 1;
                    else w2[i,j] = e;


            //Вводим с клавы вектор, который надо классифицировать 
            Console.WriteLine("Ввведите {0} значений оцениваемого ветора. Значения должны находиться в интервале [-1,1]", M);
            for (int i = 0; i < M; i++)
                vec[i] = Convert.ToDouble(Console.ReadLine());


            //Проход по второму слою нейронки и вычисление выходов. Для вывода результатов юзану befres, он все равно пока не нужен.
            for (int i = 0; i < N; i++)
            {
                //Для каждого нейрона второго слоя нахожу сумму его входных параметров.
                for (int j = 0; j < M; j++)
                    befres[i] += vec[j] * w1[i, j];
                befres[i]= Activate(Activate(befres[i])); 
            }

            do
            {
                k++;
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                        res[i] += befres[j] * w2[i, j];
                    res[i] = Activate(res[i]);
                }
                if (Check())
                    break;

                for (int i = 0; i < N; i++)
                {
                    befres[i] = res[i];
                    res[i] = 0;
                }
            } while (true);


            //Тута мы прост просматриваем выходной вектор на предмет того, сколько там положительных значений. Если оно одно, то записывается его номер в векторе.
            //Если больше (или мб меньше, если такое возможно), то выводится сообщение, что нейронка не может в классифиуировать этот вектор.
            int n = 0, a=0;
            for(int i = 0;i<N;i++)
                if (res[i] > 0)
                {
                    n++;
                    a = i;
                }
            if (n != 1)
                Console.WriteLine("Нейросеть не смогла классифицировать вектор. Затрачено {0} итераций.", k);
            else
                Console.WriteLine("Вектор отнесен к классу №{0}. Затрачено {1} итераций.", a+1,k);
        }



        //Метод, загружающий из файла эталонные векторы
        void LoadVectors()
        {
            //Ищем нужный файл, если он норм открывается, то запускам потоковое чтение из него
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    //Считывание из файла числа эталонных векторов и их размерностей
                    string temp = sr.ReadLine();
                    string[] line = temp.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    N = int.Parse(line[0]);
                    M = int.Parse(line[1]);

                    w1 = new double[N, M];
                    w2 = new double[N, N]; //Выделение памяти
                    befres = new double[N];
                    res = new double[N];
                    vec = new double[M];

                    //Цикл считывания самих эталонных векторов. Поочередно считываем N строчек, каждую из них конвертируя в массив double
                    for (int i = 0; i < N; i++)
                    {
                        //Считываем очередную строку из файла, в которой хранятся значения столбцов текущей строки
                        //Методом Split разбиваем ее по пробелам и заполняем массив.
                        temp = sr.ReadLine();
                        line = temp.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 0; j < M; j++)
                        {
                            w1[i, j] = double.Parse(line[j])/2;
                        }
                    }
                }
        }



        //Функция активации
        double Activate(double s)
        {
            if (s <= 0)
                return 0;
            else if (s > 0 && s <= 1)
                return s;
            else return M / 2;
        }



        //Метод, проверяющий условие номированности выходного вектора третьего слоя нейронки.
        {
            double norm = new double();
            for (int i = 0; i < N; i++)
                norm += Math.Pow(res[i] - befres[i], 2);
            if (Math.Sqrt(norm) < eM)
                return true;
            else return false;
        }
    }




    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            HNN h = new HNN();
        }
    }
}
