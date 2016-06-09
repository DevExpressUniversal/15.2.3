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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data.Utils;
using DevExpress.Skins;
using DevExpress.Utils.Animation;
using DevExpress.Utils.Base;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.VisualEffects;
namespace DevExpress.Utils.Animation {
	public enum LineAnimationElementType { Circle, Rectangle, Triangle  }
	public class LineAnimator : DotAnimator {
		LineAnimationElementType elementTypeCore;
		int animationHeigntCore;
		AppearanceObject appearanceCore;
		AppearanceObject appearanceCaptionCore;
		AppearanceObject appearanceDescriptionCore;
		int captionToDescriptionInterval;
		int lineToTextIntervalCore;
		public ContentAlignment ContentAlignment { get; set; }
		public bool ShowCaption { get; set; }
		public bool ShowDescription { get; set; }
		internal bool IsCaptionVisible {
			get { return !string.IsNullOrEmpty(Caption) && ShowCaption; }
		}
		internal bool IsDescriptionVisible {
			get { return !string.IsNullOrEmpty(Description) && ShowDescription; }
		}
		internal bool IsAutonome { get; set; }
		public int AnimationHeight {
			get { return DotImage == null ? animationHeigntCore : DotImage.Height; }
			set {
				if(value < 0) throw new ArgumentException();
				animationHeigntCore = value; 
			}
		}
		public LineAnimator()
			: base() {
			AnimationHeight = 10;
			lineToTextIntervalCore = 12;
			captionToDescriptionInterval = 8;
			ShowCaption = true;
			ShowDescription = true;
			appearanceCore = new AppearanceObject();
			appearanceCaptionCore = new AppearanceObject();
			appearanceDescriptionCore = new AppearanceObject();
			elementTypeCore = LineAnimationElementType.Circle;
		}
		public int LineToTextInterval {
			get { return lineToTextIntervalCore; }
			set {
				if(value < 0) throw new ArgumentException();
				lineToTextIntervalCore = value; 
			}
		}
		public int CaptionToDescriptionDistance {
			get { return captionToDescriptionInterval; }
			set {
				if(value < 0) throw new ArgumentException();
				captionToDescriptionInterval = value; 
			}
		}
		public LineAnimationElementType ElementType {
			get { return elementTypeCore; }
			set { elementTypeCore = value; }
		}
		public string Caption { get; set; }
		public string Description { get; set; }
		protected override BaseProgressDotPainter CreatePainter() {
			return new LineProgressDotPainter();
		}
		protected override void OnImageAnimation(BaseAnimationInfo info) {
			if(AnimationInProgress) {
				Point location = (ContentAlignment != ContentAlignment.BottomCenter && 
					ContentAlignment != ContentAlignment.BottomLeft && 
					ContentAlignment != ContentAlignment.BottomRight) ?
					Bounds.Location :
					new Point(Bounds.X, Bounds.Bottom - AnimationHeight);
				Progress = (double)(info.CurrentFrame / (double)info.FrameCount);
				double viewDelay = Math.Sqrt((double)300 / Bounds.Width * 4) * 2;
				if(ProgressDotList.Count > 0) {
					double animationPeriod = ProgressDotList[0].Period + viewDelay * (ProgressDotList.Count - 1);
					double currentTiming = (animationPeriod * Progress);
					for(int i = 0; i < ProgressDotList.Count; i++) {
						ProgressDotList[i].Calc(new Rectangle(location, new Size(Bounds.Width, AnimationHeight)), (currentTiming - i * viewDelay));
					}
				}
				if(!info.IsFinalFrame)
					RaiseInvalidate();
			}
		}
		LineAnimatorViewInfo viewInfoCore;
		internal LineAnimatorViewInfo ViewInfo {
			get {
				if(viewInfoCore == null)
					viewInfoCore = new LineAnimatorViewInfo(this);
				return viewInfoCore;
			}
		}
		public AppearanceObject Appearance { get { return appearanceCore; } }
		public AppearanceObject AppearanceCaption {
			get { return appearanceCaptionCore; }
		}
		public AppearanceObject AppearanceDescription {
			get { return appearanceDescriptionCore; }
		}
		public override Color Color {
			get { return Appearance.ForeColor; }
			set { }
		}
		protected override internal void InitProgressDotList(Rectangle bounds) {
			ProgressDotList = new List<BaseProgressDotInfoArgs>();
			for(int i = 0; i < AnimationElementNumber; i++) {
				LineProgressDotInfoArgs progressDot = new LineProgressDotInfoArgs(Acceleration, InitialSpeed, ElementType, DotImage);
				progressDot.CalcPeriod(bounds);
				ProgressDotList.Add(progressDot);
			}
		}
	}
	public class LineAnimatorDefaultPropeties : BaseDefaultProperties {
		public LineAnimatorDefaultPropeties(IBaseProperties parentProperties)
			: base(parentProperties) {
		}
		protected override IBaseProperties CloneCore() {
			return null;
		}
	}
	public class LineAnimatorPainter : DotAnimatorPainter {
		public LineAnimatorPainter(LineAnimator animator) : base(animator) { }
		public override void DrawObject(ObjectInfoArgs e) {
			LineAnimator lineAnimator = Animator as LineAnimator;
			LineAnimatorViewInfo viewInfo = lineAnimator.ViewInfo;
			Animator.Bounds = lineAnimator.ViewInfo.CalcBounds(e.Graphics, e.Bounds);
			if(lineAnimator.ShowCaption) {
				e.Graphics.DrawString(lineAnimator.Caption, lineAnimator.AppearanceCaption.GetFont(), lineAnimator.AppearanceCaption.GetForeBrush(e.Cache),
				   viewInfo.CaptionBounds, lineAnimator.AppearanceCaption.GetStringFormat());
			}
			if(lineAnimator.ShowDescription) {
				e.Graphics.DrawString(lineAnimator.Description, lineAnimator.AppearanceDescription.GetFont(), lineAnimator.AppearanceDescription.GetForeBrush(e.Cache),
				   viewInfo.DescriptionBounds, lineAnimator.AppearanceDescription.GetStringFormat());
			}
			Animator.DrawAnimatedItem(e.Cache, lineAnimator.ViewInfo.LineBounds);
		}
	}
	internal class LineAnimatorViewInfo {
		LineAnimator Animator { get; set; }
		internal LineAnimatorViewInfo(LineAnimator animator) {
			Animator = animator;
		}
		bool UseCaptionToDescriptionInterval {
			get {
				LineAnimator lineAnimator = Animator as LineAnimator;
				return lineAnimator.IsCaptionVisible && lineAnimator.IsDescriptionVisible;
			}
		}
		bool UseLineToTextInterval {
			get {
				LineAnimator lineAnimator = Animator as LineAnimator;
				return lineAnimator.IsCaptionVisible || lineAnimator.IsDescriptionVisible;
			}
		}
		protected internal Rectangle CalcBounds(Graphics g, Rectangle controlBounds) {
			if(controlBounds.IsEmpty) return controlBounds;
			LineAnimator lineAnimator = Animator as LineAnimator;
			ContentAlignment contentAlignment = lineAnimator.ContentAlignment;
			Rectangle result = new Rectangle();
			Size captionSize = GetCaptionSize(g, controlBounds);
			Size descriptionSize = GetDescriptionSize(g, controlBounds);
			int captionToDescriptionDistance = !UseCaptionToDescriptionInterval ? 0 : lineAnimator.CaptionToDescriptionDistance;
			int lineToTextDistance = !UseLineToTextInterval ? 0 : lineAnimator.LineToTextInterval;
			int distanceSum = captionToDescriptionDistance + lineToTextDistance;
			result.Size = new Size(controlBounds.Width, lineAnimator.AnimationHeight + captionSize.Height + descriptionSize.Height + distanceSum);
			result = PlacementHelper.Arrange(result.Size, controlBounds, contentAlignment);
			Rectangle content = result;
			Size lineSize = new Size(result.Width, lineAnimator.AnimationHeight);
			LineBounds = IsLineOnTop(contentAlignment) ? PlacementHelper.Arrange(lineSize, content, ContentAlignment.TopCenter) :
					PlacementHelper.Arrange(lineSize, content, ContentAlignment.BottomCenter);
			content.Height -= (lineAnimator.AnimationHeight + lineToTextDistance);
			if(IsLineOnTop(contentAlignment))
				content.Y += (lineAnimator.AnimationHeight + lineToTextDistance);
			CaptionBounds = PlacementHelper.Arrange(captionSize, content, GetCaptionAlignment(lineAnimator.ContentAlignment));
			DescriptionBounds = PlacementHelper.Arrange(descriptionSize, content, GetDescriptionAlignment(lineAnimator.ContentAlignment));
			return result;
		}
		bool IsLineOnTop(ContentAlignment contentAlignment) {
			return contentAlignment != ContentAlignment.BottomCenter &&
				   contentAlignment != ContentAlignment.BottomLeft &&
				   contentAlignment != ContentAlignment.BottomRight;
		}
		Size GetCaptionSize(Graphics g, Rectangle controlBounds) {
			Size captionSize = Animator.IsCaptionVisible ?
				GetTextSize(g, Animator.AppearanceCaption, Animator.Caption, controlBounds) : Size.Empty;
			return captionSize;
		}
		Size GetDescriptionSize(Graphics g, Rectangle controlBounds) {
			Size descriptionSize = Animator.IsDescriptionVisible ?
				GetTextSize(g, Animator.AppearanceDescription, Animator.Description, controlBounds) : Size.Empty;
			return descriptionSize;
		}
		private Size GetTextSize(Graphics g, AppearanceObject appearance, string text, Rectangle controlBounds) {
			StringFormat sf = appearance.GetStringFormat();
			sf.SetMeasurableCharacterRanges(new CharacterRange[] { new CharacterRange(0, text.Length) });
			var regions = g.MeasureCharacterRanges(text, appearance.GetFont(), controlBounds, sf);
			if(regions.Count() == 0) return Size.Empty;
			return Size.Round(regions[0].GetBounds(g).Size);
		}
		ContentAlignment GetDescriptionAlignment(ContentAlignment contentAlignment) {
			if(contentAlignment == ContentAlignment.BottomCenter ||
			   contentAlignment == ContentAlignment.MiddleCenter ||
			   contentAlignment == ContentAlignment.TopCenter)
				return ContentAlignment.BottomCenter;
			if(contentAlignment == ContentAlignment.BottomRight ||
			   contentAlignment == ContentAlignment.MiddleRight ||
			   contentAlignment == ContentAlignment.TopRight)
				return ContentAlignment.BottomRight;
			return ContentAlignment.BottomLeft;
		}
		ContentAlignment GetCaptionAlignment(ContentAlignment contentAlignment) {
			if(contentAlignment == ContentAlignment.BottomCenter ||
			   contentAlignment == ContentAlignment.MiddleCenter ||
			   contentAlignment == ContentAlignment.TopCenter)
				return ContentAlignment.TopCenter;
			if(contentAlignment == ContentAlignment.BottomRight ||
			   contentAlignment == ContentAlignment.MiddleRight ||
			   contentAlignment == ContentAlignment.TopRight)
				return ContentAlignment.TopRight;
			return ContentAlignment.TopLeft;
		}
		internal Rectangle DescriptionBounds { get; set; }
		internal Rectangle CaptionBounds { get; set; }
		internal Rectangle LineBounds { get; set; }
	}
	internal class LineProgressDotInfoArgs : BaseProgressDotInfoArgs {
		double maxSpeedCore;
		internal double MovingDistance { get; set; }
		public LineAnimationElementType ElementType { get; set; }
		public LineProgressDotInfoArgs(double acceleration, double initialSpeed, LineAnimationElementType elementType, Image image)
			: base(acceleration, initialSpeed, image) {
			ElementType = elementType;
		}
		public override void Calc(Rectangle bounds, double frameCount) {
			Bounds = bounds;
			AllowDraw = false;
			if(frameCount < -1) {
				return;
			}
			else if(frameCount > Period) {
				return;
			}
			double frameTime = frameCount;
			double scale = (double)bounds.Width / (3 * ScalingDistance);
			frameTime = (frameCount > Period) ? (frameCount - (Period * ((int)(((double)frameCount) / Period)))) : ((double)frameCount);
			if(frameTime < AccelerationTime) {
				MovingDistance = ((maxSpeedCore * frameTime) - ((Acceleration * Math.Pow(frameTime, 2)) / 2.0)) * scale;
			}
			if((frameTime >= AccelerationTime) && (frameTime <= (AccelerationTime + (ScalingDistance) / InitialSpeed))) {
				MovingDistance = ((frameTime - AccelerationTime) * InitialSpeed + ScalingDistance) * scale;
			}
			if(frameTime > (AccelerationTime + (ScalingDistance) / InitialSpeed)) {
				double tempTime = frameTime - (AccelerationTime + ((ScalingDistance) / InitialSpeed));
				MovingDistance = (InitialSpeed * tempTime) + ((Acceleration * Math.Pow(tempTime, 2)) / 2.0);
				MovingDistance += (2 * ScalingDistance);
				MovingDistance *= scale;
			}
			AllowDraw = true;
		}
		public override void CalcPeriod(Rectangle bounds) {
			double discriminant = Math.Pow(2.0 * InitialSpeed, 2.0) - ((4.0 * Acceleration * -(2 * 100)));
			AccelerationTime = ((-2.0 * InitialSpeed) + Math.Sqrt(discriminant)) / (2.0 * Acceleration);
			maxSpeedCore = InitialSpeed + (Acceleration * AccelerationTime);
			Period = (2.0 * AccelerationTime) + (ScalingDistance / InitialSpeed);
		}
	}
	public class LineProgressDotPainter : BaseProgressDotPainter {
		public override void DrawProgressDot(GraphicsCache cache, BaseProgressDotInfoArgs info) {
			LineProgressDotInfoArgs infoArgs = info as LineProgressDotInfoArgs;
			if(info.AllowDraw) {
				if(infoArgs.Image == null) {
					if(infoArgs.ElementType == LineAnimationElementType.Circle)
						cache.Graphics.FillEllipse(cache.GetSolidBrush(infoArgs.Color),
							(int)(infoArgs.Bounds.X + infoArgs.MovingDistance),
							infoArgs.Bounds.Y,
							(float)(infoArgs.Bounds.Height - 1), (float)(infoArgs.Bounds.Height - 1));
					if(infoArgs.ElementType == LineAnimationElementType.Rectangle)
						cache.Graphics.FillRectangle(cache.GetSolidBrush(infoArgs.Color),
							(int)(infoArgs.Bounds.X + infoArgs.MovingDistance),
							infoArgs.Bounds.Y,
							(float)(infoArgs.Bounds.Height), (float)(infoArgs.Bounds.Height));
					if(infoArgs.ElementType == LineAnimationElementType.Triangle)
						cache.Graphics.FillPolygon(cache.GetSolidBrush(infoArgs.Color), new PointF[] { 
						new PointF((float)infoArgs.Bounds.X + (float)infoArgs.MovingDistance, (float)infoArgs.Bounds.Y),
						new PointF((float)infoArgs.Bounds.X + (float)infoArgs.MovingDistance, (float)infoArgs.Bounds.Y + (float)infoArgs.Bounds.Height),
						new PointF((float)infoArgs.Bounds.X + (float)infoArgs.MovingDistance + (float)infoArgs.Bounds.Height, (float)infoArgs.Bounds.Y + (float)infoArgs.Bounds.Height / 2) });
				}
				else {
					cache.Graphics.DrawImage(infoArgs.Image, new Point((int)(infoArgs.Bounds.X + infoArgs.MovingDistance), infoArgs.Bounds.Y));
				}
			}
		}
	}
	public class LineAnimatorInfo : BaseWaitingIndicatorInfo {
		LineAnimatorPainter painter;
		LineAnimator animator;
		AppearanceDefault defaultAppearance;
		AppearanceDefault defaultCaptionAppearance;
		AppearanceDefault defaultDescriptionAppearance;
		static Font DefaultCaptionFont = new Font("Microsoft Sans Serif", 12F);
		static Font DefaultDescriptionFont = new Font("Microsoft Sans Serif", 8.25F);
		public LineAnimator Animator {
			get { return Animator; }
		}
		ISkinProvider skinProviderCore;
		public LineAnimatorInfo(IWaitingIndicatorProperties properties, AsyncSkinInfo skinInfo, bool isAutonome = false) : this(properties, skinInfo.LookAndFeel) {
			if(animator != null)
				animator.IsAutonome = isAutonome;
		}
		public LineAnimatorInfo(IWaitingIndicatorProperties properties, ISkinProvider skinProvider)
			: base(properties) {
			animator = new LineAnimator();
			if(skinProvider != null) {
				skinProviderCore = skinProvider;
				defaultAppearance = CreateDefaultAppearance();
				defaultCaptionAppearance = CreateDefaultAppearanceCaption();
				defaultDescriptionAppearance = CreateDefaultAppearanceDescription();
			}
			LineWaitingIndicatorProperties lineProperties = properties as LineWaitingIndicatorProperties;
			if(lineProperties != null) {
				animator.Acceleration = lineProperties.AnimationAcceleration;
				animator.AnimationInterval = lineProperties.FrameInterval;
				animator.FramesCount = lineProperties.FrameCount;
				animator.AnimationHeight = lineProperties.LineAnimationElementHeight;
				animator.ElementType = lineProperties.LineAnimationElementType;
				animator.InitialSpeed = lineProperties.AnimationSpeed;
				animator.DotImage = lineProperties.AnimationElementImage;
				animator.AnimationElementNumber = lineProperties.AnimationElementCount;
				animator.ShowCaption = properties.ShowCaption;
				animator.ShowDescription = properties.ShowDescription;
				animator.LineToTextInterval = properties.ImageToTextDistance;
				animator.CaptionToDescriptionDistance = properties.CaptionToDescriptionDistance;
				animator.Caption = properties.Caption;
				animator.Description = properties.Description;
				animator.ContentAlignment = lineProperties.ContentAlignment;
				if(defaultCaptionAppearance != null && defaultDescriptionAppearance != null && defaultAppearance != null) {
					AppearanceHelper.Combine(animator.Appearance, lineProperties.Appearance, defaultAppearance);
					AppearanceHelper.Combine(animator.AppearanceCaption, lineProperties.AppearanceCaption, defaultCaptionAppearance);
					AppearanceHelper.Combine(animator.AppearanceDescription, lineProperties.AppearanceDescription, defaultDescriptionAppearance);
				}
			}
			painter = new LineAnimatorPainter(animator);
		}
		protected AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault appearance = new AppearanceDefault(SystemColors.Control);
			SkinElement window = GetWindow();
			if(window != null) {
				window.Apply(appearance);
				window.ApplyForeColorAndFont(appearance);
			}
			return appearance;
		}
		protected AppearanceDefault CreateDefaultAppearanceCaption() {
			AppearanceDefault appearance = new AppearanceDefault(SystemColors.ControlText, Color.Empty, DefaultCaptionFont);
			SkinElement window = GetWindow();
			if(window != null)
				window.ApplyForeColorAndFont(appearance);
			return appearance;
		}
		protected AppearanceDefault CreateDefaultAppearanceDescription() {
			AppearanceDefault appearance = new AppearanceDefault(SystemColors.ControlText, Color.Empty, DefaultDescriptionFont);
			SkinElement window = GetWindow();
			if(window != null)
				window.ApplyForeColorAndFont(appearance);
			return appearance;
		}
		protected virtual Skin GetSkin() {
			return BarSkins.GetSkin(skinProviderCore);
		}
		protected virtual SkinElement GetWindow() {
			return GetSkin()[BarSkins.SkinAlertWindow];
		}
		protected override void OnDispose() {
			base.OnDispose();
			skinProviderCore = null;
			animator = null;
			painter = null;
		}
		public override string Caption { get { return Properties.Caption; } }
		public override string Description { get { return Properties.Description; } }
		public override WaitingIndicatorPainter Painter {
			get { return painter; }
		}
		public override ILoadingAnimator WaitingAnimator {
			get { return animator; }
		}
	}
}
