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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.Persistent.Base;
using DevExpress.XtraFilterEditor;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	[ToolboxItem(false)]
	public class CriteriaModelEditorControl : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value) {
			if(context != null) {
				string propertyPath = null;
				ModelNode modelNode = context.Instance is IModelNodeContainer ? (ModelNode)((IModelNodeContainer)context.Instance).ModelNode : context.Instance as ModelNode;
				ITypeInfo typeInfo = CalculateFilteredTypeInfo(modelNode, context.PropertyDescriptor.Name, out propertyPath);
				ModelBrowserAdapter adapter = new ModelBrowserAdapter();
				adapter.ShowCollection = true;
				if(typeInfo != null) {
					bool isComplexType = Enumerator.Count(adapter.GetChildren(typeInfo)) > 0;
					if(isComplexType) {
						value = EditValueCore(typeInfo, value);
					}
					else {
						Messaging.GetMessaging(null).Show(string.Format("Cannot show the Filter Builder dialog as the {0} is the simple type", propertyPath), "Filter Builder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}
			}
			return value;
		}
		private ITypeInfo CalculateFilteredTypeInfo(ModelNode modelNode, string criteriaPropertyName, out string propertyPath) {
			ITypeInfo result = null;
			propertyPath = null;
			string objectTypeMemberName = "";
#pragma warning disable 0618
			CriteriaObjectTypeMemberAttribute criteriaObjectTypeMemberAttribute = ModelEditorHelper.GetPropertyAttribute<CriteriaObjectTypeMemberAttribute>(modelNode, criteriaPropertyName);
#pragma warning restore 0618
			if(criteriaObjectTypeMemberAttribute != null) {
				objectTypeMemberName = criteriaObjectTypeMemberAttribute.MemberName;
			}
			else {
				CriteriaOptionsAttribute criteriaOptionsAttribute = ModelEditorHelper.GetPropertyAttribute<CriteriaOptionsAttribute>(modelNode, criteriaPropertyName);
				if(criteriaOptionsAttribute != null) {
					objectTypeMemberName = criteriaOptionsAttribute.ObjectTypeMemberName;
				}
			}
			if(!string.IsNullOrEmpty(objectTypeMemberName)) {
				string[] criteriaObjectTypeMembers = objectTypeMemberName.Split(',');
				propertyPath = string.Join(", ", criteriaObjectTypeMembers);
				foreach(string filteredTypePropertyName in criteriaObjectTypeMembers) {
					object typeProperty = ModelNodePersistentPathHelper.FindValueByPath(modelNode, filteredTypePropertyName);
					if(typeProperty != null) {
						if(typeProperty is ITypeInfo) {
							result = (ITypeInfo)typeProperty;
						}
						else if(typeProperty is Type) {
							result = XafTypesInfo.Instance.FindTypeInfo((Type)typeProperty);
						}
						else if(typeProperty is string) {
							result = XafTypesInfo.Instance.FindTypeInfo((string)typeProperty);
						}
						else if(typeProperty is IMemberInfo) {
							IMemberInfo memberInfo = (IMemberInfo)typeProperty;
							if(memberInfo.IsList) {
								result = memberInfo.ListElementTypeInfo;
							}
							else {
								result = memberInfo.MemberTypeInfo;
							}
						}
					}
					if(result != null) {
						propertyPath = filteredTypePropertyName;
						break;
					}
				}
				if(result == null) {
					if(criteriaObjectTypeMembers.Length == 1) {
						Messaging.GetMessaging(null).Show(string.Format("Cannot show the Filter Builder dialog as the '{0}' value is not specified", propertyPath), "Filter Builder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
					else {
						Messaging.GetMessaging(null).Show(string.Format("Cannot show the Filter Builder dialog as none of the following values are specified:\r\n{0}", propertyPath), "Filter Builder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}
			}
			return result;
		}
		private static object EditValueCore(ITypeInfo filteredTypeInfo, object value) {
			using(PopupForm form = new PopupForm()) {
				form.AutoShrink = false;
				form.ClientSize = new Size(500, 320);
				SimpleAction okAction = new SimpleAction();
				okAction.ActionMeaning = ActionMeaning.Accept;
				okAction.Caption = CaptionHelper.GetLocalizedText("DialogButtons", "OK", "OK");
				form.ButtonsContainer.Register(okAction);
				SimpleAction cancelAction = new SimpleAction();
				cancelAction.ActionMeaning = ActionMeaning.Cancel;
				cancelAction.Caption = CaptionHelper.GetLocalizedText("DialogButtons", "Cancel", "Cancel");
				form.ButtonsContainer.Register(cancelAction);
				form.StartPosition = FormStartPosition.CenterScreen;
				FilterEditorControl filterControl = new FilterEditorControl();
				FilterEditorControlHelper helper = new FilterEditorControlHelper();
				helper.Attach(filterControl);
				if(filteredTypeInfo != null) {
					filterControl.SourceControl = CriteriaPropertyEditorHelper.CreateFilterControlDataSource(filteredTypeInfo.Type, null);
				}
				if(value != null) {
					filterControl.FilterString = (string)value;
				}
				form.AddControl(filterControl, CaptionHelper.GetLocalizedText("Captions", "FilterBuilder", "Filter Builder"));
				if(form.ShowDialog() == DialogResult.OK) {
					value = filterControl.FilterString;
				}
			}
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
#if DebugTest
		public ITypeInfo DebugTest_CalculateFilteredTypeInfo(ModelNode modelNode, string criteriaPropertyName, out string propertyPath) {
			return CalculateFilteredTypeInfo(modelNode, criteriaPropertyName, out propertyPath);
		}
#endif
	}
	public class CriteriaEmptyValuesProcessor : CriteriaProcessorBase {
		protected override void Process(OperandValue theOperator) {
			if(!ReferenceEquals(theOperator, null)) {
				if(theOperator.Value == null) {
					theOperator.Value = string.Empty;
				}
			}
		}
	}
}
