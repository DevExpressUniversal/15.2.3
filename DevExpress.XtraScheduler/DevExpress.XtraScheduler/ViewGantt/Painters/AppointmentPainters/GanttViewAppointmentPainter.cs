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

using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class GanttViewAppointmentPainter : TimelineAppointmentPainter {
		public GanttViewAppointmentPainter()
			: base() {
		}
		protected override void DrawBackgroundContent(GraphicsCache cache, AppointmentViewInfo viewInfo) {
			Rectangle bounds = viewInfo.Bounds;
			if (viewInfo.PercentCompleteBounds.Width != 0) {
				bounds = RectUtils.CutFromLeft(bounds, viewInfo.PercentCompleteBounds.Width);
				viewInfo.PercentCompleteBounds = new Rectangle(viewInfo.PercentCompleteBounds.X, bounds.Y, viewInfo.PercentCompleteBounds.Width, bounds.Height); 
				cache.FillRectangle(cache.GetSolidBrush(viewInfo.PercentCompleteColor), viewInfo.PercentCompleteBounds);
			}
			viewInfo.Appearance.DrawBackground(cache, bounds);
		}
		public static Color GetBackColorForPercentComplete(Color imageColor) {
			Color resultColor;
			resultColor = Color.FromArgb(imageColor.A, Math.Abs(imageColor.R - 50), Math.Abs(imageColor.G - 50), Math.Abs(imageColor.B - 50));
			return resultColor;
		}
	}
}
