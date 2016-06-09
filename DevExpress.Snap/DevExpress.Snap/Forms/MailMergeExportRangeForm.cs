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
using System.IO;
using System.Drawing;
using DevExpress.Snap.Core;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.Snap.Core.Forms;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Localization;
using DevExpress.XtraEditors.Controls;
using DevExpress.Snap.Core.Options;
#region FxCop Suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.Snap.Forms.MailMergeExportForm.groupBox1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.Snap.Forms.MailMergeExportForm.icbSeparator")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.Snap.Forms.MailMergeExportForm.labelControl1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.Snap.Forms.MailMergeExportForm.labelControl2")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.Snap.Forms.MailMergeExportForm.lblSeparator")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.Snap.Forms.MailMergeExportForm.radioGroupMode")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.Snap.Forms.MailMergeExportForm.simpleButton1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.Snap.Forms.MailMergeExportForm.simpleButton2")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.Snap.Forms.MailMergeExportForm.spinEditCount")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.Snap.Forms.MailMergeExportForm.spinEditFrom")]
#endregion
namespace DevExpress.Snap.Forms {
	public partial class MailMergeExportRangeForm : XtraForm {
		readonly ISnapControl control;
		readonly MailMergeExportFormController controller;
		MailMergeExportRangeForm() {
			InitializeComponent();
		}
		public MailMergeExportRangeForm(MailMergeExportFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = (ISnapControl)controllerParameters.Control;
			this.controller = CreateController(controllerParameters);
			InitializeComponent();
			InitializeRecordSeparatorComboBox();
			UpdateValues();
		}
		protected internal MailMergeExportFormController Controller { get { return controller; } }
		protected internal virtual MailMergeExportFormController CreateController(MailMergeExportFormControllerParameters controllerParameters) {
			return new MailMergeExportFormController(controllerParameters);
		}
		void UpdateValues() {
			radioGroupMode.SelectedIndex = (int)Controller.Mode;
			icbSeparator.SelectedIndex = (int)Controller.Separator;
			spinEditFrom.Value = Controller.From;
			spinEditCount.Value = Controller.Count;
		}
		void InitializeRecordSeparatorComboBox() {
			LoadSmallImages();
			AddRecordSeparatorImageComboBoxItems();
		}
		void LoadSmallImages() {
			ImageCollection imgCollection = new ImageCollection();
			LoadSmallImage(imgCollection, "SeparatorListNone");
			LoadSmallImage(imgCollection, "SeparatorPageBreakList");
			LoadSmallImage(imgCollection, "SectionBreaksList_NextPage");
			LoadSmallImage(imgCollection, "SectionBreaksList_EvenPage");
			LoadSmallImage(imgCollection, "SectionBreaksList_OddPage");
			LoadSmallImage(imgCollection, "EmptyParagraphSeparatorList");
			this.icbSeparator.Properties.SmallImages = imgCollection;
		}
		void LoadSmallImage(ImageCollection target, string imageName) {
			string path = string.Format("DevExpress.Snap.Images.{0}_16x16.png", imageName);
			Stream imageStream = typeof(DevExpress.Snap.CoreResFinder).Assembly.GetManifestResourceStream(path);
			target.AddImage(Image.FromStream(imageStream));
		}
		void AddRecordSeparatorImageComboBoxItems() {
			AddRecordSeparatorImageComboBoxItem(SnapStringId.RecordSeparator_None, RecordSeparator.None);
			AddRecordSeparatorImageComboBoxItem(SnapStringId.RecordSeparator_PageBreak, RecordSeparator.PageBreak);
			AddRecordSeparatorImageComboBoxItem(SnapStringId.RecordSeparator_SectionNextPage, RecordSeparator.SectionNextPage);
			AddRecordSeparatorImageComboBoxItem(SnapStringId.RecordSeparator_SectionEvenPage, RecordSeparator.SectionEvenPage);
			AddRecordSeparatorImageComboBoxItem(SnapStringId.RecordSeparator_SectionOddPage, RecordSeparator.SectionOddPage);
			AddRecordSeparatorImageComboBoxItem(SnapStringId.RecordSeparator_Paragraph, RecordSeparator.Paragraph);
		}
		void AddRecordSeparatorImageComboBoxItem(SnapStringId id, RecordSeparator value) {
			this.icbSeparator.Properties.Items.Add(new ImageComboBoxItem(SnapLocalizer.Active.GetLocalizedString(id), (int)value, (int)value));
		}
		void radioGroup1_SelectedIndexChanged(object sender, EventArgs e) {
			Controller.Mode = (MailMergeExportFormController.ModeType)radioGroupMode.SelectedIndex;
			groupBox1.Enabled = radioGroupMode.SelectedIndex == 2;
			icbSeparator.Enabled = radioGroupMode.SelectedIndex != 1;
			lblSeparator.Enabled = icbSeparator.Enabled;
		}
		void spinEdit1_EditValueChanged(object sender, EventArgs e) {
			Controller.From = (int)spinEditFrom.Value;
		}
		void spinEdit2_EditValueChanged(object sender, EventArgs e) {
			Controller.Count = (int)spinEditCount.Value;
			spinEditFrom.Properties.Increment = spinEditCount.Value;
		}
		void icbSeparator_SelectedIndexChanged(object sender, EventArgs e) {
			Controller.Separator = (RecordSeparator)icbSeparator.SelectedIndex;
		} 
		void simpleButton1_Click(object sender, EventArgs e) {
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Controller.ApplyChanges();
			Close();
		}
		void simpleButton2_Click(object sender, EventArgs e) {
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}
	}
}
