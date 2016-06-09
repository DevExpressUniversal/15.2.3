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
using System.ComponentModel.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxNavBarDesigner : ASPxHierarchicalDataWebControlDesigner {
		private NavBarGroupCollection fSavedGroups = new NavBarGroupCollection();
		private static string fGroupTemplateCaption = "Groups[{0}]";
		private static string[] fGroupTemplateNames = new string[] { "ContentTemplate", 
			"HeaderTemplate", "HeaderTemplateCollapsed", "ItemTemplate", "ItemTextTemplate" };
		private static string[] fGroupItemTemplateNames = new string[] { "Template", "TextTemplate" };
		private static string[] fGroupItemTemplateCaptions = new string[] { "Items[{0}].Template", "Items[{0}].TextTemplate" };
		private static string[] fControlTemplateNames = new string[] { "GroupContentTemplate", 
			"GroupHeaderTemplate", "GroupHeaderTemplateCollapsed", "ItemTemplate", "ItemTextTemplate" };
		ASPxNavBar NavBar { get; set; }
		public override void Initialize(IComponent component) {
			NavBar = (ASPxNavBar)component;
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Groups", "Groups");
			propertyNameToCaptionMap.Add("Items", "Items");
		}
		protected override string GetBaseProperty() {
			return "Groups";
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new NavBarItemsOwner(NavBar, DesignerHost)));
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
			ASPxNavBar navBar = dataControl as ASPxNavBar;
			if(!string.IsNullOrEmpty(navBar.DataSourceID) || (navBar.DataSource != null) || !navBar.HasVisibleGroups()) {
				navBar.Groups.Clear();
				base.DataBind(navBar);
			}
		}
		protected override IHierarchicalEnumerable GetSampleDataSource() {
			return new NavBarHierarchicalSampleData(0, string.Empty);
		}
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection templateGroups = base.CreateTemplateGroups();
			NavBarGroupCollection groups = NavBar.Groups;
			for(int i = 0; i < groups.Count; i++) {
				NavBarGroup group = groups[i] as NavBarGroup;
				TemplateGroup templateGroup = new TemplateGroup(string.Format(fGroupTemplateCaption, i));
				for(int j = 0; j < fGroupTemplateNames.Length; j++) {
					TemplateDefinition templateDefinition = new TemplateDefinition(this, fGroupTemplateNames[j],
						group, fGroupTemplateNames[j], GetTemplateStyle(group, null, fGroupTemplateNames[j]));
					templateDefinition.SupportsDataBinding = true;
					templateGroup.AddTemplateDefinition(templateDefinition);
				}
				for(int j = 0; j < group.Items.Count; j++) {
					NavBarItem item = group.Items[j];
					for(int k = 0; k < fGroupItemTemplateNames.Length; k++) {
						TemplateDefinition templateDefinition = new TemplateDefinition(this,
							string.Format(fGroupItemTemplateCaptions[k], j), item, fGroupItemTemplateNames[k],
							GetTemplateStyle(group, item, fGroupItemTemplateNames[k]));
						templateDefinition.SupportsDataBinding = true;
						templateGroup.AddTemplateDefinition(templateDefinition);
					}
				}
				templateGroups.Add(templateGroup);
			}
			for(int i = 0; i < fControlTemplateNames.Length; i++) {
				TemplateGroup templateGroup = new TemplateGroup(fControlTemplateNames[i]);
				TemplateDefinition templateDefinition = new TemplateDefinition(this, fControlTemplateNames[i],
					Component, fControlTemplateNames[i], GetTemplateStyle(null, null, fControlTemplateNames[i]));
				templateDefinition.SupportsDataBinding = true;
				templateGroup.AddTemplateDefinition(templateDefinition);
				templateGroups.Add(templateGroup);
			}
			return templateGroups;
		}
		protected Style GetTemplateStyle(NavBarGroup group, NavBarItem item, string templateName) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(NavBar.GetControlStyle());
			switch(templateName) {
				case "HeaderTemplate":
				case "GroupHeaderTemplate":
					style.CopyFrom(NavBar.GetGroupHeaderStyle(group, true));
					break;
				case "HeaderTemplateCollapsed":
				case "GroupHeaderTemplateCollapsed":
					style.CopyFrom(NavBar.GetGroupHeaderStyle(group, false));
					break;
				case "ContentTemplate":
				case "GroupContentTemplate":
				case "ItemTemplate":
				case "Template":
					style.CopyFrom(NavBar.GetGroupContentStyle(group));
					break;
				case "ItemTextTemplate":
				case "TextTemplate":
					style.CopyFrom(NavBar.GetItemStyle(item));
					break;
			}
			return style;
		}
		protected override void AddDesignerRegions(DesignerRegionCollection regions) {
			for(int i = 0; i < NavBar.Groups.GetVisibleItemCount(); i++) {
				NavBarGroup group = NavBar.Groups.GetVisibleItem(i);
				AddHeaderSelectableRegion("Collapse " + this.GetRegionName(group), group, true, regions);
				AddHeaderSelectableRegion("Expand " + this.GetRegionName(group), group, false, regions);
			}
		}
		protected void AddHeaderSelectableRegion(string name, NavBarGroup group, bool expanded, DesignerRegionCollection regions) {
			regions.Add(new NavBarSelectableRegion(this, name, group));
			string headerElementId = NavBar.GetGroupHeaderExpandButtonID(group, expanded);
			WebControl headerElement = ViewControl.FindControl(headerElementId) as WebControl;
			if(headerElement != null)
				headerElement.Attributes[DesignerRegion.DesignerRegionAttributeName] = (regions.Count - 1).ToString();
		}
		protected string GetRegionName(NavBarGroup group) {
			if(group.Text != "")
				return group.Text;
			else if(group.Name != "")
				return group.Name;
			else
				return ("[Group (" + group.Index + ")]");
		}
		protected override void OnClick(DesignerRegionMouseEventArgs e) {
			base.OnClick(e);
			NavBarSelectableRegion region = e.Region as NavBarSelectableRegion;
			if(region != null) {
				region.Group.Expanded = !region.Group.Expanded;
				PropertyChanged("Groups");
			}
		}
		protected internal override string[] GetDataBindingSchemaFields() {
			return new string[] { "Enabled", "NavigateUrl", "Target", "Text", "ToolTip" };
		}
		protected internal override Type GetDataBindingSchemaItemType() {
			return typeof(NavBarGroup); 
		}
	}
	public class NavBarSelectableRegion : DesignerRegion {
		private NavBarGroup fGroup = null;
		public NavBarGroup Group {
			get { return fGroup; }
		}
		public NavBarSelectableRegion(ASPxNavBarDesigner designer, string name, NavBarGroup group)
			: base(designer, name, true) {
			fGroup = group;
		}
	}
}
