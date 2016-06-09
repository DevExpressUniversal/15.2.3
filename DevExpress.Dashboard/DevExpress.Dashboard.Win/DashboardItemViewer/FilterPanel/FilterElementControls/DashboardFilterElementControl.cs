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
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess.Native;
using DevExpress.XtraEditors;
using DevExpress.DashboardCommon.ViewerData;
namespace DevExpress.DashboardWin.Native {
	public abstract class DashboardFilterElementControl : XtraUserControl, IFilterElementControl {
		IFilterElementEditor editor;
		event EventHandler selectionChanged;
		readonly Locker locker = new Locker();
		event EventHandler IFilterElementControl.SelectionChanged {
			add { selectionChanged += value; }
			remove { selectionChanged -= value; }
		}
		int IFilterElementControl.EditorHeight { get { return EditorHeightInternal; } }
		public IFilterElementEditor Editor {
			get { return editor; }
			private set {
				if(editor != value) {
					RemoveControl();
					editor = value;
					AddControl();
				}
			}
		}
		protected virtual int EditorHeightInternal { get { return 0; } }
		protected DashboardFilterElementControl() {
			LookAndFeel.StyleChanged += OnStyleChanged;
		}
		void OnStyleChanged(object sender, EventArgs e) {
			if(editor != null)
				DashboardWinHelper.SetParentLookAndFeel(editor, LookAndFeel);
		}
		void IFilterElementControl.Configure(FilterElementDashboardItemViewModel viewModel) {
			IFilterElementEditor editor =  ConfigureInternal(viewModel);
			if(editor != null)
				Editor = editor;
		}
		IEnumerable<int> IFilterElementControl.GetSelection() {
			return editor.GetSelection();
		}
		void IFilterElementControl.SetSelection(IEnumerable<int> selection) {
			LockUpdatesOnAction(() => editor.SetSelection(selection));
		}
		void IFilterElementControl.SetData(object dataSource) {
			LockUpdatesOnAction(() => editor.DataSource = dataSource);
		}
		protected abstract IFilterElementEditor ConfigureInternal(FilterElementDashboardItemViewModel viewModel);
		void OnElementEditorSelectionChanged(object sender, EventArgs e) {
			if(!locker.IsLocked && selectionChanged != null)
				selectionChanged(sender, e);
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				RemoveControl();
			base.Dispose(disposing);
		}
		void AddControl() {
			Control control = (Control)editor;
			Controls.Clear();
			Controls.Add(control);
			control.Dock = DockStyle.Fill;
			DashboardWinHelper.SetParentLookAndFeel(editor, LookAndFeel);
			editor.ElementSelectionChanged += OnElementEditorSelectionChanged;
		}
		void RemoveControl() {
			if(editor != null) {
				editor.ElementSelectionChanged -= OnElementEditorSelectionChanged;
				Control control = (Control)editor;
				control.Dispose();
				Controls.Remove(control);
				editor = null;
			}
		}
		void LockUpdatesOnAction(Action action) {
			locker.Lock();
			try {
				action();
			}
			finally {
				locker.Unlock();
			}
		}
	}
}
