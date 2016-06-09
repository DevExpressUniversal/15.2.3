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
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public interface IAxisMapping {
		double Lenght { get; }
		double GetAxisValue(double value);
		double GetRoundedAxisValue(double value);
		double GetClampedAxisValue(double value);
		double GetRoundedClampedAxisValue(double value);
		double GetInternalCoord(double coordinate, double length);
		int GetRoundedInterval(double interval);
		double Clamp(double value);
	}
	public class AxisMappingEx : IAxisMapping {
		readonly IAxisData axis;
		readonly double axisLenght;
		public double Lenght { get { return axisLenght; } }
		protected virtual double ClampSize { get { return axisLenght + 1; } }
		public AxisMappingEx(IAxisData axis, double axisLenght) {
			this.axis = axis;
			this.axisLenght = Math.Max(0, axisLenght - 1);
		}
		double ApplyReverse(double axisValue) {
			return axis.Reverse ? axisLenght - axisValue : axisValue;
		}
		public double GetAxisValue(double value) {
			ITransformation transform = axis.AxisScaleTypeMap.Transformation;
			IMinMaxValues range = new MinMaxValues(transform.TransformForward(axis.VisualRange.Min), transform.TransformForward(axis.VisualRange.Max));
			return ApplyReverse(AxisCoordCalculator.GetCoord(range, transform.TransformForward(value), axisLenght));
		}
		public double GetRoundedAxisValue(double value) {
			return MathUtils.StrongRound(GetAxisValue(value));
		}
		public double GetClampedAxisValue(double value) {
			ITransformation transform = axis.AxisScaleTypeMap.Transformation;
			IMinMaxValues range = new MinMaxValues(transform.TransformForward(axis.VisualRange.Min), transform.TransformForward(axis.VisualRange.Max));
			return ApplyReverse(AxisCoordCalculator.GetClampedCoord(range, transform.TransformForward(value), axisLenght));
		}
		public double GetRoundedClampedAxisValue(double value) {
			return MathUtils.StrongRound(GetClampedAxisValue(value));
		}
		public double GetInternalCoord(double coordinate, double length) {
			coordinate = ApplyReverse(coordinate);
			double ratio = coordinate / length;
			return AxisCoordCalculator.GetInternalValue(axis.AxisScaleTypeMap.Transformation.TransformForward(axis.VisualRange.Min), axis.AxisScaleTypeMap.Transformation.TransformForward(axis.VisualRange.Max), ratio);
		}
		public int GetRoundedInterval(double interval) {
			return (int)(Math.Abs(GetRoundedAxisValue(axis.WholeRange.Min + interval) - GetRoundedAxisValue(axis.WholeRange.Min)));
		}
		public double Clamp(double value) {
			return value < 0 ? 0 : Math.Min(value, ClampSize);
		}
	}
}
