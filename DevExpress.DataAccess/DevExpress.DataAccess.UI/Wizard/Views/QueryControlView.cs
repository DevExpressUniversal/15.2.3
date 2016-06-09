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
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.DataAccess.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Native.Sql;
using DevExpress.DataAccess.UI.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraRichEdit;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	public partial class QueryControlView : XtraUserControl {
		readonly bool noCustomSql;
		readonly bool queryBuilderLight;
		readonly IDisplayNameProvider displayNameProvider;
		readonly IServiceProvider propertyGridServices;
		readonly ICustomQueryValidator customQueryValidator;
		protected SqlRichEditControl richSqlEditor;
		protected LayoutControlItem layoutItemSqlEditor;
		public QueryControlView(bool noCustomSql, bool queryBuilderLight, IDisplayNameProvider displayNameProvider, IServiceProvider propertyGridServices, ICustomQueryValidator customQueryValidator) {
			this.noCustomSql = noCustomSql;
			this.queryBuilderLight = queryBuilderLight;
			this.displayNameProvider = displayNameProvider;
			this.propertyGridServices = propertyGridServices;
			this.customQueryValidator = customQueryValidator;
			InitializeComponent();
			LocalizeComponent();
			InitializeRichSqlEditor();
			PutRichSqlEditorToLayout();
			richSqlEditor.CustomContentChanged += richSqlEditor_CustomContentChanged;
		}
		QueryControlView() : this(false, false, null, null, null) { }
		public event EventHandler Changed;
		#region Implementation of IQueryControlView
		public string SqlString {
			get { return richSqlEditor.Text; }
			set {
				richSqlEditor.CustomContentChanged -= richSqlEditor_CustomContentChanged;
				richSqlEditor.Text = value;
				richSqlEditor.CustomContentChanged += richSqlEditor_CustomContentChanged;
			}
		}
		public void Initialize(bool allowCustomSql) {
			richSqlEditor.ReadOnly = !allowCustomSql;
			richSqlEditor.SyntaxColors.SqlEnabled = allowCustomSql;
			richSqlEditor.ActiveView.BackColor = richSqlEditor.SyntaxColors.BackgroundColor;
		}
		public QueryBuilderRunnerBase CreateQueryBuilderRunner(IDBSchemaProvider schemaProvider, SqlDataConnection connection, IParameterService parameterService) { return new QueryBuilderRunner(schemaProvider, connection, FindForm(), LookAndFeel, noCustomSql, queryBuilderLight, displayNameProvider, parameterService, propertyGridServices, customQueryValidator); }
		#endregion
		protected void RaiseSqlTextChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		void LocalizeComponent() {
			labelCaption.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryControl_SqlString);
		}
		void InitializeRichSqlEditor() {
			SqlSyntaxColors syntaxColors = new SqlSyntaxColors(LookAndFeel);
			richSqlEditor = new SqlRichEditControl(syntaxColors);
			richSqlEditor.ActiveViewType = RichEditViewType.Draft;
			richSqlEditor.Name = "richSqlEditor";
			richSqlEditor.Options.HorizontalRuler.Visibility = RichEditRulerVisibility.Hidden;
			richSqlEditor.Options.HorizontalScrollbar.Visibility = RichEditScrollbarVisibility.Auto;
			richSqlEditor.Options.VerticalScrollbar.Visibility = RichEditScrollbarVisibility.Auto;
			richSqlEditor.SyntaxHelper = null;
			richSqlEditor.Views.DraftView.AllowDisplayLineNumbers = true;
			richSqlEditor.Views.DraftView.Padding = new Padding(0, 0, 0, 0);
			richSqlEditor.Views.DraftView.AllowDisplayLineNumbers = false;
			richSqlEditor.SyntaxColors.SqlEnabled = true;
			richSqlEditor.ActiveView.BackColor = richSqlEditor.SyntaxColors.BackgroundColor;
			richSqlEditor.ReadOnly = false;
			richSqlEditor.Dock = DockStyle.Fill;
			richSqlEditor.AllowDrop = true;
			richSqlEditor.SyntaxHelper = new SyntaxHelper(richSqlEditor);
			richSqlEditor.CreateNewDocument();
		}
		void PutRichSqlEditorToLayout() {
			layoutItemSqlEditor = new LayoutControlItem(layoutControl, richSqlEditor) { TextVisible = false };
		}
		void richSqlEditor_CustomContentChanged(object sender, EventArgs e) {
			RaiseSqlTextChanged();
		}
	}
}
