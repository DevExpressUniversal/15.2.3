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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using DevExpress.Office.Internal;
using DevExpress.Utils;
#if SL
using PlatformIndependentScrollEventHandler = System.Windows.Controls.Primitives.ScrollEventHandler;
using PlatformIndependentScrollEventArgs = System.Windows.Controls.Primitives.ScrollEventArgs;
using PlatformIndependentScrollEventType = System.Windows.Controls.Primitives.ScrollEventType;
#else
using PlatformIndependentScrollEventArgs = System.Windows.Forms.ScrollEventArgs;
using PlatformIndependentScrollEventHandler = System.Windows.Forms.ScrollEventHandler;
using PlatformIndependentScrollEventType = System.Windows.Forms.ScrollEventType;
#endif
namespace DevExpress.Office.Internal {
	#region IXpfOfficeScrollbar
	public interface IXpfOfficeScrollbar : IOfficeScrollbar {
		Visibility Visibility { get; set; }
		double ViewportSize { get; set; }
		Dispatcher Dispatcher { get; }
		GeneralTransform TransformToVisual(UIElement visual);
	}
	#endregion
	public class XpfOfficeScrollbar : IXpfOfficeScrollbar {
#if !SL
		static readonly Dictionary<ScrollEventType, PlatformIndependentScrollEventType> scrollEventTypeTable = CreateScrollEventTypeTable();
		static Dictionary<ScrollEventType, PlatformIndependentScrollEventType> CreateScrollEventTypeTable() {
			Dictionary<ScrollEventType, PlatformIndependentScrollEventType> result = new Dictionary<ScrollEventType, PlatformIndependentScrollEventType>();
			result.Add(ScrollEventType.SmallIncrement, PlatformIndependentScrollEventType.SmallIncrement);
			result.Add(ScrollEventType.SmallDecrement, PlatformIndependentScrollEventType.SmallDecrement);
			result.Add(ScrollEventType.LargeIncrement, PlatformIndependentScrollEventType.LargeIncrement);
			result.Add(ScrollEventType.LargeDecrement, PlatformIndependentScrollEventType.LargeDecrement);
			result.Add(ScrollEventType.ThumbPosition, PlatformIndependentScrollEventType.ThumbPosition);
			result.Add(ScrollEventType.ThumbTrack, PlatformIndependentScrollEventType.First);
			result.Add(ScrollEventType.Last, PlatformIndependentScrollEventType.Last);
			result.Add(ScrollEventType.EndScroll, PlatformIndependentScrollEventType.EndScroll);
			return result;
		}
#endif
		readonly ScrollBar scrollBar;
		public XpfOfficeScrollbar(ScrollBar scrollBar) {
			this.scrollBar = scrollBar;
			scrollBar.Scroll += OnScrollBarScroll;
			scrollBar.ValueChanged += ScrollBarValueChanged;
		}
		int OldValue { get; set; }
		void ScrollBarValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			OldValue = (int)e.OldValue;
		}
		public ScrollBar ScrollBar { get { return scrollBar; } }
		#region IOfficeScrollbar Members
		void IOfficeScrollbar.BeginUpdate() {
		}
		void IOfficeScrollbar.EndUpdate() {
		}
		Dispatcher IXpfOfficeScrollbar.Dispatcher { get { return ScrollBar.Dispatcher; } }
		int IOfficeScrollbar.LargeChange {
			get { return (int)Math.Round(ScrollBar.LargeChange); }
			set { ScrollBar.LargeChange = value; }
		}
		int IOfficeScrollbar.Maximum {
			get { return (int)Math.Round(ScrollBar.Maximum); }
			set { ScrollBar.Maximum = value; }
		}
		int IOfficeScrollbar.Minimum {
			get { return (int)Math.Round(ScrollBar.Minimum); }
			set { ScrollBar.Minimum = value; }
		}
		bool IOfficeScrollbar.Enabled {
			get { return ScrollBar.IsEnabled; }
			set { ScrollBar.IsEnabled = value; }
		}
		PlatformIndependentScrollEventHandler onScroll;
		event PlatformIndependentScrollEventHandler IOfficeScrollbar.Scroll { 
			add { onScroll += value; } 
			remove { onScroll -= value; } 
		}
		protected internal virtual void RaiseScroll(PlatformIndependentScrollEventArgs args) {
			if (onScroll != null)
				onScroll(this, args);
		}
		int IOfficeScrollbar.SmallChange {
			get { return (int)Math.Round(ScrollBar.SmallChange); }
			set { ScrollBar.SmallChange = value; }
		}
		GeneralTransform IXpfOfficeScrollbar.TransformToVisual(UIElement visual) {
			try {
				return ScrollBar.TransformToVisual(visual);
			}
			catch {
				TranslateTransform result = new TranslateTransform();
				result.X = 0;
				result.Y = 0;
				return result;
			}
		}
		int IOfficeScrollbar.Value {
			get { return (int)Math.Round(ScrollBar.Value); }
			set { ScrollBar.Value = value; }
		}
		double IXpfOfficeScrollbar.ViewportSize {
			get { return ScrollBar.ViewportSize; }
			set { ScrollBar.ViewportSize = value; }
		}
		Visibility IXpfOfficeScrollbar.Visibility {
			get { return ScrollBar.Visibility; }
			set { ScrollBar.Visibility = value; }
		}
		#endregion
		void OnScrollBarScroll(object sender, ScrollEventArgs e) {
			RaiseScroll(ConvertEventArgs(e));
		}
		protected PlatformIndependentScrollEventArgs ConvertEventArgs(ScrollEventArgs e, int oldValue) {
#if SL
			return e;
#else
			int newValue = ConvertScrollBarValue(e.NewValue);
#if SPREADSHEET
			if (e.ScrollEventType == ScrollEventType.ThumbTrack && ((IOfficeScrollbar)this).Value == ((IOfficeScrollbar)this).Maximum)
				newValue = oldValue;
#endif
			return new PlatformIndependentScrollEventArgs(ConvertScrollEventType(e.ScrollEventType), oldValue, newValue);
#endif
		}
		protected PlatformIndependentScrollEventArgs ConvertEventArgs(ScrollEventArgs e) {
			return ConvertEventArgs(e, OldValue);
		}
		public static PlatformIndependentScrollEventType ConvertScrollEventType(ScrollEventType value) {
#if !SL
			return scrollEventTypeTable[value];
#else
			return value;
#endif
		}
#if !SL
		public static int ConvertScrollBarValue(double value) {
			return (int)Math.Floor(value);
		}
#else
		public static double ConvertScrollBarValue(double value) {
			return value;
		}
#endif
	}
}
