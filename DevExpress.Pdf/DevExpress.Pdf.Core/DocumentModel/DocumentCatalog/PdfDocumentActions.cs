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
namespace DevExpress.Pdf {
	public class PdfDocumentActions : PdfObject {
		const string documentClosingDictionaryKey = "WC";
		const string documentSavingDictionaryKey = "WS";
		const string documentSavedDictionaryKey = "DS";
		const string documentPrintingDictionaryKey = "WP";
		const string documentPrintedDictionaryKey = "DP";
		readonly PdfJavaScriptAction documentClosing;
		readonly PdfJavaScriptAction documentSaving;
		readonly PdfJavaScriptAction documentSaved;
		readonly PdfJavaScriptAction documentPrinting;
		readonly PdfJavaScriptAction documentPrinted;
		public PdfJavaScriptAction DocumentClosing { get { return documentClosing; } }
		public PdfJavaScriptAction DocumentSaving { get { return documentSaving; } }
		public PdfJavaScriptAction DocumentSaved { get { return documentSaved; } }
		public PdfJavaScriptAction DocumentPrinting { get { return documentPrinting; } }
		public PdfJavaScriptAction DocumentPrinted { get { return documentPrinted; } }
		internal PdfDocumentActions(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			documentClosing = dictionary.GetJavaScriptAction(documentClosingDictionaryKey);
			documentSaving = dictionary.GetJavaScriptAction(documentSavingDictionaryKey);
			documentSaved = dictionary.GetJavaScriptAction(documentSavedDictionaryKey);
			documentPrinting = dictionary.GetJavaScriptAction(documentPrintingDictionaryKey);
			documentPrinted = dictionary.GetJavaScriptAction(documentPrintedDictionaryKey);
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.Add(documentClosingDictionaryKey, documentClosing);
			dictionary.Add(documentSavingDictionaryKey, documentSaving);
			dictionary.Add(documentSavedDictionaryKey, documentSaved);
			dictionary.Add(documentPrintingDictionaryKey, documentPrinting);
			dictionary.Add(documentPrintedDictionaryKey, documentPrinted);
			return dictionary;
		}
	}
}
