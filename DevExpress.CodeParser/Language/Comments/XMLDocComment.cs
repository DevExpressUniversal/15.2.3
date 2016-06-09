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
using System.Collections;
#if SL
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  using Xml;
  public class XmlDocComment : Comment, IFormattingElement
	{
	#region XmlDocComment
		public XmlDocComment()
	{
	}
		#endregion
	#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);			
		}
		#endregion
	#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.DocComment;	
		}
		#endregion
	#region SplitDocComment
		public virtual SourceLineCollection SplitDocComment()
		{
			SourceLineCollection lResult = new SourceLineCollection();
			string[] lLines = StringHelper.SplitLines(InternalName);
			if (lLines == null)
				return lResult;
			for (int i = 0; i < lLines.Length; i++)
			{
				string originalLine = lLines[i];
				if (originalLine == null)
					continue;
				string line = originalLine.Trim();
				if (line.Length == 0)
					continue;
				string lLineText = originalLine + "\r\n";
				int lMinPos = 0;
				int lDelta = 1;
				if (line.StartsWith("/") || line.StartsWith("*"))
				{
					int lPos1 = lLineText.IndexOf("/");
					int lPos2 = lLineText.IndexOf("*");					
					if (lPos1 < 0 && lPos2 < 0)
						continue;
					if (lPos1 < 0)
						lMinPos = lPos2;
					else if (lPos2 < 0)
						lMinPos = lPos1;
					else
						lMinPos = Math.Min(lPos1, lPos2);					
					if (lPos1 == lMinPos)
						lDelta = 3;
				}
				else
					lDelta = 0;
				int lLine = StartLine + i;
				int lNewIndex = lMinPos + lDelta;
				int lOffset = lNewIndex;
				if (i == 0)
					lOffset += StartOffset;
				else
					lOffset = lOffset + 1;
				SourceLine lSourceLine = new SourceLine(lLine, lOffset, lLineText.Substring(lNewIndex));
				lResult.Add(lSourceLine);
			}
			return lResult;
	}
	#endregion
	#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
	  XmlDocComment clone = options.GetClonedElement(this) as XmlDocComment;
	  if (clone == null)
	  {
		clone = new XmlDocComment();
		clone.CloneDataFrom(this, options);
		if (options != null)
		  options.AddClonedElement(this, clone);
	  }
	  if (options != null && options.SourceFile != null)
		options.SourceFile.AddXmlDocComment(clone);
	  return clone;
		}
		#endregion
	#region ParseXmlDocComment
	private void ParseXmlDocComment()
	{
	  if (!Name.Contains("<") || !Name.Contains(">"))
		return;
	  SourceLineCollection lines = SplitDocComment();
	  ISourceReader  reader = new SourceLinesReader(lines);
	  IXmlDocParser xmlDocParser = new NewXmlParser();
	  ArrayList nodes = xmlDocParser.ParseXmlDocNodes(reader);
	  if (nodes == null)
		return;
	  foreach (LanguageElement element in nodes)
		AddNode(element);
	}
	#endregion
	#region ParseXmlDoc
	public virtual XmlDocComment ParseXmlNodes()
		{
	  ParseXmlDocComment();
			return this;
		}
		#endregion
	#region ElementType
	public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.XmlDocComment;
			}
	}
	#endregion
	#region MultiLine
		public bool MultiLine
		{
		  get
		  {
		return CommentType == CommentType.MultiLine;
		  }
		  set
		  {
			if (value)
					CommentType = CommentType.MultiLine;
				else
					CommentType = CommentType.SingleLine;	
		  }
		}
		#endregion
	}
}
