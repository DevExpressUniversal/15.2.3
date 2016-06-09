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

using System.Linq;
using DevExpress.Data.WizardFramework;
using DevExpress.Data.XtraReports.Labels;
using DevExpress.Data.XtraReports.Wizard.Views;
using DevExpress.Utils;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.XtraPrinting;
using System;
namespace DevExpress.Data.XtraReports.Wizard.Presenters {
	public class CustomizeLabelPage<TModel> : WizardPageBase<ICustomizeLabelPageView, TModel> where TModel : ReportModel {
		readonly ILabelProductRepository repository;
		GraphicsUnit currentGraphicsUnit;
		public override bool MoveNextEnabled { get { return false; } }
		public override bool FinishEnabled { get { return true; } }
		public CustomizeLabelPage(ICustomizeLabelPageView view, ILabelProductRepository repository)
			: base(view) {
			Guard.ArgumentNotNull(repository, "repository");
			this.repository = repository;
		}
		public override void Begin() {
			View.FillPageSizeList(GetPageSizeList());
			currentGraphicsUnit = Model.CustomLabelInformation.Unit;
			UpdateBaseValues();
			UpdatePaperKindText();
			UpdateLabelsCountText();
			UpdateRightBottomMargins();
			View.UnitChanged += View_UnitChanged;
			View.SelectedPaperKindChanged += View_SelectedPaperKindChanged;
			View.LabelInformationChanged += View_LabelInformationChanged;
		}
		void View_UnitChanged(object sender, EventArgs e) {
			if(View.Unit != currentGraphicsUnit) {
				View.Width = GraphicsUnitConverter.Convert(View.Width, currentGraphicsUnit, View.Unit);
				View.Height = GraphicsUnitConverter.Convert(View.Height, currentGraphicsUnit, View.Unit);
				View.VerticalPitch = GraphicsUnitConverter.Convert(View.VerticalPitch, currentGraphicsUnit, View.Unit);
				View.HorizontalPitch = GraphicsUnitConverter.Convert(View.HorizontalPitch, currentGraphicsUnit, View.Unit);
				View.TopMargin = GraphicsUnitConverter.Convert(View.TopMargin, currentGraphicsUnit, View.Unit);
				View.LeftMargin = GraphicsUnitConverter.Convert(View.LeftMargin, currentGraphicsUnit, View.Unit);
				UpdatePaperKindText();
				currentGraphicsUnit = View.Unit;
			}
		}
		void View_SelectedPaperKindChanged(object sender, EventArgs e) {
			UpdatePaperKindText();
			UpdateLabelsCountText();
			UpdateRightBottomMargins();
		}
		void View_LabelInformationChanged(object sender, EventArgs e) {
			UpdateLabelsCountText();
			UpdateRightBottomMargins();
		}
		void UpdateBaseValues() {
			View.SelectedPaperKindId = Model.CustomLabelInformation.PaperKindDataId;
			View.Width = Model.CustomLabelInformation.Width;
			View.Height = Model.CustomLabelInformation.Height;
			View.VerticalPitch = Model.CustomLabelInformation.VerticalPitch;
			View.HorizontalPitch = Model.CustomLabelInformation.HorizontalPitch;
			View.TopMargin = Model.CustomLabelInformation.TopMargin;
			View.LeftMargin = Model.CustomLabelInformation.LeftMargin;
			View.Unit = Model.CustomLabelInformation.Unit;
		}
		void UpdatePaperKindText() {
			View.PaperKindFormattedText = LabelWizardHelper.GetPaperKindFormattedString(repository.GetPaperKindData(View.SelectedPaperKindId), View.Unit);
		}
		void UpdateLabelsCountText() {
			PaperKindData currentPaperKindData = repository.GetPaperKindData(View.SelectedPaperKindId);
			View.LabelsCountHorz = LabelWizardHelper.GetLabelsCount(View.HorizontalPitch, View.Width, View.LeftMargin, View.Unit, currentPaperKindData.Width, currentPaperKindData.Unit);
			View.LabelsCountVert = LabelWizardHelper.GetLabelsCount(View.VerticalPitch, View.Height, View.TopMargin, View.Unit, currentPaperKindData.Height, currentPaperKindData.Unit);
		}
		void UpdateRightBottomMargins() {
			PaperKindData currentPaperKindData = repository.GetPaperKindData(View.SelectedPaperKindId);
			View.RightMargin = LabelWizardHelper.GetOtherMarginValue(View.HorizontalPitch, View.Width, View.LeftMargin, View.Unit, currentPaperKindData.Width, currentPaperKindData.Unit);
			View.BottomMargin = LabelWizardHelper.GetOtherMarginValue(View.VerticalPitch, View.Height, View.TopMargin, View.Unit, currentPaperKindData.Height, currentPaperKindData.Unit);
		}
		List<PaperKindViewInfo> GetPageSizeList() {
			IEnumerable<PaperKindData> paperKinds = repository.GetSortedPaperKinds();
			List<PaperKindViewInfo> list = new List<PaperKindViewInfo>();
			foreach(PaperKindData paperKind in paperKinds) {
				list.Add(new PaperKindViewInfo() {
					Id = paperKind.Id,
					DisplayName = paperKind.Name,
					SizeText = LabelWizardHelper.GetPaperKindFormattedString(paperKind, paperKind.Unit)
				});
			} 
			return list;
		}
		public override void Commit() {
			View.UnitChanged -= View_UnitChanged;
			View.SelectedPaperKindChanged -= View_SelectedPaperKindChanged;
			View.LabelInformationChanged -= View_LabelInformationChanged;
			Model.CustomLabelInformation.PaperKindDataId = View.SelectedPaperKindId;
			Model.CustomLabelInformation.Width = View.Width;
			Model.CustomLabelInformation.Height = View.Height;
			Model.CustomLabelInformation.VerticalPitch = View.VerticalPitch;
			Model.CustomLabelInformation.HorizontalPitch = View.HorizontalPitch;
			Model.CustomLabelInformation.TopMargin = View.TopMargin;
			Model.CustomLabelInformation.LeftMargin = View.LeftMargin;
			Model.CustomLabelInformation.Unit = View.Unit;
		}
	}
}
