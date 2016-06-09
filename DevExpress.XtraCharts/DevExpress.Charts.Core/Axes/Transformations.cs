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
using System.Collections.Generic;
namespace DevExpress.Charts.Native {
	public interface ILogarithmic {
		bool Enabled { get; }
		double Base { get; }
	}
	public interface ITransformation {
		double TransformForward(double value);
		double TransformBackward(double value);
	}
	public abstract class Transformation : ITransformation {
		public static MinMaxValues TransformForward(IAxisRangeData rangeData, ITransformation transformation) {
			return new MinMaxValues(transformation.TransformForward(rangeData.Min), transformation.TransformForward(rangeData.Max));
		}
		public abstract bool IsIdentity { get; } 
		public abstract double TransformForward(double value);
		public abstract double TransformBackward(double value);
		public virtual void Reset() {}
		public virtual void Update(params double[] values) {}
		public virtual void CompleteUpdate() {}
		public double[] TransformForward(double[] values) {
			double[] result = new double[values.Length];
			for (int i = 0; i < values.Length; i++)
				result[i] = TransformForward(values[i]);
			return result;
		}
		public IList<double> TransformForward(IList<double> values) {
			List<double> result = new List<double>(values.Count);
			foreach (double value in values)
				result.Add(TransformForward(value));
			return result;
		}
		public void Update(IAxisValueContainer container) {
			object value = container.GetAxisValue();
			if (value != null)
				try {
					Update(Convert.ToDouble(value));
				}
				catch {
				}
		}
	}
	public class IdentityTransformation : Transformation {
		public override bool IsIdentity { get { return true; } } 
		public override double TransformForward(double value) {
			return value;
		}
		public override double TransformBackward(double value) {
			return value;
		}
	}
	public class LogarithmicTransformation : Transformation {
		const double defaultIndentFromZero = 1.0;
		readonly double logarithmicBase;
		readonly double indentFromZero;
		double minAbsValue;
		double minLogValue;
		public double LogarithmicBase { get { return logarithmicBase; } }
		public double MinLogValue { get { return minLogValue; } }
		public override bool IsIdentity { get { return false; } } 
		public LogarithmicTransformation(double logarithmicBase) : this(logarithmicBase, defaultIndentFromZero) {
		}
		public LogarithmicTransformation(double logarithmicBase, double indentFromZero) {
			this.logarithmicBase = logarithmicBase;
			this.indentFromZero = indentFromZero;
		}
		public override double TransformForward(double value) {			
			if (IsSpecialValue(value))
				return value;
			double transformedAbsValue = Math.Log(Math.Abs(value), logarithmicBase) - minLogValue;
			if (transformedAbsValue < 0)
				transformedAbsValue = 0;
			return Math.Sign(value) * transformedAbsValue;
		}
		public override double TransformBackward(double value) {
			return IsSpecialValue(value) ? value : (Math.Pow(logarithmicBase, Math.Abs(value) + minLogValue) * Math.Sign(value));
		}
		public override void Reset() {
			minAbsValue = Double.NaN;
		}
		public override void Update(params double[] values) {
			foreach (double value in values) {
				double absValue = Math.Abs(value);
				if (absValue > 0 && (Double.IsNaN(minAbsValue) || absValue < minAbsValue))
					minAbsValue = absValue;
			}
		}
		public override void CompleteUpdate() {
			if (Double.IsNaN(minAbsValue))
				minAbsValue = 1.0;
			minLogValue = Math.Floor(Math.Log(minAbsValue, logarithmicBase)) - indentFromZero;
		}
		bool IsSpecialValue(double value) {
			return value == 0 || Double.IsNaN(value) || Double.IsNegativeInfinity(value) || Double.IsPositiveInfinity(value);
		}
	}
}
