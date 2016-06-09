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
using System.Linq;
using System.Drawing;
using DevExpress.XtraMap.Native;
using DevExpress.Utils;
using DevExpress.Map.Native;
namespace DevExpress.XtraMap.Drawing {
	public class ZoomTrackBarController {
		const double maxPosition = 120.0;
		double minZoomLevel = InnerMap.DefaultMinZoomLevel;
		double maxZoomLevel = InnerMap.DefaultMaxZoomLevel;
		double zoomLevel;
		public ZoomTrackBarController() {
		}
		public Rectangle ViewInfoClientBounds { get; set; }
		public double MapMinZoomLevel { get { return minZoomLevel; } }
		public double MapMaxZoomLevel { get { return maxZoomLevel; } }
		public double ZoomLevel { get { return zoomLevel; } }
		public double NormalizedZoomLevel { get { return CalculateNormalizedZoomLevel(ZoomLevel); } }
		double CalculateNormalizedZoomLevel(double value) {
			double delta = MapMaxZoomLevel - MapMinZoomLevel;
			return delta != 0 ? (value - MapMinZoomLevel) / delta : 0.5;
		}
		public void UpdateMinMaxZoomLevel(double minZoomLevel, double maxZoomLevel) {
			this.minZoomLevel = minZoomLevel;
			this.maxZoomLevel = maxZoomLevel;
		}
		public void InitZoomLevel(double zoomLevel) {
			this.zoomLevel = zoomLevel;
		}
		public void UpdateZoomLevel(int positionX, int maxPosition, double minZoom, double maxZoom) {
			double ratio = positionX / (double)maxPosition;
			zoomLevel =  MapMinZoomLevel + ratio * (double)(MapMaxZoomLevel - MapMinZoomLevel);
			zoomLevel = MathUtils.MinMax(zoomLevel, minZoom, maxZoom);
		}
	}
	public class ZoomTrackBarViewInfo : NonTextElementViewInfoBase {
		ZoomTrackBarController zoomTrackBarController;
		Rectangle thumbBounds;
		public int Width { get { return 140; } }
		public int ThumbHeight { get { return 20; } }
		public int ThumbWidth { get { return 7; } }
		public int ThumbPadding { get { return 2; } }
		public int LineHeight { get { return 3; } }
		public Rectangle LineBoundsLeft { get; set; }
		public Rectangle LineBoundsRight { get; set; }
		public Rectangle ThumbBounds {
			get { return thumbBounds; }
			set {
				if(thumbBounds == value)
					return;
				this.thumbBounds = value;
				OnThumbBoundsChanged();
			}
		}
		public override int DesiredWidth { get { return Width; } }
		public override int LeftMargin { get { return NavigationPanelViewInfo.ItemPadding / 2; } }
		public ZoomTrackBarViewInfo(InnerMap map, ZoomTrackBarController zoomTrackBarController)
			: base(map) {
				this.zoomTrackBarController = zoomTrackBarController;
		}
		protected override void DisposeOverride() {
			zoomTrackBarController = null;
		}
		protected override void CalcClientBounds(Graphics gr, Rectangle availableBounds) {
			ClientBounds = new Rectangle(availableBounds.X, availableBounds.Y + (availableBounds.Height - ThumbHeight) / 2, Width, ThumbHeight + ThumbPadding * 2);
			int halfThumbWidth = Convert.ToInt32(ThumbWidth / 2);
			int maxX = ClientBounds.Right - halfThumbWidth;
			int minX = ClientBounds.X - halfThumbWidth;
			int offset = ClientBounds.X + Convert.ToInt32((ClientBounds.Width - halfThumbWidth) * zoomTrackBarController.NormalizedZoomLevel);
			int zoomLevelOffsetX = Convert.ToInt32(MathUtils.MinMax(offset, minX, maxX));
			ThumbBounds = new Rectangle(zoomLevelOffsetX - halfThumbWidth, ClientBounds.Y + ThumbPadding, ThumbWidth, ThumbHeight);
			Rectangle lineRectLeft = new Rectangle(ClientBounds.X, ClientBounds.Y,
									 ThumbBounds.Left - ThumbPadding - ClientBounds.X, LineHeight);
			LineBoundsLeft = RectUtils.AlignRectangle(lineRectLeft, ClientBounds, ContentAlignment.MiddleLeft);
			Rectangle lineRectRight = new Rectangle(ThumbBounds.Right + ThumbPadding, ClientBounds.Y, ClientBounds.Right - ThumbBounds.Right - ThumbPadding, LineHeight);
			LineBoundsRight = RectUtils.AlignRectangle(lineRectRight, ClientBounds, ContentAlignment.MiddleRight);
		}
		protected override IViewinfoInteractionController CreateInteractionController() {
			return new RectangularViewinfoInteractionController();
		}
		protected override void OnClientBoundsChanged() {
			zoomTrackBarController.ViewInfoClientBounds = ClientBounds;
		}
		protected void OnThumbBoundsChanged() {
			InteractionController.Bounds = ThumbBounds;
		}
	}
	public abstract class ZoomButtonViewInfoBase : NonTextElementViewInfoBase {
		protected const int ZoomButtonsRadius = 28;
		public const float LineWidth = ZoomButtonsRadius / 9.0f;
		RectangleF[] buttonSymbolRects = new RectangleF[] { };
		public RectangleF[] ButtonSymbolRects { get { return buttonSymbolRects; } }
		public override int DesiredWidth {
			get {
				return ZoomButtonsRadius;
			}
		}
		protected ZoomButtonViewInfoBase(InnerMap map)
			: base(map) {
		}
		protected abstract RectangleF[] GetButtonSymbolRects(Rectangle zoomButtonBounds);
		protected override IViewinfoInteractionController CreateInteractionController() {
			return new RoundViewinfoInteractionController((int)LineWidth);
		}
		protected override void CalcClientBounds(Graphics gr, Rectangle availableBounds) {
			ClientBounds = new Rectangle(availableBounds.X, availableBounds.Y + (availableBounds.Height - ZoomButtonsRadius) / 2, ZoomButtonsRadius, ZoomButtonsRadius);
			InteractionController.Bounds = ClientBounds;
			buttonSymbolRects = GetButtonSymbolRects(ClientBounds);
		}
	}
	public class ZoomInBtnViewInfo : ZoomButtonViewInfoBase {
		public override int LeftMargin { get { return NavigationPanelViewInfo.ItemPadding / 2; } }
		public ZoomInBtnViewInfo(InnerMap map)
			: base(map) {
		}
		protected override RectangleF[] GetButtonSymbolRects(Rectangle zoomButtonBounds) {
			float width = ZoomButtonsRadius * 0.5f;
			float minOffset = ZoomButtonsRadius * 0.25f;
			float maxOffset = ZoomButtonsRadius * 0.5f - LineWidth * 0.5f;
			RectangleF rect = new RectangleF(zoomButtonBounds.X + minOffset, zoomButtonBounds.Y + maxOffset, width, LineWidth);
			RectangleF rect2 = new RectangleF(zoomButtonBounds.X + maxOffset, zoomButtonBounds.Y + minOffset, LineWidth, width);
			return new RectangleF[] { rect, rect2 };
		}
	}
	public class ZoomOutBtnViewInfo : ZoomButtonViewInfoBase {
		public override int LeftMargin { get { return (int)(NavigationPanelViewInfo.ItemPadding * 1.5); } }
		public ZoomOutBtnViewInfo(InnerMap map)
			: base(map) {
		}
		protected override RectangleF[] GetButtonSymbolRects(Rectangle zoomButtonBounds) {
			float left = zoomButtonBounds.X + ZoomButtonsRadius * 0.25f;
			float top = zoomButtonBounds.Y + ZoomButtonsRadius * 0.5f - LineWidth * 0.5f;
			float width = ZoomButtonsRadius * 0.5f;
			return new RectangleF[] { new RectangleF(left, top, width, LineWidth)  };
		}
	}
}
