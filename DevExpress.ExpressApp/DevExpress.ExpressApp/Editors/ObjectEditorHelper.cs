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
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Editors {
	public class ObjectEditorHelper : ObjectEditorHelperBase {
		public ObjectEditorHelper(ITypeInfo lookupObjectTypeInfo, IMemberInfo displayMember)
			: base(lookupObjectTypeInfo, displayMember) {
		}
		public ObjectEditorHelper(ITypeInfo lookupObjectTypeInfo, IModelMemberViewItem model)
			: base(lookupObjectTypeInfo, model) {
		}
		public static DetailView CreateDetailView(XafApplication application, IObjectSpace objectSpace, object editValue, Type editValueType, bool allowEdit) {
			return CreateDetailView(application, objectSpace, editValue, editValueType, allowEdit, "");
		}
		public static DetailView CreateDetailView(XafApplication application, IObjectSpace objectSpace, object editValue, Type editValueType, bool allowEdit, string viewId) {
			IObjectSpace nestedObjectSpace = objectSpace.CreateNestedObjectSpace();
			object shownObject = nestedObjectSpace.GetObject(editValue);
			if((shownObject == null || shownObject == DBNull.Value)) {
				shownObject = nestedObjectSpace.CreateObject(editValueType);
			}
			DetailView result = null;
			if(String.IsNullOrEmpty(viewId)) {
				result = application.CreateDetailView(nestedObjectSpace, shownObject, true);
			}
			else {
				result = application.CreateDetailView(nestedObjectSpace, viewId, true, shownObject);
			}
			result.AllowEdit.SetItemValue(typeof(ObjectEditorHelper).FullName, allowEdit);
			return result;
		}
	}
}
