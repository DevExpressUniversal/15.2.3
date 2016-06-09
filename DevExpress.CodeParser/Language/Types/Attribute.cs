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

using System.Text;
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public enum AttributeTargetType : byte
	{
		None,
		Assembly,
		Field,
		Event,
		Method,
		Module,
		Param,
		Property,
		Return,
		Type,
		Class,
		Constructor,
		Delegate,
		Enum,
		Interface,
		Struct
	}
  public class Attribute : SupportElement, IHasQualifier, IAttributeElement, IWithArgumentsModifier, IHasParens
	{
		const int INT_MaintainanceComplexity = 3;
		TextRange _NameRange;
	TextRange _ParensRange;
		Expression _Qualifier;
		ExpressionCollection _Arguments;
	#region protected fields...
	  AttributeTargetType _TargetType;
	#endregion
		#region Attribute
		public Attribute()
		{
	  _TargetType = AttributeTargetType.None;
		}
		#endregion
		#region SetQualifier
		void SetQualifier(Expression qualifier)
		{
			if (_Qualifier == null)
			{
				_Qualifier = qualifier;
				AddDetailNode(qualifier);
			}
			else
				ReplaceDetailNode(_Qualifier, qualifier);
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Attribute))
				return;
			Attribute lSource = (Attribute)source;			
	  _NameRange = lSource.NameRange;
			_TargetType = lSource._TargetType;
			if (lSource._Qualifier != null)
			{
				_Qualifier = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Qualifier) as Expression;
				if (_Qualifier == null)
					_Qualifier = lSource._Qualifier.Clone(options) as Expression;
			}
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
	  _NameRange = NameRange;
	  _ParensRange = ParensRange;
	}
		#region ReplaceOwnedReference
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Arguments != null && _Arguments.Contains(oldElement))
				_Arguments.Replace(oldElement, newElement);
			else if (_Qualifier != null && _Qualifier == oldElement)
				_Qualifier = newElement as Expression;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.Attribute;
		}
		#endregion
	#region ToString
	  public override string ToString()
	  {
	  StringBuilder lResult = new StringBuilder();
	  if (_TargetType != AttributeTargetType.None)
		lResult.Append(_TargetType.ToString() + ": ");
			lResult.Append(InternalName);
		return lResult.ToString();
	  }
	  #endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Attribute lClone = new Attribute();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion		
	public void AddArgument(Expression argument)
	{
	  if (argument == null)
		return;
	  Arguments.Add(argument);
	  AddDetailNode(argument);
	}
	public void RemoveArgument(Expression argument)
	{
	  if (argument == null)
		return;
	  Arguments.Remove(argument);
	  RemoveDetailNode(argument);
	}
	public void InsertArgument(int index, Expression argument)
	{
	  if (argument == null)
		return;
	  Arguments.Insert(index, argument);
	  InsertDetailNode(index, argument);
	}
		#region GetTargetTypeFromName
		public static AttributeTargetType GetTargetTypeFromName(string name)
		{
			switch (name.ToLower())
			{
				case "assembly":
					return AttributeTargetType.Assembly;
				case "field":
					return AttributeTargetType.Field;
				case "event":
					return AttributeTargetType.Event;
				case "method":
					return AttributeTargetType.Method;
				case "module":
					return AttributeTargetType.Module;
				case "param":
					return AttributeTargetType.Param;
				case "property":
					return AttributeTargetType.Property;
				case "return":
					return AttributeTargetType.Return;
				case "type":
					return AttributeTargetType.Type;
			}
			return AttributeTargetType.None;
		}
		#endregion
		#region ThisMaintenanceComplexity
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		#endregion
		#region Qualifier
		public Expression Qualifier
		{
			get
			{
				return _Qualifier;
			}
			set
			{
				SetQualifier(value);
			}
		}
		#endregion
		#region Source
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Expression Source
		{
			get
			{
				return Qualifier;
			}
			set
			{
				Qualifier = value;
			}
		}
		#endregion
		#region ArgumentCount
		public int ArgumentCount
		{
			get
			{
				if (_Arguments == null)
					return 0;
				return _Arguments.Count;
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
				if (_Arguments == value)
					return;
				_Arguments = value;
			}
		}
		#endregion		
	  #region TargetType
	  public AttributeTargetType TargetType
	  {
		get
		{
		  return _TargetType;
 		}
			set
			{
				_TargetType = value;
			}
 	  }
	  #endregion
		#region ElementType
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Attribute;
			}
		}
		#endregion
		#region NameRange
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
		#region IAttributeElement Members
	bool IAttributeElement.HasTargetNode 
	{
	  get 
	  {
		return TargetNode != null;
	  }
	}
		IExpressionCollection IAttributeElement.Arguments
		{
			get
			{
				if (_Arguments == null)
					return EmptyLiteElements.EmptyIExpressionCollection;
				return _Arguments;
			}
		}
		IExpression IAttributeElement.Qualifier
		{
			get
			{
				return Qualifier;
			}
		}
		IExpression IWithSource.Source
		{
			get
			{
				return Qualifier;
			}
		}
		#endregion
	#region IWithArguments Members
	IExpressionCollection IWithArguments.Args
	{
	  get
	  {
		return Arguments;
	  }
	}
	#endregion
	#region IWithArgumentsModifier Members
	void IWithArgumentsModifier.AddArgument(IExpression argument)
	{
	  AddArgument(argument as Expression);
	}
	void IWithArgumentsModifier.InsertArgument(int index, IExpression argument)
	{
	  InsertArgument(index, argument as Expression);
	}
	void IWithArgumentsModifier.RemoveArgument(IExpression argument)
	{
	  RemoveArgument(argument as Expression);
	}
	#endregion
  }
}
