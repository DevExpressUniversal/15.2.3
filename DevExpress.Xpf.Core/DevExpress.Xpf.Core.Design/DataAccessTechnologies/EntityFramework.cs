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

using System.Collections.Generic;
using DevExpress.Xpf.Core.DataSources;
using DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard;
using DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard.Templates;
using DevExpress.Xpf.Core.ServerMode;
using System;
using System.Windows.Data;
using DevExpress.Xpf.Core.Design.Wizards.Utils;
namespace DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies {
	public class EntityFrameworkInfo<TGenerator> : DataAccessTechnologyInfoBase<TGenerator> where TGenerator : DataSourceGenerator, new() {
		public override string Name { get { return "Entity Framework"; } }
		public override string DisplayName { get { return "Entity Framework"; } }
		public override System.IO.Stream GetTechnologyHelp() {
			return WizardResourceHelper.GetHelpStream(WizardResourceHelper.TechnologiesResourceNameRoot, "EntityFramework.rtf");
		}
		protected override IEnumerable<DataSourceType> GetDataSourceTypes(SourceItem item) {
			List<DataSourceType> dataSourceTypes = new List<DataSourceType>();
			if(item == null) {
				dataSourceTypes.Add(new DataSourceType("Simple", null, WizardResourceHelper.GetSimpleTypeHelp()) { DisplayName = "In-Memory Data Processing" });
				dataSourceTypes.Add(new DataSourceType("ICollectionView", null, WizardResourceHelper.GetICollectionViewTypeHelp()) { DisplayName = "Manipulating Data via ICollectionView" });
				if(IsSupportServerMode) {
					dataSourceTypes.Add(new DataSourceType("InstantFeedback", null, WizardResourceHelper.GetInstantFeedBackTypeHelp()) { DisplayName = "Asynchronous Server Side Data Processing" });
					dataSourceTypes.Add(new DataSourceType("ServerMode", null, WizardResourceHelper.GetServerModeTypeHelp()) { DisplayName = "Server Side Data Processing" });
					dataSourceTypes.Add(new DataSourceType("PLinqInstantFeedback", null, WizardResourceHelper.GetPLinqInstantFeedBackTypeHelp()) { DisplayName = "Asynchronous Parallel In-Memory Data Processing" });
					dataSourceTypes.Add(new DataSourceType("PLinqServerMode", null, WizardResourceHelper.GetPLinqServerModeTypeHelp()) { DisplayName = "Parallel In-Memory Data Processing" });
				}
				return dataSourceTypes;
			}
			List<DataTable> tables = AttributeHelper.GetDataTables(item, "EdmScalarPropertyAttribute", "EntityKeyProperty");
			DataSourceGeneratorBase simpleDataSourceGenerator = new TGenerator();
			simpleDataSourceGenerator.Initialize(item, new SimpleDataSourceGenerator(item, typeof(EntitySimpleDataSource)));
			dataSourceTypes.Add(DataSourceGeneratorContainerBuilder.BuildAction("Simple", simpleDataSourceGenerator,
				b => b.BuildSimpleComplete(new SimpleConfigurationViewModel(tables))));
			DataSourceGeneratorBase collectionViewGenerator = new TGenerator();
			collectionViewGenerator.Initialize(item, new CollectionViewDataSourceGenerator(item, typeof(EntityCollectionViewSource)));
			dataSourceTypes.Add(DataSourceGeneratorContainerBuilder.BuildAction("ICollectionView", collectionViewGenerator,
				b => b.BuildCollectionViewComplete(new CollectionViewConfigurationViewModel(tables, new List<Type>() { typeof(CollectionView), typeof(ListCollectionView) }))));
			if(IsSupportServerMode) {
				DataSourceGeneratorBase instantFeedbackDataSourceGenerator = new TGenerator();
				instantFeedbackDataSourceGenerator.Initialize(item, new InstantFeedbackDataSourceGenerator(item, typeof(EntityInstantFeedbackDataSource)));
				dataSourceTypes.Add(DataSourceGeneratorContainerBuilder.BuildAction("InstantFeedback", instantFeedbackDataSourceGenerator,
					b => b.BuildInstantFeedbackComplete(new InstantFeedBackConfigurationViewModel(tables))));
				DataSourceGeneratorBase serverModeDataSourceGenerator = new TGenerator();
				serverModeDataSourceGenerator.Initialize(item, new ServerModeDataSourceGenerator(item, typeof(EntityServerModeDataSource)));
				dataSourceTypes.Add(DataSourceGeneratorContainerBuilder.BuildAction("ServerMode", serverModeDataSourceGenerator,
					b => b.BuildServerModeComplete(new ServerModeConfigurationViewModel(tables))));
				DataSourceGeneratorBase plinqIFGenerator = new TGenerator();
				plinqIFGenerator.Initialize(item, new PLinqDataSourceGenerator(item, typeof(EntityPLinqInstantFeedbackDataSource)));
				dataSourceTypes.Add(DataSourceGeneratorContainerBuilder.BuildAction("PLinqInstantFeedback", plinqIFGenerator,
					b => b.BuildPLinqComplete(new PLinqInstantFeedBackConfigurationViewModel(tables))));
				DataSourceGeneratorBase plinqSMGenerator = new TGenerator();
				plinqSMGenerator.Initialize(item, new PLinqDataSourceGenerator(item, typeof(EntityPLinqServerModeDataSource)));
				dataSourceTypes.Add(DataSourceGeneratorContainerBuilder.BuildAction("PLinqServerMode", plinqSMGenerator,
					b => b.BuildPLinqComplete(new PLinqServerModeConfigurationViewModel(tables))));
			}
			return dataSourceTypes;
		}
		protected override IList<TypeInfoDescriptor> CreateBaseClassesInfo() {
			PropertyInfoDescriptor ef4descriptor = new PropertyInfoDescriptor("System.Data.Objects.ObjectContext", "System.Data.Objects.ObjectSet`1");
			PropertyInfoDescriptor ef5descriptor = new PropertyInfoDescriptor("System.Data.Entity.DbContext", "System.Data.Entity.DbSet`1");
			return new List<TypeInfoDescriptor>() { ef4descriptor, ef5descriptor };
		}
		protected override IVSObjectsCreator CreateVSObjectsCreator() {
			return new AddNewItemCreator("Data\\ADO.NET Entity Data Model", "Model{0}.edmx");
		}
	}
}
