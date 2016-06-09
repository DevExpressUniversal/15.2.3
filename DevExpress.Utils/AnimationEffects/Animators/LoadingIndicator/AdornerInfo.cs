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
using DevExpress.Utils.Drawing;
using System.Drawing;
namespace DevExpress.Utils.Animation {
	public interface IWaitingIndicatorInfoArgs : IAdornerElementInfoArgs {
		IWaitingIndicatorInfo WaitingInfo { get; }
	}
	public interface IWaitingIndicatorInfo : IDisposable {
		void Calc(GraphicsCache cache, IWaitingIndicatorInfoArgs e);
		ILoadingAnimator WaitingAnimator { get; }
		WaitingIndicatorPainter Painter { get; }
		AppearanceObject PaintAppearance { get; }
		AppearanceObject PaintAppearanceCaption { get; }
		AppearanceObject PaintAppearanceDescription { get; }
		Rectangle Bounds { get; }
		Rectangle ImageBounds { get; }
		Rectangle DescriptionBounds { get; }
		Rectangle CaptionBounds { get; }
		string Caption { get; }
		string Description { get; }
	}
	public abstract class BaseWaitingIndicatorInfo : IWaitingIndicatorInfo {
		IWaitingIndicatorProperties propertiesCore;
		AppearanceObject paintAppearance;
		AppearanceObject paintAppearanceCaptionCore;
		AppearanceObject paintAppearanceDescriptionCore;
		Rectangle boundsCore;
		Rectangle imageBoundsCore;
		Rectangle descriptionBoundsCore;
		Rectangle captionBoundsCore;
		public BaseWaitingIndicatorInfo(IWaitingIndicatorProperties properties) {
			propertiesCore = properties;
		}
		public abstract string Caption { get; }
		public abstract string Description { get; }
		public abstract WaitingIndicatorPainter Painter { get; }
		public abstract ILoadingAnimator WaitingAnimator { get; }
		public AppearanceObject PaintAppearance {
			get {
				if(paintAppearance == null) {
					paintAppearance = new AppearanceObject();
					paintAppearance.Assign(Painter.DefaultAppearance);
				}
				return paintAppearance;
			}
		}
		public AppearanceObject PaintAppearanceCaption {
			get {
				if(paintAppearanceCaptionCore == null)
					paintAppearanceCaptionCore = CreateAppearance(Properties.AppearanceCaption, Painter.DefaultAppearanceCaption);
				return paintAppearanceCaptionCore;
			}
		}
		public AppearanceObject PaintAppearanceDescription {
			get {
				if(paintAppearanceDescriptionCore == null)
					paintAppearanceDescriptionCore = CreateAppearance(Properties.AppearanceDescription, Painter.DefaultAppearanceDescription);
				return paintAppearanceDescriptionCore;
			}
		}
		public Rectangle Bounds {
			get {
				return boundsCore;
			}
		}
		public Rectangle ImageBounds {
			get {
				return imageBoundsCore;
			}
		}
		public Rectangle CaptionBounds {
			get {
				return captionBoundsCore;
			}
		}
		public Rectangle DescriptionBounds {
			get {
				return descriptionBoundsCore;
			}
		}
		public virtual int ImageToTextDistance { get { return Properties.ImageToTextDistance; } }
		public virtual Size ContentMinSize {
			get { return Properties.ContentMinSize; }
		}
		public virtual int CaptionToDescriptionDistance { get { return Properties.CaptionToDescriptionDistance; } }
		public IWaitingIndicatorProperties Properties { get { return propertiesCore; } }
		AppearanceObject CreateAppearance(AppearanceObject source, AppearanceDefault defaultAppearance) {
			AppearanceObject appearance = new AppearanceObject();
			AppearanceHelper.Combine(appearance, source, defaultAppearance);
			return appearance;
		}
		protected virtual Size CalcCaptionSize(GraphicsCache cache) {
			if(!Properties.ShowCaption) return Size.Empty;
			return CalcTextSize(cache, PaintAppearanceCaption, Caption);
		}
		protected virtual Size CalcDescriptionSize(GraphicsCache cache) {
			if(!Properties.ShowDescription) return Size.Empty;
			return CalcTextSize(cache, PaintAppearanceDescription, Description);
		}
		protected virtual int CalcTextHeight() {
			int bottomPadding = 0;
			if(Properties.ShowCaption && Properties.ShowDescription) {
				double verticalPadding = (CaptionBounds.Height + DescriptionBounds.Height - PaintAppearanceCaption.Font.Size - PaintAppearanceDescription.Font.Size) / 2;
				bottomPadding = (int)Math.Ceiling(verticalPadding);
				if(bottomPadding % 2 != 0)
					bottomPadding += 1;
				bottomPadding /= 2;
			}
			return CaptionBounds.Height + DescriptionBounds.Height + CaptionToDescriptionDistance + bottomPadding;
		}
		protected virtual Size CalcContentSize() {
			int imageToTextDistance = CaptionBounds.Width == 0 && DescriptionBounds.Width == 0 ? 0 : ImageToTextDistance;
			int contentWidth = ImageBounds.Width + Math.Max(CaptionBounds.Width, DescriptionBounds.Width) + imageToTextDistance;
			int contentHeight = Math.Max(ImageBounds.Height, CalcTextHeight());
			return new Size(contentWidth, contentHeight);
		}
		protected virtual Size CalcImageSize() {
			return WaitingAnimator.GetMinSize();
		}
		protected Size CalcTextSize(GraphicsCache cache, AppearanceObject appearance, string text) {
			return Size.Round(appearance.CalcTextSize(cache, text, 0));
		}
		void IWaitingIndicatorInfo.Calc(GraphicsCache cache, IWaitingIndicatorInfoArgs e) {
			imageBoundsCore = new Rectangle(Point.Empty, CalcImageSize());
			captionBoundsCore = new Rectangle(Point.Empty, CalcCaptionSize(cache));
			descriptionBoundsCore = new Rectangle(Point.Empty, CalcDescriptionSize(cache));
			Size contentSize = CalcContentSize();
			contentSize = new Size(Math.Max(ContentMinSize.Width, contentSize.Width), Math.Max(ContentMinSize.Height, contentSize.Height));
			Rectangle content = PlacementHelper.Arrange(contentSize, e.InfoArgs.Bounds, ContentAlignment.MiddleCenter);
			boundsCore = Painter.CalcBoundsByClientRectangle(null, content);
			Arrange(content);
		}
		protected virtual Point CalcImageLocation(Rectangle clientRect) {
			int y = (clientRect.Height - ImageBounds.Height) / 2;
			return new Point(clientRect.X + Properties.ImageOffset, clientRect.Y + y);
		}
		protected virtual Point CalcCaptionLocation(Rectangle clientRect) {
			int x = ImageBounds.Right + ImageToTextDistance;
			int y = DescriptionBounds.Height == 0 ? ImageBounds.Top + (ImageBounds.Height - CaptionBounds.Height) / 2 : clientRect.Y;
			return new Point(x, y);
		}
		protected virtual Point CalcDescriptionLocation(Rectangle clientRect) {
			int x = ImageBounds.Right + ImageToTextDistance;
			int y = CaptionBounds.Height == 0 ? ImageBounds.Top + (ImageBounds.Height - DescriptionBounds.Height) / 2 : CaptionBounds.Bottom + CaptionToDescriptionDistance;
			return new Point(x, y);
		}
		void Arrange(Rectangle clientRect) {
			imageBoundsCore.Offset(CalcImageLocation(clientRect));
			captionBoundsCore.Offset(CalcCaptionLocation(clientRect));
			descriptionBoundsCore.Offset(CalcDescriptionLocation(clientRect));
		}
		#region IDisposable Members
		bool isDisposing;
		void IDisposable.Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				OnDispose();
			}
		}
		protected virtual void OnDispose() {
			Ref.Dispose(ref paintAppearance);
			Ref.Dispose(ref paintAppearanceCaptionCore);
			Ref.Dispose(ref paintAppearanceDescriptionCore);
			propertiesCore = null;
		}
		#endregion
	}
}
