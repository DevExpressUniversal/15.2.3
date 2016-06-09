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

using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using System.Drawing;
using System;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Native.TOC {
	class XRControlFactory {
		internal float Dpi { get; set; }
		public XtraReport CreateXtraReport(object dataSource) {
			return new XtraReport() { DataSource = dataSource, Dpi = Dpi };
		}
		public DetailReportBand CreateDetailReportBand(object dataSource, string dataMember, params Band[] innerBands) {
			var detailReportBand = new DetailReportBand() { DataSource = dataSource, DataMember = dataMember, Dpi = Dpi };
			detailReportBand.ReportPrintOptions.PrintOnEmptyDataSource = false;
			detailReportBand.Bands.AddRange(innerBands);
			return detailReportBand;
		}
		public DetailBand CreateDetailBand(params XRControl[] children) {
			var detailBand = new DetailBand() { HeightF = 0f, Dpi = Dpi };
			detailBand.Controls.AddRange(children);
			foreach(XRControl item in children)
				detailBand.HeightF = Math.Max(detailBand.HeightF, item.BottomF);
			return detailBand;
		}
		public ReportHeaderBand CreateReportHeaderBand(XRLabel label) {
			var reportHeader = new ReportHeaderBand() { HeightF = label.HeightF, Dpi = Dpi };
			reportHeader.Controls.Add(label);
			return reportHeader;
		}
		public XRTableOfContentsLine XRTableOfContentsLine(object dataSource, string dataMember, float width, XRControlStyle style, PointF location, char leaderSymbol) {
			return new XRTableOfContentsLine() {
				DataSource = dataSource,
				TextDataMember = GetValidDataMember(dataMember, "Text"),
				BrickPagePairDataMember = GetValidDataMember(dataMember, "Pair"),
				WidthF = width,
				Target = "_self",
				BackColor = style.BackColor,
				BorderColor = style.BorderColor,
				BorderDashStyle = style.BorderDashStyle,
				Borders = style.Borders,
				BorderWidth = style.BorderWidth,
				Font = style.Font,
				ForeColor = style.ForeColor,
				TextAlignment = style.TextAlignment,
				Padding = style.Padding,
				LocationF = location,
				LeaderSymbol = leaderSymbol,
				Dpi = Dpi
			};
		}
		public XRLabel CreateXRLabel(string text, XRControlStyle style, float widthf, PointF location) {
			return new XRLabel() {
				Text = text,
				BackColor = style.BackColor,
				BorderColor = style.BorderColor,
				BorderDashStyle = style.BorderDashStyle,
				Borders = style.Borders,
				BorderWidth = style.BorderWidth,
				Font = style.Font,
				ForeColor = style.ForeColor,
				TextAlignment = style.TextAlignment,
				Padding = style.Padding,
				CanShrink = false,
				CanGrow = true,
				WidthF = widthf,
				LocationF = location,
				Multiline = true,
				Dpi = Dpi
			};
		}
		static string GetValidDataMember(string rootDataMember, string addition) {
			return string.IsNullOrEmpty(rootDataMember)
				? addition
				: rootDataMember + "." + addition;
		}
	}
}
