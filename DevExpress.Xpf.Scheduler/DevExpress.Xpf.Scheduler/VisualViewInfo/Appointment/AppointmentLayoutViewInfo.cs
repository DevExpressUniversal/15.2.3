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

using System.Windows;
using System;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Xpf.Utils;
using DevExpress.Utils;
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public delegate void LayoutPropertyChangedEventHandler(object sender, DependencyPropertyChangedEventArgs e);
	#region VisualLayoutViewInfo
	public class VisualLayoutViewInfo : DependencyObject, ISupportCopyFrom<AppointmentIntermediateViewInfoCore>, IBatchUpdateable, IBatchUpdateHandler {
		BatchUpdateHelper batchUpdateHelper;
		bool isRaiseDeferredLayoutPropertyChanged;
		public VisualLayoutViewInfo() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
		}
		#region Properties
		#region FirstCellIndex
		public static readonly DependencyProperty FirstCellIndexProperty = DependencyPropertyManager.Register("FirstCellIndex", typeof(int), typeof(VisualLayoutViewInfo), new PropertyMetadata(0, OnDependencyPropertyChanged));
		public int FirstCellIndex { get { return (int)GetValue(FirstCellIndexProperty); } set { SetValue(FirstCellIndexProperty, value); } }
		#endregion
		#region LastCellIndex
		public static readonly DependencyProperty LastCellIndexProperty = DependencyPropertyManager.Register("LastCellIndex", typeof(int), typeof(VisualLayoutViewInfo), new PropertyMetadata(0, OnDependencyPropertyChanged));
		public int LastCellIndex { get { return (int)GetValue(LastCellIndexProperty); } set { SetValue(LastCellIndexProperty, value); } }
		#endregion
		#region FirstIndexPosition
		public static readonly DependencyProperty FirstIndexPositionProperty = DependencyPropertyManager.Register("FirstIndexPosition", typeof(int), typeof(VisualLayoutViewInfo), new PropertyMetadata(0, OnDependencyPropertyChanged));
		public int FirstIndexPosition { get { return (int)GetValue(FirstIndexPositionProperty); } set { SetValue(FirstIndexPositionProperty, value); } }
		#endregion
		#region LastIndexPosition
		public static readonly DependencyProperty LastIndexPositionProperty = DependencyPropertyManager.Register("LastIndexPosition", typeof(int), typeof(VisualLayoutViewInfo), new PropertyMetadata(0, OnDependencyPropertyChanged));
		public int LastIndexPosition { get { return (int)GetValue(LastIndexPositionProperty); } set { SetValue(LastIndexPositionProperty, value); } }
		#endregion
		#region MaxIndexInGroup
		public static readonly DependencyProperty MaxIndexInGroupProperty = DependencyPropertyManager.Register("MaxIndexInGroup", typeof(int), typeof(VisualLayoutViewInfo), new PropertyMetadata(0, OnDependencyPropertyChanged));
		public int MaxIndexInGroup { get { return (int)GetValue(MaxIndexInGroupProperty); } set { SetValue(MaxIndexInGroupProperty, value); } }
		#endregion
		#region StartRelativeOffset
		public static readonly DependencyProperty StartRelativeOffsetProperty = DependencyPropertyManager.Register("StartRelativeOffset", typeof(int), typeof(VisualLayoutViewInfo), new PropertyMetadata(0, OnDependencyPropertyChanged));
		public int StartRelativeOffset { get { return (int)GetValue(StartRelativeOffsetProperty); } set { SetValue(StartRelativeOffsetProperty, value); } }
		#endregion
		#region EndRelativeOffset
		public static readonly DependencyProperty EndRelativeOffsetProperty = DependencyPropertyManager.Register("EndRelativeOffset", typeof(int), typeof(VisualLayoutViewInfo), new PropertyMetadata(0, OnDependencyPropertyChanged));
		public int EndRelativeOffset { get { return (int)GetValue(EndRelativeOffsetProperty); } set { SetValue(EndRelativeOffsetProperty, value); } }
		#endregion        
		#endregion
		static void OnDependencyPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e) {
			VisualLayoutViewInfo viewInfo = (VisualLayoutViewInfo)dp;
			viewInfo.OnViewInfoPropertyChanged(e);
		}
		void OnViewInfoPropertyChanged(DependencyPropertyChangedEventArgs e) {
			if (IsUpdateLocked)
				this.isRaiseDeferredLayoutPropertyChanged = true;
			else
				RaiseLayoutPropertyChanged(e);
		}
		LayoutPropertyChangedEventHandler layoutPropertyChanged;
		public event LayoutPropertyChangedEventHandler LayoutPropertyChanged {
			add { layoutPropertyChanged += value; }
			remove { layoutPropertyChanged -= value; }
		}
		protected virtual void RaiseLayoutPropertyChanged(DependencyPropertyChangedEventArgs e) {
			if (layoutPropertyChanged != null)
				layoutPropertyChanged(this, e);
		}
		#region ISupportCopyFrom<AppointmentIntermediateViewInfoCore> Members
		public void CopyFrom(AppointmentIntermediateViewInfoCore source) {
			EndRelativeOffset = source.EndRelativeOffset;
			FirstCellIndex = source.FirstCellIndex;
			FirstIndexPosition = source.FirstIndexPosition;
			LastCellIndex = source.LastCellIndex;
			LastIndexPosition = source.LastIndexPosition;
			MaxIndexInGroup = source.MaxIndexInGroup;
			StartRelativeOffset = source.StartRelativeOffset;
		}
		#endregion
		#region IBatchUpdateable implementation
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		public void BeginUpdate() {
			this.batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			this.batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			this.batchUpdateHelper.CancelUpdate();
		}
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			OnFirstBeginUpdate();
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
			OnBeginUpdate();
		}
		void IBatchUpdateHandler.OnEndUpdate() {
			OnEndUpdate();
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			OnLastEndUpdate();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
			OnCancelUpdate();
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			OnLastCancelUpdate();
		}
		protected virtual void OnFirstBeginUpdate() {
			this.isRaiseDeferredLayoutPropertyChanged = false;
		}
		protected virtual void OnBeginUpdate() {
		}
		protected virtual void OnEndUpdate() {
		}
		protected virtual void OnLastEndUpdate() {
			if (this.isRaiseDeferredLayoutPropertyChanged)
				RaiseLayoutPropertyChanged(new DependencyPropertyChangedEventArgs());
			this.isRaiseDeferredLayoutPropertyChanged = false;
		}
		protected virtual void OnCancelUpdate() {
		}
		protected virtual void OnLastCancelUpdate() {
			this.isRaiseDeferredLayoutPropertyChanged = false;
		}
		#endregion
	}
	#endregion
}
