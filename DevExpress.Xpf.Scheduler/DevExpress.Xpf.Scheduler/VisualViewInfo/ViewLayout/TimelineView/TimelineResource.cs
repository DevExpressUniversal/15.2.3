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
using System.Windows;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualTimelineResource : VisualResource {
		protected override VisualSimpleResourceInterval CreateVisualSimpleResourceInterval() {
			return new VisualTimeline();
		}
		protected override void CopyFromCore(SingleResourceViewInfo source) {
			base.CopyFromCore(source);
		}
		protected override CellContainerCollection GetCellContainers(SingleResourceViewInfo source) {
			return source.CellContainers;
		}
		protected override VisualResourceHeaderBaseContent CreateResourceHeaderCore() {
			return new VisualResourceHeaderContent(); 
		}
	}
	public class VisualGanttResource : VisualTimelineResource {
		#region NestingLevel
		public static readonly DependencyProperty NestingLevelProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualGanttResource, int>("NestingLevel", 0);
		public int NestingLevel { get { return (int)GetValue(NestingLevelProperty); } set { SetValue(NestingLevelProperty, value); } }
		#endregion
		#region IsExpanded
		public static readonly DependencyProperty IsExpandedProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualGanttResource, bool>("IsExpanded", false);
		public bool IsExpanded { get { return (bool)GetValue(IsExpandedProperty); } set { SetValue(IsExpandedProperty, value); } }
		#endregion
		#region ExpandCommand
		public static readonly DependencyProperty ExpandCommandProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualGanttResource, SchedulerUICommandBase>("ExpandCommand", null);
		public SchedulerUICommandBase ExpandCommand { get { return (SchedulerUICommandBase)GetValue(ExpandCommandProperty); } set { SetValue(ExpandCommandProperty, value); } }
		#endregion
		#region CollapseCommand
		public static readonly DependencyProperty CollapseCommandProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualGanttResource, SchedulerUICommandBase>("CollapseCommand", null);
		public SchedulerUICommandBase CollapseCommand { get { return (SchedulerUICommandBase)GetValue(CollapseCommandProperty); } set { SetValue(CollapseCommandProperty, value); } }
		#endregion
		protected override void CopyFromCore(SingleResourceViewInfo source) {
			base.CopyFromCore(source);
			GanttViewSingleResourceViewInfo viewInfo = source as GanttViewSingleResourceViewInfo;
			if (viewInfo == null)
				return;
			NestingLevel = viewInfo.NestingLevel;
			IsExpanded = viewInfo.IsExpanded;
			ExpandCommand = viewInfo.ExpandCommand;
			CollapseCommand = viewInfo.CollapseCommand;
		}
	}
}
