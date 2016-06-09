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

using DevExpress.Mvvm;
using DevExpress.Xpf.DemoBase.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.DemoBase {
	public interface IProductsPageViewModel {
		IEnumerable<IDemoCarouselItem> FeaturedDemos { get; }
		IEnumerable<IGroupedLinks> Groups { get; }
		IEnumerable<IGroupedLinks> EmptyGroups { get; }
		ICommand LinkSelectedCommand { get; }
		int ProductsViewMaxGroupsPerColumn { get; }
		string PlatformName { get; }
		ImageSource PlatformIcon { get; }
	}
	public class DataContextTypeAssert : DependencyObject {
		public static Type GetExpectedType(DependencyObject obj) {
			return (Type)obj.GetValue(ExpectedTypeProperty);
		}
		public static void SetExpectedType(DependencyObject obj, Type value) {
			obj.SetValue(ExpectedTypeProperty, value);
		}
		public static readonly DependencyProperty ExpectedTypeProperty =
			DependencyProperty.RegisterAttached("ExpectedType", typeof(Type), typeof(DataContextTypeAssert), new PropertyMetadata(typeof(string), 
				(d, e) => OnExpectedTypeChanged((FrameworkElement)d, (Type)e.NewValue)));
		static void OnExpectedTypeChanged(FrameworkElement obj, Type expectedType) {
#if DEBUG
			VerifyType(obj, expectedType);
			obj.DataContextChanged += (s, e) => {
				VerifyType((FrameworkElement)s, expectedType);
			};
#endif
		}
		static void VerifyType(FrameworkElement obj, Type expectedType) {
			if(obj.DataContext != null) {
				var type = obj.DataContext.GetType();
				obj.Dispatcher.BeginInvoke((Action)(() => {
					Debug.Assert(type.IsInstanceOfType(expectedType) || type.GetInterface(expectedType.Name) != null,
						string.Format("DataContext of object {0} should be of type {1} but was {2}", obj.Name, expectedType.Name, type.Name));
				}));
			}
		}
	}
}
