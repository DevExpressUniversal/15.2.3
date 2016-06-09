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

namespace DevExpress.Design.DataAccess.Wpf {
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
				resolvers.Add(typeof(IXPCollectionSourceSettingsModel), (codeName) => XPO.GetInitializer(codeName)());
				resolvers.Add(typeof(IXPViewSourceSettingsModel), (codeName) => XPOView.GetInitializer(codeName)());
				resolvers.Add(typeof(ISimpleBindingSettingsModel), (codeName) => SimpleBinding.GetInitializer(codeName)());
				resolvers.Add(typeof(ICollectionViewSettingsModel), (codeName) => CollectionView.GetInitializer(codeName)());
				resolvers.Add(typeof(IServerModeSettingsModel), (codeName) => ServerMode.GetInitializer(codeName)());
				resolvers.Add(typeof(IInstantFeedbackSettingsModel), (codeName) => InstantFeedback.GetInitializer(codeName)());
				resolvers.Add(typeof(IPLinqServerModeSettingsModel), (codeName) => PLinqServerMode.GetInitializer(codeName)());
				resolvers.Add(typeof(IPLinqInstantFeedbackSettingsModel), (codeName) => PLinqInstantFeedback.GetInitializer(codeName)());
				resolvers.Add(typeof(IOLAPDataSourceSettingsModel), (codeName) => OLAP.GetInitializer(codeName)());
			}
			internal static Initializer GetInitializer(IDataSourceSettingsModel settingsModel) {
				return resolvers[settingsModel.Key];
			}
		}
		delegate IDataSourceGenerator Initializer();
		static class SimpleBinding {
			static IDictionary<DataAccessTechnologyCodeName, Initializer> initializers;
			static SimpleBinding() {
				initializers = new Dictionary<DataAccessTechnologyCodeName, Initializer>();
				initializers.Add(DataAccessTechnologyCodeName.TypedDataSet, () => new TypedDataSetSimpleBindingGenerator());
				initializers.Add(DataAccessTechnologyCodeName.EntityFramework, () => new EntityFrameworkSimpleBindingGenerator());
				initializers.Add(DataAccessTechnologyCodeName.LinqToSql, () => new LinqToSqlSimpleBindingGenerator());
				initializers.Add(DataAccessTechnologyCodeName.Wcf, () => new WcfSimpleBindingGenerator());
				initializers.Add(DataAccessTechnologyCodeName.IEnumerable, () => new IEnumerableSimpleBindingGenerator());
				initializers.Add(DataAccessTechnologyCodeName.Enum, () => new EnumItemsSourceSimpleBindingGenerator());
			}
			internal static Initializer GetInitializer(DataAccessTechnologyCodeName codeName) {
				return initializers[codeName];
			}
		}
		static class CollectionView {
			static IDictionary<DataAccessTechnologyCodeName, Initializer> initializers;
			static CollectionView() {
				initializers = new Dictionary<DataAccessTechnologyCodeName, Initializer>();
				initializers.Add(DataAccessTechnologyCodeName.TypedDataSet, () => new TypedDataSetCollectionViewSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.EntityFramework, () => new EntityFrameworkCollectionViewSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.LinqToSql, () => new LinqToSqlCollectionViewSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.Wcf, () => new WcfCollectionViewSourceGenerator());
				initializers.Add(DataAccessTechnologyCodeName.IEnumerable, () => new IEnumerableCollectionViewSourceGenerator());
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
				initializers.Add(DataAccessTechnologyCodeName.XPO, () => new XPServerCollectionSourceGenerator());
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
				initializers.Add(DataAccessTechnologyCodeName.XPO, () => new XPInstantFeedbackSourceGenerator());
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
		static class XPO { 
			static IDictionary<DataAccessTechnologyCodeName, Initializer> initializers;
			static XPO() {
				initializers = new Dictionary<DataAccessTechnologyCodeName, Initializer>();
				initializers.Add(DataAccessTechnologyCodeName.XPO, () => new XPCollectionSourceGenerator());
			}
			internal static Initializer GetInitializer(DataAccessTechnologyCodeName codeName) {
				return initializers[codeName];
			}
		}
		static class XPOView {
			static IDictionary<DataAccessTechnologyCodeName, Initializer> initializers;
			static XPOView() {
				initializers = new Dictionary<DataAccessTechnologyCodeName, Initializer>();
				initializers.Add(DataAccessTechnologyCodeName.XPO, () => new XPViewSourceGenerator());
			}
			internal static Initializer GetInitializer(DataAccessTechnologyCodeName codeName) {
				return initializers[codeName];
			}
		}
	}
}
