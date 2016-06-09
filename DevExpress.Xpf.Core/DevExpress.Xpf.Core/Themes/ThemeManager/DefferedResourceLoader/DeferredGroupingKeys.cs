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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
namespace DevExpress.Xpf.Core {
	public class DeferredGroupingKeyExtensionBase<T> : MarkupExtension {
		public DeferredGroupingKeyExtensionBase(T key) {
			Key = key;
		}
		public T Key { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return Key;
		}
	}
	public class TestGroupingKeyExtension : DeferredGroupingKeyExtensionBase<TestGroupingKey> {
		public TestGroupingKeyExtension(TestGroupingKey key) : base(key) {
		}
	}
	public enum TestGroupingKey {
		TestControl
	}
	public class DefaultCoreGroupingKeyExtension : DeferredGroupingKeyExtensionBase<DefaultCoreGroupingKey> {
		public DefaultCoreGroupingKeyExtension(DefaultCoreGroupingKey key) : base(key) {
		}
	}
	public enum DefaultCoreGroupingKey {
		DXPanel,
		DragIconControl,
		SuperTip,
		SearchPanel,
		Window,
		Colorizer,
		UserControl,
		WaitIndicators,
		TransferStyles,
		Popup,
		PseudoWindow,
		ScrollBar,
		TabControl,
		CornerBox,
		StandardControlStyles,
		DisplayFormatTextControls,
	}
	public class BarsGroupingKeyExtension : DeferredGroupingKeyExtensionBase<BarsGroupingKey> {
		public BarsGroupingKeyExtension(BarsGroupingKey key)
			: base(key) {
		}
	}
	public enum BarsGroupingKey {
		Generic
	}
	public class EditorsGroupingKeyExtension : DeferredGroupingKeyExtensionBase<EditorsGroupingKey> {
		public EditorsGroupingKeyExtension(EditorsGroupingKey key)
			: base(key) {
		}
	}
	public enum EditorsGroupingKey {
		Generic
	}
}
