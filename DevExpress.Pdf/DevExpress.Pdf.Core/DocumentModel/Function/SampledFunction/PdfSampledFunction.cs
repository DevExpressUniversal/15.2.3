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
	public class PdfSampledFunction : PdfCustomFunction {
		internal const int Number = 0;
		const string sizeDictionaryKey = "Size";
		const string bitsPerSampleDictionaryKey = "BitsPerSample";
		const string orderDictionaryKey = "Order";
		const string encodeDictionaryKey = "Encode";
		const string decodeDictionaryKey = "Decode";
		static IList<PdfRange> CreateRangeArray(IList<object> array, int size) {
			if (array.Count != size * 2) 
				PdfDocumentReader.ThrowIncorrectDataException();
			IList<PdfRange> result = new List<PdfRange>(size); 
			for (int i = 0, index = 0; i < size; i++) {
				double min = PdfDocumentReader.ConvertToDouble(array[index++]);
				double max = PdfDocumentReader.ConvertToDouble(array[index++]);
				result.Add(new PdfRange(min, max));
			}
			return result; 
		}
		readonly IList<int> size;
		readonly int bitsPerSample;
		readonly bool isCubicInterpolation;
		readonly IList<PdfRange> encode;
		readonly IList<PdfRange> decode;
		readonly long[] samples; 
		public IList<int> Size { get { return size; } }
		public int BitsPerSample { get { return bitsPerSample; } }
		public bool IsCubicInterpolation { get { return isCubicInterpolation; } }
		public IList<PdfRange> Encode { get { return encode; } }
		public IList<PdfRange> Decode { get { return decode; } }
		public long[] Samples { get { return samples; } }
		int SamplesCount {
			get {
				int samplesCount = 1;
				int m = size.Count;
				for (int i = 0; i < m; i++)
					samplesCount *= size[i];
				return samplesCount * Range.Count;
			}
		}
		long MaxSampleValue { 
			get { 
				switch (bitsPerSample) {
					case 1:
						return 1;
					case 2:
						return 3;
					case 4:
						return 15;
					case 12:
						return 4095;
					case 16:
						return 65535;
					case 24:
						return 16777215;
					case 32:
						return 4294967295;
					default:
						return 255;
				}
			} 
		}
		protected override int FunctionType { get { return Number; } }
		internal PdfSampledFunction(PdfReaderDictionary dictionary, byte[] data) : base(dictionary) {
			IList<object> sizeArray = dictionary.GetArray(sizeDictionaryKey);
			if (data == null || sizeArray == null) 
				PdfDocumentReader.ThrowIncorrectDataException();
			int m = sizeArray.Count;
			size = new List<int>(m);
			foreach (object element in sizeArray) {
				if (!(element is int)) 
					PdfDocumentReader.ThrowIncorrectDataException();
				size.Add((int)element);
			}
			int? bitsPerSampleValue = dictionary.GetInteger(bitsPerSampleDictionaryKey);
			if (!bitsPerSampleValue.HasValue) 
				PdfDocumentReader.ThrowIncorrectDataException();
			bitsPerSample = bitsPerSampleValue.Value;
			int? order = dictionary.GetInteger(orderDictionaryKey);
			if (order.HasValue) 
				switch (order.Value) {
					case 1: 
						isCubicInterpolation = false;
						break;
					case 3:
						isCubicInterpolation = true;
						break;
					default: 
						PdfDocumentReader.ThrowIncorrectDataException();
						break;
				}
			IList<object> encodeArray = dictionary.GetArray(encodeDictionaryKey);
			encode = encodeArray == null ? CreateEncode() : CreateRangeArray(encodeArray, m);
			IList<object> decodeArray = dictionary.GetArray(decodeDictionaryKey);
			decode = decodeArray == null ? Range : CreateRangeArray(decodeArray, Range.Count);
			samples = PdfSampledDataConverter.Convert(bitsPerSample, SamplesCount, data);
		}
		IList<PdfRange> CreateEncode() {
			List<PdfRange> result = new List<PdfRange>(size.Count);
			foreach (int element in size)
				result.Add(new PdfRange(0, element - 1));
			return result;
		}
		protected internal override bool IsSame(PdfFunction function) {
			if (!base.IsSame(function))
				return false;
			PdfSampledFunction sampledFunction = (PdfSampledFunction)function;
			if (bitsPerSample != sampledFunction.bitsPerSample || isCubicInterpolation != sampledFunction.isCubicInterpolation || 
				!CompareRanges(encode, sampledFunction.encode) || !CompareRanges(decode, sampledFunction.decode))
					return false;
			IList<int> sampledFunctionSize = sampledFunction.size;
			int count = sampledFunctionSize.Count;
			if (size.Count != count)
				return false;
			for (int i = 0; i < count; i++)
				if (size[i] != sampledFunctionSize[i])
					return false;
			long[] sampledFunctionSamples = sampledFunction.samples;
			count = sampledFunctionSamples.Length;
			if (samples.Length != count)
				return false;
			for (int i = 0; i < count; i++)
				if (samples[i] != sampledFunctionSamples[i])
					return false;
			return true;
		}
		protected override double[] PerformTransformation(double[] arguments) {
			int sourceDimension = arguments.Length;
			int resultDimension = RangeSize;
			if (sourceDimension != Domain.Count)
				return new double[resultDimension];
			for (int i = 0; i < sourceDimension; i++) {
				PdfRange domainSample = Domain[i];
				arguments[i] = Interpolate(arguments[i], domainSample.Min, domainSample.Max, encode[i], 0, size[i] - 1);
			}
			double[] values = InterpolateSamples(arguments);
			for (int i = 0; i < resultDimension; i++) {
				PdfRange range = Range[i];
				values[i] = Interpolate(values[i], 0, MaxSampleValue, decode[i], range.Min, range.Max);
			}
			return values;
		}
		protected override PdfWriterDictionary FillDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.FillDictionary(objects);
			List<object> sizeArray = new List<object>(size.Count);
			foreach (int element in size)
				sizeArray.Add(element);
			dictionary.Add(sizeDictionaryKey, sizeArray);
			dictionary.Add(bitsPerSampleDictionaryKey, bitsPerSample);
			if (isCubicInterpolation)
				dictionary.Add(orderDictionaryKey, 3);
			if (!CompareRanges(encode, CreateEncode()))
				dictionary.Add(encodeDictionaryKey, ToObjectArray(encode));
			if (!CompareRanges(decode, Range))
				dictionary.Add(decodeDictionaryKey, ToObjectArray(decode));
			return dictionary;
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return new PdfCompressedStream(FillDictionary(objects), PdfSampledDataConverter.ConvertBack(bitsPerSample, SamplesCount, samples));
		}
		double Interpolate(double argument, double argumentMin, double argumentMax, PdfRange code, double valueMin, double valueMax) {
			argument = Normalize(argument, argumentMin, argumentMax);
			argument = Interpolate(argument, argumentMin, argumentMax, code);
			return Normalize(argument, valueMin, valueMax);
		}
		double[] InterpolateSamples(double[] coordinates) {
			int argumentDimension = coordinates.Length;
			int sampleDimension = RangeSize;
			double[] sample = new double[sampleDimension];
			for (int code = 0; code < 1 << argumentDimension; code++) {
				double factor = 1;
				int position = 0;
				int rate = 1;
				for (int i = 0; i < argumentDimension; i++) {
					int coordinate0 = (int)Math.Truncate(coordinates[i]);
					double coefficient = coordinates[i] - coordinate0;
					if ((code & (1 << i)) == 0) {
						factor *= 1 - coefficient;
						position += coordinate0 * rate;
					}
					else {
						factor *= coefficient;
						position += (coordinate0 + 1) * rate;
					}
					rate *= size[i];
				}
				if (factor > 0) {
					position *= sampleDimension;
					for (int component = 0; component < sampleDimension; component++)
						sample[component] += samples[position++] * factor;
				}
			}
			return sample;
		}
		double Normalize(double value, double min, double max) {
			return Math.Min(Math.Max(value, min), max);
		}
	}
}
