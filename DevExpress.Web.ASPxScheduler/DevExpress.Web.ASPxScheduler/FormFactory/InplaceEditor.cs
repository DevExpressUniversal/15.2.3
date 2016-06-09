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
using System.ComponentModel;
using DevExpress.XtraScheduler;
namespace DevExpress.Web.ASPxScheduler {
	#region AppointmentInplaceEditorTemplateContainer
	public class AppointmentInplaceEditorTemplateContainer : AppointmentFormTemplateContainerBase {
		[Obsolete("You should use the 'AppointmentInplaceEditorTemplateContainer(ASPxScheduler control)' constructor instead", false)]
		public AppointmentInplaceEditorTemplateContainer(ASPxScheduler control, Appointment apt)
			: this(control) { 
		}
		public AppointmentInplaceEditorTemplateContainer(ASPxScheduler control)
			: base(control) {
		}
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentInplaceEditorTemplateContainerSaveHandler")]
#endif
		public override string SaveHandler { get { return String.Format("function() {{ ASPx.InplaceEditorSave(\"{0}\"); }}", ControlClientId); } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentInplaceEditorTemplateContainerSaveScript")]
#endif
		public override string SaveScript { get { return String.Format("ASPx.InplaceEditorSave(\"{0}\")", ControlClientId); } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentInplaceEditorTemplateContainerEditFormHandler")]
#endif
		public virtual string EditFormHandler { get { return String.Format("function() {{ ASPx.InplaceEditorEditForm(\"{0}\"); }}", ControlClientId); } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentInplaceEditorTemplateContainerEditFormScript")]
#endif
		public virtual string EditFormScript { get { return String.Format("ASPx.InplaceEditorEditForm(\"{0}\")", ControlClientId); } }
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return e;
		}
	}
	#endregion
}
