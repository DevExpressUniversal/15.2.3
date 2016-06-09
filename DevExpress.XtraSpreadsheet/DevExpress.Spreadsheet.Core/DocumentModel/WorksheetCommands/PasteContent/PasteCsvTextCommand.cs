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

using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.XtraSpreadsheet.Import.Csv;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.IO;
namespace DevExpress.XtraSpreadsheet.Model {
	public class PasteCsvText : PasteContentWorksheetCommand {
		public PasteCsvText(Worksheet worksheet, IErrorHandler errorHandler) :
			base(worksheet, errorHandler) {
		}
		public override DocumentFormat Format { get { return DocumentFormat.Csv; } }
		public override bool ShouldGetContentFromStream { get { return true; } }
		protected override bool IsTextOrCommaSeparatedImporterUsed { get { return true; } }
		protected internal override bool IsDataAvailable() {
			return PasteSource.ContainsData(SpreadsheetDataFormats.CommaSeparatedValue);
		}
		protected internal override CellRange ObtainSourceRange(DocumentModel source) {
			CellPosition startPosition = Worksheet.Selection.ActiveRange.TopLeft.AsAbsolute();
			Worksheet sheetFromTempModel = source.Sheets[0];
			CellRange dataRange = sheetFromTempModel.GetDataRange();
			CellPosition endPosition = dataRange.BottomRight;
			return new CellRange(sheetFromTempModel, startPosition, endPosition);
		}
		protected internal override void PopulateDocumentModelFromContentStream(DocumentModel result, Stream stream) {
			CsvDocumentImporterOptions options = new CsvDocumentImporterOptions();
			options.SourceUri = String.Empty;
			options.Culture = result.Culture;
			options.ValueTypeDetectMode = CsvValueTypeDetectMode.None;
			options.ClearWorksheetBeforeImport = false; 
			options.StartCellToInsert = Worksheet.Selection.ActiveRange.TopLeft.ToString();
			CsvImporter importer = new CsvImporter(result, options);
			importer.Import(stream);
		}
		protected internal override Stream GetContentStream() {
			Stream stream = PasteSource.GetData(SpreadsheetDataFormats.CommaSeparatedValue) as Stream;
			return stream;
		}
		protected override ModelPasteSpecialFlags GetRestrictedFlags(ModelPasteSpecialFlags flags) {
			return flags
				& ~ModelPasteSpecialFlags.FormatAndStyle
				& ~ModelPasteSpecialFlags.Comments;
		}
	}
}
