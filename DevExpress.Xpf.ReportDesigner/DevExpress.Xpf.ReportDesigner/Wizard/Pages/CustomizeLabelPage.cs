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
using System.Linq;
using DevExpress.Data.XtraReports.Wizard;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.XtraPrinting;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportWizard.Pages {
	public class CustomizeLabelPage : ReportWizardPageBase {
		public static CustomizeLabelPage Create(ReportWizardModel model, ILabelProductRepository repository) {
			return ViewModelSource.Create(() => new CustomizeLabelPage(model, repository));
		}
		GraphicsUnit actualUnit;
		public IEnumerable<PaperKindData> PaperKinds { get; private set; }
		[BindableProperty(true)]
		public virtual PaperKindData SelectedPaperKind {
			get { return PaperKinds.SingleOrDefault(x => x.Id == ReportModel.CustomLabelInformation.PaperKindDataId); }
			set { ReportModel.CustomLabelInformation.PaperKindDataId = value.Id; }
		}
		public IEnumerable<GraphicsUnit> Units { get; private set; }
		[BindableProperty(OnPropertyChangedMethodName="OnUnitChanged")]
		public virtual GraphicsUnit Unit {
			get { return actualUnit; }
			set { actualUnit = value; }
		}
		[BindableProperty(true)]
		public virtual float Width {
			get { return ReportModel.CustomLabelInformation.Width; }
			set { ReportModel.CustomLabelInformation.Width = value; }
		}
		[BindableProperty(true)]
		public virtual float Height {
			get { return ReportModel.CustomLabelInformation.Height; }
			set { ReportModel.CustomLabelInformation.Height = value; }
		}
		[BindableProperty(true)]
		public virtual float HorizontalPitch {
			get { return ReportModel.CustomLabelInformation.HorizontalPitch; }
			set { ReportModel.CustomLabelInformation.HorizontalPitch = value; }
		}
		[BindableProperty(true)]
		public virtual float VerticalPitch {
			get { return ReportModel.CustomLabelInformation.VerticalPitch; }
			set { ReportModel.CustomLabelInformation.VerticalPitch = value; }
		}
		[BindableProperty(true)]
		public virtual float TopMargin {
			get { return ReportModel.CustomLabelInformation.TopMargin; }
			set { ReportModel.CustomLabelInformation.TopMargin = value; }
		}
		[BindableProperty(true)]
		public virtual float LeftMargin {
			get { return ReportModel.CustomLabelInformation.LeftMargin; }
			set { ReportModel.CustomLabelInformation.LeftMargin = value; }
		}
		public virtual float RightMargin {
			get { return 0; }
		}
		public virtual float BottomMargin {
			get { return 0; }
		}
		#region ctor
		protected CustomizeLabelPage(ReportWizardModel model, ILabelProductRepository repository) : base(model) {
			PaperKinds = repository.GetSortedPaperKinds().ToArray();
			SelectedPaperKind = PaperKinds.FirstOrDefault();
			Units = new[] { GraphicsUnit.Inch, GraphicsUnit.Millimeter };
			actualUnit = ReportModel.CustomLabelInformation.Unit;
		}
		#endregion
		protected void OnUnitChanged() {
			Width = GraphicsUnitConverter.Convert(Width, ReportModel.CustomLabelInformation.Unit, Unit);
			Height = GraphicsUnitConverter.Convert(Height, ReportModel.CustomLabelInformation.Unit, Unit);
			VerticalPitch = GraphicsUnitConverter.Convert(VerticalPitch, ReportModel.CustomLabelInformation.Unit, Unit);
			HorizontalPitch = GraphicsUnitConverter.Convert(HorizontalPitch, ReportModel.CustomLabelInformation.Unit, Unit);
			TopMargin = GraphicsUnitConverter.Convert(TopMargin, ReportModel.CustomLabelInformation.Unit, Unit);
			LeftMargin = GraphicsUnitConverter.Convert(LeftMargin, ReportModel.CustomLabelInformation.Unit, Unit);
			ReportModel.CustomLabelInformation.Unit = Unit;
		}
		#region overrides
		public override bool CanFinish {
			get {
				return true;
			}
		}
		public override bool CanGoForward {
			get {
				return false;
			}
		}
		protected override void NavigateToNextPage(WizardController wizardController) {
			throw new NotSupportedException();
		}
		#endregion
	}
}
