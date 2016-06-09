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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using System.Collections.Generic;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraBars.Docking2010.Views;
using nm = DevExpress.Utils.Drawing.Helpers.NativeMethods;
using DevExpress.Skins;
namespace DevExpress.XtraBars.Docking2010.Customization {
	class SkinDockGuidePainter : SkinElementPainter {
		static SkinDockGuidePainter defaultPainter;
		public static new SkinDockGuidePainter Default { get { return defaultPainter; } }
		static SkinDockGuidePainter() {
			defaultPainter = new SkinDockGuidePainter();
		}
		protected override void DrawSkinForeground(SkinElementInfo ee) {
			if(ee.GlyphIndex > -1)
				base.DrawSkinForeground(ee);
		}
	}
	class DrawSkinDockGuideHelper {
		static bool IsSideDockGuide(DockGuide type) {			
			return type == DockGuide.Left || type == DockGuide.Right || type == DockGuide.Top || type == DockGuide.Bottom;
		}
		static string GetSkinName(DockGuide type) {
			if(type == DockGuide.Center) return DockingSkins.SkinCenterDockGuide;
			if(type == DockGuide.CenterDock) return DockingSkins.SkinCenterDockGuidePanel;
			if(IsSideDockGuide(type)) return DockingSkins.SkinSideDockGuide;
			return null;
		}
		static int GetIndex(DockGuide type) {
			if(type == DockGuide.Center) return 0;
			if(type == DockGuide.CenterDock) return 0;
			if(IsSideDockGuide(type)) return (int)type;
			return -1;
		}
		static Skin GetSkin(ISkinProvider provider) {
			return DockingSkins.GetSkin(provider);
		}
		public static Color GetColor(Color defaultColor, string nameColor, ISkinProvider provider) {			
			return GetSkin(provider).Colors.GetColor(nameColor, defaultColor);
		}
		static SkinElement GetSkinElement(string name, ISkinProvider provider) {
			return GetSkin(provider)[name];
		}
		static SkinElementInfo GetSkinInfo(string name, ISkinProvider provider) {
			return new SkinElementInfo(GetSkinElement(name, provider));
		}
		static void DrawHotGuide(SkinElementInfo info, BaseDockGuide guide) {			
			info.GlyphIndex = GetIndex(guide.Type) + info.Element.Glyph.ImageCount / 2;
			SkinDockGuidePainter.Default.DrawObject(info);
		}
		static void DrawNormalGuide(SkinElementInfo info, BaseDockGuide guide) {
			info.GlyphIndex = GetIndex(guide.Type);
			SkinDockGuidePainter.Default.DrawObject(info);
		}
		static void DrawBackground(SkinElementInfo info, BaseDockGuide guide){
			info.ImageIndex = GetIndex(guide.Type);
			SkinDockGuidePainter.Default.DrawObject(info);
		}
		public static bool DrawDockGuide(GraphicsCache cache, ISkinProvider provider, BaseDockGuide guide, bool hotGlyph) {			
			SkinElementInfo info = GetSkinInfo(GetSkinName(guide.Type), provider);
			if(info != null && info.Element != null) {
				info.Cache = cache;
				info.Bounds = guide.Bounds;
				if(hotGlyph) DrawHotGuide(info, guide);
				else {
					DrawBackground(info, guide);
					guide.ExcludeHints(cache);
					DrawNormalGuide(info, guide);
				}				
				return true;
			}
			return false;
		}
	}	
	class DockingAdornerOpaquePainter : AdornerOpaquePainter {
		public override void DrawObject(ObjectInfoArgs e) {
			DrawDockingGuide(e.Cache, e as DockingAdornerInfoArgs);
		}
		protected virtual void DrawDockingGuide(GraphicsCache cache, DockingAdornerInfoArgs ea) {
			if(ea.HotHint == DockHint.None) return;
			using(GraphicsClipState state = cache.ClipInfo.SaveAndSetClip(ea.HotHintBounds)) {
				BaseDockGuide guide = ea.HotGuide;
				if(!DrawSkinDockGuideHelper.DrawDockGuide(cache, ea.Owner.ElementsLookAndFeel, guide, true))
					cache.Graphics.DrawImageUnscaled(Resources.DockGuideResourceLoader.GetHotGlyph(guide.Type), guide.Bounds);
			}
		}
	}	
	class DockingAdornerPainter : BaseTabHintPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			DockingAdornerInfoArgs ea = e as DockingAdornerInfoArgs;
			DrawDockZone(e.Cache, ea);
			DrawDockingGuide(e.Cache, ea);
		}
		protected virtual void DrawDockingGuide(GraphicsCache cache, DockingAdornerInfoArgs ea) {
			foreach(BaseDockGuide guide in ea.Guides) {
				if(!DrawSkinDockGuideHelper.DrawDockGuide(cache, ea.Owner.ElementsLookAndFeel, guide, false)) {
					cache.Graphics.DrawImageUnscaled(Resources.DockGuideResourceLoader.GetImage(guide.Type), guide.Bounds);
					guide.ExcludeHints(cache);
					cache.Graphics.DrawImageUnscaled(Resources.DockGuideResourceLoader.GetGlyph(guide.Type), guide.Bounds);
				}
			}
		}
		protected override Color GetDockZoneBorderColor(BaseTabHintInfoArgs e) {
			DockingAdornerInfoArgs ea = e as DockingAdornerInfoArgs;
			return DrawSkinDockGuideHelper.GetColor(ea.Appearance.BorderColor, "DockZoneBorderColor", ea.Owner.ElementsLookAndFeel);	
		}
		protected override Color GetDockZoneBackColor(BaseTabHintInfoArgs e) {
			DockingAdornerInfoArgs ea = e as DockingAdornerInfoArgs;
			return DrawSkinDockGuideHelper.GetColor(ea.Appearance.BackColor, "DockZoneBackColor", ea.Owner.ElementsLookAndFeel);	  
		}
		protected virtual void DrawDockZone(GraphicsCache cache, DockingAdornerInfoArgs ea) {
			if(ea.TabHintVisible)
				DrawTabHint(ea);
			else DrawDockZoneCore(ea);
		}
		protected void DrawDockZoneCore(DockingAdornerInfoArgs ea) {
			if(ea.DockZone.IsEmpty) return;
			ea.Cache.FillRectangle(GetDockZoneBorderColor(ea), ea.DockZone);
			ea.Cache.FillRectangle(GetDockZoneBackColor(ea), Rectangle.Inflate(ea.DockZone, -borderWidth, -borderWidth));
		}
	}
	class DockingAdornerInfoArgs : BaseTabHintInfoArgs {
		BaseView ownerCore;
		public DockingAdornerInfoArgs(BaseView owner) {
			ownerCore = owner;
		}
		Rectangle containerCore;
		public Rectangle Container {
			get { return containerCore; }
			set { containerCore = value; }
		}
		Rectangle adornerCore;
		public Rectangle Adorner {
			get { return adornerCore; }
			set { adornerCore = value; }
		}
		public BaseView Owner {
			get { return ownerCore; }
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
		IEnumerable<BaseDockGuide> guidesCore;
		public IEnumerable<BaseDockGuide> Guides {
			get { return guidesCore; }
		}
		Rectangle dockZoneCore;
		public Rectangle DockZone {
			get { return dockZoneCore; }
		}
		Rectangle hotHintBoundsCore;
		public Rectangle HotHintBounds {
			get { return hotHintBoundsCore; }
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
		protected override int CalcCore() {
			if(!isGuidesReady || StateShiftModifier) {
				List<BaseDockGuide> guidesList = new List<BaseDockGuide>();
				EnsureGuides(Owner, guidesList, DragItem);
				guidesCore = guidesList;
				isGuidesReady = true;
			}
			return CalcGuides(Guides);
		}
		public bool IsOverDockHint(Point point, out DockHint hint) {
			CalcCore();
			hint = HotHint;
			return hint != DockHint.None;
		}
		int CalcGuides(IEnumerable<BaseDockGuide> guides) {
			bool boundsChanged = false;
			dockZoneCore = Rectangle.Empty;
			hotHintBoundsCore = Rectangle.Empty;
			hotHintCore = DockHint.None;
			hotGuideCore = null;
			TabHintVisible = false;
			Rectangle dockingRect = IsRTLAware() ?
				new Rectangle(Bounds.Left - (Adorner.Right - Bounds.Right), Adorner.Top, Adorner.Width, Adorner.Height) : Adorner;
			foreach(BaseDockGuide guide in guides) {
				Rectangle oldBounds = guide.Bounds;
				guide.DockingRect = dockingRect;
				guide.Calc(Configuration, Bounds, Container, MousePosition);
				boundsChanged |= (oldBounds != guide.Bounds);
				if(guide.IsHot) {
					hotGuideCore = guide;
					hotHintCore = guide.HotHint;
					hotHintBoundsCore = guide.GetHint(guide.HotHint);
					if(guide.HotHint != DockHint.Center)
						dockZoneCore = guide.DockZone;
					else TabHintVisible = Configuration.IsTabHintEnabled;
				}
			}
			return CalcState(GetStateByHotHintAndGuide(), GetStateByTabHintOrBoundsChanged(boundsChanged), DockGuideCache.GetStateByGuides(guides));
		}
		int GetStateByHotHintAndGuide() {
			return (((int)HotHint) << 4) + ((HotGuide != null) ? (int)HotGuide.Type : 0);
		}
		int GetStateByTabHintOrBoundsChanged(bool boundsChanged) {
			return (boundsChanged ? 0x02 : 0) + (TabHintVisible ? 0x01 : 0);
		}
		bool IsRTLAware() {
			return (Owner != null) && (Owner.Manager != null) && Owner.Manager.IsRightToLeftLayout();
		}
		DockGuidesConfiguration Configuration;
		DockGuideCache cache = new DockGuideCache();
		void EnsureGuides(BaseView view, IList<BaseDockGuide> guidesList, BaseDocument dragItem) {
			if(dragItem == null) return;
			DockGuide[] guides = new DockGuide[] { DockGuide.Center };
			DockHint[] hints = new DockHint[] { DockHint.Center, 
				DockHint.CenterLeft, DockHint.CenterTop, DockHint.CenterRight, DockHint.CenterBottom };
			if(dragItem.IsDockPanel) {
				guides = new DockGuide[] { 
					DockGuide.Left, DockGuide.Top, DockGuide.Right, DockGuide.Bottom, DockGuide.CenterDock
				};
				hints = new DockHint[] { 
					DockHint.Center, 
					DockHint.CenterLeft, DockHint.CenterTop, DockHint.CenterRight, DockHint.CenterBottom,
					DockHint.CenterSideLeft, DockHint.CenterSideTop, DockHint.CenterSideRight, DockHint.CenterSideBottom,
					DockHint.SideLeft, DockHint.SideTop, DockHint.SideRight, DockHint.SideBottom,
				};
			}
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
			if(!opaque) {
				foreach(BaseDockGuide guide in Guides) {
					if(Configuration.IsEnabled(guide.Type))
						rects.Add(guide.Bounds);
				}
				if(!DockZone.IsEmpty)
					rects.Add(DockZone);
				if(TabHintVisible) {
					rects.Add(Header);
					rects.Add(Content);
				}
			}
			else rects.Add(HotHintBounds);
			return rects;
		}
		public static DockingAdornerInfoArgs EnsureInfoArgs(ref AdornerElementInfo target, Adorner adorner, BaseView owner, Rectangle bounds) {
			return EnsureInfoArgs(ref target, adorner, owner, null, bounds);
		}
		public static DockingAdornerInfoArgs EnsureInfoArgs(ref AdornerElementInfo target, Adorner adorner, BaseView owner, BaseDocument dragItem, Rectangle bounds) {
			DockingAdornerInfoArgs args;
			if(target == null) {
				args = new DockingAdornerInfoArgs(owner);
				args.Bounds = bounds;
				target = new AdornerElementInfo(new DockingAdornerPainter(), new DockingAdornerOpaquePainter(), args);
				if(dragItem != null && adorner.Elements.Count == 0)
					UpdateOwnerWindowZOrder(adorner, owner, dragItem);
				adorner.Show(target);
			}
			else args = target.InfoArgs as DockingAdornerInfoArgs;
			args.SetDirty();
			return args;
		}
		static void UpdateOwnerWindowZOrder(Adorner adorner, BaseView owner, BaseDocument dragItem) {
			if(dragItem == null || dragItem.Form == null || dragItem.Form.Owner != null) return;
			var behindForm = DocumentsHostContext.GetForm(owner.Manager);
			if(behindForm != null && behindForm.IsHandleCreated) {
				int flags = nm.SWP.SWP_NOACTIVATE | nm.SWP.SWP_NOSIZE | nm.SWP.SWP_NOMOVE | nm.SWP.SWP_NOREDRAW;
				nm.SetWindowPos(behindForm.Handle, IntPtr.Zero, 0, 0, 0, 0, flags);
				nm.SetWindowPos(dragItem.Form.Handle, IntPtr.Zero, 0, 0, 0, 0, flags);
			}
		}
	}
#if DEBUGTEST
#endif
}
