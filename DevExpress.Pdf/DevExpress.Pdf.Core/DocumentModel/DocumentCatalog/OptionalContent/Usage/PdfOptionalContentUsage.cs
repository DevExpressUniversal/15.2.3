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
using System.Globalization;
using DevExpress.Utils;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	[PdfDefaultField(PdfPageElement.None, false)]
	public enum PdfPageElement { 
		None, 
		[PdfFieldName("HF")]
		HeaderFooter, 
		[PdfFieldName("FG")]
		Foreground, 
		[PdfFieldName("BG")]
		Background, 
		[PdfFieldName("L")]
		Logo 
	}
	public class PdfOptionalContentUsage : PdfObject {
		const string creatorInfoDictionaryKey = "CreatorInfo";
		const string languageDictionaryKey = "Language";
		const string languagePrefferedDictionaryKey = "Preferred";
		const string exportDictionaryKey = "Export";
		const string exportStateDictionaryKey = "ExportState";
		const string zoomDictionaryKey = "Zoom";
		const string minZoomDictionaryKey = "min";
		const string maxZoomDictionaryKey = "max";
		const string printDictionaryKey = "Print";
		const string printStateDictionaryKey = "PrintState";
		const string viewDictionaryKey = "View";
		const string viewStateDictionaryKey = "ViewState";
		const string pageElementDictionaryKey = "PageElement";
		const string onValue = "ON";
		const string offValue = "OFF";
		const double defaultMinZoom = 0.0;
		const double defaultMaxZoom = Double.PositiveInfinity;
		static DefaultBoolean ParseOnOff(PdfReaderDictionary dictionary, string key) {
			string name = dictionary.GetName(key);
			if (String.IsNullOrEmpty(name))
				return DefaultBoolean.Default;
			switch (name) {
				case onValue:
					return DefaultBoolean.True;
				case offValue:
					return DefaultBoolean.False;
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					return DefaultBoolean.Default;
			}
		}
		static void WriteOnOffState(PdfDictionary dictionary, string key, DefaultBoolean state) {
			if (state != DefaultBoolean.Default)
				dictionary.Add(key, new PdfName(state == DefaultBoolean.True ? onValue : offValue));
		}
		readonly PdfOptionalContentUsageCreatorInfo creatorInfo;
		readonly CultureInfo languageCulture = CultureInfo.InvariantCulture;
		readonly bool isLanguagePreferred;
		readonly DefaultBoolean exportState;
		readonly double minZoom;
		readonly double maxZoom;
		readonly string printContentKind;
		readonly DefaultBoolean printState;
		readonly DefaultBoolean viewState;
		readonly PdfPageElement pageElement;
		public PdfOptionalContentUsageCreatorInfo CreatorInfo { get { return creatorInfo; } }
		public CultureInfo LanguageCulture { get { return languageCulture; } }
		public bool IsLanguagePreferred { get { return isLanguagePreferred; } }
		public DefaultBoolean ExportState { get { return exportState; } }
		public double MinZoom { get { return minZoom; } }
		public double MaxZoom { get { return maxZoom; } }
		public string PrintContentKind { get { return printContentKind; } }
		public DefaultBoolean PrintState { get { return printState; } }
		public DefaultBoolean ViewState { get { return viewState; } }
		public PdfPageElement PageElement { get { return pageElement; } }
		internal PdfOptionalContentUsage(PdfReaderDictionary dictionary) {
			PdfReaderDictionary creatorInfoDictionary = dictionary.GetDictionary(creatorInfoDictionaryKey);
			if (creatorInfoDictionary != null)
				creatorInfo = new PdfOptionalContentUsageCreatorInfo(creatorInfoDictionary);
			PdfReaderDictionary languageDictionary = dictionary.GetDictionary(languageDictionaryKey);
			if (languageDictionary != null) {
				languageCulture = languageDictionary.GetLanguageCulture();
				isLanguagePreferred = ParseOnOff(languageDictionary, languagePrefferedDictionaryKey) == DefaultBoolean.True;
			}
			PdfReaderDictionary exportDictionary = dictionary.GetDictionary(exportDictionaryKey);
			if (exportDictionary == null)
				exportState = DefaultBoolean.Default;
			else {
				exportState = ParseOnOff(exportDictionary, exportStateDictionaryKey);
				if (exportState == DefaultBoolean.Default)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			PdfReaderDictionary zoomDictionary = dictionary.GetDictionary(zoomDictionaryKey);
			if (zoomDictionary == null) {
				minZoom = defaultMinZoom;
				maxZoom = defaultMaxZoom;
			}
			else {
				minZoom = dictionary.GetNumber(minZoomDictionaryKey) ?? defaultMinZoom;
				maxZoom = dictionary.GetNumber(maxZoomDictionaryKey) ?? defaultMaxZoom;
			}
			PdfReaderDictionary printDictionary = dictionary.GetDictionary(printDictionaryKey);
			if (printDictionary != null) {
				printContentKind = printDictionary.GetName(PdfDictionary.DictionarySubtypeKey);
				if (printContentKind != null && printContentKind.Length == 0)
					PdfDocumentReader.ThrowIncorrectDataException();
				printState = ParseOnOff(printDictionary, printStateDictionaryKey);
			}
			PdfReaderDictionary viewDictionary = dictionary.GetDictionary(viewDictionaryKey);
			if (viewDictionary != null) {
				viewState = ParseOnOff(viewDictionary, viewStateDictionaryKey);
				if (viewState == DefaultBoolean.Default)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			PdfReaderDictionary pageElementDictionary = dictionary.GetDictionary(pageElementDictionaryKey);
			if (pageElementDictionary != null)
				pageElement = PdfEnumToStringConverter.Parse<PdfPageElement>(pageElementDictionary.GetName(PdfDictionary.DictionarySubtypeKey));
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.Add(creatorInfoDictionaryKey, creatorInfo);
			if (languageCulture != CultureInfo.InvariantCulture || isLanguagePreferred) {
				PdfDictionary languageDictionary = new PdfDictionary();
				languageDictionary.Add(PdfDictionary.DictionaryLanguageKey, PdfDocumentStream.ConvertToArray(languageCulture.Name));
				if (isLanguagePreferred)
					languageDictionary.Add(languagePrefferedDictionaryKey, new PdfName(onValue));
				dictionary.Add(languageDictionaryKey, languageDictionary);
			}
			if (exportState != DefaultBoolean.Default) {
				PdfDictionary exportDictionary = new PdfDictionary();
				WriteOnOffState(exportDictionary, exportStateDictionaryKey, exportState);
				dictionary.Add(exportDictionaryKey, exportDictionary);
			}
			bool isNonDefaultMinZoom = minZoom != 0.0;
			bool isNonDefaultMaxZoom = !Double.IsPositiveInfinity(maxZoom);
			if (isNonDefaultMaxZoom || isNonDefaultMaxZoom) {
				PdfDictionary zoomDictionary = new PdfDictionary();
				if (isNonDefaultMinZoom)
					zoomDictionary.Add(minZoomDictionaryKey, minZoom);
				if (isNonDefaultMaxZoom)
					zoomDictionary.Add(maxZoomDictionaryKey, maxZoom);
				dictionary.Add(zoomDictionaryKey, zoomDictionary);
			}
			bool isNonDefaultPrintDictionaryKind = !String.IsNullOrEmpty(printContentKind);
			if (isNonDefaultPrintDictionaryKind || printState != DefaultBoolean.Default) {
				PdfDictionary printDictionary = new PdfDictionary();
				if (isNonDefaultPrintDictionaryKind)
					printDictionary.Add(PdfDictionary.DictionarySubtypeKey, new PdfName(printContentKind));
				WriteOnOffState(printDictionary, printStateDictionaryKey, printState);
				dictionary.Add(printDictionaryKey, printDictionary);
			}
			if (viewState != DefaultBoolean.Default) {
				PdfDictionary viewDictionary = new PdfDictionary();
				WriteOnOffState(viewDictionary, viewStateDictionaryKey, viewState);
				dictionary.Add(viewDictionaryKey, viewDictionary);
			}
			if (pageElement != PdfPageElement.None) {
				PdfDictionary pageElementDictionary = new PdfDictionary();
				pageElementDictionary.Add(PdfDictionary.DictionarySubtypeKey, new PdfName(PdfEnumToStringConverter.Convert(pageElement)));
				dictionary.Add(pageElementDictionaryKey, pageElementDictionary);
			}
			return dictionary;
		}
	}
}
