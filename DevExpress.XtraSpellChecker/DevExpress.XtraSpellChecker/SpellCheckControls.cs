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
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraSpellChecker.Parser;
using System.Windows.Forms;
using System.Text;
using DevExpress.Utils.Menu;
namespace DevExpress.XtraSpellChecker.Controls {
	[DXToolboxItem(false)]
	public class CustomSpellCheckMemoEdit : MemoEdit {
		static readonly object onSelectionStartChanged = new object();
		bool allowSelectAll = false;
		int selectionStart = -1;
		public bool AllowSelectAll {
			get { return allowSelectAll; }
			set { allowSelectAll = value; }
		}
		public override void SelectAll() {
			if(AllowSelectAll)
				base.SelectAll();
		}
		public override void DeselectAll() {
			if(AllowSelectAll)
				base.DeselectAll();
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			if(e.KeyChar == '\r')
				e.Handled = true;
			base.OnKeyPress(e);
		}
		protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e) {
			base.OnKeyUp(e);
			if(IsInputKey(e.KeyData))
				if(selectionStart != SelectionStart && !e.Handled)
					OnSelectionStartChanged(SelectionChangeReason.Keyboard, e.KeyCode);
		}
		protected virtual void OnSelectionStartChanged(SelectionChangeReason reason, Keys key) {
			selectionStart = SelectionStart;
			RaiseSelectionStartChanged(new SelectionStartChangedEventArgs(new IntPosition(SelectionStart), reason, key));
		}
		public event SelectionStartChangedEventHandler SelectionStartChanged {
			add { Events.AddHandler(onSelectionStartChanged, value); }
			remove { Events.RemoveHandler(onSelectionStartChanged, value); }
		}
		protected internal virtual void RaiseSelectionStartChanged(SelectionStartChangedEventArgs e) {
			SelectionStartChangedEventHandler handler = this.Events[onSelectionStartChanged] as SelectionStartChangedEventHandler;
			if(handler != null) handler(this, e);
		}
		protected override DevExpress.Utils.Menu.DXPopupMenu CreateMenu() {
			return new DXPopupMenu();
		}
	}
}
