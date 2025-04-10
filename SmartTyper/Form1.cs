using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using NHotkey;
using NHotkey.WindowsForms;
//using WindowsInput; // No longer strictly needed if SendInput works for everything
//using WindowsInput.Native; // No longer strictly needed
using System.Runtime.InteropServices; // Needed for P/Invoke DllImport

namespace SmartTyper
{
    public partial class Form1 : Form
    {
        // P/Invoke Definitions (as listed above)
        #region P/Invoke Definitions for SendInput

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct INPUT_UNION
        {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;
            [FieldOffset(0)] public HARDWAREINPUT hi;
        }

        internal struct INPUT
        {
            public uint type; // INPUT_KEYBOARD = 1
            public INPUT_UNION U;
        }

        // Import the SendInput function from user32.dll
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        // Import GetMessageExtraInfo for dwExtraInfo (recommended)
        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        // Constants for keyboard events
        private const uint INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_KEYDOWN = 0x0000; // Key down flag
        private const uint KEYEVENTF_KEYUP = 0x0002; // Key up flag
        private const ushort VK_LCONTROL = 0xA2; // Left Control key code
        private const ushort VK_KEY_C = 0x43; // 'C' key code
        private const ushort VK_KEY_V = 0x56; // 'V' key code

        #endregion

        // State variable
        private bool emojiReplacementState = false;
        private const string HotkeyName = "SmartEmojiTrigger";

        // Emoji dictionary
        private Dictionary<string, string> emojis = new Dictionary<string, string>
            { /* --- Your full emoji list here --- */
                { ":)", "🙂" }, { ":D", "😄" }, { ":(", "🙁" }, { ":rofl:", "🤣" },
                { "^^'", "😅" }, { ":sweat:", "😅" }, { ":>", "😊" }, { ":blush:", "😊" },
                { "!!!", "😍" }, { ":heartseyes:", "😍" }, { ":P", "😋" }, { ":tongue:", "😋" },
                { ">B)", "😎" }, { ":sunglasses:", "😎" }, { ":cool:", "😎" }, { "<3", "❤" },
                { ":heart:", "❤" }, { "<!>", "🔥" }, { ":fire:", "🔥" }, { ":lit:", "🔥" },
                { ":eyes:", "👀" }, { ":relief:", "😌" }, { ":skull:", "💀" }, { ":muscle:", "💪" },
                { ":pray:", "🙏" }, { ":pleading:", "🥺" }, { ":smirk:", "😏" }, { "o/", "👋" },
                { ":wave:", "👋" }, { ":facepalm:", "🤦" }, { ";)", "😉" }, { ":/", "😕" },
                { "://", "😕" }, { ":confused:", "😕" }, { ":thinking:", "🤔" }, { ":hmm:", "🤔" },
                { ":cry:", "😢" }, { ":sad:", "😢" }, { ":disappointed:", "😢" }, { ":angry:", "😠" },
                { ":rage:", "😡" }, { ":pout:", "😡" }, { ":mad:", "😡" }, { ":tonguewink:", "😜" },
                { ":wink:", "😉" }, { ":kissheart:", "😘" }, { ":scream:", "😱" }, { ":shock:", "😱"},
                { ":sweatsmile:", "😅" }, { ":O", "😮" }, { ":o", "😮" }, { "xD", "😆" },
                { "XD", "😆" }, { "o.O", "😳" }, { "O.o", "😳" }
            };

        public Form1()
        {
            InitializeComponent();
            SetupHotkey();
            UpdateUIState();
        }

        private void SetupHotkey()
        {
            try
            {
                HotkeyManager.Current.AddOrReplace(HotkeyName, Keys.Control | Keys.Oemtilde, OnHotkeyActivated);
                Console.WriteLine("Hotkey CTRL+` registered successfully."); // Update confirmation message
            }
            catch (HotkeyAlreadyRegisteredException ex) { /* ... error handling ... */ }
            catch (Exception ex) { /* ... error handling ... */ }
        }

        // Helper function to simulate a single key press (down and up)
        private void SimulateKeyPress(ushort vkCode)
        {
            INPUT[] inputs = new INPUT[2];

            // Key Down
            inputs[0] = new INPUT();
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].U.ki.wVk = vkCode;
            inputs[0].U.ki.dwFlags = KEYEVENTF_KEYDOWN;
            inputs[0].U.ki.dwExtraInfo = GetMessageExtraInfo();

            // Key Up
            inputs[1] = new INPUT();
            inputs[1].type = INPUT_KEYBOARD;
            inputs[1].U.ki.wVk = vkCode;
            inputs[1].U.ki.dwFlags = KEYEVENTF_KEYUP;
            inputs[1].U.ki.dwExtraInfo = GetMessageExtraInfo();

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        // Helper function to simulate holding a modifier and pressing a key
        private void SimulateModifiedKeyPress(ushort modifierVkCode, ushort vkCode)
        {
            INPUT[] inputs = new INPUT[4];
            IntPtr extraInfo = GetMessageExtraInfo();

            // Modifier Down
            inputs[0] = new INPUT { type = INPUT_KEYBOARD };
            inputs[0].U.ki = new KEYBDINPUT { wVk = modifierVkCode, dwFlags = KEYEVENTF_KEYDOWN, dwExtraInfo = extraInfo };

            // Key Down
            inputs[1] = new INPUT { type = INPUT_KEYBOARD };
            inputs[1].U.ki = new KEYBDINPUT { wVk = vkCode, dwFlags = KEYEVENTF_KEYDOWN, dwExtraInfo = extraInfo };

            // Key Up
            inputs[2] = new INPUT { type = INPUT_KEYBOARD };
            inputs[2].U.ki = new KEYBDINPUT { wVk = vkCode, dwFlags = KEYEVENTF_KEYUP, dwExtraInfo = extraInfo };

            // Modifier Up
            inputs[3] = new INPUT { type = INPUT_KEYBOARD };
            inputs[3].U.ki = new KEYBDINPUT { wVk = modifierVkCode, dwFlags = KEYEVENTF_KEYUP, dwExtraInfo = extraInfo };

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }


        private async void OnHotkeyActivated(object sender, HotkeyEventArgs e)
        {
            Console.WriteLine($"OnHotkeyActivated called! State: {emojiReplacementState}");
            if (!emojiReplacementState) return;
            e.Handled = true;

            string originalClipboardText = null;
            bool clipboardContainsText = false;

            try
            {
                Console.WriteLine("Hotkey proceeding...");
                bool success = false;

                // 1. Save original clipboard
                if (Clipboard.ContainsText())
                {
                    clipboardContainsText = true;
                    originalClipboardText = Clipboard.GetText();
                    Console.WriteLine($"Original clipboard had text: '{originalClipboardText}'");
                }
                else { Console.WriteLine("Original clipboard was empty or had non-text content."); }
                Clipboard.Clear();

                // 2. Simulate CTRL+C using SendInput
                SimulateModifiedKeyPress(VK_LCONTROL, VK_KEY_C); // Use the helper
                Console.WriteLine("Simulated CTRL+C (SendInput method)");

                // 3. Wait for clipboard update (Keep the longer delay for now)
                await Task.Delay(100); // Delay the task by 100ms

                // 4. Read clipboard
                if (Clipboard.ContainsText())
                {
                    string selectedText = Clipboard.GetText();
                    Console.WriteLine($"Text copied: '{selectedText}'");

                    if (!string.IsNullOrEmpty(selectedText))
                    {
                        // 5. Process text
                        string correctedText = PerformEmojiReplacements(selectedText);
                        Console.WriteLine($"Text corrected: '{correctedText}'");

                        if (correctedText != selectedText)
                        {
                            // 6. Write back to clipboard
                            Clipboard.SetText(correctedText);
                            Console.WriteLine("Corrected text set to clipboard.");
                            await Task.Delay(50); // Delay before paste

                            // 7. Simulate CTRL+V using SendInput
                            SimulateModifiedKeyPress(VK_LCONTROL, VK_KEY_V); // Use the helper
                            Console.WriteLine("Simulated CTRL+V (SendInput method)");
                            success = true;
                        }
                        else
                        {
                            Console.WriteLine("No replacements needed.");
                            if (clipboardContainsText) Clipboard.SetText(originalClipboardText); else Clipboard.Clear();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Clipboard was empty after copy (selectedText was null/empty).");
                        if (clipboardContainsText) Clipboard.SetText(originalClipboardText); else Clipboard.Clear();
                    }
                }
                else
                {
                    Console.WriteLine("Clipboard did not contain text after copy and delay.");
                    if (clipboardContainsText) Clipboard.SetText(originalClipboardText); else Clipboard.Clear();
                }

                // Play sound
                if (success) System.Media.SystemSounds.Asterisk.Play();
                else System.Media.SystemSounds.Exclamation.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"!!! EXCEPTION in OnHotkeyActivated: {ex.ToString()}");
                try
                { // Restore clipboard in catch
                    if (clipboardContainsText && originalClipboardText != null) Clipboard.SetText(originalClipboardText);
                    else if (!clipboardContainsText) Clipboard.Clear();
                }
                catch (Exception clipEx) { Console.WriteLine($"Error restoring clipboard after exception: {clipEx.Message}"); }
                System.Media.SystemSounds.Hand.Play();
            }
        }

        // Performs emoji replacements
        private string PerformEmojiReplacements(string inputText)
        {
            string modifiedText = inputText;
            foreach (var emoji in emojis) { modifiedText = modifiedText.Replace(emoji.Key, emoji.Value); }
            return modifiedText;
        }

        // Button click handler
        private void button1_Click(object sender, EventArgs e)
        {
            emojiReplacementState = !emojiReplacementState;
            UpdateUIState();
        }

        // UI Update handler
        private void UpdateUIState()
        {
            if (emojiReplacementState)
            {
                label1.Text = "SmartEmojis are enabled! (CTRL+`)"; // Update hotkey hint
                button1.Text = "Disable SmartEmojis";
            }
            else
            {
                label1.Text = "SmartEmojis are currently disabled.";
                button1.Text = "Enable SmartEmojis";
            }
        }

        // Form closing handler
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try { HotkeyManager.Current.Remove(HotkeyName); Console.WriteLine("Hotkey unregistered successfully."); }
            catch (Exception ex) { Console.WriteLine($"Error unregistering hotkey: {ex.Message}"); }
            base.OnFormClosing(e);
        }
    }
}