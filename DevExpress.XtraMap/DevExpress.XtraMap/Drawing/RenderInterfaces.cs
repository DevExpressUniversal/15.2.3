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

using DevExpress.Map;
using DevExpress.Skins;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.XtraMap.Drawing {
	public enum UnitGeometryType {
		Areal,
		Linear
	}
	public enum ImageCachePriority {
		Normal,
		Hight,
		Low
	}
	public interface IGeometry {
	   IList<MapPoint[]> Points { get; }
	}
	public interface IMapItemGeometry {
		MapRect Bounds { get; }
		IList<MapUnit> GetPoints();
	}
	public interface IImageGeometry : IMapItemGeometry, IDisposable {
		Image Image { get; }
		RectangleF ImageRect { get; }
		RectangleF ClipRect { get; }
		bool StoringInPool { get; }
		byte Transparency { get; }
		bool AlignImage { get; }
		ImageCachePriority CachePriority { get; }
	}
	public interface IUnitGeometry : IMapItemGeometry {
		MapUnit[] Points { get; }
		UnitGeometryType Type { get; }
	}
	public interface IImageContainer {
		void UpdateImage(object imageList);
	}
	public interface IRenderShapeTitle {
		bool Visible { get; }
		bool UseAntiAliasing { get; }
		IImageGeometry Geometry { get; }
	}
	public interface IRenderItem {
		IRenderShapeTitle Title { get; }
		IMapItemGeometry Geometry { get; }
		IRenderItemStyle Style { get; }
		IRenderItemResourceHolder ResourceHolder { get; }
		void SetResourceHolder(IRenderer renderer, IRenderItemProvider provider);
		bool ForceUpdateResourceHolder { get; set; }
		object UpdateLocker { get; }
		bool Visible { get; }
		bool UseAntiAliasing { get; }
		void OnRender();
		bool CanExport();
		void PrepareGeometry();
	}
	public interface IRenderItemContainer {
		IEnumerable<IRenderItem> Items { get; }
		bool RenderItself { get; }
	}
	public interface ILocatableRenderItem {
		MapUnit Location { get; }
		Size SizeInPixels { get; }
		MapPoint Origin { get; }
		MapPoint StretchFactor { get; }
		void ResetLocation();
	}
	public interface IRenderItemResourceHolder : IDisposable {
		void Update();
	}
	public interface IRenderItemStyle {
		Color Fill { get; }
		Color Stroke { get; }
		int StrokeWidth { get; }
	}
	public interface IPointRenderItemStyle : IRenderItemStyle {
		Image Image { get; }
		string Text { get; }
		byte Transparency { get; }
	}
	public interface IShapeRenderItemStyle : IRenderItemStyle {
		Color TitleColor { get; }
		Color TitleGlowColor { get; }
		string ActualTitle { get; }
	}
	public interface ICompositeRenderItemStyle : IShapeRenderItemStyle {
		IRenderItemStyle[] Parts { get; }
	}
	public interface IRenderContextProvider {
		IRenderContext RenderContext { get; }
	}
	public interface IRenderItemProvider {
		MapPoint RenderOffset { get; }
		Rectangle RenderClipBounds { get; }
		double RenderScale { get; }
		double RenderScaleFactorX { get; }
		double RenderScaleFactorY { get; }
		bool IsReady { get; }
		IEnumerable<IRenderItem> RenderItems { get; }
		void PrepareData();
		MapColorizer GetColorizer(IRenderItem item);
		MapPoint[] GeometryToScreenPoints(MapUnit[] geometry);
		void UpdateRenderParameters(IRenderContext renderContext);
		void OnRenderComplete();
	}
	public interface IRenderContext {
		Rectangle ClipBounds { get; }
		Rectangle ContentBounds { get; }
		Rectangle Bounds { get; }
		CoordPoint CenterPoint { get; }
		double ZoomLevel { get; }
		Color BackColor { get; }
		bool IsExport { get; set; }
	}
	public interface IRenderOverlay : IDisposable {
		Image OverlayImage { get; set; }
		Size OverlayImageSize { get; set; }
		Point ScreenPosition { get; set; }
		bool StoringInPool { get; set; }
		bool Printable { get; }
		bool ShowInDesign { get; }
	}
	public interface IRenderOverlayProvider {
		IEnumerable<IRenderOverlay> GetOverlays();
	}
	public interface IRenderMiniMapProvider {
		IRenderMiniMapContentProvider ContentProvider { get; }
		IRenderStyleProvider StyleProvider { get; }
	}
	public interface IRenderMiniMapContentProvider {
		IRenderContext RenderContext { get; }
		Rectangle ViewportInPixels { get; }
		IList<LayerBase> Layers { get; }
	}
	public interface IRenderStyleProvider {
		Rectangle Bounds { get; }
		BorderedElementStyle BorderedElementStyle { get; }
		bool IsSkinActive { get; }
		ISkinProvider SkinProvider { get; }
		BorderStyles BorderStyle { get; }
		SkinElement BorderElement { get; }
	}
	public interface IBoundingRectUpdater {
		bool NeedUpdateBoundingRect { get; }
		void EnsureBoundingRect(ICollection<LayerBase> layers);
	}
	public interface IRenderer {
		bool Initialize(object context, Size size);
		void StartUpdateItems();
		void UpdateItems(IRenderItemProvider provider);
		void EndUpdateItems();
		void StartRender(IRenderContext renderContext);
		void Render(IRenderItemProvider provider);
		void RenderBorder(Graphics gr, IRenderStyleProvider provider);
		void RenderBorderFinally(Graphics gr, IRenderStyleProvider provider);
		void BeforeRenderOverlay();
		void RenderOverlays(IRenderOverlayProvider provider);
		void DrawRectangle(Rectangle rect, Color fill, Color stroke);
		void EndRender();
		void DisposeResources();
		IRenderItemResourceHolder CreateResourceHolder(IRenderItemProvider provider, IRenderItem owner);
	}
	public interface ITemplateGeometryItem {
		TemplateGeometryType Type { get; }
		double StretchFactor { get; }
	}
}
