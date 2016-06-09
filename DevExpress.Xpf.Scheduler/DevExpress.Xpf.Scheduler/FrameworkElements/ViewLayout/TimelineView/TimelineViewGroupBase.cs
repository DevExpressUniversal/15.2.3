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
using DevExpress.Xpf.Scheduler.Native;
using System.ComponentModel;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualTimelineViewGroupBase : VisualResourcesBasedViewInfo {
		#region Header
		public static readonly DependencyProperty HeaderProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimelineViewGroupBase, VisualTimelineHeader>("Header", null);
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualTimelineViewGroupBaseHeader")]
#endif
public VisualTimelineHeader Header { get { return (VisualTimelineHeader)GetValue(HeaderProperty); } set { SetValue(HeaderProperty, value); } }
		#endregion
		#region SelectionBarContainer
		public static readonly DependencyProperty SelectionBarContainerProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimelineViewGroupBase, VisualTimelineSelectionBar>("SelectionBarContainer", null);
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualTimelineViewGroupBaseSelectionBarContainer")]
#endif
		public VisualTimelineSelectionBar SelectionBarContainer { get { return (VisualTimelineSelectionBar)GetValue(SelectionBarContainerProperty); } set { SetValue(SelectionBarContainerProperty, value); } }
		#endregion
		protected override VisualResource CreateVisualResource() {
			return new VisualTimelineResource();
		}
		protected override void CopyFromCore(ISchedulerViewInfoBase source) {
			base.CopyFromCore(source);
			TimelineViewInfoBase viewInfo = source as TimelineViewInfoBase;
			if (ResourceContainers == null)
				ResourceContainers = new VisualResourcesCollection();
			CollectionCopyHelper.Copy(ResourceContainers, viewInfo.ResourcesContainers, CreateVisualResource);
			if (Header == null)
				Header = new VisualTimelineHeader();
			((ISupportCopyFrom<TimelineHeader>)Header).CopyFrom(viewInfo.Header);
			if (SelectionBarContainer == null)
				SelectionBarContainer = new VisualTimelineSelectionBar();
			if (viewInfo.SelectionBar != null)
				((ISupportCopyFrom<ICellContainer>)SelectionBarContainer).CopyFrom(viewInfo.SelectionBar);			
		}
		protected override void CopyAppointmentsViewInfoCore(ISchedulerViewInfoBase source) {
			base.CopyAppointmentsViewInfoCore(source);
			if (ResourceContainers == null)
				return;
			TimelineViewInfoBase viewInfo = source as TimelineViewInfoBase;
			int count = viewInfo.ResourcesContainers.Count;
			for (int i = 0; i < count; i++)
				ResourceContainers[i].CopyAppointmentsFrom(viewInfo.ResourcesContainers[i]);
		}
	}
}
