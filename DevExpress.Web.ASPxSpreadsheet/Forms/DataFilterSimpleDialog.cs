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

using System.Web.UI.WebControls;
using System.Web.UI;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxSpreadsheet.Localization;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	public class DataFilterSimpleDialog : SpreadsheetDialogBase {
		protected const string ValuesListBoxId = "lbxValues";
		protected const string UncheckButtonClassName = "dxssUncheckButton";
		protected ASPxStatelessTreeView ValuesTreeView { get; private set; }
		protected ASPxButton CheckAllButton { get; private set; }
		protected ASPxButton UnCheckAllButton { get; private set; }
		protected override string GetDialogCssClassName() {
			return SpreadsheetStyles.DataFilterSimpleDialogCssClass;
		}
		protected override string GetContentTableID() {
			return "dxDataFilterSimpleForm";
		}
		protected override void PopulateContentArea(Control container) {
			ValuesTreeView = new ASPxStatelessTreeView();
			container.Controls.Add(ValuesTreeView);
			ValuesTreeView.ID = "twValues";
			ValuesTreeView.ClientInstanceName = GetControlClientInstanceName("_twValues");
			ValuesTreeView.Width = Unit.Percentage(100);
			ValuesTreeView.Height = Unit.Pixel(300);
			ValuesTreeView.AllowCheckNodes = true;
			ValuesTreeView.AutoPostBack = false;
			ValuesTreeView.EnableViewState = false;
			ValuesTreeView.ViewStateMode = ViewStateMode.Disabled;
			ValuesTreeView.EnableCallBacks = false;
			ValuesTreeView.CheckNodesRecursive = true;
			ValuesTreeView.ClientSideEvents.NodeClick = GetNodeClickClientHandler();
			ValuesTreeView.ClientSideEvents.CheckedChanged = GetCheckedChangedClientHandler();
			BindValuesTreeList();
			ValuesTreeView.ParentStyles = Spreadsheet.StylesEditors;
		}
		string GetNodeClickClientHandler() {
			return @"
                function(s, e) {{ 
                    e.node.SetChecked(!e.node.GetChecked());
                    ASPx.SpreadsheetDialog.ValuesTreeViewCheckedChanged();
                }}";
		}
		string GetCheckedChangedClientHandler() {
			return @"
                function(s, e) {{ 
                    ASPx.SpreadsheetDialog.ValuesTreeViewCheckedChanged(); 
                }}";
		}
		protected void BindValuesTreeList() {
			var webSpreadsheetControl = Spreadsheet.GetCurrentWorkSessions().WebSpreadsheetControl;
			var innerControl = webSpreadsheetControl.InnerControl;
			var command = innerControl.CreateCommand(SpreadsheetCommandId.DataFilterSimple);
			SimpleFilterViewModel vm = new SimpleFilterViewModel(webSpreadsheetControl);
			var accessor = new DataSortOrFilterAccessor(innerControl.DocumentModel);
			CellRange range = accessor.GetSortOrFilterRange();
			AutoFilterBase autoFilter = accessor.Filter;
			if(autoFilter != null) {
				AutoFilterColumn filterColumn = GetFilterColumn(autoFilter, range, innerControl);
				vm.SetupViewModel(autoFilter, filterColumn);
			}
			AddNodes(ValuesTreeView.RootNode, vm.Root.Children);
		}
		protected void AddNodes(TreeViewNode parentNode, List<FilterValueNode> nodes) {
			foreach(var valueNode in nodes) {
				TreeViewNode node = new TreeViewNode(valueNode.Text, valueNode.Id.ToString());
				parentNode.Nodes.Add(node);
				node.Expanded = valueNode.DateTimeGrouping < DateTimeGroupingType.Day;
				node.Checked = valueNode.IsChecked;
				AddNodes(node, valueNode.Children);
			}
		}
		protected AutoFilterColumn GetFilterColumn(AutoFilterBase autoFilter, CellRange range,
			XtraSpreadsheet.Internal.InnerSpreadsheetControl innerControl) {
				int columnId = innerControl.DocumentModel.ActiveSheet.Selection.ActiveCell.Column - range.TopLeft.Column;
				return autoFilter.FilterColumns[columnId];
		}
		protected override void PopulateFooterArea(Control container) {
			CheckAllButton = new ASPxButton();
			container.Controls.Add(CheckAllButton);
			CheckAllButton.CssClass = SpreadsheetStyles.DialogFooterButtonCssClass;
			CheckAllButton.ID = "btnCheckAll";
			CheckAllButton.ClientInstanceName = GetControlClientInstanceName("_btnCheckAll");
			CheckAllButton.AutoPostBack = false;
			UnCheckAllButton = new ASPxButton();
			container.Controls.Add(UnCheckAllButton);
			UnCheckAllButton.CssClass = SpreadsheetStyles.DialogFooterButtonCssClass;
			UnCheckAllButton.ID = "btnUncheckAll";
			UnCheckAllButton.ClientInstanceName = GetControlClientInstanceName("_btnUnCheckAll");
			UnCheckAllButton.AutoPostBack = false;
			RenderUtils.AppendDefaultDXClassName(UnCheckAllButton, UncheckButtonClassName);
			base.PopulateFooterArea(container);
		}
		protected override ASPxButton[] GetChildDxButtons() {
			return new ASPxButton[] { CheckAllButton, UnCheckAllButton };
		}
		protected override void ApplyLocalization() {
			base.ApplyLocalization();
			CheckAllButton.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.FilterSimple_CheckAll);
			UnCheckAllButton.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.FilterSimple_UncheckAll);
		}
		protected class ASPxStatelessTreeView : ASPxTreeView {
			public ASPxStatelessTreeView()
				: base() { }
			protected override bool LoadPostData(System.Collections.Specialized.NameValueCollection postCollection) {
				return false;
			}
		}
	}
}
