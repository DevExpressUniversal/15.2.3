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
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Utils;
using DevExpress.XtraRichEdit;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.RichEdit.UI {
	#region ReviewersBarListItem
	public class ReviewersBarListItem : BarListItem {
		static ReviewersBarListItem() {
			BarItemLinkCreator.Default.RegisterObject(typeof(ReviewersBarListItem), typeof(ReviewersBarListItemLink), delegate(object arg) { return new ReviewersBarListItemLink(); });
		}
		public static readonly DependencyProperty RichEditControlProperty = DependencyPropertyManager.Register("RichEditControl", typeof(RichEditControl), typeof(ReviewersBarListItem), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRichEditControlChanged)));
		protected static void OnRichEditControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ReviewersBarListItem instance = d as ReviewersBarListItem;
			if (instance != null)
				instance.OnRichEditControlChanged((RichEditControl)e.OldValue, (RichEditControl)e.NewValue);
		}
		public RichEditControl RichEditControl {
			get { return (RichEditControl)GetValue(RichEditControlProperty); }
			set { SetValue(RichEditControlProperty, value); }
		}
		protected internal virtual void OnRichEditControlChanged(RichEditControl oldValue, RichEditControl newValue) {
			UpdateItems();
		}
		public override BarItemLink CreateLink(bool isPrivate) {
			return new ReviewersBarListItemLink();
		}
		protected override void UpdateItems() {
			try {
				Items.Clear();
				if (RichEditControl != null)
					UpdateItemsCore();
			}
			finally {
			}
		}
		protected internal virtual void UpdateItemsCore() {
			List<string> authors = RichEditControl.DocumentModel.GetAuthors();
			BarCheckItem item = new BarCheckItem();
			item.Content = "All Authors";
			item.CommandParameter = RichEditControl;
			if (RichEditControl.ActiveView.DocumentModel.CommentOptions.ShowAllAuthors)
				item.IsChecked = true;
			else
				item.IsChecked = false;
			Items.Add(item);
			foreach (string author in authors) {
				AppendItem(author);
			}
		}
		protected internal virtual void AppendItem(string author) {
			BarCheckItem item = new BarCheckItem();
			item.Content = author;
			item.CommandParameter = RichEditControl;
			if (RichEditControl.ActiveView.DocumentModel.CommentOptions.ShowAllAuthors)
				item.IsChecked = true;
			else
				item.IsChecked = CheckedAuthors(RichEditControl, author);
			Items.Add(item);
		}
		bool CheckedAuthors(RichEditControl control, string author) {
			ObservableCollection<string> visibleAuthors = control.Options.Comments.VisibleAuthors;
			return (visibleAuthors.Contains(author));
		}
	}
	#endregion
	#region ReviewersBarListItemLink
	public class ReviewersBarListItemLink : BarListItemLink { }
	#endregion
	#region ReviewersBarSubItem
	public class ReviewersBarSubItem : BarSubItem {
		public static readonly DependencyProperty RichEditControlProperty = DependencyPropertyManager.Register("RichEditControl", typeof(RichEditControl), typeof(ReviewersBarSubItem));
		public RichEditControl RichEditControl {
			get { return (RichEditControl)GetValue(RichEditControlProperty); }
			set { SetValue(RichEditControlProperty, value); }
		}
		public ReviewersBarSubItem() {
			this.GetItemData += OnGetItemData;
		}
		void OnGetItemData(object sender, EventArgs e) {
			UpdateItems();
		}
		protected internal void UpdateItems() {
			if (RichEditControl != null)
				UpdateItemsCore();
		}
		protected internal virtual void UpdateItemsCore() {
			List<string> authors = RichEditControl.DocumentModel.GetAuthors();
			BarCheckItem item = new BarCheckItem();
			item.Content = "All Authors";
			item.CommandParameter = RichEditControl;
			if (RichEditControl.ActiveView.DocumentModel.CommentOptions.ShowAllAuthors)
				item.IsChecked = true;
			else
				item.IsChecked = false;
			ItemLinks.Add(item);
			item.ItemClick += item_ItemClick;
			foreach (string author in authors) {
				AppendItem(author);
			}
		}
		void item_ItemClick(object sender, ItemClickEventArgs e) {
			BarCheckItem item = (BarCheckItem)sender;
			bool itemIsChecked;
			if (item.IsChecked == true) {
				itemIsChecked = true;
			}
			else {
				itemIsChecked = false;
			}
			if ((string)item.Content == "All Authors") {
				RichEditControl.Options.Comments.ShowAllAuthors = itemIsChecked;
				if (itemIsChecked)
					RichEditControl.Options.Comments.Visibility = RichEditCommentVisibility.Visible;
				else
					RichEditControl.Options.Comments.Visibility = RichEditCommentVisibility.Hidden;
				List<string> authors = RichEditControl.DocumentModel.GetAuthors();
				if (!RichEditControl.ActiveView.DocumentModel.CommentOptions.ShowAllAuthors) {
					ObservableCollection<string> visibleAuthors = RichEditControl.Options.Comments.VisibleAuthors;
					visibleAuthors.Clear();
					return;
				}
				if (authors.Count > 0) {
					ObservableCollection<string> visibleAuthors = RichEditControl.Options.Comments.VisibleAuthors;
					foreach (string author in authors) {
						if (!visibleAuthors.Contains(author))
							visibleAuthors.Add(author);
					}
				}
			}
			else {
				ObservableCollection<string> visibleAuthors = RichEditControl.Options.Comments.VisibleAuthors;
				if (item.IsChecked == true) {
					RichEditControl.Options.Comments.Visibility = RichEditCommentVisibility.Visible;
					if (!visibleAuthors.Contains(item.Content))
						visibleAuthors.Add((string)item.Content);
				}
				else {
					visibleAuthors.Remove((string)item.Content);
					RichEditControl.Options.Comments.Visibility = RichEditCommentVisibility.Hidden;
				}
			}
		}
		protected internal virtual void AppendItem(string author) {
			BarCheckItem item = new BarCheckItem();
			item.Content = author;
			item.CommandParameter = RichEditControl;
			if (RichEditControl.ActiveView.DocumentModel.CommentOptions.ShowAllAuthors)
				item.IsChecked = true;
			else
				item.IsChecked = CheckedAuthors(author);
			ItemLinks.Add(item);
			item.ItemClick += item_ItemClick;
		}
		bool CheckedAuthors(string author) {
			ObservableCollection<string> visibleAuthors = RichEditControl.Options.Comments.VisibleAuthors;
			return (visibleAuthors.Contains(author));
		}
	}
	#endregion
}
