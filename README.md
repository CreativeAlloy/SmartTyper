# SmartTyper
Writing is an integral part of using computers in our daily lives. Nevertheless, despite being accustomed to keyboard layouts, it's possible for each and every person to mistype occasionally, and there are various reasons for this.
SmartTyper's main goal is to fix typos within any selected text using a tiny local LLM (Large Language Model).
Alternatively, SmartTyper has a separate but logically adjacent function, which I've titled SmartEmojis.

## SmartEmojis
As of writing this readme (April 10, 2025), only SmartEmojis have been implemented. For the sake of being fully transparent, I'd like to share that the initial, barebones implementation of SmartEmojis, which comprises a richTextBox and a button that toggles the automatic replacement of text representations of emojis with their unicode character counterparts, was written by me for the most part.
Even so, making sure this function works on any text field is Gemini 2.5 Pro Preview 03-25's work.

The key difference between the two implementations is that the initial richTextBox isolated environment supports live replacement of emojis. I'm not entirely sure how this could be done outside of that environment, so my workaround is as follows:
1. Select any text using your cursor or CTRL+A to select all text in your active textbox. This part is akin to the way I envision the functionality of SmartTyper - in principle, the action performed by the user is exactly the same.
2. Press CTRL+Space (once you've activated SmartEmojis on the WinForms app).
3. This should replace all occuring text representations with their emoji counterparts.

This approach mitigates a potential issue that I would've otherwise encountered, had I decided to perform these updates in real time - figuring out where the user is typing.
I might work on this further over time!
