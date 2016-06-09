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
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.Map.Native;
using DevExpress.XtraMap.Native;
using System.Drawing.Design;
using System.Collections;
namespace DevExpress.XtraMap {
	public abstract class DataSourceAdapterBase : CoordinateSystemDataAdapterBase, ILayerDataManagerProvider {
		public const MapItemType MapItemTypeDefault = MapItemType.Unknown;
		readonly LayerDataManager dataManager;
		MapItemMappingInfoBase mappingInfo;
		MapItemAttributeMappingCollection attributeMappings;
		MapItemPropertyMappingCollection propertyMappings;
		string dataMember = "";
		object dataSource;
		protected override bool IsReady {
			get {
				if (DataSource == null)
					return true;
				return DataManager.GetIsDataReady();
			}
		}
		protected internal LayerDataManager DataManager { 
			get { return dataManager; }
		}
		protected internal MapItemMappingInfoBase Mappings {
			get {
				if (mappingInfo == null)
					this.mappingInfo = CreateMappings(DataManager);
				return mappingInfo;
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("DataSourceAdapterBaseDataMember"),
#endif
		Category(SRCategoryNames.Data), DefaultValue(""),
		Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
		public string DataMember {
			get {
				return dataMember;
			}
			set {
				if (Object.Equals(dataMember, value))
					return;
				dataMember = value;
				OnDataPropertyChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("DataSourceAdapterBaseDataSource"),
#endif
		AttributeProvider(typeof(IListSource)), Category(SRCategoryNames.Data), DefaultValue(null)]
		public object DataSource {
			get { return dataSource; }
			set {
				if (Object.Equals(dataSource, value))
					return;
				dataSource = value;
				OnDataPropertyChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("DataSourceAdapterBaseAttributeMappings"),
#endif
		Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MapItemAttributeMappingCollection AttributeMappings {
			get {
				if (attributeMappings == null)
					this.attributeMappings = new MapItemAttributeMappingCollection(DataManager);
				return attributeMappings;
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("DataSourceAdapterBasePropertyMappings"),
#endif
		Category(SRCategoryNames.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(CollectionConverter)),
		Editor("DevExpress.XtraMap.Design.PropertyMappingsCollectionEditor," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign, typeof(UITypeEditor))
		]
		public MapItemPropertyMappingCollection PropertyMappings {
			get {
				if(propertyMappings == null)
					this.propertyMappings = new MapItemPropertyMappingCollection(DataManager);
				return propertyMappings;
			}
		}
		#region ILayerDataManagerProvider members
		LayerDataManager ILayerDataManagerProvider.DataManager { get { return DataManager; } }
		#endregion
		protected abstract MapItemMappingInfoBase CreateMappings(LayerDataManager DataManager);
		protected override void DisposeOverride() {
			base.DisposeOverride();
			if(propertyMappings != null) {
				propertyMappings.Clear();
				propertyMappings = null;
			}
			if(attributeMappings != null) {
				attributeMappings.Clear();
				attributeMappings = null;
			}
			if(dataManager != null) {
				dataManager.Dispose();
			}
		}
		protected override MapItem GetItemBySourceObject(object sourceObject) {
			return DataManager.GetMapItemBySourceObject(sourceObject);
		}
		protected override object GetItemSourceObject(MapItem item) {
			return DataSource != null ? DataManager.GetItemSourceObject(item) : item;
		}
		protected override void LoadData(IMapItemFactory factory) {
			IList listLocker = DataSource as IList;
			if (listLocker != null) {
				lock (listLocker) {
					DataManager.LoadData(factory);
				}
			}
			else
				DataManager.LoadData(factory);
		}	 
		protected override bool IsCSCompatibleTo(MapCoordinateSystem mapCS) {
			return mapCS.PointType == SourceCoordinateSystem.GetSourcePointType();
		}
		protected DataSourceAdapterBase() {
			this.dataManager = new LayerDataManager(this); 
		}
		protected void OnDataPropertyChanged() {
			DataManager.OnDataPropertyChanged();
		}
		protected internal abstract IMapDataEnumerator CreateDataEnumerator(MapDataController controller);	   
		protected internal virtual void FillActualMappings(MappingCollection mappings) {
			Mappings.FillActualMappings(mappings, SourceCoordinateSystem);
		}	   
		protected internal virtual void Aggregate(DataController controller) {
		}
		protected internal void OnDataChanged() {
			NotifyDataChanged(MapUpdateType.Render);
		}
		protected internal void Clear() {
			ClearItems();
		}
		public override void Load() {
			base.Load();
			DataManager.UpdateDataState();
		}
	}
}
