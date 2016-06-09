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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public static class XRObjectModelCollection<T>
		where T : XRObjectModelBase {
		class XRObjectModelCollectionImpl<TFactoryOwner, TFactorySource, TFactorySourceBase, TFactoryTargetBase> : HierarchyCollectionWithStorage<T, TFactorySource, XRModelBase>
			where TFactoryOwner : class
			where TFactorySource : TFactorySourceBase
			where TFactorySourceBase : class
			where TFactoryTargetBase : XRObjectModelBase {
			protected internal XRObjectModelCollectionImpl(XRModelBase owner, IList<TFactorySource> xrCollection, ModelFactory<TFactoryOwner, TFactorySourceBase, TFactoryTargetBase> factory) : base(xrCollection, x => (TFactorySource)x.XRObject, x => (T)(XRObjectModelBase)factory.GetModel(x), owner, (item, parent) => item.Parent = parent, (item, parent) => item.Parent = null) { }
		}
		public static ObservableCollection<T> Create<TFactoryOwner, TFactorySource, TFactorySourceBase, TFactoryTargetBase>(
				XRModelBase owner, IList<TFactorySource> xrCollection, ModelFactory<TFactoryOwner, TFactorySourceBase, TFactoryTargetBase> factory)
			where TFactoryOwner : class
			where TFactorySource : TFactorySourceBase
			where TFactorySourceBase : class
			where TFactoryTargetBase : XRObjectModelBase {
			return new XRObjectModelCollectionImpl<TFactoryOwner, TFactorySource, TFactorySourceBase, TFactoryTargetBase>(owner, xrCollection, factory);
		}
		public static ObservableCollection<T> Create<TFactoryOwner, TFactorySourceBase, TFactoryTargetBase>(
				XRModelBase owner, IList xrCollection, ModelFactory<TFactoryOwner, TFactorySourceBase, TFactoryTargetBase> factory)
			where TFactoryOwner : class
			where TFactorySourceBase : class
			where TFactoryTargetBase : XRObjectModelBase {
			return new XRObjectModelCollectionImpl<TFactoryOwner, TFactorySourceBase, TFactorySourceBase, TFactoryTargetBase>(owner, ListAdapter<TFactorySourceBase>.FromObjectList(xrCollection), factory);
		}
	}
}
