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

using DevExpress.Utils;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
namespace DevExpress.XtraMap.Drawing {
	public class ScrollButtonsController {
		public Rectangle ViewInfoClientBounds { get; set; }
	}
	public class ScrollButtonsViewInfo : NonTextElementViewInfoBase {
		const int ArrowWidth = 12;
		const int ArrowHeight = 6;
		const int ArrowOffset = 10;
		public const double OffsetFactor = 0.5;
		public const double MaxScrollOffset = 50.0;
		public const int BackgroundLineThickness = 3;
		ScrollButtonsController scrollButtonsController;
		Point[] leftArrowPolygonPoints;
		Point[] topArrowPolygonPoints;
		Point[] rightArrowPolygonPoints;
		Point[] bottomArrowPolygonPoints;
		public Point[] LeftArrowPolygonPoints { get { return leftArrowPolygonPoints; } }
		public Point[] TopArrowPolygonPoints { get { return topArrowPolygonPoints; } }
		public Point[] RightArrowPolygonPoints { get { return rightArrowPolygonPoints; } }
		public Point[] BottomArrowPolygonPoints { get { return bottomArrowPolygonPoints; } }
		public override int DesiredWidth { get { return NavigationPanelViewInfo.ScrollButtonsRadius; } }
		public override int LeftMargin { get { return NavigationPanelViewInfo.ItemPadding; } }
		public ScrollButtonsViewInfo(InnerMap map, ScrollButtonsController scrollButtonsController)
			: base(map) {
				this.scrollButtonsController = scrollButtonsController;
		}
		protected override void DisposeOverride() {
			scrollButtonsController = null;
		}
		protected override IViewinfoInteractionController CreateInteractionController() {
			return new RoundViewinfoInteractionController(BackgroundLineThickness);
		}
		protected override void CalcClientBounds(Graphics gr, Rectangle availableBounds) {
			int radius = NavigationPanelViewInfo.ScrollButtonsRadius;
			ClientBounds = new Rectangle(availableBounds.X, availableBounds.Y + (availableBounds.Height - radius) / 2, radius, radius);
			Rectangle r = ClientBounds;
			int verticalArrowX = r.Left + (r.Width / 2) - (ArrowWidth / 2) + 1;
			int horizontalArrowY = r.Top + (r.Height / 2) - (ArrowWidth / 2);
			Rectangle topArrowRect = new Rectangle(verticalArrowX, r.Top + ArrowOffset - 1, ArrowWidth, ArrowHeight);
			topArrowPolygonPoints = new Point[] { RectUtils.GetLeftBottom(topArrowRect), RectUtils.GetRightBottom(topArrowRect), new Point(RectUtils.GetHorizontalCenter(topArrowRect), topArrowRect.Top) };
			Rectangle leftArrowRect = new Rectangle(r.Left + ArrowOffset, horizontalArrowY, ArrowHeight, ArrowWidth);
			leftArrowPolygonPoints = new Point[] { RectUtils.GetRightTop(leftArrowRect), RectUtils.GetRightBottom(leftArrowRect), new Point(leftArrowRect.Left, RectUtils.GetVerticalCenter(leftArrowRect)) };
			Rectangle rightArrowRect = new Rectangle(r.Right - ArrowOffset - ArrowHeight + 1, horizontalArrowY, ArrowHeight, ArrowWidth);
			rightArrowPolygonPoints = new Point[] { rightArrowRect.Location, RectUtils.GetLeftBottom(rightArrowRect), new Point(rightArrowRect.Right, RectUtils.GetVerticalCenter(rightArrowRect)) };
			Rectangle bottomArrowRect = new Rectangle(verticalArrowX, r.Bottom - ArrowOffset - ArrowHeight + 1, ArrowWidth, ArrowHeight);
			bottomArrowPolygonPoints = new Point[] { bottomArrowRect.Location, RectUtils.GetRightTop(bottomArrowRect), new Point(RectUtils.GetHorizontalCenter(bottomArrowRect), bottomArrowRect.Bottom) };
		}
		protected override void OnClientBoundsChanged() {
			scrollButtonsController.ViewInfoClientBounds = ClientBounds;
			InteractionController.Bounds = ClientBounds;
		}
	}
}
