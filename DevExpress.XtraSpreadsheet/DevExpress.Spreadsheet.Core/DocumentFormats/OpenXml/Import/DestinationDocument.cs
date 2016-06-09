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
using DevExpress.XtraSpreadsheet.Internal;
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DocumentDestination
	public class DocumentDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("bookViews", OnBookViews);
			result.Add("sheets", OnSheets);
			result.Add("workbookPr", OnWorkbookProperties);
			result.Add("definedNames", OnDefinedNames);
			result.Add("externalReferences", OnExternalReferences);
			result.Add("calcPr", OnCalcProperties);
			result.Add("workbookProtection", OnWorkbookProtection);
			result.Add("pivotCaches", OnPivotCaches);
			return result;
		}
		#endregion
		public DocumentDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnBookViews(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new BookViewsDestination(importer);
		}
		static Destination OnSheets(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SheetsDestination(importer);
		}
		static Destination OnWorkbookProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new WorkbookPropertiesDestination(importer);
		}
		static Destination OnDefinedNames(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DefinedNamesDestination(importer);
		}
		static Destination OnExternalReferences(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ExternalReferencesDestination(importer);
		}
		static Destination OnCalcProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new CalcPropertiesDestination(importer);
		}
		static Destination OnWorkbookProtection(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new WorkbookProtectionDestination(importer);
		}
		static Destination OnPivotCaches(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCachesDestination(importer);
		}
	}
	#endregion
	#region WorkbookPropertiesDestination
	public class WorkbookPropertiesDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		public WorkbookPropertiesDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			WorkbookProperties properties = Importer.DocumentModel.Properties;
			properties.BeginUpdate();
			try {
				properties.CalculationOptions.DateSystem = Importer.GetOnOffValue(reader, "date1904", false) ? DateSystem.Date1904 : DateSystem.Date1900;
				properties.SaveBackup = Importer.GetOnOffValue(reader, "backupFile", false);
				properties.CodeName = Importer.ReadAttribute(reader, "codeName");
				properties.DefaultThemeVersion = Importer.GetIntegerValue(reader, "defaultThemeVersion", 0);
				properties.HidePivotFieldList = Importer.GetOnOffValue(reader, "hidePivotFieldList", false);
			}
			finally {
				properties.EndUpdate();
			}
		}
	}
	#endregion
}
