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
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using System.ComponentModel.Design;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraLayout.Converter;
using System.Collections.Generic;
using DevExpress.XtraBars.Navigation;
namespace DevExpress.XtraLayout.DesignTime {
	public class LayoutConverterDesigner : BaseComponentDesigner {
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			if(AllowDesigner)
				list.Add(new LayoutConverterDesignerActionList(this));
			base.RegisterActionLists(list);
		}
		protected LayoutConverter Converter {
			get {
				return Component as LayoutConverter;
			}
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
		}
		protected void ShowDesignerFormCore(Form designerForm) {
			if(Converter == null) return;
			try {
				designerForm.ShowInTaskbar = false;
				designerForm.ShowDialog();
			}
			finally {
			}
		}
		public virtual void OnConvertToXtraLayout(object sender, EventArgs ea) {
			Form form = new ConvertToXtraLayoutForm(Converter);
			StartChange();
			ShowDesignerFormCore(form);
			EndChange();
			form = null;
		}
		public virtual void OnConvertToStandartLayout(object sender, EventArgs ea) {
			Form form = new ConvertToStandardLayoutForm(Converter);
			StartChange();
			ShowDesignerFormCore(form);
			EndChange();
			form = null;
		}
		public virtual void OnConvertToAccordionControl(object sender, EventArgs ea) {
			using(ConvertToAccordionControlForm form = new ConvertToAccordionControlForm(Converter)) {
				StartChange();
				ShowDesignerFormCore(form);
				EndChange();
				LayoutControl control = form.SelectedLayoutControl;
				if(control != null) {
					LayoutConverterHelper.CheckDevExpressXtraBarsReference(Component.Site);
					Helper.CreateFromLayoutControl(control, DesignerHost);
					control.Dispose();
				}
			}
		}
		LayoutConverterHelper helper;
		protected LayoutConverterHelper Helper {
			get {
				if(helper == null)
					helper = new LayoutConverterHelper();
				return helper;
			}
		}
		DesignerTransaction temporaryTransaction = null;
		protected void StartChange() {
			temporaryTransaction = DesignerHost.CreateTransaction("layout conversion");
		}
		protected void EndChange() {
			if(temporaryTransaction != null) {
				temporaryTransaction.Commit();
			}
			temporaryTransaction = null;
		}
		IDesignerHost designerHost;
		public IDesignerHost DesignerHost {
			get {
				if(designerHost == null) designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
				return designerHost;
			}
		}
	}
	public class LayoutConverterHelper {
		protected IDesignerHost DesignerHost { get; private set; }
		protected AccordionControl AccordionControl { get; private set; }
		public static void CheckDevExpressXtraBarsReference(IServiceProvider provider) {
			EnvDTE.Project project = GetProject(provider);
			if(project == null) return;
			try {
				if(ProjectHelper.IsReferenceExists(project, AssemblyInfo.SRAssemblyBars))
					return;
				ProjectHelper.AddReference(project, AssemblyInfo.SRAssemblyBars);
			}
			catch {
			}
		}
		static EnvDTE.Project GetProject(IServiceProvider provider) {
			if(provider == null) return null;
			var item = provider.GetService(typeof(EnvDTE.ProjectItem)) as EnvDTE.ProjectItem;
			return item != null ? item.ContainingProject : null;
		}
		public void CreateFromLayoutControl(LayoutControl control, IDesignerHost designerHost) {
			DesignerHost = designerHost;
			Control parentControl = control.Parent;
			if(parentControl == null || DesignerHost == null) return;
			AccordionControl accordionControl = new AccordionControl();
			designerHost.Container.Add(accordionControl);
			AccordionControl = accordionControl;
			accordionControl.Size = control.Size;
			accordionControl.Dock = control.Dock;
			CreateFromLayoutControlCore(control.Root, accordionControl.Elements, 0);
			control.Dispose();
			parentControl.Controls.Add(accordionControl);
		}
		protected int CreateFromLayoutControlCore(LayoutControlGroup group, AccordionControlElementCollection ownerElements, int level) {
			int count = 0;
			foreach(var child in group.Items) {
				LayoutControlGroup childGroup = child as LayoutControlGroup;
				if(childGroup != null) {
					string groupName = childGroup.Text;
					AccordionControlElement accordionGroup = CreateGroup(groupName);
					int childGroups = CreateFromLayoutControlCore(childGroup, accordionGroup.Elements, level + 1);
					if(childGroups == 0) {
						string itemName = groupName + (level == 0? "Item" : "");
						AccordionControlElement accordionItem = CreateItem(itemName);
						if(level == 0)
							AddElement(accordionItem, accordionGroup.Elements);
						else
							AddElement(accordionItem, ownerElements);
						AccordionContentContainer contentContainer = CreateContentContainer(childGroup);
						if(contentContainer != null) {
							DesignerHost.Container.Add(contentContainer);
							accordionItem.ContentContainer = contentContainer;
						}
						if(level == 0)
							AddElement(accordionGroup, ownerElements);
					}
					else AddElement(accordionGroup, ownerElements);
					count++;
				}
			}
			return count;
		}
		protected AccordionControlElement CreateGroup(string text) {
			AccordionControlElement group = new AccordionControlElement(ElementStyle.Group);
			group.Text = text;
			return group;
		}
		protected AccordionControlElement CreateItem(string text) {
			AccordionControlElement item = new AccordionControlElement(ElementStyle.Item);
			item.Text = text;
			return item;
		}
		protected void AddElement(AccordionControlElement element, AccordionControlElementCollection ownerElements) {
			if(ownerElements != null) ownerElements.Add(element);
			DesignerHost.Container.Add(element);
		}
		protected AccordionContentContainer CreateContentContainer(LayoutControlGroup group) {
			AccordionContentContainer contentContainer = new AccordionContentContainer();
			LayoutControl layoutControl = new LayoutControl();
			layoutControl.Size = contentContainer.Size = group.Size;
			while(group.Items.Count != 0) {
				BaseLayoutItem item = group.Items[0];
				item.Parent = null;
				item.Owner = null;
				layoutControl.Root.AddItem(item);
			}
			layoutControl.Dock = DockStyle.Fill;
			if(layoutControl.Root.Items.Count == 0) return null;
			DesignerHost.Container.Add(layoutControl);
			foreach(BaseLayoutItem item in layoutControl.Items) {
				DesignerHost.Container.Add(item);
			}
			layoutControl.Parent = contentContainer;
			AccordionControl.Controls.Add(contentContainer);
			return contentContainer;
		}
	}
	public class LayoutConverterDesignerActionList : DesignerActionList {
		LayoutConverterDesigner designer;
		public LayoutConverterDesignerActionList(LayoutConverterDesigner designer) : base(designer.Component) {
			this.designer = designer;
		}
		public void ToXtraLayout() {
			if (designer != null) designer.OnConvertToXtraLayout(this, null);
		}
		public void ToRegularLayout() {
			if (designer != null) designer.OnConvertToStandartLayout(this, null);
		}
		public void ToAccordionControl() {
			if(designer != null) designer.OnConvertToAccordionControl(this, null);
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			if (designer != null && designer.Component != null) {
				res.Add(new DesignerActionMethodItem(this, "ToXtraLayout", "Convert regular layout to XtraLayout", "Actions", true));
				res.Add(new DesignerActionMethodItem(this, "ToRegularLayout", "Convert XtraLayout to regular layout", "Actions", true));
				res.Add(new DesignerActionMethodItem(this, "ToAccordionControl", "Convert XtraLayout to AccordionControl", "Actions", true));
			}
			return res;
		}
	}
}
