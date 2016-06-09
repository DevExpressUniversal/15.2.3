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
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public interface IHierarchicalPathProvider {
		HierarchicalPath GetItemPath(object itemValue);
	}
	public class BindingDataHierarchicalPathProvider : IHierarchicalPathProvider {
		const string NullSegment = "NullSegment";
		public HierarchicalPath GetItemPath(object itemValue) {
			if(itemValue == null) {
				return new HierarchicalPath(NullSegment.Yield());
			}
			BindingData bindingData = (BindingData)itemValue;
			if(bindingData.Source is Parameter) {
				return new HierarchicalPath(new object[] { NullSegment, bindingData.Source });
			}
			List<object> segments = new List<object>();
			segments.Add(bindingData.Source);
			segments.AddRange(bindingData.Member.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries));
			return new HierarchicalPath(segments);
		}
	}
	public class ReportExplorerHierarchicalPathProvider : IHierarchicalPathProvider {
		public HierarchicalPath GetItemPath(object itemValue) {
			List<object> path = new List<object>();
			XRControl parent = (XRControl)itemValue;
			do {
				string name = parent.Return(x => x.Name, () => null);
				if(name != null) {
					parent = parent.Parent;
					path.Add(name);
				}
			}
			while(parent != null);
			path.Reverse();
			return new HierarchicalPath(path);
		}
	}
}
