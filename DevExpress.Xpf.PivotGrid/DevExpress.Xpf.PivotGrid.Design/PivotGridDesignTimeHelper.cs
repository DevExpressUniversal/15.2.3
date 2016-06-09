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

#if SILVERLIGHT
extern alias Platform;
#endif
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Linq;
using Microsoft.Windows.Design.Model;
using DevExpress.Xpf.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Windows.Design.Policies;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Collections.Specialized;
using Microsoft.Windows.Design.Services;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Windows.Interop;
using System.Reflection;
using DevExpress.Xpf.Core.Design;
using DevExpress.Design.UI;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Design.DependencyPropertyHelper;
#if SILVERLIGHT
using DependencyObject = Platform::System.Windows.DependencyObject;
using DependencyProperty = Platform::System.Windows.DependencyProperty;
using PropertyMetadata = Platform::System.Windows.PropertyMetadata;
using Point = Platform::System.Windows.Point;
using RoutedEventHandler = Platform::System.Windows.RoutedEventHandler;
using RoutedEventArgs = Platform::System.Windows.RoutedEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using FrameworkElement = Platform::System.Windows.FrameworkElement;
using ToggleStateButton = DevExpress.Xpf.Core.Design.CoreUtils.ToggleStateButton;
using UIElement = Platform::System.Windows.FrameworkElement;
using LayoutHelper = Platform::DevExpress.Xpf.Core.Native.LayoutHelper;
using HitTestResult =  Platform::DevExpress.Xpf.Core.HitTestResult;
using HitTestResultBehavior = Platform::DevExpress.Xpf.Core.HitTestResultBehavior;
using HitTestFilterCallback = Platform::DevExpress.Xpf.Core.HitTestFilterCallback;
using HitTestResultCallback = Platform::DevExpress.Xpf.Core.HitTestResultCallback;
using HitTestFilterBehavior = Platform::DevExpress.Xpf.Core.HitTestFilterBehavior;
using PointHitTestParameters = Platform::DevExpress.Xpf.Core.PointHitTestParameters;
using FieldHeader = Platform::DevExpress.Xpf.PivotGrid.Internal.FieldHeader;
using PivotGridControl = Platform::DevExpress.Xpf.PivotGrid.PivotGridControl;
using PivotGridField = Platform::DevExpress.Xpf.PivotGrid.PivotGridField;
using DevExpress.Xpf.Core.Design.CoreUtils;
using Platform::DevExpress.Xpf.Editors.Settings;
using Platform::DevExpress.Data;
using Platform::DevExpress.Xpf.Bars.Helpers;
using Platform::DevExpress.Xpf.Core;
using Platform::DevExpress.Utils;
using Platform::DevExpress.Xpf.Core.Commands;
using Platform::DevExpress.Xpf.Core.WPFCompatibility;
using System.Text.RegularExpressions;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
#else
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Data;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Commands;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.PivotGrid.Internal;
using System.Text.RegularExpressions;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
#endif
namespace DevExpress.Xpf.PivotGrid.Design {
	public static class PivotGridDesignTimeHelper {
		public static readonly string FieldNamePropertyName = DependencyPropertyHelper.GetPropertyName(() => PivotGridField.FieldNameProperty);
		public static readonly string CaptionPropertyName = DependencyPropertyHelper.GetPropertyName(() => PivotGridField.CaptionProperty);
		public static readonly string AreaPropertyName = DependencyPropertyHelper.GetPropertyName(() => PivotGridField.AreaProperty);
		internal static ModelItem CreateModelItem(EditingContext context, Type elementType, CreateOptions createOptions) {
			ModelItem modelItem = ModelFactory.CreateItem(context, elementType, createOptions, null);
			ClearProperty(modelItem, "HorizontalAlignment");
			ClearProperty(modelItem, "VerticalAlignment");
			return modelItem;
		}
		static void ClearProperty(ModelItem modelItem, string propertyName) {
			ModelProperty property = modelItem.Properties.Find(propertyName);
			if(property != null)
				property.ClearValue();
		}
		public static PivotGridControl GetPivotGrid(ModelItem item) {
			object curr = item.GetCurrentValue();
			PivotGridControl pivot = curr as PivotGridControl;
			if(pivot != null)
				return pivot;
			PivotGridField field = curr as PivotGridField;
			if(field != null)
				return field.Parent as PivotGridControl;
			FormatConditionBase format = curr as FormatConditionBase;
			if(format != null)
				return format.Owner.Owner as PivotGridControl;
			return null;
		}
		public static ModelItem GetPrimarySelection(PropertyValue propertyValue) {
			return GetPrimarySelection(propertyValue.ParentProperty.Context);
		}
		public static ModelItem GetPrimarySelection(EditingContext context) {
			return context.Items.GetValue<Microsoft.Windows.Design.Interaction.Selection>().PrimarySelection;
		}
		internal static ModelItemCollection GetPivotGridFieldsCollection(ModelItem AdornedElement) {
			return AdornedElement.Properties["Fields"].Collection;
		}
		internal static FieldHeader GetFieldHeaderElements(PivotGridControl pivotGrid, PivotGridField field) {
			return LayoutHelper.FindElement(pivotGrid, (d) => d is FieldHeader && ((FieldHeader)d).Field == field) as FieldHeader;
		}
		internal static string SplitString(object value){
			return string.Join(" ", Regex.Split(value.ToString(), @"(?<!^)(?=[A-Z])"));
		}
		public static void SetField(IModelItem modelItem, object field, string caption) {
			ModelPropertyHelper.SetPropertyValue(modelItem, FieldNamePropertyName, field);
			ModelPropertyHelper.SetPropertyValue(modelItem, CaptionPropertyName, caption);
		}
		static ResourceDictionary editorsTemplates;
		public static ResourceDictionary EditorsTemplates { 
			get {
				if(editorsTemplates == null)
					editorsTemplates = new ResourceDictionary() { Source = new Uri("/DevExpress.Xpf.PivotGrid" + AssemblyInfo.VSuffix + ".Design;component/PivotGridEditorsTemplates.xaml", UriKind.Relative) };
				return editorsTemplates;
			}
		}
	}
}
