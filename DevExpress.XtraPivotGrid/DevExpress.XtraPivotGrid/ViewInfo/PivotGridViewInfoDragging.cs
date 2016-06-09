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
using DevExpress.XtraPivotGrid;
using DevExpress.Utils;
using DevExpress.Utils.DragDrop;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Utils.Internal;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	public delegate void DragCompletedCallback();
	public class PivotGridDragManager : DragManager {
		readonly PivotGridViewInfoData data;
		readonly PivotFieldItem field;
		DragIndicator dragIndicator;
		Point lastMovePt;
		DragState lastDragStateInternal;
		PivotDragHeaderViewInfo headerViewInfo;
		PivotDragHeaderViewInfo[] dragHeaders;
		DragCompletedCallback dragCompletedCallback;
		public PivotGridDragManager(PivotGridViewInfoData data, PivotFieldItem field)
			: this(data, field, null) {
		}
		public PivotGridDragManager(PivotGridViewInfoData data, PivotFieldItem field, DragCompletedCallback dragCompletedCallback) {
			this.data = data;
			this.field = field;
			LastMovePt = Point.Empty;
			this.lastDragStateInternal = DragState.None;
			this.dragCompletedCallback = dragCompletedCallback;
		}
		protected virtual DragState LastDragStateCore {
			get { return base.LastDragState; }
		}
		PivotGridControl Control { get { return Data.PivotGrid; } }
		protected internal Point LastMovePt { get { return lastMovePt; } set { lastMovePt = value; } }
		public PivotGridViewInfoData Data { get { return data; } }
		public PivotGridViewInfo ViewInfo { get { return Data.ViewInfo; } }
		public PivotFieldItem Field { get { return (PivotFieldItem)Data.GetFieldItem(field); } }
		public PivotDragHeaderViewInfo HeaderViewInfo {
			get {
				if(headerViewInfo == null)
					headerViewInfo = CreateHeaderViewInfo(Field);
				return headerViewInfo;
			}
		}
		public PivotDragHeaderViewInfo[] DragHeaders {
			get {
				if(dragHeaders == null)
					dragHeaders = CreateDragHeaders();
				return dragHeaders;
			}
		}
		public DragIndicator DragIndicator {
			get {
				if(dragIndicator == null)
					dragIndicator = DragIndicator.CreateDragIndicator(Control, Control.LookAndFeel);
				return dragIndicator;
			}
			set {
				if(dragIndicator == value) return;
				if(dragIndicator != null) 
					dragIndicator.Dispose();				
				dragIndicator = value;
			}
		}
		public override void Dispose() {
			base.Dispose();
			DragIndicator = null;
		}
		protected virtual PivotDragHeaderViewInfo CreateHeaderViewInfo(PivotFieldItem field) {
			Size size = new Size(ViewInfo.FieldMeasures.DefaultFieldWidth, ViewInfo.FieldMeasures.GetHeaderHeight(field));
			PivotDragHeaderViewInfo viewInfo = new PivotDragHeaderViewInfo(ViewInfo, field);
			viewInfo.Size = size;
			return viewInfo;
		}
		protected virtual PivotDragHeaderViewInfo[] CreateDragHeaders() {
			if(Field.Group == null) {
				return new PivotDragHeaderViewInfo[] { HeaderViewInfo };
			} else {
				int count = Field.Group.VisibleCount;
				PivotDragHeaderViewInfo[] headers = new PivotDragHeaderViewInfo[count];
				for(int i = 0; i < count; i++) {
					PivotGridField field = Data.GetField(Field) as PivotGridField;
					headers[i] = CreateHeaderViewInfo(Data.GetFieldItem(field.Group[i]) as PivotFieldItem);
				}
				return headers;
			}
		}
		protected virtual void DoDragDropCore() {
			base.DoDragDrop(HeaderSize, HeaderViewInfo.ControlBounds.Location);
		}
		public void DoDragDrop() {
			DoDragDropCore();
			DragIndicator = null;
			ViewInfo.MouseUp(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
			bool completeHere = true;
			switch(LastDragStateCore) {
				case DragState.Move:
					if(IsMouseOverCustomizationForm) {
						Data.CustomizationForm.SetDragComletedCallback(delegate() {
							CompleteAndDispose();
						});
						completeHere = !Data.CustomizationForm.HandleDragComplete(Field, LastMovePt);
					} else {
						PivotArea area;
						int areaIndex = ViewInfo.GetNewFieldPosition(Field, LastMovePt, out area);
						if(areaIndex > -1) {
							Data.SetFieldAreaPositionAsync(Data.GetField(Field), area, areaIndex, false, delegate(AsyncOperationResult result) {
								CompleteDoDragDropSetFieldAreaPosition();
							});
							completeHere = false;
						}
					}
					break;
				case DragState.Remove:
					PivotGridField pivotField = Data.GetField(Field);
					if(pivotField.CanHide) {
						Data.SetFieldVisibleAsync(pivotField, false, false, delegate(AsyncOperationResult result) {
							CompleteDoDragDropSetFieldAreaPosition();
						});
						completeHere = false;
					}
					break;
			}
			ViewInfo.Invalidate();
			if(completeHere)
				CompleteAndDispose();
		}
		void CompleteAndDispose() {
			if(dragCompletedCallback != null)
				dragCompletedCallback();
			ViewInfo.Invalidate();
			ViewInfo.DisposeDragManager();
		}
		void CompleteDoDragDropSetFieldAreaPosition() {
			if(dragCompletedCallback != null)
				dragCompletedCallback();
			if(Data.OptionsBehavior.ApplyBestFitOnFieldDragging)
				Data.ViewInfo.BestFit(SizingField);
			ViewInfo.Invalidate();
			ViewInfo.DisposeDragManager();
		}
		PivotFieldItem SizingField {
			get { return Data.VisualItems.GetSizingField(Field) as PivotFieldItem; }
		}
		protected override DragState GetDragState(Point pt) {
			if(LastMovePt.Equals(pt)) return this.lastDragStateInternal;
			LastMovePt = pt;
			this.lastDragStateInternal = DragState.Remove;
			if(IsMouseOverCustomizationForm) {
				this.lastDragStateInternal = IsMouseOverHiddenFieldsList && !Field.CanHide ? DragState.None : DragState.Move;
				DragIndicator.Hide();
				return this.lastDragStateInternal;
			}
			Rectangle newDrawRectangle = ViewInfo.GetDragDrawRectangle(Field, pt);
			bool accept = ViewInfo.AcceptDragDrop(pt);
			if(accept) {
				PivotArea area;
				int areaIndex = ViewInfo.GetNewFieldPosition(Field, LastMovePt, out area);
				accept = !Data.IsLocked && Data.OnFieldAreaChanging(Data.GetField(Field) as PivotGridField, area, areaIndex); 
			}
			this.lastDragStateInternal = DragState.Move;
			if(!accept) {
				this.lastDragStateInternal = Field.CanHide ? DragState.Remove : DragState.None;
				newDrawRectangle = Rectangle.Empty;
			}
			DragIndicator.Show(newDrawRectangle, GetIsLeft(newDrawRectangle));
			return this.lastDragStateInternal;
		}
		internal bool GetIsLeft(Rectangle newDrawRectangle) {
			if(newDrawRectangle.IsEmpty) 
				return false;
			Point center = new Point(
				(newDrawRectangle.Left + newDrawRectangle.Right) / 2,
				(newDrawRectangle.Top + newDrawRectangle.Bottom) / 2);
			BaseViewInfo viewInfo = Data.ViewInfo.GetViewInfoAtPoint(center);
			return viewInfo != null && center.X < (viewInfo.PaintBounds.Left + viewInfo.PaintBounds.Right) / 2;
		}
		protected virtual bool IsMouseOverCustomizationForm {
			get {
				if(Data.CustomizationForm == null) return false;
				Rectangle bounds = Data.CustomizationForm.Bounds;
				bounds.Location = Data.CustomizationForm.PointToScreen(new Point(0, 0));
				if(Data.CustomizationForm.Parent != null) {
					if(Data.CustomizationForm.Parent.Bounds.IsEmpty)
						return false;
					Rectangle parentBounds = Data.CustomizationForm.Parent.Bounds;
					parentBounds.Location = Data.CustomizationForm.Parent.PointToScreen(new Point(0, 0));
					bounds.Intersect(parentBounds);
				}
				return bounds.Contains(LastMovePt);
			}
		}
		bool IsMouseOverHiddenFieldsList {
			get {
				if(Data.CustomizationForm == null) return false;
				Rectangle bounds = Data.CustomizationForm.ActiveListBox.Bounds;
				bounds.Location = Data.CustomizationForm.ActiveListBox.PointToScreen(new Point(0, 0));
				return bounds.Contains(LastMovePt);
			}
		}
		protected override void RaisePaint (PaintEventArgs e) {
			ViewInfoPaintArgs pe = new ViewInfoPaintArgs(Data.ControlOwner, e);
			PivotDragHeaderViewInfo[] headers = DragHeaders;
			int x = 0;
			for(int i = 0; i < headers.Length; i ++) {
				headers[i].PaintDragHeader(pe, x);
				x += headers[i].Bounds.Width;
			}
		}
		protected Size HeaderSize {
			get {	
				PivotHeaderViewInfoBase[] headers = DragHeaders;
				int width = 0;
				for(int i = 0; i < headers.Length; i ++) {
					width += headers[i].Bounds.Width;
				}
				return new Size(width, HeaderViewInfo.Bounds.Height);
			}
		}
	}
	public abstract class DragIndicator : IDisposable {
		public static DragIndicator CreateDragIndicator(Control control, UserLookAndFeel lookAndFeel) {
			if(DragArrowsHelper.IsAllow)
				return new DragArrowsIndicator(control, lookAndFeel);
			return new ReversibleFrameDragIndicator(control);
		}
		Control control;
		public DragIndicator(Control control) {
			this.control = control;
		}
		protected Control Control { get { return control; } }
		public abstract void Show(Rectangle newDrawRectangle, bool isLeft);
		public abstract void Hide();
		public virtual void Dispose() {
			Hide();
		}
	}
	public class ReversibleFrameDragIndicator : DragIndicator {
		Rectangle drawRectangle;
		public ReversibleFrameDragIndicator(Control control)
			: base(control) {
			this.drawRectangle = Rectangle.Empty;
		}
		public override void Show(Rectangle newDrawRectangle, bool isLeft) {
			if(!newDrawRectangle.Equals(this.drawRectangle)) {
				DrawReversibleFrame();
				this.drawRectangle = newDrawRectangle;
				DrawReversibleFrame();
			}
		}
		public override void Hide() {
			if(!this.drawRectangle.IsEmpty) {
				DrawReversibleFrame();
				this.drawRectangle = Rectangle.Empty;
			}
		}
		void DrawReversibleFrame() {
			if(!this.drawRectangle.IsEmpty) {
				DevExpress.XtraEditors.Drawing.SplitterLineHelper.Default.DrawReversibleFrame(Control.Handle, this.drawRectangle);
			}
		}
	}
	public class DragArrowsIndicator : DragIndicator {
		DragArrowsHelper arrows;
		UserLookAndFeel lookAndFeel;
		public DragArrowsIndicator(Control control, UserLookAndFeel lookAndFeel)
			: base(control) {
				this.lookAndFeel = lookAndFeel;
		}
		protected DragArrowsHelper Arrows {
			get {
				if(arrows == null)
					arrows = new DragArrowsHelper(this.lookAndFeel, Control);
				return arrows;
			}
		}
		public override void Show(Rectangle newDrawRectangle, bool isLeft) {
			if(newDrawRectangle.IsEmpty) {
				Hide();
				return;
			}
			if(newDrawRectangle.X < 0)
				newDrawRectangle.X = 0;
			if(newDrawRectangle.Right < 0) {
				newDrawRectangle.X = 0;
				newDrawRectangle.Width = 0;
			}
			if(newDrawRectangle.Right > Control.Width) 
				newDrawRectangle.Width = Control.Width - newDrawRectangle.X;
			newDrawRectangle = Control.RectangleToScreen(newDrawRectangle);
			int x = isLeft ? newDrawRectangle.Left : newDrawRectangle.Right;
			Arrows.Show(true, new Point(x, newDrawRectangle.Top),
				new Point(x, newDrawRectangle.Bottom));
		}
		public override void Hide() {
			Arrows.Hide();
		}
		public override void Dispose() {
			base.Dispose();
			if(arrows != null) {
				arrows.Dispose();
				arrows = null;
			}
		}
	}
}
