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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreXtraPivotGrid = DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Events;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.XtraPivotGrid;
namespace DevExpress.Xpf.PivotGrid {
	public delegate void AsyncCompletedHandler(AsyncOperationResult result);
	public class AsyncOperationResult {
		CoreXtraPivotGrid.AsyncOperationResult result;
		internal AsyncOperationResult(CoreXtraPivotGrid.AsyncOperationResult result) {
			if(result == null)
				throw new ArgumentException("AsyncOperationResult");
			this.result = result;
		}		
		public virtual object Value { 
			get { 
				return result.Value; 
			}
		}
		public Exception Exception { get { return result.Exception; } }
	}
	class AsyncDrillDownResult : AsyncOperationResult {
		PivotDrillDownDataSource value;
		internal AsyncDrillDownResult(PivotGridWpfData data, CoreXtraPivotGrid.AsyncOperationResult result) : base(result) {
			this.value = (Exception == null && data != null) ? data.CreateDrillDownDataSourceWrapper((CoreXtraPivotGrid.PivotDrillDownDataSource)(base.Value)) : null;
		}
		public override object Value {
			get {
				return value;
			}
		}
	}
}
namespace DevExpress.Xpf.PivotGrid.Internal {
	static class AsyncModeHelper {
		public static CoreXtraPivotGrid.AsyncCompletedHandler ToCoreAsyncCompleted(this AsyncCompletedHandler completed) {
			return delegate(CoreXtraPivotGrid.AsyncOperationResult result) {
				if(completed != null)
					completed.Invoke(new AsyncOperationResult(result));
			};
		}
		public static void DoEmptyComplete(AsyncOperationResult result) {
		}
	}
	class PivotGridEventRaiser : PivotGridEventRaiserBase, IPivotGridEventsImplementor {
		int syncFieldRecording = 0;
		public PivotGridEventRaiser(IPivotGridEventsImplementorBase eventsImplementor)
			: base(eventsImplementor) {
		}
		public bool IsInrecording {
			get { return IsInRecording; }
		}
		public void StartRecordingInFieldSync() {
			syncFieldRecording++;
		}
		public void FinishRecordingInFieldSync() {
			if(syncFieldRecording > 0)
				syncFieldRecording--;
			if(IsSyncFieldRecording || IsInRecording)
				return;
			FinishRecordingAndRaiseEvents();
		}
		protected bool IsSyncFieldRecording {
			get { return syncFieldRecording != 0; }
		}
		protected new IPivotGridEventsImplementor BaseImpl {
			get { return (IPivotGridEventsImplementor)base.BaseImpl; }
		}
		#region IPivotGridEventsImplementor Members
		#region sync field recording
		void IPivotGridEventsImplementorBase.FieldVisibleChanged(PivotGridFieldBase field) {
			if(IsInRecording || IsSyncFieldRecording)
				AddEventRecord(FieldVisibleChangedEntryPoint, field);
			else
				BaseImpl.FieldVisibleChanged(field);
		}
		void IPivotGridEventsImplementorBase.FieldAreaChanged(PivotGridFieldBase field) {
			if(IsInRecording || IsSyncFieldRecording)
				AddEventRecord(FieldAreaChangedEntryPoint, field);
			else
				BaseImpl.FieldAreaChanged(field);
		}
		#endregion
		void IPivotGridEventsImplementor.CellSelectionChanged() {
			if(!IsInRecording)
				BaseImpl.CellSelectionChanged();
		}
		void IPivotGridEventsImplementor.FocusedCellChanged() {
			if(!IsInRecording)
				BaseImpl.FocusedCellChanged();
		}
		void IPivotGridEventsImplementor.AsyncOperationStarting() {
			EnsureIsNotRecording();
			BaseImpl.AsyncOperationStarting();
		}
		void IPivotGridEventsImplementor.AsyncOperationCompleted() {
			BaseImpl.AsyncOperationCompleted();
		}
		#endregion
	}
}
