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
using System.Collections.Generic;
namespace DevExpress.Entity.Model.Metadata {
	public abstract class EntitySetInfoBase : IEntitySetInfo {
		readonly IDataColumnAttributesProvider dataColumnAttributesProvider;
		readonly EntityTypeInfoFactory entityTypeInfoFactory;
		IEntitySetAttachedInfo attachedInfo;
		protected IEntityTypeInfo elementTypeInfo;
		protected EntitySetInfoBase(EntityTypeBaseInfo entityType, IEntityContainerInfo entityContainerInfo, IDataColumnAttributesProvider dataColumnAttributesProvider, EntityTypeInfoFactory entityTypeInfoFactory) {
			EntityContainerInfo = entityContainerInfo;
			EntityType = entityType;
			attachedInfo = new EntitySetAttachedInfo(this);
			this.dataColumnAttributesProvider = dataColumnAttributesProvider;
			this.entityTypeInfoFactory = entityTypeInfoFactory;
		}
		protected IDataColumnAttributesProvider DataColumnAttributesProvider { get { return dataColumnAttributesProvider;} }
		public bool IsAbstract {
			get {
				return EntityType.Abstract;
			}
		}
		public IEntityTypeInfo ElementType {
			get {
				if (elementTypeInfo == null)
					elementTypeInfo = this.entityTypeInfoFactory.Create(EntityType, this.EntityContainerInfo as IAssociationTypeSource, Mapper, DataColumnAttributesProvider);
				return elementTypeInfo;
			}
		}
		public IEntityContainerInfo EntityContainerInfo {
			get;
			private set;
		}
		public EntityTypeBaseInfo EntityType {
			get;
			private set;
		}
		public virtual bool IsView {
			get {
				return false;
			}
		}
		public virtual string Name {
			get {
				return EntityType.Name;
			}
		}
		public IEntitySetAttachedInfo AttachedInfo {
			get {
				return attachedInfo;
			}
		}
		public bool ReadOnly {
			get {
				if(ElementType.KeyMembers == null || !ElementType.KeyMembers.Any())
					return false;
				return ElementType.KeyMembers.Count() > 7; 
			}
		}
		protected IMapper Mapper {
			get {
				EntityContainerInfo container = EntityContainerInfo as EntityContainerInfo;
				if (container == null)
					return null;
				return container.Mapper;
			}
		}
	}
	internal class EntitySetInfoProxy : EntitySetInfoBase {
		protected string name;
		IPluralizationService pluralizationService;		
		public EntitySetInfoProxy(EntityTypeBaseInfo entityType, IEntityContainerInfo entityContainerInfo, IDataColumnAttributesProvider dataColumnAttributesProvider, IPluralizationService pluralizationService, EntityTypeInfoFactory entityTypeInfoFactory)
			: base(entityType, entityContainerInfo, dataColumnAttributesProvider, entityTypeInfoFactory) {
				this.pluralizationService = pluralizationService;				
		}
		public override string Name {
			get {
				try {
					if (String.IsNullOrEmpty(name))
						name = pluralizationService.GetPluralizedName(EntityType.Name);
				}
				catch {
					name = EntityType.Name;
				}
				return name;
			}
		}
	}
	internal class EntitySetInfo : EntitySetInfoBase {
		protected EntitySetBaseInfo entitySetBase;
		public EntitySetInfo(EntitySetBaseInfo entitySet, IEntityContainerInfo entityContainerInfo, IDataColumnAttributesProvider dataColumnAttributesProvider, EntityTypeInfoFactory entityTypeInfoFactory) :
			base(entitySet.ElementType, entityContainerInfo, dataColumnAttributesProvider, entityTypeInfoFactory) {
			this.entitySetBase = entitySet;
		}
		public override bool IsView {
			get {
				if(Mapper == null)
					return false;
				return Mapper.HasView(entitySetBase);
			}
		}
		public override string Name {
			get {
				if(entitySetBase == null)
					return null;
				return entitySetBase.Name; }
		}
	}
}
