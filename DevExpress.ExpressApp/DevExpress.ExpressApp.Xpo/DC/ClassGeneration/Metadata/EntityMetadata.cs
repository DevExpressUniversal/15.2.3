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
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	sealed class EntityMetadata : ClassMetadata {
		private readonly IList<InterfaceMetadata> ownImplementedInterfaces;
		internal EntityMetadata() {
			ownImplementedInterfaces = new List<InterfaceMetadata>();
			NeedInitializeKeyProperty = true;
		}
		internal Type BaseClass { get; set; }
		internal EntityMetadata BaseEntity { get; set; }
		internal bool NeedInitializeKeyProperty { get; set; }
		internal IList<InterfaceMetadata> OwnImplementedInterfaces { get { return ownImplementedInterfaces; } }
		internal override InterfaceMetadata PrimaryInterface { get { return ownImplementedInterfaces[0]; } }
		internal DataMetadata[] GetSortedAggregatedData() {
			List<DataMetadata> list = new List<DataMetadata>();
			Queue<DataMetadata> queue = new Queue<DataMetadata>(AggregatedData);
			while(queue.Count > 0) {
				DataMetadata data = queue.Dequeue();
				bool skip = false;
				foreach(DataMetadata aggregatedData in data.AggregatedData) {
					if(queue.Contains(aggregatedData)) {
						skip = true;
						break;
					}
				}
				if(skip) {
					queue.Enqueue(data);
				}
				else {
					list.Add(data);
				}
			}
			return list.ToArray();
		}
		public override string ToString() {
			return string.Format("EntityMetadata: {0}", Name);
		}
	}
}
