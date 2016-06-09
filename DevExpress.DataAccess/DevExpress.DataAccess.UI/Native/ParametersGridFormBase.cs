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

using System.Collections.Generic;
using System.Linq;
using DevExpress.Data;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
namespace DevExpress.DataAccess.UI.Native {
	public partial class ParametersGridFormBase : XtraForm {
		readonly IParameterService parameterService;
		readonly IDBSchemaProvider dbSchemaProvider;
		protected IParameterService ParameterService { get { return parameterService; } }
		protected IDBSchemaProvider DbSchemaProvider { get { return dbSchemaProvider; } }
		public ParametersGridFormBase() {
			InitializeComponent();
			LocalizeComponent();
		}
		public ParametersGridFormBase(UserLookAndFeel lookAndFeel, IParameterService parameterService) : this() {
			this.LookAndFeel.ParentLookAndFeel = lookAndFeel;
			this.parameterService = parameterService;
		}
		public ParametersGridFormBase(UserLookAndFeel lookAndFeel, IParameterService parameterService, IDBSchemaProvider dbSchemaProvider) : this(lookAndFeel, parameterService) {
			this.dbSchemaProvider = dbSchemaProvider;
		}
		public IEnumerable<IParameter> GetParameters() {
			return parametersGrid.Parameters;
		}
		protected void HideAddRemove() {
			layoutItemAddButton.ContentVisible = false;
			layoutItemRemoveButton.ContentVisible = false;
		}
		protected IEnumerable<IParameter> GetSourceParameters() {
			return parameterService != null ? parameterService.Parameters : new List<IParameter>();
		}
		protected IEnumerable<IParameter> EvaluatedParameters { get { return ParametersEvaluator.EvaluateParameters(GetSourceParameters(), GetParameters().Select(DataSourceParameterBase.FromIParameter)); } }
		void LocalizeComponent() {
			Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ParametersGridForm_Title);
			btnAdd.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Add);
			btnRemove.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Remove);
			btnPreview.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Preview);
			btnOk.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_OK);
			btnCancel.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Cancel);
		}
	}
}
