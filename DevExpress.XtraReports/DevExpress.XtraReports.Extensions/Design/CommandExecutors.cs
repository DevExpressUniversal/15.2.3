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
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Browsing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraReports.Design.SnapLines;
using System.Linq;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils.Internal;
namespace DevExpress.XtraReports.Design.Commands {
	public abstract class CommandExecutorBase : ICommandExecutor {	
		protected IDesignerHost designerHost;
		protected ISelectionService selectionServ;
		protected IComponentChangeService changeServ;
		protected object[] parameters;
		IBandViewInfoService fBandViewSvc;
		#region IDisposable implementation
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
		}
		~CommandExecutorBase() {
			Dispose(false);
		}
		#endregion
		protected IBandViewInfoService BandViewService {
			get {
				if(fBandViewSvc == null)
					fBandViewSvc = designerHost.GetService(typeof(IBandViewInfoService)) as IBandViewInfoService;
				return fBandViewSvc;
			}
		}
		protected float ReportDpi {
			get {
				return ((XtraReport)designerHost.RootComponent).Dpi;
			}
		}
		protected bool SnapToGrid {
			get {
				return ((XtraReport)designerHost.RootComponent).SnapToGrid;
			}
			set {
				((XtraReport)designerHost.RootComponent).SnapToGrid = value;
			}
		}
		protected SizeF GridSize {
			get {
				return ((XtraReport)designerHost.RootComponent).GridSizeF;
			}
		}
		protected static SizeF DefaultGridSize {
			get {
				return new SizeF(1, 1);
			}
		}
		protected CommandExecutorBase(IDesignerHost designerHost) {
			this.designerHost = designerHost;
			selectionServ = designerHost.GetService(typeof(ISelectionService)) as ISelectionService;
			changeServ = designerHost.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
		}
		public void ExecCommand(CommandID cmdID, object[] parameters) {
			this.parameters = parameters;
			ExecCommand(cmdID);
		}
		abstract public void ExecCommand(CommandID cmdID);
		protected ArrayList GetSelectedComponents() {
			return new ArrayList( selectionServ.GetSelectedComponents() );
		}
		public virtual bool OnUpdateCommandCheck(XRControl xrControl, CommandID cmdID) {
			return false;
		}
		protected void RaiseComponentChanging(IComponent component, string propertyName) {
			if(component is IWeighty) {
				IWeighty weightyComponent = component as IWeighty;
				IComponent prev = weightyComponent.Previous as IComponent;
				if(prev != null) RaiseComponentChangingFunctionality(prev, XRComponentPropertyNames.Weight);
				IComponent next = weightyComponent.Next as IComponent;
				if(next != null) RaiseComponentChangingFunctionality(next, XRComponentPropertyNames.Weight);
				RaiseComponentChangingFunctionality(component, XRComponentPropertyNames.Weight);
				XRTable table = null;
				XRTableCell cell = component as XRTableCell;
				if(cell != null && cell.Row != null && cell.Row.Table != null) table = cell.Row.Table;
				XRTableRow row = component as XRTableRow;
				if(row != null && row.Table != null) table = row.Table;
				if(table != null) {
					RaiseComponentChangingFunctionality(table, XRComponentPropertyNames.Size);
					RaiseComponentChangingFunctionality(table, XRComponentPropertyNames.Location);
				}
			}
			else RaiseComponentChangingFunctionality(component, propertyName);
		}
		void RaiseComponentChangingFunctionality(IComponent component, string propertyName) {
			PropertyDescriptor property = propertyName != null ? XRAccessor.GetPropertyDescriptor(component, propertyName) : null;
			changeServ.OnComponentChanging(component, property);
		}
		protected void RaiseComponentChanged(IComponent component) {
			if(component is IWeighty) {
				IWeighty weightyComponent = component as IWeighty;
				IComponent prev = weightyComponent.Previous as IComponent;
				if(prev != null) RaiseComponentChangedFunctionality(prev);
				IComponent next = weightyComponent.Next as IComponent;
				if(next != null) RaiseComponentChangedFunctionality(next);
				RaiseComponentChangedFunctionality(component);
				XRTable table = null;
				XRTableCell cell = component as XRTableCell;
				if(cell != null && cell.Row != null && cell.Row.Table != null) table = cell.Row.Table;
				XRTableRow row = component as XRTableRow;
				if(row != null && row.Table != null) table = row.Table;
				if(table != null) RaiseComponentChangedFunctionality(table);
			}
			RaiseComponentChangedFunctionality(component);
		}
		void RaiseComponentChangedFunctionality(IComponent component) {
			changeServ.OnComponentChanged(component, null, null, null);
		}
	}
	public class KeySelectionCommandExecutor : CommandExecutorBase
	{
		public KeySelectionCommandExecutor(IDesignerHost designerHost) : base(designerHost) {
		}
		public override void ExecCommand(CommandID cmdID) {
			XRControl primaryComponent = selectionServ.PrimarySelection as XRControl;
			XRControl nextComponent = null;
			if (cmdID == MenuCommands.KeySelectNext)
				nextComponent = ComponentsTabOrder.GetNextComponent(primaryComponent);
			else if (cmdID == MenuCommands.KeySelectPrevious)
				nextComponent = ComponentsTabOrder.GetPrevComponent(primaryComponent);
			if (nextComponent != null)
				selectionServ.SetSelectedComponents(new object[] { nextComponent }, SelectionTypes.Replace);
		}
	}
	public class EscCommandExecutor : CommandExecutorBase 
	{
		public EscCommandExecutor(IDesignerHost designerHost) : base(designerHost) {
		}
		public override void ExecCommand(CommandID cmdID) {
			CapturePaintService svc = CapturePaintService.GetRunningService(designerHost);
			if(svc != null) {
				svc.EndServiceInternal(true);
				return;
			}
			IComponent primaryControl = selectionServ.PrimarySelection as IComponent;
			if (primaryControl == null) 
				return;
			XRControlDesignerBase designer = designerHost.GetDesigner(primaryControl) as XRControlDesignerBase;
			if(designer != null)  {
				System.ComponentModel.CancelEventArgs args = new System.ComponentModel.CancelEventArgs();
				designer.OnKeyCancel(args); 
				if(args.Cancel) 
					return;
			}
			IComponent parent = GetParentComponent(primaryControl);
			if (parent != null)
				selectionServ.SetSelectedComponents(new object[] { parent }, SelectionTypes.Replace);
		}
		private IComponent GetParentComponent(IComponent comp) {
			XRControl control = comp as XRControl;
			if (control != null)
				return control.Parent;
			FakeComponent chartComponent = comp as FakeComponent;
			if(chartComponent != null)
				return chartComponent.Parent;
			return FrameSelectionUIService.GetComponentContainer(designerHost, comp);
		}
	}
	internal class PivotGridCommandExecutor : CommandExecutorBase {
		public PivotGridCommandExecutor(IDesignerHost host)
			: base(host) {
		}
		public override void ExecCommand(CommandID cmdID) {
			if(!(selectionServ.PrimarySelection is XRPivotGrid)) return;
			XRPivotGridDesigner designer = (XRPivotGridDesigner)designerHost.GetDesigner(selectionServ.PrimarySelection as IComponent);
			if(cmdID == VerbCommands.PivotGridDesigner)
				designer.RunDesigner();
		}
	}
	internal class RichTextBoxCommandExecutor : CommandExecutorBase {
		public RichTextBoxCommandExecutor(IDesignerHost host) : base(host) {
		}
		public override void ExecCommand(CommandID cmdID) {
			if( !(selectionServ.PrimarySelection is XRRichTextBase) ) return;
			XRRichTextBaseDesigner designer = (XRRichTextBaseDesigner)designerHost.GetDesigner(selectionServ.PrimarySelection as IComponent);
			if(cmdID == VerbCommands.RtfClear)
				designer.OnClear(this, EventArgs.Empty); 
			else if(cmdID == VerbCommands.RtfLoadFile) 
				designer.OnLoadFile(this, EventArgs.Empty);
		}
	}
	internal class VerbCommandExecutor : CommandExecutorBase {
		public VerbCommandExecutor(IDesignerHost host)
			: base(host) {
		}
		public override void ExecCommand(CommandID cmdID) {
			try {
				if(parameters != null && parameters.Length > 0 && parameters[0] is DesignerVerb)
					((DesignerVerb)parameters[0]).Invoke();
			} catch(Exception ex) {
				if(ExceptionHelper.IsCriticalException(ex))
					throw;
				else
					NotificationService.ShowException<XtraReport>(LookAndFeelProviderHelper.GetLookAndFeel(designerHost), designerHost.GetOwnerWindow(), ex);
			}
		}
	}
	internal class ReportCommandExecutor : CommandExecutorBase {
		public ReportCommandExecutor(IDesignerHost host) : base(host) {
		}
		public override void ExecCommand(CommandID cmdID) { 
			ReportDesigner designer = (ReportDesigner)designerHost.GetDesigner(designerHost.RootComponent);
			bool result = false;
			if(cmdID == VerbCommands.ReportWizard) {
				UndoEngine undoEngine = designerHost.GetService<UndoEngine>();
				undoEngine.ExecuteWithoutUndo(() => { result = designer.OnReportWizard(); });
			} else if(cmdID == VerbCommands.EditBands)
				designer.OnEditBands(this, EventArgs.Empty);
			else if(cmdID == VerbCommands.LoadReportTemplate) {
				UndoEngine undoEngine = designerHost.GetService<UndoEngine>();
				undoEngine.ExecuteWithoutUndo(() => { result = designer.LoadReportTemplate(); });
			} else if(cmdID == VerbCommands.EditBindings)
				designer.OnEditBindings();
			else if(cmdID == VerbCommands.Import) {
				UndoEngine undoEngine = designerHost.GetService<UndoEngine>();
				undoEngine.ExecuteWithoutUndo(() => { result = designer.OnImport(); });
			}
			if(result) ClearUndoStack();
		}
		void ClearUndoStack() {
			IUndoService undoService = designerHost.GetService<IUndoService>();
			if(undoService != null) undoService.ClearUndoStack();
		}
	}
	internal class TextControlCommandExecutor : CommandExecutorBase {
		public TextControlCommandExecutor(IDesignerHost host) : base(host) {
		}
		public override void ExecCommand(CommandID cmdID) { 
			XRTextControlDesigner designer = designerHost.GetDesigner(selectionServ.PrimarySelection as IComponent) as XRTextControlDesigner;
			if(designer != null && cmdID == VerbCommands.EditText)
				designer.OnEditText(this, EventArgs.Empty);
		}
	}
	internal class DetailReportCommandExecutor : CommandExecutorBase { 
		public DetailReportCommandExecutor(IDesignerHost host) : base(host) {
		}
		public override void ExecCommand(CommandID cmdID) { 
			if( cmdID.Equals(ReportCommands.InsertDetailReport) ) 
				AddDetailReport();
		}
		private void AddDetailReport() {
			DetailReportBand detailReport = CreateDetailReport();
			string description = String.Format(DesignSR.Trans_Add, detailReport.GetType().Name);
			DesignerTransaction transaction = designerHost.CreateTransaction(description);
			try {
				AddToContainer(detailReport);
				Band detail = XtraReport.CreateBand(BandKind.Detail);
				XRControlDesignerBase.RaiseCollectionChanging(detailReport, this.changeServ);
				detailReport.Bands.Add(detail);
				AddToContainer(detailReport.Bands[0]);
				XRControlDesignerBase.RaiseCollectionChanged(detailReport, this.changeServ);
			} finally {
				transaction.Commit();
			}
			selectionServ.SetSelectedComponents(new object[] { detailReport }, SelectionTypes.Replace);
		}
		private DetailReportBand CreateDetailReport() {
			DetailReportBand report = (DetailReportBand)XtraReport.CreateBand(BandKind.DetailReport);
			if(parameters != null && parameters.Length > 0)
				report.DataMember = (string)parameters[0];
			return report;
		}
		private void AddToContainer(Band band) {
			string name = BandDesigner.CreateBandName(designerHost, band);
			DesignToolHelper.AddToContainer(designerHost, band, name);
		}
	}
	internal class BandCommandExecutor : CommandExecutorBase 
	{ 
		public BandCommandExecutor(IDesignerHost host)
			: base(host) {
		}
		private void AddToContainer(Band band) {
			string description = String.Format(DesignSR.Trans_Add, band.GetType().Name);
			DesignerTransaction transaction = designerHost.CreateTransaction(description);
			try {
				string name = BandDesigner.CreateBandName(designerHost, band);
				DesignToolHelper.AddToContainer(designerHost, band, name);
				selectionServ.SetSelectedComponents(new object[] { band }, SelectionTypes.Replace );
			}
			finally {
				transaction.Commit();
			}
		}
		public override void ExecCommand(CommandID cmdID) {
			BandKind bandKind = BandCommandID.GetBandKind(cmdID);
			try {
				Band band = XtraReport.CreateBand(bandKind);
				if(band != null)
					AddToContainer(band);
			} catch { }
		}
	}
	internal class FieldDropCommandExecutor : CommandExecutorBase {
		#region static
		static void SetupControl(XRControl control, PointF location, XRBinding binding, bool snapToGrid, SizeF gridSize) {
			if(binding != null)
				control.SetDataBindingWhenDroppedFromTheFieldList(binding);
			control.LocationF = location;
			if(snapToGrid) control.SizeF = SnapSize(control.SizeF, gridSize);
		}
		static protected SizeF SnapSize(SizeF size, SizeF gridSize) {
			size = Divider.GetDivisibleValue(size, gridSize);
			return NativeMethods.GetMaxSize(size, gridSize);
		}
		static void AddToContainer(IDesignerHost host, XRControl[] controls) {
			DesignerTransaction trans = host.CreateTransaction(DesignSR.Trans_CreateComponents);
			try {
				DesignToolHelper.AddToContainer(host, controls);
				trans.Commit();
			}
			catch {
				trans.Cancel();
			}
		}
		XRControl CreateXRControl(Type type, PointF location, XRBinding binding, bool snapToGrid, SizeF gridSize) {
			XRControl control = (XRControl)Activator.CreateInstance(type);
			control.SyncDpi(ReportDpi);
			SetupControl(control, location, binding, snapToGrid, gridSize);
			return control;
		}
		#endregion
		#region fields & properties
		protected ReportDesigner fReportDesigner;
		protected ReportDesigner ReportDesigner {
			get {
				if(fReportDesigner == null)
					fReportDesigner = (ReportDesigner)designerHost.GetDesigner(designerHost.RootComponent);
				return fReportDesigner;
			}
		}
		#endregion
		public FieldDropCommandExecutor(IDesignerHost host) : base(host) {
		}
		public override void ExecCommand(CommandID cmdID) {
			if(cmdID.Equals(BandCommands.BindFieldToXRLabel))
				DropXRControl(typeof(XRLabel), "Text");
			else if(cmdID.Equals(BandCommands.BindFieldToXRPictureBox))
				DropXRControl(typeof(XRPictureBox), "Image");
			else if(cmdID.Equals(BandCommands.BindFieldToXRCheckBox))
				DropXRControl(typeof(XRCheckBox), "CheckState");
			else if(cmdID.Equals(BandCommands.BindFieldToXRRichText))
				DropXRControl(typeof(XRRichText), "Rtf");
			else if(cmdID.Equals(BandCommands.BindFieldToXRBarCode))
				DropXRControl(typeof(XRBarCode), "Text");
			else if(cmdID.Equals(BandCommands.BindFieldToXRZipCode))
				DropXRControl(typeof(XRZipCode), "Text");
			else if(cmdID.Equals(BandCommands.BindFieldsToXRTable))
				DropXRTable();
		}
		void DropXRTable() {
			DesignerTransaction transaction = designerHost.CreateTransaction("Drop table");
			try {
				XtraReport report = ReportDesigner.RootReport;
				bool bindCells = (bool)parameters[0];
				PointF dropPoint = (PointF)parameters[1];
				DataInfo[] droppedData = (DataInfo[])parameters[2];
				IDataContextService dataContextService = designerHost.GetService(typeof(IDataContextService)) as IDataContextService;
				DataContext dataContext = dataContextService.CreateDataContext(new DataContextOptions(false, true));
				IEnumerable<DataInfo> realData = droppedData.Where<DataInfo>(info => {
					Type type = dataContext.GetPropertyType(info.Source, info.Member);
					return !typeof(Image).IsAssignableFrom(type) && !typeof(byte[]).Equals(type);
				});
				XRTable table = new XRTable();
				table.SyncDpi(report.Dpi);
				table.BeginInit();
				XRTableRow row = new XRTableRow();
				table.Rows.Add(row);
				List<XRControl> cells = CreateDroppedCells(row, bindCells, realData);
				if(cells.Count <= 0) return;				
				XRControl parent = BandViewService.GetContainerDropTarget(dropPoint, new XRControl[] { table });
				table.LocationF = new PointF(table.LeftF, PointToClient(parent, dropPoint).Y);
				table.WidthF = parent.WidthF > 0 ? parent.WidthF :
					report.PageWidth - report.Margins.Left - report.Margins.Right;
				table.EndInit();
				List<XRControl> controls = new List<XRControl>();
				controls.Add(table);
				controls.Add(row);
				controls.AddRange(cells);
				AddToContainer(designerHost, controls.ToArray());
				XRControlDesigner rowDesinger = (XRControlDesigner)designerHost.GetDesigner(row);
				rowDesinger.ChangeParent(table);
				foreach(XRControl control in cells) {
					XRControlDesigner designer = (XRControlDesigner)designerHost.GetDesigner(control);
					designer.ChangeParent(row);
				}
				XRControlDesigner tableDesigner = (XRControlDesigner)designerHost.GetDesigner(table);
				tableDesigner.ChangeParent(parent);
				table.BringToFront();
				ReportDesigner.SelectComponents(new IComponent[] { table });
			} finally {
				transaction.Commit();
			}
		}
		List<XRControl> CreateDroppedCells(XRControl parent, bool performBindings, IEnumerable<DataInfo> droppedData) {
			Point location = new Point(0, 0);
			List<XRControl> cells = new List<XRControl>();
			foreach(DataInfo dataInfo in droppedData) {
				XRBinding binding = null;
				if(performBindings) 
					binding = new XRBinding(XRComponentPropertyNames.Text, dataInfo);
				XRControl cell = CreateXRControl(typeof(XRTableCell), location, binding, ReportDesigner.SnapToGrid && ReportDesigner.RootReport.SnappingMode == SnappingMode.SnapToGrid, ReportDesigner.RootReport.GridSizeF);
				if(!performBindings) 
					cell.Text = NativeMethods.MakeFieldDisplayName(dataInfo.DisplayName);
				cells.Add(cell);
				cell.Parent = parent;
			}
			return cells;
		}
		void DropXRControl(Type type, string bindPropertyName) {
			if(!IsValidDropXRControlParameters()) 
				return;
			PointF screenPoint = (PointF)parameters[0];
			DataInfo[] droppedData = (DataInfo[])parameters[1];
			if(droppedData == null || droppedData.Length != 1) 
				return;			
			DesignerTransaction transaction = designerHost.CreateTransaction("Drop field");
			XRControl control = GetContainerDropTarget();
			PointF location = PointToClient(control, screenPoint);
			XRControl newControl = CreateXRControl(type, location, XRBinding.Create(bindPropertyName, droppedData[0].Source, droppedData[0].Member, string.Empty), ReportDesigner.SnapToGrid && ReportDesigner.RootReport.SnappingMode == SnappingMode.SnapToGrid, ReportDesigner.RootReport.GridSizeF);
			try {
				AddToContainer(designerHost, newControl);
				newControl.BringToFront();
			} catch {
				transaction.Cancel();
			}
			finally {
				transaction.Commit();
			}
		}
		bool IsValidDropXRControlParameters() {
			return parameters.Length == 2 ? 
				(parameters[0] is PointF) && (parameters[1] is DataInfo[]) :
				false;
		}
		PointF PointToClient(XRControl control, PointF screenPoint) {
			return ZoomService.GetInstance(designerHost).FromScaledPixels(BandViewService.PointToClient(screenPoint, null, control), ReportDpi);
		}
		void AddToContainer(IDesignerHost host, XRControl control) {
			AddToContainer(host, new XRControl[] { control });
			ReportDesigner.SelectComponents(new IComponent[] { control });
		}
		XRControl GetContainerDropTarget() {
			XRControl control = selectionServ.PrimarySelection as XRControl;
			return control.CanHaveChildren ? control : control.Parent;
		}
	}
	public class ComponentCommandExecutor : CommandExecutorBase 
	{
		public static ControlModifier CreateControlModifier(XRControl control, IDesignerHost designerHost) {
			XRRichTextBaseDesigner designer = designerHost.GetDesigner(control) as XRRichTextBaseDesigner;
			return designer != null && designer.Editor != null ? new RichTextBoxModifier() : new ControlModifier();
		}
		protected ArrayList selComponents;
		protected string[] modifyPropNames;
		protected CommandID cmdID;
		protected ReportDesigner repDesigner;
		XRControl primaryControl;
		protected XRControl PrimaryControl {
			get { return primaryControl; }
		}
		public ComponentCommandExecutor(IDesignerHost designerHost)
			: base(designerHost) {
			this.modifyPropNames = new string[] {};
			repDesigner = designerHost.GetDesigner(designerHost.RootComponent) as ReportDesigner;
		}
		ArrayList ExcludeUnUpgradableComponents(ArrayList components) {
			if(!repDesigner.IsEUD)
				return new ArrayList(components);
			ArrayList newComponents = new ArrayList(components.Count);
			foreach(IComponent item in components) {
				XRControl controlXr = item as XRControl;
				if(controlXr != null && !controlXr.ActualLockedInUserDesigner) {
					newComponents.Add(item);
				}
			}
			return newComponents;
		}
		public override void ExecCommand(CommandID cmdID) {
			this.cmdID = cmdID;
			primaryControl = selectionServ.PrimarySelection as XRControl;
			if(primaryControl == null)
				return;
			selComponents = GetSelectedComponents(primaryControl.Parent);
			if(selComponents != null && modifyPropNames.Length > 0) {
				ArrayList oldSelComponents = selComponents;
				selComponents = ExcludeUnUpgradableComponents(oldSelComponents);
				oldSelComponents.Clear();
				if(selComponents.Count > 0 && ShouldCreateTransaction()) {
					Initialize();
					string descr = GetTransDescription();
					DesignerTransaction trans = designerHost.CreateTransaction(descr);
					try {
						ExecuteCore(modifyPropNames);
					} catch {
						trans.Cancel();
					} finally {
						try {
							trans.Commit();
						} catch(System.ComponentModel.Design.Serialization.CodeDomSerializerException) {
						}
					}
				}
			}
		}
		protected virtual void Initialize() {			
		}
		private bool ShouldCreateTransaction() {
			foreach(XRControl xrControl in selComponents) {
				if(AllowModifyComponent(xrControl)) {
					return true;
				}
			}
			return false;
		}
		protected virtual void ExecuteCore(string[] propertyNames) {
			bool snapToGrid = SnapToGrid;
			if(selComponents.Count > 1 && ((cmdID == StandardCommands.CenterHorizontally) || (cmdID == StandardCommands.CenterVertically)))
				SnapToGrid = false;
			foreach(XRControl xrControl in selComponents) {
				foreach(string propertyName in propertyNames) {
					try {	
						RaiseComponentChanging(xrControl, propertyName);
					} catch {}
				}
				ModifyComponent(xrControl);
				RaiseComponentChanged(xrControl);				
			}
			SnapToGrid = snapToGrid;
		}
		protected virtual bool AllowModifyComponent(XRControl xrcontrol) {
			return true;
		}
		protected virtual void ModifyComponent(XRControl xrcontrol) { 
		}
		protected virtual string GetTransDescription() { 
			return String.Empty;
		}
		protected virtual ArrayList GetSelectedComponents(XRControl parent) {
			ArrayList list = new ArrayList();
			ICollection comps = GetSelectedComponents();
			foreach(IComponent item in comps)
				if(item is XRControl) list.Add(item); 
			return list;
		}
	}
	public class ControlCommandExecutor : ComponentCommandExecutor 
	{	
		#region inner classes
		protected class HorizComparer : IComparer {
			public int Compare(object x, object y) {
				return Comparer.Default.Compare( ((XRControl)x).LeftF, ((XRControl)y).LeftF );
			}
		}
		protected class VertComparer : IComparer {
			public int Compare(object x, object y) {
				return Comparer.Default.Compare( ((XRControl)x).TopF, ((XRControl)y).TopF );
			}
		}
		#endregion
		protected static IComparer fHorizComparer = new HorizComparer();
		protected static IComparer fVertComparer = new VertComparer();
		static CommandID[] vertAlignCommands = new CommandID[] { StandardCommands.AlignBottom, StandardCommands.AlignTop, StandardCommands.AlignHorizontalCenters };
		public ControlCommandExecutor(IDesignerHost designerHost) : base(designerHost) {
		}
		protected XRControl SelComponent(int index) {
			return selComponents[index] as XRControl;
		}
		protected override ArrayList GetSelectedComponents(XRControl parent) {
			ArrayList list = new ArrayList();
			ICollection comps = GetSelectedComponents();
			foreach(IComponent item in comps)
				if(item is XRControl && IsValidControl( (XRControl)item, parent) ) 
					list.Add(item); 
			return list;
		}
		private bool IsValidControl(XRControl ctl, XRControl parent) {
			if(ctl == null || parent == null)
				return false;
			if(ctl.Band == parent.Band && Array.IndexOf<CommandID>(vertAlignCommands, cmdID) >= 0)
				return true;
			if(Array.IndexOf<CommandID>(CommandGroups.StrongCommands, cmdID) >= 0)
				return true;
			return ctl.Parent == parent;
		}
		protected override void ModifyComponent(XRControl xrControl) {
			ModifyControl(xrControl);
		}
		protected virtual void ModifyControl(XRControl xrControl) { 
		}
	}
	public abstract class KeyCommandExecutor: ControlCommandExecutor  {
		static int snapLinesTimerIntervalMagicNumber = 1000;
		AdornerService adornerSerive;
		Timer snapLineTimer;
		protected static PointF ValidateSnap(PointF value, PointF prevValue, SizeF stepSize) {
			return new PointF(ValidateSnap(value.X, prevValue.X, stepSize.Width), ValidateSnap(value.Y, prevValue.Y, stepSize.Height));
		}
		static float ValidateSnap(float value, float prevValue, float step) {
			if(FloatsComparer.Default.FirstGreaterSecond(value - prevValue, step))
				return value - step;
			if(FloatsComparer.Default.FirstGreaterSecond(prevValue - value, step))
				return value + step;
			return value;
		}
		protected KeyCommandExecutor(IDesignerHost designerHost)
			: base(designerHost) {
		}
		protected AdornerService AdornerService {
			get {
				if(adornerSerive == null)
					adornerSerive = (AdornerService)designerHost.GetService(typeof(AdornerService));
				return adornerSerive;
			}
		}
		protected Timer SnapLineTimer {
			get {
				if(snapLineTimer == null) {
					snapLineTimer = new Timer();
					snapLineTimer.Interval = snapLinesTimerIntervalMagicNumber;
					snapLineTimer.Tick += new EventHandler(snapLineTimer_Tick);
				}
				return snapLineTimer;
			}
		}
		protected virtual bool SnapOn {
			get { return SnapToGrid; } 
		}
		protected override bool AllowModifyComponent(XRControl xrcontrol) {
			return LockService.GetInstance(designerHost).CanChangeComponent(xrcontrol) && base.AllowModifyComponent(xrcontrol);
		}
		protected bool CanModifyBandBounds(Band parent, Rectangle rect) {
			if(parent != null) {
				if(!LockService.GetInstance(designerHost).CanChangeComponent(parent))
					return rect.Bottom <= parent.HeightF;
			}
			return true;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.snapLineTimer != null) {
					this.snapLineTimer.Stop();
					this.snapLineTimer.Tick -= new EventHandler(this.snapLineTimer_Tick);
					this.snapLineTimer = null;
				}
			}
			base.Dispose(disposing);
		}
		void snapLineTimer_Tick(object sender, EventArgs e) {
			AdornerService.ResetSnapping();
			snapLineTimer.Stop();
		}
	}
	public abstract class KeyMoveBaseCommandExecutor : KeyCommandExecutor {
		CommandID leftCommand;
		CommandID rightCommand;
		CommandID upCommand;
		CommandID downCommand;
		protected KeyMoveBaseCommandExecutor(IDesignerHost designerHost)
			: base(designerHost) {
			this.modifyPropNames = new string[] { XRComponentPropertyNames.Location };
		}
		protected override string GetTransDescription() { 
			return (selComponents.Count == 1) ? String.Format(DesignSR.TransFmt_OneMove, PrimaryControl.Site.Name) 
				: String.Format(DesignSR.TransFmt_Move, selComponents.Count);
		}
		protected override void ExecuteCore(string[] propertyNames) {
			XRControlDesigner designer = (XRControlDesigner)designerHost.GetDesigner(PrimaryControl);
			if(this.SnapOn && PrimaryControl.SupportSnapLines && selComponents != null && selComponents.Count > 1) {
				PointF location = PrimaryControl.LocationF;
				PointF value = SnapLineHelper.GetNextSnapPosition(PrimaryControl, (XRControl[])selComponents.ToArray(typeof(XRControl)), GetSnapDirection());
				designer.SetLocation(value, DefaultGridSize, RectangleSpecified.Location, true);
				location.X -= value.X;
				location.Y -= value.Y;
				foreach(XRControl xrControl in selComponents) {
					if(xrControl == PrimaryControl)
						continue;
					PointF controlLocation = xrControl.LocationF;
					controlLocation.X -= location.X;
					controlLocation.Y -= location.Y;
					RaiseComponentChanging(xrControl, XRComponentPropertyNames.Location);
					xrControl.LocationF = controlLocation;
					RaiseComponentChanged(xrControl);
				}
			} else
				base.ExecuteCore(propertyNames);
		}
		protected override void ModifyControl(XRControl xrControl) {
			XRControlDesigner designer = (XRControlDesigner)designerHost.GetDesigner(xrControl);
			if(this.SnapOn && xrControl.SupportSnapLines) {
				PointF value = SnapLineHelper.GetNextSnapPosition(xrControl, GetSnapDirection());
				designer.SetLocation(value, DefaultGridSize, RectangleSpecified.Location, true);
				AdornerService.DrawSnapLines(xrControl);
				SnapLineTimer.Start();
			} else if(SnapOn) {
				RectangleSpecified specified = RectangleSpecified.None;
				PointF value = ModifyCore(xrControl.LocationF, this.GridSize, ref specified);
				PointF snappedPoint = xrControl.LocationParent.SnapPoint(new PointF(value.X, Math.Max(0, value.Y)));
				snappedPoint = ValidateSnap(snappedPoint, xrControl.LocationF, this.GridSize);
				if(value.Y < 0)
					snappedPoint.Y = value.Y;
				designer.SetLocation(snappedPoint, GridSize, specified, true);
			} else {
				RectangleSpecified specified = RectangleSpecified.None;
				PointF value = ModifyCore(xrControl.LocationF, DefaultGridSize, ref specified);
				designer.SetLocation(value, DefaultGridSize, specified, true);
			}
		}
		SnapDirection GetSnapDirection() {
			if(object.Equals(cmdID, leftCommand))
				return SnapDirection.Left;
			if(object.Equals(cmdID, rightCommand))
				return SnapDirection.Right;
			if(object.Equals(cmdID, upCommand))
				return SnapDirection.Up;
			if(object.Equals(cmdID, downCommand))
				return SnapDirection.Down;
			return SnapDirection.None;
		}
		private PointF ModifyCore(PointF value, SizeF stepSize, ref RectangleSpecified specified) {
			PointF pt = value;
			if(object.Equals(cmdID, leftCommand)) {
				pt.X -= stepSize.Width;
				specified = RectangleSpecified.X;
			} else if(object.Equals(cmdID, rightCommand)) {
				pt.X += stepSize.Width;
				specified = RectangleSpecified.X;
			} else if(object.Equals(cmdID, upCommand)) {
				pt.Y -= stepSize.Height;
				specified = RectangleSpecified.Y;
			} else if(object.Equals(cmdID, downCommand)) {
				pt.Y += stepSize.Height;
				specified = RectangleSpecified.Y;
			}
			return pt;
		}
		protected void SetMoveCommands(CommandID leftCommand, CommandID rightCommand, CommandID upCommand, CommandID downCommand) {
			this.leftCommand = leftCommand;
			this.rightCommand = rightCommand;
			this.upCommand = upCommand;
			this.downCommand = downCommand;
		}
		protected override bool AllowModifyComponent(XRControl xrcontrol) {
			ResizeService resizeService = (ResizeService)designerHost.GetService(typeof(ResizeService));
			return !(xrcontrol is Band) && base.AllowModifyComponent(xrcontrol) && !resizeService.IsRunning;
		}
	}
	public class KeyMoveCommandExecutor : KeyMoveBaseCommandExecutor 
	{ 
		public KeyMoveCommandExecutor(IDesignerHost designerHost): base(designerHost) {
			SetMoveCommands(MenuCommands.KeyMoveLeft, MenuCommands.KeyMoveRight, MenuCommands.KeyMoveUp, MenuCommands.KeyMoveDown);
		}
		}
	public class KeyNudgeMoveCommandExecutor : KeyMoveBaseCommandExecutor 
	{
		public KeyNudgeMoveCommandExecutor(IDesignerHost designerHost) : base(designerHost) {
			SetMoveCommands(MenuCommands.KeyNudgeLeft, MenuCommands.KeyNudgeRight, MenuCommands.KeyNudgeUp, MenuCommands.KeyNudgeDown);
		}
		protected override bool SnapOn { get { return !SnapToGrid; }  
		}
		} 
	public abstract class KeySizeBaseCommandExecutor: KeyCommandExecutor 
	{ 
		CommandID widthIncreaseCommand;
		CommandID widthDecreaseCommand;
		CommandID heightIncreaseCommand;
		CommandID heightDecreaseCommand;
		protected KeySizeBaseCommandExecutor(IDesignerHost designerHost): base(designerHost) {
			this.modifyPropNames = new string[] { XRComponentPropertyNames.Size };
		}
		protected override string GetTransDescription() { 
			return (selComponents.Count == 1) ? String.Format(DesignSR.TransFmt_OneSize, PrimaryControl.Site.Name) 
				: String.Format(DesignSR.TransFmt_Size, selComponents.Count);
		}
		protected override void ExecuteCore(string[] propertyNames) {
			foreach(XRControl xrControl in selComponents) {
				ModifyControl(xrControl);
			}
		}
		protected override void ModifyControl(XRControl xrControl) {
			XRControlDesignerBase designer = (XRControlDesignerBase)designerHost.GetDesigner(xrControl);
			if(this.SnapOn && xrControl.SupportSnapLines) {
				SizeF value = SnapLineHelper.SnapSize(xrControl, GetSnapDirection());
				designer.SetSize(value, true);
				AdornerService.DrawSnapLines(xrControl);
				SnapLineTimer.Start();
			} else if(this.SnapOn) {
				RectangleSpecified specified = RectangleSpecified.None;
				PointF value = ModifyCore(xrControl.RightBottomF, this.GridSize, ref specified);				
				value = xrControl.RightBottomParent.SnapPoint(value);
				value = ValidateSnap(value, xrControl.RightBottomF, this.GridSize);
				designer.SetRightBottom(value, GridSize, specified, true);
			} else {
				RectangleSpecified specified = RectangleSpecified.None;
				PointF value = ModifyCore(xrControl.RightBottomF, DefaultGridSize, ref specified);
				designer.SetRightBottom(value, DefaultGridSize, specified, true);
			}
		}
		SnapDirection GetSnapDirection() {
			if(object.Equals(cmdID, widthDecreaseCommand)) {
				return SnapDirection.Left;
			} else if(object.Equals(cmdID, widthIncreaseCommand)) {
				return SnapDirection.Right;
			} else if(object.Equals(cmdID, heightDecreaseCommand)) {
				return SnapDirection.Up;
			} else if(object.Equals(cmdID, heightIncreaseCommand)) {
				return SnapDirection.Down;
			}
			return SnapDirection.None;
		}
		PointF ModifyCore(PointF value, SizeF stepSize, ref RectangleSpecified specified) {
			PointF pt = value;
			if(object.Equals(cmdID, widthDecreaseCommand)) {
				pt.X -= stepSize.Width;
				specified = RectangleSpecified.Width;
			} else if(object.Equals(cmdID, widthIncreaseCommand)) {
				pt.X += stepSize.Width;
				specified = RectangleSpecified.Width;
			} else if(object.Equals(cmdID, heightDecreaseCommand)) {
				pt.Y -= stepSize.Height;
				specified = RectangleSpecified.Height;
			} else if(object.Equals(cmdID, heightIncreaseCommand)) {
				pt.Y += stepSize.Height;
				specified = RectangleSpecified.Height;
			}
			return pt;
		}
		protected void SetSizeCommands(CommandID widthIncreaseCommand, CommandID widthDecreaseCommand, CommandID heightIncreaseCommand, CommandID heightDecreaseCommand) {
			this.widthIncreaseCommand = widthIncreaseCommand;
			this.widthDecreaseCommand = widthDecreaseCommand;
			this.heightIncreaseCommand = heightIncreaseCommand;
			this.heightDecreaseCommand = heightDecreaseCommand;
		}
		protected override bool AllowModifyComponent(XRControl xrcontrol) {
			return !(xrcontrol is XtraReport) && base.AllowModifyComponent(xrcontrol);
		}
	}
	public class KeySizeCommandExecutor: KeySizeBaseCommandExecutor 
	{ 
		public KeySizeCommandExecutor(IDesignerHost designerHost): base(designerHost) {
			SetSizeCommands(MenuCommands.KeySizeWidthIncrease, MenuCommands.KeySizeWidthDecrease, MenuCommands.KeySizeHeightIncrease, MenuCommands.KeySizeHeightDecrease);
		}
	}
	public class KeyNudgeSizeCommandExecutor: KeySizeBaseCommandExecutor 
	{ 
		public KeyNudgeSizeCommandExecutor(IDesignerHost designerHost): base(designerHost) {
			SetSizeCommands(MenuCommands.KeyNudgeWidthIncrease, MenuCommands.KeyNudgeWidthDecrease, MenuCommands.KeyNudgeHeightIncrease, MenuCommands.KeyNudgeHeightDecrease);
		}
		protected override bool SnapOn { get { return !SnapToGrid; }  
		}
	}
	public class AlignCommandExecutor : ControlCommandExecutor 
	{
		static RectangleF BoundsFromBand(XRControl ctl, RectangleF rect) {
			return ctl.Parent.RectangleFFromBand(rect);
		}
		public AlignCommandExecutor(IDesignerHost designerHost): base(designerHost) {
			this.modifyPropNames = new string[] { XRComponentPropertyNames.Location };
		}
		protected override string GetTransDescription() { 
			return String.Format(DesignSR.TransFmt_Format, selComponents.Count, "alignment");
		}
		protected override void ModifyControl(XRControl xrControl) { 
			if(xrControl == PrimaryControl)
				return;
			RectangleF rect = PrimaryControl.RectangleFToBand(PrimaryControl.ClientRectangleF);
			rect = BoundsFromBand(xrControl, rect);
			if(cmdID == StandardCommands.AlignLeft) {
				SetControlLeft(xrControl, rect.Left);
			} else if(cmdID == StandardCommands.AlignTop) {
				xrControl.TopF = rect.Top;
			} else if(cmdID == StandardCommands.AlignRight) {
				SetControlRight(xrControl, rect.Right);
			} else if(cmdID == StandardCommands.AlignBottom) {
				xrControl.TopF = rect.Bottom - xrControl.HeightF;
			} else if(cmdID == StandardCommands.AlignHorizontalCenters) {
				xrControl.TopF = rect.Top + (PrimaryControl.HeightF - xrControl.HeightF) / 2;
			} else if(cmdID == StandardCommands.AlignVerticalCenters) {
				SetControlLeft(xrControl, rect.Left + (PrimaryControl.WidthF - xrControl.WidthF) / 2);
			}
		}
		private void SetControlLeft(XRControl xrControl, float left) {
			XRControlDesigner designer = (XRControlDesigner)designerHost.GetDesigner(xrControl);
			SelectionRules selRules = designer.GetSelectionRules();
			if((selRules & SelectionRules.Moveable) > 0)
				xrControl.LeftF = left;
			else if((selRules & SelectionRules.LeftSizeable) > 0) {
				RectangleF rect = xrControl.BoundsF;
				rect.Width = rect.Right - left;
				rect.X = left;
				xrControl.BoundsF = rect;
			}
		}
		private void SetControlRight(XRControl xrControl, float right) {
			XRControlDesigner designer = (XRControlDesigner)designerHost.GetDesigner(xrControl);
			SelectionRules selRules = designer.GetSelectionRules();
			if((selRules & SelectionRules.Moveable) > 0)
				xrControl.LeftF = right - xrControl.WidthF;
			else if((selRules & SelectionRules.RightSizeable) > 0) {
				RectangleF rect = xrControl.BoundsF;
				rect.Width = right - rect.Left;
				xrControl.BoundsF = rect;
			}
		}
	}
	public class SameSizeCommandExecutor : ControlCommandExecutor 
	{ 
		public SameSizeCommandExecutor(IDesignerHost designerHost): base(designerHost) {
			this.modifyPropNames = new string[] { XRComponentPropertyNames.Size };
		}
		protected override string GetTransDescription() { 
			return String.Format(DesignSR.TransFmt_Size, selComponents.Count);
		}
		protected override void ModifyControl(XRControl xrControl) { 
			if(cmdID == StandardCommands.SizeToControl) {
				xrControl.SizeF = PrimaryControl.SizeF;
			} else if(cmdID == StandardCommands.SizeToControlHeight) {
				xrControl.HeightF = PrimaryControl.HeightF;
			} else if(cmdID == StandardCommands.SizeToControlWidth) {
				xrControl.WidthF = PrimaryControl.WidthF;
			}
		}
	}
	public abstract class SpaceCommandExecutor : ControlCommandExecutor {
		#region static
		static protected float CalcAverageValue(float[] values) {
			if(values.Length == 0)
				return 0;
			float sum = 0;
			for(int i = 0; i < values.Length; i++) {
				sum += values[i];
			}
			return sum / values.Length;
		}
		#endregion
		protected float[] spaces;
		protected float averageSpace;
		protected SpaceCommandExecutor(IDesignerHost designerHost)
			: base(designerHost) {
			this.modifyPropNames = new string[] { XRComponentPropertyNames.Location };	
		}
		protected override string GetTransDescription() { 
			return String.Format(DesignSR.TransFmt_Format, selComponents.Count, "spacing");
		}
	} 
	public class HSpaceCommandExecutor : SpaceCommandExecutor 
	{ 
		public HSpaceCommandExecutor(IDesignerHost designerHost): base(designerHost) {
		}
		protected override void Initialize() {
			base.Initialize();
			selComponents.Sort(fHorizComparer);
			spaces = new float[selComponents.Count - 1];
			for(int i = 0; i < selComponents.Count - 1; i++) {
				spaces[i] = SelComponent(i + 1).LeftF - SelComponent(i).RightF;
			}
			averageSpace = CalcAverageValue(spaces);
		}
		protected override void ModifyControl(XRControl xrControl) { 
			int index = selComponents.IndexOf(xrControl);
			int primIndex = selComponents.IndexOf(PrimaryControl);
			float spaceOffset = Math.Abs(index - primIndex) * GridSize.Height;
			if(cmdID == StandardCommands.HorizSpaceConcatenate) {
				if(index > 0)
					xrControl.LeftF = SelComponent(index - 1).RightF;
			} else if(cmdID == StandardCommands.HorizSpaceDecrease) {
				if(xrControl.Equals(PrimaryControl))
					return;
				if(index < primIndex) {
					xrControl.LeftF += spaceOffset;
					XRControl nextControl = SelComponent(index + 1);
					xrControl.LeftF = Math.Min(xrControl.LeftF, nextControl.LeftF - GridSize.Width);
				} else {
					xrControl.LeftF -= spaceOffset;
					XRControl prevControl = SelComponent(index - 1);
					xrControl.LeftF = Math.Max(xrControl.LeftF, prevControl.LeftF + GridSize.Width);
				}
			} else if(cmdID == StandardCommands.HorizSpaceIncrease) {
				if(xrControl.Equals(PrimaryControl))
					return;
				if(index < primIndex)
					xrControl.LeftF -= spaceOffset;
				else 
					xrControl.LeftF += spaceOffset;
			} else if(cmdID == StandardCommands.HorizSpaceMakeEqual) {
				if(index > 0) xrControl.LeftF = SelComponent(index - 1).RightF + averageSpace; 
			}
		}
	} 
	public class VSpaceCommandExecutor : SpaceCommandExecutor 
	{ 
		public VSpaceCommandExecutor(IDesignerHost designerHost): base(designerHost) {
		}
		protected override void Initialize() {
			base.Initialize();
			selComponents.Sort(fVertComparer);
			spaces = new float[selComponents.Count - 1];
			for(int i = 0; i < selComponents.Count - 1; i++) {
				spaces[i] = SelComponent(i + 1).TopF - SelComponent(i).BottomF;
			}
			averageSpace = CalcAverageValue(spaces);
		} 
		protected override void ModifyControl(XRControl xrControl) { 
			int index = selComponents.IndexOf(xrControl);
			int primIndex = selComponents.IndexOf(PrimaryControl);
			float spaceOffset = Math.Abs(index - primIndex) * GridSize.Height; 
			if(cmdID == StandardCommands.VertSpaceConcatenate) {
				if(index > 0) 
					xrControl.TopF = SelComponent(index - 1).BottomF;
			} else if(cmdID == StandardCommands.VertSpaceDecrease) {
				if(xrControl.Equals(PrimaryControl))
					return;
				if(index < primIndex) {
					xrControl.TopF += spaceOffset;
					XRControl nextControl = SelComponent(index + 1);
					xrControl.TopF = Math.Min(xrControl.TopF, nextControl.TopF - GridSize.Height);
				} else {
					xrControl.TopF -= spaceOffset;
					XRControl prevControl = SelComponent(index - 1);
					xrControl.TopF = Math.Max(xrControl.TopF, prevControl.TopF + GridSize.Height);
				}
			} else if(cmdID == StandardCommands.VertSpaceIncrease) {
				if(xrControl.Equals(PrimaryControl))
					return;
				if(index < primIndex)
					xrControl.TopF -= spaceOffset;
				else 
					xrControl.TopF += spaceOffset;
			} else if(cmdID == StandardCommands.VertSpaceMakeEqual) {
				if(index > 0) 
					xrControl.TopF = SelComponent(index - 1).BottomF + averageSpace;
			}
		}
	}
	public class ZOrderCommandExecutor : ControlCommandExecutor 
	{ 
		public ZOrderCommandExecutor(IDesignerHost designerHost): base(designerHost) {
			this.modifyPropNames = new string[] { XRComponentPropertyNames.Index };
		}
		protected override string GetTransDescription() { 
			if(cmdID == StandardCommands.BringToFront)
				return String.Format(DesignSR.TransFmt_BringToFront, selComponents.Count);
			else if(cmdID == StandardCommands.SendToBack)
				return String.Format(DesignSR.TransFmt_SendToBack, selComponents.Count);
			return String.Empty;
		}
		protected override void Initialize() {
			base.Initialize();
			if(cmdID == StandardCommands.BringToFront)
				selComponents.Reverse();
		}
		protected override void ModifyControl(XRControl xrControl) { 
			if(cmdID == StandardCommands.BringToFront) {
				xrControl.BringToFront();
			} else if(cmdID == StandardCommands.SendToBack) {
				xrControl.SendToBack();
			} 
		}
	}
	public class MoveCommandExecutor : ControlCommandExecutor 
	{
		static RectangleF GetComponentRect(XRControl control) {
			return (control is Band) ? ((Band)control).GetClientRectangle() : control != null ?
				control.BoundsF : RectangleF.Empty;
		}
		PointF offset;
		public MoveCommandExecutor(IDesignerHost designerHost): base(designerHost) {
			this.modifyPropNames = new string[] { XRComponentPropertyNames.Location };
		}
		private PointF GetCenterOffset() {
			RectangleF unionRect = GetUnionRect();
			RectangleF baseRect = GetComponentRect(PrimaryControl.Parent);
			baseRect.Location = PointF.Empty;
			RectangleF centerRect = RectHelper.AlignRectangleF(unionRect, baseRect, ContentAlignment.MiddleCenter);
			if(PrimaryControl is XRCrossBandControl)
				return new PointF(centerRect.Left - unionRect.Left, 0);
			return new PointF(centerRect.Left - unionRect.Left, centerRect.Top - unionRect.Top);
		}
		private RectangleF GetUnionRect() {
			RectangleF rect = Rectangle.Empty;
			foreach(XRControl control in selComponents) {
				if(rect.IsEmpty) rect = control.BoundsF;
				else rect = RectangleF.Union(rect, control.BoundsF);
			}
			return rect;
		}
		protected override string GetTransDescription() {
			if(cmdID == StandardCommands.AlignToGrid) {
				return String.Format(DesignSR.TransFmt_AlignToGrid, selComponents.Count);
			} else if(cmdID == StandardCommands.CenterHorizontally) {
				return String.Format(DesignSR.TransFmt_CenterHoriz, selComponents.Count);
			} else if(cmdID == StandardCommands.CenterVertically) {
				return String.Format(DesignSR.TransFmt_CenterVert, selComponents.Count);
			}
			return String.Empty;
		}
		protected override void Initialize() {
			base.Initialize();
			offset = GetCenterOffset();
		}
		protected override void ModifyControl(XRControl xrControl) {
			XRControlDesigner designer = (XRControlDesigner)this.designerHost.GetDesigner(xrControl);
			if(cmdID == StandardCommands.AlignToGrid) {
				PointF location = xrControl.LocationParent.SnapPoint(xrControl.LocationF);
				designer.SetLocation(location, GridSize, RectangleSpecified.Location, true);
			} else {
				PointF value = xrControl.LocationF;
				RectangleSpecified specified = RectangleSpecified.None;
				if(cmdID == StandardCommands.CenterHorizontally) {
					value.X += offset.X;
					specified = RectangleSpecified.X;
				} else if(cmdID == StandardCommands.CenterVertically) {
					value.Y += offset.Y;
					specified = RectangleSpecified.Y;
				}
				if(SnapToGrid) {
					value = xrControl.LocationParent.SnapPoint(value);
					designer.SetLocation(value, GridSize, RectangleSpecified.Location, true);
				} else
					designer.SetLocation(value, DefaultGridSize, specified, true);
			}
		}
	}
	public class SizeCommandExecutor : ControlCommandExecutor 
	{	
		public SizeCommandExecutor(IDesignerHost designerHost) : base(designerHost) {
			this.modifyPropNames = new string[] { XRComponentPropertyNames.Location, XRComponentPropertyNames.Size };
		}
		protected override string GetTransDescription() { 
			return String.Format(DesignSR.TransFmt_SizeToGrid, selComponents.Count);
		}
		protected override void ModifyControl(XRControl xrControl) { 
			if(cmdID == StandardCommands.SizeToGrid) {
				XRControlDesigner designer = (XRControlDesigner)this.designerHost.GetDesigner(xrControl);
				PointF location = xrControl.LocationParent.SnapPoint(xrControl.LocationF);
				designer.SetLocation(location, GridSize, RectangleSpecified.Location, false);
				PointF rightBottom = xrControl.RightBottomParent.SnapPoint(xrControl.RightBottomF);
				designer.SetRightBottom(rightBottom, GridSize, RectangleSpecified.Size, true);
			}
		}
	}
	public class FormatFontCommandExecutor : ComponentCommandExecutor 
	{	
		FontStyle primaryStyle;
		Color foreColor;
		Color backColor;
		public FormatFontCommandExecutor(IDesignerHost designerHost) : base(designerHost) {
			this.modifyPropNames = new string[] { XRComponentPropertyNames.Font };
		}
		protected override string GetTransDescription() { 
			if (modifyPropNames.Length == 1)
				return String.Format(DesignSR.TransFmt_SetProperty, modifyPropNames[0]);
			else
				return DesignSR.TransFmt_Font;
		}
		protected override void Initialize() {
			if(PrimaryControl != null) {
				Font primaryFont = PrimaryControl.GetEffectiveFont().Clone() as Font;
				primaryStyle = primaryFont.Style; 
			}
		}
		public override void ExecCommand(CommandID cmdID) {
			if (object.Equals(cmdID, FormattingCommands.ForeColor))
				this.modifyPropNames = new string[] { XRComponentPropertyNames.ForeColor };
			else if (object.Equals(cmdID, FormattingCommands.BackColor))
				this.modifyPropNames = new string[] { XRComponentPropertyNames.BackColor };
			else
				this.modifyPropNames = new string[] { XRComponentPropertyNames.Font };
			base.ExecCommand(cmdID);
		}
		protected override void ExecuteCore(string[] propertyNames) {
			if(PrimaryControl == null)
				return;
			if (object.Equals(cmdID, FormattingCommands.ForeColor)) {
				if(CanGetColorFromParameter()) {
					foreColor = GetColorFromParameter();
				} else {
					foreColor = ComponentCommandExecutor.CreateControlModifier(PrimaryControl, designerHost).GetForeColor(PrimaryControl, designerHost);
					if (foreColor == Color.Empty)
						return;
				}
			}
			if (object.Equals(cmdID, FormattingCommands.BackColor)) {
				if(CanGetColorFromParameter()) {
					backColor = GetColorFromParameter();
				} else {
					backColor = ComponentCommandExecutor.CreateControlModifier(PrimaryControl, designerHost).GetBackColor(PrimaryControl, designerHost);
					if (backColor == Color.Empty)
						return;
				}
			}
			base.ExecuteCore(propertyNames);
		}
		Color GetColorFromParameter() {
			if(parameters == null || parameters.Length == 0) return Color.Empty;
			try {
				return (Color)parameters[0];
			} catch {
				return Color.Empty;
			}
		}
		bool CanGetColorFromParameter() {
			return GetColorFromParameter() != Color.Empty;
		}
		protected override void ModifyComponent(XRControl xrControl) { 
			ComponentCommandExecutor.CreateControlModifier(xrControl, designerHost).ModifyComponentFontStyle(xrControl, cmdID, primaryStyle, foreColor, backColor, designerHost);
		}
		public override bool OnUpdateCommandCheck(XRControl xrControl, CommandID cmdID) {
			FontSurrogate font = FontSurrogate.FromFont(xrControl.Font);
			if(xrControl is XRRichTextBase) {
				XRRichTextBaseDesigner designer = designerHost.GetDesigner(xrControl) as XRRichTextBaseDesigner;
				if(designer != null && designer.Editor != null && !designer.Editor.SelectionFont.IsEmpty)
					font = designer.Editor.SelectionFont;
			}
			if(cmdID == FormattingCommands.Bold)
				return font.Bold;
			else if(cmdID == FormattingCommands.Italic)
				return font.Italic;
			else if(cmdID == FormattingCommands.Underline)
				return font.Underline;
			else
				return false;
		}
	}
	public class JustifyCommandExecutor : ControlCommandExecutor 
	{ 
		public JustifyCommandExecutor(IDesignerHost designerHost): base(designerHost) {
			this.modifyPropNames = new string[] { XRComponentPropertyNames.TextAlignment };
		}
		protected override string GetTransDescription() { 
			return DesignSR.TransFmt_Justify;
		}
		protected override void ModifyControl(XRControl xrControl) { 
			ComponentCommandExecutor.CreateControlModifier(xrControl, designerHost).ModifyAlignment(xrControl, cmdID, designerHost);
		}
		public override bool OnUpdateCommandCheck(XRControl xrControl, CommandID cmdID) {
			return ComponentCommandExecutor.CreateControlModifier(xrControl, designerHost).ShouldCheckAlignment(xrControl, cmdID);
		}
	}
	public class FontInfoCommandExecutor : ComponentCommandExecutor 
	{	
 		public FontInfoCommandExecutor(IDesignerHost designerHost): base(designerHost) {
			this.modifyPropNames = new string[] { XRComponentPropertyNames.Font };
		}
		protected override string GetTransDescription() { 
			return DesignSR.TransFmt_Font;
		}
		protected override bool AllowModifyComponent(XRControl xrcontrol) {
			return !string.IsNullOrEmpty(GetParameter());
		}
		protected override void ModifyComponent(XRControl xrControl) { 
			string s = GetParameter();
			if(string.IsNullOrEmpty(s)) return;
			ComponentCommandExecutor.CreateControlModifier(xrControl, designerHost).ModifyFont(xrControl, s, cmdID, designerHost);
		}
		string GetParameter() {
			if(parameters == null || parameters.Length == 0) return string.Empty;
			try {
				return Convert.ToString(parameters[0]);
			}
			catch {
				return string.Empty;
			}
		}
	}
	public class ZoomCommandExecutor : CommandExecutorBase {
		public ZoomCommandExecutor(IDesignerHost designerHost) : base(designerHost) {
		}
		public override void ExecCommand(CommandID cmdID) {
			ZoomService zoomService = ZoomService.GetInstance(designerHost);
			if(object.Equals(cmdID, ZoomCommands.Zoom)) {
				object newZoomFactor = ((parameters != null) && (parameters.Length > 0)) ?
					parameters[0] : zoomService.ZoomFactor;
				try {
					zoomService.ZoomFactor = Convert.ToSingle(newZoomFactor);
				} catch(InvalidCastException) {
				}
			}
			if(object.Equals(cmdID, ZoomCommands.ZoomIn))
				zoomService .ZoomIn();
			if(object.Equals(cmdID, ZoomCommands.ZoomOut))
				zoomService .ZoomOut();
		}
	}
	public class ReorderBandsCommandExecutor : ComponentCommandExecutor {
		public ReorderBandsCommandExecutor(IDesignerHost designerHost) : base(designerHost) {
			this.modifyPropNames = new string[] { XRComponentPropertyNames.Level };
		}
		protected override string GetTransDescription() {
			return DesignSR.Trans_ReorderBands;
		}
		public override void ExecCommand(CommandID cmdID) {
			if(selectionServ.PrimarySelection is IMoveableBand)
				base.ExecCommand(cmdID);
		}
		protected override void ModifyComponent(XRControl xrControl) {
			IMoveableBand moveableBand = (IMoveableBand)xrControl;
			if(object.Equals(cmdID, ReorderBandsCommands.MoveUp))
				moveableBand.Move(BandReorderDirection.Up);
			else if(object.Equals(cmdID, ReorderBandsCommands.MoveDown))
				moveableBand.Move(BandReorderDirection.Down);
			else
				System.Diagnostics.Debug.Assert(false);
		}
	}
	public class ControlModifier {
		static string[] keys = { "Top", "Middle", "Bottom" };
		TextAlignment[] arrLeft = { TextAlignment.TopLeft, TextAlignment.MiddleLeft, TextAlignment.BottomLeft };
		TextAlignment[] arrCenter = { TextAlignment.TopCenter, TextAlignment.MiddleCenter, TextAlignment.BottomCenter };
		TextAlignment[] arrRight = { TextAlignment.TopRight, TextAlignment.MiddleRight, TextAlignment.BottomRight };
		TextAlignment[] arrJustify = { TextAlignment.TopJustify, TextAlignment.MiddleJustify, TextAlignment.BottomJustify };
		static Color GetColor(Color defaultColor) {
			ColorDialog dlg = new ColorDialog();
			dlg.AllowFullOpen = true;
			dlg.FullOpen = true;
			dlg.ShowHelp = true;
			dlg.Color = Color.FromArgb(255, defaultColor.R, defaultColor.G, defaultColor.B);
			if (DevExpress.XtraPrinting.Native.DialogRunner.ShowDialog(dlg) == DialogResult.OK)
				return dlg.Color;
			else
				return Color.Empty;
		}
		static void SetPropertyValue(XRControl control, string propertyName, object value) {
			PropertyDescriptor property = TypeDescriptor.GetProperties(control)[XRComponentDesigner.GetStylePropertyName(propertyName)];
			if(property != null)
				property.SetValue(control, value);
		}
		protected virtual Color GetCurrentBackColor(XRControl control, IDesignerHost designerHost) {
			return control.BackColor;
		}
		protected virtual Color GetCurrentForeColor(XRControl control, IDesignerHost designerHost) {
			return control.ForeColor;
			}
		public Color GetBackColor(XRControl control, IDesignerHost designerHost) {
			return GetColor(GetCurrentBackColor(control, designerHost));
		}
		public Color GetForeColor(XRControl control, IDesignerHost designerHost) {
			return GetColor(GetCurrentForeColor(control, designerHost));
		}
		protected static bool TryModifyFont(FontSurrogate font, string param, CommandID cmd) {
			try {
				if(cmd == FormattingCommands.FontName) {
					font.Name = param;
					font.Style = FontServiceBase.GetAvailableFontStyle(param, font.Style);
					return true;
				} else if(cmd == FormattingCommands.FontSize) {
					font.Size = Convert.ToSingle(param);
					return true;
				}
			} catch {
			}
			return false;
		}
		protected virtual void ModifyForeColor(XRControl control, Color foreColor, IDesignerHost designerHost) {
			SetPropertyValue(control, XRComponentPropertyNames.ForeColor, foreColor);
		}
		protected virtual void ModifyBackColor(XRControl control, Color backColor, IDesignerHost designerHost) {
			SetPropertyValue(control, XRComponentPropertyNames.BackColor, backColor);
		}
		public virtual void ModifyComponentFontStyle(XRControl control, CommandID cmd, FontStyle primaryFontStyle, Color foreColor, Color backColor, IDesignerHost designerHost) {
			if (object.Equals(cmd, FormattingCommands.ForeColor))
				ModifyForeColor(control, foreColor, designerHost);
			else if (object.Equals(cmd, FormattingCommands.BackColor))
				ModifyBackColor(control, backColor, designerHost);
			else
				ModifyFontStyleInternal(control, cmd, primaryFontStyle, designerHost);
		}
		protected virtual void ModifyFontStyleInternal(XRControl control, CommandID cmd, FontStyle primaryFontStyle, IDesignerHost designerHost) {
			Font controlFont = control.GetEffectiveFont();
			FontStyle fs = controlFont.Style;
			ChangeFontStyle(cmd, ref fs, primaryFontStyle);
			SetPropertyValue(control, XRComponentPropertyNames.Font, new Font(controlFont, FontServiceBase.GetAvailableFontStyle(controlFont.Name, fs)));
		}
		protected static void ChangeFontStyle(CommandID cmdID, ref FontStyle fs, FontStyle primaryFontStyle) {
			if(cmdID == FormattingCommands.Bold) {
				SetStyleAttribute(ref fs, FontStyle.Bold, primaryFontStyle);
			} else if(cmdID == FormattingCommands.Italic) {
				SetStyleAttribute(ref fs, FontStyle.Italic, primaryFontStyle);
			} else if(cmdID == FormattingCommands.Underline) {
				SetStyleAttribute(ref fs, FontStyle.Underline, primaryFontStyle);
			}
		}
		private static void SetStyleAttribute(ref FontStyle target, FontStyle attr, FontStyle primaryStyle) {
			FontStyle mask = (~primaryStyle) & attr;
			if( IsAttributeExists(target, attr) ) {
				mask = (~mask) ^ attr;
				target &= mask; 
			}
			else target |= mask; 	
		}	
		private static bool IsAttributeExists(FontStyle controlStyle, FontStyle attr) {
			return (controlStyle & attr) == attr;
		}
		public virtual void ModifyAlignment(XRControl xrControl, CommandID cmdID, IDesignerHost designerHost) {
			TextAlignment controlAlign = xrControl.GetEffectiveTextAlignment();
			if(cmdID == FormattingCommands.JustifyLeft) 
				controlAlign = GetValidAlign(arrLeft, controlAlign.ToString(), controlAlign);
			else if(cmdID == FormattingCommands.JustifyCenter) 
				controlAlign = GetValidAlign(arrCenter, controlAlign.ToString(), controlAlign);
			else if(cmdID == FormattingCommands.JustifyRight) 
				controlAlign = GetValidAlign(arrRight, controlAlign.ToString(), controlAlign);
			else if(cmdID == FormattingCommands.JustifyJustify) 
				controlAlign = GetValidAlign(arrJustify, controlAlign.ToString(), controlAlign);
			else
				return; 
			SetPropertyValue(xrControl, XRComponentPropertyNames.TextAlignment, controlAlign);
		}
		public virtual bool ShouldCheckAlignment(XRControl xrControl, CommandID cmdID) {
			TextAlignment controlAlign = xrControl.TextAlignment;
			if(cmdID == FormattingCommands.JustifyLeft) 
				return controlAlign == GetValidAlign(arrLeft, controlAlign.ToString(), controlAlign);
			else if(cmdID == FormattingCommands.JustifyCenter) 
				return controlAlign == GetValidAlign(arrCenter, controlAlign.ToString(), controlAlign);
			else if(cmdID == FormattingCommands.JustifyRight) 
				return controlAlign == GetValidAlign(arrRight, controlAlign.ToString(), controlAlign);
			else if (cmdID == FormattingCommands.JustifyJustify) 
				return controlAlign == GetValidAlign(arrJustify, controlAlign.ToString(), controlAlign);
			return false;
		}
		static TextAlignment GetValidAlign(TextAlignment[] values, string align, TextAlignment controlAlign) { 
			for(int i = 0; i < keys.Length; i++) {
				if(align.IndexOf(keys[i]) > -1) 
					return values[i];
			}
			return controlAlign;
		}
		public virtual void ModifyFont(XRControl control, string param, CommandID cmd, IDesignerHost designerHost) {
			FontSurrogate surrogate = FontSurrogate.FromFont(control.GetEffectiveFont());
			if(TryModifyFont(surrogate, param, cmd))
				SetPropertyValue(control, XRComponentPropertyNames.Font, FontSurrogate.ToFont(surrogate));
		}
	}
	public class RichTextBoxModifier : ControlModifier {
		static InplaceRichTextEditorBase GetRichTextBoxControl(XRControl xrControl, IDesignerHost designerHost) {
			XRRichTextBaseDesigner designer = designerHost.GetDesigner(xrControl) as XRRichTextBaseDesigner;
			System.Diagnostics.Debug.Assert(designer != null);
			return (InplaceRichTextEditorBase)designer.Editor;
		}
		public override void ModifyFont(XRControl xrControl, string param, CommandID cmd, IDesignerHost designerHost) {
			InplaceRichTextEditorBase richTextEditor = GetRichTextBoxControl(xrControl, designerHost);
			FontSurrogate surrogate = !richTextEditor.SelectionFont.IsEmpty ? richTextEditor.SelectionFont : FontSurrogate.FromFont(xrControl.Font);
			if(TryModifyFont(surrogate, param, cmd))
				richTextEditor.SelectionFont = surrogate;
		}
		protected override void ModifyForeColor(XRControl control, Color foreColor, IDesignerHost designerHost) {
			InplaceRichTextEditorBase richTextEditor = GetRichTextBoxControl(control, designerHost);
			richTextEditor.SelectionColor = foreColor;
		}
		protected override void ModifyBackColor(XRControl control, Color backColor, IDesignerHost designerHost) {
			if (DXColor.IsTransparentOrEmpty(backColor))
				backColor = Color.White;
			InplaceRichTextEditorBase richTextEditor = GetRichTextBoxControl(control, designerHost);
			richTextEditor.SelectionBackColor = backColor;
		}
		public override void ModifyAlignment(XRControl control, CommandID cmd, IDesignerHost designerHost) {
			InplaceRichTextEditorBase richTextEditor = GetRichTextBoxControl(control, designerHost);
			if(cmd == FormattingCommands.JustifyLeft) 
				richTextEditor.SelectionAlignment = HorizontalAlignmentEx.Left;
			else if(cmd == FormattingCommands.JustifyCenter) 
				richTextEditor.SelectionAlignment = HorizontalAlignmentEx.Center;
			else if(cmd == FormattingCommands.JustifyRight) 
				richTextEditor.SelectionAlignment = HorizontalAlignmentEx.Right;
			else if(cmd == FormattingCommands.JustifyJustify)
				richTextEditor.SelectionAlignment = HorizontalAlignmentEx.Justify;
		}
		protected override void ModifyFontStyleInternal(XRControl control, CommandID cmd, FontStyle primaryFontStyle, IDesignerHost designerHost) {
			InplaceRichTextEditorBase richTextEditor = GetRichTextBoxControl(control, designerHost);
			FontStyle fontStyle = richTextEditor.SelectionFont.Style;
			ChangeFontStyle(cmd, ref fontStyle, fontStyle);
			FontSurrogate surrogate = !richTextEditor.SelectionFont.IsEmpty ? richTextEditor.SelectionFont : FontSurrogate.FromFont(control.Font);
			surrogate.Style = fontStyle;
			richTextEditor.SelectionFont = surrogate;
		}
		public override bool ShouldCheckAlignment(XRControl xrControl, CommandID cmdID) {
			return false;
		}
		protected override Color GetCurrentBackColor(XRControl control, IDesignerHost designerHost) {
			InplaceRichTextEditorBase richTextEditor = GetRichTextBoxControl(control, designerHost);
			return richTextEditor.SelectionBackColor;
		}
		protected override Color GetCurrentForeColor(XRControl control, IDesignerHost designerHost) {
			InplaceRichTextEditorBase richTextEditor = GetRichTextBoxControl(control, designerHost);
			return richTextEditor.SelectionColor;
		}
	}
}
