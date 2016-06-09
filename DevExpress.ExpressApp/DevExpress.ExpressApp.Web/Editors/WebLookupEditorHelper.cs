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
using DevExpress.ExpressApp.Editors;
using System.Web;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Web.Core;
namespace DevExpress.ExpressApp.Web.Editors {
	public class WebLookupEditorHelper : LookupEditorHelper {
		public WebLookupEditorHelper(XafApplication application, IObjectSpace objectSpace, ITypeInfo lookupObjectTypeInfo, IModelMemberViewItem model)
			: base(application, objectSpace, lookupObjectTypeInfo, model) {
		}
		protected override string Escape(string text) {
			string result = base.Escape(text);
			return HttpUtility.HtmlEncode(result);
		}
		public void ReloadCollectionSource(CollectionSourceBase collectionSource, Object editingObject) {
			if(collectionSource is PropertyCollectionSource) {
				((PropertyCollectionSource)collectionSource).MasterObject = editingObject;
			}
			if(collectionSource is CollectionSource) {
				CriteriaWrapper criteriaWrapper = InitCriteriaWrapper(LookupObjectType, dataSourceCriteria, dataSourceCriteriaMemberInfo, editingObject);
				SetCriteriaToCollectionSource(collectionSource, criteriaWrapper);
			}
			collectionSource.ResetCollection();
		}
		public virtual object GetObjectByKey(object currentObject, string stringKey) {
			if(String.IsNullOrEmpty(stringKey)) {
				return null;
			}
			if(LookupObjectTypeInfo.IsPersistent) {
				return ObjectSpaceHelper.FindObjectByHandle(ObjectSpace, stringKey);
			}
			object result = null;
			if(DisplayMember != null) {
				CollectionSourceBase collectionSource = CreateCollectionSource(currentObject);
				if(collectionSource.CanApplyCriteria) {
					CriteriaOperator criteria = new BinaryOperator(DisplayMember.BindingName, stringKey);
					collectionSource.Criteria[FilterController.FullTextSearchCriteriaName] = criteria;
					if(collectionSource.List != null && collectionSource.List.Count > 0) {
						result = collectionSource.List[0];
					}
				}
				else if(collectionSource.List != null) {
					foreach(object item in collectionSource.List) {
						object value = DisplayMember.GetValue(item);
						if(value != null && value.ToString() == stringKey) {
							result = item;
							break;
						}
					}
				}
			}
			return result;
		}
		public virtual string GetObjectKey(object obj) {
			if(obj == null) {
				return String.Empty;
			}
			if(LookupObjectTypeInfo.IsPersistent) {
				return ObjectSpace.GetObjectHandle(obj);
			}
			if(DisplayMember != null) {
				object value = DisplayMember.GetValue(obj);
				return value == null ? String.Empty : value.ToString();
			}
			return String.Empty;
		}
	}
}
