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

namespace DevExpress.Utils.MVVM.Services {
	using System.ComponentModel;
	using System.Windows.Forms;
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ViewServiceBase : UI.IViewService {
		protected virtual Control CreateAndInitializeView(string viewType, object viewModel, object parameter, object parentViewModel) {
			return CreateAndInitializeView(viewType, viewModel, parameter, parentViewModel, null);
		}
		protected Control CreateAndInitializeView(string viewType, object viewModel, object parameter, object parentViewModel, UI.IViewLocator viewLocator) {
			Control viewControl = RaiseQueryView(viewType, parentViewModel, viewModel, parameter, viewLocator);
			if(viewControl != null)
				InitializeViewControl(viewControl, viewModel, parameter, parentViewModel);
			return viewControl;
		}
		public event QueryEventHandler<QueryViewEventArgs, Control> QueryView;
		Control RaiseQueryView(string viewType, object parentViewModel, object viewModel, object parameter, UI.IViewLocator viewLocator) {
			QueryEventHandler<QueryViewEventArgs, Control> handler = QueryView;
			QueryViewEventArgs args = new QueryViewEventArgs(viewType, viewModel, parameter);
			if(handler != null)
				handler(this, args);
			return args.Result ?? ResolveViewControl(viewType, parentViewModel, viewLocator);
		}
		void InitializeViewControl(Control viewControl, object viewModel, object parameter, object parentViewModel) {
			var context = MVVMContext.FromControl(viewControl);
			if(context != null) {
				MVVMContext.SetParentViewModel(context, parentViewModel);
				MVVMContext.SetParameter(context, parameter);
				if(viewModel != null && context.ViewModelType != null)
					context.SetViewModel(context.ViewModelType, viewModel);
				MVVMContext.SetDocumentOwner(context, this);
			}
		}
		#region static
		static UI.IViewLocator GetViewLocator(object parentViewModel) {
			var parentViewLocator = MVVMContext.GetService<UI.IViewLocator>(parentViewModel);
			return parentViewLocator ?? MVVMContext.GetDefaultService<UI.IViewLocator>();
		}
		static Control ResolveViewControl(string viewType, object parentViewModel, UI.IViewLocator viewLocator) {
			return
				ResolveViewControlCore(viewType, parentViewModel, viewLocator, () => GetViewLocator(parentViewModel)) ??
				ResolveViewControlCore(viewType, parentViewModel, GetViewLocator(parentViewModel), () => UI.ViewLocator.Instance);
		}
		static Control ResolveViewControlCore(string viewType, object parentViewModel, UI.IViewLocator viewLocator, System.Func<UI.IViewLocator> GetParentViewLocator) {
			var viewControl = (viewLocator != null) ? viewLocator.Resolve(viewType, parentViewModel) as Control : null;
			if(viewControl == null) {
				var parentViewLocator = GetParentViewLocator();
				if(viewLocator != parentViewLocator && parentViewLocator != null)
					viewControl = parentViewLocator.Resolve(viewType, parentViewModel) as Control;
			}
			return viewControl;
		}
		#endregion static
		#region Validation Helper
		protected static class ValidationHelper {
			static System.Reflection.FieldInfo fldUnvalidatedControl;
			static System.Reflection.FieldInfo fldFocusedControl;
			public static void Reset(Control view) {
				if(view != null)
					Reset(view as ContainerControl ?? view.GetContainerControl() as ContainerControl);
			}
			public static void Reset(ContainerControl container) {
				if(container == null) 
					return;
				try {
					ResetField(ref fldUnvalidatedControl, "unvalidatedControl", container);
					ResetField(ref fldFocusedControl, "focusedControl", container);
				}
				catch { }
			}
			static void ResetField(ref System.Reflection.FieldInfo fInfo, string fieldName, ContainerControl container) {
				fInfo = EnsureField(fInfo, fieldName);
				if(fInfo == null)
					return;
				Control control = fInfo.GetValue(container) as Control;
				if(IsChild(container.ActiveControl, control))
					fInfo.SetValue(container, null);
			}
			static System.Reflection.FieldInfo EnsureField(System.Reflection.FieldInfo fInfo, string fieldName) {
				return fInfo ?? typeof(ContainerControl).GetField(fieldName,
					System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			}
			static bool IsChild(Control control, Control container) {
				while(control != null) {
					if(control == container)
						return true;
					control = control.Parent;
				}
				return false;
			}
		}
		#endregion
	}
}
