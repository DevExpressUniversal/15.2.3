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
using System.ComponentModel;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars.Commands.Internal;
namespace DevExpress.XtraBars.Commands.Design {
	#region StatusBarGenerationManager<TControl, TCommandId> (abstract class)
	public abstract class StatusBarGenerationManager<TControl, TCommandId> : BarGenerationManager<TControl, TCommandId>
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		protected StatusBarGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<TControl, TCommandId> barController)
			: base(creator, container, barController) {
		}
		protected override Bar FindCommandBar() {
			return BarManager.StatusBar;
		}
		protected internal override Bar CreateBar(ControlCommandBarCreator creator) {
			Bar bar = new Bar(BarManager); 
			bar.DockStyle = BarDockStyle.Bottom;
			bar.CanDockStyle = BarCanDockStyle.Bottom;
			bar.OptionsBar.AllowQuickCustomization = false;
			bar.OptionsBar.DrawDragBorder = false;
			bar.OptionsBar.UseWholeRow = true;
			BarManager.StatusBar = bar;
			return bar;
		}
		protected internal override void SetupItemLink(ICommandBarItem barItem, BarItemLink link) {
			barItem.SetupStatusBarLink(link);
		}
	}
	#endregion
}
