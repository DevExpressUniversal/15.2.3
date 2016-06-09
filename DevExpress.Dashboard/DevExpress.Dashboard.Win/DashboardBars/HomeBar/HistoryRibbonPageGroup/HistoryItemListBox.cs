#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class HistoryItemListBox : ListBoxControl {
		class HistoryItemListBoxState : ListBoxControlHandler.UnselectableState {
			public HistoryItemListBoxState(HistoryItemListBoxHandler handler) : base(handler) {
			}
			public void SetFocusedIndex(int index) {
				FocusedIndex = index;
			}
			public override void KeyDown(KeyEventArgs e) {
			}
		}
		class HistoryItemListBoxHandler : ListBoxControlHandler {
			public HistoryItemListBoxHandler(HistoryItemListBox listBox) : base(listBox) {
			}
			public void SetFocusedIndex(int index) {
				HistoryItemListBoxState state = ControlState as HistoryItemListBoxState;
				if (state != null)
					state.SetFocusedIndex(index);
			}
			protected override ListBoxControlState CreateState(HandlerState state) {
				return state == HandlerState.Unselectable ? new HistoryItemListBoxState(this) : base.CreateState(state);
			}
		}
		class HistoryItemSelectedIndexCollection : BaseListBoxControl.SelectedIndexCollection {
			public HistoryItemSelectedIndexCollection(HistoryItemListBox owner) : base(owner) {
			}
			public void SetSelection(int highIndex) {
				List<int> indices = new List<int>(highIndex + 1);
				for (int i = 0; i <= highIndex; i++)
					indices.Add(i);
				Set(indices);
			}
		}
		public void SetSelection(int highIndex) {
			((HistoryItemSelectedIndexCollection)SelectedIndices).SetSelection(highIndex);
			((HistoryItemListBoxHandler)Handler).SetFocusedIndex(highIndex);
		}
		protected override ListBoxControlHandler CreateHandler() {
			return new HistoryItemListBoxHandler(this);
		}
		protected override BaseListBoxControl.SelectedIndexCollection CreateSelectedIndexCollection() {
			return new HistoryItemSelectedIndexCollection(this);
		}
	}
}
