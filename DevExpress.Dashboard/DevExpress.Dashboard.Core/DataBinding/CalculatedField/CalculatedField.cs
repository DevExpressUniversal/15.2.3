#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Collections;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.Native;
using DevExpress.PivotGrid.CriteriaVisitors;
using System.Collections.Generic;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public class CalculatedField : INamedItem, INameContainer, ISupportPrefix {
		const string xmlName = "Name";
		const string xmlExpression = "Expression";
		const string xmlDataType = "DataType";
		const string xmlDataMember = "DataMember";
		const CalculatedFieldType DefaultDataType = CalculatedFieldType.String;
		string name;
		string expression;
		CalculatedFieldType dataType = DefaultDataType;
		string dataMember;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("CalculatedFieldName"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(null),
		Localizable(false)
		]
		public string Name {
			get { return name; }
			set {
				if(name != value) {
					if(nameChanging != null)
						nameChanging(this, new NameChangingEventArgs(value));
					object oldValue = name;
					name = value;
					OnChanged(xmlName, oldValue);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("CalculatedFieldExpression"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(null),
		Localizable(false),
		Editor(TypeNames.CalculatedFieldExpressionEditor, typeof(UITypeEditor))
		]
		public string Expression {
			get { return expression; }
			set {
				if(expression != value) {
					object oldValue = expression;
					expression = value;
					OnChanged(xmlExpression, oldValue);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("CalculatedFieldDataType"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultDataType)
		]
		public CalculatedFieldType DataType {
			get { return dataType; }
			set {
				if(dataType != value) {
					object oldValue = dataType;
					dataType = value;
					OnChanged(xmlDataType, oldValue);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("CalculatedFieldDataMember"),
#endif
		Category(CategoryNames.Data),
		Localizable(false),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CalculatedFieldDataMemberListConverter),
		DefaultValue(null)
		]
		public string DataMember {
			get { return dataMember; }
			set { 
				dataMember = value; 
			}
		}
		string ISupportPrefix.Prefix { get { return DashboardLocalizer.GetString(DashboardStringId.NewCalculatedFieldNamePrefix); } }
		event EventHandler<NameChangingEventArgs> nameChanging;
		event EventHandler<NameChangingEventArgs> INameContainer.NameChanging { add { nameChanging += value; } remove { nameChanging -= value; } }
		internal event EventHandler<CalculatedFieldChangedEventArgs> Changed;
		public CalculatedField()
			: this(null) {
		}
		public CalculatedField(string expression)
			: this(expression, null) {
		}
		public CalculatedField(string expression, string name)
			: this(expression, name, DefaultDataType) {
		}
		public CalculatedField(string dataMember, string expression, string name)
			: this(dataMember, expression, name, DefaultDataType) {
		}
		public CalculatedField(string expression, string name, CalculatedFieldType dataType)
			: this(null, expression, name, dataType) {
		}
		public CalculatedField(string dataMember, string expression, string name, CalculatedFieldType dataType) {
			this.expression = expression;
			this.name = name;
			this.dataType = dataType;
			this.DataMember = dataMember;
		}
		internal void SaveToXml(XElement element) {
			if(!string.IsNullOrEmpty(Name))
				element.Add(new XAttribute(xmlName, Name));
			if(!string.IsNullOrEmpty(Expression))
				element.Add(new XAttribute(xmlExpression, Expression));
			if(DataType != DefaultDataType)
				element.Add(new XAttribute(xmlDataType, DataType));
			if(!string.IsNullOrEmpty(DataMember)) {
				element.Add(new XAttribute(xmlDataMember, DataMember));
			}
		}
		internal void LoadFromXml(XElement element) {
			string xmlNameValue = XmlHelper.GetAttributeValue(element, xmlName);
			if(!string.IsNullOrEmpty(xmlNameValue))
				Name = xmlNameValue;
			string xmlExpressionValue = XmlHelper.GetAttributeValue(element, xmlExpression);
			if(!string.IsNullOrEmpty(xmlExpressionValue))
				Expression = xmlExpressionValue;
			string xmlDataTypeValue = XmlHelper.GetAttributeValue(element, xmlDataType);
			if(!string.IsNullOrEmpty(xmlDataTypeValue))
				DataType = XmlHelper.EnumFromString<CalculatedFieldType>(xmlDataTypeValue);
			string xmlDataMemberValue = XmlHelper.GetAttributeValue(element, xmlDataMember);
			if(!string.IsNullOrEmpty(xmlDataMemberValue))
				DataMember = xmlDataMemberValue;
		}
		internal void SetAutoType(Type type) {
			if(type == null)
				return;
			if(type == typeof(string)) {
				DataType = CalculatedFieldType.String;
				return;
			}
			if(type == typeof(int)) {
				DataType = CalculatedFieldType.Integer;
				return;
			}
			if(type == typeof(decimal) || type == typeof(float) || type == typeof(double)) {
				DataType = CalculatedFieldType.Decimal;
				return;
			}
			if(type == typeof(bool)) {
				DataType = CalculatedFieldType.Boolean;
				return;
			}
			if(type == typeof(DateTime)) {
				DataType = CalculatedFieldType.DateTime;
				return;
			}
			DataType = CalculatedFieldType.Object;
		}
		void OnChanged(string name, object oldValue) {
			if(Changed != null)
				Changed(this, new CalculatedFieldChangedEventArgs(this, name, oldValue));
		}
		internal bool CheckHasAggregate(IEnumerable<CalculatedField> collection) {
			try {
				CriteriaOperator criteria = new CalculatedFieldsExpressionExpander(collection, Name, true).Process(CriteriaOperator.Parse(Expression));
				return HasAggregateCriteriaChecker.Check(criteria);
			} catch {
				return false;
			}
		}
	}
}
namespace DevExpress.DashboardCommon.Native {
	public class CalculatedFieldChangedEventArgs : EventArgs {
		public CalculatedField Field { get; private set; }
		public string PropertyName { get; private set; }
		public object OldValue { get; private set; }
		public CalculatedFieldChangedEventArgs(CalculatedField field, string propertyName, object oldValue) {
			Field = field;
			PropertyName = propertyName;
			OldValue = oldValue;
		}
	}
}
