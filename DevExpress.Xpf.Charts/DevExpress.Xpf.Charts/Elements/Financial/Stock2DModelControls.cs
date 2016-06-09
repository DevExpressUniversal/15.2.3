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
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class ThinStock2DModelControl : PredefinedModelControl {
		public ThinStock2DModelControl() {
			DefaultStyleKey = typeof(ThinStock2DModelControl);
		}
	}
	public class FlatStock2DModelControl : PredefinedModelControl {
		public FlatStock2DModelControl() {
			DefaultStyleKey = typeof(FlatStock2DModelControl);
		}
	}
	public class DropsStock2DModelControl : PredefinedModelControl {
		public DropsStock2DModelControl() {
			DefaultStyleKey = typeof(DropsStock2DModelControl);
		}
	}
	public class ArrowsStock2DModelControl : PredefinedModelControl {
		public ArrowsStock2DModelControl() {
			DefaultStyleKey = typeof(ArrowsStock2DModelControl);
		}
	}
	public class CustomStockModelControl : CustomModelControl {
		public static readonly DependencyProperty OpenLineTemplateProperty = CustomStock2DModel.OpenLineTemplateProperty.AddOwner(typeof(CustomStockModelControl));
		public static readonly DependencyProperty CloseLineTemplateProperty = CustomStock2DModel.CloseLineTemplateProperty.AddOwner(typeof(CustomStockModelControl));
		public static readonly DependencyProperty CenterLineTemplateProperty = CustomStock2DModel.CenterLineTemplateProperty.AddOwner(typeof(CustomStockModelControl));
		[
		Category(Categories.Presentation)
		]
		public DataTemplate OpenLineTemplate {
			get { return (DataTemplate)GetValue(OpenLineTemplateProperty); }
			set { SetValue(OpenLineTemplateProperty, value); }
		}
		[
		Category(Categories.Presentation)
		]
		public DataTemplate CloseLineTemplate {
			get { return (DataTemplate)GetValue(CloseLineTemplateProperty); }
			set { SetValue(CloseLineTemplateProperty, value); }
		}
		[
		Category(Categories.Presentation)
		]
		public DataTemplate CenterLineTemplate {
			get { return (DataTemplate)GetValue(CenterLineTemplateProperty); }
			set { SetValue(CenterLineTemplateProperty, value); }
		}
	}
}
