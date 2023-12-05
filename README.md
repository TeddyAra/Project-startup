# Project-startup

The build will most likely not work! This is because the ip override needs to be renewed, and you need to open a browser tab to get the play code.

# Getting a Cloudflared tunnel

Find the LocalIPOverride.txt file in the Assets folder. This file should have a cloudflared link.

To get a new link, you need to download cloudflared. Follow ONLY the first step of the following link:
https://developers.cloudflare.com/cloudflare-one/connections/connect-networks/get-started/create-local-tunnel/

Now, in PowerShell, relocate to the folder where cloudflared.exe has been installed to. 
This will most likely be your downloads folder (cd Downloads).

Then type .\cloudflared.exe tunnel --url http://localhost:7842/
This will generate a new link, which you can put in the LocalIPOverride.txt file.

# Playing the game

You can now play the game by clicking the .exe file. You'll be stuck on the front screen without a code though.
You need to connect to AirConsole and have at least one virtual device. To do this, you need to open a link.

The link looks like this, with the brackets being replaced with the link you made earlier.
https://www.airconsole.com/simulator/#(THE LOCAL IP OVERRIDE)/?unity-editor-websocket-port=7843&unity-plugin-version=2.14
