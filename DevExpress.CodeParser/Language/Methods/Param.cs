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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{	
	public class Param : BaseVariable, IParameterElement, IBaseVariable, IParameterElementModifier
	{
		private const int INT_InParamMaintainanceComplexity = 1;
		private const int INT_RefOutParamMaintainanceComplexity = 2;
		#region private fields...
		ArgumentDirection _Direction;
		bool _IsOptional;
		string _DefaultValue = String.Empty;
		Expression _DefaultValueExpression;
	SourceRange _DirectionRange;
		#endregion
		#region Param
		public Param()
		{				
		}
		#endregion
		#region Param
		public Param(string name)
			: this ()
		{
			InternalName = name;
		}
		#endregion
		#region Param
		public Param(string type, string name)
			: this (name)
		{
			MemberType = type;
		}
		#endregion
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				if (IsReferenceParam || IsOutParam)
					return INT_RefOutParamMaintainanceComplexity;
				return INT_InParamMaintainanceComplexity;
			}
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Param))
				return;			
			Param lSource = (Param)source;
			if (lSource.DefaultValueExpression != null)
				_DefaultValueExpression = lSource.DefaultValueExpression.Clone() as Expression;
			_IsOptional = lSource._IsOptional;
			_DefaultValue = lSource._DefaultValue;
			_Direction = lSource._Direction;			
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			if (IsParamArray)
		return ImageIndex.ParamArray;
			else if (IsOutParam)
	  	return ImageIndex.ParamOut;
			else if (IsReferenceParam)
				return ImageIndex.ParamRef;
			else		
				return ImageIndex.ParamIn;
		}
		#endregion
		#region GetTypeName
		public override string GetTypeName()
		{
			return ParamType;
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Param lClone = new Param();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public new void SetFullTypeName(string fullTypeName)
		{
			base.SetFullTypeName(fullTypeName);
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Parameter;
			}
		}
		public override bool DeclaresIdentifier
		{
			get
			{
				return true;
			}
		}		
		#region ParamType
		[Description("The type of this parameter.")]
	[Category("Details")]
	public string ParamType
	{
	  get
	  {
		return MemberType;
	  }
			set
			{
				MemberType = value;
			}
	}
	#endregion
		#region DefaultValue
		[Description("The default value for this parameter, if specified.")]
		[Category("Details")]
		public string DefaultValue
		{
			get
			{
				return _DefaultValue;
			}
			set
			{
				_DefaultValue = value;
			}
		}
		#endregion
		#region Direction
		[Description("Gets or sets parameter direction (in/out/ref).")]
		[Category("Details")]
		public ArgumentDirection Direction
		{
			get
			{
				return _Direction;
			}
			set
			{
				_Direction = value;
			}
		}
		#endregion
	public SourceRange DirectionRange
	{
	  get
	  {
		return _DirectionRange;
	  }
	  set
	  {
		_DirectionRange = value;
	  }
	}
		#region IsOptional
		[Description("True if this parameter is optional.")]
		[Category("Details")]
		public bool IsOptional
		{
			get
			{
				return _IsOptional;
			}
			set
			{
				_IsOptional = value;
			}
		}
		#endregion
		#region IsByVal
		[Description("True if this parameter is passed by value.")]
		[Category("Details")]
		public bool IsByVal
		{
			get
			{
				return Direction == ArgumentDirection.In ||
							 Direction == ArgumentDirection.ParamArray;
			}
		}
		#endregion
		#region IsReferenceParam
		[Description("True if this parameter is passed by reference.")]
		[Category("Details")]
		public bool IsReferenceParam
		{
			get
			{
				return Direction == ArgumentDirection.Ref;
			}
		}
		#endregion
		#region IsOutParam
		[Description("True if this is an out parameter.")]
		[Category("Details")]
		public bool IsOutParam
		{
			get
			{
				return Direction == ArgumentDirection.Out;
			}
		}
		#endregion
		#region IsParamArray
		[Description("True if this parameter is an array of objects.")]
		[Category("Details")]
		public bool IsParamArray
		{
			get
			{
				return Direction == ArgumentDirection.ParamArray;
			}
		}
		#endregion
		#region IBaseVariable Members
		bool IBaseVariable.IsParameter
		{
			get
			{
				return true;
			}
		}
		#endregion
	#region IParameterElement Members
	IExpression IParameterElement.DefaultValue
	{
	  get
	  {
		return DefaultValueExpression;
	  }
	}
	#endregion
	#region IParameterElementModifier Members
	void IParameterElementModifier.SetType(ITypeReferenceExpression type)
		{
			TypeReferenceExpression typeRef = type as TypeReferenceExpression;
			MemberTypeReference = typeRef;
			AddDetailNode(typeRef);
		}
	void IParameterElementModifier.SetDirection(ArgumentDirection direction)
	{
	  Direction = direction;
	}
		#endregion
		#region DefaultValueExpression
		public Expression DefaultValueExpression
		{
			get
			{
				return _DefaultValueExpression;
			}
			set
			{
				_DefaultValueExpression = value;
			}
		}
		#endregion
	public bool IsArgList
	{
	  get
	  {
		return Direction == ArgumentDirection.ArgList;
	  }
	}
	}
}
