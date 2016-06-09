#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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
using DevExpress.XtraSpreadsheet.Model;
using System.ComponentModel;
namespace DevExpress.UnitConversion {
	#region Units
	public static class Units {
		readonly static MassUnitsConverter mass = new MassUnitsConverter();
		readonly static DistanceUnitsConverter distance = new DistanceUnitsConverter();
		readonly static TimeUnitsConverter time = new TimeUnitsConverter();
		readonly static PressureUnitsConverter pressure = new PressureUnitsConverter();
		readonly static ForceUnitsConverter force = new ForceUnitsConverter();
		readonly static EnergyUnitsConverter energy = new EnergyUnitsConverter();
		readonly static PowerUnitsConverter power = new PowerUnitsConverter();
		readonly static MagnetismUnitsConverter magnetism = new MagnetismUnitsConverter();
		readonly static TemperatureUnitsConverter temperature = new TemperatureUnitsConverter();
		readonly static VolumeUnitsConverter volume = new VolumeUnitsConverter();
		readonly static AreaUnitsConverter area = new AreaUnitsConverter();
		readonly static InformationUnitsConverter information = new InformationUnitsConverter();
		readonly static SpeedUnitsConverter speed = new SpeedUnitsConverter();
		readonly static MetricUnitsConverter metric = new MetricUnitsConverter();
		readonly static BinaryUnitsConverter binary = new BinaryUnitsConverter();
#if !SL
	[DevExpressDocsLocalizedDescription("UnitsMass")]
#endif
		public static MassUnitsConverter Mass { get { return mass; } }
#if !SL
	[DevExpressDocsLocalizedDescription("UnitsDistance")]
#endif
		public static DistanceUnitsConverter Distance { get { return distance; } }
#if !SL
	[DevExpressDocsLocalizedDescription("UnitsTime")]
#endif
		public static TimeUnitsConverter Time { get { return time; } }
#if !SL
	[DevExpressDocsLocalizedDescription("UnitsPressure")]
#endif
		public static PressureUnitsConverter Pressure { get { return pressure; } }
#if !SL
	[DevExpressDocsLocalizedDescription("UnitsForce")]
#endif
		public static ForceUnitsConverter Force { get { return force; } }
#if !SL
	[DevExpressDocsLocalizedDescription("UnitsEnergy")]
#endif
		public static EnergyUnitsConverter Energy { get { return energy; } }
#if !SL
	[DevExpressDocsLocalizedDescription("UnitsPower")]
#endif
		public static PowerUnitsConverter Power { get { return power; } }
#if !SL
	[DevExpressDocsLocalizedDescription("UnitsMagnetism")]
#endif
		public static MagnetismUnitsConverter Magnetism { get { return magnetism; } }
#if !SL
	[DevExpressDocsLocalizedDescription("UnitsTemperature")]
#endif
		public static TemperatureUnitsConverter Temperature { get { return temperature; } }
#if !SL
	[DevExpressDocsLocalizedDescription("UnitsVolume")]
#endif
		public static VolumeUnitsConverter Volume { get { return volume; } }
#if !SL
	[DevExpressDocsLocalizedDescription("UnitsArea")]
#endif
		public static AreaUnitsConverter Area { get { return area; } }
#if !SL
	[DevExpressDocsLocalizedDescription("UnitsInformation")]
#endif
		public static InformationUnitsConverter Information { get { return information; } }
#if !SL
	[DevExpressDocsLocalizedDescription("UnitsSpeed")]
#endif
		public static SpeedUnitsConverter Speed { get { return speed; } }
#if !SL
	[DevExpressDocsLocalizedDescription("UnitsMetric")]
#endif
		public static MetricUnitsConverter Metric { get { return metric; } }
#if !SL
	[DevExpressDocsLocalizedDescription("UnitsBinary")]
#endif
		public static BinaryUnitsConverter Binary { get { return binary; } }
	}
	#endregion
	#region BaseUnitsConverter<T>
	public abstract class BaseUnitsConverter<T> where T : struct {
		protected abstract UnitCategory Category { get; }
		protected abstract Dictionary<T, string> UnitNameTable { get; }
		public Func<double, double> GetTransform(T from, T to) {
			int fromIndex = Category.UnitIndices[UnitNameTable[from]];
			int toIndex = Category.UnitIndices[UnitNameTable[to]];
			UnitCategory category = Category;
			return (double x) => category.Convert(x, fromIndex, toIndex);
		}
		public double Convert(double value, T from, T to) {
			return Category.Convert(value, UnitNameTable[from], UnitNameTable[to]);
		}
		public void Convert(double[] values, T from, T to) {
			int fromIndex = Category.UnitIndices[UnitNameTable[from]];
			int toIndex = Category.UnitIndices[UnitNameTable[to]];
			Category.Convert(values, fromIndex, toIndex);
		}
		public void Convert(IList<double> values, T from, T to) {
			int fromIndex = Category.UnitIndices[UnitNameTable[from]];
			int toIndex = Category.UnitIndices[UnitNameTable[to]];
			Category.Convert(values, fromIndex, toIndex);
		}
	}
	#endregion
	#region PrefixUnitsConverter<T>
	public abstract class PrefixUnitsConverter<T> where T : struct {
		protected abstract Dictionary<string, double> Multipliers { get; }
		protected abstract Dictionary<T, string> UnitNameTable { get; }
		protected abstract T Empty { get; }
		public Func<double, double> GetTransform(T from, T to) {
			return (double x) => Convert(x, from, to);
		}
		double GetMultiplier(T prefix) {
			if (Object.Equals(prefix, Empty))
				return 1;
			return Multipliers[UnitNameTable[prefix]];
		}
		[Obsolete("Please use the 'void Convert(double value, T from, T to)'", true), EditorBrowsable(EditorBrowsableState.Never)]
		public double Convert(double value, T prefix) {
			return 0;
		}
		[Obsolete("Please use the 'void Convert(double[] values, T from, T to)'", true), EditorBrowsable(EditorBrowsableState.Never)]
		public void Convert(double[] values, T prefix) {
		}
		[Obsolete("Please use the 'void Convert(IList<double> values, T from, T to)'", true), EditorBrowsable(EditorBrowsableState.Never)]
		public void Convert(IList<double> values, T prefix) {
		}
		double ConvertCore(double value, double multiplier, double divisor) {
			return (double)((double)(value * multiplier) / divisor);
		}
		public double Convert(double value, T from, T to) {
			return ConvertCore(value, GetMultiplier(from), GetMultiplier(to));
		}
		public void Convert(double[] values, T from, T to) {
			double multiplier = GetMultiplier(from);
			double divisor = GetMultiplier(to);
			int count = values.Length;
			for (int i = 0; i < count; i++)
				values[i] = ConvertCore(values[i], multiplier, divisor);
		}
		public void Convert(IList<double> values, T from, T to) {
			double multiplier = GetMultiplier(from);
			double divisor = GetMultiplier(to);
			int count = values.Count;
			for (int i = 0; i < count; i++)
				values[i] = ConvertCore(values[i], multiplier, divisor);
		}
	}
	#endregion
}
