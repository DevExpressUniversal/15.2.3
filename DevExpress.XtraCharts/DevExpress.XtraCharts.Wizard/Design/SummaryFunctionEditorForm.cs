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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Design {
	public class SummaryFunctionEditorForm : XtraForm {
		private SummaryFunctionBindingControl summaryFunctionBindingControl;
		private LabelControl lblSplitter;
		private SimpleButton btnOK;
		private SimpleButton btnCancel;
		public SummaryFunctionEditorForm() {
			InitializeComponent();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SummaryFunctionEditorForm));
			this.lblSplitter = new DevExpress.XtraEditors.LabelControl();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.summaryFunctionBindingControl = new DevExpress.XtraCharts.Design.SummaryFunctionBindingControl();
			this.SuspendLayout();
			resources.ApplyResources(this.lblSplitter, "lblSplitter");
			this.lblSplitter.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.lblSplitter.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.lblSplitter.LineVisible = true;
			this.lblSplitter.Name = "lblSplitter";
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.Name = "btnOK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.summaryFunctionBindingControl, "summaryFunctionBindingControl");
			this.summaryFunctionBindingControl.Name = "summaryFunctionBindingControl";
			resources.ApplyResources(this, "$this");
			this.ControlBox = false;
			this.Controls.Add(this.summaryFunctionBindingControl);
			this.Controls.Add(this.lblSplitter);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SummaryFunctionEditorForm";
			this.ShowInTaskbar = false;
			this.ResumeLayout(false);
		}
		#endregion
		public void Initialize(SeriesBase series, Chart chart, bool lockFunctionNameEditing, IServiceProvider provider) { 
			summaryFunctionBindingControl.Initialize(series, chart, lockFunctionNameEditing, provider);
		}
		void btnOK_Click(object sender, EventArgs e) {
			try {
				summaryFunctionBindingControl.ApplyChanges();
				DialogResult = DialogResult.OK;
			}
			catch (Exception exc) {
				XtraMessageBox.Show(LookAndFeel, exc.Message);
			}
		}
	}
}
