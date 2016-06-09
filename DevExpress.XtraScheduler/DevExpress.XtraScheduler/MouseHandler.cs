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
using System.Collections;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Controls;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraScheduler.Commands.Internal;
#if !SILVERLIGHT && !WPF
using DevExpress.XtraEditors;
#else
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
#endif
#if !SILVERLIGHT 
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using PlatformIndependentMouseButtons = System.Windows.Forms.MouseButtons;
using PlatformIndependentDragEventArgs = System.Windows.Forms.DragEventArgs;
using PlatformIndependentQueryContinueDragEventArgs = System.Windows.Forms.QueryContinueDragEventArgs;
using PlatformIndependentDragDropEffects = System.Windows.Forms.DragDropEffects;
using PlatformIndependentIDataObject = System.Windows.Forms.IDataObject;
using PlatformIndependentDragAction = System.Windows.Forms.DragAction;
using PlatformIndependentSystemInformation = System.Windows.Forms.SystemInformation;
using PlatformIndependentCursor = System.Windows.Forms.Cursor;
using PlatformIndependentCursors = System.Windows.Forms.Cursors;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#else
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
using PlatformIndependentMouseButtons = DevExpress.Data.MouseButtons;
using PlatformIndependentDragEventArgs = DevExpress.Utils.DragEventArgs;
using PlatformIndependentQueryContinueDragEventArgs = DevExpress.Utils.QueryContinueDragEventArgs;
using PlatformIndependentDragDropEffects = DevExpress.Utils.DragDropEffects;
using PlatformIndependentIDataObject = System.Windows.IDataObject;
using PlatformIndependentDragAction = DevExpress.Utils.DragAction;
using PlatformIndependentSystemInformation = DevExpress.Utils.SystemInformation;
using PlatformIndependentCursor = System.Windows.Input.Cursor;
using PlatformIndependentCursors = System.Windows.Input.Cursors;
#endif
#if WPF||SL
using DevExpress.Xpf.Scheduler;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Scheduler.UI;
#endif
namespace DevExpress.XtraScheduler.Native {
	#region ScrollType
	public enum ScrollType {
		Resources,
		DateTime,
		None
	}
	#endregion
	#region SchedulerOfficeScroller
	public class SchedulerOfficeScroller : OfficeScroller, ISchedulerOfficeScroller {
		#region static methods and fields
		static readonly Hashtable htHorizontalScrollTypes;
		static readonly Hashtable htVerticalScrollTypes;
		static SchedulerOfficeScroller() {
			htHorizontalScrollTypes = new Hashtable();
			htVerticalScrollTypes = new Hashtable();
			AddScrollTypes(SchedulerViewType.Day, SchedulerGroupType.None, ScrollType.None, ScrollType.DateTime);
			AddScrollTypes(SchedulerViewType.Month, SchedulerGroupType.None, ScrollType.None, ScrollType.DateTime);
			AddScrollTypes(SchedulerViewType.Timeline, SchedulerGroupType.None, ScrollType.DateTime, ScrollType.None);
			AddScrollTypes(SchedulerViewType.Week, SchedulerGroupType.None, ScrollType.None, ScrollType.DateTime);
			AddScrollTypes(SchedulerViewType.WorkWeek, SchedulerGroupType.None, ScrollType.None, ScrollType.DateTime);
			AddScrollTypes(SchedulerViewType.FullWeek, SchedulerGroupType.None, ScrollType.None, ScrollType.DateTime);
			AddScrollTypes(SchedulerViewType.Gantt, SchedulerGroupType.None, ScrollType.None, ScrollType.DateTime);
			AddScrollTypes(SchedulerViewType.Day, SchedulerGroupType.Date, ScrollType.Resources, ScrollType.DateTime);
			AddScrollTypes(SchedulerViewType.Month, SchedulerGroupType.Date, ScrollType.DateTime, ScrollType.Resources);
			AddScrollTypes(SchedulerViewType.Timeline, SchedulerGroupType.Date, ScrollType.DateTime, ScrollType.Resources);
			AddScrollTypes(SchedulerViewType.Week, SchedulerGroupType.Date, ScrollType.DateTime, ScrollType.Resources);
			AddScrollTypes(SchedulerViewType.WorkWeek, SchedulerGroupType.Date, ScrollType.Resources, ScrollType.DateTime);
			AddScrollTypes(SchedulerViewType.FullWeek, SchedulerGroupType.Date, ScrollType.Resources, ScrollType.DateTime);
			AddScrollTypes(SchedulerViewType.Gantt, SchedulerGroupType.Date, ScrollType.Resources, ScrollType.DateTime);
			AddScrollTypes(SchedulerViewType.Day, SchedulerGroupType.Resource, ScrollType.Resources, ScrollType.DateTime);
			AddScrollTypes(SchedulerViewType.Month, SchedulerGroupType.Resource, ScrollType.Resources, ScrollType.DateTime);
			AddScrollTypes(SchedulerViewType.Timeline, SchedulerGroupType.Resource, ScrollType.DateTime, ScrollType.Resources);
			AddScrollTypes(SchedulerViewType.Week, SchedulerGroupType.Resource, ScrollType.Resources, ScrollType.DateTime);
			AddScrollTypes(SchedulerViewType.WorkWeek, SchedulerGroupType.Resource, ScrollType.Resources, ScrollType.DateTime);
			AddScrollTypes(SchedulerViewType.FullWeek, SchedulerGroupType.Resource, ScrollType.Resources, ScrollType.DateTime);
			AddScrollTypes(SchedulerViewType.Gantt, SchedulerGroupType.Resource, ScrollType.Resources, ScrollType.DateTime);
#if DEBUG && !SILVERLIGHT && !WPF
			Array viewTypes = Enum.GetValues(typeof(SchedulerViewType));
			Array groupTypes = Enum.GetValues(typeof(SchedulerGroupType));
			foreach (SchedulerViewType viewType in viewTypes) {
				foreach (SchedulerGroupType groupType in groupTypes) {
					int hashCode = CalculateHashCode(viewType, groupType);
					XtraSchedulerDebug.Assert(htHorizontalScrollTypes[hashCode] != null);
					XtraSchedulerDebug.Assert(htVerticalScrollTypes[hashCode] != null);
				}
			}
#endif
		}
		static void AddScrollTypes(SchedulerViewType viewType, SchedulerGroupType groupType, ScrollType horizontalScrollType, ScrollType verticalScrollType) {
			int hashCode = CalculateHashCode(viewType, groupType);
			htHorizontalScrollTypes[hashCode] = horizontalScrollType;
			htVerticalScrollTypes[hashCode] = verticalScrollType;
		}
		internal static int CalculateHashCode(SchedulerViewType viewType, SchedulerGroupType groupType) {
			return ((int)viewType << 16) + (int)groupType;
		}
		#endregion
		#region Fields
		SchedulerControl control;
		bool isDisposed;
		#endregion
		public SchedulerOfficeScroller(SchedulerControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			isDisposed = false;
		}
		#region Properties
#if !SILVERLIGHT && !WPF
		protected override bool AllowHScroll { get { return CanDoScroll(htHorizontalScrollTypes); } }
		protected override bool AllowVScroll { get { return CanDoScroll(htVerticalScrollTypes); } }
		ResourceScrollController ResourceScrollController { get { return control.ResourceScrollController; } }
#endif
		ViewDateTimeScrollController DateTimeScrollController { get { return control.DateTimeScrollController; } }
		protected internal bool IsDisposed { get { return isDisposed; } }
		#endregion
#if !SILVERLIGHT && !WPF
		#region IDisposable implementation
		public override void Dispose() {
			base.Dispose();
			isDisposed = true;
		}
		~SchedulerOfficeScroller() {
			isDisposed = true;
			GC.SuppressFinalize(this);
		}
		#endregion
#endif
		protected internal virtual bool CanDoScroll(Hashtable htScrollTypes) {
#if !SILVERLIGHT && !WPF
			ScrollType scrollType = GetScrollType(htScrollTypes);
			if (scrollType == ScrollType.None)
				return false;
			if (scrollType == ScrollType.Resources) {
				bool canScrollResources = ResourceScrollController.IsResourceNavigatorActionEnabled(NavigatorButtonType.First) ||
					ResourceScrollController.IsResourceNavigatorActionEnabled(NavigatorButtonType.Last);
				return control.ResourceNavigator.Visible && canScrollResources;
			}
			else
				return control.DateTimeScrollBarObject.Visible;
#else
			return false;
#endif
		}
		int ISchedulerOfficeScroller.VerticalScrollValue {
			get {
				ScrollType scrollType = GetScrollType(htVerticalScrollTypes);
				if (scrollType == ScrollType.None)
					return -1;
				if (scrollType == ScrollType.Resources) {
					return control.ActiveView.InnerView.ActualFirstVisibleResourceIndex;
				} else
					return (int)control.DateTimeScrollBarObject.ScrollBarAdapter.Value;
			}
		}
		int ISchedulerOfficeScroller.HorizontalScrollValue {
			get {
				ScrollType scrollType = GetScrollType(htHorizontalScrollTypes);
				if (scrollType == ScrollType.None)
					return -1;
				if (scrollType == ScrollType.Resources) {
					return control.ActiveView.InnerView.ActualFirstVisibleResourceIndex;
				} else
					return (int)control.DateTimeScrollBarObject.ScrollBarAdapter.Value;
			}
		}
		void ISchedulerOfficeScroller.ScrollVertical(int delta) {
			Scroll(htVerticalScrollTypes, delta);
		}
		void ISchedulerOfficeScroller.ScrollVerticalByPixel(int delta) {
			Scroll(htVerticalScrollTypes, delta, false);
		}
		void ISchedulerOfficeScroller.ScrollHorizontal(int delta) {
			Scroll(htHorizontalScrollTypes, delta);
		}
		protected override void OnVScroll(int delta) {
			Scroll(htVerticalScrollTypes, delta);
		}
#if !SILVERLIGHT && !WPF
		protected override void OnHScroll(int delta) {
			Scroll(htHorizontalScrollTypes, delta);
		}
#endif
		protected internal virtual void Scroll(Hashtable htScrollTypes, int delta) {
			Scroll(htScrollTypes, delta, true);
		}
		protected internal virtual void Scroll(Hashtable htScrollTypes, int delta, bool deltaAsLine) {
			ScrollType scrollType = GetScrollType(htScrollTypes);
			if (scrollType == ScrollType.None)
				return;
			if (scrollType == ScrollType.Resources) {
				ScrollResource(delta);
			} else
				DateTimeScrollController.Scroll(control.DateTimeScrollBarObject.ScrollBarAdapter, delta, deltaAsLine);
		}
		void ISchedulerOfficeScroller.ScrollResource(int delta) {
			ScrollResource(delta);
		}
		void ScrollResource(int delta) {
			int newFirstVisibleResourceIndex = this.control.ActiveView.InnerView.ActualFirstVisibleResourceIndex + delta;
			control.ActiveView.InnerView.FirstVisibleResourceIndex = NormalizeResourceIndex(newFirstVisibleResourceIndex);
		}
		protected internal int NormalizeResourceIndex(int index) {
#if !SILVERLIGHT && !WPF
			if (index < 0)
				return 0;
			int maxValue = ResourceScrollController.View.FilteredResources.Count - control.ActiveView.ActualResourcesPerPage;
			if (index > maxValue)
				return maxValue;
			else
				return index;
#else
			if (index < 0)
				return 0;
			int maxValue = control.InnerControl.ActiveView.FilteredResources.Count - control.InnerControl.ActiveView.ActualResourcesPerPage;
			if (index > maxValue)
				return maxValue;
			else
				return index;
#endif
		}
		protected internal virtual ScrollType GetScrollType(Hashtable htScrollTypes) {
			SchedulerGroupType realGroupType = control.GroupType;
			if (realGroupType == SchedulerGroupType.Resource && control.ActiveView.VisibleResources.Count == 0)
				realGroupType = SchedulerGroupType.None;
			int hashCode = CalculateHashCode(control.ActiveViewType, realGroupType);
			object scrollType = htScrollTypes[hashCode];
			if (scrollType == null)
				return ScrollType.None;
			return (ScrollType)scrollType;
		}			   
	}
	#endregion
}
