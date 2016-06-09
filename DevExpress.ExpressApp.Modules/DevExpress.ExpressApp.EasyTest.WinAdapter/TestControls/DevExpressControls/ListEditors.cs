#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
using DevExpress.ExpressApp.EasyTest.WinAdapter.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Services;
using DevExpress.Services.Implementation;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.XtraTreeList.ViewInfo;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Drawing;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.DevExpressControls {
	public class ListEditorTestControlAct : TestControlInterfaceImplementerBase<IDXManagerPopupMenu>, IGridAct {
		public ListEditorTestControlAct(IDXManagerPopupMenu control) : base(control) { }
		private ITestControl GetPopupMenuActionControl(string actionName) {
			Object actionControl = BarItemFindStrategy.Instance.FindControlInPopupMenu(control as Control, actionName);
			if(actionControl == null) {
				throw new AdapterOperationException(String.Format(
					"Cannot find a control. Action: '{0}', OperationTag: {1}'", actionName, "Action in grid"));
			}
			ITestControl result = TestControlFactoryWin.Instance.CreateControl(actionControl);
			result.Name = actionName;
			return result;
		}
		#region IGridAct Members
		public void GridAct(string actionName, int rowIndex, IGridColumn column) {
			if((actionName == null) || (actionName == "")) {
				TestControl.GetInterface<IGridDoubleClick>().DoubleClickToCell(rowIndex, column);
			}
			else {
				ITestControl result = GetPopupMenuActionControl(actionName);
				result.GetInterface<IControlAct>().Act(null);
			}
		}
		public void CheckGridAct(string actionName, int rowIndex, IGridColumn column, bool isInlineOnly) {
			ITestControl result = GetPopupMenuActionControl(actionName);
			if(!result.GetInterface<IControlEnabled>().Enabled) {
				throw new AdapterOperationException(string.Format("The '{0}' action of '{1}' table is not enabled", actionName, TestControl.Name));
			}
		}
		#endregion
	}
	public class TestColumn<T> : MarshalByRefObject {
		private WeakReference column_;
		protected T column {
			get {
				return (T)column_.Target;
			}
		}
		public TestColumn(T column) {
			this.column_ = new WeakReference(column);
		}
	}
	public class DXGridColumn : TestColumn<GridColumn>, IGridColumn {
		public DXGridColumn(GridColumn column)
			: base(column) {
		}
		public GridColumn Column {
			get {
				return column;
			}
		}
		#region IGridColumn Members
		public string Caption {
			get {
				if(!String.IsNullOrEmpty(column.Caption)) {
					return column.Caption;
				}
				return column.ToString();
			}
		}
		public bool Visible {
			get { return column.Visible; }
		}
		#endregion
	}
	public class GridTestControl : TestControlInterfaceImplementerBase<GridControl>,
		IGridBase, IGridRowsSelection, IGridDoubleClick, IGridCellControlCreation, IGridControlAct {
		private DevExpress.XtraGrid.Views.Grid.GridView gridView {
			get {
				return (DevExpress.XtraGrid.Views.Grid.GridView)control.DefaultView;
			}
		}
		public GridTestControl(GridControl control)
			: base(control) {
		}
		[DllImport("user32.dll", EntryPoint = "SetCursorPos")]
		extern static bool SetCursorPos(int X, int Y);
		private void PrepareGrid() {
			bool isAllExpanded = true;
			foreach(DevExpress.Data.GroupRowInfo info in gridView.DataController.GroupInfo) {
				if(!info.Expanded) {
					isAllExpanded = info.Expanded;
					break;
				}
			}
			if(gridView.GroupCount > 0 && !isAllExpanded) {
				gridView.ExpandAllGroups();
				gridView.LayoutChanged();
			}
		}
		private void RaiseDoubleClick() {
			Type type = control.GetType();
			MethodInfo mi = type.GetMethod("OnDoubleClick", BindingFlags.NonPublic | BindingFlags.Instance);
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(control, EventArgs.Empty);
			mi.Invoke(control, new object[] { new DXMouseEventArgs(MouseButtons.Left, e.Clicks, e.X, e.Y, e.Delta) });
		}
		private void MouseClick(int hWnd, uint x, uint y) {
		}
		private void RaiseMouseDownClick() {
			Type type = gridView.GetType();
			MethodInfo mi = type.GetMethod("RaiseMouseDown", BindingFlags.NonPublic | BindingFlags.Instance);
			Point position = new Point(80, 80);
			position = gridView.GridControl.PointToScreen(position);
			MouseEventArgs e = new MouseEventArgs(MouseButtons.Right, 1, 60, 70, 0);
			mi.Invoke(gridView, new object[] { e });
		}
		private GridColumn GetFirstUnGroupedColumn(GridView view) {
			foreach(GridColumn column in view.Columns) {
				if(column.Visible && column.GroupIndex < 0) {
					return column;
				}
			}
			throw new AdapterOperationException(string.Format("Cannot find an ungrouped column in the {2} table.", TestControl.Name));
		}
		private System.Drawing.Rectangle GetCellRect(GridView view, int rowHandle, GridColumn column) {
			GridViewInfo info;
			Type type = view.GetType();
			FieldInfo fi = type.GetField("fViewInfo", BindingFlags.NonPublic | BindingFlags.Instance);
			info = fi.GetValue(view) as GridViewInfo;
			if(column.GroupIndex >= 0) {
				column = GetFirstUnGroupedColumn(view);
			}
			GridCellInfo cell = info.GetGridCellInfo(rowHandle, column);
			if(cell == null) {
				throw new AdapterOperationException(string.Format("Cannot select the ({0}, {1}) invisible cell in the '{2}' table", rowHandle, column.AbsoluteIndex, TestControl.Name));
			}
			return cell.Bounds;
		}
		public void DoubleClickToCell(int row, IGridColumn column) {
			PrepareGrid();
			SetFocusToCell(row, column);
			RaiseDoubleClick();
		}
		private void RaiseKeyDownEnter() {
			Type type = control.GetType();
			MethodInfo mi = type.GetMethod("OnKeyDown", BindingFlags.NonPublic | BindingFlags.Instance);
			mi.Invoke(control, new object[] { new KeyEventArgs(Keys.Enter) });
		}
		public void SetFocusToCell(int row, IGridColumn column) {
			PrepareGrid();
			control.Focus();
			gridView.ClearSelection();
			gridView.FocusedRowHandle = row;
			gridView.SelectRow(row);
			gridView.MakeRowVisible(row, true);
			gridView.FocusedColumn = ((DXGridColumn)column).Column;
			System.Drawing.Rectangle r = Rectangle.Empty;
			r = GetCellRect(gridView, row, ((DXGridColumn)column).Column);
			Point point = new Point(r.X, r.Y);
			point = control.PointToScreen(point);
			SetCursorPos(point.X, point.Y);
		}
		#region IGridRowsSelection Members
		public bool IsRowSelected(int rowIndex) {
			return Array.IndexOf(gridView.GetSelectedRows(), rowIndex) != -1;
		}
		public void ClearSelection() {
			PrepareGrid();
			gridView.ClearSelection();
		}
		public void SelectRow(int row) {
			PrepareGrid();
			gridView.FocusedRowHandle = row;
			gridView.SelectRow(row);
		}
		public void UnselectRow(int rowIndex) {
			gridView.UnselectRow(rowIndex);
		}
		#endregion
		#region IGridBase Members
		public IEnumerable<IGridColumn> Columns {
			get {
				foreach(GridColumn column in gridView.Columns) {
					yield return new DXGridColumn(column);
				}
			}
		}
		public string GetCellValue(int row, IGridColumn column) {
			PrepareGrid();
			GridColumn gridColumn = ((DXGridColumn)column).Column;
			string displayText = gridView.GetRowCellDisplayText(row, gridColumn);
			object value = gridView.GetRowCellValue(row, gridColumn);
			if(value is bool) {
				if(string.IsNullOrEmpty(displayText) || displayText == "Checked" || displayText == "Unchecked") {
					displayText = value.ToString();	
				}
			}
			return displayText;
		}
		public int GetRowCount() {
			PrepareGrid();
			return gridView.RowCount;
		}
		#endregion
		#region IGridCellControlCreation Members
		public ITestControl CreateCellControl(int row, IGridColumn column) {
			gridView.FocusedColumn = ((DXGridColumn)column).Column;
			gridView.ShowEditor();
			if(gridView.ActiveEditor == null) {
				throw new AdapterOperationException(String.Format("Cannot get the ActiveEditor for the '{0}' table's ({1}, {2}) cell", TestControl.Name, row, column.Caption));
			}
			return TestControlFactoryWin.Instance.CreateControl(gridView.ActiveEditor);
		}
		public void BeginEdit(int row) {
			gridView.FocusedRowHandle = row;
		}
		public void EndEdit() {
			if(gridView.ActiveEditor != null) {
				RaiseKeyDownEnter();
			}
		}
		#endregion
		#region IGridControlAct Members
		public void GridActEx(string actionName, int rowIndex, IGridColumn column, string[] param) {
			if(actionName == "Action" && param != null && param.Length == 1) {
				actionName = param[0];
				param = new string[] { "" };
			}
			switch(actionName) {
				case "InlineNew":
					gridView.FocusedRowHandle = GridControl.NewItemRowHandle;
					Application.DoEvents();
					break;
				case "InlineEdit":
					break;
				case "InlineUpdate":
					gridView.FocusedRowHandle = 0;
					break;
				case "InlineCancel":
					break;
				case "SetTableFilter":
					gridView.FocusedRowHandle = GridControl.AutoFilterRowHandle;
					FillRecordCommand fillRecordCommand = new FillRecordCommand();
					int separator = param[0].IndexOf("=");
					string columnName = param[0].Substring(0, separator);
					string value = param[0].Substring(separator + 1);
					if(string.IsNullOrEmpty(value)) {
						value = "''";
					}
					fillRecordCommand.Parameters.MainParameter = new MainParameter("");
					fillRecordCommand.Parameters.Add(new Parameter(" Columns = " + columnName, new PositionInScript(0)));
					fillRecordCommand.Parameters.Add(new Parameter(" Values = " + value, new PositionInScript(1)));
					ICommandAdapter commandAdapter = new WinEasyTestCommandAdapter();
					fillRecordCommand.Execute(commandAdapter);
					break;
			}
		}
		#endregion
	}
	[Serializable]
	public class TreeListGridColumn : TestColumn<TreeListColumn>, IGridColumn {
		public TreeListGridColumn(TreeListColumn column) : base(column) { }
		public TreeListColumn Column {
			get {
				return column;
			}
		}
		#region IGridColumn Members
		public string Caption {
			get { return column.Caption; }
		}
		public bool Visible {
			get { return column.Visible; }
		}
		#endregion
	}
	public class TreeListTestControl : TestControlInterfaceImplementerBase<TreeList>,
		IGridBase, IGridRowsSelection, IGridDoubleClick {
		public TreeListTestControl(TreeList control)
			: base(control) {
		}
		protected virtual void ExpandAll() {
			control.ExpandAll();
		}
		protected virtual TreeListOperationCount CreateTreeListOperationCount(TreeListNodes nodes) {
			return new TreeListOperationFullCount(nodes);
		}
		#region IGridRowsSelection Members
		public bool IsRowSelected(int rowIndex) {
			foreach(DevExpress.XtraTreeList.Nodes.TreeListNode item in control.Selection) {
				if(rowIndex == GetTreeListNodeIndex(item)) {
					return true;
				}
			}
			return false;
		}
		protected virtual int GetTreeListNodeIndex(TreeListNode item) {
			return item.Id;
		}
		public virtual void UnselectRow(int rowIndex) {
			control.Selection.Remove(control.GetNodeByVisibleIndex(rowIndex));
		}
		public virtual void ClearSelection() {
			control.Selection.Clear();
			control.FocusedNode = null;
		}
		public virtual void SelectRow(int rowIndex) {
			control.FocusedNode = control.GetNodeByVisibleIndex(rowIndex);
		}
		#endregion
		#region IGridBase Members
		public IEnumerable<IGridColumn> Columns {
			get {
				foreach(TreeListColumn column in control.Columns) {
					yield return new TreeListGridColumn(column);
				}
			}
		}
		public string GetCellValue(int row, IGridColumn column) {
			TreeListNode treeListNode = control.GetNodeByVisibleIndex(row);
			Stack<string> values = new Stack<string>();
			while(treeListNode != null) {
				object columnValue = treeListNode.GetValue(((TreeListGridColumn)column).Column);
				values.Push((columnValue == null || string.IsNullOrEmpty(columnValue.ToString()) ? "''" : columnValue.ToString()));
				treeListNode = treeListNode.ParentNode;
			}
			return string.Join(".", values.ToArray());
		}
		public int GetRowCount() {
			ExpandAll();
			TreeListOperationCount operation = CreateTreeListOperationCount(control.Nodes);
			control.NodesIterator.DoOperation(operation);
			return operation.Count;
		}
		#endregion
		#region IGridDoubleClick Members
		private void RaiseDoubleClick(RowInfo rowInfo) {
			CellInfo cellInfo = (CellInfo)rowInfo.Cells[control.FocusedColumn.VisibleIndex];
			if(cellInfo == null) {
				throw new WarningException("rowInfo.Cells[control.FocusedColumn.VisibleIndex] value is null");
			}
			Rectangle cellBounds = cellInfo.Bounds;
			if(cellBounds == Rectangle.Empty) {
				throw new WarningException("The cellInfo.Bounds property value is Rectangle.Empty");
			}
			Type type = control.GetType();
			MethodInfo mi = type.GetMethod("OnMouseDown", BindingFlags.NonPublic | BindingFlags.Instance);
			mi.Invoke(control, new object[] { new MouseEventArgs(MouseButtons.Left, 2, cellBounds.X, cellBounds.Y, 0) });
			mi = type.GetMethod("OnMouseClick", BindingFlags.NonPublic | BindingFlags.Instance);
			mi.Invoke(control, new object[] { new MouseEventArgs(MouseButtons.Left, 2, cellBounds.X, cellBounds.Y, 0) });
			mi = type.GetMethod("OnDoubleClick", BindingFlags.NonPublic | BindingFlags.Instance);
			mi.Invoke(control, new object[] { new MouseEventArgs(MouseButtons.Left, 2, cellBounds.X, cellBounds.Y, 0) });
		}
		public virtual void SetFocusToCell(int row, IGridColumn column) {
			control.FocusedColumn = ((TreeListGridColumn)column).Column;
			control.FocusedNode = control.GetNodeByVisibleIndex(row);
			control.MakeNodeVisible(control.FocusedNode);
		}
		private RowInfo GetRowInfo() {
			RowInfo rowInfo = null;
			rowInfo = control.ViewInfo.RowsInfo[control.FocusedNode];
			return rowInfo;
		}
		public void DoubleClickToCell(int row, IGridColumn column) {
			ExpandAll();
			Application.DoEvents();
			control.Invalidate();
			Application.DoEvents();
			SetFocusToCell(row, column);
			Application.DoEvents();
			RowInfo rowInfo = GetRowInfo();
			if(rowInfo == null) {
				throw new WarningException("It's impossible to get rowInfo for TreeListCell");
			}
			else {
				RaiseDoubleClick(rowInfo);
			}
		}
		#endregion
	}
	[Serializable]
	public class SchedulerColumn : MarshalByRefObject, IGridColumn {
		private string caption;
		public SchedulerColumn(string caption) {
			this.caption = caption;
		}
		#region IGridColumn Members
		public string Caption {
			get { return caption; }
		}
		public bool Visible {
			get { return true; }
		}
		#endregion
	}
	public class SchedulerTestControl : TestControlInterfaceImplementerBase<SchedulerControl>,
		IGridBase, IGridRowsSelection, IGridDoubleClick, IGridControlAct, IControlDragDrop, IGridCellControlCreation {
		private const int indent = 5;
		public const string CheckVisibleResourcesCommandSignature = "CheckVisibleResources";
		public const string CheckSelectedEventResourcesCommandSignature = "CheckSelectedEventResources";
		public const string CheckAppointmentTypeCommandSignature = "CheckAppointmentType";
		public const string ChangeViewToCommandSignature = "ChangeViewTo";
		public const string SelectIntervalCommandSignature = "SelectInterval";
		public const string SetCurrentDateCommandSignature = "SetCurrentDate";
		public SchedulerTestControl(SchedulerControl control)
			: base(control) {
		}
		private void WaitAnimation() {
			DateTime startTime = DateTime.Now;
			while(control.GetService<ISchedulerStateService>().IsAnimationActive) {
				if((DateTime.Now - startTime).TotalSeconds > 2) {
					break;
				}
				Application.DoEvents();
				Thread.Sleep(20);
			}
		}
		private Appointment GetAppointment(int row) {
			return control.ActiveView.GetAppointments()[row];
		}
		private MouseHandler GetMouseHandler() {
			return ((MouseHandlerService)control.GetService(typeof(IMouseHandlerService))).Handler;
		}
		private Appointment FindAppointment(SchedulerColumn[] columns, string[] propertyValues) {
			Appointment result = null;
			int rowCount = GetRowCount();
			for(int i = 0; i < rowCount; i++) {
				Appointment apt = GetAppointment(i);
				bool failed = false;
				for(int j = 0; j < columns.Length; j++) {
					if(GetAppointmentPropertyValue(apt, columns[j].Caption) != propertyValues[j]) {
						failed = true;
						break;
					}
				}
				if(!failed) {
					result = apt;
					break;
				}
			}
			return result;
		}
		private Appointment FindAppointmentBySubject(string subject) {
			return FindAppointment(new SchedulerColumn[] { new SchedulerColumn("Subject") }, new string[] { subject });
		}
		private void ExecuteCheckVisibleResourcesCommand(string[] parameters) {
			int resourceCount;
			if(Int32.TryParse(parameters[0], out resourceCount)) {
				if(resourceCount != control.ActiveView.ViewInfo.VisibleResources.Count) {
					throw new EasyTestException(String.Format("The number of the resources doesn't equal {0}", resourceCount));
				}
			}
			else {
				List<String> resourceCaptions = new List<String>(parameters);
				foreach(Resource resource in control.ActiveView.ViewInfo.VisibleResources) {
					if(resourceCaptions.Contains(resource.Caption)) {
						resourceCaptions.Remove(resource.Caption);
					}
					else {
						throw new EasyTestException("Scheduler resources are inconsistent");
					}
				}
				if(resourceCaptions.Count > 0 && !String.IsNullOrEmpty(resourceCaptions[0])) {
					throw new EasyTestException("Scheduler resources are inconsistent");
				}
			}
		}
		private void ExecuteCheckSelectedEventResourcesCommand(string[] parameters) {
			foreach(Appointment appointment in control.SelectedAppointments) {
				List<String> resourceCaptions = new List<String>(parameters);
				foreach(object resourceId in appointment.ResourceIds) {
					Resource resource = control.DataStorage.Resources.Items.GetResourceById(resourceId);
					if(resourceCaptions.Contains(resource.Caption)) {
						resourceCaptions.Remove(resource.Caption);
					}
					else {
						throw new EasyTestException("Scheduler resources are inconsistent");
					}
				}
				if(resourceCaptions.Count > 0) {
					throw new EasyTestException("Scheduler resources are inconsistent");
				}
			}
		}
		private void ExecuteCheckAppointmentTypeCommand(string commandParameter) {
			AppointmentType appointmentType = (AppointmentType)Enum.Parse(typeof(AppointmentType), commandParameter);
			if(control.SelectedAppointments.Count > 0) {
				foreach(Appointment appointment in control.SelectedAppointments) {
					if(appointment.Type != appointmentType) {
						throw new EasyTestException(String.Format("The selected appointments type isn't {0}", appointmentType));
					}
				}
			}
		}
		private void ExecuteSelectIntervalCommand(string commandParameter) {
			DateTime startDateTime;
			DateTime endDateTime;
			Resource resource = null;
			try {
				string[] parts = commandParameter.Split(';');
				startDateTime = DateTime.Parse(parts[0]);
				endDateTime = DateTime.Parse(parts[1]);
				if(parts.Length > 2) {
					resource = FindResourceByCaption(parts[2]);
					if(resource == null) {
						throw new AdapterOperationException(String.Format("Cannot find resource with caption '{0}'", parts[2]));
					}
				}
			}
			catch {
				throw new AdapterOperationException(String.Format("Check the '{0}' parameter value. Use 'StartDateTime;EndDateTime' or 'StartDateTime;EndDateTime;ResourceCaption' format for the 'SelectInterval' command", commandParameter));
			}
			TimeInterval selectedInterval = control.Services.Selection.SelectedInterval;
			selectedInterval.Start = startDateTime;
			selectedInterval.End = endDateTime;
			if(resource != null) {
				control.Services.Selection.SelectedResource = resource;
			}
			control.Services.Selection.SelectedInterval = selectedInterval;
		}
		private string GetCommandName(string commandString) {
			string result = commandString;
			int startIndex = commandString.IndexOf('(');
			if(startIndex > -1) {
				result = commandString.Substring(0, startIndex);
			}
			return result;
		}
		private Rectangle GetAppointmentBounds(Appointment appointment) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(appointment, "appointment");
			AppointmentViewInfoCollection infos = DevExpress.XtraScheduler.Internal.Diagnostics.Xaf.ViewInfoTestHelper.ObtainAppointmentViewInfos(control.ActiveView);
			AppointmentViewInfo aptViewInfo = infos.Find((item) => { return item.Appointment == appointment; });
			if(aptViewInfo == null) {
				throw new Exception("Cannot find AppointmentViewInfo object.");
			}
			return DevExpress.XtraScheduler.Internal.Diagnostics.Xaf.ViewInfoTestHelper.GetAppointmentViewInfoBounds(control.ActiveView.ViewInfo, aptViewInfo);
		}
		private Rectangle GetDateTimeBounds(DateTime date, string resourceCaption) {
			bool resourceFound = String.IsNullOrEmpty(resourceCaption) || resourceCaption == "(Any)";
			foreach(Resource resource in control.ActiveView.GetResources()) {
				if(resource.Caption == resourceCaption) {
					resourceFound = true;
				}
			}
			if(!resourceFound) {
				throw new AdapterOperationException("The active view doesn't contain the '{0}' resource");
			}
			Rectangle result = new Rectangle();
			foreach(DevExpress.XtraScheduler.Drawing.SchedulerViewCellContainer container in control.ActiveView.ViewInfo.CellContainers) {
				if(String.IsNullOrEmpty(resourceCaption) || container.Resource.Caption == resourceCaption) {
					foreach(DevExpress.XtraScheduler.Drawing.SchedulerViewCellBase cell in container.Cells) {
						if(cell.Interval.Start <= date && cell.Interval.End > date) {
							result = cell.Bounds;
							break;
						}
					}
				}
			}
			return result;
		}
		private Point GetTopLeftCorner(Rectangle bounds) {
			return new Point(bounds.X + indent, bounds.Y + indent);
		}
		internal void SetCellValue(int row, IGridColumn column, string text) {
			SynchronousMethodExecutor.Instance.Execute("SetCellValue", delegate() {
				try {
					object appointment = GetAppointment(row);
					if(appointment == null) {
						throw new InvalidOperationException("appointment == null");
					}
					PropertyInfo propertyInfo = TypeHelper.GetProperty(appointment.GetType(), column.Caption);
					object value = Convert.ChangeType(text, propertyInfo.PropertyType);
					propertyInfo.SetValue(appointment, value, null);
				}
				catch(Exception e) {
					throw new EasyTestException(String.Format("An error occurred while setting a cell's value: {0}", e.Message));
				}
			});
		}
		private Resource FindResource(string[] propertyNames, string[] propertyValues) {
			Resource result = null;
			foreach(Resource resource in control.ActiveView.GetResources()) {
				bool failed = false;
				for(int i = 0; i < propertyNames.Length; i++) {
					if(typeof(Resource).GetProperty(propertyNames[i]).GetValue(resource, null).ToString() != propertyValues[i]) {
						failed = true;
					}
				}
				if(!failed) {
					result = resource;
					break;
				}
			}
			return result;
		}
		private Resource FindResourceByCaption(string resourceCaption) {
			return FindResource(new string[] { "Caption" }, new string[] { resourceCaption });
		}
		private Resource FindResourceById(string resourceId) {
			return FindResource(new string[] { "Id" }, new string[] { resourceId });
		}
		#region ITestControlDragDrop Members
		private enum DragEventOperations { DropTo, DropStart, DropEnd }
		public void DragDrop(string source, string sourceId, string dropTo, string dropToValue) {
			WaitAnimation();
			if(source != "Event") {
				throw new AdapterOperationException("The 'source' argument should be 'Event'");
			}
			Appointment apt = FindAppointmentBySubject(sourceId);
			if(apt == null) {
				throw new AdapterOperationException("Cannot find the '" + sourceId + "' appointment");
			}
			String[] dropToParameters = dropToValue.Split(',');
			DateTime dropToDate;
			if(!DateTime.TryParse(dropToParameters[0], out dropToDate)) {
				throw new AdapterOperationException("The '" + dropToParameters[0] + "' is not a DateTime value");
			}
			String resourceCaption = null;
			if(dropToParameters.Length == 2) {
				resourceCaption = dropToParameters[1].Trim();
			}
			else {
				if(apt.ResourceId == ResourceBase.Empty.Id) {
					resourceCaption = ResourceBase.Empty.Caption;
				}
				else if(apt.ResourceId is ResourceBase) {
					resourceCaption = ((ResourceBase)apt.ResourceId).Caption;
				}
				else {
					resourceCaption = FindResourceById(apt.ResourceId.ToString()).Caption;
				}
			}
			DragEventOperations operationType = (DragEventOperations)Enum.Parse(typeof(DragEventOperations), dropTo);
			if(control.OptionsCustomization.AllowAppointmentDrag != UsedAppointmentType.All
			  || control.OptionsCustomization.AllowAppointmentDragBetweenResources != UsedAppointmentType.All) {
				throw new WarningException("The Drag operatoin isn't supported by the control");
			}
			DragDrop(operationType, apt, dropToDate, resourceCaption);
		}
		private void DragDrop(DragEventOperations operationType, Appointment apt, DateTime dropToDate, String resourceCaption) {
			Rectangle boundsFrom = GetAppointmentBounds(apt);
			ActiveActions actions = new ActiveActions();
			actions.LockEvents();
			actions.MouseDown(control, GetDragPoint(boundsFrom, operationType));
			bool scrollLeft = false;
			if(dropToDate < control.ActiveView.GetVisibleIntervals().Start) {
				scrollLeft = true;
			}
			if(control.ActiveViewType == SchedulerViewType.Timeline) {
				while(!control.ActiveView.GetVisibleIntervals().Contains(new TimeInterval(dropToDate, dropToDate))) {
					actions.UnlockEvents();
					actions.MoveMousePointTo(control, GetControlEdgePoint(scrollLeft, GetDragPoint(boundsFrom, operationType)));
					actions.LockEvents();
				}
			}
			Rectangle boundsTo = GetDateTimeBounds(dropToDate, resourceCaption);
			if(boundsTo.IsEmpty) {
				throw new AdapterOperationException("Cannot find the '" + dropToDate + "' within the active view");
			}
			actions.MouseUp(control, GetDragPoint(GetDateTimeBounds(dropToDate, resourceCaption), operationType));
			actions.UnlockEvents();
		}
		private Point GetControlEdgePoint(bool scrollLeft, Point appointmentDragPoint) {
			if(scrollLeft) {
				return new Point(control.ActiveView.ViewInfo.Bounds.Left - 1, appointmentDragPoint.Y);
			}
			else {
				return new Point(control.ActiveView.ViewInfo.Bounds.Right - 1, appointmentDragPoint.Y);
			}
		}
		private Point GetDragPoint(Rectangle bounds, DragEventOperations operationType) {
			switch(operationType) {
				case DragEventOperations.DropTo:
					return new Point(bounds.Left + indent, bounds.Top + indent);
				case DragEventOperations.DropStart:
					if(control.ActiveViewType == SchedulerViewType.Timeline) {
						return new Point(bounds.Left + 1, bounds.Top + indent);
					}
					else {
						return new Point(bounds.Left + indent, bounds.Top + 1);
					}
				case DragEventOperations.DropEnd:
					if(control.ActiveViewType == SchedulerViewType.Timeline) {
						return new Point(bounds.Right - 1, bounds.Top + indent);
					}
					else {
						return new Point(bounds.Left + indent, bounds.Bottom - 1);
					}
				default:
					throw new ArgumentException("operationType");
			}
		}
		#endregion
		#region IGridRowsSelection Members
		public bool IsRowSelected(int rowIndex) {
			foreach(Appointment selectedAppointment in control.SelectedAppointments) {
				AppointmentViewInfoCollection appointmentViewInfos = DevExpress.XtraScheduler.Internal.Diagnostics.Xaf.ViewInfoTestHelper.ObtainAppointmentViewInfos(control.ActiveView);
				foreach (DevExpress.XtraScheduler.Drawing.AppointmentViewInfo appointmentViewInfo in appointmentViewInfos) {
					if(appointmentViewInfo.Appointment == selectedAppointment) {
						return DevExpress.XtraScheduler.Internal.Diagnostics.Xaf.ViewInfoTestHelper.ObtainAppointmentViewInfos(control.ActiveView).IndexOf(appointmentViewInfo) == rowIndex;
					}
				}
			}
			return false;
		}
		public void UnselectRow(int rowIndex) {
			DevExpress.XtraScheduler.Internal.Diagnostics.Xaf.ViewInfoTestHelper.ObtainAppointmentViewInfos(control.ActiveView).RemoveAt(rowIndex);
		}
		public void ClearSelection() {
			control.SelectedAppointments.Clear();
		}
		public void SelectRow(int rowIndex) {
			WaitAnimation();
			Appointment appoitmentToSelect = GetAppointment(rowIndex);
			SchedulerViewBase activeView = control.ActiveView;
			control.ActiveView.SelectAppointment(appoitmentToSelect);
			Rectangle bounds = GetAppointmentBounds(appoitmentToSelect);
			MouseEventArgs mouseEventArgs = new MouseEventArgs(MouseButtons.Left, 1, bounds.X + indent, bounds.Y + indent, 0);
			MouseHandler mouseHandler = GetMouseHandler();
			mouseHandler.OnMouseDown(mouseEventArgs);
			Thread.Sleep(10);
			mouseHandler.OnMouseUp(mouseEventArgs);
		}
		#endregion
		#region IGridBase Members
		public IEnumerable<IGridColumn> Columns {
			get {
				return new SchedulerColumn[] { 
				new SchedulerColumn("Subject"), 
				new SchedulerColumn("Start"), 
				new SchedulerColumn("End"), 
				new SchedulerColumn("ResourceIDs") 
				};
			}
		}
		public string GetAppointmentPropertyValue(Appointment appointment, string propertyName) {
			PropertyInfo appointmentPropertyInfo = appointment.GetType().GetProperty(propertyName);
			if(appointmentPropertyInfo != null) {
				return appointmentPropertyInfo.GetValue(appointment, null).ToString();
			}
			else {
				throw new MissingMemberException("The " + propertyName + " property is absent in the " + appointment.GetType().FullName + " type");
			}
		}
		public string GetCellValue(int row, IGridColumn column) {
			Appointment appointment = GetAppointment(row);
			return GetAppointmentPropertyValue(appointment, column.Caption);
		}
		public int GetRowCount() {
			return control.ActiveView.GetAppointments().Count;
		}
		#endregion
		#region IGridDoubleClick Members
		public void DoubleClickToCell(int row, IGridColumn column) {
			WaitAnimation();
			ActiveActions activeActions = new ActiveActions();
			activeActions.LockEvents();
			AppointmentViewInfoCollection aptViewInfos = DevExpress.XtraScheduler.Internal.Diagnostics.Xaf.ViewInfoTestHelper.ObtainAppointmentViewInfos(control.ActiveView);
			Rectangle bounds = DevExpress.XtraScheduler.Internal.Diagnostics.Xaf.ViewInfoTestHelper.GetAppointmentViewInfoBounds(control.ActiveView.ViewInfo, aptViewInfos[row]);
			Point mousePoint = GetMousePoint(bounds, control.ClientRectangle);
			activeActions.MouseDblClick(control, mousePoint);
			activeActions.UnlockEvents();
		}
		private Point GetMousePoint(Rectangle cellRectangle, Rectangle controlRectangle) {
			Point result = new Point(cellRectangle.X + cellRectangle.Width / 2, cellRectangle.Y + cellRectangle.Height / 2);
			if(controlRectangle.Height < result.Y) {
				result.Y = cellRectangle.Y + ((controlRectangle.Height - cellRectangle.Y) / 2);
			}
			return result;
		}
		#endregion
		#region IGridCellControlCreation Members
		public void BeginEdit(int row) {
		}
		public ITestControl CreateCellControl(int row, IGridColumn column) {
			TestControlBase result = new TestControlBase(this);
			result.AddInterface(typeof(IControlText), new SchedulerInplace(this, row, column));
			return result;
		}
		public void EndEdit() {
		}
		#endregion
		public class SchedulerInplace : MarshalByRefObject, IControlText, IDisposable {
			SchedulerTestControl scheduler;
			int row;
			IGridColumn column;
			public SchedulerInplace(SchedulerTestControl scheduler, int row, IGridColumn column) {
				this.scheduler = scheduler;
				this.row = row;
				this.column = column;
			}
			#region IControlText Members
			public string Text {
				get {
					throw new NotImplementedException();
				}
				set {
					scheduler.SetCellValue(row, column, value);
				}
			}
			#endregion
			#region IDisposable Members
			public void Dispose() {
				scheduler = null;
				column = null;
			}
			#endregion
		}
		#region IGridControlAct Members
		public void GridActEx(string actionName, int rowIndex, IGridColumn column, string[] parameterValues) {
			string commandParameter = parameterValues[0];
			switch(actionName) {
				case CheckVisibleResourcesCommandSignature:
					ExecuteCheckVisibleResourcesCommand(parameterValues);
					break;
				case CheckSelectedEventResourcesCommandSignature:
					ExecuteCheckSelectedEventResourcesCommand(parameterValues);
					break;
				case CheckAppointmentTypeCommandSignature:
					ExecuteCheckAppointmentTypeCommand(commandParameter);
					break;
				case ChangeViewToCommandSignature:
					control.ActiveViewType = (SchedulerViewType)Enum.Parse(typeof(SchedulerViewType), commandParameter);
					break;
				case SelectIntervalCommandSignature:
					ExecuteSelectIntervalCommand(commandParameter);
					break;
				case SetCurrentDateCommandSignature: {
						ControlFinder finder = new ControlFinder();
						object dateNavControl = finder.FindControl("Field", "DateNavigator");
						TestControlFactoryWin.Instance.CreateControl(dateNavControl).GetInterface<IControlAct>().Act(commandParameter);
						break;
					}
				default:
					throw new AdapterOperationException(string.Format("Unknown action: {0}", actionName));
			}
		}
		#endregion
	}
}
