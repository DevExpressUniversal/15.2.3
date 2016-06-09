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

using DevExpress.Xpf.Editors.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.Core.Design.Editors {
	public static class EditSettingsSourceTypeProvider {
		static Dictionary<Type, Type> editSettingsTypes = new Dictionary<Type, Type>();
		static EditSettingsSourceTypeProvider() {
			editSettingsTypes.Add(typeof(PopupColorEditSettings), typeof(Color));
			editSettingsTypes.Add(typeof(ColorEditSettings), typeof(Color));
			editSettingsTypes.Add(typeof(ListBoxEditSettings), typeof(object));
			editSettingsTypes.Add(typeof(MemoEditSettings), typeof(String));
			editSettingsTypes.Add(typeof(PopupBaseEditSettings), typeof(object));
			editSettingsTypes.Add(typeof(PasswordBoxEditSettings), typeof(String));
			editSettingsTypes.Add(typeof(TrackBarEditSettings), typeof(Int32));
			editSettingsTypes.Add(typeof(ImageEditSettings), typeof(object));
			editSettingsTypes.Add(typeof(PopupImageEditSettings), typeof(object));
			editSettingsTypes.Add(typeof(ProgressBarEditSettings), typeof(Int32));
			editSettingsTypes.Add(typeof(FontEditSettings), typeof(Font));
			editSettingsTypes.Add(typeof(CheckEditSettings), typeof(Boolean));
			editSettingsTypes.Add(typeof(ButtonEditSettings), typeof(object));
			editSettingsTypes.Add(typeof(CalcEditSettings), typeof(Int32));
			editSettingsTypes.Add(typeof(TextEditSettings), typeof(String));
			editSettingsTypes.Add(typeof(ComboBoxEditSettings), typeof(object));
			editSettingsTypes.Add(typeof(DateEditSettings), typeof(DateTime));
			editSettingsTypes.Add(typeof(SpinEditSettings), typeof(Int32));
			editSettingsTypes.Add(typeof(SparklineEditSettings), typeof(object));
			editSettingsTypes.Add(typeof(BarCodeEditSettings), typeof(Int32));
		}
		public static Type GetEditSettingsDefaultSourceType(Type editSettingsType) { 
			Type result = typeof(object);
			editSettingsTypes.TryGetValue(editSettingsType, out result);
			return result;
		}
	}
}
