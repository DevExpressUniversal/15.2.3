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
namespace DevExpress.DashboardCommon.Native.PerfomanceMonitor {
	public class PfEventFake : IPfEvent {
		int IPfEvent.ThreadId { get { throw new NotSupportedException(); } }
		Guid IPfEvent.Id { get { throw new NotSupportedException(); } }
		Guid? IPfEvent.ParentId { get { throw new NotSupportedException(); } }
		int IPfEvent.StepNumber { get { throw new NotSupportedException(); } }
		int IPfEvent.Level { get { throw new NotSupportedException(); } }
		string IPfEvent.Name { get { throw new NotSupportedException(); } }
		string IPfEvent.FullName { get { throw new NotSupportedException(); } }
		DateTime IPfEvent.StartTime { get { throw new NotSupportedException(); } }
		long IPfEvent.Duration { get { throw new NotSupportedException(); } }
		int IPfEvent.GC0Count { get { throw new NotSupportedException(); } }
		int IPfEvent.GC1Count { get { throw new NotSupportedException(); } }
		int IPfEvent.GC2Count { get { throw new NotSupportedException(); } }
		long IPfEvent.GCMemory { get { throw new NotSupportedException(); } }
		int IPfEvent.GC0CountTotal { get { throw new NotSupportedException(); } }
		int IPfEvent.GC1CountTotal { get { throw new NotSupportedException(); } }
		int IPfEvent.GC2CountTotal { get { throw new NotSupportedException(); } }
		long IPfEvent.GCMemoryTotal { get { throw new NotSupportedException(); } }
		void IDisposable.Dispose() {
		}
		void IPfEvent.WalkTree(Action<IPfEvent> action) {
			throw new NotSupportedException();
		}
	}
}
