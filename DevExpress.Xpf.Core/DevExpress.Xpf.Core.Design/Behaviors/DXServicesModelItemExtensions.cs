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

using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
namespace DevExpress.Xpf.Core.Design.Services {
	static class DXServicesModelItemExtensions {
		public static PropertyIdentifier BehaviorsProperty;
		public static DXPropertyIdentifier DXBehaviorsProperty;
		static DXServicesModelItemExtensions() {
			BehaviorsProperty = new PropertyIdentifier(typeof(Interaction), "Behaviors");
			DXBehaviorsProperty = new DXPropertyIdentifier(typeof(Interaction), "Behaviors");
		}
		public static ModelItemCollection GetBehaviorsCollection(this ModelItem item) {
			return item.Properties[BehaviorsProperty].Collection;
		}
		public static IModelItemCollection GetBehaviorsCollection(this IModelItem item) {
			return item.Properties[DXBehaviorsProperty].Collection;
		}
	}
}
