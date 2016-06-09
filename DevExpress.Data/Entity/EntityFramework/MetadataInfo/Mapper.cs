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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.Entity.Model.Metadata {
	public class MetadataWorkspaceInfo : RuntimeWrapper {
		public MetadataWorkspaceInfo(object value)
			: base(typeof(System.Data.Metadata.Edm.MetadataWorkspace).Name, value) {			
		}
		protected override bool CheckOnlyTypeName {
			get { return true; }
		}
		public IEnumerable<object> GetItems(DataSpace dataSpace) {
			IEnumerable readOnlyCollection = GetMethodAccessor("GetItems").Invoke(() => new object[] { (int)dataSpace }) as IEnumerable;
			if (readOnlyCollection == null)
				return new object[0];
			return readOnlyCollection.Cast<object>();
		}
	}
	public class Mapper : IMapper {
		IEnumerable<EdmTypeInfo> oSpaceEdmTypes;
		IEnumerable<EntitySetBaseInfo> cViews;
		TypesCollector typesCollector;
		public Mapper(MetadataWorkspaceInfo mw, TypesCollector typesCollector) {
			this.typesCollector = typesCollector;
			if(mw != null)
				RegisterMetadataWorkspace(mw);
		}
		System.Data.Metadata.Edm.StoreItemCollection GetSsdlFromDirectory<T>(string directoryPath) {
			if (String.IsNullOrEmpty(directoryPath))
				return null;
			string[] filePaths = Directory.GetFiles(directoryPath, "*" + EdmxResource.SsdlExtension, SearchOption.TopDirectoryOnly);
			return new System.Data.Metadata.Edm.StoreItemCollection(filePaths);
		}
		void RegisterMetadataWorkspace(MetadataWorkspaceInfo mw) {
			RegisterOSpaceItems(mw.GetItems(DataSpace.OSpace));
			RegisterSSpaceItems(mw);
			RegisterCSpaceItems(mw);
		}
		public IEnumerable<EntityTypeBaseInfo> CEntityTypes { get; set; }
		void RegisterCSpaceItems(MetadataWorkspaceInfo mw) {
			CEntityTypes = mw.GetItems(DataSpace.CSpace)
				.Where(t => RuntimeWrapper.ConvertEnum<BuiltInTypeKind>(PropertyAccessor.GetValue(t, "BuiltInTypeKind")) == BuiltInTypeKind.EntityType)
				.Select(x => new EntityTypeBaseInfo(x));
		}
		public IEnumerable<EntityTypeBaseInfo> GetUndeclaredTypesFormHierarchy(IEnumerable<EntityTypeBaseInfo> declaredDbSetsTypes) {
			Dictionary<string, EntityTypeBaseInfo> entityTypes = new Dictionary<string, EntityTypeBaseInfo>();
			Dictionary<string, EntityTypeBaseInfo> sourceEntityTypes = new Dictionary<string, EntityTypeBaseInfo>();
			foreach (EntityTypeBaseInfo type in declaredDbSetsTypes) {
				entityTypes.Add(type.FullName, type);
				sourceEntityTypes.Add(type.FullName, type);
			}
			foreach (EntityTypeBaseInfo et in sourceEntityTypes.Values)
				InitAllDescendants(et, entityTypes);
			return entityTypes.Values.Where(et => et != null && !et.Abstract && !sourceEntityTypes.ContainsKey(et.FullName));
		}
		void InitAllDescendants(EntityTypeBaseInfo type, Dictionary<string, EntityTypeBaseInfo> types) {
			IEnumerable<EntityTypeBaseInfo> ds = GetDescendatns(type);
			if (ds.Any()) {
				foreach (EntityTypeBaseInfo dt in ds) {
					if (types.ContainsKey(dt.FullName))
						continue;
					InitAllDescendants(dt, types);
				}
			}
			else if (!types.ContainsKey(type.FullName)) {
				types.Add(type.FullName, type);
			}
		}
		void RegisterSSpaceItems(MetadataWorkspaceInfo mw) {
			try {
				IEnumerable<object> sSpaceItems = mw.GetItems(DataSpace.SSpace);
				if (sSpaceItems == null || typesCollector == null || typesCollector.MetadataHelper == null)
					return;
				Type metadataHelperType = typesCollector.MetadataHelper.ResolveType();
				if (metadataHelperType == null)
					return;
				object container = sSpaceItems
					.Where(x => RuntimeWrapper.ConvertEnum<BuiltInTypeKind>(PropertyAccessor.GetValue(x, "BuiltInTypeKind")) == BuiltInTypeKind.EntityContainer).First();
				if (container == null)
					return;
				List<object> cViews = new List<object>();
				IEnumerable baseEntitySets = PropertyAccessor.GetValue(container, "BaseEntitySets") as IEnumerable;
				foreach (object esb in baseEntitySets.Cast<object>()) {
					if (RuntimeWrapper.ConvertEnum<BuiltInTypeKind>(PropertyAccessor.GetValue(esb, "BuiltInTypeKind")) != BuiltInTypeKind.EntitySet)
						continue;
					if (!IsView(esb))
						continue;
					IEnumerable cSets = metadataHelperType.GetMethod("GetInfluencingEntitySetsForTable", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { esb, mw.Value }) as IEnumerable;
					if (cSets != null)
						cViews.AddRange(cSets.Cast<object>());
				}
				if (cViews.Count != 0)
					this.cViews = cViews.Select(x => new EntitySetBaseInfo(x));
			}
			catch {
			}
		}
		bool IsView(object set) {
			if (set == null)
				return false;
			IEnumerable metadataProperties = PropertyAccessor.GetValue(set, "MetadataProperties") as IEnumerable;
			if (metadataProperties == null)
				return false;
			return GetPropertyValueByName(metadataProperties.Cast<object>(), Constants.EntityStoreSchemaGeneratorTypeAttributeName) == Constants.EntityStoreSchemaGeneratorTypeAttributeValueIsViews
				|| !string.IsNullOrEmpty(GetPropertyValueByName(metadataProperties.Cast<object>(), "DefiningQuery"));
		}
		static string GetPropertyValueByName(IEnumerable<object> metadataProperties, string propertyName) {
			if (metadataProperties == null)
				return null;
			foreach (object metadataProperty in metadataProperties)
				if (PropertyAccessor.GetValue(metadataProperty, "Name") as string == propertyName)
					return PropertyAccessor.GetValue(metadataProperty, "Value") as string;
			return null;
		}
		void RegisterOSpaceItems(IEnumerable<object> oSpaceItems) {
			if (oSpaceItems == null)
				return;
			oSpaceEdmTypes = oSpaceItems.Select(x => {
				try {
					return new EdmTypeInfo(x);
				}
				catch {
					return null;
				}
			}).Where(x => x != null);
		}
		EntityTypeBaseInfo IMapper.GetMappedOSpaceType(EntityTypeBaseInfo cSpaceType) {
			if (cSpaceType == null || oSpaceEdmTypes == null)
				return null;
			EdmTypeInfo result = oSpaceEdmTypes.Where(oSpaceType => oSpaceType.BuiltInTypeKind == cSpaceType.BuiltInTypeKind && oSpaceType.Name == cSpaceType.Name).FirstOrDefault();
			if (result == null)
				return null;
			return new EntityTypeBaseInfo(result.Value);
		}
		bool IMapper.HasView(EntitySetBaseInfo entitySetBase) {
			if (entitySetBase == null || cViews == null)
				return false;
			foreach (EntitySetBaseInfo set in cViews)
				if (set == entitySetBase)
					return true;
			return false;
		}
		Type IMapper.ResolveClrType(EntityTypeBaseInfo cSpaceType) {
			object value = cSpaceType.Value;
			Type type = value.GetType();
			PropertyInfo pi = type.GetProperty("ClrType", BindingFlags.NonPublic | BindingFlags.Instance);
			return pi.GetValue(value, null) as Type;
		}
		internal IEnumerable<EntityTypeBaseInfo> GetDescendatns(EntityTypeBaseInfo entityType) {
			return CEntityTypes.Where(et => et.BaseTypeFullName == entityType.FullName);
		}
		string IPluralizationService.GetPluralizedName(string name) {
			return GetPluralizedNameCore(name);
		}
		protected virtual string GetPluralizedNameCore(string name) {
			return name;
		}
	}
}
