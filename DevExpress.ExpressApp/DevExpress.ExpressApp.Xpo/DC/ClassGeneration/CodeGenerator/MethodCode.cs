#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo.Helpers;
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	internal class MethodCode : MethodCodeBase {
		public MethodCode(string name, string typeFullName) : base(name, typeFullName) { }
		public MethodCode(string name, string typeFullName, ParameterCode[] param)
			: base(name, typeFullName) {
				Initialize(param);
		}
		public MethodCode(MethodMetadata methodMetadata)
			: base(methodMetadata.Name, CodeBuilder.TypeToString(methodMetadata.ReturnType)) {
				Initialize(methodMetadata);
		}
		private void Initialize(ParameterCode[] param) {
			foreach(ParameterCode p in param) {
				AddParameterCore(p);
			}
		}
		private void Initialize(MethodMetadata methodMetadata) {
			foreach(ParameterMetadata parameterMetadata in methodMetadata.Parameters) {
				Type parameterType = parameterMetadata.Type.IsByRef ? parameterMetadata.Type.UnderlyingSystemType : parameterMetadata.Type;
				ParameterCode paramterCode = new ParameterCode(parameterMetadata.Name, CodeBuilder.TypeToString(parameterType));
				paramterCode.IsByRef = parameterMetadata.Type.IsByRef;
				paramterCode.IsOut = parameterMetadata.IsOut;
				AddParameterCore(paramterCode);
			}
		}
		private string GetterLogicInvokeCode(MethodLogic methodLogic, IEnumerable<ParameterCode> parameterCodes) {
			if(methodLogic.IsAccessor) {
				return string.Format("return {0}.{1};", LogicName(methodLogic), LogicMemberName(methodLogic));
			}
			else {
				string parametersString = GetLogicParametersString(methodLogic, parameterCodes);
				return string.Format("return {0}.{1}({2});", LogicName(methodLogic), LogicMemberName(methodLogic), parametersString);
			}
		}
		private string SetterLogicInvokeCode(MethodLogic methodLogic, IEnumerable<ParameterCode> parameterCodes) {
			if(methodLogic.IsAccessor) {
				return string.Format("{0}.{1} = value;", LogicName(methodLogic), LogicMemberName(methodLogic));
			}
			else {
				string parametersString = GetLogicParametersString(methodLogic, parameterCodes);
				return string.Format("{0}.{1}({2});", LogicName(methodLogic), LogicMemberName(methodLogic), parametersString);
			}
		}
		private string LogicName(MethodLogic methodLogic) {
			return methodLogic.IsStatic ? CodeBuilder.TypeToString(methodLogic.Owner) : DcSpecificWords.FieldLogicInstancePrefix + methodLogic.Owner.Name;
		}
		protected string GetLogicParametersString(MethodLogic methodLogic, IEnumerable<ParameterCode> parameterCodes) {
			List<string> parameters = new List<string>();
			if(methodLogic.HasInstanceParameter) {
				parameters.Add("this");
			}
			if(methodLogic.HasObjectSpaceParameter) {
				parameters.Add(string.Format("{0}.FindObjectSpaceByObject(this)", CodeBuilder.TypeToString(typeof(XPObjectSpace))));
			}
			foreach(ParameterCode parameter in parameterCodes) {
				parameters.Add(parameter.GetCode(false));
			}
			return string.Join(", ", parameters.ToArray());
		}
		protected string GetLogicInvokeCode(MethodLogic methodLogic, string returnType, IEnumerable<ParameterCode> parameterCodes) {
			if(returnType == "void") {
				return SetterLogicInvokeCode(methodLogic, parameterCodes);
			}
			else {
				return GetterLogicInvokeCode(methodLogic, parameterCodes);
			}
		}
		protected virtual string LogicMemberName(MethodLogic methodLogic) {
			return methodLogic.IsAccessor ? Name : methodLogic.Name;
		}
		protected virtual void GetMethodBody(CodeBuilder builder) {
			if(TypeFullName != "void") {
				builder.AppendLineFormat("return default({0});", TypeFullName);
			}
		}
		protected override void GetCodeCore(CodeBuilder builder) {
			base.GetCodeCore(builder);
			builder.Append(string.Format("{0} {1}(", TypeFullName, Name));
			AppendParameters(builder);
			if(Virtuality == Virtuality.Abstract) {
				builder.Append(");").AppendNewLine();
			}
			else {
				builder.Append(") {").AppendNewLine();
				builder.PushIndent();
				GetMethodBody(builder);
				builder.PopIndent();
				builder.AppendLine("}");
			}
		}
	}
	internal interface ILogicsProvider {
		IEnumerable<MethodLogic> Logics { get; }
	}
	internal class PropertyLogicMethodCode : MethodCode, ILogicsProvider {
		MethodLogic methodLogic;
		string propertyName;
		public PropertyLogicMethodCode(string name, string propertyName, Type type, ParameterCode[] parameters, MethodLogic methodLogic)
			: base(name, CodeBuilder.TypeToString(type), parameters) {
			this.propertyName = propertyName;
			this.methodLogic = methodLogic;
		}
		protected override void GetMethodBody(CodeBuilder builder) {
			builder.AppendLine(GetLogicInvokeCode(methodLogic, TypeFullName, Parameters));
		}
		protected override string LogicMemberName(MethodLogic methodLogic) {
			return methodLogic.IsAccessor ? propertyName : methodLogic.Name;
		}
		#region ILogicsProvider Members
		public IEnumerable<MethodLogic> Logics {
			get { return new MethodLogic[] { methodLogic }; }
		}
		#endregion
	}
	internal class LogicMethodCode : MethodCode, ILogicsProvider {
		MethodLogic methodLogic;
		public LogicMethodCode(MethodMetadata methodMetadata, MethodLogic methodLogic)
			: base(methodMetadata) {
			this.methodLogic = methodLogic;
		}
		protected override void GetMethodBody(CodeBuilder builder) {
			builder.AppendLine(GetLogicInvokeCode(methodLogic, TypeFullName, Parameters));
		}
		#region ILogicMethod Members
		public IEnumerable<MethodLogic> Logics {
			get { return new MethodLogic[] { methodLogic }; }
		}
		#endregion
	}
	internal class ClassLogicMethodCode : MethodCode, ILogicsProvider {
		private string logicMethodName;
		private IEnumerable<MethodLogic> methodLogics;
		private EntityMetadata entityMetadata;
		protected void AppendBaseClassMethodInvoke(CodeBuilder builder) {
			builder.AppendLineFormat("base.{0}();", Name);
		}
		protected void AppendLogicMethodInvokes(CodeBuilder builder) {
			foreach(MethodLogic methodLogic in Logics) {
				string logicName = methodLogic.IsStatic ?
					CodeBuilder.TypeToString(methodLogic.Owner) :
					DcSpecificWords.FieldLogicInstancePrefix + methodLogic.Owner.Name;
				if(!methodLogic.IsStatic) {
					InitLogicIfNeed(builder, methodLogic.Owner, logicName, EntityMetadata);
				}
				string parametersString = GetLogicParametersString(methodLogic, Parameters);
				builder.AppendLineFormat("{0}.{1}({2});", logicName, logicMethodName, parametersString);
			}
		}
		public ClassLogicMethodCode(string name, string logicMethodName, IEnumerable<MethodLogic> methodLogics, EntityMetadata entityMetadata)
			: base(name, "void") {
			Virtuality = Virtuality.Override;
			Visibility = Visibility.Protected;
			this.methodLogics = methodLogics;
			this.logicMethodName = logicMethodName;
			this.entityMetadata = entityMetadata;
		}
		protected override void GetMethodBody(CodeBuilder builder) {
			AppendBaseClassMethodInvoke(builder);
			AppendLogicMethodInvokes(builder);
		}
		protected EntityMetadata EntityMetadata {
			get {
				return entityMetadata;
			}
		}
		public IEnumerable<MethodLogic> Logics {
			get {
				return methodLogics;
			}
		}
	}
	internal class AfterConstructionMethodCode : ClassLogicMethodCode {
		private IEnumerable<Type> logicTypes;
		public AfterConstructionMethodCode(EntityMetadata entityMetadata, IEnumerable<MethodLogic> methodLogics, IEnumerable<Type> logicTypes)
			: base("AfterConstruction", DcSpecificWords.LogicAfterConstruction, methodLogics, entityMetadata) {
			this.Visibility = Visibility.Public;
			this.logicTypes = logicTypes;
		}
		protected override void GetMethodBody(CodeBuilder builder) {
			AppendBaseClassMethodInvoke(builder);
			if(EntityMetadata.NeedInitializeKeyProperty) {
				builder.AppendLine("this.Oid = System.Guid.NewGuid();");
			}
			foreach(DataMetadata dataMetadata in EntityMetadata.GetSortedAggregatedData()) {
				string entityDataClassName = CodeModelGeneratorHelper.GetEntityDataClassName(dataMetadata, EntityMetadata.Name);
				string dataClassFieldName = CodeModelGeneratorHelper.GetDataClassFieldName(dataMetadata);
				builder.AppendLineFormat("{0} = new {1}(this);", dataClassFieldName, entityDataClassName);
			}
			foreach(CodeModelPropertyInfo propertyInfo in CodeModelGeneratorHelper.GetClassPropertyInfos(EntityMetadata, false)) {
				if(propertyInfo.PropertyMetadata.FindAttribute<DefaultValueAttribute>() != null) {
					string propertyName = propertyInfo.PropertyMetadata.Name;
					string propertyTypeFullName = CodeBuilder.TypeToString(propertyInfo.PropertyMetadata.PropertyType);
					string defaultValueAttributeTypeFullName = CodeBuilder.TypeToString(typeof(DefaultValueAttribute));
					string interfaceTypeFullName = CodeBuilder.TypeToString(propertyInfo.InterfaceMetadata.InterfaceType);
					builder.AppendLineFormat(@"{0} = ({1})(({2})typeof({3}).GetProperty(""{0}"").GetCustomAttributes(typeof({2}), false)[0]).Value;",
						propertyName, propertyTypeFullName, defaultValueAttributeTypeFullName, interfaceTypeFullName);
				}
			}
			foreach(Type logicType in this.logicTypes) {
				string logicName = DcSpecificWords.FieldLogicInstancePrefix + logicType.Name;
				InitLogicIfNeed(builder, logicType, logicName, EntityMetadata);
			}
			AppendLogicMethodInvokes(builder);
		}
	}
	internal class CreateInstanceMethodCode : MethodCode {
		private string entityClassName;
		private CreateInstanceMethodCode(string name, string typeFullName) : base(name, typeFullName) { }
		private CreateInstanceMethodCode(string name, string typeFullName, ParameterCode[] param)
			: base(name, typeFullName, param) { }
		public CreateInstanceMethodCode(MethodMetadata methodMetadata, string entityClassName)
			: base(methodMetadata) {
			this.entityClassName = entityClassName;
		}
		protected override void GetMethodBody(CodeBuilder builder) {
			builder.AppendLineFormat("return new {0}(Session);", entityClassName);
		}
	}
	internal class ObjectChangedMethodCode : MethodCode {
		public const string ObjectChangedMethodName = "OnChanged";
		private readonly Dictionary<PropertyMetadata, MethodLogic> propertyChangedLogicsInfo;
		private readonly IList<PropertyMetadata> aliasedPropertyMetadata;
		private readonly bool isEntity;
		private readonly bool isBaseEntity;
		public ObjectChangedMethodCode(Dictionary<PropertyMetadata, MethodLogic> changedPropertyLogics, IList<PropertyMetadata> aliasedPropertyMetadata, bool isEntity, bool isBaseEntity)
			: base(ObjectChangedMethodName, "void", new ParameterCode[] { 
				new ParameterCode("propertyName", CodeBuilder.TypeToString(typeof(string))), 
				new ParameterCode("oldValue", CodeBuilder.TypeToString(typeof(object))),
				new ParameterCode("newValue", CodeBuilder.TypeToString(typeof(object)))
			}) {
			Virtuality = Virtuality.Override;
			Visibility = Visibility.Protected;
			this.propertyChangedLogicsInfo = changedPropertyLogics;
			this.aliasedPropertyMetadata = aliasedPropertyMetadata;
			this.isEntity = isEntity;
			this.isBaseEntity = isBaseEntity;
		}
		protected override void GetCodeCore(CodeBuilder builder) {
			if(isBaseEntity) {
				IPropertyChangeNotificationReceiverHelper.AppendPropertyChangedImplementation(builder,
					delegate(CodeBuilder codeBuilder) {
						codeBuilder.AppendLineFormat("{0}(propertyName, oldValue, newValue);", ObjectChangedMethodName);
					}
				);
			}
			if(NeedAddMethod) {
				base.GetCodeCore(builder);
			}
		}
		private void GetAliasedPropertiesFireChangedCode(CodeBuilder builder) {
			foreach(PropertyMetadata propertyMetadata in aliasedPropertyMetadata) {
				builder.AppendLineFormat("if (propertyName == \"{0}\") {{", CodeModelGeneratorHelper.GetAliasPropertyName(propertyMetadata.Name));
				builder.PushIndent();
				Type associatedType = CodeModelGeneratorHelper.GetAssociatedType(propertyMetadata);
				Type persistentInterfaceDataType = typeof(IPersistentInterfaceData<>).MakeGenericType(associatedType);
				builder.AppendLineFormat("chobj.{0}(\"{1}\", oldValue == null ? null : (({2})oldValue).Instance, newValue == null ? null : (({2})newValue).Instance);",
					IPropertyChangeNotificationReceiverHelper.PropertyChangedMethodName,
					propertyMetadata.Name,
					CodeBuilder.TypeToString(persistentInterfaceDataType)
				);
				builder.AppendLine("return;");
				builder.PopIndent();
				builder.AppendLine("}");
			}
		}
		protected override void GetMethodBody(CodeBuilder builder) {
			builder.AppendLineFormat("base.{0}(propertyName, oldValue, newValue);", ObjectChangedMethodName);
			if(isEntity) {
				builder.AppendLine("if (this.IsLoading) return;");
				if(aliasedPropertyMetadata.Count > 0) {
					IPropertyChangeNotificationReceiverHelper.AppendDirectCast(builder, "this", "chobj");
					GetAliasedPropertiesFireChangedCode(builder);
				}
				if(isBaseEntity) {
					builder.AppendLineFormat("{0}(propertyName, oldValue, newValue);", InvokeAfterChangeLogicMethodCode.MethodName);
				}
			}
			else {
				IPropertyChangeNotificationReceiverHelper.AppendSafeCastScope(builder, "Instance", "chobj",
					delegate(CodeBuilder codeBuilder) {
						if(aliasedPropertyMetadata.Count > 0) {
							GetAliasedPropertiesFireChangedCode(codeBuilder);
						}
						codeBuilder.AppendLineFormat("chobj.{0}(propertyName, oldValue, newValue);", IPropertyChangeNotificationReceiverHelper.PropertyChangedMethodName);
					}
				); 
			}
		}
		private bool NeedAddMethod {
			get {
				if(isEntity) {
					return isBaseEntity || aliasedPropertyMetadata.Count > 0;
				}
				return true;
			}
		}
	}
	internal class InvokeAfterChangeLogicMethodCode : MethodCode, ILogicsProvider {
		public const string MethodName = "InvokeAfterChangeLogic";
		private readonly Dictionary<PropertyMetadata, MethodLogic> propertyChangedLogicsInfo;
		private readonly bool isBaseEntity;
		public InvokeAfterChangeLogicMethodCode(Dictionary<PropertyMetadata, MethodLogic> changedPropertyLogics, bool isBaseEntity)
			: base(MethodName, "void", new ParameterCode[] { 
				new ParameterCode("propertyName", CodeBuilder.TypeToString(typeof(string))), 
				new ParameterCode("oldValue", CodeBuilder.TypeToString(typeof(object))),
				new ParameterCode("newValue", CodeBuilder.TypeToString(typeof(object)))
			}) {
			this.propertyChangedLogicsInfo = changedPropertyLogics;
			this.isBaseEntity = isBaseEntity;
			Visibility = Visibility.Protected;
			Virtuality = isBaseEntity ? Virtuality.Virtual : Virtuality.Override;
		}
		protected override void GetCodeCore(CodeBuilder builder) {
			if(NeedAddMethod) {
				base.GetCodeCore(builder);
			}
		}
		protected override void GetMethodBody(CodeBuilder builder) {
			if(!isBaseEntity) {
				builder.AppendLineFormat("base.{0}(propertyName, oldValue, newValue);", MethodName);
			}
			foreach(KeyValuePair<PropertyMetadata, MethodLogic> logicInfo in propertyChangedLogicsInfo) {
				builder.AppendLineFormat("if (propertyName == \"{0}\") {{", logicInfo.Key.Name);
				builder.PushIndent();
				builder.AppendLine(GetLogicInvokeCode(logicInfo.Value, "void", new ParameterCode[] { }));
				builder.AppendLine("return;");
				builder.PopIndent();
				builder.AppendLine("}");
			}
		}
		#region ILogicsProvider Members
		public IEnumerable<MethodLogic> Logics {
			get {
				if(propertyChangedLogicsInfo != null) {
					return propertyChangedLogicsInfo.Values;
				}
				return new MethodLogic[0];
			}
		}
		#endregion
		private bool NeedAddMethod {
			get { return isBaseEntity || (propertyChangedLogicsInfo != null && propertyChangedLogicsInfo.Count > 0); }
		}
	}
	internal class ObjectChangingMethodCode : MethodCode {
		public const string ObjectChangingMethodName = "OnChanging";
		private readonly Dictionary<PropertyMetadata, MethodLogic> propertyChangingLogicsInfo;
		private readonly IList<PropertyMetadata> aliasedPropertyMetadata;
		private readonly bool isEntity;
		private readonly bool isBaseEntity;
		public ObjectChangingMethodCode(Dictionary<PropertyMetadata, MethodLogic> changingPropertyLogics, IList<PropertyMetadata> aliasedPropertyMetadata, bool isEntity, bool isBaseEntity)
			: base(ObjectChangingMethodName, "void", new ParameterCode[] { 
				new ParameterCode("propertyName", CodeBuilder.TypeToString(typeof(string))), 
				new ParameterCode("newValue", CodeBuilder.TypeToString(typeof(object)))
			}) {
			this.propertyChangingLogicsInfo = changingPropertyLogics;
			this.aliasedPropertyMetadata = aliasedPropertyMetadata;
			this.isEntity = isEntity;
			this.isBaseEntity = isBaseEntity;
			Visibility = isEntity ? Visibility.Protected : Visibility.Private;
			if(isEntity) {
				Virtuality = isBaseEntity ? Virtuality.Virtual : Virtuality.Override;
			}
		}
		protected override void GetCodeCore(CodeBuilder builder) {
			if(isBaseEntity) {
				IPropertyChangeNotificationReceiverHelper.AppendPropertyChangingImplementation(builder,
					delegate(CodeBuilder codeBuilder) {
						codeBuilder.AppendLineFormat("{0}(propertyName, newValue);", ObjectChangingMethodName);
					}
				);
			}
			if(NeedAddMethod) {
				base.GetCodeCore(builder);
			}
		}
		private void GetAliasedPropertiesFireChangingCode(CodeBuilder builder) {
			foreach(PropertyMetadata propertyMetadata in aliasedPropertyMetadata) {
				builder.AppendLineFormat("if (propertyName == \"{0}\") {{", CodeModelGeneratorHelper.GetAliasPropertyName(propertyMetadata.Name));
				builder.PushIndent();
				Type associatedType = CodeModelGeneratorHelper.GetAssociatedType(propertyMetadata);
				Type persistentInterfaceDataType = typeof(IPersistentInterfaceData<>).MakeGenericType(associatedType);
				builder.AppendLineFormat("chobj.{0}(\"{1}\", newValue == null ? null : (({2})newValue).Instance);",
					IPropertyChangeNotificationReceiverHelper.PropertyChangingMethodName,
					propertyMetadata.Name,
					CodeBuilder.TypeToString(persistentInterfaceDataType)
				);
				builder.AppendLine("return;");
				builder.PopIndent();
				builder.AppendLine("}");
			}
		}
		protected override void GetMethodBody(CodeBuilder builder) {
			if(isEntity) {
				if(!isBaseEntity) {
					builder.AppendLineFormat("base.{0}(propertyName, newValue);", ObjectChangingMethodName);
				}
				builder.AppendLine("if (this.IsLoading) return;");
				if(aliasedPropertyMetadata.Count > 0 && propertyChangingLogicsInfo.Count > 0) {
					IPropertyChangeNotificationReceiverHelper.AppendDirectCast(builder, "this", "chobj");
					GetAliasedPropertiesFireChangingCode(builder);
				}
				if(isBaseEntity) {
					builder.AppendLineFormat("{0}(propertyName, newValue);", InvokeBeforeChangeLogicMethodCode.MethodName);
				}
			}
			else {
				IPropertyChangeNotificationReceiverHelper.AppendSafeCastScope(builder, "Instance", "chobj",
					delegate(CodeBuilder codeBuilder) {
						if(aliasedPropertyMetadata.Count > 0 && propertyChangingLogicsInfo.Count > 0) {
							GetAliasedPropertiesFireChangingCode(codeBuilder);
						}
						codeBuilder.AppendLineFormat("chobj.{0}(propertyName, newValue);", IPropertyChangeNotificationReceiverHelper.PropertyChangingMethodName);
					}
				);
			}
		}
		private bool NeedAddMethod {
			get {
				if(isEntity) {
					return isBaseEntity || aliasedPropertyMetadata.Count > 0;
				}
				return true;
			}
		}
	}
	internal class InvokeBeforeChangeLogicMethodCode : MethodCode, ILogicsProvider {
		public const string MethodName = "InvokeBeforeChangeLogic";
		private readonly Dictionary<PropertyMetadata, MethodLogic> propertyChangingLogicsInfo;
		private readonly bool isBaseEntity;
		public InvokeBeforeChangeLogicMethodCode(Dictionary<PropertyMetadata, MethodLogic> changingPropertyLogics, bool isBaseEntity)
			: base(MethodName, "void", new ParameterCode[] { 
				new ParameterCode("propertyName", CodeBuilder.TypeToString(typeof(string))), 
				new ParameterCode("newValue", CodeBuilder.TypeToString(typeof(object)))
			}) {
			this.propertyChangingLogicsInfo = changingPropertyLogics;
			this.isBaseEntity = isBaseEntity;
			Visibility = Visibility.Protected;
			Virtuality = isBaseEntity ? Virtuality.Virtual : Virtuality.Override;
		}
		protected override void GetCodeCore(CodeBuilder builder) {
			if(NeedAddMethod) {
				base.GetCodeCore(builder);
			}
		}
		protected override void GetMethodBody(CodeBuilder builder) {
			if(!isBaseEntity) {
				builder.AppendLineFormat("base.{0}(propertyName, newValue);", MethodName);
			}
			foreach(KeyValuePair<PropertyMetadata, MethodLogic> logicInfo in propertyChangingLogicsInfo) {
				builder.AppendLineFormat("if (propertyName == \"{0}\") {{", logicInfo.Key.Name);
				builder.PushIndent();
				builder.AppendLine(GetLogicInvokeCode(logicInfo.Value, "void", new ParameterCode[] { new ParameterCode("newValue", CodeBuilder.TypeToString(logicInfo.Key.PropertyType)) }));
				builder.AppendLine("return;");
				builder.PopIndent();
				builder.AppendLine("}");
			}
		}
		#region ILogicsProvider Members
		public IEnumerable<MethodLogic> Logics {
			get {
				if(propertyChangingLogicsInfo != null) {
					return propertyChangingLogicsInfo.Values;
				}
				return new MethodLogic[0];
			}
		}
		#endregion
		private bool NeedAddMethod {
			get { return isBaseEntity || (propertyChangingLogicsInfo != null && propertyChangingLogicsInfo.Count > 0); }
		}
	}
	internal class CustomMethodCode : MethodCode {
		private Action<CodeBuilder> getMethodBody;
		public CustomMethodCode(string name, string typeFullName, ParameterCode[] param, Action<CodeBuilder> getMethodBody)
			: base(name, typeFullName, param) {
			Guard.ArgumentNotNull(getMethodBody, "getMethodBody");
			this.getMethodBody = getMethodBody;
		}
		protected override void GetMethodBody(CodeBuilder builder) {
			getMethodBody(builder);
		}
	}
	public interface IPropertyChangeNotificationReceiver {
		void PropertyChanging(string propertyName, object newValue);
		void PropertyChanged(string propertyName, object oldValue, object newValue);
	}
	internal class IPropertyChangeNotificationReceiverHelper {
		public const string PropertyChangingMethodName = "PropertyChanging";
		public const string PropertyChangedMethodName = "PropertyChanged";
		private static readonly Type InterfaceType;
		private static readonly string InterfaceTypeString;
		private static readonly string PropertyChangingMethodDeclarationName;
		private static readonly string PropertyChangedMethodDeclarationName;
		private static readonly ParameterCode PropertyNameParameter;
		private static readonly ParameterCode OldValueParameter;
		private static readonly ParameterCode NewValueParameter;
		static IPropertyChangeNotificationReceiverHelper() {
			InterfaceType = typeof(IPropertyChangeNotificationReceiver);
			InterfaceTypeString = CodeBuilder.TypeToString(InterfaceType);
			PropertyChangingMethodDeclarationName = InterfaceTypeString + "." + PropertyChangingMethodName;
			PropertyChangedMethodDeclarationName = InterfaceTypeString + "." + PropertyChangedMethodName;
			PropertyNameParameter = new ParameterCode("propertyName", CodeBuilder.TypeToString(typeof(string)));
			OldValueParameter = new ParameterCode("oldValue", CodeBuilder.TypeToString(typeof(object)));
			NewValueParameter = new ParameterCode("newValue", CodeBuilder.TypeToString(typeof(object)));
		}
		public static void AppendPropertyChangingImplementation(CodeBuilder builder, Action<CodeBuilder> getBody) {
			ParameterCode[] parameters = new ParameterCode[] { PropertyNameParameter, NewValueParameter };
			MethodCode method = new CustomMethodCode(PropertyChangingMethodDeclarationName, "void", parameters, getBody);
			method.Visibility = Visibility.None;
			method.GetCode(builder);
		}
		public static void AppendPropertyChangedImplementation(CodeBuilder builder, Action<CodeBuilder> getBody) {
			ParameterCode[] parameters = new ParameterCode[] { PropertyNameParameter, OldValueParameter, NewValueParameter };
			MethodCode method = new CustomMethodCode(PropertyChangedMethodDeclarationName, "void", parameters, getBody);
			method.Visibility = Visibility.None;
			method.GetCode(builder);
		}
		public static void AppendSafeCastScope(CodeBuilder builder, string theObject, string tempVarName, Action<CodeBuilder> getBody) {
			builder.AppendLineFormat("{0} {1} = {2} as {0};", InterfaceTypeString, tempVarName, theObject);
			builder.AppendLineFormat("if ({0} != null) {{", tempVarName);
			builder.PushIndent();
			getBody(builder);
			builder.PopIndent();
			builder.AppendLine("}");
		}
		public static void AppendDirectCast(CodeBuilder builder, string theObject, string tempVarName) {
			builder.AppendLineFormat("{0} {1} = ({0}){2};", InterfaceTypeString, tempVarName, theObject);
		}
	}
}
