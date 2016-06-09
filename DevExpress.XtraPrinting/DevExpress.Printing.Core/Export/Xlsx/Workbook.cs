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
using DevExpress.Office;
using DevExpress.XtraExport.Implementation;
using DevExpress.Export.Xl;
namespace DevExpress.XtraExport.Xlsx {
	using System.IO;
	using System.Reflection;
	partial class XlsxDataAwareExporter {
		#region TranslationTables
		public static Dictionary<XlSheetVisibleState, string> VisibilityTypeTable = CreateVisibilityTypeTable();
		static Dictionary<XlSheetVisibleState, string> CreateVisibilityTypeTable() {
			Dictionary<XlSheetVisibleState, string> result = new Dictionary<XlSheetVisibleState, string>();
			result.Add(XlSheetVisibleState.Hidden, "hidden");
			result.Add(XlSheetVisibleState.VeryHidden, "veryHidden");
			result.Add(XlSheetVisibleState.Visible, "visible");
			return result;
		}
		#endregion
		XlDocument currentDocument = null;
		XlDocumentProperties documentProperties = null;
		XlCalculationOptions calculationOptions;
		protected IXlDocument BeginDocument() {
			documentProperties = new XlDocumentProperties();
			documentProperties.Created = DevExpress.XtraPrinting.Native.DateTimeHelper.Now;
			currentDocument = new XlDocument(this);
			calculationOptions = new XlCalculationOptions();
			return currentDocument;
		}
		protected void EndDocument() {
			if (sheets.Count == 0) {
				BeginSheet();
				EndSheet();
			}
			AddDocumentApplicationPropertiesContent();
			AddDocumentCorePropertiesContent();
			AddDocumentCustomPropertiesContent();
			AddThemeContent();
			AddStylesContent();
			AddSharedStringContent();
			AddWorkbookContent();
			AddWorkbookRelations();
			AddContentTypes();
			AddPackageRelations();
			currentDocument = null;
			documentProperties = null;
		}
		void AddThemeContent() {
			if(currentDocument.Theme == XlDocumentTheme.None)
				return;
#if DXPORTABLE
			Assembly asm = this.GetType().GetTypeInfo().Assembly;
			string resourceName = currentDocument.Theme == XlDocumentTheme.Office2010 ? "Export/Xlsx/Theme/theme2010.xml" : "Export/Xlsx/Theme/theme2013.xml";
#else
			Assembly asm = this.GetType().Assembly;
			string resourceName = currentDocument.Theme == XlDocumentTheme.Office2010 ? "DevExpress.Printing.Export.Xlsx.Theme.theme2010.xml" : "DevExpress.Printing.Export.Xlsx.Theme.theme2013.xml";
#endif
			Stream resourceStream = asm.GetManifestResourceStream(resourceName);
			AddPackageContent(@"xl\theme\theme1.xml", resourceStream);
			OpenXmlRelationCollection relations = Builder.WorkbookRelations;
			relations.Add(new OpenXmlRelation(relations.GenerateId(), "theme/theme1.xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme"));
			Builder.OverriddenContentTypes.Add("/xl/theme/theme1.xml", "application/vnd.openxmlformats-officedocument.theme+xml");
		}
		void AddWorkbookContent() {
			BeginWriteXmlContent();
			WriteShStartElement("workbook");
			try {
				WriteStringAttr("xmlns", XlsxPackageBuilder.RelsPrefix, null, XlsxPackageBuilder.RelsNamespace);
				GenerateSheetReferences();
				GenerateDefinedNames();
				GenerateCalculationProperties();
			}
			finally {
				WriteShEndElement();
			}
			AddPackageContent(@"xl/workbook.xml", EndWriteXmlContent());
			Builder.OverriddenContentTypes.Add("/xl/workbook.xml", XlsxPackageBuilder.WorkbookContentType);
		}
		void GenerateSheetReferences() {
			WriteShStartElement("sheets");
			try {
				sheets.ForEach(GenerateSheetReference);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateSheetReference(IXlSheet sheet) {
			WriteShStartElement("sheet");
			try {
				SheetInfo info = sheetInfos[sheet];
				WriteShStringValue("name", sheet.Name);
				WriteShIntValue("sheetId", info.SheetId);
				if (sheet.VisibleState != XlSheetVisibleState.Visible)
					WriteShStringValue("state", VisibilityTypeTable[sheet.VisibleState]);
				WriteStringAttr(XlsxPackageBuilder.RelsPrefix, "id", null, info.RelationId);
			}
			finally {
				WriteShEndElement();
			}
		}
		void AddContentTypes() {
			BeginWriteXmlContent();
			if (ShouldExportDocumentApplicationProperties())
				Builder.OverriddenContentTypes.Add(@"/docProps/app.xml", documentApplicationPropertiesContentType);
			if (ShouldExportDocumentCoreProperties())
				Builder.OverriddenContentTypes.Add(@"/docProps/core.xml", documentCorePropertiesContentType);
			if (ShouldExportDocumentCustomProperties())
				Builder.OverriddenContentTypes.Add(@"/docProps/custom.xml", documentCustomPropertiesContentType);
			Builder.GenerateContentTypesContent(writer);
			AddPackageContent(@"[Content_Types].xml", EndWriteXmlContent());
		}
		void AddWorkbookRelations() {
			BeginWriteXmlContent();
			Builder.GenerateRelationsContent(writer, Builder.WorkbookRelations);
			AddPackageContent(@"xl\_rels\workbook.xml.rels", EndWriteXmlContent());
		}
		void AddPackageRelations() {
			BeginWriteXmlContent();
			OpenXmlRelationCollection relations = new OpenXmlRelationCollection();
			relations.Add(new OpenXmlRelation(relations.GenerateId(), "xl/workbook.xml", XlsxPackageBuilder.OfficeDocumentNamespace));
			if (ShouldExportDocumentApplicationProperties())
				relations.Add(new OpenXmlRelation(relations.GenerateId(), "docProps/app.xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties"));
			if (ShouldExportDocumentCoreProperties())
				relations.Add(new OpenXmlRelation(relations.GenerateId(), "docProps/core.xml", "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties"));
			if (ShouldExportDocumentCustomProperties())
				relations.Add(new OpenXmlRelation(relations.GenerateId(), "docProps/custom.xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/custom-properties"));
			Builder.GenerateRelationsContent(writer, relations);
			AddPackageContent(@"_rels\.rels", EndWriteXmlContent());
		}
		void GenerateDefinedNames() {
			if(!ShouldWriteDefinedNames())
				return;
			WriteShStartElement("definedNames");
			try {
				GenerateSheetDefinedNames();
			}
			finally {
				WriteShEndElement();
			}
		}
		bool ShouldWriteDefinedNames() {
			foreach(IXlSheet sheet in sheets) {
				if(sheet.AutoFilterRange != null || sheet.PrintTitles.IsValid() || sheet.PrintArea != null)
					return true;
			}
			return false;
		}
		void GenerateSheetDefinedNames() {
			for(int i = 0; i < sheets.Count; i++) {
				GeneratePrintArea(i, sheets[i]);
				GeneratePrintTitles(i, sheets[i]);
				GenerateAutoFilterDatabase(i, sheets[i]);
			}
		}
		void GeneratePrintTitles(int sheetIndex, IXlSheet sheet) {
			if(!sheet.PrintTitles.IsValid())
				return;
			WriteShStartElement("definedName");
			try {
				SheetInfo info = sheetInfos[sheet];
				WriteShStringValue("name", "_xlnm.Print_Titles");
				WriteShIntValue("localSheetId", sheetIndex);
				WriteShString(sheet.PrintTitles.ToString());
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateAutoFilterDatabase(int sheetIndex, IXlSheet sheet) {
			if(sheet.AutoFilterRange == null)
				return;
			XlCellRange range = sheet.AutoFilterRange.AsAbsolute();
			range.SheetName = sheet.Name;
			WriteShStartElement("definedName");
			try {
				SheetInfo info = sheetInfos[sheet];
				WriteShStringValue("name", "_xlnm._FilterDatabase");
				WriteBoolValue("hidden", true);
				WriteShIntValue("localSheetId", sheetIndex);
				WriteShString(range.ToString(true));
			}
			finally {
				WriteShEndElement();
			}
		}
		void GeneratePrintArea(int sheetIndex, IXlSheet sheet) {
			if(sheet.PrintArea == null)
				return;
			XlCellRange range = sheet.PrintArea.AsAbsolute();
			range.SheetName = sheet.Name;
			WriteShStartElement("definedName");
			try {
				SheetInfo info = sheetInfos[sheet];
				WriteShStringValue("name", "_xlnm.Print_Area");
				WriteShIntValue("localSheetId", sheetIndex);
				WriteShString(range.ToString(true));
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateCalculationProperties() {
			if (!ShouldWriteCalculationProperties())
				return;
			WriteShStartElement("calcPr");
			try {
				WriteShBoolValue("fullCalcOnLoad", true);
			}
			finally {
				WriteShEndElement();
			}
		}
		bool ShouldWriteCalculationProperties() {
			return CalculationOptions.FullCalculationOnLoad;
		}
	}
}
