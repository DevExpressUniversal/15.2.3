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

using System.ComponentModel;
using System.Windows;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using System;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class LineFullStackedSeries2D : LineStackedSeries2D {
		public static readonly DependencyProperty PercentOptionsProperty = DependencyPropertyManager.RegisterAttached("PercentOptions",
				typeof(PercentOptions), typeof(LineFullStackedSeries2D), new PropertyMetadata(PointOptions.PercentOptionsPropertyChanged));
		[Obsolete(ObsoleteMessages.PercentOptionsProperty), EditorBrowsable(EditorBrowsableState.Never)]
		public static PercentOptions GetPercentOptions(PointOptions pointOptions) {
			return (PercentOptions)pointOptions.GetValue(PercentOptionsProperty);
		}
		[Obsolete(ObsoleteMessages.PercentOptionsProperty), EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetPercentOptions(PointOptions pointOptions, PercentOptions value) {
			pointOptions.SetValue(PercentOptionsProperty, value);
		}
		protected internal override ToolTipPointDataToStringConverter ToolTipPointValuesConverter { get { return new ToolTipFullStackedValueToStringConverter(this); } }
		protected override Type PointInterfaceType { get { return typeof(IFullStackedPoint); } }
		protected override string DefaultLabelTextPattern { get { return "{" + PatternUtils.PercentValuePlaceholder + ":G4}"; } }
		public LineFullStackedSeries2D() {
			DefaultStyleKey = typeof(LineFullStackedSeries2D);
		}
		protected override Series CreateObjectForClone() {
			return new LineFullStackedSeries2D();
		}
		protected override SeriesContainer CreateContainer() {
			return new FullStackedInteractionContainer(this, true);
		}
		protected override double GetMaxValue(RefinedPoint refinedPoint) {
			return ((IFullStackedPoint)refinedPoint).MaxValue;
		}
		protected internal override string ConstructValuePattern(PointOptionsContainerBase pointOptionsContainer, ScaleType valueScaleType) {
			PercentOptions percentOptions = pointOptionsContainer.GetPercentOptions(PercentOptionsProperty);
			return pointOptionsContainer.ConstructValuePatternFromPercentOptions(percentOptions, valueScaleType);
		}
		protected internal override string[] GetAvailablePointPatternPlaceholders() {
			return ToolTipPatternUtils.PercentViewPointPatterns;
		}
	}
}
