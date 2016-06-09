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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraBars.Ribbon;
using System.Drawing;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.UI {
	#region RichEditTableTableItemBuilder
	public class RichEditTableTableItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SelectTableElementsItem selectItem = new SelectTableElementsItem();
			items.Add(selectItem);
			IBarSubItem selectSubItem = selectItem;
			selectSubItem.AddBarItem(new SelectTableCellItem());
			selectSubItem.AddBarItem(new SelectTableColumnItem());
			selectSubItem.AddBarItem(new SelectTableRowItem());
			selectSubItem.AddBarItem(new SelectTableItem());
			items.Add(new ToggleShowTableGridLinesItem());
			items.Add(new ShowTablePropertiesFormItem());
		}
	}
	#endregion
	#region SelectTableElementsItem
	public class SelectTableElementsItem : RichEditCommandBarSubItem {
		public SelectTableElementsItem() {
		}
		public SelectTableElementsItem(BarManager manager)
			: base(manager) {
		}
		public SelectTableElementsItem(string caption)
			: base(caption) {
		}
		public SelectTableElementsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SelectTablePlaceholder; } }
	}
	#endregion
	#region SelectTableCellItem
	public class SelectTableCellItem: RichEditCommandBarButtonItem {
		public SelectTableCellItem() {
		}
		public SelectTableCellItem(BarManager manager)
			: base(manager) {
		}
		public SelectTableCellItem(string caption)
			: base(caption) {
		}
		public SelectTableCellItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SelectTableCell; } }
	}
	#endregion
	#region SelectTableColumnItem
	public class SelectTableColumnItem: RichEditCommandBarButtonItem {
		public SelectTableColumnItem() {
		}
		public SelectTableColumnItem(BarManager manager)
			: base(manager) {
		}
		public SelectTableColumnItem(string caption)
			: base(caption) {
		}
		public SelectTableColumnItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SelectTableColumns; } }
	}
	#endregion
	#region SelectTableRowItem
	public class SelectTableRowItem: RichEditCommandBarButtonItem {
		public SelectTableRowItem() {
		}
		public SelectTableRowItem(BarManager manager)
			: base(manager) {
		}
		public SelectTableRowItem(string caption)
			: base(caption) {
		}
		public SelectTableRowItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SelectTableRow; } }
	}
	#endregion
	#region SelectTableItem
	public class SelectTableItem: RichEditCommandBarButtonItem {
		public SelectTableItem() {
		}
		public SelectTableItem(BarManager manager)
			: base(manager) {
		}
		public SelectTableItem(string caption)
			: base(caption) {
		}
		public SelectTableItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SelectTable; } }
	}
	#endregion
	#region ToggleShowTableGridLinesItem
	public class ToggleShowTableGridLinesItem : RichEditCommandBarCheckItem {
		public ToggleShowTableGridLinesItem() {
		}
		public ToggleShowTableGridLinesItem(BarManager manager)
			: base(manager) {
		}
		public ToggleShowTableGridLinesItem(string caption)
			: base(caption) {
		}
		public ToggleShowTableGridLinesItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleShowTableGridLines; } }
	}
	#endregion
	#region ShowTablePropertiesFormItem
	public class ShowTablePropertiesFormItem: RichEditCommandBarButtonItem {
		public ShowTablePropertiesFormItem() {
		}
		public ShowTablePropertiesFormItem(BarManager manager)
			: base(manager) {
		}
		public ShowTablePropertiesFormItem(string caption)
			: base(caption) {
		}
		public ShowTablePropertiesFormItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowTablePropertiesForm; } }
	}
	#endregion
	#region RichEditTableRowsAndColumnsItemBuilder
	public class RichEditTableRowsAndColumnsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			DeleteTableElementsItem deleteItem = new DeleteTableElementsItem();
			items.Add(deleteItem);
			IBarSubItem deleteSubItem = deleteItem;
			deleteSubItem.AddBarItem(new ShowDeleteTableCellsFormItem());
			deleteSubItem.AddBarItem(new DeleteTableColumnsItem());
			deleteSubItem.AddBarItem(new DeleteTableRowsItem());
			deleteSubItem.AddBarItem(new DeleteTableItem());
			items.Add(new InsertTableRowAboveItem());
			items.Add(new InsertTableRowBelowItem());
			items.Add(new InsertTableColumnToLeftItem());
			items.Add(new InsertTableColumnToRightItem());
			items.Add(new ShowInsertTableCellsFormItem());
		}
	}
	#endregion
	#region DeleteTableElementsItem
	public class DeleteTableElementsItem : RichEditCommandBarSubItem {
		public DeleteTableElementsItem() {
		}
		public DeleteTableElementsItem(BarManager manager)
			: base(manager) {
		}
		public DeleteTableElementsItem(string caption)
			: base(caption) {
		}
		public DeleteTableElementsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.DeleteTablePlaceholder; } }
	}
	#endregion
	#region ShowDeleteTableCellsFormItem
	public class ShowDeleteTableCellsFormItem: RichEditCommandBarButtonItem {
		public ShowDeleteTableCellsFormItem() {
		}
		public ShowDeleteTableCellsFormItem(BarManager manager)
			: base(manager) {
		}
		public ShowDeleteTableCellsFormItem(string caption)
			: base(caption) {
		}
		public ShowDeleteTableCellsFormItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowDeleteTableCellsForm; } }
	}
	#endregion
	#region DeleteTableColumnsItem
	public class DeleteTableColumnsItem: RichEditCommandBarButtonItem {
		public DeleteTableColumnsItem() {
		}
		public DeleteTableColumnsItem(BarManager manager)
			: base(manager) {
		}
		public DeleteTableColumnsItem(string caption)
			: base(caption) {
		}
		public DeleteTableColumnsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.DeleteTableColumns; } }
	}
	#endregion
	#region DeleteTableRowsItem
	public class DeleteTableRowsItem: RichEditCommandBarButtonItem {
		public DeleteTableRowsItem() {
		}
		public DeleteTableRowsItem(BarManager manager)
			: base(manager) {
		}
		public DeleteTableRowsItem(string caption)
			: base(caption) {
		}
		public DeleteTableRowsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.DeleteTableRows; } }
	}
	#endregion
	#region DeleteTableItem
	public class DeleteTableItem: RichEditCommandBarButtonItem {
		public DeleteTableItem() {
		}
		public DeleteTableItem(BarManager manager)
			: base(manager) {
		}
		public DeleteTableItem(string caption)
			: base(caption) {
		}
		public DeleteTableItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.DeleteTable; } }
	}
	#endregion
	#region InsertTableRowBelowItem
	public class InsertTableRowBelowItem: RichEditCommandBarButtonItem {
		public InsertTableRowBelowItem() {
		}
		public InsertTableRowBelowItem(BarManager manager)
			: base(manager) {
		}
		public InsertTableRowBelowItem(string caption)
			: base(caption) {
		}
		public InsertTableRowBelowItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertTableRowBelow; } }
	}
	#endregion
	#region InsertTableRowAboveItem
	public class InsertTableRowAboveItem: RichEditCommandBarButtonItem {
		public InsertTableRowAboveItem() {
		}
		public InsertTableRowAboveItem(BarManager manager)
			: base(manager) {
		}
		public InsertTableRowAboveItem(string caption)
			: base(caption) {
		}
		public InsertTableRowAboveItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertTableRowAbove; } }
	}
	#endregion
	#region InsertTableColumnToLeftItem
	public class InsertTableColumnToLeftItem: RichEditCommandBarButtonItem {
		public InsertTableColumnToLeftItem() {
		}
		public InsertTableColumnToLeftItem(BarManager manager)
			: base(manager) {
		}
		public InsertTableColumnToLeftItem(string caption)
			: base(caption) {
		}
		public InsertTableColumnToLeftItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertTableColumnToTheLeft; } }
	}
	#endregion
	#region InsertTableColumnToRightItem
	public class InsertTableColumnToRightItem: RichEditCommandBarButtonItem {
		public InsertTableColumnToRightItem() {
		}
		public InsertTableColumnToRightItem(BarManager manager)
			: base(manager) {
		}
		public InsertTableColumnToRightItem(string caption)
			: base(caption) {
		}
		public InsertTableColumnToRightItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertTableColumnToTheRight; } }
	}
	#endregion
	#region ShowInsertTableCellsFormItem
	public class ShowInsertTableCellsFormItem: RichEditCommandBarButtonItem {
		public ShowInsertTableCellsFormItem() {
		}
		public ShowInsertTableCellsFormItem(BarManager manager)
			: base(manager) {
		}
		public ShowInsertTableCellsFormItem(string caption)
			: base(caption) {
		}
		public ShowInsertTableCellsFormItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowInsertTableCellsForm; } }
	}
	#endregion
	#region RichEditTableMergeItemBuilder
	public class RichEditTableMergeItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new MergeTableCellsItem());
			items.Add(new ShowSplitTableCellsForm());
			items.Add(new SplitTableItem());
		}
	}
	#endregion
	#region MergeTableCellsItem
	public class MergeTableCellsItem: RichEditCommandBarButtonItem {
		public MergeTableCellsItem() {
		}
		public MergeTableCellsItem(BarManager manager)
			: base(manager) {
		}
		public MergeTableCellsItem(string caption)
			: base(caption) {
		}
		public MergeTableCellsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.MergeTableCells; } }
	}
	#endregion
	#region ShowSplitTableCellsForm
	public class ShowSplitTableCellsForm: RichEditCommandBarButtonItem {
		public ShowSplitTableCellsForm() {
		}
		public ShowSplitTableCellsForm(BarManager manager)
			: base(manager) {
		}
		public ShowSplitTableCellsForm(string caption)
			: base(caption) {
		}
		public ShowSplitTableCellsForm(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowSplitTableCellsForm; } }
	}
	#endregion
	#region SplitTableItem
	public class SplitTableItem: RichEditCommandBarButtonItem {
		public SplitTableItem() {
		}
		public SplitTableItem(BarManager manager)
			: base(manager) {
		}
		public SplitTableItem(string caption)
			: base(caption) {
		}
		public SplitTableItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SplitTable; } }
	}
	#endregion
	#region RichEditTableCellSizeItemBuilder
	public class RichEditTableCellSizeItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			ToggleTableAutoFitItem autoFitItem = new ToggleTableAutoFitItem();
			items.Add(autoFitItem);
			IBarSubItem autoFitSubItem = autoFitItem;
			autoFitSubItem.AddBarItem(new ToggleTableAutoFitContentsItem());
			autoFitSubItem.AddBarItem(new ToggleTableAutoFitWindowItem());
			autoFitSubItem.AddBarItem(new ToggleTableFixedColumnWidthItem());
		}
	}
	#endregion
	#region ToggleTableAutoFitItem
	public class ToggleTableAutoFitItem : RichEditCommandBarSubItem {
		public ToggleTableAutoFitItem() {
		}
		public ToggleTableAutoFitItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableAutoFitItem(string caption)
			: base(caption) {
		}
		public ToggleTableAutoFitItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableAutoFitPlaceholder; } }
	}
	#endregion
	#region ToggleTableAutoFitContentsItem
	public class ToggleTableAutoFitContentsItem: RichEditCommandBarButtonItem {
		public ToggleTableAutoFitContentsItem() {
		}
		public ToggleTableAutoFitContentsItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableAutoFitContentsItem(string caption)
			: base(caption) {
		}
		public ToggleTableAutoFitContentsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableAutoFitContents; } }
	}
	#endregion
	#region ToggleTableAutoFitWindowItem
	public class ToggleTableAutoFitWindowItem: RichEditCommandBarButtonItem {
		public ToggleTableAutoFitWindowItem() {
		}
		public ToggleTableAutoFitWindowItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableAutoFitWindowItem(string caption)
			: base(caption) {
		}
		public ToggleTableAutoFitWindowItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableAutoFitWindow; } }
	}
	#endregion
	#region ToggleTableFixedColumnWidthItem
	public class ToggleTableFixedColumnWidthItem: RichEditCommandBarButtonItem {
		public ToggleTableFixedColumnWidthItem() {
		}
		public ToggleTableFixedColumnWidthItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableFixedColumnWidthItem(string caption)
			: base(caption) {
		}
		public ToggleTableFixedColumnWidthItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableFixedColumnWidth; } }
	}
	#endregion
	#region RichEditTableAlignmentItemBuilder
	public class RichEditTableAlignmentItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ToggleTableCellsTopLeftAlignmentItem());
			items.Add(new ToggleTableCellsMiddleLeftAlignmentItem());
			items.Add(new ToggleTableCellsBottomLeftAlignmentItem());
			items.Add(new ToggleTableCellsTopCenterAlignmentItem());
			items.Add(new ToggleTableCellsMiddleCenterAlignmentItem());
			items.Add(new ToggleTableCellsBottomCenterAlignmentItem());
			items.Add(new ToggleTableCellsTopRightAlignmentItem());
			items.Add(new ToggleTableCellsMiddleRightAlignmentItem());
			items.Add(new ToggleTableCellsBottomRightAlignmentItem());
			items.Add(new ShowTableOptionsFormItem());
		}
	}
	#endregion
	#region ToggleTableCellsTopLeftAlignmentItem
	public class ToggleTableCellsTopLeftAlignmentItem: RichEditCommandBarButtonItem {
		public ToggleTableCellsTopLeftAlignmentItem() {
		}
		public ToggleTableCellsTopLeftAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableCellsTopLeftAlignmentItem(string caption)
			: base(caption) {
		}
		public ToggleTableCellsTopLeftAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableCellsTopLeftAlignment; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
	}
	#endregion
	#region ToggleTableCellsTopCenterAlignmentItem
	public class ToggleTableCellsTopCenterAlignmentItem: RichEditCommandBarButtonItem {
		public ToggleTableCellsTopCenterAlignmentItem() {
		}
		public ToggleTableCellsTopCenterAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableCellsTopCenterAlignmentItem(string caption)
			: base(caption) {
		}
		public ToggleTableCellsTopCenterAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableCellsTopCenterAlignment; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
	}
	#endregion
	#region ToggleTableCellsTopRightAlignmentItem
	public class ToggleTableCellsTopRightAlignmentItem: RichEditCommandBarButtonItem {
		public ToggleTableCellsTopRightAlignmentItem() {
		}
		public ToggleTableCellsTopRightAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableCellsTopRightAlignmentItem(string caption)
			: base(caption) {
		}
		public ToggleTableCellsTopRightAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableCellsTopRightAlignment; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
	}
	#endregion
	#region ToggleTableCellsMiddleLeftAlignmentItem
	public class ToggleTableCellsMiddleLeftAlignmentItem: RichEditCommandBarButtonItem {
		public ToggleTableCellsMiddleLeftAlignmentItem() {
		}
		public ToggleTableCellsMiddleLeftAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableCellsMiddleLeftAlignmentItem(string caption)
			: base(caption) {
		}
		public ToggleTableCellsMiddleLeftAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableCellsMiddleLeftAlignment; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
	}
	#endregion
	#region ToggleTableCellsMiddleCenterAlignmentItem
	public class ToggleTableCellsMiddleCenterAlignmentItem: RichEditCommandBarButtonItem {
		public ToggleTableCellsMiddleCenterAlignmentItem() {
		}
		public ToggleTableCellsMiddleCenterAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableCellsMiddleCenterAlignmentItem(string caption)
			: base(caption) {
		}
		public ToggleTableCellsMiddleCenterAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableCellsMiddleCenterAlignment; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
	}
	#endregion
	#region ToggleTableCellsMiddleRightAlignmentItem
	public class ToggleTableCellsMiddleRightAlignmentItem: RichEditCommandBarButtonItem {
		public ToggleTableCellsMiddleRightAlignmentItem() {
		}
		public ToggleTableCellsMiddleRightAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableCellsMiddleRightAlignmentItem(string caption)
			: base(caption) {
		}
		public ToggleTableCellsMiddleRightAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableCellsMiddleRightAlignment; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
	}
	#endregion
	#region ToggleTableCellsBottomLeftAlignmentItem
	public class ToggleTableCellsBottomLeftAlignmentItem: RichEditCommandBarButtonItem {
		public ToggleTableCellsBottomLeftAlignmentItem() {
		}
		public ToggleTableCellsBottomLeftAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableCellsBottomLeftAlignmentItem(string caption)
			: base(caption) {
		}
		public ToggleTableCellsBottomLeftAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableCellsBottomLeftAlignment; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
	}
	#endregion
	#region ToggleTableCellsBottomCenterAlignmentItem
	public class ToggleTableCellsBottomCenterAlignmentItem: RichEditCommandBarButtonItem {
		public ToggleTableCellsBottomCenterAlignmentItem() {
		}
		public ToggleTableCellsBottomCenterAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableCellsBottomCenterAlignmentItem(string caption)
			: base(caption) {
		}
		public ToggleTableCellsBottomCenterAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableCellsBottomCenterAlignment; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
	}
	#endregion
	#region ToggleTableCellsBottomRightAlignmentItem
	public class ToggleTableCellsBottomRightAlignmentItem: RichEditCommandBarButtonItem {
		public ToggleTableCellsBottomRightAlignmentItem() {
		}
		public ToggleTableCellsBottomRightAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTableCellsBottomRightAlignmentItem(string caption)
			: base(caption) {
		}
		public ToggleTableCellsBottomRightAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTableCellsBottomRightAlignment; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
	}
	#endregion
	#region ShowTableOptionsFormItem
	public class ShowTableOptionsFormItem: RichEditCommandBarButtonItem {
		public ShowTableOptionsFormItem() {
		}
		public ShowTableOptionsFormItem(BarManager manager)
			: base(manager) {
		}
		public ShowTableOptionsFormItem(string caption)
			: base(caption) {
		}
		public ShowTableOptionsFormItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowTableOptionsForm; } }
	}
	#endregion
	#region RichEditTableTableBarCreator
	public class RichEditTableTableBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(TableLayoutRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(TableTableRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(TableToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(TableTableBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 8; } }
		public override Bar CreateBar() {
			return new TableTableBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditTableTableItemBuilder();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new TableToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new TableLayoutRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new TableTableRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditTableRowsAndColumnsBarCreator
	public class RichEditTableRowsAndColumnsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(TableLayoutRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(TableRowsAndColumnsRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(TableToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(TableRowsAndColumnsBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 9; } }
		public override Bar CreateBar() {
			return new TableRowsAndColumnsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditTableRowsAndColumnsItemBuilder();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new TableToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new TableLayoutRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new TableRowsAndColumnsRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditTableMergeBarCreator
	public class RichEditTableMergeBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(TableLayoutRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(TableMergeRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(TableToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(TableMergeBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 10; } }
		public override Bar CreateBar() {
			return new TableMergeBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditTableMergeItemBuilder();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new TableToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new TableLayoutRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new TableMergeRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditTableCellSizeBarCreator
	public class RichEditTableCellSizeBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(TableLayoutRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(TableCellSizeRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(TableToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(TableCellSizeBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 11; } }
		public override Bar CreateBar() {
			return new TableCellSizeBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditTableCellSizeItemBuilder();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new TableToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new TableLayoutRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new TableCellSizeRibbonPageGroup() { AllowTextClipping = false };
		}
	}
	#endregion
	#region RichEditTableAlignmentBarCreator
	public class RichEditTableAlignmentBarCreator : ControlCommandBarCreator {
		private static readonly System.Reflection.Assembly imageResourceAssembly = typeof(IRichEditControl).Assembly;
		private const string imageResourcePrefix = "DevExpress.XtraRichEdit.Images";
		private const string pageGroupImageName = "AlignMiddleCenter";
		public override Type SupportedRibbonPageType { get { return typeof(TableLayoutRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(TableAlignmentRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(TableToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(TableAlignmentBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 12; } }
		public override Bar CreateBar() {
			return new TableAlignmentBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditTableAlignmentItemBuilder();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new TableToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new TableLayoutRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			Image glyph = CommandResourceImageLoader.LoadSmallImage(imageResourcePrefix, pageGroupImageName, imageResourceAssembly);
			return new TableAlignmentRibbonPageGroup() { Glyph = glyph };
		}
	}
	#endregion
	#region TableTableBar
	public class TableTableBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public TableTableBar()
			: base() {
		}
		public TableTableBar(BarManager manager)
			: base(manager) {
		}
		public TableTableBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_GroupTableTable); } }
	}
	#endregion
	#region TableRowsAndColumnsBar
	public class TableRowsAndColumnsBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public TableRowsAndColumnsBar()
			: base() {
		}
		public TableRowsAndColumnsBar(BarManager manager)
			: base(manager) {
		}
		public TableRowsAndColumnsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_GroupTableRowsAndColumns); } }
	}
	#endregion
	#region TableMergeBar
	public class TableMergeBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public TableMergeBar()
			: base() {
		}
		public TableMergeBar(BarManager manager)
			: base(manager) {
		}
		public TableMergeBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_GroupTableMerge); } }
	}
	#endregion
	#region TableCellSizeBar
	public class TableCellSizeBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public TableCellSizeBar()
			: base() {
		}
		public TableCellSizeBar(BarManager manager)
			: base(manager) {
		}
		public TableCellSizeBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_GroupTableCellSize); } }
	}
	#endregion
	#region TableAlignmentBar
	public class TableAlignmentBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public TableAlignmentBar()
			: base() {
		}
		public TableAlignmentBar(BarManager manager)
			: base(manager) {
		}
		public TableAlignmentBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_GroupTableAlignment); } }
	}
	#endregion
	#region TableLayoutRibbonPage
	public class TableLayoutRibbonPage : ControlCommandBasedRibbonPage {
		public TableLayoutRibbonPage() {
		}
		public TableLayoutRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_PageTableLayout); } }
		protected override RibbonPage CreatePage() {
			return new TableLayoutRibbonPage();
		}
	}
	#endregion
	#region TableTableRibbonPageGroup
	public class TableTableRibbonPageGroup : RichEditControlRibbonPageGroup {
		public TableTableRibbonPageGroup() {
		}
		public TableTableRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_GroupTableTable); } }
	}
	#endregion
	#region TableRowsAndColumnsRibbonPageGroup
	public class TableRowsAndColumnsRibbonPageGroup : RichEditControlRibbonPageGroup {
		public TableRowsAndColumnsRibbonPageGroup() {
		}
		public TableRowsAndColumnsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_GroupTableRowsAndColumns); } }
		public override RichEditCommandId CommandId { get { return RichEditCommandId.ShowInsertTableCellsForm; } }
	}
	#endregion
	#region TableMergeRibbonPageGroup
	public class TableMergeRibbonPageGroup : RichEditControlRibbonPageGroup {
		public TableMergeRibbonPageGroup() {
		}
		public TableMergeRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_GroupTableMerge); } }
	}
	#endregion
	#region TableCellSizeRibbonPageGroup
	public class TableCellSizeRibbonPageGroup : RichEditControlRibbonPageGroup {
		public TableCellSizeRibbonPageGroup() {
		}
		public TableCellSizeRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_GroupTableCellSize); } }
		public override RichEditCommandId CommandId { get { return RichEditCommandId.ShowTablePropertiesForm; } }
	}
	#endregion
	#region TableAlignmentRibbonPageGroup
	public class TableAlignmentRibbonPageGroup : RichEditControlRibbonPageGroup {
		public TableAlignmentRibbonPageGroup() {
		}
		public TableAlignmentRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_GroupTableAlignment); } }
	}
	#endregion
	#region TableToolsRibbonPageCategory
	public class TableToolsRibbonPageCategory : ControlCommandBasedRibbonPageCategory<RichEditControl, RichEditCommandId> {
		public TableToolsRibbonPageCategory() {
			this.Color = DXColor.FromArgb(0xff, 0xfc, 0xe9, 0x14);
		}
		public override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_PageCategoryTableTools); } }
		protected override RichEditCommandId EmptyCommandId { get { return RichEditCommandId.None; } }
		public override RichEditCommandId CommandId { get { return RichEditCommandId.ToolsTableCommandGroup; } }
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new TableToolsRibbonPageCategory();
		}
	}
	#endregion
}
