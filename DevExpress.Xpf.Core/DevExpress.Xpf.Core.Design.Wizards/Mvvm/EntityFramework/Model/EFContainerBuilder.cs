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

using System.Linq;
using DevExpress.Xpf.Core.Mvvm.UI.ViewGenerator.Metadata;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using System.ComponentModel.DataAnnotations;
using DevExpress.Internal;
using System;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Entity.Model;
using DevExpress.Entity.Model.Metadata;
using DevExpress.Entity.Model.DescendantBuilding;
using DevExpress.Entity.ProjectModel;
using DevExpress.Design.Mvvm.Wizards;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework {
	class ScaffoldDbContainerInfo : DbContainerInfo {
		public ScaffoldDbContainerInfo(System.Type type, EntityContainerInfo container, MetadataWorkspaceInfo mw)
			: base(type, container, mw) {
		}
		protected override bool IsValidEntitySet(IEntitySetInfo info) {
			return base.IsValidEntitySet(info) && DataColumnAttributes.GetAttributeValue<ScaffoldTableAttribute, bool>(info.ElementType.Type, x => x.Scaffold, true);
		}
	}
	public class EFContainerBuilder : EFContainerBuilderBase {
		protected override IDataColumnAttributesProvider CreateDataColumnAttributesProvider() {
			return new DataColumnAttributesProvider();
		}
		protected override Mapper GetMapper(TypesCollector typesCollector, MetadataWorkspaceInfo mwInfo) {
			return new PluralizationMapper(mwInfo, typesCollector);
		}
		protected override DescendantBuilderFactoryBase CreateBuilderProviderFactory() {
			return new DescendantBuilderFactory();
		}
		protected override DbContainerInfo CreateDbContainerInfo(IDXTypeInfo type, EntityContainerInfo result, MetadataWorkspaceInfo mw) {
			return new ScaffoldDbContainerInfo(type.ResolveType(), result, mw) { Assembly = type.Assembly };			
		}
		protected override EntityTypeInfoFactory CreateEntityTypeInfoFactory() {
			return new ScaffoldingEntityTypeInfoFactory();
		}
		protected override void LogException(Exception ex, bool display) {
			base.LogException(ex, display);
			Log.SendException(ex, display);
		}
	}
	class ScaffoldingEntityTypeInfoFactory : EntityTypeInfoFactory {
		public override IEntityTypeInfo Create(EntityTypeBaseInfo entityType, IAssociationTypeSource associationTypeSource, IMapper mapper, IDataColumnAttributesProvider attributesProvider = null) {
			EntityTypeBaseInfo mappedEntityType = mapper.GetMappedOSpaceType(entityType);
			return new ScaffoldingEntityTypeInfo(mappedEntityType, mapper.ResolveClrType(mappedEntityType), associationTypeSource, mapper, attributesProvider ?? new EmptyDataColumnAttributesProvider(), this);
		}
	}
	class ScaffoldingEntityTypeInfo : EntityTypeInfo {
		public ScaffoldingEntityTypeInfo(EntityTypeBaseInfo entityType, Type clrType, IAssociationTypeSource associationTypeSource, IMapper mapper, IDataColumnAttributesProvider attributesProvider, EntityTypeInfoFactory entityTypeInfoFactory)
			:base(entityType, clrType, associationTypeSource, mapper, attributesProvider, entityTypeInfoFactory) { 
		}
		protected override bool IsValidLookUpTableProperty(IEdmAssociationPropertyInfo property) {
			return base.IsValidLookUpTableProperty(property) && property.Attributes.ScaffoldDetailCollection();
		}
	}
	class ScaffoldingSqlClientDescendantBuilderProvider : SqlClientDescendantBuilderProvider
	{
		public ScaffoldingSqlClientDescendantBuilderProvider(bool isSqlExpressInstalled, bool isLocalDbInstalled, string localDbVersion)
		:base(isSqlExpressInstalled, isLocalDbInstalled, localDbVersion)
		{
		}
		protected override string ExpectedProviderName
		{
			get
			{
				return null;
			}
		}
	}
	public class DescendantBuilderFactory : DescendantBuilderFactoryBase {
		public static bool DoNotUseLocalDbForTests { get; set; }
		protected override void InitializeProviders() {
			Add(new SqlCeDescendantBuilderProvider(DbEngineDetector.GetSclCEInstalledVersions().Contains("v4.0")));
			bool isSqlExpressInstalled = DbEngineDetector.IsSqlExpressInstalled;
			bool isLocalDbInstalled = DbEngineDetector.IsLocalDbInstalled;
			if(DoNotUseLocalDbForTests)
				isLocalDbInstalled = !isSqlExpressInstalled;
			Add(new ScaffoldingSqlClientDescendantBuilderProvider(isSqlExpressInstalled, isLocalDbInstalled, DbEngineDetector.LocalDbVersion));
		}
	}	
	class PluralizationMapper : Mapper {
		public PluralizationMapper(MetadataWorkspaceInfo mw, TypesCollector typesCollector) : base(mw, typesCollector) { 
		}
		protected override string GetPluralizedNameCore(string name) {
			return PluralizationService.CreateService(CultureInfo.GetCultureInfoByIetfLanguageTag("En")).Pluralize(name);
		}
	}
}
