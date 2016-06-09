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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraEditors;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.Controls;
using System.ComponentModel.Design;
using DevExpress.XtraPrinting.Design;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.XtraPrinting.Native {
	public interface IPopupOwner {
		void DisableClosing();
		void EnableClosing();
	}
}
namespace DevExpress.XtraPrinting.Native.Lines {
	public interface IObjectWrapperComponent {
		object ObjectInstance { get; }
	}
	public static class ObjectWrapperComponentHelper {
		public static object GetObjectInstance(IComponent component) {
			IObjectWrapperComponent objectWrapperComponent = component as IObjectWrapperComponent;
			if (objectWrapperComponent != null)
				return objectWrapperComponent.ObjectInstance;
			else
				return component;
		}
	}
	public class EditorPropertyLine : EditorPropertyLineBase, IWindowsFormsEditorService, IServiceProvider, IPopupOwner {
		#region inner classes
		class DropDownContainerForm : DevExpress.XtraEditors.Popup.PopupContainerForm {
			public DropDownContainerForm(PopupContainerEdit ownerEdit)
				: base(ownerEdit) {
			}
			public override bool AllowMouseClick(System.Windows.Forms.Control control, Point mousePosition) {
				DropDownEdit edit = OwnerEdit as DropDownEdit;
				if(edit != null && !edit.AllowClosing) 
					return true;
				if(control != null) {
					Form form = control.FindForm();
					if(form != null && form.Owner == this)
						return true;
				}
				return base.AllowMouseClick(control, mousePosition);
			}
		}
		class DropDownEdit : PopupContainerEdit {
			EditorPropertyLine editorPropertyLine;
			bool isDirty;
			bool allowClosing = true;
			internal bool AllowClosing {get {return allowClosing;}}
			public DropDownEdit(EditorPropertyLine editorPropertyLine) {
				this.editorPropertyLine = editorPropertyLine;
				Properties.AutoHeight = true;
				Properties.CloseOnLostFocus = true;
				Properties.CloseOnOuterMouseClick = true;
				Properties.PopupBorderStyle = PopupBorderStyles.Simple;
				Properties.PopupSizeable = false;
				Properties.ShowPopupCloseButton = false;
				Properties.ShowPopupShadow = false;
			}
			public bool IsDirty { get { return isDirty; } }			
			public System.Windows.Forms.Control GetPopupWindow() {
				return PopupForm;
			}
			public void SetText(string value) {
				if(!Object.Equals(Text, value)) {
					ToolTip = Text = value;
					ErrorText = "";
					isDirty = false;
				}
			}
			protected override DevExpress.XtraEditors.Popup.PopupBaseForm CreatePopupForm() {
				if(Properties.PopupControl == null) return null;
				return new DropDownContainerForm(this);
			}
			protected override void NotifyButtonStateChanged(EditorButton button) {
				if(!IsDisposed)
					base.NotifyButtonStateChanged(button);
			}
			protected override void OnTextChanged(EventArgs e) {
				base.OnTextChanged(e);
				isDirty = true;
			}
			protected override void DoShowPopup() {
				base.DoShowPopup();
				if(!editorPropertyLine.isRunning)
					DoEditValue();
			}
			protected override void OnEditorKeyDown(KeyEventArgs e) {
				if(editorPropertyLine.isRunning) {
					base.OnEditorKeyDown(e);
					return;
				}
				base.OnEditorKeyDown(e);
			}
			void DoEditValue() {
				if(Properties.TextEditStyle == TextEditStyles.Standard) {
					SelectionLength = 0;
					SelectionStart = 0;
					Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
					try {
						editorPropertyLine.EditValue();
					}
					finally {
						Properties.TextEditStyle = TextEditStyles.Standard;
						if(MaskBox != null)
							MaskBox.Focus();
						SelectionLength = 0;
						SelectionStart = 0;
					}
				}
				else
					editorPropertyLine.EditValue();
			}
			protected override void OnPopupClosed(PopupCloseMode closeMode) {
				base.OnPopupClosed(closeMode);
				editorPropertyLine.isRunning = false;
			}
			protected override void ClosePopup(PopupCloseMode closeMode) {
				if(allowClosing)
					base.ClosePopup(closeMode);
			}
			public void EnableClosing() {
				allowClosing = true;
			}
			public void DisableClosing() {
				allowClosing = false;
			}
		}
		#endregion
		UITypeEditor editor;
		ITypeDescriptorContext context;		
		PopupContainerControl popupControl;
		IComponent component;
		IServiceProvider provider;
		bool isRunning;
		public EditorPropertyLine(IServiceProvider provider, IComponent component, IStringConverter converter, PropertyDescriptor property, object obj)
			: base(converter, property, obj) {
			this.component = component;
			this.provider = provider;
		}
		DropDownEdit PopupEdit {
			get { return (DropDownEdit)baseEdit; }
		}
		protected ITypeDescriptorContext Context {
			get {
				if(context == null)
					context = new TypeDescriptorContext(component != null && component.Site != null ? component.Site : provider, Property, ObjectWrapperComponentHelper.GetObjectInstance(component));
				return context;
			}
		}
		public bool IsRunning { get { return isRunning; } }
		void EditValue() {
			object editValue = null;
			object oldValue = Value;
			if(IsModalEditor()) {
				try {
					isRunning = true;
					PopupEdit.Properties.Buttons[0].Visible = false;
					editValue = editor.EditValue(Context, this, Value);
				} catch(Exception ex) {
					IUIService serv = provider.GetService(typeof(IUIService)) as IUIService;
					if(serv != null)
						serv.ShowError(ex);
				} finally {
					PopupEdit.Properties.Buttons[0].Visible = true;
					isRunning = false;
				}
			}
			else {
				PopupEdit.Properties.PopupControl = popupControl;
				editValue = editor.EditValue(Context, this, Value);
				PopupEdit.Properties.PopupControl = null;
			}
			if(!Property.IsReadOnly && editValue != oldValue) {
				try {
					Value = editValue;
				}
				catch(Exception ex) {
					PopupEdit.ErrorText = ex.Message;
				}
			}
			else {
				RefreshProperty();
				RefreshLines();
			}
		}
		object IServiceProvider.GetService(Type type) {
			if(type == typeof(IWindowsFormsEditorService))
				return this;
			else if(type == typeof(UserLookAndFeel) && UserLookAndFeel != null)
				return UserLookAndFeel;
			else if(provider != null)
				return provider.GetService(type);
			else
				return null;
		}
		void IWindowsFormsEditorService.CloseDropDown() {
			PopupEdit.ClosePopup();
		}
		void IWindowsFormsEditorService.DropDownControl(System.Windows.Forms.Control control) {
			try {
				isRunning = true;
				popupControl.Size = control.Size;
				control.Dock = DockStyle.Fill;
				popupControl.Controls.Clear();
				popupControl.Controls.Add(control);
				PopupEdit.ShowPopup();
				control.Focus();
				while(isRunning) {
					Win32.WaitMessage();
					Application.DoEvents();
				}
			}
			finally {
				isRunning = false;
			}
		}
		DialogResult IWindowsFormsEditorService.ShowDialog(Form form) {
			if(form != null) {
				form.StartPosition = FormStartPosition.CenterScreen;
				if(form is ISupportLookAndFeel)
					LookAndFeelProviderHelper.SetParentLookAndFeel((ISupportLookAndFeel)form, this);
				IUIService uiService = provider.GetService<IUIService>();
				if(uiService != null)
					return uiService.ShowDialog(form);
				return form.ShowDialog(FindForm());
			}
			return DialogResult.Cancel;
		}
		protected bool IsModalEditor() {
			return editor.GetEditStyle(Context) == System.Drawing.Design.UITypeEditorEditStyle.Modal;
		}
		protected override void Initialize() {
			base.Initialize();
			popupControl = new PopupContainerControl();
			popupControl.BorderStyle = BorderStyles.NoBorder;
		}
		protected override BaseEdit CreateEditor() {
			return new DropDownEdit(this);
		}
		protected override void IntializeEditor() {
			base.IntializeEditor();
			if(!Property.IsReadOnly && converter.CanConvertFromString()) {
				PopupEdit.Properties.TextEditStyle = TextEditStyles.Standard;
				PopupEdit.Properties.ValidateOnEnterKey = true;
				PopupEdit.Validating += new CancelEventHandler(OnBaseEditValidating);
			}
		}
		protected override void SetEditText(object val) {
			PopupEdit.SetText(ValueToString(val));
		}
		protected override void OnBaseEditValidating(object sender, CancelEventArgs e) {
			if(PopupEdit.IsDirty)
				base.OnBaseEditValidating(sender, e);
		}
		public override void Init(UserLookAndFeel lookAndFeel) {
			base.Init(lookAndFeel);
			editor = Property.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
			if(IsModalEditor())
				PopupEdit.Properties.Buttons[0].Kind = ButtonPredefines.Ellipsis;
			if(editor.IsDropDownResizable)
				PopupEdit.Properties.PopupSizeable = true;
		}
		public override System.Windows.Forms.Control GetPopupWindow() {
			return PopupEdit.GetPopupWindow();
		}
		void IPopupOwner.DisableClosing() {
			IPopupOwner parentOwner = FindForm() as IPopupOwner;
			if(parentOwner != null)
				parentOwner.DisableClosing();
			PopupEdit.DisableClosing();
		}
		void IPopupOwner.EnableClosing() {
			IPopupOwner parentOwner = FindForm() as IPopupOwner;
			if(parentOwner != null)
				parentOwner.EnableClosing();
			PopupEdit.EnableClosing();
		}
	}
	public sealed class TypeDescriptorContext : RuntimeTypeDescriptorContext {
		IServiceProvider serviceProvider;
		public TypeDescriptorContext(IServiceProvider serviceProvider, PropertyDescriptor propDesc, object instance)
			: base(propDesc, instance) {
			this.serviceProvider = serviceProvider;
		}
		public override IContainer Container {
			get {
				return GetService(typeof(IContainer)) as IContainer;
			}
		}
		public override object GetService(Type serviceType) {
			return serviceProvider == null ? null : serviceProvider.GetService(serviceType);
		}
		public override bool OnComponentChanging() {
			IComponentChangeService componentChangeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(componentChangeService != null)
				componentChangeService.OnComponentChanging(this.Instance, this.PropertyDescriptor);
			return true;
		}
		public override void OnComponentChanged() {
			IComponentChangeService componentChangeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(componentChangeService != null)
				componentChangeService.OnComponentChanged(this.Instance, this.PropertyDescriptor, null, null);
		}
	}
}
