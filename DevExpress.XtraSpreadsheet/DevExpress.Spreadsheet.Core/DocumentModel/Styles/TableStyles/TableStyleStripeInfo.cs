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

using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region TableStyleStripeInfo
	public struct TableStyleStripeInfo {
		#region Fields
		const byte MaskIsFirstStripe = 1;
		const byte MaskIsFirstOutline = 2;
		const byte MaskIsLastOutline = 4;
		readonly byte packedValues;
		#endregion
		public TableStyleStripeInfo(int relativeIndex, int firstStripeSize, int stripeCount, bool isLastIndex) {
			int stripeIndex = relativeIndex % stripeCount;
			bool isFirstStripe = stripeIndex < firstStripeSize;
			bool isFirstOutline = isFirstStripe ? stripeIndex == 0 : stripeIndex == firstStripeSize;
			bool isLastOutline = isLastIndex || (isFirstStripe ? stripeIndex == firstStripeSize - 1 : stripeIndex == stripeCount - 1);
			byte packedValues = 0;
			PackedValues.SetBoolBitValue(ref packedValues, MaskIsFirstStripe, isFirstStripe);
			PackedValues.SetBoolBitValue(ref packedValues, MaskIsFirstOutline, isFirstOutline);
			PackedValues.SetBoolBitValue(ref packedValues, MaskIsLastOutline, isLastOutline);
			this.packedValues = packedValues;
		}
		public TableStyleStripeInfo(bool isFirstStripe, bool isFirstOutline, bool isLastOutline) {
			byte packedValues = 0;
			PackedValues.SetBoolBitValue(ref packedValues, MaskIsFirstStripe, isFirstStripe);
			PackedValues.SetBoolBitValue(ref packedValues, MaskIsFirstOutline, isFirstOutline);
			PackedValues.SetBoolBitValue(ref packedValues, MaskIsLastOutline, isLastOutline);
			this.packedValues = packedValues;
		}
		public bool IsFirstStripe { get { return PackedValues.GetBoolBitValue(packedValues, MaskIsFirstStripe); } }
		public bool IsFirstOutline { get { return PackedValues.GetBoolBitValue(packedValues, MaskIsFirstOutline); } }
		public bool IsLastOutline { get { return PackedValues.GetBoolBitValue(packedValues, MaskIsLastOutline); } }
	}
	#endregion
} 
