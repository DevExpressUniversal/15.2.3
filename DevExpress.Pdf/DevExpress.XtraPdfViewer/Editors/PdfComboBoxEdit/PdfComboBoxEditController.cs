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

using System.Collections.Generic;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
namespace DevExpress.XtraPdfViewer.Native {
	public class PdfComboBoxController : PdfTextEditBasedEditorContoller {
		readonly PdfComboBoxEdit comboBox;
		readonly PdfComboBoxSettings settings;
		protected override BaseControl Control { get { return comboBox; } }
		protected override TextEdit Editor { get { return comboBox; } }
		protected override object Value {
			get {
				int i = comboBox.SelectedIndex;
				IList<PdfOptionsFormFieldOption> values = settings.Values;
				return values != null && i < values.Count && i >= 0 ? values[i].Text : comboBox.Text;
			}
		}
		public PdfComboBoxController(PdfViewer viewer, PdfComboBoxSettings settings, IPdfViewerValueEditingCallBack callback)
			: base(viewer, settings, callback) {
			this.settings = settings;
			comboBox = new PdfComboBoxEdit(this);
		}
		public override void SetUp() {
			RepositoryItemComboBox properties = comboBox.Properties;
			AppearanceObject dropDownAppearance = properties.AppearanceDropDown;
			dropDownAppearance.ForeColor = settings.FontColor;
			dropDownAppearance.BackColor = settings.BackgroundColor;
			dropDownAppearance.TextOptions.HAlignment = PdfEditorController.GetAlignment(settings.TextJustification);
			ComboBoxItemCollection items = properties.Items;
			items.BeginUpdate();
			IList<PdfOptionsFormFieldOption> values = settings.Values;
			properties.TextEditStyle = settings.Editable ? TextEditStyles.Standard : TextEditStyles.DisableTextEditor;
			string str = (string)settings.EditValue;
			comboBox.Text = str;
			if (values != null) {
				foreach (PdfOptionsFormFieldOption opt in values)
					items.Add(opt.ExportText);
				items.EndUpdate();
				if (values != null) {
					foreach (PdfOptionsFormFieldOption opt in values) {
						if (opt.Text == str)
							comboBox.SelectedItem = opt.ExportText;
					}
				}
			}
			base.SetUp();
		}
		protected override void SetFonts(Font scaledFont) {
			base.SetFonts(scaledFont);
			comboBox.Properties.AppearanceDropDown.Font = scaledFont;
		}
		protected override void DisposeFonts() {
			base.DisposeFonts();
			comboBox.Properties.AppearanceDropDown.Font.Dispose();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing)
				comboBox.Dispose();
		}
	}
}
