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
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using DevExpress.Design.Mvvm;
using DevExpress.Design.Mvvm.EntityFramework;
using System.Data.Metadata.Edm;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Xpf.Core.Native;
using DevExpress.Entity.Model;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework
{
	 public class EntityModelData{
#if DEBUGTEST
		public
#else
		protected
#endif
		EntityModelData(string name) {
			this.Name = name;
			Entities = new List<EntitySetData>();
		}
		public string Name { get; private set; }
		public string FullName { get; private set; }
		public string NamespaceName { get; private set; }
		public List<EntitySetData> Entities { get; private set; }
		public DbContainerType ContainerType { get; private set; }
		public string SourceUrl { get; private set; }
		public bool WithoutDesignTime { get; private set; }
		public static EntityModelData Create(IDbContainerInfo dbContextInfo, IEnumerable<IEntitySetInfo> selectedTables, bool withoutDesignTime)
		{
			if(dbContextInfo == null)
				return null;
			EntityModelData modelData = new EntityModelData(dbContextInfo.Name);
			modelData.WithoutDesignTime = withoutDesignTime;
			modelData.NamespaceName = dbContextInfo.NamespaceName;
			modelData.FullName = dbContextInfo.FullName;
			modelData.ContainerType = dbContextInfo.ContainerType;
			modelData.SourceUrl = dbContextInfo.SourceUrl;
			List<EntitySetData> entities = new List<EntitySetData>();
			foreach(var table in dbContextInfo.EntitySets) {
				if(selectedTables == null || selectedTables.Any(ei => ei == table)) {
					var data = EntitySetData.Create(table, selectedTables);
					if(data != null && !entities.Contains(data)) {
						entities.Add(data);
					}
				}
			}
			modelData.Entities = entities;
			return modelData;
		}		
	}
	public class EntitySetData {
		public string PropertyName { get; private set; }
		public string Name { get; private set; }
		public Type ElementType { get; private set; }
		public string Type { get { return ElementType.Name; } }
		public string TypeFullName { get { return ElementType.FullName; } }
		public string TypeNamespace { get { return ElementType.Namespace; } }
		public bool ReadOnly { get; private set; }
		public EntitySetData[] FKCollections { get; private set; }
		public EntitySetData[] LookUpTables { get; private set; }
		static bool IsNullable(Type type) {
			return Nullable.GetUnderlyingType(type) != null;
		}
		static string GetValueSuffix(IEdmPropertyInfo property) {
			return IsNullable(property.PropertyType) ? ".Value" : "";
		}
		public string GetPrimaryKeyConstructor() {
			if(!HasPrimaryKey)
				return "";
			if (keyProperties.Count() == 1) {
				var prop = keyProperties.First();
				return string.Format("({0} x) => x.{1}{2}", Type, prop.Name, GetValueSuffix(prop));
			} else {
				var exprs = keyProperties.Select(p => string.Format("x.{0}{1}", p.Name, GetValueSuffix(p)));
				return string.Format("({0} x) => Tuple.Create({1})", Type, exprs.Aggregate((l, r) => l + ", " + r));
			}
		}
		public string PrimaryKeyType {
			get {
				if(!HasPrimaryKey)
					return "";
				if(keyProperties.Count() == 1) {
					return GetPrimaryKeyTypes().Single();
				} else {
					return string.Format("Tuple<{0}>", GetPrimaryKeyTypes().Aggregate((l, r) => l + ", " + r));
				}
			}
		}
		bool HasPrimaryKey {
			get {
				return keyProperties != null && keyProperties.Any();
			}
		}
		IEnumerable<string> GetPrimaryKeyTypes() {
			return keyProperties.Select(p => Constants.TypeNameToKeyword(p.GetUnderlyingClrType().Name));
		}
		public IEnumerable<IEdmPropertyInfo> KeyProperties { get { return keyProperties; } }
		IEnumerable<IEdmPropertyInfo> keyProperties;
		public static EntitySetData Create(IEntitySetInfo table, IEnumerable<IEntitySetInfo> selectedTables, bool includeLookUpTables = true, string propertyName = null) {
			if(table == null)
				return null;
			EntitySetData data = new EntitySetData();
			data.ElementType = table.ElementType.Type;
			data.PropertyName = propertyName;
			data.Name = table.Name;
			data.ReadOnly = EntitySetInfoExtensions.ShouldGenerateReadOnlyView(table);
			data.keyProperties = table.ElementType.KeyMembers;
			if(includeLookUpTables) {
				data.FKCollections = GetFkSets(table, selectedTables).Select(x => EntitySetData.Create(x, selectedTables, false)).ToArray();
				List<EntitySetData> lookUpTables = new List<EntitySetData>();
				foreach(IEdmAssociationPropertyInfo info in table.ElementType.LookupTables) {
					IEntitySetInfo lookUpTable = selectedTables.FirstOrDefault(x => x.ElementType.Type == info.ToEndEntityType.Type);
					if(lookUpTable != null)
						lookUpTables.Add(EntitySetData.Create(lookUpTable, selectedTables, false, info.Name));
				}
				data.LookUpTables = lookUpTables.ToArray();
			}
			else {
				data.FKCollections = new EntitySetData[0];
				data.LookUpTables = new EntitySetData[0];
			}
			return data;
		}
		public static IEntitySetInfo[] GetFkSets(IEntitySetInfo table, IEnumerable<IEntitySetInfo> selectedTables) {
			string[] fKTypes = GetFKTypes(table);
			return selectedTables.Where(x => fKTypes.Contains(x.ElementType.Type.FullName) && !x.AttachedInfo.UserSelectedIsReadOnly).ToArray();
		}
		static string[] GetFKTypes(IEntitySetInfo setInfo) {
			IEnumerable<IEdmPropertyInfo> navigationProperties = setInfo.ElementType.GetNavigationProperties();
			return navigationProperties.Select(np => np.GetUnderlyingClrType().FullName).ToArray();
		}
	}	
	public class FKCollectionInfo {
		public string TypeName { get; set; }
		public string NamespaceName { get; set; }
		public string CollectionName { get; set; }
	}
	[XmlRoot("T4TemplateInfo")]
	public class T4TemplateInfo {
		public T4TemplateInfo() {
			Properties = new Dictionary<string, object>();
			UsingList = new List<string>();
		}
		public T4TemplateInfo Clone() {
			T4TemplateInfo result = new T4TemplateInfo();
			result.Properties.AddRange(Properties);
			result.UsingList.AddRange(UsingList);
			return result;
		}
		[XmlElement("Properties")]
		public Dictionary<string, object> Properties { get; set; }
		[XmlElement("UsingList")]
		public List<string> UsingList { get; set; }
		public object GetProperty(string propertyName) {
			if(Properties == null)
				return null;
			object value;
			if(Properties.TryGetValue(propertyName, out value))
				return value;
			return null;
		}
	}
}
