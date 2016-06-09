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

namespace DevExpress.Web.ASPxScheduler.Forms.Internal {
	[System.CLSCompliantAttribute(false)]
	public partial class AppointmentForm {
		protected global::DevExpress.Web.ASPxLabel lblSubject;
		protected global::DevExpress.Web.ASPxTextBox tbSubject;
		protected global::DevExpress.Web.ASPxLabel lblLocation;
		protected global::DevExpress.Web.ASPxTextBox tbLocation;
		protected global::DevExpress.Web.ASPxLabel lblLabel;
		protected global::DevExpress.Web.ASPxComboBox edtLabel;
		protected global::DevExpress.Web.ASPxLabel lblStartDate;
		protected global::DevExpress.Web.ASPxDateEdit edtStartDate;
		protected global::DevExpress.Web.ASPxTimeEdit edtStartTime;
		protected global::DevExpress.Web.ASPxLabel lblEndDate;
		protected global::DevExpress.Web.ASPxDateEdit edtEndDate;
		protected global::DevExpress.Web.ASPxTimeEdit edtEndTime;
		protected global::DevExpress.Web.ASPxLabel lblStatus;
		protected global::DevExpress.Web.ASPxComboBox edtStatus;
		protected global::DevExpress.Web.ASPxCheckBox chkAllDay;
		protected global::DevExpress.Web.ASPxLabel lblAllDay;
		protected global::DevExpress.Web.ASPxLabel lblResource;
		protected global::DevExpress.Web.ASPxDropDownEdit ddResource;
		protected global::DevExpress.Web.ASPxComboBox edtResource;
		protected global::DevExpress.Web.ASPxCheckBox chkReminder;
		protected global::DevExpress.Web.ASPxLabel lblReminder;
		protected global::DevExpress.Web.ASPxComboBox cbReminder;
		protected global::DevExpress.Web.ASPxMemo tbDescription;
		protected global::System.Web.UI.HtmlControls.HtmlGenericControl ValidationContainer;
		protected global::DevExpress.Web.ASPxScheduler.Controls.AppointmentRecurrenceForm AppointmentRecurrenceForm1;
		protected global::DevExpress.Web.ASPxButton btnOk;
		protected global::DevExpress.Web.ASPxButton btnCancel;
		protected global::DevExpress.Web.ASPxButton btnDelete;
		protected global::DevExpress.Web.ASPxScheduler.Controls.ASPxSchedulerStatusInfo schedulerStatusInfo;
		private static bool @__initialized;
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public AppointmentForm() {
			((global::System.Web.UI.UserControl)(this)).AppRelativeVirtualPath = "~/AppointmentForm.ascx";
			if ((global::DevExpress.Web.ASPxScheduler.Forms.Internal.AppointmentForm.@__initialized == false)) {
				global::DevExpress.Web.ASPxScheduler.Forms.Internal.AppointmentForm.@__initialized = true;
			}
		}
		protected System.Web.Profile.DefaultProfile Profile {
			get {
				return ((System.Web.Profile.DefaultProfile)(this.Context.Profile));
			}
		}
		protected System.Web.HttpApplication ApplicationInstance {
			get {
				return ((System.Web.HttpApplication)(this.Context.ApplicationInstance));
			}
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxLabel @__BuildControllblSubject() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblSubject = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "lblSubject";
			@__ctrl.AssociatedControlID = "tbSubject";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxTextBox @__BuildControltbSubject() {
			global::DevExpress.Web.ASPxTextBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxTextBox();
			this.tbSubject = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.ID = "tbSubject";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingtbSubject);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingtbSubject(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxTextBox dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxTextBox)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.Text = global::System.Convert.ToString( ((AppointmentFormTemplateContainer)Container).Subject , global::System.Globalization.CultureInfo.CurrentCulture);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxLabel @__BuildControllblLocation() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblLocation = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "lblLocation";
			@__ctrl.AssociatedControlID = "tbLocation";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxTextBox @__BuildControltbLocation() {
			global::DevExpress.Web.ASPxTextBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxTextBox();
			this.tbLocation = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.ID = "tbLocation";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingtbLocation);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingtbLocation(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxTextBox dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxTextBox)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.Text = global::System.Convert.ToString( ((AppointmentFormTemplateContainer)Container).Appointment.Location , global::System.Globalization.CultureInfo.CurrentCulture);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxLabel @__BuildControllblLabel() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblLabel = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "lblLabel";
			@__ctrl.AssociatedControlID = "edtLabel";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxComboBox @__BuildControledtLabel() {
			global::DevExpress.Web.ASPxComboBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxComboBox();
			this.edtLabel = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.ID = "edtLabel";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.ValueType = typeof(int);
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingedtLabel);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingedtLabel(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxComboBox dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxComboBox)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.DataSource = ((object)( ((AppointmentFormTemplateContainer)Container).LabelDataSource ));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxLabel @__BuildControllblStartDate() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblStartDate = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "lblStartDate";
			@__ctrl.AssociatedControlID = "edtStartDate";
			@__ctrl.Wrap = global::DevExpress.Utils.DefaultBoolean.False;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control2(DevExpress.Web.ValidationSettings @__ctrl) {
			@__ctrl.ErrorDisplayMode = global::DevExpress.Web.ErrorDisplayMode.ImageWithTooltip;
			@__ctrl.ValidateOnLeave = false;
			@__ctrl.EnableCustomValidation = true;
			@__ctrl.Display = global::DevExpress.Web.Display.Dynamic;
			@__ctrl.ValidationGroup = "DateValidatoinGroup";
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxDateEdit @__BuildControledtStartDate() {
			global::DevExpress.Web.ASPxDateEdit @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxDateEdit();
			this.edtStartDate = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "edtStartDate";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.EditFormat = global::DevExpress.Web.EditFormat.Date;
			@__ctrl.DateOnError = global::DevExpress.Web.DateOnError.Undo;
			@__ctrl.AllowNull = false;
			@__ctrl.EnableClientSideAPI = true;
			this.@__BuildControl__control2(@__ctrl.ValidationSettings);
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingedtStartDate);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingedtStartDate(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxDateEdit dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxDateEdit)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.Date = ((System.DateTime)( ((AppointmentFormTemplateContainer)Container).Start ));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control3(DevExpress.Web.ValidationSettings @__ctrl) {
			@__ctrl.ErrorDisplayMode = global::DevExpress.Web.ErrorDisplayMode.ImageWithTooltip;
			@__ctrl.ValidateOnLeave = false;
			@__ctrl.EnableCustomValidation = true;
			@__ctrl.Display = global::DevExpress.Web.Display.Dynamic;
			@__ctrl.ValidationGroup = "DateValidatoinGroup";
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxTimeEdit @__BuildControledtStartTime() {
			global::DevExpress.Web.ASPxTimeEdit @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxTimeEdit();
			this.edtStartTime = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "edtStartTime";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("DateOnError", "Undo");
			@__ctrl.AllowNull = false;
			@__ctrl.EnableClientSideAPI = true;
			this.@__BuildControl__control3(@__ctrl.ValidationSettings);
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingedtStartTime);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingedtStartTime(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxTimeEdit dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxTimeEdit)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.DateTime = ((System.DateTime)( ((AppointmentFormTemplateContainer)Container).Start ));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxLabel @__BuildControllblEndDate() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblEndDate = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "lblEndDate";
			@__ctrl.Wrap = global::DevExpress.Utils.DefaultBoolean.False;
			@__ctrl.AssociatedControlID = "edtEndDate";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control4(DevExpress.Web.ValidationSettings @__ctrl) {
			@__ctrl.ErrorDisplayMode = global::DevExpress.Web.ErrorDisplayMode.ImageWithTooltip;
			@__ctrl.ValidateOnLeave = false;
			@__ctrl.EnableCustomValidation = true;
			@__ctrl.Display = global::DevExpress.Web.Display.Dynamic;
			@__ctrl.ValidationGroup = "DateValidatoinGroup";
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxDateEdit @__BuildControledtEndDate() {
			global::DevExpress.Web.ASPxDateEdit @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxDateEdit();
			this.edtEndDate = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "edtEndDate";
			@__ctrl.EditFormat = global::DevExpress.Web.EditFormat.Date;
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.DateOnError = global::DevExpress.Web.DateOnError.Undo;
			@__ctrl.AllowNull = false;
			@__ctrl.EnableClientSideAPI = true;
			this.@__BuildControl__control4(@__ctrl.ValidationSettings);
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingedtEndDate);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingedtEndDate(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxDateEdit dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxDateEdit)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.Date = ((System.DateTime)( ((AppointmentFormTemplateContainer)Container).End ));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control5(DevExpress.Web.ValidationSettings @__ctrl) {
			@__ctrl.ErrorDisplayMode = global::DevExpress.Web.ErrorDisplayMode.ImageWithTooltip;
			@__ctrl.ValidateOnLeave = false;
			@__ctrl.EnableCustomValidation = true;
			@__ctrl.Display = global::DevExpress.Web.Display.Dynamic;
			@__ctrl.ValidationGroup = "DateValidatoinGroup";
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxTimeEdit @__BuildControledtEndTime() {
			global::DevExpress.Web.ASPxTimeEdit @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxTimeEdit();
			this.edtEndTime = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "edtEndTime";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("DateOnError", "Undo");
			@__ctrl.AllowNull = false;
			@__ctrl.EnableClientSideAPI = true;
			@__ctrl.HelpTextSettings.PopupMargins.MarginLeft = new System.Web.UI.WebControls.Unit(50D, global::System.Web.UI.WebControls.UnitType.Pixel);
			this.@__BuildControl__control5(@__ctrl.ValidationSettings);
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingedtEndTime);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingedtEndTime(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxTimeEdit dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxTimeEdit)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.DateTime = ((System.DateTime)( ((AppointmentFormTemplateContainer)Container).End ));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxLabel @__BuildControllblStatus() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblStatus = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "lblStatus";
			@__ctrl.AssociatedControlID = "edtStatus";
			@__ctrl.Wrap = global::DevExpress.Utils.DefaultBoolean.False;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxComboBox @__BuildControledtStatus() {
			global::DevExpress.Web.ASPxComboBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxComboBox();
			this.edtStatus = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.ID = "edtStatus";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.ValueType = typeof(int);
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingedtStatus);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingedtStatus(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxComboBox dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxComboBox)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.DataSource = ((object)( ((AppointmentFormTemplateContainer)Container).StatusDataSource ));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxCheckBox @__BuildControlchkAllDay() {
			global::DevExpress.Web.ASPxCheckBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxCheckBox();
			this.chkAllDay = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.ID = "chkAllDay";
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingchkAllDay);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingchkAllDay(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxCheckBox dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxCheckBox)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.Checked = ((bool)( ((AppointmentFormTemplateContainer)Container).Appointment.AllDay ));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxLabel @__BuildControllblAllDay() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblAllDay = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "lblAllDay";
			@__ctrl.AssociatedControlID = "chkAllDay";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxLabel @__BuildControllblResource() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblResource = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "lblResource";
			@__ctrl.AssociatedControlID = "edtResource";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxListBox @__BuildControl__control7() {
			global::DevExpress.Web.ASPxListBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxListBox();
			@__ctrl.TemplateControl = this;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "edtMultiResource";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.SelectionMode = global::DevExpress.Web.ListEditSelectionMode.CheckColumn;
			@__ctrl.Border.BorderWidth = new System.Web.UI.WebControls.Unit(0D, global::System.Web.UI.WebControls.UnitType.Pixel);
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBinding__control7);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBinding__control7(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxListBox dataBindingExpressionBuilderTarget;
			DevExpress.Web.TemplateContainerBase Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxListBox)(sender));
			Container = ((DevExpress.Web.TemplateContainerBase)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.DataSource = ((object)( ResourceDataSource ));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control6(System.Web.UI.Control @__ctrl) {
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                "));
			global::DevExpress.Web.ASPxListBox @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control7();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                            "));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxDropDownEdit @__BuildControlddResource() {
			global::DevExpress.Web.ASPxDropDownEdit @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxDropDownEdit();
			this.ddResource = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.DropDownWindowTemplate = new System.Web.UI.CompiledTemplateBuilder(new System.Web.UI.BuildTemplateMethod(this.@__BuildControl__control6));
			@__ctrl.ID = "ddResource";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.ClientInstanceName = "ddResource";
			@__ctrl.AllowUserInput = false;
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingddResource);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingddResource(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxDropDownEdit dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxDropDownEdit)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.Enabled = ((bool)( ((AppointmentFormTemplateContainer)Container).CanEditResource ));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxComboBox @__BuildControledtResource() {
			global::DevExpress.Web.ASPxComboBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxComboBox();
			this.edtResource = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.ID = "edtResource";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingedtResource);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingedtResource(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxComboBox dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxComboBox)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.DataSource = ((object)( ResourceDataSource ));
			dataBindingExpressionBuilderTarget.Enabled = ((bool)( ((AppointmentFormTemplateContainer)Container).CanEditResource ));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxCheckBox @__BuildControlchkReminder() {
			global::DevExpress.Web.ASPxCheckBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxCheckBox();
			this.chkReminder = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "chkReminder";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxLabel @__BuildControllblReminder() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblReminder = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "lblReminder";
			@__ctrl.AssociatedControlID = "chkReminder";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxComboBox @__BuildControlcbReminder() {
			global::DevExpress.Web.ASPxComboBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxComboBox();
			this.cbReminder = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "cbReminder";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingcbReminder);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingcbReminder(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxComboBox dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxComboBox)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.DataSource = ((object)( ((AppointmentFormTemplateContainer)Container).ReminderDataSource ));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxMemo @__BuildControltbDescription() {
			global::DevExpress.Web.ASPxMemo @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxMemo();
			this.tbDescription = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.ID = "tbDescription";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.Rows = 6;
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingtbDescription);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingtbDescription(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxMemo dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxMemo)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.Text = global::System.Convert.ToString( ((AppointmentFormTemplateContainer)Container).Appointment.Description , global::System.Globalization.CultureInfo.CurrentCulture);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlGenericControl @__BuildControlValidationContainer() {
			global::System.Web.UI.HtmlControls.HtmlGenericControl @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlGenericControl("div");
			this.ValidationContainer = @__ctrl;
			@__ctrl.ID = "ValidationContainer";
			global::DevExpress.Web.ASPxLabel @__ctrl1;
			@__ctrl1 = this.@__BuildControllblSubject();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			global::DevExpress.Web.ASPxTextBox @__ctrl2;
			@__ctrl2 = this.@__BuildControltbSubject();
			@__parser.AddParsedSubObject(@__ctrl2);
			global::DevExpress.Web.ASPxLabel @__ctrl3;
			@__ctrl3 = this.@__BuildControllblLocation();
			@__parser.AddParsedSubObject(@__ctrl3);
			global::DevExpress.Web.ASPxTextBox @__ctrl4;
			@__ctrl4 = this.@__BuildControltbLocation();
			@__parser.AddParsedSubObject(@__ctrl4);
			global::DevExpress.Web.ASPxLabel @__ctrl5;
			@__ctrl5 = this.@__BuildControllblLabel();
			@__parser.AddParsedSubObject(@__ctrl5);
			global::DevExpress.Web.ASPxComboBox @__ctrl6;
			@__ctrl6 = this.@__BuildControledtLabel();
			@__parser.AddParsedSubObject(@__ctrl6);
			global::DevExpress.Web.ASPxLabel @__ctrl7;
			@__ctrl7 = this.@__BuildControllblStartDate();
			@__parser.AddParsedSubObject(@__ctrl7);
			global::DevExpress.Web.ASPxDateEdit @__ctrl8;
			@__ctrl8 = this.@__BuildControledtStartDate();
			@__parser.AddParsedSubObject(@__ctrl8);
			global::DevExpress.Web.ASPxTimeEdit @__ctrl9;
			@__ctrl9 = this.@__BuildControledtStartTime();
			@__parser.AddParsedSubObject(@__ctrl9);
			global::DevExpress.Web.ASPxLabel @__ctrl10;
			@__ctrl10 = this.@__BuildControllblEndDate();
			@__parser.AddParsedSubObject(@__ctrl10);
			global::DevExpress.Web.ASPxDateEdit @__ctrl11;
			@__ctrl11 = this.@__BuildControledtEndDate();
			@__parser.AddParsedSubObject(@__ctrl11);
			global::DevExpress.Web.ASPxTimeEdit @__ctrl12;
			@__ctrl12 = this.@__BuildControledtEndTime();
			@__parser.AddParsedSubObject(@__ctrl12);
			global::DevExpress.Web.ASPxLabel @__ctrl13;
			@__ctrl13 = this.@__BuildControllblStatus();
			@__parser.AddParsedSubObject(@__ctrl13);
			global::DevExpress.Web.ASPxComboBox @__ctrl14;
			@__ctrl14 = this.@__BuildControledtStatus();
			@__parser.AddParsedSubObject(@__ctrl14);
			global::DevExpress.Web.ASPxCheckBox @__ctrl15;
			@__ctrl15 = this.@__BuildControlchkAllDay();
			@__parser.AddParsedSubObject(@__ctrl15);
			global::DevExpress.Web.ASPxLabel @__ctrl16;
			@__ctrl16 = this.@__BuildControllblAllDay();
			@__parser.AddParsedSubObject(@__ctrl16);
			global::DevExpress.Web.ASPxLabel @__ctrl17;
			@__ctrl17 = this.@__BuildControllblResource();
			@__parser.AddParsedSubObject(@__ctrl17);
			global::DevExpress.Web.ASPxDropDownEdit @__ctrl18;
			@__ctrl18 = this.@__BuildControlddResource();
			@__parser.AddParsedSubObject(@__ctrl18);
			global::DevExpress.Web.ASPxComboBox @__ctrl19;
			@__ctrl19 = this.@__BuildControledtResource();
			@__parser.AddParsedSubObject(@__ctrl19);
			global::DevExpress.Web.ASPxCheckBox @__ctrl20;
			@__ctrl20 = this.@__BuildControlchkReminder();
			@__parser.AddParsedSubObject(@__ctrl20);
			global::DevExpress.Web.ASPxLabel @__ctrl21;
			@__ctrl21 = this.@__BuildControllblReminder();
			@__parser.AddParsedSubObject(@__ctrl21);
			global::DevExpress.Web.ASPxComboBox @__ctrl22;
			@__ctrl22 = this.@__BuildControlcbReminder();
			@__parser.AddParsedSubObject(@__ctrl22);
			global::DevExpress.Web.ASPxMemo @__ctrl23;
			@__ctrl23 = this.@__BuildControltbDescription();
			@__parser.AddParsedSubObject(@__ctrl23);
			@__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__RenderValidationContainer));
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__RenderValidationContainer(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
			@__w.Write("\r\n    <table class=\"dxscAppointmentForm\" ");
							   @__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(" style=\"width: 100%; height: 230px;\">\r\n    <tr>\r\n        <td class=\"dxscDoubleCel" +
					"l\" colspan=\"2\">\r\n            <table class=\"dxscLabelControlPair\" ");
										@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(">\r\n                <tr>\r\n                    <td class=\"dxscLabelCell\">\r\n        " +
					"                ");
			parameterContainer.Controls[0].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxscControlCell\">\r\n  " +
					"                      ");
			parameterContainer.Controls[1].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                </tr>\r\n            </table>\r\n       " +
					" </td>\r\n    </tr>\r\n    <tr> \r\n        <td class=\"dxscSingleCell\">\r\n            <" +
					"table class=\"dxscLabelControlPair\" ");
										@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(">\r\n                <tr>\r\n                    <td class=\"dxscLabelCell\">\r\n        " +
					"                ");
			parameterContainer.Controls[2].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxscControlCell\">\r\n  " +
					"                      ");
			parameterContainer.Controls[3].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                </tr>\r\n            </table>\r\n       " +
					" </td>\r\n        <td class=\"dxscSingleCell\">\r\n            <table class=\"dxscLabel" +
					"ControlPair\" ");
										@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(">\r\n                <tr>\r\n                    <td class=\"dxscLabelCell\" style=\"pad" +
					"ding-left: 25px;\">\r\n                        ");
			parameterContainer.Controls[4].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxscControlCell\">\r\n  " +
					"                      ");
			parameterContainer.Controls[5].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                </tr>\r\n            </table>\r\n       " +
					" </td>\r\n    </tr>\r\n    <tr>\r\n        <td class=\"dxscSingleCell\">\r\n            <t" +
					"able class=\"dxscLabelControlPair\" ");
										@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(">\r\n                <tr>\r\n                    <td class=\"dxscLabelCell\">\r\n        " +
					"                ");
			parameterContainer.Controls[6].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxscControlCell\">\r\n  " +
					"                      ");
			parameterContainer.Controls[7].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxscControlCell\" id=\"" +
					"edtStartTimeLayoutRoot\" style=\"padding-left: 5px;\">\r\n                        ");
			parameterContainer.Controls[8].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                </tr>\r\n            </table>\r\n       " +
					" </td>\r\n        <td class=\"dxscSingleCell\">\r\n            <table class=\"dxscLabel" +
					"ControlPair\" ");
										@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(">\r\n                <tr>\r\n                    <td class=\"dxscLabelCell\" style=\"pad" +
					"ding-left: 25px;\">\r\n                        ");
			parameterContainer.Controls[9].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxscControlCell\">\r\n  " +
					"                      ");
			parameterContainer.Controls[10].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxscControlCell\" id=\"" +
					"edtEndTimeLayoutRoot\" style=\"padding-left: 5px;\">\r\n                        ");
			parameterContainer.Controls[11].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                </tr>\r\n            </table>\r\n       " +
					" </td>\r\n    </tr>\r\n    <tr>\r\n        <td class=\"dxscSingleCell\">\r\n            <t" +
					"able class=\"dxscLabelControlPair\" ");
										@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(">\r\n                <tr>\r\n                    <td class=\"dxscLabelCell\">\r\n        " +
					"                ");
			parameterContainer.Controls[12].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxscControlCell\">\r\n  " +
					"                      ");
			parameterContainer.Controls[13].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                </tr>\r\n            </table>\r\n       " +
					" </td>\r\n        <td class=\"dxscSingleCell\" style=\"padding-left: 22px;\">\r\n       " +
					"     <table class=\"dxscLabelControlPair\" ");
										@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(">\r\n                <tr>\r\n                    <td style=\"width: 20px; height: 20px" +
					";\">\r\n                        ");
			parameterContainer.Controls[14].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td style=\"padding-left: 2px;\">\r" +
					"\n                        ");
			parameterContainer.Controls[15].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                </tr>\r\n            </table>\r\n       " +
					" </td>\r\n    </tr>\r\n    <tr>\r\n");
   if(CanShowReminders) { 
			@__w.Write("\r\n        <td class=\"dxscSingleCell\">\r\n");
   } else { 
			@__w.Write("\r\n        <td class=\"dxscDoubleCell\" colspan=\"2\">\r\n");
   } 
			@__w.Write("\r\n            <table class=\"dxscLabelControlPair\" ");
										@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(">\r\n                <tr>\r\n                    <td class=\"dxscLabelCell\">\r\n        " +
					"                ");
			parameterContainer.Controls[16].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxscControlCell\">\r\n");
   if(ResourceSharing) { 
			parameterContainer.Controls[17].RenderControl(@__w);
			@__w.Write("                        \r\n");
   } else { 
			parameterContainer.Controls[18].RenderControl(@__w);
			@__w.Write("\r\n");
   } 
			@__w.Write("             \r\n                    </td>\r\n\r\n                </tr>\r\n            </" +
					"table>\r\n        </td>\r\n");
   if(CanShowReminders) { 
			@__w.Write("\r\n        <td class=\"dxscSingleCell\">\r\n            <table class=\"dxscLabelControl" +
					"Pair\" ");
										@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(">\r\n                <tr>\r\n                    <td class=\"dxscLabelCell\" style=\"pad" +
					"ding-left: 22px;\">\r\n                        <table class=\"dxscLabelControlPair\" " +
					"");
													@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(">\r\n                            <tr>\r\n                                <td style=\"w" +
					"idth: 20px; height: 20px;\">\r\n                                    ");
			parameterContainer.Controls[19].RenderControl(@__w);
			@__w.Write("\r\n                                </td>\r\n                                <td styl" +
					"e=\"padding-left: 2px;\">\r\n                                    ");
			parameterContainer.Controls[20].RenderControl(@__w);
			@__w.Write("\r\n                                </td>\r\n                            </tr>\r\n     " +
					"                   </table>\r\n                    </td>\r\n                    <td " +
					"class=\"dxscControlCell\" style=\"padding-left: 3px\">\r\n                        ");
			parameterContainer.Controls[21].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                </tr>\r\n            </table>\r\n       " +
					" </td>\r\n");
   } 
			@__w.Write("\r\n    </tr>\r\n    <tr>\r\n        <td class=\"dxscDoubleCell\" colspan=\"2\" style=\"heig" +
					"ht: 90px;\">\r\n            ");
			parameterContainer.Controls[22].RenderControl(@__w);
			@__w.Write("\r\n        </td>\r\n    </tr>\r\n</table>\r\n");
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxScheduler.Controls.AppointmentRecurrenceForm @__BuildControlAppointmentRecurrenceForm1() {
			global::DevExpress.Web.ASPxScheduler.Controls.AppointmentRecurrenceForm @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxScheduler.Controls.AppointmentRecurrenceForm();
			this.AppointmentRecurrenceForm1 = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "AppointmentRecurrenceForm1";
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingAppointmentRecurrenceForm1);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingAppointmentRecurrenceForm1(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxScheduler.Controls.AppointmentRecurrenceForm dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxScheduler.Controls.AppointmentRecurrenceForm)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.IsRecurring = ((bool)( ((AppointmentFormTemplateContainer)Container).Appointment.IsRecurring ));
			dataBindingExpressionBuilderTarget.DayNumber = ((int)( ((AppointmentFormTemplateContainer)Container).RecurrenceDayNumber ));
			dataBindingExpressionBuilderTarget.End = ((System.DateTime)( ((AppointmentFormTemplateContainer)Container).RecurrenceEnd ));
			dataBindingExpressionBuilderTarget.Month = ((int)( ((AppointmentFormTemplateContainer)Container).RecurrenceMonth ));
			dataBindingExpressionBuilderTarget.OccurrenceCount = ((int)( ((AppointmentFormTemplateContainer)Container).RecurrenceOccurrenceCount ));
			dataBindingExpressionBuilderTarget.Periodicity = ((int)( ((AppointmentFormTemplateContainer)Container).RecurrencePeriodicity ));
			dataBindingExpressionBuilderTarget.RecurrenceRange = ((DevExpress.XtraScheduler.RecurrenceRange)( ((AppointmentFormTemplateContainer)Container).RecurrenceRange ));
			dataBindingExpressionBuilderTarget.Start = ((System.DateTime)( ((AppointmentFormTemplateContainer)Container).RecurrenceStart ));
			dataBindingExpressionBuilderTarget.WeekDays = ((DevExpress.XtraScheduler.WeekDays)( ((AppointmentFormTemplateContainer)Container).RecurrenceWeekDays ));
			dataBindingExpressionBuilderTarget.WeekOfMonth = ((DevExpress.XtraScheduler.WeekOfMonth)( ((AppointmentFormTemplateContainer)Container).RecurrenceWeekOfMonth ));
			dataBindingExpressionBuilderTarget.RecurrenceType = ((DevExpress.XtraScheduler.RecurrenceType)( ((AppointmentFormTemplateContainer)Container).RecurrenceType ));
			dataBindingExpressionBuilderTarget.IsFormRecreated = ((bool)( ((AppointmentFormTemplateContainer)Container).IsFormRecreated ));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbtnOk() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.btnOk = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "btnOk";
			@__ctrl.UseSubmitBehavior = false;
			@__ctrl.AutoPostBack = false;
			@__ctrl.EnableViewState = false;
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(91D, global::System.Web.UI.WebControls.UnitType.Pixel);
			@__ctrl.EnableClientSideAPI = true;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbtnCancel() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.btnCancel = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "btnCancel";
			@__ctrl.UseSubmitBehavior = false;
			@__ctrl.AutoPostBack = false;
			@__ctrl.EnableViewState = false;
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(91D, global::System.Web.UI.WebControls.UnitType.Pixel);
			@__ctrl.CausesValidation = false;
			@__ctrl.EnableClientSideAPI = true;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbtnDelete() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.btnDelete = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "btnDelete";
			@__ctrl.UseSubmitBehavior = false;
			@__ctrl.AutoPostBack = false;
			@__ctrl.EnableViewState = false;
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(91D, global::System.Web.UI.WebControls.UnitType.Pixel);
			@__ctrl.CausesValidation = false;
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingbtnDelete);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingbtnDelete(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxButton dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxButton)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.Enabled = ((bool)( ((AppointmentFormTemplateContainer)Container).CanDeleteAppointment ));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxScheduler.Controls.ASPxSchedulerStatusInfo @__BuildControlschedulerStatusInfo() {
			global::DevExpress.Web.ASPxScheduler.Controls.ASPxSchedulerStatusInfo @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxScheduler.Controls.ASPxSchedulerStatusInfo();
			this.schedulerStatusInfo = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "schedulerStatusInfo";
			@__ctrl.Priority = 1;
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingschedulerStatusInfo);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingschedulerStatusInfo(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxScheduler.Controls.ASPxSchedulerStatusInfo dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxScheduler.Controls.ASPxSchedulerStatusInfo)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.MasterControlID = global::System.Convert.ToString( ((DevExpress.Web.ASPxScheduler.AppointmentFormTemplateContainer)Container).ControlId , global::System.Globalization.CultureInfo.CurrentCulture);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControlTree(AppointmentForm @__ctrl) {
			global::System.Web.UI.HtmlControls.HtmlGenericControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlValidationContainer();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			global::DevExpress.Web.ASPxScheduler.Controls.AppointmentRecurrenceForm @__ctrl2;
			@__ctrl2 = this.@__BuildControlAppointmentRecurrenceForm1();
			@__parser.AddParsedSubObject(@__ctrl2);
			global::DevExpress.Web.ASPxButton @__ctrl3;
			@__ctrl3 = this.@__BuildControlbtnOk();
			@__parser.AddParsedSubObject(@__ctrl3);
			global::DevExpress.Web.ASPxButton @__ctrl4;
			@__ctrl4 = this.@__BuildControlbtnCancel();
			@__parser.AddParsedSubObject(@__ctrl4);
			global::DevExpress.Web.ASPxButton @__ctrl5;
			@__ctrl5 = this.@__BuildControlbtnDelete();
			@__parser.AddParsedSubObject(@__ctrl5);
			global::DevExpress.Web.ASPxScheduler.Controls.ASPxSchedulerStatusInfo @__ctrl6;
			@__ctrl6 = this.@__BuildControlschedulerStatusInfo();
			@__parser.AddParsedSubObject(@__ctrl6);
			@__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__Render__control1));
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__Render__control1(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
			parameterContainer.Controls[0].RenderControl(@__w);
			@__w.Write("\r\n                        \r\n");
			parameterContainer.Controls[1].RenderControl(@__w);
			@__w.Write("\r\n                   \r\n<table ");
@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(" style=\"width: 100%; height: 35px;\">\r\n    <tr>\r\n        <td class=\"dx-ac\" style=\"" +
					"width: 100%; height: 100%;\" ");
													 @__w.Write( DevExpress.Web.Internal.RenderUtils.GetAlignAttributes(this, "center", null) );
			@__w.Write(">\r\n            <table class=\"dxscButtonTable\" style=\"height: 100%\">\r\n            " +
					"    <tr>\r\n                    <td class=\"dxscCellWithPadding\">\r\n                " +
					"        ");
			parameterContainer.Controls[2].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxscCellWithPadding\">" +
					"\r\n                        ");
			parameterContainer.Controls[3].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxscCellWithPadding\">" +
					"\r\n                        ");
			parameterContainer.Controls[4].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                </tr>\r\n            </table>\r\n       " +
					" </td>\r\n    </tr>\r\n</table>\r\n<table ");
@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(" style=\"width: 100%;\">\r\n    <tr>\r\n        <td class=\"dx-al\" style=\"width: 100%;\" " +
					" ");
										@__w.Write( DevExpress.Web.Internal.RenderUtils.GetAlignAttributes(this, "left", null) );
			@__w.Write(">\r\n            ");
			parameterContainer.Controls[5].RenderControl(@__w);
			@__w.Write("\r\n        </td>\r\n    </tr>\r\n</table>\r\n<script id=\"dxss_ASPxSchedulerAppoinmentFor" +
					"m\" type=\"text/javascript\">\r\n    ASPxAppointmentForm = ASPx.CreateClass(ASPxClien" +
					"tFormBase, {\r\n        Initialize: function () {\r\n            this.isValid = true" +
					";\r\n            this.isRecurrenceValid = true;\r\n            this.controls.edtStar" +
					"tDate.Validation.AddHandler(ASPx.CreateDelegate(this.OnEdtStartDateValidate, thi" +
					"s));\r\n            this.controls.edtEndDate.Validation.AddHandler(ASPx.CreateDele" +
					"gate(this.OnEdtEndDateValidate, this));\r\n            this.controls.edtStartDate." +
					"ValueChanged.AddHandler(ASPx.CreateDelegate(this.OnUpdateStartDateTimeValue, thi" +
					"s));\r\n            this.controls.edtEndDate.ValueChanged.AddHandler(ASPx.CreateDe" +
					"legate(this.OnUpdateEndDateTimeValue, this));\r\n            this.controls.edtStar" +
					"tTime.ValueChanged.AddHandler(ASPx.CreateDelegate(this.OnUpdateStartDateTimeValu" +
					"e, this));\r\n            this.controls.edtEndTime.ValueChanged.AddHandler(ASPx.Cr" +
					"eateDelegate(this.OnUpdateEndDateTimeValue, this));\r\n            this.controls.c" +
					"hkAllDay.CheckedChanged.AddHandler(ASPx.CreateDelegate(this.OnChkAllDayCheckedCh" +
					"anged, this));\r\n            this.controls.btnOk.Click.AddHandler(ASPx.CreateDele" +
					"gate(this.OnBtnOk, this));\r\n            if (this.controls.AppointmentRecurrenceF" +
					"orm1)\r\n                this.controls.AppointmentRecurrenceForm1.ValidationComple" +
					"ted.AddHandler(ASPx.CreateDelegate(this.OnRecurrenceRangeControlValidationComple" +
					"ted, this));\r\n            this.UpdateTimeEditorsVisibility();\r\n            if (t" +
					"his.controls.chkReminder)\r\n                this.controls.chkReminder.CheckedChan" +
					"ged.AddHandler(ASPx.CreateDelegate(this.OnChkReminderCheckedChanged, this));\r\n  " +
					"          if (this.controls.edtMultiResource)\r\n                this.controls.edt" +
					"MultiResource.SelectedIndexChanged.AddHandler(ASPx.CreateDelegate(this.OnEdtMult" +
					"iResourceSelectedIndexChanged, this));\r\n            var start = this.controls.ed" +
					"tStartDate.GetValue();\r\n            var end = this.controls.edtEndDate.GetValue(" +
					");\r\n            var duration = ASPxClientTimeInterval.CalculateDuration(start, e" +
					"nd);\r\n            this.appointmentInterval = new ASPxClientTimeInterval(start, d" +
					"uration);\r\n            this.appointmentInterval.SetAllDay(this.controls.chkAllDa" +
					"y.GetValue());\r\n            this.primaryIntervalJson = ASPx.Json.ToJson(this.app" +
					"ointmentInterval);\r\n            this.UpdateDateTimeEditors();\r\n        },\r\n     " +
					"   OnBtnOk: function (s, e) {\r\n            e.processOnServer = false;\r\n         " +
					"   var formOwner = this.GetFormOwner();\r\n            if (!formOwner)\r\n          " +
					"      return;\r\n            if (this.controls.AppointmentRecurrenceForm1 && this." +
					"IsRecurrenceChainRecreationNeeded() && this.cpHasExceptions) {\r\n                " +
					"formOwner.ShowMessageBox(this.localization.SchedulerLocalizer.Msg_Warning, this." +
					"localization.SchedulerLocalizer.Msg_RecurrenceExceptionsWillBeLost, this.OnWarni" +
					"ngExceptionWillBeLostOk.aspxBind(this));\r\n            } else {\r\n                " +
					"formOwner.AppointmentFormSave();\r\n            }\r\n        },\r\n        IsRecurrenc" +
					"eChainRecreationNeeded: function () {\r\n            var isIntervalChanged = this." +
					"primaryIntervalJson != ASPx.Json.ToJson(this.appointmentInterval);\r\n            " +
					"return isIntervalChanged || this.controls.AppointmentRecurrenceForm1.IsChanged()" +
					";\r\n        },\r\n        OnWarningExceptionWillBeLostOk: function () {\r\n          " +
					"  this.GetFormOwner().AppointmentFormSave();\r\n        },\r\n        OnEdtMultiReso" +
					"urceSelectedIndexChanged: function (s, e) {\r\n            var resourceNames = new" +
					" Array();\r\n            var items = s.GetSelectedItems();\r\n            var count " +
					"= items.length;\r\n            if (count > 0) {\r\n                for (var i = 0; i" +
					" < count; i++)\r\n                    resourceNames.push(items[i].text);\r\n        " +
					"    }\r\n            else\r\n                resourceNames.push(ddResource.cp_Captio" +
					"n_ResourceNone);\r\n            ddResource.SetValue(resourceNames.join(\', \'));\r\n  " +
					"      },\r\n        OnEdtStartDateValidate: function (s, e) {\r\n            if (!e." +
					"isValid)\r\n                return;\r\n            var startDate = this.controls.edt" +
					"StartDate.GetDate();\r\n            var endDate = this.controls.edtEndDate.GetDate" +
					"();\r\n            e.isValid = startDate == null || endDate == null || startDate <" +
					"= endDate;\r\n            e.errorText = \"The Start Date must precede the End Date." +
					"\";\r\n        },\r\n        OnEdtEndDateValidate: function (s, e) {\r\n            if " +
					"(!e.isValid)\r\n                return;\r\n            var startDate = this.controls" +
					".edtStartDate.GetDate();\r\n            var endDate = this.controls.edtEndDate.Get" +
					"Date();\r\n            e.isValid = startDate == null || endDate == null || startDa" +
					"te <= endDate;\r\n            e.errorText = \"The Start Date must precede the End D" +
					"ate.\";\r\n        },\r\n        OnUpdateEndDateTimeValue: function (s, e) {\r\n       " +
					"     var isAllDay = this.controls.chkAllDay.GetValue();\r\n            var date = " +
					"ASPxSchedulerDateTimeHelper.TruncToDate(this.controls.edtEndDate.GetDate());\r\n  " +
					"          if (isAllDay)\r\n                date = ASPxSchedulerDateTimeHelper.AddD" +
					"ays(date, 1);\r\n            var time = ASPxSchedulerDateTimeHelper.ToDayTime(this" +
					".controls.edtEndTime.GetDate());\r\n            var dateTime = ASPxSchedulerDateTi" +
					"meHelper.AddTimeSpan(date, time);\r\n            this.appointmentInterval.SetEnd(d" +
					"ateTime);\r\n            this.UpdateDateTimeEditors();\r\n            this.Validate(" +
					");\r\n        },\r\n        OnUpdateStartDateTimeValue: function (s, e) {\r\n         " +
					"   var date = ASPxSchedulerDateTimeHelper.TruncToDate(this.controls.edtStartDate" +
					".GetDate());\r\n            var time = ASPxSchedulerDateTimeHelper.ToDayTime(this." +
					"controls.edtStartTime.GetDate());\r\n            var dateTime = ASPxSchedulerDateT" +
					"imeHelper.AddTimeSpan(date, time);\r\n            this.appointmentInterval.SetStar" +
					"t(dateTime);\r\n            this.UpdateDateTimeEditors();\r\n            if (this.co" +
					"ntrols.AppointmentRecurrenceForm1)\r\n                this.controls.AppointmentRec" +
					"urrenceForm1.SetStart(dateTime);\r\n            this.Validate();\r\n        },\r\n    " +
					"    OnChkReminderCheckedChanged: function (s, e) {\r\n            var isReminderEn" +
					"abled = this.controls.chkReminder.GetValue();\r\n            if (isReminderEnabled" +
					")\r\n                this.controls.cbReminder.SetSelectedIndex(3);\r\n            el" +
					"se\r\n                this.controls.cbReminder.SetSelectedIndex(-1);\r\n            " +
					"this.controls.cbReminder.SetEnabled(isReminderEnabled);\r\n        },\r\n        OnC" +
					"hkAllDayCheckedChanged: function (s, e) {\r\n            this.UpdateTimeEditorsVis" +
					"ibility();\r\n            var isAllDay = this.controls.chkAllDay.GetValue();\r\n    " +
					"        this.appointmentInterval.SetAllDay(isAllDay);\r\n            this.UpdateDa" +
					"teTimeEditors();\r\n        },\r\n        UpdateDateTimeEditors: function () {\r\n    " +
					"        var isAllDay = this.controls.chkAllDay.GetValue();\r\n            this.con" +
					"trols.edtStartDate.SetValue(this.appointmentInterval.GetStart());\r\n            v" +
					"ar end = this.appointmentInterval.GetEnd();\r\n            if (isAllDay) {\r\n      " +
					"          end = ASPxSchedulerDateTimeHelper.AddDays(end, -1);\r\n            }\r\n  " +
					"          this.controls.edtEndDate.SetValue(end);\r\n            this.controls.edt" +
					"StartTime.SetValue(this.appointmentInterval.GetStart());\r\n            this.contr" +
					"ols.edtEndTime.SetValue(end);\r\n        },\r\n        UpdateTimeEditorsVisibility: " +
					"function () {\r\n            var isAllDay = this.controls.chkAllDay.GetValue();\r\n " +
					"           var visible = (isAllDay) ? \"none\" : \"\";\r\n            var startRoot = " +
					"ASPx.GetParentById(this.controls.edtStartTime.GetMainElement(), \"edtStartTimeLay" +
					"outRoot\");\r\n            var endRoot = ASPx.GetParentById(this.controls.edtEndTim" +
					"e.GetMainElement(), \"edtEndTimeLayoutRoot\");\r\n            startRoot.style.displa" +
					"y = visible;\r\n            endRoot.style.display = visible;\r\n        },\r\n        " +
					"Validate: function () {\r\n            this.isValid = ASPxClientEdit.ValidateEdito" +
					"rsInContainer(null);\r\n            this.controls.btnOk.SetEnabled(this.isValid &&" +
					" this.isRecurrenceValid);\r\n        },\r\n        OnRecurrenceRangeControlValidatio" +
					"nCompleted: function (s, e) {\r\n            if (!this.controls.AppointmentRecurre" +
					"nceForm1)\r\n                return;\r\n            this.isRecurrenceValid = this.co" +
					"ntrols.AppointmentRecurrenceForm1.IsValid();\r\n            this.controls.btnOk.Se" +
					"tEnabled(this.isValid && this.isRecurrenceValid);\r\n        }\r\n    });\r\n</script>" +
					"");
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		protected override void FrameworkInitialize() {
			base.FrameworkInitialize();
			this.@__BuildControlTree(this);
		}
	}
}
