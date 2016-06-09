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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class ExpressionSignatureBuilder
	{
		const char CHAR_LessThan = '<';
		const char CHAR_GreaterThan = '>';
		const string CHAR_Dot = ".";
		private ExpressionSignatureBuilder()
		{	
		}
		static void AppendGeneric(StringBuilder builder, IGenericExpression expr)
		{
			if (builder == null || expr == null || !expr.IsGeneric)
				return;
			builder.Append(CHAR_LessThan);
			ITypeReferenceExpressionCollection types = expr.TypeArguments;
			int count = types.Count;			
	  for (int i = 0; i < count; i++)
	  {
		builder.Append(BuildSignature(types[i]));
		if (i < count - 1)
		  builder.Append(", ");
	  }
			builder.Append(CHAR_GreaterThan);
		}
		static void AppendTypeReferenceModifiers(StringBuilder builder, ITypeReferenceExpression expr)
		{
			if (builder == null || expr == null || !expr.IsArrayType)
				return;
			builder.Append("[");
			for (int i = 0; i < expr.Rank - 1; i++)
				builder.Append(",");
			builder.Append("]");	
			if (expr.IsConst)
				builder.Append("const ");
			if (expr.IsVolatile)
				builder.Append("volatile ");
			if (expr.IsPointerType)
			{
				if (expr.IsManaged)
					builder.Append("^");
				else
					builder.Append("*");
			}
			if (expr.IsReferenceType)
			{
				if (expr.IsManaged)
					builder.Append("%");
				else
					builder.Append("&");
			}
			if (expr.IsNullable)
				builder.Append("?");
		}
		static void AppendExpressionsSignature(StringBuilder builder, IExpressionCollection expressions)
		{
			if (builder == null || expressions == null)
				return;
			int count = expressions.Count;
			for (int i = 0; i < count; i++)
			{
				IExpression expr = expressions[i];
				if (expr == null)
					continue;
				builder.Append(GetSignature(expr));
				if (i < count - 1)
					builder.Append(", ");
			}
		}
		static void AppendArgumentsSignature(StringBuilder builder, IMethodCallExpression methodCall)
		{
			if (builder == null || methodCall == null)
				return;
			builder.Append("(");
			AppendExpressionsSignature(builder, methodCall.Arguments);			
			builder.Append(")");
		}
		static string BuildSignature(IExpression expr)
		{
			if (expr == null)
				return String.Empty;
			StringBuilder builder = new StringBuilder();
			if (expr is IWithSource)
			{
				IWithSource withSource = (IWithSource)expr;
				if (withSource.Source != null)
				{
					builder.Append(BuildSignature(withSource.Source));
		  ITypeReferenceExpression typeReference = expr as ITypeReferenceExpression;
		  if (typeReference == null || !typeReference.IsArrayType)
					  builder.Append(CHAR_Dot);
				}
			}
			if (expr is IMethodCallExpression)
				AppendArgumentsSignature(builder, expr as IMethodCallExpression);
			else
				builder.Append(expr.Name);
			AppendGeneric(builder, expr as IGenericExpression);
			AppendTypeReferenceModifiers(builder, expr as ITypeReferenceExpression);
			return builder.ToString();
		}
		#region GetSignature
		[EditorBrowsable(EditorBrowsableState.Never)]		
		public static string GetSignature(IExpression expr)
		{
			if (expr == null)
				return String.Empty;
			return BuildSignature(expr);
		}
		#endregion
	}
}
