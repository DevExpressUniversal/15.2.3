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

using DevExpress.Xpf.Core.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
#if SILVERLIGHT
using System.Threading;
using System.Windows.Threading;
#endif
namespace DevExpress.Xpf.WindowsUI.Base {
	class VisibilityHelper {
		public static Visibility Convert(bool isVisible, Visibility invisibleValue = Visibility.Collapsed) {
			return isVisible ? Visibility.Visible : invisibleValue;
		}
	}
	static class MathHelper {
		public static double GetDimension(double value, double defaultDimension = 0.0) {
			return double.IsNaN(value) ? defaultDimension : value;
		}
		public static bool AreClose(double value1, double value2, double epsilon = 1e-9) {
			return Math.Abs(value1 - value2) < epsilon;
		}
		public static bool IsZero(double value, double epsilon = 1e-9) {
			return Math.Abs(value) < epsilon;
		}
		public static Rect Deflate(Rect rect, Thickness padding) {
			double left = GetDimension(padding.Left);
			double top = GetDimension(padding.Top);
			double right = GetDimension(padding.Right);
			double bottom = GetDimension(padding.Bottom);
			return new Rect(rect.Left + left, rect.Top + top,
				Math.Max(0, rect.Width - (left + right)),
				Math.Max(0, rect.Height - (top + bottom)));
		}
		public static Rect Inflate(Rect rect, Thickness padding) {
			double left = GetDimension(padding.Left);
			double top = GetDimension(padding.Top);
			double right = GetDimension(padding.Right);
			double bottom = GetDimension(padding.Bottom);
			return new Rect(rect.Left - left, rect.Top - top, rect.Width + (left + right), rect.Height + (top + bottom));
		}
		public static Size Deflate(Size size, Thickness padding) {
			double w = GetDimension(padding.Left) + GetDimension(padding.Right);
			double h = GetDimension(padding.Top) + GetDimension(padding.Bottom);
			return new Size(Math.Max(0, size.Width - w), Math.Max(0, size.Height - h));
		}
		public static Size Inflate(Size size, Thickness padding) {
			double w = GetDimension(padding.Left) + GetDimension(padding.Right);
			double h = GetDimension(padding.Top) + GetDimension(padding.Bottom);
			return new Size(size.Width + w, size.Height + h);
		}
		public static Rect DeflateSize(Size size, Thickness padding) {
			return Deflate(new Rect(0, 0, size.Width, size.Height), padding);
		}
		public static Rect InflateSize(Size size, Thickness padding) {
			return Inflate(new Rect(0, 0, size.Width, size.Height), padding);
		}
		public static Size Union(IEnumerable<Size> sizes) {
			Size result = new Size(0, 0);
			foreach(Size s in sizes)
				result = Union(result, s);
			return result;
		}
		public static Size Union(Size size, Size s) {
			double w = Math.Max(MathHelper.GetDimension(s.Width), size.Width);
			double h = Math.Max(MathHelper.GetDimension(s.Height), size.Height);
			return new Size(w, h);
		}
	}
#if !SILVERLIGHT
	class EnumerableObservableWrapper<T> : List<T>, INotifyCollectionChanged, IWeakEventListener, IRefreshable {
		readonly IEnumerable enumerable;
		public EnumerableObservableWrapper(IEnumerable enumerable) {
			this.enumerable = enumerable;
			if(enumerable is INotifyCollectionChanged)
				CollectionChangedEventManager.AddListener((INotifyCollectionChanged)enumerable, this);
			Repopulate();
		}
		void Repopulate() {
			Clear();
			foreach(T obj in enumerable)
				Add(obj);
		}
		void OnInnerCollectionChanged(NotifyCollectionChangedEventArgs e) {
			Repopulate();
			if(CollectionChanged != null)
				CollectionChanged(this, e);
		}
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if(managerType == typeof(CollectionChangedEventManager)) {
				OnInnerCollectionChanged((NotifyCollectionChangedEventArgs)e);
				return true;
			}
			return false;
		}
		void IRefreshable.Refresh() {
			Repopulate();
		}
	}
#endif
}
