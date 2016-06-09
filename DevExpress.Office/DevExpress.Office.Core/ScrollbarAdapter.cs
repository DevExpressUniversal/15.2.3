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
using System.Drawing;
using DevExpress.Utils;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
using System.Diagnostics;
using DevExpress.Office.Utils;
namespace DevExpress.Office.Internal {
	#region IOfficeScrollbar
	public interface IOfficeScrollbar {
		int Value { get; set; }
		int Minimum { get; set; }
		int Maximum { get; set; }
		int LargeChange { get; set; }
		int SmallChange { get; set; }
		bool Enabled { get; set; }
		event ScrollEventHandler Scroll;
		void BeginUpdate();
		void EndUpdate();
	}
	#endregion
	#region IScrollBarAdapter
	public interface IScrollBarAdapter : IBatchUpdateable, IBatchUpdateHandler {
		void Activate();
		void Deactivate();
		void RefreshValuesFromScrollBar();
		bool SynchronizeScrollBarAvoidJump();
		void EnsureSynchronized();
		void ApplyValuesToScrollBar();
		ScrollEventArgs CreateEmulatedScrollEventArgs(ScrollEventType eventType);
		int GetRawScrollBarValue();
		bool SetRawScrollBarValue(int value);
		int GetPageUpRawScrollBarValue();
		int GetPageDownRawScrollBarValue();
		event ScrollEventHandler Scroll;
		long Minimum { get; set; }
		long Maximum { get; set; }
		long Value { get; set; }
		long LargeChange { get; set; }
		bool Enabled { get; set; }
	}
	#endregion
	public interface IPlatformSpecificScrollBarAdapter {
		void OnScroll(ScrollBarAdapter adapter, object sender, ScrollEventArgs e);
		void ApplyValuesToScrollBarCore(ScrollBarAdapter adapter);
		int GetRawScrollBarValue(ScrollBarAdapter adapter);
		bool SetRawScrollBarValue(ScrollBarAdapter adapter, int value);
		int GetPageUpRawScrollBarValue(ScrollBarAdapter adapter);
		int GetPageDownRawScrollBarValue(ScrollBarAdapter adapter);
		ScrollEventArgs CreateLastScrollEventArgs(ScrollBarAdapter adapter);
	}
	#region ScrollBarAdapter (abstract class)
	public abstract class ScrollBarAdapter : IScrollBarAdapter {
		#region Fields
		readonly IOfficeScrollbar scrollBar;
		readonly BatchUpdateHelper batchUpdateHelper;
		readonly IPlatformSpecificScrollBarAdapter adapter;
		double factor = 1.0;
		long minimum;
		long maximum;
		long val;
		long largeChange;
		bool enabled;
		#endregion
		protected ScrollBarAdapter(IOfficeScrollbar scrollBar, IPlatformSpecificScrollBarAdapter adapter) {
			Guard.ArgumentNotNull(scrollBar, "scrollBar");
			Guard.ArgumentNotNull(adapter, "adapter");
			this.scrollBar = scrollBar;
			this.adapter = adapter;
			this.batchUpdateHelper = new BatchUpdateHelper(this);
		}
		#region Properties
		public double Factor { get { return factor; } set { factor = value; } }
		protected internal abstract bool DeferredScrollBarUpdate { get; }
		protected internal abstract bool Synchronized { get; set; }
		#region Minimum
		public long Minimum {
			get { return minimum; }
			set {
				if (minimum.Equals(value))
					return;
				minimum = value;
				ValidateValues();
			}
		}
		#endregion
		#region Maximum
		public long Maximum {
			get { return maximum; }
			set {
				if (maximum.Equals(value))
					return;
				maximum = value;
				ValidateValues();
			}
		}
		#endregion
		#region Value
		public long Value {
			get { return val; }
			set {
				if (val.Equals(value))
					return;
				val = value;
				ValidateValues();
			}
		}
		#endregion
		#region LargeChange
		public long LargeChange {
			get { return largeChange; }
			set {
				if (largeChange.Equals(value))
					return;
				largeChange = value;
				ValidateValues();
			}
		}
		#endregion
		#region Enabled
		public bool Enabled {
			get { return enabled; }
			set { enabled = value; }
		}
		#endregion
		public IOfficeScrollbar ScrollBar { get { return scrollBar; } }
		protected internal IPlatformSpecificScrollBarAdapter Adapter { get { return adapter; } }
		#endregion
		#region Events
		#region Scroll
		ScrollEventHandler onScroll;
		public event ScrollEventHandler Scroll { add { onScroll += value; } remove { onScroll -= value; } }
		public virtual void RaiseScroll(ScrollEventArgs args) {
			if (onScroll != null)
				onScroll(ScrollBar, args);
		}
		#endregion
		#endregion
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			OnLastEndUpdateCore();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			OnLastEndUpdateCore();
		}
		#endregion
		public virtual void Activate() {
			RefreshValuesFromScrollBar();
			SubscribeScrollbarEvents();
		}
		public virtual void Deactivate() {
			UnsubscribeScrollbarEvents();
		}
		protected internal virtual void OnLastEndUpdateCore() {
			ValidateValues();
		}
		protected internal virtual void ValidateValues() {
			if (!IsUpdateLocked)
				ValidateValuesCore();
		}
		protected internal virtual void ValidateValuesCore() {
			if (LargeChange < 0)
				Exceptions.ThrowArgumentException("LargeChange", LargeChange);
			if (Minimum > Maximum)
				this.maximum = Minimum;
			this.largeChange = Math.Min(LargeChange, Maximum - Minimum + 1);
			this.val = Math.Max(Value, Minimum);
			this.val = Math.Min(Value, Maximum - LargeChange + 1);
		}
		protected internal virtual void SubscribeScrollbarEvents() {
			ScrollBar.Scroll += OnScroll;
		}
		protected internal virtual void UnsubscribeScrollbarEvents() {
			ScrollBar.Scroll -= OnScroll;
		}
		public virtual bool SynchronizeScrollBarAvoidJump() {
			if (ShouldSynchronize()) {
				double relativePos = Value / Math.Max(1f, (float)(Maximum - LargeChange + 1 - Minimum));
				double actualRelativePos = ScrollBar.Value / Math.Max(1.0d, ScrollBar.Maximum - ScrollBar.LargeChange + 1 - ScrollBar.Minimum);
				if (relativePos <= actualRelativePos) {
					ApplyValuesToScrollBarCore();
					System.Diagnostics.Debug.Assert(Synchronized);
					return true;
				}
			}
			return false;
		}
		public bool EnsureSynchronizedCore() {
			if (ShouldSynchronize()) {
				ApplyValuesToScrollBarCore();
				System.Diagnostics.Debug.Assert(Synchronized);
				return true;
			}
			System.Diagnostics.Debug.Assert(Synchronized);
			return false;
		}
		public virtual void EnsureSynchronized() {
			EnsureSynchronizedCore();
		}
		protected internal virtual bool ShouldSynchronize() {
			return DeferredScrollBarUpdate && !Synchronized;
		}
		protected internal virtual void OnScroll(object sender, ScrollEventArgs e) {
			Adapter.OnScroll(this, sender, e);
		}
		public virtual void ApplyValuesToScrollBar() {
			if (DeferredScrollBarUpdate)
				Synchronized = false;
			else
				ApplyValuesToScrollBarCore();
		}
		protected internal virtual void ApplyValuesToScrollBarCore() {
			Adapter.ApplyValuesToScrollBarCore(this);
			this.Synchronized = true;
		}
		public virtual void RefreshValuesFromScrollBar() {
			BeginUpdate();
			try {
				Minimum = (long)Math.Round(ScrollBar.Minimum / Factor);
				Maximum = (long)Math.Round(ScrollBar.Maximum / Factor);
				LargeChange = (long)Math.Round(ScrollBar.LargeChange / Factor);
				Value = (long)Math.Round(ScrollBar.Value / Factor);
				Enabled = ScrollBar.Enabled;
			}
			finally {
				EndUpdate();
			}
			Synchronized = true;
		}
		public virtual ScrollEventArgs CreateEmulatedScrollEventArgs(ScrollEventType eventType) {
			EnsureSynchronized();
			switch (eventType) {
				case ScrollEventType.First:
					return new ScrollEventArgs(eventType, ScrollBar.Minimum);
				case ScrollEventType.Last:
					return adapter.CreateLastScrollEventArgs(this);
				case ScrollEventType.SmallIncrement:
					return new ScrollEventArgs(eventType, ScrollBar.Value + ScrollBar.SmallChange); 
				case ScrollEventType.SmallDecrement:
					return new ScrollEventArgs(eventType, ScrollBar.Value - ScrollBar.SmallChange); 
				default:
					Exceptions.ThrowInternalException();
					return null;
			}
		}
		public int GetRawScrollBarValue() {
			EnsureSynchronized();
			return Adapter.GetRawScrollBarValue(this);
		}
		public bool SetRawScrollBarValue(int value) {
			System.Diagnostics.Debug.Assert(Synchronized);
			return Adapter.SetRawScrollBarValue(this, value);
		}
		public virtual int GetPageUpRawScrollBarValue() {
			EnsureSynchronized();
			return Adapter.GetPageUpRawScrollBarValue(this);
		}
		public virtual int GetPageDownRawScrollBarValue() {
			EnsureSynchronized();
			return Adapter.GetPageDownRawScrollBarValue(this);
		}
	}
	#endregion
	#region OfficeScrollControllerBase (abstract class)
	public abstract partial class OfficeScrollControllerBase {
		#region Fields
		IScrollBarAdapter scrollBarAdapter;
		#endregion
		protected OfficeScrollControllerBase() {
		}
		#region Properties
		protected internal virtual bool SupportsDeferredUpdate { get { return false; } }
		public virtual IScrollBarAdapter ScrollBarAdapter { get { return scrollBarAdapter; } }
		public abstract IOfficeScrollbar ScrollBar { get; }
		#endregion
		protected internal virtual void Initialize() {
			this.scrollBarAdapter = CreateScrollBarAdapter();
			ScrollBarAdapter.RefreshValuesFromScrollBar();
		}
		public virtual void Activate() {
			ScrollBarAdapter.Activate();
			SubscribeScrollBarAdapterEvents();
		}
		public virtual void Deactivate() {
			ScrollBarAdapter.Deactivate();
			UnsubscribeScrollBarAdapterEvents();
		}
		protected abstract ScrollBarAdapter CreateScrollBarAdapter();
		protected internal virtual void SubscribeScrollBarAdapterEvents() {
			ScrollBarAdapter.Scroll += new ScrollEventHandler(OnScroll);
		}
		protected internal virtual void UnsubscribeScrollBarAdapterEvents() {
			ScrollBarAdapter.Scroll -= new ScrollEventHandler(OnScroll);
		}
		protected internal void EmulateScroll(ScrollEventType eventType) {
			ScrollEventArgs args = ScrollBarAdapter.CreateEmulatedScrollEventArgs(eventType);
			OnScroll(ScrollBar, args);
		}
		protected abstract void OnScroll(object sender, ScrollEventArgs e);
		protected internal virtual void SynchronizeScrollbar() {
			UnsubscribeScrollBarAdapterEvents();
			try {
				ScrollBarAdapter.ApplyValuesToScrollBar();
			}
			finally {
				SubscribeScrollBarAdapterEvents();
			}
		}
		public virtual void UpdateScrollBar() {
			if (ScrollBar == null) 
				return;
			UpdateScrollBarAdapter();
			SynchronizeScrollbar();
		}
		public virtual bool IsScrollPossible() {
			return ScrollBarAdapter.Maximum - ScrollBarAdapter.Minimum > ScrollBarAdapter.LargeChange &&
				ScrollBarAdapter.Value >= ScrollBarAdapter.Minimum &&
				ScrollBarAdapter.Value <= ScrollBarAdapter.Maximum - ScrollBarAdapter.LargeChange + 1;
		}
		protected abstract void UpdateScrollBarAdapter();
	}
	#endregion
}
