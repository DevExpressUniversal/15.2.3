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
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
namespace DevExpress.Xpf.Editors {
	public class RangeEditRange {
		public object SelectionStart { get; set; }
		public object SelectionEnd { get; set; }
		public RangeEditRange() {
		}
		public RangeEditRange(object selectionStart, object selectionEnd) {
			SelectionStart = selectionStart;
			SelectionEnd = selectionEnd;
		}
		public override bool Equals(object obj) {
			RangeEditRange range = obj as RangeEditRange;
			if(range == null)
				return false;
			return range.SelectionStart == SelectionStart && range.SelectionEnd == SelectionEnd;
		}
		public override int GetHashCode() {
			return (SelectionStart != null ? SelectionStart.GetHashCode() : 53) ^ (SelectionEnd != null ? SelectionEnd.GetHashCode() : 111);
		}
	}
	public class TrackBarEditRange {
		double selectionStart;
		double selectionEnd;
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TrackBarEditRangeSelectionStart")]
#endif
		public double SelectionStart { 
			get { return selectionStart; }
			set {
				if(selectionEnd < value)
					selectionEnd = value;
				selectionStart = value;
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TrackBarEditRangeSelectionEnd")]
#endif
		public double SelectionEnd {
			get { return selectionEnd; }
			internal set {
				if(selectionStart > value)
					selectionStart = value;
				selectionEnd = value;
			}
		}
		public TrackBarEditRange() {
		}
		public TrackBarEditRange(double selectionStart, double selectionEnd) {
			SelectionStart = selectionStart;
			SelectionEnd = selectionEnd;
		}
		public override bool Equals(object obj) {
			TrackBarEditRange range = obj as TrackBarEditRange;
			if(range == null)
				return false;
			return range.selectionStart == SelectionStart && range.SelectionEnd == SelectionEnd;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public class TrackBarEditRangeMultiValueConverter : IMultiValueConverter {
		static bool IsNullOrUnset(object value) { return value == null || value == DependencyProperty.UnsetValue; }
		object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if(IsNullOrUnset(values[0]) || IsNullOrUnset(values[1])) return DependencyProperty.UnsetValue;
			return new TrackBarEditRange(Convert.ToDouble(values[0]), Convert.ToDouble(values[1]));
		}
		object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			var range = value as TrackBarEditRange;
			if(range == null) return null;
			var type = targetTypes[0] ?? typeof(object);
			type = Nullable.GetUnderlyingType(type) ?? type;
			return new object[] { Convert.ChangeType(range.SelectionStart, type), Convert.ChangeType(range.SelectionEnd, type) };
		}
	}
	public class TrackBarEditRangeMultiValueConverterExtension : MarkupExtension {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new TrackBarEditRangeMultiValueConverter();
		}
	}
}
