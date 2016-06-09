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

using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
namespace DevExpress.Xpf.Core {
	public class EventArgsToDataCellConverter : EventArgsConverterBase<EventArgs> {
		protected override object Convert(object sender, EventArgs args) {
			object row = (args as IDataCellEventArgs).With(x => x.Cell);
			if(row != null)
				return row;
			var converter = EventArgsConverterHelper.GetSpecificConverter<IDataCellEventArgsConverter>(args);
			var routedArgs = args as RoutedEventArgs;
			return (converter != null && routedArgs != null) ? converter.GetDataCell(routedArgs) : null;
		}
	}
	[Obsolete("Use EventArgsToDataCellConverter class instead")]
	public class RoutedEventArgsToDataCellConverter : EventArgsToDataCellConverter {
	}
	public class EventArgsToDataRowConverter : EventArgsConverterBase<EventArgs> {
		protected override object Convert(object sender, EventArgs args) {
			object row = (args as IDataRowEventArgs).With(x => x.Row);
			if(row != null)
				return row;
			var converter = EventArgsConverterHelper.GetSpecificConverter<IDataRowEventArgsConverter>(args);
			var routedArgs = args as RoutedEventArgs;
			return (converter != null && routedArgs != null) ? converter.GetDataRow(routedArgs) : null;
		}
	}
	[Obsolete("Use EventArgsToDataRowConverter class instead")]
	public class RoutedEventArgsToDataRowConverter : EventArgsToDataRowConverter {
	}
	public static class EventArgsConverterHelper {
		public static T GetSpecificConverter<T>(EventArgs e) where T : class {
			var converterSource = LayoutHelper.FindLayoutOrVisualParentObject<IEventArgsConverterSource>(e.With(x => x as RoutedEventArgs).With(x => x.OriginalSource as DependencyObject), true);
			return converterSource.With(x => x.EventArgsConverter as T);
		}
	}
}
