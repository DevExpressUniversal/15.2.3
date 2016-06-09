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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.XtraTreeList;
using R = System.Reflection;
namespace DevExpress.ExpressApp.Win.Core {
	public interface IGridInplaceEdit {
		Object GridEditingObject { get; set; }
		ControlBindingsCollection DataBindings { get; }
	}
	internal class MemberObjectWrapper {
		public IMemberInfo MemberInfo;
		public Object MemberObject;
	}
	internal class XafBinding : Binding {
		public XafBinding(String propertyName, Object dataSource, String dataMember, Boolean formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode)
			: base(propertyName, dataSource, dataMember, formattingEnabled, dataSourceUpdateMode) {
		}
		public Object OriginalDataSource;
	}
	public class BindingHelper : IDisposable {
		private List<MemberObjectWrapper> memberObjectWrappers;
		private Boolean immediatePostData;
		private R.PropertyInfo controlPropertyInfo;
		private IMemberInfo memberInfo;
		private Control control;
		private Object obj;
		private XafBinding binding;
		private void Object_PropertyChanged(Object sender, PropertyChangedEventArgs args) {
			if(obj != null) {
				Object memberObject = obj;
				Boolean needUpdate = false;
				for(Int32 i = 0; i < memberObjectWrappers.Count; i++) {
					memberObject = memberObjectWrappers[i].MemberInfo.GetValue(memberObject);
					if((memberObjectWrappers[i].MemberObject == sender) && (memberObjectWrappers[i].MemberInfo.Name == args.PropertyName)) {
						if(memberObject != memberObjectWrappers[i].MemberObject) {
							needUpdate = true;
							break;
						}
					}
				}
				if(needUpdate) {
					RefreshBinding(obj);
				}
			}
		}
		private void SubscribeToMemberObjectPropertyChangedEvents() {
			Object currentObject = obj;
			IList<IMemberInfo> memberInfoPath = memberInfo.GetPath();
			for(Int32 i = 0; i <= memberInfoPath.Count - 2; i++) {
				if(currentObject == null) {
					break;
				}
				else if(currentObject is INotifyPropertyChanged) {
					((INotifyPropertyChanged)currentObject).PropertyChanged += new PropertyChangedEventHandler(Object_PropertyChanged);
					MemberObjectWrapper memberObjectWrapper = new MemberObjectWrapper();
					memberObjectWrapper.MemberObject = currentObject;
					memberObjectWrapper.MemberInfo = memberInfoPath[i];
					memberObjectWrappers.Add(memberObjectWrapper);
				}
				currentObject = memberInfoPath[i].GetValue(currentObject);
			}
		}
		private void UnsubscribeFromMemberObjectPropertyChangedEvents() {
			foreach(MemberObjectWrapper memberObjectWrapper in memberObjectWrappers) {
				((INotifyPropertyChanged)memberObjectWrapper.MemberObject).PropertyChanged -= new PropertyChangedEventHandler(Object_PropertyChanged);
			}
			memberObjectWrappers.Clear();
		}
		private void TurnBindingToRefreshValue() {
			foreach(Binding b in control.DataBindings) {
				if(b.IsBinding) {
					b.ReadValue();
				}
			}
		}
		private void ClearControlProperty() {
			if(controlPropertyInfo != null) {
				controlPropertyInfo.SetValue(control, null, null);
			}
		}
		private void RemoveBinding() {
			if(binding != null) {
				control.DataBindings.Remove(binding);
				binding = null;
			}
		}
		public BindingHelper(Control control, String controlPropertyName, IMemberInfo memberInfo) : this(control, controlPropertyName, memberInfo, false) { }
		public BindingHelper(Control control, String controlPropertyName, IMemberInfo memberInfo, Boolean immediatePostData) {
			this.control = control;
			if(!String.IsNullOrEmpty(controlPropertyName)) {
				controlPropertyInfo = control.GetType().GetProperty(controlPropertyName);
			}
			this.memberInfo = memberInfo;
			this.immediatePostData = immediatePostData;
			memberObjectWrappers = new List<MemberObjectWrapper>();
		}
		public void Dispose() {
			if(control != null) {
				control.DataBindings.Clear();
				control = null;
			}
			if(memberObjectWrappers != null) {
				UnsubscribeFromMemberObjectPropertyChangedEvents();
				memberObjectWrappers = null;
			}
			controlPropertyInfo = null;
			binding = null;
			obj = null;
			memberInfo = null;
			GC.SuppressFinalize(this);
		}
		public Object GetControlValue() {
			if(controlPropertyInfo != null) {
				return controlPropertyInfo.GetValue(control, null);
			}
			return null;
		}
		public void ForceWriteValue() {
			binding.WriteValue();
		}
		public void RefreshBinding(Object obj) {
			if(!control.IsDisposed && (controlPropertyInfo != null)) {
				this.obj = obj;
				Object objForBinding = memberInfo.GetOwnerInstance(obj);
				if(objForBinding != null) {
					IMemberInfo memberInfoForBinding = memberInfo;
					if(memberInfo.GetPath().Count > 1) {
						memberInfoForBinding = memberInfo.LastMember;
					}
					if((binding == null) || (binding.DataSource != objForBinding) || (binding.BindingManagerBase == null)) {
						RemoveBinding();
						DataSourceUpdateMode mode = immediatePostData ? DataSourceUpdateMode.OnPropertyChanged : DataSourceUpdateMode.OnValidation;
						binding = new XafBinding(controlPropertyInfo.Name, objForBinding, memberInfoForBinding.BindingName, true, mode);
						binding.DataSourceNullValue = null;
						binding.OriginalDataSource = obj;
						control.DataBindings.Add(binding);
						UnsubscribeFromMemberObjectPropertyChangedEvents();
						SubscribeToMemberObjectPropertyChangedEvents();
					}
					else {
						TurnBindingToRefreshValue();
					}
				}
				else {
					RemoveBinding();
					ClearControlProperty();
					UnsubscribeFromMemberObjectPropertyChangedEvents();
					SubscribeToMemberObjectPropertyChangedEvents();
				}
			}
		}
		public Object DataSource {
			get { return obj; }
		}
		public Boolean ImmediatePostData {
			get { return immediatePostData; }
			set { immediatePostData = value; }
		}
		private static Control GetActiveControl(Control parentControl) {
			ContainerControl container = parentControl as ContainerControl;
			if(container != null) {
				Control child = container.ActiveControl;
				if(ReferenceEquals(child, container))
					return child;
				else
					return GetActiveControl(child);
			}
			else {
				return parentControl;
			}
		}
		private static void OnEndEdit() {
			if(EndEdit != null) {
				EndEdit(null, EventArgs.Empty);
			}
		}
		private static void EndCurrentEditCore(Control control) {
			Control currentControl = GetActiveControl(control);
			Boolean needInvokeNotifyValidating = true;
			R.MethodInfo methodInfo = typeof(Control).GetMethod("NotifyValidating", R.BindingFlags.NonPublic | R.BindingFlags.Instance);
			while(currentControl != null) {
				Control currentControlParent = currentControl.Parent;
				TreeList tree = currentControl as TreeList;
				if(tree != null) {
					tree.CloseEditor();
				}
				XtraEditors.BaseEdit edit = currentControl as XtraEditors.BaseEdit;
				if(edit != null) {
					edit.DoValidate();
				}
				XtraVerticalGrid.VGridControl verticalGrid = currentControl as XtraVerticalGrid.VGridControl;
				if(verticalGrid != null && verticalGrid.ActiveEditor != null) {
					verticalGrid.PostEditor();
					verticalGrid.CloseEditor();
				}
				else if(verticalGrid != null) {
					needInvokeNotifyValidating = false;
				}
				DevExpress.XtraGrid.GridControl grid = currentControl as DevExpress.XtraGrid.GridControl;
				if((grid != null) && (grid.MainView != null) && (grid.MainView.ActiveEditor != null)) {
					grid.MainView.PostEditor();
					grid.MainView.CloseEditor();
				}
				else if(grid != null) {
					needInvokeNotifyValidating = false;
				}
				if((grid != null) && (grid.MainView != null)) {
					grid.MainView.UpdateCurrentRow();
				}
				if(needInvokeNotifyValidating) {
					try {
						methodInfo.Invoke(currentControl, null);
					}
					catch(Exception e) {
						throw e.InnerException;
					}
				}
				currentControl = currentControlParent;
			}
		}
		public static Object FindEditingObject(IGridInplaceEdit edit) {
			if(edit.DataBindings.Count == 1) {
				if(edit.DataBindings[0] is XafBinding) {
					return ((XafBinding)edit.DataBindings[0]).OriginalDataSource;
				}
				else {
					return edit.DataBindings[0].DataSource;
				}
			}
			else {
				return edit.GridEditingObject;
			}
		}
		public static Object GetEditingObject(IGridInplaceEdit edit) {
			Object result = FindEditingObject(edit);
			if(result == null) {
				throw new InvalidOperationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotObtainTheEditingObject));
			}
			return result;
		}
		public static void EndCurrentEdit(Control control) {
			if(control != null) {
				Form form = control.FindForm();
				if(form != null) {
					foreach(Form mdiChildForm in form.MdiChildren) {
						EndCurrentEditCore(mdiChildForm);
					}
				}
			}
			EndCurrentEditCore(control);
			OnEndEdit();
		}
		public static event EventHandler EndEdit;
	}
}
