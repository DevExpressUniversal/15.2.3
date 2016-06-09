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
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxDockPanelDesigner : ASPxWebControlDesigner {
		ASPxDockPanel panel = null;
		static string[] ControlTemplateNames = new string[] { "HeaderTemplate", "FooterTemplate" };
		public ASPxDockPanel Panel {
			get { return this.panel; }
		}
		public string PanelUID {
			get { return Panel.PanelUID; }
			set {
				string oldValue = PanelUID;
				Panel.PanelUID = value;
				RaiseComponentChanged(TypeDescriptor.GetProperties(typeof(ASPxDockPanel))["PanelUID"], oldValue, value);
			}
		}
		public string OwnerZoneUID {
			get { return Panel.OwnerZoneUID; }
			set {
				string oldValue = OwnerZoneUID;
				Panel.OwnerZoneUID = value;
				RaiseComponentChanged(TypeDescriptor.GetProperties(typeof(ASPxDockPanel))["OwnerZoneUID"], oldValue, value);
			}
		}
		public override void Initialize(IComponent component) {
			this.panel = (ASPxDockPanel)component;
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
		}
		protected override string GetBaseProperty() {
			return "PanelUID";
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new DockPanelCommonFormDesigner(Panel, DesignerHost)));
		}
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection templateGroups = base.CreateTemplateGroups();
			for(int i = 0; i < ControlTemplateNames.Length; i++) {
				TemplateGroup templateGroup = new TemplateGroup(ControlTemplateNames[i]);
				TemplateDefinition templateDefinition = new TemplateDefinition(this, ControlTemplateNames[i],
					Component, ControlTemplateNames[i], GetTemplateStyle(ControlTemplateNames[i]));
				templateDefinition.SupportsDataBinding = false;
				templateGroup.AddTemplateDefinition(templateDefinition);
				templateGroups.Add(templateGroup);
			}
			return templateGroups;
		}
		private Style GetTemplateStyle(string templateName) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(panel.GetControlStyle());
			switch(templateName) {
				case "HeaderTemplate":
					style.CopyFrom(panel.GetHeaderStyle(null));
					break;
				case "FooterTemplate":
					style.CopyFrom(panel.GetFooterStyle(null));
					break;
			}
			return style;
		}
		protected override void AddDesignerRegions(DesignerRegionCollection regions) {
			base.AddDesignerRegions(regions);
			int regionCount = regions.Count;
			regions.Add(new EditableDesignerRegion(this, "Edit content", false));
			PopupWindow window = ((ASPxDockPanel)ViewControl).DefaultWindow;
			window.ContentControl.Attributes[DesignerRegion.DesignerRegionAttributeName] =
				regionCount.ToString();
		}
		public override string GetEditableDesignerRegionContent(EditableDesignerRegion region) {
			return GetEditableDesignerRegionContent(panel.DefaultWindow.ContentControl.Controls);
		}
		public override void SetEditableDesignerRegionContent(EditableDesignerRegion region, string content) {
			SetEditableDesignerRegionContent(panel.DefaultWindow.ContentControl.Controls, content);
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new DockPanelDesignerActionList(this);
		}
	}
	public class DockPanelDesignerActionList : ASPxWebControlDesignerActionList {
		ASPxDockPanelDesigner designer = null;
		protected internal ASPxDockPanelDesigner PanelDesigner { get { return this.designer; } }
		public string PanelUID {
			get { return PanelDesigner.PanelUID; }
			set { PanelDesigner.PanelUID = value; }
		}
		[Editor(typeof(OwnerZoneUIDEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string OwnerZoneUID {
			get { return PanelDesigner.OwnerZoneUID; }
			set { PanelDesigner.OwnerZoneUID = value; }
		}
		public DockPanelDesignerActionList(ASPxDockPanelDesigner designer)
			: base(designer) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("PanelUID",
				StringResources.DockPanelActionList_PanelUID,
				"UID",
				StringResources.DockPanelActionList_EditPanelUIDDescription));
			collection.Add(new DesignerActionPropertyItem("OwnerZoneUID",
				StringResources.DockPanelActionList_OwnerZoneUID,
				"UID",
				StringResources.DockPanelActionList_EditOwnerZoneUIDDescription));
			return collection;
		}
	}
	public class OwnerZoneUIDEditor : DropDownUITypeEditorBase {
		protected ASPxDockPanel GetPanel(ITypeDescriptorContext context) {
			ASPxDockPanel panel = context.Instance as ASPxDockPanel;
			return panel ?? (context.Instance as DockPanelDesignerActionList).PanelDesigner.Panel;
		}
		protected override void ApplySelectedValue(System.Windows.Forms.ListBox valueList, ITypeDescriptorContext context) {
			ASPxDockPanel panel = GetPanel(context);
			panel.OwnerZoneUID = valueList.SelectedItem.ToString();
		}
		protected override object GetComponent(ITypeDescriptorContext context) {
			return GetPanel(context);
		}
		protected override PropertyDescriptor GetChangedPropertyDescriptor(object component) {
			return null;
		}
		protected override void FillValueList(System.Windows.Forms.ListBox valueList, ITypeDescriptorContext context) {
			ASPxDockPanel panel = GetPanel(context); 
			foreach(ASPxDockZone zone in PanelZoneRelationsMediator.GetZones(panel.Page)) {
				if(zone == null || panel.Page != zone.Page)
					continue;
				if(!panel.ForbiddenZones.Contains(zone))
					valueList.Items.Add(zone.ZoneUID);
			}
		}
		protected override void SetInitiallySelectedValue(System.Windows.Forms.ListBox valueList, ITypeDescriptorContext context) {
			valueList.SelectedItem = null;
		}
	}
	public class ForbiddenZoneUIDEditor : DropDownUITypeEditorBase {
		protected override void ApplySelectedValue(System.Windows.Forms.ListBox valueList, ITypeDescriptorContext context) {
			ForbiddenZoneItem item = context.Instance as ForbiddenZoneItem;
			item.ZoneUID = valueList.SelectedItem.ToString();
		}
		protected override object GetComponent(ITypeDescriptorContext context) {
			ForbiddenZoneItem item = context.Instance as ForbiddenZoneItem;
			return item.Panel;
		}
		protected override PropertyDescriptor GetChangedPropertyDescriptor(object component) {
			return null;
		}
		protected override void FillValueList(System.Windows.Forms.ListBox valueList, ITypeDescriptorContext context) {
			ForbiddenZoneItem item = context.Instance as ForbiddenZoneItem;
			foreach(ASPxDockZone zone in PanelZoneRelationsMediator.GetZones(item.Panel.Page)) {
				if(zone == null || item.Panel.Page != zone.Page)
					continue;
				if(!item.Panel.ForbiddenZones.Contains(zone))
					valueList.Items.Add(zone.ZoneUID);
			}
		}
		protected override void SetInitiallySelectedValue(System.Windows.Forms.ListBox valueList, ITypeDescriptorContext context) {
			valueList.SelectedItem = null;
		}
	}
	public class DockPanelCommonFormDesigner : CommonFormDesigner {
		public DockPanelCommonFormDesigner(ASPxDockPanel dockPanel, IServiceProvider provider)
			: base(new DockPanelZonesOwner(dockPanel, provider)) {
			ItemsImageIndex = ForbiddenZonesImageIndex;
		}
	}
	public class DockPanelZonesOwner : FlatCollectionItemsOwner<ForbiddenZoneItem> {
		public DockPanelZonesOwner(ASPxDockPanel dockPanel, IServiceProvider provider)
			: base(dockPanel, provider, dockPanel.ForbiddenZones, "Forbidden Zones") {
		}
		public override List<string> GetViewDependedProperties() {
			var result = base.GetViewDependedProperties();
			result.Add("ZoneUID");
			return result;
		}
	}
}
