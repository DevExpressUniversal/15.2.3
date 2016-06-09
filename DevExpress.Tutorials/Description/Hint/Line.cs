#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Windows;
namespace DevExpress.NoteHint
{
  class Line
  {
	public Line(double x1, double y1, double x2, double y2)
	{
	  X1 = x1;
	  Y1 = y1;
	  X2 = x2;
	  Y2 = y2;
	}
	double Len(Point vec)
	{
	  return Math.Sqrt(Math.Pow(vec.X, 2) + Math.Pow(vec.Y, 2));
	}
	public static double GetLength(double x1, double y1, double x2, double y2)
	{
	  return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
	}
	public double GetLength()
	{
	  return GetLength(X1, Y1, X2, Y2);
	}
	public bool ContainsPoint(Point point)
	{
	  double firstPointDistance = GetLength(point.X, point.Y, X1, Y1);
	  double secondPointDistance = GetLength(point.X, point.Y, X2, Y2);
	  return (int)(firstPointDistance + secondPointDistance) == (int)GetLength();
	}
	public double GetDistance(Point point, out Point intersectionPoint)
	{
	  double x = (X1 * Math.Pow(Y2 - Y1, 2) + point.X * Math.Pow(X2 - X1, 2) + (X2 - X1) * (Y2 - Y1) * (point.Y - X1)) / (Math.Pow(Y2 - Y1, 2) + Math.Pow(X2 - X1, 2));
	  if (double.IsNaN(x))
		x = X1;
	  double y = ((X2 - X1) * (point.X - x) / (Y2 - Y1)) + point.Y;
	  if (double.IsNaN(y))
		y = Y1;
	  intersectionPoint = new Point(x, y);
	  return Len(new Point(intersectionPoint.X - point.X, intersectionPoint.Y - point.Y));
	}
	public double GetDistanceToCenterPoint(Point point, out Point intersectPoint)
	{
	  const int minLengthForMidPointCalculation = 40;
	  double firstPointDistance = GetLength(point.X, point.Y, X1, Y1);
	  double secondPointDistance = GetLength(point.X, point.Y, X2, Y2);
	  double pointOffset = 0.5;
	  double lineLength = GetLength();
	  if (lineLength > minLengthForMidPointCalculation)
		pointOffset = firstPointDistance > secondPointDistance ? 0.6 : 0.3;
	  intersectPoint = new Point(X1 + (X2 - X1) * pointOffset, Y1 + (Y2 - Y1) * pointOffset);
	  return GetLength(intersectPoint.X, intersectPoint.Y, point.X, point.Y);
	}
	public double X1 { get; private set; }
	public double Y1 { get; private set; }
	public double X2 { get; private set; }
	public double Y2 { get; private set; }
  }
}
