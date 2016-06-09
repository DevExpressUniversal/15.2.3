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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Native;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
namespace DevExpress.Xpf.Core {
	public enum HeaderLocation { None, Left, Top, Right, Bottom }
	[Flags]
	public enum HideButtonShowMode { NoWhere = 0x0, InAllTabs = 0x1, InActiveTab = 0x2, InHeaderArea = 0x4,
		InAllTabsAndHeaderArea = InAllTabs | InHeaderArea, InActiveTabAndHeaderArea = InActiveTab | InHeaderArea, }
	[Flags]
	public enum NewButtonShowMode { NoWhere = 0x0, InHeaderArea = 0x1, InTabPanel = 0x2,
		InHeaderAreaAndTabPanel = InHeaderArea | InTabPanel, }
	public enum SingleTabItemHideMode { Disable, Hide, HideAndShowNewItem }
	public abstract class TabControlViewBase : Freezable {
		#region Properties
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyPropertyKey OwnerPropertyKey = DependencyProperty.RegisterReadOnly("Owner", typeof(DXTabControl), typeof(TabControlViewBase), new PropertyMetadata(null, (d, e) => ((TabControlViewBase)d).OnOwnerChanged((DXTabControl)e.OldValue, (DXTabControl)e.NewValue)));
		public static readonly DependencyProperty OwnerProperty = OwnerPropertyKey.DependencyProperty;
		public static readonly DependencyProperty AllowKeyboardNavigationProperty = DependencyProperty.Register("AllowKeyboardNavigation", typeof(bool?), typeof(TabControlViewBase), new PropertyMetadata(null));
		public static readonly DependencyProperty TagProperty = FrameworkElement.TagProperty.AddOwner(typeof(TabControlViewBase));
		public static readonly DependencyProperty HeaderLocationProperty = DependencyProperty.Register("HeaderLocation", typeof(HeaderLocation), typeof(TabControlViewBase),
			new PropertyMetadata(HeaderLocation.Top, (d, e) => ((TabControlViewBase)d).UpdateViewProperties()));
		public static readonly DependencyProperty ShowHeaderMenuProperty = DependencyProperty.Register("ShowHeaderMenu", typeof(bool), typeof(TabControlViewBase), new PropertyMetadata(false));
		public static readonly DependencyProperty ShowVisibleTabItemsInHeaderMenuProperty = DependencyProperty.Register("ShowVisibleTabItemsInHeaderMenu", typeof(bool), typeof(TabControlViewBase), 
			new PropertyMetadata(true, (d,e) => ((TabControlViewBase)d).UpdateViewProperties()));
		public static readonly DependencyProperty ShowHiddenTabItemsInHeaderMenuProperty = DependencyProperty.Register("ShowHiddenTabItemsInHeaderMenu", typeof(bool), typeof(TabControlViewBase),
			new PropertyMetadata(false, (d, e) => ((TabControlViewBase)d).UpdateViewProperties()));
		public static readonly DependencyProperty ShowDisabledTabItemsInHeaderMenuProperty = DependencyProperty.Register("ShowDisabledTabItemsInHeaderMenu", typeof(bool), typeof(TabControlViewBase),
			new PropertyMetadata(false, (d, e) => ((TabControlViewBase)d).UpdateViewProperties()));
		public static readonly DependencyProperty CloseHeaderMenuOnItemSelectingProperty = DependencyProperty.Register("CloseHeaderMenuOnItemSelecting", typeof(bool), typeof(TabControlViewBase), new PropertyMetadata(false));
		public static readonly DependencyProperty NewTabCommandProperty = DependencyProperty.Register("NewTabCommand", typeof(ICommand), typeof(TabControlViewBase), new PropertyMetadata(null, (d, e) => ((TabControlViewBase)d).OnNewTabCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue)));
		public static readonly DependencyProperty NewTabCommandParameterProperty = DependencyProperty.Register("NewTabCommandParameter", typeof(object), typeof(TabControlViewBase), new PropertyMetadata(null));
		public static readonly DependencyProperty NewButtonShowModeProperty = DependencyProperty.Register("NewButtonShowMode", typeof(NewButtonShowMode), typeof(TabControlViewBase),
			new PropertyMetadata(NewButtonShowMode.NoWhere, (d, e) => ((TabControlViewBase)d).UpdateNewButtonsVisibility()));
		static readonly DependencyPropertyKey MainNewButtonVisibilityPropertyKey = DependencyProperty.RegisterReadOnly("MainNewButtonVisibility", typeof(Visibility), typeof(TabControlViewBase), null);
		static readonly DependencyPropertyKey PanelNewButtonVisibilityPropertyKey = DependencyProperty.RegisterReadOnly("PanelNewButtonVisibility", typeof(Visibility), typeof(TabControlViewBase), null);
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute, EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty MainNewButtonVisibilityProperty = MainNewButtonVisibilityPropertyKey.DependencyProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute, EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty PanelNewButtonVisibilityProperty = PanelNewButtonVisibilityPropertyKey.DependencyProperty;
		public static readonly DependencyProperty HideButtonShowModeProperty = DependencyProperty.Register("HideButtonShowMode", typeof(HideButtonShowMode), typeof(TabControlViewBase),
			new PropertyMetadata(HideButtonShowMode.NoWhere, (d, e) => ((TabControlViewBase)d).UpdateViewProperties()));
		public static readonly DependencyProperty SingleTabItemHideModeProperty = DependencyProperty.Register("SingleTabItemHideMode", typeof(SingleTabItemHideMode), typeof(TabControlViewBase), new PropertyMetadata(SingleTabItemHideMode.Hide));
		public static readonly DependencyProperty RemoveTabItemsOnHidingProperty = DependencyProperty.Register("RemoveTabItemsOnHiding", typeof(bool), typeof(TabControlViewBase), new PropertyMetadata(false));
		public bool? AllowKeyboardNavigation { get { return (bool?)GetValue(AllowKeyboardNavigationProperty); } set { SetValue(AllowKeyboardNavigationProperty, value); } }
		public object Tag { get { return (object)GetValue(TagProperty); } set { SetValue(TagProperty, value); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TabControlViewBaseHeaderLocation")]
#endif
		public HeaderLocation HeaderLocation { get { return (HeaderLocation)GetValue(HeaderLocationProperty); } set { SetValue(HeaderLocationProperty, value); } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ObservableCollection<IControllerAction> HeaderMenuCustomizations { get { return HeaderMenuCustomizationController.Actions; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TabControlViewBaseShowHeaderMenu")]
#endif
		public bool ShowHeaderMenu { get { return (bool)GetValue(ShowHeaderMenuProperty); } set { SetValue(ShowHeaderMenuProperty, value); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TabControlViewBaseShowVisibleTabItemsInHeaderMenu")]
#endif
		public bool ShowVisibleTabItemsInHeaderMenu { get { return (bool)GetValue(ShowVisibleTabItemsInHeaderMenuProperty); } set { SetValue(ShowVisibleTabItemsInHeaderMenuProperty, value); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TabControlViewBaseShowHiddenTabItemsInHeaderMenu")]
#endif
		public bool ShowHiddenTabItemsInHeaderMenu { get { return (bool)GetValue(ShowHiddenTabItemsInHeaderMenuProperty); } set { SetValue(ShowHiddenTabItemsInHeaderMenuProperty, value); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TabControlViewBaseShowDisabledTabItemsInHeaderMenu")]
#endif
		public bool ShowDisabledTabItemsInHeaderMenu { get { return (bool)GetValue(ShowDisabledTabItemsInHeaderMenuProperty); } set { SetValue(ShowDisabledTabItemsInHeaderMenuProperty, value); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TabControlViewBaseCloseHeaderMenuOnItemSelecting")]
#endif
		public bool CloseHeaderMenuOnItemSelecting { get { return (bool)GetValue(CloseHeaderMenuOnItemSelectingProperty); } set { SetValue(CloseHeaderMenuOnItemSelectingProperty, value); } }
		public ICommand NewTabCommand { get { return (ICommand)GetValue(NewTabCommandProperty); } set { SetValue(NewTabCommandProperty, value); } }
		public object NewTabCommandParameter { get { return (object)GetValue(NewTabCommandParameterProperty); } set { SetValue(NewTabCommandParameterProperty, value); } }
		public NewButtonShowMode NewButtonShowMode { get { return (NewButtonShowMode)GetValue(NewButtonShowModeProperty); } set { SetValue(NewButtonShowModeProperty, value); } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public DelegateCommand ActualNewTabCommand { get; private set; }
		public HideButtonShowMode HideButtonShowMode { get { return (HideButtonShowMode)GetValue(HideButtonShowModeProperty); } set { SetValue(HideButtonShowModeProperty, value); } }
		[Obsolete("Use the HideButtonShowMode property.")]
		public bool AllowHideTabItems {
			get { return HideButtonShowMode != Xpf.Core.HideButtonShowMode.NoWhere; }
			set { HideButtonShowMode = value ? HideButtonShowMode.InAllTabs : HideButtonShowMode.NoWhere; }
		}
		public SingleTabItemHideMode SingleTabItemHideMode { get { return (SingleTabItemHideMode)GetValue(SingleTabItemHideModeProperty); } set { SetValue(SingleTabItemHideModeProperty, value); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TabControlViewBaseRemoveTabItemsOnHiding")]
#endif
		public bool RemoveTabItemsOnHiding { get { return (bool)GetValue(RemoveTabItemsOnHidingProperty); } set { SetValue(RemoveTabItemsOnHidingProperty, value); } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public DelegateCommand CloseTabCommand { get; private set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public TabControlScrollView ScrollView { get { return this as TabControlScrollView; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public TabControlMultiLineView MultiLineView { get { return this as TabControlMultiLineView; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public TabControlStretchView StretchView { get { return this as TabControlStretchView; } }
		public DXTabControl Owner { get { return (DXTabControl)GetValue(OwnerProperty); } private set { SetValue(OwnerPropertyKey, value); } }
		ControllerBehavior headerMenuCustomizationController;
		ControllerBehavior CreateHeaderMenuCustomizationController() {
			ControllerBehavior result = new ControllerBehavior();
			result.Actions.CollectionChanged += (d, e) => UpdateViewProperties();
			Owner.Do(result.Attach);
			return result;
		}
		protected internal ControllerBehavior HeaderMenuCustomizationController { get { return headerMenuCustomizationController ?? (headerMenuCustomizationController = CreateHeaderMenuCustomizationController()); } }
		#endregion Properties
		public TabControlViewBase() {
			CloseTabCommand = new DelegateCommand(
				() => Owner.With(x => x.SelectedContainer).Do(x => x.Close()), 
				() => Owner.With(x => x.SelectedContainer).Return(x => x.CanClose(), () => false), false);
			ActualNewTabCommand = new DelegateCommand(AddNewTabItem, CanAddNewTabItem, false);
			ActualNewTabCommand.RaiseCanExecuteChanged();
		}
		internal void Assign(DXTabControl owner) {
			Owner = owner;
		}
		protected virtual void OnOwnerChanged(DXTabControl oldValue, DXTabControl newValue) {
			oldValue.Do(x => HeaderMenuCustomizationController.Detach());
			newValue.Do(x => HeaderMenuCustomizationController.Attach(x));
		}
		protected virtual internal void UpdateViewPropertiesCore() {
			UpdateNewButtonsVisibility();
			CloseTabCommand.RaiseCanExecuteChanged();
		}
		protected void UpdateViewProperties() {
			Owner.Do(x => x.UpdateViewProperties());
		}
		protected override Freezable CreateInstanceCore() {
			return (Freezable)Activator.CreateInstance(this.GetType());
		}
		protected override bool FreezeCore(bool isChecking) {
			return false;
		}
		protected override void CloneCore(Freezable sourceFreezable) {
			base.CloneCore(sourceFreezable);
			Owner = null;
		}
		void UpdateNewButtonsVisibility() {
			switch(NewButtonShowMode) {
				case NewButtonShowMode.InHeaderArea:
					SetValue(MainNewButtonVisibilityPropertyKey, Visibility.Visible);
					SetValue(PanelNewButtonVisibilityPropertyKey, Visibility.Collapsed);
					break;
				case NewButtonShowMode.InTabPanel:
					SetValue(MainNewButtonVisibilityPropertyKey, Visibility.Collapsed);
					SetValue(PanelNewButtonVisibilityPropertyKey, Visibility.Visible);
					break;
				case NewButtonShowMode.InHeaderAreaAndTabPanel:
					SetValue(MainNewButtonVisibilityPropertyKey, Visibility.Visible);
					SetValue(PanelNewButtonVisibilityPropertyKey, Visibility.Visible);
					break;
				default:
					SetValue(MainNewButtonVisibilityPropertyKey, Visibility.Collapsed);
					SetValue(PanelNewButtonVisibilityPropertyKey, Visibility.Collapsed);
					break;
			}
		}
		protected internal virtual bool CanCloseTabItem(DXTabItem item) {
			if(Owner == null || Owner.VisibleItemsCount > 1) return true;
			switch(SingleTabItemHideMode) {
				case SingleTabItemHideMode.Hide:
					return true;
				case SingleTabItemHideMode.HideAndShowNewItem:
					return !item.IsNew;
				case SingleTabItemHideMode.Disable:
				default: return false;
			}
		}
		protected internal virtual void OnTabItemClosed(DXTabItem item) {
			if(Owner == null || Owner.VisibleItemsCount > 0) return;
			if(SingleTabItemHideMode == SingleTabItemHideMode.HideAndShowNewItem)
				Owner.AddNewTabItem();
		}
		internal void AddNewTabItem() {
			if(!CanAddNewTabItem()) return;
			if(NewTabCommand == null) {
				Owner.Do(x => x.AddNewTabItem());
				return;
			}
			NewTabCommand.Execute(NewTabCommandParameter);
		}
		bool CanAddNewTabItem() {
			if(NewTabCommand == null) return true;
			lockNewTabControlCanExectureChanged = true;
			try {
				return NewTabCommand.CanExecute(NewTabCommandParameter);
			} finally { lockNewTabControlCanExectureChanged = false; }
		}
		void OnNewTabCommandChanged(ICommand oldValue, ICommand newValue) {
			oldValue.Do(x => x.CanExecuteChanged -= OnNewTabCommandCanExecuteChanged);
			newValue.Do(x => x.CanExecuteChanged += OnNewTabCommandCanExecuteChanged);
		}
		bool lockNewTabControlCanExectureChanged = false;
		void OnNewTabCommandCanExecuteChanged(object sender, EventArgs e) {
			if(lockNewTabControlCanExectureChanged) return;
			ActualNewTabCommand.RaiseCanExecuteChanged();
		}
		protected internal virtual int CoerceSelection(int index, NotifyCollectionChangedAction? originativeAction) {
			if(Owner == null) return index;
			return Owner.GetCoercedSelectedIndexCore(Owner.GetContainers(), index);
		}
	}
}
