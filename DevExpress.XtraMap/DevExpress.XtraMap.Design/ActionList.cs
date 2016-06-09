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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using DevExpress.Utils.Design;
namespace DevExpress.XtraMap.Design {
	public class MapControlActionList : DesignerActionList {
		readonly MapControlDesigner designer;
		readonly DesignerActionUIService designerActionUISvc;
		protected MapControl MapControl { get { return (MapControl)Component; } }
		public bool EnableZooming {
			get { return MapControl.EnableZooming; }
			set { SetPropertyValue(Component, MapPropertyNames.EnableZooming, value); }
		}
		public bool EnableScrolling {
			get { return MapControl.EnableScrolling; }
			set { SetPropertyValue(Component, MapPropertyNames.EnableScrolling, value); }
		}
		public bool ShowToolTips {
			get { return MapControl.ShowToolTips; }
			set { SetPropertyValue(Component, MapPropertyNames.ShowToolTips, value); }
		}
		public MapControlActionList(MapControlDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
			this.designerActionUISvc = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
		}
		void SetPropertyValue(object component, string name, object val) {
			if (component is IComponent && ((IComponent)component).Site != null) {
				PropertyDescriptor property = TypeDescriptor.GetProperties(component)[name];
				if (property != null)
					property.SetValue(component, val);
			}
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection items = new DesignerActionItemCollection();
			items.Add(new DesignerActionMethodItem(this, "EditLayers", "Edit Layers...", true));
			items.Add(new DesignerActionMethodItem(this, "EditLegends", "Edit Legends...", true));
			items.Add(new DesignerActionMethodItem(this, "ConnectToBingMaps", "Connect to Bing Maps", true));
			items.Add(new DesignerActionMethodItem(this, "ConnectToOpenStreetMapServer", "Connect to OpenStreetMap Server", true));
			items.Add(new DesignerActionMethodItem(this, "LoadShapefile", "Load from Shapefile", true));
			items.Add(new DesignerActionMethodItem(this, "LoadKmlFile", "Load from KML file", true));
			items.Add(new DesignerActionMethodItem(this, "LoadSvgFile", "Load from SVG file", true));
			if (!designer.HasMiniMap)
				items.Add(new DesignerActionMethodItem(this, "AddMiniMap", "Add MiniMap", true));
			else
				items.Add(new DesignerActionMethodItem(this, "RemoveMiniMap", "Remove MiniMap", true));
			items.Add(new DesignerActionPropertyItem(MapPropertyNames.EnableZooming, "Enable Zooming"));
			items.Add(new DesignerActionPropertyItem(MapPropertyNames.EnableScrolling, "Enable Scrolling"));
			items.Add(new DesignerActionPropertyItem(MapPropertyNames.ShowToolTips, "Show ToolTips"));
			return items;
		}
		[RefreshProperties(RefreshProperties.All)]
		public void EditLayers() {
			EditorContextHelper.EditValue(designer, MapControl, "Layers");
			EditorContextHelper.FireChanged(designer as IDesigner, MapControl);
		}
		[RefreshProperties(RefreshProperties.All)]
		public void EditLegends() {
			EditorContextHelper.EditValue(designer, MapControl, "Legends");
			EditorContextHelper.FireChanged(designer as IDesigner, MapControl);
		}
		[RefreshProperties(RefreshProperties.All)]
		public void ConnectToBingMaps() {
			designer.ConnectToBingMaps();
		}
		[RefreshProperties(RefreshProperties.All)]
		public void ConnectToOpenStreetMapServer() {
			designer.ConnectToOpenStreetMapServer();
		}
		[RefreshProperties(RefreshProperties.All)]
		public void LoadShapefile() {
			Uri fileUri = DesignHelper.SelectFileUri(null, DesignSR.ShapefileFilter);
			designer.LoadShapefile(fileUri);
		}
		[RefreshProperties(RefreshProperties.All)]
		public void LoadKmlFile() {
			Uri fileUri = DesignHelper.SelectFileUri(null, DesignSR.KmlFileFilter);
			designer.LoadKmlFile(fileUri);
		}
		[RefreshProperties(RefreshProperties.All)]
		public void LoadSvgFile() {
			Uri fileUri = DesignHelper.SelectFileUri(null, DesignSR.SvgFileFilter);
			designer.LoadSvgFile(fileUri);
		}
		[RefreshProperties(RefreshProperties.All)]
		public void AddMiniMap() {
			designer.AddMiniMap();
			designerActionUISvc.Refresh(Component);
		}
		[RefreshProperties(RefreshProperties.All)]
		public void RemoveMiniMap() {
			designer.RemoveMiniMap();
			designerActionUISvc.Refresh(Component);
		}
	}
}
