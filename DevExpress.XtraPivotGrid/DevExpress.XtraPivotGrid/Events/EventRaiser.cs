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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPivotGrid.Events;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraPivotGrid {
	class PivotGridEventRaiser : PivotGridEventRaiserBase, IPivotGridEventsImplementor {
		public PivotGridEventRaiser(IPivotGridEventsImplementorBase eventsImplementor)
			: base(eventsImplementor) {
		}
		protected new IPivotGridEventsImplementor BaseImpl {
			get { return (IPivotGridEventsImplementor)base.BaseImpl; }
		}
		#region IPivotGridEventsImplementor Members
		void IPivotGridEventsImplementor.OnPopupMenuShowing(PopupMenuShowingEventArgs e) {
			EnsureIsNotRecording();
			BaseImpl.OnPopupMenuShowing(e);
		}
		void IPivotGridEventsImplementor.OnPopupMenuItemClick(PivotGridMenuItemClickEventArgs e) {
			EnsureIsNotRecording();
			BaseImpl.OnPopupMenuItemClick(e);
		}
		void IPivotGridEventsImplementor.FocusedCellChanged() {
			EnsureIsNotRecording();
			BaseImpl.FocusedCellChanged();
		}
		void IPivotGridEventsImplementor.CellSelectionChanged() {
			EnsureIsNotRecording();
			BaseImpl.CellSelectionChanged();
		}
		object IPivotGridEventsImplementor.CustomEditValue(object value, PivotGridCellItem cellItem) {
			EnsureIsNotRecording();
			return BaseImpl.CustomEditValue(value, cellItem);
		}
		RepositoryItem IPivotGridEventsImplementor.GetCellEdit(PivotGridCellItem cellItem) {
			return BaseImpl.GetCellEdit(cellItem);
		}
		void IPivotGridEventsImplementor.AsyncOperationStarting() {
			EnsureIsNotRecording();
			BaseImpl.AsyncOperationStarting();
		}
		void IPivotGridEventsImplementor.AsyncOperationCompleted() {
			BaseImpl.AsyncOperationCompleted();
		}
		bool IPivotGridEventsImplementor.ShowingCustomizationForm(Form customizationForm, ref Control parentControl) {
			return BaseImpl.ShowingCustomizationForm(customizationForm, ref parentControl);
		}
		void IPivotGridEventsImplementor.ShowCustomizationForm() {
			BaseImpl.ShowCustomizationForm();
		}
		void IPivotGridEventsImplementor.HideCustomizationForm() {
			BaseImpl.HideCustomizationForm();
		}
		void IPivotGridEventsImplementor.CellDoubleClick(PivotCellViewInfo cellViewInfo) {
			if(Mode == EventRaiserMode.Record)
				return;
			BaseImpl.CellDoubleClick(cellViewInfo);
		}
		void IPivotGridEventsImplementor.CellClick(PivotCellViewInfo cellViewInfo) {
			if(Mode == EventRaiserMode.Record)
				return;
			BaseImpl.CellClick(cellViewInfo);
		}
		int IPivotGridEventsImplementor.GetCustomRowHeight(PivotFieldValueItem item, int height) {
			if(Mode == EventRaiserMode.Record)
				return -1;
			return BaseImpl.GetCustomRowHeight(item, height);
		}
		int IPivotGridEventsImplementor.GetCustomColumnWidth(PivotFieldValueItem item, int width) {
			if(Mode == EventRaiserMode.Record)
				return -1;
			return BaseImpl.GetCustomColumnWidth(item, width);
		}
		int IPivotGridEventsImplementor.FieldValueImageIndex(PivotFieldValueItem item) {
			return BaseImpl.FieldValueImageIndex(item);
		}
		bool IPivotGridEventsImplementor.CustomDrawHeaderArea(PivotHeadersViewInfoBase headersViewInfo, ViewInfoPaintArgs paintArgs, Rectangle bounds, MethodInvoker defaultDraw) {
			return BaseImpl.CustomDrawHeaderArea(headersViewInfo, paintArgs, bounds, defaultDraw);
		}
		bool IPivotGridEventsImplementor.CustomDrawEmptyArea(IPivotCustomDrawAppearanceOwner appearanceOwner, ViewInfoPaintArgs paintArgs, Rectangle bounds, MethodInvoker defaultDraw) {
			return BaseImpl.CustomDrawEmptyArea(appearanceOwner, paintArgs, bounds, defaultDraw);
		}
		bool IPivotGridEventsImplementor.CustomDrawFieldHeader(PivotHeaderViewInfoBase headerViewInfo, ViewInfoPaintArgs paintArgs, HeaderObjectPainter painter, MethodInvoker defaultDraw) {
			return BaseImpl.CustomDrawFieldHeader(headerViewInfo, paintArgs, painter, defaultDraw);
		}
		bool IPivotGridEventsImplementor.CustomDrawFieldValue(ViewInfoPaintArgs paintArgs, PivotFieldsAreaCellViewInfo fieldCellViewInfo, PivotHeaderObjectInfoArgs info, PivotHeaderObjectPainter painter, MethodInvoker defaultDraw) {
			return BaseImpl.CustomDrawFieldValue(paintArgs, fieldCellViewInfo, info, painter, defaultDraw);
		}
		bool IPivotGridEventsImplementor.CustomDrawCell(ViewInfoPaintArgs paintArgs, ref AppearanceObject appearance, PivotCellViewInfo cellViewInfo, MethodInvoker defaultDraw) {
			return BaseImpl.CustomDrawCell(paintArgs, ref appearance, cellViewInfo, defaultDraw);
		}
		void IPivotGridEventsImplementor.CustomAppearance(ref AppearanceObject appearance, PivotGridCellItem cellItem, Rectangle? bounds) {
			BaseImpl.CustomAppearance(ref appearance, cellItem, bounds);
		}
		void IPivotGridEventsImplementor.LeftTopCellChanged(Point oldValue, Point newValue) {
			BaseImpl.LeftTopCellChanged(oldValue, newValue);
		}
		#endregion
	}
}
