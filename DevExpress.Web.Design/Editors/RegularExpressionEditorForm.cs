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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
namespace DevExpress.Web.Design.Editors {
	class RegularExpressionEditorForm : XtraForm {
		TextEdit editTest;
		Label labelEditTest;
		Label labelRegExDescription;
		ListBoxControl listExpressions;
		Label lblList;
		SimpleButton btnCancel;
		SimpleButton btnOk;
		TextEdit editRegExpression;
		Label labelEditRegExpression;
		Regex regEx;
		static PredefinedRegExpression[] predefinedRegExpressions = new PredefinedRegExpression[] {
																		 new PredefinedRegExpression("AnySymbols", "Any symbols", @".+"),
																		 new PredefinedRegExpression("LatinLettersOnly", "Any letters of the latin alphabet", @"[a-zA-Z]+"),
																		 new PredefinedRegExpression("LettersOnly", "Any letters", @"\p{L}+"),
																		 new PredefinedRegExpression("UppercaseLetters", "Any uppercase letters", @"\p{Lu}+"),
																		 new PredefinedRegExpression("LowercaseLetters", "Any lowercase letters", @"\p{Ll}+"),															 
																		 new PredefinedRegExpression("TimeOfDay", "The 24 hour day time:\n15:25\n2:05\n03:57", @"(0?\d|1\d|2[0-3])\:[0-5]\d"),
																		 new PredefinedRegExpression("TimeOfDateWithSeconds", "The 24 hour day time with seconds:\n12:45:10\n3:00:01", @"(0?\d|1\d|2[0-3]):[0-5]\d:[0-5]\d"),
																		 new PredefinedRegExpression("TimeOfDayAMPM", "The 12 hour day time:\n1:35PM\n12:45AM", @"(0?[1-9]|1[012]):[0-5]\d(AM|PM)"),
																		 new PredefinedRegExpression("TimeOfDateWithSecondsAMPM", "The 12 hour day time with seconds:\n10:03:10AM\n03:00:01PM", @"(0?[1-9]|1[012]):[0-5]\d:[0-5]\d(AM|PM)"),
																		 new PredefinedRegExpression("Date", "The MM/dd/yy or MM/dd/yyyy date with year from 1000 to 3999:\n3/12/99\n06/25/1800", @"(0?[1-9]|1[012])/([012]?[1-9]|[123]0|31)/([123][0-9])?[0-9][0-9]"),
																		 new PredefinedRegExpression("E-mail", "E-mail adress:\n user@example.com", @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"),
																		 new PredefinedRegExpression("URL", "URL:\n http://www.example.com", @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?"),
																		 new PredefinedRegExpression("TelephoneNumber", "The telephone number with or without city code:\n(345) 234-12-07\n(210) 7-17-81\n26-32-22", @"(\(\d\d\d\) )?\d{1,3}-\d\d-\d\d"),
																		 new PredefinedRegExpression("Extension", "15450", @"\d{0,5}"),
																		 new PredefinedRegExpression("SocialSecurity", "555-55-5555", @"\d\d\d-\d\d-\d\d\d\d"),
																		 new PredefinedRegExpression("ShortZipCode", "11200", @"\d\d\d\d\d"),
																		 new PredefinedRegExpression("LongZipCode", "11200-0000", @"\d\d\d\d\d-\d\d\d\d"),
																		 new PredefinedRegExpression("DecimalNumber", "Any decimal number", @"\d+"),
																		 new PredefinedRegExpression("HexadecimalNumber", "Any hexadecimal number", @"[0-9A-Fa-f]+"),
																		 new PredefinedRegExpression("OctalNumber", "Any octal number", @"[0-7]+"),
																		 new PredefinedRegExpression("BinaryNumber", "Any binary number", @"[01]+"),
																		 new PredefinedRegExpression("YesNo", "Yes\nNo", @"Yes|No"),
																		 new PredefinedRegExpression("TrueFalse", "True\nFalse", @"True|False"),
		};
		protected bool RegExValid { get { return regEx != null; } }
		public string RegularExpression { get { return editRegExpression.Text; } }
		public RegularExpressionEditorForm(string regExpression) {
			InitializeForm();
			editRegExpression.Text = regExpression;
		}
		void UpdateListAndDescription() {
			var item = predefinedRegExpressions.FirstOrDefault(i => i.RegExpression == RegularExpression);
			if(listExpressions.SelectedItem != item)
				listExpressions.SelectedIndex = item != null ? listExpressions.Items.IndexOf(item) : -1;
			labelRegExDescription.Text = item != null ? item.FullDescription :  "Custom regular expression";
			if(!RegExValid)
				labelRegExDescription.Text = "Invalid regular expression";				
		}
		void DoExpressionChanged() {
			try {
				regEx = new Regex(RegularExpression);
			} catch {
				regEx = null;
			}
			editTest.Enabled = RegExValid;
			editRegExpression.ForeColor = RegExValid ? Color.Black : Color.Red;
			UpdateListAndDescription();
			TestInput();			
		}
		void TestInput() {
			if(RegExValid)
				editTest.ForeColor = regEx.IsMatch(editTest.Text) ? Color.Black : Color.Red;
			else
				editTest.ForeColor = Color.Gray;
		}
		void InitializeButtons() {
			btnCancel = new SimpleButton();
			btnCancel.DialogResult = DialogResult.Cancel;
			btnCancel.Location = new Point(261, 214);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new Size(75, 23);
			btnCancel.TabIndex = 37;
			btnCancel.Text = "&Cancel";
			Controls.Add(btnCancel);
			CancelButton = btnCancel;
			btnOk = new SimpleButton();
			btnOk.DialogResult = DialogResult.OK;
			btnOk.Location = new Point(181, 214);
			btnOk.Name = "btnOk";
			btnOk.Size = new Size(75, 23);
			btnOk.TabIndex = 36;
			btnOk.Text = "&OK";
			Controls.Add(btnOk);
			AcceptButton = btnOk;
		}
		void InitializeLabels() {
			labelEditTest = new Label();
			labelEditTest.ImeMode = ImeMode.NoControl;
			labelEditTest.Location = new Point(13, 166);
			labelEditTest.Name = "labelEditTest";
			labelEditTest.Size = new Size(144, 23);
			labelEditTest.TabIndex = 34;
			labelEditTest.Text = "Test input:";
			Controls.Add(labelEditTest);
			lblList = new Label();
			lblList.ImeMode = ImeMode.NoControl;
			lblList.Location = new Point(13, 54);
			lblList.Name = "lblList";
			lblList.Size = new Size(184, 23);
			lblList.TabIndex = 24;
			lblList.Text = "Predefined regular expressions:";
			Controls.Add(lblList);
			labelEditRegExpression = new Label();
			labelEditRegExpression.ImeMode = ImeMode.NoControl;
			labelEditRegExpression.Location = new Point(13, 10);
			labelEditRegExpression.Name = "labelEditRegExpression";
			labelEditRegExpression.Size = new Size(90, 23);
			labelEditRegExpression.TabIndex = 22;
			labelEditRegExpression.Text = "Edit expression:";
			Controls.Add(labelEditRegExpression);
			labelRegExDescription = new Label();
			labelRegExDescription.ImeMode = ImeMode.NoControl;
			labelRegExDescription.Location = new Point(205, 70);
			labelRegExDescription.Name = "labelRegExDescription";
			labelRegExDescription.Size = new Size(136, 88);
			labelRegExDescription.TabIndex = 26;
			Controls.Add(labelRegExDescription);
		}
		void InitEditTest() {
			editTest = new TextEdit();
			((ISupportInitialize)(editTest.Properties)).BeginInit();
			editTest.EditValue = "";
			editTest.Location = new Point(13, 182);
			editTest.Name = "editTest";
			editTest.Size = new Size(320, 20);
			editTest.TabIndex = 35;
			editTest.EditValueChanged += new EventHandler(editTest_Changed);
			Controls.Add(editTest);
			((ISupportInitialize)(editTest.Properties)).EndInit();
		}
		void InitEditRegExpression() {
			editRegExpression = new TextEdit();
			((ISupportInitialize)(editRegExpression.Properties)).BeginInit();
			editRegExpression.EditValue = "";
			editRegExpression.Location = new Point(13, 26);
			editRegExpression.Name = "editRegExpression";
			editRegExpression.Properties.EditValueChangedFiringMode = EditValueChangedFiringMode.Buffered;
			editRegExpression.Size = new Size(320, 20);
			editRegExpression.TabIndex = 23;
			editRegExpression.EditValueChanged += new EventHandler(editRegExpression_EditValueChanged);
			Controls.Add(editRegExpression);
			((ISupportInitialize)(editRegExpression.Properties)).EndInit();
		}
		void InitListExpressions() {
			listExpressions = new ListBoxControl();
			((ISupportInitialize)(listExpressions)).BeginInit();
			listExpressions.ItemHeight = 16;
			listExpressions.Location = new Point(13, 70);
			listExpressions.Name = "listExpressions";
			listExpressions.Size = new Size(184, 88);
			listExpressions.TabIndex = 25;
			listExpressions.SelectedIndexChanged += new EventHandler(listExpressions_SelectedIndexChanged);
			Controls.Add(listExpressions);
			listExpressions.Items.AddRange(predefinedRegExpressions);
			((ISupportInitialize)(listExpressions)).EndInit();
		}
		void InitializeForm() {
			SuspendLayout();
			ClientSize = new Size(354, 251);
			LookAndFeel.SetSkinStyle(DevExpress.Skins.SkinRegistrator.DesignTimeSkinName);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			Name = "RegularExpressionEditorForm";
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.CenterParent;
			Text = "Regular Expression Editor";
			InitEditRegExpression();
			InitListExpressions();
			InitEditTest();
			InitializeButtons();
			InitializeLabels();
			ResumeLayout(false);
		}
		void listExpressions_SelectedIndexChanged(object sender, EventArgs e) {
			if(listExpressions.SelectedIndex >= 0)
				editRegExpression.Text = ((PredefinedRegExpression)listExpressions.SelectedItem).RegExpression;
		}
		void editRegExpression_EditValueChanged(object sender, EventArgs e) {
			DoExpressionChanged();
		}	   
		void editTest_Changed(object sender, EventArgs e) {
			TestInput();
		}
	}
	internal class PredefinedRegExpression {
		public string ShortDescription;
		public string FullDescription;
		public string RegExpression;
		public PredefinedRegExpression(string shortDescription, string fullDescription, string regExpression) {
			ShortDescription = shortDescription;
			FullDescription = fullDescription;
			RegExpression = regExpression;
		}
		public override string ToString() {
			return ShortDescription;
		}
	}
	public class RegularExpressionEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			using(RegularExpressionEditorForm form = new RegularExpressionEditorForm(value as String)) {
				if(form.ShowDialog() == DialogResult.OK)
					return form.RegularExpression;
			}
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
	}
}
