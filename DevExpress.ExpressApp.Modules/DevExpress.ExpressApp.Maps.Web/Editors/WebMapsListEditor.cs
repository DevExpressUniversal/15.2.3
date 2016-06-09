#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Maps.Web.Helpers;
namespace DevExpress.ExpressApp.Maps.Web {
	public class WebMapsListEditor : ListEditor, IComplexListEditor {
		private object selectedObject;
		private IObjectSpace objectSpace;
		private XafApplication application;
		private MapSettings mapSettings;
		public MapViewer MapViewer {
			get {
				return Control as MapViewer;
			}
		}
		public override Templates.IContextMenuTemplate ContextMenuTemplate {
			get { return null; }
		}
		public override SelectionType SelectionType {
			get { return SelectionType.TemporarySelection; }
		}
		protected XafCallbackManager XafCallbackManager {
			get {
				return WebWindow.CurrentRequestPage != null ? ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager : null;
			}
		}
		private string GetMarkerClickScript() {
			if(XafCallbackManager != null) {
				string callBackScript = XafCallbackManager.GetScript("MapsListEditor", string.Format("'{0}' + e", MapObjectClickedCallbackHandler.CallbackParameterPrefix(MapObjectType.Marker)));
				return string.Format("function(s, e) {{ {0} }}", callBackScript);
			}
			return string.Empty;
		}
		private void OnMarkerClicked(Object sender, MapEventArgs e) {
			object key = objectSpace.GetObjectKey(this.ObjectType, e.Info);
			if(key != null) {
				selectedObject = objectSpace.GetObjectByKey(this.ObjectType, key);
				OnSelectionChanged();
				OnProcessSelectedItem();
			}
		}
		private void RegisterMapClickCallback() {
			if(XafCallbackManager != null) {
				var handler = new MapObjectClickedCallbackHandler(MapObjectType.Marker);
				XafCallbackManager.RegisterHandler("MapsListEditor", handler);
				handler.MapObjectClicked += OnMarkerClicked;
			}
		}
		private void UpdateControlDataSource() {
			if(MapViewer != null) {
				if(List != null) {
					if(List is IListServer) {
						throw new Exception("The MapsListEditor doesn't support Server Mode and so cannot use an IListServer object as the data source.");
					}
					MapViewer.MapSettings.Markers.Clear();
					foreach(IMapsMarker marker in List) {
						if(DataManipulationRight.CanRead(ObjectType, null, marker, null, objectSpace))
							((IList<IMapsMarker>)MapViewer.MapSettings.Markers).Add(marker);
					}
				}
			}
		}
		protected override object CreateControlsCore() {
			MapsAspNetModule.SetEmbeddingClientLibraries(application);
			RegisterMapClickCallback();
			return CreateMapControl();
		}
		protected override void OnDataSourceChanged() {
			base.OnDataSourceChanged();
			UpdateControlDataSource();
		}
		protected override void OnControlsCreated() {
			base.OnControlsCreated();
			UpdateControlDataSource();
		}
		protected override void AssignDataSourceToControl(object dataSource) {
		}
		protected WebControl CreateMapControl() {
			if(mapSettings == null)
				mapSettings = MapsAspNetModule.MapSettingsFromModel((IModelMaps)Model, application);
			mapSettings.IsShowDetailsEnabled = true;
			MapViewer mapViewer = new MapViewer(new ObjectInfoHelper(objectSpace), mapSettings);
			mapViewer.ClientSideEvents.MarkerClicked = GetMarkerClickScript();
			return mapViewer;
		}
		public WebMapsListEditor(IModelListView model) : base(model) { }
		public void Setup(CollectionSourceBase collectionSource, XafApplication application) {
			objectSpace = collectionSource.ObjectSpace;
			this.application = application;
		}
		public override IList GetSelectedObjects() {
			return (selectedObject != null) ? new object[] { selectedObject } : new object[0];
		}
		public override void Refresh() { }
		public override Boolean SupportsDataAccessMode(CollectionSourceDataAccessMode dataAccessMode) {
			return (dataAccessMode == CollectionSourceDataAccessMode.Client);
		}
	}
}
