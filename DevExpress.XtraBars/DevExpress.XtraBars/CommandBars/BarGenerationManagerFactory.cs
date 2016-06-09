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
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraBars.Commands.Design {
	#region BarGenerationManagerFactory<TControl, TCommandId> (abstract class)
	public abstract class BarGenerationManagerFactory<TControl, TCommandId>
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		public BarGenerationManagerBase<TControl, TCommandId> CreateBarGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<TControl, TCommandId> barController) {
			if (container is BarManager)
				return CreateBarGenerationManagerInstance(creator, container, barController);
			else if (container is RibbonControl)
				return CreateRibbonGenerationManagerInstance(creator, container, barController);
			return null;
		}
		protected abstract RibbonGenerationManager<TControl, TCommandId> CreateRibbonGenerationManagerInstance(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<TControl, TCommandId> barController);
		protected abstract BarGenerationManager<TControl, TCommandId> CreateBarGenerationManagerInstance(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<TControl, TCommandId> barController);
	}
	#endregion
}
