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
namespace DevExpress.XtraScheduler.Native {
	public interface IRangeControlClientDataProvider : IDisposable {
		DateTime SelectedRangeStart { get; }
		DateTime SelectedRangeEnd { get; }
		IRangeControlClientSyncSupport SyncSupport { get; set; }
		IScaleBasedRangeControlClientOptions GetOptions();
		List<DataItemThumbnailList> CreateThumbnailData(TimeIntervalCollection intervals);
		void OnSelectedRangeChanged(DateTime rangeMinimum, DateTime rangeMaximum);
		void OnOptionsChanged(string name, object oldValue, object newValue);
	}
	#region EmptyTimeClientDataProvider
	public class EmptyTimeClientDataProvider : IRangeControlClientDataProvider {
		public EmptyTimeClientDataProvider() {
			SelectedRangeStart = DateTime.Today;
			SelectedRangeEnd = DateTime.Today.AddDays(1);
		}
		public virtual DateTime SelectedRangeStart { get; set; }
		public virtual DateTime SelectedRangeEnd { get; set; }
		public IScaleBasedRangeControlClientOptions GetOptions() {
			return new ScaleBasedRangeControlClientOptionsEmpty();
		}
		void IRangeControlClientDataProvider.OnSelectedRangeChanged(DateTime rangeMinimum, DateTime rangeMaximum) { }
		void IRangeControlClientDataProvider.OnOptionsChanged(string name, object oldValue, object newValue) { }
		List<DataItemThumbnailList> IRangeControlClientDataProvider.CreateThumbnailData(TimeIntervalCollection intervals) {
			return new List<DataItemThumbnailList>();
		}
		IRangeControlClientSyncSupport IRangeControlClientDataProvider.SyncSupport { get { return null; } set { ; } }
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~EmptyTimeClientDataProvider() {
			Dispose(false);
		}
		#endregion
	}
	#endregion
	#region ScaleBasedRangeControlClientOptionsEmpty
	public class ScaleBasedRangeControlClientOptionsEmpty : IScaleBasedRangeControlClientOptions {
		TimeScaleCollection scales = new TimeScaleCollection();
		public TimeScaleCollection Scales { get { return scales; } }
		public bool AutoFormatScaleCaptions { get { return false; } set { } }
		public int MinIntervalWidth { get { return 0; } set { } }
		public int MaxIntervalWidth { get { return 0; } set { } }
		public int MaxSelectedIntervalCount { get { return 0; } set { } }
		public RangeControlDataDisplayType DataDisplayType { get { return RangeControlDataDisplayType.Auto; } set { } }
		public int ThumbnailHeight { get { return 0; } set { } }
		public DateTime RangeMinimum { get { return DateTime.MinValue; } set { } }
		public DateTime RangeMaximum { get { return DateTime.MinValue; } set { } }
		public event Utils.Controls.BaseOptionChangedEventHandler Changed;
		public void BeginUpdate() {
		}
		public void EndUpdate() {
		}
		protected virtual void RaiseChanged() {
			if (Changed == null)
				return;
			Changed(this, new Utils.Controls.BaseOptionChangedEventArgs());
		}
	}
	#endregion
}
