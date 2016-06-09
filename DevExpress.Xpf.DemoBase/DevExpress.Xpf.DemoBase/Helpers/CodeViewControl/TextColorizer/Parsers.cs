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
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
namespace DevExpress.Xpf.DemoBase.Helpers.TextColorizer.Internal {
	enum LexemType { Error = 0, Block, Symbol, Object, Property, Value, Space, LineBreak, Complex, Comment, PlainText, String, KeyWord }
	class CodeBlockPresenter {
		public CodeLanguage CodeLanguage { get; set; }
		public CodeBlockPresenter() : this(CodeLanguage.Plain) { }
		public CodeBlockPresenter(CodeLanguage language) {
			CodeLanguage = language;
		}
		public TextBlock ToTextBlock(string text) {
			TextBlock textBlock = new TextBlock();
			FillInlines(text, textBlock.Inlines);
			return textBlock;
		}
		public void FillInlines(string text, InlineCollection collection) {
			text = text.Replace("\r", "");
			CodeLexem lex = new CodeLexem(text);
			List<CodeLexem> res = lex.Parse(CodeLanguage);
			foreach(CodeLexem elem in res)
				collection.Add(elem.ToInline());
		}
	}
	class CodeLexem {
		public string Text { get; set; }
		public LexemType Type { get; set; }
		public CodeLexem() : this("") { }
		public CodeLexem(string text) : this(LexemType.Complex, text) { }
		public CodeLexem(LexemType type, string text) {
			Text = text;
			Type = type;
		}
		public List<CodeLexem> Parse(CodeLanguage lang) {
			switch(lang) {
				case CodeLanguage.Plain: return (new BaseParser()).Parse(Text);
				case CodeLanguage.XAML: return (new XamlParser()).Parse(Text);
				case CodeLanguage.CS: return (new CSParser()).Parse(Text);
				case CodeLanguage.VB: return (new VBParser()).Parse(Text);
			}
			return null;
		}
		protected Run CreateRun(string text, Color color) { return new Run() { Text = text, Foreground = new SolidColorBrush(color) }; }
		public Inline ToInline() {
			switch(Type) {
				case LexemType.Complex: return CreateRun(Text, Colors.LightGray);
				case LexemType.LineBreak: return CreateRun("\n", Colors.Black);
				case LexemType.Object: return CreateRun(Text, Colors.Brown);
				case LexemType.Property: return CreateRun(Text, Colors.Red);
				case LexemType.Space: return CreateRun(Text, Colors.Black);
				case LexemType.Symbol: return CreateRun(Text, Colors.Blue);
				case LexemType.Value: return CreateRun(Text, Colors.Blue);
				case LexemType.PlainText: return CreateRun(Text, Colors.Black);
				case LexemType.Comment: return CreateRun(Text, Colors.Green);
				case LexemType.Error: return CreateRun(Text, Colors.LightGray);
				case LexemType.String: return CreateRun(Text, Colors.Brown);
				case LexemType.KeyWord: return CreateRun(Text, Colors.Blue);
			}
			return null;
		}
	}
	class SourceString {
		string Source { get; set; }
		int StartIndex { get; set; }
		public SourceString(string source, int startIndex) {
			if(startIndex > source.Length)
				throw new ArgumentException();
			this.Source = source;
			this.StartIndex = startIndex;
		}
		public int Length { get { return Source.Length - StartIndex; } }
		public char this[int index] { get { return Source[StartIndex + index]; } }
		internal int IndexOf(string text) {
			int index = Source.IndexOf(text, StartIndex, StringComparison.Ordinal);
			if(index < 0)
				return index;
			return index - StartIndex;
		}
		internal int IndexOf(string value, int startIndex) {
			int index = Source.IndexOf(value, StartIndex + startIndex, StringComparison.Ordinal);
			if(index < 0)
				return index;
			return index - StartIndex;
		}
		internal int IndexOfAny(char[] anyOf) {
			int index = Source.IndexOfAny(anyOf, StartIndex);
			if(index < 0)
				return index;
			return index - StartIndex;
		}
		internal int IndexOf(char value) {
			int index = Source.IndexOf(value, StartIndex);
			if(index < 0)
				return index;
			return index - StartIndex;
		}
		internal string Substring(int startIndex, int length) {
			return Source.Substring(StartIndex + startIndex, length);
		}
		internal SourceString Substring(int startIndex) {
			return new SourceString(Source, StartIndex + startIndex);
		}
		internal bool StartsWith(string value, StringComparison comparisonType) {
			return string.Compare(Source, StartIndex, value, 0, value.Length, comparisonType) == 0;
		}
	}
	class BaseParser {
		bool caseSensitive;
		char[] SpaceChars = { ' ', '	' };
		public BaseParser() : this(true) { }
		public BaseParser(bool caseSensitive) {
			this.caseSensitive = caseSensitive;
		}
		protected char previousSimbol;
		protected string StringCut(ref SourceString text, int count) {
			if(count == 0)
				return string.Empty;
			previousSimbol = text[count - 1];
			string result = text.Substring(0, count);
			text = text.Substring(count);
			return result;
		}
		protected void TrySpace(List<CodeLexem> res, ref SourceString text) {
			StringBuilder spaces = new StringBuilder();
			while(SpaceChars.Contains(text[0]))
				spaces.Append(StringCut(ref text, 1));
			if(spaces.Length > 0)
				res.Add(new CodeLexem(LexemType.Space, spaces.ToString()));
		}
		protected bool TryExtract(List<CodeLexem> res, ref SourceString text, string lex, LexemType type) {
			if(text.StartsWith(lex, StringComparison.Ordinal)) {
				res.Add(new CodeLexem(type, StringCut(ref text, lex.Length)));
				return true;
			}
			return false;
		}
		protected void TryExtractTo(List<CodeLexem> res, ref SourceString text, string lex, LexemType type, string except) {
			int index = text.IndexOf(lex);
			if(except != null)
				while(index >= 0 && text.Substring(0, index + 1).EndsWith(except, StringComparison.Ordinal))
					index = text.IndexOf(lex, index + 1);
			if(index < 0) return;
			BreackLines(res, ref text, index + lex.Length, type);
		}
		protected void BreackLines(List<CodeLexem> res, ref SourceString text, int to, LexemType type) {
			while(text.Length > 0 && to > 0) {
				int index = text.IndexOf("\n");
				if(index >= to) {
					res.Add(new CodeLexem(type, StringCut(ref text, to)));
					break;
				}
				if(index != 0) res.Add(new CodeLexem(type, StringCut(ref text, index)));
				res.Add(new CodeLexem(LexemType.LineBreak, StringCut(ref text, 1)));
				to -= index + 1;
			}
		}
		public List<CodeLexem> Parse(string text) {
			return Parse(new SourceString(text + "\n", 0));
		}
		protected virtual List<CodeLexem> Parse(SourceString text) {
			List<CodeLexem> res = new List<CodeLexem>();
			SourceString extendedText = text;
			BreackLines(res, ref extendedText, extendedText.Length, LexemType.PlainText);
			return res;
		}
	}
	internal class CSParser : BaseParser {
		char[] CSEndOfTerm = { ' ', '\t', '\n', '=', '/', '>', '<', '"', '{', '}', ',', '(', ')', ';', '\0', '?' };
		string[] CSKeyWords = { "abstract","event","new","struct","as","explicit","null",
								"switch","base","extern","object","this","bool","false",
								"operator","throw","break","finally","out","true","byte",
								"fixed","override","try","case","float","params","typeof",
								"catch","for","private","uint","char","foreach","protected",
								"ulong","checked","goto","public","unchecked","class",
								"if","readonly","unsafe","const","implicit","ref","ushort",
								"continue","in","return","using","decimal","int","sbyte",
								"virtual","default","interface","sealed","volatile","delegate",
								"internal","short","void","do","is","sizeof","while",
								"double","lock","stackalloc","else","long","static","enum",
								"namespace","string","from","get","group","into","join","let",
								"orderby","partial","select","set","var","where","yield", "async", "await",
								"#region","#endregion","#if","#endif"};
		public CSParser() { }
		protected override List<CodeLexem> Parse(SourceString text) {
			SourceString extendedText = text;
			List<CodeLexem> res = new List<CodeLexem>();
			while(extendedText.Length > 0) {
				if(TryExtract(res, ref extendedText, "/*", LexemType.Comment)) {
					TryExtractTo(res, ref extendedText, "*/", LexemType.Comment, null);
				}
				if(TryExtract(res, ref extendedText, "//", LexemType.Comment)) {
					TryExtractTo(res, ref extendedText, "\n", LexemType.Comment, null);
				}
				if(TryExtract(res, ref extendedText, "\"", LexemType.String)) {
					TryExtractTo(res, ref extendedText, "\"", LexemType.String, "\\\"");
				}
				if(TryExtract(res, ref extendedText, "'", LexemType.String)) {
					TryExtractTo(res, ref extendedText, "'", LexemType.String, null);
				}
				ParseCSKeyWord(res, ref extendedText, LexemType.KeyWord);
				ParseCSSymbol(res, ref extendedText, LexemType.PlainText);
				TrySpace(res, ref extendedText);
				TryExtract(res, ref extendedText, "\n", LexemType.LineBreak);
			}
			return res;
		}
		int lastLength = -1;
		void ParseCSSymbol(List<CodeLexem> res, ref SourceString text, LexemType lt) {
			if(lastLength == -1 || lastLength != text.Length) {
				lastLength = text.Length;
				return;
			}
			CodeLexem cl = res.Count > 0 ? res.Last() : null;
			if(cl != null && cl.Type == LexemType.PlainText)
				cl.Text += StringCut(ref text, 1);
			else
				res.Add(new CodeLexem(LexemType.PlainText, StringCut(ref text, 1)));
		}
		void ParseCSKeyWord(List<CodeLexem> res, ref SourceString text, LexemType type) {
			int index = -1;
			if(!CSEndOfTerm.Contains(previousSimbol)) return;
			foreach(string str in CSKeyWords) {
				if(text.StartsWith(str, StringComparison.Ordinal)) {
					if(!CSEndOfTerm.Contains(text[str.Length])) continue;
					index = str.Length;
					break;
				}
			}
			if(index < 0) return;
			res.Add(new CodeLexem(type, StringCut(ref text, index)));
		}
	}
	class XamlParser : BaseParser {
		char[] XamlEndOfTerm = { ' ', '	', '\n', '=', '/', '>', '<', '"', '{', '}', ',' };
		char[] XamlSymbol = { '=', '/', '>', '"', '{', '}', ',' };
		char[] XamlNamespaceDelimeter = { ':' };
		public XamlParser() { }
		protected bool IsInsideBlock = false;
		protected override List<CodeLexem> Parse(SourceString text) {
			SourceString extendedText = text;
			List<CodeLexem> res = new List<CodeLexem>();
			while(extendedText.Length > 0) {
				if(TryExtract(res, ref extendedText, "<!--", LexemType.Comment)) {
					TryExtractTo(res, ref extendedText, "-->", LexemType.Comment, null);
				}
				if(extendedText.StartsWith("<", StringComparison.Ordinal)) IsInsideBlock = false;
				if(TryExtract(res, ref extendedText, "\"{}", LexemType.Value))
					TryExtractTo(res, ref extendedText, "\"", LexemType.Value, null);
				if(TryExtract(res, ref extendedText, "</", LexemType.Symbol) ||
				   TryExtract(res, ref extendedText, "<", LexemType.Symbol) ||
				   TryExtract(res, ref extendedText, "{", LexemType.Symbol) ||
				   TryExtract(res, ref extendedText, "\"{", LexemType.Symbol)) {
					ParseXamlKeyWord(res, ref extendedText, LexemType.Object);
				}
				if(TryExtract(res, ref extendedText, "\"", LexemType.Value)) {
					TryExtractTo(res, ref extendedText, "\"", LexemType.Value, null);
				}
				ParseXamlKeyWord(res, ref extendedText, IsInsideBlock ? LexemType.Object : LexemType.Property);
				TryExtract(res, ref extendedText, "}\"", LexemType.Symbol);
				if(extendedText.StartsWith(">", StringComparison.Ordinal)) IsInsideBlock = true;
				ParseSymbol(res, ref extendedText, LexemType.Symbol);
				TrySpace(res, ref extendedText);
				TryExtract(res, ref extendedText, "\n", LexemType.LineBreak);
			}
			return res;
		}
		void ParseSymbol(List<CodeLexem> res, ref SourceString text, LexemType lt) {
			int index = text.IndexOfAny(XamlSymbol);
			if(index != 0) return;
			res.Add(new CodeLexem(LexemType.Symbol, text.Substring(0, 1)));
			text = text.Substring(1);
		}
		void ParseXamlKeyWord(List<CodeLexem> res, ref SourceString text, LexemType type) {
			int index = text.IndexOfAny(XamlEndOfTerm);
			if(index <= 0) return;
			int nsIndex = text.IndexOf(':');
			if(nsIndex > 0 && nsIndex < index) {
				res.Add(new CodeLexem(type, StringCut(ref text, nsIndex)));
				res.Add(new CodeLexem(LexemType.Symbol, StringCut(ref text, 1)));
				res.Add(new CodeLexem(type, StringCut(ref text, index - nsIndex - 1)));
			} else {
				res.Add(new CodeLexem(type, StringCut(ref text, index)));
			}
		}
	}
	class VBParser : BaseParser {
		char[] VBEndOfTerm = { ' ', '\t', '\n', '=', '/', '>', '<', '"', '{', '}', ',', '(', ')', ';', ':', '\0', '?' };
		string[] VBKeyWords = { "attribute","addhandler","andalso","byte","catch","cdate","cint","const",
								"csgn","culgn","declare","directcast","else","enum","exit",
								"friend","getxmlnamespace","handles","in","is","like","mod",
								"mybase","new","noinheritable","on","or","overrides","property",
								"readonly","resume","set","single","string","then","try",
								"ulong","wend","with","addressof","as","byval",
								"cbool","cdbl","class","continue","cstr","cushort","default",
								"do","elseif","erase","false","function","global","if",
								"inherits","isnot","long","module","myclass","next",
								"notoverridable","operator","orelse","paramarray","protected","redim","return",
								"shadows","static","structure","throw","trycast","ushort","when",
								"withevents","alias","boolean","call","cbyte","cdec","clng","csbyte",
								"ctype","date","delegate","double","end","error","finally","get",
								"gosub","implements","integer","let","loop","mustinherit","namespace",
								"not","object","option","overloads","partial","public","rem",
								"sbyte","shared","step","sub","to","typeof","using","while", "async", "await",
								"writeonly","and","byref","case","cchar","char","cobj","cshort",
								"cuint","decimal","dim","each","endif","event","for","gettype","goto","imports",
								"interface","lib","me","mustoverride","narrowing","nothing","of","optional",
								"overridable","private","raiseevent","removehandler","select","short","stop",
								"synclock","true","uinteger","variant","widening","xor" };
		public VBParser() : base(false) { }
		protected override List<CodeLexem> Parse(SourceString text) {
			SourceString extendedText = text;
			List<CodeLexem> res = new List<CodeLexem>();
			while(extendedText.Length > 0) {
				if(TryExtract(res, ref extendedText, "'", LexemType.Comment)) {
					TryExtractTo(res, ref extendedText, "\n", LexemType.Comment, null);
				}
				if(TryExtract(res, ref extendedText, "`", LexemType.Comment)) {
					TryExtractTo(res, ref extendedText, "\n", LexemType.Comment, null);
				}
				TryExtract(res, ref extendedText, "rem\n", LexemType.Comment);
				if(TryExtract(res, ref extendedText, "rem ", LexemType.Comment)) {
					TryExtractTo(res, ref extendedText, "\n", LexemType.Comment, null);
				}
				if(TryExtract(res, ref extendedText, "rem\t", LexemType.Comment)) {
					TryExtractTo(res, ref extendedText, "\n", LexemType.Comment, null);
				}
				if(TryExtract(res, ref extendedText, "\"", LexemType.String)) {
					TryExtractTo(res, ref extendedText, "\"", LexemType.String, "\"\"");
					TryExtract(res, ref extendedText, "c", LexemType.String);
				}
				ParseVBKeyWord(res, ref extendedText, LexemType.KeyWord);
				ParseVBSymbol(res, ref extendedText, LexemType.PlainText);
				TrySpace(res, ref extendedText);
				TryExtract(res, ref extendedText, "\n", LexemType.LineBreak);
			}
			return res;
		}
		int lastLength = -1;
		void ParseVBSymbol(List<CodeLexem> res, ref SourceString text, LexemType lt) {
			if(lastLength == -1 || lastLength != text.Length) {
				lastLength = text.Length;
				return;
			}
			CodeLexem cl = res.Count > 0 ? res.Last() : null;
			if(cl != null && cl.Type == LexemType.PlainText)
				cl.Text += StringCut(ref text, 1);
			else
				res.Add(new CodeLexem(LexemType.PlainText, StringCut(ref text, 1)));
		}
		void ParseVBKeyWord(List<CodeLexem> res, ref SourceString text, LexemType type) {
			int index = -1;
			if(!VBEndOfTerm.Contains(previousSimbol)) return;
			foreach(string str in VBKeyWords) {
				if(text.StartsWith(str, StringComparison.OrdinalIgnoreCase)) {
					if(!VBEndOfTerm.Contains(text[str.Length])) continue;
					index = str.Length;
					break;
				}
			}
			if(index < 0) return;
			res.Add(new CodeLexem(type, StringCut(ref text, index)));
		}
	}
}
