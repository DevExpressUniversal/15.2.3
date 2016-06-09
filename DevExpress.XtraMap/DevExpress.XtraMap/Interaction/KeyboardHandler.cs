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
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Utils;
using DevExpress.Services.Implementation;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap.Services {
	public class MapKeyboardHandlerService : KeyboardHandlerService {
		readonly InnerMap control;
		public MapKeyboardHandlerService(InnerMap control, KeyboardHandler handler)
			: base(handler) {
			Guard.ArgumentNotNull(handler, "handler");
			Guard.ArgumentNotNull(control, "map");
			this.control = control;
		}
		public InnerMap Control { get { return control; } }
		public override object CreateContext() {
			return Control;
		}
	}
	public class MapKeyboardHandler : CommandBasedKeyboardHandler<Type> {
		readonly InnerMap map;
		readonly IKeyHashProvider provider;
		protected IKeyHashProvider Provider { get { return provider; } }
		protected InnerMap Map { get { return map; } }
		public MapKeyboardHandler(InnerMap map) {
			this.map = map;
			provider = new MapKeyHashProvider();
			Initialize();
		}
		protected virtual void Initialize() {
			RegisterKeyHandler(provider, Keys.Add, Keys.None, typeof(ZoomInCommand));
			RegisterKeyHandler(provider, Keys.Subtract, Keys.None, typeof(ZoomOutCommand));
			RegisterKeyHandler(provider, Keys.Left, Keys.None, typeof(MoveRightCommand));
			RegisterKeyHandler(provider, Keys.Right, Keys.None, typeof(MoveLeftCommand));
			RegisterKeyHandler(provider, Keys.Up, Keys.None, typeof(MoveBottomCommand));
			RegisterKeyHandler(provider, Keys.Down, Keys.None, typeof(MoveTopCommand));
#if DEBUG
			RegisterKeyHandler(provider, Keys.I, Keys.Control, typeof(DebugInfoCommand));
			RegisterKeyHandler(provider, Keys.C, Keys.Control, typeof(ViewCenterPointCommand));
			RegisterKeyHandler(provider, Keys.Z, Keys.Control, typeof(ResetZoomCommand));
#endif
		}
		protected override void ValidateHandlerId(Type handlerType) {
			if(!typeof(MapKeyboardCommand).IsAssignableFrom(handlerType))
				throw new MapException("handlerType is not valid for MapControl");
		}
		public override Command CreateHandlerCommand(Type handlerType) {
			ConstructorInfo ci = handlerType.GetConstructor(new Type[] { typeof(InnerMap) });
			return (ci != null) ? (MapKeyboardCommand)ci.Invoke(new object[] { map }) : null;
		}
		protected override IKeyHashProvider CreateKeyHashProviderFromContext() {
			return new MapKeyHashProvider();
		}
	}
	public class MapKeyHashProvider : IKeyHashProvider {
		public MapKeyHashProvider() {
		}
		public Int64 CreateHash(Int64 keyData) {
			return keyData;
		}
	}
}
