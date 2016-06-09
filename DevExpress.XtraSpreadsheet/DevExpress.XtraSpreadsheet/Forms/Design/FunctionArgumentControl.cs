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
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Forms.Design {
	[DXToolboxItem(false)]
	public class FunctionArgumentControl : XtraUserControl {
		readonly FunctionArgumentViewModel viewModel;
		private LabelControl lblName;
		private ReferenceEditControl edtValue;
		private LabelControl labelControl1;
		private LabelControl lblResult;
		public FunctionArgumentControl() {
			InitializeComponent();
		}
		public FunctionArgumentControl(FunctionArgumentViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			SetupBindings();
		}
		#region Properties
		public FunctionArgumentViewModel ViewModel { get { return viewModel; } }
		public ISpreadsheetControl SpreadsheetControl { get { return edtValue.SpreadsheetControl; } set { edtValue.SpreadsheetControl = value; } }
		public override bool Focused { get { return edtValue.Focused; } }
		public ReferenceEditControl EdtValue {get { return edtValue; }}
		#endregion
		void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FunctionArgumentControl));
			this.lblName = new DevExpress.XtraEditors.LabelControl();
			this.lblResult = new DevExpress.XtraEditors.LabelControl();
			this.edtValue = new DevExpress.XtraSpreadsheet.ReferenceEditControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.edtValue.Properties)).BeginInit();
			this.SuspendLayout();
			this.lblName.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			resources.ApplyResources(this.lblName, "lblName");
			this.lblName.Name = "lblName";
			resources.ApplyResources(this.lblResult, "lblResult");
			this.lblResult.Name = "lblResult";
			this.edtValue.Activated = false;
			this.edtValue.EditValuePrefix = "";
			this.edtValue.IncludeSheetName = false;
			resources.ApplyResources(this.edtValue, "edtValue");
			this.edtValue.Name = "edtValue";
			this.edtValue.PositionType = DevExpress.XtraSpreadsheet.Model.PositionType.Relative;
			this.edtValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtValue.SpreadsheetControl = null;
			this.edtValue.GotFocus += edtValue_GotFocus;
			this.edtValue.LostFocus += edtValue_LostFocus;
			this.labelControl1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.edtValue);
			this.Controls.Add(this.lblResult);
			this.Controls.Add(this.lblName);
			this.Name = "FunctionArgumentControl";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.edtValue.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		void edtValue_LostFocus(object sender, EventArgs e) {
			OnLostFocus(e);
		}
		void edtValue_GotFocus(object sender, EventArgs e) {
			OnGotFocus(e);
		}
		public Size CalcBestSize() {
			return this.Size;
		}
		void SetupBindings() {
			if (ViewModel == null)
				return;
			lblName.DataBindings.Add("Text", ViewModel, "Name");
			SetFontBold(lblName, ViewModel.IsRequired);
			lblResult.DataBindings.Add("Text", ViewModel, "ValueOrType", true, DataSourceUpdateMode.OnPropertyChanged).BindingComplete += OnResultBindingComplete;
			edtValue.DataBindings.Add("EditValue", ViewModel, "Value", false, DataSourceUpdateMode.OnPropertyChanged);
		}
		void OnResultBindingComplete(object sender, BindingCompleteEventArgs e) {
			SetFontBold(lblResult, ViewModel.IsInvalidOrError);
			if (ViewModel.IsInvalidOrError)
				lblResult.Appearance.ForeColor = Color.Red;
			else
				lblResult.Appearance.ForeColor = Color.Empty;
		}
		public static void SetFontBold(BaseStyleControl edit, bool bold) {
			AppearanceObject appearance = edit.Appearance;
			Font font = appearance.Font;
			if (font.Bold == bold)
				return;
			if (bold)
				appearance.Font = new Font(font.Name, font.Size, FontStyle.Bold);
			else
				appearance.Font = new Font(font.Name, font.Size, FontStyle.Regular);
		}
	}
}
