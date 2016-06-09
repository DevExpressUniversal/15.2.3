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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Drawing;
namespace DevExpress.Utils.Design {
	public class FlyoutPanelDesigner : BaseParentControlDesigner {
		public FlyoutPanelDesigner() {
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			EnsureContentPanel();
		}
		void EnsureContentPanel() {
			FlyoutPanelControl panel = new FlyoutPanelControl(FlyoutPanel);
			FlyoutPanel.Container.Add(panel);
			FlyoutPanel.Controls.Add(panel);
			panel.Dock = DockStyle.Fill;
			ComponentChangeSvc.OnComponentChanging(FlyoutPanel, null);
			ComponentChangeSvc.OnComponentChanged(FlyoutPanel, null, null, null);
		}
		IComponentChangeService componentChangeSvc = null;
		protected IComponentChangeService ComponentChangeSvc {
			get {
				if(componentChangeSvc == null)
					componentChangeSvc = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				return componentChangeSvc;
			}
		}
		#region ActionList
		DesignerActionListCollection list = null;
		public override DesignerActionListCollection ActionLists {
			get {
				if(list == null) {
					list = new DesignerActionListCollection();
					list.Add(new FlyoutPanelDesignerActionList(Component));
					DXSmartTagsHelper.CreateDefaultLinks(this, list);
				}
				return list;
			}
		}
		#endregion
		public FlyoutPanel FlyoutPanel { get { return Control as FlyoutPanel; } }
	}
	public class FlyoutPanelDesignerActionList : DesignerActionList {
		public FlyoutPanelDesignerActionList(IComponent component)
			: base(component) {
		}
		const string ButtonPanelOptionsCategory = "Buttons Panel";
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionHeaderItem(CategoryName.Options));
			res.Add(new DesignerActionPropertyItem("OwnerControl", "Owner Control", CategoryName.Options));
			res.Add(new DesignerActionPropertyItem("CloseOnOuterClick", "Close on Outer Click", CategoryName.Options));
			res.Add(new DesignerActionHeaderItem(ButtonPanelOptionsCategory));
			res.Add(new DesignerActionPropertyItem("ShowButtonPanel", "Show ButtonPanel", ButtonPanelOptionsCategory));
			res.Add(new DesignerActionPropertyItem("ButtonPanelLocation", "ButtonPanel Location", ButtonPanelOptionsCategory));
			res.Add(new DesignerActionPropertyItem("ButtonPanelHeight", "ButtonPanel Height", ButtonPanelOptionsCategory));
			res.Add(new DesignerActionPropertyItem("ButtonPanelContentAlignment", "ButtonPanel ContentAlignment", ButtonPanelOptionsCategory));
			res.Add(new DesignerActionMethodItem(this, "EditButtons", "Edit Buttons", ButtonPanelOptionsCategory));
			return res;
		}
		[TypeConverter(typeof(FlyoutPanelOwnerControlPropertyConverter))]
		public Control OwnerControl {
			get { return FlyoutPanel.OwnerControl; }
			set { EditorContextHelper.SetPropertyValue(FlyoutPanel.Site, FlyoutPanel, "OwnerControl", value); }
		}
		public bool CloseOnOuterClick {
			get { return FlyoutPanel.Options.CloseOnOuterClick; }
			set { EditorContextHelper.SetPropertyValue(FlyoutPanel.Site, FlyoutPanel.Options, "CloseOnOuterClick", value); }
		}
		public FlyoutPanelButtonPanelLocation ButtonPanelLocation {
			get { return FlyoutPanel.OptionsButtonPanel.ButtonPanelLocation; }
			set { EditorContextHelper.SetPropertyValue(FlyoutPanel.Site, FlyoutPanel.OptionsButtonPanel, "ButtonPanelLocation", value); }
		}
		public int ButtonPanelHeight {
			get { return FlyoutPanel.OptionsButtonPanel.ButtonPanelHeight; }
			set { EditorContextHelper.SetPropertyValue(FlyoutPanel.Site, FlyoutPanel.OptionsButtonPanel, "ButtonPanelHeight", value); }
		}
		public bool ShowButtonPanel {
			get { return FlyoutPanel.OptionsButtonPanel.ShowButtonPanel; }
			set { EditorContextHelper.SetPropertyValue(FlyoutPanel.Site, FlyoutPanel.OptionsButtonPanel, "ShowButtonPanel", value); }
		}
		public ContentAlignment ButtonPanelContentAlignment {
			get { return FlyoutPanel.OptionsButtonPanel.ButtonPanelContentAlignment; }
			set { EditorContextHelper.SetPropertyValue(FlyoutPanel.Site, FlyoutPanel.OptionsButtonPanel, "ButtonPanelContentAlignment", value); }
		}
		public void EditButtons() {
			EditorContextHelper.EditValue(DesignerHost.GetDesigner(FlyoutPanel), FlyoutPanel.OptionsButtonPanel, "Buttons");
		}
		IDesignerHost designerHost = null;
		protected IDesignerHost DesignerHost {
			get {
				if(designerHost == null) designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
				return designerHost;
			}
		}
		public FlyoutPanel FlyoutPanel { get { return Component as FlyoutPanel; } }
	}
	public class FlyoutPanelPropertyConverterBase : ComponentConverter {
		public FlyoutPanelPropertyConverterBase(Type type)
			: base(type) {
		}
		protected override bool IsValueAllowed(ITypeDescriptorContext context, object value) {
			if(value is FlyoutPanel || value is FlyoutPanelControl)
				return false;
			return base.IsValueAllowed(context, value);
		}
	}
	public class FlyoutPanelOwnerControlPropertyConverter : FlyoutPanelPropertyConverterBase {
		public FlyoutPanelOwnerControlPropertyConverter(Type type)
			: base(type) {
		}
		protected override bool IsValueAllowed(ITypeDescriptorContext context, object value) {
			return base.IsValueAllowed(context, value);
		}
	}
	public class FlyoutPanelTypeConverter : ComponentConverter {
		public FlyoutPanelTypeConverter()
			: base(typeof(IFlyoutPanel)) {
		}
	}
}
