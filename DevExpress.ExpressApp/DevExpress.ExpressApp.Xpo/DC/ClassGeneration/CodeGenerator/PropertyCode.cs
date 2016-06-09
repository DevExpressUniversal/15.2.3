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
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	abstract class PropertyCodeBase : MemberCode {
		public PropertyCodeBase(String propertyTypeString, String propertyName)
			: base(propertyName, propertyTypeString) {
			CanRead = true;
		}
		protected override void GetCodeCore(CodeBuilder builder) {
			if(!CanRead && !CanWrite) {
				throw new InvalidOperationException();
			}
			base.GetCodeCore(builder);
			if(String.IsNullOrEmpty(DeclaringInterfaceTypeString)) {
				builder.Append(string.Format("{0} {1} {{", TypeFullName, Name));
			}
			else {
				builder.Append(String.Format("{0} {1}.{2} {{", TypeFullName, DeclaringInterfaceTypeString, Name));
			}
			builder.AppendNewLine();
			builder.PushIndent();
			if(CanRead) {
				if(Virtuality == Virtuality.Abstract){
					builder.AppendLine("get;");
				}
				else{
					builder.AppendLine("get {");
					builder.PushIndent();
					GetGetterCode(builder);
					builder.PopIndent();
					builder.AppendLine("}");
				}
			}
			if(CanWrite) {
				if(Virtuality == Virtuality.Abstract) {
					builder.AppendLine("set;");
				}
				else {
					builder.AppendLine("set {");
					builder.PushIndent();
					GetSetterCode(builder);
					builder.PopIndent();
					builder.AppendLine("}");
				}
			}
			builder.PopIndent();
			builder.AppendLine("}");
		}
		protected virtual void GetGetterCode(CodeBuilder builder) {
			builder.AppendLineFormat("return default({0});", TypeFullName);
		}
		protected virtual void GetSetterCode(CodeBuilder builder) {
		}
		protected String DeclaringInterfaceTypeString { get; set; }
		protected Boolean CanRead { get; set; }
		protected Boolean CanWrite { get; set; }
	}
	internal delegate void GetCode(CodeBuilder builder);
	sealed class CustomPropertyCode : PropertyCodeBase {
		private GetCode getGetter;
		private GetCode getSetter;
		public CustomPropertyCode(String propertyTypeString, String propertyName)
			: base(propertyTypeString, propertyName) {
			CanRead = false;
		}
		public CustomPropertyCode(String propertyTypeString, String declaringInterfaceTypeString, String propertyName)
			: base(propertyTypeString, propertyName) {
			CanRead = false;
			DeclaringInterfaceTypeString = declaringInterfaceTypeString;
		}
		protected override void GetGetterCode(CodeBuilder builder) {
			GetGetter(builder);
		}
		protected override void GetSetterCode(CodeBuilder builder) {
			GetSetter(builder);
		}
		public GetCode GetGetter {
			get { return getGetter; }
			set {
				getGetter = value;
				CanRead = getGetter != null;
			}
		}
		public GetCode GetSetter {
			get { return getSetter; }
			set {
				getSetter = value;
				CanRead = getSetter != null;
			}
		}
	}
	internal interface ISupportFireChanging { }
	internal class PropertyCode : PropertyCodeBase {
		private readonly PropertyMetadata propertyMetadata;
		private readonly ClassCode owner;
		private bool raiseOnChanging;
		public PropertyCode(PropertyMetadata propertyMetadata, ClassCode owner)
			: base(CodeBuilder.TypeToString(propertyMetadata.PropertyType), propertyMetadata.Name) {
			this.propertyMetadata = propertyMetadata;
			this.owner = owner;
			Initialize();
		}
		private void Initialize() {
			Visibility = Visibility.Public;
			CanWrite = !PropertyMetadata.IsReadOnly;
			raiseOnChanging = !(owner is LinkClassCode) && (this is ISupportFireChanging);
			foreach(Attribute attribute in PropertyMetadata.Attributes) {
				if(!(attribute is DefaultValueAttribute)) {
					AddAttribute(attribute);
				}
			}
		}
		public virtual bool IsReadOnly {
			get { return !CanWrite; }
			set { CanWrite = !value; }
		}
		public string InterfaceFullName {
			get { return DeclaringInterfaceTypeString; }
			set {
				DeclaringInterfaceTypeString = value;
				if(!string.IsNullOrEmpty(value)) {
					Visibility = Visibility.None;
				}
			}
		}
		public ClassCode Owner {
			get { return owner; }
		}
		public PropertyMetadata PropertyMetadata {
			get { return propertyMetadata; }
		}
		protected override void GetSetterCode(CodeBuilder builder) {
			if(raiseOnChanging) {
				builder.AppendLineFormat("{0}(\"{1}\", value);", ObjectChangingMethodCode.ObjectChangingMethodName, Name);
			}
		}
		public override void AddAttribute(Attribute attribute) {
			if(attribute is PersistentAttribute) {
				if(CodeModelGeneratorHelper.FindAttribute<PersistentAliasAttribute>(Attributes) == null) {
					base.AddAttribute(attribute);
				}
			}
			else if(attribute is PersistentAliasAttribute) {
				Attribute persistentAttribute = CodeModelGeneratorHelper.FindAttribute<PersistentAttribute>(Attributes);
				if(persistentAttribute != null) {
					RemoveAttribute(persistentAttribute);
				}
				base.AddAttribute(attribute);
			}
			else {
				base.AddAttribute(attribute);
			}
		}
	}
	internal class PersistentPropertyCode : PropertyCode {
		private bool isDelayed;
		public PersistentPropertyCode(PropertyMetadata propertyMetadata, ClassCode owner)
			: base(propertyMetadata, owner) {
			isDelayed = propertyMetadata.FindAttribute<DelayedAttribute>() != null;
		}
		protected override void GetGetterCode(CodeBuilder builder) {
			if(!isDelayed) {
				builder.AppendLineFormat("return GetPropertyValue<{0}>(\"{1}\");", TypeFullName, Name);
			}
			else {
				builder.AppendLineFormat("return GetDelayedPropertyValue<{0}>(\"{1}\");", TypeFullName, Name);
			}
		}
		protected override void GetSetterCode(CodeBuilder builder) {
			base.GetSetterCode(builder);
			if(!isDelayed) {
				builder.AppendLineFormat("SetPropertyValue(\"{0}\", value);", Name);
			}
			else {
				builder.AppendLineFormat("SetDelayedPropertyValue(\"{0}\", value);", Name);
			}
		}
	}
	internal class AggregatedDataClassPropertyCode : AggregatedPropertyCode {
		private string fieldTypeFullName;
		public AggregatedDataClassPropertyCode(PropertyMetadata propertyMetadata, ClassCode owner, DataMetadata dataMetadata)
			: base(propertyMetadata, owner, CodeModelGeneratorHelper.GetDataClassFieldName(dataMetadata)) {
			this.fieldTypeFullName = CodeBuilder.TypeToString(typeof(DevExpress.Xpo.Helpers.IPersistentInterfaceData<>).MakeGenericType(dataMetadata.PrimaryInterface.InterfaceType));
			SetTypeFullName(CodeBuilder.TypeToString(PropertyMetadata.PropertyType));
		}
		protected override void GetGetterCode(CodeBuilder builder) {
			builder.AppendLineFormat("return {0}.Instance.{1};", FieldName, Name);
		}
		protected override void GetSetterCode(CodeBuilder builder) {
			builder.AppendLineFormat("{0}.Instance.{1} = value;", FieldName, Name);
		}
		public override string FieldTypeFullName {
			get {
				return fieldTypeFullName;
			}
		}
	}
	internal class AliaseCollectionPropertyCode : CollectionPropertyCode {
		public AliaseCollectionPropertyCode(PropertyMetadata propertyMetadata, ClassCode owner) :
			base(propertyMetadata, owner) {
			SetName(CodeModelGeneratorHelper.GetAliasPropertyName(PropertyMetadata));
		}
		protected override Type AssociatedType {
			get {
				Type persistentInterfaceDataType = typeof(IPersistentInterfaceData<>).MakeGenericType(base.AssociatedType);
				return persistentInterfaceDataType;
			}
		}
	}
	internal class CollectionPropertyCode : PersistentPropertyWithFieldCodeBase {
		protected string genericArgumentTypeFullName;
		public CollectionPropertyCode(PropertyMetadata propertyMetadata, ClassCode owner) :
			base(propertyMetadata, owner) {
			SetTypeFullName(CodeBuilder.TypeToString(typeof(IList<>).MakeGenericType(AssociatedType)));
			this.genericArgumentTypeFullName = CodeBuilder.TypeToString(AssociatedType);
			CanWrite = false;
		}
		public override bool NeedCreateField {
			get {
				return false;
			}
		}
		protected virtual Type AssociatedType {
			get {
				Type associatedType = CodeModelGeneratorHelper.GetAssociatedType(PropertyMetadata);
				if(Owner is DataClassCode) {
					associatedType = typeof(DevExpress.Xpo.Helpers.IPersistentInterfaceData<>).MakeGenericType(associatedType);
				}
				return associatedType;
			}
		}
		protected override void GetSetterCode(CodeBuilder builder) { }
		protected override void GetGetterCode(CodeBuilder builder) {
			builder.AppendLine(string.Format("return GetList<{0}>(\"{1}\");", GenericArgumentTypeFullName, Name));
		}
		public override bool IsReadOnly {
			set {
				if(!value) {
					throw new InvalidOperationException();
				}
			}
		}
		public string GenericArgumentTypeFullName {
			get {
				return genericArgumentTypeFullName;
			}
		}
	}
	internal class AliasePersistentPropertyCode : PersistentPropertyCode, ISupportFireChanging {
		public AliasePersistentPropertyCode(PropertyMetadata propertyMetadata, ClassCode owner)
			: base(propertyMetadata, owner) {
			SetName(CodeModelGeneratorHelper.GetAliasPropertyName(PropertyMetadata));
			Type associatedType = CodeModelGeneratorHelper.GetAssociatedType(PropertyMetadata);
			Type persistentInterfaceDataType = typeof(IPersistentInterfaceData<>).MakeGenericType(associatedType);
			SetTypeFullName(CodeBuilder.TypeToString(persistentInterfaceDataType));
			AddAttribute(new PersistentAttribute(PropertyMetadata.Name));
		}
	}
	internal class LogicPersistentInterfaceDataPropertyCode : PropertyCode {
		private string persistentInterfaceTypeFullName;
		private string assignedPropertyName;
		public LogicPersistentInterfaceDataPropertyCode(PropertyMetadata propertyMetadata, ClassCode owner)
			: base(propertyMetadata, owner) {
			SetName(CodeModelGeneratorHelper.GetAliasPropertyName(PropertyMetadata));
			SetTypeFullName(CodeBuilder.TypeToString(typeof(IPersistentInterfaceData<>).MakeGenericType(PropertyMetadata.PropertyType)));
			persistentInterfaceTypeFullName = CodeBuilder.TypeToString(typeof(IPersistentInterface<>).MakeGenericType(PropertyMetadata.PropertyType));
			assignedPropertyName = PropertyMetadata.Name;
			IsReadOnly = true;
			AddAttribute(new PersistentAttribute(PropertyMetadata.Name));
		}
		protected override void GetGetterCode(CodeBuilder builder) {
			builder.AppendLineFormat("return {1} == null ? null : (({0}){1}).PersistentInterfaceData;", persistentInterfaceTypeFullName, assignedPropertyName);
		}
	}
	internal class AggregatedPersistentInterfaceDataPropertyCode : AggregatedPropertyCode {
		private string fieldTypeFullName;
		public AggregatedPersistentInterfaceDataPropertyCode(string dataClassFieldName, string typeFullName, string fieldTypeFullName, ClassCode owner)
			: base(new PropertyMetadata(), owner, dataClassFieldName) {
			IsReadOnly = true;
			SetName("PersistentInterfaceData");
			this.fieldTypeFullName = fieldTypeFullName;
			SetTypeFullName(typeFullName);
		}
		protected override void GetGetterCode(CodeBuilder builder) {
			builder.AppendLineFormat("return {0};", FieldName);
		}
		public override string FieldTypeFullName {
			get {
				return fieldTypeFullName;
			}
		}
	}
	internal class AggregatedPersistentInterfacePropertyCode : AggregatedPropertyCode {
		private Type propertyType;
		private string dataPropertyName;
		public AggregatedPersistentInterfacePropertyCode(PropertyMetadata propertyMetadata, ClassCode owner, string dataFieldName) :
			base(propertyMetadata, owner, dataFieldName) {
			SetTypeFullName(CodeBuilder.TypeToString(PropertyMetadata.PropertyType));
			dataPropertyName = (IsAggregated(dataFieldName) && owner is EntityClassCode) ? propertyMetadata.Name :
				CodeModelGeneratorHelper.GetAliasPropertyName(propertyMetadata);
			propertyType = propertyMetadata.PropertyType;
		}
		private bool IsAggregated(string dataFieldName) {
			return dataFieldName != "this";
		}
		protected override void GetGetterCode(CodeBuilder builder) {
			builder.AppendLineFormat("return {0}.{1} == null ? null : {0}.{1}.Instance;", FieldName, dataPropertyName);
		}
		protected override void GetSetterCode(CodeBuilder builder) {
			Type persistentInterfaceType = typeof(IPersistentInterface<>).MakeGenericType(propertyType);
			builder.AppendLineFormat("{0} helper = ({0})value;", CodeBuilder.TypeToString(persistentInterfaceType));
			builder.AppendLine("if (helper == null) {");
			builder.PushIndent();
			builder.AppendLineFormat("{0}.{1} = null;", FieldName, dataPropertyName);
			builder.PopIndent();
			builder.AppendLine("}");
			builder.AppendLine("else {");
			builder.PushIndent();
			builder.AppendLineFormat("{0}.{1} = helper.PersistentInterfaceData;", FieldName, dataPropertyName);
			builder.PopIndent();
			builder.AppendLine("}");
		}
	}
	internal class AggregatedPropertyCode : PersistentPropertyWithFieldCodeBase {
		public AggregatedPropertyCode(PropertyMetadata propertyMetadata, ClassCode owner, string dataClassFieldName) :
			base(propertyMetadata, owner) {
			fieldName = dataClassFieldName;
			AttributesClear();
		}
		protected override void GetGetterCode(CodeBuilder builder) {
			builder.AppendLineFormat("return {0}.{1};", FieldName, Name);
		}
		protected override void GetSetterCode(CodeBuilder builder) {
			builder.AppendLineFormat("{0}.{1} = value;", FieldName, Name);
		}
	}
	internal class LinkPropertyCode : PersistentPropertyWithFieldCodeBase {
		private bool isLeftField = true;
		public LinkPropertyCode(PropertyMetadata propertyMetadata, ClassCode owner, string linkPropertyAssociationName, string entityClassName) :
			base(propertyMetadata, owner) {
			AttributesClear();
			Type associatedInterfaceType = PropertyMetadata.PropertyType.IsGenericType ?
					PropertyMetadata.PropertyType.GetGenericArguments()[0] : PropertyMetadata.PropertyType;
			string linkPropertyInLinkClassName = string.Format("{0}_{1}_Link", associatedInterfaceType.Name, PropertyMetadata.AssociationInfo.AssociatedProperty.Name);
			SetName(linkPropertyInLinkClassName);
			fieldName = CodeModelGeneratorHelper.GetFieldName(linkPropertyInLinkClassName);
			foreach(PropertyCodeBase item in Owner.Properties) {
				if(item is LinkPropertyCode) {
					isLeftField = false;
					break;
				}
			}
			IsReadOnly = false;
			SetTypeFullName(entityClassName);
			AddAttribute(new DevExpress.Xpo.AssociationAttribute(linkPropertyAssociationName));
		}
		protected override void GetSetterCode(CodeBuilder builder) {
			builder.AppendLine("if(value != null) { ");
			builder.PushIndent();
			builder.AppendLineFormat("_{0} = value;", IsLeftField ? DCIntermediateObjectSettings.LeftObjectPropertyName : DCIntermediateObjectSettings.RightObjectPropertyName);
			builder.PopIndent();
			builder.AppendLine("}");
			base.GetSetterCode(builder);
		}
		public bool IsLeftField {
			get {
				return isLeftField;
			}
		}
	}
	internal class LinkCollectionPropertyCode : CollectionPropertyCode {
		private string linkPropertyAssociationName;
		public LinkCollectionPropertyCode(PropertyMetadata propertyMetadata, ClassCode owner, string linkPropertyAssociationName)
			: base(propertyMetadata, owner) {
			SetName(CodeModelGeneratorHelper.GetLinksPropertyName(PropertyMetadata));
			genericArgumentTypeFullName = PropertyMetadata.AssociationInfo.Name;
			SetTypeFullName(string.Format("global::System.Collections.Generic.IList<{0}>", genericArgumentTypeFullName));
			this.linkPropertyAssociationName = linkPropertyAssociationName;
			AddAttribute(new DevExpress.Xpo.AggregatedAttribute());
			AddAttribute(new DevExpress.Xpo.AssociationAttribute(linkPropertyAssociationName));
		}
		public string LinkPropertyAssociationName {
			get {
				return linkPropertyAssociationName;
			}
		}
	}
	internal class PersistentPropertyWithFieldCodeBase : PropertyCode, ISupportFireChanging {
		protected string fieldName;
		private bool isDelayed;
		public PersistentPropertyWithFieldCodeBase(PropertyMetadata propertyMetadata, ClassCode owner)
			: base(propertyMetadata, owner) {
			isDelayed = propertyMetadata.FindAttribute<DelayedAttribute>() != null;
			if(Owner is DataClassCode) {
				if(PropertyMetadata.PropertyType != null &&
					PropertyMetadata.PropertyType.IsGenericType &&
					PropertyMetadata.PropertyType.GetGenericTypeDefinition() == typeof(IList<>)) {
					SetTypeFullName(CodeBuilder.TypeToString(typeof(DevExpress.Xpo.Helpers.IPersistentInterfaceData<>).MakeGenericType(PropertyMetadata.PropertyType)));
				}
			}
			else {
				SetTypeFullName(CodeBuilder.TypeToString(PropertyMetadata.PropertyType));
			}
			fieldName = CodeModelGeneratorHelper.GetFieldName(PropertyMetadata.Name);
		}
		public string FieldName {
			get {
				return fieldName;
			}
		}
		protected override void GetGetterCode(CodeBuilder builder) {
			if(!isDelayed) {
				builder.AppendLine(string.Format("return {0};", FieldName));
			}
			else {
				builder.AppendLineFormat("return GetDelayedPropertyValue<{0}>(\"{1}\");", TypeFullName, Name);
			}
		}
		protected override void GetSetterCode(CodeBuilder builder) {
			base.GetSetterCode(builder);
			if(!isDelayed) {
				builder.AppendLine(string.Format("SetPropertyValue<{0}>(\"{1}\", ref {2}, value);", this.TypeFullName, Name, FieldName));
			}
			else {
				builder.AppendLineFormat("SetDelayedPropertyValue(\"{0}\", value);", Name);
			}
		}
		public virtual string FieldTypeFullName {
			get {
				return TypeFullName;
			}
		}
		public virtual bool NeedCreateField {
			get {
				return !isDelayed;
			}
		}
	}
	internal class PersistentPropertyWithFieldCode : PersistentPropertyWithFieldCodeBase {
		public PersistentPropertyWithFieldCode(PropertyMetadata propertyMetadata, ClassCode owner)
			: base(propertyMetadata, owner) {
			if(Owner is DataClassCode) {
				SetTypeFullName(CodeBuilder.TypeToString(typeof(DevExpress.Xpo.Helpers.IPersistentInterfaceData<>).MakeGenericType(PropertyMetadata.PropertyType)));
			}
			else {
				SetTypeFullName(CodeBuilder.TypeToString(PropertyMetadata.PropertyType));
			}
		}
	}
	internal class InstancePropertyCode : PersistentPropertyWithFieldCodeBase {
		private string fieldTypeFullName;
		public InstancePropertyCode(string typeFullName, string fieldName, ClassCode owner)
			: this(typeFullName, fieldName, typeFullName, owner) {
		}
		public InstancePropertyCode(string typeFullName, string fieldName, string fieldTypeFullName, ClassCode owner)
			: base(new PropertyMetadata(), owner) {
			SetName("Instance");
			this.fieldName = fieldName;
			SetTypeFullName(typeFullName);
			this.fieldTypeFullName = fieldTypeFullName;
			this.IsReadOnly = true;
			AttributesClear();
		}
		public override string FieldTypeFullName {
			get {
				return fieldTypeFullName;
			}
		}
	}
	internal class AggregatedCollectionPropertyCode : CollectionPropertyCode {
		private string persistentGenericArgumentTypeFullName = null;
		private string persistentInterfaceDataPropertyName = null;
		public AggregatedCollectionPropertyCode(PropertyMetadata propertyMetadata, ClassCode owner, string persistentInterfaceDataPropertyName)
			: base(propertyMetadata, owner) {
			this.persistentGenericArgumentTypeFullName =  string.Format("global::DevExpress.Xpo.Helpers.PersistentInterfaceMorpher<{0}>", GenericArgumentTypeFullName);
			this.persistentInterfaceDataPropertyName = persistentInterfaceDataPropertyName;
		}
		protected override Type AssociatedType {
			get {
				return CodeModelGeneratorHelper.GetAssociatedType(PropertyMetadata);
			}
		}
		protected override void GetGetterCode(CodeBuilder builder) {
			builder.AppendLine(string.Format("if ({0} == null)", FieldName));
			builder.PushIndent();
			builder.AppendLine(string.Format("{0} = new {1}({2});", FieldName, persistentGenericArgumentTypeFullName, persistentInterfaceDataPropertyName));
			builder.PopIndent();
			builder.AppendLine(string.Format("return {0};", FieldName));
		}
		public override string FieldTypeFullName {
			get {
				return persistentGenericArgumentTypeFullName;
			}
		}
		public override bool NeedCreateField {
			get {
				return true;
			}
		}
	}
	internal sealed class LogicPropertyCode : PropertyCode {
		private MethodLogic getterLogic;
		private MethodLogic setterLogic;
		public LogicPropertyCode(PropertyMetadata propertyMetadata, ClassCode owner, MethodLogic getterLogic, MethodLogic setterLogic)
			: base(propertyMetadata, owner) {
			this.getterLogic = getterLogic;
			this.setterLogic = setterLogic;
			this.IsReadOnly = propertyMetadata.IsReadOnly;
			IncludeItemAttribute includeItemAttribute = propertyMetadata.FindAttribute<IncludeItemAttribute>();
			if(includeItemAttribute != null) {
				Virtuality = includeItemAttribute.IsOverrideMethod ? Virtuality.Override : Virtuality.New;
			}
		}
		protected override void GetGetterCode(CodeBuilder builder) {
			builder.AppendLineFormat("return {0}();", GetGetterMethodName(PropertyMetadata));
		}
		protected override void GetSetterCode(CodeBuilder builder) {
			builder.AppendLineFormat("{0}(value);", GetSetterMethodName(PropertyMetadata));
			builder.AppendLineFormat("OnChanged(\"{0}\");", PropertyMetadata.Name);
		}
		internal static string GetGetterMethodName(PropertyMetadata propertyMetadata) {
			return "_Get_" + propertyMetadata.Name;
		}
		internal static string GetSetterMethodName(PropertyMetadata propertyMetadata) {
			return "_Set_" + propertyMetadata.Name;
		}
	}
	internal class LogicDataClassPropertyCode : PropertyCode {
		public LogicDataClassPropertyCode(PropertyMetadata propertyMetadata, ClassCode owner)
			: base(propertyMetadata, owner) {
		}
		protected override void GetGetterCode(CodeBuilder builder) {
			builder.AppendLineFormat("return Instance.{0};", Name);
		}
		protected override void GetSetterCode(CodeBuilder builder) {
			builder.AppendLineFormat("Instance.{0} = value;", Name);
		}
	}
	internal class PersistentAliasPropertyCode : PropertyCode {
		public PersistentAliasPropertyCode(PropertyMetadata propertyMetadata, ClassCode owner)
			: base(propertyMetadata, owner) {
		}
		protected override void GetGetterCode(CodeBuilder builder) {
			builder.AppendLineFormat("object obj = EvaluateAlias(\"{0}\");", this.Name);
			builder.AppendLineFormat("return obj == null ? default({0}) : ({0})obj;", TypeFullName);
		}
	}
}
