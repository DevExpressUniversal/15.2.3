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

using System.Collections.Generic;
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public abstract class BaseDockGuide {
		protected internal Rectangle DockingRect;
		DockGuide guideCore;
		protected BaseDockGuide(DockGuide guide) {
			guideCore = guide;
			Hints = new Dictionary<DockHint, Rectangle>();
			ArrangeHints(Hints);
		}
		public abstract Size Size { get; }
		public DockGuide Type {
			get { return guideCore; }
		}
		public abstract ContentAlignment Alignment { get; }
		protected abstract void ArrangeHints(IDictionary<DockHint, Rectangle> hints);
		public void Calc(DockGuidesConfiguration configuration, Rectangle element, Rectangle container, Point hitPoint) {
			dockCore = Rectangle.Empty;
			Configuration = configuration;
			hotHintCore = DockHint.None;
			boundsCore = ArrangeCore(element, container);
			if(boundsCore.Contains(hitPoint)) {
				if(HitTest(hitPoint, out hotHintCore)) {
					dockCore = ArrangeDockZoneCore(element, container, HotHint);
				}
			}
		}
		protected internal void EmulateHotTrack(Rectangle element, Rectangle container, DockHint hint) {
			hotHintCore = hint;
			dockCore = ArrangeDockZoneCore(element, container, hint);
		}
		protected virtual Rectangle ArrangeCore(Rectangle element, Rectangle container) {
			return DevExpress.Utils.PlacementHelper.Arrange(Size, element, Alignment);
		}
		protected virtual Rectangle ArrangeDockZoneCore(Rectangle element, Rectangle container, DockHint hint) {
			Size size = DockHintExtension.GetDockZoneSize(element.Size, hint);
			return DevExpress.Utils.PlacementHelper.Arrange(size, element, DockHintExtension.ToAlignment(hint));
		}
		protected IDictionary<DockHint, Rectangle> Hints;
		DockGuidesConfiguration Configuration;
		protected internal void ExcludeHints(GraphicsCache cache) {
			foreach(KeyValuePair<DockHint, Rectangle> pair in Hints) {
				if(!Configuration.IsEnabled(pair.Key)) {
					Rectangle clip = pair.Value;
					clip.Offset(Bounds.Location);
					cache.ClipInfo.ExcludeClip(clip);
				}
			}
		}
		protected bool HitTest(Point point, out DockHint result) {
			result = DockHint.None;
			point.Offset(-Bounds.Left, -Bounds.Top);
			foreach(KeyValuePair<DockHint, Rectangle> pair in Hints) {
				if(pair.Value.Contains(point) && Configuration.IsEnabled(pair.Key)) {
					result = pair.Key;
				}
			}
			return (result != DockHint.None);
		}
		Rectangle boundsCore;
		public Rectangle Bounds {
			get { return boundsCore; }
		}
		Rectangle dockCore;
		public Rectangle DockZone {
			get { return dockCore; }
		}
		public bool IsHot {
			get { return hotHintCore != DockHint.None; }
		}
		DockHint hotHintCore;
		public DockHint HotHint {
			get { return hotHintCore; }
		}
		public Rectangle GetHint(DockHint hint) {
			Rectangle result = Rectangle.Empty;
			if(Hints.TryGetValue(hint, out result)) {
				result.Offset(Bounds.Location);
			}
			return result;
		}
		protected int ScaleOffset(int offset) {
			var factor = DevExpress.Skins.DpiProvider.Default.DpiScaleFactor;
			return Round((float)offset * factor);
		}
		protected Size ScaleSize(Size size) {
			var factor = DevExpress.Skins.DpiProvider.Default.DpiScaleFactor;
			return new Size(Round((float)size.Width * factor), Round((float)size.Height * factor));
		}
		protected Rectangle ScaleRect(Rectangle rect) {
			var factor = DevExpress.Skins.DpiProvider.Default.DpiScaleFactor;
			return new Rectangle(Round((float)rect.X * factor), Round((float)rect.Y * factor), Round((float)rect.Width * factor), Round((float)rect.Height * factor));
		}
		static int Round(float value) {
			return value > 0 ? (int)(value + 0.5f) : (int)(value - 0.5f);
		}
	}
	class CenterDockGuide : BaseDockGuide {
		public CenterDockGuide()
			: base(DockGuide.Center) {
		}
		public override ContentAlignment Alignment {
			get { return ContentAlignment.MiddleCenter; }
		}
		public override Size Size {
			get { return ScaleSize(new Size(112, 112)); }
		}
		protected override void ArrangeHints(IDictionary<DockHint, Rectangle> hints) {
			hints.Add(DockHint.Center, ScaleRect(new Rectangle(40, 40, 32, 32)));
			hints.Add(DockHint.CenterLeft, ScaleRect(new Rectangle(4, 40, 32, 32)));
			hints.Add(DockHint.CenterTop, ScaleRect(new Rectangle(40, 4, 32, 32)));
			hints.Add(DockHint.CenterRight, ScaleRect(new Rectangle(76, 40, 32, 32)));
			hints.Add(DockHint.CenterBottom, ScaleRect(new Rectangle(40, 76, 32, 32)));
		}
	}
	class CenterDockDockGuide : BaseDockGuide {
		public CenterDockDockGuide()
			: base(DockGuide.CenterDock) {
		}
		public override Size Size {
			get { return ScaleSize(new Size(182, 182)); }
		}
		public override ContentAlignment Alignment {
			get { return ContentAlignment.MiddleCenter; }
		}
		protected override void ArrangeHints(IDictionary<DockHint, Rectangle> hints) {
			hints.Add(DockHint.Center, ScaleRect(new Rectangle(75, 75, 32, 32)));
			hints.Add(DockHint.CenterLeft, ScaleRect(new Rectangle(39, 75, 32, 32)));
			hints.Add(DockHint.CenterTop, ScaleRect(new Rectangle(75, 39, 32, 32)));
			hints.Add(DockHint.CenterRight, ScaleRect(new Rectangle(111, 75, 32, 32)));
			hints.Add(DockHint.CenterBottom, ScaleRect(new Rectangle(75, 111, 32, 32)));
			hints.Add(DockHint.CenterSideLeft, ScaleRect(new Rectangle(4, 75, 32, 32)));
			hints.Add(DockHint.CenterSideTop, ScaleRect(new Rectangle(75, 4, 32, 32)));
			hints.Add(DockHint.CenterSideRight, ScaleRect(new Rectangle(146, 75, 32, 32)));
			hints.Add(DockHint.CenterSideBottom, ScaleRect(new Rectangle(75, 146, 32, 32)));
		}
		protected override Rectangle ArrangeDockZoneCore(Rectangle element, Rectangle container, DockHint hint) {
			bool isCenterSide = DockHintExtension.IsCenterSide(hint);
			Size size = DockHintExtension.GetDockZoneSize(isCenterSide ? container.Size : element.Size, hint);
			return DevExpress.Utils.PlacementHelper.Arrange(size, isCenterSide ? container : element, DockHintExtension.ToAlignment(hint));
		}
	}
	class SideDockGuide : BaseDockGuide {
		public SideDockGuide(DockGuide guide)
			: base(guide) {
			AssertionException.IsFalse(guide == DockGuide.Center || guide == DockGuide.CenterDock);
		}
		protected override Rectangle ArrangeCore(Rectangle element, Rectangle container) {
			return DevExpress.Utils.PlacementHelper.Arrange(Size, Rectangle.Inflate(DockingRect, ScaleOffset(-6), ScaleOffset(-6)), Alignment);
		}
		protected override Rectangle ArrangeDockZoneCore(Rectangle element, Rectangle container, DockHint hint) {
			Size size = DockHintExtension.GetDockZoneSize(DockingRect.Size, hint);
			return DevExpress.Utils.PlacementHelper.Arrange(size, DockingRect, DockHintExtension.ToAlignment(hint));
		}
		protected override void ArrangeHints(IDictionary<DockHint, Rectangle> hints) {
			hints.Add(DockGuideExtension.ToSideDockHint(Type), ScaleRect(new Rectangle(4, 4, 32, 32)));
		}
		public override ContentAlignment Alignment {
			get { return DockGuideExtension.ToAlignment(Type); }
		}
		public override Size Size {
			get { return ScaleSize(new Size(40, 40)); }
		}
	}
	class SnapDockGuide : BaseDockGuide {
		public SnapDockGuide(DockGuide guide)
			: base(guide) {
			AssertionException.IsFalse(guide == DockGuide.Center || guide == DockGuide.CenterDock);
			AssertionException.IsFalse(guide == DockGuide.Left || guide == DockGuide.Top);
			AssertionException.IsFalse(guide == DockGuide.Right || guide == DockGuide.Bottom);
		}
		protected override Rectangle ArrangeCore(Rectangle element, Rectangle container) {
			Size guideSize = DockHintExtension.GetSnapGuideSize(Size, DockingRect.Size, Type);
			Hints[DockGuideExtension.ToSnapDockHint(Type)] = new Rectangle(Point.Empty, guideSize);
			return DevExpress.Utils.PlacementHelper.Arrange(guideSize, DockingRect, Alignment);
		}
		protected override Rectangle ArrangeDockZoneCore(Rectangle element, Rectangle container, DockHint hint) {
			Size size = DockHintExtension.GetDockZoneSize(DockingRect.Size, hint);
			return DevExpress.Utils.PlacementHelper.Arrange(size, DockingRect, DockHintExtension.ToAlignment(hint));
		}
		protected override void ArrangeHints(IDictionary<DockHint, Rectangle> hints) {
			hints.Add(DockGuideExtension.ToSnapDockHint(Type), Rectangle.Empty);
		}
		public override ContentAlignment Alignment {
			get { return DockGuideExtension.ToAlignment(Type); }
		}
		public override Size Size {
			get { return ScaleSize(new Size(25, 25)); }
		}
	}
	class DockGuideCache {
		IDictionary<DockGuide, BaseDockGuide> guides;
		public DockGuideCache() {
			guides = new Dictionary<DockGuide, BaseDockGuide>();
			guides.Add(DockGuide.Center, new CenterDockGuide());
			guides.Add(DockGuide.CenterDock, new CenterDockDockGuide());
			guides.Add(DockGuide.Left, new SideDockGuide(DockGuide.Left));
			guides.Add(DockGuide.Top, new SideDockGuide(DockGuide.Top));
			guides.Add(DockGuide.Right, new SideDockGuide(DockGuide.Right));
			guides.Add(DockGuide.Bottom, new SideDockGuide(DockGuide.Bottom));
			guides.Add(DockGuide.SnapLeft, new SnapDockGuide(DockGuide.SnapLeft));
			guides.Add(DockGuide.SnapTop, new SnapDockGuide(DockGuide.SnapTop));
			guides.Add(DockGuide.SnapRight, new SnapDockGuide(DockGuide.SnapRight));
			guides.Add(DockGuide.SnapBottom, new SnapDockGuide(DockGuide.SnapBottom));
		}
		public BaseDockGuide GetGuide(DockGuide guide) {
			BaseDockGuide hint;
			return guides.TryGetValue(guide, out hint) ? hint : null;
		}
		internal static int GetStateByGuides(IEnumerable<BaseDockGuide> guides) {
			int result = 0;
			foreach(BaseDockGuide guide in guides) {
				if(guide.Type == DockGuide.Left || guide.Type == DockGuide.SnapLeft)
					result = (0x01 << 1);
				if(guide.Type == DockGuide.Top || guide.Type == DockGuide.SnapTop)
					result = (0x01 << 2);
				if(guide.Type == DockGuide.Right || guide.Type == DockGuide.SnapRight)
					result = (0x01 << 3);
				if(guide.Type == DockGuide.Bottom || guide.Type == DockGuide.SnapBottom)
					result = (0x01 << 4);
				if(guide.Type == DockGuide.Center || guide.Type == DockGuide.CenterDock)
					result = (0x01 << 5);
			}
			return result;
		}
	}
}
