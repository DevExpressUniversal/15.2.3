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
	public class PrimitiveExpression : Expression, IPrimitiveExpression
	{
		const int INT_MaintainanceComplexity = 1;
		string _Literal = null;
		bool _IsVerbatimStringLiteral = false;
		PrimitiveType _PrimitiveType = PrimitiveType.Undefined;
		protected PrimitiveExpression()
		{			
		}
		public PrimitiveExpression(string value)
		{
			InternalName = value;			
		}
		public PrimitiveExpression(string value, SourceRange range)
			: this(value)
		{
			SetRange(range);
		}
		void InitializeValue(object value)
		{
			PrimitiveValue = value;
			PrimitiveType = PrimitiveTypeUtils.GetPrimitiveType(value);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is PrimitiveExpression))
				return;
			PrimitiveExpression lSource = (PrimitiveExpression)source;
			_PrimitiveType = lSource._PrimitiveType;
			_Literal = lSource.Literal;
			_IsVerbatimStringLiteral = lSource._IsVerbatimStringLiteral;
		}
		#endregion
		#region FromObject
		public static PrimitiveExpression FromObject(object value)
		{
			string lStringValue = String.Empty;
			if (value != null)
				lStringValue = value.ToString();
			PrimitiveExpression lResult = new PrimitiveExpression(lStringValue);
			lResult.InitializeValue(value);
			return lResult;
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			return InternalName;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.PrimitiveExpression;
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is PrimitiveExpression)
			{
				PrimitiveExpression lPrimitive = expression as PrimitiveExpression;
				return lPrimitive.Name == Name;
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
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			PrimitiveExpression lClone = new PrimitiveExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		protected override object EvaluateExpression()
		{
			return TestValue;
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.PrimitiveExpression;
			}
		}
		#region NeedsInvertParens
		public override bool NeedsInvertParens
		{
			get
			{
				return false;
			}
		}
		#endregion
		public bool IsNumberLiteral
		{
			get
			{
				if (PrimitiveValue != null)
					return PrimitiveValue is SByte || PrimitiveValue is Byte || PrimitiveValue is UInt16 ||
						PrimitiveValue is Int16 || PrimitiveValue is UInt32 || PrimitiveValue is Int32 ||
						PrimitiveValue is UInt64 || PrimitiveValue is Int64 || PrimitiveValue is Single ||
						PrimitiveValue is Double || PrimitiveValue is Decimal;
				return PrimitiveType == PrimitiveType.SByte || PrimitiveType == PrimitiveType.Byte || PrimitiveType == PrimitiveType.UInt16 ||
					PrimitiveType == PrimitiveType.Int16 || PrimitiveType == PrimitiveType.UInt32 || PrimitiveType == PrimitiveType.Int32 ||
					PrimitiveType == PrimitiveType.UInt64 || PrimitiveType == PrimitiveType.Int64 || PrimitiveType == PrimitiveType.Single ||
					PrimitiveType == PrimitiveType.Double || PrimitiveType == PrimitiveType.Decimal;
			}
		}
		public string Literal
		{
			get
			{
				return _Literal;
			}
			set
			{
				_Literal = value;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanHasEscapeSequence
		{
			get
			{
				return false;
			}
		}
		[Category("Expression Type")]
		[Description("Returns true if this primitive expression is boolean literal (true or false in C#).")]
		public bool IsBooleanLiteral
		{
			get
			{
				return PrimitiveType == PrimitiveType.Boolean;
			}
		}
		[Category("Expression Type")]
		[Description("Returns true if this primitive expression is string literal.")]
		public bool IsStringLiteral
		{
			get
			{
				return PrimitiveType == PrimitiveType.String;
			}
		}
		[Category("Expression Type")]
		[Description("Returns true if this primitive expression is char literal.")]
		public bool IsCharLiteral
		{
			get
			{
				return PrimitiveType == PrimitiveType.Char;
			}
		}
		[Category("Expression Type")]
		[Description("Returns true if this primitive expression is DateTime literal.")]
		public bool IsDateTime
		{
			get
			{
				if (PrimitiveValue != null)
					return PrimitiveValue is DateTime;
				return (PrimitiveType == PrimitiveType.DateTime);
			}
		}
		[Category("Expression Type")]
		[Description("Returns true if this primitive expression is null literal.")]
		public bool IsNullLiteral
		{
			get
			{
				return PrimitiveType == PrimitiveType.Void;
			}
		}
		[Category("Expression Type")]
		[Description("Gets or sets primitive type of this expression.")]
		public PrimitiveType PrimitiveType
		{
			get
			{
				return _PrimitiveType;
			}
			set
			{
				_PrimitiveType = value;
			}
		}
		[Description("Gets or sets primitive value of this expression.")]
		public object PrimitiveValue
		{
			get
			{
				return TestValue;
			}
			set
			{
				SetTestValue(value);
			}
		}
		[Category("Expression Type")]
		[Description("Gets expression type name.")]
		public override string ExpressionTypeName
		{
			get
			{
				return PrimitiveTypeUtils.GetFullTypeName(PrimitiveType);
			}
		}
		public override SourceRange NameRange
		{
			get
			{
				return InternalRange;
			}
		}
		public bool IsVerbatimStringLiteral
		{
			get
			{
				return _IsVerbatimStringLiteral;
			}
			set
			{
				_IsVerbatimStringLiteral = value;
			}
		}
		#region IPrimitiveExpression Members
		object IPrimitiveExpression.Value
		{
			get
			{
				return TestValue;
			}
		}
		PrimitiveType IPrimitiveExpression.PrimitiveType
		{
			get
			{
				return PrimitiveType;
			}
		}
	bool IPrimitiveExpression.IsVerbatimString
	{
	  get
	  {
		return IsVerbatimStringLiteral;
	  }
	}
		#endregion
	}
}
