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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class ParametrizedObjectCreationExpression : ObjectCreationExpression
	{
		ExpressionCollection _NewOperatorArguments;
		protected ParametrizedObjectCreationExpression()
		{
		}
		public ParametrizedObjectCreationExpression(TypeReferenceExpression type, ExpressionCollection ctorArguments, ExpressionCollection newArguments)
			: base (type, ctorArguments)
		{
			SetNewOperatorArguments(newArguments);
		}
	void SetNewOperatorArguments(ExpressionCollection newArguments)
	{
	  if (_NewOperatorArguments != null)
		RemoveDetailNodes(_NewOperatorArguments);
	  _NewOperatorArguments = newArguments;
	  if (_NewOperatorArguments != null)
		AddDetailNodes(_NewOperatorArguments);
	}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is ParametrizedObjectCreationExpression))
				return;
			ParametrizedObjectCreationExpression sourceElement = (ParametrizedObjectCreationExpression)source;
			if (sourceElement._NewOperatorArguments != null)
			{
				_NewOperatorArguments = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, sourceElement.DetailNodes, _NewOperatorArguments, sourceElement._NewOperatorArguments);
				if (_NewOperatorArguments.Count == 0 && sourceElement._NewOperatorArguments.Count > 0)
					_NewOperatorArguments = sourceElement._NewOperatorArguments.DeepClone(options) as ExpressionCollection;
			}
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_NewOperatorArguments != null && _NewOperatorArguments.Contains(oldElement as Expression))
				_NewOperatorArguments.ReplaceExpression(oldElement as Expression, newElement as Expression);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ParametrizedObjectCreationExpression expression = new ParametrizedObjectCreationExpression();
			expression.CloneDataFrom(this, options);
			return expression;
		}
		public override LanguageElementType ElementType
		{
	  get { return LanguageElementType.ParametrizedObjectCreationExpression; }
		}
		public ExpressionCollection NewOperatorArguments
		{
	  get { return _NewOperatorArguments; }
	  set { SetNewOperatorArguments(value); }
		}
	}
}
