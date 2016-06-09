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
using System.Text;
using DevExpress.XtraReports.UI;
using System.Text.RegularExpressions;
using System.Security.Permissions;
using DevExpress.Data.Helpers;
using System.Collections;
namespace DevExpress.XtraReports.Native {
	public abstract class ReportConverter {
		protected XtraReport report;
		protected ReportConverter(XtraReport report) {
			this.report = report;
		}
		public abstract void Convert();
	}
	public class ReportConverter_v8_1 : ReportConverter {
		public ReportConverter_v8_1(XtraReport report)
			: base(report) {
		}
		public override void Convert() {
			if(SecurityHelper.IsPermissionGranted(new ReflectionPermission(Enum.IsDefined(typeof(ReflectionPermissionFlag), ReflectionPermissionFlag.RestrictedMemberAccess) ? ReflectionPermissionFlag.RestrictedMemberAccess : ReflectionPermissionFlag.MemberAccess))) {
				IEnumerable<XRControlStyle> instanceStyles = Native.XRAccessor.GetFieldValues(report, typeof(XRControlStyle)).Cast<XRControlStyle>();
				instanceStyles.ApplyStyleUsings();
				instanceStyles.ClearDirty();
			}
			report.StyleContainer.ApplyStyleUsings();
		}
	}
	public class ReportConverter_v9_1 : ReportConverter {
		public ReportConverter_v9_1(XtraReport report)
			: base(report) {
		}
		public override void Convert() {
			if(report.ShouldSerializeBackColor())
				report.PageColor = report.BackColor;
		}
	}
	public class ReportConverter_v9_3 : ReportConverter {
		public ReportConverter_v9_3(XtraReport report)
			: base(report) {
		}
		public override void Convert() {
			foreach(CalculatedField calculatedField in report.CalculatedFields)
				calculatedField.Scripts.ConvertScripts(report);
		}
	}
	public class ReportConverter_v10_2 : ReportConverter {
		public ReportConverter_v10_2(XtraReport report)
			: base(report) {
		}
		public override void Convert() {
			report.IterateReportsRecursive(delegate(XtraReportBase item) {
#pragma warning disable 0618
				DetailBand band = item.Bands[BandKind.Detail] as DetailBand;
				if(band != null && band.RepeatCountOnEmptyDataSource >= 0)
					item.ReportPrintOptions.DetailCountOnEmptyDataSource = band.RepeatCountOnEmptyDataSource;
				if(item is XtraReport)
					item.ReportPrintOptions.EnsureDetailCount(((XtraReport)item).PreviewRowCount);
#pragma warning restore 0618
			});
		}
	}
	public class ReportConverter_v12_2 : ReportConverter {
		public static string ConvertSource(string source) {
			if(source.Contains("DevExpress.XtraPivotGrid.PivotGridControl"))
				return source;
			string s = Regex.Replace(source, @"DevExpress\.XtraPivotGrid\.(?<args>\w+EventArgs)", new MatchEvaluator(m => {
				return "DevExpress.XtraReports.UI.PivotGrid." + m.Groups["args"];
			}), RegexOptions.IgnoreCase);
			s = Regex.Replace(s, @"DevExpress.XtraPivotGrid.PivotGridField", "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridField", RegexOptions.IgnoreCase);
			s = Regex.Replace(s, @"DevExpress.XtraPivotGrid.PivotGridCustomTotal", "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridCustomTotal", RegexOptions.IgnoreCase);
			return s;
		}
		public ReportConverter_v12_2(XtraReport report) : base(report) {
		}
		public override void Convert() {
			if(!string.IsNullOrEmpty(report.ScriptsSource))
				report.ScriptsSource = ConvertSource(report.ScriptsSource);
		}
	}
}
