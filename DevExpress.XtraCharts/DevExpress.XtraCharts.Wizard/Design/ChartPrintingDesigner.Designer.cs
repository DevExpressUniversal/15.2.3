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

namespace DevExpress.XtraCharts.Design {
	partial class ChartPrintingDesigner {
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartPrintingDesigner));
			this.gcSizeModeRadio = new DevExpress.XtraEditors.GroupControl();
			this.cheZoom = new DevExpress.XtraEditors.CheckEdit();
			this.cheStretch = new DevExpress.XtraEditors.CheckEdit();
			this.cheNone = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gcSizeModeRadio)).BeginInit();
			this.gcSizeModeRadio.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cheZoom.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cheStretch.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cheNone.Properties)).BeginInit();
			this.SuspendLayout();
			this.lbCaption.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lbCaption.Appearance.Font")));
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			resources.ApplyResources(this.lbCaption, "lbCaption");
			this.pnlMain.Controls.Add(this.gcSizeModeRadio);
			resources.ApplyResources(this.pnlMain, "pnlMain");
			resources.ApplyResources(this.horzSplitter, "horzSplitter");
			this.gcSizeModeRadio.Controls.Add(this.cheZoom);
			this.gcSizeModeRadio.Controls.Add(this.cheStretch);
			this.gcSizeModeRadio.Controls.Add(this.cheNone);
			resources.ApplyResources(this.gcSizeModeRadio, "gcSizeModeRadio");
			this.gcSizeModeRadio.Name = "gcSizeModeRadio";
			resources.ApplyResources(this.cheZoom, "cheZoom");
			this.cheZoom.Name = "cheZoom";
			this.cheZoom.Properties.Appearance.Options.UseTextOptions = true;
			this.cheZoom.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.cheZoom.Properties.Caption = resources.GetString("cheZoom.Properties.Caption");
			this.cheZoom.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.cheZoom.Properties.RadioGroupIndex = 0;
			this.cheZoom.TabStop = false;
			this.cheZoom.CheckedChanged += new System.EventHandler(this.Item_CheckedChanged);
			resources.ApplyResources(this.cheStretch, "cheStretch");
			this.cheStretch.Name = "cheStretch";
			this.cheStretch.Properties.Appearance.Options.UseTextOptions = true;
			this.cheStretch.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.cheStretch.Properties.Caption = resources.GetString("cheStretch.Properties.Caption");
			this.cheStretch.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.cheStretch.Properties.RadioGroupIndex = 0;
			this.cheStretch.TabStop = false;
			this.cheStretch.CheckedChanged += new System.EventHandler(this.Item_CheckedChanged);
			resources.ApplyResources(this.cheNone, "cheNone");
			this.cheNone.Name = "cheNone";
			this.cheNone.Properties.Appearance.Options.UseTextOptions = true;
			this.cheNone.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.cheNone.Properties.Caption = resources.GetString("cheNone.Properties.Caption");
			this.cheNone.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.cheNone.Properties.RadioGroupIndex = 0;
			this.cheNone.TabStop = false;
			this.cheNone.CheckedChanged += new System.EventHandler(this.Item_CheckedChanged);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Name = "ChartPrintingDesigner";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gcSizeModeRadio)).EndInit();
			this.gcSizeModeRadio.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cheZoom.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cheStretch.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cheNone.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl gcSizeModeRadio;
		private DevExpress.XtraEditors.CheckEdit cheNone;
		private DevExpress.XtraEditors.CheckEdit cheZoom;
		private DevExpress.XtraEditors.CheckEdit cheStretch;
	}
}
