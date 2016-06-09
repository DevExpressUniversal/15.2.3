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
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	#region LetExpression
	public class LetExpression : QueryExpressionBase
	{
		LanguageElementCollection _Declarations;
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is LetExpression))
				return;
			LetExpression lSource = (LetExpression)source;
			if (lSource._Declarations != null)
			{
				_Declarations = new LanguageElementCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Declarations, lSource._Declarations);
				if (_Declarations.Count == 0 && lSource._Declarations.Count > 0)
				{
					for (int i = 0; i < lSource._Declarations.Count; i++)
					{
						LanguageElement element = lSource._Declarations[i];
						_Declarations.Add(element.Clone());
					}
				}
			}
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Declarations != null && _Declarations.Contains(oldElement))
				_Declarations.Replace(oldElement, newElement);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			LetExpression lClone = new LetExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
	public void AddDeclarations(IEnumerable<LanguageElement> declarations)
	{
	  if (declarations == null)
		return;
	  foreach (LanguageElement declaration in declarations)
		AddDeclaration(declaration);
	}
		public void AddDeclaration(LanguageElement element)
		{
			if (element == null)
				return;
			Declarations.Add(element);
			AddDetailNode(element);
		}	
		public LanguageElementCollection Declarations
		{
			get
			{
				if (_Declarations == null)
				{
					_Declarations = new LanguageElementCollection();
				}
				return _Declarations;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.LetExpression;
			}
		}
		public override string ToString()
		{
				return "LetExpression";
		}
	}
	#endregion
}
