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

extern alias Platform;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using DevExpress.Xpf.Core.Design;
using Platform::DevExpress.Xpf.Ribbon;
namespace DevExpress.Xpf.Ribbon.Design {
	class BackstageViewDefaultInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item, EditingContext context) {
			base.InitializeDefaults(item, context);
			ModelItem button = ModelFactory.CreateItem(context, typeof(BackstageButtonItem), CreateOptions.InitializeDefaults);
			button.ResetLayout();
			ModelItem separator = ModelFactory.CreateItem(context, typeof(BackstageSeparatorItem), CreateOptions.InitializeDefaults);
			separator.ResetLayout();
			ModelItem tab = ModelFactory.CreateItem(context, typeof(BackstageTabItem), CreateOptions.InitializeDefaults);
			tab.ResetLayout();
			item.Properties["Items"].Collection.Add(button);
			item.Properties["Items"].Collection.Add(separator);
			item.Properties["Items"].Collection.Add(tab);
		}
	}
	class BackstageItemInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item) {
			base.InitializeDefaults(item);
			item.Properties["Content"].SetValue(item.Name);
		}
	}
}
