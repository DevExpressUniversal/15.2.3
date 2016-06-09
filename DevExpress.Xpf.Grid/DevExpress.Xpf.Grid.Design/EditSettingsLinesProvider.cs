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

#if SILVERLIGHT
extern alias Platform;
#endif
using System;
using System.Collections.Generic;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Design.DependencyPropertyHelper;
using DevExpress.Design.SmartTags;
using System.Windows;
using Microsoft.Windows.Design.Model;
using System.Windows.Input;
using DevExpress.Xpf.Core.Design.Wizards;
using System.Reflection;
using DevExpress.Xpf.Core.Design;
#if SILVERLIGHT
using Platform::DevExpress.Xpf.Editors.Settings;
using Platform::DevExpress.Xpf.Editors;
using Platform::DevExpress.Xpf.Grid.LookUp;
using Platform::DevExpress.Xpf.Editors.Helpers;
#else
using DevExpress.Data;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid.LookUp;
using DevExpress.Xpf.Editors.Helpers;
#endif
using DevExpress.Xpf.Core.Design.SmartTags;
namespace DevExpress.Xpf.Grid.Design {
	public class LookUpEditPropertyLinesProvider : LookUpEditPropertyProviderBase {
		public LookUpEditPropertyLinesProvider(Type itemType, PropertyTarget target) : base(itemType, target) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = base.GetPropertiesImpl(viewModel);
			lines.Add(() => new ItemListPropertyLineViewModel(viewModel, GetPropertyName(() => LookUpEdit.StyleSettingsProperty), typeof(BaseLookUpStyleSettings), GetLookUpStyleSettings()), PropertyTarget.All);
			return lines;
		}
		IEnumerable<InstanceSourceBase> GetLookUpStyleSettings() {
			return DXTypeInfoInstanceSource.FromDXTypeInfoList(new DXTypeInfo[] {
				DXTypeInfo.FromType(typeof(LookUpEditStyleSettings)),
				DXTypeInfo.FromType(typeof(SearchLookUpEditStyleSettings))
			});
		}
	}
}
