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

using System.ComponentModel;
using System;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Exceptions;
using DevExpress.XtraReports.UI;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using DevExpress.Services.Internal;
using DevExpress.XtraReports.Native;
using DevExpress.Compatibility.System.ComponentModel;
using System.Globalization;
namespace DevExpress.Data.Browsing {
	public class CalculatedPropertyDescriptorBase : PropertyDescriptor, IContainerComponent {
		bool isEvaluated;
		Type propertyType;
		string displayName;
		ExpressionEvaluator evaluator;
		protected ICalculatedField calculatedField;
		public override Type PropertyType { get { return propertyType; } }
		public override TypeConverter Converter { get { return TypeDescriptor.GetConverter(propertyType); } }
		public override Type ComponentType { get { return typeof(IEnumerable<ICalculatedField>); } }
		public override string DisplayName {
			get {
				return displayName;
			}
		}
		public CalculatedPropertyDescriptorBase(ICalculatedField calculatedField, IEnumerable<IParameter> parameters, DataContext dataContext)
			: this(calculatedField, new CalculatedEvaluatorContextDescriptor(parameters, calculatedField, dataContext)) {
		}
		public CalculatedPropertyDescriptorBase(ICalculatedField calculatedField, IEnumerable<IParameter> parameters)
			: this(calculatedField, new CalculatedEvaluatorContextDescriptor(parameters, calculatedField, null)) {
		}
		public CalculatedPropertyDescriptorBase(ICalculatedField calculatedField, DataContext dataContext)
			: this(calculatedField, new CalculatedEvaluatorContextDescriptor(null, calculatedField, dataContext)) {
		}
		public CalculatedPropertyDescriptorBase(ICalculatedField calculatedField)
			: this(calculatedField, new CalculatedEvaluatorContextDescriptor(null, calculatedField, null)) {
		}
		protected CalculatedPropertyDescriptorBase(ICalculatedField calculatedField, CalculatedEvaluatorContextDescriptor descriptor)
			: base(calculatedField.Name, null) {
			this.calculatedField = calculatedField;
			this.propertyType = FieldTypeConverter.ToType(calculatedField.FieldType);
			this.displayName = calculatedField.DisplayName;
			CriteriaOperator criteriaOperator;
			try {
				criteriaOperator = CriteriaOperator.Parse(calculatedField.Expression, null);
			}
			catch (CriteriaParserException) {
				criteriaOperator = CriteriaOperator.Parse(string.Empty, null);
			}
			evaluator = new ExpressionEvaluator(descriptor, criteriaOperator, true);
		}
		public override bool CanResetValue(object component) {
			return false;
		}
		public override void ResetValue(object component) {
		}
		public override void SetValue(object component, object value) {
		}
		public override bool ShouldSerializeValue(object component) {
			return false;
		}
		public override bool IsReadOnly { get { return true; } }
		public override object GetValue(object component) {
			try {
				if(isEvaluated)
					throw new CalculatedPropertyException(string.Format("Calculated property '{0}' is evaluated recursive.", this.Name));
				isEvaluated = true;
				object value = evaluator.Evaluate(component);
				return ConvertToType(value, this.propertyType);
			} catch(CalculatedPropertyException) {
				throw;
			} catch {
				return null;
			} finally {
				isEvaluated = false;
			}
		}
		static object ConvertToType(object value, Type type) {
			if(value == null || type == typeof(object))
				return value;
			if(type == typeof(string) && !(value is IConvertible)) {
				return Convert.ToString(value, CultureInfo.CurrentCulture);
			}
			return Convert.ChangeType(value, type, CultureInfo.CurrentCulture);
		}
		#region IContainerComponent Members
		object IContainerComponent.Component {
			get { return calculatedField; }
		}
		#endregion
	}
	public class CalculatedPropertyException : Exception {
		public CalculatedPropertyException(string message) : base(message) { 
		}
	}
}
