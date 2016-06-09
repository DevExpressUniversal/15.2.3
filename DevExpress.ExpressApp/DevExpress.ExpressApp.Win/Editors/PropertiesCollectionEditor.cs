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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Events;
using DevExpress.XtraVerticalGrid.Rows;
namespace DevExpress.ExpressApp.Win.Editors {
	internal class CustomTypeDescriptor_ : CustomTypeDescriptor {
		public List<PropertyDescriptor> PropertyDescriptors = new List<PropertyDescriptor>();
		public override PropertyDescriptorCollection GetProperties() {
			return new PropertyDescriptorCollection(PropertyDescriptors.ToArray());
		}
	}
	internal class PropertyBagTypeDescriptionProvider : TypeDescriptionProvider {
		private CustomTypeDescriptor_ customTypeDescriptor = new CustomTypeDescriptor_();
		private Type describedType;
		public PropertyBagTypeDescriptionProvider(Type describedType) {
			this.describedType = describedType;
		}
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) {
			if(describedType == objectType) {
				return customTypeDescriptor;
			}
			return base.GetTypeDescriptor(objectType, instance);
		}
		public void RegisterObject(ITypedList propertyValue) {
			PropertyDescriptorCollection propertyDescriptorCollection = propertyValue.GetItemProperties(null);
			foreach(PropertyDescriptor propertyDescriptor in propertyDescriptorCollection) {
				if(!customTypeDescriptor.PropertyDescriptors.Contains(propertyDescriptor)) {
					customTypeDescriptor.PropertyDescriptors.Add(propertyDescriptor);
				}
			}
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(propertyValue.GetType());
			XafTypesInfo.Instance.RefreshInfo(typeInfo);
		}
	}
	internal class DataSourceWrapper : ITypedList {
		private ITypedList owner;
		public DataSourceWrapper(ITypedList owner) {
			if(owner == null) {
				throw new ArgumentNullException("owner");
			}
			this.owner = owner;
		}
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			foreach(PropertyDescriptor pd in owner.GetItemProperties(listAccessors)) {
				list.Add(new PropertyDescriptorWrapper(owner, pd));
			}
			PropertyDescriptorCollection collection = new PropertyDescriptorCollection(list.ToArray());
			return collection;
		}
		public string GetListName(PropertyDescriptor[] listAccessors) {
			return owner.GetListName(listAccessors);
		}
	}
	internal class PropertyDescriptorWrapper : PropertyDescriptor {
		private ITypedList owner;
		private PropertyDescriptor propertyDescriptor;
		public PropertyDescriptorWrapper(ITypedList owner, PropertyDescriptor propertyDescriptor)
			: base(propertyDescriptor.Name, null) {
			this.owner = owner;
			this.propertyDescriptor = propertyDescriptor;
		}
		public override bool CanResetValue(object component) {
			return propertyDescriptor.CanResetValue(component);
		}
		public override void ResetValue(object component) {
			propertyDescriptor.ResetValue(component);
		}
		public override bool ShouldSerializeValue(object component) {
			return propertyDescriptor.ShouldSerializeValue(component);
		}
		public override object GetValue(object component) {
			return propertyDescriptor.GetValue(owner);
		}
		public override void SetValue(object component, object value) {
			propertyDescriptor.SetValue(owner, value);
		}
		public override Type ComponentType {
			get { return propertyDescriptor.ComponentType; }
		}
		public override bool IsReadOnly {
			get { return propertyDescriptor.IsReadOnly; }
		}
		public override Type PropertyType {
			get { return propertyDescriptor.PropertyType; }
		}
	}
	public class PropertiesCollectionEditor : PropertyEditor, IComplexViewItem{
		static LightDictionary<Type, PropertyBagTypeDescriptionProvider> registredTypes = new LightDictionary<Type, PropertyBagTypeDescriptionProvider>();
		private VGridControl verticalGrid;
		private RepositoryEditorsFactory repositoryFactory;
		private void CreateRows(ITypedList propertyValue) {
			PropertyDescriptorCollection propertyDescriptorCollection = propertyValue.GetItemProperties(null);
			IModelDetailView detailView = TempDetailViewHelper.CreateTempDetailViewModel(Model.Application, propertyValue.GetType());
			foreach(PropertyDescriptor propertyDescriptor in propertyDescriptorCollection) {
				EditorRow row = new EditorRow();
				row.Properties.FieldName = propertyDescriptor.Name;
				if(!string.IsNullOrEmpty(propertyDescriptor.Description)) {
					row.Properties.Caption = propertyDescriptor.Description;
				}
				else {
					row.Properties.Caption = propertyDescriptor.Name;
				}
				row.Properties.ReadOnly = propertyDescriptor.IsReadOnly;
				RepositoryItem repositoryItem = repositoryFactory.CreateRepositoryItem(false, (IModelMemberViewItem)detailView.Items[row.Properties.FieldName], propertyValue.GetType());
				if(repositoryItem != null) {
					repositoryItem.EditValueChanged += new EventHandler(repositoryItem_EditValueChanged);
					row.Properties.RowEdit = repositoryItem;
				}
				verticalGrid.Rows.Add(row);
			}
		}
		private void ClearRows() {
			foreach(BaseRow baseRow in verticalGrid.Rows) {
				EditorRow row = baseRow as EditorRow;
				if(row != null && row.Properties.RowEdit != null) {
					row.Properties.RowEdit.EditValueChanged -= new EventHandler(repositoryItem_EditValueChanged);
				}
			}
			verticalGrid.Rows.Clear();
		}
		private void verticalGrid_HandleCreated(object sender, EventArgs e) {
			ReadValue();
		}
		private void verticalGrid_VisibleChanged(object sender, EventArgs e) {
			ReadValue();
		}
		private void repositoryItem_EditValueChanged(object sender, EventArgs e) {
			OnControlValueChanged();
		}
		private void verticalGrid_CellValueChanged(object sender, CellValueChangedEventArgs e) {
			OnControlValueChanged();
		}
		protected override object CreateControlCore() {
			verticalGrid = new VGridControl();
			verticalGrid.Dock = DockStyle.Fill;
			verticalGrid.LayoutStyle = LayoutViewStyle.SingleRecordView;
			verticalGrid.HandleCreated += new EventHandler(verticalGrid_HandleCreated);
			verticalGrid.VisibleChanged += new EventHandler(verticalGrid_VisibleChanged);
			verticalGrid.CellValueChanged += new CellValueChangedEventHandler(verticalGrid_CellValueChanged);
			return verticalGrid;
		}
		protected override object GetControlValueCore() {
			if((verticalGrid != null) && (verticalGrid.DataSource != null) && (MemberInfo != null) && (CurrentObject != null)) {
				return MemberInfo.GetValue(CurrentObject);
			}
			return null;
		}
		public PropertiesCollectionEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
		protected override void SetTestTag() {
			verticalGrid.Tag = EasyTestTagHelper.FormatTestField(Caption);
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					repositoryFactory = null;
					if(verticalGrid != null) {
						ClearRows();
						verticalGrid.HandleCreated -= new EventHandler(verticalGrid_HandleCreated);
						verticalGrid.VisibleChanged -= new EventHandler(verticalGrid_VisibleChanged);
						verticalGrid.CellValueChanged -= new CellValueChangedEventHandler(verticalGrid_CellValueChanged);
						verticalGrid.Dispose();
						verticalGrid = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected override void ReadValueCore() {
			if(verticalGrid != null && verticalGrid.FindForm() != null && verticalGrid.Visible) {
				ITypedList propertyValue = (ITypedList)MemberInfo.GetValue(CurrentObject);
				ClearRows();
				if(propertyValue != null) {
					ITypedList wrappedPropertyValue = new DataSourceWrapper(propertyValue);
					PropertyBagTypeDescriptionProvider typeDescriptorProvider = registredTypes[propertyValue.GetType()];
					if(typeDescriptorProvider == null) {
						typeDescriptorProvider = new PropertyBagTypeDescriptionProvider(wrappedPropertyValue.GetType());
						TypeDescriptor.AddProvider(typeDescriptorProvider, wrappedPropertyValue.GetType());
					}
					typeDescriptorProvider.RegisterObject(wrappedPropertyValue);
					CreateRows(wrappedPropertyValue);
					verticalGrid.DataSource = new object[] { wrappedPropertyValue };
				}
				else {
					verticalGrid.DataSource = null;
				}
				verticalGrid.LayoutStyle = LayoutViewStyle.SingleRecordView;
			}
		}
		public new VGridControl Control {
			get {return verticalGrid; }
		}
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			repositoryFactory = new RepositoryEditorsFactory(application, objectSpace);
		}
	}
}
