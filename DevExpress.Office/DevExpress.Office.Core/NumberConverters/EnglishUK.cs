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
namespace DevExpress.Office.NumberConverters {
	#region CardinalEnglishUKNumericsProvider
	public class EnglishUKOptionalNumericsProvider : INumericsProvider {
		static string[] separator = { " and " };
		public string[] Separator { get { return separator; } }
		public string[] SinglesNumeral { get { return null; } }
		public string[] Singles { get { return null; } }
		public string[] Teens { get { return null; } }
		public string[] Tenths { get { return null; } }
		public string[] Hundreds { get { return null; } }
		public string[] Thousands { get { return null; } }
		public string[] Million { get { return null; } }
		public string[] Billion { get { return null; } }
		public string[] Trillion { get { return null; } }
		public string[] Quadrillion { get { return null; } }
		public string[] Quintillion { get { return null; } }
	}
	#endregion
	#region DescriptiveEnglishUKNumberConverterBase (abstract class)
	public abstract class DescriptiveEnglishUKNumberConverterBase : DescriptiveNumberConverterBase {
		protected internal override void GenerateTenthsSeparator(DigitInfoCollection digits, INumericsProvider provider) {
			if (ShouldAddCustomSeparator(digits))
				GenerateCustomSeparator(digits);
			else
				base.GenerateTenthsSeparator(digits, provider);
		}
		protected internal override void GenerateTeensSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (ShouldAddCustomSeparator(digits))
				GenerateCustomSeparator(digits);
			else
				base.GenerateTeensSeparator(digits, provider, value);
		}
		protected internal override void GenerateSinglesSeparator(DigitInfoCollection digits, INumericsProvider provider, long value) {
			if (ShouldAddCustomSeparator(digits))
				GenerateCustomSeparator(digits);
			else
				base.GenerateSinglesSeparator(digits, provider, value);
		}
		bool ShouldAddCustomSeparator(DigitInfoCollection digits) {
			if (digits.Count == 0)
				return false;
			DigitType last = digits.Last.Type;
			return last == DigitType.Hundred || (!IsValueGreaterHundred && last != DigitType.Tenth);
		}
		void GenerateCustomSeparator(DigitInfoCollection digits) {
			INumericsProvider provider = new EnglishUKOptionalNumericsProvider();
			digits.Add(new SeparatorDigitInfo(provider, 0));
		}
	}
	#endregion
	#region DescriptiveCardinalEnglishUKNumberConverter
	public class DescriptiveCardinalEnglishUKNumberConverter : DescriptiveEnglishUKNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.CardinalText; } }
	}
	#endregion
	#region DescriptiveOrdinalEnglishUKNumberConverter
	public class DescriptiveOrdinalEnglishUKNumberConverter : DescriptiveEnglishUKNumberConverterBase {
		protected internal override NumberingFormat Type { get { return NumberingFormat.OrdinalText; } }
		protected internal override void GenerateDigits(DigitInfoCollection digits, long value) {
			base.GenerateDigits(digits, value);
			digits.Last.Provider = new OrdinalEnglishNumericsProvider();
		}
	}
	#endregion
}
