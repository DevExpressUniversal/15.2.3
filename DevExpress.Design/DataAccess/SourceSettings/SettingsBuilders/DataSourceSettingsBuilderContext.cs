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

namespace DevExpress.Design.DataAccess {
	using System.Collections.Generic;
	class DataSourceSettingsBuilderContext : IDataSourceSettingsBuilderContext {
		IDataSourceSettingsModel model;
		IDataProcessingMode mode;
		List<IDataSourceProperty> result;
		public DataSourceSettingsBuilderContext(IDataProcessingMode mode, IDataSourceSettingsModel model) {
			AssertionException.IsNotNull(mode);
#if !DEBUGTEST
			AssertionException.IsNotNull(model);
#endif
			this.result = new List<IDataSourceProperty>();
			this.model = model;
			this.mode = mode;
		}
		#region IDataSourceSettingsBuilderContext
		public IDataAccessMetadata Metadata { get; set; }
		IDataSourceSettingsModel IDataSourceSettingsBuilderContext.Model {
			get { return model; }
		}
		IEnumerable<IDataSourceProperty> IDataSourceSettingsBuilderContext.Result {
			get { return result; }
		}
		IDataProcessingMode IDataSourceSettingsBuilderContext.ProcessingMode {
			get { return mode; }
		}
		void IDataSourceSettingsBuilderContext.ThrowNotSupported() {
			throw new System.NotSupportedException(mode.ToString());
		}
		#endregion IDataSourceSettingsBuilderContext
		protected IDataSourceSettingsBuilderContext AddProperty(DataSourcePropertyCodeName name) {
			result.Add(CreateProperty(name, model));
			return this;
		}
		protected virtual IDataSourceProperty CreateProperty(DataSourcePropertyCodeName name, IDataSourceSettingsModel model) {
			return new DataSourceProperty(name, model);
		}
		protected virtual IDataSourceProperty CreateCustomProperty(IDataSourceSettingsModel model, ICustomBindingPropertyMetadata propertyMetadata) {
			return new CustomBindingDataSourceProperty(model, propertyMetadata);
		}
		public IDataSourceSettingsBuilderContext BuildXmlPath() {
			return AddProperty(DataSourcePropertyCodeName.XmlPath);
		}
		public IDataSourceSettingsBuilderContext BuildExcelPath() {
			return AddProperty(DataSourcePropertyCodeName.ExcelPath);
		}
		public IDataSourceSettingsBuilderContext BuildTables() {
			return AddProperty(DataSourcePropertyCodeName.Tables);
		}
		public IDataSourceSettingsBuilderContext BuildQuery() {
			return AddProperty(DataSourcePropertyCodeName.Query);
		}
		public IDataSourceSettingsBuilderContext BuildElementType() {
			return AddProperty(DataSourcePropertyCodeName.ElementType);
		}
		public IDataSourceSettingsBuilderContext BuildObjectType() {
			return AddProperty(DataSourcePropertyCodeName.ObjectType);
		}
		public IDataSourceSettingsBuilderContext BuildServiceRoot() {
			return AddProperty(DataSourcePropertyCodeName.ServiceRoot);
		}
		public IDataSourceSettingsBuilderContext BuildDefaultSorting() {
			return AddProperty(DataSourcePropertyCodeName.DefaultSorting);
		}
		public IDataSourceSettingsBuilderContext BuildSortDescriptions() {
			return AddProperty(DataSourcePropertyCodeName.SortDescriptions);
		}
		public IDataSourceSettingsBuilderContext BuildCollectionViewSettings() {
			AddProperty(DataSourcePropertyCodeName.GroupAndSortDescriptions);
			AddProperty(DataSourcePropertyCodeName.Culture);
			AddProperty(DataSourcePropertyCodeName.IsSynchronizedWithCurrentItem);
			return this;
		}
		public IDataSourceSettingsBuilderContext BuildBindingSourceSettings() {
			BuildSortDescriptions();
			AddProperty(DataSourcePropertyCodeName.Filter);
			return this;
		}
		public IDataSourceSettingsBuilderContext BuildServerModeSettings() {
			AddProperty(DataSourcePropertyCodeName.KeyExpression);
			return BuildDefaultSorting();
		}
		public IDataSourceSettingsBuilderContext BuildInstantFeedbackSettings() {
			BuildServerModeSettings();
			return this;
		}
		public IDataSourceSettingsBuilderContext BuildDomainDataSourceSettings() {
			AddProperty(DataSourcePropertyCodeName.Query);
			AddProperty(DataSourcePropertyCodeName.GroupAndSortDescriptions);
			AddProperty(DataSourcePropertyCodeName.AutoLoad);
			AddProperty(DataSourcePropertyCodeName.LoadDelay);
			AddProperty(DataSourcePropertyCodeName.LoadInterval);
			AddProperty(DataSourcePropertyCodeName.LoadSize);
			AddProperty(DataSourcePropertyCodeName.RefreshInterval);
			return this;
		}
		public IDataSourceSettingsBuilderContext BuildOLAPDataSourceSettingsOLEDB() {
			AddProperty(DataSourcePropertyCodeName.Provider);
			return BuildOLAPDataSourceSettings();
		}
		public IDataSourceSettingsBuilderContext BuildOLAPDataSourceSettingsADOMD() {
			AddProperty(DataSourcePropertyCodeName.Provider);
			return BuildOLAPDataSourceSettings();
		}
		public IDataSourceSettingsBuilderContext BuildOLAPDataSourceSettingsXMLA() {
			return BuildOLAPDataSourceSettings();
		}
		protected IDataSourceSettingsBuilderContext BuildOLAPDataSourceSettings() {
			AddProperty(DataSourcePropertyCodeName.Server);
			AddProperty(DataSourcePropertyCodeName.Catalog);
			AddProperty(DataSourcePropertyCodeName.Cube);
			AddProperty(DataSourcePropertyCodeName.QueryTimeout);
			AddProperty(DataSourcePropertyCodeName.Culture);
			AddProperty(DataSourcePropertyCodeName.ConnectionTimeout);
			AddProperty(DataSourcePropertyCodeName.UserId);
			AddProperty(DataSourcePropertyCodeName.Password);
			AddProperty(DataSourcePropertyCodeName.ConnectionString);
			return this;
		}
		public IDataSourceSettingsBuilderContext BuildCustomBindingProperties() {
			if(Metadata != null && Metadata.CustomBindingProperties != null) {
				foreach(ICustomBindingPropertyMetadata propertyMetadata in Metadata.CustomBindingProperties)
					result.Add(CreateCustomProperty(model, propertyMetadata));
			}
			return this;
		}
		public IDataSourceSettingsBuilderContext BuildDesignSettings() {
			if(DesignSettingsResolver.CanShowDesignData(Metadata, model))
				AddProperty(DataSourcePropertyCodeName.ShowDesignData);
			if(DesignSettingsResolver.CanShowCodeBehind(Metadata, model))
				AddProperty(DataSourcePropertyCodeName.ShowCodeBehind);
			return this;
		}
		static class DesignSettingsResolver {
			static HashSet<System.Type> modelsWithoutCodeBehind;
			static DesignSettingsResolver() {
				modelsWithoutCodeBehind = new HashSet<System.Type>();
				modelsWithoutCodeBehind.Add(typeof(IXPCollectionSourceSettingsModel));
				modelsWithoutCodeBehind.Add(typeof(IXPViewSourceSettingsModel));
				modelsWithoutCodeBehind.Add(typeof(IXPServerCollectionSourceSettingsModel));
				modelsWithoutCodeBehind.Add(typeof(IXPInstantFeedbackSourceSettingsModel));
			}
			internal static bool CanShowDesignData(IDataAccessMetadata metadata, IDataSourceSettingsModel model) {
				return CanShowDesignData(model) && (metadata != null) && metadata.CanShowDesignData;
			}
			static bool CanShowDesignData(IDataSourceSettingsModel model) {
				return (model == null || model.IsDesignDataAllowed);
			}
			internal static bool CanShowCodeBehind(IDataAccessMetadata metadata, IDataSourceSettingsModel model) {
				return (metadata != null) && metadata.CanShowCodeBehind && CanShowCodeBehind(model);
			}
			static bool CanShowCodeBehind(IDataSourceSettingsModel model) {
				return model != null && !modelsWithoutCodeBehind.Contains(model.Key);
			}
		}
	}
}
