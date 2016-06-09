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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraEditors {
	[DXToolboxItem(DXToolboxItemKind.Free), Description("A separating line"), ToolboxTabName(AssemblyInfo.DXTabCommon)]
	[Designer("DevExpress.XtraEditors.Design.SeparatorControlDesigner," + AssemblyInfo.SRAssemblyDesign)]
	[SmartTagSupport(typeof(ControlBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.Auto), SmartTagFilter(typeof(ControlFilter))]
	public class SeparatorControl : ControlBase, ISupportLookAndFeel, ISupportInitialize {
		SeparatorControlPainter painterCore;
		SeparatorControlInfo infoCore;
		bool autoSizeModeCore;
		UserLookAndFeel lookAndFeelCore;
		Alignment lineAlignmentCore;
		Orientation lineOrientationCore;
		DashStyle lineStyleCore;
		Color lineColorCore;
		int lineThicknessCore;
		bool isDisposingCore;
		public SeparatorControl() {
			OnInitialize();
		}
		protected internal bool IsDisposing {
			get { return isDisposingCore; }
		}
		protected override void Dispose(bool disposing) {
			if(!IsDisposing) {
				isDisposingCore = true;
				OnDisposing();
			}
			base.Dispose(disposing);
		}
		public new Padding Padding {
			get { return base.Padding; }
			set { base.Padding = value; }
		}
		protected override Padding DefaultPadding {
			get { return new Padding(9); }
		}
		public bool ShouldSerializePadding() {
			return Padding != DefaultPadding;
		}
		public void ResetPadding() {
			Padding = DefaultPadding;
		}
		protected override void OnPaddingChanged(EventArgs e) {
			LayoutChanged();
		}
		[ DefaultValue(false), Category("Behavior"), SmartTagProperty("AutoSizeMode", "", 2)]
		public bool AutoSizeMode {
			get { return autoSizeModeCore; }
			set { SetValueCore(ref autoSizeModeCore, value, OnAutoSizeModeChanged); }
		}
		protected virtual void OnAutoSizeModeChanged() {
			OnResize(EventArgs.Empty);
		}
		[ Category("Layout")]
		public int LineThickness {
			get { return lineThicknessCore; }
			set { SetValueCore(ref lineThicknessCore, value, OnLineThicknessChanged); }
		}
		public bool ShouldSerializeLineThickness() {
			return LineThickness != Painter.DefaultLineThickness;
		}
		public void ResetLineThickness() {
			LineThickness = Painter.DefaultLineThickness;
			SetLineThicknessChanged(false);
		}
		bool lineThicknessChanged;
		protected void SetLineThicknessChanged(bool state) {
			lineThicknessChanged = state;
		}
		protected virtual void OnLineThicknessChanged() {
			SetLineThicknessChanged(true);
			LayoutChanged();
		}
		[ DefaultValue(Alignment.Default), Category("Layout"), SmartTagProperty("Line Alignment", "", 0)]
		public Alignment LineAlignment {
			get { return lineAlignmentCore; }
			set { SetValueCore(ref lineAlignmentCore, value, LayoutChanged); }
		}
		[ DefaultValue(Orientation.Horizontal), Category("Layout"), SmartTagProperty("Line Orientation", "", 1)]
		public Orientation LineOrientation {
			get { return lineOrientationCore; }
			set { SetValueCore(ref lineOrientationCore, value, LayoutChanged); }
		}
		[ Category("Appearance")]
		public Color LineColor {
			get { return lineColorCore; }
			set { SetValueCore(ref lineColorCore, value, LayoutChanged); }
		}
		public bool ShouldSerializeLineColor() {
			return !LineColor.IsEmpty;
		}
		public void ResetLineColor() {
			LineColor = Color.Empty;
		}
		[ DefaultValue(DashStyle.Solid), Category("Appearance")]
		public DashStyle LineStyle {
			get { return lineStyleCore; }
			set { SetValueCore(ref lineStyleCore, value, LayoutChanged); }
		}
		[ Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UserLookAndFeel LookAndFeel {
			get { return lookAndFeelCore; }
		}
		protected internal SeparatorControlPainter Painter {
			get { return painterCore; }
		}
		protected internal SeparatorControlInfo Info {
			get { return infoCore; }
		}
		protected virtual void OnInitialize() {
			lookAndFeelCore = CreateLookAndFeel();
			infoCore = CreateInfo();
			painterCore = CreatePainter();
			lineThicknessCore = Painter.DefaultLineThickness;
			SubscribeOnEvents();
		}
		void OnDisposing() {
			UnsubscribeOnEvents();
			LookAndFeel.Dispose();
		}
		protected virtual SeparatorControlPainter CreatePainter() {
			if(LookAndFeel.Style == LookAndFeelStyle.Skin)
				return new SeparatorControlSkinPainter(LookAndFeel);
			else
				return new SeparatorControlPainter();
		}
		protected virtual SeparatorControlInfo CreateInfo() {
			return new SeparatorControlInfo(this);
		}
		protected virtual UserLookAndFeel CreateLookAndFeel() {
			return new ControlUserLookAndFeel(this);
		}
		protected virtual void SubscribeOnEvents() {
			LookAndFeel.StyleChanged += OnStyleChanged;
		}
		protected virtual void UnsubscribeOnEvents() {
			LookAndFeel.StyleChanged -= OnStyleChanged;
		}
		public void LayoutChanged() {
			Info.SetDirty();
			Invalidate();
			Update();
		}
		protected virtual void UpdateStyle() {
			painterCore = CreatePainter();
		}
		protected virtual void UpdateLineWidth() {
			if(lineThicknessChanged) return;
			lineThicknessCore = Painter.DefaultLineThickness;
		}
		protected override void OnResize(EventArgs e) {
			if(IsInitializing) return;
			OnResizeCore();
		}
		protected virtual void OnResizeCore(){
			if(AutoSizeMode)
				Info.UpdateSavedSize();
			else
				Info.RestoreSavedSize();
			LayoutChanged();
		}
		protected override void OnPaint(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				if(!Info.IsReady)
					infoCore.Calc(ClientRectangle);
				ObjectPainter.DrawObject(cache, Painter, Info);
			}
		}
		protected override void OnRightToLeftChanged() {
			LayoutChanged();
		}
		protected bool SetValueCore<T>(ref T property, T value, Action action = null) {
			if(property.Equals(value)) return false;
			property = value;
			if(action != null)
				action();
			return true;
		}
		protected internal Alignment GetActualLineAlignment() {
			if(IsRightToLeft && LineOrientation == Orientation.Vertical && LineAlignment != Alignment.Center)
				return LineAlignment == Alignment.Far ? Alignment.Near : Alignment.Far;
			return LineAlignment;
		}
		#region EventHandlers
		void OnStyleChanged(object sender, EventArgs e) {
			UpdateStyle();
			UpdateLineWidth();
			LayoutChanged();
		}
		#endregion
		#region ISupportLookAndFeel Members
		bool ISupportLookAndFeel.IgnoreChildren {
			get { return true; }
		}
		#endregion
		#region Hiding Properties
		[Browsable(false)]
		public new Font Font { get; set; }
		[Browsable(false)]
		public new Color ForeColor { get; set; }
		[Browsable(false)]
		public new String Text { get; set; }
		#endregion
		#region ISupportInitialize Members
		int initializeCount;
		protected bool IsInitializing {
			get { return initializeCount != 0; }
		}
		void ISupportInitialize.BeginInit() {
			initializeCount++;
		}
		void ISupportInitialize.EndInit() {
			initializeCount--;
			if(!IsInitializing) {
				OnResizeCore();
			}
		}
		#endregion
	}
	public enum Alignment {
		Default, Near, Center, Far
	}
	public class SeparatorControlInfo : ObjectInfoArgs {
		SeparatorControl ownerCore;
		Rectangle clientBoundsCore;
		public SeparatorControlInfo(SeparatorControl owner) {
			ownerCore = owner;
		}
		public bool IsReady { get; private set; }
		protected Alignment LineAlignment { get { return Owner.GetActualLineAlignment(); } }
		public DashStyle LineStyle { get { return Owner.LineStyle; } }
		public Padding Padding { get { return Owner.Padding; } }
		public bool IsHorizontal { get { return Owner.LineOrientation == Orientation.Horizontal; } }
		public int LineThickness { get { return Owner.LineThickness; } }
		public Color LineColor { get { return Owner.LineColor; } }
		protected Size SavedSize { get; set; }
		public virtual Rectangle ClientBounds { get { return clientBoundsCore; } }
		public Rectangle LineBounds { get; set; }
		protected virtual SeparatorControl Owner { get { return ownerCore; } }
		protected virtual Rectangle CalcClientBounds(Rectangle bounds) {
			return Owner.Painter.GetObjectClientRectangle(this);
		}
		int lockUpdateCount;
		protected void BeginUpdate(){
			lockUpdateCount++;
		}
		protected bool UpdateLocked { get { return lockUpdateCount != 0; } }
		protected void EndUpdate() {
			lockUpdateCount--;
			if(!UpdateLocked)
				Calc(Bounds);
		}
		protected void CancelUpdate() {
			lockUpdateCount--;
		}
		public void Calc(Rectangle bounds) {
			if(UpdateLocked) return;
			Bounds = bounds;
			clientBoundsCore = CalcClientBounds(bounds);
			if(Owner.AutoSizeMode) {
				CalcByAutoWidth();
				return;
			}
			CalcCore();
		}
		protected virtual Point GetLineLocation() {
			int clientSizeValue = IsHorizontal ? ClientBounds.Height : ClientBounds.Width;
			int offset = 0;
			switch(LineAlignment) {
				case Alignment.Far:
					offset = clientSizeValue - LineThickness;
					break;
				case Alignment.Center:
				case Alignment.Default:
					offset = (int)((clientSizeValue - LineThickness) / 2.0 + 0.5);
					break;
			}
			return IsHorizontal ? new Point(ClientBounds.X, ClientBounds.Y + offset) : new Point(ClientBounds.X + offset, ClientBounds.Y);
		}
		protected virtual void CalcCore() {
			if(ClientBounds.Size.Width <= 0 || ClientBounds.Size.Height < 0) {
				LineBounds = Rectangle.Empty;
				return;
			}
			Point lineLocation = GetLineLocation();
			Size lineSize = IsHorizontal ? new Size(ClientBounds.Width, LineThickness) : new Size(LineThickness, ClientBounds.Height);
			LineBounds = new Rectangle(lineLocation, lineSize);
			IsReady = true;
		}
		protected virtual void CalcByAutoWidth() {
			Size savedClientSize = Owner.Painter.GetObjectClientRectangle(this, new Rectangle(Point.Empty, SavedSize)).Size;
			Size lineSize = IsHorizontal ? new Size(savedClientSize.Width, LineThickness) : new Size(LineThickness, savedClientSize.Height);
			LineBounds = new Rectangle(ClientBounds.Location, lineSize);
			Size autoSize = Owner.Painter.CalcBoundsByClientRectangle(this, LineBounds).Size;
			BeginUpdate();
			Owner.Bounds = new Rectangle(Owner.Location, autoSize);
			CancelUpdate();
			IsReady = true;
		}
		protected virtual Size GetActualSize(){
			return Owner.Bounds.Size;
		}
		protected internal virtual void UpdateSavedSize() {
			Size actualSize = GetActualSize();
			if(SavedSize.IsEmpty) {
				SavedSize = actualSize;
				return;
			}
			if(IsHorizontal)
				SavedSize = new Size(actualSize.Width, SavedSize.Height);
			else
				SavedSize = new Size(SavedSize.Width, actualSize.Height);
		}
		protected internal virtual void RestoreSavedSize() {
			if(!SavedSize.IsEmpty) {
				Size oldSize = SavedSize;
				SavedSize = Size.Empty;
				Owner.Size = oldSize;
			}
		}
		public void SetDirty() {
			IsReady = false;
		}
	}
	public class SeparatorControlPainter : ObjectPainter {
		public SeparatorControlPainter() { }
		public virtual int DefaultLineThickness { get { return 1; } }
		public virtual Color DefaultLineColor { get { return SystemColors.ControlDark; } }
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return GetObjectClientRectangle(e, e.Bounds);
		}
		public virtual Rectangle GetObjectClientRectangle(ObjectInfoArgs e, Rectangle bounds) {
			SeparatorControlInfo info = e as SeparatorControlInfo;
			Rectangle rect = bounds;
			rect.Offset(info.Padding.Left, info.Padding.Top);
			rect.Size = new Size(rect.Width - info.Padding.Horizontal, rect.Height - info.Padding.Vertical);
			return rect;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			SeparatorControlInfo info = e as SeparatorControlInfo;
			Padding margins = info.Padding;
			Rectangle bounds = client;
			bounds.Offset(-margins.Left, -margins.Top);
			bounds.Size = new Size(client.Width + margins.Horizontal, client.Height + margins.Vertical);
			return bounds;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DrawLineRectangle(e as SeparatorControlInfo);
		}
		protected virtual void DrawLineRectangle(SeparatorControlInfo info) {
			int offset = (int)(info.LineThickness / 2.0 + 0.5);
			Color color = info.LineColor.IsEmpty ? DefaultLineColor : info.LineColor;
			using(Pen pen = new Pen(color, info.LineThickness) { DashStyle = info.LineStyle }) {
				if(info.IsHorizontal)
					info.Graphics.DrawLine(pen, info.LineBounds.X, info.LineBounds.Y + offset, info.LineBounds.Right, info.LineBounds.Y + offset);
				else
					info.Graphics.DrawLine(pen, info.LineBounds.X + offset, info.LineBounds.Y, info.LineBounds.X + offset, info.LineBounds.Bottom);
			}
		}
	}
	public class SeparatorControlSkinPainter : SeparatorControlPainter {
		ISkinProvider skinProviderCore;
		public SeparatorControlSkinPainter(ISkinProvider skinProvider)
			: base() {
			skinProviderCore = skinProvider;
		}
		public override int DefaultLineThickness {
			get {
				SkinElement element = GetSkinElement();
				if(element.Image == null){
					if(element.Border != null)
						return element.Border.Thin.Height;
					return 2;
				}
				return element.Image.Image.Size.Height;
			}
		}
		protected ISkinProvider SkinProvider {
			get { return skinProviderCore; }
		}
		protected override void DrawLineRectangle(SeparatorControlInfo info) {
			if(!info.LineColor.IsEmpty) {
				base.DrawLineRectangle(info);
				return;
			}
			RotateObjectPaintHelper rotateHelper = new RotateObjectPaintHelper();
			rotateHelper.DrawRotated(info.Cache, GetSkinElementInfo(info), SkinElementPainter.Default, GetRotateFlipType(info));
		}
		protected RotateFlipType GetRotateFlipType(SeparatorControlInfo info) {
			if(!info.IsHorizontal)
				return RotateFlipType.Rotate270FlipNone;
			return RotateFlipType.RotateNoneFlipNone;
		}
		protected virtual SkinElementInfo GetSkinElementInfo(SeparatorControlInfo info) {
			return new SkinElementInfo(GetSkinElement(), info.LineBounds);
		}
		protected virtual Skin GetSkin() {
			return CommonSkins.GetSkin(SkinProvider);
		}
		protected virtual SkinElement GetSkinElement() {
			return GetSkin()[CommonSkins.SkinLabelLine];
		}
	}
}
