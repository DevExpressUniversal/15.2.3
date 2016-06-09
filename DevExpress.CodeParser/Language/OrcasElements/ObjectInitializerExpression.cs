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
	public class ObjectInitializerExpression : Expression, IObjectInitializerExpression
	{
		ExpressionCollection _Initializers = null;
		#region ComareInitializers
		bool ComareInitializers(ObjectInitializerExpression expression)
		{
			if (expression == null)
				return false;
			if (Initializers == null && expression.Initializers == null)
				return true;
			if (Initializers == null)
				return false;
			return Initializers.IsIdenticalTo(expression.Initializers);
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is ObjectInitializerExpression))
				return;
			ObjectInitializerExpression lSource = (ObjectInitializerExpression)source;
			if (lSource._Initializers != null)
			{
				_Initializers = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Initializers, lSource._Initializers);
				if (_Initializers.Count == 0 && lSource._Initializers.Count > 0)
					_Initializers = lSource._Initializers.DeepClone(options) as ExpressionCollection;
			}				
		}
		#endregion
	protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
	{
	  if (_Initializers != null && _Initializers.Contains(oldElement as Expression))
		_Initializers.ReplaceExpression(oldElement as Expression, newElement as Expression);
	  else
		base.ReplaceOwnedReference(oldElement, newElement);
	}
		public override IElement Resolve(ISourceTreeResolver resolver)
		{
			return null;
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ObjectInitializerExpression lClone = new ObjectInitializerExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			ObjectInitializerExpression objectInit = expression as ObjectInitializerExpression;
			if (objectInit != null)
				return ComareInitializers(objectInit);
			return false;
		}
		#endregion
	public void AddInitializers(IEnumerable<Expression> initializers)
	{
	  if (initializers == null)
		return;
	  foreach (Expression initializer in initializers)
		AddInitializer(initializer);
	}
	public void AddInitializer(Expression init)
	{
	  if (init == null)
		return;
	  Initializers.Add(init);
	  AddDetailNode(init);
	}
		public ExpressionCollection Initializers
		{
			get
			{
				if (_Initializers == null)
					_Initializers = new ExpressionCollection();
				return _Initializers;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.ObjectInitializerExpression;
			}
		}
		IExpressionCollection IObjectInitializerExpression.Initializers
		{
			get
			{
				return Initializers;
			}
		}
	}
}
