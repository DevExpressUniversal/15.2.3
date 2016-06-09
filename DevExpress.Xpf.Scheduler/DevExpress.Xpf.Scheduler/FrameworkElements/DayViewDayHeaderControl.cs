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
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.ComponentModel;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public abstract class AutoTextFormatControlBase : Decorator {
#if !SL
		static AutoTextFormatControlBase() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoTextFormatControlBase), new FrameworkPropertyMetadata(typeof(AutoTextFormatControlBase)));
		}
#endif
		#region MeasurerTextBlock (static property)
		[ThreadStatic]
		static TextMeasurer measurerTextBlock;
		static TextMeasurer MeasurerTextBlock {
			get {
				if (measurerTextBlock == null)
					measurerTextBlock = new TextMeasurer();
				return measurerTextBlock;
			}
		}
		#endregion
		Size[] textSizes;
		string[] captions;
		protected virtual Size[] TextSizes { get { return textSizes; } set { textSizes = value; } }
		protected virtual string[] Captions { get { return captions; } set { captions = value; } }
		protected virtual void InvalidateContent() {
			Captions = null;
			TextSizes = null;
			InvalidateMeasure();
		}
		protected override Size MeasureOverride(Size constraint) {			
			if (TextSizes == null) {
				TextSizes = CalculateSizes();
			}
			int index = GetBestFitIndex(constraint);
			if (index >= 0)
				return TextSizes[index];
			else
				return base.MeasureOverride(constraint);
		}
		protected virtual int GetBestFitIndex(Size size) {
			double formatWidth = double.NegativeInfinity;
			double minWidth = double.PositiveInfinity;
			double adjustedWidth = double.NegativeInfinity;
			int minFormatIndex = -1;
			int adjustedFormatIndex = -1;
			double boundsWidth = size.Width;
			int count = TextSizes.Length;
			for (int i = 0; i < count; i++) {
				formatWidth = TextSizes[i].Width;
				if (formatWidth < minWidth) {
					minWidth = formatWidth;
					minFormatIndex = i;
				}
				if (formatWidth <= boundsWidth) {
					if (formatWidth > adjustedWidth) {
						adjustedWidth = formatWidth;
						adjustedFormatIndex = i;
					}
				}
			}
			if (adjustedFormatIndex < 0) {
				adjustedFormatIndex = minFormatIndex;
			}
			return adjustedFormatIndex;
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			if (TextSizes == null) {
				TextSizes = CalculateSizes();
			}
			int index = GetBestFitIndex(arrangeSize);
			TextBlock textBlock = Child as TextBlock;
			if(index >= 0 && textBlock != null) {
				string newText = Captions[index];
				if(newText != textBlock.Text)
					textBlock.Text = Captions[index];
			}
			return base.ArrangeOverride(arrangeSize);
		}
		protected virtual Size[] CalculateSizes() {
			if (Captions == null)
				Captions = CalculateCaptions();			
			TextBlock textBlock = Child as TextBlock;
			if (textBlock == null)
				return new Size[0];
			int count = Captions.Length;
			Size[] result = new Size[count];
			Size infinitySize = new Size(double.PositiveInfinity, double.PositiveInfinity);
			for (int i = 0; i < count; i++) {
				textBlock.Text = Captions[i];
				textBlock.Measure(infinitySize);
				result[i] = textBlock.DesiredSize;
			}
			return result;
		}
		protected abstract string[] CalculateCaptions();
	}
	public class DayHeaderControl : AutoTextFormatControlBase {
		static DayHeaderControl() {
		}
		public DayHeaderControl() {
#if !SL
			DefaultStyleKey = typeof(DayHeaderControl);
#endif
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);
		}
		#region DateFormats
		public ObservableStringCollection DateFormats {
			get { return (ObservableStringCollection)GetValue(DateFormatsProperty); }
			set { SetValue(DateFormatsProperty, value); }
		}
		public static readonly DependencyProperty DateFormatsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayHeaderControl, ObservableStringCollection>("DateFormats", null, (d, e)=>d.OnDateFormatsPropertyChanged(e.OldValue, e.NewValue));
		protected virtual void OnDateFormatsPropertyChanged(ObservableStringCollection oldValue, ObservableStringCollection newValue) {
			if (oldValue == newValue)
				return;
			UnsubscribeDateFormatsEvents(oldValue);
			SubscribeDateFormatsEvents(newValue);
			InvalidateContent();
		}
		#endregion
		#region Date
		public DateTime Date {
			get { return (DateTime)GetValue(DateProperty); }
			set { SetValue(DateProperty, value); }
		}
		public static readonly DependencyProperty DateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayHeaderControl, DateTime>("Date", DateTime.MinValue, (d, e) => d.OnDatePropertyChanged(e.OldValue, e.NewValue));
		protected virtual void OnDatePropertyChanged(DateTime oldValue, DateTime newValue) {
			if (oldValue != newValue)
				InvalidateContent();
		}
		#endregion
		protected void OnLoaded(object sender, RoutedEventArgs e) {
			UnsubscribeDateFormatsEvents(DateFormats);
			SubscribeDateFormatsEvents(DateFormats);
		}
		protected void OnUnloaded(object sender, RoutedEventArgs e) {
			UnsubscribeDateFormatsEvents(DateFormats);
		}
		protected void SubscribeDateFormatsEvents(ObservableStringCollection dateFormats) {
			if (dateFormats != null)
				SubscribeObservableCollectionEvents(dateFormats);
		}
		protected void UnsubscribeDateFormatsEvents(ObservableStringCollection dateFormats) {
			if (dateFormats != null)
				UnsubscirbeObservableCollectionEvents(dateFormats);
		}
		protected virtual void SubscribeObservableCollectionEvents(ObservableStringCollection collection) {
			collection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(DateFormatsCollectionChanged);
		}
		protected virtual void UnsubscirbeObservableCollectionEvents(ObservableStringCollection collection) {
			collection.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(DateFormatsCollectionChanged);
		}
		void DateFormatsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			InvalidateContent();
		}		
		protected override string[] CalculateCaptions() {
			if (DateFormats == null || DateFormats.Count == 0)
				return new string[0];
			int count = DateFormats.Count;
			string[] result = new string[count];
			for (int i = 0; i < count; i++) {
				result[i] = Date.ToString(DateFormats[i]);
			}
			return result;
		}	   
	}
	public class MonthViewDayHeaderControl : DayHeaderControl {
		protected override string[] CalculateCaptions() {
			if (Date.Day == 1) 
				return base.CalculateCaptions();
			else {
				return new string[] { Date.Day.ToString() };				
			}
		}
	}
}
