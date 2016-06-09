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
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Editors.RangeControl {
	public abstract class IntervalFactory {
		public virtual string ShortTextFormat { get; set; }
		public virtual string TextFormat { get; set; }
		public virtual string LongTextFormat { get; set; }
		public virtual string LongestTextFormat {get { return String.Empty; } }
		public virtual string LabelTextFormat { get { return String.Empty; } }
		public abstract object GetNextValue(object value);
		public abstract object Snap(object value);
		public abstract bool FormatText(object current, out string text, double fontSize, double length);
		public virtual string GetLongestText(object current) {
			return string.Format(CultureInfo.CurrentCulture, LongestTextFormat, current);
		}
		public virtual string FormatLabelText(object value) {
			return string.Format(CultureInfo.CurrentCulture, LabelTextFormat, value);
		}
	}
	public class DummyIntervalFactory : IntervalFactory {
		public override object GetNextValue(object value) {
			return value;
		}
		public override object Snap(object value) {
			return value;
		}
		public override bool FormatText(object current, out string text, double fontSize, double length) {
			text = string.Empty;
			return false;
		}
	}
	public class NumericIntervalFactory : IntervalFactory {
		const double DefaultMinLength = 5d;
		public double MinLength { get; set; }
		string DefaultTextFormat { get { return "{0:n}"; } }
		double ActualMinLength { get { return MinLength.IsZero() ? DefaultMinLength : MinLength; } }
		protected bool Equals(NumericIntervalFactory other) {
			return Step.Equals(other.Step);
		}
		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((NumericIntervalFactory)obj);
		}
		public override int GetHashCode() {
			return Step.GetHashCode();
		}
		public double Step { get; set; }
		public override object GetNextValue(object value) {
			return SnapInternal(Convert.ToDouble(value)) + Step;
		}
		public override object Snap(object value) {
			return SnapInternal(Convert.ToDouble(value));
		}
		double SnapInternal(double value) {
			int count = (int)(value / Step);
			return Step * count;
		}
		public override bool FormatText(object current, out string text, double fontSize, double length) {
			text = string.Empty;
			if (length < ActualMinLength * fontSize * (1 + Math.Log10(Step > 1 ? Step : 1 / Step) / 5))
				return false;
			text = string.Format(TextFormat ?? DefaultTextFormat, current);
			return true;
		}
	}
	public abstract class DateTimeIntervalFactory : IntervalFactory {
		protected abstract string DefaultShortTextFormat { get; }
		protected abstract string DefaultTextFormat { get; }
		protected abstract string DefaultLongTextFormat { get; }
		protected virtual double TextMaxLength { get { return 10 * MinLength; } }
		protected virtual double ShortTextMaxLength { get { return 2 * MinLength; } }
		protected bool Equals(DateTimeIntervalFactory other) {
			return object.Equals(GetType(), other.GetType());
		}
		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return Equals((DateTimeIntervalFactory)obj);
		}
		public override int GetHashCode() {
			return GetType().GetHashCode();
		}
		protected virtual double MinLength { get { return 2; } }
		public override object Snap(object value) {
			return value != null ? SnapInternal(ToDateTime(value)) : value;
		}
		protected abstract DateTime SnapInternal(DateTime dt);
		protected DateTime ToDateTime(object value) {
			return Convert.ToDateTime(value);
		}
		public override bool FormatText(object current, out string text, double fontSize, double length) {
			text = string.Empty;
			if (length < MinLength * fontSize) {
				text = FormatTextInternal(current, ShortTextFormat ?? DefaultShortTextFormat);
				return false;
			}
			if (length < ShortTextMaxLength * fontSize)
				text = FormatTextInternal(current, ShortTextFormat ?? DefaultShortTextFormat);
			else if (length < TextMaxLength * fontSize)
				text = FormatTextInternal(current, TextFormat ?? DefaultTextFormat);
			else
				text = FormatTextInternal(current, LongTextFormat ?? DefaultLongTextFormat);
			return true;
		}
		protected virtual string FormatTextInternal(object current, string format) {
			return string.Format(format, current);
		}
		public string FormatShortText(object current) {
			return FormatTextInternal(current, ShortTextFormat ?? DefaultShortTextFormat);
		}
		public string FormatMiddleText(object current) {
			return FormatTextInternal(current, TextFormat ?? DefaultTextFormat);
		}
		public string FormatLongText(object current) {
			return FormatTextInternal(current, LongTextFormat ?? DefaultLongTextFormat);
		}
	}
	public class YearIntervalFactory : DateTimeIntervalFactory {
		protected override string DefaultShortTextFormat { get { return "{0:yyyy}"; } }
		protected override string DefaultTextFormat { get { return "{0:yyyy}"; } }
		protected override string DefaultLongTextFormat { get { return "{0:yyyy}"; } }
		public override string LongestTextFormat { get { return "{0:yyyy}"; }  }
		public override string LabelTextFormat { get { return "{0:yyyy}"; } }
		protected override DateTime SnapInternal(DateTime value) {
			return new DateTime(value.Year, 1, 1, 0, 0, 0);
		}
		public override object GetNextValue(object value) {
			DateTime dt = ToDateTime(value);
			return SnapInternal(dt).AddYears(1);
		}
	}
	public class QuarterIntervalFactory : DateTimeIntervalFactory {
		protected override string DefaultShortTextFormat { get { return "Q{0}"; } }
		protected override string DefaultTextFormat { get { return "Q{0}, {1:yyyy}"; } }
		protected override string DefaultLongTextFormat { get { return "Q{0}, {1:yyyy}"; } }
		public override string LongestTextFormat { get { return "Q{0}, {1:yyyy}"; } }
		public override string LabelTextFormat { get { return "{0:MMM} {0:yyyy}"; } }
		protected override DateTime SnapInternal(DateTime value) {
			int quarter = GetQuarter(value);
			return new DateTime(value.Year, 3 * quarter + 1, 1, 0, 0, 0);
		}
		int GetQuarter(DateTime value) {
			return (value.Month - 1) / 3;
		}
		public override object GetNextValue(object value) {
			DateTime dt = ToDateTime(value);
			return SnapInternal(dt).AddMonths(3);
		}
		protected override string FormatTextInternal(object current, string format) {
			return string.Format(format, GetQuarter((DateTime)Snap(current)) + 1, current);
		}
		public override string GetLongestText(object current) {
			return FormatTextInternal(current, LongestTextFormat);
		}
	}
	public class MonthIntervalFactory : DateTimeIntervalFactory {
		protected override double ShortTextMaxLength { get { return 5 * MinLength; } }
		protected override string DefaultShortTextFormat { get { return "{0:MMM}"; } }
		protected override string DefaultTextFormat { get { return "{0:MMMM}"; } }
		protected override string DefaultLongTextFormat { get { return "{0:MMMM}, {0:yyyy}"; } }
		public override string LongestTextFormat { get { return "{0:MMMM}, {0:yyyy}"; } }
		public override string LabelTextFormat { get { return "{0:%d} {0:MMM} {0:yyyy}"; } }
		protected override DateTime SnapInternal(DateTime value) {
			return new DateTime(value.Year, value.Month, 1, 0, 0, 0);
		}
		public override object GetNextValue(object value) {
			DateTime dt = ToDateTime(value);
			return SnapInternal(dt).AddMonths(1);
		}
	}
	public class DayIntervalFactory : DateTimeIntervalFactory {
		protected override string DefaultShortTextFormat { get { return "{0:%d}"; } }
		protected override string DefaultTextFormat { get { return "{0:ddd}, {0:%d}"; } }
		protected override string DefaultLongTextFormat { get { return "{0:dddd}, {0:%d}"; } }
		public override string LongestTextFormat { get { return "{0:%d} {0:MMMM}, {0:yyyy}"; } }
		public override string LabelTextFormat { get { return "{0:%d} {0:MMMM}"; } }
		protected override DateTime SnapInternal(DateTime value) {
			return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
		}
		public override object GetNextValue(object value) {
			DateTime dt = ToDateTime(value);
			return SnapInternal(dt).AddDays(1);
		}
	}
	public class HourIntervalFactory : DateTimeIntervalFactory {
		protected override string DefaultShortTextFormat { get { return "{0:%h}"; } }
		protected override string DefaultTextFormat { get { return "{0:hh}"; } }
		protected override string DefaultLongTextFormat { get { return "{0:hh}"; } }
		public override string LongestTextFormat { get { return "{0:hh} {0:%d} {0:MMMM}, {0:yyyy}"; } }
		public override string LabelTextFormat { get { return "{0:hh} {0:%d} {0:MMMM}"; } }
		protected override DateTime SnapInternal(DateTime value) {
			return new DateTime(value.Year, value.Month, value.Day, value.Hour, 0, 0);
		}
		public override object GetNextValue(object value) {
			DateTime dt = ToDateTime(value);
			return SnapInternal(dt).AddHours(1);
		}
	}
	public class MinuteIntervalFactory : DateTimeIntervalFactory {
		protected override string DefaultShortTextFormat { get { return "{0:%m}"; } }
		protected override string DefaultTextFormat { get { return "{0:mm}"; } }
		protected override string DefaultLongTextFormat { get { return "{0:mm}"; } }
		public override string LongestTextFormat { get { return "{0:hh}:{0:mm} {0:%d} {0:MMMM}, {0:yyyy}"; } }
		public override string LabelTextFormat { get { return "{0:hh}:{0:mm} {0:%d}"; } }
		protected override DateTime SnapInternal(DateTime value) {
			return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0);
		}
		public override object GetNextValue(object value) {
			DateTime dt = ToDateTime(value);
			return SnapInternal(dt).AddMinutes(1);
		}
	}
	public class SecondIntervalFactory : DateTimeIntervalFactory {
		protected override string DefaultShortTextFormat { get { return "{0:%s}"; } }
		protected override string DefaultTextFormat { get { return "{0:ss}"; } }
		protected override string DefaultLongTextFormat { get { return "{0:ss}"; } }
		public override string LongestTextFormat { get { return "{0:hh}:{0:mm}:{0:ss} {0:%d} {0:MMMM}, {0:yyyy}"; } }
		public override string LabelTextFormat { get { return "{0:hh}:{0:mm}:{0:ss}"; } }
		protected override DateTime SnapInternal(DateTime value) {
			return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
		}
		public override object GetNextValue(object value) {
			DateTime dt = ToDateTime(value);
			return SnapInternal(dt).AddSeconds(1);
		}
	}
}
