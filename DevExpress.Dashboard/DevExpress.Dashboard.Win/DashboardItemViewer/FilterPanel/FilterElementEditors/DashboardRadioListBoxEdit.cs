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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class DashboardRadioListBoxEdit : DashboardCheckedListBoxControlBase {
		public DashboardRadioListBoxEdit() {
			CheckOnClick = true;
			ItemChecking += OnItemChecking;
			SelectionMode = System.Windows.Forms.SelectionMode.None;
			ValueMember = FilterElementValuePropertyDescriptor.Member;
			DisplayMember = FilterElementDisplayTextPropertyDescriptor.Member;
			BorderStyle = BorderStyles.NoBorder;
		}
		protected override BaseControlPainter CreatePainter() {
			return new DashboardRadioListBoxControlPainter();
		}
		protected override void SetItemCheckStateCore(int index, CheckState value) {
			if(value == CheckState.Checked) {
				PerformAction(() => {
					UnCheckAll();
				});
			}
			base.SetItemCheckStateCore(index, value);
		}
		protected override void RaiseItemCheck(XtraEditors.Controls.ItemCheckEventArgs e) {
			base.RaiseItemCheck(e);
			if(!UpdatesLocker.IsLocked)
				RaiseElementSelectionChanged(e);
		}
		void OnItemChecking(object sender, ItemCheckingEventArgs e) {
			if(e.NewValue == CheckState.Unchecked)
				e.Cancel = true;
		}
		int FindItem(int value) {
			return FindItem(0, true, (e) => { e.IsFound = Object.Equals(e.ItemValue, value); });
		}
		protected override IEnumerable<int> GetSelectionInternal() {
			IList<int> selection = new List<int>();
			for(int i = 0; i < CheckedItems.Count; i++)
				selection.Add((int)CheckedItems[i]);
			return selection;
		}
		protected override void SetSelectionInternal(IEnumerable<int> selection) {
			if(selection.Count() > 0) {
				FindItem(selection.First());
				PerformAction(() => {
					SetItemChecked(FindItem(selection.First()), true);
				});
			}
		}
	}
	public class DashboardRadioListBoxControlPainter : PainterCheckedListBox {
		protected override void DrawItemCore(ControlGraphicsInfoArgs info, BaseListBoxViewInfo.ItemInfo itemInfo, ListBoxDrawItemEventArgs e) {
			itemInfo.State = DrawItemState.None;
			CheckedListBoxViewInfo.CheckedItemInfo checkedInfo = (CheckedListBoxViewInfo.CheckedItemInfo)itemInfo;
			checkedInfo.CheckArgs.CheckStyle = CheckStyles.Radio;
			base.DrawItemCore(info, itemInfo, e);
		}
	}
}
