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
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
namespace DevExpress.Utils.Animation {
	public class WaitingIndicatorPainter : ObjectPainter {
		AppearanceDefault defaultAppearanceDescription;
		AppearanceDefault defaultAppearance;
		AppearanceDefault defaultAppearanceCaption;
		static Font DefaultCaptionFont = new Font(
		  AppearanceObject.DefaultFont.FontFamily, AppearanceObject.DefaultFont.Size + 4);
		static Font DefaultDescriptionFont = new Font(
			AppearanceObject.DefaultFont.FontFamily, AppearanceObject.DefaultFont.Size);
		public sealed override AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
		}
		public AppearanceDefault DefaultAppearanceCaption {
			get {
				if(defaultAppearanceCaption == null)
					defaultAppearanceCaption = CreateDefaultAppearanceCaption();
				return defaultAppearanceCaption;
			}
		}
		public AppearanceDefault DefaultAppearanceDescription {
			get {
				if(defaultAppearanceDescription == null)
					defaultAppearanceDescription = CreateDefaultAppearanceDescription();
				return defaultAppearanceDescription;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearance() {
			return new AppearanceDefault(SystemColors.Control);
		}
		protected virtual AppearanceDefault CreateDefaultAppearanceCaption() {
			return new AppearanceDefault(SystemColors.ControlText, Color.Empty, DefaultCaptionFont);
		}
		protected virtual AppearanceDefault CreateDefaultAppearanceDescription() {
			return new AppearanceDefault(SystemColors.ControlText, Color.Empty, DefaultDescriptionFont);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			IWaitingIndicatorInfoArgs info = e as IWaitingIndicatorInfoArgs;
			IWaitingIndicatorInfo waitingInfo = info.WaitingInfo;
			if(waitingInfo == null) return;
			waitingInfo.Calc(e.Cache, info);
			int cornerRadius = GetCornerRadius();
			Rectangle indicator = new Rectangle(cornerRadius, cornerRadius,
				waitingInfo.Bounds.Width - cornerRadius * 2, waitingInfo.Bounds.Height - cornerRadius * 2);
			if(indicator.Height == 0 || indicator.Width == 0) return;
			using(Bitmap indicatorImage = new Bitmap(indicator.Width, indicator.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)) {
				using(Graphics g = Graphics.FromImage(indicatorImage)) {
					using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(g, indicator)) {
						bg.Graphics.TranslateTransform(-cornerRadius - waitingInfo.Bounds.X, -cornerRadius - waitingInfo.Bounds.Y);
						using(GraphicsCache bufferedCache = new GraphicsCache(bg.Graphics)) {
							if(CanDrawBackground(waitingInfo as WaitingIndicatorInfo))
								DrawBackground(bufferedCache, waitingInfo);
							DrawContent(bufferedCache, waitingInfo);
						}
						bg.Render();
					}
				}
				if(cornerRadius > 0 && CanDrawBackground(waitingInfo as WaitingIndicatorInfo))
					DrawBackground(e.Cache, waitingInfo);
				if(CanDrawBackground(waitingInfo as WaitingIndicatorInfo))
					e.Cache.Graphics.DrawImageUnscaled(indicatorImage,
						new Point(waitingInfo.Bounds.X + cornerRadius, waitingInfo.Bounds.Y + cornerRadius));
				waitingInfo.WaitingAnimator.DrawAnimatedItem(e.Cache, waitingInfo.ImageBounds);
			}
		}
		bool CanDrawBackground(WaitingIndicatorInfo waitingInfo) {
			if(waitingInfo !=null && waitingInfo.Properties != null && waitingInfo.Properties.AllowBackground)
				return true;
			if(waitingInfo == null || waitingInfo.Properties == null)
				return true;
			return false;
		}
		protected virtual int GetCornerRadius() { return 0; }
		protected virtual void DrawBackground(GraphicsCache cache, IWaitingIndicatorInfo e) {
			e.PaintAppearance.FillRectangle(cache, e.Bounds);
		}
		protected virtual void DrawContent(GraphicsCache cache, IWaitingIndicatorInfo e) {
			e.PaintAppearanceCaption.DrawString(cache, e.Caption, e.CaptionBounds);
			e.PaintAppearanceDescription.DrawString(cache, e.Description, e.DescriptionBounds);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return Rectangle.Inflate(client, 8, 8);
		}
	}
	public class WaitingIndicatorSkinPainter : WaitingIndicatorPainter {
		ISkinProvider provider;
		public WaitingIndicatorSkinPainter(ISkinProvider provider) : base() {
			this.provider = provider;
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault appearance = base.CreateDefaultAppearance();
			SkinElement window = GetWindow();
			if(window != null) {
				window.Apply(appearance);
				window.ApplyForeColorAndFont(appearance);
			}
			return appearance;
		}
		protected override AppearanceDefault CreateDefaultAppearanceCaption() {
			AppearanceDefault appearance = base.CreateDefaultAppearanceCaption();
			SkinElement window = GetWindow();
			if(window != null)
				window.ApplyForeColorAndFont(appearance);
			return appearance;
		}
		protected override AppearanceDefault CreateDefaultAppearanceDescription() {
			AppearanceDefault appearance = base.CreateDefaultAppearanceDescription();
			SkinElement window = GetWindow();
			if(window != null)
				window.ApplyForeColorAndFont(appearance);
			return appearance;
		}
		protected override int GetCornerRadius() {
			return GetWindow().Properties.GetInteger(BarSkins.SkinAlertWindowCornerRadius);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			SkinElement window = GetWindow();
			int r = (int)Math.Ceiling((double)window.Properties.GetInteger(BarSkins.SkinAlertWindowCornerRadius) * 0.5);
			return Rectangle.Inflate(window.ContentMargins.Inflate(client), 10 + r, 10 + r);
		}
		protected override void DrawBackground(GraphicsCache cache, IWaitingIndicatorInfo e) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default,
				new SkinElementInfo(GetWindow(), e.Bounds));
			ObjectPainter.DrawObject(cache, new EmptySkinElementPainter(),
				new SkinElementInfo(GetWindowTop(), GetTopRectangle(e.Bounds)));
		}
		protected virtual Skin GetSkin() {
			return BarSkins.GetSkin(provider);
		}
		protected virtual SkinElement GetWindow() {
			return GetSkin()[BarSkins.SkinAlertWindow];
		}
		protected virtual SkinElement GetWindowTop() {
			return GetSkin()[BarSkins.SkinAlertCaptionTop];
		}
		class EmptySkinElementPainter : SkinElementPainter {
			protected override void DrawSkinForeground(SkinElementInfo ee) { }
		}
		Rectangle GetTopRectangle(Rectangle bounds) {
			return new Rectangle(bounds.X, bounds.Y, bounds.Width, 10);
		}
	}
}
