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
	using System;
	using System.Collections.Generic;
	public interface IDataAccessMetadata : ICustomBindingPropertiesMetadata {
		PlatformCodeName Platform { get; }
		IEnumerable<DataAccessTechnologyCodeName> SupportedTechnologies { get; }
		IEnumerable<DataProcessingModeCodeName> SupportedProcessingModes { get; }
		IDictionary<DataAccessTechnologyCodeName, DataProcessingModeCodeName> ExcludeProcessingModes { get; }
		string DataSourceProperty { get; }
		string DataMemberProperty { get; }
		bool CanShowDesignData { get; }
		bool CanShowCodeBehind { get; }
	}
	public interface IOLAPDataAccessMetadata {
		string OLAPConnectionStringProperty { get; }
		string OLAPDataProviderProperty { get; }
	}
	public interface IDashboardDataAccessMetadata {
		string DesignTimeElementTypeProperty { get; }
	}
	static class DataAccessMetadataLoader {
		public static IDataAccessMetadata Load(Type type) {
			AssertionException.IsNotNull(type);
			return LoadMetadataCore(type) ?? DataAccessMetadata.Empty;
		}
		static IDataAccessMetadata LoadMetadataCore(Type type) {
			return Metadata.AttributeHelper.GetAttributeValue<IDataAccessMetadata, Utils.Design.DataAccess.DataAccessMetadataAttribute>(
				type, (attribute) => new DataAccessMetadata(GetPlatform(type), attribute, CustomBindingPropertiesMetadataLoader.Load(type)));
		}
		static PlatformCodeName GetPlatform(Type type) {
			if(typeof(System.Windows.Forms.Control).IsAssignableFrom(type))
				return PlatformCodeName.Win;
			if(typeof(System.ComponentModel.Component).IsAssignableFrom(type))
				return PlatformCodeName.Win;
			if(typeof(System.Web.UI.Control).IsAssignableFrom(type))
				return PlatformCodeName.ASP;
			if(typeof(System.Windows.UIElement).IsAssignableFrom(type))
				return PlatformCodeName.Wpf;
			return PlatformCodeName.Unknown;
		}
		class DataAccessMetadata : IDataAccessMetadata, IOLAPDataAccessMetadata, IDashboardDataAccessMetadata {
			#region static
			public static IDataAccessMetadata Empty;
			static DataAccessTechnologyCodeName[] AllTechnologyCodeNames;
			static DataProcessingModeCodeName[] AllProcessingModeCodeNames;
			static DataAccessMetadata() {
				Empty = new EmptyDataAccessControlMetadata();
				AllTechnologyCodeNames = (DataAccessTechnologyCodeName[])Enum.GetValues(typeof(DataAccessTechnologyCodeName));
				AllProcessingModeCodeNames = (DataProcessingModeCodeName[])Enum.GetValues(typeof(DataProcessingModeCodeName));
			}
			#endregion static
			string stringRepresentation;
			public DataAccessMetadata(PlatformCodeName platform, Utils.Design.DataAccess.DataAccessMetadataAttribute attribute,
				ICustomBindingPropertiesMetadata customBindingPropertiesMetadata) {
				AssertionException.IsNotNullOrEmpty(attribute.SupportedTechnologies);
				CustomBindingProperties = customBindingPropertiesMetadata.CustomBindingProperties;
				if(!string.IsNullOrEmpty(attribute.Platform))
					Platform = GetPlatform(attribute.Platform);
				else
					Platform = platform;
				if(Platform == PlatformCodeName.Win || Platform == PlatformCodeName.ASP) {
					DataSourceProperty = "DataSource";
					DataMemberProperty = "DataMember";
				}
				if(Platform == PlatformCodeName.Wpf || Platform == PlatformCodeName.Silverlight) {
					DataSourceProperty = "ItemsSource";
					DataMemberProperty = "Data";
				}
				if(!string.IsNullOrEmpty(attribute.DataSourceProperty))
					DataSourceProperty = attribute.DataSourceProperty;
				if(!string.IsNullOrEmpty(attribute.DataMemberProperty))
					DataMemberProperty = attribute.DataMemberProperty;
				var technologies = GetNames<DataAccessTechnologyCodeName>(attribute.SupportedTechnologies, AllTechnologyCodeNames);
				if(Platform != PlatformCodeName.Silverlight)
					technologies.Remove(DataAccessTechnologyCodeName.Ria);
				if(Platform != PlatformCodeName.Win) {
					technologies.Remove(DataAccessTechnologyCodeName.XmlDataSet);
					technologies.Remove(DataAccessTechnologyCodeName.SQLDataSource);
					technologies.Remove(DataAccessTechnologyCodeName.ExcelDataSource);
				}
				if(Platform != PlatformCodeName.Win && Platform != PlatformCodeName.Wpf)
					technologies.Remove(DataAccessTechnologyCodeName.XPO);
				CanShowDesignData = (Platform == PlatformCodeName.Wpf);
				CanShowCodeBehind = (Platform == PlatformCodeName.Win);
				if(!attribute.EnableBindingToEnum)
					technologies.Remove(DataAccessTechnologyCodeName.Enum);
				var olapAttribute = attribute as Utils.Design.DataAccess.OLAPDataAccessMetadataAttribute;
				if(olapAttribute != null) {
					OLAPConnectionStringProperty = "OLAPConnectionString";
					if(Platform != PlatformCodeName.Silverlight)
						OLAPDataProviderProperty = "OLAPDataProvider";
					if(!string.IsNullOrEmpty(olapAttribute.OLAPConnectionStringProperty))
						OLAPConnectionStringProperty = olapAttribute.OLAPConnectionStringProperty;
					if(!string.IsNullOrEmpty(olapAttribute.OLAPDataProviderProperty))
						OLAPDataProviderProperty = olapAttribute.OLAPDataProviderProperty;
				}
				else technologies.Remove(DataAccessTechnologyCodeName.OLAP);
				var dashboardAttribute = attribute as Utils.Design.DataAccess.DashboardDataAccessMetadataAttribute;
				if(dashboardAttribute != null) {
					DesignTimeElementTypeProperty = "DesignTimeElementType";
					if(!string.IsNullOrEmpty(dashboardAttribute.DesignTimeElementTypeProperty))
						DesignTimeElementTypeProperty = dashboardAttribute.DesignTimeElementTypeProperty;
				}
				DataProcessingModeCodeName baseProcessingMode = GetBaseProcessingMode(Platform);
				SupportedTechnologies = technologies;
				SupportedProcessingModes = new DataProcessingModeCodeName[] { baseProcessingMode };
				if(!string.IsNullOrEmpty(attribute.SupportedProcessingModes)) {
					var processingModes = GetProcessingModes(attribute.SupportedProcessingModes);
					processingModes.Remove(GetBaseProcessingModeToRemove(baseProcessingMode));
					if(Platform != PlatformCodeName.Unknown) {
						if(olapAttribute == null) {
							processingModes.Remove(DataProcessingModeCodeName.XMLAforOLAP);
							processingModes.Remove(DataProcessingModeCodeName.OLEDBforOLAP);
							processingModes.Remove(DataProcessingModeCodeName.ADOMDforOLAP);
						}
						if(Platform == PlatformCodeName.Win) {
							processingModes.Remove(DataProcessingModeCodeName.InMemoryCollectionView);
							if(dashboardAttribute != null)
								processingModes.Remove(DataProcessingModeCodeName.InMemoryBindingSource);
						}
						else {
							processingModes.Remove(DataProcessingModeCodeName.InMemoryBindingSource);
							if(Platform != PlatformCodeName.Wpf) {
								processingModes.Remove(DataProcessingModeCodeName.XPCollectionForXPO);
								processingModes.Remove(DataProcessingModeCodeName.XPViewForXPO);
							}
							if(olapAttribute != null && platform == PlatformCodeName.Silverlight) {
								processingModes.Remove(DataProcessingModeCodeName.OLEDBforOLAP);
								processingModes.Remove(DataProcessingModeCodeName.ADOMDforOLAP);
							}
						}
					}
					if(!processingModes.Contains(baseProcessingMode))
						processingModes.Insert(0, baseProcessingMode);
					SupportedProcessingModes = processingModes;
				}
				ExcludeProcessingModes = new Dictionary<DataAccessTechnologyCodeName, DataProcessingModeCodeName>();
				if(!attribute.EnableDirectBinding) {
					ExcludeProcessingModes.Add(DataAccessTechnologyCodeName.TypedDataSet, DataProcessingModeCodeName.DirectBinding);
					ExcludeProcessingModes.Add(DataAccessTechnologyCodeName.SQLDataSource, DataProcessingModeCodeName.DirectBinding);
					ExcludeProcessingModes.Add(DataAccessTechnologyCodeName.ExcelDataSource, DataProcessingModeCodeName.DirectBinding);
				}
				UpdateStringRepresentation();
			}
			static DataProcessingModeCodeName GetBaseProcessingMode(PlatformCodeName platform) {
				return (platform == PlatformCodeName.Wpf || platform == PlatformCodeName.Silverlight) ?
					DataProcessingModeCodeName.SimpleBinding : DataProcessingModeCodeName.DirectBinding;
			}
			static DataProcessingModeCodeName GetBaseProcessingModeToRemove(DataProcessingModeCodeName mode) {
				return (mode == DataProcessingModeCodeName.DirectBinding) ?
					DataProcessingModeCodeName.SimpleBinding : DataProcessingModeCodeName.DirectBinding;
			}
			void UpdateStringRepresentation() {
				var sb = new System.Text.StringBuilder();
				foreach(DataAccessTechnologyCodeName codeName in SupportedTechnologies)
					sb.Append((sb.Length == 0 ? '{' : ';') + codeName.ToString());
				if(sb.Length > 0)
					stringRepresentation = sb.ToString() + "}";
			}
			public void SetCanShowDesignData(bool showData) {
				CanShowDesignData = showData;
			}
			public override string ToString() {
				return stringRepresentation;
			}
			static PlatformCodeName GetPlatform(string platformString) {
				string[] entries = platformString.Split(
					new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
				for(int i = 0; i < entries.Length; i++) {
					PlatformCodeName result;
					if(Enum.TryParse<PlatformCodeName>(entries[i], true, out result))
						return result;
				}
				return PlatformCodeName.Unknown;
			}
			static List<DataProcessingModeCodeName> GetProcessingModes(string modesString) {
				string tag = modesString.Trim().ToUpperInvariant();
				if(tag == "SIMPLE") {
					return new List<DataProcessingModeCodeName> { 
						DataProcessingModeCodeName.XMLtoDataSet,
						DataProcessingModeCodeName.InMemoryBindingSource,
						DataProcessingModeCodeName.InMemoryCollectionView,
						DataProcessingModeCodeName.OLEDBforOLAP,
						DataProcessingModeCodeName.ADOMDforOLAP,
						DataProcessingModeCodeName.XMLAforOLAP,
						DataProcessingModeCodeName.XPCollectionForXPO,
						DataProcessingModeCodeName.XPViewForXPO,
					};
				}
				if(tag == "PIVOT") {
					return new List<DataProcessingModeCodeName> { 
						DataProcessingModeCodeName.XMLtoDataSet,
						DataProcessingModeCodeName.InMemoryBindingSource,
						DataProcessingModeCodeName.InMemoryCollectionView,
						DataProcessingModeCodeName.ServerMode,
						DataProcessingModeCodeName.OLEDBforOLAP,
						DataProcessingModeCodeName.ADOMDforOLAP,
						DataProcessingModeCodeName.XMLAforOLAP,
						DataProcessingModeCodeName.XPCollectionForXPO,
						DataProcessingModeCodeName.XPViewForXPO,
					};
				}
				if(tag == "GRIDLOOKUP") {
					return new List<DataProcessingModeCodeName> { 
						DataProcessingModeCodeName.XMLtoDataSet,
						DataProcessingModeCodeName.InMemoryBindingSource,
						DataProcessingModeCodeName.InMemoryCollectionView,
						DataProcessingModeCodeName.ServerMode,
						DataProcessingModeCodeName.XPCollectionForXPO,
						DataProcessingModeCodeName.XPViewForXPO,
					};
				}
				if(tag == "SEARCHLOOKUP") {
					return new List<DataProcessingModeCodeName> { 
						DataProcessingModeCodeName.XMLtoDataSet,
						DataProcessingModeCodeName.InMemoryBindingSource,
						DataProcessingModeCodeName.InMemoryCollectionView,
						DataProcessingModeCodeName.ServerMode,
						DataProcessingModeCodeName.InstantFeedback,
						DataProcessingModeCodeName.XPCollectionForXPO,
						DataProcessingModeCodeName.XPViewForXPO,
					};
				}
				return GetNames<DataProcessingModeCodeName>(modesString, AllProcessingModeCodeNames);
			}
			static List<T> GetNames<T>(string csvString, T[] allCodeNames) where T : struct {
				if(csvString.Trim().ToUpperInvariant() == "ALL")
					return new List<T>(allCodeNames);
				string[] entries = csvString.Split(
					new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
				var names = new List<T>();
				for(int i = 0; i < allCodeNames.Length; i++) {
					string codeNameStr = allCodeNames[i].ToString();
					if(!Contains(entries, codeNameStr))
						continue;
					names.Add(allCodeNames[i]);
				}
				return names;
			}
			static bool Contains(string[] technologies, string codeName) {
				for(int i = 0; i < technologies.Length; i++) {
					if(string.Compare(technologies[i], codeName, true) == 0)
						return true;
				}
				return false;
			}
			public PlatformCodeName Platform { get; private set; }
			public IEnumerable<DataAccessTechnologyCodeName> SupportedTechnologies { get; private set; }
			public IEnumerable<DataProcessingModeCodeName> SupportedProcessingModes { get; private set; }
			public IEnumerable<ICustomBindingPropertyMetadata> CustomBindingProperties { get; private set; }
			public IDictionary<DataAccessTechnologyCodeName, DataProcessingModeCodeName> ExcludeProcessingModes { get; private set; }
			public string DataSourceProperty { get; private set; }
			public string DataMemberProperty { get; private set; }
			public string OLAPConnectionStringProperty { get; private set; }
			public string OLAPDataProviderProperty { get; private set; }
			public string DesignTimeElementTypeProperty { get; private set; }
			public bool CanShowDesignData { get; private set; }
			public bool CanShowCodeBehind { get; private set; }
			#region Empty
			class EmptyDataAccessControlMetadata : IDataAccessMetadata {
				DataAccessTechnologyCodeName[] emptyTechnologies = new DataAccessTechnologyCodeName[0];
				DataProcessingModeCodeName[] emptyModes = new DataProcessingModeCodeName[0];
				ICustomBindingPropertyMetadata[] emptyProperties = new ICustomBindingPropertyMetadata[0];
				IDictionary<DataAccessTechnologyCodeName, DataProcessingModeCodeName> emptyExcludeModes = new Dictionary<DataAccessTechnologyCodeName, DataProcessingModeCodeName>();
				public IEnumerable<DataAccessTechnologyCodeName> SupportedTechnologies {
					get { return emptyTechnologies; }
				}
				public IEnumerable<DataProcessingModeCodeName> SupportedProcessingModes {
					get { return emptyModes; }
				}
				public IEnumerable<ICustomBindingPropertyMetadata> CustomBindingProperties {
					get { return emptyProperties; }
				}
				public IDictionary<DataAccessTechnologyCodeName, DataProcessingModeCodeName> ExcludeProcessingModes {
					get { return emptyExcludeModes; }
				}
				public PlatformCodeName Platform {
					get { return PlatformCodeName.Unknown; }
				}
				public string DataSourceProperty {
					get { return string.Empty; }
				}
				public string DataMemberProperty {
					get { return string.Empty; }
				}
				public bool CanShowDesignData {
					get { return false; }
				}
				public bool CanShowCodeBehind {
					get { return false; }
				}
				public override string ToString() {
					return "{Empty}";
				}
				public void SetCanShowDesignData(bool showData) { }
			}
			#endregion Empty
		}
	}
	public interface ICustomBindingPropertyMetadata {
		string PropertyName { get; }
		string Name { get; }
		string Description { get; }
	}
	public interface ICustomBindingPropertiesMetadata {
		IEnumerable<ICustomBindingPropertyMetadata> CustomBindingProperties { get; }
	}
	static class CustomBindingPropertiesMetadataLoader {
		public static ICustomBindingPropertiesMetadata Load(Type type) {
			AssertionException.IsNotNull(type);
			return GetLookupBindingPropertiesMetadata(type) ?? GetCustomBindingPropertiesMetadata(type) ?? Empty;
		}
		static ICustomBindingPropertiesMetadata LoadMetadataCore(Type type) {
			return GetLookupBindingPropertiesMetadata(type) ?? GetCustomBindingPropertiesMetadata(type);
		}
		static ICustomBindingPropertiesMetadata GetCustomBindingPropertiesMetadata(Type type) {
			return Metadata.AttributeHelper.GetAttributeValue<ICustomBindingPropertiesMetadata, Utils.Design.DataAccess.CustomBindingPropertiesAttribute>(
				type, (attribute) => new CustomBindingPropertiesMetadata(attribute));
		}
		static ICustomBindingPropertiesMetadata GetLookupBindingPropertiesMetadata(Type type) {
			return Metadata.AttributeHelper.GetAttributeValue<ICustomBindingPropertiesMetadata, System.ComponentModel.LookupBindingPropertiesAttribute>(
				type, (attribute) => new LookupBindingPropertiesMetadata(type, attribute));
		}
		class CustomBindingPropertiesMetadata : ICustomBindingPropertiesMetadata {
			public CustomBindingPropertiesMetadata(Utils.Design.DataAccess.CustomBindingPropertiesAttribute attribute) {
				CustomBindingProperties = System.Linq.Enumerable.Select(attribute.GetCustomBindingProperties(),
					(p) => new CustomBindingPropertyMetadata(p));
			}
			public IEnumerable<ICustomBindingPropertyMetadata> CustomBindingProperties { get; private set; }
			class CustomBindingPropertyMetadata : ICustomBindingPropertyMetadata {
				Utils.Design.DataAccess.ICustomBindingProperty property;
				public CustomBindingPropertyMetadata(Utils.Design.DataAccess.ICustomBindingProperty property) {
					AssertionException.IsNotNull(property);
					this.property = property;
				}
				public string PropertyName {
					get { return property.PropertyName; }
				}
				public string Name {
					get { return property.DisplayName; }
				}
				public string Description {
					get { return property.Description; }
				}
			}
		}
		class LookupBindingPropertiesMetadata : ICustomBindingPropertiesMetadata {
			public LookupBindingPropertiesMetadata(Type type, System.ComponentModel.LookupBindingPropertiesAttribute attribute) {
				var properties = System.ComponentModel.TypeDescriptor.GetProperties(type);
				var propertiesHashSet = new HashSet<string>();
				if(!string.IsNullOrEmpty(attribute.DisplayMember))
					propertiesHashSet.Add(attribute.DisplayMember);
				if(!string.IsNullOrEmpty(attribute.ValueMember))
					propertiesHashSet.Add(attribute.ValueMember);
				var metadataList = new List<ICustomBindingPropertyMetadata>();
				foreach(var sProp in propertiesHashSet)
					metadataList.Add(new LookupBindingPropertyMetadata(properties[sProp]));
				CustomBindingProperties = metadataList.ToArray();
			}
			public IEnumerable<ICustomBindingPropertyMetadata> CustomBindingProperties { get; private set; }
			class LookupBindingPropertyMetadata : ICustomBindingPropertyMetadata {
				System.ComponentModel.PropertyDescriptor descriptor;
				public LookupBindingPropertyMetadata(System.ComponentModel.PropertyDescriptor descriptor) {
					AssertionException.IsNotNull(descriptor);
					this.descriptor = descriptor;
				}
				public string PropertyName {
					get { return descriptor.Name; }
				}
				public string Name {
					get { return descriptor.DisplayName; }
				}
				public string Description {
					get { return descriptor.Description; }
				}
			}
		}
		#region Empty
		public static ICustomBindingPropertiesMetadata Empty = new EmptyCustomBindingPropertiesMetadata();
		class EmptyCustomBindingPropertiesMetadata : ICustomBindingPropertiesMetadata {
			public EmptyCustomBindingPropertiesMetadata() {
				CustomBindingProperties = new ICustomBindingPropertyMetadata[] { };
			}
			public override string ToString() { return "{Empty}"; }
			public IEnumerable<ICustomBindingPropertyMetadata> CustomBindingProperties { get; private set; }
		}
		#endregion Empty
	}
}
