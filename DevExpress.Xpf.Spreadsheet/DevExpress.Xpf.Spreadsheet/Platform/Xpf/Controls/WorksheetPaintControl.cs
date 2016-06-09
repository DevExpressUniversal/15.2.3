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
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraSpreadsheet.Layout;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public class WorksheetPaintControl : Control {
		public WorksheetPaintControl() {
			this.Loaded += WorksheetPaintControlLoaded;
		}
		#region Properties
		protected internal DocumentLayout LayoutInfo { get { return Owner != null ? Owner.LayoutInfo : null; } }
		protected internal SpreadsheetPropertiesProvider SpreadsheetProvider { get { return GetValue(SpreadsheetViewControl.SpreadsheetProviderProperty) as SpreadsheetPropertiesProvider; } }
		protected internal WorksheetControl Owner { get; private set; }
		protected internal SpreadsheetControl Spreadsheet { get; private set; }
		#endregion
		protected void WorksheetPaintControlLoaded(object sender, RoutedEventArgs e) {
			Owner = LayoutHelper.FindParentObject<WorksheetControl>(this);
			Spreadsheet = LayoutHelper.FindParentObject<SpreadsheetControl>(this);
			InvalidateVisual();
		}
	}
}
