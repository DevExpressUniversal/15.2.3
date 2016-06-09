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

using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Mvvm.Native;
using System;
using System.Windows.Controls;
namespace DevExpress.Mvvm.UI.Native.ViewGenerator {
	public sealed class LayoutGroupGenerator : IGroupGenerator {
		readonly IModelItem group;
		readonly EditorsGeneratorBase itemGenerator;
		readonly Func<IModelItem, IModelItem> createChildGroupCallback;
		readonly Action<IModelItem> onAfterGenerateContentCallback;
		readonly Func<IModelItem, EditorsGeneratorBase> createGeneratorCallback;
		public LayoutGroupGenerator(IModelItem group, Func<IModelItem, IModelItem> createChildGroupCallback, Action<IModelItem> onAfterGenerateContentCallback, Func<IModelItem, EditorsGeneratorBase> createGeneratorCallback) {
			this.group = group;
			this.createChildGroupCallback = createChildGroupCallback;
			this.onAfterGenerateContentCallback = onAfterGenerateContentCallback;
			this.createGeneratorCallback = createGeneratorCallback;
			this.itemGenerator = createGeneratorCallback(group);
		}
		void IGroupGenerator.ApplyGroupInfo(string name, GroupView view, Orientation orientation) {
			IModelProperty viewProperty = group.Properties["View"];
			viewProperty.SetValueIfNotSet(Enum.ToObject(viewProperty.PropertyType, view));
			group.Properties["Orientation"].SetValueIfNotSet(orientation);
			group.Properties["Header"].SetValueIfNotSet(name);
		}
		IGroupGenerator IGroupGenerator.CreateNestedGroup(string name, GroupView view, Orientation orientation) {
			IModelItem childGroup = createChildGroupCallback(group);
			group.Properties["Children"].Collection.Add(childGroup);
			return new LayoutGroupGenerator(childGroup, createChildGroupCallback, onAfterGenerateContentCallback, createGeneratorCallback);
		}
		void AddChild(IModelItem childGroup) {
			group.Properties["Children"].Collection.Add(childGroup);
		}
		void IGroupGenerator.OnAfterGenerateContent() {
			onAfterGenerateContentCallback(group);
		}
		EditorsGeneratorBase IGroupGenerator.EditorsGenerator {
			get { return itemGenerator; }
		}
	}
}
