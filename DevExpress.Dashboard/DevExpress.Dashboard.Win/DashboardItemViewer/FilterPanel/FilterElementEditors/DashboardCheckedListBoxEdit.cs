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

using DevExpress.DashboardCommon.Viewer;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class DashboardCheckedListBoxEdit : DashboardCheckedListBoxControlBase {
		public DashboardCheckedListBoxEdit() {
			CheckOnClick = true;
			ValueMember = FilterElementValuePropertyDescriptor.Member;
			DisplayMember = FilterElementDisplayTextPropertyDescriptor.Member;
			BorderStyle = BorderStyles.NoBorder;
		}
		protected override void RaiseItemCheck(DevExpress.XtraEditors.Controls.ItemCheckEventArgs e) {
			base.RaiseItemCheck(e);
			if(!UpdatesLocker.IsLocked) {
				if(!IsShowAllItem(e.Index))
					SetShowAllItemState();
				RaiseElementSelectionChanged(e);
			}
		}		
		protected override IEnumerable<int> GetSelectionInternal() {
			IList<int> selection = new List<int>();
			for(int i = 0; i < CheckedItems.Count; i++)
				if((int)CheckedItems[i] != -1)
					selection.Add((int)CheckedItems[i]);
			return selection;
		}
		protected override void SetSelectionInternal(IEnumerable<int> selection) {
			PerformAction(() => {
				UnCheckAll();
				HashSet<object> selectionHash = new HashSet<object>(selection.Cast<object>());
				for(int i = 0; i < ItemCount; i++) {
					if(selectionHash.Contains(GetItemValue(i))) 
						SetItemChecked(i, true);
				}
				SetShowAllItemState();
			});
		}
		#region ShowAll client emulation
		protected override void RaiseItemChecking(ItemCheckingEventArgs e) {
			if(IsShowAllItem(e.Index) && !UpdatesLocker.IsLocked) {
				PerformAction(() => {
					if(e.NewValue == CheckState.Checked)
						CheckAll();
					else
						UnCheckAll();
				});
			}
			base.RaiseItemChecking(e);
		}
		void SetShowAllItemState() {
			PerformAction(() => {
				int showAllIndex = Convert.ToInt32(GetItemValue(-1));
				int count = CheckedIndices.Count;
				if(GetItemChecked(showAllIndex))
					count--;
				CheckState checkState;
				if(count == 0)
					checkState = CheckState.Unchecked;
				else if(count == ItemCount - 1)
					checkState = CheckState.Checked;
				else
					checkState = CheckState.Indeterminate;
				SetItemCheckState(showAllIndex, checkState);
			});
		}
		public override void SetItemCheckState(int index, CheckState value) {
			OnSetItemCheckState(new ItemCheckingEventArgs(index, value, value == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked));
		}
		bool IsShowAllItem(int index) {
			return (int)GetItemValue(index) == -1;
		}
		#endregion
	}
}
