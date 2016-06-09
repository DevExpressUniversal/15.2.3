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

using DevExpress.Mvvm.POCO;
using System;
using DevExpress.Mvvm;
using DevExpress.Data.XtraReports.Labels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.DataAnnotations;
using System.ComponentModel;
using DevExpress.Data.XtraReports.Wizard;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportWizard.Pages {
	public class SelectLabelTypePage : ReportWizardPageBase {
		public static SelectLabelTypePage Create(ReportWizardModel model) {
			return ViewModelSource.Create(() => new SelectLabelTypePage(model));
		}
		const string onSelectedLabelChangedMethod = "OnSelectedLabelProductChanged";
		readonly ILabelProductRepository labelsRepository;
		public IEnumerable<LabelProduct> LabelProducts { get; private set; }
		[BindableProperty(OnPropertyChangedMethodName = onSelectedLabelChangedMethod)]
		public virtual LabelProduct SelectedLabelProduct {
			get { return LabelProducts.SingleOrDefault(x => x.Id == ReportModel.LabelProductId); }
			set { ReportModel.LabelProductId = value.Id; }
		}
		public virtual IEnumerable<LabelDetails> LabelDetails { get; set; }
		[BindableProperty(OnPropertyChangedMethodName = "OnSelectedLabelDetailChanged")]
		public virtual LabelDetails SelectedLabelDetail {
			get { return LabelDetails.SingleOrDefault(x => x.Id == ReportModel.LabelProductDetailId) ?? null; }
			set { ReportModel.LabelProductDetailId = value != null ? value.Id : 0; }
		}
		[BindableProperty(true)]
		public virtual PaperKindData PaperKindData { get; protected set; }
		#region ctor
		protected SelectLabelTypePage(ReportWizardModel model) : base(model) {
			this.labelsRepository = new LabelProductRepositoryFactory().Create();
			LabelProducts = labelsRepository.GetSortedProducts().ToArray();
			SelectedLabelProduct = LabelProducts.SingleOrDefault(x=> x.Id == ReportModel.LabelProductId) ?? LabelProducts.FirstOrDefault();
			LabelDetails = SelectedLabelProduct.Return(x => labelsRepository.GetSortedProductDetails(x.Id), () => Enumerable.Empty<LabelDetails>()).ToArray();
			SelectedLabelDetail = LabelDetails.SingleOrDefault(x=> x.Id == ReportModel.LabelProductDetailId) ?? LabelDetails.FirstOrDefault();
			PaperKindData = labelsRepository.GetPaperKindData(SelectedLabelDetail.PaperKindId);
		}
		#endregion
		#region overrides
		public override bool CanFinish {
			get {
				return false;
			}
		}
		public override bool CanGoForward {
			get {
				return true;
			}
		}
		protected override void NavigateToNextPage(WizardController wizardController) {
			wizardController.NavigateTo(CustomizeLabelPage.Create(ReportWizardModel, labelsRepository));
		}
		protected override void OnGoForward(CancelEventArgs e) {
			base.OnGoForward(e);
			ReportModel.CustomLabelInformation = new CustomLabelInformation() {
				Width = SelectedLabelDetail.Width,
				Height = SelectedLabelDetail.Height,
				VerticalPitch = SelectedLabelDetail.VPitch,
				HorizontalPitch = SelectedLabelDetail.HPitch,
				TopMargin = SelectedLabelDetail.TopMargin,
				LeftMargin = SelectedLabelDetail.LeftMargin,
				PaperKindDataId = SelectedLabelDetail.PaperKindId,
				Unit = SelectedLabelDetail.Unit
			};
		}
		#endregion
		protected virtual void OnSelectedLabelProductChanged() {
			LabelDetails = SelectedLabelProduct.Return(x => labelsRepository.GetSortedProductDetails(x.Id), () => null).ToArray();
			SelectedLabelDetail = LabelDetails.FirstOrDefault();
		}
		protected void OnSelectedLabelDetailChanged() {
			PaperKindData = labelsRepository.GetPaperKindData(SelectedLabelDetail.PaperKindId);
		}
	}
}
