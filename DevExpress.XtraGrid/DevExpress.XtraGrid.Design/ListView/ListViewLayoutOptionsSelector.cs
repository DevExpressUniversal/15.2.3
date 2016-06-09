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
using System.Windows.Forms;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors.Designer.Utils;
namespace DevExpress.XtraGrid.Design.ListView {
	[ToolboxItem(false)]
	public class ListViewLayoutOptionsSelector : UserControl {
		EditingGridInfo gridInfo;
		public ListViewLayoutOptionsSelector() {
			this.gridInfo = null;
			InitializeComponent();
			SubscribeEvents();
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			InitPropertyGrid(pgOptions);
		}
		protected virtual void InitPropertyGrid(PropertyGrid propertyGrid) {
			propertyGrid.ExpandAllGridItems();
			if(splitterPos.HasValue)
				PGOptions.MoveSplitterTo((int)splitterPos);
		}
		#region Designer Generated
		DXPropertyGridEx pgOptions;
		private void InitializeComponent() {
			this.pgOptions = new DevExpress.XtraEditors.Designer.Utils.DXPropertyGridEx();
			this.SuspendLayout();
			this.pgOptions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgOptions.DrawFlat = false;
			this.pgOptions.Location = new System.Drawing.Point(0, 0);
			this.pgOptions.Name = "pgOptions";
			this.pgOptions.Size = new System.Drawing.Size(380, 362);
			this.pgOptions.TabIndex = 0;
			this.Controls.Add(this.pgOptions);
			this.Name = "ListViewLayoutOptionsSelector";
			this.Size = new System.Drawing.Size(380, 362);
			this.ResumeLayout(false);
		}
		#endregion
		public void AssignGridInfo(EditingGridInfo gridInfo) {
			this.gridInfo = gridInfo;
		}
		public void SelectObject(object obj) {
			PGOptions.SelectedObject = obj;
		}
		int? splitterPos = null;
		public void MovePropertyGridSplitterTo(int xpos) {
			splitterPos = xpos;
		}
		protected virtual void SubscribeEvents() {
			PGOptions.PropertyValueChanged += OnPropertyValueChanged;
		}
		protected virtual void UnsubscribeEvents() {
			PGOptions.PropertyValueChanged -= OnPropertyValueChanged;
		}
		protected virtual void OnPropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
			SelectedObjectChangedEventArgs ee = new SelectedObjectChangedEventArgs();
			RaiseSelectedObjectChanged(ee);
		}
		public event SelectedObjectChangedHandler SelectedObjectChanged;
		protected virtual void RaiseSelectedObjectChanged(SelectedObjectChangedEventArgs e) {
			if(SelectedObjectChanged != null)
				SelectedObjectChanged(this, e);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(PGOptions != null) {
					SubscribeEvents();
					PGOptions.Dispose();
					this.pgOptions = null;
				}
			}
			base.Dispose(disposing);
		}
		protected DXPropertyGridEx PGOptions { get { return pgOptions; } }
	}
	public delegate void SelectedObjectChangedHandler(object sender, SelectedObjectChangedEventArgs e);
	public class SelectedObjectChangedEventArgs : EventArgs {
		public SelectedObjectChangedEventArgs() {
		}
	}
}
