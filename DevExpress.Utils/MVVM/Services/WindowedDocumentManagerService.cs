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
	using System.ComponentModel;
	using System.Drawing;
	using System.Linq;
	using System.Windows.Forms;
	public interface IWindowedDocumentAdapterFactory {
		IDocumentAdapter Create();
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IDocumentFormFactory {
		Form Create();
	}
	public enum DefaultWindowedDocumentManagerServiceType {
		Default,
		Form,
		XtraForm,
		RibbonForm,
		FlyoutForm
	}
	public class WindowedDocumentManagerService : DocumentManagerService {
		protected WindowedDocumentManagerService(Func<IDocumentAdapter> factoryMethod, IWin32Window owner)
			: base(factoryMethod) {
			this.Owner = owner;
			this.StartPosition = FormStartPosition.CenterParent;
		}
		public IWin32Window Owner { get; private set; }
		public FormStartPosition StartPosition { get; set; }
		public string Title { get; set; }
		public Action<Form> FormStyle { get; set; }
		protected override void Initialize(IDocumentAdapter adapter) {
			FormAdapter formAdapter = adapter as FormAdapter;
			if(formAdapter != null) {
				formAdapter.Title = Title;
				formAdapter.Owner = Owner;
				formAdapter.StartPosition = StartPosition;
				formAdapter.FormStyle = FormStyle;
			}
		}
		#region static
		public static WindowedDocumentManagerService CreateXtraFormService(IWin32Window owner = null) {
			return Create(DefaultFormDocumentAdapterFactory.XtraFormFactory.Value, owner);
		}
		[Obsolete("Use the CreateRibbonFormService() method instead.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static WindowedDocumentManagerService CreateRibbbonFormService(IWin32Window owner = null) {
			return Create(DefaultFormDocumentAdapterFactory.RibbonFormFactory.Value, owner);
		}
		public static WindowedDocumentManagerService CreateRibbonFormService(IWin32Window owner = null) {
			return Create(DefaultFormDocumentAdapterFactory.RibbonFormFactory.Value, owner);
		}
		public static WindowedDocumentManagerService CreateFlyoutFormService(IWin32Window owner = null) {
			return Create(DefaultFormDocumentAdapterFactory.FlyoutFormFactory.Value, owner);
		}
		public static WindowedDocumentManagerService Create(IWin32Window owner = null) {
			return Create(DefaultFormDocumentAdapterFactory.Default, owner);
		}
		public static WindowedDocumentManagerService Create(DefaultWindowedDocumentManagerServiceType type, IWin32Window owner = null) {
			return Create(DefaultFormDocumentAdapterFactory.GetFactory(type), owner);
		}
		public static WindowedDocumentManagerService Create(Func<Form> factoryMethod, IWin32Window owner = null) {
			return Create(() => new FormAdapter(factoryMethod), owner);
		}
		public static WindowedDocumentManagerService Create(IWindowedDocumentAdapterFactory factory, IWin32Window owner = null) {
			return Create(() => factory.Create(), owner);
		}
		static WindowedDocumentManagerService Create(Func<IDocumentAdapter> factoryMethod, IWin32Window owner) {
			IMVVMServiceTypesResolver typesResolver = MVVMTypesResolver.Instance as IMVVMServiceTypesResolver;
			return DynamicServiceSource.Create<WindowedDocumentManagerService, Func<IDocumentAdapter>, IWin32Window>(
					new Type[] { 
						typesResolver.GetIDocumentManagerServiceType(), 
						typesResolver.GetIDocumentOwnerType(), 
					}, factoryMethod, owner);
		}
		#endregion static
		#region FormAdapter
		[EditorBrowsable(EditorBrowsableState.Never)]
		public sealed class FormDocumentAdapterFactory : IWindowedDocumentAdapterFactory {
			Func<Form> factoryMethod;
			internal FormDocumentAdapterFactory(string srAssembly, string srFactoryType)
				: this(EnsureFormFactory(srAssembly, srFactoryType)) {
			}
			public FormDocumentAdapterFactory(IDocumentFormFactory formFactory) {
				this.factoryMethod = () =>
				{
					return (formFactory != null) ? formFactory.Create() : new Form();
				};
			}
			IDocumentAdapter IWindowedDocumentAdapterFactory.Create() {
				return new FormAdapter(factoryMethod);
			}
			static IDocumentFormFactory EnsureFormFactory(string srAssembly, string srFactoryType) {
				Type formFactoryType = null;
				if(TryGetFormFactoryType(srAssembly, srFactoryType, out formFactoryType))
					return Activator.CreateInstance(formFactoryType) as IDocumentFormFactory;
				return null;
			}
			static bool TryGetFormFactoryType(string srAssembly, string srFactoryType, out Type formFactoryType) {
				formFactoryType = null;
				var assembly = AssemblyHelper.GetLoadedAssembly(srAssembly)
					?? AssemblyHelper.LoadDXAssembly(srAssembly);
				if(assembly != null)
					formFactoryType = assembly.GetType(srFactoryType);
				return formFactoryType != null;
			}
		}
		sealed class FormDocumentAdapterFactory<TForm> : IWindowedDocumentAdapterFactory
			where TForm : Form, new() {
			IDocumentAdapter IWindowedDocumentAdapterFactory.Create() {
				return new FormAdapter(() => new TForm() { ShowIcon = false });
			}
		}
		sealed class FormAdapter : IDocumentAdapter {
			Form form;
			Func<Form> factoryMethod;
			public FormAdapter(Func<Form> factoryMethod) {
				this.factoryMethod = factoryMethod;
			}
			public void Dispose() {
				if(form != null) {
					var control = GetControl(form);
					if(control != null)
						control.TextChanged -= control_TextChanged;
					form.FormClosed -= form_FormClosed;
					form.FormClosing -= form_FormClosing;
					form = null;
				}
			}
			void form_FormClosing(object sender, FormClosingEventArgs e) {
				if(sender == form)
					RaiseClosing(e);
			}
			void form_FormClosed(object sender, FormClosedEventArgs e) {
				if(sender == form) {
					RaiseClosed(e);
					Dispose();
				}
			}
			void control_TextChanged(object sender, EventArgs e) {
				form.Text = ((Control)sender).Text;
			}
			void RaiseClosed(FormClosedEventArgs e) {
				if(Closed != null) Closed(form, e);
			}
			void RaiseClosing(FormClosingEventArgs e) {
				if(Closing != null) Closing(form, e);
			}
			public void Show(Control control) {
				var openForm = Application.OpenForms.OfType<Form>().FirstOrDefault(f => GetControl(f) == control);
				if(openForm == null) {
					form = factoryMethod();
					form.StartPosition = StartPosition;
					var ownerForm = (Owner as Form) ?? (Owner is Control ? ((Control)Owner).FindForm() : null) ?? GuessOwner();
					if(ownerForm != null) {
						form.RightToLeft = ownerForm.RightToLeft;
						form.RightToLeftLayout = ownerForm.RightToLeftLayout;
					}
					form.Text = GetText(control);
					form.FormClosed += form_FormClosed;
					form.FormClosing += form_FormClosing;
					control.TextChanged += control_TextChanged;
				}
				if(form != null) {
					IDialogForm dialog = form as IDialogForm;
					if(dialog == null) {
						form.ClientSize = control.Size;
						if(FormStyle != null)
							FormStyle(form);
						control.Dock = DockStyle.Fill;
						form.Controls.Add(control);
						if(form.StartPosition == FormStartPosition.CenterParent)
							form.Load += form_Load;
						if(!form.Visible)
							form.Show(Owner ?? GuessOwner());
						else
							form.Activate();
					}
					else dialog.ShowDialog(Owner ?? GuessOwner(), control, GetText(control), new DialogResult[] { });
				}
			}
			string GetText(Control control) {
				return !string.IsNullOrEmpty(control.Text) ? control.Text : Title;
			}
			void form_Load(object sender, EventArgs e) {
				Form form = sender as Form;
				if(form != null) {
					form.Load -= form_Load;
					if(form.Owner != null) {
						form.Location = new Point(
							form.Owner.Location.X + form.Owner.Width / 2 - form.Width / 2,
							form.Owner.Location.Y + form.Owner.Height / 2 - form.Height / 2);
					}
				}
			}
			public void Close(Control control, bool force = true) {
				if(force)
					form.FormClosing -= form_FormClosing;
				if(control != null)
					control.TextChanged -= control_TextChanged;
				form.Close();
			}
			internal static Form GuessOwner() {
				Form frm = Form.ActiveForm;
				if(frm == null || frm.InvokeRequired)
					return null;
				while(frm != null && frm.Owner != null && !frm.ShowInTaskbar && !frm.TopMost)
					frm = frm.Owner;
				return frm;
			}
			public event EventHandler Closed;
			public event CancelEventHandler Closing;
			static Control GetControl(Form form) {
				return form != null && form.Controls.Count > 0 ? form.Controls[0] : null;
			}
			#region Settings
			public IWin32Window Owner { get; internal set; }
			public FormStartPosition StartPosition { get; internal set; }
			public string Title { get; internal set; }
			public Action<Form> FormStyle { get; internal set; }
			#endregion Settings
		}
		static class DefaultFormDocumentAdapterFactory {
			#region Default
			static IWindowedDocumentAdapterFactory PreferredFactory;
			internal static IWindowedDocumentAdapterFactory Default {
				get { return PreferredFactory ?? XtraFormFactory.Value; }
			}
			internal static readonly Lazy<IWindowedDocumentAdapterFactory> FormFactory =
				new Lazy<IWindowedDocumentAdapterFactory>(() => new FormDocumentAdapterFactory<Form>());
			internal static readonly Lazy<IWindowedDocumentAdapterFactory> XtraFormFactory =
				new Lazy<IWindowedDocumentAdapterFactory>(() => new FormDocumentAdapterFactory<DevExpress.XtraEditors.XtraForm>());
			internal static readonly Lazy<IWindowedDocumentAdapterFactory> RibbonFormFactory =
				new Lazy<IWindowedDocumentAdapterFactory>(() => new FormDocumentAdapterFactory(AssemblyInfo.SRAssemblyBars, "DevExpress.XtraBars.MVVM.Services.RibbonDocumentFormFactory"));
			internal static readonly Lazy<IWindowedDocumentAdapterFactory> FlyoutFormFactory =
				new Lazy<IWindowedDocumentAdapterFactory>(() => new FormDocumentAdapterFactory(AssemblyInfo.SRAssemblyBars, "DevExpress.XtraBars.MVVM.Services.FlyoutDialogFormFactory"));
			#endregion Default
			internal static IWindowedDocumentAdapterFactory PreferFormService() {
				return SetPreferredFactory(FormFactory.Value);
			}
			internal static IWindowedDocumentAdapterFactory PreferXtraFormService() {
				return SetPreferredFactory(XtraFormFactory.Value);
			}
			internal static IWindowedDocumentAdapterFactory PreferRibbonFormService() {
				return SetPreferredFactory(RibbonFormFactory.Value);
			}
			internal static IWindowedDocumentAdapterFactory PreferFlyoutFormService() {
				return SetPreferredFactory(FlyoutFormFactory.Value);
			}
			static IWindowedDocumentAdapterFactory SetPreferredFactory(IWindowedDocumentAdapterFactory factory) {
				return (PreferredFactory = factory);
			}
			internal static IWindowedDocumentAdapterFactory GetFactory(DefaultWindowedDocumentManagerServiceType type) {
				switch(type) {
					case DefaultWindowedDocumentManagerServiceType.Form:
						return FormFactory.Value;
					case DefaultWindowedDocumentManagerServiceType.XtraForm:
						return XtraFormFactory.Value;
					case DefaultWindowedDocumentManagerServiceType.RibbonForm:
						return RibbonFormFactory.Value;
					case DefaultWindowedDocumentManagerServiceType.FlyoutForm:
						return FlyoutFormFactory.Value;
					default:
						return Default;
				}
			}
		}
		#endregion FormAdapter
		internal static void RegisterFormService() {
			RegisterService(DefaultFormDocumentAdapterFactory.PreferFormService());
		}
		internal static void RegisterXtraFormService() {
			RegisterService(DefaultFormDocumentAdapterFactory.PreferXtraFormService());
		}
		internal static void RegisterRibbonFormService() {
			RegisterService(DefaultFormDocumentAdapterFactory.PreferRibbonFormService());
		}
		internal static void RegisterFlyoutFormService() {
			RegisterService(DefaultFormDocumentAdapterFactory.PreferFlyoutFormService());
		}
		static void RegisterService(IWindowedDocumentAdapterFactory factory) {
			IPOCOInterfaces pocoInterfaces = POCOInterfacesProxy.Instance;
			object serviceContainer = pocoInterfaces.GetDefaultServiceContainer();
			var service = WindowedDocumentManagerService.Create(factory);
			pocoInterfaces.RegisterService(serviceContainer, service);
		}
	}
}
