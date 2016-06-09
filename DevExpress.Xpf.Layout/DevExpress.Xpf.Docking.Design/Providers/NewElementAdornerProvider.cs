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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Services;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using PlatformOrientation = System.Windows.Controls.Orientation;
namespace DevExpress.Xpf.Docking.Design {
	abstract class NewElementAdornerProvider : AdornerProvider {
		private bool _IsActive;
		private ModelItem _ModelItem;
		public bool IsActive {
			get { return _IsActive; }
			private set { _IsActive = value; }
		}
		public NewElementAdornerProvider() {
			var panel = new AdornerPanel();
			panel.Order = AdornerOrder.Foreground;
			panel.SnapsToDevicePixels = true;
			panel.UseLayoutRounding = true;
			CreateAdorners(panel);
			Adorners.Add(panel);
		}
		protected override void Activate(ModelItem item) {
			base.Activate(item);
			IsActive = true;
			ModelItem = item;
			UpdateAdorners();
			Context.Services.GetService<ModelService>().ModelChanged += OnModelChanged;
		}
		protected override void Deactivate() {
			IsActive = false;
			Context.Services.GetService<ModelService>().ModelChanged -= OnModelChanged;
			ModelItem = null;
			base.Deactivate();
		}
		private void OnModelChanged(object sender, ModelChangedEventArgs e) {
			UpdateAdorners();
		}
		private DockLayoutManagerDesignService DesignService {
			get { return Context.Services.GetService<DockLayoutManagerDesignService>(); }
		}
		protected ModelItem AddNewElement(ModelItem modelItem, int position, LayoutTypes newElementType) {
			ModelItem newElement = DesignService.CreateItem(newElementType);
			ModelItem newParent = null;
			if(modelItem.Is<DockLayoutManager>()) {
				ModelItem root = null;
				if(modelItem.Is<DockLayoutManager>() && !modelItem.Properties["LayoutRoot"].IsSet) {
					root = ModelFactory.CreateItem(Context, typeof(LayoutGroup), CreateOptions.InitializeDefaults, null);
					modelItem.Properties["LayoutRoot"].SetValue(root);
				}
				if(newElementType == LayoutTypes.Group) return root;
				newParent = root;
			}
			if(modelItem.Is<LayoutGroup>()) {
				newParent = modelItem;
			}
			if(modelItem.Is<LayoutPanel>()) {
				ModelItem parent = modelItem.Parent;
				if(parent.Is<TabbedGroup>()) {
					newParent = parent;
				}
				else {
					newParent = ModelFactory.CreateItem(Context, typeof(TabbedGroup), CreateOptions.InitializeDefaults, null);
					int index = parent.ItemsProperty().IndexOf(modelItem);
					parent.ItemsProperty().Insert(index, newParent);
					parent.ItemsProperty().Remove(modelItem);
					newParent.ItemsProperty().Add(modelItem);
				}
			}
			newParent.ItemsProperty().Insert(position < 0 ? newParent.ItemsProperty().Count : position, newElement);
			return newElement;
		}
		protected abstract ModelItem AddNewElement(LayoutGroupNewElementAdorner adorner, LayoutTypes newElementType);
		protected virtual LayoutGroupNewElementAdorner ResolveAdornerControl(Orientation orientation, PlacementMode placementMode) {
			return new LayoutGroupNewElementAdorner(orientation, placementMode);
		}
		protected virtual void CreateAdorners(AdornerPanel panel) {
			bool isWizardEnabled = DesignTimeHelper.GetIsWizardEnabled();
			AdornerCenter = ResolveAdornerControl(Orientation.Horizontal, PlacementMode.Bottom);
			AdornerCenter.AddNewElement = OnAddNewElement;
			AdornerPanel.SetAdornerHorizontalAlignment(AdornerCenter, AdornerHorizontalAlignment.Center);
			AdornerPanel.SetAdornerVerticalAlignment(AdornerCenter, isWizardEnabled ? AdornerVerticalAlignment.Top : AdornerVerticalAlignment.Center);
			panel.Children.Add(AdornerCenter);
		}
		protected void OnAddNewElement(LayoutGroupNewElementAdorner adorner, LayoutTypes newElementKind) {
			using(ModelEditingScope editingScope = ModelItem.Parent.BeginEdit("Add LayoutGroup(s) and/or LayoutItem")) {
				ModelItem newItem = AddNewElement(adorner, newElementKind);
				Context.Items.SetValue(new Microsoft.Windows.Design.Interaction.Selection(newItem));
				editingScope.Complete();
			}
		}
		void UpdateAdorners() {
			if(!IsActive) return;
			UpdateAdornersCore();
		}
		protected virtual void UpdateAdornersCore() { }
		protected ModelItem ModelItem {
			get { return _ModelItem; }
			private set {
				if(ModelItem == value) return;
				_ModelItem = value;
			}
		}
		protected LayoutGroupNewElementAdorner AdornerCenter { get; private set; }
	}
	abstract class SideNewElementAdornerProvider : NewElementAdornerProvider {
		protected override ModelItem AddNewElement(LayoutGroupNewElementAdorner adorner, LayoutTypes newElementKind) {
			if(adorner == AdornerCenter) return base.AddNewElement(ModelItem, -1, newElementKind);
			Side side = GetAdornerLocation(adorner);
			ModelItem parent = ModelItem.Parent;
			var parentOrientation = (PlatformOrientation)parent.Properties["Orientation"].ComputedValue;
			int index = parent.ItemsProperty().IndexOf(ModelItem);
			if(side.GetOrientation() == parentOrientation) {
				ModelItem newParent = ModelFactory.CreateItem(Context, typeof(LayoutGroup), null);
				newParent.Properties["Orientation"].SetValueIfNotEqual(parentOrientation.OrthogonalValue());
				parent.ItemsProperty().Insert(index, newParent);
				parent.ItemsProperty().Remove(ModelItem);
				newParent.ItemsProperty().Add(ModelItem);
				return AddNewElement(newParent, side.IsStart() ? 0 : 1, newElementKind);
			}
			else {
				ModelItem newItem = AddNewElement(parent, index + (side.IsStart() ? 0 : 1), newElementKind);
				return newItem;
			}
		}
		protected override void CreateAdorners(AdornerPanel panel) {
			base.CreateAdorners(panel);
			AdornerLeft = ResolveAdornerControl(Orientation.Vertical, PlacementMode.Left);
			AdornerLeft.AddNewElement = OnAddNewElement;
			AdornerPanel.SetAdornerHorizontalAlignment(AdornerLeft, AdornerHorizontalAlignment.OutsideLeft);
			AdornerPanel.SetAdornerVerticalAlignment(AdornerLeft, AdornerVerticalAlignment.Center);
			panel.Children.Add(AdornerLeft);
			AdornerRight = ResolveAdornerControl(Orientation.Vertical, PlacementMode.Right);
			AdornerRight.AddNewElement = OnAddNewElement;
			AdornerPanel.SetAdornerHorizontalAlignment(AdornerRight, AdornerHorizontalAlignment.OutsideRight);
			AdornerPanel.SetAdornerVerticalAlignment(AdornerRight, AdornerVerticalAlignment.Center);
			panel.Children.Add(AdornerRight);
			AdornerTop = ResolveAdornerControl(Orientation.Horizontal, PlacementMode.Top);
			AdornerTop.AddNewElement = OnAddNewElement;
			AdornerPanel.SetAdornerHorizontalAlignment(AdornerTop, AdornerHorizontalAlignment.Center);
			AdornerPanel.SetAdornerVerticalAlignment(AdornerTop, AdornerVerticalAlignment.OutsideTop);
			panel.Children.Add(AdornerTop);
			AdornerBottom = ResolveAdornerControl(Orientation.Horizontal, PlacementMode.Bottom);
			AdornerBottom.AddNewElement = OnAddNewElement;
			AdornerPanel.SetAdornerHorizontalAlignment(AdornerBottom, AdornerHorizontalAlignment.Center);
			AdornerPanel.SetAdornerVerticalAlignment(AdornerBottom, AdornerVerticalAlignment.OutsideBottom);
			panel.Children.Add(AdornerBottom);
		}
		private Side GetAdornerLocation(LayoutGroupNewElementAdorner adorner) {
			if(adorner == AdornerLeft)
				return Side.Left;
			if(adorner == AdornerRight)
				return Side.Right;
			if(adorner == AdornerTop)
				return Side.Top;
			if(adorner == AdornerBottom)
				return Side.Bottom;
			throw new ArgumentOutOfRangeException("adorner");
		}
		protected LayoutGroupNewElementAdorner AdornerLeft { get; set; }
		protected LayoutGroupNewElementAdorner AdornerRight { get; set; }
		protected LayoutGroupNewElementAdorner AdornerTop { get; set; }
		protected LayoutGroupNewElementAdorner AdornerBottom { get; set; }
	}
	[UsesItemPolicy(typeof(LayoutGroupParentWithInplaceLayoutBuilderSelectionPolicy))]
	class LayoutGroupChildAdornerProvider : SideNewElementAdornerProvider {
		protected override void UpdateAdornersCore() {
			base.UpdateAdornersCore();
			bool isInTabGroup = ModelItem.Parent.Is<TabbedGroup>();
			bool isInAutoHideGroup = ModelItem.Parent.Is<AutoHideGroup>();
			bool isInMDIGroup = ModelItem.Parent.Is<DocumentGroup>() && object.Equals(ModelItem.Parent.Properties["MDIStyle"].ComputedValue, MDIStyle.MDI);
			if(ModelItem.Is<DocumentGroup>() || ModelItem.Is<DocumentPanel>() || ModelItem.Parent.Is<DocumentGroup>()) {
				AdornerCenter.ButtonItem.Content = LayoutItemTypeInfo.DocumentTabTypeInfo;
			}
			else {
				if(ModelItem.Is<TabbedGroup>() || ModelItem.Is<LayoutPanel>())
					AdornerCenter.ButtonItem.Content = LayoutItemTypeInfo.PanelTabTypeInfo;
			}
			AdornerCenter.ShowOnlyButtonItem = ModelItem.Is<LayoutPanel>() || ModelItem.Is<TabbedGroup>();
			AdornerLeft.Visibility = AdornerTop.Visibility = AdornerRight.Visibility = AdornerBottom.Visibility =
				isInTabGroup || isInAutoHideGroup ? Visibility.Collapsed : Visibility.Visible;
			AdornerCenter.Visibility = isInAutoHideGroup || isInMDIGroup ? Visibility.Collapsed : Visibility.Visible;
		}
	}
	[UsesItemPolicy(typeof(ControlItemsHostParentWithInplaceLayoutBuilderSelectionPolicy))]
	class ControlItemsHostChildAdornerProvider : SideNewElementAdornerProvider {
		protected override void UpdateAdornersCore() {
			base.UpdateAdornersCore();
			AdornerCenter.Visibility = Visibility.Collapsed;
			if(ModelItem.Is<LayoutGroup>()) {
				if(object.Equals(ModelItem.Properties["GroupBorderStyle"].ComputedValue, GroupBorderStyle.Tabbed) || ModelItem.Properties["Items"].Collection.Count == 0) {
					AdornerCenter.Visibility = Visibility.Visible;
				}
				if(object.Equals(ModelItem.Properties["GroupBorderStyle"].ComputedValue, GroupBorderStyle.Tabbed)) {
					AdornerCenter.ShowOnlyButtonItem = true;
					AdornerCenter.ButtonItem.Content = LayoutItemTypeInfo.LayoutTabTypeInfo;
				}
			}
			if(ModelItem.Parent.Is<LayoutGroup>() && object.Equals(ModelItem.Parent.Properties["GroupBorderStyle"].ComputedValue, GroupBorderStyle.Tabbed)) {
				AdornerLeft.Visibility = AdornerTop.Visibility = AdornerRight.Visibility = AdornerBottom.Visibility = Visibility.Collapsed;
			}
		}
		protected override LayoutGroupNewElementAdorner ResolveAdornerControl(Orientation orientation, PlacementMode placementMode) {
			return new ControlItemsHostNewElementAdorner(orientation, placementMode);
		}
	}
	[UsesItemPolicy(typeof(InplaceLayoutBuilderSelectionPolicy))]
	class LayoutGroupAdornerProvider : NewElementAdornerProvider {
		protected override ModelItem AddNewElement(LayoutGroupNewElementAdorner adorner, LayoutTypes newElementKind) {
			return AddNewElement(ModelItem, -1, newElementKind);
		}
		protected override void UpdateAdornersCore() {
			base.UpdateAdornersCore();
			AdornerCenter.Visibility = ModelItem.Properties["Items"].Collection.Count > 0 || ModelItem.Is<AutoHideGroup>() ? Visibility.Collapsed : Visibility.Visible;
		}
	}
	[UsesItemPolicy(typeof(DockLayoutManagerPolicy))]
	class DockLayoutManagerNewElementAdornerProvider : NewElementAdornerProvider {
		protected override ModelItem AddNewElement(LayoutGroupNewElementAdorner adorner, LayoutTypes newElementKind) {
			return AddNewElement(ModelItem, -1, newElementKind);
		}
		protected override void UpdateAdornersCore() {
			base.UpdateAdornersCore();
			AdornerCenter.Visibility = ModelItem.Properties["LayoutRoot"].IsSet ? Visibility.Collapsed : Visibility.Visible;
		}
	}
}
