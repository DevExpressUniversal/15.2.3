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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Entity.Model.Metadata {
	public class EntityFunctionInfo : IEntityFunctionInfo {
		EdmFunctionInfo functionInfo;
		public string Name {
			get;
			private set;
		}
		public FunctionParameterInfo[] Parameters {
			get;
			private set;
		}
		public EdmComplexTypePropertyInfo[] ResultTypeProperties {
			get;
			private set;
		}
		public EntityFunctionInfo(EdmFunctionInfo functionInfo) {
			this.functionInfo = functionInfo;
			Name = functionInfo.Name;
			Parameters = functionInfo.Parameters;
			ResultTypeProperties = functionInfo.ResultTypeProperties;
		}
	}
	public class EntityContainerInfo : RuntimeWrapper, IEntityContainerInfo, IAssociationTypeSource {		
		IDataColumnAttributesProvider dataColumnAttributesProvider;
		List<IEntitySetInfo> entitySets;
		List<IEntityFunctionInfo> entityFunctions;
		List<AssociationTypeInfo> associationTypesFromCSpace = new List<AssociationTypeInfo>();
		IMapper mapper;
		EntityTypeInfoFactory entityTypeInfoFactory;
		public EntityContainerInfo(object entityContainer, IMapper mapper, IDataColumnAttributesProvider dataColumnAttributesProvider, EntityTypeInfoFactory entityTypeInfoFactory)
			: base(typeof(System.Data.Metadata.Edm.EntityContainer).FullName, entityContainer) {
			this.mapper = mapper;
			this.dataColumnAttributesProvider = dataColumnAttributesProvider;
			this.entityTypeInfoFactory = entityTypeInfoFactory;
		}
		public string Name {
			get { return GetPropertyAccessor("Name").Value as string; }
		}
		public IMapper Mapper {
			get {
				return mapper;
			}
		}
		AssociationTypeInfo IAssociationTypeSource.GetAssociationTypeFromCSpace(string fullName) {
			if(String.IsNullOrEmpty(fullName))
				return null;
			foreach (AssociationTypeInfo associationType in associationTypesFromCSpace)
				if (associationType.FullName == fullName)
					return associationType;
			return null;
		}
		void InitEntitySets() {
			IEnumerable baseEntitySets = GetPropertyAccessor("BaseEntitySets").Value as IEnumerable;
			if(baseEntitySets == null)
				return;
			if(!baseEntitySets.Cast<object>().Any())
				return;
			IEnumerable<EntitySetBaseInfo> baseEntitySetInfos = baseEntitySets.Cast<object>().Select(x => new EntitySetBaseInfo(x));
			List<EntityTypeBaseInfo> declaredEntityTypes = new List<EntityTypeBaseInfo>();
			foreach(EntitySetBaseInfo baseEntitySet in baseEntitySetInfos) {
				EntityTypeBaseInfo type = baseEntitySet.ElementType;
				if(type == null || !type.Abstract)
					AddEntitySet(baseEntitySet);
				if(type != null)
					declaredEntityTypes.Add(type);
			}
			if(Mapper is Mapper) {
				IEnumerable<EntityTypeBaseInfo> undeclaredSets = (Mapper as Mapper).GetUndeclaredTypesFormHierarchy(declaredEntityTypes);
				foreach(EntityTypeBaseInfo type in undeclaredSets) {
					if(entitySets == null)
						entitySets = new List<IEntitySetInfo>();
					entitySets.Add(new EntitySetInfoProxy(type, this, dataColumnAttributesProvider, Mapper, entityTypeInfoFactory));
				}
			}
			entitySets = entitySets.OrderBy(s => s.Name).ToList();
		}
		void InitEntityFunctions() {
			entityFunctions = new List<IEntityFunctionInfo>();
			IEnumerable baseEdmFunctions = GetPropertyAccessor("FunctionImports").Value as IEnumerable;
			if(baseEdmFunctions == null)
				return;
			if(!baseEdmFunctions.Cast<object>().Any())
				return;
			IEnumerable<EdmFunctionInfo> baseEdmFunctionInfos = baseEdmFunctions.Cast<object>().Select(x => new EdmFunctionInfo(x));
			foreach(EdmFunctionInfo edmFunctionInfo in baseEdmFunctionInfos) {
				entityFunctions.Add(new EntityFunctionInfo(edmFunctionInfo));
			}
		}
		void AddEntitySet(EntitySetBaseInfo entitySet) {
			if(entitySet == null)
				return;
			switch(entitySet.BuiltInTypeKind) {
				case BuiltInTypeKind.EntitySet:
					if (entitySets == null)
						entitySets = new List<IEntitySetInfo>();
					entitySets.Add(new EntitySetInfo(entitySet, this, dataColumnAttributesProvider, entityTypeInfoFactory));
					return;
				case BuiltInTypeKind.AssociationSet:
					associationTypesFromCSpace.Add(new AssociationTypeInfo(entitySet.ElementType.Value)); 
					return;
			}
		}		
		public ICollection<IEntitySetInfo> BaseEntitySets {
			get {
				return null;
			}
		}
		public IEnumerable<IEntitySetInfo> EntitySets {
			get {
				if(entitySets != null)
					return entitySets;
				InitEntitySets();
				return entitySets;				
			}
		}
		public IEnumerable<IEntityFunctionInfo> EntityFunctions {
			get {
				if(entityFunctions != null)
					return entityFunctions;
				InitEntityFunctions();
				return entityFunctions;
			}
		}
	}
}
