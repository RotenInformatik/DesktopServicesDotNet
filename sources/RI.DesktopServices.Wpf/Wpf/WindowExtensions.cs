using System;
using System.Windows;
using System.Windows.Interop;




namespace RI.DesktopServices.Wpf
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="Window" /> type.
	/// </summary>
	public static class WindowExtensions
	{
		#region Static Methods

		/// <summary>
		///     Enables or disables a window.
		/// </summary>
		/// <param name="window"> The window to enable/disable. </param>
		/// <param name="enable"> true if the window should be enabled, false if it should be disabled. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="window" /> is null. </exception>
		public static void EnableWindow (this Window window, bool enable)
		{
			if (window == null)
			{
				throw new ArgumentNullException(nameof(window));
			}

			IntPtr hWnd = window.GetWindowHandle();

			WindowsWindow.EnableWindow(hWnd, enable);
		}

		/// <summary>
		///     Gets the screen a window is currently shown on or associated to.
		/// </summary>
		/// <param name="window"> The window. </param>
		/// <returns>
		///     The screen the window is currently shown on or associated to.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="window" /> is null. </exception>
		public static Screen GetScreen (this Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException(nameof(window));
			}

			IntPtr hWnd = window.GetWindowHandle();
			Screen screen = Screen.FromHandle(hWnd);
			return screen;
		}

		/// <summary>
		///     Gets the screen index a window is currently shown on or associated to.
		/// </summary>
		/// <param name="window"> The window. </param>
		/// <returns> </returns>
		/// <returns>
		///     The screen index the window is currently shown on or associated to.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="window" /> is null. </exception>
		public static int GetScreenIndex (this Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException(nameof(window));
			}

			Screen screen = window.GetScreen();

			Screen[] allScreens = Screen.AllScreens;
			for (int i1 = 0; i1 < allScreens.Length; i1++)
			{
				if (allScreens[i1].DeviceName.Equals(screen.DeviceName, StringComparison.Ordinal))
				{
					return i1;
				}
			}

			return 0;
		}

		/// <summary>
		///     Gets the native window handle (HWND) for a window.
		/// </summary>
		/// <param name="window"> The window. </param>
		/// <returns>
		///     The window handle for the window.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="window" /> is null. </exception>
		public static IntPtr GetWindowHandle (this Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException(nameof(window));
			}

			WindowInteropHelper helper = new WindowInteropHelper(window);
			return helper.Handle;
		}

		/// <summary>
		///     Hides a window.
		/// </summary>
		/// <param name="window"> The window to hide. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="window" /> is null. </exception>
		public static void HideWindow (this Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException(nameof(window));
			}

			IntPtr hWnd = window.GetWindowHandle();

			WindowsWindow.HideWindow(hWnd);
		}

		/// <summary>
		///     Maximizes a window.
		/// </summary>
		/// <param name="window"> The window to maximize. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="window" /> is null. </exception>
		public static void MaximizeWindow (this Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException(nameof(window));
			}

			IntPtr hWnd = window.GetWindowHandle();

			WindowsWindow.MaximizeWindow(hWnd);
		}

		/// <summary>
		///     Minimizes a window.
		/// </summary>
		/// <param name="window"> The window to minimize. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="window" /> is null. </exception>
		public static void MinimizeWindow (this Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException(nameof(window));
			}

			IntPtr hWnd = window.GetWindowHandle();

			WindowsWindow.MinimizeWindow(hWnd);
		}

		/// <summary>
		///     Moves a window to the background (behind other windows).
		/// </summary>
		/// <param name="window"> The window to move to the background. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="window" /> is null. </exception>
		public static void MoveWindowToBackground (this Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException(nameof(window));
			}

			IntPtr hWnd = window.GetWindowHandle();

			WindowsWindow.MoveWindowToBackground(hWnd);
		}

		/// <summary>
		///     Moves a window to the foreground (ontop of other windows).
		/// </summary>
		/// <param name="window"> The window to move to the foreground. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="window" /> is null. </exception>
		public static void MoveWindowToForeground (this Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException(nameof(window));
			}

			IntPtr hWnd = window.GetWindowHandle();

			WindowsWindow.MoveWindowToForeground(hWnd);
		}

		/// <summary>
		///     Moves a window to the primary screen.
		/// </summary>
		/// <param name="window"> The window to move. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="window" /> is null. </exception>
		public static void MoveWindowToPrimaryScreen (this Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException(nameof(window));
			}

			IntPtr hWnd = window.GetWindowHandle();

			WindowsWindow.MoveWindowToPrimaryScreen(hWnd);
		}

		/// <summary>
		///     Moves a window to a screen.
		/// </summary>
		/// <param name="window"> The window to move. </param>
		/// <param name="screen"> The screen or null to move to the primary screen. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="window" /> is null. </exception>
		public static void MoveWindowToScreen (this Window window, Screen screen)
		{
			if (window == null)
			{
				throw new ArgumentNullException(nameof(window));
			}

			IntPtr hWnd = window.GetWindowHandle();

			WindowsWindow.MoveWindowToScreen(hWnd, screen);
		}

		/// <summary>
		///     Moves a window to a screen.
		/// </summary>
		/// <param name="window"> The window to move. </param>
		/// <param name="screenIndex"> The screen index or -1 to move to the primary screen. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="window" /> is null. </exception>
		public static void MoveWindowToScreen (this Window window, int screenIndex)
		{
			if (window == null)
			{
				throw new ArgumentNullException(nameof(window));
			}

			IntPtr hWnd = window.GetWindowHandle();

			WindowsWindow.MoveWindowToScreen(hWnd, screenIndex);
		}

		/// <summary>
		///     Sets a window to its normal position and size.
		/// </summary>
		/// <param name="window"> The window to set to its normal position and size. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="window" /> is null. </exception>
		public static void NormalizeWindow (this Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException(nameof(window));
			}

			IntPtr hWnd = window.GetWindowHandle();

			WindowsWindow.NormalizeWindow(hWnd);
		}

		/// <summary>
		///     Moves a window to a new position and size.
		/// </summary>
		/// <param name="window"> The window to move to a new position and size. </param>
		/// <param name="x"> The new x position of the window (top-left of the window). </param>
		/// <param name="y"> The new y position of the window (top-left of the window). </param>
		/// <param name="width"> The new width of the window. </param>
		/// <param name="height"> The new height of the window. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="window" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="width" /> or <paramref name="height" /> is less than zero. </exception>
		public static void RelocateWindow (this Window window, int x, int y, int width, int height)
		{
			if (window == null)
			{
				throw new ArgumentNullException(nameof(window));
			}

			if (width < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(width));
			}

			if (height < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(height));
			}

			IntPtr hWnd = window.GetWindowHandle();

			WindowsWindow.RelocateWindow(hWnd, x, y, width, height);
		}

		/// <summary>
		///     Shows a window.
		/// </summary>
		/// <param name="window"> The window to show. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="window" /> is null. </exception>
		public static void ShowWindow (this Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException(nameof(window));
			}

			IntPtr hWnd = window.GetWindowHandle();

			WindowsWindow.ShowWindow(hWnd);
		}

		#endregion
	}
}
