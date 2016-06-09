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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Map;
using DevExpress.Skins;
using DevExpress.Utils;
namespace DevExpress.XtraMap {
	public interface IOwnedElement {
		object Owner { get; set; }
	}
	public interface ILockableObject {
		object UpdateLocker { get; }
	}
	public interface IMapItemFactory {
		MapItem CreateMapItem(MapItemType type, object obj);
	}
	public interface IImage : IDisposable {
		Size Size { get; }
	}
	public interface IMapControl {
		bool IsSkinActive { get; }
		bool IsDisposed { get; }
		ISkinProvider SkinProvider { get; }
		ToolTipController ToolTipController { get; }
		bool IsDesignMode { get; }
		bool IsDesigntimeProcess { get; }
		IntPtr Handle { get; }
		bool IsHandleCreated { get; }
		Cursor Cursor { get; set; }
		bool Capture { get; set; }
		Rectangle ClientRectangle { get; }
		void AddChildControl(Control child);
		void RemoveChildControl(Control child);
		Graphics CreateGraphics();
		void Refresh(); 
		void ShowToolTip(string s, Point point);
		void HideToolTip();
	}
	public interface IMapClickHandler {
		void OnPointClick(MapPoint point, CoordPoint coordPoint);
	}
	public interface IColorizerElement {
		Color ColorizerColor { get; set; }
	}
	public interface IKeyColorizerElement : IColorizerElement {
		object ColorItemKey { get; }
	}
	public interface IColorizerValueProvider : ISupportObjectChanged {
		double GetValue(object item);
	}
	public interface IColorizerItemKeyProvider {
		object GetItemKey(object item);
	}   
	public interface IMeasuredItemValueProvider {
		double GetValue(object item);
	}
	public interface IMapDataAdapter : ILockableObject {
		MapItemType DefaultMapItemType { get; }
		bool IsReady { get; }
		int Count { get; }
		IEnumerable<MapItem> Items { get; }
		IClusterer Clusterer { get; set; }
		void LoadData(IMapItemFactory factory);
		MapItem GetItem(int index);
		object GetItemSourceObject(MapItem item);
		MapItem GetItemBySourceObject(object sourceObject);
		void SetLayer(MapItemsLayerBase layer);
		MapItemsLayerBase GetLayer();
		event DataAdapterChangedEventHandler DataChanged;
		bool IsCSCompatibleTo(MapCoordinateSystem mapCS);
		void OnViewportUpdated(MapViewport viewport);
		void OnClustered();
		IEnumerable<MapItem> SourceItems { get; }
	}
	public interface IClusterer {
		bool IsBusy { get; }
		MapItemCollection Items { get; }
		void Clusterize(IEnumerable<MapItem> sourceItems, MapViewport viewport, bool sourceChanged);
		void SetOwner(IMapDataAdapter owner);
	}
	public interface IMapChartDataAdapter : IMapDataAdapter {
		void OnChartItemsUpdated();
	}
}
