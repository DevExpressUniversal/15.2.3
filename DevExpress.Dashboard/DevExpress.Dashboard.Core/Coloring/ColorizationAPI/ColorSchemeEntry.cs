#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.Native;
using DevExpress.DataAccess;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using System.Linq;
using System.Xml;
using System.Drawing;
using System;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public class ColorSchemeEntry {
		const string xmlColorSchemeEntry = "Entry";
		const string xmlPaletteIndex = "PaletteIndex";
		const string xmlColor = "Color";
		const string xmlDataMember = "DataMember";
		const string xmlDataSource = "DataSource";
		const string xmlMeasureKey = "MeasureKey";
		const string xmlMeasureDefinition = "Definition";
		const string xmlDimensionKeys = "DimensionKeys";
		const string xmlDimensionKey = "DimensionKey";
		const string xmlDimensionDefinition = "Definition";
		const string xmlDimensionValue = "Value";
		readonly NotifyingCollection<ColorSchemeDimensionKey> dimensionKeys = new NotifyingCollection<ColorSchemeDimensionKey>();
		ColorSchemeMeasureKey measureKey;
		ColorDefinition colorDefinition;
		IDashboardDataSource dataSource;
		string dataSourceName;
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public NotifyingCollection<ColorSchemeDimensionKey> DimensionKeys { get { return dimensionKeys; } }
		[
		DefaultValue(null)
		]
		public IDashboardDataSource DataSource {
			get { return dataSource; }
			set {
				if (dataSource != value) {
					dataSource = value;
					dataSourceName = null;
					OnChanged();
				}
			}
		}
		[
		DefaultValue(null)
		]
		public string DataMember { get; set; }
		internal IDataSourceSchema PickManager { get { return DataSource != null ? DataSource.GetDataSourceSchema(DataMember) : null; } }
		internal string DataSourceName { get { return dataSource != null ? dataSource.ComponentName : dataSourceName; } }
		[
		DefaultValue(null)
		]
		public ColorSchemeMeasureKey MeasureKey {
			get { return measureKey; }
			set {
				if (measureKey != value) {
					measureKey = value;
					OnChanged();
				}
			}
		}
		[
		DefaultValue(null)
		]
		public ColorDefinition ColorDefinition {
			get { return colorDefinition; }
			set {
				if (colorDefinition != value) {
					colorDefinition = value;
					OnChanged();
				}
			}
		}
		internal IChangeService ChangeService { get; set; }
		public ColorSchemeEntry() {
			dimensionKeys.CollectionChanged += (s, e) => OnChanged();
		}
		internal ColorSchemeEntry(string dataSourceName, string dataMember)
			: this() {
			this.dataSourceName = dataSourceName;
			DataMember = dataMember;
		}
		void OnChanged() {
			if (ChangeService != null)
				ChangeService.OnChanged(new ChangedEventArgs(ChangeReason.Coloring, null, null));
		}
		internal XElement SaveToXml() {
			XElement element = new XElement(xmlColorSchemeEntry);
			if (!string.IsNullOrEmpty(DataSourceName))
				element.Add(new XAttribute(xmlDataSource, DataSourceName));
			if(!string.IsNullOrEmpty(DataMember))
				element.Add(new XAttribute(xmlDataMember, DataMember));
			if (ColorDefinition != null) {
				if (ColorDefinition.PaletteIndex.HasValue)
					element.Add(new XAttribute(xmlPaletteIndex, ColorDefinition.PaletteIndex.Value));
				else if (ColorDefinition.Color.HasValue)
					element.Add(new XAttribute(xmlColor, ColorDefinition.Color.Value.ToArgb()));
			}
			if (MeasureKey != null) {
				XElement measureKeyElement = new XElement(xmlMeasureKey);
				foreach (MeasureDefinition measureDefinition in MeasureKey.MeasureDefinitions) {
					XElement measureDefinitionElement = new XElement(xmlMeasureDefinition);
					measureDefinition.SaveToXml(measureDefinitionElement);
					measureKeyElement.Add(measureDefinitionElement);
				}
				element.Add(measureKeyElement);
			}
			if (DimensionKeys.Count > 0) {
				XElement dimensionKeysElement = new XElement(xmlDimensionKeys);
				foreach (ColorSchemeDimensionKey dimensionKey in DimensionKeys) {
					XElement dimensionKeyElement = new XElement(xmlDimensionKey);
					XElement dimensionDefinitionElement = new XElement(xmlDimensionDefinition);
					dimensionKey.DimensionDefinition.SaveToXml(dimensionDefinitionElement);
					dimensionKeyElement.Add(dimensionDefinitionElement);
					XmlHelper.SaveObject(dimensionKeyElement, xmlDimensionValue, dimensionKey.Value);
					dimensionKeysElement.Add(dimensionKeyElement);
				}
				element.Add(dimensionKeysElement);
			}
			return element;
		}
		internal void LoadFromXml(XElement element) {
			dataSourceName = element.GetAttributeValue(xmlDataSource);
			DataMember = element.GetAttributeValue(xmlDataMember);
			string attr = element.GetAttributeValue(xmlPaletteIndex);
			if (!string.IsNullOrEmpty(attr))
				ColorDefinition = new ColorDefinition(XmlHelper.FromString<int>(attr));
			else {
				attr = element.GetAttributeValue(xmlColor);
				if (!string.IsNullOrEmpty(attr))
					ColorDefinition = new ColorDefinition(Color.FromArgb(XmlHelper.FromString<int>(attr)));
			}
			XElement measureKeyElement = element.Element(xmlMeasureKey);
			if (measureKeyElement != null) {
				List<MeasureDefinition> measureDefinitions = new List<MeasureDefinition>();
				foreach (XElement measureDefinitionElement in measureKeyElement.Elements(xmlMeasureDefinition)) {
					MeasureDefinition measureDefinition = new MeasureDefinition();
					measureDefinition.LoadFromXml(measureDefinitionElement);
					measureDefinitions.Add(measureDefinition);
				}
				MeasureKey = new ColorSchemeMeasureKey(measureDefinitions);
			}
			XElement dimensionKeysElement = element.Element(xmlDimensionKeys);
			if (dimensionKeysElement != null) {
				DimensionKeys.BeginUpdate();
				try {
					DimensionKeys.Clear();
					foreach (XElement dimensionKeyElement in dimensionKeysElement.Elements(xmlDimensionKey)) {
						DimensionDefinition dimensionDefinition = null;
						object value = null;
						XElement dimensionDefinitionElement = dimensionKeyElement.Element(xmlDimensionDefinition);
						if (dimensionDefinitionElement != null) {
							dimensionDefinition = new DimensionDefinition();
							dimensionDefinition.LoadFromXml(dimensionDefinitionElement);
						}
						XmlHelper.LoadObject(dimensionKeyElement, xmlDimensionValue, x => value = x);
						if (dimensionDefinition != null)
							DimensionKeys.Add(new ColorSchemeDimensionKey(dimensionDefinition, value));
						else
							throw new XmlException();
					}
				}
				finally {
					DimensionKeys.EndUpdate();
				}
			}
		}
		internal void EnsureConsistency(Dashboard dashboard) {
			if (!string.IsNullOrEmpty(dataSourceName))
				DataSource = dashboard.DataSources.FirstOrDefault<IDashboardDataSource>(ds => ds.ComponentName == dataSourceName);
		}
		internal void EnsureConsistency(ColorSchemeEntry entry) {
			DataSource = entry.DataSource;
		}
	}
}
