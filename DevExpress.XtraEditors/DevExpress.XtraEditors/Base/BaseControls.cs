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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Text;
using DevExpress.Skins;
namespace DevExpress.XtraEditors.ViewInfo {
	public class ObjectPainterBaseControl : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			ObjectInfoArgsBaseControlViewInfo vi = (ObjectInfoArgsBaseControlViewInfo)e;
			if(vi.ViewInfo.Painter == null) return;
			vi.ViewInfo.Painter.Draw(new ControlGraphicsInfoArgs(vi.ViewInfo, e.Cache, vi.ViewInfo.Bounds));
		}
		static ObjectPainterBaseControl _default;
		public static ObjectPainterBaseControl Default {
			get {
				if(_default == null) _default = new ObjectPainterBaseControl();
				return _default;
			}
		}
	}
	public class ObjectInfoArgsBaseControlViewInfo : ObjectInfoArgs {
		BaseControlViewInfo viewInfo;
		public ObjectInfoArgsBaseControlViewInfo(BaseControlViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			base.Bounds = ViewInfo.Bounds;
		}
		public override Rectangle Bounds {
			get {
				return ViewInfo.Bounds;
			}
			set {
				base.Bounds = value;
			}
		}
		public BaseControlViewInfo ViewInfo { get { return viewInfo; } }
	}
	public class BaseControlViewInfo : IDisposable, ICloneable {
		AppearanceDefault defaultAppearance = null;
		object owner;
		bool fIsReady;
		protected bool toolTipInfoCalculated;
		Size fTextSize;
		bool rightToLeft;
		AppearanceObject paintAppearance;
		protected AppearanceObject fAppearance;
		int _lockState, lockPaint;
		protected Rectangle fBounds;
		protected Rectangle fFocusRect, fClientRect, fContentRect, fBorderRect;
		GraphicsInfo gInfo;
		protected ObjectState fState;
		Point _mousePosition;
		MouseButtons _mouseButtons;
		BorderPainter borderPainter;
		UserLookAndFeel lookAndFeel;
		public BaseControlViewInfo(object owner) {
			this.paintAppearance = CreatePaintAppearance();
			this.lockPaint = this._lockState = 0;
			this.owner = owner;
			this.gInfo = GraphicsInfo.Default;
			this.lookAndFeel = null;
			UpdatePainters();
			Reset();
		}
		protected virtual bool IsSkinLookAndFeel {
			get { return LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin; }
		}
		protected virtual Font GetDefaultFont() {
			if(!IsSkinLookAndFeel) return AppearanceObject.DefaultFont;
			IAppearanceDefaultFontProvider fp = LookAndFeel as IAppearanceDefaultFontProvider;
			if(fp != null) return fp.DefaultFont ?? AppearanceObject.DefaultFont;
			return AppearanceObject.DefaultFont;
		}
		protected virtual Font GetDefaultSkinFont(string commonElement) {
			Font res = AppearanceObject.DefaultFont;
			SkinElement element = GetSkinElement(commonElement);
			if(element != null) return element.GetFont(null, LookAndFeel);
			return res;
		}
		protected virtual AppearanceDefault ApplySkinElement(AppearanceDefault appearance, string commonElement) {
			SkinElement element = GetSkinElement(commonElement);
			if(element != null) return element.Apply(appearance, LookAndFeel);
			return appearance;
		}
		protected virtual SkinElement GetSkinElement(string commonElement) {
			SkinElement res = null;
			Skin skin = CommonSkins.GetSkin(LookAndFeel);
			if(skin == null) return res;
			SkinElement element = skin[commonElement];
			if(element != null) return element;
			return skin[CommonSkins.SkinButton];
		}
		internal void SetOwner(object owner) {
			this.owner = owner;
			this.lookAndFeel = null;
		}
		public virtual bool AllowAnimation { 
			get {
				return WindowsFormsSettings.GetAllowHoverAnimation(LookAndFeel.ActiveLookAndFeel);
			} 
		}
		protected virtual AppearanceObject CreatePaintAppearance() { return new AppearanceObject(); }
		protected virtual AppearanceDefault CreateDefaultAppearance() { 
			return new AppearanceDefault(GetSystemColor(SystemColors.WindowText), GetSystemColor(SystemColors.Window), GetDefaultFont());
		}
		public virtual AppearanceDefault DefaultAppearance { 
			get { 
				if(defaultAppearance == null) {
					defaultAppearance = CreateDefaultAppearance();
				}
				return defaultAppearance; 
			} 
		}
		public virtual Color GetSystemColor(Color color) {
			return LookAndFeelHelper.GetSystemColor(LookAndFeel, color);
		}
		public virtual object Clone() {
			return null;
		}
		public virtual UserLookAndFeel LookAndFeel {
			get {
				if(lookAndFeel == null) {
					if(OwnerControl != null) 
						lookAndFeel = OwnerControl.LookAndFeel;
					else
						lookAndFeel = UserLookAndFeel.Default;
				}
				return lookAndFeel;
			}
			set {
				lookAndFeel = value;
			}
		}
		protected virtual void Assign(BaseControlViewInfo info) {
			this.rightToLeft = info.RightToLeft;
			this.fBounds = info.Bounds;
			this.fFocusRect = info.FocusRect;
			this.fClientRect = info.ClientRect;
			this.fContentRect = info.ContentRect;
			this.fBorderRect = info.BorderRect;
			this.fTextSize = info.TextSize;
			this.paintAppearance = info.paintAppearance;
			this.fAppearance = info.fAppearance;
			this.fState = info.State;
			this.fIsReady = info.fIsReady;
			this.borderPainter = info.BorderPainter;
			this.owner = info.owner;
		}
		public virtual void Reset() {
			this.fTextSize = Size.Empty;
			this.fState = ObjectState.Normal;
			this._mouseButtons = MouseButtons.None;
			this._mousePosition = new Point(-10000, -10000);
			this.fState = ObjectState.Normal;
			Clear();
		}
		public bool RightToLeft {
			get { return rightToLeft; }
			set { rightToLeft = value; }
		}
		public virtual AppearanceObject Appearance {
			get { return fAppearance; }
			set { fAppearance = value; }
		}
		protected virtual bool CanUseFocusedAppearance { get { return OwnerControl.ContainsFocus; } }
		protected virtual IStyleController StyleController { get { return OwnerControl == null ? null : OwnerControl.StyleController; } }
		protected internal virtual Size TextSize { get { return fTextSize; } }
		public virtual BorderPainter BorderPainter { get { return borderPainter; } }
		public virtual AppearanceObject PaintAppearance { 
			get { return paintAppearance; } 
			set {
				SetPaintAppearance(value);
			}
		}
		internal void SetPaintAppearanceCore(AppearanceObject paintAppearance) {
			this.paintAppearance = paintAppearance;
			this.paintAppearance.TextOptions.RightToLeft = RightToLeft;
			this.fTextSize = Size.Empty;
		}
		protected void SetPaintAppearance(AppearanceObject paintAppearance) {
			SetPaintAppearanceCore(paintAppearance);
			OnPaintAppearanceChanged();
			this.fTextSize = Size.Empty;
		}
		protected virtual void OnPaintAppearanceChanged() {
		}
		protected internal virtual void ResetAppearanceDefault() {
			this.defaultAppearance = null;
		}
		public virtual void UpdatePaintAppearance() {
			ResetAppearanceDefault();
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { Appearance, StyleController == null ? null : StyleController.Appearance }, DefaultAppearance);
			this.paintAppearance.TextOptions.RightToLeft = RightToLeft;
		}
		public virtual void Clear() {
			this.toolTipInfoCalculated = false;
			this.fIsReady = false;
			this.fBorderRect = this.fFocusRect = this.fClientRect = this.fContentRect = Rectangle.Empty;
		}
		public virtual void Dispose() {
			PaintAppearance.Dispose();
		}
		protected virtual object Owner { get { return owner; } }
		protected internal virtual BaseControl OwnerControl { get { return Owner as BaseControl; } }
		public virtual bool IsReady { get { return fIsReady; } set { fIsReady = false; } }
		public virtual BaseControlPainter Painter { get { return null; } }
		public virtual GraphicsInfo GInfo { get { return gInfo; } }
		public virtual Rectangle Bounds { get { return fBounds; } set { fBounds = value; } }
		public virtual Rectangle FocusRect { get { return fFocusRect; } set { fFocusRect = value; } }
		public virtual Rectangle ContentRect { get { return fContentRect; } }
		public virtual Rectangle ClientRect { get { return fClientRect; } }
		public virtual Rectangle BorderRect { get { return fBorderRect; } }
		public MouseButtons MouseButtons { get { return _mouseButtons; } set { _mouseButtons = value; } }
		public Point MousePosition { get { return _mousePosition; } set { _mousePosition = value; } }
		public void ReCalcViewInfo(Graphics g, MouseButtons buttons, Point mousePosition, Rectangle bounds) {
			if(!CanFastRecalcViewInfo(bounds, mousePosition)) {
				CalcViewInfo(g, buttons, mousePosition, bounds);
				return;
			}
			ReCalcViewInfoCore(g, buttons, mousePosition, bounds);
		}
		protected virtual bool CanFastRecalcViewInfo(Rectangle bounds, Point mousePosition) {
			return IsSupportFastViewInfo && Bounds == bounds && !Bounds.Contains(mousePosition) && IsReady;
		}
		protected virtual void ReCalcViewInfoCore(Graphics g, MouseButtons buttons, Point mousePosition, Rectangle bounds) {
			this.toolTipInfoCalculated = false;
			MousePosition = mousePosition;
			MouseButtons = buttons;
		}
		public void CalcViewInfo(Graphics g, MouseButtons buttons, Point mousePosition, Rectangle bounds) {
			MousePosition = mousePosition;
			MouseButtons = buttons;
			Bounds = bounds;
			CalcViewInfo(g);
		}
		public void UpdateAppearances() {
			UpdateFromOwner();
		}
		bool lockCalcViewInfo = false;
		public void CalcViewInfo() {
			CalcViewInfo(null);
		}
		public virtual void CalcViewInfo(Graphics g) {
			if(lockCalcViewInfo) return;
			GInfo.AddGraphics(g);
			try {
				this.lockCalcViewInfo = true;
				Clear();
				UpdateEnabledState();
				UpdateFromOwner();
				UpdatePainters();
				CalcRects();
				UpdateObjectState();
			}
			finally {
				this.lockCalcViewInfo = false;
				GInfo.ReleaseGraphics();
			}
			this.fIsReady = true;
		}
		public virtual ObjectState State { 
			get { return fState; } 
			set { 
				if(_lockState != 0) return;
				fState = value; 
			} 
		}
		public virtual bool Focused {
			get { return (State & ObjectState.Selected) != 0; }
			set { 
				if(value) State |= ObjectState.Selected; 
				else State &= (~ObjectState.Selected);
			}
		}
		public virtual ObjectState CalcBorderState() {
			ObjectState state = State;
			if(state == ObjectState.Normal && BorderPainter is Office2003BorderPainter) {
				state = ObjectState.Hot;
			}
			return state;
		}
		protected void CheckShowHint() {
			if(this.toolTipInfoCalculated) return;
			CalcShowHint();
		}
		protected virtual void CalcShowHint() { this.toolTipInfoCalculated = true; } 
		protected virtual void UpdateEnabledState() { }
		protected virtual void UpdateFromOwner() { }
		internal void UpdatePaintersCore() { UpdatePainters(); }
		protected virtual void UpdatePainters() {
			this.borderPainter = GetBorderPainter();
		}
		protected virtual bool IsOffice2003ExBorder {
			get {
				if(OwnerControl.BorderStyle == BorderStyles.Office2003 ||
					(OwnerControl.BorderStyle == BorderStyles.Default && OwnerControl.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Office2003)) return true;
				return false;
			}
		}
		protected virtual BorderPainter GetBorderPainterCore() { 
			if(OwnerControl == null) return new EmptyBorderPainter();
			if(IsOffice2003ExBorder) return Office2003BorderPainterEx.Default;
			return BorderHelper.GetPainter(OwnerControl.BorderStyle, OwnerControl.LookAndFeel); 
		}
		protected virtual BorderPainter GetBorderPainter() { 
			return GetBorderPainterCore();
		}
		public virtual bool IsSupportFastViewInfo { get { return true; } }
		protected virtual void ReCalcRects() {
		}
		protected virtual void CalcRects() {
			CalcConstants();
			CalcClientRect(Bounds);
			CalcFocusRect(ClientRect);
			CalcContentRect(FocusRect.IsEmpty ? ClientRect : FocusRect);
		}
		protected virtual void CalcConstants() {
		}
		protected virtual void CalcContentRect(Rectangle bounds) {
			this.fContentRect = bounds;
		}
		protected virtual ObjectInfoArgs GetBorderArgs(Rectangle bounds) {
			return new BorderObjectInfoArgs(null, bounds, null);
		}
		protected internal virtual Rectangle CalcClientRectCore(Rectangle bounds) {
			return BorderPainter.GetObjectClientRectangle(GetBorderArgs(bounds));
		}
		protected virtual void CalcClientRect(Rectangle bounds) {
			this.fBorderRect = bounds;
			this.fClientRect = CalcClientRectCore(bounds);
		}
		protected virtual void CalcFocusRect(Rectangle bounds) {
			this.fFocusRect = Rectangle.Empty;
		}
		protected virtual ObjectState CalcObjectState() { return ObjectState.Normal; }
		protected virtual bool UpdateObjectState() {
			return false;
		}
		public virtual bool UpdateObjectState(MouseButtons mouseButtons, Point mousePosition) { 
			this.MouseButtons = mouseButtons;
			this.MousePosition = mousePosition;
			return UpdateObjectStateCore();
		}
		protected virtual bool UpdateObjectStateCore() {
			bool res = UpdateObjectState();
			if(res) {
				_lockState ++;
				try {
					CalcViewInfo(null);
				}
				finally {
					-- _lockState;
				}
			}
			return res;
		}
		protected bool IsLockStateChanging { get { return _lockState != 0; } }
		public virtual void BeginPaint() {
			if(lockPaint ++ == 0) {
				OnBeginPaint();
			}
		}
		public virtual void EndPaint() {
			if(-- lockPaint == 0) {
				OnEndPaint();
			}
		}
		protected virtual void OnBeginPaint() {
		}
		protected virtual void OnEndPaint() {
		}
		protected void CalcTextSize(Graphics g) { CalcTextSize(g, false); }
		protected virtual int MaxTextWidth { get { return 0; } }
		protected virtual void CalcTextSize(Graphics g, bool useDisplayText) {
			g = GInfo.AddGraphics(g);
			try {
				this.fTextSize = CalcTextSizeCore(g, useDisplayText || AllowHtmlString ? DisplayText : "", MaxTextWidth); 
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		public virtual string DisplayText { get { return ""; } }
		public virtual TextOptions DefaultTextOptions { get { return TextOptions.DefaultOptionsNoWrap; } }
		protected Size CalcTextSizeCore(Graphics g, string text, int maxWidth) {
			g = GInfo.AddGraphics(g);
			try {
				using(GraphicsCache cache = new GraphicsCache(g)) {
					return CalcTextSizeCore(cache, text, maxWidth);
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected internal StringInfo StringInfo { get; set; } 
		public virtual bool AllowHtmlString { get { return false; } }
		protected virtual Size CalcHtmlTextSize(GraphicsCache cache, string text, int maxWidth) {
			return Size.Empty;
		}
		protected virtual Size CalcTextSizeCore(GraphicsCache cache, string text, int maxWidth) {
			Size size = Size.Empty;
			if(text == null || text == "") {
				size = PaintAppearance.CalcDefaultTextSize(cache.Graphics);
			}
			else {
				if(AllowHtmlString) {
					size = CalcHtmlTextSize(cache, text, maxWidth);
				}
				else {
					size = PaintAppearance.CalcTextSizeInt(cache, PaintAppearance.GetTextOptions().GetStringFormat(DefaultTextOptions), text, maxWidth);
				}
			}
			if(size.Height % 2 != 0) size.Height++;
			return size;
		}
		public virtual Size CalcBestFit(Graphics g) { 
			return new Size(16, 16);
		}
	}
	public class BaseStyleControlViewInfo : BaseControlViewInfo {
		public BaseStyleControlViewInfo(BaseStyleControl owner) : base(owner) {
		}
		public override void Reset() {
			base.Reset();
			this.fAppearance = OwnerControl.Appearance;
		}
		protected new BaseStyleControl OwnerControl { get { return base.OwnerControl as BaseStyleControl; } }
		public override BaseControlPainter Painter { get { return OwnerControl.Painter; } }
		protected override void UpdateFromOwner() {
			base.UpdateFromOwner();
			if(OwnerControl == null) return;
			Bounds = OwnerControl.ClientRectangle;
			Focused = OwnerControl.ContainsFocus;
			Appearance = OwnerControl.Appearance;
			RightToLeft = WindowsFormsSettings.GetIsRightToLeft(OwnerControl); 
			UpdatePaintAppearance();
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class ControlGraphicsInfoArgs : GraphicsInfoArgs {
		BaseControlViewInfo viewInfo;
		bool isDrawOnGlass = false;
		public ControlGraphicsInfoArgs(BaseControlViewInfo viewInfo, GraphicsCache cache, Rectangle bounds) : this(viewInfo, cache, bounds, false) {
		}
		public ControlGraphicsInfoArgs(BaseControlViewInfo viewInfo, GraphicsInfoArgs info, Rectangle bounds) : this(viewInfo, info, bounds, false) {
		}
		public ControlGraphicsInfoArgs(BaseControlViewInfo viewInfo, GraphicsCache cache, Rectangle bounds, bool isDrawOnGlass)
			: base(cache, bounds) {
			this.viewInfo = viewInfo;
			this.isDrawOnGlass = isDrawOnGlass;
		}
		public ControlGraphicsInfoArgs(BaseControlViewInfo viewInfo, GraphicsInfoArgs info, Rectangle bounds, bool isDrawOnGlass)
			: base(info, bounds) {
			this.viewInfo = viewInfo;
			this.isDrawOnGlass = isDrawOnGlass;
		}
		public BaseControlViewInfo ViewInfo { get { return viewInfo; } }
		public bool IsDrawOnGlass { get { return isDrawOnGlass; } set { isDrawOnGlass = value; } }
	}
	public class BaseControlPainter {
		protected virtual bool IsDrawBorderLast(ControlGraphicsInfoArgs info) { return true; }
		public virtual void Draw(ControlGraphicsInfoArgs info) {
			info.ViewInfo.BeginPaint();
			try {
				if(!IsDrawBorderLast(info)) DrawBorder(info);
				DrawAdornments(info);
				DrawContent(info);
				DrawFocusRect(info);
				if(IsDrawBorderLast(info)) DrawBorder(info);
			}
			finally {
				info.ViewInfo.EndPaint();
			}
		}
		protected virtual void DrawBorder(ControlGraphicsInfoArgs info) {
			BaseControlViewInfo vi = info.ViewInfo as BaseControlViewInfo;
			ObjectState borderState = vi.CalcBorderState();
			vi.BorderPainter.DrawObject(new BorderObjectInfoArgs(info.Cache, vi.BorderRect, vi.PaintAppearance, borderState));
		}
		protected virtual void DrawFocusRect(ControlGraphicsInfoArgs info) {
		}
		protected virtual void DrawContent(ControlGraphicsInfoArgs info) {
		}
		protected virtual void DrawAdornments(ControlGraphicsInfoArgs info) {
		}
	}
}
