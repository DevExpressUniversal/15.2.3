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
	public partial class AppointmentFormEx {
		protected global::DevExpress.Web.ASPxLabel lblSubject;
		protected global::DevExpress.Web.ASPxTextBox tbSubject;
		protected global::DevExpress.Web.ASPxLabel lblLocation;
		protected global::DevExpress.Web.ASPxTextBox tbLocation;
		protected global::DevExpress.Web.ASPxLabel lblLabel;
		protected global::DevExpress.Web.ASPxComboBox edtLabel;
		protected global::DevExpress.Web.ASPxLabel lblStartDate;
		protected global::DevExpress.Web.ASPxDateEdit edtStartDate;
		protected global::DevExpress.Web.ASPxLabel lblEndDate;
		protected global::DevExpress.Web.ASPxDateEdit edtEndDate;
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
		protected global::DevExpress.Web.ASPxCheckBox chkRecurrence;
		protected global::DevExpress.Web.ASPxCallbackPanel RecurrencePanel;
		protected global::DevExpress.Web.ASPxButton btnOk;
		protected global::DevExpress.Web.ASPxButton btnCancel;
		protected global::DevExpress.Web.ASPxButton btnDelete;
		protected global::DevExpress.Web.ASPxScheduler.Controls.ASPxSchedulerStatusInfo schedulerStatusInfo;
		private static bool @__initialized;
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public AppointmentFormEx() {
			((global::System.Web.UI.UserControl)(this)).AppRelativeVirtualPath = "~/AppointmentFormEx.ascx";
			if ((global::DevExpress.Web.ASPxScheduler.Forms.Internal.AppointmentFormEx.@__initialized == false)) {
				global::DevExpress.Web.ASPxScheduler.Forms.Internal.AppointmentFormEx.@__initialized = true;
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
			dataBindingExpressionBuilderTarget.Text = global::System.Convert.ToString( ((AppointmentFormTemplateContainer)Container).Appointment.Subject , global::System.Globalization.CultureInfo.CurrentCulture);
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
		private global::DevExpress.Web.ASPxDateEdit @__BuildControledtStartDate() {
			global::DevExpress.Web.ASPxDateEdit @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxDateEdit();
			this.edtStartDate = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.ID = "edtStartDate";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.EditFormat = global::DevExpress.Web.EditFormat.DateTime;
			@__ctrl.DateOnError = global::DevExpress.Web.DateOnError.Undo;
			@__ctrl.AllowNull = false;
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
		private global::DevExpress.Web.ASPxDateEdit @__BuildControledtEndDate() {
			global::DevExpress.Web.ASPxDateEdit @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxDateEdit();
			this.edtEndDate = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "edtEndDate";
			@__ctrl.EditFormat = global::DevExpress.Web.EditFormat.DateTime;
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.DateOnError = global::DevExpress.Web.DateOnError.Undo;
			@__ctrl.AllowNull = false;
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
		private void @__BuildControl__control4(DevExpress.Web.ListBoxClientSideEvents @__ctrl) {
			@__ctrl.SelectedIndexChanged = @"function(s, e) {
                                        var resourceNames = new Array();
                                        var items = s.GetSelectedItems();
                                        var count = items.length;
                                        if (count > 0) {
                                            for(var i=0; i<count; i++) 
                                                resourceNames.push(items[i].text);
                                        }
                                        else
                                            resourceNames.push(ddResource.cp_Caption_ResourceNone);
                                        ddResource.SetValue(resourceNames.join(', '));
                                    }";
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxListBox @__BuildControl__control3() {
			global::DevExpress.Web.ASPxListBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxListBox();
			@__ctrl.TemplateControl = this;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "edtMultiResource";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.SelectionMode = global::DevExpress.Web.ListEditSelectionMode.CheckColumn;
			@__ctrl.Border.BorderWidth = new System.Web.UI.WebControls.Unit(0D, global::System.Web.UI.WebControls.UnitType.Pixel);
			this.@__BuildControl__control4(@__ctrl.ClientSideEvents);
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBinding__control3);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBinding__control3(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxListBox dataBindingExpressionBuilderTarget;
			DevExpress.Web.TemplateContainerBase Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxListBox)(sender));
			Container = ((DevExpress.Web.TemplateContainerBase)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.DataSource = ((object)( ResourceDataSource ));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control2(System.Web.UI.Control @__ctrl) {
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                "));
			global::DevExpress.Web.ASPxListBox @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control3();
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
			@__ctrl.DropDownWindowTemplate = new System.Web.UI.CompiledTemplateBuilder(new System.Web.UI.BuildTemplateMethod(this.@__BuildControl__control2));
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
		private void @__BuildControl__control5(DevExpress.Web.CheckEditClientSideEvents @__ctrl) {
			@__ctrl.CheckedChanged = "function(s, e) { OnChkReminderCheckedChanged(s, e); }";
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxCheckBox @__BuildControlchkReminder() {
			global::DevExpress.Web.ASPxCheckBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxCheckBox();
			this.chkReminder = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.ID = "chkReminder";
			this.@__BuildControl__control5(@__ctrl.ClientSideEvents);
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
			@__ctrl.ClientInstanceName = "_dxAppointmentForm_cbReminder";
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
		private void @__BuildControl__control6(DevExpress.Web.CheckEditClientSideEvents @__ctrl) {
			@__ctrl.CheckedChanged = "function(s,e) { if (s.GetChecked()) { if (RecurrencePanel.mainElement.innerHTML.r" +
				"eplace(/^\\s*(\\b.*\\b|)\\s*$/, \'\') == \'\') RecurrencePanel.PerformCallback(); else R" +
				"ecurrencePanel.SetVisible(true); } else RecurrencePanel.SetVisible(false); }";
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxCheckBox @__BuildControlchkRecurrence() {
			global::DevExpress.Web.ASPxCheckBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxCheckBox();
			this.chkRecurrence = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "chkRecurrence";
			this.@__BuildControl__control6(@__ctrl.ClientSideEvents);
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingchkRecurrence);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingchkRecurrence(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxCheckBox dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxCheckBox)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.Checked = ((bool)( ((AppointmentFormTemplateContainer)Container).Appointment.IsRecurring ));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxCallbackPanel @__BuildControlRecurrencePanel() {
			global::DevExpress.Web.ASPxCallbackPanel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxCallbackPanel();
			this.RecurrencePanel = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "RecurrencePanel";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.ClientInstanceName = "RecurrencePanel";
			@__ctrl.Callback += new DevExpress.Web.CallbackEventHandlerBase(this.OnCallback);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbtnOk() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.btnOk = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.ID = "btnOk";
			@__ctrl.UseSubmitBehavior = false;
			@__ctrl.AutoPostBack = false;
			@__ctrl.EnableViewState = false;
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(91D, global::System.Web.UI.WebControls.UnitType.Pixel);
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
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.ID = "btnCancel";
			@__ctrl.UseSubmitBehavior = false;
			@__ctrl.AutoPostBack = false;
			@__ctrl.EnableViewState = false;
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(91D, global::System.Web.UI.WebControls.UnitType.Pixel);
			@__ctrl.CausesValidation = false;
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
			@__ctrl.ClientInstanceName = "_dx";
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
		private void @__BuildControlTree(AppointmentFormEx @__ctrl) {
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
			global::DevExpress.Web.ASPxLabel @__ctrl9;
			@__ctrl9 = this.@__BuildControllblEndDate();
			@__parser.AddParsedSubObject(@__ctrl9);
			global::DevExpress.Web.ASPxDateEdit @__ctrl10;
			@__ctrl10 = this.@__BuildControledtEndDate();
			@__parser.AddParsedSubObject(@__ctrl10);
			global::DevExpress.Web.ASPxLabel @__ctrl11;
			@__ctrl11 = this.@__BuildControllblStatus();
			@__parser.AddParsedSubObject(@__ctrl11);
			global::DevExpress.Web.ASPxComboBox @__ctrl12;
			@__ctrl12 = this.@__BuildControledtStatus();
			@__parser.AddParsedSubObject(@__ctrl12);
			global::DevExpress.Web.ASPxCheckBox @__ctrl13;
			@__ctrl13 = this.@__BuildControlchkAllDay();
			@__parser.AddParsedSubObject(@__ctrl13);
			global::DevExpress.Web.ASPxLabel @__ctrl14;
			@__ctrl14 = this.@__BuildControllblAllDay();
			@__parser.AddParsedSubObject(@__ctrl14);
			global::DevExpress.Web.ASPxLabel @__ctrl15;
			@__ctrl15 = this.@__BuildControllblResource();
			@__parser.AddParsedSubObject(@__ctrl15);
			global::DevExpress.Web.ASPxDropDownEdit @__ctrl16;
			@__ctrl16 = this.@__BuildControlddResource();
			@__parser.AddParsedSubObject(@__ctrl16);
			global::DevExpress.Web.ASPxComboBox @__ctrl17;
			@__ctrl17 = this.@__BuildControledtResource();
			@__parser.AddParsedSubObject(@__ctrl17);
			global::DevExpress.Web.ASPxCheckBox @__ctrl18;
			@__ctrl18 = this.@__BuildControlchkReminder();
			@__parser.AddParsedSubObject(@__ctrl18);
			global::DevExpress.Web.ASPxLabel @__ctrl19;
			@__ctrl19 = this.@__BuildControllblReminder();
			@__parser.AddParsedSubObject(@__ctrl19);
			global::DevExpress.Web.ASPxComboBox @__ctrl20;
			@__ctrl20 = this.@__BuildControlcbReminder();
			@__parser.AddParsedSubObject(@__ctrl20);
			global::DevExpress.Web.ASPxMemo @__ctrl21;
			@__ctrl21 = this.@__BuildControltbDescription();
			@__parser.AddParsedSubObject(@__ctrl21);
			global::DevExpress.Web.ASPxCheckBox @__ctrl22;
			@__ctrl22 = this.@__BuildControlchkRecurrence();
			@__parser.AddParsedSubObject(@__ctrl22);
			global::DevExpress.Web.ASPxCallbackPanel @__ctrl23;
			@__ctrl23 = this.@__BuildControlRecurrencePanel();
			@__parser.AddParsedSubObject(@__ctrl23);
			global::DevExpress.Web.ASPxButton @__ctrl24;
			@__ctrl24 = this.@__BuildControlbtnOk();
			@__parser.AddParsedSubObject(@__ctrl24);
			global::DevExpress.Web.ASPxButton @__ctrl25;
			@__ctrl25 = this.@__BuildControlbtnCancel();
			@__parser.AddParsedSubObject(@__ctrl25);
			global::DevExpress.Web.ASPxButton @__ctrl26;
			@__ctrl26 = this.@__BuildControlbtnDelete();
			@__parser.AddParsedSubObject(@__ctrl26);
			global::DevExpress.Web.ASPxScheduler.Controls.ASPxSchedulerStatusInfo @__ctrl27;
			@__ctrl27 = this.@__BuildControlschedulerStatusInfo();
			@__parser.AddParsedSubObject(@__ctrl27);
			@__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__Render__control1));
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__Render__control1(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
			@__w.Write("\r\n\r\n<table class=\"dxscAppointmentForm\" ");
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
			@__w.Write(" \r\n                    </td>\r\n                </tr>\r\n            </table>\r\n      " +
					"  </td>\r\n        <td class=\"dxscSingleCell\">\r\n            <table class=\"dxscLabe" +
					"lControlPair\" ");
										@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(">\r\n                <tr>\r\n                    <td class=\"dxscLabelCell\" style=\"pad" +
					"ding-left: 25px;\">\r\n                        ");
			parameterContainer.Controls[8].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxscControlCell\">\r\n  " +
					"                      ");
			parameterContainer.Controls[9].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                </tr>\r\n            </table>\r\n       " +
					" </td>\r\n    </tr>\r\n    <tr>\r\n        <td class=\"dxscSingleCell\">\r\n            <t" +
					"able class=\"dxscLabelControlPair\" ");
										@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(">\r\n                <tr>\r\n                    <td class=\"dxscLabelCell\">\r\n        " +
					"                ");
			parameterContainer.Controls[10].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxscControlCell\">\r\n  " +
					"                      ");
			parameterContainer.Controls[11].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                </tr>\r\n            </table>\r\n       " +
					" </td>\r\n        <td class=\"dxscSingleCell\" style=\"padding-left: 22px;\">\r\n       " +
					"     <table class=\"dxscLabelControlPair\" ");
										@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(">\r\n                <tr>\r\n                    <td style=\"width: 20px; height: 20px" +
					";\">\r\n                        ");
			parameterContainer.Controls[12].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td style=\"padding-left: 2px;\">\r" +
					"\n                        ");
			parameterContainer.Controls[13].RenderControl(@__w);
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
			parameterContainer.Controls[14].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxscControlCell\">\r\n  " +
					"                      ");
						   if(ResourceSharing) { 
			parameterContainer.Controls[15].RenderControl(@__w);
			@__w.Write("                        \r\n");
   } else { 
			parameterContainer.Controls[16].RenderControl(@__w);
			@__w.Write("\r\n");
   } 
			@__w.Write("             \r\n                    </td>\r\n                </tr>\r\n            </ta" +
					"ble>\r\n        </td>\r\n");
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
			parameterContainer.Controls[17].RenderControl(@__w);
			@__w.Write("\r\n                                </td>\r\n                                <td styl" +
					"e=\"padding-left: 2px;\">\r\n                                    ");
			parameterContainer.Controls[18].RenderControl(@__w);
			@__w.Write("\r\n                                </td>\r\n                            </tr>\r\n     " +
					"                   </table>\r\n                    </td>\r\n                    <td " +
					"class=\"dxscControlCell\" style=\"padding-left: 3px\">\r\n                        ");
			parameterContainer.Controls[19].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                </tr>\r\n            </table>\r\n       " +
					" </td>\r\n");
   } 
			@__w.Write("\r\n    </tr>\r\n    <tr>\r\n        <td class=\"dxscDoubleCell\" colspan=\"2\" style=\"heig" +
					"ht: 90px;\">\r\n            ");
			parameterContainer.Controls[20].RenderControl(@__w);
			@__w.Write("\r\n        </td>\r\n    </tr>\r\n</table>\r\n                        \r\n<table>\r\n    <tr>" +
					"\r\n        <td  class=\"dxscDoubleCell\" colspan=\"2\">\r\n            ");
			parameterContainer.Controls[21].RenderControl(@__w);
			@__w.Write("\r\n            ");
			parameterContainer.Controls[22].RenderControl(@__w);
			@__w.Write("\r\n        </td>\r\n    </tr>\r\n</table>\r\n                   \r\n<table ");
@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(" style=\"width: 100%; height: 35px;\">\r\n    <tr>\r\n        <td class=\"dx-ac\" style=\"" +
					"width: 100%; height: 100%;\" ");
													 @__w.Write( DevExpress.Web.Internal.RenderUtils.GetAlignAttributes(this, "center", null) );
			@__w.Write(">\r\n            <table class=\"dxscButtonTable\" style=\"height: 100%\">\r\n            " +
					"    <tr>\r\n                    <td class=\"dxscCellWithPadding\">\r\n                " +
					"        ");
			parameterContainer.Controls[23].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxscCellWithPadding\">" +
					"\r\n                        ");
			parameterContainer.Controls[24].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxscCellWithPadding\">" +
					"\r\n                        ");
			parameterContainer.Controls[25].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                </tr>\r\n            </table>\r\n       " +
					" </td>\r\n    </tr>\r\n</table>\r\n<table ");
@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(" style=\"width: 100%;\">\r\n    <tr>\r\n        <td class=\"dx-al\" style=\"width: 100%;\" " +
					"");
									   @__w.Write( DevExpress.Web.Internal.RenderUtils.GetAlignAttributes(this, "left", null) );
			@__w.Write(">\r\n            ");
			parameterContainer.Controls[26].RenderControl(@__w);
			@__w.Write(@"
        </td>
    </tr>
</table>
<script id=""dxss_ASPxSchedulerAppoinmentForm"" type=""text/javascript"">
    function OnChkReminderCheckedChanged(s, e) {
        var isReminderEnabled = s.GetValue();
        if (isReminderEnabled)
            _dxAppointmentForm_cbReminder.SetSelectedIndex(3);
        else
            _dxAppointmentForm_cbReminder.SetSelectedIndex(-1);
            
        _dxAppointmentForm_cbReminder.SetEnabled(isReminderEnabled);
        
    }
</script>");
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		protected override void FrameworkInitialize() {
			base.FrameworkInitialize();
			this.@__BuildControlTree(this);
		}
	}
}
