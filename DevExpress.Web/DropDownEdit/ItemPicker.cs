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
using System.Text;
using System.ComponentModel;
using DevExpress.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using System.Collections;
namespace DevExpress.Web.Internal {
	[ToolboxItem(false)
]
	public abstract class ItemPickerBase : ASPxWebControl {
		protected internal const string DefaultMainCssResourceName = ASPxColorEdit.EditDefaultCssResourceName; 
		int columnCount = 0;
		Collection items;
		AppearanceStyle tableStyle;
		ItemPickerTableCellStyle tableCellStyle;
		public ItemPickerBase()
			: this(null) {
		}
		public ItemPickerBase(ASPxWebControl ownerControl)
			: base(ownerControl) {
			this.tableStyle = new AppearanceStyle();
			this.tableCellStyle = CreateTableCellStyles();
			this.items = CreateItemsCollection();
		}
		protected virtual Collection CreateItemsCollection() {
			return new Collection();
		}
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		public AppearanceStyle TableStyle {
			get { return this.tableStyle; }
		}
		public ItemPickerTableCellStyle TableCellStyle {
			get { return this.tableCellStyle; }
		}
		protected ItemPickerStyles Styles {
			get { return StylesInternal as ItemPickerStyles; }
		}
		public string GetControlOnClick() {
			return string.Format(ControlClickHandlerName, ClientID);
		}
		protected internal virtual AppearanceStyle GetTableStyle() {
			AppearanceStyle ret = new AppearanceStyle();
			ret.CopyFrom(Styles.GetDefaultTableStyle());
			ret.CopyFrom(TableStyle);
			ret.CopyFrom(ControlStyle);
			return ret;
		}
		protected internal virtual AppearanceStyle GetTableCellStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetDefaultCellStyle());
			style.CopyFrom(TableCellStyle);
			return style;
		}
		protected internal virtual AppearanceStyle GetTableCellHoverStyle() {
			AppearanceStyle ret = new AppearanceStyle();
			ret.CopyFrom(Styles.GetDefaultTableCellHoverStyle());
			ret.CopyFrom(TableCellStyle.HoverStyle);
			return ret;
		}
		protected override StylesBase CreateStyles() {
			return new ItemPickerStyles(this);
		}
		protected virtual ItemPickerTableCellStyle CreateTableCellStyles() {
			return new ItemPickerTableCellStyle();
		}
		protected override bool HasHoverScripts() {
			return IsEnabled();
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			AppearanceStyleBase style = GetTableCellHoverStyle();
			for(int i = 0; i < RowCount * ColumnCount && i < Items.Count; i++) {
				helper.AddStyle(style, GetItemCellID(i), IsEnabled());
			}
		}
		protected override void RegisterDefaultRenderCssFile() {
			ResourceManager.RegisterCssResource(Page, typeof(ItemPickerBase), DefaultMainCssResourceName);
		}
		protected override string GetSkinControlName() {
			return "Editors";
		}
		[DefaultValue(3)]
		public int ColumnCount {
			get { return columnCount; }
			set { columnCount = value; }
		}
		public virtual int RowCount {
			get { return (int)Math.Ceiling((double)items.Count / (double)ColumnCount); }
		}
		public Collection Items {
			get { return items; }
		}
		protected internal virtual string GetItemCellID(int index) {
			return "C" + index.ToString();
		}
		protected internal virtual string GetItemsTableID() {
			return "CT";
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { TableStyle, TableCellStyle });
		}
	}
	public abstract class ItemPickerBaseControl : ASPxInternalWebControl {
		ItemPickerBase itemsPicker = null;
		Table mainTable = null;
		TableCell mainCell = null;
		Table itemPickerTable = null;
		List<TableCell> itemsCells = null;
		public ItemPickerBaseControl(ItemPickerBase itemPicker) {
			this.itemsPicker = itemPicker;
		}
		protected ItemPickerBase ItemPicker {
			get { return itemsPicker; }
		}
		protected TableCell MainCell {
			get { return this.mainCell; }
		}
		protected Table MainTable {
			get { return this.mainTable; }
		}
		protected Table ItemPickerTable {
			get { return this.itemPickerTable; }
		}
		protected List<TableCell> ItemsCells {
			get { return this.itemsCells; }
		}
		protected override void ClearControlFields() {
			this.mainTable = null;
			this.mainCell = null;
			this.itemPickerTable = null;
			this.itemsCells = null;
		}
		protected override void CreateControlHierarchy() {
			this.mainTable = RenderUtils.CreateTable(true);
			Controls.Add(MainTable);
			TableRow row = RenderUtils.CreateTableRow();
			MainTable.Rows.Add(row);
			this.mainCell = RenderUtils.CreateTableCell();
			row.Cells.Add(MainCell);
			CreateItemsTableCells(MainCell);
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(ItemPicker, MainTable);
			RenderUtils.AppendDefaultDXClassName(MainCell, RenderUtils.DefaultStyleNamePrefix);
			RenderUtils.SetPaddings(MainCell, ItemPicker.GetTableStyle().Paddings);
			ItemPicker.GetTableStyle().AssignToControl(MainTable);
		}
		protected void CreateItemsTableCells(WebControl parent) {
			this.itemPickerTable = RenderUtils.CreateTable(true);
			parent.Controls.Add(ItemPickerTable);
			ItemPickerTable.ID = ItemPicker.GetItemsTableID();
			this.itemsCells = new List<TableCell>();
			for(int i = 0; i < ItemPicker.RowCount; i++) {
				TableRow row = RenderUtils.CreateTableRow();
				ItemPickerTable.Rows.Add(row);
				for(int j = 0; j < ItemPicker.ColumnCount && AllowRenderCell(i * ItemPicker.ColumnCount + j); j++) {
					TableCell cell = RenderUtils.CreateTableCell();
					row.Cells.Add(cell);
					int index = i * ItemPicker.ColumnCount + j;
					CreateItemsTableCellContent(cell, index);
					ItemsCells.Add(cell);
				}
			}
		}
		protected virtual bool AllowRenderCell(int index) {
			return true;
		}
		protected virtual void CreateItemsTableCellContent(TableCell cell, int index) {
		}
	}
	public class ItemPickerStyles : StylesBase {
		public ItemPickerStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxe";
		}
		protected virtual string GetControlClassName() {
			return "ItemPicker";
		}
		protected internal virtual AppearanceStyle GetDefaultTableStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(GetControlClassName()));
			return style;
		}
		protected internal virtual AppearanceStyle GetDefaultCellStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(string.Format("{0}Cell",GetControlClassName())));
			return style;
		}
		protected internal virtual AppearanceStyle GetDefaultTableCellHoverStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(string.Format("{0}CellHover", GetControlClassName())));
			return style;
		}
	}
}
