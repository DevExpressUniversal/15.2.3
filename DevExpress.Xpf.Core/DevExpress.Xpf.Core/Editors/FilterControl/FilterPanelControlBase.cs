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
using System.Windows.Input;
using DevExpress.Xpf.Utils;
using System.Collections.ObjectModel;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Core;
using System.Collections.Specialized;
#if SL
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using DevExpress.Utils;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Editors.Filtering {
	public abstract class FilterPanelControlBase : Control, IWeakEventListener {
		public static readonly DependencyProperty ClearFilterCommandProperty;
		public static readonly DependencyProperty ShowFilterEditorCommandProperty;
		public static readonly DependencyProperty IsFilterEnabledProperty;
		public static readonly DependencyProperty IsCanEnableFilterProperty;
		public static readonly DependencyProperty AllowFilterEditorProperty;
		public static readonly DependencyProperty MRUFiltersProperty;
		public static readonly DependencyProperty ActiveFilterInfoProperty;
		public static readonly DependencyProperty AllowMRUFilterListProperty;
		public static readonly DependencyProperty FilterPanelContentProperty;
		static FilterPanelControlBase() {
			Type ownerType = typeof(FilterPanelControlBase);
			ClearFilterCommandProperty = DependencyPropertyManager.Register("ClearFilterCommand", typeof(ICommand), ownerType, new PropertyMetadata(null));
			ShowFilterEditorCommandProperty = DependencyPropertyManager.Register("ShowFilterEditorCommand", typeof(ICommand), ownerType, new PropertyMetadata(null));
			IsFilterEnabledProperty = DependencyPropertyManager.Register("IsFilterEnabled", typeof(bool), ownerType, new PropertyMetadata(false, OnIsFilterEnabledChanged));
			IsCanEnableFilterProperty = DependencyPropertyManager.Register("IsCanEnableFilter", typeof(bool), ownerType, new PropertyMetadata(true));
			AllowFilterEditorProperty = DependencyPropertyManager.Register("AllowFilterEditor", typeof(bool), ownerType, new PropertyMetadata(true));
			MRUFiltersProperty = DependencyPropertyManager.Register("MRUFilters", typeof(ReadOnlyObservableCollection<CriteriaOperatorInfo>),
				ownerType, new PropertyMetadata(null, OnMRUFiltersChanged));
			ActiveFilterInfoProperty = DependencyPropertyManager.Register("ActiveFilterInfo", typeof(DevExpress.Xpf.Core.CriteriaOperatorInfo),
				ownerType, new PropertyMetadata(null,new PropertyChangedCallback(OnActiveFilterInfoChanged)));
			AllowMRUFilterListProperty = DependencyPropertyManager.Register("AllowMRUFilterList", typeof(bool),
				ownerType, new PropertyMetadata(true, OAllowMRUFilterListChanged));
			FilterPanelContentProperty = DependencyPropertyManager.Register("FilterPanelContent", typeof(object), ownerType);
		}
		static void OAllowMRUFilterListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((FilterPanelControlBase)d).UpdateMRUComboBoxEditorButtonsVisibility();
		}
		static void OnIsFilterEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((FilterPanelControlBase)d).UpdateIsFilterEnabledButton();
		}
		static void OnActiveFilterInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DevExpress.Xpf.Core.CriteriaOperatorInfo selectedFilter = e.NewValue as DevExpress.Xpf.Core.CriteriaOperatorInfo;
			FilterPanelControlBase panel = (FilterPanelControlBase)d;
			panel.UpdateMRUComboEditValue(selectedFilter);
			panel.UpdateMRUComboBoxEditorButtonsVisibility();
		}
		static void OnMRUFiltersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FilterPanelControlBase panel = (FilterPanelControlBase)d;
			panel.OnMRUFiltersChanged((ReadOnlyObservableCollection<CriteriaOperatorInfo>)e.OldValue);
		}
		public FilterPanelControlBase() {
			this.SetDefaultStyleKey(typeof(FilterPanelControlBase));
		}
		void UpdateMRUComboEditValue(DevExpress.Xpf.Core.CriteriaOperatorInfo selectedFilter) {
			ApplyTemplate();
			ComboBoxEdit mruListCombo = GetMRUListCombo();
			if(mruListCombo != null) {
				mruListCombo.EditValue = selectedFilter;
			}
		}
		void UpdateMRUComboBoxEditorButtonsVisibility() {
			ComboBoxEdit mruListCombo = GetMRUListCombo();
			if(mruListCombo != null) {
				mruListCombo.IsHitTestVisible = AllowMRUFilterList;
				UpdateMRUComboBoxEditorButtonsVisibilityCore();
			}
		}
		ComboBoxEdit GetMRUListCombo() {
			return GetTemplateChild("PART_FilterPanelMRUComboBox") as ComboBoxEdit;
		}
		void mruListCombo_MouseLeave(object sender, MouseEventArgs e) {
			ComboBoxEdit mruListCombo = sender as ComboBoxEdit;
			if(mruListCombo != null) {
					mruListCombo.TextDecorations = null;
			}
		}
		void mruListCombo_MouseEnter(object sender, MouseEventArgs e) {
			ComboBoxEdit mruListCombo = sender as ComboBoxEdit;
			if(mruListCombo != null && IsMRUComboPopupActive) {
				mruListCombo.TextDecorations = TextDecorations.Underline;   
			}
		}
		bool IsMRUComboPopupActive {
			get {return MRUFilters != null && MRUFilters.Count > 0 && AllowMRUFilterList;}
		}
		public ICommand ClearFilterCommand {
			get { return (ICommand)GetValue(ClearFilterCommandProperty); }
			set { SetValue(ClearFilterCommandProperty, value); }
		}
		public ICommand ShowFilterEditorCommand {
			get { return (ICommand)GetValue(ShowFilterEditorCommandProperty); }
			set { SetValue(ShowFilterEditorCommandProperty, value); }
		}
		public bool IsFilterEnabled {
			get { return (bool)GetValue(IsFilterEnabledProperty); }
			set { SetValue(IsFilterEnabledProperty, value); }
		}
		public bool IsCanEnableFilter {
			get { return (bool)GetValue(IsCanEnableFilterProperty); }
			set { SetValue(IsCanEnableFilterProperty, value); }
		}
		public bool AllowFilterEditor {
			get { return (bool)GetValue(AllowFilterEditorProperty); }
			set { SetValue(AllowFilterEditorProperty, value); }
		}
		public object FilterPanelContent {
			get { return (object)GetValue(FilterPanelContentProperty); }
			set { SetValue(FilterPanelContentProperty, value); }
		}
		public bool AllowMRUFilterList {
			get { return (bool)GetValue(AllowMRUFilterListProperty); }
			set { SetValue(AllowMRUFilterListProperty, value); }
		}
		public ReadOnlyObservableCollection<DevExpress.Xpf.Core.CriteriaOperatorInfo> MRUFilters {
			get { return (ReadOnlyObservableCollection<DevExpress.Xpf.Core.CriteriaOperatorInfo>)GetValue(MRUFiltersProperty); }
			set { SetValue(MRUFiltersProperty, value); }
		}
		public DevExpress.Xpf.Core.CriteriaOperatorInfo ActiveFilterInfo {
			get { return (DevExpress.Xpf.Core.CriteriaOperatorInfo)GetValue(ActiveFilterInfoProperty); }
			set { SetValue(ActiveFilterInfoProperty, value); }
		}
		DependencyObject IsFilterEnabledButton { get; set; }
		protected ComboBoxEdit mruListCombo;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(mruListCombo != null) {
				mruListCombo.SelectedIndexChanged -= FilterPanelMRUComboBoxSelectedIndexChanged;
				mruListCombo.MouseEnter -= new MouseEventHandler(mruListCombo_MouseEnter);
				mruListCombo.MouseLeave -= new MouseEventHandler(mruListCombo_MouseLeave);
			}
			mruListCombo = GetMRUListCombo();
			if(mruListCombo != null) {
				mruListCombo.SelectedIndexChanged += FilterPanelMRUComboBoxSelectedIndexChanged;
				mruListCombo.MouseEnter += new MouseEventHandler(mruListCombo_MouseEnter);
				mruListCombo.MouseLeave += new MouseEventHandler(mruListCombo_MouseLeave);
				UpdateMRUComboBoxEditorButtonsVisibilityCore();
				mruListCombo.EditValue = ActiveFilterInfo;
			}
			IsFilterEnabledButton = GetTemplateChild("PART_FilterPanelIsActiveButton");
			UpdateIsFilterEnabledButton();
		}
		void UpdateMRUComboBoxEditorButtonsVisibilityCore() {
			if(MRUFilters != null) {
				mruListCombo.ShowEditorButtons = (MRUFilters.Count > 0) && AllowMRUFilterList;
			}
		}
		void UpdateIsFilterEnabledButton() {
			if(IsFilterEnabledButton == null)
				return;
			ToolTipService.SetToolTip(IsFilterEnabledButton, EditorLocalizer.GetString(IsFilterEnabled ? EditorStringId.FilterPanelDisableFilter : EditorStringId.FilterPanelEnableFilter));
		}
		protected CriteriaOperatorInfo GetSelectedFilter() { 
			return (mruListCombo == null) ? null : (mruListCombo.SelectedItem as CriteriaOperatorInfo);
		} 
		protected virtual void FilterPanelMRUComboBoxSelectedIndexChanged(object sender, System.Windows.RoutedEventArgs args) {
		}
		void OnMRUFiltersChanged(ReadOnlyObservableCollection<CriteriaOperatorInfo> oldValue) {
			if(oldValue != null)
				CollectionChangedEventManager.RemoveListener(oldValue, this);
			if(MRUFilters != null)
				CollectionChangedEventManager.AddListener(MRUFilters, this);
		}
		#region IWeakEventListener Members
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if(managerType == typeof(CollectionChangedEventManager)) {
				UpdateMRUComboBoxEditorButtonsVisibility();
				return true;
			}
			return false;
		}
		#endregion
	}
	public class FilterPanelCaptionControl : ContentControl {
		public FilterPanelCaptionControl() {
			this.SetDefaultStyleKey(typeof(FilterPanelCaptionControl));
		}
	}
}
