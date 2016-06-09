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
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using DevExpress.Internal;
using System.Globalization;
namespace DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard.Templates {
	internal class BindableRichTextBox : RichTextBox {
		#region static
		public static readonly DependencyProperty DocumentStreamProperty;
		public static readonly DependencyProperty DocumentFontSizeProperty;
		public static readonly DependencyProperty DocumentFontFamilyProperty;
		static BindableRichTextBox() {
			Type ownerType = typeof(BindableRichTextBox);
			DocumentStreamProperty = DependencyProperty.Register("DocumentStream", typeof(Stream), ownerType,
			new FrameworkPropertyMetadata((o, e) => ((BindableRichTextBox)o).OnDocumentChanged((Stream)e.NewValue)));
			DocumentFontSizeProperty = DependencyProperty.Register("DocumentFontSize", typeof(double), ownerType,
			new FrameworkPropertyMetadata((o, e) => ((BindableRichTextBox)o).OnFontSizeChanged()));
			DocumentFontFamilyProperty = DependencyProperty.Register("DocumentFontFamily", typeof(FontFamily), ownerType,
			new FrameworkPropertyMetadata((o, e) => ((BindableRichTextBox)o).OnFontFamilyChanged()));
		}
		#endregion
		public BindableRichTextBox() {
			this.AddHandler(Hyperlink.ClickEvent, new RoutedEventHandler(OnHyprelinkClick));
		}
		public double DocumentFontSize {
			get { return (double)GetValue(DocumentFontSizeProperty); }
			set { SetValue(DocumentFontSizeProperty, value); }
		}
		public FontFamily DocumentFontFamily {
			get { return (FontFamily)GetValue(DocumentFontFamilyProperty); }
			set { SetValue(DocumentFontFamilyProperty, FontFamily); }
		}
		public Stream DocumentStream {
			get { return (Stream)GetValue(DocumentStreamProperty); }
			set { SetValue(DocumentStreamProperty, value); }
		}
		protected override Size MeasureOverride(Size constraint) {
			Size resultSize = base.MeasureOverride(constraint);
			resultSize.Height = resultSize.Height > 30 ? resultSize.Height - 30 : resultSize.Height;
			return resultSize;
		}
		private void OnHyprelinkClick(object sender, RoutedEventArgs e) {
			Process.Start(((Hyperlink)e.Source).NavigateUri.ToString());
		}
		private void OnDocumentChanged(Stream newDocumentStream) {
			if(newDocumentStream == null) return;
			this.Selection.Load(newDocumentStream, DataFormats.Rtf);
			ApplyDocumentSettings();
		}
		private void ApplyDocumentSettings() {
			SetFontSize();
			SetFontFamily();
			SetForeground();
		}
		private void SetForeground() {
			if(Selection != null && Foreground != null) {
				try {
					this.Selection.ApplyPropertyValue(RichTextBox.ForegroundProperty, Foreground);
				} catch { }
			}
		}
		private void SetFontSize() {
			if(Selection != null && DocumentFontSize != 0) {
				try {
					this.Selection.ApplyPropertyValue(RichTextBox.FontSizeProperty, DocumentFontSize);
				} catch { }
			}
		}
		private void OnFontSizeChanged() {
			SetFontSize();
		}
		private void OnFontFamilyChanged() {
			SetFontFamily();
		}
		private void SetFontFamily() {
			if(Selection != null && DocumentFontFamily != null) {
				try {
					this.Selection.ApplyPropertyValue(RichTextBox.FontFamilyProperty, DocumentFontFamily);
				} catch { }
			}
		}
	}
	internal class IntegerTextBox : TextBox {
		public IntegerTextBox() : base() { }
		protected override void OnTextChanged(TextChangedEventArgs e) {
			if(this.Text == "")
				Text = "0";
			base.OnTextChanged(e);
		}
		protected override void OnPreviewTextInput(System.Windows.Input.TextCompositionEventArgs e) {
			if(!ValidateText(e.Text)) {
				e.Handled = true;
				return;
			}
			base.OnPreviewTextInput(e);
		}
		protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e) {
			if(IsKeyNumeric(e.Key)) {
				e.Handled = true;
				return;
			}
			base.OnPreviewKeyDown(e);
		}
		private bool IsKeyNumeric(System.Windows.Input.Key key) {
			return key == System.Windows.Input.Key.Space;
		}
		private bool ValidateText(string inputText) {
			try {
				int.Parse(inputText);
			} catch {
				return false;
			}
			return true;
		}
	}
	internal class AutoSearchTextBox : TextBox {
		string searchText = "";
		bool isCrtlPressed = false;
		bool isBackSpacePressed = false;
		bool isInternalSet = false;
		#region static
		public static readonly DependencyProperty SearchResultsProperty;
		static AutoSearchTextBox() {
			SearchResultsProperty =
				DependencyProperty.Register("SearchResults", typeof(ObservableCollection<string>), typeof(AutoSearchTextBox), new FrameworkPropertyMetadata());
		}
		#endregion
		public ObservableCollection<string> SearchResults {
			get { return (ObservableCollection<string>)GetValue(SearchResultsProperty); }
			set { SetValue(SearchResultsProperty, value); }
		}
		public IEnumerable<string> SearchCollection { get; set; }
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			switch(e.Key) {
				case Key.LeftCtrl & Key.RightCtrl:
					isCrtlPressed = true;
					e.Handled = true;
					break;
				case Key.Back:
					isBackSpacePressed = true;
					break;
				case Key.Space:
					if(isCrtlPressed && !string.IsNullOrEmpty(searchText)) {
						TryFindResult();
						e.Handled = true;
					}
					break;
				default:
					base.OnPreviewKeyDown(e);
					break;
			}
		}
		protected override void OnPreviewKeyUp(KeyEventArgs e) {
			switch(e.Key) {
				case Key.LeftCtrl & Key.RightCtrl:
					isCrtlPressed = false;
					e.Handled = true;
					break;
				case Key.Back:
					isBackSpacePressed = false;
					base.OnPreviewKeyDown(e);
					break;
				default:
					base.OnPreviewKeyUp(e);
					break;
			}
		}
		protected override void OnTextChanged(TextChangedEventArgs e) {
			if(!isInternalSet && SearchCollection != null) {
				if(!string.IsNullOrEmpty(SelectedText))
					searchText = this.Text.Replace(SelectedText, "");
				else
					searchText = Text;
				TryFindResult();
			}
			base.OnTextChanged(e);
		}
		private void TryFindResult() {
			SearchResults = new ObservableCollection<string>();
			if(SearchCollection == null)
				return;
			try {
				string[] searchTypes = SearchCollection.Where(t => t.ToUpper().StartsWith(searchText.ToUpper())).Select(t => t).ToArray();
				SearchResults = new ObservableCollection<string>(SearchResults.Concat(searchTypes).ToArray().OrderBy(s => s));
			} catch { }
			if(SearchResults.Count > 0) {
				SetSearchedText(SearchResults[0]);
			}
		}
		private void SetSearchedText(string text) {
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
	internal class AutoCompletedComboBox : ComboBox {
		AutoSearchTextBox searchTextBox;
		bool isSelectedIndexSettedInternal = false;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			searchTextBox = (AutoSearchTextBox)this.GetTemplateChild("PART_TextBox");
			searchTextBox.SearchCollection = (IEnumerable<string>)ItemsSource;
			searchTextBox.PreviewMouseDown += new MouseButtonEventHandler(searchTextBox_PreviewMouseDown);
			Binding bind = new Binding("SearchResults");
			bind.Source = searchTextBox;
			bind.Mode = BindingMode.OneWay;
			this.SetBinding(AutoCompletedComboBox.ItemsSourceProperty, bind);
			searchTextBox.TextChanged += new TextChangedEventHandler(searchTextBox_TextChanged);
			this.IsSynchronizedWithCurrentItem = false;
		}
		void searchTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			IsDropDownOpen = false;
		}
		protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			base.OnSelectionChanged(e);
			if(!string.IsNullOrEmpty((string)SelectedItem) && !isSelectedIndexSettedInternal && searchTextBox != null)
				searchTextBox.Text = SelectedItem.ToString();
			else
				isSelectedIndexSettedInternal = false;
		}
		private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e) {
			if(ItemsSource != null && ItemsSource.GetEnumerator().MoveNext() && searchTextBox.SearchResults.Contains(searchTextBox.Text)) {
				int index = searchTextBox.SearchResults.IndexOf(searchTextBox.Text);
				if(index != this.SelectedIndex) {
					isSelectedIndexSettedInternal = true;
					this.SelectedIndex = index;
				}
			}
		}
	}
}
