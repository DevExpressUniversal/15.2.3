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
namespace DevExpress.Xpf.Diagram  {
	public abstract class ExtendedSelectionCollectionEditor : CollectionEditorBase {
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
			DoWithSelectedItem(x => controller.Move(x, moveDown ? x + 1 : x - 1), moveDown, false);
		}
		protected override bool CanExecuteMoveUpCommand() {
			return SelectedItems != null && SelectedItems.Min(x => Items.IndexOf(x)) > 0;
		}
		protected override bool CanExecuteMoveDownCommand() {
			return SelectedItems != null && SelectedItems.Max(x => Items.IndexOf(x)) < Items.Count - 1;
		}
		protected override bool CanExecuteRemoveItemCommand() {
			return Items.Count > 0 && SelectedItems != null && SelectedItems.Any();
		}
	}
}
