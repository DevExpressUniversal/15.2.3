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

using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Localization;
using System;
using System.Collections;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Design {
	public partial class HolidaysCollectionForm : XtraForm {
		HolidaysCollectionForm() {
			InitializeComponent();
		}
		public HolidaysCollectionForm(KnownDateCollection collection, IChartContainer chartContainer, bool isHolidays)
			: this(new KnownDateCollectionAccessor(collection), chartContainer, isHolidays) {
		}
		public HolidaysCollectionForm(IKnownDateCollectionAccessor collectionAccessor, IChartContainer chartContainer, bool isHolidays) : this() {
			holidaysEditControl.SetValue(collectionAccessor, chartContainer, isHolidays);
			Text = ChartLocalizer.GetString(isHolidays ? ChartStringId.Holidays : ChartStringId.ExactWorkdays);
		}
	}
	public class KnownDateCollectionAccessor : IKnownDateCollectionAccessor {
		readonly KnownDateCollection collection;
		public KnownDateCollectionAccessor(KnownDateCollection collection) {
			this.collection = collection;
		}
		#region IKnownDateCollectionAccessor Members
		int IKnownDateCollectionAccessor.Count { get { return collection.Count; } }
		void IKnownDateCollectionAccessor.Add(KnownDate item) {
			collection.Add(item);
		}
		void IKnownDateCollectionAccessor.Remove(KnownDate item) {
			collection.Remove(item);
		}
		void IKnownDateCollectionAccessor.AddRange(KnownDate[] items) {
			collection.AddRange(items);
		}
		void IKnownDateCollectionAccessor.Clear() {
			collection.Clear();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return collection.GetEnumerator();
		}
		#endregion
	}
}
