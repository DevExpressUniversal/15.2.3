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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Browsing;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design.Adapters;
using DevExpress.XtraReports.Design.SnapLines;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.UI;
using System.Linq;
namespace DevExpress.XtraReports.Design {
	public interface IDragHandler {
		void HandleDragEnter(object sender, DragEventArgs e);
		void HandleDragOver(object sender, DragEventArgs e);
		void HandleDragDrop(object sender, DragEventArgs e);
		void HandleDragLeave(object sender, EventArgs e);
		void HandleGiveFeedback(object sender, GiveFeedbackEventArgs e);
	}
	public class DragHandlerBase : IDragHandler {
		public static ToolboxItem GetToolboxItem(IDataObject data, IDesignerHost host) {
			try {
				IToolboxService tbxSvc = host.GetService(typeof(IToolboxService)) as IToolboxService;
				return (tbxSvc != null) ? tbxSvc.DeserializeToolboxItem(data, host) : null;
			} catch {
				return null;
			}
		}		
		static protected RectangleF GetControlRect(XRControl control, Band band, float dragPointX, float dragPointY, IDesignerHost designerHost) {
			IBoundsAdapter adapter = BoundsAdapterService.GetAdapter(band, designerHost);
			RectangleF bandScreenBounds = adapter.GetScreenBounds();
			PointF primaryControlScreenLocation = new PointF(dragPointX, dragPointY);
			primaryControlScreenLocation.X -= bandScreenBounds.X;
			primaryControlScreenLocation.Y -= bandScreenBounds.Y;
			PointF primaryControlLocation = ZoomService.GetInstance(designerHost).FromScaledPixels(primaryControlScreenLocation, control.Dpi);
			RectangleF controlRect = new RectangleF(primaryControlLocation, control.SizeF);
			return controlRect;
		}
		IComponentChangeService fChangeSvc;
		ReportDesigner fReportDesigner;
		PointF dragPointOffset;
		bool lastIsSnappingAllowed;
		protected IDesignerHost host;
		protected IBandViewInfoService bandViewSvc;
		protected IMenuCommandServiceEx menuCommandService;
		protected IComponentChangeService ChangeSvc {
			get {
				if(fChangeSvc == null) {
					fChangeSvc = (IComponentChangeService)host.GetService(typeof(IComponentChangeService));
				}
				return fChangeSvc;
			}
		}
		protected RulerService RulerService {
			get { return host.GetService(typeof(RulerService)) as RulerService; }
		}
		protected AdornerService AdornerService {
			get { return host.GetService(typeof(AdornerService)) as AdornerService; }
		}
		protected ReportDesigner ReportDesigner {
			get {
				if(fReportDesigner == null) {
					fReportDesigner = (ReportDesigner)host.GetDesigner(host.RootComponent);
				}
				return fReportDesigner;
			}
		}
		protected XtraReport RootReport { get { return ReportDesigner.RootReport; } }
		public DragHandlerBase(IDesignerHost host) {
			this.host = host;
			bandViewSvc = (IBandViewInfoService)host.GetService(typeof(IBandViewInfoService));
			menuCommandService = host.GetService(typeof(IMenuCommandService)) as IMenuCommandServiceEx;
		}
		public virtual void Dispose() {
		}
		public virtual void HandleDragEnter(object sender, DragEventArgs e) {
			dragPointOffset = new PointF();
			e.Effect = DragDropEffects.None;
		}
		public virtual void HandleDragOver(object sender, DragEventArgs e) {
			e.Effect = DragDropEffects.None;
		}
		public virtual void HandleDragDrop(object sender, DragEventArgs e) {
			AdornerService.ResetSnapping();
			RulerService.HideShadows();
		}
		public virtual void HandleDragLeave(object sender, EventArgs e) {
			AdornerService.ResetSnapping();
			RulerService.HideShadows();
		}
		public virtual void HandleGiveFeedback(object sender, GiveFeedbackEventArgs e) {
		}
		protected virtual PointF EvalBasePoint(DragEventArgs e) {
			return GetDragPointWithOffset(e.X, e.Y);
		}
		protected PointF GetDragPointWithOffset(int dragX, int dragY) {
			return new PointF(dragX + dragPointOffset.X, dragY + dragPointOffset.Y);
		}
		protected void SelectControlContainer(Point screenPoint, IComponent component) {
			XRControl ctrl = component as XRControl;
			XRControl[] controls = ctrl != null ? new XRControl[] { ctrl } : new XRControl[] { };
			XRControl container = bandViewSvc.GetContainerDropTarget(screenPoint, controls);
			if(container != null)
				ReportDesigner.SelectComponents(new object[] { container });
		}
		protected void SelectComponent(IServiceProvider serviceProvider, IComponent component) {
			if(component != null) {
				ISelectionService selectionService = (ISelectionService)serviceProvider.GetService(typeof(ISelectionService));
				if(!selectionService.PrimarySelection.Equals(component))
					ReportDesigner.SelectComponents(new object[] { component });
			}
		}
		protected virtual bool IsContainerDropTargetLocked() {
			XRControl control = bandViewSvc.GetControlByScreenPoint(Control.MousePosition);
			if(control == null)
				return true;
			return IsContainerDropTargetLocked(control);
		}
		protected virtual bool IsContainerDropTargetLocked(XRControl control) {			
			return !LockService.GetInstance(host).CanChangeComponent(control);
		}
		protected PointF CorrectPointWithSnapOffset(PointF point, XRControl control, RectangleF controlRect, Band parentBand) {
			PointF snapDiffPoint = SnapLineHelper.GetSnapOffset(control, controlRect, parentBand, new XRControl[] { }, SelectionRules.AllSizeable, RootReport);
			PointF snapDiffPointInScaledPixels = ZoomService.GetInstance(host).ToScaledPixels(snapDiffPoint, control.Dpi);
			return new PointF(point.X + snapDiffPointInScaledPixels.X, point.Y + snapDiffPointInScaledPixels.Y);
		}
		protected RectangleF CorrectControlRectWithSnapOffset(RectangleF controlRect, XRControl control, Band parentBand) {
			PointF snapDiffPoint = SnapLineHelper.GetSnapOffset(control, controlRect, parentBand, new XRControl[] { }, SelectionRules.AllSizeable, RootReport);
			RectangleF newControlRect = controlRect;
			newControlRect.Offset(snapDiffPoint);
			return newControlRect;
		}
		protected void HandleDragOverCore(DragEventArgs e, XRControl primaryControl, XRControl[] dragControls, RectangleF[] dragControlRects) {
			XRControl parent = bandViewSvc.GetContainerDropTarget(new PointF(e.X, e.Y), dragControls);
			if(dragControls.Any(control => control is XRTableOfContents) && !(parent is ReportHeaderBand) && !(parent is ReportFooterBand)) {
				e.Effect = DragDropEffects.None;
			}
			if(e.Effect != DragDropEffects.None && parent !=null) {
				try {  
					if(!lastIsSnappingAllowed && SnapLineHelper.IsSnappingAllowed) {
						dragPointOffset = PointF.Empty;
					}
					Band parentBand = parent.Band;
					PointF basePoint = EvalBasePoint(e);
					RectangleF primaryControlRect = GetControlRect(primaryControl, parentBand, basePoint.X, basePoint.Y, host);
					if(RootReport.SnappingMode == SnappingMode.SnapLines && SnapLineHelper.IsSnappingAllowed && primaryControl.SupportSnapLines) {
						RectangleF snappedPrimaryControlRect = CorrectControlRectWithSnapOffset(primaryControlRect, primaryControl, parentBand);
						AdornerService.DrawSnapLines(primaryControl, snappedPrimaryControlRect, parentBand, dragControls, RootReport);
						lastIsSnappingAllowed = true;
					} else {
						if(lastIsSnappingAllowed) {
							dragPointOffset = CorrectPointWithSnapOffset(dragPointOffset, primaryControl, primaryControlRect, parentBand);
							basePoint = EvalBasePoint(e);
							AdornerService.ResetSnapping();
							lastIsSnappingAllowed = false;
						}
					}
					RectangleF[] rects = CreateRectArray(basePoint, parentBand, primaryControl, primaryControlRect, dragControlRects, dragControls);
					AdornerService.DrawScreenRects(rects);
					RulerService.DrawShadows(rects, parentBand.NestedLevel);
				} catch { }
			} else {
				AdornerService.ResetSnapping();
				RulerService.HideShadows();
			}
		}
		protected RectangleF[] CreateRectArray(PointF basePoint, Band parentBand, XRControl primaryControl, RectangleF primaryControlRect, RectangleF[] controlRects, XRControl[] dragControls) {
			PointF snappedBasePoint = bandViewSvc.PointToClientRelativeToBand(basePoint, new DragDataObject(dragControls, primaryControl, controlRects, primaryControlRect));
			return Array.ConvertAll<RectangleF, RectangleF>(controlRects, delegate(RectangleF value) { return RectHelper.OffsetRectF(value, snappedBasePoint); });
		}
		protected PointF GetControlLeftTopPoint(XRControl control, float x, float y) {
			SizeF controlSizeInScaledPixels = ZoomService.GetInstance(host).ToScaledPixels(control.BoundsF.Size, control.Dpi);
			return new PointF(x - controlSizeInScaledPixels.Width / 2, y - controlSizeInScaledPixels.Height / 2);
		}
	}
	public class FieldDragHandler : DragHandlerBase {
		enum FieldDropAction { None, CreateXRTable, CreateXRControl, AssignBinding };
		#region static
		public static Type DragType {
			get { return typeof(DataInfo[]); }
		}
		static CommandID GetCommandID(XRDataContext dataContext, DataInfo[] dataInfos) {
			Type type = dataContext.GetPropertyType(dataInfos[0].Source, dataInfos[0].Member);
			if(typeof(bool).Equals(type))
				return Commands.BandCommands.BindFieldToXRCheckBox;
			else if(typeof(Image).IsAssignableFrom(type) || typeof(byte[]).Equals(type))
				return Commands.BandCommands.BindFieldToXRPictureBox;
			else
				return Commands.BandCommands.BindFieldToXRLabel;
		}
		static string GetTransactionName(string compName, string propName) {
			string s = String.Format("{0}.{1}", compName, propName);
			return String.Format(DesignSR.Trans_ChangeProp, s);
		}
		static DataInfo[] GetDragData(IDataObject dataObject) {
			return dataObject.GetData(typeof(DataInfo[])) as DataInfo[];
		}
		static bool IsValidData(IDataObject data) {
			return data.GetDataPresent(DragType);
		}
		static FieldDropAction GetDropAction(DataInfo[] data, XRControl control) {
			if(control != null && data != null) {
				if(data.Length > 1)
					return FieldDropAction.CreateXRTable;
				else if(data.Length == 1 && CanAssignBinding(control) && (Control.ModifierKeys & Keys.Control) == 0)
					return FieldDropAction.AssignBinding;
				else if(data.Length == 1)
					return FieldDropAction.CreateXRControl;
			}
			return FieldDropAction.None;
		}
		#endregion
		bool rightButtonDrag;
		IDictionaryService dictionaryService;
		bool startedWithInplaceEditor;
		XRControl control;
		public FieldDragHandler(IDesignerHost host)
			: base(host) {
			dictionaryService = host.GetService(typeof(IDictionaryService)) as IDictionaryService;
			ISelectionService selectionServ = host.GetService(typeof(ISelectionService)) as ISelectionService;
			if(selectionServ == null)
				return;
			XRControl control = selectionServ.PrimarySelection as XRControl;
			if(control == null)
				return;
			XRTextControlDesigner designer = host.GetDesigner(control) as XRTextControlDesigner;
			if(designer != null && designer.IsInplaceEditingMode) {
				startedWithInplaceEditor = true;
				designer.CloseInplaceEditorOnDragLeave = false;
			}
		}
		public override void HandleDragEnter(object sender, DragEventArgs e) {
			base.HandleDragEnter(sender, e);
			UpdateDragEffect(e);
			DataInfo[] data = GetDragData(e.Data);
			if(data.Length == 1) {
				control = (XRControl)Activator.CreateInstance(typeof(XRControl));
				control.SyncDpi(RootReport.Dpi);
			}
			if(data.Length > 1) {
				control = (XRTable)Activator.CreateInstance(typeof(XRTable));
				control.SyncDpi(RootReport.Dpi);
				control.WidthF = RootReport.PageWidth - RootReport.Margins.Left - RootReport.Margins.Right;
			}
		}
		public override void HandleDragOver(object sender, DragEventArgs e) {
			base.HandleDragOver(sender, e);
			UpdateDragEffect(e);
			XRControl parent = bandViewSvc.GetControlByScreenPoint(new Point(e.X, e.Y));
			if(parent == null || startedWithInplaceEditor) {
				return;
			}
			if(control is XRTable || (!(CanAssignBinding(parent) && (Control.ModifierKeys & Keys.Control) == 0)) && (control != null && !rightButtonDrag && (e.KeyState & 4) == 0)) {
				RectangleF controlRectInScaledPixels = ZoomService.GetInstance(host).ToScaledPixels(control.BoundsF, control.Dpi);
				HandleDragOverCore(e, control, new XRControl[] { control }, new RectangleF[] { controlRectInScaledPixels });
			} else {
				AdornerService.ResetSnapping();
				RulerService.HideShadows();
			}					
			SelectComponent(host, parent);
			XRFieldEmbeddableControl fieldEmbeddableControl = parent as XRFieldEmbeddableControl;
			if(fieldEmbeddableControl == null || fieldEmbeddableControl.DataBindings["Text"] != null)
				return;
			XRTextControlBaseDesigner designer = host.GetDesigner(parent) as XRTextControlBaseDesigner;
			if(designer != null && !designer.IsInplaceEditingMode && EmbeddedFieldsHelper.HasEmbeddedFieldNames(fieldEmbeddableControl)) {
				designer.ShowInplaceEditor(false);
				designer.CloseInplaceEditorOnDragLeave = true;
			}
		}
		public override void HandleDragDrop(object sender, DragEventArgs e) {
			base.HandleDragDrop(sender, e);
			if(!IsValidData(e.Data))
				return;
			XRControl parent = bandViewSvc.GetControlByScreenPoint(new Point(e.X, e.Y));
			if(parent == null)
				return;
			PointF basePoint = EvalBasePoint(e);
			RectangleF controlRect = new RectangleF(basePoint, control.BoundsF.Size);
			PointF dropPoint = bandViewSvc.PointToClientRelativeToBand(basePoint, 
				new DragDataObject(new XRControl[] { control }, control, new RectangleF[] { controlRect }, controlRect));
			DataInfo[] droppedData = GetDragData(e.Data);
			System.Diagnostics.Debug.Assert(droppedData != null && droppedData.Length > 0);
			switch(GetDropAction(droppedData, parent)) {
				case FieldDropAction.CreateXRTable:
					DropXRTable(parent, !(rightButtonDrag || (e.KeyState & 4) != 0 ), dropPoint, droppedData);
					break;
				case FieldDropAction.CreateXRControl:
					if(rightButtonDrag || (e.KeyState & 4) != 0 )
						ShowDropContextMenu(dropPoint, droppedData);
					else
						DropXRControl(parent, dropPoint, droppedData);
					break;
				case FieldDropAction.AssignBinding:
					BindControl(parent, droppedData[0]);
					break;
			}
		}
		public override void HandleGiveFeedback(object sender, GiveFeedbackEventArgs e) {
			base.HandleGiveFeedback(sender, e);
			XRControl parent = bandViewSvc.GetControlByScreenPoint(Control.MousePosition);
			bool singleFieldIsDragging = control != null && !(control is XRTable);
			if(singleFieldIsDragging && CanAssignBinding(parent) && (Control.ModifierKeys & Keys.Control) == 0) {
				e.UseDefaultCursors = false;
				if(!Comparer.Equals(Cursor.Current, XRCursors.CanAssignBindingCursor)) {
					Cursor.Current = XRCursors.CanAssignBindingCursor;
				}
			} else {
				e.UseDefaultCursors = true;
			}
		}
		private void UpdateDragEffect(DragEventArgs e) {
			XRControl control = bandViewSvc.GetControlByScreenPoint(new Point(e.X, e.Y));
			if(IsValidData(e.Data) && CanChangeControl(control)) {
				if(GetDropAction(GetDragData(e.Data), control) != FieldDropAction.None) {
					e.Effect = DragDropEffects.Copy;
				}
			}
			rightButtonDrag = (e.KeyState & 2) != 0;
		}
		bool CanChangeControl(XRControl control) {
			return control != null && LockService.GetInstance(host).CanChangeComponent(control);
		}
		void DropXRControl(XRControl target, PointF dropPoint, DataInfo[] dataInfos) {
			using(XRDataContext dataContext = new XRDataContext(null, true))
				menuCommandService.GlobalInvoke(GetCommandID(dataContext, dataInfos), new object[] { dropPoint, dataInfos });
		}
		void ShowDropContextMenu(PointF dropPoint, DataInfo[] data) {
			IMenuCommandService menuService = host.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			if(menuService != null) {
				dictionaryService.SetValue("DataInfos", data);
				menuService.ShowContextMenu(Design.Commands.MenuCommandServiceCommands.FieldDropMenu, (int)Math.Round(dropPoint.X), (int)Math.Round(dropPoint.Y));
			}
		}
		void DropXRTable(XRControl target, bool bindCells, PointF dropPoint, DataInfo[] droppedData) {
			menuCommandService.GlobalInvoke(Commands.BandCommands.BindFieldsToXRTable, new object[] { bindCells, dropPoint, droppedData });
		}
		void BindControl(XRControl control, DataInfo data) {
			XRControlDesigner designer = host.GetDesigner(control) as XRControlDesigner;
			if(designer == null)
				return;
			IComponent[] components = { };
			string desc = GetTransactionName(control.Site.Name, XRComponentPropertyNames.DataBindings);
			DesignerTransaction trans = host.CreateTransaction(desc);
			try {
				XRControlDesignerBase.RaiseComponentChanging(ChangeSvc, control, XRComponentPropertyNames.DataBindings);
				string propName = designer.GetBindablePropName(data);
				XRBinding binding = control.DataBindings[propName];
				if(binding != null)
					binding.Assign(data);
				else
					control.DataBindings.Add(XRBinding.Create(propName, data.Source, data.Member, string.Empty));
				XRControlDesignerBase.RaiseComponentChanged(ChangeSvc, control);
				components = new IComponent[] { control };
				trans.Commit();
			} catch {
				trans.Cancel();
			}
			ReportDesigner.SelectComponents(components);
		}
		static bool CanAssignBinding(XRControl control) {
			DefaultBindablePropertyAttribute controlDefaultBindablePropertyAttribute = (DefaultBindablePropertyAttribute)TypeDescriptor.GetAttributes(control)[typeof(DefaultBindablePropertyAttribute)];
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(control)["DataBindings"];
			return propertyDescriptor != null && ((BrowsableAttribute)propertyDescriptor.Attributes[typeof(BrowsableAttribute)]).Browsable &&
				controlDefaultBindablePropertyAttribute != null && controlDefaultBindablePropertyAttribute.Name != null;
		}
		protected override PointF EvalBasePoint(DragEventArgs e) {
			PointF basePoint;
			PointF dragPointWithOffset = GetDragPointWithOffset(e.X, e.Y);
			PointF controlLeftTopPoint = GetControlLeftTopPoint(control, dragPointWithOffset.X, dragPointWithOffset.Y);
			if(control is XRTable) {
				XRControl parent = bandViewSvc.GetContainerDropTarget(new Point(e.X, e.Y), new XRControl[] { control });
				IBoundsAdapter adapter = BoundsAdapterService.GetAdapter(parent.Band, host);
				RectangleF bandScreenBounds = adapter.GetScreenBounds();
				basePoint = new PointF(bandScreenBounds.X, controlLeftTopPoint.Y);
			} else {
				basePoint = controlLeftTopPoint;
			}
			return basePoint;
		}
	}
	public class TbxItemDragHandler : DragHandlerBase {
		ToolboxItem tbxItem;
		XRControl control;
		public TbxItemDragHandler(IDesignerHost host) : base(host) { }
		protected XRControl XRControl { get { return control; } }
		public override void HandleDragEnter(object sender, DragEventArgs e) {
			base.HandleDragEnter(sender, e);
			control = null;
			tbxItem = GetToolboxItem(e.Data, host);
			if(tbxItem != null) {
				Type type = host.GetType(tbxItem.TypeName);
				object obj = null;
				try {
					obj = Activator.CreateInstance(type);
				} catch { }
				if(obj is XRControl) {
					control = (XRControl)obj;
					control.SyncDpi(RootReport.Dpi);
				}
				if(obj is Control) {
					control = new WinControlContainer();
					((WinControlContainer)control).WinControl = (Control)obj;
					control.SyncDpi(RootReport.Dpi);
				}
			}
			UpdateDragEffect(e);
		}
		public override void HandleDragOver(object sender, DragEventArgs e) {
			UpdateDragEffect(e);
			if(control != null) {
				RectangleF controlRectInScaledPixels = ZoomService.GetInstance(host).ToScaledPixels(control.BoundsF, control.Dpi);
				HandleDragOverCore(e, control, new XRControl[] { control }, new RectangleF[] { controlRectInScaledPixels });
			}
		}
		private void UpdateDragEffect(DragEventArgs e) {
			if(tbxItem != null && !IsContainerDropTargetLocked()) {
				e.Effect = DragDropEffects.Copy;
			} else
				e.Effect = DragDropEffects.None;
		}
		public override void HandleDragDrop(object sender, DragEventArgs e) {
			base.HandleDragDrop(sender, e);
			if(tbxItem != null) {
				SelectControlContainer(Control.MousePosition, control);
				DoToolPicked(tbxItem, e);
				tbxItem = null;
			}
		}
		protected override PointF EvalBasePoint(DragEventArgs e) {
			PointF basePoint = GetDragPointWithOffset(e.X, e.Y);
			return GetControlLeftTopPoint(control, basePoint.X, basePoint.Y);
		}
		protected void DoToolPicked(ToolboxItem tbxItem, DragEventArgs e) {
			System.Diagnostics.Debug.Assert(tbxItem != null);
			CapturePaintService capturePaintSvc = host.GetService(typeof(CapturePaintService)) as CapturePaintService;
			System.Diagnostics.Debug.Assert(capturePaintSvc != null);
			PointF dropPoint = GetDropPoint(e);
			capturePaintSvc.DragBounds = new RectangleF(dropPoint, Size.Empty);
			ReportDesigner.ToolPicked(tbxItem);
			capturePaintSvc.DragBounds = Rectangle.Empty;
		}
		PointF GetDropPoint(DragEventArgs e) {
			XRControl parent = bandViewSvc.GetContainerDropTarget(new Point(e.X, e.Y), new XRControl[] { control });
			if(parent == null) {
				return Control.MousePosition;
			}
			PointF basePoint = EvalBasePoint(e);
			RectangleF controlRect = new RectangleF(basePoint, control.BoundsF.Size);
			return bandViewSvc.PointToClientRelativeToBand(basePoint, new DragDataObject(new XRControl[] { control }, control, new RectangleF[] { controlRect }, controlRect)); 
		}
	}
	public class ControlDragHandler : DragHandlerBase {
		#region static
		public static PointF GetPrimaryDragControlSnapDiffPoint(PointF basePoint, XRControl primaryControl, Band parentBand, XRControl[] dragControls, XtraReport rootReport, IDesignerHost designerHost) {
			RectangleF primaryControlRect = GetControlRect(primaryControl, parentBand, basePoint.X, basePoint.Y, designerHost);
			return SnapLineHelper.GetSnapOffset(primaryControl, primaryControlRect, parentBand, dragControls, SelectionRules.AllSizeable, rootReport);
		}
		static void CorrectControlRectangles(RectangleF[] controlRects, PointF basePoint) {
			for(int i = 0; i < controlRects.Length; i++)
				controlRects[i].Offset(basePoint);
		}
		static DragDataObject GetDragData(IDataObject dataObject) {
			return (DragDataObject)dataObject.GetData(typeof(DragDataObject));
		}
		static bool IsValidData(IDataObject data) {
			return data.GetDataPresent(typeof(DragDataObject));
		}
		static void SuspendComponent(XRControl prevParent, bool suspend) {
			if(prevParent != null) {
				if(suspend)
					prevParent.SuspendLayout();
				else
					prevParent.ResumeLayout();
			}
		}
		#endregion
		public ControlDragHandler(IDesignerHost host): base(host) { }
		public override void HandleDragEnter(object sender, DragEventArgs e) {
			base.HandleDragEnter(sender, e);			
			if(IsValidData(e.Data))
				UpdateDragEffect(e);
		}
		public override void HandleDragOver(object sender, DragEventArgs e) {
			if(IsValidData(e.Data)) {
				UpdateDragEffect(e);
				DragDataObject dragData = GetDragData(e.Data);
				HandleDragOverCore(e, dragData.PrimaryControl, dragData.Controls, dragData.ControlRects);
			}
		}
		public override void HandleGiveFeedback(object sender, GiveFeedbackEventArgs e) {
		}
		protected virtual void UpdateDragEffect(DragEventArgs e) {
			DragDataObject dragData = GetDragData(e.Data);
			e.Effect = GetDragDropEffect(dragData);
		}
		private DragDropEffects GetDragDropEffect(DragDataObject dragData) {
			if(IsAppropriateReport(dragData.Controls)) {
				XRControl parent = bandViewSvc.GetContainerDropTarget(Control.MousePosition, dragData.Controls);
				if(parent != null) 
					return CalculateDragDropEffect(dragData, parent);
			}
			return DragDropEffects.None;
		}
		protected virtual DragDropEffects CalculateDragDropEffect(DragDataObject dragData, XRControl parent) {
			bool controlKeyPressed = Control.ModifierKeys == Keys.Control;
			if (!controlKeyPressed && !ContainsMoveableControl(dragData) || (controlKeyPressed && dragData.ControlsContainRotatable(host)))
				return DragDropEffects.None;
			return controlKeyPressed ? DragDropEffects.Copy : DragDropEffects.Move;
		}
		private bool ContainsMoveableControl(DragDataObject dragData) {
			XRControl[] controls = dragData.Controls;
			XRControl newParent = bandViewSvc.GetControlByScreenPoint(Control.MousePosition);
			foreach(XRControl control in controls) {
				if(LockService.GetInstance(host).CanChangeControlParent(control, newParent))
					return true;
			}
			return false;
		}
		private bool IsAppropriateReport(XRControl[] controls) {
			if(controls.Length > 0) {
				XtraReport report = controls[0].RootReport;
				return host.RootComponent.Equals(report);
			}
			return false;
		}
		public override void HandleDragDrop(object sender, DragEventArgs e) {
			base.HandleDragDrop(sender, e);
			if(IsValidData(e.Data)) {
				DragDataObject dragData = GetDragData(e.Data);
				if(dragData.Controls.Length > 0) {
					XRControl parent = bandViewSvc.GetContainerDropTarget(new Point(e.X, e.Y), dragData.Controls);
					System.Diagnostics.Debug.Assert(parent != null);
					PointF dragPointWithOffset = GetDragPointWithOffset(e.X, e.Y);
					RectangleF primaryControlRect = GetControlRect(dragData.PrimaryControl, parent.Band, dragPointWithOffset.X, dragPointWithOffset.Y, host);
					PointF basePoint = EvalBasePoint(e);
					RectangleF[] controlScreenRects = CreateRectArray(basePoint, parent.Band, dragData.PrimaryControl, primaryControlRect, dragData.ControlRects, dragData.Controls);
					PointF snappedPoint = bandViewSvc.PointToClient(new PointF(dragPointWithOffset.X, dragPointWithOffset.Y), dragData, parent);
					CorrectControlRectangles(dragData.ControlRects, snappedPoint);
					DropControls(parent, dragData.Controls, dragData.ControlRects, controlScreenRects);
				}
			}
		}
		protected override PointF EvalBasePoint(DragEventArgs e) {
			PointF basePoint = GetDragPointWithOffset(e.X, e.Y);
			return GetDragData(e.Data).EvalBasePoint(basePoint.X, basePoint.Y);
		}
		void DropControls(XRControl parent, XRControl[] controls, RectangleF[] controlRects, RectangleF[] controlScreenRects) {
			if(controls.Length == 0)
				return;
			if(Control.ModifierKeys == Keys.Control) {
				CopySelectedControls(parent, controlRects, controlScreenRects);
				return;
			}
			string desc = controls.Length == 1
				? string.Format(DesignSR.TransFmt_OneMove, controls[0].Site.Name)
				: string.Format(DesignSR.TransFmt_Move, controls.Length);
			DesignerTransaction trans = host.CreateTransaction(desc);
			try {
				DoMoveControls(parent, controls, controlRects, controlScreenRects);
				trans.Commit();
			} catch {
				trans.Cancel();
			}
		}
		#region tests
#if DEBUG
		public
#endif
		#endregion
		void CopySelectedControls(XRControl parent, RectangleF[] controlRects, RectangleF[] controlScreenRects) {
			DesignerTransaction trans = host.CreateTransaction(DesignSR.Trans_CreateComponents);
			List<XRControl> suspendedParents = new List<XRControl>(1);
			try {
				object[] copyingControls = ReportDesigner.GetSelectedComponents();
				System.Diagnostics.Debug.Assert(copyingControls.Length >= controlRects.Length);
				System.Diagnostics.Debug.Assert(copyingControls.Length >= controlScreenRects.Length);
				ReportDesigner.SelectComponents(copyingControls.OfType<XRControl>().ToArray());
				menuCommandService.GlobalInvoke(StandardCommands.Copy);
				ReportDesigner.SelectComponents(new XRControl[] { parent });
				menuCommandService.GlobalInvoke(StandardCommands.Paste);
				XRControl[] copiedControls = ReportDesigner.GetSelectedComponents().OfType<XRControl>().Where(x => x.Parent != null).ToArray();
				if(copiedControls.Length != 0)
					DoMoveControls(parent, copiedControls, controlRects, controlScreenRects);
				trans.Commit();
			} catch {
				trans.Cancel();
			} finally {
				foreach(XRControl suspendedParent in suspendedParents)
					SuspendComponent(suspendedParent, false);
				suspendedParents.Clear();
			}
		}
		void DoMoveControls(XRControl parent, XRControl[] controls, RectangleF[] controlRects, RectangleF[] controlScreenRects) {
			for(int i = 0; i < controls.Length; i++)
				DoMoveControl(parent, controls[i], controlRects[i].Location, controlScreenRects[i]);
		}
		void DoMoveControl(XRControl parent, XRControl control, PointF controlLocation, RectangleF controlScreenRect) {
			XRControl oldParent = control.Parent != parent ? control.Parent : null;
			try {
				SuspendComponent(oldParent, true);
				if(!LockService.GetInstance(host).CanChangeControlParent(control, parent))
					return;
				XRControlDesigner designer = this.host.GetDesigner(control) as XRControlDesigner;
				if(designer != null)
					designer.MoveControl(parent, controlLocation, controlScreenRect);
			} finally {
				SuspendComponent(oldParent, false);
			}
		}
	}
	public class ReportExplorerDragHandler : DragHandlerBase {
		enum ReportExplorerDropAction { None, AssignStyle, AssignRule };
		#region static
		public static Type DragType {
			get { return typeof(FormattingDataObject[]); }
		}
		static FormattingDataObject[] GetDragData(IDataObject dataObject) {
			return dataObject.GetData(typeof(FormattingDataObject[])) as FormattingDataObject[];
		}
		static bool IsValidData(IDataObject data) {
			return data.GetDataPresent(DragType);
		}
		static ReportExplorerDropAction GetDropAction(FormattingDataObject[] data, XRControl control) {
			if(control != null && data != null) {
				if(data.Length == 1) {
					if(data[0].IsStyle && FormattingComponentAssignmentHelper.CanAssignStyle(control))
						return ReportExplorerDropAction.AssignStyle;
					else if(data[0].IsRule && FormattingComponentAssignmentHelper.CanAssignRule(control))
						return ReportExplorerDropAction.AssignRule;
				}
			}
			return ReportExplorerDropAction.None;
		}
		#endregion
		IDictionaryService dictionaryService;
		XRControlStyle style;
		FormattingRule rule;
		public ReportExplorerDragHandler(IDesignerHost host)
			: base(host) {
			dictionaryService = host.GetService(typeof(IDictionaryService)) as IDictionaryService;
		}
		public override void HandleDragEnter(object sender, DragEventArgs e) {
			base.HandleDragEnter(sender, e);
			UpdateDragEffect(e); 
			FormattingDataObject[] droppedData = GetDragData(e.Data);
			System.Diagnostics.Debug.Assert(droppedData != null && droppedData.Length > 0);
			if(droppedData.Length == 1) {
				if(droppedData[0].IsRule)
					rule = droppedData[0].FormattingComponent as FormattingRule;
				else if(droppedData[0].IsStyle)
					style = droppedData[0].FormattingComponent as XRControlStyle;
			}
		}
		public override void HandleDragOver(object sender, DragEventArgs e) {
			base.HandleDragOver(sender, e);
			UpdateDragEffect(e);
			XRControl parent = bandViewSvc.GetControlByScreenPoint(new Point(e.X, e.Y));
			if(parent == null)
				return;
			SelectComponent(host, parent);
		}
		public override void HandleDragDrop(object sender, DragEventArgs e) {
		   base.HandleDragDrop(sender, e);
		   if(!IsValidData(e.Data))
			   return;
		   XRControl parent = bandViewSvc.GetControlByScreenPoint(new Point(e.X, e.Y));
		   if(parent == null)
			   return;
			FormattingDataObject[] droppedData = GetDragData(e.Data);
			System.Diagnostics.Debug.Assert(droppedData != null && droppedData.Length > 0);
			ReportDesigner.SelectComponents(droppedData.Select(x => x.FormattingComponent).ToList());  
			switch(GetDropAction(droppedData, parent)) {
				case ReportExplorerDropAction.AssignStyle:
					menuCommandService.GlobalInvoke(Commands.FormattingComponentCommands.AssignStyleToXRControl, new object[] { parent, droppedData });
					break;
				case ReportExplorerDropAction.AssignRule:
					menuCommandService.GlobalInvoke(Commands.FormattingComponentCommands.AssignRuleToXRControl, new object[] { parent, droppedData });
					break;
			}
		}
		private void UpdateDragEffect(DragEventArgs e) {
			XRControl control = bandViewSvc.GetControlByScreenPoint(new Point(e.X, e.Y));
			if(IsValidData(e.Data) && CanChangeControl(control)) {
				if(GetDropAction(GetDragData(e.Data), control) != ReportExplorerDropAction.None) {
					e.Effect = DragDropEffects.Copy;
				}
			}
		}
		bool CanChangeControl(XRControl control) {
			return control != null && LockService.GetInstance(host).CanChangeComponent(control);
		}
	}
}
