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

using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Reports.UserDesigner.Editors;
using DevExpress.Xpf.Reports.UserDesigner.FieldList;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.Mvvm.Native;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.Xpf.Reports.UserDesigner.GroupSort {
	public class GroupSortHierarchicalPathProviderBehavior : Behavior<PopupTreeListEditSettings>, IHierarchicalPathProvider {
		const string NullSegment = "NullSegment";
		public IEnumerable<FieldListNodeBase<XRDiagramControl>> DataSource {
			get {
				return AssociatedObject.Return(x =>
				(IEnumerable<FieldListNodeBase<XRDiagramControl>>)x.GetValue(PopupTreeListEditSettings.ItemsSourceProperty),
				() => null);
			}
		}
		FieldListNodeBase<XRDiagramControl> effectiveDataSource;
		internal FieldListNodeBase<XRDiagramControl> EffectiveDataSource {
			get { return DataSource.Return(x => x.FirstOrDefault(), () => effectiveDataSource); }
			set { effectiveDataSource = value; }
		}
		public HierarchicalPath GetItemPath(object itemValue) {
			if(itemValue == null) 
				return new HierarchicalPath(NullSegment.Yield());
			BindingData bindingData = (BindingData)itemValue;
			if(bindingData.Source == null)
				return new HierarchicalPath(NullSegment.Yield());
			if (bindingData.Source is Parameter) {
				return new HierarchicalPath(new object[] { NullSegment, bindingData.Source });
			}
			HashSet<object> segments = new HashSet<object>();
			segments.Add(EffectiveDataSource.DataMember);
			string fieldSegment = string.Empty;
			if (string.Equals(bindingData.Member, EffectiveDataSource.DataMember))
				fieldSegment = string.Empty;
			else if (bindingData.Member.StartsWith(EffectiveDataSource.DataMember))
				fieldSegment = bindingData.Member.Remove(0, EffectiveDataSource.DataMember.Length + 1);
			else
				fieldSegment = bindingData.Member;
			if (!string.IsNullOrEmpty(fieldSegment))
				fieldSegment.Split('.').ForEach(x => segments.Add(x));
			return new HierarchicalPath(segments);
		}
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.HierarchicalPathProvider = this;
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			AssociatedObject.HierarchicalPathProvider = null;
		}
	}
}
