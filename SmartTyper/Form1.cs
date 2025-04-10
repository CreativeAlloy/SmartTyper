using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartTyper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Dictionary<string, string> emojis = new Dictionary<string, string>
            {
                // This dictionary contains the Keys (the text representations of emojis that will be changed into their unicode counterparts) and the Values (the unicode emojis themselves).

                { ":)", "🙂" },
                { ":D", "😄" },
                { ":(", "🙁" },
                { ":rofl:", "🤣" },
                { "^^'", "😅" },
                { ":sweat:", "😅" }, // Added :sweat: as alternative - some emojis can have alternative Key values.
                { ":>", "😊" },
                { ":blush:", "😊" },
                { "!!!", "😍" },
                { ":heartseyes:", "😍" },
                { ":P", "😋" },
                { ":tongue:", "😋" },
                { ">B)", "😎" },
                { ":sunglasses:", "😎" },
                { ":cool:", "😎" },
                { "<3", "❤" },
                { ":heart:", "❤" },
                { "<!>", "🔥" },
                { ":fire:", "🔥" },
                { ":lit:", "🔥" },
                { ":eyes:", "👀" },
                { ":relief:", "😌" },
                { ":skull:", "💀" },
                { ":muscle:", "💪" },
                { ":pray:", "🙏" },
                { ":pleading:", "🥺" },
                { ":smirk:", "😏" },
                { "o/", "👋" },
                { ":wave:", "👋" },
                { ":facepalm:", "🤦" }, // Neutral facepalm chosen.
                { ";)", "😉" },
                { ":/", "😕" },
                { "://", "😕" }, // Alternative for :/
                { ":confused:", "😕" },
                { ":thinking:", "🤔" },
                { ":hmm:", "🤔" }, // Alternative for :thinking:
                { ":cry:", "😢" },
                { ":sad:", "😢" }, // Alternative for :cry:
                { ":disappointed:", "😢" }, // Alternative for :cry:
                { ":angry:", "😠" },
                { ":rage:", "😡" },
                { ":pout:", "😡" }, // Alternative for :rage:
                { ":mad:", "😡" }, // Alternative for :rage:
                { ":tonguewink:", "😜" },
                { ":wink:", "😉" },
                { ":kissheart:", "😘" },
                { ":scream:", "😱" },
                { ":shock:", "😱"}, // Alternative for :scream:
                { ":sweatsmile:", "😅" }, // Alternative for :sweat:
                { ":O", "😮" },
                { ":o", "😮" },
                { "xD", "😆" },
                { "XD", "😆" },
                { "o.O", "😳" },
                { "O.o", "😳" }
            };

        string richTextBoxText = string.Empty;
        bool emojiReplacementState = false;

        private void replaceAllEmojis() {

            int currentSelection = richTextBox1.SelectionStart;

            foreach (var emoji in emojis)
            {
                if (richTextBoxText.Contains(emoji.Key))
                {
                    richTextBoxText = richTextBoxText.Replace(emoji.Key, emoji.Value);

                    // This section runs through all emoji entries and checks if the emoji is already in the text. If it is, it replaces the emoji with its unicode counterpart.
                }
            }

            richTextBox1.Text = richTextBoxText;
            richTextBox1.SelectionStart = currentSelection;
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBoxText = richTextBox1.Text;

            if (emojiReplacementState)
            {
                replaceAllEmojis();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!emojiReplacementState)
            {
                emojiReplacementState = true;
                label1.Text = "SmartEmojis have been enabled!";
                replaceAllEmojis();
            } else
            {
                emojiReplacementState = false;
                label1.Text = "SmartEmojis are currently disabled.";
            }
        }
    }
}
