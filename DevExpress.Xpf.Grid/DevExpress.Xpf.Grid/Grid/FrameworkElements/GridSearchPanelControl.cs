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
using DevExpress.Xpf.Grid.HitTest;
using System.Windows.Input;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Xpf.Utils;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Xpf.Editors.Helpers;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Core.Native;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Xpf.Core.WPFCompatibility;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.Grid {
	[DXToolboxBrowsable(false)]
	public class GridSearchControl : GridSearchControlBase {
		public static readonly DependencyProperty GroupPanelShownProperty;
		public static readonly DependencyProperty ActualShowButtonCloseProperty;
		public static readonly DependencyProperty ShowSearchPanelModeProperty;
		public static readonly DependencyProperty ActualVisibilityProperty;
		static GridSearchControl() {
			Type ownerType = typeof(GridSearchControl);
			GroupPanelShownProperty = DependencyPropertyManager.Register("GroupPanelShown", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			ActualShowButtonCloseProperty = DependencyPropertyManager.Register("ActualShowButtonClose", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((GridSearchControl)d).UpdateCloseButtonVisibility()));
			ShowSearchPanelModeProperty = DependencyPropertyManager.Register("ShowSearchPanelMode", typeof(ShowSearchPanelMode), ownerType, new FrameworkPropertyMetadata(ShowSearchPanelMode.Default, (d, e) => ((GridSearchControl)d).UpdateCloseButtonVisibility()));
			ActualVisibilityProperty = DependencyPropertyManager.Register("ActualVisibility", typeof(Visibility), ownerType, new FrameworkPropertyMetadata(Visibility.Collapsed, (d, e) => ((GridSearchControl)d).OnVisibilityChanged((Visibility)e.OldValue, (Visibility)e.NewValue)));
		}
		internal override bool IsLogicControl { get { return false; } }
		public GridSearchControl() {
			GotFocus += GridSearchControl_GotFocus;
			Loaded += GridSearchControl_Loaded;
		}
		public bool GroupPanelShown {
			get { return (bool)GetValue(GroupPanelShownProperty); }
			set { SetValue(GroupPanelShownProperty, value); }
		}
		public bool ActualShowButtonClose {
			get { return (bool)GetValue(ActualShowButtonCloseProperty); }
			set { SetValue(ActualShowButtonCloseProperty, value); }
		}
		public ShowSearchPanelMode ShowSearchPanelMode {
			get { return (ShowSearchPanelMode)GetValue(ShowSearchPanelModeProperty); }
			set { SetValue(ShowSearchPanelModeProperty, value); }
		}
		public Visibility ActualVisibility {
			get { return (Visibility)GetValue(ActualVisibilityProperty); }
			set { SetValue(ActualVisibilityProperty, value); }
		}
		void GridSearchControl_GotFocus(object sender, RoutedEventArgs e) {
			if(View == null)
				return;
			View.CommitEditing();
			View.SetSearchPanelFocus(true);
		}
		void UpdateCloseButtonVisibility() {
			switch(ShowSearchPanelMode) {
				case ShowSearchPanelMode.Never:
				case Grid.ShowSearchPanelMode.Always:
					ShowCloseButton = false;
					break;
				case Grid.ShowSearchPanelMode.Default:
					ShowCloseButton = ActualShowButtonClose;
					break;
			}
		}
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsKeyboardFocusWithinChanged(e);
			if((bool)e.NewValue || View == null)
				return;
			View.SetSearchPanelFocus(false);
			if(View.GetIsKeyboardFocusWithin())
				View.Focus();
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(View == null)
				return;
			if(e.Key != Key.Tab)
				return;
			if(ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e))) {
				SetFocusOnCellEditor();
				ViewInplaceEditorMoveFocus(e);
				e.Handled = true;
				return;
			}
			SetFocusOnCellEditor();
			e.Handled = true;
			return;
		}
		protected override void OnPreviewKeyUp(KeyEventArgs e) {
			base.OnPreviewKeyUp(e);
			if(!ModifierKeysHelper.NoModifiers(ModifierKeysHelper.GetKeyboardModifiers(e)))
				return;
			switch(e.Key) {
				case Key.Escape:
					if(!e.Handled && View != null)
						View.HideSearchPanel();
					break;
				case Key.Down:
					PopupBaseEdit popupBaseEdit = GetEditorControl() as PopupBaseEdit;
					if(popupBaseEdit == null || popupBaseEdit.IsPopupOpen)
						break;
					e.Handled = true;
					break;
			}
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			base.OnPreviewKeyDown(e);
			if(!ModifierKeysHelper.NoModifiers(ModifierKeysHelper.GetKeyboardModifiers(e)))
				return;
			switch(e.Key) {
				case Key.Down:
					PopupBaseEdit popupBaseEdit = GetEditorControl() as PopupBaseEdit;
					if(popupBaseEdit == null || !popupBaseEdit.IsPopupOpen) {
						SetFocusOnCellEditor();
						e.Handled = true;
					}
					break;
			}
		}
#if DEBUGTEST
		public void ProcessKeyDown(KeyEventArgs e) {
			OnPreviewKeyDown(e);
			if(e.Handled)
				return;
			OnPreviewKeyUp(e);
			if(e.Handled)
				return;
			OnKeyDown(e);
			if(!e.Handled)
				OnPreviewKeyUp(e);
		}
#endif
		void SetFocusOnCellEditor() {
			if(View == null)
				return;
			View.SetSearchPanelFocus(false);
			View.Focus();
		}
		void ViewInplaceEditorMoveFocus(KeyEventArgs e) {
#if !SL
			View.InplaceEditorOwner.MoveFocus(e);
#endif
		}
		void GridSearchControl_Loaded(object sender, RoutedEventArgs e) {
			if(Visibility != Visibility.Collapsed && View != null && View.PostponedSearchControlFocus) {
				Focus();
				View.PostponedSearchControlFocus = false;
			}
		}
		void OnVisibilityChanged(Visibility oldValue, Visibility newValue) {
			if(oldValue == newValue)
				return;
			if(View == null || Visibility != Visibility.Collapsed) {
				return;
			}
			SetFocusOnCellEditor();
			if(View.SearchPanelClearOnClose && !View.DataControl.IsDeserializing) {
#if SL
			ButtonEdit editor = GetEditorControl();
			if(editor != null)
				editor.EditValueChanged += GridSearchControl_EditValueChanged;
#endif
				SearchText = null;
				OnFindCommandExecuted();
			}
		}
#if SL
		void GridSearchControl_EditValueChanged(object sender, EditValueChangedEventArgs e) {
			(sender as ButtonEdit).EditValueChanged -= GridSearchControl_EditValueChanged;
			GetEditorControl().EditValue = String.Empty;
		}
#endif
		void BindShowGroupPanel(DataViewBase view) {
			GridViewBase viewBase = view as GridViewBase;
			if(viewBase == null)
				return;
			Binding bindingShowGroupPanelProperty = new Binding() { Source = view, Path = new PropertyPath(GridViewBase.ShowGroupPanelProperty.GetName()) };
			SetBinding(GridSearchControl.GroupPanelShownProperty, bindingShowGroupPanelProperty);
		}
		protected override void BindSearchPanel(DataViewBase view) {
			base.BindSearchPanel(view);
			Binding bindingShowSearchPanelMRUButtonProperty = new Binding() { Source = view, Path = new PropertyPath(DataViewBase.ShowSearchPanelMRUButtonProperty.GetName()) };
			SetBinding(SearchControl.ShowMRUButtonProperty, bindingShowSearchPanelMRUButtonProperty);
			Binding bindingShowSearchPanelFindButtonProperty = new Binding() { Source = view, Path = new PropertyPath(DataViewBase.ShowSearchPanelFindButtonProperty.GetName()) };
			SetBinding(SearchControl.ShowFindButtonProperty, bindingShowSearchPanelFindButtonProperty);
			Binding bindingCloseCommandProperty = new Binding() { Source = view.Commands, Path = new PropertyPath("HideSearchPanel") };
			SetBinding(SearchControl.CloseCommandProperty, bindingCloseCommandProperty);
			Binding bindingActualShowButtonCloseProperty = new Binding() { Source = view, Path = new PropertyPath(DataViewBase.ShowSearchPanelCloseButtonProperty.GetName()) };
			SetBinding(GridSearchControl.ActualShowButtonCloseProperty, bindingActualShowButtonCloseProperty);
			ShowCloseButton = ActualShowButtonClose;
			Binding bindingShowSearchPanelModeProperty = new Binding() { Source = view, Path = new PropertyPath(DataViewBase.ShowSearchPanelModeProperty.GetName()) };
			SetBinding(GridSearchControl.ShowSearchPanelModeProperty, bindingShowSearchPanelModeProperty);
			Binding bindingActualVisibilityProperty = new Binding() { Source = this, Path = new PropertyPath(GridSearchControl.VisibilityProperty.GetName()) };
			SetBinding(GridSearchControl.ActualVisibilityProperty, bindingActualVisibilityProperty);
			Binding bindingSearchPanelImmediateMRUPopup = new Binding() { Source = view, Path = new PropertyPath(DataViewBase.SearchPanelImmediateMRUPopupProperty.GetName()) };
			SetBinding(GridSearchControl.ImmediateMRUPopupProperty, bindingSearchPanelImmediateMRUPopup);
			BindShowGroupPanel(view);
		}
#if !SL
		[Browsable(false)]
		public bool ShouldSerializeMRU(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		[Browsable(false)]
		public bool ShouldSerializeImmediateMRUPopup(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
#endif
	}
	[DXToolboxBrowsable(false)]
	public class ChildMinWidthPanel : Decorator {
		public static readonly DependencyProperty ChildMinWidthProperty = DependencyProperty.Register("ChildMinWidth", typeof(double), typeof(ChildMinWidthPanel), new PropertyMetadata(0.0, (d, e) => ((ChildMinWidthPanel)d).OnChildMinWidthChanged()));
		void OnChildMinWidthChanged() {
			InvalidateMeasure();
		}
		public double ChildMinWidth {
			get { return (double)GetValue(ChildMinWidthProperty); }
			set { SetValue(ChildMinWidthProperty, value); }
		}
		public ChildMinWidthPanel()
			: base() {
		}
		protected override Size MeasureOverride(Size availableSize) {
			Child.Measure(availableSize);
			Size childDesiredSize = Child.DesiredSize;
			Size finalSize = new Size(Math.Min(Math.Max(ChildMinWidth, childDesiredSize.Width), availableSize.Width), childDesiredSize.Height);
			return finalSize;
		}
	}
}
