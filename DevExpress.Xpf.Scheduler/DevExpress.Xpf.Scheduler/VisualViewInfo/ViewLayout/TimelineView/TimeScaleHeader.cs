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

using System.Windows;
using System;
using DevExpress.Xpf.Scheduler.Native;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using FrameworkPropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#endif 
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualTimeScaleHeader : SchedulerContentControl, ISupportCopyFrom<TimeScaleHeader> {
		static readonly PropertyPath StartOffsetPropertyPath = new PropertyPath("StartOffset");
		static readonly PropertyPath EndOffsetPropertyPath = new PropertyPath("EndOffset");
		static VisualTimeScaleHeader() {
		}
		public VisualTimeScaleHeader() {
			DefaultStyleKey = typeof(VisualTimeScaleHeader);
		}
		#region StartOffset
		public static readonly DependencyProperty StartOffsetProperty = VisualTimeScaleHeaderContent.StartOffsetProperty.AddOwner(typeof(VisualTimeScaleHeader), new FrameworkPropertyMetadata(OnStartOffsetPropertyChanged));
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualTimeScaleHeaderStartOffset")]
#endif
public SingleTimelineHeaderCellOffset StartOffset { get { return (SingleTimelineHeaderCellOffset)GetValue(StartOffsetProperty); } set { SetValue(StartOffsetProperty, value); } }
		#endregion
		#region EndOffset
		public static readonly DependencyProperty EndOffsetProperty = VisualTimeScaleHeaderContent.EndOffsetProperty.AddOwner(typeof(VisualTimeScaleHeader), new FrameworkPropertyMetadata (OnEndOffsetPropertyChanged));
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualTimeScaleHeaderEndOffset")]
#endif
public SingleTimelineHeaderCellOffset EndOffset { get { return (SingleTimelineHeaderCellOffset)GetValue(EndOffsetProperty); } set { SetValue(EndOffsetProperty, value); } }
		#endregion
		static void OnStartOffsetPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e) {
			InvalidateParentArrange(source);
		}
		static void OnEndOffsetPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e) {
			InvalidateParentArrange(source);
		}
		private static void InvalidateParentArrange(DependencyObject source) {
			DependencyObject panel = LayoutHelper.GetParent(source);
			if (panel!=null)
				((UIElement)panel).InvalidateArrange();
		}
		protected override Size MeasureInternal(Size constraint) {
			Size result = base.MeasureInternal(constraint);
			SchedulerLogger.Trace(XpfLoggerTraceLevel.VisualTimeScaleHeader, "->VisualTimeScaleHeader.MeasureInternal: {0}-({1}), constraint={2}, resultOfMeasure={3}", VisualElementHelper.GetElementName(this), VisualElementHelper.GetTypeName(this), constraint, result);
			return result;
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			Size size = base.ArrangeOverride(arrangeBounds);
			if (!DoubleUtil.AreClose(size.Width, RenderSize.Width) || !DoubleUtil.AreClose(size.Height, RenderSize.Height)) {
				FrameworkElement feParent = Parent as FrameworkElement;
				if (feParent != null)
					feParent.InvalidateArrange();
			}
			SchedulerLogger.Trace(XpfLoggerTraceLevel.VisualTimeScaleHeader, "->VisualTimeScaleHeader.ArrangeOverride: {0}-({1}), arrangeBoundes={2}, RenderSize={3}", VisualElementHelper.GetElementName(this), VisualElementHelper.GetTypeName(this), arrangeBounds, RenderSize);
			return size;
		}
		protected override void OnContentChanged(object oldContent, object newContent) {
			base.OnContentChanged(oldContent, newContent);
			VisualTimeScaleHeaderContent timeScaleHeaderContent = newContent as VisualTimeScaleHeaderContent;
			if (oldContent != null && timeScaleHeaderContent == null) {
				StartOffset = null;
				EndOffset = null;
			}
			else if (timeScaleHeaderContent != null) {
				BindToContentProperty(StartOffsetProperty, timeScaleHeaderContent, StartOffsetPropertyPath);
				BindToContentProperty(EndOffsetProperty, timeScaleHeaderContent, EndOffsetPropertyPath);
			}
		}
		protected virtual void BindToContentProperty(DependencyProperty dp, object source, PropertyPath sourcePropertyPath) {
			Binding binding = new Binding();
			binding.Mode = BindingMode.OneWay;
			binding.Source = source;
			binding.Path = sourcePropertyPath;
			binding.BindsDirectlyToSource = true;
			SetBinding(dp, binding);
		}
		protected virtual void CopyFrom(TimeScaleHeader source) {
			VisualTimeScaleHeaderContent content = Content as VisualTimeScaleHeaderContent;
			if (content != null)
				((ISupportCopyFrom<TimeScaleHeader>)content).CopyFrom(source);
		}
		#region ISupportCopyFrom<TimeScaleHeader> Members
		void ISupportCopyFrom<TimeScaleHeader>.CopyFrom(TimeScaleHeader source) {
			CopyFrom(source);
		}
		#endregion
	}	
	public class VisualTimeScaleHeaderContent : DependencyObject, ISupportCopyFrom<TimeScaleHeader> {
		#region StartOffset
		public static readonly DependencyProperty StartOffsetProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeScaleHeaderContent, SingleTimelineHeaderCellOffset>("StartOffset", null);
		public SingleTimelineHeaderCellOffset StartOffset { get { return (SingleTimelineHeaderCellOffset)GetValue(StartOffsetProperty); } set { SetValue(StartOffsetProperty, value); } }
		#endregion
		#region EndOffset
		public static readonly DependencyProperty EndOffsetProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeScaleHeaderContent, SingleTimelineHeaderCellOffset>("EndOffset", null);
		public SingleTimelineHeaderCellOffset EndOffset { get { return (SingleTimelineHeaderCellOffset)GetValue(EndOffsetProperty); } set { SetValue(EndOffsetProperty, value); } }
		#endregion
		#region IntervalStart
		public static readonly DependencyProperty IntervalStartProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeScaleHeaderContent, DateTime>("IntervalStart", DateTime.MinValue);
		public DateTime IntervalStart { get { return (DateTime)GetValue(IntervalStartProperty); } set { SetValue(IntervalStartProperty, value); } }
		#endregion
		#region IntervalEnd
		public static readonly DependencyProperty IntervalEndProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeScaleHeaderContent, DateTime>("IntervalEnd", DateTime.MinValue);
		public DateTime IntervalEnd { get { return (DateTime)GetValue(IntervalEndProperty); } set { SetValue(IntervalEndProperty, value); } }
		#endregion        
		#region IsAlternate
		public static readonly DependencyProperty IsAlternateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeScaleHeaderContent, bool>("IsAlternate", false);
		public bool IsAlternate { get { return (bool)GetValue(IsAlternateProperty); } set { SetValue(IsAlternateProperty, value); } }
		#endregion
		#region Caption
		public static readonly DependencyProperty CaptionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeScaleHeaderContent, string>("Caption", String.Empty);
		public string Caption { get { return (string)GetValue(CaptionProperty); } set { SetValue(CaptionProperty, value); } }
		#endregion
		#region ISupportCopyFrom<TimeScaleHeader> Members
		void ISupportCopyFrom<TimeScaleHeader>.CopyFrom(TimeScaleHeader source) {
			CopyFrom(source);
		}
		#endregion
		protected virtual void CopyFrom(TimeScaleHeader source) {
			this.IntervalStart = source.Interval.Start;
			this.IntervalEnd = source.Interval.End;
			this.IsAlternate = source.IsAlternate;
			this.StartOffset = source.StartOffset;
			this.EndOffset = source.EndOffset;
			this.Caption = source.Caption;
		}
	}
	public class VisualTimeScaleHeaderContentCollection : ObservableCollection<VisualTimeScaleHeaderContent> {
	}
	public class SchedulerTimeScaleHeaderItemsControl : GenericContainerItemsControl<VisualTimeScaleHeader> {
	}
}
