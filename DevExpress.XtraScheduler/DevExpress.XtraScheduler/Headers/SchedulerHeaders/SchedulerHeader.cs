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
using System.Linq;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services.Internal;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class SchedulerHeader : BorderObjectViewInfo, IDisposable, IHeaderCaptionServiceObject{
		#region Fields
		bool isDisposed;
		string caption = String.Empty;
		Rectangle anchorBounds;
		Rectangle separatorBounds;
		Rectangle fullSeparatorBounds;
		Rectangle underlineBounds;
		Rectangle contentBounds;
		Rectangle imageBounds;
		Rectangle textBounds;
		bool hasSeparator;
		bool alternate;
		Image image;
		BaseHeaderAppearance appearance;
		SkinElementInfo cachedSkinElementInfo;
		#endregion
		protected SchedulerHeader() { }
		protected SchedulerHeader(BaseHeaderAppearance appearance) {
			if (appearance == null)
				Exceptions.ThrowArgumentException("appearance", appearance);
			AssignAppearance(appearance);
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool IsDisposed { get { return isDisposed; } }
		protected internal virtual bool AllowAbbreviatedCaption { get { return false; } }
		public string Caption { get { return caption; } set { caption = value; } }
		public Rectangle AnchorBounds { get { return anchorBounds; } set { anchorBounds = value; } }
		public Rectangle SeparatorBounds { get { return separatorBounds; } set { separatorBounds = value; } }
		public Rectangle FullSeparatorBounds { get { return fullSeparatorBounds; } set { fullSeparatorBounds = value; } }
		public Rectangle UnderlineBounds { get { return underlineBounds; } set { underlineBounds = value; } }
		public Rectangle ContentBounds { get { return contentBounds; } set { contentBounds = value; } }
		public Rectangle ImageBounds { get { return imageBounds; } set { imageBounds = value; } }
		public Rectangle TextBounds { get { return textBounds; } set { textBounds = value; } }
		public InterpolationMode ImageInterpolationMode { get { return GetImageInterpolationMode(); } }
		public bool HasSeparator { get { return hasSeparator; } set { hasSeparator = value; } }
		public bool Alternate { get { return alternate; } set { alternate = value; } }
		public virtual bool RotateCaption { get { return false; } }
		public Image Image {
			get { return image; }
			set {
				if (image != null)
					image.Dispose();
				if (value != null)
					image = (Image)value.Clone();
				else
					image = null;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'Appearance' property instead.", false)]
		public BaseHeaderAppearance HeaderAppearance { get { return appearance; } }
		public BaseHeaderAppearance Appearance { get { return appearance; } }
		public AppearanceObject CaptionAppearance { get { return CalcActualCaptionAppearance(false); } }
		public AppearanceObject UnderlineAppearance { get { return alternate ? appearance.AlternateHeaderCaptionLine : appearance.HeaderCaptionLine; } }
		public string SkinElementName { get { return CalcActualSkinElementName(false); } }
		internal SkinElementInfo CachedSkinElementInfo { get { return cachedSkinElementInfo; } set { cachedSkinElementInfo = value; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (image != null) {
					image.Dispose();
					image = null;
				}
				if (appearance != null) {
					appearance.Dispose();
					appearance = null;
				}
				this.cachedSkinElementInfo = null;
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~SchedulerHeader() {
			Dispose(false);
		}
		#endregion
		internal void ReplaceImage(Image newImage) {
			if (image != null)
				image.Dispose();
			if (newImage != null)
				image = newImage;
			else
				image = null;
		}
		internal virtual InterpolationMode GetImageInterpolationMode() {
			return InterpolationMode.Default;
		}
		public void AssignAppearance(BaseHeaderAppearance appearance) {
			this.appearance = new BaseHeaderAppearance();
			this.appearance.Assign(appearance);
		}
		protected internal virtual AppearanceObject CalcActualCaptionAppearance(bool hideSelection) {
			bool isSelected = Selected && !hideSelection;
			return isSelected ? appearance.Selection : (this.alternate ? appearance.AlternateHeaderCaption : appearance.HeaderCaption);
		}
		protected internal virtual string CalcActualSkinElementName(bool hideSelection) {
			bool isSelected = Selected && !hideSelection;
			if (isSelected)
				return Alternate ? SchedulerSkins.SkinHeaderAlternateSelected : SchedulerSkins.SkinHeaderSelected;
			else
				return Alternate ? SchedulerSkins.SkinHeaderAlternate : SchedulerSkins.SkinHeader;
		}
		public virtual SchedulerHeaderPreliminaryLayoutResult CalculateHeaderPreliminaryLayout(GraphicsCache cache, SchedulerHeaderPainter painter) {
			SchedulerHeaderPreliminaryLayoutResult result = CreatePreliminaryLayoutResultObject();
			CalcPreliminaryLayout(cache, painter, result);
			return result;
		}
		public SchedulerHeaderPreliminaryLayoutResult CalculateHeaderPreliminaryLayout(GraphicsCache cache, SchedulerHeaderPainter painter, Size viewSize) {
			SchedulerHeaderPreliminaryLayoutResult result = CreatePreliminaryLayoutResultObject();
			result.ViewSize = viewSize;
			CalcPreliminaryLayout(cache, painter, result);
			return result;
		}
		protected internal virtual void CalcPreliminaryLayoutCore(GraphicsCache cache, SchedulerHeaderPreliminaryLayoutResult preliminaryResult) {
			SchedulerHeaderContentLayoutCalculator layoutCalculator = CreateContentLayoutCalculator(preliminaryResult);
			layoutCalculator.CalcPreliminaryLayout(cache);
		}
		protected virtual void CalculateHeaderAppearances(SchedulerHeaderPainter painter) {
			SetAppearanceParameters(painter, Appearance.HeaderCaption, SchedulerSkins.SkinHeader);
			SetAppearanceParameters(painter, Appearance.AlternateHeaderCaption, SchedulerSkins.SkinHeaderAlternate);
			string selectedSkinElementName = Alternate ? SchedulerSkins.SkinHeaderAlternateSelected : SchedulerSkins.SkinHeaderSelected;
			SetAppearanceParameters(painter, Appearance.Selection, selectedSkinElementName);
		}
		protected internal virtual void SetAppearanceParameters(SchedulerHeaderPainter painter, AppearanceObject appearanceObject, string skinElementName) {
			appearanceObject.ForeColor = painter.GetHeaderForeColor(skinElementName, appearanceObject.ForeColor);
			bool fontBold = painter.GetHeaderFontBold(skinElementName, appearanceObject.Font.Bold);
			if (fontBold != appearanceObject.Font.Bold) {
				FontStyle style = appearanceObject.Font.Style;
				if (fontBold)
					style |= FontStyle.Bold;
				else
					style &= ~FontStyle.Bold;
				appearanceObject.Font = new Font(appearanceObject.Font, style);
			}
		}
		protected internal virtual SchedulerHeaderPreliminaryLayoutResult CreatePreliminaryLayoutResultObject() {
			return new SchedulerHeaderPreliminaryLayoutResult();
		}
		protected internal virtual void CalculateIntermediateParameters(SchedulerHeaderPreliminaryLayoutResult preliminaryResult) {
			SchedulerHeaderPainter headerPainter = preliminaryResult.Painter;
			preliminaryResult.ContentLeftPadding = headerPainter.GetContentLeftPadding(this);
			preliminaryResult.ContentRightPadding = headerPainter.GetContentRightPadding(this);
			preliminaryResult.ContentTopPadding = headerPainter.GetContentTopPadding(this);
			preliminaryResult.ContentBottomPadding = headerPainter.GetContentBottomPadding(this);
			preliminaryResult.ImageTextGapSize = headerPainter.ImageTextGapSize;
		}
		protected internal virtual void CustomizeIntermediateParameters(SchedulerHeaderPreliminaryLayoutResult preliminaryResult) {
			CustomizeIntermediateParametersCore(preliminaryResult);
		}
		protected internal virtual void CustomizeIntermediateParametersCore(SchedulerHeaderPreliminaryLayoutResult preliminaryResult) {
		}
		protected abstract SchedulerHeader CreateCloneInstance();
		public virtual void CalcLayout(GraphicsCache cache, SchedulerHeaderPreliminaryLayoutResult preliminaryResult) {
			if (cache == null)
				Exceptions.ThrowArgumentException("cache", cache);
			if (preliminaryResult == null)
				Exceptions.ThrowArgumentException("preliminaryResult", preliminaryResult);
			SchedulerHeaderPainter headerPainter = preliminaryResult.Painter;
			CalcBorderBounds(headerPainter);
			this.fullSeparatorBounds = CalcFullSeparatorBounds(headerPainter);
			this.separatorBounds = CalcSeparatorMarkBounds(headerPainter);
			this.underlineBounds = CalcUnderlineBounds(headerPainter);
			Rectangle innerBounds = CalcInnerBounds();
			this.contentBounds = CalcContentBounds(preliminaryResult, innerBounds);
			CalcContentLayout(cache, preliminaryResult);
		}
		public override void Assign(ObjectInfoArgs info) {
			base.Assign(info);
			SchedulerHeader h = info as SchedulerHeader;
			if (h == null)
				return;
			this.caption = h.Caption;
			this.anchorBounds = h.anchorBounds;
			this.separatorBounds = h.SeparatorBounds;
			this.fullSeparatorBounds = h.FullSeparatorBounds;
			this.underlineBounds = h.UnderlineBounds;
			this.contentBounds = h.ContentBounds;
			this.imageBounds = h.ImageBounds;
			this.textBounds = h.TextBounds;
			this.hasSeparator = h.HasSeparator;
			this.alternate = h.Alternate;
			if (h.Image != null)
				this.image = (Image)h.Image.Clone();
			this.cachedSkinElementInfo = h.CachedSkinElementInfo;
			if (h.Appearance != null)
				AssignAppearance(h.Appearance);
		}
		protected internal virtual Rectangle CalcInnerBounds() {
			return new Rectangle(fullSeparatorBounds.Right, TopBorderBounds.Bottom, RightBorderBounds.Left - fullSeparatorBounds.Right, underlineBounds.Top - TopBorderBounds.Bottom);
		}
		protected internal virtual Rectangle CalcFullSeparatorBounds(SchedulerHeaderPainter painter) {
			int width = HasSeparator ? painter.HeaderSeparatorWidth : 0;
			return new Rectangle(LeftBorderBounds.Right, TopBorderBounds.Bottom, width, BottomBorderBounds.Top - TopBorderBounds.Bottom);
		}
		protected internal virtual Rectangle CalcUnderlineBounds(SchedulerHeaderPainter painter) {
			return new Rectangle(fullSeparatorBounds.Right, BottomBorderBounds.Top - painter.CaptionLineWidth, RightBorderBounds.Left - fullSeparatorBounds.Right, painter.CaptionLineWidth);
		}
		protected internal virtual Rectangle CalcSeparatorMarkBounds(SchedulerHeaderPainter painter) {
			int offset = painter.HeaderSeparatorPadding;
			return new Rectangle(fullSeparatorBounds.Left, fullSeparatorBounds.Top + offset, fullSeparatorBounds.Width, fullSeparatorBounds.Height - 2 * offset - painter.CaptionLineWidth);
		}
		protected internal virtual Rectangle CalcContentBounds(SchedulerHeaderPreliminaryLayoutResult preliminaryResult, Rectangle innerBounds) {
			Rectangle result = innerBounds;
			result.Y += preliminaryResult.ContentTopPadding;
			result.Height -= preliminaryResult.ContentTopPadding;
			result.Height -= preliminaryResult.ContentBottomPadding;
			result.X += preliminaryResult.ContentLeftPadding;
			result.Width -= preliminaryResult.ContentLeftPadding;
			result.Width -= preliminaryResult.ContentRightPadding;
			return result;
		}
		protected internal virtual SchedulerHeaderContentLayoutCalculator CreateContentLayoutCalculator(SchedulerHeaderPreliminaryLayoutResult preliminaryResult) {
			if (Image == null)
				return new SchedulerHeaderSimpleContentLayoutCalculator(this, preliminaryResult);
			else
				return CreateComplexContentLayoutCalculator(preliminaryResult);
		}
		protected internal virtual SchedulerHeaderComplexContentLayoutCalculator CreateComplexContentLayoutCalculator(SchedulerHeaderPreliminaryLayoutResult preliminaryResult) {
			return CreateComplexContentLayoutCalculatorForRotatedCaption(preliminaryResult);
		}
		protected internal virtual SchedulerHeaderComplexContentLayoutCalculator CreateComplexContentLayoutCalculatorForRotatedCaption(SchedulerHeaderPreliminaryLayoutResult preliminaryResult) {
			switch (preliminaryResult.ImageAlign) {
				case HeaderImageAlign.Left:
					return new SchedulerHeaderComplexContentLayoutCalculatorLeftImageAlign(this, preliminaryResult);
				case HeaderImageAlign.Right:
					return new SchedulerHeaderComplexContentLayoutCalculatorRightImageAlign(this, preliminaryResult);
				case HeaderImageAlign.Top:
					return new SchedulerHeaderComplexContentLayoutCalculatorTopImageAlign(this, preliminaryResult);
				case HeaderImageAlign.Bottom:
					return new SchedulerHeaderComplexContentLayoutCalculatorBottomImageAlign(this, preliminaryResult);
				default:
					Exceptions.ThrowInternalException();
					return null;
			}
		}
		protected internal virtual void CalcContentLayout(GraphicsCache cache, SchedulerHeaderPreliminaryLayoutResult preliminaryResult) {
			SchedulerHeaderContentLayoutCalculator layoutCalculator = CreateContentLayoutCalculator(preliminaryResult);
			layoutCalculator.CalcLayout(cache);
		}
		protected internal virtual Size CalculateOutputImageSize(Size size) {
			return size;
		}
		protected internal virtual Size CalculateTextSizeCore(GraphicsCache cache, Size maxAvailableTextSize, string text) {
			StringInfo stringInfo = StringPainter.Default.Calculate(cache.Graphics, CaptionAppearance, text, maxAvailableTextSize.Width, cache.Paint);
			return stringInfo.Bounds.Size;
		}
		protected internal virtual Size CalculateTextSize(GraphicsCache cache, Size maxAvailableTextSize) {
			string caption = GetActualCaption();
			return CalculateTextSizeCore(cache, maxAvailableTextSize, caption);
		}
		protected internal virtual string GetActualCaption() {
			return Caption;
		}
		protected internal virtual Size CalculateWrappedTextSizeForRotatedCaption(GraphicsCache cache, Size maxAvailableTextSize) {
			return CalculateTextSize(cache, maxAvailableTextSize);
		}
		protected internal virtual Size CalculateTextSizeForRotatedCaption(GraphicsCache cache, Size maxAvailableTextSize) {
			return CalculateTextSize(cache, maxAvailableTextSize);
		}
		protected internal abstract bool RaiseCustomDrawEvent(GraphicsCache cache, ISupportCustomDraw customDrawProvider, DefaultDrawDelegate defaultDrawDelegate);
		void CalcPreliminaryLayout(GraphicsCache cache, SchedulerHeaderPainter painter, SchedulerHeaderPreliminaryLayoutResult preliminaryResult) {
			preliminaryResult.Painter = painter;
			InitializeHeaderImage();
			CalculateHeaderAppearances(painter);
			CalculateIntermediateParameters(preliminaryResult);
			CustomizeIntermediateParameters(preliminaryResult);
			CalcPreliminaryLayoutCore(cache, preliminaryResult);
		}
		protected virtual void InitializeHeaderImage() {
		}
	}
}
