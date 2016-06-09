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
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using System.ComponentModel;
using System.Drawing;
using System.Activities;
using DevExpress.Data.Filtering;
namespace DevExpress.Workflow.Activities {
	[DevExpress.Utils.ToolboxTabName(ActivitiesAssemblyInfo.DXActivitiesTabName)]
	[ToolboxBitmap(typeof(ObjectSpaceTransactionScope), "Images.GetObjectByKey.bmp")]
	public class GetObjectByKey<T> : ObjectSpaceActivityBase<T> where T : class {
		protected override void Execute(NativeActivityContext context) {
			IObjectSpace os = GetObjectSpace(context);
			Result.Set(context, os.GetObjectByKey<T>(Key.Get<object>(context)));
		}
		[RequiredArgument]
		public InArgument<object> Key { get; set; }
	}
	[DevExpress.Utils.ToolboxTabName(ActivitiesAssemblyInfo.DXActivitiesTabName)]
	[ToolboxBitmap(typeof(ObjectSpaceTransactionScope), "Images.FindObjectByCriteria.bmp")]
	public class FindObjectByCriteria<T> : ObjectSpaceActivityBase<T> where T : class {
		protected override void Execute(NativeActivityContext context) {
			IObjectSpace os = GetObjectSpace(context);
			Result.Set(context, os.FindObject<T>(CriteriaOperator.Parse(Criteria.Get<string>(context)), true));
		}
		public InArgument<string> Criteria { get; set; }
	}
	[DevExpress.Utils.ToolboxTabName(ActivitiesAssemblyInfo.DXActivitiesTabName)]
	[ToolboxBitmap(typeof(ObjectSpaceTransactionScope), "Images.FindObjectByCriteria.bmp")]
	public class GetObjectsByCriteria<T> : ObjectSpaceActivityBase<IList<T>> where T : class {
		protected override void Execute(NativeActivityContext context) {
			IObjectSpace os = GetObjectSpace(context);
			Result.Set(context, os.GetObjects<T>(CriteriaOperator.Parse(Criteria.Get<string>(context))));
		}
		public InArgument<string> Criteria { get; set; }
	}
}
