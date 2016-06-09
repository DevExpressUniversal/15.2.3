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
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Wizards.Builder {
	public static class ReportInfoFactory {
		public static ReportInfo Create(ReportModel model, float reportDpi) {
			var reportInfo = new ReportInfo {
				Layout = model.Layout,
				Style = GetReportStyle(model.ReportStyleId),
				Spacing = XRConvert.Convert(6, GraphicsDpi.Pixel, reportDpi),
				Orientation = model.Portrait
					? PageOrientation.Portrait
					: PageOrientation.Landscape,
				IgnoreNullValuesForSummary = model.IgnoreNullValuesForSummary,
				ReportTitle = model.ReportTitle,
				FitFieldsToPage = model.AdjustFieldWidth
			};
			model.Columns
				.Select(CreateObjectName)
				.ForEach(i => reportInfo.SelectedFields.Add(i));
			foreach(var groupingLevel in model.GroupingLevels) {
				var groupingCollection = new ObjectNameCollection();
				groupingLevel
					.Select(CreateObjectName)
					.ForEach(i => groupingCollection.Add(i));
				reportInfo.GroupingFieldsSet.Add(groupingCollection);
			}
			foreach(var summaryOptions in model.SummaryOptions) {
				var options = new SummaryOptions(summaryOptions.Flags);
				var summaryField = new SummaryField(summaryOptions.ColumnName, summaryOptions.ColumnName) {
					Sum = options.Sum,
					Avg = options.Avg,
					Min = options.Min,
					Max = options.Max,
					Count = options.Count
				};  
				reportInfo.SummaryFields.Add(summaryField);
			}
			return reportInfo;
		}
		static ReportStyle GetReportStyle(ReportStyleId styleId) {
			switch(styleId) {
				case ReportStyleId.Bold:
					return new ReportStyle("Bold", "Wizards.Bold.repss", typeof(ResFinder));
				case ReportStyleId.Casual:
					return new ReportStyle("Casual", "Wizards.Casual.repss", typeof(ResFinder));
				case ReportStyleId.Compact:
					return new ReportStyle("Compact", "Wizards.Compact.repss", typeof(ResFinder));
				case ReportStyleId.Corporate:
					return new ReportStyle("Corporate", "Wizards.Corporate.repss", typeof(ResFinder));
				case ReportStyleId.Formal:
					return new ReportStyle("Formal", "Wizards.Formal.repss", typeof(ResFinder));
				default:
					throw new ArgumentException("Unexpected ReportStyleId: " + styleId, "styleId");
			}
		}
		static ObjectName CreateObjectName(string columnName) {
			return new ObjectName(columnName, columnName);
		}
	}
}
