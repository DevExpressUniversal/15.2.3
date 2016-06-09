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

namespace DevExpress.XtraReports.Web.Native {
	public static class WebResourceNames {
		internal const string
			WebRootResourcePath = "DevExpress.XtraReports.Web.",
			WebScriptResourcePath = WebRootResourcePath + "Scripts.",
			WebCssResourcePath = WebRootResourcePath + "Css.",
			WebHtmlResourcePath = WebRootResourcePath + "Html.",
			WebImagesResourcePath = WebRootResourcePath + "Images.";
		#region DevExtreme
		internal const string DXTremeCssPostfix =
#if DEBUG
 ".css";
#else
 ".min.css";
#endif
		internal const string DXTremeJSPostfix =
#if DEBUG
 ".js";
#else
 ".min.js";
#endif
		#endregion
		public static class DocumentMap {
			public const string ScriptResourceName = WebScriptResourcePath + "ReportDocumentMap.js";
		}
		public static class DocumentViewer {
			public const string ScriptResourceName = WebScriptResourcePath + "DocumentViewer.js";
		}
		internal static class WebClientUIControl {
			internal const string
				ScriptResourceName = WebScriptResourcePath + "WebClientUIControl.js",
				DXScriptResourceName = WebScriptResourcePath + "dx-designer" + DXTremeJSPostfix,
				CssResourceName = WebCssResourcePath + "WebClientUIControl.css";
		}
		internal static class Frameworks {
			const string
				WebFrameworkScriptResourcePath = WebScriptResourcePath + "Frameworks.",
				AceSnippetsScriptResourcePath = WebFrameworkScriptResourcePath + "snippets.";
			internal const string
				AceScriptResourceName = WebFrameworkScriptResourcePath + "ace.js",
				AceExtLanguageToolsScriptResourceName = WebFrameworkScriptResourcePath + "ext-language_tools.js",
				AceModeCSharpScriptResourceName = WebFrameworkScriptResourcePath + "mode-csharp.js",
				AceModeVBScriptResourceName = WebFrameworkScriptResourcePath + "mode-vbscript.js",
				AceThemeAmbianceScriptResourceName = WebFrameworkScriptResourcePath + "theme-ambiance.js",
				AceThemeDreamweaverScriptResourceName = WebFrameworkScriptResourcePath + "theme-dreamweaver.js",
				AceSnippetTextScriptResourceName = AceSnippetsScriptResourcePath + "text.js",
				AceSnippetCSharpScriptResourceName = AceSnippetsScriptResourcePath + "csharp.js",
				AceSnippetVBScriptScriptResourceName = AceSnippetsScriptResourcePath + "vbscript.js";
		}
	}
}
