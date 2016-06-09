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
using System.Linq;
using DevExpress.Design.SmartTags;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Design;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Core.Design.SmartTags;
namespace DevExpress.Xpf.Core.Design.Services.PropertyLinesProviders {
	public sealed class DXSplashScreenServicePropertyLinesProvider : ViewServicePropertyLinesProviderBase {
		static bool IsWindowSplashScreen(Type type) {
			return typeof(Window).IsAssignableFrom(type) && typeof(ISplashScreen).IsAssignableFrom(type);
		}
		public DXSplashScreenServicePropertyLinesProvider() :
			base(typeof(DXSplashScreenService)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DXSplashScreenService.ShowSplashScreenOnLoadingProperty.Name));
			lines.Add(() => new TypeSelectorPropertyLineViewModel(viewModel, DXSplashScreenService.SplashScreenTypeProperty.Name, ((IPropertyLineContext)viewModel).PlatformInfoFactory.ForStandardProperty(DXSplashScreenService.SplashScreenTypeProperty.Name), TypePredicate));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DXSplashScreenService.SplashScreenStartupLocationProperty.Name, typeof(System.Windows.WindowStartupLocation)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DXSplashScreenService.MaxProgressProperty.Name));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DXSplashScreenService.ProgressProperty.Name));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DXSplashScreenService.StateProperty.Name));
			return lines;
		}
		bool TypePredicate(IDXTypeMetadata typeMetadata) {
			Type type = typeMetadata.GetRuntimeType();
			return type != null && (typeof(UserControl).IsAssignableFrom(type) || IsWindowSplashScreen(type));
		}
	}
}
