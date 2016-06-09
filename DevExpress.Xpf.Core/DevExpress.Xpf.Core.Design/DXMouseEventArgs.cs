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

extern alias Platform;
using System.Windows;
using Microsoft.Windows.Design.Interaction;
using System.Windows.Input;
#if SL
using Platform::DevExpress.Xpf.Core;
#else
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.Xpf.Core.Design {
	public class DXMouseEventArgsFromWPF : DXMouseEventArgs {
		internal static void TranslatePointToPlatform(FrameworkElement adorner, ref Point p) {
			double zoomLevel = DesignerView.GetDesignerView(adorner).ZoomLevel;
			p.X /= zoomLevel;
			p.Y /= zoomLevel;
		}
		public DXMouseEventArgsFromWPF(MouseEventArgs args, FrameworkElement adorner, Platform::System.Windows.FrameworkElement platformControl) {
			OriginalSource = args.OriginalSource;
			RelativePositionElement = platformControl;
			Point p = args.GetPosition(adorner);
			TranslatePointToPlatform(adorner, ref p);
			RelativePosition = new Platform::System.Windows.Point(p.X, p.Y);
		}
	}
	public class DXMouseButtonEventArgsFromWPF : DXMouseButtonEventArgs {
		public DXMouseButtonEventArgsFromWPF(MouseButtonEventArgs args, FrameworkElement adorner, Platform::System.Windows.FrameworkElement platformControl) {
			OriginalSource = args.OriginalSource;
			RelativePositionElement = platformControl;
			Point p = args.GetPosition(adorner);
			DXMouseEventArgsFromWPF.TranslatePointToPlatform(adorner, ref p);
			RelativePosition = new Platform::System.Windows.Point(p.X, p.Y);
			Handled = args.Handled;
			HandledChanged = e => args.Handled = e.Handled;
		}
	}
}
