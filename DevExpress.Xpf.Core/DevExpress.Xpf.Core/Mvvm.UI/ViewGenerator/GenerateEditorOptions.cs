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

using DevExpress.Data.Browsing;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Mvvm.UI.Native.ViewGenerator {
	public class GenerateEditorOptions {
		public static GenerateEditorOptions ForGridScaffolding(IEnumerable<TypeNamePropertyPair> collectionProperties = null) {
			return new GenerateEditorOptions(true, true, true, true, collectionProperties, LayoutType.Table, true);
		}
		public static GenerateEditorOptions ForLayoutScaffolding(IEnumerable<TypeNamePropertyPair> collectionProperties = null) {
			return new GenerateEditorOptions(true, true, true, false, collectionProperties, LayoutType.DataForm, true);
		}
		public static GenerateEditorOptions ForGridRuntime() {
			return new GenerateEditorOptions(false, false, false, true, null, LayoutType.Table, true);
		}
		public static GenerateEditorOptions ForLayoutRuntime() {
			return new GenerateEditorOptions(false, false, false, true, null, LayoutType.DataForm, true);
		}
		public static GenerateEditorOptions ForRuntime() {
			return new GenerateEditorOptions(false, false, false, true, null, LayoutType.Default, false);
		}
		internal GenerateEditorOptions(bool scaffolding, bool guessImageProperties, bool guessDisplayMembers, bool sortColumnsWithNegativeOrder, IEnumerable<TypeNamePropertyPair> collectionProperties, LayoutType layoutType, bool skipIEnumerableProperties) {
			this.Scaffolding = scaffolding;
			this.GuessDisplayMembers = guessDisplayMembers;
			this.GuessImageProperties = guessImageProperties;
			this.CollectionProperties = collectionProperties;
			this.SortColumnsWithNegativeOrder = sortColumnsWithNegativeOrder;
			this.SkipIEnumerableProperties = skipIEnumerableProperties;
			this.LayoutType = layoutType;
		}
		public bool Scaffolding { get; private set; }
		public bool GuessImageProperties { get; private set; }
		public bool GuessDisplayMembers { get; private set; }
		public bool SortColumnsWithNegativeOrder { get; private set; }
		public bool SkipIEnumerableProperties { get; private set; }
		public IEnumerable<TypeNamePropertyPair> CollectionProperties { get; private set; }
		internal LayoutType LayoutType { get; private set; }
	}
}
