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
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Drawing.Internal;
namespace DevExpress.XtraScheduler.Drawing {
	public static class ViewInfoItemsLayoutHelper {
		public static Rectangle LayoutItem(GraphicsCache cache, ViewInfoItem item, Rectangle availableBounds, int contentGap, ViewInfoItemAlignment alignment) {
			Point itemLocation;
			Size itemSize = item.CalcContentSize(cache, availableBounds);
			int width = Math.Min(itemSize.Width, availableBounds.Width);
			int height = Math.Min(itemSize.Height, availableBounds.Height);
			int cutWidth = Math.Min(availableBounds.Width, width + contentGap);
			int cutHeight = Math.Min(availableBounds.Height, height + contentGap);
			switch (alignment) {
				case ViewInfoItemAlignment.Top:
					itemLocation = availableBounds.Location;
					availableBounds = RectUtils.CutFromTop(availableBounds, cutHeight);
					break;
				case ViewInfoItemAlignment.Right:
					itemLocation = new Point(availableBounds.Right - width, availableBounds.Y);
					availableBounds = RectUtils.CutFromRight(availableBounds, cutWidth);
					break;
				case ViewInfoItemAlignment.Bottom:
					itemLocation = availableBounds.Location;
					availableBounds = RectUtils.CutFromBottom(availableBounds, cutHeight);
					break;
				case ViewInfoItemAlignment.Left:
				default:
					itemLocation = availableBounds.Location;
					availableBounds = RectUtils.CutFromLeft(availableBounds, cutWidth);
					break;
			}
			item.Bounds = new Rectangle(itemLocation.X, itemLocation.Y, width, height);
			return availableBounds;
		}
		public static Rectangle LayoutItemAtLeft(GraphicsCache cache, ViewInfoItem item, Rectangle availableBounds, int contentHorizontalGap) {
			return LayoutItem(cache, item, availableBounds, contentHorizontalGap, ViewInfoItemAlignment.Left);
		}
		public static Rectangle LayoutItemAtRight(GraphicsCache cache, ViewInfoItem item, Rectangle availableBounds, int contentHorizontalGap) {
			return LayoutItem(cache, item, availableBounds, contentHorizontalGap, ViewInfoItemAlignment.Right);
		}
		public static Rectangle LayoutItemAtTop(GraphicsCache cache, ViewInfoItem item, Rectangle availableBounds, int contentVerticalGap) {
			return LayoutItem(cache, item, availableBounds, contentVerticalGap, ViewInfoItemAlignment.Top);
		}
		public static Rectangle LayoutItemAtBottom(GraphicsCache cache, ViewInfoItem item, Rectangle availableBounds, int contentVerticalGap) {
			return LayoutItem(cache, item, availableBounds, contentVerticalGap, ViewInfoItemAlignment.Bottom);
		}
		public static Rectangle LayoutItemsAlignLeft(GraphicsCache cache, Rectangle availableBounds, ViewInfoItemCollection items, int contentHorizontalGap) {
			Rectangle bounds = availableBounds;
			int count = items.Count;
			for (int i = 0; i < count; i++)
				bounds = LayoutItemAtLeft(cache, items[i], bounds, contentHorizontalGap);
			return bounds;
		}
		public static Rectangle LayoutItemsAlignRight(GraphicsCache cache, Rectangle availableBounds, ViewInfoItemCollection items, int contentHorizontalGap) {
			Rectangle bounds = availableBounds;
			int count = items.Count;
			for (int i = 0; i < count; i++)
				bounds = LayoutItemAtRight(cache, items[i], bounds, contentHorizontalGap);
			return bounds;
		}
		public static Rectangle LayoutItemsAlignTop(GraphicsCache cache, Rectangle availableBounds, ViewInfoItemCollection items, int contentVerticalGap) {
			Rectangle bounds = availableBounds;
			int count = items.Count;
			for (int i = 0; i < count; i++)
				bounds = LayoutItemAtTop(cache, items[i], bounds, contentVerticalGap);
			return bounds;
		}
		public static void CenterItemsHorizontally(Rectangle availableBounds, ViewInfoItemCollection items) {
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				ViewInfoItem item = items[i];
				Rectangle rect = item.Bounds;
				Rectangle baseRect = new Rectangle(availableBounds.Left, rect.Top, availableBounds.Width, rect.Height);
				item.Bounds = RectUtils.AlignRectangle(rect, baseRect, ContentAlignment.MiddleCenter);
			}
		}
	}
	public class AppointmentViewInfoStatusItem : ViewInfoItem {
		#region Fields
		AppointmentStatusViewInfo backgroundViewInfo;
		AppointmentStatusViewInfo foregroundViewInfo;
		int maskImageIndex;
		Image cachedImage;
		string skinElementName;
		ViewInfoItemAlignment alignment;
		#endregion
		public AppointmentViewInfoStatusItem(AppointmentViewInfo aptViewInfo, TimeInterval statusInterval) {
			Guard.ArgumentNotNull(aptViewInfo, "aptViewInfo");
			Guard.ArgumentNotNull(statusInterval, "statusInterval");
			InitializeStatusViewInfos(aptViewInfo, statusInterval);
		}
		#region Properties
		public virtual AppointmentStatusViewInfo BackgroundViewInfo { get { return backgroundViewInfo; } }
		public virtual AppointmentStatusViewInfo ForegroundViewInfo { get { return foregroundViewInfo; } }
		protected internal virtual string SkinElementName { get { return skinElementName; } set { skinElementName = value; } }
		public virtual ViewInfoItemAlignment Alignment { get { return alignment; } protected internal set { alignment = value; } }
		public override Rectangle Bounds { get { return backgroundViewInfo.Bounds; } set { backgroundViewInfo.Bounds = value; } }
		public int MaskImageIndex { get { return maskImageIndex; } set { maskImageIndex = value; } }
		public Image CachedImage { get { return cachedImage; } set { cachedImage = value; } }
		#endregion
		protected internal virtual void InitializeStatusViewInfos(AppointmentViewInfo aptViewInfo, TimeInterval statusInterval) {
			Color borderColor = aptViewInfo.Appearance.GetBorderColor();
			foregroundViewInfo = new AppointmentStatusViewInfo(aptViewInfo.Status, borderColor);
			backgroundViewInfo = new AppointmentStatusViewInfo(AppointmentStatus.Empty, borderColor);
			foregroundViewInfo.Interval = statusInterval;
			backgroundViewInfo.Interval = aptViewInfo.Interval;
		}
		protected internal override Size CalcContentSize(GraphicsCache cache, Rectangle availableBounds) {
			return Bounds.Size;
		}
		public override void Draw(GraphicsCache cache, IViewInfoItemPainter painter) {
			AppointmentPainter aptPainter = (AppointmentPainter)painter;
			aptPainter.DrawStatusItem(cache, this);
		}
	}
	public class AppointmentViewInfoClockImageItem : ViewInfoImageItem {
		DateTime time;
		Color arrowsColor;
		public AppointmentViewInfoClockImageItem(Image image)
			: base(image) {
		}
		public DateTime Time { get { return time; } set { time = value; } }
		public Color ArrowsColor { get { return arrowsColor; } set { arrowsColor = value; } }
		public override void Draw(GraphicsCache cache, IViewInfoItemPainter painter) {
			base.Draw(cache, painter);
			ClockArrowsPainterHelper.DrawClockArrows(cache, this);
		}
	}
	#region AppointmentViewInfoClockTextItem
	public class AppointmentViewInfoClockTextItem : ViewInfoTextItem {
		protected internal override Size CalcContentSize(GraphicsCache cache, Rectangle availableBounds) {
			availableBounds.Width = Int32.MaxValue;
			return base.CalcContentSize(cache, availableBounds);
		}
	}
	#endregion
	public class AppointmentImageInfo : AppointmentImageInfoCore {
		Image image;
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentImageInfoImage")]
#endif
		public Image Image { get { return image; } set { image = value; } }
	}
	public class AppointmentImageInfoCollection : AppointmentImageInfoCoreCollection<AppointmentImageInfo> {
		#region Fields
		object images;
		#endregion
		public AppointmentImageInfoCollection(AppointmentImageProvider imageProvider)
			: base(imageProvider) {
		}
		#region Properties
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentImageInfoCollectionImages")]
#endif
		public object Images { get { return images; } set { images = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentImageInfoCollectionInnerImages")]
#endif
		public object InnerImages { get { return images != null ? images : ImageProvider.DefaultAppointmentImages; } }
		#endregion
		protected internal override void SetImage(AppointmentImageType type, AppointmentImageInfo info) {
			Image image = ((AppointmentImageProvider)ImageProvider).GetAppointmentImage(type);
			info.Image = image;
		}
	}
	#region AppointmentContentLayoutCalculator (abstract)
	public abstract class AppointmentContentLayoutCalculator : AppointmentContentLayoutCalculatorCore<ViewInfoItemCollection, AppointmentImageInfoCollection> {
		#region Fields
		AppearanceObject defaultAppointmentAppearance;
		AppointmentPainter painter;
		int contentHorizontalGap = 0;
		object appointmentImages;
		Image appointmentStartContinueArrow;
		Image appointmentEndContinueArrow;
		#endregion
		#region Events
		#region AppointmentViewInfoCustomizing
		AppointmentViewInfoCustomizingEventHandler onAppointmentViewInfoCustomizing;
		public event AppointmentViewInfoCustomizingEventHandler AppointmentViewInfoCustomizing { add { onAppointmentViewInfoCustomizing += value; } remove { onAppointmentViewInfoCustomizing -= value; } }
		protected internal virtual void RaiseAppointmentViewInfoCustomizing(AppointmentViewInfo aptViewInfo) {
			if (onAppointmentViewInfoCustomizing != null) {
				AppointmentViewInfoCustomizingEventArgs args = new AppointmentViewInfoCustomizingEventArgs(aptViewInfo);
				onAppointmentViewInfoCustomizing(this, args);
			}
		}
		#endregion
		#region InitAppointmentImages
		AppointmentImagesEventHandler onInitAppointmentImages;
		public event AppointmentImagesEventHandler InitAppointmentImages { add { onInitAppointmentImages += value; } remove { onInitAppointmentImages -= value; } }
		protected internal override void RaiseInitAppointmentImages(IAppointmentViewInfo aptViewInfo, AppointmentImageInfoCollection imageInfos) {
			if (onInitAppointmentImages != null) {
				AppointmentImagesEventArgs args = new AppointmentImagesEventArgs(aptViewInfo, imageInfos);
				onInitAppointmentImages(this, args);
			}
		}
		#endregion
		#endregion
		protected AppointmentContentLayoutCalculator(ISupportAppointments viewInfo, AppointmentPainter painter)
			: base(viewInfo) {
			if (painter == null)
				Exceptions.ThrowArgumentNullException("painter");
			this.painter = painter;
			Initialize();
		}
		protected internal virtual void Initialize() {
			this.defaultAppointmentAppearance = ViewInfo.AppointmentAppearance;
			this.appointmentImages = CreateAppointmentImages();
			this.contentHorizontalGap = Painter.ContentHorizontalGap;
			this.appointmentStartContinueArrow = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraScheduler.Images.appointmentStartContinueArrow.png", System.Reflection.Assembly.GetExecutingAssembly());
			this.appointmentEndContinueArrow = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraScheduler.Images.appointmentEndContinueArrow.png", System.Reflection.Assembly.GetExecutingAssembly());
		}
		#region Properties
		protected internal new ISupportAppointments ViewInfo { get { return (ISupportAppointments)base.ViewInfo; } }
		protected internal AppointmentDisplayOptions AppointmentDisplayOptions { get { return ViewInfo.AppointmentDisplayOptions; } }
		protected internal virtual AppointmentStatusDisplayType StatusDisplayType { get { return AppointmentDisplayOptions.StatusDisplayType; } }
		protected internal AppearanceObject DefaultAppointmentAppearance { get { return defaultAppointmentAppearance; } }
		protected internal AppointmentPainter Painter { get { return painter; } }
		protected internal int ContentHorizontalGap { get { return contentHorizontalGap; } }
		protected internal virtual ViewInfoItemAlignment StatusAlignment { get { return AppointmentDisplayOptions.StatusAlignment; } }
		protected internal object AppointmentImages { get { return appointmentImages; } }
		protected internal virtual Image AppointmentStartContinueArrow { get { return appointmentStartContinueArrow; } }
		protected internal virtual Image AppointmentEndContinueArrow { get { return appointmentEndContinueArrow; } }
		#endregion
		protected internal abstract void SetStatusItemBorders(AppointmentViewInfoStatusItem statusItem, bool hasStartBorder, bool hasEndBorder);
		protected internal abstract Rectangle CalculateStatusViewInfoBounds(AppointmentViewInfoStatusItem item);
		protected internal abstract String CalculateStatusItemSkinElemenName();
		protected internal abstract void CalculatePreliminaryContentLayoutCore(GraphicsCache cache, AppointmentViewInfo aptViewInfo);
		protected internal abstract void OffsetViewInfoItems(GraphicsCache cache, AppointmentViewInfo aptViewInfo);
		protected internal virtual object CreateAppointmentImages() {
			object appointmentImages = ViewInfo.AppointmentImages;
			if (appointmentImages == null)
				return Painter.DefaultAppointmentImages;
			else
				return appointmentImages;
		}
		protected internal override IViewInfoTextItem CreateClockTextItemInstance() {
			return new ViewInfoHorizontalTextItem();
		}
		protected internal override IAppointmentFormatStringService GetFormatStringProvider() {
			return ViewInfo.GetFormatStringProvider();
		}
		public virtual void CalculateContentLayout(GraphicsCache cache, AppointmentViewInfoCollection viewInfos) {
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++) {
				if (ViewInfo.CancellationToken.IsCancellationRequested)
					return;
				CalculateContentLayoutCore(cache, viewInfos[i]);
			}
		}
		protected internal virtual void CalculateContentLayoutCore(GraphicsCache cache, AppointmentViewInfo aptViewInfo) {
			CalculatePreliminaryContentLayout(cache, aptViewInfo);
			CalculateFinalContentLayout(cache, aptViewInfo);
		}
		public virtual void CalculatePreliminaryContentLayout(GraphicsCache cache, AppointmentViewInfo aptViewInfo) {
			CalculateViewInfoProperties(aptViewInfo);
			RaiseAppointmentViewInfoCustomizing(aptViewInfo);
			CreateAndLayoutStatusItems(aptViewInfo);
			CalculatePreliminaryContentLayoutCore(cache, aptViewInfo);
		}
		protected internal virtual void CreateAndLayoutStatusItems(AppointmentViewInfo aptViewInfo) {
			aptViewInfo.StatusItems.Clear();
			ViewInfoItemCollection statusItems = CreateStatusItems(aptViewInfo);
			LayoutStatusItems(aptViewInfo, statusItems);
			if (statusItems.Count != 0)
				RecalcInnerBounds(aptViewInfo);
			aptViewInfo.StatusItems.AddRange(statusItems);
		}
		public virtual void CalculateFinalContentLayout(GraphicsCache cache, AppointmentViewInfo aptViewInfo) {
			OffsetViewInfoItems(cache, aptViewInfo);
			if (CanUpdateToolTipText(aptViewInfo))
				UpdateToolTipText(aptViewInfo);
			if (CanUpdateToolVisibility(aptViewInfo))
				UpdateToolTipVisibility(cache, aptViewInfo);
		}
		protected internal virtual bool CanUpdateToolTipText(AppointmentViewInfo aptViewInfo) {
			return String.IsNullOrEmpty(aptViewInfo.ToolTipText);
		}
		protected internal virtual void UpdateToolTipText(AppointmentViewInfo aptViewInfo) {
			aptViewInfo.ToolTipText = aptViewInfo.DisplayText;
		}
		protected internal virtual bool CanUpdateToolVisibility(AppointmentViewInfo aptViewInfo) {
			return !aptViewInfo.ShouldShowToolTip;
		}
		protected internal virtual void UpdateToolTipVisibility(GraphicsCache cache, AppointmentViewInfo aptViewInfo) {
			ViewInfoTextItem item = aptViewInfo.DisplayTextItem;
			if (item != null) {
				if (String.IsNullOrEmpty(aptViewInfo.ToolTipText))
					return;
				if (SchedulerWinUtils.IsWordWrap(item.Appearance))
					aptViewInfo.ShouldShowToolTip = CalculateWrappedTextToolTipVisibility(cache, item);
				else
					aptViewInfo.ShouldShowToolTip = CalculateUnwrappedTextToolTipVisibility(cache, item);
			}
		}
		protected internal virtual bool CalculateUnwrappedTextToolTipVisibility(GraphicsCache cache, ViewInfoTextItem item) {
			Size size = item.CalcContentSize(cache, new Rectangle(0, 0, Int32.MaxValue, Int32.MaxValue));
			return (size.Width > item.Bounds.Width) || (size.Height > item.Bounds.Height);
		}
		protected internal virtual bool CalculateWrappedTextToolTipVisibility(GraphicsCache cache, ViewInfoTextItem item) {
			Size size = item.CalcContentSize(cache, new Rectangle(0, 0, item.Bounds.Width, Int32.MaxValue));
			if (size.Height == item.Bounds.Height)
				return CalculateUnwrappedTextToolTipVisibility(cache, item);
			else
				return size.Height > item.Bounds.Height || item.Bounds.Width <= 0;
		}
		protected internal virtual void CalculateViewInfoBounds(AppointmentViewInfo aptViewInfo) {
			aptViewInfo.CalcBorderBounds(Painter);
			aptViewInfo.CalcInnerBounds(Painter);
		}
		protected internal virtual void CalculateViewInfoProperties(AppointmentViewInfo aptViewInfo) {
			ContentCalculatorHelper.CalculateViewInfoOptions(aptViewInfo);
			aptViewInfo.Status = ViewInfo.GetStatus(aptViewInfo.Appointment.StatusKey);
			CalculateViewInfoBounds(aptViewInfo);
			AppearanceHelper.Combine(aptViewInfo.Appearance, new AppearanceObject[] { this.DefaultAppointmentAppearance });
			aptViewInfo.Appearance.BackColor = CalculateAppointmentBackColor(aptViewInfo.Appearance.BackColor, aptViewInfo.Appointment.LabelKey);
			aptViewInfo.Appearance.BorderColor = Painter.GetBorderColor(aptViewInfo.Appearance);
			if (!ViewInfo.OverriddenAppointmentForeColor)
				aptViewInfo.Appearance.ForeColor = Painter.GetForeColor(aptViewInfo.Appearance);
		}
		protected internal virtual Color CalculateAppointmentBackColor(Color appearanceColor, object labelId) {
			if ((appearanceColor == SystemColors.Window) || (appearanceColor == Color.Empty))
				return ViewInfo.GetLabelColor(labelId);
			else
				return appearanceColor;
		}
		protected internal virtual void CalculateTextItemsAppearance(ViewInfoItemCollection items, AppearanceObject aptViewInfoAppearance, WordWrap wordWrap) {
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				ViewInfoTextItem textItem = (ViewInfoTextItem)items[i];
				CalculateTextItemAppearance(textItem, aptViewInfoAppearance, wordWrap);
			}
		}
		protected internal virtual void CalculateTextItemAppearance(ViewInfoTextItem textItem, AppearanceObject aptViewInfoAppearance, WordWrap wordWrap) {
			AppearanceHelper.Combine(textItem.Appearance, new AppearanceObject[] { aptViewInfoAppearance });
			textItem.Appearance.TextOptions.WordWrap = wordWrap;
		}
		protected internal virtual ViewInfoItemCollection CreateStatusItems(AppointmentViewInfo aptViewInfo) {
			switch (aptViewInfo.Options.StatusDisplayType) {
				case AppointmentStatusDisplayType.Never:
				default:
					return new ViewInfoItemCollection();
				case AppointmentStatusDisplayType.Bounds:
					return CreateStatusItemsCore(aptViewInfo, aptViewInfo.Interval);
				case AppointmentStatusDisplayType.Time:
					TimeInterval aptInterval = aptViewInfo.AppointmentInterval;
					return CreateStatusItemsCore(aptViewInfo, aptInterval);
			}
		}
		protected internal virtual ViewInfoItemCollection CreateStatusItemsCore(AppointmentViewInfo aptViewInfo, TimeInterval statusInterval) {
			ViewInfoItemCollection statusItems = new ViewInfoItemCollection();
			AppointmentViewInfoStatusItem item = new AppointmentViewInfoStatusItem(aptViewInfo, statusInterval);
			item.SkinElementName = CalculateStatusItemSkinElemenName();
			item.Alignment = StatusAlignment;
			statusItems.Add(item);
			return statusItems;
		}
		protected internal virtual void LayoutStatusItems(AppointmentViewInfo aptViewInfo, ViewInfoItemCollection statusItems) {
			int count = statusItems.Count;
			Rectangle statusItemBounds = CalculateStatusItemBounds(aptViewInfo);
			for (int i = 0; i < count; i++)
				LayoutStatusItemsCore(aptViewInfo, (AppointmentViewInfoStatusItem)statusItems[i], statusItemBounds);
		}
		protected internal virtual void SetStatusItemBorders(AppointmentViewInfoStatusItem item) {
			TimeInterval statusInterval = item.ForegroundViewInfo.Interval;
			TimeInterval viewInfoInterval = item.BackgroundViewInfo.Interval;
			bool IsDuartionNotZero = (statusInterval.Duration != TimeSpan.Zero);
			bool hasStartBorder = IsDuartionNotZero && (statusInterval.Start != viewInfoInterval.Start);
			bool hasEndBorder = IsDuartionNotZero && (statusInterval.End != viewInfoInterval.End);
			SetStatusItemBorders(item, hasStartBorder, hasEndBorder);
		}
		protected internal virtual Rectangle CalculateStatusItemBounds(AppointmentViewInfo aptViewInfo) {
			int appointmentStatusSize = CalculateAppointmentStatusSize(aptViewInfo);
			Rectangle leftBorderBounds = aptViewInfo.LeftBorderBounds;
			Rectangle rightBorderBounds = aptViewInfo.RightBorderBounds;
			int left = leftBorderBounds.Right + Painter.CalcLeftStatusOffset(leftBorderBounds);
			int right = rightBorderBounds.Left + Painter.CalcRightStatusOffset(rightBorderBounds);
			int top = aptViewInfo.TopBorderBounds.Bottom;
			int bottom = aptViewInfo.BottomBorderBounds.Top;
			switch (StatusAlignment) {
				case ViewInfoItemAlignment.Left:
					right = left + appointmentStatusSize;
					break;
				case ViewInfoItemAlignment.Top:
					bottom = top + appointmentStatusSize;
					break;
				case ViewInfoItemAlignment.Right:
					left = right - appointmentStatusSize;
					break;
				case ViewInfoItemAlignment.Bottom:
					top = bottom - appointmentStatusSize;
					break;
			}
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
		protected internal virtual void LayoutStatusItemsCore(AppointmentViewInfo aptViewInfo, AppointmentViewInfoStatusItem statusItem, Rectangle statusItemBounds) {
			statusItem.Bounds = statusItemBounds;
			Rectangle statusViewInfoBounds = CalculateStatusViewInfoBounds(statusItem);
			statusItem.ForegroundViewInfo.Bounds = statusViewInfoBounds;
			SetStatusItemBorders(statusItem);
			statusItem.MaskImageIndex = aptViewInfo.CalcAppointmenStatusMaskImageIndex();
			lock (statusItem.BackgroundViewInfo.Brush)
				Painter.CacheStatusImage(statusItem);
		}
		protected internal virtual int CalculateAppointmentStatusSize(AppointmentViewInfo aptViewInfo) {
			if (aptViewInfo.Options.StatusDisplayType == AppointmentStatusDisplayType.Never)
				return 0;
			if ((StatusAlignment == ViewInfoItemAlignment.Left) || (StatusAlignment == ViewInfoItemAlignment.Right))
				return Painter.VerticalStatusLineWidth;
			else
				return Painter.HorizontalStatusLineHeight;
		}
		protected internal virtual void RecalcInnerBounds(AppointmentViewInfo aptViewInfo) {
			int appointmentStatusSize = CalculateAppointmentStatusSize(aptViewInfo);
			Rectangle bounds = aptViewInfo.InnerBounds;
			switch (StatusAlignment) {
				case ViewInfoItemAlignment.Left:
					bounds = RectUtils.CutFromLeft(bounds, appointmentStatusSize);
					break;
				case ViewInfoItemAlignment.Top:
					bounds = RectUtils.CutFromTop(bounds, appointmentStatusSize);
					break;
				case ViewInfoItemAlignment.Right:
					bounds = RectUtils.CutFromRight(bounds, appointmentStatusSize);
					break;
				case ViewInfoItemAlignment.Bottom:
					bounds = RectUtils.CutFromBottom(bounds, appointmentStatusSize);
					break;
			}
			aptViewInfo.InnerBounds = bounds;
		}
		protected internal virtual Rectangle LayoutItemsAlignLeft(GraphicsCache cache, Rectangle availableBounds, ViewInfoItemCollection items) {
			return ViewInfoItemsLayoutHelper.LayoutItemsAlignLeft(cache, availableBounds, items, ContentHorizontalGap);
		}
		protected internal virtual Rectangle LayoutItemsAlignRight(GraphicsCache cache, Rectangle availableBounds, ViewInfoItemCollection items) {
			return ViewInfoItemsLayoutHelper.LayoutItemsAlignRight(cache, availableBounds, items, ContentHorizontalGap);
		}
		protected internal virtual Rectangle LayoutItemsAlignTop(GraphicsCache cache, Rectangle availableBounds, ViewInfoItemCollection items) {
			return ViewInfoItemsLayoutHelper.LayoutItemsAlignTop(cache, availableBounds, items, ContentHorizontalGap);
		}
		protected internal override void FillAppointmentViewInfoImageItems(AppointmentImageInfoCollection imageInfos, ViewInfoItemCollection items) {
			int count = imageInfos.Count;
			for (int i = 0; i < count; i++) {
				if (imageInfos[i].Visible) {
					Image image = GetAppointmentImage(imageInfos, i);
					if (image != null)
						items.Add(new ViewInfoImageItem(image));
				}
			}
		}
		protected internal virtual Image GetAppointmentImage(AppointmentImageInfoCollection imageInfos, int index) {
			AppointmentImageInfo imageInfo = imageInfos[index];
			Image image = imageInfo.Image;
			if (image == null)
				image = GetImageFromInnerImages(imageInfos, imageInfo.ImageIndex);
			return image;
		}
		protected internal virtual Image GetImageFromInnerImages(AppointmentImageInfoCollection imageInfos, int imageIndex) {
			return ImageCollection.GetImageListImage(imageInfos.InnerImages, imageIndex);
		}
		protected internal override AppointmentImageInfoCollection CreateImageInfoCollection() {
			AppointmentImageInfoCollection result = new AppointmentImageInfoCollection(Painter.ImageProvider);
			result.Images = AppointmentImages;
			return result;
		}
		protected internal virtual Color CalculateClockArrowsColor(DateTime time) {
			return IsNightTime(time) ? Painter.NightClockArrowsColor : Painter.DayClockArrowsColor;
		}
	}
	#endregion
	public class HorizontalAppointmentContentLayoutCalculator : AppointmentContentLayoutCalculator {
		public HorizontalAppointmentContentLayoutCalculator(ISupportAppointments viewInfo, AppointmentPainter painter)
			: base(viewInfo, painter) {
		}
		protected internal new HorizontalAppointmentContentCalculatorHelper ContentCalculatorHelper { get { return (HorizontalAppointmentContentCalculatorHelper)base.ContentCalculatorHelper; } }
		protected internal override AppointmentContentCalculatorHelper CreateContentLayoutCalculatorHelper() {
			return new HorizontalAppointmentContentCalculatorHelper(ViewInfo, StatusDisplayType, FormatProvider);
		}
		protected internal override IViewInfoTextItem CreateTextItemInstance() {
			if (AppointmentDisplayOptions.AppointmentAutoHeight)
				return new ViewInfoHorizontalAutoHeightTextItem();
			else
				return new ViewInfoHorizontalTextItem();
		}
		protected internal override void CalculatePreliminaryContentLayoutCore(GraphicsCache cache, AppointmentViewInfo aptViewInfo) {
			aptViewInfo.Items.Clear();
			ViewInfoItemCollection viewInfoItems = CreateAppointmentViewInfoItemCollection(aptViewInfo);
			if (aptViewInfo.IsLongTime())
				CalculateContentLayoutLongAppointment(cache, aptViewInfo, viewInfoItems);
			else
				CalculateContentLayoutShortAppointment(cache, aptViewInfo, viewInfoItems);
		}
		protected internal virtual void CalculateContentLayoutShortAppointment(GraphicsCache cache, AppointmentViewInfo aptViewInfo, ViewInfoItemCollection viewInfoItems) {
			Rectangle availableBounds = aptViewInfo.InnerBounds;
			ViewInfoItemCollection clockItems = CreateClockItems(aptViewInfo);
			InsertGapsIntoClockItems(clockItems);
			availableBounds = LayoutClockItemsShortAppointment(aptViewInfo, clockItems, availableBounds, cache);
			LayoutViewInfoItemsShortAppointment(aptViewInfo, viewInfoItems, availableBounds, cache);
		}
		protected internal virtual void CalculateContentLayoutLongAppointment(GraphicsCache cache, AppointmentViewInfo aptViewInfo, ViewInfoItemCollection viewInfoItems) {
			Rectangle availableBounds = aptViewInfo.InnerBounds;
			availableBounds = CreateAndLayoutContinueItems(aptViewInfo, availableBounds, cache);
			ViewInfoItemCollection clockItems = CreateClockItems(aptViewInfo);
			availableBounds = LayoutClockItemsLongAppointment(aptViewInfo, clockItems, availableBounds, cache);
			LayoutViewInfoItemsLongAppointment(aptViewInfo, viewInfoItems, availableBounds, cache);
		}
		protected internal virtual void LayoutViewInfoItemsShortAppointment(AppointmentViewInfo aptViewInfo, ViewInfoItemCollection viewInfoItems, Rectangle availableBounds, GraphicsCache cache) {
			LayoutItemsAlignLeft(cache, availableBounds, viewInfoItems);
			aptViewInfo.Items.AddRange(viewInfoItems);
		}
		protected internal virtual void LayoutViewInfoItemsLongAppointment(AppointmentViewInfo aptViewInfo, ViewInfoItemCollection viewInfoItems, Rectangle availableBounds, GraphicsCache cache) {
			Rectangle freeBounds = LayoutItemsAlignLeft(cache, availableBounds, viewInfoItems);
			freeBounds = RemoveLastGap(freeBounds, viewInfoItems);
			OffsetViewInfoItemsHorizontally(viewInfoItems, freeBounds.Width / 2);
			aptViewInfo.Items.AddRange(viewInfoItems);
		}
		protected internal virtual void InsertGapsIntoClockItems(ViewInfoItemCollection clockItems) {
			if (clockItems.Count == 2)
				InsertHorizontalGap(clockItems, 1);
			if (clockItems.Count != 0)
				InsertHorizontalGap(clockItems, clockItems.Count);
		}
		protected internal virtual void InsertHorizontalGap(ViewInfoItemCollection items, int index) {
			ViewInfoFreeSpaceItem gapItem = new ViewInfoFreeSpaceItem();
			gapItem.Size = new Size(ContentHorizontalGap, 0);
			index = Math.Max(index, 0);
			index = Math.Min(index, items.Count);
			items.Insert(index, gapItem);
		}
		protected internal virtual Rectangle CreateAndLayoutContinueItems(AppointmentViewInfo aptViewInfo, Rectangle availableBounds, GraphicsCache cache) {
			ViewInfoItemCollection startContinueItems = CreateStartContinueItems(aptViewInfo);
			ViewInfoItemCollection endContinueItems = CreateEndContinueItems(aptViewInfo);
			AdjustContinueItems(aptViewInfo, startContinueItems, endContinueItems, availableBounds, cache);
			availableBounds = LayoutContinueItems(aptViewInfo, startContinueItems, endContinueItems, availableBounds, cache);
			return availableBounds;
		}
		protected internal virtual ViewInfoItemCollection CreateStartContinueItems(AppointmentViewInfo aptViewInfo) {
			ViewInfoItemCollection items = new ViewInfoItemCollection();
			AppointmentContinueArrowDisplayType type = aptViewInfo.Options.StartContinueItemDisplayType;
			if (type != AppointmentContinueArrowDisplayType.Never) {
				items.Add(CreateStartContinueImageItem());
				if (ShouldShowContinueTextItems(type))
					items.Add(CreateStartContinueTextItem(aptViewInfo));
			}
			return items;
		}
		protected internal virtual ViewInfoItemCollection CreateEndContinueItems(AppointmentViewInfo aptViewInfo) {
			ViewInfoItemCollection items = new ViewInfoItemCollection();
			AppointmentContinueArrowDisplayType type = aptViewInfo.Options.EndContinueItemDisplayType;
			if (type != AppointmentContinueArrowDisplayType.Never) {
				items.Add(CreateEndContinueImageItem());
				if (ShouldShowContinueTextItems(type))
					items.Add(CreateEndContinueTextItem(aptViewInfo));
			}
			return items;
		}
		protected internal virtual ViewInfoImageItem CreateStartContinueImageItem() {
			return new ViewInfoImageItem(AppointmentStartContinueArrow);
		}
		protected internal virtual ViewInfoImageItem CreateEndContinueImageItem() {
			return new ViewInfoImageItem(AppointmentEndContinueArrow);
		}
		protected internal virtual ViewInfoTextItem CreateEndContinueTextItem(AppointmentViewInfo aptViewInfo) {
			ViewInfoTextItem item = CreateContinueTextItem(aptViewInfo);
			item.Text = GetEndContinueItemText(aptViewInfo);
			return item;
		}
		protected internal virtual ViewInfoTextItem CreateStartContinueTextItem(AppointmentViewInfo aptViewInfo) {
			ViewInfoTextItem item = CreateContinueTextItem(aptViewInfo);
			item.Text = GetStartContinueItemText(aptViewInfo);
			return item;
		}
		protected internal virtual ViewInfoTextItem CreateContinueTextItem(AppointmentViewInfo aptViewInfo) {
			ViewInfoTextItem item = new ViewInfoHorizontalTextItem();
			AppearanceObject appearance = aptViewInfo.Appearance;
			CalculateTextItemAppearance(item, appearance, WordWrap.NoWrap);
			return item;
		}
		protected internal virtual void AdjustContinueItems(AppointmentViewInfo aptViewInfo, ViewInfoItemCollection startContinueItems, ViewInfoItemCollection endContinueItems, Rectangle availableBounds, GraphicsCache cache) {
			int totalItemWidth = CalculateTotalContinueItemsWidth(startContinueItems, endContinueItems, cache);
			if (!ShouldRemoveContinueTextItems(availableBounds, totalItemWidth))
				return;
			if (aptViewInfo.Options.StartContinueItemDisplayType == AppointmentContinueArrowDisplayType.Auto)
				RemoveContinueTextItems(startContinueItems);
			if (aptViewInfo.Options.EndContinueItemDisplayType == AppointmentContinueArrowDisplayType.Auto)
				RemoveContinueTextItems(endContinueItems);
		}
		protected internal virtual Rectangle LayoutContinueItems(AppointmentViewInfo aptViewInfo, ViewInfoItemCollection startContinueItems, ViewInfoItemCollection endContinueItems, Rectangle availableBounds, GraphicsCache cache) {
			availableBounds = LayoutItemsAlignLeft(cache, availableBounds, startContinueItems);
			availableBounds = LayoutItemsAlignRight(cache, availableBounds, endContinueItems);
			aptViewInfo.Items.AddRange(startContinueItems);
			aptViewInfo.Items.AddRange(endContinueItems);
			return availableBounds;
		}
		protected internal virtual void RemoveContinueTextItems(ViewInfoItemCollection continueItems) {
			if (continueItems.Count == 2)
				continueItems.RemoveAt(1);
		}
		protected internal virtual bool ShouldRemoveContinueTextItems(Rectangle availableBounds, int totalContinueItemsWidth) {
			if (totalContinueItemsWidth < availableBounds.Width / 2)
				return false;
			else
				return true;
		}
		protected internal virtual int CalculateTotalContinueItemsWidth(ViewInfoItemCollection startContinueItems, ViewInfoItemCollection endContinueItems, GraphicsCache cache) {
			return CalculateTotalItemsWidth(startContinueItems, cache) + CalculateTotalItemsWidth(endContinueItems, cache);
		}
		protected internal virtual int CalculateTotalItemsWidth(ViewInfoItemCollection items, GraphicsCache cache) {
			int totalWidth = 0;
			Rectangle bounds = new Rectangle(0, 0, Int32.MaxValue, Int32.MaxValue);
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				Size size = items[i].CalcContentSize(cache, bounds);
				totalWidth += size.Width;
			}
			return totalWidth;
		}
		protected internal virtual ViewInfoItemCollection CreateAppointmentViewInfoTextItems(AppointmentViewInfo aptViewInfo) {
			ViewInfoItemCollection items = base.CreateAppointmentViewInfoTextItems(aptViewInfo);
			WordWrap wordWrap = CalculateWordWrap(aptViewInfo.Appearance);
			CalculateTextItemsAppearance(items, aptViewInfo.Appearance, wordWrap);
			return items;
		}
		protected internal virtual WordWrap CalculateWordWrap(AppearanceObject aptAppearance) {
			if (AppointmentDisplayOptions.AppointmentAutoHeight)
				return WordWrap.Wrap;
			if (aptAppearance.TextOptions.WordWrap == WordWrap.Default)
				return WordWrap.NoWrap;
			else
				return aptAppearance.TextOptions.WordWrap;
		}
		protected internal virtual ViewInfoItemCollection CreateAppointmentViewInfoItemCollection(AppointmentViewInfo aptViewInfo) {
			ViewInfoItemCollection imageItems = CreateAppointmentViewInfoImageItems(aptViewInfo);
			ViewInfoItemCollection textItems = CreateAppointmentViewInfoTextItems(aptViewInfo);
			ViewInfoTextItem titleTextItem = (ViewInfoTextItem)GetTitleTextItem(textItems);
			ViewInfoItemCollection items = new ViewInfoItemCollection();
			items.AddRange(imageItems);
			titleTextItem.Text += GetPercentCompleteString(aptViewInfo);
			items.Add(titleTextItem);
			aptViewInfo.DisplayTextItem = titleTextItem;
			return items;
		}
		protected internal virtual string GetPercentCompleteString(AppointmentViewInfo aptViewInfo) {
			PercentCompleteDisplayType type = aptViewInfo.Options.PercentCompleteDisplayType;
			if (type == PercentCompleteDisplayType.Both || type == PercentCompleteDisplayType.Number) {
				Appointment apt = aptViewInfo.Appointment;
				string format = String.IsNullOrEmpty(apt.Location) && String.IsNullOrEmpty(apt.Subject) ? "{0}%" : " - {0}%";
				return String.Format(format, apt.PercentComplete);
			}
			return String.Empty;
		}
		protected internal virtual void OffsetViewInfoItemsHorizontally(ViewInfoItemCollection items, int offset) {
			if (offset == 0)
				return;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				ViewInfoItem item = items[i];
				Rectangle bounds = item.Bounds;
				bounds.X += offset;
				item.Bounds = bounds;
			}
		}
		protected internal virtual ViewInfoItemCollection CreateClockItems(AppointmentViewInfo aptViewInfo) {
			ViewInfoItemCollection clockItems;
			if (aptViewInfo.Options.ShowTimeAsClock)
				clockItems = CreateClockImageItems(aptViewInfo);
			else
				clockItems = CreateClockTextItems(aptViewInfo);
			return clockItems;
		}
		protected internal virtual ViewInfoItemCollection CreateClockImageItems(AppointmentViewInfo aptViewInfo) {
			ViewInfoItemCollection items = new ViewInfoItemCollection();
			Appointment appointment = aptViewInfo.Appointment;
			if (aptViewInfo.Options.ShowStartTime) {
				DateTime start = ViewInfo.TimeZoneHelper.ToClientTime(appointment.Start);
				items.Add(CreateClockImageItem(start));
			}
			if (aptViewInfo.Options.ShowEndTime) {
				DateTime end = ViewInfo.TimeZoneHelper.ToClientTime(appointment.End);
				items.Add(CreateClockImageItem(end));
			}
			return items;
		}
		protected internal override ViewInfoItemCollection CreateClockTextItems(IAppointmentViewInfo aptViewInfo) {
			ViewInfoItemCollection clockItems = base.CreateClockTextItems(aptViewInfo);
			AppearanceObject appearance = ((AppointmentViewInfo)aptViewInfo).Appearance;
			CalculateTextItemsAppearance(clockItems, appearance, WordWrap.NoWrap);
			return clockItems;
		}
		protected internal virtual Image GetClockImage(DateTime time) {
			int imageIndex = CalculateClockImageIndex(time);
			Image image = ImageCollection.GetImageListImage(AppointmentImages, imageIndex);
			if (image == null)
				image = Painter.DefaultAppointmentImages.Images[imageIndex];
			return image;
		}
		protected internal virtual AppointmentViewInfoClockImageItem CreateClockImageItem(DateTime time) {
			Image image = GetClockImage(time);
			AppointmentViewInfoClockImageItem result = new AppointmentViewInfoClockImageItem(image);
			result.ArrowsColor = CalculateClockArrowsColor(time);
			result.Time = time;
			return result;
		}
		protected internal virtual Rectangle LayoutClockItemsShortAppointment(AppointmentViewInfo aptViewInfo, ViewInfoItemCollection clocks, Rectangle availableBounds, GraphicsCache cache) {
			aptViewInfo.Items.AddRange(clocks);
			return LayoutItemsAlignLeft(cache, availableBounds, clocks);
		}
		protected internal virtual Rectangle LayoutClockItemsLongAppointment(AppointmentViewInfo aptViewInfo, ViewInfoItemCollection clockItems, Rectangle availableBounds, GraphicsCache cache) {
			int itemIndex = 0;
			Rectangle bounds = availableBounds;
			if (aptViewInfo.Options.ShowStartTime) {
				bounds = ViewInfoItemsLayoutHelper.LayoutItemAtLeft(cache, clockItems[itemIndex], bounds, ContentHorizontalGap);
				itemIndex++;
			}
			if (aptViewInfo.Options.ShowEndTime)
				bounds = ViewInfoItemsLayoutHelper.LayoutItemAtRight(cache, clockItems[itemIndex], bounds, ContentHorizontalGap);
			aptViewInfo.Items.AddRange(clockItems);
			return bounds;
		}
		protected internal virtual Rectangle RemoveLastGap(Rectangle freeBounds, ViewInfoItemCollection viewInfoItems) {
			int count = viewInfoItems.Count;
			if (count != 0) {
				int delta = freeBounds.Left - viewInfoItems[count - 1].Bounds.Right;
				freeBounds.X -= delta;
				freeBounds.Width += delta;
			}
			return freeBounds;
		}
		protected internal override void SetStatusItemBorders(AppointmentViewInfoStatusItem item, bool hasStartBorder, bool hasEndBorder) {
			AppointmentStatusViewInfo freeStatusViewInfo = item.BackgroundViewInfo;
			AppointmentStatusViewInfo statusViewInfo = item.ForegroundViewInfo;
			freeStatusViewInfo.HasLeftBorder = false;
			freeStatusViewInfo.HasTopBorder = false;
			freeStatusViewInfo.HasRightBorder = false;
			freeStatusViewInfo.HasBottomBorder = true;
			statusViewInfo.HasTopBorder = false;
			statusViewInfo.HasBottomBorder = true;
			statusViewInfo.HasLeftBorder = hasStartBorder;
			statusViewInfo.HasRightBorder = hasEndBorder;
		}
		protected internal override Rectangle CalculateStatusViewInfoBounds(AppointmentViewInfoStatusItem statusItem) {
			Rectangle itemBounds = statusItem.Bounds;
			Rectangle statusViewInfoBounds;
			if (statusItem.ForegroundViewInfo.Interval.Duration == TimeSpan.Zero) {
				statusViewInfoBounds = itemBounds;
				statusViewInfoBounds.Width = Math.Min(itemBounds.Width, itemBounds.Height);
			} else
				statusViewInfoBounds = AppointmentTimeScaleHelper.ScaleRectangleHorizontally(itemBounds, statusItem.BackgroundViewInfo.Interval, statusItem.ForegroundViewInfo.Interval);
			return statusViewInfoBounds;
		}
		protected internal override void OffsetViewInfoItems(GraphicsCache cache, AppointmentViewInfo aptViewInfo) {
			OffsetContentItems(aptViewInfo, aptViewInfo.Items);
			OffsetStatusItems(aptViewInfo, aptViewInfo.StatusItems);
		}
		protected internal virtual void OffsetContentItems(AppointmentViewInfo aptViewInfo, ViewInfoItemCollection items) {
			int count = items.Count;
			for (int i = 0; i < count; i++)
				OffsetContentItemVertically(aptViewInfo, items[i]);
		}
		protected internal virtual void OffsetContentItemVertically(AppointmentViewInfo aptViewInfo, ViewInfoItem item) {
			Rectangle itemBounds = item.Bounds;
			itemBounds.Height = Math.Min(itemBounds.Height, aptViewInfo.InnerBounds.Height);
			itemBounds.Y = aptViewInfo.InnerBounds.Y + (aptViewInfo.InnerBounds.Height - itemBounds.Height) / 2;
			item.Bounds = itemBounds;
		}
		protected internal virtual void OffsetStatusItems(AppointmentViewInfo aptViewInfo, ViewInfoItemCollection items) {
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				AppointmentViewInfoStatusItem statusItem = (AppointmentViewInfoStatusItem)items[i];
				OffsetStatusItemVertically(aptViewInfo, statusItem);
			}
		}
		protected internal virtual void OffsetStatusItemVertically(AppointmentViewInfo aptViewInfo, AppointmentViewInfoStatusItem statusItem) {
			Rectangle freeStatusBounds = statusItem.BackgroundViewInfo.Bounds;
			Rectangle statusBounds = statusItem.ForegroundViewInfo.Bounds;
			freeStatusBounds.Y = aptViewInfo.TopBorderBounds.Bottom;
			statusBounds.Y = aptViewInfo.TopBorderBounds.Bottom;
			statusItem.BackgroundViewInfo.Bounds = freeStatusBounds;
			statusItem.ForegroundViewInfo.Bounds = statusBounds;
		}
		protected internal override string CalculateStatusItemSkinElemenName() {
			return SchedulerSkins.SkinAppointmentStatusTopMask;
		}
		protected internal virtual bool ShouldShowContinueTextItems(AppointmentContinueArrowDisplayType type) {
			return ContentCalculatorHelper.ShouldShowContinueTextItems(type);
		}
		protected internal virtual string GetStartContinueItemText(IAppointmentViewInfo aptViewInfo) {
			return ContentCalculatorHelper.GetStartContinueItemText(aptViewInfo);
		}
		protected internal virtual string GetEndContinueItemText(IAppointmentViewInfo aptViewInfo) {
			return ContentCalculatorHelper.GetEndContinueItemText(aptViewInfo);
		}
	}
	public class VerticalAppointmentContentLayoutCalculator : AppointmentContentLayoutCalculator {
		public VerticalAppointmentContentLayoutCalculator(ISupportAppointments viewInfo, AppointmentPainter painter)
			: base(viewInfo, painter) {
		}
		protected internal new VerticalAppointmentContentCalculatorHelper ContentCalculatorHelper { get { return (VerticalAppointmentContentCalculatorHelper)base.ContentCalculatorHelper; } }
		protected internal override void OffsetViewInfoItems(GraphicsCache cache, AppointmentViewInfo aptViewInfo) {
		}
		protected internal override IViewInfoTextItem CreateTextItemInstance() {
			return new ViewInfoHorizontalTextItem();
		}
		protected internal override AppointmentContentCalculatorHelper CreateContentLayoutCalculatorHelper() {
			return new VerticalAppointmentContentCalculatorHelper(ViewInfo, StatusDisplayType, FormatProvider);
		}
		protected internal override void CalculatePreliminaryContentLayoutCore(GraphicsCache cache, AppointmentViewInfo aptViewInfo) {
			aptViewInfo.Items.Clear();
			ViewInfoItemCollection imageItems = CreateAppointmentViewInfoImageItems(aptViewInfo);
			ViewInfoItemCollection textItems = CreateAppointmentViewInfoTextItems(aptViewInfo);
			aptViewInfo.DisplayTextItem = (ViewInfoTextItem)GetTitleTextItem(textItems);
			LayoutViewInfoItems(cache, aptViewInfo, imageItems, textItems);
		}
		protected internal virtual void LayoutViewInfoItems(GraphicsCache cache, AppointmentViewInfo aptViewInfo, ViewInfoItemCollection imageItems, ViewInfoItemCollection textItems) {
			Rectangle availableBounds = LayoutViewInfoImageItems(cache, aptViewInfo, imageItems, aptViewInfo.InnerBounds);
			AddHorizontalSeparator(aptViewInfo, textItems);
			LayoutViewInfoTextItems(cache, aptViewInfo, textItems, availableBounds);
		}
		protected internal virtual void AddHorizontalSeparator(AppointmentViewInfo aptViewInfo, ViewInfoItemCollection textItems) {
			IViewInfoTextItem descriptionTextItem = GetDescriptionTextItem(textItems);
			if (!String.IsNullOrEmpty(descriptionTextItem.Text)) {
				ViewInfoHorizontalLineItem horizLineItem = CreateHorizontalSeparator(aptViewInfo.Appearance);
				textItems.Insert(1, horizLineItem);
			}
		}
		protected internal virtual ViewInfoItemCollection CreateAppointmentViewInfoTextItems(AppointmentViewInfo aptViewInfo) {
			ViewInfoItemCollection aptTextItems = base.CreateAppointmentViewInfoTextItems(aptViewInfo);
			ViewInfoItemCollection clockTextItems = CreateClockTextItems(aptViewInfo);
			AddClockTextToAppointmentTitle(clockTextItems, aptTextItems);
			WordWrap wrap = CalculateWordWrap(aptViewInfo.Appearance);
			CalculateTextItemsAppearance(aptTextItems, aptViewInfo.Appearance, wrap);
			return aptTextItems;
		}
		protected internal virtual WordWrap CalculateWordWrap(AppearanceObject aptViewInfoAppearance) {
			if (aptViewInfoAppearance.TextOptions.WordWrap == WordWrap.Default)
				return WordWrap.Wrap;
			else
				return aptViewInfoAppearance.TextOptions.WordWrap;
		}
		protected internal virtual void AddClockTextToAppointmentTitle(ViewInfoItemCollection clockTextItems, ViewInfoItemCollection aptTextItems) {
			string clockText = MergeClockItemsText(clockTextItems);
			IViewInfoTextItem titleTextItem = GetTitleTextItem(aptTextItems);
			if (!String.IsNullOrEmpty(clockText))
				titleTextItem.Text = String.Concat(clockText, " ", titleTextItem.Text);
		}
		protected internal virtual string MergeClockItemsText(ViewInfoItemCollection clockTextItems) {
			string result = String.Empty;
			int count = clockTextItems.Count;
			for (int i = 0; i < count; i++) {
				IViewInfoTextItem clockItem = (IViewInfoTextItem)clockTextItems[i];
				result = String.Concat(result, clockItem.Text);
			}
			return result;
		}
		protected internal virtual Rectangle LayoutViewInfoImageItems(GraphicsCache cache, AppointmentViewInfo aptViewInfo, ViewInfoItemCollection imageItems, Rectangle availableBounds) {
			availableBounds = LayoutImageItemsByVertical(cache, availableBounds, imageItems);
			aptViewInfo.Items.AddRange(imageItems);
			return availableBounds;
		}
		protected internal virtual Rectangle LayoutViewInfoTextItems(GraphicsCache cache, AppointmentViewInfo aptViewInfo, ViewInfoItemCollection textItems, Rectangle availableBounds) {
			availableBounds = LayoutItemsAlignTop(cache, availableBounds, textItems);
			aptViewInfo.Items.AddRange(textItems);
			return availableBounds;
		}
		protected internal virtual Rectangle LayoutImageItemsByVertical(GraphicsCache cache, Rectangle availableBounds, ViewInfoItemCollection items) {
			Rectangle currentAvailableBounds = availableBounds;
			int count = items.Count;
			int columnWidth = 0;
			for (int i = 0; i < count; i++) {
				ViewInfoItem item = items[i];
				Size itemSize = item.CalcContentSize(cache, currentAvailableBounds);
				if (itemSize.Height > currentAvailableBounds.Height) {
					availableBounds = RectUtils.CutFromLeft(availableBounds, columnWidth + ContentHorizontalGap);
					currentAvailableBounds = availableBounds;
					columnWidth = 0;
				}
				currentAvailableBounds = ViewInfoItemsLayoutHelper.LayoutItemAtTop(cache, item, currentAvailableBounds, ContentHorizontalGap);
				columnWidth = Math.Max(columnWidth, item.Bounds.Width);
			}
			return RectUtils.CutFromLeft(availableBounds, columnWidth + ContentHorizontalGap);
		}
		protected internal virtual ViewInfoHorizontalLineItem CreateHorizontalSeparator(AppearanceObject aptAppearance) {
			ViewInfoHorizontalLineItem horizSeparator = new ViewInfoHorizontalLineItem();
			AppearanceHelper.Combine(horizSeparator.Appearance, new AppearanceObject[] { aptAppearance });
			Color color = aptAppearance.BorderColor;
			horizSeparator.Appearance.BackColor = color;
			horizSeparator.Appearance.BackColor2 = color;
			return horizSeparator;
		}
		protected internal override void SetStatusItemBorders(AppointmentViewInfoStatusItem item, bool hasStartBorder, bool hasEndBorder) {
			AppointmentStatusViewInfo freeStatusViewInfo = item.BackgroundViewInfo;
			AppointmentStatusViewInfo statusViewInfo = item.ForegroundViewInfo;
			freeStatusViewInfo.HasLeftBorder = false;
			freeStatusViewInfo.HasTopBorder = false;
			freeStatusViewInfo.HasRightBorder = true;
			freeStatusViewInfo.HasBottomBorder = false;
			statusViewInfo.HasTopBorder = hasStartBorder;
			statusViewInfo.HasBottomBorder = hasEndBorder;
			statusViewInfo.HasLeftBorder = false;
			statusViewInfo.HasRightBorder = true;
		}
		protected internal override string CalculateStatusItemSkinElemenName() {
			return SchedulerSkins.SkinAppointmentStatusLeftMask;
		}
		protected internal virtual void CalculateStatusItemBounds(AppointmentViewInfo aptViewInfo, AppointmentViewInfoStatusItem statusItem, Rectangle statusItemAvailableBounds) {
			AppointmentStatusViewInfo statusViewInfo = statusItem.ForegroundViewInfo;
			statusViewInfo.Bounds = AppointmentTimeScaleHelper.ScaleRectangleVertically(statusItemAvailableBounds, aptViewInfo.Interval, statusViewInfo.Interval);
			statusItem.Bounds = statusItemAvailableBounds;
			statusItem.MaskImageIndex = aptViewInfo.CalcAppointmenStatusMaskImageIndex();
			Painter.CacheStatusImage(statusItem);
		}
		protected internal override Rectangle CalculateStatusViewInfoBounds(AppointmentViewInfoStatusItem statusItem) {
			Rectangle itemBounds = statusItem.Bounds;
			Rectangle statusViewInfoBounds;
			if (statusItem.ForegroundViewInfo.Interval.Duration == TimeSpan.Zero) {
				statusViewInfoBounds = itemBounds;
				statusViewInfoBounds.Height = Math.Min(itemBounds.Width, itemBounds.Height);
			} else
				statusViewInfoBounds = AppointmentTimeScaleHelper.ScaleRectangleVertically(itemBounds, statusItem.BackgroundViewInfo.Interval, statusItem.ForegroundViewInfo.Interval);
			return statusViewInfoBounds;
		}
	}
	public class DayViewTimeCellAppointmentContentLayoutCalculator : VerticalAppointmentContentLayoutCalculator {
		public DayViewTimeCellAppointmentContentLayoutCalculator(ISupportAppointments viewInfo, AppointmentPainter painter)
			: base(viewInfo, painter) {
		}
		protected internal override ViewInfoItemAlignment StatusAlignment { get { return ViewInfoItemAlignment.Left; } }
	}
	public class DayViewAllDayAppointmentContentLayoutCalculator : HorizontalAppointmentContentLayoutCalculator {
		public DayViewAllDayAppointmentContentLayoutCalculator(ISupportAppointments viewInfo, AppointmentPainter painter)
			: base(viewInfo, painter) {
		}
		protected internal override AppointmentStatusDisplayType StatusDisplayType {
			get {
				return ((DayViewAppointmentDisplayOptions)AppointmentDisplayOptions).AllDayAppointmentsStatusDisplayType;
			}
		}
	}
	public class WorkWeekViewTimeCellAppointmentContentLayoutCalculator : DayViewTimeCellAppointmentContentLayoutCalculator {
		public WorkWeekViewTimeCellAppointmentContentLayoutCalculator(DayViewInfo viewInfo, AppointmentPainter painter)
			: base(viewInfo, painter) {
		}
	}
	public class WorkWeekViewAllDayAppointmentContentLayoutCalculator : DayViewAllDayAppointmentContentLayoutCalculator {
		public WorkWeekViewAllDayAppointmentContentLayoutCalculator(DayViewInfo viewInfo, AppointmentPainter painter)
			: base(viewInfo, painter) {
		}
	}
	public class FullWeekViewTimeCellAppointmentContentLayoutCalculator : DayViewTimeCellAppointmentContentLayoutCalculator {
		public FullWeekViewTimeCellAppointmentContentLayoutCalculator(FullWeekViewInfo viewInfo, AppointmentPainter painter)
			: base(viewInfo, painter) {
		}
	}
	public class WeekViewAppointmentContentLayoutCalculator : HorizontalAppointmentContentLayoutCalculator {
		public WeekViewAppointmentContentLayoutCalculator(ISupportAppointments viewInfo, AppointmentPainter painter)
			: base(viewInfo, painter) {
		}
	}
	public class MonthViewAppointmentContentLayoutCalculator : WeekViewAppointmentContentLayoutCalculator {
		public MonthViewAppointmentContentLayoutCalculator(ISupportAppointments viewInfo, AppointmentPainter painter)
			: base(viewInfo, painter) {
		}
	}
	public abstract class TimelineViewAppointmentContentLayoutCalculatorBase : HorizontalAppointmentContentLayoutCalculator {
		protected TimelineViewAppointmentContentLayoutCalculatorBase(ISupportAppointments viewInfo, AppointmentPainter painter)
			: base(viewInfo, painter) {
		}
		protected internal override AppointmentContentCalculatorHelper CreateContentLayoutCalculatorHelper() {
			return new TimelineAppointmentContentLayoutCalculatorHelper(ViewInfo, StatusDisplayType, FormatProvider);
		}
	}
	public class TimelineViewAppointmentContentLayoutCalculator : TimelineViewAppointmentContentLayoutCalculatorBase {
		public TimelineViewAppointmentContentLayoutCalculator(SchedulerViewInfoBase viewInfo, AppointmentPainter painter)
			: base(viewInfo, painter) {
		}
		protected internal new SchedulerViewInfoBase ViewInfo { get { return (SchedulerViewInfoBase)base.ViewInfo; } }
		protected internal virtual void RestrictViewInfoInnerBounds(AppointmentViewInfo aptViewInfo) {
			int viewInfoRightBound = ViewInfo.Bounds.Right;
			Rectangle aptInnerBounds = aptViewInfo.InnerBounds;
			if (aptInnerBounds.Right > viewInfoRightBound)
				aptViewInfo.InnerBounds = RectUtils.GetLeftSideRect(aptInnerBounds, viewInfoRightBound - aptInnerBounds.Left - Painter.ContentHorizontalGap);
		}
		protected internal override void CalculateViewInfoBounds(AppointmentViewInfo aptViewInfo) {
			base.CalculateViewInfoBounds(aptViewInfo);
			RestrictViewInfoInnerBounds(aptViewInfo);
		}
	}
	public class GanttViewAppointmentContentLayoutCalculator : TimelineViewAppointmentContentLayoutCalculator {
		public GanttViewAppointmentContentLayoutCalculator(SchedulerViewInfoBase viewInfo, AppointmentPainter painter)
			: base(viewInfo, painter) {
		}
		protected internal new GanttViewInfo ViewInfo { get { return (GanttViewInfo)base.ViewInfo; } }
		protected internal override void CalculateViewInfoBounds(AppointmentViewInfo aptViewInfo) {
			base.CalculateViewInfoBounds(aptViewInfo);
			if (!ShouldShowPercentCompleteBar(aptViewInfo.Options.PercentCompleteDisplayType))
				return;
			int progressValue = aptViewInfo.Appointment.PercentComplete;
			Rectangle newBounds = aptViewInfo.Bounds;
			int progressWidth = (int)Math.Round(progressValue / (float)AppointmentProcessValues.Max * newBounds.Width);
			aptViewInfo.PercentCompleteBounds = new Rectangle(newBounds.X, newBounds.Y, progressWidth, newBounds.Height);
		}
		protected internal override void CalculateViewInfoProperties(AppointmentViewInfo aptViewInfo) {
			base.CalculateViewInfoProperties(aptViewInfo);
			if (ShouldShowPercentCompleteBar(aptViewInfo.Options.PercentCompleteDisplayType)) {
				Color aptColor = aptViewInfo.Appearance.GetBackColor();
				aptViewInfo.PercentCompleteColor = GanttViewAppointmentPainter.GetBackColorForPercentComplete(aptColor);
			}
		}
		protected internal virtual bool ShouldShowPercentCompleteBar(PercentCompleteDisplayType type) {
			return type == PercentCompleteDisplayType.Both || type == PercentCompleteDisplayType.BarProgress;
		}
	}
}
