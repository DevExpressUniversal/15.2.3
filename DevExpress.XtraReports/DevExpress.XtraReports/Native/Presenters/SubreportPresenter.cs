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
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Native.Printing;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Localization;
using DevExpress.Utils;
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native.LayoutView;
namespace DevExpress.XtraReports.Native.Presenters {
	class SubreportPresenter : ControlPresenter {
		protected XRSubreport Subreport {
			get { return (XRSubreport)control; }
		}
		public SubreportPresenter(XRSubreport subreport) : base(subreport) {
		}
		public override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return new SubreportBrick(Subreport);
		}
		public override void BeforeReportPrint() {
			if(Subreport.ReportSource != null)
				Subreport.ReportSource.PrintingSystem.ClearContent();
		}
	}
	class DesignSubreportPresenter : SubreportPresenter {
		public DesignSubreportPresenter(XRSubreport subreport) : base(subreport) {
		}
		public override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return new TextBrick();
		}
		public override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			TextBrick textBrick = brick as TextBrick;
			if(textBrick != null) {
				SetBrickAppearance(textBrick);
				if(!string.IsNullOrEmpty(Subreport.Name))
					textBrick.Text += string.Format(ReportLocalizer.GetString(ReportStringId.XRSubreport_NameInfo), Subreport.Name);
				SetReportSourceInfo(textBrick);
				SetReportSourceUrlInfo(textBrick);
			}
		}
		void SetReportSourceUrlInfo(TextBrick textBrick) {
			if(TypeDescriptor.GetProperties(Subreport)["ReportSourceUrl"] != null) {
				string url = string.IsNullOrEmpty(Subreport.ReportSourceUrl) ?
					ReportLocalizer.GetString(ReportStringId.UD_Title_FieldList_NonePickerNodeText) :
					Subreport.ReportSourceUrl;
				textBrick.Text += string.Format(ReportLocalizer.GetString(ReportStringId.XRSubreport_ReportSourceUrlInfo), url);
			}
		}
		void SetReportSourceInfo(TextBrick textBrick) {
			if(TypeDescriptor.GetProperties(Subreport)["ReportSource"] != null) {
				string reportSourceName = Subreport.ReportSource == null ?
					ReportLocalizer.GetString(ReportStringId.XRSubreport_NullReportSourceInfo) :
					Subreport.ReportSource.GetType().Name;
				textBrick.Text += string.Format(ReportLocalizer.GetString(ReportStringId.XRSubreport_ReportSourceInfo), reportSourceName);
			}
		}
		static void SetBrickAppearance(TextBrick textBrick) {
			if(textBrick.Style is XRControlStyle)
				((XRControlStyle)textBrick.Style).ResetFont();
			BrickStyle style = new BrickStyle(textBrick.Style);
			LayoutViewAppearance.ApplyContour(style);
			style.Padding = new PaddingInfo(2, 2, 2, 2, GraphicsDpi.Pixel);
			style.BackColor = Color.FromArgb(120, Color.LightGray);
			style.StringFormat = new BrickStringFormat(style.StringFormat, StringTrimming.EllipsisCharacter);
			style.SetAlignment(HorzAlignment.Center, VertAlignment.Center);
			textBrick.Style = style;
		}
	}
}
