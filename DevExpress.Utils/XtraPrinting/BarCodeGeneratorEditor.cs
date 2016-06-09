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
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting.BarCode.Native;
namespace DevExpress.XtraPrinting.BarCode.Design {
	class BarCodeGeneratorEditor : ObjectPickerEditor {
		#region static
		static void SetBarCodeSymbology(object control, BarCodeSymbology symbology) {
			PropertyDescriptor descriptor = GetSymbologyProperty(control);
			descriptor.SetValue(control, BarCodeGeneratorFactory.Create(symbology));
		}
		static BarCodeGeneratorBase GetBarCodeSymbology(object control) {
			PropertyDescriptor descriptor = GetSymbologyProperty(control);
			return descriptor != null ? (BarCodeGeneratorBase)descriptor.GetValue(control) : null;
		}
		static PropertyDescriptor GetSymbologyProperty(object control) {
			PropertyDescriptor result = TypeDescriptor.GetProperties(control)["Symbology"];
			return result != null && typeof(BarCodeGeneratorBase).IsAssignableFrom(result.PropertyType) ? result : null;
		}
		static bool ContainsSymbologyProperty(object control) {
			return GetSymbologyProperty(control) != null;
		}
		#endregion
		protected override ObjectPickerControl CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			return new PickerFromValuesControl(this, value, Enum.GetValues(typeof(BarCodeSymbology)), true);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(IsContextInvalid(context))
				return value;
			object[] controls = GetControls(context);
			if(controls == null || controls.GetLength(0) <= 0)
				return value;
			object oldValue = GetBarCodeSymbology(controls[0]).SymbologyCode;
			object newValue = base.EditValue(context, provider, oldValue);
			if(object.Equals(newValue, oldValue))
				return value;
			BarCodeSymbology newBarCodeSymbology = (BarCodeSymbology)newValue;
			IDesignerHost designerHost = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
			DesignerTransaction transaction = designerHost.CreateTransaction("Symbology changed"); 
			try {
				context.OnComponentChanging();
				foreach(object control in controls)
					SetBarCodeSymbology(control, newBarCodeSymbology);
				context.OnComponentChanged();
			} catch {
				transaction.Cancel();
			} finally {
				transaction.Commit();
			}
			return BarCodeGeneratorFactory.Create(newBarCodeSymbology);
		}
		object[] GetControls(ITypeDescriptorContext context) {
			if(context.Instance is Array)
				return GetComponentsFromArray((Array)context.Instance);
			if(ContainsSymbologyProperty(context.Instance))
				return new object[] { context.Instance };
			return null;
		}
		object[] GetComponentsFromArray(Array componentsArray) {
			ArrayList result = new ArrayList(componentsArray.Length);
			foreach(object obj in componentsArray) {
				if(ContainsSymbologyProperty(obj))
					result.Add(obj);
			}
			return result.ToArray();
		}
	}
}
