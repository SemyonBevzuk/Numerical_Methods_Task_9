﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerical_Methods_Task_9
{
    class Runge_Kutta_2
    {
        private FunkDelegate f; // функция
        private double h; // шаг
        private double eps; // контроль шага
        private Point currentPoint; // текущая точка 

        private bool flagStepControl; // включить или отключить контроль шага
        private int maxSteps;         // ограничение по числу шагов
        private readonly List<Point> listOfPoints = new List<Point>(); // список точек для графика
        private readonly List<MetodInfo> listIfMetodInfo = new List<MetodInfo>(); // список для таблиц
        private int countPlusH = 0; // счётчик увеличения шага
        private int countMinusH = 0; // счётчик уменьшения шага
        private int steps = 0; // число шагов в данный момент
        public void Init(double _x0, double _u0, double _h, double _eps, int _maxSteps, FunkDelegate _f, bool _flagIsHControl)
        {
            currentPoint = new Point(_x0, _u0);
            h = _h;
            eps = _eps;
            f = _f;
            maxSteps = _maxSteps;
            flagStepControl = _flagIsHControl;
            listOfPoints.Add(currentPoint);
            listIfMetodInfo.Add(new MetodInfo(steps,0,_x0,_u0,0,0,0,0,0,0,0,0));
            steps++;
        } 

        public void Run()
        {
            while (!NeedStop())
            {
                var oldH = h;
                var newPoint = MakeStep(currentPoint, h);

                var halfPoint = GetHalfPoint(currentPoint, h);

                var s = Math.Abs(GetS(newPoint, halfPoint));

                var e = Math.Abs(Math.Pow(2.0,2.0)*s);

                var uCorr = GetVCorr(newPoint, s);

                if (flagStepControl == true)
                {
                    if (s < eps/(Math.Pow(2.0, 2.0)))
                    {
                        currentPoint = newPoint;
                        h = h*2;
                        countPlusH++;
                        listOfPoints.Add(newPoint);
                    }
                    else
                    {
                        if (s > eps)
                        {
                            h = h/2.0;
                            countMinusH++;
                        }
                        else
                        {
                            currentPoint = newPoint;
                            listOfPoints.Add(newPoint);
                        }
                    }
                }
                else
                {
                    currentPoint = newPoint;
                    listOfPoints.Add(newPoint);
                }
                listIfMetodInfo.Add(
                    new MetodInfo(steps, oldH, currentPoint.X, currentPoint.V, halfPoint.V, 
                        currentPoint.V - halfPoint.V, s, e, uCorr, currentPoint.V, countMinusH, countPlusH));
                steps++;
            }
        }

        private double GetVCorr(Point nextPoint, double s)
        {
            return nextPoint.V + Math.Pow(2.0, 2.0)*s;
        }
        private double GetS(Point next_1, Point next_2)
        {
            return (next_2.V - next_1.V)/(2.0*2.0 - 1.0);
        }
        private Point GetHalfPoint(Point currentPoint, double h)
        {
            var p1 = MakeStep(currentPoint, h/2.0);
            return MakeStep(p1, h/2.0);
        }

        private Point MakeStep(Point curPoint, double h)
        {
            var x1 = GetNextX(curPoint.X, h);
            var u1 = GetNextV(curPoint.X, curPoint.V, h);
            return new Point(x1, u1);
        }
        private double GetNextX(double x, double h)
        {
            return x + h;
        }

        private double GetNextV(double x, double u, double h)
        {
            if (u < 0)
            {
                return 0;
            }
            double a = f(x + h/2.0, u + (h/2.0)*f(x,u));
            return u + h*a;
        }
        private bool NeedStop()
        {
            if (steps > maxSteps)
            {
                return true;
            }
            if (currentPoint.V < 0.01 || currentPoint.V + (h/2.0)*f(currentPoint.X, currentPoint.V) < 0.01) // контроль границы по v
            {
                return true;
            }
            return false;
        }

        public List<MetodInfo> GetMetodInfos()
        {
            return listIfMetodInfo;
        } 
        public List<Point> GetPoints()
        {
            return listOfPoints;
        }
        public double GetResultTime()
        {
            return currentPoint.X;
        }
    }
}
