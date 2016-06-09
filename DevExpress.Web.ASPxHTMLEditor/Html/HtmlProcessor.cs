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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public static class HtmlRecoverHelper {
		private static XElement[] ToArray(this IEnumerable<XElement> xElementEnum) {
			List<XElement> res = new List<XElement>();
			foreach(XElement el in xElementEnum) {
				res.Add(el);
			}
			return res.ToArray();
		}
		public static string RestoreHtmlStructure(string processedHtml) {
			XDocument oldDocument = XDocumentTryParse(processedHtml);
			if(oldDocument == null)
				return processedHtml;
			XElement oldBody = oldDocument.Root.Element("body");
			XElement oldHead = oldDocument.Root.Element("head");
			XElement oldHtmlTag = (XElement)oldDocument.Element("html");
			bool isBodyHtml = oldBody == null && oldHead == null && !String.IsNullOrEmpty(oldHtmlTag.Value);
			if(isBodyHtml) {
				Regex unWrapper = new Regex("<html>([\\s\\S]*)</html>", RegexOptions.IgnoreCase);
				string unWrappedProcessedHtml = unWrapper.Matches(processedHtml)[0].Groups[1].ToString();
				return string.Format("<html> \n \n <head>\n <title>title</title>\n </head>\n <body>\n{0}</body>\n </html>", unWrappedProcessedHtml);
			}
			XElement[] nodesList = oldHtmlTag.Elements().ToArray();
			bool restoringRequired = (nodesList == null || nodesList.Length != 2 || nodesList[0].Name != "head" || nodesList[1].Name != "body"
				|| oldHead.Elements("title").ToArray().Length != 1);
			if(!restoringRequired)
				return processedHtml;
			IEnumerable<XAttribute> htmlTagAttributes = (oldHtmlTag != null) ? oldHtmlTag.Attributes() : null;
			XElement headElement = (oldHead == null) ? new XElement("head", "\n  ") : oldHead;
			XElement bodyElement = (oldBody == null) ? new XElement("body", "\n \n  ") : oldBody;
			if(headElement.Element("title") == null) {
				headElement.AddFirst(new XElement("title", "title"));
			}
			XDocument newDocument = new XDocument(
				new XElement("html",
					headElement,
					bodyElement,
					htmlTagAttributes)
					);
			return newDocument.ToString();
		}
		public static XDocument XDocumentTryParse(string html) {
			XDocument doc = new XDocument();
			try {
				doc = XDocument.Parse(html);
			} catch {
				return null;
			}
			return doc;
		}
	}
	public static class HtmlProcessor {
		public static string PrepareHtmlBeforeProcessing(string html, bool allowEditFullDocument) {
			bool wrapped = html.StartsWith("<html") && html.EndsWith("</html>");
			if(!allowEditFullDocument || !wrapped) {
				html = string.Format("<html>{0}</html>", html);
			}
			return html;
		}
		public static string ProcessHtml(string html, IHtmlProcessingSettings settings) {
			using(HtmlParser parser = new HtmlParser()) {
				parser.HtmlValidationSettings = settings;
				html = PrepareHtmlBeforeProcessing(html, settings.AllowEditFullDocument);
				parser.InputReader = new StringReader(html.Trim());
				using(StringWriter strWriter = new StringWriter()) {
					using(InternalXmlWriter xmlWriter = new InternalXmlWriter(strWriter)) {
						if(settings.AutoIndentation) {
							xmlWriter.Formatting = Formatting.Indented;
							xmlWriter.Indentation = settings.IndentSize;
							parser.SkipWhitespaceNodes = true;
						} else {
							parser.SkipWhitespaceNodes = false;
						}
						xmlWriter.IndentChar = settings.IndentChar;
						xmlWriter.QuoteChar = '\"';
						while(parser.Read()) {
							if(parser.NodeType == XmlNodeType.Whitespace) {
								xmlWriter.WriteWhitespace(parser.Value);
							} else {
								xmlWriter.WriteNode(parser, true);
							}
						}
						xmlWriter.Flush();
					}
					return PrepareHtmlStructure(strWriter.ToString(), settings.AllowEditFullDocument);
				}
			}
		}
		public static string PrepareHtmlStructure(string result, bool allowEditFullDocument) {
			if(allowEditFullDocument) {
				result = HtmlRecoverHelper.RestoreHtmlStructure(result);
			} else
				if(result.Length < ("<html>" + "</html>").Length)
					return string.Empty;
				else {
					result = result.Substring("<html>".Length, result.Length - ("<html>" + "</html>").Length);
				}
			result = result.Replace("&amp;", "&");
			return result.Trim();
		}
		public static string ProcessHtmlForSpellChecker(string html, IHtmlProcessingSettings settings) {
			html = ProcessHtml(html, settings);
			StringBuilder sb = new StringBuilder();
			string wrappedHtml = string.Format("<html>{0}</html>", html);
			using (HtmlParser parser = new HtmlParser())
			using(StringReader stringReader = new StringReader(wrappedHtml)) {
				parser.HtmlValidationSettings = settings;
				parser.InputReader = stringReader;
				int prevNodeAbsolutePosition = 0;
				while (parser.Read()) {
					if (parser.NodeType == XmlNodeType.Text || parser.NodeType == XmlNodeType.Whitespace) {
						sb.Append(parser.Value);
						prevNodeAbsolutePosition += parser.Value.Length;
					} else {
						sb.Append(new string(' ', parser.AbsolutePosition - prevNodeAbsolutePosition));
						prevNodeAbsolutePosition = parser.AbsolutePosition;
					}
				}
				string textWithEntityReferences = sb.ToString();
				textWithEntityReferences = (textWithEntityReferences.Length < "<html>".Length) ?
					string.Empty : textWithEntityReferences.Substring("<html>".Length).Replace("&amp;", "[!dxheamp;]");
				sb.Length = 0;
				StringReader reader = new StringReader(textWithEntityReferences);
				StringBuilder entityRef = new StringBuilder();
				char ch = (char)reader.Read();
				while(ch != HtmlScanner.EOF) {
					if(ch == '&') {
						do {
							ch = (char)reader.Read();
							entityRef.Append(ch);
						} while(char.IsLetter(ch));
						if(ch == '&') {
							sb.Append(new string(' ', entityRef.Length));
						} else if(ch == ';') {
							sb.Append(new string(' ', entityRef.Length + 1));
							ch = (char)reader.Read();
						} else {
							sb.Append('&');
							sb.Append(entityRef.ToString());
							ch = (char)reader.Read();
						}
						entityRef.Length = 0;
					} else {
						sb.Append(ch);
						ch = (char)reader.Read();
					}
				}
			}
			return sb.ToString().Replace("[!dxheamp;]", "     ");
		}
	}
	internal class InternalXmlWriter : XmlTextWriter {
		TextWriter textWriter;
		string lastOpenTag;
#if DEBUG
		public static InternalXmlWriter Instance;
#endif
		public InternalXmlWriter(TextWriter textWriter)
			: base(textWriter) {
			this.textWriter = textWriter;
#if DEBUG
			Instance = this;
#endif
		}
		public override void WriteStartElement(string prefix, string localName, string ns) {
			base.WriteStartElement(prefix, localName, ns);
			this.lastOpenTag = localName;
		}
		public override void WriteString(string text) {
			base.WriteString(text);
		}
		public override void WriteCData(string text) {
			text = text.Replace("&", "&amp;");
			bool script = this.lastOpenTag == "script";
			bool style = this.lastOpenTag == "style";
			WriteRaw(string.Empty);
			Flush();
			if(!script && !style) {
				this.textWriter.Write("<![CDATA[");
				this.textWriter.Write(text.Replace("]]>", "]]>]]><![CDATA["));
				this.textWriter.Write("]]>");
			} else {
				this.textWriter.Write(text);
			}				
		}
	}
}
