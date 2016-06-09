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

extern alias Platform;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Design;
using System.Collections.Generic;
using System.Windows;
using System;
namespace DevExpress.Xpf.Map.Design {
	public sealed class MapControlPropertyLinesProvider : PropertyLinesProviderBase {
		public MapControlPropertyLinesProvider() : base(typeof(MapControl)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			IPropertyLineCommandProvider connectToBingMapsProvider = new ConnectToBingMapsProvider(viewModel);
			IPropertyLineCommandProvider connectToTheOpenStreetMapServerProvider = new ConnectToTheOpenStreetMapServerProvider(viewModel);
			IPropertyLineCommandProvider loadFromDataSourceProvider = new LoadFromDataSourceProvider(viewModel);
			IPropertyLineCommandProvider loadFromShapeFileProvider = new LoadFromShapeFileProvider(viewModel);
			IPropertyLineCommandProvider loadFromKmlProvider = new LoadFromKmlProvider(viewModel);
			IPropertyLineCommandProvider enableMSBingSearchProvider = new EnableMSBingSearchProvider(viewModel);
			IPropertyLineCommandProvider enableMSBingGeocodeProvider = new EnableMSBingGeocodeProvider(viewModel);
			IPropertyLineCommandProvider enableMSBingRouteProvider = new EnableMSBingRouteProvider(viewModel);
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(connectToBingMapsProvider));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(connectToTheOpenStreetMapServerProvider));
			lines.Add(() => new SeparatorLineViewModel(viewModel));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(loadFromShapeFileProvider));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(loadFromKmlProvider));
			lines.Add(() => new SeparatorLineViewModel(viewModel));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(loadFromDataSourceProvider));
			lines.Add(() => new SeparatorLineViewModel(viewModel));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(enableMSBingSearchProvider));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(enableMSBingGeocodeProvider));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(enableMSBingRouteProvider));
			return lines;
		}
	}
	public class ConnectToBingMapsProvider : CommandActionLineProvider {
		public ConnectToBingMapsProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override string GetCommandText() {
			return "Connect to Bing Maps";
		}
		protected override void OnCommandExecute(object param) {
			if(Context != null && Context.ModelItem != null && Context.ModelItem.View != null && Context.ModelItem.View.PlatformObject is MapControl) {
				IModelItem imageTilesLayer = null;
				foreach(IModelItem layer in Context.ModelItem.Properties["Layers"].Collection) {
					if(layer.ItemType == typeof(ImageTilesLayer)) {
						imageTilesLayer = layer;
						break;
					}
				}
				if(imageTilesLayer == null)
					imageTilesLayer = Context.ModelItem.Properties["Layers"].Collection.Add(new ImageTilesLayer());
				if(!(imageTilesLayer.Properties["DataProvider"].ComputedValue is BingMapDataProvider)) {
					IModelItem bingMapDataProvider = imageTilesLayer.Properties["DataProvider"].SetValue(new BingMapDataProvider());
					bingMapDataProvider.Properties["Kind"].SetValue(BingMapKind.Area);
					bingMapDataProvider.Properties["BingKey"].SetValue("INSERT_BING_MAPS_KEY");
				}
			}
		}
	}
	public class ConnectToTheOpenStreetMapServerProvider : CommandActionLineProvider {
		public ConnectToTheOpenStreetMapServerProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override string GetCommandText() {
			return "Connect to OpenStreetMap";
		}
		protected override void OnCommandExecute(object param) {
			if(Context != null && Context.ModelItem != null && Context.ModelItem.View != null && Context.ModelItem.View.PlatformObject is MapControl) {
				IModelItem imageTilesLayer = null;
				foreach(IModelItem layer in Context.ModelItem.Properties["Layers"].Collection) {
					if(layer.ItemType == typeof(ImageTilesLayer)) {
						imageTilesLayer = layer;
						break;
					}
				}
				if(imageTilesLayer == null)
					imageTilesLayer = Context.ModelItem.Properties["Layers"].Collection.Add(new ImageTilesLayer());
				if(!(imageTilesLayer.Properties["DataProvider"].ComputedValue is OpenStreetMapDataProvider)) {
					IModelItem openStreetMapDataProvider = imageTilesLayer.Properties["DataProvider"].SetValue(new OpenStreetMapDataProvider());
					openStreetMapDataProvider.Properties["TileUriTemplate"].SetValue("http://{subdomain}.tile.INSERT_SERVER_NAME.com/{tileLevel}/{tileX}/{tileY}.png");
				}
			}
		}
	}
	public class LoadFromShapeFileProvider : CommandActionLineProvider {
		public LoadFromShapeFileProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override string GetCommandText() {
			return "Load Shapes from Shapefile";
		}
		protected override void OnCommandExecute(object param) {
			if(Context != null && Context.ModelItem != null && Context.ModelItem.View != null && Context.ModelItem.View.PlatformObject is MapControl) {
				IModelItem vectorLayer = null;
				foreach(IModelItem layer in Context.ModelItem.Properties["Layers"].Collection) {
					if((layer.ItemType == typeof(VectorLayer)) && (!layer.Properties["Data"].IsSet)) {
						vectorLayer = layer;
						break;
					}
				}
				if(vectorLayer == null)
					vectorLayer = Context.ModelItem.Properties["Layers"].Collection.Add(new VectorLayer());
				if (!(vectorLayer.Properties["Data"].ComputedValue is ShapefileDataAdapter)) {
					IModelItem shapeFileLoader = vectorLayer.Properties["Data"].SetValue(new ShapefileDataAdapter());
					shapeFileLoader.Properties["FileUri"].SetValue("INSERT_FILE_URI");
				}
			}
		}
	}
	public class LoadFromKmlProvider : CommandActionLineProvider {
		public LoadFromKmlProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override string GetCommandText() {
			return "Load Shapes from KML";
		}
		protected override void OnCommandExecute(object param) {
			if(Context != null && Context.ModelItem != null && Context.ModelItem.View != null && Context.ModelItem.View.PlatformObject is MapControl) {
				IModelItem vectorLayer = null;
				foreach(IModelItem layer in Context.ModelItem.Properties["Layers"].Collection)
					if ((layer.ItemType == typeof(VectorLayer)) && (!layer.Properties["Data"].IsSet)) {
						vectorLayer = layer;
						break;
					}
				if(vectorLayer == null)
					vectorLayer = Context.ModelItem.Properties["Layers"].Collection.Add(new VectorLayer());
				if(!(vectorLayer.Properties["Data"].ComputedValue is KmlFileDataAdapter)) {
					IModelItem kmlFileLoader = vectorLayer.Properties["Data"].SetValue(new KmlFileDataAdapter());
					kmlFileLoader.Properties["FileUri"].SetValue("INSERT_FILE_URI");
				}
			}
		}
	}
	public class LoadFromDataSourceProvider : CommandActionLineProvider {
		public LoadFromDataSourceProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override string GetCommandText() {
			return "Load Items from a Data Source";
		}
		protected override void OnCommandExecute(object param) {
			if(Context != null && Context.ModelItem != null && Context.ModelItem.View != null && Context.ModelItem.View.PlatformObject is MapControl) {
				IModelItem vectorLayer = null;
				foreach(IModelItem layer in Context.ModelItem.Properties["Layers"].Collection)
					if((layer.ItemType == typeof(VectorLayer)) && (!layer.Properties["Data"].IsSet)) {
						vectorLayer = layer;
						break;
					}
				if(vectorLayer == null)
					vectorLayer = Context.ModelItem.Properties["Layers"].Collection.Add(new VectorLayer());
				IModelProperty dataProperty = vectorLayer.Properties["Data"];
				IModelItem dataItem = dataProperty.SetValue(new ListSourceDataAdapter());
				dataItem.Properties["Mappings"].SetValue(new MapItemMappingInfo());
				XpfModelItem.ToModelItem(Context.ModelItem).Context.Services.GetService<SmartTagDesignService>().IsSmartTagButtonPressed = false;
				DevExpress.Xpf.Core.Design.DataAccess.UI.ItemsSourceWizard.Run(XpfModelItem.ToModelItem(dataProperty.Value));
			}
		}
	}
	public class EnableMSBingSearchProvider : CommandActionLineProvider {
		public EnableMSBingSearchProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override string GetCommandText() {
			return "Enable MS Bing Search";
		}
		protected override void OnCommandExecute(object param) {
			if(Context != null && Context.ModelItem != null && Context.ModelItem.View != null && Context.ModelItem.View.PlatformObject is MapControl) {
				IModelItem informationLayer = null;
				foreach(IModelItem layer in Context.ModelItem.Properties["Layers"].Collection)
					if(layer.ItemType == typeof(InformationLayer)) {
						if(layer.Properties["DataProvider"].ComputedValue is BingSearchDataProvider) {
							informationLayer = layer;
							break;
						} else if(!layer.Properties["DataProvider"].IsSet)
							informationLayer = layer;
					}
				if(informationLayer == null)
					informationLayer = Context.ModelItem.Properties["Layers"].Collection.Add(new InformationLayer());
				if(!informationLayer.Properties["DataProvider"].IsSet) {
					IModelItem bingSearchDataProvider = informationLayer.Properties["DataProvider"].SetValue(new BingSearchDataProvider());
					bingSearchDataProvider.Properties["BingKey"].SetValue("INSERT_BING_MAPS_KEY");
				}
			}
		}
	}
	public class EnableMSBingGeocodeProvider : CommandActionLineProvider {
		public EnableMSBingGeocodeProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override string GetCommandText() {
			return "Enable MS Bing Geocode";
		}
		protected override void OnCommandExecute(object param) {
			if(Context != null && Context.ModelItem != null && Context.ModelItem.View != null && Context.ModelItem.View.PlatformObject is MapControl) {
				IModelItem informationLayer = null;
				foreach(IModelItem layer in Context.ModelItem.Properties["Layers"].Collection)
					if(layer.ItemType == typeof(InformationLayer)) {
						if(layer.Properties["DataProvider"].ComputedValue is BingGeocodeDataProvider) {
							informationLayer = layer;
							break;
						} else if(!layer.Properties["DataProvider"].IsSet)
							informationLayer = layer;
					}
				if(informationLayer == null)
					informationLayer = Context.ModelItem.Properties["Layers"].Collection.Add(new InformationLayer());
				if(!informationLayer.Properties["DataProvider"].IsSet) {
					IModelItem bingGeocodeDataProvider = informationLayer.Properties["DataProvider"].SetValue(new BingGeocodeDataProvider());
					bingGeocodeDataProvider.Properties["BingKey"].SetValue("INSERT_BING_MAPS_KEY");
				}
			}
		}
	}
	public class EnableMSBingRouteProvider : CommandActionLineProvider {
		public EnableMSBingRouteProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override string GetCommandText() {
			return "Enable MS Bing Route";
		}
		protected override void OnCommandExecute(object param) {
			if(Context != null && Context.ModelItem != null && Context.ModelItem.View != null && Context.ModelItem.View.PlatformObject is MapControl) {
				IModelItem informationLayer = null;
				foreach(IModelItem layer in Context.ModelItem.Properties["Layers"].Collection)
					if(layer.ItemType == typeof(InformationLayer)) {
						if(layer.Properties["DataProvider"].ComputedValue is BingRouteDataProvider) {
							informationLayer = layer;
							break;
						} else if(!layer.Properties["DataProvider"].IsSet)
							informationLayer = layer;
					}
				if(informationLayer == null)
					informationLayer = Context.ModelItem.Properties["Layers"].Collection.Add(new InformationLayer());
				if(!informationLayer.Properties["DataProvider"].IsSet) {
					IModelItem bingGeocodeDataProvider = informationLayer.Properties["DataProvider"].SetValue(new BingRouteDataProvider());
					bingGeocodeDataProvider.Properties["BingKey"].SetValue("INSERT_BING_MAPS_KEY");
				}
			}
		}
	}
}
