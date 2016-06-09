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
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Drawing {
	public class NavigationButtonPreliminaryLayoutResult {
		#region Fields
		NavigationButtonPainter painter;
		int contentLeftPadding;
		int contentRightPadding;
		int contentTopPadding;
		int contentBottomPadding;
		int contentVerticalGap;
		Size imageSize;
		#endregion
		#region Properties
		public NavigationButtonPainter Painter { get { return painter; } set { painter = value; } }
		public int ContentLeftPadding { get { return contentLeftPadding; } set { contentLeftPadding = value; } }
		public int ContentRightPadding { get { return contentRightPadding; } set { contentRightPadding = value; } }
		public int ContentTopPadding { get { return contentTopPadding; } set { contentTopPadding = value; } }
		public int ContentBottomPadding { get { return contentBottomPadding; } set { contentBottomPadding = value; } }
		public int ContentVerticalGap { get { return contentVerticalGap; } set { contentVerticalGap = value; } }
		public Size ImageSize { get { return imageSize; } set { imageSize = value; } }
		#endregion
	}
	#region NavigationButton (abstract class)
	public abstract class NavigationButton : ViewInfoItemContainer {
		#region Fields
		AppearanceObject appearance;
		ViewInfoImageItem imageItem;
		int imagesCount;
		ViewInfoVerticalTextItem displayTextItem;
		ViewInfoFreeSpaceItem freeSpaceItem;
		Rectangle contentBounds = Rectangle.Empty;
		SkinElementInfo cachedSkinElementInfo;
		SkinElementInfo cachedSkinArrowElementInfo;
		string defaultDisplayText;
		#endregion
		protected NavigationButton(string defaultDisplayText) {
			this.defaultDisplayText = defaultDisplayText;
			this.appearance = new AppearanceObject();
			HasLeftBorder = true;
			HasTopBorder = true;
			HasRightBorder = true;
			HasBottomBorder = true;
			this.imagesCount = 1;
		}
		#region Properties
		public AppearanceObject Appearance { get { return appearance; } }
		public bool Enabled { get { return Interval != null && !TimeInterval.Empty.Equals(Interval); } }
		public Rectangle ContentBounds { get { return contentBounds; } set { contentBounds = value; } }
		protected internal override bool AllowHotTrack { get { return true; } }
		public override SchedulerHitTest HitTestType { get { return SchedulerHitTest.NavigationButton; } }
		public bool HotTracked { get { return HotTrackedInternal; } }
		public ViewInfoImageItem ImageItem { get { return imageItem; } set { imageItem = value; } }
		public int ImagesCount { get { return imagesCount; } set { imagesCount = value; } }
		public ViewInfoVerticalTextItem DisplayTextItem { get { return displayTextItem; } set { displayTextItem = value; } }
		public ViewInfoFreeSpaceItem FreeSpaceItem { get { return freeSpaceItem; } set { freeSpaceItem = value; } }
		public string DisplayText { get { return displayTextItem != null ? displayTextItem.Text : string.Empty; } }
		internal SkinElementInfo CachedSkinElementInfo { get { return cachedSkinElementInfo; } set { cachedSkinElementInfo = value; } }
		internal SkinElementInfo CachedSkinArrowElementInfo { get { return cachedSkinArrowElementInfo; } set { cachedSkinArrowElementInfo = value; } }
		protected internal virtual string DefaultDisplayText { get { return defaultDisplayText; } }
		protected internal abstract Image DefaultImage { get; }
		protected internal abstract string SkinElementName { get; }
		protected internal abstract string SkinArrowElementName { get; }
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (appearance != null) {
						appearance.Dispose();
						appearance = null;
					}
					if (displayTextItem != null) {
						DisposeItemCore(displayTextItem);
						displayTextItem = null;
					}
					if (imageItem != null) {
						DisposeItemCore(imageItem);
						imageItem = null;
					}
					if (freeSpaceItem != null) {
						DisposeItemCore(freeSpaceItem);
						freeSpaceItem = null;
					}
					this.cachedSkinElementInfo = null;
					this.cachedSkinArrowElementInfo = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		public virtual NavigationButtonPreliminaryLayoutResult CalcPreliminaryLayout(GraphicsCache cache, NavigationButtonPainter painter) {
			NavigationButtonPreliminaryLayoutResult result = CreatePreliminaryLayoutResultObject();
			result.Painter = painter;
			CalculateIntermediateParameters(result);
			CalcPreliminaryLayoutCore(cache, result);
			return result;
		}
		protected internal virtual void CalcPreliminaryLayoutCore(GraphicsCache cache, NavigationButtonPreliminaryLayoutResult result) {
			if (FreeSpaceItem != null)
				FreeSpaceItem.Size = new Size(0, result.ImageSize.Height);
		}
		protected internal virtual NavigationButtonPreliminaryLayoutResult CreatePreliminaryLayoutResultObject() {
			return new NavigationButtonPreliminaryLayoutResult();
		}
		protected internal virtual void CalculateIntermediateParameters(NavigationButtonPreliminaryLayoutResult preliminaryResult) {
			NavigationButtonPainter painter = preliminaryResult.Painter;
			preliminaryResult.ContentLeftPadding = painter.GetContentLeftPadding(this);
			preliminaryResult.ContentRightPadding = painter.GetContentRightPadding(this);
			preliminaryResult.ContentTopPadding = painter.GetContentTopPadding(this);
			preliminaryResult.ContentBottomPadding = painter.GetContentBottomPadding(this);
			preliminaryResult.ImageSize = CalculateImageSize(); 
			preliminaryResult.ContentVerticalGap = painter.GetContentVerticalGap(this);
		}
		protected virtual Size CalculateImageSize() {
			if (ImageItem == null)
				return Size.Empty;
			Size contentSize = ImageItem.CalcContentSize(Cache, Rectangle.Empty);
			return new Size(contentSize.Width, contentSize.Height / ImagesCount);
		}
		public virtual void CalcLayout(GraphicsCache cache, NavigationButtonPreliminaryLayoutResult preliminaryResult) {
			if (cache == null)
				Exceptions.ThrowArgumentException("cache", cache);
			if (preliminaryResult == null)
				Exceptions.ThrowArgumentException("preliminaryResult", preliminaryResult);
			CalcBorderBounds(preliminaryResult.Painter);
			Rectangle innerBounds = CalcInnerBounds();
			this.contentBounds = CalcContentBounds(preliminaryResult, innerBounds);
			CalcItemsLayout(preliminaryResult, cache, ContentBounds);
		}
		protected internal virtual void ReleaseItems() {
			DisplayTextItem = null;
			ImageItem = null;
			FreeSpaceItem = null;
			DisposeItems();
			Items.Clear();
		}
		protected internal virtual Rectangle CalcInnerBounds() {
			return Rectangle.FromLTRB(LeftBorderBounds.Right, TopBorderBounds.Bottom, RightBorderBounds.Left, BottomBorderBounds.Top);
		}
		protected internal virtual Rectangle CalcContentBounds(NavigationButtonPreliminaryLayoutResult preliminaryResult, Rectangle availableBounds) {
			if (availableBounds == Rectangle.Empty)
				return availableBounds;
			return Rectangle.FromLTRB(availableBounds.Left + preliminaryResult.ContentLeftPadding,
				availableBounds.Top + preliminaryResult.ContentTopPadding,
				availableBounds.Right - preliminaryResult.ContentRightPadding,
				availableBounds.Bottom - preliminaryResult.ContentBottomPadding);
		}
		protected internal virtual Size CalcContentSize(GraphicsCache cache, NavigationButtonPreliminaryLayoutResult preliminaryLayout, Rectangle availableBounds) {
			Size result = Size.Empty;
			int count = Items.Count;
			for (int i = 0; i < count; i++) {
				Size size = Items[i].CalcContentSize(cache, availableBounds);
				result.Width = Math.Max(result.Width, size.Width);
				result.Height += size.Height;
				if (i < count - 1)
					result.Height += preliminaryLayout.ContentVerticalGap;
			}
			return result;
		}
		protected internal virtual void CalcItemsLayout(NavigationButtonPreliminaryLayoutResult preliminaryResult, GraphicsCache cache, Rectangle availableBounds) {
			Rectangle bounds = availableBounds;
			if (ImageItem != null)
				bounds = ViewInfoItemsLayoutHelper.LayoutItemAtTop(cache, ImageItem, bounds, preliminaryResult.ContentVerticalGap);
			if (FreeSpaceItem != null)
				bounds = ViewInfoItemsLayoutHelper.LayoutItemAtBottom(cache, FreeSpaceItem, bounds, preliminaryResult.ContentVerticalGap);
			if (DisplayTextItem != null)
				DisplayTextItem.Bounds = bounds;
			ViewInfoItemsLayoutHelper.CenterItemsHorizontally(availableBounds, Items);
		}
		protected internal virtual int CalculateImageIndex() {
			return Enabled ? (HotTrackedInternal ? 2 : 0) : 1;
		}
	}
	#endregion
	public class NavigationButtonPrev : NavigationButton {
		internal static System.Drawing.Image defaultImage;
		static NavigationButtonPrev() {
			defaultImage = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraScheduler.Images.navButtonPrev.png", System.Reflection.Assembly.GetExecutingAssembly());
			XtraSchedulerDebug.Assert(defaultImage != null);
		}
		public NavigationButtonPrev(string defaultDisplayText)
			: base(defaultDisplayText) {
		}
		protected internal override Image DefaultImage { get { return defaultImage; } }
		protected internal override string SkinElementName { get { return SchedulerSkins.SkinNavigationButtonPrev; } }
		protected internal override string SkinArrowElementName { get { return SchedulerSkins.SkinNavigationButtonPrevArrow; } }
	}
	public class NavigationButtonNext : NavigationButton {
		internal static System.Drawing.Image defaultImage;
		static NavigationButtonNext() {
			defaultImage = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraScheduler.Images.navButtonNext.png", System.Reflection.Assembly.GetExecutingAssembly());
			XtraSchedulerDebug.Assert(defaultImage != null);
		}
		public NavigationButtonNext(string defaultDisplayText)
			: base(defaultDisplayText) {
		}
		protected internal override Image DefaultImage { get { return defaultImage; } }
		protected internal override string SkinElementName { get { return SchedulerSkins.SkinNavigationButtonNext; } }
		protected internal override string SkinArrowElementName { get { return SchedulerSkins.SkinNavigationButtonNextArrow; } }
	}
	public class NavigationButtonCollection : List<NavigationButton> {
	}
	#region NavigationButtonsLayoutCalculator
	public class NavigationButtonsLayoutCalculator : SchedulerViewLayoutCalculatorBase {
		#region Fields
		PrevNextAppointmentIntervalPairCollection data;
		bool makeHorzOverlap;
		#endregion
		public NavigationButtonsLayoutCalculator(PrevNextAppointmentIntervalPairCollection data, GraphicsCache cache, SchedulerViewInfoBase viewInfo, NavigationButtonPainter painter, bool makeHorzOverlap)
			: base(cache, viewInfo, painter) {
			if (data == null)
				Exceptions.ThrowArgumentException("data", data);
			this.data = data;
			this.makeHorzOverlap = makeHorzOverlap;
		}
		#region Properties
		protected internal new NavigationButtonPainter Painter { get { return (NavigationButtonPainter)base.Painter; } }
		protected PrevNextAppointmentIntervalPairCollection Data { get { return data; } }
		protected internal bool MakeHorzOverlap { get { return makeHorzOverlap; } }
		#endregion
		public override void CalcLayout(Rectangle bounds) {
			CalcLayoutCore(Data, bounds);
		}
		protected internal virtual void CalcLayoutCore(PrevNextAppointmentIntervalPairCollection data, Rectangle bounds) {
			SchedulerViewCellContainerCollection containers = ViewInfo.CellContainers;
			if (containers.Count <= 0) return;
			int count = data.Count;
			for (int i = 0; i < count; i++) {
				NavigationButtonCollection buttons = LayoutResourceNavigationButtons(containers, data[i]);
				ViewInfo.AddNavigationButtons(buttons);
			}
		}
		protected internal virtual NavigationButtonCollection LayoutResourceNavigationButtons(SchedulerViewCellContainerCollection containers, PrevNextAppointmentIntervalPair data) {
			SchedulerViewCellContainerCollection resourceContainers = FilterContainersByResource(containers, data.Resource);
			if (resourceContainers.Count <= 0)
				return new NavigationButtonCollection();
			NavigationButtonCollection buttons = CreateButtons(data);
			XtraSchedulerDebug.Assert(buttons.Count == 2);
			Rectangle bounds = CalculateContainersBounds(resourceContainers);
			CalculateButtonsLayout(buttons[0], buttons[1], bounds);
			CacheButtonCollectionSkinElementInfos(buttons, data.Resource);
			return buttons;
		}
		protected internal virtual NavigationButtonCollection CreateButtons(PrevNextAppointmentIntervalPair data) {
			NavigationButtonCollection result = new NavigationButtonCollection();
			NavigationButton prevButton = CreateNavigationButtonPrev();
			InitButton(prevButton, data.Resource, data.PrevAppointmentInterval);
			result.Add(prevButton);
			NavigationButton nextButton = CreateNavigationButtonNext();
			InitButton(nextButton, data.Resource, data.NextAppointmentInterval);
			result.Add(nextButton);
			return result;
		}
		protected internal virtual NavigationButton CreateNavigationButtonPrev() {
			string caption = View.Control.OptionsView.NavigationButtons.PrevCaption;
			return new NavigationButtonPrev(caption);
		}
		protected internal virtual NavigationButton CreateNavigationButtonNext() {
			string caption = View.Control.OptionsView.NavigationButtons.NextCaption;
			return new NavigationButtonNext(caption);
		}
		protected internal virtual void CalculateButtonsLayout(NavigationButton prevButton, NavigationButton nextButton, Rectangle bounds) {
			NavigationButtonPreliminaryLayoutResult prevPreliminaryLayout = prevButton.CalcPreliminaryLayout(Cache, Painter);
			NavigationButtonPreliminaryLayoutResult nextPreliminaryLayout = nextButton.CalcPreliminaryLayout(Cache, Painter);
			Size contentSize = CalculateMaxContentSize(prevButton, prevPreliminaryLayout, nextButton, nextPreliminaryLayout, bounds);
			CalculateButtonFinalLayout(prevButton, prevPreliminaryLayout, bounds, contentSize, ContentAlignment.MiddleLeft);
			CalculateButtonFinalLayout(nextButton, nextPreliminaryLayout, bounds, contentSize, ContentAlignment.MiddleRight);
		}
		protected internal virtual void CalculateButtonFinalLayout(NavigationButton button, NavigationButtonPreliminaryLayoutResult preliminaryLayout, Rectangle bounds, Size contentSize, ContentAlignment align) {
			Size borderSize = Painter.CalculateTotalBorderSize(button);
			Size minSize = Painter.CalculateObjectMinSize(Cache, button, preliminaryLayout);
			Size totalPaddings = new Size(preliminaryLayout.ContentLeftPadding + preliminaryLayout.ContentRightPadding, preliminaryLayout.ContentTopPadding + preliminaryLayout.ContentBottomPadding);
			Size nonContentSize = MathUtils.Plus(borderSize, totalPaddings);
			Size totalSize = CalculateButtonTotalSize(contentSize, nonContentSize, minSize);
			if (totalSize.Height > bounds.Height) {
				RecreateButtomItems(button);
				contentSize = preliminaryLayout.ImageSize;
				totalSize = CalculateButtonTotalSize(contentSize, nonContentSize, minSize);
			}
			button.Bounds = RectUtils.AlignRectangle(new Rectangle(Point.Empty, totalSize), bounds, align);
			button.CalcLayout(Cache, preliminaryLayout);
		}
		protected internal virtual Size CalculateButtonTotalSize(Size contentSize, Size nonContentSize, Size minSize) {
			return MathUtils.Max(minSize, MathUtils.Plus(contentSize, nonContentSize));
		}
		protected internal virtual void RecreateButtomItems(NavigationButton button) {
			button.ReleaseItems();
			CreateButtonItems(button, string.Empty, Painter.GetButtonImage(button), Painter.GetButtonImageCount(button));
		}
		protected internal virtual Size CalculateMaxContentSize(NavigationButton prevButton, NavigationButtonPreliminaryLayoutResult prevPreliminaryLayout, NavigationButton nextButton, NavigationButtonPreliminaryLayoutResult nextPreliminaryLayout, Rectangle availableBounds) {
			Size prevSize = prevButton.CalcContentSize(Cache, prevPreliminaryLayout, availableBounds);
			Size nextSize = nextButton.CalcContentSize(Cache, nextPreliminaryLayout, availableBounds);
			return MathUtils.Max(prevSize, nextSize);
		}
		protected internal virtual Size CalculateMaxBorderSize(NavigationButton prevButton, NavigationButton nextButton) {
			return MathUtils.Max(Painter.CalculateTotalBorderSize(prevButton), Painter.CalculateTotalBorderSize(nextButton));
		}
		protected internal virtual Rectangle CalculateContainersBounds(SchedulerViewCellContainerCollection containers) {
			int count = containers.Count;
			if (count == 0)
				return Rectangle.Empty;
			int horzOverlap = 0;
			if (MakeHorzOverlap) {
				horzOverlap = Painter.HorizontalOverlap;
			}
			return Rectangle.FromLTRB(containers[0].Bounds.Left - horzOverlap, containers[0].Bounds.Top, containers[count - 1].Bounds.Right, containers[count - 1].Bounds.Bottom);
		}
		protected internal virtual SchedulerViewCellContainerCollection FilterContainersByResource(SchedulerViewCellContainerCollection containers, Resource resource) {
			SchedulerViewCellContainerCollection result = new SchedulerViewCellContainerCollection();
			int count = containers.Count;
			for (int i = 0; i < count; i++) {
				if (Comparer.Equals(resource, containers[i].Resource))
					result.Add(containers[i]);
			}
			return result;
		}
		protected internal virtual void InitButton(NavigationButton button, Resource resource, TimeInterval interval) {
			button.Interval = interval;
			button.Resource = resource;
			CreateButtonItems(button, button.DefaultDisplayText, Painter.GetButtonImage(button), Painter.GetButtonImageCount(button));
			CalculateButtonAppearances(button);
		}
		protected internal virtual void CalculateButtonAppearances(NavigationButton button) {
			AppearanceObject appearance = CalcActualButtonAppearance(button);
			button.Appearance.Combine(appearance);
			CalculateButtonItemsAppearance(button.Items, button.Appearance);
			CalculateDisplayTextForeColor(button);
		}
		protected internal virtual AppearanceObject CalcActualButtonAppearance(NavigationButton button) {
			return button.Enabled ? ViewInfo.PaintAppearance.NavigationButton : ViewInfo.PaintAppearance.NavigationButtonDisabled;
		}
		protected internal virtual void CalculateDisplayTextForeColor(NavigationButton button) {
			ViewInfoVerticalTextItem item = button.DisplayTextItem;
			if (item == null)
				return;
			item.Appearance.ForeColor = Painter.GetDisplayTextForeColor(button);
		}
		protected internal virtual void CreateButtonItems(NavigationButton button, string text, Image image, int imagesCount) {
			ViewInfoImageItem imageItem = new ViewInfoImageItem(image);
			button.Items.Add(imageItem);
			button.ImageItem = imageItem;
			button.ImagesCount = imagesCount;
			if (string.IsNullOrEmpty(text))
				return;
			ViewInfoVerticalTextItem textItem = new ViewInfoVerticalTextItem();
			textItem.Text = text;
			button.DisplayTextItem = textItem;
			button.Items.Add(textItem);
			ViewInfoFreeSpaceItem freeSpaceItem = new ViewInfoFreeSpaceItem();
			button.FreeSpaceItem = freeSpaceItem;
			button.Items.Add(freeSpaceItem);
		}
		protected internal virtual void CalculateButtonItemsAppearance(ViewInfoItemCollection items, AppearanceObject appearance) {
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				ViewInfoAppearanceItem item = items[i] as ViewInfoAppearanceItem;
				if (item != null) item.Appearance.Combine(appearance);
			}
		}
		protected internal virtual void CacheButtonCollectionSkinElementInfos(NavigationButtonCollection buttons, Resource resource) {
			if (!ShouldCacheSkinElementInfo())
				return;
			int count = buttons.Count;
			if (count <= 0)
				return;
			Color buttonsColor = CalculateButtonColor(resource);
			for (int i = 0; i < count; i++)
				CacheButtonSkinElementInfos(buttons[i], buttonsColor);
		}
		protected internal virtual bool ShouldCacheSkinElementInfo() {
			return Painter.CanCacheSkinElementInfo && View.FactoryHelper.CalcActualGroupType(View) != SchedulerGroupType.None;
		}
		protected internal virtual void CacheButtonSkinElementInfos(NavigationButton button, Color color) {
			button.CachedSkinElementInfo = Painter.PrepareCachedSkinElementInfo(button.SkinElementName, button.Bounds, color);
			if (button.ImageItem != null)
				button.CachedSkinArrowElementInfo = Painter.PrepareCachedSkinElementInfo(button.SkinArrowElementName, button.ImageItem.Bounds, color);
		}
		protected internal virtual Color CalculateButtonColor(Resource resource) {
			int visibleResourceIndex = CalculateVisibleResourceIndex(resource);
			int resourceColorIndex = CalculateResourceColorIndex(visibleResourceIndex);
			SchedulerColorSchema schema = View.GetResourceColorSchema(resource, resourceColorIndex); 
			return schema.Cell;
		}
		protected internal virtual int CalculateVisibleResourceIndex(Resource resource) {
			int coutOfInnerChildResourcesInCollapsedVisibleResources = 0;
			int count = ViewInfo.VisibleResources.Count;
			for (int i = 0; i < count; i++) {
				Resource curResource = ViewInfo.VisibleResources[i];
				IInternalResource curResourceInternal = (IInternalResource)curResource;
				if (!curResourceInternal.IsExpanded)
					coutOfInnerChildResourcesInCollapsedVisibleResources += curResourceInternal.AllChildResourcesCount;
				if (ResourceBase.MatchIds(resource, curResource))
					return i + coutOfInnerChildResourcesInCollapsedVisibleResources;
			}
			return 0;
		}
	}
	#endregion
	public class WinNavigationButtonCalculator : NavigationButtonCalculator {
		SchedulerControl control;
		SchedulerViewInfoBase viewInfo;
		public WinNavigationButtonCalculator(SchedulerControl control, SchedulerViewInfoBase viewInfo)
			: base(control.InnerControl, control.InnerControl.ActiveView) {
			this.control = control;
			this.viewInfo = viewInfo;
		}
		protected internal SchedulerControl Control { get { return control; } }
		protected internal SchedulerViewInfoBase ViewInfo { get { return viewInfo; } }
		protected internal override bool CanShowNavigationButtons() {
			if (base.CanShowNavigationButtons())
				return ViewInfo.CellContainers.Count > 0; 
			else
				return false;
		}
		protected internal override AppointmentBaseCollection GetVisibleAppointments() {
			return control.AppointmentChangeHelper.GetActualVisibleAppointments(View.FilteredAppointments);
		}
		protected internal override bool UseGroupByNoneCriteriaCalculator() {
			SchedulerViewBase view = ViewInfo.View;
			return view.FactoryHelper.CalcActualGroupType(view) == SchedulerGroupType.None;
		}
	}
}
