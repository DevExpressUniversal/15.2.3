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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraGrid.EditForm.Layout;
using DevExpress.XtraGrid.EditForm;
using DevExpress.XtraGrid.Views.Grid;
using System.ComponentModel.Design;
namespace DevExpress.XtraGrid.Frames {
	[ToolboxItem(false)]
	public partial class EditFormDesigner : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		public EditFormDesigner() : base() {
			InitializeComponent();
		}
		bool initialized = false;
		protected GridView EditingView { get { return EditingObject as GridView; } }
		protected void InitBuilder() {
			if(initialized) return;
			if(EditingView == null) return;
			this.initialized = true;
			this.lb = new LayoutBuilder(EditingView);
			this.owner = lb.GenerateOwner();
			this.owner.ElementsLookAndFeel.Assign(LookAndFeel);
			pgMain.SelectedObject = owner;
			pgMain.PropertyValueChanged += (s, e) => { OnPropertyChanged(); };
			Generate();
		}
		void OnPropertyChanged() {
			Modified = true;
			AllowCancel = true;
			Generate();
		}
		bool modified = false, allowCancel = false;
		[Browsable(false)]
		protected bool AllowCancel {
			get { return allowCancel;}
			set {
				if(AllowCancel == value) return;
				allowCancel = value;
				btCancel.Enabled = AllowCancel;
			}
		}
		[Browsable(false)]
		protected bool Modified {
			get { return modified; }
			set {
				if(Modified == value) return;
				modified = value;
				btApply.Enabled = Modified;
			}
		}
		public override void InitComponent() {
			base.InitComponent();
			InitBuilder();
		}
		protected bool IsBuilderInitialized { get { return initialized; } }
		LayoutBuilder lb;
		EditFormOwner owner;
		protected virtual void Generate() {
			LayoutTableControlGenerator lg = new LayoutTableControlGenerator(owner);
			var panel = lg.Generate(owner);
			panel.Dock = DockStyle.Fill;
			formPanel.Controls.Clear();
			formPanel.Controls.Add(panel);
		}
		private void btCancel_Click(object sender, EventArgs e) {
			AllowCancel = Modified = false;
			this.owner = lb.GenerateOwner();
			pgMain.SelectedObject = owner;
			Generate();
		}
		private void btApply_Click(object sender, EventArgs e) {
			ApplyChanges();
		}
		void ApplyChanges() {
			if(IsBuilderInitialized && EditingView != null) {
				lb.AssignToView(owner, EditingView);
				Modified = AllowCancel = false;
				FireChanged();
			}
		}
		private void FireChanged() {
			if(EditingView == null) return;
			IComponentChangeService srv = EditingView.GridControl.InternalGetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) srv.OnComponentChanged(EditingView, null, null, null);
		}
		protected override string DescriptionText { get { return DevExpress.XtraGrid.Design.Properties.Resources.EditFormDescription; } }
		protected override void Dispose(bool disposing) {
			if(Modified) ApplyChanges();
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
