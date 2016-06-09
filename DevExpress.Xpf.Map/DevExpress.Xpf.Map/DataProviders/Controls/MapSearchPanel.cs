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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Map.Localization;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	[NonCategorized]
	public class MapSearchPanel : Control {
		internal static readonly DependencyPropertyKey SearchResultsPropertyKey = DependencyPropertyManager.RegisterReadOnly("SearchResults",
			typeof(ObservableCollection<string>), typeof(MapSearchPanel), new PropertyMetadata());
		public static readonly DependencyProperty SearchResultsProperty = SearchResultsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty IsBusyProperty = DependencyPropertyManager.Register("IsBusy",
			typeof(bool), typeof(MapSearchPanel), new PropertyMetadata(false, IsBusyPropertyChanged));
		public ObservableCollection<string> SearchResults {
			get { return (ObservableCollection<string>)GetValue(SearchResultsProperty); }
		}
		public bool IsBusy {
			get { return (bool)GetValue(IsBusyProperty); }
			set { SetValue(IsBusyProperty, value); }
		}
		TextEdit textInput;
		ListBox resultsList;
		Button clearButton;
		ToggleButton dropDownButton;
		bool showldFocusSearchString = false;
		bool areAlternateRequestsListed;
		public string SearchString { get { return textInput != null ? textInput.Text : ""; } }
		public string SelectedResult { get { return resultsList != null && resultsList.SelectedItem != null ? resultsList.SelectedItem.ToString() : ""; } }
		public bool AreAlternateRequestsListed { get { return areAlternateRequestsListed; } }
		public event EventHandler SearchStringChanged;
		public event SelectionChangedEventHandler SelectedResultChanged;
		public event RoutedEventHandler DropDownButtonClick;
		static void IsBusyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapSearchPanel panel = d as MapSearchPanel;
			if (panel != null)
				panel.UpdateState();
		}
		public MapSearchPanel() {
			DefaultStyleKey = typeof(MapSearchPanel);
			this.SetValue(SearchResultsPropertyKey, new ObservableCollection<string>());
			SearchResults.CollectionChanged += new NotifyCollectionChangedEventHandler(SearchResults_CollectionChanged);
		}
		void UpdateState() {
			if (clearButton != null && textInput != null)
				clearButton.Visibility = !IsBusy && !String.IsNullOrEmpty(textInput.Text) ? Visibility.Visible : Visibility.Collapsed;
			if (dropDownButton != null)
				dropDownButton.IsChecked = AreAlternateRequestsListed ? true : false;
			UpdateResultListVisibility();
		}
		void ResultsList_KeyUp(object sender, KeyEventArgs e) {
			if (resultsList.SelectedIndex == 0 && e.Key == Key.Up)
				if (!showldFocusSearchString)
					showldFocusSearchString = true;
				else {
					resultsList.SelectedIndex = -1;
					textInput.Focus();
					showldFocusSearchString = false;
				}
			else {
				showldFocusSearchString = false;
				if (e.Key == Key.Enter)
					StartAlternateSearch();
			}
		}
		void TextInput_KeyUp(object sender, KeyEventArgs e) {
			if (resultsList != null && e.Key == Key.Down && resultsList.Items.Count > 0) {
				resultsList.SelectedIndex = 0;
				resultsList.Focus();
				showldFocusSearchString = true;
			}
		}
		void OnTextInputValueChanged(object sender, EditValueChangedEventArgs e) {
			if (SearchStringChanged != null)
				SearchStringChanged(this, e);
		}
		void TextInput_Loaded(object sender, RoutedEventArgs e) {
			TextBox editor = LayoutHelper.FindElementByName(textInput, "PART_Editor") as TextBox;
			if (editor != null)
				editor.CaretBrush = Brushes.Black;
		}
		void ResultsList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (SelectedResultChanged != null)
				SelectedResultChanged(this, e);
		}
		void ResultsList_MouseUp(object sender, MouseButtonEventArgs e) {
			if (SelectedResultChanged != null)
				SelectedResultChanged(this, new SelectionChangedEventArgs(e.RoutedEvent, new List<object>(), new List<object>()));
		}
		void ClearButton_Click(object sender, RoutedEventArgs e) {
			Reset();
		}
		void DropDownButton_Click(object sender, RoutedEventArgs e) {
			if (DropDownButtonClick != null)
				DropDownButtonClick(this, e);
			areAlternateRequestsListed = !areAlternateRequestsListed;
		}
		void ResultsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			StartAlternateSearch();
		}
		void SearchResults_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (resultsList != null)
				UpdateResultListVisibility();
		}
		void StartAlternateSearch() {
			if (textInput != null && resultsList != null && resultsList.SelectedItem != null) {
				textInput.Focus();
				textInput.Text = resultsList.SelectedItem.ToString();
				resultsList.SelectedIndex = -1;
			}
		}
		void UpdateResultListVisibility() {
			if (SearchResults.Count == 0 || IsBusy)
				resultsList.Visibility = Visibility.Collapsed;
			else {
				resultsList.SelectedIndex = 0;
				resultsList.Visibility = Visibility.Visible;
			}
		}
		internal void ShowAlternateResultsButton(bool isShowButton) {
			if (dropDownButton != null) {
				dropDownButton.Visibility = isShowButton ? Visibility.Visible : Visibility.Collapsed;
				if (!isShowButton) {
					areAlternateRequestsListed = false;
					UpdateState();
				}
			}
		}
		internal void Reset() {
			if (textInput != null)
				textInput.Text = "";
			if (resultsList != null)
				SearchResults.Clear();
			ShowAlternateResultsButton(false);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			textInput = LayoutHelper.FindElementByName(this, "PART_TextInput") as TextEdit;
			if (textInput != null) {
				textInput.NullText = MapLocalizer.GetString(MapStringId.SearchPanelNullText);
				textInput.EditValueChanged += OnTextInputValueChanged;
				textInput.PreviewKeyDown += new KeyEventHandler(TextInput_KeyUp);
				textInput.Loaded += TextInput_Loaded;
			}
			resultsList = LayoutHelper.FindElementByName(this, "PART_ResultsList") as ListBox;
			if (resultsList != null) {
				resultsList.KeyUp += new KeyEventHandler(ResultsList_KeyUp);
				resultsList.SelectionChanged += new SelectionChangedEventHandler(ResultsList_SelectionChanged);
				resultsList.MouseDoubleClick += new MouseButtonEventHandler(ResultsList_MouseDoubleClick);
				resultsList.MouseLeftButtonUp += new MouseButtonEventHandler(ResultsList_MouseUp);
			}
			clearButton = LayoutHelper.FindElementByName(this, "PART_ClearButton") as Button;
			if (clearButton != null)
				clearButton.Click+=new RoutedEventHandler(ClearButton_Click);
			dropDownButton = LayoutHelper.FindElementByName(this, "PART_DropDownButton") as ToggleButton;
			if (dropDownButton != null)
				dropDownButton.Click += new RoutedEventHandler(DropDownButton_Click);
		}
	}
	[NonCategorized]
	public class MapSearchListBox : ListBox {
		public MapSearchListBox() {
			DefaultStyleKey=typeof(MapSearchListBox);
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new MapSearchListBoxItem();
		}
	}
	[NonCategorized]
	public class MapSearchListBoxItem : ListBoxItem {
		public MapSearchListBoxItem() {
			DefaultStyleKey = typeof(MapSearchListBoxItem);
		}
	}
	public class VisibilityToCornerRadiusConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(CornerRadius))
				if (value is Visibility)
					return (Visibility)value == Visibility.Visible ? new CornerRadius(15, 15, 3, 3) : new CornerRadius(15, 15, 15, 15);
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
