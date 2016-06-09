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

using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
using System;
namespace DevExpress.XtraScheduler.Native {
	public class SchedulerViewSelection : ICloneable {
		TimeInterval interval = TimeInterval.Empty;
		TimeInterval firstSelectedInterval = TimeInterval.Empty;
		Resource resource = ResourceBase.Empty;
		public TimeInterval Interval { get { return interval; } set { interval = value; } }
		public TimeInterval FirstSelectedInterval { get { return firstSelectedInterval; } set { firstSelectedInterval = value; } }
		public Resource Resource { get { return resource; } set { resource = value; } }
		public bool Forward {
			get {
				XtraSchedulerDebug.Assert(Interval.Contains(FirstSelectedInterval));
				return firstSelectedInterval.Start <= interval.Start;
			}
		}
		#region ICloneable implementation
		object ICloneable.Clone() {
			return CloneCore();
		}
		SchedulerViewSelection CloneCore() {
			SchedulerViewSelection clone = new SchedulerViewSelection();
			clone.Interval = this.Interval.Clone();
			clone.Resource = this.Resource;
			clone.FirstSelectedInterval = this.FirstSelectedInterval.Clone();
			return clone;
		}
		public SchedulerViewSelection Clone() {
			return CloneCore();
		}
		#endregion
		public bool IsEqual(SchedulerViewSelection selection) {
			return selection.interval.Equals(interval) && selection.firstSelectedInterval.Equals(this.firstSelectedInterval) &&
				selection.resource.Id.Equals(resource.Id);
		}
	}
}
