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

  #if SILVERLIGHT
extern alias Platform;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design.Model;
using DevExpress.Xpf.Design;
using System.Windows;
#if SILVERLIGHT
using AssemblyInfo = Platform::AssemblyInfo;
using PivotGridControl = Platform.DevExpress.Xpf.PivotGrid.PivotGridControl;
using PivotGridField = Platform.DevExpress.Xpf.PivotGrid.PivotGridField;
using IPivotOLAPDataSource = Platform.DevExpress.XtraPivotGrid.Data.IPivotOLAPDataSource;
using PivotGridWpfData = Platform.DevExpress.Xpf.PivotGrid.Internal.PivotGridWpfData;
using DevExpress.Xpf.Core.Design.SmartTags;
using FrameworkElement = Platform.System.Windows.FrameworkElement;
using FieldHeader = Platform::DevExpress.Xpf.PivotGrid.Internal.FieldHeader;
using LayoutHelper = Platform::DevExpress.Xpf.Core.Native.LayoutHelper;
using DevExpress.Design.SmartTags;
#else
using DevExpress.Xpf.Core.Design.SmartTags;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.Xpf.PivotGrid.Design {
	public class PivotGridFieldProvider : IViewProvider {
		public FrameworkElement ProvideView(ModelItem item) {
			if(item.Parent.View == null)
				return null;
			PivotGridField field = item.GetCurrentValue() as PivotGridField;
			return LayoutHelper.FindElement(item.Parent.GetCurrentValue() as PivotGridControl, (d) => d is FieldHeader && ((FieldHeader)d).Field == field) as FrameworkElement;
		}
	}
}
