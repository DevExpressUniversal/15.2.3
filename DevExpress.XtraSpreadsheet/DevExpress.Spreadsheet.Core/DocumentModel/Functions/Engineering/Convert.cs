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
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class FunctionConvert : WorksheetFunctionBase {
		#region Static members
		static readonly UnitCategory mass = new MassUnitCategory();
		static readonly UnitCategory distance = new DistanceUnitCategory();
		static readonly UnitCategory time = new TimeUnitCategory();
		static readonly UnitCategory pressure = new PressureUnitCategory();
		static readonly UnitCategory force = new ForceUnitCategory();
		static readonly UnitCategory energy = new EnergyUnitCategory();
		static readonly UnitCategory power = new PowerUnitCategory();
		static readonly UnitCategory magnetism = new MagnetismUnitCategory();
		static readonly UnitCategory temperature = new TemperatureUnitCategory();
		static readonly UnitCategory volume = new VolumeUnitCategory();
		static readonly UnitCategory area = new AreaUnitCategory();
		static readonly UnitCategory information = new InformationUnitCategory();
		static readonly UnitCategory speed = new SpeedUnitCategory();
		static readonly Dictionary<string, double> metric = CreateMetric();
		static readonly Dictionary<string, double> binary = CreateBinary();
		static readonly List<UnitCategory> unitCategoryList = CreateUnitCategotyList();
		public static UnitCategory Mass { get { return mass; } }
		public static UnitCategory Distance { get { return distance; } }
		public static UnitCategory Time { get { return time; } }
		public static UnitCategory Pressure { get { return pressure; } }
		public static UnitCategory Force { get { return force; } }
		public static UnitCategory Energy { get { return energy; } }
		public static UnitCategory Power { get { return power; } }
		public static UnitCategory Magnetism { get { return magnetism; } }
		public static UnitCategory Temperature { get { return temperature; } }
		public static UnitCategory Volume { get { return volume; } }
		public static UnitCategory Area { get { return area; } }
		public static UnitCategory Information { get { return information; } }
		public static UnitCategory Speed { get { return speed; } }
		public static Dictionary<string, double> Metric { get { return metric; } }
		public static Dictionary<string, double> Binary { get { return binary; } }
		public static List<UnitCategory> UnitCategoryList { get { return unitCategoryList; } }
		static Dictionary<string, double> CreateMetric() {
			Dictionary<string, double> result = new Dictionary<string, double>();
			result.Add("Y", 1E+24); 
			result.Add("Z", 1E+21); 
			result.Add("E", 1E+18); 
			result.Add("P", 1E+15); 
			result.Add("T", 1E+12); 
			result.Add("G", 1E+09); 
			result.Add("M", 1E+06); 
			result.Add("k", 1E+03); 
			result.Add("h", 1E+02); 
			result.Add("da", 1E+01); 
			result.Add("e", 1E+01); 
			result.Add("d", 1E-01); 
			result.Add("c", 1E-02); 
			result.Add("m", 1E-03); 
			result.Add("u", 1E-06); 
			result.Add("n", 1E-09); 
			result.Add("p", 1E-12); 
			result.Add("f", 1E-15); 
			result.Add("a", 1E-18); 
			result.Add("z", 1E-21); 
			result.Add("y", 1E-24); 
			return result;
		}
		static Dictionary<string, double> CreateBinary() {
			Dictionary<string, double> result = new Dictionary<string, double>();
			result.Add("Yi", Math.Pow(2, 80)); 
			result.Add("Zi", Math.Pow(2, 70)); 
			result.Add("Ei", 1152921504606846976); 
			result.Add("Pi", 1125899906842624); 
			result.Add("Ti", 1099511627776); 
			result.Add("Gi", 1073741824); 
			result.Add("Mi", 1048576); 
			result.Add("ki", 1024); 
			return result;
		}
		static List<UnitCategory> CreateUnitCategotyList() {
			List<UnitCategory> result = new List<UnitCategory>();
			result.Add(mass);
			result.Add(distance);
			result.Add(time);
			result.Add(pressure);
			result.Add(force);
			result.Add(energy);
			result.Add(power);
			result.Add(magnetism);
			result.Add(temperature);
			result.Add(volume);
			result.Add(area);
			result.Add(information);
			result.Add(speed);
			return result;
		}
		#endregion
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			return collection;
		}
		public override int Code { get { return 0x01D4; } }
		public override string Name { get { return "CONVERT"; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			foreach (VariantValue value in arguments)
				if (value.IsBoolean)
					return VariantValue.ErrorInvalidValueInFunction;
			VariantValue number = arguments[0].ToNumeric(context);
			if(number.IsError)
				return number;
			double numberValue = number.NumericValue;
			VariantValue unitFrom = arguments[1].ToText(context);
			if(unitFrom.IsError)
				return unitFrom;
			string unitFromValue = unitFrom.InlineTextValue;
			VariantValue unitTo = arguments[2].ToText(context);
			if(unitTo.IsError)
				return unitTo;
			string unitToValue = unitTo.InlineTextValue;
			FunctionConvertParsedUnit parsedUnitFrom = ParseUnit(unitFromValue);
			if(!parsedUnitFrom.IsValid())
				return VariantValue.ErrorValueNotAvailable;
			FunctionConvertParsedUnit parsedUnitTo = ParseUnit(unitToValue);
			if(!parsedUnitTo.IsValid())
				return VariantValue.ErrorValueNotAvailable;
			if(parsedUnitFrom.CategoryId != parsedUnitTo.CategoryId)
				return VariantValue.ErrorValueNotAvailable;
			UnitCategory unitCategory = UnitCategoryList[parsedUnitFrom.CategoryId];
			double scaleFactor = parsedUnitFrom.Multiplier / parsedUnitTo.Multiplier;
			numberValue *= scaleFactor;
			return unitCategory.Convert(numberValue, parsedUnitFrom.UnitId, parsedUnitTo.UnitId);
		}
		protected int LocateUnitValueCore(string value, UnitCategory category, bool metricOnly) {
			int unitCode;
			if(category.UnitIndices.TryGetValue(value, out unitCode)) {
				if(!metricOnly || category.IsMetricUnit(unitCode)) {
					return unitCode;
				}
			}
			return -1;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")]
		protected bool LocateUnitValue(string value, ref FunctionConvertParsedUnit unit, bool metricOnly) {
			for(int i = 0; i < UnitCategoryList.Count; ++i) {
				UnitCategory currentCategory = UnitCategoryList[i];
				int unitCode = LocateUnitValueCore(value, currentCategory, metricOnly);
				if(unitCode >= 0) {
					unit.CategoryId = i;
					unit.UnitId = unitCode;
					unit.MetricPower = currentCategory.GetMetricPower(unitCode);
					return true;
				}
			}
			return false;
		}
		double ApplyMetricPower(double value, int metricPower) {
			if(metricPower > 1)
				return Math.Pow(value, metricPower);
			return value;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")]
		protected bool TryMetricPrefixes(string value, ref FunctionConvertParsedUnit result) {
			foreach(KeyValuePair<string, double> prefix in Metric) {
				string prefixValue = prefix.Key;
				int prefixLength = prefixValue.Length;
				if(value.Length < prefixLength)
					continue;
				if(value.Substring(0, prefixLength) != prefixValue)
					continue;
				string unitValue = value.Substring(prefixLength);
				if(LocateUnitValue(unitValue, ref result, true)) {
					result.Multiplier = ApplyMetricPower(prefix.Value, result.MetricPower);
					return true;
				}
			}
			return false;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")]
		protected bool TryBinaryPrefixes(string value, ref FunctionConvertParsedUnit result) {
			foreach(KeyValuePair<string, double> prefix in Binary) {
				string prefixValue = prefix.Key;
				int prefixLength = prefixValue.Length;
				if(value.Length < prefixLength)
					continue;
				if(value.Substring(0, prefixLength) != prefixValue)
					continue;
				string unitValue = value.Substring(prefixLength);
				int unitCode = LocateUnitValueCore(unitValue, Information, true);
				if(unitCode >= 0) {
					result.CategoryId = unitCategoryList.IndexOf(Information);
					result.UnitId = unitCode;
					result.Multiplier = prefix.Value;
					return true;
				}
			}
			return false;
		}
		protected FunctionConvertParsedUnit ParseUnit(string value) {
			FunctionConvertParsedUnit result = FunctionConvertParsedUnit.CreateInstance();
			if((value.Length < 2) || (!TryMetricPrefixes(value, ref result) && !TryBinaryPrefixes(value, ref result)))
				LocateUnitValue(value, ref result, false);
			return result;
		}
	}
	public struct FunctionConvertParsedUnit {
		public int CategoryId { get; set; }
		public int UnitId { get; set; }
		public int MetricPower { get; set; }
		public double Multiplier { get; set; }
		public bool IsValid() {
			return (CategoryId >= 0) && (UnitId >= 0);
		}
		public static FunctionConvertParsedUnit CreateInstance() {
			return new FunctionConvertParsedUnit() { CategoryId = -1, UnitId = -1, MetricPower=0, Multiplier = 1.0 };
		}
	}
	#region UnitCategory (abstract class)
	public abstract class UnitCategory {
		protected internal int UniqueUnitsCount { get { return MetricPowerTable.Length; } }
		public abstract Dictionary<string, int> UnitIndices { get; }
		protected internal abstract int[] MetricPowerTable { get; }
		public double Convert(double value, string from, string to) {
			int fromIndex = UnitIndices[from];
			int toIndex = UnitIndices[to];
			return Convert(value, fromIndex, toIndex);
		}
		public bool IsMetricUnit(string unit) {
			return IsMetricUnit(UnitIndices[unit]);
		}
		public bool IsMetricUnit(int unitIndex) {
			return MetricPowerTable[unitIndex] > 0;
		}
		public virtual int GetMetricPower(int unitIndex) {
			return MetricPowerTable[unitIndex]; 
		}
		public abstract double Convert(double value, int fromIndex, int toIndex);
		public abstract void Convert(double[] values, int fromIndex, int toIndex);
		public abstract void Convert(IList<double> values, int fromIndex, int toIndex);
	}
	#endregion
	#region SimpleUnitCategory
	abstract class SimpleUnitCategory : UnitCategory {
		protected abstract double[,] Multipliers { get; }
		protected abstract bool CheckIndex(int index);
		#region Helper functions
		double ConvertCore(double value, int fromIndex, int toIndex) {
			double multiplier = Multipliers[fromIndex, toIndex];
			double result = value * multiplier;
			return result;
		}
		void CheckIndices(int fromIndex, int toIndex) {
			if(!CheckIndex(fromIndex))
				Exceptions.ThrowArgumentOutOfRangeException("fromIndex", "Incorrect value");
			if(!CheckIndex(toIndex))
				Exceptions.ThrowArgumentOutOfRangeException("toIndex", "Incorrect value");
		}
		#endregion
		public override double Convert(double value, int fromIndex, int toIndex) {
			CheckIndices(fromIndex, toIndex);
			if(fromIndex == toIndex)
				return value;
			return ConvertCore(value, fromIndex, toIndex);
		}
		public override void Convert(double[] values, int fromIndex, int toIndex) {
			CheckIndices(fromIndex, toIndex);
			if(fromIndex == toIndex)
				return;
			for(int i = 0; i < values.Length; ++i)
				values[i] = ConvertCore(values[i], fromIndex, toIndex);
		}
		public override void Convert(IList<double> values, int fromIndex, int toIndex) {
			CheckIndices(fromIndex, toIndex);
			if(fromIndex == toIndex)
				return;
			for(int i = 0; i < values.Count; ++i)
				values[i] = ConvertCore(values[i], fromIndex, toIndex);
		}
	}
	#endregion
	#region MassUnitCategory
	class MassUnitCategory : SimpleUnitCategory {
		#region Fields
		const int numberOfMultipliers = 11;
		static readonly double[,] multipliers = new double[numberOfMultipliers, numberOfMultipliers] {
			{				  1, 6.8521765856791766E-05,  0.0022046226218487759, 6.0221417942167639E+23,  0.035273961949580414,	  15.43235835294143, 2.2046226218487758E-05, 1.9684130552221215E-05, 0.00015747304441776972,  1.102311310924388E-06, 9.8420652761106068E-07 }, 
			{ 14593.902937206363,					  1,	 32.174048556430442, 8.7886552818893226E+27,	514.78477690288707,	 225218.33989501311,	 0.3217404855643044,	0.28726829068241466,	 2.2981463254593173,	0.01608702427821522,   0.014363414534120733 }, 
			{		  453.59237,   0.031080950171567256,					  1, 2.7315975689148344E+26,					16,				   7000,				   0.01,	0.00892857142857143,   0.071428571428571438,				 0.0005, 0.00044642857142857147 }, 
			{	1.660538782E-24, 1.1378304961632617E-28, 3.6608613632544125E-27,					  1,  5.85737818120706E-26, 2.5626029542780888E-23, 3.6608613632544126E-29,   3.26862621719144E-29, 2.6149009737531522E-28, 1.8304306816272066E-30, 1.6343131085957202E-30 }, 
			{	   28.349523125,  0.0019425593857229535,				 0.0625, 1.7072484805717715E+25,					 1,				  437.5,			   0.000625, 0.00055803571428571436,  0.0044642857142857149,			  3.125E-05, 2.7901785714285717E-05 }, 
			{		 0.06479891, 4.4401357387953223E-06, 0.00014285714285714287, 3.9022822413069061E+22, 0.0022857142857142859,					  1, 1.4285714285714286E-06, 1.2755102040816327E-06, 1.0204081632653061E-05, 7.1428571428571437E-08, 6.3775510204081641E-08 }, 
			{		  45359.237,	 3.1080950171567259,					100, 2.7315975689148345E+28,				  1600,				 700000,					  1,	 0.8928571428571429,	 7.1428571428571432,				   0.05,   0.044642857142857144 }, 
			{		50802.34544,	 3.4810664192155327,	 111.99999999999999, 3.0593892771846142E+28,	1791.9999999999998,				 784000,	 1.1199999999999999,					  1,					  8,   0.055999999999999994,   0.049999999999999996 }, 
			{		 6350.29318,	0.43513330240194159,	 13.999999999999998, 3.8242365964807678E+27,	223.99999999999997,				  98000,	0.13999999999999999,				  0.125,					  1,  0.0069999999999999993,  0.0062499999999999995 }, 
			{		  907184.74,	 62.161900343134512,				   2000, 5.4631951378296688E+29,				 32000,			   14000000,					 20,	 17.857142857142858,	 142.85714285714286,					  1,	 0.8928571428571429 }, 
			{	   1016046.9088,	 69.621328384310658,				   2240, 6.1187785543692285E+29,				 35840,			   15680000,				   22.4,					 20,					160,	 1.1199999999999999,					  1 }, 
		};
		static readonly int[] metricPowerTable = new int[] { 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
		static readonly Dictionary<string, int> unitIndices = CreateUnitIndices();
		static Dictionary<string, int> CreateUnitIndices() {
			Dictionary<string, int> result = new Dictionary<string, int>();
			result.Add("g", 0);
			result.Add("sg", 1);
			result.Add("lbm", 2);
			result.Add("u", 3);
			result.Add("ozm", 4);
			result.Add("grain", 5);
			result.Add("cwt", 6);
			result.Add("shweight", 6);
			result.Add("uk_cwt", 7);
			result.Add("lcwt", 7);
			result.Add("hweight", 7);
			result.Add("stone", 8);
			result.Add("ton", 9);
			result.Add("uk_ton", 10);
			result.Add("LTON", 10);
			result.Add("brton", 10);
			return result;
		}
		#endregion
		#region Helper functions
		protected override bool CheckIndex(int index) {
			return (index >= 0) && (index < numberOfMultipliers);
		}
		#endregion
		protected override double[,]  Multipliers { get { return multipliers; } }
		public override Dictionary<string, int> UnitIndices { get { return unitIndices; } }
		protected internal override int[] MetricPowerTable { get { return metricPowerTable; } }
	}
	#endregion
	#region DistanceUnitCategory
	class DistanceUnitCategory : SimpleUnitCategory {
		#region Fields
		const int numberOfMultipliers = 13;
		static readonly double[,] multipliers = new double[numberOfMultipliers, numberOfMultipliers] {
			{					  1, 0.00062137119223733392, 0.00053995680345572358,	 39.370078740157481,	 3.2808398950131235,	1.0936132983377078,			 10000000000,	 2834.6456692913384,	 236.22047244094486, 0.00062136994949494955,	0.87489063867016625, 1.0570008340246154E-16, 3.2407792896647253E-17 }, 
			{			   1609.344,					  1,	0.86897624190064793,				  63360,				   5280,				  1760,		  16093440000000,				4561920,				 380160,			   0.999998,				   1408, 1.7010779502325107E-13, 5.2155287051461876E-14 }, 
			{				   1852,	 1.1507794480235425,					  1,	 72913.385826771657,	 6076.1154855643044,	2025.3718285214347,		  18520000000000,	 5249763.7795275589,	 437480.31496062991,	 1.1507771464646466,	 1620.2974628171478, 1.9575655446135877E-13, 6.0019232444590715E-14 }, 
			{				 0.0254, 1.5782828282828283E-05, 1.3714902807775378E-05,					  1,   0.083333333333333329,  0.027777777777777776,			   254000000,					 72,					  6, 1.5782796717171717E-05,   0.022222222222222223,  2.684782118422523E-18, 8.2315793957484025E-19 }, 
			{				 0.3048, 0.00018939393939393939, 0.00016457883369330455,					 12,					  1,	0.33333333333333331,			 3048000000,					864,					 72, 0.00018939356060606062,	0.26666666666666666, 3.2217385421070278E-17, 9.8778952748980834E-18 }, 
			{				 0.9144, 0.00056818181818181815, 0.00049373650107991361,					 36,					  3,					  1,			 9144000000,				   2592,					216, 0.00056818068181818183,					0.8, 9.6652156263210828E-17, 2.9633685824694249E-17 }, 
			{				  1E-10, 6.2137119223733393E-14, 5.3995680345572351E-14, 3.9370078740157481E-09, 3.2808398950131236E-10, 1.0936132983377078E-10,					  1, 2.8346456692913386E-07, 2.3622047244094487E-08, 6.2136994949494947E-14, 8.7489063867016618E-11, 1.0570008340246154E-26, 3.2407792896647256E-27 }, 
			{ 0.00035277777777777781, 2.1920594837261504E-07, 1.9048476121910247E-07,	0.01388888888888889,  0.0011574074074074076,  0.0003858024691358025,	  3527777.777777778,					  1,   0.083333333333333329, 2.1920550996071833E-07, 0.00030864197530864197, 3.7288640533646156E-20,  1.143274916076167E-20 }, 
			{  0.0042333333333333337, 2.6304713804713806E-06, 2.2858171346292298E-06,	0.16666666666666669,	0.01388888888888889,	0.00462962962962963,	 42333333.333333336,					 12,					  1,   2.63046611952862E-06,  0.0037037037037037038, 4.4746368640375387E-19, 1.3719298992914005E-19 }, 
			{	 1609.3472186944373,		 1.000002000004,	0.86897797985660763,	 63360.126720253436,	   5280.01056002112,	   1760.00352000704,	 16093472186944.373,	  4561929.123858247,	 380160.76032152062,					  1,	  1408.002816005632, 1.7010813523952153E-13,   5.21553913622446E-14 }, 
			{				  1.143, 0.00071022727272727275,   0.000617170626349892,					 45,				   3.75,				   1.25,			11430000000,				   3240,					270, 0.00071022585227272731,					  1, 1.2081519532901353E-16, 3.7042107280867811E-17 }, 
			{	9.4607304725808E+15,	 5878625373183.6084,	 5108385784330.8857, 3.7246970364491341E+17,	  31039141970409452,	  10346380656803150,	9.4607304725808E+25, 2.6817818662433763E+19, 2.2348182218694805E+18,	 5878613615932.8623,   8.27710452544252E+15,					  1,	0.30660139380639828 }, 
			{	  30856775812815532,	 19173511575409.316,	 16661326032837.762, 1.2148336934179343E+18,  1.012361411181612E+17,   3.37453803727204E+16, 3.0856775812815533E+26, 8.7468025926091276E+19,  7.289002160507606E+18,	 19173473228386.168,	  26996304298176320,	   3.26156377694566,					  1 }  
		};
		static readonly Dictionary<string, int> unitIndices = CreateUnitIndices();
		static readonly int[] metricPowerTable = new int[] { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1 };
		#endregion
		#region Helper functions
		static Dictionary<string, int> CreateUnitIndices() {
			Dictionary<string, int> result = new Dictionary<string, int>();
			result.Add("m", 0);
			result.Add("mi", 1);
			result.Add("Nmi", 2);
			result.Add("in", 3);
			result.Add("ft", 4);
			result.Add("yd", 5);
			result.Add("ang", 6);
			result.Add("Pica", 7);
			result.Add("Picapt", 7);
			result.Add("pica", 8);
			result.Add("survey_mi", 9);
			result.Add("ell", 10);
			result.Add("ly", 11);
			result.Add("parsec", 12);
			result.Add("pc", 12);
			return result;
		}
		#endregion
		protected override bool CheckIndex(int index) {
			return (index >= 0) && (index < numberOfMultipliers);
		}
		protected override double[,]  Multipliers { get { return multipliers; } }
		public override Dictionary<string,int>  UnitIndices { get { return unitIndices; } }
		protected internal override int[]  MetricPowerTable { get { return metricPowerTable; } }
	}
	#endregion
	#region TimeUnitCategory
	class TimeUnitCategory : SimpleUnitCategory {
		#region Fields
		const int numberOfMultipliers = 5;
		static readonly double[,] multipliers = new double[numberOfMultipliers, numberOfMultipliers] {
			{					  1,				 365.25,				   8766,			   525960, 31557600 }, 
			{  0.0027378507871321013,					  1,					 24,				 1440,	86400 }, 
			{ 0.00011407711613050422,   0.041666666666666664,					  1,				   60,	 3600 }, 
			{ 1.9012852688417369E-06, 0.00069444444444444447,   0.016666666666666666,					1,	   60 }, 
			{ 3.1688087814028947E-08, 1.1574074074074074E-05, 0.00027777777777777778, 0.016666666666666666,		1 }  
		};
		static readonly Dictionary<string, int> unitIndices = CreateUnitIndices();
		static readonly int[] metricPowerTable = new int[] { 0, 0, 0, 0, 1 };
		#endregion
		protected override bool CheckIndex(int index) {
			return (index >= 0) && (index < numberOfMultipliers);
		}
		#region Helper functions
		static Dictionary<string, int> CreateUnitIndices() {
			Dictionary<string, int> result = new Dictionary<string, int>();
			result.Add("yr", 0);
			result.Add("day", 1);
			result.Add("d", 1);
			result.Add("hr", 2);
			result.Add("mn", 3);
			result.Add("min", 3);
			result.Add("sec", 4);
			result.Add("s", 4);
			return result;
		}
		#endregion
		protected override double[,] Multipliers { get { return multipliers; } }
		public override Dictionary<string, int> UnitIndices { get { return unitIndices; } }
		protected internal override int[] MetricPowerTable { get { return metricPowerTable; } }
	}
	#endregion
	#region PressureUnitCategory
	class PressureUnitCategory : SimpleUnitCategory {
		#region Fields
		const int numberOfMultipliers = 5;
		static readonly double[,] multipliers = new double[numberOfMultipliers, numberOfMultipliers] {
			{				  1, 9.8692326671601285E-06, 0.0075006375541921064, 0.00014503773773020921, 0.0075006168270416972 }, 
			{			 101325,					  1,	760.00210017851521,	 14.695948775513449,				   760 }, 
			{			133.322,  0.0013157858376511226,					 1,   0.019336721269666953,   0.99999723661485318 }, 
			{ 6894.7572931683617,   0.068045963909877744,	51.715075480178527,					  1,	51.714932571507077 }, 
			{ 133.32236842105263,  0.0013157894736842105,	1.0000027633927831,   0.019336774704622958,					 1 }  
		};
		static readonly Dictionary<string, int> unitIndices = CreateUnitIndices();
		static readonly int[] metricPowerTable = new int[] { 1, 1, 1, 0, 0 };
		#endregion
		protected override bool CheckIndex(int index) {
			return (index >= 0) && (index < numberOfMultipliers);
		}
		#region Helper functions
		static Dictionary<string, int> CreateUnitIndices() {
			Dictionary<string, int> result = new Dictionary<string, int>();
			result.Add("p", 0);
			result.Add("Pa", 0);
			result.Add("atm", 1);
			result.Add("at", 1);
			result.Add("mmHg", 2);
			result.Add("psi", 3);
			result.Add("Torr", 4);
			return result;
		}
		#endregion
		protected override double[,] Multipliers { get { return multipliers; } }
		public override Dictionary<string, int> UnitIndices { get { return unitIndices; } }
		protected internal override int[] MetricPowerTable { get { return metricPowerTable; } }
	}
	#endregion
	#region ForceUnitCategory
	class ForceUnitCategory : SimpleUnitCategory {
		#region Fields
		const int numberOfMultipliers = 4;
		static readonly double[,] multipliers = new double[numberOfMultipliers, numberOfMultipliers] {
			{					  1,				 100000,	0.22480894309971047,	 101.97162129779282 }, 
			{				  1E-05,					  1, 2.2480894309971049E-06,  0.0010197162129779282 }, 
			{	 4.4482216152605005,		444822.16152605,					  1,			  453.59237 }, 
			{			 0.00980665,				980.665,  0.0022046226218487759,					  1 }, 
		};
		static readonly int[] metricPowerTable = new int[] { 1, 1, 0, 1 };
		static readonly Dictionary<string, int> unitIndices = CreateUnitIndices();
		#endregion
		#region Helper functions
		static Dictionary<string, int> CreateUnitIndices() {
			Dictionary<string, int> result = new Dictionary<string, int>();
			result.Add("N", 0);
			result.Add("dy", 1);
			result.Add("dyn", 1);
			result.Add("lbf", 2);
			result.Add("pond", 3);
			return result;
		}
		#endregion
		protected override double[,] Multipliers { get { return multipliers; } }
		public override Dictionary<string, int> UnitIndices { get { return unitIndices; } }
		protected internal override int[] MetricPowerTable { get { return metricPowerTable; } }
		protected override bool CheckIndex(int index) {
			return (index >= 0) && (index < numberOfMultipliers);
		}
	}
	#endregion
	#region EnergyUnitCategory
	class EnergyUnitCategory : SimpleUnitCategory {
		#region Fields
		const int numberOfMultipliers = 9;
		static readonly double[,] multipliers = new double[numberOfMultipliers, numberOfMultipliers] {
			{				  1,		   10000000,	0.23900573613766729,	0.23884589662749595, 6.2415096471204178E+18, 3.7250613599861881E-07, 0.00027777777777777778,	0.73756214927726538, 0.00094781712031331714 }, 
			{			  1E-07,				  1,  2.390057361376673E-08, 2.3884589662749594E-08,	 624150964712.04175, 3.7250613599861882E-14, 2.7777777777777777E-11, 7.3756214927726531E-08, 9.4781712031331715E-11 }, 
			{			  4.184,		   41840000,					  1,	  0.999331231489443, 2.6114476363551826E+19, 1.5585656730182213E-06,  0.0011622222222222223,	 3.0859600325760783,  0.0039656668313909193 }, 
			{			 4.1868,		   41868000,	 1.0006692160611854,					  1, 2.6131952590563766E+19, 1.5596086901990174E-06,			   0.001163,	 3.0880252065940548,  0.0039683207193277961 }, 
			{	1.602176487E-19,	1.602176487E-12, 3.8292937069789676E-20, 3.8267327959300658E-20,					  1, 5.9682057236021138E-26, 4.4504902416666668E-23, 1.1817047332732185E-19, 1.5185703041420467E-22 }, 
			{ 2684519.5376961729,  26845195376961.73,	 641615.56828302413,	 641186.47599507333, 1.6755454592413908E+25,					  1,	 745.69987158227025,	 1980000.0000000002,	 2544.4335776440244 }, 
			{			   3600,		36000000000,	 860.42065009560224,	 859.84522785898537, 2.2469434729633503E+22,  0.0013410220895950279,					  1,	 2655.2237373981552,	 3.4121416331279417 }, 
			{ 1.3558179483314004, 13558179.483314004,	0.32404826680960813,	 0.3238315535328653, 8.4623508042494474E+18,   5.05050505050505E-07, 0.00037661609675872232,					  1,  0.0012850674634565778 }, 
			{	  1055.05585262,	  10550558526.2,	 252.16440072179734,	 251.99576111111114, 6.5851412823785876E+21, 0.00039301477892220448,	0.29307107017222223,	   778.169262265965,					  1 }  
		};
		static readonly int[] metricPowerTable = new int[] { 1, 1, 1, 1, 1, 0, 1, 0, 0 };
		static readonly Dictionary<string, int> unitIndices = CreateUnitIndices();
		#endregion
		#region Helper functions
		static Dictionary<string, int> CreateUnitIndices() {
			Dictionary<string, int> result = new Dictionary<string, int>();
			result.Add("J", 0);
			result.Add("e", 1);
			result.Add("c", 2);
			result.Add("cal", 3);
			result.Add("ev", 4);
			result.Add("eV", 4);
			result.Add("HPh", 5);
			result.Add("hh", 5);
			result.Add("wh", 6);
			result.Add("Wh", 6);
			result.Add("flb", 7);
			result.Add("btu", 8);
			result.Add("BTU", 8);
			return result;
		}
		#endregion
		protected override double[,] Multipliers { get { return multipliers; } }
		public override Dictionary<string, int> UnitIndices { get { return unitIndices; } }
		protected internal override int[] MetricPowerTable { get { return metricPowerTable; } }
		protected override bool CheckIndex(int index) {
			return (index >= 0) && (index < numberOfMultipliers);
		}
	}
	#endregion
	#region PowerUnitCategory
	class PowerUnitCategory : SimpleUnitCategory {
		#region Fields
		const int numberOfMultipliers = 3;
		static readonly double[,] multipliers = new double[numberOfMultipliers, numberOfMultipliers] {
			{					 1, 745.69987158227025,		1.013869665424 }, 
			{ 0.0013410220895950279,				  1, 0.0013596216173039043 }, 
			{	 0.986320070619531,		  735.49875,					 1 }  
		};
		static readonly int[] metricPowerTable = new int[] { 0, 1, 0 };
		static readonly Dictionary<string, int> unitIndices = CreateUnitIndices();
		#endregion
		protected override double[,] Multipliers { get { return multipliers; } }
		public override Dictionary<string, int> UnitIndices { get { return unitIndices; } }
		protected internal override int[] MetricPowerTable { get { return metricPowerTable; } }
		protected override bool CheckIndex(int index) {
			return (index >= 0) && (index < numberOfMultipliers);
		}
		#region Helper functions
		static Dictionary<string, int> CreateUnitIndices() {
			Dictionary<string, int> result = new Dictionary<string, int>();
			result.Add("HP", 0);
			result.Add("h", 0);
			result.Add("W", 1);
			result.Add("w", 1);
			result.Add("PS", 2);
			return result;
		}
		#endregion
	}
	#endregion
	#region MagnetismUnitCategory
	class MagnetismUnitCategory : SimpleUnitCategory {
		#region Fields
		const int numberOfMultipliers = 2;
		static readonly double[,] multipliers = new double[numberOfMultipliers, numberOfMultipliers] {
			{	  1, 10000 }, 
			{ 0.0001,	 1 } 
		};
		static readonly int[] metricPowerTable = new int[] { 1, 1 };
		static readonly Dictionary<string, int> unitIndices = CreateUnitIndices();
		#endregion
		protected override double[,] Multipliers { get { return multipliers; } }
		public override Dictionary<string, int> UnitIndices { get { return unitIndices; } }
		protected internal override int[] MetricPowerTable { get { return metricPowerTable; } }
		protected override bool CheckIndex(int index) {
			return (index >= 0) && (index < numberOfMultipliers);
		}
		#region Helper functions
		static Dictionary<string, int> CreateUnitIndices() {
			Dictionary<string, int> result = new Dictionary<string, int>();
			result.Add("T", 0);
			result.Add("ga", 1);
			return result;
		}
		#endregion
	}
	#endregion
	#region AreaUnitCategory
	class AreaUnitCategory : SimpleUnitCategory {
		#region Fields
		const int numberOfMultipliers = 14;
		static readonly double[,] multipliers = new double[numberOfMultipliers, numberOfMultipliers] {
			{					  1,		   0.999996000004,	  4.0468564224E+23,		   40.468564224,				  43560,	0.40468564224000003,				6272640, 4.5213534261988142E-29,		   4046.8564224,	 1.6187425689600001,			  0.0015625,   0.001179874545293396,	 32517365760.000004,				   4840 }, 
			{		 1.000004000012,						1, 4.046872609874252E+23,	 40.468726098742522,	  43560.17424052272,	0.40468726098742519,	 6272665.0906352717, 4.5213715116667754E-29,	 4046.8726098742522,	 1.6187490439497008,	0.00156250625001875,  0.0011798792648057357,	 32517495829.853252,	 4840.0193600580806 }, 
			{ 2.4710538146716534E-24,   2.4710439304662788E-24,					 1,				  1E-22, 1.0763910416709723E-19, 1.0000000000000001E-24,	1.5500031000062E-17,  1.117250763128733E-52,				  1E-20, 4.0000000000000004E-24, 3.8610215854244584E-27, 2.9155334959812285E-27, 8.0352160704321417E-14, 1.1959900463010802E-20 }, 
			{   0.024710538146716535,	 0.024710439304662789,				 1E+22,					  1,	 1076.3910416709723,				   0.01,		155000.31000062, 1.1172507631287329E-30,					100,				   0.04, 3.8610215854244585E-05, 2.9155334959812286E-05,	  803521607.0432142,	 119.59900463010803 }, 
			{  2.295684113865932E-05,   2.2956749311386592E-05,		  9.290304E+18,		   0.0009290304,					  1,		   9.290304E-06,					144, 1.0379599233697922E-33,			 0.09290304,		  3.7161216E-05, 3.5870064279155189E-08, 2.7086192499848393E-08,	 746496.00000000012,	0.11111111111111111 }, 
			{	 2.4710538146716532,		2.471043930466279,				 1E+24,					100,	 107639.10416709722,					  1,		15500031.000062, 1.1172507631287329E-28,				  10000,					  4,  0.0038610215854244585,  0.0029155334959812284,	 80352160704.321411,	 11959.900463010803 }, 
			{ 1.5942250790735638E-07,   1.5942187021796245E-07,			6.4516E+16,			 6.4516E-06,  0.0069444444444444441, 6.4516000000000007E-08,					  1, 7.2080550234013338E-36,			 0.00064516, 2.5806400000000003E-07, 2.4909766860524435E-10, 1.8809855902672494E-10,	 5184.0000000000009,  0.0007716049382716049 }, 
			{ 2.2117271218072384E+28,	2.211718274907598E+28,  8.95054210748189E+51,   8.95054210748189E+29, 9.6342833425923309E+32,   8.95054210748189E+27, 1.3873368013332956E+35,					  1,   8.95054210748189E+31, 3.5802168429927559E+28,   3.45582362782381E+25,  2.609560532155387E+25, 7.1919539781118055E+38, 1.0704759269547035E+32 }, 
			{ 0.00024710538146716532,	0.0002471043930466279,				 1E+20,				   0.01,	 10.763910416709722,				 0.0001,		1550.0031000062,  1.117250763128733E-32,					  1,				 0.0004, 3.8610215854244582E-07, 2.9155334959812285E-07,	 8035216.0704321414,	 1.1959900463010802 }, 
			{	 0.6177634536679133,	  0.61776098261656975,			   2.5E+23,					 25,	 26909.776041774305,				   0.25,		3875007.7500155, 2.7931269078218323E-29,				   2500,					  1, 0.00096525539635611464,  0.0007288833739953071,	 20088040176.080353,	 2989.9751157527007 }, 
			{					640,		  639.99744000256,	2.589988110336E+26,		 25899.88110336,			   27878400,		 258.9988110336,			 4014489600, 2.8936661927672414E-26,		 2589988.110336,		1035.9952441344,					  1,	0.75511970898777336,	 20811114086400.004,				3097600 }, 
			{	 847.54773631575631,	   847.54434612820125,		  3.429904E+26,			   34299.04,	 36919179.393914342,			   342.9904,	 5316361832.7236652, 3.8320628614582939E-26,				3429904,			  1371.9616,	 1.3242933379933692,					  1,	 27560019740839.484,	 4102131.0437682606 }, 
			{ 3.0752798593240045E-11,   3.0752675582168678E-11,	12445216049382.715, 1.2445216049382714E-09, 1.3395919067215363E-06, 1.2445216049382716E-11, 0.00019290123456790122, 1.3904427128474794E-39, 1.2445216049382715E-07, 4.9780864197530862E-11, 4.8051247801937566E-14, 3.6284444256698481E-14,					  1, 1.4884354519128179E-07 }, 
			{ 0.00020661157024793388,   0.00020661074380247933,		 8.3612736E+19,		   0.0083612736,					  9,		  8.3612736E-05,				   1296, 9.3416393103281281E-33,			 0.83612736,		 0.000334450944, 3.2283057851239667E-07, 2.4377573249863554E-07,	 6718464.0000000009,					  1 }  
		};
		static readonly int[] metricPowerTable = new int[] { 0, 0, 2, 1, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0 };
		static readonly Dictionary<string, int> unitIndices = CreateUnitIndices();
		#endregion
		protected override double[,] Multipliers { get { return multipliers; } }
		public override Dictionary<string, int> UnitIndices { get { return unitIndices; } }
		protected internal override int[] MetricPowerTable { get { return metricPowerTable; } }
		protected override bool CheckIndex(int index) {
			return (index >= 0) && (index < numberOfMultipliers);
		}
		public override int GetMetricPower(int unitIndex) {
			switch(unitIndex) {
				case 2: 
				case 4: 
				case 6: 
				case 7: 
				case 8: 
				case 10: 
				case 11: 
				case 12: 
				case 13: 
					return 2;
			}
			return 1;
		}
		#region Helper functions
		static Dictionary<string, int> CreateUnitIndices() {
			Dictionary<string, int> result = new Dictionary<string, int>();
			result.Add("uk_acre", 0);
			result.Add("us_acre", 1);
			result.Add("ang2", 2);
			result.Add("ang^2", 2);
			result.Add("ar", 3);
			result.Add("ft2", 4);
			result.Add("ft^2", 4);
			result.Add("ha", 5);
			result.Add("in2", 6);
			result.Add("in^2", 6);
			result.Add("ly2", 7);
			result.Add("ly^2", 7);
			result.Add("m2", 8);
			result.Add("m^2", 8);
			result.Add("Morgen", 9);
			result.Add("mi2", 10);
			result.Add("mi^2", 10);
			result.Add("Nmi2", 11);
			result.Add("Nmi^2", 11);
			result.Add("Pica2", 12);
			result.Add("Pica^2", 12);
			result.Add("Picapt2", 12);
			result.Add("Picapt^2", 12);
			result.Add("yd2", 13);
			result.Add("yd^2", 13);
			return result;
		}
		#endregion
	}
	#endregion
	#region VolumeUnitCategory
	class VolumeUnitCategory : SimpleUnitCategory {
		#region Fields
		const int numberOfMultipliers = 25;
		static readonly double[,] multipliers = new double[numberOfMultipliers, numberOfMultipliers] {
			{					  1,	0.98578431874999994,	0.33333333333333331,	0.16666666666666666,   0.020833333333333332,   0.010416666666666666,  0.0086736894232186338,   0.005208333333333333,  0.0043368447116093169,  0.0013020833333333333,  0.0010842111779023292,	   0.00492892159375, 4.9289215937499993E+24, 3.1001984126984125E-05, 0.00013987093218999081, 0.00017406322337962962,			 0.30078125, 5.8207359653452429E-54,	  4.92892159375E-06, 1.1825111763758073E-15, 6.4467860510973937E-06, 7.7594146898721952E-16,	 112265.99999999999, 1.7406322337962961E-06,   4.35158058449074E-06 }, 
			{	   1.01442068105529,					  1,	   0.33814022701843,	  0.169070113509215,   0.021133764188651875,   0.010566882094325937,  0.0087987699319635113,  0.0052834410471629687,  0.0043993849659817556,  0.0013208602617907422,  0.0010998462414954389,				  0.005, 4.9999999999999994E+24, 3.1449053852160527E-05, 0.00014188796629200875, 0.00017657333360744296,	0.30511872047366145, 5.9046749422085426E-54,				  5E-06, 1.1995637928946385E-15, 6.5397530965719613E-06,  7.871310734290574E-16,	 113884.95217935316, 1.7657333360744295E-06, 4.4143333401860733E-06 }, 
			{					  3,		  2.95735295625,					  1,					0.5,				 0.0625,				0.03125,	 0.0260210682696559,			   0.015625,	0.01301053413482795,			 0.00390625,  0.0032526335337069875,	   0.01478676478125,	 1.478676478125E+25, 9.3005952380952376E-05,  0.0004196127965699724, 0.00052218967013888888,			 0.90234375,  1.746220789603573E-53, 1.4786764781249999E-05, 3.5475335291274221E-15, 1.9340358153292181E-05, 2.3278244069616589E-15,	 336797.99999999994, 5.2218967013888886E-06, 1.3054741753472222E-05 }, 
			{					  6,		   5.9147059125,					  2,					  1,				  0.125,				 0.0625,	 0.0520421365393118,				0.03125,	 0.0260210682696559,			  0.0078125,  0.0065052670674139749,		0.0295735295625,	  2.95735295625E+25, 0.00018601190476190475,  0.0008392255931399448,  0.0010443793402777778,			  1.8046875,  3.492441579207146E-53, 2.9573529562499998E-05, 7.0950670582548441E-15, 3.8680716306584362E-05, 4.6556488139233177E-15,	 673595.99999999988, 1.0443793402777777E-05, 2.6109483506944443E-05 }, 
			{					 48,			 47.3176473,					 16,					  8,					  1,					0.5,	0.41633709231449439,				   0.25,	 0.2081685461572472,				 0.0625,	 0.0520421365393118,		   0.2365882365,		2.365882365E+26,   0.001488095238095238,  0.0067138047451195584,   0.008355034722222222,				14.4375, 2.7939532633657168E-52, 0.00023658823649999998, 5.6760536466038753E-14,  0.0003094457304526749, 3.7245190511386542E-14,	 5388767.9999999991, 8.3550347222222218E-05, 0.00020887586805555555 }, 
			{					 96,			 94.6352946,					 32,					 16,					  2,					  1,	0.83267418462898879,					0.5,	0.41633709231449439,				  0.125,	 0.1040842730786236,			0.473176473,		 4.73176473E+26,   0.002976190476190476,   0.013427609490239117,   0.016710069444444444,				 28.875, 5.5879065267314336E-52, 0.00047317647299999996, 1.1352107293207751E-13,  0.0006188914609053498, 7.4490381022773083E-14,	 10777535.999999998, 0.00016710069444444444, 0.00041775173611111109 }, 
			{	 115.29119284846608,	 113.65225000000001,	 38.430397616155361,	  19.21519880807768,	   2.40189985100971,	  1.200949925504855,					  1,	0.60047496275242751,					0.5,	0.15011874068810688,				  0.125,			 0.56826125,		  5.6826125E+26,   0.003574255730669211,   0.016125886617010952,   0.020067956654486508,	 34.677429098952686, 6.7107959270062085E-52,		  0.00056826125, 1.3633312408100971E-13, 0.00074325765386987067,   8.94592175401276E-14,	 12943281.056325892, 0.00020067956654486508, 0.00050169891636216277 }, 
			{					192,			189.2705892,					 64,					 32,					  4,					  2,	 1.6653483692579776,					  1,	0.83267418462898879,				   0.25,	 0.2081685461572472,			0.946352946,		 9.46352946E+26,  0.0059523809523809521,   0.026855218980478233,   0.033420138888888888,				  57.75, 1.1175813053462867E-51, 0.00094635294599999993,   2.27042145864155E-13,  0.0012377829218106996, 1.4898076204554617E-13,	 21555071.999999996, 0.00033420138888888887, 0.00083550347222222218 }, 
			{	 230.58238569693216,	 227.30450000000002,	 76.860795232310721,	 38.430397616155361,	   4.80379970201942,	   2.40189985100971,					  2,	  1.200949925504855,					  1,	0.30023748137621376,				   0.25,			  1.1365225,		  1.1365225E+27,  0.0071485114613384221,   0.032251773234021903,   0.040135913308973016,	 69.354858197905372, 1.3421591854012417E-51,		   0.0011365225, 2.7266624816201942E-13,  0.0014865153077397413, 1.7891843508025519E-13,	 25886562.112651784, 0.00040135913308973016,  0.0010033978327243255 }, 
			{					768,			757.0823568,					256,					128,					 16,					  8,	   6.66139347703191,					  4,	 3.3306967385159552,					  1,	0.83267418462898879,			3.785411784,		3.785411784E+27,   0.023809523809523808,	0.10742087592191293,	0.13368055555555555,					231, 4.4703252213851469E-51,  0.0037854117839999997,	9.0816858345662E-13,  0.0049511316872427984, 5.9592304818218467E-13,	 86220287.999999985,  0.0013368055555555555,  0.0033420138888888887 }, 
			{	 922.32954278772866,	 909.21800000000007,	 307.44318092924289,	 153.72159046462144,	  19.21519880807768,	   9.60759940403884,					  8,	   4.80379970201942,					  4,	  1.200949925504855,					  1,				4.54609,			4.54609E+27,   0.028594045845353688,	0.12900709293608761,	0.16054365323589206,	 277.41943279162149, 5.3686367416049668E-51,			 0.00454609, 1.0906649926480777E-12,  0.0059460612309589654, 7.1567374032102078E-13,	 103546248.45060714,  0.0016054365323589206,  0.0040135913308973021 }, 
			{	 202.88413621105798,					200,		67.628045403686,		33.814022701843,	 4.2267528377303751,	 2.1133764188651876,	 1.7597539863927021,	 1.0566882094325938,	  0.879876993196351,	0.26417205235814845,	0.21996924829908776,					  1,				  1E+27,  0.0062898107704321051,   0.028377593258401747,   0.035314666721488586,	 61.023744094732287, 1.1809349884417084E-51,				  0.001, 2.3991275857892773E-13,  0.0013079506193143921, 1.5742621468581149E-13,	 22776990.435870633, 0.00035314666721488586, 0.00088286666803721468 }, 
			{   2.02884136211058E-25,				  2E-25,	6.7628045403686E-26,	3.3814022701843E-26, 4.2267528377303749E-27, 2.1133764188651874E-27, 1.7597539863927021E-27, 1.0566882094325937E-27,   8.79876993196351E-28, 2.6417205235814843E-28, 2.1996924829908777E-28,				  1E-27,					  1, 6.2898107704321052E-30, 2.8377593258401747E-29, 3.5314666721488591E-29, 6.1023744094732289E-26, 1.1809349884417085E-78,				  1E-30, 2.3991275857892772E-40, 1.3079506193143922E-30, 1.5742621468581149E-40, 2.2776990435870634E-20, 3.5314666721488588E-31, 8.8286666803721467E-31 }, 
			{	 32256.000000000004,	 31797.458985600002,	 10752.000000000002,	 5376.0000000000009,	 672.00000000000011,	 336.00000000000006,	 279.77852603534024,	 168.00000000000003,	 139.88926301767012,	 42.000000000000007,	 34.972315754417529,		  158.987294928,	  1.58987294928E+29,					  1,	 4.5116767887203437,	 5.6145833333333339,				   9702, 1.8775365929817618E-49,		 0.158987294928, 3.8143080505178049E-11,	0.20794753086419754, 2.5028768023651756E-11,	 3621252095.9999995,   0.056145833333333332,	0.14036458333333335 }, 
			{	 7149.4483116883112,	 7047.8140333759993,	 2383.1494372294374,	 1191.5747186147187,	 148.94683982683984,	 74.473419913419917,	 62.012094202939217,	 37.236709956709959,	 31.006047101469608,	   9.30917748917749,	 7.7515117753674021,		 35.23907016688, 3.5239070166879998E+28,	0.22164708307565448,					  1,	 1.2444560185185185,				2150.42, 4.1615050920220985E-50,	   0.03523907016688, 8.4543025334925748E-12,   0.046090963648834013, 5.5475534254196254E-12,	 802639964.15999985,   0.012444560185185184,	0.03111140046296296 }, 
			{	 5745.0389610389611,		   5663.3693184,	 1915.0129870129872,	  957.5064935064936,	  119.6883116883117,	  59.84415584415585,	   49.8306836723426,	 29.922077922077925,	   24.9153418361713,	 7.4805194805194812,	 6.2288354590428252,		   28.316846592,	   2.8316846592E+28,	0.17810760667903525,	0.80356395494833577,					  1,				   1728, 3.3440354902829152E-50,		 0.028316846592, 6.7935727801430287E-12,   0.037037037037037035, 4.4578139708173813E-12,	 644972543.99999988,				   0.01,   0.024999999999999998 }, 
			{	 3.3246753246753249,			  3.2774128,	 1.1082251082251082,	0.55411255411255411,   0.069264069264069264,   0.034632034632034632,   0.028837201199272338,   0.017316017316017316,   0.014418600599636169,   0.004329004329004329,  0.0036046501499090423,			0.016387064,		  1.6387064E+25, 0.00010307153164296021,   0.000465025436891398, 0.00057870370370370367,					  1, 1.9352057235433537E-53,		  1.6387064E-05, 3.9314657292494374E-15,  2.143347050754458E-05, 2.5797534553341326E-15,	 373247.99999999994, 5.7870370370370367E-06, 1.4467592592592591E-05 }, 
			{ 1.7179958100722532E+53, 1.6935733292474305E+53, 5.7266527002408444E+52, 2.8633263501204222E+52, 3.5791579376505278E+51, 1.7895789688252639E+51, 1.4901362086957631E+51,   8.94789484412632E+50, 7.4506810434788153E+50,   2.23697371103158E+50, 1.8626702608697038E+50, 8.4678666462371522E+50, 8.4678666462371516E+77, 5.3261278834085228E+48, 2.4029767545330446E+49, 2.9903988845387433E+49, 5.1674092724829485E+52,					  1, 8.4678666462371517E+47, 2.0315492463772484E+38, 1.1075551424217569E+48, 1.3330641925813524E+38, 1.9287251761357154E+58, 2.9903988845387432E+47, 7.4759972113468579E+47 }, 
			{	 202884.13621105798,				 200000,		67628.045403686,		33814.022701843,	 4226.7528377303752,	 2113.3764188651876,	 1759.7539863927023,	 1056.6882094325938,	 879.87699319635112,	 264.17205235814845,	 219.96924829908778,				   1000,				  1E+30,	 6.2898107704321049,	 28.377593258401749,	 35.314666721488592,	 61023.744094732283, 1.1809349884417084E-48,					  1, 2.3991275857892774E-10,	 1.3079506193143922, 1.5742621468581148E-10,	 22776990435.870632,	0.35314666721488586,	 0.8828666680372147 }, 
			{	 845657969225142.88,	 833636365088115.88,	 281885989741714.31,	 140942994870857.16,	 17617874358857.145,	 8808937179428.5723,	   7334974583328.67,	 4404468589714.2861,	  3667487291664.335,	 1101117147428.5715,	 916871822916.08374,	 4168181825440.5796, 4.1681818254405794E+39,	 26217074938.775509,	 118282968469.41528,		   147197952000,		254358061056000,   4.92235175584961E-39,	 4168181825.4405794,					  1,			 5451776000,	0.65618108690130628, 9.4938637573029872E+19,			 1471979520,			 3679948800 }, 
			{	 155116.05194805196,		 152910.9715968,	  51705.35064935065,	 25852.675324675325,	 3231.5844155844156,	 1615.7922077922078,	 1345.4284591532503,	 807.89610389610391,	 672.71422957662514,	 201.97402597402598,	 168.17855739415629,		  764.554857984,	  7.64554857984E+29,	 4.8089053803339512,	 21.696226783605066,					 27,				  46656, 9.0288958237638713E-49,		 0.764554857984, 1.8342646506386176E-10,					  1, 1.2036097721206928E-10,			17414258688,				   0.27,	0.67499999999999993 }, 
			{	   1288757000325331,	   1.2704364416E+15,	 429585666775110.38,	 214792833387555.19,	   26849104173444.4,	   13424552086722.2,	 11178277962820.797,		6712276043361.1,	 5589138981410.3984,	 1678069010840.2749,	 1397284745352.5996,		  6352182208000,		6.352182208E+39,	 39954024067.625587,	 180259643001.88034,	 224325197629.68951,	  387633941504103.5, 7.5015142223841059E-39,			 6352182208,	  1.523969556517264,	 8308340652.9514637,					  1, 1.4468359339852359E+20,	  2243251976.296895,	 5608129940.7422371 }, 
			{ 8.9074163148237244E-06,   8.78079132373114E-06, 2.9691387716079077E-06, 1.4845693858039539E-06, 1.8557117322549423E-07, 9.2785586612747116E-08, 7.7260162678091627E-08, 4.6392793306373558E-08, 3.8630081339045814E-08,  1.159819832659339E-08, 9.6575203347614534E-09,   4.39039566186557E-08, 4.3903956618655695E+19, 2.7614757920460452E-10, 1.2458886233587267E-09,  1.550453595742519E-09, 2.6791838134430729E-06, 5.1847718501997442E-59, 4.3903956618655696E-11,  1.053311934491126E-20, 5.7424207249722931E-11, 6.9116337002050465E-21,					  1, 1.5504535957425191E-11, 3.8761339893562976E-11 }, 
			{	  574503.8961038962,		   566336.93184,	 191501.29870129871,	 95750.649350649357,	  11968.83116883117,	 5984.4155844155848,	   4983.06836723426,	 2992.2077922077924,	   2491.53418361713,	  748.0519480519481,	 622.88354590428253,		   2831.6846592,	   2.8316846592E+30,	 17.810760667903526,	 80.356395494833578,					100,				 172800, 3.3440354902829155E-48,		   2.8316846592,  6.793572780143029E-10,	 3.7037037037037037, 4.4578139708173815E-10,	 64497254399.999992,					  1,					2.5 }, 
			{	 229801.55844155847,		  226534.772736,	 76600.519480519491,	 38300.259740259746,	 4787.5324675324682,	 2393.7662337662341,	 1993.2273468937042,	 1196.8831168831171,	 996.61367344685209,	 299.22077922077926,	 249.15341836171302,		  1132.67386368,	  1.13267386368E+30,	   7.12430426716141,	 32.142558197933432,					 40,				  69120, 1.3376141961131661E-48,		  1.13267386368, 2.7174291120572113E-10,	 1.4814814814814816, 1.7831255883269526E-10,	 25798901759.999996,					0.4,					  1 }  
		};
		static readonly int[] metricPowerTable = new int[] { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0 };
		static readonly Dictionary<string, int> unitIndices = CreateUnitIndices();
		#endregion
		protected override double[,] Multipliers { get { return multipliers; } }
		public override Dictionary<string, int> UnitIndices { get { return unitIndices; } }
		protected internal override int[] MetricPowerTable { get { return metricPowerTable; } }
		protected override bool CheckIndex(int index) {
			return (index >= 0) && (index < numberOfMultipliers);
		}
		#region Helper functions
		static Dictionary<string, int> CreateUnitIndices() {
			Dictionary<string, int> result = new Dictionary<string, int>();
			result.Add("tsp", 0);
			result.Add("tspm", 1);
			result.Add("tbs", 2);
			result.Add("oz", 3);
			result.Add("cup", 4);
			result.Add("pt", 5);
			result.Add("us_pt", 5);
			result.Add("uk_pt", 6);
			result.Add("qt", 7);
			result.Add("uk_qt", 8);
			result.Add("gal", 9);
			result.Add("uk_gal", 10);
			result.Add("l", 11);
			result.Add("lt", 11);
			result.Add("ang3", 12);
			result.Add("ang^3", 12);
			result.Add("barrel", 13);
			result.Add("bushel", 14);
			result.Add("ft3", 15);
			result.Add("ft^3", 15);
			result.Add("in3", 16);
			result.Add("in^3", 16);
			result.Add("ly3", 17);
			result.Add("ly^3", 17);
			result.Add("m3", 18);
			result.Add("m^3", 18);
			result.Add("mi3", 19);
			result.Add("mi^3", 19);
			result.Add("yd3", 20);
			result.Add("yd^3", 20);
			result.Add("Nmi3", 21);
			result.Add("Nmi^3", 21);
			result.Add("Pica3", 22);
			result.Add("Pica^3", 22);
			result.Add("Picapt3", 22);
			result.Add("Picapt^3", 22);
			result.Add("GRT", 23);
			result.Add("regton", 23);
			result.Add("MTON", 24);
			return result;
		}
		#endregion
	}
	#endregion
	#region InformationUnitCategory
	class InformationUnitCategory : SimpleUnitCategory {
		#region Fields
		const int numberOfMultipliers = 2;
		static readonly double[,] multipliers = new double[numberOfMultipliers, numberOfMultipliers] {
			{ 1, 0.125 }, 
			{ 8,	 1 }  
		};
		static readonly Dictionary<string, int> unitIndices = CreateUnitIndices();
		static readonly int[] metricTable = new int[] { 1, 1 };
		#endregion
		protected override double[,] Multipliers { get { return multipliers; } }
		public override Dictionary<string, int> UnitIndices { get { return unitIndices; } }
		protected internal override int[] MetricPowerTable { get { return metricTable; } }
		protected override bool CheckIndex(int index) {
			return (index >= 0) && (index < numberOfMultipliers);
		}
		#region Helper functions
		static Dictionary<string, int> CreateUnitIndices() {
			Dictionary<string, int> result = new Dictionary<string, int>();
			result.Add("bit", 0);
			result.Add("byte", 1);
			return result;
		}
		#endregion
	}
	#endregion
	#region SpeedUnitCategory
	class SpeedUnitCategory : SimpleUnitCategory {
		#region Fields
		static readonly int[] metricPowerTable = new int[] { 0, 0, 1, 1, 1 };
		const int numberOfMultipliers = 5;
		static readonly double[,] multipliers = new double[numberOfMultipliers, numberOfMultipliers] {
			{					  1,	0.99936109959939234, 1852.0000000000002,	0.51444444444444448,   1.1507794480235427 }, 
			{	 1.0006393088552914,					  1,		   1853.184,	 0.5147733333333333,   1.1515151515151514 }, 
			{ 0.00053995680345572347, 0.00053961182483768476,				  1, 0.00027777777777777778, 0.000621371192237334 }, 
			{	 1.9438444924406046,	 1.9426025694156652,			   3600,					  1,   2.2369362920544025 }, 
			{	0.86897624190064782,	  0.868421052631579,		   1609.344,				0.44704,					1 }  
		};
		static readonly Dictionary<string, int> unitIndices = CreateUnitIndices();
		#endregion
		#region Helper functions
		static Dictionary<string, int> CreateUnitIndices() {
			Dictionary<string, int> result = new Dictionary<string, int>();
			result.Add("kn", 0);
			result.Add("admkn", 1);
			result.Add("m/h", 2);
			result.Add("m/hr", 2);
			result.Add("m/s", 3);
			result.Add("m/sec", 3);
			result.Add("mph", 4);
			return result;
		}
		#endregion
		protected override bool CheckIndex(int index) {
			return (index >= 0) && (index < numberOfMultipliers);
		}
		protected override double[,] Multipliers { get { return multipliers; } }
		public override Dictionary<string, int> UnitIndices { get { return unitIndices; } }
		protected internal override int[] MetricPowerTable { get { return metricPowerTable; } }
	}
	#endregion
	#region TemperatureUnitCategory
	class TemperatureUnitCategory : UnitCategory {
		readonly static Dictionary<string, int> unitIndices = CreateUnitIndices();
		readonly static IUnitConverter[] converters = CreateConverters();
		readonly static int[] metricPowerTable = new int[] { 0, 1, 0, 0, 0 };
		static Dictionary<string, int> CreateUnitIndices() {
			Dictionary<string, int> result = new Dictionary<string, int>();
			result.Add("C", 0);
			result.Add("cel", 0);
			result.Add("K", 1);
			result.Add("kel", 1);
			result.Add("F", 2);
			result.Add("fah", 2);
			result.Add("Rank", 3);
			result.Add("Reau", 4);
			return result;
		}
		static IUnitConverter[] CreateConverters() {
			return new IUnitConverter[] {
				new IdentityUnitConverter("C", false), new CelsiusToKelvinUnitConverter("C", "K"), new CelsiusToFahrenheitUnitConverter("C", "F"), new CelsiusToRankineUnitConverter("C", "Rank"), new CelsiusToReaumurUnitConverter("C", "Reau"),
				new KelvinToCelsiusUnitConverter("K", "C"), new IdentityUnitConverter("K", true), new KelvinToFahrenheitUnitConverter("K", "F"), new KelvinToRankineUnitConverter("K", "Rank"), new KelvinToReaumurUnitConverter("K", "Reau"),
				new FahrenheitToCelsiusUnitConverter("F", "C"), new FahrenheitToKelvinUnitConverter("F", "K"), new IdentityUnitConverter("F", false), new FahrenheitToRankineUnitConverter("F", "Rank"), new FahrenheitToReaumurUnitConverter("F", "Reau"),
				new RankineToCelsiusUnitConverter("Rank", "C"), new RankineToKelvinUnitConverter("Rank", "K"), new RankineToFahrenheitUnitConverter("Rank", "F"), new IdentityUnitConverter("Rank", false), new RankineToReaumurUnitConverter("Rank", "Reau"),
				new ReaumurToCelsiusUnitConverter("Reau", "C"), new ReaumurToKelvinUnitConverter("Reau", "K"), new ReaumurToFahrenheitUnitConverter("Reau", "F"), new ReaumurToRankineUnitConverter("Reau", "Rank"), new IdentityUnitConverter("Reau", false),
			};
		}
		public override Dictionary<string, int> UnitIndices { get { return unitIndices; } }
		protected internal override int[] MetricPowerTable { get { return metricPowerTable; } }
		public override double Convert(double value, int fromIndex, int toIndex) {
			if (fromIndex == toIndex)
				return value;
			int index = fromIndex * UniqueUnitsCount + toIndex;
			return converters[index].Convert(value);
		}
		public override void Convert(double[] values, int fromIndex, int toIndex) {
			if (fromIndex == toIndex)
				return;
			int index = fromIndex * UniqueUnitsCount + toIndex;
			IUnitConverter converter = converters[index];
			int count = values.Length;
			for (int i = 0; i < count; i++)
				values[i] = converter.Convert(values[i]);
		}
		public override void Convert(IList<double> values, int fromIndex, int toIndex) {
			if (fromIndex == toIndex)
				return;
			int index = fromIndex * UniqueUnitsCount + toIndex;
			IUnitConverter converter = converters[index];
			int count = values.Count;
			for (int i = 0; i < count; i++)
				values[i] = converter.Convert(values[i]);
		}
	}
	#endregion
	#region IUnitConverter
	public interface IUnitConverter {
		string From { get; }
		string To { get; }
		bool IsMetricFrom { get; }
		bool IsMetricTo { get; }
		double Convert(double value);
	}
	#endregion
	#region IdentityUnitConverter
	class IdentityUnitConverter : IUnitConverter {
		readonly string from;
		readonly string to;
		readonly bool isMetric;
		public IdentityUnitConverter(string unit, bool isMetric) {
			this.from = unit;
			this.to = unit;
			this.isMetric = isMetric;
		}
		public IdentityUnitConverter(string from, string to, bool isMetric) {
			this.from = from;
			this.to = to;
			this.isMetric = isMetric;
		}
		public string From { get { return from; } }
		public string To { get { return to; } }
		public bool IsMetricFrom { get { return isMetric; } }
		public bool IsMetricTo { get { return isMetric; } }
		public double Convert(double value) {
			return value;
		}
	}
	#endregion
	#region SimpleUnitConverter
	class SimpleUnitConverter : IUnitConverter {
		public SimpleUnitConverter(string from, string to, bool isMetricFrom, bool isMetricTo, double multiplier, double divisor) {
			this.From = from;
			this.To = to;
			this.IsMetricFrom = isMetricFrom;
			this.IsMetricTo = isMetricTo;
			this.Multiplier = multiplier;
			this.Divisor = divisor;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get; private set; }
		public bool IsMetricTo { get; private set; }
		public double Multiplier { get; private set; }
		public double Divisor { get; private set; }
		public double Convert(double value) {
			return (double)((double)(value * Multiplier) / Divisor);
		}
	}
	#endregion
	#region CelsiusToKelvinUnitConverter
	class CelsiusToKelvinUnitConverter : IUnitConverter {
		public CelsiusToKelvinUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return false; } }
		public bool IsMetricTo { get { return true; } }
		public double Convert(double value) {
			return (double)(value + 273.15);
		}
	}
	#endregion
	#region CelsiusToFahrenheitUnitConverter
	class CelsiusToFahrenheitUnitConverter : IUnitConverter {
		public CelsiusToFahrenheitUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return false; } }
		public bool IsMetricTo { get { return false; } }
		public double Convert(double value) {
			return (double)((double)(1.8 * value) + 32.0);
		}
	}
	#endregion
	#region CelsiusToRankineUnitConverter
	class CelsiusToRankineUnitConverter : IUnitConverter {
		public CelsiusToRankineUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return false; } }
		public bool IsMetricTo { get { return false; } }
		public double Convert(double value) {
			return (double)((double)(value + 273.15) * 1.8);
		}
	}
	#endregion
	#region CelsiusToReaumurUnitConverter
	class CelsiusToReaumurUnitConverter : IUnitConverter {
		public CelsiusToReaumurUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return false; } }
		public bool IsMetricTo { get { return false; } }
		public double Convert(double value) {
			return (double)(value * 0.8);
		}
	}
	#endregion
	#region KelvinToCelsiusUnitConverter
	class KelvinToCelsiusUnitConverter : IUnitConverter {
		public KelvinToCelsiusUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return true; } }
		public bool IsMetricTo { get { return false; } }
		public double Convert(double value) {
			return (double)(value - 273.15);
		}
	}
	#endregion
	#region KelvinToFahrenheitUnitConverter
	class KelvinToFahrenheitUnitConverter : IUnitConverter {
		public KelvinToFahrenheitUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return true; } }
		public bool IsMetricTo { get { return false; } }
		public double Convert(double value) {
			return (double)((double)(1.8 * (double)(value - 273.15)) + 32.0);
		}
	}
	#endregion
	#region KelvinToRankineUnitConverter
	class KelvinToRankineUnitConverter : IUnitConverter {
		public KelvinToRankineUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return true; } }
		public bool IsMetricTo { get { return false; } }
		public double Convert(double value) {
			return (double)(value * 1.8);
		}
	}
	#endregion
	#region KelvinToReaumurUnitConverter
	class KelvinToReaumurUnitConverter : IUnitConverter {
		public KelvinToReaumurUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return true; } }
		public bool IsMetricTo { get { return false; } }
		public double Convert(double value) {
			return (double)((double)(value - 273.15) * 0.8);
		}
	}
	#endregion
	#region FahrenheitToCelsiusUnitConverter
	class FahrenheitToCelsiusUnitConverter : IUnitConverter {
		public FahrenheitToCelsiusUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return false; } }
		public bool IsMetricTo { get { return false; } }
		public double Convert(double value) {
			return (double)((double)(value - 32.0) / 1.8);
		}
	}
	#endregion
	#region FahrenheitToKelvinUnitConverter
	class FahrenheitToKelvinUnitConverter : IUnitConverter {
		public FahrenheitToKelvinUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return false; } }
		public bool IsMetricTo { get { return true; } }
		public double Convert(double value) {
			return (double)((double)((double)(value - 32.0) / 1.8) + 273.15);
		}
	}
	#endregion
	#region FahrenheitToRankineUnitConverter
	class FahrenheitToRankineUnitConverter : IUnitConverter {
		public FahrenheitToRankineUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return false; } }
		public bool IsMetricTo { get { return false; } }
		public double Convert(double value) {
			return (double)(value + 459.67);
		}
	}
	#endregion
	#region FahrenheitToReaumurUnitConverter
	class FahrenheitToReaumurUnitConverter : IUnitConverter {
		public FahrenheitToReaumurUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return false; } }
		public bool IsMetricTo { get { return false; } }
		public double Convert(double value) {
			return (double)((double)(value - 32.0) / 2.25);
		}
	}
	#endregion
	#region RankineToCelsiusUnitConverter
	class RankineToCelsiusUnitConverter : IUnitConverter {
		public RankineToCelsiusUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return false; } }
		public bool IsMetricTo { get { return false; } }
		public double Convert(double value) {
			return (double)((double)(value - 491.67) / 1.8);
		}
	}
	#endregion
	#region RankineToKelvinUnitConverter
	class RankineToKelvinUnitConverter : IUnitConverter {
		public RankineToKelvinUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return false; } }
		public bool IsMetricTo { get { return true; } }
		public double Convert(double value) {
			return (double)(value / 1.8);
		}
	}
	#endregion
	#region RankineToFahrenheitUnitConverter
	class RankineToFahrenheitUnitConverter : IUnitConverter {
		public RankineToFahrenheitUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return false; } }
		public bool IsMetricTo { get { return false; } }
		public double Convert(double value) {
			return (double)(value - 459.67);
		}
	}
	#endregion
	#region RankineToReaumurUnitConverter
	class RankineToReaumurUnitConverter : IUnitConverter {
		public RankineToReaumurUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return false; } }
		public bool IsMetricTo { get { return false; } }
		public double Convert(double value) {
			return (double)((double)(value - 491.67) / 2.25);
		}
	}
	#endregion
	#region ReaumurToCelsiusUnitConverter
	class ReaumurToCelsiusUnitConverter : IUnitConverter {
		public ReaumurToCelsiusUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return false; } }
		public bool IsMetricTo { get { return false; } }
		public double Convert(double value) {
			return (double)(value * 1.25);
		}
	}
	#endregion
	#region ReaumurToKelvinUnitConverter
	class ReaumurToKelvinUnitConverter : IUnitConverter {
		public ReaumurToKelvinUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return false; } }
		public bool IsMetricTo { get { return true; } }
		public double Convert(double value) {
			return (double)((double)(value * 1.25) + 273.15);
		}
	}
	#endregion
	#region ReaumurToFahrenheitUnitConverter
	class ReaumurToFahrenheitUnitConverter : IUnitConverter {
		public ReaumurToFahrenheitUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return false; } }
		public bool IsMetricTo { get { return false; } }
		public double Convert(double value) {
			return (double)((double)(value * 2.25) + 32.0);
		}
	}
	#endregion
	#region ReaumurToRankineUnitConverter
	class ReaumurToRankineUnitConverter : IUnitConverter {
		public ReaumurToRankineUnitConverter(string from, string to) {
			this.From = from;
			this.To = to;
		}
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsMetricFrom { get { return false; } }
		public bool IsMetricTo { get { return false; } }
		public double Convert(double value) {
			return (double)((double)(value * 2.25) + 491.67);
		}
	}
	#endregion
}
