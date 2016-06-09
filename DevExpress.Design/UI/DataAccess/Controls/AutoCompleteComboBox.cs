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

namespace DevExpress.Design.UI {
	using System.Collections;
	using System.Collections.ObjectModel;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Data;
	using System.Windows.Input;
	public sealed class AutoSearchTextBox : TextBox {
		#region static
		public static readonly DependencyProperty SearchResultsProperty;
		static AutoSearchTextBox() {
			var dProp = new DependencyPropertyRegistrator<AutoSearchTextBox>();
			dProp.Register("SearchResults", ref SearchResultsProperty, (IEnumerable)null);
		}
		#endregion
		string searchText = "";
		bool isCtrlPressed = false;
		bool isAltPressed = false;
		bool isBackSpacePressed = false;
		bool isInternalSet = false;
		#region Properties
		public IEnumerable SearchResults {
			get { return (IEnumerable)GetValue(SearchResultsProperty); }
			set { SetValue(SearchResultsProperty, value); }
		}
		public IEnumerable SearchCollection { get; set; }
		#endregion Properties
		protected override void OnLostFocus(RoutedEventArgs e) {
			base.OnLostFocus(e);
			AutoCompleteComboBox cb = TemplatedParent as AutoCompleteComboBox;
			if(searchItem != null && (cb != null && cb.SelectedItem != searchItem)) {
				searchText = searchItem.ToString();
				SetSearchedText(searchText);
				if(cb != null)
					cb.SelectedItem = searchItem;
			}
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			AutoCompleteComboBox cb = TemplatedParent as AutoCompleteComboBox;
			switch(e.Key) {
				case Key.Enter:
					if(searchItem != null) {
						searchText = searchItem.ToString();
						SetSearchedText(searchText);
						if(cb != null) {
							cb.SelectedItem = searchItem;
							MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
						}
						e.Handled = true;
						return;
					}
					break;
				case Key.F4:
					if(cb != null) {
						e.Handled = cb.IsDropDownOpen = true;
						return;
					}
					break;
				case Key.Up:
					searchItem = AutoCompleteComboBox.Prev(SearchResults, searchItem);
					if(searchItem != null) {
						SetSearchedText(searchItem.ToString());
						e.Handled = true;
						return;
					}
					break;
				case Key.Down:
					if(isAltPressed) {
						if(cb != null) {
							e.Handled = cb.IsDropDownOpen = true;
							return;
						}
					}
					else {
						searchItem = AutoCompleteComboBox.Next(SearchResults, searchItem);
						if(searchItem != null) {
							SetSearchedText(searchItem.ToString());
							e.Handled = true;
							return;
						}
					}
					break;
				case Key.LeftAlt & Key.RightAlt:
					isAltPressed = true;
					break;
				case Key.LeftCtrl & Key.RightCtrl:
					isCtrlPressed = true;
					break;
				case Key.Back:
					isBackSpacePressed = true;
					break;
				case Key.Space:
					if(isCtrlPressed && !string.IsNullOrEmpty(searchText)) {
						if(searchItem != null) {
							SetSearchedText(searchItem.ToString());
							e.Handled = true;
							return;
						}
					}
					break;
			}
			base.OnPreviewKeyDown(e);
		}
		protected override void OnPreviewKeyUp(KeyEventArgs e) {
			switch(e.Key) {
				case Key.LeftAlt & Key.RightAlt:
					isAltPressed = false;
					break;
				case Key.LeftCtrl & Key.RightCtrl:
					isCtrlPressed = false;
					break;
				case Key.Back:
					isBackSpacePressed = false;
					break;
			}
			base.OnPreviewKeyUp(e);
		}
		protected override void OnTextChanged(TextChangedEventArgs e) {
			if(textChangedLockCounter > 0) return;
			if(!isInternalSet && SearchCollection != null) {
				if(!string.IsNullOrEmpty(SelectedText))
					searchText = this.Text.Replace(SelectedText, "");
				else
					searchText = Text;
				TryFindResult();
			}
			base.OnTextChanged(e);
		}
		internal object searchItem;
		int textChangedLockCounter = 0;
		void TryFindResult() {
			textChangedLockCounter++;
			SearchResults = new ObservableCollection<object>();
			if(SearchCollection == null)
				return;
			string searchResult = null;
			searchItem = null;
			if(!string.IsNullOrEmpty(searchText)) {
				try {
					string searchTextUpper = searchText.ToUpper();
					foreach(object item in SearchCollection) {
						string itemString = item.ToString();
						if(itemString.StartsWith(searchTextUpper, System.StringComparison.CurrentCultureIgnoreCase)) {
							if(searchResult == null) {
								searchItem = item;
								searchResult = itemString;
							}
							((ObservableCollection<object>)SearchResults).Add(item);
						}
					}
				}
				catch { }
			}
			if(!string.IsNullOrEmpty(searchResult))
				SetSearchedText(searchResult);
			textChangedLockCounter--;
		}
		void SetSearchedText(string text) {
			int index = text.ToUpper().IndexOf(searchText.ToUpper()) + searchText.Length;
			int length = text.Length - index;
			if(!isBackSpacePressed) {
				isInternalSet = true;
				this.Text = text;
			}
			this.Select(index, length);
			isInternalSet = false;
		}
	}
	public sealed class AutoCompleteComboBox : ComboBox {
		#region static
		public static readonly DependencyProperty NullTextPromptProperty;
		public static readonly DependencyProperty IsTextFocusedProperty;
		static AutoCompleteComboBox() {
			var dProp = new DependencyPropertyRegistrator<AutoCompleteComboBox>();
			dProp.RegisterAttached("NullTextPrompt", ref NullTextPromptProperty, (string)null);
			dProp.RegisterAttached("IsTextFocused", ref IsTextFocusedProperty, false);
		}
		public static string GetNullTextPrompt(DependencyObject dObj) {
			return (string)dObj.GetValue(NullTextPromptProperty);
		}
		public static void SetNullTextPrompt(DependencyObject dObj, string value) {
			dObj.SetValue(NullTextPromptProperty, value);
		}
		public static bool GetIsTextFocused(DependencyObject dObj) {
			return (bool)dObj.GetValue(IsTextFocusedProperty);
		}
		public static void SetIsTextFocused(DependencyObject dObj, bool value) {
			dObj.SetValue(IsTextFocusedProperty, value);
		}
		#endregion
		public AutoCompleteComboBox() {
			Focusable = false;
			IsTabStop = false;
		}
		AutoSearchTextBox PartSearchTextBox;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartSearchTextBox = (AutoSearchTextBox)this.GetTemplateChild("PART_TextBox");
			PartSearchTextBox.SearchCollection = ItemsSource;
			PartSearchTextBox.PreviewMouseDown += searchTextBox_PreviewMouseDown;
			string searchText = (Few(ItemsSource) && SelectedItem != null) ? SelectedItem.ToString() : null;
			Binding binding = new Binding("SearchResults");
			binding.Source = PartSearchTextBox;
			binding.Mode = BindingMode.OneWay;
			this.SetBinding(AutoCompleteComboBox.ItemsSourceProperty, binding);
			PartSearchTextBox.TextChanged += searchTextBox_TextChanged;
			if(GetIsTextFocused(this) && !PartSearchTextBox.IsFocused)
				PartSearchTextBox.Focus();
			if(!string.IsNullOrEmpty(searchText)) {
				PartSearchTextBox.Text = searchText;
				PartSearchTextBox.Select(0, searchText.Length);
			}
		}
		void searchTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			IsDropDownOpen = false;
		}
		bool isSelectedIndexChangedInternal;
		protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			base.OnSelectionChanged(e);
			string selected = (SelectedItem != null) ? SelectedItem.ToString() : null;
			if(!string.IsNullOrEmpty(selected) && !isSelectedIndexChangedInternal && PartSearchTextBox != null)
				PartSearchTextBox.Text = selected;
			else
				isSelectedIndexChangedInternal = false;
		}
		void searchTextBox_TextChanged(object sender, TextChangedEventArgs e) {
			if(!Any(ItemsSource)) return;
			int index = IndexOf(PartSearchTextBox.SearchResults, PartSearchTextBox.searchItem);
			if(index >= 0) {
				if(index != SelectedIndex) {
					isSelectedIndexChangedInternal = true;
					SelectedIndex = index;
				}
			}
			else SelectedItem = null;
		}
		protected override void OnDropDownClosed(System.EventArgs e) {
			base.OnDropDownClosed(e);
			if(PartSearchTextBox != null && !PartSearchTextBox.IsFocused)
				PartSearchTextBox.Focus();
		}
		internal static bool Any(IEnumerable source) {
			return (source != null) && source.GetEnumerator().MoveNext();
		}
		internal static bool Few(IEnumerable source, int threshold = 3) {
			if(source == null)
				return false;
			var e = source.GetEnumerator();
			while(threshold > 0 && e.MoveNext())
				threshold--;
			return threshold > 0;
		}
		static int IndexOf(IEnumerable source, object item) {
			int index = 0;
			foreach(object obj in source) {
				if(object.Equals(obj, item))
					return index;
				index++;
			}
			return -1;
		}
		internal static object Next(IEnumerable source, object item) {
			if(source == null)
				return item;
			bool found = false;
			object first = null;
			foreach(object obj in source) {
				if(object.ReferenceEquals(first, null))
					first = obj;
				if(found)
					return obj;
				if(object.Equals(obj, item))
					found = true;
			}
			return first;
		}
		internal static object Prev(IEnumerable source, object item) {
			if(source == null)
				return item;
			object last = null;
			foreach(object obj in source) {
				if(object.Equals(obj, item)) {
					if(!object.ReferenceEquals(last, null))
						return last;
				}
				last = obj;
			}
			return last;
		}
	}
}
