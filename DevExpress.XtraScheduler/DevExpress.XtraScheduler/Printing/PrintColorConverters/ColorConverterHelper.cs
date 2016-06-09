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

using DevExpress.XtraScheduler.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing {
	public class ColorConverterHelper {
		PrintColorConverter colorConverter;
		public ColorConverterHelper(PrintColorConverter colorConverter) {
			this.colorConverter = colorConverter;
		}
		public PrintColorConverter ColorConverter { get { return colorConverter; } }
		public void ApplyColorConverterToAppointments(IEnumerable<AppointmentViewInfo> viewInfos) {
			foreach (AppointmentViewInfo vi in viewInfos)
				ApplyColorConverterToAppointment(vi);
		}
		public void ApplyColorConverterToHeaders(SchedulerHeaderCollection headers) {
			headers.ForEach(ApplyColorConverterToHeader);
		}
		public void ApplyColorConverterToHeader(SchedulerHeader header) {
			colorConverter.ConvertHeaderCaptionAppearance(header.Appearance.HeaderCaption);
			colorConverter.ConvertHeaderCaptionLineAppearance(header.Appearance.HeaderCaptionLine);
			if (header.Image != null)
				header.Image = colorConverter.GetConvertedImage(header.Image);
		}
		void ApplyColorConverterToAppointment(AppointmentViewInfo viewInfo) {
			colorConverter.ConvertAppointmentAppearance(viewInfo.Appearance);
			viewInfo.Items.ForEach(ApplyColorConverterToAppointmentViewInfoItem);
			viewInfo.StatusItems.ForEach(ApplyColorConverterToAppointmentStatusItem);
		}
		void ApplyColorConverterToAppointmentStatusItem(ViewInfoItem item) {
			AppointmentViewInfoStatusItem statusItem = item as AppointmentViewInfoStatusItem;
			if (statusItem != null) {
				if (statusItem.BackgroundViewInfo.Bounds != Rectangle.Empty) {
					statusItem.BackgroundViewInfo.SetBrush(colorConverter.ConvertBrush(statusItem.BackgroundViewInfo.GetBrush()));
					statusItem.BackgroundViewInfo.BorderColor = colorConverter.ConvertColor(statusItem.BackgroundViewInfo.BorderColor);
				}
				statusItem.ForegroundViewInfo.SetBrush(colorConverter.ConvertBrush(statusItem.ForegroundViewInfo.GetBrush()));
				statusItem.ForegroundViewInfo.BorderColor = colorConverter.ConvertColor(statusItem.ForegroundViewInfo.BorderColor);
			}
		}
		void ApplyColorConverterToAppointmentViewInfoItem(ViewInfoItem item) {
			ViewInfoImageItem imageItem = item as ViewInfoImageItem;
			ViewInfoTextItem textItem = item as ViewInfoTextItem;
			ViewInfoHorizontalLineItem horizontalLineItem = item as ViewInfoHorizontalLineItem;
			if (imageItem != null) {
				imageItem.SetImage(colorConverter.GetConvertedImage(imageItem.Image));
			}
			if (textItem != null) {
				colorConverter.ConvertAppearance(textItem.Appearance);
			}
			if (horizontalLineItem != null) {
				colorConverter.ConvertAppearance(horizontalLineItem.Appearance);
			}
		}
	}
}
