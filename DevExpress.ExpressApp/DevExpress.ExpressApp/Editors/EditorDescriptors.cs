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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Editors {
	public class EditorDescriptors : IEnumerable<EditorDescriptor> {
		readonly List<ViewItemDescriptor> viewItems;
		readonly List<EditorDescriptor> editorDescriptors;
		readonly EditorTypeRegistrations propertyEditorRegistrations;
		readonly EditorTypeRegistrations listEditorRegistrations;
		public EditorDescriptors() {
			this.viewItems = new List<ViewItemDescriptor>();
			this.editorDescriptors = new List<EditorDescriptor>();
			this.propertyEditorRegistrations = new EditorTypeRegistrations();
			this.listEditorRegistrations = new EditorTypeRegistrations();
		}
		public EditorDescriptors(IEnumerable<EditorDescriptor> localEditorDescriptors)
			: this() {
				Initialize(localEditorDescriptors);
		}
		private void Initialize(IEnumerable<EditorDescriptor> localEditorDescriptors) {
			foreach(EditorDescriptor localEditorDescriptor in localEditorDescriptors) {
				if(localEditorDescriptor is ViewItemDescriptor) {
					Register((ViewItemDescriptor)localEditorDescriptor);
				}
				if(localEditorDescriptor is PropertyEditorDescriptor) {
					Register((PropertyEditorDescriptor)localEditorDescriptor);
				}
				if(localEditorDescriptor is ListEditorDescriptor) {
					Register((ListEditorDescriptor)localEditorDescriptor);
				}
			}
		}
		public ICollection<ViewItemDescriptor> ViewItems { get { return viewItems; } }
		public EditorTypeRegistrations PropertyEditorRegistrations { get { return propertyEditorRegistrations; } }
		public EditorTypeRegistrations ListEditorRegistrations { get { return listEditorRegistrations; } }
		public void Register(ViewItemDescriptor viewItem) {
			viewItems.Add(viewItem);
			editorDescriptors.Add(viewItem);
		}
		public void Register(PropertyEditorDescriptor propertyEditorDescriptor) {
			propertyEditorRegistrations.Registered(propertyEditorDescriptor);
			editorDescriptors.Add(propertyEditorDescriptor);
		}
		public void Register(ListEditorDescriptor listEditorDescriptor) {
			listEditorRegistrations.Registered(listEditorDescriptor);
			editorDescriptors.Add(listEditorDescriptor);
		}
		#region IEnumerable<EditorDescriptor> Members
		public IEnumerator<EditorDescriptor> GetEnumerator() {
			return editorDescriptors.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return editorDescriptors.GetEnumerator();
		}
		#endregion
	}
	public class EditorTypeRegistrations : IEnumerable<IEditorTypeRegistration> {
		readonly List<IEditorTypeRegistration> unassignedEditors = new List<IEditorTypeRegistration>();
		readonly List<IAliasRegistration> aliasRegistrations = new List<IAliasRegistration>();
		protected internal readonly Dictionary<Type, IEditorTypeRegistration> defaultEditors = new Dictionary<Type, IEditorTypeRegistration>();
		protected internal readonly Dictionary<IEditorTypeRegistration, IAliasRegistration> allEditorTypeToAliasMap = new Dictionary<IEditorTypeRegistration, IAliasRegistration>();
		public IEnumerable<IEditorTypeRegistration> TypeRegistrations {
			get {
				if(unassignedEditors.Count > 0) {
					throw new Exception(string.Format("Cannot register the {0} editor type with the {1} alias. The alias for type {2} isn't registered.",
						unassignedEditors[0].EditorType, unassignedEditors[0].Alias, unassignedEditors[0].ElementType));
				}
				return allEditorTypeToAliasMap.Keys;
			}
		}
		public void Registered(EditorDescriptor descriptor) {
			if(descriptor.RegistrationParams is IEditorTypeRegistration) {
				unassignedEditors.Add((IEditorTypeRegistration)descriptor.RegistrationParams);
			}
			if(descriptor.RegistrationParams is IAliasRegistration) {
				aliasRegistrations.Add((IAliasRegistration)descriptor.RegistrationParams);
			}
			for(int i = unassignedEditors.Count - 1; i >= 0; i--) {
				foreach(IAliasRegistration aliasRegistration in aliasRegistrations) {
					if((unassignedEditors[i].ElementType == aliasRegistration.ElementType) && (unassignedEditors[i].Alias == aliasRegistration.Alias)) {
						allEditorTypeToAliasMap.Add(unassignedEditors[i], aliasRegistration);
						unassignedEditors.Remove(unassignedEditors[i]);
						break;
					}
				}
			}
			foreach(KeyValuePair<IEditorTypeRegistration, IAliasRegistration> item in allEditorTypeToAliasMap) {
				if(item.Key.IsDefaultEditor && item.Value.IsDefaultAlias) {
					defaultEditors[item.Key.ElementType] = item.Key; 
				}
			}
		}
		public IEditorTypeRegistration GetDefaultEditorRegistration(Type type) {
			IEditorTypeRegistration defaultEditorRegistration = null;
			defaultEditors.TryGetValue(type, out defaultEditorRegistration);
			return defaultEditorRegistration;
		}
		public IEnumerable<IEditorTypeRegistration> GetEditorRegistrations(Type type) {
			return TypeRegistrations.Where(item => type == item.ElementType);
		}
		public IAliasRegistration GetAliasRegistration(IEditorTypeRegistration editorTypeRegistration) {
			return allEditorTypeToAliasMap[editorTypeRegistration];
		}
		#region IEnumerable<IEditorTypeRegistration> Members
		public IEnumerator<IEditorTypeRegistration> GetEnumerator() {
			return TypeRegistrations.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return TypeRegistrations.GetEnumerator();
		}
		#endregion
	}
}
