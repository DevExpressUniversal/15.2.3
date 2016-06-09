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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Printing;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.ShadingSetupForm.cbedtColorScheme")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.ShadingSetupForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.ShadingSetupForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.ShadingSetupForm.lblColorScheme")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.ShadingSetupForm.chklbApplyTo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.ShadingSetupForm.lblApplyTo")]
#endregion
namespace DevExpress.XtraScheduler.Forms {
	[System.Runtime.InteropServices.ComVisible(false)]
	public partial class ShadingSetupForm : DevExpress.XtraEditors.XtraForm {
		PrintColorConverter colorConverter;
		ShadingSetupForm() {
			InitializeComponent();
		}
		public ShadingSetupForm(DevExpress.XtraEditors.XtraForm parent, PrintColorConverter colorConverter) {
			this.LookAndFeel.ParentLookAndFeel = parent.LookAndFeel.ParentLookAndFeel;
			InitializeComponent();
			InitApplyToCheckedListBox();
			FillColorSchemesCombo();
			InitCurrentColorConverter(colorConverter);
			this.colorConverter = (PrintColorConverter)cbedtColorScheme.EditValue;
		}
		public PrintColorConverter ColorConverter {
			get { return colorConverter; }
		}
		void FillColorSchemesCombo() {
			PrintColorConverter conv;
			conv = new PrintColorConverter();
			cbedtColorScheme.Properties.Items.Add(new ImageComboBoxItem(conv.DisplayName, conv));
			conv = new GrayScalePrintColorConverter();
			cbedtColorScheme.Properties.Items.Add(new ImageComboBoxItem(conv.DisplayName, conv));
			conv = new BlackAndWhitePrintColorConverter();
			cbedtColorScheme.Properties.Items.Add(new ImageComboBoxItem(conv.DisplayName, conv));
		}
		void InitCurrentColorConverter(PrintColorConverter colorConverter) {
			if (colorConverter == null) {
				cbedtColorScheme.SelectedIndex = 0;
				UpdateApplyToList();
				return;
			}
			int count = cbedtColorScheme.Properties.Items.Count;
			for (int i = 0; i < count; i++) {
				PrintColorConverter conv = (PrintColorConverter)cbedtColorScheme.Properties.Items[i].Value;
				if (conv.GetType().Equals(colorConverter.GetType())) {
					cbedtColorScheme.SelectedIndex = i;
					UpdateApplyToList(colorConverter);
					return;
				}
			}
		}
		void UpdateApplyToList() {
			PrintColorConverter conv = (PrintColorConverter)cbedtColorScheme.EditValue;
			UpdateApplyToList(conv);
		}
		void UpdateApplyToList(PrintColorConverter conv) {
			chklbApplyTo.Items[0].CheckState = ToCheckState(conv.ApplyToAllDayArea);
			chklbApplyTo.Items[1].CheckState = ToCheckState(conv.ApplyToAppointment);
			chklbApplyTo.Items[2].CheckState = ToCheckState(conv.ApplyToAppointmentStatus);
			chklbApplyTo.Items[3].CheckState = ToCheckState(conv.ApplyToHeader);
			chklbApplyTo.Items[4].CheckState = ToCheckState(conv.ApplyToTimeRuler);
			chklbApplyTo.Items[5].CheckState = ToCheckState(conv.ApplyToCells);
		}
		void InitApplyToCheckedListBox() {
			chklbApplyTo.Items.Add(new CheckedListBoxItem(SchedulerLocalizer.GetString(SchedulerStringId.Caption_ShadingApplyToAllDayArea)));
			chklbApplyTo.Items.Add(new CheckedListBoxItem(SchedulerLocalizer.GetString(SchedulerStringId.Caption_ShadingApplyToAppointments)));
			chklbApplyTo.Items.Add(new CheckedListBoxItem(SchedulerLocalizer.GetString(SchedulerStringId.Caption_ShadingApplyToAppointmentStatuses)));
			chklbApplyTo.Items.Add(new CheckedListBoxItem(SchedulerLocalizer.GetString(SchedulerStringId.Caption_ShadingApplyToHeaders)));
			chklbApplyTo.Items.Add(new CheckedListBoxItem(SchedulerLocalizer.GetString(SchedulerStringId.Caption_ShadingApplyToTimeRulers)));
			chklbApplyTo.Items.Add(new CheckedListBoxItem(SchedulerLocalizer.GetString(SchedulerStringId.Caption_ShadingApplyToCells)));
		}
		static CheckState ToCheckState(bool val) {
			return val ? CheckState.Checked : CheckState.Unchecked;
		}
		static bool FromCheckState(CheckState val) {
			return val == CheckState.Checked;
		}
		void SaveApplyToList() {
			if (colorConverter == null)
				return;
			colorConverter.ApplyToAllDayArea = FromCheckState(chklbApplyTo.Items[0].CheckState);
			colorConverter.ApplyToAppointment = FromCheckState(chklbApplyTo.Items[1].CheckState);
			colorConverter.ApplyToAppointmentStatus = FromCheckState(chklbApplyTo.Items[2].CheckState);
			colorConverter.ApplyToHeader = FromCheckState(chklbApplyTo.Items[3].CheckState);
			colorConverter.ApplyToTimeRuler = FromCheckState(chklbApplyTo.Items[4].CheckState);
			colorConverter.ApplyToCells = FromCheckState(chklbApplyTo.Items[5].CheckState);
		}
		private void btnOk_Click(object sender, System.EventArgs e) {
			SaveApplyToList();
		}
		private void cbedtColorScheme_SelectedIndexChanged(object sender, System.EventArgs e) {
			SaveApplyToList();
			UpdateApplyToList();
			this.colorConverter = (PrintColorConverter)cbedtColorScheme.EditValue;
		}
	}
}
