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

using System.Web.UI;
using DevExpress.Web;
using DevExpress.XtraReports.Web;
using DevExpress.XtraReports.Web.Native;
using DevExpress.XtraReports.Web.Native.DocumentViewer;
using DevExpress.XtraReports.Web.ReportDesigner.Native;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native;
using DevExpress.XtraReports.Web.QueryBuilder.Native;
[assembly: WebResource(WebResourceNames.WebImagesResourcePath + ImagesBase.SpriteImageName + ".png", "image/png")]
[assembly: WebResource(DocumentViewerImages.DesignTimeSpriteImageResourceName, "image/png")]
[assembly: WebResource(WebResourceNames.WebImagesResourcePath + "pageTopBorder.png", "image/png")]
[assembly: WebResource(WebResourceNames.WebImagesResourcePath + "pageBottomBorder.png", "image/png")]
[assembly: WebResource(WebResourceNames.WebImagesResourcePath + "pageLeftBorder.png", "image/png")]
[assembly: WebResource(WebResourceNames.WebImagesResourcePath + "pageRightBorder.png", "image/png")]
[assembly: WebResource(ReportDesignerDesignModeImages.ImageResourceName, "image/png")]
[assembly: WebResource(WebDocumentViewerDesignModeImages.ImageResourceName, "image/png")]
[assembly: WebResource(QueryBuilderDesignModeImages.ImageResourceName, "image/png")]
[assembly: WebResource(ReportViewer.ScriptResourceName, "text/javascript")]
[assembly: WebResource(ReportViewer.OperaScriptResourceName, "text/javascript")]
[assembly: WebResource(ReportViewer.PrintHelperScriptResourceName, "text/javascript")]
[assembly: WebResource(ReportViewer.SearcherScriptResourceName, "text/javascript")]
[assembly: WebResource(ReportToolbar.ScriptResourceName, "text/javascript")]
[assembly: WebResource(WebResourceNames.DocumentMap.ScriptResourceName, "text/javascript")]
[assembly: WebResource(ReportParametersPanel.ScriptResourceName, "text/javascript")]
[assembly: WebResource(WebResourceNames.DocumentViewer.ScriptResourceName, "text/javascript")]
[assembly: WebResource(WebResourceNames.WebClientUIControl.ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxReportDesigner.DXScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxReportDesigner.ScriptResourceName, "text/javascript", PerformSubstitution = true)]
[assembly: WebResource(ASPxWebDocumentViewer.DXScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxWebDocumentViewer.ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxQueryBuilder.DXScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxQueryBuilder.ScriptResourceName, "text/javascript")]
[assembly: WebResource(WebResourceNames.WebClientUIControl.DXScriptResourceName, "text/javascript")]
[assembly: WebResource(WebResourceNames.Frameworks.AceScriptResourceName, "text/javascript")]
[assembly: WebResource(WebResourceNames.Frameworks.AceExtLanguageToolsScriptResourceName, "text/javascript")]
[assembly: WebResource(WebResourceNames.Frameworks.AceModeCSharpScriptResourceName, "text/javascript")]
[assembly: WebResource(WebResourceNames.Frameworks.AceModeVBScriptResourceName, "text/javascript")]
[assembly: WebResource(WebResourceNames.Frameworks.AceThemeAmbianceScriptResourceName, "text/javascript")]
[assembly: WebResource(WebResourceNames.Frameworks.AceThemeDreamweaverScriptResourceName, "text/javascript")]
[assembly: WebResource(WebResourceNames.Frameworks.AceSnippetTextScriptResourceName, "text/javascript")]
[assembly: WebResource(WebResourceNames.Frameworks.AceSnippetCSharpScriptResourceName, "text/javascript")]
[assembly: WebResource(WebResourceNames.Frameworks.AceSnippetVBScriptScriptResourceName, "text/javascript")]
[assembly: WebResource(ReportToolbar.SpriteCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ReportToolbar.CssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ReportParametersPanel.SystemCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxDocumentViewer.SpriteCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxDocumentViewer.CssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxDocumentViewer.SystemCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(WebResourceNames.WebClientUIControl.CssResourceName, "text/css")]
[assembly: WebResource(ASPxReportDesigner.LightCssResourceName, "text/css")]
[assembly: WebResource(ASPxReportDesigner.LightCompactCssResourceName, "text/css")]
[assembly: WebResource(ASPxReportDesigner.DarkCssResourceName, "text/css")]
[assembly: WebResource(ASPxReportDesigner.DarkCompactCssResourceName, "text/css")]
[assembly: WebResource(ASPxWebDocumentViewer.LightCssResourceName, "text/css")]
[assembly: WebResource(ASPxWebDocumentViewer.LightCompactCssResourceName, "text/css")]
[assembly: WebResource(ASPxWebDocumentViewer.DarkCssResourceName, "text/css")]
[assembly: WebResource(ASPxWebDocumentViewer.DarkCompactCssResourceName, "text/css")]
[assembly: WebResource(ASPxQueryBuilder.LightCssResourceName, "text/css")]
[assembly: WebResource(ASPxQueryBuilder.LightCompactCssResourceName, "text/css")]
[assembly: WebResource(ASPxQueryBuilder.DarkCssResourceName, "text/css")]
[assembly: WebResource(ASPxQueryBuilder.DarkCompactCssResourceName, "text/css")]
