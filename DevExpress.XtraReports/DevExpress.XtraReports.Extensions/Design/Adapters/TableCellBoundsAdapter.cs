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

using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraReports.Design.Adapters {
	class TableCellBoundsAdapter : BoundsAdapter {
		XRTableCell Cell {
			get { return (XRTableCell)XRControl; }
		}
		XRTable Table {
			get { return ((XRTableCell)XRControl).Row.Table; }
		}
		XRTableCellDesigner XRTableCellDesigner {
			get { return (XRTableCellDesigner)Designer; }
		}
		XRTableDesigner XRTableDesigner {
			get { return (XRTableDesigner)Host.GetDesigner(Table); }
		}
		public TableCellBoundsAdapter(XRTableCell xrControl, IServiceProvider servProvider)
			: base(xrControl, servProvider) {
		}
		public override RectangleF GetClientBandBounds(BandViewInfo viewInfo) {
			if(viewInfo == null) return Rectangle.Empty;
			RectangleF result = base.GetClientBandBounds(viewInfo);
			if(!XRTableDesigner.MergedCells.ContainsKey(Cell)) return result;
			if(Cell.RowSpan < 2) return RectangleF.Empty;
			result.Height = this.ZoomService.ToScaledPixels(RowSpanHelper.CalculateMergedCellHeight(Cell, XRTableDesigner.MergedCells), Cell.Dpi);
			return result;
		}
		public override void SetScreenBounds(RectangleF screenRect) {
			SetScreenBounds(screenRect, ResizeBehaviour.DefaultMode);
		}
		public void SetSpecifiedBounds(RectangleF screenRect) {
			SetScreenBounds(screenRect, ResizeBehaviour.SpecifiedMode);
		}
		public void SetProportionalBounds(RectangleF screenRect) {
			SetScreenBounds(screenRect, ResizeBehaviour.ProportionalMode);
		}
		void SetScreenBounds(RectangleF screenRect, ResizeBehaviour resizeMode) {
			IBoundsAdapter bandAdapter = GetControlAdapter(XRControl.Band);
			RectangleF bandRect = bandAdapter.GetScreenBounds();
			screenRect.Offset(-bandRect.X, -bandRect.Y);
			RectangleF rect = XRControl.Parent.RectangleFFromBand(this.ZoomService.FromScaledPixels(screenRect, XRControl.Dpi));
			if(!XRTableDesigner.MergedCells.ContainsKey(Cell)) {
				SetCellBounds(XRTableCellDesigner, resizeMode, rect);
			} else {
				object key;
				XRTableDesigner.MergedCells.TryGetValue(Cell, out key);
				IEnumerable selection = XRTableDesigner.MergedCells.Where(x => x.Value == key).ToList();
				float availableWidth = float.MaxValue;
				bool isIncrease = rect.Width > Cell.BoundsF.Width;
				if(isIncrease) {
					foreach(KeyValuePair<XRTableCell, object> pair in selection) {
						float width = pair.Key.GetMaxAvailableWidth(rect.X, rect.Width);
						if(availableWidth > width) availableWidth = width;
					}
				}
				foreach(KeyValuePair<XRTableCell, object> pair in selection) {
					rect = this.ZoomService.FromScaledPixels(screenRect, pair.Key.Dpi);
					XRTableCellDesigner designer = (XRTableCellDesigner)Host.GetDesigner(pair.Key);
					{
						rect = pair.Key.Parent.RectangleFFromBand(rect);
						rect.Width = Math.Min(availableWidth, rect.Width);
						SetCellBounds(designer, resizeMode, rect);
					}
				}
			}
		}
		void SetCellBounds(XRTableCellDesigner designer, ResizeBehaviour resizeMode, RectangleF rect) {
			switch(resizeMode) {
				case ResizeBehaviour.ProportionalMode:
					designer.SetProportionalCellBounds(rect);
					break;
				case ResizeBehaviour.SpecifiedMode:
					designer.SetSpecifiedCellBounds(rect);
					break;
				case ResizeBehaviour.DefaultMode:
					designer.SetBounds(rect);
					break;
			}
		}
	}
}
