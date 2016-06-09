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

#if SL
extern alias Platform;
#endif
using System.Collections.Generic;
using DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard;
using DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard.Templates;
using System;
using DevExpress.Xpf.Core.Design.Wizards.Utils;
using DevExpress.Xpf.Core.Design.CoreUtils;
#if !SL
using System.Windows.Data;
using DevExpress.Xpf.Core.DataSources;
using DevExpress.Xpf.Core.ServerMode;
using DevExpress.Xpf.Core.Native;
#else
using Platform::DevExpress.Xpf.Core.DataSources;
using Platform::DevExpress.Xpf.Core.ServerMode;
using Platform::System.Windows.Data;
#endif
namespace DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies {
	public class WCFDataSourceInfo<TGenerator> : DataAccessTechnologyInfoBase<TGenerator> where TGenerator : DataSourceGenerator, new() {
		public override string Name { get { return "WCF"; } }
		public override string DisplayName { get { return "WCF Data Services"; } }
		public override System.IO.Stream GetTechnologyHelp() {
			return WizardResourceHelper.GetHelpStream(WizardResourceHelper.TechnologiesResourceNameRoot, "WCF.rtf");
		}
		protected override IEnumerable<DataSourceType> GetDataSourceTypes(SourceItem item) {
			List<DataSourceType> dataSourceTypes = new List<DataSourceType>();
			if(item == null) {
				dataSourceTypes.Add(new DataSourceType("Simple", null, WizardResourceHelper.GetSimpleTypeHelp()) { DisplayName = "In-Memory Data Processing" });
				dataSourceTypes.Add(new DataSourceType("ICollectionView", null, WizardResourceHelper.GetICollectionViewTypeHelp()) { DisplayName = "Manipulating Data via ICollectionView" });
				if(IsSupportServerMode) {
					dataSourceTypes.Add(new DataSourceType("InstantFeedback", null, WizardResourceHelper.GetInstantFeedBackTypeHelp()) { DisplayName = "Asynchronous Server Side Data Processing" });
#if !SL
					dataSourceTypes.Add(new DataSourceType("ServerMode", null, WizardResourceHelper.GetServerModeTypeHelp()) { DisplayName = "Server Side Data Processing" });
#endif
				}
				return dataSourceTypes;
			}
			List<DataTable> tables = AttributeHelper.GetDataTables(item, "DataServiceKeyAttribute", "KeyNames");
			DataSourceGeneratorBase simpleDataSourceGenerator = new WcfDataSourceGenerator(item, new SimpleDataSourceGenerator(item, typeof(WcfSimpleDataSource)));
			simpleDataSourceGenerator.Settings.IsSyncLoading = true;
			dataSourceTypes.Add(DataSourceGeneratorContainerBuilder.BuildAction("Simple", simpleDataSourceGenerator, (builder) => {
				SimpleConfigurationViewModel viewModel = new SimpleConfigurationViewModel(tables);
				builder.BuildServiceRoot(viewModel);
				builder.BuildSimpleComplete(viewModel);
			}));
			DataSourceGeneratorBase collectionViewGenerator = new WcfDataSourceGenerator(item, new CollectionViewDataSourceGenerator(item, typeof(WcfCollectionViewSource)));
			collectionViewGenerator.Settings.IsSyncLoading = true;
			dataSourceTypes.Add(DataSourceGeneratorContainerBuilder.BuildAction("ICollectionView", collectionViewGenerator, (builder) => {
				CollectionViewConfigurationViewModel viewModel = new CollectionViewConfigurationViewModel(tables, DataAccessTechnologyHelper.GetCollectionViewTypes());
				builder.BuildServiceRoot(viewModel);
				builder.BuildCollectionViewComplete(viewModel);
			}));
			if(IsSupportServerMode) {
				DataSourceGeneratorBase instantFeedbackDataSourceGenerator = new WcfDataSourceGenerator(item, new InstantFeedbackDataSourceGenerator(item, typeof(WcfInstantFeedbackDataSource)));
				dataSourceTypes.Add(DataSourceGeneratorContainerBuilder.BuildAction("InstantFeedback", instantFeedbackDataSourceGenerator, (builder) => {
					InstantFeedBackConfigurationViewModel viewModel = new InstantFeedBackConfigurationViewModel(tables);
					builder.BuildServiceRoot(viewModel);
					builder.BuildInstantFeedbackComplete(viewModel);
				}));
#if !SL
				DataSourceGeneratorBase serverModeDataSourceGenerator = new WcfDataSourceGenerator(item, new ServerModeDataSourceGenerator(item, typeof(WcfServerModeDataSource)));
				dataSourceTypes.Add(DataSourceGeneratorContainerBuilder.BuildAction("ServerMode", serverModeDataSourceGenerator, (builder) => {
					ServerModeConfigurationViewModel viewModel = new ServerModeConfigurationViewModel(tables);
					builder.BuildServiceRoot(viewModel);
					builder.BuildServerModeComplete(viewModel);
				}));
#endif
			}
			return dataSourceTypes;
		}
		protected override IList<TypeInfoDescriptor> CreateBaseClassesInfo() {
			return new List<TypeInfoDescriptor>() { new PropertyInfoDescriptor("System.Data.Services.Client.DataServiceContext", "System.Data.Services.Client.DataServiceQuery`1") };
		}
		protected override IVSObjectsCreator CreateVSObjectsCreator() {
			return new ExecuteCommandCreator("Project.AddServiceReference");
		}
	}
	public static class DataAccessTechnologyHelper {
		public static List<Type> GetCollectionViewTypes() {
#if !SL
			return new List<Type>() { typeof(CollectionView), typeof(ListCollectionView) };
#else
			return new List<Type>() { typeof(CollectionViewSource), typeof(PagedCollectionView) };
#endif
		}
		public static List<Type> GetCollectionViewSourceTypes() {
#if !SL
			return new List<Type>() { typeof(CollectionView), typeof(ListCollectionView), typeof(BindingListCollectionView) };
#else
			return new List<Type>() { typeof(CollectionViewSource) };
#endif
		}
	}
}
