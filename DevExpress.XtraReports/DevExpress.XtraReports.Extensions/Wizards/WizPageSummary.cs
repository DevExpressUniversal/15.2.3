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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraReports.UI;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.Wizards;
namespace DevExpress.XtraReports.Design {
	[ToolboxItem(false)]
	public class WizPageSummary : DevExpress.Utils.InteriorWizardPage {
		#region static
		static SummaryField CreateSummaryField(ArrayList row) {
			string displayFieldName = ((Label)row[0]).Text;
			string fieldName = (string)((Label)row[0]).Tag;
			SummaryField field = new SummaryField(fieldName, displayFieldName);
			field.Sum = ((CheckEdit)row[1]).Checked;
			field.Avg = ((CheckEdit)row[2]).Checked;
			field.Min = ((CheckEdit)row[3]).Checked;
			field.Max = ((CheckEdit)row[4]).Checked;
			field.Count = ((CheckEdit)row[5]).Checked;
			return field;
		}
		#endregion
		private System.ComponentModel.IContainer components = null;
		StandardReportWizard wizard;
		private SummaryPanel pnlSummary;
		ArrayList controlRows = new ArrayList();
		private DevExpress.XtraEditors.CheckEdit chkIgnoreNullValues;
		ArrayList captions = new ArrayList();
		public WizPageSummary(XRWizardRunnerBase runner)
			: this() {
			this.wizard = (StandardReportWizard)runner.Wizard;
		}
		WizPageSummary() {
			InitializeComponent();
			headerPicture.Image = ResourceImageHelper.CreateBitmapFromResources("Images.WizTopSummary.gif", typeof(LocalResFinder));
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizPageSummary));
			this.pnlSummary = new DevExpress.XtraReports.Design.SummaryPanel();
			this.chkIgnoreNullValues = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkIgnoreNullValues.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.titleLabel, "titleLabel");
			resources.ApplyResources(this.subtitleLabel, "subtitleLabel");
			resources.ApplyResources(this.pnlSummary, "pnlSummary");
			this.pnlSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlSummary.Name = "pnlSummary";
			resources.ApplyResources(this.chkIgnoreNullValues, "chkIgnoreNullValues");
			this.chkIgnoreNullValues.Name = "chkIgnoreNullValues";
			this.chkIgnoreNullValues.Properties.Appearance.Options.UseTextOptions = true;
			this.chkIgnoreNullValues.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.chkIgnoreNullValues.Properties.Caption = resources.GetString("chkIgnoreNullValues.Properties.Caption");
			this.Controls.Add(this.chkIgnoreNullValues);
			this.Controls.Add(this.pnlSummary);
			this.Name = "WizPageSummary";
			this.Controls.SetChildIndex(this.pnlSummary, 0);
			this.Controls.SetChildIndex(this.chkIgnoreNullValues, 0);
			this.Controls.SetChildIndex(this.headerPanel, 0);
			this.Controls.SetChildIndex(this.headerSeparator, 0);
			this.Controls.SetChildIndex(this.titleLabel, 0);
			this.Controls.SetChildIndex(this.subtitleLabel, 0);
			this.Controls.SetChildIndex(this.headerPicture, 0);
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkIgnoreNullValues.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		ArrayList CreateControlRow(string displayFieldName, string fieldName) {
			ArrayList row = new ArrayList();
			Label label = new Label();
			label.AutoSize = true;
			label.Text = fieldName;
			label.Tag = fieldName;
			row.Add(label);
			int count = captions.Count - 1;
			for (int i = 0; i < count; i++) {
				CheckEdit chk = new CheckEdit();
				chk.Properties.AutoHeight = true;
				chk.Properties.AutoWidth = true;
				chk.Text = "";
				row.Add(chk);
			}
			return row;
		}
		int CalcMaxFieldWidth() {
			int width = 0;
			int count = controlRows.Count;
			for (int i = 0; i < count; i++) {
				ArrayList row = (ArrayList)controlRows[i];
				Label label = (Label)row[0];
				width = Math.Max(label.Width, width);
			}
			return width;
		}
		int LayoutRow(int xOffset, int xCheckBoxOffset, int y, ArrayList row) {
			int maxHeight = 0;
			Label label = (Label)row[0];
			label.AutoSize = false;
			label.Location = new Point(xOffset, y);
			pnlSummary.Controls.Add(label);
			maxHeight = Math.Max(maxHeight, label.Height);
			int count = row.Count;
			for (int i = 1; i < count; i++) {
				CheckEdit chk = (CheckEdit)row[i];
				Label caption = (Label)captions[i];
				maxHeight = Math.Max(maxHeight, chk.Height);
				int x = chk.Width > caption.Width ? caption.Location.X : caption.Location.X + (caption.Width - chk.Width) / 2;
				chk.Location = new Point(x, y + (maxHeight - chk.Height) / 2);
				pnlSummary.Controls.Add(chk);
			}
			label.Width = xCheckBoxOffset - xOffset;
			label.Height = maxHeight;
			label.TextAlign = ContentAlignment.MiddleLeft;
			return maxHeight;
		}
		void CreateCaptionControls() {
			string[] captionStrings = pnlSummary.GetCaptionStrings();
			for (int i = 0; i < captionStrings.Length; i++) {
				Label label = new Label();
				label.AutoSize = true;
				label.Text = captionStrings[i];
				label.Font = new Font(label.Font, label.Font.Style | FontStyle.Bold);
				captions.Add(label);
			}
		}
		int LayoutCaptions(int xOffset, int xCheckBoxOffset, int y, int spacing) {
			int count = captions.Count;
			int maxHeight = 0;
			for (int i = 1; i < count; i++) {
				Label label = (Label)captions[i];
				maxHeight = Math.Max(maxHeight, label.Height);
			}
			Label fieldCaption = (Label)captions[0];
			maxHeight = Math.Max(maxHeight, fieldCaption.Height);
			pnlSummary.Controls.Add(fieldCaption);
			int width = fieldCaption.Width;
			fieldCaption.Location = new Point(xOffset, y);
			fieldCaption.Height = maxHeight;
			fieldCaption.Width = width;
			fieldCaption.TextAlign = ContentAlignment.MiddleLeft;
			int x = xCheckBoxOffset;
			for (int i = 1; i < count; i++) {
				Label caption = (Label)captions[i];
				pnlSummary.Controls.Add(caption);
				caption.Location = new Point(x, y);
				caption.Height = maxHeight;
				caption.TextAlign = ContentAlignment.MiddleLeft;
				x += caption.Width + spacing;
			}
			Panel panel = new Panel();
			panel.Location = new Point(xOffset, y + maxHeight + spacing / 2);
			panel.Size = new Size(x - xOffset - spacing, 1);
			panel.BorderStyle = BorderStyle.FixedSingle;
			pnlSummary.Controls.Add(panel);
			return maxHeight + 2 * spacing;
		}
		void LayoutRows() {
			int maxFieldWidth = Math.Max(CalcMaxFieldWidth(), ((Label)captions[0]).Width);
			int offset = 10;
			int spacing = 5;
			int checkBoxOffset = offset + maxFieldWidth + 2 * spacing;
			int count = controlRows.Count;
			int y = offset;
			y += LayoutCaptions(offset, checkBoxOffset, y, spacing);
			for (int i = 0; i < count; i++)
				y += LayoutRow(offset, checkBoxOffset, y, (ArrayList)controlRows[i]) + spacing;
		}
		void ClearControls() {
			Native.NativeMethods.ClearUnvalidatedControl(this);
			controlRows.Clear();
			captions.Clear();
			pnlSummary.Controls.Clear();
		}
		protected override bool OnSetActive() {
			if (wizard.SummaryFields.Count <= 0) {
				ClearControls();
				CreateCaptionControls();
				ObjectNameCollection numericalFields = wizard.GetFieldsForSummary();
				int count = numericalFields.Count;
				for (int i = 0; i < count; i++)
					controlRows.Add(CreateControlRow(numericalFields[i].DisplayName, numericalFields[i].Name));
				LayoutRows();
			}
			return true;
		}
		protected override void UpdateWizardButtons() {
		}
		void ApplyChanges() {
			wizard.SummaryFields.Clear();
			int count = controlRows.Count;
			for (int i = 0; i < count; i++) {
				SummaryField field = CreateSummaryField((ArrayList)controlRows[i]);
				if (field.IsActive)
					wizard.SummaryFields.Add(field);
			}
			wizard.IgnoreNullValuesForSummary = chkIgnoreNullValues.Checked;
		}
		void RollbackChanges() {
			wizard.SummaryFields.Clear();
		}
		protected override string OnWizardNext() {
			ApplyChanges();
			return "WizPageGroupedLayout";
		}
		protected override string OnWizardBack() {
			RollbackChanges();
			return "WizPageGrouping";
		}
		protected override bool OnWizardFinish() {
			ApplyChanges();
			return true;
		}
	}
	[ToolboxItem(false)]
	public class SummaryPanel : System.Windows.Forms.Panel {
		string srFieldName = "Field Name";
		string srSum = SummaryFunc.Sum.ToString();
		string srAvg = SummaryFunc.Avg.ToString();
		string srMin = SummaryFunc.Min.ToString();
		string srMax = SummaryFunc.Max.ToString();
		string srCount = SummaryFunc.Count.ToString();
		[Localizable(true), Category("String Resources")]
		public virtual string SRFieldName { get { return srFieldName; } set { srFieldName = value; } }
		[Localizable(true), Category("String Resources")]
		public virtual string SRSum { get { return srSum; } set { srSum = value; } }
		[Localizable(true), Category("String Resources")]
		public virtual string SRAvg { get { return srAvg; } set { srAvg = value; } }
		[Localizable(true), Category("String Resources")]
		public virtual string SRMin { get { return srMin; } set { srMin = value; } }
		[Localizable(true), Category("String Resources")]
		public virtual string SRMax { get { return srMax; } set { srMax = value; } }
		[Localizable(true), Category("String Resources")]
		public virtual string SRCount { get { return srCount; } set { srCount = value; } }
		public string[] GetCaptionStrings() {
			return new string[] { srFieldName, srSum, srAvg, srMin, srMax, srCount };
		}
	}
}
