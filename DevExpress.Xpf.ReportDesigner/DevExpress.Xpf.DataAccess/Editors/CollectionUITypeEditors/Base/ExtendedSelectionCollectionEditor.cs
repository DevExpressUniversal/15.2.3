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
using DevExpress.Mvvm.UI.Native;
using System.Windows;
using DevExpress.Xpf.DataAccess.Editors.CollectionUITypeEditors;
namespace DevExpress.Xpf.DataAccess.Editors.CollectionUITypeEditors {
	public abstract class ExtendedSelectionCollectionEditor : CollectionUITypeEditorBase {
		public static readonly DependencyProperty SelectedItemsProperty;
		static ExtendedSelectionCollectionEditor() {
			DependencyPropertyRegistrator<ExtendedSelectionCollectionEditor>.New()
				.Register(d => d.SelectedItems, out SelectedItemsProperty, null)
				.OverrideDefaultStyleKey()
			;
		}
		public List<object> SelectedItems {
			get { return (List<object>)GetValue(SelectedItemsProperty); }
			set { SetValue(SelectedItemsProperty, value); }
		}
		protected override void SetSelected(object item) {
			SelectedItems = new List<object>() { item };
		}
		protected override bool ItemIsSelected(object item) {
			return SelectedItems != null && SelectedItems.Contains(item);
		}
		protected override void MoveItems(bool moveDown) {
			var items = Items.Select(x => x.Item);
			var orderedSelectedItems = (moveDown ? items.Reverse() : items).Intersect(SelectedItems).ToArray();
			var itemsList = items.ToList();
			foreach(var item in orderedSelectedItems) {
				var index = itemsList.IndexOf(item);
				Items.Swap(index, moveDown ? index + 1 : index - 1);
			}
		}
		protected override bool CanExecuteMoveUpCommand() {
			var items = GetItemsList();
			return items.Any() && SelectedItems.Min(x => items.IndexOf(x)) > 0;
		}
		protected override bool CanExecuteMoveDownCommand() {
			var items = GetItemsList();
			return items.Any() && SelectedItems.Max(x => items.IndexOf(x)) < items.Count - 1; ;
		}
		protected override bool CanExecuteRemoveItemCommand() {
			return SelectedItems != null && SelectedItems.Any();
		}
	}
}
