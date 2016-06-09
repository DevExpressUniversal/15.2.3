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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils.Design;
using System.Reflection;
using DevExpress.XtraSpellChecker;
using System.Text;
using DevExpress.XtraSpellChecker.Native;
using System.Security.Permissions;
using System.ComponentModel.Design.Serialization;
using System.Security;
namespace DevExpress.XtraSpellChecker.Design {
	public class CustomDropDownForm : Panel {
		private ListBox listBox;
		private object editValue;
		private DropDownEditor editor;
		public CustomDropDownForm(DropDownEditor editor, object editValue) {
			Editor = editor;
			BorderStyle = BorderStyle.None;
			EditValue = editValue;
			listBox = new ListBox();
			listBox.Dock = DockStyle.Fill;
			listBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			listBox.MouseUp += new MouseEventHandler(lbMouseUp);
			FillListBox();
			this.Size = new Size(0, listBox.ItemHeight * 7);
			listBox.SelectedIndexChanged += new EventHandler(lbSelectedIndexChanged);
			Controls.Add(listBox);
		}
		protected virtual void FillListBox() {
			int index = listBox.Items.IndexOf(EditValue);
			listBox.SelectedIndex = index;
		}
		protected ListBox ListBox {
			get { return listBox; }
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				listBox.MouseUp -= new MouseEventHandler(lbMouseUp);
				listBox.SelectedIndexChanged -= new EventHandler(lbSelectedIndexChanged);
				listBox.Dispose();
				listBox = null;
			}
			base.Dispose(disposing);
		}
		public object EditValue {
			get { return editValue; }
			set {
				if (editValue != value) {
					editValue = value;
					editor.edSvc.CloseDropDown();
				}
			}
		}
		public virtual DropDownEditor Editor {
			get { return editor; }
			set {
				if (editor != value)
					editor = value;
			}
		}
		protected virtual void lbMouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			EditValue = GetEditValue(sender);
		}
		protected virtual void lbSelectedIndexChanged(object sender, EventArgs e) {
			EditValue = GetEditValue(sender);
		}
		protected virtual object GetEditValue(object sender) {
			return (sender as ListBox).SelectedItem;
		}
	}
	public class FileNameEditor : System.Drawing.Design.UITypeEditor {
		[SecuritySafeCritical]
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		[SecuritySafeCritical]
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			System.Windows.Forms.OpenFileDialog fd = new System.Windows.Forms.OpenFileDialog();
			if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				return fd.FileName;
			return "";
		}
	}
	public class DropDownEditor : System.Drawing.Design.UITypeEditor {
		internal IWindowsFormsEditorService edSvc;
		protected virtual CustomDropDownForm CreateDropDownForm(object value) {
			return new CustomDropDownForm(this, value);
		}
		[SecuritySafeCritical]
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		protected virtual object GetReturnValue(object editValue) {
			return Convert.ToString(editValue);
		}
		[SecuritySafeCritical]
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (context == null || context.Instance == null || provider == null)
				return value;
			edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if (edSvc == null) return value;
			CustomDropDownForm form = CreateDropDownForm(value);
			edSvc.DropDownControl(form);
			return GetReturnValue(form.EditValue);
		}
	}
	public class EncodingsEditor : DropDownEditor {
		protected override object GetReturnValue(object editValue) {
			return editValue;
		}
		protected override CustomDropDownForm CreateDropDownForm(object value) {
			return new EncodingsDropDownForm(this, value);
		}
	}
	public class EncodingsDropDownForm : CustomDropDownForm {
		static ArrayList encodings = null;
		public EncodingsDropDownForm(DropDownEditor editor, object editValue) : base(editor, editValue) { }
		static void FillEncodings() {
			encodings = new ArrayList();
			EncodingInfo[] encodingInfos = Encoding.GetEncodings();
			foreach (EncodingInfo encodingInfo in encodingInfos)
				encodings.Add(encodingInfo.GetEncoding());
			encodings.Sort(new EncodingComparer());
		}
		protected override object GetEditValue(object sender) {
			return Encoding.GetEncoding(((EncodingListBoxItem)ListBox.SelectedItem).CodePage);
		}
		protected override void FillListBox() {
			if (encodings == null)
				FillEncodings();
			int count = encodings.Count;
			int selectedIndex = 0;
			for (int i = 0; i < count; i++) {
				Encoding encoding = (Encoding)encodings[i];
				if (encoding.Equals(EditValue))
					selectedIndex = i;
				ListBox.Items.Add(new EncodingListBoxItem(encoding));
			}
			ListBox.SelectedIndex = selectedIndex;
		}
		public new EncodingsEditor Editor {
			get { return base.Editor as EncodingsEditor; }
			set { base.Editor = value; }
		}
	}
	public class DictionaryItemCollectionEditor : DXCollectionEditor {
		static Type[] itemTypes = { typeof(SpellCheckerDictionary), typeof(SpellCheckerISpellDictionary), typeof(SpellCheckerOpenOfficeDictionary), typeof(SpellCheckerCustomDictionary), typeof(HunspellDictionary) };
		public DictionaryItemCollectionEditor(Type type) : base(type) { }
		protected override Type[] CreateNewItemTypes() {
			return itemTypes;
		}
	}
	public class EncodingListBoxItem {
		readonly string encodingName;
		readonly int codePage;
		public EncodingListBoxItem(Encoding encoding) {
			this.encodingName = encoding.EncodingName;
			this.codePage = encoding.CodePage;
		}
		public string EncodingName { get { return encodingName; } }
		public int CodePage { get { return codePage; } }
		public override string ToString() {
			return EncodingName;
		}
	}
	public class EncodingComparer : IComparer {
		public int Compare(object x, object y) {
			string language1 = ((Encoding)x).EncodingName;
			string language2 = ((Encoding)y).EncodingName;
			return string.Compare(language1, language2);
		}
	}
	public class EncodingTypeConverter : TypeConverter {
		public static void Register() {
			TypeDescriptor.AddAttributes(typeof(Encoding), new TypeConverterAttribute(typeof(EncodingTypeConverter)));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(String))
				return true;
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			Encoding item = value as Encoding;
			if (item != null) {
				if (destinationType == typeof(String))
					return item.EncodingName;
				if (destinationType == typeof(InstanceDescriptor)) {
					MethodInfo methodInfo = typeof(Encoding).GetMethod("GetEncoding", new Type[] { typeof(int) });
					if (methodInfo != null)
						return new InstanceDescriptor(methodInfo, new object[] { item.CodePage });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			return base.ConvertFrom(context, culture, value);
		}
	}
	public class SpellCheckerDesigner : BaseComponentDesigner, IServiceProvider {
		IComponentChangeService changeService;
		IDesignerHost host;
		public SpellCheckerDesigner() {
			EncodingTypeConverter.Register();
		}
		public SpellChecker SpellChecker { get { return Component as SpellChecker; } }
		protected IComponentChangeService ChangeService { get { return changeService; } }
		protected IDesignerHost DesignerHost { get { return host; } }
		protected override bool AllowHookDebugMode { get { return true; } }
		object IServiceProvider.GetService(Type type) {
			return base.GetService(type);
		}
		protected override DXAboutActionList GetAboutAction() {
			return new DXAboutActionList(Component, new MethodInvoker(SpellChecker.About));
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			this.host = (IDesignerHost)GetService(typeof(IDesignerHost));
			ProcessExistingControls();
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (changeService != null) {
				changeService.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
				changeService.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
			}
		}
		protected virtual void ProcessExistingControls() {
			if (host != null && host.RootComponent != null && host.RootComponent.Site.Container != null && host.RootComponent.Site.Container.Components != null)
				for (int i = 0; i < host.RootComponent.Site.Container.Components.Count; i++)
					if (CanSetMenu(host.RootComponent.Site.Container.Components[i]))
						SpellChecker.SetShowSpellCheckMenu(((Control)host.RootComponent.Site.Container.Components[i]), true);
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (changeService != null) {
						changeService.ComponentAdded -= new ComponentEventHandler(OnComponentAdded);
						changeService.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected virtual bool CanSetMenu(IComponent component) {
			if (component is Control)
				return SpellCheckTextControllersManager.Default.IsClassRegistered(component.GetType());
			return false;
		}
		protected virtual void OnComponentRemoved(object sender, ComponentEventArgs e) {
			if (CanSetMenu(e.Component)) {
				Control control = (Control)e.Component;
				if (SpellChecker.GetShowSpellCheckMenu(control))
					SpellChecker.SetShowSpellCheckMenu(control, false);
			}
		}
		protected virtual void OnComponentAdded(object sender, ComponentEventArgs e) {
			if (CanSetMenu(e.Component))
				SpellChecker.SetShowSpellCheckMenu(((Control)e.Component), true);
		}
	}
}
