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
using System.Windows;
using DevExpress.Xpf.Bars;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Printing {
	public static class BarManagerCustomization {
		public static readonly DependencyProperty TemplateProperty =
			DependencyProperty.RegisterAttached("Template", typeof(DataTemplate), typeof(BarManagerCustomization), new PropertyMetadata(null, TemplateChangedCallback));
		public static DataTemplate GetTemplate(DependencyObject obj) {
			return (DataTemplate)obj.GetValue(TemplateProperty);
		}
		public static void SetTemplate(DependencyObject obj, DataTemplate value) {
			obj.SetValue(TemplateProperty, value);
		}
		static List<DataTemplate> GetTemplatesCollector(DependencyObject obj) {
			return (List<DataTemplate>)obj.GetValue(TemplatesCollectorProperty);
		}
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty TemplatesCollectorProperty =
			DependencyProperty.RegisterAttached("TemplatesCollector", typeof(List<DataTemplate>), typeof(BarManagerCustomization), new PropertyMetadata(new List<DataTemplate>()));
		static void TemplateChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d == null || e.NewValue == null)
				return;
			var preview = d as DocumentPreview;
			if(preview == null)
				throw new NotSupportedException("Dependency Property can by applied only to the DevExpress.Xpf.Printing.DocumentPreview");
			if(preview.BarManager == null) {
				var collector = GetTemplatesCollector(d);
				collector.Add((DataTemplate)e.NewValue);
			}
			ApplyTemplate(preview);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal static void ApplyTemplate(DocumentPreview preview) {
			if(preview == null || preview.BarManager == null)
				return;
			var collector = GetTemplatesCollector(preview);
			foreach(var template in collector) {
				AddAndExecute(preview.BarManager, template);
			}
			var actualTemplate = GetTemplate(preview);
			if(!collector.Contains(actualTemplate)) {
				AddAndExecute(preview.BarManager, actualTemplate);
			}
			collector.Clear();
		}
		static void AddAndExecute(BarManager manager, DataTemplate template) {
			if(manager.Controllers.OfType<TemplatedBarManagerController>()
			   .Any(x => x.Template == template))
				return;
			var controller = new TemplatedBarManagerController() { Template = template };
			manager.Controllers.Add(controller);
			controller.Execute();
		}
	}
}
