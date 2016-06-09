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
using System.Drawing;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.ExpressApp.Win.Editors {
	public interface IInplaceEditSupport {
		RepositoryItem CreateRepositoryItem();
	}
	public abstract class DXPropertyEditor : WinPropertyEditor, IInplaceEditSupport, IAppearanceFormat {
		public static List<Type> RepositoryItemsTypesWithMandatoryButtons = new List<Type>();
		static DXPropertyEditor() {
			RepositoryItemsTypesWithMandatoryButtons.AddRange(
				new Type[] {
																		typeof(RepositoryItemImageEdit),
																		typeof(RepositoryItemMemoExEdit),
																		typeof(RepositoryItemRtfEditEx),
																		typeof(RepositoryItemObjectEdit)
																	});
		}
		private void Editor_EditValueChanged(object sender, EventArgs e) {
			if(!inReadValue && Control.IsModified && (Control.DataBindings.Count > 0)) {
				OnControlValueChanged();
			}
		}
		private void Control_Validating(object sender, CancelEventArgs e) {
			if(!String.IsNullOrEmpty(ErrorMessage) && Control != null) {
				Control.ErrorText = ErrorMessage;
				Control.ErrorIcon = ErrorIcon.Image;
			}
		}
		protected DXPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
			ControlBindingProperty = "EditValue";
		}
		protected virtual RepositoryItem CreateRepositoryItem() {
			return null;
		}
		protected virtual void SetRepositoryItemReadOnly(RepositoryItem item, bool readOnly) {
			RepositoryItemTextEdit textItem = item as RepositoryItemTextEdit;
			if((textItem != null) && IsPassword) {
				textItem.PasswordChar = '*';
			}
			item.ReadOnly = readOnly;
		}
		protected virtual void SetupRepositoryItem(RepositoryItem item) {
			SetRepositoryItemReadOnly(item, !AllowEdit);
			InitializeAppearance(item);
			OnCustomSetupRepositoryItem(new CustomSetupRepositoryItemEventArgs(item));
		}
		protected override void OnControlCreated() {
			Control.ErrorText = ErrorMessage;
			Control.ErrorIcon = ErrorIcon.Image;
			Control.EditValueChanged += new EventHandler(Editor_EditValueChanged);
			Control.Validating += new CancelEventHandler(Control_Validating);
			SetupRepositoryItem(Control.Properties);
			base.OnControlCreated();
		}
		protected virtual void InitializeAppearance(RepositoryItem item) { }
		protected virtual void OnCustomSetupRepositoryItem(CustomSetupRepositoryItemEventArgs args) {
			if(CustomSetupRepositoryItem != null) {
				CustomSetupRepositoryItem(this, args);
			}
		}
		protected override void OnAllowEditChanged() {
			if(Control != null) {
				SetRepositoryItemReadOnly(Control.Properties, !AllowEdit);
			}
			base.OnAllowEditChanged();
		}
		RepositoryItem IInplaceEditSupport.CreateRepositoryItem() {
			RepositoryItem item = CreateRepositoryItem();
			if(item != null) {
				SetupRepositoryItem(item);
			}
			return item;
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing && Control != null) {
					Control.EditValueChanged -= new EventHandler(Editor_EditValueChanged);
					Control.Validating -= new CancelEventHandler(Control_Validating);
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("DXPropertyEditorErrorMessage")]
#endif
		public override string ErrorMessage {
			get { return base.ErrorMessage; }
			set {
				if(Control != null) {
					Control.ErrorText = value;
				}
				base.ErrorMessage = value;
			}
		}
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("DXPropertyEditorErrorIcon")]
#endif
		public override ImageInfo ErrorIcon {
			get { return base.ErrorIcon; }
			set {
				if(Control != null) {
					Control.ErrorIcon = value.Image;
				}
				base.ErrorIcon = value;
			}
		}
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("DXPropertyEditorControl")]
#endif
		public new BaseEdit Control {
			get { return (BaseEdit)base.Control; }
		}
		public event EventHandler<CustomSetupRepositoryItemEventArgs> CustomSetupRepositoryItem;
		#region IAppearanceFormat Members
		FontStyle IAppearanceFormat.FontStyle {
			get {
				if(Control != null) {
					return Control.Font.Style;
				}
				return DevExpress.Utils.AppearanceObject.DefaultFont.Style;
			}
			set {
				if(Control != null) {
					Control.Font = new System.Drawing.Font(Control.Font, value);
				}
			}
		}
		Color IAppearanceFormat.FontColor {
			get {
				if(Control != null) {
					return Control.ForeColor;
				}
				return Color.Empty;
			}
			set {
				if(Control != null) {
					Control.ForeColor = value;
				}
			}
		}
		Color IAppearanceFormat.BackColor {
			get {
				if(Control != null) {
					return Control.BackColor;
				}
				return Color.Empty;
			}
			set {
				if(Control != null) {
					Control.BackColor = value;
				}
			}
		}
		void IAppearanceFormat.ResetBackColor() {
			if(Control != null) {
				Control.Properties.Appearance.BackColor = Color.Empty;
				Control.Properties.Appearance.BackColor2 = Color.Empty;
				Control.Properties.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
				Control.Properties.Appearance.Image = null;
				InitializeAppearance(Control.Properties);
			}
		}
		void IAppearanceFormat.ResetFontColor() {
			if(Control != null) {
				Control.Properties.Appearance.ForeColor = Color.Empty;
				Control.Properties.Appearance.BorderColor = Color.Empty;
				Control.Properties.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
				Control.Properties.Appearance.Image = null;
				InitializeAppearance(Control.Properties);
			}
		}
		void IAppearanceFormat.ResetFontStyle() {
			if(Control != null) {
				if(!DevExpress.Utils.AppearanceObject.DefaultFont.Equals(Control.Properties.Appearance.Font)) {
					Control.Properties.Appearance.Font = DevExpress.Utils.AppearanceObject.DefaultFont; 
				}
				Control.Properties.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
				Control.Properties.Appearance.Image = null;
				InitializeAppearance(Control.Properties);
			}
		}
		#endregion
	}
	public class CustomSetupRepositoryItemEventArgs : EventArgs {
		private RepositoryItem item;
		public CustomSetupRepositoryItemEventArgs(RepositoryItem item) {
			this.item = item;
		}
		public RepositoryItem Item {
			get { return item; }
		}
	}
	public class RepositoryEditorsFactory {
		private IObjectSpace objectSpace;
		private XafApplication application;
		private string protectedContentText;
		private MemberEditorInfoCalculator calculator;
		private Dictionary<Type, RepositoryItem> standaloneRepositoryItemByValueType;
		public string ProtectedContentText {
			get { return protectedContentText; }
			set { protectedContentText = value; }
		}
		public RepositoryEditorsFactory(XafApplication application, IObjectSpace objectSpace) {
			this.application = application;
			this.objectSpace = objectSpace;
			calculator = new MemberEditorInfoCalculator();
			standaloneRepositoryItemByValueType = new Dictionary<Type, RepositoryItem>();
		}
		public RepositoryItem CreateRepositoryItem(bool needProtectedContent, IModelMemberViewItem memberViewItem, Type objectType) {
			RepositoryItem result = null;
			if(application != null && application.EditorFactory != null) {
				ViewItem propertyEditorCandidate = null;
				if(!needProtectedContent && memberViewItem.PropertyEditorType == null) {
					propertyEditorCandidate = application.EditorFactory.CreatePropertyEditorByType(typeof(StringPropertyEditor), memberViewItem, objectType, application, objectSpace);
				}
				else {
					propertyEditorCandidate = application.EditorFactory.CreateDetailViewEditor(needProtectedContent, memberViewItem, objectType, application, objectSpace);
				}
				IInplaceEditSupport inplaceEditSupport = propertyEditorCandidate as IInplaceEditSupport;
				if(inplaceEditSupport == null) {
					Type editorType = calculator.GetEditorType(memberViewItem.ModelMember, typeof(IInplaceEditSupport));
					if(editorType != null) {
						inplaceEditSupport = application.EditorFactory.CreatePropertyEditorByType(editorType, memberViewItem, objectType, application, objectSpace) as IInplaceEditSupport;
					}
				}
				IProtectedContentEditor protectedContentEditor = inplaceEditSupport as IProtectedContentEditor;
				if(protectedContentEditor != null) {
					protectedContentEditor.ProtectedContentText = ProtectedContentText;
				}
				if(inplaceEditSupport != null) {
					result = inplaceEditSupport.CreateRepositoryItem();
				}
				if(propertyEditorCandidate != null) {
					propertyEditorCandidate.Dispose(); 
				}
			}
			return result;
		}
		public virtual RepositoryItem CreateStandaloneRepositoryItem(Type valueType) {
			Guard.ArgumentNotNull(valueType, "valueType");
			RepositoryItem item;
			if(!standaloneRepositoryItemByValueType.TryGetValue(valueType, out item)) {
				string uniqueParameterName = "StandaloneRepositoryItem_" + valueType.FullName.Replace('.', '_');
				IParameter xafParameter = new UpdatableParameter(uniqueParameterName, valueType);
				ParameterList parameterList = new ParameterList();
				parameterList.Add(xafParameter);
				ParametersObject.CreateBoundObject(parameterList);
				IModelDetailView detailViewModel = TempDetailViewHelper.CreateTempDetailViewModel(application.Model, typeof(ParametersObject));
				IModelMemberViewItem modelMemberViewItem = (IModelMemberViewItem)detailViewModel.Items[uniqueParameterName];
				item = CreateRepositoryItem(false, modelMemberViewItem, typeof(ParametersObject));
				if(item is RepositoryItemLookupEdit) {
					((RepositoryItemLookupEdit)item).ShowActionContainersPanel = false;
				}
				standaloneRepositoryItemByValueType.Add(valueType, item);
			}
			return item != null ? (RepositoryItem)item.Clone() : null;
		}
	}
}
