# Project-startup

There are a few things you need to do to play this game! In short, you need to renew a https link every now and then due to a known CloudFlared issue with non-secured connections.
To make sure you do have a secured connection, and AirConsole functions properly, you have to generate a CloudFlared link.

# Getting a Cloudflared tunnel

To get a new link, you need to download CloudFlared. Follow ONLY the first step of the following link:
https://developers.cloudflare.com/cloudflare-one/connections/connect-networks/get-started/create-local-tunnel/

Now, in PowerShell, relocate to the folder where cloudflared.exe has been installed to. 
This will most likely be your downloads folder (cd Downloads).

Then type `.\cloudflared.exe tunnel --url http://localhost:7842/`
This will generate a new link, which you have to copy.

# Playing the game

You can now play the game by clicking the .exe file, as per usual. You'll be stuck on the front screen without a code though.
You need to connect to AirConsole and have at least one virtual device. 

To do this, you need to put the generated link in the input field on the bottom right. Make sure it doesn't have any spaces or anything like that.
After this, press enter. A browser tab should open that displays two virtual devices. Keep it open for a moment to see if your link works.

If the virtual devices load for too long, you've pasted the link incorrectly or the link has expired. To be safe, you can generate a new link.
If the virtual devices do load, the link is valid. You can alt+tab back to Unity, and the room code should be displayed in the UI.
