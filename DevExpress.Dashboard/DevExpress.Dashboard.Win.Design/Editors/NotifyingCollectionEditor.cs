#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DataAccess.Native;
using DevExpress.Utils.UI;
namespace DevExpress.DashboardWin.Design {
	public class NotifyingCollectionEditor : CollectionEditor {
		const string EmptyName = "<Empty>";
		static string GetTextWithSpaces(string text) {
			string result = string.Empty;
			foreach (char c in text)
				result += (Char.IsUpper(c) ? " " : string.Empty) + c;
			return result.Trim();
		}
		public NotifyingCollectionEditor(Type type)
			: base(type) {
		}
		protected override CollectionEditorFormBase CreateCollectionForm(IServiceProvider serviceProvider) {
			CollectionEditorFormBase form = base.CreateCollectionForm(serviceProvider);
			form.AllowGlyphSkinning = !BitmapStorage.UseColors;
			form.Text = GetTextWithSpaces(Context.PropertyDescriptor.DisplayName);
			return form;
		}
		protected override string GetItemName(object item, int index) {
			string itemName = string.Empty;
			GridColumnTotal total = item as GridColumnTotal;
			if(total != null)
				itemName = total.TotalType.ToString();
			else {
				IEditNameProvider provider = item as IEditNameProvider;
				if (provider != null)
					itemName = provider.DisplayName;
				else {
					INamedItem namedItem = item as INamedItem;
					if(namedItem != null)
						itemName = namedItem.Name;
				}
			}
			if(string.IsNullOrEmpty(itemName)) {
				return EmptyName;
			} else {
				return itemName;
			}
		}
	}
}
