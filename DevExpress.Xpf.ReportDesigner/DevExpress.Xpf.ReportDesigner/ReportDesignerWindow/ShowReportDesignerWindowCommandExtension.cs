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
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native;
namespace DevExpress.Xpf.Reports.UserDesigner {
	[ContentProperty("WindowSettings")]
	public class ShowReportDesignerWindowCommandExtension : MarkupExtension {
		DataTemplate reportDesignerTemplate;
		public DataTemplate ReportDesignerTemplate {
			get { return reportDesignerTemplate; }
			set { reportDesignerTemplate = value; }
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			var target = GetTargetObject(serviceProvider);
			var associatedObject = target as DependencyObject;
			return new DelegateCommand(() => Show(associatedObject, ReportDesignerTemplate), false);
		}
		object GetTargetObject(IServiceProvider serviceProvider) {
			var provideValueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
			return provideValueTarget.With(x => x.TargetObject);
		}
		static void Show(DependencyObject associatedObject, DataTemplate reportDesignerTemplate) {
			var reportDesigner = reportDesignerTemplate == null ? new ReportDesigner() : XamlTemplateHelper.CreateObjectFromTemplate<ReportDesigner>(reportDesignerTemplate);
			reportDesigner.DataContext = new AssociatedObjectContainer(associatedObject);
			reportDesigner.ShowWindow(associatedObject as FrameworkElement ?? Window.GetWindow(associatedObject));
		}
	}
}
