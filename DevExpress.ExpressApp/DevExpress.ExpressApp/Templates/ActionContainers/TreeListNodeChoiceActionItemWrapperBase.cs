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

using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Templates.ActionContainers {
	public abstract class TreeListNodeChoiceActionItemWrapperBase : ChoiceActionItemWrapper {
		private string defaultLeafImageName;
		private string defaultParentImageName;
		protected abstract void SetImageInfo(ImageInfo imageInfo);
		public TreeListNodeChoiceActionItemWrapperBase(ChoiceActionItem item, ChoiceActionBase action) : this(item, action, null, null) { }
		public TreeListNodeChoiceActionItemWrapperBase(ChoiceActionItem item, ChoiceActionBase action, string defaultParentImageName, string defaultLeafImageName) : base(item, action) {
			this.defaultParentImageName = defaultParentImageName;
			this.defaultLeafImageName = defaultLeafImageName;
		}
		public override void SetData(object data) {
		}
		public override void SetShortcut(string shortcutString) {
		}
		public override void SetImageName(string imageName) {
			ImageInfo imageInfo = ImageLoader.Instance.GetImageInfo(imageName);
			if(imageInfo.IsEmpty) {
				imageInfo = ImageLoader.Instance.GetImageInfo(ActionItem.Items.Count == 0 ? defaultLeafImageName : defaultParentImageName);
			}
			if(!imageInfo.IsEmpty) {
				SetImageInfo(imageInfo);
			}
		}
	}
}
