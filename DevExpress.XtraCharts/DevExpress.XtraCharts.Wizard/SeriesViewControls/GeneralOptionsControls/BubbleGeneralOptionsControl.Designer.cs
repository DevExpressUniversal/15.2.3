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

namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	partial class BubbleGeneralOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BubbleGeneralOptionsControl));
			this.pnlMinSize = new DevExpress.XtraEditors.PanelControl();
			this.spnMinSize = new DevExpress.XtraEditors.SpinEdit();
			this.lblMinSize = new DevExpress.XtraEditors.LabelControl();
			this.sepSeparator = new DevExpress.XtraEditors.PanelControl();
			this.pnlMaxSize = new DevExpress.XtraEditors.PanelControl();
			this.spnMaxSize = new DevExpress.XtraEditors.SpinEdit();
			this.lblMaxSize = new DevExpress.XtraEditors.LabelControl();
			this.grpBubbleOptions = new DevExpress.XtraEditors.GroupControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlMinSize)).BeginInit();
			this.pnlMinSize.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnMinSize.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepSeparator)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxSize)).BeginInit();
			this.pnlMaxSize.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnMaxSize.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpBubbleOptions)).BeginInit();
			this.grpBubbleOptions.SuspendLayout();
			this.SuspendLayout();
			this.pnlMinSize.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMinSize.Controls.Add(this.spnMinSize);
			this.pnlMinSize.Controls.Add(this.lblMinSize);
			resources.ApplyResources(this.pnlMinSize, "pnlMinSize");
			this.pnlMinSize.Name = "pnlMinSize";
			resources.ApplyResources(this.spnMinSize, "spnMinSize");
			this.spnMinSize.Name = "spnMinSize";
			this.spnMinSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnMinSize.Properties.Increment = new decimal(new int[] {
			1,
			0,
			0,
			65536});
			this.spnMinSize.Properties.MaxValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.spnMinSize.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			65536});
			this.spnMinSize.Properties.ValidateOnEnterKey = true;
			this.spnMinSize.EditValueChanged += new System.EventHandler(this.spnMinSize_EditValueChanged);
			this.spnMinSize.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.spnMinSize_EditValueChanging);
			resources.ApplyResources(this.lblMinSize, "lblMinSize");
			this.lblMinSize.Name = "lblMinSize";
			this.sepSeparator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepSeparator, "sepSeparator");
			this.sepSeparator.Name = "sepSeparator";
			resources.ApplyResources(this.pnlMaxSize, "pnlMaxSize");
			this.pnlMaxSize.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMaxSize.Controls.Add(this.spnMaxSize);
			this.pnlMaxSize.Controls.Add(this.lblMaxSize);
			this.pnlMaxSize.Name = "pnlMaxSize";
			resources.ApplyResources(this.spnMaxSize, "spnMaxSize");
			this.spnMaxSize.Name = "spnMaxSize";
			this.spnMaxSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnMaxSize.Properties.Increment = new decimal(new int[] {
			1,
			0,
			0,
			65536});
			this.spnMaxSize.Properties.MaxValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.spnMaxSize.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			65536});
			this.spnMaxSize.Properties.ValidateOnEnterKey = true;
			this.spnMaxSize.EditValueChanged += new System.EventHandler(this.spnMaxSize_EditValueChanged);
			this.spnMaxSize.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.spnMaxSize_EditValueChanging);
			resources.ApplyResources(this.lblMaxSize, "lblMaxSize");
			this.lblMaxSize.Name = "lblMaxSize";
			resources.ApplyResources(this.grpBubbleOptions, "grpBubbleOptions");
			this.grpBubbleOptions.Controls.Add(this.pnlMinSize);
			this.grpBubbleOptions.Controls.Add(this.sepSeparator);
			this.grpBubbleOptions.Controls.Add(this.pnlMaxSize);
			this.grpBubbleOptions.Name = "grpBubbleOptions";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grpBubbleOptions);
			this.Name = "BubbleGeneralOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlMinSize)).EndInit();
			this.pnlMinSize.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnMinSize.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepSeparator)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMaxSize)).EndInit();
			this.pnlMaxSize.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnMaxSize.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpBubbleOptions)).EndInit();
			this.grpBubbleOptions.ResumeLayout(false);
			this.grpBubbleOptions.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.PanelControl pnlMinSize;
		private DevExpress.XtraEditors.SpinEdit spnMinSize;
		private DevExpress.XtraEditors.LabelControl lblMinSize;
		private DevExpress.XtraEditors.PanelControl sepSeparator;
		private DevExpress.XtraEditors.PanelControl pnlMaxSize;
		private DevExpress.XtraEditors.SpinEdit spnMaxSize;
		private DevExpress.XtraEditors.LabelControl lblMaxSize;
		private DevExpress.XtraEditors.GroupControl grpBubbleOptions;
	}
}
