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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Internal {
	public enum CodeLanguage { cs, vb, delphi, cbuilder, c, vb6, js, vbs, html, xml, sql, aspx, xaml, unknown };
	public class CodeRegionInfo {
		private string codeString;
		private CodeLanguage codeLanguage = CodeLanguage.unknown;
		public CodeRegionInfo() {
		}
		public CodeRegionInfo(string codeString, CodeLanguage codeLanguage) {
			this.codeString = codeString;
			this.codeLanguage = codeLanguage;
		}
		public string CodeString { get { return codeString; } set { codeString = value; } }
		public CodeLanguage CodeLanguage { get { return codeLanguage; } set { codeLanguage = value; } }
	}
	public class RegionPosition {
		public int EndPos;
		public int CorrectedEndPos;
	}
	public class CodeRender {
		private const string BeginAspxCodeRegionString = "<%-- CODE_BEGIN --%>";
		private const string EndAspxCodeRegionString = "<%-- CODE_END --%>";
		private const string BeginJSCodeRegionString = "<%-- SKIPJSCODE_BEGIN --%>";
		private const string EndJSCodeRegionString = "<%-- SKIPJSCODE_END --%>";
		private const string AspxCodeRegionRegExString = @"<%-- ([CODE_BEGIN+\\s|CODE_END+\\s])* --%>";
		private const string JSInAspxCodeRegionRegExString = @"<%-- ([SKIPJSCODE_BEGIN+\\s|SKIPJSCODE_END+\\s])* --%>";
		private const string JSCodeRegionRegExString = @"(<script[^>]*>[\s\n]*<!--[\s.]*|<script[^>]*>[\s\n]*//\s*<!\[?CDATA\[[\s.]*|[.\s]*//-->[\s\n]*</script>|[.\s]*//\s*]]>[\s\n]*</script>)";
		private const string JSCodeRegionRegExStringWithoutComment = "(<script[^>]*>[\\s.]*|[.\\s]*</script>)";
		private const string JSCodeRegionRegExStringInBindingExpression = @"(<%\#.*<script[^>]*>[\\s.]*|[.\\s]*</script>.*%>)";
		private const string BeginJStCodeTag = "<script type=\"text/javascript\">";
		private const string EndJStCodeTag = "</script>";
		private const string PreControlString = "<pre class=\"{0}\">{1}</pre>";
		private Regex AspxCodeRegionRegEx = null;
		private Regex JSCodeRegionRegEx = null;
		public CodeRender() {
		}
		protected virtual bool ShowJsContentAlways {
			get {
				return false;
			}
		}
		public Control GetDescriptionTextControl(string text) {
			WebControl div = CreateWebControl(HtmlTextWriterTag.Div);
			div.CssClass = "cr-div-description";
			div.Controls.Add(new LiteralControl(text));
			return div;
		}
		public Control GetHTMLFormattedControl(string fileName, string content) {
			return GetHTMLFormattedUnknownFileControl(fileName, content);
		}
		public Control GetHTMLFormattedUnknownFileControl(string filePath) {
			return GetHTMLFormattedUnknownFileControl(filePath, "");
		}
		public Control GetHTMLFormattedUnknownFileControl(string filePath, string content) {
			int extensionIndex = filePath.LastIndexOf('.');
			string extension = String.Empty;
			if(extensionIndex != -1) {
				extension = filePath.Substring(extensionIndex).ToLowerInvariant();
			}
			if(content == "")
				content = GetFileAllText(filePath);
			return GetHTMLFormattedUnknownFileControlInternal(extension, content);
		}
		public bool HasJSCodeRegions(string content) {
			return GetJSCodeRegions(content).Length > 0;
		}
		public WebControl GetHTMLFormattedAspxFileControl(string content) {
			return GetHTMLFormattedAspxFileControl(content, true);
		}
		public WebControl GetHTMLFormattedAspxFileControl(string content, bool excludeJScript) {
			WebControl div = CreateWebControl(HtmlTextWriterTag.Div);
			div.CssClass = "cr-div";
			content = GetFileAllText(content); 
			CodeRegionInfo[] codeRegions = GetAspxDocumentCodeRegions(content, excludeJScript);
			Control curControl = null;
			for(int i = 0; i < codeRegions.Length; i++) {
				if(codeRegions[i].CodeLanguage != CodeLanguage.js || !excludeJScript) {
					string regionHTMLCode = GetHTMLFormattedCodeRegion(codeRegions[i]);
					curControl = CreateAspxCodePreControl(regionHTMLCode);
					div.Controls.Add(curControl);
				}
			}
			return div;
		}
		public virtual WebControl GetHTMLFormattedJSFileControl(string content) {
			content = GetFileAllText(content); 
			WebControl div = null;
			CodeRegionInfo[] codeRegions = GetJSCodeRegions(content);
			if(ShowJsContentAlways && codeRegions.Length == 0 && !string.IsNullOrEmpty(content))
				codeRegions = new CodeRegionInfo[] { new CodeRegionInfo(content, CodeLanguage.js) };
			if(codeRegions.Length > 0) {
				div = CreateWebControl(HtmlTextWriterTag.Div);
				div.CssClass = "cr-div";
				Control curControl = null;
				for(int i = 0; i < codeRegions.Length; i++) {
					string regionHTMLCode = GetHTMLFormattedCodeRegion(codeRegions[i]);
					curControl = CreateJSCodeControl(regionHTMLCode, false);
					div.Controls.Add(curControl);
				}
			}
			return div;
		}
		public WebControl GetHTMLFormattedAsaxFileControl(string content) {
			if(GetIsCsAsax(content))
				return GetHTMLFormattedCSFileControl(content);
			else
				return GetHTMLFormattedVBFileControl(content);
		}
		public WebControl GetHTMLFormattedCSFileControl(string content) {
			return GetHTMLFormattedCodeFileControl(content, CodeLanguage.cs);
		}
		public WebControl GetHTMLFormattedVBFileControl(string content) {
			return GetHTMLFormattedCodeFileControl(content, CodeLanguage.vb);
		}
		public WebControl GetHTMLFormattedXmlFileControl(string content) {
			return GetHTMLFormattedCodeFileControl(content, CodeLanguage.xml);
		}
		protected virtual string GetFormattedCode(CodeLanguage language, string content, bool displayCodeType) {
			CodeFormatter codeRender = new CodeFormatter(language, displayCodeType);
			return codeRender.GetFormattedCode(content);
		}
		protected virtual Control GetHTMLFormattedUnknownFileControlInternal(string extension, string content) {
			switch(extension) {
				case ".master": 
				case ".aspx":
				case ".ascx":
					return GetHTMLFormattedAspxFileControl(content);
				case ".js":
					return GetHTMLFormattedJSFileControl(content);
				case ".cs":
					return GetHTMLFormattedCSFileControl(content);
				case ".vb":
					return GetHTMLFormattedVBFileControl(content);
				case ".asax":
					return GetHTMLFormattedAsaxFileControl(content);
				case ".xml":
				case ".xaml":
				case ".config":
				case ".xafml":
					return GetHTMLFormattedXmlFileControl(content);
				default:
					return GetHTMLFormattedCodeFileControl(content, CodeLanguage.unknown);
			}
		}
		protected bool GetIsCsAsax(string content) {
			string languageNeedle = "Language=\"";
			int languageIndex = content.IndexOf(languageNeedle);
			if(languageIndex < 0) return true;
			languageIndex += languageNeedle.Length;
			return content[languageIndex] == 'C';
		}
		protected WebControl GetHTMLFormattedCodeFileControl(string content, CodeLanguage codeLanguage) {
			WebControl div = CreateWebControl(HtmlTextWriterTag.Div);
			div.CssClass = "cr-div";
			content = GetFileAllText(content); 
			div.Controls.Add(CreatePreControl(this.GetFormattedCode(codeLanguage, content, false), ""));
			return div;
		}
		protected Control CreateJSCodeControl(string text, bool needScriptTags) {
			Control mainControl = new Control();
			if(needScriptTags) {
				CodeRegionInfo jsTag = new CodeRegionInfo();
				jsTag.CodeLanguage = CodeLanguage.aspx;
				jsTag.CodeString = BeginJStCodeTag;
				WebControl beginJSTag = CreateAspxCodeSpanControl(GetHTMLFormattedCodeRegion(jsTag));
				mainControl.Controls.Add(beginJSTag);
			}
			mainControl.Controls.Add(CreatePreControl(text, "cr-js-pre"));
			if(needScriptTags) {
				CodeRegionInfo jsEndTag = new CodeRegionInfo();
				jsEndTag.CodeString = EndJStCodeTag;
				WebControl endJSTag = CreateAspxCodeSpanControl(GetHTMLFormattedCodeRegion(jsEndTag));
				mainControl.Controls.Add(endJSTag);
			}
			return mainControl;
		}
		protected CodeRegionInfo[] GetAspxDocumentCodeRegions(string documentText, bool excludeJScript) {
			List<CodeRegionInfo> ret = new List<CodeRegionInfo>();
			AspxCodeRegionRegEx = new Regex(AspxCodeRegionRegExString);
			MatchCollection mathCollection = AspxCodeRegionRegEx.Matches(documentText);
			if((double)mathCollection.Count % 2 != 0)
				throw new Exception("");
			int count = mathCollection.Count;
			int regionCount = (int)mathCollection.Count / 2;
			if(regionCount == 0) {
				CodeRegionInfo[] regions = GetASPXCodeRegions(documentText, 0,
					documentText.Length, excludeJScript);
				ret.AddRange(regions);
			}
			else {
				for(int i = 0; i < regionCount; i++) {
					CodeRegionInfo[] regions = GetASPXCodeRegions(documentText, mathCollection[2 * i].Index,
						mathCollection[2 * i + 1].Index, excludeJScript);
					ret.AddRange(regions);
				}
			}
			return ret.ToArray();
		}
		protected string GetHTMLFormattedCodeRegion(CodeRegionInfo codeRegion) {
			string ret = "";
			string regFormattedString = "";
			regFormattedString = this.GetFormattedCode(codeRegion.CodeLanguage, codeRegion.CodeString, false); 
			ret += regFormattedString;
			return ret.Trim();
		}
		private CodeRegionInfo[] GetASPXCodeRegions(string content, int startIndex, int endIndex, bool excludeJScript) {
			List<CodeRegionInfo> ret = new List<CodeRegionInfo>();
			string regionContent = content.Substring(startIndex, endIndex - startIndex).Trim();
			regionContent = regionContent.Replace(BeginAspxCodeRegionString, "");
			regionContent = regionContent.Replace(EndAspxCodeRegionString, "");
			MatchCollection mathCollection;
			if(excludeJScript) {
				JSCodeRegionRegEx = new Regex(JSCodeRegionRegExString, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
				mathCollection = JSCodeRegionRegEx.Matches(regionContent);
				if(mathCollection.Count == 0) {
					JSCodeRegionRegEx = new Regex(JSCodeRegionRegExStringWithoutComment, RegexOptions.Multiline |
						RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
					mathCollection = JSCodeRegionRegEx.Matches(regionContent);
				}
			}
			else
				mathCollection = new Regex("FAKE_NO_JSCRIPT_REGEX").Matches(regionContent);
			Dictionary<int, RegionPosition> scriptStartEndPosInBindingExp = GetScriptPosInBindingExpressionDic(regionContent);
			Dictionary<int, int> scriptInJSBlocks = GetJSBlockPosition(regionContent);
			if((double)mathCollection.Count % 2 != 0)
				throw new Exception("");
			int regionCount = (int)mathCollection.Count / 2;
			int index = 0;
			for(int i = 0; i < regionCount; i++) {
				int startJSIndex = mathCollection[2 * i].Index;
				int endJSIndex = mathCollection[2 * i + 1].Index;
				bool isScriptInBindingExpression =
								scriptStartEndPosInBindingExp.ContainsKey(startJSIndex) &&
								(scriptStartEndPosInBindingExp[startJSIndex].EndPos == endJSIndex);
				bool isScriptInJSCodeBlock = IsScriptInJSCodeBlock(scriptInJSBlocks,
					startJSIndex, endJSIndex);
				int correctStartJSIndex = startJSIndex;
				if(isScriptInBindingExpression)
					correctStartJSIndex = scriptStartEndPosInBindingExp[startJSIndex].CorrectedEndPos;
				else if(isScriptInJSCodeBlock)
					correctStartJSIndex = scriptInJSBlocks.ContainsKey(startJSIndex) ? scriptInJSBlocks[startJSIndex] : endJSIndex;
				CodeRegionInfo aspxRegionInfo = GetCodeRegionInfo(regionContent, index, correctStartJSIndex,
					CodeLanguage.aspx);
				if(aspxRegionInfo != null)
					ret.Add(aspxRegionInfo);
				index = endJSIndex + mathCollection[2 * i + 1].Value.Length;
				if(isScriptInBindingExpression)
					index = scriptStartEndPosInBindingExp[startJSIndex].CorrectedEndPos;
			}
			if(index <= regionContent.Length - 1) {
				CodeRegionInfo aspxRegionInfo = GetCodeRegionInfo(regionContent, index, regionContent.Length, CodeLanguage.aspx);
				if(aspxRegionInfo != null)
					ret.Add(aspxRegionInfo);
			}
			return ret.ToArray();
		}
		private Dictionary<int, int> GetJSBlockPosition(string regionContent) {
			Dictionary<int, int> ret = new Dictionary<int, int>();
			Regex regEx = new Regex(JSInAspxCodeRegionRegExString);
			MatchCollection mathCollection = regEx.Matches(regionContent);
			if((double)mathCollection.Count % 2 != 0)
				throw new Exception("");
			int count = mathCollection.Count;
			int regionCount = (int)mathCollection.Count / 2;
			for(int i = 0; i < regionCount; i++) {
				string jsCode = regionContent.Substring(mathCollection[2 * i].Index,
					mathCollection[2 * i + 1].Index - mathCollection[2 * i].Index);
				ret.Add(jsCode.IndexOf("<script") + mathCollection[2 * i].Index,
					mathCollection[2 * i + 1].Index);
			}
			return ret;
		}
		private Dictionary<int, RegionPosition> GetScriptPosInBindingExpressionDic(string regionContent) {
			Dictionary<int, RegionPosition> ret = new Dictionary<int, RegionPosition>();
			Regex regEx = new Regex(JSCodeRegionRegExStringInBindingExpression, RegexOptions.Multiline |
				RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
			MatchCollection mathCollection = regEx.Matches(regionContent);
			if((double)mathCollection.Count % 2 != 0)
				throw new Exception("");
			int regionCount = (int)mathCollection.Count / 2;
			for(int i = 0; i < regionCount; i++) {
				int startJSIndex = mathCollection[2 * i].Index +
					mathCollection[2 * i].Value.IndexOf("<script");
				int endJSIndex = mathCollection[2 * i + 1].Index;
				RegionPosition endPos = new RegionPosition();
				endPos.EndPos = endJSIndex;
				endPos.CorrectedEndPos = endJSIndex + mathCollection[2 * i + 1].Value.Length;
				ret.Add(startJSIndex, endPos);
			}
			return ret;
		}
		private CodeRegionInfo[] GetJSCodeRegions(string content) {
			List<CodeRegionInfo> ret = new List<CodeRegionInfo>();
			JSCodeRegionRegEx = new Regex(JSCodeRegionRegExString, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
			MatchCollection mathCollection = JSCodeRegionRegEx.Matches(content);
			if(mathCollection.Count == 0) {
				JSCodeRegionRegEx = new Regex(JSCodeRegionRegExStringWithoutComment, RegexOptions.Multiline |
					RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
				mathCollection = JSCodeRegionRegEx.Matches(content);
			}
			if((double)mathCollection.Count % 2 != 0)
				throw new Exception("");
			Dictionary<int, RegionPosition> scriptStartEndPosInBindingExp = GetScriptPosInBindingExpressionDic(content);
			Dictionary<int, int> scriptInJSBlocks = GetJSBlockPosition(content);
			int regionCount = (int)mathCollection.Count / 2;
			if(regionCount != 0) {
				for(int i = 0; i < regionCount; i++) {
					int startJSIndex = mathCollection[2 * i].Index;
					int endJSIndex = mathCollection[2 * i + 1].Index;
					bool isScriptInBindingExpression =
									scriptStartEndPosInBindingExp.ContainsKey(startJSIndex) &&
									(scriptStartEndPosInBindingExp[startJSIndex].EndPos == endJSIndex);
					bool isScriptInJSCodeBlock = IsScriptInJSCodeBlock(scriptInJSBlocks, startJSIndex, endJSIndex);
					if(!isScriptInBindingExpression && !isScriptInJSCodeBlock) {
						CodeRegionInfo jsRegionInfo = GetCodeRegionInfo(content, startJSIndex + mathCollection[2 * i].Value.Length,
							endJSIndex, CodeLanguage.js);
						if(jsRegionInfo != null)
							ret.Add(jsRegionInfo);
					}
				}
			}
			return ret.ToArray();
		}
		private CodeRegionInfo GetCodeRegionInfo(string text, int startIndex, int endIndex, CodeLanguage language) {
			CodeRegionInfo ret = new CodeRegionInfo();
			ret.CodeLanguage = language;
			if(endIndex - startIndex > 0) {
				ret.CodeString = text.Substring(startIndex, endIndex - startIndex).Trim();
				ret.CodeString = ret.CodeString.Replace(BeginJSCodeRegionString, "").Replace(EndJSCodeRegionString, "");
			}
			return ret.CodeString != "" ? ret : null;
		}
		private bool IsScriptInJSCodeBlock(Dictionary<int, int> scriptInJSBlocks,
			int startJSIndex, int endJSIndex) {
			foreach(int key in scriptInJSBlocks.Keys) {
				if((key >= startJSIndex) && (key <= endJSIndex))
					return true;
			}
			return false;
		}
		protected Control CreateAspxCodePreControl(string text) {
			return CreatePreControl(text, "cr-aspx-text cr-aspx-pre");
		}
		private WebControl CreateAspxCodeSpanControl(string text) {
			WebControl preControl = CreateWebControl(HtmlTextWriterTag.Span);
			preControl.Controls.Add(CreateLiteralControl(text));
			preControl.CssClass = "cr-aspx-text";
			return preControl;
		}
		private Control CreatePreControl(string text, string cssClass) {
			return new LiteralControl(string.Format(PreControlString, cssClass, text));
		}
		private Control CreateLiteralControl(string text) {
			LiteralControl litControl = new LiteralControl(text);
			litControl.EnableViewState = false;
			return litControl;
		}
		protected WebControl CreateWebControl(HtmlTextWriterTag tagName) {
			WebControl control = new WebControl(tagName);
			control.EnableViewState = false;
			return control;
		}
		private string HtmlEncode(string content) {
			return HttpUtility.HtmlEncode(content);
		}
		protected string GetFileAllText(string fileName) {
			string contentString = string.Empty;
			if(File.Exists(fileName)) {
				using(StreamReader reader = new StreamReader(fileName, Encoding.UTF8)) {
					contentString = reader.ReadToEnd();
					reader.Close();
				}
			}
			else {
				contentString = fileName;
			}
			return contentString;
		}
	}
	public class CodeFormatter {
		public interface IFormatter {
			string Render(string code);
			string Render(string code, bool extractstyles, Dictionary<string, string> styles);
			string LanguageName { get; set;}
		}
		public class FormatterBase : IFormatter {
			protected static int regNumber = 0;
			private bool extractStyles = false;
			private Dictionary<string, string> Styles = null;
			public FormatterBase(string langName) {
				LanguageName = langName;
			}
			private string _LanguageName;
			public string LanguageName {
				get {
					return _LanguageName;
				}
				set {
					_LanguageName = value;
				}
			}
			protected const string regionBlockReg = @"(?<WhiteSpace>(?:{0})*\s*)(?<BeginRegion>#region){{1}}\s*(?<Caption>.*)(?<Block>[\s\S]*?)(?<EndRegion>#end\s*region.*[\r\n]?){{1}}";
			protected const string textBlock =
						"<h1 class=\"{5}\">" +
						"<span onclick=\"ExpandCollapse('section{4}{0}Toggle')\" style=\"cursor:default;\" onkeypress=\"ExpandCollapse_CheckKey(event, 'section{4}{0}Toggle')\" tabindex=\"0\">" +
						"<img id=\"section{4}{0}Toggle\" alt=\"Expand\" src=\"{7}Images/collapsedbutton.gif\"/> {1}" +
						"</span></h1>" +
						"<div id=\"section{4}{0}Section\" class=\"{6}\" style=\"display: none;\"><div>{2}</div></div>";
			protected internal string rxComments = "";
			protected internal string rxStrings = "";
			protected internal string rxKeywords = "";
			protected internal string rxPreprocs = "";
			protected internal string rxTags = "";
			protected internal string rxAttributes = "";
			protected internal string cssKeyword = "";
			protected internal string cssComment = "";
			protected internal string cssString = "";
			protected internal string cssPreproc = "";
			protected internal string cssTag = "";
			protected internal string cssAttribute = "", cssRegionHead = "cr-region-head", cssRegionDiv = "cr-region-div", cssRegionSpan = "cr-region-span";
			protected internal bool caseSensitive = true;
			protected string GetRegionSpan() { return string.Format("<span class=\"{0}\">&nbsp;</span>", this.cssRegionSpan); }
			protected virtual string ReplaceEntities(Match match) {
				return Regex.Replace(match.ToString(), "&([^agl][^mt][^p;][^;])", "&amp;$1", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
			}
			protected string ReplaceRegion(Match match) {
				string res = match.ToString();
				if(match.Groups["BeginRegion"] == null || match.Groups["EndRegion"] == null)
					return res;
				string codeBlock = match.Groups["Block"].ToString();
				string spacing = match.Groups["WhiteSpace"] == null ? string.Empty : match.Groups["WhiteSpace"].ToString();
				if(spacing.EndsWith(" "))
					spacing += "   ";
				codeBlock = codeBlock.Replace(GetRegionSpan(), "");
				codeBlock = codeBlock.Trim('\xd', '\xa').TrimEnd(' ', '\xd', '\xa', '\t');
				string firstUrlSegment = HttpContext.Current.Request.Url.Segments[1].ToLowerInvariant();
				bool isTutorials = (firstUrlSegment.IndexOf("tutorials") == 0) || (firstUrlSegment.IndexOf("helpsamples") == 0);
				string caption = match.Groups["Caption"].Value;
				if(caption.StartsWith(GetRegionSpan())) caption = caption.Remove(0, GetRegionSpan().Length);
				res = string.Format(textBlock, regNumber, caption,
					codeBlock, spacing, LanguageName.Replace("#", "S").Replace(".", ""), this.cssRegionHead, this.cssRegionDiv,
					HttpContext.Current.Request.ApplicationPath + "/");
				regNumber++;
				return res;
			}
			private string MatchEval(Match match) {
				if(match.Groups[1].Success && cssComment != "") { 
					StringReader reader = new StringReader(ReplaceEntities(match));
					StringBuilder sb = new StringBuilder();
					string line = reader.ReadLine();
					while(!string.IsNullOrEmpty(line)) {
						if(sb.Length > 0) {
							sb.Append("\r\n");
						}
						sb.Append("<span class=\"" + cssComment + "\">");
						sb.Append(line);
						sb.Append("</span>");
						line = reader.ReadLine();
					}
					return sb.ToString();
				}
				if(match.Groups[5].Success && cssTag != "") 
				{
					if(extractStyles) {
						return "<span style=\"" + Styles[cssTag] + "\">" + ReplaceEntities(match) + "</span>";
					}
					else return "<span class=\"" + cssTag + "\">" + ReplaceEntities(match) + "</span>";
				}
				if(match.Groups[6].Success && cssAttribute != "") 
				{
					if(extractStyles) {
						return "<span style=\"" + Styles[cssAttribute] + "\">" + ReplaceEntities(match) + "</span>";
					}
					else return "<span class=\"" + cssAttribute + "\">" + ReplaceEntities(match) + "</span>";
				}
				if(match.Groups[2].Success && cssString != "") 
				{
					if(extractStyles) {
						return "<span style=\"" + Styles[cssString] + "\">" + ReplaceEntities(match) + "</span>";
					}
					else return "<span class=\"" + cssString + "\">" + ReplaceEntities(match) + "</span>";
				}
				if(match.Groups[3].Success && cssPreproc != "") 
				{
					if(extractStyles) {
						return "<span style=\"" + Styles[cssPreproc] + "\">" + ReplaceEntities(match) + "</span>";
					}
					else return "<span class=\"" + cssPreproc + "\">" + ReplaceEntities(match) + "</span>";
				}
				if(match.Groups[4].Success && cssKeyword != "") 
				{
					if(extractStyles) {
						return "<span style=\"" + Styles[cssKeyword] + "\">" + ReplaceEntities(match) + "</span>";
					}
					else return "<span class=\"" + cssKeyword + "\">" + ReplaceEntities(match) + "</span>";
				}
				return ReplaceEntities(match); 
			}
			private Regex GetCompiledRegex() {
				Regex r;
				r = new Regex(@"\w+|#\w+|#(\\s\*\w+)+");
				string regKeyword = r.Replace(rxKeywords, @"(?<=^|\W)$0(?=\W)");
				string regPreproc = r.Replace(rxPreprocs, @"(?<=^|\s)$0(?=\s|$)");
				string regHPKeyword = r.Replace(rxKeywords, @"(?<=^|\W)$0(?=\W)");
				r = new Regex(@" +");
				regKeyword = r.Replace(regKeyword, @"|");
				regPreproc = r.Replace(regPreproc, @"|");
				regHPKeyword = r.Replace(regHPKeyword, @"|");
				StringBuilder regAll = new StringBuilder();
				regAll.Append("(");
				regAll.Append(rxComments);
				regAll.Append(")|(");
				regAll.Append(rxStrings);
				regAll.Append(")|(");
				regAll.Append(regPreproc);
				if(regKeyword != String.Empty) {
					regAll.Append(")|(");
					regAll.Append(regKeyword);
				}
				if(rxTags != String.Empty) {
					regAll.Append(")|(");
					regAll.Append(rxTags);
				}
				if(rxAttributes != String.Empty) {
					regAll.Append(")|(");
					regAll.Append(rxAttributes);
				}
				regAll.Append(")");
				RegexOptions caseInsensitive = caseSensitive ? 0 : RegexOptions.IgnoreCase;
				return new Regex(regAll.ToString(), RegexOptions.Singleline | caseInsensitive);
			}
			public virtual string Render(string code, bool extractstyles, Dictionary<string, string> styles) {
				extractStyles = extractstyles;
				Styles = styles;
				return Render(code);
			}
			public virtual string Render(string code) {
				string res = GetCompiledRegex().Replace(code, new MatchEvaluator(this.MatchEval));
				res = GetLastReplace(res);
				if(AllowRegions)
					res = ReplaceRegions(res);
				res = res.Replace("\t", "    ");
				return res;
			}
			protected virtual string ReplaceRegions(string res) {
				res = new Regex("^", RegexOptions.Multiline).Replace(res, GetRegionSpan());
				Regex reg = new Regex(string.Format(regionBlockReg, GetRegionSpan().Replace("<", "\\<").Replace(">", "\\>")), RegexOptions.IgnoreCase);
				return reg.Replace(res, new MatchEvaluator(ReplaceRegion));
			}
			protected virtual bool AllowRegions { get { return false; } }
			public virtual string GetLastReplace(string code) {
				return code;
			}
		}
		public class UnknownFormatter : FormatterBase {
			public UnknownFormatter(string langName)
				: base(langName) {
			}
		}
		public class CSFormatter : FormatterBase {
			public CSFormatter(string langName)
				: base(langName) {
				rxKeywords = "abstract as base bool break byte case catch char "
				+ "checked class const continue decimal default delegate do double else "
				+ "enum event explicit extern false finally fixed float for foreach goto "
				+ "if implicit in int interface internal is lock long namespace new null "
				+ "object operator out override params private protected public readonly "
				+ "ref return sbyte sealed short sizeof stackalloc static string struct "
				+ "switch this throw true try typeof uint ulong unchecked unsafe ushort "
				+ "using virtual void while partial";
				rxComments = @"/\*.*?\*/|//.*?(?=\r|\n)";
				rxStrings = @"@?""""|@?"".*?(?!\\).*?""|''|'.*?(?!\\).'";
				rxPreprocs = "#if #else #elif #endif #define #undef #warning #error #line";
				cssComment = "cr-cs-comment";
				cssKeyword = "cr-cs-keyword";
				cssPreproc = "cr-cs-preproc";
				cssString = "cr-cs-string";
			}
			protected override bool AllowRegions { get { return true; } }
			public override string Render(string code) {
				code = code.Replace("<", "&lt;").Replace(">", "&gt;");
				return base.Render(code);
			}
		}
		public class XMLFormatter : FormatterBase {
			public XMLFormatter(string langName)
				: base(langName) {
				rxKeywords = "=\" \"";
				rxComments = @"<!--.*?-->";
				rxStrings = @"NevypolnimoeUslovie";
				rxTags = @"/>|<.+?[\s|>]";
				rxAttributes = @"[\w]+?=";
				rxPreprocs = "#if #else #elif #endif #define #undef #warning #error #line #region #endregion";
				cssComment = "cr-xml-comment";
				cssKeyword = "cr-xml-keyword";
				cssPreproc = "cr-xml-preproc";
				cssString = "cr-xml-string";
				cssTag = "cr-xml-tag";
				cssAttribute = "cr-xml-attribute";
			}
			protected override string ReplaceEntities(Match match) {
				return match.ToString().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
			}
			public override string Render(string code) {
				return base.Render(code);
			}
		}
		public class ASPXFormatter : FormatterBase {
			public ASPXFormatter(string langName)
				: base(langName) {
				rxKeywords = "asdasdasdasd";
				rxComments = @"<%--.*?--%>";
				rxStrings = @"# .+?[\) .]";
				rxTags = @"/>|<.+?[\s|>]";
				rxAttributes = @"[-\w]+?=";
				rxPreprocs = "<% %>";
				cssComment = "cr-aspx-comment";
				cssKeyword = "cr-aspx-keyword";
				cssPreproc = "cr-aspx-preproc";
				cssString = "cr-aspx-string";
				cssTag = "cr-aspx-tag";
				cssAttribute = "cr-aspx-attribute";
				cssRegionHead = "cr-aspx-region-head";
			}
			protected override string ReplaceEntities(Match match) {
				return match.ToString().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
			}
			public override string Render(string code) {
				Regex reg = new Regex(@"\r?(?<WhiteSpace>\s*)(?<BeginRegion>\<\%--\s*BeginRegion){1}\s*(?<Caption>.*?)(?:--\%\>){1}(?<Block>[\s\S]*?)(?<EndRegion>\s*\<\%--\s*EndRegion\s*--\%\>){1}");
				code = reg.Replace(code, new MatchEvaluator(ReplaceRegionPre));
				return base.Render(code);
			}
			public override string GetLastReplace(string code) {
				Regex rx = new Regex("([^c^l^a^s^s])=");
				string newCode = rx.Replace(code, "$1<span class=\"cr-aspx-text\">=</span>");
				rx = new Regex("(&lt;/|&lt;)");
				newCode = rx.Replace(newCode, "<span class=\"cr-aspx-text\">$1</span>");
				rx = new Regex("(/&gt;|&gt;)");
				return rx.Replace(newCode, "<span class=\"cr-aspx-text\">$1</span>");
			}
			protected override bool AllowRegions { get { return true; } }
			string ReplaceRegionPre(Match match) {
				string res = match.ToString();
				if(match.Groups["BeginRegion"] == null || match.Groups["EndRegion"] == null)
					return res;
				string spacing = match.Groups["WhiteSpace"] == null ? string.Empty : match.Groups["WhiteSpace"].ToString();
				string caption = match.Groups["Caption"] == null ? "" : match.Groups["Caption"].ToString();
				return string.Format("{0}#region {1}\xd\xa{2}\xd\xa#endregion", spacing, caption, match.Groups["Block"]);
			}
		}
		public class VBNETFormatter : FormatterBase {
			public VBNETFormatter(string langName)
				: base(langName) {
				rxKeywords = "AddHandler AddressOf AndAlso Alias And Ansi As Assembly "
					+ "Auto Boolean ByRef Byte ByVal Call Case Catch "
					+ "CBool CByte CChar CDate CDec CDbl Char CInt "
					+ "Class CLng CObj Const CShort CSng CStr CType "
					+ "Date Decimal Declare Default Delegate Dim DirectCast Do "
					+ "Double Each Else ElseIf End Enum Erase Error "
					+ "Event Exit False Finally For Friend Function Get "
					+ "GetType GoTo  Handles If Implements Imports In Inherits "
					+ "Integer Interface Is Let Lib Like Long Loop "
					+ "Me Mod Module MustInherit MustOverride MyBase MyClass Namespace "
					+ "New Next Not Nothing NotInheritable NotOverridable Object On "
					+ "Option Optional Or OrElse Overloads Overridable Overrides ParamArray "
					+ "Preserve Private Property Protected Public RaiseEvent ReadOnly ReDim "
					+ "REM RemoveHandler Resume Return Select Set Shadows Shared "
					+ "Short Single Static Step Stop String Structure Sub "
					+ "SyncLock Then Throw To True Try TypeOf Unicode "
					+ "Until Variant When While With WithEvents WriteOnly Xor Partial";
				rxComments = @"(?:'|REM\s).*?(?=\r|\n)";
				rxPreprocs = @"#\s*Const #\s*If #\s*Else #\s*ElseIf #\s*End\s*If "
					+ @"#\s*ExternalSource #\s*End\s*ExternalSource";
				rxStrings = @"""""|"".*?""";
				cssComment = "cr-vb-comment";
				cssKeyword = "cr-vb-keyword";
				cssString = "cr-vb-string";
				cssPreproc = "cr-vb-preproc";
				caseSensitive = false;
			}
			public override string Render(string code) {
				code = new Regex(@"#End\s{1}Region").Replace(code, "#EndRegion");
				code = code.Replace("<", "&lt;").Replace(">", "&gt;");
				return base.Render(code);
			}
			protected override bool AllowRegions { get { return true; } }
		}
		public class DelphiFormatter : FormatterBase {
			public DelphiFormatter(string langName)
				: base(langName) {
				rxKeywords = "end; and array as begin case class const constructor destructor div do downto else end except"
					+ " file finally for function goto if implementation in inherited interface is mod not object of on or packed procedure program property raise"
					+ " record repeat set shl shr then threadvar to try type unit until uses var while with xor"
					+ " true false ansichar ansistring boolean byte cardinal char comp currency double extended int64"
					+ " integer longint longword pchar pointer real shortint single string tlist"
					+ " variant word nil read write override private public protected virtual";
				rxComments = @"\{.*?\}|//.*?(?=\r|\n)";
				rxStrings = @"'.*?'";
				rxPreprocs = "#if #else #elif #endif #define #undef #warning #error #line #region #endregion";
				cssKeyword = "cr-delphi-keyword";
				cssComment = "cr-delphi-comment";
				cssString = "cr-delphi-string";
				cssPreproc = "cr-delphi-preproc";
				caseSensitive = false;
			}
		}
		public class JSFormatter : FormatterBase {
			public JSFormatter(string langName)
				: base(langName) {
				rxKeywords = "var break case catch array "
				+ "function continue default do else "
				+ "false finally for foreach goto "
				+ "if new null "
				+ "object "
				+ "return "
				+ "switch this throw true try typeof "
				+ "while";
				rxComments = @"/\*.*?\*/|//.*?(?=\r|\n)";
				rxStrings = @"@?""""|@?"".*?(?!\\).""|''|'.*?(?!\\).'";
				rxPreprocs = "#if #else #elif #endif #define #undef #warning #error #line #region #endregion";
				cssComment = "cr-js-comment";
				cssKeyword = "cr-js-keyword";
				cssPreproc = "cr-js-preproc";
				cssString = "cr-js-string";
			}
			protected override string ReplaceEntities(Match match) {
				return match.ToString().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
			}
		}
		protected static Dictionary<CodeLanguage, IFormatter> languages = new Dictionary<CodeLanguage, IFormatter>();
		public CodeFormatter(string language) : this(language, false) {
		}
		public CodeFormatter(CodeLanguage language) : this(language, false) {
		}
		public CodeFormatter(string language, bool displayCodeType) {
			CodeLanguage = parseLanguageType(language);
			DisplayCodeType = displayCodeType;
		}
		public CodeFormatter(CodeLanguage language, bool displayCodeType) {
			CodeLanguage = language;
			DisplayCodeType = displayCodeType;
		}
		static CodeFormatter() {
			languages.Add(CodeLanguage.cs, new CSFormatter("C#"));
			languages.Add(CodeLanguage.vb, new VBNETFormatter("VB.NET"));
			languages.Add(CodeLanguage.delphi, new DelphiFormatter("Delphi"));
			languages.Add(CodeLanguage.cbuilder, new UnknownFormatter("C++ Builder"));
			languages.Add(CodeLanguage.c, new UnknownFormatter("C++"));
			languages.Add(CodeLanguage.vb6, new UnknownFormatter("VB"));
			languages.Add(CodeLanguage.js, new JSFormatter("JScript"));
			languages.Add(CodeLanguage.vbs, new UnknownFormatter("VBScript"));
			languages.Add(CodeLanguage.html, new UnknownFormatter("HTML"));
			languages.Add(CodeLanguage.sql, new UnknownFormatter("SQL"));
			languages.Add(CodeLanguage.xml, new XMLFormatter("XML"));
			languages.Add(CodeLanguage.xaml, new XMLFormatter("XAML"));
			languages.Add(CodeLanguage.aspx, new ASPXFormatter("ASPx"));
		}
		private CodeLanguage _CodeLanguage;
		public CodeLanguage CodeLanguage {
			get {
				return _CodeLanguage;
			}
			set {
				_CodeLanguage = value;
			}
		}
		private bool _DisplayCodeType;
		public bool DisplayCodeType {
			get {
				return _DisplayCodeType;
			}
			set {
				_DisplayCodeType = value;
			}
		}
		protected static CodeLanguage ParseLanguageType(string lang) {
			if(lang.Trim() != "") {
				if(lang.ToLower() == "asp")
					lang = "aspx";
				try {
					return (CodeLanguage)Enum.Parse(typeof(CodeLanguage), lang, true);
				}
				catch {
				}
			}
			return CodeLanguage.unknown;
		}
		private CodeLanguage parseLanguageType(string lang) {
			foreach(CodeLanguage cl in Enum.GetValues(typeof(CodeLanguage))) {
				if(cl.ToString() == lang.ToLower())
					return cl;
			}
			return CodeLanguage.unknown;
		}
		public string GetLanguageName() {
			return languages.ContainsKey(CodeLanguage) ? languages[CodeLanguage].LanguageName : String.Empty;
		}
		public static string GetFormattedCode(string lang, string code) {
			CodeLanguage language = ParseLanguageType(lang);
			string resCode = "";
			if(language == CodeLanguage.unknown)
				return HttpUtility.HtmlEncode(code);
			if(languages.ContainsKey(language)) {
				resCode += languages[language].Render(code + " ");
			}
			return resCode.TrimEnd(' ');
		}
		public virtual string GetFormattedCode(string code) {
			string resCode = "";
			if(CodeLanguage == CodeLanguage.unknown)
				return HttpUtility.HtmlEncode(code);
			if(DisplayCodeType && languages.ContainsKey(CodeLanguage)) {
				resCode = "[" + languages[CodeLanguage].LanguageName + "]\r\n\r\n";
			}
			if(code != "" && languages.ContainsKey(CodeLanguage)) {
				resCode += languages[CodeLanguage].Render(code + " ");
			}
			return resCode.TrimEnd(' ');
		}
	}
	public class CodeRendererStyles {
		public static Dictionary<string, string> StyleDicCS = new Dictionary<string, string>();
		public static Dictionary<string, string> StyleDicVB = new Dictionary<string, string>();
		public static Dictionary<string, string> StyleDicJS = new Dictionary<string, string>();
		public static Dictionary<string, string> StyleDicXML = new Dictionary<string, string>();
		public static Dictionary<string, string> StyleDicDELPHI = new Dictionary<string, string>();
		public static Dictionary<CodeLanguage, Dictionary<string, string>> LanguageStyles = new Dictionary<CodeLanguage, Dictionary<string, string>>();
		static CodeRendererStyles() {
			StyleDicCS.Add("cr-cs-keyword", "color: Blue;");
			StyleDicCS.Add("cr-cs-comment", "color: Green;");
			StyleDicCS.Add("cr-cs-string", "color: #A31515;");
			StyleDicCS.Add("cr-cs-preproc", "color: #2B91AF;");
			StyleDicCS.Add("cr-cs-tag", "");
			StyleDicCS.Add("cr-cs-attribute", "");
			StyleDicVB.Add("cr-vb-keyword", "color: Blue;");
			StyleDicVB.Add("cr-vb-comment", "color: Green;");
			StyleDicVB.Add("cr-vb-string", "color: #A31515;");
			StyleDicVB.Add("cr-vb-preproc", "color: #2B91AF;");
			StyleDicVB.Add("cr-vb-tag", "");
			StyleDicVB.Add("cr-vb-attribute", "");
			StyleDicJS.Add("cr-js-keyword", "color: Blue;");
			StyleDicJS.Add("cr-js-comment", "color: Green;");
			StyleDicJS.Add("cr-js-string", "color: #A31515;");
			StyleDicJS.Add("cr-js-preproc", "color: #2B91AF;");
			StyleDicJS.Add("cr-js-tag", "");
			StyleDicJS.Add("cr-js-attribute", "");
			StyleDicXML.Add("cr-xml-keyword", "color: Blue;");
			StyleDicXML.Add("cr-xml-comment", "color: Green;");
			StyleDicXML.Add("cr-xml-string", "color: #A31515;");
			StyleDicXML.Add("cr-xml-preproc", "color: #2B91AF;");
			StyleDicXML.Add("cr-xml-tag", "color: #A31515;");
			StyleDicXML.Add("cr-xml-attribute", "color: #FF0000;");
			StyleDicDELPHI.Add("cr-delphi-keyword", "color: Blue;");
			StyleDicDELPHI.Add("cr-delphi-comment", "color: Green;");
			StyleDicDELPHI.Add("cr-delphi-string", "color: #A31515;");
			StyleDicDELPHI.Add("cr-delphi-preproc", "color: #2B91AF;");
			StyleDicDELPHI.Add("cr-delphi-tag", "");
			StyleDicDELPHI.Add("cr-delphi-attribute", "");
			LanguageStyles.Add(CodeLanguage.cs, StyleDicCS);
			LanguageStyles.Add(CodeLanguage.vb, StyleDicVB);
			LanguageStyles.Add(CodeLanguage.js, StyleDicJS);
			LanguageStyles.Add(CodeLanguage.xml, StyleDicXML);
			LanguageStyles.Add(CodeLanguage.xaml, StyleDicXML);
			LanguageStyles.Add(CodeLanguage.delphi, StyleDicDELPHI);
		}
	}
}
