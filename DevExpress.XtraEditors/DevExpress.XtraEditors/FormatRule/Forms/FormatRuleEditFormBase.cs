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
using DevExpress.XtraEditors.Helpers;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraEditors.FormatRule.Forms {
	public partial class FormatRuleEditFormBase : XtraForm {
		Size filterSize = Size.Empty;
		IFilterControl filterControl = null;
		public FormatRuleEditFormBase() {
			ShowInTaskbar = false;
			InitializeComponent();
			InitAppearances();
			InitCaptions();
		}
		public void Init(string formCaption, string caption) {
			this.Text = formCaption;
			lbCaption.AllowHtmlString = true;
			lbCaption.Text = string.Format("<b>{0}</b>", caption);
			lbWith.Text = Localizer.Active.GetLocalizedString(StringId.FormatRuleForThisColumnWith);
		}
		void InitCaptions() {
			sbOK.Text = Localizer.Active.GetLocalizedString(StringId.OK);
			sbCancel.Text = Localizer.Active.GetLocalizedString(StringId.Cancel);
			ceRowFormat.Text = Localizer.Active.GetLocalizedString(StringId.FormatRuleApplyFormatProperty);
			lbWith.Padding = new Padding(0, (icbFormat.Height - lbWith.Height) / 2, 0, 0);
		}
		void InitAppearances() {
			icbFormat.Properties.DropDownRows = 15;
			foreach(FormatPredefinedAppearance app in FormatPredefinedAppearances.Default.Find(this.LookAndFeel))
				icbFormat.Properties.Items.Add(new ImageComboBoxItem(app.Title, app.Key, -1));
			icbFormat.Properties.Sorted = true;
			icbFormat.SelectedIndex = 0;
		}
		public void UpdateSize() {
			pnlButtons.Height = sbOK.CalcBestSize().Height + pnlButtons.Padding.Vertical;
			pnlCheckEdit.Height = ceRowFormat.Height + pnlCheckEdit.Padding.Vertical;
			pnlCaption.Height = lbCaption.Height + pnlCaption.Padding.Vertical;
			int editorStringWidth = icbFormat.Width + this.Padding.Horizontal + pnlSeparator2.Width + lbWith.Width + pnlEditors.Width + pnlRightIndent.Width;
			UpdateRightIndentSize(editorStringWidth, lbCaption.Width + Padding.Horizontal);
			this.ClientSize = new Size(
				GetMaxValue(new int[] { editorStringWidth, filterSize.Width, lbCaption.Width + Padding.Horizontal }),
				pnlButtons.Height + pnlCaption.Height + pnlCheckEdit.Height + this.Padding.Vertical + pnlMain.Padding.Vertical + icbFormat.Height + filterSize.Height);
			if(WindowsFormsSettings.GetIsRightToLeft(this))
				RightToLeftHelper.UpdateRightToLeftDockStyle(this);
		}
		void UpdateRightIndentSize(int editorStringWidth, int captionWidth) {
			if(captionWidth > editorStringWidth) pnlRightIndent.Width = captionWidth - editorStringWidth;
		}
		int GetMaxValue(int[] values) {
			int ret = 0;
			foreach(int val in values)
				if(ret < val) ret = val;
			return ret;
		}
		public string AppearanceName { get { return string.Format("{0}", icbFormat.EditValue); } }
		public bool ApplyToRow { get { return ceRowFormat.Checked; } }
		public void AddEditors(Control[] controls) {
			int tabIndex = 99;
			foreach(Control c in controls) {
				pnlEditors.Controls.Add(c);
				if(c is LabelControl) c.Padding = lbWith.Padding;
				c.Dock = DockStyle.Left;
				c.TabIndex = tabIndex--;
				pnlEditors.Width += c.Width;
				BaseEdit edit = c as BaseEdit;
				if(edit != null) {
					edit.EditValueChanged += (s, e) => {
						sbOK.Enabled = IsValuesExist;
					};
					sbOK.Enabled = IsValuesExist;
				}
			}
		}
		public void AddFilterControl(IFilterControl fc, Size size) {
			filterControl = fc;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			pnlMain.Dock = DockStyle.Bottom;
			pnlMain.Padding = new Padding(0, 10, 0, 10);
			pnlMain.Height = icbFormat.Height + pnlMain.Padding.Vertical;
			lbCaption.Padding = new Padding(0, 0, 0, 10);
			lbWith.Dock = pnlSeparator2.Dock = icbFormat.Dock = DockStyle.Left;
			lbWith.SendToBack();
			icbFormat.BringToFront();
			filterSize = size;
			Control fControl = fc as Control;
			this.Controls.Add(fControl);
			fControl.Dock = DockStyle.Fill;
			fControl.TabIndex = 0;
			fControl.BringToFront();
			lbWith.Text = Localizer.Active.GetLocalizedString(StringId.FormatRuleWith);
			fc.FilterTextChanged += (s, e) => {
				sbOK.Enabled = fc is FilterControl ? IsValuesExist : e.IsValid;
			};
			this.MinimumSize = size;
		}
		public void SetApplyToRowText(string text, bool enabled) {
			ceRowFormat.Text = text;
			ceRowFormat.Enabled = enabled;
			ceRowFormat.Checked = true;
		}
		bool IsValuesExist {
			get {
				foreach(Control c in pnlEditors.Controls) {
					BaseEdit edit = c as BaseEdit;
					if(edit != null && string.IsNullOrEmpty(string.Format("{0}", edit.EditValue))) return false;
					if(edit != null && edit is CheckedComboBoxEdit && FilterDateType.User.Equals(edit.Tag) &&
						string.Format("{0}", edit.EditValue).Equals(Localizer.Active.GetLocalizedString(StringId.FilterCriteriaToStringFunctionNone))) return false;
				}
				if(filterControl != null && ReferenceEquals(filterControl.FilterCriteria, null)) return false;
				return  true;
			}
		}
		internal Control CreateSeparator() {
			lbWith.Text = Localizer.Active.GetLocalizedString(StringId.FormatRuleWith);
			PanelControl pnl = new PanelControl();
			pnl.BorderStyle = BorderStyles.NoBorder;
			pnl.Width = 10;
			return pnl;
		}
		public void GetEditValues(List<object> list) {
			if(filterControl != null)
				list.Add(filterControl.FilterCriteria);
			else {
				for(int i = pnlEditors.Controls.Count - 1; i >= 0; i--) {
					BaseEdit edit = pnlEditors.Controls[i] as BaseEdit;
					if(edit != null) list.Add(edit.EditValue);
				}
			}
		}
		public int GetEditorWidth() {
			return icbFormat.Width;
		}
	}
}
