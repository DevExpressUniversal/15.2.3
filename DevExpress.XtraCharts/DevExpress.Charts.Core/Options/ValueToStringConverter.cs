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
namespace DevExpress.Charts.Native {
	public class ValueToStringConverter {
		readonly INumericOptions numericOptions;
		readonly IDateTimeOptions dateTimeOptions;
		protected INumericOptions NumericOptions { get { return numericOptions; } }
		public ValueToStringConverter(INumericOptions numericOptions, IDateTimeOptions dateTimeOptions) {
			this.numericOptions = numericOptions;
			this.dateTimeOptions = dateTimeOptions;
		}
		protected string GetValueText(object value) {
			if (value is double)
				return NumericOptionsHelper.GetValueText((double)value, numericOptions);
			if (value is DateTime)
				return DateTimeOptionsHelper.GetValueText((DateTime)value, dateTimeOptions);
			ChartDebug.Fail("Incorrect value type.");
			return value == null ? String.Empty : value.ToString();
		}
		protected virtual object GetValue(object[] values) {
			return values[0];
		}
		public virtual string ConvertTo(object[] values) {
			return GetValueText(GetValue(values));
		}
	}
}
