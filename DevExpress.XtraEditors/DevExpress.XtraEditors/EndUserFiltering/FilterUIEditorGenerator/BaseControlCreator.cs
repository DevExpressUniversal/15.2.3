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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Filtering;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraEditors.Filtering {
	public abstract class BaseControlCreator : IControlCreator {
		public abstract IEditorGeneratorItemWrapper CreateItem(ElementBindingInfo info);
		public abstract IEditorGeneratorGroupWrapper CreateGroup(ElementBindingInfo info);
		public virtual void BeginCreateLayout() { }
		public virtual void EndCreateLayout() { }
		public abstract void AddItemToGroup(IEditorGeneratorItemWrapper item, IEditorGeneratorGroupWrapper group);
		protected virtual Control CreateBindableControlRunTime(ElementBindingInfo elementBi) {
			RepositoryItem repository = GetRepositoryItem(elementBi);
			if(repository is RepositoryItemMemoExEdit) elementBi.EditorType = typeof(MemoEdit);
			else elementBi.EditorType = repository.GetEditorType();
			BaseEdit baseEdit = Activator.CreateInstance(elementBi.EditorType) as BaseEdit;
			baseEdit.Properties.Assign(repository);
			SetControlProperties(baseEdit, elementBi);
			return baseEdit;
		}
		public static IDesignerHost GetIDesignerHost(IContainer container) {
			if(container is IDesignerHost) return container as IDesignerHost;
			if(container is NestedContainer) return ((NestedContainer)container).Owner.Site.Container as IDesignerHost;
			return null;
		}
		protected virtual Control CreateBindableControl(ElementBindingInfo elementBi, IContainer container) {
			if(container != null)
				return CreateBindableControlDesignTime(elementBi, container);
			return CreateBindableControlRunTime(elementBi);
		}
		protected virtual Control CreateBindableControlDesignTime(ElementBindingInfo elementBi, IContainer container) {
			RepositoryItem repository = GetRepositoryItem(elementBi);
			if(repository is RepositoryItemMemoExEdit) elementBi.EditorType = typeof(MemoEdit);
			else elementBi.EditorType = repository.GetEditorType();
			BaseEdit baseEdit = GetIDesignerHost(container).CreateComponent(elementBi.EditorType) as BaseEdit;
			baseEdit.Properties.Assign(repository);
			AddControllInDesignMode(baseEdit, container);
			SetControlProperties(baseEdit, elementBi);
			return baseEdit;
		}
		void AddControllInDesignMode(Control customControl, IContainer container) {
			container.Add(customControl);
			IComponentInitializer initializer = GetIDesignerHost(container).GetDesigner(customControl) as IComponentInitializer;
			if(initializer != null) {
				bool flag = true;
				try {
					initializer.InitializeNewComponent(new Hashtable());
					flag = false;
				}
				finally {
					if(flag) {
						GetIDesignerHost(container).DestroyComponent(customControl);
					}
				}
			}
		}
		DefaultEditorsRepository defaultEditorsRepository = new DefaultEditorsRepository() { SupportEditMaskFormPrimitiveTypes = true };
		protected internal virtual RepositoryItem GetRepositoryItem(ElementBindingInfo bi) {
			RepositoryItem displayRepositoryItem = defaultEditorsRepository.GetRepositoryItem(bi.PropertyType, bi.AnnotationAttributes);
			RepositoryItem repositoryItemForEditing = defaultEditorsRepository.GetRepositoryItemForEditing(displayRepositoryItem, bi.PropertyType, bi.AnnotationAttributes);
			if(bi.ColumnOptions.ReadOnly) repositoryItemForEditing.ReadOnly = true;
			if(bi.ColumnOptions.IsFarAlignedByDefault)
				repositoryItemForEditing.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
			return repositoryItemForEditing;
		}
		protected virtual string GetControlName(Control control, ElementBindingInfo elementBi) {
			Type type = control.GetType();
			string typeName = type.ToString();
			string typeSName = typeName.Substring(typeName.LastIndexOf(".") + 1);
			return elementBi.DataMember + typeSName;
		}
		protected virtual void SetControlProperties(Control control, ElementBindingInfo elementBi) {
			if(control == null) throw new Exception("Internal error:cant create instance of" + elementBi.EditorType);
			Binding binding = GetBinding(control, elementBi);
			control.DataBindings.Add(binding);
			control.Name = GetControlName(control, elementBi);
			if(control.Site != null) {
				try {
					control.Site.Name = control.Name;
				}
				catch { }
				finally { }
			}
			control.Visible = false;
		}
		public object DataSource { get; set; }
		public string DataMember { get; set; }
		private Binding GetBinding(Control control, ElementBindingInfo elementBi) {
			string datamember = MakeDataMemberFullName(DataMember, elementBi.DataMember);
			Binding binding = new Binding(elementBi.BoundPropertyName, DataSource, datamember, true, elementBi.DataSourceUpdateMode);
			binding.DataSourceNullValue = elementBi.DataSourceNullValue;
			return binding;
		}
		static string MakeDataMemberFullName(string dataMember, string name) {
			return dataMember + (String.IsNullOrEmpty(dataMember) ? String.Empty : ".") + name;
		}
		public virtual void BestSize() { }
	}
}
