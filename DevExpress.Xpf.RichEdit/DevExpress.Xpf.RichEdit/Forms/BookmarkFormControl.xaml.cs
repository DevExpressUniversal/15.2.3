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
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Xpf.Editors;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.RichEdit;
using System.Windows.Data;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class BookmarkFormControl : UserControl, IDialogContent {
		public static readonly DependencyProperty CanAddBookmarkProperty = DependencyProperty.Register("CanAddBookmark", typeof(bool), typeof(BookmarkFormControl), new PropertyMetadata(false));
		public static readonly DependencyProperty CanDeleteBookmarkProperty = DependencyProperty.Register("CanDeleteBookmark", typeof(bool), typeof(BookmarkFormControl), new PropertyMetadata(false));
		public static readonly DependencyProperty CanSelectBookmarkProperty = DependencyProperty.Register("CanSelectBookmark", typeof(bool), typeof(BookmarkFormControl), new PropertyMetadata(false));
		public static readonly DependencyProperty SelectedBookmarkIndexProperty = DependencyProperty.Register("SelectedBookmarkIndex", typeof(int), typeof(BookmarkFormControl), new PropertyMetadata(-1, OnSelectedBookmarksIndexChanged));
		public static readonly DependencyProperty BookmarkNameProperty = DependencyProperty.Register("BookmarkName", typeof(string), typeof(BookmarkFormControl), new PropertyMetadata(String.Empty, OnBookmarkNameChanged));
		static void OnSelectedBookmarksIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BookmarkFormControl)d).OnSelectedBookmarksIndexChanged((int)e.NewValue);
		}
		static void OnBookmarkNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BookmarkFormControl)d).OnBookmarkNameChanged((string)e.NewValue);
		}
		readonly BookmarkFormController controller;
		readonly ObservableCollection<Bookmark> bookmarks;
		PieceTable pieceTable;
		public BookmarkFormControl() {
			this.bookmarks = new ObservableCollection<Bookmark>();
			InitializeComponent();
			CreateBindings();
		}
		public BookmarkFormControl(BookmarkFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controller = CreateController(controllerParameters);
			this.bookmarks = new ObservableCollection<Bookmark>();
			InitializeComponent();
			Loaded += OnLoaded;
			CreateBindings();
			PopulateBookmarks(Controller.GetBookmarksSortedByName(false));
			SetActivePieceTable(controllerParameters.Control.InnerControl.DocumentModel.Selection.PieceTable);
			SelectedBookmarkIndex = Bookmarks.IndexOf(Controller.GetCurrentBookmark());
		}
		public ObservableCollection<Bookmark> Bookmarks { get { return bookmarks; } }
		public BookmarkFormController Controller { get { return controller; } }
		protected virtual BookmarkFormController CreateController(BookmarkFormControllerParameters controllerParameters) {
			return new BookmarkFormController(controllerParameters);
		}
		protected Bookmark SelectedBookmark {
			get {
				if (SelectedBookmarkIndex >= 0 && SelectedBookmarkIndex < Bookmarks.Count)
					return Bookmarks[SelectedBookmarkIndex];
				return null;
			}
		}
		public int SelectedBookmarkIndex {
			get { return (int)GetValue(SelectedBookmarkIndexProperty); }
			set { SetValue(SelectedBookmarkIndexProperty, value); }
		}
		public string BookmarkName {
			get { return (string)GetValue(BookmarkNameProperty); }
			set { SetValue(BookmarkNameProperty, value); }
		}
		public bool CanAddBookmark {
			get { return (bool)GetValue(CanAddBookmarkProperty); }
			private set { SetValue(CanAddBookmarkProperty, value); }
		}
		public bool CanDeleteBookmark {
			get { return (bool)GetValue(CanDeleteBookmarkProperty); }
			private set { SetValue(CanDeleteBookmarkProperty, value); }
		}
		public bool CanSelectBookmark {
			get { return (bool)GetValue(CanSelectBookmarkProperty); }
			private set { SetValue(CanSelectBookmarkProperty, value); }
		}
		public void SetActivePieceTable(PieceTable pieceTable) {
			this.pieceTable = pieceTable;
		}
		public void CreateBindings() {
			CreateBinding("BookmarkName", this.bookmarkName, TextEdit.EditValueProperty, BindingMode.TwoWay);
			CreateBinding("Bookmarks", this.bookmarkList, ListBox.ItemsSourceProperty, BindingMode.OneWay);
			CreateBinding("SelectedBookmarkIndex", this.bookmarkList, ListBox.SelectedIndexProperty, BindingMode.TwoWay);
		}
		Binding CreateBinding(string path, FrameworkElement target, DependencyProperty property, BindingMode mode) {
			Binding b = new Binding(path);
#if !SL
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
#endif
			b.Source = this;
			b.Mode = mode;
			target.SetBinding(property, b);
			return b;
		}
		void OnSelectedBookmarksIndexChanged(int newValue) {
			SelectedBookmarkIndex = newValue >= 0 && newValue < Bookmarks.Count ? newValue : -1;
			bool isBookmarkSelected = SelectedBookmark != null;
			BookmarkName = isBookmarkSelected ? SelectedBookmark.Name : BookmarkName;
			CanDeleteBookmark = isBookmarkSelected;
			CanSelectBookmark = isBookmarkSelected && Object.ReferenceEquals(SelectedBookmark.PieceTable, pieceTable);
		}
		void OnBookmarkNameChanged(string newValue) {
			CanAddBookmark = !String.IsNullOrEmpty(newValue) && Bookmark.IsNameValid(newValue);
			UpdateSelectedBookmarkByName(newValue);
		}
		Bookmark GetSelectedBookmark() {
			if (SelectedBookmarkIndex >= 0 && SelectedBookmarkIndex < Bookmarks.Count)
				return Bookmarks[SelectedBookmarkIndex];
			return null;
		}
		void UpdateSelectedBookmarkByName(string name) {
			if (SelectedBookmark == null || SelectedBookmark.Name != name) {
				Bookmark bookmark = GetBookmarkByName(name);
				if (bookmark != null)
					SelectedBookmarkIndex = Bookmarks.IndexOf(bookmark);
				else
					SelectedBookmarkIndex = -1;
			}
		}
		Bookmark GetBookmarkByName(string name) {
			IList<Bookmark> bookmarks = Controller.GetBookmarksSortedByName(false);
			int index = Algorithms.BinarySearch(bookmarks, new BookmarksComparer(name));
			if (index >= 0 && index < bookmarks.Count)
				return bookmarks[index];
			return null;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
#if SL
			DXDialog dialog = FloatingContainer.GetDialogOwner(this) as DXDialog;
			if (dialog != null)
				dialog.OkButton.Visibility = System.Windows.Visibility.Collapsed;
#endif
		}
		private void OnAddButtonClick(object sender, RoutedEventArgs e) {
			if (Controller == null)
				return;
			Controller.CreateBookmark(BookmarkName, delegate() { return true; });
			IDialogOwner owner = FloatingContainer.GetDialogOwner(this);
			if (owner != null)
				owner.CloseDialog(false);
		}
		private void OnDeleteButtonClick(object sender, RoutedEventArgs e) {
			if (Controller == null)
				return;
			Bookmark selectedItem = SelectedBookmark;
			Controller.DeleteBookmark(selectedItem);
			Bookmarks.Remove(selectedItem);
			BookmarkName = String.Empty;
		}
		private void OnGoToButtonClick(object sender, RoutedEventArgs e) {
			if (Controller == null)
				return;
			Controller.SelectBookmark(SelectedBookmark);
		}
		private void sortByName_Checked(object sender, RoutedEventArgs e) {
			if (Controller == null)
				return;
			PopulateBookmarks(Controller.GetBookmarksSortedByName(false));
		}
		private void sortByLocation_Checked(object sender, RoutedEventArgs e) {
			if (Controller == null)
				return;
			PopulateBookmarks(Controller.GetBookmarksSortedByLocation(false));
		}
		protected void PopulateBookmarks(IList<Bookmark> bookmarks) {
			Bookmark selectedBookmark = SelectedBookmark;
			Bookmarks.Clear();
			for (int i = 0; i < bookmarks.Count; i++)
				Bookmarks.Add(bookmarks[i]);
			SelectedBookmarkIndex = Bookmarks.IndexOf(selectedBookmark);
		}
		#region IDialogContent Members
		bool IDialogContent.CanCloseWithOKResult() {
			return true;
		}
		void IDialogContent.OnApply() {
		}
		void IDialogContent.OnOk() {
		}
		#endregion
	}
	public class BookmarksComparer : IComparable<Bookmark> {
		string name;
		public BookmarksComparer(string name) {
			this.name = name;
		}
		#region IComparable<Bookmark> Members
		public int CompareTo(Bookmark other) {
			return other.Name.CompareTo(name);
		}
		#endregion
	}
	public enum BookmarkFormAction {
		Add,
		Delete,
		GoTo,
		SortByName,
		SortByLocation
	}
}
