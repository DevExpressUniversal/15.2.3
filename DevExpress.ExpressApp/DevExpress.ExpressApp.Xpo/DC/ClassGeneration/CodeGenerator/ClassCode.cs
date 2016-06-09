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
using System.Text;
using DevExpress.Xpo;
using System.ComponentModel;
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	internal abstract class ClassCode : BaseCode {
		private String baseClassFullName;
		private List<FieldCode> fields;
		private List<ConstructorCode> constructors;
		private List<MethodCode> methods;
		private List<PropertyCodeBase> properties;
		private List<string> implementedInterfaceFullNames;
		protected IMetadataSource metadataSource;
		public ClassCode(string className) : base(className) {
			fields = new List<FieldCode>();
			constructors = new List<ConstructorCode>();
			methods = new List<MethodCode>();
			properties = new List<PropertyCodeBase>();
			implementedInterfaceFullNames = new List<string>();
			Visibility = Visibility.Public;
		}
		bool constructorsCreated = false;
		public void FillClass(ClassMetadata classMetadata, DataMetadata dataMetadata, IMetadataSource metadataSource) {
			this.metadataSource = metadataSource;
			if(!constructorsCreated) {
				FillConstructorsCodeModel(classMetadata, dataMetadata);
				constructorsCreated = true;
			}
			FillClassCore(classMetadata, dataMetadata);
			FillClassFields();
		}
		public virtual String BaseClassFullName {
			get { return baseClassFullName; }
			set { baseClassFullName = value; }
		}
		public FieldCode[] Fields {
			get { return fields.ToArray(); }
		}
		public ConstructorCode[] Constructors {
			get { return constructors.ToArray(); }
		}
		public MethodCode[] Methods {
			get { return methods.ToArray(); }
		}
		public PropertyCodeBase[] Properties {
			get { return properties.ToArray(); }
		}
		public IList<string> ImplementedInterfaceFullNames {
			get {
				return implementedInterfaceFullNames;
			}
		}
		public IMetadataSource MetadataSource {
			get {
				return metadataSource;
			}
		}
		protected override void GetCodeCore(CodeBuilder builder) {
			base.GetCodeCore(builder);
			if(string.IsNullOrEmpty(BaseClassFullName)) {
				builder.Append(string.Format("class {0} {{", Name));
			}
			else {
				string implementedInterfacesString = string.Join(", ", implementedInterfaceFullNames.ToArray());
				if(!string.IsNullOrEmpty(BaseClassFullName) && !string.IsNullOrEmpty(implementedInterfacesString)) {
					implementedInterfacesString = ", " + implementedInterfacesString;
				}
				builder.Append(string.Format("class {0} : {1}{2} {{", Name, BaseClassFullName, implementedInterfacesString));
			}
			builder.AppendNewLine();
			builder.PushIndent();
			foreach(FieldCode fieldCode in Fields) {
				fieldCode.GetCode(builder);
			}
			foreach(ConstructorCode constructorCode in Constructors) {
				constructorCode.GetCode(builder);
			}
			foreach(MethodCode methodCode in Methods) {
				methodCode.GetCode(builder);
			}
			foreach(PropertyCodeBase propertyCode in Properties) {
				propertyCode.GetCode(builder);
			}
			builder.PopIndent();
			builder.AppendLine("}");
		}
		protected virtual void FillClassCore(ClassMetadata classMetadata, DataMetadata dataMetadata) { }
		protected InterfaceMetadata GetInterfaceMetadataByType(Type interfaceType) {
			InterfaceMetadata associatedInterfaceMetadata = MetadataSource.FindInterfaceMetadataByType(interfaceType);
			if(associatedInterfaceMetadata == null) {
				throw new Exception(string.Format("Interface metadata for '{0}' type was not found", interfaceType.FullName));
			}
			return associatedInterfaceMetadata;
		}
		protected virtual void FillClassFieldsCore() { }
		protected void FillClassFields() {
			foreach(PropertyCodeBase propertyCode in Properties) {
				PersistentPropertyWithFieldCodeBase propertyWithFieldCode = propertyCode as PersistentPropertyWithFieldCodeBase;
				if(propertyWithFieldCode != null && propertyWithFieldCode.NeedCreateField &&
				  propertyWithFieldCode.FieldName != "this" &&
				  NeedCreateField(propertyWithFieldCode.FieldName) && !string.IsNullOrEmpty(propertyWithFieldCode.FieldTypeFullName)) {
					FieldCode fieldCode = new FieldCode(propertyWithFieldCode.FieldName, propertyWithFieldCode.FieldTypeFullName);
					AddFieldCode(fieldCode);
					if(propertyCode is AggregatedPropertyCode) {
						fieldCode.AddAttribute(new DevExpress.Xpo.AggregatedAttribute());
						fieldCode.AddAttribute(new DevExpress.Xpo.PersistentAttribute());
					}
					else if(propertyCode is InstancePropertyCode) {
						fieldCode.AddAttribute(new DevExpress.Xpo.AggregatedAttribute());
						fieldCode.AddAttribute(new DevExpress.Xpo.PersistentAttribute(Name + "_" + propertyCode.Name));
						fieldCode.AddAttribute(new DevExpress.Xpo.ExplicitLoadingAttribute(0));
					}
					if(propertyCode is AggregatedDataClassPropertyCode) {
						fieldCode.Visibility = Visibility.Public;
					}
					if(propertyWithFieldCode.Owner is LinkClassCode && !(propertyWithFieldCode is LinkPropertyCode)) {
						fieldCode.AddAttribute(new NonPersistentAttribute());
					}
				}
			}
			FillClassFieldsCore();
		}
		protected bool NeedCreateField(string fieldName) {
			if(string.IsNullOrEmpty(fieldName)) {
				return false;
			}
			foreach(FieldCode fieldCode in Fields) {
				if(fieldCode.Name == fieldName) {
					return false;
				}
			}
			return true;
		}
		protected internal void AddMethodCode(MethodCode methodCode) {
			methods.Add(methodCode);
		}
		protected void AddConstructorCode(ConstructorCode constructorCode) {
			constructors.Add(constructorCode);
		}
		protected void AddFieldCode(FieldCode fieldCode) {
			fields.Add(fieldCode);
		}
		internal virtual void AddPropertyCode(PropertyCodeBase propertyCode) {
			if(propertyCode != null) {
				properties.Add(propertyCode);
			}
		}
		protected virtual ConstructorCode GenerateDefaultConstructor(ClassMetadata classMetadata) {
			ConstructorCode constructorCode = new ConstructorCode(Name);
			ParameterCode sessionParameterCode = new ParameterCode("session", CodeBuilder.TypeToString(typeof(DevExpress.Xpo.Session)));
			constructorCode.AddBaseConstructorArgument("session");
			constructorCode.AddParameter(sessionParameterCode);
			return constructorCode;
		}
		protected void FillConstructorsCodeModel(ClassMetadata classMetadata, DataMetadata dataMetadata) {
			ConstructorCode constructorCode = GenerateDefaultConstructor(classMetadata);
			AddConstructorCode(constructorCode);
			FillExtendedConstructorsCodeModel(classMetadata, dataMetadata);
		}
		protected virtual void FillExtendedConstructorsCodeModel(ClassMetadata classMetadata, DataMetadata dataMetadata) {
		}
	}
}
