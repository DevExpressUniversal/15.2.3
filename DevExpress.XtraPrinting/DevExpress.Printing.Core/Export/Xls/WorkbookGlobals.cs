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
using System.Globalization;
using System.IO;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraExport.Xlsx;
using DevExpress.Office.Utils;
using DevExpress.Export.Xl;
namespace DevExpress.XtraExport.Xls {
	using DevExpress.XtraExport.Implementation;
	using System.Reflection;
	#region XlsDataAwareExporter
	public partial class XlsDataAwareExporter {
		#region Fields
		readonly Queue<long> boundPositions = new Queue<long>();
		readonly SimpleSharedStringTable sharedStrings = new SimpleSharedStringTable();
		readonly List<XlsSSTInfo> extendedSSTItems = new List<XlsSSTInfo>();
		int sharedStringsRefCount;
		int stringsInBucket;
		readonly Dictionary<string, int> sheetDefinitions = new Dictionary<string, int>();
		readonly List<XlsContentDefinedName> definedNames = new List<XlsContentDefinedName>();
		XlDocument currentDocument = null;
		XlDocumentProperties documentProperties = null;
		XlCalculationOptions calculationOptions;
		#endregion
		protected IXlDocument BeginDocument() {
			this.sheetDefinitions.Clear();
			this.definedNames.Clear();
			this.dataValidationListRanges.Clear();
			this.drawingGroup.Clear();
			this.firstVisibleSheet = true;
			documentProperties = new XlDocumentProperties();
			documentProperties.Created = DevExpress.XtraPrinting.Native.DateTimeHelper.Now;
			currentDocument = new XlDocument(this);
			calculationOptions = new XlCalculationOptions();
			return currentDocument;
		}
		protected void EndDocument() {
			if(sheets.Count == 0) {
				BeginSheet();
				EndSheet();
			}
			RegisterDefinedNames();
			boundPositions.Clear();
			writer = workbookWriter;
			WriteWorkbookGlobals();
			WriteWorksheets();
			writer = null;
			ExportSummaryInformation();
			ExportDocumentSummaryInformation();
			currentDocument = null;
			documentProperties = null;
		}
		void InitializeSharedStrings() {
			this.sharedStringsRefCount = 0;
			this.sharedStrings.Clear();
			this.extendedSSTItems.Clear();
		}
		protected void WriteWorkbookGlobals() {
			WriteBOF(XlsSubstreamType.WorkbookGlobals);
			WriteInterface();
			WriteWriteAccess();
			WriteCodePage();
			WriteDSF();
			WriteSheetIdTable();
			WriteProtection();
			WriteWindow1();
			WriteWorkbookOptions();
			WriteWorkbookBool();
			WriteFonts();
			WriteNumberFormats();
			WriteXFs();
			WriteXFExtensions();
			WriteStyles();
			WriteUseELFs();
			WriteBundleSheet();
			WriteCountry();
			WriteSupportingLinks();
			WriteDefinedNames();
			WriteDrawingGroup();
			WriteSharedStrings();
			WriteExtSST();
			WriteTheme();
			WriteEOF();
		}
		#region WriteInterface
		protected void WriteInterface() {
			XlsContentEncoding content = new XlsContentEncoding();
			content.Value = Encoding.Unicode;
			WriteContent(XlsRecordType.InterfaceHdr, content);
			WriteContent(XlsRecordType.Mms, (short)0);
			WriteContent(XlsRecordType.InterfaceEnd, this.contentEmpty);
		}
		#endregion
		#region WriteWriteAccess
		protected void WriteWriteAccess() {
			XlsContentWriteAccess content = new XlsContentWriteAccess();
			WriteContent(XlsRecordType.WriteAccess, content);
		}
		#endregion
		#region WriteCodePage
		protected void WriteCodePage() {
			XlsContentEncoding content = new XlsContentEncoding();
			content.Value = Encoding.Unicode;
			WriteContent(XlsRecordType.CodePage, content);
		}
		#endregion
		#region WriteDSF
		protected void WriteDSF() {
			WriteContent(XlsRecordType.DSF, (short)0);
		}
		#endregion
		#region WriteSheetIdTable
		protected void WriteSheetIdTable() {
			XlsContentSheetIdTable content = new XlsContentSheetIdTable();
			for(int i = 0; i < sheets.Count; i++)
				content.SheetIdTable.Add(i + 1);
			WriteContent(XlsRecordType.RRTabId, content);
		}
		#endregion
		#region WriteProtection
		protected void WriteProtection() {
			WriteContent(XlsRecordType.WinProtect, false);
			WriteContent(XlsRecordType.Protect, false);
			WriteContent(XlsRecordType.Password, (short)0);
			WriteContent(XlsRecordType.Prot4Rev, false);
			WriteContent(XlsRecordType.Prot4RevPass, (short)0);
		}
		#endregion
		#region WriteWindow1
		protected void WriteWindow1() {
			XlsContentWorkbookWindow content = new XlsContentWorkbookWindow();
			content.HorizontalScrollDisplayed = true;
			content.VerticalScrollDisplayed = true;
			content.SheetTabsDisplayed = true;
			content.NoAutoFilterDateGrouping = true;
			WriteContent(XlsRecordType.Window1, content);
		}
		#endregion
		#region WriteWorkbookOptions
		protected void WriteWorkbookOptions() {
			WriteContent(XlsRecordType.Backup, false);
			WriteContent(XlsRecordType.HideObj, false);
			WriteContent(XlsRecordType.Date1904, false);
			WriteContent(XlsRecordType.CalcPrecision, true);
			WriteContent(XlsRecordType.RefreshAll, false);
		}
		#endregion
		#region WriteWorkbookBool
		protected void WriteWorkbookBool() {
			XlsContentWorkbookBool content = new XlsContentWorkbookBool();
			WriteContent(XlsRecordType.BookBool, content);
		}
		#endregion
		#region WriteUseELFs
		protected void WriteUseELFs() {
			XlsContentWorkbookBool content = new XlsContentWorkbookBool();
			WriteContent(XlsRecordType.UseELFs, content);
		}
		#endregion
		#region WriteBundleSheet
		protected void WriteBundleSheet() {
			XlsContentBoundSheet8 content = new XlsContentBoundSheet8();
			foreach(XlsTableBasedDocumentSheet sheet in sheets) {
				boundPositions.Enqueue(writer.BaseStream.Position + 4);
				content.Name = sheet.Name;
				content.VisibleState = sheet.VisibleState;
				WriteContent(XlsRecordType.BoundSheet8, content);
			}
		}
		#endregion
		#region WriteCountry
		protected void WriteCountry() {
			XlsContentCountry content = new XlsContentCountry();
			content.DefaultCountryIndex = XlsCountryCodes.GetCountryCode(CultureInfo.CurrentUICulture);
			content.CountryIndex = XlsCountryCodes.GetCountryCode(CultureInfo.CurrentCulture);
			WriteContent(XlsRecordType.Country, content);
		}
		#endregion
		#region WriteSupportingLinks
		protected void WriteSupportingLinks() {
			if(sheetDefinitions.Count == 0)
				return;
			XlsContentSupBookSelf content = new XlsContentSupBookSelf();
			content.SheetCount = sheets.Count;
			WriteContent(XlsRecordType.SupBook, content);
			WriteExternSheet();
		}
		void WriteExternSheet() {
			XlsChunk firstChunk = new XlsChunk(XlsRecordType.ExternSheet);
			XlsChunk nextChunk = new XlsChunk(XlsRecordType.Continue);
			using(XlsChunkWriter chunkWriter = new XlsChunkWriter(writer, firstChunk, nextChunk)) {
				int count = sheetDefinitions.Count;
				chunkWriter.Write((ushort)count);
				XlsXTI xti = new XlsXTI();
				xti.SupBookIndex = 0;
				foreach(string sheetName in sheetDefinitions.Keys) {
					int sheetIndex = IndexOfSheet(sheetName);
					xti.FirstSheetIndex = sheetIndex;
					xti.LastSheetIndex = sheetIndex;
					xti.Write(chunkWriter);
				}
			}
		}
		int IndexOfSheet(string sheetName) {
			for(int i = 0; i < sheets.Count; i++) {
				if(sheets[i].Name == sheetName)
					return i;
			}
			return -1;
		}
		#endregion
		#region WriteDefinedNames
		protected void WriteDefinedNames() {
			foreach(XlsContentDefinedName content in definedNames)
				WriteContent(XlsRecordType.Lbl, content);
		}
		#endregion
		#region WriteSharedStrings
		protected void WriteSharedStrings() {
			int count = this.sharedStrings.Count;
			CalcStringsInBucket(count);
			XlsChunk firstChunk = new XlsChunk(XlsRecordType.SST);
			XlsChunk nextChunk = new XlsChunk(XlsRecordType.Continue);
			using(XlsChunkWriter chunkWriter = new XlsChunkWriter(writer, firstChunk, nextChunk)) {
				chunkWriter.Write(this.sharedStringsRefCount); 
				chunkWriter.Write(count); 
				XLUnicodeRichExtendedString item = new XLUnicodeRichExtendedString();
				IList<IXlString> stringList = this.sharedStrings.StringList;
				for(int i = 0; i < count; i++) {
					IXlString stringListItem = stringList[i];
					item.Value = stringListItem.Text;
					item.FormatRuns.Clear();
					if(!stringListItem.IsPlainText)
						SetupFormatRuns(item, stringListItem as XlRichTextString);
					if(FirstStringInBucket(i)) {
						XlsSSTInfo sstInfo = new XlsSSTInfo();
						sstInfo.Offset = (int)(chunkWriter.BaseStream.Position + 4);
						sstInfo.StreamPosition = (int)(writer.BaseStream.Position + sstInfo.Offset);
						this.extendedSSTItems.Add(sstInfo);
					}
					item.Write(chunkWriter);
				}
			}
		}
		void SetupFormatRuns(XLUnicodeRichExtendedString item, XlRichTextString text) {
			int charIndex = 0;
			foreach(XlRichTextRun textRun in text.Runs) {
				XlsFormatRun formatRun = new XlsFormatRun();
				formatRun.CharIndex = charIndex;
				formatRun.FontIndex = textRun.FontIndex;
				if(!formatRun.IsDefault())
					item.FormatRuns.Add(formatRun);
				charIndex += textRun.Text.Length;
			}
		}
		void CalcStringsInBucket(int count) {
			this.stringsInBucket = (count / 128) + 1;
			if(this.stringsInBucket < XlsDefs.MinStringsInBucket)
				this.stringsInBucket = XlsDefs.MinStringsInBucket;
		}
		bool FirstStringInBucket(int index) {
			return (index % this.stringsInBucket) == 0;
		}
		#endregion
		#region WriteExtSST
		protected void WriteExtSST() {
			XlsContentExtSST content = new XlsContentExtSST();
			content.StringsInBucket = this.stringsInBucket;
			content.Items.AddRange(this.extendedSSTItems);
			WriteContent(XlsRecordType.ExtSST, content);
		}
		#endregion
		#region WriteTheme
		void WriteTheme() {
			if(currentDocument.Theme == XlDocumentTheme.None || ClipboardMode)
				return;
			XlsContentTheme content = new XlsContentTheme();
			Assembly asm = this.GetType().GetAssembly();
#if DXPORTABLE
			string resourceName = currentDocument.Theme == XlDocumentTheme.Office2010 ? "DevExpress.Printing.Core.Export.Xls.Theme.theme2010.zip" : "DevExpress.Printing.Core.Export.Xls.Theme.theme2013.zip";
#else
			string resourceName = currentDocument.Theme == XlDocumentTheme.Office2010 ? "DevExpress.Printing.Export.Xls.Theme.theme2010.zip" : "DevExpress.Printing.Export.Xls.Theme.theme2013.zip";
#endif
			Stream resourceStream = asm.GetManifestResourceStream(resourceName);
			int count = (int)resourceStream.Length;
			byte[] themeContent = new byte[count];
			resourceStream.Read(themeContent, 0, count);
			content.ThemeContent = themeContent;
			WriteContent(XlsRecordType.Theme, content);
		}
#endregion
		internal int RegisterSheetDefinition(string sheetName) {
			int index = -1;
			if (sheetDefinitions.TryGetValue(sheetName, out index))
				return index;
			index = sheetDefinitions.Count;
			sheetDefinitions.Add(sheetName, index);
			return index;
		}
		internal int GetSheetDefinitionIndex(string sheetName) {
			int index = -1;
			sheetDefinitions.TryGetValue(sheetName, out index);
			return index;
		}
		void RegisterDefinedNames() {
			for(int i = 0; i < sheets.Count; i++) {
				IXlSheet sheet = sheets[i];
				RegisterAutoFilter(sheet, i);
				RegisterPrintArea(sheet, i);
				RegisterPrintTitles(sheet, i);
			}
		}
		void RegisterAutoFilter(IXlSheet sheet, int sheetIndex) {
			if(sheet.AutoFilterRange == null)
				return;
			XlCellRange range = sheet.AutoFilterRange.AsAbsolute();
			XlsContentDefinedName content = new XlsContentDefinedName();
			content.Name = "_xlnm._FilterDatabase";
			content.SheetIndex = sheetIndex + 1;
			content.IsHidden = true;
			XlExpression formula = new XlExpression();
			XlPtgArea3d ptg = new XlPtgArea3d(range, sheet.Name);
			formula.Add(ptg);
			content.FormulaBytes = formula.ToXlsExpression().GetBytes(this);
			definedNames.Add(content);
		}
		void RegisterPrintArea(IXlSheet sheet, int sheetIndex) {
			if(sheet.PrintArea == null)
				return;
			XlCellRange range = sheet.PrintArea.AsAbsolute();
			XlsContentDefinedName content = new XlsContentDefinedName();
			content.Name = "_xlnm.Print_Area";
			content.SheetIndex = sheetIndex + 1;
			XlExpression formula = new XlExpression();
			XlPtgArea3d ptg = new XlPtgArea3d(range, sheet.Name);
			formula.Add(ptg);
			content.FormulaBytes = formula.ToXlsExpression().GetBytes(this);
			definedNames.Add(content);
		}
		void RegisterPrintTitles(IXlSheet sheet, int sheetIndex) {
			if(!sheet.PrintTitles.IsValid())
				return;
			XlsContentDefinedName content = new XlsContentDefinedName();
			content.Name = "_xlnm.Print_Titles";
			content.SheetIndex = sheetIndex + 1;
			XlExpression formula = new XlExpression();
			AddRangeReference(formula, sheet, sheet.PrintTitles.Columns);
			AddRangeReference(formula, sheet, sheet.PrintTitles.Rows);
			if(formula.Count > 1)
				formula.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Union));
			content.FormulaBytes = formula.GetBytes(this);
			definedNames.Add(content);
		}
		void AddRangeReference(XlExpression formula, IXlSheet sheet, XlCellRange range) {
			if(range == null)
				return;
			XlPtgArea3d ptg = new XlPtgArea3d(range, sheet.Name);
			formula.Add(ptg);
		}
	}
#endregion
}
