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
using System.Windows;
using DevExpress.Mvvm.UI.Native;
using System.Collections;
using DevExpress.Diagram.Core;
namespace DevExpress.Xpf.Diagram {
	public abstract class SingleSelectionCollectionEditor : CollectionEditorBase {
		public static readonly DependencyProperty SelectedItemProperty;
		static SingleSelectionCollectionEditor() {
			DependencyPropertyRegistrator<SingleSelectionCollectionEditor>.New()
				.Register(d => d.SelectedItem, out SelectedItemProperty, null, d => d.OnSelectedItemChanged())
				.OverrideDefaultStyleKey()
			;
		}
		public object SelectedItem {
			get { return (object)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		protected virtual void OnSelectedItemChanged() { }
		protected override void SetSelected(object item) {
			SelectedItem = item;
		}
		protected override bool ItemIsSelected(object item) {
			return SelectedItem == item;
		}
		protected override bool CanExecuteMoveUpCommand() {
			var collectionItemsList = GetCollectionEditorItems();
			return SelectedItem != null && collectionItemsList.Any() && SelectedItem != collectionItemsList[0];
		}
		protected override bool CanExecuteMoveDownCommand() {
			var collectionItemsList = GetCollectionEditorItems();
			return SelectedItem != null && collectionItemsList.IndexOf(SelectedItem) < collectionItemsList.Count - 1;
		}
		protected override bool CanExecuteRemoveItemCommand() {
			return SelectedItem != null && GetCollectionEditorItems().Any();
		}
		protected override void MoveItems(bool moveDown) {
			DoWithSelectedItem(x => controller.Move(x, GetIndex(x, moveDown)), false, true);
		}
		int GetIndex(int firstIndex, bool moveDown) {
			int secondIndex = firstIndex;
			if(moveDown) {
				for(int i = firstIndex; i < Items.Count - 1; ++i) {
					if(IsEditorItem((IMultiModel)Items[++secondIndex]))
						return secondIndex;
				}
			} else {
				for(int i = firstIndex; i > 0; --i) {
					if(IsEditorItem((IMultiModel)Items[--secondIndex]))
						return secondIndex;
				}
			}
			throw new InvalidOperationException();
		}
	}
}
