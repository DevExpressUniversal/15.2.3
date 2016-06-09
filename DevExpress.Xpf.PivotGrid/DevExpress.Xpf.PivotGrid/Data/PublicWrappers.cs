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
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.PivotGrid.OLAP;
using CoreXtraPivotGrid = DevExpress.XtraPivotGrid;
namespace DevExpress.Xpf.PivotGrid {
	public class OlapPropertyDescriptor {
		readonly string name;
		readonly string xmlName;
		readonly string uniqueName;
		readonly Type type;
		bool isUserDefined;
		public string Name { get { return name; } }
		public Type Type { get { return type; } }
		public bool IsUserDefined { get { return isUserDefined; } }
		internal string XmlName { get { return xmlName; } }
		public OlapPropertyDescriptor(string name, string uniqueName, Type type) {
			this.name = name;
			this.uniqueName = uniqueName;
			this.type = type;
			isUserDefined = !OLAPMetadataHelper.IsIntrinsicMemberProperty(name);
			this.xmlName = isUserDefined ? System.Xml.XmlConvert.EncodeName(new StringBuilder().Append(uniqueName).Append(".[").Append(name).Append("]").ToString()) : name;
		}
	}
	public class PivotOlapKpiValue : DependencyObject {
		CoreXtraPivotGrid.PivotOLAPKPIValue value;
		internal PivotOlapKpiValue(CoreXtraPivotGrid.PivotOLAPKPIValue value) {
			this.value = value;
		}
		public object Goal { get { return value.Goal; } }
		public int Status { get { return value.Status; } }
		public int Trend { get { return value.Trend; } }
		public object Value { get { return value.Value; } }
		public double Weight { get { return value.Weight; } }
		internal static PivotOlapKpiValue Create(CoreXtraPivotGrid.PivotOLAPKPIValue value) {
			if(value == null)
				return null;
			return new PivotOlapKpiValue(value);
		}
	}
	public class PivotOlapKpiMeasures : DependencyObject {
		CoreXtraPivotGrid.PivotOLAPKPIMeasures value;
		internal PivotOlapKpiMeasures(CoreXtraPivotGrid.PivotOLAPKPIMeasures value) {
			this.value = value;
		}
		public string GoalMeasure { get { return value.GoalMeasure; } }
		public string KpiName { get { return value.KPIName; } }
		public string StatusMeasure { get { return value.StatusMeasure; } }
		public string TrendMeasure { get { return value.TrendMeasure; } }
		public string ValueMeasure { get { return value.ValueMeasure; } }
		public string WeightMeasure { get { return value.WeightMeasure; } }
		internal static PivotOlapKpiMeasures Create(CoreXtraPivotGrid.PivotOLAPKPIMeasures value) {
			if(value == null)
				return null;
			return new PivotOlapKpiMeasures(value);
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class FieldTreeViewContent : FrameworkElement {
		public Brush Foreground {
			get { return (Brush)GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}
		public static readonly DependencyProperty ForegroundProperty =
			DependencyProperty.Register("Foreground", typeof(Brush), typeof(FieldTreeViewContent), new PropertyMetadata(null));
		public ImageSource ImageSource {
			get { return (ImageSource)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}
		public static readonly DependencyProperty ImageSourceProperty =
			DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(FieldTreeViewContent), new PropertyMetadata(null));
		public string DisplayText {
			get { return (string)GetValue(DisplayTextProperty); }
			set { SetValue(DisplayTextProperty, value); }
		}
		public static readonly DependencyProperty DisplayTextProperty =
			DependencyProperty.Register("DisplayText", typeof(string), typeof(FieldTreeViewContent), new PropertyMetadata(string.Empty));
		public PivotGridField Field {
			get { return (PivotGridField)GetValue(FieldProperty); }
			set { SetValue(FieldProperty, value); }
		}
		public static readonly DependencyProperty FieldProperty =
			DependencyProperty.Register("Field", typeof(PivotGridField), typeof(FieldTreeViewContent), new PropertyMetadata(null));
		public bool IsChecked {
			get { return (bool)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}
		public static readonly DependencyProperty IsCheckedProperty =
			DependencyProperty.Register("IsChecked", typeof(bool), typeof(FieldTreeViewContent), new PropertyMetadata(false));
		public Visibility CheckBoxVisibility {
			get { return (Visibility)GetValue(CheckBoxVisibilityProperty); }
			set { SetValue(CheckBoxVisibilityProperty, value); }
		}
		public static readonly DependencyProperty CheckBoxVisibilityProperty =
			DependencyProperty.Register("CheckBoxVisibility", typeof(Visibility), typeof(FieldTreeViewContent), new PropertyMetadata(Visibility.Collapsed));
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty ContentTemplateProperty =
			DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(FieldTreeViewContent), new PropertyMetadata(null));
		public DataTemplateSelector ContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
			set { SetValue(ContentTemplateSelectorProperty, value); }
		}
		public static readonly DependencyProperty ContentTemplateSelectorProperty =
			DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(FieldTreeViewContent), new PropertyMetadata(null));
		public Style ActualHeaderContentStyle {
			get { return (Style)GetValue(ActualHeaderContentStyleProperty); }
			set { SetValue(ActualHeaderContentStyleProperty, value); }
		}
		public static readonly DependencyProperty ActualHeaderContentStyleProperty =
			DependencyProperty.Register("ActualHeaderContentStyle", typeof(Style), typeof(FieldTreeViewContent), new PropertyMetadata(null));
	}
}
