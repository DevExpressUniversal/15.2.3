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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Data.XtraReports.Wizard.Views;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.DataAccess.UI.Wizard.Views;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.XtraReports.Wizards3.Localization;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Wizards3.Views {
	[ToolboxItem(false)]
	partial class ChooseReportStylePageView : WizardViewBase, IChooseReportStylePageView  {
		public ReportStyleId ReportStyleId {
			get {
				return (ReportStyleId)reportStyleGroup.SelectedIndex;
				throw new Exception(ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_ReportStyle_UnexpectedBehavior));
			}
			set {
				reportStyleGroup.SelectedIndex = (int)value;
			}
		}
		public override string HeaderDescription {
			get {
				return ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_ReportStyle_Description);
			}
		}
		public ChooseReportStylePageView() {
			InitializeComponent();
			InitializeImages();
		}
		void InitializeImages() {
			this.panel1.BackgroundImage = ResourceImageHelper.CreateImageFromResources(LocalResFinder.GetFullName("Wizards3.Images.ReportStylePage.png"), LocalResFinder.Assembly);
		}
		private void reportStyleGroup_SelectedIndexChanged(object sender, EventArgs e) {
			switch(ReportStyleId) {
				case ReportStyleId.Bold:
					sampleTitleLabel.Font = new Font("Times New Roman", 20F, FontStyle.Bold);
					sampleTitleLabel.ForeColor = Color.FromArgb(255, 128, 0, 0);
					sampleCaptionLabel.Font = new Font("Arial", 10F);
					sampleCaptionLabel.ForeColor = Color.FromArgb(255, 128, 0, 0);
					sampleDataLabel.Font = new Font("Times New Roman", 10f);
					sampleDataLabel.ForeColor = Color.FromArgb(255, 0, 0, 0);
					break;
				case ReportStyleId.Casual:
					sampleTitleLabel.Font = new Font("Tahoma", 24f);
					sampleTitleLabel.ForeColor = Color.FromArgb(255, 0, 128, 128);
					sampleCaptionLabel.Font = new Font("Arial", 10F);
					sampleCaptionLabel.ForeColor = Color.FromArgb(255, 0, 0, 0);
					sampleDataLabel.Font = new Font("Arial", 10f);
					sampleDataLabel.ForeColor = Color.FromArgb(255, 0, 0, 0);
					break;
				case ReportStyleId.Compact:
					sampleTitleLabel.Font = new Font("Times New Roman", 21F);
					sampleTitleLabel.ForeColor = Color.FromArgb(255, 0, 0, 0);
					sampleCaptionLabel.Font = new Font("Times New Roman", 10F);
					sampleCaptionLabel.ForeColor = Color.FromArgb(255, 0, 0, 0);
					sampleDataLabel.Font = new Font("Arial", 9F);
					sampleDataLabel.ForeColor = Color.FromArgb(255, 0, 0, 0);
					break;
				case ReportStyleId.Corporate:
					sampleTitleLabel.Font = new Font("Times New Roman", 20f, (FontStyle)(FontStyle.Bold | FontStyle.Italic));
					sampleTitleLabel.ForeColor = Color.FromArgb(255, 0, 0, 128);
					sampleCaptionLabel.Font = new Font("Times New Roman", 11f, (FontStyle)(FontStyle.Bold | FontStyle.Italic));
					sampleCaptionLabel.ForeColor = Color.FromArgb(255, 0, 0, 128);
					sampleDataLabel.Font = new Font("Arial", 8f);
					sampleDataLabel.ForeColor = Color.FromArgb(255, 0, 0, 0);
					break;
				case ReportStyleId.Formal:
					sampleTitleLabel.Font = new Font("Times New Roman", 24f, FontStyle.Bold);
					sampleTitleLabel.ForeColor = Color.FromArgb(255, 0, 0, 0);
					sampleCaptionLabel.Font = new Font("Times New Roman", 10f, FontStyle.Bold);
					sampleCaptionLabel.ForeColor = Color.FromArgb(255, 0, 0, 0);
					sampleDataLabel.Font = new Font("Times New Roman", 18f);
					sampleDataLabel.ForeColor = Color.FromArgb(255, 0, 0, 0);
					break;
				default:
					throw new ArgumentNullException();
			}
		}
	}
}
