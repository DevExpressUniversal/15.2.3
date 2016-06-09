#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public class PdfExponentialInterpolationFunction : PdfCustomFunction {
		internal const int Number = 2;
		const string c0DictionaryKey = "C0";
		const string c1DictionaryKey = "C1";
		const string exponentDictionaryKey = "N";
		static IList<double> CreateArray(IList<object> cArray, int n) {
			if (cArray.Count != n) 
				PdfDocumentReader.ThrowIncorrectDataException();
			IList<double> result = new List<double>(n);
			foreach (object element in cArray) 
				result.Add(PdfDocumentReader.ConvertToDouble(element));
			return result;
		}
		static bool CompareArrays(IList<double> array1, IList<double> array2) {
			int count = array1.Count;
			if (array2.Count != count)
				return false;
			for (int i = 0; i < count; i++)
				if (array1[i] != array2[i])
					return false;
			return true;
		}
		static IList<object> ToObjectArray(IList<double> array) {
			List<object> result = new List<object>(array.Count);
			foreach (double element in array)
				result.Add(element);
			return result;
		}
		readonly IList<double> c0;
		readonly IList<double> c1;
		readonly double exponent;
		public IList<double> C0 { get { return c0; } }
		public IList<double> C1 { get { return c1; } }
		public double Exponent { get { return exponent; } }
		protected internal override int RangeSize { get { return c0.Count; } }
		protected override bool ShouldCheckEmptyRange { get { return false; } }
		protected override int FunctionType { get { return Number; } }
		internal PdfExponentialInterpolationFunction(PdfReaderDictionary dictionary) : base(dictionary) {
			IList<object> c0Array = dictionary.GetArray(c0DictionaryKey);
			IList<object> c1Array = dictionary.GetArray(c1DictionaryKey);
			int n;
			IList<PdfRange> range = Range;
			if (range == null) 
				n = c0Array == null ? 1 : c0Array.Count;
			else
				n = range.Count;
			c0 = c0Array == null ? new List<double>() { 0.0 } : CreateArray(c0Array, n);
			c1 = c1Array == null ? new List<double>() { 1.0 } : CreateArray(c1Array, n);
			if (c0.Count != c1.Count) 
				PdfDocumentReader.ThrowIncorrectDataException();
			double? exponentValue = dictionary.GetNumber(exponentDictionaryKey);
			if (!exponentValue.HasValue) 
				PdfDocumentReader.ThrowIncorrectDataException();
			exponent = exponentValue.Value;
		}
		internal PdfExponentialInterpolationFunction(IList<double> c0, IList<double> c1, double exponent, IList<PdfRange> domain, IList<PdfRange> range) 
			: base(domain, range) {
			this.c0 = c0;
			this.c1 = c1;
			this.exponent = exponent;
		}
		protected internal override bool IsSame(PdfFunction function) {
			if (!base.IsSame(function))
				return false;
			PdfExponentialInterpolationFunction interpolationFunction = (PdfExponentialInterpolationFunction)function;
			return exponent == interpolationFunction.exponent && CompareArrays(c0, interpolationFunction.c0) && CompareArrays(c1, interpolationFunction.c1);
		}
		protected override double[] PerformTransformation(double[] arguments) {
			int size = RangeSize;
			if (arguments.Length != 1)
				return arguments;
			double argument = Math.Pow(arguments[0], exponent);
			double[] result = new double[size];
			for (int i = 0; i < size; i++) {
				double min = c0[i];
				result[i] = min + argument * (c1[i] - min);
			}
			return result;
		}
		protected override PdfWriterDictionary FillDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.FillDictionary(objects);
			if (c0.Count != 1 || c0[0] != 0.0)
				dictionary.Add(c0DictionaryKey, ToObjectArray(c0));
			if (c1.Count != 1 || c1[0] != 1.0)
				dictionary.Add(c1DictionaryKey, ToObjectArray(c1));
			dictionary.Add(exponentDictionaryKey, exponent);
			return dictionary;
		}
	}
}
