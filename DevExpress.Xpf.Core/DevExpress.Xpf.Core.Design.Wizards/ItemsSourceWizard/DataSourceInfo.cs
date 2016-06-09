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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard.Templates;
using DevExpress.Xpf.Core.Design.Wizards.Utils;
using Microsoft.Windows.Design.Model;
namespace DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard {
	public abstract class TypeInfoDescriptor {
		private readonly string baseTypeName;
		public string BaseTypeName { get { return this.baseTypeName; } }
		public TypeInfoDescriptor(string baseTypeName) {
			this.baseTypeName = baseTypeName;
		}
		public bool Match(Type type) {
			Type baseType = type.BaseType;
			return baseType != null && baseType.FullName == BaseTypeName && MatchCore(type);
		}
		public abstract IEnumerable<MemberInfo> FindMembers(Type type);
		protected abstract bool MatchCore(Type type);
	}
	public class PropertyInfoDescriptor : TypeInfoDescriptor {
		private readonly string propertyTypeName;
		public string PropertyTypeName { get { return this.propertyTypeName; } }
		public PropertyInfoDescriptor(string baseTypeName, string propertyTypeName)
			: base(baseTypeName) {
			this.propertyTypeName = propertyTypeName;
		}
		public override IEnumerable<MemberInfo> FindMembers(Type type) {
			return type.GetProperties().Where(p => MatchName(p));
		}
		protected override bool MatchCore(Type type) {
			return type.GetProperties().Any(p => MatchName(p));
		}
		private bool MatchName(PropertyInfo p) {
			return p.PropertyType.GetFullName() == PropertyTypeName || p.PropertyType.BaseType.GetFullName() == PropertyTypeName;
		}
	}
	public class MethodInfoDescriptor : TypeInfoDescriptor {
		private readonly string returnTypeName;
		private readonly IList<string> parameterTypes;
		public IList<string> ParameterTypes { get { return this.parameterTypes; } }
		public string ReturnTypeName { get { return this.returnTypeName; } }
		public MethodInfoDescriptor(string baseTypeName, string returnTypeName, IList<string> parameterTypes)
			: base(baseTypeName) {
			this.returnTypeName = returnTypeName;
			this.parameterTypes = parameterTypes;
		}
		public override IEnumerable<MemberInfo> FindMembers(Type type) {
			return type.GetMethods().Where(m => MatchMethod(m));
		}
		protected override bool MatchCore(Type type) {
			return type.GetMethods().Any(m => MatchMethod(m));
		}
		private bool MatchMethod(MethodInfo m) {
			bool matchName = m.ReturnType.GetFullName() == ReturnTypeName;
			IEnumerable<string> parametersType = GetParametersType(m);
			return ParameterTypes == null ? matchName && parametersType.Count() == 0 : matchName && parametersType.SequenceEqual(ParameterTypes);
		}
		private static IEnumerable<string> GetParametersType(MethodInfo methodInfo) {
			return methodInfo.GetParameters().Select(p => { return p.ParameterType.GetFullName(); });
		}
	}
	public class DataSourceGeneratorContainer : DataSourceType {
		private readonly DataSourceGeneratorBase generator;
		public DataSourceGeneratorContainer(string name) : base(name) { }
		public DataSourceGeneratorContainer(string name, IEnumerable<DataSourcePropertyTemplate> properties) : base(name, properties) { }
		public DataSourceGeneratorContainer(string name, IEnumerable<DataSourcePropertyTemplate> properties, Stream help) : base(name, properties, help) { }
		public DataSourceGeneratorContainer(string name, IEnumerable<DataSourcePropertyTemplate> properties, Stream help, DataSourceGeneratorBase generator)
			: base(name, properties, help) {
			this.generator = generator;
		}
		public DataSourceGeneratorBase Generator { get { return this.generator; } }
	}
	public abstract class DataAccessTechnologyInfoBase<TGenerator> : IDataAccessTechnologyInfo where TGenerator : DataSourceGeneratorBase, new() {
		private bool isSupportServerMode;
		private readonly IVSObjectsCreator creator;
		private readonly IList<TypeInfoDescriptor> baseClasses;
		IEnumerable<DataSourceType> dataSourceTypes;
		public IEnumerable<TypeInfoDescriptor> BaseClasses { get { return this.baseClasses; } }
		public bool IsSupportServerMode {
			get { return this.isSupportServerMode; }
			set {
				this.isSupportServerMode = value;
				this.dataSourceTypes = GetDataSourceTypes(null);
			}
		}
		public abstract string Name { get; }
		public abstract string DisplayName { get; }
		protected abstract IEnumerable<DataSourceType> GetDataSourceTypes(SourceItem item);
		public abstract Stream GetTechnologyHelp();
		protected abstract IList<TypeInfoDescriptor> CreateBaseClassesInfo();
		protected virtual IVSObjectsCreator CreateVSObjectsCreator() {
			return new EmptyItemCreator();
		}
		public virtual DataAccessTechnologyViewModel CreateViewModel() {
			return new DataAccessTechnologyViewModel(this);
		}
		public DataAccessTechnologyInfoBase() {
			IsSupportServerMode = true;
			this.baseClasses = CreateBaseClassesInfo();
			this.creator = CreateVSObjectsCreator();
		}
		public IEnumerable<DataSourceType> DataSourceTypes { get { return this.dataSourceTypes; } }
		public IDataAccessTechnologyInfo PopulateDataSourceTypes(SourceItem item = null) {
			this.dataSourceTypes = GetDataSourceTypes(item);
			return this;
		}
		public void CreateItem(EnvDTE.DTE dte) {
			this.creator.CreateDataAccessTechnologyObject(this, dte);
		}
	}
	public static class AttributeHelper {
		public static IEnumerable<PropertyInfo> FindPropertiesWithAttribute(Type type, string attributeTypeName, Predicate<object> condition = null) {
			PropertyInfo[] properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
			if(condition == null)
				condition = d => true;
			return properties.Where(p => p.GetCustomAttributes(false).Any(a => a.GetType().Name == attributeTypeName && condition(a)));
		}
		public static List<DataTable> GetDataTables(SourceItem item, string keyAttributeName = null, string keyPropertyName = null) {
			List<string> tablesName = item.Members.Select(d => d.Name).ToList();
			List<DataTable> tables = new List<DataTable>();
			foreach(string name in tablesName) {
				MemberInfo member = item.Type.GetMember(name)[0];
				Type propertyType = member is PropertyInfo ? ((PropertyInfo)member).PropertyType : ((MethodInfo)member).ReturnType;
				Type tableType = propertyType.IsGenericType ? propertyType.GetGenericArguments()[0] : propertyType.BaseType.GetGenericArguments()[0];
				PropertyInfo[] properties = tableType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
				DataTable dataTable = new DataTable(name, properties.Select(p => p.Name).ToList());
				bool isAutoDetected;
				if(!string.IsNullOrEmpty(keyAttributeName))
					dataTable.KeyExpressions = KeyExpression.Wrap(GetKeyExpressions(tableType, keyAttributeName, keyPropertyName, out isAutoDetected), isAutoDetected);
				tables.Add(dataTable);
			}
			return tables;
		}
		internal static IEnumerable<string> GetKeyExpressions(Type tableType, string keyAttributeName, string keyPropertyName, out bool isAutoDetected) {
			isAutoDetected = true;
			object tableAttr = tableType.GetCustomAttributes(false).FirstOrDefault(p => p.GetType().Name == keyAttributeName);
			if(tableAttr != null)
				return (IEnumerable<string>)tableAttr.GetType().GetProperty(keyPropertyName).GetValue(tableAttr, null);
			IEnumerable<PropertyInfo> properties = AttributeHelper.FindPropertiesWithAttribute(tableType, keyAttributeName, d => IsKey(d, keyPropertyName));
			if(properties.Count() == 0) {
				properties = tableType.GetProperties();
				isAutoDetected = false;
			}
			return properties.Select(p => p.Name);
		}
		private static bool IsKey(object attr, string keyPropertyName) {
			if(string.IsNullOrEmpty(keyPropertyName))
				return true;
			PropertyInfo info = attr.GetType().GetProperty(keyPropertyName, BindingFlags.Public | BindingFlags.Instance);
			return info != null ? (bool)info.GetValue(attr, null) : false;
		}
	}
	public class GeneratorSettings {
		public bool AllowDesignData { get; set; }
		public bool AllowTypedDesignData { get; set; }
		public string BindingPath { get; set; }
		public bool IsStandartSource { get; set; }
		public bool IsSyncLoading { get; set; }
		public GeneratorSettings() {
			AllowDesignData = true;
			BindingPath = "Data";
			IsStandartSource = true;
		}
	}
	public abstract class DataSourceGeneratorBase {
		Type generatedObjectType;
		DataSourceGeneratorBase internalGenerator;
		readonly GeneratorSettings settings;
		SourceItem sourceItem;
		protected DataSourceGeneratorBase InternalGenerator { get { return this.internalGenerator; } }
		protected SourceItem SourceItem { get { return this.sourceItem; } }
		protected DataSourceGeneratorBase() {
			this.settings = new GeneratorSettings();
		}
		public void Initialize(SourceItem sourceItem, DataSourceGeneratorBase generator = null, Type generatedType = null) {
			this.sourceItem = sourceItem;
			this.internalGenerator = generator;
			this.generatedObjectType = generatedType;
		}
		public GeneratorSettings Settings { get { return this.settings; } }
		public Type GeneratedObjectType { get { return this.internalGenerator != null ? this.internalGenerator.GeneratedObjectType : this.generatedObjectType; } }
		protected virtual void Destroy(ModelItem item) { }
		protected virtual ModelItem PreGenerate(object settings, ModelItem item) {
			return item;
		}
		protected virtual void PostGenerate(object settings, ModelItem item, ModelItem generateItem) { }
		public void Generate(object settings, ModelItem item) {
			ModelItem generateItem;
			using(ModelEditingScope batchedChangeRoot = item.BeginEdit()) {
				generateItem = GenerateCore(settings, item);
				batchedChangeRoot.Complete();
			}
			Destroy(generateItem);
		}
		protected virtual ModelItem GenerateCore(object settings, ModelItem item) {
			ModelItem generateItem = PreGenerate(settings, item);
			if(this.internalGenerator != null)
				this.internalGenerator.Generate(settings, generateItem);
			PostGenerate(settings, item, generateItem);
			return generateItem;
		}
	}
}
