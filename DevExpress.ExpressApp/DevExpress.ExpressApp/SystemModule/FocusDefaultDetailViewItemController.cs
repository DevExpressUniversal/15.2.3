#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using System.ComponentModel;
namespace DevExpress.ExpressApp.SystemModule {
	public class CustomFocusDetailViewItemEventArgs : EventArgs {
		private DetailView detailView;
		private ViewItem detailViewItemToFocus;
		public CustomFocusDetailViewItemEventArgs(DetailView detailView) {
			this.detailView = detailView;
		}
		public DetailView DetailView {
			get { return detailView; }
		}
		public ViewItem DetailViewItemToFocus {
			get { return detailViewItemToFocus; }
			set {
				detailViewItemToFocus = value;
			}
		}
	}
	public class FocusDefaultDetailViewItemController : ViewController<DetailView>, IModelExtender {
		protected internal ViewItem defaultItem;
		protected IList<IModelLayoutGroup> tabPageNodesWithDefaultItemList;
		private void View_ControlsCreating(object sender, EventArgs e) {
			OnViewControlsCreating();
		}
		private ViewItem GetDefaultDetailViewItem() {
			CustomFocusDetailViewItemEventArgs args = new CustomFocusDetailViewItemEventArgs(View);
			if(View != null && View.Model != null) {
				IModelViewItem defaultItemModel = ((IModelDetailViewDefaultFocusedItem)View.Model).DefaultFocusedItem;
				if(defaultItemModel != null) {
					args.DetailViewItemToFocus = View.FindItem(defaultItemModel.Id);
				}
			}
			OnCustomFocusDetailViewItem(args);
			return args.DetailViewItemToFocus;
		}
		private static IModelViewLayoutElement GetLayoutItemWithSpecifiedID<T>(string id, IModelList<T> nodesToSearch) where T : IModelViewLayoutElement {
			IModelViewLayoutElement result = null;
			foreach(IModelViewLayoutElement childNode in nodesToSearch) {
				IModelLayoutViewItem layoutItem = childNode as IModelLayoutViewItem;
				if(layoutItem is IModelLayoutViewItem) {
					if(string.IsNullOrEmpty(id) || layoutItem.Id == id) {
						result = layoutItem;
					}
				}
				else if(childNode is IModelLayoutGroup) {
					result = GetLayoutItemWithSpecifiedID<IModelViewLayoutElement>(id, (IModelLayoutGroup)childNode);
				}
				else if(childNode is IModelTabbedGroup) {
					result = GetLayoutItemWithSpecifiedID<IModelLayoutGroup>(id, (IModelTabbedGroup)childNode);
				}
				if(result != null) {
					break;
				}
			}
			return result;
		}
		private IList<IModelLayoutGroup> GetTabPageNodesWithDefaultItemList() {
			IModelLayoutViewItem defaultItemLayoutNode = (IModelLayoutViewItem)GetLayoutItemWithSpecifiedID<IModelViewLayoutElement>(defaultItem.Id, View.Model.Layout);
			List<IModelLayoutGroup> result = new List<IModelLayoutGroup>();
			IModelViewLayoutElement currentNode = defaultItemLayoutNode;
			if(defaultItemLayoutNode != null) {
				while(!(currentNode.Parent is IModelViewLayout)) {
					if(currentNode.Parent is IModelTabbedGroup && ((IModelTabbedGroup)currentNode.Parent).Direction == FlowDirection.Horizontal) {
						result.Insert(0, (IModelLayoutGroup)currentNode);
					}
					currentNode = (IModelViewLayoutElement)currentNode.Parent;
				}
			}
			return result;
		}
		protected virtual void OnViewControlsCreating() { }
		protected virtual void FocusDefaultItemControlCore() { }
		protected string GetFullPathNode(IModelViewLayoutElement node) {
			string result = "";
			IModelNode currentNode = node;
			while(!(currentNode is IModelViewLayout)) {
				if(currentNode is IModelViewLayoutElement) {
					result = ((IModelViewLayoutElement)currentNode).Id + "\\" + result;
				}
				currentNode = currentNode.Parent;
			}
			return result;
		}
		protected bool NeedActivateDefaultTab() {
			bool result = false;
			if(tabPageNodesWithDefaultItemList != null && tabPageNodesWithDefaultItemList.Count > 0) {
				if(View.IsRoot) {
					result = true;
				}
				else if(Frame is NestedFrame) {
					IModelDetailViewDefaultFocusedItem parentViewModel = ((NestedFrame)Frame).ViewItem.View.Model as IModelDetailViewDefaultFocusedItem;
					result = parentViewModel != null && parentViewModel.DefaultFocusedItem != null && parentViewModel.DefaultFocusedItem.Id == ((NestedFrame)Frame).ViewItem.Id;					
				}
			}
			return result;
		}
		protected void FocusDefaultItemControl() {
			if(defaultItem != null) {
				DetailPropertyEditor detailPropertyEditor = defaultItem as DetailPropertyEditor;
				if(detailPropertyEditor != null && detailPropertyEditor.Frame != null) { 
					FocusDefaultDetailViewItemController controller = detailPropertyEditor.Frame.GetController<FocusDefaultDetailViewItemController>();
					if(controller != null) {
						controller.FocusDefaultItemControl();
					}
				}
				else {
					FocusDefaultItemControlCore();
				}
			}
		}
		protected virtual void OnCustomFocusDetailViewItem(CustomFocusDetailViewItemEventArgs args) {
			if(CustomFocusDetailViewItem != null) {
				CustomFocusDetailViewItem(this, args);
			}
		}
		protected override void OnActivated() {
			View.ControlsCreating += new EventHandler(View_ControlsCreating);
			View.ModelLoaded += new EventHandler(View_ModelLoaded);
			View.ItemsChanged += new EventHandler<ViewItemsChangedEventArgs>(View_ItemsChanged);
			base.OnActivated();
			InitializeDefaultItem();
		}
		private void View_ItemsChanged(object sender, ViewItemsChangedEventArgs e) {
			InitializeDefaultItem();
		}
		private void InitializeDefaultItem() {
			defaultItem = GetDefaultDetailViewItem();
			if(defaultItem != null) {
				tabPageNodesWithDefaultItemList = GetTabPageNodesWithDefaultItemList();
			}
		}
		private void View_ModelLoaded(object sender, EventArgs e) {
			InitializeDefaultItem();
		}
		protected override void OnDeactivated() {
			View.ItemsChanged -= new EventHandler<ViewItemsChangedEventArgs>(View_ItemsChanged);
			View.ControlsCreating -= new EventHandler(View_ControlsCreating);
			View.ModelLoaded -= new EventHandler(View_ModelLoaded);
			defaultItem = null;
			if(tabPageNodesWithDefaultItemList != null) {
				tabPageNodesWithDefaultItemList.Clear();
				tabPageNodesWithDefaultItemList = null;
			}
			base.OnDeactivated();
		}
		public static IModelViewLayoutElement GetFirstLayoutItem<T>(IModelList<T> nodesToSearch) where T : IModelViewLayoutElement {
			return GetLayoutItemWithSpecifiedID<T>(string.Empty, nodesToSearch);
		}
		public event EventHandler<CustomFocusDetailViewItemEventArgs> CustomFocusDetailViewItem;
		#region IExtendModel Members
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelDetailView, IModelDetailViewDefaultFocusedItem>();
		}
		#endregion
	}
	public interface IModelDetailViewDefaultFocusedItem {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelDetailViewDefaultFocusedItemDefaultFocusedItem"),
#endif
 Category("Behavior")]
		[DataSourceProperty("Items")]
		IModelViewItem DefaultFocusedItem { get; set; }
	}
	[DomainLogic(typeof(IModelDetailViewDefaultFocusedItem))]
	public static class ModelDetailViewDefaultFocusedItemLogic {
		public static IModelViewItem Get_DefaultFocusedItem(IModelDetailViewDefaultFocusedItem model) {
			IModelViewItem result = new ModelValuePersistentPathCalculator().Calculate((ModelNode)model, "DefaultFocusedItem") as IModelViewItem;
			if(result != null) return result;
			IModelDetailView detailViewModel = model as IModelDetailView;
			IModelViewLayout layoutNode = detailViewModel.Layout;
			IModelViewLayoutElement firstLayoutItem = null;
			if(layoutNode != null) {
				firstLayoutItem = FocusDefaultDetailViewItemController.GetFirstLayoutItem<IModelViewLayoutElement>(layoutNode);
			}
			if(firstLayoutItem != null) {
				result = detailViewModel.Items[firstLayoutItem.Id];
			}
			return result;
		}
	}
}
