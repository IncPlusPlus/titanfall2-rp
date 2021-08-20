# Map Previews

Here's how to extract these images from the game. The square ones are cropped manually but the 1080p ones are ripped straight from the game.

1. Go to Titanfall2\r3\paks\Win64
2. In that directory do `ls | grep -E "_loadscreen.*\.rpak"` and for each entry in the list
   1. Open that `.rpak` file in [Legion](https://wiki.modme.co/wiki/apps/Legion.html)
      - If there are multiple (like how there's a `mp_glitch_loadscreen(04).rpak` all the way through `mp_glitch_loadscreen(11).rpak`), just pick one. The highest numbered is fine.
   2. Export the "image"
   3. Rename the image (it'll end in `.dds`) to the map name
   4. Open the DDS in [paint.net](getpaint.net)
   5. Export it as a PNG