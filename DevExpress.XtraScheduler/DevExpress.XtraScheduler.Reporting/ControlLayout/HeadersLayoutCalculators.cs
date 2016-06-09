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
using DevExpress.XtraScheduler.Drawing;
using System.Drawing;
using System.Collections.Specialized;
using DevExpress.XtraScheduler.Native;
using System.Globalization;
using System.Collections;
using DevExpress.XtraScheduler.Services;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraScheduler.Services.Implementation;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Reporting.Native {
	public abstract class HeadersBaseLayoutCalculator : ViewControlBaseLayoutCalculator {
		protected HeadersBaseLayoutCalculator(HeadersControlBase headers)
			: base(headers) {
		}
		protected internal new SchedulerHeaderPainter Painter { get { return (SchedulerHeaderPainter)base.Painter; } }
		protected internal new HeadersControlBase Control { get { return (HeadersControlBase)base.Control; } }
		public void CalculateLayout(ControlLayoutInfo info) {
			PrepareLayout();
			AnchorCollection anchors = GetHeadersAnchors(info);
			CalculateLayout(anchors, info.ControlPrintBounds);
		}
		protected internal virtual  void CalculateLayout(AnchorCollection anchors, Rectangle controlPrintBounds) {
			int count = anchors.Count;
			for (int i = 0; i < count; i++)				
				CalculateLayoutCore(anchors.GetAnchorInfo(i), controlPrintBounds);			
		}
		protected internal virtual void PrepareLayout() {
			Control.Headers.Clear();
		}		   
		protected internal virtual void CalculateLayoutCore(AnchorInfo anchorInfo, Rectangle controlPrintBounds) {
			if (anchorInfo.InnerAnchors.Count == 0)
				return;
			SchedulerHeaderCollection headers = CreateHeaders(anchorInfo.Anchor);
			AnchorBase actualAnchor = CalculateActualAnchor(anchorInfo.Anchor, controlPrintBounds);
			Rectangle fakeBounds = CalculateFakeHeadersBounds(actualAnchor);			
			InitializeHeadersBorders(headers, anchorInfo.IsFirstAnchor);
			CalculateInitialHeadersBounds(headers, actualAnchor.InnerAnchors, fakeBounds );
			CalculateHeadersLayout(headers, actualAnchor);
			TransformHeaders(actualAnchor.Bounds, headers);		
			Control.Headers.AddRange(headers);
		}
		protected internal virtual SchedulerHeaderCollection CreateCornerHeaders(AnchorBase headerAnchor) {
			return new SchedulerHeaderCollection();
		}
		protected internal virtual void InitializeHeadersBorders(SchedulerHeaderCollection headers, bool isFirstAnchor) {
			SetLeftAndRightBorders(headers, isFirstAnchor);
		}
		protected internal virtual void AssignHeadersHeight(SchedulerHeaderCollection headers, int height) {
			int count = headers.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHeader header = headers[i];
				Rectangle bounds = header.Bounds;
				bounds.Height = height;
				header.Bounds = bounds;
				bounds = header.AnchorBounds;
				bounds.Height = height;
				header.AnchorBounds = bounds;
			}
		}
		protected internal virtual void SetLeftAndRightBorders(SchedulerHeaderCollection headers, bool isFirstAnchor) {
			int count = headers.Count;
			for (int i = 0; i < count; i++) {
				bool firstHeader = isFirstAnchor && i == 0;
				headers[i].HasLeftBorder = !firstHeader;
			}
		}   
		protected internal virtual SchedulerHeaderPreliminaryLayoutResultCollection CalculateHeadersPreliminaryLayout(SchedulerHeaderCollection headers) {
			SchedulerHeaderPreliminaryLayoutResultCollection result = new SchedulerHeaderPreliminaryLayoutResultCollection();
			int count = headers.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHeader header = headers[i];
				try {
					SchedulerHeaderPreliminaryLayoutResult preliminaryResult = (SchedulerHeaderPreliminaryLayoutResult)header.CalculateHeaderPreliminaryLayout(Cache, Painter);
					result.Add(preliminaryResult);
				}
				finally {
				}
			}
			return result;
		}
		protected internal virtual void CalcFinalLayout(SchedulerHeaderCollection headers, SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults) {
			int count = headers.Count;
			for (int i = 0; i < count; i++) {
				headers[i].CalcLayout(Cache, preliminaryResults[i]);
			}
		}
		protected internal virtual int CalculateHeadersHeight(SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults) {
			int result = 0;
			int count = preliminaryResults.Count;
			for (int i = 0; i < count; i++)
				result = Math.Max(result, preliminaryResults[i].Size.Height);
			return result;
		}	   
		protected internal virtual void CalculateInitialHeadersBounds(SchedulerHeaderCollection headers, AnchorCollection anchors, Rectangle headersBounds) {
			int count = anchors.Count;
			XtraSchedulerDebug.Assert(headers.Count == anchors.Count);
			for (int i = 0; i < count; i++) {
				Rectangle anchor = anchors[i].Bounds;
				headers[i].Bounds = Rectangle.FromLTRB(anchor.Left, headersBounds.Top, anchor.Right, headersBounds.Bottom);
			}
		}		 
		protected internal virtual void CalculateHeadersLayout(SchedulerHeaderCollection headers, AnchorBase headersAnchor) {
			SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults = CalculateHeadersPreliminaryLayout(headers);
			CalculateHeadersCaptions(headers, preliminaryResults);
			AssignHeadersHeight(headers, headersAnchor.Bounds.Height);
			CalcFinalLayout(headers, preliminaryResults);
		}
		protected internal virtual Rectangle CalculateFakeHeadersBounds(AnchorBase headerActualAnchor) {
			return headerActualAnchor.Bounds;
		}
		protected internal virtual void TransformHeaders(Rectangle anchorBounds, SchedulerHeaderCollection headers) {
		}
		protected internal virtual AnchorBase CalculateActualAnchor(AnchorBase anchor, Rectangle controlPrintBounds) {
			AnchorBase result = anchor.Clone();
			result.Bounds = CalculateHorizontalHeaderActualAnchorBounds(anchor.Bounds, controlPrintBounds);
			return result;
		}
		protected internal virtual Rectangle CalculateHorizontalHeaderActualAnchorBounds(Rectangle anchorBounds, Rectangle controlPrintBounds) {
			return Rectangle.FromLTRB(anchorBounds.Left, controlPrintBounds.Top, anchorBounds.Right, controlPrintBounds.Bottom);
		}
		protected internal virtual Rectangle CalculateVerticalHeaderActualAnchorBounds(Rectangle anchorBounds, Rectangle controlPrintBounds) {
			return Rectangle.FromLTRB(controlPrintBounds.Left, anchorBounds.Top, controlPrintBounds.Right, anchorBounds.Bottom);
		}
		protected internal abstract SchedulerHeaderCollection CreateHeaders(AnchorBase anchor);
		protected internal abstract void CalculateHeadersCaptions(SchedulerHeaderCollection headers, SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults);
		protected internal abstract AnchorCollection GetHeadersAnchors(ControlLayoutInfo info);
	}
	public abstract class ResourceHeadersBaseLayoutCalculator : HeadersBaseLayoutCalculator {
		protected ResourceHeadersBaseLayoutCalculator(HeadersControlBase headersControl)
			: base(headersControl) {
		}
		public new HeadersControlBase Control { get { return (HeadersControlBase)base.Control; } }
		protected internal override SchedulerHeaderCollection CreateHeaders(AnchorBase anchor) {
			TimeInterval interval = anchor.Interval;
			SchedulerHeaderCollection result = new SchedulerHeaderCollection();
			AnchorCollection headersAnchors = anchor.InnerAnchors;
			int count = headersAnchors.Count;
			for (int i = 0; i < count; i++) {
				Resource resource = headersAnchors[i].Resource;
				SchedulerHeader header = CreateHeaderInstance(Control.Appearance, GetHeaderOptions());
				InitializeResourceHeader(header, resource, interval);
				result.Add(header);
			}
			return result;
		}		
		protected internal virtual void InitializeResourceHeader(SchedulerHeader header, Resource resource, TimeInterval interval) {
			header.Resource = resource;
			InitializeResourceHeaderCore(header, resource.Caption, resource.GetImage(), interval);
		}
		protected internal virtual void InitializeResourceHeaderCore(SchedulerHeader header, String caption, Image image, TimeInterval interval) {
			header.Interval = interval;
			header.Caption = caption;
			header.ToolTipText = caption;			
			header.Image = image;
			header.HasTopBorder = false;
			header.HasBottomBorder = false;
		}
		protected internal override void CalculateHeadersCaptions(SchedulerHeaderCollection headers, SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults) {
		}
		protected internal abstract ResourceHeader CreateHeaderInstance(BaseHeaderAppearance appearance, SchedulerResourceHeaderOptions options);
		protected internal abstract SchedulerResourceHeaderOptions GetHeaderOptions();
	}
	public class DateHeadersLayoutCalculator : HeadersBaseLayoutCalculator {
		HeaderCaptionFormatProviderBase captionFormatProvider;
		public DateHeadersLayoutCalculator(HorizontalDateHeaders dateHeaders, HeaderCaptionFormatProviderBase captionFormatProvider)
			: base(dateHeaders) {			
			this.captionFormatProvider = captionFormatProvider;
		}
		public HeaderCaptionFormatProviderBase CaptionFormatProvider { get { return captionFormatProvider; } }
		protected internal new HorizontalDateHeaders Control { get { return (HorizontalDateHeaders)base.Control; } }
		protected internal override AnchorCollection GetHeadersAnchors(ControlLayoutInfo info) {
			XtraSchedulerDebug.Assert(info.VerticalAnchors.Count == 0);
			return info.HorizontalAnchors;
		}
		protected internal override void CalculateHeadersCaptions(SchedulerHeaderCollection headers, SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults) {
			StringCollection formats = DateTimeFormatHelper.GenerateFormatsWithoutYear();
			int count = headers.Count;
			for (int i = 0; i < count; i++) {
				DayHeader header = (DayHeader)headers[i];
				header.Caption = CalculateHeaderCaption(header, formats, preliminaryResults[i].TextSize.Width);				
			}
		}
		protected internal virtual string CalculateHeaderCaption(DayHeader header, StringCollection formats, int headerWidth) {
			DateTime date = header.Interval.Start;
			if (CaptionFormatProvider != null) {
				string format = CaptionFormatProvider.GetDayColumnHeaderCaption(header);
				if (!String.IsNullOrEmpty(format)) {
					return String.Format(CultureInfo.CurrentCulture, format, date);
				}
			}
			StringOptimalMeasurement measurement = DateTimeFormatHelper.DateToStringOptimalMeasurement(Cache.Graphics, header.CaptionAppearance.Font, date, headerWidth, formats);
			return measurement.Text;
		}	   
		protected internal override SchedulerHeaderCollection CreateHeaders(AnchorBase anchor) {
			SchedulerHeaderCollection result = new SchedulerHeaderCollection();
			AnchorCollection headersAnchors = anchor.InnerAnchors;
			int count = headersAnchors.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHeader header = new DayHeader(Control.Appearance);
				TimeInterval interval = headersAnchors[i].Interval;
				header.Interval = interval;
				header.Alternate = false; 
				header.HasTopBorder = false;
				header.HasBottomBorder = false;
				header.Resource = anchor.Resource;
				result.Add(header);
			}
			return result;
		}
	}
	public class VerticalResourceHeadersLayoutCalculator : ResourceHeadersBaseLayoutCalculator {
		public VerticalResourceHeadersLayoutCalculator(VerticalResourceHeaders headersControl)
			: base(headersControl) {
		}
		public new VerticalResourceHeaders Control { get { return (VerticalResourceHeaders)base.Control; } }
		protected internal override void CalculateLayout(AnchorCollection anchors, Rectangle controlPrintBounds) {
			base.CalculateLayout(anchors, controlPrintBounds);
			CalculateCornerHeadersLayout(anchors, controlPrintBounds);
		}
		protected internal virtual void CalculateCornerHeadersLayout(AnchorCollection anchors, Rectangle controlPrintBounds) {
			int count = anchors.Count;
			if (count == 0)
				return;
			CalculateCornerHeadersLayoutCore(anchors[0], controlPrintBounds);
			CalculateCornerHeadersLayoutCore(anchors[count - 1], controlPrintBounds);
		}
		protected internal virtual void CalculateCornerHeadersLayoutCore(AnchorBase headerAnchor, Rectangle controlPrintBounds) {
			AnchorBase actualAnchor = CalculateActualAnchor(headerAnchor, controlPrintBounds);
			SchedulerHeaderCollection cornerHeaders = CreateCornerHeaders(actualAnchor);
			CalculateHeadersLayout(cornerHeaders, actualAnchor);
			TransformHeaders(actualAnchor.Bounds, cornerHeaders);
			Control.Headers.AddRange(cornerHeaders);
		}
		protected internal override SchedulerHeaderCollection CreateCornerHeaders(AnchorBase headerAnchor) {
			SchedulerHeaderCollection result = new SchedulerHeaderCollection();
			int topCornerHeight = CalculateTopCornerHeight(headerAnchor);
			int bottomCornerHeight = CalculateBottomCornerHeight(headerAnchor);
			if (topCornerHeight > 0)
				result.Add(CreateTopCorner(headerAnchor, topCornerHeight));
			if (bottomCornerHeight > 0)
				result.Add(CreateBottomCorner(headerAnchor, bottomCornerHeight));
			return result;
		}
		protected internal virtual int CalculateTopCornerHeight(AnchorBase headerAnchor) {
			if (headerAnchor.InnerAnchors.Count == 0)
				return 0;
			return headerAnchor.InnerAnchors[0].Bounds.Left - headerAnchor.Bounds.Left;
		}
		protected internal virtual int CalculateBottomCornerHeight(AnchorBase headerAnchor) {
			int count = headerAnchor.InnerAnchors.Count;
			if (count == 0)
				return 0;
			return headerAnchor.Bounds.Right - headerAnchor.InnerAnchors[count - 1].Bounds.Right;
		}
		protected internal virtual SchedulerHeader CreateTopCorner(AnchorBase headerAnchor, int height) {
			Rectangle anchorBounds = headerAnchor.Bounds;			
			int position = anchorBounds.Left;
			SchedulerHeader header = CreateCornerHeaderCore(height, position);
			header.HasRightBorder = true;
			return header;
		}
		protected internal virtual SchedulerHeader CreateBottomCorner(AnchorBase headerAnchor, int height) {
			Rectangle anchorBounds = headerAnchor.Bounds;
			int position = anchorBounds.Right - height;
			SchedulerHeader header = CreateCornerHeaderCore(height, position);
			header.HasLeftBorder = true;
			return header;
		}
		protected internal virtual SchedulerHeader CreateCornerHeaderCore(int height, int position) {
			SchedulerHeader header = CreateHeaderInstance(Control.Appearance, new SchedulerResourceHeaderOptions());
			InitializeResourceHeaderCore(header, String.Empty, null, TimeInterval.Empty);			
			header.Bounds = new Rectangle(position, header.Bounds.Top, height, 0);
			return header;
		}
		protected internal override void TransformHeaders(Rectangle anchorBounds, SchedulerHeaderCollection headers) {
			TransformMatrix transform = PrepareTransformMatrix(0, 0);
			Transform(headers, transform);
		}
		protected internal override Rectangle CalculateFakeHeadersBounds(AnchorBase actualHeaderAnchor) {
			return new Rectangle(0, 0, actualHeaderAnchor.Bounds.Height, actualHeaderAnchor.Bounds.Width);
		}
		protected internal override AnchorBase CalculateActualAnchor(AnchorBase anchor, Rectangle controlPrintBounds) {
			AnchorBase rootAnchor = TransformAnchor(anchor, controlPrintBounds);
			rootAnchor.InnerAnchors.Clear();
			int count = anchor.InnerAnchors.Count;
			for (int i = 0; i < count; i++) {
				AnchorBase innerAnchor = TransformAnchor(anchor.InnerAnchors[i], controlPrintBounds);
				rootAnchor.InnerAnchors.Add(innerAnchor);
			}
			return rootAnchor;
		}
		protected internal virtual AnchorBase TransformAnchor(AnchorBase anchor, Rectangle controlPrintBounds) {
			AnchorBase result = anchor.Clone();
			Rectangle bounds = CalculateVerticalHeaderActualAnchorBounds(anchor.Bounds, controlPrintBounds);
			result.Bounds = new Rectangle(bounds.Y, bounds.X, bounds.Height, bounds.Width);
			return result;
		}
		protected internal virtual TransformMatrix PrepareTransformMatrix(int x, int y) {
			TransformMatrix transform = new TransformMatrix();
			transform = transform.RotateCW90();
			transform = transform.Scale(-1, 1);
			transform = transform.Translate(x, y);
			return transform;
		}
		protected internal virtual void Transform(SchedulerHeaderCollection headers, TransformMatrix transform) {
			int count = headers.Count;
			for (int i = 0; i < count; i++)
				Transform(headers[i], transform);
		}
		protected internal virtual void Transform(SchedulerHeader header, TransformMatrix transform) {
			header.Bounds = transform.Apply(header.Bounds);
			header.AnchorBounds = transform.Apply(header.AnchorBounds);
			header.ContentBounds = transform.Apply(header.ContentBounds);
			header.TextBounds = transform.Apply(header.TextBounds);
			header.ImageBounds = transform.Apply(header.ImageBounds);
			header.FullSeparatorBounds = transform.Apply(header.FullSeparatorBounds);
			header.SeparatorBounds = transform.Apply(header.SeparatorBounds);
			header.UnderlineBounds = transform.Apply(header.UnderlineBounds);
			header.LeftBorderBounds = transform.Apply(header.LeftBorderBounds);
			header.RightBorderBounds = transform.Apply(header.RightBorderBounds);
			header.TopBorderBounds = transform.Apply(header.TopBorderBounds);
			header.BottomBorderBounds = transform.Apply(header.BottomBorderBounds);
		}
		protected internal override ResourceHeader CreateHeaderInstance(BaseHeaderAppearance appearance, SchedulerResourceHeaderOptions options) {
			return new ReportVerticalResourceHeader(appearance, options);
		}
		protected internal override AnchorCollection GetHeadersAnchors(ControlLayoutInfo info) {
			XtraSchedulerDebug.Assert(info.HorizontalAnchors.Count == 0);
			return info.VerticalAnchors;
		}
		protected internal override SchedulerResourceHeaderOptions GetHeaderOptions() {
			return Control.Options;
		}
	}
	public class HorizontalResourceHeadersLayoutCalculator : ResourceHeadersBaseLayoutCalculator {
		public HorizontalResourceHeadersLayoutCalculator(HorizontalResourceHeaders resourceHeadersControl)
			: base(resourceHeadersControl) {
		}
		public new HorizontalResourceHeaders Control { get { return (HorizontalResourceHeaders)base.Control; } }
		protected internal override ResourceHeader CreateHeaderInstance(BaseHeaderAppearance appearance, SchedulerResourceHeaderOptions options) {
			return new HorizontalResourceHeader(appearance, options);
		}
		protected internal override AnchorCollection GetHeadersAnchors(ControlLayoutInfo info) {
			XtraSchedulerDebug.Assert(info.VerticalAnchors.Count == 0);
			return info.HorizontalAnchors;
		}
		protected internal override SchedulerResourceHeaderOptions GetHeaderOptions() {
			return Control.Options;
		}
	}
	#region ReportWeekDayHeadersLayoutCalculator
	public class DayOfWeekHeadersLayoutCalculator : HeadersBaseLayoutCalculator {
		WeekBasedViewHeaderCaptionCalculator headerCaptionCalculator;
		public DayOfWeekHeadersLayoutCalculator(DayOfWeekHeaders control)
			: base(control) {
			this.headerCaptionCalculator = CreateHeaderCaptionCalculator();
		}
		protected internal new DayOfWeekHeaders Control { get { return (DayOfWeekHeaders)base.Control; } }
		protected internal new ReportWeekView View { get { return (ReportWeekView)base.View; } }
		public WeekBasedViewHeaderCaptionCalculator HeaderCaptionCalculator { get { return headerCaptionCalculator; } }
		protected internal override SchedulerHeaderCollection CreateHeaders(AnchorBase anchor) {
			SchedulerHeaderCollection result = new SchedulerHeaderCollection();
			AnchorCollection headerAnchors = anchor.InnerAnchors;
			int count = headerAnchors.Count;
			for (int i = 0; i < count; i++) {
				DayOfWeek dayOfWeek = headerAnchors[i].Interval.Start.DayOfWeek;
				DayOfWeekHeader header = new DayOfWeekHeader(Control.Appearance, dayOfWeek);
				header.HasTopBorder = false;
				header.HasBottomBorder = false;
				header.Resource = anchor.Resource;				
				result.Add(header);
			}
			return result;
		}
		protected internal override AnchorCollection GetHeadersAnchors(ControlLayoutInfo info) {
			XtraSchedulerDebug.Assert(info.VerticalAnchors.Count == 0);
			return info.HorizontalAnchors;
		}	  
		protected internal override void CalculateHeadersCaptions(SchedulerHeaderCollection headers, SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults) {			
			CalculateFullHeadersCaptions(headers);
			if ((ShouldUseAbbreviatedCaptions(headers, preliminaryResults)))
				CalculateAbbreviatedHeadersCaptions(headers);
		}
		protected internal virtual WeekBasedViewHeaderCaptionCalculator CreateHeaderCaptionCalculator() {
			WeekBasedViewHeaderCaptionCalculator calculator = new WeekBasedViewOptimalHeaderCaptionCalculator();
			calculator.CompressWeekend = Control.ActualCompressWeekend;
			HeaderCaptionFormatProviderBase captionProvider = View.GetHeaderCaptionFormatProvider();
			if (captionProvider == null)
				return calculator;
			WeekBasedViewHeaderCaptionCalculator result = new WeekViewBasedFixedFormatHeaderCaptionCalculator(captionProvider, calculator);
			result.CompressWeekend = Control.ActualCompressWeekend;
			return result;
		}
		protected internal virtual void CalculateFullHeadersCaptions(SchedulerHeaderCollection headers) {
			int count = headers.Count;
			for (int i = 0; i < count; i++) {
				DayOfWeekHeader header = (DayOfWeekHeader)headers[i];
				HeaderCaptionCalculator.CalculateFullHeaderCaption(header);				
			}
		}
		protected internal virtual bool ShouldUseAbbreviatedCaptions(SchedulerHeaderCollection headers, SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults) {
			int totalWidth = 0;
			int totalSpaceWidth = 0;
			int count = headers.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHeader header = headers[i];
				int availableTextWidth = preliminaryResults[i].TextSize.Width;
				totalWidth += Math.Max(availableTextWidth, CalculateHeaderCaptionWidth(header));
				totalSpaceWidth += availableTextWidth;
			}
			return totalWidth > totalSpaceWidth;
		}
		protected internal virtual int CalculateHeaderCaptionWidth(SchedulerHeader header) {
			return (int)header.CaptionAppearance.CalcTextSize(Cache, header.Caption, Int32.MaxValue).Width;
		}
		protected internal virtual void CalculateAbbreviatedHeadersCaptions(SchedulerHeaderCollection headers) {
			int count = headers.Count;
			for (int i = 0; i < count; i++) {
				DayOfWeekHeader header = (DayOfWeekHeader)headers[i];
				HeaderCaptionCalculator.CalculateAbbreviatedHeaderCaption(header);				
			}
		}  
	}
	#endregion
	public class WeekViewBasedFixedFormatHeaderCaptionCalculator : WeekViewBasedFixedFormatHeaderCaptionCalculatorBase {
		public WeekViewBasedFixedFormatHeaderCaptionCalculator(HeaderCaptionFormatProviderBase captionProvider, WeekBasedViewHeaderCaptionCalculator fallbackCalculator)
			: base(captionProvider, fallbackCalculator) {
		}
		public override void CalculateHeaderToolTip(DayOfWeekHeader header) {
		}
		public override void CalculateShouldShowToolTip(DayOfWeekHeader header, Graphics gr, Size textSize) {
			header.ShouldShowToolTip = false;
		}
	}
	public class TimelineScaleHeadersLayoutCalculator : HeadersBaseLayoutCalculator {
		HeaderCaptionFormatProviderBase captionFormatProvider;
		protected HeaderCaptionFormatProviderBase CaptionFormatProvider { get { return captionFormatProvider; } }
		public TimelineScaleHeadersLayoutCalculator(TimelineScaleHeaders scaleHeaders)
			: base(scaleHeaders) {
			this.captionFormatProvider = View.GetHeaderCaptionFormatProvider();
		}
		protected internal new TimelineScaleHeaders Control { get { return (TimelineScaleHeaders)base.Control; } }
		protected internal override AnchorCollection GetHeadersAnchors(ControlLayoutInfo info) {
			XtraSchedulerDebug.Assert(info.VerticalAnchors.Count == 0);
			return info.HorizontalAnchors;
		}
		protected internal override void PrepareLayout() {
			base.PrepareLayout();
			Control.ScaleLevels.Clear();
		}
		protected internal override void CalculateLayoutCore(AnchorInfo anchorInfo, Rectangle controlPrintBounds) {
			AnchorCollection baseLevelAnchors = anchorInfo.InnerAnchors;
			if (anchorInfo.InnerAnchors.Count == 0)
				return;
			XtraSchedulerDebug.Assert(baseLevelAnchors.Count == Control.PrintIntervals.Count);			
			TimeScaleLevelCollection levels = CalculateScaleLevels(baseLevelAnchors);
			Rectangle headersBounds = CalculateHeadersBounds(anchorInfo.Anchor, controlPrintBounds);
			PerformScaleHeadersLayout(anchorInfo.Anchor.Resource, levels, headersBounds, baseLevelAnchors, anchorInfo.IsFirstAnchor);
		}
		protected internal virtual Rectangle CalculateHeadersBounds(ISchedulerObjectAnchor anchor, Rectangle controlPrintBounds) {
			return Rectangle.FromLTRB(anchor.Bounds.Left, controlPrintBounds.Top, anchor.Bounds.Right, controlPrintBounds.Bottom);
		}
		protected internal virtual TimeScaleLevelCollection CalculateScaleLevels(AnchorCollection anchors) {
			ReportTimeScaleLevelsCalculator calc = new ReportTimeScaleLevelsCalculator();
			XtraSchedulerDebug.Assert(anchors.Count != 0);
			return calc.Calculate(Control.Scales, anchors);
		}
		protected internal virtual void PerformScaleHeadersLayout(Resource resource, TimeScaleLevelCollection levels, Rectangle bounds, AnchorCollection baseLevelAnchors, bool isFirstAnchor) {
			SchedulerHeaderLevelCollection headerLevels = CreateHeaderLevels(resource, levels, bounds, baseLevelAnchors);
			InitializeHeadersBorders(headerLevels, isFirstAnchor);
			PerformHeaderLevelsLayout(headerLevels, bounds);
			Control.ScaleLevels.AddRange(headerLevels);
		}
		protected internal virtual void InitializeHeadersBorders(SchedulerHeaderLevelCollection headerLevels, bool isFirstAnchor) {
			int count = headerLevels.Count;
			for (int i = 0; i < count; i++)
				InitializeHeadersBorders(headerLevels[i].Headers, isFirstAnchor);
		}		
		protected internal virtual void PerformHeaderLevelsLayout(SchedulerHeaderLevelCollection levels, Rectangle headersBounds) {
			int top = headersBounds.Top;
			int count = levels.Count;
			int levelHeight = CalculateHeaderLevelHeight(headersBounds, levels.Count);
			for (int i = 0; i < count; i++) {
				SchedulerHeaderCollection levelHeaders = levels[i].Headers;
				ArrangeLevelHeadersVertically(levelHeaders, top);
				bool visible = (i == count - 1) ? Control.GetBaseTimeScale().Visible : true;
				PerformFixedHeightLevelHeadersLayout(levelHeaders, visible, levelHeight);
				top = levelHeaders[0].Bounds.Bottom;
				if (visible)
					top -= Painter.VerticalOverlap;
			}
		}
		protected internal virtual int CalculateHeaderLevelHeight(Rectangle headersBounds, int levelsCount) {			
			bool isBaseScaleVisible = Control.GetBaseTimeScale().Visible;
			int visibleLevelsCount = isBaseScaleVisible ? levelsCount : levelsCount - 1;
			return headersBounds.Height / visibleLevelsCount;
		}
		private SchedulerHeaderLevelCollection CreateHeaderLevels(Resource resource, TimeScaleLevelCollection levels, Rectangle bounds, AnchorCollection baseLevelAnchors) {
			SchedulerHeaderLevelCollection result = new SchedulerHeaderLevelCollection();
			if (levels.Count == 0)
				return result;
			SchedulerHeaderLevel baseLevel = CreateBaseLevel(resource, levels, bounds, baseLevelAnchors);
			result.Add(baseLevel);
			SchedulerHeaderLevelCollection upperLevels = CreateUpperLevels(resource, levels, baseLevel.Headers, bounds);
			result.InsertRange(0, upperLevels);
			return result;
		}
		private SchedulerHeaderLevel CreateBaseLevel(Resource resource, TimeScaleLevelCollection levels, Rectangle bounds, AnchorCollection anchors) {
			int count = levels.Count;
			TimeScaleLevel baseLevel = levels[count - 1];
			SchedulerHeaderCollection baseHeaders = CreateLevelHeaders(resource, baseLevel, true);
			SetBaseHeadersInitialBounds(baseHeaders, bounds,  anchors);
			SchedulerHeaderLevel result = new SchedulerHeaderLevel();
			result.Headers.AddRange(baseHeaders);
			return result;
		}
		protected internal virtual SchedulerHeaderLevelCollection CreateUpperLevels(Resource resource, TimeScaleLevelCollection levels, SchedulerHeaderCollection baseHeaders, Rectangle bounds) {
		   SchedulerHeaderLevelCollection result = new SchedulerHeaderLevelCollection();
			int count = levels.Count;
			for (int i = count - 2; i >= 0; i--) {
				TimeScaleLevel level = levels[i];
				SchedulerHeaderCollection upperHeaders = CreateLevelHeaders(resource, level, false);
				SetUpperHeadersInitialBounds(upperHeaders,  baseHeaders, bounds);
				SchedulerHeaderLevel headerLevel = new SchedulerHeaderLevel();
				headerLevel.Headers.AddRange(upperHeaders);
				result.Insert(0, headerLevel);
			}
			return result;
		}
		protected internal virtual void SetBaseHeadersInitialBounds(SchedulerHeaderCollection headers, Rectangle bounds, AnchorCollection baseLevelAnchors) {
			Rectangle[] rects = CalculateBaseHeadersInitialBounds(bounds, baseLevelAnchors);
			SetInitialHeadersBounds(headers, rects);
		}
		protected internal virtual void SetUpperHeadersInitialBounds(SchedulerHeaderCollection headers, SchedulerHeaderCollection baseHeaders, Rectangle bounds) {
			Rectangle[] rects = CalculateUpperHeadersInitialBounds(headers, baseHeaders, bounds.Top, bounds.Height);
			SetInitialHeadersBounds(headers, rects);
		}
		protected internal virtual void SetInitialHeadersBounds(SchedulerHeaderCollection headers, Rectangle[] headersBounds) {
			int count = headers.Count;
			XtraSchedulerDebug.Assert(count == headersBounds.Length);			
			for (int i = 0; i < count; i++) {
				Rectangle headerBounds = headersBounds[i];				
				headers[i].Bounds = headerBounds;
			}
		}		
		protected internal virtual void ArrangeLevelHeadersVertically(SchedulerHeaderCollection headers, int top) {
			for (int i = 0; i < headers.Count; i++) {
				Rectangle rect = headers[i].Bounds;
				rect.Y = top;
				headers[i].Bounds = rect;
			}
		}
		protected internal virtual void PerformFixedHeightLevelHeadersLayout(SchedulerHeaderCollection headers, bool visible, int height) {
			SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults = CalculateHeadersPreliminaryLayout(headers);
			CalculateInitialHeadersAnchorBounds(headers);
			int actualHeight = visible ? height : 0;
			PerformLevelHeadersLayoutCore(preliminaryResults, headers, actualHeight);
		}
		protected internal virtual void PerformAutoHeightLevelHeadersLayout(SchedulerHeaderCollection headers, bool visible) {
			SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults = CalculateHeadersPreliminaryLayout(headers);
			CalculateInitialHeadersAnchorBounds(headers);
			int actualHeight = visible ? CalculateHeadersHeight(preliminaryResults) : 0;
			PerformLevelHeadersLayoutCore(preliminaryResults, headers, actualHeight);
		}
		protected internal virtual void PerformLevelHeadersLayoutCore(SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults, SchedulerHeaderCollection headers, int height) {
			AssignHeadersHeight(headers, height);			
			CalcFinalLayout(headers, preliminaryResults);
		}
		protected internal virtual SchedulerHeaderCollection CreateLevelHeaders(Resource resource, TimeScaleLevel level, bool baseLevel) {
			SchedulerHeaderCollection headers = new SchedulerHeaderCollection();
			int count = level.Intervals.Count;
			for (int i = 0; i < count; i++) {
				TimeScaleHeader header = CreateTimeScaleHeader(level.Scale, level.Intervals[i], resource, baseLevel);
				headers.Add(header);
			}
			return headers;
		}
		protected internal virtual TimeScaleHeader CreateTimeScaleHeader(TimeScale scale, TimeInterval interval, Resource resource, bool baseScale) {
			TimeScaleHeader header = new TimeScaleHeader(Control.Appearance, scale);
			header.Interval = interval;
			header.Resource = resource;
			header.Caption = CalculateHeaderCaption(header);			
			header.HasTopBorder = false;
			header.HasBottomBorder = !baseScale;
			return header;
		}
		protected internal virtual string CalculateHeaderCaption(TimeScaleHeader header) {
			if (CaptionFormatProvider != null) {
				string format = CaptionFormatProvider.GetTimeScaleHeaderCaption(header);
				if (!String.IsNullOrEmpty(format))
					return String.Format(CultureInfo.CurrentCulture, format, header.Interval.Start, header.Interval.End);
			}
			return header.Scale.FormatCaption(header.Interval.Start, header.Interval.End);
		}
		protected internal virtual void CalculateInitialHeadersAnchorBounds(SchedulerHeaderCollection headers) {
			int count = headers.Count;			
			for (int i = 0; i < count; i++) {
				SchedulerHeader header = headers[i];
				Rectangle bounds = header.Bounds;
				int leftAnchor = bounds.Left;
				int rightAnchor = bounds.Right;	  
				header.AnchorBounds = Rectangle.FromLTRB(leftAnchor, bounds.Top, rightAnchor, bounds.Bottom);
			}
		}
		protected internal virtual Rectangle[] CalculateBaseHeadersInitialBounds(Rectangle bounds, AnchorCollection anchors) {
			int count = anchors.Count;
			Rectangle[] result = new Rectangle[count];
			for (int i = 0; i < count; i++) {
				Rectangle anchorBounds = anchors[i].Bounds;
				result[i] = Rectangle.FromLTRB(anchorBounds.Left, bounds.Top, anchorBounds.Right, bounds.Bottom);
			}
			return result;
		}
		protected internal virtual Rectangle[] CalculateUpperHeadersInitialBounds(SchedulerHeaderCollection upperHeaders, SchedulerHeaderCollection baseHeaders, int top, int height) {
			ArrayList results = new ArrayList();
			int left = baseHeaders[0].Bounds.Left;
			int startIndex = 0;
			for (int i = 0; i < upperHeaders.Count; i++) {
				startIndex = FindNextHeaderIndexByDate(baseHeaders, upperHeaders[i].Interval.End, startIndex);
				SchedulerHeader baseHeader = baseHeaders[Math.Max(0, startIndex - 1)];
				Rectangle baseBounds = baseHeader.Bounds;
				int right = baseBounds.Right;
				Rectangle rect = Rectangle.FromLTRB(left, top, right, top + height);
				left = right;
				results.Add(rect);
			}
			XtraSchedulerDebug.Assert(results.Count == upperHeaders.Count);
			return (Rectangle[])results.ToArray(typeof(Rectangle));
		}
		protected int FindNextHeaderIndexByDate(SchedulerHeaderCollection headers, DateTime date,  int from) {
			for (int i = from; i < headers.Count; i++) {
				if (headers[i].Interval.Start >= date) 
					return i;
			}
			return headers.Count;
		}
		protected internal virtual int EstimateHeaderRight(DateTime date, TimeInterval baseInterval, int baseLeft, int baseRight) {
			double ratio = Convert.ToDouble(date.Ticks - baseInterval.Start.Ticks) / Convert.ToDouble(baseInterval.End.Ticks - baseInterval.Start.Ticks);
			return baseLeft + Convert.ToInt32((baseRight - baseLeft) * ratio);
		}
		protected internal override void CalculateHeadersCaptions(SchedulerHeaderCollection headers, SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults) {
			StringCollection formats = DateTimeFormatHelper.GenerateFormatsWithoutYear();
			int count = headers.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHeader header = headers[i];
				DateTime date = header.Interval.Start;
				StringOptimalMeasurement measurement = DateTimeFormatHelper.DateToStringOptimalMeasurement(Cache.Graphics, header.CaptionAppearance.Font, date, preliminaryResults[i].TextSize.Width, formats);
				header.Caption = measurement.Text;
				header.ShouldShowToolTip = measurement.Abbreviated;
			}
		}
		protected internal override SchedulerHeaderCollection CreateHeaders(AnchorBase anchor) {
			return new SchedulerHeaderCollection();
		}		
	}
	#region ReportTimeScaleLevelsCalculator
	public class ReportTimeScaleLevelsCalculator {
		public ReportTimeScaleLevelsCalculator() {
		}
		public virtual TimeScaleLevelCollection Calculate(TimeScaleCollection scales, AnchorCollection anchors) {
			TimeScaleLevelCollection levels = new TimeScaleLevelCollection();
			int scaleCount = scales.Count;
			if (scaleCount == 0 || anchors.Count == 0)
				return levels;
			for (int i = 0; i < scaleCount; i++) {
				TimeScaleLevel level = new TimeScaleLevel(scales[i]);
				levels.Add(level);
			}
			int intervalCount = anchors.Count;
			for (int n = 0; n < intervalCount; n++) {
				TimeInterval currentInterval = anchors[n].Interval;
				for (int i = 0; i < scaleCount - 1; i++) 
					CreateLevelIntervals(levels[i], currentInterval);
				levels[scaleCount - 1].Intervals.Add(currentInterval);  
			}
			return levels;
		}
		protected void CreateLevelIntervals(TimeScaleLevel level, TimeInterval interval) {
			TimeScale scale = level.Scale;
			TimeIntervalCollection intervals = level.Intervals;
			DateTime start = scale.Floor(interval.Start);
			TimeInterval scaleInterval = CalculateScaleInterval(scale, start);
			for (; ;) {
				if (interval.IntersectsWith(scaleInterval)) {
					if (!intervals.Contains(scaleInterval))
						intervals.Add(scaleInterval);
					scaleInterval = CalculateScaleInterval(scale, scaleInterval.End);
				} else
					break;
				if (scaleInterval.Start >= interval.End)
					break;
			 }
		}
		protected TimeInterval CalculateScaleInterval(TimeScale scale, DateTime start) {
			return new TimeInterval(start, scale.GetNextDate(start));
		}
	}
	#endregion
	public class ReportVerticalResourceHeader : VerticalResourceHeader {
		public ReportVerticalResourceHeader(BaseHeaderAppearance appearance, SchedulerResourceHeaderOptions options) : base(appearance, options){
		}
		protected internal override Size AdjustTextSize(GraphicsCache cache, Size preliminarySize) {
			Size additionalSize = CalculateTextSizeCore(cache, new Size(Int32.MaxValue, Int32.MaxValue), "w");
			preliminarySize.Width += additionalSize.Width;
			preliminarySize.Height += additionalSize.Height;
			return preliminarySize;
		}
	}
}
