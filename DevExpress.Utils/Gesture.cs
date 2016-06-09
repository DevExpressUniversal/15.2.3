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
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraEditors;
namespace DevExpress.Utils.Gesture {
	public enum GID : int {
		None = 0,
		BEGIN = 1,
		END = 2,
		ZOOM = 3,
		PAN = 4,
		ROTATE = 5,
		TWOFINGERTAP = 6,
		PRESSANDTAP = 7,
		ALL = 255
	}
	[Flags]
	public enum GF : int {
		None = 0,
		BEGIN = 0x00000001,
		INERTIA = 0x00000002,
		END = 0x00000004
	}
	public interface IGestureClient {
		IntPtr OverPanWindowHandle { get; }
		IntPtr Handle { get; }
		Point PointToClient(Point p);
		void OnPan(GestureArgs info, Point delta, ref Point overPan);
		void OnZoom(GestureArgs info, Point center, double zoomDelta);
		void OnRotate(GestureArgs info, Point center, double degreeDelta);
		void OnBegin(GestureArgs info);
		void OnEnd(GestureArgs info);
		void OnTwoFingerTap(GestureArgs info);
		void OnPressAndTap(GestureArgs info);
		GestureAllowArgs[] CheckAllowGestures(Point point);
	}
	public interface IFlickGestureClient {
		IntPtr Handle { get; }
		Point PointToClient(Point p);
		bool OnFlick(Point point, FlickGestureArgs args);
	}
	public class NullGestureClient : IGestureClient {
		IntPtr IGestureClient.OverPanWindowHandle { get { return IntPtr.Zero; } }
		void IGestureClient.OnTwoFingerTap(GestureArgs info) { }
		void IGestureClient.OnPressAndTap(GestureArgs info) { }
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) { }
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) { }
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) { }
		void IGestureClient.OnBegin(GestureArgs info) { }
		void IGestureClient.OnEnd(GestureArgs info) { }
		IntPtr IGestureClient.Handle { get { return IntPtr.Zero; } }
		Point IGestureClient.PointToClient(Point p) { return p; }
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) { return GestureAllowArgs.None; }
	}
	public class GestureAllowArgs {
		public static readonly GestureAllowArgs[] None = new GestureAllowArgs[] { new GestureAllowArgs() { GID = GID.None } };
		public static readonly GestureAllowArgs Zoom = new GestureAllowArgs() { GID = GID.ZOOM, AllowID = GestureHelper.GC_DEFAULT };
		public static readonly GestureAllowArgs PressAndTap = new GestureAllowArgs() { GID = GID.PRESSANDTAP, AllowID = GestureHelper.GC_DEFAULT };
		public static readonly GestureAllowArgs TwoFingerTap = new GestureAllowArgs() { GID = GID.TWOFINGERTAP, AllowID = GestureHelper.GC_DEFAULT };
		public static readonly GestureAllowArgs Rotate = new GestureAllowArgs() { GID = GID.ROTATE, AllowID = GestureHelper.GC_DEFAULT };
		public static readonly GestureAllowArgs Pan = new GestureAllowArgs() { GID = GID.PAN, AllowID = GestureHelper.GC_PAN_ALL };
		public static readonly GestureAllowArgs PanVertical = new GestureAllowArgs() { GID = GID.PAN, AllowID = GestureHelper.GC_PAN_ALL & (~GestureHelper.GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY) };
		public GestureAllowArgs()
			: this(GID.None, GestureHelper.GC_DEFAULT, 0) {
		}
		public GestureAllowArgs(GID gid, int allowID, int blockID) {
			GID = gid;
			AllowID = allowID;
			BlockID = blockID;
		}
		public int AllowID { get; set; }
		public int BlockID { get; set; }
		public GID GID { get; set; }
	}
	public class GesturePointInfo {
		Point point;
		public GesturePointInfo() { Point = Point.Empty; }
		public Point Point { get { return point; } set { point = value; } }
		public int X { get { return point.X; } }
		public int Y { get { return point.Y; } }
		public int Argument { get; set; }
		public double Radians { get { return GestureHelper.ArgToRadians(Argument); } }
		public double Degrees { get { return (180f / 3.1415f) * Radians; } }
		internal GesturePointInfo Clone() {
			return new GesturePointInfo() { Point = this.Point, Argument = this.Argument };
		}
	}
	public class GestureArgs {
		GID gid = GID.None;
		public GestureArgs() {
			Clear();
		}
		public bool IsBegin { get { return (GF & GF.BEGIN) != 0; } }
		public bool IsEnd { get { return (GF & GF.END) != 0; } }
		public int Arguments { get; set; }
		public GID GID {
			get { return gid; }
			set {
				if(GID != value) Clear();
				this.gid = value;
			}
		}
		internal Point StartWindowOffset { get; set; }
		public GesturePointInfo Start { get; set; }
		public GesturePointInfo Current { get; set; }
		public GesturePointInfo End { get; set; }
		public GF GF { get; set; }
		public bool Ignore { get; set; }
		public void Clear() {
			this.Ignore = false;
			this.GF = GF.None;
			this.gid = GID.None;
			Start = new GesturePointInfo();
			Current = new GesturePointInfo();
			End = new GesturePointInfo();
		}
	}
	public class WrappedGestureAllowArgs : GestureAllowArgs {
		GestureAllowArgs originalArgs;
		public WrappedGestureAllowArgs(GestureAllowArgs args)
			: base(args.GID, args.AllowID, args.BlockID) {
			originalArgs = args;
		}
	}
	public class FlickGestureArgs {
		FlickGestureHelper.FLICK_DATA data;
		internal FlickGestureArgs(FlickGestureHelper.FLICK_DATA data) {
			this.data = data;
		}
		public FlickActionCommandCode CommandCode {
			get { return (FlickActionCommandCode)data.iFlickActionCommandCode; }
		}
		public FlickDirection Direction {
			get { return (FlickDirection)data.iFlickDirection; }
		}
		public bool Shift { get { return data.fShiftModifier; } }
		public bool Control { get { return data.fControlModifier; } }
		public bool AltGR { get { return data.fAltGRModifier; } }
		public bool Menu { get { return data.fMenuModifier; } }
		public bool Win { get { return data.fWinModifier; } }
		public bool OnInkingSurface { get { return data.fOnInkingSurface; } }
	}
	public class FlickGestureHelper {
		IFlickGestureClient owner;
		public FlickGestureHelper(IFlickGestureClient owner) {
			this.owner = owner;
			if(!IsGestureSupported) return;
		}
		public IFlickGestureClient Owner { get { return owner; } }
		static bool? isGestureSupported = null;
		public static bool IsGestureSupported {
			get {
				if(isGestureSupported.HasValue) return isGestureSupported.Value;
				try {
					Version v = Environment.OSVersion.Version;
					isGestureSupported = (v.Major > 6 || (v.Major == 6 && v.Minor > 0));
				}
				catch { isGestureSupported = false; }
				return isGestureSupported.Value;
			}
		}
		public virtual bool WndProc(ref Message msg) {
			if(!IsGestureSupported) return false;
			switch(msg.Msg) {
				case WM_TABLET_FLICK:
					return DecodeFlickGesture(ref msg);
			}
			return false;
		}
		bool DecodeFlickGesture(ref Message msg) {
			FLICK_DATA data = FLICK_DATA.GetFrom(msg.WParam);
			msg.Result = OnFlick(data, FLICK_POINT.GetPoint(msg.LParam)) ?
				new IntPtr(FLICK_WM_HANDLED_MASK) : IntPtr.Zero;
			return false;
		}
		bool OnFlick(FLICK_DATA data, Point point) {
			Point clientPoint = Owner.PointToClient(point);
			return Owner.OnFlick(clientPoint, new FlickGestureArgs(data));
		}
		#region WinAPI
		const int WM_TABLET_DEFBASE = 0x02C0;
		const int WM_TABLET_FLICK = (WM_TABLET_DEFBASE + 11);
		[StructLayout(LayoutKind.Sequential)]
		struct FLICK_POINT {
			public int x;
			public int y;
			public static Point GetPoint(IntPtr lParam) {
				return new Point(GetInt(lParam));
			}
		}
		[StructLayout(LayoutKind.Sequential)]
		internal struct FLICK_DATA {
			public int iFlickActionCommandCode;
			public int iFlickDirection;
			public bool fControlModifier;
			public bool fMenuModifier;
			public bool fAltGRModifier;
			public bool fWinModifier;
			public bool fShiftModifier;
			public int iReserved;
			public bool fOnInkingSurface;
			public int iActionArgument;
			public static FLICK_DATA GetFrom(IntPtr wParam) {
				int data = GetInt(wParam);
				FLICK_DATA flickData = new FLICK_DATA();
				flickData.iFlickActionCommandCode = data & 0x1F;
				flickData.iFlickDirection = (data >> 5) & 0x7;
				flickData.fControlModifier = ((data >> 8) & 0x1) != 0;
				flickData.fMenuModifier = ((data >> 9) & 0x1) != 0;
				flickData.fAltGRModifier = ((data >> 10) & 0x1) != 0;
				flickData.fWinModifier = ((data >> 11) & 0x1) != 0;
				flickData.fShiftModifier = ((data >> 12) & 0x1) != 0;
				flickData.iReserved = (data >> 13) & 0x3;
				flickData.fOnInkingSurface = ((data >> 15) & 0x1) != 0;
				flickData.iActionArgument = data >> 16;
				return flickData;
			}
		}
		static int GetInt(IntPtr ptr) {
			return (IntPtr.Size == 8) ? (int)(ptr.ToInt64() & 0x00000000FFFFFFFFL) : ptr.ToInt32();
		}
		const int FLICK_WM_HANDLED_MASK = 0x01;
		#endregion
	}
	public class GestureHelper {
		IGestureClient owner;
		GestureArgs info;
		public GestureHelper(IGestureClient owner) {
			this.owner = owner;
			this.info = new GestureArgs();
			if(!IsGestureSupported) return;
			SetupStructSizes();
			PanWithGutter = true;
		}
		public static DefaultBoolean AllowOverpanWindow {
			get { return WindowsFormsSettings.AllowOverpanApplicationWindow; }
			set { WindowsFormsSettings.AllowOverpanApplicationWindow = value; }
		}
		public static IntPtr FindOverpanWindow(Control control) {
			Form f = control.FindForm();
			if(f == null || f.IsMdiChild || !f.IsHandleCreated) return IntPtr.Zero;
			return f.Handle;
		}
		static bool? isGestureSupported = null;
		public static bool IsGestureSupported {
			get {
				if(isGestureSupported.HasValue) return isGestureSupported.Value;
				try {
					Version v = Environment.OSVersion.Version;
					isGestureSupported = (v.Major > 6 || (v.Major == 6 && v.Minor > 0));
					SetupStructSizes();
				}
				catch {
					isGestureSupported = false;
				}
				return isGestureSupported.Value;
			}
		}
		public bool TogglePressAndHold(bool enable) {
			string tabletAtom = "MicrosoftTabletPenServiceProperty";
			short atomID = NativeMethods.GlobalAddAtom(tabletAtom);
			if(atomID == 0)
				return false;
			const int TABLET_DISABLE_PRESSANDHOLD = 1;
			const int TABLET_DISABLE_PENTAPFEEDBACK = 8;
			if(enable)
				NativeMethods.RemoveProp(Owner.Handle, tabletAtom);
			else
				NativeMethods.SetProp(Owner.Handle, tabletAtom, new IntPtr(TABLET_DISABLE_PRESSANDHOLD | TABLET_DISABLE_PENTAPFEEDBACK));
			return true;
		}
		public bool PanWithGutter { get; set; }
		protected GestureArgs Info { get { return info; } }
		public IGestureClient Owner { get { return owner as IGestureClient; } }
		Point PointToClient(Point p) {
			return Owner.PointToClient(p);
		}
		public virtual bool WndProc(ref Message msg) {
			if(!IsGestureSupported) return false;
			switch(msg.Msg) {
				case WM_GESTURENOTIFY: return GestureNotify(ref msg);
				case WM_GESTURE: return DecodeGesture(ref msg);
			}
			return false;
		}
		int debugCount = 0;
		void WriteLog(string text, params object[] args) {
			System.Diagnostics.Debug.WriteLine(string.Format("{0} :{1}", debugCount++, string.Format(text, args)));
		}
		void WriteLog(ref NativeMethods.GESTUREINFO gi) {
			WriteLog("{0}.{1}: Point {2}", (GID)gi.dwID, (GF)gi.dwFlags, GetClientLocation(ref gi));
		}
		bool DecodeGesture(ref Message msg) {
			if(Owner == null) return false;
			NativeMethods.GESTUREINFO gi = new NativeMethods.GESTUREINFO();
			gi.cbSize = _gestureInfoSize;
			if(!NativeMethods.GetGestureInfo(msg.LParam, ref gi)) {
				return false;
			}
			Info.GID = (GID)gi.dwID;
			info.GF = (GF)gi.dwFlags;
			switch(info.GID) {
				case GID.BEGIN:
					OnGidBegin(ref gi);
					break;
				case GID.END:
					ProcessGidEnd(ref gi);
					break;
				case GID.ZOOM:
					GidZoom(ref gi);
					break;
				case GID.PAN:
					GidPan(ref gi);
					break;
				case GID.ROTATE:
					GidRotate(ref gi);
					break;
				case GID.TWOFINGERTAP:
					GidTwoFingerTap(ref gi);
					break;
				case GID.PRESSANDTAP:
					GidPressAndTap(ref gi);
					break;
			}
			msg.Result = new IntPtr(1);
			NativeMethods.CloseGestureInfoHandle(msg.LParam);
			return true;
		}
		protected virtual void ProcessGidEnd(ref NativeMethods.GESTUREINFO gi) {
			OnEndOverpan();
		}
		Point GetClientLocation(ref NativeMethods.GESTUREINFO gi) {
			return PointToClient(gi.ptsLocation.ToPoint());
		}
		Point GetClientLocation(ref NativeMethods.GESTURENOTIFYSTRUCT gi) {
			return PointToClient(gi.ptsLocation.ToPoint());
		}
		protected virtual void OnGidBegin(ref NativeMethods.GESTUREINFO gi) {
			Info.Start.Point = GetClientLocation(ref gi);
			Owner.OnBegin(info);
		}
		protected virtual void OnGidEnd(ref NativeMethods.GESTUREINFO gi) {
			Info.End.Point = GetClientLocation(ref gi);
			Info.End.Argument = (int)(gi.ullArguments & ULL_ARGUMENTS_BIT_MASK);
			Owner.OnEnd(info);
			OnEndOverpan();
		}
		protected virtual void GidTwoFingerTap(ref NativeMethods.GESTUREINFO gi) {
			GesturePointInfo newPoint = new GesturePointInfo() { Point = PointToClient(new Point(gi.ptsLocation.x, gi.ptsLocation.y)), Argument = (int)(gi.ullArguments & ULL_ARGUMENTS_BIT_MASK) };
			Info.Current = Info.Start = newPoint;
			Owner.OnTwoFingerTap(info);
		}
		protected virtual void GidPressAndTap(ref NativeMethods.GESTUREINFO gi) {
			GesturePointInfo newPoint = new GesturePointInfo() { Point = PointToClient(new Point(gi.ptsLocation.x, gi.ptsLocation.y)), Argument = (int)(gi.ullArguments & ULL_ARGUMENTS_BIT_MASK) };
			Info.Current = Info.Start = newPoint;
			Owner.OnPressAndTap(info);
		}
		bool isFirstRotateZoomMessage = false;
		protected virtual void GidRotate(ref NativeMethods.GESTUREINFO gi) {
			GesturePointInfo newPoint = new GesturePointInfo() { Point = PointToClient(new Point(gi.ptsLocation.x, gi.ptsLocation.y)), Argument = (int)(gi.ullArguments & ULL_ARGUMENTS_BIT_MASK) };
			if((info.GF & GF.BEGIN) != 0) {
				isFirstRotateZoomMessage = true;
				info.Start = newPoint.Clone();
				info.Current = newPoint;
				return;
			}
			if(isFirstRotateZoomMessage) {
				this.isFirstRotateZoomMessage = false;
				info.GF |= GF.BEGIN;
				info.Current = newPoint;
				info.Start = newPoint.Clone();
			}
			OnGidRotate(info, newPoint);
		}
		protected virtual void GidZoom(ref NativeMethods.GESTUREINFO gi) {
			GesturePointInfo newPoint = new GesturePointInfo() { Point = PointToClient(new Point(gi.ptsLocation.x, gi.ptsLocation.y)), Argument = (int)(gi.ullArguments & ULL_ARGUMENTS_BIT_MASK) };
			if((info.GF & GF.BEGIN) != 0) {
				info.Start = newPoint.Clone();
				info.Current = newPoint;
				isFirstRotateZoomMessage = true;
				return;
			}
			if(isFirstRotateZoomMessage) {
				this.isFirstRotateZoomMessage = false;
				info.GF |= GF.BEGIN;
				info.Current = newPoint;
				info.Start = newPoint.Clone();
			}
			OnGidZoom(info, newPoint);
		}
		protected virtual void GidPan(ref NativeMethods.GESTUREINFO gi) {
			if(SwipeGestureHelper.BeforeGIDPan(gi)) return;
			Point newPoint = info.Current.Point;
			if((info.GF & GF.BEGIN) != 0) {
				newPoint = info.Current.Point = info.Start.Point = PointToClient(new Point(gi.ptsLocation.x, gi.ptsLocation.y));
				info.StartWindowOffset = PointToClient(new Point(0, 0));
				OnBeginOverpan();
			}
			else {
				if((info.GF & GF.END) != 0) {
					OnEndOverpan();
				}
				newPoint = PointToClient(new Point(gi.ptsLocation.x, gi.ptsLocation.y));
				Point offset = PointToClient(new Point(0, 0));
				if(offset != info.StartWindowOffset) {
					Point delta = new Point(info.StartWindowOffset.X - offset.X, info.StartWindowOffset.Y - offset.Y);
					newPoint.Offset(delta);
				}
			}
			Point overPan = Point.Empty;
			OnGidPan(info, newPoint, ref overPan);
			if(!overPan.IsEmpty) OnUpdateOverpan(overPan, (info.GF & GF.INERTIA) != 0);
			SwipeGestureHelper.AfterGIDPan(gi);
		}
		void OnUpdateOverpan(Point overPan, bool inertia) {
			if(!WindowsFormsSettings.GetAllowOverpanApplicationWindowCore()) return;
			if(Owner.OverPanWindowHandle != IntPtr.Zero)
				NativeMethods.UpdatePanningFeedback(Owner.OverPanWindowHandle, overPan.X, overPan.Y, inertia);
		}
		void OnEndOverpan() {
			if(!WindowsFormsSettings.GetAllowOverpanApplicationWindowCore()) return;
			if(Owner.OverPanWindowHandle != IntPtr.Zero)
				NativeMethods.EndPanningFeedback(Owner.OverPanWindowHandle, true);
		}
		void OnBeginOverpan() {
			if(!WindowsFormsSettings.GetAllowOverpanApplicationWindowCore()) return;
			if(Owner.OverPanWindowHandle != IntPtr.Zero)
				NativeMethods.BeginPanningFeedback(Owner.OverPanWindowHandle);
		}
		protected virtual void OnGidRotate(GestureArgs info, GesturePointInfo newPoint) {
			GesturePointInfo prev = info.Current.Clone();
			info.Current = newPoint;
			Owner.OnRotate(info, newPoint.Point, prev.Degrees - info.Current.Degrees);
		}
		protected virtual void OnGidZoom(GestureArgs info, GesturePointInfo newPoint) {
			GesturePointInfo prev = info.Current.Clone();
			info.Current = newPoint;
			Point center = new Point((prev.X + newPoint.X) / 2, (prev.Y + newPoint.Y) / 2);
			Owner.OnZoom(info, center, (double)info.Current.Argument / (double)prev.Argument);
		}
		protected virtual void OnGidPan(GestureArgs info, Point newPoint, ref Point overPan) {
			Point prev = info.Current.Point;
			info.Current.Point = newPoint;
			Owner.OnPan(info, new Point(info.Current.X - prev.X, info.Current.Y - prev.Y), ref overPan);
		}
		[System.Security.SecuritySafeCritical]
		bool GestureNotify(ref Message msg) {
			if(Owner == null) return false;
			NativeMethods.GESTURENOTIFYSTRUCT gs = new NativeMethods.GESTURENOTIFYSTRUCT();
			Marshal.PtrToStructure(msg.LParam, gs);
			GestureAllowArgs[] args = Owner.CheckAllowGestures(GetClientLocation(ref gs));
			NativeMethods.GESTURECONFIG[] gc = ConvertToGestureConfig(args);
			gc = SwipeGestureHelper.ProcessGestureNotify(gs, gc);
			for(int i = 0; i < gc.Length; i++) {
				if(gc[i].dwID == (int)GID.PAN) {
					if(PanWithGutter)
						gc[i].dwWant |= GC_PAN_WITH_GUTTER;
					else {
						gc[i].dwWant &= ~GC_PAN_WITH_GUTTER;
						gc[i].dwBlock |= GC_PAN_WITH_GUTTER;
					}
				}
			}
			NativeMethods.SetGestureConfig(
				Owner.Handle, 
				0,	  
				gc.Length,	  
				gc, 
				Marshal.SizeOf(new NativeMethods.GESTURECONFIG()) 
			);
			msg.Result = new IntPtr(1);
			return true;
		}
		static internal double ArgToRadians(Int64 arg) {
			return ((((double)(arg) / 65535.0) * 4.0 * 3.14159265) - 2.0 * 3.14159265);
		}
		NativeMethods.GESTURECONFIG[] ConvertToGestureConfig(GestureAllowArgs[] args) {
			if(args == null || args.Length == 0 || args[0].GID == GID.None) {
				return new NativeMethods.GESTURECONFIG[] { new NativeMethods.GESTURECONFIG() { dwID = 0, dwWant = 0, dwBlock = GestureHelper.GC_ALLGESTURES } };
			}
			if(args[0].GID == GID.ALL) {
				return new NativeMethods.GESTURECONFIG[] { new NativeMethods.GESTURECONFIG() { dwID = 0, dwWant = GestureHelper.GC_ALLGESTURES } };
			}
			List<NativeMethods.GESTURECONFIG> res = new List<NativeMethods.GESTURECONFIG>();
			foreach(GestureAllowArgs g in args) {
				if(g.GID == GID.PAN) res.Add(new NativeMethods.GESTURECONFIG() { dwID = (int)GID.PAN, dwWant = g.AllowID < 0 ? GestureHelper.GC_PAN_ALL : g.AllowID, dwBlock = g.BlockID });
				if(g.GID == GID.PRESSANDTAP) res.Add(new NativeMethods.GESTURECONFIG() { dwID = (int)GID.PRESSANDTAP, dwWant = g.AllowID < 0 ? GestureHelper.GC_PRESSANDTAP : g.AllowID, dwBlock = g.BlockID });
				if(g.GID == GID.ROTATE) res.Add(new NativeMethods.GESTURECONFIG() { dwID = (int)GID.ROTATE, dwWant = g.AllowID < 0 ? GestureHelper.GC_ROTATE : g.AllowID, dwBlock = g.BlockID });
				if(g.GID == GID.TWOFINGERTAP) res.Add(new NativeMethods.GESTURECONFIG() { dwID = (int)GID.TWOFINGERTAP, dwWant = g.AllowID < 0 ? GestureHelper.GC_TWOFINGERTAP : g.AllowID, dwBlock = g.BlockID });
				if(g.GID == GID.ZOOM) res.Add(new NativeMethods.GESTURECONFIG() { dwID = (int)GID.ZOOM, dwWant = g.AllowID < 0 ? GestureHelper.GC_ZOOM : g.AllowID, dwBlock = g.BlockID });
			}
			return res.ToArray();
		}
		#region Gesture API
		const Int64 ULL_ARGUMENTS_BIT_MASK = 0x00000000FFFFFFFF;
		const int WM_GESTURENOTIFY = 0x011A;
		const int WM_GESTURE = 0x0119;
		public const int GC_ALLGESTURES = 0x00000001,
							GC_DEFAULT = -1,
							GC_ZOOM = 0x00000001,
							GC_PAN_ALL = GC_PAN + GC_PAN_WITH_SINGLE_FINGER_VERTICALLY + GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY +
											GC_PAN_WITH_GUTTER + GC_PAN_WITH_INERTIA, 
							GC_PAN = 0x00000001, 
							GC_PAN_WITH_SINGLE_FINGER_VERTICALLY = 0x00000002, 
							GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY = 0x00000004, 
							GC_PAN_WITH_GUTTER = 0x00000008, 
							GC_PAN_WITH_INERTIA = 0x00000010, 
							GC_ROTATE = 0x00000001, 
							GC_TWOFINGERTAP = 0x00000001, 
							GC_PRESSANDTAP = 0x00000001; 
		static int _gestureConfigSize;
		static int _gestureInfoSize = -1;
		[SecurityPermission(SecurityAction.Demand)]
		static void SetupStructSizes() {
			if(_gestureInfoSize != -1) return;
			_gestureConfigSize = Marshal.SizeOf(new NativeMethods.GESTURECONFIG());
			_gestureInfoSize = Marshal.SizeOf(new NativeMethods.GESTUREINFO());
		}
		#endregion
	}
	public enum FlickActionCommandCode {
		Null = 0,
		Scroll = 1,
		Appcommand = 2,
		Customkey = 3,
		Keymodifier = 4
	}
	public enum FlickDirection {
		Right = 0,
		Upright = 1,
		Up = 2,
		Upleft = 3,
		Left = 4,
		Downleft = 5,
		Down = 6,
		Downright = 7,
		Invalid = 8
	}
	public interface ISwipeGestureClient {
		void OnSwipe(SwipeEventArgs args);
	}
	public enum Edge {
		None,
		Bottom,
		Top
	}
	public class SwipeEventArgs : System.ComponentModel.CancelEventArgs {
		readonly Edge edge;
		public SwipeEventArgs(Edge edge) {
			this.edge = edge;
		}
		public bool IsTopEdge { 
			get { return edge == Edge.Top; } 
		}
		public bool IsBottomEdge { 
			get { return edge == Edge.Bottom; } 
		}
	}
	[System.Security.SecuritySafeCritical]
	static class SwipeGestureHelper {
		const int swipeArea = 80;
		static List<IntPtr> blockList = new List<IntPtr>();
		static List<SwipeInfo> notifyList = new List<SwipeInfo>();
		class SwipeInfo {
			public Edge edge;
			public IntPtr hWnd;
			public int start;
			public bool IsValid(int current) {
				if(edge == Edge.Top)
					return current > start;
				if(edge == Edge.Bottom)
					return current < start;
				return false;
			}
		}
		internal static NativeMethods.GESTURECONFIG[] ProcessGestureNotify(NativeMethods.GESTURENOTIFYSTRUCT gs, NativeMethods.GESTURECONFIG[] gc) {
			var rootWnd = GetRoot(gs.hwndTarget);
			Edge edge;
			if(IsSwipeEdge(rootWnd, gs.ptsLocation.ToPoint(), out edge)) {
				notifyList.Add(new SwipeInfo() { hWnd = gs.hwndTarget, edge = edge });
				if(gc.Any(c => c.dwWant == GestureHelper.GC_ALLGESTURES))
					return gc;
				if(gc.Any(c => c.dwID == (int)GID.PAN))
					return gc;
				blockList.Add(gs.hwndTarget);
				var pan = new NativeMethods.GESTURECONFIG() { dwID = (int)GID.PAN, dwWant = GestureHelper.GC_PAN_WITH_SINGLE_FINGER_VERTICALLY };
				if(gc.Any(c => c.dwBlock == GestureHelper.GC_ALLGESTURES))
					return new NativeMethods.GESTURECONFIG[] { pan };
				return new NativeMethods.GESTURECONFIG[] { pan }.Concat(gc).ToArray();
			}
			return gc;
		}
		internal static bool BeforeGIDPan(NativeMethods.GESTUREINFO gi) {
			if(blockList.Contains(gi.hwndTarget))
				NotifyClient(gi);
			return blockList.Remove(gi.hwndTarget);
		}
		internal static void AfterGIDPan(NativeMethods.GESTUREINFO gi) {
			NotifyClient(gi);
		}
		static void NotifyClient(NativeMethods.GESTUREINFO gi) {
			SwipeInfo info = notifyList.Find(i => i.hWnd == gi.hwndTarget);
			if(info != null) {
				if((gi.dwFlags & (int)GF.BEGIN) == (int)GF.BEGIN)
					info.start = gi.ptsLocation.y;
				else {
					if(info.IsValid(gi.ptsLocation.y))
						NotifyClient(gi.hwndTarget, new SwipeEventArgs(info.edge));
					notifyList.Remove(info);
				}
			}
		}
		static bool NotifyClient(IntPtr handle, SwipeEventArgs args) {
			while(handle != IntPtr.Zero) {
				ISwipeGestureClient client = Control.FromHandle(handle) as ISwipeGestureClient;
				if(client != null) {
					client.OnSwipe(args);
					return !args.Cancel;
				}
				handle = GetParent(handle);
			}
			return false;
		}
		static bool IsSwipeEdge(IntPtr rootWnd, Point pt, out Edge edge) {
			edge = Edge.None;
			NativeMethods.RECT rect;
			if(NativeMethods.GetClientRect(rootWnd, out rect)) {
				NativeMethods.POINT lt = new NativeMethods.POINT(rect.Left, rect.Top);
				NativeMethods.POINT rb = new NativeMethods.POINT(rect.Right, rect.Bottom);
				NativeMethods.ClientToScreen(rootWnd, ref lt);
				NativeMethods.ClientToScreen(rootWnd, ref rb);
				Rectangle bounds = new Rectangle(lt.X, lt.Y, rb.X - lt.X, rb.Y - lt.Y);
				Rectangle topEdge = PlacementHelper.Arrange(new Size(bounds.Width, swipeArea), bounds, ContentAlignment.TopCenter);
				if(topEdge.Contains(pt)) 
					edge = Edge.Top;
				Rectangle bottomEdge = PlacementHelper.Arrange(new Size(bounds.Width, swipeArea), bounds, ContentAlignment.BottomCenter);
				if(bottomEdge.Contains(pt)) 
					edge = Edge.Bottom;
			}
			return edge != Edge.None;
		}
		internal static IntPtr GetRoot(IntPtr hWnd) {
			return UnsafeNativeMethods.GetAncestor(hWnd, 0x02);
		}
		internal static IntPtr GetParent(IntPtr hWnd) {
			return UnsafeNativeMethods.GetParent(hWnd);
		}
		static class UnsafeNativeMethods {
			[DllImport("user32.dll")]
			internal static extern IntPtr GetAncestor(IntPtr hWnd, uint dwFlag);
			[DllImport("user32.dll")]
			internal static extern IntPtr GetParent(IntPtr hWnd);
		}
	}
}
