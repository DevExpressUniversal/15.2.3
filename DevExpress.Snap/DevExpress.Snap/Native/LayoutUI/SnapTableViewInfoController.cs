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
using System.Windows.Forms;
using System.Collections.Generic;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Native.HoverDecorators;
using DevExpress.Snap.Core.Native.MouseHandler;
using DevExpress.Office.Drawing;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.Utils;
namespace DevExpress.Snap.Native.LayoutUI {
	public struct DraggedFieldDataInfo {
		readonly string dataSource;
		readonly string dataMember;
		readonly string[] escapedDataPaths;
		readonly bool isParameter;
		public DraggedFieldDataInfo(string dataSource, string dataMember, string[] escapedDataPaths, bool isParameter) {
			Guard.ArgumentNotNull(escapedDataPaths, "escapedDataPaths");
			this.dataSource = dataSource;
			this.dataMember = dataMember;
			this.escapedDataPaths = escapedDataPaths;
			this.isParameter = isParameter;
		}
		public string DataSource { get { return dataSource; } }
		public string DataMember { get { return dataMember; } }
		public bool IsParameter { get { return isParameter; } }
		internal string[] EscapedDataPaths { get { return escapedDataPaths; } }
	}
	public class DragExternalContentTableViewInfoController : TableViewInfoController, IDropFieldTarget {
		readonly Dictionary<Page, HotZoneCollection> hotZones;
		RichEditView view;
		PageViewInfo pageViewInfo;
		DraggedFieldDataInfo[] draggedFields;
		public DragExternalContentTableViewInfoController(RichEditView view, Point mousePosition)
			: base(mousePosition) {
			this.view = view;
			this.hotZones = new DropFieldTableHotZoneCalculator().CalculateHotZones(view);
		}
		public HotZone VisibleHotZone { get; protected internal set; }
		protected internal Dictionary<Page, HotZoneCollection> HotZones { get { return hotZones; } }
		protected internal DraggedFieldDataInfo[] DraggedFields { get { return draggedFields; } }
		protected DocumentModel DocumentModel { get { return view != null ? view.DocumentModel : null; } }
		protected RichEditView View { get { return view; } }
		protected PageViewInfo PageViewInfo { get { return pageViewInfo; } }
		protected Page ActivePage { get { return pageViewInfo != null ? pageViewInfo.Page : null; } }
		protected bool DropAllowed { get { return view.Control.InnerControl.Options.Behavior.DropAllowed; } }
		public override ITableViewInfoDecorator CreateDecorator(Painter painter) {
			return new DragExternalContentTableHotZonePainter(this, painter);
		}
		public ILayoutToPhysicalBoundsConverter CreateConverter() {
			return new LayoutToPhysicalBoundsConverter((SnapControl)view.Control, pageViewInfo);
		}
		public override void Update(Point physicalPoint, RichEditHitTestResult hitTestResult) {
			this.pageViewInfo = View.GetPageViewInfoFromPoint(physicalPoint, true);
			if (hitTestResult != null)
				MousePosition = hitTestResult.LogicalPoint;
		}
		protected override void OnMousePositionChanged() {
			if (ActivePage == null)
				return;
			HotZoneCollection activeHotZones = new HotZoneCollection();
			foreach (DropFieldTableHotZoneBase hotZone in HotZones[ActivePage])
				if (hotZone.IsAllowed)
					activeHotZones.Add(hotZone);
			VisibleDropFieldTableHotZoneCalculator calculator = new VisibleDropFieldTableHotZoneCalculator();
			VisibleHotZone = calculator.CalcVisibleHotZone(activeHotZones, MousePosition) as DropFieldTableHotZoneBase;
		}
		#region IDropFieldTarget implementation
		public void OnDragOver(DragEventArgs e) {
			if (draggedFields != null)
				return;
			if (e.Data.GetDataPresent(SnapDataFormats.SNDataInfo))
				OnDragOverCore((SNDataInfo[])e.Data.GetData(SnapDataFormats.SNDataInfo));
		}
		void OnDragOverCore(SNDataInfo[] data) {
			CalcDraggedFields(data);
			CheckHotZonesAvailability();
		}
		public bool DoDragDrop(DragEventArgs e) {
			if (!DropAllowed)
				return false;
			DropFieldTableHotZoneBase hotZone = VisibleHotZone as DropFieldTableHotZoneBase;
			if (hotZone == null)
				return false;
			hotZone.OnDragDrop(e);
			return true;
		}
		void CalcDraggedFields(SNDataInfo[] data) {
			SnapDocumentModel model = (SnapDocumentModel)View.DocumentModel;
			int length = data.Length;
			draggedFields = new DraggedFieldDataInfo[length];
			for (int i = 0; i < length; i++) {
				SNDataInfo info = data[i];
				string dataSource = model.DataSourceDispatcher.FindDataSourceName(info.Source);
				string dataMember = string.Empty;
				if (info.DataPaths.Length > 1) {
					string[] prevDataPaths = new string[info.EscapedDataPaths.Length - 1];
					Array.Copy(info.DataPaths, 0, prevDataPaths, 0, prevDataPaths.Length);
					dataMember = String.Join(".", prevDataPaths);
				}
				draggedFields[i] = new DraggedFieldDataInfo(dataSource, dataMember, info.EscapedDataPaths, info.Source is ParametersDataSource);
			}
		}
		void CheckHotZonesAvailability() {
			foreach (var item in HotZones)
				foreach (DropFieldTableHotZoneBase hotZone in item.Value)
					hotZone.CheckIsAllowed(DraggedFields);
		}
		#endregion
	}
}
