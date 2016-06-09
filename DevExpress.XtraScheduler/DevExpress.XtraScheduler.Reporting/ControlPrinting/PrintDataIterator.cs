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
using DevExpress.XtraScheduler.Reporting.Native;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Reporting.Native {
	#region PrintDataIteratorBase
	public abstract class PrintDataIteratorBase {
		PrintDataCache dataCache;
		protected PrintDataIteratorBase() {
		}
		protected internal PrintDataCache DataCache { get { return dataCache; } }
		public bool MoveNext(PrintDataCache dataCache) {
			SetDataCacheInternal(dataCache);
			XtraSchedulerDebug.Assert(DataCache != null);
			if (CanResetData()) {
				return ResetData();
			}
			if (CanKeepCurrentData()) {
				return KeepCurrentData();
			}
			if (CanMoveData()) {
				return MoveData();
			}
			return false;
		}
		protected internal virtual void SetDataCacheInternal(PrintDataCache dataCache) {
			if (dataCache == null)
				Exceptions.ThrowArgumentNullException("dataCache");
			this.dataCache = dataCache;
		}
		protected internal virtual bool CanResetData() {
			return DataCache.CanReset();
		}
		protected internal virtual bool CanKeepCurrentData() {
			return DataCache.CanKeepCurrent();
		}
		protected internal virtual bool CanMoveData() {
			return DataCache.CanMoveNext();
		}
		protected internal virtual bool ResetData() {
			DataCache.Reset();
			DataCache.FillData();
			return true;
		}
		protected internal abstract bool MoveData();
		protected internal virtual bool KeepCurrentData() {
			return true;
		}
	}
	#endregion
	#region PrintDataIterator
	public class PrintDataIterator : PrintDataIteratorBase {
		public PrintDataIterator() {
		}
		protected internal override bool MoveData() {
			DataCache.MoveNextData();
			DataCache.FillData();
			return true;
		}
	}
	#endregion
	#region DesignPrintDataIterator
	public class DesignPrintDataIterator : PrintDataIteratorBase {
		public DesignPrintDataIterator() {
		}
		protected internal override bool CanResetData() {
			return true;
		}
		protected internal override bool ResetData() {
			base.ResetData();
			DataCache.SetPrintState(ControlPrintState.KeepCurrent);
			return true;
		}
		protected internal override bool MoveData() {
			return false;
		}
	}
	#endregion
}
