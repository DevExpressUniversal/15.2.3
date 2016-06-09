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
using System.ComponentModel.Design;
using System.Linq;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class GridViewDesigner : GridDesignerBase {
		GridViewDesignerHelper helper;
		public GridViewDesignerHelper Helper {
			get {
				helper = helper ?? new GridViewDesignerHelper(this);
				return helper;
			}
		}
		public override GridDesignerHelperBase BaseHelper { get { return Helper; } }
		protected override TemplateGroupCollection TemplateGroups { get { return Helper.TemplateGroups; } }
		public override ASPxWebControlDesignerActionList ActionList { get { return new GridViewDesignerActionList(GridView, this); } }
		ASPxGridView GridView { get { return (ASPxGridView)Grid; } }
		public override void RunDesigner() {
			var autoGenerate = Grid.AutoGenerateColumns;
			ShowDialog(new GridViewDesignerEditForm(GridView));
			if(Grid.AutoGenerateColumns != autoGenerate)
				FireControlPropertyChanged("Columns");
		}
	}
	public class GridViewDesignerActionList : GridViewDesignerActionListBase {
		public GridViewDesignerActionList(ASPxGridView gridView, ASPxWebControlDesigner designer)
			: base(designer) {
			GridView = gridView;
			Settings = GridView.Settings;
		}
		ASPxGridView GridView { get; set; }
		protected override ASPxGridBase Grid { get { return GridView; } }
		protected ASPxGridViewSettings Settings { get; set; }
		public override DesignerActionItemCollection GetSortedActionItems() {
			var result = base.GetSortedActionItems();
			var actionItems = result.OfType<DesignerActionItem>().ToList();
			InsertActionItemBefore(actionItems, StringResources.GridViewActionList_ShowSearchPanel,
				new DesignerActionPropertyItem("ShowGroupPanel",
					StringResources.GridViewActionList_ShowGroupPanel,
					StringResources.GridViewActionList_ChecksCategory,
					StringResources.GridViewActionList_ShowGroupPanelDescription));
			InsertActionItemBefore(actionItems, StringResources.GridViewActionList_ShowSelectCheckBox,
				new DesignerActionPropertyItem("ShowFilterRow",
					StringResources.GridViewActionList_ShowFilterRow,
					StringResources.GridViewActionList_ChecksCategory,
					StringResources.GridViewActionList_ShowFilterRowDescription));
			result.Clear();
			actionItems.ForEach(i => result.Add(i));
			return result;
		}
		public bool ShowFilterRow {
			get { return Settings.ShowFilterRow; }
			set {
				if(value) {
					GetOrCreateCommandColumn().ShowClearFilterButton = true;
				} else {
					var column = FirstCommandColumn;
					if(column != null)
						column.ShowClearFilterButton = false;
				}
				Settings.ShowFilterRow = value;
				Designer.FireControlPropertyChanged("Settings");
			}
		}
		public bool ShowGroupPanel {
			get { return Settings.ShowGroupPanel; }
			set {
				Settings.ShowGroupPanel = value;
				Designer.FireControlPropertyChanged("Settings");
			}
		}
		public override bool ShowSelectCheckBox {
			get {
				var commandColumn = FirstCommandColumn;
				return commandColumn != null && commandColumn.ShowSelectCheckbox;
			}
			set {
				GridViewCommandColumn column = GetOrCreateCommandColumn();
				column.ShowSelectCheckbox = value;
				column.SelectAllCheckboxMode = value ? GridViewSelectAllCheckBoxMode.Page : GridViewSelectAllCheckBoxMode.None;
				Designer.FireControlPropertyChanged("Columns");
			}
		}
		public override bool ShowDeleteButton {
			get {
				var commandColumn = FirstCommandColumn;
				return commandColumn != null && commandColumn.ShowDeleteButton;
			}
			set {
				GetOrCreateCommandColumn().ShowDeleteButton = value;
				Designer.FireControlPropertyChanged("Columns");
			}
		}
		public override bool ShowEditButton {
			get {
				var commandColumn = FirstCommandColumn;
				return commandColumn != null && commandColumn.ShowEditButton;
			}
			set {
				GetOrCreateCommandColumn().ShowEditButton = value;
				Designer.FireControlPropertyChanged("Columns");
			}
		}
		public override bool ShowNewButton {
			get {
				var commandColumn = FirstCommandColumn;
				return commandColumn != null && commandColumn.ShowNewButtonInHeader;
			}
			set {
				GetOrCreateCommandColumn().ShowNewButtonInHeader = value;
				Designer.FireControlPropertyChanged("Columns");
			}
		}
		protected GridViewCommandColumn FirstCommandColumn { get { return Grid.ColumnHelper.AllColumns.OfType<GridViewCommandColumn>().FirstOrDefault(); } }
		protected GridViewCommandColumn GetOrCreateCommandColumn() {
			GridViewCommandColumn res = FirstCommandColumn;
			if(res == null)
				res = CreateCommandColumn();
			return res;
		}
		protected GridViewCommandColumn CreateCommandColumn() {
			GridViewCommandColumn col = new GridViewCommandColumn();
			Grid.Columns.Insert(0, col);
			col.VisibleIndex = 0;
			return col;
		}
		void InsertActionItemBefore(List<DesignerActionItem> items, string insertBeforeDisplayName, DesignerActionPropertyItem insertItem) {
			var item = items.FirstOrDefault(i => i.DisplayName == insertBeforeDisplayName);
			if(item == null) {
				items.Add(insertItem);
				return;
			}
			var index = items.IndexOf(item) - 1;
			if(index <= 0) {
				items.Add(insertItem);
				return;
			}
			items.Insert(index, insertItem);
		}
	}
	public class GridViewDesignerHelper : GridDesignerHelperBase {
		public GridViewDesignerHelper(ASPxDataWebControlDesigner designer)
			: base(designer) {
		}
		protected override PropertiesBase GridTemplates { get { return ((ASPxGridView)GridBase).Templates; } }
		protected override string GridPlaceholderName { get { return "GridView"; } }
		protected override List<string> GetControlTemplateNames() {
			var templates = base.GetControlTemplateNames();
			templates.Add("HeaderCaption");
			templates.Add("FilterRow");
			templates.Add("FilterRow");
			templates.Add("DataRow");
			templates.Add("GroupRow");
			templates.Add("GroupRowContent");
			templates.Add("DetailRow");
			templates.Add("PreviewRow");
			templates.Add("EmptyDataRow");
			templates.Add("FooterRow");
			templates.Add("FooterCell");
			templates.Add("GroupFooterRow");
			templates.Add("GroupFooterCell");
			return templates;
		}
		protected override List<string> GetColumnTemplateNames() {
			var templates = base.GetColumnTemplateNames();
			templates.Add("HeaderCaptionTemplate");
			templates.Add("!FilterTemplate");
			templates.Add("!FooterTemplate");
			templates.Add("$GroupRowTemplate");
			templates.Add("!GroupFooterTemplate");
			return templates;
		}
		protected override IWebGridDataColumn CreateDataColumnCore(Type dataType) {
			return GridViewEditDataColumn.CreateColumn(dataType);
		}
		protected override void ProcessIdentityColumn(IWebGridDataColumn column) {
			((GridViewEditDataColumn)column).EditFormSettings.Visible = DefaultBoolean.False;
		}
	}
}
