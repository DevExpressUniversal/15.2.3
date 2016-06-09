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
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Charts.Native {
	public static class TemplateHelper {
		static DependencyProperty[] GetLabelProperies() {
			List<DependencyProperty> properties = new List<DependencyProperty>();
			properties.AddRange(new DependencyProperty[] { Control.PaddingProperty, Control.MarginProperty });
			properties.AddRange(new DependencyProperty[] { Control.ForegroundProperty, Control.FontSizeProperty, Control.FontFamilyProperty, 
				Control.FontStretchProperty, Control.FontStyleProperty, Control.FontWeightProperty });
			return properties.ToArray();
		}
		static string BuildString(Control control, DependencyProperty[] properties) {
			string s = string.Empty;
			for (int i = 0; i < properties.Length; i++) {
				object propertyValue = control.GetValue(properties[i]);
				if (propertyValue != null)
					s += String.Format(" {0}=\"{1}\" ", properties[i].ToString(), propertyValue.ToString());
			}
			return s;
		}
		public static DataTemplate GetAxisTitleTemplate(AxisTitle title) {
			string dataTemplate = String.Format("<Border><Label Content=\"{{Binding Path=Content}}\" {0}/></Border>", BuildString(title, GetLabelProperies()));
			return XamlHelper.GetTemplate(dataTemplate) as DataTemplate;
		}
		public static DataTemplate GetAxisLabelItemTemplate(AxisLabel label) {
			string dataTemplate = String.Format("<Border><Label Content=\"{{Binding Path=Content}}\" {0}/></Border>", BuildString(label, GetLabelProperies()));
			return XamlHelper.GetTemplate(dataTemplate) as DataTemplate;
		}
		public static DataTemplate GetSeriesLabelTemplate() {
			string template = @"<Border Padding=""{Binding Path=Label.Padding}""
                                        Background=""{Binding Path=Label.Background}"" 
                                        BorderBrush=""{Binding Path=Label.BorderBrush}"" 
                                        BorderThickness=""{Binding Path=Label.BorderThickness}"">
                                    <ContentPresenter Content=""{Binding }"" ContentTemplate=""{Binding Path=Label.ElementTemplate}""
                                                    FlowDirection=""{Binding Path=Label.FlowDirection}""
                                                    TextElement.Foreground=""{Binding Path=Label.Foreground}""
                                                    TextElement.FontFamily=""{Binding Path=Label.FontFamily}""
                                                    TextElement.FontSize=""{Binding Path=Label.FontSize}""
                                                    TextElement.FontStretch=""{Binding Path=Label.FontStretch}""
                                                    TextElement.FontStyle=""{Binding Path=Label.FontStyle}""
                                                    TextElement.FontWeight=""{Binding Path=Label.FontWeight}""/>
                                </Border>";
			return XamlHelper.GetTemplate(template) as DataTemplate;
		}
	}
}
