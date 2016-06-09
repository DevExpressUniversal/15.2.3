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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraPrinting;
using DevExpress.Spreadsheet.Functions;
using System.Collections.Generic;
using DevExpress.Spreadsheet.Formulas;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Spreadsheet {
	public interface IWorkbook : ISpreadsheetComponent, IPrintable, ExternalWorkbook {
		new WorksheetCollection Worksheets { get; }
		new DefinedNameCollection DefinedNames { get; }
		StyleCollection Styles { get; }
		TableStyleCollection TableStyles { get; }
		DocumentSettings DocumentSettings { get; }
		bool IsDisposed { get; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		DocumentModelAccessor Model { get; }
		string CurrentAuthor { get; }
		SpreadsheetHistory History { get; }
		PivotCacheCollection PivotCaches { get; }
		CustomFunctionCollection GlobalCustomFunctions { get; }
		CustomFunctionCollection CustomFunctions { get; }
		WorkbookFunctions Functions { get; }
		FormulaEngine FormulaEngine { get; }
		bool HasMacros { get; }
		ExternalWorkbookCollection ExternalWorkbooks { get; }
		IRangeProvider Range { get; }
		void Calculate();
		void Calculate(Range range);
		void Calculate(Worksheet sheet);
		void CalculateFull();
		void CalculateFullRebuild();
		ParameterValue Evaluate(string formula);
		ParameterValue Evaluate(string formula, FormulaEvaluationContext context);
		IEnumerable<Cell> Search(string text);
		IEnumerable<Cell> Search(string text, SearchOptions options);
		void ExportToHtml(Stream stream, Worksheet sheet);
		void ExportToHtml(string fileName, Worksheet sheet);
		void ExportToHtml(Stream stream, int sheetIndex);
		void ExportToHtml(string fileName, int sheetIndex);
		void ExportToHtml(Stream stream, Range range);
		void ExportToHtml(string fileName, Range range);
		void ExportToHtml(Stream stream, HtmlDocumentExporterOptions options);
		void ExportToHtml(string fileName, HtmlDocumentExporterOptions options);
		object MailMergeDataSource { get; set; }
		string MailMergeDataMember { get; set; }
		ParametersCollection MailMergeParameters { get; }
		IList<IWorkbook> GenerateMailMergeDocuments();
#if !SL && !DXPORTABLE
		void ExportToPdf(Stream stream);
		void ExportToPdf(string fileName);
		void ExportToPdf(Stream stream, PdfExportOptions options);
		void ExportToPdf(string fileName, PdfExportOptions options);
#endif
		void Protect(string password, bool lockStructure, bool lockWindows);
		bool Unprotect(string password);
		bool IsProtected { get; }
		DocumentProperties DocumentProperties { get; }
		object Tag { get; set; }
	}
	public interface SpreadsheetHistory {
		void Undo();
		void Redo();
		void Clear();
		int Count { get; }
		bool IsEnabled { get; set; }
	}
}
