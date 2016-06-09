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
using System.Windows.Forms;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WinAdapter.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraNavBar;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.DevExpressControls {
	public class FormControl : TestControlInterfaceImplementerBase<Form>, ITestWindowEx, IControlAct, IControlReadOnlyText   {
		public FormControl(Form control)
			: base(control) {
		}
		#region IControlAct Members
		public void Act(string value) {
			control.Show();
		}
		#endregion
		#region ITestWindow Members
		public IntPtr GetActiveWindowHandle() {
			ProcessFormsEnumerator p = new ProcessFormsEnumerator();
			Form[] pGetActiveForms = p.GetActiveForms(true);
			if(pGetActiveForms.Length > 0) {
				return pGetActiveForms[0].Handle;
			}
			return IntPtr.Zero;
		}
		private Form GetNotMdiActiveForm() {
			ProcessFormsEnumerator p = new ProcessFormsEnumerator();
			Form[] activeForms = p.GetActiveForms(true);
			foreach(Form activeForm in activeForms) {
				if(!activeForm.IsMdiChild) {
					return activeForm;
				}
			}
			return null;
		}
		public void SetWindowSize(int width, int height) {
			Form activeForm = GetNotMdiActiveForm();
			if(activeForm != null) {
				SynchronousMethodExecutor.Instance.Execute("SetWindowSize", delegate() {
					if(activeForm.WindowState != FormWindowState.Normal) {
						activeForm.WindowState = FormWindowState.Normal;
					}
					activeForm.Size = new Size(width, height);
				});
			}
		}
		public string Caption {
			get { return control.Text; }
		}
		public void Close() {
			control.Close();
		}
		#endregion
		#region IControlReadOnlyText Members
		public string Text {
			get { return string.Empty; }
		}
		#endregion
		#region ITestWindowEx Members
		public void SetWindowPosition(int left, int top) {
			Form activeForm = GetNotMdiActiveForm();
			if(activeForm != null) {
				SynchronousMethodExecutor.Instance.Execute("SetWindowPosition", delegate() {
					if(activeForm.WindowState != FormWindowState.Normal) {
						activeForm.WindowState = FormWindowState.Normal;
					}
					activeForm.Location = new Point(left, top);
				});
			}
		}
		public void CheckWindowPosition(int left, int top) {
			Form activeForm = GetNotMdiActiveForm();
			if(activeForm == null) {
				throw new AdapterOperationException("Active window not found.");
			}
			if(activeForm.Location.X != left || activeForm.Location.Y != top) {
				throw new AdapterOperationException(string.Format("CheckActiveWindowPosition: incorrect active window position (expected values: X = {0}, Y = {1}, actual values: X = {2}, Y = {3})", left, top, activeForm.Location.X, activeForm.Location.Y));
			}
		}
		public void CheckWindowSize(int width, int height) {
			Form activeForm = GetNotMdiActiveForm();
			if(activeForm == null) {
				throw new AdapterOperationException("Active window not found.");
			}
			if(activeForm.Size.Width != width || activeForm.Size.Height != height) {
				throw new AdapterOperationException(string.Format("CheckActiveWindowSize: incorrect active window size (expected size: {0}x{1}, actual size: {2}x{3})", width, height, activeForm.Size.Width, activeForm.Size.Height));
			}
		}
		#endregion
	}
	internal class SimpleButtonControl : TestControlInterfaceImplementerBase<SimpleButton>, IControlAct, IControlActionItems {
		public SimpleButtonControl(SimpleButton control)
			: base(control) {
		}
		#region IControlAct Members
		public void Act(string value) {
			if(value != null) {
				BarSelectionInfo info;
				PropertyInfo pInfo = (PropertyInfo)typeof(BarManager).GetProperty("SelectionInfo", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				IList<BarManager> managers = BarManagerFindStrategy.Instance.FindBarManagers(control.FindForm());
				string reason = "BarMangers";
				foreach(BarManager manager in managers) {
					info = (BarSelectionInfo)pInfo.GetValue(manager, null);
					PopupMenuBarControl lastPopup = (PopupMenuBarControl)info.OpenedPopups.LastPopup;
					if(lastPopup != null) {
						PopupMenu popupMenu = lastPopup.Menu;
						if(popupMenu != null) {
							object obj = BarItemFindStrategy.Instance.FindControl(popupMenu, value);
							if(obj != null) {
								ITestControl testControl = TestControlFactoryWin.Instance.CreateControl(obj);
								testControl.GetInterface<IControlAct>().Act(null);
								reason = null;
								break;
							}
							else {
								throw new AdapterOperationException(String.Format("A context menu doesn't contain the {0} Action item", value));
							}
						}
						else {
							reason = "PopupMenu";
						}
					}
					else {
						reason = "LastPopup";
					}
				}
				if(reason != null) {
					throw new WarningException(
						string.Format("Cannot execute an Action with the '{0}' parameter. Cannot access '{1}'", value, reason));
				}
			}
			else {
				control.PerformClick();
			}
		}
		#endregion
		#region IActionItems Members
		private BarItemLink FindPopupActionItem(string value) {
			if(value != null) {
				BarSelectionInfo info;
				PropertyInfo pInfo = (PropertyInfo)typeof(BarManager).GetProperty("SelectionInfo", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				IList<BarManager> managers = BarManagerFindStrategy.Instance.FindBarManagers(control.FindForm());
				foreach(BarManager manager in managers) {
					info = (BarSelectionInfo)pInfo.GetValue(manager, null);
					PopupMenuBarControl lastPopup = (PopupMenuBarControl)info.OpenedPopups.LastPopup;
					if(lastPopup != null) {
						PopupMenu popupMenu = lastPopup.Menu;
						if(popupMenu != null) {
							BarItemLink obj = BarItemFindStrategy.Instance.FindControl(popupMenu, value);
							return obj;
						}
					}
				}
			}
			return null;
		}
		public bool IsVisible(string value) {
			BarItemLink barItemLink = FindPopupActionItem(value);
			return barItemLink != null && barItemLink.Visible;
		}
		public bool IsEnabled(string value) {
			BarItemLink barItemLink = FindPopupActionItem(value);
			return barItemLink != null && barItemLink.Enabled;
		}
		#endregion
	}
	public class ButtonsContainerSingleChoiceActionItemTestControl : TestControlInterfaceImplementerBase<DropDownButton>, IControlAct, IControlEnabled, IControlActionItems {
		public ButtonsContainerSingleChoiceActionItemTestControl(DropDownButton control)
			: base(control) {
		}
		#region IControlAct Members
		public void Act(string value) {
			if(string.IsNullOrEmpty(value)) {
				control.PerformClick();
			}
			else {
				control.ShowDropDown();
				if(control.DropDownControl != null) {
					object obj = BarItemFindStrategy.Instance.FindControl(((PopupMenu)control.DropDownControl), value);
					if(obj != null) {
						ITestControl testControl = TestControlFactoryWin.Instance.CreateControl(obj);
						testControl.GetInterface<IControlAct>().Act(null);
					}
					else {
						throw new AdapterOperationException(String.Format("A context menu doesn't contain the {0} Action item", value));
					}
				}
			}
		}
		#endregion
		#region IControlEnabled Members
		public bool Enabled {
			get { return control.Enabled && control.Visible; }
		}
		#endregion
		#region IActionItems Members
		private BarItemLink FindDropDownItem(string value) {
			if(!string.IsNullOrEmpty(value)) {
				control.ShowDropDown();
				if(control.DropDownControl != null) {
					BarItemLink obj = BarItemFindStrategy.Instance.FindControl(((PopupMenu)control.DropDownControl), value);
					control.HideDropDown();
					return obj;
				}
			}
			return null;
		}
		public bool IsVisible(string value) {
			BarItemLink barItemLink = FindDropDownItem(value);
			return barItemLink != null && barItemLink.Visible;
		}
		public bool IsEnabled(string value) {
			BarItemLink barItemLink = FindDropDownItem(value);
			return barItemLink != null && barItemLink.Enabled;
		}
		#endregion
	}
	internal class BarItemLinkControl : TestControlInterfaceImplementerBase<BarItemLink>, IControlEnabled, IControlReadOnlyText, IControlHint {
		public BarItemLinkControl(BarItemLink control)
			: base(control) {
		}
		public bool Enabled {
			get { return control.Item.Visibility != BarItemVisibility.OnlyInCustomizing && control.Enabled; }
		}
		public string Hint {
			get {
				return control.Item.Hint;
			}
		}
		#region IControlReadOnlyText Members
		public string Text {
			get {
				return control.Caption;
			}
		}
		#endregion
	}
	public class BarEditItemLinkControl : TestControlInterfaceImplementerBase<BarEditItemLink>, IControlText, IControlAct, IControlActionItems {
		class VisibleEditorScope : IDisposable {
			private BarEditItemLinkControl control;
			public VisibleEditorScope(BarEditItemLinkControl control) {
				this.control = control;
				this.control.ShowEditor();
			}
			void IDisposable.Dispose() {
				BarEditItemLink barEditItemLink = control.control.Manager.ActiveEditItemLink as BarEditItemLink;
				if(barEditItemLink != null) {
					barEditItemLink.CloseEditor();
				}
				control = null;
			}
		}
		private string easyTestBarName = "EasyTest";
		public BarEditItemLinkControl(BarEditItemLink control) : base(control) { }
		private ITestControl GetTestControl() {
			ITestControl result;
			if(!TryGetTestControl(out result)) {
				throw new EasyTestException(String.Format("Cannot display editor for the '{0}' action", TestControl.Name));
			}
			return result;
		}
		private bool TryGetTestControl(out ITestControl testControl) {
			BaseEdit activeEditor = control.Manager.ActiveEditor;
			if(activeEditor == null) {
				testControl = null;
				return false;
			}
			else {
				testControl = TestControlFactoryWin.Instance.CreateControl(activeEditor);
				return true;
			}
		}
		private void ShowEditor() {
			if(!TryShowEditor()) {
				Bar easyTestBar = GetEasyTestBar(control.Manager);
				if(easyTestBar == null) {
					easyTestBar = new Bar(control.Manager, easyTestBarName);
					control.Manager.Bars.Add(easyTestBar);
				}
				easyTestBar.Visible = true;
				if(!easyTestBar.ItemLinks.Contains(control.Item.Links[0]))
					easyTestBar.ItemLinks.Add(control.Item);
				Application.DoEvents();
				TryShowEditor();
			}
		}
		private bool TryShowEditor() {
			foreach(Bar bar in control.Manager.Bars) {
				CustomLinksControl ctrl = bar.GetBarControl();
				if(ctrl == null)
					continue;
				ctrl.LayoutChanged();
				ctrl.UpdateViewInfo();
				control.Manager.Form.Update();
				foreach(BarItemLink link in bar.VisibleLinks) {
					if(link.Item == control.Item) {
						((BarEditItemLink)link).ShowEditor();
						return true;
					}
				}
			}
			return false;
		}
		private Bar GetEasyTestBar(BarManager manager) {
			foreach(Bar bar in control.Manager.Bars) {
				if(bar.BarName == easyTestBarName) {
					return bar;
				}
			}
			return null;
		}
		private string GetText() {
			using(new VisibleEditorScope(this)) {
				ITestControl dxEditor;
				if(TryGetTestControl(out dxEditor)) {
					return dxEditor.GetInterface<IControlReadOnlyText>().Text;
				}
				else {
					object editValue = control.Item.EditValue;
					return editValue == null ? "" : editValue.ToString();
				}
			}
		}
		private void SetText(string text, bool invokeAction) {
			using(new VisibleEditorScope(this)) {
				ITestControl dxEditor = GetTestControl();
				IControlAct controlAct = invokeAction ? dxEditor.FindInterface<IControlAct>() : null;
				if(text != null && text.StartsWith(":")) {
					if(controlAct != null) {
						controlAct.Act(text);
					}
				}
				else {
					IControlText controlText = text != null ? dxEditor.FindInterface<IControlText>() : null;
					if(controlText != null) {
						controlText.Text = text;
					}
					if(controlAct != null) {
						controlAct.Act(null);
					}
				}
			}
		}
		public void Act(string parameterValue) {
			SetText(parameterValue, true);
		}
		#region IActionItems Members
		public bool IsVisible(string value) {
			using(new VisibleEditorScope(this)) {
				IControlActionItems controlActionItems = GetTestControl().FindInterface<IControlActionItems>();
				return controlActionItems != null && controlActionItems.IsVisible(value);
			}
		}
		public bool IsEnabled(string value) {
			using(new VisibleEditorScope(this)) {
				IControlActionItems controlActionItems = GetTestControl().FindInterface<IControlActionItems>();
				return controlActionItems != null && controlActionItems.IsEnabled(value);
			}
		}
		#endregion
		#region IControlReadOnlyText Members
		string IControlReadOnlyText.Text {
			get { return GetText(); }
		}
		#endregion
		#region IControlText Members
		string IControlText.Text {
			get { return GetText(); }
			set { SetText(value, false); }
		}
		#endregion
	}
	public class BarStaticItemLinkControl : TestControlInterfaceImplementerBase<BarStaticItemLink>, IControlReadOnlyText {
		public BarStaticItemLinkControl(BarStaticItemLink control)
			: base(control) {
		}
		public string Text {
			get {
				return control.Caption;
			}
		}
	}
	public abstract class ContainerBarItemLinkControlBase<T> : TestControlInterfaceImplementerBase<T>, IControlAct, IControlActionItems
		where T : BarItemLink {
		public ContainerBarItemLinkControlBase(T control)
			: base(control) {
		}
		private void ExecuteClick(object item) {
			BarItem barItem = (BarItem)item;
			if(barItem.Enabled) {
				barItem.PerformClick();
			}
			else {
				throw new AdapterOperationException(String.Format("The '{0}' Action's '{1}' parameter is disabled", TestControl.Name, barItem.Caption));
			}
		}
		protected abstract BarItemLink FindExecutedLink(string parameterValue);
		public void Act(string parameterValue) {
			if(string.IsNullOrEmpty(parameterValue)) {
				ExecuteClick(control.Item);
			}
			else {
				BarItemLink itemLink = FindExecutedLink(parameterValue);
				if(itemLink == null) {
					throw new AdapterOperationException(String.Format("Cannot find the '{0}' action in the '{1}' control", TestControl.Name, parameterValue));
				}
				ExecuteClick(itemLink.Item);
			}
		}
		#region IActionItems Members
		public bool IsVisible(string value) {
			BarItemLink foundBarItemLink = FindExecutedLink(value);
			return foundBarItemLink == null ? false : foundBarItemLink.Visible;
		}
		public bool IsEnabled(string value) {
			BarItemLink foundBarItemLink = FindExecutedLink(value);
			return foundBarItemLink == null ? false : foundBarItemLink.Enabled;
		}
		#endregion
	}
	public class BarButtonItemLinkControl : ContainerBarItemLinkControlBase<BarButtonItemLink> {
		public BarButtonItemLinkControl(BarButtonItemLink control)
			: base(control) {
		}
		protected override BarItemLink FindExecutedLink(string parameterValue) {
			PopupMenu popup = control.Item.DropDownControl as PopupMenu;
			if(popup != null) {
				return BarItemFindStrategy.Instance.FindControl(popup, parameterValue);
			}
			else {
				throw new AdapterOperationException("Cannot find the context menu");
			}
		}
	}
	public class BarCheckItemLinkControl : ContainerBarItemLinkControlBase<BarCheckItemLink> {
		public BarCheckItemLinkControl(BarCheckItemLink control)
			: base(control) {
		}
		protected override BarItemLink FindExecutedLink(string parameterValue) {
			return null;
		}
	}
	public class BarCustomContainerItemLinkControl : ContainerBarItemLinkControlBase<BarCustomContainerItemLink> {
		public BarCustomContainerItemLinkControl(BarCustomContainerItemLink control)
			: base(control) {
		}
		protected override BarItemLink FindExecutedLink(string parameterValue) {
			return BarItemFindStrategy.Instance.FindControl(control.VisibleLinks, parameterValue);
		}
	}
	public class DXNavBarControl : TestControlInterfaceImplementerBase<NavBarControl>, IControlAct, IControlText, IControlActionItems, IGridBase {
		public DXNavBarControl(NavBarControl control)
			: base(control) {
		}
		private void FindItemLink(string parameterValue, List<string> foundValues) {
			foreach(NavBarGroup navBarGroup in control.Groups) {
				if(navBarGroup.GroupStyle == NavBarGroupStyle.ControlContainer) {
					ITestControl actionTreeList = TestControlFactoryWin.Instance.CreateControl(navBarGroup.ControlContainer.Controls[0]);
					IGridBase grid = actionTreeList.GetInterface<IGridBase>();
					List<IGridColumn> columns = new List<IGridColumn>(grid.Columns);
					int rowCount = grid.GetRowCount();
					for(int i = 0; i < rowCount; i++) {
						string cellValue = navBarGroup.Caption + "." + grid.GetCellValue(i, columns[0]);
						if(cellValue == parameterValue || cellValue.EndsWith("." + parameterValue)) {
							foundValues.Add(cellValue);
						}
					}
				}
				else {
					foreach(NavBarItemLink navBarItemLink in navBarGroup.ItemLinks) {
						string currentValue = navBarGroup.Caption + "." + navBarItemLink.Caption;
						if(currentValue == parameterValue || currentValue.EndsWith("." + parameterValue)) {
							foundValues.Add(navBarGroup.Caption + "." + navBarItemLink.Caption);
						}
					}
				}
			}
		}
		private NavBarItemLinkControl FindNavBarItemLink(string parameterValue) {
			foreach(NavBarGroup navBarGroup in control.Groups) {
				if(navBarGroup.GroupStyle != NavBarGroupStyle.ControlContainer) {
					foreach(NavBarItemLink navBarItemLink in navBarGroup.ItemLinks) {
						string currentValue = navBarGroup.Caption + "." + navBarItemLink.Caption;
						if(currentValue == parameterValue || currentValue.EndsWith("." + parameterValue)) {
							return new NavBarItemLinkControl(navBarItemLink);
						}
					}
				}
			}
			return null;
		}
		private void Execute(string parameterValue) {
			foreach(NavBarGroup navBarGroup in control.Groups) {
				if(navBarGroup.GroupStyle == NavBarGroupStyle.ControlContainer) {
					ITestControl actionTreeList = TestControlFactoryWin.Instance.CreateControl(navBarGroup.ControlContainer.Controls[0]);
					IGridBase grid = actionTreeList.GetInterface<IGridBase>();
					List<IGridColumn> columns = new List<IGridColumn>(grid.Columns);
					int rowCount = grid.GetRowCount();
					for(int i = 0; i < rowCount; i++) {
						string cellValue = grid.GetCellValue(i, columns[0]);
						if(navBarGroup.Caption + "." + cellValue == parameterValue) {
							control.ActiveGroup = navBarGroup;
							actionTreeList.GetInterface<IGridDoubleClick>().DoubleClickToCell(i, columns[0]);
						}
					}
				}
				else {
					NavBarItemLinkControl actionItem = null;
					foreach(NavBarItemLink navBarItemLink in navBarGroup.ItemLinks) {
						if(navBarGroup.Caption + "." + navBarItemLink.Caption == parameterValue) {
							actionItem = new NavBarItemLinkControl(navBarItemLink);
						}
						if(actionItem != null) {
							if(actionItem.Enabled) {
								control.Focus();
								System.Threading.Thread.Sleep(SystemInformation.DoubleClickTime);
								actionItem.Act(null);
								return;
							}
							else {
								throw new AdapterOperationException(String.Format("The '{0}' action's '{1}' parameter is disabled", TestControl.Name, parameterValue));
							}
						}
					}
				}
			}
		}
		public void Act(string parameterValue) {
			List<string> items = new List<string>();
			FindItemLink(parameterValue, items);
			if(items.Count == 0) {
				throw new AdapterOperationException(String.Format("Cannot find the '{0}' action's '{1}' parameter", TestControl.Name, parameterValue));
			}
			if(items.Count > 1) {
				throw new WarningException(String.Format("The action's '{0}' parameter '{1}' must be unique. Use one of following variants: {2}", TestControl.Name, parameterValue, string.Join(", ", items.ToArray())));
			}
			Execute(items[0]);
		}
		#region IControlText Members
		public string Text {
			get {
				NavBarGroup navBarActiveGroup = this.control.ActiveGroup;
				if(navBarActiveGroup.GroupStyle == NavBarGroupStyle.ControlContainer) {
					ITestControl actionTreeList = TestControlFactoryWin.Instance.CreateControl(navBarActiveGroup.ControlContainer.Controls[0]);
					return actionTreeList.GetInterface<IControlReadOnlyText>().Text;
				}
				else {
					return navBarActiveGroup.Caption + "." + navBarActiveGroup.SelectedLink.Caption;
				}
			}
			set {
				throw new NotImplementedException();
			}
		}
		#endregion
		#region IControlActionItems Members
		private List<string> FindItemLink(string value) {
			List<string> items = new List<string>();
			FindItemLink(value, items);
			if(items.Count > 1) {
				throw new WarningException(String.Format("The action's '{0}' parameter '{1}' must be unique. Use one of following variants: {2}", TestControl.Name, value, string.Join(", ", items.ToArray())));
			}
			return items;
		}
		public bool IsVisible(string value) {
			List<string> items = FindItemLink(value);
			NavBarItemLinkControl foundNavBarItemLink = FindNavBarItemLink(value);
			return foundNavBarItemLink == null ? items.Count > 0 : foundNavBarItemLink.Visible;
		}
		public bool IsEnabled(string value) {
			List<string> items = FindItemLink(value);
			NavBarItemLinkControl foundNavBarItemLink = FindNavBarItemLink(value);
			return foundNavBarItemLink == null ? items.Count > 0 : foundNavBarItemLink.Enabled;
		}
		#endregion
		#region IGridBase Members
		public IEnumerable<IGridColumn> Columns {
			get { return new IGridColumn[] { new TestGridColumn("Name") }; }
		}
		public string GetCellValue(int row, IGridColumn column) {
			string result = "";
			foreach(NavBarGroup navBarGroup in control.Groups) {
				if(navBarGroup.GroupStyle == NavBarGroupStyle.ControlContainer) {
					ITestControl actionTreeList = TestControlFactoryWin.Instance.CreateControl(navBarGroup.ControlContainer.Controls[0]);
					IGridBase grid = actionTreeList.GetInterface<IGridBase>();
					if(row < grid.GetRowCount()) {
						result = string.Format("{0}.{1}", navBarGroup.Caption, grid.GetCellValue(row, new List<IGridColumn>(grid.Columns)[0]));
						break;
					}
					else {
						row -= grid.GetRowCount();
					}
				}
				else {
					if(row < navBarGroup.ItemLinks.Count) {
						result = string.Format("{0}.{1}", navBarGroup.Caption, navBarGroup.ItemLinks[row].Caption);
						break;
					}
					else {
						row -= navBarGroup.ItemLinks.Count;
					}
				}
			}
			return result;
		}
		public int GetRowCount() {
			int result = 0;
			foreach(NavBarGroup navBarGroup in control.Groups) {
				if(navBarGroup.GroupStyle == NavBarGroupStyle.ControlContainer) {
					ITestControl actionTreeList = TestControlFactoryWin.Instance.CreateControl(navBarGroup.ControlContainer.Controls[0]);
					IGridBase grid = actionTreeList.GetInterface<IGridBase>();
					result += grid.GetRowCount();
				}
				else {
					result += navBarGroup.ItemLinks.Count;
				}
			}
			return result;
		}
		#endregion
	}
	public class TestGridColumn : DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.DevExpressControls.TestColumn<string>, IGridColumn {
		public TestGridColumn(string name) : base(name) { }
		#region IGridColumn Members
		public string Caption {
			get { return column; }
		}
		public bool Visible {
			get { return true; }
		}
		#endregion
	}
	internal class NavBarItemLinkControl : TestControlInterfaceImplementerBase<NavBarItemLink>, IControlEnabled, IControlAct {
		public NavBarItemLinkControl(NavBarItemLink control)
			: base(control) {
		}
		public bool Enabled {
			get { return control.Enabled && control.Visible; }
		}
		public bool Visible {
			get { return control.Visible; }
		}
		public void Act(string parameterValue) {
			if(control.NavBar.LinkSelectionMode != LinkSelectionModeType.None) {
				if(control.NavBar.SelectedLink != control) {
					control.NavBar.SelectedLink = control;
				}
				else {
					control.PerformClick();
				};
			}
			else {
				control.PerformClick();
			}
		}
	}
}
