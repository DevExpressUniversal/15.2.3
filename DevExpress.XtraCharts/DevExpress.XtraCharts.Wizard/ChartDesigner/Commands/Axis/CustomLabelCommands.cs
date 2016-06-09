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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public class AddCustomAxisLabelCommand : AddCommandBase<CustomAxisLabel> {
		readonly CustomAxisLabelCollection customAxisLabelCollection;
		protected override ChartCollectionBase ChartCollection { get { return customAxisLabelCollection; } }
		public AddCustomAxisLabelCommand(CommandManager commandManager, CustomAxisLabelCollection labelCollection)
			: base(commandManager) {
			this.customAxisLabelCollection = labelCollection;
		}
		protected override CustomAxisLabel CreateChartElement(object parameter) {
			object owner = ((IOwnedElement)customAxisLabelCollection).Owner;
			IAxisData axis = (IAxisData)owner;
			return new CustomAxisLabel(string.Empty, axis.AxisScaleTypeMap.DefaultAxisValue);
		}
		protected override void AddToCollection(CustomAxisLabel chartElement) {
			customAxisLabelCollection.Add(chartElement);
		}
	}
	public class DeleteCustomAxisLabelCommand : DeleteCommandBase<CustomAxisLabel> {
		readonly CustomAxisLabelCollection customAxisLabelCollection;
		protected override ChartCollectionBase ChartCollection { get { return customAxisLabelCollection; } }
		public DeleteCustomAxisLabelCommand(CommandManager commandManager, CustomAxisLabelCollection scaleBreakCollection)
			: base(commandManager) {
			this.customAxisLabelCollection = scaleBreakCollection;
		}
		protected override void InsertIntoCollection(int index, CustomAxisLabel chartElement) {
			((IList)customAxisLabelCollection).Insert(index, chartElement);
		}
	}
}
