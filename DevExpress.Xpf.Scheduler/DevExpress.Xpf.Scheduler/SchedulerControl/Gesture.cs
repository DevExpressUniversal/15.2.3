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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Office.Internal;
using DevExpress.Xpf.Scheduler.Commands;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
namespace DevExpress.Xpf.Scheduler {
	public partial class SchedulerControl : IGestureClient {
		GestureHelper gestureHelper;
		int overY = 0;
		int overX = 0;
		double intermediateZoom = 1;
		void InitializeTouch() {
			this.gestureHelper = new GestureHelper(this);
			this.gestureHelper.Stop();
			this.gestureHelper.Start(this);
		}
		#region IGestureClient
		void IGestureClient.OnManipulationStarting(ManipulationStartingEventArgs e) {
			if (!OptionsBehavior.TouchAllowed) {
				e.Mode = ManipulationModes.None;
				return;
			}
			this.overX = 0;
			this.overY = 0;
			this.intermediateZoom = 1;
			e.Mode = ManipulationModes.TranslateY;
			e.Mode |= ManipulationModes.Scale;
			e.Mode |= ManipulationModes.TranslateX;
		}
		int IGestureClient.OnVerticalPan(ManipulationDeltaEventArgs e) {
			if (!OptionsBehavior.TouchAllowed)
				return 0;
			this.overY += (int)Math.Round(e.DeltaManipulation.Translation.Y);
			ScrollVerticallyByPhysicalOffsetCommand scrollCommand = new ScrollVerticallyByPhysicalOffsetCommand(this);
			scrollCommand.PhysicalOffset = this.overY;
			scrollCommand.Execute();
			this.overY -= scrollCommand.ScrolledOffset;
			return 0;
		}
		int IGestureClient.OnHorizontalPan(ManipulationDeltaEventArgs e) {
			if (!OptionsBehavior.TouchAllowed)
				return 0;
			this.overX += (int)Math.Round(e.DeltaManipulation.Translation.X);
			ScrollHorizontallyByPhysicalOffsetCommand scrollCommand = new ScrollHorizontallyByPhysicalOffsetCommand(this);
			scrollCommand.PhysicalOffset = this.overX;
			scrollCommand.Execute();
			this.overX -= scrollCommand.ScrolledOffset;
			return 0;
		}		
		void IGestureClient.OnZoom(ManipulationDeltaEventArgs e, double zoomDelta) {
			if (!OptionsBehavior.TouchAllowed)
				return;
			this.intermediateZoom *= zoomDelta;
			SchedulerCommandId commandId = SchedulerCommandId.None;
			if (this.intermediateZoom > 2) {
				this.intermediateZoom = 1;
				commandId = SchedulerCommandId.SwitchToMoreDetailedView;
			} else if (this.intermediateZoom < 0.5) {
				this.intermediateZoom = 1;
				commandId = SchedulerCommandId.SwitchToLessDetailedView;
			}
			ISchedulerCommandFactoryService commandFactoryService = GetService<ISchedulerCommandFactoryService>();
			if (commandFactoryService == null || commandId == SchedulerCommandId.None)
				return;
			SchedulerCommand command = commandFactoryService.CreateCommand(commandId);
			if (command == null)
				return;
			command.Execute();
		}
		#endregion
		protected virtual void OnTouchAllowedChanged(bool value) {
			if (value)
				this.gestureHelper.Start(this);
			else
				this.gestureHelper.Stop();
		}
	}	
}
