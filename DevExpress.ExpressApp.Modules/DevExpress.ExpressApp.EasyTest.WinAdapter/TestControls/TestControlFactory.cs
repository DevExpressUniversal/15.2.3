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
using System.Reflection;
using System.Windows.Forms;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.DevExpressControls;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.Standard;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.Xaf;
using DevExpress.ExpressApp.EasyTest.WinAdapter.Utils;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls {
	public class TestControlFactoryWin : TestControlFactory<TestControlFactoryWin> {
		static TestControlFactoryWin() {
			SetInstance(new TestControlFactoryWin());
		}
		public TestControlFactoryWin() {
			RegisterInterface<TestControlThreadSafeUIWrapper>();
			RegisterInterface<WinControlEnabled>();
			RegisterInterface<XtraMessageFormControl>();
			RegisterInterface<XafMessageFormControl>();
			RegisterInterface<WinTextEditableControl>();
			RegisterInterface<TextBoxBaseWinControl>();
			RegisterInterface<ButtonControl>();
			RegisterInterface<TabPageControl>();
			RegisterInterface<UnmanagedControlWrapperTestControl>();
			RegisterInterface<DXBaseEditControlText>();
			RegisterInterface<DXBaseEditControlEnabled>();
			RegisterInterface<TextEditControl>();
			RegisterInterface<ButtonEditTextControl>();
			RegisterInterface<DateTimeEditControl>();
			RegisterInterface<TimeEditControl>();
			RegisterInterface<ComboBoxEditControl>();
			RegisterInterface<CheckEditControl>();
			RegisterInterface<ButtonEditControl>();
			RegisterInterface<ButtonEditWithClearButtonControl>();
			RegisterInterface<AnalysisEditControl>();
			RegisterInterface<DateNavigatorControl>();
			RegisterInterface<MemoExEditTextControl>();
			RegisterInterface<RtfEditExTextControl>();
			RegisterInterface<ExpressionEditControl>();
			RegisterInterface<FilterEditorControlAdapter>();
			RegisterInterface<XtraTabPageControl>();
			RegisterInterface<XtraLayoutTabControl>();
			RegisterInterface<SimpleButtonControl>();
			RegisterInterface<ButtonsContainerSingleChoiceActionItemTestControl>();
			RegisterInterface<BarItemLinkControl>();
			RegisterInterface<BarEditItemLinkControl>();
			RegisterInterface<BarStaticItemLinkControl>();
			RegisterInterface<BarButtonItemLinkControl>();
			RegisterInterface<BarCheckItemLinkControl>();
			RegisterInterface<BarCustomContainerItemLinkControl>();
			RegisterInterface<DXNavBarControl>();
			RegisterInterface<NavBarItemLinkControl>();
			RegisterInterface<FormControl>();
			RegisterInterface<ListEditorTestControlAct>();
			RegisterInterface<GridTestControl>();
			RegisterInterface<TreeListTestControl>();
			RegisterInterface<SchedulerTestControl>();
			RegisterInterface<LookupObjectListViewEditControl>();
			RegisterInterface<GridLookupEditorTestControl>();
			RegisterInterface<HtmlEditorControl>();
			RegisterInterface<PopupCriteriaEditTestControl>();
			RegisterInterface<XafTreeListNavigationControl>();
			RegisterInterface<NavigationActionContainerTestControl>();
			RegisterInterface<PivotGridTestControl>();
			RegisterInterface<PictureEditControl>();
			RegisterInterface<PopupBaseEditControl>();
			RegisterInterface<CheckedListBoxViewEditControl>();
		}
	}
	public class TestControlFactory<T> : Singleton<T> where T : TestControlFactory<T>, new() {
		protected List<Type> registredInterfaces = new List<Type>();
		private IList<Type> GetBaseTypes(Type controlType) {
			List<Type> baseTypes = new List<Type>();
			Type type = controlType;
			while(type != null) {
				baseTypes.Add(type);
				type = type.BaseType;
			}
			return baseTypes;
		}
		private IEnumerable<KeyValuePair<Type, Type>> GetCompatibleInterfaceImplementers(Type controlType) {
			Dictionary<Type, Type> compatibleTypes = new Dictionary<Type, Type>();
			foreach(Type item in registredInterfaces) {
				foreach(ConstructorInfo info in item.GetConstructors()) {
					try {
						ParameterInfo[] getParameters = info.GetParameters();
						if(getParameters.Length == 1) {
							if(getParameters[0].ParameterType.IsAssignableFrom(controlType)) {
								compatibleTypes.Add(item, getParameters[0].ParameterType);
							}
						}
					}
					catch(System.IO.FileNotFoundException) { }
				}
			}
			return compatibleTypes;
		}
		protected virtual IEnumerable<KeyValuePair<Type, Type>> FindCompatibleInterfaces(Type controlType) {
			Dictionary<Type, Type> compatibleInterfaces = new Dictionary<Type, Type>();
			Dictionary<Type, Type> interfaceImplementerTypeToControlType = new Dictionary<Type, Type>();
			IList<Type> baseTypes = GetBaseTypes(controlType);
			IEnumerable<KeyValuePair<Type, Type>> compatibleTypes = GetCompatibleInterfaceImplementers(controlType);
			foreach(KeyValuePair<Type, Type> item in compatibleTypes) {
				Type implementerType = item.Key;
				Type[] interfaces = implementerType.GetInterfaces();
				foreach(Type currentInterface in interfaces) {
					Type compatibleControlType = item.Value;
					Type interfaceImplementer = null;
					if(compatibleInterfaces.TryGetValue(currentInterface, out interfaceImplementer)) {
						Type currentControlType = interfaceImplementerTypeToControlType[currentInterface];
						if(baseTypes.IndexOf(currentControlType) > baseTypes.IndexOf(compatibleControlType)) {
							compatibleInterfaces[currentInterface] = implementerType;
						}
					} else {
						compatibleInterfaces.Add(currentInterface, implementerType);
						interfaceImplementerTypeToControlType[currentInterface] = compatibleControlType;
					}
				}
			}
			return compatibleInterfaces;
		}
		public virtual ITestControl CreateControl(object control) {
			EasyTestTracer.Tracer.LogText("CreateControl: " + control);
			TestControlBase result = new TestControlBase(control);
			bool isFound = false;
			foreach(KeyValuePair<Type, Type> item in FindCompatibleInterfaces(control.GetType())) {
				object interfaceImplementer = item.Value.GetConstructors()[0].Invoke(new object[] { control });
				if(interfaceImplementer is ITestControlContainer) {
					((ITestControlContainer)interfaceImplementer).TestControl = result;
				}
				result.AddInterface(item.Key, interfaceImplementer);
				isFound = true;
			}
			TestControlWrapper wrappedTestControl = new TestControlWrapper(result);
			foreach(KeyValuePair<Type, Type> item in FindCompatibleInterfaces(typeof(TestControlWrapper))) {
				if(result.FindInterface(item.Key) != null) {
					object interfaceImplementer = item.Value.GetConstructors()[0].Invoke(new object[] { result });
					wrappedTestControl.AddInterface(item.Key, interfaceImplementer);
				}
			}
			if(isFound) {
				return wrappedTestControl;
			}
			return null;
		}
		public void RegisterInterface<InterfaceType>() {
			registredInterfaces.Add(typeof(InterfaceType));
		}
		public void UnRegisterInterface<InterfaceType>() {
			registredInterfaces.Remove(typeof(InterfaceType));
		}
	}
	public class TestControlWrapper : TestControlBase {
		private TestControlBase testControl;
		public override IEnumerable<KeyValuePair<Type, object>> GetAvailalbeInterfaces() {
			return testControl.GetAvailalbeInterfaces();
		}
		public TestControlWrapper(TestControlBase testControl)
			: base(testControl) {
			this.testControl = testControl;
			controlTypeName = testControl.controlTypeName;
		}
		public override InterfaceType FindInterface<InterfaceType>() {
			InterfaceType result = base.FindInterface<InterfaceType>();
			if(result != null) {
				return result;
			}
			else {
				return testControl.FindInterface<InterfaceType>();
			}
		}
		public override string Name {
			get { return testControl.Name; }
			set {
				testControl.Name = value;
			}
		}
	}
	public class TestControlBase : MarshalByRefObject, ITestControl {
		public const string InterfaceIsntSupported = "The {0} interface is not supported for the control: {1}\r\nAvailable interfaces: {2}";
		protected Dictionary<Type, object> interfaces;
		private string name;
		private string testControlType;
		internal string controlTypeName;
		private string FormatAvailalbeInterfaces(IEnumerable<KeyValuePair<Type, object>> interfaces) {
			List<string> availalbeInterfaces = new List<string>();
			foreach(KeyValuePair<Type, object> interfaceDescription in interfaces) {
				availalbeInterfaces.Add(interfaceDescription.Key.FullName + "(" + interfaceDescription.Value.GetType().FullName + ")");
			}
			return string.Join(",\r\n", availalbeInterfaces.ToArray());
		}
		public virtual IEnumerable<KeyValuePair<Type, object>> GetAvailalbeInterfaces() {
			return interfaces;
		}
		public void AddInterface(Type InterfaceType, object interfaceImpl) {
			interfaces[InterfaceType] = interfaceImpl;
		}
		public TestControlBase(object control) {
			this.controlTypeName = control.GetType().FullName;
			this.interfaces = new Dictionary<Type, object>();
		}
		public object FindInterface(Type interfaceType) {
			object result = null;
			interfaces.TryGetValue(interfaceType, out result);
			return result;
		}
		#region ITestControl Members
		public virtual InterfaceType FindInterface<InterfaceType>() {
			object result = default(InterfaceType);
			interfaces.TryGetValue(typeof(InterfaceType), out result);
			return ((InterfaceType)result);
		}
		public InterfaceType GetInterface<InterfaceType>() {
			InterfaceType result = FindInterface<InterfaceType>();
			if(result == null) {
				string availalbeInterfaces = FormatAvailalbeInterfaces(GetAvailalbeInterfaces());
				throw new WarningException(String.Format(InterfaceIsntSupported,
					typeof(InterfaceType).FullName, controlTypeName, availalbeInterfaces));
			}
			else {
				return result;
			}
		}
		public virtual string Name {
			get { return name; }
			set {
				name = value;
			}
		}
		public virtual string ControlType {
			get { return testControlType; }
			set {
				testControlType = value;
			}
		}
		#endregion
	}
	public class TestControlInterfaceImplementerBase<T> : ITestControlContainer {
		private WeakReference control_;
		protected T control {
			get {
				return (T)control_.Target;
			}
		}
		public TestControlInterfaceImplementerBase(T control) {
			control_ = new WeakReference(control);
		}
		private ITestControl testControl;
		public ITestControl TestControl {
			get { return testControl; }
			set {
				testControl = value;
			}
		}
	}
	public abstract class TestControlTextValidated<T> : TestControlInterfaceImplementerBase<T>, IControlText where T : Control {
		public TestControlTextValidated(T control) : base(control) { }
		private void EndCurrentEdit(Control control) {
			BeforeEndCurrentEdit();
			while(control != null) {
				if(control.DataBindings.Count > 0) {
					control.DataBindings[0].WriteValue();
				}
				control = control.Parent;
			}
		}
		protected string ValidateMaxLength(int maxLength, string text) {
			if(maxLength != 0 && text.Length > maxLength) {
				return String.Format("Requested value '{0}' was higher than control's MaxLength: {1}.", text, maxLength);
			}
			else {
				return "";
			}
		}
		protected virtual void BeforeEndCurrentEdit() {
		}
		protected virtual string Validate(string text) {
			if(!TestControl.GetInterface<IControlEnabled>().Enabled) {
				return "The control is disabled";
			}
			return String.Empty;
		}
		protected abstract void InternalSetText(string text);
		protected void SetText(string text) {
			string error = Validate(text);
			if(error == String.Empty) {
				if(Text != text) {
					control.Focus();
					InternalSetText(text);
					EndCurrentEdit(control);
				}
			}
			else {
				throw new AdapterOperationException(
					String.Format("Cannot change the '{0}' control's field value. {1}", TestControl.Name, error));
			}
		}
		protected virtual string GetText() {
			return control.Text;
		}
		#region IControlText Members
		public string Text {
			get {
				return GetText();
			}
			set {
				SetText(value);
			}
		}
		#endregion
	}
	public interface IControlDragDrop {
		void DragDrop(string source, string sourceId, string dropTo, string dropToValue);
	}
	public interface IGridDoubleClick {
		void DoubleClickToCell(int row, IGridColumn column);
	}
	public interface ITestWindowEx : ITestWindow {
		void SetWindowPosition(int left, int top);
		void CheckWindowPosition(int left, int top);
		void CheckWindowSize(int width, int height);
	}
	public class TestInterfaceWrapper<InterfaceType> : MarshalByRefObject {
		private ITestControl control;
		public InterfaceType TestInterface {
			get {
				InterfaceType result = default(InterfaceType);
				SynchronousMethodExecutor.Instance.Execute(string.Format("TestInterfaceWrapper<{0}> '{1}'", typeof(InterfaceType).FullName, control.Name),
					delegate() {
						result = control.GetInterface<InterfaceType>();
					});
				return result;
			}
		}
		public TestInterfaceWrapper(ITestControl control) {
			this.control = control;
		}
	}
	public class TestControlThreadSafeUIWrapper : MarshalByRefObject, IControlEnabled, IControlText, IControlAct, IControlActionItems, IControlHint,
		IGridBase, IGridRowsSelection, IGridCellControlCreation, IGridAct, IGridDoubleClick, IGridControlAct,
		IControlDragDrop, ITestWindow, ITestWindowEx {
		private ITestControl control;
		public TestControlThreadSafeUIWrapper(ITestControl control) {
			this.control = control;
		}
		#region IBaseTestControl Members
		public bool Enabled {
			get {
				bool result = false;
				SynchronousMethodExecutor.Instance.Execute(string.Format("Enabled '{0}'", control.Name),
					delegate() {
						result = control.GetInterface<IControlEnabled>().Enabled;
					});
				return result;
			}
		}
		#endregion
		#region IControlText Members
		public string Text {
			get {
				string result = "";
				SynchronousMethodExecutor.Instance.Execute("Text",
					delegate() {
						result = control.GetInterface<IControlReadOnlyText>().Text;
					});
				return result;
			}
			set {
				SynchronousMethodExecutor.Instance.Execute("Text = " + value,
					delegate() {
						control.GetInterface<IControlText>().Text = value;
					});
			}
		}
		#endregion
		#region IControlAct Members
		public void Act(string value) {
			SynchronousMethodExecutor.Instance.Execute(string.Format("Act '{0}'", control.Name),
				delegate() {
					control.GetInterface<IControlAct>().Act(value);
				});
		}
		#endregion
		#region IActionItems Members
		public bool IsVisible(string value) {
			bool result = false;
			SynchronousMethodExecutor.Instance.Execute(string.Format("IsVisible '{0}'", control.Name),
				delegate() {
					result = control.GetInterface<IControlActionItems>().IsVisible(value);
				});
			return result;
		}
		public bool IsEnabled(string value) {
			bool result = false;
			SynchronousMethodExecutor.Instance.Execute(string.Format("IsEnabled '{0}'", control.Name),
				delegate() {
					result = control.GetInterface<IControlActionItems>().IsEnabled(value);
				});
			return result;
		}
		#endregion
		#region IControlHint Members
		public string Hint {
			get { return control.GetInterface<IControlHint>().Hint; }
		}
		#endregion
		#region ITestControlDragDrop Members
		public void DragDrop(string source, string sourceId, string dropTo, string dropToValue) {
			SynchronousMethodExecutor.Instance.Execute(string.Format("DragDrop '{0}'", control.Name),
				delegate() {
					control.FindInterface<IControlDragDrop>().DragDrop(source, sourceId, dropTo, dropToValue);
				});
		}
		#endregion
		#region IGridBase Members
		public IEnumerable<IGridColumn> Columns {
			get {
				IEnumerable<IGridColumn> result = null;
				SynchronousMethodExecutor.Instance.ExecuteSimple(string.Format("Columns '{0}'", control.Name),
					delegate() {
						result = control.FindInterface<IGridBase>().Columns;
					});
				return new MarshalByRefGridColumnEnumerator(result);
			}
		}
		public string GetCellValue(int row, IGridColumn column) {
			string result = null;
			SynchronousMethodExecutor.Instance.ExecuteSimple(string.Format("GetCellValue '{0}'", control.Name),
				delegate() {
					result = control.FindInterface<IGridBase>().GetCellValue(row, column);
				});
			return result;
		}
		public int GetRowCount() {
			int result = -1;
			SynchronousMethodExecutor.Instance.ExecuteSimple(string.Format("GetRowCount '{0}'", control.Name),
				delegate() {
					result = control.FindInterface<IGridBase>().GetRowCount();
				});
			return result;
		}
		#endregion
		#region IGridRowsSelection Members
		public void ClearSelection() {
			SynchronousMethodExecutor.Instance.Execute(string.Format("ClearSelection '{0}'", control.Name),
				delegate() {
					control.FindInterface<IGridRowsSelection>().ClearSelection();
				});
		}
		public void SelectRow(int rowIndex) {
			SynchronousMethodExecutor.Instance.Execute(string.Format("SelectRow '{0}'", control.Name),
				delegate() {
					control.FindInterface<IGridRowsSelection>().SelectRow(rowIndex);
				});
		}
		public void UnselectRow(int rowIndex) {
			SynchronousMethodExecutor.Instance.Execute(string.Format("UnselectRow '{0}'", control.Name),
				delegate() {
					control.FindInterface<IGridRowsSelection>().UnselectRow(rowIndex);
				});
		}
		public bool IsRowSelected(int rowIndex) {
			bool result = false;
			SynchronousMethodExecutor.Instance.Execute(string.Format("IsRowSelected '{0}'", control.Name),
				delegate() {
					result = control.FindInterface<IGridRowsSelection>().IsRowSelected(rowIndex);
				});
			return result;
		}
		#endregion
		#region IGridAct Members
		public void GridAct(string actionName, int rowIndex, IGridColumn column) {
			SynchronousMethodExecutor.Instance.Execute(string.Format("GridAct '{0}'", control.Name),
				delegate() {
					control.FindInterface<IGridAct>().GridAct(actionName, rowIndex, column);
				});
		}
		public void CheckGridAct(string actionName, int rowIndex, IGridColumn column, bool isInlineOnly) {
			SynchronousMethodExecutor.Instance.Execute(string.Format("CheckGridAct '{0}'", control.Name),
				delegate() {
					control.FindInterface<IGridAct>().CheckGridAct(actionName, rowIndex, column, isInlineOnly);
				});
		}
		#endregion
		#region IGridDoubleClick Members
		public void DoubleClickToCell(int row, IGridColumn column) {
			SynchronousMethodExecutor.Instance.Execute(string.Format("DoubleClickToCell '{0}'", control.Name),
				delegate() {
					control.FindInterface<IGridDoubleClick>().DoubleClickToCell(row, column);
				});
		}
		#endregion
		#region IGridCellControlCreation Members
		public ITestControl CreateCellControl(int row, IGridColumn column) {
			ITestControl result = null;
			SynchronousMethodExecutor.Instance.Execute(string.Format("GetCellValue '{0}'", control.Name),
				delegate() {
					result = control.FindInterface<IGridCellControlCreation>().CreateCellControl(row, column);
				});
			return result;
		}
		public void BeginEdit(int row) {
			SynchronousMethodExecutor.Instance.Execute(string.Format("BeginEdit '{0}'", control.Name),
				delegate() {
					control.FindInterface<IGridCellControlCreation>().BeginEdit(row);
				});
		}
		public void EndEdit() {
			SynchronousMethodExecutor.Instance.Execute(string.Format("EndEdit '{0}'", control.Name),
				delegate() {
					control.FindInterface<IGridCellControlCreation>().EndEdit();
				});
		}
		#endregion
		#region IGridControlAct Members
		public void GridActEx(string actionName, int rowIndex, IGridColumn column, string[] param) {
			SynchronousMethodExecutor.Instance.Execute(string.Format("GridActEx '{0}'", control.Name),
				delegate() {
					control.FindInterface<IGridControlAct>().GridActEx(actionName, rowIndex, column, param);
				});
		}
		#endregion
		#region ITestWindow Members
		public IntPtr GetActiveWindowHandle() {
			return control.FindInterface<ITestWindow>().GetActiveWindowHandle();
		}
		public void SetWindowSize(int width, int height) {
			control.FindInterface<ITestWindow>().SetWindowSize(width, height);
		}
		public string Caption {
			get { return control.FindInterface<ITestWindow>().Caption; }
		}
		public void Close() {
			control.FindInterface<ITestWindow>().Close();
		}
		#endregion
		#region ITestWindowEx Members
		public void SetWindowPosition(int left, int top) {
			SynchronousMethodExecutor.Instance.Execute(string.Format("SetWindowPosition '{0}'", control.Name),
				delegate() {
					control.GetInterface<ITestWindowEx>().SetWindowPosition(left, top);
				});
		}
		public void CheckWindowPosition(int left, int top) {
			SynchronousMethodExecutor.Instance.Execute(string.Format("CheckWindowPosition '{0}'", control.Name),
				delegate() {
					control.GetInterface<ITestWindowEx>().CheckWindowPosition(left, top);
				});
		}
		public void CheckWindowSize(int width, int height) {
			SynchronousMethodExecutor.Instance.Execute(string.Format("CheckWindowSize '{0}'", control.Name),
				delegate() {
					control.GetInterface<ITestWindowEx>().CheckWindowSize(width, height);
				});
		}
		#endregion
	}
}
