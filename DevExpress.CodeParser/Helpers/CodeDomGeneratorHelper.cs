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
using System.IO;
using System.Text;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  using Diagnostics;
	public enum CodeGenerationContext
	{
		Class,
		Struct,
		Enum,
		Interface
	}
	public class CodeDomGeneratorHelper
	{
		enum CodeGenParameterUsage
		{
			Target,
			TargetWrapper,
			ParameterList
		}
		const string STR_WrapperName = "WrapperType";
		const string STR_CurrentClassField = "currentClass";
		const string STR_OutputField = "output";
		const string STR_OptionsField = "options";
		const string STR_GenerateMethod = "GenerateMethod";
		const string STR_GenerateProperty = "GenerateProperty";
		const string STR_GenerateEvent = "GenerateEvent";
		const string STR_GenerateField = "GenerateField";
		const string STR_GenerateConstructor = "GenerateConstructor";
		const string STR_GenerateEntryPointMethod = "GenerateEntryPointMethod";
		const string STR_GenerateTypeConstructor = "GenerateTypeConstructor";
		const string STR_GenerateAttributes = "GenerateAttributes";
		CodeDomProvider _CodeProvider;
		CodeGeneratorOptions _Options;
		public CodeDomGeneratorHelper(CodeDomProvider provider, CodeGeneratorOptions options)
		{
			_CodeProvider = provider;
			_Options = options;
		}
		object[] GetCodeGenMethodParameters(CodeGenParameterUsage usage, CodeObject target, CodeTypeDeclaration wrapper, object[] parameters)
		{
			switch (usage)
			{
				case CodeGenParameterUsage.Target:
					return new object[] {target};
				case CodeGenParameterUsage.TargetWrapper:
					return new object[] {target, wrapper};
				case CodeGenParameterUsage.ParameterList:
					return parameters;
			}
			return new object[] {target, wrapper};
		}
		void SetCodeGenContext(CodeTypeDeclaration type, CodeGenerationContext context)
		{
			if (type == null)
				return;
			switch (context)
			{
				case CodeGenerationContext.Class:
					type.IsClass = true;
					break;
				case CodeGenerationContext.Interface:
					type.IsInterface = true;
					break;
				case CodeGenerationContext.Enum:
					type.IsEnum = true;
					break;
				case CodeGenerationContext.Struct:
					type.IsStruct = true;
					break;
			}
		}
		CodeTypeDeclaration GetCodeTypeWrapper(CodeObject target, CodeGenerationContext context) 
		{
			string lWrapperName = STR_WrapperName;
			if (target is CodeConstructor)
				lWrapperName = ((CodeConstructor)target).Name;
			if (target is CodeTypeConstructor)
				lWrapperName = ((CodeTypeConstructor)target).Name;
			CodeTypeDeclaration lWrapper = new CodeTypeDeclaration(lWrapperName);
			SetCodeGenContext(lWrapper, context);
			return lWrapper;
		}
		void GetGeneratorTypeAndObject(out Type type, out object generator)
		{
			generator = CodeGenerator;
			type = null;
			if (generator != null)
				type = typeof(CodeGenerator);
			else
			{
				generator = CodeGeneratorAsObject;
				type = generator.GetType();
			}
		}
		void InvokeMethod(Type type, object target, string method, params object[] args)
		{
			MethodInfo lMethodInfo = type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance);
			lMethodInfo.Invoke(target, args);
		}
		void SetFieldValue(Type type, object target, string field, object value)
		{
			FieldInfo lField = type.GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
			lField.SetValue(target, value);
		}		
		void PrepareGenerator(Type type, object generator, CodeTypeDeclaration wrapper, TextWriter output)
		{
			SetFieldValue(type, generator, STR_CurrentClassField, wrapper);
			SetFieldValue(type, generator, STR_OutputField, new IndentedTextWriter(output));
			SetFieldValue(type, generator, STR_OptionsField, _Options);
		}
		void GenerateField(CodeObject target, TextWriter output, bool isEnum)
		{
			CodeGenerationContext lContext = CodeGenerationContext.Class;
			if (isEnum)
				lContext = CodeGenerationContext.Enum;
			GenerateCode(STR_GenerateField, target, output, lContext, CodeGenParameterUsage.Target, null);
		}
		void GenerateCode(string methodName, TextWriter output, object[] parameters)
		{
			GenerateCode(methodName, null, output, CodeGenerationContext.Class, CodeGenParameterUsage.ParameterList, parameters);
		}
		void GenerateCode(string methodName, CodeObject target, TextWriter output)
		{
			GenerateCode(methodName, target, CodeGenerationContext.Class, output);
		}	
		void GenerateCode(string methodName, CodeObject target, CodeGenerationContext context, TextWriter output)
		{
			GenerateCode(methodName, target, output, context, CodeGenParameterUsage.TargetWrapper, null);
		}
		void GenerateCode(string methodName, CodeObject target, TextWriter output, CodeGenerationContext context, CodeGenParameterUsage usage, object[] parameters)
		{
			try
			{
				if (CodeProvider == null)
					return;
				object lGenerator;
				Type lType;
				GetGeneratorTypeAndObject(out lType, out lGenerator);
				CodeTypeDeclaration lWrapper = GetCodeTypeWrapper(target, context);				
				object[] lParameters = GetCodeGenMethodParameters(usage, target, lWrapper, parameters);
				PrepareGenerator(lType, lGenerator, lWrapper, output);
				InvokeMethod(lType, lGenerator, methodName, lParameters);
			}
			catch (Exception e)
			{
				Log.SendException(e);
			}
		}		
		#region GenerateCode
		public string GenerateCode(CodeCompileUnit unit)
		{
			StringBuilder lBuilder = new StringBuilder();
			StringWriter lOutput = new StringWriter(lBuilder);
			GenerateCode(unit, lOutput);
			return lBuilder.ToString();
		}
		#endregion
		#region GenerateCode
		public void GenerateCode(CodeCompileUnit unit, TextWriter output)
		{
			if (ICodeGenerator != null)
				ICodeGenerator.GenerateCodeFromCompileUnit(unit, output, _Options);
		}
		#endregion
		#region GenerateCode
		public string GenerateCode(CodeNamespace space)
		{
			StringBuilder lBuilder = new StringBuilder();
			StringWriter lOutput = new StringWriter(lBuilder);
			GenerateCode(space, lOutput);
			return lBuilder.ToString();
		}
		#endregion
		#region GenerateCode
		public void GenerateCode(CodeNamespace space, TextWriter output)
		{
			if (ICodeGenerator != null)
				ICodeGenerator.GenerateCodeFromNamespace(space, output, _Options);
		}
		#endregion
		#region GenerateCode
		public string GenerateCode(CodeTypeDeclaration type)
		{
			StringBuilder lBuilder = new StringBuilder();
			StringWriter lOutput = new StringWriter(lBuilder);
			GenerateCode(type, lOutput);
			return lBuilder.ToString();
		}
		#endregion
		#region GenerateCode
		public void GenerateCode(CodeTypeDeclaration type, TextWriter output)
		{
			if (ICodeGenerator != null)
				ICodeGenerator.GenerateCodeFromType(type, output, _Options);
		}
		#endregion
		#region GenerateCode
		public string GenerateCode(CodeMemberEvent e)
		{
			StringBuilder lBuilder = new StringBuilder();
			StringWriter lOutput = new StringWriter(lBuilder);
			GenerateCode(e, lOutput);
			return lBuilder.ToString();
		}
		#endregion
		#region GenerateCode
		public void GenerateCode(CodeMemberEvent e, TextWriter output)
		{
			GenerateCode(STR_GenerateEvent, e, output);
		}
		#endregion
		#region GenerateCode
		public string GenerateCode(CodeMemberField field)
		{
			StringBuilder lBuilder = new StringBuilder();
			StringWriter lOutput = new StringWriter(lBuilder);
			GenerateCode(field, lOutput);
			return lBuilder.ToString();
		}
		#endregion
		#region GenerateCode
		public void GenerateCode(CodeMemberField field, TextWriter output)
		{
			GenerateField(field, output, false);
		}
		#endregion
		#region GenerateCode
		public string GenerateCode(CodeMemberField field, bool isEnum)
		{
			StringBuilder lBuilder = new StringBuilder();
			StringWriter lOutput = new StringWriter(lBuilder);
			GenerateCode(field, isEnum, lOutput);
			return lBuilder.ToString();
		}
		#endregion
		#region GenerateCode
		public void GenerateCode(CodeMemberField field, bool isEnum, TextWriter output)
		{
			GenerateField(field, output, isEnum);
		}
		#endregion
		#region GenerateCode
		public string GenerateCode(CodeMemberMethod method)
		{
			StringBuilder lBuilder = new StringBuilder();
			StringWriter lOutput = new StringWriter(lBuilder);
			GenerateCode(method, lOutput);
			return lBuilder.ToString();
		}
		#endregion
		#region GenerateCode
		public string GenerateCode(CodeMemberMethod method, CodeGenerationContext context)
		{
			StringBuilder lBuilder = new StringBuilder();
			StringWriter lOutput = new StringWriter(lBuilder);
			GenerateCode(method, context, lOutput);
			return lBuilder.ToString();
		}
		#endregion
		#region GenerateCode
		public void GenerateCode(CodeMemberMethod method, TextWriter output)
		{
			GenerateCode(STR_GenerateMethod, method, output);
		}
		#endregion
		#region GenerateCode
		public void GenerateCode(CodeMemberMethod method, CodeGenerationContext context, TextWriter output)
		{
			GenerateCode(STR_GenerateMethod, method, context, output);
		}
		#endregion
		#region GenerateCode
		public string GenerateCode(CodeConstructor constructor)
		{
			StringBuilder lBuilder = new StringBuilder();
			StringWriter lOutput = new StringWriter(lBuilder);
			GenerateCode(constructor, lOutput);
			return lBuilder.ToString();
		}
		#endregion
		#region GenerateCode
		public void GenerateCode(CodeConstructor constructor, TextWriter output)
		{
			GenerateCode(STR_GenerateConstructor, constructor, output);
		}
		#endregion
		#region GenerateCode
		public string GenerateCode(CodeTypeConstructor constructor)
		{
			StringBuilder lBuilder = new StringBuilder();
			StringWriter lOutput = new StringWriter(lBuilder);
			GenerateCode(constructor, lOutput);
			return lBuilder.ToString();
		}
		#endregion
		#region GenerateCode
		public void GenerateCode(CodeTypeConstructor constructor, TextWriter output)
		{
			GenerateCode(STR_GenerateTypeConstructor, constructor, output, CodeGenerationContext.Class, CodeGenParameterUsage.Target, null);
		}
		#endregion
		#region GenerateCode
		public string GenerateCode(CodeEntryPointMethod method)
		{
			StringBuilder lBuilder = new StringBuilder();
			StringWriter lOutput = new StringWriter(lBuilder);
			GenerateCode(method, lOutput);
			return lBuilder.ToString();
		}
		#endregion
		#region GenerateCode
		public void GenerateCode(CodeEntryPointMethod method, TextWriter output)
		{
			GenerateCode(STR_GenerateEntryPointMethod, method, output);
		}
		#endregion
		#region GenerateCode
		public string GenerateCode(CodeMemberProperty property)
		{
			StringBuilder lBuilder = new StringBuilder();
			StringWriter lOutput = new StringWriter(lBuilder);
			GenerateCode(property, lOutput);
			return lBuilder.ToString();
		}
		#endregion
		#region GenerateCode
		public string GenerateCode(CodeMemberProperty property, CodeGenerationContext context)
		{
			StringBuilder lBuilder = new StringBuilder();
			StringWriter lOutput = new StringWriter(lBuilder);
			GenerateCode(property, context, lOutput);
			return lBuilder.ToString();
		}
		#endregion
		#region GenerateCode
		public void GenerateCode(CodeMemberProperty property, TextWriter output)
		{
			GenerateCode(STR_GenerateProperty, property, output);
		}
		#endregion
		#region GenerateCode
		public void GenerateCode(CodeMemberProperty property, CodeGenerationContext context, TextWriter output)
		{
			GenerateCode(STR_GenerateProperty, property, context, output);
		}
		#endregion
		#region GenerateCode
		public string GenerateCode(CodeStatement statement)
		{
			StringBuilder lBuilder = new StringBuilder();
			StringWriter lOutput = new StringWriter(lBuilder);
			GenerateCode(statement, lOutput);
			return lBuilder.ToString();
		}
		#endregion
		#region GenerateCode
		public void GenerateCode(CodeStatement statement, TextWriter output)
		{
			if (ICodeGenerator != null)
				ICodeGenerator.GenerateCodeFromStatement(statement, output, _Options);
		}
		#endregion
		#region GenerateCode
		public string GenerateCode(CodeExpression expression)
		{
			StringBuilder lBuilder = new StringBuilder();
			StringWriter lOutput = new StringWriter(lBuilder);
			GenerateCode(expression, lOutput);
			return lBuilder.ToString();
		}
		#endregion
		#region GenerateCode
		public void GenerateCode(CodeExpression expression, TextWriter output)
		{
			if (ICodeGenerator != null)
				ICodeGenerator.GenerateCodeFromExpression(expression, output, _Options);
		}
		#endregion
		#region GenerateCode
		public string GenerateCode(CodeAttributeDeclarationCollection attributes)
		{
			StringBuilder lBuilder = new StringBuilder();
			StringWriter lOutput = new StringWriter(lBuilder);
			GenerateCode(attributes, lOutput);
			return lBuilder.ToString();
		}
		#endregion
		#region GenerateCode
		public void GenerateCode(CodeAttributeDeclarationCollection attributes, TextWriter output)
		{
			GenerateCode(STR_GenerateAttributes, output, new object[] {attributes, null, false});
		}
		#endregion
		#region CodeProvider
		public CodeDomProvider CodeProvider
		{
			get
			{
				return _CodeProvider;
			}
			set
			{
				_CodeProvider = value;
			}
		}
		#endregion
		#region Options
		public CodeGeneratorOptions Options
		{
			get
			{
				return _Options;
			}
			set
			{
				_Options = value;
			}
		}
		#endregion
#pragma warning disable 0618
		#region CodeGenerator
		CodeGenerator CodeGenerator
		{
			get
			{
				if (CodeProvider == null)
				{
					Log.SendWarning("CodeDomServices.get_CodeGenerator: CodeProvider is null!");
					return null;
				}
				return CodeProvider.CreateGenerator() as CodeGenerator;
			}
		}
		#endregion
		#region CodeGeneratorAsObject
		object CodeGeneratorAsObject
		{
			get
			{
				if (CodeProvider == null)
				{
					Log.SendWarning("CodeDomServices.CodeGeneratorAsObject: CodeProvider is null!");
					return null;
				}
				return CodeProvider.CreateGenerator();
			}
		}
		#endregion
		#region ICodeGenerator
		ICodeGenerator ICodeGenerator
		{
			get
			{
				if (CodeProvider == null)
				{
					Log.SendWarning("CodeDomServices.get_ICodeGenerator: CodeProvider is null!");
					return null;
				}
				return CodeProvider.CreateGenerator();
			}
		}
		#endregion
#pragma warning restore 0618
  }
}
