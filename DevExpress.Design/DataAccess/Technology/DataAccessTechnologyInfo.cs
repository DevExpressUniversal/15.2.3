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

namespace DevExpress.Design.DataAccess {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	sealed class DataAccessTechnologyInfoItem : IDataAccessTechnologyInfoItem {
		public DataAccessTechnologyInfoItem(Type type, IEnumerable<MemberInfo> members) {
			this.Type = type;
			this.Name = type.Name;
			this.Members = members;
		}
		public Type Type { get; private set; }
		public string Name { get; private set; }
		public IEnumerable<MemberInfo> Members { get; private set; }
	}
	sealed class TypesProviderDataAccessTechnologyInfoItem : ITypesProviderDataAccessTechnologyInfoItem {
		IEnumerable<Type> typesProvider;
		public TypesProviderDataAccessTechnologyInfoItem(IEnumerable<Type> typesProvider) {
			this.typesProvider = System.Linq.Enumerable.Where(typesProvider, DataTypeInfoHelper.ValidDataType);
		}
		public Type Type { get { return null; } }
		public string Name { get { return null; } }
		public IEnumerable<MemberInfo> Members {
			get { yield break; }
		}
		public IEnumerator<Type> GetEnumerator() {
			return typesProvider.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return typesProvider.GetEnumerator();
		}
	}
	sealed class EnumTypesProviderDataAccessTechnologyInfoItem : ITypesProviderDataAccessTechnologyInfoItem {
		IEnumerable<Type> typesProvider;
		public EnumTypesProviderDataAccessTechnologyInfoItem(IEnumerable<Type> typesProvider) {
			IEnumTypesProviderInfo info = (typesProvider as IEnumTypesProviderInfo) ??
				new DefaultIEnumTypesProviderInfo();
			this.typesProvider = System.Linq.Enumerable.Concat(
					System.Linq.Enumerable.Where(typesProvider, EnumTypeInfoHelper.ValidDataType),
					GetStandardEnums(info));
		}
		public Type Type { get { return null; } }
		public string Name { get { return null; } }
		public IEnumerable<MemberInfo> Members {
			get { yield break; }
		}
		static IEnumerable<Type> GetStandardEnums(IEnumTypesProviderInfo info) {
			List<Type> enumTypes = new List<Type>();
			for(int i = 0; i < info.RootTypes.Length; i++) {
				var rootType = info.RootTypes[i];
				var types = Assembly.GetAssembly(rootType).GetTypes();
				enumTypes.AddRange(Array.FindAll(types,
					(t) => EnumTypeInfoHelper.ValidDataType(t) && t.Namespace == rootType.Namespace));
			}
			return enumTypes.FindAll((t) => !Array.Exists(info.SkipList, (name) => t.Name.StartsWith(name)));
		}
		public IEnumerator<Type> GetEnumerator() {
			return typesProvider.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return typesProvider.GetEnumerator();
		}
		#region Default EnumTypesProviderInfo
		class DefaultIEnumTypesProviderInfo : IEnumTypesProviderInfo {
			Type[] IEnumTypesProviderInfo.RootTypes {
				get { return new Type[] { typeof(object) }; }
			}
			string[] IEnumTypesProviderInfo.SkipList {
				get { return new string[] { }; }
			}
		}
		#endregion
	}
	sealed class ComponentDataAccessTechnologyInfoItem : IComponentDataAccessTechnologyInfoItem {
		public ComponentDataAccessTechnologyInfoItem(object component, Type type) {
			this.Type = type;
			var c = component as System.ComponentModel.IComponent;
			this.Name = (c != null && c.Site != null) ? c.Site.Name : type.Name;
			this.Component = component;
		}
		public Type Type { get; private set; }
		public string Name { get; private set; }
		public object Component { get; private set; }
		public IEnumerable<MemberInfo> Members { get { yield break; } }
	}
	sealed class TypeSetDataAccessTechnologyInfoItem : ITypeSetDataAccessTechnologyInfoItem {
		List<Type> types;
		TypeSetDataAccessTechnologyInfoItem(string namespaceStr) {
			types = new List<Type>();
			Namespace = namespaceStr;
			Name = GetDataModelName(namespaceStr);
		}
		public Type Type { get { return null; } }
		public string Name { get; private set; }
		public string Namespace { get; private set; }
		public IEnumerable<MemberInfo> Members {
			get { yield break; }
		}
		public IEnumerator<Type> GetEnumerator() {
			return types.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return types.GetEnumerator();
		}
		public static IEnumerable<ITypeSetDataAccessTechnologyInfoItem> Classify(IEnumerable<Type> types) {
			var classifier = new Dictionary<string, TypeSetDataAccessTechnologyInfoItem>();
			foreach(Type type in types) {
				TypeSetDataAccessTechnologyInfoItem item;
				if(!classifier.TryGetValue(type.Namespace, out item)) {
					item = new TypeSetDataAccessTechnologyInfoItem(type.Namespace);
					classifier.Add(type.Namespace, item);
					item.types.Add(type);
					yield return item;
				}
				else item.types.Add(type);
			}
		}
		static string GetDataModelName(string namespaceStr) {
			if(namespaceStr.EndsWith(".DataModel"))
				namespaceStr = namespaceStr.Substring(0, namespaceStr.IndexOf(".DataModel"));
			string modelName = namespaceStr.Substring(namespaceStr.LastIndexOf('.') + 1);
			return string.Format("Persistent Data Model ({0})", modelName);
		}
	}
	sealed class DataAccessTechnologyInfo : IDataAccessTechnologyInfo {
		IEnumerable<Type> typesProvider;
		Design.UI.IServiceContainer serviceContainer;
		public DataAccessTechnologyInfo(Design.UI.IServiceContainer serviceContainer, DataAccessTechnologyCodeName codeName, IEnumerable<Type> typesProvider) {
			this.serviceContainer = serviceContainer;
			this.typesProvider = typesProvider;
			this.Name = new DataAccessTechnologyName(codeName);
		}
		public IDataAccessTechnologyName Name { get; private set; }
		IDataAccessTechnologyConstraint constraintCore;
		public IDataAccessTechnologyConstraint Constraint {
			get {
				if(constraintCore == null)
					constraintCore = CodeNamesResolver.GetConstraint(Name.GetCodeName());
				return constraintCore;
			}
		}
		IEnumerable<IDataAccessTechnologyInfoItem> itemsCore;
		IEnumerable<IDataAccessTechnologyInfoItem> IDataAccessTechnologyInfo.Items {
			get {
				if(itemsCore == null) {
					if(!ComponentsAsItems && !TypeSetAsItems) {
						itemsCore = Metadata.AvailableTypes.Unique(GetItemsCore(), (item) => item.Type);
						itemsCore = System.Linq.Enumerable.ToArray(itemsCore);
					}
					else itemsCore = System.Linq.Enumerable.ToArray(GetItemsCore());
				}
				return itemsCore;
			}
		}
		public bool CanCreateItems {
			get { return CodeNamesResolver.CanCreateItems(Name.GetCodeName()); }
		}
		public bool ComponentsAsItems {
			get { return CodeNamesResolver.IsBasedOnComponents(Name.GetCodeName()); }
		}
		public bool TypeSetAsItems {
			get { return CodeNamesResolver.IsBasedOnTypeSet(Name.GetCodeName()); }
		}
		public bool HasItems {
			get { return CanCreateItems && (itemsCore != null) && ((IDataAccessTechnologyInfoItem[])itemsCore).Length > 0; }
		}
		IEnumerable<IDataAccessTechnologyInfoItem> GetItemsCore() {
			if(!CanCreateItems) {
				switch(Name.GetCodeName()) {
					case DataAccessTechnologyCodeName.IEnumerable:
						yield return new TypesProviderDataAccessTechnologyInfoItem(typesProvider);
						break;
					case DataAccessTechnologyCodeName.Enum:
						yield return new EnumTypesProviderDataAccessTechnologyInfoItem(typesProvider);
						break;
					default:
						yield break;
				}
			}
			if(ComponentsAsItems) {
				var componentsProvider = (serviceContainer != null) ? serviceContainer.Resolve<IDataAccessTechnologyComponentsProvider>() : null;
				if(componentsProvider != null) {
					foreach(System.ComponentModel.IComponent component in componentsProvider) {
						Type componentType = component.GetType();
						IEnumerable<MemberInfo> members;
						if(Constraint.TryGetMembers(componentType, out members))
							yield return new ComponentDataAccessTechnologyInfoItem(component, componentType);
					}
				}
			}
			else {
				if(typesProvider != null) {
					if(TypeSetAsItems) {
						foreach(ITypeSetDataAccessTechnologyInfoItem item in TypeSetDataAccessTechnologyInfoItem.Classify(GetTypeSetTypes()))
							yield return item;
					}
					else {
						foreach(Type type in typesProvider) {
							IEnumerable<MemberInfo> members;
							if(Constraint.TryGetMembers(type, out members))
								yield return new DataAccessTechnologyInfoItem(type, members);
						}
					}
				}
			}
		}
		IEnumerable<Type> GetTypeSetTypes() {
			foreach(Type type in typesProvider) {
				IEnumerable<MemberInfo> members;
				if(Constraint.TryGetMembers(type, out members))
					yield return type;
			}
		}
		#region CodeNamesResolver
		static class CodeNamesResolver {
			static IDictionary<DataAccessTechnologyCodeName, IDataAccessTechnologyConstraint> constraints;
			static HashSet<DataAccessTechnologyCodeName> technologiesWithoutItems;
			static HashSet<DataAccessTechnologyCodeName> componentsBasedTechnologies;
			static HashSet<DataAccessTechnologyCodeName> typeSetBasedTechnologies;
			static CodeNamesResolver() {
				constraints = new Dictionary<DataAccessTechnologyCodeName, IDataAccessTechnologyConstraint>();
				constraints.Add(DataAccessTechnologyCodeName.TypedDataSet, new TypedDataSetTechnologyConstraint());
				constraints.Add(DataAccessTechnologyCodeName.LinqToSql, new LinqToSQLTechnologyConstraint());
				constraints.Add(DataAccessTechnologyCodeName.EntityFramework, new EntityFrameworkTechnologyConstraint());
				constraints.Add(DataAccessTechnologyCodeName.XPO, new XPOTechnologyConstraint());
				constraints.Add(DataAccessTechnologyCodeName.Wcf, new WcfTechnologyConstraint());
				constraints.Add(DataAccessTechnologyCodeName.Ria, new RiaTechnologyConstraint());
				constraints.Add(DataAccessTechnologyCodeName.IEnumerable, new IEnumerableTechnologyConstraint());
				constraints.Add(DataAccessTechnologyCodeName.Enum, new EnumTechnologyConstraint());
				constraints.Add(DataAccessTechnologyCodeName.SQLDataSource, new SQLDataSourceTechnologyConstraint());
				constraints.Add(DataAccessTechnologyCodeName.ExcelDataSource, new ExcelDataSourceTechnologyConstraint());
				technologiesWithoutItems = new HashSet<DataAccessTechnologyCodeName>();
				technologiesWithoutItems.Add(DataAccessTechnologyCodeName.XmlDataSet);
				technologiesWithoutItems.Add(DataAccessTechnologyCodeName.IEnumerable);
				technologiesWithoutItems.Add(DataAccessTechnologyCodeName.Enum);
				technologiesWithoutItems.Add(DataAccessTechnologyCodeName.OLAP);
				componentsBasedTechnologies = new HashSet<DataAccessTechnologyCodeName>();
				componentsBasedTechnologies.Add(DataAccessTechnologyCodeName.SQLDataSource);
				componentsBasedTechnologies.Add(DataAccessTechnologyCodeName.ExcelDataSource);
				typeSetBasedTechnologies = new HashSet<DataAccessTechnologyCodeName>();
				typeSetBasedTechnologies.Add(DataAccessTechnologyCodeName.XPO);
			}
			internal static IDataAccessTechnologyConstraint GetConstraint(DataAccessTechnologyCodeName codeName) {
				return constraints[codeName];
			}
			internal static bool CanCreateItems(DataAccessTechnologyCodeName codeName) {
				return !technologiesWithoutItems.Contains(codeName);
			}
			internal static bool IsBasedOnComponents(DataAccessTechnologyCodeName codeName) {
				return componentsBasedTechnologies.Contains(codeName);
			}
			internal static bool IsBasedOnTypeSet(DataAccessTechnologyCodeName codeName) {
				return typeSetBasedTechnologies.Contains(codeName);
			}
		}
		#endregion CodeNamesResolver
	}
}
