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

using DevExpress.XtraScheduler.Commands;
using System;
namespace DevExpress.XtraScheduler.Native {
	#region WorkWeekViewMovePrevCellCommand
	public class WorkWeekViewMovePrevCellCommand : DayViewMovePrevCellCommand {
		public WorkWeekViewMovePrevCellCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override Commands.SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewMovePrevCell; } }
	}
	#endregion
	#region WorkWeekViewMoveNextCellCommand
	public class WorkWeekViewMoveNextCellCommand : DayViewMoveNextCellCommand {
		public WorkWeekViewMoveNextCellCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewMoveNextCell; } }
	}
	#endregion
	#region WorkWeekViewMovePageUpCommand
	public class WorkWeekViewMovePageUpCommand : DayViewMovePageUpCommand {
		public WorkWeekViewMovePageUpCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewMovePageUp; } }
	}
	#endregion
	#region WorkWeekViewMovePageDownCommand
	public class WorkWeekViewMovePageDownCommand : DayViewMovePageDownCommand {
		public WorkWeekViewMovePageDownCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewMovePageDown; } }
	}
	#endregion
	#region WorkWeekViewMoveToStartOfWorkTimeCommand
	public class WorkWeekViewMoveToStartOfWorkTimeCommand : DayViewMoveToStartOfWorkTimeCommand {
		public WorkWeekViewMoveToStartOfWorkTimeCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewMoveToStartOfWorkTime; } }
	}
	#endregion
	#region WorkWeekViewMoveToEndOfWorkTimeCommand
	public class WorkWeekViewMoveToEndOfWorkTimeCommand : DayViewMoveToEndOfWorkTimeCommand {
		public WorkWeekViewMoveToEndOfWorkTimeCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewMoveToEndOfWorkTime; } }
	}
	#endregion
	#region WorkWeekViewMoveToStartOfVisibleTimeCommand
	public class WorkWeekViewMoveToStartOfVisibleTimeCommand : DayViewMoveToStartOfVisibleTimeCommand {
		public WorkWeekViewMoveToStartOfVisibleTimeCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewMoveToStartOfVisibleTime; } }
	}
	#endregion
	#region WorkWeekViewMoveToEndOfVisibleTimeCommand
	public class WorkWeekViewMoveToEndOfVisibleTimeCommand : DayViewMoveToEndOfVisibleTimeCommand {
		public WorkWeekViewMoveToEndOfVisibleTimeCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewMoveToEndOfVisibleTime; } }
	}
	#endregion
	#region WorkWeekViewExtendPrevCellCommand
	public class WorkWeekViewExtendPrevCellCommand : WorkWeekViewMovePrevCellCommand {
		public WorkWeekViewExtendPrevCellCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewExtendPrevCell; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WorkWeekViewExtendNextCellCommand
	public class WorkWeekViewExtendNextCellCommand : WorkWeekViewMoveNextCellCommand {
		public WorkWeekViewExtendNextCellCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewExtendNextCell; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WorkWeekViewExtendPageUpCommand
	public class WorkWeekViewExtendPageUpCommand : WorkWeekViewMovePageUpCommand {
		public WorkWeekViewExtendPageUpCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewExtendPageUp; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WorkWeekViewExtendPageDownCommand
	public class WorkWeekViewExtendPageDownCommand : WorkWeekViewMovePageDownCommand {
		public WorkWeekViewExtendPageDownCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewExtendPageDown; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WorkWeekViewExtendToStartOfWorkTimeCommand
	public class WorkWeekViewExtendToStartOfWorkTimeCommand : WorkWeekViewMoveToStartOfWorkTimeCommand {
		public WorkWeekViewExtendToStartOfWorkTimeCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewExtendToStartOfWorkTime; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WorkWeekViewExtendToEndOfWorkTimeCommand
	public class WorkWeekViewExtendToEndOfWorkTimeCommand : WorkWeekViewMoveToEndOfWorkTimeCommand {
		public WorkWeekViewExtendToEndOfWorkTimeCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewExtendToEndOfWorkTime; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WorkWeekViewExtendToStartOfVisibleTimeCommand
	public class WorkWeekViewExtendToStartOfVisibleTimeCommand : WorkWeekViewMoveToStartOfVisibleTimeCommand {
		public WorkWeekViewExtendToStartOfVisibleTimeCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewExtendToStartOfVisibleTime; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
	#region WorkWeekViewExtendToEndOfVisibleTimeCommand
	public class WorkWeekViewExtendToEndOfVisibleTimeCommand : WorkWeekViewMoveToEndOfVisibleTimeCommand {
		public WorkWeekViewExtendToEndOfVisibleTimeCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.WorkWeekViewExtendToEndOfVisibleTime; } }
		protected internal override bool ExtendSelection { get { return true; } }
	}
	#endregion
}
