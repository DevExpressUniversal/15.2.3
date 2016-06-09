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
using DevExpress.XtraSpreadsheet.Model;
using System.Xml;
using DevExpress.Office;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region SheetColumnsDestination
	public class SheetColumnsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("col", OnColumn);
			return result;
		}
		public SheetColumnsDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnColumn(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ColumnDestination(importer);
		}
	}
	#endregion
	#region ColumnDestination
	public class ColumnDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		public ColumnDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int firstIndex = Importer.GetWpSTIntegerValue(reader, "min", 0);
			if (firstIndex <= 0)
				Importer.ThrowInvalidFile("Invalid first column index");
			int lastIndex = Importer.GetWpSTIntegerValue(reader, "max", 0);
			if (lastIndex <= 0)
				Importer.ThrowInvalidFile("Invalid last column index");
			if (lastIndex < firstIndex)
				Importer.ThrowInvalidFile("Last column index less than first");
			Column columnRange = Importer.CurrentWorksheet.Columns.CreateNewColumnRange(firstIndex - 1, lastIndex - 1);
			columnRange.IsCustomWidth = Importer.GetWpSTOnOffValue(reader, "customWidth", false);
			double width = Importer.GetWpDoubleValue(reader, "width", double.MinValue);
			if (width != double.MinValue) {
				if (width < 0)
					width = 255;
				else
					width = Importer.DocumentModel.GetService<IColumnWidthCalculationService>().RemoveGaps(Importer.CurrentWorksheet, (float)width);
				columnRange.Width = (float)Math.Min(255, width);
			}
			columnRange.BestFit = Importer.GetWpSTOnOffValue(reader, "bestFit", false);
			columnRange.IsHidden = Importer.GetWpSTOnOffValue(reader, "hidden", false);
			columnRange.IsCollapsed = Importer.GetWpSTOnOffValue(reader, "collapsed", false);
			columnRange.OutlineLevel = Math.Max(0, Math.Min(7, Importer.GetWpSTIntegerValue(reader, "outlineLevel", 0)));
			int outlineLevel = columnRange.OutlineLevel;
			if (columnRange.Sheet.Properties.FormatProperties.OutlineLevelCol < outlineLevel && outlineLevel <= 7)
				columnRange.Sheet.Properties.FormatProperties.OutlineLevelCol = outlineLevel;
			int index = Importer.GetWpSTIntegerValue(reader, "style", Int32.MinValue);
			int formatIndex;
			if (!Importer.StyleSheet.CellFormatTable.TryGetValue(index, out formatIndex))
				formatIndex = Importer.DocumentModel.StyleSheet.DefaultCellFormatIndex;
			columnRange.AssignCellFormatIndex(formatIndex);
		}
	}
	#endregion
}
