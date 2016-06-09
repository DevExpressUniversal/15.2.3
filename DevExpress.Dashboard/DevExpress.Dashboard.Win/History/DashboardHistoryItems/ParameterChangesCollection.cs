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

using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	public class ParameterChangeInfo {
		public DashboardParameter OldParameter { get; set; }
		public DashboardParameter NewParameter { get; set; }
		public ParameterChangeInfo(DashboardParameter oldParameter, DashboardParameter newParameter) {
			this.OldParameter = oldParameter;
			this.NewParameter = newParameter;
		}
	}
	public class ParameterChangesCollection : List<ParameterChangeInfo> {
		public ParameterChangesCollection() { }
		public ParameterChangesCollection(IEnumerable<DashboardParameter> parameters) {
			this.AddRange(parameters.Select(p => new ParameterChangeInfo((DashboardParameter)p.Clone(), p)));
		}
		public void ApplyChanges(IEnumerable<DashboardParameter> parameters) {
			foreach(ParameterChangeInfo pair in this)
				if(!parameters.Contains(pair.NewParameter))
					pair.NewParameter = null;
			List<DashboardParameter> added = parameters.Where(p => !this.Any(pair => pair.NewParameter == p)).ToList();
			foreach(ParameterChangeInfo pair in this)
				if(pair.OldParameter != null && pair.NewParameter == null)
					pair.NewParameter = added.FirstOrDefault(p => p.Name == pair.OldParameter.Name && p.Type == pair.OldParameter.Type);
			foreach(DashboardParameter newParameter in parameters)
				if(!this.Any(pair => pair.NewParameter == newParameter))
					this.Add(new ParameterChangeInfo(null, newParameter));
		}
		public void ResetChanges() {
			foreach(ParameterChangeInfo pair in this)
				pair.OldParameter = (DashboardParameter)pair.NewParameter.Clone();
		}
		public IEnumerable<DashboardParameter> ParametersAdded {
			get { return this.Where(p => p.NewParameter != null && p.OldParameter == null).Select(p => p.NewParameter); }
		}
		public IEnumerable<DashboardParameter> ParametersRemoved {
			get { return this.Where(p => p.OldParameter != null && p.NewParameter == null).Select(p => p.OldParameter); }
		}
		public IEnumerable<DashboardParameter> ParametersChanged {
			get { return this.Where(p => p.OldParameter != null && p.NewParameter != null && !new ParametersEqualityComparer().Equals(p.NewParameter, p.OldParameter)).Select(p => p.NewParameter); }
		}
		public IEnumerable<ParameterChangeInfo> ParametersRenamed {
			get { return this.Where(p => p.OldParameter != null && p.NewParameter != null && p.NewParameter.Name != p.OldParameter.Name); }
		}
	}
}
