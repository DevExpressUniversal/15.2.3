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
using System.ComponentModel;
using System.Globalization;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using System.Diagnostics;
using DevExpress.Utils;
namespace DevExpress.Data.Helpers {
	public interface IByIntDictionary {
		bool TryGetValue(int index, out object value);
		bool TryGetKeyByValue(object value, out int index, int minIndex, int maxIndex);
		bool ContainsKey(int index);
		bool ContainsValue(object value);
		void Add(int index, object value);
		int GetFirstFilledIndex(int startIndex, bool isBackward);
	}
	public static class ByIntDictionary {
		public static IByIntDictionary CreateForType(Type keyType) {
			Type unnulledType = Nullable.GetUnderlyingType(keyType);
			if(unnulledType != null)
				keyType = unnulledType;
			Type dictTypeGen = typeof(ByIntDictionary<>);
			Type dictType = dictTypeGen.MakeGenericType(keyType);
			IByIntDictionary rv = (IByIntDictionary)Activator.CreateInstance(dictType);
			return rv;
		}
	}
	public abstract class ByIntDictionaryPage<T> {
		public abstract void Add(int index, T value);
		public abstract bool ContainsKey(int index);
		public abstract bool TryGetValue(int index, out T value);
		public abstract bool TryGetKeyByValue(T value, out int index);
	}
	public class ByIntDictionaryPageForRefTypes<T>: ByIntDictionaryPage<T> where T: class {
		protected readonly T[] Data;
		public override void Add(int index, T value) {
			if(value == null)
				throw new ArgumentException("value");
			if(Data[index] != null)
				throw new InvalidOperationException(string.Format("Can't add value '{0}' for local index {1} because value '{2}' is already added for that index", value, index, Data[index]));
			Data[index] = value;
		}
		public override bool ContainsKey(int index) {
			return Data[index] != null;
		}
		public override bool TryGetValue(int index, out T value) {
			value = Data[index];
			return value != null;
		}
		public override bool TryGetKeyByValue(T value, out int index) {
			if(value == null)
				throw new ArgumentException("value");
			index = Array.IndexOf(Data, value);
			return index >= 0;
		}
		public ByIntDictionaryPageForRefTypes(int dataSize) {
			Data = new T[dataSize];
		}
	}
	public class ByIntDictionaryPageForValueTypes<T>: ByIntDictionaryPage<T> where T: struct {
		protected readonly T?[] Data;
		public override void Add(int index, T value) {
			if(Data[index].HasValue)
				throw new InvalidOperationException(string.Format("Can't add value '{0}' for local index {1} because value '{2}' is already added for that index", value, index, Data[index].Value));
			Data[index] = value;
		}
		public override bool ContainsKey(int index) {
			return Data[index].HasValue;
		}
		public override bool TryGetValue(int index, out T value) {
			T? v = Data[index];
			if(v.HasValue) {
				value = v.Value;
				return true;
			} else {
				value = default(T);
				return false;
			}
		}
		public override bool TryGetKeyByValue(T value, out int index) {
			index = Array.IndexOf(Data, (T?)value);
			return index >= 0;
		}
		public ByIntDictionaryPageForValueTypes(int dataSize) {
			Data = new T?[dataSize];
		}
	}
	public class ByIntDictionary<T>: IByIntDictionary {
		protected readonly int PageSize;
		protected ByIntDictionaryPage<T>[] Pages = new ByIntDictionaryPage<T>[0];
		public ByIntDictionary() {
			PageSize = 4000;
		}
		void SplitToPage(int index, out int pageIndex, out int indexWithinPage) {
			pageIndex = index / PageSize;
			indexWithinPage = index % PageSize;
			System.Diagnostics.Debug.Assert(indexWithinPage == index - pageIndex * PageSize);
		}
		int ComposeFromPageIndex(int pageIndex, int indexWithinPage) {
			return pageIndex * PageSize + indexWithinPage;
		}
		public bool TryGetValue(int index, out object value) {
			if(index >= 0) {
				int pageIndex;
				int withinPageIndex;
				SplitToPage(index, out pageIndex, out withinPageIndex);
				if(Pages.Length > pageIndex) {
					ByIntDictionaryPage<T> page = Pages[pageIndex];
					if(page != null) {
						T val;
						if(page.TryGetValue(withinPageIndex, out val)) {
							value = val;
							return true;
						}
					}
				}
			}
			value = null;
			return false;
		}
		public bool TryGetKeyByValue(object value, out int index, int minIndex, int maxIndex) {
			if(value is T) {
				if(minIndex < 0)
					minIndex = 0;
				int minPage;
				int maxPage;
				int dummy;
				SplitToPage(minIndex, out minPage, out dummy);
				SplitToPage(maxIndex, out maxPage, out dummy);
				if(maxPage >= Pages.Length)
					maxPage = Pages.Length - 1;
				T val = (T)value;
				for(int i = minPage; i <= maxPage; ++i) {
					ByIntDictionaryPage<T> page = Pages[i];
					if(page == null)
						continue;
					int rvIndex;
					if(page.TryGetKeyByValue(val, out rvIndex)) {
						index = ComposeFromPageIndex(i, rvIndex);
						return true;
					}
				}
			}
			index = default(int);
			return false;
		}
		public bool ContainsKey(int index) {
			if(index >= 0) {
				int pageIndex;
				int withinPageIndex;
				SplitToPage(index, out pageIndex, out withinPageIndex);
				if(Pages.Length > pageIndex) {
					ByIntDictionaryPage<T> page = Pages[pageIndex];
					if(page != null) {
						if(page.ContainsKey(withinPageIndex)) {
							return true;
						}
					}
				}
			}
			return false;
		}
		public bool ContainsValue(object value) {
			int dummy;
			return TryGetKeyByValue(value, out dummy, 0, int.MaxValue);
		}
		public void Add(int index, object value) {
			if(value == null)
				throw new ArgumentNullException("value");
			if(index < 0)
				throw new ArgumentException("index is less then zero");
			int pageIndex;
			int withinPageIndex;
			SplitToPage(index, out pageIndex, out withinPageIndex);
			if(Pages.Length <= pageIndex) {
				ByIntDictionaryPage<T>[] newPages = new ByIntDictionaryPage<T>[pageIndex + 1];
				Pages.CopyTo(newPages, 0);
				Pages = newPages;
			}
			ByIntDictionaryPage<T> page = Pages[pageIndex];
			if(page == null) {
				Type pageType;
				if(typeof(T).IsValueType()) {
					pageType = typeof(ByIntDictionaryPageForValueTypes<>);
				} else {
					pageType = typeof(ByIntDictionaryPageForRefTypes<>);
				}
				page = (ByIntDictionaryPage<T>)Activator.CreateInstance(pageType.MakeGenericType(typeof(T)), PageSize);
				Pages[pageIndex] = page;
			}
			page.Add(withinPageIndex, (T)value);
		}
		public int GetFirstFilledIndex(int startIndex, bool isBackward) {
			if(isBackward)
				return GetFirstFilledIndexBackward(startIndex);
			else
				return GetFirstFilledIndexForward(startIndex);
		}
		int GetFirstFilledIndexForward(int startIndex) {
			int basePageIndex;
			{
				int baseInPage;
				SplitToPage(startIndex, out basePageIndex, out baseInPage);
				if(basePageIndex < Pages.Length) {
					ByIntDictionaryPage<T> basePage = Pages[basePageIndex];
					if(basePage != null) {
						for(int i = baseInPage; i < PageSize; ++i) {
							if(basePage.ContainsKey(i))
								return ComposeFromPageIndex(basePageIndex, i);
						}
					}
				}
			}
			for(int pageIndex = basePageIndex + 1; pageIndex < Pages.Length; ++pageIndex) {
				ByIntDictionaryPage<T> page = Pages[pageIndex];
				if(page == null)
					continue;
				for(int i = 0; i < PageSize; ++i) {
					if(page.ContainsKey(i))
						return ComposeFromPageIndex(pageIndex, i);
				}
			}
			return int.MaxValue;
		}
		int GetFirstFilledIndexBackward(int startIndex) {
			int basePageIndex;
			{
				int baseInPage;
				SplitToPage(startIndex, out basePageIndex, out baseInPage);
				if(basePageIndex < Pages.Length) {
					ByIntDictionaryPage<T> basePage = Pages[basePageIndex];
					if(basePage != null) {
						for(int i = baseInPage; i >= 0; --i) {
							if(basePage.ContainsKey(i))
								return ComposeFromPageIndex(basePageIndex, i);
						}
					}
				}
			}
			for(int pageIndex = Math.Min(basePageIndex, Pages.Length) - 1; pageIndex >= 0; --pageIndex) {
				ByIntDictionaryPage<T> page = Pages[pageIndex];
				if(page == null)
					continue;
				for(int i = PageSize - 1; i >= 0; --i) {
					if(page.ContainsKey(i))
						return ComposeFromPageIndex(pageIndex, i);
				}
			}
			return -1;
		}
	}
	public class ServerModeServerAndChannelModel {
		public readonly double ConstantPart;
		public readonly double TakeCoeff;
		public readonly double ScanCoeff;
		public ServerModeServerAndChannelModel(double constPart, double takeCoeff, double scanCoeff) {
			this.ConstantPart = constPart;
			this.TakeCoeff = takeCoeff;
			this.ScanCoeff = scanCoeff;
		}
		public override string ToString() {
			return string.Format("Take*{1} + Scan*{2} + {0}", ConstantPart, TakeCoeff, ScanCoeff);
		}
	}
	public class ServerModeServerAndChannelModelBuilder {
		public static bool TraceWriteLines = false;
		public const int MaxSamples = 1024;
		public static double[] Linear(int unknowns, double[,] data) {
			for(int i = 0; i < unknowns; ++i) {
				const double eps = float.Epsilon;
				if(Math.Abs(data[i, i]) < eps) {
					double bestV = Math.Abs(data[i, i]);
					int bestI = -1;
					for(int j = i + 1; j < unknowns; ++j) {
						double currentV = Math.Abs(data[j, i]);
						if(bestV < currentV) {
							bestV = currentV;
							bestI = j;
						}
					}
					if(bestI < 0)
						return null;
					for(int j = 0; j <= unknowns; ++j) {
						double buf = data[i, j];
						data[i, j] = data[bestI, j];
						data[bestI, j] = buf;
					}
				}
#if DEBUG
				for(int j = 0; j < i; ++j) {
					if(data[i, j] != 0.0)
						throw new InvalidOperationException();
				}
#endif
				double div = data[i, i];
				data[i, i] = 1.0;
				for(int j = i + 1; j <= unknowns; ++j) {
					data[i, j] /= div;
				}
				for(int k = 0; k < unknowns; ++k) {
					if(k == i)
						continue;
					double kdiv = data[k, i];
					data[k, i] = 0.0;
					for(int j = i + 1; j <= unknowns; ++j) {
						data[k, j] -= data[i, j] * kdiv;
					}
				}
			}
			double[] result = new double[unknowns];
			for(int i = 0; i < unknowns; ++i) {
				result[i] = data[i, unknowns];
			}
			return result;
		}
		public class LinearLeastSquaresArgs {
			public readonly int Unknowns;
			public readonly IEnumerable<double[]> Data;
			public LinearLeastSquaresArgs(int unknowns, IEnumerable<double[]> data) {
				this.Unknowns = unknowns;
				this.Data = data;
			}
		}
		public static double[] LinearLeastSquares(LinearLeastSquaresArgs args) {
			int unknowns = args.Unknowns;
			IEnumerable<double[]> data = args.Data;
			double[,] normal = new double[unknowns, unknowns + 1];
			foreach(double[] row in data) {
				for(int i = 0; i < unknowns; ++i) {
					for(int j = 0; j <= unknowns; ++j) {
						normal[i, j] += row[i] * row[j];
					}
				}
			}
			return Linear(unknowns, normal);
		}
		protected class Sample {
			public readonly int Take;
			public readonly int Scan;
			public readonly double Time;
			public Sample(int take, int scan, double time) {
				this.Take = take;
				this.Scan = scan;
				this.Time = time;
			}
		}
		protected readonly IList<Sample> Samples = new List<Sample>();
		public void RegisterSample(int take, int scan, double time) {
			Samples.Add(new Sample(take, scan, time));
			if(Samples.Count > MaxSamples)
				Samples.RemoveAt(0);
		}
		static double[] PackSample(Sample sample, string combination) {
			double[] rv = new double[combination.Length + 1];
			for(int i = 0; i < combination.Length; ++i) {
				char field = combination[i];
				double val;
				switch(field) {
					case 'C':
						val = 1.0;
						break;
					case 'T':
						val = sample.Take;
						break;
					case 'S':
						val = sample.Scan;
						break;
					default:
						throw new InvalidOperationException();
				}
				rv[i] = val;
			}
			rv[combination.Length] = sample.Time;
			return rv;
		}
		static ServerModeServerAndChannelModel UnpackSolution(double[] llsResults, string combination) {
			if(llsResults == null)
				return null;
			double c = 0.0;
			double t = 0.0;
			double s = 0.0;
			for(int i = 0; i < combination.Length; ++i) {
				char field = combination[i];
				double val = llsResults[i];
				if(double.IsInfinity(val) || double.IsNaN(val))
					return null;
				switch(field) {
					case 'C':
						c = val;
						break;
					case 'T':
						t = val;
						break;
					case 'S':
						s = val;
						break;
					default:
						throw new InvalidOperationException();
				}
			}
			return new ServerModeServerAndChannelModel(c, t, s);
		}
		static readonly string[] Combinations = new string[] { "C", "S", "T", "CS", "CT", "TS", "CTS" };   
		static IEnumerable<double[]> PackSamples(IEnumerable<Sample> samples, string combination) {
			foreach(Sample s in samples)
				yield return PackSample(s, combination);
		}
		static ServerModeServerAndChannelModel Resolve(IEnumerable<Sample> samples, string combination) {
			LinearLeastSquaresArgs llsArgs = new LinearLeastSquaresArgs(combination.Length, PackSamples(samples, combination));
			double[] sol = LinearLeastSquares(llsArgs);
			return UnpackSolution(sol, combination);
		}
		static double Weight(IEnumerable<Sample> samples, ServerModeServerAndChannelModel solution) {
			double weight = 0.0;
			foreach(Sample s in samples) {
				double prediction = solution.ConstantPart + solution.TakeCoeff * s.Take + solution.ScanCoeff * s.Scan;
				double delta = prediction - s.Time;
				double square = delta * delta;
				weight += square;
			}
			return weight;
		}
		static IEnumerable<T> Join<T>(IEnumerable<T> first, IEnumerable<T> second) {
			foreach(T t in first)
				yield return t;
			foreach(T t in second)
				yield return t;
		}
		static IEnumerable<T> Get2OfEvery3<T>(IEnumerable<T> src) {
			int i = 0;
			foreach(T t in src) {
				if(i == 1)
					yield return t;
				i = (i + 1) % 3;
			}
		}
		static IEnumerable<T> Get1And3OfEvery3<T>(IEnumerable<T> src) {
			int i = 0;
			foreach(T t in src) {
				if(i != 1)
					yield return t;
				i = (i + 1) % 3;
			}
		}
		public ServerModeServerAndChannelModel Resolve() {
			if(Samples.Count < 2)
				return null;
			ICollection<Sample> baseSequence;
			IEnumerable<Sample> validateSequence;
			if(TraceWriteLines) {
				Trace.TraceInformation("<=== {0} ===", this.GetType().Name);
				foreach(Sample sample in Samples) {
					Trace.TraceInformation("Scan: {1}\tTake: {2}\tTime: {0}", sample.Time, sample.Scan, sample.Take);
				}
			}
			bool needResolveOnFullSequence;
			if(Samples.Count < 9) {
				validateSequence = baseSequence = Samples;
				needResolveOnFullSequence = false;
			} else {
				baseSequence = new List<Sample>(Get1And3OfEvery3(Samples));
				validateSequence = new List<Sample>(Get2OfEvery3(Samples));
				needResolveOnFullSequence = true;
			}
			ServerModeServerAndChannelModel finalSolution = null;
			double finalSolutionWeight = double.PositiveInfinity;
			foreach(string comb in Combinations) {
				if(comb.Length >= baseSequence.Count)
					continue;
				ServerModeServerAndChannelModel s = Resolve(baseSequence, comb);
				if(s == null)
					continue;
				if(IsInvalid(s)) {
					if(TraceWriteLines) Trace.TraceInformation("Weight {1} for solution\t({0}) ( rejected )", s, Weight(validateSequence, s));
					continue;
				}
				double weight = Weight(validateSequence, s);
				if(TraceWriteLines) Trace.TraceInformation("Weight {1} for solution\t({0})", s, weight);
				if(weight >= finalSolutionWeight)
					continue;
				if(needResolveOnFullSequence) {
					ServerModeServerAndChannelModel correctedSolution = Resolve(Samples, comb);
					if(correctedSolution == null || IsInvalid(correctedSolution))
						continue;
					finalSolution = correctedSolution;
				} else {
					finalSolution = s;
				}
				finalSolutionWeight = weight;
			}
			if(TraceWriteLines) {
				Trace.TraceInformation("Winner: ({0})", finalSolution);
				Trace.TraceInformation(">=== {0} ===", this.GetType().Name);
			}
			return finalSolution;
		}
		static bool IsInvalid(ServerModeServerAndChannelModel s) {
			return s.ConstantPart < .0 || s.ScanCoeff < .0 || s.TakeCoeff < .0;
		}
		public int? GetMaxObservableTake() {
			int rv = 0;
			foreach(Sample s in Samples) {
				if(s.Take > rv)
					rv = s.Take;
			}
			if(rv > 0)
				return rv;
			else
				return null;
		}
	}
	public class ServerModeOptimalFetchResult {
		public readonly int Skip;
		public readonly int Take;
		public readonly bool IsFromEnd;
		public ServerModeOptimalFetchResult(bool isFromEnd, int skip, int take) {
			this.IsFromEnd = isFromEnd;
			this.Skip = skip;
			this.Take = take;
		}
	}
	public class ServerModeOptimalFetchParam {
		public readonly ServerModeServerAndChannelModel Model;
		public readonly int Index;
		public readonly int MinIndex;
		public readonly int MaxIndex;
		public readonly int TotalCount;
		public readonly int BaseCountTakeData;
		public readonly int MaxAllowedTake;
		public readonly double FillTimeMultiplier;
		public readonly double EdgeTimeMultiplier;
		public readonly double MiddleTimeMultiplier;
		public int SkipFromTop(int firstIndexToFetch) {
			return firstIndexToFetch;
		}
		public int ScanFromTop(int lastIndexToFetch) {
			return lastIndexToFetch + 1;
		}
		public int SkipFromBottom(int firstIndexToFetch) {
			return TotalCount - firstIndexToFetch - 1;
		}
		public int ScanFromBottom(int lastIndexToFetch) {
			return TotalCount - lastIndexToFetch;
		}
		public ServerModeOptimalFetchParam(ServerModeServerAndChannelModel model, int index, int minIndex, int maxIndex, int totalCount, int baseCountTakeData,
									int maxAllowedTake, double fillTimeMultiplier, double edgeTimeMultiplier, double middleTimeMultiplier) {
			if((index < 0) || (minIndex < 0) || (maxIndex < 0) || (index < minIndex) || (index > maxIndex) || (minIndex > maxIndex)
				|| (totalCount <= 0) || (baseCountTakeData < 0) || (totalCount < 0) || (middleTimeMultiplier < 1.0) || edgeTimeMultiplier < middleTimeMultiplier || fillTimeMultiplier < edgeTimeMultiplier)
				throw new NotSupportedException("Incorrect input data!");
			Model = model;
			Index = index;
			MinIndex = minIndex;
			MaxIndex = maxIndex;
			TotalCount = totalCount;
			BaseCountTakeData = baseCountTakeData;
			MaxAllowedTake = maxAllowedTake;
			FillTimeMultiplier = fillTimeMultiplier;
			EdgeTimeMultiplier = edgeTimeMultiplier;
			MiddleTimeMultiplier = middleTimeMultiplier;
		}
		public override string ToString() {
			return string.Format(CultureInfo.InvariantCulture, "new ServerModeOptimalFetchParam(new ServerModeServerAndChannelModel({0}, {1}, {2}), {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})"
				, Model.ConstantPart, Model.TakeCoeff, Model.ScanCoeff, Index, MinIndex, MaxIndex, TotalCount, BaseCountTakeData, MaxAllowedTake, FillTimeMultiplier, EdgeTimeMultiplier, MiddleTimeMultiplier);
		}
	}
	public static class ServerModeOptimalFetchHelper {
		public static ServerModeOptimalFetchResult CalculateOptimalFetchResult(ServerModeOptimalFetchParam resParam) {
			try {
				Solution internalResult = OptimalResultCore(resParam);
				return new ServerModeOptimalFetchResult(internalResult.IsFromEnd, internalResult.Skip, internalResult.Take);
			} catch(Exception e) {
				throw new InvalidOperationException("Invalid arguments or internal error. Repro args: " + resParam.ToString(), e);
			}
		}
		static Solution OptimalResultCore(ServerModeOptimalFetchParam p) {
			double baseTime;
			{
				int baseSkip = Math.Min(Math.Max(0, p.Index - p.BaseCountTakeData / 2), Math.Max(0, p.TotalCount - p.Index - 1 - p.BaseCountTakeData / 2));
				baseTime = p.Model.ConstantPart + p.Model.TakeCoeff * p.BaseCountTakeData + p.Model.ScanCoeff * (baseSkip + p.BaseCountTakeData);
			}
			int fillTake = p.MaxIndex - p.MinIndex + 1;
			if(fillTake <= p.MaxAllowedTake) {
				double targetFillTime = baseTime * p.FillTimeMultiplier;
				int skipFromTop = p.SkipFromTop(p.MinIndex);
				int skipFromBottom = p.SkipFromBottom(p.MaxIndex);
				if(skipFromTop <= skipFromBottom) {
					Solution s = Solution.FromSkipTake(p, false, skipFromTop, fillTake);
					if(targetFillTime >= s.Time)
						return s;
				} else {
					Solution s = Solution.FromSkipTake(p, true, skipFromBottom, fillTake);
					if(targetFillTime >= s.Time)
						return s;
				}
			}
			int maxTake = Math.Min(fillTake, p.MaxAllowedTake);
			{
				double edgeTime = baseTime * p.EdgeTimeMultiplier;
				List<Solution> solutions = new List<Solution>();
				{   
					{
						int skip = p.SkipFromTop(p.MinIndex);
						int? calculatedTake = CalculateTakeFromFixedTimeAndSkip(p.Model, edgeTime, skip);
						if(calculatedTake.HasValue) {
							int take = calculatedTake.Value;
							if(take > maxTake)
								take = maxTake;
							int lastIndex = skip + take - 1;
							if(p.Index <= lastIndex) {
								solutions.Add(Solution.FromSkipTake(p, false, skip, take));
							}
						}
					}
					{
						int skip = p.SkipFromBottom(p.MaxIndex);
						int? calculatedTake = CalculateTakeFromFixedTimeAndSkip(p.Model, edgeTime, skip);
						if(calculatedTake.HasValue) {
							int take = calculatedTake.Value;
							if(take > maxTake)
								take = maxTake;
							int lastIndex = p.TotalCount - skip - take;
							if(lastIndex <= p.Index) {
								solutions.Add(Solution.FromSkipTake(p, true, skip, take));
							}
						}
					}
					{
						int scan = p.MaxIndex + 1;
						int? calculatedTake = CalculateTakeFromFixedTimeAndScan(p.Model, edgeTime, scan);
						if(calculatedTake.HasValue) {
							int take = calculatedTake.Value;
							if(take > maxTake)
								take = maxTake;
							System.Diagnostics.Debug.Assert(take <= scan);
							int skip = scan - take;
							int firstIndex = skip;
							if(p.Index >= firstIndex) {
								solutions.Add(Solution.FromSkipTake(p, false, skip, take));
							}
						}
					}
					{
						int scan = p.TotalCount - p.MinIndex;
						int? calculatedTake = CalculateTakeFromFixedTimeAndScan(p.Model, edgeTime, scan);
						if(calculatedTake.HasValue) {
							int take = calculatedTake.Value;
							if(take > maxTake)
								take = maxTake;
							System.Diagnostics.Debug.Assert(take <= scan);
							int skip = scan - take;
							int firstIndex = p.TotalCount - skip - 1;
							if(p.Index <= firstIndex) {
								solutions.Add(Solution.FromSkipTake(p, true, skip, take));
							}
						}
					}
				}
				Solution bestSolution = null;
				foreach(Solution s in solutions) {
					if(bestSolution == null || s.Take > bestSolution.Take || (s.Take == bestSolution.Take && s.Scan < bestSolution.Scan))
						bestSolution = s;
				}
				if(bestSolution != null)
					return bestSolution;
			}
			{
				double targetTime = baseTime * p.MiddleTimeMultiplier;
				int scanToMiddleFromTop = p.Index + 1;
				int scanToMiddleFromBottom = p.TotalCount - p.Index;
				bool isFromBottom;
				int scanToMiddle;
				if(scanToMiddleFromTop < scanToMiddleFromBottom) {
					isFromBottom = false;
					scanToMiddle = scanToMiddleFromTop;
				} else {
					isFromBottom = true;
					scanToMiddle = scanToMiddleFromBottom;
				}
				double claculatedTake = (targetTime - p.Model.ScanCoeff * scanToMiddle - p.Model.ConstantPart) / (p.Model.TakeCoeff + p.Model.ScanCoeff / 2);
				if(claculatedTake < 1)
					throw new InvalidOperationException("internal error, take(middle) < 1");
				int take = claculatedTake >= maxTake ? maxTake : (int)claculatedTake;
				int skip = scanToMiddle - 1 - take / 2;
				return Solution.FromSkipTake(p, isFromBottom, skip, take);
			}
		}
		static int? CalculateTakeFromFixedTimeAndSkip(ServerModeServerAndChannelModel model, double targetTime, int skip) {
			double result = (targetTime - model.ScanCoeff * skip - model.ConstantPart) / (model.ScanCoeff + model.TakeCoeff);
			if(result < 1)
				return null;
			if(double.IsNaN(result) || double.IsInfinity(result) || result > int.MaxValue)
				return int.MaxValue;
			return (int)result;
		}
		static int? CalculateTakeFromFixedTimeAndScan(ServerModeServerAndChannelModel model, double targetTime, int scan) {
			double result = (targetTime - model.ScanCoeff * scan - model.ConstantPart) / model.TakeCoeff;
			if(result < 1)
				return null;
			if(double.IsNaN(result) || double.IsInfinity(result) || result >= int.MaxValue)
				return int.MaxValue;
			return (int)result;
		}
		class Solution {
			public readonly int Skip;
			public readonly int Take;
			public readonly bool IsFromEnd;
			readonly ServerModeOptimalFetchParam Params;
			public int Scan { get { return Skip + Take; } }
			public double Time { get { return Params.Model.ScanCoeff * Scan + Params.Model.TakeCoeff * Take + Params.Model.ConstantPart; } }
			Solution(int skip, int take, bool isFromEnd, ServerModeOptimalFetchParam _params) {
				if(take <= 0)
					throw new ArgumentException("take: " + take.ToString());
				if(skip < 0)
					throw new ArgumentException("skip: " + skip.ToString());
				if(skip + take > _params.TotalCount)
					throw new ArgumentException(string.Format("skip({0}) + take({1}) > TotalCount({2})", skip, take, _params.TotalCount));
				this.Skip = skip;
				this.Take = take;
				this.IsFromEnd = isFromEnd;
				this.Params = _params;
				int i1 = IsFromEnd ? Params.TotalCount - Skip - 1 : Skip;
				int i2 = IsFromEnd ? Params.TotalCount - Scan : Scan - 1;
				if(Params.Index < Math.Min(i1, i2) || Params.Index > Math.Max(i1, i2))
					throw new ArgumentException(string.Format("Index ({0}) is outside of range ({1}, {2})", Params.Index, i1, i2));
			}
			public static Solution FromSkipTake(ServerModeOptimalFetchParam _params, bool isFromEnd, int skip, int take) {
				return new Solution(skip, take, isFromEnd, _params);
			}
			public static Solution FromI1I2(ServerModeOptimalFetchParam _params, bool isFromEnd, int i1, int i2) {
				Solution rv;
				if(isFromEnd) {
					rv = FromSkipTake(_params, isFromEnd, _params.TotalCount - i1 - 1, i1 - i2 + 1);
				} else {
					rv = FromSkipTake(_params, isFromEnd, i1, i2 - i1 + 1);
				}
				return rv;
			}
		}
	}
}
