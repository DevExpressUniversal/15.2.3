#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
namespace DevExpress.DashboardCommon.Native {
	public abstract class FilterAgentBase : IDisposable {
		readonly FilterCombinator inputFilter;
		readonly int priority;
		IFilterLevel level;
		IFilter levelListener;
		protected abstract IFilter OutputFilter { get; }
		public IFilter InputFilter { get { return inputFilter; } }
		protected FilterAgentBase(int priority) {
			this.inputFilter = new FilterCombinator();
			this.priority = priority;
		}
		public IFilterLevel Level {
			get { return level; }
			set {
				if (level != value) {
					inputFilter.BeginUpdate();
					try {
						if (level != null) {
							level.UnregisterElement(OutputFilter);
							SetLevelListener(null);
						}
						level = value;
						if (level != null)
							SetLevelListener(level.RegisterElement(OutputFilter, priority));
					} finally {
						inputFilter.EndUpdate();
					}
				}
			}
		}
		public abstract void Changed();
		public abstract IEnumerable<IMasterFilterItem> GetAffected();
		void SetLevelListener(IFilter value) {
			if (levelListener == value)
				return;
			levelListener = value;
			UpdateInputFilterCombinator();
		}
		void UpdateInputFilterCombinator() {
			inputFilter.UpdateElements(Enumerable
				.Empty<IFilter>()
				.AppendNotNull(levelListener));
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing)
				Level = null;
		}
	}
	public class FilterAgent : FilterAgentBase {
		readonly FilterIMasterFilter outputFilter;
		protected override IFilter OutputFilter { get { return outputFilter; } }
		public FilterAgent(IMasterFilter masterFilter, int priority)
			: base(priority) {
			this.outputFilter = new FilterIMasterFilter(masterFilter);
		}
		public override void Changed() {
			outputFilter.Changed();
		}
		public override IEnumerable<IMasterFilterItem> GetAffected() {
			return outputFilter.GetAffected();
		}
	}
	public class GroupFilterAgent : FilterAgentBase {
		readonly IFilterGroup group;
		protected override IFilter OutputFilter { get { return group; } }
		public GroupFilterAgent(IFilterGroup group, int priority)
			: base(priority) {
			this.group = group;
			this.group.InputFilter = InputFilter;
		}
		public override void Changed() {
			throw new NotSupportedException();
		}
		public override IEnumerable<IMasterFilterItem> GetAffected() {
			throw new NotSupportedException();
		}
	}
}
