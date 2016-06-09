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
using System.Reflection;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public static class CommandUtils {
		static PropertyInfo GetPropertyInfo(string propertyPath, DesignerChartElementModelBase parentModel, out object targetObject) {
			string[] paths = propertyPath.Split('.');
			string[] objectPath = new string[paths.Length - 1];
			Array.Copy(paths, 0, objectPath, 0, paths.Length - 1);
			string targetPropertyName = paths[paths.Length - 1];
			targetObject = null;
			DesignerChartElementModelBase model = parentModel;
			foreach (string pathPart in objectPath) {
				model = model.GetChild(pathPart);
				if (model == null)
					return null;
			}
			if (model is ChartCollectionBaseModel)
				targetObject = ((ChartCollectionBaseModel)model).ChartCollection;
			else {
				if (model.ChartElement != null)
					targetObject = model.ChartElement;
				else
					targetObject = model.Element;
			}
			PropertyInfo propertyInfo = null;
			try {
				propertyInfo = targetObject.GetType().GetProperty(targetPropertyName);
			}
			catch {
				propertyInfo = ReflectionHelper.GetProperty(targetPropertyName, targetObject.GetType());
			}
			return propertyInfo;
		}
		public static object GetObjectProperty(DesignerChartElementModelBase parentModel, string propertyPath) {
			object targetObject = null;
			PropertyInfo targetPropertyInfo = GetPropertyInfo(propertyPath, parentModel, out targetObject);
			return targetPropertyInfo.GetValue(targetObject, new object[0]);
		}
		public static void SetObjectProperty(DesignerChartElementModelBase parentModel, string propertyPath, object value) {
			object targetObject = null;
			PropertyInfo targetPropertyInfo = GetPropertyInfo(propertyPath, parentModel, out targetObject);
			targetPropertyInfo.SetValue(targetObject, value, new object[0]);
		}
	}
	public class SetPropertyCommandParameter {
		readonly object targetObject;
		readonly string propertyName;
		readonly object value;
		public object TargetObject { get { return targetObject; } }
		public string PropertyName { get { return propertyName; } }
		public object Value { get { return value; } }
		public SetPropertyCommandParameter(object targetObject, string propertyName, object value) {
			this.targetObject = targetObject;
			this.propertyName = propertyName;
			this.value = value;
		}
	}
	public class SetObjectPropertyCommand : ChartCommandBase {
		protected internal override bool CanDisposeNewValue { get { return true; } }
		protected internal override bool CanUpdatePointsGrid { get { return true; } }
		public SetObjectPropertyCommand(CommandManager commandManager) : base(commandManager) { }
		public override bool CanExecute(object parameter) {
			return true;
		}
		public override HistoryItem ExecuteInternal(object parameter) {
			SetPropertyCommandParameter commandParameter = (SetPropertyCommandParameter)parameter;
			object oldValue = CommandUtils.GetObjectProperty(commandParameter.TargetObject as DesignerChartElementModelBase, commandParameter.PropertyName);
			object newValue;
			if (commandParameter.Value is DesignerChartElementModelBase) {
				newValue = ((DesignerChartElementModelBase)commandParameter.Value).ChartElement;
				if (newValue == null)
					newValue = ((DesignerChartElementModelBase)commandParameter.Value).Element;
			}
			else
				newValue = commandParameter.Value;
			try {
				CommandUtils.SetObjectProperty(commandParameter.TargetObject as DesignerChartElementModelBase, commandParameter.PropertyName, newValue);
			}
			catch (Exception e) {
				CommandUtils.SetObjectProperty(commandParameter.TargetObject as DesignerChartElementModelBase, commandParameter.PropertyName, oldValue);
				if (e.InnerException == null)
					XtraMessageBox.Show(e.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				else
					XtraMessageBox.Show(e.InnerException.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return null;
			}
			HistoryItem historyItem = new HistoryItem(this, oldValue, newValue, commandParameter.PropertyName);
			historyItem.TargetObject = commandParameter.TargetObject;
			return historyItem;
		}
		public override void UndoInternal(HistoryItem historyItem) {
			CommandUtils.SetObjectProperty(historyItem.TargetObject as DesignerChartElementModelBase, (string)historyItem.Parameter, historyItem.OldValue);
		}
		public override void RedoInternal(HistoryItem historyItem) {
			CommandUtils.SetObjectProperty(historyItem.TargetObject as DesignerChartElementModelBase, (string)historyItem.Parameter, historyItem.NewValue);
		}
	}
	public class BatchSetObjectPropertyCommand : ChartCommandBase {
		protected internal override bool CanDisposeNewValue { get { return true; } }
		public override bool CanExecute(object parameter) {
			return true;
		}
		public BatchSetObjectPropertyCommand(CommandManager commandManager)
			: base(commandManager) {
		}
		public override HistoryItem ExecuteInternal(object parameter) {
			List<SetPropertyCommandParameter> parameters = parameter as List<SetPropertyCommandParameter>;
			if (parameters == null)
				return null;
			List<object> oldValues = new List<object>();
			List<object> values = new List<object>();
			List<string> propertyNames = new List<string>();
			List<object> targets = new List<object>();
			foreach (SetPropertyCommandParameter commandParameter in parameters)
				oldValues.Add(CommandUtils.GetObjectProperty(commandParameter.TargetObject as DesignerChartElementModelBase, commandParameter.PropertyName));
			foreach (SetPropertyCommandParameter commandParameter in parameters) {
				CommandUtils.SetObjectProperty(commandParameter.TargetObject as DesignerChartElementModelBase, commandParameter.PropertyName, commandParameter.Value);
				values.Add(commandParameter.Value);
				propertyNames.Add(commandParameter.PropertyName);
				targets.Add(commandParameter.TargetObject);
			}
			HistoryItem historyItem = new HistoryItem(this, oldValues, values, propertyNames);
			historyItem.TargetObject = targets;
			return historyItem;
		}
		public override void UndoInternal(HistoryItem historyItem) {
			List<object> oldValues = historyItem.OldValue as List<object>;
			List<string> propertyNames = historyItem.Parameter as List<string>;
			List<object> targets = historyItem.TargetObject as List<object>;
			for (int i = 0; i < oldValues.Count; i++) {
				CommandUtils.SetObjectProperty(targets[i] as DesignerChartElementModelBase, propertyNames[i], oldValues[i]);
			}
		}
		public override void RedoInternal(HistoryItem historyItem) {
			List<object> values = historyItem.NewValue as List<object>;
			List<string> propertyNames = historyItem.Parameter as List<string>;
			List<object> targets = historyItem.TargetObject as List<object>;
			for (int i = 0; i < values.Count; i++) {
				CommandUtils.SetObjectProperty(targets[i] as DesignerChartElementModelBase, propertyNames[i], values[i]);
			}
		}
	}
}
