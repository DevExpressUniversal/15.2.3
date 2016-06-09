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
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
namespace DevExpress.Web.Internal {
	public enum MvcRenderMode { None, Render, RenderWithSimpleIDs, RenderResources }
	public enum MvcAssemblyVersion { 
		[Description("4")]
		Default,
		[Description("5")]
		Mvc5 
	}
	public delegate string MvcClientUrlResolver(string relativeUrl);
	public static class MvcUtils {
		public const string CallbackNameParam = "DXCallbackName";
		public const string CallbackArgumentParam = "DXCallbackArgument";
		public const string StateKey = "DXMvcState";
		public const string RenderScriptsCalledKey = "DXMvcRenderScriptsCalled";
		public const string MvcUrlResolutionServiceKey = "DXMvcUrlResolutionService";
		public const string ModelBinderProcessingKey = "DXMvcModelBinderProcessing";
		public const string AspSignature = ".ASPx";
		public const string MvcSignature = ".MVCx";
		public const string MvcTagPrefix = "dxmvc";
		static MvcRenderMode renderMode = MvcRenderMode.None;
		static bool renderScriptsCalled = false;
		static bool modelBinderProcessing = false;
		public static bool IsCallback() {
			return !string.IsNullOrEmpty(CallbackName);
		}
		public static string CallbackName {
			get {
				if(HttpContext.Current != null && HttpContext.Current.Request != null)
					return HttpUtils.GetValueFromRequest(CallbackNameParam);
				return string.Empty;
			}
		}
		public static string CallbackArgument {
			get {
				if(HttpContext.Current != null && HttpContext.Current.Request != null)
					return HttpUtils.GetValueFromRequest(CallbackArgumentParam, true);
				return string.Empty;
			}
		}
		public static MvcRenderMode RenderMode {
			get {
				if(HttpContext.Current != null)
					return HttpUtils.GetContextValue<MvcRenderMode>(StateKey, MvcRenderMode.None);
				return renderMode;
			}
			set {
				if(HttpContext.Current != null)
					HttpUtils.SetContextValue<MvcRenderMode>(StateKey, value);
				else
					renderMode = value;
			}
		}
		public static bool RenderScriptsCalled {
			get {
				if(HttpContext.Current != null)
					return HttpUtils.GetContextValue<bool>(RenderScriptsCalledKey, false);
				return renderScriptsCalled;
			}
			set {
				if(HttpContext.Current != null)
					HttpUtils.SetContextValue<bool>(RenderScriptsCalledKey, value);
				else
					renderScriptsCalled = value;
			}
		}
		public static bool ModelBinderProcessing {
			get {
				if(HttpContext.Current != null)
					return HttpUtils.GetContextValue<bool>(ModelBinderProcessingKey, false);
				return modelBinderProcessing;
			}
			set {
				if(HttpContext.Current != null)
					HttpUtils.SetContextValue<bool>(ModelBinderProcessingKey, value);
				else
					modelBinderProcessing = value;
			}
		}
		public static string ResolveClientUrl(string relativeUrl) {
			if(string.IsNullOrEmpty(relativeUrl))
				return string.Empty;
			IUrlResolutionService service = MvcUrlResolutionService;
			return service != null
				? service.ResolveClientUrl(relativeUrl)
				: relativeUrl;
		}
		public static IUrlResolutionService MvcUrlResolutionService {
			get { return HttpUtils.GetContextValue<IUrlResolutionService>(MvcUrlResolutionServiceKey, null); }
			set { HttpUtils.SetContextValue<IUrlResolutionService>(MvcUrlResolutionServiceKey, value); }
		}
		public static string GetMvcSkinContent(string aspSkinContent) {
			string content = MvcUtils.AddMvcTagPrefix(aspSkinContent);
			content = MvcUtils.ReplaceAspTagsWithMvc(content, string.Empty);
			content = MvcUtils.ReplaceAspTagsWithMvc(content, "WebChartControl", "MVCxChartControl");
			content = MvcUtils.ReplaceAspTagsWithMvc(content, "ReportToolbar", "MVCxReportToolbar");
			content = MvcUtils.ReplaceAspTagsWithMvc(content, "ReportViewer", "MVCxReportViewer");
			return content;
		}
		public static string AddMvcTagPrefix(string text) {
			return AddMvcTagPrefix(text, "DevExpress.Web.Mvc");
		}
		public static string AddMvcTagPrefix(string text, string ns) {
			return AddMvcTagPrefix(text, ns, ResourceManager.MvcAssemblyVersion == MvcAssemblyVersion.Default ? AssemblyInfo.SRAssemblyMVC : AssemblyInfo.SRAssemblyMVC5, AssemblyInfo.Version);
		}
		public static string AddMvcTagPrefix(string text, string ns, string assembly, string version) {
			string MVCTagPrefixDirective = "<%@ Register TagPrefix=\"" + MvcTagPrefix + "\" Namespace=\"" + ns + "\" " +
				"Assembly=\"" + assembly + ", " +
				"Version=" + version + ", Culture=neutral, " +
				"PublicKeyToken=" + AspxCodeUtils.GetPublicKeyToken(typeof(ASPxWebControl)) + "\" %>";
			Regex regex = new Regex("<%@ Register .*? %>", RegexOptions.Singleline | RegexOptions.Multiline);
			MatchCollection matches = regex.Matches(text);
			if(matches.Count == 0)
				return MVCTagPrefixDirective + Environment.NewLine + text;
			else {
				Match lastMatch = matches[matches.Count - 1];
				return text.Insert(lastMatch.Index + lastMatch.Length, Environment.NewLine + MVCTagPrefixDirective);
			}
		}
		public static string ReplaceAspTagsWithMvc(string text, string tagsRoot) {
			return ReplaceAspTagsWithMvc(text, "ASPx" + tagsRoot, "MVCx" + tagsRoot);
		}
		public static string ReplaceAspTagsWithMvc(string text, string originalControlName, string replaceControlName) {
			Regex regex = new Regex("<(/?)\\w+:" + originalControlName, RegexOptions.Singleline | RegexOptions.Multiline);
			MatchCollection matches = regex.Matches(text);
			foreach(Match match in matches) {
				string slashStr = match.Groups[1].ToString();
				text = text.Replace(match.Value, "<" + slashStr + MvcTagPrefix + ":" + replaceControlName);
			}
			return text;
		}
	}
}
