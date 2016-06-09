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
using System.Drawing;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using DevExpress.Utils.About;
namespace DevExpress.Web.Design {
	public class FormLayoutEditableDesignerRegion : EditableDesignerRegion {
		private LayoutItem layoutItem = null;
		public LayoutItem LayoutItem {
			get { return this.layoutItem; }
		}
		public FormLayoutEditableDesignerRegion(ASPxFormLayoutDesigner designer, string itemPath, LayoutItem layoutItem)
			: base(designer, itemPath, false) {
				this.layoutItem = layoutItem;
		}
	}
	public class FormLayoutSelectableRegion : DesignerRegion {
		TabbedLayoutGroup tabbedGroup = null;
		int tabIndex = -1;
		public TabbedLayoutGroup TabbedLayoutGroup {
			get { return tabbedGroup; }
		}
		public int TabIndex {
			get { return tabIndex; }
		}
		public FormLayoutSelectableRegion(ASPxFormLayoutDesigner designer, string name, TabbedLayoutGroup tabbedGroup, int tabIndex)
			: base(designer, name, true) {
				this.tabbedGroup = tabbedGroup;
				this.tabIndex = tabIndex;
		}
	}
	public class ASPxFormLayoutDesigner : ASPxDataWebControlDesigner {
		private ASPxFormLayout formLayout;
		private bool itemsRetrieving = false;
		public ASPxFormLayoutDesigner()
			: base() {
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			formLayout = (ASPxFormLayout)component;
		}
		public override void ShowAbout() {
			FormLayoutAboutDialogHelper.ShowAbout(Component.Site);
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			propertyNameToCaptionMap.Add("Items", "Items");
		}
		protected override void AddDesignerRegions(DesignerRegionCollection regions) {
			AddEditableRegionsRecursive(regions, formLayout.Root);
			List<TabbedLayoutGroup> tabbedGroups = new List<TabbedLayoutGroup>();
			formLayout.Root.ForEach(delegate(LayoutItemBase item) {
				if (item is TabbedLayoutGroup && (item as TabbedLayoutGroup).ShowGroupDecoration && IsItemVisible(item))
					tabbedGroups.Add(item as TabbedLayoutGroup);
			});
			for (int i = 0; i < tabbedGroups.Count; i++) {
				TabbedLayoutGroup tabbedGroup = tabbedGroups[i];
				for (int j = 0; j < tabbedGroup.Items.GetVisibleItemCount(); j++) {
					int regionIndex = regions.Count;
					regions.Add(new FormLayoutSelectableRegion(this, string.Format("Activate [Item (\"{0}\")]", regionIndex), tabbedGroup, j));
					ASPxPageControl pageControl = GetPageControl(tabbedGroup);
					TabBase tab = pageControl.TabPages[j];
					WebControl tabItemControl = pageControl.FindControl(pageControl.GetTabElementID(tab, false)) as WebControl;
					tabItemControl.Attributes[DesignerRegion.DesignerRegionAttributeName] = regionIndex.ToString();
				}
			}
			base.AddDesignerRegions(regions);
		}
		protected bool IsItemVisible(LayoutItemBase item) {
			LayoutItemBase currentItem = item;
			while (currentItem != null) {
				if (!currentItem.Visible)
					return false;
				currentItem = currentItem.ParentGroupInternal;
			}
			return true;
		}
		protected ASPxPageControl GetPageControl(TabbedLayoutGroup tabbedGroup) {
			LayoutGroupBase currentParent = tabbedGroup.ParentGroup;
			while (currentParent != null) {
				if (currentParent is TabbedLayoutGroup)
					return GetPageControl(currentParent as TabbedLayoutGroup).FindControl(tabbedGroup.PageControlId) as ASPxPageControl;
				currentParent = currentParent.ParentGroup;
			}
			return ViewControl.FindControl(tabbedGroup.PageControlId) as ASPxPageControl;
		}
		protected override void OnClick(DesignerRegionMouseEventArgs e) {
			base.OnClick(e);
			FormLayoutSelectableRegion region = e.Region as FormLayoutSelectableRegion;
			if (region != null) {
				region.TabbedLayoutGroup.ActiveTabIndex = region.TabIndex;
				PropertyChanged("Root");
			}
		}
		protected void AddEditableRegionsRecursive(DesignerRegionCollection regions, LayoutItemBase item) {
			if(item is LayoutGroupBase) {
				for(int i = 0; i < (item as LayoutGroupBase).Items.GetVisibleItemCount(); i++)
					AddEditableRegionsRecursive(regions, (item as LayoutGroupBase).Items.GetVisibleItemOrGroup(i));
			} else if (item is LayoutItem) {
				LayoutItem layoutItem = item as LayoutItem;
				regions.Add(new FormLayoutEditableDesignerRegion(this, item.Path, layoutItem));
				LayoutItemNestedControlContainer contentControl = (((ASPxFormLayout)ViewControl).FindItemOrGroupByPath(item.Path) as LayoutItem).NestedControlContainer;
				contentControl.DesignerRegionAttribute = (regions.Count - 1).ToString();
			}
		}
		protected override void OnSchemaRefreshed() {
			base.OnSchemaRefreshed();
			Cursor current = Cursor.Current;
			try {
				Cursor.Current = Cursors.WaitCursor;
				System.Web.UI.Design.ControlDesigner.InvokeTransactedChange(Component, new TransactedChangeCallback(OnSchemaRefreshedCallback), null, "SchemaRefreshed");
				UpdateDesignTimeHtml();
			}
			finally {
				Cursor.Current = current;
			}
		}
		private bool OnSchemaRefreshedCallback(object context) {
			if(itemsRetrieving)
				return false;
			itemsRetrieving = true;
			try {
				FormLayoutItemsEditorFrame.OnRetrieveFields(new FormLayoutItemsOwner(formLayout));
			}
			finally {
				itemsRetrieving = false;
			}
			return true;
		}
		protected override string GetBaseProperty() {
			return "Items";
		}
		protected override string GetDesignTimeHtmlInternal() {
			return this.formLayout.Root.Items.GetVisibleItemCount() == 0 ? base.GetEmptyDesignTimeHtmlInternal() : base.GetDesignTimeHtmlInternal();
		}
		public override string GetEditableDesignerRegionContent(EditableDesignerRegion region) {
			FormLayoutEditableDesignerRegion layoutRegion = region as FormLayoutEditableDesignerRegion;
			return GetEditableDesignerRegionContent((formLayout.FindItemOrGroupByPath(layoutRegion.LayoutItem.Path) as LayoutItem).Controls);
		}
		public override void SetEditableDesignerRegionContent(EditableDesignerRegion region, string content) {
			FormLayoutEditableDesignerRegion layoutRegion = region as FormLayoutEditableDesignerRegion;
			LayoutItem layoutItem = formLayout.FindItemOrGroupByPath(layoutRegion.LayoutItem.Path) as LayoutItem;
			SetEditableDesignerRegionContent(layoutItem.Controls, content);
		}
		public override void RunDesigner() {
			ShowDialog(new FormLayoutDesignerEditorForm((ASPxFormLayout)Component));
		}
	}
	public class FormLayoutDesignerEditorForm : LayoutViewDesignerEditorForm {
		public FormLayoutDesignerEditorForm(ASPxFormLayout formLayout) 
			: base(new FormLayoutCommonDesigner(formLayout, formLayout.Site)) {
		}
	}
	public class LayoutViewDesignerEditorForm : WrapperEditorForm {
		public LayoutViewDesignerEditorForm(CommonFormDesigner formDesigner) : base(formDesigner) {
		}
		protected override Size DefaultMinimumSize { get { return new Size(1180, 800); } }
	}
	public class FormLayoutCommonDesigner : CommonFormDesigner {
		public FormLayoutCommonDesigner(ASPxFormLayout formLayout, IServiceProvider provider)
			: base(formLayout, provider) {
			FormLayout = formLayout;
		}
		ASPxFormLayout FormLayout { get; set; }
		protected override void CreateItemsItem() {
			MainGroup.Add(CreateDesignerItem("Items", "Items", typeof(FormLayoutItemsEditorFrame), FormLayout, ItemsImageIndex, new FormLayoutItemsOwner(FormLayout)));
		}
	}
	public class FormLayoutAboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxFormLayout), new ProductKind[] { ProductKind.FreeOffer, ProductKind.DXperienceASP });
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if(ShouldShowTrialAbout(typeof(ASPxFormLayout)))
				ShowAbout(provider);
		}
	}
}
