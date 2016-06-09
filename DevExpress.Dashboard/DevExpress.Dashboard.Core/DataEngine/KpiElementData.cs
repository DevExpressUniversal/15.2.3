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

using System.Collections;
using System.Collections.Generic;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.Data {
	public abstract class KpiElementData : ISelectionData {
		readonly decimal value;
		readonly string valueText;
		readonly bool singleValue;
		readonly string title;
		readonly IList selectionValues;
		readonly IList<string> selectionCaptions;
		readonly IndicatorType indicatorType;
		readonly bool isGood;
		public string Title { get { return title; } }
		public decimal Value { get { return value; } }
		public string ValueText { get { return valueText; } }
		public bool SingleValue { get { return singleValue; } }
		public IList SelectionValues { get { return selectionValues; } }
		public IList<string> SelectionCaptions { get { return selectionCaptions; } }
		public IndicatorType IndicatorType { get { return indicatorType; } }
		public bool IsGood { get { return isGood; } }
		IList ISelectionData.Values { get { return selectionValues; } }
		IList<string> ISelectionData.Captions { get { return selectionCaptions; } }
		protected KpiElementData(string title, SelectionData selectionData, DeltaValues deltaValues) {
			this.title = title;
			value = deltaValues.Value;
			valueText = deltaValues.ValueText;
			singleValue = deltaValues.SingleValue;
			if (selectionData != null) {
				selectionValues = selectionData.Values;
				selectionCaptions = selectionData.Captions;
			}
			indicatorType = deltaValues.IndicatorType;
			isGood = deltaValues.IsGood;
		}
	}
}
