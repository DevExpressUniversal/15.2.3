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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Filtering;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	public class DataAccessFilterEditor : Control {
		public static readonly DependencyProperty FilterStringProperty;
		public static readonly DependencyProperty WizardFilterCriteriaProperty;
		public static readonly DependencyProperty ShowOperandTypeIconProperty;
		public static readonly DependencyProperty ShowGroupCommandsIconProperty;
		public static readonly DependencyProperty ObjectTypeProperty;
		public static readonly DependencyProperty AdditionalPropertiesProperty;
		static DataAccessFilterEditor() {
			DependencyPropertyRegistrator<DataAccessFilterEditor>.New()
				.Register(d => d.FilterString, out FilterStringProperty, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
				.Register(d => d.WizardFilterCriteria, out WizardFilterCriteriaProperty, null, d => d.OnWizardFilterCriteriaChanged())
				.Register(d => d.ShowOperandTypeIcon, out ShowOperandTypeIconProperty, false)
				.Register(d => d.ShowGroupCommandsIcon, out ShowGroupCommandsIconProperty, false)
				.Register(d => d.AdditionalProperties, out AdditionalPropertiesProperty, new PropertyInfoCollection(), d => d.OnAdditionalPropertiesChanged())
				.Register(d => d.ObjectType, out ObjectTypeProperty, null, d => d.OnObjectTypeChanged())
				.OverrideDefaultStyleKey()
			;
		}
		public DataAccessFilterEditor() {
			locker = new Locker();
			propertyList = new List<IBoundProperty>();
		}
		readonly Locker locker;
		readonly List<IBoundProperty> propertyList;
		Lazy<ErrorsEvaluatorCriteriaValidator> criteriaValidator;
		public string FilterString {
			get { return (string)GetValue(FilterStringProperty); }
			set { SetValue(FilterStringProperty, value); }
		}
		public CriteriaOperator WizardFilterCriteria {
			get { return (CriteriaOperator)GetValue(WizardFilterCriteriaProperty); }
			set { SetValue(WizardFilterCriteriaProperty, value); }
		}
		void OnWizardFilterCriteriaChanged() {
			locker.DoLockedAction(() => SetCurrentValue(FilterStringProperty, CriteriaOperator.ToString(WizardFilterCriteria)));
		}
		public bool ShowOperandTypeIcon {
			get { return (bool)GetValue(ShowOperandTypeIconProperty); }
			set { SetValue(ShowOperandTypeIconProperty, value); }
		}
		public bool ShowGroupCommandsIcon {
			get { return (bool)GetValue(ShowGroupCommandsIconProperty); }
			set { SetValue(ShowGroupCommandsIconProperty, value); }
		}
		public PropertyInfoCollection AdditionalProperties {
			get { return (PropertyInfoCollection)GetValue(AdditionalPropertiesProperty); }
			set { SetValue(AdditionalPropertiesProperty, value); }
		}
		void OnAdditionalPropertiesChanged() {
			if(AdditionalProperties == null)
				return;
			foreach(var property in AdditionalProperties)
				propertyList.Add(new BoundPropertyData(property.Name, property.Type));
			ValidateAndSet(propertyList);
		}
		public Type ObjectType {
			get { return (Type)GetValue(ObjectTypeProperty); }
			set { SetValue(ObjectTypeProperty, value); }
		}
		void OnObjectTypeChanged() {
			propertyList.Clear();
			foreach(var property in ObjectType.GetProperties())
				propertyList.Add(new BoundPropertyData(property.Name, property.PropertyType));
			ValidateAndSet(propertyList);
		}
		void ValidateAndSet(IEnumerable<IBoundProperty> properties) {
			if(propertyList.Count == 0)
				return;
			criteriaValidator = new Lazy<ErrorsEvaluatorCriteriaValidator>(() => {
				return new ErrorsEvaluatorCriteriaValidator(propertyList);
			});
			locker.DoLockedActionIfNotLocked(() => {
				WizardFilterCriteria = CriteriaOperator.Parse(FilterString);
				if(!IsFilterCriteriaValid(WizardFilterCriteria))
					WizardFilterCriteria = null;
			});
		}
		bool IsFilterCriteriaValid(CriteriaOperator criteria) {
			criteriaValidator.Value.Validate(criteria);
			return criteriaValidator.Value.Count == 0;
		}
	}
}
