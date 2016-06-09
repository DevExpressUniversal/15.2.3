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
namespace DevExpress.XtraSpreadsheet.Model {
	#region TableStyleElementInfoCache
	public class TableStyleElementInfoCache {
		#region Fields
		readonly Dictionary<byte, int> formatIndexTable;
		readonly bool[] hasFormattingTable;
		int firstRowStripeSize;
		int secondRowStripeSize;
		int firstColumnStripeSize;
		int secondColumnStripeSize;
		bool isValid;
		#endregion
		public TableStyleElementInfoCache() { 
			formatIndexTable = new Dictionary<byte, int>();
			hasFormattingTable = new bool[TableStyle.ElementsCount];
			firstRowStripeSize = StripeSizeInfo.DefaultValue;
			secondRowStripeSize = StripeSizeInfo.DefaultValue;
			firstColumnStripeSize = StripeSizeInfo.DefaultValue;
			secondColumnStripeSize = StripeSizeInfo.DefaultValue;
		}
		#region Properties
		public bool IsValid { get { return isValid; } }
		public int FirstRowStripeSize { get { return firstRowStripeSize; } }
		public int SecondRowStripeSize { get { return secondRowStripeSize; } }
		public int FirstColumnStripeSize { get { return firstColumnStripeSize; } }
		public int SecondColumnStripeSize { get { return secondColumnStripeSize; } }
		public bool IsEmpty { get { return formatIndexTable.Count == 0; } }
		#endregion
		#region Invalid event
		EventHandler onInvalid;
		public event EventHandler OnInvalid { add { onInvalid += value; } remove { onInvalid -= value; } }
		protected internal virtual void RaiseInvalid() {
			if (onInvalid != null)
				onInvalid(this, EventArgs.Empty);
		}
		#endregion
		public void Prepare(TableStyle style) {
			secondRowStripeSize = style.GetElementFormat(TableStyle.SecondRowStripeIndex).StripeSize;
			firstRowStripeSize = style.GetElementFormat(TableStyle.FirstRowStripeIndex).StripeSize;
			secondColumnStripeSize = style.GetElementFormat(TableStyle.SecondColumnStripeIndex).StripeSize;
			firstColumnStripeSize = style.GetElementFormat(TableStyle.FirstColumnStripeIndex).StripeSize;
			for (byte elementIndex = 0; elementIndex < TableStyle.ElementsCount; elementIndex++) {
				bool hasFormatting = style.HasTableStyleElementFormatting(elementIndex);
				if (hasFormatting) {
					TableStyleElementFormat format = style.GetElementFormat(elementIndex);
					if (!format.HasDefaultDifferentialFormatting) {
						formatIndexTable.Add(elementIndex, format.DifferentialFormatIndex);
						hasFormattingTable[elementIndex] = true;
					}
					else hasFormattingTable[elementIndex] = false;
				}
				else hasFormattingTable[elementIndex] = false;
			}
			isValid = true;
		}
		public bool HasDifferentialFormatting(byte elementIndex) {
			return hasFormattingTable[elementIndex];
		}
		public int GetDifferentialFormatIndex(byte elementIndex) {
			return formatIndexTable[elementIndex];
		}
		public void SetInvalid() {
			if (isValid) {
				formatIndexTable.Clear();
				firstRowStripeSize = StripeSizeInfo.DefaultValue;
				secondRowStripeSize = StripeSizeInfo.DefaultValue;
				firstColumnStripeSize = StripeSizeInfo.DefaultValue;
				secondColumnStripeSize = StripeSizeInfo.DefaultValue;
				isValid = false;
				RaiseInvalid();
			}
		}
	}
	#endregion
} 
