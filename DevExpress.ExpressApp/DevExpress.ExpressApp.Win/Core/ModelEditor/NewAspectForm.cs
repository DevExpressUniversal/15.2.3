#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public class NewAspectForm : XtraForm, IComparer<NewAspectForm.CultureDescription> {
		class CultureDescription {
			private string name;
			private string caption;
			public CultureDescription(CultureInfo culture) {
				this.name = culture.Name;
				caption = string.Concat(new string[] { name, " (", culture.DisplayName, " - ", culture.NativeName, ")" });
			}
			public override string ToString() {
				return caption;
			}
			public override int GetHashCode() {
				return base.GetHashCode();
			}
			public override bool Equals(object obj) {
				CultureDescription objCulture = obj as CultureDescription;
				if(objCulture != null) {
					return name == objCulture.Name;
				} else
					return base.Equals(obj);
			}
			public string Caption {
				get { return caption; }
			}
			public string Name {
				get { return name; }
			}
		}
		private System.ComponentModel.Container components = null;
		private SimpleButton okButton;
		private SimpleButton cancelButton;
		private ComboBoxEdit aspectComboBoxEdit;
		private void InitializeComponent() {
			this.okButton = new DevExpress.XtraEditors.SimpleButton();
			this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
			this.aspectComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
			this.SuspendLayout();
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Enabled = false;
			this.okButton.Location = new System.Drawing.Point(80, 48);
			this.okButton.Name = "okButton";
			this.okButton.TabIndex = 1;
			this.okButton.Text = "OK";
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(160, 48);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			this.aspectComboBoxEdit.Location = new System.Drawing.Point(17, 17);
			this.aspectComboBoxEdit.Name = "aspectComboBoxEdit";
			this.aspectComboBoxEdit.Size = new System.Drawing.Size(220, 20);
			this.aspectComboBoxEdit.TabIndex = 0;
			this.aspectComboBoxEdit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			this.aspectComboBoxEdit.EditValueChanged += new System.EventHandler(this.aspectComboBoxEdit_EditValueChanged);
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(248, 78);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.aspectComboBoxEdit);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.ShowInTaskbar = false;
			this.KeyPreview = true;
			this.Name = "NewAspectForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select language";
			LookAndFeelUtils.ApplyStyle(this);
			this.ResumeLayout(false);
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(components != null) {
						components.Dispose();
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		public NewAspectForm() {
			InitializeComponent();
		}
		public void FillAspects(ICollection<string> existingAspects) {
			aspectComboBoxEdit.Properties.Items.BeginUpdate();
			aspectComboBoxEdit.Properties.Items.Clear();
			List<CultureDescription> cultures = new List<CultureDescription>();
			CultureDescription invariantCultureDescr = new CultureDescription(CultureInfo.InvariantCulture);
			foreach(CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures)) {
				CultureDescription cultureDescr = new CultureDescription(culture);
				if(cultureDescr.Equals(invariantCultureDescr)) continue;
				bool founded = false;
				foreach(string aspect in existingAspects) {
					founded = (aspect == cultureDescr.Name);
					if(founded) break;
				}
				if(!founded && cultures.IndexOf(cultureDescr) == -1) {
					cultures.Add(cultureDescr);
				}
			}
			cultures.Sort(this);
			aspectComboBoxEdit.Properties.Items.AddRange(cultures.ToArray());
			aspectComboBoxEdit.Properties.Items.EndUpdate();
		}
		public string SelectedAspect {
			get { return ((CultureDescription)aspectComboBoxEdit.EditValue).Name; }
		}
		private void aspectComboBoxEdit_EditValueChanged(object sender, EventArgs e) {
			okButton.Enabled = true;
		}
		#region IComparer<CultureDescription> Members
		int IComparer<NewAspectForm.CultureDescription>.Compare(NewAspectForm.CultureDescription x, NewAspectForm.CultureDescription y) {
			return string.Compare(x.Caption, y.Caption);
		}
		#endregion
	}
}
