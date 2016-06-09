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
using DevExpress.Data.XtraReports.ReportGeneration;
using DevExpress.Export.Xl;
using DevExpress.Utils;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraReports.ReportGeneration;
namespace DevExpress.XtraReports.UI {
	public static class ReportGenerationExtensions<TCol, TRow> where TRow : IRowBase where TCol : IColumn {
		public static void Generate(XtraReport report, IGridViewFactory<TCol, TRow> viewFactory){
			GenerateReportCore(report, viewFactory, null);
		}
		public static void Generate(XtraReport report, IGridViewFactory<TCol, TRow> viewFactory, ReportGenerationOptions options){
			GenerateReportCore(report, viewFactory, options);
		}
		static void GenerateReportCore(XtraReport report, IGridViewFactory<TCol, TRow> viewFactory, ReportGenerationOptions options){
			if(viewFactory == null) return;
			IGridView<TCol, TRow> view = viewFactory.GetIViewImplementerInstance();
			if(view == null) return;
			report.Bands.Clear();
			report.FilterString = view.FilterString;
			AssignReportDataSource(report, viewFactory);
			ReportGenerationModel<TCol, TRow> model = new ReportGenerationModel<TCol, TRow>(view, options);
			ILayoutContextStrategy layoutStrategy = new GridViewLayoutStrategy<TCol, TRow>(report, model);
			ReportLayoutContext<TCol, TRow> context = new ReportLayoutContext<TCol, TRow>(layoutStrategy);
			context.Execute();
			XRBuilder<TCol, TRow> builder = new XRBuilder<TCol, TRow>(layoutStrategy.Report, model);
			XRBuilderDirector director = new XRBuilderDirector(builder);
			director.Costruct();
			viewFactory.ReleaseIViewImplementerInstance(view);
		}
		static void AssignReportDataSource(XtraReport report, IGridViewFactory<TCol, TRow> viewFactory){
			report.DataSource = viewFactory.GetDataSource();
			report.DataMember = viewFactory.GetDataMember();
		}
	}
}
