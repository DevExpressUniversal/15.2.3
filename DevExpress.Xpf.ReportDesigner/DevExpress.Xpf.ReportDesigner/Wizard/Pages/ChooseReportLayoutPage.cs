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

using System.Collections.Generic;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Data.XtraReports.Wizard;
using System.Windows.Media.Imaging;
using System.Reflection;
using DevExpress.Xpf.Reports.UserDesigner.ReportWizard.Pages.Common;
using DevExpress.Utils;
using System;
using DevExpress.Xpf.DataAccess.DataSourceWizard;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportWizard.Pages {
	public class ChooseReportLayoutPage : ReportWizardPageBase {
		public static ChooseReportLayoutPage Create(ReportWizardModel model) {
			return ViewModelSource.Create(() => new ChooseReportLayoutPage(model));
		}
		public IEnumerable<ReportLayoutViewModel> Layouts { get; private set; }
		[BindableProperty(true)]
		public virtual ReportLayout SelectedReportLayout {
			get { return ReportModel.Layout; }
			set { ReportModel.Layout = value; }
		}
		readonly Lazy<IEnumerable<BooleanViewModel>> orientations = BooleanViewModel.CreateList("Portrait", "Landscape", true);
		public IEnumerable<BooleanViewModel> Orientations { get { return orientations.Value; } }
		[BindableProperty(true)]
		public virtual bool Portrait {
			get { return ReportModel.Portrait; }
			set { ReportModel.Portrait = value; }
		}
		#region ctor
		public ChooseReportLayoutPage(ReportWizardModel model) : base(model) {
			FillLayouts();
		}
		#endregion
		void FillLayouts() {
			var assembly = Assembly.GetExecutingAssembly();
			Layouts = ReportModel.IsGrouped()
				? new ReportLayoutViewModel[] {
					new ReportLayoutViewModel(ReportLayout.Stepped, new BitmapImage(AssemblyHelper.GetResourceUri(assembly, @"\Images\Wizard\Stepped.png")), "Stepped"),
					new ReportLayoutViewModel(ReportLayout.Outline1, new BitmapImage(AssemblyHelper.GetResourceUri(assembly, @"\Images\Wizard\Outline1.png")), "Outline1"),
					new ReportLayoutViewModel(ReportLayout.Outline2, new BitmapImage(AssemblyHelper.GetResourceUri(assembly, @"\Images\Wizard\Outline2.png")), "Outline2"),
					new ReportLayoutViewModel(ReportLayout.AlignLeft1, new BitmapImage(AssemblyHelper.GetResourceUri(assembly, @"\Images\Wizard\AlignLeft1.png")), "AlignLeft1"),
					new ReportLayoutViewModel(ReportLayout.AlignLeft2, new BitmapImage(AssemblyHelper.GetResourceUri(assembly, @"\Images\Wizard\AlignLeft2.png")), "AlignLeft2")}
				: new ReportLayoutViewModel[] {
					new ReportLayoutViewModel(ReportLayout.Columnar, new BitmapImage(AssemblyHelper.GetResourceUri(assembly, @"\Images\Wizard\Columnar.png")), "Columnar"),
					new ReportLayoutViewModel(ReportLayout.Tabular, new BitmapImage(AssemblyHelper.GetResourceUri(assembly, @"\Images\Wizard\Tabular.png")), "Tabular"),
					new ReportLayoutViewModel(ReportLayout.Justified, new BitmapImage(AssemblyHelper.GetResourceUri(assembly, @"\Images\Wizard\Justified.png")), "Justified")};
		}
		#region overrides
		public override bool CanFinish {
			get { return true; }
		}
		public override bool CanGoForward {
			get { return true; }
		}
		protected override void NavigateToNextPage(WizardController wizardController) {
			wizardController.NavigateTo(ChooseReportStylePage.Create(ReportWizardModel));
		}
		#endregion
	}
}
