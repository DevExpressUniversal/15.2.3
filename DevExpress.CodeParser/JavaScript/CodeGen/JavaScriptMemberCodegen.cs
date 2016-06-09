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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.JavaScript
#else
namespace DevExpress.CodeParser.JavaScript
#endif
{
	public class JavaScriptMemberCodeGen : MemberCodeGenBase
	{
		public JavaScriptMemberCodeGen(CodeGen codeGen)
			: base(codeGen)
		{
		}
		# region  GenerateCode
		public override void GenerateCode(CodeWriter writer, LanguageElement languageElement, bool calculateIndent)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			if (CodeGen == null)
				throw new ArgumentNullException("CodeGen is Null");
			PushCodeWriter();
			SetCodeWriter(writer);
			if (calculateIndent)
				CalculateIndent(languageElement);
			CodeGen.GenerateElement(languageElement);
			if (calculateIndent)
				ResetIndent();
			PopCodeWriter();
		}
		#endregion
		#region GenerateVariableStart
		private void GenerateVariableStart(Variable member)
		{
			if (member.PreviousVariable == null)
		Write(FormattingTokenType.Var);
			Write(FormattingTokenType.Ident);
		}
		#endregion
		#region GenerateVariableEnd
	void GenerateVariableEnd(Variable member)
	{
	  Variable nextMember = member.NextVariable;
	  if (nextMember == null)
	  {
		if (!member.IsDetailNode)
		  Write(FormattingTokenType.Semicolon);
		return;
	  }
	  if (member.PreviousVariable != null)
		return;
	  while (nextMember != null)
	  {
		Write(FormattingTokenType.Comma);
		CodeGen.AddSkiped(nextMember);
		CodeGen.GenerateElement(nextMember);
		nextMember = nextMember.NextVariable;
	  }
	}
		#endregion
		#region Methods for generation members, which are'nt present in JavaScript
		protected override void GenerateConst(Const member)
		{
		}
		protected override void GenerateConstVolatile(ConstVolatile member)
		{
		}
		protected override void GenerateDelegate(DelegateDefinition member)
		{
		}
		protected override void GenerateEnumElement(EnumElement member)
		{
		}
		protected override void GenerateEvent(Event member)
		{
		}
		protected override void GenerateExtensionMethodParam(ExtensionMethodParam member)
		{
		}
		public override void GenerateGenericModifier(GenericModifier generic)
		{
		}
		protected override void GenerateInitializedVolatile(InitializedVolatile member)
		{
		}
		protected override void GenerateMethodPrototype(MethodPrototype member)
		{
		}
		protected override void GenerateProperty(Property member)
		{
		}
		protected override void GenerateQueryIdent(QueryIdent expression)
		{
		}
		public override void GenerateTypeParameter(TypeParameter parameter)
		{
		}
		public override void GenerateTypeParameterConstraint(TypeParameterConstraint constraint)
		{
		}
		protected override void GenerateVolatile(Volatile member)
		{
		}
		#endregion
		#region GenerateImplicitVariable
		protected override void GenerateImplicitVariable(ImplicitVariable member)
		{
			if (member == null)
				return;
			if (member.Expression != null)
				GenerateInitializedVariable(member);
			else
				GenerateVariable(member);
		}
		#endregion
		#region GenerateLambdaImplicitlyTypedParam
		protected override void GenerateLambdaImplicitlyTypedParam(LambdaImplicitlyTypedParam member)
		{
			GenerateParameter(member);
		}
		#endregion
		#region GenerateMethod
		protected override void GenerateMethod(Method member)
		{
			if (member == null)
				return;
	  Write(FormattingTokenType.Function);
	  Write(FormattingTokenType.Ident);
	  Write(FormattingTokenType.ParenOpen);
	  GenerateElementCollection(member.Parameters, FormattingTokenType.Comma);
	  Write(FormattingTokenType.ParenClose);
			Write(FormattingTokenType.CurlyBraceOpen);
			Code.IncreaseIndent();
			GenerateElementCollection(member.Nodes, FormattingTokenType.None);
			Code.DecreaseIndent();
	  Write(FormattingTokenType.CurlyBraceClose);
		}
		#endregion
		#region GenerateVariable
		protected override void GenerateVariable(Variable member)
		{
			if (member == null)
				return;
			GenerateVariableStart(member);
			GenerateVariableEnd(member);
		}
		#endregion
		#region GenerateInitializedVariable
		protected override void GenerateInitializedVariable(InitializedVariable member)
		{
			if (member == null)
				return;
			GenerateVariableStart(member);
			if (member.Expression != null)
			{
				Write(FormattingTokenType.Equal);
				CodeGen.GenerateElement(member.Expression);
			}
			GenerateVariableEnd(member);
		}
		#endregion
		#region GenerateParameter
		protected override void GenerateParameter(Param member)
		{
			if (member == null)
				return;
			Write(FormattingTokenType.Ident);
		}
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Var:
		case FormattingTokenType.Comma:
		  result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
#endregion
	}
}
