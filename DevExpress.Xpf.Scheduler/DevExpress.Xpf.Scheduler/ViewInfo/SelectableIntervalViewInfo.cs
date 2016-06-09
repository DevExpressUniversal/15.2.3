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

using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler;
using System;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public interface ISupportHitTest {
		ISelectableIntervalViewInfo GetSelectableIntervalViewInfo(SchedulerControl control);
	}
	public class VisualSelectableIntervalViewInfo : ISelectableIntervalViewInfo {
		SchedulerHitTest hitTest;
		TimeInterval interval;
		Resource resource;
		bool selected;
		public VisualSelectableIntervalViewInfo(SchedulerHitTest hitTest, TimeInterval interval, Resource resource, bool selected) {
			this.hitTest = hitTest;
			this.interval = interval;
			this.resource = resource;
			this.selected = selected;
		}
		#region ISelectableIntervalViewInfo Members
		public SchedulerHitTest HitTestType { get { return hitTest; } }
		public TimeInterval Interval { get { return interval; } }
		public Resource Resource { get { return resource; } }
		public bool Selected { get { return selected; } }
		#endregion
		public override bool Equals(object obj) {
			VisualSelectableIntervalViewInfo other = obj as VisualSelectableIntervalViewInfo;
			if (other == null)
				return false;
			return EqualsIgnoreSelection(other) && Selected == other.Selected;
		}
		public bool EqualsIgnoreSelection(VisualSelectableIntervalViewInfo other) {
			if(other == null)
				return false;
			return HitTestType == other.HitTestType &&
				Interval.Equals(other.Interval) &&
				Resource.Equals(other.Resource);
		}
		public static bool operator ==(VisualSelectableIntervalViewInfo info1, VisualSelectableIntervalViewInfo info2) {
			if (Object.ReferenceEquals(info1, info2))
				return true;
			if (Object.ReferenceEquals(info1, null) || Object.ReferenceEquals(info2, null))
				return false;
			return info1.Equals(info2);
		}
		public static bool operator !=(VisualSelectableIntervalViewInfo info1, VisualSelectableIntervalViewInfo info2) {
			return !(info1 == info2);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
