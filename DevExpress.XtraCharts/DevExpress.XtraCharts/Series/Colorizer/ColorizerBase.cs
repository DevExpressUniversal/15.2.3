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

using DevExpress.Utils.Serializing.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.XtraCharts {
	public abstract class ChartColorizerBase : IColorizer, IXtraSupportShouldSerialize {
		PropertyChangedEventHandler propertyChangedEventHandler;
		PropertyChangedEventHandler PropertyChangedEventHandler { get { return propertyChangedEventHandler; } }
		internal event EventHandler PropertyChanging;
		public abstract Color GetPointColor(object argument, object[] values, object colorKey, Palette palette);
		public virtual Color GetPointColor(object argument, object[] values, object[] colorKeys, Palette palette) { return GetPointColor(argument, values, colorKeys[0], palette); }
		public virtual Color GetAggregatedPointColor(object argument, object[] values, SeriesPoint[] points, Palette palette) { return Color.Empty; }
		public override string ToString() {
			return String.Format("({0})", GetType().Name);
		}
		#region IColorizer implementation
		Color IColorizer.GetPointColor(object argument, object[] values, object colorKey, Palette palette) { return GetPointColor(argument, values, colorKey, palette); }
		Color IColorizer.GetPointColor(object argument, object[] values, object[] colorKeys, Palette palette) { return GetPointColor(argument, values, colorKeys, palette); }
		Color IColorizer.GetAggregatedPointColor(object argument, object[] values, SeriesPoint[] points, Palette palette) { return GetAggregatedPointColor(argument, values, points, palette); }
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { propertyChangedEventHandler += value; }
			remove { propertyChangedEventHandler -= value; }
		}
		#endregion
		internal void RaisePropertyChanged(string propertyName) {
			if (PropertyChangedEventHandler != null)
				PropertyChangedEventHandler(this, new PropertyChangedEventArgs(propertyName));
		}
		internal void RaisePropertyChanging() {
			if (PropertyChanging != null)
				PropertyChanging(this, new EventArgs());
		}
		#region XtraSerializing
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			return XtraShouldSerialize(propertyName);
		}
		protected virtual bool XtraShouldSerialize(string propertyName) {
			return false;
		}
		#endregion
	}
}
