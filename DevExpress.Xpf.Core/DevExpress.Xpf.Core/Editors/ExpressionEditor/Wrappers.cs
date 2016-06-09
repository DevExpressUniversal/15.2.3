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

using System.Windows.Controls;
using DevExpress.Data;
using DevExpress.Data.ExpressionEditor;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Utils;
using System;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Editors.ExpressionEditor {
	public class MemoEditWrapper : IMemoEdit {
		readonly TextEdit edit;
		public MemoEditWrapper(TextEdit edit) {
			this.edit = edit;
		}
		#region IMemoEdit Members
		int IMemoEdit.SelectionStart { get { return edit.SelectionStart; } set { edit.SelectionStart = value; } }
		int IMemoEdit.SelectionLength { get { return edit.SelectionLength; } set { edit.SelectionLength = value; } }
		string IMemoEdit.Text { get { return edit.Text; } set { edit.Text = value; } }
		string IMemoEdit.SelectedText { get { return edit.SelectedText; } }
		int IMemoEdit.GetLineLength(int lineIndex) {
			return edit.GetLineLength(lineIndex);
		}
		void IMemoEdit.Paste(string text) {
			edit.SelectedText = text;
		}
		void IMemoEdit.Focus() {
			edit.Dispatcher.BeginInvoke(new Action(() => edit.Focus()));
		}
		#endregion
	}
	static class SortHelper {
		class ReverseComparer : IComparer<object> {
			readonly IComparer<object> comparer;
			public ReverseComparer(IComparer<object> comparer) {
				this.comparer = comparer;
			}
			#region IComparer Members
			int IComparer<object>.Compare(object x, object y) {
				return comparer.Compare(y, x);
			}
			#endregion
		}
		public static object[] GetSortedItems(object[] items, ColumnSortOrder sortOrder) {
			if(sortOrder != ColumnSortOrder.None) {
				List<object> list = new List<object>(items);
				list.Sort(sortOrder == ColumnSortOrder.Ascending ? (IComparer<object>)Comparer<object>.Default : new ReverseComparer(Comparer<object>.Default));
				return list.ToArray();
			}
			return items;
		}
	}
	public class ListBoxControlWrappper : ISelector {
		readonly ListBox listBox;
		public ListBoxControlWrappper(ListBox listBox) {
			this.listBox = listBox;
		}
		#region ISelector Members
		void ISelector.SetItemsSource(object[] items, ColumnSortOrder sortOrder) {
			listBox.ItemsSource = SortHelper.GetSortedItems(items, sortOrder);
		}
		int ISelector.ItemCount { get { return listBox.Items.Count; } }
		object ISelector.SelectedItem { get { return listBox.SelectedItem; } }
		int ISelector.SelectedIndex { get { return listBox.SelectedIndex; } set { listBox.SelectedIndex = value; } }
		#endregion
	}
	public class ComboBoxEditWrappper : ISelector {
		readonly ComboBoxEdit comboBox;
		public ComboBoxEditWrappper(ComboBoxEdit comboBox) {
			this.comboBox = comboBox;
		}
		#region ISelector Members
		void ISelector.SetItemsSource(object[] items, ColumnSortOrder sortOrder) {
			comboBox.ItemsSource = SortHelper.GetSortedItems(items, sortOrder);
		}
		int ISelector.ItemCount { get { return comboBox.ItemsSource != null ? ((ICollection)comboBox.ItemsSource).Count : 0; } }
		object ISelector.SelectedItem { get { return comboBox.SelectedItem; } }
		int ISelector.SelectedIndex { get { return comboBox.SelectedIndex; } set { comboBox.SelectedIndex = value; } }
		#endregion
	}
}
