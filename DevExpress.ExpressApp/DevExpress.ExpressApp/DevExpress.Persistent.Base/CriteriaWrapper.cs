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
using System.Runtime.Serialization;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model.NodeGenerators;
namespace DevExpress.Persistent.Base {
	public class ParameterEventArgs : EventArgs {
		private IParameter parameter;
		public ParameterEventArgs(IParameter parameter) {
			this.parameter = parameter;
		}
		public IParameter Parameter {
			get { return parameter; }
		}
	}
	public class CriteriaWrapper {
		public static CriteriaOperator ParseCriteriaWithReadOnlyParameters(string criteriaText, Type targetObjectType) {
			CriteriaOperator criteria = CriteriaOperator.Parse(criteriaText);
			CriteriaWrapper criteriaWrapper = new CriteriaWrapper(targetObjectType, criteria);
			if(criteriaWrapper.EditableParameters.Count > 0) {
				string message = "Cannot process editable parameters:\n";
				foreach(string name in criteriaWrapper.EditableParameters.Keys) {
					message += "'@" + name + "'\n";
				}
				throw new InvalidOperationException(message);
			}
			criteriaWrapper.UpdateParametersValues();
			return criteriaWrapper.CriteriaOperator;
		}
		public static CriteriaOperator UpgradeOldReadOnlyParameters(Type targetObjectType, CriteriaOperator criteria) {
			CriteriaWrapper criteriaWrapper = new CriteriaWrapper(targetObjectType, criteria);
			return criteriaWrapper.CriteriaOperator;
		}
		public const string ParameterPrefix = "@";
		private CriteriaOperator criteriaOperator;
		private Type objectType;
		private Object currentObject;
		private bool isCreated = false;
		List<OperandParameter> parameters = new List<OperandParameter>();
		List<IParameter> iParameters = new List<IParameter>();
		List<CriteriaWrapperInvalidProperty> invalidProperties = new List<CriteriaWrapperInvalidProperty>();
		List<string> invalidRegisterParameters = new List<string>();
		bool hasJoinOperand = false;
		private static bool HasParameterPrefix(string parameterName) {
			return parameterName != null && parameterName.StartsWith(ParameterPrefix);
		}
		private IParameter FindParameter(string name, bool isCaption) {
			if(name != null) {
				foreach(IParameter parameter in Parameters) {
					if(isCaption) {
						if(parameter.Caption.ToLower() == name.ToLower()) {
							return parameter;
						}
					}
					else {
						if(parameter.Name.ToLower() == name.ToLower()) {
							return parameter;
						}
					}
				}
			}
			return null;
		}
		private void IsCurrentObjectRequired() {
			if(currentObject != null || !isCreated) return;
			foreach(CriteriaWrapperOperandParameter parameter in EditableParameters.Values) {
				if(parameter.IsObjectMember) {
					throw new RequiresCurrentObjectException();
				}
			}
		}
		protected virtual string GetFullMemberCaption(Type objectType, string fullMemberName) {
			return fullMemberName;
		}
		protected CriteriaWrapper(Type objectType, CriteriaOperator criteriaOperator, bool updateParameters, Object currentObject) {
			this.objectType = objectType;
			this.currentObject = currentObject;
			this.criteriaOperator = UpgradeCriteriaOperator(criteriaOperator);
			if(updateParameters) {
				if(currentObject != null) {
					UpdateParametersValues(this.currentObject);
				}
				else {
					UpdateParametersValues();
				}
			}
			isCreated = true;
		}
		protected CriteriaWrapper(Type objectType, CriteriaOperator criteriaOperator, bool updateParameters)
			: this(objectType, criteriaOperator, updateParameters, null) { }
		public CriteriaWrapper(Type objectType, CriteriaOperator criteriaOperator)
			: this(objectType, criteriaOperator, true) { }
		public CriteriaWrapper(Type objectType, string criteriaString) 
			: this(objectType, criteriaString, true) { }
		public CriteriaWrapper(Type objectType, string criteriaString, bool updateParameters) 
			: this(objectType, CriteriaOperator.Parse(criteriaString), updateParameters) { }
		public CriteriaWrapper(Type objectType, CriteriaOperator criteriaOperator, Object currentObject)
			: this(objectType, criteriaOperator, true, currentObject) { }
		public CriteriaWrapper(Type objectType, string criteriaString, Object currentObject)
			: this(objectType, CriteriaOperator.Parse(criteriaString), true, currentObject) { }
		public CriteriaWrapper(CriteriaOperator criteriaOperator, Object currentObject)
			: this(currentObject.GetType(), criteriaOperator, true, currentObject) { }
		public CriteriaWrapper(string criteriaString, Object currentObject)
			: this(currentObject.GetType(), CriteriaOperator.Parse(criteriaString), true, currentObject) { }
		public object GetParameterValue(string name) {
			IParameter parameter = FindParameter(name, false);
			if(parameter != null) {
				return parameter.CurrentValue;
			}
			return null;
		}
		public object GetParameterValue(int index) {
			List<IParameter> parameters = Parameters;
			if ((index >= 0) && (index < parameters.Count)) {
				return parameters[index].CurrentValue;
			}
			return null;
		}
		public void UpdateParametersValues(Object currentObject) {
			Type oldCurrentType = CurrentObjectType;
			this.currentObject = currentObject;
			if(CurrentObjectType != oldCurrentType) {
				this.criteriaOperator = UpgradeCriteriaOperator(CriteriaOperator);
			}
			foreach(CriteriaWrapperOperandParameter parameter in this.parameters) {
				parameter.CurrentObject = currentObject;
			}
		}
		public void UpdateParametersValues() {
			IsCurrentObjectRequired();
		}
		public List<IParameter> Parameters { get { return iParameters; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public List<OperandParameter> OperandParameters { get { return parameters; } }
		[EditorBrowsable(EditorBrowsableState.Never)] 
		public ParameterList EditableParameters { 
			get {
				ParameterList dict = new ParameterList();
				foreach(CriteriaWrapperOperandParameter parameter in Parameters) {
					if(!parameter.IsReadOnly) {
						dict.Add(parameter.Name, parameter);
					}
				}
				return dict; 
			} 
		}
		public bool HasVisibleParameters {
			get {
				foreach(IParameter parameter in Parameters) {
					if(parameter.Visible) {
						return true;
					}
				}
				return false;
			}
		}
		public CriteriaOperator CriteriaOperator { get { return criteriaOperator; } }
		public Type ObjectType { get { return objectType; } }
		public Type CurrentObjectType {
			get { return currentObject != null ? currentObject.GetType() : objectType; }
		}
		public static object TryGetReadOnlyParameterValue(object value) {
			string parameterName = value as string;
			if(HasParameterPrefix(parameterName)) {
				IParameter parameter = ParametersFactory.CreateParameter(parameterName.Substring(1));
				if(parameter != null) {
					return parameter.CurrentValue;
				}
			}
			return value;
		}
		public void Validate() {
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(CurrentObjectType);
			if(typeInfo == null || hasJoinOperand) return;
			foreach(CriteriaWrapperInvalidProperty property in this.invalidProperties) {
					throw new MemberNotFoundException(property.Type, property.PropertyName);
			}
			foreach(string registerParameter in invalidRegisterParameters) {
				throw new ArgumentException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.NotRegisteredReadOnlyParameterException, registerParameter));
			}
		}
		CriteriaOperator  UpgradeCriteriaOperator(CriteriaOperator criteria) {
			if(ReferenceEquals(criteria, null)) return criteria;
			Type generalType = ObjectType != null ? ObjectType : CurrentObjectType;
			ITypesInfo typesInfo = XafTypesInfo.Instance;
			ClientWrapperCriteriaVisitor processor = new ClientWrapperCriteriaVisitor(typesInfo.FindTypeInfo(generalType), typesInfo.FindTypeInfo(CurrentObjectType));
			CriteriaOperator result = criteria.Accept(processor) as CriteriaOperator;
			parameters.Clear();
			iParameters.Clear();
			invalidProperties.Clear();
			invalidRegisterParameters.Clear();
			invalidProperties.AddRange(processor.InvalidProperties);
			invalidRegisterParameters.AddRange(processor.InvalidRegisterParameters);
			hasJoinOperand = processor.HasJoinOperand;
			foreach(CriteriaWrapperOperandParameter parameter in processor.Parameters) {
				parameters.Add(parameter);
				iParameters.Add(parameter);
			}
			return result;
		}
	}
	internal class CriteriaWrapperOperandParameter : OperandParameter, IParameter {
		IParameter predefinedParameter;
		object currentObject;
		bool isRequired;
		bool isMasked;
		string memberName;
		string caption;
		ITypeInfo typeInfo;
		IMemberInfo memberInfo;
		bool isCustomParameter;
		bool useParameterValue;
		public CriteriaWrapperOperandParameter(IParameter predefinedParameter)
			: base(predefinedParameter.Name) {
			this.predefinedParameter = predefinedParameter;
		}
		public CriteriaWrapperOperandParameter(ITypeInfo typeInfo, string memberName, string name, bool isCustomParameter)
			: base(name) {
			this.memberName = memberName;
			this.typeInfo = typeInfo;
			this.isCustomParameter = isCustomParameter;
			if(!string.IsNullOrEmpty(MemberName) && TypeInfo != null) {
				this.memberInfo = TypeInfo.FindMember(MemberName);
			}
		}
		public string MemberName { get { return memberName; } }
		public ITypeInfo TypeInfo { get { return typeInfo; } }
		public IMemberInfo MemberInfo { get { return memberInfo; } }
		public bool IsObjectMember { get { return !isCustomParameter && MemberInfo != null; } }
		public object CurrentObject { 
			get { return currentObject; } 
			set { currentObject = value; } 
		}
		public override object Value {
			get {
				if(useParameterValue) return base.Value;
				if(PredefinedParameter != null) return PredefinedParameter.CurrentValue;
				if(IsObjectMember) return MemberInfo.GetValue(CurrentObject);
				if(base.Value == null && Type.IsValueType) {
					base.Value = Activator.CreateInstance(Type);
				}
				return base.Value;
			}
			set {
				useParameterValue = true;
				base.Value = value;
			}
		}
		public bool IsReadOnly { get { return PredefinedParameter != null ? PredefinedParameter.IsReadOnly : false; } }
		protected IParameter PredefinedParameter { get { return predefinedParameter; } }
		#region IParameter Members
		public object CurrentValue { get { return Value; } set { Value = value; } }
		public string Caption {  
			get {
				if(!string.IsNullOrEmpty(caption)) return caption;
				if(PredefinedParameter != null) return PredefinedParameter.Caption;
				string memberInfoName = MemberInfo != null ? MemberInfo.Name.Replace('.', '_') : string.Empty;
				if (!IsObjectMember && MemberInfo != null && Name != null && Name.StartsWith(memberInfoName + '_')) return DevExpress.ExpressApp.Utils.CaptionHelper.GetFullMemberCaption(MemberInfo.Owner, MemberInfo.Name);
				return IsObjectMember ? MemberInfo.DisplayName : Name;
			}
			set { caption = value; }
		}
		public string Name { get { return ParameterName; } }
		public bool Visible { get { return MemberInfo != null;  } }
		public Type Type { 
			get { 
				if(PredefinedParameter != null) return predefinedParameter.Type;
				return MemberInfo != null ? MemberInfo.MemberType : typeof(string);
			} 
		}
		public bool IsRequired { get { return isRequired; } set { isRequired = value; } }
		public bool IsMasked { get { return isMasked; } set { isMasked = value; } }
		#endregion
	}
	internal class CriteriaWrapperInvalidProperty {
		ITypeInfo typeInfo;
		string propertyName;
		public CriteriaWrapperInvalidProperty(ITypeInfo typeInfo, string propertyName) {
			this.typeInfo = typeInfo;
			this.propertyName = propertyName;
		}
		public ITypeInfo TypeInfo { get { return typeInfo; } }
		public Type Type { get { return TypeInfo != null ? TypeInfo.Type : null; } }
		public string PropertyName { get { return propertyName; } }
	}
	internal class ClientWrapperCriteriaVisitor : ClientCriteriaVisitorBase {
		const string ParameterPrefix = "@";
		const string ThisParameterPrefix = "@this.";
		const string ReadOnlyParameterPrefix = "@@";
		Dictionary<string, CriteriaWrapperOperandParameter> parameters = new Dictionary<string, CriteriaWrapperOperandParameter>();
		Dictionary<string, CriteriaWrapperInvalidProperty> invalidProperties = new Dictionary<string, CriteriaWrapperInvalidProperty>();
		List<string> invalidRegisterParameters = new List<string>();
		List<string> collectionPropertyList = new List<string>();
		ITypeInfo typeInfo;
		ITypeInfo parametersTypeInfo;
		string currentPropertyName;
		public ClientWrapperCriteriaVisitor(ITypeInfo typeInfo, ITypeInfo parametersTypeInfo) {
			this.typeInfo = typeInfo;
			this.parametersTypeInfo = parametersTypeInfo;
			HasJoinOperand = false;
		}
		public ICollection<CriteriaWrapperOperandParameter> Parameters { get { return parameters.Values; } }
		public ICollection<CriteriaWrapperInvalidProperty> InvalidProperties { get { return invalidProperties.Values; } }
		public List<string> InvalidRegisterParameters { get { return invalidRegisterParameters; } }
		public ITypeInfo TypeInfo { get { return typeInfo; } }
		public ITypeInfo ParametersTypeInfo { get { return parametersTypeInfo; } }
		public bool HasJoinOperand { get; set; }
		protected string CurrentPropertyName { get { return currentPropertyName; } }
		protected IMemberInfo CurrentMemberInfo { 
			get { 
				string dummy = string.Empty;
				ITypeInfo currentTypeInfo = GetCurrentOrCollectionTypeInfoAndPropertyName(ref dummy);
				return currentTypeInfo != null ? currentTypeInfo.FindMember(CurrentPropertyName) : null; 
			} 
		}
		protected string GetParameterName(OperandValue operand, out IMemberInfo memberInfo) {
			memberInfo = null;
			if(ReferenceEquals(operand, null)) return null;
			string operandValue = operand.Value as string;
			if(string.IsNullOrEmpty(operandValue) || operandValue.Contains(":")) return null;
			if(!operandValue.StartsWith(ParameterPrefix)) return null;
			if(operandValue.StartsWith(ReadOnlyParameterPrefix)) {
				string parameterName = operandValue.Remove(0, ReadOnlyParameterPrefix.Length);
				if(ParametersFactory.FindParameter(parameterName) == null) {
					InvalidRegisterParameters.Add(parameterName);
				}
				return parameterName;
			}
			string propertyName = operandValue.Remove(0, ParameterPrefix.Length);
			if(operandValue.ToLower().StartsWith(ThisParameterPrefix)) {
				propertyName = operandValue.Remove(0, ThisParameterPrefix.Length);
				ValidateProperty(propertyName, ParametersTypeInfo);
			}
			memberInfo = ParametersTypeInfo != null ? ParametersTypeInfo.FindMember(propertyName) : null;
			return propertyName;
		}
		const string generatedParameterPrefix = "param";
		int generatedParameterCounter = 0;
		Dictionary<string, int> generatedParameterNamesCounter = new Dictionary<string, int>();
		protected string GeneratedParameterNameForEmptyParameter() {
			if(CurrentMemberInfo == null) {
				generatedParameterCounter++;
				return generatedParameterPrefix + generatedParameterCounter.ToString();
			}
			string result = CurrentMemberInfo.Name.Replace('.', '_') + '_' + CurrentMemberInfo.MemberType.Name.ToLower();
			if(!generatedParameterNamesCounter.ContainsKey(result)) {
				generatedParameterNamesCounter[result] = 0;
			}
			generatedParameterNamesCounter[result]++;
			if(generatedParameterNamesCounter[result] > 1) {
				result += '_' + generatedParameterNamesCounter[result].ToString();
			}
			return result;
		}
		protected override CriteriaOperator Visit(FunctionOperator theOperator) {
			foreach (CriteriaOperator criteria in theOperator.Operands) {
				SetCurrentPropertyName(criteria);
			}
			return base.Visit(theOperator);
		}
		protected override CriteriaOperator Visit(OperandValue operand) {
			IMemberInfo memberInfo;
			string parameterName = GetParameterName(operand, out memberInfo);
			if(!string.IsNullOrEmpty(parameterName)) {
				return VisitParameter(parameterName, memberInfo);
			}
			if(!(operand is OperandParameter) && operand.Value == null) {
				operand = new OperandParameter();
			}
			if((operand is OperandParameter) && !(operand is CriteriaWrapperOperandParameter)) {
				return VisitParameter(((OperandParameter)operand).ParameterName, null);
			}
			if(operand is CriteriaWrapperOperandParameter) {
				return VisitParameter(((OperandParameter)operand).ParameterName, null);
			}
			return VisitOperandValue(operand);
		}
		CriteriaOperator VisitParameter(string parameterName, IMemberInfo memberInfo) {
			if(!string.IsNullOrEmpty(parameterName) && this.parameters.ContainsKey(parameterName)) {
				return this.parameters[parameterName];
			}
			IParameter predefinedParameter = memberInfo == null ? ParametersFactory.FindParameter(parameterName) : null;
			if(predefinedParameter != null) {
				return VisitParameter(new CriteriaWrapperOperandParameter(predefinedParameter));
			}
			string caption = null;
			if(string.IsNullOrEmpty(parameterName)) {
				parameterName = GeneratedParameterNameForEmptyParameter();
				if(CurrentMemberInfo != null) {
					caption = DevExpress.ExpressApp.Utils.CaptionHelper.GetFullMemberCaption(CurrentMemberInfo.Owner, CurrentMemberInfo.Name);
				} else {
					caption = parameterName;
				}
			}
			bool isCustomParameter = ParametersTypeInfo != null && ParametersTypeInfo.FindMember(parameterName) == null;
			String memberName = isCustomParameter ? currentPropertyName : parameterName;
			ITypeInfo currentTypeInfo = ParametersTypeInfo;
			CriteriaWrapperOperandParameter parameter = new CriteriaWrapperOperandParameter(currentTypeInfo, memberName, parameterName, isCustomParameter);
			if(!string.IsNullOrEmpty(caption)) {
				parameter.Caption = caption;
			}
			return VisitParameter(parameter);
		}
		CriteriaOperator VisitOperandValue(OperandValue operand) {
			if(operand is CriteriaWrapperOperandParameter) {
				return VisitParameter(operand as CriteriaWrapperOperandParameter);
			}
			return base.Visit(operand);
		}
		CriteriaOperator VisitParameter(CriteriaWrapperOperandParameter parameter) {
			this.parameters[parameter.ParameterName] = parameter;
			return parameter;
		}
		protected override CriteriaOperator Visit(BinaryOperator operand) {
			if(operand.OperatorType == BinaryOperatorType.Equal && operand.LeftOperand is OperandProperty && operand.RightOperand is OperandValue) {
				IMemberInfo memberInfo;
				string parameterName = GetParameterName(operand.RightOperand as OperandValue, out memberInfo);
				PredefinedParameter parameter = memberInfo == null ? ParametersFactory.FindParameter(parameterName) as PredefinedParameter : null;
				if(parameter != null && parameter.DefaultOperatorType != FunctionOperatorType.None) {
					return Visit(new FunctionOperator(parameter.DefaultOperatorType, operand.LeftOperand));
				}
			}
			SetCurrentPropertyName(operand.LeftOperand);
			SetCurrentPropertyName(operand.RightOperand);
			return base.Visit(operand);
		}
		protected override CriteriaOperator Visit(BetweenOperator theOperator) {
			SetCurrentPropertyName(theOperator.TestExpression);
			return base.Visit(theOperator);
		}
		protected override CriteriaOperator Visit(InOperator theOperator) {
			SetCurrentPropertyName(theOperator.LeftOperand);
			return base.Visit(theOperator);
		}
		bool proccessCollectionProperty = false;
		protected override CriteriaOperator Visit(OperandProperty theOperand) {
			string propertyName = theOperand.PropertyName;
			ITypeInfo typeInfo = GetCurrentOrCollectionTypeInfoAndPropertyName(ref propertyName);
			ValidateProperty(propertyName, typeInfo);
			this.proccessCollectionProperty = false;
			return base.Visit(theOperand);
		}
		protected override CriteriaOperator Visit(JoinOperand theOperand) {
			HasJoinOperand = true;
			return base.Visit(theOperand);
		}
		protected override CriteriaOperator Visit(AggregateOperand theOperand, bool processCollectionProperty) {
			string collectionProperty = theOperand.CollectionProperty.PropertyName;
			if(!string.IsNullOrEmpty(collectionProperty)) {
				this.collectionPropertyList.Add(collectionProperty);
				this.proccessCollectionProperty = true;
			}
			var result = base.Visit(theOperand, processCollectionProperty);
			if(!string.IsNullOrEmpty(collectionProperty)) {
				this.collectionPropertyList.RemoveAt(this.collectionPropertyList.Count - 1);
			}
			return result;
		}
		protected void ValidateProperty(string propertyName, ITypeInfo typeInfo) {
			if(typeInfo == null || string.IsNullOrEmpty(propertyName)) return;
			IMemberInfo memberInfo = typeInfo.FindMember(propertyName);
			if(memberInfo == null || !memberInfo.IsPublic) {
				this.invalidProperties[GetCollectionPropertiesPrefix() + propertyName] = new CriteriaWrapperInvalidProperty(typeInfo, propertyName);
			}
		}
		ITypeInfo GetCurrentOrCollectionTypeInfoAndPropertyName(ref string propertyName) {
			if(TypeInfo == null) return null;
			ITypeInfo propertyTypeInfo = TypeInfo;
			int length = collectionPropertyList.Count;
			if(this.proccessCollectionProperty) length--;
			int depth = 0;
			if (!string.IsNullOrEmpty(propertyName)) {
				while (propertyName.StartsWith("^.") && depth < length) {
					propertyName = propertyName.Remove(0, 2);
					depth++;
				}
			}
			length -= depth;
			for(int i = 0; i < length; i++) {
				IMemberInfo memberInfo = propertyTypeInfo.FindMember(this.collectionPropertyList[i]);
				if(memberInfo == null || !memberInfo.IsList) return null;
				propertyTypeInfo = memberInfo.ListElementTypeInfo;
				if(propertyTypeInfo == null) break;
			}
			return propertyTypeInfo;
		}
		string GetCollectionPropertiesPrefix() {
			string result = string.Empty;
			int length = collectionPropertyList.Count;
			if(this.proccessCollectionProperty) length--;
			for(int i = 0; i < length; i ++) {
				result += collectionPropertyList[i] + '.';
			}
			return result;
		}
		void SetCurrentPropertyName(CriteriaOperator operand) {
			OperandProperty property = operand as OperandProperty;
			if(!ReferenceEquals(property, null)) {
				this.currentPropertyName = property.PropertyName;
			}
		}
	}
	[Serializable]
	public class RequiresCurrentObjectException : Exception {
		protected RequiresCurrentObjectException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public RequiresCurrentObjectException()
			: base(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.RequiresCurrentObjectException)) {
		}
	}
}
