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
using DevExpress.Utils.Commands;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Operations;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Commands {
	public abstract class SwitchToDetailedViewBase : SchedulerCommand {
		static List<SchedulerViewType> orderedViewTypeList;
		protected static List<SchedulerViewType> OrderedViewTypes {
			get {
				if (orderedViewTypeList == null)
					orderedViewTypeList = CreateOrderedViewTypeList();
				return orderedViewTypeList;
			}
		}
		static List<SchedulerViewType> CreateOrderedViewTypeList() {
			List<SchedulerViewType> result = new List<SchedulerViewType>();
			result.Add(SchedulerViewType.Day);
			result.Add(SchedulerViewType.WorkWeek);
			result.Add(SchedulerViewType.Week);
			result.Add(SchedulerViewType.Month);
			result.Add(SchedulerViewType.Timeline);
			result.Add(SchedulerViewType.Gantt);
			return result;
		}
		protected SwitchToDetailedViewBase(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.Custom; } }
		public override Localization.SchedulerStringId DescriptionStringId { get { return Localization.SchedulerStringId.MenuCmd_None; } }
		public override Localization.SchedulerStringId MenuCaptionStringId { get { return Localization.SchedulerStringId.MenuCmd_None; } }
		public override void ForceExecute(Utils.Commands.ICommandUIState state) {
			SchedulerViewType viewType = GetNextViewType(Control.ActiveViewType);
			Control.ActiveViewType = viewType;
		}
		protected override void UpdateUIStateCore(Utils.Commands.ICommandUIState state) {
		}
		protected abstract SchedulerViewType GetNextViewType(SchedulerViewType schedulerViewType);
	}
	public class SwitchToMoreDetailedView : SwitchToDetailedViewBase {
		public SwitchToMoreDetailedView(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.SwitchToMoreDetailedView; } }
		protected override SchedulerViewType GetNextViewType(SchedulerViewType viewType) {
			int indx = OrderedViewTypes.IndexOf(viewType);
			XtraSchedulerDebug.Assert(indx != -1);
			indx--;
			if (indx < 0)
				return viewType;
			return OrderedViewTypes[indx];
		}
	}
	public class SwitchToLessDetailedView : SwitchToDetailedViewBase {
		public SwitchToLessDetailedView(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.SwitchToLessDetailedView; } }
		protected override SchedulerViewType GetNextViewType(SchedulerViewType viewType) {
			int indx = OrderedViewTypes.IndexOf(viewType);
			XtraSchedulerDebug.Assert(indx != -1);
			indx++;
			if (indx >= OrderedViewTypes.Count)
				return viewType;
			return OrderedViewTypes[indx];
		}
	}
}
