// This file was generated by the Gtk# code generator.
// Any changes made will be lost if regenerated.

namespace GLib {

	using System;
	using System.Runtime.InteropServices;

#region Autogenerated code
	public partial class Cond : GLib.Opaque {
		public struct ABI {
			IntPtr p;
			int i1;
			int i2;
		}
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void d_g_cond_broadcast(IntPtr raw);
		static d_g_cond_broadcast g_cond_broadcast = FuncLoader.LoadFunction<d_g_cond_broadcast>(FuncLoader.GetProcAddress(GLibrary.Load(Library.GLib), "g_cond_broadcast"));

		public void Broadcast() {
			g_cond_broadcast(Handle);
		}
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void d_g_cond_clear(IntPtr raw);
		static d_g_cond_clear g_cond_clear = FuncLoader.LoadFunction<d_g_cond_clear>(FuncLoader.GetProcAddress(GLibrary.Load(Library.GLib), "g_cond_clear"));

		public void Clear() {
			g_cond_clear(Handle);
		}
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void d_g_cond_init(IntPtr raw);
		static d_g_cond_init g_cond_init = FuncLoader.LoadFunction<d_g_cond_init>(FuncLoader.GetProcAddress(GLibrary.Load(Library.GLib), "g_cond_init"));

		public void Init() {
			g_cond_init(Handle);
		}
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void d_g_cond_signal(IntPtr raw);
		static d_g_cond_signal g_cond_signal = FuncLoader.LoadFunction<d_g_cond_signal>(FuncLoader.GetProcAddress(GLibrary.Load(Library.GLib), "g_cond_signal"));

		public void Signal() {
			g_cond_signal(Handle);
		}
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate void d_g_cond_wait(IntPtr raw, IntPtr mutex);
		static d_g_cond_wait g_cond_wait = FuncLoader.LoadFunction<d_g_cond_wait>(FuncLoader.GetProcAddress(GLibrary.Load(Library.GLib), "g_cond_wait"));

		public void Wait(GLib.Mutex mutex) {
			g_cond_wait(Handle, mutex == null ? IntPtr.Zero : mutex.Handle);
		}
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate bool d_g_cond_wait_until(IntPtr raw, IntPtr mutex, long end_time);
		static d_g_cond_wait_until g_cond_wait_until = FuncLoader.LoadFunction<d_g_cond_wait_until>(FuncLoader.GetProcAddress(GLibrary.Load(Library.GLib), "g_cond_wait_until"));

		public bool WaitUntil(GLib.Mutex mutex, long end_time) {
			bool raw_ret = g_cond_wait_until(Handle, mutex == null ? IntPtr.Zero : mutex.Handle, end_time);
			bool ret = raw_ret;
			return ret;
		}

		public Cond(IntPtr raw) : base(raw) {}

#endregion
	}
}

