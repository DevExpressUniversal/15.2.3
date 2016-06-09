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
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.WindowsUI.Base {
	public interface ILayoutCalculatorResult {
		Rect[] ItemRects { get; }
		Size TotalSize { get; }
	}
	public interface ILayoutCalculatorOptions {
		Size AvailableSize { get; }
		IItemHeaderInfo[] Headers { get; }
		double Spacing { get; }
	}
	public interface IItemHeaderInfo {
		Size Header { get; }
		bool IsSelected { get; }
	}
	public interface ILayoutCalculator {
		ILayoutCalculatorResult Measure(ILayoutCalculatorOptions options);
	}
	public interface ISelector {
		void Select(ISelectorItem item);
		void UpdateSelection();
		int SelectedIndex { get; set; }
		object SelectedItem { get; set; }
	}
	public interface ISelectorItem : IDisposable {
		ISelector Owner { get; set; }
		bool IsSelected { get; set; }
	}
	public interface IContentSelector : ISelector {
		DataTemplate ContentTemplate { get; set; }
		DataTemplateSelector ContentTemplateSelector { get; set; }
	}
	public interface IContentSelectorItem : ISelectorItem {
		object Content { get; }
		DataTemplate ContentTemplate { get; }
		DataTemplateSelector ContentTemplateSelector { get; set; }
	}
}
