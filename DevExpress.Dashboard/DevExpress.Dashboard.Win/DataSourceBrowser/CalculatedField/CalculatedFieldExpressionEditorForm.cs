#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Data;
using DevExpress.Data.ExpressionEditor;
using DevExpress.Utils;
using DevExpress.XtraEditors.Design;
namespace DevExpress.DashboardWin.Native {
	public class CalculatedFieldExpressionEditorForm : ExpressionEditorForm {
		readonly DashboardParameterCollection parameters;
		readonly DataFieldsBrowser browser;
		IServiceProvider serviceProvider;
		DataFieldsBrowserPresenter browserPresenter;
		ComponentResourceManager resources;
		internal DashboardParameterCollection Parameters { get { return parameters; } }
		internal DataSourceInfo DataSourceInfo { get { return browserPresenter.DataSourceInfo; } }
		internal Dashboard Dashboard {
			get {
				IDashboardOwnerService service = serviceProvider.RequestServiceStrictly<IDashboardOwnerService>();
				return service != null ? service.Dashboard : null;
			}
		}
		protected override string CaptionName { get { return "Expression.Text"; } }
		public CalculatedFieldExpressionEditorForm(CalculatedField field, DataSourceInfo dataSourceInfo, DashboardParameterCollection parameters, IServiceProvider serviceProvider)
			: base(dataSourceInfo.DataSource.CreateCalculatedFieldColumnInfo(field, parameters), null) {
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			if(resources == null)
				InitializeComponent();
			this.parameters = parameters;
			this.serviceProvider = serviceProvider;
			IDashboardGuiContextService guiService = serviceProvider.RequestService<IDashboardGuiContextService>();
			if(guiService != null)
				LookAndFeel.ParentLookAndFeel = guiService.LookAndFeel;
			ControlBox = true;
			MaximizeBox = false;
			MinimizeBox = false;
			Icon = ResourceImageHelper.CreateIconFromResources("DevExpress.XtraEditors.Images.expression.ico", typeof(ExpressionEditorFormEx).Assembly);
			browser = new SimpleDataFieldsBrowser { Visible = false };
			Controls.Add(browser);
			IDataFieldsBrowserPresenterFactory factory = serviceProvider.RequestServiceStrictly<IDataFieldsBrowserPresenterFactory>();
			browserPresenter = factory.CreatePresenter(browser, dataSourceInfo, serviceProvider);
			browser.SetToolbarVisibility(false);
		}
		public string GetCalculatedFieldResourceString(string name) {
			if(resources == null)
				InitializeComponent();
			return resources.GetString(name);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(browserPresenter != null) {
					browserPresenter.Dispose();
					browserPresenter = null;
				}
				if(serviceProvider != null)
					serviceProvider = null;
			}
			base.Dispose(disposing);
		}
		protected override ExpressionEditorLogic CreateExpressionEditorLogic() {
			return new CalculatedFieldExpressionEditorLogic(this, (IDataColumnInfo)ContextInstance, this);
		}
		internal void ShowParametersEditor() {
			browser.Visible = true;
			browser.Bounds = GetParametersEditorBounds();
			browser.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			browserPresenter.DataFieldDoubleClick += OnDataFieldDoubleClick;
			browserPresenter.FocusedDataFieldChanged += OnFocusedDataFieldChanged;
			HideParameters();
		}
		internal void HideParametersEditor() {
			if(browser != null) {
				browser.Visible = false;
				browserPresenter.DataFieldDoubleClick -= OnDataFieldDoubleClick;
				browserPresenter.FocusedDataFieldChanged -= OnFocusedDataFieldChanged;
			}
			ShowParameters();
		}
		void OnFocusedDataFieldChanged(object sender, DataFieldEventArgs e) {
			DataField field = e.DataField;
			if(field == null || !field.IsDataMemberNode || string.IsNullOrEmpty(field.DataMember))
				SetDescription(string.Empty);
			else
				SetDescription(GetResourceString("Fields Description Prefix") + field.ActualFieldType.ToString());
		}
		void OnDataFieldDoubleClick(object sender, DataFieldEventArgs e) {
			DataField field = e.DataField;
			if(field == null || !field.IsDataMemberNode || string.IsNullOrEmpty(field.DataMember))
				return;
			CalculatedFieldExpressionEditorLogic cfEditorLogic = (CalculatedFieldExpressionEditorLogic)fEditorLogic;
			IDataColumnInfo info = null;
			if(cfEditorLogic.ColumnInfo != null && cfEditorLogic.ColumnInfo.Columns != null)
				info = cfEditorLogic.ColumnInfo.Columns.FirstOrDefault((f) => f.FieldName == field.DataMember);
			fEditorLogic.InsertTextInExpressionMemo(string.Format("[{0}]", info == null ? field.Caption : info.Caption)); 
		}
		void InitializeComponent() {
			resources = new System.ComponentModel.ComponentResourceManager(typeof(CalculatedFieldExpressionEditorForm));
			this.SuspendLayout();
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			resources.ApplyResources(this.buttonOK, "buttonOK");
			resources.ApplyResources(this, "$this");
			this.Name = "CalculatedFieldExpressionEditorForm";
			this.ResumeLayout(false);
		}
	}
}
