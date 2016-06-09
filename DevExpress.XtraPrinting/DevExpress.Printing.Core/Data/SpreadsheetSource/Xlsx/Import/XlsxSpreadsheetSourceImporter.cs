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

using DevExpress.Export.Xl;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.SpreadsheetSource.Implementation;
using DevExpress.Utils;
using DevExpress.XtraExport.Xlsx;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
namespace DevExpress.SpreadsheetSource.Xlsx.Import {
	#region XlsxSpreadsheetSourceImporter
	public class XlsxSpreadsheetSourceImporter : XlsxImporter {
		#region Fields
		const int defaultFirstRowIndex = -1;
		const int defaultFirstCellIndex = -1;
		readonly XlsxSpreadsheetSource source;
		XmlReader worksheetXmlReader;
		List<string> tableRalationIds;
		int lastRowIndex = defaultFirstRowIndex;
		int lastCellIndex = defaultFirstCellIndex;
		#endregion
		public XlsxSpreadsheetSourceImporter(XlsxSpreadsheetSource source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			this.tableRalationIds = new List<string>();
		}
		#region Properties
		public XlsxSpreadsheetSource Source { get { return source; } }
		protected internal List<string> TableRelationIds { get { return tableRalationIds; } }
		public override string EncryptionPassword { get { return source.Options.Password; } }
		#endregion
		#region Read global imformation
		protected internal override Destination CreateMainDocumentDestination() {
			return new DocumentDestination(this);
		}
		protected internal override void AfterImportMainDocument() {
			ImportStyles();
			ImportSharedStrings();
			ImportTables();
		}
		void ImportStyles() {
			string fileName = LookupRelationTargetByType(DocumentRelations, XlsxPackageBuilder.RelsStylesNamespace, DocumentRootFolder, "styles.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader == null)
				return;
			if (ReadToRootElement(reader, "styleSheet", XlsxPackageBuilder.SpreadsheetNamespaceConst))
				ImportContent(reader, new StylesDestination(this));
		}
		void ImportSharedStrings() {
			string fileName = LookupRelationTargetByType(DocumentRelations, XlsxPackageBuilder.RelsSharedStringsNamespace, DocumentRootFolder, "sharedStrings.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader == null)
				return;
			if (ReadToRootElement(reader, "sst", XlsxPackageBuilder.SpreadsheetNamespaceConst))
				ImportContent(reader, new SharedStringsDestination(this));
		}
		void ImportTables() {
			int count = Source.InnerWorksheets.Count;
			for (int i = 0; i < count; i++) {
				XlsxWorksheet sheet = (XlsxWorksheet)Source.InnerWorksheets[i];
				if (!ImportTable(sheet)) {
					Source.InnerWorksheets.RemoveAt(i);
					count--;
					i--;
				}
			}
		}
		bool ImportTable(XlsxWorksheet sheet) {
			ClearTableRelations();
			XmlReader reader = GetWorksheetXmlReader(sheet.RelationId);
			if (reader == null)
				return false;
			ReadToTables(reader);
			ReadTableContent(sheet);
			return true;
		}
		void ClearTableRelations() {
			this.tableRalationIds.Clear();
		}
		XmlReader GetWorksheetXmlReader(string sheetRelationId) {
			string fileName = GetWorksheetFileName(sheetRelationId);
			XmlReader reader = GetPackageFileXmlReaderBasedSeekableStream(fileName);
			if (!ReadToRootElement(reader, "worksheet", XlsxPackageBuilder.SpreadsheetNamespaceConst))
				return null; 
			return reader;
		}
		string GetWorksheetFileName(string sheetRelationId) {
			OpenXmlRelation relation = DocumentRelations.LookupRelationById(sheetRelationId);
			if (relation == null)
				ThrowInvalidFile("Can't find worksheet relation");
			return CalculateRelationTarget(relation, DocumentRootFolder, String.Empty);
		}
		void ReadToTables(XmlReader reader) {
			reader.Read();
			for (; ; ) {
				if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "tableParts") {
					ImportContent(reader, new WorksheetTablePartsDestination(this), ReadToEndTables);
				}
				reader.Skip();
				if (reader.ReadState == ReadState.EndOfFile || reader.ReadState == ReadState.Error)
					break;
			}
		}
		void ReadToEndTables(XmlReader reader) {
			ReadToEndElement(reader, "tableParts");
		}
		void ReadToEndElement(XmlReader reader, string elementName) {
			for (; ; ) {
				reader.Read();
				if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == elementName)
					break;
				ProcessCurrentDestination(reader);
			}
		}
		void ReadTableContent(XlsxWorksheet sheet) {
			if (tableRalationIds.Count == 0)
				return;
			string worksheetFileName = GetWorksheetFileName(sheet.RelationId);
			worksheetFileName = BuildPathToRelation(worksheetFileName);
			try {
				DocumentRelationsStack.Push(ImportRelations(worksheetFileName));
				foreach (string relationId in tableRalationIds)
					ReadTableContentCore(relationId, sheet.Name);
			}
			finally {
				DocumentRelationsStack.Pop();
			}
		}
		string BuildPathToRelation(string fileName) {
			return Path.GetDirectoryName(fileName) + "/_rels/" + Path.GetFileName(fileName) + ".rels";
		}
		void ReadTableContentCore(string tableRelationId, string sheetName) {
			string fileName = LookupRelationTargetById(DocumentRelations, tableRelationId, DocumentRootFolder, String.Empty);
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (!ReadToRootElement(reader, "table", XlsxPackageBuilder.SpreadsheetNamespaceConst))
				return;
			string name = ReadAttribute(reader, "displayName");
			if (string.IsNullOrEmpty(name))
				ThrowInvalidFile("Table display name is null or empty");
			string reference = ReadAttribute(reader, "ref");
			XlCellRange range = XlRangeReferenceParser.Parse(reference);
			range.SheetName = sheetName;
			bool hasHeaderRow = GetIntegerValue(reader, "headerRowCount", 1) > 0;
			bool hasTotalRow = GetIntegerValue(reader, "totalsRowCount", 0) > 0;
			Table table = new Table(name, range, hasHeaderRow, hasTotalRow);
			Source.InnerTables.Add(table);
		}
		#endregion
		#region Read to Sheet Data
		internal void PrepareBeforeReadSheet(string relationId) {
			worksheetXmlReader = GetWorksheetXmlReader(relationId);
			lastRowIndex = defaultFirstRowIndex;
		}
		internal void ReadWorksheetColumns() {
			worksheetXmlReader.Read();
			for (; ; ) {
				if (worksheetXmlReader.NodeType == XmlNodeType.Element) {
					if (worksheetXmlReader.LocalName == "cols")
						ImportContent(worksheetXmlReader, new WorksheetColumnsDestination(this), ReadToEndColumns);
					else if (worksheetXmlReader.LocalName == "sheetData")
						break;
				}
				worksheetXmlReader.Skip();
				if (worksheetXmlReader.ReadState == ReadState.EndOfFile || worksheetXmlReader.ReadState == ReadState.Error)
					break;
			}
		}
		void ReadToEndColumns(XmlReader reader) {
			ReadToEndElement(reader, "cols");
		}
		#endregion
		#region Read Row
		internal bool ReadToNextRow() {
			ResetFirstCellIndex();
			if (ReadToNextElement(worksheetXmlReader, "row"))
				return true;
			WorksheetCellDestination.ClearInstance();
			return false;
		}
		bool ReadToNextElement(XmlReader reader, string elementName) {
			if (reader.NodeType == XmlNodeType.Element && reader.LocalName == elementName)
				reader.Skip();
			else
				reader.Read();
			if (reader.NodeType != XmlNodeType.Element || reader.LocalName != elementName) 
				return false;
			return true;
		}
		internal XlsxRowAttributes ReadRowAttributes() {
			Debug.Assert(worksheetXmlReader.NodeType == XmlNodeType.Element && worksheetXmlReader.LocalName == "row");
			int index = GetWpSTIntegerValue(worksheetXmlReader, "r", Int32.MinValue);
			index = index == Int32.MinValue ? lastRowIndex + 1 : index - 1;
			lastRowIndex = index;
			bool isHidden = GetWpSTOnOffValue(worksheetXmlReader, "hidden", false);
			int styleIndex = GetWpSTIntegerValue(worksheetXmlReader, "s");
			return new XlsxRowAttributes() { Index = index, IsHidden = isHidden, StyleIndex = styleIndex };
		}
		#endregion
		#region Read Cell
		internal bool ReadToNextCell() {
			return ReadToNextElement(worksheetXmlReader, "c");
		}
		internal int ReadCellIndex() {
			string reference = ReadAttribute(worksheetXmlReader, "r");
			return GetCellIndex(reference);
		}
		internal void ReadCell() {
			WorksheetCellDestination destination = WorksheetCellDestination.GetInstance(this);
			destination.ProcessElementOpen(worksheetXmlReader);
			ImportContent(worksheetXmlReader, destination, ReadToEndCell);
		}
		void ReadToEndCell(XmlReader reader) {
			for (; ; ) {
				if (reader.LocalName == "c" && (reader.NodeType == XmlNodeType.EndElement || reader.IsEmptyElement))
					break;
				reader.Read();
				ProcessCurrentDestination(reader);
			}
		}
		internal void SkipToNextRow() {
			for (; ; ) {
				worksheetXmlReader.Skip();
				if (worksheetXmlReader.LocalName == "row" && worksheetXmlReader.NodeType == XmlNodeType.EndElement)
					break;
			}
		}
		#endregion
		void ResetFirstCellIndex() {
			lastCellIndex = defaultFirstCellIndex;
		}
		internal int GetCurrentCellIndex() {
			return lastCellIndex;
		}
		int GetCellIndex(string reference) {
			int index = String.IsNullOrEmpty(reference) ? lastCellIndex + 1 : XlCellReferenceParser.Parse(reference).Column;
			lastCellIndex = index;
			return index;
		}
		internal void CloseWorksheetReader() {
			if (worksheetXmlReader != null) {
				this.worksheetXmlReader.Close();
				this.worksheetXmlReader = null;
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				CloseWorksheetReader();
				if (tableRalationIds != null) {
					tableRalationIds.Clear();
					tableRalationIds = null;
				}
			}
			base.Dispose(disposing);
		}
	}
	#endregion
}
