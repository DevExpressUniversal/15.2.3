#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Text;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	public class NumericFormatUnitItem {
		readonly DataItemNumericUnit unit;
		public DataItemNumericUnit Unit { get { return unit; } }
		public NumericFormatUnitItem(DataItemNumericUnit unit) {
			this.unit = unit;
		}
		public override string ToString() {
			switch(unit) {
				case DataItemNumericUnit.Auto:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.NumericFormatUnitAutoCaption);
				case DataItemNumericUnit.Ones:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.NumericFormatUnitOnesCaption);
				case DataItemNumericUnit.Thousands:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.NumericFormatUnitThousandsCaption);
				case DataItemNumericUnit.Millions:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.NumericFormatUnitMillionsCaption);
				case DataItemNumericUnit.Billions:
					return DashboardWinLocalizer.GetString(DashboardWinStringId.NumericFormatUnitBillionsCaption);
				default:
					throw new Exception(Helper.GetUnknownEnumValueMessage(unit));
			}
		}
		public override bool Equals(object obj) {
			NumericFormatUnitItem target = obj as NumericFormatUnitItem;
			return target != null && unit == target.unit;
		}
		public override int GetHashCode() {
			return unit.GetHashCode();
		}
	}
}
