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
using System.Collections.Specialized;
using System.Text;
using System.ComponentModel.Design;
using System.ComponentModel;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using System.Web.UI;
using System.Web.UI.Design;
using DevExpress.Web.Design.Forms;
using System.Configuration;
using System.Web.Configuration;
using System.Xml;
namespace DevExpress.Web.Design {
	public class ASPxUploadControlDesigner : ASPxWebControlDesigner {
		ASPxUploadControl uploadControl = null;
		public override void Initialize(IComponent component) {
			this.uploadControl = (ASPxUploadControl)component;
			base.Initialize(component);
		}
		protected override bool IsControlRequireHttpHandlerRegistration() {
			return true;
		}
		public bool ShowProgressPanel {
			get { return UploadControl.ShowProgressPanel; }
			set {
				UploadControl.ShowProgressPanel = value;
			}
		}
		protected internal bool ShowError {
			get { return UploadControl.PreviewErrorInDesigner; }
			set {
				UploadControl.PreviewErrorInDesigner = value;
				UpdateDesignTimeHtml();
				TypeDescriptor.Refresh(Component);
			}
		}
		public ASPxUploadControl UploadControl {
			get { return this.uploadControl; }
		}
		protected override Control CreateViewControl() {
			Control view = base.CreateViewControl();
			(view as ASPxUploadControl).PreviewErrorInDesigner = UploadControl.PreviewErrorInDesigner;
			return view;
		}
		protected override string GetBaseProperty() {
			return "FileInputCount";
		}
		protected override void PreFilterProperties(System.Collections.IDictionary properties) {
			base.PreFilterProperties(properties);
			PropertyDescriptor showPP = (PropertyDescriptor)properties["ShowProgressPanel"];
			properties["ShowProgressPanel"] = TypeDescriptor.CreateProperty(typeof(ASPxUploadControlDesigner), showPP);
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new UploadDesignerActionList(this);
		}
	}
	public class UploadDesignerActionList : ASPxWebControlDesignerActionList {
		private ASPxUploadControlDesigner designer = null;
		public UploadDesignerActionList(ASPxUploadControlDesigner designer)
			: base(designer) {
			this.designer = designer;
		}
		public new ASPxUploadControlDesigner Designer {
			get { return this.designer; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Insert(0, new DesignerActionPropertyItem("UploadStorage", 
				StringResources.UploadControl_UploadStorageActionItem, "Behavior"));
			collection.Add(new DesignerActionPropertyItem("EnableMultiSelect",
				StringResources.UploadControl_EnableMultiSelectActionItem, "",
				StringResources.UploadControl_EnableMultiSelectActionItemDescription));
			collection.Add(new DesignerActionPropertyItem("EnableDragAndDrop",
				StringResources.UploadControl_EnableDragAndDropActionItem));
			collection.Add(new DesignerActionPropertyItem("EnableFileList",
				StringResources.UploadControl_EnableFileListActionItem));
			collection.Add(new DesignerActionPropertyItem("ShowClearFileSelectionButton",
				StringResources.UploadControl_ShowClearFileSelectionButtonActionItem));
			collection.Add(new DesignerActionPropertyItem("ShowUploadButton",
				StringResources.UploadControl_ShowUploadButtonActionItem));
			collection.Add(new DesignerActionPropertyItem("ShowProgressPanel",
				StringResources.UploadControl_ShowProgressPanelActionItem));
			collection.Add(new DesignerActionPropertyItem("ShowError", "Preview Error"));
			collection.Add(new DesignerActionMethodItem(this, "OpenConfigureMaximumUploadLimitsHelp",
				StringResources.UploadControl_HowConfigureMaximumUploadLimitsActionItem,
				StringResources.UploadControl_HowConfigureMaximumUploadLimitsActionItem,
				StringResources.ActionList_OpenHelpActionItemDescription, false));			
			return collection;
		}
		protected void OpenConfigureMaximumUploadLimitsHelp() {
			ShowHelpFromUrl("#AspNet/CustomDocument9822");
		}
		public UploadControlUploadStorage UploadStorage {
			get { return designer.UploadControl.UploadStorage; }
			set { 
				designer.UploadControl.UploadStorage = value;
				UpdateDesigner(designer.Component, "UploadStorage");
			}
		}
		public int FileInputCount {
			get { return designer.UploadControl.FileInputCount; }
			set {
				IComponent component = designer.Component;
				TypeDescriptor.GetProperties(component)["FileInputCount"].SetValue(component, value); 
			}
		}
		public bool EnableMultiSelect {
			get { return designer.UploadControl.AdvancedModeSettings.EnableMultiSelect; }
			set {
				designer.UploadControl.AdvancedModeSettings.EnableMultiSelect = value;
				UpdateDesigner(designer.Component, "AdvancedModeSettings");
			}
		}
		public bool EnableFileList {
			get { return designer.UploadControl.AdvancedModeSettings.EnableFileList; }
			set {
				designer.UploadControl.AdvancedModeSettings.EnableFileList = value;
				UpdateDesigner(designer.Component, "AdvancedModeSettings");
			}
		}
		public bool EnableDragAndDrop {
			get { return designer.UploadControl.AdvancedModeSettings.EnableDragAndDrop; }
			set {
				designer.UploadControl.AdvancedModeSettings.EnableDragAndDrop = value;
				UpdateDesigner(designer.Component, "AdvancedModeSettings");
			}
		}
		public bool ShowAddRemoveButtons {
			get { return designer.UploadControl.ShowAddRemoveButtons; }
			set {
				IComponent component = designer.Component;
				TypeDescriptor.GetProperties(component)["ShowAddRemoveButtons"].SetValue(component, value); 
			}
		}
		public bool ShowClearFileSelectionButton {
			get { return designer.UploadControl.ShowClearFileSelectionButton; }
			set {
				IComponent component = designer.Component;
				TypeDescriptor.GetProperties(component)["ShowClearFileSelectionButton"].SetValue(component, value); 
			}
		}
		public bool ShowUploadButton {
			get { return designer.UploadControl.ShowUploadButton; }
			set {
				IComponent component = designer.Component;
				TypeDescriptor.GetProperties(component)["ShowUploadButton"].SetValue(component, value); 
			}
		}
		public bool ShowProgressPanel {
			get { return designer.UploadControl.ShowProgressPanel; }
			set {
				IComponent component = designer.Component;
				TypeDescriptor.GetProperties(component)["ShowProgressPanel"].SetValue(component, value); 
			}
		}
		public bool ShowError {
			get { return Designer.ShowError; }
			set { Designer.ShowError = value; }
		}
		static void UpdateDesigner(IComponent component, string propertyName) {
			PropertyDescriptor descriptor = TypeDescriptor.GetProperties(component)[propertyName];
			IComponentChangeService changeService = component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(changeService != null)
				changeService.OnComponentChanged(component, descriptor, null, null);
			DesignerActionUIService uiService = component.Site.GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
			if(uiService != null)
				uiService.Refresh(component);
		}
	}
}
