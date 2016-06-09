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

#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
  using BaseXMLDocComment = XmlDocComment;
	public class XMLDocComment : BaseXMLDocComment
	{
		public XMLDocComment()
		{
		}
		public override SourceLineCollection SplitDocComment()
		{
			SourceLineCollection lResult = new SourceLineCollection();
			string[] lLines = StringHelper.SplitLines(InternalName);
			if (lLines == null)
				return lResult;
			for (int i=0; i<lLines.Length; i++)
			{
				string lLineText = lLines[i] + "\r\n";
				int lPos = lLineText.IndexOf("'");
				int lDelta = 3;
				int lLine = StartLine + i;
				int lNewIndex = lPos + lDelta;
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
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XMLDocComment lClone = new XMLDocComment();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
	}
}
