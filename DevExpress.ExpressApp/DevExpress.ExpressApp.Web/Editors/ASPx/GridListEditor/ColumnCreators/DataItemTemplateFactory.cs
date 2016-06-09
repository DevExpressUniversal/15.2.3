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
using System.Collections.ObjectModel;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class DataItemTemplateFactory : IDisposable {
		private MemberEditorInfoCalculator calculator = new MemberEditorInfoCalculator();
		private List<WebPropertyEditor> allEditors = new List<WebPropertyEditor>();
		private Dictionary<string, List<WebPropertyEditor>> editorsByPropertyName = new Dictionary<string, List<WebPropertyEditor>>();
		private bool isDisposed = false;
		public DataItemTemplateFactory() {
		}
		public WebPropertyEditor FindPropertyEditor(IModelMemberViewItem memberViewItem, ViewEditMode viewEditMode) {
			return FindPropertyEditor(GetPropertyEditorId(memberViewItem), viewEditMode);
		}
		public WebPropertyEditor FindPropertyEditor(string propertyName, ViewEditMode viewEditMode) {
			return FindPropertyEditorCore(propertyName, viewEditMode);
		}
		public void AddEditor(IModelMemberViewItem memberViewItem, WebPropertyEditor editor) {
			CheckIsDisposed();
			if(editor != null && !allEditors.Contains(editor)) {
				AddEditorToCache(memberViewItem, editor);
			}
		}
		public ReadOnlyCollection<WebPropertyEditor> PropertyEditors {
			get {
				CheckIsDisposed();
				return allEditors.AsReadOnly();
			}
		}
		public DataItemTemplateBase CreateColumnTemplate(IModelColumn columnInfo, IDataItemTemplateInfoProvider dataItemTemplateInfoProvider, XafApplication application, ITypeInfo objectTypeInfo, IObjectSpace objectSpace, ViewEditMode viewEditMode) {
			WebPropertyEditor propertyEditor = TryGetPropertyEditor(columnInfo, application, objectTypeInfo, objectSpace, viewEditMode);
			if(propertyEditor != null) {
				if(viewEditMode == ViewEditMode.View) {
					return new ViewModeDataItemTemplate(propertyEditor, dataItemTemplateInfoProvider);
				}
				else {
					return new EditModeDataItemTemplate(propertyEditor, dataItemTemplateInfoProvider);
				}
			}
			return null;
		}
		public void Dispose() {
			isDisposed = true;
			if(allEditors != null) {
				foreach(WebPropertyEditor propertyEditor in allEditors) {
					IDisposable disposable = propertyEditor as IDisposable;
					if(disposable != null) {
						disposable.Dispose();
					}
				}
				allEditors.Clear();
				allEditors = null;
			}
			if(editorsByPropertyName != null) {
				editorsByPropertyName.Clear();
				editorsByPropertyName = null;
			}
		}
		internal WebPropertyEditor CreatePropertyEditorCore(IModelMemberViewItem modelDetailViewItem, XafApplication application, ITypeInfo objectTypeInfo, IObjectSpace objectSpace) {
			CheckIsDisposed();
			WebPropertyEditor result = CreatePropertyEditor(modelDetailViewItem, application, objectTypeInfo, objectSpace);
			if(result != null) {
				AddEditorToCache(modelDetailViewItem, result);
			}
			return result;
		}
		protected virtual WebPropertyEditor CreatePropertyEditor(IModelMemberViewItem modelDetailViewItem, XafApplication application, ITypeInfo objectTypeInfo, IObjectSpace objectSpace) {
			CheckIsDisposed();
			WebPropertyEditor result = null;
			if(application != null && modelDetailViewItem.ModelMember != null) {
				Type objectType = objectTypeInfo.Type;
				bool needProtectedContent = HasProtectedContent(modelDetailViewItem.PropertyName);
				ViewItem propertyEditorCandidate = application.EditorFactory.CreateDetailViewEditor(needProtectedContent, modelDetailViewItem, objectType, application, objectSpace);
				result = propertyEditorCandidate as WebPropertyEditor;
				if(result == null) {
					if(propertyEditorCandidate != null) {
						propertyEditorCandidate.Dispose();
					}
					Type editorType = calculator.GetEditorType(modelDetailViewItem.ModelMember, typeof(WebPropertyEditor));
					if(editorType != null) {
						result = application.EditorFactory.CreatePropertyEditorByType(editorType, modelDetailViewItem, objectType, application, objectSpace) as WebPropertyEditor;
					}
				}
			}
			return result;
		}
		protected virtual bool HasProtectedContent(string propertyName) {
			if(QueryProtectedContentEditorState != null) {
				QueryProtectedContentEditorStateEventArgs args = new QueryProtectedContentEditorStateEventArgs(propertyName);
				QueryProtectedContentEditorState(this, args);
				return args.HasProtected;
			}
			else {
				return false;
			}
		}
		private WebPropertyEditor TryGetPropertyEditor(IModelMemberViewItem memberViewItem, XafApplication application, ITypeInfo objectTypeInfo, IObjectSpace objectSpace, ViewEditMode viewEditMode) {
			WebPropertyEditor result = FindPropertyEditor(memberViewItem, viewEditMode);
			if(result == null) {
				result = CreatePropertyEditorCore(memberViewItem, application, objectTypeInfo, objectSpace);
				if(result != null) {
					result.ViewEditMode = viewEditMode;
				}
			}
			return result;
		}
		private string GetPropertyEditorId(IModelMemberViewItem memberViewItem) {
			return memberViewItem != null ? memberViewItem.Id : string.Empty;
		}
		private WebPropertyEditor FindPropertyEditorCore(string propertyName, ViewEditMode viewEditMode) {
			CheckIsDisposed();
			List<WebPropertyEditor> editorsForProperty = null;
			if(editorsByPropertyName.ContainsKey(propertyName)) {
				editorsForProperty = editorsByPropertyName[propertyName];
			}
			if(editorsForProperty != null) {
				foreach(WebPropertyEditor editor in editorsForProperty) {
					if(editor.ViewEditMode == viewEditMode) {
						return editor;
					}
				}
			}
			return null;
		}
		private void AddEditorToCache(IModelMemberViewItem memberViewItem, WebPropertyEditor editor) {
			CheckIsDisposed();
			allEditors.Add(editor);
			string propertyEditorId = GetPropertyEditorId(memberViewItem);
			List<WebPropertyEditor> editorsForProperty = null;
			if(editorsByPropertyName.ContainsKey(propertyEditorId)) {
				editorsForProperty = editorsByPropertyName[propertyEditorId];
			}
			if(editorsForProperty == null) {
				editorsForProperty = new List<WebPropertyEditor>();
				editorsByPropertyName[propertyEditorId] = editorsForProperty;
			}
			editorsForProperty.Add(editor);
		}
		private void CheckIsDisposed() {
			if(isDisposed) {
				throw new ObjectDisposedException(this.GetType().FullName);
			}
		}
		public event EventHandler<QueryProtectedContentEditorStateEventArgs> QueryProtectedContentEditorState;
	}
}
