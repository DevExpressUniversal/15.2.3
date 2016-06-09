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
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Windows.Data;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Grid {
	public class GridExpandButtonBase : ContentControl {
		public bool IsChecked {
			get { return (bool)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public object CommandParameter {
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}
		public static readonly DependencyProperty IsCheckedProperty = DependencyPropertyManager.Register("IsChecked", typeof(bool), typeof(GridExpandButtonBase), new PropertyMetadata(false));
		public static readonly DependencyProperty CommandProperty = DependencyPropertyManager.Register("Command", typeof(ICommand), typeof(GridExpandButtonBase), new PropertyMetadata(null));
		public static readonly DependencyProperty CommandParameterProperty = DependencyPropertyManager.Register("CommandParameter", typeof(object), typeof(GridExpandButtonBase), new PropertyMetadata(null));
	}
	public class GridGroupExpandButton : GridExpandButtonBase {
		public GridGroupExpandButton() {
			this.SetDefaultStyleKey(typeof(GridGroupExpandButton));
		}
	}
	public class GridDetailExpandButton : GridExpandButtonBase {
		public GridDetailExpandButton() {
			this.SetDefaultStyleKey(typeof(GridDetailExpandButton));
		}
	}
	public class GridDetailExpandButtonContainer : ContentControl, INotifyVisibilityChanged {
		public static readonly DependencyProperty IsDetailButtonVisibleProperty =
			DependencyPropertyManager.RegisterAttached("IsDetailButtonVisible", typeof(bool), typeof(GridDetailExpandButtonContainer), new FrameworkPropertyMetadata(true, OnIsDetailButtonVisibleChanged));
		public static readonly DependencyProperty IsDetailButtonVisibleBindingContainerProperty =
			DependencyPropertyManager.Register("IsDetailButtonVisibleBindingContainer", typeof(BindingContainer), typeof(GridDetailExpandButtonContainer), new PropertyMetadata(null, (d, e) => ((GridDetailExpandButtonContainer)d).UpdateExpandButtonVisibilityBinding()));
		public static readonly DependencyProperty IsMasterRowExpandedProperty =
			DependencyPropertyManager.Register("IsMasterRowExpanded", typeof(bool), typeof(GridDetailExpandButtonContainer), new PropertyMetadata(false, (d, e) => ((GridDetailExpandButtonContainer)d).UpdateMargin()));
		public static readonly DependencyProperty RowPositionProperty =
			DependencyPropertyManager.Register("RowPosition", typeof(RowPosition), typeof(GridDetailExpandButtonContainer), new PropertyMetadata(RowPosition.Single, (d, e) => ((GridDetailExpandButtonContainer)d).UpdateMargin()));
		public static readonly DependencyProperty ShowHorizontalLinesProperty = DependencyPropertyManager.Register("ShowHorizontalLines", typeof(bool), typeof(GridDetailExpandButtonContainer), new PropertyMetadata(false, (d, e) => ((GridDetailExpandButtonContainer)d).UpdateMargin()));
		public static readonly DependencyProperty RowHandleProperty = DependencyPropertyManager.Register("RowHandle", typeof(int), typeof(GridDetailExpandButtonContainer), new PropertyMetadata(DataControlBase.InvalidRowHandle, (d, e) => ((GridDetailExpandButtonContainer)d).OnRowHandleChanged()));
		public GridDetailExpandButtonContainer() {
			this.SetDefaultStyleKey(typeof(GridDetailExpandButtonContainer));
		}
		static void OnIsDetailButtonVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			GridDetailExpandButtonContainer owner = d as GridDetailExpandButtonContainer;
			if(owner == null) return;
			owner.OnIsDetailButtonVisibleChangedCore();
		}
		public BindingContainer IsDetailButtonVisibleBindingContainer {
			get { return (BindingContainer)GetValue(IsDetailButtonVisibleBindingContainerProperty); }
			set { this.SetValue(IsDetailButtonVisibleBindingContainerProperty, value); }
		}
		public bool IsMasterRowExpanded {
			get { return (bool)GetValue(IsMasterRowExpandedProperty); }
			set { this.SetValue(IsMasterRowExpandedProperty, value); }
		}
		public RowPosition RowPosition {
			get { return (RowPosition)GetValue(RowPositionProperty); }
			set { this.SetValue(RowPositionProperty, value); }
		}
		public bool ShowHorizontalLines {
			get { return (bool)GetValue(ShowHorizontalLinesProperty); }
			set { SetValue(ShowHorizontalLinesProperty, value); }
		}
		public int RowHandle {
			get { return (int)GetValue(RowHandleProperty); }
			set { SetValue(RowHandleProperty, value); }
		}
		GridDetailExpandButton button;
		public static bool GetIsDetailButtonVisible(DependencyObject d) {
			if(d == null) return true;
			return (bool)d.GetValue(IsDetailButtonVisibleProperty);
		}
		public static void SetIsDetailButtonVisible(DependencyObject d, bool value) {
			if(d == null) return;
			d.SetValue(IsDetailButtonVisibleProperty, value);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			button = GetTemplateChild("PART_ExpandButton") as GridDetailExpandButton;
			UpdateBindings();
			UpdateExpandButtonVisibilityBinding();
			UpdateMargin();
		}
		void OnIsDetailButtonVisibleChangedCore() {
			if(button == null) return;
			button.IsHitTestVisible = GetIsDetailButtonVisible(this);
		}
		void UpdateExpandButtonVisibilityBinding() {
			if(RowHandle != DataControlBase.NewItemRowHandle) {
				if(IsDetailButtonVisibleBindingContainer != null) {
					BindingOperations.SetBinding(this, IsDetailButtonVisibleProperty, IsDetailButtonVisibleBindingContainer.Binding);
				}
				else {
					ClearValue(IsDetailButtonVisibleProperty);
				}
			}
			else {
				SetIsDetailButtonVisible(this, false);
			}
		}
		void UpdateBindings() {
			if(Visibility == Visibility.Visible) {
				BindingOperations.SetBinding(this, IsDetailButtonVisibleBindingContainerProperty, new Binding("View." + TableView.IsDetailButtonVisibleBindingContainerProperty.GetName()));
				BindingOperations.SetBinding(this, WidthProperty, new Binding("View." + TableView.ActualExpandDetailButtonWidthProperty.GetName()));
				BindingOperations.SetBinding(this, ShowHorizontalLinesProperty, new Binding("View." + TableView.ShowHorizontalLinesProperty.GetName()));
				BindingOperations.SetBinding(this, RowHandleProperty, new Binding("RowHandle.Value"));
			} else {
				this.ClearValue(IsDetailButtonVisibleBindingContainerProperty);
				this.ClearValue(WidthProperty);
				this.ClearValue(ShowHorizontalLinesProperty);
				this.ClearValue(RowHandleProperty);
			}
		}
		void INotifyVisibilityChanged.OnVisibilityChanged() {
			UpdateBindings();
		}
		void UpdateMargin() {
			if(ShowHorizontalLines && ((RowPosition == RowPosition.Bottom) || (RowPosition == RowPosition.Single)) && !IsMasterRowExpanded) {
				Margin = new Thickness(0, 0, 0, 1);
			} else {
				Margin = new Thickness(0);
			}
		}
		void OnRowHandleChanged() {
			UpdateExpandButtonVisibilityBinding();
		}
	}
	public class GridToggleStateButton : ToggleStateButton, IDataObjectReset {
		public Storyboard Expand { get; set; }
		public Storyboard Collapse { get; set; }
		bool isReady;
		public GridToggleStateButton() {
			this.SetDefaultStyleKey(typeof(GridToggleStateButton));
			Checked += new RoutedEventHandler(GridToggleStateButton_Checked);
			Unchecked += new RoutedEventHandler(GridToggleStateButton_Unchecked);
		}
#if !SL
		[System.ComponentModel.Browsable(false)]
		public bool ShouldSerializeIsChecked(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
#endif
		void GridToggleStateButton_Unchecked(object sender, RoutedEventArgs e) {
			if(Collapse != null) {
				isReady = true;
				Collapse.Begin();
			}
		}
		void GridToggleStateButton_Checked(object sender, RoutedEventArgs e) {
			if(Expand != null) {
				isReady = true;
				Expand.Begin();
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			FrameworkElement root = GetTemplateChild("root") as FrameworkElement;
			if(root == null)
				return;
			Expand = root.Resources["expand"] as Storyboard;
			Collapse = root.Resources["collapse"] as Storyboard;
			InitializeAnimation(IsChecked.GetValueOrDefault(false) ? Expand : Collapse);
			isReady = false;
		}
		void InitializeAnimation(Storyboard storyboard) {
			if(storyboard == null)
				return;
			storyboard.Begin();
			storyboard.SkipToFill();
		}
		void IDataObjectReset.Reset() {
			if(!isReady) return;
			if(Expand != null)
				Expand.SkipToFill();
			if(Collapse != null)
				Collapse.SkipToFill();
		}
	}
}
