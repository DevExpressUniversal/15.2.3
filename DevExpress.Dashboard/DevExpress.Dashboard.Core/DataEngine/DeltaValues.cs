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
namespace DevExpress.DashboardCommon.Native {
	public class DeltaValues {
		readonly bool singleValue;
		public bool SingleValue { get { return singleValue; } }
		public decimal Value { get; set; }
		public string ValueText { get; set; }
		public bool IsGood { get; set; }
		public IndicatorType IndicatorType { get; set; }
		public decimal SubValue1 { get; set; }
		public decimal SubValue2 { get; set; }
		public string SubValue1Text { get; set; }
		public string SubValue2Text { get; set; }
		public double ActualValue { get; set; }
		public double TargetValue { get; set; }
		public DeltaValues() {
		}
		public DeltaValues(decimal value) {
			Value = value;
			ActualValue = Convert.ToDouble(value);
			singleValue = true;
		}
		public DeltaValues(decimal value, string valueText)
			: this(value) {
			ValueText = valueText;
		}
		public override string ToString() {
			string indicator;
			switch (IndicatorType) {
				case IndicatorType.UpArrow:
					indicator = "^";
					break;
				case IndicatorType.DownArrow:
					indicator = "v";
					break;
				case IndicatorType.Warning:
					indicator = "o";
					break;
				default:
					indicator = String.Empty;
					break;
			}
			return String.Format("{0}\t{1}", indicator, Value);
		}
	}
}
