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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using DevExpress.Utils.Controls;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.ASPxPivotGrid.Html;
using DevExpress.Web.Internal;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
namespace DevExpress.Web.ASPxPivotGrid.HtmlControls {
	public abstract class PivotGridHtmlFilterPopupContentBase : ASPxInternalWebControl {
		string callbackContent;
		object callbackPartialContent;
		PivotGridField field;
		Table table;
		PivotFilterItemsBase filterItems;
		PivotGridWebData data;
		protected PivotGridHtmlFilterPopupContentBase(PivotGridWebData data, PivotGridField field, bool deferUpdates) {
			this.callbackContent = string.Empty;
			this.callbackPartialContent = null;
			this.data = data;
			this.field = field;
			if(field == null)
				throw new ArgumentNullException("PopupContent.Initialize: field == null");
			this.filterItems = Data.CreatePivotGridFilterItems(this.field, deferUpdates);
		}
		public abstract bool IsGroupFilter { get; }
		public abstract void PrepareCallbackResultObject(string callbackArgs);
		protected PivotGridWebData Data { get { return data; } }
		public new void EnsureChildControls() {
			base.EnsureChildControls();
		}
		public object GetCallbackResult() {
			return new object[] { GetCallBackString(), CallbackPartialContent };
		}
		protected internal string GetCallBackString() {
			StringBuilder res = new StringBuilder();
			res.Append("F|");
			res.Append(FilterItems.VisibleStatesString).Append("|")
				.Append(FilterItems.PersistentString).Append("|")
				.Append(Field.Index).Append("|").Append(FilterItems.DeferUpdates ? "D" : "N").Append("|").Append(CallbackContent);
			return res.ToString();
		}
		protected PivotGridField Field { get { return field; } }
		protected PivotFilterItemsBase FilterItems { get { return filterItems; } }
		protected ASPxPivotGrid PivotGrid { get { return Data.Owner as ASPxPivotGrid; } }
		protected override void ClearControlFields() {
			this.table = null;
		}
		protected override void CreateControlHierarchy() {
			this.table = RenderUtils.CreateTable();
			Controls.Add(this.table);
			CreateContent();
		}
		protected string CallbackContent {
			get { return callbackContent; }
			set { callbackContent = value; }
		}
		protected object CallbackPartialContent {
			get { return callbackPartialContent; }
			set { callbackPartialContent = value; }
		}
		protected abstract void CreateContent();		 
		protected TableRow AddTableRow() {
			TableRow row = RenderUtils.CreateTableRow();
			this.table.Rows.Add(row);
			return row;
		}
	}
	public class PivotGridHtmlFilterPopupContent : PivotGridHtmlFilterPopupContentBase {
		public PivotGridHtmlFilterPopupContent(PivotGridWebData data, PivotGridField field, bool deferUpdates)
			: base(data, field, deferUpdates) {
		}
		protected bool? IsShowAllChecked() {
			return FilterItems.VisibleCheckState;
		}
		public override bool IsGroupFilter { get { return false; } }
		protected bool ShowHiddenItems { get { return Data.OptionsFilter.ShowHiddenItems; } }
		public override void PrepareCallbackResultObject(string callbackArgs) {
			CallbackContent = RenderUtils.GetRenderResult(this);
		}
		protected override void CreateContent() {
			if(FilterItems.Count == 0)
				FilterItems.CreateItems();
			else
				FilterItems.EnsureAvailableItems();
			int visibleIndex = 0;
			CheckBoxFactory factory = new CheckBoxFactory(PivotGrid, PivotGrid.OptionsFilter.NativeCheckBoxes);
			CreateShowAllRow(factory);
			foreach(PivotGridFilterItem filterItem in ShowHiddenItems ? FilterItems : FilterItems.VisibleItems) {
				TableRow row = AddTableRow();
				TableCell cell = RenderUtils.CreateTableCell();
				ICheckBoxWrapper checkBox = CreateCheckBox(factory, visibleIndex, filterItem);
				cell.Controls.Add(checkBox.Control);
				Data.GetFilterItemStyle().AssignToControl(cell);
				row.Cells.Add(cell);
				if(filterItem.IsVisible)
					visibleIndex++;
			}
		}
		void CreateShowAllRow(CheckBoxFactory factory) {
			TableRow row = AddTableRow();
			TableCell cell = RenderUtils.CreateTableCell();
			ICheckBoxWrapper checkBox = factory.CreateShowAllCheckBox();
			checkBox.SetChecked(IsShowAllChecked());
			checkBox.AddValueChangedHandler(string.Format("ASPx.pivotGrid_FieldFilterValueChanged('{0}', {1});", PivotGrid.ClientID, -1));
			checkBox.ID = "FTRIAll";
			checkBox.Text = PivotGridLocalizer.GetString(PivotGridStringId.FilterShowAll);
			cell.Controls.Add(checkBox.Control);
			Data.GetFilterItemStyle().AssignToControl(cell);
			row.Cells.Add(cell);
		}
		ICheckBoxWrapper CreateCheckBox(CheckBoxFactory factory, int visibleIndex, PivotGridFilterItem filterItem) {
			ICheckBoxWrapper checkBox = factory.CreateCheckBox();
			checkBox.SetEnabled(filterItem.IsVisible);
			checkBox.SetChecked(filterItem.IsChecked == true);
			checkBox.AddValueChangedHandler(string.Format("ASPx.pivotGrid_FieldFilterValueChanged('{0}', {1});", PivotGrid.ClientID, visibleIndex));
			if(filterItem.IsVisible)
				checkBox.ID = "FTRI" + visibleIndex.ToString();
			checkBox.Text = PivotGrid.HtmlEncode(filterItem.Text).Replace("|", "&#124;");
			return checkBox;
		}
	}
	public class PivotGridHtmlGroupFilterPopupContent : PivotGridHtmlFilterPopupContentBase {
		ASPxPivotGroupFilterTree treeView;
		bool requireCreateTreeChildren;
		bool isGroupCallback;
		public PivotGridHtmlGroupFilterPopupContent(PivotGridWebData data, PivotGridField field,
			string filterItemsState, string filterValues, bool deferUpdates, bool isGroupCallback)
			: base(data, field, deferUpdates) {
			this.isGroupCallback = isGroupCallback;
			this.requireCreateTreeChildren = false;
			if(!string.IsNullOrEmpty(filterItemsState))
				FilterItems.Initialize(filterItemsState, filterValues);
		}
		public override bool IsGroupFilter { get { return true; } }
		public override void PrepareCallbackResultObject(string callbackArgs) {
			requireCreateTreeChildren = true;
			RenderUtils.LoadPostDataRecursive(this, System.Web.HttpContext.Current.Request.Params);
			if(string.IsNullOrEmpty(callbackArgs)) {
				CallbackContent = RenderUtils.GetRenderResult(this);
			} else {
				CallbackPartialContent = this.treeView.GetCallbackResult(callbackArgs);
			}
		}
		protected internal ASPxPivotGroupFilterTree TreeView { get { return treeView; } }
		protected override void CreateContent() {
			TableRow row = AddTableRow();
			TableCell cell = RenderUtils.CreateTableCell();
			row.Cells.Add(cell);
			this.treeView = CreateTreeView();
			cell.Controls.Add(this.treeView);
		}
		protected virtual ASPxPivotGroupFilterTree CreateTreeView() {
			ASPxPivotGroupFilterTree treeView = new ASPxPivotGroupFilterTree(this.PivotGrid, isGroupCallback);
			treeView.VirtualModeCreateChildren += new TreeViewVirtualModeCreateChildrenEventHandler(OnRetrieveChildren);
			return treeView;
		}
		protected void OnRetrieveChildren(object sender, TreeViewVirtualModeCreateChildrenEventArgs e) {
			if(!requireCreateTreeChildren) return;
			if(sender as ASPxPivotGroupFilterTree == null || this.treeView == null)
				throw new ArgumentException("ASPxPivotGroupFilterTree: OnRetrieveChildren");
			PivotGrid.EnsureRefreshData();
			if(FilterItems.Count == 0)
			FilterItems.CreateItems();
			string nodeName = e.NodeName;
			IList items = FilterItems;
			bool isLastLevel = FilterItems.Field != null && FilterItems.Field.Group != null && FilterItems.Field.Group.Count == 1;
			if(!ASPxPivotGroupFilterTree.IsRoot(nodeName)) {
				object[] branch = ASPxPivotGroupFilterTree.GetBranch(nodeName, FilterItems);
				isLastLevel = (branch.Length == FilterItems.LevelCount - 1);
				items = ((PivotGroupFilterItems)FilterItems).GetChildValues(branch) ?? FilterItems.LoadValues(branch);
			}
			e.Children = ASPxPivotGroupFilterTree.GetNodes(items, nodeName, isLastLevel);
		}
	}
	public class ASPxPivotGroupFilterTree : ASPxTreeView {
		bool canLoadPostData;
		protected const char BranchSeparator = '_';
		protected static string IndexToString(int index) {
			return Convert.ToString(index);
		}
		protected override bool LoadPostData(System.Collections.Specialized.NameValueCollection postCollection) {
			if(!canLoadPostData) {
				EnsureDataBound();
				ResetViewStateStoringFlag();
				return false;
			}
			return base.LoadPostData(postCollection);
		}
		protected static string GenerateNodeName(string parentNodeName, int index) {
			string name = IndexToString(index);
			if(!string.IsNullOrEmpty(parentNodeName))
				name = parentNodeName + BranchSeparator + name;
			return name;
		}
		public static bool IsRoot(string nodeName) {
			return string.IsNullOrEmpty(nodeName);
		}
		public static object[] GetBranch(string nodeName, 
				PivotFilterItemsBase filterItems) {
			string[] branchStrings = nodeName.Split(BranchSeparator);
			int levelCount = branchStrings.Length;
			if(levelCount == 0) return null;
			object[] branch = new object[levelCount];
			int branchItemIndex = 0, leafIndex = 0;
			foreach(PivotGridFilterItem item in filterItems) {
				if(item.Level < branchItemIndex)
					throw new ArgumentException("ASPxPivotGroupFilterTree: branch not found");
				if(item.Level != branchItemIndex) continue;
				if(branchStrings[branchItemIndex] == IndexToString(leafIndex)) {
					branch[branchItemIndex] = item;
					branchItemIndex++;
					leafIndex = 0;
				} else {
					leafIndex++;
				}
				if(branchItemIndex == levelCount) break;
			}
			return branch;
		}
		public static List<TreeViewVirtualNode> GetNodes(IList items, string parentNodeName, bool isLastLevel) {
			int count = items.Count;
			List<TreeViewVirtualNode> nodes = new List<TreeViewVirtualNode>(count + 1);
			IFilterItems filterItems = items as IFilterItems;
			if(filterItems != null) {
				nodes.Add(new ShowAllGroupFilterNode(filterItems.CheckState));
				List<PivotGridFilterItem> list = new List<PivotGridFilterItem>();
				list.AddRange(((PivotGroupFilterItems)filterItems).VisibleItems);
				items = list;
				count = list.Count;
			}
			for(int i = 0; i < count; i++) {
				PivotGridFilterItem item = (PivotGridFilterItem)items[i];
				string childNodeName = GenerateNodeName(parentNodeName, i);
				GroupFilterNode node = new GroupFilterNode(childNodeName, item.ToString(), item.IsChecked, isLastLevel);
				nodes.Add(node);
			}
			return nodes;
		}
		public ASPxPivotGroupFilterTree(ASPxPivotGrid owner, bool canLoadPostData)
			: base(owner) {
			this.canLoadPostData = canLoadPostData;
			Initialize();
			this.EnableCallBacks = true;
			this.ParentSkinOwner = owner as ISkinOwner;
			this.Images.NodeLoadingPanel.Assign(owner.RenderHelper.GetTreeViewNodeLoadingPanelImage());
		}
		public new void EnsureChildControls() {
			base.EnsureChildControls();
		}
		public new void ResetControlHierarchy() {
			base.ResetControlHierarchy();
		}
		protected void Initialize() {
			this.ID = "treeGFTR";
			this.AllowCheckNodes = true;
			this.AllowSelectNode = true;
			this.CheckNodesRecursive = true;
			this.SyncSelectionMode = SyncSelectionMode.None;
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.PivotGroupFilterTree";
		}
		protected internal object GetCallbackResult(string callbackArgs) {
			RaiseCallbackEvent(callbackArgs);
			return GetCallbackResult();
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(this.GetType(), PivotGridWebData.GroupFilterScriptResourceName);
		}
	}
	public class GroupFilterNode : TreeViewVirtualNode {
		protected static CheckState GetCheckState(bool? isChecked) {
			if(isChecked == null) return CheckState.Indeterminate;
			return isChecked == true ? CheckState.Checked : CheckState.Unchecked;
		}
		public GroupFilterNode(string nodeName, string text, bool? isChecked, bool isLeaf)
			: base(nodeName, text) {
			this.IsLeaf = isLeaf;
			this.SetCheckState(GetCheckState(isChecked));
		}
	}
	public sealed class ShowAllGroupFilterNode : GroupFilterNode {
		public ShowAllGroupFilterNode(bool? isChecked)
			: base("All", PivotGridLocalizer.GetString(PivotGridStringId.FilterShowAll), isChecked, true) {
		}
	}
}
