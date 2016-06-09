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

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxLookupDesigner : ASPxButtonEditDesigner {
		private ASPxGridLookup lookup = null;
		private GridViewDesignerHelper helper = null;
		public GridViewDesignerHelper Helper {
			get {
				helper = helper ?? new GridViewDesignerHelper(this);
				return helper;
			}
		}
		public ASPxGridLookup Lookup {
			get { return lookup; }
		}
		public ASPxGridView Grid {
			get { return Lookup.Properties.GridView; }
		}
		public override void ShowAbout() {
			GridViewAboutDialogHelper.ShowAbout(Component.Site);
		}
		public override void RunDesigner() {
			ShowDialog(new GridLookupDesignerEditForm(Lookup));
		}
		public override void Initialize(IComponent component) {
			this.lookup = (ASPxGridLookup)component;
			base.Initialize(component);
			RegisterTagPrefix(typeof(ASPxGridView));
		}
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection templateGroups = base.CreateTemplateGroups();
			templateGroups.AddRange(Helper.TemplateGroups);
			return templateGroups;
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new DropDownLookupDesignerActionList(this);
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Columns", "Columns");
		}
		protected override void OnSchemaRefreshed() {
			base.OnSchemaRefreshed();
			Helper.OnSchemaRefreshed();
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
			Helper.DataBind(dataControl);
			base.DataBind(dataControl);
		}  
	}
	public class DropDownLookupDesignerActionList : ButtonEditDesignerActionList {
		private GridViewDesignerActionList gridViewActionList = null;
		public DropDownLookupDesignerActionList(ASPxLookupDesigner designer)
			: base(designer) {
		}
		public GridViewDesignerActionList GridViewActionList {
			get {
				if(gridViewActionList == null)
					gridViewActionList = new GridViewDesignerActionList(LookupDesigner.Grid, Designer);
				return gridViewActionList;
			}
		}
		protected ASPxLookupDesigner LookupDesigner { get { return (ASPxLookupDesigner)Designer; } }
		protected ASPxGridView Grid { get { return LookupDesigner.Lookup.Properties.GridView; } }
		protected DesignerActionItemCollection CleanActionList(DesignerActionItemCollection actionItems) {
			var prohibitedActionItemNames = new List<string>(new string[] { 
				StringResources.GridViewActionList_ShowPager,
				StringResources.GridViewActionList_ShowSearchPanel,
				StringResources.GridViewActionList_ShowGroupPanel,
				StringResources.GridViewActionList_ShowFilterRow,
				StringResources.GridViewActionList_ShowSelectCheckBox,
				StringResources.GridViewActionList_ShowEditButton,
				StringResources.GridViewActionList_ShowNewButton,
				StringResources.GridViewActionList_ShowDeleteButton,
				StringResources.DataEditingActionList_AllowEdit,
				StringResources.DataEditingActionList_AllowInsert,
				StringResources.DataEditingActionList_AllowDelete
			});
			var result = new DesignerActionItemCollection();
			foreach(DesignerActionItem item in actionItems)
				if(!prohibitedActionItemNames.Contains(item.DisplayName))
					result.Add(item);
			return result;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			var gridViewActionItems = CleanActionList(GridViewActionList.GetSortedActionItems());
			var dropDownActionItems = CleanActionList(base.GetSortedActionItems());
			return MergeActionLists(dropDownActionItems, gridViewActionItems);
		}
		protected DesignerActionItemCollection MergeActionLists(DesignerActionItemCollection masterItems, DesignerActionItemCollection actionItems) {
			var result = new DesignerActionItemCollection();
			foreach(DesignerActionItem item in actionItems)
				if(!IsActionListContainsItem(item, masterItems))
					result.Add(item);
			foreach(DesignerActionItem item in masterItems)
				result.Add(item);
			return result;
		}
		protected bool IsActionListContainsItem(DesignerActionItem targetItem, DesignerActionItemCollection items) {
			foreach(DesignerActionItem listItem in items)
				if(listItem.DisplayName == targetItem.DisplayName)
					return true;
			return false;
		}
	}
}
