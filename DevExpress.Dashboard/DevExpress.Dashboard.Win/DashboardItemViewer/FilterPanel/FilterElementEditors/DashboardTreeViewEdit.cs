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
using DevExpress.DataAccess.Native;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class DashboardTreeViewEdit : TreeList, ITreeViewEditor {
		readonly Locker updateLocker = new Locker();
		protected Locker UpdateLocker { get { return updateLocker; } }
		public event EventHandler ElementSelectionChanged;
		int IFilterElementEditor.Height { get { return 0; } }
		public bool AutoExpandNodes { get; set; }
		protected void RaiseElementSelectionChanged(EventArgs e) { 
			if(ElementSelectionChanged != null)
				ElementSelectionChanged(this, e);
		}
		protected void PerformAction(Action action) {
			updateLocker.Lock();
			BeginUpdate();
			try {
				action();
			} finally {
				updateLocker.Unlock();
				EndUpdate();
			}
		}
		protected virtual void SetSelectionInternal(IEnumerable<int> selection) {
		}
		protected virtual IEnumerable<int> GetSelectionInternal() {
			return new int[0];
		}
		object IFilterElementEditor.DataSource {
			get { return DataSource; }
			set {
				DataSource = value;
				ForceInitialize();
				if(AutoExpandNodes)
					ExpandAll();
			}
		}
		int IFilterElementEditor.GetIndex(Point location) {
			TreeListNode node = CalcHitInfo(location).Node;
			if(node != null)
				return (int)node.GetValue(TreeViewUniqueIDPropertyDescriptor.Member);
			return -1;
		}
		IEnumerable<int> IFilterElementEditor.GetSelection() {
			return GetSelectionInternal();
		}
		void IFilterElementEditor.SetSelection(IEnumerable<int> selection) {
			SetSelectionInternal(selection);
		}
	}
}
