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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.DemoBase.Helpers {
	class MouseTouchAdapterArguments {
		public bool Handled { get; set; }
		public Point Position { get; set; }
	}
	class MouseTouchAdapter {
		public static void SubscribeToPointerUp(FrameworkElement elem, FrameworkElement relativeTo, Action<MouseTouchAdapterArguments> handler) {
			elem.IsManipulationEnabled = true;
			elem.PreviewMouseUp += (sender, args) => {
				if (args.ChangedButton != MouseButton.Left)
					return;
				var adapterArgs = new MouseTouchAdapterArguments { Position = args.GetPosition(relativeTo) };
				handler(adapterArgs);
				args.Handled = adapterArgs.Handled;
			};
			elem.TouchUp += (sender, args) => {
				var adapterArgs = new MouseTouchAdapterArguments { Position = args.GetTouchPoint(relativeTo).Position };
				handler(adapterArgs);
				args.Handled = adapterArgs.Handled;
			};
		}
		public static void SubscribeToSwipe(FrameworkElement elem, FrameworkElement relative, Action<Point> begin, Action<Point> update, Action<Point> end) {
			var cumulativeDelta = new Point();
			elem.IsManipulationEnabled = true;
			elem.ManipulationStarted += (sender, args) => {
				args.Handled = true;
				begin(args.ManipulationOrigin);
			};
			elem.ManipulationDelta += (sender, args) => {
				var translation = args.DeltaManipulation.Translation;
				cumulativeDelta.X += translation.X;
				cumulativeDelta.Y += translation.Y;
				Debug.WriteLine(cumulativeDelta);
				update(cumulativeDelta);
			};
			elem.ManipulationCompleted += (sender, args) => {
				var translation = args.TotalManipulation.Translation;
				if (translation.X < 10d)
					args.Cancel();
				end(new Point(translation.X, translation.Y));
				cumulativeDelta = new Point();
			};
		}
	}
}
