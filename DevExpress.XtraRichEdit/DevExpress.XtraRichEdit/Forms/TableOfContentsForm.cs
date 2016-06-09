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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Office.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class TableOfContentsForm : XtraForm {
		TableOfContentsFormController controller;
		public TableOfContentsForm() {
			InitializeComponent();
		}
		public void Initialize(TableOfContentsFormController controller) {
			this.controller = controller;
			chkRightAlignPageNumbers.Checked = controller.RightAlignPageNumbers;
			chkShowPageNumbers.Checked = controller.ShowPageNumbers;
			chkUseHyperlinks.Checked = controller.UseHyperlinks;
			spEditShowLevels.Value = controller.ShowLevels;
			FillPreviewControl();
		}
		private void chkShowPageNumbers_CheckedChanged(object sender, EventArgs e) {
			controller.ShowPageNumbers = chkShowPageNumbers.Checked;
			chkRightAlignPageNumbers.Enabled = controller.ShowPageNumbers;
			FillPreviewControl();
		}
		private void chkRightAlignPageNumbers_CheckedChanged(object sender, EventArgs e) {
			controller.RightAlignPageNumbers = chkRightAlignPageNumbers.Checked;
			FillPreviewControl();
		}
		private void chkUseHyperlinks_CheckedChanged(object sender, EventArgs e) {
			controller.UseHyperlinks = chkUseHyperlinks.Checked;
			FillPreviewControl();
		}
		private void btnOk_Click(object sender, EventArgs e) {
			controller.ApplyChanges();
			Close();
		}
		private void spEditShowLevels_EditValueChanged(object sender, EventArgs e) {
			controller.ShowLevels = (int)spEditShowLevels.Value;
			FillPreviewControl();
		}
		void FillPreviewControl() {
			previewControl.BeginUpdate();
			previewControl.CreateNewDocument();
			Document document = previewControl.Document;
			document.Unit = Office.DocumentUnit.Inch;
			float tabPosition = Units.PixelsToHundredthsOfInch(previewControl.Width - 50, previewControl.DpiX) / 100f;
			for (int i = 0; i < controller.ShowLevels; i++) {
				Paragraph paragraph = document.Paragraphs[i];
				paragraph.LeftIndent += 0.25f;
				string headingLevel = (i + 1).ToString();
				string pageNumber = (i * 2 + 1).ToString();
				document.InsertText(paragraph.Range.Start, XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_Heading) + headingLevel);
				if (controller.ShowPageNumbers) {
					if (controller.RightAlignPageNumbers) {
						TabInfoCollection tabs = paragraph.BeginUpdateTabs(true);
						TabInfo tab = new TabInfo();
						tab.Position = tabPosition;
						tab.Leader = TabLeaderType.Dots;
						tab.Alignment = TabAlignmentType.Right;
						tabs.Add(tab);
						paragraph.EndUpdateTabs(tabs);
						document.InsertText(paragraph.Range.End, "\t");
					}
					document.InsertText(paragraph.Range.End, "  " + pageNumber);
				}
				document.Paragraphs.Insert(document.Range.End);
			}
			document.Selection = document.CreateRange(document.CreatePosition(0), 0);
			previewControl.ScrollToCaret();
			previewControl.EndUpdate();
		}
		private void BtnCancel_Click(object sender, EventArgs e) {
			Close();
		}
	}
}
