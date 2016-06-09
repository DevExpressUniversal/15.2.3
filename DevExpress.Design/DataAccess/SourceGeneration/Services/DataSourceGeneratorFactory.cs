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

namespace DevExpress.Design.DataAccess.Win {
	using System.Collections.Generic;
	sealed class DataSourceGeneratorFactory : IDataSourceGeneratorFactory {
		public IDataSourceGenerator GetGenerator(IDataAccessTechnologyName technologyName, IDataSourceSettingsModel settingsModel) {
			return SettingsModelResolver.GetInitializer(settingsModel)(technologyName.GetCodeName());
		}
		static class SettingsModelResolver {
			public delegate IDataSourceGenerator Initializer(DataAccessTechnologyCodeName codeName);
			static IDictionary<System.Type, Initializer> resolvers;
			static SettingsModelResolver() {
				resolvers = new Dictionary<System.Type, Initializer>();
				resolvers.Add(typeof(IDirectBindingSettingsModel), (codeName) => DirectBinding.GetInitializer(codeName)());
				resolvers.Add(typeof(ITypedListSourceSettingsModel), (codeName) => TypedList.GetInitializer(codeName)());
				resolvers.Add(typeof(IBindingListViewSourceSettingsModel), (codeName) => BindingListView.GetInitializer(codeName)());
				resolvers.Add(typeof(IXmlDataSetSettingsModel), (codeName) => XmlDataSet.GetInitializer(codeName)());
				resolvers.Add(typeof(IServerModeSettingsModel), (codeName) => ServerMode.GetInitializer(codeName)());
				resolvers.Add(typeof(IInstantFeedbackSettingsModel), (codeName) => InstantFeedback.GetInitializer(codeName)());
				resolvers.Add(typeof(IPLinqServerModeSettingsModel), (codeName) => PLinqServerMode.GetInitializer(codeName)());
				resolvers.Add(typeof(IPLinqInstantFeedbackSettingsModel), (codeName) => PLinqInstantFeedback.GetInitializer(codeName)());
				resolvers.Add(typeof(IOLAPDataSourceSettingsModel), (codeName) => OLAP.GetInitializer(codeName)());
				resolvers.Add(typeof(IXPCollectionSourceSettingsModel), (codeName) => XPCollectionSource.GetInitializer(codeName)());
				resolvers.Add(typeof(IXPViewSourceSettingsModel), (codeName) => XPViewSource.GetInitializer(codeName)());
				resolvers.Add(typeof(IXPServerCollectionSourceSettingsModel), (codeName) => ServerMode.GetInitializer(codeName)());
				resolvers.Add(typeof(IXPInstantFeedbackSourceSettingsModel), (codeName) => InstantFeedback.GetInitializer(codeName)());
			}
			internal static Initializer GetInitializer(IDataSourceSettingsModel settingsModel) {
				return resolvers[settingsModel.Key];
			}
		}
		delegate IDataSourceGenerator Initializer();
		static class DirectBinding {
			static IDictionary<DataAccessTechnologyCodeName, Initializer> initializers;
			static DirectBinding() {
				initializers = new Dictionary<DataAccessTechnologyCodeName, Initializer>();
				initializers.Add(DataAccessTechnologyCodeName.TypedDataSet, () => new TypedDataSetDirectBindingGenerator());
				initializers.Add(DataAccessTechnologyCodeName.SQLDataSource, () => new SQLDataSourceDirectBindingGenerator());
				initializers.Add(DataAccessTechnologyCodeName.ExcelDataSource, () => new ExcelDataSourceDirectBindingGenerator());
				initializers.Add(DataAccessTechnologyCodeName.EntityFramework, () => new EntityFrameworkDirectBindingGenerator());
				initializers.Add(DataAccessTechnologyCodeName.LinqToSql, () => new LinqToSqlDirectBindingGenerator());
				initializers.Add(DataAccessTechnologyCodeName.Wcf, () => new WcfDirectBindingGenerator());
				initializers.Add(DataAccessTechnologyCodeName.IEnumerable, () => new IEnumerableDirectBindingGenerator());
				initializers.Add(DataAccessTechnologyCodeName.Enum, () => new EnumDirectBindingGenerator());
			}
			internal static Initializer GetInitializer(DataAccessTechnologyCodeName codeName) {
				return initializers[codeName];
			}
		}
		static class TypedList {
			static IDictionary<DataAccessTechnologyCodeName, Initializer> initializers;
			static TypedList() {
				initializers = new Dictionary<DataAccessTechnologyCodeName, Initializer>();
				initializers.Add(DataAccessTechnologyCodeName.IEnumerable, () => new IEnumerableTypedListSourceGenerator());
			}
			internal static Initializer GetInitializer(DataAccessTechnologyCodeName codeName) {
				return initializers[codeName];
			}
		}
		static class XPCollectionSource {
			static IDictionary<DataAccessTechnologyCodeName, Initializer> initializers;
			static XPCollectionSource() {
				initializers = new Dictionary<DataAccessTechnologyCodeName, Initializer>();
				initializers.Add(DataAccessTechnologyCodeName.XPO, () => new XPCollectionSourceGenerator());
			}
			internal static Initializer GetInitializer(DataAccessTechnologyCodeName codeName) {
				return initializers[codeName];
			}
		}
		static class XPViewSource {
			static IDictionary<DataAccessTechnologyCodeName, Initializer> initializers;
			static XPViewSource() {
				initializers = new Dictionary<DataAccessTechnologyCodeName, Initializer>();
				initializers.Add(DataAccessTechnologyCodeName.XPO, () => new XPViewSourceGenerator());
			}
			internal static Initializer GetInitializer(DataAccessTechnologyCodeName codeName) {
				return initializers[codeName];
			}
		}
		static class XmlDataSet {
			static IDictionary<DataAccessTechnologyCodeName, Initializer> initializers;
			static XmlDataSet() {
				initializers = new Dictionary<DataAccessTechnologyCodeName, Initializer>();
				initializers.Add(DataAccessTechnologyCodeName.XmlDataSet, () => new XmlDataSetSourceGenerator());
			}
			internal static Initializer GetInitializer(DataAccessTechnologyCodeName codeName) {
				return initializers[codeName];
			}
		}
		static class BindingListView {
			static IDictionary<DataAccessTechnologyCodeName, Initializer> initializers;
			static BindingListView() {
				initializers = new Dictionary<DataAccessTechnologyCodeName, Initializer>();
				initializers.Add(DataAccessTechnologyCodeName.TypedDataSet, () => new TypedDataSetBindingListViewSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.SQLDataSource, () => new SQLDataSourceBindingListViewSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.ExcelDataSource, () => new ExcelDataSourceBindingListViewSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.EntityFramework, () => new EntityFrameworkBindingListViewSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.LinqToSql, () => new LinqToSqlBindingListViewSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.Wcf, () => new WcfBindingListViewSourceGenerator());
			}
			internal static Initializer GetInitializer(DataAccessTechnologyCodeName codeName) {
				return initializers[codeName];
			}
		}
		static class ServerMode {
			static IDictionary<DataAccessTechnologyCodeName, Initializer> initializers;
			static ServerMode() {
				initializers = new Dictionary<DataAccessTechnologyCodeName, Initializer>();
				initializers.Add(DataAccessTechnologyCodeName.EntityFramework, () => new EntityFrameworkServerModeSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.LinqToSql, () => new LinqToSqlServerModeSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.Wcf, () => new WcfServerModeSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.XPO, () => new XPOServerModeSourceGenerator());
			}
			internal static Initializer GetInitializer(DataAccessTechnologyCodeName codeName) {
				return initializers[codeName];
			}
		}
		static class InstantFeedback {
			static IDictionary<DataAccessTechnologyCodeName, Initializer> initializers;
			static InstantFeedback() {
				initializers = new Dictionary<DataAccessTechnologyCodeName, Initializer>();
				initializers.Add(DataAccessTechnologyCodeName.EntityFramework, () => new EntityFrameworkInstantFeedbackSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.LinqToSql, () => new LinqToSqlInstantFeedbackSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.Wcf, () => new WcfInstantFeedbackSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.XPO, () => new XPOInstantFeedbackSourceGenerator());
			}
			internal static Initializer GetInitializer(DataAccessTechnologyCodeName codeName) {
				return initializers[codeName];
			}
		}
		static class PLinqServerMode {
			static IDictionary<DataAccessTechnologyCodeName, Initializer> initializers;
			static PLinqServerMode() {
				initializers = new Dictionary<DataAccessTechnologyCodeName, Initializer>();
				initializers.Add(DataAccessTechnologyCodeName.EntityFramework, () => new EntityFrameworkPLinqServerModeSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.LinqToSql, () => new LinqToSqlPLinqServerModeSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.IEnumerable, () => new IEnumetablePLinqServerModeSourceGenerator());
			}
			internal static Initializer GetInitializer(DataAccessTechnologyCodeName codeName) {
				return initializers[codeName];
			}
		}
		static class PLinqInstantFeedback {
			static IDictionary<DataAccessTechnologyCodeName, Initializer> initializers;
			static PLinqInstantFeedback() {
				initializers = new Dictionary<DataAccessTechnologyCodeName, Initializer>();
				initializers.Add(DataAccessTechnologyCodeName.EntityFramework, () => new EntityFrameworkPLinqInstantFeedbackSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.LinqToSql, () => new LinqToSqlPLinqInstantFeedbackSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.IEnumerable, () => new IEnumetablePLinqInstantFeedbackSourceGenerator());
			}
			internal static Initializer GetInitializer(DataAccessTechnologyCodeName codeName) {
				return initializers[codeName];
			}
		}
		static class OLAP {
			static IDictionary<DataAccessTechnologyCodeName, Initializer> initializers;
			static OLAP() {
				initializers = new Dictionary<DataAccessTechnologyCodeName, Initializer>();
				initializers.Add(DataAccessTechnologyCodeName.OLAP, () => new OLAPSourceGenerator());
			}
			internal static Initializer GetInitializer(DataAccessTechnologyCodeName codeName) {
				return initializers[codeName];
			}
		}
	}
}
