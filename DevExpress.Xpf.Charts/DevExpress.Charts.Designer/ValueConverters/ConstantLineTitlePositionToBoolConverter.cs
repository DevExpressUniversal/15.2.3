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
using System.Globalization;
using System.Windows.Data;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public sealed class ConstantLineTitlePositionToBoolConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			WpfChartConstantLineModel model = value as WpfChartConstantLineModel;
			if (model == null)
				return false;
			if (model.ConstantLine.Title == null && parameter is ChangeConstantLineTitlePositionNone)
				return true;
			else if (model.ConstantLine.Title == null && !(parameter is ChangeConstantLineTitlePositionNone))
				return false;
			ConstantLineTitleAlignment alignment = model.TitleAlignment;
			bool belowLine = model.ShowTitleBelowLine;
			ConstantLine constantLine = ((ConstantLineCommandBase)parameter).SelectedConstantLine;
			Axis2D constantLineOwner = ChartDesignerPropertiesProvider.GetConstantLineOwner(constantLine);
			if (alignment == ConstantLineTitleAlignment.Near && belowLine == false && IsVertical(constantLine) && parameter is ChangeConstantLineTitlePositionNearAboveVertical)
				return true;
			else if (alignment == ConstantLineTitleAlignment.Near && belowLine == true && IsVertical(constantLine) && parameter is ChangeConstantLineTitlePositionNearBelowVertical)
				return true;
			else if (alignment == ConstantLineTitleAlignment.Far && belowLine == false && IsVertical(constantLine) && parameter is ChangeConstantLineTitlePositionFarAboveVertical)
				return true;
			else if (alignment == ConstantLineTitleAlignment.Far && belowLine == true && IsVertical(constantLine) && parameter is ChangeConstantLineTitlePositionFarBelowVertical)
				return true;
			else if (alignment == ConstantLineTitleAlignment.Near && belowLine == false && !IsVertical(constantLine) && parameter is ChangeConstantLineTitlePositionNearAboveHorizontal)
				return true;
			else if (alignment == ConstantLineTitleAlignment.Near && belowLine == true && !IsVertical(constantLine) && parameter is ChangeConstantLineTitlePositionNearBelowHorizontal)
				return true;
			else if (alignment == ConstantLineTitleAlignment.Far && belowLine == false && !IsVertical(constantLine) && parameter is ChangeConstantLineTitlePositionFarAboveHorizontal)
				return true;
			else if (alignment == ConstantLineTitleAlignment.Far && belowLine == true && !IsVertical(constantLine) && parameter is ChangeConstantLineTitlePositionFarBelowHorizontal)
				return true;
			else
				return false;
		}
		bool IsVertical(ConstantLine constantLine) {
			Axis2D axis = ChartDesignerPropertiesProvider.GetConstantLineOwner(constantLine);
			return !ChartDesignerPropertiesProvider.IsAxisVertical(axis);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
}
