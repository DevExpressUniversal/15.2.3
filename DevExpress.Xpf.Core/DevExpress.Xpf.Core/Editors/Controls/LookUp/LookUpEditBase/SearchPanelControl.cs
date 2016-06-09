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

#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Automation.Peers;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Xpf.Core.Native;
#if !SL
using DevExpress.Data.Access;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Data.Mask;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Automation;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
#else
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using DevExpress.Data.Mask;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Automation;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using SelectionChangedEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventArgs;
using SelectionChangedEventHandler = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventHandler;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
#endif
#endregion
namespace DevExpress.Xpf.Editors {
	public enum CriteriaOperatorType {
		And,
		Or,
	}
	public enum FilterByColumnsMode {
		Default,
		Custom,
	}
	public class TextHighlightingProperties {
		public TextHighlightingProperties(string text, FilterCondition filterCondition) {
			Text = text;
			FilterCondition = filterCondition;
		}
		public string Text { get; private set; }
		public FilterCondition FilterCondition { get; private set; }
	}
	public interface ISearchPanelColumnProviderBase {
		string GetSearchText();
		void UpdateColumns(FilterByColumnsMode mode);
		bool UpdateFilter(string searchText, FilterCondition filterCondition, CriteriaOperator filterCriteria);
		ObservableCollection<string> CustomColumns { get; set; }
	}
	public interface ISearchPanelColumnProviderOwner {
		ISearchPanelColumnProviderEx SearchPanelColumnProvider { get; }
	}
	public interface ISearchPanelColumnProvider : ISearchPanelColumnProviderBase {
		IEnumerable<string> Columns { get; }
	}
	public interface ISearchPanelColumnProviderEx : ISearchPanelColumnProviderBase {
		IList Columns { get; }
		bool IsServerMode { get; }
		IList ColumnsForceWithoutPrefix { get; }
		IList<CustomFilterColumn> CustomFilterColumns { get; }
	}
	public class CustomFilterColumn {
		public CustomFilterColumn() { }
		public CustomFilterColumn(DevExpress.Data.IDataColumnInfo column, FilterCondition filterCondition) {
			Column = column;
			FilterCondition = filterCondition;
		}
		public DevExpress.Data.IDataColumnInfo Column { get; set; }
		public FilterCondition FilterCondition { get; set; }
	}
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	public class SearchControl : Control {
		public static readonly DependencyProperty ShowMRUButtonProperty;
		public static readonly DependencyProperty ShowFindButtonProperty;
		public static readonly DependencyProperty ShowClearButtonProperty;
		public static readonly DependencyProperty ShowCloseButtonProperty;
		public static readonly DependencyProperty SearchTextProperty;
		public static readonly DependencyProperty FindModeProperty;
		public static readonly DependencyProperty FilterCriteriaProperty;
		public static readonly DependencyProperty FilterConditionProperty;
		public static readonly DependencyProperty FilterByColumnsModeProperty;
		public static readonly DependencyProperty ColumnProviderProperty;
		public static readonly DependencyProperty CriteriaOperatorTypeProperty;
		public static readonly DependencyProperty PostModeProperty;
		public static readonly DependencyProperty CloseCommandProperty;
		public static readonly DependencyProperty MRUProperty;
		public static readonly DependencyProperty MRULengthProperty;
		public static readonly DependencyProperty SearchTextPostDelayProperty;
		public static readonly DependencyProperty ImmediateMRUPopupProperty;
		public static readonly DependencyProperty NullTextProperty;
		public static readonly DependencyProperty SourceControlProperty;
		public static readonly DependencyProperty IsEditorTabStopProperty;
		public static readonly DependencyProperty SearchControlPropertyProviderProperty;
		static SearchControl() {
			Type ownerType = typeof(SearchControl);
			ShowMRUButtonProperty = DependencyPropertyManager.Register("ShowMRUButton", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, (d, e) => ((SearchControl)d).OnActualImmediateMRUPopupChanged()));
			ShowClearButtonProperty = DependencyPropertyManager.Register("ShowClearButton", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, (d, e) => ((SearchControl)d).ShowClearButtonChanged()));
			ShowCloseButtonProperty = DependencyPropertyManager.Register("ShowCloseButton", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None));
			ShowFindButtonProperty = DependencyPropertyManager.Register("ShowFindButton", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, (d, e) => ((SearchControl)d).ShowFindButtonChanged((bool)e.OldValue, (bool)e.NewValue)));
			SearchTextProperty = DependencyPropertyManager.Register("SearchText", typeof(string), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (d, e) => ((SearchControl)d).SearchTextChanged((string)e.OldValue, (string)e.NewValue)));
			FindModeProperty = DependencyPropertyManager.Register("FindMode", typeof(FindMode), ownerType,
				new FrameworkPropertyMetadata(FindMode.Always, FrameworkPropertyMetadataOptions.None, (d, e) => ((SearchControl)d).FindModeChanged((FindMode)e.NewValue)));
			FilterConditionProperty = DependencyPropertyManager.Register("FilterCondition", typeof(FilterCondition), ownerType,
				new FrameworkPropertyMetadata(FilterCondition.StartsWith, (d, e) => ((SearchControl)d).FilterConditionChanged((FilterCondition)e.NewValue)));
			FilterCriteriaProperty = DependencyPropertyManager.Register("FilterCriteria", typeof(CriteriaOperator), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (d, e) => ((SearchControl)d).FilterCriteriaChanged((CriteriaOperator)e.OldValue, (CriteriaOperator)e.NewValue)));
			FilterByColumnsModeProperty = DependencyPropertyManager.Register("FilterByColumnsMode", typeof(FilterByColumnsMode), ownerType,
				new FrameworkPropertyMetadata(FilterByColumnsMode.Default, (d, e) => ((SearchControl)d).FilterByColumnsModeChanged((FilterByColumnsMode)e.NewValue)));
			ColumnProviderProperty = DependencyPropertyManager.Register("ColumnProvider", typeof(ISearchPanelColumnProviderBase), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((SearchControl)d).ColumnProviderChanged((ISearchPanelColumnProviderBase)e.NewValue)));
			CriteriaOperatorTypeProperty = DependencyPropertyManager.Register("CriteriaOperatorType", typeof(CriteriaOperatorType), ownerType,
				new FrameworkPropertyMetadata(CriteriaOperatorType.Or, FrameworkPropertyMetadataOptions.None, (d, e) => ((SearchControl)d).CriteriaOperatorTypeChanged((CriteriaOperatorType)e.NewValue)));
			PostModeProperty = DependencyPropertyManager.Register("PostMode", typeof(PostMode?), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((SearchControl)d).OnPostModeChanged()));
			CloseCommandProperty = DependencyPropertyManager.Register("CloseCommand", typeof(ICommand), ownerType, new FrameworkPropertyMetadata(null));
			MRUProperty = DependencyPropertyManager.Register("MRU", typeof(IList<string>), ownerType, new FrameworkPropertyMetadata(null));
			MRULengthProperty = DependencyPropertyManager.Register("MRULength", typeof(int), ownerType, new FrameworkPropertyMetadata(7, (d, e) => ((SearchControl)d).OnMRULengthChanged()));
			SearchControlPropertyProviderProperty = DependencyPropertyManager.Register("SearchControlPropertyProvider", typeof(SearchControlPropertyProvider), ownerType, new FrameworkPropertyMetadata(null));
			SearchTextPostDelayProperty = DependencyPropertyManager.Register("SearchTextPostDelay", typeof(int), ownerType, new FrameworkPropertyMetadata(1000));
			ImmediateMRUPopupProperty = DependencyPropertyManager.Register("ImmediateMRUPopup", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((SearchControl)d).OnActualImmediateMRUPopupChanged()));
			NullTextProperty = DependencyPropertyManager.Register("NullText", typeof(string), typeof(SearchControl), new FrameworkPropertyMetadata(null));
			IsEditorTabStopProperty = DependencyPropertyManager.Register("IsEditorTabStop", typeof(bool), typeof(SearchControl), new FrameworkPropertyMetadata(true));
			SourceControlProperty = DependencyPropertyManager.Register("SourceControl", typeof(object), typeof(SearchControl), new FrameworkPropertyMetadata(null, (d, e) => ((SearchControl)d).OnSourceControlChanged()));
		}
		void OnMRULengthChanged() {
			if(MRULength <= 0) {
				MRU.Clear();
				return;
			}
			while(MRU.Count > MRULength)
				MRU.RemoveAt(MRU.Count - 1);
		}
		public bool ShowMRUButton {
			get { return (bool)GetValue(ShowMRUButtonProperty); }
			set { SetValue(ShowMRUButtonProperty, value); }
		}
		public IList<string> MRU {
			get { return (IList<string>)GetValue(MRUProperty); }
			set { SetValue(MRUProperty, value); }
		}
		public int MRULength {
			get { return (int)GetValue(MRULengthProperty); }
			set { SetValue(MRULengthProperty, value); }
		}
		public bool ShowClearButton {
			get { return (bool)GetValue(ShowClearButtonProperty); }
			set { SetValue(ShowClearButtonProperty, value); }
		}
		public bool ShowCloseButton {
			get { return (bool)GetValue(ShowCloseButtonProperty); }
			set { SetValue(ShowCloseButtonProperty, value); }
		}
		public bool ShowFindButton {
			get { return (bool)GetValue(ShowFindButtonProperty); }
			set { SetValue(ShowFindButtonProperty, value); }
		}
		public FilterByColumnsMode FilterByColumnsMode {
			get { return (FilterByColumnsMode)GetValue(FilterByColumnsModeProperty); }
			set { SetValue(FilterByColumnsModeProperty, value); }
		}
		public FilterCondition FilterCondition {
			get { return (FilterCondition)GetValue(FilterConditionProperty); }
			set { SetValue(FilterConditionProperty, value); }
		}
		public CriteriaOperator FilterCriteria {
			get { return (CriteriaOperator)GetValue(FilterCriteriaProperty); }
			set { SetValue(FilterCriteriaProperty, value); }
		}
		public string SearchText {
			get { return (string)GetValue(SearchTextProperty); }
			set { SetValue(SearchTextProperty, value); }
		}
		public FindMode FindMode {
			get { return (FindMode)GetValue(FindModeProperty); }
			set { SetValue(FindModeProperty, value); }
		}
		public ISearchPanelColumnProviderBase ColumnProvider {
			get { return (ISearchPanelColumnProviderBase)GetValue(ColumnProviderProperty); }
			set { SetValue(ColumnProviderProperty, value); }
		}
		public CriteriaOperatorType CriteriaOperatorType {
			get { return (CriteriaOperatorType)GetValue(CriteriaOperatorTypeProperty); }
			set { SetValue(CriteriaOperatorTypeProperty, value); }
		}
		public PostMode? PostMode {
			get { return (PostMode?)GetValue(PostModeProperty); }
			set { SetValue(PostModeProperty, value); }
		}
		public ICommand CloseCommand {
			get { return (ICommand)GetValue(CloseCommandProperty); }
			set { SetValue(CloseCommandProperty, value); }
		}
		public int SearchTextPostDelay {
			get { return (int)GetValue(SearchTextPostDelayProperty); }
			set { SetValue(SearchTextPostDelayProperty, value); }
		}
		public bool? ImmediateMRUPopup {
			get { return (bool?)GetValue(ImmediateMRUPopupProperty); }
			set { SetValue(ImmediateMRUPopupProperty, value); }
		}
		public string NullText {
			get { return (string)GetValue(NullTextProperty); }
			set { SetValue(NullTextProperty, value); }
		}
		public object SourceControl {
			get { return (object)GetValue(SourceControlProperty); }
			set { SetValue(SourceControlProperty, value); }
		}
		public bool IsEditorTabStop {
			get { return (bool)GetValue(IsEditorTabStopProperty); }
			set { SetValue(IsEditorTabStopProperty, value); }
		}
		public SearchControlPropertyProvider SearchControlPropertyProvider {
			get { return (SearchControlPropertyProvider)GetValue(SearchControlPropertyProviderProperty); }
			set { SetValue(SearchControlPropertyProviderProperty, value); }
		}
		public SearchControl() {
			this.SetDefaultStyleKey(typeof(SearchControl));
			MRU = new ObservableCollection<string>();
			SearchControlPropertyProvider = new SearchControlPropertyProvider(this);
			SearchControlPropertyProvider.DisplayTextChanged += (o, e) => DisplayTextChanged();
		}
		public bool DoValidate() {
			if (EditorControl == null)
				return true;
			return EditorControl.DoValidate();
		}
		void OnSourceControlChanged() {
			if(SourceControl == null) {
				ColumnProvider = null;
			} else if(SourceControl is ISelectorEdit)
				ColumnProvider = new SelectorEditColumnProvider() { OwnerEdit = SourceControl as ISelectorEdit, AllowFilter = true };
			else
				throw new NotImplementedException();
		}
		void OnActualImmediateMRUPopupChanged() {
			if(SearchControlPropertyProvider != null)
				SearchControlPropertyProvider.UpdateActualImmediateMRUPopup(this);
		}
		protected virtual bool GetIsFindCommandCanExecute() {
			return FindMode != FindMode.Always;
		}
		protected internal virtual void OnFindCommandExecuted() {
			if(!GetIsFindCommandCanExecute())
				return;
			UpdateFilterCriteria();
			if(ColumnProvider != null)
				UpdateColumnProviderFilter(FilterCriteria);
			UpdateMRU();
		}
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsKeyboardFocusWithinChanged(e);
			if((bool)e.NewValue)
				return;
			if(SearchControlPropertyProvider != null && SearchControlPropertyProvider.ActualPostMode == Editors.PostMode.Immediate)
				SearchControlPropertyProvider.FindCommand.Execute(null);
			if(isSearchResultHaveValue)
				UpdateMRU();
			isSearchResultHaveValue = false;
		}
		ButtonEdit editor = null;
		protected ButtonEdit EditorControl { get { return editor; } }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			editor = GetEditorControl();
			if(editor == null)
				return;
			SearchControlPropertyProvider.BindEditorProperties(this, editor);
			if(editor is PopupBaseEdit) {
				((PopupBaseEdit)editor).PopupClosed += SearchControl_PopupClosed;
			}
			UpdateClearButtonVisibility();
		}
		bool popupClosed = false;
		void SearchControl_PopupClosed(object sender, ClosePopupEventArgs e) {
			popupClosed = e.CloseMode == PopupCloseMode.Cancel || e.EditValue == null;
		}
		protected virtual ButtonEdit GetEditorControl() {
			return GetTemplateChild("editor") as ButtonEdit;
		}
		void ShowClearButtonChanged() {
			UpdateClearButtonVisibility();
		}
		protected virtual void FilterByColumnsChanged(IEnumerable<string> oldValue, IEnumerable<string> newValue) {
			UpdateFilterCriteria();
		}
		protected virtual void FilterCriteriaChanged(CriteriaOperator oldValue, CriteriaOperator newValue) {
			if(FindMode == FindMode.FindClick && !String.IsNullOrWhiteSpace(SearchText))
				return;
			if(ColumnProvider != null)
				UpdateColumnProviderFilter(newValue);
			if(SearchControlPropertyProvider.FindCommand == null)
				return;
			if(SearchControlPropertyProvider.FindCommand.CanExecute(newValue))
				SearchControlPropertyProvider.FindCommand.Execute(newValue);
		}
		void CriteriaOperatorTypeChanged(CriteriaOperatorType criteriaOperatorType) {
			UpdateFilterCriteria();
		}
		protected virtual void DisplayTextChanged() {
			UpdateClearButtonVisibility();
		}
		bool isSearchResultHaveValue = false;
		void UpdateColumnProviderFilter(CriteriaOperator filterCriteria) {
			isSearchResultHaveValue = ColumnProvider.UpdateFilter(SearchText, FilterCondition, filterCriteria);
		}
		protected virtual void SearchTextChanged(string oldValue, string newValue) {
			UpdateClearButtonVisibility();
			if(FindMode == FindMode.FindClick)
				return;
			UpdateFilterCriteria();
		}
		protected internal virtual bool SaveMRUOnStringChanged {
			get { return true; }
		}
		public void UpdateMRU() {
			if(MRULength == 0)
				return;
			if(MRU.Count > MRULength)
				OnMRULengthChanged();
			if(String.IsNullOrWhiteSpace(SearchText))
				return;
			string trimmedText = SearchText.Trim();
			if(MRU.Count == 0) {
				MRU.Add(trimmedText);
				return;
			}
			int searchIndex = MRU.IndexOf(trimmedText);
			switch(MRU.IndexOf(trimmedText)) {
				case 0: return;
				case -1:
					if(MRU.Count >= MRULength)
						MRU.RemoveAt(MRU.Count - 1);
					MRU.Insert(0, trimmedText);
					return;
				default:
					MRU.Remove(trimmedText);
					MRU.Insert(0, trimmedText);
					return;
			}
		}
		void OnPostModeChanged() {
			SearchControlPropertyProvider.UpdatePostMode(this);
		}
		protected internal virtual void OnClearSearchTextCommandExecuted() {
			if(editor == null)
				Dispatcher.BeginInvoke(new Action(() => { SearchText = null; }));
			else {
				editor.EditValue = null;
			}
		}
		protected virtual void FindModeChanged(FindMode value) {
			SearchControlPropertyProvider.ActualShowFindButton = ShowFindButton && value == FindMode.FindClick;
			SearchControlPropertyProvider.UpdatePostMode(this);
			if(value != FindMode.FindClick)
				UpdateFilterCriteria();
		}
		protected virtual void ColumnProviderChanged(ISearchPanelColumnProviderBase columnProvider) {
			UpdateColumnProvider();
		}
		protected virtual void FilterByColumnsModeChanged(FilterByColumnsMode filterByColumnsMode) {
			UpdateColumnProvider();
		}
		public virtual void UpdateColumnProvider() {
			if(ColumnProvider != null)
				ColumnProvider.UpdateColumns(FilterByColumnsMode);
			UpdateFilterCriteria();
		}
		protected virtual void FilterConditionChanged(FilterCondition filterCondition) {
			UpdateFilterCriteria();
		}
		void ShowFindButtonChanged(bool oldValue, bool newValue) {
			if(FindMode != FindMode.FindClick)
				return;
			SearchControlPropertyProvider.ActualShowFindButton = newValue;
		}
		protected virtual void UpdateFilterCriteria() {
			if(string.IsNullOrEmpty(SearchText) || ColumnProvider == null) {
				if(!ReferenceEquals(FilterCriteria, null))
					FilterCriteria = null;
				if(ColumnProvider != null && !string.IsNullOrEmpty(ColumnProvider.GetSearchText()))
					FilterCriteriaChanged(null, null);
				return;
			}
			FilterCriteria = SearchControlHelper.GetCriteriaOperator(ColumnProvider, FilterCondition, SearchText, CriteriaOperatorType);
		}
		void UpdateClearButtonVisibility() {
			if(!ShowClearButton) {
				SearchControlPropertyProvider.ActualShowClearButton = false;
				return;
			}
			if(editor != null)
				SearchControlPropertyProvider.ActualShowClearButton = SearchControlPropertyProvider.IsNullTextVisible ? false : !String.IsNullOrEmpty(SearchControlPropertyProvider.DisplayText);
			else
				SearchControlPropertyProvider.ActualShowClearButton = !String.IsNullOrEmpty(SearchText);
		}
		protected override void OnPreviewKeyUp(KeyEventArgs e) {
			base.OnPreviewKeyUp(e);
			if(!ModifierKeysHelper.NoModifiers(ModifierKeysHelper.GetKeyboardModifiers(e)))
				return;
			switch(e.Key){
				case Key.Enter:
					if(SearchControlPropertyProvider != null) {
						SearchControlPropertyProvider.FindCommand.Execute(null);
						e.Handled = true;
					}
					break;
				case Key.Escape:
					if(editor != null) {
						if(popupClosed) {
							e.Handled = true;
							popupClosed = false;
							return;
						}
						editor.DoValidate();
					}
					if(!String.IsNullOrEmpty(SearchText)) {
						SearchText = null;
						if(FindMode == Editors.FindMode.FindClick) UpdateFilterCriteria();
						e.Handled = true;
					}
					break;
			}
			popupClosed = false;
		}
	}
	public class SearchControlPropertyProvider : FrameworkElement {
		internal static readonly DependencyPropertyKey ActualShowClearButtonPropertyKey;
		public static readonly DependencyProperty ActualShowClearButtonProperty;
		public static readonly DependencyProperty DisplayTextProperty;
		public static readonly DependencyProperty IsNullTextVisibleProperty;
		static readonly DependencyPropertyKey ActualShowFindButtonPropertyKey;
		public static readonly DependencyProperty ActualShowFindButtonProperty;
		public static readonly DependencyProperty FindCommandProperty;
		static readonly DependencyPropertyKey ActualImmediateMRUPopupPropertyKey;
		public static readonly DependencyProperty ActualImmediateMRUPopupProperty;
		public static readonly DependencyProperty CloseCommandInternalProperty;
		static readonly DependencyPropertyKey ActualPostModePropertyKey;
		public static readonly DependencyProperty ActualPostModeProperty;
		public static readonly DependencyProperty ClearSearchTextCommandProperty;
		static SearchControlPropertyProvider() {
			Type ownerType = typeof(SearchControlPropertyProvider);
			ActualShowClearButtonPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualShowClearButton", typeof(bool), ownerType,
new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None));
			ActualShowClearButtonProperty = ActualShowClearButtonPropertyKey.DependencyProperty;
			DisplayTextProperty = DependencyPropertyManager.Register("DisplayText", typeof(string), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (d, e) => ((SearchControlPropertyProvider)d).OnDisplayTextChanged()));
			IsNullTextVisibleProperty = DependencyPropertyManager.Register("IsNullTextVisible", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, (d, e) => ((SearchControlPropertyProvider)d).OnDisplayTextChanged()));
			ActualShowFindButtonPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualShowFindButton", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			ActualShowFindButtonProperty = ActualShowFindButtonPropertyKey.DependencyProperty;
			FindCommandProperty = DependencyPropertyManager.Register("FindCommand", typeof(ICommand), ownerType, new FrameworkPropertyMetadata(null));
			ActualImmediateMRUPopupPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualImmediateMRUPopup", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None));
			ActualImmediateMRUPopupProperty = ActualImmediateMRUPopupPropertyKey.DependencyProperty;
			ActualPostModePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualPostMode", typeof(PostMode), ownerType, new FrameworkPropertyMetadata(PostMode.Delayed, FrameworkPropertyMetadataOptions.None));
			ActualPostModeProperty = ActualPostModePropertyKey.DependencyProperty;
			CloseCommandInternalProperty = DependencyPropertyManager.Register("CloseCommandInternal", typeof(ICommand), ownerType, new FrameworkPropertyMetadata(null));
			ClearSearchTextCommandProperty = DependencyPropertyManager.Register("ClearSearchTextCommand", typeof(ICommand), ownerType, new FrameworkPropertyMetadata(null));
		}
		public SearchControlPropertyProvider(SearchControl SearchControl) {
			FindCommand = DelegateCommandFactory.Create<object>(command => SearchControl.OnFindCommandExecuted(), false);
			ClearSearchTextCommand = DelegateCommandFactory.Create<object>(command => SearchControl.OnClearSearchTextCommandExecuted(), false);
		}
		public void BindEditorProperties(SearchControl searchControl, ButtonEdit editor) {
			Binding bindingDisplayTextProperty = new Binding() { Source = editor, Path = new PropertyPath(ButtonEdit.DisplayTextProperty.GetName()), Mode = BindingMode.OneWay };
			SetBinding(SearchControlPropertyProvider.DisplayTextProperty, bindingDisplayTextProperty);
			Binding bindingIsNullTextVisibleProperty = new Binding() { Source = editor, Path = new PropertyPath(ButtonEdit.IsNullTextVisibleProperty.GetName()), Mode = BindingMode.OneWay };
			SetBinding(SearchControlPropertyProvider.IsNullTextVisibleProperty, bindingIsNullTextVisibleProperty);
			CloseCommandInternal = DelegateCommandFactory.Create<object>(o => {
				editor.DoValidate();
				if(searchControl.CloseCommand != null)
					searchControl.CloseCommand.Execute(null);
			}, false);
			editor.KeyDown += (o, e) => {
				if(e.Key == Key.Enter && (ModifierKeysHelper.GetKeyboardModifiers(e) == ModifierKeys.None))
					searchControl.UpdateMRU();
			};
			editor.EditValueChanged += (o, e) => {
				if(searchControl.SaveMRUOnStringChanged)
					searchControl.UpdateMRU();
			};
		}
		internal void UpdatePostMode(SearchControl searchControl) {
			if(!searchControl.PostMode.HasValue)
				ActualPostMode = searchControl.FindMode == FindMode.FindClick ? PostMode.Immediate : PostMode.Delayed;
			else
				ActualPostMode = searchControl.PostMode.Value;
		}
		void OnDisplayTextChanged() {
			if(DisplayTextChanged != null)
				DisplayTextChanged(this, EventArgs.Empty);
		}
		public void UpdateActualImmediateMRUPopup(SearchControl SearchControl) {
			ActualImmediateMRUPopup = SearchControl.ImmediateMRUPopup.HasValue ?
				SearchControl.ImmediateMRUPopup.Value : false; 
		}
		public bool ActualShowClearButton {
			get { return (bool)GetValue(ActualShowClearButtonProperty); }
			internal set { this.SetValue(ActualShowClearButtonPropertyKey, value); }
		}
		public string DisplayText {
			get { return (string)GetValue(DisplayTextProperty); }
			set { SetValue(DisplayTextProperty, value); }
		}
		public bool IsNullTextVisible {
			get { return (bool)GetValue(IsNullTextVisibleProperty); }
			set { SetValue(IsNullTextVisibleProperty, value); }
		}
		public bool ActualShowFindButton {
			get { return (bool)GetValue(ActualShowFindButtonProperty); }
			internal set { this.SetValue(ActualShowFindButtonPropertyKey, value); }
		}
		public ICommand FindCommand {
			get { return (ICommand)GetValue(FindCommandProperty); }
			set { SetValue(FindCommandProperty, value); }
		}
		public ICommand ClearSearchTextCommand {
			get { return (ICommand)GetValue(ClearSearchTextCommandProperty); }
			set { SetValue(ClearSearchTextCommandProperty, value); }
		}
		public bool ActualImmediateMRUPopup {
			get { return (bool)GetValue(ActualImmediateMRUPopupProperty); }
			internal set { this.SetValue(ActualImmediateMRUPopupPropertyKey, value); }
		}
		public PostMode ActualPostMode {
			get { return (PostMode)GetValue(ActualPostModeProperty); }
			internal set { this.SetValue(ActualPostModePropertyKey, value); } 
		}
		public ICommand CloseCommandInternal {
			get { return (ICommand)GetValue(CloseCommandInternalProperty); }
			set { SetValue(CloseCommandInternalProperty, value); }
		}
		public event EventHandler DisplayTextChanged;
	}
}
