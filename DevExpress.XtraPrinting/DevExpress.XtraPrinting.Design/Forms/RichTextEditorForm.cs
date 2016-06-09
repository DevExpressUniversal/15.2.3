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
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraPrinting.Native.RichText;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.Design.Forms {
	public partial class RichTextEditorForm : DevExpress.XtraEditors.XtraForm {
		private string editValue;
		public string EditValue {
			get { return editValue; }
			set {
				editValue = value;
				textBox.RtfText = value;
			}
		}
		public RichTextEditorForm() {
			InitializeComponent();
			textBox.DocumentLoaded += textBox_DocumentLoaded;
		}
		void textBox_DocumentLoaded(object sender, EventArgs e) {
			textBox.Document.DefaultCharacterProperties.FontName = XtraRichTextEditHelper.DefaultPlainTextFontName;
			textBox.Document.DefaultCharacterProperties.FontSize = XtraRichTextEditHelper.DefaultPlainTextFontSize;
			textBox.Document.Sections[0].Page.PaperKind = System.Drawing.Printing.PaperKind.Letter;
			textBox.Document.Sections[0].Margins.Left = 0;
			textBox.Document.Sections[0].Margins.Right = 0;
		}
		private void textBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Escape) {
				cancelButton.PerformClick();
				e.Handled = true;
			}
		}
		private void okButton_Click(object sender, System.EventArgs e) {
			if(editValue == textBox.RtfText) {
				DialogResult = DialogResult.Cancel;
			} else {
				editValue = !string.IsNullOrEmpty(textBox.Text) ? textBox.RtfText : string.Empty;
				DialogResult = DialogResult.OK;
			}
		}
		private void insertPageNumberItem2_ItemClick(object sender, XtraBars.ItemClickEventArgs e) {
			AddVariable(DOCVARIABLES.PageNumber, " \"{0}\"", DOCVARIABLES.PageNumber);
		}
		void AddVariable(string variableName, string format, string text) {
			Field field = textBox.Document.Fields.Create(textBox.Document.CaretPosition, string.Format("DOCVARIABLE {0}{1}", variableName, format));
			textBox.Document.InsertText(field.ResultRange.Start, text);
			field.ShowCodes = false;
		}
		private void insertPageCountItem2_ItemClick(object sender, XtraBars.ItemClickEventArgs e) {
			AddVariable(DOCVARIABLES.PageCount, " \"{0}\"", DOCVARIABLES.PageCount);
		}
		private void insertDateItem1_ItemClick(object sender, XtraBars.ItemClickEventArgs e) {
			AddVariable(DOCVARIABLES.Date, " \"{0:D}\"", DOCVARIABLES.Date);
		}
		private void insertUserItem1_ItemClick(object sender, XtraBars.ItemClickEventArgs e) {
			AddVariable(DOCVARIABLES.UserName, " \"{0}\"", DOCVARIABLES.UserName);
		}
	}
}
