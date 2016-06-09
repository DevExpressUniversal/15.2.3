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
using System.ComponentModel;
using DevExpress.ChartRangeControlClient.Core;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors.Design;
using DevExpress.Sparkline.Core;
namespace DevExpress.XtraEditors {
	[TypeConverter(typeof(ChartRangeControlClientRangeTypeConverter))]
	public abstract class ChartRangeControlClientRange {
		const bool defaultAuto = true;
		bool auto = defaultAuto;
		ChartRangeControlClientBase client;
		protected abstract object MinInternal { get; set; }
		protected abstract object MaxInternal { get; set; }
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.ClientRange.Auto"),
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All)
		]
		public bool Auto {
			get { return auto; }
			set {
				if (auto != value) {
					auto = value;
					UpdateWithDataArgumentRange();
					if (auto)
						client.SetCustomRange(new Tuple<object, object>(null, null));
				}
			}
		}
		#region ShouldSerialize & Reset
		protected bool ShouldSerializeAuto() {
			return auto != defaultAuto;
		}
		void ResetAuto() {
			Auto = defaultAuto;
		}
		#endregion
		protected abstract void ValidateCore();
		protected void MinMaxChanged() {
			auto = false;
			Validate();
			client.SetCustomRange(new Tuple<object, object>(MinInternal, MaxInternal));
		}
		protected void Update() {
			client.InvalidateRangeControl(false);
		}
		internal void UpdateWithDataArgumentRange() {
			Tuple<object, object> argumentRange = client.GetArgumentRange();
			MinInternal = argumentRange.Item1;
			MaxInternal = argumentRange.Item2;
		}
		internal void Validate() {
			if (!client.Loading)
				ValidateCore();
		}
		internal void SetClient(ChartRangeControlClientBase client) {
			this.client = client;
		}
		public override string ToString() {
			return "(" + this.GetType().Name + ")";
		}
	}
	public class DateTimeChartRangeControlClientRange : ChartRangeControlClientRange {
		DateTime min;
		DateTime max;
		protected override object MinInternal {
			get { return min; }
			set { min = (DateTime)value; }
		}
		protected override object MaxInternal {
			get { return max; }
			set { max = (DateTime)value; }
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.DateTimeClientRange.Min"),
		XtraSerializableProperty
		]
		public DateTime Min {
			get { return min; }
			set {
				if (min != value) {
					min = value;
					MinMaxChanged();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.DateTimeClientRange.Max"),
		XtraSerializableProperty
		]
		public DateTime Max {
			get { return max; }
			set {
				if (max != value) {
					max = value;
					MinMaxChanged();
				}
			}
		}
		#region ShouldSerialize
		bool ShouldSerializeMin() {
			return ShouldSerializeAuto();
		}
		bool ShouldSerializeMax() {
			return ShouldSerializeAuto();
		}
		#endregion
		protected override void ValidateCore() {
			if (min > max)
				throw new ArgumentException("Max should be greater than or equal to min");
		}
	}
	public class NumericChartRangeControlClientRange : ChartRangeControlClientRange {
		double min;
		double max;
		protected override object MinInternal {
			get { return min; }
			set { min = Convert.ToDouble(value); }
		}
		protected override object MaxInternal {
			get { return max; }
			set { max = Convert.ToDouble(value); }
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.NumericClientRange.Min"),
		XtraSerializableProperty
		]
		public double Min {
			get { return min; }
			set {
				if (min != value) {
					min = value;
					MinMaxChanged();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.NumericClientRange.Max"),
		XtraSerializableProperty
		]
		public double Max {
			get { return max; }
			set {
				if (max != value) {
					max = value;
					MinMaxChanged();
				}
			}
		}
		#region ShouldSerialize
		bool ShouldSerializeMin() {
			return ShouldSerializeAuto();
		}
		bool ShouldSerializeMax() {
			return ShouldSerializeAuto();
		}
		#endregion
		protected override void ValidateCore() {
			if (min > max)
				throw new ArgumentException("Max should be greater than or equal to min");
		}
	}
}
