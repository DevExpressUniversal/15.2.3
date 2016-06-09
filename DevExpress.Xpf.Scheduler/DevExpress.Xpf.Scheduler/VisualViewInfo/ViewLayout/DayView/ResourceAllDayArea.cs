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
using System.Windows;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualResourceAllDayArea : VisualHorizontalCellContainer {
		protected override VisualTimeCellBaseContent CreateVisualTimeCell() {
			return new VisualAllDayAreaCellContent();
		}
	}
	public class VisualResourceAllDayAreaCollection : ObservableCollectionWithFirstAndLast<VisualResourceAllDayArea> {
	}
	public class VisualResourceAllDayAreaContainerGroupCollection : ObservableCollectionWithFirstAndLast<VisualResourceAllDayAreaContainerGroup> {
	}
	public abstract class VisualAllDayAreaContainerGroup : DependencyObject {
		#region AllDayAreaContainers
		public VisualResourceAllDayAreaCollection AllDayAreaContainers {
			get { return (VisualResourceAllDayAreaCollection)GetValue(AllDayAreaContainersProperty); }
			set { SetValue(AllDayAreaContainersProperty, value); }
		}
		public static readonly DependencyProperty AllDayAreaContainersProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAllDayAreaContainerGroup, VisualResourceAllDayAreaCollection>("AllDayAreaContainers", null);
		#endregion
		#region View
		SchedulerViewBase lastView;
		public SchedulerViewBase View {
			get { return (SchedulerViewBase)GetValue(ViewProperty); }
			set {
				if (this.lastView == value)
					return;
				SetValue(ViewProperty, value);
				this.lastView = value;
			}
		}
		public static readonly DependencyProperty ViewProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAllDayAreaContainerGroup, SchedulerViewBase>("View", null);
		#endregion
	}
	public class VisualResourceAllDayAreaContainerGroup : VisualAllDayAreaContainerGroup, ISupportCopyFrom<VisualResourcesCollection> {
		public void CopyFrom(VisualResourcesCollection source) {
			if (AllDayAreaContainers == null)
				AllDayAreaContainers = new VisualResourceAllDayAreaCollection();
			CollectionCopyHelper.Copy(AllDayAreaContainers, source);
		}
	}
	public class VisualDayAllDayAreaContainer : VisualAllDayAreaContainerGroup, ISupportCopyFrom<SingleDayViewInfo> {
		public void CopyFrom(SingleDayViewInfo source) {
			View = source.View;
			if (AllDayAreaContainers == null)
				AllDayAreaContainers = new VisualResourceAllDayAreaCollection();
			CollectionCopyHelper.Copy(AllDayAreaContainers, source.SingleResourceViewInfoCollection, CreateResourceAllDayArea);
		}
		protected virtual VisualResourceAllDayArea CreateResourceAllDayArea() {
			return new VisualResourceAllDayArea();
		}
	}
	public class VisualDayAllDayAreaContainerCollection :  ObservableCollectionWithFirstAndLast<VisualDayAllDayAreaContainer> {
	}
}
