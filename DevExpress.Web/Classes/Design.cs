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

using System.Collections;
namespace DevExpress.Web.Design {
	public interface IDesignTimeCollectionItem {
		string FieldName { get; set; }
		string Caption { get; }
		bool Visible { get; set; }
		bool ReadOnly { get; }
		int VisibleIndex { get; set; }
		PropertiesBase EditorProperties { get; }
		IDesignTimeCollectionItem Parent { get; }
		IList Items { get; }
		string[] GetHiddenPropertyNames();
		void Assign(IDesignTimeCollectionItem item);
	}
	public interface IDesignTimePropertiesOwner {
		object Owner { get; }
	}
	public interface IControlDesigner {
		string DesignerType { get; }
	}
	public static class DesignTimeItemsHelper {
		public static IDesignTimeCollectionItem FindParentBand(CollectionItem item) {
			if(item.Collection == null)
				return null;
			var group = item.Collection.Owner as IDesignTimeCollectionItem;
			var designTimeOwner = group == null ? item.Collection.Owner as IDesignTimePropertiesOwner : null;
			while(designTimeOwner != null) {
				group = designTimeOwner.Owner as IDesignTimeCollectionItem;
				designTimeOwner = group == null ? designTimeOwner.Owner as IDesignTimePropertiesOwner : null;
			}
			return group;
		}
	}
}
