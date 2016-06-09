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

using DevExpress.Web.ASPxRichEdit.Export;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class ChangeTableCommand : WebRichEditPropertyStateBasedCommand<TableState, JSONTableProperty> {
		static Dictionary<JSONTableProperty, JSONPieceTableModifier<TableState>> modifiers = new Dictionary<JSONTableProperty, JSONPieceTableModifier<TableState>>() {
			{ JSONTableProperty.PreferredWidth, (PieceTable pieceTable) => new TablePropertiesPreferredWidthModifier(pieceTable) },
			{ JSONTableProperty.TableLookTypes, (PieceTable pieceTable) => new TablePropertiesTableLookTypesModifier(pieceTable) },
		};
		public ChangeTableCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeTable; } }
		protected override bool IsEnabled() {
			return Client.DocumentCapabilitiesOptions.TablesAllowed;
		}
		protected override IModelModifier<TableState> CreateModifier(JSONTableProperty property) {
			JSONPieceTableModifier<TableState> creator;
			if (!modifiers.TryGetValue(property, out creator))
				throw new ArgumentException();
			return creator(PieceTable);
		}
	}
	public class ChangeTablePropertyCommand : WebRichEditPropertyStateBasedCommand<TablePropertyState, JSONTableProperty> {
		static Dictionary<JSONTableProperty, JSONPieceTableModifier<TablePropertyState>> modifiers = new Dictionary<JSONTableProperty, JSONPieceTableModifier<TablePropertyState>>() {
			{ JSONTableProperty.AvoidDoubleBorders, (PieceTable pieceTable) => new TablePropertiesAvoidDoubleBordersModifier(pieceTable) },
			{ JSONTableProperty.BackgroundColor, (PieceTable pieceTable) => new TablePropertiesBackgroundColorModifier(pieceTable) },
			{ JSONTableProperty.Borders, (PieceTable pieceTable) => new TablePropertiesBordersModifier(pieceTable) },
			{ JSONTableProperty.CellMargins, (PieceTable pieceTable) => new TablePropertiesCellMarginsModifier(pieceTable) },
			{ JSONTableProperty.CellSpacing, (PieceTable pieceTable) => new TablePropertiesCellSpacingModifier(pieceTable) },
			{ JSONTableProperty.Indent, (PieceTable pieceTable) => new TablePropertiesIndentModifier(pieceTable) },
			{ JSONTableProperty.IsTableOverlap, (PieceTable pieceTable) => new TablePropertiesIsTableOverlapModifier(pieceTable) },
			{ JSONTableProperty.LayoutType, (PieceTable pieceTable) => new TablePropertiesLayoutTypeModifier(pieceTable) },
			{ JSONTableProperty.TableRowAlignment, (PieceTable pieceTable) => new TablePropertiesTableRowAlignmentModifier(pieceTable) },
			{ JSONTableProperty.TableStyleColBandSize, (PieceTable pieceTable) => new TablePropertiesTableStyleColBandSizeModifier(pieceTable) },
			{ JSONTableProperty.TableStyleRowBandSize, (PieceTable pieceTable) => new TablePropertiesTableStyleRowBandSizeModifier(pieceTable) },
		};
		public ChangeTablePropertyCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeTableProperty; } }
		protected override bool IsEnabled() {
			return Client.DocumentCapabilitiesOptions.TablesAllowed;
		}
		protected override IModelModifier<TablePropertyState> CreateModifier(JSONTableProperty property) {
			JSONPieceTableModifier<TablePropertyState> creator;
			if (!modifiers.TryGetValue(property, out creator))
				throw new ArgumentException();
			return creator(PieceTable);
		}
	}
	abstract class TablePropertyModelModifierBase<T> : TablePropertyModelModifier<T> {
		public TablePropertyModelModifierBase(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyCore(DocumentLogPosition tablePosition, int tableNestedLevel, T value) {
			var tableStartParagraphIndex = PieceTable.FindParagraphIndex(tablePosition);
			var table = PieceTable.TableCellsManager.GetCellByNestingLevel(tableStartParagraphIndex, tableNestedLevel).Table;
			ModifyTableCore(table.TableProperties, value);
		}
		protected abstract void ModifyTableCore(TableProperties properties, T value);
	}
	abstract class TablePropertyWithUseModelModifierBase<T> : TablePropertyWithUseModelModifier<T> {
		public TablePropertyWithUseModelModifierBase(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyCore(DocumentLogPosition tablePosition, int tableNestedLevel, T value, bool useValue) {
			var tableStartParagraphIndex = PieceTable.FindParagraphIndex(tablePosition);
			var table = PieceTable.TableCellsManager.GetCellByNestingLevel(tableStartParagraphIndex, tableNestedLevel).Table;
			var options = table.TableProperties.GetInfoForModification();
			ModifyTableCore(table.TableProperties, options, value, useValue);
			ReplaceInfo(table.TableProperties, options);
		}
		protected abstract void ReplaceInfo(TableProperties properties, TablePropertiesOptions options);
		protected abstract void ModifyTableCore(TableProperties properties, TablePropertiesOptions options, T value, bool useValue);
	}
	abstract class TableGeneralPropertyWithUseModelModifierBase<T> : TablePropertyWithUseModelModifierBase<T> {
		public TableGeneralPropertyWithUseModelModifierBase(PieceTable pieceTable) : base(pieceTable) { }
		protected abstract TableGeneralSettingsChangeType TableChangeType { get; }
		protected override void ReplaceInfo(TableProperties properties, TablePropertiesOptions options) {
			properties.ReplaceInfo(options, TableGeneralSettingsChangeActionsCalculator.CalculateChangeActions(TableChangeType));
		}
	}
	abstract class TableFloatingPositionPropertyWithUseModelModifierBase<T> : TablePropertyWithUseModelModifierBase<T> {
		public TableFloatingPositionPropertyWithUseModelModifierBase(PieceTable pieceTable) : base(pieceTable) { }
		protected abstract TableFloatingPositionChangeType TableFloatingPositionChangeType { get; }
		protected override void ReplaceInfo(TableProperties properties, TablePropertiesOptions options) {
			properties.ReplaceInfo(options, TableFloatingPositionChangeActionsCalculator.CalculateChangeActions(TableFloatingPositionChangeType));
		}
	}
	abstract class TableComplexPropertyWithUseModelModifierBase<T> : TableComplexPropertyWithUseModelModifier<T> {
		public TableComplexPropertyWithUseModelModifierBase(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyCore(DocumentLogPosition tablePosition, int tableNestedLevel, T[] values, bool[] useValues) {
			var tableStartParagraphIndex = PieceTable.FindParagraphIndex(tablePosition);
			var table = PieceTable.TableCellsManager.GetCellByNestingLevel(tableStartParagraphIndex, tableNestedLevel).Table;
			var options = table.TableProperties.GetInfoForModification();
			ModifyTableCore(table.TableProperties, options, values, useValues);
			ReplaceInfo(table.TableProperties, options);
		}
		protected abstract void ModifyTableCore(TableProperties properties, TablePropertiesOptions options, T[] values, bool[] useValues);
		protected abstract void ReplaceInfo(TableProperties properties, TablePropertiesOptions options);
	}
	class TablePropertiesAvoidDoubleBordersModifier : TableGeneralPropertyWithUseModelModifierBase<bool> {
		public TablePropertiesAvoidDoubleBordersModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableGeneralSettingsChangeType TableChangeType { get { return TableGeneralSettingsChangeType.AvoidDoubleBorders; } }
		protected override void ModifyTableCore(TableProperties properties, TablePropertiesOptions options, bool value, bool useValue) {
			properties.AvoidDoubleBorders = value;
			options.UseAvoidDoubleBorders = useValue;
		}
	}
	class TablePropertiesBackgroundColorModifier : TableGeneralPropertyWithUseModelModifierBase<int> {
		public TablePropertiesBackgroundColorModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableGeneralSettingsChangeType TableChangeType { get { return TableGeneralSettingsChangeType.BackgroundColor; } }
		protected override void ModifyTableCore(TableProperties properties, TablePropertiesOptions options, int value, bool useValue) {
			properties.BackgroundColor = System.Drawing.Color.FromArgb(value);
			options.UseBackgroundColor = useValue;
		}
	}
	class TablePropertiesLayoutTypeModifier : TableGeneralPropertyWithUseModelModifierBase<TableLayoutType> {
		public TablePropertiesLayoutTypeModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableGeneralSettingsChangeType TableChangeType { get { return TableGeneralSettingsChangeType.TableLayout; } }
		protected override void ModifyTableCore(TableProperties properties, TablePropertiesOptions options, TableLayoutType value, bool useValue) {
			properties.TableLayout = value;
			options.UseTableLayout = useValue;
		}
	}
	class TablePropertiesBordersModifier : TableComplexPropertyWithUseModelModifierBase<ArrayList> {
		public TablePropertiesBordersModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyTableCore(TableProperties properties, TablePropertiesOptions options, ArrayList[] values, bool[] useValues) {
			var borders = new DevExpress.XtraRichEdit.Model.BorderBase[] { properties.Borders.TopBorder, properties.Borders.RightBorder, properties.Borders.BottomBorder, properties.Borders.LeftBorder, properties.Borders.InsideHorizontalBorder, properties.Borders.InsideVerticalBorder};
			var skippedIndices = new List<int>();
			for (int i = 0; i < 6; i++) {
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
		protected override void ReplaceInfo(TableProperties properties, TablePropertiesOptions options) {
			properties.ReplaceInfo(options, BorderChangeActionsCalculator.CalculateChangeActions(BorderChangeType.BatchUpdate));
		}
	}
	class TablePropertiesTableRowAlignmentModifier : TableGeneralPropertyWithUseModelModifierBase<TableRowAlignment> {
		public TablePropertiesTableRowAlignmentModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableGeneralSettingsChangeType TableChangeType { get { return TableGeneralSettingsChangeType.TableAlignment; } }
		protected override void ModifyTableCore(TableProperties properties, TablePropertiesOptions options, TableRowAlignment value, bool useValue) {
			properties.TableAlignment = value;
			options.UseTableAlignment = useValue;
		}
	}
	class TablePropertiesTableStyleColBandSizeModifier : TableGeneralPropertyWithUseModelModifierBase<int> {
		public TablePropertiesTableStyleColBandSizeModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableGeneralSettingsChangeType TableChangeType { get { return TableGeneralSettingsChangeType.TableStyleColumnBandSize; } }
		protected override void ModifyTableCore(TableProperties properties, TablePropertiesOptions options, int value, bool useValue) {
			properties.TableStyleColBandSize = value;
			options.UseTableStyleColBandSize = useValue;
		}
	}
	class TablePropertiesTableStyleRowBandSizeModifier : TableGeneralPropertyWithUseModelModifierBase<int> {
		public TablePropertiesTableStyleRowBandSizeModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableGeneralSettingsChangeType TableChangeType { get { return TableGeneralSettingsChangeType.TableStyleRowBandSize; } }
		protected override void ModifyTableCore(TableProperties properties, TablePropertiesOptions options, int value, bool useValue) {
			properties.TableStyleRowBandSize = value;
			options.UseTableStyleRowBandSize = useValue;
		}
	}
	class TablePropertiesIsTableOverlapModifier : TableGeneralPropertyWithUseModelModifierBase<bool> {
		public TablePropertiesIsTableOverlapModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableGeneralSettingsChangeType TableChangeType { get { return TableGeneralSettingsChangeType.IsTableOverlap; } }
		protected override void ModifyTableCore(TableProperties properties, TablePropertiesOptions options, bool value, bool useValue) {
			properties.IsTableOverlap = value;
			options.UseIsTableOverlap = useValue;
		}
	}
	class TablePropertiesCellSpacingModifier : TablePropertyWithUseModelModifierBase<ArrayList> {
		public TablePropertiesCellSpacingModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyTableCore(TableProperties properties, TablePropertiesOptions options, ArrayList value, bool useValue) {
			properties.CellSpacing.Type = (WidthUnitType)value[0];
			properties.CellSpacing.Value = (int)value[1];
			options.UseCellSpacing = useValue;
		}
		protected override void ReplaceInfo(TableProperties properties, TablePropertiesOptions options) {
			properties.ReplaceInfo(options, DocumentModelChangeActions.None);
		}
	}
	class TablePropertiesCellMarginsModifier : TableComplexPropertyWithUseModelModifierBase<ArrayList> {
		public TablePropertiesCellMarginsModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyTableCore(TableProperties properties, TablePropertiesOptions options, ArrayList[] values, bool[] useValues) {
			var margins = new MarginUnitBase[] { properties.CellMargins.Top, properties.CellMargins.Right, properties.CellMargins.Bottom, properties.CellMargins.Left };
			for(var i = 0; i < 4; i++) {
				margins[i].Type = (WidthUnitType)values[i][0];
				margins[i].Value = (int)values[i][1];
			}
			options.UseTopMargin = useValues[0];
			options.UseRightMargin = useValues[1];
			options.UseBottomMargin = useValues[2];
			options.UseLeftMargin = useValues[3];
		}
		protected override void ReplaceInfo(TableProperties properties, TablePropertiesOptions options) {
			properties.ReplaceInfo(options, DocumentModelChangeActions.None);
		}
	}
	class TablePropertiesIndentModifier : TablePropertyWithUseModelModifierBase<ArrayList> {
		public TablePropertiesIndentModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyTableCore(TableProperties properties, TablePropertiesOptions options, ArrayList value, bool useValue) {
			properties.TableIndent.Type = (WidthUnitType)value[0];
			properties.TableIndent.Value = (int)value[1];
			options.UseCellSpacing = useValue;
		}
		protected override void ReplaceInfo(TableProperties properties, TablePropertiesOptions options) {
			properties.ReplaceInfo(options, DocumentModelChangeActions.None);
		}
	}
	class TablePropertiesPreferredWidthModifier : TablePropertyModelModifierBase<ArrayList> {
		public TablePropertiesPreferredWidthModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyTableCore(TableProperties properties, ArrayList value) {
			properties.PreferredWidth.Type = (WidthUnitType)value[0];
			properties.PreferredWidth.Value = (int)value[1];
		}
	}
	class TablePropertiesTableLookTypesModifier : TablePropertyModelModifierBase<TableLookTypes> {
		public TablePropertiesTableLookTypesModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyTableCore(TableProperties properties, TableLookTypes value) {
			properties.TableLook = value;
		}
	}
}
