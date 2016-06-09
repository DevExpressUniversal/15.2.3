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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.Xpf.Core.DataSources;
using DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard;
using DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard.Templates;
using System.Windows.Data;
using DevExpress.Xpf.Core.Design.Wizards.Utils;
namespace DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies {
	public class TypedDataSourceInfo<TGenerator> : DataAccessTechnologyInfoBase<TGenerator> where TGenerator : DataSourceGenerator, new() {
		public override string Name { get { return "Typed DataSet"; } }
		public override string DisplayName { get { return "ADO.NET Data Set"; } }
		public override System.IO.Stream GetTechnologyHelp() {
			return WizardResourceHelper.GetHelpStream(WizardResourceHelper.TechnologiesResourceNameRoot, "DataSet.rtf");
		}
		protected override IEnumerable<DataSourceType> GetDataSourceTypes(SourceItem item) {
			List<DataSourceType> dataSourceTypes = new List<DataSourceType>();
			if(item == null) {
				dataSourceTypes.Add(new DataSourceType("Simple", null, WizardResourceHelper.GetSimpleTypeHelp()) { DisplayName = "In-Memory Data Processing" });
				dataSourceTypes.Add(new DataSourceType("ICollectionView", null, WizardResourceHelper.GetICollectionViewTypeHelp()) { DisplayName = "Manipulating Data via ICollectionView" });
				return dataSourceTypes;
			}
			List<DataTable> tables = DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard.AttributeHelper.GetDataTables(item);
			DataSourceGeneratorBase collectionViewGenerator = new TGenerator();
			collectionViewGenerator.Initialize(item, new CollectionViewDataSourceGenerator(item, typeof(TypedCollectionViewSource)));
			DataSourceGeneratorBase simpleDataSourceGenerator = new TGenerator();
			simpleDataSourceGenerator.Initialize(item, new SimpleDataSourceGenerator(item, typeof(TypedSimpleSource)));
			dataSourceTypes.Add(DataSourceGeneratorContainerBuilder.BuildAction("ICollectionView", collectionViewGenerator,
				b => b.BuildCollectionViewComplete(new CollectionViewConfigurationViewModel(tables, new List<Type>() { typeof(ListCollectionView), typeof(BindingListCollectionView) }))));
			dataSourceTypes.Add(DataSourceGeneratorContainerBuilder.BuildAction("Simple", simpleDataSourceGenerator,
				b => b.BuildSimpleComplete(new SimpleConfigurationViewModel(tables))));
			return dataSourceTypes;
		}
		protected override IList<TypeInfoDescriptor> CreateBaseClassesInfo() {
			return new List<TypeInfoDescriptor>() { new PropertyInfoDescriptor("System.Data.DataSet", "System.Data.TypedTableBase`1") };
		}
		protected override IVSObjectsCreator CreateVSObjectsCreator() {
			string commandStr = DevExpress.Utils.Design.DTEHelper.IsVS2012 ? "Project.AddNewDataSource" : "Data.AddNewDataSource";
			return new ExecuteCommandCreator(commandStr);
		}
	}
}
