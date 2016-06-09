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

namespace DevExpress.Design.Filtering.UI {
	using System;
	using System.Collections.Generic;
	using System.Windows;
	using DevExpress.Design.UI;
	public sealed class MetricPropertyContainer : System.Windows.Controls.ContentControl {
		public static readonly DependencyProperty SourceProperty;
		static MetricPropertyContainer() {
			var dProp = new DependencyPropertyRegistrator<MetricPropertyContainer>();
			dProp.Register("Source", ref SourceProperty, (IFilteringModelMetricProperty)null,
				(dObj, e) => ((MetricPropertyContainer)dObj).OnSourceChanged());
		}
		public MetricPropertyContainer() {
			Focusable = false;
		}
		public IFilteringModelMetricProperty Source {
			get { return (IFilteringModelMetricProperty)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		static string editableTemplatePrefix = "Editable";
		void OnSourceChanged() {
			ContentTemplate = MetricPropertyTemplateLoader.LoadTemplate(Source.GetCodeName(), Source.IsEditable ? editableTemplatePrefix : "");
		}
	}
	static class MetricPropertyTemplateLoader {
		static ResourceDictionary resources;
		static MetricPropertyTemplateLoader() {
			resources = new ResourceDictionary();
			string resourceUri = string.Format(
				"pack://application:,,,/DevExpress.Design.{0};component/UI/EndUserFiltering/MetricPropertyTemplates.xaml",
				AssemblyInfo.VSuffixWithoutSeparator);
			resources.Source = new Uri(resourceUri, UriKind.Absolute);
		}
		public static DataTemplate LoadTemplate(MetricPropertyCodeName codeName, string templatePrefix) {
			return (DataTemplate)resources[GetTemplateName(codeName, templatePrefix)];
		}
		static string GetTemplateName(MetricPropertyCodeName codeName, string templatePrefix) {
			return templatePrefix + "MetricProperty" + codeName.ToString() + "Template";
		}
	}
	public sealed class MetricPropertyContainerTest : System.Windows.Controls.ContentControl {
		public static readonly DependencyProperty SourceProperty;
		static MetricPropertyContainerTest() {
			var dProp = new DependencyPropertyRegistrator<MetricPropertyContainerTest>();
			dProp.Register("Source", ref SourceProperty, (ModelTypeSettingsPageViewModel)null,
				(dObj, e) => ((MetricPropertyContainerTest)dObj).OnSourceChanged((MetricPropertyContainerTest)dObj, e));
		}
		public MetricPropertyContainerTest() {
			Focusable = false;
		}
		public ModelTypeSettingsPageViewModel Source {
			get { return (ModelTypeSettingsPageViewModel)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		void OnSourceChanged(MetricPropertyContainerTest dobj, DependencyPropertyChangedEventArgs e) {
			ContentTemplate = MetricPropertyTemplateLoaderTest.LoadTemplate("Types");
		}
	}
	static class MetricPropertyTemplateLoaderTest {
		static ResourceDictionary resources;
		static MetricPropertyTemplateLoaderTest() {
			resources = new ResourceDictionary();
			string resourceUri = string.Format(
				"pack://application:,,,/DevExpress.Design.{0};component/UI/EndUserFiltering/MetricPropertyTemplates.xaml",
				AssemblyInfo.VSuffixWithoutSeparator);
			resources.Source = new Uri(resourceUri, UriKind.Absolute);
		}
		public static DataTemplate LoadTemplate(string codeName) {
			string templateName = GetTemplateName(codeName);
			return (DataTemplate)resources[templateName];
		}
		static string GetTemplateName(string codeName) {
			return "MetricProperty" + codeName.ToString() + "Template";
		}
	}
}
