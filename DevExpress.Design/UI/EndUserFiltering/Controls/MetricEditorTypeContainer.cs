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
	using System.Windows;
	using DevExpress.Design.UI;
	public sealed class MetricEditorTypeContainer : System.Windows.Controls.ContentControl {
		public static readonly DependencyProperty SourceProperty;
		static MetricEditorTypeContainer() {
			var dProp = new DependencyPropertyRegistrator<MetricEditorTypeContainer>();
			dProp.Register("Source", ref SourceProperty, (IFilteringModelMetricEditorType)null,
				(dObj, e) => ((MetricEditorTypeContainer)dObj).OnSourceChanged());
		}
		public MetricEditorTypeContainer() {
			Focusable = false;
		}
		public IFilteringModelMetricEditorType Source {
			get { return (IFilteringModelMetricEditorType)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		void OnSourceChanged() {
			ContentTemplate = MetricEditorTypeTemplateLoader.LoadTemplate(Source.GetCodeName());
		}
	}
	static class MetricEditorTypeTemplateLoader {
		static ResourceDictionary resources;
		static MetricEditorTypeTemplateLoader() {
			resources = new ResourceDictionary();
			string resourceUri = string.Format(
				"pack://application:,,,/DevExpress.Design.{0};component/UI/EndUserFiltering/MetricEditorTypeTemplates.xaml",
				AssemblyInfo.VSuffixWithoutSeparator);
			resources.Source = new Uri(resourceUri, UriKind.Absolute);
		}
		public static DataTemplate LoadTemplate(MetricEditorTypeCodeName codeName) {
			return (DataTemplate)resources[GetTemplateName(codeName)];
		}
		static string GetTemplateName(MetricEditorTypeCodeName codeName) {
			return "MetricEditorType" + codeName.ToString() + "Template";
		}
	}
}
