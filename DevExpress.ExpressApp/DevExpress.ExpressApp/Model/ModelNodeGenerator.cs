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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Model {
	public abstract class ModelNodesGeneratorBase  {
		private static object LockUpdaterListObject = new object(); 
		List<IModelNodesGeneratorUpdater> updaters = new List<IModelNodesGeneratorUpdater>();
		public void GenerateNodes(ModelNode node) {
			ModelNodesDefaultInterfaceGenerator.Instance.GenerateNodesCore(node);
			if(this.GetType() != typeof(ModelNodesDefaultInterfaceGenerator)) {
				GenerateNodesCore(node);
			}
			RunUpdaters(node);
		}
		protected List<IModelNodesGeneratorUpdater> Updaters { get { return updaters; } }
		protected abstract void GenerateNodesCore(ModelNode node);
		protected internal void AddUpdater(IModelNodesGeneratorUpdater updater) {
			if(!Updaters.Contains(updater)) {
				Updaters.Add(updater);
			}
		}
		internal void AddUpdaters(ReadOnlyCollection<IModelNodesGeneratorUpdater> updaters) {
			lock(TypesInfo.lockObject) {
				foreach(IModelNodesGeneratorUpdater updater in updaters) {
					if(!Updaters.Contains(updater)) {
						Updaters.Add(updater);
					}
				}
			}
		}
		internal void ClearUpdaters() {
			lock(TypesInfo.lockObject) {
				Updaters.Clear();
			}
		}
		void RunUpdaters(ModelNode node) {
			lock(TypesInfo.lockObject) {
				foreach(IModelNodesGeneratorUpdater updater in Updaters) {
					updater.UpdateNode(node);
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsBinaryImage(IMemberInfo memberInfo) {
			return memberInfo != null && (memberInfo.FindAttribute<ImageEditorAttribute>() != null && memberInfo.MemberType == typeof(byte[]));
		}
	}
}
