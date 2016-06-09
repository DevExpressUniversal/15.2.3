#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.IO;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Utils;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	#region WorksheetPasteBiff8ContentCommand
	public class PasteBiff8Content : PasteContentWorksheetCommand {
		const string LinkSourceDescriptor = "Link Source Descriptor";
		const string LinkSource = "Link Source";
		string filePath;
		CellRangeInfo rangeFromDocument ;
		public PasteBiff8Content(Worksheet worksheet, IErrorHandler errorHandler) :
			base(worksheet,errorHandler) {
			rangeFromDocument = null;
		}
		public override DocumentFormat Format { get { return DocumentFormat.Xls; } }
		public override bool ShouldGetContentFromStream { get { return true; } }
		public override string SourceFilePath { get { return filePath; } }
		protected internal override bool IsDataAvailable() {
			return PasteSource.ContainsData(SpreadsheetDataFormats.Biff8);
		}
		protected internal override void PopulateDocumentModelFromContentStream(DocumentModel result, Stream stream) {
			XlsDocumentImporterOptions options = new XlsDocumentImporterOptions();
			options.SourceUri = String.Empty;
			using (XlsImporter importer = new XlsImporter(result, options)) {
				importer.Import(stream);
				rangeFromDocument = importer.ContentBuilder.OleObjectRange;
			}
		}
		protected internal override Stream GetContentStream() {
			return PasteSource.GetData(SpreadsheetDataFormats.Biff8) as Stream;
		}
		protected internal override CellRange ObtainSourceRange(DocumentModel source) {
			string text = GetTextFromMemoryStream(SpreadsheetDataFormats.Link, DXEncoding.Default);
			ClipboardLinkParser linkParser = new ClipboardLinkParser(source);
			CellRange range = linkParser.GetCellRange(text);
			if (range != null) {
				this.filePath = linkParser.FilePath;
				return range;
			}
			CellRange result = null;
			byte[] linkSourceBytes = GetBytesFromMemoryStream(LinkSource);
			var parsedLinkSource = ClipboardLinkSourceParser.Parse(linkSourceBytes);
			if (!String.IsNullOrEmpty(parsedLinkSource.RangeReference)) {
				this.filePath = parsedLinkSource.FilePath;
				result = linkParser.GetCellRangefromFullRCReference(parsedLinkSource.RangeReference);
				if (result != null)
					return result;
			}
			byte[] bytes = GetBytesFromMemoryStream(LinkSourceDescriptor);
			string rcReference = CfObjectDescriptorParser.ParseSrcOfCopy(bytes);
			result = linkParser.GetCellRangefromFullRCReference(rcReference);
			if (result != null)
				return result;
			CellPosition topLeft = CellPosition.InvalidValue;
			CellPosition bottomRight = CellPosition.InvalidValue;
			if (rangeFromDocument != null) {
				topLeft = rangeFromDocument.First.AsAbsolute();
				bottomRight = rangeFromDocument.Last.AsAbsolute();
			}
			else {
				CellRange dataRange = source.ActiveSheet.GetDataRange(); 
				if (dataRange == null)
					return result;
				topLeft = dataRange.TopLeft.AsAbsolute();
				bottomRight = dataRange.BottomRight.AsAbsolute();
			}
			string sheetName = source.ActiveSheet.Name;
			string reference = CellReferenceParser.ToString(topLeft, true, 0, 0) + ':' + CellReferenceParser.ToString(bottomRight, true, 0, 0);
			result = linkParser.GetCellRangeFromR1C1Reference(sheetName, reference);
			return result;
		}
	}
	#endregion
}
