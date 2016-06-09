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
using DevExpress.Map.Native;
using DevExpress.XtraMap.Native;
using System;
using System.Drawing;
namespace DevExpress.XtraMap.Drawing {
	public class NavigationPanelViewInfo : OverlayViewInfoBase, ISupportPressedStatePanel, IHitTestableViewinfo {
		public const int ItemPadding = 16;
		public const int ScrollButtonsRadius = 56;
		Size clientSize;
		bool showZoomElements = true;
		bool showScrollButtons = true;
		bool showScale = true;
		bool showCoordinates = true;
		double kilometersScale;
		ZoomTrackBarViewInfo zoomTrackBarViewInfo;
		ZoomInBtnViewInfo zoomInBtnViewInfo;
		ZoomOutBtnViewInfo zoomOutBtnViewInfo;
		ScaleViewInfo scaleViewInfo;
		ScrollButtonsViewInfo scrollButtonsViewInfo;
		CoordPointType pointsType;
		CoordinateViewInfo xCoordinateViewInfo;
		CoordinateViewInfo yCoordinateViewInfo;
		protected override int KeyInternal { get { return 0; } }
		public override ViewInfoUpdateType SupportedUpdateType { get { return ViewInfoUpdateType.NavigationPanel; } }
		public override bool ShowInDesign { get { return true; } }
		public new NavigationPanelPainter Painter { get { return (NavigationPanelPainter)base.Painter; } }
		protected override MapStyleCollection DefaultAppearance { get { return Painter.ViewInfoAppearanceProvider.DefaultNavigationPanelAppearance; } }
		protected override MapStyleCollection UserAppearance { get { return Map.NavigationPanelOptions.Appearance; } }
		public ZoomTrackBarViewInfo ZoomTrackBarViewInfo { get { return zoomTrackBarViewInfo; } }
		public ZoomInBtnViewInfo ZoomInBtnViewInfo { get { return zoomInBtnViewInfo; } }
		public ZoomOutBtnViewInfo ZoomOutBtnViewInfo { get { return zoomOutBtnViewInfo; } }
		public ScrollButtonsViewInfo ScrollButtonsViewInfo { get { return scrollButtonsViewInfo; } }
		public CoordinateViewInfo XCoordinateViewInfo { get { return xCoordinateViewInfo; } }
		public CoordinateViewInfo YCoordinateViewInfo { get { return yCoordinateViewInfo; } }
		public ScaleViewInfo ScaleViewInfo { get { return scaleViewInfo; } }
		public bool ShowZoomElements { get { return showZoomElements; } }
		public bool ShowScrollButtons { get { return showScrollButtons; } }
		public bool ShowScale { get { return showScale; } }
		public bool ShowCoordinates { get { return showCoordinates; } }
		public Point MouseClientPosition { get; set; }
		public override bool Printable { get { return true; } }
		public Size ClientSize {
			get { return clientSize; }
			set {
				if(clientSize == value)
					return;
				clientSize = value;
			}
		}
		public double KilometersScale {
			get {
				return kilometersScale;
			}
			set {
				if(kilometersScale == value)
					return;
				kilometersScale = value;
			}
		}
		public NavigationPanelViewInfo(InnerMap map, ZoomTrackBarController zoomTrackBarController, ScrollButtonsController scrollButtonsController, NavigationPanelPainter painter)
			: base(map, new NavigationPanelAppearance(null), painter) {
			zoomTrackBarViewInfo = new ZoomTrackBarViewInfo(Map, zoomTrackBarController);
			scrollButtonsViewInfo = new ScrollButtonsViewInfo(Map, scrollButtonsController);
			zoomInBtnViewInfo = new ZoomInBtnViewInfo(Map);
			zoomOutBtnViewInfo = new ZoomOutBtnViewInfo(Map);
			scaleViewInfo = new ScaleViewInfo(Map, ((NavigationPanelAppearance)PaintAppearance).ScaleStyle);
			CreateCoordinatesViewInfo();
		}
		#region ISupportPressedStatePanel implementation
		Point ISupportPressedStatePanel.CalculatePanelPoint(Point controlPoint) {
			return Map.RenderController.TranslateToNavPanelPosition(controlPoint);
		}
		void ISupportPressedStatePanel.UpdatePressedState(MapHitUiElementType hitTestType) {
			bool scrollButtonsPressed = hitTestType == MapHitUiElementType.ScrollButtons;
			scrollButtonsViewInfo.Pressed = scrollButtonsPressed;
			bool zoomTrackBarThumbPressed = hitTestType == MapHitUiElementType.ZoomTrackBarThumb;
			zoomTrackBarViewInfo.Pressed = zoomTrackBarThumbPressed;
			bool zoomInBtnPressed = hitTestType == MapHitUiElementType.ZoomIn;
			zoomInBtnViewInfo.Pressed = zoomInBtnPressed;
			bool zoomOutBtnPressed = hitTestType == MapHitUiElementType.ZoomOut;
			zoomOutBtnViewInfo.Pressed = zoomOutBtnPressed;
		}
		#endregion
		#region IHitTestableViewinfo implementation
		IMapUiHitInfo IHitTestableViewinfo.CalcHitInfo(Point point) {
			Point clientHitPoint = new Point(point.X, point.Y - Bounds.Y);
			if(!ClientBounds.Contains(clientHitPoint))
				return null;
			if(ZoomTrackBarViewInfo.ThumbBounds.Contains(clientHitPoint))
				return CreateUiHitTestInfo(clientHitPoint, MapHitUiElementType.ZoomTrackBarThumb);
			if(ZoomTrackBarViewInfo.ClientBounds.Contains(clientHitPoint))
				return CreateUiHitTestInfo(clientHitPoint, MapHitUiElementType.ZoomTrackBar);
			if(zoomInBtnViewInfo.HotTracked)
				return CreateUiHitTestInfo(clientHitPoint, MapHitUiElementType.ZoomIn);
			else if(zoomOutBtnViewInfo.HotTracked)
				return CreateUiHitTestInfo(clientHitPoint, MapHitUiElementType.ZoomOut);
			else if(scrollButtonsViewInfo.HotTracked)
				return CreateUiHitTestInfo(clientHitPoint, MapHitUiElementType.ScrollButtons);
			return CreateUiHitTestInfo(clientHitPoint, MapHitUiElementType.NavigationPanel);
		}
		#endregion
		void CreateCoordinatesViewInfo() {
			this.pointsType = Map.CoordinateSystem.PointType;
			if (pointsType != CoordPointType.Cartesian) {
				this.xCoordinateViewInfo = new LongitudeViewInfo(Map, Map.NavigationPanelOptions.XCoordinatePattern);
				this.yCoordinateViewInfo = new LatitudeViewInfo(Map, Map.NavigationPanelOptions.YCoordinatePattern);
			} else {
				this.xCoordinateViewInfo = new CartesianCoordinateViewInfo(Map, Map.NavigationPanelOptions.XCoordinatePattern);
				this.yCoordinateViewInfo = new CartesianCoordinateViewInfo(Map, Map.NavigationPanelOptions.YCoordinatePattern);
			}
		}
		protected override void DisposeOverride() {
			base.DisposeOverride();
			if(zoomTrackBarViewInfo != null) {
				zoomTrackBarViewInfo = null;
			}
			if(xCoordinateViewInfo != null)
				xCoordinateViewInfo = null;
			if(yCoordinateViewInfo != null)
				yCoordinateViewInfo = null;
			if(zoomInBtnViewInfo != null)
				zoomInBtnViewInfo = null;
			if(zoomOutBtnViewInfo != null)
				zoomOutBtnViewInfo = null;
			if(scaleViewInfo != null)
				scaleViewInfo = null;
			if(scrollButtonsViewInfo != null)
				scrollButtonsViewInfo = null;
		}
		void UpdateItemsVisibility(Graphics gr, TextElementStyle coordinatesStyle) {
			int scrollButtonsWidth = GetElementSpace(scrollButtonsViewInfo);
			int zoomContentWidth = GetElementSpace(zoomInBtnViewInfo, zoomOutBtnViewInfo, zoomTrackBarViewInfo);
			int coordinatesWidth = (MapUtils.CalcStringPixelSize(gr, string.Format("{0} {1}", YCoordinateViewInfo.Text, XCoordinateViewInfo.Text), coordinatesStyle.Font).Width) + ItemPadding * 2;
			int scaleInfoWidth = GetElementSpace(scaleViewInfo);
			int availableWidth = ClientBounds.Width;
			showCoordinates = Map.OperationHelper.CanShowCoordinates();
			if(showCoordinates) {
				availableWidth -= coordinatesWidth;
				showCoordinates = IsEnoughSpace(availableWidth);
			}
			showZoomElements = Map.OperationHelper.CanShowZoomTrackbar();
			if(showZoomElements) {
				availableWidth -= zoomContentWidth;
				showZoomElements = IsEnoughSpace(availableWidth);
			}
			showScrollButtons = Map.OperationHelper.CanShowScrollButtons();
			if(showScrollButtons) {
				availableWidth -= scrollButtonsWidth;
				showScrollButtons = IsEnoughSpace(availableWidth);
			}
			showScale = CalculateActualScaleViewType() != ScaleViewType.None;
			if(showScale) {
				availableWidth -= scaleInfoWidth;
				showScale = IsEnoughSpace(availableWidth);
			}
		}
		bool IsEnoughSpace(int availableWidth) {
			return availableWidth > 0;
		}
		int GetElementSpace(params PanelElementViewInfoBase[] subElements) {
			int totalSpace = 0;
			foreach(PanelElementViewInfoBase subElement in subElements)
				totalSpace += subElement.LeftMargin + subElement.DesiredWidth + subElement.RightMargin;
			return totalSpace;
		}
		protected internal override void CalculateOverlay(Graphics gr, Rectangle controlBounds) {
			MouseClientPosition = Map.RenderController.TranslateToNavPanelPosition(MousePosition);
			TextElementStyle coordinatesStyle = ((NavigationPanelAppearance)PaintAppearance).CoordinatesStyle;
			TextElementStyle scaleStyle = ((NavigationPanelAppearance)PaintAppearance).ScaleStyle;
			Size size = new Size(ClientBounds.Width, ScrollButtonsRadius); 
			Rectangle contentRect = new Rectangle(ClientBounds.X + ItemPadding * 2, ClientBounds.Y, ClientBounds.Width, size.Height);
			Rectangle availableRect = RectUtils.AlignRectangle(contentRect, ClientBounds, ContentAlignment.MiddleLeft);
			UpdateItemsVisibility(gr, coordinatesStyle);
			if(showScrollButtons)
				availableRect = ArrangeElementAtLeftSide(gr, null, availableRect, scrollButtonsViewInfo);
			if(showZoomElements) {
				availableRect = ArrangeElementAtLeftSide(gr, null, availableRect, zoomOutBtnViewInfo);
				availableRect = ArrangeElementAtLeftSide(gr, null, availableRect, zoomTrackBarViewInfo);
				availableRect = ArrangeElementAtLeftSide(gr, null, availableRect, zoomInBtnViewInfo);
			}
			if(showScale) {
				scaleViewInfo.ViewType = CalculateActualScaleViewType();
				availableRect = ArrangeElementAtRightSide(gr, scaleStyle, availableRect, scaleViewInfo);
			}
			if(showCoordinates) {
				if (pointsType != CoordPointType.Cartesian)
					availableRect = ArrangeCoordinates(gr, coordinatesStyle, availableRect, yCoordinateViewInfo, xCoordinateViewInfo);
				else
					availableRect = ArrangeCoordinates(gr, coordinatesStyle, availableRect, xCoordinateViewInfo, yCoordinateViewInfo);
			}
		}
		Rectangle ArrangeCoordinates(Graphics gr, TextElementStyle coordinatesStyle, Rectangle availableRect, CoordinateViewInfo first, CoordinateViewInfo second) {
			availableRect = ArrangeElementAtRightSide(gr, coordinatesStyle, availableRect, second);
			availableRect = ArrangeElementAtRightSide(gr, coordinatesStyle, availableRect, first);
			return availableRect;
		}
		Rectangle ArrangeElementAtLeftSide(Graphics gr, TextElementStyle textStyle, Rectangle availableRect, PanelElementViewInfoBase viewInfo) {
			Rectangle result = RectUtils.CutFromLeft(availableRect, viewInfo.LeftMargin);
			TextElementViewInfoBase textViewInfo = viewInfo as TextElementViewInfoBase;
			if(textViewInfo != null)
				textViewInfo.SetTextStyle(textStyle);
			viewInfo.Calculate(gr, result, MouseClientPosition);
			return RectUtils.CutFromLeft(result, viewInfo.ClientBounds.Width + viewInfo.RightMargin);
		}
		Rectangle ArrangeElementAtRightSide(Graphics gr, TextElementStyle textStyle, Rectangle availableRect, PanelElementViewInfoBase viewInfo) {
			Rectangle result = RectUtils.CutFromRight(availableRect, viewInfo.RightMargin);
			TextElementViewInfoBase textViewInfo = viewInfo as TextElementViewInfoBase;
			if(textViewInfo != null)
				textViewInfo.SetTextStyle(textStyle);
			viewInfo.Calculate(gr, result, MouseClientPosition);
			return RectUtils.CutFromRight(result, viewInfo.ClientBounds.Width + viewInfo.LeftMargin);
		}
		IMapUiHitInfo CreateUiHitTestInfo(Point hitPoint, MapHitUiElementType hitTest) {
			return new MapUiHitInfo(hitPoint, hitTest);
		}
		internal ScaleViewType CalculateActualScaleViewType() {
			bool showMiles = Map.OperationHelper.CanShowMilesScale();
			bool showKilometers = Map.OperationHelper.CanShowKilometersScale();
			if(showMiles && showKilometers)
				return ScaleViewType.All;
			if(showKilometers)
				return ScaleViewType.Meters;
			if(showMiles)
				return ScaleViewType.Miles;
			return ScaleViewType.None;
		}
		public override void SetCoordData(CoordPoint activePoint, double kilometersScale) {
			XCoordinateViewInfo.Coordinate = activePoint.GetX();
			YCoordinateViewInfo.Coordinate = activePoint.GetY();
			scaleViewInfo.KilometersScale = kilometersScale;
		}
		protected internal override void CalculateLayout(Rectangle controlBounds) {
			Size clientSize = new Size(controlBounds.Width, Map.NavigationPanelOptions.Height);
			ClientBounds = new Rectangle(Point.Empty, clientSize);
			Bounds = RectUtils.AlignRectangle(ClientBounds, controlBounds, ContentAlignment.BottomLeft);
		}
	}
	public abstract class PanelElementViewInfoBase : MapViewInfoBase {
		public virtual int DesiredWidth { get { return 0; } }
		public virtual int LeftMargin { get { return 0; } }
		public virtual int RightMargin { get { return 0; } }
		protected PanelElementViewInfoBase(InnerMap map)
			: base(map) {
		}
		public abstract void Calculate(Graphics gr, Rectangle availableBounds, Point mouseClientPosition);
	}
	public abstract class TextElementViewInfoBase : PanelElementViewInfoBase {
		TextElementStyle textStyle;
		protected TextElementViewInfoBase(InnerMap map)
			: base(map) {
		}
		public void SetTextStyle(TextElementStyle textStyle) {
			this.textStyle = textStyle;
		}
		public override void Calculate(Graphics gr, Rectangle availableBounds, Point mouseClientPosition) {
			CalcClientBounds(gr, textStyle, availableBounds);
		}
		protected abstract void CalcClientBounds(Graphics gr, TextElementStyle textStyle, Rectangle availableBounds);
	}
	public abstract class NonTextElementViewInfoBase : PanelElementViewInfoBase, IViewinfoInteractionControllerProvider {
		readonly IViewinfoInteractionController interactionController;
		protected IViewinfoInteractionController InteractionController { get { return interactionController; } }
		public bool HotTracked { get { return interactionController.HotTracked; } }
		public bool Pressed { get; set; }
		protected NonTextElementViewInfoBase(InnerMap map)
			: base(map) {
			this.interactionController = CreateInteractionController();
		}
		#region IViewinfoInteractionControllerProvider implementation
		IViewinfoInteractionController IViewinfoInteractionControllerProvider.InteractionController { get { return interactionController; } }
		#endregion
		protected abstract IViewinfoInteractionController CreateInteractionController();
		protected abstract void CalcClientBounds(Graphics gr, Rectangle availableBounds);
		public override void Calculate(Graphics gr, Rectangle availableBounds, Point mouseClientPosition) {
			CalcClientBounds(gr, availableBounds);
			interactionController.RecalculateHotTracked(mouseClientPosition);
		}
	}
}
