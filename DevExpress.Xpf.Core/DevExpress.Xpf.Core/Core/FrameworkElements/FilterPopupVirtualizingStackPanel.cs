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
using System;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core {
	public class DXVirtualizingStackPanel : VirtualizingStackPanel {
		public bool AllowClipping { get; set; }
		double maxMeasureWidth;
		double firstMeasureWidth;
		protected override Size MeasureOverride(Size constraint) {
			Size result = base.MeasureOverride(constraint);
			if (AllowClipping && firstMeasureWidth > 0d)
				result.Width = firstMeasureWidth;
			else {
				if (result.Width > maxMeasureWidth)
					maxMeasureWidth = result.Width;
				else
					result.Width = maxMeasureWidth;
			}
			if (Math.Abs(firstMeasureWidth) < double.Epsilon && !double.IsInfinity(result.Width)) {
				firstMeasureWidth = result.Width;
			}
			return result;
		}
	}
	public class FilterPopupVirtualizingStackPanel : DXVirtualizingStackPanel {
		public const int MaxNonVirtualItemsCount = 200;
		static ItemsPanelTemplate filterPopupVirtualizingStackPanelTemplate;
		static ItemsPanelTemplate stackPanelTemplate;
		static ItemsPanelTemplate FilterPopupVirtualizingStackPanelTemplate {
			get {
				return filterPopupVirtualizingStackPanelTemplate ?? (filterPopupVirtualizingStackPanelTemplate = CreateVirtualizingStackPanelTemplate());
			}
		}
		static ItemsPanelTemplate StackPanelTemplate { get { return stackPanelTemplate ?? (stackPanelTemplate = XamlHelper.GetItemsPanelTemplate("<StackPanel/>")); } }
		static ItemsPanelTemplate CreateVirtualizingStackPanelTemplate() {
			return XamlHelper.GetItemsPanelTemplate(string.Format(@"<{0}:{2} xmlns:{0}=""{1}""/>",
								XmlNamespaceConstants.UtilsPrefix, XmlNamespaceConstants.UtilsNamespaceDefinition, typeof(FilterPopupVirtualizingStackPanel).Name));
		}
		public static ItemsPanelTemplate GetItemsPanelTemplate(int recordCount) {
			return recordCount > FilterPopupVirtualizingStackPanel.MaxNonVirtualItemsCount ? FilterPopupVirtualizingStackPanelTemplate : StackPanelTemplate;
		}
	}
}
