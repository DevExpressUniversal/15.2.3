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
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Layout.Core;
using Microsoft.Windows.Design.Model;
using System.Windows.Controls;
using GridLength = System.Windows.GridLength;
using GridUnitType = System.Windows.GridUnitType;
using DevExpress.Xpf.Core.Design.SmartTags;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Docking.Design.SmartTag.ViewModels {
	sealed class DockLayoutManagerPropertyLinesProvider : PropertyLinesProviderBase {
		public DockLayoutManagerPropertyLinesProvider()
			: base(typeof(DockLayoutManager)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DockLayoutManager.FloatingModeProperty), typeof(FloatingMode)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DockLayoutManager.ItemsSourceProperty)));
			lines.Add(() => new ActionListPropertyLineViewModel(new CreateLayoutActionListPropertyLineContext(viewModel)) { Text = "Generate predefined layout" });
			return lines;
		}
		class CreateLayoutActionListPropertyLineContext : ActionListPropertyLineContext {
			public CreateLayoutActionListPropertyLineContext(IPropertyLineContext context)
				: base(context) {
			}
			protected override void InitializeItems() {
				ObservableCollection<MenuItemInfo> items = new ObservableCollection<MenuItemInfo>();
				items.Add(new MenuItemInfo() { Caption = "Dock panels", Command = SetSelectedItemCommand, Tag = DefaultLayoutType.Simple });
				items.Add(new MenuItemInfo() { Caption = "IDE-inspired UI", Command = SetSelectedItemCommand, Tag = DefaultLayoutType.IDE });
				items.Add(new MenuItemInfo() { Caption = "Multiple Document Interface", Command = SetSelectedItemCommand, Tag = DefaultLayoutType.MDI });
				SelectedItem = items[0];
				Items = items;
			}
			protected override void OnSelectedItemExecute(MenuItemInfo param) {
				XpfModelItem owner = Context.ModelItem as XpfModelItem;
				if(owner != null) {
					var modelItem = owner.Value;
					modelItem.Context.Services.GetService<DockLayoutManagerDesignService>().RestoreLayout(modelItem, (DefaultLayoutType)SelectedItem.Tag);
				}
			}
		}
	}
	abstract class DockingPropertyLinesProviderBase<T> : PropertyLinesProviderBase {
		public DockingPropertyLinesProviderBase()
			: base(typeof(T)) {
		}
	}
	sealed class BaseLayoutItemPropertyLinesProvider : DockingPropertyLinesProviderBase<BaseLayoutItem> {
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			var parent = viewModel.GetParent();
			var itemTypeProperty = parent.Is<LayoutGroup>() ? parent.Properties.FindProperty("ItemType", typeof(LayoutGroup)) : null;
			if(itemTypeProperty != null)
				if(parent.Is<LayoutGroup>() && object.Equals(itemTypeProperty.ComputedValue, LayoutItemType.Group)) {
					if(object.Equals(viewModel.GetParentRuntimeProperty("Orientation"), Orientation.Horizontal))
						lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutPanel.ItemWidthProperty)) { ItemsSource = new List<object> { GridLength.Auto, new GridLength(1, GridUnitType.Star) } });
					else
						lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutPanel.ItemHeightProperty)) { ItemsSource = new List<object> { GridLength.Auto, new GridLength(1, GridUnitType.Star) } });
				}
			return lines;
		}
	}
	sealed class ContentItemPropertyLinesProvider : DockingPropertyLinesProviderBase<ContentItem> {
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutPanel.CaptionProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutPanel.ContentProperty)));
			return lines;
		}
	}
	sealed class LayoutPanelPropertyLinesProvider : DockingPropertyLinesProviderBase<LayoutPanel> {
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutPanel.CloseCommandProperty)));
			IModelItem runtimeParent = viewModel.RuntimeSelectedItem.Parent;
			if(runtimeParent != null && object.Equals(runtimeParent.ItemType, typeof(DocumentGroup))) {
				if(!object.Equals(runtimeParent.Properties["MDIStyle"].ComputedValue, MDIStyle.MDI)) {
					lines.Add(() => new ColorPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutPanel.TabBackgroundColorProperty)));
					lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DocumentGroup.PinnedProperty), typeof(DocumentGroup)));
					lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DocumentGroup.ShowPinButtonProperty), typeof(DocumentGroup)));
					lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DocumentGroup.PinLocationProperty), typeof(TabHeaderPinLocation), typeof(DocumentGroup)));
				}
			}
			return lines;
		}
	}
	sealed class DocumentPanelPropertyLinesProvider : DockingPropertyLinesProviderBase<DocumentPanel> {
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			IModelItem runtimeParent = viewModel.RuntimeSelectedItem.Parent;
			if(runtimeParent != null && object.Equals(runtimeParent.ItemType, typeof(DocumentGroup))) {
				if(object.Equals(runtimeParent.Properties["MDIStyle"].ComputedValue, MDIStyle.MDI)) {
					lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DocumentPanel.MDILocationProperty)));
					lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DocumentPanel.MDISizeProperty)));
				}
			}
			return lines;
		}
	}
	abstract class LayoutGroupPropertyLinesProviderBase : PropertyLinesProviderBase {
		protected LayoutGroupPropertyLinesProviderBase(Type itemType)
			: base(itemType) {
		}
		protected bool IsOrientedGroup(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			var itemTypeProperty = viewModel.RuntimeSelectedItem.Properties.FindProperty("ItemType", typeof(LayoutGroup));
			if(itemTypeProperty == null) return false;
			return object.Equals(viewModel.GetRuntimeProperty("ItemType"), LayoutItemType.Group) &&
				!object.Equals(viewModel.GetRuntimeProperty("GroupBorderStyle"), GroupBorderStyle.Tabbed);
		}
		protected bool IsTabbedGroup(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			var itemTypeProperty = viewModel.RuntimeSelectedItem.Properties.FindProperty("ItemType", typeof(LayoutGroup));
			if(itemTypeProperty == null) return false;
			LayoutItemType itemType = (LayoutItemType)viewModel.GetRuntimeProperty("ItemType");
			switch(itemType) {
				case LayoutItemType.Group:
					return !IsOrientedGroup(viewModel);
				case LayoutItemType.TabPanelGroup:
					return true;
				case LayoutItemType.DocumentPanelGroup:
					return !object.Equals(viewModel.GetRuntimeProperty("MDIStyle"), MDIStyle.MDI);
			}
			return false;
		}
	}
	sealed class LayoutGroupPropertyLinesProvider : LayoutGroupPropertyLinesProviderBase {
		public LayoutGroupPropertyLinesProvider()
			: base(typeof(LayoutGroup)) {
		}
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutGroup.ItemsSourceProperty)));
			if(IsOrientedGroup(viewModel)) {
				lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutGroup.OrientationProperty), typeof(Orientation)));
			}
			if(IsTabbedGroup(viewModel)) {
				lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutGroup.SelectedTabIndexProperty)));
				lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutGroup.TabHeaderLayoutTypeProperty), typeof(TabHeaderLayoutType)));
				lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutGroup.TabContentCacheModeProperty), typeof(TabContentCacheMode)));
			}
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => LayoutGroup.DestroyOnClosingChildrenProperty)));
			return lines;
		}
	}
	sealed class DocumentGroupPropertyLinesProvider : LayoutGroupPropertyLinesProviderBase {
		public DocumentGroupPropertyLinesProvider()
			: base(typeof(DocumentGroup)) {
		}
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DocumentGroup.MDIStyleProperty), typeof(MDIStyle)));
			if(IsTabbedGroup(viewModel)) {
				lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DocumentGroup.CaptionLocationProperty), typeof(CaptionLocation)));
				lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DocumentGroup.CaptionOrientationProperty), typeof(Orientation)));
				lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => DocumentGroup.ClosePageButtonShowModeProperty), typeof(ClosePageButtonShowMode)));
			}
			return lines;
		}
	}
	static class DesignTimeViewModelBaseExtensions {
		public static object GetRuntimeProperty(this DesignTimeViewModelBase viewModel, string name) {
			return viewModel.RuntimeSelectedItem.Properties[name].ComputedValue;
		}
		public static object GetParentRuntimeProperty(this DesignTimeViewModelBase viewModel, string name) {
			return viewModel.RuntimeSelectedItem.Parent.Properties[name].ComputedValue;
		}
		public static IModelItem GetParent(this DesignTimeViewModelBase viewModel) {
			return viewModel.RuntimeSelectedItem.Parent;
		}
		public static bool Is<T>(this IModelItem item) {
			if(item == null) return false;
			return typeof(T).IsAssignableFrom(item.ItemType);
		}
		public static IModelItemCollection GetCollectionProperty(this IModelItem item, string propertyName) {
			return item.Properties[propertyName].Collection;
		}
	}
}
