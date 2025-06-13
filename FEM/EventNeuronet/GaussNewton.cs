using CsvHelper;
using ScottPlot.Drawing.Colormaps;
using ScottPlot.MarkerShapes;
using System;
using System.Diagnostics.PerformanceData;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Converters;
using System.Windows.Media.Media3D;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

using Vector = Numerics.Vector<double>;
using Matrix = Numerics.Matrix<double>;
using MathNet.Numerics.Distributions;

namespace FEM;

//: GaussNewton
public partial class Neuronet
{
    int rows = 40;
    int cols = 66;
    int maxIterations = 100;

    //: Дорешиваем задачу Гауссом Ньютоном
    private void GaussNewton_Click(object sender, RoutedEventArgs e)
    {
        // Стратовые параметры (X, Z, Width, Height, Sigma)
        // -------------------------------------- Тесты Магистерская ----------------------------------- //

        // Первая модель
        // Нейронка
        //Vector parameters = new(new[] { 865.1261, -4862.1929, 1677.4897, 2745.3149, 23.9978 });
        // Стартовая модель
        //Vector parameters = new(new[] { 0.0, -3000.0, 1000.0, 1000.0, 5.0 });
        // Истинная модель
        //Vector trueParameters = new(new[] { 1000.0000, -5000.0000, 1500.0000, 3000.0000, 20.0000 });

        // Вторая модель
        // Нейронка
        //Vector parameters = new(new[] { 7815.7080, -5393.3276, 6020.5977, 1501.0981, 402.1259 });
        // Стартовая модель
        Vector parameters = new(new[] { 1000.0, -3000.0, 2000.0, 2000.0, 50.0 });
        // Истинная модель
        Vector trueParameters = new(new[] { 8000.0000, -6000.0000, 4000.0000, 3500.0000, 400.0000 });

        string logFilePath = @"gauss/log.txt";
        File.WriteAllText(logFilePath, "%***** Лог итераций метода Гаусса-Ньютона *****%\n"); // Очищаем файл перед началом

        Stopwatch stopwatch = Stopwatch.StartNew(); // Засекаем время работы

        Matrix trueSignals = SolutionDirectTask(trueParameters);
        Matrix currSignals = SolutionDirectTask(parameters);
        //AddNoiseToMatrix(trueSignals, noiseLevel: 0.01); // 1% шум
        double previousFunctional = ComputeFunctional(trueSignals, currSignals, trueParameters, parameters);
        
        double EPS_Functional = 1e-7;
        double EPS_Delta = 1e-6;
        double EPS_MinValue = 1e-7;
        double alpha = 1e-10;

        // Открываем файл в режиме дозаписи
        using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
        {
            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                StringBuilder log = new StringBuilder($"{iteration + 1} итерация: \n");
                Matrix jacobian = Jacobian(parameters, currSignals);
                Vector residual = ComputeResidual(trueSignals, currSignals);
                (Matrix hessian, Vector gradient) = CreateHessianGradient(jacobian, residual);

                log.AppendLine($"Предыдущий функционал = {previousFunctional}\n");
                Trace.WriteLine($"Предыдущий функционал = {previousFunctional}\n");
                log.AppendLine($"Применяем alpha-регуляризацию\n");

                // Применяем alpha регуляризацию
                //Vector bestDelta = new Vector(parameters.Length);
                //double bestFunctionalAlpha = double.MaxValue;
                    
                Matrix copyHessian = (Matrix)hessian.Clone();
                Vector copyGradient = (Vector)gradient.Clone();

                for (int i = 0; i < copyHessian.Rows; i++)
                    copyHessian[i, i] += alpha;

                // Число обусловленности
                var matrix = MathNet.Numerics.LinearAlgebra.Matrix<double>.
                                Build.DenseOfArray(copyHessian.ToArray());
                var svd = matrix.Svd();
                double conditionNumber = svd.S.Maximum() / svd.S.Minimum();
                log.AppendLine($"Alpha = {alpha}, cond = {conditionNumber}\n");
                Trace.WriteLine($"Alpha = {alpha}, cond = {conditionNumber}");

                // Решаем СЛАУ
                Vector delta = SolveSLAE_Cholesky(copyHessian, copyGradient);

                double deltaNorm = delta.Norm;
                if (deltaNorm > 1000)
                    delta *= 1000 / deltaNorm;

                parameters += delta;
                currSignals = SolutionDirectTask(parameters);
                double currFunctional = ComputeFunctional(trueSignals, currSignals, trueParameters, parameters);

                // Лог
                string logEntry =
                    $"Результат\n" +
                    $"Delta = {delta}\n" +
                    $"||delta|| = {delta.Norm}\n" +
                    $"Параметры = {parameters}\n" +
                    $"Точность = {(parameters - trueParameters).Norm}\n" +
                    $"Функционал на предыдущей итерации = {previousFunctional}\n" +
                    $"Функционал на текущей итерации = {currFunctional}\n" +
                    $"------------------------------\n";
                log.AppendLine(logEntry);
                Trace.WriteLine(logEntry);
                writer.WriteLine(log.ToString());
                writer.Flush();

                // Условия остановки:
                if (Math.Abs(previousFunctional - currFunctional) < EPS_Functional)
                    break;

                if (delta.Norm < EPS_Delta)
                    break;

                if (currFunctional < EPS_MinValue)
                    break;

                previousFunctional = currFunctional;
            }
            stopwatch.Stop();
            writer.WriteLine($"Общее время выполнения: {stopwatch.Elapsed:hh\\:mm\\:ss\\.fff}\n");
        }
    }

    // Добавление шума
    private void AddNoiseToMatrix(Matrix matrix, double noiseLevel)
    {
        Random random = new Random();

        for (int i = 0; i < matrix.Rows; i++)
        {
            for (int j = 0; j < matrix.Columns; j++)
            {
                double value = matrix[i, j];
                double noise = noiseLevel * value * (2 * random.NextDouble() - 1); // шум [-x..x]
                matrix[i, j] += noise;
            }
        }
    }

    // Проверка параметров на границу
    bool CheckParameters(Vector parameters)
    {
        (double Min, double Max)[] parameterBounds = new (double, double)[]
        {
            (-5500.0, 20500.0),
            (-15500.0, -750),
            (950.0, 8500.0),
            (950.0, 8500.0),
            (0.01, 10500.0)
        };

        //for (int i = 0; i < parameters.Length; i++)
        //{
        //    if (parameters[i] < parameterBounds[i].Min || parameters[i] > parameterBounds[i].Max)
        //    {
        //        return false;
        //    }
        //}
        return true;
    }

    // Функционал
    private double ComputeFunctional(Matrix trueSignals, Matrix currSignals, Vector trueParameters, Vector currParameters)
    {
        (double Min, double Max)[] parameterBounds = new (double, double)[]
        {
            (-5500.0, 20500.0),
            (-15500.0, -750),
            (950.0, 8500.0),
            (950.0, 8500.0),
            (0.01, 11000.0)
        };

        double deltaEps = 0;
        for (int i = 0; i < trueSignals.Rows; i++)
        {
            for (int j = 0; j < trueSignals.Columns; j++)
            {
                double diff = trueSignals[i, j] - currSignals[i, j];
                deltaEps += diff * diff;
            }
        }

        //double error = 0;
        //for (int i = 0; i < trueParameters.Length; i++)
        //{
        //    double diff = currParameters[i] - trueParameters[i];
        //    error += diff * diff;
        //}

        double penalty = 0.0;
        for (int i = 0; i < currParameters.Length; i++)
        {
            if (currParameters[i] < parameterBounds[i].Min)
                penalty += Math.Pow(currParameters[i] - parameterBounds[i].Min, 2);
            else if (currParameters[i] > parameterBounds[i].Max)
                penalty += Math.Pow(currParameters[i] - parameterBounds[i].Max, 2);
        }
        double penaltyCoeff = 0;

        //double alpha = 0; // Вот здесь занулю пока что ошибку
        //return deltaEps + alpha * error;

        return deltaEps + penaltyCoeff * penalty;
    }

    // Вычимление Якобиана
    private Matrix Jacobian(Vector parameters, Matrix matrix_iter)
    {
        Matrix jacobian = new Matrix(rows * cols, parameters.Length);
        Vector steps = new Vector(new double[] { 10, 10, 10, 10, 0.01 });

        // Односторонняя разность
        for (int i = 0; i < parameters.Length; i++)
        {
            Vector paramPlus = (Vector)parameters.Clone();

            paramPlus[i] += steps[i];
            Matrix matrixPlus = SolutionDirectTask(paramPlus);

            for (int j = 0; j < rows; j++)
            {
                for (int k = 0; k < cols; k++)
                {
                    double dF = (matrixPlus[j, k] - matrix_iter[j, k]) / steps[i];
                    jacobian[j * cols + k, i] = dF;
                }
            }
        }

        // Центральная разность
        //for (int i = 0; i < parameters.Length; i++)
        //{
        //    Vector paramPlus = (Vector)parameters.Clone();
        //    Vector paramMinus = (Vector)parameters.Clone();
        //    paramPlus[i] += steps[i];
        //    paramMinus[i] -= steps[i];

        //    Matrix matrixPlus = SolutionDirectTask(paramPlus);
        //    Matrix matrixMinus = SolutionDirectTask(paramMinus);

        //    for (int j = 0; j < rows; j++)
        //    {
        //        for (int k = 0; k < cols; k++)
        //        {
        //            double dF = (matrixPlus[j, k] - matrixMinus[j, k]) / (2 * steps[i]);
        //            jacobian[j * cols + k, i] = dF;
        //        }
        //    }
        //}

        for (int i = 0; i < jacobian.Columns; i++)
        {
            double norm = 0.0;
            for (int j = 0; j < jacobian.Rows; j++)
            {
                norm += jacobian[j, i] * jacobian[j, i];
            }
            norm = Math.Sqrt(norm);
            Trace.WriteLine($"||J_col[{i}]|| = {norm}");
        }

        return jacobian;
    }

    // Вычисление отклонения
    private Vector ComputeResidual(Matrix matrixLeft, Matrix matrixRight)
    {
        Vector residual = new Vector(rows * cols);
        int index = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                residual[index++] = matrixLeft[i, j] - matrixRight[i, j];
            }
        }

        return residual;
    }

    // Итерация Гаусса-Ньютона
    private (Matrix, Vector) CreateHessianGradient(Matrix J, Vector residual)
    {
        Matrix Jtrans = J.Transposition();

        Matrix hessian = Jtrans * J;

        Vector gradient = Jtrans * residual;
        
        return (hessian, gradient);
    }

    // Решение СЛАУ LLt
    private Vector SolveSLAE_Cholesky(Matrix A, Vector b)
    {
        int n = A.Rows;
        Matrix L = new Matrix(n, n);

        // Шаг 1: Разложение A = L * L^T
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                double sum = 0.0;
                for (int k = 0; k < j; k++)
                    sum += L[i, k] * L[j, k];

                if (i == j)
                    L[i, j] = Math.Sqrt(A[i, i] - sum);
                else
                    L[i, j] = (A[i, j] - sum) / L[j, j];
            }
        }

        // Шаг 2: Прямой ход: L * y = b
        Vector y = new Vector(n);
        for (int i = 0; i < n; i++)
        {
            double sum = 0.0;
            for (int k = 0; k < i; k++)
                sum += L[i, k] * y[k];

            y[i] = (b[i] - sum) / L[i, i];
        }

        // Шаг 3: Обратный ход: L^T * x = y
        Vector x = new Vector(n);
        for (int i = n - 1; i >= 0; i--)
        {
            double sum = 0.0;
            for (int k = i + 1; k < n; k++)
                sum += L[k, i] * x[k]; // т.к. транспонированная L

            x[i] = (y[i] - sum) / L[i, i];
        }

        return x;
    }

    //: Решение прямой задачи
    private Matrix SolutionDirectTask(Vector param)
    {
        // Частоты (20 штук)
        double[] nuList = new[] {
           0.0001, 0.0004, 0.0016, 0.0064,
           0.01, 0.02, 0.04, 0.08,
           0.1, 0.2, 0.4, 0.8,
           1, 2, 4, 8, 32, 128, 256, 500
        };

        // Приемники (33 штуки)
        double[] receiversList = new double[] {
            0, 500, 1000, 1500, 2000, 2500, 3000,
            3500, 4000, 4500, 5000, 5500, 6000,
            6500, 7000, 7500, 8000, 8500, 9000,
            9500, 10000, 10500, 11000, 11500, 12000,
            12500, 13000, 13500, 14000, 14500, 15000,
            15500, 16000
        };

        // Параметры
        double X, Z, Width, Height, Sigma;
        (X, Z, Width, Height, Sigma) = (param[0], param[1], param[2], param[3], param[4]);

        // Путь к exe файлы
        string path = @"mtz_2d";

        // Новая точки объекта
        Vector begin = new Vector(2);
        Vector end = new Vector(2);
        begin[0] = X - Width / 2.0;
        begin[1] = Z - Height / 2.0;
        end[0] = X + Width / 2.0;
        end[1] = Z + Height / 2.0;

        StringBuilder Rok = new StringBuilder();
        StringBuilder Fi = new StringBuilder();

        Matrix output = new Matrix(rows, cols);
        for (int k = 0; k < nuList.Length; k++)
        {
            // Решение вместо моего для первого направления тока
            MTZ2D mtz2D = new MTZ2D();
            mtz2D.WriteFileMtz2D_Full(nuList[k], Sigma, receiversList, begin, end, path, 0);
            mtz2D.RunMtz2D(path);
            mtz2D.ReadSolve(path);

            // Решение для другого напраления тока
            MTZ2D mtz2D_2 = new MTZ2D();
            mtz2D_2.WriteFileMtz2D_Full(nuList[k], Sigma, receiversList, begin, end, path, 1);
            mtz2D_2.RunMtz2D(path);
            mtz2D_2.ReadSolve(path);

            for (int j = 0; j < cols / 2; j++)
            {
                output[k, j] = mtz2D.Rk[j];
                output[rows / 2 + k, j] = mtz2D.Fi[j];
                output[k, cols / 2 + j] = mtz2D_2.Rk[j];
                output[rows / 2 + k, cols / 2 + j] = mtz2D_2.Fi[j];
            }
        }

        return output;
    }
}