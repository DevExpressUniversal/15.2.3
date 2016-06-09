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
using System.Windows.Media;
using System.Diagnostics;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.Xpf.Core;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#if SILVERLIGHT
using DevExpress.Xpf.Editors.Controls;
using Decorator =  DevExpress.Xpf.Core.WPFCompatibility.Decorator;
using DevExpress.Xpf.Scheduler.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#else
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class HeightLimitControl : Decorator {
		#region OuterSizeSourceElement
		public FrameworkElement OuterSizeSourceElement {
			get { return (FrameworkElement)GetValue(OuterSizeSourceElementProperty); }
			set { SetValue(OuterSizeSourceElementProperty, value); }
		}
		public static readonly DependencyProperty OuterSizeSourceElementProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<HeightLimitControl, FrameworkElement>("OuterSizeSourceElement", null, (d, e) => d.OnOuterSizeSourceElementChanged(e.OldValue, e.NewValue), null);
		void OnOuterSizeSourceElementChanged(FrameworkElement oldValue, FrameworkElement newValue) {
			if(oldValue != null)
				oldValue.SizeChanged -= OnSizeSourceElementSizeChanged;
			if(newValue != null)
				newValue.SizeChanged += OnSizeSourceElementSizeChanged;
		}
		#endregion
		LayoutInfo LayoutInfo { get; set; }
		protected override Size MeasureOverride(Size availableSize) {
			SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.HightLimitControl, "->HeightLimitControl.MeasureOverride [begin]: {0}", availableSize);
			Size size = base.MeasureOverride(availableSize);
			SchedulerLogger.Trace(XpfLoggerTraceLevel.HightLimitControl, "->measured size {0}", size);
			size.Height = Math.Min(size.Height, GetMaxHeight());
			if(Child != null) {
				UIElement visualChild = (UIElement)Child;
				SchedulerLogger.Trace(XpfLoggerTraceLevel.HightLimitControl, "->child.DesiredSize={0} (before child measure)", visualChild.DesiredSize);
				if(visualChild.DesiredSize.Height > size.Height) {
					visualChild.InvalidateMeasure();
					visualChild.Measure(size);
					SchedulerLogger.Trace(XpfLoggerTraceLevel.HightLimitControl, "->child.DesiredSize={0} (after child measure)", visualChild.DesiredSize);
				}
			}
			LayoutInfo = new LayoutInfo(availableSize);
			LayoutInfo.MeasuredSize = size;
			SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.HightLimitControl, "<-HeightLimitControl.MeasureOverride [end]: {0}", LayoutInfo.MeasuredSize);
			return size;
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.HightLimitControl,"->HeightLimitControl.ArrangeOverride: {0}, {1}", LayoutInfo.MeasuredSize, arrangeSize);
			if(!LayoutInfo.AreCloseWithMeasured(arrangeSize))
				Dispatcher.BeginInvoke((Action)(() => { UpdateTreePart(); }));
			if (Child != null) {
				UIElement visualChild = (UIElement)Child;
				visualChild.InvalidateArrange();
				visualChild.Arrange(new Rect(new Point(0, 0), arrangeSize));
			}
			SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.HightLimitControl);
			return base.ArrangeOverride(arrangeSize);
		}
		void OnSizeSourceElementSizeChanged(object sender, SizeChangedEventArgs e) {
			InvalidateMeasure();
		}
		protected internal virtual double GetMaxHeight() {
			return (int)OuterSizeSourceElement.ActualHeight / 2;
		}
		void UpdateTreePart() {
			SchedulerLogger.WriteWithoutIndent(XpfLoggerTraceLevel.HightLimitControl, "->HeightLimitControl.UpdateTreePart:");
			if(OuterSizeSourceElement == null)
				return;
			FrameworkElement parent = VisualTreeHelper.GetParent(this) as FrameworkElement;
			while(parent != null) {
				parent.InvalidateMeasure();
				if(parent == OuterSizeSourceElement)
					break;
				parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
			}
		}		
	}
	public class LayoutInfo {
		static readonly LayoutInfo empty = new EmptyLayoutInfo();
		public static LayoutInfo Empty { get { return empty; } }
		public LayoutInfo(Size availableSize) : this() {
			AvailableSize = availableSize;
		}
		protected LayoutInfo() {
			MeasuredSize = Size.Empty;
		}
		public virtual Size AvailableSize { get; set; }
		public virtual Size MeasuredSize { get; set; }
		protected bool AreClose(Size some, Size other) {
			return DoubleUtil.AreClose(some.Height, other.Height) && DoubleUtil.AreClose(some.Width, other.Width);
		}
		public bool CanUsePreviousHeight(Size size) {
			if(double.IsInfinity(size.Height))
				return false;
			return DoubleUtil.AreClose(AvailableSize.Height, size.Height);
		}
		public bool CanUsePreviousWidth(Size size) {
			if(double.IsInfinity(size.Width))
				return false;
			return DoubleUtil.AreClose(AvailableSize.Width, size.Width);
		}
		public bool CanUsePrevious(Size availableSize) {
			if(double.IsInfinity(availableSize.Height) && double.IsInfinity(availableSize.Width))
				return false;
			return AreClose(AvailableSize, availableSize);
		}
		public bool AreCloseWithMeasured(Size size) {
			return AreClose(MeasuredSize, size);
		}
	}
	public class EmptyLayoutInfo : LayoutInfo {
		public EmptyLayoutInfo() {
		}
		public override Size AvailableSize {
			get { return Size.Empty; }
			set {
			}
		}
		public override Size MeasuredSize {
			get { return Size.Empty; }
			set {
				base.MeasuredSize = value;
			}
		}
	}
	public class HeightLimitRestrictionControl : Panel {
		#region Padding
		public Thickness Padding {
			get { return (Thickness)GetValue(PaddingProperty); }
			set { SetValue(PaddingProperty, value); }
		}
		public static readonly DependencyProperty PaddingProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<HeightLimitRestrictionControl, Thickness>("Padding", new Thickness());
		#endregion
		protected override Size MeasureOverride(Size availableSize) {
			base.MeasureOverride(availableSize);
			if (Children.Count <= 0)
				return new Size();
			Debug.Assert(Children.Count == 1);
			HeightLimitControl heightLimitControl = LayoutHelper.FindParentObject<HeightLimitControl>(this);
			if (heightLimitControl == null)
				return new Size();
			double maxAllowableHeight = heightLimitControl.GetMaxHeight();
			FrameworkElement child = Children[0] as FrameworkElement;
			Debug.Assert(child != null);
			Size allowableSize = new Size(availableSize.Width, maxAllowableHeight);
			SizeHelper.Deflate(ref allowableSize, Padding);
			child.Measure(allowableSize);
			Size desiredSize = child.DesiredSize;
			SizeHelper.Inflate(ref desiredSize, Padding);
			return desiredSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Debug.Assert(Children.Count == 1);
			FrameworkElement child = Children[0] as FrameworkElement;
			Debug.Assert(child != null);
			child.Arrange(new Rect(new Point(0, 0), finalSize));
			return finalSize;
		}
	}
}
