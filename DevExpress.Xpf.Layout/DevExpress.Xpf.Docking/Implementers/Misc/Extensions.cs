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
using System.Windows.Input;
using DevExpress.Xpf.Bars;
using SWC = System.Windows.Controls;
using CoreDock = System.Windows.Controls.Dock;
namespace DevExpress.Xpf.Docking {
	public static class DockExtensions {
		public static Cursor ToCursor(this SWC.Dock dock) {
			switch(dock) {
				case SWC.Dock.Right:
				case SWC.Dock.Left:
					return Cursors.SizeWE;
				case SWC.Dock.Top:
				case SWC.Dock.Bottom:
					return Cursors.SizeNS;
				default: 
					return Cursors.Arrow;
			}
		}
		public static BarContainerType ToBarContainerType(this CoreDock dock) {
			switch(dock) {
				case CoreDock.Right:
					return BarContainerType.Right;
				case CoreDock.Left:
					return BarContainerType.Left;
				case CoreDock.Top:
					return BarContainerType.Top;
				case CoreDock.Bottom:
					return BarContainerType.Bottom;
				default:
					return BarContainerType.None;
			}
		}
		public static HorizontalAlignment ToHorizontalAlignment(SWC.Dock dock, bool IsInverted = false) {
			HorizontalAlignment alignment;
			switch(dock) {
				case SWC.Dock.Left:
					alignment = IsInverted ? HorizontalAlignment.Right : HorizontalAlignment.Left;
					break;
				case SWC.Dock.Right:
					alignment = IsInverted ? HorizontalAlignment.Left : HorizontalAlignment.Right;
					break;
				default:
					alignment = HorizontalAlignment.Stretch;
					break;
			}
			return alignment;
		}
		public static VerticalAlignment ToVerticalAlignment(SWC.Dock dock, bool IsInverted = false) {
			VerticalAlignment alignment;
			switch(dock) {
				case SWC.Dock.Top:
					alignment = IsInverted ? VerticalAlignment.Bottom : VerticalAlignment.Top;
					break;
				case SWC.Dock.Bottom:
					alignment = IsInverted ? VerticalAlignment.Top : VerticalAlignment.Bottom;
					break;
				default:
					alignment = VerticalAlignment.Stretch;
					break;
			}
			return alignment;
		}
	}
	internal static class MDIMergeStyleExtensions {
		internal static bool IsDefault(this MDIMergeStyle value) {
			return value == MDIMergeStyle.Default || value == MDIMergeStyle.WhenChildActivated;
		}
	}
	static class IDictionaryExtensions {
		public static TKey GetKeyByValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value) {
			if(dictionary == null) throw new System.ArgumentNullException("dictionary");
			foreach(KeyValuePair<TKey, TValue> pair in dictionary)
				if(value.Equals(pair.Value)) return pair.Key;
			throw new System.ArgumentException("value");
		}
	}
	static class EnumerableEx {
		public static T FirstOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate, T defaultValue) {
			if(enumerable == null) throw new ArgumentNullException("enumerable");
			if(predicate == null) throw new ArgumentNullException("predicate");
			foreach(T value in enumerable) {
				if(predicate(value)) return value;
			}
			return defaultValue;
		}
	}
}
