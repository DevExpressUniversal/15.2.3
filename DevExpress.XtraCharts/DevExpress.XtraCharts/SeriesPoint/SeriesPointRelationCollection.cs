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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartCollectionSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SeriesPointRelationCollection : ChartCollectionBase, ICloneable {
		internal SeriesPoint ParentPoint { get { return Owner as SeriesPoint; } }
		internal Series Series { get { return ParentPoint != null ? ParentPoint.Series : null; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("SeriesPointRelationCollectionItem")]
#endif
		public Relation this[int index] { get { return (Relation)List[index]; } }
		internal SeriesPointRelationCollection(SeriesPoint parentPoint) : base() {
			base.Owner = parentPoint;
		}
		#region ICloneable implementation
		object ICloneable.Clone() {
			SeriesPointRelationCollection relations = new SeriesPointRelationCollection(null);
			relations.InnerList.AddRange(InnerList);
			relations.SetOwner(ParentPoint);
			return relations;
		}
		#endregion
		void SetOwner(SeriesPoint owner) {
			base.Owner = owner;
		}
		internal void TestPoint(SeriesPoint childPoint) {
			if (!(ParentPoint.Owner is Series))
				throw new InvalidOwnerException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectParentSeriesPointOwner));
			if (!((Series)ParentPoint.Owner).View.IsSupportedRelations)
				throw new InvalidOwnerException(ChartLocalizer.GetString(ChartStringId.MsgSeriesViewNotSupportRelations));
			if (childPoint == null)
				throw new ArgumentNullException("childPoint");
			if (!(childPoint.Owner is Series))
				throw new InvalidOwnerException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectChildSeriesPointOwner));
			if (childPoint.SeriesPointID < 0)
				throw new InvalidSeriesPointIDException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectChildSeriesPointID));
			if (ParentPoint.Owner != childPoint.Owner)
				throw new InvalidOwnerException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectSeriesOfParentAndChildPoints));
			if (childPoint.SeriesPointID == ParentPoint.SeriesPointID || childPoint.GetHashCode() == ParentPoint.GetHashCode())
				throw new InvalidSeriesPointIDException(ChartLocalizer.GetString(ChartStringId.MsgSelfRelatedSeriesPoint));
			foreach (Relation relation in this)
				if (relation.ChildPoint.SeriesPointID == childPoint.SeriesPointID)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgSeriesPointRelationAlreadyExists));
		}
		internal void ValidateIDs() {
			ArrayList relations = new ArrayList();
			foreach (Relation relation in this) {
				if (relation.ChildPointID < 0)
					throw new InvalidSeriesPointIDException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectChildSeriesPointID));
				else if (relation.ChildPointID == ParentPoint.SeriesPointID)
					throw new InvalidSeriesPointIDException(ChartLocalizer.GetString(ChartStringId.MsgSelfRelatedSeriesPoint));
				else if (Series.Points.GetByID(relation.ChildPointID) == null)
					throw new InvalidSeriesPointIDException(string.Format(ChartLocalizer.GetString(ChartStringId.MsgChildSeriesPointNotExist), relation.ChildPointID));
				else if (relations.Contains(relation))
					throw new InvalidSeriesPointIDException(ChartLocalizer.GetString(ChartStringId.MsgRelationChildPointIDNotUnique));
				relations.Add(relation);
			}
		}
		internal void ClearInternal() {
			InnerList.Clear();
		}
		public int Add(Relation relation) {
			if (ParentPoint.Owner != null && !ParentPoint.Owner.Loading)
				TestPoint(relation.ChildPoint);
			return base.Add(relation);
		}
		public int Add(TaskLink relation) {
			if (ParentPoint.Owner != null && !ParentPoint.Owner.Loading)
				TestPoint(relation.ChildPoint);
			return base.Add(relation);
		}
		public int Add(SeriesPoint childPoint, TaskLinkType linkType) {
			return Add(new TaskLink(childPoint, linkType));
		}
		public int Add(SeriesPoint childPoint) {
			return Add(new TaskLink(childPoint));
		}
		public void AddRange(Relation[] relations) {
			if (ParentPoint.Owner != null && !ParentPoint.Owner.Loading) {
				foreach (Relation relation in relations)
					TestPoint(relation.ChildPoint);
			}
			base.AddRange(relations);
		}
		public void Insert(int index, Relation relation) {
			if (ParentPoint.Owner != null && !ParentPoint.Owner.Loading)
				TestPoint(relation.ChildPoint);
			base.Insert(index, relation);
		}
		public void Remove(Relation relation) {
			base.Remove(relation);
		}
		public Relation GetByChildSeriesPoint(SeriesPoint point) {
			foreach (Relation relation in this)
				if (relation.ChildPoint.SeriesPointID == point.SeriesPointID)
					return relation;
			return null;
		}
		public void Remove(SeriesPoint childPoint) {
			Relation relation = GetByChildSeriesPoint(childPoint);
			if (relation != null)
				Remove(relation);
		}
		public bool Contains(SeriesPoint childPoint) {
			return List.Contains(childPoint.SeriesPointID);
		}
	}
}
