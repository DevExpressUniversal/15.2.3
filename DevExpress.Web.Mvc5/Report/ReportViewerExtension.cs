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
using System.Web;
using System.Web.Mvc;
using DevExpress.Web;
using DevExpress.XtraReports.UI;
namespace DevExpress.Web.Mvc {
	public class ReportViewerExtension : ExtensionBase {
		#region static
		public static FileResult ExportTo(XtraReport report) {
			return DocumentViewerExtension.ExportTo(report);
		}
		public static FileResult ExportTo(XtraReport report, HttpContextBase httpContext) {
			return DocumentViewerExtension.ExportTo(report, httpContext);
		}
		public static FileResult ExportTo(XtraReport report, HttpRequestBase request) {
			return DocumentViewerExtension.ExportTo(report, request);
		}
		#endregion
		public new ReportViewerSettings Settings {
			get { return (ReportViewerSettings)base.Settings; }
		}
		protected internal new MVCxReportViewer Control {
			get { return (MVCxReportViewer)base.Control; }
		}
		public ReportViewerExtension(SettingsBase settings)
			: base(settings) {
		}
		public ReportViewerExtension(SettingsBase settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxReportViewer();
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.PrepareControl();
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.Report = Settings.Report;
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.ExportRouteValues = Settings.ExportRouteValues;
			Control.EnableReportMargins = Settings.EnableReportMargins;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			Control.Images.CopyFrom(Settings.Images);
			Control.SearchDialogEditorsImages.CopyFrom(Settings.SearchDialogEditorsImages);
			Control.SearchDialogFormImages.CopyFrom(Settings.SearchDialogFormImages);
			Control.SearchDialogEditorsStyles.CopyFrom(Settings.SearchDialogEditorsStyles);
			Control.SearchDialogButtonStyles.CopyFrom(Settings.SearchDialogButtonStyles);
			Control.SearchDialogFormStyles.CopyFrom(Settings.SearchDialogFormStyles);
			Control.PageByPage = Settings.PageByPage;
			Control.PrintUsingAdobePlugIn = Settings.PrintUsingAdobePlugIn;
			Control.AutoSize = Settings.AutoSize;
			Control.TableLayout = Settings.TableLayout;
			Control.ImagesEmbeddingMode = Settings.ImagesEmbeddingMode;
			Control.ShouldDisposeReport = Settings.ShouldDisposeReport;
			Control.Paddings.CopyFrom(Settings.Paddings);
			Control.CacheReportDocument += Settings.CacheReportDocument;
			Control.RestoreReportDocumentFromCache += Settings.RestoreReportDocumentFromCache;
			Control.DeserializeClientParameters += Settings.DeserializeClientParameters;
		}
	}
}
