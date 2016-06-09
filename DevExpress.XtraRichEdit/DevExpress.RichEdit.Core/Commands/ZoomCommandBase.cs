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
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
namespace DevExpress.XtraRichEdit.Commands {
	#region ZoomCommandBase
	public abstract class ZoomCommandBase : RichEditMenuItemSimpleCommand {
		internal const float DefaultZoomFactorDelta = 0.1f;
		float? delta;
		protected ZoomCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal float ZoomFactor { get { return ActiveView.ZoomFactor; } set { ActiveView.ZoomFactor = value; } }
		protected internal abstract float CalculateNewZoomFactor(float oldZoomFactor);
		internal float? Delta { get { return delta; } set { delta = value; } }
		internal float ActualDelta { get { return Delta.HasValue ? Delta.Value : DefaultZoomFactorDelta; } }
		protected internal override void ExecuteCore() {
			CheckExecutedAtUIThread();
			ZoomFactor = CalculateNewZoomFactor(ZoomFactor);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			ApplyCommandsRestriction(state, Options.Behavior.Zooming);
		}
	}
	#endregion
}
