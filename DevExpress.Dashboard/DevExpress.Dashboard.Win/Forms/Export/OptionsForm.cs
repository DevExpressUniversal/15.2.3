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

using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardExport;
using DevExpress.DashboardWin.Forms.Export.Groups;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Native.Printing;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Forms.Export {
	public partial class OptionsForm : DevExpress.DashboardWin.Native.DashboardForm {
		public const string DashboardType = "Dashboard";
		string type;
		DashboardExportFormat format;
		IExportOptionsOwner options;
		OptionsGroup group;
		public OptionsForm() {
			InitializeComponent();
		}
		public OptionsForm(string name, string type, DashboardExportFormat format, IExportOptionsOwner options)
			: this() {
			this.type = type;
			this.format = format;
			this.options = options;
			Text = GetTitle(name, format);
		}
		protected virtual string GetTitle(string name, DashboardExportFormat format) { return String.Empty; }
		protected virtual string GetSubmitButtonText() { return buttonSubmit.Text; }
		protected virtual OptionsGroup CreateGroup(string type, DashboardExportFormat format) { return null; }
		ExtendedReportOptions GetActualOptions() {
			return options != null ? options.GetActual() : ExtendedReportOptions.Empty;
		}
		void OnButtonCancelClick(object sender, EventArgs e) {
			Close();
		}
		void OnButtonSubmitClick(object sender, EventArgs e) {
			ExtendedReportOptions opts = GetActualOptions();
			group.Apply(opts);
			options.Set(opts);
		}
		void OnButtonResetClick(object sender, EventArgs e) {
			group.Set(options.GetDefault());
		}
		void OnLoad(object sender, EventArgs e) {
			const int minFormWidth = 310;
			const int offset = 30;
			int borderWidth = SystemInformation.BorderSize.Width;
			int oldFormHeight = Size.Height - tableLayoutPanel.Height;
			group = CreateGroup(type, format);
			if(group != null) {
				group.Init(GetActualOptions());
				int rowNumber = 0;
				foreach(LabelAndEditor labelAndEditor in group.GetControls()) {
					labelAndEditor.Label.Anchor = AnchorStyles.Left;
					if(labelAndEditor.Label.Height < labelAndEditor.Editor.Height) {
						labelAndEditor.Label.Top = (labelAndEditor.Editor.Height - labelAndEditor.Label.Height) / 2;
					} else {
						labelAndEditor.Editor.Top = (labelAndEditor.Label.Height - labelAndEditor.Editor.Height) / 2;
					}
					tableLayoutPanel.Controls.Add(labelAndEditor.Label, 0, rowNumber);
					tableLayoutPanel.Controls.Add(labelAndEditor.Editor, 2, rowNumber);
					rowNumber++;
				}
			}
			int desiredFormWidth = tableLayoutPanel.Width + 2 * offset;
			if(desiredFormWidth > minFormWidth) {
				Width = desiredFormWidth;
				tableLayoutPanel.Left = offset - borderWidth;
			} else {
				Width = minFormWidth;
				tableLayoutPanel.Left = (minFormWidth - tableLayoutPanel.Width) / 2 - borderWidth;
			}
			Height = tableLayoutPanel.Height + oldFormHeight;
			CancelButton = this.buttonCancel;
			this.buttonSubmit.Text = GetSubmitButtonText();
		}
		protected override void Dispose(bool disposing) {
			if(disposing && group != null) {
				group.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
