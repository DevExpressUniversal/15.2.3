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
namespace DevExpress.Xpf.Bars.Native {
	public interface IBarToContainerNameBinderService : IElementBinderService { }
	public interface IBarToContainerTypeBinderService : IElementBinderService { }
	sealed class BarToContainerNameBinderService : ElementBinderService, IBarToContainerNameBinderService {
		public BarToContainerNameBinderService(object binder) : base(binder) { }
	}
	sealed class BarToContainerTypeBinderService : ElementBinderService, IBarToContainerTypeBinderService {
		public BarToContainerTypeBinderService(object binder) : base(binder) { }
	}
	public class BarToContainerNameBinder : ElementBinder<IBarToContainerNameBinderService> {
		public BarToContainerNameBinder() : base(BarRegistratorKeys.BarNameKey, BarRegistratorKeys.ContainerNameKey, ScopeSearchSettings.Local | ScopeSearchSettings.Ancestors, ScopeSearchSettings.Local | ScopeSearchSettings.Descendants) { } 
		protected override bool CanLink(IBarNameScopeSupport first, IBarNameScopeSupport second) { return BarToContainerTypeBinderHelper.CanLink(first, second, BarRegistratorKeys.BarNameKey); }
		protected override bool CanUnlink(IBarNameScopeSupport first, IBarNameScopeSupport second) { return BarToContainerTypeBinderHelper.CanUnlink(first, second, BarRegistratorKeys.BarNameKey); }
		protected override void Link(IBarNameScopeSupport element, IBarNameScopeSupport link) { BarToContainerTypeBinderHelper.Link(element, link); }
		protected override void Unlink(IBarNameScopeSupport element, IBarNameScopeSupport link) { BarToContainerTypeBinderHelper.Unlink(element, link); }
	}	
	public class BarToContainerTypeBinder : ElementBinder<IBarToContainerTypeBinderService> {
		public BarToContainerTypeBinder() : base(BarRegistratorKeys.BarTypeKey, BarRegistratorKeys.ContainerTypeKey, ScopeSearchSettings.Local, ScopeSearchSettings.Local) { }
		protected override bool CanLink(IBarNameScopeSupport first, IBarNameScopeSupport second) { return BarToContainerTypeBinderHelper.CanLink(first, second, BarRegistratorKeys.BarTypeKey); }
		protected override bool CanUnlink(IBarNameScopeSupport first, IBarNameScopeSupport second) { return BarToContainerTypeBinderHelper.CanUnlink(first, second, BarRegistratorKeys.BarTypeKey); }
		protected override void Link(IBarNameScopeSupport element, IBarNameScopeSupport link) { BarToContainerTypeBinderHelper.Link(element, link); }
		protected override void Unlink(IBarNameScopeSupport element, IBarNameScopeSupport link) { BarToContainerTypeBinderHelper.Unlink(element, link); }
	}	
	static class BarToContainerTypeBinderHelper {
		public static bool CanLink(IBarNameScopeSupport first, IBarNameScopeSupport second, object binderKey) { return CanLink((first as IBar) ?? (second as IBar), (first as BarContainerControl) ?? (second as BarContainerControl), binderKey); }		
		public static bool CanUnlink(IBarNameScopeSupport first, IBarNameScopeSupport second, object binderKey) { return true; }
		public static void Link(IBarNameScopeSupport element, IBarNameScopeSupport link) { Link((element as IBar) ?? (link as IBar), (element as BarContainerControl) ?? (link as BarContainerControl)); }		
		public static void Unlink(IBarNameScopeSupport element, IBarNameScopeSupport link) { Unlink((element as IBar) ?? (link as IBar), (element as BarContainerControl) ?? (link as BarContainerControl)); }
		static void Link(IBar bar, BarContainerControl barContainerControl) { barContainerControl.Link(bar); }
		static void Unlink(IBar bar, BarContainerControl barContainerControl) { barContainerControl.Unlink(bar); }
		static bool CanLink(IBar bar, BarContainerControl barContainerControl, object binderKey) { return bar.CanBind(barContainerControl, binderKey) && barContainerControl.CanBind; }
	}
}
