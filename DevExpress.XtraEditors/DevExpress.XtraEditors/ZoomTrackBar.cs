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
using System.Text;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors.Controls;
using DevExpress.Accessibility;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Design;
using System.Drawing.Drawing2D;
using DevExpress.Utils.WXPaint;
using DevExpress.XtraPrinting;
using System.Windows.Forms.Layout;
using System.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Text;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public enum ScrollThumbStyle { ArrowUpLeft, ArrowDownRight, Bar }
	public class RepositoryItemZoomTrackBar : RepositoryItemTrackBar {
		private static readonly object buttonPressed = new object();
		private static readonly object buttonClick = new object();
		EditorButton zoomInButton, zoomOutButton;
		bool allowKeyboardNavigation = true;
		public RepositoryItemZoomTrackBar() : base() {
			middleCore = (Maximum + Minimum) / 2;
		}
		ScrollThumbStyle TickToThumbStyle(TickStyle st) {
			if(st == TickStyle.TopLeft) return ScrollThumbStyle.ArrowUpLeft;
			if(st == TickStyle.Both) return ScrollThumbStyle.Bar;
			return ScrollThumbStyle.ArrowDownRight;
		}
		TickStyle ThumbToTickStyle(ScrollThumbStyle st) {
			if(st == ScrollThumbStyle.ArrowDownRight) return TickStyle.BottomRight;
			if(st == ScrollThumbStyle.ArrowUpLeft) return TickStyle.TopLeft;
			return TickStyle.Both;
		}
		[SmartTagProperty("ScrollThumbStyle", "")]
		public virtual ScrollThumbStyle ScrollThumbStyle { get { return TickToThumbStyle(TickStyle); } set { TickStyle = ThumbToTickStyle(value); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TickStyle TickStyle { get { return base.TickStyle; } set { base.TickStyle = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowKeyboardNavigation { get { return allowKeyboardNavigation; } set { allowKeyboardNavigation = value; } }
		[Browsable(false)]
		public override string EditorTypeName { get { return "ZoomTrackBarControl"; } }
		protected new ZoomTrackBarControl OwnerEdit { get { return base.OwnerEdit as ZoomTrackBarControl; } }
		protected internal new ZoomTrackBarPainter CreatePainter() { return new ZoomTrackBarPainter(); }
		protected internal new ZoomTrackBarViewInfo CreateViewInfo() { return new ZoomTrackBarViewInfo(this); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int TickFrequency { get { return base.TickFrequency; } set { base.TickFrequency = value; } }
		bool allowUseMiddleValue;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemZoomTrackBarAllowUseMiddleValue"),
#endif
 RefreshProperties(RefreshProperties.All), DXCategory(CategoryName.Behavior), DefaultValue(false)]
		public bool AllowUseMiddleValue {
			get { return this.allowUseMiddleValue; }
			set {
				if (AllowUseMiddleValue == value) return;
				this.allowUseMiddleValue = value;
				if (value && !IsMiddleValidCore(Middle)) RefreshMiddleCore(); 
				OnPropertiesChanged();
			}
		}
		int middleCore;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemZoomTrackBarMiddle"),
#endif
 RefreshProperties(RefreshProperties.All), DXCategory(CategoryName.Behavior)]
		public int Middle {
			get { return this.middleCore; }
			set {
				if (Middle == value || !AllowUseMiddleValue) return;
				if(!IsLoading)
					value = ConstraintMiddleValue(value);
				this.middleCore = value;
				OnPropertiesChanged();
			}
		}
		protected internal int ConstraintMiddleValue(int value) {
			if(value <= Minimum || value >= Maximum)
				return (Minimum + Maximum) / 2;
			return value;
		}
		int snapToMiddle;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemZoomTrackBarSnapToMiddle"),
#endif
 DXCategory(CategoryName.Behavior), DefaultValue(0)]
		public int SnapToMiddle {
			get { return this.snapToMiddle; }
			set {
				if (SnapToMiddle == value) return;
				this.snapToMiddle = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TrackBarLabelCollection Labels {
			get { return base.Labels; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowLabels {
			get { return base.ShowLabels; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowLabelsForHiddenTicks {
			get { return base.ShowLabelsForHiddenTicks; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int DistanceFromTickToLabel {
			get { return base.DistanceFromTickToLabel; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceObject LabelAppearance {
			get { return base.LabelAppearance; }
		}
		protected virtual void RefreshMiddleCore() {
			if (AllowUseMiddleValue && Maximum - Minimum <= 1){
				AllowUseMiddleValue = false;
				return;
			}
			Middle = (Maximum + Minimum) / 2;
		}
		protected virtual bool IsMiddleValidCore(int value) {
			if(IsLoading)
				return true;
			return value < Maximum && value > Minimum;
		}
		public override int Minimum {
			set {
				base.Minimum = value;
				if(!IsMiddleValidCore(Middle)) RefreshMiddleCore();
			}
		}
		public override int Maximum {
			set {
				base.Maximum = value;
				if(!IsMiddleValidCore(Middle)) RefreshMiddleCore();
			}
		}
		public override void Assign(RepositoryItem item) {
			base.Assign(item);
			RepositoryItemZoomTrackBar zoomItem = item as RepositoryItemZoomTrackBar;
			if(zoomItem == null) return;
			this.AllowUseMiddleValue = zoomItem.AllowUseMiddleValue;
			this.Middle = zoomItem.Middle;
			this.ScrollThumbStyle = zoomItem.ScrollThumbStyle;
			this.TickFrequency = zoomItem.TickFrequency;
			this.TickStyle = zoomItem.TickStyle;
			this.allowKeyboardNavigation = zoomItem.allowKeyboardNavigation;
			this.SnapToMiddle = zoomItem.SnapToMiddle;
			Events.AddHandler(buttonClick, zoomItem.Events[buttonClick]);
			Events.AddHandler(buttonPressed, zoomItem.Events[buttonPressed]);
		}
		protected virtual EditorButton CreateZoomInButton() { return new EditorButton(ButtonPredefines.Plus); }
		protected virtual EditorButton CreateZoomOutButton() { return new EditorButton(ButtonPredefines.Minus); }
		protected internal EditorButton ZoomInButton { 
			get {
				if(zoomInButton == null) zoomInButton = CreateZoomInButton();
				return zoomInButton; 
			} 
		}
		protected internal EditorButton ZoomOutButton { 
			get {
				if(zoomOutButton == null) zoomOutButton = CreateZoomOutButton();
				return zoomOutButton; 
			} 
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemButtonEditButtonClick"),
#endif
 Category(CategoryName.Events)]
		public event ButtonPressedEventHandler ButtonClick {
			add { this.Events.AddHandler(buttonClick, value); }
			remove { this.Events.RemoveHandler(buttonClick, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemButtonEditButtonPressed"),
#endif
 Category(CategoryName.Events)]
		public event ButtonPressedEventHandler ButtonPressed {
			add { this.Events.AddHandler(buttonPressed, value); }
			remove { this.Events.RemoveHandler(buttonPressed, value); }
		}
		protected internal virtual void RaiseButtonClick(ButtonPressedEventArgs e) {
			ButtonPressedEventHandler handler = (ButtonPressedEventHandler)this.Events[buttonClick];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseButtonPressed(ButtonPressedEventArgs e) {
			ButtonPressedEventHandler handler = (ButtonPressedEventHandler)this.Events[buttonPressed];
			if(handler != null) handler(GetEventSender(), e);
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public enum ZoomTrackBarHitTest { None, ZoomInButton, ZoomOutButton, ZoomThumb }
	public class ZoomTrackBarHitInfo : EditHitInfo {
		ZoomTrackBarHitTest zoomHitTest;
		public ZoomTrackBarHitInfo() {
			this.zoomHitTest = ZoomTrackBarHitTest.None;
		}
		public ZoomTrackBarHitInfo(Point pt) : base(pt) { }
		public ZoomTrackBarHitTest ZoomHitTest { get { return zoomHitTest; } set { zoomHitTest = value; } }
		public override void Assign(EditHitInfo hitInfo) {
			base.Assign(hitInfo);
			ZoomTrackBarHitInfo zoomInfo = hitInfo as ZoomTrackBarHitInfo;
			if(zoomInfo == null) return;
			this.ZoomHitTest = zoomInfo.ZoomHitTest;
		}
	}
	public class ZoomTrackBarViewInfo : TrackBarViewInfo {
		public static int StandAloneZoomThumbCriticalHeight = 16;
		public static int StandAloneZoomTrackLineCriticalHeight = 16;
		public static int GridZoomThumbCriticalHeight = 16;
		public static int GridZoomTrackLineCriticalHeight = 16;
		static int ZoomPreferredDimension = 18;
		EditorButtonObjectInfoArgs zoomInButtonInfo, zoomOutButtonInfo;
		EditorButtonPainter buttonPainter;
		int minHeight;
		Rectangle zoomInButtonClientRectangle;
		Rectangle zoomOutButtonClientRectangle;
		public ZoomTrackBarViewInfo(RepositoryItem item) : base(item) {
			this.zoomInButtonClientRectangle = Rectangle.Empty;
			this.zoomOutButtonClientRectangle = Rectangle.Empty;
		}
		public override void Offset(int x, int y) {
			base.Offset(x, y);
			Rectangle r = ZoomInButtonInfo.Bounds;
			r.Offset(x, y);
			ZoomInButtonInfo.Bounds = r;
			ZoomInButtonClientRectangle.Offset(x, y);
			r = ZoomOutButtonInfo.Bounds;
			r.Offset(x, y);
			ZoomOutButtonInfo.Bounds = r;
			ZoomOutButtonClientRectangle.Offset(x, y);
		}
		public override int ThumbCriticalHeight {
			get {
				if(InplaceType == InplaceType.Standalone)
					return StandAloneZoomThumbCriticalHeight;
				return GridZoomThumbCriticalHeight;
			}
		}
		public override int TrackLineCriticalHeight {
			get {
				if(InplaceType == InplaceType.Standalone)
					return StandAloneZoomTrackLineCriticalHeight;
				return GridZoomTrackLineCriticalHeight;
			}
		}
		protected override Point CalcThumbPosCore(int val) {
			Point pt = base.CalcThumbPosCore(val);
			if (!Item.AllowUseMiddleValue) {
				if (TickStyle == TickStyle.TopLeft) pt.Y++;
				return pt;
			}
			int distanceToTick;
			if(this.Value <= Item.Middle) {
				distanceToTick = (int)((float)PointsRect.Width / 2 * ((float)(val - Item.Minimum) / (float)(Item.Middle - Item.Minimum)) + 0.5f);
			}
			else {
				distanceToTick = (int)(float)PointsRect.Width / 2 + (int)((float)PointsRect.Width / 2 * ((float)(val - Item.Middle) / (float)(Item.Maximum - Item.Middle)) + 0.5f);
			}
			pt.X = IsRightToLeft ? PointsRect.Right - distanceToTick : PointsRect.Left + distanceToTick;
			return pt;
		}
		public override int ValueFromPoint(Point p) {
			if (!Item.AllowUseMiddleValue) return base.ValueFromPoint(p);
			int distanceToTick = IsRightToLeft ? PointsRect.Right - (p.X - PointsRect.Left) : p.X;
			if(distanceToTick <= PointsRect.Left + (float)PointsRect.Width / 2) return Item.Minimum + (int)((((float)Item.Middle - (float)Item.Minimum) * ((float)distanceToTick - (float)PointsRect.Left) / ((float)PointsRect.Width / 2)) + 0.5f);
			return Item.Middle + (int)((((float)Item.Maximum - (float)Item.Middle) * ((float)distanceToTick - (float)PointsRect.Left - (float)PointsRect.Width / 2) / ((float)PointsRect.Width / 2)) + 0.5f);
		}
		protected new ZoomTrackBarControl OwnerControl { get { return base.OwnerControl as ZoomTrackBarControl; } }
		public override bool AllowDrawFocusRect {
			get { return false; }
			set { }
		}
		[EditorPainterActivator(typeof(SkinZoomTrackBarObjectPainter), typeof(TrackBarObjectPainter))]
		public override TrackBarObjectPainter GetTrackPainter() {
			if(IsPrinting)
				return new ZoomTrackBarObjectPainter();
			if(this.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.WindowsXP)
				return new ZoomTrackBarObjectPainter();
			if(this.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				return new SkinZoomTrackBarObjectPainter(LookAndFeel);
			if(this.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Office2003)
				return new Office2003ZoomTrackBarObjectPainter();
			return new ZoomTrackBarObjectPainter();
		}
		protected new ZoomTrackBarInfoCalculator TrackCalculator { get { return base.TrackCalculator as ZoomTrackBarInfoCalculator; } }
		public new RepositoryItemZoomTrackBar Item { get { return base.Item as RepositoryItemZoomTrackBar; } }
		public override EditHitInfo CalcHitInfo(Point p) {
			ZoomTrackBarHitInfo hitInfo = new ZoomTrackBarHitInfo(p);
			if(!ShouldUpdateState) {
				hitInfo.ZoomHitTest = ZoomTrackBarHitTest.ZoomThumb;
				hitInfo.SetHitTest(EditHitTest.Button);
				return hitInfo;
			}
			if(ThumbBounds.Contains(p)) hitInfo.ZoomHitTest = ZoomTrackBarHitTest.ZoomThumb;
			else if(ZoomInButtonClientRectangle.Contains(p)) {
				hitInfo.ZoomHitTest = ZoomTrackBarHitTest.ZoomInButton;
				hitInfo.SetHitObject(ZoomInButtonInfo);
			}
			else if(ZoomOutButtonClientRectangle.Contains(p)) {
				hitInfo.ZoomHitTest = ZoomTrackBarHitTest.ZoomOutButton;
				hitInfo.SetHitObject(ZoomOutButtonInfo);
			}
			if(hitInfo.ZoomHitTest == ZoomTrackBarHitTest.ZoomThumb) hitInfo.SetHitTest(EditHitTest.Button);
			else if(hitInfo.ZoomHitTest != ZoomTrackBarHitTest.None) hitInfo.SetHitTest(EditHitTest.Button2);
			return hitInfo;
		}
		protected override bool UpdateObjectState() {
			ZoomTrackBarHitTest prevHitTest = HotInfo != null ? HotInfo.ZoomHitTest : ZoomTrackBarHitTest.None;
			bool res = base.UpdateObjectState();
			ZoomTrackBarHitTest newHitTest = HotInfo != null? HotInfo.ZoomHitTest : ZoomTrackBarHitTest.None;
			return res || prevHitTest != newHitTest;
		}
		protected override bool IsHotEdit(EditHitInfo hitInfo) {
			ZoomTrackBarHitInfo zoomInfo = hitInfo as ZoomTrackBarHitInfo;
			if(zoomInfo == null) return base.IsHotEdit(hitInfo);
			return zoomInfo.ZoomHitTest != ZoomTrackBarHitTest.None;
		}
		public ZoomTrackBarObjectPainter ZoomPainter { get { return base.TrackPainter as ZoomTrackBarObjectPainter; } }
		public EditorButtonPainter ButtonPainter { get { return buttonPainter; } }
		protected override void UpdatePainters() {
			base.UpdatePainters();
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				this.buttonPainter = new ZoomTrackBarSkinButtonPainter(LookAndFeel.Painter.Button, LookAndFeel);
			else
				this.buttonPainter = EditorButtonHelper.GetPainter(Item.BorderStyle, LookAndFeel);
		}
		protected internal EditorButtonObjectInfoArgs ZoomInButtonInfo { 
			get {
				if(zoomInButtonInfo == null) this.zoomInButtonInfo = new EditorButtonObjectInfoArgs(Item.ZoomInButton, PaintAppearance);
				return zoomInButtonInfo; 
			} 
		}
		protected internal EditorButtonObjectInfoArgs ZoomOutButtonInfo { 
			get {
				if(zoomOutButtonInfo == null) this.zoomOutButtonInfo = new EditorButtonObjectInfoArgs(Item.ZoomOutButton, PaintAppearance);	
				return zoomOutButtonInfo; 
			} 
		}
		public override Point[] RectThumbRegion {
			get {
				TrackBarObjectPainter pt = TrackPainter;
				int[,] offsetP1 = { { -pt.GetThumbBestWidth(this) / 2, 7 }, { -pt.GetThumbBestWidth(this) / 2, -6 }, { pt.GetThumbBestWidth(this) / 2, -6 }, { pt.GetThumbBestWidth(this) / 2, 7 }, { -pt.GetThumbBestWidth(this) / 2, 7 } };
				Point[] polygon = new Point[5];
				TransformPoints(offsetP1, polygon, ThumbPos);
				return polygon;
			}
		}
		public override Point[] ArrowThumbRegion {
			get {
				TrackBarObjectPainter pt = TrackPainter;
				int[,] offsetP1 = { { 0, 8 }, { -pt.GetThumbBestWidth(this) / 2, 2 }, { -pt.GetThumbBestWidth(this) / 2, -8 }, { pt.GetThumbBestWidth(this) / 2, -8 }, { pt.GetThumbBestWidth(this) / 2, 2 }, { 0, 7 } };
				Point[] polygon = new Point[6];
				TransformPoints(offsetP1, polygon, ThumbPos);
				return polygon;
			}
		}
		protected override void CalcTrackInfoCore() {
			this.minHeight = CalcMinHeight(GInfo.Graphics);
			CalcTrackButtons();
			base.CalcTrackInfoCore();
			CalcButtonsState();
		}
		internal virtual int MinHeight { get { return minHeight; } }
		public new ZoomTrackBarHitInfo HotInfo {
			get { return base.HotInfo as ZoomTrackBarHitInfo; }
			set { base.HotInfo = value; }
		}
		protected virtual void CalcButtonsState() { 
			ZoomInButtonInfo.State = Enabled? ObjectState.Normal: ObjectState.Disabled;
			ZoomOutButtonInfo.State = Enabled? ObjectState.Normal: ObjectState.Disabled;
			if(!Enabled) return;
			ZoomTrackBarHitInfo pInfo = PressedInfo as ZoomTrackBarHitInfo;
			if(pInfo != null) {
				if(pInfo.ZoomHitTest == ZoomTrackBarHitTest.ZoomInButton) ZoomInButtonInfo.State = ObjectState.Pressed;
				if(pInfo.ZoomHitTest == ZoomTrackBarHitTest.ZoomOutButton) ZoomOutButtonInfo.State = ObjectState.Pressed;
			}
			if(HotInfo != null) {
				if(HotInfo.ZoomHitTest == ZoomTrackBarHitTest.ZoomInButton && ZoomInButtonInfo.State == ObjectState.Normal) ZoomInButtonInfo.State = ObjectState.Hot;
				if(HotInfo.ZoomHitTest == ZoomTrackBarHitTest.ZoomOutButton && ZoomOutButtonInfo.State == ObjectState.Normal) ZoomOutButtonInfo.State = ObjectState.Hot;
			}
		}
		protected internal Rectangle ZoomInButtonClientRectangle {
			get { return zoomInButtonClientRectangle; }
			set { zoomInButtonClientRectangle = value; }
		}
		protected internal Rectangle ZoomOutButtonClientRectangle {
			get { return zoomOutButtonClientRectangle; }
			set { zoomOutButtonClientRectangle = value; }
		}
		protected override void CalcTrackButtons() {
			ZoomInButtonInfo.Bounds = TrackCalculator.CalcZoomInButtonRect();
			ZoomOutButtonInfo.Bounds = TrackCalculator.CalcZoomOutButtonRect();
			ZoomInButtonClientRectangle = TrackCalculator.CalcZoomInButtonClientRect();
			ZoomOutButtonClientRectangle = TrackCalculator.CalcZoomOutButtonClientRect();
		}
		protected override void CalcContentRect(Rectangle bounds) {
			fContentRect = bounds;
		}
		protected override int CalcMinHeightCore(Graphics g) {
			CalcTrackButtons();
			UpdatePainters();
			TrackCalculator.CalcBestHeights();
			int height = Math.Max(ThumbBounds.Height, TrackCalculator.ThumbHeight) + TrackCalculator.RealDistanceFromTopToThumb + TrackCalculator.RealDistanceFromTicksToBottom;
			height = Math.Max(height, ZoomInButtonClientRectangle.Height);
			return BorderPainter.CalcBoundsByClientRectangle(new BorderObjectInfoArgs(null, new Rectangle(0, 0, 0, height), null)).Height;
		}
		bool inPreferredDimension = false;
		protected internal override int PreferredDimension {
			get {
				if(inPreferredDimension)
					return ZoomPreferredDimension;
				try {
					inPreferredDimension = true;
					if(OwnerEdit != null && !inPreferredDimension) {
						return OwnerEdit.CalcMinHeight();
					}
					return ZoomPreferredDimension;
				}
				finally {
					inPreferredDimension = false;
				}
			}
		}
	}
	public class ZoomTrackBarInfoCalculator : TrackBarInfoCalculator {
		public ZoomTrackBarInfoCalculator(ZoomTrackBarViewInfo viewInfo, ZoomTrackBarObjectPainter painter) : base(viewInfo, painter) { }
		public new ZoomTrackBarViewInfo ViewInfo { get { return base.ViewInfo as ZoomTrackBarViewInfo; } }
		public new ZoomTrackBarObjectPainter TrackPainter { get { return base.TrackPainter as ZoomTrackBarObjectPainter; } }
		protected internal override int CalcBestContentHeight() { return ThumbHeight; }
		protected virtual int PointsRectRightIndent { get { return 1; } }
		protected internal override Rectangle CalcPointsRect() {
			Rectangle pointsRect = base.CalcPointsRect();
			pointsRect.Height = 0;
			pointsRect.Width -= PointsRectRightIndent;
			return pointsRect;
		}
		protected internal override int CalcBestHeight() { return ViewInfo.ThumbCriticalHeight; }
		protected internal override void CalcBestHeights() {
			RealDistanceFromTopToThumb = 1;
			RealThumbHeight = ThumbHeight;
			RealDistanceFromThumbToTicks = 0;
			RealTickHeight = 0;
			RealDistanceFromTicksToBottom = 1;
		}
		protected override int ApplyAlignment(int trackLineY, TrackBarHelper tb) {
			if(tb.ClientHeight <= ViewInfo.PreferredDimension) return trackLineY;
			if(ViewInfo.Item.Alignment == VertAlignment.Center || ViewInfo.Item.Alignment == VertAlignment.Default) 
				trackLineY = trackLineY + (tb.ClientHeight - ActualClientHeight)/ 2;
			else if(ViewInfo.Item.Alignment == VertAlignment.Bottom) 
				trackLineY = tb.ClientHeight - ActualClientHeight + trackLineY;
			return trackLineY;
		}
		protected internal override Rectangle CalcTrackLineRect() {
			int indent = GetButtonIndent();
			Rectangle rect = Rectangle.Empty;
			Rectangle leftButton = ViewInfo.IsRightToLeft ? ViewInfo.ZoomInButtonClientRectangle : ViewInfo.ZoomOutButtonClientRectangle;
			Rectangle rightButton = ViewInfo.IsRightToLeft ? ViewInfo.ZoomOutButtonClientRectangle : ViewInfo.ZoomInButtonClientRectangle;
			if(ViewInfo.Orientation == Orientation.Horizontal) {
				rect = new Rectangle(leftButton.Right + indent, ViewInfo.ContentRect.Y + (ActualClientHeight - RealTrackLineHeight) / 2, rightButton.X - leftButton.Right - indent * 2, RealTrackLineHeight);
			}
			else {
				rect = new Rectangle(rightButton.Bottom + indent + 1, ViewInfo.ContentRect.X + (ActualClientHeight - RealTrackLineHeight) / 2, leftButton.Y - rightButton.Bottom - indent * 2, RealTrackLineHeight);
			}
			rect.Y = ApplyAlignment(rect.Y, ViewInfo.TrackBarHelper);
			return rect;
		}
		protected internal override Size GetLineSize() {
			SkinElementInfo lineInfo = GetLineInfo();
			if(lineInfo == null || lineInfo.Element == null || lineInfo.Element.Image == null) return new Size(5, 4);
			return lineInfo.Element.Image.GetImageBounds(0).Size;
		}
		protected internal virtual Size GetZoomInButtonSize() { return new Size(16, 16); }
		protected internal virtual Size GetZoomOutButtonSize() { return new Size(16, 16); }
		protected internal virtual int GetButtonIndent() { return 0; }
		int GetButtonY(Rectangle buttonRect) {
			int height = ViewInfo.Item.Orientation == Orientation.Horizontal ? buttonRect.Height : buttonRect.Width;
			int contentY = ViewInfo.Item.Orientation == Orientation.Horizontal ? ViewInfo.ContentRect.Y : ViewInfo.ContentRect.X;
			int contentHeight = ViewInfo.Item.Orientation == Orientation.Horizontal ? ViewInfo.ContentRect.Height : ViewInfo.ContentRect.Width;
			if(ViewInfo.Item.Alignment == VertAlignment.Center || ViewInfo.Item.Alignment == VertAlignment.Default)
				return contentY + (contentHeight - height) / 2;
			else if(ViewInfo.Item.Alignment == VertAlignment.Bottom)
				return contentY + contentHeight - (ActualClientHeight + height) / 2;
			return contentY + (ActualClientHeight - height) / 2;
		}
		protected internal virtual Rectangle CalcZoomOutButtonRect() {
			Rectangle rect = new Rectangle(Point.Empty, GetZoomOutButtonSize());
			if(ViewInfo.IsRightToLeft) rect.X = ViewInfo.Bounds.X + ViewInfo.TrackBarHelper.ContentRectangle.Right - rect.Width;
			else rect.X = ViewInfo.ContentRect.X;
			rect.Y = GetButtonY(rect);
			if(ViewInfo.IsTouchMode) {
				if(ViewInfo.IsRightToLeft) rect.X -= CalcTouchAddedSize(rect.Size).Width / 2;
				else rect.X += CalcTouchAddedSize(rect.Size).Width / 2;
			}
			return ViewInfo.TrackBarHelper.Rotate(rect);
		}
		protected internal virtual Rectangle CalcZoomInButtonRect() {
			Rectangle rect = new Rectangle(Point.Empty, GetZoomInButtonSize()); 
			if(ViewInfo.IsRightToLeft) rect.X = ViewInfo.ContentRect.X;
			else rect.X = ViewInfo.Bounds.X + ViewInfo.TrackBarHelper.ContentRectangle.Right - rect.Width;
			rect.Y = GetButtonY(rect);
			if(ViewInfo.IsTouchMode) {
				if(ViewInfo.IsRightToLeft) rect.X += CalcTouchAddedSize(rect.Size).Width / 2;
				else rect.X -= CalcTouchAddedSize(rect.Size).Width / 2;
			}
			return ViewInfo.TrackBarHelper.Rotate(rect);
		}
		protected internal virtual Rectangle CalcZoomInButtonClientRect() {
			Rectangle rect = CalcZoomInButtonRect();
			return CalcZoomButtonClientRect(rect);
		}
		protected internal virtual Rectangle CalcZoomOutButtonClientRect() {
			Rectangle rect = CalcZoomOutButtonRect();
			return CalcZoomButtonClientRect(rect);
		}
		protected Rectangle CalcZoomButtonClientRect(Rectangle rect) {
			if(!ViewInfo.IsTouchMode) return rect;
			Size addedSize = CalcTouchAddedSize(rect.Size);
			return new Rectangle(rect.X - addedSize.Width / 2, rect.Y - addedSize.Height / 2, rect.Width + addedSize.Width, rect.Height + addedSize.Height);
		}
	}
	public class ZoomTrackBarSkinInfoCalculator : ZoomTrackBarInfoCalculator {
		public ZoomTrackBarSkinInfoCalculator(ZoomTrackBarViewInfo viewInfo, SkinZoomTrackBarObjectPainter painter) : base(viewInfo, painter) { }
		public new ZoomTrackBarViewInfo ViewInfo { get { return base.ViewInfo as ZoomTrackBarViewInfo; } }
		public new SkinZoomTrackBarObjectPainter TrackPainter { get { return base.TrackPainter as SkinZoomTrackBarObjectPainter; } }
		protected internal override SkinElementInfo GetLineInfo() { return new SkinElementInfo(EditorsSkins.GetSkin(ViewInfo.LookAndFeel)[EditorsSkins.SkinTrackBarTrack]); }
		public override int ThumbHeight { get { return GetSkinThumbElementOriginSize().Height; } }
		public override int RealTrackLineHeight { get { return GetLineSize().Height; } }
		protected override int PointsRectRightIndent { get { return 0; } }
		protected override int ActualClientHeight { get { return Math.Min(ViewInfo.TrackBarHelper.ClientHeight, ViewInfo.MinHeight); } }
		protected override int ActualContentBottom {
			get {
				TrackBarHelper tb = ViewInfo.TrackBarHelper;
				return Math.Min(tb.ContentRectangle.Bottom, tb.ClientRectangle.Top + ViewInfo.MinHeight - (tb.ClientRectangle.Bottom - tb.ContentRectangle.Bottom));
			}
		}
		protected internal override int GetButtonIndent() {
			Skin skin = SkinManager.Default.GetSkin(SkinProductId.Editors, ViewInfo.LookAndFeel);
			if(skin == null) return base.GetButtonIndent();
			return skin.Properties.GetInteger("ZoomButtonIndent");
		}
		public override SkinElementInfo GetThumbElementInfo() {
			if(ViewInfo.TickStyle == TickStyle.Both)
				return new SkinElementInfo(EditorsSkins.GetSkin(ViewInfo.LookAndFeel)[EditorsSkins.SkinTrackBarThumbBoth], Rectangle.Empty);
			return new SkinElementInfo(EditorsSkins.GetSkin(ViewInfo.LookAndFeel)[EditorsSkins.SkinTrackBarThumb], Rectangle.Empty);
		}
		protected internal override Size GetZoomInButtonSize() {
			ZoomTrackBarSkinButtonPainter painter = ViewInfo.ButtonPainter as ZoomTrackBarSkinButtonPainter;
			if(painter != null) {
				SkinElementInfo info = painter.GetButtonElementInfo(ViewInfo.ZoomInButtonInfo);
				if(info != null && info.Element != null && info.Element.Image != null) return info.Element.Image.GetImageBounds(0).Size;
			}
			return base.GetZoomInButtonSize();
		}
		protected internal override Size GetZoomOutButtonSize() {
			ZoomTrackBarSkinButtonPainter painter = ViewInfo.ButtonPainter as ZoomTrackBarSkinButtonPainter;
			if(painter != null) {
				SkinElementInfo info = painter.GetButtonElementInfo(ViewInfo.ZoomOutButtonInfo);
				if(info != null && info.Element != null && info.Element.Image != null) return info.Element.Image.GetImageBounds(0).Size;
			}
			return base.GetZoomOutButtonSize();
		}
		protected override int PointsRectOffsetX { get { return GetSkinThumbElementOffset().X; } }
		protected override int ThumbUpperPartHeight { get { return GetSkinThumbElementOffset().Y; } }
		public override Rectangle GetThumbBounds() {
			return GetSkinThumbBounds();
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class ZoomTrackBarPainter : TrackBarPainter { 
	}
	public class Office2003ZoomTrackBarObjectPainter : ZoomTrackBarObjectPainter {
		protected Brush GetFillBrush(TrackBarObjectInfoArgs e) {
			return new Office2003TrackBarThumbPainter().GetThumbFillBrush(e);
		}
		public override void FillThumb(TrackBarObjectInfoArgs e, bool bMirror, Point[] polygon, bool hot) {
			e.Graphics.FillPolygon(GetFillBrush(e), polygon);
		}
		protected virtual Color GetForeColor(TrackBarObjectInfoArgs e) {
			return new Office2003TrackBarThumbPainter().GetThumbBorderColor(e);
		}
		public override void DrawArrowThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			int xOffset = GetThumbBestWidth(e.ViewInfo) / 2;
			int[,] offsetP1 = { { 0, 7 }, { -xOffset, 2 }, { -xOffset, -8 }, { xOffset, -8 }, { xOffset, 2 } };
			int[,] offsetP2 = { { -xOffset, 2 }, { -xOffset, -8 }, { xOffset, -8 }, { xOffset, 2 }, { 0, 7 } };
			Color c = GetForeColor(e);
			Color[] colors = { c, c, c, c, c };
			DrawThumb(e, bMirror, offsetP1, offsetP2, colors, 5);
		}
		public override void DrawRectThumb(TrackBarObjectInfoArgs e) {
			int xOffset = GetThumbBestWidth(e.ViewInfo) / 2;
			int[,] offsetP1 = { { -xOffset, 7 }, { -xOffset, -8 }, { xOffset, -8 }, { xOffset, 7 } };
			int[,] offsetP2 = { { -xOffset, -8 }, { xOffset, -8 }, { xOffset, 7 }, { -xOffset, 7 } };
			Color c = GetForeColor(e);
			Color[] colors = { c, c, c, c };
			DrawThumb(e, false, offsetP1, offsetP2, colors, 4);
		}
		protected override Color GetTrackLineMiddleLineColor(TrackBarObjectInfoArgs e) {
			return Office2003Colors.Default[Office2003Color.Border];
		}
	}
	public class ZoomTrackBarObjectPainter : TrackBarObjectPainter {
		protected internal override TrackBarInfoCalculator GetCalculator(TrackBarViewInfo viewInfo) {
			return new ZoomTrackBarInfoCalculator(viewInfo as ZoomTrackBarViewInfo, this);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			DrawButtons(e);
		}
		protected virtual void DrawButtons(ObjectInfoArgs e) {
			TrackBarObjectInfoArgs te = e as TrackBarObjectInfoArgs;
			ZoomTrackBarViewInfo vi = te.ViewInfo as ZoomTrackBarViewInfo;
			vi.ZoomInButtonInfo.Cache = e.Cache;
			vi.ZoomOutButtonInfo.Cache = e.Cache;
			vi.ButtonPainter.DrawObject(vi.ZoomInButtonInfo);
			vi.ButtonPainter.DrawObject(vi.ZoomOutButtonInfo);
		}
		public override int GetThumbBestHeight(TrackBarViewInfo vi) { return vi.PreferredDimension; }
		public override void DrawPoints(TrackBarObjectInfoArgs e) { }
		public override void DrawArrowThumb(TrackBarObjectInfoArgs e, bool bMirror) {
			int xOffset = GetThumbBestWidth(e.ViewInfo) / 2;
			int[,] offsetP1 = { { 0, 7 }, { -xOffset, 2 }, { -xOffset, -8 }, { xOffset, -8 }, { xOffset, 2 }, { xOffset - 1, -7 }, { xOffset - 1, 2 } };
			int[,] offsetP2 = { { -xOffset, 2 }, { -xOffset, -8 }, { xOffset, -8 }, { xOffset, 2 }, { 0, 7 }, { xOffset - 1, 2 }, { 0, 6 } };
			Color[] colors = { SystemColors.ControlLightLight, SystemColors.ControlLightLight, SystemColors.ControlLightLight, SystemColors.ControlDarkDark, SystemColors.ControlDarkDark, SystemColors.ControlDark, SystemColors.ControlDark };
			DrawThumb(e, bMirror, offsetP1, offsetP2, colors, 7);
		}
		public override void DrawRectThumb(TrackBarObjectInfoArgs e) {
			int xOffset = GetThumbBestWidth(e.ViewInfo) / 2;
			int[,] offsetP1 = { { -xOffset, 7 }, { -xOffset, -8 }, { xOffset, -8 }, { xOffset, 7 }, { xOffset - 1, -7 }, { xOffset - 1, 6 } };
			int[,] offsetP2 = { { -xOffset, -8 }, { xOffset, -8 }, { xOffset, 7 }, { -xOffset, 7 }, { xOffset - 1, 6 }, { -(xOffset - 1), 6 } };
			Color[] colors = { SystemColors.ControlLightLight, SystemColors.ControlLightLight, SystemColors.ControlDarkDark, SystemColors.ControlDarkDark, SystemColors.ControlDark, SystemColors.ControlDark };
			DrawThumb(e, false, offsetP1, offsetP2, colors, 6);
		}
		protected virtual Color GetTrackLineMiddleLineColor(TrackBarObjectInfoArgs e) {
			return e.ViewInfo.BorderPainter.DefaultAppearance.BorderColor;
		}
		protected virtual void DrawTrackLineMiddleLine(TrackBarObjectInfoArgs e) {
			int x, y, y2;
			x = e.ViewInfo.TrackLineRect.X + e.ViewInfo.TrackLineRect.Width / 2;
			y = e.ViewInfo.TrackLineRect.Y - 3;
			y2 = e.ViewInfo.TrackLineRect.Bottom + 3;
			Point pt1 = e.ViewInfo.TrackBarHelper.Rotate(new Point(x, y));
			Point pt2 = e.ViewInfo.TrackBarHelper.Rotate(new Point(x, y2));
			e.Graphics.DrawLine(e.Cache.GetPen(GetTrackLineMiddleLineColor(e)), pt1, pt2);
		}
		protected override void DrawTrackLineCore(TrackBarObjectInfoArgs e, Rectangle bounds) {
			base.DrawTrackLineCore(e, bounds);
			DrawTrackLineMiddleLine(e);
		}
	}
	public class ZoomTrackBarSkinButtonPainter : EditorButtonPainter {
		ISkinProvider provider;
		public ZoomTrackBarSkinButtonPainter(ObjectPainter buttonPainter, ISkinProvider provider) : base(buttonPainter) {
			this.provider = provider;
		}
		public ISkinProvider Provider { get { return provider; } }
		protected virtual void UpdateSkinInfoState(SkinElementInfo skinInfo, EditorButtonObjectInfoArgs info) { 
			skinInfo.State = info.State;
			if(info.State == ObjectState.Hot) skinInfo.ImageIndex = 1;
			else if(info.State == ObjectState.Pressed) skinInfo.ImageIndex = 2;
			else if(info.State == ObjectState.Disabled) skinInfo.ImageIndex = 3;
		}
		protected internal virtual SkinElementInfo GetButtonElementInfo(EditorButtonObjectInfoArgs info) {
			SkinElementInfo skinInfo = null;
			if(info.Button.Kind == ButtonPredefines.Minus)
				skinInfo = new SkinElementInfo(EditorsSkins.GetSkin(Provider)[EditorsSkins.SkinZoomTrackBarZoomOutButton], info.Bounds);
			else if(info.Button.Kind == ButtonPredefines.Plus)
				skinInfo = new SkinElementInfo(EditorsSkins.GetSkin(Provider)[EditorsSkins.SkinZoomTrackBarZoomInButton], info.Bounds);
			if(skinInfo != null) {
				UpdateSkinInfoState(skinInfo, info);
			}
			return skinInfo;
		}
		protected override void DrawContent(ObjectInfoArgs e) { }
		protected override void DrawButton(ObjectInfoArgs e) {
			EditorButtonObjectInfoArgs buttonInfo = e as EditorButtonObjectInfoArgs;
			SkinElementInfo info = GetButtonElementInfo(buttonInfo);
			if(info == null) return;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
	}
	public class SkinZoomTrackBarObjectPainter : ZoomTrackBarObjectPainter { 
		ISkinProvider provider;
		public SkinZoomTrackBarObjectPainter(ISkinProvider provider) : base() {
			this.provider = provider;
		}
		protected override ISkinProvider Provider { get { return provider; } }
		public override void DrawPoints(TrackBarObjectInfoArgs e) { }
		protected override SkinElement GetTrack(TrackBarViewInfo vi) { return EditorsSkins.GetSkin(provider)[EditorsSkins.SkinTrackBarTrack]; }
		protected override void DrawSkinTrackLineCore(TrackBarObjectInfoArgs e, Rectangle bounds) {
			base.DrawSkinTrackLineCore(e, bounds);
			DrawTrackLineMiddleLine(e);
		}
		public override int GetThumbBestHeight(TrackBarViewInfo vi) {
			ZoomTrackBarViewInfo zi = (ZoomTrackBarViewInfo)vi;
			return zi.TrackCalculator.ThumbHeight;
		}
		protected override void UpdateSkinThumbState(SkinElementInfo info, TrackBarObjectInfoArgs e) {
			ZoomTrackBarViewInfo vi = e.ViewInfo as ZoomTrackBarViewInfo;
			info.ImageIndex = -1;
			info.State = ObjectState.Normal;
			ZoomTrackBarHitInfo pressedInfo = vi.PressedInfo as ZoomTrackBarHitInfo;
			if(!vi.Enabled) info.State = ObjectState.Disabled;
			else if(pressedInfo != null && pressedInfo.ZoomHitTest == ZoomTrackBarHitTest.ZoomThumb) info.State = ObjectState.Pressed;
			else if(vi.HotInfo != null && vi.HotInfo.ZoomHitTest == ZoomTrackBarHitTest.ZoomThumb) info.State = ObjectState.Hot;
			else if(vi.Focused) info.State = ObjectState.Selected;
		}
		protected internal override TrackBarInfoCalculator GetCalculator(TrackBarViewInfo viewInfo) {
			return new ZoomTrackBarSkinInfoCalculator(viewInfo as ZoomTrackBarViewInfo, this);
		}
		public override void DrawThumb(TrackBarObjectInfoArgs e) {
			DrawSkinThumb(e);
		}
		protected override void DrawButtons(ObjectInfoArgs e) {
			TrackBarObjectInfoArgs te = e as TrackBarObjectInfoArgs;
			ZoomTrackBarViewInfo vi = te.ViewInfo as ZoomTrackBarViewInfo;
			vi.ZoomInButtonInfo.Cache = e.Cache;
			vi.ZoomOutButtonInfo.Cache = e.Cache;
			vi.ButtonPainter.DrawObject(vi.ZoomInButtonInfo);
			vi.ButtonPainter.DrawObject(vi.ZoomOutButtonInfo);
		}
		protected override void DrawTrackLineCore(TrackBarObjectInfoArgs e, Rectangle bounds) {
			DrawSkinTrackLineCore(e, bounds);
		}
		protected virtual SkinElementInfo GetTrackLineMiddleLineInfo(TrackBarObjectInfoArgs e) {
			SkinElement elem = EditorsSkins.GetSkin(provider)[EditorsSkins.SkinZoomTrackBarMiddleLine];
			if(elem == null || elem.Image == null || elem.Image.Image == null) return null;
			Rectangle rect = new Rectangle(Point.Empty, elem.Image.Image.Size);
			rect.X = e.ViewInfo.TrackLineRect.X + (e.ViewInfo.TrackLineRect.Width - rect.Width) / 2;
			if(e.ViewInfo.Orientation == Orientation.Vertical) rect.X = rect.X - 1;
			rect.Y = e.ViewInfo.TrackLineRect.Y + (e.ViewInfo.TrackLineRect.Height - rect.Height) / 2;
			rect = e.ViewInfo.TrackBarHelper.Rotate(rect);
			return new SkinElementInfo(elem, rect);
		}
		protected override void DrawTrackLineMiddleLine(TrackBarObjectInfoArgs e) {
			SkinElementInfo info = GetTrackLineMiddleLineInfo(e);
			if(info == null) return;
			new RotateObjectPaintHelper().DrawRotated(e.Cache, info, SkinElementPainter.Default, GetTrackLineRotateAngle(e.ViewInfo));
		}
	}
}
namespace DevExpress.Accessibility {
	public class ZoomTrackBarAccessible : TrackBarAccessible {
		public ZoomTrackBarAccessible(RepositoryItem item) : base(item) { }
	}
}
namespace DevExpress.XtraEditors {
	[DXToolboxItem(DXToolboxItemKind.Free), Designer("DevExpress.XtraEditors.Design.TrackBarDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Implements a zoom bar."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon),
	SmartTagAction(typeof(ZoomTrackBarActions), "EditLabel", "Edit Label", SmartTagActionType.CloseAfterExecute),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "ZoomTrackBarControl")
	]
	public class ZoomTrackBarControl : TrackBarControl {
		Timer scrollTimer;
		ZoomTrackBarHitTest scrollHitTest;
		[Browsable(false)]
		public override string EditorTypeName { get { return "ZoomTrackBarControl"; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ZoomTrackBarControlProperties"),
#endif
 DXCategory(CategoryName.Properties), SmartTagSearchNestedProperties]
		public new RepositoryItemZoomTrackBar Properties { get { return (base.Properties as RepositoryItemZoomTrackBar); } }
		protected override void SetControlStyle() {
			if(this.Properties.AllowKeyboardNavigation)
				SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.Selectable, true);
			else {
				SetStyle(ControlStyles.SupportsTransparentBackColor, true);
				SetStyle(ControlStyles.Selectable, false);
			}
		}
		protected override void OnEndInitCore() {
			base.OnEndInitCore();
			if(Properties.AllowUseMiddleValue)
				Properties.Middle = Properties.ConstraintMiddleValue(Properties.Middle);
		}
		protected override bool ShouldProcessHitTest(EditHitInfo hi) {
			ZoomTrackBarHitInfo hitInfo = hi as ZoomTrackBarHitInfo;
			if(hitInfo == null) return base.ShouldProcessHitTest(hi);
			return hitInfo.ZoomHitTest != ZoomTrackBarHitTest.None;
		}
		protected override void ClearHotPressedCore(EditHitInfo newHotInfo, bool focusLost) {
			EditViewInfo.PressedInfo = new ZoomTrackBarHitInfo();
			EditViewInfo.HotInfo = new ZoomTrackBarHitInfo();
			RefreshVisualLayoutCore(focusLost);
		}
		protected override Size DefaultSize { get { return new Size(104, 18); } }
		protected void InitScrollTimer(ZoomTrackBarHitTest hitTest) {
			int interval = 50;
			ZoomTrackBarHitInfo hitInfo = ViewInfo.PressedInfo as ZoomTrackBarHitInfo;
			if(this.scrollTimer == null || (hitInfo != null && hitInfo.ZoomHitTest != hitTest)) {
				interval = 400;
				DestroyScrollTimer();
				this.scrollTimer = new Timer();
				this.scrollTimer.Tick += new EventHandler(OnScrollTimerTick);
			}
			this.scrollHitTest = hitTest;
			this.scrollTimer.Interval = interval;
			this.scrollTimer.Enabled = true;
		}
		void OnScrollTimerTick(object sender, EventArgs e) {
			ZoomTrackBarHitInfo info = ViewInfo.PressedInfo as ZoomTrackBarHitInfo;
			if(Control.MouseButtons != MouseButtons.Left || info == null || scrollHitTest != info.ZoomHitTest) {
				DestroyScrollTimer();
				return;
			}
			if(scrollHitTest == ZoomTrackBarHitTest.ZoomInButton) MoveRight();
			else if(scrollHitTest == ZoomTrackBarHitTest.ZoomOutButton) MoveLeft();
			else DestroyScrollTimer();
			this.scrollTimer.Interval = 50;
		}
		protected void DestroyScrollTimer() {
			if(scrollTimer != null) scrollTimer.Dispose();
			this.scrollTimer = null;
			this.scrollHitTest = ZoomTrackBarHitTest.None;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if(!Properties.AllowKeyboardNavigation)
				return;
			base.OnKeyDown(e);
		}
		protected internal virtual void OnPressButtonCore(ZoomTrackBarHitInfo hitInfo, EditorButtonObjectInfoArgs buttonInfo) {
			if(buttonInfo.Button.Kind == ButtonPredefines.Plus) {
				MoveRight();
				InitScrollTimer(hitInfo.ZoomHitTest);
			}
			else if(buttonInfo.Button.Kind == ButtonPredefines.Minus) {
				MoveLeft();
				InitScrollTimer(hitInfo.ZoomHitTest);
			}
		}
		protected virtual void OnPressButton(ZoomTrackBarHitInfo hitInfo, EditorButtonObjectInfoArgs buttonInfo) {
			if(buttonInfo == null) return;
			OnPressButtonCore(hitInfo, buttonInfo);
			Properties.RaiseButtonPressed(new ButtonPressedEventArgs(buttonInfo.Button));
		}
		protected internal virtual void OnClickButtonCore(ZoomTrackBarHitInfo hitInfo, EditorButtonObjectInfoArgs buttonInfo) {
			DestroyScrollTimer();
		}
		protected internal virtual void OnClickButton(ZoomTrackBarHitInfo hitInfo, EditorButtonObjectInfoArgs buttonInfo) {
			if(buttonInfo == null) return;
			OnClickButtonCore(hitInfo, buttonInfo);
			Properties.RaiseButtonClick(new ButtonPressedEventArgs(buttonInfo.Button));
		}
		protected override void ProcessHitTestOnMouseDown(EditHitInfo hi) {
			base.ProcessHitTestOnMouseDown(hi);
			OnPressButton(hi as ZoomTrackBarHitInfo, hi.HitObject as EditorButtonObjectInfoArgs);
		}
		protected override void ProcessHitTestOnMouseUp(EditHitInfo hi) {
			ZoomTrackBarHitInfo prevPressed = ViewInfo.PressedInfo as ZoomTrackBarHitInfo;
			ZoomTrackBarHitInfo zoomInfo = hi as ZoomTrackBarHitInfo;
			base.ProcessHitTestOnMouseUp(hi);
			if(prevPressed != null && prevPressed.ZoomHitTest == zoomInfo.ZoomHitTest)
				OnClickButton(hi as ZoomTrackBarHitInfo, zoomInfo.HitObject as EditorButtonObjectInfoArgs);
		}
		protected override void OnProcessThumb(Point p){
			int value = ViewInfo.ValueFromPoint(ViewInfo.ControlToClient(p));
			if (IsInSnapRange(value)){
				value = Properties.Middle;
			}
			Value = value;
			ShowValue();
		}
		private bool IsInSnapRange(int value) {
			return Math.Abs(value - Properties.Middle) < Properties.SnapToMiddle;
		}
		public override int CalcMinHeight() {
			if(!Properties.GetAutoSize())
				return this.requestedDim;
			bool added = false;
			if(ViewInfo.GInfo.Graphics == null) { 
				ViewInfo.GInfo.AddGraphics(null);
				added = true;
			}
			int height = ViewInfo.CalcMinHeight(ViewInfo.GInfo.Graphics);
			if(added) ViewInfo.GInfo.ReleaseGraphics();
			return height;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ZoomTrackBarControlButtonClick"),
#endif
 Category(CategoryName.Events)]
		public event ButtonPressedEventHandler ButtonClick {
			add { Properties.ButtonClick += value; }
			remove { Properties.ButtonClick -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ZoomTrackBarControlButtonPressed"),
#endif
 Category(CategoryName.Events)]
		public event ButtonPressedEventHandler ButtonPressed {
			add { Properties.ButtonPressed += value; }
			remove { Properties.ButtonPressed -= value; }
		}
	}
}
