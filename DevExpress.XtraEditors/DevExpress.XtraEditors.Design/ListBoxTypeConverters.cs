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
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ListControls;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils;
using DevExpress.Data;
namespace DevExpress.XtraEditors.Design {
	public abstract class ItemTypeConverter : ExpandableObjectConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor)) { return true; }
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) { throw new ArgumentNullException("destinationType"); }
			if(destinationType.Equals(typeof(InstanceDescriptor))) {
				return GetInstanceDescriptor(value);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		protected abstract InstanceDescriptor GetInstanceDescriptor(object value);
	}
	public class ListBoxItemTypeConverter : ItemTypeConverter {
		protected override InstanceDescriptor GetInstanceDescriptor(object value) {
			ListBoxItem item = (ListBoxItem)value;
			ConstructorInfo ctor = null;
			object[] parameters = null;
			if(item.Tag != null) {
				ctor = typeof(ListBoxItem).GetConstructor(new Type[] { typeof(string) });
				parameters = new object[] { item.Value };
			}
			else {
				ctor = typeof(ListBoxItem).GetConstructor(new Type[] { typeof(string), typeof(object) });
				parameters = new object[] { item.Value, item.Tag };
			}
			return new InstanceDescriptor(ctor, parameters);
		}
	}
	public class CheckedListBoxItemTypeConverter : ItemTypeConverter {
		protected override InstanceDescriptor GetInstanceDescriptor(object value) {
			CheckedListBoxItem item = (CheckedListBoxItem)value;
			ConstructorInfo ctor = null;
			object[] parameters = null;
			if(item.Tag != null) {
				if(item.CheckState != CheckState.Unchecked) {
					ctor = typeof(CheckedListBoxItem).GetConstructor(new Type[] { typeof(string), typeof(string), typeof(CheckState), typeof(object) });
					parameters = new object[] { item.Value, item.Description, item.CheckState, item.Tag };
				}
				else {
					ctor = typeof(CheckedListBoxItem).GetConstructor(new Type[] { typeof(string), typeof(string), typeof(object) });
					parameters = new object[] { item.Value, item.Description, item.Tag };
				}
				return new InstanceDescriptor(ctor, parameters);
			}
			if(item.Enabled) {
				if(item.CheckState == CheckState.Unchecked) {
					if(string.IsNullOrEmpty(item.Description)) {
						ctor = typeof(CheckedListBoxItem).GetConstructor(new Type[] { typeof(string) });
						parameters = new object[] { item.Value };
					}
					else {
						ctor = typeof(CheckedListBoxItem).GetConstructor(new Type[] { typeof(string), typeof(string) });
						parameters = new object[] { item.Value, item.Description };
					}
				}
				else {
					if(string.IsNullOrEmpty(item.Description)) {
						ctor = typeof(CheckedListBoxItem).GetConstructor(new Type[] { typeof(string), typeof(CheckState) });
						parameters = new object[] { item.Value, item.CheckState };
					}
					else {
						ctor = typeof(CheckedListBoxItem).GetConstructor(new Type[] { typeof(string), typeof(string), typeof(CheckState) });
						parameters = new object[] { item.Value, item.Description, item.CheckState };
					}
				}
			}
			else {
				if(string.IsNullOrEmpty(item.Description)) {
					ctor = typeof(CheckedListBoxItem).GetConstructor(new Type[] { typeof(string), typeof(CheckState), typeof(bool) });
					parameters = new object[] { item.Value, item.CheckState, item.Enabled };
				}
				else {
					ctor = typeof(CheckedListBoxItem).GetConstructor(new Type[] { typeof(string), typeof(string), typeof(CheckState), typeof(bool) });
					parameters = new object[] { item.Value, item.Description, item.CheckState, item.Enabled };
				}
			}
			return new InstanceDescriptor(ctor, parameters);
		}
	}
	public class ImageListBoxItemTypeConverter : ItemTypeConverter {
		protected override InstanceDescriptor GetInstanceDescriptor(object value) {
			ImageListBoxItem item = (ImageListBoxItem)value;
			ConstructorInfo ctor = null;
			object[] parameters = null;
			if(item.Tag != null) {
				if(item.ImageIndex > -1) {
					ctor = typeof(ImageListBoxItem).GetConstructor(new Type[] { typeof(string), typeof(string), typeof(int), typeof(object) });
					parameters = new object[] { item.Value, item.Description, item.ImageIndex, item.Tag };
				}
				else {
					ctor = typeof(ImageListBoxItem).GetConstructor(new Type[] { typeof(string), typeof(string), typeof(object) });
					parameters = new object[] { item.Value, item.Description, item.Tag };
				}
				return new InstanceDescriptor(ctor, parameters);
			}
			if(item.ImageIndex < 0) {
				if(string.IsNullOrEmpty(item.Description)) {
					ctor = typeof(ImageListBoxItem).GetConstructor(new Type[] { typeof(string) });
					parameters = new object[] { item.Value };
				}
				else {
					ctor = typeof(ImageListBoxItem).GetConstructor(new Type[] { typeof(string), typeof(string) });
					parameters = new object[] { item.Value, item.Description };
				}
			}
			else {
				if(string.IsNullOrEmpty(item.Description)) {
					ctor = typeof(ImageListBoxItem).GetConstructor(new Type[] { typeof(string), typeof(int) });
					parameters = new object[] { item.Value, item.ImageIndex };
				}
				else {
					ctor = typeof(ImageListBoxItem).GetConstructor(new Type[] { typeof(string), typeof(string), typeof(int) });
					parameters = new object[] { item.Value, item.Description, item.ImageIndex };
				}
			}
			return new InstanceDescriptor(ctor, parameters);
		}
	}
	public class RadioGroupItemTypeConverter : ItemTypeConverter {
		protected override InstanceDescriptor GetInstanceDescriptor(object value) {
			RadioGroupItem item = (RadioGroupItem)value;
			ConstructorInfo ctor = null;
			object[] parameters = null;
			if(item.Value == null && item.Description == string.Empty && item.Enabled && item.Tag == null) {
				ctor = typeof(RadioGroupItem).GetConstructor(new Type[] { });
				parameters = new object[] { };
			}
			else {
				if(item.Tag != null) {
					ctor = typeof(RadioGroupItem).GetConstructor(new Type[] { typeof(object), typeof(string), typeof(bool), typeof(object) });
					parameters = new object[] { item.Value, item.Description, item.Enabled, item.Tag };
				}
				else if(item.Enabled) {
					ctor = typeof(RadioGroupItem).GetConstructor(new Type[] { typeof(object), typeof(string) });
					parameters = new object[] { item.Value, item.Description };
				}
				else {
					ctor = typeof(RadioGroupItem).GetConstructor(new Type[] { typeof(object), typeof(string), typeof(bool) });
					parameters = new object[] { item.Value, item.Description, item.Enabled };
				}
			}
			return new InstanceDescriptor(ctor, parameters);
		}
	}
	public class DataMemberTypeConverter : StringConverter {
		protected const string noField = "(None)";
		protected static readonly StandardValuesCollection none = new StandardValuesCollection(new string[] { noField });
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return GetFieldList(context);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool IsValid(ITypeDescriptorContext context, object value) {
			return true;
		}
		protected virtual StandardValuesCollection GetFieldList(ITypeDescriptorContext context) {
			if(context == null) return none;
			object dataSource = ExtractDataSource(context);
			DataColumnInfo[] columns = new DevExpress.Data.Helpers.MasterDetailHelper().GetDataColumnInfo(dataSource);
			if(columns == null || columns.Length == 0) return none;
			string[] fields = new string[columns.Length + 1];
			for(int i = 0; i < columns.Length; i++) {
				fields[i + 1] = columns[i].Name;
			}
			fields[0] = noField;
			return new StandardValuesCollection(fields);
		}
		protected virtual object ExtractDataSource(ITypeDescriptorContext context) {
			PropertyDescriptor dataSourceDescriptor = TypeDescriptor.GetProperties(context.Instance)["DataSource"];
			if(dataSourceDescriptor == null) return null;
			return dataSourceDescriptor.GetValue(context.Instance);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType.Equals(typeof(string))) {
				if(value == null || value.ToString() == string.Empty) return noField;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value == null) return none;
			if(value is string) {
				string source = value.ToString();
				if(source == noField) return string.Empty;
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
	public class LookUpColumnDataMemberTypeConverter : DataMemberTypeConverter {
		protected override object ExtractDataSource(ITypeDescriptorContext context) {
			RepositoryItemLookUpEdit propItem = GetEditProperties(context.Instance);
			return (propItem == null ? null : propItem.DataSource);
		}
		private RepositoryItemLookUpEdit GetEditProperties(object selectedObject) {
			LookUpColumnInfo ci = selectedObject as LookUpColumnInfo;
			if(ci != null)
				return ci.GetOwner() as RepositoryItemLookUpEdit;
			return null;
		}
	}
	public class LookUpColumnInfoCollectionEditor : DevExpress.Utils.Design.DXCollectionEditorBase {
		public LookUpColumnInfoCollectionEditor(Type type) : base(type) { }
		Utils.Design.DXCollectionEditorBase.DXCollectionEditorBaseForm form;
		protected override Utils.Design.DXCollectionEditorBase.DXCollectionEditorBaseForm CreateCollectionForm() {
			form = new LookUpColumnInfoEditorForm(this);
			return form;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			LookUpColumnInfoCollection originalColumnsContent = new LookUpColumnInfoCollection();
			originalColumnsContent.Assign(value as LookUpColumnInfoCollection);
			LookUpColumnInfoEditorForm.EditContext = context;
			LookUpColumnInfoCollection result = base.EditValue(context, provider, value) as LookUpColumnInfoCollection;
			if(result != null && form.DialogResult == DialogResult.Cancel)
				result.Assign(originalColumnsContent);
			return result;
		}
		protected override string GetDisplayText(object value) {
			return value.ToString();
		}
		protected override bool AllowLiveUpdates {
			get { return true; }
		}
		class LookUpColumnInfoEditorFormContent : DevExpress.Utils.Design.DXCollectionEditorContent {
			SimpleButton populateColumnsButton;
			ITypeDescriptorContext contextCore;
			DevExpress.Utils.Design.DXCollectionEditorBase.DXCollectionEditorBaseForm owner;
			public LookUpColumnInfoEditorFormContent(DevExpress.Utils.Design.DXCollectionEditorBase.DXCollectionEditorBaseForm ownerForm, ITypeDescriptorContext context)
				: base() {
				this.contextCore = context;
				this.owner = ownerForm;
				InitPopulateButton();
				this.AddOtherButton(populateColumnsButton);
			}
			void InitPopulateButton() {
				if(populateColumnsButton != null && !populateColumnsButton.IsDisposed) return;
				populateColumnsButton = new SimpleButton();
				populateColumnsButton.Name = "btRetrFields";
				populateColumnsButton.TabIndex = 12;
				populateColumnsButton.Text = Properties.Resources.PopulateColumnsCaption;
				populateColumnsButton.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
				populateColumnsButton.Click += new System.EventHandler(this.PopulateColumns_Click);
			}
			void PopulateColumns_Click(object sender, EventArgs e) {
				RepositoryItemLookUpEdit item = GetItem();
				if(item == null) return;
				item.ForceInitialize();
				item.PopulateColumns();
				owner.EditValue = null;
				owner.EditValue = item.Columns;
				this.RefreshContent();
			}
			RepositoryItemLookUpEdit GetItem() {
				if(this.contextCore == null) return null;
				if(this.contextCore.Instance is LookUpEdit) return ((LookUpEdit)this.contextCore.Instance).Properties;
				return (this.contextCore.Instance as RepositoryItemLookUpEdit);
			}
		}
		class LookUpColumnInfoEditorForm : DevExpress.Utils.Design.DXCollectionEditorBase.DXCollectionEditorBaseForm {
			static ITypeDescriptorContext contextCore;
			public LookUpColumnInfoEditorForm(DevExpress.Utils.Design.DXCollectionEditorBase collectionEditor) : base(collectionEditor) { }
			protected override Utils.Design.DXCollectionEditorContent CreateContentControl() {
				return new LookUpColumnInfoEditorFormContent(this, contextCore);
			}
			public static ITypeDescriptorContext EditContext {
				get { return LookUpColumnInfoEditorForm.contextCore; }
				set { LookUpColumnInfoEditorForm.contextCore = value; }
			}
		}
	}
	public class LookUpColumnInfoTypeConverter : ExpandableObjectConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor)) return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) { throw new ArgumentNullException("destinationType"); }
			if(destinationType == typeof(InstanceDescriptor)) {
				LookUpColumnInfo item = (LookUpColumnInfo)value;
				ConstructorInfo ctor = null;
				int mask = 0;
				object[] parameters = null;
				if(item.FieldName != string.Empty) mask |= 1;
				if(item.Caption != string.Empty) mask |= 2;
				if(item.Width != 20) mask |= 4;
				if(item.FormatType != FormatType.None) mask |= 8;
				if(item.FormatString != string.Empty) mask |= 16;
				if(!item.Visible) mask |= 32;
				if(item.Alignment != HorzAlignment.Default) mask |= 64;
				if(item.SortOrder != ColumnSortOrder.None) mask |= 128;
				switch(mask) {
					case 0:
						ctor = typeof(LookUpColumnInfo).GetConstructor(new Type[] { });
						parameters = null;
						break;
					case 1:
						ctor = typeof(LookUpColumnInfo).GetConstructor(new Type[] { typeof(string) });
						parameters = new object[] { item.FieldName };
						break;
					case 7:
						ctor = typeof(LookUpColumnInfo).GetConstructor(new Type[] { typeof(string), typeof(string), typeof(int) });
						parameters = new object[] { item.FieldName, item.Caption, item.Width };
						break;
					case 71:
						ctor = typeof(LookUpColumnInfo).GetConstructor(new Type[] { typeof(string), typeof(string), typeof(int), typeof(FormatType), typeof(string), typeof(bool), typeof(HorzAlignment) });
						parameters = new object[] { item.FieldName, item.Caption, item.Width, item.FormatType, item.FormatString, item.Visible, item.Alignment };
						break;
					default:
						ctor = typeof(LookUpColumnInfo).GetConstructor(new Type[] { typeof(string), typeof(string), typeof(int), typeof(FormatType), typeof(string), typeof(bool), typeof(HorzAlignment), typeof(ColumnSortOrder) });
						parameters = new object[] { item.FieldName, item.Caption, item.Width, item.FormatType, item.FormatString, item.Visible, item.Alignment, item.SortOrder };
						break;
				}
				if(ctor != null)
					return new InstanceDescriptor(ctor, parameters);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
