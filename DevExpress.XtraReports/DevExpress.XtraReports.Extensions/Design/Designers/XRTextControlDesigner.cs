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
using DevExpress.XtraReports.UI;
using System.ComponentModel;
using DevExpress.Data.Browsing.Design;
using System.Windows.Forms;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.Native;
using DevExpress.Data;
using DevExpress.XtraReports.Design.Commands;
using System.Collections;
using System.Globalization;
using System.Collections.ObjectModel;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.Design.MouseTargets;
using System.Drawing;
using DevExpress.Utils.Internal;
namespace DevExpress.XtraReports.Design {
	[MouseTarget(typeof(TextControlMouseTarget))]
	public class XRTextControlDesigner : XRTextControlBaseDesigner {
		public XRTextControlDesigner()
			: base() {
		}
		protected internal override DesignBinding GetDesignBinding(string propName) {
			if(IsInplaceEditingMode) {
				MailMergeFieldInfo mailMergeFieldInfo = InplaceEditorHelper.GetMailMergeFieldInfoFromInplaceEditor(XRControl);
				return mailMergeFieldInfo != null ? new DesignBinding(XRControl.Report.DataSource, XRControl.Report.GetDataMemberFromDisplayName(mailMergeFieldInfo.DisplayName)) :
					new DesignBinding();
			}
			DesignBinding designBinding = base.GetDesignBinding(propName);
			if(!designBinding.IsNull)
				return designBinding;
			MailMergeFieldInfoCollection columns = EmbeddedFieldsHelper.GetEmbeddedFieldInfos(XRControl);
			return columns.Count == 1 ? new DesignBinding(XRControl.Report.DataSource, XRControl.Report.GetDataMemberFromDisplayName(columns[0].DisplayName)) :
				new DesignBinding();
		}
		protected internal override void SetBinding(string propName, DesignBinding value) {
			XRSmartTagService smartTagService = (XRSmartTagService)DesignerHost.GetService(typeof(XRSmartTagService));
			if(IsInplaceEditingMode) {
				MailMergeFieldInfo mailMergeFieldInfo = InplaceEditorHelper.GetMailMergeFieldInfoFromInplaceEditor(XRControl);
				bool shouldUpdateForm = mailMergeFieldInfo == null;
				string displayColumnName = string.Empty;
				using(XRDataContextBase datacontext = new XRDataContextBase(XRControl.ReportCalculatedFields, true))
					displayColumnName = datacontext.GetDataMemberDisplayName(XRControl.Report.DataSource, XRControl.Report.DataMember, value.DataMember);
				if(value.DataSource is DevExpress.XtraReports.Native.Parameters.ParametersDataSource)
					displayColumnName = DevExpress.XtraReports.Native.Parameters.ParametersReplacer.GetParameterFullName(displayColumnName);
				ReplaceSelection(MailMergeFieldInfo.WrapColumnInfoInBrackets(displayColumnName, string.Empty));
				shouldUpdateForm = !shouldUpdateForm && InplaceEditorHelper.GetMailMergeFieldInfoFromInplaceEditor(XRControl) == null ? true : shouldUpdateForm;
				if(shouldUpdateForm)
					smartTagService.UpdateForm(this, false);
				return;
			}
			if(XRControl.DataBindings["Text"] != null || !EmbeddedFieldsHelper.HasEmbeddedFieldNames(XRControl)) {
				DesignerTransaction changeBindingTransaction = DesignerHost.CreateTransaction(XRComponentPropertyNames.DataBindings);
				try {
					RaiseComponentChanging(XRControl, XRComponentPropertyNames.DataBindings);
					base.SetBinding(propName, value);
					SetDefaultControlText();
					RaiseComponentChanged(XRControl);
					smartTagService.UpdateForm(this, false);
				} catch {
					changeBindingTransaction.Cancel();
				} finally {
					changeBindingTransaction.Commit();
				}
				return;
			}
			DesignerTransaction transaction = DesignerHost.CreateTransaction(XRComponentPropertyNames.Text);
			try {
				RaiseComponentChanging(XRControl, XRComponentPropertyNames.Text);
				bool shouldUpdateForm = !EmbeddedFieldsHelper.HasEmbeddedFieldNames(XRControl);
				EmbeddedFieldsHelper.ChangeBinding(XRControl, value.DataMember);
				shouldUpdateForm = !shouldUpdateForm && !EmbeddedFieldsHelper.HasEmbeddedFieldNames(XRControl) ? true : shouldUpdateForm;
				RaiseComponentChanged(XRControl);
				if(shouldUpdateForm)
					smartTagService.UpdateForm(this, false);
			} catch {
				transaction.Cancel();
			} finally {
				transaction.Commit();
			}
		}
		void SetDefaultControlText() {
			if(XRControl.DataBindings["Text"] == null)
				return;
			DesignerTransaction transaction1 = DesignerHost.CreateTransaction(XRComponentPropertyNames.Text);
			try {
				RaiseComponentChanging(XRControl, XRComponentPropertyNames.Text);
				XRControl.Text = string.Empty;
				RaiseComponentChanged(XRControl);
			} catch {
				transaction1.Cancel();
			} finally {
				transaction1.Commit();
			}
		}
		protected internal override void SetBindingFormatString(string propName, string formatString) {
			if(IsInplaceEditingMode) {
				MailMergeFieldInfo mailMergeFieldInfo = InplaceEditorHelper.GetMailMergeFieldInfoFromInplaceEditor(XRControl);
				if(mailMergeFieldInfo == null)
					return;
				ReplaceSelection(MailMergeFieldInfo.WrapColumnInfoInBrackets(mailMergeFieldInfo.DisplayName, EmbeddedFieldsHelper.GetClearFormatString(formatString)));
				return;
			}
			if(SetFormatStringCore(propName, formatString))
				return;
			if(EmbeddedFieldsHelper.HasEmbeddedFieldNames(XRControl)) {
				DesignerTransaction transaction = DesignerHost.CreateTransaction(XRComponentPropertyNames.Text);
				try {
					RaiseComponentChanging(XRControl, XRComponentPropertyNames.Text);
					EmbeddedFieldsHelper.ChangeFormatString(XRControl, formatString);
					RaiseComponentChanged(XRControl);
				} catch {
					transaction.Cancel();
				} finally {
					transaction.Commit();
				}
			}
		}
		protected virtual bool SetFormatStringCore(string propName, string formatString) {
			if(XRControl.DataBindings["Text"] != null) {
				base.SetBindingFormatString(propName, formatString);
				return true;
			}
			return false;
		}
		void SetVerbsState(bool enabled, params string[] verbs) {
			foreach(string verb in verbs) {
				foreach(DesignerVerb designerVerb in Verbs) {
					if(designerVerb.Text == verb)
						designerVerb.Enabled = enabled;
				}
			}
		}
		public override void OnComponentChanged(ComponentChangedEventArgs e) {
			string propertyName = e.Member != null ? e.Member.Name : "";
			if(propertyName == XRComponentPropertyNames.DataBindings)
				SetDefaultControlText();
			base.OnComponentChanged(e);
		}
		public override void CloseInplaceEditor(bool commit) {
			if(IsInplaceEditingMode && EmbeddedFieldsHelper.HasEmbeddedFieldNames(XRControl))
				RemoveTextBinding();
			base.CloseInplaceEditor(commit);
		}
		public override void ShowInplaceEditor(bool selectAll) {
			base.ShowInplaceEditor(selectAll);
			XRBinding binding = XRControl != null ? XRControl.DataBindings["Text"] : null;
			if(binding != null && XRControl.Text == XRControl.Site.Name) {
				ShowInplaceEditor(MailMergeFieldInfo.WrapColumnInfoInBrackets(binding.DisplayColumnName, string.Empty), selectAll);
				return;
			}
		}
		void RemoveTextBinding() {
			XRBinding binding = XRControl.DataBindings["Text"];
			if(binding == null)
				return;
			string displayDataMember = binding.DisplayColumnName;
			if(!MailMergeFieldInfo.WrapColumnInfoInBrackets(displayDataMember, string.Empty).Equals(XRControl.Text)
				&& !MailMergeFieldInfo.WrapColumnInfoInBrackets(binding.DisplayColumnName, string.Empty).Equals(XRControl.Text)) {
				DesignerTransaction transaction = DesignerHost.CreateTransaction(XRComponentPropertyNames.DataBindings);
				try {
					RaiseComponentChanging(XRControl, XRComponentPropertyNames.DataBindings);
					XRControl.DataBindings.Remove(binding);
					RaiseComponentChanged(XRControl);
				} catch {
					transaction.Cancel();
				} finally {
					transaction.Commit();
				}
			}
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			if(properties["Lines"] != null)
				properties["Lines"] = new LinesPropertyDescriptor((PropertyDescriptor)properties["Lines"]);
		}
		class LinesPropertyDescriptor : WrappedPropertyDescriptor {
			public LinesPropertyDescriptor(PropertyDescriptor oldPropertyDescriptor)
				: base(oldPropertyDescriptor) {
			}
			public override object GetValue(object component) {
				return XRConvert.StringToStringArray(((XRFieldEmbeddableControl)component).GetTextPropertyWithDisplayColumnNames());
			}
			public override void SetValue(object component, object value) {
				ISite site = MemberDescriptor.GetSite(component);
				if(!CanWrapProperty(site)) {
					oldPropertyDescriptor.SetValue(component, value);
					return;
				}
				IComponentChangeService service = site != null ? (IComponentChangeService)site.GetService(typeof(IComponentChangeService)) : null;
				XRControlDesignerBase.RaiseComponentChanging(service, (XRFieldEmbeddableControl)component, oldPropertyDescriptor.Name);
				((XRFieldEmbeddableControl)component).SetTextFromTextWithDisplayColumnNames(XRConvert.StringArrayToString((string[])value));
				XRControlDesignerBase.RaiseComponentChanged(service, (XRFieldEmbeddableControl)component, oldPropertyDescriptor.Name, null, ((XRFieldEmbeddableControl)component).Text);
			}
		}
	}
	abstract class WrappedPropertyDescriptor : PropertyDescriptor {
		protected PropertyDescriptor oldPropertyDescriptor;
		public WrappedPropertyDescriptor(PropertyDescriptor oldPropertyDescriptor)
			: base(oldPropertyDescriptor) {
			this.oldPropertyDescriptor = oldPropertyDescriptor;
		}
		public override bool CanResetValue(object component) {
			return oldPropertyDescriptor.CanResetValue(component);
		}
		public override void ResetValue(object component) {
			oldPropertyDescriptor.ResetValue(component);
		}
		public override Type ComponentType {
			get { return oldPropertyDescriptor.ComponentType; }
		}
		public override Type PropertyType {
			get { return oldPropertyDescriptor.PropertyType; }
		}
		public override bool IsReadOnly {
			get { return oldPropertyDescriptor.IsReadOnly; }
		}
		public override bool ShouldSerializeValue(object component) {
			return oldPropertyDescriptor.ShouldSerializeValue(component);
		}
		protected static bool CanWrapProperty(ISite site) {
			return site != null && site.DesignMode && !ReportDesigner.HostIsLoading(site);
		}
	}
	public class XRTextControlBaseDesigner : XRControlDesigner, IKeyTarget {
#if DEBUGTEST
		public InplaceTextEditorBase InplaceEditor { get { return fEditor; } }
#endif
		protected InplaceTextEditorBase fEditor;
		bool closeInplaceEditorOnDragLeave;
		public InplaceTextEditorBase Editor { get { return fEditor; } }
		public bool IsInplaceEditingMode { get { return fEditor != null; } }
		public bool CloseInplaceEditorOnDragLeave {
			get { return closeInplaceEditorOnDragLeave; }
			set { closeInplaceEditorOnDragLeave = value; }
		}
		internal protected new XRFieldEmbeddableControl XRControl {
			get { return Component as XRFieldEmbeddableControl; }
		}
		protected virtual InplaceTextEditorBase GetInplaceEditor(string text, bool selectAll) {
			return null;
		}
		public virtual RectangleF GetEditorScreenBounds() {
			IBandViewInfoService svc = (IBandViewInfoService)GetService(typeof(IBandViewInfoService));
			return svc.GetControlScreenBounds(XRControl);
		}
		protected internal virtual void ReplaceSelection(string newString) {
			int selectionStart = Editor.SelectionStart;
			Editor.SelectedText = newString;
			Editor.SetSelection(selectionStart, newString.Length);
		}
		public void OnEditText(object sender, EventArgs e) {
			ShowInplaceEditor(true);
		}
		public void ShowInplaceEditor(string text, bool selectAll) {
			if(Locked || IsInplaceEditingMode)
				return;
			if(selectionService.GetSelectedComponents().Count > 1)
				return;
			if(ResizeService.IsRunning)
				return;
			fEditor = GetInplaceEditor(text, selectAll);
			if(fEditor == null)
				return;
			ScrollablePanel scrollPanel = (ScrollablePanel)GetService(typeof(ScrollablePanel));
			if(scrollPanel != null) scrollPanel.SetScroll(false);
			if(!fEditor.Control.Parent.ContainsFocus)
				fEditor.Control.Parent.Focus();
			fEditor.Control.Focus();
			if(scrollPanel != null) scrollPanel.SetScroll(true);
			fEditor.Control.DragLeave += new EventHandler(OnDragLeave);
			BandDesigner bandDesigner = (BandDesigner)DesignerHost.GetDesigner(Band);
			System.Diagnostics.Debug.Assert(bandDesigner != null);
			MenuCommandHandler menuCommandHandler = (MenuCommandHandler)DesignerHost.GetService(typeof(MenuCommandHandler));
			menuCommandHandler.UpdateCommandStatus();
			closeInplaceEditorOnDragLeave = false;
		}
		public void ShowInplaceEditor(string text) {
			ShowInplaceEditor(text, true);
		}
		public virtual void ShowInplaceEditor(bool selectAll) {
			ShowInplaceEditor(string.Empty, selectAll);
		}
		void OnDragLeave(object sender, EventArgs e) {
			if(closeInplaceEditorOnDragLeave)
				CloseInplaceEditor(false);
		}
		public void HandleKeyPress(object sender, KeyPressEventArgs e) {
			if((int)e.KeyChar < 32) {
				e.Handled = false;
				return;
			}
			e.Handled = true;
			ShowInplaceEditor(new string(e.KeyChar, 1), false);
		}
		internal void CommitInplaceEditor() {
			CloseInplaceEditor(true);
		}
		public virtual void CloseInplaceEditor(bool commit) {
			if(fEditor != null) {
				fEditor.Control.DragLeave -= new EventHandler(OnDragLeave);
				fEditor.Close(commit);
				fEditor = null;
				MenuCommandHandler menuCommandHandler = (MenuCommandHandler)DesignerHost.GetService(typeof(MenuCommandHandler));
				menuCommandHandler.UpdateCommandStatus();
			}
		}
		protected override void OnDeactivated(object sender, EventArgs e) {
			base.OnDeactivated(sender, e);
			CommitInplaceEditor();
		}
		protected override void OnSelectionChanged(object sender, EventArgs e) {
			base.OnSelectionChanged(sender, e);
			CommitInplaceEditor();
		}
		public override void OnKeyCancel(CancelEventArgs e) {
			if(fEditor != null) {
				CloseInplaceEditor(false);
			} else
				e.Cancel = false;
		}
		public override void OnComponentChanged(ComponentChangedEventArgs e) {
			base.OnComponentChanged(e);
			string propertyName = e.Member != null ? e.Member.Name : "";
			if(propertyName == "Name" && object.Equals(XRControl.Text, e.OldValue))
				XRControl.Text = (string)e.NewValue;
			if(fEditor != null && IsInplaceEditingMode) {
				try {
					fEditor.UpdateProperties(propertyName);
				} catch { }
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				CloseInplaceEditor(false);
			}
			base.Dispose(disposing);
		}
	}
}
