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
using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPrinting {
	static class ProgressReflectorExtensions {
		public static void Do(this ProgressReflector me, int rangeMaximum, Action action) {
			me.InitializeRange(rangeMaximum);
			try {
				action();
			} finally {
				me.MaximizeRange();
			}
		}
	}
	public class ProgressReflector {
		#region static
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ProgressReflectorValue"),
#endif
		Obsolete("This property is now obsolete. You should use the PrintingSystemBase.ProgressReflector.RangeValue property instead.")
		]
		public static float Value {
			get { return 0; }
			set { }
		}
		[Obsolete("This method is now obsolete. You should use the PrintingSystemBase.ProgressReflector property instead. To see an updated example, refer to http://www.devexpress.com/example=E906.")]
		public static void RegisterReflector(ProgressReflector value) {
		}
		[Obsolete("This method is now obsolete. You should use the PrintingSystemBase.ProgressReflector property instead. To see an updated example, refer to http://www.devexpress.com/example=E906.")]
		public static void UnregisterReflector(ProgressReflector value) {
		}
		[Obsolete("This method is now obsolete. You should use the PrintingSystemBase.ProgressReflector.SetProgressRanges method instead.")]
		public static void SetRanges(float[] ranges) {
		}
		[Obsolete("This method is now obsolete. You should use the PrintingSystemBase.ProgressReflector.InitializeRange method instead.")]
		public static void Initialize(int maximum) {
		}
		[Obsolete("This method is now obsolete. You should use the PrintingSystemBase.ProgressReflector.MaximizeRange method instead.")]
		public static void MaximizeValue() {
		}
		[Obsolete("This method is now obsolete. You should use the PrintingSystemBase.ProgressReflector property instead.")]
		public static void DisableReflector() {
		}
		[Obsolete("This method is now obsolete. You should use the PrintingSystemBase.ProgressReflector property instead.")]
		public static void EnableReflector() {
		}
		#endregion
		ArrayList ranges = new ArrayList();
		int fPosition;
		protected int fMaximum = 100;
		ProgressReflectorLogic logic;
		int updates = 0;
		public ProgressReflector() {
		}
		protected bool Updating {
			get { return updates > 0; }
		}
		public ProgressReflectorLogic Logic {
			get {
				if(logic == null)
					logic = new ProgressReflectorLogic(this);
				return logic;
			}
			set {
				if(value == null)
					throw new NullReferenceException();
				logic = value;
			}
		}
		public int RangeCount {
			get { return Ranges.Count; }
		}
		protected internal ArrayList Ranges { get { return ranges; } }
		protected internal virtual int PositionCore {
			set { fPosition = value; }
			get { return fPosition; }
		}
		protected internal virtual int MaximumCore {
			set { fMaximum = value; }
			get { return fMaximum; }
		}
		public int Position { get { return PositionCore; } }
		public int Maximum { get { return MaximumCore; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("ProgressReflectorRangeValue")]
#endif
		public virtual float RangeValue {
			get { return Logic.RangeValue; }
			set { Logic.RangeValue = value; }
		}
		public bool CanAutocreateRange {
			get { return Logic.CanAutocreateRange; }
			set { Logic.CanAutocreateRange = value; }
		}
		#region events
		EventHandler onPositionChanged;
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("ProgressReflectorPositionChanged")]
#endif
		public event EventHandler PositionChanged {
			add { onPositionChanged = System.Delegate.Combine(onPositionChanged, value) as EventHandler; }
			remove { onPositionChanged = System.Delegate.Remove(onPositionChanged, value) as EventHandler; }
		}
		void RaisePositionChanged(ProgressReflector sender, EventArgs e) {
			if(onPositionChanged != null)
				onPositionChanged(sender, e);
		}
		#endregion
		public virtual void SetProgressRanges(float[] ranges) {
			if(ranges == null)
				throw new ArgumentException("ranges");
			Reset();
			float sum = CalcSum(ranges);
			for(int i = 0; i < ranges.Length; i++)
				this.ranges.Add(ranges[i] * MaximumCore / sum);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetProgressRanges(float[] ranges, ProgressReflectorLogic logic) {
			SetProgressRanges(ranges);
			this.logic = logic;
			Logic.SetProgressReflector(this);
		}
		internal void IncrementRangeValue(int value) {
			for(int i = 0; i < value; i++)
				RangeValue++;
		}
		internal virtual void Reset() {
			ranges.Clear();
			Logic.Reset();
			PositionCore = 0;
		}
		protected internal virtual void SetPosition(int value) {
			PositionCore = value;
			RaisePositionChanged(this, EventArgs.Empty);
		}
		protected virtual void MaximizeRangeCore() {
		}
		protected virtual void InitializeRangeCore(int maximum) {
		}
		float CalcSum(float[] values) {
			float sum = 0f;
			for(int i = 0; i < values.Length; i++)
				sum += values[i];
			return sum;
		}
		public virtual void InitializeRange(int maximum) {
			if(Ranges.Count == 0 && !CanAutocreateRange)
				return;
			Logic.InitializeRange(maximum);
			InitializeRangeCore(maximum);
		}
		public virtual void MaximizeRange() {
			if(Ranges.Count == 0 && !CanAutocreateRange)
				return;
			Logic.MaximizeRange();
			MaximizeRangeCore();
		}
		public void EnsureRangeDecrement(Action0 action) {
			int rangeCount = RangeCount;
			try {
				action();
			} finally {
				if(rangeCount == RangeCount)
					MaximizeRange();
			}
		}
		internal void FinalizeRangeCount() {
			BeginUpdate();
			try {
				while(RangeCount > 1) {
					MaximizeRange();
					if(RangeCount > 1)
						InitializeRange(1);
				}
			} finally {
				EndUpdate();
			}
		}
		protected virtual void BeginUpdate() {
			updates++;
		}
		protected virtual void EndUpdate() {
			updates--;
		}
	}
}
namespace DevExpress.XtraPrinting.Native {
		public class ProgressReflectorLogic {
			ProgressReflector progressReflector;
			protected bool shouldAutocreateRange = true;
		float scaleFactor = 1f;
		float rangeValue;
		float progressValue;
			public ProgressReflectorLogic(ProgressReflector progressReflector) {
				SetProgressReflector(progressReflector);
			}
			[EditorBrowsable(EditorBrowsableState.Never)]
			public void SetProgressReflector(ProgressReflector progressReflector) {
				if(progressReflector == null)
					throw new NullReferenceException();
				this.progressReflector = progressReflector;
			}
			public virtual float RangeValue {
			get { return rangeValue; }
				set {
				if((!CanAutocreateRange && Ranges.Count == 0) || rangeValue == value)
						return;
					int newPosition = (int)Math.Round(GetPositionFromValue(value));
				if(newPosition != Position || rangeValue == 0f)
						SetPosition(Math.Min(newPosition, Maximum));
				rangeValue = value;
				}
			}
			public virtual bool CanAutocreateRange {
				get { return this.shouldAutocreateRange; }
				set { this.shouldAutocreateRange = value; }
			}
			protected float ProgressValue {
			get { return progressValue; }
			set { progressValue = value; }
			}
			protected float ScaleFactor {
			get { return scaleFactor; }
			set { scaleFactor = value; }
			}
			protected ArrayList Ranges {
			get { return progressReflector.Ranges; }
			}
			protected int Maximum {
				get { return progressReflector.MaximumCore; }
			}
			protected int Position {
				get { return progressReflector.PositionCore; }
			}
			protected void SetPosition(int value) {
				progressReflector.SetPosition(value);
			}
			public virtual void InitializeRange(int maximum) {
				maximum = Math.Max(1, maximum);
				if(Ranges.Count > 0) {
				ProgressValue = GetPositionFromValue(RangeValue);
					ScaleFactor = (float)Ranges[0] / maximum;
			}
			else {
					ProgressValue = 0f;
					ScaleFactor = (float)Maximum / maximum;
				}
			rangeValue = 0f;
			}
			public virtual void MaximizeRange() {
				if(Ranges.Count > 0) {
				RangeValue = Math.Max(RangeValue, (float)Ranges[0] / ScaleFactor);
					Ranges.RemoveAt(0);
				}
				if(CanAutocreateRange && Ranges.Count == 0) {
				ProgressValue = rangeValue = 0f;
					if(Position != Maximum)
						SetPosition(Maximum);
				}
			}
			float GetPositionFromValue(float value) {
				return ProgressValue + value * ScaleFactor;
			}
		internal void Reset() {
			progressValue = rangeValue = 0f;
		}
	}
}
