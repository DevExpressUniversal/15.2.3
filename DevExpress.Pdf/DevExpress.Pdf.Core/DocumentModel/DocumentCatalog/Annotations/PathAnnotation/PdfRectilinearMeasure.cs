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
namespace DevExpress.Pdf {
	public class PdfRectilinearMeasure : PdfObject {
		const string scaleRatioDictionaryKey = "R";
		const string xAxisNumberFormatDictionaryKey = "X";
		const string yAxisNumberFormatDictionaryKey = "Y";
		const string distanceNumberFormatsDictionaryKey = "D";
		const string areaNumberFormatsDictionaryKey = "A";
		const string angleNumberFormatsDictionaryKey = "T";
		const string lineSlopeNumberFormatsDictionaryKey = "S";
		const string originDictionaryKey = "O";
		const string yToXFactorDictionaryKey = "CYX";
		static PdfNumberFormat[] ParseNumberFormats(PdfReaderDictionary dictionary, string key) {
			IList<object> array = dictionary.GetArray(key);
			if (array == null)
				return null;
			int length = array.Count;
			if (length == 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			PdfObjectCollection collection = dictionary.Objects;
			PdfNumberFormat[] formats = new PdfNumberFormat[length];
			for (int i = 0; i < length; i++) {
				PdfReaderDictionary formatDictionary = collection.TryResolve(array[i]) as PdfReaderDictionary;
				if (formatDictionary == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				formats[i] = new PdfNumberFormat(formatDictionary);
			}
			return formats;
		}
		readonly string scaleRatio;
		readonly PdfNumberFormat[] xAxisNumberFormats;
		readonly PdfNumberFormat[] yAxisNumberFormats;
		readonly PdfNumberFormat[] distanceNumberFormats;
		readonly PdfNumberFormat[] areaNumberFormats;
		readonly PdfNumberFormat[] angleNumberFormats;
		readonly PdfNumberFormat[] lineSlopeNumberFormats;
		readonly PdfPoint? origin;
		readonly double? yToXFactor;
		public string ScaleRatio { get { return scaleRatio; } }
		public PdfNumberFormat[] XAxisNumberFormats { get { return xAxisNumberFormats; } }
		public PdfNumberFormat[] YAxisNumberFormats { get { return yAxisNumberFormats; } }
		public PdfNumberFormat[] DistanceNumberFormats { get { return distanceNumberFormats; } }
		public PdfNumberFormat[] AreaNumberFormats { get { return areaNumberFormats; } }
		public PdfNumberFormat[] AngleNumberFormats { get { return angleNumberFormats; } }
		public PdfNumberFormat[] LineSlopeNumberFormats { get { return lineSlopeNumberFormats; } }
		public PdfPoint? Origin { get { return origin; } }
		public double? YToXFactor { get { return yToXFactor; } }
		internal PdfRectilinearMeasure(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			string subtype = dictionary.GetName(PdfDictionary.DictionarySubtypeKey);
			scaleRatio = dictionary.GetString(scaleRatioDictionaryKey);
			xAxisNumberFormats = ParseNumberFormats(dictionary, xAxisNumberFormatDictionaryKey);
			distanceNumberFormats = ParseNumberFormats(dictionary, distanceNumberFormatsDictionaryKey);
			areaNumberFormats = ParseNumberFormats(dictionary, areaNumberFormatsDictionaryKey);
			if ((type != null && type != "Measure") || (subtype != null && subtype != "RL") || scaleRatio == null || xAxisNumberFormats == null || distanceNumberFormats == null || areaNumberFormats == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			yAxisNumberFormats = ParseNumberFormats(dictionary, yAxisNumberFormatDictionaryKey) ?? xAxisNumberFormats;
			angleNumberFormats = ParseNumberFormats(dictionary, angleNumberFormatsDictionaryKey);
			lineSlopeNumberFormats = ParseNumberFormats(dictionary, lineSlopeNumberFormatsDictionaryKey);
			IList<object> originArray = dictionary.GetArray(originDictionaryKey);
			if (originArray != null) {
				if (originArray.Count != 2)
					PdfDocumentReader.ThrowIncorrectDataException();
				origin = PdfDocumentReader.CreatePoint(originArray, 0);
			}
			yToXFactor = dictionary.GetNumber(yToXFactorDictionaryKey);
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(collection);
			dictionary.Add(scaleRatioDictionaryKey, scaleRatio);
			dictionary.AddList<PdfNumberFormat>(xAxisNumberFormatDictionaryKey, xAxisNumberFormats);
			if (!Object.ReferenceEquals(xAxisNumberFormats, yAxisNumberFormats))
				dictionary.AddList<PdfNumberFormat>(yAxisNumberFormatDictionaryKey, yAxisNumberFormats);
			dictionary.AddList<PdfNumberFormat>(distanceNumberFormatsDictionaryKey, distanceNumberFormats);
			dictionary.AddList<PdfNumberFormat>(areaNumberFormatsDictionaryKey, areaNumberFormats);
			dictionary.AddList<PdfNumberFormat>(angleNumberFormatsDictionaryKey, angleNumberFormats);
			dictionary.AddList<PdfNumberFormat>(lineSlopeNumberFormatsDictionaryKey, lineSlopeNumberFormats);
			if (origin.HasValue) {
				PdfPoint originValue = origin.Value;
				dictionary.Add(originDictionaryKey, new double[] { originValue.X, originValue.Y });
			}
			if (yToXFactor.HasValue)
				dictionary.Add(yToXFactorDictionaryKey, yToXFactor.Value);
			return dictionary;
		}
	}
}
