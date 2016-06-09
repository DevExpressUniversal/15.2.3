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

using DevExpress.Office.Utils;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.XtraSpreadsheet.Import.Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
#if SL
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	public class PasteTabDelimitedText : PasteContentWorksheetCommand {
		public PasteTabDelimitedText(Worksheet worksheet, IErrorHandler errorHandler) :
			base(worksheet, errorHandler) {
		}
		public override DocumentFormat Format { get { return DocumentFormat.Text; } }
		public override bool ShouldGetContentFromStream { get { return false; } }
		protected override bool IsTextOrCommaSeparatedImporterUsed { get { return true; } }
		protected internal override bool IsDataAvailable() {
			return PasteSource.ContainsData(OfficeDataFormats.UnicodeText) ||
				PasteSource.ContainsData(OfficeDataFormats.Text) ||
				PasteSource.ContainsData(OfficeDataFormats.OemText);
		}
		protected internal override CellRange ObtainSourceRange(DocumentModel source) {
			CellPosition startPosition = Worksheet.Selection.ActiveRange.TopLeft;
			Worksheet sheetFromTempModel = source.Sheets[0];
			CellRange dataRange = sheetFromTempModel.GetDataRange();
			CellPosition endPosition = dataRange.BottomRight;
			return new CellRange(sheetFromTempModel, startPosition, endPosition);
		}
		protected internal override void PopulateDocumentModelFromContentStream(DocumentModel result, Stream content) {
			TxtDocumentImporterOptions options = new TxtDocumentImporterOptions();
			options.SourceUri = String.Empty;
			options.Culture = result.Culture;
			options.Delimiter = '\t';
			options.ValueTypeDetectMode = CsvValueTypeDetectMode.None;
			options.ClearWorksheetBeforeImport = false; 
			options.StartCellToInsert = Worksheet.Selection.ActiveRange.TopLeft.ToString();
			CsvImporter importer = new CsvImporter(result, options);
			importer.Import(content);
		}
		protected internal override Stream GetContentStream() {
			string result = PasteSource.GetData(OfficeDataFormats.UnicodeText) as string;
			if(result != null)
				return new MemoryStream(Encoding.UTF8.GetBytes(result));
			result = PasteSource.GetData(OfficeDataFormats.Text) as string;
			return new MemoryStream(Encoding.UTF8.GetBytes(result));
		}
		protected override ModelPasteSpecialFlags GetRestrictedFlags(ModelPasteSpecialFlags flags) {
			return flags
				& ~ModelPasteSpecialFlags.FormatAndStyle
				& ~ModelPasteSpecialFlags.Comments;
		}
	}
}
