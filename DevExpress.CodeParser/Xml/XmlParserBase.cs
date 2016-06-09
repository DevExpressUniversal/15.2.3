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
using System.Text;
using System.Collections;
using System.Collections.Specialized;
#if SL
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.Xml
#else
namespace DevExpress.CodeParser.Xml
#endif
{
	public abstract class XmlParserBase : XmlLanguageParserBase, IXmlDocParser
	{
		XmlNode GetXmlNode(LanguageElement root)
		{
			if (root == null)
				return null;
			if (root is XmlNode)
				return (root as XmlNode);
			else
				if (root is SourceFile)
			{
				for (int i = 0; i < root.NodeCount; i++)
					if (root.Nodes[i] is XmlNode)
						return  (root.Nodes[i] as XmlNode);
			}
			return null;
		}
		void FillArrayListWithNodes(ArrayList list, LanguageElement root)
		{
			if (root is SourceFile)
			{
				for (int i = 0; i < root.NodeCount; i++)
					list.Add(root.Nodes[i]);
			}
			else
				list.Add(root);
		}		
		string GetTextFromSourceLinesReader(SourceLinesReader linesReader)
		{
			StringBuilder builder = new StringBuilder();
			if ((linesReader.SourceLines == null) || (linesReader.SourceLines.Count == 0))
			{
				return string.Empty;
			}
			int prevLine = -1;
			for (int i = 0; i < linesReader.LineCount; i++)
			{
				SourcePoint lineStart = linesReader.SourceLines[i].Start;
				if (prevLine != -1 && (lineStart.Line - prevLine) > 1)
					for (int j = 0; j < lineStart.Line - prevLine -1; j++)
						builder.Append("\r\n");
				if (i > 0 && lineStart.Offset != 1)
					for (int j = 0; j < lineStart.Offset - 1; j++)
						builder.Append(' ');
				builder.Append(linesReader.SourceLines[i].Text);
				prevLine = lineStart.Line;
			}
			return builder.ToString();
		}
	protected override TokenCategory GetTokenCategory(CategorizedToken token)
	{
	  if (token.Type == Tokens.EOF)
		return TokenCategory.Unknown;
	  return TokenCategory.Text;
	}
		protected abstract LanguageElement Parse(string code, int startLine, int startColumn);
		public XmlNode ParseXmlDocNode(ISourceReader reader)
		{
			if (reader == null || !(reader is SourceLinesReader))
				return null;
			SourceLinesReader linesReader = reader as SourceLinesReader;
			string xmlText = GetTextFromSourceLinesReader(linesReader);
			LanguageElement root = Parse(xmlText, linesReader.StartLine, linesReader.StartColumn);
			return GetXmlNode(root);
		}
		public XmlNode ParseXmlDocNode(string xmlDoc)
		{
			if (xmlDoc == null || xmlDoc.Length == 0)
				return null;
			LanguageElement root = Parse(xmlDoc, 1, 1);
			return GetXmlNode(root);
		}
		public ArrayList ParseXmlDocNodes(ISourceReader reader)
		{
			ArrayList result = new ArrayList();
			if (reader == null || !(reader is SourceLinesReader))
				return result;
			SourceLinesReader linesReader = reader as SourceLinesReader;
			string xmlText = GetTextFromSourceLinesReader(linesReader);
			LanguageElement root = Parse(xmlText, linesReader.StartLine, linesReader.StartColumn);
			if (root == null)
				return result;
			FillArrayListWithNodes(result, root);
			return result;
		}
		public ArrayList ParseXmlDocNodes(string xmlDoc)
		{
			ArrayList result = new ArrayList();
			if (xmlDoc == null || xmlDoc.Length == 0)
				return result;
			LanguageElement root = Parse(xmlDoc, 1, 1);
			if (root == null)
				return result;
			FillArrayListWithNodes(result, root);
			return result;
		}
		protected XmlScanner XmlScanner
		{
			get
			{
				return (XmlScanner)scanner;
			}
		}
	}
}
