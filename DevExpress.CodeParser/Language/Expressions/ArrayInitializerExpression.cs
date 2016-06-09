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
using System.ComponentModel;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class ArrayInitializerExpression : Expression, IArrayInitializerExpression
	{
		const int INT_MaintainanceComplexity = 2;
		#region private fields...
	ExpressionCollection _Initializers;
		#endregion
		#region ArrayInitializerExpression
		public ArrayInitializerExpression()
		{
		}
		#endregion
		#region ArrayInitializerExpression
		public ArrayInitializerExpression(ExpressionCollection initializers)
		{
	  AddInitializers(initializers);
		}
		#endregion
	void SetInitializers(ExpressionCollection initializers)
	{
	  if (_Initializers != null)
		RemoveDetailNodes(_Initializers);
	  _Initializers = initializers;
	  if (_Initializers != null)
		AddDetailNodes(_Initializers);
	}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is ArrayInitializerExpression))
				return;
			ArrayInitializerExpression lSource = (ArrayInitializerExpression)source;
			if (lSource._Initializers != null)
			{
				_Initializers = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Initializers, lSource._Initializers);
				if (_Initializers.Count == 0 && lSource._Initializers.Count > 0)
					_Initializers = lSource._Initializers.DeepClone(options) as ExpressionCollection;
			}
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			StringBuilder lResult = new StringBuilder();
			if (_Initializers != null)
			{
				lResult.Append("{");
				string lComma = String.Empty;
				for (int i = 0; i < _Initializers.Count; i++)
				{
			lResult.AppendFormat("{0}{1}", lComma, _Initializers[i].ToString());
					lComma = ", ";
				}
				lResult.Append("}");
			}
			return lResult.ToString();
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is ArrayInitializerExpression)
			{
				ArrayInitializerExpression lExpression = expression as ArrayInitializerExpression;
				if (Initializers == null && lExpression.Initializers == null)
					return true;
				if (Initializers == null)
					return false;
				return Initializers.IsIdenticalTo(lExpression.Initializers);
			}
			return false;
		}
		#endregion
		#region Resolve
		public override IElement Resolve(ISourceTreeResolver resolver)
		{
	  if (resolver != null)
		return resolver.Resolve(this);
			return null;
		}
		#endregion
		#region CleanUpOwnedReferences
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
	  _Initializers = null;
			base.CleanUpOwnedReferences();
		}
		#endregion
		#region ReplaceOwnedReference
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Initializers != null)
				_Initializers.ReplaceExpression(oldElement as Expression, newElement as Expression);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ArrayInitializerExpression lClone = new ArrayInitializerExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
	public void AddInitializer(Expression initializer)
	{
	  Initializers.Add(initializer);
	  AddDetailNode(initializer);
	}
	public void AddInitializers(IEnumerable<Expression> initializers)
	{
	  if (initializers == null)
		return;
	  foreach (Expression initializer in initializers)
		AddInitializer(initializer);
	}
		#region ThisMaintenanceComplexity
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		#endregion
		#region ElementType
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.ArrayInitializerExpression;
			}
		}
		#endregion
		#region Initializers
		public ExpressionCollection Initializers
		{
			get
			{
		if (_Initializers == null)
		  _Initializers = new ExpressionCollection();
				return _Initializers;
			}
	  set { SetInitializers(value); }
		}
		#endregion
		#region IArrayInitializerExpression Members
		IExpressionCollection IArrayInitializerExpression.Initializers
		{
			get
			{
				if (_Initializers == null)
					return EmptyLiteElements.EmptyIExpressionCollection;
				return _Initializers;
			}
		}
		#endregion
	}
}
