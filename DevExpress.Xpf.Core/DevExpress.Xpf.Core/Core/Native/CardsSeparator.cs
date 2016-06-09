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
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Data;
using DevExpress.Xpf.Core.Native;
using System.Windows.Markup;
using System.Windows.Controls;
using DevExpress.Xpf.Utils.Native;
using System.Collections;
namespace DevExpress.Xpf.Core.Native {
	public class CardsSeparator : DependencyObject {
		static readonly DependencyPropertyKey MarginPropertyKey;
		public static readonly DependencyProperty MarginProperty;
		static readonly DependencyPropertyKey LengthPropertyKey;
		public static readonly DependencyProperty LengthProperty;
		static CardsSeparator() {
			MarginPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Margin", typeof(Thickness), typeof(CardsSeparator), new PropertyMetadata(default(Thickness)));
			MarginProperty = MarginPropertyKey.DependencyProperty;
			LengthPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Length", typeof(double), typeof(CardsSeparator), new PropertyMetadata(double.NaN));
			LengthProperty = LengthPropertyKey.DependencyProperty;
			CardsPanel.OrientationProperty.AddOwner(typeof(CardsSeparator), new FrameworkPropertyMetadata(CardsPanel.DefaultOrientation));
		}
		public CardsSeparator(int rowIndex) {
			RowIndex = rowIndex;
		}
		public int RowIndex { get; private set; }
		public Thickness Margin {
			get { return (Thickness)GetValue(MarginProperty); }
			internal set { SetValue(MarginPropertyKey, value); }
		}
		public double Length {
			get { return (double)GetValue(LengthProperty); }
			internal set { SetValue(LengthPropertyKey, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(CardsPanel.OrientationProperty); }
			set { SetValue(CardsPanel.OrientationProperty, value); }
		}
	}
}
