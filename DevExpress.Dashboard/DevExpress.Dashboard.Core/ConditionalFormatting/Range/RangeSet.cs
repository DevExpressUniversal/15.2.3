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

using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.DataAccess;
using DevExpress.Utils.Design;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
#if !DXPORTABLE // TODO dnxcore
	[TypeConverter(typeof(UniversalTypeConverterEx))]
#endif
	public class RangeSet : NotifyingCollection<RangeInfo> {
		[Browsable(false)]
		public virtual bool HasRanges { get { return Count > 0; } }
		internal IEnumerable<StyleSettingsBase> ActualStyles {
			get {
				foreach(RangeInfo range in this) {
					yield return range.ActualStyleSettings;
				}
			}
		}
		public RangeSet() {
			XmlSerializer = new RangeInfoCollectionXmlSerializer();
		}
		public override string ToString() {
			return Count == 0 ? string.Empty : string.Format("(Count={0})", Count);
		}
		protected internal virtual void Assign(RangeSet obj) {
			Clear();
			foreach(RangeInfo rangeInfo in obj) 
				Add(rangeInfo.Clone());
		}
		protected virtual RangeSet CreateInstance() {
			return new RangeSet();
		}
		protected internal RangeSet SortRanges() {
			List<RangeInfo> ranges = new List<RangeInfo>(this);
			ranges.Sort((a, b) => {
				int c = ValueManager.CompareValues(a.Value, b.Value, true);
				if(c != 0) return c;
				if(a.ValueComparison == b.ValueComparison) return 0;
				if(a.ValueComparison == DashboardFormatConditionComparisonType.Greater) return 1;
				return -1;
			});
			RangeSet res = new RangeSet();
			res.AddRange(ranges);
			return res;
		}
		protected internal RangeSet Clone() {
			RangeSet obj = CreateInstance();
			obj.Assign(this);
			return obj;
		}
	}
}
