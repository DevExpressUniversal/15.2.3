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

using System.Xml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office;
using System;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region SheetProtectionDestination
	public class SheetProtectionDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		#endregion
		public SheetProtectionDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			Worksheet worksheet = Importer.CurrentWorksheet;
			WorksheetProtectionInfo defaultItem = Importer.DocumentModel.Cache.WorksheetProtectionInfoCache.SchemaDefaultItem;
			WorksheetProtectionOptions protection = worksheet.Properties.Protection;
			try {
				protection.BeginUpdate();
				protection.SheetLocked = Importer.GetWpSTOnOffValue(reader, "sheet", defaultItem.SheetLocked);
				protection.AutoFiltersLocked = Importer.GetWpSTOnOffValue(reader, "autoFilter", defaultItem.AutoFiltersLocked);
				protection.DeleteColumnsLocked = Importer.GetWpSTOnOffValue(reader, "deleteColumns", defaultItem.DeleteColumnsLocked);
				protection.DeleteRowsLocked = Importer.GetWpSTOnOffValue(reader, "deleteRows", defaultItem.DeleteRowsLocked);
				protection.FormatCellsLocked = Importer.GetWpSTOnOffValue(reader, "formatCells", defaultItem.FormatCellsLocked);
				protection.FormatColumnsLocked = Importer.GetWpSTOnOffValue(reader, "formatColumns", defaultItem.FormatColumnsLocked);
				protection.FormatRowsLocked = Importer.GetWpSTOnOffValue(reader, "formatRows", defaultItem.FormatRowsLocked);
				protection.InsertColumnsLocked = Importer.GetWpSTOnOffValue(reader, "insertColumns", defaultItem.InsertColumnsLocked);
				protection.InsertHyperlinksLocked = Importer.GetWpSTOnOffValue(reader, "insertHyperlinks", defaultItem.InsertHyperlinksLocked);
				protection.InsertRowsLocked = Importer.GetWpSTOnOffValue(reader, "insertRows", defaultItem.InsertRowsLocked);
				protection.ObjectsLocked = Importer.GetWpSTOnOffValue(reader, "objects", defaultItem.ObjectsLocked);
				protection.PivotTablesLocked = Importer.GetWpSTOnOffValue(reader, "pivotTables", defaultItem.PivotTablesLocked);
				protection.ScenariosLocked = Importer.GetWpSTOnOffValue(reader, "scenarios", defaultItem.ScenariosLocked);
				protection.SelectLockedCellsLocked = Importer.GetWpSTOnOffValue(reader, "selectLockedCells", defaultItem.SelectLockedCellsLocked);
				protection.SelectUnlockedCellsLocked = Importer.GetWpSTOnOffValue(reader, "selectUnlockedCells", defaultItem.SelectUnlockedCellsLocked);
				protection.SortLocked = Importer.GetWpSTOnOffValue(reader, "sort", defaultItem.SortLocked);
				ProtectionCredentials credentials = Importer.ReadProtection(reader);
				protection.Credentials = credentials;
			}
			finally {
				protection.EndUpdate();
			}
		}
	}
	#endregion
}
