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
using DevExpress.Utils;
using DevExpress.Xpf.Editors.RangeControl.Internal;
using System.Windows;
using DevExpress.Xpf.Core;
using System.Windows.Controls;
using DevExpress.XtraScheduler;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Native;
using System.Windows.Media;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Core.Native;
using System.ComponentModel;
using System.Collections.Specialized;
namespace DevExpress.Xpf.Scheduler.Native {
	public abstract class ItemLayout<T> {
		readonly IItemGenerator generator;
		readonly List<IItemGenerator> generators;
		protected ItemLayout(IItemGenerator generator) {
			Guard.ArgumentNotNull(generator, "panel");
			this.generator = generator;
			this.generators = new List<IItemGenerator>();
			RegisterGenerator(generator);
		}
		protected IItemGenerator Generator { get { return generator; } }
		public Rect Layout(Rect bounds, T info) {
			BeginGeneration();
			Rect result = LayoutCore(bounds, info);
			EndGeneration();
			return result;
		}
		void BeginGeneration() {
			int count = this.generators.Count;
			for (int i = 0; i < count; i++) {
				this.generators[i].Begin();
			}
		}
		void EndGeneration() {
			int count = this.generators.Count;
			for (int i = 0; i < count; i++) {
				this.generators[i].End();
			}
		}
		protected void RegisterGenerator(IItemGenerator generator) {
			this.generators.Add(generator);
		}
		protected abstract Rect LayoutCore(Rect bounds, T info);
	}
	public abstract class RulerItemInfoLayout : ItemLayout<RulerInfo> {
		const double InnerItemPadding = 10;
		protected RulerItemInfoLayout(IItemGenerator generator)
			: base(generator) {
			ChildrentBounds = new List<Rect>();
		}
		public List<Rect> ChildrentBounds { get; private set; }
		double MinItemWidth { get; set; }
		public string OptimalFormat { get; set; }
		public ITimeScaleDateTimeFormatter Formatter { get; set; }
		void PrecalculateChildrenBounds(Rect bounds, List<RulerItemInfo> items) {
			double minWidth = double.MaxValue;
			ChildrentBounds.Clear();
			IMapping pointMapping = new PointMapping(new Point(), bounds);
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				RulerItemInfo itemInfo = items[i];
				DoubleInterval normalInterval = itemInfo.NormalInterval;
				Point leftTop = pointMapping.GetSnappedPoint(normalInterval.Start, 0);
				Point middleBottom = pointMapping.GetSnappedPoint(normalInterval.End, 1);
				Rect rect = new Rect(leftTop, middleBottom);
				ChildrentBounds.Add(rect);
				if (rect.Width < minWidth)
					minWidth = rect.Width;
			}
			MinItemWidth = minWidth;
		}
		protected override Rect LayoutCore(Rect bounds, RulerInfo info) {
			List<RulerItemInfo> items = info.Items;
			PrecalculateChildrenBounds(bounds, items);
			CalculateOptimalFormat(info.Ruler.Scale, items);
			int count = items.Count;
			double maxHeight = 0;
			for (int i = 0; i < count; i++) {
				RulerItemInfo itemInfo = items[i];
				Control item = Generator.GenerateNext();
				ApplyProperties(item, itemInfo);
				Rect arrangeRect = ChildrentBounds[i];
				if (item.VerticalAlignment != VerticalAlignment.Stretch) {
					item.Measure(new Size(arrangeRect.Width, double.PositiveInfinity));
					arrangeRect.Height = item.DesiredSize.Height;
				}
				item.Arrange(arrangeRect);
				ChildrentBounds[i] = arrangeRect;
				if (maxHeight < arrangeRect.Height)
					maxHeight = arrangeRect.Height;
			}
			Formatter = null;
			OptimalFormat = String.Empty;
			return new Rect(0, 0, 0, maxHeight);
		}
		void CalculateOptimalFormat(TimeScale scale, List<RulerItemInfo> items) {
			Formatter = TimeScaleDateTimeFormatterFactory.Default.CreateFormatter(scale);
			if (Formatter.SupportsAutoFormats) {
				Control testItem = Generator.GenerateNext();
				SchedulerTextMeasurer measurer = new SchedulerTextMeasurer(testItem);
				OptimalFormat = CalculateOptimalFormatCore(Formatter, items, measurer, MinItemWidth - InnerItemPadding);
				Generator.LoseNext();
			}
		}
		string CalculateOptimalFormatCore(ITimeScaleDateTimeFormatter formatter, List<RulerItemInfo> list, SchedulerTextMeasurer measurer, double minWidth) {
			int count = list.Count;
			int optimalFormatIndx = 0;
			StringCollection formats = formatter.GetDateTimeAutoFormats();
			int formatCount = formats.Count;
			if (formatCount <= 0)
				return String.Empty;
			for (int i = 0; i < count; i++) {
				RulerItemInfo item = list[i];
				DateTime start = item.Interval.Start;
				DateTime end = item.Interval.End;
				for (; optimalFormatIndx < formatCount; optimalFormatIndx++) {
					string currentFormat = formats[optimalFormatIndx];
					string s = formatter.Format(currentFormat, start, end);
					double width = measurer.MeasureWidth(s);
					if (width < minWidth) 
						break;
				}
			}
			return formats[Math.Min(optimalFormatIndx, formatCount - 1)];
		}
		protected abstract void ApplyProperties(Control item, RulerItemInfo itemInfo);
	}
	public class RulerHeaderLevelLayout : RulerItemInfoLayout {
		public RulerHeaderLevelLayout(IItemGenerator generator)
			: base(generator) {
		}
		protected override void ApplyProperties(Control item, RulerItemInfo itemInfo) {
			RulerHeaderControl headerControl = item as RulerHeaderControl;
			TimeInterval interval = itemInfo.Interval;
			if (Formatter != null && !String.IsNullOrEmpty(OptimalFormat))
				headerControl.Caption = Formatter.Format(OptimalFormat, interval.Start, itemInfo.Interval.End);
			else
				headerControl.Caption = itemInfo.Scale.FormatCaption(interval.Start, itemInfo.Interval.End);
		}
	}
	public class RulerHeaderGroupLayout : ItemLayout<List<RulerInfo>> {
		List<RulerHeaderLevelLayout> headerLevels = new List<RulerHeaderLevelLayout>();
		public RulerHeaderGroupLayout(IItemGenerator generator)
			: base(generator) {
			LevelBoundsList = new List<Rect>();
		}
		public List<Rect> LevelBoundsList { get; private set; }
		protected override Rect LayoutCore(Rect bounds, List<RulerInfo> levelInfos) {
			LevelBoundsList.Clear();
			double usedHeight = 0;
			int levelCount = levelInfos.Count;
			Rect availableBounds = bounds;
			for (int i = 0; i < levelCount; i++) {
				RulerHeaderLevelLayout layout = new RulerHeaderLevelLayout(Generator);
				Rect levelBounds = layout.Layout(availableBounds, levelInfos[i]);
				double startHeight = usedHeight;
				usedHeight += levelBounds.Height;
				LevelBoundsList.Add(new Rect(0, startHeight, 0, usedHeight));
				RectHelper.Offset(ref availableBounds, 0, levelBounds.Height);
			}
			return new Rect(0, 0, 0, usedHeight);
		}
	}
	public class CellContentLayout : RulerHeaderLevelLayout {
		public CellContentLayout(IItemGenerator generator)
			: base(generator) {
		}
	}
	public abstract class ThumbnailListLayoutBase : ItemLayout<List<DataItemThumbnailList>> {
		CellContentLayout cellLayout;
		SchedulerStorage storage;
		protected ThumbnailListLayoutBase(IItemGenerator generator, CellContentLayout cells, SchedulerStorage storage)
			: base(generator) {
			this.cellLayout = cells;
			this.storage = storage;
		}
		public CellContentLayout CellLayout { get { return cellLayout; } }
		protected SchedulerStorage Storage { get { return storage; } }
		protected override Rect LayoutCore(Rect bounds, List<DataItemThumbnailList> info) {
			int count = info.Count;
			for (int i = 0; i < count; i++) {
				Rect cellRect = CellLayout.ChildrentBounds[i];
				DataItemThumbnailList thumbnailList = info[i];
				LayoutThumbnailList(cellRect, thumbnailList);
			}
			return new Rect();
		}
		protected abstract void LayoutThumbnailList(Rect cellRect, DataItemThumbnailList thumbnailList);
	}
	public class ThumbnailListLayout : ThumbnailListLayoutBase {
		IItemGenerator groupItemGenerator;
		RangeControlDataDisplayType dataDisplayType;
		public ThumbnailListLayout(IItemGenerator generator, IItemGenerator groupItemGenerator, CellContentLayout cells, SchedulerStorage storage, RangeControlDataDisplayType dataDisplayType)
			: base(generator, cells, storage) {
			this.groupItemGenerator = groupItemGenerator;
			RegisterGenerator(groupItemGenerator);
			this.dataDisplayType = dataDisplayType;
		}
		public IItemGenerator GroupItemGenerator { get { return groupItemGenerator; } }
		RangeControlDataDisplayType DataDisplayType { get { return dataDisplayType; } }
		protected override void LayoutThumbnailList(Rect cellRect, DataItemThumbnailList thumbnailList) {
			bool needThumnailGroupItem = true;
			if (DataDisplayType == RangeControlDataDisplayType.Auto || DataDisplayType == RangeControlDataDisplayType.Thumbnail)
				needThumnailGroupItem = !LayoutThumnailItems(cellRect, thumbnailList);
			if (needThumnailGroupItem)
				LayoutThumnailGroupItem(cellRect, thumbnailList);
		}
		void LayoutThumnailGroupItem(Rect cellRect, DataItemThumbnailList thumbnailList) {
			if (thumbnailList.Thumbnails.Count <= 0)
				return;
			ThumbnailGroupControl control = GroupItemGenerator.GenerateNext() as ThumbnailGroupControl;
			if (control == null)
				return;
			AssignProperties(control, thumbnailList);
			double minSide = Math.Min(cellRect.Width, cellRect.Height);
			Rect thumbnailGroupItemBounds = new Rect(0, 0, minSide, minSide);
			RectHelper.AlignVertically(ref thumbnailGroupItemBounds, cellRect, VerticalAlignment.Center);
			RectHelper.AlignHorizontally(ref thumbnailGroupItemBounds, cellRect, HorizontalAlignment.Center);
			Thickness padding = control.Padding;
			RectHelper.Deflate(ref thumbnailGroupItemBounds, padding);
			control.Measure(thumbnailGroupItemBounds.Size);
			control.Arrange(thumbnailGroupItemBounds);
		}
		bool LayoutThumnailItems(Rect cellRect, DataItemThumbnailList thumbnailList) {
			double x = cellRect.X;
			double width = cellRect.Width;
			double y = cellRect.Y;
			double cellHeight = cellRect.Height;
			bool isAllItemFitToCell = true;
			bool isAllItemFitToCellCalculated = DataDisplayType == RangeControlDataDisplayType.Thumbnail;
			int thumbnailListCount = thumbnailList.Thumbnails.Count;
			for (int j = 0; j < thumbnailListCount; j++) {
				if (cellHeight < 8)
					break;
				Control control = Generator.GenerateNext();
				Rect thumbnailItemBounds = new Rect(x, y, width, cellHeight);
				Thickness padding = control.Padding;
				RectHelper.Deflate(ref thumbnailItemBounds, padding);
				AssignProperties(control, thumbnailList.Thumbnails[j]);
				control.Measure(thumbnailItemBounds.Size);
				control.Arrange(thumbnailItemBounds);
				if (!isAllItemFitToCellCalculated) {
					double fittedItemCount = cellHeight / (control.ActualHeight + padding.Bottom + padding.Top);
					if (fittedItemCount < thumbnailListCount) {
						isAllItemFitToCell = false;
						control.Visibility = Visibility.Hidden;
						break;
					}
					isAllItemFitToCellCalculated = true;
				}
				cellHeight -= control.ActualHeight + padding.Bottom + padding.Top;
			}
			return isAllItemFitToCell;
		}
		void AssignProperties(Control control, IDataItemThumbnail thumbnail) {
			ThumbnailControl thumbnailControl = control as ThumbnailControl;
			if (thumbnailControl == null)
				return;
			if (thumbnailControl.DataContext == null)
				thumbnailControl.DataContext = new VisualThumbnailData();
			VisualThumbnailData visualData = thumbnailControl.DataContext as VisualThumbnailData;
			if (visualData == null)
				return;
			visualData.CopyFrom(Storage, thumbnail);
		}
		void AssignProperties(ThumbnailGroupControl control, DataItemThumbnailList thumbnailList) {
			if (control.DataContext == null)
				control.DataContext = new VisualThumbnailGroupData();
			VisualThumbnailGroupData visualData = control.DataContext as VisualThumbnailGroupData;
			if (visualData == null)
				return;
			visualData.CopyFrom(thumbnailList);
		}
	}
	public class SchedulerTextMeasurer {
		Control testSurface;
		public SchedulerTextMeasurer(Control testSurface) {
			this.testSurface = testSurface;
		}
		public double MeasureWidth(string text) {
			Typeface typeface = new Typeface(this.testSurface.FontFamily, this.testSurface.FontStyle, this.testSurface.FontWeight, this.testSurface.FontStretch);
			return new FormattedText(text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, this.testSurface.FontSize, Brushes.Black).Width;
		}
	}
}
namespace DevExpress.Xpf.Scheduler {
	public class RulerHeaderControl : Control {
		public RulerHeaderControl() {
			DefaultStyleKey = typeof(RulerHeaderControl);
		}
		#region Caption
		public string Caption {
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
		public static readonly DependencyProperty CaptionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RulerHeaderControl, string>("Caption", String.Empty, (d, e) => d.OnCaptionChanged(e.OldValue, e.NewValue), null);
		void OnCaptionChanged(string oldValue, string newValue) {
		}
		#endregion
		#region Interval
		public TimeInterval Interval {
			get { return (TimeInterval)GetValue(IntervalProperty); }
			set { SetValue(IntervalProperty, value); }
		}
		public static readonly DependencyProperty IntervalProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RulerHeaderControl, TimeInterval>("Interval", TimeInterval.Empty, (d, e) => d.OnIntervalChanged(e.OldValue, e.NewValue), null);
		void OnIntervalChanged(TimeInterval oldValue, TimeInterval newValue) {
		}
		#endregion
	}
	public class RulerCellControl : RulerHeaderControl {
		public RulerCellControl() {
			DefaultStyleKey = typeof(RulerCellControl);
		}
	}
	public class ThumbnailGroupControl : Control {
		public ThumbnailGroupControl() {
			DefaultStyleKey = typeof(ThumbnailGroupControl);
		}
	}
	public class VisualThumbnailGroupData : DependencyObject {
		#region Count
		public int Count {
			get { return (int)GetValue(CountProperty); }
			set { SetValue(CountProperty, value); }
		}
		public static readonly DependencyProperty CountProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualThumbnailGroupData, int>("Count", 0, (d, e) => d.OnCountChanged(e.OldValue, e.NewValue), null);
		void OnCountChanged(int oldValue, int newValue) {
		}
		#endregion
		internal void CopyFrom(DataItemThumbnailList thumbnailGroup) {
			Count = thumbnailGroup.Thumbnails.Count;
		}
	}
	public class ThumbnailControl : Control {
		public ThumbnailControl() {
			DefaultStyleKey = typeof(ThumbnailControl);
		}
	}
	public class VisualThumbnailData : INotifyPropertyChanged {
		#region LabelColor
		Color lastLabelColor = Colors.White;
		public Color LabelColor {
			get { return lastLabelColor; }
			set {
				if (this.lastLabelColor == value)
					return;
				this.lastLabelColor = value;
				RaisePropertyChanged("LabelColor");
			}
		}
		#endregion
		#region LabelBrush
		Brush lastLabelBrush;
		public Brush LabelBrush {
			get { return lastLabelBrush; }
			set {
				if (lastLabelBrush == value)
					return;
				lastLabelBrush = value;
				RaisePropertyChanged("LabelBrush");
			}
		}
		#endregion
		#region Appointment
		Appointment lastAppointment;
		public Appointment Appointment {
			get { return lastAppointment; }
			set {
				if (lastAppointment == value)
					return;
				OnAppointmentChanged(lastAppointment, value);
				lastAppointment = value;
				RaisePropertyChanged("Appointment");
			}
		}
		void OnAppointmentChanged(Appointment oldValue, Appointment newValue) {
			if (oldValue != null)
				((IInternalAppointment)oldValue).StateChanged -= OnAppointmentStateChanged;
			if (newValue != null)
				((IInternalAppointment)newValue).StateChanged += OnAppointmentStateChanged;
			this.lastAppointment = newValue;
		}
		void OnAppointmentStateChanged(object sender, PersistentObjectStateChangedEventArgs e) {
			RaisePropertyChanged("Appointment");
		}
		#endregion
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		void RaisePropertyChanged(string name) {
			if (PropertyChanged == null)
				return;
			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
		#endregion
		internal void CopyFrom(SchedulerStorage storage, IDataItemThumbnail thumbnail) {
			Color labelColor = storage.GetLabelColor(thumbnail.Appointment.LabelKey);
			LabelColor = labelColor;
			LabelBrush = BrushHelper.CreateSolidColorBrush(labelColor);
			Appointment = thumbnail.Appointment;
		}
	}
}
