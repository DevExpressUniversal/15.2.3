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
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
namespace DevExpress.Charts.Designer.Native {
	public abstract class PropertyGridModelBase {
		ChartModelElement modelElement;
		WpfChartModel chartModel;
		protected internal ChartModelElement ModelElement { get { return modelElement; } }
		protected WpfChartModel ChartModel { get { return chartModel; } }
		protected abstract ICommand SetObjectPropertyCommand { get; }
		protected virtual ICommand SetObjectAttachedPropertyCommand { get { return null; } }
		public PropertyGridModelBase(ChartModelElement modelElement) {
			UpdateModelElementCore(modelElement, false);
		}
		void UpdateModelElementCore(ChartModelElement modelElement, bool updatePropertyGridModel) {
			if (modelElement != null) {
				this.modelElement = modelElement;
				this.chartModel = (WpfChartModel)(modelElement is WpfChartModel ? modelElement : modelElement.GetParent<WpfChartModel>());
				UpdateCommands();
				if (updatePropertyGridModel)
					modelElement.UpdatePropertyGridModel(this);
			}
		}
		void UpdateComplexProperties() {
			Type modelType = this.GetType();
			PropertyInfo[] propertiesInfo = modelType.GetProperties();
			foreach (PropertyInfo propertyInfo in propertiesInfo) {
				object property = propertyInfo.GetValue(this, new object[0]);
				if (property is PropertyGridModelBase && property != null)
					((PropertyGridModelBase)property).Update();
			}
		}
		protected virtual void SetProperty(string propertyName, object value) {
			PropertyGridCommandParameter parameter = new PropertyGridCommandParameter(propertyName, value, ModelElement);
			SetObjectPropertyCommand.Execute(parameter);
		}
		protected void SetProperty(string propertyName, object value, ChartModelElement model) {
			PropertyGridCommandParameter parameter = new PropertyGridCommandParameter(propertyName, value, model);
			SetObjectPropertyCommand.Execute(parameter);
		}
		protected virtual void SetAttachedProperty(string propertyName, object value, Type propertyOwnerType, SetAttached setAttachedProperty, GetAttached getAttachedProperty) {
			PropertyGridCommandParameterAttached parameter = new PropertyGridCommandParameterAttached(propertyName, value, propertyOwnerType, setAttachedProperty, getAttachedProperty);
			SetObjectAttachedPropertyCommand.Execute(parameter);
		}
		protected void SetAttachedProperty(string propertyName, object value, ChartModelElement model, Type propertyOwnerType, SetAttached setAttachedProperty, GetAttached getAttachedProperty) {
			PropertyGridCommandParameterAttached parameter = new PropertyGridCommandParameterAttached(propertyName, value, model, propertyOwnerType, setAttachedProperty, getAttachedProperty);
			SetObjectAttachedPropertyCommand.Execute(parameter);
		}
		protected void SetProperty(ChartCommandBase command, object value) {
			((ICommand)command).Execute(value);
		}
		protected virtual void UpdateInternal() { }
		protected virtual void UpdateCommands() { }
		public void Update() {
			UpdateInternal();
			UpdateComplexProperties();
		}
		public void UpdateModelElement(ChartModelElement modelElement) {
			UpdateModelElementCore(modelElement, true);
		}
	}
	public abstract class NestedElementPropertyGridModelBase : PropertyGridModelBase {
		readonly string propertyPath;
		public NestedElementPropertyGridModelBase(ChartModelElement modelElement, string propertyPath)
			: base(modelElement) {
			this.propertyPath = propertyPath;
		}
		protected override void SetProperty(string propertyName, object value) {
			base.SetProperty(propertyPath + propertyName, value);
		}
		protected override void SetAttachedProperty(string propertyName, object value, Type propertyOwnerType, SetAttached setAttachedProperty, GetAttached getAttachedProperty) {
			PropertyGridCommandParameterAttached parameter = new PropertyGridCommandParameterAttached(propertyPath.Substring(0, propertyPath.Length - 1), propertyName, value, ModelElement, propertyOwnerType, setAttachedProperty, getAttachedProperty);
			SetObjectAttachedPropertyCommand.Execute(parameter);
		}
	}
	public abstract class PropertyGridModelCollectionBase : ObservableCollection<PropertyGridModelBase> {
	}
}
