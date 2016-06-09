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
using System.Activities;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.Workflow.Activities {
	public abstract class ObjectSpaceActivityBase<T> : NativeActivity<T> where T : class {
		protected IObjectSpace GetObjectSpace(NativeActivityContext context) {
			IObjectSpace os = this.ObjectSpace.Get(context);
			if(os == null) {
				os = (IObjectSpace)context.Properties.Find(ObjectSpaceTransactionScope.ObjectSpacePropertyName);
				Guard.ArgumentNotNull(os, "IObjectSpace");
			}
			return os;
		}
		public ObjectSpaceActivityBase() {
#if !DebugTest
			this.Constraints.Add(DevExpress.Workflow.Utils.ConstraintHelper.VerifyParent<NoPersistScope>(this));
#endif
		}
		[DefaultValue(null)]
		public InArgument<IObjectSpace> ObjectSpace { get; set; }
	}
	public abstract class ObjectSpaceActivityBase : NativeActivity {
		protected IObjectSpace GetObjectSpace(NativeActivityContext context) {
			IObjectSpace os = this.ObjectSpace.Get(context);
			if(os == null) {
				os = (IObjectSpace)context.Properties.Find(ObjectSpaceTransactionScope.ObjectSpacePropertyName);
				Guard.ArgumentNotNull(os, "IObjectSpace");
			}
			return os;
		}
		public ObjectSpaceActivityBase() {
#if !DebugTest
			this.Constraints.Add(DevExpress.Workflow.Utils.ConstraintHelper.VerifyParent<NoPersistScope>(this));
#endif
		}
		[DefaultValue(null)]
		public InArgument<IObjectSpace> ObjectSpace { get; set; }
	}
}
