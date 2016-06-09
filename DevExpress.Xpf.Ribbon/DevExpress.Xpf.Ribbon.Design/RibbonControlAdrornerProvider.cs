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

extern alias Platform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Services;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Ribbon;
using Platform::DevExpress.Xpf.Utils;
using Platform::DevExpress.Xpf.Core.Native;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core.Design.SmartTags;
namespace DevExpress.Xpf.Ribbon.Design {
	class RibbonControlAdornerProviderBase : PrimarySelectionAdornerProvider {
		public RibbonControlDesignService Service {
			get { return Context.Services.GetService<RibbonControlDesignService>(); }
		}
		public event Action Activated;
		public event Action Deactivated;
		public ModelItem Target { get; private set; }
		public bool IsActive { get; private set; }
		protected override void Activate(ModelItem item) {
			base.Activate(item);
			IsActive = true;
			Target = item;
			UpdatePanelsVisibity();
			SubscribeEvents();
			if(Activated != null) Activated();
		}
		protected override void Deactivate() {
			IsActive = false;
			UnsubscribeEvents();
			if(Deactivated != null) Deactivated();
			Target = null;
			base.Deactivate();
		}
		protected virtual void SubscribeEvents() {
			Target.PropertyChanged += OnTargetPropertyChanged;
		}
		protected virtual void UnsubscribeEvents() {
			Target.PropertyChanged -= OnTargetPropertyChanged;
		}
		protected virtual void OnTargetPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName == "Parent")
				OnParentChanged();
		}
		protected virtual void OnParentChanged() {
			UpdatePanelsVisibity();
		}
		protected virtual void UpdatePanelsVisibity() { }
		protected void ResetLayout(ModelItem item) {
			item.Properties["VerticalAlignment"].ClearValue();
			item.Properties["HorizontalAlignment"].ClearValue();
			item.Properties["Width"].ClearValue();
			item.Properties["Height"].ClearValue();
			item.Properties["Margin"].ClearValue();
		}
	}
	class RibbonModelItemProvider : IModelItemProvider {
		public object GetModelItemObject(UIElement child) {
		   if(child is RibbonPageGroupControl) {
				RibbonPageGroupControl control = (RibbonPageGroupControl)child;
				if(!control.IsCollapsed)
					return control.PageGroup;
			} else if(child is RibbonSelectedPageControl) {
				return ((RibbonSelectedPageControl)child).SelectedPage;
			} else if(child is RibbonPageCaptionControl) {
				return ((RibbonPageCaptionControl)child).Page;
			} else if(child is RibbonPageCategoryHeaderControl) {
				return ((RibbonPageCategoryHeaderControl)child).Category;
			}
			return null;
		}
	}
	[FeatureConnector(typeof(RibbonControlDesignFeatureConnector))]
	[UsesItemPolicy(typeof(RibbonControlHookPanelPolicy))]
	class RibbonControlAdornerProvider : BarManagerAdornerProviderBase {
		RibbonControl Ribbon { get; set; }
		RibbonPage LastSelectedPage { get; set; }
		ModelService ModelService { get { return Context == null ? null : Context.Services.GetRequiredService<ModelService>(); } }
		protected override void Activate(ModelItem item) {
			base.Activate(item);
			BarsModelItemProvider.RegisterAdditionalProvider(OnGetModelItemObject);
		}
		private object OnGetModelItemObject(UIElement arg) {
			var linkInfo = arg as BarItemLinkInfo;
			RibbonPageGroupControl pageGroupControl = null;
			if(linkInfo == null || linkInfo.Item == null)
				return null;
			pageGroupControl = LayoutHelper.FindParentObject<RibbonPageGroupControl>(linkInfo);
			if(pageGroupControl == null || RibbonControlHelper.GetRibbonPageGroupCollapsedItem(pageGroupControl) != linkInfo.Item)
				return null;
			return pageGroupControl.PageGroup;
		}
		protected override void Deactivate() {
			BarsModelItemProvider.UnregisterAdditionalProvider();
			base.Deactivate();
		}
		protected override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			if(Ribbon != null) {
				RibbonControlDesignTimeEventHelper.UnsubscribeRibbonSelectedPageChanged(Ribbon, OnRibbonSelectedPageChanged);
				RibbonControlDesignTimeEventHelper.UnsubscribeRibbonPageRemoved(Ribbon, OnRibbonPageRemoved);
				RibbonControlDesignTimeEventHelper.UnsubscribeRibbonPageGroupRemoved(Ribbon, OnRibbonPageGroupRemoved);
			}
			if(ModelService != null) ModelService.ModelChanged -= new EventHandler<ModelChangedEventArgs>(ModelService_ModelChanged);
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			if(AdornedElement != null && AdornedElement.GetCurrentValue() is RibbonControl) {
				Ribbon = (RibbonControl)AdornedElement.GetCurrentValue();
				RibbonControlDesignTimeEventHelper.SubscribeRibbonSelectedPageChanged(Ribbon, OnRibbonSelectedPageChanged);
				RibbonControlDesignTimeEventHelper.SubscribeRibbonPageRemoved(Ribbon, OnRibbonPageRemoved);
				RibbonControlDesignTimeEventHelper.SubscribeRibbonPageGroupRemoved(Ribbon, OnRibbonPageGroupRemoved);
				if(LastSelectedPage != Ribbon.SelectedPage) {
					OnRibbonSelectedPageChanged(Ribbon, new RibbonPropertyChangedEventArgs(LastSelectedPage, Ribbon.SelectedPage));
				}
			}
			if(ModelService != null) ModelService.ModelChanged += new EventHandler<ModelChangedEventArgs>(ModelService_ModelChanged);
		}
		void ModelService_ModelChanged(object sender, ModelChangedEventArgs e) {
			UnsubscribeEvents();
			SubscribeEvents();
		}
		void OnRibbonPageRemoved(object sender, RibbonPageRemovedEventArgs e) {
			if(e.RibbonPage == null) return;
			ModelItem ribbonPageCategory = RibbonDesignTimeHelper.GetModelItem(AdornedElement.Root, e.RibbonPage.PageCategory);
			ModelItem pageForSelect = GetItemForSelect(ribbonPageCategory.Properties["Pages"].Collection, e.Index);
			if(pageForSelect != null)
				SelectionOperations.SelectOnly(Context, pageForSelect);
		}
		void OnRibbonPageGroupRemoved(object sender, RibbonPageGroupRemovedEventArgs e) {
			if(e.RibbonPageGroup == null) return;
			ModelItem ribbonPage = RibbonDesignTimeHelper.GetModelItem(AdornedElement.Root, e.RibbonPageGroup.Page);
			ModelItem groupForSelect = GetItemForSelect(ribbonPage.Properties["Groups"].Collection, e.Index);
			if(groupForSelect != null)
				SelectionOperations.SelectOnly(Context, groupForSelect);
		}
		void OnRibbonSelectedPageChanged(object sender, RibbonPropertyChangedEventArgs e) {
			if(e.OldValue == null) return;
			RibbonPage page = e.NewValue as RibbonPage;
			RibbonControl ribbon = (RibbonControl)sender;
			if(page == null) {
				page = ribbon.Categories.Count > 0 && ((RibbonPageCategoryBase)ribbon.Categories[0]).Pages.Count > 0 ?
					((RibbonPageCategoryBase)ribbon.Categories[0]).Pages[0] : null;
			}
			if(page != null) {
				ModelItem pageItem = RibbonDesignTimeHelper.GetModelItem(AdornedElement, page);
				if(pageItem != null) SelectionOperations.Select(pageItem.Context, pageItem);
			}
			LastSelectedPage = page;
		}
		ModelItem GetItemForSelect(ModelItemCollection collection, int oldIndex) {
			if(collection == null || oldIndex < 0) return null;
			ModelItem result = null;
			if(collection.Count > 0)
				result = collection.Count > oldIndex ? collection[oldIndex] : collection[collection.Count - 1];
			return result;
		}
	}
	[UsesItemPolicy(typeof(RibbonControlHookPanelPolicy))]
	class RibbonControlDeleteItemTaskProvider : DeleteItemTaskProviderBase {
		protected override void OnDeleteKeyPressed(ModelItem selectedItem) {
			base.OnDeleteKeyPressed(selectedItem);
			if(selectedItem.IsItemOfType(typeof(BarItemLink))) BarManagerDesignTimeHelper.RemoveBarItemLink(selectedItem);
			if(selectedItem.IsItemOfType(typeof(RibbonPage))) RibbonDesignTimeHelper.RemoveRibbonPage(selectedItem);
			if(selectedItem.IsItemOfType(typeof(RibbonPageGroup))) RibbonDesignTimeHelper.RemoveRibbonPageGroup(selectedItem);
			if(selectedItem.IsItemOfType(typeof(RibbonPageCategory))) RibbonDesignTimeHelper.RemoveRibbonPageCategory(selectedItem);
		}
	}
}
