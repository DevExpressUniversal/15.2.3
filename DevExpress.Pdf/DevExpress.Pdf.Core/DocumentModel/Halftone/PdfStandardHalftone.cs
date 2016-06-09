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
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public class PdfStandardHalftone : PdfHalftone {
		internal const int Number = 1;
		const string frequencyDictionaryKey = "Frequency";
		const string angleDictionaryKey = "Angle";
		const string spotFunctionDictionaryKey = "SpotFunction";
		const string accurateScreensDictionaryKey = "AccurateScreens";
		const string transferFunctionDictionaryKey = "TransferFunction";
		readonly double frequency;
		readonly double angle;
		readonly PdfFunction spotFunction;
		readonly bool accurateScreens;
		readonly PdfFunction transferFunction;
		public double Frequency { get { return frequency; } }
		public double Angle { get { return angle; } }
		public PdfFunction SpotFunction { get { return spotFunction; } }
		public bool AccurateScreens { get { return accurateScreens; } }
		public PdfFunction TransferFunction { get { return transferFunction; } }
		internal PdfStandardHalftone(PdfReaderDictionary dictionary) : base(dictionary) {
			PdfObjectCollection objects = dictionary.Objects;
			double? frequencyValue = dictionary.GetNumber(frequencyDictionaryKey);
			double? angleValue = dictionary.GetNumber(angleDictionaryKey);
			object spotFunctionValue = null;
			if (!frequencyValue.HasValue || !angleValue.HasValue || !dictionary.TryGetValue(spotFunctionDictionaryKey, out spotFunctionValue))
				PdfDocumentReader.ThrowIncorrectDataException();
			PdfName spotFunctionName = spotFunctionValue as PdfName;
			spotFunction = spotFunctionName == null ? PdfFunction.Parse(objects, spotFunctionValue, false) : new PdfPredefinedSpotFunction(spotFunctionName.Name);
			frequency = frequencyValue.Value;
			angle = angleValue.Value;
			if (frequency <= 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			accurateScreens = dictionary.GetBoolean(accurateScreensDictionaryKey) ?? false;
			object transferFunctionValue;
			if (dictionary.TryGetValue(transferFunctionDictionaryKey, out transferFunctionValue))
				transferFunction = PdfFunction.Parse(objects, transferFunctionValue, false);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.CreateDictionary(objects);
			dictionary.Add(HalftoneTypeDictionaryKey, Number);
			dictionary.Add(frequencyDictionaryKey, frequency);
			dictionary.Add(angleDictionaryKey, angle);
			dictionary.Add(spotFunctionDictionaryKey, spotFunction.Write(objects));
			dictionary.Add(accurateScreensDictionaryKey, accurateScreens, false);
			if (transferFunction != null)
				dictionary.Add(transferFunctionDictionaryKey, transferFunction.Write(objects));
			return dictionary;
		}
		protected internal override bool IsSame(PdfHalftone halftone) {
			PdfStandardHalftone standardHalftone = halftone as PdfStandardHalftone;
			if (standardHalftone == null || !base.IsSame(halftone) || frequency != standardHalftone.frequency || angle != standardHalftone.angle || 
				!spotFunction.IsSame(standardHalftone.spotFunction) || accurateScreens != standardHalftone.accurateScreens)
					return false;
			return transferFunction == null ? standardHalftone.transferFunction == null : transferFunction.IsSame(standardHalftone.transferFunction);
		}
	}
}
