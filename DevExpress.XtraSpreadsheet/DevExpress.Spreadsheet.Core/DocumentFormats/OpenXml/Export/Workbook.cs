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
using System.Xml;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils.Zip;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		static Dictionary<SheetVisibleState, string> visibilityTypeTable = CreatevisibilityTypeTableTable();
		static Dictionary<SheetVisibleState, string> CreatevisibilityTypeTableTable() {
			Dictionary<SheetVisibleState, string> result = new Dictionary<SheetVisibleState, string>();
			result.Add(SheetVisibleState.Hidden, "hidden");
			result.Add(SheetVisibleState.VeryHidden, "veryHidden");
			result.Add(SheetVisibleState.Visible, "visible");
			return result;
		}
		protected internal virtual CompressedStream ExportWorkbook() {
			return CreateXmlContent(GenerateWorkbookXmlContent);
		}
		protected internal virtual void GenerateWorkbookXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateWorkbookContent();
		}
		protected internal virtual void GenerateWorkbookContent() {
			WriteShStartElement("workbook");
			try {
				WriteStringAttr("xmlns", RelsPrefix, null, RelsNamespace);
				GenerateWorkbookProperties();
				GenerateWorkbookProtection();
				GenerateBookViews();
				ExportSheetReferences();
				GenerateExternalReferences();
				GenerateDefinedNames();
				GenerateCalcProperties(Workbook.Properties);
				GeneratePivotCaches();
			}
			finally {
				WriteShEndElement();
			}
		}
		#region BookView
		protected internal virtual void GenerateBookViews() {
			WriteShStartElement("bookViews");
			try {
				foreach(WorkbookWindowProperties properties in Workbook.Properties.WorkbookWindowPropertiesList)
					GenerateBookView(properties);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateBookView(WorkbookWindowProperties workbookWindowInfo) {
			WorkbookWindowInfo defaultItem = Workbook.Cache.WindowInfoCache.DefaultItem;
			WriteShStartElement("workbookView");
			try {
				if (workbookWindowInfo.Visibility != defaultItem.Visibility)
					WriteShStringValue("visibility", visibilityTypeTable[workbookWindowInfo.Visibility]);
				if (workbookWindowInfo.Minimized != defaultItem.Minimized)
					WriteShBoolValue("minimized", workbookWindowInfo.Minimized);
				if (workbookWindowInfo.ShowHorizontalScroll != defaultItem.ShowHorizontalScroll)
					WriteShBoolValue("showHorizontalScroll", workbookWindowInfo.ShowHorizontalScroll);
				if (workbookWindowInfo.ShowVerticalScroll != defaultItem.ShowVerticalScroll)
					WriteShBoolValue("showVerticalScroll", workbookWindowInfo.ShowVerticalScroll);
				if (workbookWindowInfo.ShowSheetTabs != defaultItem.ShowSheetTabs)
					WriteShBoolValue("showSheetTabs", workbookWindowInfo.ShowSheetTabs);
					WriteShIntValue("xWindow", workbookWindowInfo.HorizontalPosition);
					WriteShIntValue("yWindow", workbookWindowInfo.VerticalPosition);
					WriteShIntValue("windowWidth", workbookWindowInfo.WidhtInTwips);
					WriteShIntValue("windowHeight", workbookWindowInfo.HeightInTwips);
				if (workbookWindowInfo.TabRatio != defaultItem.TabRatio)
					WriteShIntValue("tabRatio", workbookWindowInfo.TabRatio);
				if (workbookWindowInfo.FirstDisplayedTabIndex != defaultItem.FirstDisplayedTabIndex)
					WriteShIntValue("firstSheet", workbookWindowInfo.FirstDisplayedTabIndex);
				if (workbookWindowInfo.SelectedTabIndex != WorkbookWindowProperties.DefaultSelectedTabIndex)
					WriteShIntValue("activeTab", workbookWindowInfo.SelectedTabIndex);
				if (workbookWindowInfo.AutoFilterDateGrouping != defaultItem.AutoFilterDateGrouping)
					WriteShBoolValue("autoFilterDateGrouping", workbookWindowInfo.AutoFilterDateGrouping);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		protected internal virtual void GenerateWorkbookProperties() {
			WorkbookProperties properties = Workbook.Properties;
			if (properties.CalculationOptions.DateSystem == DateSystem.Date1900 &&
				!properties.SaveBackup && string.IsNullOrEmpty(properties.CodeName) &&
				!properties.IsDefaultThemeVersion && !properties.HidePivotFieldList)
				return;
			WriteShStartElement("workbookPr");
			try {
				if (properties.CalculationOptions.DateSystem != DateSystem.Date1900)
					WriteBoolValue("date1904", true);
				if (properties.SaveBackup)
					WriteBoolValue("backupFile", true);
				if(!string.IsNullOrEmpty(properties.CodeName))
					WriteStringValue("codeName", properties.CodeName);
				if (properties.IsDefaultThemeVersion)
					WriteIntValue("defaultThemeVersion", properties.DefaultThemeVersion);
				if(properties.HidePivotFieldList)
					WriteBoolValue("hidePivotFieldList", true);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateWorkbookProtection() {
			WorkbookProtectionOptions protection = Workbook.Properties.Protection;
			if (!protection.LockRevisions && !protection.LockStructure && !protection.LockWindows)
				return;
			WriteShStartElement("workbookProtection");
			try {
				if (!protection.Credentials.IsEmpty)
					ExportProtectionPasswordHashes(protection.Credentials, true);
				if (protection.LockStructure)
					WriteBoolValue("lockStructure", protection.LockStructure);
				if (protection.LockWindows)
					WriteBoolValue("lockWindows", protection.LockWindows);
				if (protection.LockRevisions)
					WriteBoolValue("lockRevision", protection.LockRevisions);
			}
			finally {
				WriteShEndElement();
			}
		}
		void ExportSheetReferences() {
			WriteShStartElement("sheets");
			try {
				base.Export();
			}
			finally {
				WriteShEndElement();
			}
		}
		void GeneratePivotCaches() {
			Dictionary<int, PivotCache> caches = GetNonExternalSourceCaches(); 
			if (caches.Count == 0)
				return;
			WriteShStartElement("pivotCaches");
			try {
				foreach (int i in caches.Keys) 
					GeneratePivotCache(caches[i], i);
			}
			finally {
				WriteShEndElement();
			}
		}
		Dictionary<int, PivotCache> GetNonExternalSourceCaches() {
			PivotCacheCollection caches = Workbook.PivotCaches;
			Dictionary<int, PivotCache> result = new Dictionary<int, PivotCache>();
			for (int i = 0; i < caches.Count; i++) {
				PivotCache cache = caches[i];
				if (!CheckExternalWorksheetSource(cache))
					result.Add(i, cache);
			}
			return result;
		}
		void GeneratePivotCache(PivotCache cache, int i) {
			WriteShStartElement("pivotCache");
			try {
				WriteIntValue("cacheId", i);
				string id = GeneratePivotCacheRelation(cache, i);
				WriteStringAttr(RelsPrefix, "id", null, id);
			}
			finally {
				WriteShEndElement();
			}
		}
	}
}
