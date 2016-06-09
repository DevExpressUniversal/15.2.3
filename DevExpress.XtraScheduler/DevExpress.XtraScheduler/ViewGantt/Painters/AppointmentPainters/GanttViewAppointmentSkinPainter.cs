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

using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
namespace DevExpress.XtraScheduler.Drawing {
	public class GanttViewAppointmentSkinPainter : TimelineAppointmentSkinPainter {
		public GanttViewAppointmentSkinPainter(UserLookAndFeel lookAndFeel)
			: base(lookAndFeel) {
		}
		protected override void DrawBackgroundContent(GraphicsCache cache, AppointmentViewInfo viewInfo) {
			base.DrawBackgroundContent(cache, viewInfo);
			AppointmentViewInfo aptViewInfo = (AppointmentViewInfo)viewInfo;
			SkinElementInfo progress = aptViewInfo.CachedSkinElementInfos.Progress;
			if (progress != null && progress.Bounds.Width > 0) {
				progress.ImageIndex = viewInfo.Selected ? 1 : 0;
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, progress);
			}
		}
		protected internal override int GetAppointmentPercentCompleteBoundsWidth(AppointmentViewInfo viewInfo) {
			if (!ShouldShowPercentCompleteBar(viewInfo.Options.PercentCompleteDisplayType))
				return base.GetAppointmentPercentCompleteBoundsWidth(viewInfo);
			int progressValue = viewInfo.Appointment.PercentComplete;
			int width = viewInfo.RightBorderBounds.Left - viewInfo.LeftBorderBounds.Right;
			int result = (int)Math.Round(progressValue / (float)AppointmentProcessValues.Max * width);
			return result;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		protected internal override void PrepareSkinElementInfosForPercentComplete(AppointmentViewInfo viewInfo, AppointmentViewInfoSkinElementInfoCache cache, int progressWidth, Rectangle contentBounds) {
			base.PrepareSkinElementInfosForPercentComplete(viewInfo, cache, progressWidth, contentBounds);
			if (!ShouldShowPercentCompleteBar(viewInfo.Options.PercentCompleteDisplayType))
				return;
			int progressValue = viewInfo.Appointment.PercentComplete;
			if (progressWidth > AppointmentProcessValues.Min) {
				Rectangle progressBounds = new Rectangle(contentBounds.X, contentBounds.Y, progressWidth, contentBounds.Height);
				Color progressColor = GetBackColorForProgress(cache);
				cache.Progress = ModifySkinElement(cache.Content, progressBounds, progressColor, viewInfo.Selected);
				cache.LeftBorder = ModifySkinElement(cache.LeftBorder, cache.LeftBorder.Bounds, progressColor, viewInfo.Selected);
				if (progressValue == AppointmentProcessValues.Max)
					cache.RightBorder = ModifySkinElement(cache.RightBorder, cache.RightBorder.Bounds, progressColor, viewInfo.Selected);
			}
		}
		protected internal virtual bool ShouldShowPercentCompleteBar(PercentCompleteDisplayType type) {
			return type == PercentCompleteDisplayType.Both || type == PercentCompleteDisplayType.BarProgress;
		}
		protected internal virtual Color GetBackColorForProgress(AppointmentViewInfoSkinElementInfoCache cache) {
			Images elElementImageGetImagesImages = cache.Content.Element.Image.GetImages().Images;
			Color resultColor;
			if (elElementImageGetImagesImages.Count != 0) {
				Image image = elElementImageGetImagesImages[cache.Content.ImageIndex];
				Color imageColor = (new Bitmap(image)).GetPixel(image.Width / 2, image.Height / 2);
				resultColor = GanttViewAppointmentPainter.GetBackColorForPercentComplete(imageColor);
			} else {
				resultColor = Color.FromArgb(200, 200, 200, 200);
			}
			return resultColor;
		}
	}
}
