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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class HistoryPopupControl : XtraUserControl {
		const int visibleItemsCount = 12;
		const int minimalWidth = 200;
		string actionText;
		bool commit;
		int operationIndex;
		public bool Commit { get { return commit; } }
		public int OperationIndex { get { return operationIndex; } }
		public HistoryPopupControl() {
			InitializeComponent();
		}
		public Size OnPopup(HistoryBarItem barItem, bool redoOperations) {
			commit = false;
			actionText = DashboardWinLocalizer.GetString(redoOperations ? DashboardWinStringId.RedoText : DashboardWinStringId.UndoText);
			ListBoxItemCollection items = listBox.Items;
			items.BeginUpdate();
			try {
				items.Clear();
				foreach (IHistoryItem item in redoOperations ? barItem.Control.History.RedoItems : barItem.Control.History.UndoItems)
					items.Add(item.Caption);
			}
			finally {
				items.EndUpdate();
			}
			listBox.TopIndex = 0;
			int itemHeight = listBox.GetItemRectangle(0).Height;
			int width = listBox.CalcBestSize().Width;
			int itemsCount = items.Count;
			if (itemsCount > visibleItemsCount) {
				width +=  (listBox.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseListBoxViewInfo).ScrollInfo.VScrollWidth;
				itemsCount = visibleItemsCount;
			}
			UpdateSelection(0);
			return new Size(Math.Max(width, minimalWidth), itemHeight * itemsCount + panel.Height);
		}
		void UpdateOperationIndex() {
			BaseListBoxControl.SelectedIndexCollection selectedIndices = listBox.SelectedIndices;
			operationIndex = selectedIndices[selectedIndices.Count - 1];
		}
		void UpdateSelection(int index) {
			if (index >= 0 && index < listBox.Items.Count) {
				listBox.SetSelection(index);
				label.Text = String.Format(DashboardWinLocalizer.GetString(index == 0 ? DashboardWinStringId.UndoRedoSingleAction : DashboardWinStringId.UndoRedoActionsCount),
					actionText, index + 1);
			}
		}
		void CommitPopup() {
			PopupControlContainer parentContainer = Parent as PopupControlContainer;
			if (parentContainer != null) {
				commit = true;
				parentContainer.HidePopup();
			}
		}
		void listBox_KeyDown(object sender, KeyEventArgs e) {
			UpdateOperationIndex();
			switch (e.KeyCode) {
				case Keys.Down:
					UpdateSelection(operationIndex + 1);
					break;
				case Keys.Up:
					UpdateSelection(operationIndex - 1);
					break;
				case Keys.Enter:
					CommitPopup();
					break;
			}
		}
		void listBox_MouseMove(object sender, MouseEventArgs e) {
			UpdateSelection(listBox.IndexFromPoint(new Point(e.X, e.Y)));
		}
		void listBox_MouseClick(object sender, MouseEventArgs e) {
			UpdateOperationIndex();
			CommitPopup();
		}
	}
}
