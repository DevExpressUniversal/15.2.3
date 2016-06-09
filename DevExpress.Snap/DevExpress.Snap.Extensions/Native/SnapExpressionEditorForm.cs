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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.Data.ExpressionEditor;
using DevExpress.Snap.Core;
using DevExpress.Snap.Native;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.Snap.Core.API;
namespace DevExpress.Snap.Extensions.Native {
	public class SnapExpressionEditorForm : ExpressionEditorForm, IServiceProvider {
		class ProxyCalculatedField : ICalculatedField {
			public string DataMember { get { return string.Empty; } }
			public object DataSource { get { return null; } }
			public string DisplayName { get { return string.Empty; } }
			public string Expression { get { return string.Empty; } }
			public XtraReports.UI.FieldType FieldType { get { return XtraReports.UI.FieldType.None; } }
			public string Name { get { return string.Empty; } }
		}
		SnapExpressionEditorForm() : this(new ProxyCalculatedField(), null) { }
		public SnapExpressionEditorForm(object instance, IDesignerHost designerHost)
			: base(instance, designerHost) {
		}
		protected override ExpressionEditorLogic CreateExpressionEditorLogic() {
			return new SnapExpressionEditorLogic(this, ContextInstance);
		}
		protected override string CaptionName { get { return "Expression.Text"; } }
		#region IServiceProvider Members
		object IServiceProvider.GetService(Type serviceType) {
			return ServiceProvider.GetService(serviceType);
		}
		#endregion
	}
	public class SnapExpressionEditorLogic : ExpressionEditorLogic {
		public SnapExpressionEditorLogic(IExpressionEditor editor, object instance)
			: base(editor, instance) {
		}
		protected override void FillFieldsTable(Dictionary<string, string> itemsTable) {
			using (XRDataContextBase dataContext = new XRDataContextBase()) {
				ICalculatedField calculatedField = (ICalculatedField)contextInstance;
				PropertyDescriptorCollection propertyDescriptors = dataContext[calculatedField.DataSource, calculatedField.DataMember].GetItemProperties();
				foreach (PropertyDescriptor item in propertyDescriptors) {
					if (!typeof(IList).IsAssignableFrom(item.PropertyType) || XRDataContextBase.IsStandardType(item.PropertyType))
						itemsTable[string.Format("[{0}]", item.Name)] = "";
				}
			}
		}
		protected override void FillParametersTable(Dictionary<string, string> itemsTable) {
			ParametersOwner parametersOwner = (ParametersOwner)((IServiceProvider)this.editor).GetService(typeof(ParametersOwner));
			foreach (Parameter item in parametersOwner.Parameters) {
				itemsTable[string.Format("[Parameters.{0}]", item.Name)] = "";
			}
		}
		protected override string GetExpressionMemoEditText() {
			return ((ICalculatedField)contextInstance).Expression;
		}
		protected override object[] GetListOfInputTypesObjects() {
			return new object[] {
			editor.GetResourceString("Functions.Text"),
			editor.GetResourceString("Operators.Text"),
			editor.GetResourceString("Fields.Text"),
			editor.GetResourceString("Constants.Text"),
			editor.GetResourceString("Parameters.Text")};
		}
	}
}
