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
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Internal;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region RowDestination
	public class RowDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("c", OnCell);
			return result;
		}
		static RowDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (RowDestination)importer.PeekDestination();
		}
		readonly IRowCollection rows;
		Row row;
		public RowDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
			this.rows = Importer.CurrentWorksheet.Rows;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public IRowCollection Rows { get { return rows; } }
		public override void ProcessElementOpen(XmlReader reader) {
			int index = Importer.GetWpSTIntegerValue(reader, "r", Int32.MinValue);
			if (index == Int32.MinValue)
				index = Rows.Last != null ? Rows.Last.Index + 1 : 0;
			else
				index -= 1;
			row = Rows[index];
			float height = Importer.GetWpSTFloatValue(reader, "ht");
			if (height >= 0) {
				float maxHeight = Importer.DocumentModel.UnitConverter.TwipsToModelUnits(Row.MaxHeightInTwips);
				row.Height = Math.Min(Importer.DocumentModel.UnitConverter.PointsToModelUnitsF(height), maxHeight);
			}
			int outlineLevel = Importer.GetWpSTIntegerValue(reader, "outlineLevel");
			if (outlineLevel > 0 && outlineLevel <= 7) {
				row.OutlineLevel = outlineLevel;
				if (row.Sheet.Properties.FormatProperties.OutlineLevelRow < outlineLevel)
					row.Sheet.Properties.FormatProperties.OutlineLevelRow = outlineLevel;
			}
			row.IsHidden = Importer.GetWpSTOnOffValue(reader, "hidden", false);
			row.IsCollapsed = Importer.GetWpSTOnOffValue(reader, "collapsed", false);
			row.IsCustomHeight = Importer.GetWpSTOnOffValue(reader, "customHeight", false);
			bool applyStyle = Importer.GetWpSTOnOffValue(reader, "customFormat", false);
			int styleIndex = Importer.GetWpSTIntegerValue(reader, "s");
			if (applyStyle && styleIndex > 0) {
				int value;
				if (Importer.StyleSheet.CellFormatTable.TryGetValue(styleIndex, out value))
					row.AssignCellFormatIndex(value);
			}
			SheetFormatProperties formatProperties = Importer.CurrentWorksheet.Properties.FormatProperties;
			row.IsThickTopBorder = Importer.GetWpSTOnOffValue(reader, "thickTop", formatProperties.ThickTopBorder);
			row.IsThickBottomBorder = Importer.GetWpSTOnOffValue(reader, "thickBot", formatProperties.ThickBottomBorder);
		}
		static Destination OnCell(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return CellDestination.GetInstance(importer, GetThis(importer).row.Cells);
		}
	}
	#endregion
}
