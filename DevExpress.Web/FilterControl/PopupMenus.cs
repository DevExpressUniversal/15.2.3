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
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Linq;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Web.Localization;
using DevExpress.Web.Internal;
using DevExpress.XtraEditors.Filtering;
using DevExpress.Data.Filtering;
using DevExpress.Data;
using System.Web.UI;
namespace DevExpress.Web.FilterControl {
	[ToolboxItem(false)]
	public abstract class FilterControlPopupMenu : ASPxPopupMenu {
		WebFilterControlRenderHelper renderHelper;
		public FilterControlPopupMenu(WebFilterControlRenderHelper renderHelper)  : base(renderHelper.ControlOwner) {
			this.renderHelper = renderHelper;
			EnableViewState = false;
			EnableScrolling = RenderHelper.EnablePopupMenuScrolling;
			PopupVerticalAlign = PopupVerticalAlign.Below;
			ParentSkinOwner = RenderHelper.ControlOwner as ISkinOwner;
		}
		protected abstract string GetItemClick();
		public virtual void PrepareImages() { }
		protected new WebFilterControlRenderHelper RenderHelper { get { return renderHelper; } }
		protected DevExpress.Web.MenuItem AddMenuItem(string id, string displayText) {
			return AddMenuItem(id, displayText, false);
		}
		protected DevExpress.Web.MenuItem AddMenuItem(string id, string displayText, bool beginGroup) {
			return AddMenuItem(id, displayText, beginGroup, Items);
		}
		protected MenuItem AddMenuItem(string id, string displayText, bool beginGroup, MenuItemCollection items) {
			DevExpress.Web.MenuItem item = new DevExpress.Web.MenuItem(displayText, id);
			item.BeginGroup = beginGroup;
			items.Add(item);
			return item;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(RenderHelper.Enabled)
				ClientSideEvents.ItemClick = GetItemClick();
		}
	}
	public class FilterControlPropertiesPopupTreeView : ASPxPopupControl {
		const string ElbowHiddenCssClassName = "dxtv-elbHide";
		public FilterControlPropertiesPopupTreeView(WebFilterControlRenderHelper renderHelper) : base(renderHelper.ControlOwner) {
			ID = WebFilterControlRenderHelper.PopupTreeViewFieldNameID;
			RenderHelper = renderHelper;
			ShowHeader = false;
			PopupVerticalAlign = Web.PopupVerticalAlign.Below;
			Views = new Dictionary<string, ASPxTreeView>();
		}
		WebFilterControlRenderHelper RenderHelper { get; set; }
		Dictionary<string, ASPxTreeView> Views { get; set; }
		protected override void ClearControlFields() {
			base.ClearControlFields();
			Views.Clear();
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			var views = Views.Select(view => new { key = view.Key, treeViewId = view.Value.ClientID });
			stb.AppendFormat("{0}.views = {1};\n", localVarName, HtmlConvertor.ToJSON(views));
		}
		protected override void CreateControlHierarchy() {
			Controls.Clear(); 
			base.CreateControlHierarchy();
			var treeView = new ASPxTreeView(this);
			treeView.ID = GenerateTreeViewId();
			Views.Add(string.Empty, treeView);
			var owner = OwnerControl as ASPxFilterControlBase;
			foreach(FilterControlColumn column in owner.Columns)
				AddNode(treeView.Nodes, column);
			if(RenderHelper.Enabled)
				foreach(var view in Views.Values) {
					var displayAsTree = view.Nodes.Any(node => node.Nodes.Count > 0);
					if(!displayAsTree) { 
						view.ShowTreeLines = false;
						view.Styles.Elbow.CssClass = ElbowHiddenCssClassName;
					}
					view.ClientSideEvents.NodeClick = RenderHelper.GetScriptForPopupMenuFieldNameOnItemClick();
					view.ParentSkinOwner = OwnerControl.ParentSkinOwner ?? OwnerControl;
					view.AllowSelectNode = true;
					Controls.Add(view);
				}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			ClientSideEvents.CloseUp = RenderHelper.GetScriptForPopupTreeViewCloseUp();
		}
		void AddNode(TreeViewNodeCollection nodes, FilterControlColumn column, FilterControlColumn parent = null) {
			var node = nodes.Add((column as IFilterColumn).DisplayName, column.GetFullName());
			var complexTypeColumn = column as FilterControlComplexTypeColumn;
			if(complexTypeColumn == null) return;
			TreeViewNodeCollection nodeCollection = GetNodeCollection(node, complexTypeColumn);
			if(nodeCollection == null)
				return;
			foreach(FilterControlColumn col in complexTypeColumn.Columns)
				AddNode(nodeCollection, col, complexTypeColumn);
		}
		TreeViewNodeCollection GetNodeCollection(TreeViewNode node, FilterControlComplexTypeColumn complexTypeColumn) {
			if(!(complexTypeColumn as IBoundProperty).IsList)
				return node.Nodes;
			if(!ViewExists(complexTypeColumn.ListPropertyType) && complexTypeColumn.Columns.Count > 0) {
				var treeView = CreateView(complexTypeColumn.ListPropertyType);
				return treeView.Nodes;
			}
			return null;
		}
		bool ViewExists(string name) {
			return Views.Keys.Any(key => key == name);
		}
		ASPxTreeView CreateView(string name) {
			var treeView = new ASPxTreeView(this);
			treeView.ID = GenerateTreeViewId();
			treeView.ClientVisible = false;
			Views.Add(name, treeView);
			return treeView;
		}
		string GenerateTreeViewId() {
			return "View" + Views.Count;
		}
	}
	public class FilterControlPropertiesPopupMenu : FilterControlPopupMenu {
		public FilterControlPropertiesPopupMenu(WebFilterControlRenderHelper renderHelper) : base(renderHelper) {
			GutterWidth = Unit.Pixel(0);
			ID = WebFilterControlRenderHelper.PopupMenuFieldNameID;
			for(int i = 0; i < RenderHelper.ColumnCount; i++) {
				IFilterColumn column = RenderHelper.GetColumn(i);
				AddMenuItem(column.PropertyName, column.DisplayName);
			}
		}
		protected override string GetItemClick() { return RenderHelper.GetScriptForPopupMenuFieldNameOnItemClick(); }
	}
	public class FilterControlOperationPopupMenu : FilterControlPopupMenu {
		public static IEnumerable<ClauseType> SupportedClauses;
		static FilterControlOperationPopupMenu() {
			SupportedClauses = new ClauseType[] {
				ClauseType.Equals, ClauseType.DoesNotEqual,
				ClauseType.Greater, ClauseType.GreaterOrEqual,
				ClauseType.Less, ClauseType.LessOrEqual,
				ClauseType.Between, ClauseType.NotBetween,
				ClauseType.Contains, ClauseType.DoesNotContain,
				ClauseType.BeginsWith, ClauseType.EndsWith,
				ClauseType.Like, ClauseType.NotLike,
				ClauseType.IsNull, ClauseType.IsNotNull,
				ClauseType.AnyOf, ClauseType.NoneOf,
				ClauseType.IsBeyondThisYear,
				ClauseType.IsLaterThisYear,
				ClauseType.IsLaterThisMonth,
				ClauseType.IsNextWeek,
				ClauseType.IsLaterThisWeek,
				ClauseType.IsTomorrow,
				ClauseType.IsToday,
				ClauseType.IsYesterday,
				ClauseType.IsEarlierThisWeek,
				ClauseType.IsLastWeek,
				ClauseType.IsEarlierThisMonth,
				ClauseType.IsEarlierThisYear,
				ClauseType.IsPriorThisYear
			};
		}
		public FilterControlOperationPopupMenu(WebFilterControlRenderHelper renderHelper)
			: base(renderHelper) {
			ID = WebFilterControlRenderHelper.PopupMenuOperationID;
			var dateTimeOperatorMenu = AddMenuItem("D|", RenderHelper.GetLocalizedString(ASPxEditorsStringId.FilterControl_DateTimeOperatorMenuCaption));
			foreach(ClauseType operation in SupportedClauses) {
				var items = IsDateTimeOperatorClause(operation) ? dateTimeOperatorMenu.Items : Items;
				AddMenuItem(GetMenuItemId(operation), renderHelper.GetTextForOperation(operation), false, items);
			}
			Items.Add(dateTimeOperatorMenu);
		}
		protected override string GetItemClick() { return RenderHelper.GetScriptForPopupMenuOperationOnItemClick(); }
		public override void PrepareImages() {
			var clausesWithImages = SupportedClauses.Where(clause => !IsDateTimeOperatorClause(clause));
			foreach(ClauseType operation in clausesWithImages) {
				Items[(int)operation].Image.CopyFrom(RenderHelper.GetImageProperties(FilterControlImages.GetOperationName(operation)));
			}
		}
		protected string GetMenuItemId(ClauseType operation) {
			return GetOperationUsing(operation) + operation.ToString();
		}
		protected string GetOperationUsing(ClauseType operation) {
			string res = "";
			foreach(FilterColumnClauseClass clauseClass in Enum.GetValues(typeof(FilterColumnClauseClass))) {
				if (RenderHelper.Model.IsValidClause(operation, clauseClass)) {
					res += clauseClass.ToString()[0];
				}
			}
			return res + WebFilterControlRenderHelper.JSDivideChar; 
		}
		bool IsDateTimeOperatorClause(ClauseType type) {
			return type >= ClauseType.IsBeyondThisYear;
		}
	}
	public enum FilterColumnAggregateClass {
		Common,
		MaxMin,
		SumAvg
	}
	public class FilterControlAggregatePopupMenu : FilterControlPopupMenu {
		public static IEnumerable<Aggregate> SupportedAggregates;
		static FilterControlAggregatePopupMenu() {
			SupportedAggregates = new Aggregate[] {
				Aggregate.Exists, Aggregate.Count, Aggregate.Max, Aggregate.Min, Aggregate.Avg, Aggregate.Sum, 
			};
		}
		public FilterControlAggregatePopupMenu(WebFilterControlRenderHelper renderHelper)
			: base(renderHelper) {
			ID = WebFilterControlRenderHelper.PopupMenuAggregateID;
			foreach(Aggregate aggregate in SupportedAggregates) {
				AddMenuItem(GetMenuItemId(aggregate), renderHelper.GetTextForAggregate(aggregate));
			}
		}
		public override void PrepareImages() {
			foreach(Aggregate aggregate in SupportedAggregates) {
				Items[(int)aggregate].Image.CopyFrom(RenderHelper.GetImageProperties(FilterControlImages.GetAggregateName(aggregate)));
			}
		}
		protected override string GetItemClick() { 
			return RenderHelper.GetScriptForPopupMenuAggregateOnItemClick(); 
		}
		protected string GetMenuItemId(Aggregate aggregate) {
			return GetOperationUsing(aggregate) + aggregate.ToString(); 
		}
		protected string GetOperationUsing(Aggregate aggregate) {
			string res = "";
			foreach(FilterColumnAggregateClass aggregateClass in Enum.GetValues(typeof(FilterColumnAggregateClass))) {
				if(IsValidAggregate(aggregate, aggregateClass)) {
					res += aggregateClass.ToString()[0];
				}
			}
			return res + WebFilterControlRenderHelper.JSDivideChar;
		}
		bool IsValidAggregate(Aggregate aggregate, FilterColumnAggregateClass aggregateClass) {
			switch(aggregate) {
				case Aggregate.Exists:
				case Aggregate.Count:
				return true;
				case Aggregate.Max:
				case Aggregate.Min:
				return aggregateClass != FilterColumnAggregateClass.Common;
				case Aggregate.Sum:
				case Aggregate.Avg:
				return aggregateClass == FilterColumnAggregateClass.SumAvg;
			}
			return false;
		}
	}
	public class FilterControlGroupPopupMenu : FilterControlPopupMenu {
		DevExpress.Web.MenuItem addGroupMenuItem;
		DevExpress.Web.MenuItem addCondtionMenuItem;
		DevExpress.Web.MenuItem removeGroupMenuItem;
		Dictionary<GroupType, MenuItem> itemsByGroups = new Dictionary<GroupType, MenuItem>();
		public FilterControlGroupPopupMenu(WebFilterControlRenderHelper renderHelper)
			: base(renderHelper) {
			ID = WebFilterControlRenderHelper.PopupMenuGroupID;
			foreach(GroupType group in Enum.GetValues(typeof(GroupType))) {
				if(IsGroupOperationVisible(group)) { 
					var menuItem = AddMenuItem(group.ToString(), renderHelper.GetTextForGroup(group));
					itemsByGroups.Add(group, menuItem);
				}
			}
			if(GroupOperationsVisibility.AddGroup)
				this.addGroupMenuItem = AddMenuItem("|AddGroup", RenderHelper.GetLocalizedString(ASPxEditorsStringId.FilterControl_AddGroup), true); 
			if(GroupOperationsVisibility.AddCondition)
				this.addCondtionMenuItem = AddMenuItem("|AddCondition", RenderHelper.GetLocalizedString(ASPxEditorsStringId.FilterControl_AddCondition), false);
			if(GroupOperationsVisibility.Remove)
				this.removeGroupMenuItem = AddMenuItem("|Remove", RenderHelper.GetLocalizedString(ASPxEditorsStringId.FilterControl_Remove), true); 
		}
		protected DevExpress.Web.MenuItem AddGroupMenuItem { get { return addGroupMenuItem; } }
		protected DevExpress.Web.MenuItem AddCondtionMenuItem { get { return addCondtionMenuItem; } }
		protected DevExpress.Web.MenuItem RemoveGroupMenuItem { get { return removeGroupMenuItem; } }
		protected override string GetItemClick() { return RenderHelper.GetScriptForPopupMenuGroupOnItemClick(); }
		protected FilterControlGroupOperationsVisibility GroupOperationsVisibility { 
			get { return RenderHelper.FilterOwner.GroupOperationsVisibility; } 
		}
		bool IsGroupOperationVisible(GroupType groupType) {
			switch(groupType) {
				case GroupType.And: return GroupOperationsVisibility.And;
				case GroupType.Or: return GroupOperationsVisibility.Or;
				case GroupType.NotAnd: return GroupOperationsVisibility.NotAnd;
				case GroupType.NotOr: return GroupOperationsVisibility.NotOr;
			}
			return false;
		}
		public override void PrepareImages() {
			foreach(var itemByGroup in itemsByGroups) { 
				if(itemsByGroups.ContainsKey(itemByGroup.Key))
					itemByGroup.Value.Image.CopyFrom(RenderHelper.GetImageProperties(FilterControlImages.GetGroupTypeName(itemByGroup.Key)));
			}
			if(AddGroupMenuItem != null)
				AddGroupMenuItem.Image.CopyFrom(RenderHelper.GetImageProperties(FilterControlImages.AddGroupName));
			if(AddCondtionMenuItem != null)
				AddCondtionMenuItem.Image.CopyFrom(RenderHelper.GetImageProperties(FilterControlImages.AddConditionName));
			if(RemoveGroupMenuItem != null)
				RemoveGroupMenuItem.Image.CopyFrom(RenderHelper.GetImageProperties(FilterControlImages.RemoveGroupName));
		}
	}
}
