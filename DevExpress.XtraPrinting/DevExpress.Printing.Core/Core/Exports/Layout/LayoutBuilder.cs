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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export.Rtf;
using System.Collections.Generic;
using System.Drawing.Printing;
namespace DevExpress.XtraPrinting.Export {
	public interface ILayoutBuilder {
		LayoutControlCollection BuildLayoutControls();
	}
	public abstract class LayoutBuilder : ILayoutBuilder, IBrickExportVisitor {
		Document document;
		protected LayoutBuilder(Document document) {
			this.document = document;
		}
		LayoutControlCollection layoutControls;
		ContinuousExportInfo exportInfo;
		int offsetYMult;
		internal Margins PageMargins { get { return exportInfo.PageMargins; } }
		internal Rectangle PageBounds { get { return exportInfo.PageBounds; } }
		internal float BottomMarginOffset { get { return exportInfo.BottomMarginOffset; } }
		internal ICollection PageBreakPositions { get { return exportInfo.PageBreakPositions; } }
		internal ICollection MultiColumnInfo { get { return exportInfo.MultiColumnInfo; } }
		public LayoutControlCollection BuildLayoutControls() {
			layoutControls = new LayoutControlCollection();
			exportInfo = document.GetContinuousExportInfo();
			exportInfo.ExecuteExport(this, document.PrintingSystem);
			return layoutControls;
		}
		ILayoutControl[] GetLayoutControlInPixels(Brick brick, double horizontalOffset, double verticalOffset) {
			RectangleDF brickRect = RectangleDF.FromRectangleF(brick.InitialRect);
			if(!brickRect.IsEmpty) {
				brickRect.X = horizontalOffset;
				brickRect.Y = verticalOffset;
				brickRect = GraphicsUnitConverter.Convert(brickRect, GraphicsDpi.Document, GraphicsDpi.DeviceIndependentPixel);
				offsetYMult = (int)(((long)brickRect.Y) >> 13);
				brickRect.Y -= (double)(offsetYMult << 13);
				ILayoutControl[] result = GetBrickLayoutControls(brick, brickRect.ToRectangleF());
				offsetYMult = 0;
				return result;
			}
			return null;
		}
		void IBrickExportVisitor.ExportBrick(double horizontalOffset, double verticalOffset, Brick brick) {
			ILayoutControl[] data = GetLayoutControlInPixels(brick, horizontalOffset, verticalOffset);
			if(data != null)
				layoutControls.AddRange(data);
		}
		protected abstract ILayoutControl[] GetBrickLayoutControls(Brick brick, RectangleF rect);
		protected ILayoutControl[] ToLayoutControls(BrickViewData[] data, Brick brick) {
			if(data != null) {
				ILayoutControl[] layoutData = new ILayoutControl[data.Length];
				for(int i = 0; i < data.Length; i++) {
					data[i].SetOffesetY(offsetYMult);
					layoutData[i] = CreateLayoutControl(data[i], brick);
				}
				return layoutData;
			}
			return null;
		}
		protected virtual ILayoutControl CreateLayoutControl(BrickViewData data, Brick brick) {
			return LayoutControl.Validate(data);
		}
	}
}
