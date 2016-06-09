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

using System.Drawing;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.NativeBricks {
	[BrickExporter(typeof(XETextPanelBrickExporter))]
	public class XETextPanelBrick : PanelBrick {
		public override string BrickType { get { return BrickTypes.XETextPanel; } }
		public XETextPanelBrick() : base() { }
		public XETextPanelBrick(BrickStyle style) : base(style) { }
		XETextPanelBrick(XETextPanelBrick panelBrick) : base(panelBrick) { }
		protected internal override void OnAfterMerge() {
			RectangleF rect = GetChildrenRegion();
			float dy = (InitialRect.Height - rect.Height) / 2 - rect.Y;
			foreach(Brick brick in Bricks) {
				brick.InitialRect = RectF.Offset(brick.InitialRect, 0, dy);
			}
		}
		RectangleF GetChildrenRegion() {
			RectangleF rect = RectangleF.Empty;
			for(int i = 0; i < Bricks.Count; i++) {
				if(i == 0) rect = Bricks[i].Rect;
				else rect = RectangleF.Union(rect, Bricks[i].Rect);
			}
			return rect;
		}
		#region ICloneable Members
		public override object Clone() {
			PanelBrick panel = new XETextPanelBrick(this);
			foreach(Brick brick in Bricks) {
				panel.Bricks.Add((Brick)brick.Clone());
			}
			return panel;
		}
		#endregion
	}
	class XETextPanelBrickExporter : PanelBrickExporter {
		internal new PanelBrick Brick { get { return base.Brick as PanelBrick; } }
		protected internal override BrickViewData[] GetExportData(ExportContext exportContext, RectangleF rect, RectangleF clipRect) {
			if(!VisualBrick.IsVisible)
				return new BrickViewData[0];
			if(exportContext is XlsExportContext || exportContext is TextExportContext) {
				return exportContext.CreateBrickViewDataArray(Style, rect, TableCell);
			}
			return base.GetExportData(exportContext, rect, clipRect);
		}
		protected internal override void FillXlsTableCellInternal(IXlsExportProvider exportProvider) {
			TextBrick textBrick = FindTextBrick();
			exportProvider.SetCellText(textBrick != null ? textBrick.Text : Brick.Value, null);
		}
		protected internal override void FillTextTableCellInternal(ITableExportProvider exportProvider, bool shouldSplitText) {
			TextBrick textBrick = FindTextBrick();
			if(textBrick != null) {
				BrickExporter exporter = (BrickExporter)GetExporter(exportProvider.ExportContext, textBrick);
				exporter.FillTextTableCell(exportProvider, shouldSplitText);
			} else
				exportProvider.SetCellText(Brick.Value, null);
		}
		TextBrick FindTextBrick() {
			foreach(Brick brick in Brick.Bricks) {
				TextBrick result = brick as TextBrick;
				if(result != null)
					return result;
			}
			return null;
		}
	}
}
