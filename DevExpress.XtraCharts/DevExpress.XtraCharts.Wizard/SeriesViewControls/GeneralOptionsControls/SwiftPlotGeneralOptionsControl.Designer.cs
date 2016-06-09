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
	partial class SwiftPlotGeneralOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SwiftPlotGeneralOptionsControl));
			this.grpSwiftPlotOptions = new DevExpress.XtraEditors.GroupControl();
			this.chAntialiasing = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.grpSwiftPlotOptions)).BeginInit();
			this.grpSwiftPlotOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chAntialiasing.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.grpSwiftPlotOptions, "grpSwiftPlotOptions");
			this.grpSwiftPlotOptions.Controls.Add(this.chAntialiasing);
			this.grpSwiftPlotOptions.Name = "grpSwiftPlotOptions";
			resources.ApplyResources(this.chAntialiasing, "chAntialiasing");
			this.chAntialiasing.Name = "chAntialiasing";
			this.chAntialiasing.Properties.Caption = resources.GetString("chAntialiasing.Properties.Caption");
			this.chAntialiasing.CheckedChanged += new System.EventHandler(this.chAntialiasing_CheckedChanged);
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grpSwiftPlotOptions);
			this.Name = "SwiftPlotGeneralOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.grpSwiftPlotOptions)).EndInit();
			this.grpSwiftPlotOptions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chAntialiasing.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl grpSwiftPlotOptions;
		private DevExpress.XtraEditors.CheckEdit chAntialiasing;
	}
}
