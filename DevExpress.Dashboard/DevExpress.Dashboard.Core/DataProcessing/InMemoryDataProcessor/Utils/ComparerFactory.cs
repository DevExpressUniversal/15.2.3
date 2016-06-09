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
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	static class ComparerFactory {
		#region custom comparers
		class ByteArrComparer : Comparer<byte[]> {
			public override int Compare(byte[] x, byte[] y) {
				int diff = x.Length - y.Length;
				if(diff == 0)
					for(int i = 0; diff == 0 && i < x.Length; i++)
						diff = x[i] - y[i];
				return diff;
			}
		}
		class DayOfWeekComparer : Comparer<DayOfWeek> {
			readonly DayOfWeek firstDayOfWeek;
			public DayOfWeekComparer() {
				this.firstDayOfWeek = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
			}
			public override int Compare(DayOfWeek x, DayOfWeek y) {
				return GetAbsDayOfWeek(x) - GetAbsDayOfWeek(y);
			}
			int GetAbsDayOfWeek(DayOfWeek val) {
				return val < firstDayOfWeek ? 7 + (int)val : (int)val;
			}
		}
		#endregion
		static ByteArrComparer byteArrComparer = new ByteArrComparer();
		static DayOfWeekComparer dayoOWeekComparer = new DayOfWeekComparer();
		public static Comparer<T> Get<T>() {
			if(typeof(T) == typeof(byte[]))
				return byteArrComparer as Comparer<T>;
			if(typeof(T) == typeof(DayOfWeek))
				return dayoOWeekComparer as Comparer<T>;
			return Comparer<T>.Default;
		}
	}
}
