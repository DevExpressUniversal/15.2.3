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

using DevExpress.Pdf.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Pdf {
	public class PdfStitchingFunction : PdfCustomFunction {
		internal const int Number = 3;
		const string functionsDictionaryKey = "Functions";
		const string boundsDictionaryKey = "Bounds";
		const string encodeDictionaryKey = "Encode";
		readonly PdfObjectList<PdfCustomFunction> functions;
		readonly IList<double> bounds;
		readonly IList<PdfRange> encode;
		protected override int FunctionType { get { return Number; } }
		protected override bool ShouldCheckEmptyRange { get { return false; } }
		public IList<PdfCustomFunction> Functions { get { return functions; } }
		public IList<double> Bounds { get { return bounds; } }
		public IList<PdfRange> Encode { get { return encode; } }
		protected internal override int RangeSize {
			get {
				IList<PdfRange> range = Range;
				return range == null ? functions[0].RangeSize : range.Count;
			}
		}
		internal PdfStitchingFunction(PdfReaderDictionary dictionary)
			: base(dictionary) {
			IList<PdfRange> domain = Domain;
			functions = dictionary.GetFunctions(functionsDictionaryKey, true);
			int functionsCount = functions.Count;
			encode = dictionary.GetRanges(encodeDictionaryKey);
			if (domain.Count > 1 || functionsCount < 1 || encode.Count != functionsCount)
				PdfDocumentReader.ThrowIncorrectDataException();
			int range = RangeSize;
			foreach (PdfCustomFunction function in functions)
				if (function.RangeSize != range)
					PdfDocumentReader.ThrowIncorrectDataException();
			bounds = dictionary.GetDoubleArray(boundsDictionaryKey);
			int boundsCount = bounds.Count;
			if (boundsCount != functionsCount - 1)
				PdfDocumentReader.ThrowIncorrectDataException();
			PdfRange actualDomain = domain[0];
			if (boundsCount == 0) {
				if (actualDomain.Min >= actualDomain.Max)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			else {
				double bound = bounds[0];
				if (actualDomain.Min > bound)
					PdfDocumentReader.ThrowIncorrectDataException();
				for (int i = 1; i < boundsCount; i++) {
					double boundMax = bounds[i];
					if (boundMax < bound)
						PdfDocumentReader.ThrowIncorrectDataException();
					bound = boundMax;
				}
				if (actualDomain.Max < bound)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
		}
		internal PdfStitchingFunction(IList<double> bounds, IList<PdfRange> encode, PdfObjectList<PdfCustomFunction> functions, IList<PdfRange> domain, IList<PdfRange> range)
			: base(domain, range) {
			this.bounds = bounds;
			this.encode = encode;
			this.functions = functions;
		}
		protected override double[] PerformTransformation(double[] arguments) {
			if (arguments.Length != 1)
				return arguments;
			PdfRange domain = Domain[0];
			double argument = arguments[0];
			PdfFunction function = null;
			double xmin = domain.Min;
			double xmax = domain.Max;
			PdfRange encodeRange = null;
			bool wasFound = false;
			int boundsCount = bounds.Count;
			for (int i = 0; i < boundsCount; i++) {
				xmax = bounds[i];
				if (argument < xmax) {
					function = functions[i];
					encodeRange = encode[i];
					wasFound = true;
					break;
				}
				xmin = xmax;
			}
			if (!wasFound) {
				function = functions[boundsCount];
				encodeRange = encode[boundsCount];
				xmax = domain.Max;
			}
			return function.Transform(new double[] { Interpolate(argument, xmin, xmax, encodeRange) });
		}
		protected override PdfWriterDictionary FillDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.FillDictionary(objects);
			dictionary.Add(functionsDictionaryKey, functions);
			dictionary.Add(boundsDictionaryKey, bounds);
			dictionary.Add(encodeDictionaryKey, PdfRange.ToArray(encode));
			return dictionary;
		}
	}
}
