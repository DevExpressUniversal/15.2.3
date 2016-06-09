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
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Core;
namespace DevExpress.ExpressApp.Win.Editors {
	public abstract class WinPropertyEditor : PropertyEditor, ISupportToolTip {
		public const int TextControlHeight = 19;
		private BindingHelper bindingHelper;
		private string controlBindingProperty;
		private void Control_ParentChanged(object sender, EventArgs e) {
			ReadValue();
		}
		private void DataBindings_CollectionChanging(object sender, CollectionChangeEventArgs e) {
			if(e.Element != null) {
				if(e.Action == CollectionChangeAction.Add) {
					Binding binding = (Binding)e.Element;
					binding.Format += new ConvertEventHandler(binding_Format);
					binding.BindingComplete += new BindingCompleteEventHandler(binding_BindingComplete);
					binding.Parse += new ConvertEventHandler(binding_Parse);
				}
				if(e.Action == CollectionChangeAction.Remove) {
					Binding binding = (Binding)e.Element;
					binding.Format -= new ConvertEventHandler(binding_Format);
					binding.BindingComplete -= new BindingCompleteEventHandler(binding_BindingComplete);
					binding.Parse -= new ConvertEventHandler(binding_Parse);
				}
			}
		}
		private void binding_Format(object sender, ConvertEventArgs e) {
			OnFormatValue(e);
		}
		private void binding_Parse(object sender, ConvertEventArgs e) {
			OnValueStoring(e.Value);
		}
		private void binding_BindingComplete(object sender, BindingCompleteEventArgs e) {
			if(e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate
				&& e.BindingCompleteState == BindingCompleteState.Success) {
				OnValueStored();
			}
		}
		protected override void WriteValueCore() {
			BindingHelper.ForceWriteValue();
		}
		protected virtual void OnFormatValue(ConvertEventArgs e) { }
		protected override bool CanReadValue() {
			return base.CanReadValue() && bindingHelper != null;
		}
		protected override void SetTestTag() {
			Control.Tag = EasyTestTagHelper.FormatTestField(Caption);
		}
		protected override void OnControlCreated() {
			Control.ParentChanged += new EventHandler(Control_ParentChanged);
			bindingHelper = CreateBindingHelper();
			Control.DataBindings.CollectionChanging += new CollectionChangeEventHandler(DataBindings_CollectionChanging);
			((ISupportToolTip)this).SetToolTip(Control, Model);
			UpdateControlEnabled(AllowEdit);
			base.OnControlCreated();
		}
		protected virtual BindingHelper CreateBindingHelper() {
			return new BindingHelper(Control, ControlBindingProperty, MemberInfo, ImmediatePostData);
		}
		protected override void OnAllowEditChanged() {
			UpdateControlEnabled(AllowEdit);
			base.OnAllowEditChanged();
		}
		void ISupportToolTip.SetToolTip(object element, IModelToolTip model) {
			BaseControl baseControl = element as BaseControl;
			if(model != null && baseControl != null) {
				if(!string.IsNullOrEmpty(model.ToolTip)) {
					baseControl.ToolTip = model.ToolTip;
				}
				IModelToolTipOptions options = model as IModelToolTipOptions;
				if(options != null) {
					if(!string.IsNullOrEmpty(options.ToolTipTitle)) {
						baseControl.ToolTipTitle = options.ToolTipTitle;
					}
					if(options.ToolTipIconType != DevExpress.Utils.ToolTipIconType.None) {
						baseControl.ToolTipIconType = options.ToolTipIconType;
					}
				}
			}
		}
		protected override object GetControlValueCore() {
			if(bindingHelper != null) {
				return bindingHelper.GetControlValue();
			}
			else {
				return null;
			}
		}
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			if(Control != null) {
				Control.ParentChanged -= new EventHandler(Control_ParentChanged);
				Control.DataBindings.Clear();
				Control.DataBindings.CollectionChanging -= new CollectionChangeEventHandler(DataBindings_CollectionChanging);
			}
			base.BreakLinksToControl(unwireEventsOnly);
		}
		protected BindingHelper BindingHelper {
			get { return bindingHelper; }
		}
		protected WinPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
		protected override void ReadValueCore() {
			bindingHelper.RefreshBinding(CurrentObject);
			AllowEdit.SetItemValue("Bindings", Control.DataBindings.Count > 0);
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(Control != null) {
						Control.Controls.Clear();
					}
					if(bindingHelper != null) {
						bindingHelper.Dispose();
						bindingHelper = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected virtual void UpdateControlEnabled(bool enabled) {
			if((Control != null) && CanUpdateControlEnabled) {
				Control.Enabled = enabled;
			}
		}
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("WinPropertyEditorControl")]
#endif
		public new Control Control {
			get { return (Control)base.Control; }
		}
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("WinPropertyEditorControlBindingProperty")]
#endif
		public string ControlBindingProperty {
			get { return controlBindingProperty; }
			set { controlBindingProperty = value; }
		}
		protected bool CanUpdateControlEnabled { get; set; }
	}
}
