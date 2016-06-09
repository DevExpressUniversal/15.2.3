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
	partial class SparklineOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SparklineOptionsControl));
			this.ceShowMinMaxPoints = new DevExpress.XtraEditors.CheckEdit();
			this.ceShowStartEndPoints = new DevExpress.XtraEditors.CheckEdit();
			this.cbSparklineIndication = new DevExpress.XtraEditors.ComboBoxEdit();
			this.labelSparklineIndication = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.ceShowMinMaxPoints.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceShowStartEndPoints.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbSparklineIndication.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.ceShowMinMaxPoints, "ceShowMinMaxPoints");
			this.ceShowMinMaxPoints.Name = "ceShowMinMaxPoints";
			this.ceShowMinMaxPoints.Properties.Caption = resources.GetString("ceShowMinMaxPoints.Properties.Caption");
			this.ceShowMinMaxPoints.CheckedChanged += new System.EventHandler(this.ShowMinMaxValuesCheckedChanged);
			resources.ApplyResources(this.ceShowStartEndPoints, "ceShowStartEndPoints");
			this.ceShowStartEndPoints.Name = "ceShowStartEndPoints";
			this.ceShowStartEndPoints.Properties.Caption = resources.GetString("ceShowStartEndPoints.Properties.Caption");
			this.ceShowStartEndPoints.CheckedChanged += new System.EventHandler(this.ShowStartEndValuesCheckedChanged);
			resources.ApplyResources(this.cbSparklineIndication, "cbSparklineIndication");
			this.cbSparklineIndication.Name = "cbSparklineIndication";
			this.cbSparklineIndication.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbSparklineIndication.Properties.Buttons"))))});
			this.cbSparklineIndication.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbSparklineIndication.SelectedValueChanged += new System.EventHandler(this.SparklineIndicationChanged);
			resources.ApplyResources(this.labelSparklineIndication, "labelSparklineIndication");
			this.labelSparklineIndication.Name = "labelSparklineIndication";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.cbSparklineIndication);
			this.Controls.Add(this.labelSparklineIndication);
			this.Controls.Add(this.ceShowStartEndPoints);
			this.Controls.Add(this.ceShowMinMaxPoints);
			this.Name = "SparklineOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.ceShowMinMaxPoints.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceShowStartEndPoints.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbSparklineIndication.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.CheckEdit ceShowMinMaxPoints;
		private XtraEditors.CheckEdit ceShowStartEndPoints;
		private XtraEditors.ComboBoxEdit cbSparklineIndication;
		private XtraEditors.LabelControl labelSparklineIndication;
	}
}
