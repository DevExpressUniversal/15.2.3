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
using System.Text;
using DevExpress.Utils.Design;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;
using System.Reflection;
using System.Collections;
namespace DevExpress.XtraMap.Design {
	public sealed class MapPropertyNames {
		public const string EnableZooming = "EnableZooming";
		public const string EnableScrolling = "EnableScrolling";
		public const string ShowToolTips = "ShowToolTips";
	}
	public class MapControlDesigner : BaseControlDesigner, IServiceProvider {
		IDesignerHost designerHost;
		IComponentChangeService changeService = null;
		DesignerVerbCollection verbs;
		MapControlActionList mapControlActionList;
		protected MapControl MapControl { get { return Control != null ? (MapControl)Control : null; } }
		internal bool HasMiniMap { get { return MapControl != null && MapControl.MiniMap != null; } }
		public override DesignerVerbCollection DXVerbs { get { return verbs; } }
		public MapControlDesigner() {
			this.verbs = new DesignerVerbCollection(new DesignerVerb[] { new DesignerVerb("About...", OnAboutClick) });
		}
		object IServiceProvider.GetService(Type type) {
			return base.GetService(type);
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if(changeService != null) {
				changeService.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
				changeService.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
			}
			designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			if(designerHost != null)
				designerHost.LoadComplete += new EventHandler(OnLoadComplete);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
			mapControlActionList = new MapControlActionList(this);
			list.Add(mapControlActionList);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
		}
		void OnAboutClick(object sender, EventArgs e) {
			MapControl.About();
		}
		bool CompareLayerByType(LayerBase layer) {
			return true;
		}
		void OnComponentAdded(object sender, ComponentEventArgs args) {
		}
		void OnComponentChanged(object sender, ComponentChangedEventArgs args) {
		}
		void OnLoadComplete(object sender, EventArgs args) {
		}
		LayerBase FindLayerByType(Type layerType) {
			Predicate<LayerBase> predicate = delegate(LayerBase item) { return layerType.IsAssignableFrom(item.GetType()); };
			return MapControl.Layers.Find(predicate);
		}
		ImageTilesLayer FindImageTileLayer() {
			return FindLayerByType(typeof(ImageTilesLayer)) as ImageTilesLayer;
		}
		bool CheckDataAdapterType(LayerBase layer, Type dataAdapterType) {
			VectorItemsLayer vectorLayer = layer as VectorItemsLayer;
			if(vectorLayer != null) {
				IMapDataAdapter dataApapter = vectorLayer.Data;
				return dataApapter == null || dataAdapterType.IsAssignableFrom(dataApapter.GetType());
			}
			return false;
		}
		VectorItemsLayer FindFileLayer(Type dataAdapterType) {
			Predicate<LayerBase> predicate = delegate(LayerBase item) { return CheckDataAdapterType(item, dataAdapterType); };
			return MapControl.Layers.Find(predicate) as VectorItemsLayer;
		}
		void PerformDesignerTransaction(string transDesc, Action action) {
			using(DesignerTransaction transaction = designerHost.CreateTransaction(transDesc)) {
				changeService.OnComponentChanging(MapControl, null);
				action();
				changeService.OnComponentChanged(MapControl, null, null, null);
				transaction.Commit();
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		internal void ConnectToBingMaps() {
			ImageTilesLayer layer = FindImageTileLayer();
			if(layer != null) {
				BingMapDataProvider provider = layer.DataProvider as BingMapDataProvider;
				if(provider != null)
					return;
			}
			Action action = () => {
				if(layer == null) {
					layer = new ImageTilesLayer();
					MapControl.Layers.Add(layer);
				}
				layer.DataProvider = new BingMapDataProvider();
			};
			PerformDesignerTransaction("Connect to Bing Maps", action);
		}
		internal void ConnectToOpenStreetMapServer() {
			ImageTilesLayer layer = FindImageTileLayer();
			if(layer != null) {
				OpenStreetMapDataProvider provider = layer.DataProvider as OpenStreetMapDataProvider;
				if(provider != null)
					return;
			}
			Action action = () => {
				if(layer == null) {
					layer = new ImageTilesLayer();
					MapControl.Layers.Add(layer);
				}
				layer.DataProvider = new OpenStreetMapDataProvider() { TileUriTemplate = DesignSR.OpenStreetInvalidTileUriTemplate };
			};
			PerformDesignerTransaction("Connect to OpenStreetMap Server", action);
		}
		void LoadFile(Uri fileUri, Type fileDataAdapterType, string transDesc) {
			if(fileUri == null)
				return;
			VectorItemsLayer layer = FindFileLayer(fileDataAdapterType);
			if(layer != null) {
				FileDataAdapterBase shapeAdapter = layer.Data as FileDataAdapterBase;
				if(shapeAdapter != null && shapeAdapter.FileUri == fileUri)
					return;
			}
			Action action = () => {
				if(layer == null) {
					layer = new VectorItemsLayer();
					MapControl.Layers.Add(layer);
				}
				FileDataAdapterBase dataAdapter = (FileDataAdapterBase)Activator.CreateInstance(fileDataAdapterType);
				dataAdapter.FileUri = fileUri;
				layer.Data = dataAdapter;
			};
			PerformDesignerTransaction(transDesc, action);
		}
		internal void LoadShapefile(Uri fileUri) {
			LoadFile(fileUri, typeof(ShapefileDataAdapter), "Load Shapefile");
		}
		internal void LoadKmlFile(Uri fileUri) {
			LoadFile(fileUri, typeof(KmlFileDataAdapter), "Load KML file");
		}
		internal void LoadSvgFile(Uri fileUri) {
			LoadFile(fileUri, typeof(SvgFileDataAdapter), "Load SVG file");
		}
		void PerformTransaction(string name, Action action) {
			using(DesignerTransaction transaction = designerHost.CreateTransaction(name)) {
				changeService.OnComponentChanging(MapControl, null);
				action();
				changeService.OnComponentChanged(MapControl, null, null, null);
				transaction.Commit();
			}
		}
		internal void RemoveMiniMap() {
			PerformTransaction("Remove MiniMap", () => { MapControl.MiniMap = null; });
		}
		internal void AddMiniMap() {
			PerformTransaction("Add MiniMap", () => {
				MiniMap map = new MiniMap();
				BingMapDataProvider tilesProvider = new BingMapDataProvider();
				tilesProvider.BingKey = "YOUR BING MAPS KEY";
				MiniMapImageTilesLayer tilesLayer = new MiniMapImageTilesLayer();
				tilesLayer.DataProvider = tilesProvider;
				MiniMapVectorItemsLayer itemsLayer = new MiniMapVectorItemsLayer();
				itemsLayer.Data = new MapItemStorage();
				map.Layers.Add(tilesLayer);
				map.Layers.Add(itemsLayer);
				MapControl.MiniMap = map;
			});
		}
	}
}
