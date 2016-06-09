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
using System.Windows.Forms;
using System.Windows.Input;
using DevExpress.XtraDiagram.Extensions;
using PlatformKey = System.Windows.Input.Key;
namespace DevExpress.XtraDiagram.Utils {
	public class KeysTranslationUtil {
		public static PlatformKey GetPlatformKey(Keys keyCode) {
			return translationTable.GetValueOrDefault(keyCode, PlatformKey.None);
		}
		static readonly Dictionary<Keys, PlatformKey> translationTable = Create();
		static Dictionary<Keys, PlatformKey> Create() {
			Dictionary<Keys, PlatformKey> keys = new Dictionary<Keys, Key>();
			LoadKeys(keys);
			return keys;
		}
		static Dictionary<Keys, PlatformKey> LoadKeys(Dictionary<Keys, PlatformKey> keys) {
			keys.Add(Keys.None, PlatformKey.None);
			keys.Add(Keys.Cancel, PlatformKey.Cancel);
			keys.Add(Keys.Back, PlatformKey.Back);
			keys.Add(Keys.Tab, PlatformKey.Tab);
			keys.Add(Keys.LineFeed, PlatformKey.LineFeed);
			keys.Add(Keys.Clear, PlatformKey.Clear);
			keys.Add(Keys.Enter, PlatformKey.Enter);
			keys.Add(Keys.ShiftKey, PlatformKey.LeftShift);
			keys.Add(Keys.ControlKey, PlatformKey.LeftCtrl);
			keys.Add(Keys.Menu, PlatformKey.LeftAlt);
			keys.Add(Keys.Pause, PlatformKey.Pause);
			keys.Add(Keys.CapsLock, PlatformKey.Capital);
			keys.Add(Keys.Escape, PlatformKey.Escape);
			keys.Add(Keys.IMEConvert, PlatformKey.ImeConvert);
			keys.Add(Keys.IMENonconvert, PlatformKey.ImeNonConvert);
			keys.Add(Keys.IMEAccept, PlatformKey.ImeAccept);
			keys.Add(Keys.IMEModeChange, PlatformKey.ImeModeChange);
			keys.Add(Keys.Space, PlatformKey.Space);
			keys.Add(Keys.PageUp, PlatformKey.PageUp);
			keys.Add(Keys.PageDown, PlatformKey.PageDown);
			keys.Add(Keys.End, PlatformKey.End);
			keys.Add(Keys.Home, PlatformKey.Home);
			keys.Add(Keys.Left, PlatformKey.Left);
			keys.Add(Keys.Up, PlatformKey.Up);
			keys.Add(Keys.Right, PlatformKey.Right);
			keys.Add(Keys.Down, PlatformKey.Down);
			keys.Add(Keys.Select, PlatformKey.Select);
			keys.Add(Keys.Print, PlatformKey.Print);
			keys.Add(Keys.Execute, PlatformKey.Execute);
			keys.Add(Keys.Snapshot, PlatformKey.Snapshot);
			keys.Add(Keys.Insert, PlatformKey.Insert);
			keys.Add(Keys.Delete, PlatformKey.Delete);
			keys.Add(Keys.Help, PlatformKey.Help);
			keys.Add(Keys.D0, PlatformKey.D0);
			keys.Add(Keys.D1, PlatformKey.D1);
			keys.Add(Keys.D2, PlatformKey.D2);
			keys.Add(Keys.D3, PlatformKey.D3);
			keys.Add(Keys.D4, PlatformKey.D4);
			keys.Add(Keys.D5, PlatformKey.D5);
			keys.Add(Keys.D6, PlatformKey.D6);
			keys.Add(Keys.D7, PlatformKey.D7);
			keys.Add(Keys.D8, PlatformKey.D8);
			keys.Add(Keys.D9, PlatformKey.D9);
			keys.Add(Keys.A, PlatformKey.A);
			keys.Add(Keys.B, PlatformKey.B);
			keys.Add(Keys.C, PlatformKey.C);
			keys.Add(Keys.D, PlatformKey.D);
			keys.Add(Keys.E, PlatformKey.E);
			keys.Add(Keys.F, PlatformKey.F);
			keys.Add(Keys.G, PlatformKey.G);
			keys.Add(Keys.H, PlatformKey.H);
			keys.Add(Keys.I, PlatformKey.I);
			keys.Add(Keys.J, PlatformKey.J);
			keys.Add(Keys.K, PlatformKey.K);
			keys.Add(Keys.L, PlatformKey.L);
			keys.Add(Keys.M, PlatformKey.M);
			keys.Add(Keys.N, PlatformKey.N);
			keys.Add(Keys.O, PlatformKey.O);
			keys.Add(Keys.P, PlatformKey.P);
			keys.Add(Keys.Q, PlatformKey.Q);
			keys.Add(Keys.R, PlatformKey.R);
			keys.Add(Keys.S, PlatformKey.S);
			keys.Add(Keys.T, PlatformKey.T);
			keys.Add(Keys.U, PlatformKey.U);
			keys.Add(Keys.V, PlatformKey.V);
			keys.Add(Keys.W, PlatformKey.M);
			keys.Add(Keys.X, PlatformKey.X);
			keys.Add(Keys.Y, PlatformKey.Y);
			keys.Add(Keys.Z, PlatformKey.Z);
			keys.Add(Keys.LWin, PlatformKey.LWin);
			keys.Add(Keys.RWin, PlatformKey.RWin);
			keys.Add(Keys.Apps, PlatformKey.Apps);
			keys.Add(Keys.Sleep, PlatformKey.Sleep);
			keys.Add(Keys.NumPad0, PlatformKey.NumPad0);
			keys.Add(Keys.NumPad1, PlatformKey.NumPad1);
			keys.Add(Keys.NumPad2, PlatformKey.NumPad2);
			keys.Add(Keys.NumPad3, PlatformKey.NumPad3);
			keys.Add(Keys.NumPad4, PlatformKey.NumPad4);
			keys.Add(Keys.NumPad5, PlatformKey.NumPad5);
			keys.Add(Keys.NumPad6, PlatformKey.NumPad6);
			keys.Add(Keys.NumPad7, PlatformKey.NumPad7);
			keys.Add(Keys.NumPad8, PlatformKey.NumPad8);
			keys.Add(Keys.NumPad9, PlatformKey.NumPad9);
			keys.Add(Keys.Multiply, PlatformKey.Multiply);
			keys.Add(Keys.Add, PlatformKey.Add);
			keys.Add(Keys.Separator, PlatformKey.Separator);
			keys.Add(Keys.Subtract, PlatformKey.Subtract);
			keys.Add(Keys.Decimal, PlatformKey.Decimal);
			keys.Add(Keys.Divide, PlatformKey.Divide);
			keys.Add(Keys.F1, PlatformKey.F1);
			keys.Add(Keys.F2, PlatformKey.F2);
			keys.Add(Keys.F3, PlatformKey.F3);
			keys.Add(Keys.F4, PlatformKey.F4);
			keys.Add(Keys.F5, PlatformKey.F5);
			keys.Add(Keys.F6, PlatformKey.F6);
			keys.Add(Keys.F7, PlatformKey.F7);
			keys.Add(Keys.F8, PlatformKey.F8);
			keys.Add(Keys.F9, PlatformKey.F9);
			keys.Add(Keys.F10, PlatformKey.F10);
			keys.Add(Keys.F11, PlatformKey.F11);
			keys.Add(Keys.F12, PlatformKey.F12);
			keys.Add(Keys.F13, PlatformKey.F13);
			keys.Add(Keys.F14, PlatformKey.F14);
			keys.Add(Keys.F15, PlatformKey.F15);
			keys.Add(Keys.F16, PlatformKey.F16);
			keys.Add(Keys.F17, PlatformKey.F17);
			keys.Add(Keys.F18, PlatformKey.F18);
			keys.Add(Keys.F19, PlatformKey.F19);
			keys.Add(Keys.F20, PlatformKey.F20);
			keys.Add(Keys.F21, PlatformKey.F21);
			keys.Add(Keys.F22, PlatformKey.F22);
			keys.Add(Keys.F23, PlatformKey.F23);
			keys.Add(Keys.F24, PlatformKey.F24);
			keys.Add(Keys.NumLock, PlatformKey.NumLock);
			keys.Add(Keys.Scroll, PlatformKey.Scroll);
			keys.Add(Keys.LShiftKey, PlatformKey.LeftShift);
			keys.Add(Keys.RShiftKey, PlatformKey.RightShift);
			keys.Add(Keys.LControlKey, PlatformKey.LeftCtrl);
			keys.Add(Keys.RControlKey, PlatformKey.RightCtrl);
			keys.Add(Keys.LMenu, PlatformKey.LeftAlt);
			keys.Add(Keys.RMenu, PlatformKey.RightAlt);
			keys.Add(Keys.BrowserBack, PlatformKey.BrowserBack);
			keys.Add(Keys.BrowserForward, PlatformKey.BrowserForward);
			keys.Add(Keys.BrowserRefresh, PlatformKey.BrowserRefresh);
			keys.Add(Keys.BrowserStop, PlatformKey.BrowserStop);
			keys.Add(Keys.BrowserSearch, PlatformKey.BrowserSearch);
			keys.Add(Keys.BrowserFavorites, PlatformKey.BrowserFavorites);
			keys.Add(Keys.BrowserHome, PlatformKey.BrowserHome);
			keys.Add(Keys.VolumeMute, PlatformKey.VolumeMute);
			keys.Add(Keys.VolumeDown, PlatformKey.VolumeDown);
			keys.Add(Keys.VolumeUp, PlatformKey.VolumeUp);
			keys.Add(Keys.MediaNextTrack, PlatformKey.MediaNextTrack);
			keys.Add(Keys.MediaPreviousTrack, PlatformKey.MediaPreviousTrack);
			keys.Add(Keys.MediaStop, PlatformKey.MediaStop);
			keys.Add(Keys.MediaPlayPause, PlatformKey.MediaPlayPause);
			keys.Add(Keys.LaunchMail, PlatformKey.LaunchMail);
			keys.Add(Keys.SelectMedia, PlatformKey.SelectMedia);
			keys.Add(Keys.LaunchApplication1, PlatformKey.LaunchApplication1);
			keys.Add(Keys.LaunchApplication2, PlatformKey.LaunchApplication2);
			keys.Add(Keys.Oem1, PlatformKey.Oem1);
			keys.Add(Keys.Oemplus, PlatformKey.OemPlus);
			keys.Add(Keys.Oemcomma, PlatformKey.OemComma);
			keys.Add(Keys.OemMinus, PlatformKey.OemMinus);
			keys.Add(Keys.OemPeriod, PlatformKey.OemPeriod);
			keys.Add(Keys.Oem2, PlatformKey.Oem2);
			keys.Add(Keys.Oem3, PlatformKey.Oem3);
			keys.Add(Keys.Oem4, PlatformKey.Oem4);
			keys.Add(Keys.Oem5, PlatformKey.Oem5);
			keys.Add(Keys.Oem6, PlatformKey.Oem6);
			keys.Add(Keys.Oem7, PlatformKey.Oem7);
			keys.Add(Keys.Oem8, PlatformKey.Oem8);
			keys.Add(Keys.Oem102, PlatformKey.Oem102);
			keys.Add(Keys.Attn, PlatformKey.Attn);
			keys.Add(Keys.Crsel, PlatformKey.CrSel);
			keys.Add(Keys.Exsel, PlatformKey.ExSel);
			keys.Add(Keys.EraseEof, PlatformKey.EraseEof);
			keys.Add(Keys.Play, PlatformKey.Play);
			keys.Add(Keys.Zoom, PlatformKey.Zoom);
			keys.Add(Keys.NoName, PlatformKey.NoName);
			keys.Add(Keys.Pa1, PlatformKey.Pa1);
			keys.Add(Keys.OemClear, PlatformKey.OemClear);
			keys.Add(Keys.Shift, PlatformKey.LeftShift);
			keys.Add(Keys.Control, PlatformKey.LeftCtrl);
			keys.Add(Keys.Alt, PlatformKey.LeftAlt);
			return keys;
		}
	}
}
