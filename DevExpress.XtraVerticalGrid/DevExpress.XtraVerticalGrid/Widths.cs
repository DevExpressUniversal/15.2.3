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

using DevExpress.XtraVerticalGrid.Rows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraVerticalGrid.Internal {
	public class WidthsManager {
		VGridControlBase vGrid;
		WidthsBase simple;
		WidthsBase autoSize;
		WidthsBase fixedHeader;
		WidthsBase loading;
		WidthsBase oldWidths;
		public WidthsManager(VGridControlBase vGrid) {
			this.vGrid = vGrid;
			this.simple = new SimpleWidths(vGrid, this);
			this.autoSize = new AutoSizeWidths(vGrid, this);
			this.fixedHeader = new FixedHeaderWidths(vGrid, this);
			this.loading = new LoadingWidths(vGrid, this);
		}
		public int HeaderWidth { get { return GetWidths().HeaderWidth; } set { GetWidths().HeaderWidth = value; } }
		public int RecordWidth { get { return GetWidths().RecordWidth; } set { GetWidths().RecordWidth = value; } }
		VGridControlBase VGrid { get { return vGrid; } }
		public WidthsBase GetWidths() {
			WidthsBase newWidths = GetWidthsCore();
			newWidths.AcceptWidths(this.oldWidths);
			this.oldWidths = newWidths;
			return newWidths;
		}
		WidthsBase GetWidthsCore() {
			if (VGrid.IsLoading)
				return loading;
			if (VGrid.IsAutoSize && VGrid.OptionsView.FixRowHeaderPanelWidth)
				return fixedHeader;
			if (VGrid.IsAutoSize)
				return autoSize;
			return simple;
		}
	}
	public abstract class WidthsBase {
		protected const int DefaultWidth = 100;
		WidthsManager manager;
		VGridControlBase vGrid;
		protected int fHeaderWidth = DefaultWidth;
		protected int fRecordWidth = DefaultWidth;
		protected WidthsBase(VGridControlBase vGrid, WidthsManager manager) {
			this.vGrid = vGrid;
			this.manager = manager;
		}
		public virtual int HeaderWidth {
			get { return fHeaderWidth; }
			set {
				if (value < VGrid.RowHeaderMinWidth)
					value = VGrid.RowHeaderMinWidth;
				if (HeaderWidth == value)
					return;
				AssignWidths(value, true);
				OnChanged();
				VGrid.RaiseRowHeaderWidthChanged();
			}
		}
		void AssignHeaderWidth(int value) {
			int cx = value - fHeaderWidth;
			if (fRecordWidth - cx < VGrid.RecordMinWidth)
				cx = fRecordWidth - VGrid.RecordMinWidth;
			fHeaderWidth += cx;
			fRecordWidth -= cx;
		}
		void AssignRecordWidth(int value) {
			int cx = value - fRecordWidth;
			if (fHeaderWidth - cx < VGrid.RowHeaderMinWidth)
				cx = fHeaderWidth - VGrid.RowHeaderMinWidth;
			fRecordWidth += cx;
			fHeaderWidth -= cx;
		}
		public virtual int RecordWidth {
			get { return BandWidthCore - HeaderWidth; }
			set {
				if (value < VGrid.RecordMinWidth)
					value = VGrid.RecordMinWidth;
				if (RecordWidth == value)
					return;
				AssignWidths(value, false);
				OnChanged();
				VGrid.RaiseRecordWidthChanged();
			}
		}
		protected virtual void AssignWidths(int value, bool assignHeader) {
			if (assignHeader)
				AssignHeaderWidth(value);
			else
				AssignRecordWidth(value);
		}
		protected int ClientRectWidth { get { return VGrid.ViewInfo.ViewRects.Client.Width; } }
		protected VGridControlBase VGrid { get { return vGrid; } }
		void OnChanged() {
			VGrid.InvalidateUpdate();
			VGrid.FireChanged();
		}
		protected WidthsManager Manager { get { return manager; } }
		public abstract int ActualHeaderWidth();
		public abstract int ActualRecordWidth();
		public abstract void AcceptHeaderWidth(int actualHeaderWidth);
		public abstract void AcceptRecordWidth(int actualRecordWidth);
		public virtual int BandWidthCore {
			get { return 2 * DefaultWidth; }
		}
		public void AcceptWidths(WidthsBase oldWidths) {
			if (object.ReferenceEquals(oldWidths, this) || oldWidths == null)
				return;
			if (VGrid.OptionsView.FixRowHeaderPanelWidth) {
				AcceptHeaderWidth(oldWidths.ActualHeaderWidth());
				AcceptRecordWidth(oldWidths.ActualRecordWidth());
			}
			else {
				AssignWidths(oldWidths.fHeaderWidth, true);
				AssignWidths(oldWidths.fRecordWidth, false);
			}
		}
		protected int ToClient(int value) {
			return value * ClientRectWidth / BandWidthCore;
		}
		protected int ToFraction(int value) {
			return (int)Math.Round((float)value * (float)BandWidthCore / (float)ClientRectWidth);
		}
	}
	class SimpleWidths : WidthsBase {
		public SimpleWidths(VGridControlBase vGrid, WidthsManager manager) : base(vGrid, manager) { }
		public override int ActualHeaderWidth() {
			return HeaderWidth;
		}
		public override int ActualRecordWidth() {
			return RecordWidth;
		}
		public override void AcceptHeaderWidth(int actualHeaderWidth) {
			this.fHeaderWidth = actualHeaderWidth;
		}
		public override void AcceptRecordWidth(int actualRecordWidth) {
			this.fRecordWidth = actualRecordWidth;
		}
		protected override void AssignWidths(int value, bool assignHeader) {
			if (assignHeader) {
				this.fHeaderWidth = value;
			}
			else {
				this.fRecordWidth = value;
			}
		}
		public override int BandWidthCore {
			get { return fRecordWidth + fHeaderWidth; }
		}
	}
	class AutoSizeWidths : WidthsBase {
		public AutoSizeWidths(VGridControlBase vGrid, WidthsManager manager) : base(vGrid, manager) { }
		public override int ActualHeaderWidth() {
			return ToClient(this.fHeaderWidth);
		}
		public override int ActualRecordWidth() {
			return ToClient(RecordWidth);
		}
		public override void AcceptHeaderWidth(int actualHeaderWidth) {
			AssignWidths(actualHeaderWidth, true);
			if (ClientRectWidth != 0)
				AssignWidths(ToFraction(actualHeaderWidth), true);
		}
		public override void AcceptRecordWidth(int actualRecordWidth) {
			if (ClientRectWidth != 0)
				AssignWidths(ToFraction(actualRecordWidth), false);
		}
	}
	class FixedHeaderWidths : WidthsBase {
		public FixedHeaderWidths(VGridControlBase vGrid, WidthsManager manager) : base(vGrid, manager) { }
		public override int ActualHeaderWidth() {
			return this.fHeaderWidth;
		}
		public override int ActualRecordWidth() {
			return RecordWidth;
		}
		public override void AcceptHeaderWidth(int actualHeaderWidth) {
			this.fHeaderWidth = Math.Max(actualHeaderWidth, ToClient(VGrid.RowHeaderMinWidth));
		}
		public override void AcceptRecordWidth(int actualRecordWidth) { }
		public override int HeaderWidth {
			get {
				if (ClientRectWidth != 0)
					return ToFraction(this.fHeaderWidth);
				else
					return this.fHeaderWidth;
			}
		}
		protected override void AssignWidths(int value, bool assignHeader) {
			int maxRowHeaderWidth = BandWidthCore - VGrid.RecordMinWidth;
			if (!assignHeader) {
				int headerWidthValue = Math.Min(BandWidthCore - value, maxRowHeaderWidth);
				this.fHeaderWidth = ToClient(headerWidthValue);
			}
			else {
				this.fHeaderWidth = ToClient(Math.Min(value, maxRowHeaderWidth));
			}
		}
	}
	class LoadingWidths : WidthsBase {
		public LoadingWidths(VGridControlBase vGrid, WidthsManager manager) : base(vGrid, manager) { }
		public override int ActualHeaderWidth() {
			if (VGrid.IsAutoSize)
				return ToClient(HeaderWidth);
			else
				return HeaderWidth;
		}
		public override int ActualRecordWidth() {
			if (VGrid.IsAutoSize)
				return ToClient(RecordWidth);
			else
				return RecordWidth;
		}
		public override void AcceptHeaderWidth(int actualHeaderWidth) {
			this.fHeaderWidth = actualHeaderWidth;
		}
		public override void AcceptRecordWidth(int actualRecordWidth) {
			this.fRecordWidth = actualRecordWidth;
		}
		public override int RecordWidth {
			get { return fRecordWidth; }
		}
		protected override void AssignWidths(int value, bool assignHeader) {
			if (assignHeader) {
				this.fHeaderWidth = value;
			}
			else {
				this.fRecordWidth = value;
			}
		}
	}
	public class AutoHeightsStore {
		readonly Hashtable autoHeights = new Hashtable();
		public int? this[BaseRow row] { get { return autoHeights[row] as int?; } set { autoHeights[row] = value; } }
		public ICollection Keys { get { return autoHeights.Keys; } }
		public void Clear() {
			autoHeights.Clear();
		}
		public bool ContainsKey(BaseRow row) {
			return autoHeights.ContainsKey(row);
		}
		public void Remove(BaseRow row) {
			autoHeights.Remove(row);
		}
	}
}
