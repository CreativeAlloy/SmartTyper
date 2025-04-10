using System;
using System.Collections.Generic;
using System.Threading.Tasks; // Needed for Task.Delay
using System.Windows.Forms;
using NHotkey; // Added for Hotkeys
using NHotkey.WindowsForms; // Added for Hotkeys
using WindowsInput; // Added for Input Simulation
using WindowsInput.Native; // Added for VirtualKeyCode

namespace SmartTyper
{
    public partial class Form1 : Form
    {
        // State variable to track if the feature is ON or OFF
        private bool emojiReplacementState = false;
        // Instance of the InputSimulator for sending keystrokes
        private InputSimulator inputSimulator = new InputSimulator();
        // Name for our hotkey registration
        private const string HotkeyName = "SmartEmojiTrigger";

        // The dictionary mapping text smilies to Unicode emojis
        private Dictionary<string, string> emojis = new Dictionary<string, string>
            {
                // --- Your comprehensive emoji list goes here ---
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
                // --- End of emoji list ---
            };

        public Form1()
        {
            InitializeComponent();
            SetupHotkey(); // Register the hotkey when the form starts
            UpdateUIState(); // Set initial text for button and label
        }

        // Registers the global hotkey
        private void SetupHotkey()
        {
            // Using CTRL + Space as an example hotkey. Choose what works best for you!
            // Ensure the key combination isn't commonly used by other apps.
            // Common modifiers: Keys.Control, Keys.Alt, Keys.Shift, Keys.Win
            try
            {
                HotkeyManager.Current.AddOrReplace(HotkeyName, Keys.Alt | Keys.Oemtilde, OnHotkeyActivated);
                Console.WriteLine("Hotkey Alt+` registered successfully.");
            }
            catch (HotkeyAlreadyRegisteredException ex)
            {
                MessageBox.Show($"Hotkey already registered, possibly by another application or instance. {ex.Message}", "Hotkey Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to register hotkey. {ex.Message}", "Hotkey Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // This method is called when the global hotkey (CTRL+Space) is pressed
        private async void OnHotkeyActivated(object sender, HotkeyEventArgs e)
        {
            Console.WriteLine($"OnHotkeyActivated called! State: {emojiReplacementState}");
            if (!emojiReplacementState) return;
            e.Handled = true;

            string originalClipboardText = null;
            bool clipboardContainsText = false; // Also good to declare here

            try
            {
                Console.WriteLine("Hotkey proceeding...");

                bool success = false;

                // 1. Save original clipboard content (if it's text)
                if (Clipboard.ContainsText())
                {
                    clipboardContainsText = true;
                    originalClipboardText = Clipboard.GetText();
                    Console.WriteLine($"Original clipboard had text: '{originalClipboardText}'"); // Debug
                }
                else
                {
                    Console.WriteLine("Original clipboard was empty or had non-text content."); // Debug
                }
                Clipboard.Clear(); // Clear clipboard before copy attempt

                // 2. Simulate CTRL+C (Copy selected text)
                /*inputSimulator.Keyboard.KeyDown(VirtualKeyCode.LCONTROL); // Press Left Control
                await Task.Delay(50); // Small delay between keys
                inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_C); // Press C
                await Task.Delay(50); // Small delay
                inputSimulator.Keyboard.KeyUp(VirtualKeyCode.LCONTROL); // Release Left Control
                Console.WriteLine("Simulated CTRL+C (KeyDown/Up method)"); // Update log message
                Console.WriteLine("Simulated CTRL+C");*/

                SendKeys.SendWait("^{c}"); // Send CTRL+C using SendKeys
                Console.WriteLine("Simulated CTRL+C (SendKeys method)"); // Update log message

                // 3. Wait LONGER for clipboard to update
                await Task.Delay(1000); // <--- INCREASED DELAY HERE

                // 4. Read the newly copied text from clipboard
                if (Clipboard.ContainsText())
                {
                    string selectedText = Clipboard.GetText();
                    Console.WriteLine($"Text copied: '{selectedText}'"); // Should have text now

                    if (!string.IsNullOrEmpty(selectedText))
                    {
                        // 5. Process text (apply emoji replacements)
                        string correctedText = PerformEmojiReplacements(selectedText);
                        Console.WriteLine($"Text corrected: '{correctedText}'");

                        if (correctedText != selectedText)
                        {
                            // 6. Write corrected text back to clipboard
                            Clipboard.SetText(correctedText);
                            Console.WriteLine("Corrected text set to clipboard.");

                            // Add a small delay BEFORE pasting, sometimes helps
                            await Task.Delay(50);

                            // 7. Simulate CTRL+V (Paste corrected text)
                            inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                            Console.WriteLine("Simulated CTRL+V");
                            success = true;
                        }
                        else
                        {
                            Console.WriteLine("No replacements needed.");
                            // Restore clipboard immediately if no changes
                            if (clipboardContainsText) Clipboard.SetText(originalClipboardText);
                            else Clipboard.Clear();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Clipboard was empty after copy (selectedText was null/empty).");
                        if (clipboardContainsText) Clipboard.SetText(originalClipboardText); // Restore
                        else Clipboard.Clear();
                    }
                }
                else // This is the path that was likely taken before
                {
                    Console.WriteLine("Clipboard did not contain text after copy and delay."); // Updated message
                    if (clipboardContainsText) Clipboard.SetText(originalClipboardText); // Restore
                    else Clipboard.Clear();
                }

                // Play sound based on success
                if (success)
                {
                    System.Media.SystemSounds.Asterisk.Play();
                }
                else
                {
                    System.Media.SystemSounds.Exclamation.Play();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"!!! EXCEPTION in OnHotkeyActivated: {ex.ToString()}");
                System.Media.SystemSounds.Hand.Play(); // Error sound
                                                       // Attempt to restore clipboard even on error
                try
                {
                    if (Clipboard.ContainsText() && originalClipboardText != null)
                    { // Check if originalClipboardText was captured
                        Clipboard.SetText(originalClipboardText);
                        Console.WriteLine("Original clipboard restored after exception.");
                    }
                    else if (!Clipboard.ContainsText())
                    {
                        Clipboard.Clear(); // Ensure it's clear if it was empty
                    }
                }
                catch (Exception clipEx)
                {
                    Console.WriteLine($"Error restoring clipboard after exception: {clipEx.Message}");
                }
            }
            // Removed the finally block as restoration is handled within try/catch now
        }


        // Performs the emoji replacements on a given string
        // Now takes input text and returns the modified text
        private string PerformEmojiReplacements(string inputText)
        {
            string modifiedText = inputText; // Start with the input text
            foreach (var emoji in emojis)
            {
                // Replace all occurrences of the key with the value
                modifiedText = modifiedText.Replace(emoji.Key, emoji.Value);
            }
            return modifiedText; // Return the result
        }

        // Handles the button click to toggle the feature's state
        private void button1_Click(object sender, EventArgs e)
        {
            emojiReplacementState = !emojiReplacementState; // Toggle the boolean state
            UpdateUIState(); // Update the UI elements (label and button text)
        }

        // Updates the Label and Button text based on the current state
        private void UpdateUIState()
        {
            if (emojiReplacementState)
            {
                label1.Text = "SmartEmojis are enabled! (Alt+`)"; // Update hotkey hint
                button1.Text = "Disable SmartEmojis";
            }
            else
            {
                label1.Text = "SmartEmojis are currently disabled.";
                button1.Text = "Enable SmartEmojis";
            }
        }

        // Clean up: Unregister the hotkey when the application closes
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                HotkeyManager.Current.Remove(HotkeyName);
                Console.WriteLine("Hotkey unregistered successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error unregistering hotkey: {ex.Message}");
                // Log or handle as needed, but usually safe to ignore on close
            }
            base.OnFormClosing(e);
        }

        // Remove or comment out the old TextChanged handler and replaceAllEmojis if they only reference richTextBox1
        // We no longer need the class-level richTextBoxText variable either
        /*
        string richTextBoxText = string.Empty; // No longer needed
        private void replaceAllEmojis_OLD_UI_VERSION() { ... } // Old method not needed
        private void richTextBox1_TextChanged(object sender, EventArgs e) { ... } // No longer needed for global
        */

    }
}