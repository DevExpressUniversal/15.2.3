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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class TypedElementReferenceExpression : ElementReferenceExpression, ITypedElementReferenceExpression
	{
		private char _TypeCharacter;
		private SourceRange _TypeCharacterRange;
		protected TypedElementReferenceExpression()
		{
		}
		public TypedElementReferenceExpression(string name, char typeCharacter, SourceRange typeCharacterRange) : base(name)
		{
			_TypeCharacter = typeCharacter;
			_TypeCharacterRange = typeCharacterRange;
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			base.CloneDataFrom (source, options);
	  TypedElementReferenceExpression sourceExpr = (TypedElementReferenceExpression)source;
			if (sourceExpr == null)
				return;
			_TypeCharacter = sourceExpr._TypeCharacter;
			_TypeCharacterRange = sourceExpr.TypeCharacterRange;
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _TypeCharacterRange = TypeCharacterRange;
	}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			TypedElementReferenceExpression lClone = new TypedElementReferenceExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion	
		public char TypeCharacter
		{
			get	{ return _TypeCharacter; }
			set
			{
				SetTypeChar(value);
			}
		}
		public void SetTypeChar(char ch)
		{
			_TypeCharacter = ch;
		}
		public SourceRange TypeCharacterRange
		{
			get	
	  { 
		return GetTransformedRange(_TypeCharacterRange); 
	  }
			set
			{
		ClearHistory();
				_TypeCharacterRange = value;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.TypedElementReferenceExpression;
			}
		}
	}	
}
