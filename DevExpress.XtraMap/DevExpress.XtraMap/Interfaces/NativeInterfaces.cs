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
using System.Drawing;
using DevExpress.Map;
using DevExpress.Utils;
using DevExpress.XtraMap.Drawing;
using DevExpress.Map.Native;
using DevExpress.Data.Svg;
namespace DevExpress.XtraMap.Native {
	public interface IUIThreadRunner {
		bool AllowInvoke { get; }
		void BeginInvoke(Action action);
		void Invoke(Action action);
	}
	public interface ITemplateGeometryProvider {
		TemplateGeometryType TemplateType { get; }
	}
	public interface IInteractiveElementProvider {
		IInteractiveElement GetInteractiveElement();
	}
	public interface IInteractiveElement {
		bool EnableHighlighting { get; }
		bool EnableSelection { get; }
		bool IsHighlighted { get; set; }
		bool IsSelected { get; set; }
	}
	public interface ISupportToolTip {
		MapItem ActiveObject { get; }
		string CalculateToolTipText();
	}
	public interface IMapControlEventsListener {
		void NotifySelectionChanged();
		void NotifySelectionChanging(MapSelectionChangingEventArgs args);
		IRenderItemStyle NotifyDrawMapItem(MapItem item);
		void NotifyExportMapItem(ExportMapItemEventArgs args);
		void NotifyMapItemClick(MapItemClickEventArgs args);
		void NotifyMapItemDoubleClick(MapItemClickEventArgs args);
		void NotifyHyperlinkClick(HyperlinkClickEventArgs args);
		void NotifyLegendItemCreating(LegendItemCreatingEventArgs e);
		void NotifyOverlaysArranged(OverlaysArrangedEventArgs args);
	}
	public interface IMapNotificationObserver {
		void HandleChanged(MapNotification notification);
	}
	public interface IMapView {
		double ZoomLevel { get; }
		double MaxZoomLevel { get; }
		CoordPoint CenterPoint { get; }
		MapViewportInternal Viewport { get; }
		bool ReadyForRender { get; }
		Size InitialMapSize { get; }
		bool AnimationInProgress { get; }
		IMapItemStyleProvider StyleProvider { get; }
		IMapEventHandler EventHandler { get; }
		MapRect RenderBounds { get; }
		MapCoordinateSystem CoordinateSystem { get; }
		CoordPoint AnchorPoint { get; }
	}
	public interface ISupportUnitConverter {
		MapUnitConverter UnitConverter { get; }
	}
	public interface ISupportSwapItems {
		void Swap(int index1, int index2);
	}
	public interface ISupportImagePainter {
		MapItemStyle ActualStyle { get; }
		ImageGeometry Geometry { get; }
	}
	public interface ISupportIndexOverlay {
		int Index { get; set; }
		int MaxIndex { get; }
	}
	public interface ISupportBoundingRectAdapter {
		DevExpress.Map.Native.CoordBounds BoundingRect { get; }
	}
	public interface IViewinfoInteractionControllerProvider {
		IViewinfoInteractionController InteractionController { get; }
	}
	public interface IViewinfoInteractionController {
		bool HotTracked { get; }
		Rectangle Bounds { get; set; }
		void RecalculateHotTracked(Point point);
	}
	public interface ISupportPressedStatePanel {
		Point CalculatePanelPoint(Point controlPoint);
		void UpdatePressedState(MapHitUiElementType hitTestType);
	}
	public interface ISupportViewinfoLayout {
		OverlayArrangement CreateOverlayLayout(ViewInfoUpdateType updateType);
		void ApplyLayout(OverlayArrangement layout);
	}
	public interface IHitTestableViewinfo {
		IMapUiHitInfo CalcHitInfo(Point point);
	}
	public interface ISupportSegments {
		MapSegmentBase[] Segments { get; }
	}
}
namespace DevExpress.XtraMap.Design {
	public interface IMapCoordSystemProvider {
		MapCoordinateSystem GetMapCoordSystem();
	}
}
