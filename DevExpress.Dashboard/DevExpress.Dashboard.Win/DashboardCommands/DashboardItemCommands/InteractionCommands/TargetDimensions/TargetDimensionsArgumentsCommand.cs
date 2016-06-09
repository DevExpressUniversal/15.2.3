﻿#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
namespace DevExpress.DashboardWin.Commands {
	public abstract class TargetDimensionsArgumentsCommand : TargetDimensionsCommand {
		protected override TargetDimensions TargetDimensions { get { return DashboardCommon.TargetDimensions.Arguments; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandTargetDimensionsArgumentsCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandTargetDimensionsArgumentsDescription; } }
		protected TargetDimensionsArgumentsCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class ChartTargetDimensionsArgumentsCommand : TargetDimensionsArgumentsCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.ChartTargetDimensionsArguments; } }
		public override string ImageName { get { return "ChartTargetDimensionsArguments"; } }
		public ChartTargetDimensionsArgumentsCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class PieTargetDimensionsArgumentsCommand : TargetDimensionsArgumentsCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.PieTargetDimensionsArguments; } }
		public override string ImageName { get { return "PieTargetDimensionsArguments"; } }
		public PieTargetDimensionsArgumentsCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
}
