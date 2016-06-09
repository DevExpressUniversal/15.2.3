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

using DevExpress.XtraScheduler.Internal;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
namespace DevExpress.XtraScheduler.Design {
	public class AppointmentStatusBrushUITypeEditor : UITypeEditor {
		ColorEditor colorEditor;
		public AppointmentStatusBrushUITypeEditor() {
			this.colorEditor = new ColorEditor();
		}
		public override bool IsDropDownResizable {
			get {
				return this.colorEditor.IsDropDownResizable;
			}
		}
		public override void PaintValue(PaintValueEventArgs e) {
			AppointmentStatus status = e.Context.Instance as AppointmentStatus;
			AppointmentStatusType type = (status == null) ? AppointmentStatusType.Custom : status.Type;
			Brush brush = e.Value as Brush;
			if (brush == null)
				return;
			Color color = SchedulerBrushHelper.GetColorFromAppointmentStatusType(type, brush);
			PaintValueEventArgs ea = new PaintValueEventArgs(e.Context, color, e.Graphics, e.Bounds);
			this.colorEditor.PaintValue(ea);
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			AppointmentStatus status = context.Instance as AppointmentStatus;
			if (status == null)
				return false;
			return this.colorEditor.GetPaintValueSupported(context);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			AppointmentStatus status = context.Instance as AppointmentStatus;
			AppointmentStatusType type = (status == null) ? AppointmentStatusType.Custom : status.Type;
			Brush brush = value as Brush;
			if (brush == null)
				return base.EditValue(context, provider, value);
			Color result = (Color)this.colorEditor.EditValue(provider, SchedulerBrushHelper.GetColorFromAppointmentStatusType(type, brush));
			return SchedulerBrushHelper.GetBrushFromAppointmentStatusType(type, result);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return this.colorEditor.GetEditStyle();
		}
	}
}
