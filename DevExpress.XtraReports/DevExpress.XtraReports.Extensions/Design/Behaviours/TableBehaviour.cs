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
using System.Text;
using DevExpress.XtraReports.UI;
using System.Drawing;
using DevExpress.XtraPrinting;
using System.Collections;
using System.ComponentModel.Design;
namespace DevExpress.XtraReports.Design.Behaviours {
	class TableDesignerBehaviour : DesignerBehaviour {
		#region inner classes
		class TableBoundsComparer : IComparer, IComparer<XRTable> {
			IBandViewInfoService svc;
			public TableBoundsComparer(IBandViewInfoService svc) {
				this.svc = svc;
			}
			public int Compare(object x, object y) {
				BandViewInfo viewInfoX = GetBandViewInfo(x as XRTable);
				BandViewInfo viewInfoY = GetBandViewInfo(y as XRTable);
				return (viewInfoX != null && viewInfoY != null) ?
					viewInfoX.Bounds.Top - viewInfoY.Bounds.Top : 0;
			}
			BandViewInfo GetBandViewInfo(XRTable table) {
				if(table != null) {
					for(int i = 0; i < svc.ViewInfos.Length; i++) {
						BandViewInfo viewInfo = svc.ViewInfos[i];
						if(viewInfo.Band.Equals(table.Band))
							return viewInfo;
					}
				}
				return null;
			}
			int IComparer<XRTable>.Compare(XRTable x, XRTable y) {
				return Compare(x, y);
			}
		}
		#endregion
		static Size defaultTableSize = new Size(300, 23);
		XRTable Table { get { return XRControl as XRTable; } }
		new XRTableDesigner Designer {
			get { return (XRTableDesigner)base.Designer; }
		}
		public TableDesignerBehaviour(IServiceProvider servProvider)
			: base(servProvider) {
		}
		public override void SetDefaultComponentBounds() {
			CapturePaintService captPaintSvc = (CapturePaintService)GetService(typeof(CapturePaintService));
			RectangleF bounds = captPaintSvc.DragBounds;
			captPaintSvc.DragBounds = Rectangle.Empty;
			if(Table.Parent != null) {
				RectangleF  tableBounds = GetControlBounds(Table.Parent, Table.BoundsF, bounds, true);
				Designer.CreateDefaultTable(tableBounds);
			} else
				Designer.CreateDefaultTable(RectangleF.Empty);
			if(BandViewSvc != null) {
				if(bounds.Size.IsEmpty) 
					bounds.Size = XRConvert.Convert(defaultTableSize, XRControl.Dpi, GraphicsDpi.Pixel);
				List<XRTable> tables = CreateTables(BandViewSvc, bounds);
				Designer.AddTablesToContainer(tables);
				if(tables.Count > 0) {
					tables.Add(Table);
					tables.Sort(new TableBoundsComparer(BandViewSvc));
					AnchorTables(tables);
					tables.Clear();
				}
			}
		}
		static void AnchorTables(List<XRTable> tables) {
			if(tables.Count == 1)
				return;
			tables[0].AnchorVertical = VerticalAnchorStyles.Bottom;
			for(int i = 1; i < tables.Count - 1; i++)
				tables[i].AnchorVertical = VerticalAnchorStyles.Both;
			foreach(XRTable table in tables)
				table.UpdatedBounds();
		}
		List<XRTable> CreateTables(IBandViewInfoService svc, RectangleF bounds) {
			List<XRTable> tables = new List<XRTable>();
			if(!(Table.Parent is Band) || bounds.Location.IsEmpty) return tables;
			foreach(BandViewInfo viewInfo in svc.ViewInfos) {
				if(viewInfo.Band is XtraReportBase || Comparer.Equals(Table.Parent, viewInfo.Band)) continue;
				RectangleF bandBounds = svc.GetControlScreenBounds(viewInfo.Band);
				if(bandBounds.IntersectsWith(bounds)) {
					RectangleF rect = GetControlBounds(viewInfo.Band, new Rectangle(0, 0, XRControl.DefaultWidth, XRControl.DefaultHeight), bounds, true);
					if(viewInfo == svc.ViewInfos[svc.ViewInfos.Length - 1]) {
						RectangleF lastTableBounds = new RectangleF(bounds.X, bandBounds.Y, bounds.Width, bounds.Bottom - bandBounds.Top);
						rect = GetControlBounds(viewInfo.Band, new Rectangle(0, 0, XRControl.DefaultWidth, XRControl.DefaultHeight), lastTableBounds, false);
					}
					XRTable table = XRTable.CreateTable(XRConvert.Convert(rect, XRControl.Dpi, GraphicsDpi.HundredthsOfAnInch), 1, 3);
					table.Parent = viewInfo.Band;
					tables.Add(table);
				}
			}
			return tables;
		}
	}
}
