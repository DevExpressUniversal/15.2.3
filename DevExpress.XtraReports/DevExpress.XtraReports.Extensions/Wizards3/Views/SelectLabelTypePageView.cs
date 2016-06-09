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
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data.XtraReports.Labels;
using DevExpress.Data.XtraReports.Wizard.Views;
using DevExpress.DataAccess.UI.Wizard.Views;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.XtraReports.Wizards3.Localization;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Wizards3.Views {
	[ToolboxItem(false)]
	public partial class SelectLabelTypePageView : WizardViewBase, ISelectLabelTypePageView {
		public override string HeaderDescription { get { return ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_LabelInformation_Description); } }
		public event EventHandler SelectedLabelProductChanged;
		public event EventHandler SelectedLabelProductDetailsChanged;
		PaperKindData currentPaperKindData;
		public XtraLayout.LayoutControl BaseLayout {
			get { return layoutControl; }
		}
		public PaperKindData CurrentPaperKindData {
			get { return currentPaperKindData; }
			set { currentPaperKindData = value; }
		}
		public LabelDetails SelectedDetails {
			get {
				return (LabelDetails)detailsList.GetSelectedDataRow();
			}
			set {
				detailsList.EditValue = value.Id;
				detailsList.Properties.ForceInitialize();
			}
		}
		public LabelProduct SelectedProduct {
			get {
				return (LabelProduct)productList.GetSelectedDataRow();
			}
			set {
				productList.EditValue = value.Id;
				detailsList.Properties.ForceInitialize();
			}
		}
		public SelectLabelTypePageView() {
			InitializeComponent();
		}
		public void FillLabelDetails(IEnumerable<LabelDetails> details) {
			detailsList.Properties.DataSource = details.ToArray();
		}
		public void FillLabelProducts(IEnumerable<LabelProduct> products) {
			productList.Properties.DataSource = products.ToArray();
		}
		public void UpdateLabelInformation() {
			widthValueLabel.Text = LabelWizardHelper.GetFormattedValueInUnits(SelectedDetails.Width, SelectedDetails.Unit);
			heightValueLabel.Text = LabelWizardHelper.GetFormattedValueInUnits(SelectedDetails.Height, SelectedDetails.Unit);
			paperTypeTextLabel.Text = currentPaperKindData.Name;
			paperSizeTextLabel.Text = LabelWizardHelper.GetPaperKindFormattedString(currentPaperKindData, SelectedDetails.Unit);
		}
		private void productList_EditValueChanged(object sender, EventArgs e) {
			if(SelectedLabelProductChanged != null) {
				SelectedLabelProductChanged(this, EventArgs.Empty);
			}
		}
		private void detailsList_EditValueChanged(object sender, EventArgs e) {
			if(SelectedLabelProductDetailsChanged != null) {
				SelectedLabelProductDetailsChanged(this, EventArgs.Empty);
			}
		}
	}
}
