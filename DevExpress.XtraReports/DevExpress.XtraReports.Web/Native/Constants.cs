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

using System.Web.UI.WebControls;
using DevExpress.XtraReports.Web.DocumentViewer;
using DevExpress.XtraReports.Web.WebDocumentViewer;
namespace DevExpress.XtraReports.Web.Native {
	public static class Constants {
		public static class ReportViewer {
			public const string
				ClientParametersKey = "parameters",
				ClientDrillDownKey = "drillDown",
				SubmitParametersCallbackName = "submitParameters",
				EnableReportMarginsName = "EnableReportMargins",
				ImagesEmbeddingModeName = "ImagesEmbeddingMode",
				ShouldDisposeReportName = "ShouldDisposeReport",
				ReportViewerIDPropertyName = "ReportViewerID";
			public const bool
				DefaultPageByPage = true,
				DefaultPrintUsingAdobePlugIn = true,
				DefaultEnableReportMargins = false,
				DefaultAutoSize = true,
				DefaultTableLayout = true,
				DefaultShouldDisposeReport = true;
			public const ImagesEmbeddingMode DefaultImagesEmbeddingMode = ImagesEmbeddingMode.Url;
		}
		public static class DocumentViewer {
			public const DocumentViewerToolbarMode ToolbarModeDefault = DocumentViewerToolbarMode.StandardToolbar;
			public const bool AutoHeightDefault = true;
		}
		public static class ReportDesigner {
			public const bool
				ShouldDisposeReportDefault = true,
				ShouldDisposeDataSourcesDefault = true,
				ShouldShareReportDataSourcesDefault = true,
				TryAddDefaultDataSerializerDefault = true;
			public static readonly Unit
				WidthDefault = Unit.Percentage(100),
				HeightDefault = Unit.Pixel(850);
		}
		public static class WebDocumentViewer {
			public const ReportSourceKind ReportSourceKindDefault = ReportSourceKind.ReportType;
			public static readonly Unit
				DefaultWidth = Unit.Percentage(100),
				DefaultHeight = Unit.Pixel(1100);
		}
		public static class QueryBuilder {
			public static readonly Unit
				DefaultWidth = Unit.Percentage(100),
				DefaultHeight = Unit.Pixel(850);
		}
	}
}
