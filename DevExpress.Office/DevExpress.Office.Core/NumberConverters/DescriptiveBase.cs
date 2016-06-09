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
using System.Text;
using DevExpress.Office.Utils;
using System.Diagnostics;
namespace DevExpress.Office.NumberConverters {
	#region INumericsProvider
	public interface INumericsProvider {
		string[] Separator { get; }
		string[] SinglesNumeral { get; }
		string[] Singles { get; }
		string[] Teens { get; }
		string[] Tenths { get; }
		string[] Hundreds { get; }
		string[] Thousands { get; }
		string[] Million { get; }
		string[] Billion { get; }
		string[] Trillion { get; }
		string[] Quadrillion { get; }
		string[] Quintillion { get; }
	}
	#endregion
	#region DigitType
	public enum DigitType {
		Zero,
		SingleNumeral,
		Single,
		Teen,
		Tenth,
		Hundred,
		Thousand,
		Million,
		Billion,
		Trillion,
		Quadrillion,
		Quintillion,
		Separator
	}
	#endregion
	#region DigitInfo (abstract class)
	public abstract class DigitInfo {
		#region Fields
		INumericsProvider provider;
		long number;
		#endregion
		protected DigitInfo(INumericsProvider provider, long number) {
			System.Diagnostics.Debug.Assert(number >= 0);
			this.provider = provider;
			this.number = number;
		}
		#region Properties
		public abstract DigitType Type { get; }
		public INumericsProvider Provider { get { return provider; } set { provider = value; } }
		public long Number { get { return number; } }
		#endregion
		public string ConvertToString() {
			string[] numerics = GetNumerics();
			System.Diagnostics.Debug.Assert(number < numerics.Length);
			return numerics[number];
		}
		protected internal abstract string[] GetNumerics();
	}
	#endregion
	#region DigitInfoCollection
	public class DigitInfoCollection : List<DigitInfo> {
		public DigitInfo Last { get { return this[Count - 1]; } }
	}
	#endregion
	#region SingleNumeralDigitInfo
	public class SingleNumeralDigitInfo : DigitInfo {
		public SingleNumeralDigitInfo(INumericsProvider provider, long number)
			: base(provider, number - 1) {
		}
		public override DigitType Type { get { return DigitType.SingleNumeral; } }
		protected internal override string[] GetNumerics() {
			return Provider.SinglesNumeral;
		}
	}
	#endregion
	#region ZeroDigitInfo
	public class ZeroDigitInfo : DigitInfo {
		public ZeroDigitInfo(INumericsProvider provider)
			: base(provider, 9) {
		}
		public override DigitType Type { get { return DigitType.Zero; } }
		protected internal override string[] GetNumerics() {
			return Provider.Singles;
		}
	}
	#endregion
	#region SingleDigitInfo
	public class SingleDigitInfo : DigitInfo {
		public SingleDigitInfo(INumericsProvider provider, long number)
			: base(provider, number - 1) {
		}
		public override DigitType Type { get { return DigitType.Single; } }
		protected internal override string[] GetNumerics() {
			return Provider.Singles;
		}
	}
	#endregion
	#region TeensDigitInfo
	public class TeensDigitInfo : DigitInfo {
		public TeensDigitInfo(INumericsProvider provider, long number)
			: base(provider, number) {
		}
		public override DigitType Type { get { return DigitType.Teen; } }
		protected internal override string[] GetNumerics() {
			return Provider.Teens;
		}
	}
	#endregion
	#region TenthsDigitInfo
	public class TenthsDigitInfo : DigitInfo {
		public TenthsDigitInfo(INumericsProvider provider, long number)
			: base(provider, number - 2) {
		}
		public override DigitType Type { get { return DigitType.Tenth; } }
		protected internal override string[] GetNumerics() {
			return Provider.Tenths;
		}
	}
	#endregion
	#region HundredsDigitInfo
	public class HundredsDigitInfo : DigitInfo {
		public HundredsDigitInfo(INumericsProvider provider, long number)
			: base(provider, number - 1) {
		}
		public override DigitType Type { get { return DigitType.Hundred; } }
		protected internal override string[] GetNumerics() {
			return Provider.Hundreds;
		}
	}
	#endregion
	#region ThousandsDigitInfo
	public class ThousandsDigitInfo : DigitInfo {
		public ThousandsDigitInfo(INumericsProvider provider, long number)
			: base(provider, number) {
		}
		public override DigitType Type { get { return DigitType.Thousand; } }
		protected internal override string[] GetNumerics() {
			return Provider.Thousands;
		}
	}
	#endregion
	#region MillionDigitInfo
	public class MillionDigitInfo : DigitInfo {
		public MillionDigitInfo(INumericsProvider provider, long number)
			: base(provider, number) {
		}
		public override DigitType Type { get { return DigitType.Million; } }
		protected internal override string[] GetNumerics() {
			return Provider.Million;
		}
	}
	#endregion
	#region BillionDigitInfo
	public class BillionDigitInfo : DigitInfo {
		public BillionDigitInfo(INumericsProvider provider, long number)
			: base(provider, number) {
		}
		public override DigitType Type { get { return DigitType.Billion; } }
		protected internal override string[] GetNumerics() {
			return Provider.Billion;
		}
	}
	#endregion
	#region TrillionDigitInfo
	public class TrillionDigitInfo : DigitInfo {
		public TrillionDigitInfo(INumericsProvider provider, long number)
			: base(provider, number) {
		}
		public override DigitType Type { get { return DigitType.Trillion; } }
		protected internal override string[] GetNumerics() {
			return Provider.Trillion;
		}
	}
	#endregion
	#region QuadrillionDigitInfo
	public class QuadrillionDigitInfo : DigitInfo {
		public QuadrillionDigitInfo(INumericsProvider provider, long number)
			: base(provider, number) {
		}
		public override DigitType Type { get { return DigitType.Quadrillion; } }
		protected internal override string[] GetNumerics() {
			return Provider.Quadrillion;
		}
	}
	#endregion
	#region QuintillionDigitInfo
	public class QuintillionDigitInfo : DigitInfo {
		public QuintillionDigitInfo(INumericsProvider provider, long number)
			: base(provider, number) {
		}
		public override DigitType Type { get { return DigitType.Quintillion; } }
		protected internal override string[] GetNumerics() {
			return Provider.Quintillion;
		}
	}
	#endregion
	#region SeparatorDigitInfo
	public class SeparatorDigitInfo : DigitInfo {
		public SeparatorDigitInfo(INumericsProvider provider, long number)
			: base(provider, number) {
		}
		public override DigitType Type { get { return DigitType.Separator; } }
		protected internal override string[] GetNumerics() {
			return Provider.Separator;
		}
	}
	#endregion
	#region DescriptiveNumberConverterBase (abstract class)
	public abstract class DescriptiveNumberConverterBase : OrdinalBasedNumberConverter {
		#region Properties
		protected internal bool FlagThousand { get; set; }
		protected internal bool FlagMillion { get; set; }
		protected internal bool FlagBillion { get; set; }
		protected internal bool FlagTrillion { get; set; }
		protected internal bool FlagQuadrillion { get; set; }
		protected internal bool FlagQuintillion { get; set; }
		protected internal bool IsValueGreaterThousand { get { return FlagMillion || FlagBillion || FlagTrillion || FlagQuadrillion || FlagQuintillion; } }
		protected internal bool IsValueGreaterHundred { get { return FlagThousand || IsValueGreaterThousand; } }
		protected internal override long MinValue { get { return 0; } }
		#endregion
		#region GenerateQuintillionSeparator
		protected internal virtual void GenerateQuintillionSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count != 0)
				digits.Add(new SeparatorDigitInfo(provider, 0));
		}
		#endregion
		#region GenerateQuintillionProvider
		protected internal virtual INumericsProvider GenerateQuintillionProvider() {
			return new CardinalEnglishNumericsProvider();
		}
		#endregion
		#region GenerateQuintillionDigits
		protected internal virtual void GenerateQuintillionDigits(DigitInfoCollection digits, long value) {
			FlagQuintillion = true;
			GenerateDigitsInfo(digits, value);
			GenerateQuintillionSeparator(digits, GenerateQuintillionProvider(), value);
			digits.Add(new QuintillionDigitInfo(GenerateQuintillionProvider(), 0));
			FlagQuintillion = false;
		}
		#endregion
		#region GenerateQuadrillionSeparator
		protected internal virtual void GenerateQuadrillionSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count != 0)
				digits.Add(new SeparatorDigitInfo(provider, 0));
		}
		#endregion
		#region GenerateQuadrillionProvider
		protected internal virtual INumericsProvider GenerateQuadrillionProvider() {
			return new CardinalEnglishNumericsProvider();
		}
		#endregion
		#region GenerateQuadrillionDigits
		protected internal virtual void GenerateQuadrillionDigits(DigitInfoCollection digits, long value) {
			FlagQuadrillion = true;
			GenerateDigitsInfo(digits, value);
			GenerateQuadrillionSeparator(digits, GenerateQuadrillionProvider(), value);
			digits.Add(new QuadrillionDigitInfo(GenerateQuadrillionProvider(), 0));
			FlagQuadrillion = false;
		}
		#endregion
		#region GenerateTrillionSeparator
		protected internal virtual void GenerateTrillionSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count != 0)
				digits.Add(new SeparatorDigitInfo(provider, 0));
		}
		#endregion
		#region GenerateTrillionProvider
		protected internal virtual INumericsProvider GenerateTrillionProvider() {
			return new CardinalEnglishNumericsProvider();
		}
		#endregion
		#region GenerateTrillionDigits
		protected internal virtual void GenerateTrillionDigits(DigitInfoCollection digits, long value) {
			FlagTrillion = true;
			GenerateDigitsInfo(digits, value);
			GenerateTrillionSeparator(digits, GenerateTrillionProvider(), value);
			digits.Add(new TrillionDigitInfo(GenerateTrillionProvider(), 0));
			FlagTrillion = false;
		}
		#endregion
		#region GenerateBillionSeparator
		protected internal virtual void GenerateBillionSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count != 0)
				digits.Add(new SeparatorDigitInfo(provider, 0));
		}
		#endregion
		#region GenerateBillionProvider
		protected internal virtual INumericsProvider GenerateBillionProvider() {
			return new CardinalEnglishNumericsProvider();
		}
		#endregion
		#region GenerateBillionDigits
		protected internal virtual void GenerateBillionDigits(DigitInfoCollection digits, long value) {
			FlagBillion = true;
			GenerateDigitsInfo(digits, value);
			GenerateBillionSeparator(digits, GenerateBillionProvider(), value);
			digits.Add(new BillionDigitInfo(GenerateBillionProvider(), 0));
			FlagBillion = false;
		}
		#endregion
		#region GenerateMillionSeparator
		protected internal virtual void GenerateMillionSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count != 0)
				digits.Add(new SeparatorDigitInfo(provider, 0));
		}
		#endregion
		#region GenerateMillionProvider
		protected internal virtual INumericsProvider GenerateMillionProvider() {
			return new CardinalEnglishNumericsProvider();
		}
		#endregion
		#region GenerateMillionDigits
		protected internal virtual void GenerateMillionDigits(DigitInfoCollection digits, long value) {
			FlagMillion = true;
			GenerateDigitsInfo(digits, value);
			GenerateMillionSeparator(digits, GenerateMillionProvider(), value);
			digits.Add(new MillionDigitInfo(GenerateMillionProvider(), 0));
			FlagMillion = false;
		}
		#endregion
		#region GenerateThousandSeparator
		protected internal virtual void GenerateThousandSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count != 0)
				digits.Add(new SeparatorDigitInfo(provider, 0));
		}
		#endregion
		#region GenerateThousandProvider
		protected internal virtual INumericsProvider GenerateThousandProvider() {
			return new CardinalEnglishNumericsProvider();
		}
		#endregion
		#region GenerateThousandDigits
		protected internal virtual void GenerateThousandDigits(DigitInfoCollection digits, long value) {
			FlagThousand = true;
			GenerateDigitsInfo(digits, value);
			GenerateThousandSeparator(digits, GenerateThousandProvider(), value);
			digits.Add(new ThousandsDigitInfo(GenerateThousandProvider(), 0));
			FlagThousand = false;
		}
		#endregion
		#region GenerateHundredSeparator
		protected internal virtual void GenerateHundredSeparator(DigitInfoCollection digits, INumericsProvider provider) {
			if (digits.Count != 0)
				digits.Add(new SeparatorDigitInfo(provider, 0));
		}
		#endregion
		#region GenerateHundredProvider
		protected internal virtual INumericsProvider GenerateHundredProvider() {
			return new CardinalEnglishNumericsProvider();
		}
		#endregion
		#region GenerateHundredDigits
		protected internal virtual void GenerateHundredDigits(DigitInfoCollection digits, long value) {
			GenerateHundredSeparator(digits, GenerateHundredProvider());
			digits.Add(new HundredsDigitInfo(GenerateHundredProvider(), value));
		}
		#endregion
		#region GenerateTenthsSeparator
		protected internal virtual void GenerateTenthsSeparator(DigitInfoCollection digits, INumericsProvider provider) {
			if (digits.Count != 0)
				digits.Add(new SeparatorDigitInfo(provider, 0));
		}
		#endregion
		#region GenerateTenthsProvider
		protected internal virtual INumericsProvider GenerateTenthsProvider() {
			return new CardinalEnglishNumericsProvider();
		}
		#endregion
		#region GenerateTenthsDigits
		protected internal virtual void GenerateTenthsDigits(DigitInfoCollection digits, long value) {
			GenerateTenthsSeparator(digits, GenerateTenthsProvider());
			digits.Add(new TenthsDigitInfo(GenerateTenthsProvider(), value / 10));
			GenerateSinglesDigits(digits, value % 10);
		}
		#endregion
		#region GenerateTeensSeparator
		protected internal virtual void GenerateTeensSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count != 0)
				digits.Add(new SeparatorDigitInfo(provider, 0));
		}
		#endregion
		#region GenerateTeensProvider
		protected internal virtual INumericsProvider GenerateTeensProvider() {
			return new CardinalEnglishNumericsProvider();
		}
		#endregion
		#region GenerateTeensDigits
		protected internal virtual void GenerateTeensDigits(DigitInfoCollection digits, long value) {
			GenerateTeensSeparator(digits, GenerateTeensProvider(), value);
			digits.Add(new TeensDigitInfo(GenerateTeensProvider(), value));
		}
		#endregion
		#region GenerateSinglesSeparator
		protected internal virtual void GenerateSinglesSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (digits.Count != 0) {
				if (digits.Last.Type == DigitType.Tenth)
					digits.Add(new SeparatorDigitInfo(provider, 1));
				else
					digits.Add(new SeparatorDigitInfo(provider, 0));
			}
		}
		#endregion
		#region GenerateSinglesProvider
		protected internal virtual INumericsProvider GenerateSinglesProvider() {
			return new CardinalEnglishNumericsProvider();
		}
		#endregion
		#region GenerateSinglesDigits
		protected internal virtual void GenerateSinglesDigits(DigitInfoCollection digits, long value) {
			if (value == 0)
				return;
			GenerateSinglesSeparator(digits, GenerateSinglesProvider(), value);
			if (FlagThousand)
				digits.Add(new SingleNumeralDigitInfo(GenerateSinglesProvider(), value));
			else
				digits.Add(new SingleDigitInfo(GenerateSinglesProvider(), value));
			FlagThousand = false;
		}
		#endregion
		#region GenerateDigitsInfo
		protected internal virtual void GenerateDigitsInfo(DigitInfoCollection digits, long value) {
			if (value / 1000000000000000000 != 0)
				GenerateQuintillionDigits(digits, value / 1000000000000000000);
			value %= 1000000000000000000;
			if (value / 1000000000000000 != 0)
				GenerateQuadrillionDigits(digits, value / 1000000000000000);
			value %= 1000000000000000;
			if (value / 1000000000000 != 0)
				GenerateTrillionDigits(digits, value / 1000000000000);
			value %= 1000000000000;
			if (value / 1000000000 != 0)
				GenerateBillionDigits(digits, value / 1000000000);
			value %= 1000000000;
			if (value / 1000000 != 0)
				GenerateMillionDigits(digits, value / 1000000);
			value %= 1000000;
			if (value / 1000 != 0)
				GenerateThousandDigits(digits, value / 1000);
			value %= 1000;
			if (value / 100 != 0)
				GenerateHundredDigits(digits, value / 100);
			value %= 100;
			if (value == 0)
				return;
			if (value >= 20)
				GenerateTenthsDigits(digits, value);
			else {
				if (value >= 10)
					GenerateTeensDigits(digits, value % 10);
				else
					GenerateSinglesDigits(digits, value);
			}
		}
		#endregion
		#region GenerateDigits
		protected internal virtual void GenerateDigits(DigitInfoCollection digits, long value) {
			GenerateDigitsInfo(digits, value);
			if (digits.Count == 0)
				AddZero(digits);
		}
		#endregion
		#region AddZero
		protected internal virtual void AddZero(DigitInfoCollection digits) {
			digits.Add(new ZeroDigitInfo(GenerateSinglesProvider()));
		}
		#endregion
		#region ConvertNumber
		public override string ConvertNumberCore(long value) {
			DigitInfoCollection digits = new DigitInfoCollection();
			GenerateDigits(digits, value);
			return ConvertDigitsToString(digits);
		}
		#endregion
		protected internal virtual string ConvertDigitsToString(DigitInfoCollection digits) {
			StringBuilder result = new StringBuilder();
			int count = digits.Count;
			for (int i = 0; i < count; i++)
				result.Append(digits[i].ConvertToString());
			if (result.Length > 0)
				result[0] = Char.ToUpper(result[0]);
			return result.ToString();
		}
		protected internal bool IsDigitInfoGreaterHundred(DigitInfo info) {
			return info.Type == DigitType.Thousand || IsDigitInfoGreaterThousand(info);
		}
		protected internal bool IsDigitInfoGreaterThousand(DigitInfo info) {
			DigitType type = info.Type;
			return type == DigitType.Million || type == DigitType.Billion ||
				type == DigitType.Trillion || type == DigitType.Quadrillion || type == DigitType.Quintillion;
		}
	}
	#endregion
}
