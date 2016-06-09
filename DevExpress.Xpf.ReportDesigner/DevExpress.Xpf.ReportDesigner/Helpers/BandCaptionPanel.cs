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

using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public class BandCaptionPanel : Panel {
		public static readonly DependencyProperty MinTextLengthProperty;
		static BandCaptionPanel() {
			DependencyPropertyRegistrator<BandCaptionPanel>.New()
				.Register(d => d.MinTextLength, out MinTextLengthProperty, 36.0);
		}
		public double MinTextLength {
			get { return (double)GetValue(MinTextLengthProperty); }
			set { SetValue(MinTextLengthProperty, value); }
		}
		protected override Size MeasureOverride(Size constraint) {
			var desiredSize = new Size();
			if(constraint.Height < MinTextLength + 2)
				return desiredSize;
			foreach(UIElement child in Children) {
				child.Measure(constraint);
				desiredSize = child.DesiredSize;
			}
			return desiredSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(finalSize.Height < MinTextLength + 2) {
				foreach(UIElement child in Children)
					child.Arrange(new Rect(new Point(0.0, 0.0), new Size(0, 0)));
				return new Size(0, 0);
			}
			foreach(UIElement child in Children) {
				var paddingTop = finalSize.Height < MinTextLength ? ((finalSize.Height - MinTextLength) / 2) + 1 : 0;
				child.Arrange(new Rect(new Point(0, paddingTop), new Size(finalSize.Width, finalSize.Height)));
			}
			return finalSize;
		}
	}
}
