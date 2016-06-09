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
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils.Frames;
using DevExpress.Utils.Design;
namespace DevExpress.XtraEditors.Frames {
	[ToolboxItem(false)]
	public class SchemeDesignerBase : DevExpress.XtraEditors.Designer.Utils.XtraFrame {
		public PanelControl pnlControl;
		protected DevExpress.XtraEditors.PanelControl pnlControls;
		protected DevExpress.XtraEditors.SimpleButton btApply;
		private DevExpress.XtraEditors.SplitterControl splitter1;
		public GroupControl gcFormats;
		public ListBoxControl lsStyles;
		public CheckEdit ceNew;
		private DevExpress.XtraEditors.GroupControl groupControl2;
		public PanelControl pcFormats;
		public PanelControl pnlCheckBox;
		private System.ComponentModel.Container components = null;
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SchemeDesignerBase));
			this.btApply = new DevExpress.XtraEditors.SimpleButton();
			this.pnlControls = new DevExpress.XtraEditors.PanelControl();
			this.pnlControl = new DevExpress.XtraEditors.PanelControl();
			this.pcFormats = new DevExpress.XtraEditors.PanelControl();
			this.gcFormats = new DevExpress.XtraEditors.GroupControl();
			this.lsStyles = new DevExpress.XtraEditors.ListBoxControl();
			this.pnlCheckBox = new DevExpress.XtraEditors.PanelControl();
			this.ceNew = new DevExpress.XtraEditors.CheckEdit();
			this.splitter1 = new DevExpress.XtraEditors.SplitterControl();
			this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlControls)).BeginInit();
			this.pnlControls.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pcFormats)).BeginInit();
			this.pcFormats.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcFormats)).BeginInit();
			this.gcFormats.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lsStyles)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlCheckBox)).BeginInit();
			this.pnlCheckBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceNew.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
			this.SuspendLayout();
			this.lbCaption.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lbCaption.Appearance.Font")));
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			resources.ApplyResources(this.lbCaption, "lbCaption");
			this.pnlMain.Controls.Add(this.groupControl2);
			this.pnlMain.Controls.Add(this.splitter1);
			this.pnlMain.Controls.Add(this.pnlControl);
			this.pnlMain.Controls.Add(this.pnlControls);
			resources.ApplyResources(this.pnlMain, "pnlMain");
			resources.ApplyResources(this.horzSplitter, "horzSplitter");
			resources.ApplyResources(this.btApply, "btApply");
			this.btApply.Name = "btApply";
			this.btApply.Click += new System.EventHandler(this.btApply_Click);
			this.pnlControls.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlControls.Controls.Add(this.btApply);
			resources.ApplyResources(this.pnlControls, "pnlControls");
			this.pnlControls.Name = "pnlControls";
			this.pnlControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlControl.Controls.Add(this.pcFormats);
			resources.ApplyResources(this.pnlControl, "pnlControl");
			this.pnlControl.Name = "pnlControl";
			this.pcFormats.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pcFormats.Controls.Add(this.gcFormats);
			this.pcFormats.Controls.Add(this.pnlCheckBox);
			resources.ApplyResources(this.pcFormats, "pcFormats");
			this.pcFormats.Name = "pcFormats";
			this.gcFormats.Controls.Add(this.lsStyles);
			resources.ApplyResources(this.gcFormats, "gcFormats");
			this.gcFormats.Name = "gcFormats";
			this.lsStyles.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.lsStyles, "lsStyles");
			this.lsStyles.ItemHeight = 16;
			this.lsStyles.Name = "lsStyles";
			this.lsStyles.SelectedIndexChanged += new System.EventHandler(this.lsStyles_SelectedIndexChanged);
			this.pnlCheckBox.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlCheckBox.Controls.Add(this.ceNew);
			resources.ApplyResources(this.pnlCheckBox, "pnlCheckBox");
			this.pnlCheckBox.Name = "pnlCheckBox";
			this.pnlCheckBox.Paint += new System.Windows.Forms.PaintEventHandler(this.ceNew_Paint);
			resources.ApplyResources(this.ceNew, "ceNew");
			this.ceNew.Name = "ceNew";
			this.ceNew.Properties.AutoHeight = ((bool)(resources.GetObject("ceNew.Properties.AutoHeight")));
			this.ceNew.Properties.Caption = resources.GetString("ceNew.Properties.Caption");
			this.ceNew.CheckedChanged += new System.EventHandler(this.ceNew_CheckedChanged);
			resources.ApplyResources(this.splitter1, "splitter1");
			this.splitter1.Name = "splitter1";
			this.splitter1.TabStop = false;
			resources.ApplyResources(this.groupControl2, "groupControl2");
			this.groupControl2.Name = "groupControl2";
			this.Name = "SchemeDesignerBase";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlControls)).EndInit();
			this.pnlControls.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pcFormats)).EndInit();
			this.pcFormats.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcFormats)).EndInit();
			this.gcFormats.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lsStyles)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlCheckBox)).EndInit();
			this.pnlCheckBox.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceNew.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		#region Init & Ctor
		public ListBoxControl StyleList { get {return lsStyles; } }
		protected virtual void AddPreviewControl(Control control) {
			groupControl2.Controls.Add(control);
		} 
		public SchemeDesignerBase() : base(5) {
			InitializeComponent();
		}
		protected override string DescriptionText {
			get { return "Select the painting scheme and click the Apply button. This will change the appearance settings of the current control."; }
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion
		#region Editing
		private void lsStyles_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(lsStyles.SelectedItem != null) {
				LoadSchemePreview();
				lbCaption.Text = (lsStyles.SelectedIndex == 0 ? "Default Style Scheme" : "Style Schemes : " + lsStyles.SelectedItem.ToString());
			}
			btApply.Enabled = lsStyles.SelectedIndex > -1;
		}
		protected virtual void btApply_Click(object sender, System.EventArgs e) {
			if(lsStyles.SelectedItem != null) 
				LoadScheme();
		}
		private void ceNew_CheckedChanged(object sender, System.EventArgs e) {
			int index = lsStyles.SelectedIndex;
			lsStyles.Items.Clear();
			SetFormatNames(ceNew.Checked);
			lsStyles.SelectedIndex = index;
		}
		protected virtual void LoadSchemePreview() {}
		protected virtual void LoadScheme() {}
		protected virtual void SetFormatNames(bool isEnabled) {}
		#endregion
		public override void StoreLocalProperties(PropertyStore localStore) {
			localStore.AddProperty("ControlPanel", gcFormats.Width);
			localStore.AddProperty("NewStyles", ceNew.Checked);
		}
		public override void RestoreLocalProperties(PropertyStore localStore) {
			pnlControl.Width = localStore.RestoreIntProperty("ControlPanel", pnlControl.Width);
			gcFormats.Width = pnlControl.Width;
			ceNew.Checked = localStore.RestoreBoolProperty("NewStyles", false);
		}
		private void ceNew_Paint(object sender, PaintEventArgs e) {
			Color borderColor = DevExpress.Skins.CommonSkins.GetSkin(LookAndFeel)[DevExpress.Skins.CommonSkins.SkinTextBorder].Border.Left;
			e.Graphics.DrawLine(new Pen(borderColor), new Point(0, 0), new Point(0, pnlCheckBox.Bounds.Height - 1));
			e.Graphics.DrawLine(new Pen(borderColor), new Point(pnlCheckBox.Bounds.Width - 1, 0), new Point(pnlCheckBox.Bounds.Width - 1, pnlCheckBox.Bounds.Height - 1));
			e.Graphics.DrawLine(new Pen(borderColor), new Point(0, pnlCheckBox.Bounds.Height - 1), new Point(pnlCheckBox.Bounds.Width - 1, pnlCheckBox.Bounds.Height - 1));
		}
	}
}
