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
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.Data.XtraReports.Wizard.Views;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.XtraReports.Wizards3.Localization;
using DevExpress.Utils;
using DevExpress.DataAccess.UI.Wizard.Views;
using System.Drawing;
namespace DevExpress.XtraReports.Wizards3.Views {
	[System.ComponentModel.ToolboxItem(false)]
	public partial class CustomizeLabelPageView : WizardViewBase, ICustomizeLabelPageView {
		public string LabelsOnPage { get { return ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_LabelOptions_LabelsOnPage); } }
		int labelsCountHorz, labelsCountVert;
		public CustomizeLabelPageView() {
			InitializeComponent();
			InitializeImages();
		}
		void InitializeImages() {
			this.reportImageEdit.Image = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.labelReportPage.png"), LocalResFinder.Assembly);
		}
		public XtraLayout.LayoutControl BaseLayout {
			get { return layoutControl; }
		}
		public override string HeaderDescription {
			get { return ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_LabelOptions_Description); }
		}
		public event EventHandler SelectedPaperKindChanged;
		public event EventHandler LabelInformationChanged;
		public event EventHandler UnitChanged;
		#region ICustomizeLabelPageView Members
		public void FillPageSizeList(IEnumerable<PaperKindViewInfo> paperKinds) {
			paperKindList.Properties.DataSource = paperKinds;
			paperKindList.Properties.DisplayMember = "DisplayName";
			paperKindList.Properties.ValueMember = "Id";
		}
		public string PaperKindFormattedText {
			set { paperKindTextLabel.Text = value; }
		}
		public new float Width {
			get { return Decimal.ToSingle(widthEdit.Value); }
			set { widthEdit.EditValue = value; }
		}
		public new float Height {
			get { return Decimal.ToSingle(heightEdit.Value); }
			set { heightEdit.EditValue = value; }
		}
		public float HorizontalPitch {
			get { return Decimal.ToSingle(horizontalPitchEdit.Value); }
			set { horizontalPitchEdit.EditValue = value; }
		}
		public float VerticalPitch {
			get { return Decimal.ToSingle(verticalPitchEdit.Value); }
			set { verticalPitchEdit.EditValue = value; }
		}
		public int SelectedPaperKindId {
			get { return (int)paperKindList.EditValue; }
			set { paperKindList.EditValue = value; }
		}
		public float TopMargin {
			get { return Decimal.ToSingle(topMarginEdit.Value); }
			set { topMarginEdit.EditValue = value; }
		}
		public float LeftMargin {
			get { return Decimal.ToSingle(leftMarginEdit.Value); }
			set { leftMarginEdit.EditValue = value; }
		}
		public float RightMargin {
			set { rightMarginEdit.EditValue = value; }
		}
		public float BottomMargin {
			set { bottomMarginEdit.EditValue = value; }
		}
		public int LabelsCountHorz {
			set { 
				labelsCountHorz = value;
				UpdateLabelsCountText();
			}
		}
		public int LabelsCountVert {
			set { 
				labelsCountVert = value;
				UpdateLabelsCountText();
			}
		}
		public GraphicsUnit Unit {
			get { return rbMillimeter.Checked ? GraphicsUnit.Millimeter : GraphicsUnit.Inch; }
			set {
				if(value == GraphicsUnit.Millimeter)
					rbMillimeter.Checked = true;
				else
					rbInch.Checked = true;
			}
		}
		#endregion
		private void rb_CheckedChanged(object sender, EventArgs e) {
			UpdateSpinEdits();
			if(UnitChanged != null) {
				UnitChanged(this, EventArgs.Empty);
			}
		}
		private void paperKindList_EditValueChanged(object sender, EventArgs e) {
			UpdateLookUpToolTip();
			if(SelectedPaperKindChanged != null) {
				SelectedPaperKindChanged(this, EventArgs.Empty);
			}
		}
		private void spinEdit_EditValueChanged(object sender, EventArgs e) {
			if(LabelInformationChanged != null) {
				LabelInformationChanged(this, EventArgs.Empty);
			}
		}
		void UpdateLabelsCountText() {
			labelsCountTextLabel.Text = string.Format("{0} {1}{2} {3} x {4}", labelsCountHorz * labelsCountVert, LabelsOnPage,
				System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator, labelsCountHorz, labelsCountVert);
		}
		void UpdateSpinEdits() {
			decimal actualIncrementValue = rbInch.Checked ? 0.01M : 0.1M;
			string actualMask = rbInch.Checked ? "##0.00" : "##0.0";
			widthEdit.Properties.Increment = heightEdit.Properties.Increment = verticalPitchEdit.Properties.Increment = horizontalPitchEdit.Properties.Increment =
				topMarginEdit.Properties.Increment = leftMarginEdit.Properties.Increment = rightMarginEdit.Properties.Increment = bottomMarginEdit.Properties.Increment = 
				actualIncrementValue;
			widthEdit.Properties.EditMask = heightEdit.Properties.EditMask = verticalPitchEdit.Properties.EditMask = horizontalPitchEdit.Properties.EditMask =
				topMarginEdit.Properties.EditMask = leftMarginEdit.Properties.EditMask = rightMarginEdit.Properties.EditMask = bottomMarginEdit.Properties.EditMask = 
				actualMask;
		}
		void UpdateLookUpToolTip() {
			paperKindList.ToolTip = paperKindList.Properties.GetDisplayText(paperKindList.EditValue);
		}	  
	}
}
