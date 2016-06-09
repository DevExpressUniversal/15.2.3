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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking;
namespace DevExpress.XtraBars.Docking2010.Customization {
	interface IStyleDockZone {
		Control Owner { get; }
		DevExpress.LookAndFeel.UserLookAndFeel ElementsLookAndFeel { get; }
	}
	class VS2010StyleDockZoneVisualizer : VS2005StyleDockingVisualizer {
		Adorner adornerCore;
		DevExpress.LookAndFeel.UserLookAndFeel elementsLookAndFeel;
		Docking.DockManager dockManagerCore;
		public VS2010StyleDockZoneVisualizer(VisualizerRole role, int fadeSpeed, int framesCount, IStyleDockZone info)
			: base(role, fadeSpeed, framesCount) {
			adornerCore = new Adorner(info.Owner);
			elementsLookAndFeel = info.ElementsLookAndFeel;
			dockManagerCore = info as Docking.DockManager;
		}
		public DevExpress.LookAndFeel.UserLookAndFeel ElementsLookAndFeel {
			get { return elementsLookAndFeel; }
		}
		public Docking.DockManager DockManager {
			get { return dockManagerCore; }
		}
		[ThreadStatic]
		static WeakReference ownerPanelRefCore;
		internal static DockPanel OwnerPanel {
			get { return (ownerPanelRefCore != null) ? ownerPanelRefCore.Target as DockPanel : null; }
			set {
				if(value != null)
					ownerPanelRefCore = new WeakReference(value);
				else ownerPanelRefCore = null;
			}
		}
		[ThreadStatic]
		static WeakReference targetPanelRefCore;
		internal static DockPanel TargetPanel {
			get { return (targetPanelRefCore != null) ? targetPanelRefCore.Target as DockPanel : null; }
			set {
				if(value != null)
					targetPanelRefCore = new WeakReference(value);
				else targetPanelRefCore = null;
			}
		}
		protected override void OnDispose() {
			Ref.Dispose(ref adornerCore);
			elementsLookAndFeel = null;
			base.OnDispose();
		}
		protected override void SetDefaultWindowSizes() {
			defaultWindowSizes.Add(VisualizerFormType.Center, new Size(140, 140));
			defaultWindowSizes.Add(VisualizerFormType.CenterNoTabs, new Size(140, 140));
			defaultWindowSizes.Add(VisualizerFormType.Left, new Size(50, 50));
			defaultWindowSizes.Add(VisualizerFormType.Top, new Size(50, 50));
			defaultWindowSizes.Add(VisualizerFormType.Right, new Size(50, 50));
			defaultWindowSizes.Add(VisualizerFormType.Bottom, new Size(50, 50));
		}
		protected override void CreateWindows() { }
		AdornerElementInfo adornerInfo;
		protected override void UpdateWindows() {
			VS2010StyleDockingVisualizerAdornerInfoArgs args =
				VS2010StyleDockingVisualizerAdornerInfoArgs.EnsureAdornerInfoArgs(ref adornerInfo, this);
			if(State == VisualizerState.AllHidden) {
				Adorner.Reset(adornerInfo);
				return;
			}
			args.Bounds = new Rectangle(Point.Empty, Bounds.Size);
			Rectangle screenBounds = Bounds;
			if(IsRTLAware())
				screenBounds = new Rectangle(screenBounds.X - screenBounds.Width, screenBounds.Y, screenBounds.Width, screenBounds.Height);
			Point paintOffset = Point.Empty;
			if(Role == VisualizerRole.PanelVisualizer) {
				if(screenBounds.Width < 140)
					paintOffset.X = Round((double)(140 - screenBounds.Width) * 0.5);
				if(screenBounds.Height < 140)
					paintOffset.Y = Round((double)(140 - screenBounds.Height) * 0.5);
				screenBounds = new Rectangle(
						screenBounds.X - paintOffset.X,
						screenBounds.Y - paintOffset.Y,
						Math.Max(140, screenBounds.Width),
						Math.Max(140, screenBounds.Height)
					);
			}
			Adorner.Show(adornerInfo, screenBounds, paintOffset);
		}
		int Round(double value) {
			return value > 0 ? (int)(value + 0.5) : (int)(value - 0.5);
		}
		bool IsRTLAware() {
			return (Role == VisualizerRole.RootLayoutVisualizer) && Adorner.IsRightToLeftLayout;
		}
		public Adorner Adorner {
			get { return adornerCore; }
		}
		protected override VS2005StyleDockingVisualizerViewInfo CreateViewInfo() {
			return new VS2010StyleDockingVisualizerViewInfo(this);
		}
		public bool IsHot {
			get { return CalcIsHotTrack(); }
		}
		internal bool CalcIsHotTrack() {
			if(adornerInfo == null || adornerInfo.InfoArgs == null) return false;
			if(!Adorner.OpaqueLayer.IsVisible && !Adorner.TransparentLayer.IsVisible) return false;
			VS2010StyleDockingVisualizerAdornerInfoArgs args = adornerInfo.InfoArgs as VS2010StyleDockingVisualizerAdornerInfoArgs;
			if(args == null) return false;
			if(args.HotHint != DockHint.None) return true;
			return false;
		}
		public override bool UpdateHotTracked(Point point, bool processOnlyCenter, TabsPosition pos, Rectangle headerRect) {
			viewInfoCore.Calculate();
			VS2010StyleDockingVisualizerAdornerInfoArgs args = VS2010StyleDockingVisualizerAdornerInfoArgs.EnsureAdornerInfoArgs(ref adornerInfo, this);
			args.MousePosition = PointToClient(point);
			args.HeaderRect = headerRect;
			if(args.Calc()) {
				if(args.TabHintVisible)
					CalcTabs(args, args.Bounds, pos);
				Adorner.Invalidate();
				return true;
			}
			return false;
		}
		Point PointToClient(Point screenPoint) {
			return Point.Subtract(Adorner.PointToClient(screenPoint), new Size(Adorner.PaintOffset));
		}
		void CalcTabs(VS2010StyleDockingVisualizerAdornerInfoArgs e, Rectangle bounds, TabsPosition tabsPosition) {
			Rectangle content = bounds;
			content.Inflate(-4, -4);
			content.Y += 20;
			content.Height -= 20;
			Rectangle header = bounds;
			header.Inflate(-4, -4);
			Size tabSizeH = new Size(50, 20);
			Size tabSizeV = new Size(20, 50);
			Size headerRectSize = Size.Empty;
			Point headerRectLocation = Point.Empty;
			if(!e.HeaderRect.IsEmpty) {
				headerRectSize = e.HeaderRect.Size;
				headerRectLocation = e.HeaderRect.Location;
			}
			switch(tabsPosition) {
				case TabsPosition.Left:
					header.Width = tabSizeV.Width;
					header.Height = tabSizeV.Height + ((headerRectSize.Height > 0) ? headerRectSize.Height - tabSizeV.Height : 0);
					header.Y = headerRectLocation.Y > 0 ? headerRectLocation.Y : content.Y;
					content.X = header.Right;
					content.Width -= header.Width;
					e.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Left;
					break;
				case TabsPosition.Right:
					header.Width = tabSizeV.Width;
					header.Height = tabSizeV.Height + ((headerRectSize.Height > 0) ? headerRectSize.Height - tabSizeV.Height : 0);
					header.Y = headerRectLocation.Y > 0 ? headerRectLocation.Y : content.Y;
					content.Width -= header.Width;
					header.X = content.Right;
					e.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Right;
					break;
				case TabsPosition.Top:
					header.Width = tabSizeH.Width + ((headerRectSize.Width > 0) ? headerRectSize.Width - tabSizeH.Width : 0);
					header.Height = tabSizeH.Height;
					header.X = headerRectLocation.X > 0 ? headerRectLocation.X : header.X;
					header.Y += 20;
					content.Y = header.Bottom;
					content.Height -= header.Height;
					e.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Top;
					break;
				case TabsPosition.Bottom:
					header.Width = tabSizeH.Width + ((headerRectSize.Width > 0) ? headerRectSize.Width - tabSizeH.Width : 0);
					header.Height = tabSizeH.Height;
					header.X = headerRectLocation.X > 0 ? headerRectLocation.X : header.X;
					content.Height -= header.Height;
					header.Y = content.Bottom;
					e.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Bottom;
					break;
			}
			e.Content = content;
			e.Header = header;
		}
		protected override VisualizerHitInfoType CalcHitInfoCore(Point p) {
			if(adornerInfo == null) return base.CalcHitInfoCore(p);
			VS2010StyleDockingVisualizerAdornerInfoArgs args = adornerInfo.InfoArgs as VS2010StyleDockingVisualizerAdornerInfoArgs;
			if(args == null || !args.IsReady) return VisualizerHitInfoType.Nothing;
			if(args.HitTest(PointToClient(p)))
				return ConvertDockHintToVisualizerHitInfoType(args.HotHint);
			else return VisualizerHitInfoType.Nothing;
		}
		protected internal VisualizerVisibilityArgs RequestVisibilityPanel(VisualizerVisibilityArgs visibilityArgs) {
			if(adornerInfo == null) return visibilityArgs;
			VS2010StyleDockingVisualizerAdornerInfoArgs args = adornerInfo.InfoArgs as VS2010StyleDockingVisualizerAdornerInfoArgs;
			if(args != null)
				args.EnsurePanelVisibilityArgs(visibilityArgs);
			return visibilityArgs;
		}
		protected internal VisualizerVisibilityArgs RequestVisibilityGlobal(VisualizerVisibilityArgs visibilityArgs) {
			if(adornerInfo == null) return visibilityArgs;
			VS2010StyleDockingVisualizerAdornerInfoArgs args = adornerInfo.InfoArgs as VS2010StyleDockingVisualizerAdornerInfoArgs;
			if(args != null)
				args.EnsureGlobalVisibilityArgs(visibilityArgs);
			return visibilityArgs;
		}
		VisualizerHitInfoType ConvertDockHintToVisualizerHitInfoType(DockHint hotHint) {
			bool isRTL = IsRTLAware();
			switch(hotHint) {
				case DockHint.Center:
					return VisualizerHitInfoType.CenterCenter;
				case DockHint.CenterLeft:
					return isRTL ? VisualizerHitInfoType.CenterRight : VisualizerHitInfoType.CenterLeft;
				case DockHint.CenterRight:
					return isRTL ? VisualizerHitInfoType.CenterLeft : VisualizerHitInfoType.CenterRight;
				case DockHint.CenterTop:
					return VisualizerHitInfoType.CenterTop;
				case DockHint.CenterBottom:
					return VisualizerHitInfoType.CenterBottom;
				case DockHint.SideBottom:
					return VisualizerHitInfoType.Bottom;
				case DockHint.SideLeft:
					return isRTL ? VisualizerHitInfoType.Right : VisualizerHitInfoType.Left;
				case DockHint.SideRight:
					return isRTL ? VisualizerHitInfoType.Left : VisualizerHitInfoType.Right;
				case DockHint.SideTop:
					return VisualizerHitInfoType.Top;
				default: return VisualizerHitInfoType.Nothing;
			}
		}
	}
	class VS2010StyleDockingVisualizerViewInfo : VS2005StyleDockingVisualizerViewInfo {
		public VS2010StyleDockingVisualizerViewInfo(VS2010StyleDockZoneVisualizer owner)
			: base(owner) {
		}
	}
	class VS2010StyleDockingVisualizerAdornerInfoArgs : BaseTabHintInfoArgs {
		VS2010StyleDockZoneVisualizer ownerCore;
		public VS2010StyleDockingVisualizerAdornerInfoArgs(VS2010StyleDockZoneVisualizer owner) {
			ownerCore = owner;
		}
		public VS2010StyleDockZoneVisualizer Owner {
			get { return ownerCore; }
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
		bool ShowHotTrackWindow { get; set; }
		Rectangle headerRectCore;
		public Rectangle HeaderRect {
			get { return headerRectCore; }
			set {
				if(HeaderRect != value) {
					headerRectCore = value;
					ShowHotTrackWindow = !headerRectCore.IsEmpty;
					UpdateHotTrackWindow = true;
				}
				else UpdateHotTrackWindow = false;
			}
		}
		bool UpdateHotTrackWindow { get; set; }
		Point mousePositionCore;
		public Point MousePosition {
			get { return mousePositionCore; }
			set { mousePositionCore = value; }
		}
		readonly static DockGuide[] guides = new DockGuide[] { 
					DockGuide.Left, DockGuide.Top, DockGuide.Right, DockGuide.Bottom, DockGuide.Center};
		readonly static DockHint[] hints = new DockHint[] { 
					DockHint.Center, 
					DockHint.CenterLeft, DockHint.CenterTop, DockHint.CenterRight, DockHint.CenterBottom,
					DockHint.SideLeft, DockHint.SideTop, DockHint.SideRight, DockHint.SideBottom};
		protected override int CalcCore() {
			Configuration = new DockGuidesConfiguration(guides, hints);
			Configuration.Calc(Owner.State, Owner.Role, Owner.StateArgs);
			Configuration.RaiseShowingDockGuides(Owner);
			guidesCore = EnsureGuides();
			return CalcGuides(Guides);
		}
		protected internal void EnsurePanelVisibilityArgs(Docking.VisualizerVisibilityArgs stateArgs) {
			if(Owner.State == VisualizerState.AllHidden || Owner.Role == VisualizerRole.RootLayoutVisualizer) return;
			var configuration = new DockGuidesConfiguration(guides, hints);
			configuration.Calc(Owner.State, Owner.Role, stateArgs);
			configuration.RaiseShowingDockGuides(Owner);
			configuration.UpdateVisibilityArgsByCenterDockGuide(stateArgs);
		}
		protected internal void EnsureGlobalVisibilityArgs(Docking.VisualizerVisibilityArgs stateArgs) {
			if(Owner.State == VisualizerState.AllHidden || Owner.Role == VisualizerRole.PanelVisualizer) return;
			var configuration = new DockGuidesConfiguration(guides, hints);
			configuration.Calc(Owner.State, Owner.Role, stateArgs);
			configuration.RaiseShowingDockGuides(Owner);
			configuration.UpdateVisibilityArgsBySideDockGuides(stateArgs);
		}
		DockGuidesConfiguration Configuration;
		DockGuideCache cache = new DockGuideCache();
		IEnumerable<BaseDockGuide> EnsureGuides() {
			List<BaseDockGuide> guidesList = new List<BaseDockGuide>();
			for(int i = 0; i < guides.Length; i++) {
				if(Configuration.IsEnabled(guides[i])) {
					guidesList.Add(cache.GetGuide(guides[i]));
				}
			}
			return guidesList;
		}
		int CalcGuides(IEnumerable<BaseDockGuide> guides) {
			bool boundsChanged = false;
			dockZoneCore = Rectangle.Empty;
			hotHintBoundsCore = Rectangle.Empty;
			hotHintCore = DockHint.None;
			hotGuideCore = null;
			TabHintVisible = false;
			foreach(BaseDockGuide guide in guides) {
				Rectangle oldBounds = guide.Bounds;
				guide.DockingRect = Bounds;
				guide.Calc(Configuration, Bounds, Bounds, MousePosition);
				boundsChanged |= (oldBounds != guide.Bounds);
				if(guide.IsHot) {
					hotGuideCore = guide;
					hotHintCore = guide.HotHint;
					hotHintBoundsCore = guide.GetHint(guide.HotHint);
					if(guide.HotHint != DockHint.Center)
						dockZoneCore = guide.DockZone;
					else TabHintVisible = true;
				}
				if(ShowHotTrackWindow)
					TabHintVisible = true;
			}
			return CalcState(GetStateByHotHintAndGuide(), GetStateByTabHintOrHotTrackOrBounds(boundsChanged), DockGuideCache.GetStateByGuides(guides));
		}
		int GetStateByHotHintAndGuide() {
			return (((int)HotHint) << 4) + ((HotGuide != null) ? (int)HotGuide.Type : 0);
		}
		int GetStateByTabHintOrHotTrackOrBounds(bool boundsChanged) {
			return (boundsChanged ? 0x04 : 0) + (TabHintVisible ? 0x02 : 0) + (UpdateHotTrackWindow ? 0x01 : 0);
		}
		public bool HitTest(Point point) {
			dockZoneCore = Rectangle.Empty;
			hotHintBoundsCore = Rectangle.Empty;
			hotHintCore = DockHint.None;
			hotGuideCore = null;
			TabHintVisible = false;
			foreach(BaseDockGuide guide in Guides) {
				guide.Calc(Configuration, Bounds, Bounds, point);
				if(guide.IsHot) {
					hotGuideCore = guide;
					hotHintCore = guide.HotHint;
					hotHintBoundsCore = guide.GetHint(guide.HotHint);
					if(guide.HotHint != DockHint.Center)
						dockZoneCore = guide.DockZone;
					else TabHintVisible = true;
				}
			}
			return hotHintCore != DockHint.None;
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
		public static VS2010StyleDockingVisualizerAdornerInfoArgs EnsureAdornerInfoArgs(
			ref AdornerElementInfo adornerInfo, VS2010StyleDockZoneVisualizer owner) {
			VS2010StyleDockingVisualizerAdornerInfoArgs args;
			if(adornerInfo == null) {
				args = new VS2010StyleDockingVisualizerAdornerInfoArgs(owner);
				adornerInfo = new AdornerElementInfo(
					new VS2010StyleDockingVisualizerAdornerPainter(),
					new VS2010StyleDockingVisualizerOpaqueAdornerPainter(), args);
			}
			else args = adornerInfo.InfoArgs as VS2010StyleDockingVisualizerAdornerInfoArgs;
			args.SetDirty();
			return args;
		}
	}
	class VS2010StyleDockingVisualizerOpaqueAdornerPainter : AdornerOpaquePainter {
		public override void DrawObject(ObjectInfoArgs e) {
			DrawDockingGuide(e.Cache, e as VS2010StyleDockingVisualizerAdornerInfoArgs);
		}
		protected virtual void DrawDockingGuide(GraphicsCache cache, VS2010StyleDockingVisualizerAdornerInfoArgs ea) {
			if(ea.HotHint == DockHint.None) return;
			using(GraphicsClipState state = cache.ClipInfo.SaveAndSetClip(ea.HotHintBounds)) {
				BaseDockGuide guide = ea.HotGuide;
				if(!DrawSkinDockGuideHelper.DrawDockGuide(cache, ea.Owner.ElementsLookAndFeel, guide, true))
					cache.Graphics.DrawImageUnscaled(Resources.DockGuideResourceLoader.GetHotGlyph(guide.Type), guide.Bounds);
			}
		}
	}
	class VS2010StyleDockingVisualizerAdornerPainter : BaseTabHintPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			VS2010StyleDockingVisualizerAdornerInfoArgs ea = e as VS2010StyleDockingVisualizerAdornerInfoArgs;
			DrawDockZone(e.Cache, ea);
			DrawDockingGuide(e.Cache, ea);
		}
		protected virtual void DrawDockingGuide(GraphicsCache cache, VS2010StyleDockingVisualizerAdornerInfoArgs ea) {
			foreach(BaseDockGuide guide in ea.Guides) {
				if(!DrawSkinDockGuideHelper.DrawDockGuide(cache, ea.Owner.ElementsLookAndFeel, guide, false)) {
					cache.Graphics.DrawImageUnscaled(Resources.DockGuideResourceLoader.GetImage(guide.Type), guide.Bounds);
					guide.ExcludeHints(cache);
					cache.Graphics.DrawImageUnscaled(Resources.DockGuideResourceLoader.GetGlyph(guide.Type), guide.Bounds);
				}
			}
		}
		protected override Color GetDockZoneBorderColor(BaseTabHintInfoArgs e) {
			VS2010StyleDockingVisualizerAdornerInfoArgs ea = e as VS2010StyleDockingVisualizerAdornerInfoArgs;
			return DrawSkinDockGuideHelper.GetColor(ea.Appearance.BorderColor, "DockZoneBorderColor", ea.Owner.ElementsLookAndFeel);
		}
		protected override Color GetDockZoneBackColor(BaseTabHintInfoArgs e) {
			VS2010StyleDockingVisualizerAdornerInfoArgs ea = e as VS2010StyleDockingVisualizerAdornerInfoArgs;
			return DrawSkinDockGuideHelper.GetColor(ea.Appearance.BackColor, "DockZoneBackColor", ea.Owner.ElementsLookAndFeel);
		}
		protected virtual void DrawDockZone(GraphicsCache cache, VS2010StyleDockingVisualizerAdornerInfoArgs ea) {
			if(ea.TabHintVisible)
				DrawTabHint(ea);
			else DrawDockZoneCore(ea);
		}
		protected void DrawDockZoneCore(VS2010StyleDockingVisualizerAdornerInfoArgs ea) {
			if(ea.DockZone.IsEmpty) return;
			ea.Cache.FillRectangle(GetDockZoneBorderColor(ea), ea.DockZone);
			ea.Cache.FillRectangle(GetDockZoneBackColor(ea), Rectangle.Inflate(ea.DockZone, -borderWidth, -borderWidth));
		}
	}
}
