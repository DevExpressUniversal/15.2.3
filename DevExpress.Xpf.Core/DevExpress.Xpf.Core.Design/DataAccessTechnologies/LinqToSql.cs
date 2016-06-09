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
using System.Windows.Data;
using System;
using DevExpress.Xpf.Core.Design.Wizards.Utils;
namespace DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies {
	public class LinqToSQLDataSourceInfo<TGenerator> : DataAccessTechnologyInfoBase<TGenerator> where TGenerator : DataSourceGenerator, new() {
		public override string Name { get { return "LinqToSQL"; } }
		public override string DisplayName { get { return "Linq To SQL"; } }
		public override System.IO.Stream GetTechnologyHelp() {
			return WizardResourceHelper.GetHelpStream(WizardResourceHelper.TechnologiesResourceNameRoot, "LinqTosql.rtf");
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
			List<DataTable> tables = AttributeHelper.GetDataTables(item, "ColumnAttribute", "IsPrimaryKey");
			DataSourceGeneratorBase simpleDataSourceGenerator = new TGenerator();
			simpleDataSourceGenerator.Initialize(item, new SimpleDataSourceGenerator(item, typeof(LinqSimpleDataSource)));
			dataSourceTypes.Add(DataSourceGeneratorContainerBuilder.BuildAction("Simple", simpleDataSourceGenerator,
				b => b.BuildSimpleComplete(new SimpleConfigurationViewModel(tables))));
			DataSourceGeneratorBase collectionViewGenerator = new TGenerator();
			collectionViewGenerator.Initialize(item, new CollectionViewDataSourceGenerator(item, typeof(LinqCollectionViewDataSource)));
			dataSourceTypes.Add(DataSourceGeneratorContainerBuilder.BuildAction("ICollectionView", collectionViewGenerator,
				b => b.BuildCollectionViewComplete(new CollectionViewConfigurationViewModel(tables, new List<Type>() { typeof(CollectionView), typeof(ListCollectionView) }))));
			if(IsSupportServerMode) {
				DataSourceGeneratorBase instantFeedbackDataSourceGenerator = new TGenerator();
				instantFeedbackDataSourceGenerator.Initialize(item, new InstantFeedbackDataSourceGenerator(item, typeof(LinqInstantFeedbackDataSource)));
				dataSourceTypes.Add(DataSourceGeneratorContainerBuilder.BuildAction("InstantFeedback", instantFeedbackDataSourceGenerator,
					b => b.BuildInstantFeedbackComplete(new InstantFeedBackConfigurationViewModel(tables))));
				DataSourceGeneratorBase serverModeDataSourceGenerator = new TGenerator();
				serverModeDataSourceGenerator.Initialize(item, new ServerModeDataSourceGenerator(item, typeof(LinqServerModeDataSource)));
				dataSourceTypes.Add(DataSourceGeneratorContainerBuilder.BuildAction("ServerMode", serverModeDataSourceGenerator,
					b => b.BuildServerModeComplete(new ServerModeConfigurationViewModel(tables))));
				DataSourceGeneratorBase plinqIFGenerator = new TGenerator();
				plinqIFGenerator.Initialize(item, new PLinqDataSourceGenerator(item, typeof(LinqPlinqInstantFeedbackDataSource)));
				dataSourceTypes.Add(DataSourceGeneratorContainerBuilder.BuildAction("PLinqInstantFeedback", plinqIFGenerator,
					b => b.BuildPLinqComplete(new PLinqInstantFeedBackConfigurationViewModel(tables))));
				DataSourceGeneratorBase plinqSMGenerator = new TGenerator();
				plinqSMGenerator.Initialize(item, new PLinqDataSourceGenerator(item, typeof(LinqPlinqServerModeDataSource)));
				dataSourceTypes.Add(DataSourceGeneratorContainerBuilder.BuildAction("PLinqServerMode", plinqSMGenerator,
					b => b.BuildPLinqComplete(new PLinqServerModeConfigurationViewModel(tables))));
			}
			return dataSourceTypes;
		}
		protected override IList<TypeInfoDescriptor> CreateBaseClassesInfo() {
			return new List<TypeInfoDescriptor>() { new PropertyInfoDescriptor("System.Data.Linq.DataContext", "System.Data.Linq.Table`1") };
		}
		protected override IVSObjectsCreator CreateVSObjectsCreator() {
			return new AddNewItemCreator("Data\\LINQ to SQL Classes", "DataClasses{0}.dbml");
		}
	}
}
