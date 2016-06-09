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
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.SystemModule {
	public class DependentEditorController : ViewController {
		private Dictionary<PropertyEditor, List<IDependentPropertyEditor>> dependentEditors = new Dictionary<PropertyEditor, List<IDependentPropertyEditor>>();
		private List<PropertyEditor> allEditors;
		protected Dictionary<PropertyEditor, List<IDependentPropertyEditor>> DependentEditors {
			get { return dependentEditors; }
		}
		protected virtual void RefreshDependentPropertyEditor(PropertyEditor masterEditor, IDependentPropertyEditor dependentEditor) {
			dependentEditor.Refresh();
		}
		protected virtual void RefreshDependentPropertyEditors(PropertyEditor propertyEditor) {
			if(dependentEditors.ContainsKey(propertyEditor)) {
				List<IDependentPropertyEditor> dependents = dependentEditors[propertyEditor];
				if(dependents != null) {
					foreach(IDependentPropertyEditor dependent in dependents) {
						RefreshDependentPropertyEditor(propertyEditor, dependent);
					}
				}
			}
		}
		private void PropertyEditor_ValueStored(object sender, EventArgs e) {
			RefreshDependentPropertyEditors((PropertyEditor)sender);
		}
		protected List<string> GetMasterProperties(IList<string> masterProperties) {
			List<string> result = new List<string>();
			foreach(string masterProperty in masterProperties) {
				string[] masterPropertyParts = masterProperty.Split('.');
				string current = "";
				for(int i = 0; i < masterPropertyParts.Length; i++) {
					current = string.Join(".", masterPropertyParts, 0, i + 1);
					if(!result.Contains(current)) {
						result.Add(current);
					}
				}
			}
			return result;
		}
		protected override void OnDeactivated() {
			if(allEditors != null) {
				foreach(PropertyEditor editor in allEditors) {
					editor.ValueStored -= new EventHandler(PropertyEditor_ValueStored);
				}
				allEditors.Clear();
			}
			dependentEditors.Clear();
			base.OnDeactivated();
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(View is DetailView) {
				ProcessEditors(((DetailView)View).GetItems<PropertyEditor>());
			}
		}
		protected virtual void ProcessEditors(ICollection<PropertyEditor> editors) {
			foreach(PropertyEditor editor in editors) {
				IDependentPropertyEditor dependent = editor as IDependentPropertyEditor;
				if(dependent == null && editor.PropertyName.LastIndexOf('.') > 0) {
					dependent = new SimpleDependentPropertyHelper(editor);
					if(!dependentEditors.ContainsKey(editor)) {
						dependentEditors.Add(editor, new List<IDependentPropertyEditor>());
					}
				}
				if(dependent != null) {
					List<string> masterProperties = GetMasterProperties(dependent.MasterProperties);
					if(masterProperties.Count > 0) {
						foreach(PropertyEditor masterEditor in editors) {
							if(masterProperties.IndexOf(masterEditor.PropertyName) >= 0) {
								if(!dependentEditors.ContainsKey(masterEditor)) {
									dependentEditors.Add(masterEditor, new List<IDependentPropertyEditor>());
								}
								dependentEditors[masterEditor].Add(dependent);
								if(dependent is SimpleDependentPropertyHelper) {
									if(masterEditor is IDependentPropertyEditor) {
										dependentEditors[editor].Add(masterEditor as IDependentPropertyEditor);
									}
									else {
										dependentEditors[editor].Add(new SimpleDependentPropertyHelper(masterEditor));
									}
								}
							}
						}
					}
				}
				editor.ValueStored += new EventHandler(PropertyEditor_ValueStored);
			}
			allEditors = new List<PropertyEditor>(editors);
		}
		public DependentEditorController() : base() {
			TargetViewType = ViewType.DetailView;
		}
	}
	public class SimpleDependentPropertyHelper : IDependentPropertyEditor {
		private PropertyEditor editor = null;
		public SimpleDependentPropertyHelper(PropertyEditor editor) {
			this.editor = editor;
		}
		#region IDependentPropertyEditor Members
		public IList<string> MasterProperties {
			get {
				if(editor.PropertyName.LastIndexOf('.') > 0)
					return new string[] { editor.PropertyName.Remove(editor.PropertyName.LastIndexOf('.')) };
				else
					return new string[] { "" };
			}
		}
		public void Refresh() {
			editor.Refresh();			
		}
		#endregion
	}
}
