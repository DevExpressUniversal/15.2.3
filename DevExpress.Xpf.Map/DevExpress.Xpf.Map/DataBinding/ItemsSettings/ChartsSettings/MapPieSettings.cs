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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public class MapPieSettings : MapItemSettingsBase {
		public static readonly DependencyProperty RotationDirectionProperty = DependencyPropertyManager.Register("RotationDirection",
			typeof(RotationDirection), typeof(MapPieSettings), new PropertyMetadata(RotationDirection.Clockwise, UpdateItems));
		public static readonly DependencyProperty RotationAngleProperty = DependencyPropertyManager.Register("RotationAngle",
			typeof(double), typeof(MapPieSettings), new PropertyMetadata(0.0, UpdateItems));
		public static readonly DependencyProperty HoleRadiusPercentProperty = DependencyPropertyManager.Register("HoleRadiusPercent",
			typeof(double), typeof(MapPieSettings), new PropertyMetadata(0.0, UpdateItems), new ValidateValueCallback(MapPie.HoleRadiusPercentValidation));
		[Category(Categories.Layout)]
		public double RotationAngle {
			get { return (double)GetValue(RotationAngleProperty); }
			set { SetValue(RotationAngleProperty, value); }
		}
		[Category(Categories.Layout)]
		public RotationDirection RotationDirection {
			get { return (RotationDirection)GetValue(RotationDirectionProperty); }
			set { SetValue(RotationDirectionProperty, value); }
		}
		[Category(Categories.Layout)]
		public double HoleRadiusPercent {
			get { return (double)GetValue(HoleRadiusPercentProperty); }
			set { SetValue(HoleRadiusPercentProperty, value); }
		}
		protected override MapDependencyObject CreateObject() {
			return new MapPieSettings();
		}
		protected override MapItem CreateItemInstance() {
			return new MapPie();
		}
		protected override void FillPropertiesMappings(DependencyPropertyMappingDictionary mappings) {
			base.FillPropertiesMappings(mappings);
			mappings.Add(MapPieSettings.RotationAngleProperty, MapPie.RotationAngleProperty);
			mappings.Add(MapPieSettings.RotationDirectionProperty, MapPie.RotationDirectionProperty);
			mappings.Add(MapPieSettings.HoleRadiusPercentProperty, MapPie.HoleRadiusPercentProperty);
		}
	}
}
