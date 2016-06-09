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

using Microsoft.SharePoint.WebPartPages;
using System.Xml.Serialization;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.ComponentModel;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using DevExpress.Web;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using DevExpress.SharePoint.Internal;
using System.Collections;
using System;
using System.Data;
using DevExpress.Web.Internal;
using System.Collections.Generic;
using DevExpress.Web;
using System.Web.UI.WebControls.WebParts;
namespace DevExpress.SharePoint {
	public class SPxListViewToolPart : EditorPart {
		ASPxComboBox listNameComboBox = null;
		CheckBox showToolBarChechBox = null;
		HtmlTable settingsTable = null;
		public SPxListViewToolPart() {
			Title = SPUtility.GetLocalizedString("$Resources:cui_ButListSettings", "core", (uint)SPContext.Current.Web.UICulture.LCID);
		}
		protected ASPxComboBox ListNameComboBox { get { return listNameComboBox; } }
		protected CheckBox ShowToolBarChechBox { get { return showToolBarChechBox; } }
		protected HtmlTable SettingsTable { get { return settingsTable; } }
		protected override void CreateChildControls() {
			CreateSettingsTable();
			InitSettingsTable();
		}
		protected void InitSettingsTable() {
			CreateListNameComboBox();
			InsertGroupControlToSettingsTable(SPUtility.GetLocalizedString("$Resources:cui_TabList", "core", (uint)SPContext.Current.Web.UICulture.LCID),
				ListNameComboBox, true);
			CreateShowToolbarCheckBox();
			InsertSingleControlToSettingsTable(SPUtility.GetLocalizedString("$Resources:propertiesAdvanced_aspx_treeViewChartProperties_Chart_Toolbar", "MossChart", (uint)SPContext.Current.Web.UICulture.LCID),
				ShowToolBarChechBox, false);
		}
		protected void CreateShowToolbarCheckBox() {
			showToolBarChechBox = new CheckBox();
			ShowToolBarChechBox.Text = "&nbsp;&nbsp;" + SPUtility.GetLocalizedString("$Resources:SqlReportView_ShowToolbar_Name", "ppsma.Client", (uint)SPContext.Current.Web.UICulture.LCID);
			ShowToolBarChechBox.Checked = true;
		}
		protected void CreateSettingsTable() {
			settingsTable = new HtmlTable();
			SettingsTable.CellPadding = 0;
			SettingsTable.Width = "100%";
			SettingsTable.CellSpacing = 0;
			Controls.Add(SettingsTable);
		}
		protected void InsertGroupControlToSettingsTable(string groupHeader, WebControl groupBodyControl, bool addDottedLine) {
			HtmlTableRow row = new HtmlTableRow();
			HtmlTableCell cell = new HtmlTableCell();
			cell.Controls.Add(new LiteralControl("<div class='UserSectionHead'>" + groupHeader + "</div>"));
			cell.Controls.Add(new LiteralControl("<div class='UserSectionBody'><div class='UserControlGroup'>"));
			cell.Controls.Add(groupBodyControl);
			cell.Controls.Add(new LiteralControl("</div></div>"));
			if(addDottedLine)
				cell.Controls.Add(new LiteralControl("<div style='width:100%' class='UserDottedLine'></div>"));
			row.Cells.Add(cell);
			SettingsTable.Rows.Add(row);
		}
		protected void InsertSingleControlToSettingsTable(string groupHeader, WebControl singleBodyControl, bool addDottedLine) {
			HtmlTableRow row = new HtmlTableRow();
			HtmlTableCell cell = new HtmlTableCell();
			cell.Controls.Add(new LiteralControl("<div class='UserSectionHead'>" + groupHeader + "</div>"));
			cell.Controls.Add(new LiteralControl("<div class='UserSectionHead'>"));
			cell.Controls.Add(singleBodyControl);
			cell.Controls.Add(new LiteralControl("</div>"));
			if(addDottedLine)
				cell.Controls.Add(new LiteralControl("<div style='width:100%' class='UserDottedLine'></div>"));
			row.Cells.Add(cell);
			SettingsTable.Rows.Add(row);
		}
		protected void CreateListNameComboBox() {
			this.listNameComboBox = new ASPxComboBox();
			ListNameComboBox.Native = true;
			ListNameComboBox.ValueType = typeof(Guid);
			InitListNameCombobox();
		}
		protected void InitListNameCombobox() {
			SPListCollection lists = SPContext.Current.Web.Lists;
			SPList curPageList = SPxListViewUtils.GetCurrentPageList(Context);
			Guid curPageListID = curPageList != null ? curPageList.ID : Guid.Empty;
			for(int i = 0; i < lists.Count; i++)
				if(!lists[i].Hidden)
					ListNameComboBox.Items.Add(lists[i].Title, lists[i].ID);
		}
		public override bool ApplyChanges() {
			EnsureChildControls();
			SPxListViewWebPart parentPart = base.WebPartToEdit as SPxListViewWebPart;
			parentPart.ListID = new Guid(ListNameComboBox.SelectedItem.Value.ToString());
			parentPart.ShowToolbar = ShowToolBarChechBox.Checked;
			return true;
		}
		public override void SyncChanges() {
			EnsureChildControls();
			SPxListViewWebPart parentPart = base.WebPartToEdit as SPxListViewWebPart;
			ListEditItem ls = ListNameComboBox.Items.FindByValue(parentPart.ListID);
			if(ls != null)
				ls.Selected = true;
			ShowToolBarChechBox.Checked = parentPart.ShowToolbar;
		}
	}
	public class SPxListViewViewToolBar : ViewToolBar {
		const string ToolBarTableID = "toolBarTbl";
		const string ViewSelectorID = "RightRptControls";
		bool showViewSelector = true;
		public bool ShowViewSelector {
			get { return showViewSelector; }
			set {
				showViewSelector = value;
				ChildControlsCreated = false;
			}
		}
		protected override void CreateChildControls() {
			base.CreateChildControls();
			Control viewSelector = GetViewSelector();
			if(viewSelector != null)
				viewSelector.Visible = ShowViewSelector;
		}
		protected Control GetViewSelector() {
			Control viewSelector = null;
			Control toolBarTable = TemplateContainer.FindControl(ToolBarTableID);
			if(toolBarTable != null)
				viewSelector = toolBarTable.FindControl(ViewSelectorID);
			return viewSelector;
		}
	}
}
