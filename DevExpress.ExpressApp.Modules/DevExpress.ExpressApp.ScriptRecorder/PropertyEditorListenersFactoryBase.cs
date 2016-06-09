#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.ScriptRecorder {
	public abstract class PropertyEditorListenersFactoryBase {
		private Dictionary<Type, IPropertyEditorListener> registeredListeners = new Dictionary<Type, IPropertyEditorListener>();
		private IPropertyEditorListener defaultListener = new PropertyEditorListener();
		public PropertyEditorListenersFactoryBase() {
			RegisterListeners();
		}
		protected abstract void RegisterListeners();
		public IPropertyEditorListener DefaultListener {
			get { return defaultListener; }
			set { defaultListener = value; }
		}
		public Dictionary<Type, IPropertyEditorListener> RegisteredListeners {
			get { return registeredListeners; }
			set { registeredListeners = value; }
		}
		public IList<IScriptRecorderControlListener> CreatePropertyEditorListeners(IList<DevExpress.ExpressApp.Editors.PropertyEditor> editors) {
			List<IScriptRecorderControlListener> result = new List<IScriptRecorderControlListener>();
			foreach(PropertyEditor editor in editors) {
				bool isFindlistener = false;
				IPropertyEditorListener listener = null;
				foreach(Type listenerType in RegisteredListeners.Keys) {
					if(listenerType.IsAssignableFrom(editor.GetType())) {
						if(RegisteredListeners.TryGetValue(listenerType, out listener)) {
							listener = listener.Clone();
							isFindlistener = true;
							break;
						}
					}
				}
				if(!isFindlistener) {
					listener = DefaultListener.Clone();
				}
				listener.RegisterControl(editor);
				result.Add(listener);
			}
			return result;
		}
	}
}
