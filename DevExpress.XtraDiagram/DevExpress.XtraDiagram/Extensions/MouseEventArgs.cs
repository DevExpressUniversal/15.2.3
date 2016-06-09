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
using System.Windows.Forms;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Handlers;
using DevExpress.XtraDiagram.Platform;
namespace DevExpress.XtraDiagram.Extensions {
	public static class MouseEventArgsExtensions {
		public static IMouseButtonArgs GetIMouseButtonArgs(this MouseEventArgs args, DiagramControlHandler diagramHandler) {
			return new PlatformMouseButtonArgs(args, diagramHandler);
		}
		public static System.Windows.Input.MouseButton GetChangeButton(this MouseEventArgs args) {
			if((args.Button & MouseButtons.Left) != 0) {
				return System.Windows.Input.MouseButton.Left;
			}
			if((args.Button & MouseButtons.Middle) != 0) {
				return System.Windows.Input.MouseButton.Middle;
			}
			if((args.Button & MouseButtons.Right) != 0) {
				return System.Windows.Input.MouseButton.Right;
			}
			if((args.Button & MouseButtons.XButton1) != 0) {
				return System.Windows.Input.MouseButton.XButton1;
			}
			if((args.Button & MouseButtons.XButton2) != 0) {
				return System.Windows.Input.MouseButton.XButton2;
			}
			return System.Windows.Input.MouseButton.Left;
		}
	}
}
