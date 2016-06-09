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

using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public partial class DesignTimeTreeViewResources : ResourceDictionary {
		public DesignTimeTreeViewResources() {
			InitializeComponent();
		}
	}
	public class DesignTimeTreeViewItem : TreeViewItem {
		#region Dependency Properties
		public static readonly DependencyProperty MouseDoubleClickCommandProperty =
			DependencyPropertyManager.Register("MouseDoubleClickCommand", typeof(ICommand), typeof(DesignTimeTreeViewItem), new PropertyMetadata(null));
		public static readonly DependencyProperty MouseDoubleClickCommandParameterProperty =
			DependencyPropertyManager.Register("MouseDoubleClickCommandParameter", typeof(object), typeof(DesignTimeTreeViewItem), new PropertyMetadata(null));
		public static readonly DependencyProperty ExpanderMinWidthProperty =
			DependencyProperty.Register("ExpanderMinWidth", typeof(double), typeof(DesignTimeTreeViewItem), new PropertyMetadata(19d));
		public static readonly DependencyProperty ShowExpanderProperty =
			DependencyPropertyManager.Register("ShowExpander", typeof(bool), typeof(DesignTimeTreeViewItem), new PropertyMetadata(true,
				(d, e) => ((DesignTimeTreeViewItem)d).OnShowExpanderChanged(e)));
		public static readonly DependencyProperty CanBeSelectedProperty =
			DependencyPropertyManager.Register("CanBeSelected", typeof(bool), typeof(DesignTimeTreeViewItem), new PropertyMetadata(true,
				(d, e) => ((DesignTimeTreeViewItem)d).OnCanBeSelectedChanged(e)));
		static readonly DependencyProperty IsSelectedListenProperty =
			DependencyProperty.Register("IsSelectedListen", typeof(bool), typeof(DesignTimeTreeViewItem), new PropertyMetadata(false,
				(d, e) => ((DesignTimeTreeViewItem)d).OnIsSelectedChanged(e)));
		#endregion
		public DesignTimeTreeViewItem() {
			SetBinding(IsSelectedListenProperty, new Binding("IsSelected") { Source = this, Mode = BindingMode.OneWay });
		}
		public ICommand MouseDoubleClickCommand { get { return (ICommand)GetValue(MouseDoubleClickCommandProperty); } set { SetValue(MouseDoubleClickCommandProperty, value); } }
		public object MouseDoubleClickCommandParameter { get { return GetValue(MouseDoubleClickCommandParameterProperty); } set { SetValue(MouseDoubleClickCommandParameterProperty, value); } }
		public double ExpanderMinWidth { get { return (double)GetValue(ExpanderMinWidthProperty); } set { SetValue(ExpanderMinWidthProperty, value); } }
		public bool ShowExpander { get { return (bool)GetValue(ShowExpanderProperty); } set { SetValue(ShowExpanderProperty, value); } }
		public bool CanBeSelected { get { return (bool)GetValue(CanBeSelectedProperty); } set { SetValue(CanBeSelectedProperty, value); } }
		protected internal DesignTimeTreeView TreeView { get; set; }
		protected ToggleButton PART_Expander { get; private set; }
		protected FrameworkElement PART_Header { get; private set; }
		protected override void OnMouseDoubleClick(MouseButtonEventArgs e) {
			if(MouseDoubleClickCommand != null) {
				DependencyObject originalSource = e.OriginalSource as DependencyObject;
				DependencyObject source = originalSource == null ? null : LayoutHelper.FindLayoutOrVisualParentObject(originalSource, d => d == PART_Header || d == this, false);
				if(source == PART_Header && MouseDoubleClickCommand.CanExecute(MouseDoubleClickCommandParameter)) {
					if(!(ShowExpander && !IsExpanded)) {
						MouseDoubleClickCommand.Execute(MouseDoubleClickCommandParameter);
						e.Handled = true;
					}
				}
			}
			if(!ShowExpander)
				e.Handled = true;
			base.OnMouseDoubleClick(e);
		}
		protected virtual void OnShowExpanderChanged(DependencyPropertyChangedEventArgs e) { UpdateExpanderVisibility(); }
		protected override DependencyObject GetContainerForItemOverride() {
			return new DesignTimeTreeViewItem();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is DesignTimeTreeViewItem;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			DesignTimeTreeViewItem treeViewItem = (DesignTimeTreeViewItem)element;
			treeViewItem.TreeView = TreeView;
		}
		protected virtual void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e) {
			bool newValue = (bool)e.NewValue;
			if(newValue && !CanBeSelected && TreeView != null) {
				TreeView.Focus();
				IsSelected = false;
				TreeView.Focus();
			}
			if(newValue)
				BringIntoView();
		}
		protected override void OnMouseDown(MouseButtonEventArgs e) {
			if(!CanBeSelected)
				e.Handled = true;
			base.OnMouseDown(e);
		}
		protected virtual void OnCanBeSelectedChanged(DependencyPropertyChangedEventArgs e) { }
		void UpdateExpanderVisibility() {
			if(PART_Expander == null) return;
			PART_Expander.Visibility = ShowExpander ? Visibility.Visible : Visibility.Collapsed;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PART_Expander = GetTemplateChild("Expander") as ToggleButton;
			PART_Header = GetTemplateChild("PART_Header") as FrameworkElement;
			UpdateExpanderVisibility();
		}
	}
	public class DesignTimeTreeView : TreeView {
		protected override DependencyObject GetContainerForItemOverride() {
			return new DesignTimeTreeViewItem();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is DesignTimeTreeViewItem;
		}
	}
}
