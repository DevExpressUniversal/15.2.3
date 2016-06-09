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
using PlatformIndependentScrollEventArgs = System.Windows.Forms.ScrollEventArgs;
using PlatformIndependentScrollEventHandler = System.Windows.Forms.ScrollEventHandler;
using PlatformIndependentScrollEventType = System.Windows.Forms.ScrollEventType;
namespace DevExpress.Office.Internal {
	#region IXpfOfficeScrollbar
	public interface IXpfOfficeScrollbar : IOfficeScrollbar {
		Visibility Visibility { get; set; }
		double ViewportSize { get; set; }
		Dispatcher Dispatcher { get; }
		GeneralTransform TransformToVisual(UIElement visual);
	}
	#endregion
	#region XpfOfficeScrollbar
	public class XpfOfficeScrollbar : IXpfOfficeScrollbar {
		#region Fields
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
		readonly ScrollBar scrollBar;
		#endregion
		public XpfOfficeScrollbar(ScrollBar scrollBar) {
			this.scrollBar = scrollBar;
			scrollBar.ValueChanged += OnValueChanged;
		}
		#region Events
		PlatformIndependentScrollEventHandler onScroll;
		event PlatformIndependentScrollEventHandler IOfficeScrollbar.Scroll {
			add { onScroll += value; }
			remove { onScroll -= value; }
		}
		protected internal virtual void RaiseScroll(PlatformIndependentScrollEventArgs args) {
			if (onScroll != null)
				onScroll(this, args);
		}
		#endregion
		#region Properties
		public ScrollBar ScrollBar { get { return scrollBar; } }
		Dispatcher IXpfOfficeScrollbar.Dispatcher { get { return ScrollBar.Dispatcher; } }
		int IOfficeScrollbar.SmallChange {
			get { return (int)Math.Round(ScrollBar.SmallChange); }
			set { ScrollBar.SmallChange = value; }
		}
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
		int IOfficeScrollbar.Value {
			get { return (int)Math.Round(ScrollBar.Value); }
			set { ScrollBar.Value = value; }
		}
		bool IOfficeScrollbar.Enabled {
			get { return ScrollBar.IsEnabled; }
			set { ScrollBar.IsEnabled = value; }
		}
		double IXpfOfficeScrollbar.ViewportSize {
			get { return ScrollBar.ViewportSize; }
			set { ScrollBar.ViewportSize = value; }
		}
		Visibility IXpfOfficeScrollbar.Visibility {
			get { return ScrollBar.Visibility; }
			set { ScrollBar.Visibility = value; }
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
		#endregion
		void IOfficeScrollbar.BeginUpdate() {
		}
		void IOfficeScrollbar.EndUpdate() {
		}
		void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			int oldValue = ConvertScrollBarValue(e.OldValue);
			int newValue = ConvertScrollBarValue(e.NewValue);
			if (oldValue != newValue) {
				PlatformIndependentScrollEventArgs args = new PlatformIndependentScrollEventArgs(PlatformIndependentScrollEventType.ThumbTrack, oldValue, newValue);
				RaiseScroll(args);
				e.Handled = true;
			}
		}
		protected PlatformIndependentScrollEventArgs ConvertEventArgs(ScrollEventArgs e, int oldValue) {
			int newValue = ConvertScrollBarValue(e.NewValue);
			PlatformIndependentScrollEventType scrollEventType = ConvertScrollEventType(e.ScrollEventType);
			return new PlatformIndependentScrollEventArgs(scrollEventType, oldValue, newValue);
		}
		public static PlatformIndependentScrollEventType ConvertScrollEventType(ScrollEventType value) {
			return scrollEventTypeTable[value];
		}
		public static int ConvertScrollBarValue(double value) {
			return (int)Math.Round(value);
		}
	}
	#endregion
}
