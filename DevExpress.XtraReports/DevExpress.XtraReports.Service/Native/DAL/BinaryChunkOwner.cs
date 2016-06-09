﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.Xpo;
namespace DevExpress.XtraReports.Service.Native.DAL {
	public class BinaryChunkOwner : ExtendedXPObject {
		internal const string BinaryChunkToBinaryChunkOwnerAssociationName = "BinaryChunk-BinaryChunkOwner";
		public static XPCollection<BinaryChunkOwner> FindByKeys(IEnumerable<string> keys, Session session) {
			var outdateCriteria = XpoHelper.CreateInOperator<BinaryChunkOwner>(d => d.ExternalKey, keys);
			return new XPCollection<BinaryChunkOwner>(session, outdateCriteria);
		}
		DateTime lastModifiedTime;
		string externalKey;
		XPCollection<BinaryChunk> chunks;
		public DateTime LastModifiedTime {
			get { return lastModifiedTime; }
			set { SetPropertyValue(() => LastModifiedTime, ref lastModifiedTime, value); }
		}
		[Indexed(Unique = true)]
		public string ExternalKey {
			get { return externalKey; }
			set { SetPropertyValue(() => ExternalKey, ref externalKey, value); }
		}
		[Aggregated]
		[Association(BinaryChunkToBinaryChunkOwnerAssociationName)]
		public XPCollection<BinaryChunk> Chunks {
			get {
				if(chunks == null) {
					chunks = GetCollection(() => Chunks);
					chunks.SortBy(c => c.Oid);
				}
				return chunks;
			}
		}
		public BinaryChunkOwner(Session session)
			: base(session) {
		}
	}
}
