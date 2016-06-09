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
using System.Linq;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DevExpress.Mvvm;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
using Microsoft.Win32;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Ribbon.Customization {
	public class BaseTabsCustomizationControl : UserControl {		
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyPropertyKey LeftPartItemsPropertyKey =
			DependencyProperty.RegisterReadOnly("LeftPartItems", typeof(ObservableCollection<IBarItem>), typeof(BaseTabsCustomizationControl), new PropertyMetadata(null));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty LeftPartItemsProperty = LeftPartItemsPropertyKey.DependencyProperty;
		public BaseTabsCustomizationControl() {
			SetValue(LeftPartItemsPropertyKey, new ObservableCollection<IBarItem>());
			LeftPartItems.CollectionChanged += OnLeftPartItemsCollectionChanged;
		}
		protected virtual void OnLeftPartItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.OldItems != null)
				foreach (var element in e.OldItems)
					RemoveLogicalChild(element);
			if (e.NewItems != null)
				foreach (var element in e.NewItems)
					AddLogicalChild(element);
		}
		protected override IEnumerator LogicalChildren {
			get { return new MergedEnumerator(base.LogicalChildren, LeftPartItems.GetEnumerator()); }
		}
		public ObservableCollection<IBarItem> LeftPartItems {
			get { return (ObservableCollection<IBarItem>)GetValue(LeftPartItemsProperty); }
		}
	}
	public partial class TabsCustomizationControl : BaseTabsCustomizationControl {
		public static readonly DependencyProperty LeftElementProperty =
			DependencyPropertyManager.Register("LeftElement", typeof(ElementWrapper), typeof(TabsCustomizationControl), new FrameworkPropertyMetadata(null, (d, e) => ((TabsCustomizationControl)d).OnLeftElementChanged((ElementWrapper)e.OldValue)));
		public static readonly DependencyProperty LeftSourceProperty =
			DependencyPropertyManager.Register("LeftSource", typeof(IEnumerable), typeof(TabsCustomizationControl), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty LeftTabsModeProperty =
			DependencyPropertyManager.Register("LeftTabsMode", typeof(int), typeof(TabsCustomizationControl), new FrameworkPropertyMetadata(0, (d, e) => ((TabsCustomizationControl)d).OnLeftTabsModeChanged((int)e.OldValue)));
		public static readonly DependencyProperty RibbonProperty =
			DependencyPropertyManager.Register("Ribbon", typeof(RibbonControl), typeof(TabsCustomizationControl), new FrameworkPropertyMetadata(null, (d, e) => ((TabsCustomizationControl)d).OnRibbonChanged((RibbonControl)e.OldValue)));
		public static readonly DependencyProperty RibbonWrapperProperty =
			DependencyPropertyManager.Register("RibbonWrapper", typeof(RibbonElementWrapper), typeof(TabsCustomizationControl), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty RightElementProperty =
			DependencyPropertyManager.Register("RightElement", typeof(ElementWrapper), typeof(TabsCustomizationControl), new FrameworkPropertyMetadata(null, (d, e) => ((TabsCustomizationControl)d).OnRightElementChanged((ElementWrapper)e.OldValue)));
		public static readonly DependencyProperty RightTabsModeProperty =
	DependencyPropertyManager.Register("RightTabsMode", typeof(int), typeof(TabsCustomizationControl), new FrameworkPropertyMetadata(0, (d, e) => ((TabsCustomizationControl)d).OnRightTabsModeChanged((int)e.OldValue)));
		ICommand resetAllCustomizationsCommand;
		ICommand resetCurrentPageCommand;
		ICommand importCustomizationsCommand;
		ICommand exportCustomizationsCommand;
		public event RoutedEventHandler Reseted;
		public TabsCustomizationControl() {
			InitializeComponent();
			DataContext = this;
			SetBinding(LeftElementProperty, new Binding() { Path = new PropertyPath(TreeView.SelectedItemProperty), Source = leftTreeView });
			SetBinding(RightElementProperty, new Binding() { Path = new PropertyPath(TreeView.SelectedItemProperty), Source = rightTreeView });
		}
		protected virtual bool CanResetAllCustomizations() {
			return Ribbon.RuntimeCustomizations.Any();
		}
		protected virtual void OnLeftElementChanged(ElementWrapper oldValue) {
		}		
		protected virtual void OnLeftTabsModeChanged(int oldValue) {
			UpdateLeftSource();
		}
		protected virtual void OnResetAllCustomizations() {
			if (DXMessageBox.Show(
					  this,
					  RibbonControlLocalizer.GetString(RibbonControlStringId.RibbonCustomizationStrings_ResetAllCustomizationsQuestion),
					  RibbonControlLocalizer.GetString(RibbonControlStringId.RibbonCustomizationStrings_ResetAllCustomizationsString),
					  MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
				Ribbon.RuntimeCustomizations.Undo(true, false);   
				OnRibbonChanged(Ribbon);
				if (Reseted != null)
					Reseted(this, null);
			}			 
		}
		public void OnResetCurrentPage() {
			var pageWrapper = RightElement;
			while (!(pageWrapper is RibbonPageElementWrapper) && pageWrapper != null)
				pageWrapper = pageWrapper.ParentWrapper;
			if (pageWrapper == null)
				return;
			var name = pageWrapper.OwnerObject.With(BarManagerCustomizationHelper.GetSerializationName);
			var undoCustomization = new RuntimeUndoCustomization(name);
			Ribbon.RuntimeCustomizations.Add(undoCustomization);
			undoCustomization.Apply();
			pageWrapper.Reset();
		}
		public bool CanResetCurrentPage() {
			var pageWrapper = RightElement;
			while (!(pageWrapper is RibbonPageElementWrapper) && pageWrapper != null)
				pageWrapper = pageWrapper.ParentWrapper;
			return pageWrapper != null && !pageWrapper.CreatedByCustomizationDialog && Ribbon.RuntimeCustomizations.Any(x => x.AffectedTargets.Contains(pageWrapper.OwnerObject.With(BarManagerCustomizationHelper.GetSerializationName)));
		}
		protected virtual void OnRibbonChanged(RibbonControl oldValue) {
			if (Ribbon == null) {
				RibbonWrapper = null;
				return;
			}
			RibbonWrapper = new RibbonElementWrapper(Ribbon);
			UpdateLeftSource();
		}
		protected virtual void OnRightElementChanged(ElementWrapper oldValue) {
		}		
		protected virtual void OnRightTabsModeChanged(int oldValue) {
			if (RightTabsMode == 0) {
				RibbonWrapper.ShowMainTabs = true;
				RibbonWrapper.ShowToolTabs = true;
				return;
			} if (RightTabsMode == 1) {
				RibbonWrapper.ShowMainTabs = true;
				RibbonWrapper.ShowToolTabs = false;
				return;
			} if (RightTabsMode == 2) {
				RibbonWrapper.ShowMainTabs = false;
				RibbonWrapper.ShowToolTabs = true;
				return;
			}
		}
		protected virtual void UpdateLeftSource() {
			if (LeftTabsMode == 1) {
				LeftSource = new RibbonElementWrapper(Ribbon, true).Items;				
			} else {
				LeftSource = BarNameScope
				.GetService<IElementRegistratorService>(Ribbon)
				.GetElements<IFrameworkInputElement>(Bars.Native.ScopeSearchSettings.Ancestors | Bars.Native.ScopeSearchSettings.Local)
				.OfType<BarItem>()
				.Where(x => !x.IsPrivate)
				.Select(x => new BarItemElementWrapper(x));
			}
		}
		protected virtual void OnImportCustomizations() {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "XML files|*.xml";
			if (dialog.ShowDialog() == true) {
				Ribbon.RuntimeCustomizations.Undo(true);
				Ribbon.RestoreLayout(dialog.FileName);
				OnRibbonChanged(Ribbon);
			}
		}
		protected virtual void OnExportCustomizations() {
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = "XML files|*.xml";
			if (dialog.ShowDialog() == true) {				
				Ribbon.SaveLayout(dialog.FileName);
			}
		}
		protected virtual bool CanExportCustomizations() {
			return Ribbon.RuntimeCustomizations.Any();
		}
		public ElementWrapper LeftElement {
			get { return (ElementWrapper)GetValue(LeftElementProperty); }
			set { SetValue(LeftElementProperty, value); }
		}
		public IEnumerable LeftSource {
			get { return (IEnumerable)GetValue(LeftSourceProperty); }
			set { SetValue(LeftSourceProperty, value); }
		}				
		public int LeftTabsMode {
			get { return (int)GetValue(LeftTabsModeProperty); }
			set { SetValue(LeftTabsModeProperty, value); }
		}
		public ICommand ResetAllCustomizationsCommand {
			get { return resetAllCustomizationsCommand ?? (resetAllCustomizationsCommand = new DelegateCommand(OnResetAllCustomizations, CanResetAllCustomizations, true)); }
		}
		public ICommand ResetCurrentPageCommand {
			get { return resetCurrentPageCommand ?? (resetCurrentPageCommand = new DelegateCommand(OnResetCurrentPage, CanResetCurrentPage, true)); }
		}	 
		public ICommand ImportCustomizationsCommand {
			get { return importCustomizationsCommand ?? (importCustomizationsCommand = new DelegateCommand(OnImportCustomizations)); }
		}		
		public ICommand ExportCustomizationsCommand {
			get { return exportCustomizationsCommand ?? (exportCustomizationsCommand = new DelegateCommand(OnExportCustomizations, CanExportCustomizations, true)); }
		}		
		public RibbonControl Ribbon {
			get { return (RibbonControl)GetValue(RibbonProperty); }
			set { SetValue(RibbonProperty, value); }
		}
		public RibbonElementWrapper RibbonWrapper {
			get { return (RibbonElementWrapper)GetValue(RibbonWrapperProperty); }
			set { SetValue(RibbonWrapperProperty, value); }
		}				
		public ElementWrapper RightElement {
			get { return (ElementWrapper)GetValue(RightElementProperty); }
			set { SetValue(RightElementProperty, value); }
		}		
		public int RightTabsMode {
			get { return (int)GetValue(RightTabsModeProperty); }
			set { SetValue(RightTabsModeProperty, value); }
		}
	}
	public class CustomizationHelpers {
		public static readonly DependencyProperty StretchElementProperty =
			DependencyPropertyManager.RegisterAttached("StretchElement", typeof(string), typeof(CustomizationHelpers), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnStretchElementPropertyChanged)));
		protected static void OnStretchElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var fe = d as FrameworkElement;
			if (fe == null)
				return;
			d.Dispatcher.BeginInvoke(new Action(() => {
				if (string.Equals((string)e.NewValue, fe.Name))
					fe.HorizontalAlignment = HorizontalAlignment.Stretch;
			}));
		}
		public static string GetStretchElement(DependencyObject obj) { return (string)obj.GetValue(StretchElementProperty); }
		public static void SetStretchElement(DependencyObject obj, string value) { obj.SetValue(StretchElementProperty, value); }
	}
	public class BarItemLinkToImageNameConverter : IValueConverter {
		static BarItemLinkToImageNameConverter() {
		}
		public static object Convert(object value) {
			return new BarItemLinkToImageNameConverter().Convert(value, null, null, null);
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value as BarItemLink != null) value = (value as BarItemLink).Item;
			bool isSplit = value is BarSplitButtonItem || value is BarSplitButtonItemLink;
			bool isSub = value is BarSubItem || value is BarSubItemLink;
			bool isEdit = value is BarEditItem || value is BarEditItemLink;
			if(isSplit && !isSub && !isEdit)
				return "barSplitButtonItemCustomizationIcon.png";
			if(!isSplit && isSub && !isEdit)
				return "barSubItemCustomizationIcon.png";
			if(!isSplit && !isSub && isEdit)
				return "barEditItemCustomizationIcon.png";
			return String.Empty;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class RightSelectedItemToIsSelectedConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value != null) return true;
			return false;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}		  
}
