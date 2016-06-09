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

namespace DevExpress.XtraCharts.Native {
	public static class SeriesScaleTypeUtils {
		public static void CheckScaleTypes(SeriesBase series, SeriesViewBase view) {
			if (series.View != null && series.View.GetType() == view.GetType())
				return;
			if (series.ActualArgumentScaleType != ScaleType.Numerical && !view.NonNumericArgumentSupported)
				series.ValidatePointsByArgumentScaleType(ScaleType.Numerical);
			if (series.ValueScaleType == ScaleType.DateTime && !view.DateTimeValuesSupported)
				series.ValidatePointsByValueScaleType(ScaleType.Numerical);
		}
		public static void UpdateScaleTypes(SeriesBase series, SeriesViewBase view) {
			if (series.ArgumentScaleType != ScaleType.Numerical && !view.NonNumericArgumentSupported)
				series.SetArgumentScaleType(ScaleType.Numerical);
			if (!view.DateTimeValuesSupported && series.ValueScaleType == ScaleType.DateTime)
				series.ValueScaleType = ScaleType.Numerical;
		}
		public static void ValidatePointsByArgumentScaleType(SeriesBase series, ScaleType argumentScaleType) {
			series.ValidatePointsByArgumentScaleType(argumentScaleType);
		}
		public static void ValidatePointsByValueScaleType(SeriesBase series, ScaleType valueScaleType) {
			series.ValidatePointsByValueScaleType(valueScaleType);
		}
	}
}
