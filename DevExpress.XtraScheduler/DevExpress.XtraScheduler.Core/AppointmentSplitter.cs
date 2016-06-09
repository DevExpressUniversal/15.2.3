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
using System.Text;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler {
	#region ChangeOccurrenceSplitMode
	public enum ChangeOccurrenceSplitMode {
		Always,
		Never,
		Auto
	}
	#endregion
	#region AppointmentSimpleSplitter (abstract class)
	public abstract class AppointmentSimpleSplitter {
		AppointmentSplitter owner;
		protected AppointmentSimpleSplitter(AppointmentSplitter owner) {
			if (owner == null)
				Exceptions.ThrowArgumentNullException("owner");
			this.owner = owner;
		}
		public AppointmentSplitter Owner { get { return owner; } }
		protected internal virtual void CalculateNewAppointmentNewStart(Appointment apt, Appointment newApt){
			newApt.Start = apt.End;
		}
		#region Split
		public virtual Appointment Split(Appointment apt, DateTime splitTime) {			
			TimeSpan primaryAppointmentDuration = apt.Duration;
			if (ShouldTurnOffAllDay(apt, splitTime))
				TurnOffAllDay(apt);
			apt.End = CalculateAppointmentNewEnd(apt, splitTime);
			Appointment newAppointment = CreateAppointmentCopy(apt);
			CalculateNewAppointmentNewStart(apt, newAppointment);
			newAppointment.Duration = primaryAppointmentDuration - apt.Duration;
			owner.RaiseSplitted(apt, newAppointment);
			return newAppointment;
		}
		#endregion
		protected internal virtual void CreateDeletedOccurrenceIntoPattern(int recurrenceIndex) {
		}
		protected internal virtual bool ShouldTurnOffAllDay(Appointment apt, DateTime splitTime) {
			return apt.AllDay && splitTime.TimeOfDay != TimeSpan.Zero;
		}
		#region TurnOffAllDay
		protected internal virtual void TurnOffAllDay(Appointment apt) {
			DateTime actualStart = apt.Start;
			TimeSpan actualDuration = apt.Duration;
			apt.AllDay = false;
			apt.Start = actualStart;
			apt.Duration = actualDuration;
		}
		#endregion
		protected internal virtual DateTime CalculateAppointmentNewEnd(Appointment apt, DateTime splitTime) {
			return splitTime;
		}
		protected internal virtual Appointment CreateAppointmentCopy(Appointment apt) {
			return apt.Copy();
		}
	}
	#endregion
	#region NormalAppointmentSimpleSplitter
	public class NormalAppointmentSimpleSplitter : AppointmentSimpleSplitter {
		public NormalAppointmentSimpleSplitter(AppointmentSplitter owner)
			: base(owner) {
		}
	}
	#endregion
	#region PatternAppointmentSimpleSplitter
	public class PatternAppointmentSimpleSplitter : AppointmentSimpleSplitter {
		public PatternAppointmentSimpleSplitter(AppointmentSplitter owner)
			: base(owner) {
		}
		protected internal override void CalculateNewAppointmentNewStart(Appointment apt, Appointment newApt) {
			newApt.Start = apt.End;
			newApt.RecurrenceInfo.Start = apt.End;
		}
	}
	#endregion
	#region OccurrenceAppointmentSimpleSplitter
	public class OccurrenceAppointmentSimpleSplitter : AppointmentSimpleSplitter {
		public OccurrenceAppointmentSimpleSplitter(AppointmentSplitter owner)
			: base(owner) {
		}
	}
	#endregion
	#region ChangedOccurrenceSplitterBase
	public class ChangedOccurrenceSplitterBase : AppointmentSimpleSplitter {
		public ChangedOccurrenceSplitterBase(AppointmentSplitter owner)
			: base(owner) {
		}
		protected internal override DateTime CalculateAppointmentNewEnd(Appointment apt, DateTime splitTime) {
			return apt.Start.Date + splitTime.TimeOfDay;
		}
	}
	#endregion
	#region ChangedOccurrenceSplitterIntoChangedAndNormal
	public class ChangedOccurrenceSplitterIntoChangedAndNormal : ChangedOccurrenceSplitterBase {
		public ChangedOccurrenceSplitterIntoChangedAndNormal(AppointmentSplitter owner)
			: base(owner) {
		}
	}
	#endregion
	#region ChangedOccurrenceSplitterIntoChangedAndChanged
	public class ChangedOccurrenceSplitterIntoChangedAndChanged : ChangedOccurrenceSplitterBase {
		Appointment patternCopy;
		public ChangedOccurrenceSplitterIntoChangedAndChanged(AppointmentSplitter owner, Appointment patternCopy)
			: base(owner) {
			this.patternCopy = patternCopy;
		}
		#region CreateAppointmentCopy
		protected internal override Appointment CreateAppointmentCopy(Appointment apt) {
			AppointmentCopyHelper copyHelper = new AppointmentCopyHelper();
			Appointment result = patternCopy.CreateException(AppointmentType.ChangedOccurrence, apt.RecurrenceIndex);
			copyHelper.Assign(apt, result);
			return result;
		}
		#endregion
		protected internal override void CreateDeletedOccurrenceIntoPattern(int recurrenceIndex) {
			patternCopy.CreateException(AppointmentType.DeletedOccurrence, recurrenceIndex);
		}
	}
	#endregion
	#region AppointmentSplitter
	public class AppointmentSplitter {
		#region Events
		AppointmentSplittedEventHandler onSplitted;
		public event AppointmentSplittedEventHandler Splitted { add { onSplitted += value; } remove { onSplitted -= value; } }
		protected internal virtual void RaiseSplitted(Appointment source, Appointment copy) {
			if (onSplitted != null) {
				AppointmentSplittedEventArgs args = new AppointmentSplittedEventArgs(source, copy);
				onSplitted(this, args);
			}
		}
		#endregion
		protected internal virtual bool CanSplitAppointmentCore(Appointment apt, DateTime splitTime) {
			return (apt.Start < splitTime) && (splitTime < apt.End);
		}
		protected internal virtual bool CanSplitChangedOccurrenceAppointment(Appointment apt, DateTime splitTime) {
			return (apt.Start.TimeOfDay < splitTime.TimeOfDay) && (splitTime.TimeOfDay < apt.End.TimeOfDay);
		}
		#region SplitPatternAppointmentCore
		protected internal Appointment SplitPatternAppointmentCore(Appointment apt, DateTime splitTime) {
			PatternAppointmentSimpleSplitter splitter = new PatternAppointmentSimpleSplitter(this);
			return splitter.Split(apt, splitTime);
		}
		#endregion
		#region SplitChangeOccurrenceInNeverMode
		private void SplitChangeOccurrenceInNeverMode(Appointment pattern, AppointmentBaseCollection result, bool splitPatteAppointment) {
			if (splitPatteAppointment) {
				AppointmentSimpleSplitter splitter = new ChangedOccurrenceSplitterIntoChangedAndChanged(this, result[1]);
				foreach (Appointment apt in pattern.GetExceptions())
					splitter.CreateDeletedOccurrenceIntoPattern(apt.RecurrenceIndex);
			}
		}
		#endregion
		#region SplitChangeOccurrenceInAutoMode
		private void SplitChangeOccurrenceInAutoMode(Appointment pattern, DateTime splitTime, AppointmentBaseCollection result, bool splitPatternAppointment) {
			if (splitPatternAppointment) {
				AppointmentSimpleSplitter splitter = new ChangedOccurrenceSplitterIntoChangedAndChanged(this, result[1]);
				SplitPatternExceptions(pattern, splitTime, splitter);
			}
		}
		#endregion
		#region SplitChangeOccurrenceInAllwaysMode
		private void SplitChangeOccurrenceInAllwaysMode(Appointment pattern, DateTime splitTime, AppointmentBaseCollection result, bool splitPatternAppointment) {
			if (splitPatternAppointment) {
				AppointmentSimpleSplitter splitter = new ChangedOccurrenceSplitterIntoChangedAndChanged(this, result[1]);
				SplitPatternExceptions(pattern, splitTime, splitter);
			}
			else {
				AppointmentSimpleSplitter splitter = new ChangedOccurrenceSplitterIntoChangedAndNormal(this);
				result.AddRange(SplitPatternExceptions(pattern, splitTime, splitter));
			}
		}
		#endregion
		#region SplitPatternAppointment
		protected internal AppointmentBaseCollection SplitPatternAppointment(Appointment pattern, DateTime splitTime, ChangeOccurrenceSplitMode changedOccurrenceSplitMode) {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			bool splitPatternAppointment = false;
			if (CanSplitAppointmentCore(pattern, splitTime)) {
				result.Add(pattern);
				result.Add(SplitPatternAppointmentCore(pattern, splitTime));
				splitPatternAppointment = true;
			}
			else
				result.Add(pattern);
			if (pattern.HasExceptions) {
				switch (changedOccurrenceSplitMode) {
					case ChangeOccurrenceSplitMode.Always:
						SplitChangeOccurrenceInAllwaysMode(pattern, splitTime, result, splitPatternAppointment);
						break;
					case ChangeOccurrenceSplitMode.Auto:
						SplitChangeOccurrenceInAutoMode(pattern, splitTime, result, splitPatternAppointment);
						break;
					case ChangeOccurrenceSplitMode.Never:
						SplitChangeOccurrenceInNeverMode(pattern, result, splitPatternAppointment);
						break;
				}
			}
			return result;
		}
		#endregion
		#region SplitPatternExceptions
		protected internal AppointmentBaseCollection SplitPatternExceptions(Appointment primaryPattern, DateTime splitTime, AppointmentSimpleSplitter splitter) {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			foreach (Appointment apt in primaryPattern.GetExceptions()) {
				if (apt.Type == AppointmentType.DeletedOccurrence || !CanSplitChangedOccurrenceAppointment(apt, splitTime))
					splitter.CreateDeletedOccurrenceIntoPattern(apt.RecurrenceIndex);
				else
					result.Add(SplitAppointmentCore(apt, splitTime, splitter)[1]);
			}
			return result;
		}
		#endregion
		#region SplitAppointmentCore
		protected internal AppointmentBaseCollection SplitAppointmentCore(Appointment appointment, DateTime splitTime, AppointmentSimpleSplitter splitter) {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			result.Add(appointment);
			result.Add(splitter.Split(appointment, splitTime));
			return result;
		}
		#endregion
		#region SplitAppointment
		public AppointmentBaseCollection SplitAppointment(Appointment apt, DateTime splitTime) {
			return SplitAppointment(apt, splitTime, ChangeOccurrenceSplitMode.Auto);
		}
		#endregion
		#region SplitAppointment
		public AppointmentBaseCollection SplitAppointment(Appointment apt, DateTime splitTime, ChangeOccurrenceSplitMode changedOccurrenceSplitMode) {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			if (apt.Type == AppointmentType.Pattern)
				return SplitPatternAppointment(apt, splitTime, changedOccurrenceSplitMode);
			if (!CanSplitAppointmentCore(apt, splitTime)) {
				result.Add(apt);
				return result;
			}
			if (apt.Type == AppointmentType.Normal) {
				AppointmentSimpleSplitter splitter = new NormalAppointmentSimpleSplitter(this);
				return SplitAppointmentCore(apt, splitTime, splitter);
			}
			else {
				if (apt.Type == AppointmentType.Occurrence) {
					AppointmentSimpleSplitter splitter = new OccurrenceAppointmentSimpleSplitter(this);
					return SplitAppointmentCore(apt, splitTime, splitter);
				}
				else {
					if (apt.Type == AppointmentType.ChangedOccurrence) {
						AppointmentSimpleSplitter splitter = new ChangedOccurrenceSplitterIntoChangedAndNormal(this);
						return SplitAppointmentCore(apt, splitTime, splitter);
					}
				}
				result.Add(apt);
				return result;
			}
		}
		#endregion
	}
	#endregion
}
