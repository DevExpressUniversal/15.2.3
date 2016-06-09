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

using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Grid {
	public class GridGroupValueData : GridColumnData {
		static readonly DependencyPropertyKey DisplayTextPropertyKey;
		public static readonly DependencyProperty DisplayTextProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty DataInternalProperty;
		static GridGroupValueData() {
			DisplayTextPropertyKey = DependencyPropertyManager.RegisterReadOnly("DisplayText", typeof(string), typeof(GridGroupValueData), new PropertyMetadata(string.Empty, (d, e) => ((GridGroupValueData)d).OnDisplayTextChanged()));
			DisplayTextProperty = DisplayTextPropertyKey.DependencyProperty;
			DataInternalProperty = DependencyPropertyManager.Register("DataInternal", typeof(object), typeof(GridGroupValueData), new PropertyMetadata(null, (d, e) => ((GridGroupValueData)d).Data = e.NewValue));
		}
		public GridGroupValueData(ColumnsRowDataBase rowData)
			: base(rowData) {
		}
		public GroupRowData RowData { get { return (GroupRowData)RowDataBase; } }
		public string DisplayText {
			get { return (string)GetValue(DisplayTextProperty); }
			internal set { this.SetValue(DisplayTextPropertyKey, value); }
		}
		IGroupValueClient client;
		internal void SetGroupValueClient(IGroupValueClient client) {
			this.client = client;
		}
		void OnDisplayTextChanged() {
			if(client != null)
				client.UpdateText();
		}
	}
}
