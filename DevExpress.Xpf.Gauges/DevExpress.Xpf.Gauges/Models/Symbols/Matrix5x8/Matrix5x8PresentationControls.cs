﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public class DefaultMatrix5x8Control : SymbolPresentationControl {
		public DefaultMatrix5x8Control() {
			DefaultStyleKey = typeof(DefaultMatrix5x8Control);
		}
	}
	[NonCategorized]
	public class CustomMatrix5x8Control : SymbolPresentationControl {
		public static readonly DependencyProperty ActiveSegmentTemplateProperty = DependencyPropertyManager.Register("ActiveSegmentTemplate",
		typeof(DataTemplate), typeof(CustomMatrix5x8Control));
		public static readonly DependencyProperty InactiveSegmentTemplateProperty = DependencyPropertyManager.Register("InactiveSegmentTemplate",
		typeof(DataTemplate), typeof(CustomMatrix5x8Control));
		public DataTemplate ActiveSegmentTemplate {
			get { return (DataTemplate)GetValue(ActiveSegmentTemplateProperty); }
			set { SetValue(ActiveSegmentTemplateProperty, value); }
		}
		public DataTemplate InactiveSegmentTemplate {
			get { return (DataTemplate)GetValue(InactiveSegmentTemplateProperty); }
			set { SetValue(InactiveSegmentTemplateProperty, value); }
		}
		public CustomMatrix5x8Control() {
			DefaultStyleKey = typeof(CustomMatrix5x8Control);
		}
	}
}
