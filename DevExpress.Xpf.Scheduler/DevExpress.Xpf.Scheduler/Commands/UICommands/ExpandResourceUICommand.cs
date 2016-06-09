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
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Commands;
namespace DevExpress.Xpf.Scheduler.Commands {
	#region ResourceUICommandBase (abstract class)
	public abstract class ResourceUICommandBase : SchedulerUICommandBase {
		XtraScheduler.Resource resource;
		protected ResourceUICommandBase(XtraScheduler.Resource resource) {
			this.resource = resource;
		}
		public Resource Resource { get { return resource; } }
		protected override bool CanExecuteCore(SchedulerControl control) {
			return Resource != null && base.CanExecuteCore(control);
		}
		public override SchedulerCommand CreateCommand(SchedulerControl control) {
			ResourceCommandBase command = (ResourceCommandBase)base.CreateCommand(control);
			command.Resource = Resource;
			return command;
		}
		protected override void ExecuteCore(SchedulerControl control) {
			SchedulerCommand command = CreateCommand(control);
			command.Execute();
		}
	}
	#endregion
	#region ExpandResourceUICommand
	public class ExpandResourceUICommand : ResourceUICommandBase {
		public ExpandResourceUICommand(XtraScheduler.Resource resource)
			: base(resource) {
		}
		public override SchedulerCommandId CommandId { get { return SchedulerCommandId.ExpandResource; } }
	}
	#endregion
	#region CollapseResourceUICommand
	public class CollapseResourceUICommand : ResourceUICommandBase {
		public CollapseResourceUICommand(XtraScheduler.Resource resource)
			: base(resource) {
		}
		public override SchedulerCommandId CommandId { get { return SchedulerCommandId.CollapseResource; } }
	}
	#endregion
}
