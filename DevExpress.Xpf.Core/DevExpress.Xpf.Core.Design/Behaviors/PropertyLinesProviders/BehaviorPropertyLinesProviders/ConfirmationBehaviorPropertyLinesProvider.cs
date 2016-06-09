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
using System.Text;
using System.Windows;
using DevExpress.Design.SmartTags;
using DevExpress.Mvvm.UI;
namespace DevExpress.Xpf.Core.Design.Services.PropertyLinesProviders {
	public sealed class ConfirmationBehaviorPropertyLinesProvider : PropertyLinesProviderBase {
		public ConfirmationBehaviorPropertyLinesProvider() : base(typeof(ConfirmationBehavior)) { }
		 protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, ConfirmationBehavior.CommandProperty.Name));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, ConfirmationBehavior.CommandParameterProperty.Name));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, ConfirmationBehavior.CommandPropertyNameProperty.Name));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, ConfirmationBehavior.MessageButtonProperty.Name, typeof(MessageBoxButton)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, ConfirmationBehavior.MessageDefaultResultProperty.Name, typeof(MessageBoxResult)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, ConfirmationBehavior.MessageIconProperty.Name, typeof(MessageBoxImage)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, ConfirmationBehavior.MessageTextProperty.Name));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, ConfirmationBehavior.MessageTitleProperty.Name));
			return lines;
		}
	}
}
