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
using System.Text;
using DevExpress.XtraScheduler.Commands;
using DevExpress.Utils.Commands;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Utils;
using System.Drawing;
namespace DevExpress.XtraScheduler.Native {
	public class SchedulerControlClientDataProvider : SchedulerInnerControlClientDataProvider {
		readonly SchedulerControl control;
		public SchedulerControlClientDataProvider(SchedulerControl control)
			: base(control.InnerControl) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			PrevAutoAdjustMode = SchedulerOptionsRangeControl.DefaultAutoAdjustMode;
		}
		public SchedulerControl SchedulerControl { get { return control; } }
		protected override bool AllowChangeActiveView { get { return SchedulerControl.OptionsRangeControl.AllowChangeActiveView; } }
		protected override bool AutoAdjustMode { get { return SchedulerControl.OptionsRangeControl.AutoAdjustMode; } }
		protected override IDataItemThumbnail CreateDataItemThumbnailItem(Appointment apt) {
			Color color = CalculateAppointmentColor(apt);
			return new DataItemThumbnail(color, apt);
		}
		protected virtual Color CalculateAppointmentColor(Appointment appointment) {
			Color appearanceColor = SchedulerControl.Appearance.Appointment.BackColor;
			if ( (appearanceColor == SystemColors.Window) || (appearanceColor == Color.Empty) )
				return SchedulerControl.GetLabelColor(appointment.LabelKey);
			else
				return appearanceColor;
		}
		protected override IScaleBasedRangeControlClientOptions GetOptionsCore() {
			return SchedulerControl.OptionsRangeControl;
		}
	}
}
namespace DevExpress.XtraScheduler.Commands {
	#region RangeControlZoomInCommand
	public class RangeControlZoomInCommand : SchedulerMenuItemWinSimpleCommand {
		public RangeControlZoomInCommand(ISchedulerCommandTarget target)
			: base(target) {
			Client = SchedulerControl.RangeControlSupport;
		}
		#region Properties
		public override SchedulerCommandId Id { get { return SchedulerCommandId.None; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.Custom; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		public override string ImageName { get { return string.Empty; } }
		protected virtual ScaleBasedRangeControlClient Client { get; private set; }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = true;
			state.Enabled = Client != null && Client.CanZoomIn();
			state.Checked = false;
		}
		protected internal override void ExecuteCore() {
			if (Client == null)
				return;
			Client.ZoomIn();
		}
	}
	#endregion
	#region ViewZoomOutCommand
	public class RangeControlZoomOutCommand : SchedulerMenuItemWinSimpleCommand {
		public RangeControlZoomOutCommand(ISchedulerCommandTarget target)
			: base(target) {
			Client = SchedulerControl.RangeControlSupport;
		}
		#region Properties
		public override SchedulerCommandId Id { get { return SchedulerCommandId.None; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.Custom; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		public override string ImageName { get { return string.Empty; } }
		protected virtual ScaleBasedRangeControlClient Client { get; private set; }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = true;
			state.Enabled = Client != null && Client.CanZoomOut();
			state.Checked = false;
		}
		protected internal override void ExecuteCore() {
			if (Client == null)
				return;
			Client.ZoomOut();
		}
	}
	#endregion
}
