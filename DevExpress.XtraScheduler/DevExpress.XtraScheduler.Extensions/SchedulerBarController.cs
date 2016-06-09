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
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraScheduler.Commands;
namespace DevExpress.XtraScheduler.UI {
	#region SchedulerBarController
	[Designer("DevExpress.XtraScheduler.Design.SchedulerBarControllerDesigner," + AssemblyInfo.SRAssemblySchedulerDesign), DXToolboxItem(false), ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "SchedulerBarController.bmp")]
	public class SchedulerBarController : ControlCommandBarControllerBase<SchedulerControl, SchedulerCommandId> {
		protected override void UpdateController() {
			if (Control == null)
				return;
			base.UpdateController();
		}
	}
	#endregion
	#region SchedulerControlRibbonPageGroup (abstract class)
	public abstract class SchedulerControlRibbonPageGroup : ControlCommandBasedRibbonPageGroup<SchedulerControl, SchedulerCommandId> {
		protected SchedulerControlRibbonPageGroup()
			: this(String.Empty) {
		}
		protected SchedulerControlRibbonPageGroup(string text)
			: base(text) {
		}
		protected override SchedulerCommandId EmptyCommandId { get { return SchedulerCommandId.None; } }
	}
	#endregion
}
