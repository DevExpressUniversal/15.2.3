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

using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraMap.Drawing {
	public class SearchPanelViewInfo : OverlayViewInfoBase, IHitTestableViewinfo {
		public const int Margin = 8;
		public const int Padding = 9;
		protected override int KeyInternal { get { return 1; } }
		public override ViewInfoUpdateType SupportedUpdateType { get { return ViewInfoUpdateType.SearchPanel; } }
		public new SearchPanelPainter Painter { get { return (SearchPanelPainter)base.Painter; } }
		protected override MapStyleCollection DefaultAppearance { get { return Painter.ViewInfoAppearanceProvider.DefaultSearchPanelAppearance; } }
		public SearchPanelViewInfo(InnerMap map, SearchPanelPainter painter) : base(map, new SearchPanelAppearance(null), painter) {
		}
		#region IHitTestableViewinfo implementation
		IMapUiHitInfo IHitTestableViewinfo.CalcHitInfo(Point point) {
			Point clientHitPoint = new Point(point.X - Bounds.X, point.Y - Bounds.Y);
			if(!ClientBounds.Contains(clientHitPoint))
				return null;
			return new MapUiHitInfo(clientHitPoint, MapHitUiElementType.SearchPanel);
		}
		#endregion
		Rectangle CalculateBounds(Rectangle contentBounds) {
			int panelWidth = SearchPanel.MinWidth;
			int height = SearchPanel.MinHeight;
			Rectangle rect = new Rectangle(contentBounds.X, contentBounds.Y, panelWidth, height);
			rect.Inflate(Padding, Padding);
			Rectangle bounds = RectUtils.AlignRectangle(rect, contentBounds, ContentAlignment.TopRight);
			bounds.X -= Margin;
			bounds.Y += Margin;
			return bounds;
		}
		protected override void DisposeOverride() {
			base.DisposeOverride();
		}
		protected internal override void CalculateLayout(Rectangle controlBounds) {
			Bounds = CalculateBounds(controlBounds);
			ClientBounds = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
		}
	}
	public class SearchPanelPainter : OverlayViewInfoPainter {
		public SearchPanelPainter(IViewInfoStyleProvider provider)
			: base(provider) {
		}
		public override void Draw(GraphicsCache cache, SelfpaintingViewinfo viewInfo) {
			SearchPanelViewInfo vi = (SearchPanelViewInfo)viewInfo;
			SearchPanelAppearance paintAppearance = (SearchPanelAppearance)(viewInfo.PaintAppearance);
			BackgroundStyle bgStyle = paintAppearance.BackgroundStyle;
			DrawBackground(cache, vi, bgStyle);
		}
		protected virtual void DrawBackground(GraphicsCache cache, SearchPanelViewInfo viewInfo, BackgroundStyle bgStyle) {
			cache.FillRectangle(bgStyle.Fill, viewInfo.ClientBounds);
		}
	}
	public class GraphicsSmoothHelper : IDisposable {
		GraphicsCache cache;
		System.Drawing.Drawing2D.SmoothingMode holdMode;
		public GraphicsSmoothHelper(GraphicsCache cache, System.Drawing.Drawing2D.SmoothingMode mode) {
			this.holdMode = cache.Graphics.SmoothingMode;
			this.cache = cache;
			SetSmoothMode(mode);
		}
		protected void SetSmoothMode(System.Drawing.Drawing2D.SmoothingMode mode) {
			cache.Graphics.SmoothingMode = mode;
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				SetSmoothMode(holdMode);
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~GraphicsSmoothHelper() {
			Dispose(false);
		}
		#endregion
	}
}
