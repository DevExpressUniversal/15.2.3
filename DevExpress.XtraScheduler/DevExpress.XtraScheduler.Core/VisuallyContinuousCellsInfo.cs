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
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler.Drawing {
	#region VisuallyContinuousCellsInfoCollection
	public class VisuallyContinuousCellsInfoCollection : List<IVisuallyContinuousCellsInfoCore> {
	}
	#endregion
	#region ResourceVisuallyContinuousCellsInfos
	public class ResourceVisuallyContinuousCellsInfos {
		#region Fields
		Resource resource;
		VisuallyContinuousCellsInfoCollection cellsInfoCollection;
		#endregion
		public ResourceVisuallyContinuousCellsInfos(Resource resource) {
			if (resource == null)
				Exceptions.ThrowArgumentException("resource", resource);
			this.resource = resource;
			this.cellsInfoCollection = new VisuallyContinuousCellsInfoCollection();
		}
		#region Properties
		public Resource Resource { get { return resource; } }
		public VisuallyContinuousCellsInfoCollection CellsInfoCollection { get { return cellsInfoCollection; } }
		#endregion
	}
	#endregion
	#region ResourceVisuallyContinuousCellsInfosCollection
	public class ResourceVisuallyContinuousCellsInfosCollection : List<ResourceVisuallyContinuousCellsInfos> {
		protected internal virtual VisuallyContinuousCellsInfoCollection MergeCellInfos() {
			VisuallyContinuousCellsInfoCollection result = new VisuallyContinuousCellsInfoCollection();
			for (int i = 0; i < Count; i++)
				result.AddRange(this[i].CellsInfoCollection);
			return result;
		}
	}
	#endregion
	#region IVisuallyContinuousCellsInfoCore
	public interface IVisuallyContinuousCellsInfoCore {
		TimeInterval Interval { get; }
		TimeInterval VisibleInterval { get; }
		Resource Resource { get; }
		int Count { get; }
		int VisibleCellsCount { get; }
		IAppointmentViewInfoContainer<IAppointmentViewInfoCollection> ScrollContainer { get; }
		TimeInterval GetIntervalByIndex(int index);
		int GetIndexByStartDate(DateTime date);
		int GetIndexByEndDate(DateTime date);
		int GetNextCellIndexByStartDate(DateTime date);
		int GetPreviousCellIndexByEndDate(DateTime date);
		int GetMaxCellHeight();
	}
	#endregion
}
