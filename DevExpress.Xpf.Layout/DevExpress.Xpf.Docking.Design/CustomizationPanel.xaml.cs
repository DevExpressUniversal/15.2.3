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

using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Design.UI;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Docking.Design {
	public partial class CustomizationPanel : UserControl {
		private DockLayoutManagerDesignTimeModel _Model;
		public DockLayoutManagerDesignTimeModel Model {
			get { return _Model; }
			set {
				if(_Model == value) return;
				_Model = value;
				OnModelChanged(value);
			}
		}
		public DockTypeEditor PART_DockTypeEditor { get; set; }
		public CustomizationPanel() {
			InitializeComponent();
			Background = Brushes.Transparent;
			Loaded += CustomizationPanel_Loaded;
			Unloaded += CustomizationPanel_Unloaded;
			SetMenuBindings(changeAviableTypeMenu, createItemButton);
			PART_DockTypeEditor = DockTypeEditorFactory.CreateDockTypeEditor();
			DockTypeEditorBorder.Child = PART_DockTypeEditor;
			PART_DockTypeEditor.EditValueChanged += DockTypeEditor_EditValueChanged;
		}
		private void OnModelChanged(DockLayoutManagerDesignTimeModel value) {
			DataContext = value;
		}
		void SetMenuBindings(ContextMenu menu, Button button) {
			menu.SetBinding(FrameworkElement.DataContextProperty, new Binding("DataContext") { Source = this });
			menu.SetBinding(FrameworkElement.MinWidthProperty, new Binding("ActualWidth") { Source = button });
			menu.PlacementTarget = button;
		}
		void SetTemplatePartBindings() {
			PART_CaptionText.SetBinding(TextBox.TextProperty, new Binding()
			{
				Path = new PropertyPath("(0).(1)[Caption].(2)", DockLayoutManagerDesignTimeModel.SelectedModelItemProperty,
					TypeDescriptor.GetProperties(typeof(ModelItem))["Properties"],
					TypeDescriptor.GetProperties(typeof(ModelProperty))["ComputedValue"]),
				Mode = BindingMode.TwoWay,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
				Converter = new CaptionConverter(),
			});
			PART_LayoutTreePanel.SetBinding(FrameworkElement.VisibilityProperty, new Binding()
			{
				Path = new PropertyPath(DockLayoutManagerDesignTimeModel.IsLayoutTreeExpanedProperty),
				Mode = BindingMode.TwoWay,
				UpdateSourceTrigger = UpdateSourceTrigger.LostFocus,
				Converter = new BoolToVisibilityConverter()
			});
		}
		void CustomizationPanel_Loaded(object sender, RoutedEventArgs e) {
			if(PART_LayoutTree.PartTreeView != null) {
				PART_LayoutTree.PartTreeView.BorderBrush = Brushes.Transparent;
				PART_LayoutTree.PartTreeView.SelectedItemChanged += PartTreeView_SelectedItemChanged;
				PART_LayoutTree.PartTreeView.PreviewMouseRightButtonDown += PartTreeView_PreviewMouseRightButtonDown;
			}
			savedLeft = (int)controlPanel.GetLeft();
			savedTop = (int)controlPanel.GetTop();
			RestoreProperties();
		}
		void CustomizationPanel_Unloaded(object sender, RoutedEventArgs e) {
			if(PART_LayoutTree.PartTreeView != null) {
				PART_LayoutTree.PartTreeView.SelectedItemChanged -= PartTreeView_SelectedItemChanged;
				PART_LayoutTree.PartTreeView.PreviewMouseRightButtonDown -= PartTreeView_PreviewMouseRightButtonDown;
			}
			StoreProperties();
		}
		void PartTreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
			TreeViewItem treeViewItem = LayoutHelper.FindParentObject<TreeViewItem>(e.OriginalSource as DependencyObject);
			if(treeViewItem != null) {
				treeViewItem.Focus();
				e.Handled = true;
			}
		}
		int savedLeft;
		int savedTop;
		bool locationChanged;
		void SetLocation(double left, double top) {
			controlPanel.SetLeft(left);
			controlPanel.SetTop(top);
			locationChanged = true;
		}
		internal void RestoreProperties(bool reset = false) {
#if !DEBUGTEST
			DesignerView view = DesignerView.FromContext(Model.Context);
			if(!WpfLayoutHelper.IsChildElement(view, controlPanel)) return;
			var service = Model.Context.Services.GetService<DockLayoutManagerDesignService>();
			if(service != null) {
				if(reset) {
					SetLocation(savedLeft, savedTop);
				}
				else {
					int left = service.RestoreIntProperty(StoredProperty.CustomizationPanelLeft);
					int top = service.RestoreIntProperty(StoredProperty.CustomizationPanelTop);
					Point p = controlPanel.TransformToVisual(view).Transform(new Point(left - controlPanel.GetLeft(), top - controlPanel.GetTop()));
					if(p.X + controlPanel.ActualWidth < view.ActualWidth && p.Y + controlPanel.ActualHeight < view.ActualHeight && p.X > 0 && p.Y > 0) {
						SetLocation(left, top);
					}
				}
				Model.IsAdornerPanelExpanded = service.RestoreBoolProperty(StoredProperty.CustomizationPanelIsExpanded);
				Model.IsLayoutTreeExpaned = service.RestoreBoolProperty(StoredProperty.CustomizationPanelIsLayoutTreeExpanded);
			}
#endif
		}
		void StoreProperties() {
#if !DEBUGTEST
			var service = Model.Context.Services.GetService<DockLayoutManagerDesignService>();
			if(service != null) {
				if(locationChanged) {
					service.StoreProperty(StoredProperty.CustomizationPanelLeft, (int)controlPanel.GetLeft());
					service.StoreProperty(StoredProperty.CustomizationPanelTop, (int)controlPanel.GetTop());
					locationChanged = false;
				}
				service.StoreProperty(StoredProperty.CustomizationPanelIsExpanded, Model.IsAdornerPanelExpanded);
				service.StoreProperty(StoredProperty.CustomizationPanelIsLayoutTreeExpanded, Model.IsLayoutTreeExpaned);
				service.Store();
			}
#endif
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			DockLayoutManager.SetDockLayoutManager(PART_LayoutTree, Model.Manager);
			SetTemplatePartBindings();
		}
		int lockSelection = 0;
		void PartTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			var lockService = Model.Context.Services.GetService<LockService>();
			if(lockSelection > 0 || lockService.IsLocked) return;
			lockSelection++;
			if(Model != null)
				Model.Select(e.NewValue as BaseLayoutItem);
			lockSelection--;
		}
		void DockTypeEditor_EditValueChanged(object sender, DockTypeEditorValueChangedEventArgs e) {
			DockTypeValue value = e.NewValue as DockTypeValue;
			HideDockTypeEditor();
			if(Model != null && value != null) {
				Model.AddItem(value);
			}
		}
		internal void HideDockTypeEditor() {
			dockTypePopup.IsOpen = false;
		}
		internal void SelectTreeViewItem(BaseLayoutItem item) {
			if(lockSelection > 0) return;
			lockSelection++;
			if(PART_LayoutTree.PartTreeView != null) {
				PART_LayoutTree.EnsureItemsSource(item);
				PART_LayoutTree.PartTreeView.SetSelectedItem(item);
			}
			lockSelection--;
		}
		void dragThumb_DragDelta(object sender, DragDeltaEventArgs e) {
			SetLocation(controlPanel.GetLeft() + e.HorizontalChange, controlPanel.GetTop() + e.VerticalChange);
		}
		public void EnsureModel(DockLayoutManagerDesignTimeModel model) {
			Model = model;
			if(PART_LayoutTree != null) {
				DockLayoutManager.SetDockLayoutManager(PART_LayoutTree, Model.Manager);
				PART_LayoutTree.EnsureItemsSource();
			}
		}
		void createItemButton_Click(object sender, RoutedEventArgs e) {
			if(dockTypePopup != null)
				dockTypePopup.IsOpen = true;
		}
	}
	static class TreeViewExtension {
		static public bool SetSelectedItem(this TreeView treeView, object item) {
			return SetSelected(treeView, item);
		}
		static private bool SetSelected(ItemsControl parent, object child) {
			if(parent == null || child == null) {
				return false;
			}
			TreeViewItem childNode = parent.ItemContainerGenerator.ContainerFromItem(child) as TreeViewItem;
			if(childNode != null) {
				childNode.BringIntoView();
				return childNode.IsSelected = true;
			}
			if(parent.Items.Count > 0) {
				foreach(object childItem in parent.Items) {
					ItemsControl childControl = parent.ItemContainerGenerator.ContainerFromItem(childItem) as ItemsControl;
					if(SetSelected(childControl, child)) {
						return true;
					}
				}
			}
			return false;
		}
	}
	internal class CaptionConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value is string) {
				return value;
			}
			return string.Empty;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
	}
	internal class LocalizerConverter : IValueConverter {
		public DockingStringId DockingStringId { get; set; }
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return DockingLocalizer.GetString(DockingStringId);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
	}
	internal class CreateNewCaptionFormatter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return string.Format(DockingLocalizer.GetString(DockingStringId.ButtonNewLayoutItemFormat), value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
	}
	public class LayoutTreeItemTemplateSelector : DataTemplateSelector {
		public DataTemplate LayoutGroupTemplate { get; set; }
		public DataTemplate LayoutItemTemplate { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			return item is LayoutGroup ? LayoutGroupTemplate : LayoutItemTemplate;
		}
	}
}
