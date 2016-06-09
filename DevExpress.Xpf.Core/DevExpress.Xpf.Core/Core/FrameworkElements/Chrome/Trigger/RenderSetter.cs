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

using DevExpress.Mvvm.Native;
using System;
using System.Collections.ObjectModel;
namespace DevExpress.Xpf.Core.Native {
	public class RenderSetter {
		public string Property { get; set; }
		public object Value { get; set; }
		public string TargetName { get; set; }
		public object ConvertedValue { get; private set; }
		bool isInitialized;
		public void SetValue(INamescope namescope, IElementHost elementHost) {
			var context = namescope.GetElement(TargetName);
			if(context==null)
				throw new ArgumentException(String.Format("Cannot find element with name '{0}'", TargetName));
			InitializeConvertedValue(context);
			context.Do(x => x.SetValue(Property, ConvertedValue));
		}
		void InitializeConvertedValue(object context) {
			if (isInitialized)
				return;
			ConvertedValue = RenderTriggerHelper.GetConvertedValue(context, Property, Value);
			isInitialized = true;
		}
		public void ResetValue(INamescope namescope, IElementHost elementHost) {
			var context = namescope.GetElement(TargetName);
			if (context == null)
				throw new ArgumentException(String.Format("Cannot find element with name '{0}'", TargetName));
			context.Do(x => x.ResetValue(Property));
		}
		public bool Matches(FrameworkRenderElementContext context, string propertyName) {
			return string.Equals(Property, propertyName) && string.Equals(TargetName, context.Name);
		}
	}
	public class RenderSetterCollection : ObservableCollection<RenderSetter> { }
}
