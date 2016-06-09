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
	public class ChangeTableRowCommand : WebRichEditPropertyStateBasedCommand<TableRowState, JSONTableRowProperty> {
		static Dictionary<JSONTableRowProperty, JSONPieceTableModifier<TableRowState>> modifiers = new Dictionary<JSONTableRowProperty, JSONPieceTableModifier<TableRowState>>() {
			{ JSONTableRowProperty.Height, (PieceTable pieceTable) => new TableRowPropertiesHeightModifier(pieceTable) },
			{ JSONTableRowProperty.GridAfter, (PieceTable pieceTable) => new TableRowPropertiesGridAfterModifier(pieceTable) },
			{ JSONTableRowProperty.GridBefore, (PieceTable pieceTable) => new TableRowPropertiesGridBeforeModifier(pieceTable) },
			{ JSONTableRowProperty.WidthAfter, (PieceTable pieceTable) => new TableRowPropertiesWidthAfterModifier(pieceTable) },
			{ JSONTableRowProperty.WidthBefore, (PieceTable pieceTable) => new TableRowPropertiesWidthBeforeModifier(pieceTable) },
		};
		public ChangeTableRowCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeTableRow; } }
		protected override bool IsEnabled() {
			return Client.DocumentCapabilitiesOptions.TablesAllowed;
		}
		protected override IModelModifier<TableRowState> CreateModifier(JSONTableRowProperty property) {
			JSONPieceTableModifier<TableRowState> creator;
			if (!modifiers.TryGetValue(property, out creator))
				throw new ArgumentException();
			return creator(PieceTable);
		}
	}
	public class ChangeTableRowPropertyCommand : WebRichEditPropertyStateBasedCommand<TableRowPropertyState, JSONTableRowProperty> {
		static Dictionary<JSONTableRowProperty, JSONPieceTableModifier<TableRowPropertyState>> modifiers = new Dictionary<JSONTableRowProperty, JSONPieceTableModifier<TableRowPropertyState>>() {
			{ JSONTableRowProperty.CantSplit, (PieceTable pieceTable) => new TableRowPropertiesCantSplitModifier(pieceTable) },
			{ JSONTableRowProperty.CellSpacing, (PieceTable pieceTable) => new TableRowPropertiesCellSpacingModifier(pieceTable) },
			{ JSONTableRowProperty.Header, (PieceTable pieceTable) => new TableRowPropertiesHeaderModifier(pieceTable) },
			{ JSONTableRowProperty.HideCellMark, (PieceTable pieceTable) => new TableRowPropertiesHideCellMarkModifier(pieceTable) },
			{ JSONTableRowProperty.TableRowAlignment, (PieceTable pieceTable) => new TableRowPropertiesTableRowAlignmentModifier(pieceTable) },
		};
		public ChangeTableRowPropertyCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeTableRowProperty; } }
		protected override bool IsEnabled() {
			return Client.DocumentCapabilitiesOptions.TablesAllowed;
		}
		protected override IModelModifier<TableRowPropertyState> CreateModifier(JSONTableRowProperty property) {
			JSONPieceTableModifier<TableRowPropertyState> creator;
			if (!modifiers.TryGetValue(property, out creator))
				throw new ArgumentException();
			return creator(PieceTable);
		}
	}
	abstract class TableRowPropertyModelModifierBase<T> : TableRowPropertyModelModifier<T> {
		public TableRowPropertyModelModifierBase(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyCore(DocumentLogPosition tablePosition, int tableNestedLevel, int rowIndex, T value) {
			var tableStartParagraphIndex = PieceTable.FindParagraphIndex(tablePosition);
			var table = PieceTable.TableCellsManager.GetCellByNestingLevel(tableStartParagraphIndex, tableNestedLevel).Table;
			var row = table.Rows[rowIndex];
			ModifyTableCore(row.Properties, value);
		}
		protected abstract void ModifyTableCore(TableRowProperties properties, T value);
	}
	abstract class TableRowPropertyWithUseModelModifierBase<T> : TableRowPropertyWithUseModelModifier<T> {
		public TableRowPropertyWithUseModelModifierBase(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyCore(DocumentLogPosition tablePosition, int tableNestedLevel, int rowIndex, T value, bool useValue) {
			var tableStartParagraphIndex = PieceTable.FindParagraphIndex(tablePosition);
			var table = PieceTable.TableCellsManager.GetCellByNestingLevel(tableStartParagraphIndex, tableNestedLevel).Table;
			var row = table.Rows[rowIndex];
			var info = row.Properties.GetInfoForModification();
			ModifyTableCore(row.Properties, info, value, useValue);
			row.Properties.ReplaceInfo(info, TableRowChangeActionsCalculator.CalculateChangeActions(TableRowChangeType));
		}
		protected abstract void ModifyTableCore(TableRowProperties properties, TableRowPropertiesOptions options, T value, bool useValue);
		protected abstract TableRowChangeType TableRowChangeType { get; }
	}
	class TableRowPropertiesHeightModifier : TableRowPropertyModelModifierBase<ArrayList> {
		public TableRowPropertiesHeightModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyTableCore(TableRowProperties properties, ArrayList value) {
			properties.Height.Type = (HeightUnitType)value[0];
			properties.Height.Value = (int)value[1];
		}
	}
	class TableRowPropertiesGridAfterModifier : TableRowPropertyModelModifierBase<int> {
		public TableRowPropertiesGridAfterModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyTableCore(TableRowProperties properties, int value) {
			properties.GridAfter = value;
		}
	}
	class TableRowPropertiesGridBeforeModifier : TableRowPropertyModelModifierBase<int> {
		public TableRowPropertiesGridBeforeModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyTableCore(TableRowProperties properties, int value) {
			properties.GridBefore = value;
		}
	}
	class TableRowPropertiesWidthAfterModifier : TableRowPropertyModelModifierBase<ArrayList> {
		public TableRowPropertiesWidthAfterModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyTableCore(TableRowProperties properties, ArrayList value) {
			properties.WidthAfter.Type = (WidthUnitType)value[0];
			properties.WidthAfter.Value = (int)value[1];
		}
	}
	class TableRowPropertiesWidthBeforeModifier : TableRowPropertyModelModifierBase<ArrayList> {
		public TableRowPropertiesWidthBeforeModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override void ModifyTableCore(TableRowProperties properties, ArrayList value) {
			properties.WidthBefore.Type = (WidthUnitType)value[0];
			properties.WidthBefore.Value = (int)value[1];
		}
	}
	class TableRowPropertiesCantSplitModifier : TableRowPropertyWithUseModelModifierBase<bool> {
		public TableRowPropertiesCantSplitModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableRowChangeType TableRowChangeType { get { return TableRowChangeType.CantSplit; } }
		protected override void ModifyTableCore(TableRowProperties properties, TableRowPropertiesOptions options, bool value, bool useValue) {
			properties.CantSplit = value;
			options.UseCantSplit = useValue;
		}
	}
	class TableRowPropertiesCellSpacingModifier : TableRowPropertyWithUseModelModifierBase<ArrayList> {
		public TableRowPropertiesCellSpacingModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableRowChangeType TableRowChangeType { get { return TableRowChangeType.None; } }
		protected override void ModifyTableCore(TableRowProperties properties, TableRowPropertiesOptions options, ArrayList value, bool useValue) {
			properties.CellSpacing.Type = (WidthUnitType)value[0];
			properties.CellSpacing.Value = (int)value[1];
			options.UseCellSpacing = useValue;
		}
	}
	class TableRowPropertiesHeaderModifier : TableRowPropertyWithUseModelModifierBase<bool> {
		public TableRowPropertiesHeaderModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableRowChangeType TableRowChangeType { get { return TableRowChangeType.Header; } }
		protected override void ModifyTableCore(TableRowProperties properties, TableRowPropertiesOptions options, bool value, bool useValue) {
			properties.Header = value;
			options.UseHeader = useValue;
		}
	}
	class TableRowPropertiesHideCellMarkModifier : TableRowPropertyWithUseModelModifierBase<bool> {
		public TableRowPropertiesHideCellMarkModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableRowChangeType TableRowChangeType { get { return TableRowChangeType.HideCellMark; } }
		protected override void ModifyTableCore(TableRowProperties properties, TableRowPropertiesOptions options, bool value, bool useValue) {
			properties.HideCellMark = value;
			options.UseHideCellMark = useValue;
		}
	}
	class TableRowPropertiesTableRowAlignmentModifier : TableRowPropertyWithUseModelModifierBase<TableRowAlignment> {
		public TableRowPropertiesTableRowAlignmentModifier(PieceTable pieceTable) : base(pieceTable) { }
		protected override TableRowChangeType TableRowChangeType { get { return TableRowChangeType.TableRowAlignment; } }
		protected override void ModifyTableCore(TableRowProperties properties, TableRowPropertiesOptions options, TableRowAlignment value, bool useValue) {
			properties.TableRowAlignment = value;
			options.UseTableRowAlignment = useValue;
		}
	}
}
