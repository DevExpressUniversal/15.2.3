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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public enum InitializerTarget : byte
	{
		Ancestor,
		ThisClass,
		Expression
	}
	public class ConstructorInitializer : CodeElement, IConstructorInitializerElement, IHasArguments, IHasParens
	{
		#region private fields...
		InitializerTarget _Target;
		ExpressionCollection _Arguments;
		SourceRange _ParensRange;
		SourceRange _NameRange;
		#endregion
		#region ConstructorInitializer
		public ConstructorInitializer()
		{
			_Target = InitializerTarget.ThisClass;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.Constructor;
		}
		#endregion
		#region CleanUpOwnedReferences
		public override void CleanUpOwnedReferences()
		{
			if (_Arguments != null)
			{
				_Arguments.Clear();
				_Arguments = null;
			}
			base.CleanUpOwnedReferences();
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Arguments != null && _Arguments.Contains(oldElement))
				_Arguments.Replace(oldElement, newElement);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region SetIsBase(bool value)
		protected void SetIsBase(bool value)
		{
			if (value)
				Target = InitializerTarget.Ancestor;
			else
				Target = InitializerTarget.ThisClass;
		}
		#endregion
		#region SetIsThis(bool value)
		protected void SetIsThis(bool value)
		{
			if (value)
				Target = InitializerTarget.ThisClass;
			else
				Target = InitializerTarget.Ancestor;
		}
		#endregion
		#region SetArguments
		protected void SetArguments(ExpressionCollection arguments)
		{
			_Arguments = arguments;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is ConstructorInitializer))
				return;
			ConstructorInitializer lSource = (ConstructorInitializer)source;
			_Target = lSource._Target;			
			_ParensRange = lSource.ParensRange;
			_NameRange = lSource.NameRange;
			if (lSource._Arguments != null)
			{
				_Arguments = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Arguments, lSource._Arguments);
				if (_Arguments.Count == 0 && lSource._Arguments.Count > 0)
					_Arguments = lSource._Arguments.DeepClone(options) as ExpressionCollection;
			}			
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _ParensRange = ParensRange;
	  _NameRange = NameRange;
	}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ConstructorInitializer lClone = new ConstructorInitializer();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region IsBase
		public bool IsBase
		{
			get
			{
				return Target == InitializerTarget.Ancestor;
			}
		}
		#endregion
		#region IsThis
		public bool IsThis
		{
			get
			{
				return Target == InitializerTarget.ThisClass;
			}
		}
		#endregion
		#region Arguments
		public ExpressionCollection Arguments
		{
			get
			{
				if (_Arguments == null)
					_Arguments = new ExpressionCollection();
				return _Arguments;
			}
			set
			{
				SetArguments(value);
			}
		}
		#endregion
		#region Target
		public InitializerTarget Target
		{
			get
			{
				return _Target;
			}
			set
			{
				if (_Target == value)
					return;
				_Target = value;
			}
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.ConstructorInitializer;
			}
		}
		#region IConstructorInitializerElement Members
		InitializerTarget IConstructorInitializerElement.Target
		{
			get
			{				
				return Target;
			}
		}
		IExpressionCollection IConstructorInitializerElement.Arguments
		{
			get
			{
				if (Arguments == null)
					return EmptyLiteElements.EmptyIExpressionCollection;
				return Arguments;
			}
		}
		IExpression IConstructorInitializerElement.Expression
		{
			get
			{
				if (NodeCount == 0)
					return null;
				return Nodes[0] as IExpression;
			}
		}
		#endregion
	#region IHasArguments Members
	public void AddArgument(Expression arg)
	{
	  Arguments.Add(arg);
	  AddDetailNode(arg);
	}
	public int ArgumentsCount
	{
	  get
	  {
		if (_Arguments == null)
		  return 0;
		return _Arguments.Count;
	  }
	}
	#endregion
		public SourceRange ParensRange
		{
			get
			{
				return GetTransformedRange(_ParensRange);
			}
			set
			{
		ClearHistory();
				_ParensRange = value;
			}
		}
		public override SourceRange NameRange
		{
			get
			{
				return GetTransformedRange(_NameRange);
			}
			set
			{
		ClearHistory();
				_NameRange = value;
			}
		}
  }
}
