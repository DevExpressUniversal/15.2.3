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
#if !SL
using System.Windows.Data;
using DevExpress.Xpf.Core.ServerMode;
using DevExpress.Xpf.Core.DataSources;
#else
using Platform::System.Windows.Data;
using Platform::DevExpress.Xpf.Core.ServerMode;
using Platform::DevExpress.Xpf.Core.DataSources;
using PLinqInstantFeedbackDataSource = Platform::DevExpress.Xpf.Core.ServerMode.LinqToObjectsInstantFeedbackDataSource;
#endif
namespace DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies {
	public class IEnumerableDataSourceInfo<TGenerator> : DataAccessTechnologyInfoBase<TGenerator>
		where TGenerator : DataSourceGenerator, new() {
#if !SL
		static readonly string pLinqInstantFeedbackName = "PLinqInstantFeedback";
#else
		static readonly string pLinqInstantFeedbackName = "LinqToObjectsInstantFeedback";
#endif
		public override string Name { get { return "IEnumerable"; } }
		public override string DisplayName { get { return "IList/IEnumerable"; } }
		public override System.IO.Stream GetTechnologyHelp() {
			return WizardResourceHelper.GetHelpStream(WizardResourceHelper.TechnologiesResourceNameRoot, "IEnumerable.rtf");
		}
		protected override IList<TypeInfoDescriptor> CreateBaseClassesInfo() {
			return new List<TypeInfoDescriptor>();
		}
		protected override IEnumerable<DataSourceType> GetDataSourceTypes(SourceItem item) {
			List<DataSourceType> dataSourceTypes = new List<DataSourceType>();
			DataSourceGeneratorBase simpleGenerator = new TGenerator();
			simpleGenerator.Settings.AllowTypedDesignData = true;
			simpleGenerator.Settings.IsStandartSource = false;
			simpleGenerator.Initialize(null, new SimpleDataSourceGenerator(null, typeof(IEnumerableDataSource)));
			DataSourceType sourceType = DataSourceGeneratorContainerBuilder.BuildAction("Simple", simpleGenerator, b => {
				ConfigurationViewModelBase viewModel = new ConfigurationViewModelBase(null);
				b.BuildElementType(viewModel);
				b.BuildDesignData(viewModel);
			});
			sourceType.Help = WizardResourceHelper.GetSimpleTypeHelp();
			sourceType.DisplayName = "In-Memory Data Processing";
			dataSourceTypes.Add(sourceType);
			DataSourceGeneratorBase cvGenerator = new TGenerator();
			cvGenerator.Settings.IsStandartSource = false;
			cvGenerator.Settings.AllowTypedDesignData = true;
			cvGenerator.Settings.BindingPath = null;
			cvGenerator.Initialize(null, new CollectionViewDataSourceGenerator(null, typeof(CollectionViewSource)));
			sourceType = DataSourceGeneratorContainerBuilder.BuildAction("CollectionViewSource", cvGenerator, b => {
				CollectionViewConfigurationViewModel viewModel = new CollectionViewConfigurationViewModel(null, DataAccessTechnologyHelper.GetCollectionViewSourceTypes());
				b.BuildElementType(viewModel);
				b.BuildCollectionView(viewModel);
				b.BuildDesignData(viewModel);
			});
			sourceType.Help = WizardResourceHelper.GetCollectionViewSourceTypeHelp();
			sourceType.DisplayName = "Manipulating Data via CollectionViewSource";
			dataSourceTypes.Add(sourceType);
			if(IsSupportServerMode) {
				DataSourceGeneratorBase ifGenerator = new TGenerator();
				ifGenerator.Settings.AllowTypedDesignData = true;
				ifGenerator.Settings.IsStandartSource = false;
				ifGenerator.Initialize(item, new PLinqDataSourceGenerator(item, typeof(PLinqInstantFeedbackDataSource)));
				sourceType = DataSourceGeneratorContainerBuilder.BuildAction(pLinqInstantFeedbackName, ifGenerator, b => {
					PLinqInstantFeedBackConfigurationViewModel viewModel = new PLinqInstantFeedBackConfigurationViewModel(null);
					b.BuildElementType(viewModel);
					b.BuildDefaultSorting(viewModel);
					b.BuildDesignData(viewModel);
				});
				sourceType.Help = WizardResourceHelper.GetPLinqInstantFeedBackTypeHelp();
				sourceType.DisplayName = "Asynchronous Parallel In-Memory Data Processing";
				dataSourceTypes.Add(sourceType);
#if !SL
				DataSourceGeneratorBase smGenerator = new TGenerator();
				smGenerator.Settings.IsStandartSource = false;
				smGenerator.Initialize(null, new PLinqServerModeDataSourceGenerator(null, typeof(PLinqServerModeDataSource)));
				sourceType = DataSourceGeneratorContainerBuilder.BuildAction("PLinqServerMode", smGenerator, b => {
					PLinqServerModeConfigurationViewModel viewModel = new PLinqServerModeConfigurationViewModel(null);
					b.BuildElementType(viewModel);
					b.BuildDefaultSorting(viewModel);
					b.BuildDesignData(viewModel);
				});
				sourceType.Help = WizardResourceHelper.GetPLinqServerModeTypeHelp();
				sourceType.DisplayName = "Parallel In-Memory Data Processing";
				dataSourceTypes.Add(sourceType);
#endif
			}
			return dataSourceTypes;
		}
	}
}
