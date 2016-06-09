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

using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class SimpleCandleStick2DModelControl : PredefinedModelControl {
		public SimpleCandleStick2DModelControl() {
			DefaultStyleKey = typeof(SimpleCandleStick2DModelControl);
		}
	}
	public class BorderCandleStick2DModelControl : PredefinedModelControl {
		public BorderCandleStick2DModelControl() {
			DefaultStyleKey = typeof(BorderCandleStick2DModelControl);
		}
	}
	public class ThinCandleStick2DModelControl : PredefinedModelControl {
		public ThinCandleStick2DModelControl() {
			DefaultStyleKey = typeof(ThinCandleStick2DModelControl);
		}
	}
	public class FlatCandleStick2DModelControl : PredefinedModelControl {
		public FlatCandleStick2DModelControl() {
			DefaultStyleKey = typeof(FlatCandleStick2DModelControl);
		}
	}
	public class GlassCandleStick2DModelControl : PredefinedModelControl {
		public GlassCandleStick2DModelControl() {
			DefaultStyleKey = typeof(GlassCandleStick2DModelControl);
		}
	}
	public class CustomCandleStickModelControl : CustomModelControl {
		public static readonly DependencyProperty TopStickTemplateProperty = CustomCandleStick2DModel.TopStickTemplateProperty.AddOwner(typeof(CustomCandleStickModelControl));
		public static readonly DependencyProperty BottomStickTemplateProperty = CustomCandleStick2DModel.BottomStickTemplateProperty.AddOwner(typeof(CustomCandleStickModelControl));
		public static readonly DependencyProperty CandleTemplateProperty = CustomCandleStick2DModel.CandleTemplateProperty.AddOwner(typeof(CustomCandleStickModelControl));
		public static readonly DependencyProperty InvertedCandleTemplateProperty = CustomCandleStick2DModel.InvertedCandleTemplateProperty.AddOwner(typeof(CustomCandleStickModelControl));
		[
		Category(Categories.Common)
		]
		public DataTemplate TopStickTemplate {
			get { return (DataTemplate)GetValue(TopStickTemplateProperty); }
			set { SetValue(TopStickTemplateProperty, value); }
		}
		[
		Category(Categories.Common)
		]
		public DataTemplate BottomStickTemplate {
			get { return (DataTemplate)GetValue(BottomStickTemplateProperty); }
			set { SetValue(BottomStickTemplateProperty, value); }
		}
		[
		Category(Categories.Common)
		]
		public DataTemplate CandleTemplate {
			get { return (DataTemplate)GetValue(CandleTemplateProperty); }
			set { SetValue(CandleTemplateProperty, value); }
		}
		[
		Category(Categories.Common)
		]
		public DataTemplate InvertedCandleTemplate {
			get { return (DataTemplate)GetValue(InvertedCandleTemplateProperty); }
			set { SetValue(InvertedCandleTemplateProperty, value); }
		}
	}
}
