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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class ElseIf : If, IElseIfStatement
	{
		public ElseIf()
		{
		}
		public ElseIf(Expression expression, LanguageElementCollection block)
			: base(expression, block)
		{
		}
		public ElseIf(Expression expression, LanguageElementCollection block, SourceRange range)
			: base(expression, block, range)
		{
		}
		void SetContextBlockEnd(Token token)
		{
			if (token == null)
				return;
			LanguageElement lPrevElement = PreviousCodeSibling;
			if (!(lPrevElement is DelimiterCapableBlock))
				return;
			DelimiterCapableBlock lParent = (DelimiterCapableBlock)lPrevElement;
			lParent.SetBlockEnd(token.Range);
		}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.ElseBlock;
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			return "ElseIf";
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ElseIf lClone = new ElseIf();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.ElseIf;
			}
		}
		public override bool CompletesPrevious
		{
			get 
			{
				return true;
			}
		}	
	}
}
