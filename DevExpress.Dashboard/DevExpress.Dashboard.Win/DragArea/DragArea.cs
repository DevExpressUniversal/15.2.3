#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.DragDrop;
using DevExpress.DashboardWin.Localization;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
namespace DevExpress.DashboardWin.Native {
	public class DragArea : IDisposable {
		readonly DragAreaControl parentControl;
		readonly DragAreaScrollableControl scrollableControl;
		readonly DashboardDesigner designer;
		readonly List<DragSection> sections = new List<DragSection>();
		string warningMessage;
		Rectangle bounds;
		public DataDashboardItem DashboardItem { get { return parentControl.DashboardItem; } }
		public IList<DragSection> Sections { get { return sections; } }
		protected internal DragAreaControl ParentControl { get { return parentControl; } }
		public event EventHandler Invalidated;
		public DragArea(DashboardDesigner designer, DragAreaScrollableControl scrollableControl, DragAreaControl parentControl) {
			this.designer = designer;
			this.scrollableControl = scrollableControl;
			this.parentControl = parentControl;
		}
		public void Invalidate() {
			parentControl.Invalidate();
			if(Invalidated != null)
				Invalidated(this, EventArgs.Empty);
		}
		public void Arrange() {
			using (Graphics gr = Graphics.FromHwnd(parentControl.Handle))
				using (GraphicsCache cache = new GraphicsCache(gr))
					Arrange(cache);			
		}
		public void SetDataSource(IDataSourceSchema dataSource) {
			foreach (DragSection section in sections)
				section.SetDataSourceSchema(dataSource);
		}
		void Arrange(GraphicsCache cache) {
			if (parentControl.ArrangeLocked)
				return;
			if (Sections.Count != 0) {
				warningMessage = String.Empty;
				DragAreaDrawingContext drawingContext = parentControl.DrawingContext;
				IDragAreaPainter areaPainter = drawingContext.Painters.AreaPainter;
				ObjectPainter painter = areaPainter.ObjectPainter;
				int sectionIndent = areaPainter.SectionIndent;
				StyleObjectInfoArgs args = new StyleObjectInfoArgs(cache, new Rectangle(Point.Empty, new Size(drawingContext.SectionWidth, 0)), drawingContext.Appearances.AreaAppearance);
				Point sectionLocation = painter.GetObjectClientRectangle(args).Location;
				Rectangle clientBounds = Rectangle.Empty;
				foreach (DragSection section in sections) {
					Rectangle sectionRect = section.Arrange(drawingContext, cache, sectionLocation);
					clientBounds = clientBounds.IsEmpty ? sectionRect : Rectangle.Union(clientBounds, sectionRect);
					sectionLocation.Y += sectionRect.Height + sectionIndent;
				}
				args.Bounds = clientBounds;
				bounds = painter.CalcBoundsByClientRectangle(args);
				if (parentControl.Size != bounds.Size)
					parentControl.Size = bounds.Size;
			}
			else {
				parentControl.Size = scrollableControl.Size;
				DashboardItem item = designer.SelectedDashboardItem;
				if (item != null)
					warningMessage = String.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.DragAreaDashboardItemHasNotDataItemsMessage), item.Name);
				else if (designer.Dashboard != null && designer.Dashboard.Items.Count > 0)
					warningMessage = DashboardWinLocalizer.GetString(DashboardWinStringId.DragAreaHasNotDashboardItemMessage);
				else
					warningMessage = String.Empty;
			}
		}
		public void Paint(PaintEventArgs e) {
			using (GraphicsCache cache = new GraphicsCache(e)) {
				if (Sections.Count != 0) {
					DragAreaDrawingContext drawingContext = parentControl.DrawingContext;
					ObjectPainter.DrawObject(cache, drawingContext.Painters.AreaPainter.ObjectPainter, new StyleObjectInfoArgs(cache, bounds, drawingContext.Appearances.AreaAppearance));
					foreach (DragSection section in sections)
						section.Paint(drawingContext, cache);
				}
				else
					WarningRenderer.MessageRenderer(e, new Rectangle(0, 0, scrollableControl.Width, scrollableControl.Height), warningMessage, designer.LookAndFeel);
			}
		}
		public DragAreaSelection GetSelection(Point point) {
			foreach (DragSection section in sections) {
				DragAreaSelection selection = section.GetSelection(point);
				if (selection != null)
					return selection;
			}
			return new DragAreaSelection(DragAreaSelectionType.None);
		}
		public IDropAction GetDropAction(Point point, IDragObject dragObject) {
			foreach (DragSection section in sections) {
				IDropAction dropAction = section.GetDropAction(point, dragObject);
				if (dropAction != null)
					return dropAction;
			}
			return null;
		}
		void ClearSections() {
			foreach (DragSection section in sections)
				section.Dispose();
			sections.Clear();
		}
		public void UpdateSections() {
			ClearSections();
			DataDashboardItem dashboardItem = DashboardItem;
			if(dashboardItem != null)
				foreach(DataDashboardItemDescription description in dashboardItem.Descriptions) {
					string caption = description.Name;
					DragSection section = null;
					switch(description.ItemKind) {
						case ItemKind.ChartHeader:
							section = new ChartHeaderDragSection(this, caption);
							break;
						case ItemKind.Header:
							section = new HeaderDragSection(this, caption);
							break;
						case ItemKind.Splitter:
							section = new SplitterDragSection(this);
							break;
						case ItemKind.SingleNumericMeasure:
							section = new SingleGroupDragSection(this, caption, description.ItemName, new MeasureHolderDragGroup(null, (IDataItemHolder)description.Data));
							break;
						case ItemKind.SingleDimension:
							section = new SingleGroupDragSection(this, caption, description.ItemName, new DimensionHolderDragGroup(null, (IDataItemHolder)description.Data));
							break;
						case ItemKind.Dimension:
							section = new SingleGroupDragSection(this, caption, description.ItemName, description.ItemNamePlural, new DimensionCollectionDragGroup((IDataItemCollection<Dimension>)description.Data));
							break;
						 case ItemKind.NumericMeasure:
							section = new SingleGroupDragSection(this, caption, description.ItemName, new MeasureCollectionDragGroup((IDataItemCollection<Measure>)description.Data, true));
							break;
						case ItemKind.Measure:
							section = new SingleGroupDragSection(this, caption, description.ItemName, new MeasureCollectionDragGroup((IDataItemCollection<Measure>)description.Data, false));
							break;
						case ItemKind.ChartPane:
							ChartPaneDescription paneDescription = (ChartPaneDescription)description.Data;
							if(paneDescription.Pane != null || (paneDescription.Pane == null && !parentControl.Designer.IsDashboardVSDesignMode))
								section = new ChartSeriesDragSection(this, caption, description.ItemName, paneDescription);
							break;
						case ItemKind.RangeFilterSeries:
							section = new RangeFilterSeriesDragSection(this, caption, description.ItemName, (RangeFilterSeriesDescription)description.Data);
							break;
						case ItemKind.Gauge:
							section = new GaugeDragSection(this, caption, description.ItemName, (GaugeCollection)description.Data);
							break;
						case ItemKind.Card:
							section = new CardDragSection(this, caption, description.ItemName, (CardCollection)description.Data);
							break;
						case ItemKind.GridColumn:
							section = new GridColumnDragSection(this, caption, description.ItemName, description.ItemNamePlural, (GridColumnCollection)description.Data);
							break;
						case ItemKind.MapAttribute:
							section = new MapAttributeSection(this, caption, description.ItemName, (IDataItemHolder)description.Data);
							break;
						case ItemKind.ChoroplethMap:
							section = new ChoroplethMapDragSection(this, caption, description.ItemName, (ChoroplethMapCollection)description.Data);
							break;
						case ItemKind.GeoPointColor:
							section = new BubbleMapDragSection(this, caption, description.ItemName, (IDataItemHolder)description.Data);
							break;
					}
					if(section != null) {
						section.Initialize();
						sections.Add(section);
					}
				}
			Arrange();
			parentControl.PerformLayout();
			parentControl.Refresh();
		}
		public void ClearSectionsContent() {
			DragAreaHistoryItem historyItem = new DragAreaHistoryItem(DashboardItem, DashboardWinStringId.HistoryItemModifyBindings);
			foreach(DragSection section in sections)
				section.ClearContent(historyItem);
			historyItem.Redo(ParentControl.Designer);
			ParentControl.Designer.History.Add(historyItem);
		}
		void OnDescriptionsChanged(object sender, EventArgs e) {
			UpdateSections();
		}
		void OnLoadCompleted(object sender, EventArgs e) {
			UpdateSections();
		}
		public void UnsubscribeDashboardItemEvents() {
			if(DashboardItem != null) {
				DashboardItem.DescriptionsChanged -= OnDescriptionsChanged;
				DashboardItem.LoadCompleted -= OnLoadCompleted;
			}
		}
		public void SubscribeDashboardItemEvents() {
			if(DashboardItem != null) {
				DashboardItem.DescriptionsChanged += OnDescriptionsChanged;
				DashboardItem.LoadCompleted += OnLoadCompleted;
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				UnsubscribeDashboardItemEvents();
				ClearSections();
			}
		}
	}
}
