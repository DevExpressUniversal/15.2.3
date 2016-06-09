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

using DevExpress.Utils.Drawing;
namespace DevExpress.XtraMap.Drawing {
	public class MapUIElementsPainter {
		NavigationPanelPainter navPanelPainter;
		SizeLegendPainter sizeLegendPainter;
		ColorLegendPainter colorLegendPainter;
		ErrorPanelPainter errorPanelPainter;
		SearchPanelPainter searchPanelPainter;
		CustomOverlayPainter customOverlayPainter;
		IViewInfoStyleProvider styleProvider;
		OverlayTextItemPainter overlayTextItemPainter;
		OverlayImageItemPainter overlayImageItemPainter;
		protected IViewInfoStyleProvider StyleProvider { get { return styleProvider; } }
		public IViewInfoStyleProvider ViewInfoStyleProvider { get { return styleProvider; } }
		public NavigationPanelPainter NavigationPanelPainter { get { return navPanelPainter; } }
		public SizeLegendPainter SizeLegendPainter { get { return sizeLegendPainter; } }
		public ColorLegendPainter ColorLegendPainter { get { return colorLegendPainter; } }
		public ErrorPanelPainter ErrorPanelPainter { get { return errorPanelPainter; } }
		public SearchPanelPainter SearchPanelPainter { get { return searchPanelPainter; } }
		public CustomOverlayPainter CustomOverlayPainter { get { return customOverlayPainter; } }
		public OverlayTextItemPainter OverlayTextItemPainter { get { return overlayTextItemPainter; } }
		public OverlayImageItemPainter OverlayImageItemPainter { get { return overlayImageItemPainter; } }
		public MapUIElementsPainter(IViewInfoStyleProvider provider) {
			this.styleProvider = provider;
			UpdatePainters();
		}
		protected void UpdatePainters() {
			this.navPanelPainter = CreateNavPanelPainter();
			this.sizeLegendPainter = CreateSizeLegendPainter();
			this.colorLegendPainter = CreateColorLegendPainter();
			this.errorPanelPainter = CreateErrorPanelPainter();
			this.searchPanelPainter = CreateSearchPanelPainter();
			this.customOverlayPainter = CreateCustomOverlayPainter();
			this.overlayTextItemPainter = CreateOverlayTextItemPainter();
			this.overlayImageItemPainter = CreateOverlayImageItemPainter();
		}
		protected virtual NavigationPanelPainter CreateNavPanelPainter() {
			return new NavigationPanelPainter(styleProvider);
		}
		protected virtual ErrorPanelPainter CreateErrorPanelPainter() {
			return new ErrorPanelPainter(styleProvider);
		}
		protected virtual SearchPanelPainter CreateSearchPanelPainter() {
			return new SearchPanelPainter(styleProvider);
		}
		protected virtual SizeLegendPainter CreateSizeLegendPainter() {
			return new SizeLegendPainter(styleProvider);
		}
		protected virtual ColorLegendPainter CreateColorLegendPainter() {
			return new ColorLegendPainter(styleProvider);
		}
		protected virtual CustomOverlayPainter CreateCustomOverlayPainter() {
			return new CustomOverlayPainter(styleProvider);
		}
		protected virtual OverlayImageItemPainter CreateOverlayImageItemPainter() {
			return new OverlayImageItemPainter(styleProvider);
		}
		protected virtual OverlayTextItemPainter CreateOverlayTextItemPainter() {
			return new OverlayTextItemPainter(styleProvider);
		}
		public void Draw(GraphicsCache cache, OverlayViewInfoBase viewInfo) {
			OverlayViewInfoPainter painter = viewInfo.Painter;
			painter.Draw(cache, viewInfo);
		}
	}
	public abstract class OverlayViewInfoPainter {
		IViewInfoStyleProvider viewInfoAppearanceProvider;
		public IViewInfoStyleProvider ViewInfoAppearanceProvider { get { return viewInfoAppearanceProvider; } }
		protected OverlayViewInfoPainter(IViewInfoStyleProvider provider) {
			this.viewInfoAppearanceProvider = provider;
		}
		public abstract void Draw(GraphicsCache cache, SelfpaintingViewinfo viewInfo);
	}
}
