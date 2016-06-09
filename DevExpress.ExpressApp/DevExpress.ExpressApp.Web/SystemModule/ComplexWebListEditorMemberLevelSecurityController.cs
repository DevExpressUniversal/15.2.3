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
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public class ComplexWebListEditorMemberLevelSecurityController : ListViewControllerBase {
		protected override void SubscribeToListEditorEvent() {
			base.SubscribeToListEditorEvent();
			ComplexWebListEditor complexListEditor = View.Editor as ComplexWebListEditor;
			if(complexListEditor != null) {
				complexListEditor.CustomCreateCellControl += new EventHandler<CustomCreateCellControlEventArgs>(listEditor_CustomCreateCellControl);
			}
		}
		protected override void UnsubscribeToListEditorEvent() {
			base.UnsubscribeToListEditorEvent();
			ComplexWebListEditor complexListEditor = View.Editor as ComplexWebListEditor;
			if(complexListEditor != null) {
				complexListEditor.CustomCreateCellControl -= new EventHandler<CustomCreateCellControlEventArgs>(listEditor_CustomCreateCellControl);
			}
		}
		private void listEditor_CustomCreateCellControl(object sender, CustomCreateCellControlEventArgs e) {
			if(!e.Handled) {
				e.Handled = CustomCreateCellControl(e, View.CollectionSource, ObjectSpace);
			}
		}
		protected bool CustomCreateCellControl(CustomCreateCellControlEventArgs e, CollectionSourceBase collectionSourceBase, IObjectSpace ObjectSpace) {
			bool result = false;
			if(e.TargetObject == null || e.PropertyEditor is IProtectedContentEditor) {
				result = false;
			}
			else {
				if(e.TargetObject != null && MemberExists(e.TargetObject.GetType(), e.TargetPropertyName)) {
					if(!DataManipulationRight.CanRead(e.TargetObject.GetType(), e.TargetPropertyName, e.TargetObject, collectionSourceBase, ObjectSpace)) {
						Label label = new Label();
						label.Text = DisplayValueHelper.GetHtmlDisplayText(Application.Model.ProtectedContentText);
						e.CellControl = label;
						result = true;
					}
				}
			}
			return result;
		}
		protected bool MemberExists(Type type, string memberName) {
			return MemberExistsCore(type, memberName);
		}
		internal static bool MemberExistsCore(Type type, string memberName) {
			bool result = false;
			ITypeInfo requestedTypeInfo = XafTypesInfo.Instance.FindTypeInfo(type);
			if(requestedTypeInfo != null) {
				result = requestedTypeInfo.FindMember(memberName) != null;
			}
			return result;
		}
	}
}
