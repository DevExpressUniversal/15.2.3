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
using System.Drawing;
using DevExpress.Utils.Gesture;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Services;
namespace DevExpress.XtraScheduler {
	public partial class SchedulerControl : IGestureClient {
		const double DefaultIntermediateZoom = 1.0;
		const double DefaultZoomDeviation = 0.35;
		const double RequiredZoomDistance = 100.0;
		int panX;
		int panY;
		GestureHelper GestureHelper { get; set; }
		IntPtr IGestureClient.Handle { get { return this.Handle; } }
		double intermediateZoom = DefaultIntermediateZoom;
		double zoomDeviation = DefaultZoomDeviation;
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			GestureHelper.PanWithGutter = true;
			List<GestureAllowArgs> result = new List<GestureAllowArgs>();
			if (!ClientBounds.Contains(point) || !OptionsBehavior.TouchAllowed)
				return result.ToArray();
			result.Add(GestureAllowArgs.PressAndTap);
			result.Add(GestureAllowArgs.Zoom);
			result.Add(GestureAllowArgs.Pan);
			result.Add(GestureAllowArgs.Rotate);
			return result.ToArray();
		}
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) {
			if (!OptionsBehavior.TouchAllowed)
				return;
			double diagonal = Math.Sqrt(this.ClientSize.Width * this.ClientSize.Width + this.ClientSize.Height * this.ClientSize.Height);
			zoomDeviation = 1.0 - (Math.Max((diagonal - RequiredZoomDistance), 0.0) / diagonal);
			this.intermediateZoom = DefaultIntermediateZoom;
		}
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			if (!OptionsBehavior.TouchAllowed)
				return;
			if (info.IsBegin) {
				this.panX = 0;
				this.panY = 0;
			}
			if (delta.IsEmpty)
				return;
			BeginUpdate();
			try {
				panX += delta.X;
				panY += delta.Y;
				ScrollByPhysicalOffsetCommandBase verticalScrollCommand = new ScrollVerticallyByPhysicalOffsetCommand(this);
				verticalScrollCommand.PhysicalOffset = panY;
				verticalScrollCommand.Execute();
				if (verticalScrollCommand.ScrolledOffset != 0)
					panY += verticalScrollCommand.ScrolledOffset;
				ScrollHorizontallyByPhysicalOffsetCommand horizontalScrollCommand = new ScrollHorizontallyByPhysicalOffsetCommand(this);
				horizontalScrollCommand.PhysicalOffset = panX;
				horizontalScrollCommand.Execute();
				if (horizontalScrollCommand.ScrolledOffset != 0)
					panX += horizontalScrollCommand.ScrolledOffset;
			} finally {
				EndUpdate();
			}
		}
		void IGestureClient.OnPressAndTap(GestureArgs info) {
		}
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) {
		}
		void IGestureClient.OnTwoFingerTap(GestureArgs info) {
		}
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) {
			if (!OptionsBehavior.TouchAllowed)
				return;
			this.intermediateZoom += (zoomDelta - 1);
			SchedulerCommandId commandId = SchedulerCommandId.None;
			if (this.intermediateZoom > (DefaultIntermediateZoom + zoomDeviation)) {
				this.intermediateZoom = DefaultIntermediateZoom;
				this.zoomDeviation = DefaultZoomDeviation;
				commandId = SchedulerCommandId.SwitchToMoreDetailedView;
			} else if (this.intermediateZoom < (DefaultIntermediateZoom - zoomDeviation)) {
				this.intermediateZoom = DefaultIntermediateZoom;
				this.zoomDeviation = DefaultZoomDeviation;
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
		IntPtr IGestureClient.OverPanWindowHandle { get { return GestureHelper.FindOverpanWindow(this); } }
		Point IGestureClient.PointToClient(Point p) {
			return this.PointToClient(p);
		}
	}
}
