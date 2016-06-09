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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils.Design;
namespace DevExpress.XtraGrid.Frames {
	[ToolboxItem(false)]
	public class SchemeDesigner : DevExpress.XtraEditors.Frames.SchemeDesignerBase {
		private XtraEditors.SplitterControl splitterControl2;
		private XtraEditors.GroupControl gcPaintStyle;
		private XtraEditors.ListBoxControl listBoxControl1;
		private System.ComponentModel.Container components = null;
		public SchemeDesigner() {
			InitializeComponent();
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SchemeDesigner));
			this.gcPaintStyle = new DevExpress.XtraEditors.GroupControl();
			this.listBoxControl1 = new DevExpress.XtraEditors.ListBoxControl();
			this.splitterControl2 = new DevExpress.XtraEditors.SplitterControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlControls)).BeginInit();
			this.pnlControls.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcFormats)).BeginInit();
			this.gcFormats.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lsStyles)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceNew.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pcFormats)).BeginInit();
			this.pcFormats.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlCheckBox)).BeginInit();
			this.pnlCheckBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gcPaintStyle)).BeginInit();
			this.gcPaintStyle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.listBoxControl1)).BeginInit();
			this.SuspendLayout();
			this.pnlControl.Controls.Add(this.splitterControl2);
			this.pnlControl.Controls.Add(this.gcPaintStyle);
			resources.ApplyResources(this.pnlControl, "pnlControl");
			this.pnlControl.Controls.SetChildIndex(this.gcPaintStyle, 0);
			this.pnlControl.Controls.SetChildIndex(this.splitterControl2, 0);
			this.pnlControl.Controls.SetChildIndex(this.pcFormats, 0);
			resources.ApplyResources(this.pnlControls, "pnlControls");
			resources.ApplyResources(this.btApply, "btApply");
			resources.ApplyResources(this.gcFormats, "gcFormats");
			resources.ApplyResources(this.lsStyles, "lsStyles");
			resources.ApplyResources(this.ceNew, "ceNew");
			resources.ApplyResources(this.pcFormats, "pcFormats");
			resources.ApplyResources(this.pnlCheckBox, "pnlCheckBox");
			resources.ApplyResources(this.lbCaption, "lbCaption");
			resources.ApplyResources(this.pnlMain, "pnlMain");
			resources.ApplyResources(this.horzSplitter, "horzSplitter");
			this.gcPaintStyle.Controls.Add(this.listBoxControl1);
			resources.ApplyResources(this.gcPaintStyle, "gcPaintStyle");
			this.gcPaintStyle.Name = "gcPaintStyle";
			this.listBoxControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.listBoxControl1, "listBoxControl1");
			this.listBoxControl1.Name = "listBoxControl1";
			this.listBoxControl1.SelectedIndexChanged += new System.EventHandler(this.OnPaintStyleSelectedIndexChanged);
			resources.ApplyResources(this.splitterControl2, "splitterControl2");
			this.splitterControl2.Name = "splitterControl2";
			this.splitterControl2.TabStop = false;
			this.Name = "SchemeDesigner";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlControls)).EndInit();
			this.pnlControls.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcFormats)).EndInit();
			this.gcFormats.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lsStyles)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceNew.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pcFormats)).EndInit();
			this.pcFormats.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlCheckBox)).EndInit();
			this.pnlCheckBox.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gcPaintStyle)).EndInit();
			this.gcPaintStyle.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.listBoxControl1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		#region Init & Ctor
		protected override string DescriptionText { 
			get { return DevExpress.XtraGrid.Design.Properties.Resources.SchemeDescription; }
		}
		private DevExpress.XtraGrid.Views.Base.BaseView EditingView { get { return EditingObject as DevExpress.XtraGrid.Views.Base.BaseView; } }
		private DevExpress.XtraGrid.Design.XAppearances xs;
		private DevExpress.XtraGrid.Design.XViewsPrinting XV;
		DevExpress.XtraGrid.Design.XAppearances XS {
			get {
				if(xs == null) xs = new DevExpress.XtraGrid.Design.XAppearances("System");
				return xs;
			}
		}
		private DevExpress.XtraGrid.GridControl GridPattern;
		void CreateGrid() {
			try {
				GridPattern = Activator.CreateInstance(EditingView.GridControl.GetType()) as GridControl;
			} catch {
				GridPattern = new DevExpress.XtraGrid.GridControl();
			}
			GridPattern.Dock = System.Windows.Forms.DockStyle.Fill;
			AddPreviewControl(GridPattern);
		}
		public override void InitComponent() {
			CreateGrid();
			try {
				DevExpress.XtraGrid.Views.Base.BaseView oldView = GridPattern.MainView;
				GridPattern.MainView = GridPattern.CreateView(EditingView.BaseInfo.ViewName);
				if(oldView != null) oldView.Dispose();
			} catch {}
			XV = new DevExpress.XtraGrid.Design.XViewsPrinting(GridPattern, true);
			StyleList.Items.Clear();
			StyleList.Items.AddRange(XS.FormatNames);
			DevExpress.XtraGrid.Design.GridAssign.AppearanceAssign(EditingView, GridPattern.MainView);
			for(int i = 0; i < EditingView.BaseInfo.PaintStyles.Count; i++) {
				if(!string.Equals(EditingView.BaseInfo.PaintStyles[i].Name, "Skin"))
					listBoxControl1.Items.Add(EditingView.BaseInfo.PaintStyles[i].Name);
			}
			listBoxControl1.SelectedItem = GridPattern.MainView.PaintStyleName;
			btApply.Enabled = false;
		}
		#endregion
		#region Editing
		protected override void LoadSchemePreview() {
			XS.LoadScheme(StyleList.SelectedItem.ToString(), GridPattern.MainView);
		}
		protected override void SetFormatNames(bool isEnabled) {
			XS.ShowNewStylesOnly = isEnabled;
			StyleList.Items.AddRange(XS.FormatNames);
		}
		protected override void btApply_Click(object sender, System.EventArgs e) {
			bool needFireChanged = false;
			if(EditingView.PaintStyleName != GridPattern.MainView.PaintStyleName) { 
			EditingView.PaintStyleName = GridPattern.MainView.PaintStyleName;
				needFireChanged = true;
			}
			if(StyleList.SelectedItem != null) {
				if(StyleList.SelectedIndex == 0) needFireChanged = true;
				XS.LoadScheme(StyleList.SelectedItem.ToString(), EditingView);
			}
			if(needFireChanged) FireChanged();
		}
		public override void StoreLocalProperties(PropertyStore localStore) {
			base.StoreLocalProperties(localStore);
			localStore.AddProperty("PaintStyleControl", pnlControl.Width - gcFormats.Width);
		}
		const int DefaultSlitterWidth = 12;
		public override void RestoreLocalProperties(PropertyStore localStore) {
			base.RestoreLocalProperties(localStore);
			int gcFormatWidth = localStore.RestoreIntProperty("ControlPanel", pnlControl.Width);
			pnlControl.Width += localStore.RestoreIntProperty("PaintStyleControl", gcPaintStyle.Width);
			gcPaintStyle.Width = pnlControl.Width - gcFormatWidth - DefaultSlitterWidth;
		}
		private void FireChanged() {
			IComponentChangeService srv = EditingView.GridControl.InternalGetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) srv.OnComponentChanged(EditingView, null, null, null);
		}
		private void OnPaintStyleSelectedIndexChanged(object sender, System.EventArgs e) {
			GridPattern.MainView.PaintStyleName = listBoxControl1.SelectedItem.ToString();
			btApply.Enabled = true;
		}
		#endregion
	}
}
