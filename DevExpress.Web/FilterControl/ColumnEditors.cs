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
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using DevExpress.Web.Internal;
using System.Web.UI;
using System.Linq;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.XtraEditors.Filtering;
using DevExpress.Data.Filtering.Helpers;
namespace DevExpress.Web {
	[ControlBuilder(typeof(ControlBuilder))]
	public abstract class FilterControlEditColumn : FilterControlColumn {
		internal static FilterControlEditColumn CreateColumn(Type dataType) {
			if(dataType == null) return new FilterControlTextColumn();
			dataType = ReflectionUtils.StripNullableType(dataType);
			if(dataType.Equals(typeof(DateTime))) return new FilterControlDateColumn();
			if(dataType.Equals(typeof(bool))) return new FilterControlCheckColumn();
			return new FilterControlTextColumn();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false), Localizable(false), PersistenceMode(PersistenceMode.Attribute)]
		public override EditPropertiesBase PropertiesEdit {
			get {
				if(base.PropertiesEdit == null) base.PropertiesEdit = CreateEditProperties();
				return base.PropertiesEdit;
			}
			set { base.PropertiesEdit = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlEditColumnPropertiesEditType"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string PropertiesEditType {
			get { return string.Empty; }
			set { }
		}
		protected abstract EditPropertiesBase CreateEditProperties();
		protected override PropertiesBase GetDesignTimeItemEditProperties() {
			return PropertiesEdit;
		}
	}
	public class FilterControlTextColumn : FilterControlEditColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new TextBoxProperties(); }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlTextColumnPropertiesTextEdit"),
#endif
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)]
		public TextBoxProperties PropertiesTextEdit { get { return (TextBoxProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "PropertiesTextEdit" };			
		}
	}
	public class FilterControlButtonEditColumn : FilterControlEditColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new ButtonEditProperties(); }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlButtonEditColumnPropertiesButtonEdit"),
#endif
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ButtonEditProperties PropertiesButtonEdit { get { return (ButtonEditProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "PropertiesButtonEdit" };			
		}
	}
	public class FilterControlMemoColumn : FilterControlEditColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new MemoProperties(); }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlMemoColumnPropertiesMemoEdit"),
#endif
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public MemoProperties PropertiesMemoEdit { get { return (MemoProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "PropertiesMemoEdit" };			
		}
	}
	public class FilterControlHyperLinkColumn : FilterControlEditColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new HyperLinkProperties(); }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlHyperLinkColumnPropertiesHyperLinkEdit"),
#endif
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)]
		public HyperLinkProperties PropertiesHyperLinkEdit { get { return (HyperLinkProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "PropertiesHyperLinkEdit" };			
		}
	}
	public class FilterControlCheckColumn : FilterControlEditColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new CheckBoxProperties(); }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlCheckColumnPropertiesCheckEdit"),
#endif
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public CheckBoxProperties PropertiesCheckEdit { get { return (CheckBoxProperties)PropertiesEdit; } }
		protected override Type GetPropertyType() {
			if(ColumnType == FilterControlColumnType.Default)			
				return PropertiesCheckEdit.ValueType;
			return base.GetPropertyType();
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "PropertiesCheckEdit" };			
		}
	}
	public class FilterControlDateColumn : FilterControlEditColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new DateEditProperties(); }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlDateColumnPropertiesDateEdit"),
#endif
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public DateEditProperties PropertiesDateEdit { get { return (DateEditProperties)PropertiesEdit; } }
		protected override Type GetPropertyType() { return typeof(DateTime); }
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "PropertiesDateEdit" };			
		}
	}
	public class FilterControlSpinEditColumn : FilterControlEditColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new SpinEditProperties(); }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlSpinEditColumnPropertiesSpinEdit"),
#endif
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public SpinEditProperties PropertiesSpinEdit { get { return (SpinEditProperties)PropertiesEdit; } }
		protected override Type GetPropertyType() {
			Type type = base.GetPropertyType();
			if(type.IsValueType) return type;
			return PropertiesSpinEdit.NumberType == SpinEditNumberType.Float ? typeof(double) : typeof(int);
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "PropertiesSpinEdit" };			
		}
	}
	public class FilterControlComboBoxColumn : FilterControlEditColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new ComboBoxProperties(); }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlComboBoxColumnPropertiesComboBox"),
#endif
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ComboBoxProperties PropertiesComboBox { get { return (ComboBoxProperties)PropertiesEdit; } }
		protected override Type GetPropertyType() {
			if(PropertiesComboBox.ValueType != null) return PropertiesComboBox.ValueType;
			return base.GetPropertyType();
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "PropertiesComboBox" };			
		}
	}
	public class FilterControlComplexTypeColumn : FilterControlColumn {
		public FilterControlComplexTypeColumn() {
			Columns = new FilterControlColumnCollection(this);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlComplexTypeColumnColumns"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		public FilterControlColumnCollection Columns { get; private set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlComplexTypeColumnDataType"),
#endif
		NotifyParentProperty(true), DefaultValue(FilterControlDataType.Object)]
		public FilterControlDataType DataType {
			get { return (FilterControlDataType)GetEnumProperty("DataType", FilterControlDataType.Object); }
			set {
				if(value == DataType) return;
				SetEnumProperty("DataType", false, value);
				OnColumnChanged();
			}
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			FilterControlComplexTypeColumn column = source as FilterControlComplexTypeColumn;
			if(column != null) {
				DataType = column.DataType;
			}
		}
		protected new FilterControlColumnType ColumnType { get; set; }
		protected internal string ListPropertyType {
			get {
				if(DataType != FilterControlDataType.List) return string.Empty;
				if(InternalColumnType == null)
					return (this as IBoundProperty).GetFullName();
				var genericListTypeArgument = DevExpress.Data.Helpers.GenericTypeHelper.GetGenericIListTypeArgument(InternalColumnType);
				if(genericListTypeArgument != null)
					return genericListTypeArgument.FullName;
				return string.Empty;
			}
		}
		protected override bool GetIsList() {
			return DataType == FilterControlDataType.List;
		}
		protected override List<IBoundProperty> GetChildren() {
			return Columns.Cast<IBoundProperty>().ToList();
		}
		protected override FilterColumnClauseClass GetClauseClass() {
			var isList = (this as IBoundProperty).IsList;
			return isList ? FilterColumnClauseClass.Generic : FilterColumnClauseClass.Blob;
		}
		protected override IList GetDesignTimeItems() {
			return (IList)Columns;			
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() { 
			return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "Columns", "ColumnType" }); 
		}
		protected override IStateManager[] GetStateManagedObjects() {
			List<IStateManager> list = new List<IStateManager>(base.GetStateManagedObjects());
			list.Add(Columns);
			return list.ToArray();
		}
	}
}
