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
	public abstract class PdfCustomFunction : PdfFunction {
		const string domainDictionaryKey = "Domain";
		const string rangeDictionaryKey = "Range";
		static void Restrict(double[] values, IList<PdfRange> ranges) {
			int rangeSize = ranges.Count;
			if (values.Length == rangeSize)
				for (int i = 0; i < rangeSize; i++) {
					double value = values[i];
					PdfRange range = ranges[i];
					double min = range.Min;
					if (value < min)
						values[i] = min;
					else {
						double max = range.Max;
						if (value > max)
							values[i] = max;
					}
				}
		}
		protected static double Interpolate(double x, double xmin, double xmax, PdfRange yRange) {
			double ymin = yRange.Min;
			double addition = (x - xmin) * (yRange.Max - ymin);
			double divider = xmax - xmin;
			return (addition + divider == addition) ? ymin : (ymin + addition / divider);
		}
		protected static bool CompareRanges(IList<PdfRange> range1, IList<PdfRange> range2) {
			if (range1 == null)
				return range2 == null;
			int count = range1.Count;
			if (range2 == null || range2.Count != count)
				return false;
			for (int i = 0; i < count; i++) {
				PdfRange r1 = range1[i];
				PdfRange r2 = range2[i];
				if (r1.Min != r2.Min || r1.Max != r2.Max)
					return false;
			}
			return true;
		}
		protected static IList<object> ToObjectArray(IList<PdfRange> ranges) {
			List<object> result = new List<object>(ranges.Count * 2);
			foreach (PdfRange range in ranges) {
				result.Add(range.Min);
				result.Add(range.Max);
			}
			return result;
		}
		internal static PdfCustomFunction PerformParse(object value) {
			byte[] data = null;
			PdfReaderDictionary dictionary = value as PdfReaderDictionary;
			if (dictionary == null) {
				PdfReaderStream stream = value as PdfReaderStream;
				if (stream == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				dictionary = stream.Dictionary;
				data = stream.GetData(true);
			}
			int? functionType = dictionary.GetInteger(FunctionTypeDictionaryKey);
			if (!functionType.HasValue)
				PdfDocumentReader.ThrowIncorrectDataException();
			switch (functionType.Value) {
				case PdfSampledFunction.Number:
					if (data == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					return new PdfSampledFunction(dictionary, data);
				case PdfExponentialInterpolationFunction.Number:
					if (data != null)
						PdfDocumentReader.ThrowIncorrectDataException();
					return new PdfExponentialInterpolationFunction(dictionary);
				case PdfStitchingFunction.Number:
					return new PdfStitchingFunction(dictionary);
				case PdfPostScriptCalculatorFunction.Number:
					if (data == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					return new PdfPostScriptCalculatorFunction(dictionary, data);
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					return null;
			}
		}
		internal static PdfCustomFunction Parse(PdfObjectCollection objects, object value) {
			return PerformParse(objects.TryResolve(value));
		}
		readonly IList<PdfRange> domain;
		readonly IList<PdfRange> range;
		public IList<PdfRange> Domain { get { return domain; } }
		public IList<PdfRange> Range { get { return range; } }
		protected internal virtual int RangeSize { get { return range.Count; } }
		protected virtual bool ShouldCheckEmptyRange { get { return true; } }
		protected abstract int FunctionType { get; }
		protected PdfCustomFunction(IList<PdfRange> domain, IList<PdfRange> range) {
			if (domain == null)
				throw new ArgumentNullException("domain");
			if (domain.Count == 0)
				throw new ArgumentException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectListSize), "domain");
			if (range == null && ShouldCheckEmptyRange)
				throw new ArgumentNullException("range");
			else if (range.Count == 0)
				throw new ArgumentException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectListSize), "range");
			this.domain = domain;
			this.range = range;
		}
		protected PdfCustomFunction(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			if (dictionary == null)
				return;
			domain = PdfDocumentReader.CreateDomain(dictionary.GetArray(domainDictionaryKey));
			IList<object> rangeArray = dictionary.GetArray(rangeDictionaryKey);
			if (rangeArray != null)
				range = PdfDocumentReader.CreateDomain(rangeArray);
			else if (ShouldCheckEmptyRange)
				PdfDocumentReader.ThrowIncorrectDataException();
		}
		protected internal override bool IsSame(PdfFunction function) {
			PdfCustomFunction customFunction = function as PdfCustomFunction;
			return customFunction != null && GetType() == customFunction.GetType() && CompareRanges(domain, customFunction.domain) && CompareRanges(range, customFunction.range);
		}
		protected internal override double[] Transform(double[] arguments) {
			Restrict(arguments, Domain);
			double[] results = PerformTransformation(arguments);
			IList<PdfRange> range = Range;
			if (range != null)
				Restrict(results, range);
			return results;
		}
		protected internal override object Write(PdfObjectCollection objects) {
			return objects.AddObject(this);
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return FillDictionary(objects);
		}
		protected virtual PdfWriterDictionary FillDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.Add(FunctionTypeDictionaryKey, FunctionType);
			dictionary.Add(domainDictionaryKey, ToObjectArray(domain));
			if (range != null)
				dictionary.Add(rangeDictionaryKey, ToObjectArray(range));
			return dictionary;
		}
		protected abstract double[] PerformTransformation(double[] arguments);
	}
}
