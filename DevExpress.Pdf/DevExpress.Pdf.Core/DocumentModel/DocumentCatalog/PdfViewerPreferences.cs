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

using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	[PdfDefaultField(PdfNonFullScreenPageMode.UseNone)]
	public enum PdfNonFullScreenPageMode {
		[PdfFieldName("UseNone", "None")]
		UseNone,
		[PdfFieldName("UseOutlines", "Outlines")]
		UseOutlines,
		[PdfFieldName("UseThumbs", "Thumbs")]
		UseThumbs,
		[PdfFieldName("UseOC", "OC")]
		UseOC
	}
	[PdfDefaultField(PdfDirection.LeftToRight)]
	public enum PdfDirection {
		[PdfFieldName("L2R")]
		LeftToRight,
		[PdfFieldName("R2L")]
		RightToLeft
	}
	[PdfDefaultField(PdfViewArea.CropBox)]
	public enum PdfViewArea { MediaBox, CropBox, BleedBox, TrimBox, ArtBox }
	[PdfDefaultField(PdfPrintScaling.AppDefault)]
	public enum PdfPrintScaling { None, AppDefault }
	[PdfDefaultField(PdfPrintMode.None)]
	public enum PdfPrintMode { None, Simplex, DuplexFlipShortEdge, DuplexFlipLongEdge }
	public class PdfViewerPreferences {
		const string hideToolbarDictionaryKey = "HideToolbar";
		const string hideMenubarDictionaryKey = "HideMenubar";
		const string hideWindowUIDictionaryKey = "HideWindowUI";
		const string fitWindowDictionaryKey = "FitWindow";
		const string centerWindowDictionaryKey = "CenterWindow";
		const string displayDocTitleDictionaryKey = "DisplayDocTitle";
		const string nonFullScreenPageModeDictionaryKey = "NonFullScreenPageMode";
		const string directionDictionaryKey = "Direction";
		const string viewAreaDictionaryKey = "ViewArea";
		const string viewClipDictionaryKey = "ViewClip";
		const string printAreaDictionaryKey = "PrintArea";
		const string printClipDictionaryKey = "PrintClip";
		const string printScalingDictionaryKey = "PrintScaling";
		const string printModeDictionaryKey = "Duplex";
		const string pickTrayByPDFSizeDictionaryKey = "PickTrayByPDFSize";
		const string printPageRangeDictionaryKey = "PrintPageRange";
		const string printNumCopiesDictionaryKey = "NumCopies";
		readonly bool hideToolbar;
		readonly bool hideMenubar;
		readonly bool hideWindowUI;
		readonly bool fitWindow;
		readonly bool centerWindow;
		readonly bool displayDocTitle;
		readonly PdfNonFullScreenPageMode nonFullScreenPageMode;
		readonly PdfDirection direction;
		readonly PdfViewArea viewArea;
		readonly PdfViewArea viewClip;
		readonly PdfViewArea printArea;
		readonly PdfViewArea printClip;
		readonly PdfPrintScaling printScaling;
		readonly PdfPrintMode printMode;
		readonly bool pickTrayByPDFSize;
		readonly List<PdfPrintPageRange> printPageRange = new List<PdfPrintPageRange>();
		readonly int printNumCopies;
		public bool HideToolbar { get { return hideToolbar; } }
		public bool HideMenubar { get { return hideMenubar; } }
		public bool HideWindowUI { get { return hideWindowUI; } }
		public bool FitWindow { get { return fitWindow; } }
		public bool CenterWindow { get { return centerWindow; } }
		public bool DisplayDocTitle { get { return displayDocTitle; } }
		public PdfNonFullScreenPageMode NonFullScreenPageMode { get { return nonFullScreenPageMode; } }
		public PdfDirection Direction { get { return direction; } }
		public PdfViewArea ViewArea { get { return viewArea; } }
		public PdfViewArea ViewClip { get { return viewClip; } }
		public PdfViewArea PrintArea { get { return printArea; } }
		public PdfViewArea PrintClip { get { return printClip; } }
		public PdfPrintScaling PrintScaling { get { return printScaling; } }
		public PdfPrintMode PrintMode { get { return printMode; } }
		public bool PickTrayByPDFSize { get { return pickTrayByPDFSize; } }
		public IList<PdfPrintPageRange> PrintPageRange { get { return printPageRange; } }
		public int PrintNumCopies { get { return printNumCopies; } }
		internal PdfViewerPreferences(PdfReaderDictionary dictionary) {
			hideToolbar = dictionary.GetBoolean(hideToolbarDictionaryKey) ?? false;
			hideMenubar = dictionary.GetBoolean(hideMenubarDictionaryKey) ?? false;
			hideWindowUI = dictionary.GetBoolean(hideWindowUIDictionaryKey) ?? false;
			fitWindow = dictionary.GetBoolean(fitWindowDictionaryKey) ?? false;
			centerWindow = dictionary.GetBoolean(centerWindowDictionaryKey) ?? false;
			displayDocTitle = dictionary.GetBoolean(displayDocTitleDictionaryKey) ?? false;
			nonFullScreenPageMode = dictionary.GetEnumName<PdfNonFullScreenPageMode>(nonFullScreenPageModeDictionaryKey);
			direction = dictionary.GetEnumName<PdfDirection>(directionDictionaryKey);
			viewArea = dictionary.GetEnumName<PdfViewArea>(viewAreaDictionaryKey);
			viewClip = dictionary.GetEnumName<PdfViewArea>(viewClipDictionaryKey);
			printArea = dictionary.GetEnumName<PdfViewArea>(printAreaDictionaryKey);
			printClip = dictionary.GetEnumName<PdfViewArea>(printClipDictionaryKey);
			object value;
			if (dictionary.TryGetValue(printScalingDictionaryKey, out value)) {
				value = dictionary.Objects.TryResolve(value);
				if (value == null)
					printScaling = PdfPrintScaling.AppDefault;
				else {
					PdfName name = value as PdfName;
					if (name == null) {
						byte[] bytes = value as byte[];
						if (bytes == null) {
							if (!(value is bool))
								PdfDocumentReader.ThrowIncorrectDataException();
							printScaling = (bool)value ? PdfPrintScaling.AppDefault : PdfPrintScaling.None;
						}
						else
							printScaling = PdfEnumToStringConverter.Parse<PdfPrintScaling>(PdfDocumentReader.ConvertToString(bytes));
					}
					else
						printScaling = PdfEnumToStringConverter.Parse<PdfPrintScaling>(name.Name);
				}
			}
			else
				printScaling = PdfPrintScaling.AppDefault;
			printMode = dictionary.GetEnumName<PdfPrintMode>(printModeDictionaryKey);
			pickTrayByPDFSize = dictionary.GetBoolean(pickTrayByPDFSizeDictionaryKey) ?? true;
			IList<object> array = dictionary.GetArray(printPageRangeDictionaryKey);
			if (array != null) {
				int length = array.Count;
				if (length % 2 > 0)
					PdfDocumentReader.ThrowIncorrectDataException();
				length /= 2;
				for (int i = 0, index = 0; i < length; i++) {
					object startValue = array[index++];
					object endValue = array[index++];
					if (!(startValue is int) || !(endValue is int))
						PdfDocumentReader.ThrowIncorrectDataException();
					int start = (int)startValue;
					int end = (int)endValue;
					if (start < 1 || end < 1 || start > end)
						PdfDocumentReader.ThrowIncorrectDataException();
					printPageRange.Add(new PdfPrintPageRange(start, end));
				}
			}
			printNumCopies = dictionary.GetInteger(printNumCopiesDictionaryKey) ?? 1;
		}
		internal PdfDictionary Write() {
			PdfWriterDictionary result = new PdfWriterDictionary(null);
			result.Add(hideToolbarDictionaryKey, hideToolbar, false);
			result.Add(hideMenubarDictionaryKey, hideMenubar, false);
			result.Add(hideWindowUIDictionaryKey, hideWindowUI, false);
			result.Add(fitWindowDictionaryKey, fitWindow, false);
			result.Add(centerWindowDictionaryKey, centerWindow, false);
			result.Add(displayDocTitleDictionaryKey, displayDocTitle, false);
			result.AddEnumName(nonFullScreenPageModeDictionaryKey, nonFullScreenPageMode);
			result.AddEnumName(directionDictionaryKey, direction);
			result.AddEnumName(viewAreaDictionaryKey, viewArea);
			result.AddEnumName(viewClipDictionaryKey, viewClip);
			result.AddEnumName(printAreaDictionaryKey, printArea);
			result.AddEnumName(printClipDictionaryKey, printClip);
			result.AddEnumName(printScalingDictionaryKey, printScaling);
			result.AddEnumName(printModeDictionaryKey, printMode);
			result.Add(pickTrayByPDFSizeDictionaryKey, pickTrayByPDFSize, true);
			if (printPageRange.Count > 0) {
				List<object> lst = new List<object>();
				foreach (PdfPrintPageRange range in printPageRange) {
					lst.Add(range.Start);
					lst.Add(range.End);
				}
				result.Add(printPageRangeDictionaryKey, lst);
			}
			result.Add(printNumCopiesDictionaryKey, printNumCopies, 1);
			return result;
		}
	}
}
