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

using System.ComponentModel.Design;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.UI;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Native;
using System.ComponentModel;
using System.Drawing.Design;
using System;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraEditors;
using System.Collections.Generic;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.XtraReports.Design.Commands {
	public class FieldListCommandExecutor : CommandExecutorBase {
		DataMemberListNodeBase node;
		bool showMessage;
		readonly LockService lockService;
		XtraReport Report { get { return (XtraReport)designerHost.RootComponent; } }
		LockService LockService { get { return lockService; } }
		public FieldListCommandExecutor(IDesignerHost designerHost)
			: base(designerHost) {
				lockService = LockService.GetInstance(designerHost);
		}
		public override void ExecCommand(CommandID cmdID) {
			if(this.parameters == null || this.parameters.Length == 0 || designerHost.IsDebugging())
				return;
			node = (DataMemberListNodeBase)this.parameters[0];
			showMessage = this.parameters.Length > 1 ? (bool)this.parameters[1] : true;
			if(node != null) {
				if(cmdID == FieldListCommands.AddCalculatedField)
					AddCalculatedField();
				else if(cmdID == FieldListCommands.EditCalculatedFields)
					EditCalculatedFields();
				else if(cmdID == FieldListCommands.EditExpressionCalculatedField)
					EditExpressionCalculatedField();
				else if(cmdID == FieldListCommands.DeleteCalculatedField)
					DeleteNode(node);
				else if(cmdID == FieldListCommands.AddParameter)
					AddParameter();
				else if(cmdID == FieldListCommands.EditParameters)
					EditParameters();
				else if(cmdID == FieldListCommands.DeleteParameter)
					DeleteNode(node);
				else if(cmdID == FieldListCommands.ClearCalculatedFields)
					ClearCalculatedFields();
				else if(cmdID == FieldListCommands.ClearParameters)
					ClearParameters();
			}
		}
		void AddCalculatedField() {
			CalculatedField calculatedField = new CalculatedField(GetDataSource(), GetDataMember());
			string description = String.Format(DesignSR.Trans_Add, typeof(CalculatedField).Name);
			DesignerTransaction transaction = designerHost.CreateTransaction(description);
			try {
				PropertyDescriptor property = DevExpress.XtraReports.Native.XRAccessor.GetPropertyDescriptor(Report, "CalculatedFields");
				changeServ.OnComponentChanging(Report, property);
				DesignToolHelper.AddToContainer(designerHost, calculatedField);
				Report.CalculatedFields.Add(calculatedField);
				changeServ.OnComponentChanged(Report, property, null, null);
			}
			finally {
				transaction.Commit();
			}
			selectionServ.SetSelectedComponents(new object[] { calculatedField });
		}
		object GetDataSource() {
			return ReferenceEquals(Report.DataSource, node.DataSource) ? null : node.DataSource;
		}
		string GetDataMember() {
			return node.DataMember != null ? node.DataMember : string.Empty;
		}
		List<CalculatedField> GetCalculatedFieldsForClear() {
			List<CalculatedField> fields = new List<CalculatedField>();
			foreach (CalculatedField field in Report.CalculatedFields) {
				if(LockService.CanDeleteComponent(field))
					fields.Add(field);
			}
			return fields;
		}
		void ClearCalculatedFields() {
			List<CalculatedField> fields = GetCalculatedFieldsForClear();
			if (fields.Count == 0 ||
			showMessage ? XtraMessageBox.Show(DesignLookAndFeelHelper.GetLookAndFeel(designerHost),
				ReportLocalizer.GetString(ReportStringId.Msg_WarningRemoveCalculatedFields),
				ReportLocalizer.GetString(ReportStringId.UD_ReportDesigner),
				MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No : false)
				return;
			string description = String.Format(DesignSR.Trans_ChangeProp, XRComponentPropertyNames.CalculatedFields);
			DesignerTransaction transaction = designerHost.CreateTransaction(description);
			try {
				PropertyDescriptor property = DevExpress.XtraReports.Native.XRAccessor.GetPropertyDescriptor(Report, XRComponentPropertyNames.CalculatedFields);
				changeServ.OnComponentChanging(Report, property);
				foreach (CalculatedField calculatedField in fields) {
					DesignToolHelper.RemoveFromContainer(designerHost, calculatedField);
					Report.CalculatedFields.Remove(calculatedField);
				}
				changeServ.OnComponentChanged(Report, property, null, null);
			}
			finally {
				transaction.Commit();
			}
			selectionServ.SetSelectedComponents(new object[] { Report });
		}
		void AddParameter() {
			INameCreationService nameCreationService = (INameCreationService)designerHost.GetService(typeof(INameCreationService));
			IContainer container = (IContainer)designerHost.GetService(typeof(IContainer));
			IExtensionsProvider extensionsProvider = (IExtensionsProvider)designerHost.RootComponent;
			ReportDesigner reportDesigner = (ReportDesigner)designerHost.GetDesigner(designerHost.RootComponent);
			var form = reportDesigner.CreateNewParameterEditorForm();
			var parameter = new Parameter();
			new NewParameterEditorPresenter(parameter, form, nameCreationService, container, extensionsProvider);
			if(form.ShowDialog() == DialogResult.OK) {
				string description = String.Format(DesignSR.Trans_Add, typeof(Parameter).Name);
				DesignerTransaction transaction = designerHost.CreateTransaction(description);
				try {
					PropertyDescriptor property = XRAccessor.GetPropertyDescriptor(Report, XRComponentPropertyNames.Parameters);
					changeServ.OnComponentChanging(Report, property);
					DesignToolHelper.AddToContainer(designerHost, parameter, parameter.Name);
					Report.Parameters.Add(parameter);
					changeServ.OnComponentChanged(Report, property, null, null);
				} finally {
					transaction.Commit();
				}
				selectionServ.SetSelectedComponents(new object[] { parameter });
			}
		}
		List<Parameter> GetParametersForClear() {
			List<Parameter> parameters = new List<Parameter>();
			foreach (Parameter parameter in Report.Parameters) {
				if(LockService.CanDeleteComponent(parameter))
					parameters.Add(parameter);
			}
			return parameters;
		}
		void ClearParameters() {
			List<Parameter> parameters = GetParametersForClear();
			bool allowCustomLookAndFeel = XtraMessageBox.AllowCustomLookAndFeel;
			XtraMessageBox.AllowCustomLookAndFeel = true;
			try {
				if (parameters.Count == 0 ||
					showMessage ? XtraMessageBox.Show(DesignLookAndFeelHelper.GetLookAndFeel(designerHost),
					ReportLocalizer.GetString(ReportStringId.Msg_WarningRemoveParameters),
					ReportLocalizer.GetString(ReportStringId.UD_ReportDesigner),
					MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No : false)
					return;
			}
			finally {
				XtraMessageBox.AllowCustomLookAndFeel = allowCustomLookAndFeel;
			}
			string description = String.Format(DesignSR.Trans_ChangeProp, XRComponentPropertyNames.Parameters);
			DesignerTransaction transaction = designerHost.CreateTransaction(description);
			try {
				PropertyDescriptor property = DevExpress.XtraReports.Native.XRAccessor.GetPropertyDescriptor(Report, XRComponentPropertyNames.Parameters);
				changeServ.OnComponentChanging(Report, property);
				foreach (Parameter parameter in parameters) {
					DesignToolHelper.RemoveFromContainer(designerHost, parameter);
					Report.Parameters.Remove(parameter);
				}
				changeServ.OnComponentChanged(Report, property, null, null);
			}
			finally {
				transaction.Commit();
			}
			selectionServ.SetSelectedComponents(new object[] { Report });
		}
		void EditCalculatedFields() {
			string description = String.Format(DesignSR.Trans_ChangeProp, XRComponentPropertyNames.CalculatedFields);
			DesignerTransaction transaction = designerHost.CreateTransaction(description);
			try {
				PropertyDescriptor property = DevExpress.XtraReports.Native.XRAccessor.GetPropertyDescriptor(Report, XRComponentPropertyNames.CalculatedFields);
				changeServ.OnComponentChanging(Report, property);
				EditorContextHelper.EditValue((ComponentDesigner)designerHost.GetDesigner(Report), Report, XRComponentPropertyNames.CalculatedFields);
				changeServ.OnComponentChanged(Report, property, null, null);
			}
			finally {
				transaction.Commit();
			}
		}
		void EditParameters() {
			string description = String.Format(DesignSR.Trans_ChangeProp, XRComponentPropertyNames.Parameters);
			DesignerTransaction transaction = designerHost.CreateTransaction(description);
			try {
				PropertyDescriptor property = DevExpress.XtraReports.Native.XRAccessor.GetPropertyDescriptor(Report, XRComponentPropertyNames.Parameters);
				changeServ.OnComponentChanging(Report, property);
				EditorContextHelper.EditValue((ComponentDesigner)designerHost.GetDesigner(Report), Report, XRComponentPropertyNames.Parameters);
				changeServ.OnComponentChanged(Report, property, null, null);
			}
			finally {
				transaction.Commit();
			}
		}
		void EditExpressionCalculatedField() {
			CalculatedField calculatedField = (CalculatedField)node.Component;
			if(calculatedField == null)
				return;
			string description = String.Format(DesignSR.Trans_ChangeProp, XRComponentPropertyNames.Expression);
			DesignerTransaction transaction = designerHost.CreateTransaction(description);
			try {
				EditorContextHelper.EditValue((ComponentDesigner)designerHost.GetDesigner(calculatedField), calculatedField, XRComponentPropertyNames.Expression);
			} finally {
				transaction.Commit();
			}
		}
		void DeleteNode(DataMemberListNodeBase node) {
			TreeListNode parent = node.ParentNode;
			int index = parent.Nodes.IndexOf(node);
			DeleteComponent(node.Component);
			for(int i = index; i < parent.Nodes.Count; i++) {
				if(TrySelectNode(parent.Nodes[i] as DataMemberListNode))
					return;
			}
			for(int i = index; i >= 0; i--) {
				if(TrySelectNode(parent.Nodes[i] as DataMemberListNode))
					return;
			}
			selectionServ.SetSelectedComponents(new object[] { Report });
		}
		bool TrySelectNode(DataMemberListNodeBase node) {
			if(node != null && node.Component != null && node.Component.Site != null) {
				selectionServ.SetSelectedComponents(new object[] { node.Component });
				return true;
			}
			return false;
		}
		void DeleteComponent(IComponent c) {
			if(c != null && LockService.CanDeleteComponent(c)) {
				IMenuCommandService menuServ = designerHost.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
				selectionServ.SetSelectedComponents(new object[] { c }, SelectionTypes.Replace);
				menuServ.GlobalInvoke(StandardCommands.Delete);
			}
		}
	}
}
