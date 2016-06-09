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

using DevExpress.Office;
using DevExpress.Web.ASPxRichEdit.Export;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class ChangeTableCellCommand : WebRichEditPropertyStateBasedCommand<TableCellState, JSONTableCellProperty> {
		static Dictionary<JSONTableCellProperty, JSONPieceTableModifier<TableCellState>> modifiers = new Dictionary<JSONTableCellProperty, JSONPieceTableModifier<TableCellState>>() {
			{ JSONTableCellProperty.PreferredWidth, (PieceTable pieceTable) => new TableCellPropertiesPreferredWidthModifier(pieceTable) },
			{ JSONTableCellProperty.VerticalMerging, (PieceTable pieceTable) => new TableCellPropertiesVerticalMergingModifier(pieceTable) },
			{ JSONTableCellProperty.ColumnSpan, (PieceTable pieceTable) => new TableCellPropertiesColumnSpanModifier(pieceTable) },
		};
		public ChangeTableCellCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeTableCell; } }
		protected override bool IsEnabled() {
			return Client.DocumentCapabilitiesOptions.TablesAllowed;
		}
		protected override IModelModifier<TableCellState> CreateModifier(JSONTableCellProperty property) {
			JSONPieceTableModifier<TableCellState> creator;
			if (!modifiers.TryGetValue(property, out creator))
				throw new ArgumentException();
			return creator(PieceTable);
		}
	}
	public class ChangeTableCellPropertyCommand : WebRichEditPropertyStateBasedCommand<TableCellPropertyState, JSONTableCellProperty> {
		static Dictionary<JSONTableCellProperty, JSONPieceTableModifier<TableCellPropertyState>> modifiers = new Dictionary<JSONTableCellProperty, JSONPieceTableModifier<TableCellPropertyState>>() {
			{ JSONTableCellProperty.BackgroundColor, (PieceTable pieceTable) => new TableCellPropertiesBackgroundColorModifier(pieceTable) },
			{ JSONTableCellProperty.Borders, (PieceTable pieceTable) => new TableCellPropertiesBordersModifier(pieceTable) },
			{ JSONTableCellProperty.CellMargins, (PieceTable pieceTable) => new TableCellPropertiesCellMarginsModifier(pieceTable) },
			{ JSONTableCellProperty.FitText, (PieceTable pieceTable) => new TableCellPropertiesFitTextModifier(pieceTable) },
			{ JSONTableCellProperty.ForegroundColor, (PieceTable pieceTable) => new TableCellPropertiesForegroundColorModifier(pieceTable) },
			{ JSONTableCellProperty.HideCellMark, (PieceTable pieceTable) => new TableCellPropertiesHideCellMarkModifier(pieceTable) },
			{ JSONTableCellProperty.NoWrap, (PieceTable pieceTable) => new TableCellPropertiesNoWrapModifier(pieceTable) },
			{ JSONTableCellProperty.Shading, (PieceTable pieceTable) => new TableCellPropertiesShadingModifier(pieceTable) },
			{ JSONTableCellProperty.TextDirection, (PieceTable pieceTable) => new TableCellPropertiesTextDirectionModifier(pieceTable) },
			{ JSONTableCellProperty.VerticalAlignment, (PieceTable pieceTable) => new TableCellPropertiesVerticalAlignmentModifier(pieceTable) },
		};
		public ChangeTableCellPropertyCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeTableCellProperty; } }
		protected override bool IsEnabled() {
			return Client.DocumentCapabilitiesOptions.TablesAllowed;
		}
		protected override IModelModifier<TableCellPropertyState> CreateModifier(JSONTableCellProperty property) {
			JSONPieceTableModifier<TableCellPropertyState> creator;
			if (!modifiers.TryGetValue(property, out creator))
				throw new ArgumentException();
			return creator(PieceTable);
		}
	}
	abstract class TableCellPropertyModelModifierBase<T> : TableCellPropertyModelModifier<T> {
		public TableCellPropertyModelModifierBase(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyCore(DocumentLogPosition tablePosition, int tableNestedLevel, int rowIndex, int cellIndex, T value) {
			var tableStartParagraphIndex = PieceTable.FindParagraphIndex(tablePosition);
			var table = PieceTable.TableCellsManager.GetCellByNestingLevel(tableStartParagraphIndex, tableNestedLevel).Table;
			var row = table.Rows[rowIndex];
			var cell = row.Cells[cellIndex];
			ModifyTableCore(cell.Properties, value);
		}
		protected abstract void ModifyTableCore(TableCellProperties properties, T value);
	}
	abstract class TableCellPropertyWithUseModelModifierBase<T> : TableCellPropertyWithUseModelModifier<T> {
		public TableCellPropertyWithUseModelModifierBase(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyCore(DocumentLogPosition tablePosition, int tableNestedLevel, int rowIndex, int cellIndex, T value, bool useValue) {
			var tableStartParagraphIndex = PieceTable.FindParagraphIndex(tablePosition);
			var table = PieceTable.TableCellsManager.GetCellByNestingLevel(tableStartParagraphIndex, tableNestedLevel).Table;
			var row = table.Rows[rowIndex];
			var cell = row.Cells[cellIndex];
			var info = cell.Properties.GetInfoForModification();
			ModifyTableCore(cell.Properties, info, value, useValue);
			cell.Properties.ReplaceInfo(info, TableCellChangeActionsCalculator.CalculateChangeActions(TableCellChangeType));
		}
		protected abstract void ModifyTableCore(TableCellProperties properties, TableCellPropertiesOptions options, T value, bool useValue);
		protected abstract TableCellChangeType TableCellChangeType { get; }
	}
	abstract class TableCellComplexPropertyWithUseModelModifierBase<T> : TableCellComplexPropertyWithUseModelModifier<T> {
		public TableCellComplexPropertyWithUseModelModifierBase(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyCore(DocumentLogPosition tablePosition, int tableNestedLevel, int rowIndex, int cellIndex, T[] values, bool[] useValues) {
			var tableStartParagraphIndex = PieceTable.FindParagraphIndex(tablePosition);
			var table = PieceTable.TableCellsManager.GetCellByNestingLevel(tableStartParagraphIndex, tableNestedLevel).Table;
			var row = table.Rows[rowIndex];
			var cell = row.Cells[cellIndex];
			var info = cell.Properties.GetInfoForModification();
			ModifyTableCore(cell.Properties, info, values, useValues);
			cell.Properties.ReplaceInfo(info, TableCellChangeActionsCalculator.CalculateChangeActions(TableCellChangeType));
		}
		protected abstract void ModifyTableCore(TableCellProperties properties, TableCellPropertiesOptions options, T[] values, bool[] useValues);
		protected abstract TableCellChangeType TableCellChangeType { get; }
	}
	class TableCellPropertiesPreferredWidthModifier : TableCellPropertyModelModifierBase<ArrayList> {
		public TableCellPropertiesPreferredWidthModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyTableCore(TableCellProperties properties, ArrayList value) {
			properties.PreferredWidth.Type = (WidthUnitType)value[0];
			properties.PreferredWidth.Value = (int)value[1];
		}
	}
	class TableCellPropertiesVerticalMergingModifier : TableCellPropertyModelModifierBase<MergingState> {
		public TableCellPropertiesVerticalMergingModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyTableCore(TableCellProperties properties, MergingState value) {
			properties.VerticalMerging = value;
		}
	}
	class TableCellPropertiesColumnSpanModifier : TableCellPropertyModelModifierBase<int> {
		public TableCellPropertiesColumnSpanModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyTableCore(TableCellProperties properties, int value) {
			properties.ColumnSpan = value;
		}
	}
	class TableCellPropertiesBackgroundColorModifier : TableCellPropertyWithUseModelModifierBase<int> {
		public TableCellPropertiesBackgroundColorModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableCellChangeType TableCellChangeType { get { return TableCellChangeType.BackgroundColor; } }
		protected override void ModifyTableCore(TableCellProperties properties, TableCellPropertiesOptions options, int value, bool useValue) {
			properties.BackgroundColor = System.Drawing.Color.FromArgb(value);
			options.UseBackgroundColor = useValue;
		}
	}
	class TableCellPropertiesFitTextModifier : TableCellPropertyWithUseModelModifierBase<bool> {
		public TableCellPropertiesFitTextModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableCellChangeType TableCellChangeType { get { return TableCellChangeType.FitText; } }
		protected override void ModifyTableCore(TableCellProperties properties, TableCellPropertiesOptions options, bool value, bool useValue) {
			properties.FitText = value;
			options.UseFitText = useValue;
		}
	}
	class TableCellPropertiesForegroundColorModifier : TableCellPropertyWithUseModelModifierBase<int> {
		public TableCellPropertiesForegroundColorModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableCellChangeType TableCellChangeType { get { return TableCellChangeType.ForegroundColor; } }
		protected override void ModifyTableCore(TableCellProperties properties, TableCellPropertiesOptions options, int value, bool useValue) {
			properties.ForegroundColor = System.Drawing.Color.FromArgb(value);
			options.UseForegroundColor = useValue;
		}
	}
	class TableCellPropertiesHideCellMarkModifier : TableCellPropertyWithUseModelModifierBase<bool> {
		public TableCellPropertiesHideCellMarkModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableCellChangeType TableCellChangeType { get { return TableCellChangeType.HideCellMark; } }
		protected override void ModifyTableCore(TableCellProperties properties, TableCellPropertiesOptions options, bool value, bool useValue) {
			properties.HideCellMark = value;
			options.UseHideCellMark = useValue;
		}
	}
	class TableCellPropertiesNoWrapModifier : TableCellPropertyWithUseModelModifierBase<bool> {
		public TableCellPropertiesNoWrapModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableCellChangeType TableCellChangeType { get { return TableCellChangeType.NoWrap; } }
		protected override void ModifyTableCore(TableCellProperties properties, TableCellPropertiesOptions options, bool value, bool useValue) {
			properties.NoWrap = value;
			options.UseNoWrap = useValue;
		}
	}
	class TableCellPropertiesShadingModifier : TableCellPropertyWithUseModelModifierBase<ShadingPattern> {
		public TableCellPropertiesShadingModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableCellChangeType TableCellChangeType { get { return TableCellChangeType.ShadingPattern; } }
		protected override void ModifyTableCore(TableCellProperties properties, TableCellPropertiesOptions options, ShadingPattern value, bool useValue) {
			properties.ShadingPattern = value;
			options.UseShading = useValue;
		}
	}
	class TableCellPropertiesTextDirectionModifier : TableCellPropertyWithUseModelModifierBase<TextDirection> {
		public TableCellPropertiesTextDirectionModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableCellChangeType TableCellChangeType { get { return TableCellChangeType.TextDirection; } }
		protected override void ModifyTableCore(TableCellProperties properties, TableCellPropertiesOptions options, TextDirection value, bool useValue) {
			properties.TextDirection = value;
			options.UseTextDirection = useValue;
		}
	}
	class TableCellPropertiesVerticalAlignmentModifier : TableCellPropertyWithUseModelModifierBase<VerticalAlignment> {
		public TableCellPropertiesVerticalAlignmentModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableCellChangeType TableCellChangeType { get { return TableCellChangeType.TextDirection; } }
		protected override void ModifyTableCore(TableCellProperties properties, TableCellPropertiesOptions options, VerticalAlignment value, bool useValue) {
			properties.VerticalAlignment = value;
			options.UseVerticalAlignment = useValue;
		}
	}
	class TableCellPropertiesBordersModifier : TableCellComplexPropertyWithUseModelModifierBase<ArrayList> {
		public TableCellPropertiesBordersModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableCellChangeType TableCellChangeType { get { return TableCellChangeType.None; } }
		protected override void ModifyTableCore(TableCellProperties properties, TableCellPropertiesOptions options, ArrayList[] values, bool[] useValues) {
			XtraRichEdit.Model.BorderBase[] borders = new XtraRichEdit.Model.BorderBase[] { properties.Borders.TopBorder, properties.Borders.RightBorder, properties.Borders.BottomBorder, properties.Borders.LeftBorder, properties.Borders.InsideHorizontalBorder, properties.Borders.InsideVerticalBorder };
			var skippedIndices = new List<int>();
			for(var i = 0; i < 6; i++) {
				if (values[i] != null) {
					borders[i].Color = System.Drawing.Color.FromArgb((int)values[i][0]);
					borders[i].Frame = (bool)values[i][1];
					borders[i].Offset = (int)values[i][2];
					borders[i].Shadow = (bool)values[i][3];
					borders[i].Style = (BorderLineStyle)values[i][4];
					borders[i].Width = (int)values[i][5];
				}
				else
					skippedIndices.Add(i);
			}
			if(!skippedIndices.Contains(0))
				options.UseTopBorder = useValues[0];
			if (!skippedIndices.Contains(1))
				options.UseRightBorder = useValues[1];
			if (!skippedIndices.Contains(2))
				options.UseBottomBorder = useValues[2];
			if (!skippedIndices.Contains(3))
				options.UseLeftBorder = useValues[3];
			if (!skippedIndices.Contains(4))
				options.UseInsideHorizontalBorder = useValues[4];
			if (!skippedIndices.Contains(5))
				options.UseInsideVerticalBorder = useValues[5];
		}
	}
	class TableCellPropertiesCellMarginsModifier : TableCellComplexPropertyWithUseModelModifierBase<ArrayList> {
		public TableCellPropertiesCellMarginsModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableCellChangeType TableCellChangeType { get { return TableCellChangeType.None; } }
		protected override void ModifyTableCore(TableCellProperties properties, TableCellPropertiesOptions options, ArrayList[] values, bool[] useValues) {
			XtraRichEdit.Model.MarginUnitBase[] margins = new XtraRichEdit.Model.MarginUnitBase[] { properties.CellMargins.Top, properties.CellMargins.Right, properties.CellMargins.Bottom, properties.CellMargins.Left };
			for (var i = 0; i < 4; i++) {
				margins[i].Type = (WidthUnitType)values[i][0];
				margins[i].Value = (int)values[i][1];
			}
			options.UseTopMargin = useValues[0];
			options.UseRightMargin = useValues[1];
			options.UseBottomMargin = useValues[2];
			options.UseLeftMargin = useValues[3];
		}
	}
}
