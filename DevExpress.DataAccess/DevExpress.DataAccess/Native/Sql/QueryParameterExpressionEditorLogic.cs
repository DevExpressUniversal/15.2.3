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
using DevExpress.Data;
using DevExpress.Data.ExpressionEditor;
using DevExpress.DataAccess.Native;
namespace DevExpress.DataAccess.Native.Sql {
	public class QueryParameterExpressionEditorLogic : ExpressionEditorLogic {
		readonly IEnumerable<IParameter> parameters;
		public QueryParameterExpressionEditorLogic(IExpressionEditor editor, object contextInstance, IEnumerable<IParameter> parameters)
			: base(editor, contextInstance) {
			this.parameters = parameters;
		}
		protected override object[] GetListOfInputTypesObjects() {
			return new object[] {
			this.editor.GetResourceString("Functions.Text"),
			this.editor.GetResourceString("Operators.Text"),
			this.editor.GetResourceString("Constants.Text"),
			this.editor.GetResourceString("Parameters.Text")};
		}
		DataSourceParameterBase Parameter { get { return (DataSourceParameterBase)this.contextInstance; } }
		protected override void FillFieldsTable(Dictionary<string, string> itemsTable) {
		}
		protected override void FillParametersTable(Dictionary<string, string> itemsTable) {
			if(this.parameters != null)
				foreach(IParameter item in this.parameters) {
					itemsTable[string.Format("[Parameters.{0}]", item.Name)] = "";
				}
		}
		protected override string GetExpressionMemoEditText() {
			return Convert.ToString(Parameter.Value);
		}
	}
}
