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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
	public class VBMemberCodeGen : MemberCodeGenBase 
	{
		#region VBMemberCodeGen
		public VBMemberCodeGen(CodeGen codeGen) : base(codeGen) 
		{
		}
		#endregion
	TypeReferenceExpression GetNotArrayAndNotNullableType(TypeReferenceExpression type)
	{
	  while (type != null && (type.IsArrayType || type.IsNullable))
	  {
		type = type.BaseType;
	  }
	  return type;
	}
	TypeReferenceExpression GetBaseType(TypeReferenceExpression exp)
	{
	  if (exp == null || exp.BaseType == null)
		return exp;
	  return GetBaseType(exp.BaseType);
	}
	bool HasPrevVariable(Variable var)
	{
	  if (var == null)
		return false;
	  return var.PreviousVariable != null;
	}
	bool GenerateArrayNameModifiers(Variable var)
	{
	  if (var == null)
		return false;
	  LanguageElementCollection modifiers = var.ArrayNameModifiers;
	  if (modifiers == null || modifiers.Count == 0)
		return false;
	  int count = modifiers.Count;
	  for (int i = 0; i < count; i++)
	  {
		ArrayNameModifier modif = modifiers[i] as ArrayNameModifier;
		if (modif == null)
		  continue;
		CodeGen.GenerateElement(modif);
	  }
	  return true;
	}
	bool NeedGenerate(Variable var)
	{
	  if (var == null)
		return true;
	  return var.HasType;
	}
	bool IsSub(Member member)
	{
	  if (member == null)
		return false;
	  return member.MemberTypeReference == null &&
		(member.MemberType == null || member.MemberType.Length == 0);
	}
	bool CanGenerateMethodVisibility(Method method)
	{
	  if (method == null)
		return false;
	  if (method.IsConstructor)
		return !method.IsStatic;
	  if (IsDefaultVisibility(method))
		return false;
	  return true;
	}
	bool IsMemberWithoutBody(AccessSpecifiedElement element)
	{
	  if (element == null)
		return true;
	  if (element.IsAbstract || (element.Parent != null &&
		element.Parent.ElementType == LanguageElementType.Interface) || element.IsExtern)
		return true;
	  return false;
	}
	bool MemberWithVisibility(Member member)
	{
	  if (member == null)
		return true;
	  return member.Parent == null || member.Parent.ElementType != LanguageElementType.Interface;
	}
	bool IsDefaultVisibility(AccessSpecifiedElement element)
	{
	  if (element == null)
		return false;
	  return element.IsDefaultVisibility;
	}
	bool ShouldGenerateExtensionMethodAttribute(Method method)
	{
	  if (!ContainsExtensionMethodParameter(method.Parameters))
		return false;
	  return !ContainsExtensionAttribute(method.Attributes);
	}
	bool ContainsExtensionAttribute(NodeList attributes)
	{
	  if (attributes == null || attributes.Count == 0)
		return false;
	  for (int i = 0; i < attributes.Count; i++)
	  {
		IAttributeElement attributeElement = attributes[i] as IAttributeElement;
		if (attributeElement == null || String.IsNullOrEmpty(attributeElement.Name))
		  continue;
		if (String.Compare("Extension", attributeElement.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
		  return true;
	  }
	  return false;
	}
	bool ContainsExtensionMethodParameter(LanguageElementCollection parameters)
	{
	  if (parameters == null || parameters.Count == 0)
		return false;
	  for (int i = 0; i < parameters.Count; i++)
	  {
		ExtensionMethodParam extensionMethodParam = parameters[i] as ExtensionMethodParam;
		if (extensionMethodParam != null)
		  return true;
	  }
	  return false;
	}
	bool IsTypeCharacter(TypeReferenceExpression type)
	{
	  if (type == null)
		return false;
	  type = GetBaseType(type);
	  return type != null && type.IsTypeCharacter;
	}
	void GenerateAccessSpecifiersInInterface(AccessSpecifiers access)
		{
	  if(access.Specifiers != AccessSpecifiersFlags.None)
		CodeGen.AddWSIfNeeded();
	  if (access.IsNew)
				Write(FormattingTokenType.Shadows);
			if (access.IsOverloads)
				Write(FormattingTokenType.Overloads);
			if (access.IsWriteOnly)
				Write(FormattingTokenType.WriteOnly);
			if (access.IsReadOnly)
				Write(FormattingTokenType.ReadOnly);
			if (access.IsDefault)
				Write(FormattingTokenType.Default);
		}
		void GenerateMemberType(Member member)
		{
			if (member == null)
				return;
	  TypeReferenceExpression lTypeRef = ParserUtils.GetElementType(member);
	  string lTypeName = ParserUtils.GetElementTypeName(member);
			if (lTypeRef != null)
				CodeGen.GenerateCode(Code, lTypeRef);
			else if (lTypeName != null && lTypeName.Length > 0)
				((VBExpressionCodeGen)CodeGen.ExpressionGen).GenerateTypeString(member.MemberType);
			else
				Write(FormattingTokenType.Object);
		}
		void GenerateArgumentDirection(ArgumentDirection ad) 
		{
			switch (ad) 
			{
				case ArgumentDirection.In:
					Write(FormattingTokenType.ByVal);
					return;
				case ArgumentDirection.Out:
				case ArgumentDirection.Ref:
					Write(FormattingTokenType.ByRef);
					break;
				case ArgumentDirection.ParamArray:
		  Write(FormattingTokenType.ParamArray);
					break;
			}
		}
		void GenerateVariableBase(Variable var) 
		{
			GenerateVariableBase(var, true);
		}
		void GenerateRootMemberType(Member member)
		{
			if (member == null)
				return;
			TypeReferenceExpression typeRef = ParserUtils.GetElementType(member);
	  typeRef = GetNotArrayAndNotNullableType(typeRef);
	  string lTypeName = ParserUtils.GetElementTypeName(member);
			if (typeRef != null)
			{
				CodeGen.GenerateElement(typeRef);
			}
			else if (lTypeName != null && lTypeName.Length > 0)
				((VBExpressionCodeGen)CodeGen.ExpressionGen).GenerateTypeString(member.MemberType);
			else
				Write(FormattingTokenType.Object);
		}
		void WriteWithEvents(BaseVariable var)
		{
			if (var == null)
				return;
			AccessSpecifiers accessSpecifiers = var.AccessSpecifiers;
			if (accessSpecifiers == null)
				return;
			if (accessSpecifiers.IsWithEvents)
				Write(FormattingTokenType.WithEvents);
		}
		void GenerateDim(Variable var)
		{
			if (var == null)
				return;
			bool isStaticLocal = var.IsStatic && var.IsLocal;
			if (isStaticLocal || (VBCodeGen.CheckForDetailNodes && var.IsDetailNode)|| var.IsField)
				return;
			Write(FormattingTokenType.Dim);
		}
		void GenerateVariableBase(Variable var, bool generateMemberType) 
		{
			if (var == null)
				return;
			bool hasPrevVar = HasPrevVariable(var);
			if (!hasPrevVar)
			{
		if (!var.IsLocal)
		  GenerateMemberVisibility(var.Visibility);
		GenerateAccessSpecifiers(var.AccessSpecifiers, false, var.IsLocal);
				GenerateDim(var);
			}
			WriteWithEvents(var);
	  if (!hasPrevVar && var.NextVariable != null)
	  {
		bool needUpdateAlignment = Options.WrappingAlignment.AlignWithFirstMultiVariableDeclarationItem
		  && !Options.WrappingAlignment.WrapFirstMultiVariableDeclaration;
		CodeGen.TokenGen.NeedIncreaseAlignment = needUpdateAlignment;
	  } 
	  Write(FormattingTokenType.Ident);
			if (IsTypeCharacter(var.MemberTypeReference))
			{
				GenerateTypeCharacter(var.MemberTypeReference);
				return;
			}
			bool hasModifiers = GenerateArrayNameModifiers(var);
			if (var.NullableModifier != null)
			{
		hasModifiers = true;
				Write(FormattingTokenType.Question);
			}
			if(generateMemberType && NeedGenerate(var))
				Write(FormattingTokenType.As);
			if (var is InitializedVariable && ((InitializedVariable)var).IsObjectCreationInit)
				CodeGen.GenerateElement(((InitializedVariable)var).Expression);
			else if(generateMemberType && NeedGenerate(var))
				if (hasModifiers)
					GenerateRootMemberType(var);
				else
					GenerateMemberType(var);
		}
		void GenerateTypeCharacter(TypeReferenceExpression exp)
		{
			if (exp == null)
				return;
			CodeGen.GenerateElement(exp);
		}	
		void GenerateVariableEnd(Variable var)
		{
			if (var == null)
				return;
			if (!var.IsDetailNode && var.NextVariable != null)
				Write(FormattingTokenType.Comma);
		}
	void GenerateMemberImplementsOrHandlesCollection(Member member)
	{
	  GenerateMemberImplementsCollection(member);
	  GenerateMemberHandlesCollection(member);
	}
	void GenerateMemberImplementsCollection(Member member)
	{
	  if (member == null || member.ImplementsExpressions == null || member.ImplementsExpressions.Count == 0)
		return;
	  Write(FormattingTokenType.Implements);
	  GenerateElementCollection(member.ImplementsExpressions, FormattingTokenType.Comma);
	}
	void GenerateMemberHandlesCollection(Member member)
	{
	  if (member == null || member.HandlesExpressions == null || member.HandlesExpressions.Count == 0)
		return;
	  Write(FormattingTokenType.Handles);
	  GenerateElementCollection(member.HandlesExpressions, FormattingTokenType.Comma);
	}
	void WriteString(FormattingTokenType type, string str)
	{
	  if (String.IsNullOrEmpty(str))
		return;
	  Write(type);
	  Write(str);
	}
	void WriteMethodName(Method method)
	{
	  if (method == null)
		return;
	  if (method.MethodType == MethodTypeEnum.Constructor)
		Write(FormattingTokenType.New);
	  else if (method.MethodType == MethodTypeEnum.Destructor)
		Write(FormattingTokenType.Finalize);
	  else
		Write(VBOperatorHelper.GetOperatorNaturalName(method.Name));
	}
	void WriteMethodSpecifier(Method method)
	{
	  if (method == null)
		return;
	  if (method.IsClassOperator)
		Write(FormattingTokenType.Operator);
	  else if (method.MethodType == MethodTypeEnum.Function)
		Write(FormattingTokenType.Function);
	  else
		Write(FormattingTokenType.Sub);
	}
	bool IsMultilineMember(LanguageElement element)
	{
	  if (element == null)
		return false;
	  if (element.ElementType == LanguageElementType.Method || element.ElementType == LanguageElementType.Property)
		return true;
	  if (element is TypeDeclaration && element.Parent is TypeDeclaration)
		return true;
	  return false;
	}
	bool IsSinlelineMember(LanguageElement element)
	{
	  if (element == null)
		return false;
	  BaseVariable var = element as BaseVariable;
	  if (var != null && (var.IsField || var.IsConst))
		return true;
	  return false;
	}
	protected override void GenerateTypeParameterConstraintCollection(TypeParameterConstraintCollection collection)
	{
	  if (collection == null || collection.Count == 0)
		return;
	  Write(FormattingTokenType.As);
	  if (collection.Count == 1)
		CodeGen.GenerateElement(collection[0]);
	  else
	  {
		Write(FormattingTokenType.CurlyBraceOpen);
		GenerateElementCollection(collection, FormattingTokenType.Comma);
		Write(FormattingTokenType.CurlyBraceClose);
	  }
	}
	protected override void GenerateInitializedVolatile(InitializedVolatile var)
	{
	}
	protected override void GenerateConstVolatile(ConstVolatile member)
	{
	}
	protected override void GenerateVolatile(Volatile member)
	{
	}
	protected override void GenerateMethodPrototype(MethodPrototype member)
	{
	}
	protected override void GenerateMethod(Method method)
	{
	  if (method == null)
		return;
	  if(ShouldGenerateExtensionMethodAttribute(method)) {
		  Write("<System.Runtime.CompilerServices.Extension> _");
		  Write(FormattingTokenType.NewLine);
	  }
	  if (MemberWithVisibility(method))
	  {
		if (CanGenerateMethodVisibility(method))
		  GenerateMemberVisibility(method.Visibility);
		GenerateAccessSpecifiers(method.AccessSpecifiers, false);
	  }
	  else
		GenerateAccessSpecifiersInInterface(method.AccessSpecifiers);
	  if (!string.IsNullOrEmpty(method.CharsetModifier))
	  {
		Write(method.CharsetModifier);
		CodeGen.AddWSIfNeeded();
	  }
	  WriteMethodSpecifier(method);
	  if (method.MethodType == MethodTypeEnum.Constructor)
		Write(FormattingTokenType.New);
	  else if (method.MethodType == MethodTypeEnum.Destructor)
		Write(FormattingTokenType.Finalize);
	  else
		WriteMethodName(method);
	  if (method.IsGeneric)
		GenerateGenericModifier(method.GenericModifier);
	  bool isCharacter = IsTypeCharacter(method.MemberTypeReference);
	  if (isCharacter)
		GenerateTypeCharacter(method.MemberTypeReference);
	  WriteString(FormattingTokenType.Lib, method.Lib);
	  WriteString(FormattingTokenType.Alias, method.Alias);
	  GenerateParameters(method.Parameters);
	  if (method.MethodType == MethodTypeEnum.Function && !isCharacter)
	  {
		Write(FormattingTokenType.As);
		GenerateMemberType(method);
	  }
	  if (IsMemberWithoutBody(method) || !method.GenerateCodeBlock)
		return;
	  GenerateMemberImplementsOrHandlesCollection(method);
	  CodeGen.AddNewLineIfNeeded();
	  using (GetIndent())
	  {
		if (method.MethodType == MethodTypeEnum.Constructor && method.HasInitializer)
		  CodeGen.GenerateElement(method.Initializer);
		CodeGen.AddNewLineIfNeeded();
		GenerateElementCollection(method.Nodes, FormattingTokenType.None, true, LanguageElementType.ConstructorInitializer);
	  }
	  CodeGen.AddNewLineIfNeeded();
	  Write(FormattingTokenType.End);
	  WriteMethodSpecifier(method);
	}
	protected override void GenerateEvent(Event member)
	{
	  if (member == null)
		return;
	  if (MemberWithVisibility(member))
	  {
		GenerateMemberVisibility(member);
		GenerateAccessSpecifiers(member.AccessSpecifiers, false);
	  }
	  bool isCustom = false;
	  if (member.NodeCount != 0)
	  {
		Write(FormattingTokenType.Custom);
		isCustom = true;
	  }
	  Write(FormattingTokenType.Event);
	  Write(member.Name);
	  LanguageElementCollection coll = member.Parameters;
	  bool hasType = member.MemberTypeReference != null || (member.MemberType != null && member.MemberType != String.Empty);
	  if (coll != null && !hasType)
		GenerateParameters(member.Parameters);
	  if (hasType)
	  {
		Write(FormattingTokenType.As);
		GenerateMemberType(member);
	  }
	  if (IsMemberWithoutBody(member))
		return;
	  GenerateMemberImplementsOrHandlesCollection(member);
	  if (isCustom)
	  {
		if (member.GenerateAccessors)
		  using(GetIndent())
			GenerateElementCollection(member.Nodes, FormattingTokenType.None, true);
		else
		  Write(FormattingTokenType.NewLine);
		Write(FormattingTokenType.End, FormattingTokenType.Event);
	  }
	}
	protected override void GenerateProperty(Property property)
	{
	  bool inInterface = property.Parent != null && property.Parent.ElementType == LanguageElementType.Interface;
	  bool isAutoImplemented = property.IsAutoImplemented;
	  AccessSpecifiers access = property.AccessSpecifiers;
	  if (access == null)
		access = new AccessSpecifiers();
	  if (MemberWithVisibility(property))
		GenerateMemberVisibility(property);
	  if (inInterface)
	  {
		if (!isAutoImplemented)
		{
		  if (property.IsReadOnly)
			access.IsReadOnly = true;
		  else if (property.IsWriteOnly)
			access.IsWriteOnly = true;
		}
		GenerateAccessSpecifiersInInterface(access);
	  }
	  else
	  {
		if (!isAutoImplemented)
		{
		  if (property.HasGetter && !property.HasSetter)
			access.IsReadOnly = true;
		  else if (!property.HasGetter && property.HasSetter)
			access.IsWriteOnly = true;
		}
		GenerateAccessSpecifiers(access, false);
	  }
	  if (!CodeGen.TokenGen.TokenGenArgs.IsLastElementNewLine() && !CodeGen.TokenGen.TokenGenArgs.IsLastElementIndent())
		if (CodeGen.TokenGen.FormattingElements.Count == 0 && !CodeGen.TokenGen.IsLastGeneratedEOL())
		  CodeGen.AddWSIfNeeded();
	  Write(FormattingTokenType.Property);
	  Write(FormattingTokenType.Ident);
	  if (property.GenerateParens || !property.ParensRange.IsEmpty || property.ParameterCount > 0)
		GenerateParameters(property.Parameters);
	  if (IsTypeCharacter(property.MemberTypeReference))
		GenerateTypeCharacter(property.MemberTypeReference);
	  else
	  {
		Write(FormattingTokenType.As);
		GenerateMemberType(property);
	  }
	  if (IsMemberWithoutBody(property))
		return;
	  Expression initializer = property.Initializer;
	  if (initializer != null)
	  {
		Write(FormattingTokenType.Equal);
		CodeGen.GenerateElement(initializer);
	  }
	  GenerateMemberImplementsOrHandlesCollection(property);
	  if (isAutoImplemented || property.IsAbstract ||
		(property.Parent != null && property.Parent.ElementType == LanguageElementType.Interface))
		return;
	  if(!property.GenerateAccessors)
		return;
	  using (GetIndent())
	  {
		if (property.HasGetter && property.Getter.GenerateCodeBlock)
		{
		  CodeGen.AddNewLineIfNeeded();
		  CodeGen.GenerateElement(property.Getter);
		}
		if (property.HasSetter && property.Setter.GenerateCodeBlock)
		{
		  CodeGen.AddNewLineIfNeeded();
		  CodeGen.GenerateElement(property.Setter);
		}
	  }
	  CodeGen.AddNewLineIfNeeded();
	  Write(FormattingTokenType.End, FormattingTokenType.Property);
	}
	protected override void GenerateDelegate(DelegateDefinition member)
	{
	  GenerateMemberVisibility(member);
	  GenerateAccessSpecifiers(member.AccessSpecifiers, false);
	  Write(FormattingTokenType.Delegate);
	  bool lIsSub = IsSub(member);
	  if (lIsSub)
		Write(FormattingTokenType.Sub);
	  else
		Write(FormattingTokenType.Function);
	  Write(FormattingTokenType.Ident);
	  if (member.IsGeneric)
		GenerateGenericModifier(member.GenericModifier);
	  GenerateParameters(member.Parameters);
	  if (!lIsSub)
	  {
		Write(FormattingTokenType.As);
		GenerateMemberType(member);
	  }
	}
	protected override void GenerateConst(Const var)
	{
	  if (!var.IsLocal)
		GenerateMemberVisibility(var.Visibility);
	  GenerateAccessSpecifiers(var.AccessSpecifiers, false);
	  Write(FormattingTokenType.Const, FormattingTokenType.Ident, FormattingTokenType.As);
	  GenerateMemberType(var);
	  Write(FormattingTokenType.Equal);
	  CodeGen.GenerateElement(var.Value);
	}
	protected override void GenerateVariable(Variable var)
	{
	  GenerateVariableBase(var);
	  GenerateVariableEnd(var);
	}
	protected override void GenerateInitializedVariable(InitializedVariable var)
	{
	  GenerateVariableBase(var);
	  if (!var.IsObjectCreationInit)
	  {
		Write(FormattingTokenType.Equal);
		CodeGen.GenerateElement(var.Expression);
	  }
	  GenerateVariableEnd(var);
	}
	protected override void GenerateParameter(Param param)
	{
	  if (param.IsOptional)
		Write(FormattingTokenType.Optional);
	  GenerateArgumentDirection(param.Direction);
	  Write(FormattingTokenType.Ident);
	  bool isCharacter = IsTypeCharacter(param.MemberTypeReference);
	  if (isCharacter)
		GenerateTypeCharacter(param.MemberTypeReference);
	  else
	  {
		Write(FormattingTokenType.As);
		GenerateMemberType(param);
	  }
	  if (param.IsOptional &&
		  ((param.DefaultValue != null && param.DefaultValue != String.Empty) ||
			(param.DefaultValueExpression != null)))
	  {
		Write(FormattingTokenType.Equal);
		if (param.DefaultValueExpression != null)
		  CodeGen.GenerateElement(param.DefaultValueExpression);
		else if (param.DefaultValue != null)
		  Write(param.DefaultValue);
	  }
	}
	protected override void GenerateEnumElement(EnumElement element)
	{
	  Write(element.Name);
	  Expression exp = element.ValueExpression;
	  if (exp != null)
	  {
		Write(FormattingTokenType.Equal);
		CodeGen.GenerateElement(exp);
	  }
	  else if (element.Value != null && element.Value.Length > 0)
	  {
		Write(FormattingTokenType.Equal);
		Write(element.Value);
	  }
	}
	protected override void GenerateImplicitVariable(ImplicitVariable member)
	{
	  if (member == null)
		return;
	  bool hasPrevVar = HasPrevVariable(member);
	  if (!hasPrevVar)
	  {
		if (!member.IsLocal)
		  GenerateMemberVisibility(member.Visibility);
		GenerateAccessSpecifiers(member.AccessSpecifiers, false, member.IsLocal);
		if (member.IsConst && !(member.IsStatic))
		{
		  Write(FormattingTokenType.Const);
		}
		else if (!(member.IsStatic))
		{
		  bool lIsStaticLocal = member.IsStatic && member.IsLocal;
		  if (VBCodeGen.CheckForDetailNodes)
		  {
			if (!member.IsDetailNode && !lIsStaticLocal)
			  Write(FormattingTokenType.Dim);
		  }
		  else if (!lIsStaticLocal)
			Write(FormattingTokenType.Dim);
		}
	  }
	  WriteWithEvents(member);
	  Write(FormattingTokenType.Ident);
	  if (member.Expression != null)
	  {
		Write(FormattingTokenType.Equal);
		CodeGen.GenerateElement(member.Expression);
	  }
	  GenerateVariableEnd(member);
	}
	protected override void GenerateExtensionMethodParam(ExtensionMethodParam member)
	{
	  GenerateParameter(member);
	}
	protected override void GenerateLambdaImplicitlyTypedParam(LambdaImplicitlyTypedParam member)
	{
	  if (member == null)
		return;
	  GenerateArgumentDirection(member.Direction);
	  Write(FormattingTokenType.Ident);
	  if (member.MemberTypeReference != null)
	  {
		Write(FormattingTokenType.As);
		GenerateMemberType(member);
	  }
	}
	protected override void GenerateQueryIdent(QueryIdent member)
	{
	  if (member == null)
		return;
	  Write(FormattingTokenType.Ident);
	  if (member.MemberTypeReference != null)
	  {
		Write(FormattingTokenType.As);
		GenerateMemberType(member);
	  }
	  if (member.Expression != null)
	  {
		Write(FormattingTokenType.Equal);
		CodeGen.GenerateElement(member.Expression);
	  }
	}
		protected VBCodeGen VBCodeGen
		{
			get
			{
				return (VBCodeGen)CodeGen;
			}
		}
	public void GenerateAccessSpecifiers(AccessSpecifiers access, bool forType)
	{
	  GenerateAccessSpecifiers(access, forType, false);
	}
	public void GenerateAccessSpecifiers(AccessSpecifiers access, bool forType, bool forLocal)
	{
	  if (access == null)
		return;
	  if (access.IsAsynchronous)
		Write(FormattingTokenType.Async);
	  if (access.IsAbstract)
	  {
		if (forType)
		  Write(FormattingTokenType.MustInherit);
		else
		  Write(FormattingTokenType.MustOverride);
	  }
	  if (access.IsExtern)
		Write(FormattingTokenType.Declare);
	  if (access.IsOverride)
		Write(FormattingTokenType.Overrides);
	  if (access.IsSealed)
	  {
		if (forType)
		  Write(FormattingTokenType.NotInheritable);
		else
		  Write(FormattingTokenType.NotOverridable);
	  }
	  if (access.IsStatic)
	  {
		if (forLocal)
		  Write(FormattingTokenType.Static);
		else
		  Write(FormattingTokenType.Shared);
	  }
	  if (access.IsVirtual)
	  {
		CodeGen.AddWSIfNeeded();
		Write(FormattingTokenType.Overridable);
	  }
	  GenerateAccessSpecifiersInInterface(access);
	}
	public void GenerateMemberVisibility(AccessSpecifiedElement member)
	{
	  if (!member.IsDefaultVisibility)
		GenerateMemberVisibility(member.Visibility);
	}
	public void GenerateMemberVisibility(MemberVisibility visibility)
	{
	  switch (visibility)
	  {
		case MemberVisibility.Internal:
		  Write(FormattingTokenType.Friend);
		  break;
		case MemberVisibility.Private:
		  Write(FormattingTokenType.Private);
		  break;
		case MemberVisibility.Protected:
		  Write(FormattingTokenType.Protected);
		  break;
		case MemberVisibility.ProtectedInternal:
		  Write(FormattingTokenType.Protected, FormattingTokenType.Friend);
		  break;
		case MemberVisibility.Public:
		  Write(FormattingTokenType.Public);
		  break;
	  }
	}
	public override void GenerateGenericModifier(GenericModifier generic)
	{
	  if (generic == null)
		return;
	  Write(FormattingTokenType.ParenOpen, FormattingTokenType.Of);
	  GenerateElementCollection(generic.TypeParameters, FormattingTokenType.Comma);
	  Write(FormattingTokenType.ParenClose);
	}
	public override void GenerateTypeParameters(GenericModifier generic)
	{
	  if (generic == null)
		return;
	  GenerateElementCollection(generic.TypeParameters, FormattingTokenType.Comma);
	}
	public override void GenerateTypeParameterConstraints(GenericModifier generic, bool addLastNewLine)
	{
	  return;
	}
	public override void GenerateTypeParameterConstraints(TypeParameterCollection collection, bool addLastNewLine)
	{
	  return;
	}
	public override void GenerateTypeParameter(TypeParameter expression)
	{
	  TypeParameterDirection direction = expression.Direction;
	  if (direction == TypeParameterDirection.In)
		Write(FormattingTokenType.In);
	  else if (direction == TypeParameterDirection.Out)
		Write(FormattingTokenType.Out);
	  Write(expression.Name);
	  GenerateTypeParameterConstraintCollection(expression.Constraints);
	  AddGeneratedElement(expression);
	}
	public override void GenerateTypeParameterConstraint(TypeParameterConstraint constraint)
	{
	  if (constraint == null)
		return;
	  switch (constraint.ElementType)
	  {
		case LanguageElementType.ClassTypeParameterConstraint:
		  Write(FormattingTokenType.Class);
		  break;
		case LanguageElementType.StructTypeParameterConstraint:
		  Write(FormattingTokenType.Structure);
		  break;
		case LanguageElementType.NewTypeParameterConstraint:
		  Write(FormattingTokenType.New);
		  break;
		case LanguageElementType.NamedTypeParameterConstraint:
		  CodeGen.GenerateElement(((NamedTypeParameterConstraint)constraint).TypeReference);
		  break;
		default:
		  Write(constraint.Name);
		  break;
	  }
	}
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.End:
		case FormattingTokenType.Sub:
		case FormattingTokenType.Function:
		case FormattingTokenType.Event:
		case FormattingTokenType.Property:
		case FormattingTokenType.As:
		case FormattingTokenType.MustOverride:
		case FormattingTokenType.ReadOnly:
		case FormattingTokenType.WriteOnly:
		case FormattingTokenType.Delegate:
		case FormattingTokenType.Overridable:
		case FormattingTokenType.Optional:
		case FormattingTokenType.Shadows:
		case FormattingTokenType.Overloads:
		case FormattingTokenType.Default:
		  result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
	public override FormattingElements PrevFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.PrevFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.As:
		  result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
	public override bool GenerateElementTail(LanguageElement element)
	{
	  if (Options.BlankLines.AfterMultiLineMembers && IsMultilineMember(element) && element.NextCodeSibling != null)
	  {
		CodeGen.AddNewLine(2);
		return true;
	  }
	  if (Options.BlankLines.AfterSingleLineMembers && IsSinlelineMember(element) && element.NextCodeSibling != null)
	  {
		CodeGen.AddNewLine(2);
		return true;
	  }
	  return false;
	}
  }
}
