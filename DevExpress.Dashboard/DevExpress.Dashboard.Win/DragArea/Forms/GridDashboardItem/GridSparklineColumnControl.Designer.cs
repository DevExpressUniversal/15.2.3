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

namespace DevExpress.DashboardWin.Native {
	partial class GridSparklineColumnControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridSparklineColumnControl));
			this.sparklineOptionsControl = new DevExpress.DashboardWin.Native.SparklineOptionsControl();
			this.ceShowStartEndValues = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.ceShowStartEndValues.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.sparklineOptionsControl, "sparklineOptionsControl");
			this.sparklineOptionsControl.Name = "sparklineOptionsControl";
			this.sparklineOptionsControl.OptionsChanged += new System.EventHandler(this.SparklineOptionsControlChanged);
			resources.ApplyResources(this.ceShowStartEndValues, "ceShowStartEndValues");
			this.ceShowStartEndValues.Name = "ceShowStartEndValues";
			this.ceShowStartEndValues.Properties.Caption = resources.GetString("ceShowStartEndValues.Properties.Caption");
			this.ceShowStartEndValues.CheckedChanged += new System.EventHandler(this.ShowStartEndValuesCheckedChanged);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.ceShowStartEndValues);
			this.Controls.Add(this.sparklineOptionsControl);
			this.Name = "GridSparklineColumnControl";
			((System.ComponentModel.ISupportInitialize)(this.ceShowStartEndValues.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private SparklineOptionsControl sparklineOptionsControl;
		private XtraEditors.CheckEdit ceShowStartEndValues;
	}
}
