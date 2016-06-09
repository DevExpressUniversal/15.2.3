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

using DevExpress.XtraVerticalGrid.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.PropertyGrid.Internal {
	public class UpdateHandlesOperation : DescriptorContextOperation {
		List<RowHandle> invalidatedHandles = new List<RowHandle>();
		public IList<DescriptorContext> contextList = new List<DescriptorContext>();
		public IList<RowHandle> handleList = new List<RowHandle>();
		public List<RowHandle> InvalidatedHandles { get { return invalidatedHandles; } }
		Dictionary<string, bool?> expandedState = new Dictionary<string, bool?>();
		public override void Execute(DescriptorContext context) {
			if (context == null)
				return;
			AddContext(context);
			if (!context.IsValid) {
				InvalidatedHandles.Add(context.RowHandle);
			}
		}
		private void AddContext(DescriptorContext context) {
			contextList.Add(context);
			handleList.Add(context.RowHandle);
		}
		public override bool NeedsVisitChildren(DescriptorContext context) {
			return context != null && context.IsValid && base.NeedsVisitChildren(context);
		}
	}
	public class CollectExpandedStateOperation : DescriptorContextOperation {
		public List<KeyValuePair<string, bool?>> ExpandedState { get; private set; }
		public CollectExpandedStateOperation() {
			ExpandedState = new List<KeyValuePair<string, bool?>>();
		}
		public override void Execute(DescriptorContext context) {
			if (context == null || !context.IsLoaded || !context.IsExpanded)
				return;
			ExpandedState.Add(new KeyValuePair<string,bool?>(context.FieldName, true));
		}
		public override bool NeedsVisitChildren(DescriptorContext context) {
			return context != null && context.IsLoaded && context.IsExpanded && base.NeedsVisitChildren(context);
		}
	}
}
