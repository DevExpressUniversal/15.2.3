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
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using DevExpress.Data.XtraReports.Labels;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Wizards.Builder {
	public class WizardReportBuilder {
		public static IComponentFactory CreateDefaultComponentFactory() {
			var nameCreationService = new XRNameCreationService(null);
			return new WizardComponentFactory(nameCreationService);
		}
		readonly Func<IComponentFactory> createComponentFactory;
		readonly bool shouldInitReport;
		public WizardReportBuilder()
			: this(CreateDefaultComponentFactory, true) {
		}
		public WizardReportBuilder(Func<IComponentFactory> createComponentFactory, bool shouldInitReport) {
			this.createComponentFactory = createComponentFactory;
			this.shouldInitReport = shouldInitReport;
		}
		public void Build(XtraReport report, ReportModel model) {
			if(!shouldInitReport) {
				BuildCore(report, model);
				return;
			}
			report.BeginInit();
			try {
				BuildCore(report, model);
			} finally {
				report.EndInit();
			}
		}
		void BuildCore(XtraReport report, ReportModel model) {
			switch(model.ReportType) {
				case ReportType.Label:
					BuildLabelReport(report, model);
					break;
				case ReportType.Standard:
					BuildStandardReport(report, model);
					break;
				case ReportType.Empty:
					BuildEmptyReport(report);
					break;
				default:
					throw new ArgumentException("Unexpected report type: " + model.ReportType, "model.ReportType");
			}
		}
		#region ReportType.Label
		protected virtual void BuildLabelReport(XtraReport report, ReportModel model) {
			BeforeBuildLabelReport(report);
			var labelInfo = LabelInfoFactory.Create(model);
			var labelRepository = new LabelProductRepositoryFactory().Create();
			var paperKindItems = labelRepository
				.GetSortedPaperKinds()
				.Select(ConvertToPaperKindItem);
			report.ReportUnit = labelInfo.Unit == GraphicsUnit.Millimeter ? ReportUnit.TenthsOfAMillimeter : ReportUnit.HundredthsOfAnInch;
			var paperKindList = new PaperKindList(report.Dpi);
			foreach(var item in paperKindItems) {
				paperKindList.Add(item);
			}
			paperKindList.CurrentID = model.CustomLabelInformation.PaperKindDataId;
			IComponentFactory componentFactory = createComponentFactory();
			using(componentFactory as IDisposable) {
				var reportBuilder = new LabelReportBuilder(report, componentFactory, labelInfo, paperKindList);
				reportBuilder.Execute();
			}
		}
		protected virtual void BeforeBuildLabelReport(XtraReport report) {
		}
		static PaperKindItem ConvertToPaperKindItem(PaperKindData paperKindData) {
			float dpi = DevExpress.XtraPrinting.GraphicsDpi.UnitToDpi(paperKindData.Unit);
			 var paperKind = (PaperKind)paperKindData.EnumId;
			 SizeF size = paperKind == PaperKind.Custom
				 ? new SizeF(paperKindData.Width, paperKindData.Height)
				 : PageSizeInfo.GetPageSizeF(paperKind, dpi, PageSizeInfo.DefaultSize);
			 return new PaperKindItem(paperKindData.Name, size, paperKindData.Id, paperKind, dpi, paperKindData.IsRollPaper);
		}
		#endregion
		#region ReportType.Standard
		void BuildStandardReport(XtraReport report, ReportModel model) {
			BeforeBuildStandardReport(report, model);
			var reportInfo = ReportInfoFactory.Create(model, report.Dpi);
			IComponentFactory componentFactory = createComponentFactory();
			using(componentFactory as IDisposable) {
				var reportBuilder = new ReportBuilder(report, componentFactory, reportInfo);
				reportBuilder.Execute();
			}
			AfterBuildStandardReport(report);
		}
		protected virtual void BeforeBuildStandardReport(XtraReport report, ReportModel model) {
		}
		protected virtual void AfterBuildStandardReport(XtraReport report) {
		}
		#endregion
		#region ReportType.Empty
		protected virtual void BuildEmptyReport(XtraReport report) {
		}
		#endregion
	}
}
