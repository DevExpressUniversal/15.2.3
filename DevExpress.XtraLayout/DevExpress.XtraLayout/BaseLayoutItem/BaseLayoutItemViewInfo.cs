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
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Handlers;
using Padding = DevExpress.XtraLayout.Utils.Padding;
using DevExpress.Utils.Drawing.Animation;
using System.Windows.Forms;
namespace DevExpress.XtraLayout.ViewInfo {
	public abstract class BaseViewInfo : ObjectInfoArgs,ISupportXtraAnimation , ICloneable {
		protected Point innerOffset;
		protected bool shouldUpdateBorderInfo = true;
		BaseLayoutItem owner;
		public virtual BaseLayoutItem Owner {
			get { return owner; }
			set { owner = value; }
		}
		public ILayoutControl OwnerILayoutControl {
			get { return owner.Owner; }
		}
		protected BaseViewInfo(BaseLayoutItem owner) {
			this.owner = owner;
		}
		public bool IsViewInfoCalculated {
			get { return !Owner.shouldUpdateViewInfo; }
			set { Owner.shouldUpdateViewInfo = !value; }
		}
		protected internal void ResetHotTrackHelper() {
			if(OwnerILayoutControl == null) return;
			if(OwnerILayoutControl.RootGroup == null) return;
			if(OwnerILayoutControl.RootGroup.Handler == null) return;
			((LayoutGroupHandler)(OwnerILayoutControl.RootGroup.Handler)).HotTrackHelper.Reset();
		}
		protected ObjectState GetState(bool getExpandButtonState) {
			if(Owner != null) {
				if(Owner.Selected && !getExpandButtonState) return ObjectState.Selected;
				if(!Owner.EnabledState && (OwnerILayoutControl == null || !OwnerILayoutControl.EnableCustomizationMode || OwnerILayoutControl.OptionsView.DrawAdornerLayer != DefaultBoolean.True || OwnerILayoutControl.DesignMode)) return ObjectState.Disabled;
				if(OwnerILayoutControl != null) {
					if(OwnerILayoutControl.RootGroup != null && OwnerILayoutControl.RootGroup.Handler != null) {
						return ((LayoutGroupHandler)(OwnerILayoutControl.RootGroup.Handler)).HotTrackHelper.GetState(Owner, getExpandButtonState);
					}
				}
			}
			return ObjectState.Normal;
		}
		public override ObjectState State { get { return GetState(false); } }
		public virtual Point Offset {
			get { return innerOffset; }
			set { innerOffset = value; }
		}
		protected internal Rectangle TranslateCoordinates(Rectangle rect) {
			Rectangle screenBounds = rect;
			screenBounds.Offset(Offset);
			return screenBounds;
		}
		protected virtual Size ViewinfoOwerSize {
			get {
				if(Owner == null) return Size.Empty;
				BaseLayoutItem bitem = Owner as BaseLayoutItem;
				LayoutControlItem item = Owner as LayoutControlItem;
				LayoutGroup group = Owner as LayoutGroup;
				int width = Owner.Width;
				int height = Owner.Height;
				if(item != null || (group != null && group.ParentTabbedGroup == null)) {
					if(bitem.Parent != null) {
						width--;
						height--;
						if(bitem.Parent.ViewInfo.ClientArea.Right == bitem.Bounds.Right && bitem.Parent.GroupBordersVisible)
							width++;
						if(bitem.Parent.ViewInfo.ClientArea.Bottom == bitem.Bounds.Bottom && bitem.Parent.GroupBordersVisible)
							height++;
					}
				}
				return new Size(width, height);
			}
		}
		public virtual Rectangle BoundsRelativeToControl {
			get {
				return TranslateCoordinates(new Rectangle(Point.Empty, Owner.Size));
			}
		}
		public virtual Rectangle PainterBoundsRelativeToControl {
			get {
				return TranslateCoordinates(new Rectangle(Point.Empty, ViewinfoOwerSize)); 
			}
		}
		public virtual object Clone() {
			BaseViewInfo cloneInfo = (BaseViewInfo)this.MemberwiseClone();
			cloneInfo.Assign(new DevExpress.Utils.Drawing.ObjectInfoArgs(this.Cache, this.Bounds, this.State));
			cloneInfo.shouldUpdateBorderInfo = true;
			return cloneInfo;
		}
		float opacityCore = 1f;
		protected internal virtual float OpacityAnimation { get { return opacityCore; } set { if(value >= 0f && value <= 1f) opacityCore = value; } }
		public Control OwnerControl {
			get {
				if(OwnerILayoutControl == null) return null;
				return OwnerILayoutControl.Control;
			}
		}
		public bool CanAnimate {
			get {
				if(OwnerControl == null) return false;
				return true;
			}
		}
	}
	public class BaseLayoutItemViewInfo :BaseViewInfo {
		Rectangle controlsArea;
		Rectangle textArea;
		Padding paddings;
		bool isCalculating = false;
		protected ObjectInfoArgs borderInfoCore;
		public BaseLayoutItemViewInfo(BaseLayoutItem owner)
			: base(owner) {
			this.paddings = new Padding();
		}
		protected internal virtual void Destroy() { }
		protected void CalculateViewInfoIfNeeded() {
			if(IsViewInfoCalculated || isCalculating) return;
			if(Owner.DisposingFlag) return;
			StartCalculateViewInfo();
			CalcViewInfoCore();
			EndCalculateViewInfo();
		}
		void StartCalculateViewInfo() {
			isCalculating = true;
		}
		void EndCalculateViewInfo() {
			isCalculating = false;
		}
		public virtual Rectangle ClientArea {
			get {
				CalculateViewInfoIfNeeded();
				return controlsArea;
			}
			set { controlsArea = value; }
		}
		public Size SubLabelSizeIndentions(Size itemSize) {
			Size size = AddLabelIndentions(Size.Empty);
			return new Size(itemSize.Width - size.Width, itemSize.Height - size.Height);
		}
		public ObjectInfoArgs BorderInfo {
			get {
				CalculateViewInfoIfNeeded();
				if(borderInfoCore == null) borderInfoCore = CreateBorderInfo();
				return borderInfoCore;
			}
		}
		public override Point Offset {
			get { return innerOffset; }
			set {
				if(innerOffset != value) { innerOffset = value; Owner.SetShouldUpdateViewInfo(); }
			}
		}
		protected internal virtual void UpdateState() { }
		protected virtual ObjectInfoArgs CreateLastInfo() { return CreateBorderInfo(); }
		protected virtual ObjectInfoArgs CreateBorderInfo() { return new ObjectInfoArgs(); }
		public override object Clone() {
			BaseLayoutItemViewInfo cloneInfo = (BaseLayoutItemViewInfo)base.Clone();
			cloneInfo.paddings = new Padding(this.paddings.Left, this.paddings.Right, this.paddings.Top, this.paddings.Bottom);
			cloneInfo.isCalculating = false;
			cloneInfo.borderInfoCore = CreateBorderInfo();
			cloneInfo.LastInfo = null;
			return cloneInfo;
		}
		public virtual Rectangle TextArea {
			get {
				CalculateViewInfoIfNeeded();
				return textArea;
			}
			set {
				textArea = value;
				UpdateTextArea();
			}
		}
		protected virtual void UpdateTextArea() { }
		public virtual Rectangle TextAreaRelativeToControl {
			get {
				return TranslateCoordinates(TextArea);
			}
		}
		public virtual Rectangle ClientAreaRelativeToControl {
			get {
				return TranslateCoordinates(ClientArea);
			}
		}
		public virtual Padding Padding {
			get {
				CalculateViewInfoIfNeeded();
				return paddings;
			}
			protected set {
				paddings = value;
			}
		}
		protected virtual bool ShouldUpdateBorder {
			get {
				bool shouldUpdate = BorderInfo != null && BorderInfo.Bounds != CalculateObjectBounds(BoundsRelativeToControl) || shouldUpdateBorderInfo;
				return shouldUpdate;
			}
			set {
				shouldUpdateBorderInfo = false;
			}
		}
		protected virtual void UpdateBorderInfo(ObjectInfoArgs borderInfo) { }
		protected Padding Spaces {
			get {
				return PainterSpacing + Owner.Spacing;
			}
		}
		protected ObjectInfoArgs LastInfo;
		Padding? cachedPainterSpacing = null;
		protected virtual Padding PainterSpacing {
			get {
				if(cachedPainterSpacing == null) cachedPainterSpacing = PaintStyle.GetPainter(this.Owner).GetSpacing(this);
				return cachedPainterSpacing.Value;
			}
		}
		protected virtual Rectangle GetPainterRect() {
			if(LastInfo == null || ShouldUpdateBorder) UpdateBorder();
			if(cashedPaddingRect == Rectangle.Empty) cashedPaddingRect = PaintStyle.GetPainter(this.Owner).GetBorderPainter(this).GetObjectClientRectangle(LastInfo);
			return cashedPaddingRect;
		}
		protected Rectangle cashedPaddingRect = Rectangle.Empty;
		protected Rectangle measureRect = new Rectangle(0, 0, 300, 300);
		protected virtual void CalculatePaddings() {
			GetPainterRect();
			Padding p = Owner.Padding;
			if(!Owner.Expanded) p = Padding.Empty;
			paddings = p + Spaces + PaintStyle.GetPainter(this.Owner).GetPadding(this);
			CalculatePaddingCore();
		}
		protected Padding GetPainterPadding() {
			return new Padding(cashedPaddingRect.Left, (measureRect.Right - cashedPaddingRect.Right), cashedPaddingRect.Top, (measureRect.Bottom - cashedPaddingRect.Bottom));
		}
		protected virtual void CalculatePaddingCore() {
			Padding = Padding + GetPainterPadding();
		}
		public void CalculateViewInfo() {
			CalcViewInfo();
		}
		protected DevExpress.XtraLayout.Registrator.LayoutPaintStyle PaintStyle {
			get { return Owner == null ? DevExpress.XtraLayout.Registrator.LayoutPaintStyle.NullStyle : Owner.PaintStyle; }
		}
		protected virtual void CalcViewInfo() { CalculateViewInfoIfNeeded(); }
		protected virtual void CalcViewInfoCore() {
			CalculatePaddings();
			CalculateRegions();
			CalculateBorder();
			IsViewInfoCalculated = true;
		}
		protected virtual Rectangle CalculateObjectBounds(Rectangle rect) {
			return new Rectangle(rect.Left + Spaces.Left, rect.Top + Spaces.Top, rect.Size.Width - Spaces.Width, rect.Size.Height - Spaces.Height);
		}
		public virtual void UpdateAppearance() { }
		protected internal virtual void UpdateBorder() {
			UpdateBorderInfo(BorderInfo);
			if(LastInfo == null)
				LastInfo = CreateLastInfo();
			UpdateBorderInfo(LastInfo);
			if(cashedPaddingRect == Rectangle.Empty) {
				LastInfo.Bounds = measureRect;
				CalculateBorderInfoBounds(this, this.LastInfo);
			}
			BorderInfo.Bounds = CalculateObjectBounds(BoundsRelativeToControl);
			CalculateBorderInfoBounds(this, this.BorderInfo);
			ShouldUpdateBorder = false;
		}
		protected virtual void CalculateBorderInfoBounds(BaseLayoutItemViewInfo info, ObjectInfoArgs args) {
			PaintStyle.GetPainter(info.Owner).GetBorderPainter(info).CalcObjectBounds(args);
		}
		protected virtual void CalculateBorder() {
		}
		void CenterTextAndControl(LayoutType layoutType) {
		}
		protected virtual void CalculateRegions() {
			CalculateRegionsCore();
		}
		public void ResetAfterClone() {
			cashedPaddingRect = Rectangle.Empty;
		}
		protected Size OwnerTextSize {
			get {
				Size textSize = Owner.TextSize;
				if(Owner.TextLocation == Locations.Top || Owner.TextLocation == Locations.Bottom)
					textSize.Height += Owner.TextToControlDistance;
				if(Owner.TextLocation == Locations.Left || Owner.TextLocation == Locations.Right || Owner.TextLocation == Locations.Default)
					textSize.Width += Owner.TextToControlDistance;
				return textSize;
			}
		}
		protected virtual void CalculateRegionsCore() {
			Rectangle indentionsRect = CalcIndentionsRect();
			Rectangle controlsAreaRect = indentionsRect;
			Rectangle textAreaRect = indentionsRect;
			textAreaRect.Height = Math.Max(Owner.TextSize.Height, indentionsRect.Height);
			switch(Owner.TextLocation) {
				case Locations.Bottom:
					controlsAreaRect.Height -= OwnerTextSize.Height;
					textAreaRect.Y = controlsAreaRect.Bottom + Owner.TextToControlDistance;
					textAreaRect.Height = Owner.TextSize.Height;
					break;
				case Locations.Top:
					controlsAreaRect.Height -= OwnerTextSize.Height;
					textAreaRect.Height = Owner.TextSize.Height;
					controlsAreaRect.Y = textAreaRect.Bottom + Owner.TextToControlDistance;
					break;
				case Locations.Left:
					textAreaRect.Width = Owner.TextSize.Width;
					controlsAreaRect.Width -= OwnerTextSize.Width;
					controlsAreaRect.X = textAreaRect.Right + Owner.TextToControlDistance;
					break;
				case Locations.Right:
					textAreaRect.Width = Owner.TextSize.Width;
					controlsAreaRect.Width -= OwnerTextSize.Width;
					textAreaRect.X = controlsAreaRect.Right + Owner.TextToControlDistance;
					break;
				case Locations.Default:
					if(Owner.IsRTL) {
						textAreaRect.Width = Owner.TextSize.Width;
						controlsAreaRect.Width -= OwnerTextSize.Width;
						textAreaRect.X = controlsAreaRect.Right + Owner.TextToControlDistance;
					} else {
						textAreaRect.Width = Owner.TextSize.Width;
						controlsAreaRect.Width -= OwnerTextSize.Width;
						controlsAreaRect.X = textAreaRect.Right + Owner.TextToControlDistance;
					}
					break;
			}
			ClientArea = controlsAreaRect;
			TextArea = textAreaRect;
		}
		protected virtual Rectangle CalcIndentionsRect() {
			int ownerHeight = 0;
			Size maxSize = Owner.MaxSize;
			if((Owner.TruncateClientAreaToMaxSize) && maxSize.Height != 0 && Owner.Size.Height > maxSize.Height)
				ownerHeight = Math.Max(0, maxSize.Height - Padding.Height);
			else
				ownerHeight = Math.Max(0, Owner.Height - Padding.Height);
			Rectangle indentionsRect = new Rectangle(new Point(Padding.Left, Padding.Top), new Size(Owner.Width - Padding.Width, ownerHeight));
			return indentionsRect;
		}
		protected virtual Size AddIndentions(Size size) {
			return new Size(size.Width + Padding.Width,
				size.Height + Padding.Height);
		}
		public virtual Size AddLabelIndentions(Size size) {
			CalculateViewInfoIfNeeded();
			return AddIndentions(AddLabel(size));
		}
		protected virtual Size AddLabel(Size size) {
			switch(Owner.TextLocation) {
				case Locations.Bottom:
				case Locations.Top:
					return new Size(size.Width, size.Height + OwnerTextSize.Height);
				case Locations.Left:
				case Locations.Default:
				case Locations.Right:
					return new Size(size.Width + OwnerTextSize.Width, size.Height);
			}
			return size;
		}
	}
	internal class OpacityAnimationInfo :BaseAnimationInfo {
		BaseLayoutItemViewInfo vInfo;
		const float minOpacity = 0.4f;
		const float changeOpacity = 1f - minOpacity;
		const int animationTimeInMilliseconds = 200;
		const int frameCount = 100;
		public OpacityAnimationInfo(BaseLayoutItemViewInfo viewInfo) : base(viewInfo, viewInfo, (int)(TimeSpan.TicksPerMillisecond * animationTimeInMilliseconds / frameCount), frameCount) {
			vInfo = viewInfo;
			vInfo.OpacityAnimation = minOpacity;
		}
		public override void FrameStep() {
			float opacity = minOpacity + (changeOpacity * (float)CurrentFrame / FrameCount);
			if(IsFinalFrame) {
				opacity = 1f;
				XtraAnimator.Current.Animations.Remove(this);
			}
			vInfo.Owner.Invalidate(); 
			vInfo.OpacityAnimation = opacity;
		}
	}
}
