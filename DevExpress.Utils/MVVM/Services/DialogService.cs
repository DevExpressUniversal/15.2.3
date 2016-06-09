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
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Windows.Forms;
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IDialogForm : IDisposable {
		Control Content { get; }
		DialogResult ShowDialog(IWin32Window owner, Control content, string caption, DialogResult[] buttons);
		void Close();
		event CancelEventHandler Closing;
		event EventHandler Closed;
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IDialogFormFactory {
		IDialogForm Create();
	}
	public enum DefaultDialogServiceType {
		Default,
		XtraDialog,
		RibbonDialog,
		FlyoutDialog
	}
	public class DialogService : ViewServiceBase {
		protected DialogService(IWin32Window owner, string title, IDialogFormFactory factory) {
			this.Owner = owner;
			this.Title = title;
			this.factoryCore = factory;
		}
		public IWin32Window Owner { get; private set; }
		public string Title { get; private set; }
		IDialogFormFactory factoryCore;
		protected IDialogFormFactory Factory {
			get { return factoryCore ?? DefaultDialogFormFactory.Default; }
		}
		public static DialogService CreateXtraDialogService(IWin32Window owner, string title = null) {
			return Create(owner, title, DefaultDialogFormFactory.XtraDialogFormFactory.Value);
		}
		public static DialogService CreateFlyoutDialogService(IWin32Window owner, string title = null) {
			return Create(owner, title, DefaultDialogFormFactory.FlyoutDialogFormFactory.Value);
		}
		public static DialogService CreateRibbonDialogService(IWin32Window owner, string title = null) {
			return Create(owner, title, DefaultDialogFormFactory.RibbonDialogFormFactory.Value);
		}
		public static DialogService Create(IWin32Window owner, string title = null, Func<IDialogForm> factoryMethod = null) {
			return Create(owner, title, DefaultDialogFormFactory.FromMethod(factoryMethod));
		}
		public static DialogService Create(IWin32Window owner, DefaultDialogServiceType type, string title = null) {
			return Create(owner, title, DefaultDialogFormFactory.GetFactory(type));
		}
		public static DialogService Create(IWin32Window owner, string title = null, IDialogFormFactory factory = null) {
			IMVVMServiceTypesResolver typesResolver = MVVMTypesResolver.Instance as IMVVMServiceTypesResolver;
			return DynamicServiceSource.Create<DialogService, IWin32Window, string, IDialogFormFactory>(
					new Type[] { 
						typesResolver.GetIDialogServiceType(), 
						typesResolver.GetIDocumentOwnerType(), 
						typesResolver.GetIMessageButtonLocalizerType()
					}, owner, title, factory);
		}
		#region Implementation
		public string Localize(int button) {
			return button.ToString();
		}
		public object ShowDialog(IEnumerable dialogCommands, string title, string documentType, object viewModel, object parameter, object parentViewModel) {
			object[] commands = dialogCommands.OfType<object>().ToArray();
			DialogResult[] results = GetResults(commands);
			DialogResult result = results.FirstOrDefault();
			if(Factory != null && !DevExpress.Utils.Design.DesignTimeTools.IsDesignMode) {
				using(var dialogForm = Factory.Create()) {
					DialogForm.Register(dialogForm);
					dialogForm.Closing += DialogForm_Closing;
					dialogForm.Closed += DialogForm_Closed;
					Control content = CreateAndInitializeView(documentType, viewModel, parameter, parentViewModel);
					string caption = (!string.IsNullOrEmpty(title) ? title : Title) ?? MVVMContext.GetTitle(content) as string;
					result = dialogForm.ShowDialog(Owner, content, caption, results);
					dialogForm.Closing -= DialogForm_Closing;
					dialogForm.Closed -= DialogForm_Closed;
				}
			}
			int resultIndex = Array.IndexOf(results, result);
			return (resultIndex != -1 && commands.Length > 0) ? commands[resultIndex] : null;
		}
		public void Close(object documentContent, bool force) {
			IDialogForm dialogForm = DialogForm.ByContent(documentContent);
			if(dialogForm == null) return;
			if(force) {
				ValidationHelper.Reset(dialogForm as Control ?? dialogForm.Content);
				dialogForm.Closing -= DialogForm_Closing;
			}
			dialogForm.Close();
		}
		#endregion Implementation
		static DialogResult[] GetResults(object[] commands) {
			DialogResult[] results = new DialogResult[commands.Length];
			var uiCommandType = MVVMTypesResolver.Instance.GetUICommandType();
			for(int i = 0; i < commands.Length; i++)
				results[i] = DialogResultEnumsConverter.FromMessageResult((int)InterfacesProxy.GetUICommandTag(uiCommandType, commands[i]));
			return results;
		}
		void DialogForm_Closing(object sender, CancelEventArgs e) {
			var dialogForm = (IDialogForm)sender;
			MVVMContext.OnClose(dialogForm.Content, e);
		}
		void DialogForm_Closed(object sender, EventArgs e) {
			var dialogForm = (IDialogForm)sender;
			dialogForm.Closing -= DialogForm_Closing;
			dialogForm.Closed -= DialogForm_Closed;
			MVVMContext.OnDestroy(dialogForm.Content);
		}
		#region DialogForm
		static class DialogForm {
			static internal IDialogForm Register(IDialogForm form) {
				if(form != null)
					forms.Add(new WeakReference(form));
				return form;
			}
			static List<WeakReference> forms = new List<WeakReference>();
			static internal IDialogForm ByContent(object documentContent) {
				return GetForms().Where(form => MVVMContext.GetViewModel(form.Content) == documentContent).FirstOrDefault();
			}
			static IEnumerable<IDialogForm> GetForms() {
				for(int formIndex = forms.Count; --formIndex >= 0; ) {
					IDialogForm form = (IDialogForm)forms[formIndex].Target;
					if(form == null)
						forms.RemoveAt(formIndex);
					else
						yield return form;
				}
			}
		}
		#endregion DialogForm
		#region DefaultDialogFormFactory
		sealed class DefaultDialogFormFactory : IDialogFormFactory {
			#region static
			internal static IDialogFormFactory Default {
				get { return XtraDialogFormFactory.Value; }
			}
			internal static IDialogFormFactory GetFactory(DefaultDialogServiceType type) {
				switch(type) {
					case DefaultDialogServiceType.XtraDialog:
						return DefaultDialogFormFactory.XtraDialogFormFactory.Value;
					case DefaultDialogServiceType.RibbonDialog:
						return DefaultDialogFormFactory.RibbonDialogFormFactory.Value;
					case DefaultDialogServiceType.FlyoutDialog:
						return DefaultDialogFormFactory.FlyoutDialogFormFactory.Value;
					default:
						return Default;
				}
			}
			internal static readonly Lazy<IDialogFormFactory> XtraDialogFormFactory =
				new Lazy<IDialogFormFactory>(() => new DefaultDialogFormFactory(AssemblyInfo.SRAssemblyEditors, "DevExpress.XtraEditors.MVVM.Services.XtraDialogFormFactory"));
			internal static readonly Lazy<IDialogFormFactory> FlyoutDialogFormFactory =
				new Lazy<IDialogFormFactory>(() => new DefaultDialogFormFactory(AssemblyInfo.SRAssemblyBars, "DevExpress.XtraBars.MVVM.Services.FlyoutDialogFormFactory"));
			internal static readonly Lazy<IDialogFormFactory> RibbonDialogFormFactory =
				new Lazy<IDialogFormFactory>(() => new DefaultDialogFormFactory(AssemblyInfo.SRAssemblyBars, "DevExpress.XtraBars.MVVM.Services.RibbonDialogFormFactory"));
			#endregion static
			IDialogFormFactory defaultFormFactory;
			DefaultDialogFormFactory(string srAssembly, string srfactoryType) {
				Type dialogFormFactoryType = null;
				if(TryGetDialogFormFactoryType(srAssembly, srfactoryType, out dialogFormFactoryType))
					defaultFormFactory = Activator.CreateInstance(dialogFormFactoryType) as IDialogFormFactory;
			}
			bool TryGetDialogFormFactoryType(string srAssembly, string srfactoryType, out Type dialogFormFactoryType) {
				dialogFormFactoryType = null;
				var assembly = AssemblyHelper.GetLoadedAssembly(srAssembly)
					?? AssemblyHelper.LoadDXAssembly(srAssembly);
				if(assembly != null)
					dialogFormFactoryType = assembly.GetType(srfactoryType);
				return dialogFormFactoryType != null;
			}
			IDialogForm IDialogFormFactory.Create() {
				return (defaultFormFactory != null) ? defaultFormFactory.Create() : new FakeDialogForm();
			}
			internal static IDialogFormFactory FromMethod(Func<IDialogForm> factoryMethod) {
				return new MethodDialogFormFactory(factoryMethod);
			}
			class MethodDialogFormFactory : IDialogFormFactory {
				Func<IDialogForm> factoryMethod;
				public MethodDialogFormFactory(Func<IDialogForm> factoryMethod) {
					this.factoryMethod = factoryMethod;
				}
				IDialogForm IDialogFormFactory.Create() {
					return (factoryMethod != null) ? factoryMethod() : new FakeDialogForm();
				}
			}
			class FakeDialogForm : IDialogForm {
				Control content;
				public void Dispose() {
					content = null;
					GC.SuppressFinalize(this);
				}
				Control IDialogForm.Content {
					get { return content; }
				}
				DialogResult IDialogForm.ShowDialog(IWin32Window owner, Control content, string caption, DialogResult[] buttons) {
					this.content = content;
					MessageBox.Show(owner, "Error creating dialog \"" + caption + "\". Please provide your dialog service with an appropriate dialog form implementation");
					return DialogResult.None;
				}
				void IDialogForm.Close() {
					Dispose();
				}
				event CancelEventHandler IDialogForm.Closing { add { } remove { } }
				event EventHandler IDialogForm.Closed { add { } remove { } }
			}
		}
		#endregion
		internal static void RegisterXtraDialogService() {
			RegisterDialogService(DefaultDialogFormFactory.XtraDialogFormFactory.Value);
		}
		internal static void RegisterFlyoutDialogService() {
			RegisterDialogService(DefaultDialogFormFactory.FlyoutDialogFormFactory.Value);
		}
		internal static void RegisterRibbonDialogService() {
			RegisterDialogService(DefaultDialogFormFactory.RibbonDialogFormFactory.Value);
		}
		static void RegisterDialogService(IDialogFormFactory factory) {
			IPOCOInterfaces pocoInterfaces = POCOInterfacesProxy.Instance;
			object serviceContainer = pocoInterfaces.GetDefaultServiceContainer();
			pocoInterfaces.RegisterService(serviceContainer, DialogService.Create(null, null, factory));
		}
	}
	static class DialogResultEnumsConverter {
		internal static DialogResult FromMessageResult(int result) {
			switch(result) {
				case 4: return DialogResult.No;
				case 3: return DialogResult.Yes;
				case 2: return DialogResult.Cancel;
				case 1: return DialogResult.OK;
				default: return DialogResult.None;
			}
		}
	}
}
