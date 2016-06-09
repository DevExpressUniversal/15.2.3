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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Xpf.PivotGrid.Internal;
using CoreXtraPivotGrid = DevExpress.XtraPivotGrid;
using DevExpress.Xpf.Bars;
using System.ComponentModel;
#if SL
using SelectionChangedEventHandler = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventHandler;
using SelectionChangedEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventArgs;
#endif
namespace DevExpress.Xpf.PivotGrid {
	public abstract class FilterInfoBase {
		PivotGridField field;
		PivotFilterItemsBase filterItems;
		protected FilterInfoBase(PivotGridField field) {
			this.field = field;
		}
		protected PivotGridField Field { get { return field; } }
		protected internal PivotFilterItemsBase FilterItems { get { return filterItems; } }  
		public void OnPopupOpening(PopupBaseEdit popup, bool DeferUpdates) {
			Field.PivotGrid.UserAction = UserAction.FieldFilter;
			EnsurePopupTemplate(popup);
			EnsurePopupStyle(popup);
			this.filterItems = Field.CreateFilterItemsCore(!IsDeferredFilling, DeferUpdates);
			if(!IsDeferredFilling) {
				if(IsDeferredFilling)
					OnStartLoading(popup, IsDeferredFilling);
				OnEndLoading(popup, IsDeferredFilling);
				UpdatePopup(popup);
			} else {
				OnStartLoading(popup, IsDeferredFilling);
				if(this.filterItems.Count == 0)
					this.filterItems.CreateItemsAsync(delegate(CoreXtraPivotGrid.AsyncOperationResult result) {
						OnEndLoading(popup, IsDeferredFilling);
						UpdatePopup(popup);
					});
				else
					this.filterItems.EnsureAvailableItemsAsync(delegate(CoreXtraPivotGrid.AsyncOperationResult result) {
						OnEndLoading(popup, IsDeferredFilling);
						UpdatePopup(popup);
					});
			}
		}
		protected virtual void OnEndLoading(PopupBaseEdit popup, bool isDeferredFilling) { }
		protected virtual void OnStartLoading(PopupBaseEdit popup, bool isDeferredFilling) { }		  
		public void OnPopupClosed(PopupBaseEdit popup, bool applyFilter, bool deferUpdates) {
			if(applyFilter) 
				ApplyFilter(deferUpdates);
			ClearPopupData(popup);
			Field.PivotGrid.UserAction = UserAction.None;
		}
		void UpdatePopup(PopupBaseEdit popup) {
			UpdatePopupData(popup);
			UpdatePopupButtonsVisibility(popup);
			RaiseFilterPopupEvent(popup);
		}
		protected virtual void UpdatePopupData(PopupBaseEdit popup) {
		}
		protected bool IsDeferredFilling { 
			get {
				return field.PivotGrid.UseAsyncMode;
			}
		}
		protected virtual void ClearPopupData(PopupBaseEdit popup) {
			Field.DropDownFilterListSize = new Size(popup.PopupWidth, popup.PopupHeight);
			this.filterItems = null;
		}
		protected virtual void ApplyFilter(bool deferUpdates) {
			FilterItems.ApplyFilter(true, deferUpdates, false);
		}
		protected virtual void UpdatePopupButtonsVisibility(PopupBaseEdit popup) {
			popup.PopupFooterButtons = PopupFooterButtons.OkCancel;
			popup.ShowSizeGrip = true;
		}
		protected virtual void RaiseFilterPopupEvent(PopupBaseEdit popup) {
		}
		protected virtual void EnsurePopupStyle(PopupBaseEdit popup) {
			popup.PopupMinHeight = 100;
			popup.PopupMinWidth = 100;
			popup.PopupMaxHeight = Double.PositiveInfinity;
			popup.PopupHeight = Field.ActualDropDownFilterListSize.Height;
			popup.PopupWidth = Field.ActualDropDownFilterListSize.Width;
		}
		protected virtual void EnsurePopupTemplate(PopupBaseEdit popup) {
		}		
	}
	public class CheckedListFilterInfo : FilterInfoBase {
		public CheckedListFilterInfo(PivotGridField field) 
			: base(field) {			
		}		
		bool IsSimpleFilterPopup() {
			return IsSimpleFilterPopup(Field);
		}
		public static bool IsSimpleFilterPopup(PivotGridField field) {
			return field.Group == null || field.PivotGrid.GroupFilterMode == GroupFilterMode.List || field.Group.IndexOf(field) != 0;
		}
		protected override void EnsurePopupTemplate(PopupBaseEdit popup) {
			((FilterPopupEdit)popup).SetContentContainerTemplate(IsSimpleFilterPopup());
		}
		protected override void OnEndLoading(PopupBaseEdit popup, bool isDeferredFilling) {
			if(IsSimpleFilterPopup() || !isDeferredFilling)
				((FilterPopupEdit)popup).StyleSettings = new PivotComboBoxStyleSettings();
		}
		protected override void OnStartLoading(PopupBaseEdit popup, bool isDeferredFilling) {
			ComboBoxEdit comboBox = (ComboBoxEdit)popup;
			comboBox.ItemsSource = new List<string>();
			if(!IsSimpleFilterPopup())
				comboBox.StyleSettings = new PivotComboBoxStyleSettings();
			else
				comboBox.StyleSettings = new PivotLoadingComboBoxStyleSettings();
		}	  
		protected override void UpdatePopupData(PopupBaseEdit popup) {
			base.UpdatePopupData(popup);
			if(FilterItems == null) return;
			((FilterPopupEdit)popup).FilterItems = FilterItems;
			ComboBoxEdit comboBox = (ComboBoxEdit)popup;
#if !SL
			comboBox.ItemsPanel = GetItemsPanel(FilterItems.VisibleCount);
#endif
			comboBox.ItemsSource = FilterItems.VisibleItems;
			comboBox.BeginInit();
			comboBox.SelectedItems.Clear();
			foreach(PivotGridFilterItem item in FilterItems.VisibleItems) {
				if(item.IsChecked == true)
					comboBox.SelectedItems.Add(item);
			}
			comboBox.EndInit();
			comboBox.PopupContentSelectionChanged += PopupListBoxSelectionChanged;
		}
		protected override void ClearPopupData(PopupBaseEdit popup) {		 
			base.ClearPopupData(popup);
			ComboBoxEdit comboBox = (ComboBoxEdit)popup;
			comboBox.PopupContentSelectionChanged -= PopupListBoxSelectionChanged;
			comboBox.ItemsSource = null;
			FilterCheckedTreeView tv = ((FilterPopupEdit)comboBox).PopupTreeView;
			if(tv != null)
				tv.ResetAsyncLoad();
		}
		protected ItemsPanelTemplate GetItemsPanel(int count) {
			return FilterPopupVirtualizingStackPanel.GetItemsPanelTemplate(count);
		}
#if DEBUGTEST && SL
		internal
#endif
		void PopupListBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
			foreach(PivotGridFilterItem item in e.AddedItems)
				item.IsChecked = true;
			foreach(PivotGridFilterItem item in e.RemovedItems)
				item.IsChecked = false;
			PopupBaseEditHelper.GetOkButton((PopupBaseEdit)sender).IsEnabled = FilterItems.CanAccept;
		}
	}
	[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
	public class PivotComboBoxStyleSettings : CheckedComboBoxStyleSettings {
		protected override IEnumerable<CustomItem> GetCustomItems(LookUpEditBase editor) {
			DevExpress.Xpf.Editors.Native.VisualClientOwner visualClient = DevExpress.Xpf.Editors.Native.LookUpEditHelper.GetVisualClient(editor);
			ListBox lb = visualClient.InnerEditor as ListBox;
			if(lb != null) {
				lb.SetValue(PivotGridPopupMenu.GridMenuTypeProperty, PivotGridMenuType.FilterPopup);
				lb.SetValue(BarManager.DXContextMenuProperty, ((FilterPopupEdit)editor).PivotGrid.GridMenu);
			}
			CustomItem item = new SelectAllItem();
			item.DisplayText = PivotGridLocalizer.GetString(PivotGridStringId.FilterShowAll);
			return new List<CustomItem>{ item };
		} 
	}
	[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
	public class PivotLoadingComboBoxStyleSettings : CheckedComboBoxStyleSettings {
		protected override IEnumerable<CustomItem> GetCustomItems(LookUpEditBase editor) {
			CustomItem item = new SelectAllItem();
			Style style = new Style(typeof (ComboBoxEditItem));
			style.Setters.Add(new Setter(Control.TemplateProperty, ((FilterPopupEdit)editor).GetWaitIndicatorTemplate()));
			item.ItemContainerStyle = style;
			return new List<CustomItem>{ item };
		}
	}
}
