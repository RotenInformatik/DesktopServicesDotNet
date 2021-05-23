using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;




namespace RI.DesktopServices.Windows.IO.Keyboard
{
    /// <summary>
    ///     Provides access to the system keyboard.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class SystemKeyboard
    {
        #region Static Methods

        /// <summary>
        ///     Determines whether a specified key is pressed.
        /// </summary>
        /// <param name="key"> The key. </param>
        /// <returns>
        ///     true if the key is pressed, false otherwise.
        /// </returns>
        public static bool IsKeyPressed (SystemKeyboardKey key)
        {
            short result = SystemKeyboard.GetKeyState((int)key);

            bool keyPressed;

            switch ((result & 0x8000) >> 15)
            {
                default:
                {
                    keyPressed = true;
                    break;
                }

                case 0:
                {
                    keyPressed = false;
                    break;
                }

                case 1:
                {
                    keyPressed = true;
                    break;
                }
            }

            return keyPressed;
        }

        /// <summary>
        ///     Determines whether a keystroke (multiple keys at the same time) is pressed.
        /// </summary>
        /// <param name="keys"> The keystroke. </param>
        /// <returns>
        ///     true if all specified keys are pressed at the same time, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         true is returned if <paramref name="keys" /> is an empty sequence.
        ///     </para>
        ///     <para>
        ///         <paramref name="keys" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="keys" /> is null. </exception>
        public static bool IsKeystrokePressed (IEnumerable<SystemKeyboardKey> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            foreach (SystemKeyboardKey key in keys)
            {
                if (!SystemKeyboard.IsKeyPressed(key))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Determines whether a keystroke (multiple keys at the same time) is pressed.
        /// </summary>
        /// <param name="keys"> The keystroke. </param>
        /// <returns>
        ///     true if all specified keys are pressed at the same time, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         true is returned if <paramref name="keys" /> is an empty array.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="keys" /> is null. </exception>
        public static bool IsKeystrokePressed (params SystemKeyboardKey[] keys) =>
            SystemKeyboard.IsKeystrokePressed((IEnumerable<SystemKeyboardKey>)keys);

        /// <summary>
        ///     Determines whether a keystroke (multiple keys at the same time) is toggled on.
        /// </summary>
        /// <param name="keys"> The keystroke. </param>
        /// <returns>
        ///     true if all specified keys are toggled on at the same time, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         true is returned if <paramref name="keys" /> is an empty sequence.
        ///     </para>
        ///     <para>
        ///         <paramref name="keys" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="keys" /> is null. </exception>
        public static bool IsKeystrokeToggledOn (IEnumerable<SystemKeyboardKey> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            foreach (SystemKeyboardKey key in keys)
            {
                if (!SystemKeyboard.IsKeyToggledOn(key))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Determines whether a keystroke (multiple keys at the same time) is toggled on.
        /// </summary>
        /// <param name="keys"> The keystroke. </param>
        /// <returns>
        ///     true if all specified keys are toggled on at the same time, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         true is returned if <paramref name="keys" /> is an empty array.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="keys" /> is null. </exception>
        public static bool IsKeystrokeToggledOn (params SystemKeyboardKey[] keys) =>
            SystemKeyboard.IsKeystrokeToggledOn((IEnumerable<SystemKeyboardKey>)keys);

        /// <summary>
        ///     Determines whether a specified key is toggled on.
        /// </summary>
        /// <param name="key"> The key. </param>
        /// <returns>
        ///     true if the key is toggled on, false otherwise.
        /// </returns>
        public static bool IsKeyToggledOn (SystemKeyboardKey key)
        {
            short result = SystemKeyboard.GetKeyState((int)key);

            bool keyToggledOn;

            switch (result & 0x0001)
            {
                default:
                {
                    keyToggledOn = true;
                    break;
                }

                case 0:
                {
                    keyToggledOn = false;
                    break;
                }

                case 1:
                {
                    keyToggledOn = true;
                    break;
                }
            }

            return keyToggledOn;
        }

        /// <summary>
        ///     Maps characters to keyboard keys.
        /// </summary>
        /// <param name="characters"> The string of characters to map. </param>
        /// <returns>
        ///     The list of mapped keys.
        ///     An empty list is returned if no characters were provided or if no characters could be mapped.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Not all characters can be mapped to keys.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="characters" /> is null. </exception>
        public static List<SystemKeyboardKey> MapCharactersToKey (string characters) =>
            SystemKeyboard.MapCharactersToKey((IEnumerable<char>)characters.ToArray());

        /// <summary>
        ///     Maps characters to keyboard keys.
        /// </summary>
        /// <param name="characters"> The array of characters to map. </param>
        /// <returns>
        ///     The list of mapped keys.
        ///     An empty list is returned if no characters were provided or if no characters could be mapped.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Not all characters can be mapped to keys.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="characters" /> is null. </exception>
        public static List<SystemKeyboardKey> MapCharactersToKey (params char[] characters) =>
            SystemKeyboard.MapCharactersToKey((IEnumerable<char>)characters.ToArray());

        /// <summary>
        ///     Maps characters to keyboard keys.
        /// </summary>
        /// <param name="characters"> The sequence of characters to map. </param>
        /// <returns>
        ///     The list of mapped keys.
        ///     An empty list is returned if no characters were provided or if no characters could be mapped.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Not all characters can be mapped to keys.
        ///     </note>
        ///     <para>
        ///         <paramref name="characters" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="characters" /> is null. </exception>
        public static List<SystemKeyboardKey> MapCharactersToKey (IEnumerable<char> characters)
        {
            if (characters == null)
            {
                throw new ArgumentNullException(nameof(characters));
            }

            List<SystemKeyboardKey> keys = new List<SystemKeyboardKey>();

            foreach (char character in characters)
            {
                char chr = char.ToUpperInvariant(character);

                if (((chr >= 65) && (chr <= 90)) || ((chr >= 48) && (chr <= 57)))
                {
                    keys.Add((SystemKeyboardKey)chr);
                }
            }

            return keys;
        }

        [DllImport("user32.dll", SetLastError = false)]
        private static extern short GetKeyState (int nVirtKey);

        #endregion
    }
}
