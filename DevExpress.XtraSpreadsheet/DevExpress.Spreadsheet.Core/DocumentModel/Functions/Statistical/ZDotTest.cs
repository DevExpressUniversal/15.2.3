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
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionZDotTest
	public class FunctionZDotTest : WorksheetFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Reference | OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "Z.TEST"; } }
		public override int Code { get { return 0x406A; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value = arguments[0];
			IVector<VariantValue> vector = null;
			if (value.IsArray)
				vector = new ArrayZVector(value.ArrayValue);
			else if (value.IsCellRange)
				vector = new RangeZNVector(value.CellRangeValue);
			else {
				value = arguments[0].ToNumeric(context);
				if (value.IsError)
					return value;
			}
			value = arguments[1].ToNumeric(context);
			if (value.IsError)
				return value;
			double x = value.NumericValue;
			double sigma = 0;
			bool withSigma = arguments.Count > 2;
			if (withSigma) {
				value = arguments[2].ToNumeric(context);
				if (value.IsError)
					return value;
				sigma = value.NumericValue;
			}
			if (vector == null)
				return VariantValue.ErrorDivisionByZero;
			return GetResult(vector, x, sigma, withSigma);
		}
		VariantValue GetResult(IVector<VariantValue> vector, double x, double sigma, bool withSigma) {
			int numbersCount;
			VariantValue average = CalculateAverage(vector, out numbersCount);
			if (average.IsError)
				return average;
			if (withSigma && sigma <= 0)
				return VariantValue.ErrorNumber;
			double averageValue = average.NumericValue;
			double denominator = sigma == 0 ? CalculateStDev(vector, averageValue, numbersCount) : sigma;
			double normSDistArgument = (averageValue - x) / (denominator / Math.Sqrt(numbersCount));
			return 1.0 - FunctionNormSDist.GetResult(normSDistArgument, true);
		}
		VariantValue CalculateAverage(IVector<VariantValue> vector, out int numbersCount) {
			numbersCount = 0;
			double average = 0;
			int count = vector.Count;
			for (int i = 0; i < count; i++) {
				VariantValue value = vector[i];
				if (value.IsError)
					return value;
				if (value.IsNumeric)
					average += (value.NumericValue - average) / ++numbersCount;
			}
			if (numbersCount == 0)
				return VariantValue.ErrorValueNotAvailable;
			else if (numbersCount == 1)
				return VariantValue.ErrorDivisionByZero;
			return average;
		}
		double CalculateStDev(IVector<VariantValue> vector, double average, int numbersCount) {
			int count = vector.Count;
			double sum = 0;
			for (int i = 0; i < count; i++) {
				VariantValue value = vector[i];
				if (value.IsNumeric) {
					double dev = value.NumericValue - average;
					sum += dev * dev;
				}
			}
			return Math.Sqrt(sum / (numbersCount - 1));
		}
	}
	#endregion
}
