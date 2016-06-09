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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Platform::DevExpress.Xpf.Core;
using Platform::DevExpress.Xpf.Core.Native;
using Platform::DevExpress.Xpf.LayoutControl;
namespace DevExpress.Xpf.LayoutControl.Design {
	enum LayoutGroupNewElementKind { Group, TabbedGroup, Item }
	abstract class LayoutGroupNewElementAdornerProvider : AdornerProvider {
		private ModelItem _ModelItem;
		public LayoutGroupNewElementAdornerProvider() {
			var panel = new AdornerPanel();
			panel.Order = AdornerOrder.Foreground;
			panel.SnapsToDevicePixels = true;
			panel.UseLayoutRounding = true;
			CreateAdorners(panel);
			Adorners.Add(panel);
		}
		protected override void Activate(ModelItem item) {
			base.Activate(item);
			ModelItem = item;
			UpdateAdorners();
		}
		protected override void Deactivate() {
			base.Deactivate();
			ModelItem = null;
		}
		protected ModelItem AddNewElement(ModelItem parent, int position, LayoutGroupNewElementKind newElementKind) {
			Type newElementType = GetNewElementType(newElementKind);
			ModelItem newElement = ModelFactory.CreateItem(Context, newElementType, CreateOptions.InitializeDefaults, null);
			if (position == -1)
				parent.Properties["Children"].Collection.Add(newElement);
			else
				parent.Properties["Children"].Collection.Insert(position, newElement);
			LayoutGroupParentAdapter.InitializeChild(parent, newElement);
			if (newElementKind == LayoutGroupNewElementKind.TabbedGroup) {
				newElement.Properties["View"].SetValue(LayoutGroupView.Tabs);
				ModelItem tab = ModelFactory.CreateItem(Context, newElementType, CreateOptions.InitializeDefaults, null);
				newElement.Properties["Children"].Collection.Add(tab);
				LayoutGroupParentAdapter.InitializeChild(newElement, tab);
			}
			return newElement;
		}
		protected abstract ModelItem AddNewElement(LayoutGroupNewElementAdorner adorner, LayoutGroupNewElementKind newElementKind);
		protected abstract void CreateAdorners(AdornerPanel panel);
		protected void OnAddNewElement(LayoutGroupNewElementAdorner adorner, LayoutGroupNewElementKind newElementKind) {
			using (ModelEditingScope editingScope = ModelItem.Parent.BeginEdit("Add LayoutGroup(s) and/or LayoutItem")) {
				ModelItem newItem = AddNewElement(adorner, newElementKind);
				Context.Items.SetValue(new Selection(newItem));
				editingScope.Complete();
			}
		}
		protected virtual void UpdateAdorners() { }
		protected ModelItem ModelItem {
			get { return _ModelItem; }
			private set {
				if (ModelItem == value)
					return;
				if (ModelItem != null && LayoutControl != null) {
					LayoutControl.StartDrag -= OnLayoutControlStartDrag;
					LayoutControl.EndDrag -= OnLayoutControlEndDrag;
					LayoutControl = null;
				}
				_ModelItem = value;
				if (ModelItem != null) {
					ModelItem layoutControlItem = ModelItem.GetLayoutControl(false);
					if (layoutControlItem != null) {
						LayoutControl = (Platform::DevExpress.Xpf.LayoutControl.LayoutControl)layoutControlItem.GetCurrentValue();
						LayoutControl.StartDrag += OnLayoutControlStartDrag;
						LayoutControl.EndDrag += OnLayoutControlEndDrag;
					}
				}
			}
		}
		private Type GetNewElementType(LayoutGroupNewElementKind newElementKind) {
			switch (newElementKind) {
				case LayoutGroupNewElementKind.Group:
				case LayoutGroupNewElementKind.TabbedGroup:
					return typeof(LayoutGroup);
				case LayoutGroupNewElementKind.Item:
					return typeof(LayoutItem);
				default:
					return null;
			}
		}
		private void OnLayoutControlStartDrag(object sender, EventArgs e) {
			AdornersVisible = false;
		}
		private void OnLayoutControlEndDrag(object sender, EventArgs e) {
			AdornersVisible = true;
		}
		private Platform::DevExpress.Xpf.LayoutControl.LayoutControl LayoutControl { get; set; }
	}
	class LayoutGroupParentWithInplaceLayoutBuilderSelectionPolicy : LayoutGroupParentSelectionPolicy {
		protected override IEnumerable<ModelItem> GetPolicyItems(Selection selection) {
			return InplaceLayoutBuilderSelectionPolicy.GetItems(Context, base.GetPolicyItems(selection), false);
		}
	}
	[UsesItemPolicy(typeof(LayoutGroupParentWithInplaceLayoutBuilderSelectionPolicy))]
	class LayoutGroupChildAdornerProvider : LayoutGroupNewElementAdornerProvider {
		protected override ModelItem AddNewElement(LayoutGroupNewElementAdorner adorner, LayoutGroupNewElementKind newElementKind) {
			Side side = GetAdornerLocation(adorner);
			ModelItem parent = ModelItem.Parent;
			var parentOrientation = (Platform::System.Windows.Controls.Orientation)parent.Properties["Orientation"].ComputedValue;
			int index = parent.Properties["Children"].Collection.IndexOf(ModelItem);
			if (!parent.IsTabbedLayoutGroup() && side.GetOrientation() == parentOrientation) {
				ModelItem newParent = ModelFactory.CreateItem(Context, typeof(LayoutGroup), null);
				newParent.Properties["Orientation"].SetValueIfNotEqual(parentOrientation.OrthogonalValue());
				InitAlignment(newParent);
				parent.Properties["Children"].Collection.Insert(index, newParent);
				parent.Properties["Children"].Collection.Remove(ModelItem);
				newParent.Properties["Children"].Collection.Add(ModelItem);
				return AddNewElement(newParent, side.IsStart() ? 0 : 1, newElementKind);
			}
			else {
				ModelItem newItem = AddNewElement(parent, index + (side.IsStart() ? 0 : 1), newElementKind);
				if (!parent.IsTabbedLayoutGroup())
					InitAlignment(newItem);
				return newItem;
			}
		}
		protected override void CreateAdorners(AdornerPanel panel) {
			AdornerLeft = new LayoutGroupNewElementAdorner(Orientation.Vertical, PlacementMode.Left);
			AdornerLeft.AddNewElement = OnAddNewElement;
			AdornerPanel.SetAdornerHorizontalAlignment(AdornerLeft, AdornerHorizontalAlignment.OutsideLeft);
			AdornerPanel.SetAdornerVerticalAlignment(AdornerLeft, AdornerVerticalAlignment.Center);
			panel.Children.Add(AdornerLeft);
			AdornerRight = new LayoutGroupNewElementAdorner(Orientation.Vertical, PlacementMode.Right);
			AdornerRight.AddNewElement = OnAddNewElement;
			AdornerPanel.SetAdornerHorizontalAlignment(AdornerRight, AdornerHorizontalAlignment.OutsideRight);
			AdornerPanel.SetAdornerVerticalAlignment(AdornerRight, AdornerVerticalAlignment.Center);
			panel.Children.Add(AdornerRight);
			AdornerTop = new LayoutGroupNewElementAdorner(Orientation.Horizontal, PlacementMode.Top);
			AdornerTop.AddNewElement = OnAddNewElement;
			AdornerPanel.SetAdornerHorizontalAlignment(AdornerTop, AdornerHorizontalAlignment.Center);
			AdornerPanel.SetAdornerVerticalAlignment(AdornerTop, AdornerVerticalAlignment.OutsideTop);
			panel.Children.Add(AdornerTop);
			AdornerBottom = new LayoutGroupNewElementAdorner(Orientation.Horizontal, PlacementMode.Bottom);
			AdornerBottom.AddNewElement = OnAddNewElement;
			AdornerPanel.SetAdornerHorizontalAlignment(AdornerBottom, AdornerHorizontalAlignment.Center);
			AdornerPanel.SetAdornerVerticalAlignment(AdornerBottom, AdornerVerticalAlignment.OutsideBottom);
			panel.Children.Add(AdornerBottom);
		}
		protected override void UpdateAdorners() {
			base.UpdateAdorners();
			bool isTab = ModelItem.Parent.IsTabbedLayoutGroup();
			AdornerTop.Visibility = AdornerBottom.Visibility =
				isTab ? Visibility.Collapsed : Visibility.Visible;
			AdornerLeft.NewElementsVisibility = AdornerRight.NewElementsVisibility =
				isTab ? LayoutGroupNewElementsVisibility.GroupOnly : LayoutGroupNewElementsVisibility.All;
			AdornerLeft.ShowGroupAsTab = AdornerRight.ShowGroupAsTab = isTab;
		}
		private Side GetAdornerLocation(LayoutGroupNewElementAdorner adorner) {
			if (adorner == AdornerLeft)
				return Side.Left;
			if (adorner == AdornerRight)
				return Side.Right;
			if (adorner == AdornerTop)
				return Side.Top;
			if (adorner == AdornerBottom)
				return Side.Bottom;
			throw new ArgumentOutOfRangeException("adorner");
		}
		private void InitAlignment(ModelItem newItem) {
			bool isHorizontal = ModelItem.Parent.Properties["Orientation"].ComputedValue.Equals(Platform::System.Windows.Controls.Orientation.Horizontal);
			string alignmentPropertyName = isHorizontal ? "HorizontalAlignment" : "VerticalAlignment";
			object alignment = ModelItem.Properties[alignmentPropertyName].ComputedValue;
			ItemAlignment itemAlignment = isHorizontal ?
				((Platform::System.Windows.HorizontalAlignment)alignment).GetItemAlignment() :
				((Platform::System.Windows.VerticalAlignment)alignment).GetItemAlignment();
			if (itemAlignment == ItemAlignment.Center || itemAlignment == ItemAlignment.End)
				newItem.Properties[alignmentPropertyName].SetValue(alignment);
		}
		private LayoutGroupNewElementAdorner AdornerLeft { get; set; }
		private LayoutGroupNewElementAdorner AdornerRight { get; set; }
		private LayoutGroupNewElementAdorner AdornerTop { get; set; }
		private LayoutGroupNewElementAdorner AdornerBottom { get; set; }
	}
	class InplaceLayoutBuilderSelectionPolicy : PrimarySelectionPolicy {
		public static IEnumerable<ModelItem> GetItems(EditingContext context, IEnumerable<ModelItem> items, bool checkItemOwnValue) {
			LayoutControlDesignService service = context.Services.GetService<LayoutControlDesignService>();
			if (service != null) {
				AttachToIsInplaceLayoutBuilderVisibleChanged(service);
				foreach (ModelItem item in items) {
					if (item.View == null)
						continue;
					ModelItem layoutControl = item.GetLayoutControl(checkItemOwnValue);
					if (service.GetIsInplaceLayoutBuilderVisible(layoutControl))
						yield return item;
				}
			}
		}
		static void AttachToIsInplaceLayoutBuilderVisibleChanged(LayoutControlDesignService service) {
			service.IsInplaceLayoutBuilderVisibleChanged -= OnIsInplaceLayoutBuilderVisibleChanged;
			service.IsInplaceLayoutBuilderVisibleChanged += OnIsInplaceLayoutBuilderVisibleChanged;
		}
		static void OnIsInplaceLayoutBuilderVisibleChanged(LayoutControlDesignService service, ModelItem layoutControlModelItem) {
			service.Context.Items.SetValue(service.Context.Items.GetValue<Selection>());
		}
		protected override IEnumerable<ModelItem> GetPolicyItems(Selection selection) {
			return GetItems(Context, base.GetPolicyItems(selection), true);
		}
	}
	[UsesItemPolicy(typeof(InplaceLayoutBuilderSelectionPolicy))]
	class LayoutGroupAdornerProvider : LayoutGroupNewElementAdornerProvider {
		protected override ModelItem AddNewElement(LayoutGroupNewElementAdorner adorner, LayoutGroupNewElementKind newElementKind) {
			return AddNewElement(ModelItem, -1, newElementKind);
		}
		protected override void CreateAdorners(AdornerPanel panel) {
			AdornerCenter = new LayoutGroupNewElementAdorner(Orientation.Horizontal, PlacementMode.Bottom);
			AdornerCenter.AddNewElement = OnAddNewElement;
			AdornerPanel.SetAdornerHorizontalAlignment(AdornerCenter, AdornerHorizontalAlignment.Center);
			AdornerPanel.SetAdornerVerticalAlignment(AdornerCenter, AdornerVerticalAlignment.Center);
			panel.Children.Add(AdornerCenter);
		}
		protected override void UpdateAdorners() {
			base.UpdateAdorners();
			AdornerCenter.Visibility = ModelItem.IsEmptyPanel() ? Visibility.Visible : Visibility.Collapsed;
		}
		private LayoutGroupNewElementAdorner AdornerCenter { get; set; }
	}
}
