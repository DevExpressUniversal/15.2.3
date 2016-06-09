#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

using System.ComponentModel;
using DevExpress.Web;
using Constants = DevExpress.XtraReports.Web.Native.Constants.ReportViewer;
namespace DevExpress.XtraReports.Web.DocumentViewer {
	public class DocumentViewerReportViewerSettings : PropertiesBase, IPropertiesOwner {
		const string
			EnableRequestParametersName = "EnableRequestParameters",
			UseIFrameName = "UseIFrame",
			PageByPageName = "PageByPage",
			PrintUsingAdobePlugInName = "PrintUsingAdobePlugIn",
			EnableReportMarginsName = "EnableReportMargins",
			TableLayoutName = "TableLayout";
		const bool
			DefaultEnableRequestParameters = true,
			DefaultUseIFrame = true;
		const ImagesEmbeddingMode DefaultImagesEmbeddingMode = ImagesEmbeddingMode.Auto;
		public DocumentViewerReportViewerSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[AutoFormatDisable]
		[DefaultValue(DefaultEnableRequestParameters)]
		[NotifyParentProperty(true)]
		public bool EnableRequestParameters {
			get { return GetBoolProperty(EnableRequestParametersName, DefaultEnableRequestParameters); }
			set { SetBoolProperty(EnableRequestParametersName, DefaultEnableRequestParameters, value); }
		}
		[DefaultValue(DefaultUseIFrame)]
		[NotifyParentProperty(true)]
		public bool UseIFrame {
			get { return GetBoolProperty(UseIFrameName, DefaultUseIFrame); }
			set { SetBoolProperty(UseIFrameName, DefaultUseIFrame, value); }
		}
		[DefaultValue(Constants.DefaultPageByPage)]
		[NotifyParentProperty(true)]
		public bool PageByPage {
			get { return GetBoolProperty(PageByPageName, Constants.DefaultPageByPage); }
			set { SetBoolProperty(PageByPageName, Constants.DefaultPageByPage, value); }
		}
		[DefaultValue(Constants.DefaultPrintUsingAdobePlugIn)]
		[NotifyParentProperty(true)]
		public bool PrintUsingAdobePlugIn {
			get { return GetBoolProperty(PrintUsingAdobePlugInName, Constants.DefaultPrintUsingAdobePlugIn); }
			set { SetBoolProperty(PrintUsingAdobePlugInName, Constants.DefaultPrintUsingAdobePlugIn, value); }
		}
		[DefaultValue(Constants.DefaultEnableReportMargins)]
		[NotifyParentProperty(true)]
		public bool EnableReportMargins {
			get { return GetBoolProperty(EnableReportMarginsName, Constants.DefaultEnableReportMargins); }
			set { SetBoolProperty(EnableReportMarginsName, Constants.DefaultEnableReportMargins, value); }
		}
		[DefaultValue(Constants.DefaultTableLayout)]
		[NotifyParentProperty(true)]
		public bool TableLayout {
			get { return GetBoolProperty(TableLayoutName, Constants.DefaultTableLayout); }
			set { SetBoolProperty(TableLayoutName, Constants.DefaultTableLayout, value); }
		}
		[NotifyParentProperty(true)]
		[DefaultValue(DefaultImagesEmbeddingMode)]
		public ImagesEmbeddingMode ImagesEmbeddingMode {
			get { return (ImagesEmbeddingMode)GetEnumProperty(Constants.ImagesEmbeddingModeName, DefaultImagesEmbeddingMode); }
			set { SetEnumProperty(Constants.ImagesEmbeddingModeName, DefaultImagesEmbeddingMode, value); }
		}
		[NotifyParentProperty(true)]
		[DefaultValue(Constants.DefaultShouldDisposeReport)]
		public bool ShouldDisposeReport {
			get { return GetBoolProperty(Constants.ShouldDisposeReportName, Constants.DefaultShouldDisposeReport); }
			set { SetBoolProperty(Constants.ShouldDisposeReportName, Constants.DefaultShouldDisposeReport, value); }
		}
		#region IPropertiesOwner Members
		public void Changed(PropertiesBase properties) {
			base.Changed();
		}
		#endregion
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as DocumentViewerReportViewerSettings;
			if(src == null)
				return;
			EnableRequestParameters = src.EnableRequestParameters;
			UseIFrame = src.UseIFrame;
			PageByPage = src.PageByPage;
			PrintUsingAdobePlugIn = src.PrintUsingAdobePlugIn;
			TableLayout = src.TableLayout;
			ImagesEmbeddingMode = src.ImagesEmbeddingMode;
			ShouldDisposeReport = src.ShouldDisposeReport;
			EnableReportMargins = src.EnableReportMargins;
		}
	}
}
