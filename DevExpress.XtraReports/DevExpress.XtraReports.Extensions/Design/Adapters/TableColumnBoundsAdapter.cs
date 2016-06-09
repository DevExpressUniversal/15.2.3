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

using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraReports.Design.Adapters {
	class TableColumnBoundsAdapter : BoundsAdapter {
		XRControl[] controls;
		ResizeBehaviour resizeMode;
		XRTableCell MainCell {
			get { return (XRTableCell)XRControl; }
		}
		public TableColumnBoundsAdapter(XRControl xrControl, IServiceProvider servProvider)
			: base(xrControl, servProvider) {
		}
		public TableColumnBoundsAdapter(XRControl[] controls, IServiceProvider servProvider, bool isShiftPressed)
			: this(controls[0], servProvider) {
			this.controls = controls;
			this.resizeMode = isShiftPressed ? ResizeBehaviour.SpecifiedMode : ResizeBehaviour.ProportionalMode;
		}
		public override void SetScreenBounds(RectangleF screenRect) {
			float diff = this.ZoomService.FromScaledPixels(screenRect.Width, XRControl.Dpi) - MainCell.WidthF;
			float initialPositionX = screenRect.X;
			IBoundsAdapter bandAdapter = GetControlAdapter(XRControl.Band);
			RectangleF bandRect = bandAdapter.GetScreenBounds();
			screenRect.Offset(-bandRect.X, -bandRect.Y);
			RectangleF rect = XRControl.Parent.RectangleFFromBand(this.ZoomService.FromScaledPixels(screenRect, XRControl.Dpi));
			bool needResizeLeft = !DevExpress.XtraPrinting.Native.FloatsComparer.Default.FirstEqualsSecond(MainCell.LeftF, rect.X);
			foreach(XRTableCell cell in controls) {
				TableCellBoundsAdapter adapter = (TableCellBoundsAdapter)BoundsAdapterService.GetAdapter(cell, servProvider);
				RectangleF newBounds = cell.BoundsF;
				newBounds.Width = screenRect.Width;
				newBounds.X = initialPositionX;
				if(resizeMode == ResizeBehaviour.ProportionalMode)
					adapter.SetProportionalBounds(newBounds);
				else
					adapter.SetSpecifiedBounds(newBounds);
			}
			if(resizeMode != ResizeBehaviour.ProportionalMode) {
				if(needResizeLeft) {
					MainCell.Row.Table.LeftF -= diff;
				}
				MainCell.Row.Table.WidthF += diff;
			}
		}
	}
}
