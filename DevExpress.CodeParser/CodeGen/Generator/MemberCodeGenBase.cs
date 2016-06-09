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

#if DXCORE
using System.Collections.Generic;
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif 
{
	public abstract class MemberCodeGenBase : LanguageElementCodeGenBase 
	{
		#region MemberCodeGenBase
		public MemberCodeGenBase(CodeGen codeGen) : base(codeGen)
		{
		}
		#endregion
		#region Generate
		public override void GenerateElement(LanguageElement element) 
		{
			if (element == null)
				return;
	  if (GenerateMember(element as Member))
		return;
	  if (element is GenericModifier)
		GenerateGenericModifier(element as GenericModifier);
	  else if (element is TypeParameter)
		GenerateTypeParameter(element as TypeParameter);
	  else if (element is TypeParameterConstraint)
		GenerateTypeParameterConstraint(element as TypeParameterConstraint);
		}
		#endregion
	protected virtual void GenerateMultiVars(Variable var)
	{
	  LanguageElementCollection coll = new LanguageElementCollection();
	  while (var != null)
	  {
		coll.Add(var);
		CodeGen.AddSkiped(var);
		var = var.NextVariable;
	  }
	  ElementsGenerationRules rules = new ElementsGenerationRules();
	  rules.AlignIfWrap = Options.WrappingAlignment.AlignWithFirstMultiVariableDeclarationItem;
	  rules.WrapFirst = Options.WrappingAlignment.WrapFirstMultiVariableDeclaration;
	  rules.WrapParams = Options.WrappingAlignment.WrapMultiVariableDeclaration;
	  rules.Indenting = !rules.AlignIfWrap;
	  rules.StringDelimiter = GetCommaDelimeter();
	  GenerateElementsByRules(coll, rules);
	}
		protected virtual bool IsInitializer(LanguageElement element)
		{
			if (element == null)
				return false;
			For lFor = element.Parent as For;
			if (lFor != null)
				return element.IsDetailNode;
			ForEach lForEach = element.Parent as ForEach;
			if (lForEach != null)
				return element.IsDetailNode;
			return false;
		}
		#region GenerateMember
	protected virtual bool GenerateMember(Member member)
	{
	  if (member == null)
		return false;
	  switch (member.ElementType)
	  {
		case LanguageElementType.QueryIdent:
		  GenerateQueryIdent(member as QueryIdent);
		  return true;
		case LanguageElementType.Method:
		  GenerateMethod(member as Method);
		  return true;
		case LanguageElementType.MethodPrototype:
		  GenerateMethodPrototype(member as MethodPrototype);
		  return true;
		case LanguageElementType.Event:
		  GenerateEvent(member as Event);
		  return true;
		case LanguageElementType.Property:
		  GenerateProperty(member as Property);
		  return true;
		case LanguageElementType.Delegate:
		  GenerateDelegate(member as DelegateDefinition);
		  return true;
		case LanguageElementType.Const:
		  GenerateConst(member as Const);
		  return true;
		case LanguageElementType.Variable:
		  GenerateVariable(member as Variable);
		  return true;
		case LanguageElementType.InitializedVariable:
		  GenerateInitializedVariable(member as InitializedVariable);
		  return true;
		case LanguageElementType.Parameter:
		  GenerateParameter(member as Param);
		  return true;
		case LanguageElementType.EnumElement:
		  GenerateEnumElement(member as EnumElement);
		  return true;
		case LanguageElementType.Volatile:
		  GenerateVolatile(member as Volatile);
		  return true;
		case LanguageElementType.ConstVolatile:
		  GenerateConstVolatile(member as ConstVolatile);
		  return true;
		case LanguageElementType.InitializedVolatile:
		  GenerateInitializedVolatile(member as InitializedVolatile);
		  return true;
		case LanguageElementType.ImplicitVariable:
		  GenerateImplicitVariable(member as ImplicitVariable);
		  return true;
		case LanguageElementType.ExtensionMethodParam:
		  GenerateExtensionMethodParam(member as ExtensionMethodParam);
		  return true;
		case LanguageElementType.LambdaImplicitlyTypedParam:
		  GenerateLambdaImplicitlyTypedParam(member as LambdaImplicitlyTypedParam);
		  return true;
	  }
	  return false;
	}
		#endregion
		protected abstract void GenerateImplicitVariable(ImplicitVariable member);
		protected abstract void GenerateExtensionMethodParam(ExtensionMethodParam member);
		protected abstract void GenerateLambdaImplicitlyTypedParam(LambdaImplicitlyTypedParam member);
		protected abstract void GenerateMethod(Method member);
		protected abstract void GenerateMethodPrototype(MethodPrototype member);
		protected abstract void GenerateEvent(Event member);
		protected abstract void GenerateProperty(Property member);
		protected abstract void GenerateDelegate(DelegateDefinition member);
		protected abstract void GenerateConstVolatile(ConstVolatile member);
		protected abstract void GenerateConst(Const member);
		protected abstract void GenerateVolatile(Volatile member);
		protected abstract void GenerateVariable(Variable member);
		protected abstract void GenerateInitializedVolatile(InitializedVolatile member);
		protected abstract void GenerateInitializedVariable(InitializedVariable member);
		protected abstract void GenerateParameter(Param member);
		protected abstract void GenerateEnumElement(EnumElement member);
	protected abstract void GenerateQueryIdent(QueryIdent expression);
	public abstract void GenerateTypeParameterConstraint(TypeParameterConstraint constraint);
	public abstract void GenerateGenericModifier(GenericModifier generic);
	public abstract void GenerateTypeParameter(TypeParameter parameter);
	public virtual void GenerateTypeParameterConstraints(GenericModifier generic, bool addLastNewLine)
	{
	}
	public virtual void GenerateTypeParameterConstraints(TypeParameterCollection collection, bool addLastNewLine)
	{
	}
	public virtual void GenerateTypeParameters(GenericModifier generic)
	{
	}		
	public virtual bool IsMemberGenElement(LanguageElement element)
	{
	  if (element == null)
		return false;
	  if (element is Member || element is GenericModifier || element is TypeParameter || element is TypeParameterConstraint)
		return true;
	  return false;
	}
	public virtual void GenerateTypeParameterCollection(TypeParameterCollection collection)
	{
	  if (collection == null)
		return;
	  int lCount = collection.Count;
	  string commaDelim = GetCommaDelimeter();
	  for (int i = 0; i < lCount; i++)
	  {
		CodeGen.GenerateElement(collection[i]);
		if (i < collection.Count - 1)
		  Code.Write(commaDelim);
	  }
	}
	protected virtual void GenerateTypeParameterConstraintCollection(TypeParameterConstraintCollection constraints)
	{
	  if (constraints == null)
		return;
	  int lCount = constraints.Count;
	  string commaDelim = GetCommaDelimeter();
	  for (int i = 0; i < lCount; i++)
	  {
		GenerateTypeParameterConstraint(constraints[i]);
		if (i < constraints.Count - 1)
		  Code.Write(commaDelim);
	  }
	}	
	}
}
