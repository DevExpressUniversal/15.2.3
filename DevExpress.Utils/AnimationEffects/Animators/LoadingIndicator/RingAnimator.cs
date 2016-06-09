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
using System.Windows.Media.Imaging;
using DevExpress.Data.Utils;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Animation;
using DevExpress.Utils.Base;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.VisualEffects;
namespace DevExpress.Utils.Animation {
	public class RingAnimator : DotAnimator {
		Size ringSizeCore;
		int ringToTextIntervalCore;
		int captionToDescriptionIntervalCore;
		AppearanceObject appearanceCore;
		AppearanceObject appearanceCaptionCore;
		AppearanceObject appearanceDescriptionCore;
		public Size RingSize {
			get { return ringSizeCore; }
			set {
				if(value.Width != value.Height) throw new ArgumentException();
				ringSizeCore = value; 
			}
		}
		public bool AllowBackground { get; set; }
		public AppearanceObject Appearance {
			get { return appearanceCore; }
		}
		public AppearanceObject AppearanceCaption {
			get { return appearanceCaptionCore; }
		}
		public AppearanceObject AppearanceDescription {
			get { return appearanceDescriptionCore; }
		}
		public ContentAlignment ContentAlignment { get; set; }
		public bool ShowCaption { get; set; }
		public bool ShowDescription { get; set; }
		public string Caption { get; set; }
		public string Description { get; set; }
		internal bool IsCaptionVisible {
			get { return !string.IsNullOrEmpty(Caption) && ShowCaption; }
		}
		internal bool IsDescriptionVisible {
			get { return !string.IsNullOrEmpty(Description) && ShowDescription; }
		}
		internal bool IsAutonome { get; set; }
		public RingAnimator()
			: base() {
			ringSizeCore = new Size(40, 40);
			appearanceCaptionCore = new AppearanceObject();
			appearanceDescriptionCore = new AppearanceObject();
			appearanceCore = new AppearanceObject();
			Description = "Loading";
			ShowCaption = true;
			ShowDescription = true;
			captionToDescriptionIntervalCore = 8;
			ringToTextIntervalCore = 12;
		}
		RingAnimatorViewInfo viewInfoCore;
		internal RingAnimatorViewInfo ViewInfo {
			get {
				if(viewInfoCore == null)
					viewInfoCore = new RingAnimatorViewInfo(this);
				return viewInfoCore;
			}
		}
		public int RingToTextInterval {
			get { return ringToTextIntervalCore; }
			set {
				if(value < 0) throw new ArgumentException();
				ringToTextIntervalCore = value; 
			}
		}
		public int CaptionToDescriptionInterval {
			get { return captionToDescriptionIntervalCore; }
			set {
				if(value < 0) throw new ArgumentException();
				captionToDescriptionIntervalCore = value; 
			}
		}
		public override Color Color {
			get { return Appearance.ForeColor; }
			set { }
		}
		protected override BaseProgressDotPainter CreatePainter() {
			return new RingProgressDotPainter();
		}
		protected override void OnImageAnimation(BaseAnimationInfo info) {
			if(AnimationInProgress) {
				Progress = (double)(info.CurrentFrame / (double)info.FrameCount);
				int viewDelay = 5;
				if(ProgressDotList.Count > 0) {
					int animationPeriod = (int)ProgressDotList[0].Period * 2 + viewDelay * (ProgressDotList.Count - 1);
					int currentTiming = (int)(animationPeriod * Progress);
					for(int i = 0; i < ProgressDotList.Count; i++) {
						(ProgressDotList[i] as RingProgressDotInfoArgs).Calc(Bounds, RingSize, (currentTiming - i * viewDelay));
					}
				}
				if(!info.IsFinalFrame)
					RaiseInvalidate();
			}
		}
		protected override internal void InitProgressDotList(Rectangle bounds) {
			ProgressDotList = new List<BaseProgressDotInfoArgs>();
			for(int i = 0; i < AnimationElementNumber; i++) {
				RingProgressDotInfoArgs progressDot = new RingProgressDotInfoArgs(Acceleration, InitialSpeed, DotImage);
				progressDot.CalcPeriod(this.AnimationBounds);
				ProgressDotList.Add(progressDot);
			}
		}
	}
	public class RingAnimatorPainter : DotAnimatorPainter {
		ISkinProvider skinProviderCore;
		public RingAnimatorPainter(DotAnimator animator, ISkinProvider skinProvider) : base(animator) {
			skinProviderCore = skinProvider;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			RingAnimator ringAnimator = Animator as RingAnimator;
			RingAnimatorViewInfo viewInfo = ringAnimator.ViewInfo;
			Animator.Bounds = viewInfo.CalcBounds(e.Graphics, e.Bounds);
			System.Drawing.Text.TextRenderingHint savedTextRenderingHint = e.Graphics.TextRenderingHint;
			if(ringAnimator.IsAutonome)
				e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			if(ringAnimator.AllowBackground) {
				Rectangle backgroundBounds = Animator.Bounds;
				int r = (int)Math.Ceiling((double)GetWindow().Properties.GetInteger(BarSkins.SkinAlertWindowCornerRadius) * 0.5);
				backgroundBounds.Inflate(10 + r, 10 + r);
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default,
								new SkinElementInfo(GetWindow(), backgroundBounds));
			}
			if(!string.IsNullOrEmpty(ringAnimator.Caption) && ringAnimator.ShowCaption)
				e.Graphics.DrawString(ringAnimator.Caption, ringAnimator.AppearanceCaption.GetFont(), ringAnimator.AppearanceCaption.GetForeBrush(e.Cache),
					viewInfo.CaptionBounds, ringAnimator.AppearanceCaption.GetStringFormat());
			if(!string.IsNullOrEmpty(ringAnimator.Description) && ringAnimator.ShowDescription)
				e.Graphics.DrawString(ringAnimator.Description, ringAnimator.AppearanceDescription.Font, ringAnimator.AppearanceDescription.GetForeBrush(e.Cache),
					viewInfo.DescriptionBounds, ringAnimator.AppearanceDescription.GetStringFormat());
			e.Graphics.TextRenderingHint = savedTextRenderingHint;
			ringAnimator.DrawAnimatedItem(e.Cache, viewInfo.RingBounds);
		}
		protected virtual Skin GetSkin() {
			return BarSkins.GetSkin(skinProviderCore);
		}
		protected virtual SkinElement GetWindow() {
			return GetSkin()[BarSkins.SkinAlertWindow];
		}
	}
	internal class RingAnimatorViewInfo {
		RingAnimator Animator { get; set; }
		internal RingAnimatorViewInfo(RingAnimator animator) {
			Animator = animator;
		}
		bool UseCaptionToDescriptionInterval {
			get {
				RingAnimator ringAnimator = Animator as RingAnimator;
				return ringAnimator.IsCaptionVisible && ringAnimator.IsDescriptionVisible;
			}
		}
		bool UseRingToTextInterval {
			get {
				RingAnimator ringAnimator = Animator as RingAnimator;
				return ringAnimator.IsCaptionVisible || ringAnimator.IsDescriptionVisible;
			}
		}
		internal Rectangle CalcBounds(Graphics g, Rectangle controlBounds) {
			RingAnimator ringAnimator = Animator as RingAnimator;
			if(controlBounds.IsEmpty) return controlBounds;
			Rectangle result = new Rectangle();
			Size captionSize = GetCaptionSize(g, controlBounds);
			Size descriptionSize = GetDescriptionSize(g, controlBounds);
			int interval = !UseCaptionToDescriptionInterval ? 0 : ringAnimator.CaptionToDescriptionInterval;
			int textHeight = captionSize.Height + descriptionSize.Height + interval;
			int textWidth = Math.Max(captionSize.Width, descriptionSize.Width);
			Size textSize = new Size(textWidth, textHeight);
			int ringToTextInterval = !UseRingToTextInterval ? 0 : ringAnimator.RingToTextInterval;
			result.Size = new Size(ringAnimator.RingSize.Width + textWidth + ringToTextInterval, Math.Max(ringAnimator.RingSize.Height, textHeight));
			result = PlacementHelper.Arrange(result.Size, controlBounds, ringAnimator.ContentAlignment);
			Rectangle textRect = PlacementHelper.Arrange(textSize, result, ContentAlignment.MiddleRight);
			CaptionBounds = PlacementHelper.Arrange(captionSize, textRect, ContentAlignment.TopLeft);
			DescriptionBounds = PlacementHelper.Arrange(descriptionSize, textRect, ContentAlignment.BottomLeft);
			RingBounds = PlacementHelper.Arrange(ringAnimator.RingSize, result, ContentAlignment.MiddleLeft);
			return result;
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
			CharacterRange[] ranges = { new CharacterRange(0, text.Length) };
			Region[] regions = new Region[1];
			sf.SetMeasurableCharacterRanges(ranges);
			regions = g.MeasureCharacterRanges(text, appearance.GetFont(), controlBounds, sf);
			return Size.Round(regions[0].GetBounds(g).Size);
		}
		internal Rectangle DescriptionBounds { get; set; }
		internal Rectangle CaptionBounds { get; set; }
		internal Rectangle RingBounds { get; set; }
	}
	internal class RingProgressDotInfoArgs : BaseProgressDotInfoArgs {
		public Point Center { get; set; }
		double maxSpeedCore;
		public int Radius { get; set; }
		public double RotationAngle { get; set; }
		public RingProgressDotInfoArgs(double acceleration, double initialSpeed, Image image) : base(acceleration, initialSpeed, image) { }
		public void Calc(Rectangle bounds, Size ringSize, double frameCount) {
			Radius = ringSize.Height / 2;
			Calc(bounds, frameCount);
		}
		public override void Calc(Rectangle bounds, double frameCount) {
			Point center = bounds.Location;
			center.Offset(Radius, bounds.Height / 2);
			Center = center;
			AllowDraw = false;
			if(frameCount < -1) {
				return;
			}
			else if(frameCount > Period * 2) {
				return;
			}
			else {
				double frameTime = frameCount;
				frameTime = (frameCount > Period) ? (frameCount - (Period * ((int)((frameCount) / Period)))) : (frameCount);
				if(frameTime < AccelerationTime) {
					RotationAngle = (maxSpeedCore * frameTime) - ((Acceleration * Math.Pow(frameTime, 2)) / 2.0);
				}
				if((frameTime >= AccelerationTime) && (frameTime <= (AccelerationTime + (140.0 / InitialSpeed)))) {
					RotationAngle = (frameTime - AccelerationTime) * InitialSpeed + 120;
				}
				if(frameTime > (AccelerationTime + (140.0 / InitialSpeed))) {
					double tempTime = frameTime - (AccelerationTime + (140.0 / InitialSpeed));
					RotationAngle = (InitialSpeed * tempTime) + ((Acceleration * Math.Pow(tempTime, 2)) / 2.0);
					RotationAngle += 260.0;
				}
				AllowDraw = true;
			}
		}
		public override void CalcPeriod(Rectangle bounds) {
			double discriminant = Math.Pow(2.0 * InitialSpeed, 2.0) - ((4.0 * Acceleration) * -220.0);
			AccelerationTime = ((-2.0 * InitialSpeed) + Math.Sqrt(discriminant)) / (2.0 * Acceleration);
			maxSpeedCore = InitialSpeed + (Acceleration * AccelerationTime);
			Period = (2.0 * AccelerationTime) + (140.0 / InitialSpeed);
		}
	}
	internal class RingProgressDotPainter : BaseProgressDotPainter {
		public override void DrawProgressDot(GraphicsCache cache, BaseProgressDotInfoArgs info) {
			RingProgressDotInfoArgs infoArgs = info as RingProgressDotInfoArgs;
			float dotDiameter = infoArgs.Radius / 4;
			if(infoArgs.AllowDraw) {
				if(infoArgs.Image == null) {
					cache.Graphics.FillEllipse(cache.GetSolidBrush(info.Color),
					infoArgs.Center.X - dotDiameter / 2 - (((float)Math.Cos(((infoArgs.RotationAngle + 250.0) * Math.PI) / 180.0)) * (infoArgs.Radius - dotDiameter / 2)),
					infoArgs.Center.Y - dotDiameter / 2 - (((float)Math.Sin(((infoArgs.RotationAngle + 250.0) * Math.PI) / 180.0)) * (infoArgs.Radius - dotDiameter / 2)),
					dotDiameter, dotDiameter);
				}
				else {
					Bitmap bmp = new Bitmap(info.Image.Width, info.Image.Height);
					Graphics gfx = Graphics.FromImage(bmp);
					gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
					gfx.RotateTransform((float)infoArgs.RotationAngle);
					gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
					gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
					gfx.DrawImage(info.Image, new Point(0, 0));
					gfx.Dispose();
					cache.Graphics.DrawImage(bmp, new PointF(infoArgs.Center.X - dotDiameter / 2 - (((float)Math.Cos(((infoArgs.RotationAngle + 250.0) * Math.PI) / 180.0)) * (infoArgs.Radius - dotDiameter / 2)),
														   infoArgs.Center.Y - dotDiameter / 2 - (((float)Math.Sin(((infoArgs.RotationAngle + 250.0) * Math.PI) / 180.0)) * (infoArgs.Radius - dotDiameter / 2))));
				}
			}
		}
	}
	public class RingAnimatorInfo : BaseWaitingIndicatorInfo {
		RingAnimatorPainter painter;
		RingAnimator animator;
		AppearanceDefault appearanceDefault;
		AppearanceDefault appearanceDefaultCaption;
		AppearanceDefault appearanceDefaultDescription;
		static Font DefaultCaptionFont = new Font("Microsoft Sans Serif", 12F);
		static Font DefaultDescriptionFont = new Font("Microsoft Sans Serif", 8.25F);
		public LineAnimator Animator {
			get { return Animator; }
		}
		ISkinProvider skinProviderCore;
		public RingAnimatorInfo(IWaitingIndicatorProperties properties, AsyncSkinInfo skinInfo, bool isAutonome = false) : this(properties, skinInfo.LookAndFeel) {
			if(animator != null)
				animator.IsAutonome = isAutonome;
		}
		public RingAnimatorInfo(IWaitingIndicatorProperties properties, ISkinProvider skinProvider)
			: base(properties) {
			animator = new RingAnimator();
			if(skinProvider != null) {
				skinProviderCore = skinProvider;
				appearanceDefault = CreateDefaultAppearance();
				appearanceDefaultCaption = CreateDefaultAppearanceCaption();
				appearanceDefaultDescription = CreateDefaultAppearanceDescription();
			}
			RingWaitingIndicatorProperties ringProperties = properties as RingWaitingIndicatorProperties;
			if(ringProperties != null) {
				animator.Acceleration = ringProperties.AnimationAcceleration;
				animator.AnimationInterval = ringProperties.FrameInterval;
				animator.FramesCount = ringProperties.FrameCount;
				animator.RingSize = new Size (ringProperties.RingAnimationDiameter, ringProperties.RingAnimationDiameter);
				animator.InitialSpeed = ringProperties.AnimationSpeed;
				animator.AnimationElementNumber = ringProperties.AnimationElementCount;
				animator.CaptionToDescriptionInterval = ringProperties.CaptionToDescriptionDistance;
				animator.Caption = ringProperties.Caption;
				animator.Description = ringProperties.Description;
				animator.ShowCaption = ringProperties.ShowCaption;
				animator.ShowDescription = ringProperties.ShowDescription;
				animator.DotImage = ringProperties.AnimationElementImage;
				animator.RingToTextInterval = ringProperties.ImageToTextDistance;
				animator.ContentAlignment = ringProperties.ContentAlignment;
				animator.AllowBackground = ringProperties.AllowBackground;
				if(appearanceDefaultCaption != null && appearanceDefaultDescription != null && appearanceDefault != null) {
					AppearanceHelper.Combine(animator.Appearance, ringProperties.Appearance, appearanceDefault);
					AppearanceHelper.Combine(animator.AppearanceCaption, ringProperties.AppearanceCaption, appearanceDefaultCaption);
					AppearanceHelper.Combine(animator.AppearanceDescription, ringProperties.AppearanceDescription, appearanceDefaultDescription);
				}
			}
			painter = new RingAnimatorPainter(animator, skinProviderCore);
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
