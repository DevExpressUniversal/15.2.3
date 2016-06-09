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

using DevExpress.Mvvm.Native;
using System;
using System.Globalization;
namespace DevExpress.Mvvm.DataAnnotations {
	[AttributeUsage(AttributeTargets.Property)]
	public abstract class MaskAttributeBase : Attribute {
		public string Mask { get; set; }
		public bool UseAsDisplayFormat { get; set; }
		protected MaskAttributeBase() {
			UseAsDisplayFormat = true;
		}
		internal CultureInfo CultureInternal { get { return CultureInfoCore ?? (string.IsNullOrEmpty(CultureNameCore) ? null : new CultureInfo(CultureNameCore)); } }
		protected virtual string CultureNameCore { get { return null; } }
		protected virtual CultureInfo CultureInfoCore { get { return null; } }
		internal virtual bool AutomaticallyAdvanceCaretInternal { get { return MaskInfo.DefaultAutomaticallyAdvanceCaretValue; } }
		internal virtual bool IgnoreBlankInternal { get { return MaskInfo.DefaultIgnoreBlankValue; } }
		internal virtual char PlaceHolderInternal { get { return MaskInfo.DefaultPlaceHolderValue; } }
		internal virtual bool SaveLiteralInternal { get { return MaskInfo.DefaultSaveLiteralValue; } }
		internal virtual bool ShowPlaceHoldersInternal { get { return MaskInfo.DefaultShowPlaceHoldersValue; } }
		internal virtual bool IsDefaultMask(string mask) { return string.IsNullOrEmpty(mask); }
	}
	public class NumericMaskAttribute : MaskAttributeBase {
		public string Culture { get; set; }
		internal CultureInfo CultureInfo { get; set; }
		protected override string CultureNameCore { get { return Culture; } }
		protected override CultureInfo CultureInfoCore { get { return CultureInfo; } }
	}
	public class DateTimeMaskAttribute : MaskAttributeBase {
		public string Culture { get; set; }
		internal CultureInfo CultureInfo { get; set; }
		public bool AutomaticallyAdvanceCaret { get; set; }
		public DateTimeMaskAttribute() {
			Mask = MaskInfo.DefaultDateTimeMaskValue;
		}
		protected override string CultureNameCore { get { return Culture; } }
		protected override CultureInfo CultureInfoCore { get { return CultureInfo; } }
		internal override bool AutomaticallyAdvanceCaretInternal { get { return AutomaticallyAdvanceCaret; } }
		internal override bool IsDefaultMask(string mask) { return mask == MaskInfo.DefaultDateTimeMaskValue; }
	}
	public abstract class RegExMaskAttributeBase : MaskAttributeBase {
		public bool IgnoreBlank { get; set; }
		public char PlaceHolder { get; set; }
		internal override bool IgnoreBlankInternal { get { return IgnoreBlank; } }
		internal override char PlaceHolderInternal { get { return PlaceHolder; } }
		internal abstract RegExMaskType RegExMaskType { get; }
		public RegExMaskAttributeBase() {
			IgnoreBlank = MaskInfo.DefaultIgnoreBlankValue;
			PlaceHolder = MaskInfo.DefaultPlaceHolderValue;
			UseAsDisplayFormat = false;
		}
	}
	public class RegExMaskAttribute : RegExMaskAttributeBase {
		public bool ShowPlaceHolders { get; set; }
		public RegExMaskAttribute() {
			ShowPlaceHolders = MaskInfo.DefaultShowPlaceHoldersValue;
		}
		internal override bool ShowPlaceHoldersInternal { get { return ShowPlaceHolders; } }
		internal override RegExMaskType RegExMaskType { get { return RegExMaskType.RegEx; } }
	}
	public class SimpleMaskAttribute : RegExMaskAttributeBase {
		public bool SaveLiteral { get; set; }
		public SimpleMaskAttribute() {
			SaveLiteral = MaskInfo.DefaultSaveLiteralValue;
		}
		internal override bool SaveLiteralInternal { get { return SaveLiteral; } }
		internal override RegExMaskType RegExMaskType { get { return RegExMaskType.Simple; } }
	}
	public class RegularMaskAttribute : RegExMaskAttributeBase {
		public bool SaveLiteral { get; set; }
		public RegularMaskAttribute() {
			SaveLiteral = MaskInfo.DefaultSaveLiteralValue;
		}
		internal override bool SaveLiteralInternal { get { return SaveLiteral; } }
		internal override RegExMaskType RegExMaskType { get { return RegExMaskType.Regular; } }
	}
	public static class PredefinedMasks {
		public static class DateTime {
			public const string ShortDate = "d";
			public const string LongDate = "D";
			public const string ShortTime = "t";
			public const string LongTime = "T";
			public const string FullDateShortTime = "f";
			public const string FullDateLongTime = "F";
			public const string GeneralDateShortTime = "g";
			public const string GeneralDateLongTime = "G";
			public const string MonthDay = "m";
			public const string RFC1123 = "r";
			public const string SortableDateTime = "s";
			public const string UniversalSortableDateTime = "u";
			public const string YearMonth = "y";
		}
		public static class Numeric {
			public const string Currency = "c";
			public const string Decimal = "d";
			public const string FixedPoint = "f";
			public const string Number = "n";
			public const string Percent = "P";
			public const string PercentFractional = "p";
		}
	}
}
