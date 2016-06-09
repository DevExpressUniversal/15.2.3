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
	[PdfDefaultField(PdfRichMediaBinding.None)]
	public enum PdfRichMediaBinding { None, Foreground, Background, Material }
	public class PdfRichMediaParams : PdfObject {
		const string richMediaParamsDictionaryName = "RichMediaParams";
		const string flashVarsKey = "FlashVars";
		const string bindingMaterialNameKey = "BindingMaterialName";
		const string bindingKey = "Binding";
		const string cuePointsKey = "CuePoints";
		const string settingsKey = "Settings";
		readonly string flashVars;
		readonly string bindingMaterialName;
		readonly PdfRichMediaBinding binding;
		readonly List<PdfCuePoint> cuePoints = new List<PdfCuePoint>();
		readonly string settings;
		public string FlashVars { get { return flashVars; } }
		public string BindingMaterialName { get { return bindingMaterialName; } }
		public PdfRichMediaBinding Binding { get { return binding; } }
		public IList<PdfCuePoint> CuePoints { get { return cuePoints; } }
		public string Settings { get { return settings; } }
		internal PdfRichMediaParams(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			if (type != null && type != richMediaParamsDictionaryName)
				PdfDocumentReader.ThrowIncorrectDataException();
			flashVars = dictionary.GetStringAdvanced(flashVarsKey);
			bindingMaterialName = dictionary.GetString(bindingMaterialNameKey);
			binding = PdfEnumToStringConverter.Parse<PdfRichMediaBinding>(dictionary.GetName(bindingKey));
			if (binding == PdfRichMediaBinding.Material && String.IsNullOrEmpty(bindingMaterialName))
				PdfDocumentReader.ThrowIncorrectDataException();
			IList<object> cuePointsArray = dictionary.GetArray(cuePointsKey);
			if (cuePointsArray != null) {
				PdfObjectCollection collection = dictionary.Objects;
				foreach (object value in cuePointsArray) {
					PdfReaderDictionary cuePointDictionary = value as PdfReaderDictionary;
					if (cuePointDictionary == null) {
						PdfObjectReference reference = value as PdfObjectReference;
						if (reference == null)
							PdfDocumentReader.ThrowIncorrectDataException();
						cuePointDictionary = collection.GetDictionary(reference.Number);
						if (cuePointDictionary == null)
							PdfDocumentReader.ThrowIncorrectDataException();
					}
					cuePoints.Add(new PdfCuePoint(cuePointDictionary));
				}
			}
			settings = dictionary.GetStringAdvanced(settingsKey);
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			PdfWriterDictionary result = new PdfWriterDictionary(collection);
			result.Add(PdfDictionary.DictionaryTypeKey, new PdfName(richMediaParamsDictionaryName));
			if (!String.IsNullOrEmpty(flashVars))
				result.Add(flashVarsKey, collection.AddStream(flashVars));
			result.AddNotNullOrEmptyString(bindingMaterialNameKey, bindingMaterialName);
			result.AddEnumName(bindingKey, binding);
			if (cuePoints.Count > 0)
				result.Add(cuePointsKey, new PdfWritableConvertableArray<PdfCuePoint>(cuePoints, value => value.Write(collection)));
			result.AddStream(settingsKey, settings);
			return result;
		}
	}
}
