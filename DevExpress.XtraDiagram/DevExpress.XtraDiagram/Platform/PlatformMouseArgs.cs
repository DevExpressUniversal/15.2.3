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
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Handlers;
using System.Windows.Input;
using System.Windows;
using WinMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using WinControl = System.Windows.Forms.Control;
namespace DevExpress.XtraDiagram.Platform {
	public abstract class PlatformMouseArgsBase : IMouseArgs {
		DiagramControlHandler diagramHandler;
		public PlatformMouseArgsBase(DiagramControlHandler diagramHandler) {
			this.diagramHandler = diagramHandler;
		}
		public ModifierKeys Modifiers {
			get { return WinControl.ModifierKeys.ToPlatformModifier(); }
		}
		public Point Position { get { return GetPosition(); } }
		protected DiagramControl Diagram { get { return DiagramHandler.Diagram; } }
		public void Capture() {
			DiagramHandler.CaptureMouse();
		}
		public void Release() {
			DiagramHandler.ReleaseMouse();
		}
		protected DiagramControlHandler DiagramHandler { get { return diagramHandler; } }
		protected abstract Point GetPosition();
	}
	public class PlatformMouseArgs : PlatformMouseArgsBase {
		public PlatformMouseArgs(DiagramControlHandler diagramHandler)
			: base(diagramHandler) {
		}
		protected override Point GetPosition() {
			return Diagram.DiagramViewInfo.ClientDisplayPointToPageLogical(Diagram.PointToClient(WinControl.MousePosition)).ToPlatformPoint();
		}
	}
	public class PlatformMouseButtonArgs : PlatformMouseArgs, IMouseButtonArgs {
		WinMouseEventArgs args;
		public PlatformMouseButtonArgs(WinMouseEventArgs args, DiagramControlHandler diagramHandler) : base(diagramHandler) {
			this.args = args;
		}
		protected override Point GetPosition() {
			return Diagram.DiagramViewInfo.ClientDisplayPointToPageLogical(args.Location).ToPlatformPoint();
		}
		public int ClickCount { get { return this.args.Clicks; } }
		public MouseButton ChangedButton { get { return this.args.GetChangeButton(); } }
	}
}
