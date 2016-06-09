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
using DevExpress.Utils.Internal;
namespace DevExpress.Office.Model {
	#region PredefinedFontSizeCollection
	public class PredefinedFontSizeCollection : List<int> {
		public const int MinFontSize = 1;
		public const int MaxFontSize = 1500;
		public PredefinedFontSizeCollection() {
			CreateDefaultContent();
		}
		public static int ValidateFontSize(int value) {
			return Math.Max(Math.Min(value, MaxFontSize), MinFontSize);
		}
		protected internal virtual void CreateDefaultContent() {			
			foreach(int fontSize in FontManager.GetPredefinedFontSizes()) {
				Add(fontSize);
			}
		}
		public int CalculateNextFontSize(int fontSize) {
			return ValidateFontSize(CalculateNextFontSizeCore(fontSize));
		}
		protected internal virtual int CalculateNextFontSizeCore(int fontSize) {
			if (this.Count == 0)
				return fontSize + 1;
			if (fontSize < this[0])
				return fontSize + 1;
			int fontSizeIndex = this.BinarySearch(fontSize);
			if (fontSizeIndex < 0)
				fontSizeIndex = ~fontSizeIndex;
			else
				fontSizeIndex++;
			if (fontSizeIndex < this.Count)
				return this[fontSizeIndex];
			else
				return CalcNextTen(fontSize); 
		}
		public int CalculatePreviousFontSize(int fontSize) {
			return ValidateFontSize(CalculatePreviousFontSizeCore(fontSize));
		}
		protected internal virtual int CalculatePreviousFontSizeCore(int fontSize) {
			if (this.Count == 0)
				return fontSize - 1;
			if (fontSize <= this[0])
				return fontSize - 1;
			int fontSizeIndex = this.BinarySearch(fontSize);
			if (fontSizeIndex >= 0)
				return this[fontSizeIndex - 1];
			if (fontSizeIndex != ~Count)
				return this[(~fontSizeIndex) - 1];
			if (fontSize > CalcNextTen(this[Count - 1]))
				return CalcPrevTen(fontSize);
			else
				return this[(~fontSizeIndex) - 1];
		}
		protected internal virtual int CalcNextTen(int value) {
			return value + (10 - value % 10);
		}
		protected internal virtual int CalcPrevTen(int value) {
			if (value % 10 != 0)
				return value - (value % 10);
			else
				return value - 10;
		}
	}
	#endregion
}
