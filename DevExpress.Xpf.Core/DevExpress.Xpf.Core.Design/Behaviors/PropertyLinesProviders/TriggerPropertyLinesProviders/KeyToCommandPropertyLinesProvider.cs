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

using DevExpress.Design.SmartTags;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core.Design.Services.PropertyLinesProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.Core.Design.Services.PropertyLinesProviders {
	public sealed class KeyToCommandPropertyLinesProvider : PropertyLinesProviderBase {
		public KeyToCommandPropertyLinesProvider() 
			: base(typeof(KeyToCommand)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, KeyToCommand.CommandProperty.Name));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, KeyToCommand.CommandParameterProperty.Name));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, KeyToCommand.DispatcherPriorityProperty.Name, typeof(System.Windows.Threading.DispatcherPriority)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, KeyToCommand.MarkRoutedEventsAsHandledProperty.Name));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, KeyToCommand.ProcessEventsFromDisabledEventOwnerProperty.Name));
			lines.Add(() => new NullableBooleanPropertyLineViewModel(viewModel, KeyToCommand.UseDispatcherProperty.Name));
			return lines;
		}
	}
}
