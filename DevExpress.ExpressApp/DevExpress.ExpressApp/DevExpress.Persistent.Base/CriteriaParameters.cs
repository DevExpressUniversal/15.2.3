#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Data.Filtering;
using System.Collections.ObjectModel;
namespace DevExpress.Persistent.Base {
	public class ParametersFactory {
		public const string RegistrationErrorText = "The '{0}' parameter is registered already: {1}";
		private static Dictionary<string, ParameterBase> registeredParameters = new Dictionary<string, ParameterBase>();
		static ParametersFactory() {
			RegisterParameter(new CurrentDateParameter());
			RegisterParameter(new CurrentDateTimeParameter());
			RegisterParameter(new LastWorkDayStartParameter());
			RegisterParameter(new LastWorkDayEndParameter());
			RegisterParameter(new LastWorkWeekStartParameter());
			RegisterParameter(new LastWorkWeekEndParameter());
			RegisterParameter(new CurrentDatePlus3DaysParameter());
			RegisterParameter(new CurrentDatePlus2DaysParameter());
			RegisterParameter(new CurrentDatePlus1DayParameter());
			RegisterParameter(new WeekAgoParameter());
		}
		public static void RegisterParameter(ParameterBase parameter) {
			ParameterBase registeredParameter = null;
			registeredParameters.TryGetValue(parameter.Name, out registeredParameter);
			if(registeredParameter != null) {
				if(registeredParameter.GetType().FullName == parameter.GetType().FullName) {
					registeredParameters[parameter.Name] = parameter;
					Tracing.Tracer.LogWarning("The '{0}' parameter is replaced.", parameter.Name);
				}
				else {
					throw new InvalidOperationException(
						String.Format(RegistrationErrorText, parameter.Name, registeredParameters[parameter.Name].GetType()));
				}
			}
			else {
				registeredParameters[parameter.Name] = parameter;
			}
		}
		public static IParameter CreateParameter(string parameterName) {
			ParameterBase parameter;
			if(registeredParameters.TryGetValue(parameterName, out parameter)) {
				return parameter.Clone();
			}
			else {
				return null;
			}
		}
		public static ParameterBase FindParameter(string name) {
			if(string.IsNullOrEmpty(name)) return null;
			ParameterBase parameter;
			registeredParameters.TryGetValue(name, out parameter);
			return parameter;
		}
		public static ReadOnlyCollection<string> GetRegisteredParameterNames() {
			return new ReadOnlyCollection<string>(new List<string>(registeredParameters.Keys));
		}
	}
	public enum PredefinedParameters {
		CurrentDate,
		CurrentDateTime,
		LastWorkDayStart,
		LastWorkDayEnd,
		LastWorkWeekStart,
		LastWorkWeekEnd,
		CurrentDatePlus1Day,
		CurrentDatePlus2Days,
		CurrentDatePlus3Days,
		WeekAgo
	}
	public abstract class PredefinedParameter : ReadOnlyParameter {
		public PredefinedParameter(PredefinedParameters predefinedParameter, Type parameterType) : base(predefinedParameter.ToString(), parameterType) { }
		public virtual FunctionOperatorType DefaultOperatorType { get { return FunctionOperatorType.None; } }
	}
	public class CurrentDateParameter : PredefinedParameter {
		public CurrentDateParameter() : base(PredefinedParameters.CurrentDate, typeof(DateTime)) { }
		public override object CurrentValue { get { return DateTime.Today; } }
		public override FunctionOperatorType DefaultOperatorType { get { return FunctionOperatorType.IsOutlookIntervalToday; } }
	}
	public class CurrentDateTimeParameter : PredefinedParameter {
		public CurrentDateTimeParameter() : base(PredefinedParameters.CurrentDateTime, typeof(DateTime)) { }
		public override object CurrentValue {  get { return DateTime.Now; } }
	}
	public class LastWorkDayStartParameter : PredefinedParameter {
		public LastWorkDayStartParameter() : base(PredefinedParameters.LastWorkDayStart, typeof(DateTime)) { }
		private DateTime GetPreviousWorkDay(DateTime dateTime) {
			int dayCount = 0;
			switch(dateTime.DayOfWeek) {
				case DayOfWeek.Sunday:
					dayCount = 2;
					break;
				case DayOfWeek.Saturday:
					dayCount = 1;
					break;
			}
			return dateTime.Subtract(new TimeSpan(dayCount, 0, 0, 0));
		}
		public override object CurrentValue {
			get { return GetPreviousWorkDay(DateTime.Today.Subtract(new TimeSpan(1, 0, 0, 0))); }
		}
	}
	public class LastWorkDayEndParameter : PredefinedParameter {
		public LastWorkDayEndParameter() : base(PredefinedParameters.LastWorkDayEnd, typeof(DateTime)) { }
		public override object CurrentValue {
			get { return DateTime.Today.Subtract(new TimeSpan(1)); }
		}
	}
	public class LastWorkWeekStartParameter : PredefinedParameter {
		public LastWorkWeekStartParameter() : base(PredefinedParameters.LastWorkWeekStart, typeof(DateTime)) { }
		public override object CurrentValue {
			get {
				return
					DateTime.Today.Subtract(new TimeSpan(7 + (int)DateTime.Today.DayOfWeek - 1, 0, 0, 0));
			}
		}
	}
	public class LastWorkWeekEndParameter : PredefinedParameter {
		public LastWorkWeekEndParameter() : base(PredefinedParameters.LastWorkWeekEnd, typeof(DateTime)) { }
		public override object CurrentValue {
			get {
				return
					DateTime.Today.Subtract(new TimeSpan((int)DateTime.Today.DayOfWeek - 1, 0, 0, 0, 1));
			}
		}
	}
	public class CurrentDatePlus1DayParameter : PredefinedParameter {
		public CurrentDatePlus1DayParameter() : base(PredefinedParameters.CurrentDatePlus1Day, typeof(DateTime)) { }
		public override object CurrentValue {  get { return DateTime.Today.AddDays(1); } }
		public override FunctionOperatorType DefaultOperatorType {  get {  return FunctionOperatorType.IsOutlookIntervalTomorrow;  } }
	}
	public class CurrentDatePlus2DaysParameter : PredefinedParameter {
		public CurrentDatePlus2DaysParameter() : base(PredefinedParameters.CurrentDatePlus2Days, typeof(DateTime)) { }
		public override object CurrentValue {
			get { return DateTime.Today.AddDays(2); }
		}
	}
	public class CurrentDatePlus3DaysParameter : PredefinedParameter {
		public CurrentDatePlus3DaysParameter() : base(PredefinedParameters.CurrentDatePlus3Days, typeof(DateTime)) { }
		public override object CurrentValue {
			get { return DateTime.Today.AddDays(3); }
		}
	}
	public class WeekAgoParameter : PredefinedParameter {
		public WeekAgoParameter() : base(PredefinedParameters.WeekAgo, typeof(DateTime)) { }
		public override object CurrentValue {
			get { return DateTime.Today.AddDays(-7); }
		}
	}
}
