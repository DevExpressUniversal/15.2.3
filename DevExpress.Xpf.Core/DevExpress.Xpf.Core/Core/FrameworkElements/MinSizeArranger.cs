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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core {
	public class MinSizeArranger : Decorator {
		public static readonly DependencyProperty MinChildWidthProperty;
		public static readonly DependencyProperty MinChildHeightProperty;
		static MinSizeArranger() {
			MinChildWidthProperty = DependencyProperty.Register("MinChildWidth", typeof(double), typeof(MinSizeArranger), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange));
			MinChildHeightProperty = DependencyProperty.Register("MinChildHeight", typeof(double), typeof(MinSizeArranger), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange));
		}
		public double MinChildWidth {
			get { return (double)GetValue(MinChildWidthProperty); }
			set { SetValue(MinChildWidthProperty, value); }
		}
		public double MinChildHeight {
			get { return (double)GetValue(MinChildHeightProperty); }
			set { SetValue(MinChildHeightProperty, value); }
		}
		protected override Size MeasureOverride(Size constraint) {
			base.MeasureOverride(constraint);
			return new Size(0, 0);
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			return base.ArrangeOverride(arrangeSize.Width < MinChildWidth || arrangeSize.Height < MinChildHeight ? new Size(0, 0) : arrangeSize);
		}
	}
}
