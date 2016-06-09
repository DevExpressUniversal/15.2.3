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
using System.Linq;
using DevExpress.Data.WizardFramework;
using DevExpress.Data.XtraReports.Labels;
using DevExpress.Data.XtraReports.Wizard.Views;
using DevExpress.Utils;
namespace DevExpress.Data.XtraReports.Wizard.Presenters {
	public class SelectLabelTypePage<TModel> : WizardPageBase<ISelectLabelTypePageView, TModel> where TModel : ReportModel {
		readonly ILabelProductRepository repository;
		public override bool MoveNextEnabled { get { return true; } }
		public override bool FinishEnabled { get { return true; } }
		public SelectLabelTypePage(ISelectLabelTypePageView view, ILabelProductRepository repository)
			: base(view) {
			Guard.ArgumentNotNull(repository, "repository");
			this.repository = repository;
		}
		public override void Begin() {
			var products = repository.GetSortedProducts();
			View.FillLabelProducts(products);
			var selectedProduct = products.FirstOrDefault(p => p.Id == Model.LabelProductId);
			if(selectedProduct == null) {
				selectedProduct = products.First();
			}
			View.SelectedProduct = selectedProduct;
			var details = repository.GetSortedProductDetails(selectedProduct.Id);
			View.FillLabelDetails(details);
			var selectedDetails = details.FirstOrDefault(d => d.Id == Model.LabelProductDetailId);
			if(selectedDetails == null) {
				selectedDetails = details.First();
			}
			View.SelectedDetails = selectedDetails;
			View.CurrentPaperKindData = repository.GetPaperKindData(selectedDetails.PaperKindId);
			View.UpdateLabelInformation();
			View.SelectedLabelProductChanged += View_SelectedLabelProductChanged;
			View.SelectedLabelProductDetailsChanged += View_SelectedLabelProductDetailsChanged;
		}
		public override void Commit() {
			View.SelectedLabelProductChanged -= View_SelectedLabelProductChanged;
			View.SelectedLabelProductDetailsChanged -= View_SelectedLabelProductDetailsChanged;
			Model.LabelProductId = View.SelectedProduct.Id;
			Model.LabelProductDetailId = View.SelectedDetails.Id;
			Model.CustomLabelInformation = new CustomLabelInformation()
			{
				Width = View.SelectedDetails.Width,
				Height = View.SelectedDetails.Height,
				VerticalPitch = View.SelectedDetails.VPitch,
				HorizontalPitch = View.SelectedDetails.HPitch,
				TopMargin = View.SelectedDetails.TopMargin,
				LeftMargin = View.SelectedDetails.LeftMargin,
				PaperKindDataId = View.SelectedDetails.PaperKindId,
				Unit = View.SelectedDetails.Unit
			};
		}
		void View_SelectedLabelProductChanged(object sender, EventArgs e) {
			var details = repository.GetSortedProductDetails(View.SelectedProduct.Id);
			View.FillLabelDetails(details);
			View.SelectedDetails = details.First();
			View.CurrentPaperKindData = repository.GetPaperKindData(View.SelectedDetails.PaperKindId);
			View.UpdateLabelInformation();
		}
		void View_SelectedLabelProductDetailsChanged(object sender, EventArgs e) {
			View.CurrentPaperKindData = repository.GetPaperKindData(View.SelectedDetails.PaperKindId);
			View.UpdateLabelInformation();
		}
		public override Type GetNextPageType() {
			return typeof(CustomizeLabelPage<TModel>);
		}
	}
}
