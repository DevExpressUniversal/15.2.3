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
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010.Customization {
	class SnapAdornerOpaquePainter : AdornerOpaquePainter {
		public override void DrawObject(ObjectInfoArgs e) {
			SnapAdornerInfoArgs ea = e as SnapAdornerInfoArgs;
			DrawDockZone(e.Cache, ea);
		}
		protected virtual void DrawDockZone(GraphicsCache cache, SnapAdornerInfoArgs ea) {
			if(ea.DockZone.IsEmpty) return;
			DevExpress.Utils.Helpers.PaintHelper.DrawImage(cache.Graphics, Resources.DockGuideResourceLoader.GetSnapImage(), 
				ea.DockZone, new System.Windows.Forms.Padding(16));
		}
	}
	class SnapAdornerInfoArgs : AdornerElementInfoArgs {
		BaseView ownerCore;
		public SnapAdornerInfoArgs(BaseView owner) {
			ownerCore = owner;
		}
		public BaseView Owner {
			get { return ownerCore; }
		}
		Rectangle dockingRectCore;
		public Rectangle DockingRect {
			get { return dockingRectCore; }
			set { dockingRectCore = value; }
		}
		string screenCore;
		public string Screen {
			get { return screenCore; }
			set { screenCore = value; }
		}
		Point mousePositionCore;
		public Point MousePosition {
			get { return mousePositionCore; }
			set { mousePositionCore = value; }
		}
		BaseDocument dragItemCore;
		public BaseDocument DragItem {
			get { return dragItemCore; }
			set {
				if(DragItem == value) return;
				dragItemCore = value;
				isGuidesReady = false;
			}
		}
		Rectangle dockZoneCore;
		public Rectangle DockZone {
			get { return dockZoneCore; }
		}
		DockHint hotHintCore;
		public DockHint HotHint {
			get { return hotHintCore; }
		}
		BaseDockGuide hotGuideCore;
		public BaseDockGuide HotGuide {
			get { return hotGuideCore; }
		}
		bool isGuidesReady;
		IEnumerable<BaseDockGuide> allGuides;
		protected override int CalcCore() {
			if(!isGuidesReady) {
				List<BaseDockGuide> guidesList = new List<BaseDockGuide>();
				EnsureGuides(Owner, guidesList, DragItem);
				allGuides = guidesList;
				isGuidesReady = true;
			}
			return CalcGuide(CheckActualGuide());
		}
		BaseDockGuide CheckActualGuide() {
			Rectangle screen = new Rectangle(Point.Empty, Bounds.Size);
			Point p = new Point(MousePosition.X - Bounds.Left, MousePosition.Y - Bounds.Top);
			DockGuide actual = DockGuideExtension.GetSnap(screen, p);
			foreach(BaseDockGuide guide in allGuides) {
				if(guide.Type == actual)
					return guide;
			}
			return null;
		}
		public bool IsOverDockHint(Point point, out DockHint hint) {
			CalcCore();
			hint = HotHint;
			return hint != DockHint.None;
		}
		int CalcGuide(BaseDockGuide guide) {
			dockZoneCore = Rectangle.Empty;
			hotHintCore = DockHint.None;
			hotGuideCore = null;
			bool canSnap = !DockingRect.Contains(MousePosition);
			if(guide != null) {
				Rectangle screen = new Rectangle(Point.Empty, Bounds.Size);
				Point p = new Point(MousePosition.X - Bounds.Left, MousePosition.Y - Bounds.Top);
				guide.DockingRect = screen;
				if(screen.Contains(p))
					guide.Calc(Configuration, screen, screen, p);
				else
					guide.EmulateHotTrack(screen, screen, DockGuideExtension.ToSnapDockHint(guide.Type));
				if(guide.IsHot) {
					hotGuideCore = guide;
					hotHintCore = guide.HotHint;
					if(guide.HotHint != DockHint.Center)
						dockZoneCore = guide.DockZone;
				}
			}
			return CalcState((int)HotHint + (canSnap ? 0x100 : 0), (HotGuide != null) ? (int)HotGuide.Type : 0, Screen.GetHashCode());
		}
		DockGuidesConfiguration Configuration;
		DockGuideCache cache = new DockGuideCache();
		void EnsureGuides(BaseView view, IList<BaseDockGuide> guidesList, BaseDocument dragItem) {
			if(dragItem == null) return;
			DockGuide[] guides = new DockGuide[] { DockGuide.SnapLeft, DockGuide.SnapTop, DockGuide.SnapRight, DockGuide.SnapBottom };
			DockHint[] hints = new DockHint[] { DockHint.SnapLeft, DockHint.SnapScreen, DockHint.SnapRight, DockHint.SnapBottom };
			Configuration = new DockGuidesConfiguration(guides, hints);
			view.OnShowingDockGuides(Configuration, dragItem, MousePosition);
			for(int i = 0; i < guides.Length; i++) {
				if(Configuration.IsEnabled(guides[i])) {
					guidesList.Add(cache.GetGuide(guides[i]));
				}
			}
		}
		protected override IEnumerable<Rectangle> GetRegionsCore(bool opaque) {
			List<Rectangle> rects = new List<Rectangle>();
			if(!DockZone.IsEmpty)
				rects.Add(DockZone);
			return rects;
		}
		public static SnapAdornerInfoArgs EnsureInfoArgs(ref AdornerElementInfo target, Adorner adorner, BaseView owner) {
			SnapAdornerInfoArgs args;
			if(target == null) {
				args = new SnapAdornerInfoArgs(owner);
				target = new AdornerElementInfo(new SnapAdornerOpaquePainter(), args);
			}
			else args = target.InfoArgs as SnapAdornerInfoArgs;
			args.SetDirty();
			return args;
		}
	}
}
