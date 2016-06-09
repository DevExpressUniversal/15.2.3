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
using DevExpress.Data.Browsing;
using DevExpress.XtraCharts.Localization;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	public class DataSourceValues : SortedDictionary<string, object> {
		internal DataSourceValues() { }
	}
	public delegate ISeriesPoint[] SummaryFunction(Series series, object argument, string[] functionArguments, DataSourceValues[] values, object[] colors);
	public struct SummaryFunctionArgumentDescription {
		string name;
		ScaleType? scaleType;
		public string Name { get { return name; } }
		public ScaleType? ScaleType { get { return scaleType; } }
		SummaryFunctionArgumentDescription(string name, ScaleType? scaleType) {
			this.name = name;
			this.scaleType = scaleType;
		}
		public SummaryFunctionArgumentDescription(string name)
			: this(name, null) {
		}
		public SummaryFunctionArgumentDescription(string name, ScaleType scaleType)
			: this(name, (ScaleType?)scaleType) {
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class SummaryFunctionDescription {
		string name;
		string displayName;
		ScaleType? resultScaleType;
		int resultDimension;
		SummaryFunctionArgumentDescription[] argumentDescriptions;
		SummaryFunction function;
		bool isStandard;
		public string Name { get { return name; } }
		public string DisplayName { get { return displayName; } }
		public ScaleType? ResultScaleType { get { return resultScaleType; } }
		public int ResultDimension { get { return resultDimension; } }
		public SummaryFunctionArgumentDescription[] ArgumentDescriptions { get { return argumentDescriptions; } }
		public SummaryFunction Function { get { return function; } }
		public bool IsStandard { get { return isStandard; } }
		public SummaryFunctionDescription(string name, string displayName, ScaleType? resultScaleType, int resultDimension, SummaryFunctionArgumentDescription[] argumentDescriptions, SummaryFunction function, bool isStandard) {
			this.name = name;
			this.displayName = displayName;
			this.resultScaleType = resultScaleType;
			this.resultDimension = resultDimension;
			this.argumentDescriptions = argumentDescriptions == null ? new SummaryFunctionArgumentDescription[0] : argumentDescriptions;
			this.function = function;
			this.isStandard = isStandard;
		}
		public override string ToString() {
			return displayName;
		}
	}
	public class SummaryFunctionsStorage : IEnumerable<SummaryFunctionDescription> {
		List<SummaryFunctionDescription> innerList = new List<SummaryFunctionDescription>();
		public SummaryFunctionDescription this[string name] {
			get {
				foreach (SummaryFunctionDescription desc in innerList)
					if (desc.Name == name)
						return desc;
				return null;
			}
		}
		public SummaryFunctionsStorage() {
		}
		public void Add(SummaryFunctionDescription description) {
			Remove(description.Name);
			innerList.Add(description);
		}
		public void Remove(string name) {
			SummaryFunctionDescription desc = this[name];
			if (desc != null)
				innerList.Remove(desc);
		}
		public void Assign(SummaryFunctionsStorage storage) {
			innerList.Clear();
			innerList.AddRange(storage.innerList);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return innerList.GetEnumerator();
		}
		IEnumerator<SummaryFunctionDescription> IEnumerable<SummaryFunctionDescription>.GetEnumerator() {
			return innerList.GetEnumerator();
		}
	}
	public class SummaryFunctionParser {
		static void ThrowException() {
			throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectSummaryFunction));
		}
		static bool IsStartIdentifierCharacter(char c) {
			return Char.IsLetter(c) || c == '_';
		}
		static bool IsIdentifierCharacter(char c) {
			return IsStartIdentifierCharacter(c) || Char.IsDigit(c);
		}
		static bool CheckIdentifier(string identifier) {
			if (String.IsNullOrEmpty(identifier) || !IsStartIdentifierCharacter(identifier[0]))
				return false;
			for (int i = 1; i < identifier.Length; i++)
				if (!IsIdentifierCharacter(identifier[i]))
					return false;
			return true;
		}
		static string[] SplitArguments(string argumentsString) {
			List<string> result = new List<string>();
			for (; ; ) {
				if (argumentsString.Length < 2)
					ThrowException();
				if (argumentsString[0] != '[')
					ThrowException();
				int closeBracketPos = argumentsString.IndexOf(']', 1);
				if (closeBracketPos < 0)
					ThrowException();
				string argument = argumentsString.Substring(1, closeBracketPos - 1);
				if (String.IsNullOrEmpty(argument))
					ThrowException();
				result.Add(argument);
				argumentsString = argumentsString.Substring(closeBracketPos + 1).Trim();
				if (String.IsNullOrEmpty(argumentsString))
					break;
				if (argumentsString[0] != ',')
					ThrowException();
				argumentsString = argumentsString.Substring(1).Trim();
			}
			return result.ToArray();
		}
		string functionString;
		string functionName = null;
		string[] arguments = new string[0];
		public string FunctionName { get { return functionName; } }
		public string[] Arguments { get { return arguments; } }
		public SummaryFunctionParser(string functionString) {
			this.functionString = functionString == null ? String.Empty : functionString;
			Parse();
		}
		public bool UpdateFunctionName(string functionName, SummaryFunctionsStorage storage) {
			functionName = functionName.Trim();
			if (String.IsNullOrEmpty(functionName)) {
				this.functionName = String.Empty;
				return true;
			}
			if (!CheckIdentifier(functionName))
				return false;
			this.functionName = functionName;
			SummaryFunctionDescription desc = storage[functionName];
			if (desc != null) {
				functionName = desc.Name;
				Array.Resize<string>(ref arguments, desc.ArgumentDescriptions.Length);
			}
			return true;
		}
		public string GetSummaryFunction() {
			if (String.IsNullOrEmpty(functionName))
				return String.Empty;
			string result = functionName + '(';
			for (int i = 0; i < arguments.Length; i++) {
				if (String.IsNullOrEmpty(arguments[i]))
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgSummaryFunctionParameterIsNotSpecified));
				if (i > 0)
					result += ',';
				result += '[' + arguments[i] + ']';
			}
			result += ')';
			return result;
		}
		public string GetDisplayText(SummaryFunctionsStorage functions, DataContext dataContext, object dataSource, string chartDataMember) {
			if (String.IsNullOrEmpty(functionName))
				return String.Empty;
			SummaryFunctionDescription description = functions[functionName];
			string displayText = (description == null ? functionName : description.DisplayName) + '(';
			for (int i = 0; i < arguments.Length; i++) {
				if (String.IsNullOrEmpty(arguments[i]))
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgSummaryFunctionParameterIsNotSpecified));
				if (i > 0)
					displayText += ',';
				displayText += '[' + BindingHelper.GetDataMemberName(dataContext, dataSource, chartDataMember, arguments[i]) + ']';
			}
			displayText += ')';
			return displayText;
		}
		public void AddNewArgument() {
			Array.Resize<string>(ref arguments, arguments.Length + 1);
		}
		public void RemoveArgument(int index) {
			if (index < 0 || index >= arguments.Length)
				throw new ArgumentException();
			if (index < arguments.Length - 1)
				Array.Copy(arguments, index + 1, arguments, index, arguments.Length - index - 1);
			Array.Resize<string>(ref arguments, arguments.Length - 1);
		}
		void Parse() {
			functionString = functionString.Trim();
			if (String.IsNullOrEmpty(functionString))
				return;
			int openBracketPos = functionString.IndexOf('(');
			int closeBracketPos = functionString.LastIndexOf(')');
			if (openBracketPos <= 0 || closeBracketPos < openBracketPos || !String.IsNullOrEmpty(functionString.Substring(closeBracketPos + 1).Trim()))
				ThrowException();
			functionName = functionString.Substring(0, openBracketPos).Trim();
			if (!CheckIdentifier(functionName))
				ThrowException();
			string argumentsString = functionString.Substring(openBracketPos + 1, closeBracketPos - openBracketPos - 1).Trim();
			arguments = String.IsNullOrEmpty(argumentsString) ? new string[0] : SplitArguments(argumentsString);
		}
	}
	public static class DefaultSummaryFunctions {
		static bool CheckArgument(object argument) {
			string str = argument as string;
			return str == null || str.Length != 0;
		}
		static IList<Object> ConstructTag(DataSourceValues[] values) {
			List<Object> res = new List<object>();
			foreach (DataSourceValues value in values) {
				object tag;
				if (value.TryGetValue(string.Empty, out tag))
					res.Add(tag);
			}
			return res;
		}
		static ISeriesPoint[] CalcMin(Series series, object argument, string[] functionArguments, DataSourceValues[] values, object[] colors) {
			if (!CheckArgument(argument))
				return new SeriesPoint[0];
			if (series.ValueScaleType == ScaleType.DateTime) {
				DateTime? min = null;
				int minIndex = -1;
				for (int index = 0; index < values.Length; index++) {
					DataSourceValues item = values[index];
					object value = item[functionArguments[0]];
					if (value != DBNull.Value && value != null) {
						string str = value.ToString();
						if (!String.IsNullOrEmpty(str)) {
							DateTime data = Convert.ToDateTime(str);
							if (min == null || DateTime.Compare(data, min.Value) < 0) {
								minIndex = index;
								min = data;
							}
						}
					}
				}
				ISeriesPointFactory pointFactory = series as ISeriesPointFactory;
				if (pointFactory != null && colors != null)
					return new ISeriesPoint[] { pointFactory.CreateSeriesPoint(series, argument, new object[] { min }, ConstructTag(values), colors.Length > 0 ? new object[] { colors[minIndex] } : null) };
				else
					return new SeriesPoint[] { min == null ? new SeriesPoint(argument) { Tag = ConstructTag(values) } : new SeriesPoint(argument, min) { Tag = ConstructTag(values) } };
			}
			else {
				double? min = null;
				int minIndex = -1;
				for (int index = 0; index < values.Length; index++) {
					DataSourceValues item = values[index];
					object value = item[functionArguments[0]];
					if (value != DBNull.Value && value != null) {
						string str = value.ToString();
						if (!String.IsNullOrEmpty(str)) {
							double data = Convert.ToDouble(str);
							if (min == null || data < min.Value) {
								minIndex = index;
								min = data;
							}
						}
					}
				}
				ISeriesPointFactory pointFactory = series as ISeriesPointFactory;
				if (pointFactory != null && colors != null)
					return new ISeriesPoint[] { pointFactory.CreateSeriesPoint(series, argument, new object[] { min }, ConstructTag(values), colors.Length > 0 ? new object[] { colors[minIndex] } : null) };
				else
					return new SeriesPoint[] { min == null ? new SeriesPoint(argument) { Tag = ConstructTag(values) } : new SeriesPoint(argument, min) { Tag = ConstructTag(values) } };
			}
		}
		static ISeriesPoint[] CalcMax(Series series, object argument, string[] functionArguments, DataSourceValues[] values, object[] colors) {
			if (!CheckArgument(argument))
				return new SeriesPoint[0];
			if (series.ValueScaleType == ScaleType.DateTime) {
				DateTime? max = null;
				int maxIndex = -1;
				for (int index = 0; index < values.Length; index++) {
					DataSourceValues item = values[index];
					object value = item[functionArguments[0]];
					if (value != DBNull.Value && value != null) {
						string str = value.ToString();
						if (!String.IsNullOrEmpty(str)) {
							DateTime data = Convert.ToDateTime(str);
							if (max == null || DateTime.Compare(data, max.Value) > 0) {
								maxIndex = index;
								max = data;
							}
						}
					}
				}
				ISeriesPointFactory pointFactory = series as ISeriesPointFactory;
				if (pointFactory != null && colors != null)
					return new ISeriesPoint[] { pointFactory.CreateSeriesPoint(series, argument, new object[] { max }, ConstructTag(values), colors.Length > 0 ? new object[] { colors[maxIndex] } : null) };
				else
					return new SeriesPoint[] { max == null ? new SeriesPoint(argument) { Tag = ConstructTag(values) } : new SeriesPoint(argument, max) { Tag = ConstructTag(values) } };
			}
			else {
				double? max = null;
				int maxIndex = -1;
				for (int index = 0; index < values.Length; index++) {
					DataSourceValues item = values[index];
					object value = item[functionArguments[0]];
					if (value != DBNull.Value && value != null) {
						string str = value.ToString();
						if (!String.IsNullOrEmpty(str)) {
							double data = Convert.ToDouble(str);
							if (max == null || data > max.Value) {
								maxIndex = index;
								max = data;
							}
						}
					}
				}
				ISeriesPointFactory pointFactory = series as ISeriesPointFactory;
				if (pointFactory != null && colors != null) 
					return new ISeriesPoint[] { pointFactory.CreateSeriesPoint(series, argument, new object[] { max }, ConstructTag(values), colors.Length > 0 ? new object[] { colors[maxIndex] } : null) };
				else
					return new SeriesPoint[] { max == null ? new SeriesPoint(argument) { Tag = ConstructTag(values) } : new SeriesPoint(argument, max) { Tag = ConstructTag(values) } };
			}
		}
		static ISeriesPoint[] CalcSum(Series series, object argument, string[] functionArguments, DataSourceValues[] values, object[] colors) {
			if (!CheckArgument(argument))
				return new SeriesPoint[0];
			double sum = 0.0;
			foreach (DataSourceValues item in values) {
				object value = item[functionArguments[0]];
				if (value != DBNull.Value && value != null) {
					string str = value.ToString();
					if (!String.IsNullOrEmpty(str))
						sum += Convert.ToDouble(str);
				}
			}
			ISeriesPointFactory pointFactory = series as ISeriesPointFactory;
			if (pointFactory != null && colors != null)
				return new ISeriesPoint[] { pointFactory.CreateSeriesPoint(series, argument, new object[] { sum }, ConstructTag(values), colors) };
			else
				return new ISeriesPoint[] { new SeriesPoint(argument, sum) { Tag = ConstructTag(values) } };
		}
		static ISeriesPoint[] CalcAverage(Series series, object argument, string[] functionArguments, DataSourceValues[] values, object[] colors) {
			if (!CheckArgument(argument))
				return new SeriesPoint[0];
			double sum = 0.0;
			int count = 0;
			foreach (DataSourceValues item in values) {
				object value = item[functionArguments[0]];
				if (value != DBNull.Value && value != null) {
					string str = value.ToString();
					if (!String.IsNullOrEmpty(str)) {
						sum += Convert.ToDouble(str);
						count++;
					}
				}
			}
			ISeriesPointFactory pointFactory = series as ISeriesPointFactory;
			if (pointFactory != null && colors != null) {
				double average = count == 0 ? 0 : sum / count;
				return new ISeriesPoint[] { pointFactory.CreateSeriesPoint(series, argument, new object[] { average }, ConstructTag(values), colors) };
			}
			else
				return new SeriesPoint[] { count == 0 ? new SeriesPoint(argument) { Tag = ConstructTag(values) } : new SeriesPoint(argument, sum / count) { Tag = ConstructTag(values) } };
		}
		static ISeriesPoint[] CalcCount(Series series, object argument, string[] functionArguments, DataSourceValues[] values, object[] colors) {
			if (!CheckArgument(argument))
				return new SeriesPoint[0];
			ISeriesPointFactory pointFactory = series as ISeriesPointFactory;
			if (pointFactory != null && colors != null)
				return new ISeriesPoint[] { pointFactory.CreateSeriesPoint(series, argument, new object[] { values.Length }, ConstructTag(values), colors) };
			else
				return new SeriesPoint[] { new SeriesPoint(argument, values.Length) { Tag = ConstructTag(values) } };
		}
		public static SummaryFunctionsStorage CreateStorage() {
			SummaryFunctionArgumentDescription[] argumentDescriptions = new SummaryFunctionArgumentDescription[] { 
				new SummaryFunctionArgumentDescription(ChartLocalizer.GetString(ChartStringId.FunctionArgumentName)) };
			SummaryFunctionArgumentDescription[] sumArgumentDescriptions = new SummaryFunctionArgumentDescription[] { 
				new SummaryFunctionArgumentDescription(ChartLocalizer.GetString(ChartStringId.FunctionArgumentName), ScaleType.Numerical) };
			SummaryFunctionsStorage storage = new SummaryFunctionsStorage();
			storage.Add(new SummaryFunctionDescription("MIN", ChartLocalizer.GetString(ChartStringId.FunctionNameMin), null, 1, argumentDescriptions, CalcMin, true));
			storage.Add(new SummaryFunctionDescription("MAX", ChartLocalizer.GetString(ChartStringId.FunctionNameMax), null, 1, argumentDescriptions, CalcMax, true));
			storage.Add(new SummaryFunctionDescription("SUM", ChartLocalizer.GetString(ChartStringId.FunctionNameSum), ScaleType.Numerical, 1, sumArgumentDescriptions, CalcSum, true));
			storage.Add(new SummaryFunctionDescription("AVERAGE", ChartLocalizer.GetString(ChartStringId.FunctionNameAverage), ScaleType.Numerical, 1, sumArgumentDescriptions, CalcAverage, true));
			storage.Add(new SummaryFunctionDescription("COUNT", ChartLocalizer.GetString(ChartStringId.FunctionNameCount), ScaleType.Numerical, 1, null, CalcCount, true));
			return storage;
		}
	}
}
